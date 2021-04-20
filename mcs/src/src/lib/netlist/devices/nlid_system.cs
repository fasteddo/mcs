// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using param_fp_t = mame.netlist.param_num_t<System.Double, mame.netlist.param_num_t_operators_double>;  //using param_fp_t = param_num_t<nl_fptype>;
using param_logic_t = mame.netlist.param_num_t<bool, mame.netlist.param_num_t_operators_bool>;  //using param_logic_t = param_num_t<bool>;
using unsigned = System.UInt32;


namespace mame.netlist.devices
{
    // -----------------------------------------------------------------------------
    // clock
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(clock)


    // -----------------------------------------------------------------------------
    // varclock
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(varclock)


    // -----------------------------------------------------------------------------
    // extclock
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(extclock)


    // -----------------------------------------------------------------------------
    // Special support devices ...
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(logic_input)
    class nld_logic_input : device_t
    {
        //NETLIB_DEVICE_IMPL(logic_input, "LOGIC_INPUT", "IN,FAMILY")
        public static readonly factory.constructor_ptr_t decl_logic_input = NETLIB_DEVICE_IMPL<nld_logic_input>("LOGIC_INPUT", "IN,FAMILY");

        //NETLIB_DEVICE_IMPL_ALIAS(logic_input_ttl, logic_input, "TTL_INPUT", "IN")
        public static readonly factory.constructor_ptr_t decl_logic_input_ttl = NETLIB_DEVICE_IMPL_ALIAS<nld_logic_input>("logic_input_ttl", "TTL_INPUT", "IN");


        logic_output_t m_Q;

        param_logic_t m_IN;
        nld_power_pins m_supply;  //NETLIB_NAME(power_pins) m_supply;


        //NETLIB_CONSTRUCTOR(logic_input)
        //detail.family_setter_t m_famsetter;
        //template <class CLASS>
        public nld_logic_input(object owner, string name)
            : base(owner, name)
        {
            m_Q = new logic_output_t(this, "Q");
            m_IN = new param_logic_t(this, "IN", false);
            m_supply = new nld_power_pins(this);
        }


        //NETLIB_RESETI();
        public override void reset() { m_Q.initial(0); }

        //NETLIB_UPDATE_PARAMI();
        public override void update_param()
        {
            //printf("%s %d\n", name().c_str(), m_IN());
            m_Q.push((netlist_sig_t)((m_IN.op() ? 1 : 0) & 1), netlist_time.from_nsec(1));
        }
    }


    //template<std::size_t N>
    //NETLIB_OBJECT(logic_inputN)


    //NETLIB_OBJECT(analog_input)
    class nld_analog_input : device_t
    {
        //NETLIB_DEVICE_IMPL(analog_input,        "ANALOG_INPUT",           "IN")
        public static readonly factory.constructor_ptr_t decl_analog_input = NETLIB_DEVICE_IMPL<nld_analog_input>("ANALOG_INPUT", "IN");


        analog_output_t m_Q;
        param_fp_t m_IN;


        //NETLIB_CONSTRUCTOR(analog_input)
        //detail.family_setter_t m_famsetter;
        //template <class CLASS>
        public nld_analog_input(object owner, string name)
            : base(owner, name)
        {
            m_Q = new analog_output_t(this, "Q");
            m_IN = new param_fp_t(this, "IN", nlconst.zero());
        }


        //NETLIB_RESETI();
        public override void reset() { m_Q.initial(nlconst.zero()); }

        //NETLIB_UPDATE_PARAMI();
        public override void update_param() { m_Q.push(m_IN.op()); }
    }


    // -----------------------------------------------------------------------------
    // nld_frontier
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(frontier)


    /* -----------------------------------------------------------------------------
        * nld_function
        *
        * FIXME: Currently a proof of concept to get congo bongo working
        * ----------------------------------------------------------------------------- */
    //NETLIB_OBJECT(function)


    // -----------------------------------------------------------------------------
    // nld_sys_dsw1
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(sys_dsw1)


    // -----------------------------------------------------------------------------
    // nld_sys_dsw2
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(sys_dsw2)


    // -----------------------------------------------------------------------------
    // nld_sys_comp
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(sys_compd)


    // -----------------------------------------------------------------------------
    // nld_sys_noise - noise source
    //
    // An externally clocked noise source.
    // -----------------------------------------------------------------------------
    //template <typename E, template<class> class D>
    //NETLIB_OBJECT(sys_noise)
}
