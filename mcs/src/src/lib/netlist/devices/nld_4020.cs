// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nld_4020_global
    {
        // usage       : CD4020(name)
        //#define CD4020(...)                                                    \
        //    NET_REGISTER_DEVEXT(CD4020, __VA_ARGS__)
        public static void CD4020(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEVEXT(setup, "CD4020", name); }


        // usage       : CD4024(name)
        //#define CD4024(...)                                                    \
        //    NET_REGISTER_DEVEXT(CD4024, __VA_ARGS__)
    }
}
