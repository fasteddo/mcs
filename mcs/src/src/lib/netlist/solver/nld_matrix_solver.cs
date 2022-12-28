// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_data_t = mame.netlist.core_device_data_t;  //using device_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;
using device_t_constructor_data_t = mame.netlist.core_device_data_t;  //using constructor_data_t = device_data_t;  //using device_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;
using matrix_solver_t_fptype = System.Double;  //using fptype = nl_fptype;
using matrix_solver_t_net_list_t = mame.std.vector<mame.netlist.analog_net_t>;  //using net_list_t =  std::vector<analog_net_t *>;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using netlist_time_ext = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time_ext = plib::ptime<std::conditional<config::prefer_int128::value && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using param_fp_t = mame.netlist.param_num_t<System.Double, mame.netlist.param_num_t_operators_double>;  //using param_fp_t = param_num_t<nl_fptype>;
using param_int_t = mame.netlist.param_num_t<int, mame.netlist.param_num_t_operators_int>;  //using param_int_t = param_num_t<int>;
using param_logic_t = mame.netlist.param_num_t<bool, mame.netlist.param_num_t_operators_bool>;  //using param_logic_t = param_num_t<bool>;
using param_num_t_operators_size_t = mame.netlist.param_num_t_operators_uint64;
using size_t = System.UInt64;
using unsigned = System.UInt32;

using static mame.netlist.nl_config_global;
using static mame.netlist.nl_errstr_global;


namespace mame.netlist.solver
{
    public enum static_compile_target
    {
        CXX_EXTERNAL_C,
        CXX_STATIC
    }


    public enum matrix_sort_type_e  //PENUM(matrix_sort_type_e,
    {
        NOSORT,
        ASCENDING,
        DESCENDING,
        PREFER_IDENTITY_TOP_LEFT,
        PREFER_BAND_MATRIX
    }


    public enum matrix_type_e  //PENUM(matrix_type_e,
    {
        SOR_MAT,
        MAT_CR,
        MAT,
        SM,
        W,
        SOR,
        GMRES
    }


    public enum matrix_fp_type_e  //PENUM(matrix_fp_type_e,
    {
        FLOAT,
        DOUBLE,
        LONGDOUBLE,
        FLOATQ128
    }


    //using arena_type = plib::mempool_arena<plib::aligned_arena<>, 1024>;
    //using static_compile_container = std::vector<std::pair<pstring, pstring>>;


    public interface i_solver_parameter_defaults
    {
        nl_fptype m_freq_();

        // iteration parameters
        nl_fptype m_gs_sor_();
        matrix_type_e m_method_();
        matrix_fp_type_e m_fp_type_();
        nl_fptype m_reltol_();
        nl_fptype m_vntol_();
        nl_fptype m_accuracy_();
        size_t m_nr_loops_();
        size_t m_gs_loops_();

        nl_fptype m_gmin_();
        bool m_pivot_();
        nl_fptype m_nr_recalc_delay_();
        int m_parallel_();

        nl_fptype m_min_ts_ts_();
        // automatic time step
        bool m_dynamic_ts_();
        nl_fptype m_dynamic_lte_();
        nl_fptype m_dynamic_min_ts_();

        // matrix sorting
        matrix_sort_type_e m_sort_type_();

        // special
        bool m_use_gabs_();
    }


    struct solver_parameter_defaults : i_solver_parameter_defaults
    {
        public nl_fptype m_freq_() { return nlconst.magic(48000.0); }

        // iteration parameters
        public nl_fptype m_gs_sor_() { return nlconst.magic(1.059); }
        public matrix_type_e m_method_() { return matrix_type_e.MAT_CR; }
        public matrix_fp_type_e m_fp_type_() { return matrix_fp_type_e.DOUBLE; }
        public nl_fptype m_reltol_() { return nlconst.magic(1e-3); }
        public nl_fptype m_vntol_() { return nlconst.magic(1e-7); }
        public nl_fptype m_accuracy_() { return nlconst.magic(1e-7); }
        public size_t m_nr_loops_() { return 250; }
        public size_t m_gs_loops_() { return 50; }

        // general parameters
        public nl_fptype m_gmin_() { return nlconst.magic(1e-9); }
        public bool m_pivot_() { return false; }
        public nl_fptype m_nr_recalc_delay_(){ return netlist_time.quantum().as_fp(); }
        public int m_parallel_() { return 0; }

        public nl_fptype m_min_ts_ts_() { return nlconst.magic(1e-9); }
        // automatic time step
        public bool m_dynamic_ts_() { return false; }
        public nl_fptype m_dynamic_lte_() { return nlconst.magic(1e-5); }
        public nl_fptype m_dynamic_min_ts_() { return nlconst.magic(1e-6); }

        // matrix sorting
        public matrix_sort_type_e m_sort_type_() { return matrix_sort_type_e.PREFER_IDENTITY_TOP_LEFT; }

        // special
        public bool m_use_gabs_() { return true; }


        static solver_parameter_defaults s;

        public static solver_parameter_defaults get_instance()
        {
            return s;
        }
    }


