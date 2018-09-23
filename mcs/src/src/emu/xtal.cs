// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using u32 = System.UInt32;
using attoseconds_t = System.Int64;


namespace mame
{
    static class XTAL_global
    {
        //template <typename T> constexpr auto operator /(T &&div, const XTAL &xtal) { return div / xtal.dvalue(); }

        //constexpr XTAL operator *(int          mult, const XTAL &xtal) { return XTAL(xtal.base(), mult * xtal.dvalue()); }
        //constexpr XTAL operator *(unsigned int mult, const XTAL &xtal) { return XTAL(xtal.base(), mult * xtal.dvalue()); }
        //constexpr XTAL operator *(double       mult, const XTAL &xtal) { return XTAL(xtal.base(), mult * xtal.dvalue()); }

        //constexpr XTAL operator ""_Hz_XTAL(long double clock) { return XTAL(double(clock)); }
        //constexpr XTAL operator ""_kHz_XTAL(long double clock) { return XTAL(double(clock * 1e3)); }
        //constexpr XTAL operator ""_MHz_XTAL(long double clock) { return XTAL(double(clock * 1e6)); }

        //constexpr XTAL operator ""_Hz_XTAL(unsigned long long clock) { return XTAL(double(clock)); }
        //constexpr XTAL operator ""_kHz_XTAL(unsigned long long clock) { return XTAL(double(clock) * 1e3); }
        //constexpr XTAL operator ""_MHz_XTAL(unsigned long long clock) { return XTAL(double(clock) * 1e6); }
    }


