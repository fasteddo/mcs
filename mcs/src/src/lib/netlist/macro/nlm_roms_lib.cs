// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nlm_roms_global
    {
        /* ----------------------------------------------------------------------------
         *  Netlist Macros
         * ---------------------------------------------------------------------------*/

#if !NL_AUTO_DEVICES

        //#define PROM_82S115_DIP(name)                                                  \
        //        NET_REGISTER_DEV(PROM_82S115_DIP, name)

        //#define PROM_82S126_DIP(name)                                                  \
        //        NET_REGISTER_DEV(PROM_82S126_DIP, name)
        public static void PROM_82S126_DIP(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "PROM_82S126_DIP", name); }

        //#define PROM_82S123_DIP(name)                                                  \
        //        NET_REGISTER_DEV(PROM_82S123_DIP, name)
        public static void PROM_82S123_DIP(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "PROM_82S123_DIP", name); }

        //#define PROM_74S287_DIP(name)                                                  \
        //        NET_REGISTER_DEV(PROM_74S287_DIP, name)

        //#define PROM_MK28000_DIP(name)                                                 \
        //        NET_REGISTER_DEV(PROM_MK28000_DIP, name)

        //#define EPROM_2716_DIP(name)                                                   \
        //        NET_REGISTER_DEV(EPROM_2716_DIP, name)
        public static void EPROM_2716_DIP(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "EPROM_2716_DIP", name); }

        // usage       : TTL_82S16_DIP(name)
        //#define TTL_82S16_DIP(...)                                                     \
        //    NET_REGISTER_DEVEXT(TTL_82S16_DIP, __VA_ARGS__)

        //#define RAM_2102A_DIP(name)                                                    \
        //        NET_REGISTER_DEV(RAM_2102A_DIP, name)

