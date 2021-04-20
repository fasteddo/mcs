// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    public static class nld_7448_global
    {
        // usage       : TTL_7448(name, pA, pB, pC, pD, pLTQ, pBIQ, pRBIQ)
        // auto connect: VCC, GND
        //#define TTL_7448(...)                                                  \
        //    NET_REGISTER_DEVEXT(TTL_7448, __VA_ARGS__)
        public static void TTL_7448(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "TTL_7448", name); }
    }
}
