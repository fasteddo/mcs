// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_time = mame.netlist.ptime_u64;  //using netlist_time = ptime<std::uint64_t, NETLIST_INTERNAL_RES>;


namespace mame.netlist
{
    namespace devices
    {
        public static class nld_system_global
        {
            // -----------------------------------------------------------------------------
            // Macros
            // -----------------------------------------------------------------------------

            //#define TTL_INPUT(name, v)                                                      \
            //        NET_REGISTER_DEV(TTL_INPUT, name)                                       \
            //        PARAM(name.IN, v)
            public static void TTL_INPUT(setup_t setup, string name, int v)
            {
                nl_setup_global.NET_REGISTER_DEV(setup, "TTL_INPUT", name);
                nl_setup_global.PARAM(setup, name + ".IN", v);
            }

            //#define LOGIC_INPUT(name, v, family)                                            \
            //        NET_REGISTER_DEV(LOGIC_INPUT, name)                                     \
            //        PARAM(name.IN, v)                                                       \
            //        PARAM(name.FAMILY, family)

            //#define ANALOG_INPUT(name, v)                                                   \
            //        NET_REGISTER_DEV(ANALOG_INPUT, name)                                    \
            //        PARAM(name.IN, v)
            public static void ANALOG_INPUT(setup_t setup, string name, int v)
            {
                nl_setup_global.NET_REGISTER_DEV(setup, "ANALOG_INPUT", name);
                nl_setup_global.PARAM(setup, name + ".IN", v);
            }

            //#define MAINCLOCK(name, freq)                                                   \
            //        NET_REGISTER_DEV(MAINCLOCK, name)                                       \
            //        PARAM(name.FREQ, freq)

            //#define CLOCK(name, freq)                                                       \
            //        NET_REGISTER_DEV(CLOCK, name)                                           \
            //        PARAM(name.FREQ, freq)

            //#define EXTCLOCK(name, freq, pattern)                                           \
            //        NET_REGISTER_DEV(EXTCLOCK, name)                                        \
            //        PARAM(name.FREQ, freq)                                                  \
            //        PARAM(name.PATTERN, pattern)

            //#define GNDA()                                                                  \
            //        NET_REGISTER_DEV(GNDA, GND)

            //#define DUMMY_INPUT(name)                                                       \
            //        NET_REGISTER_DEV(DUMMY_INPUT, name)
            public static void DUMMY_INPUT(setup_t setup, string name) { nl_setup_global.NET_REGISTER_DEV(setup, "DUMMY_INPUT", name); }

            //FIXME: Usage discouraged, use OPTIMIZE_FRONTIER instead
            //#define FRONTIER_DEV(name, cIN, cG, cOUT)                                       \
            //        NET_REGISTER_DEV(FRONTIER_DEV, name)                                    \
            //        NET_C(cIN, name.I)                                                      \
            //        NET_C(cG,  name.G)                                                      \
            //        NET_C(cOUT, name.Q)

            //#define RES_SWITCH(name, cIN, cP1, cP2)                                         \
            //        NET_REGISTER_DEV(RES_SWITCH, name)                                      \
            //        NET_C(cIN, name.I)                                                      \
            //        NET_C(cP1, name.1)                                                      \
            //        NET_C(cP2, name.2)

            /* Default device to hold netlist parameters */
            //#define PARAMETERS(name)                                                        \
            //        NET_REGISTER_DEV(PARAMETERS, name)

            //#define AFUNC(name, p_N, p_F)                                                   \
            //        NET_REGISTER_DEV(AFUNC, name)                                           \
            //        PARAM(name.N, p_N)                                                      \
            //        PARAM(name.FUNC, p_F)


            //NETLIB_DEVICE_IMPL(dummy_input)
            //NETLIB_DEVICE_IMPL(frontier)
            //NETLIB_DEVICE_IMPL(function)
            //NETLIB_DEVICE_IMPL(logic_input)
            //NETLIB_DEVICE_IMPL(analog_input)
            //NETLIB_DEVICE_IMPL(clock)
            //NETLIB_DEVICE_IMPL(extclock)
            //NETLIB_DEVICE_IMPL(res_sw)
            //NETLIB_DEVICE_IMPL(mainclock)
            //NETLIB_DEVICE_IMPL(gnd)
            //NETLIB_DEVICE_IMPL(netlistparams)
        }


        partial class nld_logic_input : device_t
        {
            //NETLIB_UPDATE_AFTER_PARAM_CHANGE()
            public override bool needs_update_after_param_change()
            {
                throw new emu_unimplemented();
            }

            //NETLIB_UPDATEI();
            //NETLIB_UPDATE(logic_input)
            protected override void update()
            {
                m_Q.push((m_IN.op() ? 1U : 0U) & 1, netlist_time.from_nsec(1));
            }

            //NETLIB_RESETI();
            //NETLIB_RESET(logic_input)
            protected override void reset()
            {
                m_Q.initial(0);
            }

            //NETLIB_UPDATE_PARAMI();
            //NETLIB_UPDATE_PARAM(logic_input)
            public override void update_param() { }
        }


        partial class nld_analog_input : device_t
        {
            //NETLIB_UPDATE_AFTER_PARAM_CHANGE()
            public override bool needs_update_after_param_change() { return true; }

            //NETLIB_UPDATEI();
            //NETLIB_UPDATE(analog_input)
            protected override void update()
            {
                m_Q.push(m_IN.op());
            }

            //NETLIB_RESETI();
            //NETLIB_RESET(analog_input)
            protected override void reset()
            {
                m_Q.initial(0.0);
            }

            //NETLIB_UPDATE_PARAMI();
            //NETLIB_UPDATE_PARAM(analog_input)
            public override void update_param() { }
        }
    }
}
