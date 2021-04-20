// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    public static class nld_fourterm_global
    {
        // ----------------------------------------------------------------------------------------
        // Macros
        // ----------------------------------------------------------------------------------------

        //#define VCCS(name, G)                                                         \
        //        NET_REGISTER_DEVEXT(VCCS, name, G)

        //#define CCCS(name, G)                                                         \
        //        NET_REGISTER_DEVEXT(CCCS, name, G)
        public static void CCCS(nlparse_t setup, string name, double G) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "CCCS", name, G.ToString()); }

        //#define VCVS(name, G)                                                         \
        //        NET_REGISTER_DEVEXT(VCVS, name, G)
        public static void VCVS(nlparse_t setup, string name, double G) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "VCVS", name, G.ToString()); }

        //#define CCVS(name, G)                                                         \
        //        NET_REGISTER_DEVEXT(CCVS, name, G)
        public static void CCVS(nlparse_t setup, string name, double G) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "CCVS", name, G.ToString()); }

        //#define LVCCS(name)                                                           \
        //        NET_REGISTER_DEV(LVCCS, name)
    }
}
