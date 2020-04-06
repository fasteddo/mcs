// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;


namespace mame
{
    //template<typename _PixelType, int _SrcShiftR, int _SrcShiftG, int _SrcShiftB, int _DstShiftR, int _DstShiftG, int _DstShiftB, bool _NoDestRead = false, bool _BilinearFilter = false>
    public class software_renderer<_PixelType> : global_object
    {
        // template parameters
        public class TemplateParams
        {
            public int _bpp;  // bits per pixel
            public int _Bpp;  // bytes per pixel
            public int _SrcShiftR;
            public int _SrcShiftG;
            public int _SrcShiftB;
            public int _DstShiftR;
            public int _DstShiftG;
            public int _DstShiftB;
            public bool _NoDestRead;
            public bool _BilinearFilter;

            public TemplateParams(int bpp, int SrcShiftR, int SrcShiftG, int SrcShiftB, int DstShiftR, int DstShiftG, int DstShiftB, bool NoDestRead = false, bool BilinearFilter = false)
            { _bpp = bpp; _Bpp = _bpp / 8; _SrcShiftR = SrcShiftR; _SrcShiftG = SrcShiftG; _SrcShiftB = SrcShiftB; _DstShiftR = DstShiftR; _DstShiftG = DstShiftG; _DstShiftB = DstShiftB; _NoDestRead = NoDestRead; _BilinearFilter = BilinearFilter; }
        }


        // internal structs
        struct quad_setup_data
        {
            public s32 dudx;
            public s32 dvdx;
            public s32 dudy;
            public s32 dvdy;
            public s32 startu;
            public s32 startv;
            public s32 startx;
            public s32 starty;
            public s32 endx;
            public s32 endy;
        }


        // internal helpers
        static bool is_opaque(TemplateParams t, float alpha) { return alpha >= (t._NoDestRead ? 0.5f : 1.0f); }
        static bool is_transparent(TemplateParams t, float alpha) { return alpha < (t._NoDestRead ? 0.5f : 0.0001f); }
        static rgb_t apply_intensity(int intensity, rgb_t color) { return color.scale8((u8)intensity); }
        static float round_nearest(float f) { return floor(f + 0.5f); }


        // destination pixels are written based on the values of the template parameters
        //static inline _PixelType dest_assemble_rgb(u32 r, u32 g, u32 b) { return (r << _DstShiftR) | (g << _DstShiftG) | (b << _DstShiftB); }
        //static inline _PixelType dest_rgb_to_pixel(u32 r, u32 g, u32 b) { return dest_assemble_rgb(r >> _SrcShiftR, g >> _SrcShiftG, b >> _SrcShiftB); }
        static u8 dest_assemble_rgb8(TemplateParams t, u32 r, u32 g, u32 b) { return (u8)((r << t._DstShiftR) | (g << t._DstShiftG) | (b << t._DstShiftB)); }
        static u16 dest_assemble_rgb16(TemplateParams t, u32 r, u32 g, u32 b) { return (u16)((r << t._DstShiftR) | (g << t._DstShiftG) | (b << t._DstShiftB)); }
        static u32 dest_assemble_rgb32(TemplateParams t, u32 r, u32 g, u32 b) { return (r << t._DstShiftR) | (g << t._DstShiftG) | (b << t._DstShiftB); }
        static u8 dest_rgb_to_pixel8(TemplateParams t, u32 r, u32 g, u32 b) { return dest_assemble_rgb8(t, r >> t._SrcShiftR, g >> t._SrcShiftG, b >> t._SrcShiftB); }
        static u16 dest_rgb_to_pixel16(TemplateParams t, u32 r, u32 g, u32 b) { return dest_assemble_rgb16(t, r >> t._SrcShiftR, g >> t._SrcShiftG, b >> t._SrcShiftB); }
        static u32 dest_rgb_to_pixel32(TemplateParams t, u32 r, u32 g, u32 b) { return dest_assemble_rgb32(t, r >> t._SrcShiftR, g >> t._SrcShiftG, b >> t._SrcShiftB); }


        // source 32-bit pixels are in MAME standardized format
        static u32 source32_r(TemplateParams t, u32 pixel) { return (u32)((pixel >> (16 + t._SrcShiftR)) & (0xff >> t._SrcShiftR)); }
        static u32 source32_g(TemplateParams t, u32 pixel) { return (u32)((pixel >> ( 8 + t._SrcShiftG)) & (0xff >> t._SrcShiftG)); }
        static u32 source32_b(TemplateParams t, u32 pixel) { return (u32)((pixel >> ( 0 + t._SrcShiftB)) & (0xff >> t._SrcShiftB)); }


        // destination pixel masks are based on the template parameters as well
        //static inline u32 dest_r(_PixelType pixel) { return (pixel >> _DstShiftR) & (0xff >> _SrcShiftR); }
        static u32 dest_r8(TemplateParams t, u8 pixel) { return ((u32)pixel >> t._DstShiftR) & ((u32)0xff >> t._SrcShiftR); }
        static u32 dest_r16(TemplateParams t, u16 pixel) { return ((u32)pixel >> t._DstShiftR) & ((u32)0xff >> t._SrcShiftR); }
        static u32 dest_r32(TemplateParams t, u32 pixel) { return ((u32)pixel >> t._DstShiftR) & ((u32)0xff >> t._SrcShiftR); }
        //static inline u32 dest_g(_PixelType pixel) { return (pixel >> _DstShiftG) & (0xff >> _SrcShiftG); }
        static u32 dest_g8(TemplateParams t, u8 pixel) { return ((u32)pixel >> t._DstShiftG) & ((u32)0xff >> t._SrcShiftG); }
        static u32 dest_g16(TemplateParams t, u16 pixel) { return ((u32)pixel >> t._DstShiftG) & ((u32)0xff >> t._SrcShiftG); }
        static u32 dest_g32(TemplateParams t, u32 pixel) { return ((u32)pixel >> t._DstShiftG) & ((u32)0xff >> t._SrcShiftG); }
        //static inline u32 dest_b(_PixelType pixel) { return (pixel >> _DstShiftB) & (0xff >> _SrcShiftB); }
        static u32 dest_b8(TemplateParams t, u8 pixel) { return ((u32)pixel >> t._DstShiftB) & ((u32)0xff >> t._SrcShiftB); }
        static u32 dest_b16(TemplateParams t, u16 pixel) { return ((u32)pixel >> t._DstShiftB) & ((u32)0xff >> t._SrcShiftB); }
        static u32 dest_b32(TemplateParams t, u32 pixel) { return ((u32)pixel >> t._DstShiftB) & ((u32)0xff >> t._SrcShiftB); }


        // generic conversion with special optimization for destinations in the standard format
        //static inline _PixelType source32_to_dest(u32 pixel)
        //{
        //    if (_SrcShiftR == 0 && _SrcShiftG == 0 && _SrcShiftB == 0 && _DstShiftR == 16 && _DstShiftG == 8 && _DstShiftB == 0)
        //        return pixel;
        //    else
        //        return dest_assemble_rgb(source32_r(pixel), source32_g(pixel), source32_b(pixel));
        //}
        static u8 source32_to_dest8(TemplateParams t, u32 pixel)
        {
            if (t._SrcShiftR == 0 && t._SrcShiftG == 0 && t._SrcShiftB == 0 && t._DstShiftR == 16 && t._DstShiftG == 8 && t._DstShiftB == 0)
                return (u8)pixel;
            else
                return dest_assemble_rgb8(t, source32_r(t, pixel), source32_g(t, pixel), source32_b(t, pixel));
        }
        static u16 source32_to_dest16(TemplateParams t, u32 pixel)
        {
            if (t._SrcShiftR == 0 && t._SrcShiftG == 0 && t._SrcShiftB == 0 && t._DstShiftR == 16 && t._DstShiftG == 8 && t._DstShiftB == 0)
                return (u16)pixel;
            else
                return dest_assemble_rgb16(t, source32_r(t, pixel), source32_g(t, pixel), source32_b(t, pixel));
        }
        static u32 source32_to_dest32(TemplateParams t, u32 pixel)
        {
            if (t._SrcShiftR == 0 && t._SrcShiftG == 0 && t._SrcShiftB == 0 && t._DstShiftR == 16 && t._DstShiftG == 8 && t._DstShiftB == 0)
                return pixel;
            else
                return dest_assemble_rgb32(t, source32_r(t, pixel), source32_g(t, pixel), source32_b(t, pixel));
        }


        //-------------------------------------------------
        //  ycc_to_rgb - convert YCC to RGB; the YCC pixel
        //  contains Y in the LSB, Cb << 8, and Cr << 16
        //  This actually a YCbCr conversion,
        //  details my be found in chapter 6.4 ff of
        //  http://softwarecommunity.intel.com/isn/downloads/softwareproducts/pdfs/346495.pdf
        //  The document also contains the constants below as floats.
        //-------------------------------------------------
        static u32 clamp16_shift8(u32 x)
        {
            return (((s32)x < 0) ? 0 : (x > 65535 ? 255: x >> 8));
        }


