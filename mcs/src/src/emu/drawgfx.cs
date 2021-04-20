// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using pen_t = System.UInt32;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;


namespace mame
{
    public partial class gfx_element : global_object
    {
        // internal state
        device_palette_interface m_palette;    // palette used for drawing (optional when used as a pure decoder)

        u16 m_width;                // current pixel width of each element (changeable with source clipping)
        u16 m_height;               // current pixel height of each element (changeable with source clipping)
        u16 m_startx;               // current source clip X offset
        u16 m_starty;               // current source clip Y offset

        u16 m_origwidth;            // starting pixel width of each element
        u16 m_origheight;           // staring pixel height of each element
        u32 m_total_elements;       // total number of decoded elements

        u32 m_color_base;           // base color for rendering
        u16 m_color_depth;          // number of colors each pixel can represent
        u16 m_color_granularity;    // number of colors for each color code
        u32 m_total_colors;         // number of color codes

        u32 m_line_modulo;          // bytes between each row of data
        u32 m_char_modulo;          // bytes between each element
        Pointer<u8> m_srcdata;  //const u8 *   m_srcdata;              // pointer to the source data for decoding
        u32 m_dirtyseq;             // sequence number; incremented each time a tile is dirtied

        Pointer<u8> m_gfxdata;  //u8 *         m_gfxdata;              // pointer to decoded pixel data, 8bpp
        std.vector<u8> m_gfxdata_allocated = new std.vector<u8>();   // allocated decoded pixel data, 8bpp
        std.vector<u8> m_dirty = new std.vector<u8>();   // dirty array for detecting chars that need decoding
        std.vector<u32> m_pen_usage = new std.vector<u32>();   // bitmask of pens that are used (pens 0-31 only)

        bool m_layout_is_raw;        // raw layout?
        u8 m_layout_planes;        // bit planes in the layout
        u32 m_layout_xormask;       // xor mask applied to each bit offset
        u32 m_layout_charincrement; // per-character increment in source data
        std.vector<u32> m_layout_planeoffset = new std.vector<u32>();   // plane offsets
        std.vector<u32> m_layout_xoffset = new std.vector<u32>();   // X offsets
        std.vector<u32> m_layout_yoffset = new std.vector<u32>();   // Y offsets


        // construction/destruction
        //-------------------------------------------------
        //  gfx_element - constructor
        //-------------------------------------------------
        public gfx_element(device_palette_interface palette, Pointer<u8> base_, u16 width, u16 height, u32 rowbytes, u32 total_colors, u32 color_base, u32 color_granularity)  //gfx_element::gfx_element(device_palette_interface *palette, u8 *base, u16 width, u16 height, u32 rowbytes, u32 total_colors, u32 color_base, u32 color_granularity)
        {
            m_palette = palette;
            m_width = width;
            m_height = height;
            m_startx = 0;
            m_starty = 0;
            m_origwidth = width;
            m_origheight = height;
            m_total_elements = 1;
            m_color_base = color_base;
            m_color_depth = (UInt16)color_granularity;
            m_color_granularity = (UInt16)color_granularity;
            m_total_colors = (total_colors - color_base) / color_granularity;
            m_line_modulo = rowbytes;
            m_char_modulo = 0;
            m_srcdata = base_;
            m_dirtyseq = 1;
            m_gfxdata = base_;
            m_layout_is_raw = true;
            m_layout_planes = 0;
            m_layout_xormask = 0;
            m_layout_charincrement = 0;
        }

        public gfx_element(device_palette_interface palette, gfx_layout gl, Pointer<u8> srcdata, u32 xormask, u32 total_colors, u32 color_base)  //gfx_element::gfx_element(device_palette_interface *palette, const gfx_layout &gl, const u8 *srcdata, u32 xormask, u32 total_colors, u32 color_base)
        {
            m_palette = palette;
            m_width = 0;
            m_height = 0;
            m_startx = 0;
            m_starty = 0;
            m_origwidth = 0;
            m_origheight = 0;
            m_total_elements = 0;
            m_color_base = color_base;
            m_color_depth = 0;
            m_color_granularity = 0;
            m_total_colors = total_colors;
            m_line_modulo = 0;
            m_char_modulo = 0;
            m_srcdata = null;
            m_dirtyseq = 1;
            m_gfxdata = null;
            m_layout_is_raw = false;
            m_layout_planes = 0;
            m_layout_xormask = xormask;
            m_layout_charincrement = 0;


            // set the layout
            set_layout(gl, srcdata);
        }


        // getters
        public device_palette_interface palette() { return m_palette; }
        public u16 width() { return m_width; }
        public u16 height() { return m_height; }
        public u32 elements() { return m_total_elements; }
        public u32 colorbase() { return m_color_base; }
        public u16 depth() { return m_color_depth; }
        public u16 granularity() { return m_color_granularity; }
        public u32 colors() { return m_total_colors; }
        public u32 rowbytes() { return m_line_modulo; }
        bool has_pen_usage() { return !m_pen_usage.empty(); }
        public bool has_palette() { return m_palette != null; }


        // used by tilemaps
        public u32 dirtyseq() { return m_dirtyseq; }


        // setters

