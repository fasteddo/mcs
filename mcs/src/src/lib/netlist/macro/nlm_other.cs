// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nlm_other_global
    {
        //#define MC14584B_GATE(name)                                                   \
        //        NET_REGISTER_DEV(MC14584B_GATE, name)
        public static void MC14584B_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "MC14584B_GATE", name); }

        //#define MC14584B_DIP(name)                                                    \
        //        NET_REGISTER_DEV(MC14584B_DIP, name)


        //static NETLIST_START(MC14584B_DIP)
        public static void netlist_MC14584B_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            MC14584B_GATE(setup, "A");
            MC14584B_GATE(setup, "B");
            MC14584B_GATE(setup, "C");
            MC14584B_GATE(setup, "D");
            MC14584B_GATE(setup, "E");
            MC14584B_GATE(setup, "F");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC", "D.VCC", "E.VCC", "F.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND", "D.GND", "E.GND", "F.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.A",   /*    A1 |1     ++    14| VCC  */ "A.VCC",
                "A.Q",   /*    Y1 |2           13| A6   */ "F.A",
                "B.A",   /*    A2 |3           12| Y6   */ "F.Q",
                "B.Q",   /*    Y2 |4  MC14584B 11| A5   */ "E.A",
                "C.A",   /*    A3 |5           10| Y5   */ "E.Q",
                "C.Q",   /*    Y3 |6            9| A4   */ "D.A",
                "A.GND", /*   GND |7            8| Y4   */ "D.Q"
                         /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //NETLIST_START(otheric_lib)
        public static void netlist_otheric_lib(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.TRUTHTABLE_START("MC14584B_GATE", 1, 1, "");
                netlist.nl_setup_global.TT_HEAD(" A | Q ");
                netlist.nl_setup_global.TT_LINE(" 0 | 1 |100");
                netlist.nl_setup_global.TT_LINE(" 1 | 0 |100");
                // 2.1V negative going and 2.7V positive going at 5V
                netlist.nl_setup_global.TT_FAMILY("FAMILY(IVL=0.42 IVH=0.54 OVL=0.05 OVH=0.05 ORL=10.0 ORH=10.0)");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "MC14584B_DIP", netlist_MC14584B_DIP);

            netlist.nl_setup_global.NETLIST_END();
        }
    }
}
