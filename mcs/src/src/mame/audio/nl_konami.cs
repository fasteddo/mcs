// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.rescap_global;


namespace mame
{
    partial class galaxian_state : driver_device
    {
        //static NETLIST_START(filter)
        void netlist_filter(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.CD4066_GATE("G1");
            h.PARAM("G1.BASER", 270.0);
            h.CD4066_GATE("G2");
            h.PARAM("G2.BASER", 270.0);
            h.RES("RI", RES_K(1));
            h.RES("RO", RES_K(5));
            h.CAP("C1", CAP_U(0.22));
            h.CAP("C2", CAP_U(0.047));
            h.NET_C("RI.2", "RO.1", "G1.R.1", "G2.R.1");
            h.NET_C("G1.R.2", "C1.1");
            h.NET_C("G2.R.2", "C2.1");

            h.NET_C("C1.2", "C2.2", "G1.VSS", "G2.VSS");
            h.NET_C("G1.VDD", "G2.VDD");

            h.ALIAS("I", "RI.1");
            h.ALIAS("O", "RO.2");

            h.ALIAS("CTL1", "G1.CTL");
            h.ALIAS("CTL2", "G2.CTL");

            h.ALIAS("VDD", "G1.VDD");
            h.ALIAS("VSS", "G1.VSS");

            h.NETLIST_END();
        }


        //static NETLIST_START(amp)
        void netlist_amp(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.UA741_DIP8("X3A");
            h.RES("R1", RES_K(2.2));
            h.RES("R2", RES_K(4.7));
            h.RES("VR", 200);         // Actually a potentiometer
            h.CAP("C1", CAP_U(0.15));
            h.RES("RI", RES_K(100));

            h.NET_C("X3A.2", "R1.1");
            h.NET_C("X3A.6", "R1.2", "R2.1");
            h.NET_C("R2.2", "VR.1");
            h.NET_C("VR.1", "C1.1");    // 100% pot position
            h.NET_C("C1.2", "RI.1");

            h.NET_C("GND", "VR.2", "RI.2");

            // Amplifier M51516L, assume input RI 100k

            h.ALIAS("OPAMP", "X3A.2");
            h.ALIAS("OUT", "RI.1");
            h.ALIAS("VP", "X3A.7");
            h.ALIAS("VM", "X3A.4");
            h.ALIAS("GND", "X3A.3");

            h.NETLIST_END();
        }


        //static NETLIST_START(AY1)
        void netlist_AY1(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.TTL_INPUT("CTL0", 0);
            h.TTL_INPUT("CTL1", 0);
            h.TTL_INPUT("CTL2", 0);
            h.TTL_INPUT("CTL3", 0);
            h.TTL_INPUT("CTL4", 0);
            h.TTL_INPUT("CTL5", 0);
            /* AY 8910 internal resistors */
            h.RES("R_AY3D_A", 1000);
            h.RES("R_AY3D_B", 1000);
            h.RES("R_AY3D_C", 1000);
            h.NET_C("VP5", "R_AY3D_A.1", "R_AY3D_B.1", "R_AY3D_C.1");

            h.SUBMODEL("filter", "FCHA1");
            h.NET_C("FCHA1.I", "R_AY3D_A.2");
            h.SUBMODEL("filter", "FCHB1");
            h.NET_C("FCHB1.I", "R_AY3D_B.2");
            h.SUBMODEL("filter", "FCHC1");
            h.NET_C("FCHC1.I", "R_AY3D_C.2");

            h.NET_C("FCHA1.CTL1", "CTL0");
            h.NET_C("FCHA1.CTL2", "CTL1");
            h.NET_C("FCHB1.CTL1", "CTL2");
            h.NET_C("FCHB1.CTL2", "CTL3");
            h.NET_C("FCHC1.CTL1", "CTL4");
            h.NET_C("FCHC1.CTL2", "CTL5");

            h.NET_C("VP5", "FCHA1.VDD", "FCHB1.VDD", "FCHC1.VDD");
            h.NET_C("GND", "FCHA1.VSS", "FCHB1.VSS", "FCHC1.VSS");

            h.NET_C("VP5", "CTL0.VCC", "CTL1.VCC", "CTL2.VCC", "CTL3.VCC", "CTL4.VCC", "CTL5.VCC");
            h.NET_C("GND", "CTL0.GND", "CTL1.GND", "CTL2.GND", "CTL3.GND", "CTL4.GND", "CTL5.GND");

            h.NETLIST_END();
        }


