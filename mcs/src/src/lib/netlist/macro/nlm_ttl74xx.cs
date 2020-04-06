// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nlm_ttl74xx_global
    {
        //#define TTL_7400_GATE(name)                                                   \
        //        NET_REGISTER_DEV(TTL_7400_GATE, name)
        public static void TTL_7400_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_7400_GATE", name); }

        //#define TTL_7400_NAND(name, cA, cB)                                           \
        //        NET_REGISTER_DEV(TTL_7400_NAND, name)                                  \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A, cA)                                               \
        //        NET_CONNECT(name, B, cB)

        //#define TTL_7400_DIP(name)                                                    \
        //        NET_REGISTER_DEV(TTL_7400_DIP, name)


        //#define TTL_7402_GATE(name)                                                   \
        //        NET_REGISTER_DEV(TTL_7402_GATE, name)
        public static void TTL_7402_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_7402_GATE", name); }

        //#define TTL_7402_NOR(name, cI1, cI2)                                          \
        //        NET_REGISTER_DEV(TTL_7402_NOR, name)                                  \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A, cI1)                                             \
        //        NET_CONNECT(name, B, cI2)

        //#define TTL_7402_DIP(name)                                                    \
        //        NET_REGISTER_DEV(TTL_7402_DIP, name)

        //#define TTL_7404_GATE(name)                                                   \
        //        NET_REGISTER_DEV(TTL_7404_GATE, name)
        public static void TTL_7404_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_7404_GATE", name); }

        //#define TTL_7404_INVERT(name, cA)                                             \
        //        NET_REGISTER_DEV(TTL_7404_INVERT, name)                               \
        //        NET_CONNECT(name, A, cA)

        //#define TTL_7404_DIP(name)                                                    \
        //        NET_REGISTER_DEV(TTL_7404_DIP, name)


        //#define TTL_7408_GATE(name)                                                   \
        //        NET_REGISTER_DEV(TTL_7408_GATE, name)
        public static void TTL_7408_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_7408_GATE", name); }

        //#define TTL_7408_AND(name, cA, cB)                                            \
        //        NET_REGISTER_DEV(TTL_7408_AND, name)                                   \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A, cA)                                               \
        //        NET_CONNECT(name, B, cB)

        //#define TTL_7408_DIP(name)                                                    \
        //        NET_REGISTER_DEV(TTL_7408_DIP, name)


        //#define TTL_7410_GATE(name)                                                   \
        //        NET_REGISTER_DEV(TTL_7410_GATE, name)
        public static void TTL_7410_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_7410_GATE", name); }

        //#define TTL_7410_NAND(name, cI1, cI2, cI3)                                    \
        //        NET_REGISTER_DEV(TTL_7410_NAND, name)                                 \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A, cI1)                                             \
        //        NET_CONNECT(name, B, cI2)                                             \
        //        NET_CONNECT(name, C, cI3)

        //#define TTL_7410_DIP(name)                                                    \
        //        NET_REGISTER_DEV(TTL_7410_DIP, name)


        //#define TTL_7411_GATE(name)                                                   \
        //        NET_REGISTER_DEV(TTL_7411_GATE, name)
        public static void TTL_7411_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_7411_GATE", name); }

        //#define TTL_7411_AND(name, cI1, cI2, cI3)                                     \
        //        NET_REGISTER_DEV(TTL_7411_AND, name)                                  \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A, cI1)                                             \
        //        NET_CONNECT(name, B, cI2)                                             \
        //        NET_CONNECT(name, C, cI3)

        //#define TTL_7411_DIP(name)                                                    \
        //        NET_REGISTER_DEV(TTL_7411_DIP, name)


        //#define TTL_7414_GATE(name)                                                   \
        //        NET_REGISTER_DEV(TTL_7414_GATE, name)

        //#define TTL_7414_DIP(name)                                                    \
        //        NET_REGISTER_DEV(TTL_7414_DIP, name)


        //#define TTL_74LS14_GATE(name)                                                 \
        //        NET_REGISTER_DEV(TTL_74LS14_GATE, name)

        //#define TTL_74LS14_DIP(name)                                                  \
        //        NET_REGISTER_DEV(TTL_74LS14_DIP, name)


        //#define TTL_7416_GATE(name)                                                   \
        //        NET_REGISTER_DEV(TTL_7416_GATE, name)
        public static void TTL_7416_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_7416_GATE", name); }

        //#define TTL_7416_DIP(name)                                                    \
        //        NET_REGISTER_DEV(TTL_7416_DIP, name)


        //#define TTL_7420_GATE(name)                                                   \
        //        NET_REGISTER_DEV(TTL_7420_GATE, name)
        public static void TTL_7420_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_7420_GATE", name); }

        //#define TTL_7420_NAND(name, cI1, cI2, cI3, cI4)                               \
        //        NET_REGISTER_DEV(TTL_7420_NAND, name)                                 \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A, cI1)                                             \
        //        NET_CONNECT(name, B, cI2)                                             \
        //        NET_CONNECT(name, C, cI3)                                             \
        //        NET_CONNECT(name, D, cI4)

        //#define TTL_7420_DIP(name)                                                     \
        //        NET_REGISTER_DEV(TTL_7420_DIP, name)


        //#define TTL_7425_GATE(name)                                                    \
        //        NET_REGISTER_DEV(TTL_7425_GATE, name)
        public static void TTL_7425_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_7425_GATE", name); }

        //#define TTL_7425_NOR(name, cI1, cI2, cI3, cI4)                                 \
        //        NET_REGISTER_DEV(TTL_7425_NOR, name)                                   \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A, cI1)                                              \
        //        NET_CONNECT(name, B, cI2)                                              \
        //        NET_CONNECT(name, C, cI3)                                              \
        //        NET_CONNECT(name, D, cI4)

        //#define TTL_7425_DIP(name)                                                     \
        //        NET_REGISTER_DEV(TTL_7425_DIP, name)


        //#define TTL_7427_GATE(name)                                                    \
        //        NET_REGISTER_DEV(TTL_7427_GATE, name)
        public static void TTL_7427_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_7427_GATE", name); }

        //#define TTL_7427_NOR(name, cI1, cI2, cI3)                                      \
        //        NET_REGISTER_DEV(TTL_7427_NOR, name)                                   \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A, cI1)                                              \
        //        NET_CONNECT(name, B, cI2)                                              \
        //        NET_CONNECT(name, C, cI3)

        //#define TTL_7427_DIP(name)                                                     \
        //        NET_REGISTER_DEV(TTL_7427_DIP, name)


        //#define TTL_7430_GATE(name)                                                    \
        //        NET_REGISTER_DEV(TTL_7430_GATE, name)
        public static void TTL_7430_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_7430_GATE", name); }

        //#define TTL_7430_NAND(name, cI1, cI2, cI3, cI4, cI5, cI6, cI7, cI8)            \
        //        NET_REGISTER_DEV(TTL_7430_NAND, name)                                  \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A, cI1)                                              \
        //        NET_CONNECT(name, B, cI2)                                              \
        //        NET_CONNECT(name, C, cI3)                                              \
        //        NET_CONNECT(name, D, cI4)                                              \
        //        NET_CONNECT(name, E, cI5)                                              \
        //        NET_CONNECT(name, F, cI6)                                              \
        //        NET_CONNECT(name, G, cI7)                                              \
        //        NET_CONNECT(name, H, cI8)

        //#define TTL_7430_DIP(name)                                                     \
        //        NET_REGISTER_DEV(TTL_7430_DIP, name)


        //#define TTL_7432_GATE(name)                                                    \
        //        NET_REGISTER_DEV(TTL_7432_OR, name)
        public static void TTL_7432_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_7432_OR", name); }

        //#define TTL_7432_OR(name, cI1, cI2)                                            \
        //        NET_REGISTER_DEV(TTL_7432_OR, name)                                    \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A, cI1)                                              \
        //        NET_CONNECT(name, B, cI2)

        //#define TTL_7432_DIP(name)                                                     \
        //        NET_REGISTER_DEV(TTL_7432_DIP, name)


        //#define TTL_7437_GATE(name)                                                    \
        //        NET_REGISTER_DEV(TTL_7437_GATE, name)
        public static void TTL_7437_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_7437_GATE", name); }

        //#define TTL_7437_NAND(name, cA, cB)                                            \
        //        NET_REGISTER_DEV(TTL_7437_NAND, name)                                  \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A, cA)                                               \
        //        NET_CONNECT(name, B, cB)

        //#define TTL_7437_DIP(name)                                                     \
        //        NET_REGISTER_DEV(TTL_7437_DIP, name)

//#if (NL_USE_TRUTHTABLE_7448)
        //#define TTL_7448(name, cA0, cA1, cA2, cA3, cLTQ, cBIQ, cRBIQ)                  \
        //        NET_REGISTER_DEV(TTL_7448, name)                                       \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A, cA0)                                              \
        //        NET_CONNECT(name, B, cA1)                                              \
        //        NET_CONNECT(name, C, cA2)                                              \
        //        NET_CONNECT(name, D, cA3)                                              \
        //        NET_CONNECT(name, LTQ, cLTQ)                                           \
        //        NET_CONNECT(name, BIQ, cBIQ)                                           \
        //        NET_CONNECT(name, RBIQ, cRBIQ)

        //#define TTL_7448_DIP(name)                                                     \
        //        NET_REGISTER_DEV(TTL_7448_DIP, name)
//#endif

        //#define TTL_7486_GATE(name)                                                    \
        //        NET_REGISTER_DEV(TTL_7486_GATE, name)
        public static void TTL_7486_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_7486_GATE", name); }

        //#define TTL_7486_XOR(name, cA, cB)                                             \
        //        NET_REGISTER_DEV(TTL_7486_XOR, name)                                   \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A, cA)                                               \
        //        NET_CONNECT(name, B, cB)

        //#define TTL_7486_DIP(name)                                                     \
        //        NET_REGISTER_DEV(TTL_7486_DIP, name)

//#if (NL_USE_TRUTHTABLE_74107)
        //#define TTL_74107(name, cCLK, cJ, cK, cCLRQ)                                   \
        //        NET_REGISTER_DEV(TTL_74107, name)                                      \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, CLK, cCLK)                                           \
        //        NET_CONNECT(name, J, cJ)                                               \
        //        NET_CONNECT(name, K, cK)                                               \
        //        NET_CONNECT(name, CLRQ, cCLRQ)

        //#define TTL_74107_DIP(name)                                                    \
        //        NET_REGISTER_DEV(TTL_74107_DIP, name)
//#endif

        //#define TTL_74155_DIP(name)                                                    \
        //        NET_REGISTER_DEV(TTL_74155_DIP, name)

        //#define TTL_74156_DIP(name)                                                    \
        //        NET_REGISTER_DEV(TTL_74156_DIP, name)


        //#define TTL_74260_GATE(name)                                                   \
        //        NET_REGISTER_DEV(TTL_74260_GATE, name)
        public static void TTL_74260_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_74260_GATE", name); }

        //#define TTL_74260_NOR(name, cA, cB, cC, cD, cE) \
        //        NET_REGISTER_DEV(TTL_74260_NOR, name)   \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A, cA)    \
        //        NET_CONNECT(name, B, cB)    \
        //        NET_CONNECT(name, C, cC)    \
        //        NET_CONNECT(name, D, cD)    \
        //        NET_CONNECT(name, E, cE)

        //#define TTL_74260_DIP(name) \
        //        NET_REGISTER_DEV(TTL_74260_DIP, name)

        //#define TTL_74279_DIP(name)                                                    \
        //        NET_REGISTER_DEV(TTL_74279_DIP, name)

        //#define DM9312(name, cA, cB, cC, cSTROBE, cD0, cD1, cD2, cD3, cD4, cD5, cD6, cD7)     \
        //        NET_REGISTER_DEV(DM9312, name)                                         \
        //        NET_CONNECT(name, VCC, VCC)                                            \
        //        NET_CONNECT(name, GND, GND)                                            \
        //        NET_CONNECT(name, A,  cA)       \
        //        NET_CONNECT(name, B,  cB)       \
        //        NET_CONNECT(name, C,  cC)       \
        //        NET_CONNECT(name, G,  cSTROBE)  \
        //        NET_CONNECT(name, D0, cD0)      \
        //        NET_CONNECT(name, D1, cD1)      \
        //        NET_CONNECT(name, D2, cD2)      \
        //        NET_CONNECT(name, D3, cD3)      \
        //        NET_CONNECT(name, D4, cD4)      \
        //        NET_CONNECT(name, D5, cD5)      \
        //        NET_CONNECT(name, D6, cD6)      \
        //        NET_CONNECT(name, D7, cD7)

        //#define DM9312_DIP(name)                                                      \
        //        NET_REGISTER_DEV(DM9312_DIP, name)


        /*
         *  DM7400: Quad 2-Input NAND Gates
         *
         *                  _
         *              Y = AB
         *          +---+---++---+
         *          | A | B || Y |
         *          +===+===++===+
         *          | 0 | 0 || 1 |
         *          | 0 | 1 || 1 |
         *          | 1 | 0 || 1 |
         *          | 1 | 1 || 0 |
         *          +---+---++---+
         *
         *  Naming conventions follow National Semiconductor datasheet
         *
         */

        //static NETLIST_START(TTL_7400_DIP)
        public static void netlist_TTL_7400_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_7400_GATE(setup, "A");
            TTL_7400_GATE(setup, "B");
            TTL_7400_GATE(setup, "C");
            TTL_7400_GATE(setup, "D");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC", "D.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND", "D.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.A",  /*    A1 |1     ++    14| VCC  */ "A.VCC",
                "A.B",  /*    B1 |2           13| B4   */ "D.B",
                "A.Q",  /*    Y1 |3           12| A4   */ "D.A",
                "B.A",  /*    A2 |4    7400   11| Y4   */ "D.Q",
                "B.B",  /*    B2 |5           10| B3   */ "C.B",
                "B.Q",  /*    Y2 |6            9| A3   */ "C.A",
                "A.GND",/*   GND |7            8| Y3   */ "C.Q"
                        /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *  DM7402: Quad 2-Input NOR Gates
         *
         *              Y = A+B
         *          +---+---++---+
         *          | A | B || Y |
         *          +===+===++===+
         *          | 0 | 0 || 1 |
         *          | 0 | 1 || 0 |
         *          | 1 | 0 || 0 |
         *          | 1 | 1 || 0 |
         *          +---+---++---+
         *
         *  Naming conventions follow National Semiconductor datasheet
         *
         */

        //static NETLIST_START(TTL_7402_DIP)
        public static void netlist_TTL_7402_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_7402_GATE(setup, "A");
            TTL_7402_GATE(setup, "B");
            TTL_7402_GATE(setup, "C");
            TTL_7402_GATE(setup, "D");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC", "D.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND", "D.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.Q",  /*    Y1 |1     ++    14| VCC  */ "A.VCC",
                "A.A",  /*    A1 |2           13| Y4   */ "D.Q",
                "A.B",  /*    B1 |3           12| B4   */ "D.B",
                "B.Q",  /*    Y2 |4    7402   11| A4   */ "D.A",
                "B.A",  /*    A2 |5           10| Y3   */ "C.Q",
                "B.B",  /*    B2 |6            9| B3   */ "C.B",
                "A.GND",/*   GND |7            8| A3   */ "C.A"
                        /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *   DM7404: Hex Inverting Gates
         *
         *             Y = A
         *          +---++---+
         *          | A || Y |
         *          +===++===+
         *          | 0 || 1 |
         *          | 1 || 0 |
         *          +---++---+
         *
         *  Naming conventions follow National Semiconductor datasheet
         *
         */

        //static NETLIST_START(TTL_7404_DIP)
        public static void netlist_TTL_7404_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_7404_GATE(setup, "A");
            TTL_7404_GATE(setup, "B");
            TTL_7404_GATE(setup, "C");
            TTL_7404_GATE(setup, "D");
            TTL_7404_GATE(setup, "E");
            TTL_7404_GATE(setup, "F");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC", "D.VCC", "E.VCC", "F.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND", "D.GND", "E.GND", "F.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.A",  /*    A1 |1     ++    14| VCC  */ "A.VCC",
                "A.Q",  /*    Y1 |2           13| A6   */ "F.A",
                "B.A",  /*    A2 |3           12| Y6   */ "F.Q",
                "B.Q",  /*    Y2 |4    7404   11| A5   */ "E.A",
                "C.A",  /*    A3 |5           10| Y5   */ "E.Q",
                "C.Q",  /*    Y3 |6            9| A4   */ "D.A",
                "A.GND",/*   GND |7            8| Y4   */ "D.Q"
                        /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *  DM7408: Quad 2-Input AND Gates
         *
         *
         *              Y = AB
         *          +---+---++---+
         *          | A | B || Y |
         *          +===+===++===+
         *          | 0 | 0 || 0 |
         *          | 0 | 1 || 0 |
         *          | 1 | 0 || 0 |
         *          | 1 | 1 || 1 |
         *          +---+---++---+
         *
         *  Naming conventions follow National Semiconductor datasheet
         *
         */

        //static NETLIST_START(TTL_7408_DIP)
        public static void netlist_TTL_7408_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_7408_GATE(setup, "A");
            TTL_7408_GATE(setup, "B");
            TTL_7408_GATE(setup, "C");
            TTL_7408_GATE(setup, "D");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC", "D.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND", "D.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.A",  /*    A1 |1     ++    14| VCC  */ "A.VCC",
                "A.B",  /*    B1 |2           13| B4   */ "D.B",
                "A.Q",  /*    Y1 |3           12| A4   */ "D.A",
                "B.A",  /*    A2 |4    7400   11| Y4   */ "D.Q",
                "B.B",  /*    B2 |5           10| B3   */ "C.B",
                "B.Q",  /*    Y2 |6            9| A3   */ "C.A",
                "A.GND",/*   GND |7            8| Y3   */ "C.Q"
                        /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *  DM7410: Triple 3-Input NAND Gates
         *                  __
         *              Y = ABC
         *          +---+---+---++---+
         *          | A | B | C || Y |
         *          +===+===+===++===+
         *          | X | X | 0 || 1 |
         *          | X | 0 | X || 1 |
         *          | 0 | X | X || 1 |
         *          | 1 | 1 | 1 || 0 |
         *          +---+---+---++---+
         *
         *  Naming conventions follow National Semiconductor datasheet
         *
         */

        //static NETLIST_START(TTL_7410_DIP)
        public static void netlist_TTL_7410_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_7410_GATE(setup, "A");
            TTL_7410_GATE(setup, "B");
            TTL_7410_GATE(setup, "C");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.A",  /*    A1 |1     ++    14| VCC  */ "A.VCC",
                "A.B",  /*    B1 |2           13| C1   */ "A.C",
                "B.A",  /*    A2 |3           12| Y1   */ "A.Q",
                "B.B",  /*    B2 |4    7410   11| C3   */ "C.C",
                "B.C",  /*    C2 |5           10| B3   */ "C.B",
                "B.Q",  /*    Y2 |6            9| A3   */ "C.A",
                "A.GND",/*   GND |7            8| Y3   */ "C.Q"
                        /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *  DM7411: Triple 3-Input AND Gates
         *
         *              Y = ABC
         *          +---+---+---++---+
         *          | A | B | C || Y |
         *          +===+===+===++===+
         *          | X | X | 0 || 0 |
         *          | X | 0 | X || 0 |
         *          | 0 | X | X || 0 |
         *          | 1 | 1 | 1 || 1 |
         *          +---+---+---++---+
         *
         *  Naming conventions follow National Semiconductor datasheet
         *
         */

        //static NETLIST_START(TTL_7411_DIP)
        public static void netlist_TTL_7411_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_7411_GATE(setup, "A");
            TTL_7411_GATE(setup, "B");
            TTL_7411_GATE(setup, "C");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.A",  /*    A1 |1     ++    14| VCC  */ "A.VCC",
                "A.B",  /*    B1 |2           13| C1   */ "A.C",
                "B.A",  /*    A2 |3           12| Y1   */ "A.Q",
                "B.B",  /*    B2 |4    7411   11| C3   */ "C.C",
                "B.C",  /*    C2 |5           10| B3   */ "C.B",
                "B.Q",  /*    Y2 |6            9| A3   */ "C.A",
                "A.GND",/*   GND |7            8| Y3   */ "C.Q"
                       /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *   DM7414/DM74LS14: Hex Inverter with
         *                    Schmitt Trigger Inputs
         *
         */

        //static NETLIST_START(TTL_7414_GATE)
        public static void netlist_TTL_7414_GATE(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_schmitt_global.SCHMITT_TRIGGER(setup, "X", "DM7414");
            netlist.nl_setup_global.ALIAS(setup, "A", "X.A");
            netlist.nl_setup_global.ALIAS(setup, "Q", "X.Q");
            netlist.nl_setup_global.ALIAS(setup, "GND", "X.GND");
            netlist.nl_setup_global.ALIAS(setup, "VCC", "X.VCC");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(TTL_74LS14_GATE)
        public static void netlist_TTL_74LS14_GATE(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_schmitt_global.SCHMITT_TRIGGER(setup, "X", "DM74LS14");
            netlist.nl_setup_global.ALIAS(setup, "A", "X.A");
            netlist.nl_setup_global.ALIAS(setup, "Q", "X.Q");
            netlist.nl_setup_global.ALIAS(setup, "GND", "X.GND");
            netlist.nl_setup_global.ALIAS(setup, "VCC", "X.VCC");

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(TTL_7414_DIP)
        public static void netlist_TTL_7414_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_schmitt_global.SCHMITT_TRIGGER(setup, "A", "DM7414");
            nld_schmitt_global.SCHMITT_TRIGGER(setup, "B", "DM7414");
            nld_schmitt_global.SCHMITT_TRIGGER(setup, "C", "DM7414");
            nld_schmitt_global.SCHMITT_TRIGGER(setup, "D", "DM7414");
            nld_schmitt_global.SCHMITT_TRIGGER(setup, "E", "DM7414");
            nld_schmitt_global.SCHMITT_TRIGGER(setup, "F", "DM7414");

            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND", "D.GND", "E.GND", "F.GND");
            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC", "D.VCC", "E.VCC", "F.VCC");

            netlist.nl_setup_global.DIPPINS(setup,    /*       +--------------+      */
                "A.A",   /*    A1 |1     ++    14| VCC  */ "VCC.I",
                "A.Q",   /*    Y1 |2           13| A6   */ "F.A",
                "B.A",   /*    A2 |3           12| Y6   */ "F.Q",
                "B.Q",   /*    Y2 |4    7414   11| A5   */ "E.A",
                "C.A",   /*    A3 |5           10| Y5   */ "E.Q",
                "C.Q",   /*    Y3 |6            9| A4   */ "D.A",
                "A.GND", /*   GND |7            8| Y4   */ "D.Q"
                        /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(TTL_74LS14_DIP)
        public static void netlist_TTL_74LS14_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_schmitt_global.SCHMITT_TRIGGER(setup, "A", "DM74LS14");
            nld_schmitt_global.SCHMITT_TRIGGER(setup, "B", "DM74LS14");
            nld_schmitt_global.SCHMITT_TRIGGER(setup, "C", "DM74LS14");
            nld_schmitt_global.SCHMITT_TRIGGER(setup, "D", "DM74LS14");
            nld_schmitt_global.SCHMITT_TRIGGER(setup, "E", "DM74LS14");
            nld_schmitt_global.SCHMITT_TRIGGER(setup, "F", "DM74LS14");

            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND", "D.GND", "E.GND", "F.GND");
            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC", "D.VCC", "E.VCC", "F.VCC");

            netlist.nl_setup_global.DIPPINS(setup,    /*       +--------------+      */
                "A.A",   /*    A1 |1     ++    14| VCC  */ "VCC.I",
                "A.Q",   /*    Y1 |2           13| A6   */ "F.A",
                "B.A",   /*    A2 |3           12| Y6   */ "F.Q",
                "B.Q",   /*    Y2 |4   74LS14  11| A5   */ "E.A",
                "C.A",   /*    A3 |5           10| Y5   */ "E.Q",
                "C.Q",   /*    Y3 |6            9| A4   */ "D.A",
                "A.GND", /*   GND |7            8| Y4   */ "D.Q"
                        /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *   DM7416: Hex Inverting Buffers with
         *           High Voltage Open-Collector Outputs
         *
         */

        //static NETLIST_START(TTL_7416_DIP)
        public static void netlist_TTL_7416_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_7416_GATE(setup, "A");
            TTL_7416_GATE(setup, "B");
            TTL_7416_GATE(setup, "C");
            TTL_7416_GATE(setup, "D");
            TTL_7416_GATE(setup, "E");
            TTL_7416_GATE(setup, "F");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC", "D.VCC", "E.VCC", "F.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND", "D.GND", "E.GND", "F.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.A",  /*    A1 |1     ++    14| VCC  */ "A.VCC",
                "A.Q",  /*    Y1 |2           13| A6   */ "F.A",
                "B.A",  /*    A2 |3           12| Y6   */ "F.Q",
                "B.Q",  /*    Y2 |4    7416   11| A5   */ "E.A",
                "C.A",  /*    A3 |5           10| Y5   */ "E.Q",
                "C.Q",  /*    Y3 |6            9| A4   */ "D.A",
                "A.GND",/*   GND |7            8| Y4   */ "D.Q"
                        /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *  DM7420: Dual 4-Input NAND Gates
         *
         *                  ___
         *              Y = ABCD
         *          +---+---+---+---++---+
         *          | A | B | C | D || Y |
         *          +===+===+===+===++===+
         *          | X | X | X | 0 || 1 |
         *          | X | X | 0 | X || 1 |
         *          | X | 0 | X | X || 1 |
         *          | 0 | X | X | X || 1 |
         *          | 1 | 1 | 1 | 1 || 0 |
         *          +---+---+---+---++---+
         *
         *  Naming conventions follow National Semiconductor datasheet *
         */

        //static NETLIST_START(TTL_7420_DIP)
        public static void netlist_TTL_7420_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_7420_GATE(setup, "A");
            TTL_7420_GATE(setup, "B");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND");
            netlist.devices.nld_system_global.NC_PIN(setup, "NC");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.A",  /*    A1 |1     ++    14| VCC  */ "A.VCC",
                "A.B",  /*    B1 |2           13| D2   */ "B.D",
                "NC.I", /*    NC |3           12| C2   */ "B.C",
                "A.C",  /*    C1 |4    7420   11| NC   */ "NC.I",
                "A.D",  /*    D1 |5           10| B2   */ "B.B",
                "A.Q",  /*    Y1 |6            9| A2   */ "B.A",
                "A.GND",/*   GND |7            8| Y2   */ "B.Q"
                        /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *  DM7425: Dual 4-Input NOR Gates
         *
         *                  ______
         *              Y = A+B+C+D
         *          +---+---+---+---+---++---+
         *          | A | B | C | D | X || Y |
         *          +===+===+===+===+===++===+
         *          | X | X | X | X | 0 || Z |
         *          | 0 | 0 | 0 | 0 | 1 || 1 |
         *          | X | X | X | 1 | 1 || 0 |
         *          | X | X | 1 | X | 1 || 0 |
         *          | X | 1 | X | X | 1 || 0 |
         *          | 1 | X | X | X | 1 || 0 |
         *          +---+---+---+---+---++---+
         *
         *  FIXME: The "X" input and high impedance output are currently not simulated.
         *
         *  Naming conventions follow National Semiconductor datasheet
         *
         */

        //static NETLIST_START(TTL_7425_DIP)
        public static void netlist_TTL_7425_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_7425_GATE(setup, "A");
            TTL_7425_GATE(setup, "B");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND");
            netlist.devices.nld_system_global.NC_PIN(setup, "XA"); // FIXME: Functionality needs to be implemented
            netlist.devices.nld_system_global.NC_PIN(setup, "XB"); // FIXME: Functionality needs to be implemented

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.A",  /*    A1 |1     ++    14| VCC  */ "A.VCC",
                "A.B",  /*    B1 |2           13| D2   */ "B.D",
                "XA.I", /*    X1 |3           12| C2   */ "B.C",
                "A.C",  /*    C1 |4    7425   11| X2   */ "XB.I",
                "A.D",  /*    D1 |5           10| B2   */ "B.B",
                "A.Q",  /*    Y1 |6            9| A2   */ "B.A",
                "A.GND",/*   GND |7            8| Y2   */ "B.Q"
                        /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *  DM7427: Triple 3-Input NOR Gates
         *
         *                  ____
         *              Y = A+B+C
         *          +---+---+---++---+
         *          | A | B | C || Y |
         *          +===+===+===++===+
         *          | X | X | 1 || 0 |
         *          | X | 1 | X || 0 |
         *          | 1 | X | X || 0 |
         *          | 0 | 0 | 0 || 1 |
         *          +---+---+---++---+
         *
         *  Naming conventions follow National Semiconductor datasheet
         *
         */

        //static NETLIST_START(TTL_7427_DIP)
        public static void netlist_TTL_7427_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_7427_GATE(setup, "A");
            TTL_7427_GATE(setup, "B");
            TTL_7427_GATE(setup, "C");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.A",  /*    A1 |1     ++    14| VCC  */ "A.VCC",
                "A.B",  /*    B1 |2           13| C1   */ "A.C",
                "B.A",  /*    A2 |3           12| Y1   */ "A.Q",
                "B.B",  /*    B2 |4    7427   11| C3   */ "C.C",
                "B.C",  /*    C2 |5           10| B3   */ "C.B",
                "B.Q",  /*    Y2 |6            9| A3   */ "C.A",
                "A.GND",/*   GND |7            8| Y3   */ "C.Q"
                       /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *  DM7430: 8-Input NAND Gate
         *
         *                  _______
         *              Y = ABCDEFGH
         *          +---+---+---+---+---+---+---+---++---+
         *          | A | B | C | D | E | F | G | H || Y |
         *          +===+===+===+===+===+===+===+===++===+
         *          | X | X | X | X | X | X | X | 0 || 1 |
         *          | X | X | X | X | X | X | 0 | X || 1 |
         *          | X | X | X | X | X | 0 | X | X || 1 |
         *          | X | X | X | X | 0 | X | X | X || 1 |
         *          | X | X | X | 0 | X | X | X | X || 1 |
         *          | X | X | 0 | X | X | X | X | X || 1 |
         *          | X | 0 | X | X | X | X | X | X || 1 |
         *          | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 || 0 |
         *          +---+---+---+---+---+---+---+---++---+
         *
         *  Naming conventions follow National Semiconductor datasheet
         */

        //static NETLIST_START(TTL_7430_DIP)
        public static void netlist_TTL_7430_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_7430_GATE(setup, "A");

            netlist.devices.nld_system_global.NC_PIN(setup, "NC9");
            netlist.devices.nld_system_global.NC_PIN(setup, "NC10");
            netlist.devices.nld_system_global.NC_PIN(setup, "NC13");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.A",  /*     A |1     ++    14| VCC  */ "A.VCC",
                "A.B",  /*     B |2           13| NC   */ "NC13.I",
                "A.C",  /*     C |3           12| H    */ "A.H",
                "A.D",  /*     D |4    7430   11| G    */ "A.G",
                "A.E",  /*     E |5           10| NC   */ "NC10.I",
                "A.F",  /*     F |6            9| NC   */ "NC9.I",
                "A.GND",/*   GND |7            8| Y    */ "A.Q"
                       /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *  DM7432: Quad 2-Input OR Gates
         *
         *                  __
         *              Y = A+B
         *          +---+---++---+
         *          | A | B || Y |
         *          +===+===++===+
         *          | 0 | 0 || 0 |
         *          | 0 | 1 || 1 |
         *          | 1 | 0 || 1 |
         *          | 1 | 1 || 1 |
         *          +---+---++---+
         *
         *  Naming conventions follow National Semiconductor datasheet
         *
         */

        //static NETLIST_START(TTL_7432_DIP)
        public static void netlist_TTL_7432_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_7432_GATE(setup, "A");
            TTL_7432_GATE(setup, "B");
            TTL_7432_GATE(setup, "C");
            TTL_7432_GATE(setup, "D");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC", "D.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND", "D.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.A",  /*    A1 |1     ++    14| VCC  */ "A.VCC",
                "A.B",  /*    B1 |2           13| B4   */ "D.B",
                "A.Q",  /*    Y1 |3           12| A4   */ "D.A",
                "B.A",  /*    A2 |4    7400   11| Y4   */ "D.Q",
                "B.B",  /*    B2 |5           10| B3   */ "C.B",
                "B.Q",  /*    Y2 |6            9| A3   */ "C.A",
                "A.GND",/*   GND |7            8| Y3   */ "C.Q"
                        /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *  DM7437: Quad 2-Input NAND Gates
         *
         *                  _
         *              Y = AB
         *          +---+---++---+
         *          | A | B || Y |
         *          +===+===++===+
         *          | 0 | 0 || 1 |
         *          | 0 | 1 || 1 |
         *          | 1 | 0 || 1 |
         *          | 1 | 1 || 0 |
         *          +---+---++---+
         *
         *  Naming conventions follow National Semiconductor datasheet
         *
         *  NOTE: Same as 7400, but drains higher output currents.
         *         Netlist currently does not model over currents (should it ever?)
         */

        //static NETLIST_START(TTL_7437_DIP)
        public static void netlist_TTL_7437_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_7437_GATE(setup, "A");
            TTL_7437_GATE(setup, "B");
            TTL_7437_GATE(setup, "C");
            TTL_7437_GATE(setup, "D");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC", "D.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND", "D.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.A",  /*    A1 |1     ++    14| VCC  */ "A.VCC",
                "A.B",  /*    B1 |2           13| B4   */ "D.B",
                "A.Q",  /*    Y1 |3           12| A4   */ "D.A",
                "B.A",  /*    A2 |4    7400   11| Y4   */ "D.Q",
                "B.B",  /*    B2 |5           10| B3   */ "C.B",
                "B.Q",  /*    Y2 |6            9| A3   */ "C.A",
                "A.GND",/*   GND |7            8| Y3   */ "C.Q"
                        /*       +--------------+      */
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

        //static NETLIST_START(TTL_7486_DIP)
        public static void netlist_TTL_7486_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_7486_GATE(setup, "A");
            TTL_7486_GATE(setup, "B");
            TTL_7486_GATE(setup, "C");
            TTL_7486_GATE(setup, "D");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC", "D.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND", "D.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.A",  /*    A1 |1     ++    14| VCC  */ "A.VCC",
                "A.B",  /*    B1 |2           13| B4   */ "D.B",
                "A.Q",  /*    Y1 |3           12| A4   */ "D.A",
                "B.A",  /*    A2 |4    7486   11| Y4   */ "D.Q",
                "B.B",  /*    B2 |5           10| B3   */ "C.B",
                "B.Q",  /*    Y2 |6            9| A3   */ "C.A",
                "A.GND",/*   GND |7            8| Y3   */ "C.Q"
                       /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         * DM74155/DM74156: Dual 2-Line to 4-Line Decoders/Demultiplexers
         *
         *      +-----+-------++-----------------+
         *      | B A | G1 C1 || 1Y0 1Y1 1Y2 1Y3 |
         *      +=====+=======++=================+
         *      | X X | 1  X  ||  1   1   1   1  |
         *      | 0 0 | 0  1  ||  0   1   1   1  |
         *      | 0 1 | 0  1  ||  1   0   1   1  |
         *      | 1 0 | 0  1  ||  1   1   0   1  |
         *      | 1 1 | 0  1  ||  1   1   1   0  |
         *      | X X | X  0  ||  1   1   1   1  |
         *      +-----+-------++-----------------+
         *
         *      +-----+-------++-----------------+
         *      | B A | G2 C2 || 2Y0 2Y1 2Y2 2Y3 |
         *      +=====+=======++=================+
         *      | X X | 1  X  ||  1   1   1   1  |
         *      | 0 0 | 0  0  ||  0   1   1   1  |
         *      | 0 1 | 0  0  ||  1   0   1   1  |
         *      | 1 0 | 0  0  ||  1   1   0   1  |
         *      | 1 1 | 0  0  ||  1   1   1   0  |
         *      | X X | X  1  ||  1   1   1   1  |
         *      +-----+-------++-----------------+
         *
         * Naming conventions follow National Semiconductor datasheet
         *
         */

        //static NETLIST_START(TTL_74155_DIP)
        public static void netlist_TTL_74155_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_74155A_GATE", "A");
            netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_74155B_GATE", "B");

            netlist.nl_setup_global.NET_C(setup, "A.A", "B.A");
            netlist.nl_setup_global.NET_C(setup, "A.B", "B.B");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.C",  /*    C1 |1     ++    16| VCC  */ "A.VCC",
                "A.G",  /*    G1 |2           15| B4   */ "B.C",
                "A.B",  /*     B |3           14| B4   */ "B.G",
                "A.3",  /*   1Y3 |4   74155   13| A4   */ "B.A",
                "B.2",  /*   1Y2 |5           12| Y4   */ "B.3",
                "B.1",  /*   1Y1 |6           11| B3   */ "B.2",
                "B.0",  /*   1Y0 |7           10| A3   */ "B.1",
                "A.GND",/*   GND |8            9| Y3   */ "B.0"
                       /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(TTL_74156_DIP)
        public static void netlist_TTL_74156_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_74156A_GATE", "A");
            netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_74156B_GATE", "B");

            netlist.nl_setup_global.NET_C(setup, "A.A", "B.A");
            netlist.nl_setup_global.NET_C(setup, "A.B", "B.B");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.C",  /*    C1 |1     ++    16| VCC  */ "A.VCC",
                "A.G",  /*    G1 |2           15| B4   */ "B.C",
                "A.B",  /*     B |3           14| B4   */ "B.G",
                "A.3",  /*   1Y3 |4   74156   13| A4   */ "B.A",
                "B.2",  /*   1Y2 |5           12| Y4   */ "B.3",
                "B.1",  /*   1Y1 |6           11| B3   */ "B.2",
                "B.0",  /*   1Y0 |7           10| A3   */ "B.1",
                "A.GND",/*   GND |8            9| Y3   */ "B.0"
                       /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *  DM74260: Dual 5-Input NOR Gates
         *                 _________
         *             Y = A+B+C+D+E
         *          +---+---+---+---+---++---+
         *          | A | B | B | B | B || Y |
         *          +===+===+===+===+===++===+
         *          | 0 | 0 | 0 | 0 | 0 || 1 |
         *          | 0 | 0 | 0 | 0 | 1 || 0 |
         *          | 0 | 0 | 0 | 1 | 0 || 0 |
         *          | 0 | 0 | 1 | 0 | 0 || 0 |
         *          | 0 | 1 | 0 | 0 | 0 || 0 |
         *          | 1 | 0 | 0 | 0 | 0 || 0 |
         *          +---+---+---+---+---++---+
         *
         *  Naming conventions follow Texas Instruments datasheet
         *
         */

        //static NETLIST_START(TTL_74260_DIP)
        public static void netlist_TTL_74260_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_74260_GATE(setup, "A");
            TTL_74260_GATE(setup, "B");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND");

            netlist.nl_setup_global.DIPPINS(setup,   /*       +--------------+      */
                "A.C",  /*    C1 |1     ++    14| VCC  */ "A.VCC",
                "A.D",  /*    D1 |2           13| B1   */ "A.B",
                "A.E",  /*    E1 |3           12| A1   */ "A.A",
                "B.E",  /*    E2 |4   74260   11| D2   */ "B.D",
                "A.Q",  /*    Y1 |5           10| C2   */ "B.C",
                "B.Q",  /*    Y2 |6            9| B2   */ "B.B",
                "A.GND",/*   GND |7            8| A2   */ "B.A"
                        /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *  DM74279: Quad S-R Latch
         *
         *          +---+---+---++---+
         *          |S1 |S2 | R || Q |
         *          +===+===+===++===+
         *          | 0 | 0 | 0 || 1 |
         *          | 0 | 1 | 1 || 1 |
         *          | 1 | 0 | 1 || 1 |
         *          | 1 | 1 | 0 || 0 |
         *          | 1 | 1 | 1 ||QP |
         *          +---+---+---++---+
         *
         *  QP: Previous Q
         *
         *  Naming conventions follow Fairchild Semiconductor datasheet
         *
         */

        //#ifndef __PLIB_PREPROCESSOR__
        //#define TTL_74279A(name)                                                         \
        //        NET_REGISTER_DEV(TTL_74279A, name)
        //#define TTL_74279B(name)                                                         \
        //        NET_REGISTER_DEV(TTL_74279B, name)
        //#endif
        public static void TTL_74279A(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_74279A", name); }
        public static void TTL_74279B(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "TTL_74279B", name); }

        //static NETLIST_START(TTL_74279_DIP)
        public static void netlist_TTL_74279_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            TTL_74279B(setup, "A");
            TTL_74279A(setup, "B");
            TTL_74279B(setup, "C");
            TTL_74279A(setup, "D");

            netlist.nl_setup_global.NET_C(setup, "A.VCC", "B.VCC", "C.VCC", "D.VCC");
            netlist.nl_setup_global.NET_C(setup, "A.GND", "B.GND", "C.GND", "D.GND");

            netlist.nl_setup_global.DIPPINS(setup,    /*     +--------------+     */
                "A.R",   /*  1R |1     ++    16| VCC */ "A.VCC",
                "A.S1",  /* 1S1 |2           15| 4S  */ "D.S",
                "A.S2",  /* 1S2 |3           14| 4R  */ "D.R",
                "A.Q",   /*  1Q |4    74279  13| 4Q  */ "D.Q",
                "B.R",   /*  2R |5           12| 3S2 */ "C.S2",
                "B.S",   /*  2S |6           11| 3S1 */ "C.S1",
                "B.Q",   /*  2Q |7           10| 3R  */ "C.R",
                "A.GND", /* GND |8            9| 3Q  */ "C.Q"
                        /*     +--------------+     */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*
         *  DM9312: One of Eight Line Data Selectors/Multiplexers
         *
         *          +--------------+
         *       D0 |1     ++    16| VCC
         *       D1 |2           15| Y
         *       D2 |3           14| YQ
         *       D3 |4    9312   13| C
         *       D4 |5           12| B
         *       D5 |6           11| A
         *       D6 |7           10| G   Strobe
         *      GND |8            9| D7
         *          +--------------+
         *                  __
         *          +---+---+---+---++---+---+
         *          | C | B | A | G || Y | YQ|
         *          +===+===+===+===++===+===+
         *          | X | X | X | 1 ||  0| 1 |
         *          | 0 | 0 | 0 | 0 || D0|D0Q|
         *          | 0 | 0 | 1 | 0 || D1|D1Q|
         *          | 0 | 1 | 0 | 0 || D2|D2Q|
         *          | 0 | 1 | 1 | 0 || D3|D3Q|
         *          | 1 | 0 | 0 | 0 || D4|D4Q|
         *          | 1 | 0 | 1 | 0 || D5|D5Q|
         *          | 1 | 1 | 0 | 0 || D6|D6Q|
         *          | 1 | 1 | 1 | 0 || D7|D7Q|
         *          +---+---+---+---++---+---+
         *
         *  Naming conventions follow National Semiconductor datasheet
         *
         */

        //#ifndef __PLIB_PREPROCESSOR__
        //#define DM9312_TT(name)     \
        //        NET_REGISTER_DEV(DM9312_TT, name)
        //#endif
        public static void DM9312_TT(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "DM9312_TT", name); }

        //static NETLIST_START(DM9312_DIP)
        public static void netlist_DM9312_DIP(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            DM9312_TT(setup, "s");

            netlist.nl_setup_global.DIPPINS(setup,        /*     +--------------+     */
                "s.D0",   /*  D0 |1     ++    16| VCC */ "s.VCC",
                "s.D1",   /*  D1 |2           15| Y   */ "s.Y",
                "s.D2",   /*  D2 |3           14| YQ  */ "s.YQ",
                "s.D3",   /*  D3 |4    9312   13| C   */ "s.C",
                "s.D4",   /*  D4 |5           12| B   */ "s.B",
                "s.D5",   /*  D5 |6           11| A   */ "s.A",
                "s.D6",   /*  D6 |7           10| G   */ "s.G", //Strobe
                "s.GND",  /* GND |8            9| D7  */ "s.D7"
                        /*     +--------------+     */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //NETLIST_START(TTL74XX_lib)
        public static void netlist_TTL74XX_lib(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.NET_MODEL(setup, "DM7414         SCHMITT_TRIGGER(VTP=1.7 VTM=0.9 VI=4.35 RI=6.15k VOH=3.5 ROH=120 VOL=0.1 ROL=37.5 TPLH=15 TPHL=15)");
            netlist.nl_setup_global.NET_MODEL(setup, "TTL_7414_GATE  SCHMITT_TRIGGER(VTP=1.7 VTM=0.9 VI=4.35 RI=6.15k VOH=3.5 ROH=120 VOL=0.1 ROL=37.5 TPLH=15 TPHL=15)");
            netlist.nl_setup_global.NET_MODEL(setup, "DM74LS14       SCHMITT_TRIGGER(VTP=1.6 VTM=0.8 VI=4.4 RI=19.3k VOH=3.45 ROH=130 VOL=0.1 ROL=31.2 TPLH=15 TPHL=15)");
            //NET_MODEL("DM7414 FAMILY(FV=5 IVL=0.16 IVH=0.4 OVL=0.1 OVH=0.05 ORL=10.0 ORH=1.0e8)")


            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7400_GATE", 2, 1, "");
                netlist.nl_setup_global.TT_HEAD("A,B|Q ");
                netlist.nl_setup_global.TT_LINE("0,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,0|1|22");
                netlist.nl_setup_global.TT_LINE("1,1|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7400_NAND", 2, 1, "+A,+B,@VCC,@GND");
                netlist.nl_setup_global.TT_HEAD("A,B|Q ");
                netlist.nl_setup_global.TT_LINE("0,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,0|1|22");
                netlist.nl_setup_global.TT_LINE("1,1|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7402_GATE", 2, 1, "");
                netlist.nl_setup_global.TT_HEAD("A,B|Q ");
                netlist.nl_setup_global.TT_LINE("0,0|1|22");
                netlist.nl_setup_global.TT_LINE("X,1|0|15");
                netlist.nl_setup_global.TT_LINE("1,X|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7402_NOR", 2, 1, "+A,+B,@VCC,@GND");
                netlist.nl_setup_global.TT_HEAD("A,B|Q ");
                netlist.nl_setup_global.TT_LINE("0,0|1|22");
                netlist.nl_setup_global.TT_LINE("X,1|0|15");
                netlist.nl_setup_global.TT_LINE("1,X|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7404_GATE", 1, 1, "");
                netlist.nl_setup_global.TT_HEAD(" A | Q ");
                netlist.nl_setup_global.TT_LINE(" 0 | 1 |22");
                netlist.nl_setup_global.TT_LINE(" 1 | 0 |15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7404_INVERT", 1, 1, "+A,@VCC,@GND");
                netlist.nl_setup_global.TT_HEAD(" A | Q ");
                netlist.nl_setup_global.TT_LINE(" 0 | 1 |22");
                netlist.nl_setup_global.TT_LINE(" 1 | 0 |15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7408_GATE", 2, 1, "");
                netlist.nl_setup_global.TT_HEAD("A,B|Q ");
                netlist.nl_setup_global.TT_LINE("0,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,0|0|15");
                netlist.nl_setup_global.TT_LINE("1,1|1|22");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7408_AND", 2, 1, "+A,+B,@VCC,@GND");
                netlist.nl_setup_global.TT_HEAD("A,B|Q ");
                netlist.nl_setup_global.TT_LINE("0,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,0|0|15");
                netlist.nl_setup_global.TT_LINE("1,1|1|22");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7410_NAND", 3, 1, "+A,+B,+C,@VCC,@GND");
                netlist.nl_setup_global.TT_HEAD("A,B,C|Q ");
                netlist.nl_setup_global.TT_LINE("0,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,0,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,0|1|22");
                netlist.nl_setup_global.TT_LINE("1,1,1|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7410_GATE", 3, 1, "");
                netlist.nl_setup_global.TT_HEAD("A,B,C|Q ");
                netlist.nl_setup_global.TT_LINE("0,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,0,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,0|1|22");
                netlist.nl_setup_global.TT_LINE("1,1,1|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7411_AND", 3, 1, "+A,+B,+C,@VCC,@GND");
                netlist.nl_setup_global.TT_HEAD("A,B,C|Q ");
                netlist.nl_setup_global.TT_LINE("0,X,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,0,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,X,0|0|15");
                netlist.nl_setup_global.TT_LINE("1,1,1|1|22");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7411_GATE", 3, 1, "");
                netlist.nl_setup_global.TT_HEAD("A,B,C|Q ");
                netlist.nl_setup_global.TT_LINE("0,X,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,0,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,X,0|0|15");
                netlist.nl_setup_global.TT_LINE("1,1,1|1|22");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7416_GATE", 1, 1, "");
                netlist.nl_setup_global.TT_HEAD(" A | Q ");
                netlist.nl_setup_global.TT_LINE(" 0 | 1 |15");
                netlist.nl_setup_global.TT_LINE(" 1 | 0 |23");
                /* Open Collector */
                netlist.nl_setup_global.TT_FAMILY("74XXOC");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7420_GATE", 4, 1, "");
                netlist.nl_setup_global.TT_HEAD("A,B,C,D|Q ");
                netlist.nl_setup_global.TT_LINE("0,X,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,0,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,0,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,X,0|1|22");
                netlist.nl_setup_global.TT_LINE("1,1,1,1|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7420_NAND", 4, 1, "+A,+B,+C,+D,@VCC,@GND");
                netlist.nl_setup_global.TT_HEAD("A,B,C,D|Q ");
                netlist.nl_setup_global.TT_LINE("0,X,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,0,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,0,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,X,0|1|22");
                netlist.nl_setup_global.TT_LINE("1,1,1,1|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7425_GATE", 4, 1, "");
                netlist.nl_setup_global.TT_HEAD("A,B,C,D|Q ");
                netlist.nl_setup_global.TT_LINE("1,X,X,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,1,X,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,X,1,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,X,X,1|0|15");
                netlist.nl_setup_global.TT_LINE("0,0,0,0|1|22");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7425_NOR", 4, 1, "+A,+B,+C,+D,@VCC,@GND");
                netlist.nl_setup_global.TT_HEAD("A,B,C,D|Q ");
                netlist.nl_setup_global.TT_LINE("1,X,X,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,1,X,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,X,1,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,X,X,1|0|15");
                netlist.nl_setup_global.TT_LINE("0,0,0,0|1|22");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7427_GATE", 3, 1, "");
                netlist.nl_setup_global.TT_HEAD("A,B,C|Q ");
                netlist.nl_setup_global.TT_LINE("1,X,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,1,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,X,1|0|15");
                netlist.nl_setup_global.TT_LINE("0,0,0|1|22");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7427_NOR", 3, 1, "+A,+B,+C,@VCC,@GND");
                netlist.nl_setup_global.TT_HEAD("A,B,C|Q ");
                netlist.nl_setup_global.TT_LINE("1,X,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,1,X|0|15");
                netlist.nl_setup_global.TT_LINE("X,X,1|0|15");
                netlist.nl_setup_global.TT_LINE("0,0,0|1|22");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7430_GATE", 8, 1, "");
                netlist.nl_setup_global.TT_HEAD("A,B,C,D,E,F,G,H|Q ");
                netlist.nl_setup_global.TT_LINE("0,X,X,X,X,X,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,0,X,X,X,X,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,0,X,X,X,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,X,0,X,X,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,X,X,0,X,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,X,X,X,0,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,X,X,X,X,0,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,X,X,X,X,X,0|1|22");
                netlist.nl_setup_global.TT_LINE("1,1,1,1,1,1,1,1|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7430_NAND", 8, 1, "+A,+B,+C,+D,+E,+F,+G,+H,@VCC,@GND");
                netlist.nl_setup_global.TT_HEAD("A,B,C,D,E,F,G,H|Q ");
                netlist.nl_setup_global.TT_LINE("0,X,X,X,X,X,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,0,X,X,X,X,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,0,X,X,X,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,X,0,X,X,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,X,X,0,X,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,X,X,X,0,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,X,X,X,X,0,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,X,X,X,X,X,X,0|1|22");
                netlist.nl_setup_global.TT_LINE("1,1,1,1,1,1,1,1|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7432_GATE", 2, 1, "");
                netlist.nl_setup_global.TT_HEAD("A,B|Q ");
                netlist.nl_setup_global.TT_LINE("1,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,1|1|22");
                netlist.nl_setup_global.TT_LINE("0,0|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7432_OR", 2, 1, "+A,+B,@VCC,@GND");
                netlist.nl_setup_global.TT_HEAD("A,B|Q ");
                netlist.nl_setup_global.TT_LINE("1,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,1|1|22");
                netlist.nl_setup_global.TT_LINE("0,0|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            /*  FIXME: Same as 7400, but drains higher output currents.
             *         Netlist currently does not model over currents (should it ever?)
             */

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7437_GATE", 2, 1, "");
                netlist.nl_setup_global.TT_HEAD("A,B|Q ");
                netlist.nl_setup_global.TT_LINE("0,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,0|1|22");
                netlist.nl_setup_global.TT_LINE("1,1|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7437_NAND", 2, 1, "+A,+B");
                netlist.nl_setup_global.TT_HEAD("A,B|Q ");
                netlist.nl_setup_global.TT_LINE("0,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,0|1|22");
                netlist.nl_setup_global.TT_LINE("1,1|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7486_GATE", 2, 1, "");
                netlist.nl_setup_global.TT_HEAD("A,B|Q ");
                netlist.nl_setup_global.TT_LINE("0,0|0|15");
                netlist.nl_setup_global.TT_LINE("0,1|1|22");
                netlist.nl_setup_global.TT_LINE("1,0|1|22");
                netlist.nl_setup_global.TT_LINE("1,1|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_7486_XOR", 2, 1, "+A,+B,@VCC,@GND");
                netlist.nl_setup_global.TT_HEAD("A,B|Q ");
                netlist.nl_setup_global.TT_LINE("0,0|0|15");
                netlist.nl_setup_global.TT_LINE("0,1|1|22");
                netlist.nl_setup_global.TT_LINE("1,0|1|22");
                netlist.nl_setup_global.TT_LINE("1,1|0|15");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_74155A_GATE", 4, 4, "");
                netlist.nl_setup_global.TT_HEAD("B,A,G,C|0,1,2,3");
                netlist.nl_setup_global.TT_LINE("X,X,1,X|1,1,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("X,X,0,0|1,1,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("0,0,0,1|0,1,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("0,1,0,1|1,0,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("1,0,0,1|1,1,0,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("1,1,0,1|1,1,1,0|13,13,13,13");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_74155B_GATE", 4, 4, "");
                netlist.nl_setup_global.TT_HEAD("B,A,G,C|0,1,2,3");
                netlist.nl_setup_global.TT_LINE("X,X,1,X|1,1,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("X,X,0,1|1,1,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("0,0,0,0|0,1,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("0,1,0,0|1,0,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("1,0,0,0|1,1,0,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("1,1,0,0|1,1,1,0|13,13,13,13");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_74156A_GATE", 4, 4, "");
                netlist.nl_setup_global.TT_HEAD("B,A,G,C|0,1,2,3");
                netlist.nl_setup_global.TT_LINE("X,X,1,X|1,1,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("X,X,0,0|1,1,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("0,0,0,1|0,1,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("0,1,0,1|1,0,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("1,0,0,1|1,1,0,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("1,1,0,1|1,1,1,0|13,13,13,13");
                netlist.nl_setup_global.TT_FAMILY("74XXOC");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_74156B_GATE", 4, 4, "");
                netlist.nl_setup_global.TT_HEAD("B,A,G,C|0,1,2,3");
                netlist.nl_setup_global.TT_LINE("X,X,1,X|1,1,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("X,X,0,1|1,1,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("0,0,0,0|0,1,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("0,1,0,0|1,0,1,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("1,0,0,0|1,1,0,1|13,13,13,13");
                netlist.nl_setup_global.TT_LINE("1,1,0,0|1,1,1,0|13,13,13,13");
                netlist.nl_setup_global.TT_FAMILY("74XXOC");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_74260_GATE", 5, 1, "");
                netlist.nl_setup_global.TT_HEAD("A,B,C,D,E|Q ");
                netlist.nl_setup_global.TT_LINE("0,0,0,0,0|1|10");
                netlist.nl_setup_global.TT_LINE("X,X,X,X,1|0|12");
                netlist.nl_setup_global.TT_LINE("X,X,X,1,X|0|12");
                netlist.nl_setup_global.TT_LINE("X,X,1,X,X|0|12");
                netlist.nl_setup_global.TT_LINE("X,1,X,X,X|0|12");
                netlist.nl_setup_global.TT_LINE("1,X,X,X,X|0|12");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_74260_NOR", 5, 1, "+A,+B,+C,+D,+E,@VCC,@GND");
                netlist.nl_setup_global.TT_HEAD("A,B,C,D,E|Q ");
                netlist.nl_setup_global.TT_LINE("0,0,0,0,0|1|10");
                netlist.nl_setup_global.TT_LINE("X,X,X,X,1|0|12");
                netlist.nl_setup_global.TT_LINE("X,X,X,1,X|0|12");
                netlist.nl_setup_global.TT_LINE("X,X,1,X,X|0|12");
                netlist.nl_setup_global.TT_LINE("X,1,X,X,X|0|12");
                netlist.nl_setup_global.TT_LINE("1,X,X,X,X|0|12");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            // FIXME: We need "private" devices
            netlist.nl_setup_global.TRUTHTABLE_START("TTL_74279A", 3, 1, "");
                netlist.nl_setup_global.TT_HEAD("S,R,_Q|Q");
                netlist.nl_setup_global.TT_LINE("0,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("1,0,X|0|27");
                netlist.nl_setup_global.TT_LINE("1,1,0|0|27");
                netlist.nl_setup_global.TT_LINE("1,1,1|1|22");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("TTL_74279B", 4, 1, "");
                netlist.nl_setup_global.TT_HEAD("S1,S2,R,_Q|Q");
                netlist.nl_setup_global.TT_LINE("0,X,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("X,0,X,X|1|22");
                netlist.nl_setup_global.TT_LINE("1,1,0,X|0|27");
                netlist.nl_setup_global.TT_LINE("1,1,1,0|0|27");
                netlist.nl_setup_global.TT_LINE("1,1,1,1|1|22");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.TRUTHTABLE_START("DM9312_TT", 12, 2, "+A,+B,+C,+G,+D0,+D1,+D2,+D3,+D4,+D5,+D6,+D7,@VCC,@GND");
                netlist.nl_setup_global.TT_HEAD(" C, B, A, G,D0,D1,D2,D3,D4,D5,D6,D7| Y,YQ");
                netlist.nl_setup_global.TT_LINE(" X, X, X, 1, X, X, X, X, X, X, X, X| 0, 1|33,19");
                netlist.nl_setup_global.TT_LINE(" 0, 0, 0, 0, 0, X, X, X, X, X, X, X| 0, 1|33,28");
                netlist.nl_setup_global.TT_LINE(" 0, 0, 0, 0, 1, X, X, X, X, X, X, X| 1, 0|33,28");
                netlist.nl_setup_global.TT_LINE(" 0, 0, 1, 0, X, 0, X, X, X, X, X, X| 0, 1|33,28");
                netlist.nl_setup_global.TT_LINE(" 0, 0, 1, 0, X, 1, X, X, X, X, X, X| 1, 0|33,28");
                netlist.nl_setup_global.TT_LINE(" 0, 1, 0, 0, X, X, 0, X, X, X, X, X| 0, 1|33,28");
                netlist.nl_setup_global.TT_LINE(" 0, 1, 0, 0, X, X, 1, X, X, X, X, X| 1, 0|33,28");
                netlist.nl_setup_global.TT_LINE(" 0, 1, 1, 0, X, X, X, 0, X, X, X, X| 0, 1|33,28");
                netlist.nl_setup_global.TT_LINE(" 0, 1, 1, 0, X, X, X, 1, X, X, X, X| 1, 0|33,28");
                netlist.nl_setup_global.TT_LINE(" 1, 0, 0, 0, X, X, X, X, 0, X, X, X| 0, 1|33,28");
                netlist.nl_setup_global.TT_LINE(" 1, 0, 0, 0, X, X, X, X, 1, X, X, X| 1, 0|33,28");
                netlist.nl_setup_global.TT_LINE(" 1, 0, 1, 0, X, X, X, X, X, 0, X, X| 0, 1|33,28");
                netlist.nl_setup_global.TT_LINE(" 1, 0, 1, 0, X, X, X, X, X, 1, X, X| 1, 0|33,28");
                netlist.nl_setup_global.TT_LINE(" 1, 1, 0, 0, X, X, X, X, X, X, 0, X| 0, 1|33,28");
                netlist.nl_setup_global.TT_LINE(" 1, 1, 0, 0, X, X, X, X, X, X, 1, X| 1, 0|33,28");
                netlist.nl_setup_global.TT_LINE(" 1, 1, 1, 0, X, X, X, X, X, X, X, 0| 0, 1|33,28");
                netlist.nl_setup_global.TT_LINE(" 1, 1, 1, 0, X, X, X, X, X, X, X, 1| 1, 0|33,28");
                netlist.nl_setup_global.TT_FAMILY("74XX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7400_DIP", netlist_TTL_7400_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7402_DIP", netlist_TTL_7402_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7404_DIP", netlist_TTL_7404_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7408_DIP", netlist_TTL_7408_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7410_DIP", netlist_TTL_7410_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7411_DIP", netlist_TTL_7411_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7414_GATE", netlist_TTL_7414_GATE);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_74LS14_GATE", netlist_TTL_74LS14_GATE);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7414_DIP", netlist_TTL_7414_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_74LS14_DIP", netlist_TTL_74LS14_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7416_DIP", netlist_TTL_7416_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7420_DIP", netlist_TTL_7420_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7425_DIP", netlist_TTL_7425_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7427_DIP", netlist_TTL_7427_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7430_DIP", netlist_TTL_7430_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7432_DIP", netlist_TTL_7432_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7437_DIP", netlist_TTL_7437_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_7486_DIP", netlist_TTL_7486_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_74155_DIP", netlist_TTL_74155_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_74156_DIP", netlist_TTL_74156_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_74260_DIP", netlist_TTL_74260_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_74279_DIP", netlist_TTL_74279_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "DM9312_DIP", netlist_DM9312_DIP);

            netlist.nl_setup_global.NETLIST_END();
        }
    }
}
