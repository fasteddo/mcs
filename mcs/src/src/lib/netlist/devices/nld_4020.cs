// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nld_4020_global
    {
        ///* FIXME: only used in mario.c */
        //#define CD4020_WI(name, cIP, cRESET, cVDD, cVSS)                                \
        //        NET_REGISTER_DEV(CD4020_WI, name)                                       \
        //        NET_CONNECT(name, IP, cIP)                                              \
        //        NET_CONNECT(name, RESET,  cRESET)                                       \
        //        NET_CONNECT(name, VDD,  cVDD)                                           \
        //        NET_CONNECT(name, VSS,  cVSS)

        //#define CD4020(name)                                                            \
        //        NET_REGISTER_DEV(CD4020, name)
        public static void CD4020(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "CD4020", name); }
    }
}
