// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using PointerU8 = mame.Pointer<System.Byte>;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    public static partial class drawgfxt_global
    {
        /*-------------------------------------------------
            PIXEL_OP_COPY_OPAQUE - render all pixels
            regardless of pen, copying directly
        -------------------------------------------------*/
        //#define PIXEL_OP_COPY_OPAQUE(DEST, SOURCE)                                          \
        //do                                                                                  \
        //{                                                                                   \
        //    (DEST) = SOURCE;                                                                \
        //}                                                                                   \
        //while (0)
        public static void PIXEL_OP_COPY_OPAQUE(ref u8 DEST, u8 SOURCE) { DEST = SOURCE; }
        public static void PIXEL_OP_COPY_OPAQUE(ref u16 DEST, u8 SOURCE) { DEST = SOURCE; }
        public static void PIXEL_OP_COPY_OPAQUE(ref u16 DEST, u16 SOURCE) { DEST = SOURCE; }
        public static void PIXEL_OP_COPY_OPAQUE(ref u32 DEST, u8 SOURCE) { DEST = SOURCE; }
        public static void PIXEL_OP_COPY_OPAQUE(ref u32 DEST, u16 SOURCE) { DEST = SOURCE; }
        public static void PIXEL_OP_COPY_OPAQUE(ref u32 DEST, u32 SOURCE) { DEST = SOURCE; }


        /*-------------------------------------------------
            PIXEL_OP_COPY_TRANSPEN - render all pixels
            except those matching 'transpen', copying
            directly
        -------------------------------------------------*/
        //#define PIXEL_OP_COPY_TRANSPEN(DEST, SOURCE)                                        \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if (srcdata != trans_pen)                                                       \
        //        (DEST) = SOURCE;                                                            \
        //}                                                                                   \
        //while (0)
        public static void PIXEL_OP_COPY_TRANSPEN(u32 trans_pen, ref u8 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = SOURCE;
        }
        public static void PIXEL_OP_COPY_TRANSPEN(u32 trans_pen, ref u16 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = SOURCE;
        }
        public static void PIXEL_OP_COPY_TRANSPEN(u32 trans_pen, ref u16 DEST, u16 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = SOURCE;
        }
        public static void PIXEL_OP_COPY_TRANSPEN(u32 trans_pen, ref u32 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = SOURCE;
        }
        public static void PIXEL_OP_COPY_TRANSPEN(u32 trans_pen, ref u32 DEST, u16 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = SOURCE;
        }
        public static void PIXEL_OP_COPY_TRANSPEN(u32 trans_pen, ref u32 DEST, u32 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = SOURCE;
        }


        /*-------------------------------------------------
            PIXEL_OP_REBASE_OPAQUE - render all pixels
            regardless of pen, adding 'color' to the
            pen value
        -------------------------------------------------*/
        //#define PIXEL_OP_REBASE_OPAQUE(DEST, SOURCE)                                        \
        //do                                                                                  \
        //{                                                                                   \
        //    (DEST) = color + (SOURCE);                                                      \
        //}                                                                                   \
        //while (0)
        public static void PIXEL_OP_REBASE_OPAQUE(u32 color, ref u8 DEST, u8 SOURCE) { DEST = (u8)(color + SOURCE); }
        public static void PIXEL_OP_REBASE_OPAQUE(u32 color, ref u16 DEST, u8 SOURCE) { DEST = (u16)(color + SOURCE); }
        public static void PIXEL_OP_REBASE_OPAQUE(u32 color, ref u16 DEST, u16 SOURCE) { DEST = (u16)(color + SOURCE); }
        public static void PIXEL_OP_REBASE_OPAQUE(u32 color, ref u32 DEST, u8 SOURCE) { DEST = color + SOURCE; }
        public static void PIXEL_OP_REBASE_OPAQUE(u32 color, ref u32 DEST, u16 SOURCE) { DEST = color + SOURCE; }
        public static void PIXEL_OP_REBASE_OPAQUE(u32 color, ref u32 DEST, u32 SOURCE) { DEST = color + SOURCE; }


        /*-------------------------------------------------
            PIXEL_OP_REMAP_TRANSPEN - render all pixels
            except those matching 'trans_pen', mapping the
            pen via the 'paldata' array
        -------------------------------------------------*/
        //#define PIXEL_OP_REMAP_TRANSPEN(DEST, SOURCE)                                       \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if (srcdata != trans_pen)                                                       \
        //        (DEST) = paldata[srcdata];                                                  \
        //}                                                                                   \
        //while (0)
        public static void PIXEL_OP_REMAP_TRANSPEN(u32 trans_pen, Pointer<rgb_t> paldata, ref u32 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = paldata[srcdata];
        }
        public static void PIXEL_OP_REMAP_TRANSPEN(u32 trans_pen, Pointer<rgb_t> paldata, ref u32 DEST, u16 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = paldata[srcdata];
        }
        public static void PIXEL_OP_REMAP_TRANSPEN(u32 trans_pen, Pointer<rgb_t> paldata, ref u32 DEST, u32 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = paldata[srcdata];
        }


        /*-------------------------------------------------
            PIXEL_OP_REBASE_TRANSPEN - render all pixels
            except those matching 'transpen', adding
            'color' to the pen value
        -------------------------------------------------*/
        //#define PIXEL_OP_REBASE_TRANSPEN(DEST, SOURCE)                                      \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if (srcdata != trans_pen)                                                       \
        //        (DEST) = color + srcdata;                                                   \
        //}                                                                                   \
        //while (0)
        public static void PIXEL_OP_REBASE_TRANSPEN(u32 color, u32 trans_pen, ref u8 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = (u8)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSPEN(u32 color, u32 trans_pen, ref u16 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = (u16)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSPEN(u32 color, u32 trans_pen, ref u16 DEST, u16 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = (u16)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSPEN(u32 color, u32 trans_pen, ref u32 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = (u16)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSPEN(u32 color, u32 trans_pen, ref u32 DEST, u16 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = (u16)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSPEN(u32 color, u32 trans_pen, ref u32 DEST, u32 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = (u16)(color + srcdata);
        }


        /*-------------------------------------------------
            PIXEL_OP_REBASE_TRANSMASK - render all pixels
            except those matching 'trans_mask', adding
            'color' to the pen value
        -------------------------------------------------*/
        //#define PIXEL_OP_REBASE_TRANSMASK(DEST, SOURCE)                                     \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if (((trans_mask >> srcdata) & 1) == 0)                                         \
        //        (DEST) = color + srcdata;                                                   \
        //}                                                                                   \
        //while (0)
        public static void PIXEL_OP_REBASE_TRANSMASK(u32 color, u32 trans_mask, ref u8 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
                DEST = (u8)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSMASK(u32 color, u32 trans_mask, ref u16 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
                DEST = (u16)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSMASK(u32 color, u32 trans_mask, ref u16 DEST, u16 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
                DEST = (u16)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSMASK(u32 color, u32 trans_mask, ref u32 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
                DEST = color + srcdata;
        }
        public static void PIXEL_OP_REBASE_TRANSMASK(u32 color, u32 trans_mask, ref u32 DEST, u16 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
                DEST = color + srcdata;
        }
        public static void PIXEL_OP_REBASE_TRANSMASK(u32 color, u32 trans_mask, ref u32 DEST, u32 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
                DEST = color + srcdata;
        }
    }


    public partial class gfx_element : global_object
    {
        /***************************************************************************
            BASIC DRAWGFX CORE
        ***************************************************************************/

        /*
            Input parameters:

                bitmap_t &dest - the bitmap to render to
                const rectangle &cliprect - a clipping rectangle (assumed to be clipped to the size of 'dest')
                gfx_element *gfx - pointer to the gfx_element to render
                u32 code - index of the entry within gfx_element
                int flipx - non-zero means render right-to-left instead of left-to-right
                int flipy - non-zero means render bottom-to-top instead of top-to-bottom
                s32 destx - the top-left X coordinate to render to
                s32 desty - the top-left Y coordinate to render to
                bitmap_t &priority - the priority bitmap (if and only if priority is to be applied)
        */

        public class FunctionClass
        {
            public delegate void func8x8(ref u8 destp, u8 srcp);
            public delegate void func16x8(ref u16 destp, u8 srcp);
            public delegate void func16x16(ref u16 destp, u16 srcp);
            public delegate void func32x8(ref u32 destp, u8 srcp);
            public delegate void func32x16(ref u32 destp, u16 srcp);
            public delegate void func32x32(ref u32 destp, u32 srcp);

            func8x8   m_func8x8;
            func16x8  m_func16x8;
            func16x16 m_func16x16;
            func32x8  m_func32x8;
            func32x16 m_func32x16;
            func32x32 m_func32x32;

            public FunctionClass(func8x8 func) { m_func8x8 = func; }
            public FunctionClass(func16x8 func) { m_func16x8 = func; }
            public FunctionClass(func16x16 func) { m_func16x16 = func; }
            public FunctionClass(func32x8 func) { m_func32x8 = func; }
            public FunctionClass(func32x16 func) { m_func32x16 = func; }
            public FunctionClass(func32x32 func) { m_func32x32 = func; }

            public void op8x8(ref u8 destp, u8 srcp) { m_func8x8(ref destp, srcp); }
            public void op16x8(ref u16 destp, u8 srcp) { m_func16x8(ref destp, srcp); }
            public void op16x16(ref u16 destp, u16 srcp) { m_func16x16(ref destp, srcp); }
            public void op32x8(ref u32 destp, u8 srcp) { m_func32x8(ref destp, srcp); }
            public void op32x16(ref u32 destp, u16 srcp) { m_func32x16(ref destp, srcp); }
            public void op32x32(ref u32 destp, u32 srcp) { m_func32x32(ref destp, srcp); }
        }


        //template <typename BitmapType, typename FunctionClass>
        //inline void gfx_element::drawgfx_core(BitmapType &dest, const rectangle &cliprect, u32 code, int flipx, int flipy, s32 destx, s32 desty, FunctionClass pixel_op)
        void drawgfx_core<BitmapType, BitmapType_PixelType, BitmapType_PixelType_OPS, BitmapType_PixelTypePointer>
        (
            BitmapType dest,
            rectangle cliprect,
            u32 code,
            int flipx,
            int flipy,
            s32 destx,
            s32 desty,
            FunctionClass pixel_op
        )
            where BitmapType : bitmap_specific<BitmapType_PixelType, BitmapType_PixelType_OPS, BitmapType_PixelTypePointer> 
            where BitmapType_PixelType_OPS : PixelType_operators, new()
            where BitmapType_PixelTypePointer : PointerU8
        {
            profiler_global.g_profiler.start(profile_type.PROFILER_DRAWGFX);


            do {
                assert(dest.valid());
                assert(dest.cliprect().contains(cliprect));
                assert(code < elements());

                // ignore empty/invalid cliprects
                if (cliprect.empty())
                    break;

                // compute final pixel in X and exit if we are entirely clipped
                s32 destendx = destx + width() - 1;
                if (destx > cliprect.right() || destendx < cliprect.left())
                    break;

                // apply left clip
                s32 srcx = 0;
                if (destx < cliprect.left())
                {
                    srcx = cliprect.left() - destx;
                    destx = cliprect.left();
                }

                // apply right clip
                if (destendx > cliprect.right())
                    destendx = cliprect.right();

                // compute final pixel in Y and exit if we are entirely clipped
                s32 destendy = desty + height() - 1;
                if (desty > cliprect.bottom() || destendy < cliprect.top())
                    break;

                // apply top clip
                s32 srcy = 0;
                if (desty < cliprect.top())
                {
                    srcy = cliprect.top() - desty;
                    desty = cliprect.top();
                }

                // apply bottom clip
                if (destendy > cliprect.bottom())
                    destendy = cliprect.bottom();

                // apply X flipping
                if (flipx != 0)
                    srcx = width() - 1 - srcx;

                // apply Y flipping
                s32 dy = (s32)rowbytes();
                if (flipy != 0)
                {
                    srcy = height() - 1 - srcy;
                    dy = -dy;
                }

                // fetch the source data
                PointerU8 srcdata = get_data(code);  //const u8 *srcdata = get_data(code);

                // compute how many blocks of 4 pixels we have
                u32 numblocks = (u32)((destendx + 1 - destx) / 4);
                u32 leftovers = (u32)((destendx + 1 - destx) - 4 * numblocks);

                // adjust srcdata to point to the first source pixel of the row
                srcdata += srcy * (int)rowbytes() + srcx;

                // non-flipped 8bpp case
                if (flipx == 0)
                {
                    // iterate over pixels in Y
                    for (s32 cury = desty; cury <= destendy; cury++)
                    {
                        //auto *destptr = &dest.pix(cury, destx);
                        PointerU8 destptr8 = null;
                        PointerU16 destptr16 = null;
                        PointerU32 destptr32 = null;
                        PointerU64 destptr64 = null;
                        switch (dest.bpp())
                        {
                            case 8:  destptr8 = dest.pix8(cury, destx); break;
                            case 16: destptr16 = dest.pix16(cury, destx); break;
                            case 32: destptr32 = dest.pix32(cury, destx); break;
                            case 64: destptr64 = dest.pix64(cury, destx); break;
                            default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", dest.bpp());
                        }

                        PointerU8 srcptr = new PointerU8(srcdata);  //const u8 *srcptr = srcdata;
                        srcdata += dy;

                        // iterate over unrolled blocks of 4
                        for (s32 curx = 0; curx < numblocks; curx++)
                        {
                            //pixel_op(destptr[0], srcptr[0]);
                            //pixel_op(destptr[1], srcptr[1]);
                            //pixel_op(destptr[2], srcptr[2]);
                            //pixel_op(destptr[3], srcptr[3]);
                            switch (dest.bpp())
                            {
                                case 8:
                                {
                                    var destptrTemp0 = destptr8[0]; pixel_op.op8x8(ref destptrTemp0, srcptr[0]); destptr8[0] = destptrTemp0;
                                    var destptrTemp1 = destptr8[1]; pixel_op.op8x8(ref destptrTemp1, srcptr[1]); destptr8[1] = destptrTemp1;
                                    var destptrTemp2 = destptr8[2]; pixel_op.op8x8(ref destptrTemp2, srcptr[2]); destptr8[2] = destptrTemp2;
                                    var destptrTemp3 = destptr8[3]; pixel_op.op8x8(ref destptrTemp3, srcptr[3]); destptr8[3] = destptrTemp3;
                                    break;
                                }
                                case 16:
                                {
                                    var destptrTemp0 = destptr16[0]; pixel_op.op16x8(ref destptrTemp0, srcptr[0]); destptr16[0] = destptrTemp0;
                                    var destptrTemp1 = destptr16[1]; pixel_op.op16x8(ref destptrTemp1, srcptr[1]); destptr16[1] = destptrTemp1;
                                    var destptrTemp2 = destptr16[2]; pixel_op.op16x8(ref destptrTemp2, srcptr[2]); destptr16[2] = destptrTemp2;
                                    var destptrTemp3 = destptr16[3]; pixel_op.op16x8(ref destptrTemp3, srcptr[3]); destptr16[3] = destptrTemp3;
                                    break;
                                }
                                case 32:
                                {
                                    var destptrTemp0 = destptr32[0]; pixel_op.op32x8(ref destptrTemp0, srcptr[0]); destptr32[0] = destptrTemp0;
                                    var destptrTemp1 = destptr32[1]; pixel_op.op32x8(ref destptrTemp1, srcptr[1]); destptr32[1] = destptrTemp1;
                                    var destptrTemp2 = destptr32[2]; pixel_op.op32x8(ref destptrTemp2, srcptr[2]); destptr32[2] = destptrTemp2;
                                    var destptrTemp3 = destptr32[3]; pixel_op.op32x8(ref destptrTemp3, srcptr[3]); destptr32[3] = destptrTemp3;
                                    break;
                                }
                                default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", dest.bpp());
                            }

                            srcptr += 4;

                            //destptr += 4;
                            switch (dest.bpp())
                            {
                                case 8:  destptr8 += 4; break;
                                case 16: destptr16 += 4; break;
                                case 32: destptr32 += 4; break;
                                case 64: destptr64 += 4; break;
                                default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", dest.bpp());
                            }
                        }

                        // iterate over leftover pixels
                        for (s32 curx = 0; curx < leftovers; curx++)
                        {
                            //pixel_op(destptr[0], srcptr[0]);
                            switch (dest.bpp())
                            {
                                case 8:  { var destptrTemp = destptr8[0]; pixel_op.op8x8(ref destptrTemp, srcptr[0]); destptr8[0] = destptrTemp; break; }
                                case 16: { var destptrTemp = destptr16[0]; pixel_op.op16x8(ref destptrTemp, srcptr[0]); destptr16[0] = destptrTemp; break; }
                                case 32: { var destptrTemp = destptr32[0]; pixel_op.op32x8(ref destptrTemp, srcptr[0]); destptr32[0] = destptrTemp; break; }
                                default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", dest.bpp());
                            }

                            srcptr++;

                            //destptr++;
                            switch (dest.bpp())
                            {
                                case 8:  destptr8++; break;
                                case 16: destptr16++; break;
                                case 32: destptr32++; break;
                                case 64: destptr64++; break;
                                default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", dest.bpp());
                            }
                        }
                    }
                }

                // flipped 8bpp case
                else
                {
                    // iterate over pixels in Y
                    for (s32 cury = desty; cury <= destendy; cury++)
                    {
                        //auto *destptr = &dest.pix(cury, destx);
                        PointerU8 destptr8 = null;
                        PointerU16 destptr16 = null;
                        PointerU32 destptr32 = null;
                        PointerU64 destptr64 = null;
                        switch (dest.bpp())
                        {
                            case 8:  destptr8 = dest.pix8(cury, destx); break;
                            case 16: destptr16 = dest.pix16(cury, destx); break;
                            case 32: destptr32 = dest.pix32(cury, destx); break;
                            case 64: destptr64 = dest.pix64(cury, destx); break;
                            default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", dest.bpp());
                        }

                        PointerU8 srcptr = new PointerU8(srcdata);  //const u8 *srcptr = srcdata;
                        srcdata += dy;

                        // iterate over unrolled blocks of 4
                        for (s32 curx = 0; curx < numblocks; curx++)
                        {
                            //pixel_op(destptr[0], srcptr[ 0]);
                            //pixel_op(destptr[1], srcptr[-1]);
                            //pixel_op(destptr[2], srcptr[-2]);
                            //pixel_op(destptr[3], srcptr[-3]);
                            switch (dest.bpp())
                            {
                                case 8:
                                {
                                    var destptrTemp0 = destptr8[0]; pixel_op.op8x8(ref destptrTemp0, srcptr[ 0]); destptr8[0] = destptrTemp0;
                                    var destptrTemp1 = destptr8[1]; pixel_op.op8x8(ref destptrTemp1, srcptr[-1]); destptr8[1] = destptrTemp1;
                                    var destptrTemp2 = destptr8[2]; pixel_op.op8x8(ref destptrTemp2, srcptr[-2]); destptr8[2] = destptrTemp2;
                                    var destptrTemp3 = destptr8[3]; pixel_op.op8x8(ref destptrTemp3, srcptr[-3]); destptr8[3] = destptrTemp3;
                                    break;
                                }
                                case 16:
                                {
                                    var destptrTemp0 = destptr16[0]; pixel_op.op16x8(ref destptrTemp0, srcptr[ 0]); destptr16[0] = destptrTemp0;
                                    var destptrTemp1 = destptr16[1]; pixel_op.op16x8(ref destptrTemp1, srcptr[-1]); destptr16[1] = destptrTemp1;
                                    var destptrTemp2 = destptr16[2]; pixel_op.op16x8(ref destptrTemp2, srcptr[-2]); destptr16[2] = destptrTemp2;
                                    var destptrTemp3 = destptr16[3]; pixel_op.op16x8(ref destptrTemp3, srcptr[-3]); destptr16[3] = destptrTemp3;
                                    break;
                                }
                                default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", dest.bpp());
                            }

                            srcptr -= 4;

                            //destptr += 4;
                            switch (dest.bpp())
                            {
                                case 8:  destptr8 += 4; break;
                                case 16: destptr16 += 4; break;
                                case 32: destptr32 += 4; break;
                                case 64: destptr64 += 4; break;
                                default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", dest.bpp());
                            }
                        }

                        // iterate over leftover pixels
                        for (s32 curx = 0; curx < leftovers; curx++)
                        {
                            //pixel_op(destptr[0], srcptr[0]);
                            switch (dest.bpp())
                            {
                                case 8:  { var destptrTemp = destptr8[0]; pixel_op.op8x8(ref destptrTemp, srcptr[0]); destptr8[0] = destptrTemp; break; }
                                case 16: { var destptrTemp = destptr16[0]; pixel_op.op16x8(ref destptrTemp, srcptr[0]); destptr16[0] = destptrTemp; break; }
                                default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", dest.bpp());
                            }

                            srcptr--;

                            //destptr++;
                            switch (dest.bpp())
                            {
                                case 8:  destptr8++; break;
                                case 16: destptr16++; break;
                                case 32: destptr32++; break;
                                case 64: destptr64++; break;
                                default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", dest.bpp());
                            }
                        }
                    }
                }
            } while (false);

            profiler_global.g_profiler.stop();
        }


        //template <typename BitmapType, typename PriorityType, typename FunctionClass>
        //inline void gfx_element::drawgfx_core(BitmapType &dest, const rectangle &cliprect, u32 code, int flipx, int flipy, s32 destx, s32 desty, PriorityType &priority, FunctionClass pixel_op)
        void drawgfx_core<BitmapType, BitmapType_PixelType, BitmapType_PixelType_OPS, BitmapType_PixelTypePointer, PriorityType>
        (
            BitmapType dest,
            rectangle cliprect,
            u32 code,
            int flipx,
            int flipy,
            s32 destx,
            s32 desty,
            PriorityType priority,
            FunctionClass pixel_op
        )
            where BitmapType : bitmap_specific<BitmapType_PixelType, BitmapType_PixelType_OPS, BitmapType_PixelTypePointer> 
            where BitmapType_PixelType_OPS : PixelType_operators, new()
            where BitmapType_PixelTypePointer : PointerU8
        {
            throw new emu_unimplemented();
        }
    }


    public static partial class drawgfxt_global
    {
        /***************************************************************************
            BASIC COPYBITMAP CORE
        ***************************************************************************/

        /*
            Input parameters:

                bitmap_t &dest - the bitmap to copy to
                bitmap_t &src - the bitmap to copy from (must be same bpp as dest)
                const rectangle &cliprect - a clipping rectangle (assumed to be clipped to the size of 'dest')
                int flipx - non-zero means render right-to-left instead of left-to-right
                int flipy - non-zero means render bottom-to-top instead of top-to-bottom
                s32 destx - the top-left X coordinate to copy to
                s32 desty - the top-left Y coordinate to copy to
                bitmap_t &priority - the priority bitmap (if and only if priority is to be applied)
        */

        //template <typename BitmapType, typename FunctionClass>
        //inline void copybitmap_core(BitmapType &dest, const BitmapType &src, int flipx, int flipy, s32 destx, s32 desty, const rectangle &cliprect, FunctionClass pixel_op)
        public static void copybitmap_core<BitmapType, BitmapType_PixelType, BitmapType_PixelType_OPS, BitmapType_PixelTypePointer>
        (
            BitmapType dest,
            BitmapType src,
            int flipx,
            int flipy,
            s32 destx,
            s32 desty,
            rectangle cliprect,
            gfx_element.FunctionClass pixel_op
        )
            where BitmapType : bitmap_specific<BitmapType_PixelType, BitmapType_PixelType_OPS, BitmapType_PixelTypePointer> 
            where BitmapType_PixelType_OPS : PixelType_operators, new()
            where BitmapType_PixelTypePointer : PointerU8
        {
            profiler_global.g_profiler.start(profile_type.PROFILER_COPYBITMAP);

            do {
                global_object.assert(dest.valid());
                global_object.assert(src.valid());
                global_object.assert(dest.cliprect().contains(cliprect));

                // ignore empty/invalid cliprects
                if (cliprect.empty())
                    break;

                // standard setup; dx counts bytes in X, dy counts pixels in Y
                s32 dx = 1;
                s32 dy = src.rowpixels();

                // compute final pixel in X and exit if we are entirely clipped
                s32 destendx = destx + src.width() - 1;
                if (destx > cliprect.right() || destendx < cliprect.left())
                    break;

                // apply left clip
                s32 srcx = 0;
                if (destx < cliprect.left())
                {
                    srcx = cliprect.left() - destx;
                    destx = cliprect.left();
                }

                // apply right clip
                if (destendx > cliprect.right())
                    destendx = cliprect.right();

                // compute final pixel in Y and exit if we are entirely clipped
                s32 destendy = desty + src.height() - 1;
                if (desty > cliprect.bottom() || destendy < cliprect.top())
                    break;

                // apply top clip
                s32 srcy = 0;
                if (desty < cliprect.top())
                {
                    srcy = cliprect.top() - desty;
                    desty = cliprect.top();
                }

                // apply bottom clip
                if (destendy > cliprect.bottom())
                    destendy = cliprect.bottom();

                // apply X flipping
                if (flipx != 0)
                {
                    srcx = src.width() - 1 - srcx;
                    dx = -dx;
                }

                // apply Y flipping
                if (flipy != 0)
                {
                    srcy = src.height() - 1 - srcy;
                    dy = -dy;
                }

                // compute how many blocks of 4 pixels we have
                u32 numblocks = (u32)((destendx + 1 - destx) / 4);
                u32 leftovers = (u32)((destendx + 1 - destx) - 4 * numblocks);

                // compute the address of the first source pixel of the first row
                //const auto *srcdata = &src.pix(srcy, srcx);
                PointerU8 srcdata8 = null;
                PointerU16 srcdata16 = null;
                PointerU32 srcdata32 = null;
                PointerU64 srcdata64 = null;
                switch (src.bpp())
                {
                    case 8:  srcdata8 = src.pix8(srcy, srcx); break;
                    case 16: srcdata16 = src.pix16(srcy, srcx); break;
                    case 32: srcdata32 = src.pix32(srcy, srcx); break;
                    case 64: srcdata64 = src.pix64(srcy, srcx); break;
                    default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", src.bpp());
                }

                // non-flipped case
                if (flipx == 0)
                {
                    // iterate over pixels in Y
                    for (s32 cury = desty; cury <= destendy; cury++)
                    {
                        //auto *destptr = &dest.pix(cury, destx);
                        PointerU8 destptr8 = null;
                        PointerU16 destptr16 = null;
                        PointerU32 destptr32 = null;
                        PointerU64 destptr64 = null;
                        switch (dest.bpp())
                        {
                            case 8:  destptr8 = dest.pix8(cury, destx); break;
                            case 16: destptr16 = dest.pix16(cury, destx); break;
                            case 32: destptr32 = dest.pix32(cury, destx); break;
                            case 64: destptr64 = dest.pix64(cury, destx); break;
                            default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", dest.bpp());
                        }

                        //const auto *srcptr = srcdata;
                        PointerU8 srcptr8 = null;
                        PointerU16 srcptr16 = null;
                        PointerU32 srcptr32 = null;
                        PointerU64 srcptr64 = null;
                        switch (src.bpp())
                        {
                            case 8:  srcptr8 = new PointerU8(srcdata8); break;
                            case 16: srcptr16 = new PointerU16(srcdata16); break;
                            case 32: srcptr32 = new PointerU32(srcdata32); break;
                            case 64: srcptr64 = new PointerU64(srcdata64); break;
                            default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", src.bpp());
                        }

                        //srcdata += dy;
                        switch (src.bpp())
                        {
                            case 8:  srcdata8 += dy; break;
                            case 16: srcdata16 += dy; break;
                            case 32: srcdata32 += dy; break;
                            case 64: srcdata64 += dy; break;
                            default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", src.bpp());
                        }

                        // iterate over unrolled blocks of 4
                        for (s32 curx = 0; curx < numblocks; curx++)
                        {
                            //pixel_op(destptr[0], srcptr[0]);
                            //pixel_op(destptr[1], srcptr[1]);
                            //pixel_op(destptr[2], srcptr[2]);
                            //pixel_op(destptr[3], srcptr[3]);
                            if (dest.bpp() == 8 && src.bpp() == 8)
                            {
                                var destptrTemp0 = destptr8[0]; pixel_op.op8x8(ref destptrTemp0, srcptr8[0]); destptr8[0] = destptrTemp0;
                                var destptrTemp1 = destptr8[1]; pixel_op.op8x8(ref destptrTemp1, srcptr8[1]); destptr8[1] = destptrTemp1;
                                var destptrTemp2 = destptr8[2]; pixel_op.op8x8(ref destptrTemp2, srcptr8[2]); destptr8[2] = destptrTemp2;
                                var destptrTemp3 = destptr8[3]; pixel_op.op8x8(ref destptrTemp3, srcptr8[3]); destptr8[3] = destptrTemp3;
                            }
                            else throw new emu_fatalerror("copybitmap_core() - unknown bpp - dest: {0} src: {1}\n", dest.bpp(), src.bpp());

                            //srcptr += 4;
                            switch (src.bpp())
                            {
                                case 8:  srcptr8 += 4; break;
                                case 16: srcptr16 += 4; break;
                                case 32: srcptr32 += 4; break;
                                case 64: srcptr64 += 4; break;
                                default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", src.bpp());
                            }

                            //destptr += 4;
                            switch (dest.bpp())
                            {
                                case 8:  destptr8 += 4; break;
                                case 16: destptr16 += 4; break;
                                case 32: destptr32 += 4; break;
                                case 64: destptr64 += 4; break;
                                default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", dest.bpp());
                            }
                        }

                        // iterate over leftover pixels
                        for (s32 curx = 0; curx < leftovers; curx++)
                        {
                            //pixel_op(destptr[0], srcptr[0]);
                            if (dest.bpp() == 8 && src.bpp() == 8)
                            {
                                var destptrTemp = destptr8[0]; pixel_op.op8x8(ref destptrTemp, srcptr8[0]); destptr8[0] = destptrTemp;
                            }
                            else throw new emu_fatalerror("copybitmap_core() - unknown bpp - dest: {0} src: {1}\n", dest.bpp(), src.bpp());

                            //srcptr++;
                            switch (src.bpp())
                            {
                                case 8:  srcptr8++; break;
                                case 16: srcptr16++; break;
                                case 32: srcptr32++; break;
                                case 64: srcptr64++; break;
                                default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", src.bpp());
                            }

                            //destptr++;
                            switch (dest.bpp())
                            {
                                case 8:  destptr8++; break;
                                case 16: destptr16++; break;
                                case 32: destptr32++; break;
                                case 64: destptr64++; break;
                                default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", dest.bpp());
                            }
                        }
                    }
                }

                // flipped case
                else
                {
                    // iterate over pixels in Y
                    for (s32 cury = desty; cury <= destendy; cury++)
                    {
                        //auto *destptr = &dest.pix(cury, destx);
                        PointerU8 destptr8 = null;
                        PointerU16 destptr16 = null;
                        PointerU32 destptr32 = null;
                        PointerU64 destptr64 = null;
                        switch (dest.bpp())
                        {
                            case 8:  destptr8 = dest.pix8(cury, destx); break;
                            case 16: destptr16 = dest.pix16(cury, destx); break;
                            case 32: destptr32 = dest.pix32(cury, destx); break;
                            case 64: destptr64 = dest.pix64(cury, destx); break;
                            default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", dest.bpp());
                        }

                        //const auto *srcptr = srcdata;
                        PointerU8 srcptr8 = null;
                        PointerU16 srcptr16 = null;
                        PointerU32 srcptr32 = null;
                        PointerU64 srcptr64 = null;
                        switch (src.bpp())
                        {
                            case 8:  srcptr8 = new PointerU8(srcdata8); break;
                            case 16: srcptr16 = new PointerU16(srcdata16); break;
                            case 32: srcptr32 = new PointerU32(srcdata32); break;
                            case 64: srcptr64 = new PointerU64(srcdata64); break;
                            default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", src.bpp());
                        }

                        //srcdata += dy;
                        switch (src.bpp())
                        {
                            case 8:  srcdata8 += dy; break;
                            case 16: srcdata16 += dy; break;
                            case 32: srcdata32 += dy; break;
                            case 64: srcdata64 += dy; break;
                            default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", src.bpp());
                        }

                        // iterate over unrolled blocks of 4
                        for (s32 curx = 0; curx < numblocks; curx++)
                        {
                            //pixel_op(destptr[0], srcptr[ 0]);
                            //pixel_op(destptr[1], srcptr[-1]);
                            //pixel_op(destptr[2], srcptr[-2]);
                            //pixel_op(destptr[3], srcptr[-3]);
                            if (dest.bpp() == 8 && src.bpp() == 8)
                            {
                                var destptrTemp0 = destptr8[0]; pixel_op.op8x8(ref destptrTemp0, srcptr8[ 0]); destptr8[0] = destptrTemp0;
                                var destptrTemp1 = destptr8[1]; pixel_op.op8x8(ref destptrTemp1, srcptr8[-1]); destptr8[1] = destptrTemp1;
                                var destptrTemp2 = destptr8[2]; pixel_op.op8x8(ref destptrTemp2, srcptr8[-2]); destptr8[2] = destptrTemp2;
                                var destptrTemp3 = destptr8[3]; pixel_op.op8x8(ref destptrTemp3, srcptr8[-3]); destptr8[3] = destptrTemp3;
                            }
                            else throw new emu_fatalerror("copybitmap_core() - unknown bpp - dest: {0} src: {1}\n", dest.bpp(), src.bpp());

                            //srcptr -= 4;
                            switch (src.bpp())
                            {
                                case 8:  srcptr8 -= 4; break;
                                case 16: srcptr16 -= 4; break;
                                case 32: srcptr32 -= 4; break;
                                case 64: srcptr64 -= 4; break;
                                default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", src.bpp());
                            }

                            //destptr += 4;
                            switch (dest.bpp())
                            {
                                case 8:  destptr8 += 4; break;
                                case 16: destptr16 += 4; break;
                                case 32: destptr32 += 4; break;
                                case 64: destptr64 += 4; break;
                                default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", dest.bpp());
                            }
                        }

                        // iterate over leftover pixels
                        for (s32 curx = 0; curx < leftovers; curx++)
                        {
                            //pixel_op(destptr[0], srcptr[0]);
                            if (dest.bpp() == 8 && src.bpp() == 8)
                            {
                                var destptrTemp = destptr8[0]; pixel_op.op8x8(ref destptrTemp, srcptr8[0]); destptr8[0] = destptrTemp;
                            }
                            else throw new emu_fatalerror("copybitmap_core() - unknown bpp - dest: {0} src: {1}\n", dest.bpp(), src.bpp());

                            //srcptr--;
                            switch (src.bpp())
                            {
                                case 8:  srcptr8--; break;
                                case 16: srcptr16--; break;
                                case 32: srcptr32--; break;
                                case 64: srcptr64--; break;
                                default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", src.bpp());
                            }

                            //destptr++;
                            switch (dest.bpp())
                            {
                                case 8:  destptr8++; break;
                                case 16: destptr16++; break;
                                case 32: destptr32++; break;
                                case 64: destptr64++; break;
                                default: throw new emu_fatalerror("copybitmap_core() - unknown bpp - {0}\n", dest.bpp());
                            }
                        }
                    }
                }
            } while (false);

            profiler_global.g_profiler.stop();
        }


        //template <typename BitmapType, typename PriorityType, typename FunctionClass>
        //inline void copybitmap_core(BitmapType &dest, const BitmapType &src, int flipx, int flipy, s32 destx, s32 desty, const rectangle &cliprect, PriorityType &priority, FunctionClass pixel_op)
        public static void copybitmap_core<BitmapType, BitmapType_PixelType, BitmapType_PixelType_OPS, BitmapType_PixelTypePointer, PriorityType>
        (
            BitmapType dest,
            BitmapType src,
            int flipx,
            int flipy,
            s32 destx,
            s32 desty,
            rectangle cliprect,
            PriorityType priority,
            gfx_element.FunctionClass pixel_op
        )
            where BitmapType : bitmap_specific<BitmapType_PixelType, BitmapType_PixelType_OPS, BitmapType_PixelTypePointer> 
            where BitmapType_PixelType_OPS : PixelType_operators, new()
            where BitmapType_PixelTypePointer : PointerU8
        {
            throw new emu_unimplemented();
        }
    }
}
