// license:BSD-3-Clause
// copyright-holders:Edward Fast

/*
 * 0 = Basic hack (Norton with just amplification, no voltage cutting)
 * 1 = Model from LTSPICE mailing list - slow!
 * 2 = Simplified model using diode inputs and netlist TYPE=3
 * 3 = Model according to datasheet
 *
 * For Money Money 1 and 3 delivery comparable results.
 * 3 is simpler (less BJTs) and converges a lot faster.
 */
#define USE_LM3900_MODEL_3  // (3)


using System;
using System.Collections.Generic;


namespace mame
{
    public static class nlm_opamp_global
    {
        //#define UA741_DIP8(name)                                                           \
        //        NET_REGISTER_DEV(UA741_DIP8, name)
        public static void UA741_DIP8(netlist.nlparse_t setup, string name)
        {
            netlist.nl_setup_global.NET_REGISTER_DEV(setup, "UA741_DIP8", name);
        }


        /*
         *   Generic layout with 4 opamps, VCC on pin 4 and GND on pin 11
         */

        //static NETLIST_START(opamp_layout_4_4_11)
        public static void netlist_opamp_layout_4_4_11(netlist.nlparse_t setup)
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
        public static void netlist_opamp_layout_2_8_4(netlist.nlparse_t setup)
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
        public static void netlist_opamp_layout_2_13_9_4(netlist.nlparse_t setup)
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
         *   // FIXME: Offset inputs are not supported!
         */

        //static NETLIST_START(opamp_layout_1_7_4)
        public static void netlist_opamp_layout_1_7_4(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.DIPPINS(setup,             /*   +--------------+   */
                "NC" /* OFFSET */, /*   |1     ++     8|   */ "NC",
                "A.MINUS",         /*   |2            7|   */ "A.VCC",
                "A.PLUS",          /*   |3            6|   */ "A.OUT",
                "A.GND",           /*   |4            5|   */ "NC" /* OFFSET */
                                 /*   +--------------+   */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *   Generic layout with 1 opamp, VCC+ on pin 8, VCC- on pin 5 and compensation
         */

        //static NETLIST_START(opamp_layout_1_8_5)
        public static void netlist_opamp_layout_1_8_5(netlist.nlparse_t setup)
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
        public static void netlist_opamp_layout_1_11_6(netlist.nlparse_t setup)
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
        public static void netlist_MB3614_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "MB3614");
            nld_opamps_global.OPAMP(setup, "B", "MB3614");
            nld_opamps_global.OPAMP(setup, "C", "MB3614");
            nld_opamps_global.OPAMP(setup, "D", "MB3614");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_4_4_11");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(TL084_DIP)
        public static void netlist_TL084_DIP(netlist.nlparse_t setup)
        {
            nld_opamps_global.OPAMP(setup, "A", "TL084");
            nld_opamps_global.OPAMP(setup, "B", "TL084");
            nld_opamps_global.OPAMP(setup, "C", "TL084");
            nld_opamps_global.OPAMP(setup, "D", "TL084");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_4_4_11");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(LM324_DIP)
        public static void netlist_LM324_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "LM324");
            nld_opamps_global.OPAMP(setup, "B", "LM324");
            nld_opamps_global.OPAMP(setup, "C", "LM324");
            nld_opamps_global.OPAMP(setup, "D", "LM324");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_4_4_11");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(LM2902_DIP)
        public static void netlist_LM2902_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            // Same datasheet and mostly same characteristics as LM324
            nld_opamps_global.OPAMP(setup, "A", "LM324");
            nld_opamps_global.OPAMP(setup, "B", "LM324");
            nld_opamps_global.OPAMP(setup, "C", "LM324");
            nld_opamps_global.OPAMP(setup, "D", "LM324");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_4_4_11");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(LM358_DIP)
        public static void netlist_LM358_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "LM358");
            nld_opamps_global.OPAMP(setup, "B", "LM358");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_2_8_4");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(UA741_DIP8)
        public static void netlist_UA741_DIP8(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "UA741");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_1_7_4");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(UA741_DIP10)
        public static void netlist_UA741_DIP10(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "UA741");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_1_8_5");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(UA741_DIP14)
        public static void netlist_UA741_DIP14(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "UA741");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_1_11_6");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(LM747_DIP)
        public static void netlist_LM747_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "LM747");
            nld_opamps_global.OPAMP(setup, "B", "LM747");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_2_13_9_4");
            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(LM747A_DIP)
        public static void netlist_LM747A_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_opamps_global.OPAMP(setup, "A", "LM747A");
            nld_opamps_global.OPAMP(setup, "B", "LM747A");

