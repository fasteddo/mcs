// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;

using static mame.cpp_global;
using static mame.emucore_global;
using static mame.rendutil_global;
using static mame.rendutil_internal;


namespace mame
{
    /* ----- image formats ----- */
    //enum ru_imgformat


    public static class rendutil_global
    {
        /* ----- render utilities ----- */
        /*-------------------------------------------------
            render_resample_argb_bitmap_hq - perform a high
            quality resampling of a texture
        -------------------------------------------------*/
        public static void render_resample_argb_bitmap_hq(bitmap_argb32 dest, bitmap_argb32 source, render_color color, bool force = false)
        {
            if (dest.width() == 0 || dest.height() == 0)
                return;

            /* adjust the source base */
            PointerU32 sbase = source.pix(0);  //const u32 *sbase = &source.pix(0);

            /* determine the steppings */
            u32 swidth = (u32)source.width();
            u32 sheight = (u32)source.height();
            u32 dwidth = (u32)dest.width();
            u32 dheight = (u32)dest.height();
            u32 dx = (swidth << 12) / dwidth;
            u32 dy = (sheight << 12) / dheight;

            /* if the source is higher res than the target, use full averaging */
            if (dx > 0x1000 || dy > 0x1000 || force)
            {
                resample_argb_bitmap_average(dest.pix(0), (u32)dest.rowpixels(), dwidth, dheight, sbase, (u32)source.rowpixels(), swidth, sheight, color, dx, dy);
            }
            else
            {
                resample_argb_bitmap_bilinear(dest.pix(0), (u32)dest.rowpixels(), dwidth, dheight, sbase, (u32)source.rowpixels(), swidth, sheight, color, dx, dy);
            }
        }


        //-------------------------------------------------
        //  render_clip_line - clip a line to a rectangle
        //-------------------------------------------------
        public static bool render_clip_line(ref render_bounds bounds, in render_bounds clip)
        {
            // loop until we get a final result
            while (true)
            {
                u8 code0 = 0;
                u8 code1 = 0;
                u8 thiscode;
                float x;
                float y;

                // compute Cohen Sutherland bits for first coordinate
                if (bounds.y0 > clip.y1)
                    code0 |= 1;
                if (bounds.y0 < clip.y0)
                    code0 |= 2;
                if (bounds.x0 > clip.x1)
                    code0 |= 4;
                if (bounds.x0 < clip.x0)
                    code0 |= 8;

                // compute Cohen Sutherland bits for second coordinate
                if (bounds.y1 > clip.y1)
                    code1 |= 1;
                if (bounds.y1 < clip.y0)
                    code1 |= 2;
                if (bounds.x1 > clip.x1)
                    code1 |= 4;
                if (bounds.x1 < clip.x0)
                    code1 |= 8;

                // trivial accept: just return false
                if ((code0 | code1) == 0)
                    return false;

                // trivial reject: just return true
                if ((code0 & code1) != 0)
                    return true;

                // fix one of the OOB cases
                thiscode = code0 > 0 ? code0 : code1;

                // off the bottom
                if ((thiscode & 1) != 0)
                {
                    x = bounds.x0 + (bounds.x1 - bounds.x0) * (clip.y1 - bounds.y0) / (bounds.y1 - bounds.y0);
                    y = clip.y1;
                }

                // off the top
                else if ((thiscode & 2) != 0)
                {
                    x = bounds.x0 + (bounds.x1 - bounds.x0) * (clip.y0 - bounds.y0) / (bounds.y1 - bounds.y0);
                    y = clip.y0;
                }

                // off the right
                else if ((thiscode & 4) != 0)
                {
                    y = bounds.y0 + (bounds.y1 - bounds.y0) * (clip.x1 - bounds.x0) / (bounds.x1 - bounds.x0);
                    x = clip.x1;
                }

                // off the left
                else
                {
                    y = bounds.y0 + (bounds.y1 - bounds.y0) * (clip.x0 - bounds.x0) / (bounds.x1 - bounds.x0);
                    x = clip.x0;
                }

                // fix the appropriate coordinate
                if (thiscode == code0)
                {
                    bounds.x0 = x;
                    bounds.y0 = y;
                }
                else
                {
                    bounds.x1 = x;
                    bounds.y1 = y;
                }
            }
        }

