// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nlm_opamp_global
    {
        /*
         *   Generic layout with 4 opamps, VCC on pin 4 and GND on pin 11
         */

        //static NETLIST_START(opamp_layout_4_4_11)
        public static void nld_opamp_layout_4_4_11(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.DIPPINS(setup,        /*   +--------------+   */
                "A.OUT",      /*   |1     ++    14|   */ "D.OUT",
                "A.MINUS",    /*   |2           13|   */ "D.MINUS",
                "A.PLUS",     /*   |3           12|   */ "D.PLUS",
                "A.VCC",      /*   |4           11|   */ "A.GND",
                "B.PLUS",     /*   |5           10|   */ "C.PLUS",
                "B.MINUS",    /*   |6            9|   */ "C.MINUS",
                "B.OUT",      /*   |7            8|   */ "C.OUT"
                            /*   +--------------+   */
            );
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND", "D.GND");
            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC", "D.VCC");

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *   Generic layout with 2 opamps, VCC on pin 8 and GND on pin 4
         */

        //static NETLIST_START(opamp_layout_2_8_4)
        public static void nld_opamp_layout_2_8_4(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.DIPPINS(setup,        /*   +--------------+   */
                "A.OUT",      /*   |1     ++     8|   */ "A.VCC",
                "A.MINUS",    /*   |2            7|   */ "B.OUT",
                "A.PLUS",     /*   |3            6|   */ "B.MINUS",
                "A.GND",      /*   |4            5|   */ "B.PLUS"
                            /*   +--------------+   */
            );
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND");
            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC");

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *   Generic layout with 2 opamps, VCC+ on pins 9/13,  VCC- on pin 4 and compensation
         */

        //static NETLIST_START(opamp_layout_2_13_9_4)
        public static void nld_opamp_layout_2_13_9_4(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.DIPPINS(setup,        /*   +--------------+   */
                "A.MINUS",    /*   |1     ++    14|   */ "A.N2",
                "A.PLUS",     /*   |2           13|   */ "A.VCC",
                "A.N1",       /*   |3           12|   */ "A.OUT",
                "A.GND",      /*   |4           11|   */ "NC",
                "B.N1",       /*   |5           10|   */ "B.OUT",
                "B.PLUS",     /*   |6            9|   */ "B.VCC",
                "B.MINUS",    /*   |7            8|   */ "B.N2"
                            /*   +--------------+   */
            );
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND");

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *   Generic layout with 1 opamp, VCC+ on pin 7, VCC- on pin 4 and compensation
         */

        //static NETLIST_START(opamp_layout_1_7_4)
        public static void nld_opamp_layout_1_7_4(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.DIPPINS(setup,        /*   +--------------+   */
                "OFFSET.N1",  /*   |1     ++     8|   */ "NC",
                "MINUS",      /*   |2            7|   */ "VCC.PLUS",
                "PLUS",       /*   |3            6|   */ "OUT",
                "VCC.MINUS",  /*   |4            5|   */ "OFFSET.N2"
                            /*   +--------------+   */
            );
            netlist.nl_setup_global.NET_C(setup, "A.GND", "VCC.MINUS");
            netlist.nl_setup_global.NET_C(setup, "A.VCC", "VCC.PLUS");
            netlist.nl_setup_global.NET_C(setup, "A.MINUS", "MINUS");
            netlist.nl_setup_global.NET_C(setup, "A.PLUS", "PLUS");
            netlist.nl_setup_global.NET_C(setup, "A.OUT", "OUT");

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *   Generic layout with 1 opamp, VCC+ on pin 8, VCC- on pin 5 and compensation
         */

        //static NETLIST_START(opamp_layout_1_8_5)
        public static void nld_opamp_layout_1_8_5(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.DIPPINS(setup,        /*   +--------------+   */
                "NC.1",       /*   |1           10|   */ "NC.3",
                "OFFSET.N1",  /*   |2            9|   */ "NC.2",
                "MINUS",      /*   |3            8|   */ "VCC.PLUS",
                "PLUS",       /*   |4            7|   */ "OUT",
                "VCC.MINUS",  /*   |5            6|   */ "OFFSET.N2"
                            /*   +--------------+   */
            );
            netlist.nl_setup_global.NET_C(setup, "A.GND", "VCC.MINUS");
            netlist.nl_setup_global.NET_C(setup, "A.VCC", "VCC.PLUS");
            netlist.nl_setup_global.NET_C(setup, "A.MINUS", "MINUS");
            netlist.nl_setup_global.NET_C(setup, "A.PLUS", "PLUS");
            netlist.nl_setup_global.NET_C(setup, "A.OUT", "OUT");

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *   Generic layout with 1 opamp, VCC+ on pin 11, VCC- on pin 6 and compensation
         */

        //static NETLIST_START(opamp_layout_1_11_6)
        public static void nld_opamp_layout_1_11_6(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.DIPPINS(setup,        /*   +--------------+   */
                "NC.1",       /*   |1     ++    14|   */ "NC.7",
                "NC.2",       /*   |2           13|   */ "NC.6",
                "OFFSET.N1",  /*   |3           12|   */ "NC.5",
                "MINUS",      /*   |4           11|   */ "VCC.PLUS",
                "PLUS",       /*   |5           10|   */ "OUT",
                "VCC.MINUS",  /*   |6            9|   */ "OFFSET.N2",
                "NC.3",       /*   |7            8|   */ "NC.4"
                            /*   +--------------+   */
            );
            netlist.nl_setup_global.NET_C(setup, "A.GND", "VCC.MINUS");
            netlist.nl_setup_global.NET_C(setup, "A.VCC", "VCC.PLUS");
            netlist.nl_setup_global.NET_C(setup, "A.MINUS", "MINUS");
            netlist.nl_setup_global.NET_C(setup, "A.PLUS", "PLUS");
            netlist.nl_setup_global.NET_C(setup, "A.OUT", "OUT");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(MB3614_DIP)
        public static void nld_MB3614_DIP(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "MB3614");
            nld_opamps_global.OPAMP(setup, "B", "MB3614");
            nld_opamps_global.OPAMP(setup, "C", "MB3614");
            nld_opamps_global.OPAMP(setup, "D", "MB3614");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_4_4_11");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(LM324_DIP)
        public static void nld_LM324_DIP(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "LM324");
            nld_opamps_global.OPAMP(setup, "B", "LM324");
            nld_opamps_global.OPAMP(setup, "C", "LM324");
            nld_opamps_global.OPAMP(setup, "D", "LM324");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_4_4_11");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(LM358_DIP)
        public static void nld_LM358_DIP(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "LM358");
            nld_opamps_global.OPAMP(setup, "B", "LM358");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_2_8_4");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(UA741_DIP8)
        public static void nld_UA741_DIP8(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "UA741");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_1_7_4");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(UA741_DIP10)
        public static void nld_UA741_DIP10(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "UA741");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_1_8_5");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(UA741_DIP14)
        public static void nld_UA741_DIP14(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "UA741");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_1_11_6");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(LM747_DIP)
        public static void nld_LM747_DIP(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "LM747");
            nld_opamps_global.OPAMP(setup, "B", "LM747");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_2_13_9_4");
            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(LM747A_DIP)
        public static void nld_LM747A_DIP(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "LM747A");
            nld_opamps_global.OPAMP(setup, "B", "LM747A");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_2_13_9_4");
            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(LM3900)
        public static void nld_LM3900(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();


            /*
             *  Fast norton opamp model without bandwidth
             */

            /* Terminal definitions for calling netlists */

            netlist.nl_setup_global.ALIAS(setup, "PLUS", "R1.1"); // Positive input
            netlist.nl_setup_global.ALIAS(setup, "MINUS", "R2.1"); // Negative input
            netlist.nl_setup_global.ALIAS(setup, "OUT", "G1.OP"); // Opamp output ...
            netlist.nl_setup_global.ALIAS(setup, "VM", "G1.ON");  // V- terminal
            netlist.nl_setup_global.ALIAS(setup, "VP", "DUMMY.I");  // V+ terminal

            netlist.devices.nld_system_global.DUMMY_INPUT(setup, "DUMMY");

            /* The opamp model */

            netlist.nld_twoterm_global.RES(setup, "R1", 1);
            netlist.nld_twoterm_global.RES(setup, "R2", 1);
            netlist.nl_setup_global.NET_C(setup, "R1.1", "G1.IP");
            netlist.nl_setup_global.NET_C(setup, "R2.1", "G1.IN");
            netlist.nl_setup_global.NET_C(setup, "R1.2", "R2.2", "G1.ON");
            nld_fourterm_global.VCVS(setup, "G1");
            netlist.nl_setup_global.PARAM(setup, "G1.G", 10000000);
            //PARAM(G1.RI, 1)
            netlist.nl_setup_global.PARAM(setup, "G1.RO", rescap_global.RES_K(8));

            netlist.nl_setup_global.NETLIST_END();
        }


        //NETLIST_START(OPAMP_lib)
        public static void nld_OPAMP_lib(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "opamp_layout_4_4_11", nld_opamp_layout_4_4_11);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "opamp_layout_2_8_4", nld_opamp_layout_2_8_4);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "opamp_layout_2_13_9_4", nld_opamp_layout_2_13_9_4);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "opamp_layout_1_7_4", nld_opamp_layout_1_7_4);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "opamp_layout_1_8_5", nld_opamp_layout_1_8_5);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "opamp_layout_1_11_6", nld_opamp_layout_1_11_6);

            netlist.nl_setup_global.NET_MODEL(setup, "LM324       OPAMP(TYPE=3 VLH=2.0 VLL=0.2 FPF=5 UGF=500k SLEW=0.3M RI=1000k RO=50 DAB=0.00075)");
            netlist.nl_setup_global.NET_MODEL(setup, "LM358       OPAMP(TYPE=3 VLH=2.0 VLL=0.2 FPF=5 UGF=500k SLEW=0.3M RI=1000k RO=50 DAB=0.001)");
            netlist.nl_setup_global.NET_MODEL(setup, "MB3614      OPAMP(TYPE=3 VLH=1.4 VLL=0.02 FPF=2 UGF=500k SLEW=0.6M RI=1000k RO=50 DAB=0.0002)");
            netlist.nl_setup_global.NET_MODEL(setup, "UA741       OPAMP(TYPE=3 VLH=1.0 VLL=1.0 FPF=5 UGF=1000k SLEW=0.5M RI=2000k RO=75 DAB=0.0017)");
            netlist.nl_setup_global.NET_MODEL(setup, "LM747       OPAMP(TYPE=3 VLH=1.0 VLL=1.0 FPF=5 UGF=1000k SLEW=0.5M RI=2000k RO=50 DAB=0.0017)");
            netlist.nl_setup_global.NET_MODEL(setup, "LM747A      OPAMP(TYPE=3 VLH=2.0 VLL=2.0 FPF=5 UGF=1000k SLEW=0.7M RI=6000k RO=50 DAB=0.0015)");

            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "MB3614_DIP", nld_MB3614_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "LM324_DIP", nld_LM324_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "LM358_DIP", nld_LM358_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "UA741_DIP8", nld_UA741_DIP8);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "UA741_DIP10", nld_UA741_DIP10);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "UA741_DIP14", nld_UA741_DIP14);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "LM747_DIP", nld_LM747_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "LM747A_DIP", nld_LM747A_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "LM3900", nld_LM3900);

            netlist.nl_setup_global.NETLIST_END();
        }
    }
}