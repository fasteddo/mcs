// license:BSD-3-Clause
// copyright-holders:Edward Fast

/*
 * 0 = Basic hack (Norton with just amplification, no voltage cutting)
 * 1 = Model from LTSPICE mailing list - slow!
 * 2 = Simplified model using diode inputs and netlist TYPE=3
 * 3 = Model according to datasheet
 * 4 = Faster model by Colin Howell
 *
 * For Money Money 1 and 3 delivery comparable results.
 * 3 is simpler (less BJTs) and converges a lot faster.
 *
 * Model 4 uses a lot less resources and pn-junctions. The preferred new normal.
 */
#define USE_LM3900_MODEL_4  // (4)


using System;


namespace mame
{
    public static class nlm_opamp_global
    {
        /*
         *   Generic layout with 4 opamps, VCC on pin 4 and GND on pin 11
         */

        //static NETLIST_START(opamp_layout_4_4_11)
        public static void netlist_opamp_layout_4_4_11(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.DIPPINS(        /*   +--------------+   */
                "A.OUT",      /*   |1     ++    14|   */ "D.OUT",
                "A.MINUS",    /*   |2           13|   */ "D.MINUS",
                "A.PLUS",     /*   |3           12|   */ "D.PLUS",
                "A.VCC",      /*   |4           11|   */ "A.GND",
                "B.PLUS",     /*   |5           10|   */ "C.PLUS",
                "B.MINUS",    /*   |6            9|   */ "C.MINUS",
                "B.OUT",      /*   |7            8|   */ "C.OUT"
                            /*   +--------------+   */
            );
            h.NET_C("A.GND", "B.GND", "C.GND", "D.GND");
            h.NET_C("A.VCC", "B.VCC", "C.VCC", "D.VCC");

            h.NETLIST_END();
        }


        /*
         *   Generic layout with 2 opamps, VCC on pin 8 and GND on pin 4
         */

        //static NETLIST_START(opamp_layout_2_8_4)
        public static void netlist_opamp_layout_2_8_4(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.DIPPINS(        /*   +--------------+   */
                "A.OUT",      /*   |1     ++     8|   */ "A.VCC",
                "A.MINUS",    /*   |2            7|   */ "B.OUT",
                "A.PLUS",     /*   |3            6|   */ "B.MINUS",
                "A.GND",      /*   |4            5|   */ "B.PLUS"
                              /*   +--------------+   */
            );
            h.NET_C("A.GND", "B.GND");
            h.NET_C("A.VCC", "B.VCC");

            h.NETLIST_END();
        }


        /*
         *   Generic layout with 2 opamps, VCC+ on pins 9/13,  VCC- on pin 4 and compensation
         */

        //static NETLIST_START(opamp_layout_2_13_9_4)
        public static void netlist_opamp_layout_2_13_9_4(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.DIPPINS(        /*   +--------------+   */
                "A.MINUS",    /*   |1     ++    14|   */ "A.N2",
                "A.PLUS",     /*   |2           13|   */ "A.VCC",
                "A.N1",       /*   |3           12|   */ "A.OUT",
                "A.GND",      /*   |4           11|   */ "NC",
                "B.N1",       /*   |5           10|   */ "B.OUT",
                "B.PLUS",     /*   |6            9|   */ "B.VCC",
                "B.MINUS",    /*   |7            8|   */ "B.N2"
                              /*   +--------------+   */
            );
            h.NET_C("A.GND", "B.GND");

            h.NETLIST_END();
        }


        /*
         *   Generic layout with 1 opamp, VCC+ on pin 7, VCC- on pin 4 and compensation
         *   // FIXME: Offset inputs are not supported!
         */

        //static NETLIST_START(opamp_layout_1_7_4)
        public static void netlist_opamp_layout_1_7_4(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.DIPPINS(             /*   +--------------+   */
                "NC" /* OFFSET */, /*   |1     ++     8|   */ "NC",
                "A.MINUS",         /*   |2            7|   */ "A.VCC",
                "A.PLUS",          /*   |3            6|   */ "A.OUT",
                "A.GND",           /*   |4            5|   */ "NC" /* OFFSET */
                                   /*   +--------------+   */
            );

            h.NETLIST_END();
        }


