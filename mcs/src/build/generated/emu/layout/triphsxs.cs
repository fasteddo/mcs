// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    public static class triphsxs_global
    {
        static readonly byte [] layout_triphsxs_data =
        {
            120, 156, 189, 146,  59,  14, 194,  48,  12, 134, 119,  78,  17, 121, 130, 161,  77, 250,  16,  98,  72, 210, 145,   3, 192,   5,  90,  98, 209,  72, 109, 138,
            154,  62, 111,  79,  90, 134, 170,  44,  60, 132,  88, 108, 249, 183, 253,  73, 254, 101, 158,  12, 101,  65,  58, 172, 173, 174, 140, 128, 192, 103, 144,  72,
             94, 166,  37,  22, 233,  88, 181, 205, 210,  10,  65, 242,  78,  99,  79, 140, 107,  10,  56, 215, 250,  86,  32,  57, 105, 133,  94,  54, 122,  83, 118,   3,
            246,  82,  35,  26, 162, 141, 194,  65,   0, 115,  74,  86, 181,  70,  89,  50,  87, 100, 156,  99, 175,  85, 147,  11, 136, 129, 228, 168, 175, 121,  35,  32,
              2,  42,  57, 125,  44,  63,  67, 130,  21,  36, 246,  89, 244,  29,  39,  92, 113,  14,  62, 219, 191, 203, 161, 211, 217, 175, 142,  39, 219,  99, 234,  52,
            107, 119, 127, 177, 225,  39,  30, 124, 104,   0,  93, 222,  66, 110, 238,  14,  99, 178,  87
        };


        static readonly internal_layout layout_triphsxs = new internal_layout
        (
            569, layout_triphsxs_data.Length, internal_layout.compression.ZLIB, layout_triphsxs_data
        );
    }
}
