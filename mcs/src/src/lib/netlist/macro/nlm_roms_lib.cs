// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nlm_roms_global
    {
        /* ----------------------------------------------------------------------------
         *  External declarations
         * ---------------------------------------------------------------------------*/
        //NETLIST_EXTERNAL(ROMS_lib)


        //- Identifier:  PROM_82S126_DIP
        //- Title: 82S126 1K-bit TTL bipolar PROM
        //- Pinalias: A6,A5,A4,A3,A0,A1,A2,GND,O4,O3,O2,O1,CE1Q,CE2Q,A7,VCC
        //- Package: DIP
        //- Param: ROM
        //-    The name of the source to load the rom content from
        //- Param: FORCE_TRISTATE_LOGIC
        //-    Set this parameter to 1 force tristate outputs into logic mode.
        //-    This should be done only if the device enable inputs are connected
        //-    in a way which always enables the device.
        //- Param: MODEL
        //-    Overwrite the default model of the device. Use with care.
        //- NamingConvention: Naming conventions follow Philips Components-Signetics datasheet
        //- Limitations:
        //-    Currently OC is not supported.
        //-
        //- Example: 82S126.cpp,82S126_example
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheet/philips/82S129.pdf
        //-

        //static NETLIST_START(PROM_82S126_DIP)
        public static void netlist_PROM_82S126_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            PROM_82S126(setup, "A");
#endif

            h.DEFPARAM("ROM", "unknown");
            h.DEFPARAM("FORCE_TRISTATE_LOGIC", "0");
            h.DEFPARAM("MODEL", "$(@.A.MODEL)");
            h.PARAM("A.ROM", "$(@.ROM)");
            h.PARAM("A.FORCE_TRISTATE_LOGIC", "$(@.FORCE_TRISTATE_LOGIC)");
            h.PARAM("A.MODEL", "$(@.MODEL)");
            h.ALIAS("1", "A.A6");
            h.ALIAS("2", "A.A5");
            h.ALIAS("3", "A.A4");
            h.ALIAS("4", "A.A3");
            h.ALIAS("5", "A.A0");
            h.ALIAS("6", "A.A1");
            h.ALIAS("7", "A.A2");
            h.ALIAS("8", "A.GND");
            h.ALIAS("9", "A.O4");
            h.ALIAS("10", "A.O3");
            h.ALIAS("11", "A.O2");
            h.ALIAS("12", "A.O1");
            h.ALIAS("13", "A.CE1Q");
            h.ALIAS("14", "A.CE2Q");
            h.ALIAS("15", "A.A7");
            h.ALIAS("16", "A.VCC");

            h.NETLIST_END();
        }


        //- Identifier:  PROM_74S287_DIP
        //- Title: 74S287 (256 x 4) 1024-Bit TTL PROM
        //- Pinalias: A6,A5,A4,A3,A0,A1,A2,GND,O3,O2,O1,O0,CE1Q,CE2Q,A7,VCC
        //- Package: DIP
        //- Param: ROM
        //-    The name of the source to load the rom content from
        //- Param: FORCE_TRISTATE_LOGIC
        //-    Set this parameter to 1 force tristate outputs into logic mode.
        //-    This should be done only if the device enable inputs are connected
        //-    in a way which always enables the device.
        //- Param: MODEL
        //-    Overwrite the default model of the device. Use with care.
        //- NamingConvention: Naming conventions follow National Semiconductor datasheet
        //- Limitations:
        //-    None.
        //-
        //- Example: 74S287.cpp,74S287_example
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheet_pdf/national-semiconductor/DM54S287AJ_to_DM74S287V.pdf
        //-

        //static NETLIST_START(PROM_74S287_DIP)
        public static void netlist_PROM_74S287_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            PROM_74S287(setup, "A");
