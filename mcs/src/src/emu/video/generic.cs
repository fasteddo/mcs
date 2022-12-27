// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u32 = System.UInt32;

using static mame.digfx_global;
using static mame.generic_global;


namespace mame
{
    public static class generic_global
    {
        /***************************************************************************
            COMMON GRAPHICS LAYOUTS
        ***************************************************************************/

        public static readonly gfx_layout gfx_8x8x1 = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,1),
            1,
            new u32 [] { RGN_FRAC(0,1) },
            STEP8(0,1),
            STEP8(0,8),
            8*8
        );


        public static readonly gfx_layout gfx_8x8x2_planar = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,2),
            2,
            new u32 [] { RGN_FRAC(1,2), RGN_FRAC(0,2) },
            STEP8(0,1),
            STEP8(0,8),
            8*8
        );


        public static readonly gfx_layout gfx_8x8x3_planar = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,3),
            3,
            new u32 [] { RGN_FRAC(2,3), RGN_FRAC(1,3), RGN_FRAC(0,3) },
            STEP8(0,1),
            STEP8(0,8),
            8*8
        );


        //const gfx_layout gfx_8x8x4_planar =

        //const gfx_layout gfx_8x8x5_planar =

        //const gfx_layout gfx_8x8x6_planar =

        //const gfx_layout gfx_16x16x4_planar =


        // packed gfxs; msb and lsb is start nibble of packed pixel byte
        public static readonly gfx_layout gfx_8x8x4_packed_msb = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,1),
            4,
            STEP4(0,1),
            STEP8(0,4), // x order : hi nibble first, low nibble second
            STEP8(0,4*8),
            8*8*4
        );


        //const gfx_layout gfx_8x8x4_packed_lsb =

        //GFXLAYOUT_RAW(gfx_8x8x8_raw, 8, 8, 8*8, 8*8*8);

        //const gfx_layout gfx_16x16x4_packed_msb =

        //const gfx_layout gfx_16x16x4_packed_lsb =

        //GFXLAYOUT_RAW(gfx_16x16x8_raw, 16, 16, 16*8, 16*16*8);

        //const gfx_layout gfx_8x8x4_row_2x2_group_packed_msb =

        //const gfx_layout gfx_8x8x4_row_2x2_group_packed_lsb =

        //const gfx_layout gfx_8x8x4_col_2x2_group_packed_msb =

        //const gfx_layout gfx_8x8x4_col_2x2_group_packed_lsb =
    }
}
