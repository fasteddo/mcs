// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using base_device_t_constructor_data_t = mame.netlist.core_device_data_t;  //using constructor_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;
using base_device_t_constructor_param_t = mame.netlist.core_device_data_t;  //using constructor_param_t = base_device_param_t;  //using base_device_param_t = const base_device_data_t &;  //using base_device_data_t = core_device_data_t;
using base_device_param_t = mame.netlist.core_device_data_t;  //using base_device_param_t = const base_device_data_t &;  //using base_device_data_t = core_device_data_t;
using core_device_param_t = mame.netlist.core_device_data_t;  //using core_device_param_t = const core_device_data_t &;
using log_type = mame.plib.plog_base<mame.netlist.nl_config_global.bool_const_NL_DEBUG>;  //using log_type =  plib::plog_base<NL_DEBUG>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using state_var_s32 = mame.netlist.state_var<System.Int32>;  //using state_var_s32 = state_var<std::int32_t>;


namespace mame.netlist
{
    // -------------------------------------------------------------------------
    // core_device_t construction parameters
    // -------------------------------------------------------------------------
    public class core_device_data_t
    {
        //friend class core_device_t;
        //friend class base_device_t;
        //friend class analog::NETLIB_NAME(two_terminal);
        //friend class logic_family_std_proxy_t;

        //template <unsigned m_NI, unsigned m_NO>
        //friend class devices::factory_truth_table_t;

        //template <class C, typename... Args>
        //friend class factory::device_element_t;
        //friend class factory::library_element_t;

        //template <typename CX>
        //friend struct sub_device_wrapper;

        //friend class solver::matrix_solver_t;


        public netlist_state_t owner;
        public string name;


        public core_device_data_t(netlist_state_t o, string n)
        {
            owner = o;
            name = n;
        }

        // MCS - added for convienence
        public core_device_data_t(core_device_t o, string n)
            : this(o.state(), o.name() + "." + n)
        {
        }
    }


    // The type use to pass data on
    //using core_device_param_t = const core_device_data_t &;


    // -----------------------------------------------------------------------------
    // core_device_t
    // -----------------------------------------------------------------------------
    // FIXME: belongs into detail namespace
    public class core_device_t : detail.netlist_object_t
    {
        protected delegate void activate_delegate(bool param);  //using activate_delegate = plib::pmfp<void (bool)>;


        protected activate_delegate m_activate;

        // FIXME: should this be a state_var?
        bool m_hint_deactivate;
        state_var_s32 m_active_outputs;
        stats_t m_stats;  //device_arena::unique_ptr<stats_t> m_stats;


        protected core_device_t(core_device_param_t data)
            : base(data.owner.exec(), data.name)
        {
            m_hint_deactivate = false;
            m_active_outputs = new state_var_s32(this, "m_active_outputs", 1);


            if (exec().stats_enabled())
                m_stats = new stats_t();  //m_stats = state().make_pool_object<stats_t>();
        }


        public void do_inc_active()
        {
            //throw new emu_unimplemented();
#if false
            gsl_Expects(m_active_outputs >= 0);
#endif

            if (m_activate != null && m_hint_deactivate)
            {
                if (++m_active_outputs.op == 1)
                {
                    //throw new emu_unimplemented();
#if false
                    if (m_stats)
                        m_stats->m_stat_inc_active.inc();
#endif
                    m_activate(true); // inc_active();
                }
            }
        }


        public void do_dec_active()
        {
            //throw new emu_unimplemented();
#if false
            gsl_Expects(m_active_outputs >= 1);
#endif

            if (m_activate != null && m_hint_deactivate)
            {
                if (--m_active_outputs.op == 0)
                {
                    m_activate(false); // dec_active();
                }
            }
        }


        public void set_hint_deactivate(bool v) { m_hint_deactivate = v; }


        //bool get_hint_deactivate() const noexcept { return m_hint_deactivate; }


        // Has to be set in device reset
        protected void set_active_outputs(int n) { m_active_outputs.op = n; }


