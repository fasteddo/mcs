// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.plib.aligned_vector<mame.netlist.analog_net_t>;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using netlist_time_ext = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time_ext = plib::ptime<std::conditional<NL_PREFER_INT128 && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nld_solver_net_list_t = mame.plib.aligned_vector<mame.netlist.analog_net_t>;  //using net_list_t = solver::matrix_solver_t::net_list_t;
using nld_solver_params_uptr = mame.netlist.solver.solver_parameters_t;  //using params_uptr = solver_arena::unique_ptr<solver::solver_parameters_t>;
using nld_solver_queue_type = mame.netlist.detail.queue_base<mame.netlist.solver.matrix_solver_t, mame.bool_const_false>;  //using queue_type = detail::queue_base<solver::matrix_solver_t, false>;
using nld_solver_solver_ptr = mame.netlist.solver.matrix_solver_t;  //using solver_ptr = solver_arena::unique_ptr<solver::matrix_solver_t>;
using size_t = System.UInt64;


// ----------------------------------------------------------------------------------------
// solver
// ----------------------------------------------------------------------------------------
namespace mame.netlist
{
    namespace devices
    {
        //NETLIB_OBJECT(solver)
        public class nld_solver : device_t
        {
            //using queue_type = detail::queue_base<solver::matrix_solver_t, false>;
            //using solver_arena = device_arena;
            //using solver_ptr = solver_arena::unique_ptr<solver::matrix_solver_t>;
            //using net_list_t = solver::matrix_solver_t::net_list_t;
            //using params_uptr = solver_arena::unique_ptr<solver::solver_parameters_t>;


            //NETLIB_DEVICE_IMPL(solver, "SOLVER", "FREQ")
            public static readonly factory.constructor_ptr_t decl_solver = g.NETLIB_DEVICE_IMPL<nld_solver>("SOLVER", "FREQ");


            logic_input_t m_fb_step;
            logic_output_t m_Q_step;

            // FIXME: these should be created in device space
            std.vector<nld_solver_params_uptr> m_mat_params = new std.vector<nld_solver_params_uptr>();
            std.vector<nld_solver_solver_ptr> m_mat_solvers = new std.vector<nld_solver_solver_ptr>();

            solver.solver_parameters_t m_params;
            nld_solver_queue_type m_queue;


            //NETLIB_CONSTRUCTOR(solver)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            //NETLIB_CONSTRUCTOR(solver)
            public nld_solver(object owner, string name)
                : base(owner, name)
            {
                m_fb_step = new logic_input_t(this, "FB_step", fb_step<bool_const_false>);  //, m_fb_step(*this, "FB_step", NETLIB_DELEGATE(fb_step<false>))
                m_Q_step = new logic_output_t(this, "Q_step");
                m_params = new solver.solver_parameters_t(this, "", solver.solver_parameter_defaults.get_instance());
                m_queue = new nld_solver_queue_type(config.MAX_SOLVER_QUEUE_SIZE,
                    get_solver_id,  //queue_type::id_delegate(&NETLIB_NAME(solver).get_solver_id, this),
                    solver_by_id);  //queue_type::obj_delegate(&NETLIB_NAME(solver).solver_by_id, this));


                // internal stuff
                state().save(this, (plib.state_manager_t.callback_t)m_queue, this.name(), "m_queue");

                connect("FB_step", "Q_step");
            }