        static u32 ycc_to_rgb(u32 ycc)
        {
            // original equations:
            //
            //  C = Y - 16
            //  D = Cb - 128
            //  E = Cr - 128
            //
            //  R = clip(( 298 * C           + 409 * E + 128) >> 8)
            //  G = clip(( 298 * C - 100 * D - 208 * E + 128) >> 8)
            //  B = clip(( 298 * C + 516 * D           + 128) >> 8)
            //
            //  R = clip(( 298 * (Y - 16)                    + 409 * (Cr - 128) + 128) >> 8)
            //  G = clip(( 298 * (Y - 16) - 100 * (Cb - 128) - 208 * (Cr - 128) + 128) >> 8)
            //  B = clip(( 298 * (Y - 16) + 516 * (Cb - 128)                    + 128) >> 8)
            //
            //  R = clip(( 298 * Y - 298 * 16                        + 409 * Cr - 409 * 128 + 128) >> 8)
            //  G = clip(( 298 * Y - 298 * 16 - 100 * Cb + 100 * 128 - 208 * Cr + 208 * 128 + 128) >> 8)
            //  B = clip(( 298 * Y - 298 * 16 + 516 * Cb - 516 * 128                        + 128) >> 8)
            //
            //  R = clip(( 298 * Y - 298 * 16                        + 409 * Cr - 409 * 128 + 128) >> 8)
            //  G = clip(( 298 * Y - 298 * 16 - 100 * Cb + 100 * 128 - 208 * Cr + 208 * 128 + 128) >> 8)
            //  B = clip(( 298 * Y - 298 * 16 + 516 * Cb - 516 * 128                        + 128) >> 8)
            //
            //  Now combine constants:
            //
            //  R = clip(( 298 * Y            + 409 * Cr - 56992) >> 8)
            //  G = clip(( 298 * Y - 100 * Cb - 208 * Cr + 34784) >> 8)
            //  B = clip(( 298 * Y + 516 * Cb            - 70688) >> 8)
            //
            //  Define common = 298 * y - 56992. This will save one addition
            //
            //  R = clip(( common            + 409 * Cr -     0) >> 8)
            //  G = clip(( common - 100 * Cb - 208 * Cr + 91776) >> 8)
            //  B = clip(( common + 516 * Cb            - 13696) >> 8)
            //

            u8 y = (u8)ycc;
            u8 cb = (u8)(ycc >> 8);
            u8 cr = (u8)(ycc >> 16);

            u32 common = 298U * y - 56992;
            u32 r = (common +            409U * cr);
            u32 g = (common - 100U * cb - 208U * cr + 91776);
            u32 b = (common + 516U * cb - 13696);

            // Now clamp and shift back
            return new rgb_t((u8)clamp16_shift8(r), (u8)clamp16_shift8(g), (u8)clamp16_shift8(b));
        }


        //-------------------------------------------------
        //  get_texel_palette16 - return a texel from a
        //  palettized 16bpp source
        //-------------------------------------------------
        static u32 get_texel_palette16(TemplateParams t, render_texinfo texture, s32 curu, s32 curv)
        {
            ListBase<rgb_t> palbase = texture.palette;  //const rgb_t *palbase = texture.palette();
            if (t._BilinearFilter)
            {
                s32 u0 = curu >> 16;
                s32 u1 = 1;
                if (u0 < 0)
                {
                    u0 = 0;
                    u1 = 0;
                }
                else if (u0 + 1 >= texture.width)
                {
                    u0 = (s32)texture.width - 1;
                    u1 = 0;
                }

                s32 v0 = curv >> 16;
                s32 v1 = (s32)texture.rowpixels;
                if (v0 < 0)
                {
                    v0 = 0;
                    v1 = 0;
                }
                else if (v0 + 1 >= texture.height)
                {
                    v0 = (s32)texture.height - 1;
                    v1 = 0;
                }

                UInt16BufferPointer texbase = new UInt16BufferPointer(texture.base_);  //const u16 *texbase = reinterpret_cast<const u16 *>(texture.base);
                texbase += v0 * (s32)texture.rowpixels + u0;

                u32 pix00 = palbase[texbase[0]];
                u32 pix01 = palbase[texbase[u1]];
                u32 pix10 = palbase[texbase[v1]];
                u32 pix11 = palbase[texbase[u1 + v1]];
                return rgbaint_t.bilinear_filter(pix00, pix01, pix10, pix11, (u8)(curu >> 8), (u8)(curv >> 8));
            }
            else
            {
                UInt16BufferPointer texbase = new UInt16BufferPointer(texture.base_) + ((curv >> 16) * (s32)texture.rowpixels + (curu >> 16));  //const u16 *texbase = reinterpret_cast<const u16 *>(texture.base) + (curv >> 16) * texture.rowpixels + (curu >> 16);
                return palbase[texbase[0]];
            }
        }

#if false
        //-------------------------------------------------
        //  get_texel_palette16a - return a texel from a
        //  palettized 16bpp source with alpha
        //-------------------------------------------------
        static inline UINT32 get_texel_palette16a(const render_texinfo &texture, INT32 curu, INT32 curv)
#endif
#if false
        //-------------------------------------------------
        //  get_texel_yuy16 - return a texel from a 16bpp
        //  YCbCr source (pixel is returned as Cr-Cb-Y)
        //-------------------------------------------------
        static inline UINT32 get_texel_yuy16(const render_texinfo &texture, INT32 curu, INT32 curv)
#endif


        //-------------------------------------------------
        //  get_texel_rgb32 - return a texel from a 32bpp
        //  RGB source
        //-------------------------------------------------
        static u32 get_texel_rgb32(TemplateParams t, render_texinfo texture, s32 curu, s32 curv)
        {
            if (t._BilinearFilter)
            {
                s32 u0 = curu >> 16;
                s32 u1 = 1;
                if (u0 < 0)
                {
                    u0 = u1 = 0;
                }
                else if (u0 + 1 >= texture.width)
                {
                    u0 = (s32)texture.width - 1;
                    u1 = 0;
                }
                s32 v0 = curv >> 16;
                s32 v1 = (s32)texture.rowpixels;
                if (v0 < 0)
                {
                    v0 = v1 = 0;
                }
                else if (v0 + 1 >= texture.height)
                {
                    v0 = (s32)texture.height - 1;
                    v1 = 0;
                }

                UInt32BufferPointer texbase = new UInt32BufferPointer(texture.base_);  //const u32 *texbase = reinterpret_cast<const u32 *>(texture.base);
                texbase += v0 * (s32)texture.rowpixels + u0;

                return rgbaint_t.bilinear_filter(texbase[0], texbase[u1], texbase[v1], texbase[u1 + v1], (u8)(curu >> 8), (u8)(curv >> 8));
            }
            else
            {
                UInt32BufferPointer texbase = new UInt32BufferPointer(texture.base_) + ((curv >> 16) * (s32)texture.rowpixels + (curu >> 16));  //const u32 *texbase = reinterpret_cast<const u32 *>(texture.base) + (curv >> 16) * texture.rowpixels + (curu >> 16);
                return texbase[0];
            }
        }


        //-------------------------------------------------
        //  get_texel_argb32 - return a texel from a 32bpp
        //  ARGB source
        //-------------------------------------------------
        static u32 get_texel_argb32(TemplateParams t, render_texinfo texture, s32 curu, s32 curv)
        {
            if (t._BilinearFilter)
            {
                s32 u0 = curu >> 16;
                s32 u1 = 1;
                if (u0 < 0)
                {
                    u0 = u1 = 0;
                }
                else if (u0 + 1 >= texture.width)
                {
                    u0 = (s32)texture.width - 1;
                    u1 = 0;
                }
                s32 v0 = curv >> 16;
                s32 v1 = (s32)texture.rowpixels;
                if (v0 < 0)
                {
                    v0 = v1 = 0;
                }
                else if (v0 + 1 >= texture.height)
                {
                    v0 = (s32)texture.height - 1;
                    v1 = 0;
                }

                UInt32BufferPointer texbase = new UInt32BufferPointer(texture.base_);  //const u32 *texbase = reinterpret_cast<const u32 *>(texture.base);
                texbase += v0 * (s32)texture.rowpixels + u0;

                return rgbaint_t.bilinear_filter(texbase[0], texbase[u1], texbase[v1], texbase[u1 + v1], (u8)(curu >> 8), (u8)(curv >> 8));
            }
            else
            {
                UInt32BufferPointer texbase = new UInt32BufferPointer(texture.base_) + ((curv >> 16) * (s32)texture.rowpixels + (curu >> 16));  //const u32 *texbase = reinterpret_cast<const u32 *>(texture.base) + (curv >> 16) * texture.rowpixels + (curu >> 16);
                return texbase[0];
            }
        }