            netlist.nl_setup_global.INCLUDE(setup, "opamp_layout_2_13_9_4");
            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC");

            netlist.nl_setup_global.NETLIST_END();
        }


#if USE_LM3900_MODEL_0  // == 0
        static NETLIST_START(LM3900)

            /*
             *  Fast norton opamp model without bandwidth
             */

            /* Terminal definitions for calling netlists */

            ALIAS(PLUS, R1.1) // Positive input
            ALIAS(MINUS, R2.1) // Negative input
            ALIAS(OUT, G1.OP) // Opamp output ...
            ALIAS(GND, G1.ON)  // V- terminal
            ALIAS(VCC, DUMMY.1)  // V+ terminal

            RES(DUMMY, RES_K(1))
            NET_C(DUMMY.2, GND)

            /* The opamp model */

            RES(R1, 1)
            RES(R2, 1)
            NET_C(R1.1, G1.IP)
            NET_C(R2.1, G1.IN)
            NET_C(R1.2, R2.2, G1.ON)
            VCVS(G1)
            PARAM(G1.G, 10000000)
            //PARAM(G1.RI, 1)
            PARAM(G1.RO, RES_K(8))

        NETLIST_END()
#endif

#if USE_LM3900_MODEL_1  // == 1
        //  LTSPICE MODEL OF LM3900 FROM NATIONAL SEMICONDUCTOR
        //  MADE BY HELMUT SENNEWALD, 8/6/2004
        //  THE LM3900 IS A SO CALLED NORTON AMPLIFIER.
        //
        //  PIN ORDER:    IN+ IN- VCC VSS OUT
        static NETLIST_START(LM3900)
            PARAM(E1.G, 0.5)
            //ALIAS(IN+, Q2.B)
            //ALIAS(IN-, Q2.C)
            //ALIAS(VCC, Q10.C)
            //ALIAS(VSS, Q2.E)

            ALIAS(PLUS, Q2.B)
            ALIAS(MINUS, Q2.C)
            ALIAS(VCC, Q10.C)
            ALIAS(GND, Q2.E)
            ALIAS(OUT, Q6.C)

            //CS(B1/*I=LIMIT(0, V(VCC,VSS)/10K, 0.2m)*/)
            CS(B1, 2e-4)
            CAP(C1, CAP_P(6.000000))
            VCVS(E1)
            QBJT_EB(Q1, "LM3900_NPN1")
            QBJT_EB(Q10, "LM3900_NPN1")
            QBJT_EB(Q11, "LM3900_NPN1")
            QBJT_EB(Q12, "LM3900_NPN1")
            QBJT_EB(Q2, "LM3900_NPN1")
            QBJT_EB(Q3, "LM3900_NPN1")
            QBJT_EB(Q4, "LM3900_PNP1")
            QBJT_EB(Q5, "LM3900_PNP1")
            QBJT_EB(Q6, "LM3900_PNP1")
            QBJT_EB(Q7, "LM3900_PNP1")
            QBJT_EB(Q8, "LM3900_NPN1")
            QBJT_EB(Q9, "LM3900_NPN1")
            RES(R1, RES_K(2.000000))
            RES(R6, RES_K(1.600000))
            NET_C(Q11.B, Q12.B, R6.2)
            NET_C(Q5.C, Q5.B, B1.P, Q4.B)
            NET_C(Q8.C, Q8.B, B1.N, R1.1, E1.IP)
            NET_C(Q9.B, R1.2)
            NET_C(R6.1, E1.OP)
            NET_C(Q10.C, Q5.E, Q4.E, Q11.C, Q12.C)
            NET_C(Q2.C, Q3.B, Q12.E)
            NET_C(Q2.E, Q3.E, Q9.E, C1.2, Q1.E, Q8.E, E1.ON, E1.IN, Q7.C)
            NET_C(Q3.C, Q6.B, C1.1, Q7.B)
            NET_C(Q6.E, Q10.B, Q4.C)
            NET_C(Q6.C, Q10.E, Q9.C, Q7.E)
            NET_C(Q2.B, Q1.C, Q1.B, Q11.E)
        NETLIST_END()