    public struct solver_parameters_t : i_solver_parameter_defaults
    {
        param_fp_t m_freq;
        public param_fp_t m_gs_sor;
        public param_enum_t<matrix_type_e, param_enum_t_operators_matrix_type_e> m_method;
        public param_enum_t<matrix_fp_type_e, param_enum_t_operators_matrix_fp_type_e> m_fp_type;
        public param_fp_t m_reltol;
        public param_fp_t m_vntol;
        public param_fp_t m_accuracy;
        public param_num_t<size_t, param_num_t_operators_size_t> m_nr_loops;
        public param_num_t<size_t, param_num_t_operators_size_t> m_gs_loops;
        public param_fp_t m_gmin;
        public param_logic_t m_pivot;
        public param_fp_t m_nr_recalc_delay;
        public param_int_t m_parallel;
        public param_fp_t m_min_ts_ts;
        public param_logic_t m_dynamic_ts;
        public param_fp_t m_dynamic_lte;
        param_fp_t m_dynamic_min_ts;
        public param_enum_t<matrix_sort_type_e, param_enum_t_operators_matrix_sort_type_e> m_sort_type;

        public param_logic_t m_use_gabs;

        public nl_fptype m_min_time_step;
        public nl_fptype m_max_time_step;


        //template <typename D>
        public solver_parameters_t(device_t parent, string prefix, i_solver_parameter_defaults defaults)
        {
            m_freq = new param_fp_t(parent, prefix + "FREQ", defaults.m_freq_());

            // iteration parameters
            m_gs_sor = new param_fp_t(parent,   prefix + "SOR_FACTOR", defaults.m_gs_sor_());
            m_method = new param_enum_t<matrix_type_e, param_enum_t_operators_matrix_type_e>(parent,   prefix + "METHOD", defaults.m_method_());
            m_fp_type = new param_enum_t<matrix_fp_type_e, param_enum_t_operators_matrix_fp_type_e>(parent,  prefix + "FPTYPE", defaults.m_fp_type_());
            m_reltol = new param_fp_t(parent,   prefix + "RELTOL", defaults.m_reltol_());            ///< SPICE RELTOL parameter
            m_vntol = new param_fp_t(parent,    prefix + "VNTOL",  defaults.m_vntol_());            ///< SPICE VNTOL parameter
            m_accuracy = new param_fp_t(parent, prefix + "ACCURACY", defaults.m_accuracy_());          ///< Iterative solver accuracy
            m_nr_loops = new param_num_t<size_t, param_num_t_operators_size_t>(parent, prefix + "NR_LOOPS", defaults.m_nr_loops_());           ///< Maximum number of Newton-Raphson loops
            m_gs_loops = new param_num_t<size_t, param_num_t_operators_size_t>(parent, prefix + "GS_LOOPS", defaults.m_gs_loops_());             ///< Maximum number of Gauss-Seidel loops

            // general parameters
            m_gmin = new param_fp_t(parent, prefix + "GMIN", defaults.m_gmin_());
            m_pivot = new param_logic_t(parent, prefix + "PIVOT", defaults.m_pivot_());               ///< use pivoting on supported solvers
            m_nr_recalc_delay = new param_fp_t(parent, prefix + "NR_RECALC_DELAY", defaults.m_nr_recalc_delay_()); ///< Delay to next solve attempt if nr loops exceeded
            m_parallel = new param_int_t(parent, prefix + "PARALLEL", (int)defaults.m_parallel_());
            m_min_ts_ts = new param_fp_t(parent, prefix + "MIN_TS_TS", defaults.m_min_ts_ts_()); ///< The minimum time step for solvers with time stepping devices.

            // automatic time step
            m_dynamic_ts = new param_logic_t(parent, prefix + "DYNAMIC_TS", defaults.m_dynamic_ts_());     ///< Use dynamic time stepping
            m_dynamic_lte = new param_fp_t(parent, prefix + "DYNAMIC_LTE", defaults.m_dynamic_lte_());    ///< dynamic time stepping slope
            m_dynamic_min_ts = new param_fp_t(parent, prefix + "DYNAMIC_MIN_TIMESTEP", defaults.m_dynamic_min_ts_()); ///< smallest time step allowed

            // matrix sorting
            m_sort_type = new param_enum_t<matrix_sort_type_e, param_enum_t_operators_matrix_sort_type_e>(parent, prefix + "SORT_TYPE", defaults.m_sort_type_());

            // special
            m_use_gabs = new param_logic_t(parent, prefix + "USE_GABS", defaults.m_use_gabs_());


            m_min_time_step = m_dynamic_min_ts.op();
            m_max_time_step = netlist_time.from_fp(plib.pg.reciprocal(m_freq.op())).as_fp();  //m_max_timestep = netlist_time::from_fp(plib::reciprocal(m_freq())).as_fp<decltype(m_max_timestep)>();


            if (m_dynamic_ts != null)
            {
                m_max_time_step *= 1;//NL_FCONST(1000.0);
            }
            else
            {
                m_min_time_step = m_max_time_step;
            }
        }


        public nl_fptype m_freq_() { return m_freq.op(); }

        // iteration parameters
        public nl_fptype m_gs_sor_() { return m_gs_sor.op(); }
        public matrix_type_e m_method_() { return m_method.op(); }
        public matrix_fp_type_e m_fp_type_() { return m_fp_type.op(); }
        public nl_fptype m_reltol_() { return m_reltol.op(); }
        public nl_fptype m_vntol_() { return m_vntol.op(); }
        public nl_fptype m_accuracy_() { return m_accuracy.op(); }
        public size_t m_nr_loops_() { return m_nr_loops.op(); }
        public size_t m_gs_loops_() { return m_gs_loops.op(); }

