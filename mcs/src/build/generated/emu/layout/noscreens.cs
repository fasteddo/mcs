// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class noscreens_global
    {
        static readonly byte [] layout_noscreens_data =
        {
            120, 156, 109, 144, 221, 206, 194,  32,  12,  64, 239, 125, 138, 134,   7, 240,  55,  24,  47,  24, 190, 193, 247,  10, 134, 109, 117,  91,   2, 244,  11, 116,
             58, 125, 122,  59, 157, 113,  38,  94, 181, 165, 112,  78, 139,  57,  14, 193, 195,   5,  83, 238,  40,  22, 106, 179,  92, 171, 163,  53, 193,   5, 244, 238,
             70,  61, 127,  90,  91, 101,  13, 122,  12,  24,  25, 162, 244,  11, 117, 117,  41, 118, 177, 145, 115, 198, 129,  33, 115, 146, 170,  80, 127,   4, 185,  74,
            136,  49, 131,  99, 118,  85, 139,  53,  48,   1, 183,   8, 249, 150,  25, 131,  60, 168, 200,  83, 130, 210, 247, 130,  89,  47, 181, 130, 102, 124,  48, 229,
              9, 235, 215,  36,  43, 107,  86,  35,  90, 194, 100, 182, 230, 210, 225, 117, 242, 255,  16,   9, 186, 164,  62, 214,  89, 140, 255, 130,  19,  88, 215, 180,
             44, 211,  31,  36,  47, 137, 153, 130, 176, 181,  20,  30, 207,  60, 222,  16,  73, 137, 119, 244, 223,  75, 157, 120,  96,   5, 147, 117, 190, 234,  28, 175,
            103, 252, 221, 140, 191, 215, 111, 188, 126,  45, 241,  20,  72,  28, 135, 151, 240, 249,  94, 187, 120,   0,  25,  77, 126, 181
        };

        static readonly internal_layout layout_noscreens = new internal_layout
        (
            385, layout_noscreens_data.Length, 1, layout_noscreens_data
        );


        public static string layout_noscreens_string()
        {
            string s = "";
            foreach (var c in layout_noscreens.data)
                s += Convert.ToChar(c);

            return s;
        }
    }
}
