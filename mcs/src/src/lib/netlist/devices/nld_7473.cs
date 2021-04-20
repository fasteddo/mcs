// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    public static class nld_7473_global
    {
        // usage: TTL_7473(name, cCLK, cJ, cK, cCLRQ)
        //#define TTL_7473(...)                                                          \
        //        NET_REGISTER_DEVEXT(TTL_7473, __VA_ARGS__)
        public static void TTL_7473(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "TTL_7473", name); }

        //#define TTL_7473A(...)                                                         \
        //        TTL_7473(__VA_ARGS__)
        public static void TTL_7473A(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "TTL_7473A", name); }
    }
}
