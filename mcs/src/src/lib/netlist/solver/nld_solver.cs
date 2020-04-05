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

            param_double_t m_freq;
            param_double_t m_gs_sor;
            param_str_t m_method;
            param_double_t m_accuracy;
            param_int_t m_gs_loops;
            param_double_t m_gmin;
            param_logic_t  m_pivot;
            param_int_t m_nr_loops;
            param_double_t m_nr_recalc_delay;
            param_int_t m_parallel;
            param_logic_t  m_dynamic_ts;
            param_double_t m_dynamic_lte;
            param_double_t m_dynamic_min_ts;

            param_logic_t m_use_gabs;
            param_logic_t m_use_linear_prediction;

            param_logic_t m_log_stats;

            std.vector<matrix_solver_t> m_mat_solvers;  //std::vector<poolptr<matrix_solver_t>> m_mat_solvers;
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
                m_freq = new param_double_t(this, "FREQ", 48000.0);

                /* iteration parameters */
                m_gs_sor = new param_double_t(this, "SOR_FACTOR", 1.059);
                m_method = new param_str_t(this, "METHOD", "MAT_CR");
                m_accuracy = new param_double_t(this, "ACCURACY", 1e-7);
                m_gs_loops = new param_int_t(this, "GS_LOOPS", 9);              // Gauss-Seidel loops

                /* general parameters */
                m_gmin = new param_double_t(this, "GMIN", 1e-9);
                m_pivot = new param_logic_t(this, "PIVOT", false);                    // use pivoting - on supported solvers
                m_nr_loops = new param_int_t(this, "NR_LOOPS", 250);            // Newton-Raphson loops
                m_nr_recalc_delay = new param_double_t(this, "NR_RECALC_DELAY", netlist_time.NLTIME_FROM_NS(10).as_double());  // Delay to next solve attempt if nr loops exceeded
                m_parallel = new param_int_t(this, "PARALLEL", 0);

                /* automatic time step */
                m_dynamic_ts = new param_logic_t(this, "DYNAMIC_TS", false);
                m_dynamic_lte = new param_double_t(this, "DYNAMIC_LTE", 1e-5);                     // diff/timestep
                m_dynamic_min_ts = new param_double_t(this, "DYNAMIC_MIN_TIMESTEP", 1e-6);   // nl_double timestep resolution

                /* special */
                m_use_gabs = new param_logic_t(this, "USE_GABS", true);
                m_use_linear_prediction = new param_logic_t(this, "USE_LINEAR_PREDICTION", false); // // savings are eaten up by effort

                m_log_stats = new param_logic_t(this, "LOG_STATS", true);   // log statistics on shutdown
                m_params = new solver_parameters_t();


                // internal staff

                connect(m_fb_step, m_Q_step);
            }


            public void post_start()
            {
                m_params.m_pivot = m_pivot.op();
                m_params.m_accuracy = m_accuracy.op();
                /* FIXME: Throw when negative */
                m_params.m_gs_loops = (UInt32)m_gs_loops.op();
                m_params.m_nr_loops = (UInt32)m_nr_loops.op();
                m_params.m_nr_recalc_delay = netlist_time.from_double(m_nr_recalc_delay.op());
                m_params.m_dynamic_lte = m_dynamic_lte.op();
                m_params.m_gs_sor = m_gs_sor.op();

                m_params.m_min_timestep = m_dynamic_min_ts.op();
                m_params.m_dynamic_ts = (m_dynamic_ts.op() == true ? true : false);
                m_params.m_max_timestep = netlist_time.from_double(1.0 / m_freq.op()).as_double();

                m_params.m_use_gabs = m_use_gabs.op();
                m_params.m_use_linear_prediction = m_use_linear_prediction.op();


                if (m_params.m_dynamic_ts)
                {
                    m_params.m_max_timestep *= 1;//NL_FCONST(1000.0);
                }
                else
                {
                    m_params.m_min_timestep = m_params.m_max_timestep;
                }

                //m_params.m_max_timestep = std::max(m_params.m_max_timestep, m_params.m_max_timestep::)

                // Override log statistics
                string p = "";  //plib::util::environment("NL_STATS", "");
                if (p != "")
                    m_params.m_log_stats = plib.pstring_global.pstonum_bool(p);  //plib::pstonum<decltype(m_params.m_log_stats)>(p);
                else
                    m_params.m_log_stats = m_log_stats.op();

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
#if true
                        case 1:
                            throw new emu_unimplemented();
                            ms = new matrix_solver_direct1_t(state(), sname, m_params);  //ms = pool().make_poolptr<matrix_solver_direct1_t<double>>(state(), sname, &m_params);
                            break;
                        case 2:
                            throw new emu_unimplemented();
                            ms = new matrix_solver_direct2_t(state(), sname, m_params);  //ms = pool().make_poolptr<matrix_solver_direct2_t<double>>(state(), sname, &m_params);
                            break;
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
#if false
                        case 87:
                            ms = create_solver<87,87>(87, sname);
                            break;
#endif
#endif
                        default:
                            throw new emu_unimplemented();
                            log().warning.op(nl_errstr_global.MW_1_NO_SPECIFIC_SOLVER, net_count);
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
                            else
                                if (net_count <= 64)
                            {
                                ms = create_solver/*<double, -64>*/(-64, net_count, sname);
                            }
                            else
                                if (net_count <= 128)
                            {
                                ms = create_solver/*<double, -128>*/(-128, net_count, sname);
                            }
                            else
                            {
                                log().fatal.op(nl_errstr_global.MF_1_NETGROUP_SIZE_EXCEEDED_1, 128);
                                return; /* tease compilers */
                            }
                            break;
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


            public nl_double gmin() { return m_gmin.op(); }


            //void create_solver_code(std::map<pstring, pstring> &mp);


            //NETLIB_UPDATE(solver)
            protected override void update()
            {
                if (m_params.m_dynamic_ts)
                    return;

                netlist_time now = exec().time();
                /* force solving during start up if there are no time-step devices */
                /* FIXME: Needs a more elegant solution */
                bool force_solve = (now < netlist_time.from_double(2 * m_params.m_max_timestep));

                int nthreads = std.min(m_parallel.op(), plib.omp.pomp_global.get_max_threads());

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
            matrix_solver_t create_solver(int SIZE, UInt32 size, string solvername)  //poolptr<matrix_solver_t> create_solver(std::size_t size, const pstring &solvername);
            {
                if (m_method.op() == "SOR_MAT")
                {
                    throw new emu_unimplemented();
                }
                else if (m_method.op() == "MAT_CR")
                {
                    if (size > 0) // GCR always outperforms MAT solver
                    {
                        return new matrix_solver_GCR_t(SIZE, state(), solvername, m_params, size);  //return create_it<matrix_solver_SOR_mat_t<FT, SIZE>>(state(), solvername, m_params, size);
                    }
                    else
                    {
                        throw new emu_unimplemented();
                    }
                }
                else if (m_method.op() == "MAT")
                {
                    throw new emu_unimplemented();
                }
                else if (m_method.op() == "SM")
                {
                    throw new emu_unimplemented();
                }
                else if (m_method.op() == "W")
                {
                    throw new emu_unimplemented();
                }
                else if (m_method.op() == "SOR")
                {
                    throw new emu_unimplemented();
                }
                else if (m_method.op() == "GMRES")
                {
                    throw new emu_unimplemented();
                }
                else
                {
                    log().fatal.op(nl_errstr_global.MF_1_UNKNOWN_SOLVER_TYPE, m_method.op());
                    return null;
                }
            }


            //template <typename FT, int SIZE>
            //poolptr<matrix_solver_t> create_solver_x(std::size_t size, const pstring &solvername);
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
