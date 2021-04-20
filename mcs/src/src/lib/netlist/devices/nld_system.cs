// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


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
            //        PARAM(name.MODEL, family)

            //#define LOGIC_INPUT8(name, v, family)                                           \
            //        NET_REGISTER_DEV(LOGIC_INPUT8, name)                                    \
            //        PARAM(name.IN, v)                                                       \
            //        PARAM(name.MODEL, family)

            //#define ANALOG_INPUT(name, v)                                                   \
            //        NET_REGISTER_DEV(ANALOG_INPUT, name)                                    \
            //        PARAM(name.IN, v)
            public static void ANALOG_INPUT(nlparse_t setup, string name, int v)
            {
                nl_setup_global.NET_REGISTER_DEV(setup, "ANALOG_INPUT", name);
                nl_setup_global.PARAM(setup, name + ".IN", v);
            }

            //#define MAINCLOCK(name, freq)                                                   \
            //        NET_REGISTER_DEVEXT(MAINCLOCK, name, freq)

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

            //#define NC_PIN(name)                                                            \
            //        NET_REGISTER_DEV(NC_PIN, name)
            public static void NC_PIN(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEV(setup, "NC_PIN", name); }

            //FIXME: Usage discouraged, use OPTIMIZE_FRONTIER instead
            //#define FRONTIER_DEV(name, cIN, cG, cOUT)                                       \
            //        NET_REGISTER_DEV(FRONTIER_DEV, name)                                    \
            //        NET_C(cIN, name.I)                                                      \
            //        NET_C(cG,  name.G)                                                      \
            //        NET_C(cOUT, name.Q)

            // FIXME ... remove parameters
            //#define SYS_DSW(name, pI, p1, p2)                                              \
            //        NET_REGISTER_DEVEXT(SYS_DSW, name, pI, p1, p2)

            //#define SYS_DSW2(name)                                                         \
            //        NET_REGISTER_DEV(SYS_DSW2, name)
            public static void SYS_DSW2(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEV(setup, "SYS_DSW2", name); }

            //#define SYS_COMPD(name)                                                        \
            //        NET_REGISTER_DEV(SYS_COMPD, name)
            public static void SYS_COMPD(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEV(setup, "SYS_COMPD", name); }

            //#define SYS_NOISE_MT_U(name, pSIGMA)                                           \
            //        NET_REGISTER_DEVEXT(SYS_NOISE_MT_U, name, pSIGMA)

            //#define SYS_NOISE_MT_N(name, pSIGMA)                                           \
            //        NET_REGISTER_DEVEXT(SYS_NOISE_MT_N, name, pSIGMA)

            /* Default device to hold netlist parameters */
            //#define PARAMETERS(name)                                                        \
            //        NET_REGISTER_DEV(PARAMETERS, name)

            //#define AFUNC(name, p_N, p_F)                                                   \
            //        NET_REGISTER_DEV(AFUNC, name)                                           \
            //        PARAM(name.N, p_N)                                                      \
            //        PARAM(name.FUNC, p_F)
            public static void AFUNC(nlparse_t setup, string name, double p_N, string p_F)
            {
                nl_setup_global.NET_REGISTER_DEV(setup, "AFUNC", name);
                nl_setup_global.PARAM(setup, name + ".N", p_N);
                nl_setup_global.PARAM(setup, name + ".FUNC", p_F);
            }


            //NETLIB_DEVICE_IMPL(dummy_input, "DUMMY_INPUT",            "")
            //NETLIB_DEVICE_IMPL(frontier, "FRONTIER_DEV",           "+I,+G,+Q")
            //NETLIB_DEVICE_IMPL(function, "AFUNC",                  "N,FUNC")
            //NETLIB_DEVICE_IMPL(analog_input,        "ANALOG_INPUT",           "IN")
            //NETLIB_DEVICE_IMPL(clock,               "CLOCK",                  "FREQ")
            //NETLIB_DEVICE_IMPL(varclock,            "VARCLOCK",               "FUNC")
            //NETLIB_DEVICE_IMPL(extclock,            "EXTCLOCK",               "FREQ,PATTERN")
            //NETLIB_DEVICE_IMPL(res_sw,              "RES_SWITCH",             "+I,+1,+2")
            //NETLIB_DEVICE_IMPL(mainclock,           "MAINCLOCK",              "FREQ")
            //NETLIB_DEVICE_IMPL(gnd,                 "GND",                    "")
            //NETLIB_DEVICE_IMPL(netlistparams,       "PARAMETER",              "")

            //using NETLIB_NAME(logic_input8) = NETLIB_NAME(logic_inputN)<8>;
            //NETLIB_DEVICE_IMPL(logic_input8,         "LOGIC_INPUT8",            "IN,FAMILY")

            //NETLIB_DEVICE_IMPL(logic_input, "LOGIC_INPUT", "IN,FAMILY")
            //NETLIB_DEVICE_IMPL_ALIAS(logic_input_ttl, logic_input, "TTL_INPUT", "IN")
        }
    }
}