        //-------------------------------------------------
        //  set_layout - set the layout for a gfx_element
        //-------------------------------------------------
        void set_layout(gfx_layout gl, Pointer<u8> srcdata)  //void set_layout(const gfx_layout &gl, const u8 *srcdata)
        {
            m_srcdata = srcdata;

            // configure ourselves
            m_width = m_origwidth = gl.width;
            m_height = m_origheight = gl.height;
            m_startx = m_starty = 0;
            m_total_elements = gl.total;
            m_color_granularity = (UInt16)(1 << gl.planes);
            m_color_depth = m_color_granularity;

            // copy data from the layout
            m_layout_is_raw = gl.planeoffset[0] == digfx_global.GFX_RAW;
            m_layout_planes = (byte)gl.planes;
            m_layout_charincrement = gl.charincrement;

            // raw graphics case
            if (m_layout_is_raw)
            {
                // RAW layouts don't need these arrays
                m_layout_planeoffset.clear();
                m_layout_xoffset.clear();
                m_layout_yoffset.clear();
                m_gfxdata_allocated.clear();

                // modulos are determined for us by the layout
                m_line_modulo = gl.yoffs(0) / 8;
                m_char_modulo = gl.charincrement / 8;

                // RAW graphics must have a pointer up front
                //assert(srcdata != NULL);
                m_gfxdata = new Pointer<u8>(srcdata);  //m_gfxdata = const_cast<u8 *>(srcdata);
            }

            // decoded graphics case
            else
            {
                // copy offsets
                m_layout_planeoffset.resize(m_layout_planes);
                m_layout_xoffset.resize(m_width);
                m_layout_yoffset.resize(m_height);

                for (int p = 0; p < m_layout_planes; p++)
                    m_layout_planeoffset[p] = gl.planeoffset[p];
                for (int y = 0; y < m_height; y++)
                    m_layout_yoffset[y] = gl.yoffs(y);
                for (int x = 0; x < m_width; x++)
                    m_layout_xoffset[x] = gl.xoffs(x);

                // we get to pick our own modulos
                m_line_modulo = m_origwidth;
                m_char_modulo = m_line_modulo * m_origheight;

                // allocate memory for the data
                m_gfxdata_allocated.resize((int)(m_total_elements * m_char_modulo));
                m_gfxdata = new Pointer<u8>(m_gfxdata_allocated);  //m_gfxdata = &m_gfxdata_allocated[0];
            }

            // mark everything dirty
            m_dirty.resize((int)m_total_elements);
            memset(m_dirty, (u8)1, m_total_elements);

            // allocate a pen usage array for entries with 32 pens or less
            if (m_color_depth <= 32)
                m_pen_usage.resize((int)m_total_elements);
            else
                m_pen_usage.clear();
        }


        //void set_raw_layout(const UINT8 *srcdata, UINT32 width, UINT32 height, UINT32 total, UINT32 linemod, UINT32 charmod);


        //-------------------------------------------------
        // set_source - set the source data for a gfx_element
        //-------------------------------------------------
        public void set_source(Pointer<u8> source)  //void set_source(const u8 *source)
        {
            m_srcdata = source;
            memset(m_dirty, (u8)1, elements());  //memset(&m_dirty[0], 1, elements());
            if (m_layout_is_raw) m_gfxdata = source;  //if (m_layout_is_raw) m_gfxdata = const_cast<u8 *>(source);
        }


        //void set_source_and_total(const u8 *source, u32 total);
        //void set_xormask(u32 xormask) { m_layout_xormask = xormask; }
        //void set_palette(device_palette_interface &palette) { m_palette = &palette; }
        public void set_colors(u32 colors) { m_total_colors = colors; }
        public void set_colorbase(u16 colorbase) { m_color_base = colorbase; }
        public void set_granularity(u16 granularity) { m_color_granularity = granularity; }
        //void set_source_clip(u32 xoffs, u32 width, u32 yoffs, u32 height);


        // operations
        public void mark_dirty(u32 code) { if (code < elements()) { m_dirty[code] = 1; m_dirtyseq++; } }
        //void mark_all_dirty() { memset(&m_dirty[0], 1, elements()); }

        public Pointer<u8> get_data(u32 code)  //const u8 *get_data(u32 code)
        {
            //assert(code < elements());
            if (code < m_dirty.size() && m_dirty[(int)code] != 0)
                decode(code);

            return new Pointer<u8>(m_gfxdata, (int)(code * m_char_modulo + m_starty * m_line_modulo + m_startx));
        }

        u32 pen_usage(u32 code)
        {
            //assert(code < m_pen_usage.count());
            if (m_dirty[(int)code] != 0)
                decode(code);

            return m_pen_usage[(int)code];
        }


        // ----- core graphics drawing -----

        // specific drawgfx implementations for each transparency type

        // drawgfxt.cs
        // core drawgfx implementation
        //template <typename BitmapType, typename FunctionClass> void drawgfx_core(BitmapType &dest, const rectangle &cliprect, u32 code, int flipx, int flipy, s32 destx, s32 desty, FunctionClass pixel_op);


