// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    public static class nld_7474_global
    {
        // usage: TTL_7474(name, cCLK, cD, cCLRQ, cPREQ)
        //#define TTL_7474(...)                                             \
        //        NET_REGISTER_DEVEXT(TTL_7474, __VA_ARGS__)
        public static void TTL_7474(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "TTL_7474", name); }
    }
}
