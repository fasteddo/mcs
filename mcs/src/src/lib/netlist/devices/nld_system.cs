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
            public static void TTL_INPUT(nlparse_t setup, string name, int v)
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
            public static void ANALOG_INPUT(nlparse_t setup, string name, int v)
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

            //#define VARCLOCK(name, func)                                                    \
            //        NET_REGISTER_DEV(VARCLOCK, name)                                        \
            //        PARAM(name.FUNC, func)

            //#define EXTCLOCK(name, freq, pattern)                                           \
            //        NET_REGISTER_DEV(EXTCLOCK, name)                                        \
            //        PARAM(name.FREQ, freq)                                                  \
            //        PARAM(name.PATTERN, pattern)

            //#define GNDA()                                                                  \
            //        NET_REGISTER_DEV(GNDA, GND)

            //#define DUMMY_INPUT(name)                                                       \
            //        NET_REGISTER_DEV(DUMMY_INPUT, name)
            public static void DUMMY_INPUT(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEV(setup, "DUMMY_INPUT", name); }

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


            //NETLIB_DEVICE_IMPL(dummy_input, "DUMMY_INPUT",            "")
            //NETLIB_DEVICE_IMPL(frontier, "FRONTIER_DEV",           "+I,+G,+Q")
            //NETLIB_DEVICE_IMPL(function, "AFUNC",                  "N,FUNC")
            //NETLIB_DEVICE_IMPL(analog_input,        "ANALOG_INPUT",           "IN")
            //NETLIB_DEVICE_IMPL(clock,               "CLOCK",                  "FREQ")
            //NETLIB_DEVICE_IMPL(varclock,            "VARCLOCK",               "FUNC")
            //NETLIB_DEVICE_IMPL(extclock,            "EXTCLOCK",               "FREQ,PATTERN")
            //NETLIB_DEVICE_IMPL(res_sw,              "RES_SWITCH",             "+IN,+P1,+P2")
            //NETLIB_DEVICE_IMPL(mainclock,           "MAINCLOCK",              "FREQ")
            //NETLIB_DEVICE_IMPL(gnd,                 "GND",                    "")
            //NETLIB_DEVICE_IMPL(netlistparams,       "PARAMETER",              "")

            //NETLIB_DEVICE_IMPL(logic_input, "LOGIC_INPUT", "IN,FAMILY")
            //NETLIB_DEVICE_IMPL_ALIAS(logic_input_ttl, logic_input, "TTL_INPUT", "IN")
        }
    }
}
