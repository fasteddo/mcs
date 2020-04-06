// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.std.vector<mame.netlist.analog_net_t>;
using netlist_base_t = mame.netlist.netlist_state_t;
using netlist_time = mame.netlist.ptime_u64;  //using netlist_time = ptime<std::uint64_t, NETLIST_INTERNAL_RES>;
using nl_double = System.Double;


// ----------------------------------------------------------------------------------------
// solver
// ----------------------------------------------------------------------------------------
namespace mame.netlist
{
    namespace devices
    {
        //class NETLIB_NAME(solver);
        //class matrix_solver_t;


        //NETLIB_OBJECT(solver)
        class nld_solver : device_t
        {
            //NETLIB_DEVICE_IMPL(solver, "SOLVER", "FREQ")
            static factory.element_t nld_solver_c(string classname)
            { return new factory.device_element_t<nld_solver>("SOLVER", classname, "FREQ", "__FILE__"); }
            public static factory.constructor_ptr_t decl_solver = nld_solver_c;


            logic_input_t m_fb_step;
            logic_output_t m_Q_step;

            std.vector<matrix_solver_t> m_mat_solvers;  //std::vector<plib::unique_ptr<matrix_solver_t>> m_mat_solvers;
            std.vector<matrix_solver_t> m_mat_solvers_all = new std.vector<matrix_solver_t>();  //std::vector<std::unique_ptr<matrix_solver_t>> m_mat_solvers;
            std.vector<matrix_solver_t> m_mat_solvers_timestepping = new std.vector<matrix_solver_t>();  //std::vector<matrix_solver_t *> m_mat_solvers_timestepping;

            solver_parameters_t m_params;


            //NETLIB_CONSTRUCTOR(solver)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_solver(object owner, string name)
                : base(owner, name)
            {
                m_fb_step = new logic_input_t(this, "FB_step");
                m_Q_step = new logic_output_t(this, "Q_step");
                m_params = new solver_parameters_t(this);


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
                    matrix_solver_t ms;
                    UInt32 net_count = (UInt32)grp.size();
                    string sname = new plib.pfmt("Solver_{0}").op(m_mat_solvers.size());

                    switch (net_count)
                    {
                        case 1:
                            throw new emu_unimplemented();
                            ms = new matrix_solver_direct1_t(state(), sname, m_params);  //ms = plib::make_unique<matrix_solver_direct1_t<double>>(state(), sname, &m_params);
                            break;
                        case 2:
                            throw new emu_unimplemented();
                            ms = new matrix_solver_direct2_t(state(), sname, m_params);  //ms = plib::make_unique<matrix_solver_direct2_t<double>>(state(), sname, &m_params);
                            break;
#if true
#if false
                        case 3:
                            ms = create_solver<double, 3>(3, sname);
                            break;
                        case 4:
                            ms = create_solver<double, 4>(4, sname);
                            break;
                        case 5:
                            ms = create_solver<double, 5>(5, sname);
                            break;
                        case 6:
                            ms = create_solver<double, 6>(6, sname);
                            break;
                        case 7:
                            ms = create_solver<double, 7>(7, sname);
                            break;
                        case 8:
                            ms = create_solver<double, 8>(8, sname);
                            break;
                        case 9:
                            ms = create_solver<double, 9>(9, sname);
                            break;
                        case 10:
                            ms = create_solver<double, 10>(10, sname);
                            break;
#if false
                        case 11:
                            ms = create_solver<double, 11>(11, sname);
                            break;
                        case 12:
                            ms = create_solver<double, 12>(12, sname);
                            break;
                        case 15:
                            ms = create_solver<double, 15>(15, sname);
                            break;
                        case 31:
                            ms = create_solver<double, 31>(31, sname);
                            break;
                        case 35:
                            ms = create_solver<double, 35>(35, sname);
                            break;
                        case 43:
                            ms = create_solver<double, 43>(43, sname);
                            break;
                        case 49:
                            ms = create_solver<double, 49>(49, sname);
                            break;
#endif
#endif
#if false
#if true
                        case 86:
                            ms = create_solver<double,86>(86, sname);
                            break;
#endif
#endif
#endif
                        default:
                            throw new emu_unimplemented();
#if false
                            log().info.op(nl_errstr_global.MI_NO_SPECIFIC_SOLVER(net_count));
                            if (net_count <= 8)
                            {
                                ms = create_solver/*<double, -8>*/(-8, net_count, sname);
                            }
                            else if (net_count <= 16)
                            {
                                ms = create_solver/*<double, -16>*/(-16, net_count, sname);
                            }
                            else if (net_count <= 32)
                            {
                                ms = create_solver/*<double, -32>*/(-32, net_count, sname);
                            }
                            else if (net_count <= 64)
                            {
                                ms = create_solver/*<double, -64>*/(-64, net_count, sname);
                            }
                            else if (net_count <= 128)
                            {
                                ms = create_solver/*<double, -128>*/(-128, net_count, sname);
                            }
                            else if (net_count <= 256)
                            {
                                ms = create_solver/*<double, -256>*/(-256, net_count, sname);
                            }
                            else if (net_count <= 512)
                            {
                                ms = create_solver/*<double, -512>*/(-512, net_count, sname);
                            }
                            else
                            {
                                ms = create_solver/*<double, 0>*/(0, net_count, sname);
                            }
                            break;
#endif
                    }

                    // FIXME ...
                    ms.setup(grp);

                    log().verbose.op("Solver {0}", ms.name());
                    log().verbose.op("       ==> {0} nets", grp.size());
                    log().verbose.op("       has {0} elements", ms.has_dynamic_devices() ? "dynamic" : "no dynamic");
                    log().verbose.op("       has {0} elements", ms.has_timestep_devices() ? "timestep" : "no timestep");

                    foreach (var n in grp)
                    {
                        log().verbose.op("Net {0}", n.name());
                        foreach (var pcore in n.core_terms())
                        {
                            log().verbose.op("   {0}", pcore.name());
                        }
                    }

                    m_mat_solvers_all.push_back(ms);
                    if (ms.has_timestep_devices())
                        m_mat_solvers_timestepping.push_back(ms);

                    m_mat_solvers.emplace_back(ms);
                }
            }


