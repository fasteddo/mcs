// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;


namespace mame
{
    public delegate void PIXEL_OP(RawBufferPointer DEST, ListBytesPointer PRIORITY, u8 SOURCE, u32 color, u32 trans_mask, u32 trans_pen, ListPointer<rgb_t> paldata, int PIXEL_TYPE_SIZE);

    public delegate ListBytesPointer get_data_delegate(UInt32 code);


    public static class drawgfxm_global
    {
        /* special priority type meaning "none" */
        //struct NO_PRIORITY { char dummy[3]; };
        public class NO_PRIORITY { char [] dummy = new char[3]; }

        public static bitmap_ind8 drawgfx_dummy_priority_bitmap = new bitmap_ind8();
        public static bitmap_t DECLARE_NO_PRIORITY { get { return drawgfx_dummy_priority_bitmap; } }  //#define DECLARE_NO_PRIORITY bitmap_t &priority = drawgfx_dummy_priority_bitmap;


        /* macros for using the optional priority */

        //#define PRIORITY_VALID(x)       (sizeof(x) != sizeof(NO_PRIORITY))
        static bool PRIORITY_VALID<PRIORITY_TYPE>() { return typeof(PRIORITY_TYPE) != typeof(NO_PRIORITY); }

        //PRIORITY_TYPE *priptr = PRIORITY_ADDR(priority, PRIORITY_TYPE, cury, destx);
        //#define PRIORITY_ADDR(p,t,y,x)  (PRIORITY_VALID(t) ? (&(p).pixt<t>(y, x)) : NULL)
        static ListBytesPointer PRIORITY_ADDR<PRIORITY_TYPE>(bitmap_t p, int y, int x) { return PRIORITY_VALID<PRIORITY_TYPE>() ? p.pix8(y, x) : null; }

        //PRIORITY_ADVANCE(PRIORITY_TYPE, priptr, 4);
        //#define PRIORITY_ADVANCE(t,p,i) do { if (PRIORITY_VALID(t)) (p) += (i); } while (0)
        static void PRIORITY_ADVANCE<PRIORITY_TYPE>(ref ListBytesPointer p, UInt32 i) { if (PRIORITY_VALID<PRIORITY_TYPE>()) p += i; }


        /*-------------------------------------------------
            PIXEL_OP_COPY_OPAQUE - render all pixels
            regardless of pen, copying directly
        -------------------------------------------------*/
        //#define PIXEL_OP_COPY_OPAQUE(DEST, PRIORITY, SOURCE)
        public static void PIXEL_OP_COPY_OPAQUE(RawBufferPointer DEST, ListBytesPointer PRIORITY, u8 SOURCE, u32 color, u32 trans_mask, u32 trans_pen, ListPointer<rgb_t> paldata, int PIXEL_TYPE_SIZE)
        {
            //(DEST) = SOURCE;

            if (PIXEL_TYPE_SIZE == 1) DEST.set_uint8_offs8(SOURCE);
            else if (PIXEL_TYPE_SIZE == 2) DEST.set_uint16_offs8(SOURCE);
            else if (PIXEL_TYPE_SIZE == 4) DEST.set_uint32_offs8(SOURCE);
            else if (PIXEL_TYPE_SIZE == 8) DEST.set_uint64_offs8(SOURCE);
            else throw new emu_fatalerror("PIXEL_OP_COPY_OPAQUE() - wrong PIXEL_TYPE_SIZE: {0}", PIXEL_TYPE_SIZE);
        }


        /*-------------------------------------------------
            PIXEL_OP_COPY_TRANSPEN - render all pixels
            except those matching 'transpen', copying
            directly
        -------------------------------------------------*/
        //#define PIXEL_OP_COPY_TRANSPEN(DEST, PRIORITY, SOURCE)
        public static void PIXEL_OP_COPY_TRANSPEN(RawBufferPointer DEST, ListBytesPointer PRIORITY, u8 SOURCE, u32 color, u32 trans_mask, u32 trans_pen, ListPointer<rgb_t> paldata, int PIXEL_TYPE_SIZE)
        {
            //u32 srcdata = (SOURCE);
            //if (srcdata != trans_pen)
            //    (DEST) = SOURCE;

            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
            {
                if (PIXEL_TYPE_SIZE == 1) DEST.set_uint8_offs8(SOURCE);
                else if (PIXEL_TYPE_SIZE == 2) DEST.set_uint16_offs8(SOURCE);
                else if (PIXEL_TYPE_SIZE == 4) DEST.set_uint32_offs8(SOURCE);
                else if (PIXEL_TYPE_SIZE == 8) DEST.set_uint64_offs8(SOURCE);
                else throw new emu_fatalerror("PIXEL_OP_COPY_TRANSPEN() - wrong PIXEL_TYPE_SIZE: {0}", PIXEL_TYPE_SIZE);
            }
        }


