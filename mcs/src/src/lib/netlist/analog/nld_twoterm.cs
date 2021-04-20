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
        //        NET_REGISTER_DEVEXT(RES, name, p_R)
        public static void RES(nlparse_t setup, string name, double p_R)
        {
            nl_setup_global.NET_REGISTER_DEVEXT(setup, "RES", name, p_R.ToString());
        }

        //#define POT(name, p_R)                                                         \
        //        NET_REGISTER_DEVEXT(POT, name, p_R)
        public static void POT(nlparse_t setup, string name, int p_R)
        {
            nl_setup_global.NET_REGISTER_DEVEXT(setup, "POT", name, p_R.ToString());
        }

        // Does not have pin 3 connected
        //#define POT2(name, p_R)                                                        \
        //        NET_REGISTER_DEVEXT(POT2, name, p_R)


        //#define CAP(name, p_C)                                                         \
        //        NET_REGISTER_DEVEXT(CAP, name, p_C)
        public static void CAP(nlparse_t setup, string name, double p_C)
        {
            nl_setup_global.NET_REGISTER_DEVEXT(setup, "CAP", name, p_C.ToString());
        }

        //#define IND(name, p_L)                                                         \
        //        NET_REGISTER_DEVEXT(IND, name, p_L)

        // Generic Diode
        //#define DIODE(name,  model)                                                    \
        //        NET_REGISTER_DEVEXT(DIODE, name, model)

        //#define VS(name, pV)                                                           \
        //        NET_REGISTER_DEVEXT(VS, name, pV)

        //#define CS(name, pI)                                                           \
        //        NET_REGISTER_DEVEXT(CS, name, pI)
        public static void CS(nlparse_t setup, string name, double pI)
        {
            nl_setup_global.NET_REGISTER_DEVEXT(setup, "CS", name, pI.ToString());
        }
    }
}
