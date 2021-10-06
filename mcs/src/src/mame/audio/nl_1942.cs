// license:BSD-3-Clause
// copyright-holders:Edward Fast

namespace mame
{
    partial class _1942_state : driver_device
    {
        //static NETLIST_START(1942)
        void netlist_1942(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            /* Standard stuff */

            h.SOLVER("Solver", 48000);
            h.ANALOG_INPUT("V5", 5);

            /* AY 8910 internal resistors */

            h.RES("R_AY1_1", 1000);
            h.RES("R_AY1_2", 1000);
            h.RES("R_AY1_3", 1000);
            h.RES("R_AY2_1", 1000);
            h.RES("R_AY2_2", 1000);
            h.RES("R_AY2_3", 1000);

            h.RES("R2", 220000);
            h.RES("R3", 220000);
            h.RES("R4", 220000);
            h.RES("R5", 220000);
            h.RES("R6", 220000);
            h.RES("R7", 220000);

            h.RES("R11", 10000);
            h.RES("R12", 10000);
            h.RES("R13", 10000);
            h.RES("R14", 10000);
            h.RES("R15", 10000);
            h.RES("R16", 10000);

            h.CAP("CC7", 10e-6);
            h.CAP("CC8", 10e-6);
            h.CAP("CC9", 10e-6);
            h.CAP("CC10", 10e-6);
            h.CAP("CC11", 10e-6);
            h.CAP("CC12", 10e-6);

            h.NET_C("V5", "R_AY2_3.1", "R_AY2_2.1", "R_AY2_1.1", "R_AY1_3.1", "R_AY1_2.1", "R_AY1_1.1");
            h.NET_C("GND", "R13.2", "R15.2", "R11.2", "R12.2", "R14.2", "R16.2");
            //NLFILT(R_AY2_3, R13, CC7, R2)
            h.NET_C("R_AY2_3.2", "R13.1");
            h.NET_C("R13.1", "CC7.1");
            h.NET_C("CC7.2", "R2.1");
            //NLFILT(R_AY2_2, R15, CC8, R3)
            h.NET_C("R_AY2_2.2", "R15.1");
            h.NET_C("R15.1", "CC8.1");
            h.NET_C("CC8.2", "R3.1");
            //NLFILT(R_AY2_1, R11, CC9, R4)
            h.NET_C("R_AY2_1.2", "R11.1");
            h.NET_C("R11.1", "CC9.1");
            h.NET_C("CC9.2", "R4.1");

            //NLFILT(R_AY1_3, R12, CC10, R5)
            h.NET_C("R_AY1_3.2", "R12.1");
            h.NET_C("R12.1", "CC10.1");
            h.NET_C("CC10.2", "R5.1");
            //NLFILT(R_AY1_2, R14, CC11, R6)
            h.NET_C("R_AY1_2.2", "R14.1");
            h.NET_C("R14.1", "CC11.1");
            h.NET_C("CC11.2", "R6.1");
            //NLFILT(R_AY1_1, R16, CC12, R7)
            h.NET_C("R_AY1_1.2", "R16.1");
            h.NET_C("R16.1", "CC12.1");
            h.NET_C("CC12.2", "R7.1");

            h.POT("VR", 2000);
            h.NET_C("VR.3", "GND");

            h.NET_C("R2.2", "VR.1");
            h.NET_C("R3.2", "VR.1");
            h.NET_C("R4.2", "VR.1");
            h.NET_C("R5.2", "VR.1");
            h.NET_C("R6.2", "VR.1");
            h.NET_C("R7.2", "VR.1");

            h.CAP("CC6", 10e-6);
            h.RES("R1", 100000);

            h.NET_C("CC6.1", "VR.2");
            h.NET_C("CC6.2", "R1.1");
            h.CAP("CC3", 220e-6);
            h.NET_C("R1.2", "CC3.1");
            h.NET_C("CC3.2", "GND");

            h.NETLIST_END();
        }
    }
}
