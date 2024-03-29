// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using PointerU8 = mame.Pointer<System.Byte>;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;

using static mame.cpp_global;
using static mame.eminline_global;
using static mame.emucore_global;
using static mame.render_global;
using static mame.rendertypes_global;


namespace mame
{
    public class software_renderer<PixelType, int_SrcShiftR, int_SrcShiftG, int_SrcShiftB, int_DstShiftR, int_DstShiftG, int_DstShiftB> : software_renderer<PixelType, int_SrcShiftR, int_SrcShiftG, int_SrcShiftB, int_DstShiftR, int_DstShiftG, int_DstShiftB, bool_const_false, bool_const_false>
        where int_SrcShiftR : int_const, new()
        where int_SrcShiftG : int_const, new()
        where int_SrcShiftB : int_const, new()
        where int_DstShiftR : int_const, new()
        where int_DstShiftG : int_const, new()
        where int_DstShiftB : int_const, new()
    { }

    public class software_renderer<PixelType, int_SrcShiftR, int_SrcShiftG, int_SrcShiftB, int_DstShiftR, int_DstShiftG, int_DstShiftB, bool_NoDestRead> : software_renderer<PixelType, int_SrcShiftR, int_SrcShiftG, int_SrcShiftB, int_DstShiftR, int_DstShiftG, int_DstShiftB, bool_NoDestRead, bool_const_false>
        where int_SrcShiftR : int_const, new()
        where int_SrcShiftG : int_const, new()
        where int_SrcShiftB : int_const, new()
        where int_DstShiftR : int_const, new()
        where int_DstShiftG : int_const, new()
        where int_DstShiftB : int_const, new()
        where bool_NoDestRead : bool_const, new()
    { }