        /*-------------------------------------------------
            opaque - render a gfx element with
            no transparency
        -------------------------------------------------*/
        void opaque(bitmap_ind16 dest, rectangle cliprect,
                u32 code, u32 color, int flipx, int flipy, s32 destx, s32 desty)
        {
            color = colorbase() + granularity() * (color % colors());
            code %= elements();
            drawgfx_core<bitmap_ind16, u16, PointerU16>(dest, cliprect, code, flipx, flipy, destx, desty, new FunctionClass((ref u16 destp, u8 srcp) => { drawgfxt_global.PIXEL_OP_REBASE_OPAQUE(color, ref destp, srcp); }));  //drawgfx_core(dest, cliprect, code, flipx, flipy, destx, desty, [color](u16 &destp, const u8 &srcp) { PIXEL_OP_REBASE_OPAQUE(destp, srcp); });
        }


        void opaque(bitmap_rgb32 dest, rectangle cliprect,
                u32 code, u32 color, int flipx, int flipy, s32 destx, s32 desty)
        {
            pen_t paldata = m_palette.pens()[colorbase() + granularity() * (color % colors())];  //const pen_t *paldata = m_palette->pens() + colorbase() + granularity() * (color % colors());
            code %= elements();
            throw new emu_unimplemented();
#if false
            drawgfx_core(dest, cliprect, code, flipx, flipy, destx, desty, [paldata](u32 &destp, const u8 &srcp) { PIXEL_OP_REMAP_OPAQUE(destp, srcp); });
#endif
        }


        /*-------------------------------------------------
            transpen - render a gfx element with
            a single transparent pen
        -------------------------------------------------*/

        public void transpen(bitmap_ind16 dest, rectangle cliprect,
                u32 code, u32 color, int flipx, int flipy, s32 destx, s32 desty,
                u32 trans_pen)
        {
            // special case invalid pens to opaque
            if (trans_pen > 0xff)
            {
                opaque(dest, cliprect, code, color, flipx, flipy, destx, desty);
                return;
            }

            // use pen usage to optimize
            code %= elements();
            if (has_pen_usage())
            {
                // fully transparent; do nothing
                u32 usage = pen_usage(code);
                if ((usage & ~(1 << (int)trans_pen)) == 0)
                    return;

                // fully opaque; draw as such
                if ((usage & (1 << (int)trans_pen)) == 0)
                {
                    opaque(dest, cliprect, code, color, flipx, flipy, destx, desty);
                    return;
                }
            }

            // render
            color = colorbase() + granularity() * (color % colors());
            drawgfx_core<bitmap_ind16, u16, PointerU16>(dest, cliprect, code, flipx, flipy, destx, desty, new FunctionClass((ref u16 destp, u8 srcp) => { drawgfxt_global.PIXEL_OP_REBASE_TRANSPEN(color, trans_pen, ref destp, srcp); }));  //drawgfx_core(dest, cliprect, code, flipx, flipy, destx, desty, [trans_pen, color](u16 &destp, const u8 &srcp) { PIXEL_OP_REBASE_TRANSPEN(destp, srcp); });
        }


        public void transpen(bitmap_rgb32 dest, rectangle cliprect,
                u32 code, u32 color, int flipx, int flipy, s32 destx, s32 desty,
                u32 trans_pen)
        {
            // special case invalid pens to opaque
            if (trans_pen > 0xff)
            {
                opaque(dest, cliprect, code, color, flipx, flipy, destx, desty);
                return;
            }

            // use pen usage to optimize
            code %= elements();
            if (has_pen_usage())
            {
                // fully transparent; do nothing
                u32 usage = pen_usage(code);
                if ((usage & ~(1 << (int)trans_pen)) == 0)
                    return;

                // fully opaque; draw as such
                if ((usage & (1 << (int)trans_pen)) == 0)
                {
                    opaque(dest, cliprect, code, color, flipx, flipy, destx, desty);
                    return;
                }
            }

            // render
            Pointer<rgb_t> paldata = new Pointer<rgb_t>(m_palette.pens(), (int)(colorbase() + granularity() * (color % colors())));  //const pen_t *paldata = m_palette.pens() + colorbase() + granularity() * (color % colors());
            drawgfx_core<bitmap_rgb32, u32, PointerU32>(dest, cliprect, code, flipx, flipy, destx, desty, new FunctionClass((ref u32 destp, u8 srcp) => { drawgfxt_global.PIXEL_OP_REMAP_TRANSPEN(trans_pen, paldata, ref destp, srcp); }));  //drawgfx_core(dest, cliprect, code, flipx, flipy, destx, desty, [trans_pen, paldata](u32 &destp, const u8 &srcp) { PIXEL_OP_REMAP_TRANSPEN(destp, srcp); });
        }


        /*-------------------------------------------------
            transpen_raw - render a gfx element
            with a single transparent pen and no color
            lookups
        -------------------------------------------------*/
        public void transpen_raw(bitmap_ind16 dest, rectangle cliprect,
                u32 code, u32 color, int flipx, int flipy, s32 destx, s32 desty,
                u32 trans_pen)
        {
            // early out if completely transparent
            code %= elements();
            if (has_pen_usage() && (pen_usage(code) & ~(1U << (int)trans_pen)) == 0)
                return;

            // render
            drawgfx_core<bitmap_ind16, u16, PointerU16>(dest, cliprect, code, flipx, flipy, destx, desty, new FunctionClass((ref u16 destp, u8 srcp) => { drawgfxt_global.PIXEL_OP_REBASE_TRANSPEN(color, trans_pen, ref destp, srcp); }));  //drawgfx_core(dest, cliprect, code, flipx, flipy, destx, desty, [trans_pen, color](u16 &destp, const u8 &srcp) { PIXEL_OP_REBASE_TRANSPEN(destp, srcp); });
        }