#endif

#if USE_LM3900_MODEL_2  // == 2
        static NETLIST_START(LM3900)
            OPAMP(A, "LM3900")

            DIODE(D1, "D(IS=1e-15 N=1)")
            CCCS(CS1) // Current Mirror

            ALIAS(VCC, A.VCC)
            ALIAS(GND, A.GND)
            ALIAS(PLUS, A.PLUS)
            ALIAS(MINUS, A.MINUS)
            ALIAS(OUT, A.OUT)

            NET_C(A.PLUS, CS1.IP)
            NET_C(D1.A, CS1.IN)
            NET_C(CS1.OP, A.MINUS)
            NET_C(CS1.ON, A.GND, D1.K)

        NETLIST_END()
#endif

#if USE_LM3900_MODEL_3  // == 3
        //static NETLIST_START(LM3900)
        public static void netlist_LM3900(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.ALIAS(setup, "VCC", "Q5.C");
            netlist.nl_setup_global.ALIAS(setup, "GND", "Q1.E");
            netlist.nl_setup_global.ALIAS(setup, "PLUS", "Q1.B");
            netlist.nl_setup_global.ALIAS(setup, "MINUS", "Q1.C");
            netlist.nl_setup_global.ALIAS(setup, "OUT", "Q5.E");

            netlist.nld_twoterm_global.CAP(setup, "C1", netlist.nld_twoterm_global.CAP_P(6.000000));
            netlist.nld_twoterm_global.CS(setup, "I1", 1.300000e-3);
            netlist.nld_twoterm_global.CS(setup, "I2", 200e-6);
            nld_bjt_global.QBJT_EB(setup, "Q1", "NPN");
            nld_bjt_global.QBJT_EB(setup, "Q2", "NPN");
            nld_bjt_global.QBJT_EB(setup, "Q3", "PNP");
            nld_bjt_global.QBJT_EB(setup, "Q4", "PNP");
            nld_bjt_global.QBJT_EB(setup, "Q5", "NPN");
            nld_bjt_global.QBJT_EB(setup, "Q6", "NPN");
            netlist.nl_setup_global.NET_C(setup, "Q3.E", "Q5.B", "I2.2");
            netlist.nl_setup_global.NET_C(setup, "Q3.C", "Q4.E", "Q5.E", "I1.1");
            netlist.nl_setup_global.NET_C(setup, "Q5.C", "I2.1");
            netlist.nl_setup_global.NET_C(setup, "Q1.B", "Q6.C", "Q6.B");
            netlist.nl_setup_global.NET_C(setup, "Q1.E", "Q2.E", "Q4.C", "C1.2", "I1.2", "Q6.E");
            netlist.nl_setup_global.NET_C(setup, "Q1.C", "Q2.B");
            netlist.nl_setup_global.NET_C(setup, "Q2.C", "Q3.B", "Q4.B", "C1.1");

            netlist.nl_setup_global.NETLIST_END();
        }
