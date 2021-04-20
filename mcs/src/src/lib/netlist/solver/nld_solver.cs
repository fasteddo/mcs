// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_time = mame.plib.ptime_i64;  //using netlist_time = plib::ptime<std::int64_t, NETLIST_INTERNAL_RES>;
using netlist_time_ext = mame.plib.ptime_i64;  //netlist_time
using nl_fptype = System.Double;
using size_t = System.UInt32;


// ----------------------------------------------------------------------------------------
// solver
// ----------------------------------------------------------------------------------------
namespace mame.netlist
{
    namespace devices
    {
        //NETLIB_OBJECT(solver)
        class nld_solver : device_t
        {
            //NETLIB_DEVICE_IMPL(solver, "SOLVER", "FREQ")
            public static readonly factory.constructor_ptr_t decl_solver = NETLIB_DEVICE_IMPL<nld_solver>("SOLVER", "FREQ");


            logic_input_t m_fb_step;
            logic_output_t m_Q_step;

            std.vector<solver.matrix_solver_t> m_mat_solvers = new std.vector<solver.matrix_solver_t>();  //std::vector<plib::unique_ptr<solver::matrix_solver_t>> m_mat_solvers;
            std.vector<solver.matrix_solver_t> m_mat_solvers_all = new std.vector<solver.matrix_solver_t>();  //std::vector<solver::matrix_solver_t *> m_mat_solvers_all;
            std.vector<solver.matrix_solver_t> m_mat_solvers_timestepping = new std.vector<solver.matrix_solver_t>();  //std::vector<solver::matrix_solver_t *> m_mat_solvers_timestepping;

            solver.solver_parameters_t m_params;


            //NETLIB_CONSTRUCTOR(solver)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_solver(object owner, string name)
                : base(owner, name)
            {
                m_fb_step = new logic_input_t(this, "FB_step");
                m_Q_step = new logic_output_t(this, "Q_step");
                m_params = new solver.solver_parameters_t(this);


                // internal staff

                connect(m_fb_step, m_Q_step);
            }


            public void post_start()
            {
                log().verbose.op("Scanning net groups ...");
                // determine net groups

                net_splitter splitter = new net_splitter();

                splitter.run(state());

                // setup the solvers
                log().verbose.op("Found {0} net groups in {1} nets\n", splitter.groups.size(), state().nets().size());
                foreach (var grp in splitter.groups)
                {
                    solver.matrix_solver_t ms = null;
                    string sname = new plib.pfmt("Solver_{0}").op(m_mat_solvers.size());

                    switch (m_params.m_fp_type.op())
                    {
                        case solver.matrix_fp_type_e.FLOAT:
#if (NL_USE_FLOAT_MATRIX)
                            ms = create_solvers<float>(sname, grp);
#else
                            log().info.op("FPTYPE {0} not supported. Using DOUBLE", m_params.m_fp_type.op().ToString());
                            ms = create_solvers/*<double>*/(sname, grp);
#endif
                            break;
                        case solver.matrix_fp_type_e.DOUBLE:
                            ms = create_solvers/*<double>*/(sname, grp);
                            break;
                        case solver.matrix_fp_type_e.LONGDOUBLE:
#if (NL_USE_LONG_DOUBLE_MATRIX)
                            ms = create_solvers<long double>(sname, grp);
#else
                            log().info.op("FPTYPE {0} not supported. Using DOUBLE", m_params.m_fp_type.op().ToString());
                            ms = create_solvers/*<double>*/(sname, grp);
#endif
                            break;
                        case solver.matrix_fp_type_e.FLOATQ128:
#if (NL_USE_FLOAT128)
                            ms = create_solvers<FLOAT128>(sname, grp);
#else
                            log().info.op("FPTYPE {0} not supported. Using DOUBLE", m_params.m_fp_type.op().ToString());
                            ms = create_solvers/*<double>*/(sname, grp);
#endif
                            break;
                    }

                    log().verbose.op("Solver {0}", ms.name());
                    log().verbose.op("       ==> {0} nets", grp.size());
                    log().verbose.op("       has {0} dynamic elements", ms.dynamic_device_count());
                    log().verbose.op("       has {0} timestep elements", ms.timestep_device_count());
                    foreach (var n in grp)
                    {
                        log().verbose.op("Net {0}", n.name());
                        foreach (var pcore in n.core_terms())
                        {
                            log().verbose.op("   {0}", pcore.name());
                        }
                    }

                    m_mat_solvers_all.push_back(ms);
                    if (ms.timestep_device_count() != 0)
                        m_mat_solvers_timestepping.push_back(ms);

                    m_mat_solvers.emplace_back(ms);
                }
            }