#endif

            h.DEFPARAM("ROM", "unknown");
            h.DEFPARAM("FORCE_TRISTATE_LOGIC", "0");
            h.DEFPARAM("MODEL", "$(@.A.MODEL)");
            h.PARAM("A.ROM", "$(@.ROM)");
            h.PARAM("A.FORCE_TRISTATE_LOGIC", "$(@.FORCE_TRISTATE_LOGIC)");
            h.PARAM("A.MODEL", "$(@.MODEL)");
            h.ALIAS("1", "A.A6");
            h.ALIAS("2", "A.A5");
            h.ALIAS("3", "A.A4");
            h.ALIAS("4", "A.A3");
            h.ALIAS("5", "A.A0");
            h.ALIAS("6", "A.A1");
            h.ALIAS("7", "A.A2");
            h.ALIAS("8", "A.GND");
            h.ALIAS("9", "A.O3");
            h.ALIAS("10", "A.O2");
            h.ALIAS("11", "A.O1");
            h.ALIAS("12", "A.O0");
            h.ALIAS("13", "A.CE1Q");
            h.ALIAS("14", "A.CE2Q");
            h.ALIAS("15", "A.A7");
            h.ALIAS("16", "A.VCC");

            h.NETLIST_END();
        }


        //- Identifier:  PROM_82S123_DIP
        //- Title: 82S123 256 bit TTL bipolar PROM
        //- Pinalias: O1,O2,O3,O4,O5,O6,O7,GND,O8,A0,A1,A2,A3,A4,CEQ,VCC
        //- Package: DIP
        //- Param: ROM
        //-    The name of the source to load the rom content from
        //- Param: FORCE_TRISTATE_LOGIC
        //-    Set this parameter to 1 force tristate outputs into logic mode.
        //-    This should be done only if the device enable inputs are connected
        //-    in a way which always enables the device.
        //- Param: MODEL
        //-    Overwrite the default model of the device. Use with care.
        //- NamingConvention: Naming conventions follow Philips Components-Signetics datasheet
        //- Limitations:
        //-    Currently OC is not supported.
        //-
        //- Example: 82S123.cpp,82S123_example
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheet/philips/82S123.pdf
        //-

        //static NETLIST_START(PROM_82S123_DIP)
        public static void netlist_PROM_82S123_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            PROM_82S123(setup, "A");
#endif

            h.DEFPARAM("ROM", "unknown");
            h.DEFPARAM("FORCE_TRISTATE_LOGIC", "0");
            h.DEFPARAM("MODEL", "$(@.A.MODEL)");
            h.PARAM("A.ROM", "$(@.ROM)");
            h.PARAM("A.FORCE_TRISTATE_LOGIC", "$(@.FORCE_TRISTATE_LOGIC)");
            h.PARAM("A.MODEL", "$(@.MODEL)");
            h.ALIAS("1", "A.O0");
            h.ALIAS("2", "A.O1");
            h.ALIAS("3", "A.O2");
            h.ALIAS("4", "A.O3");
            h.ALIAS("5", "A.O4");
            h.ALIAS("6", "A.O5");
            h.ALIAS("7", "A.O6");
            h.ALIAS("8", "A.GND");

            h.ALIAS("9", "A.O7");
            h.ALIAS("10", "A.A0");
            h.ALIAS("11", "A.A1");
            h.ALIAS("12", "A.A2");
            h.ALIAS("13", "A.A3");
            h.ALIAS("14", "A.A4");
            h.ALIAS("15", "A.CEQ");
            h.ALIAS("16", "A.VCC");

            h.NETLIST_END();
        }


        //- Identifier:  EPROM_2716_DIP
        //- Title: 2716 16K (2K x 8) UV ERASABLE PROM
        //- Pinalias: A7,A6,A6,A4,A4,A2,A1,A0,O0,O1,O2,GND,O3,O4,O5,O6,O7,CE1Q/CE,A10,CE2Q/OE,VPP,A9,A8,VCC
        //- Package: DIP
        //- Param: ROM
        //-    The name of the source to load the rom content from
        //- Param: FORCE_TRISTATE_LOGIC
        //-    Set this parameter to 1 force tristate outputs into logic mode.
        //-    This should be done only if the device enable inputs are connected
        //-    in a way which always enables the device.
        //- Param: MODEL
        //-    Overwrite the default model of the device. Use with care.
        //- NamingConvention: Naming conventions follow Intel datasheet
        //- Limitations:
        //-    Currently OC is not supported.
        //-
        //- Example: 2716.cpp,2716_example
        //- FunctionTable:
        //-    http://pdf.datasheetcatalog.com/datasheets/400/500340_DS.pdf
        //-
        //static NETLIST_START(EPROM_2716_DIP)
        public static void netlist_EPROM_2716_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            EPROM_2716(setup, "A");