        // general parameters
        public nl_fptype m_gmin_() { return m_gmin.op(); }
        public bool m_pivot_() { return m_pivot.op(); }
        public nl_fptype m_nr_recalc_delay_() { return m_nr_recalc_delay.op(); }
        public int m_parallel_() { return m_parallel.op(); }

        public nl_fptype m_min_ts_ts_() { return m_min_ts_ts.op(); }
        // automatic time step
        public bool m_dynamic_ts_() { return m_dynamic_ts.op(); }
        public nl_fptype m_dynamic_lte_() { return m_dynamic_lte.op(); }
        public nl_fptype m_dynamic_min_ts_() { return m_dynamic_min_ts.op(); }

        // matrix sorting
        public matrix_sort_type_e m_sort_type_() { return m_sort_type.op(); }

        // special
        public bool m_use_gabs_() { return m_use_gabs.op(); }
    }


    public class terms_for_net_t
    {
        public std.vector<unsigned> m_nz;  //plib::arena_vector<arena_type, unsigned> m_nz;   //!< all non zero for multiplication
        public std.vector<unsigned> m_nzrd;  //plib::arena_vector<arena_type, unsigned> m_nzrd; //!< non zero right of the diagonal for elimination, may include RHS element
        public std.vector<unsigned> m_nzbd;  //plib::arena_vector<arena_type, unsigned> m_nzbd; //!< non zero below of the diagonal for elimination

        public std.vector<int> m_connected_net_idx;  //plib::arena_vector<arena_type, int> m_connected_net_idx;

        std.vector<terminal_t> m_terms;  //plib::arena_vector<arena_type, terminal_t *> m_terms;

        analog_net_t m_net;
        size_t m_rail_start;


        public terms_for_net_t(analog_net_t net = null)  //terms_for_net_t(arena_type &arena, analog_net_t * net = nullptr);
        {
            m_nz = new std.vector<unsigned>();
            m_nzrd = new std.vector<unsigned>();
            m_nzbd = new std.vector<unsigned>();
            m_connected_net_idx = new std.vector<int>();
            m_terms = new std.vector<terminal_t>();
            m_net = net;
            m_rail_start = 0;
        }


        public void add_terminal(terminal_t term, int net_other, bool sorted)
        {
            if (sorted)
            {
                for (size_t i = 0; i < m_connected_net_idx.size(); i++)
                {
                    if (m_connected_net_idx[i] > net_other)
                    {
                        m_terms.Insert((int)i, term);  //plib::container::insert_at(m_terms, i, term);
                        m_connected_net_idx.Insert((int)i, net_other);  //plib::container::insert_at(m_connected_net_idx, i, net_other);
                        return;
                    }
                }
            }

            m_terms.push_back(term);
            m_connected_net_idx.push_back(net_other);
        }


        public size_t count() { return (size_t)m_terms.size(); }  //std::size_t count() const { return m_terms.size(); }

        public size_t rail_start() { return m_rail_start; }

        public std.vector<terminal_t> terms() { return m_terms; }  //inline terminal_t **terms() { return m_terms.data(); }


        public nl_fptype getV() { return m_net.Q_Analog(); }

        public void setV(nl_fptype v) { m_net.set_Q_Analog(v); }


        public bool is_net(analog_net_t net) { return net == m_net; }


        public void set_rail_start(size_t val) { m_rail_start = val; }
    }


    class proxied_analog_output_t : analog_output_t
    {
        analog_net_t m_proxied_net; // only for proxy nets in analog input logic


        public proxied_analog_output_t(core_device_t dev, string aname, analog_net_t pnet)
            : base(dev, aname)
        {
            m_proxied_net = pnet;
        }


        public analog_net_t proxied_net() { return m_proxied_net; }
    }


    public abstract class matrix_solver_t : device_t
    {
        //using list_t = std::vector<matrix_solver_t *>;
        //using fptype = nl_fptype;
        //using net_list_t =  std::vector<analog_net_t *>;


        protected solver_parameters_t m_params;
        //arena_type m_arena;

        protected plib.pmatrix2d_vrl<matrix_solver_t_fptype> m_gonn = new plib.pmatrix2d_vrl<matrix_solver_t_fptype>();  //plib::pmatrix2d_vrl<arena_type, fptype>   m_gonn;
        protected plib.pmatrix2d_vrl<matrix_solver_t_fptype> m_gtn = new plib.pmatrix2d_vrl<matrix_solver_t_fptype>();  //plib::pmatrix2d_vrl<arena_type, fptype>   m_gtn;
        protected plib.pmatrix2d_vrl<matrix_solver_t_fptype> m_Idrn = new plib.pmatrix2d_vrl<matrix_solver_t_fptype>();  //plib::pmatrix2d_vrl<arena_type, fptype>   m_Idrn;
        protected plib.pmatrix2d_vrl<Pointer<matrix_solver_t_fptype>> m_connected_net_Vn = new plib.pmatrix2d_vrl<Pointer<matrix_solver_t_fptype>>();  //plib::pmatrix2d_vrl<arena_type, fptype *> m_connected_net_Vn;

        protected state_var<size_t> m_iterative_fail;
        protected state_var<size_t> m_iterative_total;

        protected std.vector<terms_for_net_t> m_terms; // setup only

