// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nld_bjt_global
    {
        // ----------------------------------------------------------------------------------------
        // Macros
        // ----------------------------------------------------------------------------------------

        //#define QBJT_SW(name, model)                                                 \
        //        NET_REGISTER_DEV(QBJT_SW, name)                                       \
        //        NETDEV_PARAMI(name,  MODEL, model)

        //#define QBJT_EB(name, model)                                                 \
        //        NET_REGISTER_DEV(QBJT_EB, name)                                       \
        //        NETDEV_PARAMI(name,  MODEL, model)
        public static void QBJT_EB(netlist.nlparse_t setup, string name, string model)
        {
            netlist.nl_setup_global.NET_REGISTER_DEV(setup, "QBJT_EB", name);
            netlist.nl_setup_global.NETDEV_PARAMI(setup, name, "MODEL", model);
        }
    }
}