#endif

            h.DEFPARAM("ROM", "unknown");
            h.DEFPARAM("FORCE_TRISTATE_LOGIC", "0");
            h.DEFPARAM("MODEL", "$(@.A.MODEL)");
            h.PARAM("A.ROM", "$(@.ROM)");
            h.PARAM("A.FORCE_TRISTATE_LOGIC", "$(@.FORCE_TRISTATE_LOGIC)");
            h.PARAM("A.MODEL", "$(@.MODEL)");
            h.ALIAS("1", "A.A7");
            h.ALIAS("2", "A.A6");
            h.ALIAS("3", "A.A5");
            h.ALIAS("4", "A.A4");
            h.ALIAS("5", "A.A3");
            h.ALIAS("6", "A.A2");
            h.ALIAS("7", "A.A1");
            h.ALIAS("8", "A.A0");
            throw new emu_unimplemented();
#if false
            ALIAS(9, A.O0);
            ALIAS(10, A.O1);
            ALIAS(11, A.O2);
            netlist.nl_setup_global.ALIAS(setup, "12", "A.GND");

            ALIAS(13, A.O3);
            ALIAS(14, A.O4);
            ALIAS(15, A.O5);
            ALIAS(16, A.O6);
            ALIAS(17, A.O7);
#endif
            h.ALIAS("18", "A.CE1Q"); // CEQ
            h.ALIAS("19", "A.A10");
            h.ALIAS("20", "A.CE2Q"); // OEQ
            h.ALIAS("22", "A.A9");
            h.ALIAS("23", "A.A8");
            h.ALIAS("24", "A.VCC");

            h.NETLIST_END();
        }


        /*  DM82S16: 256 Bit bipolar ram
         *
         *          +--------------+
         *       A1 |1     ++    16| VCC
         *       A0 |2           15| A2
         *     CE1Q |3           14| A3
         *     CE2Q |4   82S16   13| DIN
         *     CE3Q |5           12| WEQ
         *    DOUTQ |6           11| A7
         *       A4 |7           10| A6
         *      GND |8            9| A5
         *          +--------------+
         *
         *  Naming conventions follow Signetics datasheet
         */
        //static NETLIST_START(TTL_82S16_DIP)
        public static void netlist_TTL_82S16_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            TTL_82S16(A);
#endif

            h.DIPPINS(     /*        +--------------+       */
                   "A.A1", /*     A1 |1     ++    16| VCC   */ "A.VCC",
                   "A.A0", /*     A0 |2           15| A2    */ "A.A2",
                 "A.CE1Q", /*   CE1Q |3           14| A3    */ "A.A3",
                 "A.CE2Q", /*   CE2Q |4   82S16   13| DIN   */ "A.DIN",
                 "A.CE3Q", /*   CE3Q |5           12| WEQ   */ "A.WEQ",
                "A.DOUTQ", /*  DOUTQ |6           11| A7    */ "A.A7",
                   "A.A4", /*     A4 |7           10| A6    */ "A.A6",
                  "A.GND", /*    GND |8            9| A5    */ "A.A5"
                           /*        +--------------+       */
            );

            h.NETLIST_END();
        }


        /*  82S115: 4K-bit TTL bipolar PROM (512 x 8)
         *
         *          +--------------+
         *       A3 |1     ++    24| VCC
         *       A4 |2           23| A2
         *       A5 |3           22| A1
         *       A6 |4   82S115  21| A0
         *       A7 |5           20| CE1Q
         *       A8 |6           19| CE2
         *       O1 |7           18| STROBE
         *       O2 |8           17| O8
         *       O3 |9           16| O7
         *       O4 |10          15| O6
         *      FE2 |11          14| O5
         *      GND |12          13| FE1
         *          +--------------+
         */
        //static NETLIST_START(PROM_82S115_DIP)
        public static void netlist_PROM_82S115_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            PROM_82S115(A);
            NC_PIN(setup, "NC");