    public class XTAL
    {
        // This array *must* stay in order, it's binary-searched
        static readonly double [] known_xtals =
        {
        /*
         Frequency       Sugarvassed            Examples
         -----------  ----------------------    ---------------------------------------- */
               32768, /* 32.768_kHz_XTAL        Used to drive RTC chips */
               38400, /* 38.4_kHz_XTAL          Resonator */
              384000, /* 384_kHz_XTAL           Resonator - Commonly used for driving OKI MSM5205 */
              400000, /* 400_kHz_XTAL           Resonator - OKI MSM5205 on Great Swordman h/w */
              430000, /* 430_kHz_XTAL           Resonator */
              455000, /* 455_kHz_XTAL           Resonator - OKI MSM5205 on Gladiator h/w */
              512000, /* 512_kHz_XTAL           Resonator - Toshiba TC8830F */
              600000, /* 600_kHz_XTAL           - */
              640000, /* 640_kHz_XTAL           Resonator - NEC UPD7759, Texas Instruments Speech Chips @ 8khz */
              960000, /* 960_kHz_XTAL           Resonator - Xerox Notetaker Keyboard UART */
             1000000, /* 1_MHz_XTAL             Used to drive OKI M6295 chips */
             1008000, /* 1.008_MHz_XTAL         Acorn Microcomputer (System 1) */
             1056000, /* 1.056_MHz_XTAL         Resonator - OKI M6295 on Trio The Punch h/w */
             1294400, /* 1.2944_MHz_XTAL        BBN BitGraph PSG */
             1689600, /* 1.6896_MHz_XTAL        Diablo 1355WP Printer */
             1750000, /* 1.75_MHz_XTAL          RCA CDP1861 */
             1797100, /* 1.7971_MHz_XTAL        SWTPC 6800 (with MIKBUG) */
             1843200, /* 1.8432_MHz_XTAL        Bondwell 12/14 */
             2000000, /* 2_MHz_XTAL             - */
             2012160, /* 2.01216_MHz_XTAL       Cidelsa Draco sound board */
             2097152, /* 2.097152_MHz_XTAL      Icatel 1995 - Brazilian public payphone */
             2457600, /* 2.4576_MHz_XTAL        Atari ST MFP */
             2457600, /* 2.4576_MHz_XTAL        Atari ST MFP, NEC PC-98xx */
             2500000, /* 2.5_MHz_XTAL           Janken Man units */
             2950000, /* 2.95_MHz_XTAL          Playmatic MPU-C, MPU-III & Sound-3 */
             3000000, /* 3_MHz_XTAL             Probably only used to drive 68705 or similar MCUs on 80's Taito PCBs */
             3072000, /* 3.072_MHz_XTAL         INS 8520 input clock rate */
             3120000, /* 3.12_MHz_XTAL          SP0250 clock on Gottlieb games */
             3521280, /* 3.52128_MHz_XTAL       RCA COSMAC VIP */
             3570000, /* 3.57_MHz_XTAL          Telmac TMC-600 */
             3578640, /* 3.57864_MHz_XTAL       Atari Portfolio PCD3311T */
             3579545, /* 3.579545_MHz_XTAL      NTSC color subcarrier, extremely common, used on 100's of PCBs (Keytronic custom part #48-300-010 is equivalent) */
             3686400, /* 3.6864_MHz_XTAL        Baud rate clock for MC68681 and similar UARTs */
             3840000, /* 3.84_MHz_XTAL          Fairlight CMI Alphanumeric Keyboard */
             3900000, /* 3.9_MHz_XTAL           Resonator - Used on some Fidelity boards */
             4000000, /* 4_MHz_XTAL             - */
             4028000, /* 4.028_MHz_XTAL         Sony SMC-777 */
             4032000, /* 4.032_MHz_XTAL         GRiD Compass modem board */
             4096000, /* 4.096_MHz_XTAL         Used to drive OKI M9810 chips */
             4194304, /* 4.194304_MHz_XTAL      Used to drive MC146818 / Nintendo Game Boy */
             4224000, /* 4.224_MHz_XTAL         Used to drive OKI M6295 chips, usually with /4 divider */
             4410000, /* 4.41_MHz_XTAL          Pioneer PR-8210 ldplayer */
             4433610, /* 4.43361_MHz_XTAL       Cidelsa Draco */
             4433619, /* 4.433619_MHz_XTAL      PAL color subcarrier (technically 4.43361875mhz)*/
             4608000, /* 4.608_MHz_XTAL         Luxor ABC-77 keyboard (Keytronic custom part #48-300-107 is equivalent) */
             4915200, /* 4.9152_MHz_XTAL        - */
             5000000, /* 5_MHz_XTAL             Mutant Night */
             5068800, /* 5.0688_MHz_XTAL        Usually used as MC2661 or COM8116 baud rate clock */
             5185000, /* 5.185_MHz_XTAL         Intel INTELLEC® 4 */
             5460000, /* 5.46_MHz_XTAL          ec1840 and ec1841 keyboard */
             5529600, /* 5.5296_MHz_XTAL        Kontron PSI98 keyboard */
             5626000, /* 5.626_MHz_XTAL         RCA CDP1869 PAL dot clock */
             5670000, /* 5.67_MHz_XTAL          RCA CDP1869 NTSC dot clock */
             5714300, /* 5.7143_MHz_XTAL        Cidelsa Destroyer, TeleVideo serial keyboards */
             5911000, /* 5.911_MHz_XTAL         Philips Videopac Plus G7400 */
             5990400, /* 5.9904_MHz_XTAL        Luxor ABC 800 keyboard (Keytronic custom part #48-300-008 is equivalent) */
             6000000, /* 6_MHz_XTAL             American Poker II, Taito SJ System */
             6144000, /* 6.144_MHz_XTAL         Used on Alpha Denshi early 80's games sound board, Casio FP-200 and Namco Universal System 16 */
             6400000, /* 6.4_MHz_XTAL           Textel Compact */
             6500000, /* 6.5_MHz_XTAL           Jupiter Ace */
             6880000, /* 6.88_MHz_XTAL          Barcrest MPU4 */
             6900000, /* 6.9_MHz_XTAL           BBN BitGraph CPU */
             7000000, /* 7_MHz_XTAL             Jaleco Mega System PCBs */
             7159090, /* 7.15909_MHz_XTAL       Blood Bros (2x NTSC subcarrier) */
             7372800, /* 7.3728_MHz_XTAL        - */
             7864300, /* 7.8643_MHz_XTAL        Used on InterFlip games as video clock */
             7987000, /* 7.987_MHz_XTAL         PC9801-86 YM2608 clock */
             8000000, /* 8_MHz_XTAL             Extremely common, used on 100's of PCBs */
             8200000, /* 8.2_MHz_XTAL           Universal Mr. Do - Model 8021 PCB */
             8388000, /* 8.388_MHz_XTAL         Nintendo Game Boy Color */
             8448000, /* 8.448_MHz_XTAL         Banpresto's Note Chance - Used to drive OKI M6295 chips, usually with /8 divider */
             8467200, /* 8.4672_MHz_XTAL        Subsino's Ying Hua Lian */
             8664000, /* 8.664_MHz_XTAL         Touchmaster */
             8700000, /* 8.7_MHz_XTAL           Tandberg TDV 2324 */
             8867236, /* 8.867236_MHz_XTAL      RCA CDP1869 PAL color clock (~2x PAL subcarrier) */
             8867238, /* 8.867238_MHz_XTAL      ETI-660 (~2x PAL subcarrier) */
             8945000, /* 8.945_MHz_XTAL         Hit Me */
             9216000, /* 9.216_MHz_XTAL         Conitec PROF-180X */
             9828000, /* 9.828_MHz_XTAL         Universal PCBs */
             9830400, /* 9.8304_MHz_XTAL        Epson PX-8 */
             9832000, /* 9.832_MHz_XTAL         Robotron A7150 */
             9877680, /* 9.87768_MHz_XTAL       Microterm 420 */
             9987000, /* 9.987_MHz_XTAL         Crazy Balloon */
            10000000, /* 10_MHz_XTAL            - */
            10137600, /* 10.1376_MHz_XTAL       Wyse WY-100 */
            10245000, /* 10.245_MHz_XTAL        PES Speech box */
            10380000, /* 10.38_MHz_XTAL         Fairlight Q219 Lightpen/Graphics Card */
            10500000, /* 10.5_MHz_XTAL          Agat-7 */
            10595000, /* 10.595_MHz_XTAL        Mad Alien */
            10644500, /* 10.6445_MHz_XTAL       TRS-80 Model I */
            10687500, /* 10.6875_MHz_XTAL       BBC Bridge Companion */
            10694250, /* 10.69425_MHz_XTAL      Xerox 820 */
            10717200, /* 10.7172_MHz_XTAL       Eltec EurocomII */
            10730000, /* 10.73_MHz_XTAL         Ruleta RE-900 VDP Clock */
            10733000, /* 10.733_MHz_XTAL        The Fairyland Story */
            10738635, /* 10.738635_MHz_XTAL     TMS9918 family (3x NTSC subcarrier) */
            10816000, /* 10.816_MHz_XTAL        Universal 1979-1980 (Cosmic Alien, etc) */
            10920000, /* 10.92_MHz_XTAL         ADDS Viewpoint 60, Viewpoint A2 */
            11000000, /* 11_MHz_XTAL            Mario I8039 sound */
            11059200, /* 11.0592_MHz_XTAL       Used with MCS-51 to generate common baud rates */
            11200000, /* 11.2_MHz_XTAL          New York, New York */
            11289000, /* 11.289_MHz_XTAL        Vanguard */
            11400000, /* 11.4_MHz_XTAL          HP 9845 */
            11668800, /* 11.6688_MHz_XTAL       Gameplan pixel clock */
            11800000, /* 11.8_MHz_XTAL          IBM PC Music Feature Card */
            11980800, /* 11.9808_MHz_XTAL       Luxor ABC 80 */
            12000000, /* 12_MHz_XTAL            Extremely common, used on 100's of PCBs */
            12057600, /* 12.0576_MHz_XTAL       Poly 1 (38400 * 314) */
            12096000, /* 12.096_MHz_XTAL        Some early 80's Atari games */
            12288000, /* 12.288_MHz_XTAL        Sega Model 3 digital audio board */
            12324000, /* 12.324_MHz_XTAL        Otrona Attache */
            12432000, /* 12.432_MHz_XTAL        Kaneko Fly Boy/Fast Freddie Hardware */
            12472500, /* 12.4725_MHz_XTAL       Bonanza's Mini Boy 7 */
            12480000, /* 12.48_MHz_XTAL         TRS-80 Model II */
            12500000, /* 12.5_MHz_XTAL          Red Alert audio board */
            12672000, /* 12.672_MHz_XTAL        TRS-80 Model 4 80*24 video */
            12800000, /* 12.8_MHz_XTAL          Cave CV1000 */
            12854400, /* 12.8544_MHz_XTAL       Alphatronic P3 */
            12936000, /* 12.936_MHz_XTAL        CDC 721 */
            12979200, /* 12.9792_MHz_XTAL       Exidy 440 */
            13300000, /* 13.3_MHz_XTAL          BMC bowling */
            13330560, /* 13.33056_MHz_XTAL      Taito L */
            13333000, /* 13.333_MHz_XTAL        Ojanko High School */
            13400000, /* 13.4_MHz_XTAL          TNK3, Ikari Warriors h/w */
            13478400, /* 13.4784_MHz_XTAL       TeleVideo 970 80-column display clock */
            13495200, /* 13.4952_MHz_XTAL       Used on Shadow Force pcb and maybe other Technos pcbs? */
            13516800, /* 13.5168_MHz_XTAL       Kontron KDT6 */
            13608000, /* 13.608_MHz_XTAL        TeleVideo 910 & 925 */
            13824000, /* 13.824_MHz_XTAL        Robotron PC-1715 display circuit */
            14000000, /* 14_MHz_XTAL            - */
            14112000, /* 14.112_MHz_XTAL        Timex/Sinclair TS2068 */
            14192640, /* 14.19264_MHz_XTAL      Central Data 2650 */
            14218000, /* 14.218_MHz_XTAL        Dragon */
            14300000, /* 14.3_MHz_XTAL          Agat-7 */
            14314000, /* 14.314_MHz_XTAL        Taito TTL Board  */
            14318181, /* 14.318181_MHz_XTAL     Extremely common, used on 100's of PCBs (4x NTSC subcarrier) */
            14705882, /* 14.705882_MHz_XTAL     Aleck64 */
            14745600, /* 14.7456_MHz_XTAL       Namco System 12 & System Super 22/23 for JVS */
            14784000, /* 14.784_MHz_XTAL        Zenith Z-29 */
            14916000, /* 14.916_MHz_XTAL        ADDS Viewpoint 122 */
            14976000, /* 14.976_MHz_XTAL        CIT-101 80-column display clock */
            15000000, /* 15_MHz_XTAL            Sinclair QL, Amusco Poker */
            15148800, /* 15.1488_MHz_XTAL       Zentec 9002/9003 */
            15288000, /* 15.288_MHz_XTAL        DEC VT220 80-column display clock */
            15300720, /* 15.30072_MHz_XTAL      Microterm 420 */
            15360000, /* 15.36_MHz_XTAL         Visual 1050 */
            15400000, /* 15.4_MHz_XTAL          DVK KSM */
            15468480, /* 15.46848_MHz_XTAL      Bank Panic h/w, Sega G80 */
            15582000, /* 15.582_MHz_XTAL        Zentec Zephyr */
            15700000, /* 15.700_MHz_XTAL        Motogonki */
            15897600, /* 15.8976_MHz_XTAL       IAI Swyft */
            15920000, /* 15.92_MHz_XTAL         HP Integral PC */
            15974400, /* 15.9744_MHz_XTAL       Osborne 1 (9600 * 52 * 32) */
            16000000, /* 16_MHz_XTAL            Extremely common, used on 100's of PCBs */
            16097280, /* 16.09728_MHz_XTAL      DEC VT240 (1024 * 262 * 60) */
            16128000, /* 16.128_MHz_XTAL        Fujitsu FM-7 */
            16384000, /* 16.384_MHz_XTAL        - */
            16400000, /* 16.4_MHz_XTAL          MS 6102 */
            16572000, /* 16.572_MHz_XTAL        Micro-Term ACT-5A */
            16588800, /* 16.5888_MHz_XTAL       SM 7238 */
            16669800, /* 16.6698_MHz_XTAL       Qume QVT-102 */
            16670000, /* 16.67_MHz_XTAL         - */
            16777216, /* 16.777216_MHz_XTAL     Nintendo Game Boy Advance */
            16934400, /* 16.9344_MHz_XTAL       Usually used to drive 90's Yamaha OPL/FM chips (44100 * 384) */
            17064000, /* 17.064_MHz_XTAL        Memorex 1377 */
            17360000, /* 17.36_MHz_XTAL         OMTI Series 10 SCSI controller */
            17550000, /* 17.55_MHz_XTAL         HP 264x display clock (50 Hz configuration) */
            17734470, /* 17.73447_MHz_XTAL      (~4x PAL subcarrier) */
            17734472, /* 17.734472_MHz_XTAL     actually ~4x PAL subcarrier */
            17971200, /* 17.9712_MHz_XTAL       - */
            18000000, /* 18_MHz_XTAL            S.A.R, Ikari Warriors 3 */
            18432000, /* 18.432_MHz_XTAL        Extremely common, used on 100's of PCBs (48000 * 384) */
            18480000, /* 18.48_MHz_XTAL         Wyse WY-100 video */
            18575000, /* 18.575_MHz_XTAL        Visual 102, Visual 220 */
            18720000, /* 18.72_MHz_XTAL         Nokia MikroMikko 1 */
            18869600, /* 18.8696_MHz_XTAL       Memorex 2178 */
            19339600, /* 19.3396_MHz_XTAL       TeleVideo TVI-955 80-column display clock */
            19600000, /* 19.6_MHz_XTAL          Universal Mr. Do - Model 8021 PCB */
            19660800, /* 19.6608_MHz_XTAL       Euro League (bootleg), labeled as "UKI 19.6608 20PF" */
            19661400, /* 19.6614_MHz_XTAL       Wyse WY-30 */
            19923000, /* 19.923_MHz_XTAL        Cinematronics vectors */
            19968000, /* 19.968_MHz_XTAL        Used mostly by some Taito games */
            20000000, /* 20_MHz_XTAL            - */
            20160000, /* 20.16_MHz_XTAL         Nintendo 8080 */
            20275200, /* 20.2752_MHz_XTAL       TRS-80 Model III */
            20625000, /* 20.625_MHz_XTAL        SM 7238 */
            20790000, /* 20.79_MHz_XTAL         Blockade-hardware Gremlin games */
            21000000, /* 21_MHz_XTAL            Lock-On pixel clock */
            21052600, /* 21.0526_MHz_XTAL       NEC PC-98xx pixel clock */
            21060000, /* 21.06_MHz_XTAL         HP 264x display clock (60 Hz configuration) */
            21254400, /* 21.2544_MHz_XTAL       TeleVideo 970 132-column display clock */
            21281370, /* 21.28137_MHz_XTAL      Radica Tetris (PAL) */
            21300000, /* 21.3_MHz_XTAL          - */
            21477272, /* 21.477272_MHz_XTAL     BMC bowling, some Data East 90's games, Vtech Socrates; (6x NTSC subcarrier) */
            22000000, /* 22_MHz_XTAL            - */
            22096000, /* 22.096_MHz_XTAL        ADDS Viewpoint 122 */
            22118400, /* 22.1184_MHz_XTAL       Amusco Poker */
            22321000, /* 22.321_MHz_XTAL        Apple LaserWriter II NT */
            22464000, /* 22.464_MHz_XTAL        CIT-101 132-column display clock */
            22656000, /* 22.656_MHz_XTAL        Super Pinball Action (~1440x NTSC line rate) */
            22896000, /* 22.896_MHz_XTAL        DEC VT220 132-column display clock */
            23814000, /* 23.814_MHz_XTAL        TeleVideo TVI-912, 920 & 950 */
            23961600, /* 23.9616_MHz_XTAL       Osborne 4 (Vixen) */
            24000000, /* 24_MHz_XTAL            Mario, 80's Data East games, 80's Konami games */
            24073400, /* 24.0734_MHz_XTAL       DEC Rainbow 100 */
            24576000, /* 24.576_MHz_XTAL        Pole Position h/w, Model 3 CPU board */
            24883200, /* 24.8832_MHz_XTAL       DEC VT100 */
            25000000, /* 25_MHz_XTAL            Namco System 22, Taito GNET, Dogyuun h/w */
            25174800, /* 25.1748_MHz_XTAL       Sega System 16A/16B (1600x NTSC line rate) */
            25200000, /* 25.2_MHz_XTAL          Tektronix 4404 video clock */
            25398360, /* 25.39836_MHz_XTAL      Tandberg TDV 2324 */
            25400000, /* 25.4_MHz_XTAL          PC9801-86 PCM base clock */
            25447000, /* 25.447_MHz_XTAL        Namco EVA3A (Funcube2) */
            25590906, /* 25.590906_MHz_XTAL     Atari Jaguar NTSC */
            25593900, /* 25.5939_MHz_XTAL       Atari Jaguar PAL */
            25771500, /* 25.7715_MHz_XTAL       HP-2622A */
            25920000, /* 25.92_MHz_XTAL         ADDS Viewpoint 60 */
            26000000, /* 26_MHz_XTAL            Gaelco PCBs */
            26366000, /* 26.366_MHz_XTAL        DEC VT320 */
            26580000, /* 26.58_MHz_XTAL         Wyse WY-60 80-column display clock */
            26601712, /* 26.601712_MHz_XTAL     Astro Corp.'s Show Hand, PAL Vtech/Yeno Socrates (6x PAL subcarrier) */
            26666000, /* 26.666_MHz_XTAL        Imagetek I4100/I4220/I4300 */
            26666666, /* 26.666666_MHz_XTAL     Irem M92 but most use 27MHz */
            26686000, /* 26.686_MHz_XTAL        Typically used on 90's Taito PCBs to drive the custom chips */
            26989200, /* 26.9892_MHz_XTAL       TeleVideo 965 */
            27000000, /* 27_MHz_XTAL            Some Banpresto games macrossp, Irem M92 and 90's Toaplan games */
            27164000, /* 27.164_MHz_XTAL        Typically used on 90's Taito PCBs to drive the custom chips */
            27210900, /* 27.2109_MHz_XTAL       LA Girl */
            27562000, /* 27.562_MHz_XTAL        Visual 220 */
            28000000, /* 28_MHz_XTAL            - */
            28322000, /* 28.322_MHz_XTAL        Saitek RISC 2500, Mephisto Montreux */
            28375160, /* 28.37516_MHz_XTAL      Amiga PAL systems */
            28475000, /* 28.475_MHz_XTAL        CoCo 3 PAL */
            28480000, /* 28.48_MHz_XTAL         Chromatics CGC-7900 */
            28636363, /* 28.636363_MHz_XTAL     Later Leland games and Atari GT, Amiga NTSC, Raiden2 h/w (8x NTSC subcarrier)*/
            28640000, /* 28.64_MHz_XTAL         Fukki FG-1c AI AM-2 PCB */
            28700000, /* 28.7_MHz_XTAL          - */
            29376000, /* 29.376_MHz_XTAL        Qume QVT-103 */
            29491200, /* 29.4912_MHz_XTAL       Xerox Alto-II system clock (tagged 29.4MHz in the schematics) */
            30000000, /* 30_MHz_XTAL            Impera Magic Card */
            30476100, /* 30.4761_MHz_XTAL       Taito JC */
            30800000, /* 30.8_MHz_XTAL          15IE-00-013 */
            31279500, /* 31.2795_MHz_XTAL       Wyse WY-30+ */
            31684000, /* 31.684_MHz_XTAL        TeleVideo TVI-955 132-column display clock */
            31948800, /* 31.9488_MHz_XTAL       NEC PC-88xx, PC-98xx */
            32000000, /* 32_MHz_XTAL            - */
            32220000, /* 32.22_MHz_XTAL         Typically used on 90's Data East PCBs (close to 9x NTSC subcarrier which is 32.215905Mhz */
            32317400, /* 32.3174_MHz_XTAL       DEC VT330, VT340 */
            32530400, /* 32.5304_MHz_XTAL       Seta 2 */
            33000000, /* 33_MHz_XTAL            Sega Model 3 video board */
            33264000, /* 33.264_MHz_XTAL        Hazeltine 1500 terminal */
            33333000, /* 33.333_MHz_XTAL        Sega Model 3 CPU board, Vegas */
            33833000, /* 33.833_MHz_XTAL        - */
            33868800, /* 33.8688_MHz_XTAL       Usually used to drive 90's Yamaha OPL/FM chips with /2 divider */
            34000000, /* 34_MHz_XTAL            Gaelco PCBs */
            34291712, /* 34.291712_MHz_XTAL     Fairlight CMI master card */
            34846000, /* 34.846_MHz_XTAL        Visual 550 */
            35904000, /* 35.904_MHz_XTAL        Used on HP98543 graphics board */
            36000000, /* 36_MHz_XTAL            Sega Model 1 video board */
            38769220, /* 38.76922_MHz_XTAL      Namco System 21 video board */
            38863630, /* 38.86363_MHz_XTAL      Sharp X68000 15.98kHz video */
            39321600, /* 39.3216_MHz_XTAL       Sun 2/120 */
            39710000, /* 39.71_MHz_XTAL         Wyse WY-60 132-column display clock */
            40000000, /* 40_MHz_XTAL            - */
            40210000, /* 40.21_MHz_XTAL         Fairlight CMI IIx */
            42000000, /* 42_MHz_XTAL            BMC A-00211 - Popo Bear */
            42105200, /* 42.1052_MHz_XTAL       NEC PC-88xx */
            42954545, /* 42.954545_MHz_XTAL     CPS3 (12x NTSC subcarrier)*/
            43320000, /* 43.32_MHz_XTAL         DEC VT420 */
            44100000, /* 44.1_MHz_XTAL          Subsino's Bishou Jan */
            44452800, /* 44.4528_MHz_XTAL       TeleVideo 965 */
            45000000, /* 45_MHz_XTAL            Eolith with Hyperstone CPUs */
            45158000, /* 45.158_MHz_XTAL        Sega Model 2A video board, Model 3 CPU board */
            45619200, /* 45.6192_MHz_XTAL       DEC VK100 */
            45830400, /* 45.8304_MHz_XTAL       Microterm 5510 */
            46615120, /* 46.61512_Mhz_XTAL      Soundblaster 16 PCM base clock */
            47736000, /* 47.736_MHz_XTAL        Visual 100 */
            48000000, /* 48_MHz_XTAL            Williams/Midway Y/Z-unit system / SSV board */
            48384000, /* 48.384_MHz_XTAL        Namco NB-1 */
            48556800, /* 48.5568_MHz_XTAL       Wyse WY-85 */
            48654000, /* 48.654_MHz_XTAL        Qume QVT-201 */
            48660000, /* 48.66_MHz_XTAL         Zaxxon */
            49152000, /* 49.152_MHz_XTAL        Used on some Namco PCBs, Baraduke h/w, System 21, Super System 22 */
            49423500, /* 49.4235_MHz_XTAL       Wyse WY-185 */
            50000000, /* 50_MHz_XTAL            Williams/Midway T/W/V-unit system */
            50113000, /* 50.113_MHz_XTAL        Namco NA-1 (14x NTSC subcarrier)*/
            50349000, /* 50.349_MHz_XTAL        Sega System 18 (~3200x NTSC line rate) */
            52000000, /* 52_MHz_XTAL            Cojag */
            52832000, /* 52.832_MHz_XTAL        Wang PC TIG video controller */
            53203400, /* 53.2034_MHz_XTAL       Master System, Mega Drive PAL (~12x PAL subcarrier) */
            53693175, /* 53.693175_MHz_XTAL     PSX-based h/w, Sony ZN1-2-based (15x NTSC subcarrier) */
            54000000, /* 54_MHz_XTAL            Taito JC */
            55000000, /* 55_MHz_XTAL            Eolith Vega */
            57272727, /* 57.272727_MHz_XTAL     Psikyo SH2 with /2 divider (16x NTSC subcarrier)*/
            58000000, /* 58_MHz_XTAL            Magic Reel (Play System) */
            59292000, /* 59.292_MHz_XTAL        Data General D461 */
            60000000, /* 60_MHz_XTAL            - */
            61440000, /* 61.44_MHz_XTAL         Donkey Kong */
            64000000, /* 64_MHz_XTAL            BattleToads */
            66666700, /* 66.6667_MHz_XTAL       Later Midway games */
            67737600, /* 67.7376_MHz_XTAL       PSX-based h/w, Sony ZN1-2-based */
            68850000, /* 68.85_MHz_XTAL         Wyse WY-50 */
            69551990, /* 69.55199_MHz_XTAL      Sharp X68000 31.5kHz video */
            72000000, /* 72_MHz_XTAL            Aristocrat MKV */
            72576000, /* 72.576_MHz_XTAL        Centipede, Millipede, Missile Command, Let's Go Bowling "Multipede" */
            73728000, /* 73.728_MHz_XTAL        Ms. Pac-Man/Galaga 20th Anniversary */
            80000000, /* 80_MHz_XTAL            ARM710 */
            87183360, /* 87.18336_MHz_XTAL      AT&T 630 MTG */
           100000000, /* 100_MHz_XTAL           PSX-based Namco System 12, Vegas, Sony ZN1-2-based */
           101491200, /* 101.4912_MHz_XTAL      PSX-based Namco System 10 */
           200000000  /* 200_MHz_XTAL           Base SH4 CPU (Naomi, Hikaru etc.) */
        };