            //template <typename FT>
            solver.matrix_solver_t create_solvers(string sname, analog_net_t.list_t nets)  //plib::unique_ptr<solver::matrix_solver_t> NETLIB_NAME(solver)::create_solvers(const pstring &sname, analog_net_t::list_t &nets)
            {
                size_t net_count = (size_t)nets.size();
                switch (net_count)
                {
                    case 1:
                        return new solver.matrix_solver_direct1_t(state(), sname, nets, m_params);  //return plib::make_unique<solver::matrix_solver_direct1_t<FT>>(state(), sname, nets, &m_params);
                        break;
                    case 2:
                        throw new emu_unimplemented();
                        return new solver.matrix_solver_direct2_t(state(), sname, nets, m_params);  //return plib::make_unique<solver::matrix_solver_direct2_t<FT>>(state(), sname, nets, &m_params);
                        break;
                    case 3:
                        return create_solver/*<FT, 3>*/(3, 3, sname, nets);
                        break;
                    case 4:
                        return create_solver/*<FT, 4>*/(4, 4, sname, nets);
                        break;
                    case 5:
                        return create_solver/*<FT, 5>*/(5, 5, sname, nets);
                        break;
                    case 6:
                        return create_solver/*<FT, 6>*/(6, 6, sname, nets);
                        break;
                    case 7:
                        return create_solver/*<FT, 7>*/(7, 7, sname, nets);
                        break;
                    case 8:
                        return create_solver/*<FT, 8>*/(8, 8, sname, nets);
                        break;
                    default:
                        log().info.op(nl_errstr_global.MI_NO_SPECIFIC_SOLVER(net_count));
                        if (net_count <= 16)
                        {
                            return create_solver/*<FT, -16>*/(-16, net_count, sname, nets);
                        }
                        if (net_count <= 32)
                        {
                            return create_solver/*<FT, -32>*/(-32, net_count, sname, nets);
                        }
                        if (net_count <= 64)
                        {
                            return create_solver/*<FT, -64>*/(-64, net_count, sname, nets);
                        }
                        if (net_count <= 128)
                        {
                            return create_solver/*<FT, -128>*/(-128, net_count, sname, nets);
                        }
                        if (net_count <= 256)
                        {
                            return create_solver/*<FT, -256>*/(-256, net_count, sname, nets);
                        }
                        if (net_count <= 512)
                        {
                            return create_solver/*<FT, -512>*/(-512, net_count, sname, nets);
                        }

                        return create_solver/*<FT, 0>*/(0, net_count, sname, nets);
                        break;
                }
            }


            public void stop()
            {
                foreach (var s in m_mat_solvers)
                    s.log_stats();
            }


            public nl_fptype gmin() { return m_params.m_gmin.op(); }


            //solver::static_compile_container create_solver_code(solver::static_compile_target target);


            //NETLIB_UPDATE(solver)
            public override void update()
            {
                if (m_params.m_dynamic_ts.op())
                    return;

                netlist_time_ext now = exec().time();
                // force solving during start up if there are no time-step devices
                // FIXME: Needs a more elegant solution
                bool force_solve = (now < netlist_time_ext.from_fp(2 * m_params.m_max_timestep));

                int nthreads = std.min(m_params.m_parallel.op(), plib.omp.pomp_global.get_max_threads());

                std.vector<solver.matrix_solver_t> solvers = force_solve ? m_mat_solvers_all : m_mat_solvers_timestepping;

                if (nthreads > 1 && solvers.size() > 1)
                {
                    throw new emu_unimplemented();
#if false
                    plib::omp::set_num_threads(nthreads);
                    plib::omp::for_static(static_cast<std::size_t>(0), solvers.size(), [&solvers, now](std::size_t i)
                        {
                            const netlist_time ts = solvers[i]->solve(now);
                            plib::unused_var(ts);
                        });
#endif
                }
                else
                {
                    foreach (var solver in solvers)
                    {
                        netlist_time ts = solver.solve(now);
                    }
                }

                foreach (var solver in solvers)
                {
                    solver.update_inputs();
                }

                // step circuit
                if (!m_Q_step.net().is_queued())
                {
                    m_Q_step.net().toggle_and_push_to_queue(netlist_time.from_fp(m_params.m_max_timestep));
                }
            }


            //NETLIB_RESETI();
            //NETLIB_RESET(solver)
            public override void reset()
            {
                foreach (var s in m_mat_solvers)
                    s.reset();
            }

            // NETLIB_UPDATE_PARAMI();


            //template <class C>
            //plib::unique_ptr<solver::matrix_solver_t> create_it(netlist_state_t &nl, pstring name,
            //    analog_net_t::list_t &nets,
            //    solver::solver_parameters_t &params, std::size_t size)
            //{
            //    return plib::make_unique<C>(nl, name, nets, &params, size);
            //}


