// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using param_fp_t = mame.netlist.param_num_t<System.Double, mame.netlist.param_num_t_operators_double>;  //using param_fp_t = param_num_t<nl_fptype>;
using param_logic_t = mame.netlist.param_num_t<bool, mame.netlist.param_num_t_operators_bool>;  //using param_logic_t = param_num_t<bool>;
using unsigned = System.UInt32;

using static mame.nl_factory_global;


namespace mame.netlist
{
    namespace devices
    {
        // -----------------------------------------------------------------------------
        // mainclock
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(mainclock)
        class nld_mainclock : device_t
        {
            //NETLIB_DEVICE_IMPL(mainclock,           "MAINCLOCK",              "FREQ")
            public static readonly factory.constructor_ptr_t decl_mainclock = NETLIB_DEVICE_IMPL_ALIAS<nld_mainclock>("mainclock", "MAINCLOCK", "FREQ");


            public logic_output_t m_Q; // NOLINT: needed in core
            public netlist_time m_inc; // NOLINT: needed in core
            param_fp_t m_freq;


            //NETLIB_CONSTRUCTOR(mainclock)
            public nld_mainclock(object owner, string name)
                : base(owner, name)
            {
                m_Q = new logic_output_t(this, "Q");
                m_freq = new param_fp_t(this, "FREQ", nlconst.magic(7159000.0 * 5));


                m_inc = netlist_time.from_fp(plib.pg.reciprocal(m_freq.op() * nlconst.two()));
            }


            //NETLIB_RESETI()
            public override void reset()
            {
                m_Q.net().set_next_scheduled_time(exec().time());
            }


            //NETLIB_UPDATE_PARAMI()
            public override void update_param()
            {
                m_inc = netlist_time.from_fp(plib.pg.reciprocal(m_freq.op() * nlconst.two()));
            }
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
            //using this_type = nld_power_pins;


            analog_input_t m_VCC;
            analog_input_t m_GND;


            public nld_power_pins(device_t owner)
            {
                m_VCC = new analog_input_t(owner, owner.logic_family().vcc_pin(), noop);  //m_VCC(owner, owner.logic_family()->vcc_pin(), NETLIB_DELEGATE(noop))
                m_GND = new analog_input_t(owner, owner.logic_family().gnd_pin(), noop);  //m_GND(owner, owner.logic_family()->gnd_pin(), NETLIB_DELEGATE(noop))
            }


            nld_power_pins(device_t owner, nldelegate delegate_)
            {
                m_VCC = new analog_input_t(owner, owner.logic_family().vcc_pin(), delegate_);
                m_GND = new analog_input_t(owner, owner.logic_family().gnd_pin(), delegate_);
            }


            // Some devices like the 74LS629 have two pairs of supply pins.
            public nld_power_pins(device_t owner, string vcc, string gnd)
            {
                m_VCC = new analog_input_t(owner, vcc, noop);  //: m_VCC(owner, vcc, NETLIB_DELEGATE(noop))
                m_GND = new analog_input_t(owner, gnd, noop);  //, m_GND(owner, gnd, NETLIB_DELEGATE(noop))
            }


            // Some devices like the 74LS629 have two pairs of supply pins.
            nld_power_pins(device_t owner, string vcc, string gnd, nldelegate delegate_)
            {
                m_VCC = new analog_input_t(owner, vcc, delegate_);
                m_GND = new analog_input_t(owner, gnd, delegate_);
            }


            public analog_input_t VCC()
            {
                return m_VCC;
            }


            public analog_input_t GND()
            {
                return m_GND;
            }


            void noop() { }
        }


        // -----------------------------------------------------------------------------
        // netlistparams
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(netlistparams)
        public class nld_netlistparams : device_t
        {
            //NETLIB_DEVICE_IMPL(netlistparams,       "PARAMETER",              "")
            public static readonly factory.constructor_ptr_t decl_netlistparams = NETLIB_DEVICE_IMPL<nld_netlistparams>("PARAMETER", "");


            public param_logic_t m_use_deactivate;
            public param_num_t<unsigned, param_num_t_operators_uint32> m_startup_strategy;
            public param_num_t<unsigned, param_num_t_operators_uint32> m_mos_capmodel;
            //! How many times do we try to resolve links (connections)
            public param_num_t<unsigned, param_num_t_operators_uint32> m_max_link_loops;


            //NETLIB_CONSTRUCTOR(netlistparams)
            public nld_netlistparams(object owner, string name)
                : base(owner, name)
            { 
                m_use_deactivate = new param_logic_t(this, "USE_DEACTIVATE", false);
                m_startup_strategy = new param_num_t<unsigned, param_num_t_operators_uint32>(this, "STARTUP_STRATEGY", 0);
                m_mos_capmodel = new param_num_t<unsigned, param_num_t_operators_uint32>(this, "DEFAULT_MOS_CAPMODEL", 2);
                m_max_link_loops = new param_num_t<unsigned, param_num_t_operators_uint32>(this, "MAX_LINK_RESOLVE_LOOPS", 100);
            }


            //NETLIB_RESETI() {}
            //NETLIB_UPDATE_PARAMI() { }
        }


        // -----------------------------------------------------------------------------
        // nld_nc_pin
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(nc_pin)
        class nld_nc_pin : device_t
        {
            analog_input_t m_I;


            //NETLIB_CONSTRUCTOR(nc_pin)
            nld_nc_pin(object owner, string name)
                : base(owner, name)
            {
                m_I = new analog_input_t(this, "I", handler_noop);  //m_I(*this, "I", NETLIB_DELEGATE_NOOP())
            }


            //NETLIB_RESETI() {}
        }


        // -----------------------------------------------------------------------------
        // nld_gnd
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(gnd)
        class nld_gnd : device_t
        {
            //NETLIB_DEVICE_IMPL(gnd,                 "GNDA",                   "")
            public static readonly factory.constructor_ptr_t decl_gnd = NETLIB_DEVICE_IMPL<nld_gnd>("GNDA", "");


            analog_output_t m_Q;


            //NETLIB_CONSTRUCTOR(gnd)
            public nld_gnd(object owner, string name)
                : base(owner, name)
            {
                m_Q = new analog_output_t(this, "Q");
            }


            //NETLIB_UPDATE_PARAMI()
            public override void update_param()
            {
                m_Q.push(nlconst.zero());
            }


            //NETLIB_RESETI() {}
        }
    } // namespace devices
} // namespace netlist