        /*
         *   Generic layout with 1 opamp, VCC+ on pin 8, VCC- on pin 5 and compensation
         */

        //static NETLIST_START(opamp_layout_1_8_5)
        public static void netlist_opamp_layout_1_8_5(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.DIPPINS(        /*   +--------------+   */
                "NC.1",       /*   |1           10|   */ "NC.3",
                "OFFSET.N1",  /*   |2            9|   */ "NC.2",
                "MINUS",      /*   |3            8|   */ "VCC.PLUS",
                "PLUS",       /*   |4            7|   */ "OUT",
                "VCC.MINUS",  /*   |5            6|   */ "OFFSET.N2"
                              /*   +--------------+   */
            );
            h.NET_C("A.GND", "VCC.MINUS");
            h.NET_C("A.VCC", "VCC.PLUS");
            h.NET_C("A.MINUS", "MINUS");
            h.NET_C("A.PLUS", "PLUS");
            h.NET_C("A.OUT", "OUT");

            h.NETLIST_END();
        }


        /*
         *   Generic layout with 1 opamp, VCC+ on pin 11, VCC- on pin 6 and compensation
         */

        //static NETLIST_START(opamp_layout_1_11_6)
        public static void netlist_opamp_layout_1_11_6(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.DIPPINS(        /*   +--------------+   */
                "NC.1",       /*   |1     ++    14|   */ "NC.7",
                "NC.2",       /*   |2           13|   */ "NC.6",
                "OFFSET.N1",  /*   |3           12|   */ "NC.5",
                "MINUS",      /*   |4           11|   */ "VCC.PLUS",
                "PLUS",       /*   |5           10|   */ "OUT",
                "VCC.MINUS",  /*   |6            9|   */ "OFFSET.N2",
                "NC.3",       /*   |7            8|   */ "NC.4"
                              /*   +--------------+   */
            );
            h.NET_C("A.GND", "VCC.MINUS");
            h.NET_C("A.VCC", "VCC.PLUS");
            h.NET_C("A.MINUS", "MINUS");
            h.NET_C("A.PLUS", "PLUS");
            h.NET_C("A.OUT", "OUT");

            h.NETLIST_END();
        }


        //static NETLIST_START(MB3614_DIP)
        public static void netlist_MB3614_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            OPAMP(setup, "A", "MB3614");
            OPAMP(setup, "B", "MB3614");
            OPAMP(setup, "C", "MB3614");
            OPAMP(setup, "D", "MB3614");
#endif

            h.INCLUDE("opamp_layout_4_4_11");

            h.NETLIST_END();
        }


        //static NETLIST_START(MC3340_DIP)
        public static void netlist_MC3340_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            // A netlist description of the Motorola MC3340 Electronic Attenuator
            // IC, a voltage-controlled amplifier/attenuator. It amplifies or
            // attenuates an input signal according to the voltage of a second,
            // control signal, with a maximum gain of about 12-13 dB (about a
            // factor of 4 in voltage), and higher control voltages giving greater
            // attenuation, which scales logarithmically.

            // The netlist here is based on the circuit schematic given in
            // Motorola's own data books, especially the most recent ones
            // published in the 1990s (e.g. _Motorola Analog/Interface ICs Device
            // Data, Vol. II_ (1996), p. 9-67), which are the only schematics that
            // include resistor values. However, the 1990s schematics are missing
            // one crossover connection which is present in older schematics
            // published in the 1970s (e.g. _Motorola Linear Integrated Circuits_
            // (1979), p. 5-130). This missing connection is clearly an error
            // which has been fixed in this netlist; without it, the circuit won't
            // amplify properly, generating only a very weak output signal.