        public void transpen_raw(bitmap_rgb32 dest, rectangle cliprect,
                u32 code, u32 color, int flipx, int flipy, s32 destx, s32 desty,
                u32 trans_pen)
        {
            throw new emu_unimplemented();
        }


        /*-------------------------------------------------
            transmask - render a gfx element
            with a multiple transparent pens provided as
            a mask
        -------------------------------------------------*/
        public void transmask(bitmap_ind16 dest, rectangle cliprect,
                u32 code, u32 color, int flipx, int flipy, s32 destx, s32 desty,
                u32 trans_mask)
        {
            // special case 0 mask to opaque
            if (trans_mask == 0)
            {
                opaque(dest, cliprect, code, color, flipx, flipy, destx, desty);
                return;
            }

            // use pen usage to optimize
            code %= elements();
            if (has_pen_usage())
            {
                // fully transparent; do nothing
                u32 usage = pen_usage(code);
                if ((usage & ~trans_mask) == 0)
                    return;

                // fully opaque; draw as such
                if ((usage & trans_mask) == 0)
                {
                    opaque(dest, cliprect, code, color, flipx, flipy, destx, desty);
                    return;
                }
            }

            // render
            color = colorbase() + granularity() * (color % colors());
            drawgfx_core<bitmap_ind16, u16, PointerU16>(dest, cliprect, code, flipx, flipy, destx, desty, new FunctionClass((ref u16 destp, u8 srcp) => { drawgfxt_global.PIXEL_OP_REBASE_TRANSMASK(color, trans_mask, ref destp, srcp); }));  //drawgfx_core(dest, cliprect, code, flipx, flipy, destx, desty, [trans_mask, color](u16 &destp, const u8 &srcp) { PIXEL_OP_REBASE_TRANSMASK(destp, srcp); });
        }

        public void transmask(bitmap_rgb32 dest, rectangle cliprect,
                u32 code, u32 color, int flipx, int flipy, s32 destx, s32 desty,
                u32 trans_mask)
        {
            // special case 0 mask to opaque
            if (trans_mask == 0)
            {
                opaque(dest, cliprect, code, color, flipx, flipy, destx, desty);
                return;
            }

            // use pen usage to optimize
            code %= elements();
            if (has_pen_usage())
            {
                // fully transparent; do nothing
                u32 usage = pen_usage(code);
                if ((usage & ~trans_mask) == 0)
                    return;

                // fully opaque; draw as such
                if ((usage & trans_mask) == 0)
                {
                    opaque(dest, cliprect, code, color, flipx, flipy, destx, desty);
                    return;
                }
            }

            // render
            pen_t paldata = m_palette.pens()[colorbase() + granularity() * (color % colors())];  //const pen_t *paldata = m_palette->pens() + colorbase() + granularity() * (color % colors());
            throw new emu_unimplemented();
#if false
            drawgfx_core(dest, cliprect, code, flipx, flipy, destx, desty, [trans_mask, paldata](u32 &destp, const u8 &srcp) { PIXEL_OP_REMAP_TRANSMASK(destp, srcp); });
#endif
        }


        //void transtable(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, const UINT8 *pentable);
        //void transtable(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, const UINT8 *pentable);
        //void alpha(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 transpen, UINT8 alpha);

        // ----- zoomed graphics drawing -----

        // core zoom implementation
        //template <typename BitmapType, typename FunctionClass> void drawgfxzoom_core(BitmapType &dest, const rectangle &cliprect, u32 code, int flipx, int flipy, s32 destx, s32 desty, u32 scalex, u32 scaley, FunctionClass pixel_op);

        // specific zoom implementations for each transparency type
        //void zoom_opaque(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley);
        //void zoom_opaque(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley);
        //void zoom_transpen(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, UINT32 transpen);
        //void zoom_transpen(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, UINT32 transpen);
        //void zoom_transpen_raw(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, UINT32 transpen);
        //void zoom_transpen_raw(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, UINT32 transpen);
        //void zoom_transmask(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, UINT32 transmask);
        //void zoom_transmask(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, UINT32 transmask);
        //void zoom_transtable(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, const UINT8 *pentable);
        //void zoom_transtable(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, const UINT8 *pentable);
        //void zoom_alpha(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, UINT32 transpen, UINT8 alpha);

        // ----- priority masked graphics drawing -----

        // core prio implementation
        //template <typename BitmapType, typename PriorityType, typename FunctionClass> void drawgfx_core(BitmapType &dest, const rectangle &cliprect, u32 code, int flipx, int flipy, s32 destx, s32 desty, PriorityType &priority, FunctionClass pixel_op);