        static double last_correct_value = -1;
        static double xtal_error_low = 0;
        static double xtal_error_high = 0;


        double m_base_clock;
        double m_current_clock;


        public XTAL(double base_clock) { m_base_clock = base_clock; m_current_clock = base_clock; }

        private XTAL(double base_clock, double current_clock) { m_base_clock = base_clock; m_current_clock = current_clock; }


        public double dvalue() { return m_current_clock; }
        public u32 value() { return (u32)m_current_clock; }
        double base_get() { return m_base_clock; }

        //template <typename T> constexpr XTAL operator *(T &&mult) const noexcept { return XTAL(m_base_clock, m_current_clock * mult); }
        public static XTAL operator* (XTAL lhs, int rhs) { return new XTAL(lhs.m_base_clock, lhs.m_current_clock * rhs); }
        public static XTAL operator* (UInt16 lhs, XTAL rhs) { return new XTAL(rhs.m_base_clock, lhs * rhs.m_current_clock); }

        //template <typename T> constexpr XTAL operator /(T &&div) const noexcept { return XTAL(m_base_clock, m_current_clock / div); }
        public static XTAL operator/ (XTAL lhs, int rhs) { return new XTAL(lhs.m_base_clock, lhs.m_current_clock / rhs); }
        public static attoseconds_t operator/ (attoseconds_t lhs, XTAL rhs) { return lhs / (attoseconds_t)rhs.value(); }