            // The 1990s schematics also omit a couple of diodes which are present
            // in the 1970s schematics. Both of these diodes have been included
            // here. One raises the minimum control voltage at which signal
            // attenuation starts, so it makes the netlist's profile of
            // attenuation vs. control voltage better match Motorola's charts for
            // the device. The other affects the level of the input "midpoint",
            // and including it makes the engine sound closer to that on real
            // 280-ZZZAP machines.

            // The Motorola schematics do not label components, so I've created my
            // own labeling scheme based on numbering components on the schematics
            // from top to bottom, left to right, with resistors also getting
            // their value (expressed European-style to avoid decimal points) as
            // part of the name. The netlist is also listed following the
            // schematics in roughly top-to-bottom, left-to-right order.

            // A very simple model is used for the transistors here, based on the
            // generic NPN default but with a larger scale current. Again, this
            // was chosen to better match the netlist's attenuation vs. control
            // voltage profile to that given in Motorola's charts for the device.

            // The MC3340 has the same circuit internally as an older Motorola
            // device, the MFC6040, which was replaced by the MC3340 in the
            // mid-1970s. The two chips differ only in packaging. Older arcade
            // games which use the MFC6040 may also benefit from this netlist
            // implementation.

            throw new emu_unimplemented();
#if false
            RES(setup, "R1_5K1", rescap_global.RES_K(5.1));

            DIODE(setup, "D1", "D(IS=1e-15 N=1)");

            RES(setup, "R2_4K7", rescap_global.RES_K(4.7));

            QBJT_EB(setup, "Q1", "NPN(IS=1E-13 BF=100)");

            RES(setup, "R3_750", rescap_global.RES_R(750));
            RES(setup, "R4_10K", rescap_global.RES_K(10));

            QBJT_EB(setup, "Q2", "NPN(IS=1E-13 BF=100)");

            RES(setup, "R5_750", rescap_global.RES_R(750));
            RES(setup, "R6_3K9", rescap_global.RES_K(3.9));

            RES(setup, "R7_5K1", rescap_global.RES_K(5.1));
            RES(setup, "R8_20K", rescap_global.RES_K(20));

            DIODE(setup, "D2", "D(IS=1e-15 N=1)");

            RES(setup, "R9_510", rescap_global.RES_R(510));

            QBJT_EB(setup, "Q3", "NPN(IS=1E-13 BF=100)");

            QBJT_EB(setup, "Q4", "NPN(IS=1E-13 BF=100)");

            QBJT_EB(setup, "Q5", "NPN(IS=1E-13 BF=100)");

            RES(setup, "R10_1K3", rescap_global.RES_K(1.3));

            QBJT_EB(setup, "Q6", "NPN(IS=1E-13 BF=100)");

            RES(setup, "R11_5K1", rescap_global.RES_K(5.1));

            QBJT_EB(setup, "Q7", "NPN(IS=1E-13 BF=100)");

            QBJT_EB(setup, "Q8", "NPN(IS=1E-13 BF=100)");

            RES(setup, "R12_1K5", rescap_global.RES_K(1.5));

            RES(setup, "R13_6K2", rescap_global.RES_K(6.2));

            QBJT_EB(setup, "Q9", "NPN(IS=1E-13 BF=100)");

            RES(setup, "R14_5K1", rescap_global.RES_K(5.1));

            QBJT_EB(setup, "Q10", "NPN(IS=1E-13 BF=100)");

            RES(setup, "R15_5K1", rescap_global.RES_K(5.1));

            RES(setup, "R16_200", rescap_global.RES_R(200));

            RES(setup, "R17_5K1", rescap_global.RES_K(5.1));

            DIODE(setup, "D3", "D(IS=1e-15 N=1)");

            RES(setup, "R18_510", rescap_global.RES_R(510));
#endif

            h.ALIAS("VCC", "R1_5K1.1");
            h.NET_C("R1_5K1.1", "Q1.C", "Q2.C", "R7_5K1.1", "Q3.C", "Q4.C", "Q7.C",
                "R13_6K2.1", "Q10.C", "R17_5K1.1");
            // Location of first diode present on 1970s schematics but omitted on
            // 1990s ones. Including it raises the control voltage threshold for
            // attenuation significantly.
            h.NET_C("R1_5K1.2", "D1.A", "Q1.B");
            h.NET_C("D1.K", "R2_4K7.1");
            h.NET_C("R2_4K7.2", "GND");

