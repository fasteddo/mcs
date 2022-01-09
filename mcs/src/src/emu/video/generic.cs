// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.digfx_global;
using static mame.generic_global;


namespace mame
{
    static class generic_global
    {
        /***************************************************************************
            COMMON GRAPHICS LAYOUTS
        ***************************************************************************/

        //const gfx_layout gfx_8x8x1 =
        //{
        //    8,8,
        //    RGN_FRAC(1,1),
        //    1,
        //    { RGN_FRAC(0,1) },
        //    { STEP8(0,1) },
        //    { STEP8(0,8) },
        //    8*8
        //};

        public static readonly gfx_layout gfx_8x8x2_planar = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,2),
            2,
            ArrayCombineUInt32( RGN_FRAC(1,2), RGN_FRAC(0,2) ),
            ArrayCombineUInt32( STEP8(0,1) ),
            ArrayCombineUInt32( STEP8(0,8) ),
            8*8
        );
    }
}
