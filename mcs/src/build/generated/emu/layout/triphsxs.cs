// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class triphsxs_global
    {
        static readonly byte [] layout_triphsxs_data =
        {
            120, 156, 189, 146,  77,  10, 194,  64,  12, 133, 247, 158,  34, 100, 165, 139, 118, 250,  35, 226,  98, 102, 186, 244,   0, 122, 129, 214,   9, 118, 160, 157,
             74, 167, 191, 183, 119, 170, 139, 130,  20, 170,  32, 174,  66,  94, 146,  15, 222,  35,  60,  25, 202,   2,  58, 170, 173, 174, 140, 192, 208,  15,  48, 145,
            188,  76,  75,  42, 210, 177, 106, 155, 121,  20, 161, 228, 157, 166,  30, 140,  27,  10, 188, 212, 250,  94,  16, 156, 181,  34,  47,  27, 189, 169, 186,   5,
            123, 173, 137,  12, 104, 163, 104,  16,  24,  56,  37, 171,  90, 163,  44, 140,  83,   7,  79,  13, 114, 210, 183, 188,  17,  24,  35, 244,  90,  53, 185, 192,
             61,  50, 201, 217, 235, 248,  29,  18,  46,  64, 246, 126,  16, 127, 203, 137,  22,  56,  71,  63,  56, 172, 115, 216, 100, 123, 205,  60, 108,  79, 169, 211,
            172, 221, 253,  49, 134, 159, 100, 240, 113,   0, 108, 126,  11, 185, 121,   0,  17, 183, 178,  87
        };


        static readonly internal_layout layout_triphsxs = new internal_layout
        (
            569, layout_triphsxs_data.Length, internal_layout.compression.ZLIB, layout_triphsxs_data
        );
    }
}