            public void post_start()
            {
                log().verbose.op("Scanning net groups ...");
                // determine net groups

                net_splitter splitter = new net_splitter();

                splitter.run(state());

                log().verbose.op("Found {1} net groups in {2} nets\n", splitter.groups.size(), state().nets().size());

                int num_errors = 0;

                log().verbose.op("checking net consistency  ...");
                foreach (var grp in splitter.groups)
                {
                    int railterms = 0;
                    string nets_in_grp = "";
                    foreach (var n in grp)
                    {
                        nets_in_grp += (n.name() + " ");
                        if (!n.is_analog())
                        {
                            state().log().error.op(nl_errstr_global.ME_SOLVER_CONSISTENCY_NOT_ANALOG_NET(n.name()));
                            num_errors++;
                        }

                        if (n.is_rail_net())
                        {
                            state().log().error.op(nl_errstr_global.ME_SOLVER_CONSISTENCY_RAIL_NET(n.name()));
                            num_errors++;
                        }

                        foreach (var t in state().core_terms(n))
                        {
                            if (!t.has_net())
                            {
                                state().log().error.op(nl_errstr_global.ME_SOLVER_TERMINAL_NO_NET(t.name()));
                                num_errors++;
                            }
                            else
                            {
                                var otherterm = t is terminal_t ? (terminal_t)t : null;  //auto *otherterm = dynamic_cast<terminal_t *>(t);
                                if (otherterm != null)
                                    if (state().setup().get_connected_terminal(otherterm).net().is_rail_net())
                                        railterms++;
                            }
                        }
                    }

                    if (railterms == 0)
                    {
                        state().log().error.op(nl_errstr_global.ME_SOLVER_NO_RAIL_TERMINAL(nets_in_grp));
                        num_errors++;
                    }
                }

                if (num_errors > 0)
                    throw new nl_exception(nl_errstr_global.MF_SOLVER_CONSISTENCY_ERRORS(num_errors));

                // setup the solvers
                foreach (var grp in splitter.groups)
                {
                    nld_solver_solver_ptr ms = null;
                    string sname = new plib.pfmt("Solver_{0}").op(m_mat_solvers.size());
                    nld_solver_params_uptr params_ = new solver.solver_parameters_t(this, sname + ".", m_params);  //params_uptr params = plib::make_unique<solver::solver_parameters_t, solver_arena>(*this, sname + ".", m_params);

                    switch (params_.m_fp_type.op())
                    {
                        case solver.matrix_fp_type_e.FLOAT:
                            if (!config.use_float_matrix)
                                log().info.op("FPTYPE {0} not supported. Using DOUBLE", params_.m_fp_type.op().ToString());

                            //ms = create_solvers<std::conditional_t<config::use_float_matrix::value, float, double>>(sname, params.get(), grp);
                            if (config.use_float_matrix)
                                ms = create_solvers<float, plib.constants_operators_float>(sname, params_, grp);
                            else
                                ms = create_solvers<double, plib.constants_operators_double>(sname, params_, grp);
                            break;
                        case solver.matrix_fp_type_e.DOUBLE:
                            ms = create_solvers<double, plib.constants_operators_double>(sname, params_, grp);  //ms = create_solvers<double>(sname, params.get(), grp);
                            break;
                        case solver.matrix_fp_type_e.LONGDOUBLE:
                            if (!config.use_long_double_matrix)
                                log().info.op("FPTYPE {0} not supported. Using DOUBLE", params_.m_fp_type.op().ToString());

                            //ms = create_solvers<std::conditional_t<config::use_long_double_matrix::value, long double, double>>(sname, params.get(), grp);
                            if (config.use_long_double_matrix)
                                throw new emu_unimplemented();  //ms = create_solvers<long double>(sname, params_, grp);
                            else
                                ms = create_solvers<double, plib.constants_operators_double>(sname, params_, grp);
                            break;
                        case solver.matrix_fp_type_e.FLOATQ128:
#if (NL_USE_FLOAT128)
                            ms = create_solvers<FLOAT128>(sname, params.get(), grp);
#else
                            log().info.op("FPTYPE {0} not supported. Using DOUBLE", params_.m_fp_type.op().ToString());
                            ms = create_solvers<double, plib.constants_operators_double>(sname, params_, grp);  //ms = create_solvers<double>(sname, params.get(), grp);
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
                        foreach (var t in state().core_terms(n))
                        {
                            log().verbose.op("   {0}", t.name());
                        }
                    }

                    m_mat_params.push_back(params_);

                    m_mat_solvers.push_back(ms);
                }
            }


