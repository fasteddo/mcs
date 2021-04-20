// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int8_t = System.Byte;
using int16_t = System.Int16;


namespace mame
{
    class tms5100_coeffs
    {
        /* coefficient defines */
        const int MAX_K                   = 10;
        const int MAX_SCALE_BITS          = 6;
        const int MAX_SCALE               = 1 << MAX_SCALE_BITS;
        const int MAX_CHIRP_SIZE          = 52;


        int subtype;
        public int num_k;
        public int energy_bits;
        public int pitch_bits;
        public int [] kbits = new int [MAX_K];
        public UInt16 [] energytable = new UInt16 [MAX_SCALE];
        public UInt16 [] pitchtable = new UInt16 [MAX_SCALE];
        public int [,] ktable = new int [MAX_K, MAX_SCALE];
        public int16_t [] chirptable = new int16_t [MAX_CHIRP_SIZE];
        public int8_t [] interp_coeff = new int8_t [8];


        public tms5100_coeffs
        (
            int subtype,
            int num_k,
            int energy_bits,
            int pitch_bits,
            int [] kbits,
            UInt16 [] energytable,
            UInt16 [] pitchtable,
            int [,] ktable,
            int16_t [] chirptable,
            int8_t [] interp_coeff
        )
        {
            this.subtype = subtype;
            this.num_k = num_k;
            this.energy_bits = energy_bits;
            this.pitch_bits = pitch_bits;
            this.kbits = kbits;
            this.energytable = energytable;
            this.pitchtable = pitchtable;
            this.ktable = ktable;
            this.chirptable = chirptable;
            this.interp_coeff = interp_coeff;
        }
    }


    static class tms5110r_global
    {
        /* chip rom contents defines */
        //#define SUBTYPE_0281A           1
        //#define SUBTYPE_0281D           2
        //#define SUBTYPE_2801A           4
        //#define SUBTYPE_M58817          8
        //#define SUBTYPE_2802            16
        //#define SUBTYPE_5110            32
        //#define SUBTYPE_2501E           64
        const int SUBTYPE_5220            = 128;
        //#define SUBTYPE_PAT4335277      256
        //#define SUBTYPE_VLM5030         512


        /* common, shared coefficients */
        /* energy */
        //#define TI_0280_PATENT_ENERGY \

        static readonly UInt16 [] TI_028X_LATER_ENERGY =
            /* E  */
            {   0,  1,  2,  3,  4,  6,  8, 11,
               16, 23, 33, 47, 63, 85,114,  0 };

        /* pitch */
        //#define TI_0280_2801_PATENT_PITCH \
        //#define TI_2802_PITCH \
        //#define TI_5110_PITCH \
        //#define TI_2501E_PITCH \

        static readonly UInt16 [] TI_5220_PITCH =
            /* P */
            {    0,  15,  16,  17,  18,  19,  20,  21,
                22,  23,  24,  25,  26,  27,  28,  29,
                30,  31,  32,  33,  34,  35,  36,  37,
                38,  39,  40,  41,  42,  44,  46,  48,
                50,  52,  53,  56,  58,  60,  62,  65,
                68,  70,  72,  76,  78,  80,  84,  86,
                91,  94,  98, 101, 105, 109, 114, 118,
               122, 127, 132, 137, 142, 148, 153, 159 };


        /* LPC */
        //#define TI_0280_PATENT_LPC \
        //#define TI_2801_2501E_LPC \
        // below is the same as 2801/2501E above EXCEPT for K4 which is completely different.
        //#define TI_2802_LPC \

