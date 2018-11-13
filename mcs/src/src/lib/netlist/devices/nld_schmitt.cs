// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nld_schmitt_global
    {
        // ----------------------------------------------------------------------------------------
        // Macros
        // ----------------------------------------------------------------------------------------

        //#define SCHMITT_TRIGGER(name, model)                                           \
        //        NET_REGISTER_DEV(SCHMITT_TRIGGER, name)                                \
        //        NETDEV_PARAMI(name, MODEL, model)
        public static void SCHMITT_TRIGGER(netlist.setup_t setup, string name, string model)
        {
            netlist.nl_setup_global.NET_REGISTER_DEV(setup, "SCHMITT_TRIGGER", name);
            netlist.nl_setup_global.NETDEV_PARAMI(setup, name, "MODEL", model);
        }
    }
}