        // specific prio implementations for each transparency type
        //void prio_opaque(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, bitmap_ind8 &priority, UINT32 pmask);
        //void prio_opaque(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, bitmap_ind8 &priority, UINT32 pmask);
        //void prio_transpen(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, bitmap_ind8 &priority, UINT32 pmask, UINT32 transpen);
        //void prio_transpen(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, bitmap_ind8 &priority, UINT32 pmask, UINT32 transpen);
        //void prio_transpen_raw(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, bitmap_ind8 &priority, UINT32 pmask, UINT32 transpen);
        //void prio_transpen_raw(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, bitmap_ind8 &priority, UINT32 pmask, UINT32 transpen);
        //void prio_transmask(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, bitmap_ind8 &priority, UINT32 pmask, UINT32 transmask);
        //void prio_transmask(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, bitmap_ind8 &priority, UINT32 pmask, UINT32 transmask);
        //void prio_transtable(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, bitmap_ind8 &priority, UINT32 pmask, const UINT8 *pentable);
        //void prio_transtable(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, bitmap_ind8 &priority, UINT32 pmask, const UINT8 *pentable);
        //void prio_alpha(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, bitmap_ind8 &priority, UINT32 pmask, UINT32 transpen, UINT8 alpha);

        // ----- priority masked zoomed graphics drawing -----

        // core prio_zoom implementation
        //template <typename BitmapType, typename PriorityType, typename FunctionClass> void drawgfxzoom_core(BitmapType &dest, const rectangle &cliprect, u32 code, int flipx, int flipy, s32 destx, s32 desty, u32 scalex, u32 scaley, PriorityType &priority, FunctionClass pixel_op);

        // specific prio_zoom implementations for each transparency type
        //void prio_zoom_opaque(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, bitmap_ind8 &priority, UINT32 pmask);
        //void prio_zoom_opaque(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, bitmap_ind8 &priority, UINT32 pmask);
        //void prio_zoom_transpen(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, bitmap_ind8 &priority, UINT32 pmask, UINT32 transpen);
        //void prio_zoom_transpen(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, bitmap_ind8 &priority, UINT32 pmask, UINT32 transpen);
        //void prio_zoom_transpen_raw(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, bitmap_ind8 &priority, UINT32 pmask, UINT32 transpen);
        //void prio_zoom_transpen_raw(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, bitmap_ind8 &priority, UINT32 pmask, UINT32 transpen);
        //void prio_zoom_transmask(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, bitmap_ind8 &priority, UINT32 pmask, UINT32 transmask);
        //void prio_zoom_transmask(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, bitmap_ind8 &priority, UINT32 pmask, UINT32 transmask);
        //void prio_zoom_transtable(bitmap_ind16 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, bitmap_ind8 &priority, UINT32 pmask, const UINT8 *pentable);
        //void prio_zoom_transtable(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, bitmap_ind8 &priority, UINT32 pmask, const UINT8 *pentable);
        //void prio_zoom_alpha(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, UINT32 scalex, UINT32 scaley, bitmap_ind8 &priority, UINT32 pmask, UINT32 transpen, UINT8 alpha);

        // implementations moved here from specific drivers
        //void prio_transpen_additive(bitmap_rgb32 &dest, const rectangle &cliprect, UINT32 code, UINT32 color, int flipx, int flipy, INT32 destx, INT32 desty, bitmap_ind8 &priority, UINT32 pmask, UINT32 trans_pen);
        //void alphastore(bitmap_rgb32 &dest, const rectangle &cliprect, u32 code, u32 color, int flipx, int flipy, s32 destx, s32 desty, int fixedalpha, u8 *alphatable);
        //void prio_zoom_transpen_additive(bitmap_rgb32 &dest, const rectangle &cliprect, u32 code, u32 color, int flipx, int flipy, s32 destx, s32 desty, u32 scalex, u32 scaley, bitmap_ind8 &priority, u32 pmask, u32 trans_pen);
        //void alphatable(bitmap_rgb32 &dest, const rectangle &cliprect, u32 code, u32 color, int flipx, int flipy, s32 destx, s32 desty, int fixedalpha, u8 *alphatable);


        // internal helpers
        //-------------------------------------------------
        //  decode - decode a single character
        //-------------------------------------------------
        void decode(u32 code)
        {
            // don't decode GFX_RAW
            if (!m_layout_is_raw)
            {
                // zap the data to 0
                Pointer<u8> decode_base = new Pointer<u8>(m_gfxdata, (int)(code * m_char_modulo));  //u8 *decode_base = m_gfxdata + code * m_char_modulo;
                memset(decode_base, (u8)0, m_char_modulo);  //memset(decode_base, 0, m_char_modulo);

                // iterate over planes
                int plane;
                int planebit;
                for (plane = 0, planebit = 1 << (m_layout_planes - 1);
                        plane < m_layout_planes;
                        plane++, planebit >>= 1)
                {
                    int planeoffs = (int)(code * m_layout_charincrement + m_layout_planeoffset[plane]);

                    // iterate over rows
                    for (int y = 0; y < m_origheight; y++)
                    {
                        int yoffs = (int)(planeoffs + m_layout_yoffset[y]);
                        Pointer<u8> dp = new Pointer<u8>(decode_base, (int)(y * m_line_modulo));  //u8 *dp = decode_base + y * m_line_modulo;

                        // iterate over columns
                        for (int x = 0; x < m_origwidth; x++)
                        {
                            if (drawgfx_global.readbit(m_srcdata, (UInt32)((yoffs + m_layout_xoffset[x]) ^ m_layout_xormask)) != 0)
                                dp[x] |= (byte)planebit;
                        }
                    }
                }
            }

            // (re)compute pen usage
            if (code < m_pen_usage.size())
            {
                // iterate over data, creating a bitmask of live pens
                Pointer<u8> dp = new Pointer<u8>(m_gfxdata, (int)(code * m_char_modulo));  //const u8 *dp = m_gfxdata + code * m_char_modulo;
                u32 usage = 0;
                for (int y = 0; y < m_origheight; y++)
                {
                    for (int x = 0; x < m_origwidth; x++)
                        usage |= 1U << dp[x];

                    dp += m_line_modulo;
                }

                // store the final result
                m_pen_usage[(int)code] = usage;
            }

            // no longer dirty
            m_dirty[(int)code] = 0;
        }
    }