        devices.nld_solver m_main_solver;

        state_var<size_t> m_stat_calculations;
        state_var<size_t> m_stat_newton_raphson;
        state_var<size_t> m_stat_newton_raphson_fail;
        state_var<size_t> m_stat_vsolver_calls;

        state_var<netlist_time_ext> m_last_step;
        std.vector<nl_delegate_ts> m_step_funcs = new std.vector<nl_delegate_ts>();  //plib::arena_vector<arena_type, nl_delegate_ts> m_step_funcs;
        std.vector<nl_delegate_dyn> m_dynamic_funcs = new std.vector<nl_delegate_dyn>();  //plib::arena_vector<arena_type, nl_delegate_dyn> m_dynamic_funcs;
        std.vector<proxied_analog_output_t> m_inputs = new std.vector<proxied_analog_output_t>();  //plib::arena_vector<arena_type, device_arena::unique_ptr<proxied_analog_output_t>> m_inputs;

        size_t m_ops;

        std.vector<terms_for_net_t> m_rails_temp = new std.vector<terms_for_net_t>(); // setup only


        // ----------------------------------------------------------------------------------------
        // matrix_solver
        // ----------------------------------------------------------------------------------------
        protected matrix_solver_t(devices.nld_solver main_solver, string name, matrix_solver_t_net_list_t nets, solver.solver_parameters_t params_)
            : base(new device_data_t(main_solver.state(), main_solver.name() + "." + name))
        {
            m_params = params_;
            m_gonn = new plib.pmatrix2d_vrl<matrix_solver_t_fptype>();
            m_gtn = new plib.pmatrix2d_vrl<matrix_solver_t_fptype>();
            m_Idrn = new plib.pmatrix2d_vrl<matrix_solver_t_fptype>();
            m_connected_net_Vn = new plib.pmatrix2d_vrl<Pointer<matrix_solver_t_fptype>>();
            m_iterative_fail = new state_var<size_t>(this, "m_iterative_fail", 0);
            m_iterative_total = new state_var<size_t>(this, "m_iterative_total", 0);
            m_main_solver = main_solver;
            m_stat_calculations = new state_var<size_t>(this, "m_stat_calculations", 0);
            m_stat_newton_raphson = new state_var<size_t>(this, "m_stat_newton_raphson", 0);
            m_stat_newton_raphson_fail = new state_var<size_t>(this, "m_stat_newton_raphson_fail", 0);
            m_stat_vsolver_calls = new state_var<size_t>(this, "m_stat_vsolver_calls", 0);
            m_last_step = new state_var<netlist_time>(this, "m_last_step", netlist_time_ext.zero());
            m_step_funcs = new std.vector<nl_delegate_ts>();
            m_dynamic_funcs = new std.vector<nl_delegate_dyn>();
            m_inputs = new std.vector<proxied_analog_output_t>();
            m_ops = 0;


            setup_base(this.state().setup(), nets);

            // now setup the matrix
            setup_matrix();
            //printf("Freq: %f\n", m_params.m_freq());
        }


        public netlist_time solve(netlist_time_ext now, string source)
        {
            var delta = now - m_last_step.op;

            //throw new emu_unimplemented();
#if false
            PFDEBUG(printf("solve %.10f\n", delta.as_double());)
#endif

            // We are already up to date. Avoid oscillations.
            // FIXME: Make this a parameter!
            if (delta < netlist_time.quantum())
            {
                //printf("solve return %s at %f\n", source, now.as_double());
                return time_step_device_count() > 0 ? netlist_time.from_fp(m_params.m_min_time_step) : netlist_time.zero();
            }

            backup(); // save voltages for backup and time step calculation

            // update all terminals for new time step
            m_last_step.op = now;

            ++m_stat_vsolver_calls.op;
            if (dynamic_device_count() != 0)
            {
                step(time_step_type.FORWARD, delta);
                var resched = solve_nr_base();

                if (resched)
                    return newton_loops_exceeded(delta);
            }
            else
            {
                step(time_step_type.FORWARD, delta);
                this.m_stat_calculations.op++;
                this.upstream_solve_non_dynamic();
                this.store();
            }

            if (m_params.m_dynamic_ts.op())
            {
                if (time_step_device_count() > 0)
                    return compute_next_time_step(delta.as_fp(), m_params.m_min_time_step, m_params.m_max_time_step);
            }

            if (time_step_device_count() > 0)
                return netlist_time.from_fp(m_params.m_max_time_step);

            return netlist_time.zero();
        }


        public void update_inputs()
        {
            // avoid recursive calls. Inputs are updated outside this call
            foreach (var inp in m_inputs)
                inp.push(inp.proxied_net().Q_Analog());
        }


        public size_t dynamic_device_count() { return m_dynamic_funcs.size(); }  //std::size_t dynamic_device_count() const noexcept { return m_dynamic_funcs.size(); }
        public size_t time_step_device_count() { return m_step_funcs.size(); }  //std::size_t time_step_device_count() const noexcept { return m_step_funcs.size(); }


        /// \brief reschedule solver execution
        ///
        /// Calls reschedule on main solver
        ///
        void reschedule(netlist_time ts)
        {
            m_main_solver.reschedule(this, ts);
        }