        //-------------------------------------------------
        //  draw_aa_pixel - draw an antialiased pixel
        //-------------------------------------------------
        //template<typename _PixelType, int _SrcShiftR, int _SrcShiftG, int _SrcShiftB, int _DstShiftR, int _DstShiftG, int _DstShiftB, bool _NoDestRead = false, bool _BilinearFilter = false>
        static void draw_aa_pixel(TemplateParams t, RawBufferPointer dstdata, u32 pitch, int x, int y, u32 col)  // _PixelType *dstdata
        {
            //_PixelType *dest = dstdata + y * pitch + x;
            RawBufferPointer dest8 = null;
            UInt16BufferPointer dest16 = null;
            UInt32BufferPointer dest32 = null;
            switch (t._bpp)
            {
                case 8:  dest8 = new RawBufferPointer(dstdata, y * (int)pitch + x); break;
                case 16: dest16 = new UInt16BufferPointer(dstdata, y * (int)pitch + x); break;
                case 32: dest32 = new UInt32BufferPointer(dstdata, y * (int)pitch + x); break;
                default: throw new emu_fatalerror("draw_aa_pixel() - unknown bpp - {0}\n", t._bpp);
            }

            //u32 dpix = _NoDestRead ? 0 : *dest;
            u32 dpix;
            switch (t._bpp)
            {
                case 8:  dpix = t._NoDestRead ? 0U : dest8[0]; break;
                case 16: dpix = t._NoDestRead ? 0U : dest16[0]; break;
                case 32: dpix = t._NoDestRead ? 0U : dest32[0]; break;
                default: throw new emu_fatalerror("draw_aa_pixel() - unknown bpp - {0}\n", t._bpp);
            }

            u32 dr = source32_r(t, col) + dest_r32(t, dpix);
            u32 dg = source32_g(t, col) + dest_g32(t, dpix);
            u32 db = source32_b(t, col) + dest_b32(t, dpix);

            dr = (u32)((dr | -(dr >> (8 - t._SrcShiftR))) & (0xff >> t._SrcShiftR));
            dg = (u32)((dg | -(dg >> (8 - t._SrcShiftG))) & (0xff >> t._SrcShiftG));
            db = (u32)((db | -(db >> (8 - t._SrcShiftB))) & (0xff >> t._SrcShiftB));

            //*dest = dest_assemble_rgb(dr, dg, db);
            switch (t._bpp)
            {
                case 8:  dest8[0] = dest_assemble_rgb8(t, dr, dg, db); break;
                case 16: dest16[0] = dest_assemble_rgb16(t, dr, dg, db); break;
                case 32: dest32[0] = dest_assemble_rgb32(t, dr, dg, db); break;
                default: throw new emu_fatalerror("draw_aa_pixel() - unknown bpp - {0}\n", t._bpp);
            }
        }


        // internal tables
        static u32 [] s_cosine_table = new u32[2049];