            h.NET_C("Q1.E", "R3_750.1", "R5_750.1");
            h.NET_C("R3_750.2", "R4_10K.1", "Q2.B");
            h.NET_C("R4_10K.2", "GND");

            h.NET_C("R5_750.2", "R6_3K9.1", "Q3.B");
            h.ALIAS("CONTROL", "R6_3K9.2");

            h.ALIAS("INPUT", "Q5.B");

            h.NET_C("INPUT", "R8_20K.1");
            // Location of second diode present on 1970s schematics but omitted on
            // 1990s ones. Including it is critical to making the tone of the
            // output engine sound match that of real 280-ZZZAP machines.
            h.NET_C("R7_5K1.2", "R8_20K.2", "D2.A");
            h.NET_C("D2.K", "R9_510.1");
            h.NET_C("R9_510.2", "GND");

            h.NET_C("Q4.E", "Q6.E", "Q5.C");
            h.NET_C("Q5.E", "R10_1K3.1");
            h.NET_C("R10_1K3.2", "GND");

            h.NET_C("Q6.B", "Q7.B", "Q2.E", "R11_5K1.1");
            h.NET_C("R11_5K1.2", "GND");

            h.NET_C("Q7.E", "Q9.E", "Q8.C");
            h.NET_C("Q8.E", "R12_1K5.1");
            h.NET_C("R12_1K5.2", "GND");

            h.NET_C("Q4.B", "Q9.B", "Q3.E", "R14_5K1.1");
            h.NET_C("R14_5K1.2", "GND");

            // This is where the cross-connection is erroneously omitted from
            // 1990s schematics.
            h.NET_C("Q6.C", "R13_6K2.2", "Q9.C", "Q10.B");

            // Connection for external frequency compensation capacitor; unused
            // here.
            h.ALIAS("ROLLOFF", "Q10.B");

            h.NET_C("Q10.E", "R16_200.1", "R15_5K1.1");
            h.NET_C("R15_5K1.2", "GND");
            h.ALIAS("OUTPUT", "R16_200.2");

            h.NET_C("R17_5K1.2", "D3.A", "Q8.B");
            h.NET_C("D3.K", "R18_510.1");
            h.ALIAS("GND", "R18_510.2");

            h.ALIAS("1", "INPUT");
            h.ALIAS("2", "CONTROL");
            h.ALIAS("3", "GND");
            h.ALIAS("6", "ROLLOFF");
            h.ALIAS("7", "OUTPUT");
            h.ALIAS("8", "VCC");

            h.NETLIST_END();
        }


        //static NETLIST_START(TL081_DIP)
        public static void netlist_TL081_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            OPAMP(setup, "A", "TL084");
#endif

            h.INCLUDE("opamp_layout_1_7_4");

            h.NETLIST_END();
        }


        //static NETLIST_START(TL082_DIP)
        public static void netlist_TL082_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            OPAMP(setup, "A", "TL084");
            OPAMP(setup, "B", "TL084");
#endif

            h.INCLUDE("opamp_layout_2_8_4");

            h.NETLIST_END();
        }


        //static NETLIST_START(TL084_DIP)
        public static void netlist_TL084_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            OPAMP(setup, "A", "TL084");
            OPAMP(setup, "B", "TL084");
            OPAMP(setup, "C", "TL084");
            OPAMP(setup, "D", "TL084");
#endif

            h.INCLUDE("opamp_layout_4_4_11");

            h.NETLIST_END();
        }


        //static NETLIST_START(LM324_DIP)
        public static void netlist_LM324_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            OPAMP(setup, "A", "LM324");
            OPAMP(setup, "B", "LM324");
            OPAMP(setup, "C", "LM324");
            OPAMP(setup, "D", "LM324");
