// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using log_type = mame.plib.plog_base<mame.netlist.nl_config_global.bool_const_NL_DEBUG>;  //using log_type =  plib::plog_base<NL_DEBUG>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using state_var_s32 = mame.netlist.state_var<System.Int32>;  //using state_var_s32 = state_var<std::int32_t>;


namespace mame.netlist
{
    // -----------------------------------------------------------------------------
    // core_device_t
    // -----------------------------------------------------------------------------
    // FIXME: belongs into detail namespace
    public class core_device_t : detail.netlist_object_t
    {
        protected delegate void activate_delegate(bool param);  //using activate_delegate = plib::pmfp<void, bool>;


        protected activate_delegate m_activate;

        bool m_hint_deactivate;
        state_var_s32 m_active_outputs;
        stats_t m_stats;  //device_arena::unique_ptr<stats_t> m_stats;


        protected core_device_t(object owner, string name)
            : base(owner is netlist_state_t ? ((netlist_state_t)owner).exec() : ((core_device_t)owner).state().exec(),
                   owner is netlist_state_t ? name : ((core_device_t)owner).name() + "." + name)
        {
            if (owner is netlist_state_t) core_device_t_after_ctor((netlist_state_t)owner, name);
            else if (owner is core_device_t) core_device_t_after_ctor((core_device_t)owner, name);
            else throw new emu_unimplemented();
        }


        void core_device_t_after_ctor(netlist_state_t owner, string name)
        {
            m_hint_deactivate = false;
            m_active_outputs = new state_var_s32(this, "m_active_outputs", 1);


            if (exec().stats_enabled())
                m_stats = new stats_t();  //m_stats = owner.make_pool_object<stats_t>();
        }


        void core_device_t_after_ctor(core_device_t owner, string name)
        {
            m_hint_deactivate = false;
            m_active_outputs = new state_var_s32(this, "m_active_outputs", 1);


            //printf("owned device: %s\n", this->name().c_str());
            owner.state().register_device(this.name(), this);  //owner.state().register_device(this->name(), device_arena::owned_ptr<core_device_t>(this, false));
            if (exec().stats_enabled())
                m_stats = new stats_t();  //m_stats = owner.state().make_pool_object<stats_t>();
        }


        //PCOPYASSIGNMOVE(core_device_t, delete)

        //virtual ~core_device_t() noexcept = default;


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

                    m_activate(true);//inc_active();
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
                    m_activate(false); //dec_active();
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


        protected void handler_noop()
        {
        }


        protected log_type log()
        {
            return state().log();
        }


        public virtual void timestep(timestep_type ts_type, nl_fptype st) { }//plib::unused_var(ts_type, st); }
        public virtual void update_terminals() { }

        public virtual void update_param() { }
        public virtual bool is_dynamic() { return false; }
        public virtual bool is_timestep() { return false; }
    }


    // -----------------------------------------------------------------------------
    // base_device_t
    // -----------------------------------------------------------------------------
    public class base_device_t : core_device_t
    {
        //protected base_device_t(netlist_state_t owner, string name)
        //    : base(owner, name)
        //{ }

        //protected base_device_t(base_device_t owner, string name)
        //    : base(owner, name)
        //{ }

        protected base_device_t(object owner, string name)
            : base(owner, name)
        { }

        //PCOPYASSIGNMOVE(base_device_t, delete)

        //~base_device_t() noexcept override = default;

        //template<class O, class C, typename... Args>
        protected void create_and_register_subdevice(base_device_t owner, string name, out analog.nld_C out_) { out_ = new analog.nld_C(owner, name); } //void create_and_register_subdevice(O& owner, const pstring &name, device_arena::unique_ptr<C> &dev, Args&&... args);
        protected void create_and_register_subdevice(base_device_t owner, string name, out analog.nld_VCVS out_) { out_ = new analog.nld_VCVS(owner, name); } //void create_and_register_subdevice(O& owner, const pstring &name, device_arena::unique_ptr<C> &dev, Args&&... args);
        protected void create_and_register_subdevice(base_device_t owner, string name, out analog.nld_D out_, string model = "D") { out_ = new analog.nld_D(owner, name, model); } //void create_and_register_subdevice(O& owner, const pstring &name, device_arena::unique_ptr<C> &dev, Args&&... args);


        protected void register_subalias(string name, detail.core_terminal_t term)
        {
            string alias = this.name() + "." + name;

            // everything already fully qualified
            state().parser().register_alias_nofqn(alias, term.name());
        }


        protected void register_subalias(string name, string aliased)
        {
            string alias = this.name() + "." + name;
            string aliased_fqn = this.name() + "." + aliased;

            // everything already fully qualified
            state().parser().register_alias_nofqn(alias, aliased_fqn);
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
} // namespace netlist
