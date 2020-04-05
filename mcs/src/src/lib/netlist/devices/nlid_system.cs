// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_sig_t = System.UInt32;
using netlist_time = mame.netlist.ptime_u64;  //using netlist_time = ptime<std::uint64_t, NETLIST_INTERNAL_RES>;


namespace mame.netlist
{
    namespace devices
    {
        // -----------------------------------------------------------------------------
        // netlistparams
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(netlistparams)
        public class nld_netlistparams : device_t
        {
            //NETLIB_DEVICE_IMPL(netlistparams,       "PARAMETER",              "")
            static factory.element_t nld_netlistparams_c(string classname)
            { return new factory.device_element_t<nld_netlistparams>("PARAMETER", classname, "", "__FILE__"); }
            public static factory.constructor_ptr_t decl_netlistparams = nld_netlistparams_c;


            param_logic_t m_use_deactivate;
            param_int_t m_startup_strategy;


            //NETLIB_CONSTRUCTOR(netlistparams)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_netlistparams(object owner, string name)
                : base(owner, name)
            {
                m_use_deactivate = new param_logic_t(this, "USE_DEACTIVATE", false);
                m_startup_strategy = new param_int_t(this, "STARTUP_STRATEGY", 1);
            }


            public param_logic_t use_deactivate { get { return m_use_deactivate; } }


            //NETLIB_UPDATEI() { }
            protected override void update() { }


            //NETLIB_RESETI() { }
            //NETLIB_UPDATE_PARAMI() { }
        }


        // -----------------------------------------------------------------------------
        // mainclock
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(mainclock)
        class nld_mainclock : device_t
        {
            logic_output_t m_Q;
            netlist_time m_inc;
            param_double_t m_freq;


            //NETLIB_CONSTRUCTOR(mainclock)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            nld_mainclock(object owner, string name)
                : base(owner, name)
            {
                m_Q = new logic_output_t(this, "Q");
                m_freq = new param_double_t(this, "FREQ", 7159000.0 * 5);


                m_inc = netlist_time.from_double(1.0 / (m_freq.op()*2.0));
            }


            public logic_output_t Q { get { return m_Q; } }
            public netlist_time inc { get { return m_inc; } }


            //NETLIB_RESETI()
            public override void reset()
            {
                throw new emu_unimplemented();
#if false
                m_Q.net().set_next_scheduled_time(netlist_time::zero());
#endif
            }

            //NETLIB_UPDATE_PARAMI()
            public override void update_param()
            {
                throw new emu_unimplemented();
#if false
                m_inc = netlist_time::from_double(1.0 / (m_freq()*2.0));
#endif
            }

            //NETLIB_UPDATEI()
            protected override void update()
            {
                throw new emu_unimplemented();
#if false
                logic_net_t &net = m_Q.net();
                // this is only called during setup ...
                net.toggle_new_Q();
                net.set_next_scheduled_time(exec().time() + m_inc);
#endif
            }
        }


        // -----------------------------------------------------------------------------
        // clock
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(clock)


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
            static factory.element_t nld_logic_input_c(string classname)
            { return new factory.device_element_t<nld_logic_input>("LOGIC_INPUT", classname, "IN,FAMILY", "__FILE__"); }
            public static factory.constructor_ptr_t decl_logic_input = nld_logic_input_c;


            logic_output_t m_Q;

            param_logic_t m_IN;
            param_model_t m_FAMILY;


            //NETLIB_CONSTRUCTOR(logic_input)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_logic_input(object owner, string name)
                : base(owner, name)
            {
                m_Q = new logic_output_t(this, "Q");
                m_IN = new param_logic_t(this, "IN", false);
                /* make sure we get the family first */
                m_FAMILY = new param_model_t(this, "FAMILY", "FAMILY(TYPE=TTL)");


                set_logic_family(setup().family_from_model(m_FAMILY.op()));
                m_Q.set_logic_family(this.logic_family());
            }


            //NETLIB_UPDATEI();
            protected override void update() { }

            //NETLIB_RESETI();
            public override void reset() { m_Q.initial(0); }

            //NETLIB_UPDATE_PARAMI();
            public override void update_param() { m_Q.push((netlist_sig_t)((m_IN.op() ? 1 : 0) & 1), netlist_time.from_nsec(1)); }
        }


        //NETLIB_OBJECT(analog_input)
        class nld_analog_input : device_t
        {
            //NETLIB_DEVICE_IMPL(analog_input,        "ANALOG_INPUT",           "IN")
            static factory.element_t nld_analog_input_c(string classname)
            { return new factory.device_element_t<nld_analog_input>("ANALOG_INPUT", classname, "IN", "__FILE__"); }
            public static factory.constructor_ptr_t decl_analog_input = nld_analog_input_c;


            analog_output_t m_Q;
            param_double_t m_IN;


            //NETLIB_CONSTRUCTOR(analog_input)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_analog_input(object owner, string name)
                : base(owner, name)
            {
                m_Q = new analog_output_t(this, "Q");
                m_IN = new param_double_t(this, "IN", 0.0);
            }


            //NETLIB_UPDATEI();
            protected override void update() { }

            //NETLIB_RESETI();
            public override void reset() { m_Q.initial(0.0); }

            //NETLIB_UPDATE_PARAMI();
            public override void update_param() { m_Q.push(m_IN.op()); }
        }


        // -----------------------------------------------------------------------------
        // nld_gnd
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(gnd)
        class nld_gnd : device_t
        {
            //NETLIB_DEVICE_IMPL(gnd,                 "GND",                    "")
            static factory.element_t nld_gnd_c(string classname)
            { return new factory.device_element_t<nld_gnd>("GND", classname, "", "__FILE__"); }
            public static factory.constructor_ptr_t decl_gnd = nld_gnd_c;


            analog_output_t m_Q;


            //NETLIB_CONSTRUCTOR(gnd)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_gnd(object owner, string name)
                : base(owner, name)
            {
                m_Q = new analog_output_t(this, "Q");
            }

            //NETLIB_UPDATEI()
            protected override void update()
            {
                m_Q.push(0.0);
            }

            //NETLIB_RESETI() { }
            public override void reset() { }
        }


        // -----------------------------------------------------------------------------
        // nld_dummy_input
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT_DERIVED(dummy_input, base_dummy)
        class nld_dummy_input : nld_base_dummy
        {
            //NETLIB_DEVICE_IMPL(dummy_input, "DUMMY_INPUT",            "")
            static factory.element_t nld_dummy_input_c(string classname)
            { return new factory.device_element_t<nld_dummy_input>("DUMMY_INPUT", classname, "", "__FILE__"); }
            public static factory.constructor_ptr_t decl_dummy_input = nld_dummy_input_c;


            //analog_input_t m_I;


            //NETLIB_CONSTRUCTOR_DERIVED(dummy_input, base_dummy)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            nld_dummy_input(netlist_t owner, string name)
                : base(owner, name)
            {
                throw new emu_unimplemented();
#if false
                m_I(*this, "I")
#endif
            }


            //NETLIB_RESETI() { }
            public override void reset() { }
            //NETLIB_UPDATEI() { }
            protected override void update() { }
        }


        // -----------------------------------------------------------------------------
        // nld_frontier
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT_DERIVED(frontier, base_dummy)


        /* -----------------------------------------------------------------------------
         * nld_function
         *
         * FIXME: Currently a proof of concept to get congo bongo working
         * ----------------------------------------------------------------------------- */
        //NETLIB_OBJECT(function)


        // -----------------------------------------------------------------------------
        // nld_res_sw
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(res_sw)

    } //namespace devices
} // namespace netlist
