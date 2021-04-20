// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    public static class nld_7450_global
    {
        // usage: TTL_7450_ANDORINVERT(name, cI1, cI2, cI3, cI4)
        //#define TTL_7450_ANDORINVERT(...)                                              \
        //        NET_REGISTER_DEVEXT(TTL_7450_ANDORINVERT, __VA_ARGS__)
        public static void TTL_7450_ANDORINVERT(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "TTL_7450_ANDORINVERT", name); }
    }
}