            //template <typename FT>
            solver.matrix_solver_t create_solvers<FT, FT_OPS>(string sname, solver.solver_parameters_t params_, nld_solver_net_list_t nets)  //NETLIB_NAME(solver)::solver_ptr NETLIB_NAME(solver)::create_solvers(const pstring &sname, const solver::solver_parameters_t *params, net_list_t &nets)
                where FT_OPS : plib.constants_operators<FT>, new()
            {
                size_t net_count = nets.size();
                switch (net_count)
                {
                    case 1:
                        throw new emu_unimplemented();
#if false
                        return new solver.matrix_solver_direct1_t(state(), sname, nets, m_params);  //return plib::make_unique<solver::matrix_solver_direct1_t<FT>>(state(), sname, nets, &m_params);
#endif
                        break;
                    case 2:
                        throw new emu_unimplemented();
#if false
                        return new solver.matrix_solver_direct2_t(state(), sname, nets, m_params);  //return plib::make_unique<solver::matrix_solver_direct2_t<FT>>(state(), sname, nets, &m_params);
#endif
                        break;
                    case 3:
                        throw new emu_unimplemented();
#if false
                        return create_solver/*<FT, 3>*/(3, 3, sname, nets);
#endif
                        break;
                    case 4:
                        throw new emu_unimplemented();
#if false
                        return create_solver/*<FT, 4>*/(4, 4, sname, nets);
#endif
                        break;
                    case 5:
                        throw new emu_unimplemented();
#if false
                        return create_solver/*<FT, 5>*/(5, 5, sname, nets);
#endif
                        break;
                    case 6:
                        throw new emu_unimplemented();
#if false
                        return create_solver/*<FT, 6>*/(6, 6, sname, nets);
#endif
                        break;
                    case 7:
                        throw new emu_unimplemented();
#if false
                        return create_solver/*<FT, 7>*/(7, 7, sname, nets);
#endif
                        break;
                    case 8:
                        throw new emu_unimplemented();
#if false
                        return create_solver/*<FT, 8>*/(8, 8, sname, nets);
#endif
                        break;
                    default:
                        log().info.op(nl_errstr_global.MI_NO_SPECIFIC_SOLVER(net_count));
                        if (net_count <= 16)
                        {
                            return create_solver<FT, FT_OPS, int_const_n16>(net_count, sname, params_, nets);
                        }
                        if (net_count <= 32)
                        {
                            return create_solver<FT, FT_OPS, int_const_n32>(net_count, sname, params_, nets);
                        }
                        if (net_count <= 64)
                        {
                            return create_solver<FT, FT_OPS, int_const_n64>(net_count, sname, params_, nets);
                        }
                        if (net_count <= 128)
                        {
                            return create_solver<FT, FT_OPS, int_const_n128>(net_count, sname, params_, nets);
                        }
                        if (net_count <= 256)
                        {
                            return create_solver<FT, FT_OPS, int_const_n256>(net_count, sname, params_, nets);
                        }
                        if (net_count <= 512)
                        {
                            return create_solver<FT, FT_OPS, int_const_n512>(net_count, sname, params_, nets);
                        }

                        return create_solver<FT, FT_OPS, int_const_0>(net_count, sname, params_, nets);
                }
            }


            public void stop()
            {
                foreach (var s in m_mat_solvers)
                    s.log_stats();
            }


            public nl_fptype gmin() { return m_params.m_gmin.op(); }  //auto gmin() const -> decltype(solver::solver_parameters_t::m_gmin()) { return m_params.m_gmin(); }


            //solver::static_compile_container create_solver_code(solver::static_compile_target target);


            //NETLIB_RESETI();
            //NETLIB_RESET(solver)
            public override void reset()
            {
                if (exec().stats_enabled())
                    m_fb_step.set_delegate(fb_step<bool_const_true>);  //m_fb_step.set_delegate(NETLIB_DELEGATE(fb_step<true>));
                foreach (var s in m_mat_solvers)
                    s.reset();
                foreach (var s in m_mat_solvers)
                    m_queue.push(false, new plib.pqentry_t<netlist_time, nld_solver_solver_ptr>(netlist_time_ext.zero(), s));
            }


            // NETLIB_UPDATE_PARAMI();


            public void reschedule(solver.matrix_solver_t solv, netlist_time ts)
            {
                netlist_time_ext now = exec().time();
                netlist_time_ext sched = now + ts;
                m_queue.remove(false, solv);
                m_queue.push(false, new plib.pqentry_t<netlist_time, nld_solver_solver_ptr>(sched, solv));

                if (m_Q_step.net().is_queued())
                {
                    if (m_Q_step.net().next_scheduled_time() > sched)
                        m_Q_step.net().toggle_and_push_to_queue(ts);
                }
                else
                {
                    m_Q_step.net().toggle_and_push_to_queue(ts);
                }
            }


            // FIXME: should be created in device space
            //template <class C>
            solver.matrix_solver_t create_it<C, FT, FT_OPS, int_SIZE>(nld_solver main_solver, string name, analog_net_t_list_t nets, solver.solver_parameters_t params_, size_t size)  //NETLIB_NAME(solver)::solver_ptr create_it(NETLIB_NAME(solver) &main_solver, pstring name, NETLIB_NAME(solver)::net_list_t &nets, const solver::solver_parameters_t *params, std::size_t size)
                where FT_OPS : plib.constants_operators<FT>, new()
                where int_SIZE : int_const, new()
            {
                if (typeof(C) == typeof(solver.matrix_solver_GCR_t<FT, FT_OPS, int_SIZE>))
                    return new solver.matrix_solver_GCR_t<FT, FT_OPS, int_SIZE>(main_solver, name, nets, params_, size);  //return plib::make_unique<C, device_arena>(main_solver, name, nets, params, size);
                else
                    throw new emu_unimplemented();
            }


            public class size_t_const_MAX_SOLVER_QUEUE_SIZE : u64_const { public UInt64 value { get { return config.MAX_SOLVER_QUEUE_SIZE; } } }