            //template <typename FT, int SIZE>
            solver.matrix_solver_t create_solver(int SIZE, size_t size, string solvername, analog_net_t.list_t nets)  //plib::unique_ptr<solver::matrix_solver_t> create_solver(std::size_t size, const pstring &solvername, analog_net_t::list_t &nets);
            {
                switch (m_params.m_method.op())
                {
                    case solver.matrix_type_e.MAT_CR:
                        return new solver.matrix_solver_GCR_t(SIZE, state(), solvername, nets, m_params, size);  //return create_it<solver::matrix_solver_GCR_t<FT, SIZE>>(state(), solvername, nets, m_params, size);
                    case solver.matrix_type_e.MAT:
                        throw new emu_unimplemented();  //return create_it<solver::matrix_solver_direct_t<FT, SIZE>>(state(), solvername, nets, m_params, size);
#if NL_USE_ACADEMIC_SOLVERS
                    case solver::matrix_type_e::GMRES:
                        return create_it<solver::matrix_solver_GMRES_t<FT, SIZE>>(state(), solvername, nets, m_params, size);
                    case solver::matrix_type_e::SOR:
                        return create_it<solver::matrix_solver_SOR_t<FT, SIZE>>(state(), solvername, nets, m_params, size);
                    case solver::matrix_type_e::SOR_MAT:
                        return create_it<solver::matrix_solver_SOR_mat_t<FT, SIZE>>(state(), solvername, nets, m_params, size);
                    case solver.matrix_type_e.SM:
                        // Sherman-Morrison Formula
                        throw new emu_unimplemented();  //return create_it<solver::matrix_solver_sm_t<FT, SIZE>>(state(), solvername, nets, m_params, size);
                    case solver.matrix_type_e.W:
                        // Woodbury Formula
                        throw new emu_unimplemented();  //return create_it<solver::matrix_solver_w_t<FT, SIZE>>(state(), solvername, nets, m_params, size);
#else
                    default:
                        state().log().warning.op(nl_errstr_global.MW_SOLVER_METHOD_NOT_SUPPORTED(m_params.m_method.op().ToString(), "MAT_CR"));
                        throw new emu_unimplemented();  //return create_it<solver::matrix_solver_GCR_t<FT, SIZE>>(state(), solvername, nets, m_params, size);
#endif
                }

                throw new emu_unimplemented();  //return plib::unique_ptr<solver::matrix_solver_t>();
            }


            //template <typename FT>
            //plib::unique_ptr<solver::matrix_solver_t> create_solvers(const pstring &sname, analog_net_t::list_t &nets);
        }


        class net_splitter
        {
            public std.vector<analog_net_t.list_t> groups = new std.vector<analog_net_t.list_t>();

            std.vector<analog_net_t.list_t> groupspre = new std.vector<analog_net_t.list_t>();


            public void run(netlist_state_t netlist)
            {
                foreach (var net in netlist.nets())
                {
                    netlist.log().verbose.op("processing {0}", net.name());
                    if (!net.is_rail_net() && net.has_connections())
                    {
                        netlist.log().verbose.op("   ==> not a rail net");
                        // Must be an analog net
                        var n = (analog_net_t)net;
                        if (!already_processed(n))
                        {
                            groupspre.emplace_back(new analog_net_t.list_t());
                            process_net(netlist, n);
                        }
                    }
                }

                foreach (var g in groupspre)
                    if (!g.empty())
                        groups.push_back(g);
            }


            bool already_processed(analog_net_t n)
            {
                // no need to process rail nets - these are known variables
                if (n.is_rail_net())
                    return true;
                // if it's already processed - no need to continue
                foreach (var grp in groups)
                    if (grp.Contains(n))  //plib::container::contains(grp, n))
                        return true;
                return false;
            }


            bool check_if_processed_and_join(analog_net_t n)
            {
                // no need to process rail nets - these are known variables
                if (n.is_rail_net())
                    return true;

                // First check if it is in a previous group.
                // In this case we need to merge this group into the current group
                if (groupspre.size() > 1)
                {
                    for (UInt32 i = 0; i < groupspre.size() - 1; i++)
                    {
                        if (plib.container.contains(groupspre[i], n))
                        {
                            // copy all nets
                            foreach (var cn in groupspre[i])
                                if (!plib.container.contains(groupspre.back(), cn))
                                    groupspre.back().push_back(cn);

                            // clear
                            groupspre[i].clear();
                            return true;
                        }
                    }
                }

                // if it's already processed - no need to continue
                if (!groupspre.empty() && plib.container.contains(groupspre.back(), n))
                    return true;

                return false;
            }


            void process_net(netlist_state_t netlist, analog_net_t n)
            {
                // ignore empty nets. FIXME: print a warning message
                netlist.log().verbose.op("Net {0}", n.name());
                if (n.has_connections())
                {
                    // add the net
                    groupspre.back().push_back(n);
                    // process all terminals connected to this net
                    foreach (var term in n.core_terms())
                    {
                        netlist.log().verbose.op("Term {0} {1}", term.name(), (int)term.type());
                        // only process analog terminals
                        if (term.is_type(detail.terminal_type.TERMINAL))
                        {
                            var pt = (terminal_t)term;
                            // check the connected terminal
                            analog_net_t connected_net = netlist.setup().get_connected_terminal(pt).net();
                            netlist.log().verbose.op("  Connected net {0}", connected_net.name());
                            if (!check_if_processed_and_join(connected_net))
                                process_net(netlist, connected_net);
                        }
                    }
                }
            }
        }
    } //namespace devices
} // namespace netlist
