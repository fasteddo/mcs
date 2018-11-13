// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nld_4316_global
    {
        //#define CD4316_GATE(name)                                                       \
        //        NET_REGISTER_DEV(CD4316_GATE, name)
        public static void CD4316_GATE(netlist.setup_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "CD4316_GATE", name); }
    }
}
