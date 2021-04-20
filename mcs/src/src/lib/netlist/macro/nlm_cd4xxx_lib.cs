// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nlm_cd4xxx_global
    {
        /*
         *   CD4001BC: Quad 2-Input NOR Buffered B Series Gate
         *
         *       +--------------+
         *    A1 |1     ++    14| VDD
         *    B1 |2           13| A6
         *    A2 |3           12| Y6
         *    Y2 |4    4001   11| A5
         *    A3 |5           10| Y5
         *    Y3 |6            9| A4
         *   VSS |7            8| Y4
         *       +--------------+
         *
         */

        //static NETLIST_START(CD4001_DIP)
        public static void netlist_CD4001_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4001_GATE(setup, "A");
            CD4001_GATE(setup, "B");
            CD4001_GATE(setup, "C");
            CD4001_GATE(setup, "D");
#endif

            netlist.nl_setup_global.NET_C(setup, "A.VDD", "B.VDD", "C.VDD", "D.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.VSS", "B.VSS", "C.VSS", "D.VSS");

            netlist.nl_setup_global.DIPPINS(setup,   /*     +--------------+     */
                  "A.A", /*  A1 |1     ++    14| VDD */ "A.VDD",
                  "A.B", /*  B1 |2           13| B4  */ "D.B",
                  "A.Q", /*  Y1 |3           12| A4  */ "D.A",
                  "B.Q", /*  Y2 |4    4001   11| Y4  */ "D.Q",
                  "B.A", /*  A2 |5           10| Y3  */ "C.Q",
                  "B.B", /*  B2 |6            9| B3  */ "C.B",
                "A.VSS", /* VSS |7            8| A3  */ "C.A"
                       /*     +--------------+     */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier: CD4006_DIP
        //- Title: CD4006BM/CD4006BC 18-Stage Static Shift Register
        //- Pinalias: D1,NC,CLOCK,D2,D3,D4,VSS,D4P4,D4P5,D3P4,D2P4,D2P5,D1P4,VDD
        //- Package: DIP
        //- NamingConvention: Naming conventions follow National Semiconductor datasheet
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheet/nationalsemiconductor/DS005942.PDF
        //-
        //static NETLIST_START(CD4006_DIP)
        public static void netlist_CD4006_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4006(setup, "A");
            NC_PIN(setup, "NC");
#endif

            netlist.nl_setup_global.DIPPINS(setup,      /*       +--------------+      */
                   "A.D1", /*    D1 |1     ++    14| VDD  */ "A.VDD",
                   "NC.I", /*    NC |2           13| D1+4 */ "A.D1P4",
                "A.CLOCK", /* CLOCK |3           12| D2+5 */ "A.D2P5",
                   "A.D2", /*    D2 |4    4006   11| D2+4 */ "A.D2P4",
                   "A.D3", /*    D3 |5           10| D3+4 */ "A.D3P4",
                   "A.D4", /*    D4 |6            9| D4+5 */ "A.D4P5",
                  "A.VSS", /*   VSS |7            8| D4+4 */ "A.D4P4"
                         /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier: CD4011_DIP
        //- Title: CD4011BM/CD4011BC Quad 2-Input NAND Buffered B Series Gate
        //- Pinalias: A1,B1,Y1,Y2,A2,B2,VSS,A3,B3,Y3,Y4,A4,B4,VDD
        //- Package: DIP
        //- NamingConvention: Naming conventions follow National Semiconductor datasheet
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheets/166/108518_DS.pdf
        //-
        //-     +---+---++---+
        //-     | A | B || Y |
        //-     +===+===++===+
        //-     | X | 0 || 1 |
        //-     | 0 | X || 1 |
        //-     | 1 | 1 || 0 |
        //-     +---+---++---+
        //-
        //static NETLIST_START(CD4011_DIP)
        public static void netlist_CD4011_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4011_GATE(setup, "A");
            CD4011_GATE(setup, "B");
            CD4011_GATE(setup, "C");
            CD4011_GATE(setup, "D");
#endif

            netlist.nl_setup_global.NET_C(setup, "A.VDD", "B.VDD", "C.VDD", "D.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.VSS", "B.VSS", "C.VSS", "D.VSS");

            netlist.nl_setup_global.DIPPINS(setup,    /*     +--------------+     */
                    "A.A", /*   A |1     ++    14| VDD */ "A.VDD",
                    "A.B", /*   B |2           13| H   */ "D.B",
                    "A.Q", /*   J |3           12| G   */ "D.A",
                    "B.Q", /*   K |4    4011   11| M   */ "D.Q",
                    "B.A", /*   C |5           10| L   */ "C.Q",
                    "B.B", /*   D |6            9| F   */ "C.B",
                  "A.VSS", /* VSS |7            8| E   */ "C.A"
                        /*     +--------------+     */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier: CD4013_DIP
        //- Title: CD4013BM/CD4013BC Dual D Flip-Flop
        //- Pinalias: Q1,QQ1,CLOCK1,RESET1,DATA1,SET1,VSS,SET2,DATA2,RESET2,CLOCK2,QQ2,Q2,VDD
        //- Package: DIP
        //- NamingConvention: Naming conventions follow National Semiconductor datasheet
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheets/150/108670_DS.pdf
        //-
        //-     +-----+---+---+---++---+----+
        //-     | CLK | D | R | S || Q | QQ |
        //-     +=====+===+===+===++===+====+
        //-     | 0-1 | 0 | 0 | 0 || 0 |  1 |
        //-     | 0-1 | 1 | 0 | 0 || 1 |  0 |
        //-     | 1-0 | X | 0 | 0 || Q | QQ |
        //-     |  X  | X | 1 | 0 || 0 |  1 |
        //-     |  X  | X | 0 | 1 || 1 |  0 |
        //-     |  X  | X | 1 | 1 || 1 |  1 |
        //-     +-----+---+---+---++---+----+
        //-
        //static NETLIST_START(CD4013_DIP)
        public static void netlist_CD4013_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4013(setup, "A");
            CD4013(setup, "B");
#endif

            netlist.nl_setup_global.NET_C(setup, "A.VDD", "B.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.VSS", "B.VSS");

            netlist.nl_setup_global.DIPPINS(setup,     /*         +--------------+        */
                    "A.Q", /*      Q1 |1     ++    14| VDD    */ "A.VDD",
                   "A.QQ", /*     Q1Q |2           13| Q2     */ "B.Q",
                "A.CLOCK", /*  CLOCK1 |3           12| Q2Q    */ "B.QQ",
                "A.RESET", /*  RESET1 |4    4013   11| CLOCK2 */ "B.CLOCK",
                 "A.DATA", /*   DATA1 |5           10| RESET2 */ "B.RESET",
                  "A.SET", /*    SET1 |6            9| DATA2  */ "B.DATA",
                  "A.VSS", /*     VSS |7            8| SET2   */ "B.SET"
                         /*         +--------------+        */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier: CD4016_DIP
        //- Title: CD4016BM/CD4016BC Quad Bilateral Switch
        //- Pinalias: INOUTA,OUTINA,OUTINB,INOUTB,CONTROLB,CONTROLC,VSS,INOUTC,OUTINC,OUTIND,INOUTD,CONTROLD,CONTROLA,VDD
        //- Package: DIP
        //- NamingConvention: Naming conventions follow National Semiconductor datasheet
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheets/185/108711_DS.pdf
        //-
        //static NETLIST_START(CD4016_DIP)
        public static void netlist_CD4016_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4066_GATE(setup, "A");
            CD4066_GATE(setup, "B");
            CD4066_GATE(setup, "C");
            CD4066_GATE(setup, "D");
#endif

            netlist.nl_setup_global.NET_C(setup, "A.VDD", "B.VDD", "C.VDD", "D.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.VSS", "B.VSS", "C.VSS", "D.VSS");

            netlist.nl_setup_global.PARAM(setup, "A.BASER", 1000.0);
            netlist.nl_setup_global.PARAM(setup, "B.BASER", 1000.0);
            netlist.nl_setup_global.PARAM(setup, "C.BASER", 1000.0);
            netlist.nl_setup_global.PARAM(setup, "D.BASER", 1000.0);

            netlist.nl_setup_global.DIPPINS(setup,        /*          +--------------+          */
                "A.R.1",      /*   INOUTA |1     ++    14| VDD      */ "A.VDD",
                "A.R.2",      /*   OUTINA |2           13| CONTROLA */ "A.CTL",
                "B.R.1",      /*   OUTINB |3           12| CONTROLD */ "D.CTL",
                "B.R.2",      /*   INOUTB |4    4016   11| INOUTD   */ "D.R.1",
                "B.CTL",      /* CONTROLB |5           10| OUTIND   */ "D.R.2",
                "C.CTL",      /* CONTROLC |6            9| OUTINC   */ "C.R.1",
                "A.VSS",      /*      VSS |7            8| INOUTC   */ "C.R.2"
                            /*          +--------------+          */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier: CD4017_DIP
        //- Title: CD4017BM/CD4017BC Decade Counter/Divider with 10 Decoded Outputs
        //- Pinalias: Q5,Q1,Q0,Q2,Q6,Q7,Q3,VSS,Q8,Q4,Q9,CARRY_OUT,CLOCK_ENABLE,CLOCK,RESET,VDD
        //- Package: DIP
        //- NamingConvention: Naming conventions follow National Semiconductor datasheet
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheets/166/108736_DS.pdf
        //-
        //static NETLIST_START(CD4017_DIP)
        public static void netlist_CD4017_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4017(setup, "A");
#endif

            netlist.nl_setup_global.DIPPINS(setup,    /*     +--------------+              */
                 "A.Q5", /*  Q5 |1     ++    16| VDD          */ "A.VDD",
                 "A.Q1", /*  Q1 |2           15| RESET        */ "A.RESET",
                 "A.Q0", /*  Q0 |3           14| CLOCK        */ "A.CLK",
                 "A.Q2", /*  Q2 |4    4017   13| CLOCK ENABLE */ "A.CLKEN",
                 "A.Q6", /*  Q6 |5           12| CARRY OUT    */ "A.CO",
                 "A.Q7", /*  Q7 |6           11| Q9           */ "A.Q9",
                 "A.Q3", /*  Q3 |7           10| Q4           */ "A.Q4",
                "A.VSS", /* VSS |8            9| Q8           */ "A.Q8"
                       /*     +--------------+              */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier: CD4020_DIP
        //- Title: CD4020BC 14-Stage Ripple Carry Binary Counters
        //- Pinalias: Q12,Q13,Q14,Q6,Q5,Q7,Q4,VSS,Q1,PHI1,RESET,Q9,Q8,Q10,Q11,VDD
        //- Package: DIP
        //- NamingConvention: Naming conventions follow Fairchild Semiconductor datasheet
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheets/90/109006_DS.pdf
        //-
        //static NETLIST_START(CD4020_DIP)
        public static void netlist_CD4020_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4020(setup, "A");
#endif

            netlist.nl_setup_global.DIPPINS(setup,   /*     +--------------+       */
                "A.Q12", /* Q12 |1     ++    16| VDD   */ "A.VDD",
                "A.Q13", /* Q13 |2           15| Q11   */ "A.Q11",
                "A.Q14", /* Q14 |3           14| Q10   */ "A.Q10",
                 "A.Q6", /*  Q6 |4    4020   13| Q8    */ "A.Q8",
                 "A.Q5", /*  Q5 |5           12| Q9    */ "A.Q9",
                 "A.Q7", /*  Q7 |6           11| RESET */ "A.RESET",
                 "A.Q4", /*  Q4 |7           10| PHI1  */ "A.IP",
                "A.VSS", /* VSS |8            9| Q1    */ "A.Q1"
                       /*     +--------------+       */);

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier: CD4022_DIP
        //- Title: CD4022BM/CD4022BC Divide-by-8 Counter/Divider with 8 Decoded Outputs
        //- Pinalias: Q1,Q0,Q2,Q5,Q6,NC,Q3,VSS,NC,Q7,Q4,CARRY_OUT,CLOCK_ENABLE,CLOCK,RESET,VDD
        //- Package: DIP
        //- NamingConvention: Naming conventions follow National Semiconductor datasheet
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheets/166/108736_DS.pdf
        //-
        //static NETLIST_START(CD4022_DIP)
        public static void netlist_CD4022_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4022(A);
            NC_PIN(setup, "NC");
#endif

            netlist.nl_setup_global.DIPPINS(setup,   /*     +--------------+              */
                 "A.Q1", /*  Q1 |1     ++    16| VDD          */ "A.VDD",
                 "A.Q0", /*  Q0 |2           15| RESET        */ "A.RESET",
                 "A.Q2", /*  Q2 |3           14| CLOCK        */ "A.CLK",
                 "A.Q5", /*  Q5 |4    4022   13| CLOCK ENABLE */ "A.CLKEN",
                 "A.Q6", /*  Q6 |5           12| CARRY OUT    */ "A.CO",
                 "NC.I", /*  NC |6           11| Q4           */ "A.Q4",
                 "A.Q3", /*  Q3 |7           10| Q7           */ "A.Q7",
                "A.VSS", /* VSS |8            9| NC           */ "NC.I"
                       /*     +--------------+              */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier: CD4024_DIP
        //- Title: CD4024BM/CD4024BC 7-Stage Ripple Carry Binary Counter
        //- Pinalias: IP,RESET,Q7,Q6,Q5,Q4,VSS,NC,Q3,NC,Q2,Q1,NC,VDD
        //- Package: DIP
        //- NamingConvention: Naming conventions follow National Semiconductor datasheet
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheets/120/108894_DS.pdf
        //-
        //static NETLIST_START(CD4024_DIP)
        public static void netlist_CD4024_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4024(A);
            NC_PIN(setup, "NC");
#endif

            netlist.nl_setup_global.DIPPINS(setup,     /*       +--------------+     */
                   "A.IP", /*    IP |1     ++    14| VDD */ "A.VDD",
                "A.RESET", /* RESET |2           13| NC  */ "NC.I",
                   "A.Q7", /*    Q7 |3           12| Q1  */ "A.Q1",
                   "A.Q6", /*    Q6 |4    4024   11| Q2  */ "A.Q2",
                   "A.Q5", /*    Q5 |5           10| NC  */ "NC.I",
                   "A.Q4", /*    Q4 |6            9| Q3  */ "A.Q3",
                  "A.VSS", /*   VSS |7            8| NC  */ "NC.I"
                         /*       +--------------+     */);

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier: CD4066_DIP
        //- Title: CD4066BM/CD4066BC Quad Bilateral Switch
        //- Pinalias: INOUTA,OUTINA,OUTINB,INOUTB,CONTROLB,CONTROLC,VSS,INOUTC,OUTINC,OUTIND,INOUTD,CONTROLD,CONTROLA,VDD
        //- Package: DIP
        //- NamingConvention: Naming conventions follow National Semiconductor datasheet
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheet/nationalsemiconductor/DS005665.PDF
        //-
        //static NETLIST_START(CD4066_DIP)
        public static void netlist_CD4066_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4066_GATE(setup, "A");
            CD4066_GATE(setup, "B");
            CD4066_GATE(setup, "C");
            CD4066_GATE(setup, "D");
#endif

            netlist.nl_setup_global.NET_C(setup, "A.VDD", "B.VDD", "C.VDD", "D.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.VSS", "B.VSS", "C.VSS", "D.VSS");

            netlist.nl_setup_global.PARAM(setup, "A.BASER", 270.0);
            netlist.nl_setup_global.PARAM(setup, "B.BASER", 270.0);
            netlist.nl_setup_global.PARAM(setup, "C.BASER", 270.0);
            netlist.nl_setup_global.PARAM(setup, "D.BASER", 270.0);

            netlist.nl_setup_global.DIPPINS(setup,   /*          +--------------+          */
                "A.R.1", /*   INOUTA |1     ++    14| VDD      */ "A.VDD",
                "A.R.2", /*   OUTINA |2           13| CONTROLA */ "A.CTL",
                "B.R.1", /*   OUTINB |3           12| CONTROLD */ "D.CTL",
                "B.R.2", /*   INOUTB |4    4066   11| INOUTD   */ "D.R.1",
                "B.CTL", /* CONTROLB |5           10| OUTIND   */ "D.R.2",
                "C.CTL", /* CONTROLC |6            9| OUTINC   */ "C.R.1",
                "A.VSS", /*      VSS |7            8| INOUTC   */ "C.R.2"
                       /*          +--------------+          */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier: CD4053_DIP
        //- Title: CD4053BM/CD4053BC Triple 2-Channel AnalogMultiplexer/Demultiplexer
        //- Pinalias: INOUTBY,INOUTBX,INOUTCY,OUTINC,INOUTCX,INH,VEE,VSS,C,B,A,INOUTAX,INOUTAY,OUTINA,OUTINB,VDD
        //- Package: DIP
        //- NamingConvention: Naming conventions follow National Semiconductor datasheet
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheet/nationalsemiconductor/DS005662.PDF
        //-
        //static NETLIST_START(CD4053_DIP)
        public static void netlist_CD4053_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4053_GATE(A);
            CD4053_GATE(B);
            CD4053_GATE(C);
#endif

            netlist.nl_setup_global.NET_C(setup, "A.VEE", "B.VEE", "C.VEE");
            netlist.nl_setup_global.NET_C(setup, "A.VDD", "B.VDD", "C.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.VSS", "B.VSS", "C.VSS");
            netlist.nl_setup_global.NET_C(setup, "A.INH", "B.INH", "C.INH");

            netlist.nl_setup_global.PARAM(setup, "A.BASER", 270.0);
            netlist.nl_setup_global.PARAM(setup, "B.BASER", 270.0);
            netlist.nl_setup_global.PARAM(setup, "C.BASER", 270.0);

            netlist.nl_setup_global.DIPPINS(setup,   /*         +--------------+         */
                  "B.Y", /* INOUTBY |1     ++    16| VDD     */ "A.VDD",
                  "B.X", /* INOUTBX |2           15| OUTINB  */ "B.XY",
                  "C.Y", /* INOUTCY |3           14| OUTINA  */ "A.XY",
                 "C.XY", /*  OUTINC |4    4053   13| INOUTAY */ "A.Y",
                  "C.X", /* INOUTCX |5           12| INOUTAX */ "A.X",
                "A.INH", /*     INH |6           11| A       */ "A.S",
                "A.VEE", /*     VEE |7           10| B       */ "B.S",
                "A.VSS", /*     VSS |8            9| C       */ "C.S"
                       /*         +--------------+         */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier: CD4069_DIP
        //- Title: CD4069UBM/CD4069UBC Inverter Circuits
        //- Pinalias: A1,Y1,A2,Y2,A3,Y3,VSS,Y4,A4,Y5,A5,Y6,A6,VDD
        //- Package: DIP
        //- NamingConvention: Naming conventions follow National Semiconductor datasheet
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheets/150/206783_DS.pdf
        //-
        //static NETLIST_START(CD4069_DIP)
        public static void netlist_CD4069_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4069_GATE(setup, "A");
            CD4069_GATE(setup, "B");
            CD4069_GATE(setup, "C");
            CD4069_GATE(setup, "D");
            CD4069_GATE(setup, "E");
            CD4069_GATE(setup, "F");
#endif

            netlist.nl_setup_global.NET_C(setup, "A.VDD", "B.VDD", "C.VDD", "D.VDD", "E.VDD", "F.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.VSS", "B.VSS", "C.VSS", "D.VSS", "E.VSS", "F.VSS");

            netlist.nl_setup_global.DIPPINS(setup,   /*     +--------------+     */
                  "A.A", /*  A1 |1     ++    14| VDD */ "A.VDD",
                  "A.Q", /*  Y1 |2           13| A6  */ "F.A",
                  "B.A", /*  A2 |3           12| Y6  */ "F.Q",
                  "B.Q", /*  Y2 |4    4069   11| A5  */ "E.A",
                  "C.A", /*  A3 |5           10| Y5  */ "E.Q",
                  "C.Q", /*  Y3 |6            9| A4  */ "D.A",
                "A.VSS", /* VSS |7            8| Y4  */ "D.Q"
                       /*     +--------------+     */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier: CD4070_DIP
        //- Title: CD4070BM/CD4070BC Quad 2-Input EXCLUSIVE-OR Gate
        //- Pinalias: A,B,J,K,C,D,VSS,E,F,L,M,G,H,VDD
        //- Package: DIP
        //- NamingConvention: Naming conventions follow National Semiconductor datasheet
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheets/150/206783_DS.pdf
        //-
        //static NETLIST_START(CD4070_DIP)
        public static void netlist_CD4070_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4070_GATE(setup, "A");
            CD4070_GATE(setup, "B");
            CD4070_GATE(setup, "C");
            CD4070_GATE(setup, "D");
#endif

            netlist.nl_setup_global.NET_C(setup, "A.VDD", "B.VDD", "C.VDD", "D.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.VSS", "B.VSS", "C.VSS", "D.VSS");

            netlist.nl_setup_global.DIPPINS(setup,   /*     +--------------+     */
                  "A.A", /*   A |1     ++    14| VDD */ "A.VDD",
                  "A.B", /*   B |2           13| H   */ "D.B",
                  "A.Q", /*   J |3           12| G   */ "D.A",
                  "B.Q", /*   K |4    4070   11| M   */ "D.Q",
                  "B.A", /*   C |5           10| L   */ "C.Q",
                  "B.B", /*   D |6            9| F   */ "C.B",
                "A.VSS", /* VSS |7            8| E   */ "C.A"
                       /*     +--------------+     */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier: CD4316_DIP
        //- Title: 74HC/HCT4316 Quad bilateral switches
        //- Pinalias: 1Z,1Y,2Y,2Z,2S,3S,EQ,GND,VEE,3Z,3Y,4Y,4Z,4S,1S,VCC
        //- Package: DIP
        //- NamingConvention: Naming conventions follow Philips datasheet
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheet/philips/74HCT4316.pdf
        //-
        //static NETLIST_START(CD4316_DIP)
        public static void netlist_CD4316_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4316_GATE(setup, "A");
            CD4316_GATE(setup, "B");
            CD4316_GATE(setup, "C");
            CD4316_GATE(setup, "D");
#endif

            netlist.nl_setup_global.NET_C(setup, "A.E", "B.E", "C.E", "D.E");
            netlist.nl_setup_global.NET_C(setup, "A.VDD", "B.VDD", "C.VDD", "D.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.VSS", "B.VSS", "C.VSS", "D.VSS");

            netlist.nl_setup_global.PARAM(setup, "A.BASER", 45.0);
            netlist.nl_setup_global.PARAM(setup, "B.BASER", 45.0);
            netlist.nl_setup_global.PARAM(setup, "C.BASER", 45.0);
            netlist.nl_setup_global.PARAM(setup, "D.BASER", 45.0);

            netlist.nl_setup_global.DIPPINS(setup,   /*     +--------------+     */
                "A.R.2", /*  1Z |1     ++    16| VCC */ "A.VDD",
                "A.R.1", /*  1Y |2           15| 1S  */ "A.S",
                "B.R.1", /*  2Y |3           14| 4S  */ "D.S",
                "B.R.2", /*  2Z |4    4316   13| 4Z  */ "D.R.2",
                  "B.S", /*  2S |5           12| 4Y  */ "D.R.1",
                  "C.S", /*  3S |6           11| 3Y  */ "C.R.1",
                  "A.E", /*  EQ |7           10| 3Z  */ "C.R.2",
                "A.VSS", /* GND |8            9| VEE */ "VEE"
                       /*     +--------------+     */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier:  CD4538_DIP
        //- Title: CD4538BC Dual Precision Monostable
        //- Pinalias: C1,RC1,CLRQ1,B1,A1,Q1,QQ1,GND,QQ2,Q2,A2,B2,CLRQ2,RC2,C2,VCC
        //- Package: DIP
        //- NamingConvention: Naming conventions follow Fairchild Semiconductor datasheet
        //- Limitations:
        //-    Timing inaccuracies may occur for capacitances < 1nF. Please consult datasheet
        //-
        //- Example: 74123.cpp,74123_example
        //-
        //- FunctionTable:
        //-    https://pdf1.alldatasheet.com/datasheet-pdf/view/50871/FAIRCHILD/CD4538.html
        //-
        //static NETLIST_START(CD4538_DIP)
        public static void netlist_CD4538_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4538(setup, "A");
            CD4538(setup, "B");
#endif

            netlist.nl_setup_global.NET_C(setup, "A.VDD", "B.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.VSS", "B.VSS");

            netlist.nl_setup_global.DIPPINS(setup,    /*     +--------------+     */
                   "A.C", /*  1Z |1     ++    16| VCC */ "A.VDD",
                  "A.RC", /*  1Y |2           15| 1S  */ "B.C",
                "A.CLRQ", /*  2Y |3           14| 4S  */ "B.RC",
                   "A.A", /*  2Z |4    4316   13| 4Z  */ "B.CLRQ",
                   "A.B", /*  2S |5           12| 4Y  */ "B.A",
                   "A.Q", /*  3S |6           11| 3Y  */ "B.B",
                  "A.QQ", /*  EQ |7           10| 3Z  */ "B.Q",
                 "A.VSS", /* GND |8            9| VEE */ "B.QQ"
                        /*     +--------------+     */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //NETLIST_START(cd4xxx_lib)
        public static void netlist_cd4xxx_lib(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            TRUTHTABLE_ENTRY(CD4001_GATE);
            TRUTHTABLE_ENTRY(CD4011_GATE);
            TRUTHTABLE_ENTRY(CD4030_GATE);
            TRUTHTABLE_ENTRY(CD4049_GATE);
            TRUTHTABLE_ENTRY(CD4069_GATE);
            TRUTHTABLE_ENTRY(CD4070_GATE);

            LOCAL_LIB_ENTRY(CD4001_DIP);
            LOCAL_LIB_ENTRY(CD4011_DIP);
            LOCAL_LIB_ENTRY(CD4030_DIP);
            LOCAL_LIB_ENTRY(CD4049_DIP);
            LOCAL_LIB_ENTRY(CD4069_DIP);
            LOCAL_LIB_ENTRY(CD4070_DIP);

            /* DIP ONLY */
            LOCAL_LIB_ENTRY(CD4006_DIP);
            LOCAL_LIB_ENTRY(CD4013_DIP);
            LOCAL_LIB_ENTRY(CD4017_DIP);
            LOCAL_LIB_ENTRY(CD4022_DIP);
            LOCAL_LIB_ENTRY(CD4020_DIP);
            LOCAL_LIB_ENTRY(CD4024_DIP);
            LOCAL_LIB_ENTRY(CD4029_DIP);
            LOCAL_LIB_ENTRY(CD4042_DIP);
            LOCAL_LIB_ENTRY(CD4053_DIP);
            LOCAL_LIB_ENTRY(CD4066_DIP);
            LOCAL_LIB_ENTRY(CD4016_DIP);
            LOCAL_LIB_ENTRY(CD4076_DIP);
            LOCAL_LIB_ENTRY(CD4316_DIP);
            LOCAL_LIB_ENTRY(CD4538_DIP);

            LOCAL_LIB_ENTRY(MM5837_DIP);
#endif

            netlist.nl_setup_global.NETLIST_END();
        }
    }
}