#endif

            h.INCLUDE("opamp_layout_4_4_11");

            h.NETLIST_END();
        }


        //static NETLIST_START(LM2902_DIP)
        public static void netlist_LM2902_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            // Same datasheet and mostly same characteristics as LM324
            throw new emu_unimplemented();
#if false
            OPAMP(setup, "A", "LM324");
            OPAMP(setup, "B", "LM324");
            OPAMP(setup, "C", "LM324");
            OPAMP(setup, "D", "LM324");
#endif

            h.INCLUDE("opamp_layout_4_4_11");

            h.NETLIST_END();
        }


        //static NETLIST_START(LM348_DIP)
        public static void netlist_LM348_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            OPAMP(A, "UA741")
            OPAMP(B, "UA741")
            OPAMP(C, "UA741")
            OPAMP(D, "UA741")

            INCLUDE(opamp_layout_4_4_11)
#endif

            h.NETLIST_END();
        }


        //static NETLIST_START(LM358_DIP)
        public static void netlist_LM358_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            OPAMP(setup, "A", "LM358");
            OPAMP(setup, "B", "LM358");
#endif

            h.INCLUDE("opamp_layout_2_8_4");

            h.NETLIST_END();
        }


        //static NETLIST_START(UA741_DIP8)
        public static void netlist_UA741_DIP8(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.OPAMP("A", "UA741");

            h.INCLUDE("opamp_layout_1_7_4");

            h.NETLIST_END();
        }


        //static NETLIST_START(UA741_DIP10)
        public static void netlist_UA741_DIP10(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            OPAMP(setup, "A", "UA741");
#endif

            h.INCLUDE("opamp_layout_1_8_5");

            h.NETLIST_END();
        }


        //static NETLIST_START(UA741_DIP14)
        public static void netlist_UA741_DIP14(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            OPAMP(setup, "A", "UA741");
#endif

            h.INCLUDE("opamp_layout_1_11_6");

            h.NETLIST_END();
        }


        //static NETLIST_START(MC1558_DIP)
        public static void netlist_MC1558_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            OPAMP(setup, "A", "UA741");
            OPAMP(setup, "B", "UA741");
#endif

            h.INCLUDE("opamp_layout_2_8_4");

            h.NETLIST_END();
        }


        //static NETLIST_START(LM747_DIP)
        public static void netlist_LM747_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            OPAMP(setup, "A", "LM747");
            OPAMP(setup, "B", "LM747");
#endif

            h.INCLUDE("opamp_layout_2_13_9_4");
            h.NET_C("A.VCC", "B.VCC");

            h.NETLIST_END();
        }


        //static NETLIST_START(LM747A_DIP)
        public static void netlist_LM747A_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            OPAMP(setup, "A", "LM747A");
            OPAMP(setup, "B", "LM747A");
#endif

            h.INCLUDE("opamp_layout_2_13_9_4");
            h.NET_C("A.VCC", "B.VCC");

            h.NETLIST_END();
        }


        //- Identifier: AN6551_SIL
        //- Title: AN6551 Dual Operational Amplifier
        //- Pinalias: VCC,A.OUT,A-,A+,GND,B+,B-,B.OUT,VCC
        //- Package: SIL
        //- NamingConvention: Naming conventions follow Panasonic datasheet
        //- FunctionTable:
        //-   https://datasheetspdf.com/pdf-file/182163/PanasonicSemiconductor/AN6551/1
        //-
        //static NETLIST_START(AN6551_SIL)
        public static void netlist_AN6551_SIL(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            OPAMP(setup, "A", "AN6551");
            OPAMP(setup, "B", "AN6551");
#endif

            h.NET_C("A.GND", "B.GND");

            h.ALIAS("1", "A.VCC");
            h.ALIAS("2", "A.OUT");
            h.ALIAS("3", "A.MINUS");
            h.ALIAS("4", "A.PLUS");
            h.ALIAS("5", "A.GND");
            h.ALIAS("6", "B.PLUS");
            h.ALIAS("7", "B.MINUS");
            h.ALIAS("8", "B.OUT");
            h.ALIAS("9", "B.VCC");

            h.NETLIST_END();
        }