        //-------------------------------------------------
        //  draw_line - draw a line or point
        //-------------------------------------------------
        static void draw_line(TemplateParams t, render_primitive prim, RawBufferPointer dstdata, s32 width, s32 height, u32 pitch)  // _PixelType *dstdata
        {
            // compute the start/end coordinates
            int x1 = (int)(prim.bounds.x0 * 65536.0f);
            int y1 = (int)(prim.bounds.y0 * 65536.0f);
            int x2 = (int)(prim.bounds.x1 * 65536.0f);
            int y2 = (int)(prim.bounds.y1 * 65536.0f);

            // handle color and intensity
            u32 col = new rgb_t((byte)(255.0f * prim.color.r * prim.color.a), (byte)(255.0f * prim.color.g * prim.color.a), (byte)(255.0f * prim.color.b * prim.color.a));

            if (render_global.PRIMFLAG_GET_ANTIALIAS(prim.flags))
            {
                // build up the cosine table if we haven't yet
                if (s_cosine_table[0] == 0)
                    for (int entry = 0; entry <= 2048; entry++)
                        s_cosine_table[entry] = (u32)((double)(1.0 / Math.Cos(Math.Atan((double)(entry) / 2048.0))) * 0x10000000 + 0.5);

                int beam = (int)(prim.width * 65536.0f);
                if (beam < 0x00010000)
                    beam = 0x00010000;

                // draw an anti-aliased line
                int dx = Math.Abs(x1 - x2);
                int dy = Math.Abs(y1 - y2);
                if (dx >= dy)
                {
                    int sx = ((x1 <= x2) ? 1 : -1);
                    int sy = (dy == 0) ? 0 : eminline_global.div_32x32_shift(y2 - y1, dx, 16);
                    if (sy < 0)
                        dy--;
                    x1 >>= 16;
                    int xx = x2 >> 16;
                    int bwidth = eminline_global.mul_32x32_hi(beam << 4, (int)s_cosine_table[Math.Abs(sy) >> 5]);
                    y1 -= bwidth >> 1; // start back half the diameter
                    for (;;)
                    {
                        if (x1 >= 0 && x1 < width)
                        {
                            dx = bwidth;    // init diameter of beam
                            dy = y1 >> 16;
                            if (dy >= 0 && dy < height)
                                draw_aa_pixel(t, dstdata, pitch, x1, dy, apply_intensity(0xff & (~y1 >> 8), new rgb_t(col)));
                            dy++;
                            dx -= 0x10000 - (0xffff & y1); // take off amount plotted
                            u8 a1 = (byte)((dx >> 8) & 0xff);   // calc remainder pixel
                            dx >>= 16;                   // adjust to pixel (solid) count
                            while (dx-- != 0)                 // plot rest of pixels
                            {
                                if (dy >= 0 && dy < height)
                                    draw_aa_pixel(t, dstdata, pitch, x1, dy, col);
                                dy++;
                            }
                            if (dy >= 0 && dy < height)
                                draw_aa_pixel(t, dstdata, pitch, x1, dy, apply_intensity(a1, new rgb_t(col)));
                        }
                        if (x1 == xx) break;
                        x1 += sx;
                        y1 += sy;
                    }
                }
                else
                {
                    int sy = ((y1 <= y2) ? 1: -1);
                    int sx = (dx == 0) ? 0 : eminline_global.div_32x32_shift(x2 - x1, dy, 16);
                    if (sx < 0)
                        dx--;
                    y1 >>= 16;
                    int yy = y2 >> 16;
                    int bwidth = eminline_global.mul_32x32_hi(beam << 4, (int)s_cosine_table[Math.Abs(sx) >> 5]);
                    x1 -= bwidth >> 1; // start back half the width
                    for (;;)
                    {
                        if (y1 >= 0 && y1 < height)
                        {
                            dy = bwidth;    // calc diameter of beam
                            dx = x1 >> 16;
                            if (dx >= 0 && dx < width)
                                draw_aa_pixel(t, dstdata, pitch, dx, y1, apply_intensity(0xff & (~x1 >> 8), new rgb_t(col)));
                            dx++;
                            dy -= 0x10000 - (0xffff & x1); // take off amount plotted
                            u8 a1 = (byte)((dy >> 8) & 0xff);   // remainder pixel
                            dy >>= 16;                   // adjust to pixel (solid) count
                            while (dy-- != 0)                 // plot rest of pixels
                            {
                                if (dx >= 0 && dx < width)
                                    draw_aa_pixel(t, dstdata, pitch, dx, y1, col);
                                dx++;
                            }
                            if (dx >= 0 && dx < width)
                                draw_aa_pixel(t, dstdata, pitch, dx, y1, apply_intensity(a1, new rgb_t(col)));
                        }
                        if (y1 == yy) break;
                        y1 += sy;
                        x1 += sx;
                    }
                }
            }
            else // use good old Bresenham for non-antialiasing 980317 BW
            {
                x1 = (x1 + 0x8000) >> 16;
                y1 = (y1 + 0x8000) >> 16;
                x2 = (x2 + 0x8000) >> 16;
                y2 = (y2 + 0x8000) >> 16;

                int dx = std.abs(x1 - x2);
                int dy = std.abs(y1 - y2);
                int sx = (x1 <= x2) ? 1 : -1;
                int sy = (y1 <= y2) ? 1 : -1;
                int cx = dx / 2;
                int cy = dy / 2;

                if (dx >= dy)
                {
                    for (;;)
                    {
                        if (x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
                            draw_aa_pixel(t, dstdata, pitch, x1, y1, col);
                        if (x1 == x2) break;
                        x1 += sx;
                        cx -= dy;
                        if (cx < 0)
                        {
                            y1 += sy;
                            cx += dx;
                        }
                    }
                }
                else
                {
                    for (;;)
                    {
                        if (x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
                            draw_aa_pixel(t, dstdata, pitch, x1, y1, col);
                        if (y1 == y2) break;
                        y1 += sy;
                        cy -= dx;
                        if (cy < 0)
                        {
                            x1 += sx;
                            cy += dy;
                        }
                    }
                }
            }
        }


        //**************************************************************************
        //  RECT RASTERIZERS
        //**************************************************************************
        //-------------------------------------------------
        //  draw_rect - draw a solid rectangle
        //-------------------------------------------------
        static void draw_rect(TemplateParams t, render_primitive prim, RawBufferPointer dstdata, s32 width, s32 height, u32 pitch)  // _PixelType *dstdata
        {
            render_bounds fpos = prim.bounds;

            assert(fpos.x0 <= fpos.x1);
            assert(fpos.y0 <= fpos.y1);

            // clamp to integers
            s32 startx = (s32)round_nearest(fpos.x0);
            s32 starty = (s32)round_nearest(fpos.y0);
            s32 endx = (s32)round_nearest(fpos.x1);
            s32 endy = (s32)round_nearest(fpos.y1);

            // ensure we fit
            if (startx < 0) startx = 0;
            if (startx >= width) startx = width;
            if (endx < 0) endx = 0;
            if (endx >= width) endx = width;
            if (starty < 0) starty = 0;
            if (starty >= height) starty = height;
            if (endy < 0) endy = 0;
            if (endy >= height) endy = height;

            // bail if nothing left
            if (fpos.x0 > fpos.x1 || fpos.y0 > fpos.y1)
                return;

            // only support alpha and "none" blendmodes
            assert(render_global.PRIMFLAG_GET_BLENDMODE(prim.flags) == render_global.BLENDMODE_NONE || render_global.PRIMFLAG_GET_BLENDMODE(prim.flags) == BLENDMODE_ALPHA);

            // fast case: no alpha
            if (render_global.PRIMFLAG_GET_BLENDMODE(prim.flags) == render_global.BLENDMODE_NONE || is_opaque(t, prim.color.a))
            {
                u32 r = (u32)(256.0f * prim.color.r);
                u32 g = (u32)(256.0f * prim.color.g);
                u32 b = (u32)(256.0f * prim.color.b);
                u32 pix;

                // clamp R,G,B to 0-256 range
                if (r > 0xff) { if ((int)r < 0) r = 0; else r = 0xff; }
                if (g > 0xff) { if ((int)g < 0) g = 0; else g = 0xff; }
                if (b > 0xff) { if ((int)b < 0) b = 0; else b = 0xff; }
                pix = dest_rgb_to_pixel32(t, r, g, b);

                // loop over rows
                for (s32 y = starty; y < endy; y++)
                {
                    //_PixelType *dest = dstdata + y * pitch + startx;
                    RawBufferPointer dest8 = null;
                    UInt16BufferPointer dest16 = null;
                    UInt32BufferPointer dest32 = null;
                    switch (t._bpp)
                    {
                        case 8:  dest8 = new RawBufferPointer(dstdata, y * (int)pitch + startx); break;
                        case 16: dest16 = new UInt16BufferPointer(dstdata, y * (int)pitch + startx); break;
                        case 32: dest32 = new UInt32BufferPointer(dstdata, y * (int)pitch + startx); break;
                        default: throw new emu_fatalerror("draw_rect() - unknown bpp - {0}\n", t._bpp);
                    }

                    // loop over cols
                    for (s32 x = startx; x < endx; x++)
                    {
                        //*dest++ = pix;
                        switch (t._bpp)
                        {
                            case 8:  dest8[0] = (u8)pix;  dest8++;  break;
                            case 16: dest16[0] = (u16)pix;  dest16++;  break;
                            case 32: dest32[0] = pix;  dest32++;  break;
                            default: throw new emu_fatalerror("draw_rect() - unknown bpp - {0}\n", t._bpp);
                        }
                    }
                }
            }

            // alpha and/or coloring case
            else if (!is_transparent(t, prim.color.a))
            {
                u32 rmask = dest_rgb_to_pixel32(t, 0xff, 0x00, 0x00);
                u32 gmask = dest_rgb_to_pixel32(t, 0x00, 0xff, 0x00);
                u32 bmask = dest_rgb_to_pixel32(t, 0x00, 0x00, 0xff);
                u32 r = (u32)(256.0f * prim.color.r * prim.color.a);
                u32 g = (u32)(256.0f * prim.color.g * prim.color.a);
                u32 b = (u32)(256.0f * prim.color.b * prim.color.a);
                u32 inva = (u32)(256.0f * (1.0f - prim.color.a));

                // clamp R,G,B and inverse A to 0-256 range
                if (r > 0xff) { if ((s32)(r) < 0) r = 0; else r = 0xff; }
                if (g > 0xff) { if ((s32)(g) < 0) g = 0; else g = 0xff; }
                if (b > 0xff) { if ((s32)(b) < 0) b = 0; else b = 0xff; }
                if (inva > 0x100) { if ((s32)(inva) < 0) inva = 0; else inva = 0x100; }

                // pre-shift the RGBA pieces
                r = dest_rgb_to_pixel32(t, r, 0, 0) << 8;
                g = dest_rgb_to_pixel32(t, 0, g, 0) << 8;
                b = dest_rgb_to_pixel32(t, 0, 0, b) << 8;

                // loop over rows
                for (s32 y = starty; y < endy; y++)
                {
                    //_PixelType *dest = dstdata + y * pitch + startx;
                    RawBufferPointer dest8 = null;
                    UInt16BufferPointer dest16 = null;
                    UInt32BufferPointer dest32 = null;
                    switch (t._bpp)
                    {
                        case 8:  dest8 = new RawBufferPointer(dstdata, y * (int)pitch + startx); break;
                        case 16: dest16 = new UInt16BufferPointer(dstdata, y * (int)pitch + startx); break;
                        case 32: dest32 = new UInt32BufferPointer(dstdata, y * (int)pitch + startx); break;
                        default: throw new emu_fatalerror("draw_rect() - unknown bpp - {0}\n", t._bpp);
                    }

                    // loop over cols
                    for (s32 x = startx; x < endx; x++)
                    {
                        //u32 dpix = _NoDestRead ? 0 : *dest;
                        u32 dpix;
                        switch (t._bpp)
                        {
                            case 8:  dpix = t._NoDestRead ? 0U : dest8[0]; break;
                            case 16: dpix = t._NoDestRead ? 0U : dest16[0]; break;
                            case 32: dpix = t._NoDestRead ? 0U : dest32[0]; break;
                            default: throw new emu_fatalerror("draw_rect() - unknown bpp - {0}\n", t._bpp);
                        }

                        u32 dr = (r + ((dpix & rmask) * inva)) & (rmask << 8);
                        u32 dg = (g + ((dpix & gmask) * inva)) & (gmask << 8);
                        u32 db = (b + ((dpix & bmask) * inva)) & (bmask << 8);

                        //*dest++ = (dr | dg | db) >> 8;
                        switch (t._bpp)
                        {
                            case 8:  dest8[0] = (u8)((dr | dg | db) >> 8);  dest8++;  break;
                            case 16: dest16[0] = (u16)((dr | dg | db) >> 8);  dest16++;  break;
                            case 32: dest32[0] = (dr | dg | db) >> 8;  dest32++;  break;
                            default: throw new emu_fatalerror("draw_rect() - unknown bpp - {0}\n", t._bpp);
                        }
                    }
                }
            }
        }


        //**************************************************************************
        //  16-BIT PALETTE RASTERIZERS
        //**************************************************************************

        //-------------------------------------------------
        //  draw_quad_palette16_none - perform
        //  rasterization of a 16bpp palettized texture
        //-------------------------------------------------
        static void draw_quad_palette16_none(TemplateParams t, render_primitive prim, RawBufferPointer dstdata, u32 pitch, quad_setup_data setup)  //_PixelType *dstdata
        {
            // ensure all parameters are valid
            assert(prim.texture.palette != null);

            // fast case: no coloring, no alpha
            if (prim.color.r >= 1.0f && prim.color.g >= 1.0f && prim.color.b >= 1.0f && is_opaque(t, prim.color.a))
            {
                // loop over rows
                for (s32 y = setup.starty; y < setup.endy; y++)
                {
                    //_PixelType *dest = dstdata + y * pitch + setup.startx;
                    RawBufferPointer dest8 = null;
                    UInt16BufferPointer dest16 = null;
                    UInt32BufferPointer dest32 = null;
                    switch (t._bpp)
                    {
                        case 8:  dest8 = new RawBufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 16: dest16 = new UInt16BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 32: dest32 = new UInt32BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        default: throw new emu_fatalerror("draw_quad_palette16_none() - unknown bpp - {0}\n", t._bpp);
                    }

                    s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                    s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                    // loop over cols
                    for (s32 x = setup.startx; x < setup.endx; x++)
                    {
                        u32 pix = get_texel_palette16(t, prim.texture, curu, curv);

                        //*dest++ = source32_to_dest(pix);
                        switch (t._bpp)
                        {
                            case 8:  dest8[0] = source32_to_dest8(t, pix);  dest8++;  break;
                            case 16: dest16[0] = source32_to_dest16(t, pix);  dest16++;  break;
                            case 32: dest32[0] = source32_to_dest32(t, pix);  dest32++;  break;
                            default: throw new emu_fatalerror("draw_quad_palette16_none() - unknown bpp - {0}\n", t._bpp);
                        }

                        curu += setup.dudx;
                        curv += setup.dvdx;
                    }
                }
            }
            // coloring-only case
            else if (is_opaque(t, prim.color.a))
            {
                u32 sr = (u32)(256.0f * prim.color.r);
                u32 sg = (u32)(256.0f * prim.color.g);
                u32 sb = (u32)(256.0f * prim.color.b);

                // clamp R,G,B to 0-256 range
                if (sr > 0x100) { if ((s32)sr < 0) sr = 0; else sr = 0x100; }
                if (sg > 0x100) { if ((s32)sg < 0) sg = 0; else sg = 0x100; }
                if (sb > 0x100) { if ((s32)sb < 0) sb = 0; else sb = 0x100; }

                // loop over rows
                for (s32 y = setup.starty; y < setup.endy; y++)
                {
                    //_PixelType *dest = dstdata + y * pitch + setup.startx;
                    RawBufferPointer dest8 = null;
                    UInt16BufferPointer dest16 = null;
                    UInt32BufferPointer dest32 = null;
                    switch (t._bpp)
                    {
                        case 8:  dest8 = new RawBufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 16: dest16 = new UInt16BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 32: dest32 = new UInt32BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        default: throw new emu_fatalerror("draw_quad_palette16_none() - unknown bpp - {0}\n", t._bpp);
                    }

                    s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                    s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                    // loop over cols
                    for (s32 x = setup.startx; x < setup.endx; x++)
                    {
                        u32 pix = get_texel_palette16(t, prim.texture, curu, curv);
                        u32 r = (source32_r(t, pix) * sr) >> 8;
                        u32 g = (source32_g(t, pix) * sg) >> 8;
                        u32 b = (source32_b(t, pix) * sb) >> 8;

                        //*dest++ = dest_assemble_rgb(r, g, b);
                        switch (t._bpp)
                        {
                            case 8:  dest8[0] = dest_assemble_rgb8(t, r, g, b);  dest8++;  break;
                            case 16: dest16[0] = dest_assemble_rgb16(t, r, g, b);  dest16++;  break;
                            case 32: dest32[0] = dest_assemble_rgb32(t, r, g, b);  dest32++;  break;
                            default: throw new emu_fatalerror("draw_quad_palette16_none() - unknown bpp - {0}\n", t._bpp);
                        }

                        curu += setup.dudx;
                        curv += setup.dvdx;
                    }
                }
            }
            // alpha and/or coloring case
            else if (!is_transparent(t, prim.color.a))
            {
                u32 sr = (u32)(256.0f * prim.color.r * prim.color.a);
                u32 sg = (u32)(256.0f * prim.color.g * prim.color.a);
                u32 sb = (u32)(256.0f * prim.color.b * prim.color.a);
                u32 invsa = (u32)(256.0f * (1.0f - prim.color.a));

                // clamp R,G,B and inverse A to 0-256 range
                if (sr > 0x100) { if ((s32)sr < 0) sr = 0; else sr = 0x100; }
                if (sg > 0x100) { if ((s32)sg < 0) sg = 0; else sg = 0x100; }
                if (sb > 0x100) { if ((s32)sb < 0) sb = 0; else sb = 0x100; }
                if (invsa > 0x100) { if ((s32)invsa < 0) invsa = 0; else invsa = 0x100; }

                // loop over rows
                for (s32 y = setup.starty; y < setup.endy; y++)
                {
                    //_PixelType *dest = dstdata + y * pitch + setup.startx;
                    RawBufferPointer dest8 = null;
                    UInt16BufferPointer dest16 = null;
                    UInt32BufferPointer dest32 = null;
                    switch (t._bpp)
                    {
                        case 8:  dest8 = new RawBufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 16: dest16 = new UInt16BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 32: dest32 = new UInt32BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        default: throw new emu_fatalerror("draw_quad_palette16_none() - unknown bpp - {0}\n", t._bpp);
                    }

                    s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                    s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                    // loop over cols
                    for (s32 x = setup.startx; x < setup.endx; x++)
                    {
                        u32 pix = get_texel_palette16(t, prim.texture, curu, curv);

                        //u32 dpix = _NoDestRead ? 0 : *dest;
                        u32 dpix;
                        switch (t._bpp)
                        {
                            case 8:  dpix = t._NoDestRead ? 0U : dest8[0]; break;
                            case 16: dpix = t._NoDestRead ? 0U : dest16[0]; break;
                            case 32: dpix = t._NoDestRead ? 0U : dest32[0]; break;
                            default: throw new emu_fatalerror("draw_quad_palette16_none() - unknown bpp - {0}\n", t._bpp);
                        }

                        u32 r = (source32_r(t, pix) * sr + dest_r32(t, dpix) * invsa) >> 8;
                        u32 g = (source32_g(t, pix) * sg + dest_g32(t, dpix) * invsa) >> 8;
                        u32 b = (source32_b(t, pix) * sb + dest_b32(t, dpix) * invsa) >> 8;

                        //*dest++ = dest_assemble_rgb(r, g, b);
                        switch (t._bpp)
                        {
                            case 8:  dest8[0] = dest_assemble_rgb8(t, r, g, b);  dest8++;  break;
                            case 16: dest16[0] = dest_assemble_rgb16(t, r, g, b);  dest16++;  break;
                            case 32: dest32[0] = dest_assemble_rgb32(t, r, g, b);  dest32++;  break;
                            default: throw new emu_fatalerror("draw_quad_palette16_none() - unknown bpp - {0}\n", t._bpp);
                        }

                        curu += setup.dudx;
                        curv += setup.dvdx;
                    }
                }
            }
        }

        //-------------------------------------------------
        //  draw_quad_palette16_add - perform
        //  rasterization of a 16bpp palettized texture
        //-------------------------------------------------
        static void draw_quad_palette16_add(render_primitive prim, ListBytesPointer dstdata, u32 pitch, quad_setup_data setup)  // _PixelType *dstdata,
        {
            // ensure all parameters are valid
            assert(prim.texture.palette != null);

            throw new emu_unimplemented();
        }


        //**************************************************************************
        //  16-BIT YUY RASTERIZERS
        //**************************************************************************

        //-------------------------------------------------
        //  draw_quad_yuy16_none - perform
        //  rasterization of a 16bpp YUY image
        //-------------------------------------------------
        static void draw_quad_yuy16_none(render_primitive prim, ListBytesPointer dstdata, u32 pitch, quad_setup_data setup)  // _PixelType *dstdata
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  draw_quad_yuy16_add - perform
        //  rasterization by using RGB add after YUY
        //  conversion
        //-------------------------------------------------
        static void draw_quad_yuy16_add(render_primitive prim, ListBytesPointer dstdata, u32 pitch, quad_setup_data setup)  // _PixelType *dstdata
        {
            throw new emu_unimplemented();
        }


        //**************************************************************************
        //  32-BIT RGB QUAD RASTERIZERS
        //**************************************************************************

        //-------------------------------------------------
        //  draw_quad_rgb32 - perform rasterization of
        //  a 32bpp RGB texture
        //-------------------------------------------------
        static void draw_quad_rgb32(TemplateParams t, render_primitive prim, RawBufferPointer dstdata, u32 pitch, quad_setup_data setup)  // _PixelType *dstdata
        {
            ListBase<rgb_t> palbase = prim.texture.palette;  //const rgb_t *palbase = prim.texture.palette;

            // fast case: no coloring, no alpha
            if (prim.color.r >= 1.0f && prim.color.g >= 1.0f && prim.color.b >= 1.0f && is_opaque(t, prim.color.a))
            {
                // loop over rows
                for (s32 y = setup.starty; y < setup.endy; y++)
                {
                    //_PixelType *dest = dstdata + y * pitch + setup.startx;
                    RawBufferPointer dest8 = null;
                    UInt16BufferPointer dest16 = null;
                    UInt32BufferPointer dest32 = null;
                    switch (t._bpp)
                    {
                        case 8:  dest8 = new RawBufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 16: dest16 = new UInt16BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 32: dest32 = new UInt32BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        default: throw new emu_fatalerror("draw_quad_rgb32() - unknown bpp - {0}\n", t._bpp);
                    }

                    s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                    s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                    // no lookup case
                    if (palbase == null)
                    {
                        // loop over cols
                        for (int x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_rgb32(t, prim.texture, curu, curv);

                            //*dest++ = source32_to_dest(pix);
                            switch (t._bpp)
                            {
                                case 8:  dest8[0] = source32_to_dest8(t, pix);  dest8++;  break;
                                case 16: dest16[0] = source32_to_dest16(t, pix);  dest16++;  break;
                                case 32: dest32[0] = source32_to_dest32(t, pix);  dest32++;  break;
                                default: throw new emu_fatalerror("draw_quad_rgb32() - unknown bpp - {0}\n", t._bpp);
                            }

                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }

                    // lookup case
                    else
                    {
                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_rgb32(t, prim.texture, curu, curv);
                            u32 r = palbase[(int)((pix >> 16) & 0xff)] >> t._SrcShiftR;
                            u32 g = palbase[(int)((pix >> 8) & 0xff)] >> t._SrcShiftG;
                            u32 b = palbase[(int)((pix >> 0) & 0xff)] >> t._SrcShiftB;

                            //*dest++ = dest_assemble_rgb(r, g, b);
                            switch (t._bpp)
                            {
                                case 8:  dest8[0] = dest_assemble_rgb8(t, r, g, b);  dest8++;  break;
                                case 16: dest16[0] = dest_assemble_rgb16(t, r, g, b);  dest16++;  break;
                                case 32: dest32[0] = dest_assemble_rgb32(t, r, g, b);  dest32++;  break;
                                default: throw new emu_fatalerror("draw_quad_rgb32() - unknown bpp - {0}\n", t._bpp);
                            }

                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }
                }
            }

            // coloring-only case
            else if (is_opaque(t, prim.color.a))
            {
                u32 sr = (u32)(256.0f * prim.color.r);
                u32 sg = (u32)(256.0f * prim.color.g);
                u32 sb = (u32)(256.0f * prim.color.b);

                // clamp R,G,B to 0-256 range
                if (sr > 0x100) { if ((s32)sr < 0) sr = 0; else sr = 0x100; }
                if (sg > 0x100) { if ((s32)sg < 0) sg = 0; else sg = 0x100; }
                if (sb > 0x100) { if ((s32)sb < 0) sb = 0; else sb = 0x100; }

                // loop over rows
                for (s32 y = setup.starty; y < setup.endy; y++)
                {
                    //_PixelType *dest = dstdata + y * pitch + setup.startx;
                    RawBufferPointer dest8 = null;
                    UInt16BufferPointer dest16 = null;
                    UInt32BufferPointer dest32 = null;
                    switch (t._bpp)
                    {
                        case 8:  dest8 = new RawBufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 16: dest16 = new UInt16BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 32: dest32 = new UInt32BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        default: throw new emu_fatalerror("draw_quad_rgb32() - unknown bpp - {0}\n", t._bpp);
                    }

                    s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                    s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                    // no lookup case
                    if (palbase == null)
                    {
                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_rgb32(t, prim.texture, curu, curv);
                            u32 r = (source32_r(t, pix) * sr) >> 8;
                            u32 g = (source32_g(t, pix) * sg) >> 8;
                            u32 b = (source32_b(t, pix) * sb) >> 8;

                            //*dest++ = dest_assemble_rgb(r, g, b);
                            switch (t._bpp)
                            {
                                case 8:  dest8[0] = dest_assemble_rgb8(t, r, g, b);  dest8++;  break;
                                case 16: dest16[0] = dest_assemble_rgb16(t, r, g, b);  dest16++;  break;
                                case 32: dest32[0] = dest_assemble_rgb32(t, r, g, b);  dest32++;  break;
                                default: throw new emu_fatalerror("draw_quad_rgb32() - unknown bpp - {0}\n", t._bpp);
                            }

                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }

                    // lookup case
                    else
                    {
                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_rgb32(t, prim.texture, curu, curv);
                            u32 r = (palbase[(int)((pix >> 16) & 0xff)] * sr) >> (8 + t._SrcShiftR);
                            u32 g = (palbase[(int)((pix >> 8) & 0xff)] * sg) >> (8 + t._SrcShiftG);
                            u32 b = (palbase[(int)((pix >> 0) & 0xff)] * sb) >> (8 + t._SrcShiftB);

                            //*dest++ = dest_assemble_rgb(r, g, b);
                            switch (t._bpp)
                            {
                                case 8:  dest8[0] = dest_assemble_rgb8(t, r, g, b);  dest8++;  break;
                                case 16: dest16[0] = dest_assemble_rgb16(t, r, g, b);  dest16++;  break;
                                case 32: dest32[0] = dest_assemble_rgb32(t, r, g, b);  dest32++;  break;
                                default: throw new emu_fatalerror("draw_quad_rgb32() - unknown bpp - {0}\n", t._bpp);
                            }

                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }
                }
            }

            // alpha and/or coloring case
            else if (!is_transparent(t, prim.color.a))
            {
                u32 sr = (u32)(256.0f * prim.color.r * prim.color.a);
                u32 sg = (u32)(256.0f * prim.color.g * prim.color.a);
                u32 sb = (u32)(256.0f * prim.color.b * prim.color.a);
                u32 invsa = (u32)(256.0f * (1.0f - prim.color.a));

                // clamp R,G,B and inverse A to 0-256 range
                if (sr > 0x100) { if ((s32)sr < 0) sr = 0; else sr = 0x100; }
                if (sg > 0x100) { if ((s32)sg < 0) sg = 0; else sg = 0x100; }
                if (sb > 0x100) { if ((s32)sb < 0) sb = 0; else sb = 0x100; }
                if (invsa > 0x100) { if ((s32)invsa < 0) invsa = 0; else invsa = 0x100; }

                // loop over rows
                for (s32 y = setup.starty; y < setup.endy; y++)
                {
                    //_PixelType *dest = dstdata + y * pitch + setup.startx;
                    RawBufferPointer dest8 = null;
                    UInt16BufferPointer dest16 = null;
                    UInt32BufferPointer dest32 = null;
                    switch (t._bpp)
                    {
                        case 8:  dest8 = new RawBufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 16: dest16 = new UInt16BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 32: dest32 = new UInt32BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        default: throw new emu_fatalerror("draw_quad_rgb32() - unknown bpp - {0}\n", t._bpp);
                    }

                    s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                    s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                    // no lookup case
                    if (palbase == null)
                    {
                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_rgb32(t, prim.texture, curu, curv);

                            //u32 dpix = _NoDestRead ? 0 : *dest;
                            u32 dpix;
                            switch (t._bpp)
                            {
                                case 8:  dpix = t._NoDestRead ? 0U : dest8[0]; break;
                                case 16: dpix = t._NoDestRead ? 0U : dest16[0]; break;
                                case 32: dpix = t._NoDestRead ? 0U : dest32[0]; break;
                                default: throw new emu_fatalerror("draw_quad_rgb32() - unknown bpp - {0}\n", t._bpp);
                            }

                            u32 r = (source32_r(t, pix) * sr + dest_r32(t, dpix) * invsa) >> 8;
                            u32 g = (source32_g(t, pix) * sg + dest_g32(t, dpix) * invsa) >> 8;
                            u32 b = (source32_b(t, pix) * sb + dest_b32(t, dpix) * invsa) >> 8;

                            //*dest++ = dest_assemble_rgb(r, g, b);
                            switch (t._bpp)
                            {
                                case 8:  dest8[0] = dest_assemble_rgb8(t, r, g, b);  dest8++;  break;
                                case 16: dest16[0] = dest_assemble_rgb16(t, r, g, b);  dest16++;  break;
                                case 32: dest32[0] = dest_assemble_rgb32(t, r, g, b);  dest32++;  break;
                                default: throw new emu_fatalerror("draw_quad_rgb32() - unknown bpp - {0}\n", t._bpp);
                            }

                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }

                    // lookup case
                    else
                    {
                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_rgb32(t, prim.texture, curu, curv);

                            //u32 dpix = _NoDestRead ? 0 : *dest;
                            u32 dpix;
                            switch (t._bpp)
                            {
                                case 8:  dpix = t._NoDestRead ? 0U : dest8[0]; break;
                                case 16: dpix = t._NoDestRead ? 0U : dest16[0]; break;
                                case 32: dpix = t._NoDestRead ? 0U : dest32[0]; break;
                                default: throw new emu_fatalerror("draw_quad_rgb32() - unknown bpp - {0}\n", t._bpp);
                            }

                            u32 r = ((palbase[(int)((pix >> 16) & 0xff)] >> t._SrcShiftR) * sr + dest_r32(t, dpix) * invsa) >> 8;
                            u32 g = ((palbase[(int)((pix >> 8) & 0xff)] >> t._SrcShiftG) * sg + dest_g32(t, dpix) * invsa) >> 8;
                            u32 b = ((palbase[(int)((pix >> 0) & 0xff)] >> t._SrcShiftB) * sb + dest_b32(t, dpix) * invsa) >> 8;

                            //*dest++ = dest_assemble_rgb(r, g, b);
                            switch (t._bpp)
                            {
                                case 8:  dest8[0] = dest_assemble_rgb8(t, r, g, b);  dest8++;  break;
                                case 16: dest16[0] = dest_assemble_rgb16(t, r, g, b);  dest16++;  break;
                                case 32: dest32[0] = dest_assemble_rgb32(t, r, g, b);  dest32++;  break;
                                default: throw new emu_fatalerror("draw_quad_rgb32() - unknown bpp - {0}\n", t._bpp);
                            }

                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }
                }
            }
        }


        //-------------------------------------------------
        //  draw_quad_rgb32_add - perform
        //  rasterization by using RGB add
        //-------------------------------------------------
        static void draw_quad_rgb32_add(render_primitive prim, ListBytesPointer dstdata, u32 pitch, quad_setup_data setup)  // _PixelType *dstdata
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  draw_quad_rgb32_multiply - perform
        //  rasterization using RGB multiply
        //-------------------------------------------------
        static void draw_quad_rgb32_multiply(render_primitive prim, ListBytesPointer dstdata, u32 pitch, quad_setup_data setup)  //_PixelType *dstdata
        {
            throw new emu_unimplemented();
        }


        //**************************************************************************
        //  32-BIT ARGB QUAD RASTERIZERS
        //**************************************************************************

        //-------------------------------------------------
        //  draw_quad_argb32_alpha - perform
        //  rasterization using standard alpha blending
        //-------------------------------------------------
        static void draw_quad_argb32_alpha(TemplateParams t, render_primitive prim, RawBufferPointer dstdata, u32 pitch, quad_setup_data setup)  // _PixelType *dstdata,
        {
            ListBase<rgb_t> palbase = prim.texture.palette;  //const rgb_t *palbase = prim.texture.palette;

            // fast case: no coloring, no alpha
            if (prim.color.r >= 1.0f && prim.color.g >= 1.0f && prim.color.b >= 1.0f && is_opaque(t, prim.color.a))
            {
                // loop over rows
                for (s32 y = setup.starty; y < setup.endy; y++)
                {
                    //_PixelType *dest = dstdata + y * pitch + setup.startx;
                    RawBufferPointer dest8 = null;
                    UInt16BufferPointer dest16 = null;
                    UInt32BufferPointer dest32 = null;
                    switch (t._bpp)
                    {
                        case 8:  dest8 = new RawBufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 16: dest16 = new UInt16BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 32: dest32 = new UInt32BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        default: throw new emu_fatalerror("draw_quad_argb32_alpha() - unknown bpp - {0}\n", t._bpp);
                    }

                    s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                    s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                    // no lookup case
                    if (palbase == null)
                    {
                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_argb32(t, prim.texture, curu, curv);
                            u32 ta = pix >> 24;
                            if (ta != 0)
                            {
                                //u32 dpix = _NoDestRead ? 0 : *dest;
                                u32 dpix;
                                switch (t._bpp)
                                {
                                    case 8:  dpix = t._NoDestRead ? 0U : dest8[0]; break;
                                    case 16: dpix = t._NoDestRead ? 0U : dest16[0]; break;
                                    case 32: dpix = t._NoDestRead ? 0U : dest32[0]; break;
                                    default: throw new emu_fatalerror("draw_quad_argb32_alpha() - unknown bpp - {0}\n", t._bpp);
                                }

                                u32 invta = 0x100 - ta;
                                u32 r = (source32_r(t, pix) * ta + dest_r32(t, dpix) * invta) >> 8;
                                u32 g = (source32_g(t, pix) * ta + dest_g32(t, dpix) * invta) >> 8;
                                u32 b = (source32_b(t, pix) * ta + dest_b32(t, dpix) * invta) >> 8;

                                //*dest = dest_assemble_rgb(r, g, b);
                                switch (t._bpp)
                                {
                                    case 8:  dest8[0] = dest_assemble_rgb8(t, r, g, b);  break;
                                    case 16: dest16[0] = dest_assemble_rgb16(t, r, g, b);  break;
                                    case 32: dest32[0] = dest_assemble_rgb32(t, r, g, b);  break;
                                    default: throw new emu_fatalerror("draw_quad_argb32_alpha() - unknown bpp - {0}\n", t._bpp);
                                }
                            }

                            //dest++;
                            switch (t._bpp)
                            {
                                case 8:  dest8++;  break;
                                case 16: dest16++;  break;
                                case 32: dest32++;  break;
                                default: throw new emu_fatalerror("draw_quad_argb32_alpha() - unknown bpp - {0}\n", t._bpp);
                            }

                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }

                    // lookup case
                    else
                    {
                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_argb32(t, prim.texture, curu, curv);
                            u32 ta = pix >> 24;
                            if (ta != 0)
                            {
                                //u32 dpix = _NoDestRead ? 0 : *dest;
                                u32 dpix;
                                switch (t._bpp)
                                {
                                    case 8:  dpix = t._NoDestRead ? 0U : dest8[0]; break;
                                    case 16: dpix = t._NoDestRead ? 0U : dest16[0]; break;
                                    case 32: dpix = t._NoDestRead ? 0U : dest32[0]; break;
                                    default: throw new emu_fatalerror("draw_quad_argb32_alpha() - unknown bpp - {0}\n", t._bpp);
                                }

                                u32 invta = 0x100 - ta;
                                u32 r = ((palbase[(int)((pix >> 16) & 0xff)] >> t._SrcShiftR) * ta + dest_r32(t, dpix) * invta) >> 8;
                                u32 g = ((palbase[(int)((pix >> 8) & 0xff)] >> t._SrcShiftG) * ta + dest_g32(t, dpix) * invta) >> 8;
                                u32 b = ((palbase[(int)((pix >> 0) & 0xff)] >> t._SrcShiftB) * ta + dest_b32(t, dpix) * invta) >> 8;

                                //*dest = dest_assemble_rgb(r, g, b);
                                switch (t._bpp)
                                {
                                    case 8:  dest8[0] = dest_assemble_rgb8(t, r, g, b);  break;
                                    case 16: dest16[0] = dest_assemble_rgb16(t, r, g, b);  break;
                                    case 32: dest32[0] = dest_assemble_rgb32(t, r, g, b);  break;
                                    default: throw new emu_fatalerror("draw_quad_argb32_alpha() - unknown bpp - {0}\n", t._bpp);
                                }
                            }

                            //dest++;
                            switch (t._bpp)
                            {
                                case 8:  dest8++;  break;
                                case 16: dest16++;  break;
                                case 32: dest32++;  break;
                                default: throw new emu_fatalerror("draw_quad_argb32_alpha() - unknown bpp - {0}\n", t._bpp);
                            }

                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }
                }
            }

            // alpha and/or coloring case
            else
            {
                u32 sr = (u32)(256.0f * prim.color.r);
                u32 sg = (u32)(256.0f * prim.color.g);
                u32 sb = (u32)(256.0f * prim.color.b);
                u32 sa = (u32)(256.0f * prim.color.a);

                // clamp R,G,B and inverse A to 0-256 range
                if (sr > 0x100) { if ((s32)(sr) < 0) sr = 0; else sr = 0x100; }
                if (sg > 0x100) { if ((s32)(sg) < 0) sg = 0; else sg = 0x100; }
                if (sb > 0x100) { if ((s32)(sb) < 0) sb = 0; else sb = 0x100; }
                if (sa > 0x100) { if ((s32)(sa) < 0) sa = 0; else sa = 0x100; }

                // loop over rows
                for (s32 y = setup.starty; y < setup.endy; y++)
                {
                    //_PixelType *dest = dstdata + y * pitch + setup.startx;
                    RawBufferPointer dest8 = null;
                    UInt16BufferPointer dest16 = null;
                    UInt32BufferPointer dest32 = null;
                    switch (t._bpp)
                    {
                        case 8:  dest8 = new RawBufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 16: dest16 = new UInt16BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        case 32: dest32 = new UInt32BufferPointer(dstdata, y * (int)pitch + setup.startx); break;
                        default: throw new emu_fatalerror("draw_quad_argb32_alpha() - unknown bpp - {0}\n", t._bpp);
                    }

                    s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                    s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                    // no lookup case
                    if (palbase == null)
                    {
                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_argb32(t, prim.texture, curu, curv);
                            u32 ta = (pix >> 24) * sa;
                            if (ta != 0)
                            {
                                //u32 dpix = _NoDestRead ? 0 : *dest;
                                u32 dpix;
                                switch (t._bpp)
                                {
                                    case 8:  dpix = t._NoDestRead ? 0U : dest8[0]; break;
                                    case 16: dpix = t._NoDestRead ? 0U : dest16[0]; break;
                                    case 32: dpix = t._NoDestRead ? 0U : dest32[0]; break;
                                    default: throw new emu_fatalerror("draw_quad_argb32_alpha() - unknown bpp - {0}\n", t._bpp);
                                }

                                u32 invsta = (0x10000 - ta) << 8;
                                u32 r = (source32_r(t, pix) * sr * ta + dest_r32(t, dpix) * invsta) >> 24;
                                u32 g = (source32_g(t, pix) * sg * ta + dest_g32(t, dpix) * invsta) >> 24;
                                u32 b = (source32_b(t, pix) * sb * ta + dest_b32(t, dpix) * invsta) >> 24;

                                //*dest = dest_assemble_rgb(r, g, b);
                                switch (t._bpp)
                                {
                                    case 8:  dest8[0] = dest_assemble_rgb8(t, r, g, b);  break;
                                    case 16: dest16[0] = dest_assemble_rgb16(t, r, g, b);  break;
                                    case 32: dest32[0] = dest_assemble_rgb32(t, r, g, b);  break;
                                    default: throw new emu_fatalerror("draw_quad_argb32_alpha() - unknown bpp - {0}\n", t._bpp);
                                }
                            }

                            //dest++;
                            switch (t._bpp)
                            {
                                case 8:  dest8++;  break;
                                case 16: dest16++;  break;
                                case 32: dest32++;  break;
                                default: throw new emu_fatalerror("draw_quad_argb32_alpha() - unknown bpp - {0}\n", t._bpp);
                            }

                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }

                    // lookup case
                    else
                    {
                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_argb32(t, prim.texture, curu, curv);
                            u32 ta = (pix >> 24) * sa;
                            if (ta != 0)
                            {
                                //u32 dpix = _NoDestRead ? 0 : *dest;
                                u32 dpix;
                                switch (t._bpp)
                                {
                                    case 8:  dpix = t._NoDestRead ? 0U : dest8[0]; break;
                                    case 16: dpix = t._NoDestRead ? 0U : dest16[0]; break;
                                    case 32: dpix = t._NoDestRead ? 0U : dest32[0]; break;
                                    default: throw new emu_fatalerror("draw_quad_argb32_alpha() - unknown bpp - {0}\n", t._bpp);
                                }

                                u32 invsta = (0x10000 - ta) << 8;
                                u32 r = ((palbase[(int)((pix >> 16) & 0xff)] >> t._SrcShiftR) * sr * ta + dest_r32(t, dpix) * invsta) >> 24;
                                u32 g = ((palbase[(int)((pix >> 8) & 0xff)] >> t._SrcShiftG) * sg * ta + dest_g32(t, dpix) * invsta) >> 24;
                                u32 b = ((palbase[(int)((pix >> 0) & 0xff)] >> t._SrcShiftB) * sb * ta + dest_b32(t, dpix) * invsta) >> 24;

                                //*dest = dest_assemble_rgb(r, g, b);
                                switch (t._bpp)
                                {
                                    case 8:  dest8[0] = dest_assemble_rgb8(t, r, g, b);  break;
                                    case 16: dest16[0] = dest_assemble_rgb16(t, r, g, b);  break;
                                    case 32: dest32[0] = dest_assemble_rgb32(t, r, g, b);  break;
                                    default: throw new emu_fatalerror("draw_quad_argb32_alpha() - unknown bpp - {0}\n", t._bpp);
                                }
                            }

                            //dest++;
                            switch (t._bpp)
                            {
                                case 8:  dest8++;  break;
                                case 16: dest16++;  break;
                                case 32: dest32++;  break;
                                default: throw new emu_fatalerror("draw_quad_argb32_alpha() - unknown bpp - {0}\n", t._bpp);
                            }

                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }
                }
            }
        }


        //-------------------------------------------------
        //  draw_quad_argb32_add - perform
        //  rasterization by using RGB add
        //-------------------------------------------------
        static void draw_quad_argb32_add(render_primitive prim, ListBytesPointer dstdata, u32 pitch, quad_setup_data setup)  // _PixelType *dstdata
        {
            throw new emu_unimplemented();
        }


        //**************************************************************************
        //  CORE QUAD RASTERIZERS
        //**************************************************************************

        //-------------------------------------------------
        //  setup_and_draw_textured_quad - perform setup
        //  and then dispatch to a texture-mode-specific
        //  drawing routine
        //-------------------------------------------------
        static void setup_and_draw_textured_quad(TemplateParams t, render_primitive prim, RawBufferPointer dstdata, s32 width, s32 height, u32 pitch)  //_PixelType *dstdata
        {
            assert(prim.bounds.x0 <= prim.bounds.x1);
            assert(prim.bounds.y0 <= prim.bounds.y1);

            // determine U/V deltas
            float fdudx = (prim.texcoords.tr.u - prim.texcoords.tl.u) / (prim.bounds.x1 - prim.bounds.x0);
            float fdvdx = (prim.texcoords.tr.v - prim.texcoords.tl.v) / (prim.bounds.x1 - prim.bounds.x0);
            float fdudy = (prim.texcoords.bl.u - prim.texcoords.tl.u) / (prim.bounds.y1 - prim.bounds.y0);
            float fdvdy = (prim.texcoords.bl.v - prim.texcoords.tl.v) / (prim.bounds.y1 - prim.bounds.y0);

            // clamp to integers
            quad_setup_data setup = new quad_setup_data();
            setup.startx = (s32)round_nearest(prim.bounds.x0);
            setup.starty = (s32)round_nearest(prim.bounds.y0);
            setup.endx = (s32)round_nearest(prim.bounds.x1);
            setup.endy = (s32)round_nearest(prim.bounds.y1);

            // ensure we fit
            if (setup.startx < 0) setup.startx = 0;
            if (setup.startx >= width) setup.startx = width;
            if (setup.endx < 0) setup.endx = 0;
            if (setup.endx >= width) setup.endx = width;
            if (setup.starty < 0) setup.starty = 0;
            if (setup.starty >= height) setup.starty = height;
            if (setup.endy < 0) setup.endy = 0;
            if (setup.endy >= height) setup.endy = height;

            // compute start and delta U,V coordinates now
            setup.dudx = (s32)(round_nearest(65536.0f * (float)(prim.texture.width) * fdudx));
            setup.dvdx = (s32)(round_nearest(65536.0f * (float)(prim.texture.height) * fdvdx));
            setup.dudy = (s32)(round_nearest(65536.0f * (float)(prim.texture.width) * fdudy));
            setup.dvdy = (s32)(round_nearest(65536.0f * (float)(prim.texture.height) * fdvdy));
            setup.startu = (s32)(round_nearest(65536.0f * (float)(prim.texture.width) * prim.texcoords.tl.u));
            setup.startv = (s32)(round_nearest(65536.0f * (float)(prim.texture.height) * prim.texcoords.tl.v));

            // advance U/V to the middle of the first texel
            setup.startu += (setup.dudx + setup.dudy) / 2;
            setup.startv += (setup.dvdx + setup.dvdy) / 2;

            // if we're bilinear filtering, we need to offset u/v by half a texel
            if (t._BilinearFilter)
            {
                setup.startu -= 0x8000;
                setup.startv -= 0x8000;
            }

            // render based on the texture coordinates
            u32 primflags = prim.flags & (render_global.PRIMFLAG_TEXFORMAT_MASK | render_global.PRIMFLAG_BLENDMODE_MASK);
            //switch (prim.flags & (render_global.PRIMFLAG_TEXFORMAT_MASK | render_global.PRIMFLAG_BLENDMODE_MASK))
            {
                if (primflags == (render_global.PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_PALETTE16) | PRIMFLAG_BLENDMODE(render_global.BLENDMODE_NONE)) ||
                    primflags == (render_global.PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_PALETTE16) | PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA)))
                    draw_quad_palette16_none(t, prim, dstdata, pitch, setup);

                else if (primflags == (render_global.PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_PALETTE16) | PRIMFLAG_BLENDMODE(render_global.BLENDMODE_ADD)))
                    draw_quad_palette16_add(prim, dstdata, pitch, setup);

                else if (primflags == (render_global.PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_YUY16) | PRIMFLAG_BLENDMODE(render_global.BLENDMODE_NONE)) ||
                         primflags == (render_global.PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_YUY16) | PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA)))
                    draw_quad_yuy16_none(prim, dstdata, pitch, setup);

                else if (primflags == (render_global.PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_YUY16) | PRIMFLAG_BLENDMODE(render_global.BLENDMODE_ADD)))
                    draw_quad_yuy16_add(prim, dstdata, pitch, setup);

                else if (primflags == (render_global.PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_RGB32) | PRIMFLAG_BLENDMODE(render_global.BLENDMODE_NONE)) ||
                         primflags == (render_global.PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_RGB32) | PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA)) ||
                         primflags == (render_global.PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_ARGB32) | PRIMFLAG_BLENDMODE(render_global.BLENDMODE_NONE)))
                    draw_quad_rgb32(t, prim, dstdata, pitch, setup);

                else if (primflags == (render_global.PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_RGB32) | PRIMFLAG_BLENDMODE(render_global.BLENDMODE_RGB_MULTIPLY)) ||
                         primflags == (render_global.PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_ARGB32) | PRIMFLAG_BLENDMODE(render_global.BLENDMODE_RGB_MULTIPLY)))
                    draw_quad_rgb32_multiply(prim, dstdata, pitch, setup);

                else if (primflags == (render_global.PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_RGB32) | PRIMFLAG_BLENDMODE(render_global.BLENDMODE_ADD)))
                    draw_quad_rgb32_add(prim, dstdata, pitch, setup);

                else if (primflags == (render_global.PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_ARGB32) | PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA)))
                    draw_quad_argb32_alpha(t, prim, dstdata, pitch, setup);

                else if (primflags == (render_global.PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_ARGB32) | PRIMFLAG_BLENDMODE(render_global.BLENDMODE_ADD)))
                    draw_quad_argb32_add(prim, dstdata, pitch, setup);

                else
                    fatalerror("Unknown texformat({0})/blendmode({1}) combo\n", render_global.PRIMFLAG_GET_TEXFORMAT(prim.flags), render_global.PRIMFLAG_GET_BLENDMODE(prim.flags));
            }
        }


        //**************************************************************************
        //  PRIMARY ENTRY POINT
        //**************************************************************************

        //-------------------------------------------------
        //  draw_primitives - draw a series of primitives
        //  using a software rasterizer
        //-------------------------------------------------
        //template<typename _PixelType, int _SrcShiftR, int _SrcShiftG, int _SrcShiftB, int _DstShiftR, int _DstShiftG, int _DstShiftB, bool _NoDestRead = false, bool _BilinearFilter = false>
        public static void draw_primitives(TemplateParams t, render_primitive_list primlist, RawBufferPointer dstdata, u32 width, u32 height, u32 pitch)  //static void draw_primitives(const render_primitive_list &primlist, void *dstdata, u32 width, u32 height, u32 pitch)
        {
            // loop over the list and render each element
            for (render_primitive prim = primlist.first(); prim != null; prim = prim.next())
            {
                switch (prim.type)
                {
                    case render_primitive.primitive_type.LINE:
                        draw_line(t, prim, dstdata, (int)width, (int)height, pitch);  //draw_line(*prim, reinterpret_cast<_PixelType *>(dstdata), width, height, pitch);
                        break;

                    case render_primitive.primitive_type.QUAD:
                        if (prim.texture.base_ == null)
                            draw_rect(t, prim, dstdata, (int)width, (int)height, pitch);  //draw_rect(prim, reinterpret_cast<_PixelType *>(dstdata), width, height, pitch);
                        else
                            setup_and_draw_textured_quad(t, prim, dstdata, (int)width, (int)height, pitch);  //setup_and_draw_textured_quad(*prim, reinterpret_cast<_PixelType *>(dstdata), width, height, pitch);
                        break;

                    default:
                        throw new emu_fatalerror("Unexpected render_primitive type\n");
                }
            }
        }
    }
}