            public void stop()
            {
                foreach (var s in m_mat_solvers)
                    s.log_stats();
            }


            public nl_double gmin() { return m_params.m_gmin.op; }


            //void create_solver_code(std::map<pstring, pstring> &mp);


            //NETLIB_UPDATE(solver)
            protected override void update()
            {
                if (m_params.m_dynamic_ts.op)
                    return;

                netlist_time now = exec().time();
                /* force solving during start up if there are no time-step devices */
                /* FIXME: Needs a more elegant solution */
                bool force_solve = (now < netlist_time.from_double(2 * m_params.m_max_timestep));

                int nthreads = std.min(m_params.m_parallel.op, plib.omp.pomp_global.get_max_threads());

                std.vector<matrix_solver_t> solvers = force_solve ? m_mat_solvers_all : m_mat_solvers_timestepping;

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

                /* step circuit */
                if (!m_Q_step.net().is_queued())
                {
                    m_Q_step.net().toggle_and_push_to_queue(netlist_time.from_double(m_params.m_max_timestep));
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


            //template <typename FT, int SIZE>
            matrix_solver_t create_solver(int SIZE, UInt32 size, string solvername)  //plib::unique_ptr<matrix_solver_t> create_solver(std::size_t size, const pstring &solvername);
            {
                switch (m_params.m_method.op)
                {
                    case matrix_type_e.MAT_CR:
                        if (size > 0) // GCR always outperforms MAT solver
                        {
                            return new matrix_solver_GCR_t(SIZE, state(), solvername, m_params, size);  //return create_it<matrix_solver_SOR_mat_t<FT, SIZE>>(state(), solvername, m_params, size);
                        }
                        else
                        {
                            throw new emu_unimplemented();
                        }
                    case matrix_type_e.SOR_MAT:
                        throw new emu_unimplemented();
                    case matrix_type_e.MAT:
                        throw new emu_unimplemented();
                    case matrix_type_e.SM:
                        throw new emu_unimplemented();
                    case matrix_type_e.W:
                        throw new emu_unimplemented();
                    case matrix_type_e.SOR:
                        throw new emu_unimplemented();
                    case matrix_type_e.GMRES:
                        throw new emu_unimplemented();
                }

                return null;  //return plib::unique_ptr<matrix_solver_t>();
            }
        }


        class net_splitter
        {
            public std.vector<analog_net_t_list_t> groups = new std.vector<analog_net_t_list_t>();


            bool already_processed(analog_net_t n)
            {
                /* no need to process rail nets - these are known variables */
                if (n.isRailNet())
                    return true;
                /* if it's already processed - no need to continue */
                foreach (var grp in groups)
                    if (grp.Contains(n))  //plib::container::contains(grp, n))
                        return true;
                return false;
            }


            void process_net(analog_net_t n)
            {
                /* ignore empty nets. FIXME: print a warning message */
                if (n.num_cons() == 0)
                    return;
                /* add the net */
                groups.back().push_back(n);
                /* process all terminals connected to this net */
                foreach (var term in n.core_terms())
                {
                    /* only process analog terminals */
                    if (term.is_type(detail.terminal_type.TERMINAL))
                    {
                        var pt = (terminal_t)term;
                        /* check the connected terminal */
                        analog_net_t connected_net = pt.connected_terminal().net();
                        if (!already_processed(connected_net))
                            process_net(connected_net);
                    }
                }
            }


            public void run(netlist_state_t netlist)
            {
                foreach (var net in netlist.nets())
                {
                    netlist.log().debug.op("processing {0}\n", net.name());
                    if (!net.isRailNet() && net.num_cons() > 0)
                    {
                        netlist.log().debug.op("   ==> not a rail net\n");
                        /* Must be an analog net */
                        var n = (analog_net_t)net;
                        if (!already_processed(n))
                        {
                            groups.emplace_back(new analog_net_t_list_t());
                            process_net(n);
                        }
                    }
                }
            }
        }
    } //namespace devices
} // namespace netlist