#if USE_LM3900_MODEL_0  // == 0
#endif

#if USE_LM3900_MODEL_1  // == 1
#endif

#if USE_LM3900_MODEL_2  // == 2
#endif

#if USE_LM3900_MODEL_3  // == 3
#endif

#if USE_LM3900_MODEL_4  // == 4
        //static NETLIST_START(LM3900)
        public static void netlist_LM3900(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            OPAMP(setup, "A", "OPAMP(TYPE=3 VLH=0.5 VLL=0.03 FPF=2k UGF=2.5M SLEW=1M RI=10M RO=100 DAB=0.0015)");

            DIODE(setup, "D1", "D(IS=6e-15 N=1)");
            DIODE(setup, "D2", "D(IS=6e-15 N=1)");
            CCCS(setup, "CS1", 1); // Current Mirror

            netlist.nl_setup_global.ALIAS(setup, "VCC", "A.VCC");
            netlist.nl_setup_global.ALIAS(setup, "GND", "A.GND");
            netlist.nl_setup_global.ALIAS(setup, "OUT", "A.OUT");

            netlist.nl_setup_global.ALIAS(setup, "PLUS", "CS1.IP");
            netlist.nl_setup_global.NET_C(setup, "D1.A", "CS1.IN");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "D1.K");

            CS(setup, "CS_BIAS", 10e-6);
            netlist.nl_setup_global.NET_C(setup, "A.VCC", "CS_BIAS.P");

            netlist.nl_setup_global.ALIAS(setup, "MINUS", "CS1.OP");
            netlist.nl_setup_global.NET_C(setup, "CS1.ON", "A.GND");

            CCVS(setup, "VS1", 200000); // current-to-voltage gain
            netlist.nl_setup_global.NET_C(setup, "CS1.OP", "VS1.IP");
            netlist.nl_setup_global.NET_C(setup, "VS1.IN", "CS_BIAS.N", "D2.A");
            netlist.nl_setup_global.NET_C(setup, "D2.K", "A.GND");
            netlist.nl_setup_global.NET_C(setup, "VS1.OP", "A.MINUS");
            netlist.nl_setup_global.NET_C(setup, "VS1.ON", "A.PLUS", "A.GND");
#endif

            h.NETLIST_END();
        }
