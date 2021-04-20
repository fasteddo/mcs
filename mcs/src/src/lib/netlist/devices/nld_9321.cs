// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    public static class nld_9321_global
    {
        // usage: TTL_9321(name, cAE, cA0, cA1, cBE, cB0, cB1)
        //#define TTL_9321(...)                                                     \
        //        NET_REGISTER_DEVEXT(TTL_9321, __VA_ARGS__)
        public static void TTL_9321(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "TTL_9321", name); }
    }
}