        /// \brief Immediately solve system at current time
        ///
        /// This should only be called from update and update_param events.
        /// It's purpose is to bring voltage values to the current time step.
        /// This will be called BEFORE updating object properties.
        public void solve_now()
        {
            // this should only occur outside of execution and thus
            // using time should be safe.

            netlist_time new_time_step = solve(exec().time(), "solve_now");

            update_inputs();

            if (time_step_device_count() > 0)
            {
                this.reschedule(netlist_time.from_fp(m_params.m_dynamic_ts.op() ? m_params.m_min_time_step : m_params.m_max_time_step));
            }
        }


        //template <typename F>
        public void change_state(Action f)  //void change_state(F f)
        {
            // We only need to update the net first if this is a time stepping net
            if (time_step_device_count() > 0)
            {
                netlist_time new_time_step = solve(exec().time(), "change_state");
                update_inputs();
            }

            f();

            if (time_step_device_count() > 0)
            {
                //throw new emu_unimplemented();
#if false
                PFDEBUG(printf("here2\n");)
#endif
                this.reschedule(netlist_time.from_fp(m_params.m_min_ts_ts.op()));
            }
            else
            {
                this.reschedule(netlist_time.quantum());
            }
        }


        //NETLIB_RESETI();
        public override void reset()
        {
            //m_last_step = netlist_time_ext::zero();
        }


        public virtual void log_stats()
        {
            //throw new emu_unimplemented();
#if false
#endif
        }


        protected virtual std.pair<string, string> create_solver_code(solver.static_compile_target target)
        {
            return new std.pair<string, string>("", new plib.pfmt("// solver doesn't support static compile\n\n").op());
        }


        // return number of floating point operations for solve
        //std::size_t ops() const { return m_ops; }


        protected abstract void upstream_solve_non_dynamic();
        protected abstract netlist_time compute_next_time_step(nl_fptype cur_ts, matrix_solver_t_fptype min_ts, matrix_solver_t_fptype max_ts);
        protected abstract bool check_err();
        protected abstract void store();
        protected abstract void backup();
        protected abstract void restore();


        protected size_t max_rail_start()
        {
            size_t max_rail = 0;
            foreach (var term in m_terms)
                max_rail = std.max(max_rail, term.rail_start());

            return max_rail;
        }


        // base setup - called from constructor
        protected void setup_base(setup_t setup, matrix_solver_t_net_list_t nets)
        {
            log().debug.op("New solver setup\n");
            std.vector<core_device_t> step_devices = new std.vector<core_device_t>();
            std.vector<core_device_t> dynamic_devices = new std.vector<core_device_t>();

            m_terms.clear();

            foreach (var net in nets)
            {
                m_terms.emplace_back(new terms_for_net_t(net));  //m_terms.emplace_back(m_arena, net);
                m_rails_temp.emplace_back(new terms_for_net_t());  //m_rails_temp.emplace_back(m_arena);
            }

            for (size_t k = 0; k < nets.size(); k++)
            {
                //std::vector<detail::core_terminal_t *> temp;

                analog_net_t net = nets[k];

                // FIXME: add size() to list
                //log().debug.op("adding net with {0} populated connections\n", net.core_terms(net).size());

                net.set_solver(this);

                foreach (detail.core_terminal_t p in net.core_terms_copy())
                {
                    nl_assert_always(p.net() == net, "Net integrity violated");

                    log().debug.op("{0} {1} {2}\n", p.name(), net.name(), net.is_rail_net());

                    switch (p.type())
                    {
                        case detail.terminal_type.TERMINAL:
                            if (p.device().is_time_step())
                            {
                                if (!plib.container.contains(step_devices, p.device()))
                                    step_devices.push_back(p.device());
                            }

                            if (p.device().is_dynamic())
                            {
                                if (!plib.container.contains(dynamic_devices, p.device()))
                                    dynamic_devices.push_back(p.device());
                            }

                            {
                                var pterm = (terminal_t)p;
                                nl_assert_always(pterm != default, "cast to terminal_t * failed");
                                add_term((int)k, pterm);
                            }

                            log().debug.op("Added terminal {0}\n", p.name());
                            break;

                        case detail.terminal_type.INPUT:
                            {
                                proxied_analog_output_t net_proxy_output = null;
                                foreach (var input in m_inputs)
                                {
                                    if (input.proxied_net() == p.net())
                                    {
                                        net_proxy_output = input;
                                        break;
                                    }
                                }

                                if (net_proxy_output == null)
                                {
                                    string new_name = this.name() + "." + new plib.pfmt("m{0}").op(m_inputs.size());
                                    var proxied_net = (analog_net_t)p.net();
                                    nl_assert_always(proxied_net != null, "Net is not an analog net");
                                    var net_proxy_output_u = new proxied_analog_output_t(this, new_name, proxied_net);  //auto net_proxy_output_u = state().make_pool_object<proxied_analog_output_t>(*this, new_name, *proxied_net);
                                    net_proxy_output = net_proxy_output_u;
                                    m_inputs.emplace_back(net_proxy_output_u);
                                }

                                net.remove_terminal(p);
                                net_proxy_output.net().add_terminal(p);

                                // FIXME: repeated calling - kind of brute force
                                net_proxy_output.net().rebuild_list();

                                log().debug.op("Added input {0}", net_proxy_output.name());
                            }
                            break;

                        case detail.terminal_type.OUTPUT:
                            log().fatal.op(MF_UNHANDLED_ELEMENT_1_FOUND(p.name()));
                            throw new nl_exception(MF_UNHANDLED_ELEMENT_1_FOUND(p.name()));
                    }
                }

                net.rebuild_list();
            }

            foreach (var d in step_devices)
                m_step_funcs.emplace_back(d.time_step);
            foreach (var d in dynamic_devices)
                m_dynamic_funcs.emplace_back(d.update_terminals);
        }