#endif

            h.DIPPINS(   /*      +--------------+        */
                 "A.A3", /*   A3 |1     ++    24| VCC    */ "A.VCC",
                 "A.A4", /*   A4 |2           23| A2     */ "A.A2",
                 "A.A5", /*   A5 |3           22| A1     */ "A.A1",
                 "A.A6", /*   A6 |4   82S115  21| A0     */ "A.A0",
                 "A.A7", /*   A7 |5           20| CE1Q   */ "A.CE1Q",
                 "A.A8", /*   A8 |6           19| CE2    */ "A.CE2",
                 "A.O1", /*   O1 |7           18| STROBE */ "A.STROBE",
                 "A.O2", /*   O2 |8           17| O8     */ "A.O8",
                 "A.O3", /*   O3 |9           16| O7     */ "A.O7",
                 "A.O4", /*   O4 |10          15| O6     */ "A.O6",
                 "NC.I", /*  FE2 |11          14| O5     */ "A.O5",
                "A.GND", /*  GND |12          13| FE1    */ "NC.I"
                         /*      +--------------+        */
            );

            h.NETLIST_END();
        }


        //- Identifier:  PROM_MK28000_DIP
        //- Title: MK28000 (2048 x 8 or 4096 x 4) 16384-Bit TTL PROM
        //- Pinalias: VCC,A1,A2,A3,A4,A5,A6,A10,GND,A9,A8,A7,ARQ,OE2,A11,O8,O7,O6,O5,O4,O3,O2,O1,OE1
        //- Package: DIP
        //- Param: ROM
        //-    The name of the source to load the rom content from
        //- NamingConvention: Naming conventions follow Mostek datasheet
        //- Limitations:
        //-    None.
        //static NETLIST_START(PROM_MK28000_DIP)
        public static void netlist_PROM_MK28000_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            PROM_MK28000(A);
#endif

            h.DEFPARAM("ROM", "unknown");
            h.PARAM("A.ROM", "$(@.ROM)");
            h.ALIAS("1", "A.VCC");
            h.ALIAS("2", "A.A1");
            h.ALIAS("3", "A.A2");
            h.ALIAS("4", "A.A3");
            h.ALIAS("5", "A.A4");
            h.ALIAS("6", "A.A5");
            h.ALIAS("7", "A.A6");
            h.ALIAS("8", "A.A10");
            h.ALIAS("9", "A.GND");
            h.ALIAS("10", "A.A9");
            h.ALIAS("11", "A.A8");
            h.ALIAS("12", "A.A7");
            h.ALIAS("13", "A.ARQ");
            h.ALIAS("14", "A.OE2");
            h.ALIAS("15", "A.A11");
            h.ALIAS("16", "A.O8");
            h.ALIAS("17", "A.O7");
            h.ALIAS("18", "A.O6");
            h.ALIAS("19", "A.O5");
            h.ALIAS("20", "A.O4");
            h.ALIAS("21", "A.O3");
            h.ALIAS("22", "A.O2");
            h.ALIAS("23", "A.O1");
            h.ALIAS("24", "A.OE1");

            h.NETLIST_END();
        }


        //static NETLIST_START(ROM_MCM14524_DIP)
        public static void netlist_ROM_MCM14524_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            ROM_MCM14524(A)

            DEFPARAM(ROM, "unknown")
            PARAM(A.ROM, "$(@.ROM)")

                     /* Motorola MCM14524:      */
            DIPPINS( /*      +-----..-----+     */
              A.CLK, /* /CLK |1         16| VDD */ A.VCC,
               A.EN, /*   CE |2         15| A0  */ A.A0,
               A.B0, /*   B0 |3   MCM   14| A1  */ A.A1,
               A.B1, /*   B1 |4  14524  13| A7  */ A.A7,
               A.B2, /*   B2 |5         12| A6  */ A.A6,
               A.B3, /*   B3 |6         11| A5  */ A.A5,
               A.A2, /*   A2 |7         10| A4  */ A.A4,
              A.GND, /*  VSS |8          9| A3  */ A.A3
                     /*      +------------+ */
            )
