// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_sig_t = System.UInt32;
using netlist_time = mame.plib.ptime_i64;  //using netlist_time = plib::ptime<std::int64_t, NETLIST_INTERNAL_RES>;


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
            param_num_t_unsigned m_startup_strategy;
            param_num_t_unsigned m_mos_capmodel;
            //! How many times do we try to resolve links (connections)
            param_num_t_unsigned m_max_link_loops;


            //NETLIB_CONSTRUCTOR(netlistparams)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_netlistparams(object owner, string name)
                : base(owner, name)
            {
                m_use_deactivate = new param_logic_t(this, "USE_DEACTIVATE", false);
                m_startup_strategy = new param_num_t_unsigned(this, "STARTUP_STRATEGY", 1);
                m_mos_capmodel = new param_num_t_unsigned(this, "DEFAULT_MOS_CAPMODEL", 2);
                m_max_link_loops = new param_num_t_unsigned(this, "MAX_LINK_RESOLVE_LOOPS", 100);
            }


            public param_logic_t use_deactivate { get { return m_use_deactivate; } }
            public param_num_t_unsigned mos_capmodel { get { return m_mos_capmodel; } }
            public param_num_t_unsigned max_link_loops { get { return m_max_link_loops; } }


            //NETLIB_UPDATEI() { }
            protected override void update() { }


            //NETLIB_RESETI() { }
            //NETLIB_UPDATE_PARAMI() { }
        }


        // -----------------------------------------------------------------------------
        // power pins - not a device, but a helper
        // -----------------------------------------------------------------------------
        /// \brief Power pins class.
        ///
        /// Power Pins are passive inputs. Delegate noop will silently ignore any
        /// updates.
        class nld_power_pins
        {
            analog_input_t m_VCC;
            analog_input_t m_GND;


            public nld_power_pins(device_t owner, string sVCC = nl_errstr_global.sPowerVCC, string sGND = nl_errstr_global.sPowerGND)
            {
                m_VCC = new analog_input_t(owner, sVCC, noop);
                m_GND = new analog_input_t(owner, sGND, noop);
            }


            analog_input_t VCC()
            {
                return m_VCC;
            }

            analog_input_t GND()
            {
                return m_GND;
            }


            void noop() { }
        }


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
            static factory.element_t nld_logic_input_c(string classname)
            { return new factory.device_element_t<nld_logic_input>("LOGIC_INPUT", classname, "IN,FAMILY", "__FILE__"); }
            public static factory.constructor_ptr_t decl_logic_input = nld_logic_input_c;


            logic_output_t m_Q;

            param_logic_t m_IN;
            param_model_t m_FAMILY;
            nld_power_pins m_supply;  //NETLIB_NAME(power_pins) m_supply;


            //NETLIB_CONSTRUCTOR(logic_input)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_logic_input(object owner, string name)
                : base(owner, name)
            {
                m_Q = new logic_output_t(this, "Q");
                m_IN = new param_logic_t(this, "IN", false);
                // make sure we get the family first
                m_FAMILY = new param_model_t(this, "FAMILY", "FAMILY(TYPE=TTL)");
                m_supply = new nld_power_pins(this);


                set_logic_family(state().setup().family_from_model(m_FAMILY.op()));
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


            //NETLIB_UPDATEI();
            protected override void update() { }

            //NETLIB_RESETI();
            public override void reset() { m_Q.initial(nlconst.zero()); }

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
                m_Q.push(nlconst.zero());
            }

            //NETLIB_RESETI() { }
            public override void reset() { }
        }


        // -----------------------------------------------------------------------------
        // nld_nc_pin
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(nc_pin)
        class nld_nc_pin : device_t
        {
            //NETLIB_DEVICE_IMPL(nc_pin,              "NC_PIN",                 "")
            static factory.element_t nld_nc_pin_c(string classname)
            { return new factory.device_element_t<nld_nc_pin>("NC_PIN", classname, "", "__FILE__"); }
            public static factory.constructor_ptr_t decl_nc_pin = nld_nc_pin_c;


            //analog_input_t m_I;


            //NETLIB_CONSTRUCTOR(nc_pin)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            nld_nc_pin(netlist_t owner, string name)
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
        //NETLIB_OBJECT(frontier)


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