#endif // NL_AUTO_DEVICES


        /* ----------------------------------------------------------------------------
         *  External declarations
         * ---------------------------------------------------------------------------*/
        //NETLIST_EXTERNAL(ROMS_lib)


        //- Identifier:  PROM_82S126_DIP
        //- Title: 82S126 1K-bit TTL bipolar PROM
        //- Description: The 82S126 and 82S129 are field programmable, which means that
        //-    custom patterns are immediately available by following the Signetics
        //-    Generic I fusing procedure. The 82S126 and 82S129 devices are supplied
        //-    with all outputs at logical Low. Outputs are programmed to a logic High
        //-    level at any specified address by fusing the Ni-Cr link matrix.
        //-
        //-    These devices include on-chip decoding and 2 Chip Enable inputs for ease
        //-    of memory expansion. They feature either open collector or 3-State outputs
        //-    for optimization of word expansion in bused organizations.
        //-
        //.
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
            netlist.nl_setup_global.NETLIST_START();

            netlist.nld_roms_global.PROM_82S126(setup, "A");

            netlist.nl_setup_global.DEFPARAM(setup, "ROM", "unknown");
            netlist.nl_setup_global.DEFPARAM(setup, "FORCE_TRISTATE_LOGIC", "0");
            netlist.nl_setup_global.DEFPARAM(setup, "MODEL", "$(@.A.MODEL)");
            netlist.nl_setup_global.PARAM(setup, "A.ROM", "$(@.ROM)");
            netlist.nl_setup_global.PARAM(setup, "A.FORCE_TRISTATE_LOGIC", "$(@.FORCE_TRISTATE_LOGIC)");
            netlist.nl_setup_global.PARAM(setup, "A.MODEL", "$(@.MODEL)");
            netlist.nl_setup_global.ALIAS(setup, "1", "A.A6");
            netlist.nl_setup_global.ALIAS(setup, "2", "A.A5");
            netlist.nl_setup_global.ALIAS(setup, "3", "A.A4");
            netlist.nl_setup_global.ALIAS(setup, "4", "A.A3");
            netlist.nl_setup_global.ALIAS(setup, "5", "A.A0");
            netlist.nl_setup_global.ALIAS(setup, "6", "A.A1");
            netlist.nl_setup_global.ALIAS(setup, "7", "A.A2");
            netlist.nl_setup_global.ALIAS(setup, "8", "A.GND");
            netlist.nl_setup_global.ALIAS(setup, "9", "A.O4");
            netlist.nl_setup_global.ALIAS(setup, "10", "A.O3");
            netlist.nl_setup_global.ALIAS(setup, "11", "A.O2");
            netlist.nl_setup_global.ALIAS(setup, "12", "A.O1");
            netlist.nl_setup_global.ALIAS(setup, "13", "A.CE1Q");
            netlist.nl_setup_global.ALIAS(setup, "14", "A.CE2Q");
            netlist.nl_setup_global.ALIAS(setup, "15", "A.A7");
            netlist.nl_setup_global.ALIAS(setup, "16", "A.VCC");

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier:  PROM_74S287_DIP
        //- Title: 74S287 (256 x 4) 1024-Bit TTL PROM
        //- Description: This Schottky memory is organized in the popular 256 words by
        //-    4 bits configuration. Memory enable inputs are provided to control the
        //-    output states. When the device is enabled, the outputs represent the
        //-    contents of the selected word. When disabled, the 4 outputs go to the
        //-    or high impedance state.
        //-
        //-    PROMs are shipped from the factory with lows in all locations. A high
        //-    may be programmed into any selected location by following the
        //-    programming instructions.
        //-
        //.
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
            netlist.nl_setup_global.NETLIST_START();

            netlist.nld_roms_global.PROM_74S287(setup, "A");

            netlist.nl_setup_global.DEFPARAM(setup, "ROM", "unknown");
            netlist.nl_setup_global.DEFPARAM(setup, "FORCE_TRISTATE_LOGIC", "0");
            netlist.nl_setup_global.DEFPARAM(setup, "MODEL", "$(@.A.MODEL)");
            netlist.nl_setup_global.PARAM(setup, "A.ROM", "$(@.ROM)");
            netlist.nl_setup_global.PARAM(setup, "A.FORCE_TRISTATE_LOGIC", "$(@.FORCE_TRISTATE_LOGIC)");
            netlist.nl_setup_global.PARAM(setup, "A.MODEL", "$(@.MODEL)");
            netlist.nl_setup_global.ALIAS(setup, "1", "A.A6");
            netlist.nl_setup_global.ALIAS(setup, "2", "A.A5");
            netlist.nl_setup_global.ALIAS(setup, "3", "A.A4");
            netlist.nl_setup_global.ALIAS(setup, "4", "A.A3");
            netlist.nl_setup_global.ALIAS(setup, "5", "A.A0");
            netlist.nl_setup_global.ALIAS(setup, "6", "A.A1");
            netlist.nl_setup_global.ALIAS(setup, "7", "A.A2");
            netlist.nl_setup_global.ALIAS(setup, "8", "A.GND");
            netlist.nl_setup_global.ALIAS(setup, "9", "A.O3");
            netlist.nl_setup_global.ALIAS(setup, "10", "A.O2");
            netlist.nl_setup_global.ALIAS(setup, "11", "A.O1");
            netlist.nl_setup_global.ALIAS(setup, "12", "A.O0");
            netlist.nl_setup_global.ALIAS(setup, "13", "A.CE1Q");
            netlist.nl_setup_global.ALIAS(setup, "14", "A.CE2Q");
            netlist.nl_setup_global.ALIAS(setup, "15", "A.A7");
            netlist.nl_setup_global.ALIAS(setup, "16", "A.VCC");

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier:  PROM_82S123_DIP
        //- Title: 82S123 256 bit TTL bipolar PROM
        //- Description: The 82S123 and 82S23 are field programmable, which means that
        //-    custom patterns are immediately available by following the Signetics
        //-    Generic I fusing procedure. The 82S123 and 82S23 devices are supplied
        //-    with all outputs at logical Low. Outputs are programmed to a logic High
        //-    level at any specified address by fusing the Ni-Cr link matrix.
        //-
        //-    These devices include on-chip decoding and 1 Chip Enable inputs for
        //-    memory expansion. They feature either open collector or 3-State outputs
        //-    for optimization of word expansion in bused organizations.
        //-
        //-
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
            netlist.nl_setup_global.NETLIST_START();

            netlist.nld_roms_global.PROM_82S123(setup, "A");

            netlist.nl_setup_global.DEFPARAM(setup, "ROM", "unknown");
            netlist.nl_setup_global.DEFPARAM(setup, "FORCE_TRISTATE_LOGIC", "0");
            netlist.nl_setup_global.DEFPARAM(setup, "MODEL", "$(@.A.MODEL)");
            netlist.nl_setup_global.PARAM(setup, "A.ROM", "$(@.ROM)");
            netlist.nl_setup_global.PARAM(setup, "A.FORCE_TRISTATE_LOGIC", "$(@.FORCE_TRISTATE_LOGIC)");
            netlist.nl_setup_global.PARAM(setup, "A.MODEL", "$(@.MODEL)");
            netlist.nl_setup_global.ALIAS(setup, "1", "A.O0");
            netlist.nl_setup_global.ALIAS(setup, "2", "A.O1");
            netlist.nl_setup_global.ALIAS(setup, "3", "A.O2");
            netlist.nl_setup_global.ALIAS(setup, "4", "A.O3");
            netlist.nl_setup_global.ALIAS(setup, "5", "A.O4");
            netlist.nl_setup_global.ALIAS(setup, "6", "A.O5");
            netlist.nl_setup_global.ALIAS(setup, "7", "A.O6");
            netlist.nl_setup_global.ALIAS(setup, "8", "A.GND");

            netlist.nl_setup_global.ALIAS(setup, "9", "A.O7");
            netlist.nl_setup_global.ALIAS(setup, "10", "A.A0");
            netlist.nl_setup_global.ALIAS(setup, "11", "A.A1");
            netlist.nl_setup_global.ALIAS(setup, "12", "A.A2");
            netlist.nl_setup_global.ALIAS(setup, "13", "A.A3");
            netlist.nl_setup_global.ALIAS(setup, "14", "A.A4");
            netlist.nl_setup_global.ALIAS(setup, "15", "A.CEQ");
            netlist.nl_setup_global.ALIAS(setup, "16", "A.VCC");

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier:  EPROM_2716_DIP
        //- Title: 2716 16K (2K x 8) UV ERASABLE PROM
        //- Description: The Intel®2716 is a 16,384-bit ultraviolet erasable and
        //-   electrically programmable read-only memory (EPROM). The 2716 operates
        //-   from a single 5-volt power supply, has a static standby mode, and
        //-   features fast single address location programming. It makes designing
        //-   with EPROMs faster, easier and more economical.
        //-
        //-   The 2716, with its single 5-volt supply and with an access time up
        //-   to 350 ns, is ideal for use with the newer high performance
        //-   +5V microprocessors such as Intel's 8085 and 8086. A selected
        //-   2716-5 and 2716-6 is available for slower speed applications.
        //-   The 2716 is also the first EPROM with a static standby mode which reduces
        //-   the power dissipation without increasing access time. The maximum
        //-   active power dissipation is 525 mW while the maximum standby power
        //-   dissipation is only 132 mW, a 75% savings.
        //-
        //-   The 2716 has the simplest and fastest method yet devised for
        //-   programming EPROMs - single pulse TTL level programming. No need for high
        //-   voltage pulsing because all programming controls are handled by
        //-   TTL signals. Program any location at any time-either individually,
        //-   sequentially or at random, with the 2716's single address location
        //-   programming. Total programming time for all 16,384 bits is only 100 seconds
        //-
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
            netlist.nl_setup_global.NETLIST_START();

            netlist.nld_roms_global.EPROM_2716(setup, "A");

            netlist.nl_setup_global.DEFPARAM(setup, "ROM", "unknown");
            netlist.nl_setup_global.DEFPARAM(setup, "FORCE_TRISTATE_LOGIC", "0");
            netlist.nl_setup_global.DEFPARAM(setup, "MODEL", "$(@.A.MODEL)");
            netlist.nl_setup_global.PARAM(setup, "A.ROM", "$(@.ROM)");
            netlist.nl_setup_global.PARAM(setup, "A.FORCE_TRISTATE_LOGIC", "$(@.FORCE_TRISTATE_LOGIC)");
            netlist.nl_setup_global.PARAM(setup, "A.MODEL", "$(@.MODEL)");
            netlist.nl_setup_global.ALIAS(setup, "1", "A.A7");
            netlist.nl_setup_global.ALIAS(setup, "2", "A.A6");
            netlist.nl_setup_global.ALIAS(setup, "3", "A.A5");
            netlist.nl_setup_global.ALIAS(setup, "4", "A.A4");
            netlist.nl_setup_global.ALIAS(setup, "5", "A.A3");
            netlist.nl_setup_global.ALIAS(setup, "6", "A.A2");
            netlist.nl_setup_global.ALIAS(setup, "7", "A.A1");
            netlist.nl_setup_global.ALIAS(setup, "8", "A.A0");
            netlist.nl_setup_global.ALIAS(setup, "9", "A.D0");
            netlist.nl_setup_global.ALIAS(setup, "10", "A.D1");
            netlist.nl_setup_global.ALIAS(setup, "11", "A.D2");
            netlist.nl_setup_global.ALIAS(setup, "12", "A.GND");

            netlist.nl_setup_global.ALIAS(setup, "13", "A.D3");
            netlist.nl_setup_global.ALIAS(setup, "14", "A.D4");
            netlist.nl_setup_global.ALIAS(setup, "15", "A.D5");
            netlist.nl_setup_global.ALIAS(setup, "16", "A.D6");
            netlist.nl_setup_global.ALIAS(setup, "17", "A.D7");
            netlist.nl_setup_global.ALIAS(setup, "18", "A.CE1Q"); // CEQ
            netlist.nl_setup_global.ALIAS(setup, "19", "A.A10");
            netlist.nl_setup_global.ALIAS(setup, "20", "A.CE2Q"); // OEQ
            netlist.nl_setup_global.ALIAS(setup, "22", "A.A9");
            netlist.nl_setup_global.ALIAS(setup, "23", "A.A8");
            netlist.nl_setup_global.ALIAS(setup, "24", "A.VCC");

            netlist.nl_setup_global.NETLIST_END();
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
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            TTL_82S16(A);
#endif

            netlist.nl_setup_global.DIPPINS(setup,      /*        +--------------+       */
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

            netlist.nl_setup_global.NETLIST_END();
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
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            PROM_82S115(A);
#endif
            netlist.devices.nld_system_global.NC_PIN(setup, "NC");

            netlist.nl_setup_global.DIPPINS(setup,    /*      +--------------+        */
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

            netlist.nl_setup_global.NETLIST_END();
        }


        //- Identifier:  PROM_MK28000_DIP
        //- Title: MK28000 (2048 x 8 or 4096 x 4) 16384-Bit TTL PROM
        //- Description: This dynamic ROM is organized internally as 2048 x 8 bits,
        //-    but has separate enables for the upper and lower nybbles, permitting
        //-    use as 4096 x 4 as well.
        //.
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
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            PROM_MK28000(A);
#endif

            netlist.nl_setup_global.DEFPARAM(setup, "ROM", "unknown");
            netlist.nl_setup_global.PARAM(setup, "A.ROM", "$(@.ROM)");
            netlist.nl_setup_global.ALIAS(setup, "1", "A.VCC");
            netlist.nl_setup_global.ALIAS(setup, "2", "A.A1");
            netlist.nl_setup_global.ALIAS(setup, "3", "A.A2");
            netlist.nl_setup_global.ALIAS(setup, "4", "A.A3");
            netlist.nl_setup_global.ALIAS(setup, "5", "A.A4");
            netlist.nl_setup_global.ALIAS(setup, "6", "A.A5");
            netlist.nl_setup_global.ALIAS(setup, "7", "A.A6");
            netlist.nl_setup_global.ALIAS(setup, "8", "A.A10");
            netlist.nl_setup_global.ALIAS(setup, "9", "A.GND");
            netlist.nl_setup_global.ALIAS(setup, "10", "A.A9");
            netlist.nl_setup_global.ALIAS(setup, "11", "A.A8");
            netlist.nl_setup_global.ALIAS(setup, "12", "A.A7");
            netlist.nl_setup_global.ALIAS(setup, "13", "A.ARQ");
            netlist.nl_setup_global.ALIAS(setup, "14", "A.OE2");
            netlist.nl_setup_global.ALIAS(setup, "15", "A.A11");
            netlist.nl_setup_global.ALIAS(setup, "16", "A.O8");
            netlist.nl_setup_global.ALIAS(setup, "17", "A.O7");
            netlist.nl_setup_global.ALIAS(setup, "18", "A.O6");
            netlist.nl_setup_global.ALIAS(setup, "19", "A.O5");
            netlist.nl_setup_global.ALIAS(setup, "20", "A.O4");
            netlist.nl_setup_global.ALIAS(setup, "21", "A.O3");
            netlist.nl_setup_global.ALIAS(setup, "22", "A.O2");
            netlist.nl_setup_global.ALIAS(setup, "23", "A.O1");
            netlist.nl_setup_global.ALIAS(setup, "24", "A.OE1");

            netlist.nl_setup_global.NETLIST_END();
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
            netlist.nl_setup_global.NETLIST_START();

            throw new emu_unimplemented();
#if false
            RAM_2102A(A);
#endif

            netlist.nl_setup_global.DIPPINS(setup,    /*      +--------------+      */
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

            netlist.nl_setup_global.NETLIST_END();
        }


        //NETLIST_START(roms_lib)
        public static void netlist_roms_lib(netlist.nlparse_t setup)
        {
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "PROM_82S123_DIP", netlist_PROM_82S123_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "PROM_82S126_DIP", netlist_PROM_82S126_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "PROM_74S287_DIP", netlist_PROM_74S287_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "EPROM_2716_DIP", netlist_EPROM_2716_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "TTL_82S16_DIP", netlist_TTL_82S16_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "PROM_82S115_DIP", netlist_PROM_82S115_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "PROM_MK28000_DIP", netlist_PROM_MK28000_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "RAM_2102A_DIP", netlist_RAM_2102A_DIP);

            netlist.nl_setup_global.NETLIST_END();
        }
    }
}