        bool solve_nr_base()
        {
            bool this_resched = false;
            size_t newton_loops = 0;
            do
            {
                update_dynamic();
                // Gauss-Seidel will revert to Gaussian elimination if steps exceeded.
                this.m_stat_calculations.op++;
                this.upstream_solve_non_dynamic();
                this_resched = this.check_err();
                this.store();
                newton_loops++;
            } while (this_resched && newton_loops < m_params.m_nr_loops.op());

            m_stat_newton_raphson.op += newton_loops;
            if (this_resched)
                m_stat_newton_raphson_fail.op++;

            return this_resched;
        }


        netlist_time newton_loops_exceeded(netlist_time delta)
        {
            netlist_time next_time_step = new netlist_time();
            bool resched = false;

            restore();
            step(time_step_type.RESTORE, delta);

            for (size_t i = 0; i < 10; i++)
            {
                backup();
                step(time_step_type.FORWARD, netlist_time.from_fp(m_params.m_min_ts_ts.op()));
                resched = solve_nr_base();
                // update time step calculation
                next_time_step = compute_next_time_step(m_params.m_min_ts_ts.op(), m_params.m_min_ts_ts.op(), m_params.m_max_time_step);
                delta -= netlist_time.from_fp(m_params.m_min_ts_ts.op());
            }

            // try remaining time using compute_next_time step
            while (delta > netlist_time.zero())
            {
                if (next_time_step > delta)
                    next_time_step = delta;

                backup();
                step(time_step_type.FORWARD, next_time_step);
                delta -= next_time_step;
                resched = solve_nr_base();
                next_time_step = compute_next_time_step(next_time_step.as_fp(), m_params.m_min_ts_ts.op(), m_params.m_max_time_step);
            }

            if (m_stat_newton_raphson.op % 100 == 0)
                log().warning.op(MW_NEWTON_LOOPS_EXCEEDED_INVOCATION_3(100, this.name(), exec().time().as_double() * 1e6));

            if (resched)
            {
                // reschedule ....
                log().warning.op(MW_NEWTON_LOOPS_EXCEEDED_ON_NET_2(this.name(), exec().time().as_double() * 1e6));
                return netlist_time.from_fp(m_params.m_nr_recalc_delay.op());
            }
            if (m_params.m_dynamic_ts.op())
                return next_time_step;

            return netlist_time.from_fp(m_params.m_max_time_step);
        }


        /// \brief Sort terminals
        ///
        /// @param sort Sort algorithm to use.
        ///
        /// Sort in descending order by number of connected matrix voltages.
        /// The idea is, that for Gauss-Seidel algorithm the first voltage computed
        /// depends on the greatest number of previous voltages thus taking into
        /// account the maximum amount of information.
        ///
        /// This actually improves performance on popeye slightly. Average
        /// GS computations reduce from 2.509 to 2.370
        ///
        /// Smallest to largest : 2.613
        /// Unsorted            : 2.509
        /// Largest to smallest : 2.370
        //
        /// Sorting as a general matrix pre-conditioning is mentioned in
        /// literature but I have found no articles about Gauss Seidel.
        ///
        /// For Gaussian Elimination however increasing order is better suited.
        /// NOTE: Even better would be to sort on elements right of the matrix diagonal.
        /// FIXME: This entry needs an update.
        ///
        void sort_terms(matrix_sort_type_e sort)
        {
            size_t iN = m_terms.size();

            switch (sort)
            {
                case matrix_sort_type_e.PREFER_BAND_MATRIX:
                    {
                        for (size_t k = 0; k < iN - 1; k++)
                        {
                            var pk = get_weight_around_diagonal(k, k);
                            for (size_t i = k + 1; i < iN; i++)
                            {
                                var pi = get_weight_around_diagonal(i, k);
                                if (pi < pk)
                                {
                                    (m_terms[i], m_terms[k]) = (m_terms[k], m_terms[i]);  //std::swap(m_terms[i], m_terms[k]);
                                    pk = get_weight_around_diagonal(k, k);
                                }
                            }
                        }
                    }
                    break;
                case matrix_sort_type_e.PREFER_IDENTITY_TOP_LEFT:
                    {
                        for (size_t k = 0; k < iN - 1; k++)
                        {
                            var pk = get_left_right_of_diagonal(k, k);
                            for (size_t i = k + 1; i < iN; i++)
                            {
                                var pi = get_left_right_of_diagonal(i, k);
                                if (pi.first <= pk.first && pi.second >= pk.second)
                                {
                                    (m_terms[i], m_terms[k]) = (m_terms[k], m_terms[i]);  //std::swap(m_terms[i], m_terms[k]);
                                    pk = get_left_right_of_diagonal(k, k);
                                }
                            }
                        }
                    }
                    break;
                case matrix_sort_type_e.ASCENDING:
                case matrix_sort_type_e.DESCENDING:
                    {
                        int sort_order = (sort == matrix_sort_type_e.DESCENDING ? 1 : -1);

                        for (size_t k = 0; k < iN - 1; k++)
                        {
                            for (size_t i = k + 1; i < iN; i++)
                            {
                                if (((int)m_terms[k].rail_start() - (int)m_terms[i].rail_start()) * sort_order < 0)
                                    (m_terms[i], m_terms[k]) = (m_terms[k], m_terms[i]);  //std::swap(m_terms[i], m_terms[k]);
                            }
                        }
                    }
                    break;
                case matrix_sort_type_e.NOSORT:
                    break;
            }

            // rebuild
            foreach (var term in m_terms)
            {
                //int *other = term.m_connected_net_idx.data();
                for (size_t i = 0; i < term.count(); i++)
                {
                    //FIXME: this is weird
                    if (term.m_connected_net_idx[i] != -1)
                        term.m_connected_net_idx[i] = get_net_idx(get_connected_net(term.terms()[i]));
                }
            }
        }