        //friend constexpr XTAL operator *(int          mult, const XTAL &xtal);
        //friend constexpr XTAL operator *(unsigned int mult, const XTAL &xtal);
        //friend constexpr XTAL operator *(double       mult, const XTAL &xtal);

        //void validate(const char *message) const;
        public void validate(string message)
        {
            if (!validate(m_base_clock))
                fail(m_base_clock, message);
        }


        static void fail(double base_clock, string message)
        {
            string full_message = global.string_format("Unknown crystal value {0}. ", base_clock);

            if (xtal_error_low != 0 && xtal_error_high != 0)
                full_message += global.string_format(" Did you mean {0} or {1}?", xtal_error_low, xtal_error_high);
            else
                full_message += global.string_format(" Did you mean {0}?", xtal_error_low != 0 ? xtal_error_low : xtal_error_high);

            full_message += global.string_format(" Context: {0}\n", message);

            global.fatalerror("{0}\n", full_message.c_str());
        }


        static bool validate(double base_clock)
        {
            if (base_clock == last_correct_value)
                return true;

            UInt32 xtal_count = (UInt32)known_xtals.Length;  //sizeof(known_xtals) / sizeof(known_xtals[0]);
            UInt32 last_index = xtal_count - 1;
            UInt32 fill1  = last_index | (last_index >> 1);
            UInt32 fill2  = fill1 | (fill1 >> 2);
            UInt32 fill4  = fill2 | (fill2 >> 4);
            UInt32 fill8  = fill4 | (fill4 >> 8);
            UInt32 fill16 = fill8 | (fill8 >> 16);
            UInt32 ppow2  = fill16 - (fill16 >> 1);

            UInt32 slot = ppow2;
            UInt32 step = ppow2;

            while (step != 0)
            {
                if (slot > last_index)
                {
                    slot = slot ^ (step | (step >> 1));
                }
                else
                {
                    double sfreq = known_xtals[slot];
                    double diff = Math.Abs((base_clock - sfreq) / base_clock);
                    if (diff <= (2 * global.DBL_EPSILON))
                    {
                        last_correct_value = base_clock;
                        return true;
                    }

                    if (base_clock > sfreq)
                        slot = slot | (step >> 1);
                    else
                        slot = slot ^ (step | (step >> 1));
                }

                step = step >> 1;
            }

            {
                double sfreq = known_xtals[slot];
                if (base_clock == sfreq)
                {
                    last_correct_value = base_clock;
                    return true;
                }

                if (base_clock < sfreq)
                {
                    if (slot != 0)
                        xtal_error_low = known_xtals[slot-1];
                    else
                        xtal_error_low = 0;

                    xtal_error_high = sfreq;
                }
                else
                {
                    if (slot < last_index)
                        xtal_error_high = known_xtals[slot+1];
                    else
                        xtal_error_high = 0;

                    xtal_error_low = sfreq;
                }
            }

            return false;
        }

        //static void check_ordering();
    }
}