    //template <typename PixelType, int SrcShiftR, int SrcShiftG, int SrcShiftB, int DstShiftR, int DstShiftG, int DstShiftB, bool NoDestRead = false, bool BilinearFilter = false>
    public class software_renderer<PixelType, int_SrcShiftR, int_SrcShiftG, int_SrcShiftB, int_DstShiftR, int_DstShiftG, int_DstShiftB, bool_NoDestRead, bool_BilinearFilter>
        where int_SrcShiftR : int_const, new()
        where int_SrcShiftG : int_const, new()
        where int_SrcShiftB : int_const, new()
        where int_DstShiftR : int_const, new()
        where int_DstShiftG : int_const, new()
        where int_DstShiftB : int_const, new()
        where bool_NoDestRead : bool_const, new()
        where bool_BilinearFilter : bool_const, new()
    {
        static readonly int SrcShiftR = new int_SrcShiftR().value;
        static readonly int SrcShiftG = new int_SrcShiftG().value;
        static readonly int SrcShiftB = new int_SrcShiftB().value;
        static readonly int DstShiftR = new int_DstShiftR().value;
        static readonly int DstShiftG = new int_DstShiftG().value;
        static readonly int DstShiftB = new int_DstShiftB().value;
        static readonly bool NoDestRead = new bool_NoDestRead().value;
        static readonly bool BilinearFilter = new bool_BilinearFilter().value;


#if MCS_UNSAFE
        unsafe interface PixelType_operations
#else
        interface PixelType_operations
#endif
        {
            PointerU8 GetPointer(PointerU8 dstdata, int offset);
            u32 GetValue(PointerU8 dest);
            void SetValue(PointerU8 dest, u32 value);
            void SetValueAndIncrement(PointerU8 dest, u32 value);
            void Increment(PointerU8 dest);

#if MCS_UNSAFE
            u8 * UnsafeGetPointer(u8 * dstdata, int offset);
            u32 UnsafeGetValue(u8 * dest);
            void UnsafeSetValue(u8 * dest, u32 value);
            void UnsafeSetValueAndIncrement(ref u8 * dest, u32 value);
            void UnsafeIncrement(ref u8 * dest);
#else
            PointerU8 UnsafeGetPointer(PointerU8 dstdata, int offset);
            u32 UnsafeGetValue(PointerU8 dest);
            void UnsafeSetValue(PointerU8 dest, u32 value);
            void UnsafeSetValueAndIncrement(ref PointerU8 dest, u32 value);
            void UnsafeIncrement(ref PointerU8 dest);
#endif
        }

#if MCS_UNSAFE
        unsafe class PixelType_operations_u8 : PixelType_operations
#else
        class PixelType_operations_u8 : PixelType_operations
#endif
        {
            public PointerU8 GetPointer(PointerU8 dstdata, int offset) { return new PointerU8(dstdata, offset); }
            public u32 GetValue(PointerU8 dest) { return dest[0]; }
            public void SetValue(PointerU8 dest, u32 value) { dest[0] = (u8)value; }
            public void SetValueAndIncrement(PointerU8 dest, u32 value) { dest[0] = (u8)value; dest++; }
            public void Increment(PointerU8 dest) { dest++; }

#if MCS_UNSAFE
            public u8 * UnsafeGetPointer(u8 * dstdata, int offset) { return dstdata + offset; }
            public u32 UnsafeGetValue(u8 * dest) { return dest[0]; }
            public void UnsafeSetValue(u8 * dest, u32 value) { dest[0] = (u8)value; }
            public void UnsafeSetValueAndIncrement(ref u8 * dest, u32 value) { dest[0] = (u8)value; dest++; }
            public void UnsafeIncrement(ref u8 * dest) { dest++; }
#else
            public PointerU8 UnsafeGetPointer(PointerU8 dstdata, int offset) { return GetPointer(dstdata, offset); }
            public u32 UnsafeGetValue(PointerU8 dest) { return GetValue(dest); }
            public void UnsafeSetValue(PointerU8 dest, u32 value) { SetValue(dest, value); }
            public void UnsafeSetValueAndIncrement(ref PointerU8 dest, u32 value) { SetValueAndIncrement(dest, value); }
            public void UnsafeIncrement(ref PointerU8 dest) { Increment(dest); }
#endif
        }

#if MCS_UNSAFE
        unsafe class PixelType_operations_u16 : PixelType_operations
#else
        class PixelType_operations_u16 : PixelType_operations
#endif
        {
            public PointerU8 GetPointer(PointerU8 dstdata, int offset) { return new PointerU16(dstdata, offset); }
            public u32 GetValue(PointerU8 dest) { return ((PointerU16)dest)[0]; }
            public void SetValue(PointerU8 dest, u32 value) { var dest16 = (PointerU16)dest; dest16[0] = (u16)value; }
            public void SetValueAndIncrement(PointerU8 dest, u32 value) { var dest16 = (PointerU16)dest; dest16[0] = (u16)value; dest16++; }
            public void Increment(PointerU8 dest) { var dest16 = (PointerU16)dest; dest16++; }

#if MCS_UNSAFE
            public u8 * UnsafeGetPointer(u8 * dstdata, int offset) { return dstdata + (offset * 2); }
            public u32 UnsafeGetValue(u8 * dest) { return ((u16 *)dest)[0]; }
            public void UnsafeSetValue(u8 * dest, u32 value) { ((u16 *)dest)[0] = (u16)value; }
            public void UnsafeSetValueAndIncrement(ref u8 * dest, u32 value) { ((u16 *)dest)[0] = (u16)value; dest += 2; }
            public void UnsafeIncrement(ref u8 * dest) { dest += 2; }
#else
            public PointerU8 UnsafeGetPointer(PointerU8 dstdata, int offset) { return GetPointer(dstdata, offset); }
            public u32 UnsafeGetValue(PointerU8 dest) { return GetValue(dest); }
            public void UnsafeSetValue(PointerU8 dest, u32 value) { SetValue(dest, value); }
            public void UnsafeSetValueAndIncrement(ref PointerU8 dest, u32 value) { SetValueAndIncrement(dest, value); }
            public void UnsafeIncrement(ref PointerU8 dest) { Increment(dest); }
#endif
        }

#if MCS_UNSAFE
        unsafe class PixelType_operations_u32 : PixelType_operations
#else
        class PixelType_operations_u32 : PixelType_operations
#endif
        {
            public PointerU8 GetPointer(PointerU8 dstdata, int offset) { return new PointerU32(dstdata, offset); }
            public u32 GetValue(PointerU8 dest) { return ((PointerU32)dest)[0]; }
            public void SetValue(PointerU8 dest, u32 value) { var dest32 = (PointerU32)dest; dest32[0] = value; }
            public void SetValueAndIncrement(PointerU8 dest, u32 value) { var dest32 = (PointerU32)dest; dest32[0] = (u32)value; dest32++; }
            public void Increment(PointerU8 dest) { var dest32 = (PointerU32)dest; dest32++; }

#if MCS_UNSAFE
            public u8 * UnsafeGetPointer(u8 * dstdata, int offset) { return dstdata + (offset * 4); }
            public u32 UnsafeGetValue(u8 * dest) { return ((u32 *)dest)[0]; }
            public void UnsafeSetValue(u8 * dest, u32 value) { ((u32 *)dest)[0] = (u32)value; }
            public void UnsafeSetValueAndIncrement(ref u8 * dest, u32 value) { ((u32 *)dest)[0] = (u32)value; dest += 4; }
            public void UnsafeIncrement(ref u8 * dest) { dest += 4; }
#else
            public PointerU8 UnsafeGetPointer(PointerU8 dstdata, int offset) { return GetPointer(dstdata, offset); }
            public u32 UnsafeGetValue(PointerU8 dest) { return GetValue(dest); }
            public void UnsafeSetValue(PointerU8 dest, u32 value) { SetValue(dest, value); }
            public void UnsafeSetValueAndIncrement(ref PointerU8 dest, u32 value) { SetValueAndIncrement(dest, value); }
            public void UnsafeIncrement(ref PointerU8 dest) { Increment(dest); }
#endif
        }

        static PixelType_operations GetPixelType_operations()
        {
            if      (typeof(PixelType) == typeof(u8))  return new PixelType_operations_u8();
            else if (typeof(PixelType) == typeof(u16)) return new PixelType_operations_u16();
            else if (typeof(PixelType) == typeof(u32)) return new PixelType_operations_u32();
            else throw new emu_unimplemented();
        }

        static readonly PixelType_operations ops = GetPixelType_operations();


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
        static bool is_opaque(float alpha) { return alpha >= (NoDestRead ? 0.5f : 1.0f); }
        static bool is_transparent(float alpha) { return alpha < (NoDestRead ? 0.5f : 0.0001f); }
        static rgb_t apply_intensity(int intensity, rgb_t color) { return color.scale8((u8)intensity); }
        static float round_nearest(float f) { return std.floor(f + 0.5f); }


        // destination pixels are written based on the values of the template parameters
        //static inline _PixelType dest_assemble_rgb(u32 r, u32 g, u32 b) { return (r << _DstShiftR) | (g << _DstShiftG) | (b << _DstShiftB); }
        //static inline _PixelType dest_rgb_to_pixel(u32 r, u32 g, u32 b) { return dest_assemble_rgb(r >> _SrcShiftR, g >> _SrcShiftG, b >> _SrcShiftB); }
        static u8 dest_assemble_rgb8(u32 r, u32 g, u32 b) { return (u8)((r << DstShiftR) | (g << DstShiftG) | (b << DstShiftB)); }
        static u16 dest_assemble_rgb16(u32 r, u32 g, u32 b) { return (u16)((r << DstShiftR) | (g << DstShiftG) | (b << DstShiftB)); }
        static u32 dest_assemble_rgb32(u32 r, u32 g, u32 b) { return (r << DstShiftR) | (g << DstShiftG) | (b << DstShiftB); }
        static u8 dest_rgb_to_pixel8(u32 r, u32 g, u32 b) { return dest_assemble_rgb8(r >> SrcShiftR, g >> SrcShiftG, b >> SrcShiftB); }
        static u16 dest_rgb_to_pixel16(u32 r, u32 g, u32 b) { return dest_assemble_rgb16(r >> SrcShiftR, g >> SrcShiftG, b >> SrcShiftB); }
        static u32 dest_rgb_to_pixel32(u32 r, u32 g, u32 b) { return dest_assemble_rgb32(r >> SrcShiftR, g >> SrcShiftG, b >> SrcShiftB); }


        // source 32-bit pixels are in MAME standardized format
        static u32 source32_r(u32 pixel) { return (u32)((pixel >> (16 + SrcShiftR)) & (0xff >> SrcShiftR)); }
        static u32 source32_g(u32 pixel) { return (u32)((pixel >> ( 8 + SrcShiftG)) & (0xff >> SrcShiftG)); }
        static u32 source32_b(u32 pixel) { return (u32)((pixel >> ( 0 + SrcShiftB)) & (0xff >> SrcShiftB)); }


        // destination pixel masks are based on the template parameters as well
        //static inline u32 dest_r(_PixelType pixel) { return (pixel >> _DstShiftR) & (0xff >> _SrcShiftR); }
        static u32 dest_r8(u8 pixel) { return ((u32)pixel >> DstShiftR) & ((u32)0xff >> SrcShiftR); }
        static u32 dest_r16(u16 pixel) { return ((u32)pixel >> DstShiftR) & ((u32)0xff >> SrcShiftR); }
        static u32 dest_r32(u32 pixel) { return ((u32)pixel >> DstShiftR) & ((u32)0xff >> SrcShiftR); }
        //static inline u32 dest_g(_PixelType pixel) { return (pixel >> _DstShiftG) & (0xff >> _SrcShiftG); }
        static u32 dest_g8(u8 pixel) { return ((u32)pixel >> DstShiftG) & ((u32)0xff >> SrcShiftG); }
        static u32 dest_g16(u16 pixel) { return ((u32)pixel >> DstShiftG) & ((u32)0xff >> SrcShiftG); }
        static u32 dest_g32(u32 pixel) { return ((u32)pixel >> DstShiftG) & ((u32)0xff >> SrcShiftG); }
        //static inline u32 dest_b(_PixelType pixel) { return (pixel >> _DstShiftB) & (0xff >> _SrcShiftB); }
        static u32 dest_b8(u8 pixel) { return ((u32)pixel >> DstShiftB) & ((u32)0xff >> SrcShiftB); }
        static u32 dest_b16(u16 pixel) { return ((u32)pixel >> DstShiftB) & ((u32)0xff >> SrcShiftB); }
        static u32 dest_b32(u32 pixel) { return ((u32)pixel >> DstShiftB) & ((u32)0xff >> SrcShiftB); }


        // generic conversion with special optimization for destinations in the standard format
        //static inline _PixelType source32_to_dest(u32 pixel)
        //{
        //    if (_SrcShiftR == 0 && _SrcShiftG == 0 && _SrcShiftB == 0 && _DstShiftR == 16 && _DstShiftG == 8 && _DstShiftB == 0)
        //        return pixel;
        //    else
        //        return dest_assemble_rgb(source32_r(pixel), source32_g(pixel), source32_b(pixel));
        //}
        static u8 source32_to_dest8(u32 pixel)
        {
            if (SrcShiftR == 0 && SrcShiftG == 0 && SrcShiftB == 0 && DstShiftR == 16 && DstShiftG == 8 && DstShiftB == 0)
                return (u8)pixel;
            else
                return dest_assemble_rgb8(source32_r(pixel), source32_g(pixel), source32_b(pixel));
        }
        static u16 source32_to_dest16(u32 pixel)
        {
            if (SrcShiftR == 0 && SrcShiftG == 0 && SrcShiftB == 0 && DstShiftR == 16 && DstShiftG == 8 && DstShiftB == 0)
                return (u16)pixel;
            else
                return dest_assemble_rgb16(source32_r(pixel), source32_g(pixel), source32_b(pixel));
        }
        static u32 source32_to_dest32(u32 pixel)
        {
            if (SrcShiftR == 0 && SrcShiftG == 0 && SrcShiftB == 0 && DstShiftR == 16 && DstShiftG == 8 && DstShiftB == 0)
                return pixel;
            else
                return dest_assemble_rgb32(source32_r(pixel), source32_g(pixel), source32_b(pixel));
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
            return (((s32)x < 0) ? 0 : (x > 65535) ? 255 : (x >> 8));
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
#if MCS_UNSAFE
        unsafe static u32 get_texel_palette16(render_texinfo texture, s32 curu, s32 curv, u16 * texbase_u16)
#else
        static u32 get_texel_palette16(render_texinfo texture, s32 curu, s32 curv, PointerU16 unused)
#endif
        {
            Pointer<rgb_t> palbase = texture.palette;  //rgb_t const *const palbase = texture.palette;
            if (BilinearFilter)
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

#if MCS_UNSAFE
                {
                    var texbase = texbase_u16;  //u16 const *texbase = reinterpret_cast<u16 const *>(texture.base);
#else
                {
                    var texbase = new PointerU16(texture.base_);  //u16 const *texbase = reinterpret_cast<u16 const *>(texture.base);
#endif

                    texbase += v0 * (s32)texture.rowpixels + u0;

                    u32 pix00 = palbase[texbase[0]];
                    u32 pix01 = palbase[texbase[u1]];
                    u32 pix10 = palbase[texbase[v1]];
                    u32 pix11 = palbase[texbase[u1 + v1]];
                    return rgbaint_t.bilinear_filter(pix00, pix01, pix10, pix11, (u8)(curu >> 8), (u8)(curv >> 8));
                }
            }
            else
            {
#if MCS_UNSAFE
                {
                    var texbase = texbase_u16 + (curv >> 16) * texture.rowpixels + (curu >> 16);  //u16 const *const texbase = reinterpret_cast<u16 const *>(texture.base) + (curv >> 16) * texture.rowpixels + (curu >> 16);
#else
                {
                    var texbase = new PointerU16(texture.base_) + (curv >> 16) * (s32)texture.rowpixels + (curu >> 16);  //u16 const *const texbase = reinterpret_cast<u16 const *>(texture.base) + (curv >> 16) * texture.rowpixels + (curu >> 16);
#endif

                    return palbase[texbase[0]];
                }
            }
        }


        //-------------------------------------------------
        //  get_texel_palette16a - return a texel from a
        //  palettized 16bpp source with alpha
        //-------------------------------------------------
        //static inline UINT32 get_texel_palette16a(const render_texinfo &texture, INT32 curu, INT32 curv)

        //-------------------------------------------------
        //  get_texel_yuy16 - return a texel from a 16bpp
        //  YCbCr source (pixel is returned as Cr-Cb-Y)
        //-------------------------------------------------
        //static inline UINT32 get_texel_yuy16(const render_texinfo &texture, INT32 curu, INT32 curv)


        //-------------------------------------------------
        //  get_texel_rgb32 - return a texel from a 32bpp
        //  RGB source
        //-------------------------------------------------
        //template <bool Wrap>
        static u32 get_texel_rgb32<bool_Wrap>(render_texinfo texture, s32 curu, s32 curv)
            where bool_Wrap : bool_const, new()
        {
            bool Wrap = new bool_Wrap().value;

            if (BilinearFilter)
            {
                s32 u0;
                s32 u1;
                s32 v0;
                s32 v1;

                if (Wrap)
                {
                    u0 = (curu >> 16) % (s32)texture.width;
                    if (0 > u0)
                        u0 += (s32)texture.width;
                    u1 = (u0 + 1) % (s32)texture.width;

                    v0 = (curv >> 16) % (s32)texture.height;
                    if (0 > v0)
                        v0 += (s32)texture.height;
                    v1 = (v0 + 1) % (s32)texture.height;
                }
                else
                {
                    u0 = curu >> 16;
                    if (u0 < 0)
                        u0 = u1 = 0;
                    else if (texture.width <= (u0 + 1))
                        u0 = u1 = (s32)texture.width - 1;
                    else
                        u1 = u0 + 1;

                    v0 = curv >> 16;
                    if (v0 < 0)
                        v0 = v1 = 0;
                    else if (texture.height <= (v0 + 1))
                        v0 = v1 = (s32)texture.height - 1;
                    else
                        v1 = v0 + 1;
                }

                PointerU32 texbase = new PointerU32(texture.base_);  //u32 const *const texbase = reinterpret_cast<u32 const *>(texture.base);
                PointerU32 row0base = texbase + (v0 * (s32)texture.rowpixels);  //u32 const *const row0base = texbase + (v0 * texture.rowpixels);
                PointerU32 row1base = texbase + (v1 * (s32)texture.rowpixels);  //u32 const *const row1base = texbase + (v1 * texture.rowpixels);
                return rgbaint_t.bilinear_filter(row0base[u0], row0base[u1], row1base[u0], row1base[u1], (u8)(curu >> 8), (u8)(curv >> 8));  //return rgbaint_t::bilinear_filter(row0base[u0], row0base[u1], row1base[u0], row1base[u1], curu >> 8, curv >> 8);
            }
            else
            {
                s32 u;
                s32 v;

                if (Wrap)
                {
                    u = (curu >> 16) % (s32)texture.width;
                    if (0 > u)
                        u += (s32)texture.width;

                    v = (curv >> 16) % (s32)texture.height;
                    if (0 > v)
                        v += (s32)texture.height;
                }
                else
                {
                    u = std.clamp(curu >> 16, 0, (s32)texture.width - 1);
                    v = std.clamp(curv >> 16, 0, (s32)texture.height - 1);
                }

                PointerU32 rowbase = new PointerU32(texture.base_) + (v * (s32)texture.rowpixels);  //u32 const *const rowbase = reinterpret_cast<u32 const *>(texture.base) + (v * texture.rowpixels);
                return rowbase[u];
            }
        }


        //-------------------------------------------------
        //  get_texel_argb32 - return a texel from a 32bpp
        //  ARGB source
        //-------------------------------------------------
        //template <bool Wrap>
#if MCS_UNSAFE
        unsafe static u32 get_texel_argb32(bool Wrap, render_texinfo texture, s32 curu, s32 curv)
#else
        static u32 get_texel_argb32(bool Wrap, render_texinfo texture, s32 curu, s32 curv)
#endif
        {
            if (BilinearFilter)
            {
                s32 u0;
                s32 u1;
                s32 v0;
                s32 v1;

                if (Wrap)
                {
                    u0 = (curu >> 16) % (s32)texture.width;
                    if (0 > u0)
                        u0 += (s32)texture.width;
                    u1 = (u0 + 1) % (s32)texture.width;

                    v0 = (curv >> 16) % (s32)texture.height;
                    if (0 > v0)
                        v0 += (s32)texture.height;
                    v1 = (v0 + 1) % (s32)texture.height;
                }
                else
                {
                    u0 = curu >> 16;
                    if (u0 < 0)
                        u0 = u1 = 0;
                    else if (texture.width <= (u0 + 1))
                        u0 = u1 = (s32)texture.width - 1;
                    else
                        u1 = u0 + 1;

                    v0 = curv >> 16;
                    if (v0 < 0)
                        v0 = v1 = 0;
                    else if (texture.height <= (v0 + 1))
                        v0 = v1 = (s32)texture.height - 1;
                    else
                        v1 = v0 + 1;
                }

#if MCS_UNSAFE
                fixed(u8 * texbase_u8_zero = texture.base_.Buffer.data_raw)
                {
                    u8 * texbase_u8 = texbase_u8_zero + texture.base_.Offset;

                    u32 * texbase = (u32 *)texbase_u8;  //u32 const *const texbase = reinterpret_cast<u32 const *>(texture.base);
                    u32 * row0base = texbase + (v0 * texture.rowpixels);  //u32 const *const row0base = texbase + (v0 * texture.rowpixels);
                    u32 * row1base = texbase + (v1 * texture.rowpixels);  //u32 const *const row1base = texbase + (v1 * texture.rowpixels);
                    return rgbaint_t.bilinear_filter(row0base[u0], row0base[u1], row1base[u0], row1base[u1], (u8)(curu >> 8), (u8)(curv >> 8));  //return rgbaint_t::bilinear_filter(row0base[u0], row0base[u1], row1base[u0], row1base[u1], curu >> 8, curv >> 8);
                }
#else
                PointerU32 texbase = new PointerU32(texture.base_);  //u32 const *const texbase = reinterpret_cast<u32 const *>(texture.base);
                PointerU32 row0base = texbase + (v0 * (s32)texture.rowpixels);  //u32 const *const row0base = texbase + (v0 * texture.rowpixels);
                PointerU32 row1base = texbase + (v1 * (s32)texture.rowpixels);  //u32 const *const row1base = texbase + (v1 * texture.rowpixels);
                return rgbaint_t.bilinear_filter(row0base[u0], row0base[u1], row1base[u0], row1base[u1], (u8)(curu >> 8), (u8)(curv >> 8));  //return rgbaint_t::bilinear_filter(row0base[u0], row0base[u1], row1base[u0], row1base[u1], curu >> 8, curv >> 8);
#endif
            }
            else
            {
                s32 u;
                s32 v;

                if (Wrap)
                {
                    u = (curu >> 16) % (s32)texture.width;
                    if (0 > u)
                        u += (s32)texture.width;

                    v = (curv >> 16) % (s32)texture.height;
                    if (0 > v)
                        v += (s32)texture.height;
                }
                else
                {
                    u = std.clamp(curu >> 16, 0, (s32)texture.width - 1);
                    v = std.clamp(curv >> 16, 0, (s32)texture.height - 1);
                }

#if MCS_UNSAFE
                fixed(u8 * rowbase_u8_zero = texture.base_.Buffer.data_raw)
                {
                    u8 * rowbase_u8 = rowbase_u8_zero + texture.base_.Offset;

                    u32 * rowbase = (u32 *)rowbase_u8 + (v * texture.rowpixels);  //u32 const *const rowbase = reinterpret_cast<u32 const *>(texture.base) + (v * texture.rowpixels);
                    return rowbase[u];
                }
#else
                PointerU32 rowbase = new PointerU32(texture.base_) + (v * (s32)texture.rowpixels);  //u32 const *const rowbase = reinterpret_cast<u32 const *>(texture.base) + (v * texture.rowpixels);
                return rowbase[u];
#endif
            }
        }


        //-------------------------------------------------
        //  draw_aa_pixel - draw an antialiased pixel
        //-------------------------------------------------
        static void draw_aa_pixel(PointerU8 dstdata, u32 pitch, int x, int y, u32 col)  //static inline void draw_aa_pixel(_PixelType *dstdata, u32 pitch, int x, int y, u32 col)
        {
            PointerU8 dest = ops.GetPointer(dstdata, y * (int)pitch + x);  //_PixelType *dest = dstdata + y * pitch + x;
            u32 dpix = NoDestRead ? 0 : ops.GetValue(dest);  //u32 dpix = NoDestRead ? 0 : *dest;
            u32 dr = source32_r(col) + dest_r32(dpix);  //u32 dr = source32_r(col) + dest_r(dpix);
            u32 dg = source32_g(col) + dest_g32(dpix);  //u32 dg = source32_g(col) + dest_g(dpix);
            u32 db = source32_b(col) + dest_b32(dpix);  //u32 db = source32_b(col) + dest_b(dpix);
            dr = (u32)(dr | -(dr >> (8 - SrcShiftR))) & ((u32)0xff >> SrcShiftR);
            dg = (u32)(dg | -(dg >> (8 - SrcShiftG))) & ((u32)0xff >> SrcShiftG);
            db = (u32)(db | -(db >> (8 - SrcShiftB))) & ((u32)0xff >> SrcShiftB);
            ops.SetValue(dest, dest_assemble_rgb32(dr, dg, db));  //*dest = dest_assemble_rgb(dr, dg, db);
        }


        // internal tables
        static u32 [] s_cosine_table = new u32[2049];


        //-------------------------------------------------
        //  draw_line - draw a line or point
        //-------------------------------------------------
        static void draw_line(render_primitive prim, PointerU8 dstdata, s32 width, s32 height, u32 pitch)  //static void draw_line(render_primitive const &prim, PixelType *dstdata, s32 width, s32 height, u32 pitch)
        {
            // compute the start/end coordinates
            int x1 = (int)(prim.bounds.x0 * 65536.0f);
            int y1 = (int)(prim.bounds.y0 * 65536.0f);
            int x2 = (int)(prim.bounds.x1 * 65536.0f);
            int y2 = (int)(prim.bounds.y1 * 65536.0f);

            // handle color and intensity
            u32 col = new rgb_t((u8)(255.0f * prim.color.r * prim.color.a), (u8)(255.0f * prim.color.g * prim.color.a), (u8)(255.0f * prim.color.b * prim.color.a));

            if (PRIMFLAG_GET_ANTIALIAS(prim.flags))
            {
                // build up the cosine table if we haven't yet
                if (s_cosine_table[0] == 0)
                    for (int entry = 0; entry <= 2048; entry++)
                        s_cosine_table[entry] = (u32)(int)((double)(1.0 / std.cos(std.atan((double)(entry) / 2048.0))) * 0x10000000 + 0.5);

                int beam = (int)(prim.width * 65536.0f);
                if (beam < 0x00010000)
                    beam = 0x00010000;

                // draw an anti-aliased line
                int dx = std.abs(x1 - x2);
                int dy = std.abs(y1 - y2);
                if (dx >= dy)
                {
                    int sx = ((x1 <= x2) ? 1 : -1);
                    int sy = (dy == 0) ? 0 : div_32x32_shift(y2 - y1, dx, 16);
                    if (sy < 0)
                        dy--;
                    x1 >>= 16;
                    int xx = x2 >> 16;
                    int bwidth = mul_32x32_hi(beam << 4, (int)s_cosine_table[std.abs(sy) >> 5]);
                    y1 -= bwidth >> 1; // start back half the diameter
                    for (;;)
                    {
                        if (x1 >= 0 && x1 < width)
                        {
                            dx = bwidth;    // init diameter of beam
                            dy = y1 >> 16;
                            if (dy >= 0 && dy < height)
                                draw_aa_pixel(dstdata, pitch, x1, dy, apply_intensity(0xff & (~y1 >> 8), new rgb_t(col)));
                            dy++;
                            dx -= 0x10000 - (0xffff & y1); // take off amount plotted
                            u8 a1 = (u8)((dx >> 8) & 0xff);   // calc remainder pixel
                            dx >>= 16;                   // adjust to pixel (solid) count
                            while (dx-- != 0)                 // plot rest of pixels
                            {
                                if (dy >= 0 && dy < height)
                                    draw_aa_pixel(dstdata, pitch, x1, dy, col);
                                dy++;
                            }
                            if (dy >= 0 && dy < height)
                                draw_aa_pixel(dstdata, pitch, x1, dy, apply_intensity(a1, new rgb_t(col)));
                        }
                        if (x1 == xx) break;
                        x1 += sx;
                        y1 += sy;
                    }
                }
                else
                {
                    int sy = ((y1 <= y2) ? 1: -1);
                    int sx = (dx == 0) ? 0 : div_32x32_shift(x2 - x1, dy, 16);
                    if (sx < 0)
                        dx--;
                    y1 >>= 16;
                    int yy = y2 >> 16;
                    int bwidth = mul_32x32_hi(beam << 4, (int)s_cosine_table[std.abs(sx) >> 5]);
                    x1 -= bwidth >> 1; // start back half the width
                    for (;;)
                    {
                        if (y1 >= 0 && y1 < height)
                        {
                            dy = bwidth;    // calc diameter of beam
                            dx = x1 >> 16;
                            if (dx >= 0 && dx < width)
                                draw_aa_pixel(dstdata, pitch, dx, y1, apply_intensity(0xff & (~x1 >> 8), new rgb_t(col)));
                            dx++;
                            dy -= 0x10000 - (0xffff & x1); // take off amount plotted
                            u8 a1 = (u8)((dy >> 8) & 0xff);   // remainder pixel
                            dy >>= 16;                   // adjust to pixel (solid) count
                            while (dy-- != 0)                 // plot rest of pixels
                            {
                                if (dx >= 0 && dx < width)
                                    draw_aa_pixel(dstdata, pitch, dx, y1, col);
                                dx++;
                            }
                            if (dx >= 0 && dx < width)
                                draw_aa_pixel(dstdata, pitch, dx, y1, apply_intensity(a1, new rgb_t(col)));
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
                            draw_aa_pixel(dstdata, pitch, x1, y1, col);
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
                            draw_aa_pixel(dstdata, pitch, x1, y1, col);
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
#if MCS_UNSAFE
        unsafe static void draw_rect(render_primitive prim, PointerU8 dstdata, s32 width, s32 height, u32 pitch)  //static void draw_rect(render_primitive const &prim, PixelType *dstdata, s32 width, s32 height, u32 pitch)
#else
        static void draw_rect(render_primitive prim, PointerU8 dstdata, s32 width, s32 height, u32 pitch)  //static void draw_rect(render_primitive const &prim, PixelType *dstdata, s32 width, s32 height, u32 pitch)
#endif
        {
            render_bounds fpos = prim.bounds;

            assert(fpos.x0 <= fpos.x1);
            assert(fpos.y0 <= fpos.y1);

            // clamp to integers
            s32 startx = std.clamp((s32)round_nearest(fpos.x0), 0, width);
            s32 starty = std.clamp((s32)round_nearest(fpos.y0), 0, height);
            s32 endx = std.clamp((s32)round_nearest(fpos.x1), 0, width);
            s32 endy = std.clamp((s32)round_nearest(fpos.y1), 0, height);

            // bail if nothing left
            if ((startx > endx) || (starty > endy))
                return;

            // only support alpha and "none" blendmodes
            assert(PRIMFLAG_GET_BLENDMODE(prim.flags) == BLENDMODE_NONE || PRIMFLAG_GET_BLENDMODE(prim.flags) == BLENDMODE_ALPHA);

            if ((PRIMFLAG_GET_BLENDMODE(prim.flags) == BLENDMODE_NONE) || is_opaque(prim.color.a))
            {
                // fast case: no alpha

                // clamp R,G,B to 0-256 range
                u32 r = (u32)(std.clamp(256.0f * prim.color.r, 0.0f, 255.0f));
                u32 g = (u32)(std.clamp(256.0f * prim.color.g, 0.0f, 255.0f));
                u32 b = (u32)(std.clamp(256.0f * prim.color.b, 0.0f, 255.0f));
                u32 pix = dest_rgb_to_pixel32(r, g, b);

#if MCS_UNSAFE
                fixed(u8 * dstdata_u8_zero = dstdata.Buffer.data_raw)
                {
                    u8 * dstdata_u8 = dstdata_u8_zero + dstdata.Offset;
#else
                {
                    PointerU8 dstdata_u8 = dstdata;
#endif

                    // loop over rows
                    for (s32 y = starty; y < endy; y++)
                    {
                        var dest = ops.UnsafeGetPointer(dstdata_u8, y * (int)pitch + startx);  //PixelType *dest = dstdata + y * pitch + startx;

                        // loop over cols
                        for (s32 x = startx; x < endx; x++)
                            ops.UnsafeSetValueAndIncrement(ref dest, pix);  //*dest++ = pix;
                    }
                }
            }
            else if (!is_transparent(prim.color.a))
            {
                // alpha and/or coloring case

                u32 rmask = dest_rgb_to_pixel32(0xff, 0x00, 0x00);
                u32 gmask = dest_rgb_to_pixel32(0x00, 0xff, 0x00);
                u32 bmask = dest_rgb_to_pixel32(0x00, 0x00, 0xff);

                // clamp R,G,B and inverse A to 0-256 range
                u32 r = (u32)(std.clamp(256.0f * prim.color.r * prim.color.a, 0.0f, 255.0f));
                u32 g = (u32)(std.clamp(256.0f * prim.color.g * prim.color.a, 0.0f, 255.0f));
                u32 b = (u32)(std.clamp(256.0f * prim.color.b * prim.color.a, 0.0f, 255.0f));
                u32 inva = (u32)(std.clamp(256.0f * (1.0f - prim.color.a), 0.0f, 256.0f));

                // pre-shift the RGBA pieces
                r = dest_rgb_to_pixel32(r, 0, 0) << 8;
                g = dest_rgb_to_pixel32(0, g, 0) << 8;
                b = dest_rgb_to_pixel32(0, 0, b) << 8;

#if MCS_UNSAFE
                fixed(u8 * dstdata_u8_zero = dstdata.Buffer.data_raw)
                {
                    u8 * dstdata_u8 = dstdata_u8_zero + dstdata.Offset;
#else
                {
                    PointerU8 dstdata_u8 = dstdata;
#endif

                    // loop over rows
                    for (s32 y = starty; y < endy; y++)
                    {
                        var dest = ops.UnsafeGetPointer(dstdata_u8, y * (int)pitch + startx);  //PixelType *dest = dstdata + y * pitch + startx;

                        // loop over cols
                        for (s32 x = startx; x < endx; x++)
                        {
                            u32 dpix = NoDestRead ? 0U : ops.UnsafeGetValue(dest);  //u32 dpix = NoDestRead ? 0 : *dest;
                            u32 dr = (r + ((dpix & rmask) * inva)) & (rmask << 8);
                            u32 dg = (g + ((dpix & gmask) * inva)) & (gmask << 8);
                            u32 db = (b + ((dpix & bmask) * inva)) & (bmask << 8);
                            ops.UnsafeSetValueAndIncrement(ref dest, (dr | dg | db) >> 8);  //*dest++ = (dr | dg | db) >> 8;
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
#if MCS_UNSAFE
        unsafe static void draw_quad_palette16_none(render_primitive prim, PointerU8 dstdata, u32 pitch, quad_setup_data setup)  //static void draw_quad_palette16_none(render_primitive const &prim, PixelType *dstdata, u32 pitch, quad_setup_data const &setup)
#else
        static void draw_quad_palette16_none(render_primitive prim, PointerU8 dstdata, u32 pitch, quad_setup_data setup)  //static void draw_quad_palette16_none(render_primitive const &prim, PixelType *dstdata, u32 pitch, quad_setup_data const &setup)
#endif
        {
            // ensure all parameters are valid
            assert(prim.texture.palette != null);

            if (prim.color.r >= 1.0f && prim.color.g >= 1.0f && prim.color.b >= 1.0f && is_opaque(prim.color.a))
            {
                // fast case: no coloring, no alpha
#if MCS_UNSAFE
                fixed(u8 * dstdata_u8_zero = dstdata.Buffer.data_raw,
                           texbase_u8_zero = prim.texture.base_.Buffer.data_raw)
                {
                    u8 * dstdata_u8 = dstdata_u8_zero + dstdata.Offset;
                    u16 * texbase_u16 = (u16 *)(texbase_u8_zero + prim.texture.base_.Offset);
#else
                {
                    PointerU8 dstdata_u8 = dstdata;
                    PointerU16 texbase_u16 = null;
#endif

                    // loop over rows
                    for (s32 y = setup.starty; y < setup.endy; y++)
                    {
                        var dest = ops.UnsafeGetPointer(dstdata_u8, y * (int)pitch + setup.startx);  //PixelType *dest = dstdata + y * pitch + setup.startx;
                        s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                        s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_palette16(prim.texture, curu, curv, texbase_u16);
                            ops.UnsafeSetValueAndIncrement(ref dest, source32_to_dest32(pix));  //*dest++ = source32_to_dest(pix);
                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }
                }
            }
            else if (is_opaque(prim.color.a))
            {
                // coloring-only case

                // clamp R,G,B to 0-256 range
                u32 sr = (u32)(std.clamp(256.0f * prim.color.r, 0.0f, 256.0f));
                u32 sg = (u32)(std.clamp(256.0f * prim.color.g, 0.0f, 256.0f));
                u32 sb = (u32)(std.clamp(256.0f * prim.color.b, 0.0f, 256.0f));

#if MCS_UNSAFE
                fixed(u8 * dstdata_u8_zero = dstdata.Buffer.data_raw,
                           texbase_u8_zero = prim.texture.base_.Buffer.data_raw)
                {
                    u8 * dstdata_u8 = dstdata_u8_zero + dstdata.Offset;
                    u16 * texbase_u16 = (u16 *)(texbase_u8_zero + prim.texture.base_.Offset);
#else
                {
                    PointerU8 dstdata_u8 = dstdata;
                    PointerU16 texbase_u16 = null;
#endif

                    // loop over rows
                    for (s32 y = setup.starty; y < setup.endy; y++)
                    {
                        var dest = ops.UnsafeGetPointer(dstdata_u8, y * (int)pitch + setup.startx);  //PixelType *dest = dstdata + y * pitch + setup.startx;
                        s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                        s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_palette16(prim.texture, curu, curv, texbase_u16);
                            u32 r = (source32_r(pix) * sr) >> 8;
                            u32 g = (source32_g(pix) * sg) >> 8;
                            u32 b = (source32_b(pix) * sb) >> 8;

                            ops.UnsafeSetValueAndIncrement(ref dest, dest_assemble_rgb32(r, g, b));  //*dest++ = dest_assemble_rgb(r, g, b);
                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }
                }
            }
            else if (!is_transparent(prim.color.a))
            {
                // alpha and/or coloring case

                // clamp R,G,B and inverse A to 0-256 range
                u32 sr = (u32)(std.clamp(256.0f * prim.color.r * prim.color.a, 0.0f, 256.0f));
                u32 sg = (u32)(std.clamp(256.0f * prim.color.g * prim.color.a, 0.0f, 256.0f));
                u32 sb = (u32)(std.clamp(256.0f * prim.color.b * prim.color.a, 0.0f, 256.0f));
                u32 invsa = (u32)(std.clamp(256.0f * (1.0f - prim.color.a), 0.0f, 256.0f));

#if MCS_UNSAFE
                fixed(u8 * dstdata_u8_zero = dstdata.Buffer.data_raw,
                           texbase_u8_zero = prim.texture.base_.Buffer.data_raw)
                {
                    u8 * dstdata_u8 = dstdata_u8_zero + dstdata.Offset;
                    u16 * texbase_u16 = (u16 *)(texbase_u8_zero + prim.texture.base_.Offset);
#else
                {
                    PointerU8 dstdata_u8 = dstdata;
                    PointerU16 texbase_u16 = null;
#endif

                    // loop over rows
                    for (s32 y = setup.starty; y < setup.endy; y++)
                    {
                        var dest = ops.UnsafeGetPointer(dstdata_u8, y * (int)pitch + setup.startx);  //PixelType *dest = dstdata + y * pitch + setup.startx;
                        s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                        s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_palette16(prim.texture, curu, curv, texbase_u16);
                            u32 dpix = NoDestRead ? 0 : ops.UnsafeGetValue(dest);  //u32 const dpix = NoDestRead ? 0 : *dest;
                            u32 r = (source32_r(pix) * sr + dest_r32(dpix) * invsa) >> 8;
                            u32 g = (source32_g(pix) * sg + dest_g32(dpix) * invsa) >> 8;
                            u32 b = (source32_b(pix) * sb + dest_b32(dpix) * invsa) >> 8;

                            ops.UnsafeSetValueAndIncrement(ref dest, dest_assemble_rgb32(r, g, b));  //*dest++ = dest_assemble_rgb(r, g, b);
                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }
                }
            }
        }

        //-------------------------------------------------
        //  draw_quad_palette16_add - perform
        //  rasterization of a 16bpp palettized texture
        //-------------------------------------------------
        static void draw_quad_palette16_add(render_primitive prim, PointerU8 dstdata, u32 pitch, quad_setup_data setup)  //static void draw_quad_palette16_add(render_primitive const &prim, PixelType *dstdata, u32 pitch, quad_setup_data const &setup)
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
        static void draw_quad_yuy16_none(render_primitive prim, PointerU8 dstdata, u32 pitch, quad_setup_data setup)  //static void draw_quad_yuy16_none(render_primitive const &prim, PixelType *dstdata, u32 pitch, quad_setup_data const &setup)
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  draw_quad_yuy16_add - perform
        //  rasterization by using RGB add after YUY
        //  conversion
        //-------------------------------------------------
        static void draw_quad_yuy16_add(render_primitive prim, PointerU8 dstdata, u32 pitch, quad_setup_data setup)  //static void draw_quad_yuy16_add(render_primitive const &prim, PixelType *dstdata, u32 pitch, quad_setup_data const &setup)
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
        //template <bool Wrap>
        static void draw_quad_rgb32<bool_Wrap>(render_primitive prim, PointerU8 dstdata, u32 pitch, quad_setup_data setup)  //static void draw_quad_rgb32(const render_primitive &prim, _PixelType *dstdata, u32 pitch, const quad_setup_data&setup)
            where bool_Wrap : bool_const, new()
        {
            Pointer<rgb_t> palbase = prim.texture.palette;  //const rgb_t *palbase = prim.texture.palette;

            // fast case: no coloring, no alpha
            if (prim.color.r >= 1.0f && prim.color.g >= 1.0f && prim.color.b >= 1.0f && is_opaque(prim.color.a))
            {
                // loop over rows
                for (s32 y = setup.starty; y < setup.endy; y++)
                {
                    PointerU8 dest = ops.GetPointer(dstdata, y * (int)pitch + setup.startx);  //_PixelType *dest = dstdata + y * pitch + setup.startx;
                    s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                    s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                    if (palbase == null)
                    {
                        // no lookup case

                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_rgb32<bool_Wrap>(prim.texture, curu, curv);
                            ops.SetValueAndIncrement(dest, source32_to_dest32(pix));  //*dest++ = source32_to_dest(pix);
                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }
                    else
                    {
                        // lookup case

                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_rgb32<bool_Wrap>(prim.texture, curu, curv);
                            u32 r = palbase[(int)((pix >> 16) & 0xff)] >> SrcShiftR;
                            u32 g = palbase[(int)((pix >> 8) & 0xff)] >> SrcShiftG;
                            u32 b = palbase[(int)((pix >> 0) & 0xff)] >> SrcShiftB;
                            ops.SetValueAndIncrement(dest, dest_assemble_rgb32(r, g, b));  //*dest++ = dest_assemble_rgb(r, g, b);
                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }
                }
            }
            else if (is_opaque(prim.color.a))
            {
                // coloring-only case

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
                    PointerU8 dest = ops.GetPointer(dstdata, y * (int)pitch + setup.startx);  //_PixelType *dest = dstdata + y * pitch + setup.startx;
                    s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                    s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                    // no lookup case
                    if (palbase == null)
                    {
                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_rgb32<bool_Wrap>(prim.texture, curu, curv);
                            u32 r = (source32_r(pix) * sr) >> 8;
                            u32 g = (source32_g(pix) * sg) >> 8;
                            u32 b = (source32_b(pix) * sb) >> 8;

                            ops.SetValueAndIncrement(dest, dest_assemble_rgb32(r, g, b));  //*dest++ = dest_assemble_rgb(r, g, b);
                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }
                    else
                    {
                        // lookup case

                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_rgb32<bool_Wrap>(prim.texture, curu, curv);
                            u32 r = (palbase[(int)((pix >> 16) & 0xff)] * sr) >> (8 + SrcShiftR);
                            u32 g = (palbase[(int)((pix >> 8) & 0xff)] * sg) >> (8 + SrcShiftG);
                            u32 b = (palbase[(int)((pix >> 0) & 0xff)] * sb) >> (8 + SrcShiftB);

                            ops.SetValueAndIncrement(dest, dest_assemble_rgb32(r, g, b));  //*dest++ = dest_assemble_rgb(r, g, b);
                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }
                }
            }
            else if (!is_transparent(prim.color.a))
            {
                // alpha and/or coloring case

                // clamp R,G,B and inverse A to 0-256 range
                u32 sr = (u32)(std.clamp(256.0f * prim.color.r * prim.color.a, 0.0f, 256.0f));
                u32 sg = (u32)(std.clamp(256.0f * prim.color.g * prim.color.a, 0.0f, 256.0f));
                u32 sb = (u32)(std.clamp(256.0f * prim.color.b * prim.color.a, 0.0f, 256.0f));
                u32 invsa = (u32)(std.clamp(256.0f * (1.0f - prim.color.a), 0.0f, 256.0f));

                // loop over rows
                for (s32 y = setup.starty; y < setup.endy; y++)
                {
                    PointerU8 dest = ops.GetPointer(dstdata, y * (int)pitch + setup.startx);  //_PixelType *dest = dstdata + y * pitch + setup.startx;
                    s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                    s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                    if (palbase == null)
                    {
                        // no lookup case

                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_rgb32<bool_Wrap>(prim.texture, curu, curv);
                            u32 dpix = NoDestRead ? 0 : ops.GetValue(dest);  //u32 dpix = NoDestRead ? 0 : *dest;
                            u32 r = (source32_r(pix) * sr + dest_r32(dpix) * invsa) >> 8;
                            u32 g = (source32_g(pix) * sg + dest_g32(dpix) * invsa) >> 8;
                            u32 b = (source32_b(pix) * sb + dest_b32(dpix) * invsa) >> 8;

                            ops.SetValueAndIncrement(dest, dest_assemble_rgb32(r, g, b));  //*dest++ = dest_assemble_rgb(r, g, b);
                            curu += setup.dudx;
                            curv += setup.dvdx;
                        }
                    }
                    else
                    {
                        // lookup case

                        // loop over cols
                        for (s32 x = setup.startx; x < setup.endx; x++)
                        {
                            u32 pix = get_texel_rgb32<bool_Wrap>(prim.texture, curu, curv);
                            u32 dpix = NoDestRead ? 0 : ops.GetValue(dest);  //u32 dpix = NoDestRead ? 0 : *dest;
                            u32 r = ((palbase[(int)((pix >> 16) & 0xff)] >> SrcShiftR) * sr + dest_r32(dpix) * invsa) >> 8;
                            u32 g = ((palbase[(int)((pix >> 8) & 0xff)] >> SrcShiftG) * sg + dest_g32(dpix) * invsa) >> 8;
                            u32 b = ((palbase[(int)((pix >> 0) & 0xff)] >> SrcShiftB) * sb + dest_b32(dpix) * invsa) >> 8;

                            ops.SetValueAndIncrement(dest, dest_assemble_rgb32(r, g, b));  //*dest++ = dest_assemble_rgb(r, g, b);
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
        //template <bool Wrap>
        static void draw_quad_rgb32_add<bool_Wrap>(render_primitive prim, PointerU8 dstdata, u32 pitch, quad_setup_data setup)  //static void draw_quad_rgb32_add(const render_primitive &prim, _PixelType *dstdata, u32 pitch, const quad_setup_data&setup)
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  draw_quad_rgb32_multiply - perform
        //  rasterization using RGB multiply
        //-------------------------------------------------
        //template <bool Wrap>
        static void draw_quad_rgb32_multiply<bool_Wrap>(render_primitive prim, PointerU8 dstdata, u32 pitch, quad_setup_data setup)  //static void draw_quad_rgb32_multiply(const render_primitive &prim, _PixelType *dstdata, u32 pitch, const quad_setup_data&setup)
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
        //template <bool Wrap>
#if MCS_UNSAFE
        unsafe static void draw_quad_argb32_alpha<bool_Wrap>(render_primitive prim, PointerU8 dstdata, u32 pitch, quad_setup_data setup)  //static void draw_quad_argb32_alpha(render_primitive const &prim, PixelType *dstdata, u32 pitch, quad_setup_data const &setup)
#else
        static void draw_quad_argb32_alpha<bool_Wrap>(render_primitive prim, PointerU8 dstdata, u32 pitch, quad_setup_data setup)  //static void draw_quad_argb32_alpha(render_primitive const &prim, PixelType *dstdata, u32 pitch, quad_setup_data const &setup)
#endif
            where bool_Wrap : bool_const, new()
        {
            Pointer<rgb_t> palbase = prim.texture.palette;  //const rgb_t *palbase = prim.texture.palette;

            if (prim.color.r >= 1.0f && prim.color.g >= 1.0f && prim.color.b >= 1.0f && is_opaque(prim.color.a))
            {
                // fast case: no coloring, no alpha

                bool Wrap = new bool_Wrap().value;

#if MCS_UNSAFE
                fixed(u8 * dstdata_u8_zero = dstdata.Buffer.data_raw)
                {
                    u8 * dstdata_u8 = dstdata_u8_zero + dstdata.Offset;
#else
                {
                    PointerU8 dstdata_u8 = dstdata;
#endif

                    // loop over rows
                    for (s32 y = setup.starty; y < setup.endy; y++)
                    {
                        var dest = ops.UnsafeGetPointer(dstdata_u8, y * (int)pitch + setup.startx);  //PixelType *dest = dstdata + y * pitch + setup.startx;
                        s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                        s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                        if (palbase == null)
                        {
                            // no lookup case

                            // loop over cols
                            for (s32 x = setup.startx; x < setup.endx; x++)
                            {
                                u32 pix = get_texel_argb32(Wrap, prim.texture, curu, curv);
                                u32 ta = pix >> 24;
                                if (ta != 0)
                                {
                                    u32 dpix = NoDestRead ? 0 : ops.UnsafeGetValue(dest);  //u32 dpix = NoDestRead ? 0 : *dest;
                                    u32 invta = 0x100 - ta;
                                    u32 r = (source32_r(pix) * ta + dest_r32(dpix) * invta) >> 8;
                                    u32 g = (source32_g(pix) * ta + dest_g32(dpix) * invta) >> 8;
                                    u32 b = (source32_b(pix) * ta + dest_b32(dpix) * invta) >> 8;
                                    ops.UnsafeSetValue(dest, dest_assemble_rgb32(r, g, b));  //*dest = dest_assemble_rgb(r, g, b);
                                }

                                ops.UnsafeIncrement(ref dest);  //dest++;
                                curu += setup.dudx;
                                curv += setup.dvdx;
                            }
                        }
                        else
                        {
                            // lookup case

                            // loop over cols
                            for (s32 x = setup.startx; x < setup.endx; x++)
                            {
                                u32 pix = get_texel_argb32(Wrap, prim.texture, curu, curv);
                                u32 ta = pix >> 24;
                                if (ta != 0)
                                {
                                    u32 dpix = NoDestRead ? 0 : ops.UnsafeGetValue(dest);  //u32 dpix = NoDestRead ? 0 : *dest;
                                    u32 invta = 0x100 - ta;
                                    u32 r = ((palbase[(int)((pix >> 16) & 0xff)] >> SrcShiftR) * ta + dest_r32(dpix) * invta) >> 8;
                                    u32 g = ((palbase[(int)((pix >> 8) & 0xff)] >> SrcShiftG) * ta + dest_g32(dpix) * invta) >> 8;
                                    u32 b = ((palbase[(int)((pix >> 0) & 0xff)] >> SrcShiftB) * ta + dest_b32(dpix) * invta) >> 8;

                                    ops.UnsafeSetValue(dest, dest_assemble_rgb32(r, g, b));  //*dest = dest_assemble_rgb(r, g, b);
                                }

                                ops.UnsafeIncrement(ref dest);  //dest++;
                                curu += setup.dudx;
                                curv += setup.dvdx;
                            }
                        }
                    }
                }
            }
            else
            {
                // alpha and/or coloring case

                bool Wrap = new bool_Wrap().value;

#if MCS_UNSAFE
                fixed(u8 * dstdata_u8_zero = dstdata.Buffer.data_raw)
                {
                    u8 * dstdata_u8 = dstdata_u8_zero + dstdata.Offset;
#else
                {
                    PointerU8 dstdata_u8 = dstdata;
#endif

                    // clamp R,G,B and inverse A to 0-256 range
                    u32 sr = (u32)(std.clamp(256.0f * prim.color.r, 0.0f, 256.0f));
                    u32 sg = (u32)(std.clamp(256.0f * prim.color.g, 0.0f, 256.0f));
                    u32 sb = (u32)(std.clamp(256.0f * prim.color.b, 0.0f, 256.0f));
                    u32 sa = (u32)(std.clamp(256.0f * prim.color.a, 0.0f, 256.0f));

                    // loop over rows
                    for (s32 y = setup.starty; y < setup.endy; y++)
                    {
                        var dest = ops.UnsafeGetPointer(dstdata_u8, y * (int)pitch + setup.startx);  //PixelType *dest = dstdata + y * pitch + setup.startx;
                        s32 curu = setup.startu + (y - setup.starty) * setup.dudy;
                        s32 curv = setup.startv + (y - setup.starty) * setup.dvdy;

                        if (palbase == null)
                        {
                            // no lookup case

                            // loop over cols
                            for (s32 x = setup.startx; x < setup.endx; x++)
                            {
                                u32 pix = get_texel_argb32(Wrap, prim.texture, curu, curv);
                                u32 ta = (pix >> 24) * sa;
                                if (ta != 0)
                                {
                                    u32 dpix = NoDestRead ? 0 : ops.UnsafeGetValue(dest);  //u32 dpix = NoDestRead ? 0 : *dest;
                                    u32 invsta = (0x10000 - ta) << 8;
                                    u32 r = (source32_r(pix) * sr * ta + dest_r32(dpix) * invsta) >> 24;
                                    u32 g = (source32_g(pix) * sg * ta + dest_g32(dpix) * invsta) >> 24;
                                    u32 b = (source32_b(pix) * sb * ta + dest_b32(dpix) * invsta) >> 24;

                                    ops.UnsafeSetValue(dest, dest_assemble_rgb32(r, g, b));  //*dest = dest_assemble_rgb(r, g, b);
                                }

                                ops.UnsafeIncrement(ref dest);  //dest++;
                                curu += setup.dudx;
                                curv += setup.dvdx;
                            }
                        }
                        else
                        {
                            // lookup case

                            // loop over cols
                            for (s32 x = setup.startx; x < setup.endx; x++)
                            {
                                u32 pix = get_texel_argb32(Wrap, prim.texture, curu, curv);
                                u32 ta = (pix >> 24) * sa;
                                if (ta != 0)
                                {
                                    u32 dpix = NoDestRead ? 0 : ops.UnsafeGetValue(dest);  //u32 dpix = NoDestRead ? 0 : *dest;
                                    u32 invsta = (0x10000 - ta) << 8;
                                    u32 r = ((palbase[(int)((pix >> 16) & 0xff)] >> SrcShiftR) * sr * ta + dest_r32(dpix) * invsta) >> 24;
                                    u32 g = ((palbase[(int)((pix >> 8) & 0xff)] >> SrcShiftG) * sg * ta + dest_g32(dpix) * invsta) >> 24;
                                    u32 b = ((palbase[(int)((pix >> 0) & 0xff)] >> SrcShiftB) * sb * ta + dest_b32(dpix) * invsta) >> 24;

                                    ops.UnsafeSetValue(dest, dest_assemble_rgb32(r, g, b));  //*dest = dest_assemble_rgb(r, g, b);
                                }

                                ops.UnsafeIncrement(ref dest);  //dest++;
                                curu += setup.dudx;
                                curv += setup.dvdx;
                            }
                        }
                    }
                }
            }
        }


        //-------------------------------------------------
        //  draw_quad_argb32_add - perform
        //  rasterization by using RGB add
        //-------------------------------------------------
        //template <bool Wrap>
        static void draw_quad_argb32_add<bool_Wrap>(render_primitive prim, PointerU8 dstdata, u32 pitch, quad_setup_data setup)  //static void draw_quad_argb32_add(const render_primitive &prim, _PixelType *dstdata, u32 pitch, const quad_setup_data&setup)
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
        static void setup_and_draw_textured_quad(render_primitive prim, PointerU8 dstdata, s32 width, s32 height, u32 pitch)  //static void setup_and_draw_textured_quad(render_primitive const &prim, PixelType *dstdata, s32 width, s32 height, u32 pitch)
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
            if (BilinearFilter)
            {
                setup.startu -= 0x8000;
                setup.startv -= 0x8000;
            }

            // render based on the texture coordinates
            u32 primflags = prim.flags & (PRIMFLAG_TEXFORMAT_MASK | PRIMFLAG_BLENDMODE_MASK);
            //switch (prim.flags & (PRIMFLAG_TEXFORMAT_MASK | PRIMFLAG_BLENDMODE_MASK))
            {
                if (primflags == (PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_PALETTE16) | PRIMFLAG_BLENDMODE(BLENDMODE_NONE)) ||
                    primflags == (PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_PALETTE16) | PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA)))
                    draw_quad_palette16_none(prim, dstdata, pitch, setup);

                else if (primflags == (PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_PALETTE16) | PRIMFLAG_BLENDMODE(BLENDMODE_ADD)))
                    draw_quad_palette16_add(prim, dstdata, pitch, setup);

                else if (primflags == (PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_YUY16) | PRIMFLAG_BLENDMODE(BLENDMODE_NONE)) ||
                         primflags == (PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_YUY16) | PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA)))
                    draw_quad_yuy16_none(prim, dstdata, pitch, setup);

                else if (primflags == (PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_YUY16) | PRIMFLAG_BLENDMODE(BLENDMODE_ADD)))
                    draw_quad_yuy16_add(prim, dstdata, pitch, setup);

                else if (primflags == (PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_RGB32) | PRIMFLAG_BLENDMODE(BLENDMODE_NONE)) ||
                         primflags == (PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_RGB32) | PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA)) ||
                         primflags == (PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_ARGB32) | PRIMFLAG_BLENDMODE(BLENDMODE_NONE)))
                    if (PRIMFLAG_GET_TEXWRAP(prim.flags))
                        draw_quad_rgb32<bool_const_true>(prim, dstdata, pitch, setup);
                    else
                        draw_quad_rgb32<bool_const_false>(prim, dstdata, pitch, setup);

                else if (primflags == (PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_RGB32) | PRIMFLAG_BLENDMODE(BLENDMODE_RGB_MULTIPLY)) ||
                         primflags == (PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_ARGB32) | PRIMFLAG_BLENDMODE(BLENDMODE_RGB_MULTIPLY)))
                    if (PRIMFLAG_GET_TEXWRAP(prim.flags))
                        draw_quad_rgb32_multiply<bool_const_true>(prim, dstdata, pitch, setup);
                    else
                        draw_quad_rgb32_multiply<bool_const_false>(prim, dstdata, pitch, setup);

                else if (primflags == (PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_RGB32) | PRIMFLAG_BLENDMODE(BLENDMODE_ADD)))
                    if (PRIMFLAG_GET_TEXWRAP(prim.flags))
                        draw_quad_rgb32_add<bool_const_true>(prim, dstdata, pitch, setup);
                    else
                        draw_quad_rgb32_add<bool_const_false>(prim, dstdata, pitch, setup);

                else if (primflags == (PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_ARGB32) | PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA)))
                    if (PRIMFLAG_GET_TEXWRAP(prim.flags))
                        draw_quad_argb32_alpha<bool_const_true>(prim, dstdata, pitch, setup);
                    else
                        draw_quad_argb32_alpha<bool_const_false>(prim, dstdata, pitch, setup);

                else if (primflags == (PRIMFLAG_TEXFORMAT((u32)texture_format.TEXFORMAT_ARGB32) | PRIMFLAG_BLENDMODE(BLENDMODE_ADD)))
                    if (PRIMFLAG_GET_TEXWRAP(prim.flags))
                        draw_quad_argb32_add<bool_const_true>(prim, dstdata, pitch, setup);
                    else
                        draw_quad_argb32_add<bool_const_false>(prim, dstdata, pitch, setup);

                else
                    fatalerror("Unknown texformat({0})/blendmode({1}) combo\n", PRIMFLAG_GET_TEXFORMAT(prim.flags), PRIMFLAG_GET_BLENDMODE(prim.flags));
            }
        }


        //**************************************************************************
        //  PRIMARY ENTRY POINT
        //**************************************************************************

        //-------------------------------------------------
        //  draw_primitives - draw a series of primitives
        //  using a software rasterizer
        //-------------------------------------------------
        public static void draw_primitives(render_primitive_list primlist, PointerU8 dstdata, u32 width, u32 height, u32 pitch)  //static void draw_primitives(render_primitive_list const &primlist, void *dstdata, u32 width, u32 height, u32 pitch)
        {
            // loop over the list and render each element
            for (render_primitive prim = primlist.first(); prim != null; prim = prim.next())
            {
                switch (prim.type)
                {
                    case render_primitive.primitive_type.LINE:
                        draw_line(prim, dstdata, (int)width, (int)height, pitch);  //draw_line(*prim, reinterpret_cast<_PixelType *>(dstdata), width, height, pitch);
                        break;

                    case render_primitive.primitive_type.QUAD:
                        if (prim.texture.base_ == null)
                            draw_rect(prim, dstdata, (int)width, (int)height, pitch);  //draw_rect(prim, reinterpret_cast<_PixelType *>(dstdata), width, height, pitch);
                        else
                            setup_and_draw_textured_quad(prim, dstdata, (int)width, (int)height, pitch);  //setup_and_draw_textured_quad(*prim, reinterpret_cast<_PixelType *>(dstdata), width, height, pitch);
                        break;

                    default:
                        throw new emu_fatalerror("Unexpected render_primitive type\n");
                }
            }
        }
    }
}