        void update_dynamic()
        {
            // update all non-linear devices
            foreach (var dyn in m_dynamic_funcs)
                dyn();
        }


        void step(time_step_type ts_type, netlist_time delta)
        {
            var dd = delta.as_fp();
            foreach (var d in m_step_funcs)
                d(ts_type, dd);
        }


        int get_net_idx(analog_net_t net)
        {
            for (size_t k = 0; k < m_terms.size(); k++)
            {
                if (m_terms[k].is_net(net))
                    return (int)k;
            }
            return -1;
        }


        std.pair<int, int> get_left_right_of_diagonal(size_t irow, size_t idiag)
        {
            //
            // return the maximum column left of the diagonal (-1 if no cols found)
            // return the minimum column right of the diagonal (999999 if no cols found)
            //

            var row = (int)irow;
            var diag = (int)idiag;

            int colmax = -1;
            int colmin = 999999;

            var term = m_terms[irow];

            for (size_t i = 0; i < term.count(); i++)
            {
                var col = get_net_idx(get_connected_net(term.terms()[i]));
                if (col != -1)
                {
                    if (col == row) col = diag;
                    else if (col == diag) col = row;

                    if (col > diag && col < colmin)
                        colmin = col;
                    else if (col < diag && col > colmax)
                        colmax = col;
                }
            }

            return new std.pair<int, int>(colmax, colmin);
        }


        nl_fptype get_weight_around_diagonal(size_t row, size_t diag)
        {
            {
                //
                // return average absolute distance
                //

                std.vector<bool> touched = new std.vector<bool>(1024, false); // FIXME!

                nl_fptype weight = nlconst.zero();
                var term = m_terms[row];
                for (size_t i = 0; i < term.count(); i++)
                {
                    var col = get_net_idx(get_connected_net(term.terms()[i]));
                    if (col >= 0)
                    {
                        var colu = (size_t)col;  //auto colu = static_cast<std::size_t>(col);
                        if (!touched[colu])
                        {
                            if (colu == row) colu = (unsigned)diag;
                            else if (colu == diag) colu = (unsigned)row;

                            weight = weight + plib.pg.abs((nl_fptype)colu - (nl_fptype)diag);
                            touched[colu] = true;
                        }
                    }
                }

                return weight;
            }
        }


        void add_term(int net_idx, terminal_t term)
        {
            if (get_connected_net(term).is_rail_net())
            {
                m_rails_temp[net_idx].add_terminal(term, -1, false);
            }
            else
            {
                int ot = get_net_idx(get_connected_net(term));
                if (ot >= 0)
                {
                    m_terms[net_idx].add_terminal(term, ot, true);
                }
                else
                {
                    log().fatal.op(MF_FOUND_TERM_WITH_MISSING_OTHERNET(term.name()));
                    throw new nl_exception(MF_FOUND_TERM_WITH_MISSING_OTHERNET(term.name()));
                }
            }
        }