        //-------------------------------------------------
        //  render_clip_quad - clip a quad to a rectangle
        //-------------------------------------------------
        public static bool render_clip_quad(ref render_bounds bounds, in render_bounds clip, ref render_quad_texuv texcoords, bool use_texcoords)
        {
            // ensure our assumptions about the bounds are correct
            assert(bounds.x0 <= bounds.x1);
            assert(bounds.y0 <= bounds.y1);

            // trivial reject
            if (bounds.y1 < clip.y0)
                return true;
            if (bounds.y0 > clip.y1)
                return true;
            if (bounds.x1 < clip.x0)
                return true;
            if (bounds.x0 > clip.x1)
                return true;

            // clip top (x0,y0)-(x1,y1)
            if (bounds.y0 < clip.y0)
            {
                float frac = (clip.y0 - bounds.y0) / (bounds.y1 - bounds.y0);
                bounds.y0 = clip.y0;
                if (use_texcoords)  //if (texcoords != nullptr)
                {
                    texcoords.tl.u += (texcoords.bl.u - texcoords.tl.u) * frac;
                    texcoords.tl.v += (texcoords.bl.v - texcoords.tl.v) * frac;
                    texcoords.tr.u += (texcoords.br.u - texcoords.tr.u) * frac;
                    texcoords.tr.v += (texcoords.br.v - texcoords.tr.v) * frac;
                }
            }

            // clip bottom (x3,y3)-(x2,y2)
            if (bounds.y1 > clip.y1)
            {
                float frac = (bounds.y1 - clip.y1) / (bounds.y1 - bounds.y0);
                bounds.y1 = clip.y1;
                if (use_texcoords)  //if (texcoords != nullptr)
                {
                    texcoords.bl.u -= (texcoords.bl.u - texcoords.tl.u) * frac;
                    texcoords.bl.v -= (texcoords.bl.v - texcoords.tl.v) * frac;
                    texcoords.br.u -= (texcoords.br.u - texcoords.tr.u) * frac;
                    texcoords.br.v -= (texcoords.br.v - texcoords.tr.v) * frac;
                }
            }

            // clip left (x0,y0)-(x3,y3)
            if (bounds.x0 < clip.x0)
            {
                float frac = (clip.x0 - bounds.x0) / (bounds.x1 - bounds.x0);
                bounds.x0 = clip.x0;
                if (use_texcoords)  //if (texcoords != nullptr)
                {
                    texcoords.tl.u += (texcoords.tr.u - texcoords.tl.u) * frac;
                    texcoords.tl.v += (texcoords.tr.v - texcoords.tl.v) * frac;
                    texcoords.bl.u += (texcoords.br.u - texcoords.bl.u) * frac;
                    texcoords.bl.v += (texcoords.br.v - texcoords.bl.v) * frac;
                }
            }

            // clip right (x1,y1)-(x2,y2)
            if (bounds.x1 > clip.x1)
            {
                float frac = (bounds.x1 - clip.x1) / (bounds.x1 - bounds.x0);
                bounds.x1 = clip.x1;
                if (use_texcoords)  //if (texcoords != nullptr)
                {
                    texcoords.tr.u -= (texcoords.tr.u - texcoords.tl.u) * frac;
                    texcoords.tr.v -= (texcoords.tr.v - texcoords.tl.v) * frac;
                    texcoords.br.u -= (texcoords.br.u - texcoords.bl.u) * frac;
                    texcoords.br.v -= (texcoords.br.v - texcoords.bl.v) * frac;
                }
            }

            return false;
        }


        //std::pair<render_bounds, render_bounds> render_line_to_quad(const render_bounds &bounds, float width, float length_extension);