        //static NETLIST_START(AY2)
        void netlist_AY2(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            TTL_INPUT("CTL6", 0);
            TTL_INPUT("CTL7", 0);
            TTL_INPUT("CTL8", 0);
            TTL_INPUT("CTL9", 0);
            TTL_INPUT("CTL10", 0);
            TTL_INPUT("CTL11", 0);
            /* AY 8910 internal resistors */
            RES("R_AY3C_A", 1000);
            RES("R_AY3C_B", 1000);
            RES("R_AY3C_C", 1000);
            NET_C("VP5", "R_AY3C_A.1", "R_AY3C_B.1", "R_AY3C_C.1");
#endif

            h.SUBMODEL("filter", "FCHA2");
            h.NET_C("FCHA2.I", "R_AY3C_A.2");
            h.SUBMODEL("filter", "FCHB2");
            h.NET_C("FCHB2.I", "R_AY3C_B.2");
            h.SUBMODEL("filter", "FCHC2");
            h.NET_C("FCHC2.I", "R_AY3C_C.2");

            h.NET_C("FCHA2.CTL1", "CTL6");
            h.NET_C("FCHA2.CTL2", "CTL7");
            h.NET_C("FCHB2.CTL1", "CTL8");
            h.NET_C("FCHB2.CTL2", "CTL9");
            h.NET_C("FCHC2.CTL1", "CTL10");
            h.NET_C("FCHC2.CTL2", "CTL11");

            h.NET_C("VP5", "FCHA2.VDD", "FCHB2.VDD", "FCHC2.VDD");
            h.NET_C("GND", "FCHA2.VSS", "FCHB2.VSS", "FCHC2.VSS");

            h.NET_C("VP5", "CTL6.VCC", "CTL7.VCC", "CTL8.VCC", "CTL9.VCC", "CTL10.VCC", "CTL11.VCC");
            h.NET_C("GND", "CTL6.GND", "CTL7.GND", "CTL8.GND", "CTL9.GND", "CTL10.GND", "CTL11.GND");

            h.NETLIST_END();
        }


        //NETLIST_START(konami2x)
        void netlist_konami2x(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.SOLVER("Solver", 48000);

            throw new emu_unimplemented();
#if false
            ANALOG_INPUT("VP5", 5);
            ANALOG_INPUT("VM5", -5);
#endif

            h.LOCAL_SOURCE("filter", netlist_filter);
            h.LOCAL_SOURCE("amp", netlist_amp);
            h.LOCAL_SOURCE("AY1", netlist_AY1);
            h.LOCAL_SOURCE("AY2", netlist_AY2);

            h.INCLUDE("AY1");
            h.INCLUDE("AY2");

            h.NET_C("FCHA1.O", "FCHB1.O", "FCHC1.O", "FCHA2.O", "FCHB2.O", "FCHC2.O");

            h.SUBMODEL("amp", "AMP");

            h.NET_C("VP5", "AMP.VP");
            h.NET_C("GND", "AMP.GND");
            h.NET_C("VM5", "AMP.VM");
            h.NET_C("FCHA1.O", "AMP.OPAMP");

            h.ALIAS("OUT", "AMP.OUT");

            h.NETLIST_END();
        }


        //NETLIST_START(konami1x)
        void netlist_konami1x(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.SOLVER("Solver", 48000);

            h.ANALOG_INPUT("VP5", 5);
            h.ANALOG_INPUT("VM5", -5);

            h.LOCAL_SOURCE("filter", netlist_filter);
            h.LOCAL_SOURCE("amp", netlist_amp);
            h.LOCAL_SOURCE("AY1", netlist_AY1);
            h.LOCAL_SOURCE("AY2", netlist_AY2);

            h.INCLUDE("AY1");

            h.NET_C("FCHA1.O", "FCHB1.O", "FCHC1.O");

            h.SUBMODEL("amp", "AMP");

            h.NET_C("VP5", "AMP.VP");
            h.NET_C("GND", "AMP.GND");
            h.NET_C("VM5", "AMP.VM");
            h.NET_C("FCHA1.O", "AMP.OPAMP");

            h.ALIAS("OUT", "AMP.OUT");

            h.NETLIST_END();
        }
    }
}