        // calculate matrix
        void setup_matrix()
        {
            size_t iN = m_terms.size();

            for (size_t k = 0; k < iN; k++)
            {
                m_terms[k].set_rail_start(m_terms[k].count());
                for (size_t i = 0; i < m_rails_temp[k].count(); i++)
                    this.m_terms[k].add_terminal(m_rails_temp[k].terms()[i], m_rails_temp[k].m_connected_net_idx[i], false);
            }

            // free all - no longer needed
            m_rails_temp.clear();

            sort_terms(m_params.m_sort_type.op());

            this.set_pointers();

            // create a list of non zero elements.
            for (unsigned k = 0; k < iN; k++)
            {
                terms_for_net_t t = m_terms[k];
                // pretty brutal
                var other = t.m_connected_net_idx;

                t.m_nz.clear();

                for (size_t i = 0; i < t.rail_start(); i++)
                {
                    if (!t.m_nz.Contains((unsigned)other[i]))  //if (!plib::container::contains(t->m_nz, static_cast<unsigned>(other[i])))
                        t.m_nz.push_back((unsigned)other[i]);
                }

                t.m_nz.push_back(k);     // add diagonal

                // and sort
                t.m_nz.Sort();  //std::sort(t.m_nz.begin(), t.m_nz.end());
            }

            // create a list of non zero elements right of the diagonal
            // These list anticipate the population of array elements by
            // Gaussian elimination.
            for (size_t k = 0; k < iN; k++)
            {
                terms_for_net_t t = m_terms[k];
                // pretty brutal
                var other = t.m_connected_net_idx;

                if (k == 0)
                {
                    t.m_nzrd.clear();
                }
                else
                {
                    t.m_nzrd = m_terms[k - 1].m_nzrd;
                    for (var jIdx = 0; jIdx < t.m_nzrd.Count;  )  //for (var j = t.nzrd.begin(); j != t.nzrd.end(); )
                    {
                        var j = t.m_nzrd[jIdx];

                        if (j < k + 1)
                            t.m_nzrd.erase(jIdx);
                        else
                            ++jIdx;
                    }
                }

                for (size_t i = 0; i < t.rail_start(); i++)
                {
                    if (!t.m_nzrd.Contains((unsigned)other[i]) && other[i] >= (int)(k + 1))  //if (!plib::container::contains(t->m_nzrd, static_cast<unsigned>(other[i])) && other[i] >= static_cast<int>(k + 1))
                        t.m_nzrd.push_back((unsigned)other[i]);
                }

                // and sort
                t.m_nzrd.Sort();  //std::sort(t.m_nzrd.begin(), t.m_nzrd.end());
            }

            // create a list of non zero elements below diagonal k
            // This should reduce cache misses ...

            //std::vector<std::vector<bool>> touched(iN, std::vector<bool>(iN));
            std.vector<std.vector<bool>> touched = new std.vector<std.vector<bool>>();
            for (size_t i = 0; i < iN; i++)
                touched.Add(new std.vector<bool>(iN));

            for (size_t k = 0; k < iN; k++)
            {
                for (size_t j = 0; j < iN; j++)
                    touched[k][j] = false;

                for (size_t j = 0; j < m_terms[k].m_nz.size(); j++)
                    touched[k][m_terms[k].m_nz[j]] = true;
            }

            m_ops = 0;
            for (unsigned k = 0; k < iN; k++)
            {
                m_ops++; // 1/A(k,k)
                for (unsigned row = k + 1; row < iN; row++)
                {
                    if (touched[row][k])
                    {
                        m_ops++;
                        if (!m_terms[k].m_nzbd.Contains(row))  //if (!plib::container::contains(m_terms[k]->m_nzbd, row))
                            m_terms[k].m_nzbd.push_back(row);

                        for (size_t col = k + 1; col < iN; col++)
                        {
                            if (touched[k][col])
                            {
                                touched[row][col] = true;
                                m_ops += 2;
                            }
                        }
                    }
                }
            }

            log().verbose.op("Number of multiplications/additions for {0}: {1}", name(), m_ops);

#if false
            // Dumps non zero elements right of diagonal -> to much output, disabled
            // NOLINTNEXTLINE(readability-simplify-boolean-expr)
            if ((false))
                for (std::size_t k = 0; k < iN; k++)
                {
                    pstring line = plib::pfmt("{1:3}")(k);
                    for (const auto & nzrd : m_terms[k].m_nzrd)
                        line += plib::pfmt(" {1:3}")(nzrd);
                    log().verbose("{1}", line);
                }
#endif

            //
            // save states
            //
            for (size_t k = 0; k < iN; k++)
            {
                string num = new plib.pfmt("{0}").op(k);

                state().save(this, m_gonn.op(k),"GO" + num, this.name(), m_terms[k].count());
                state().save(this, m_gtn.op(k),"GT" + num, this.name(), m_terms[k].count());
                state().save(this, m_Idrn.op(k),"IDR" + num , this.name(), m_terms[k].count());
            }
        }


        void set_pointers()
        {
            size_t iN = this.m_terms.size();

            size_t max_count = 0;
            size_t max_rail = 0;
            for (size_t k = 0; k < iN; k++)
            {
                max_count = std.max(max_count, m_terms[k].count());
                max_rail = std.max(max_rail, m_terms[k].rail_start());
            }

            m_gtn.resize(iN, max_count);
            m_gonn.resize(iN, max_count);
            m_Idrn.resize(iN, max_count);
            m_connected_net_Vn.resize(iN, max_count);

            // Initialize arrays to 0 (in case the vrl one is used
            for (size_t k = 0; k < iN; k++)
            {
                for (size_t j = 0; j < m_terms[k].count(); j++)
                {
                    m_gtn.set(k,j, nlconst.zero());
                    m_gonn.set(k,j, nlconst.zero());
                    m_Idrn.set(k,j, nlconst.zero());
                    m_connected_net_Vn.set(k, j, null);
                }
            }

            for (size_t k = 0; k < iN; k++)
            {
                var count = m_terms[k].count();

                for (size_t i = 0; i < count; i++)
                {
                    m_terms[k].terms()[i].set_ptrs(new Pointer<nl_fptype>(m_gtn.op(k), (int)i), new Pointer<nl_fptype>(m_gonn.op(k), (int)i), new Pointer<nl_fptype>(m_Idrn.op(k), (int)i));  //m_terms[k].terms()[i]->set_ptrs(&m_gtn[k][i], &m_gonn[k][i], &m_Idrn[k][i]);
                    m_connected_net_Vn.op(k)[i] = get_connected_net(m_terms[k].terms()[i]).Q_Analog_state_ptr();  //m_connected_net_Vn[k][i] = get_connected_net(m_terms[k].terms()[i])->Q_Analog_state_ptr();
                }
            }
        }


        analog_net_t get_connected_net(terminal_t term)
        {
            return state().setup().get_connected_terminal(term).net();
        }
    }
}