        /*-------------------------------------------------
            render_load_msdib - load a Microsoft DIB file
            into a bitmap
        -------------------------------------------------*/
        public static void render_load_msdib(out bitmap_argb32 bitmap, util.random_read file)
        {
            throw new emu_unimplemented();
#if false
#endif
        }


        /*-------------------------------------------------
            render_load_jpeg - load a JPEG file into a
            bitmap
        -------------------------------------------------*/
        public static void render_load_jpeg(out bitmap_argb32 bitmap, util.random_read file)
        {
            throw new emu_unimplemented();
#if false
#endif
        }


        /*-------------------------------------------------
            render_load_png - load a PNG file into a
            bitmap
        -------------------------------------------------*/
        public static bool render_load_png(out bitmap_argb32 bitmap, util.random_read file, bool load_as_alpha_to_existing = false)
        {
            throw new emu_unimplemented();
#if false
#endif
        }


        //ru_imgformat render_detect_image(util::random_read &file);


        /*-------------------------------------------------
            render_round_nearest - floating point
            round-to-nearest
        -------------------------------------------------*/
        public static float render_round_nearest(float f) { return std.floorf(f + 0.5f); }


        /*-------------------------------------------------
            orientation_swap_flips - swap the X and Y
            flip flags
        -------------------------------------------------*/
        public static int orientation_swap_flips(int orientation)
        {
            return (int)((orientation & ORIENTATION_SWAP_XY) |
                        ((orientation & ORIENTATION_FLIP_X) != 0 ? ORIENTATION_FLIP_Y : 0) |
                        ((orientation & ORIENTATION_FLIP_Y) != 0 ? ORIENTATION_FLIP_X : 0));
        }


        /*-------------------------------------------------
            orientation_reverse - compute the orientation
            that will undo another orientation
        -------------------------------------------------*/
        public static int orientation_reverse(int orientation)
        {
            /* if not swapping X/Y, then just apply the same transform to reverse */
            if ((orientation & ORIENTATION_SWAP_XY) == 0)
                return orientation;

            /* if swapping X/Y, then swap X/Y flip bits to get the reverse */
            else
                return orientation_swap_flips(orientation);
        }


        /*-------------------------------------------------
            orientation_add - compute effective orientation
            after applying two subsequent orientations
        -------------------------------------------------*/
        public static int orientation_add(int orientation1, int orientation2)
        {
            /* if the 2nd transform doesn't swap, just XOR together */
            if ((orientation2 & ORIENTATION_SWAP_XY) == 0)
                return orientation1 ^ orientation2;

            /* otherwise, we need to effectively swap the flip bits on the first transform */
            else
                return orientation_swap_flips(orientation1) ^ orientation2;
        }


        /*-------------------------------------------------
            apply_brightness_contrast_gamma_fp - apply
            brightness, contrast, and gamma controls to
            a single RGB component
        -------------------------------------------------*/
        public static float apply_brightness_contrast_gamma_fp(float srcval, float brightness, float contrast, float gamma)
        {
            /* first apply gamma */
            srcval = std.pow(srcval, 1.0f / gamma);

            /* then contrast/brightness */
            srcval = (srcval * contrast) + brightness - 1.0f;

            /* clamp and return */
            return std.clamp(srcval, 0.0f, 1.0f);
        }


        /*-------------------------------------------------
            apply_brightness_contrast_gamma - apply
            brightness, contrast, and gamma controls to
            a single RGB component
        -------------------------------------------------*/
        public static u8 apply_brightness_contrast_gamma(u8 src, float brightness, float contrast, float gamma)
        {
            float srcval = (float)src * (1.0f / 255.0f);
            float result = apply_brightness_contrast_gamma_fp(srcval, brightness, contrast, gamma);
            return (u8)(result * 255.0f);
        }
    }


    static class rendutil_internal
    {
        //struct jpeg_corefile_source : public jpeg_source_mgr