#endif


        //NETLIST_START(OPAMP_lib)
        public static void netlist_OPAMP_lib(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "opamp_layout_4_4_11", netlist_opamp_layout_4_4_11);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "opamp_layout_2_8_4", netlist_opamp_layout_2_8_4);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "opamp_layout_2_13_9_4", netlist_opamp_layout_2_13_9_4);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "opamp_layout_1_7_4", netlist_opamp_layout_1_7_4);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "opamp_layout_1_8_5", netlist_opamp_layout_1_8_5);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "opamp_layout_1_11_6", netlist_opamp_layout_1_11_6);

            // FIXME: JFET Opamp may need better model
            // VLL and VHH for +-6V  RI=10^12 (for numerical stability 10^9 is used below
            // RO from data sheet
            netlist.nl_setup_global.NET_MODEL(setup, "TL084       OPAMP(TYPE=3 VLH=0.75 VLL=0.75 FPF=10 UGF=3000k SLEW=13M RI=1000M RO=192 DAB=0.0014)");

            netlist.nl_setup_global.NET_MODEL(setup, "LM324       OPAMP(TYPE=3 VLH=2.0 VLL=0.2 FPF=5 UGF=500k SLEW=0.3M RI=1000k RO=50 DAB=0.00075)");
            netlist.nl_setup_global.NET_MODEL(setup, "LM358       OPAMP(TYPE=3 VLH=2.0 VLL=0.2 FPF=5 UGF=500k SLEW=0.3M RI=1000k RO=50 DAB=0.001)");
            netlist.nl_setup_global.NET_MODEL(setup, "MB3614      OPAMP(TYPE=3 VLH=1.4 VLL=0.02 FPF=3 UGF=1000k SLEW=0.6M RI=1000k RO=100 DAB=0.002)");
            netlist.nl_setup_global.NET_MODEL(setup, "UA741       OPAMP(TYPE=3 VLH=1.0 VLL=1.0 FPF=5 UGF=1000k SLEW=0.5M RI=2000k RO=75 DAB=0.0017)");
            netlist.nl_setup_global.NET_MODEL(setup, "LM747       OPAMP(TYPE=3 VLH=1.0 VLL=1.0 FPF=5 UGF=1000k SLEW=0.5M RI=2000k RO=50 DAB=0.0017)");
            netlist.nl_setup_global.NET_MODEL(setup, "LM747A      OPAMP(TYPE=3 VLH=2.0 VLL=2.0 FPF=5 UGF=1000k SLEW=0.7M RI=6000k RO=50 DAB=0.0015)");
            // TI and Motorola Datasheets differ - below are Motorola values, SLEW is average of LH and HL
            netlist.nl_setup_global.NET_MODEL(setup, "LM3900      OPAMP(TYPE=3 VLH=1.0 VLL=0.03 FPF=2k UGF=4M SLEW=10M RI=10M RO=2k DAB=0.0015)");

#if USE_LM3900_MODEL_1
            netlist.nl_setup_global.NET_MODEL(setup, "LM3900_NPN1 NPN(IS=1E-14 BF=150 TF=1E-9 CJC=1E-12 CJE=1E-12 VAF=150 RB=100 RE=5 IKF=0.002)");
            netlist.nl_setup_global.NET_MODEL(setup, "LM3900_PNP1 PNP(IS=1E-14 BF=40 TF=1E-7 CJC=1E-12 CJE=1E-12 VAF=150 RB=100 RE=5)");
#endif

            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "MB3614_DIP", netlist_MB3614_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TL084_DIP", netlist_TL084_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "LM324_DIP", netlist_LM324_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "LM358_DIP", netlist_LM358_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "LM2902_DIP", netlist_LM2902_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "UA741_DIP8", netlist_UA741_DIP8);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "UA741_DIP10", netlist_UA741_DIP10);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "UA741_DIP14", netlist_UA741_DIP14);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "LM747_DIP", netlist_LM747_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "LM747A_DIP", netlist_LM747A_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "LM3900", netlist_LM3900);

            netlist.nl_setup_global.NETLIST_END();
        }
    }
}