        // stats
        public struct stats_t
        {
            // NL_KEEP_STATISTICS
            //plib::pperftime_t<true>  m_stat_total_time;
            //plib::pperfcount_t<true> m_stat_call_count;
            //plib::pperfcount_t<true> m_stat_inc_active;
        }

        public stats_t stats() { return m_stats; }


        public virtual void reset() { }


        protected void handler_noop() { }


        protected log_type log()
        {
            return state().log();
        }


        public virtual void time_step(time_step_type ts_type, nl_fptype st) { }
        public virtual void update_terminals() { }

        public virtual void update_param() { }
        public virtual bool is_dynamic() { return false; }
        public virtual bool is_time_step() { return false; }
    }


    // -------------------------------------------------------------------------
    // core_device_t construction parameters
    // -------------------------------------------------------------------------
    //using base_device_data_t = core_device_data_t;
    // The type use to pass data on
    //using base_device_param_t = const base_device_data_t &;


    // -----------------------------------------------------------------------------
    // base_device_t
    // -----------------------------------------------------------------------------
    public class base_device_t : core_device_t
    {
        //using constructor_data_t = base_device_data_t;
        //using constructor_param_t = base_device_param_t;


        protected base_device_t(base_device_param_t data)
            : base(data)
        {
        }


        //PCOPYASSIGNMOVE(base_device_t, delete)

        //~base_device_t() noexcept override = default;


        //template <class O, class C, typename... Args>
        //void create_and_register_sub_device(O &owner, const pstring &name,
        //    device_arena::unique_ptr<C> &dev, Args &&...args)
        //{
        //    // dev = state().make_pool_object<C>(owner, name,
        //    // std::forward<Args>(args)...);
        //    using dev_constructor_data_t = typename C::constructor_data_t;
        //    dev = state().make_pool_object<C>(
        //        dev_constructor_data_t{state(), owner.name() + "." + name},
        //        std::forward<Args>(args)...);
        //    state().register_device(dev->name(),
        //        device_arena::owned_ptr<core_device_t>(dev.get(), false));
        //}

        //template<class O, class C, typename... Args>
        protected void create_and_register_sub_device(base_device_t owner, string name, out analog.nld_C dev)  //void create_and_register_subdevice(O& owner, const pstring &name, device_arena::unique_ptr<C> &dev, Args&&... args);
        {
            dev = new analog.nld_C(new base_device_t_constructor_param_t(state(), owner.name() + "." + name));
            state().register_device(dev.name(), dev);
        }
        protected void create_and_register_sub_device(base_device_t owner, string name, out analog.nld_VCVS dev)  //void create_and_register_subdevice(O& owner, const pstring &name, device_arena::unique_ptr<C> &dev, Args&&... args);
        {
            dev = new analog.nld_VCVS(new base_device_t_constructor_param_t(state(), owner.name() + "." + name));
            state().register_device(dev.name(), dev);
        }
        protected void create_and_register_sub_device(base_device_t owner, string name, out analog.nld_D dev, string model = "D")  //void create_and_register_subdevice(O& owner, const pstring &name, device_arena::unique_ptr<C> &dev, Args&&... args);
        {
            dev = new analog.nld_D(new base_device_t_constructor_param_t(state(), owner.name() + "." + name), model);
            state().register_device(dev.name(), dev);
        }


        protected void register_sub_alias(string name, detail.core_terminal_t term)
        {
            string alias = this.name() + "." + name;

            // everything already fully qualified
            state().parser().register_alias_no_fqn(alias, term.name());
        }


        protected void register_sub_alias(string name, string aliased)
        {
            string alias = this.name() + "." + name;
            string aliased_fqn = this.name() + "." + aliased;

            // everything already fully qualified
            state().parser().register_alias_no_fqn(alias, aliased_fqn);
        }


        protected void connect(string t1, string t2)
        {
            state().parser().register_link_fqn(name() + "." + t1, name() + "." + t2);
        }


        protected void connect(detail.core_terminal_t t1, detail.core_terminal_t t2)
        {
            state().parser().register_link_fqn(t1.name(), t2.name());
        }


        //NETLIB_UPDATE_TERMINALSI() { }
    }
}
