// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    public static class nld_7475_global
    {
        //#define TTL_7475_GATE(...)                                                     \
        //        NET_REGISTER_DEVEXT(TTL_7475_GATE, __VA_ARGS__)
        public static void TTL_7475_GATE(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "TTL_7475_GATE", name); }

        //#define TTL_7477_GATE(...)                                                     \
        //        NET_REGISTER_DEVEXT(TTL_7477_GATE, __VA_ARGS__)
        public static void TTL_7477_GATE(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "TTL_7477_GATE", name); }
    }
}