#endif


        //NETLIST_START(opamp_lib)
        public static void netlist_opamp_lib(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.LOCAL_LIB_ENTRY("opamp_layout_4_4_11", netlist_opamp_layout_4_4_11);
            h.LOCAL_LIB_ENTRY("opamp_layout_2_8_4", netlist_opamp_layout_2_8_4);
            h.LOCAL_LIB_ENTRY("opamp_layout_2_13_9_4", netlist_opamp_layout_2_13_9_4);
            h.LOCAL_LIB_ENTRY("opamp_layout_1_7_4", netlist_opamp_layout_1_7_4);
            h.LOCAL_LIB_ENTRY("opamp_layout_1_8_5", netlist_opamp_layout_1_8_5);
            h.LOCAL_LIB_ENTRY("opamp_layout_1_11_6", netlist_opamp_layout_1_11_6);

            // FIXME: JFET Opamp may need better model
            // VLL and VHH for +-6V  RI=10^12 (for numerical stability 10^9 is used below
            // RO from data sheet
            h.NET_MODEL("TL084       OPAMP(TYPE=3 VLH=0.75 VLL=0.75 FPF=10 UGF=3000k SLEW=13M RI=1000M RO=192 DAB=0.0014)");

            h.NET_MODEL("LM324       OPAMP(TYPE=3 VLH=2.0 VLL=0.2 FPF=5 UGF=500k SLEW=0.3M RI=1000k RO=50 DAB=0.00075)");
            h.NET_MODEL("LM358       OPAMP(TYPE=3 VLH=2.0 VLL=0.2 FPF=5 UGF=500k SLEW=0.3M RI=1000k RO=50 DAB=0.001)");
            h.NET_MODEL("MB3614      OPAMP(TYPE=3 VLH=1.4 VLL=0.02 FPF=3 UGF=1000k SLEW=0.6M RI=1000k RO=100 DAB=0.002)");
            h.NET_MODEL("UA741       OPAMP(TYPE=3 VLH=1.0 VLL=1.0 FPF=5 UGF=1000k SLEW=0.5M RI=2000k RO=75 DAB=0.0017)");
            h.NET_MODEL("LM747       OPAMP(TYPE=3 VLH=1.0 VLL=1.0 FPF=5 UGF=1000k SLEW=0.5M RI=2000k RO=50 DAB=0.0017)");
            h.NET_MODEL("LM747A      OPAMP(TYPE=3 VLH=2.0 VLL=2.0 FPF=5 UGF=1000k SLEW=0.7M RI=6000k RO=50 DAB=0.0015)");
            h.NET_MODEL("LM748       OPAMP(TYPE=3 VLH=2.0 VLL=2.0 FPF=5 UGF=800k SLEW=0.7M RI=800k RO=60 DAB=0.001)");
            // TI and Motorola datasheets differ - below are Motorola values, SLEW is average of LH and HL
            h.NET_MODEL("LM3900      OPAMP(TYPE=3 VLH=1.0 VLL=0.03 FPF=2k UGF=4M SLEW=10M RI=10M RO=2k DAB=0.0015)");

            h.NET_MODEL("AN6551      OPAMP(TYPE=3 VLH=1.0 VLL=0.03 FPF=20 UGF=2M SLEW=1M RI=10M RO=200 DAB=0.0015)");

#if USE_LM3900_MODEL_1
            NET_MODEL("LM3900_NPN1 NPN(IS=1E-14 BF=150 TF=1E-9 CJC=1E-12 CJE=1E-12 VAF=150 RB=100 RE=5 IKF=0.002)")
            NET_MODEL("LM3900_PNP1 PNP(IS=1E-14 BF=40 TF=1E-7 CJC=1E-12 CJE=1E-12 VAF=150 RB=100 RE=5)")
#endif
            h.LOCAL_LIB_ENTRY("MB3614_DIP", netlist_MB3614_DIP);
            h.LOCAL_LIB_ENTRY("MC3340_DIP", netlist_MC3340_DIP);
            h.LOCAL_LIB_ENTRY("TL081_DIP", netlist_TL081_DIP);
            h.LOCAL_LIB_ENTRY("TL082_DIP", netlist_TL082_DIP);
            h.LOCAL_LIB_ENTRY("TL084_DIP", netlist_TL084_DIP);
            h.LOCAL_LIB_ENTRY("LM324_DIP", netlist_LM324_DIP);
            h.LOCAL_LIB_ENTRY("LM348_DIP", netlist_LM348_DIP);
            h.LOCAL_LIB_ENTRY("LM358_DIP", netlist_LM358_DIP);
            h.LOCAL_LIB_ENTRY("LM2902_DIP", netlist_LM2902_DIP);
            h.LOCAL_LIB_ENTRY("UA741_DIP8", netlist_UA741_DIP8);
            h.LOCAL_LIB_ENTRY("UA741_DIP10", netlist_UA741_DIP10);
            h.LOCAL_LIB_ENTRY("UA741_DIP14", netlist_UA741_DIP14);
            h.LOCAL_LIB_ENTRY("MC1558_DIP", netlist_MC1558_DIP);
            h.LOCAL_LIB_ENTRY("LM747_DIP", netlist_LM747_DIP);
            h.LOCAL_LIB_ENTRY("LM747A_DIP", netlist_LM747A_DIP);
            h.LOCAL_LIB_ENTRY("LM3900", netlist_LM3900);
            h.LOCAL_LIB_ENTRY("AN6551_SIL", netlist_AN6551_SIL);

            h.NETLIST_END();
        }
    }
}
