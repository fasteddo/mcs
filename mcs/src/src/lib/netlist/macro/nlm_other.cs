// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nlm_other_global
    {
#if !NL_AUTO_DEVICES

        //#define MC14584B_GATE(name)                                                   \
        //        NET_REGISTER_DEV(MC14584B_GATE, name)
        public static void MC14584B_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "MC14584B_GATE", name); }

        //#define MC14584B_DIP(name)                                                    \
        //        NET_REGISTER_DEV(MC14584B_DIP, name)

        //#define NE566_DIP(name)                                                        \
        //        NET_REGISTER_DEV(NE566_DIP, name)

#endif


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

            netlist.nl_setup_global.NET_C(setup, "A.VDD", "B.VDD", "C.VDD", "D.VDD", "E.VDD", "F.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.VSS", "B.VSS", "C.VSS", "D.VSS", "E.VSS", "F.VSS");
            netlist.nl_setup_global.DIPPINS(setup,  /*       +--------------+      */
                "A.A",  /*    A1 |1     ++    14| VDD  */ "A.VDD",
                "A.Q",  /*    Y1 |2           13| A6   */ "F.A",
                "B.A",  /*    A2 |3           12| Y6   */ "F.Q",
                "B.Q",  /*    Y2 |4  MC14584B 11| A5   */ "E.A",
                "C.A",  /*    A3 |5           10| Y5   */ "E.Q",
                "C.Q",  /*    Y3 |6            9| A4   */ "D.A",
                "A.VSS",/*   VSS |7            8| Y4   */ "D.Q"
                      /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier:  NE566_DIP
        //- Title: NE566 Voltage Controlled Oscillator
        //- Description: The LM566CN is a general purpose voltage controlled oscillator
        //-    which may be used to generate square and triangula waves, the frequency
        //-    of which is a very linear function of a control voltage. The frequency
        //-    is also a function of an external resistor and capacitor.
        //-
        //-    The LM566CN is specified for operation over the 0°C to a 70°C
        //-    temperature range.
        //-
        //-    Applications
        //-
        //-    - FM modulation
        //-    - Signal generation
        //-    - Function generation
        //-    - Frequency shift keying
        //-    - Tone generation
        //-
        //-    Features
        //-    - Wide supply voltage range: 10V to 24V
        //-    - Very linear modulation characteristics
        //-    - High temperature stability
        //-    - Excellent supply voltage rejection
        //-    - 10 to 1 frequency range with fixed capacitor
        //-    - Frequency programmable by means of current, voltage, resistor or capacitor
        //-
        //.
        //- Pinalias: GND,NC,SQUARE,TRIANGLE,MODULATION,R1,C1,VCC
        //- Package: DIP
        //- NamingConvention: Naming conventions follow National Semiconductor datasheet
        //- Limitations:
        //-    This implementation is focused on performance. There may be edge cases
        //-    which lead to issues and ringing.
        //.
        //- Example: ne566.cpp,ne566_example
        //- FunctionTable:
        //-    https://www.egr.msu.edu/eceshop/Parts_Inventory/datasheets/lm566.pdf
        //-
        //.
        //static NETLIST_START(NE566_DIP)
        public static void netlist_NE566_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nld_fourterm_global.VCVS(setup, "VI", 1);
            netlist.nld_fourterm_global.CCCS(setup, "CI1", -1);
            netlist.nld_fourterm_global.CCCS(setup, "CI2", 1);
            netlist.devices.nld_system_global.SYS_COMPD(setup, "COMP");
            netlist.devices.nld_system_global.SYS_DSW2(setup, "SW");
            netlist.nld_fourterm_global.VCVS(setup, "VO", 1);
            netlist.nld_twoterm_global.DIODE(setup, "DC", "D");
            netlist.nld_twoterm_global.DIODE(setup, "DM", "D");
            netlist.nld_twoterm_global.RES(setup, "ROD", 5200);
            netlist.nld_twoterm_global.RES(setup, "ROU", 200);

            netlist.nl_setup_global.PARAM(setup, "VO.RO", "50");
            netlist.nl_setup_global.PARAM(setup, "COMP.MODEL", "FAMILY(TYPE=CUSTOM IVL=0.16 IVH=0.4 OVL=0.1 OVH=0.1 ORL=50 ORH=50)");
            netlist.nl_setup_global.PARAM(setup, "SW.GOFF", "0"); // This has to be zero to block current sources

            netlist.nl_setup_global.NET_C(setup, "CI2.IN", "VI.OP");
            netlist.nl_setup_global.NET_C(setup, "CI2.IP", "CI1.IN");
            netlist.nl_setup_global.NET_C(setup, "COMP.Q", "SW.I");
            netlist.nl_setup_global.NET_C(setup, "SW.1", "CI1.OP");
            netlist.nl_setup_global.NET_C(setup, "SW.3", "CI2.OP");
            netlist.nl_setup_global.NET_C(setup, "SW.2", "VO.IP");
            netlist.nl_setup_global.NET_C(setup, "VO.OP", "COMP.IN");

            // Avoid singular Matrix due to G=0 switch
            netlist.nld_twoterm_global.RES(setup, "RX1", 1e10);
            netlist.nld_twoterm_global.RES(setup, "RX2", 1e10);
            netlist.nl_setup_global.NET_C(setup, "RX1.1", "SW.1");
            netlist.nl_setup_global.NET_C(setup, "RX2.1", "SW.3");

            netlist.nl_setup_global.NET_C(setup, "COMP.GND", "RX1.2", "RX2.2");

            // Block if VC < V+ - ~4
            netlist.nld_twoterm_global.VS(setup, "VM", "3");
            netlist.nl_setup_global.PARAM(setup, "VM.RI", "10");
            netlist.nl_setup_global.NET_C(setup, "VM.1", "COMP.VCC");
            netlist.nl_setup_global.NET_C(setup, "VM.2", "DM.A");
            netlist.nl_setup_global.NET_C(setup, "DM.K", "VI.OP");

            // Block if VC > V+
            netlist.nl_setup_global.NET_C(setup, "COMP.GND", "DC.A");
            netlist.nl_setup_global.NET_C(setup, "SW.2", "DC.K");

            netlist.nld_twoterm_global.RES(setup, "R1", 5000);
            netlist.nld_twoterm_global.RES(setup, "R2", 1800);
            netlist.nld_twoterm_global.RES(setup, "R3", 6000);

            // Square output wave
            netlist.devices.nld_system_global.AFUNC(setup, "FO", 2, "min(A1-1,A0 + 5)");
            netlist.nl_setup_global.NET_C(setup, "COMP.QQ", "FO.A0");
            netlist.nl_setup_global.NET_C(setup, "FO.Q", "ROU.1");
            netlist.nl_setup_global.NET_C(setup, "ROU.2", "ROD.1");

            netlist.nl_setup_global.NET_C(setup, "COMP.GND", "SW.GND", "VI.ON", "VI.IN", "CI1.ON", "CI2.ON", "VO.IN", "VO.ON", "R2.2", "ROD.2");
            netlist.nl_setup_global.NET_C(setup, "COMP.VCC", "SW.VCC", "R1.2");
            netlist.nl_setup_global.NET_C(setup, "COMP.IP", "R1.1", "R2.1", "R3.1");
            netlist.nl_setup_global.NET_C(setup, "COMP.Q", "R3.2");

            netlist.nl_setup_global.ALIAS(setup, "1", "VI.ON"); // GND
            netlist.nl_setup_global.ALIAS(setup, "3", "ROD.1"); // Square out
            netlist.nl_setup_global.ALIAS(setup, "4", "VO.OP"); // Diag out
            netlist.nl_setup_global.ALIAS(setup, "5", "VI.IP"); // VC
            netlist.nl_setup_global.ALIAS(setup, "6", "CI1.IP"); // R1
            netlist.nl_setup_global.ALIAS(setup, "7", "SW.2"); // C1
            netlist.nl_setup_global.ALIAS(setup, "8", "COMP.VCC"); // V+

            netlist.nl_setup_global.NET_C(setup, "COMP.VCC", "FO.A1");

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
                netlist.nl_setup_global.TT_FAMILY("FAMILY(TYPE=CMOS IVL=0.42 IVH=0.54 OVL=0.05 OVH=0.05 ORL=10.0 ORH=10.0)");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "MC14584B_DIP", netlist_MC14584B_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "NE566_DIP", netlist_NE566_DIP);

            netlist.nl_setup_global.NETLIST_END();
        }
    }
}