        //struct jpeg_setjmp_error_mgr : public jpeg_error_mgr


        /*-------------------------------------------------
            resample_argb_bitmap_average - resample a texture
            by performing a true weighted average over
            all contributing pixels
        -------------------------------------------------*/
#if MCS_UNSAFE
        public unsafe static void resample_argb_bitmap_average(PointerU32 dest, u32 drowpixels, u32 dwidth, u32 dheight, PointerU32 source, u32 srowpixels, u32 swidth, u32 sheight, render_color color, u32 dx, u32 dy)  //static void resample_argb_bitmap_average(u32 *dest, u32 drowpixels, u32 dwidth, u32 dheight, const u32 *source, u32 srowpixels, u32 swidth, u32 sheight, const render_color &color, u32 dx, u32 dy)
#else
        public static void resample_argb_bitmap_average(PointerU32 dest, u32 drowpixels, u32 dwidth, u32 dheight, PointerU32 source, u32 srowpixels, u32 swidth, u32 sheight, render_color color, u32 dx, u32 dy)  //static void resample_argb_bitmap_average(u32 *dest, u32 drowpixels, u32 dwidth, u32 dheight, const u32 *source, u32 srowpixels, u32 swidth, u32 sheight, const render_color &color, u32 dx, u32 dy)
#endif
        {
            u64 sumscale = (u64)dx * (u64)dy;
            u32 r;
            u32 g;
            u32 b;
            u32 a;
            u32 x;
            u32 y;

            /* precompute premultiplied R/G/B/A factors */
            r = (u32)(color.r * color.a * 256.0f);
            g = (u32)(color.g * color.a * 256.0f);
            b = (u32)(color.b * color.a * 256.0f);
            a = (u32)(color.a * 256.0f);

#if MCS_UNSAFE
            fixed(u8 * dest_u8_zero = dest.Buffer.data_raw,
                       source_u8_zero = source.Buffer.data_raw)
            {
                u32 * dest_u32 = (u32 *)(dest_u8_zero + dest.Offset);
                u32 * source_u32 = (u32 *)(source_u8_zero + source.Offset);
#else
            {
                PointerU32 dest_u32 = dest;
                PointerU32 source_u32 = source;
#endif

                /* loop over the target vertically */
                for (y = 0; y < dheight; y++)
                {
                    u32 starty = y * dy;

                    /* loop over the target horizontally */
                    for (x = 0; x < dwidth; x++)
                    {
                        u64 sumr = 0;
                        u64 sumg = 0;
                        u64 sumb = 0;
                        u64 suma = 0;
                        u32 startx = x * dx;
                        u32 xchunk;
                        u32 ychunk;
                        u32 curx;
                        u32 cury;

                        u32 yremaining = dy;

                        /* accumulate all source pixels that contribute to this pixel */
                        for (cury = starty; yremaining != 0; cury += ychunk)
                        {
                            u32 xremaining = dx;

                            /* determine the Y contribution, clamping to the amount remaining */
                            ychunk = 0x1000 - (cury & 0xfff);
                            if (ychunk > yremaining)
                                ychunk = yremaining;
                            yremaining -= ychunk;

                            /* loop over all source pixels in the X direction */
                            for (curx = startx; xremaining != 0; curx += xchunk)
                            {
                                u32 factor;

                                /* determine the X contribution, clamping to the amount remaining */
                                xchunk = 0x1000 - (curx & 0xfff);
                                if (xchunk > xremaining)
                                    xchunk = xremaining;
                                xremaining -= xchunk;

                                /* total contribution = x * y */
                                factor = xchunk * ychunk;

                                /* fetch the source pixel */
                                rgb_t pix = new rgb_t(source_u32[(cury >> 12) * srowpixels + (curx >> 12)]);  //rgb_t pix = source[(cury >> 12) * srowpixels + (curx >> 12)];

                                /* accumulate the RGBA values */
                                sumr += factor * pix.r();
                                sumg += factor * pix.g();
                                sumb += factor * pix.b();
                                suma += factor * pix.a();
                            }
                        }

                        /* apply scaling */
                        suma = (suma / sumscale) * a / 256;
                        sumr = (sumr / sumscale) * r / 256;
                        sumg = (sumg / sumscale) * g / 256;
                        sumb = (sumb / sumscale) * b / 256;

                        /* if we're translucent, add in the destination pixel contribution */
                        if (a < 256)
                        {
                            rgb_t dpix = new rgb_t(dest_u32[y * drowpixels + x]);  //rgb_t dpix = dest[y * drowpixels + x];
                            suma += dpix.a() * (256 - a);
                            sumr += dpix.r() * (256 - a);
                            sumg += dpix.g() * (256 - a);
                            sumb += dpix.b() * (256 - a);
                        }

                        /* store the target pixel, dividing the RGBA values by the overall scale factor */
                        dest_u32[y * drowpixels + x] = new rgb_t((u8)suma, (u8)sumr, (u8)sumg, (u8)sumb);  //dest[y * drowpixels + x] = rgb_t(suma, sumr, sumg, sumb);
                    }
                }
            }
        }


        /*-------------------------------------------------
            resample_argb_bitmap_bilinear - perform texture
            sampling via a bilinear filter
        -------------------------------------------------*/
#if MCS_UNSAFE
        public unsafe static void resample_argb_bitmap_bilinear(PointerU32 dest, u32 drowpixels, u32 dwidth, u32 dheight, PointerU32 source, u32 srowpixels, u32 swidth, u32 sheight, render_color color, u32 dx, u32 dy)  //static void resample_argb_bitmap_bilinear(u32 *dest, u32 drowpixels, u32 dwidth, u32 dheight, const u32 *source, u32 srowpixels, u32 swidth, u32 sheight, const render_color &color, u32 dx, u32 dy)
#else
        public static void resample_argb_bitmap_bilinear(PointerU32 dest, u32 drowpixels, u32 dwidth, u32 dheight, PointerU32 source, u32 srowpixels, u32 swidth, u32 sheight, render_color color, u32 dx, u32 dy)  //static void resample_argb_bitmap_bilinear(u32 *dest, u32 drowpixels, u32 dwidth, u32 dheight, const u32 *source, u32 srowpixels, u32 swidth, u32 sheight, const render_color &color, u32 dx, u32 dy)
#endif
        {
            u32 maxx = swidth << 12;
            u32 maxy = sheight << 12;
            u32 r;
            u32 g;
            u32 b;
            u32 a;
            u32 x;
            u32 y;

            /* precompute premultiplied R/G/B/A factors */
            r = (u32)(color.r * color.a * 256.0f);
            g = (u32)(color.g * color.a * 256.0f);
            b = (u32)(color.b * color.a * 256.0f);
            a = (u32)(color.a * 256.0f);

#if MCS_UNSAFE
            fixed(u8 * dest_u8_zero = dest.Buffer.data_raw,
                       source_u8_zero = source.Buffer.data_raw)
            {
                u32 * dest_u32 = (u32 *)(dest_u8_zero + dest.Offset);
                u32 * source_u32 = (u32 *)(source_u8_zero + source.Offset);
#else
            {
                PointerU32 dest_u32 = dest;
                PointerU32 source_u32 = source;
#endif

                /* loop over the target vertically */
                for (y = 0; y < dheight; y++)
                {
                    u32 starty = y * dy;

                    /* loop over the target horizontally */
                    for (x = 0; x < dwidth; x++)
                    {
                        u32 startx = x * dx;
                        rgb_t pix0;
                        rgb_t pix1;
                        rgb_t pix2;
                        rgb_t pix3;
                        u32 sumr;
                        u32 sumg;
                        u32 sumb;
                        u32 suma;
                        u32 nextx;
                        u32 nexty;
                        u32 curx;
                        u32 cury;
                        u32 factor;

                        /* adjust start to the center; note that this math will tend to produce */
                        /* negative results on the first pixel, which is why we clamp below */
                        curx = startx + dx / 2 - 0x800;
                        cury = starty + dy / 2 - 0x800;

                        /* compute the neighboring pixel */
                        nextx = curx + 0x1000;
                        nexty = cury + 0x1000;

                        /* fetch the four relevant pixels */
                        pix0 = pix1 = pix2 = pix3 = new rgb_t(0);
                        if ((int)cury >= 0 && cury < maxy && (int)curx >= 0 && curx < maxx)
                            pix0 = new rgb_t(source_u32[(cury >> 12) * srowpixels + (curx >> 12)]);  //pix0 = source[(cury >> 12) * srowpixels + (curx >> 12)];
                        if ((int)cury >= 0 && cury < maxy && (int)nextx >= 0 && nextx < maxx)
                            pix1 = new rgb_t(source_u32[(cury >> 12) * srowpixels + (nextx >> 12)]);  //pix1 = source[(cury >> 12) * srowpixels + (nextx >> 12)];
                        if ((int)nexty >= 0 && nexty < maxy && (int)curx >= 0 && curx < maxx)
                            pix2 = new rgb_t(source_u32[(nexty >> 12) * srowpixels + (curx >> 12)]);  //pix2 = source[(nexty >> 12) * srowpixels + (curx >> 12)];
                        if ((int)nexty >= 0 && nexty < maxy && (int)nextx >= 0 && nextx < maxx)
                            pix3 = new rgb_t(source_u32[(nexty >> 12) * srowpixels + (nextx >> 12)]);  //pix3 = source[(nexty >> 12) * srowpixels + (nextx >> 12)];

                        /* compute the x/y scaling factors */
                        curx &= 0xfff;
                        cury &= 0xfff;

                        /* contributions from pixel 0 (top,left) */
                        factor = (0x1000 - curx) * (0x1000 - cury);
                        sumr = factor * pix0.r();
                        sumg = factor * pix0.g();
                        sumb = factor * pix0.b();
                        suma = factor * pix0.a();

                        /* contributions from pixel 1 (top,right) */
                        factor = curx * (0x1000 - cury);
                        sumr += factor * pix1.r();
                        sumg += factor * pix1.g();
                        sumb += factor * pix1.b();
                        suma += factor * pix1.a();

                        /* contributions from pixel 2 (bottom,left) */
                        factor = (0x1000 - curx) * cury;
                        sumr += factor * pix2.r();
                        sumg += factor * pix2.g();
                        sumb += factor * pix2.b();
                        suma += factor * pix2.a();

                        /* contributions from pixel 3 (bottom,right) */
                        factor = curx * cury;
                        sumr += factor * pix3.r();
                        sumg += factor * pix3.g();
                        sumb += factor * pix3.b();
                        suma += factor * pix3.a();

                        /* apply scaling */
                        suma = (suma >> 24) * a / 256;
                        sumr = (sumr >> 24) * r / 256;
                        sumg = (sumg >> 24) * g / 256;
                        sumb = (sumb >> 24) * b / 256;

                        /* if we're translucent, add in the destination pixel contribution */
                        if (a < 256)
                        {
                            rgb_t dpix = new rgb_t(dest_u32[y * drowpixels + x]);  //rgb_t dpix = dest[y * drowpixels + x];
                            suma += dpix.a() * (256 - a);
                            sumr += dpix.r() * (256 - a);
                            sumg += dpix.g() * (256 - a);
                            sumb += dpix.b() * (256 - a);
                        }

                        /* store the target pixel, dividing the RGBA values by the overall scale factor */
                        dest_u32[y * drowpixels + x] = new rgb_t((u8)suma, (u8)sumr, (u8)sumg, (u8)sumb);  //dest[y * drowpixels + x] = rgb_t(suma, sumr, sumg, sumb);
                    }
                }
            }
        }


        //static bool copy_png_alpha_to_bitmap(bitmap_argb32 &bitmap, const png_info *png);
    }
}