        static readonly int [,] TI_5110_5220_LPC = new int [,]
        {
            /* K1  */
            { -501, -498, -497, -495, -493, -491, -488, -482,
                -478, -474, -469, -464, -459, -452, -445, -437,
                -412, -380, -339, -288, -227, -158,  -81,   -1,
                80,  157,  226,  287,  337,  379,  411,  436 },
            /* K2  */
            { -328, -303, -274, -244, -211, -175, -138,  -99,
                -59,  -18,   24,   64,  105,  143,  180,  215,
                248,  278,  306,  331,  354,  374,  392,  408,
                422,  435,  445,  455,  463,  470,  476,  506 },
            /* K3  */
            { -441, -387, -333, -279, -225, -171, -117,  -63,
                -9,   45,   98,  152,  206,  260,  314,  368,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0 },
            /* K4  */
            { -328, -273, -217, -161, -106,  -50,    5,   61,
                116,  172,  228,  283,  339,  394,  450,  506,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0 },
            /* K5  */
            { -328, -282, -235, -189, -142,  -96,  -50,   -3,
                43,   90,  136,  182,  229,  275,  322,  368,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0 },
            /* K6  */
            { -256, -212, -168, -123,  -79,  -35,   10,   54,
                98,  143,  187,  232,  276,  320,  365,  409,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0 },
            /* K7  */
            { -308, -260, -212, -164, -117,  -69,  -21,   27,
                75,  122,  170,  218,  266,  314,  361,  409,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0 },
            /* K8  */
            { -256, -161,  -66,   29,  124,  219,  314,  409,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0 },
            /* K9  */
            { -256, -176,  -96,  -15,   65,  146,  226,  307,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0 },
            /* K10 */
            { -205, -132,  -59,   14,   87,  160,  234,  307,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0 },
        };


        /* chirp */
        //#define TI_0280_PATENT_CHIRP \
        // almost, but not exactly the same as the patent chirp above (25 bits differ)
        //#define TI_2801_CHIRP \
        //#define TI_2802_CHIRP \

        static readonly int16_t [] TI_LATER_CHIRP =
            /* Chirp table */
            {   0x00, 0x03, 0x0f, 0x28, 0x4c, 0x6c, 0x71, 0x50,
                0x25, 0x26, 0x4c, 0x44, 0x1a, 0x32, 0x3b, 0x13,
                0x37, 0x1a, 0x25, 0x1f, 0x1d, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00 };


        /* Interpolation Table */
        static readonly int8_t [] TI_INTERP =
            /* interpolation shift coefficients */
            { 0, 3, 3, 3, 2, 2, 1, 1 };


        //static const struct tms5100_coeffs T0280B_0281A_coeff =
        //static const struct tms5100_coeffs T0280D_0281D_coeff =
        //static const struct tms5100_coeffs T0280F_2801A_coeff =
        //static const struct tms5100_coeffs M58817_coeff =
        //static const struct tms5100_coeffs T0280F_2802_coeff =
        //static const struct tms5100_coeffs tms5110a_coeff =
        //static const struct tms5100_coeffs pat4335277_coeff =
        //static const struct tms5100_coeffs T0285_2501E_coeff =


        /* TMS5220/5220C:
        (1983 era for 5220, 1986-1992 era for 5220C; 5220C may also be called TSP5220C)
        The TMS5220NL was decapped and imaged by digshadow in April, 2013.
        The LPC table table is verified to match the decap.
        The chirp table is verified to match the decap. (sum = 0x3da)
        Note that all the LPC K* values match the TMS5110a table (as read via PROMOUT)
        exactly.
        The TMS5220CNL was decapped and imaged by digshadow in April, 2013.
        The LPC table table is verified to match the decap and exactly matches TMS5220NL.
        The chirp table is verified to match the decap. (sum = 0x3da)
        */
        public static readonly tms5100_coeffs tms5220_coeff = new tms5100_coeffs
        (
            /* subtype */
            SUBTYPE_5220,
            10,
            4,
            6,
            new int [] { 5, 5, 4, 4, 4, 4, 4, 3, 3, 3 },
            TI_028X_LATER_ENERGY,
            TI_5220_PITCH,
            TI_5110_5220_LPC,
            TI_LATER_CHIRP,
            TI_INTERP
        );


        //static const struct tms5100_coeffs vlm5030_coeff =
    }
}
