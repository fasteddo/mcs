// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using netlist_time_ext = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time_ext = plib::ptime<std::conditional<config::prefer_int128::value && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;


namespace mame.netlist
{
    // -----------------------------------------------------------------------------
    // logic_t
    // -----------------------------------------------------------------------------
    public class logic_t : detail.core_terminal_t, 
                           logic_family_t
    {
        public logic_t(device_t dev, string aname, state_e terminal_state, nl_delegate delegate_)
            : base(dev, aname, terminal_state, delegate_)
        {
            m_logic_family = dev.logic_family();  //, logic_family_t(dev.logic_family())
        }


        // logic_family_t
        logic_family_desc_t m_logic_family;
        public logic_family_desc_t logic_family() { return m_logic_family; }
        public void set_logic_family(logic_family_desc_t fam) { m_logic_family = fam; }


        public new logic_net_t net() { return (logic_net_t)base.net(); }  //return plib::downcast<logic_net_t &>(core_terminal_t::net());
    }


    // -----------------------------------------------------------------------------
    // logic_input_t
    // -----------------------------------------------------------------------------
    public class logic_input_t : logic_t
    {
        public logic_input_t(device_t dev, string aname, nl_delegate delegate_)
            : base(dev, aname, state_e.STATE_INP_ACTIVE, delegate_)
        {
            state().setup().register_term(this);
        }


        public netlist_sig_t op()
        {
            //throw new emu_unimplemented();
#if false
            gsl_Expects(terminal_state() != STATE_INP_PASSIVE);
#endif
            if (config.use_copy_instead_of_reference)
                return m_Q_CIR.op;
            else
                return net().Q();
        }


        public void inactivate()
        {
            if (!is_state(state_e.STATE_INP_PASSIVE))
            {
                set_state(state_e.STATE_INP_PASSIVE);
                net().remove_from_active_list(this);
            }
        }


        public void activate()
        {
            if (is_state(state_e.STATE_INP_PASSIVE))
            {
                net().add_to_active_list(this);
                set_state(state_e.STATE_INP_ACTIVE);
            }
        }


        public void activate_hl()
        {
            if (is_state(state_e.STATE_INP_PASSIVE))
            {
                net().add_to_active_list(this);
                set_state(state_e.STATE_INP_HL);
            }
        }


        public void activate_lh()
        {
            if (is_state(state_e.STATE_INP_PASSIVE))
            {
                net().add_to_active_list(this);
                set_state(state_e.STATE_INP_LH);
            }
        }
    }


    // -----------------------------------------------------------------------------
    // logic_output_t
    // -----------------------------------------------------------------------------
    public class logic_output_t : logic_t
    {
        logic_net_t m_my_net;


        /// \brief logic output constructor
        ///
        /// The third parameter does nothing. It is provided only for
        /// compatibility with tristate_output_t in templatized device models
        ///
        /// \param dev Device owning this output
        /// \param aname The name of this output
        /// \param dummy Dummy parameter to allow construction like tristate
        /// output
        ///
        public logic_output_t(device_t dev, string aname, bool dummy = false)
            : base(dev, aname, state_e.STATE_OUT, null)  //: logic_t(dev, aname, STATE_OUT, nldelegate())
        {
            m_my_net = new logic_net_t(dev.state(), name() + ".net", this);


            this.set_net(m_my_net);
            state().register_net(m_my_net);  //state().register_net(device_arena::owned_ptr<logic_net_t>(&m_my_net, false));
            state().setup().register_term(this);
        }


        public void initial(netlist_sig_t val)
        {
            if (has_net())
                net().initial(val);
        }


        public void push(netlist_sig_t newQ, netlist_time delay)
        {
            m_my_net.set_Q_and_push(newQ, delay); // take the shortcut
        }


        public void set_Q_time(netlist_sig_t newQ, netlist_time_ext at)
        {
            m_my_net.set_Q_time(newQ, at); // take the shortcut
        }


        /// \brief Dummy implementation for templatized generic devices
        ///
        /// This function shall never be called. It is defined here so that
        /// templatized generic device models do not have to do tons of
        /// template magic.
        ///
        /// This function terminates if actually called.
        ///
        //[[noreturn]] static void set_tristate([[maybe_unused]] netlist_sig_t v,
        //    [[maybe_unused]] netlist_time ts_off_on, [[maybe_unused]] netlist_time ts_on_off)
        //{
        //    plib::terminate("set_tristate on logic_output should never be called!");
        //}
    }


    // -----------------------------------------------------------------------------
    // tristate_output_t
    // -----------------------------------------------------------------------------

    /// \brief Tristate output
    ///
    /// In a lot of applications tristate enable inputs are just connected to
    /// VCC/GND to permanently enable the outputs. In this case a pure
    /// implementation using analog outputs would not perform well.
    ///
    /// For this object during creation it can be decided if a logic output or
    /// a tristate output is used. Generally the owning device uses parameter
    /// FORCE_TRISTATE_LOGIC to determine this.
    ///
    /// This is the preferred way to implement tristate outputs.
    ///

    class tristate_output_t : logic_output_t
    {
        //using logic_output_t::initial;
        //using logic_output_t::set_Q_time;


        //state_var<netlist_sig_t> m_last_logic;
        //state_var<netlist_sig_t> m_tristate;
        bool m_force_logic;


        tristate_output_t(device_t dev, string aname, bool force_logic)
            : base(dev, aname)
        {
            throw new emu_unimplemented();
#if false
            , m_last_logic(dev, name() + "." + "m_last_logic", 1) // force change
            , m_tristate(dev, name() + "." + "m_tristate", force_logic ? 0 : 2) // force change
            , m_force_logic(force_logic)
#endif
        }


        //void push(netlist_sig_t newQ, netlist_time delay) noexcept
        //{
        //    if (!m_tristate)
        //        logic_output_t::push(newQ, delay);
        //    m_last_logic = newQ;
        //}

        //void set_tristate(netlist_sig_t v,
        //    netlist_time ts_off_on, netlist_time ts_on_off) noexcept
        //{
        //    if (!m_force_logic)
        //        if (v != m_tristate)
        //        {
        //            logic_output_t::push((v != 0) ? OUT_TRISTATE() : m_last_logic, v ? ts_off_on : ts_on_off);
        //            m_tristate = v;
        //        }
        //}

        public bool is_force_logic()
        {
            return m_force_logic;
        }
    }
} // namespace netlist
