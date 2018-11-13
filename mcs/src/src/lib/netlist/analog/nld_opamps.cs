// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nld_opamps_global
    {
        // ----------------------------------------------------------------------------------------
        // Macros
        // ----------------------------------------------------------------------------------------

        //#define OPAMP(name, model)                                                     \
        //        NET_REGISTER_DEV(OPAMP, name)                                          \
        //        NETDEV_PARAMI(name, MODEL, model)
        public static void OPAMP(netlist.setup_t setup, string name, string model)
        {
            netlist.nl_setup_global.NET_REGISTER_DEV(setup, "OPAMP", name);
            netlist.nl_setup_global.NETDEV_PARAMI(setup, name, "MODEL", model);
        }
    }
}
