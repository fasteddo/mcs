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

            MC14584B_GATE(setup, "s1");
            MC14584B_GATE(setup, "s2");
            MC14584B_GATE(setup, "s3");
            MC14584B_GATE(setup, "s4");
            MC14584B_GATE(setup, "s5");
            MC14584B_GATE(setup, "s6");

            netlist.devices.nld_system_global.DUMMY_INPUT(setup, "GND");
            netlist.devices.nld_system_global.DUMMY_INPUT(setup, "VCC");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "s1.A",  /*    A1 |1     ++    14| VCC  */ "VCC.I",
                "s1.Q",  /*    Y1 |2           13| A6   */ "s6.A",
                "s2.A",  /*    A2 |3           12| Y6   */ "s6.Q",
                "s2.Q",  /*    Y2 |4  MC14584B 11| A5   */ "s5.A",
                "s3.A",  /*    A3 |5           10| Y5   */ "s5.Q",
                "s3.Q",  /*    Y3 |6            9| A4   */ "s4.A",
                "GND.I", /*   GND |7            8| Y4   */ "s4.Q"
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
                netlist.nl_setup_global.TT_FAMILY("FAMILY(FV=0 IVL=0.42 IVH=0.54 OVL=0.05 OVH=0.05 ORL=10.0 ORH=10.0)");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "MC14584B_DIP", netlist_MC14584B_DIP);

            netlist.nl_setup_global.NETLIST_END();
        }
    }
}