        /*-------------------------------------------------
            PIXEL_OP_REBASE_OPAQUE - render all pixels
            regardless of pen, adding 'color' to the
            pen value
        -------------------------------------------------*/
        //#define PIXEL_OP_REBASE_OPAQUE(DEST, PRIORITY, SOURCE)
        public static void PIXEL_OP_REBASE_OPAQUE(RawBufferPointer DEST, ListBytesPointer PRIORITY, u8 SOURCE, u32 color, u32 trans_mask, u32 trans_pen, ListPointer<rgb_t> paldata, int PIXEL_TYPE_SIZE)
        {
            //DEST = color + SOURCE;

            if (PIXEL_TYPE_SIZE == 1) DEST.set_uint8_offs8((byte)(color + SOURCE));
            else if (PIXEL_TYPE_SIZE == 2) DEST.set_uint16_offs8((UInt16)(color + SOURCE));
            else if (PIXEL_TYPE_SIZE == 4) DEST.set_uint32_offs8(color + SOURCE);
            else if (PIXEL_TYPE_SIZE == 8) DEST.set_uint64_offs8(color + SOURCE);
            else throw new emu_fatalerror("PIXEL_OP_REBASE_OPAQUE() - wrong PIXEL_TYPE_SIZE: {0}", PIXEL_TYPE_SIZE);
        }

#if false
        define PIXEL_OP_REBASE_OPAQUE_PRIORITY(DEST, PRIORITY, SOURCE)               
        do                                                                           
        {                                                                            
            if (((1 << ((PRIORITY) & 0x1f)) & pmask) == 0)                           
                (DEST) = color + (SOURCE);                                           
            (PRIORITY) = 31;                                                         
        }                                                                                          
        while (0)
#endif


        /*-------------------------------------------------
            PIXEL_OP_REMAP_TRANSPEN - render all pixels
            except those matching 'trans_pen', mapping the
            pen via the 'paldata' array
        -------------------------------------------------*/
        //#define PIXEL_OP_REMAP_TRANSPEN(DEST, PRIORITY, SOURCE)                             \
        public static void PIXEL_OP_REMAP_TRANSPEN(RawBufferPointer DEST, ListBytesPointer PRIORITY, u8 SOURCE, u32 color, u32 trans_mask, u32 trans_pen, ListPointer<rgb_t> paldata, int PIXEL_TYPE_SIZE)
        {
            //u32 srcdata = (SOURCE);
            //if (srcdata != trans_pen)
            //    (DEST) = paldata[srcdata];

            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
            {
                if (PIXEL_TYPE_SIZE == 1) DEST.set_uint8_offs8((byte)paldata[srcdata]);
                else if (PIXEL_TYPE_SIZE == 2) DEST.set_uint16_offs8((UInt16)paldata[srcdata]);
                else if (PIXEL_TYPE_SIZE == 4) DEST.set_uint32_offs8(paldata[srcdata]);
                else if (PIXEL_TYPE_SIZE == 8) DEST.set_uint64_offs8(paldata[srcdata]);
                else throw new emu_fatalerror("PIXEL_OP_REMAP_TRANSPEN() - wrong PIXEL_TYPE_SIZE: {0}", PIXEL_TYPE_SIZE);
            }
        }

#if false
        define PIXEL_OP_REMAP_TRANSPEN_PRIORITY(DEST, PRIORITY, SOURCE)                    \
        do                                                                                  \
        {                                                                                   \
            u32 srcdata = (SOURCE);                                                         \
            if (srcdata != trans_pen)                                                       \
            {                                                                               \
                if (((1 << ((PRIORITY) & 0x1f)) & pmask) == 0)                              \
                    (DEST) = paldata[srcdata];                                              \
                (PRIORITY) = 31;                                                            \
            }                                                                               \
        }                                                                                   \
        while (0)
#endif


