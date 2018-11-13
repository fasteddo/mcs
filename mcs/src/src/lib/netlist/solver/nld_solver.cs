// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.std_vector<mame.netlist.analog_net_t>;
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
            //NETLIB_DEVICE_IMPL(solver)
            static factory.element_t nld_solver_c(string name, string classname, string def_param)
            { return new factory.device_element_t<nld_solver>(name, classname, def_param, "__FILE__"); }
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

            param_logic_t  m_log_stats;

            std_vector<matrix_solver_t> m_mat_solvers = new std_vector<matrix_solver_t>();  //std::vector<std::unique_ptr<matrix_solver_t>> m_mat_solvers;

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
                m_gmin = new param_double_t(this, "GMIN", nl_config_global.NETLIST_GMIN_DEFAULT);
                m_pivot = new param_logic_t(this, "PIVOT", false /*0*/);                    // use pivoting - on supported solvers
                m_nr_loops = new param_int_t(this, "NR_LOOPS", 250);            // Newton-Raphson loops
                m_nr_recalc_delay = new param_double_t(this, "NR_RECALC_DELAY", netlist_time.NLTIME_FROM_NS(10).as_double());  // Delay to next solve attempt if nr loops exceeded
                m_parallel = new param_int_t(this, "PARALLEL", 0);

                /* automatic time step */
                m_dynamic_ts = new param_logic_t(this, "DYNAMIC_TS", false /*0*/);
                m_dynamic_lte = new param_double_t(this, "DYNAMIC_LTE", 1e-5);                     // diff/timestep
                m_dynamic_min_ts = new param_double_t(this, "DYNAMIC_MIN_TIMESTEP", 1e-6);   // nl_double timestep resolution

                m_log_stats = new param_logic_t(this, "LOG_STATS", false /*0*/);   // log statistics on shutdown
                m_params = new solver_parameters_t();


                // internal staff

                connect(m_fb_step, m_Q_step);
            }


            ~nld_solver() { }


            public void post_start()
            {
                const bool use_specific = true;

                m_params.m_pivot = m_pivot.op() ? 1 : 0;
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
                    m_params.m_log_stats = p.as_long() != 0;
                else
                    m_params.m_log_stats = m_log_stats.op();

                log().verbose.op("Scanning net groups ...");
                // determine net groups

                net_splitter splitter = new net_splitter();

                splitter.run(netlist());

                // setup the solvers
                log().verbose.op("Found {0} net groups in {1} nets\n", splitter.groups.size(), netlist().nets.size());
                foreach (var grp in splitter.groups)
                {
                    matrix_solver_t ms;
                    UInt32 net_count = (UInt32)grp.size();
                    string sname = new plib.pfmt("Solver_{0}").op(m_mat_solvers.size());

                    switch (net_count)
                    {
#if true
                        case 1:
                            if (use_specific)
                                ms = new matrix_solver_direct1_t(netlist(), sname, m_params);
                            else
                                ms = create_solver(1,1, 1, sname);
                            break;
                        case 2:
                            if (use_specific)
                                ms = new matrix_solver_direct2_t(netlist(), sname, m_params);
                            else
                                ms = create_solver(2,2, 2, sname);
                            break;
#if false
                        case 3:
                            ms = create_solver<3,3>(3, sname);
                            break;
                        case 4:
                            ms = create_solver<4,4>(4, sname);
                            break;
                        case 5:
                            ms = create_solver<5,5>(5, sname);
                            break;
                        case 6:
                            ms = create_solver<6,6>(6, sname);
                            break;
                        case 7:
                            ms = create_solver<7,7>(7, sname);
                            break;
                        case 8:
                            ms = create_solver<8,8>(8, sname);
                            break;
                        case 9:
                            ms = create_solver<9,9>(9, sname);
                            break;
                        case 10:
                            ms = create_solver<10,10>(10, sname);
                            break;
                        case 11:
                            ms = create_solver<11,11>(11, sname);
                            break;
                        case 12:
                            ms = create_solver<12,12>(12, sname);
                            break;
                        case 15:
                            ms = create_solver<15,15>(15, sname);
                            break;
                        case 31:
                            ms = create_solver<31,31>(31, sname);
                            break;
                        case 35:
                            ms = create_solver<35,35>(35, sname);
                            break;
                        case 43:
                            ms = create_solver<43,43>(43, sname);
                            break;
                        case 49:
                            ms = create_solver<49,49>(49, sname);
                            break;
#endif
#if false
                        case 87:
                            ms = create_solver<87,87>(87, sname);
                            break;
#endif
#endif
                        default:
                            log().warning.op(nl_errstr_global.MW_1_NO_SPECIFIC_SOLVER, net_count);
                            if (net_count <= 8)
                            {
                                ms = create_solver(0, 8, net_count, sname);
                            }
                            else if (net_count <= 16)
                            {
                                ms = create_solver(0,16, net_count, sname);
                            }
                            else if (net_count <= 32)
                            {
                                ms = create_solver(0,32, net_count, sname);
                            }
                            else
                                if (net_count <= 64)
                            {
                                ms = create_solver(0,64, net_count, sname);
                            }
                            else
                                if (net_count <= 128)
                            {
                                ms = create_solver(0,128, net_count, sname);
                            }
                            else
                            {
                                log().fatal.op(nl_errstr_global.MF_1_NETGROUP_SIZE_EXCEEDED_1, 128);
                                ms = null; /* tease compilers */
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
                        foreach (var pcore in n.core_terms)
                        {
                            log().verbose.op("   {0}", pcore.name());
                        }
                    }

                    m_mat_solvers.push_back(ms);
                }
            }


            //void stop();

            public nl_double gmin() { return m_gmin.op(); }

            //void create_solver_code(std::map<pstring, pstring> &mp);

            //NETLIB_UPDATEI();
            protected override void update()
            {
                throw new emu_unimplemented();
            }

            //NETLIB_RESETI();
            //NETLIB_RESET(solver)
            protected override void reset()
            {
                for (int i = 0; i < m_mat_solvers.size(); i++)
                    m_mat_solvers[i].do_reset();
            }

            // NETLIB_UPDATE_PARAMI();


            //template <std::size_t m_N, std::size_t storage_N>
            //std::unique_ptr<matrix_solver_t> create_solver(std::size_t size, const pstring &solvername);
            matrix_solver_t create_solver(UInt32 m_N, UInt32 storage_N, UInt32 size, string solvername)
            {
                if ("SOR_MAT".equals(m_method.op()))
                {
                    throw new emu_unimplemented();
#if false
                    return create_it<matrix_solver_SOR_mat_t<m_N, storage_N>>(netlist(), solvername, m_params, size);
                    //typedef matrix_solver_SOR_mat_t<m_N,storage_N> solver_sor_mat;
                    //return plib::make_unique<solver_sor_mat>(netlist(), solvername, &m_params, size);
#endif
                }
                else if ("MAT_CR".equals(m_method.op()))
                {
                    if (size > 0) // GCR always outperforms MAT solver
                    {
                        //typedef matrix_solver_GCR_t<m_N,storage_N> solver_mat;
                        return new matrix_solver_GCR_t(m_N, storage_N, netlist(), solvername, m_params, size);  //return plib::make_unique<solver_mat>(netlist(), solvername, &m_params, size);
                    }
                    else
                    {
                        throw new emu_unimplemented();
#if false
                        typedef matrix_solver_direct_t<m_N,storage_N> solver_mat;
                        return plib::make_unique<solver_mat>(netlist(), solvername, &m_params, size);
#endif
                    }
                }
                else if ("MAT".equals(m_method.op()))
                {
                    throw new emu_unimplemented();
#if false
                    typedef matrix_solver_direct_t<m_N,storage_N> solver_mat;
                    return plib::make_unique<solver_mat>(netlist(), solvername, &m_params, size);
#endif
                }
                else if ("SM".equals(m_method.op()))
                {
                    throw new emu_unimplemented();
#if false
                    /* Sherman-Morrison Formula */
                    typedef matrix_solver_sm_t<m_N,storage_N> solver_mat;
                    return plib::make_unique<solver_mat>(netlist(), solvername, &m_params, size);
#endif
                }
                else if ("W".equals(m_method.op()))
                {
                    throw new emu_unimplemented();
#if false
                    /* Woodbury Formula */
                    typedef matrix_solver_w_t<m_N,storage_N> solver_mat;
                    return plib::make_unique<solver_mat>(netlist(), solvername, &m_params, size);
#endif
                }
                else if ("SOR".equals(m_method.op()))
                {
                    throw new emu_unimplemented();
#if false
                    typedef matrix_solver_SOR_t<m_N,storage_N> solver_GS;
                    return plib::make_unique<solver_GS>(netlist(), solvername, &m_params, size);
#endif
                }
                else if ("GMRES".equals(m_method.op()))
                {
                    throw new emu_unimplemented();
#if false
                    typedef matrix_solver_GMRES_t<m_N,storage_N> solver_GMRES;
                    return plib::make_unique<solver_GMRES>(netlist(), solvername, &m_params, size);
#endif
                }
                else
                {
                    log().fatal.op(nl_errstr_global.MF_1_UNKNOWN_SOLVER_TYPE, m_method.op());
                    return null;
                }
            }
        }


        class net_splitter
        {
            public std_vector<analog_net_t_list_t> groups = new std_vector<analog_net_t_list_t>();


            bool already_processed(analog_net_t n)
            {
                if (n.isRailNet())
                    return true;
                foreach (var grp in groups)
                    if (grp.Contains(n))  //plib::container::contains(grp, n))
                        return true;
                return false;
            }

            void process_net(analog_net_t n)
            {
                if (n.num_cons() == 0)
                    return;
                /* add the net */
                groups.back().push_back(n);
                foreach (var p in n.core_terms)
                {
                    if (p.is_type(detail.terminal_type.TERMINAL))
                    {
                        terminal_t pt = (terminal_t)p;
                        analog_net_t other_net = pt.otherterm.net();
                        if (!already_processed(other_net))
                            process_net(other_net);
                    }
                }
            }

            public void run(netlist_t netlist)
            {
                foreach (var net in netlist.nets)
                {
                    netlist.log().debug.op("processing {0}\n", net.name());
                    if (!net.isRailNet() && net.num_cons() > 0)
                    {
                        netlist.log().debug.op("   ==> not a rail net\n");
                        /* Must be an analog net */
                        analog_net_t n = (analog_net_t)net;
                        if (!already_processed(n))
                        {
                            groups.push_back(new analog_net_t_list_t());
                            process_net(n);
                        }
                    }
                }
            }
        }
    } //namespace devices
} // namespace netlist
