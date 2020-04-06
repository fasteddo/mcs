// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nlm_cd4xxx_global
    {
        /* ----------------------------------------------------------------------------
         *  Netlist Macros
         * ---------------------------------------------------------------------------*/
        //#define CD4001_GATE(name)                                                      \
        //        NET_REGISTER_DEV(CD4001_GATE, name)
        public static void CD4001_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "CD4001_GATE", name); }

        //#define CD4001_DIP(name)                                                      \
        //        NET_REGISTER_DEV(CD4001_DIP, name)

        //#define CD4070_GATE(name)                                                      \
        //        NET_REGISTER_DEV(CD4070_GATE, name)

        //#define CD4070_DIP(name)                                                      \
        //        NET_REGISTER_DEV(CD4070_DIP, name)

        /* ----------------------------------------------------------------------------
         *  DIP only macros
         * ---------------------------------------------------------------------------*/
        //#define CD4020_DIP(name)                                                      \
        //        NET_REGISTER_DEV(CD4020_DIP, name)

        //#define CD4066_DIP(name)                                                      \
        //        NET_REGISTER_DEV(CD4066_DIP, name)

        //#define CD4016_DIP(name)                                                      \
        //        NET_REGISTER_DEV(CD4016_DIP, name)

        //#define CD4316_DIP(name)                                                      \
        //        NET_REGISTER_DEV(CD4016_DIP, name)


        /*
         *   CD4001BC: Quad 2-Input NOR Buffered B Series Gate
         *
         *       +--------------+
         *    A1 |1     ++    14| VCC
         *    B1 |2           13| A6
         *    A2 |3           12| Y6
         *    Y2 |4    4001   11| A5
         *    A3 |5           10| Y5
         *    Y3 |6            9| A4
         *   GND |7            8| Y4
         *       +--------------+
         *
         */

        //static NETLIST_START(CD4001_DIP)
        public static void netlist_CD4001_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            CD4001_GATE(setup, "s1");
            CD4001_GATE(setup, "s2");
            CD4001_GATE(setup, "s3");
            CD4001_GATE(setup, "s4");

            netlist.nl_setup_global.NET_C(setup, "s1.VCC", "s2.VCC", "s3.VCC", "s4.VCC");
            netlist.nl_setup_global.NET_C(setup, "s1.GND", "s2.GND", "s3.GND", "s4.GND");

            netlist.nl_setup_global.DIPPINS(setup,    /*       +--------------+      */
                "s1.A",   /*    A1 |1     ++    14| VDD  */ "s1.VCC",
                "s1.B",   /*    B1 |2           13| A6   */ "s4.B",
                "s1.Q",   /*    A2 |3           12| Y6   */ "s4.A",
                "s2.Q",   /*    Y2 |4    4001   11| A5   */ "s4.Q",
                "s2.A",   /*    A3 |5           10| Y5   */ "s3.Q",
                "s2.B",   /*    Y3 |6            9| A4   */ "s3.B",
                "s1.GND", /*   VSS |7            8| Y4   */ "s3.A"
                          /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*  CD4020: 14-Stage Ripple Carry Binary Counters
         *
         *          +--------------+
         *      Q12 |1     ++    16| VDD
         *      Q13 |2           15| Q11
         *      Q14 |3           14| Q10
         *       Q6 |4    4020   13| Q8
         *       Q5 |5           12| Q9
         *       Q7 |6           11| RESET
         *       Q4 |7           10| IP (Input pulses)
         *      VSS |8            9| Q1
         *          +--------------+
         *
         *  Naming conventions follow Texas Instruments datasheet
         *
         *  FIXME: Timing depends on VDD-VSS
         *         This needs a cmos d-a/a-d proxy implementation.
         */

        //static NETLIST_START(CD4020_DIP)
        public static void netlist_CD4020_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_4020_global.CD4020(setup, "s1");
            netlist.nl_setup_global.DIPPINS(setup,     /*       +--------------+       */
                "s1.Q12",  /*   Q12 |1     ++    16| VDD   */ "s1.VDD",
                "s1.Q13",  /*   Q13 |2           15| Q11   */ "s1.Q11",
                "s1.Q14",  /*   Q14 |3           14| Q10   */ "s1.Q10",
                "s1.Q6",   /*    Q6 |4    4020   13| Q8    */ "s1.Q8",
                "s1.Q5",   /*    Q5 |5           12| Q9    */ "s1.Q9",
                "s1.Q7",   /*    Q7 |6           11| RESET */ "s1.RESET",
                "s1.Q4",   /*    Q4 |7           10| IP    */ "s1.IP",
                "s1.VSS",  /*   VSS |8            9| Q1    */ "s1.Q1"
                           /*       +--------------+       */
            );
                /*
                 * IP = (Input pulses)
                 */

            netlist.nl_setup_global.NETLIST_END();
        }


        /*  CD4066: Quad Bilateral Switch
         *
         *          +--------------+
         *   INOUTA |1     ++    14| VDD
         *   OUTINA |2           13| CONTROLA
         *   OUTINB |3           12| CONTROLD
         *   INOUTB |4    4066   11| INOUTD
         * CONTROLB |5           10| OUTIND
         * CONTROLC |6            9| OUTINC
         *      VSS |7            8| INOUTC
         *          +--------------+
         *
         *  FIXME: These devices are slow (~125 ns). THis is currently not reflected
         *
         *  Naming conventions follow National semiconductor datasheet
         *
         */
        //static NETLIST_START(CD4066_DIP)
        public static void netlist_CD4066_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_4066_global.CD4066_GATE(setup, "A");
            nld_4066_global.CD4066_GATE(setup, "B");
            nld_4066_global.CD4066_GATE(setup, "C");
            nld_4066_global.CD4066_GATE(setup, "D");

            netlist.nl_setup_global.NET_C(setup, "A.VDD", "B.VDD", "C.VDD", "D.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.VSS", "B.VSS", "C.VSS", "D.VSS");

            netlist.nl_setup_global.PARAM(setup, "A.BASER", 270.0);
            netlist.nl_setup_global.PARAM(setup, "B.BASER", 270.0);
            netlist.nl_setup_global.PARAM(setup, "C.BASER", 270.0);
            netlist.nl_setup_global.PARAM(setup, "D.BASER", 270.0);

            netlist.nl_setup_global.DIPPINS(setup,        /*          +--------------+          */
                "A.R.1",      /*   INOUTA |1     ++    14| VDD      */ "A.VDD",
                "A.R.2",      /*   OUTINA |2           13| CONTROLA */ "A.CTL",
                "B.R.1",      /*   OUTINB |3           12| CONTROLD */ "D.CTL",
                "B.R.2",      /*   INOUTB |4    4066   11| INOUTD   */ "D.R.1",
                "B.CTL",      /* CONTROLB |5           10| OUTIND   */ "D.R.2",
                "C.CTL",      /* CONTROLC |6            9| OUTINC   */ "C.R.1",
                "A.VSS",      /*      VSS |7            8| INOUTC   */ "C.R.2"
                              /*          +--------------+          */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(CD4016_DIP)
        public static void netlist_CD4016_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_4066_global.CD4066_GATE(setup, "A");
            nld_4066_global.CD4066_GATE(setup, "B");
            nld_4066_global.CD4066_GATE(setup, "C");
            nld_4066_global.CD4066_GATE(setup, "D");

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


        /*
         *  DM7486: Quad 2-Input Exclusive-OR Gates
         *
         *             Y = A+B
         *          +---+---++---+
         *          | A | B || Y |
         *          +===+===++===+
         *          | 0 | 0 || 0 |
         *          | 0 | 1 || 1 |
         *          | 1 | 0 || 1 |
         *          | 1 | 1 || 0 |
         *          +---+---++---+
         *
         *  Naming conventions follow National Semiconductor datasheet
         *
         */
        //static NETLIST_START(CD4070_DIP)
        public static void netlist_CD4070_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            CD4070_GATE(A);
            CD4070_GATE(B);
            CD4070_GATE(C);
            CD4070_GATE(D);
#endif

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC", "D.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND", "D.GND");

            netlist.nl_setup_global.DIPPINS(setup,  /*       +--------------+      */
                "A.A",  /*    A1 |1     ++    14| VCC  */ "A.VCC",
                "A.B",  /*    B1 |2           13| B4   */ "D.B",
                "A.Q",  /*    Y1 |3           12| A4   */ "D.A",
                "B.Q",  /*    Y2 |4    7486   11| Y4   */ "D.Q",
                "B.A",  /*    A2 |5           10| Y3   */ "C.Q",
                "B.B",  /*    B2 |6            9| B3   */ "C.B",
                "A.GND",/*   GND |7            8| A3   */ "C.A"
                      /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(CD4316_DIP)
        public static void netlist_CD4316_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_4316_global.CD4316_GATE(setup, "A");
            nld_4316_global.CD4316_GATE(setup, "B");
            nld_4316_global.CD4316_GATE(setup, "C");
            nld_4316_global.CD4316_GATE(setup, "D");

            netlist.nl_setup_global.NET_C(setup, "A.E", "B.E", "C.E", "D.E");
            netlist.nl_setup_global.NET_C(setup, "A.VDD", "B.VDD", "C.VDD", "D.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.VSS", "B.VSS", "C.VSS", "D.VSS");

            netlist.nl_setup_global.PARAM(setup, "A.BASER", 45.0);
            netlist.nl_setup_global.PARAM(setup, "B.BASER", 45.0);
            netlist.nl_setup_global.PARAM(setup, "C.BASER", 45.0);
            netlist.nl_setup_global.PARAM(setup, "D.BASER", 45.0);

            netlist.nl_setup_global.DIPPINS(setup,        /*          +--------------+          */
                "A.R.2",      /*       1Z |1     ++    16| VCC      */ "A.VDD",
                "A.R.1",      /*       1Y |2           15| 1S       */ "A.S",
                "B.R.1",      /*       2Y |3           14| 4S       */ "D.S",
                "B.R.2",      /*       2Z |4    4316   13| 4Z       */ "D.R.2",
                "B.S",        /*       2S |5           12| 4Y       */ "D.R.1",
                "C.S",        /*       3S |6           11| 3Y       */ "C.R.1",
                "A.E",        /*       /E |7           10| 3Z       */ "C.R.2",
                "A.VSS",      /*      GND |8            9| VEE      */ "VEE"
                              /*          +--------------+          */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //NETLIST_START(CD4XXX_lib)
        public static void netlist_CD4XXX_lib(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.TRUTHTABLE_START("CD4001_GATE", 2, 1, "");
                netlist.nl_setup_global.TT_HEAD("A , B | Q ");
                netlist.nl_setup_global.TT_LINE("0,0|1|85");
                netlist.nl_setup_global.TT_LINE("X,1|0|120");
                netlist.nl_setup_global.TT_LINE("1,X|0|120");
                netlist.nl_setup_global.TT_FAMILY("CD4XXX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("CD4070_GATE", 2, 1, "");
                netlist.nl_setup_global.TT_HEAD("A,B|Q ");
                netlist.nl_setup_global.TT_LINE("0,0|0|15");
                netlist.nl_setup_global.TT_LINE("0,1|1|22");
                netlist.nl_setup_global.TT_LINE("1,0|1|22");
                netlist.nl_setup_global.TT_LINE("1,1|0|15");
                netlist.nl_setup_global.TT_FAMILY("CD4XXX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "CD4001_DIP", netlist_CD4001_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "CD4070_DIP", netlist_CD4070_DIP);

            /* DIP ONLY */
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "CD4020_DIP", netlist_CD4020_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "CD4016_DIP", netlist_CD4016_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "CD4066_DIP", netlist_CD4066_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "CD4316_DIP", netlist_CD4316_DIP);

            netlist.nl_setup_global.NETLIST_END();
        }
    }
}