        /*-------------------------------------------------
            PIXEL_OP_REBASE_TRANSPEN - render all pixels
            except those matching 'transpen', adding
            'color' to the pen value
        -------------------------------------------------*/
        //#define PIXEL_OP_REBASE_TRANSPEN(DEST, PRIORITY, SOURCE)                            \
        public static void PIXEL_OP_REBASE_TRANSPEN(RawBufferPointer DEST, ListBytesPointer PRIORITY, u8 SOURCE, u32 color, u32 trans_mask, u32 trans_pen, ListPointer<rgb_t> paldata, int PIXEL_TYPE_SIZE)
        {
            //u32 srcdata = (SOURCE);
            //if (srcdata != trans_pen)
            //    (DEST) = color + srcdata;

            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
            {
                if (PIXEL_TYPE_SIZE == 1) DEST.set_uint8_offs8((byte)(color + srcdata));
                else if (PIXEL_TYPE_SIZE == 2) DEST.set_uint16_offs8((UInt16)(color + srcdata));
                else if (PIXEL_TYPE_SIZE == 4) DEST.set_uint32_offs8(color + srcdata);
                else if (PIXEL_TYPE_SIZE == 8) DEST.set_uint64_offs8(color + srcdata);
                else throw new emu_fatalerror("PIXEL_OP_REBASE_TRANSPEN() - wrong PIXEL_TYPE_SIZE: {0}", PIXEL_TYPE_SIZE);
            }
        }

#if false
        define PIXEL_OP_REBASE_TRANSPEN_PRIORITY(DEST, PRIORITY, SOURCE)                   \
        do                                                                                  \
        {                                                                                   \
            u32 srcdata = (SOURCE);                                                         \
            if (srcdata != trans_pen)                                                       \
            {                                                                               \
                if (((1 << ((PRIORITY) & 0x1f)) & pmask) == 0)                              \
                    (DEST) = color + srcdata;                                               \
                (PRIORITY) = 31;                                                            \
            }                                                                               \
        }                                                                                   \
        while (0)
#endif


        /*-------------------------------------------------
            PIXEL_OP_REBASE_TRANSMASK - render all pixels
            except those matching 'trans_mask', adding
            'color' to the pen value
        -------------------------------------------------*/
        //#define PIXEL_OP_REBASE_TRANSMASK(DEST, PRIORITY, SOURCE)
        public static void PIXEL_OP_REBASE_TRANSMASK(RawBufferPointer DEST, ListBytesPointer PRIORITY, u8 SOURCE, u32 color, u32 trans_mask, u32 trans_pen, ListPointer<rgb_t> paldata, int PIXEL_TYPE_SIZE)
        {
            //u32 srcdata = SOURCE;
            //if (((trans_mask >> srcdata) & 1) == 0)
            //    DEST = color + srcdata;

            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
            {
                if (PIXEL_TYPE_SIZE == 1) DEST.set_uint8_offs8((byte)(color + srcdata));
                else if (PIXEL_TYPE_SIZE == 2) DEST.set_uint16_offs8((UInt16)(color + srcdata));
                else if (PIXEL_TYPE_SIZE == 4) DEST.set_uint32_offs8(color + srcdata);
                else if (PIXEL_TYPE_SIZE == 8) DEST.set_uint64_offs8(color + srcdata);
                else throw new emu_fatalerror("PIXEL_OP_REBASE_TRANSMASK() - wrong PIXEL_TYPE_SIZE: {0}", PIXEL_TYPE_SIZE);
            }
        }

#if false
        define PIXEL_OP_REBASE_TRANSMASK_PRIORITY(DEST, PRIORITY, SOURCE)          
        do                                                                         
        {                                                                          
            u32 srcdata = (SOURCE);                                             
            if (((trans_mask >> srcdata) & 1) == 0)                                
            {                                                                      
                if (((1 << ((PRIORITY) & 0x1f)) & pmask) == 0)                     
                    (DEST) = color + srcdata;                                      
                (PRIORITY) = 31;                                                   
            }                                                                      
        }                                                                                               
        while (0)
#endif


        /***************************************************************************
            BASIC DRAWGFX CORE
        ***************************************************************************/