    // ======================> gfxdecode_device
    public class gfxdecode_device : device_t
                                    //device_gfx_interface
    {
        //DEFINE_DEVICE_TYPE(GFXDECODE, gfxdecode_device, "gfxdecode", "gfxdecode")
        static device_t device_creator_gfxdecode_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new gfxdecode_device(mconfig, tag, owner, clock); }
        public static readonly device_type GFXDECODE = DEFINE_DEVICE_TYPE(device_creator_gfxdecode_device, "gfxdecode", "gfxdecode");


        device_gfx_interface m_digfx;


        // construction/destruction
        //template <typename T>
        public gfxdecode_device(machine_config mconfig, string tag, device_t owner, string palette_tag, gfx_decode_entry [] gfxinfo)
            : this(mconfig, tag, owner, 0)
        {
            m_digfx = GetClassInterface<device_gfx_interface>();
            m_digfx.set_palette(palette_tag);
            m_digfx.set_info(gfxinfo);
        }

        public gfxdecode_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : base(mconfig, GFXDECODE, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_gfx_interface(mconfig, this));
        }

        //template <typename T>
        public void gfxdecode_device_after_ctor(string palette_tag, gfx_decode_entry [] gfxinfo)
        {
            m_digfx = GetClassInterface<device_gfx_interface>();
            m_digfx.set_palette(palette_tag);
            m_digfx.set_info(gfxinfo);
        }

        public void gfxdecode_device_after_ctor(finder_base palette, gfx_decode_entry [] gfxinfo)
        {
            m_digfx = GetClassInterface<device_gfx_interface>();
            m_digfx.set_palette(palette);
            m_digfx.set_info(gfxinfo);
        }


        public device_gfx_interface digfx { get { return m_digfx; } }


        protected override void device_start() { }
    }


    static class drawgfx_global
    {
        // ----- bitmap copying -----

        /***************************************************************************
            COPYBITMAP IMPLEMENTATIONS
        ***************************************************************************/

        /*-------------------------------------------------
            copybitmap - copy from one bitmap to another,
            copying all unclipped pixels
        -------------------------------------------------*/
        // copy from one bitmap to another, copying all unclipped pixels
        static void copybitmap(bitmap_ind16 dest, bitmap_ind16 src, int flipx, int flipy, s32 destx, s32 desty, rectangle cliprect)
        {
            drawgfxt_global.copybitmap_core<bitmap_ind16, u16, PointerU16>(dest, src, flipx, flipy, destx, desty, cliprect, new gfx_element.FunctionClass((ref u16 destp, u16 srcp) => { drawgfxt_global.PIXEL_OP_COPY_OPAQUE(ref destp, srcp); }));  //copybitmap_core(dest, src, flipx, flipy, destx, desty, cliprect, [](u16 &destp, const u16 &srcp) { PIXEL_OP_COPY_OPAQUE(destp, srcp); });
        }

        static void copybitmap(bitmap_rgb32 dest, bitmap_rgb32 src, int flipx, int flipy, s32 destx, s32 desty, rectangle cliprect)
        {
            throw new emu_unimplemented();
        }


        /*-------------------------------------------------
            copybitmap_trans - copy from one bitmap to
            another, copying all unclipped pixels except
            those that match transpen
        -------------------------------------------------*/
        // copy from one bitmap to another, copying all unclipped pixels except those that match transpen
        static void copybitmap_trans(bitmap_ind16 dest, bitmap_ind16 src, int flipx, int flipy, s32 destx, s32 desty, rectangle cliprect, u32 trans_pen)
        {
            if (trans_pen > 0xffff)
                copybitmap(dest, src, flipx, flipy, destx, desty, cliprect);
            else
                drawgfxt_global.copybitmap_core<bitmap_ind16, u16, PointerU16>(dest, src, flipx, flipy, destx, desty, cliprect, new gfx_element.FunctionClass((ref u16 destp, u16 srcp) => { drawgfxt_global.PIXEL_OP_COPY_TRANSPEN(trans_pen, ref destp, srcp); }));  //copybitmap_core(dest, src, flipx, flipy, destx, desty, cliprect, [trans_pen](u16 &destp, const u16 &srcp) { PIXEL_OP_COPY_TRANSPEN(destp, srcp); });
        }

        static void copybitmap_trans(bitmap_rgb32 dest, bitmap_rgb32 src, int flipx, int flipy, s32 destx, s32 desty, rectangle cliprect, u32 transpen)
        {
            throw new emu_unimplemented();
        }


        //void copybitmap_transalpha(bitmap_rgb32 &dest, const bitmap_rgb32 &src, int flipx, int flipy, s32 destx, s32 desty, const rectangle &cliprect);


        /*
          Copy a bitmap onto another with scroll and wraparound.
          These functions support multiple independently scrolling rows/columns.
          "rows" is the number of independently scrolling rows. "rowscroll" is an
          array of integers telling how much to scroll each row. Same thing for
          "numcols" and "colscroll".
          If the bitmap cannot scroll in one direction, set numrows or columns to 0.
          If the bitmap scrolls as a whole, set numrows and/or numcols to 1.
          Bidirectional scrolling is, of course, supported only if the bitmap
          scrolls as a whole in at least one direction.
        */

        /***************************************************************************
            COPYSCROLLBITMAP IMPLEMENTATIONS
        ***************************************************************************/

        /*-------------------------------------------------
            copyscrollbitmap - copy from one bitmap to
            another, copying all unclipped pixels, and
            applying scrolling to one or more rows/columns
        -------------------------------------------------*/
        // copy from one bitmap to another, copying all unclipped pixels, and applying scrolling to one or more rows/columns
        public static void copyscrollbitmap(bitmap_ind16 dest, bitmap_ind16 src, u32 numrows, s32 [] rowscroll, u32 numcols, s32 [] colscroll, rectangle cliprect)  //void copyscrollbitmap(bitmap_ind16 &dest, const bitmap_ind16 &src, u32 numrows, const s32 *rowscroll, u32 numcols, const s32 *colscroll, const rectangle &cliprect)
        {
            // just call through to the transparent case as the underlying copybitmap will
            // optimize for pen == 0xffffffff
            copyscrollbitmap_trans(dest, src, numrows, rowscroll, numcols, colscroll, cliprect, 0xffffffff);
        }

        //void copyscrollbitmap(bitmap_rgb32 &dest, const bitmap_rgb32 &src, u32 numrows, const s32 *rowscroll, u32 numcols, const s32 *colscroll, const rectangle &cliprect);


        /*-------------------------------------------------
            copyscrollbitmap_trans - copy from one bitmap
            to another, copying all unclipped pixels
            except those that match transpen, and applying
            scrolling to one or more rows/columns
        -------------------------------------------------*/
        //template<class _BitmapClass>
        static void copyscrollbitmap_trans_common(bitmap_ind16 dest, bitmap_ind16 src, u32 numrows, s32 [] rowscroll, u32 numcols, s32 [] colscroll, rectangle cliprect, u32 trans_pen)  //static inline void copyscrollbitmap_trans_common(_BitmapClass &dest, const _BitmapClass &src, u32 numrows, const s32 *rowscroll, u32 numcols, const s32 *colscroll, const rectangle &cliprect, u32 trans_pen)
        {
            // no rowscroll and no colscroll means no scroll
            if (numrows == 0 && numcols == 0)
            {
                copybitmap_trans(dest, src, 0, 0, 0, 0, cliprect, trans_pen);
                return;
            }

            global_object.assert(numrows != 0 || rowscroll == null);
            global_object.assert(numrows == 0 || rowscroll != null);
            global_object.assert(numcols != 0 || colscroll == null);
            global_object.assert(numcols == 0 || colscroll != null);

            // fully scrolling X,Y playfield
            if (numrows <= 1 && numcols <= 1)
            {
                s32 xscroll = normalize_xscroll(src, (numrows == 0) ? 0 : rowscroll[0]);
                s32 yscroll = normalize_yscroll(src, (numcols == 0) ? 0 : colscroll[0]);

                // iterate over all portions of the scroll that overlap the destination
                for (s32 sx = xscroll - src.width(); sx < dest.width(); sx += src.width())
                    for (s32 sy = yscroll - src.height(); sy < dest.height(); sy += src.height())
                        copybitmap_trans(dest, src, 0, 0, sx, sy, cliprect, trans_pen);
            }

            // scrolling columns plus horizontal scroll
            else if (numrows <= 1)
            {
                s32 xscroll = normalize_xscroll(src, (numrows == 0) ? 0 : rowscroll[0]);
                rectangle subclip = cliprect;

                // determine width of each column
                int colwidth = (int)(src.width() / numcols);
                global_object.assert(src.width() % colwidth == 0);

                // iterate over each column
                int groupcols;
                for (int col = 0; col < numcols; col += groupcols)
                {
                    s32 yscroll = colscroll[col];

                    // count consecutive columns scrolled by the same amount
                    for (groupcols = 1; col + groupcols < numcols; groupcols++)
                        if (colscroll[col + groupcols] != yscroll)
                            break;

                    // iterate over reps of the columns in question
                    yscroll = normalize_yscroll(src, yscroll);
                    for (s32 sx = xscroll - src.width(); sx < dest.width(); sx += src.width())
                    {
                        // compute the cliprect for this group
                        subclip.setx(col * colwidth + sx, (col + groupcols) * colwidth - 1 + sx);
                        subclip.intersection(cliprect);  //subclip &= cliprect;

                        // iterate over all portions of the scroll that overlap the destination
                        for (s32 sy = yscroll - src.height(); sy < dest.height(); sy += src.height())
                            copybitmap_trans(dest, src, 0, 0, sx, sy, subclip, trans_pen);
                    }
                }
            }

            // scrolling rows plus vertical scroll
            else if (numcols <= 1)
            {
                s32 yscroll = normalize_yscroll(src, (numcols == 0) ? 0 : colscroll[0]);
                rectangle subclip = cliprect;

                // determine width of each rows
                int rowheight = (int)(src.height() / numrows);
                global_object.assert(src.height() % rowheight == 0);

                // iterate over each row
                int grouprows;
                for (int row = 0; row < numrows; row += grouprows)
                {
                    s32 xscroll = rowscroll[row];

                    // count consecutive rows scrolled by the same amount
                    for (grouprows = 1; row + grouprows < numrows; grouprows++)
                        if (rowscroll[row + grouprows] != xscroll)
                            break;

                    // iterate over reps of the rows in question
                    xscroll = normalize_xscroll(src, xscroll);
                    for (s32 sy = yscroll - src.height(); sy < dest.height(); sy += src.height())
                    {
                        // compute the cliprect for this group
                        subclip.sety(row * rowheight + sy, (row + grouprows) * rowheight - 1 + sy);
                        subclip.intersection(cliprect);  //subclip &= cliprect;

                        // iterate over all portions of the scroll that overlap the destination
                        for (s32 sx = xscroll - src.width(); sx < dest.width(); sx += src.width())
                            copybitmap_trans(dest, src, 0, 0, sx, sy, subclip, trans_pen);
                    }
                }
            }
        }


        // copy from one bitmap to another, copying all unclipped pixels except those that match transpen, and applying scrolling to one or more rows/columns

        public static void copyscrollbitmap_trans(bitmap_ind16 dest, bitmap_ind16 src, u32 numrows, s32 [] rowscroll, u32 numcols, s32 [] colscroll, rectangle cliprect, u32 trans_pen)  //void copyscrollbitmap_trans(bitmap_ind16 &dest, const bitmap_ind16 &src, u32 numrows, const s32 *rowscroll, u32 numcols, const s32 *colscroll, const rectangle &cliprect, u32 trans_pen);
        { copyscrollbitmap_trans_common(dest, src, numrows, rowscroll, numcols, colscroll, cliprect, trans_pen); }

        //void copyscrollbitmap_trans(bitmap_rgb32 &dest, const bitmap_rgb32 &src, u32 numrows, const s32 *rowscroll, u32 numcols, const s32 *colscroll, const rectangle &cliprect, u32 trans_pen)
        //{ copyscrollbitmap_trans_common(dest, src, numrows, rowscroll, numcols, colscroll, cliprect, trans_pen); }


        //-------------------------------------------------
        //  alpha_blend_r16 - alpha blend two 16-bit
        //  5-5-5 RGB pixels
        //-------------------------------------------------
        public static u32 alpha_blend_r16(u32 d, u32 s, u8 level)
        {
            return (u32)(((((s & 0x001f) * level + (d & 0x001f) * (256 - level)) >> 8)) |
                    ((((s & 0x03e0) * level + (d & 0x03e0) * (256 - level)) >> 8) & 0x03e0) |
                    ((((s & 0x7c00) * level + (d & 0x7c00) * (256 - level)) >> 8) & 0x7c00));
        }

        //-------------------------------------------------
        //  alpha_blend_r32 - alpha blend two 32-bit
        //  8-8-8 RGB pixels
        //-------------------------------------------------
        public static u32 alpha_blend_r32(u32 d, u32 s, u8 level)
        {
            return (u32)(((((s & 0x0000ff) * level + (d & 0x0000ff) * (256 - level)) >> 8)) |
                    ((((s & 0x00ff00) * level + (d & 0x00ff00) * (256 - level)) >> 8) & 0x00ff00) |
                    ((((s & 0xff0000) * level + (d & 0xff0000) * (256 - level)) >> 8) & 0xff0000));
        }


        /*-------------------------------------------------
            readbit - read a single bit from a base
            offset
        -------------------------------------------------*/
        public static int readbit(Pointer<u8> src, UInt32 bitnum) { return src[bitnum / 8] & (0x80 >> (int)(bitnum % 8)); }  //static inline int readbit(const u8 *src, unsigned int bitnum)


        /*-------------------------------------------------
            normalize_xscroll - normalize an X scroll
            value for a bitmap to be positive and less
            than the width
        -------------------------------------------------*/
        static s32 normalize_xscroll(bitmap_t bitmap, s32 xscroll) { return xscroll >= 0 ? xscroll % bitmap.width() : (bitmap.width() - (-xscroll) % bitmap.width()); }


        /*-------------------------------------------------
            normalize_yscroll - normalize a Y scroll
            value for a bitmap to be positive and less
            than the height
        -------------------------------------------------*/
        static s32 normalize_yscroll(bitmap_t bitmap, s32 yscroll) { return yscroll >= 0 ? yscroll % bitmap.height() : (bitmap.height() - (-yscroll) % bitmap.height()); }
    }
}
