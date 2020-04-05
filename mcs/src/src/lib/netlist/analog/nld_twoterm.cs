// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    public static class nld_twoterm_global
    {
        // -----------------------------------------------------------------------------
        // Macros
        // -----------------------------------------------------------------------------

        //#define RES(name, p_R)                                                         \
        //        NET_REGISTER_DEV(RES, name)                                            \
        //        NETDEV_PARAMI(name, R, p_R)
        public static void RES(nlparse_t setup, string name, int p_R)
        {
            nl_setup_global.NET_REGISTER_DEV(setup, "RES", name);
            nl_setup_global.NETDEV_PARAMI(setup, name, "R", p_R);
        }

        //#define POT(name, p_R)                                                         \
        //        NET_REGISTER_DEV(POT, name)                                            \
        //        NETDEV_PARAMI(name, R, p_R)
        public static void POT(nlparse_t setup, string name, int p_R)
        {
            nl_setup_global.NET_REGISTER_DEV(setup, "POT", name);
            nl_setup_global.NETDEV_PARAMI(setup, name, "R", p_R);
        }

        /* Does not have pin 3 connected */
        //#define POT2(name, p_R)                                                        \
        //        NET_REGISTER_DEV(POT2, name)                                           \
        //        NETDEV_PARAMI(name, R, p_R)


        //#define CAP(name, p_C)                                                         \
        //        NET_REGISTER_DEV(CAP, name)                                            \
        //        NETDEV_PARAMI(name, C, p_C)
        public static void CAP(nlparse_t setup, string name, double p_C)
        {
            nl_setup_global.NET_REGISTER_DEV(setup, "CAP", name);
            nl_setup_global.NETDEV_PARAMI(setup, name, "C", p_C);
        }

        //#define IND(name, p_L)                                                         \
        //        NET_REGISTER_DEV(IND, name)                                            \
        //        NETDEV_PARAMI(name, L, p_L)

        /* Generic Diode */
        //#define DIODE(name,  model)                                                    \
        //        NET_REGISTER_DEV(DIODE, name)                                          \
        //        NETDEV_PARAMI(name, MODEL, model)

        //#define VS(name, pV)                                                           \
        //        NET_REGISTER_DEV(VS, name)                                             \
        //        NETDEV_PARAMI(name, V, pV)

        //#define CS(name, pI)                                                           \
        //        NET_REGISTER_DEV(CS, name)                                             \
        //        NETDEV_PARAMI(name, I, pI)
        public static void CS(nlparse_t setup, string name, double pI)
        {
            nl_setup_global.NET_REGISTER_DEV(setup, "CS", name);
            nl_setup_global.NETDEV_PARAMI(setup, name, "I", pI);
        }


        // -----------------------------------------------------------------------------
        // Generic macros
        // -----------------------------------------------------------------------------

        //#ifdef RES_R
        //#warning "Do not include rescap.h in a netlist environment"
        //#endif
        //#ifndef RES_R
        //#define RES_R(res) (static_cast<double>(res))
        //#define RES_K(res) (static_cast<double>(res) * 1e3)
        //#define RES_M(res) (static_cast<double>(res) * 1e6)
        //#define CAP_U(cap) (static_cast<double>(cap) * 1e-6)
        //#define CAP_N(cap) (static_cast<double>(cap) * 1e-9)
        public static double CAP_P(double cap) { return cap * 1e-12; }
        //#define IND_U(ind) (static_cast<double>(ind) * 1e-6)
        //#define IND_N(ind) (static_cast<double>(ind) * 1e-9)
        //#define IND_P(ind) (static_cast<double>(ind) * 1e-12)
        //#endif
    }
}
