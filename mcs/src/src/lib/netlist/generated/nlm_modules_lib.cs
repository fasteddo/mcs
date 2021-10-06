// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nlm_modules_global
    {
        //NETLIST_START(modules_lib)
        public static void netlist_modules_lib(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.EXTERNAL_LIB_ENTRY("ICL8038_DIP", nlmod_icl8038_dip_global.netlist_ICL8038_DIP);
            h.EXTERNAL_LIB_ENTRY("NE556_DIP", nlmod_ne556_dip_global.netlist_NE556_DIP);
            h.EXTERNAL_LIB_ENTRY("RTEST", nlmod_rtest_global.netlist_RTEST);

            h.NETLIST_END();
        }
    }
}
