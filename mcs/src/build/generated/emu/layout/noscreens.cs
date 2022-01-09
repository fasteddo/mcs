// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.noscreens_global;
using static mame.noscreens_internal;


namespace mame
{
    static class noscreens_internal
    {
        public static readonly byte [] layout_noscreens_data =
        {
            120, 156, 109, 144,  81, 110, 131,  48,  12, 134, 223, 123,  10,  43,   7,   0, 218, 137, 169,  15,  33, 220,  96,  87, 152,   2, 184, 128, 148, 196,  85,  98,
             10, 189, 253, 204, 232,  86,  42, 245, 233, 119, 236, 248, 255, 108, 235, 122, 241,  14, 110,  24, 211,  72, 161,  82, 199, 172,  80, 181, 209, 222, 122, 116,
            246,  78,  19,  63,  75,  39, 101,  52,  58, 244,  24,  24, 130, 212,  43,  53, 219,  24, 198, 208,  75, 158, 113,  97,  72,  28, 229,  85, 169,  47, 130, 212,
             70, 196, 144, 192,  50, 219, 118, 192,  14, 152, 128,   7, 132, 116,  79, 140,  94,  26,  90, 114,  20,  33,  98, 183,  33, 161,  95, 255,  87, 170, 200,  74,
              5, 141, 155, 112,  11, 115, 163, 243, 213,  90, 228,  65,  54, 250,  54, 226, 252, 224, 191,   1, 137, 117,  67,  83, 232,  18,  56, 188, 176, 184,  40,  65,
             95, 127,  53, 142, 253,  32, 153, 211,  89, 226, 134, 152, 201,  11, 187,  44,  86, 200, 219, 181, 190, 121,  97, 233, 194, 203, 126, 209,  23, 243, 114, 231,
            250, 241,  71,  42, 119, 246, 159, 219,  10, 255, 179, 231, 235, 240,  34, 207, 243, 154, 195,  15,   4, 226, 126, 184
        };
    }


    public static class noscreens_global
    {
        public static readonly internal_layout layout_noscreens = new internal_layout
        (
            385, layout_noscreens_data.Length, internal_layout.compression.ZLIB, layout_noscreens_data
        );
    }
}