        /*
            Assumed input parameters or local variables:

                bitmap_t &dest - the bitmap to render to
                const rectangle &cliprect - a clipping rectangle (assumed to be clipped to the size of 'dest')
                gfx_element *gfx - pointer to the gfx_element to render
                u32 code - index of the entry within gfx_element
                u32 color - index of the color within gfx_element
                int flipx - non-zero means render right-to-left instead of left-to-right
                int flipy - non-zero means render bottom-to-top instead of top-to-bottom
                s32 destx - the top-left X coordinate to render to
                s32 desty - the top-left Y coordinate to render to
                bitmap_t &priority - the priority bitmap (even if PRIORITY_TYPE is NO_PRIORITY, at least needs a dummy)
        */
        //define DRAWGFX_CORE(PIXEL_TYPE, PIXEL_OP, PRIORITY_TYPE)
        public static void DRAWGFX_CORE<PIXEL_TYPE, PRIORITY_TYPE>(PIXEL_OP PIXEL_OP_, 
            bitmap_t dest,
            rectangle cliprect,
            gfx_element gfx,
            u32 code,
            u32 color,
            int flipx, int flipy,
            s32 destx, s32 desty,
            bitmap_t priority,
            u16 width, u16 height,
            u32 rowbytes,
            get_data_delegate get_data,
            u32 trans_mask,
            u32 trans_pen,
            ListPointer<rgb_t> paldata,
            int PIXEL_TYPE_SIZE)
        {
            profiler_global.g_profiler.start(profile_type.PROFILER_DRAWGFX);


            do {
                ListBytesPointer srcdata;  //const u8 *srcdata;
                s32 destendx;
                s32 destendy;
                s32 srcx;
                s32 srcy;
                s32 curx;
                s32 cury;
                s32 dy;

                //assert(dest.valid());
                //assert(!PRIORITY_VALID(PRIORITY_TYPE) || priority.valid());
                //assert(dest.cliprect().contains(cliprect));
                //assert(code < elements());

                /* ignore empty/invalid cliprects */
                if (cliprect.empty())
                    break;

                /* compute final pixel in X and exit if we are entirely clipped */
                destendx = destx + width - 1;
                if (destx > cliprect.right() || destendx < cliprect.left())
                    break;

                /* apply left clip */
                srcx = 0;
                if (destx < cliprect.left())
                {
                    srcx = cliprect.left() - destx;
                    destx = cliprect.left();
                }

                /* apply right clip */
                if (destendx > cliprect.right())
                    destendx = cliprect.right();

                /* compute final pixel in Y and exit if we are entirely clipped */
                destendy = desty + height - 1;
                if (desty > cliprect.bottom() || destendy < cliprect.top())
                    break;

                /* apply top clip */
                srcy = 0;
                if (desty < cliprect.top())
                {
                    srcy = cliprect.top() - desty;
                    desty = cliprect.top();
                }

                /* apply bottom clip */
                if (destendy > cliprect.bottom())
                    destendy = cliprect.bottom();

                /* apply X flipping */
                if (flipx != 0)
                    srcx = width - 1 - srcx;

                /* apply Y flipping */
                dy = (int)rowbytes;
                if (flipy != 0)
                {
                    srcy = height - 1 - srcy;
                    dy = -dy;
                }

                /* fetch the source data */
                srcdata = get_data(code);

                /* compute how many blocks of 4 pixels we have */
                u32 numblocks = (u32)((destendx + 1 - destx) / 4);
                u32 leftovers = (u32)((destendx + 1 - destx) - 4 * numblocks);

                /* adjust srcdata to point to the first source pixel of the row */
                srcdata += (srcy * (int)rowbytes + srcx);

                /* non-flipped 8bpp case */
                if (flipx == 0)
                {
                    /* iterate over pixels in Y */
                    for (cury = desty; cury <= destendy; cury++)
                    {
                        ListBytesPointer priptr = PRIORITY_ADDR<PRIORITY_TYPE>(priority, cury, destx);  //PRIORITY_TYPE *priptr = PRIORITY_ADDR(priority, PRIORITY_TYPE, cury, destx);
                        RawBufferPointer destptr = dest.pix8(cury, destx);  //PIXEL_TYPE *destptr = &dest.pixt<PIXEL_TYPE>(cury, destx);
                        ListBytesPointer srcptr = new ListBytesPointer(srcdata);  //const u8 *srcptr = srcdata;
                        srcdata += dy;

                        /* iterate over unrolled blocks of 4 */
                        for (curx = 0; curx < numblocks; curx++)
                        {
                            PIXEL_OP_(destptr,                         priptr,                             srcptr[0], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE); //PIXEL_OP_(destptr[0], priptr[0], srcptr[0]);
                            PIXEL_OP_(destptr + (1 * PIXEL_TYPE_SIZE), priptr != null ? priptr + 1 : null, srcptr[1], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE); //PIXEL_OP_(destptr[1], priptr[1], srcptr[1]);
                            PIXEL_OP_(destptr + (2 * PIXEL_TYPE_SIZE), priptr != null ? priptr + 2 : null, srcptr[2], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE); //PIXEL_OP_(destptr[2], priptr[2], srcptr[2]);
                            PIXEL_OP_(destptr + (3 * PIXEL_TYPE_SIZE), priptr != null ? priptr + 3 : null, srcptr[3], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE); //PIXEL_OP_(destptr[3], priptr[3], srcptr[3]);
                            srcptr += 4;
                            destptr += 4 * PIXEL_TYPE_SIZE;  //destptr += 4;
                            PRIORITY_ADVANCE<PRIORITY_TYPE>(ref priptr, 4);
                        }

                        /* iterate over leftover pixels */
                        for (curx = 0; curx < leftovers; curx++)
                        {
                            PIXEL_OP_(destptr, priptr, srcptr[0], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE); //PIXEL_OP_(destptr[0], priptr[0], srcptr[0]);
                            srcptr++;
                            destptr += 1 * PIXEL_TYPE_SIZE;  //destptr++;
                            PRIORITY_ADVANCE<PRIORITY_TYPE>(ref priptr, 1);
                        }
                    }
                }
                /* flipped 8bpp case */
                else
                {
                    /* iterate over pixels in Y */
                    for (cury = desty; cury <= destendy; cury++)
                    {
                        ListBytesPointer priptr = PRIORITY_ADDR<PRIORITY_TYPE>(priority, cury, destx);  //PRIORITY_TYPE *priptr = PRIORITY_ADDR(priority, PRIORITY_TYPE, cury, destx);
                        RawBufferPointer destptr = dest.pix8(cury, destx);  //PIXEL_TYPE *destptr = &dest.pixt<PIXEL_TYPE>(cury, destx);
                        ListBytesPointer srcptr = new ListBytesPointer(srcdata);  //const u8 *srcptr = srcdata;
                        srcdata += dy;

                        /* iterate over unrolled blocks of 4 */
                        for (curx = 0; curx < numblocks; curx++)
                        {
                            PIXEL_OP_(destptr,                         priptr,                             srcptr[ 0], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE); //PIXEL_OP_(destptr[0], priptr[0], srcptr[ 0]);
                            PIXEL_OP_(destptr + (1 * PIXEL_TYPE_SIZE), priptr != null ? priptr + 1 : null, srcptr[-1], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE); //PIXEL_OP_(destptr[1], priptr[1], srcptr[-1]);
                            PIXEL_OP_(destptr + (2 * PIXEL_TYPE_SIZE), priptr != null ? priptr + 2 : null, srcptr[-2], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE); //PIXEL_OP_(destptr[2], priptr[2], srcptr[-2]);
                            PIXEL_OP_(destptr + (3 * PIXEL_TYPE_SIZE), priptr != null ? priptr + 3 : null, srcptr[-3], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE); //PIXEL_OP_(destptr[3], priptr[3], srcptr[-3]);

                            srcptr -= 4;
                            destptr += 4 * PIXEL_TYPE_SIZE; //destptr += 4;
                            PRIORITY_ADVANCE<PRIORITY_TYPE>(ref priptr, 4);
                        }
                        /* iterate over leftover pixels */
                        for (curx = 0; curx < leftovers; curx++)
                        {
                            PIXEL_OP_(destptr, priptr, srcptr[0], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE); //PIXEL_OP_(destptr[0], priptr[0], srcptr[0]);
                            srcptr--;
                            destptr += 1 * PIXEL_TYPE_SIZE; //destptr++;
                            PRIORITY_ADVANCE<PRIORITY_TYPE>(ref priptr, 1);
                        }
                    }
                }
            } while (false);

            profiler_global.g_profiler.stop();
        }


