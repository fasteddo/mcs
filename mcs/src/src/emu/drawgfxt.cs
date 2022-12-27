// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using pen_t = System.UInt32;  //typedef u32 pen_t;
using PointerU8 = mame.Pointer<System.Byte>;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;

using static mame.cpp_global;
using static mame.drawgfx_global;
using static mame.drawgfxt_global;
using static mame.profiler_global;


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


        //#define PIXEL_OP_COPY_OPAQUE_PRIORITY(DEST, PRIORITY, SOURCE)                       \
        //do                                                                                  \
        //{                                                                                   \
        //    if (((1 << ((PRIORITY) & 0x1f)) & pmask) == 0)                                  \
        //        (DEST) = SOURCE;                                                            \
        //    (PRIORITY) = 31;                                                                \
        //}                                                                                   \
        //while (0)

        //#define PIXEL_OP_COPY_OPAQUE_PRIMASK(DEST, PRIORITY, SOURCE)                        \
        //do                                                                                  \
        //{                                                                                   \
        //    (DEST) = SOURCE;                                                                \
        //    (PRIORITY) = ((PRIORITY) & pmask) | pcode;                                      \
        //}                                                                                   \
        //while (0)


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


        //#define PIXEL_OP_COPY_TRANSPEN_PRIORITY(DEST, PRIORITY, SOURCE)                     \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if (srcdata != trans_pen)                                                       \
        //    {                                                                               \
        //        if (((1 << ((PRIORITY) & 0x1f)) & pmask) == 0)                              \
        //            (DEST) = SOURCE;                                                        \
        //        (PRIORITY) = 31;                                                            \
        //    }                                                                               \
        //}                                                                                   \


        //while (0)
        //#define PIXEL_OP_COPY_TRANSPEN_PRIMASK(DEST, PRIORITY, SOURCE)                      \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if (srcdata != trans_pen)                                                       \
        //    {                                                                               \
        //        (DEST) = SOURCE;                                                            \
        //        (PRIORITY) = ((PRIORITY) & pmask) | pcode;                                  \
        //    }                                                                               \
        //}                                                                                   \
        //while (0)


        /*-------------------------------------------------
            PIXEL_OP_COPY_TRANSALPHA - render all pixels
            except those with an alpha of zero, copying
            directly
        -------------------------------------------------*/

        //#define PIXEL_OP_COPY_TRANSALPHA(DEST, SOURCE)                                      \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if ((srcdata & 0xff000000) != 0)                                                \
        //        (DEST) = SOURCE;                                                            \
        //}                                                                                   \
        //while (0)


        //#define PIXEL_OP_COPY_TRANSALPHA_PRIORITY(DEST, PRIORITY, SOURCE)                   \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if ((srcdata & 0xff000000) != 0)                                                \
        //    {                                                                               \
        //        if (((1 << ((PRIORITY) & 0x1f)) & pmask) == 0)                              \
        //            (DEST) = SOURCE;                                                        \
        //        (PRIORITY) = 31;                                                            \
        //    }                                                                               \
        //}                                                                                   \
        //while (0)


        //#define PIXEL_OP_COPY_TRANSALPHA_PRIMASK(DEST, PRIORITY, SOURCE)                    \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if ((srcdata & 0xff000000) != 0)                                                \
        //    {                                                                               \
        //        (DEST) = SOURCE;                                                            \
        //        (PRIORITY) = ((PRIORITY) & pmask) | pcode;                                  \
        //    }                                                                               \
        //}                                                                                   \
        //while (0)


        /*-------------------------------------------------
            PIXEL_OP_REMAP_OPAQUE - render all pixels
            regardless of pen, mapping the pen via the
            'paldata' array
        -------------------------------------------------*/

        //#define PIXEL_OP_REMAP_OPAQUE(DEST, SOURCE)                                         \
        //do                                                                                  \
        //{                                                                                   \
        //    (DEST) = paldata[SOURCE];                                                       \
        //}                                                                                   \
        //while (0)
        public static void PIXEL_OP_REMAP_OPAQUE(Pointer<pen_t> paldata, ref u16 DEST, u16 SOURCE)
        {
            DEST = (u16)paldata[SOURCE];
        }


        //#define PIXEL_OP_REMAP_OPAQUE_PRIORITY(DEST, PRIORITY, SOURCE)                      \
        //do                                                                                  \
        //{                                                                                   \
        //    if (((1 << ((PRIORITY) & 0x1f)) & pmask) == 0)                                  \
        //        (DEST) = paldata[SOURCE];                                                   \
        //    (PRIORITY) = 31;                                                                \
        //}                                                                                   \
        //while (0)


        //#define PIXEL_OP_REMAP_OPAQUE_PRIMASK(DEST, PRIORITY, SOURCE)                       \
        //do                                                                                  \
        //{                                                                                   \
        //    (DEST) = paldata[SOURCE];                                                       \
        //    (PRIORITY) = ((PRIORITY) & pmask) | pcode;                                      \
        //}                                                                                   \
        //while (0)


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

        //#define PIXEL_OP_REBASE_OPAQUE_PRIORITY(DEST, PRIORITY, SOURCE)                     \
        //do                                                                                  \
        //{                                                                                   \
        //    if (((1 << ((PRIORITY) & 0x1f)) & pmask) == 0)                                  \
        //        (DEST) = color + (SOURCE);                                                  \
        //    (PRIORITY) = 31;                                                                \
        //}                                                                                   \
        //while (0)
        public static void PIXEL_OP_REBASE_OPAQUE_PRIORITY(u32 pmask, u32 color, ref u16 DEST, ref u8 PRIORITY, u8 SOURCE)
        {
            if (((1U << (PRIORITY & 0x1f)) & pmask) == 0)
                DEST = (u16)(color + SOURCE);
            PRIORITY = 31;
        }


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


        //#define PIXEL_OP_REMAP_TRANSPEN_PRIORITY(DEST, PRIORITY, SOURCE)                    \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if (srcdata != trans_pen)                                                       \
        //    {                                                                               \
        //        if (((1 << ((PRIORITY) & 0x1f)) & pmask) == 0)                              \
        //            (DEST) = paldata[srcdata];                                              \
        //        (PRIORITY) = 31;                                                            \
        //    }                                                                               \
        //}                                                                                   \
        //while (0)


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
        public static void PIXEL_OP_REBASE_TRANSPEN(u32 trans_pen, u32 color, ref u8 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = (u8)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSPEN(u32 trans_pen, u32 color, ref u16 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = (u16)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSPEN(u32 trans_pen, u32 color, ref u16 DEST, u16 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = (u16)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSPEN(u32 trans_pen, u32 color, ref u32 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = (u32)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSPEN(u32 trans_pen, u32 color, ref u32 DEST, u16 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = (u32)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSPEN(u32 trans_pen, u32 color, ref u32 DEST, u32 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (srcdata != trans_pen)
                DEST = (u32)(color + srcdata);
        }


        //#define PIXEL_OP_REBASE_TRANSPEN_PRIORITY(DEST, PRIORITY, SOURCE)                   \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if (srcdata != trans_pen)                                                       \
        //    {                                                                               \
        //        if (((1 << ((PRIORITY) & 0x1f)) & pmask) == 0)                              \
        //            (DEST) = color + srcdata;                                               \
        //        (PRIORITY) = 31;                                                            \
        //    }                                                                               \
        //}                                                                                   \
        //while (0)


        /*-------------------------------------------------
            PIXEL_OP_REMAP_TRANSMASK - render all pixels
            except those matching 'trans_mask', mapping the
            pen via the 'paldata' array
        -------------------------------------------------*/
        //#define PIXEL_OP_REMAP_TRANSMASK(DEST, SOURCE)                                      \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if (((trans_mask >> srcdata) & 1) == 0)                                         \
        //        (DEST) = paldata[srcdata];                                                  \
        //}                                                                                   \
        //while (0)
        public static void PIXEL_OP_REMAP_TRANSMASK(u32 trans_mask, Pointer<rgb_t> paldata, ref u32 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
                DEST = paldata[srcdata];
        }


        //#define PIXEL_OP_REMAP_TRANSMASK_PRIORITY(DEST, PRIORITY, SOURCE)                   \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if (((trans_mask >> srcdata) & 1) == 0)                                         \
        //    {                                                                               \
        //        if (((1 << ((PRIORITY) & 0x1f)) & pmask) == 0)                              \
        //            (DEST) = paldata[srcdata];                                              \
        //        (PRIORITY) = 31;                                                            \
        //    }                                                                               \
        //}                                                                                   \
        //while (0)


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
        public static void PIXEL_OP_REBASE_TRANSMASK(u32 trans_mask, u32 color, ref u8 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
                DEST = (u8)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSMASK(u32 trans_mask, u32 color, ref u16 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
                DEST = (u16)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSMASK(u32 trans_mask, u32 color, ref u16 DEST, u16 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
                DEST = (u16)(color + srcdata);
        }
        public static void PIXEL_OP_REBASE_TRANSMASK(u32 trans_mask, u32 color, ref u32 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
                DEST = color + srcdata;
        }
        public static void PIXEL_OP_REBASE_TRANSMASK(u32 trans_mask, u32 color, ref u32 DEST, u16 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
                DEST = color + srcdata;
        }
        public static void PIXEL_OP_REBASE_TRANSMASK(u32 trans_mask, u32 color, ref u32 DEST, u32 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
                DEST = color + srcdata;
        }


        //#define PIXEL_OP_REBASE_TRANSMASK_PRIORITY(DEST, PRIORITY, SOURCE)                  \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if (((trans_mask >> srcdata) & 1) == 0)                                         \
        //    {                                                                               \
        //        if (((1 << ((PRIORITY) & 0x1f)) & pmask) == 0)                              \
        //            (DEST) = color + srcdata;                                               \
        //        (PRIORITY) = 31;                                                            \
        //    }                                                                               \
        //}                                                                                   \
        //while (0)
        public static void PIXEL_OP_REBASE_TRANSMASK_PRIORITY(u32 pmask, u32 trans_mask, u32 color, ref u16 DEST, ref u8 PRIORITY, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            if (((trans_mask >> (int)srcdata) & 1) == 0)
            {
                if (((1U << (PRIORITY & 0x1f)) & pmask) == 0)
                    DEST = (u16)(color + srcdata);
                PRIORITY = 31;
            }
        }


        /*-------------------------------------------------
            PIXEL_OP_REBASE_TRANSTABLE - look up each pen in
            'pentable'; if the entry is DRAWMODE_NONE,
            don't draw it; if the entry is DRAWMODE_SOURCE,
            add 'color' to the pen value; if the entry is
            DRAWMODE_SHADOW, generate a shadow of the
            destination pixel using 'shadowtable'

            PIXEL_OP_REMAP_TRANSTABLE - look up each pen in
            'pentable'; if the entry is DRAWMODE_NONE,
            don't draw it; if the entry is DRAWMODE_SOURCE,
            look up the pen via the 'paldata' array; if the
            entry is DRAWMODE_SHADOW, generate a shadow of
            the destination pixel using 'shadowtable'
        -------------------------------------------------*/
        //#define PIXEL_OP_REBASE_TRANSTABLE16(DEST, SOURCE)                                  \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    u32 entry = pentable[srcdata];                                                  \
        //    if (entry != DRAWMODE_NONE)                                                     \
        //    {                                                                               \
        //        if (entry == DRAWMODE_SOURCE)                                               \
        //            (DEST) = color + srcdata;                                               \
        //        else                                                                        \
        //            (DEST) = shadowtable[DEST];                                             \
        //    }                                                                               \
        //}                                                                                   \
        //while (0)
        public static void PIXEL_OP_REBASE_TRANSTABLE16(u8 [] pentable, u32 color, Pointer<pen_t> shadowtable, ref u16 DEST, u8 SOURCE)
        {
            u32 srcdata = SOURCE;
            u32 entry = pentable[srcdata];
            if (entry != DRAWMODE_NONE)
            {
                if (entry == DRAWMODE_SOURCE)
                    DEST = (u16)(color + srcdata);
                else
                    DEST = (u16)shadowtable[DEST];
            }
        }

        //#define PIXEL_OP_REMAP_TRANSTABLE32(DEST, SOURCE)                                   \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    u32 entry = pentable[srcdata];                                                  \
        //    if (entry != DRAWMODE_NONE)                                                     \
        //    {                                                                               \
        //        if (entry == DRAWMODE_SOURCE)                                               \
        //            (DEST) = paldata[srcdata];                                              \
        //        else                                                                        \
        //            (DEST) = shadowtable[rgb_t(DEST).as_rgb15()];                           \
        //    }                                                                               \
        //}                                                                                   \
        //while (0)
        //#define PIXEL_OP_REBASE_TRANSTABLE16_PRIORITY(DEST, PRIORITY, SOURCE)               \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    u32 entry = pentable[srcdata];                                                  \
        //    if (entry != DRAWMODE_NONE)                                                     \
        //    {                                                                               \
        //        u8 pridata = (PRIORITY);                                                    \
        //        if (entry == DRAWMODE_SOURCE)                                               \
        //        {                                                                           \
        //            if (((1 << (pridata & 0x1f)) & pmask) == 0)                             \
        //                (DEST) = color + srcdata;                                           \
        //            (PRIORITY) = 31;                                                        \
        //        }                                                                           \
        //        else if ((pridata & 0x80) == 0 && ((1 << (pridata & 0x1f)) & pmask) == 0)   \
        //        {                                                                           \
        //            (DEST) = shadowtable[DEST];                                             \
        //            (PRIORITY) = pridata | 0x80;                                            \
        //        }                                                                           \
        //    }                                                                               \
        //}                                                                                   \
        //while (0)
        //#define PIXEL_OP_REMAP_TRANSTABLE32_PRIORITY(DEST, PRIORITY, SOURCE)                \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    u32 entry = pentable[srcdata];                                                  \
        //    if (entry != DRAWMODE_NONE)                                                     \
        //    {                                                                               \
        //        u8 pridata = (PRIORITY);                                                    \
        //        if (entry == DRAWMODE_SOURCE)                                               \
        //        {                                                                           \
        //            if (((1 << (pridata & 0x1f)) & pmask) == 0)                             \
        //                (DEST) = paldata[srcdata];                                          \
        //            (PRIORITY) = 31;                                                        \
        //        }                                                                           \
        //        else if ((pridata & 0x80) == 0 && ((1 << (pridata & 0x1f)) & pmask) == 0)   \
        //        {                                                                           \
        //            (DEST) = shadowtable[rgb_t(DEST).as_rgb15()];                           \
        //            (PRIORITY) = pridata | 0x80;                                            \
        //        }                                                                           \
        //    }                                                                               \
        //}                                                                                   \
        //while (0)


        /*-------------------------------------------------
            PIXEL_OP_REMAP_TRANSPEN_ALPHA - render all
            pixels except those matching 'transpen',
            mapping the pen to via the 'paldata' array;
            the resulting color is RGB alpha blended
            against the destination using 'alpha'
        -------------------------------------------------*/
        //#define PIXEL_OP_REMAP_TRANSPEN_ALPHA32(DEST, SOURCE)                               \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if (srcdata != trans_pen)                                                       \
        //        (DEST) = alpha_blend_r32((DEST), paldata[srcdata], alpha_val);              \
        //}                                                                                   \
        //while (0)
        //#define PIXEL_OP_REMAP_TRANSPEN_ALPHA32_PRIORITY(DEST, PRIORITY, SOURCE)            \
        //do                                                                                  \
        //{                                                                                   \
        //    u32 srcdata = (SOURCE);                                                         \
        //    if (srcdata != trans_pen)                                                       \
        //    {                                                                               \
        //        if (((1 << ((PRIORITY) & 0x1f)) & pmask) == 0)                              \
        //            (DEST) = alpha_blend_r32((DEST), paldata[srcdata], alpha_val);          \
        //        (PRIORITY) = 31;                                                            \
        //    }                                                                               \
        //}                                                                                   \
        //while (0)
    }


    public partial class gfx_element
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
            public delegate void func16x8x8(ref u16 destp, ref u8 pri, u8 srcp);

            func8x8    m_func8x8;
            func16x8   m_func16x8;
            func16x16  m_func16x16;
            func32x8   m_func32x8;
            func32x16  m_func32x16;
            func32x32  m_func32x32;
            func16x8x8 m_func16x8x8;

            public FunctionClass(func8x8 func) { m_func8x8 = func; }
            public FunctionClass(func16x8 func) { m_func16x8 = func; }
            public FunctionClass(func16x16 func) { m_func16x16 = func; }
            public FunctionClass(func32x8 func) { m_func32x8 = func; }
            public FunctionClass(func32x16 func) { m_func32x16 = func; }
            public FunctionClass(func32x32 func) { m_func32x32 = func; }
            public FunctionClass(func16x8x8 func) { m_func16x8x8 = func; }

            public void op8x8(ref u8 destp, u8 srcp) { m_func8x8(ref destp, srcp); }
            public void op16x8(ref u16 destp, u8 srcp) { m_func16x8(ref destp, srcp); }
            public void op16x16(ref u16 destp, u16 srcp) { m_func16x16(ref destp, srcp); }
            public void op32x8(ref u32 destp, u8 srcp) { m_func32x8(ref destp, srcp); }
            public void op32x16(ref u32 destp, u16 srcp) { m_func32x16(ref destp, srcp); }
            public void op32x32(ref u32 destp, u32 srcp) { m_func32x32(ref destp, srcp); }
            public void op16x8x8(ref u16 destp, ref u8 pri, u8 srcp) { m_func16x8x8(ref destp, ref pri, srcp); }
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
            g_profiler.start(profile_type.PROFILER_DRAWGFX);


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
                                case 32:
                                {
                                    var destptrTemp0 = destptr32[0]; pixel_op.op32x8(ref destptrTemp0, srcptr[ 0]); destptr32[0] = destptrTemp0;
                                    var destptrTemp1 = destptr32[1]; pixel_op.op32x8(ref destptrTemp1, srcptr[-1]); destptr32[1] = destptrTemp1;
                                    var destptrTemp2 = destptr32[2]; pixel_op.op32x8(ref destptrTemp2, srcptr[-2]); destptr32[2] = destptrTemp2;
                                    var destptrTemp3 = destptr32[3]; pixel_op.op32x8(ref destptrTemp3, srcptr[-3]); destptr32[3] = destptrTemp3;
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
                                case 32: { var destptrTemp = destptr32[0]; pixel_op.op32x8(ref destptrTemp, srcptr[0]); destptr32[0] = destptrTemp; break; }
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

            g_profiler.stop();
        }


        //template <typename BitmapType, typename PriorityType, typename FunctionClass>
        //inline void gfx_element::drawgfx_core(BitmapType &dest, const rectangle &cliprect, u32 code, int flipx, int flipy, s32 destx, s32 desty, PriorityType &priority, FunctionClass pixel_op)
        void drawgfx_core<BitmapType, BitmapType_PixelType, BitmapType_PixelType_OPS, BitmapType_PixelTypePointer, PriorityType, PriorityType_PixelType, PriorityType_PixelType_OPS, PriorityType_PixelTypePointer>
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
            where PriorityType : bitmap_specific<PriorityType_PixelType, PriorityType_PixelType_OPS, PriorityType_PixelTypePointer> 
            where PriorityType_PixelType_OPS : PixelType_operators, new()
            where PriorityType_PixelTypePointer : PointerU8
        {
            g_profiler.start(profile_type.PROFILER_DRAWGFX);

            do {
                assert(dest.valid());
                assert(priority.valid());
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
                        //auto *priptr = &priority.pix(cury, destx);
                        PointerU8 priptr8 = null;
                        PointerU16 priptr16 = null;
                        PointerU32 priptr32 = null;
                        PointerU64 priptr64 = null;
                        switch (priority.bpp())
                        {
                            case 8:  priptr8 = priority.pix8(cury, destx); break;
                            case 16: throw new emu_unimplemented();
                            case 32: throw new emu_unimplemented();
                            case 64: throw new emu_unimplemented();
                            default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", priority.bpp());
                        }

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
                            //pixel_op(destptr[0], priptr[0], srcptr[0]);
                            //pixel_op(destptr[1], priptr[1], srcptr[1]);
                            //pixel_op(destptr[2], priptr[2], srcptr[2]);
                            //pixel_op(destptr[3], priptr[3], srcptr[3]);
                            switch (dest.bpp())
                            {
                                case 8: throw new emu_unimplemented();
                                case 16:
                                {
                                    var destptrTemp0 = destptr16[0]; var priptrTemp0 = priptr8[0]; pixel_op.op16x8x8(ref destptrTemp0, ref priptrTemp0, srcptr[0]); destptr16[0] = destptrTemp0; priptr8[0] = priptrTemp0;
                                    var destptrTemp1 = destptr16[1]; var priptrTemp1 = priptr8[1]; pixel_op.op16x8x8(ref destptrTemp1, ref priptrTemp1, srcptr[1]); destptr16[1] = destptrTemp1; priptr8[1] = priptrTemp1;
                                    var destptrTemp2 = destptr16[2]; var priptrTemp2 = priptr8[2]; pixel_op.op16x8x8(ref destptrTemp2, ref priptrTemp2, srcptr[2]); destptr16[2] = destptrTemp2; priptr8[2] = priptrTemp2;
                                    var destptrTemp3 = destptr16[3]; var priptrTemp3 = priptr8[3]; pixel_op.op16x8x8(ref destptrTemp3, ref priptrTemp3, srcptr[3]); destptr16[3] = destptrTemp3; priptr8[3] = priptrTemp3;
                                    break;
                                }
                                case 32: throw new emu_unimplemented();
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

                            //priptr += 4;
                            switch (priority.bpp())
                            {
                                case 8:  priptr8 += 4; break;
                                case 16: priptr16 += 4; break;
                                case 32: priptr32 += 4; break;
                                case 64: priptr64 += 4; break;
                                default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", priority.bpp());
                            }
                        }

                        // iterate over leftover pixels
                        for (s32 curx = 0; curx < leftovers; curx++)
                        {
                            //pixel_op(destptr[0], priptr[0], srcptr[0]);
                            switch (dest.bpp())
                            {
                                case 8: throw new emu_unimplemented();
                                case 16: { var destptrTemp0 = destptr16[0]; var priptrTemp0 = priptr8[0]; pixel_op.op16x8x8(ref destptrTemp0, ref priptrTemp0, srcptr[0]); destptr16[0] = destptrTemp0; priptr8[0] = priptrTemp0; break; }
                                case 32: throw new emu_unimplemented();
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

                            //priptr++;
                            switch (priority.bpp())
                            {
                                case 8:  priptr8++; break;
                                case 16: priptr16++; break;
                                case 32: priptr32++; break;
                                case 64: priptr64++; break;
                                default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", priority.bpp());
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
                        //auto *priptr = &priority.pix(cury, destx);
                        PointerU8 priptr8 = null;
                        PointerU16 priptr16 = null;
                        PointerU32 priptr32 = null;
                        PointerU64 priptr64 = null;
                        switch (priority.bpp())
                        {
                            case 8:  priptr8 = priority.pix8(cury, destx); break;
                            case 16: throw new emu_unimplemented();
                            case 32: throw new emu_unimplemented();
                            case 64: throw new emu_unimplemented();
                            default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", priority.bpp());
                        }

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
                            //pixel_op(destptr[0], priptr[0], srcptr[ 0]);
                            //pixel_op(destptr[1], priptr[1], srcptr[-1]);
                            //pixel_op(destptr[2], priptr[2], srcptr[-2]);
                            //pixel_op(destptr[3], priptr[3], srcptr[-3]);
                            switch (dest.bpp())
                            {
                                case 8: throw new emu_unimplemented();
                                case 16:
                                {
                                    var destptrTemp0 = destptr16[0]; var priptrTemp0 = priptr8[0]; pixel_op.op16x8x8(ref destptrTemp0, ref priptrTemp0, srcptr[0]); destptr16[0] = destptrTemp0; priptr8[0] = priptrTemp0;
                                    var destptrTemp1 = destptr16[1]; var priptrTemp1 = priptr8[1]; pixel_op.op16x8x8(ref destptrTemp1, ref priptrTemp1, srcptr[-1]); destptr16[1] = destptrTemp1; priptr8[1] = priptrTemp1;
                                    var destptrTemp2 = destptr16[2]; var priptrTemp2 = priptr8[2]; pixel_op.op16x8x8(ref destptrTemp2, ref priptrTemp2, srcptr[-2]); destptr16[2] = destptrTemp2; priptr8[2] = priptrTemp2;
                                    var destptrTemp3 = destptr16[3]; var priptrTemp3 = priptr8[3]; pixel_op.op16x8x8(ref destptrTemp3, ref priptrTemp3, srcptr[-3]); destptr16[3] = destptrTemp3; priptr8[3] = priptrTemp3;
                                    break;
                                }
                                case 32: throw new emu_unimplemented();
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

                            //priptr += 4;
                            switch (priority.bpp())
                            {
                                case 8:  priptr8 += 4; break;
                                case 16: priptr16 += 4; break;
                                case 32: priptr32 += 4; break;
                                case 64: priptr64 += 4; break;
                                default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", priority.bpp());
                            }
                        }

                        // iterate over leftover pixels
                        for (s32 curx = 0; curx < leftovers; curx++)
                        {
                            //pixel_op(destptr[0], priptr[0], srcptr[0]);
                            switch (dest.bpp())
                            {
                                case 8: throw new emu_unimplemented();
                                case 16: { var destptrTemp0 = destptr16[0]; var priptrTemp0 = priptr8[0]; pixel_op.op16x8x8(ref destptrTemp0, ref priptrTemp0, srcptr[0]); destptr16[0] = destptrTemp0; priptr8[0] = priptrTemp0; break; }
                                case 32: throw new emu_unimplemented();
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

                            //priptr++;
                            switch (priority.bpp())
                            {
                                case 8:  priptr8++; break;
                                case 16: priptr16++; break;
                                case 32: priptr32++; break;
                                case 64: priptr64++; break;
                                default: throw new emu_fatalerror("drawgfx_core() - unknown bpp - {0}\n", priority.bpp());
                            }
                        }
                    }
                }
            } while (false);

            g_profiler.stop();
        }
    }


    public static partial class drawgfxt_global
    {
        //template <typename BitmapType, typename FunctionClass>
        //inline void gfx_element::drawgfxzoom_core(BitmapType &dest, const rectangle &cliprect, u32 code, int flipx, int flipy, s32 destx, s32 desty, u32 scalex, u32 scaley, FunctionClass pixel_op)

        //template <typename BitmapType, typename PriorityType, typename FunctionClass>
        //inline void gfx_element::drawgfxzoom_core(BitmapType &dest, const rectangle &cliprect, u32 code, int flipx, int flipy, s32 destx, s32 desty, u32 scalex, u32 scaley, PriorityType &priority, FunctionClass pixel_op)


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
            g_profiler.start(profile_type.PROFILER_COPYBITMAP);

            do {
                assert(dest.valid());
                assert(src.valid());
                assert(dest.cliprect().contains(cliprect));

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
                            else if (dest.bpp() == 16 && src.bpp() == 16)
                            {
                                var destptrTemp0 = destptr16[0]; pixel_op.op16x16(ref destptrTemp0, srcptr16[0]); destptr16[0] = destptrTemp0;
                                var destptrTemp1 = destptr16[1]; pixel_op.op16x16(ref destptrTemp1, srcptr16[1]); destptr16[1] = destptrTemp1;
                                var destptrTemp2 = destptr16[2]; pixel_op.op16x16(ref destptrTemp2, srcptr16[2]); destptr16[2] = destptrTemp2;
                                var destptrTemp3 = destptr16[3]; pixel_op.op16x16(ref destptrTemp3, srcptr16[3]); destptr16[3] = destptrTemp3;
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
                            if      (dest.bpp() == 8  && src.bpp() == 8)  { var destptrTemp = destptr8[0];  pixel_op.op8x8(ref destptrTemp, srcptr8[0]);    destptr8[0] = destptrTemp; }
                            else if (dest.bpp() == 16 && src.bpp() == 16) { var destptrTemp = destptr16[0]; pixel_op.op16x16(ref destptrTemp, srcptr16[0]); destptr16[0] = destptrTemp; }
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

            g_profiler.stop();
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


        //template <typename BitmapType, typename FunctionClass>
        //inline void copyrozbitmap_core(BitmapType &dest, const rectangle &cliprect, const BitmapType &src, s32 startx, s32 starty, s32 incxx, s32 incxy, s32 incyx, s32 incyy, bool wraparound, FunctionClass pixel_op)

        //template <typename BitmapType, typename PriorityType, typename FunctionClass>
        //inline void copyrozbitmap_core(BitmapType &dest, const rectangle &cliprect, const BitmapType &src, s32 startx, s32 starty, s32 incxx, s32 incxy, s32 incyx, s32 incyy, bool wraparound, PriorityType &priority, FunctionClass pixel_op)


        /***************************************************************************
            BASIC DRAWSCANLINE CORE
        ***************************************************************************/

        /*
            Input parameters:

                bitmap_t &bitmap - the bitmap to copy to
                s32 destx - the X coordinate to copy to
                s32 desty - the Y coordinate to copy to
                s32 length - the total number of pixels to copy
                const UINTx *srcptr - pointer to memory containing the source pixels
                bitmap_t &priority - the priority bitmap (if and only if priority is to be applied)
        */

        //template <typename BitmapType, typename SourceType, typename FunctionClass>
        public static void drawscanline_core<BitmapType, BitmapType_PixelType, BitmapType_PixelType_OPS, BitmapType_PixelTypePointer>  //inline void drawscanline_core(BitmapType &bitmap, s32 destx, s32 desty, s32 length, const SourceType *srcptr, FunctionClass pixel_op)
        (
            BitmapType bitmap,
            s32 destx,
            s32 desty,
            s32 length,
            PointerU8 srcptr,
            gfx_element.FunctionClass pixel_op
        )
            where BitmapType : bitmap_specific<BitmapType_PixelType, BitmapType_PixelType_OPS, BitmapType_PixelTypePointer> 
            where BitmapType_PixelType_OPS : PixelType_operators, new()
            where BitmapType_PixelTypePointer : PointerU8
        {
            assert(bitmap.valid());
            assert(destx >= 0);
            assert(destx + length <= bitmap.width());
            assert(desty >= 0);
            assert(desty < bitmap.height());
            assert(srcptr != null);

            //auto *destptr = &bitmap.pix(desty, destx);
            PointerU8 destptr8 = null;
            PointerU16 destptr16 = null;
            PointerU32 destptr32 = null;
            PointerU64 destptr64 = null;
            switch (bitmap.bpp())
            {
                case 8:  destptr8 = bitmap.pix8(desty, destx); break;
                case 16: destptr16 = bitmap.pix16(desty, destx); break;
                case 32: destptr32 = bitmap.pix32(desty, destx); break;
                case 64: destptr64 = bitmap.pix64(desty, destx); break;
                default: throw new emu_fatalerror("drawscanline_core() - unknown bpp - {0}\n", bitmap.bpp());
            }

            // iterate over unrolled blocks of 4
            while (length >= 4)
            {
                //pixel_op(destptr[0], srcptr[0]);
                //pixel_op(destptr[1], srcptr[1]);
                //pixel_op(destptr[2], srcptr[2]);
                //pixel_op(destptr[3], srcptr[3]);
                if (bitmap.bpp() == 16 && srcptr is PointerU16)
                {
                    var destptrTemp0 = destptr16[0]; pixel_op.op16x16(ref destptrTemp0, ((PointerU16)srcptr)[0]); destptr16[0] = destptrTemp0;
                    var destptrTemp1 = destptr16[1]; pixel_op.op16x16(ref destptrTemp1, ((PointerU16)srcptr)[1]); destptr16[1] = destptrTemp1;
                    var destptrTemp2 = destptr16[2]; pixel_op.op16x16(ref destptrTemp2, ((PointerU16)srcptr)[2]); destptr16[2] = destptrTemp2;
                    var destptrTemp3 = destptr16[3]; pixel_op.op16x16(ref destptrTemp3, ((PointerU16)srcptr)[3]); destptr16[3] = destptrTemp3;
                }
                else throw new emu_fatalerror("drawscanline_core() - unknown bpp - dest: {0} src: {1}\n", bitmap.bpp(), srcptr.GetType());

                length -= 4;

                //srcptr += 4;
                if (srcptr is PointerU16) { srcptr = (PointerU16)srcptr + 4; }
                else throw new emu_fatalerror("drawscanline_core() - unknown bpp - dest: {0} src: {1}\n", bitmap.bpp(), srcptr.GetType());

                //destptr += 4;
                switch (bitmap.bpp())
                {
                    case 8:  destptr8 += 4; break;
                    case 16: destptr16 += 4; break;
                    case 32: destptr32 += 4; break;
                    case 64: destptr64 += 4; break;
                    default: throw new emu_fatalerror("drawscanline_core() - unknown bpp - {0}\n", bitmap.bpp());
                }
            }

            // iterate over leftover pixels
            while (length-- > 0)
            {
                //pixel_op(destptr[0], srcptr[0]);
                if (bitmap.bpp() == 16 && srcptr is PointerU16)
                {
                    var destptrTemp = destptr16[0]; pixel_op.op16x16(ref destptrTemp, ((PointerU16)srcptr)[0]); destptr16[0] = destptrTemp;
                }
                else throw new emu_fatalerror("drawscanline_core() - unknown bpp - dest: {0} src: {1}\n", bitmap.bpp(), srcptr.GetType());

                //srcptr++;
                if (srcptr is PointerU16) { srcptr = (PointerU16)srcptr + 1; }
                else throw new emu_fatalerror("drawscanline_core() - unknown bpp - dest: {0} src: {1}\n", bitmap.bpp(), srcptr.GetType());

                //destptr++;
                switch (bitmap.bpp())
                {
                    case 8:  destptr8++; break;
                    case 16: destptr16++; break;
                    case 32: destptr32++; break;
                    case 64: destptr64++; break;
                    default: throw new emu_fatalerror("drawscanline_core() - unknown bpp - {0}\n", bitmap.bpp());
                }
            }
        }


        //template <typename BitmapType, typename SourceType, typename PriorityType, typename FunctionClass>
        //inline void drawscanline_core(BitmapType &bitmap, s32 destx, s32 desty, s32 length, const SourceType *srcptr, PriorityType &priority, FunctionClass pixel_op)
    }
}