#endif

            h.NETLIST_END();
        }


        /*  2102: 1024 x 1-bit Static RAM
         *
         *          +--------------+
         *       A6 |1     ++    16| A7
         *       A5 |2           15| A8
         *      RWQ |3           14| A9
         *       A1 |4   82S16   13| CEQ
         *       A2 |5           12| DO
         *       A3 |6           11| DI
         *       A4 |7           10| VCC
         *       A0 |8            9| GND
         *          +--------------+
         */
         //static NETLIST_START(RAM_2102A_DIP)
        public static void netlist_RAM_2102A_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            RAM_2102A(A);
#endif

            h.DIPPINS(   /*      +--------------+      */
                 "A.A6", /*   A6 |1     ++    16| A7   */ "A.A7",
                 "A.A5", /*   A5 |2           15| A8   */ "A.A8",
                "A.RWQ", /*  RWQ |3           14| A9   */ "A.A9",
                 "A.A1", /*   A1 |4   82S16   13| CEQ  */ "A.CEQ",
                 "A.A2", /*   A2 |5           12| DO   */ "A.DO",
                 "A.A3", /*   A3 |6           11| DI   */ "A.DI",
                 "A.A4", /*   A4 |7           10| VCC  */ "A.VCC",
                 "A.A0", /*   A0 |8            9| GND  */ "A.GND"
                         /*      +--------------+      */
            );

            h.NETLIST_END();
        }


        //static NETLIST_START(ROM_TMS4800_DIP)
        public static void netlist_ROM_TMS4800_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            ROM_TMS4800(A)

            DIPPINS(   /*       +----------------+     */
                A.VSS, /*   VSS |1      ++     24| OE1 */ A.OE1,
                A.A1,  /*    A1 |2             23| O1  */ A.O1,
                A.A2,  /*    A2 |3             22| O2  */ A.O2,
                A.A3,  /*    A3 |4   TMS-4800  21| O3  */ A.O3,
                A.A4,  /*    A4 |5             20| O4  */ A.O4,
                A.A5,  /*    A5 |6             19| O5  */ A.O5,
                A.A6,  /*    A6 |7             18| O6  */ A.O6,
                A.A10, /*   A10 |8             17| O7  */ A.O7,
                A.VGG, /*   VGG |9             16| O8  */ A.O8,
                A.A9,  /*    A9 |10            15| A11 */ A.A11,
                A.A8,  /*    A8 |11            14| OE2 */ A.OE2,
                A.A7,  /*    A7 |12            13| AR  */ A.AR
                       /*       +----------------+      */
            )
#endif

            h.NETLIST_END();
        }


        //NETLIST_START(roms_lib)
        public static void netlist_roms_lib(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.LOCAL_LIB_ENTRY("PROM_82S123_DIP", netlist_PROM_82S123_DIP);
            h.LOCAL_LIB_ENTRY("PROM_82S126_DIP", netlist_PROM_82S126_DIP);
            h.LOCAL_LIB_ENTRY("PROM_74S287_DIP", netlist_PROM_74S287_DIP);
            h.LOCAL_LIB_ENTRY("EPROM_2716_DIP", netlist_EPROM_2716_DIP);
            h.LOCAL_LIB_ENTRY("TTL_82S16_DIP", netlist_TTL_82S16_DIP);
            h.LOCAL_LIB_ENTRY("PROM_82S115_DIP", netlist_PROM_82S115_DIP);
            h.LOCAL_LIB_ENTRY("PROM_MK28000_DIP", netlist_PROM_MK28000_DIP);
            h.LOCAL_LIB_ENTRY("ROM_MCM14524_DIP", netlist_ROM_MCM14524_DIP);
            h.LOCAL_LIB_ENTRY("RAM_2102A_DIP", netlist_RAM_2102A_DIP);
            h.LOCAL_LIB_ENTRY("ROM_TMS4800_DIP", netlist_ROM_TMS4800_DIP);

            h.NETLIST_END();
        }
    }
}
