// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nld_fourterm_global
    {
        // ----------------------------------------------------------------------------------------
        // Macros
        // ----------------------------------------------------------------------------------------

        //#define VCCS(name)                                                            \
        //        NET_REGISTER_DEV(VCCS, name)
        public static void VCCS(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "VCCS", name); }

        //#define CCCS(name)                                                            \
        //        NET_REGISTER_DEV(CCCS, name)

        //#define VCVS(name)                                                            \
        //        NET_REGISTER_DEV(VCVS, name)
        public static void VCVS(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "VCVS", name); }

        //#define LVCCS(name)                                                           \
        //        NET_REGISTER_DEV(LVCCS, name)
    }
}