        /***************************************************************************
            BASIC COPYBITMAP CORE
        ***************************************************************************/

        /*
            Assumed input parameters or local variables:

                bitmap_t &dest - the bitmap to copy to
                bitmap_t &src - the bitmap to copy from (must be same bpp as dest)
                const rectangle &cliprect - a clipping rectangle (assumed to be clipped to the size of 'dest')
                int flipx - non-zero means render right-to-left instead of left-to-right
                int flipy - non-zero means render bottom-to-top instead of top-to-bottom
                s32 destx - the top-left X coordinate to copy to
                s32 desty - the top-left Y coordinate to copy to
                bitmap_t &priority - the priority bitmap (even if PRIORITY_TYPE is NO_PRIORITY, at least needs a dummy)
        */

        //#define COPYBITMAP_CORE(PIXEL_TYPE, PIXEL_OP, PRIORITY_TYPE)
        public static void COPYBITMAP_CORE<PIXEL_TYPE, PRIORITY_TYPE>(
            PIXEL_OP PIXEL_OP_,
            bitmap_t dest, bitmap_t src,
            rectangle cliprect,
            int flipx, int flipy,
            s32 destx, s32 desty,
            bitmap_t priority,
            u32 color,
            u32 trans_mask,
            u32 trans_pen,
            ListPointer<rgb_t> paldata,
            int PIXEL_TYPE_SIZE)
        {
            profiler_global.g_profiler.start(profile_type.PROFILER_COPYBITMAP);

            do {
                ListBytesPointer srcdata;  //PIXEL_TYPE srcdata;
                u32 numblocks;
                u32 leftovers;
                s32 destendx;
                s32 destendy;
                s32 srcx;
                s32 srcy;
                s32 curx;
                s32 cury;
                s32 dx;
                s32 dy;

                global_object.assert(dest.valid());
                global_object.assert(src.valid());
                //global_object.assert(!PRIORITY_VALID(PRIORITY_TYPE) || priority.valid());
                //global_object.assert(dest.cliprect().contains(cliprect));

                /* ignore empty/invalid cliprects */
                if (cliprect.empty())
                    break;

                /* standard setup; dx counts bytes in X, dy counts pixels in Y */
                dx = 1;
                dy = src.rowpixels();

                /* compute final pixel in X and exit if we are entirely clipped */
                destendx = destx + src.width() - 1;
                if (destx > cliprect.right() || destendx < cliprect.left())
                    break;

                /* apply left clip */
                srcx = 0;
                if (destx < cliprect.left())
                {
                    srcx = cliprect.left() - destx;
                    destx = cliprect.left();
                }

                /* apply right clip */
                if (destendx > cliprect.right())
                    destendx = cliprect.right();

                /* compute final pixel in Y and exit if we are entirely clipped */
                destendy = desty + src.height() - 1;
                if (desty > cliprect.bottom() || destendy < cliprect.top())
                    break;

                /* apply top clip */
                srcy = 0;
                if (desty < cliprect.top())
                {
                    srcy = cliprect.top() - desty;
                    desty = cliprect.top();
                }

                /* apply bottom clip */
                if (destendy > cliprect.bottom())
                    destendy = cliprect.bottom();

                /* apply X flipping */
                if (flipx != 0)
                {
                    srcx = src.width() - 1 - srcx;
                    dx = -dx;
                }

                /* apply Y flipping */
                if (flipy != 0)
                {
                    srcy = src.height() - 1 - srcy;
                    dy = -dy;
                }

                /* compute how many blocks of 4 pixels we have */
                numblocks = (u32)((destendx + 1 - destx) / 4);
                leftovers = (u32)((destendx + 1 - destx) - 4 * numblocks);

                /* compute the address of the first source pixel of the first row */
                srcdata = src.pix8(srcy, srcx);  //srcdata = &src.pixt<PIXEL_TYPE>(srcy, srcx);

                /* non-flipped case */
                if (flipx == 0)
                {
                    /* iterate over pixels in Y */
                    for (cury = desty; cury <= destendy; cury++)
                    {
                        ListBytesPointer priptr = PRIORITY_ADDR<PRIORITY_TYPE>(priority, cury, destx);  //PRIORITY_TYPE* priptr = PRIORITY_ADDR(priority, PRIORITY_TYPE, cury, destx);
                        RawBufferPointer destptr = dest.pix8(cury, destx);  //PIXEL_TYPE* destptr = &dest.pixt<PIXEL_TYPE>(cury, destx);
                        ListBytesPointer srcptr = new ListBytesPointer(srcdata);  //const PIXEL_TYPE* srcptr = srcdata;
                        srcdata += dy;

                        /* iterate over unrolled blocks of 4 */
                        for (curx = 0; curx < numblocks; curx++)
                        {
                            PIXEL_OP_(destptr,                         priptr,                             srcptr[0], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE);  //PIXEL_OP_(destptr[0], priptr[0], srcptr[0]);
                            PIXEL_OP_(destptr + (1 * PIXEL_TYPE_SIZE), priptr != null ? priptr + 1 : null, srcptr[1], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE);  //PIXEL_OP_(destptr[1], priptr[1], srcptr[1]);
                            PIXEL_OP_(destptr + (2 * PIXEL_TYPE_SIZE), priptr != null ? priptr + 2 : null, srcptr[2], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE);  //PIXEL_OP_(destptr[2], priptr[2], srcptr[2]);
                            PIXEL_OP_(destptr + (3 * PIXEL_TYPE_SIZE), priptr != null ? priptr + 3 : null, srcptr[3], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE);  //PIXEL_OP_(destptr[3], priptr[3], srcptr[3]);

                            srcptr += 4;
                            destptr += 4 * PIXEL_TYPE_SIZE;  // destptr += 4;
                            PRIORITY_ADVANCE<PRIORITY_TYPE>(ref priptr, 4);
                        }

                        /* iterate over leftover pixels */
                        for (curx = 0; curx < leftovers; curx++)
                        {
                            PIXEL_OP_(destptr, priptr, srcptr[0], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE);  //PIXEL_OP(destptr[0], priptr[0], srcptr[0]);
                            srcptr++;
                            destptr += 1 * PIXEL_TYPE_SIZE;  //destptr++;
                            PRIORITY_ADVANCE<PRIORITY_TYPE>(ref priptr, 1);
                        }
                    }
                }

                /* flipped case */
                else
                {
                    /* iterate over pixels in Y */
                    for (cury = desty; cury <= destendy; cury++)
                    {
                        ListBytesPointer priptr = PRIORITY_ADDR<PRIORITY_TYPE>(priority, cury, destx);  //PRIORITY_TYPE* priptr = PRIORITY_ADDR(priority, PRIORITY_TYPE, cury, destx);
                        RawBufferPointer destptr = dest.pix8(cury, destx);  //PIXEL_TYPE* destptr = &dest.pixt<PIXEL_TYPE>(cury, destx);
                        ListBytesPointer srcptr = new ListBytesPointer(srcdata);  //const PIXEL_TYPE* srcptr = srcdata;
                        srcdata += dy;

                        /* iterate over unrolled blocks of 4 */
                        for (curx = 0; curx < numblocks; curx++)
                        {
                            PIXEL_OP_(destptr,                         priptr,                             srcptr[ 0], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE);  //PIXEL_OP_(destptr[0], priptr[0], srcptr[0]);
                            PIXEL_OP_(destptr + (1 * PIXEL_TYPE_SIZE), priptr != null ? priptr + 1 : null, srcptr[-1], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE);  //PIXEL_OP_(destptr[1], priptr[1], srcptr[-1]);
                            PIXEL_OP_(destptr + (2 * PIXEL_TYPE_SIZE), priptr != null ? priptr + 2 : null, srcptr[-2], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE);  //PIXEL_OP_(destptr[2], priptr[2], srcptr[-2]);
                            PIXEL_OP_(destptr + (3 * PIXEL_TYPE_SIZE), priptr != null ? priptr + 3 : null, srcptr[-3], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE);  //PIXEL_OP_(destptr[3], priptr[3], srcptr[-3]);

                            srcptr -= 4;
                            destptr += 4 * PIXEL_TYPE_SIZE;  //destptr += 4;
                            PRIORITY_ADVANCE<PRIORITY_TYPE>(ref priptr, 4);
                        }

                        /* iterate over leftover pixels */
                        for (curx = 0; curx < leftovers; curx++)
                        {
                            PIXEL_OP_(destptr, priptr, srcptr[0], color, trans_mask, trans_pen, paldata, PIXEL_TYPE_SIZE);  //PIXEL_OP_(destptr[0], priptr[0], srcptr[0]);
                            srcptr--;
                            destptr += 1 * PIXEL_TYPE_SIZE;  //destptr++;
                            PRIORITY_ADVANCE<PRIORITY_TYPE>(ref priptr, 1);
                        }
                    }
                }
            } while (false);

            profiler_global.g_profiler.stop();
        }
    }
}