            //template<bool KEEP_STATS>
            //NETLIB_HANDLER(solver, fb_step)
            void fb_step<bool_KEEP_STATS>()
                where bool_KEEP_STATS : bool_const, new()
            {
                bool KEEP_STATS = new bool_KEEP_STATS().value;

                netlist_time_ext now = exec().time();
                size_t nthreads = m_params.m_parallel.op() < 2 ? 1 : (size_t)std.min(m_params.m_parallel.op(), plib.omp.pomp_global.get_max_threads());
                netlist_time_ext sched = now + (nthreads <= 1 ? netlist_time_ext.zero() : netlist_time_ext.from_nsec(100));
                plib.uninitialised_array<solver.matrix_solver_t, size_t_const_MAX_SOLVER_QUEUE_SIZE> tmp = new plib.uninitialised_array<solver.matrix_solver_t, size_t_const_MAX_SOLVER_QUEUE_SIZE>();  //plib::uninitialised_array<solver::matrix_solver_t *, config::MAX_SOLVER_QUEUE_SIZE::value> tmp;
                plib.uninitialised_array<netlist_time, size_t_const_MAX_SOLVER_QUEUE_SIZE> nt = new plib.uninitialised_array<netlist_time, size_t_const_MAX_SOLVER_QUEUE_SIZE>();  //plib::uninitialised_array<netlist_time, config::MAX_SOLVER_QUEUE_SIZE::value> nt;
                size_t p = 0;

                while (!m_queue.empty())
                {
                    var t = m_queue.top().exec_time();
                    var o = m_queue.top().object_();
                    if (t != now)
                        if (t > sched)
                            break;
                    tmp[p++] = o;
                    m_queue.pop();
                }

                // FIXME: Disabled for now since parallel processing will decrease performance
                //        for tested applications. More testing required here
                if (true || nthreads < 2)
                {
                    if (!KEEP_STATS)
                    {
                        for (size_t i = 0; i < p; i++)
                            nt[i] = tmp[i].solve(now, "no-parallel");
                    }
                    else
                    {
                        throw new emu_unimplemented();
#if false
                        stats().m_stat_total_time.stop();
                        for (size_t i = 0; i < p; i++)
                        {
                            tmp[i].stats().m_stat_call_count.inc();
                            var g = tmp[i].stats().m_stat_total_time.guard();
                            nt[i] = tmp[i].solve(now, "no-parallel");
                        }

                        stats().m_stat_total_time.start();
#endif
                    }

                    for (size_t i = 0; i < p; i++)
                    {
                        if (nt[i] != netlist_time.zero())
                            m_queue.push(false, new plib.pqentry_t<netlist_time, nld_solver_solver_ptr>(now + nt[i], tmp[i]));
                        tmp[i].update_inputs();
                    }
                }
                else
                {
                    plib.omp.pomp_global.set_num_threads((int)nthreads);
                    //plib::omp::for_static(static_cast<std::size_t>(0), p, [&tmp, &nt,now](std::size_t i)
                    //    {
                    //        nt[i] = tmp[i]->solve(now, "parallel");
                    //    });
                    for (size_t i = 0; i < p; i++)
                    {
                        nt[i] = tmp[i].solve(now, "parallel");
                    }

                    for (size_t i = 0; i < p; i++)
                    {
                        if (nt[i] != netlist_time.zero())
                            m_queue.push(false, new plib.pqentry_t<netlist_time, nld_solver_solver_ptr>(now + nt[i], tmp[i]));
                        tmp[i].update_inputs();
                    }
                }

                if (!m_queue.empty())
                    m_Q_step.net().toggle_and_push_to_queue((netlist_time)(m_queue.top().exec_time() - now));
            }


            //template <typename FT, int SIZE>
            nld_solver_solver_ptr create_solver<FT, FT_OPS, int_SIZE>(size_t size, string solvername, solver.solver_parameters_t params_, nld_solver_net_list_t nets)  //solver_ptr create_solver(std::size_t size, const pstring &solvername, const solver::solver_parameters_t *params,net_list_t &nets);
                where FT_OPS : plib.constants_operators<FT>, new()
                where int_SIZE : int_const, new()
            {
                switch (m_params.m_method.op())
                {
                    case solver.matrix_type_e.MAT_CR:
                        return create_it<solver.matrix_solver_GCR_t<FT, FT_OPS, int_SIZE>, FT, FT_OPS, int_SIZE>(this, solvername, nets, params_, size);  //return create_it<solver::matrix_solver_GCR_t<FT, SIZE>>(*this, solvername, nets, params, size);
                    case solver.matrix_type_e.MAT:
                        throw new emu_unimplemented();
#if false
                        return create_it<solver::matrix_solver_direct_t<FT, SIZE>>(*this, solvername, nets, params, size);
#endif

                    case solver.matrix_type_e.GMRES:
                        throw new emu_unimplemented();
#if false
                        return create_it<solver::matrix_solver_GMRES_t<FT, SIZE>>(*this, solvername, nets, params, size);
#endif

#if NL_USE_ACADEMIC_SOLVERS
                    case solver::matrix_type_e::SOR:
                        return create_it<solver::matrix_solver_SOR_t<FT, SIZE>>(*this, solvername, nets, params, size);
                    case solver::matrix_type_e::SOR_MAT:
                        return create_it<solver::matrix_solver_SOR_mat_t<FT, SIZE>>(*this, solvername, nets, params, size);
                    case solver::matrix_type_e::SM:
                        // Sherman-Morrison Formula
                        return create_it<solver::matrix_solver_sm_t<FT, SIZE>>(*this, solvername, nets, params, size);
                    case solver::matrix_type_e::W:
                        // Woodbury Formula
                        return create_it<solver::matrix_solver_w_t<FT, SIZE>>(*this, solvername, nets, params, size);
#else
                    //case solver.matrix_type_e.GMRES:
                    case solver.matrix_type_e.SOR:
                    case solver.matrix_type_e.SOR_MAT:
                    case solver.matrix_type_e.SM:
                    case solver.matrix_type_e.W:
                        state().log().warning.op(nl_errstr_global.MW_SOLVER_METHOD_NOT_SUPPORTED(params_.m_method.op().ToString(), "MAT_CR"));
                        throw new emu_unimplemented();
#if false
                        return create_it<solver::matrix_solver_GCR_t<FT, SIZE>>(*this, solvername, nets, params, size);
#endif
#endif
                }

                throw new emu_unimplemented();
#if false
                return solver_ptr();
#endif
            }


            size_t get_solver_id(solver.matrix_solver_t net)
            {
                for (size_t i = 0; i < m_mat_solvers.size(); i++)
                {
                    if (m_mat_solvers[i] == net)
                        return i;
                }

                return size_t.MaxValue;  //return std::numeric_limits<std::size_t>::max();
            }


            solver.matrix_solver_t solver_by_id(size_t id)
            {
                return m_mat_solvers[id];
            }
        }


        class net_splitter
        {
            public std.vector<nld_solver_net_list_t> groups = new std.vector<nld_solver_net_list_t>();

            std.vector<nld_solver_net_list_t> groupspre = new std.vector<nld_solver_net_list_t>();


            public void run(netlist_state_t nlstate)
            {
                foreach (var net in nlstate.nets())
                {
                    nlstate.log().verbose.op("processing {0}", net.name());
                    if (!net.is_rail_net() && !nlstate.core_terms(net).empty())
                    {
                        nlstate.log().verbose.op("   ==> not a rail net");
                        // Must be an analog net
                        var n = (analog_net_t)net;  //auto &n = dynamic_cast<analog_net_t &>(*net);
                        if (!already_processed(n))
                        {
                            groupspre.emplace_back(new nld_solver_net_list_t());
                            process_net(nlstate, n);
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
                    if (plib.container.contains(grp, n))
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


            void process_net(netlist_state_t nlstate, analog_net_t n)
            {
                // ignore empty nets. FIXME: print a warning message
                nlstate.log().verbose.op("Net {0}", n.name());
                if (!nlstate.core_terms(n).empty())
                {
                    // add the net
                    groupspre.back().push_back(n);
                    // process all terminals connected to this net
                    foreach (var term in nlstate.core_terms(n))
                    {
                        nlstate.log().verbose.op("Term {0} {1}", term.name(), (int)term.type());
                        // only process analog terminals
                        if (term.is_type(detail.terminal_type.TERMINAL))
                        {
                            var pt = (terminal_t)term;  //auto &pt = dynamic_cast<terminal_t &>(*term);
                            // check the connected terminal
                            var connected_terminals = nlstate.setup().get_connected_terminals(pt);
                            foreach (var ct in connected_terminals)  //for (auto ct = connected_terminals->begin(); *ct != nullptr; ct++)
                            {
                                if (ct == default)
                                    continue;

                                analog_net_t connected_net = ct.net();
                                nlstate.log().verbose.op("  Connected net {0}", connected_net.name());
                                if (!check_if_processed_and_join(connected_net))
                                    process_net(nlstate, connected_net);
                            }
                        }
                    }
                }
            }
        }
    } //namespace devices
} // namespace netlist
