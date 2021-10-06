// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using bitmap8_t = mame.bitmap_specific<System.Byte, mame.PixelType_operators_u8, mame.Pointer<System.Byte>>;  //using bitmap8_t = bitmap_specific<uint8_t>;
using bitmap16_t = mame.bitmap_specific<System.UInt16, mame.PixelType_operators_u16, mame.PointerU16>;  //using bitmap16_t = bitmap_specific<uint16_t>;
using bitmap32_t = mame.bitmap_specific<System.UInt32, mame.PixelType_operators_u32, mame.PointerU32>;  //using bitmap16_t = bitmap_specific<uint16_t>;
using bitmap64_t = mame.bitmap_specific<System.UInt64, mame.PixelType_operators_u64, mame.PointerU64>;  //using bitmap16_t = bitmap_specific<uint16_t>;
using int32_t = System.Int32;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using PointerU8 = mame.Pointer<System.Byte>;
using size_t = System.UInt64;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame
{
    // bitmap_format describes the various bitmap formats we use
    public enum bitmap_format
    {
        BITMAP_FORMAT_INVALID = 0,      // invalid forma
        BITMAP_FORMAT_IND8,             // 8bpp indexed
        BITMAP_FORMAT_IND16,            // 16bpp indexed
        BITMAP_FORMAT_IND32,            // 32bpp indexed
        BITMAP_FORMAT_IND64,            // 64bpp indexed
        BITMAP_FORMAT_RGB32,            // 32bpp 8-8-8 RGB
        BITMAP_FORMAT_ARGB32,           // 32bpp 8-8-8-8 ARGB
        BITMAP_FORMAT_YUY16             // 16bpp 8-8 Y/Cb, Y/Cr in sequence
    }


    // ======================> rectangle
    // rectangles describe a bitmap portion
    public struct rectangle
    {
        // internal state
        public int32_t min_x;  // = 0;  // minimum X, or left coordinate
        public int32_t max_x;  // = 0;  // maximum X, or right coordinate (inclusive)
        public int32_t min_y;  // = 0;  // minimum Y, or top coordinate
        public int32_t max_y;  // = 0;  // maximum Y, or bottom coordinate (inclusive)


        // construction/destruction
        public rectangle(int minx, int maxx, int miny, int maxy) { min_x = minx; max_x = maxx; min_y = miny; max_y = maxy; }
        public rectangle(rectangle rect) : this(rect.min_x, rect.max_x, rect.min_y, rect.max_y) { }


        // getters
        public int32_t left() { return min_x; }
        public int32_t right() { return max_x; }
        public int32_t top() { return min_y; }
        public int32_t bottom() { return max_y; }


        // compute intersection with another rect
        public static rectangle operator&(rectangle left, rectangle right)  //rectangle &operator&=(const rectangle &src)
        {
            rectangle ret = left;
            if (right.min_x > ret.min_x) ret.min_x = right.min_x;
            if (right.max_x < ret.max_x) ret.max_x = right.max_x;
            if (right.min_y > ret.min_y) ret.min_y = right.min_y;
            if (right.max_y < ret.max_y) ret.max_y = right.max_y;
            return ret;
        }

        // compute union with another rect
        public static rectangle operator|(rectangle left, rectangle right)  //rectangle &operator|=(const rectangle &src)
        {
            rectangle ret = left;
            if (right.min_x < ret.min_x) ret.min_x = right.min_x;
            if (right.max_x > ret.max_x) ret.max_x = right.max_x;
            if (right.min_y < ret.min_y) ret.min_y = right.min_y;
            if (right.max_y > ret.max_y) ret.max_y = right.max_y;
            return ret;
        }


        // comparisons
        public static bool operator== (rectangle lhs, rectangle rhs) { return lhs.min_x == rhs.min_x && lhs.max_x == rhs.max_x && lhs.min_y == rhs.min_y && lhs.max_y == rhs.max_y; }
        public static bool operator!= (rectangle lhs, rectangle rhs) { return lhs.min_x != rhs.min_x || lhs.max_x != rhs.max_x || lhs.min_y != rhs.min_y || lhs.max_y != rhs.max_y; }
        //bool operator>(const rectangle &rhs) const { return min_x < rhs.min_x && min_y < rhs.min_y && max_x > rhs.max_x && max_y > rhs.max_y; }
        //bool operator>=(const rectangle &rhs) const { return min_x <= rhs.min_x && min_y <= rhs.min_y && max_x >= rhs.max_x && max_y >= rhs.max_y; }
        //bool operator<(const rectangle &rhs) const { return min_x >= rhs.min_x || min_y >= rhs.min_y || max_x <= rhs.max_x || max_y <= rhs.max_y; }
        //bool operator<=(const rectangle &rhs) const { return min_x > rhs.min_x || min_y > rhs.min_y || max_x < rhs.max_x || max_y < rhs.max_y; }

        public override bool Equals(Object obj)
        {
            if (obj == null || base.GetType() != obj.GetType()) return false;
            return this == (rectangle)obj;
        }
        public override int GetHashCode() { return min_x.GetHashCode() ^ max_x.GetHashCode() ^ min_y.GetHashCode() ^ max_y.GetHashCode(); }


        // other helpers
        public bool empty() { return min_x > max_x || min_y > max_y; }
        public bool contains(int32_t x, int32_t y) { return x >= min_x && x <= max_x && y >= min_y && y <= max_y; }
        public bool contains(rectangle rect) { return min_x <= rect.min_x && max_x >= rect.max_x && min_y <= rect.min_y && max_y >= rect.max_y; }
        public int32_t width() { return max_x + 1 - min_x; }
        public int32_t height() { return max_y + 1 - min_y; }
        public int32_t xcenter() { return (min_x + max_x + 1) / 2; }
        public int32_t ycenter() { return (min_y + max_y + 1) / 2; }


        // setters
        public void set(int32_t minx, int32_t maxx, int32_t miny, int32_t maxy) { min_x = minx; max_x = maxx; min_y = miny; max_y = maxy; }
        public void setx(int32_t minx, int32_t maxx) { min_x = minx; max_x = maxx; }
        public void sety(int32_t miny, int32_t maxy) { min_y = miny; max_y = maxy; }
        public void set_width(int32_t width) { max_x = min_x + width - 1; }
        public void set_height(int32_t height) { max_y = min_y + height - 1; }
        void set_origin(int32_t x, int32_t y) { max_x += x - min_x; max_y += y - min_y; min_x = x; min_y = y; }
        void set_size(int32_t width, int32_t height) { set_width(width); set_height(height); }


        // offset helpers
        public void offset(int32_t xdelta, int32_t ydelta) { min_x += xdelta; max_x += xdelta; min_y += ydelta; max_y += ydelta; }
        void offsetx(int32_t delta) { min_x += delta; max_x += delta; }
        void offsety(int32_t delta) { min_y += delta; max_y += delta; }
    }


    public interface PixelType_operators { PointerU8 new_pointer(PointerU8 memory, int offset = 0); }
    public class PixelType_operators_u8 : PixelType_operators { public PointerU8 new_pointer(PointerU8 memory, int offset = 0) { return new PointerU8(memory, offset); } }
    public class PixelType_operators_u16 : PixelType_operators { public PointerU8 new_pointer(PointerU8 memory, int offset = 0) { return new PointerU16(memory, offset); } }
    public class PixelType_operators_u32 : PixelType_operators { public PointerU8 new_pointer(PointerU8 memory, int offset = 0) { return new PointerU32(memory, offset); } }
    public class PixelType_operators_u64 : PixelType_operators { public PointerU8 new_pointer(PointerU8 memory, int offset = 0) { return new PointerU64(memory, offset); } }


    // ======================> bitmap_t
    // bitmaps describe a rectangular array of pixels
    public class bitmap_t
    {
        // internal state
        MemoryU8 m_alloc;  //std::unique_ptr<uint8_t []> m_alloc;        // pointer to allocated pixel memory
        uint32_t m_allocbytes;   // size of our allocation
        PointerU8 m_base;  //void *          m_base;         // pointer to pixel (0,0) (adjusted for padding)
        int32_t m_rowpixels;    // pixels per row (including padding)
        int32_t m_width;        // width of the bitmap
        int32_t m_height;       // height of the bitmap
        bitmap_format m_format;       // format of the bitmap
        uint8_t m_bpp;          // bits per pixel
        palette_t m_palette;      // optional palette
        rectangle m_cliprect;     // a clipping rectangle covering the full bitmap


        // construction/destruction -- subclasses only to ensure type correctness

        //bitmap_t(const bitmap_t &) = delete;
        //bitmap_t(bitmap_t &&that);

        /**
         * @fn  bitmap_t::bitmap_t(bitmap_format format, int bpp, int width, int height, int xslop, int yslop)
         *
         * @brief   -------------------------------------------------
         *            bitmap_t - basic constructor
         *          -------------------------------------------------.
         *
         * @param   format  Describes the format to use.
         * @param   bpp     The bits per pixel.
         * @param   width   The width.
         * @param   height  The height.
         * @param   xslop   The xslop.
         * @param   yslop   The yslop.
         */
        public bitmap_t(bitmap_format format, uint8_t bpp, int width = 0, int height = 0, int xslop = 0, int yslop = 0)
        {
            m_alloc = null;
            m_allocbytes = 0;
            m_format = format;
            m_bpp = (byte)bpp;
            m_palette = null;


            g.assert(valid_format());

            // allocate intializes all other fields
            allocate(width, height, xslop, yslop);
        }


        /**
         * @fn  bitmap_t::bitmap_t(bitmap_format format, uint8_t bpp, void *base, int width, int height, int rowpixels)
         *
         * @brief   Constructor.
         *
         * @param   format          Describes the format to use.
         * @param   bpp             The bits per pixel.
         * @param [in,out]  base    If non-null, the base.
         * @param   width           The width.
         * @param   height          The height.
         * @param   rowpixels       The rowpixels.
         */
        public bitmap_t(bitmap_format format, uint8_t bpp, PointerU8 base_, int width, int height, int rowpixels)  //bitmap_t::bitmap_t(bitmap_format format, uint8_t bpp, void *base, int width, int height, int rowpixels)
        {
            m_alloc = null;
            m_allocbytes = 0;
            m_base = base_;
            m_rowpixels = rowpixels;
            m_width = width;
            m_height = height;
            m_format = format;
            m_bpp = bpp;
            m_palette = null;
            m_cliprect = new rectangle(0, width - 1, 0, height - 1);


            g.assert(valid_format());
        }


        /**
         * @fn  bitmap_t::bitmap_t(bitmap_format format, int bpp, bitmap_t &source, const rectangle &subrect)
         *
         * @brief   Constructor.
         *
         * @param   format          Describes the format to use.
         * @param   bpp             The bits per pixel.
         * @param [in,out]  source  Source for the.
         * @param   subrect         The subrect.
         */
        public bitmap_t(bitmap_format format, uint8_t bpp, bitmap_t source, rectangle subrect)
        {
            m_alloc = null;
            m_allocbytes = 0;
            m_base = source.raw_pixptr(subrect.top(), subrect.left());  //m_base(source.raw_pixptr(subrect.top(), subrect.left()))
            m_rowpixels = source.m_rowpixels;
            m_width = subrect.width();
            m_height = subrect.height();
            m_format = format;
            m_bpp = (byte)bpp;
            m_palette = null;
            m_cliprect = new rectangle(0, subrect.width() - 1, 0, subrect.height() - 1);


            g.assert(format == source.m_format);
            g.assert(bpp == source.m_bpp);
            g.assert(source.cliprect().contains(subrect));
        }

        //~bitmap_t()
        //{
        //    // delete any existing stuff
        //    reset();
        //}


        // allocation/deallocation
        /**
         * @fn  void bitmap_t::reset()
         *
         * @brief   -------------------------------------------------
         *            reset -- reset to an invalid bitmap, deleting all allocated stuff
         *          -------------------------------------------------.
         */
        public void reset()
        {
            // delete any existing stuff
            set_palette(null);
            //delete[] m_alloc;
            m_alloc = null;
            m_base = null;

            // reset all fields
            m_rowpixels = 0;
            m_width = 0;
            m_height = 0;
            m_cliprect.set(0, -1, 0, -1);
        }


        // getters
        public int32_t width() { return m_width; }
        public int32_t height() { return m_height; }
        public int32_t rowpixels() { return m_rowpixels; }
        int32_t rowbytes() { return m_rowpixels * m_bpp / 8; }
        public uint8_t bpp() { return m_bpp; }
        bitmap_format format() { return m_format; }
        public bool valid() { return m_base != null; }
        public palette_t palette() { return m_palette; }
        public rectangle cliprect() { return m_cliprect; }


        // allocation/sizing

        /**
         * @fn  void bitmap_t::allocate(int width, int height, int xslop, int yslop)
         *
         * @brief   -------------------------------------------------
         *            allocate -- (re)allocate memory for the bitmap at the given size, destroying
         *            anything that already exists
         *          -------------------------------------------------.
         *
         * @param   width   The width.
         * @param   height  The height.
         * @param   xslop   The xslop.
         * @param   yslop   The yslop.
         */
        public void allocate(int width, int height, int xslop = 0, int yslop = 0)
        {
            g.assert(m_format != bitmap_format.BITMAP_FORMAT_INVALID);
            g.assert(m_bpp == 8 || m_bpp == 16 || m_bpp == 32 || m_bpp == 64);

            // delete any existing stuff
            reset();

            // handle empty requests cleanly
            if (width <= 0 || height <= 0)
                return;

            // initialize fields
            m_rowpixels = compute_rowpixels(width, xslop);
            m_width = width;
            m_height = height;
            m_cliprect.set(0, width - 1, 0, height - 1);

            // allocate memory for the bitmap itself
            m_allocbytes = (uint32_t)(m_rowpixels * (m_height + 2 * yslop) * m_bpp / 8);
            m_alloc = new MemoryU8((int)m_allocbytes, true);  //m_alloc = new byte[m_allocbytes];

            // clear to 0 by default
            std.memset(m_alloc, (uint8_t)0, m_allocbytes);

            // compute the base
            compute_base(xslop, yslop);
        }


        /**
         * @fn  void bitmap_t::resize(int width, int height, int xslop, int yslop)
         *
         * @brief   -------------------------------------------------
         *            resize -- resize a bitmap, reusing existing memory if the new size is smaller than
         *            the current size
         *          -------------------------------------------------.
         *
         * @param   width   The width.
         * @param   height  The height.
         * @param   xslop   The xslop.
         * @param   yslop   The yslop.
         */
        public void resize(int width, int height, int xslop = 0, int yslop = 0)
        {
            //assert(m_format != BITMAP_FORMAT_INVALID);
            //assert(m_bpp == 8 || m_bpp == 16 || m_bpp == 32 || m_bpp == 64);

            // handle empty requests cleanly
            if (width <= 0 || height <= 0)
                width = height = 0;

            // determine how much memory we need for the new bitmap
            int new_rowpixels = compute_rowpixels(width, xslop);
            uint32_t new_allocbytes = (uint32_t)(new_rowpixels * (height + 2 * yslop) * m_bpp / 8);

            if (new_allocbytes > m_allocbytes)
            {
                // if we need more memory, just realloc
                palette_t palette = m_palette;
                allocate(width, height, xslop, yslop);
                set_palette(palette);
            }
            else
            {
                // otherwise, reconfigure
                m_rowpixels = new_rowpixels;
                m_width = width;
                m_height = height;
                m_cliprect.set(0, width - 1, 0, height - 1);

                // re-compute the base
                compute_base(xslop, yslop);
            }
        }


        // operations

        /**
         * @fn  void bitmap_t::set_palette(palette_t *palette)
         *
         * @brief   -------------------------------------------------
         *            set_palette -- associate a palette with a bitmap
         *          -------------------------------------------------.
         *
         * @param [in,out]  palette If non-null, the palette.
         */
        public void set_palette(palette_t palette)
        {
            // first dereference any existing palette
            if (m_palette != null)
            {
                m_palette.deref();
                m_palette = null;
            }

            // then reference any new palette
            if (palette != null)
            {
                palette.ref_();
                m_palette = palette;
            }
        }

        public void fill(uint64_t color) { fill(color, m_cliprect); }

        /**
         * @fn  void bitmap_t::fill(UINT32 color, const rectangle &cliprect)
         *
         * @brief   -------------------------------------------------
         *            fill -- fill a bitmap with a solid color
         *          -------------------------------------------------.
         *
         * @param   color       The color.
         * @param   cliprect    The cliprect.
         */
        public void fill(uint64_t color, rectangle bounds)
        {
            // if we have a cliprect, intersect with that
            rectangle fill = bounds;
            fill &= m_cliprect;
            if (!fill.empty())
            {
                // based on the bpp go from there
                switch (m_bpp)
                {
                case 8:
                    for (int32_t y = fill.top(); y <= fill.bottom(); y++)
                        std.fill_n(pixt<uint8_t, PixelType_operators_u8>(y, fill.left()), (size_t)fill.width(), (uint8_t)color);  //std::fill_n(&pixt<uint8_t>(y, fill.left()), fill.width(), uint8_t(color));
                    break;

                case 16:
                    for (int32_t y = fill.top(); y <= fill.bottom(); ++y)
                        std.fill_n((PointerU16)pixt<uint16_t, PixelType_operators_u16>(y, fill.left()), (size_t)fill.width(), (uint16_t)color);  //std::fill_n(&pixt<uint16_t>(y, fill.left()), fill.width(), uint16_t(color));
                    break;

                case 32:
                    for (int32_t y = fill.top(); y <= fill.bottom(); ++y)
                        std.fill_n((PointerU32)pixt<uint32_t, PixelType_operators_u32>(y, fill.left()), (size_t)fill.width(), (uint32_t)color);  //std::fill_n(&pixt<uint32_t>(y, fill.left()), fill.width(), uint32_t(color));
                    break;

                case 64:
                    for (int32_t y = fill.top(); y <= fill.bottom(); ++y)
                        std.fill_n((PointerU64)pixt<uint64_t, PixelType_operators_u64>(y, fill.left()), (size_t)fill.width(), (uint64_t)color);  //std::fill_n(&pixt<uint64_t>(y, fill.left()), fill.width(), uint64_t(color));
                    break;
                }
            }
        }


        void plot_box(int32_t x, int32_t y, int32_t width, int32_t height, uint64_t color)
        {
            fill(color, new rectangle(x, x + width - 1, y, y + height - 1));
        }


        // pixel access

        public PointerU8 raw_pixptr(int32_t y, int32_t x = 0) { return new PointerU8(m_base, (y * m_rowpixels + x) * m_bpp / 8); }  //void *raw_pixptr(int32_t y, int32_t x = 0) { return reinterpret_cast<uint8_t *>(m_base) + (y * m_rowpixels + x) * m_bpp / 8; }


        // for use by subclasses only to ensure type correctness
        protected PointerU8 pixt<PixelType, PixelType_OPS>(int32_t y, int32_t x = 0)  //template <typename PixelType> PixelType &pixt(int32_t y, int32_t x = 0) { return *(reinterpret_cast<PixelType *>(m_base) + y * m_rowpixels + x); }
            where PixelType_OPS : PixelType_operators, new()
        {
            PixelType_OPS ops = new PixelType_OPS();
            return ops.new_pointer(m_base, y * m_rowpixels + x);
        }


        //void wrap(void *base, int width, int height, int rowpixels);
        //void wrap(bitmap_t &source, const rectangle &subrect);


        // internal helpers

        //-------------------------------------------------
        //  compute_rowpixels - compute a rowpixels value
        //-------------------------------------------------
        int32_t compute_rowpixels(int width, int xslop)
        {
            return width + 2 * xslop;
        }

        //-------------------------------------------------
        //  compute_base - compute a bitmap base address
        //  with the given slop values
        //-------------------------------------------------
        void compute_base(int xslop, int yslop)
        {
            m_base = new PointerU8(m_alloc, (m_rowpixels * yslop + xslop) * (m_bpp / 8));  // m_base = m_alloc + (m_rowpixels * yslop + xslop) * (m_bpp / 8);
        }


        //-------------------------------------------------
        //  valid_format - return true if the bitmap format
        //  is valid and agrees with the BPP
        //-------------------------------------------------
        bool valid_format()
        {
            switch (m_format)
            {
            // invalid format
            case bitmap_format.BITMAP_FORMAT_INVALID:
                return false;

            // 8bpp formats
            case bitmap_format.BITMAP_FORMAT_IND8:
                return m_bpp == 8;

            // 16bpp formats
            case bitmap_format.BITMAP_FORMAT_IND16:
            case bitmap_format.BITMAP_FORMAT_YUY16:
                return m_bpp == 16;

            // 32bpp formats
            case bitmap_format.BITMAP_FORMAT_IND32:
            case bitmap_format.BITMAP_FORMAT_RGB32:
            case bitmap_format.BITMAP_FORMAT_ARGB32:
                return m_bpp == 32;

            // 64bpp formats
            case bitmap_format.BITMAP_FORMAT_IND64:
                return m_bpp == 64;
            }

            return false;
        }
    }


    // ======================> bitmap_specific, bitmap8_t, bitmap16_t, bitmap32_t, bitmap64_t

    //template<typename PixelType>
    public class bitmap_specific<PixelType, PixelType_OPS, PixelTypePointer> : bitmap_t
        where PixelType_OPS : PixelType_operators, new()
        where PixelTypePointer : PointerU8
    {
        static readonly int PIXEL_BITS = 8 * g.sizeof_(typeof(PixelType));  //static constexpr int PixelBits = 8 * sizeof(PixelType);


        // construction/destruction -- subclasses only
        //bitmap_specific(bitmap_specific<PixelType> &&) = default;
        protected bitmap_specific(bitmap_format format, int width = 0, int height = 0, int xslop = 0, int yslop = 0) : base(format, (uint8_t)PIXEL_BITS, width, height, xslop, yslop) { }  //bitmap_specific(bitmap_format format, int width = 0, int height = 0, int xslop = 0, int yslop = 0) : bitmap_t(format, PIXEL_BITS, width, height, xslop, yslop) { }
        protected bitmap_specific(bitmap_format format, PixelTypePointer base_, int width, int height, int rowpixels) : base(format, (uint8_t)PIXEL_BITS, base_, width, height, rowpixels) { }  //bitmap_specific(bitmap_format format, PixelType *base, int width, int height, int rowpixels) : bitmap_t(format, PIXEL_BITS, base, width, height, rowpixels) { }
        protected bitmap_specific(bitmap_format format, bitmap_specific<PixelType, PixelType_OPS, PixelTypePointer> source, rectangle subrect) : base(format, (uint8_t)PIXEL_BITS, source, subrect) { }  //bitmap_specific(bitmap_format format, bitmap_specific<PixelType> &source, const rectangle &subrect) : bitmap_t(format, PIXEL_BITS, source, subrect) { }


        //bitmap_specific<PixelType> &operator=(bitmap_specific<PixelType> &&) = default;


        //using pixel_t = PixelType;


        // getters
        public new uint8_t bpp() { return (uint8_t)PIXEL_BITS; }


        // pixel accessors
        public PixelTypePointer pix(int32_t y, int32_t x = 0) { return (PixelTypePointer)pixt<PixelType, PixelType_OPS>(y, x); }  //PixelType &pix(int32_t y, int32_t x = 0) { return pixt<PixelType>(y, x); }

        public PointerU8 pix8(int32_t y, int32_t x = 0) { g.static_assert(PIXEL_BITS == 8, "must be 8bpp"); return pixt<PixelType, PixelType_OPS>(y, x); }  //PixelType &pix8(int32_t y, int32_t x = 0) const { static_assert(PixelBits == 8, "must be 8bpp"); return pixt<PixelType>(y, x); }
        public PointerU16 pix16(int32_t y, int32_t x = 0) { g.static_assert(PIXEL_BITS == 16, "must be 16bpp"); return (PointerU16)pixt<PixelType, PixelType_OPS>(y, x); }  //PixelType &pix16(int32_t y, int32_t x = 0) const { static_assert(PixelBits == 16, "must be 16bpp"); return pixt<PixelType>(y, x); }
        public PointerU32 pix32(int32_t y, int32_t x = 0) { g.static_assert(PIXEL_BITS == 32, "must be 32bpp"); return (PointerU32)pixt<PixelType, PixelType_OPS>(y, x); }  //PixelType &pix32(int32_t y, int32_t x = 0) const { static_assert(PixelBits == 32, "must be 32bpp"); return pixt<PixelType>(y, x); }
        public PointerU64 pix64(int32_t y, int32_t x = 0) { g.static_assert(PIXEL_BITS == 64, "must be 64bpp"); return (PointerU64)pixt<PixelType, PixelType_OPS>(y, x); }  //PixelType &pix64(int32_t y, int32_t x = 0) const { static_assert(PixelBits == 64, "must be 64bpp"); return pixt<PixelType>(y, x); }


        // operations
        void fill(PixelType color) { fill(color, cliprect()); }
        void fill(PixelType color, rectangle bounds)
        {
            throw new emu_unimplemented();
        }


        void plot_box(int32_t x, int32_t y, int32_t width, int32_t height, PixelType color)
        {
            fill(color, new rectangle(x, x + width - 1, y, y + height - 1));
        }
    }


    // 8bpp bitmaps
    //using bitmap8_t = bitmap_specific<uint8_t>;

    // 16bpp bitmaps
    //using bitmap16_t = bitmap_specific<uint16_t>;

    // 32bpp bitmaps
    //using bitmap32_t = bitmap_specific<uint32_t>;

    // 64bpp bitmaps
    //using bitmap64_t = bitmap_specific<uint64_t>;


    // ======================> bitmap_ind8, bitmap_ind16, bitmap_ind32, bitmap_ind64

    // BITMAP_FORMAT_IND8 bitmaps
    public class bitmap_ind8 : bitmap8_t
    {
        const bitmap_format k_bitmap_format = bitmap_format.BITMAP_FORMAT_IND8;

        // construction/destruction
        //bitmap_ind8(bitmap_ind8 &&) = default;
        public bitmap_ind8(int width = 0, int height = 0, int xslop = 0, int yslop = 0) : base(k_bitmap_format, width, height, xslop, yslop) { }
        //bitmap_ind8(uint8_t *base, int width, int height, int rowpixels) : bitmap8_t(k_bitmap_format, base, width, height, rowpixels) { }
        public bitmap_ind8(bitmap_ind8 source, rectangle subrect) : base(k_bitmap_format, source, subrect) { }

        //void wrap(uint8_t *base, int width, int height, int rowpixels) { bitmap_t::wrap(base, width, height, rowpixels); }
        //void wrap(const bitmap_ind8 &source, const rectangle &subrect) { bitmap_t::wrap(static_cast<const bitmap_t &>(source), subrect); }

        // getters
        //bitmap_format format() const { return k_bitmap_format; }

        //bitmap_ind8 &operator=(bitmap_ind8 &&) = default;
    }

    // BITMAP_FORMAT_IND16 bitmaps
    public class bitmap_ind16 : bitmap16_t
    {
        const bitmap_format k_bitmap_format = bitmap_format.BITMAP_FORMAT_IND16;

        // construction/destruction
        //bitmap_ind16(bitmap_ind16 &&) = default;
        public bitmap_ind16() : this(0, 0, 0, 0) { }
        public bitmap_ind16(int width, int height = 0, int xslop = 0, int yslop = 0) : base(k_bitmap_format, width, height, xslop, yslop) { }
        public bitmap_ind16(PointerU16 base_, int width, int height, int rowpixels) : base(k_bitmap_format, base_, width, height, rowpixels) { }  //bitmap_ind16(uint16_t *base, int width, int height, int rowpixels) : bitmap16_t(k_bitmap_format, base, width, height, rowpixels) { }
        public bitmap_ind16(bitmap_ind16 source, rectangle subrect) : base(k_bitmap_format, source, subrect) { }

        //void wrap(uint16_t *base, int width, int height, int rowpixels) { bitmap_t::wrap(base, width, height, rowpixels); }
        //void wrap(const bitmap_ind8 &source, const rectangle &subrect) { bitmap_t::wrap(static_cast<const bitmap_t &>(source), subrect); }

        // getters
        //bitmap_format format() const { return k_bitmap_format; }

        //bitmap_ind16 &operator=(bitmap_ind16 &&) = default;
    }

    // BITMAP_FORMAT_IND32 bitmaps
    class bitmap_ind32 : bitmap32_t
    {
        const bitmap_format k_bitmap_format = bitmap_format.BITMAP_FORMAT_IND32;

        // construction/destruction
        //bitmap_ind32(bitmap_ind32 &&) = default;
        bitmap_ind32(int width = 0, int height = 0, int xslop = 0, int yslop = 0) : base(k_bitmap_format, width, height, xslop, yslop) { }
        //bitmap_ind32(uint32_t *base, int width, int height, int rowpixels) : bitmap32_t(k_bitmap_format, base, width, height, rowpixels) { }
        bitmap_ind32(bitmap_ind32 source, rectangle subrect) : base(k_bitmap_format, source, subrect) { }

        //void wrap(uint32_t *base, int width, int height, int rowpixels) { bitmap_t::wrap(base, width, height, rowpixels); }
        //void wrap(const bitmap_ind8 &source, const rectangle &subrect) { bitmap_t::wrap(static_cast<const bitmap_t &>(source), subrect); }

        // getters
        //bitmap_format format() const { return k_bitmap_format; }

        //bitmap_ind32 &operator=(bitmap_ind32 &&) = default;
    }

    // BITMAP_FORMAT_IND64 bitmaps
    class bitmap_ind64 : bitmap64_t
    {
        const bitmap_format k_bitmap_format = bitmap_format.BITMAP_FORMAT_IND64;

        // construction/destruction
        //bitmap_ind64(bitmap_ind64 &&) = default;
        public bitmap_ind64(int width = 0, int height = 0, int xslop = 0, int yslop = 0) : base(k_bitmap_format, width, height, xslop, yslop) { }
        //bitmap_ind64(uint64_t *base, int width, int height, int rowpixels) : bitmap64_t(k_bitmap_format, base, width, height, rowpixels) { }
        public bitmap_ind64(bitmap_ind64 source, rectangle subrect) : base(k_bitmap_format, source, subrect) { }

        //void wrap(uint64_t *base, int width, int height, int rowpixels) { bitmap_t::wrap(base, width, height, rowpixels); }
        //void wrap(const bitmap_ind8 &source, const rectangle &subrect) { bitmap_t::wrap(static_cast<const bitmap_t &>(source), subrect); }

        // getters
        //bitmap_format format() const { return k_bitmap_format; }

        //bitmap_ind64 &operator=(bitmap_ind64 &&) = default;
    }


    // ======================> bitmap_yuy16, bitmap_rgb32, bitmap_argb32

    // BITMAP_FORMAT_YUY16 bitmaps
    class bitmap_yuy16 : bitmap16_t
    {
        const bitmap_format k_bitmap_format = bitmap_format.BITMAP_FORMAT_YUY16;

        // construction/destruction
        //bitmap_yuy16(bitmap_yuy16 &&) = default;
        bitmap_yuy16(int width = 0, int height = 0, int xslop = 0, int yslop = 0) : base(k_bitmap_format, width, height, xslop, yslop) { }
        //bitmap_yuy16(uint16_t *base, int width, int height, int rowpixels) : bitmap16_t(k_bitmap_format, base, width, height, rowpixels) { }
        bitmap_yuy16(bitmap_yuy16 source, rectangle subrect) : base(k_bitmap_format, source, subrect) { }

        //void wrap(uint16_t *base, int width, int height, int rowpixels) { bitmap_t::wrap(base, width, height, rowpixels); }
        //void wrap(const bitmap_yuy16 &source, const rectangle &subrect) { bitmap_t::wrap(static_cast<const bitmap_t &>(source), subrect); }

        // getters
        //bitmap_format format() const { return k_bitmap_format; }

        //bitmap_yuy16 &operator=(bitmap_yuy16 &&) = default;
    }

    // BITMAP_FORMAT_RGB32 bitmaps
    public class bitmap_rgb32 : bitmap32_t
    {
        const bitmap_format k_bitmap_format = bitmap_format.BITMAP_FORMAT_RGB32;

        // construction/destruction
        //bitmap_rgb32(bitmap_rgb32 &&) = default;
        public bitmap_rgb32(int width = 0, int height = 0, int xslop = 0, int yslop = 0) : base(k_bitmap_format, width, height, xslop, yslop) { }
        //bitmap_rgb32(uint32_t *base, int width, int height, int rowpixels) : bitmap32_t(k_bitmap_format, base, width, height, rowpixels) { }
        public bitmap_rgb32(bitmap_rgb32 source, rectangle subrect) : base(k_bitmap_format, source, subrect) { }

        //void wrap(uint32_t *base, int width, int height, int rowpixels) { bitmap_t::wrap(base, width, height, rowpixels); }
        //void wrap(const bitmap_rgb32 &source, const rectangle &subrect) { bitmap_t::wrap(static_cast<const bitmap_t &>(source), subrect); }

        // getters
        //bitmap_format format() const { return k_bitmap_format; }

        //bitmap_rgb32 &operator=(bitmap_rgb32 &&) = default;
    }

    // BITMAP_FORMAT_ARGB32 bitmaps
    public class bitmap_argb32 : bitmap32_t
    {
        const bitmap_format k_bitmap_format = bitmap_format.BITMAP_FORMAT_ARGB32;

        // construction/destruction
        //bitmap_argb32(bitmap_argb32 &&) = default;
        public bitmap_argb32(int width = 0, int height = 0, int xslop = 0, int yslop = 0) : base(k_bitmap_format, width, height, xslop, yslop) { }
        //bitmap_argb32(uint32_t *base, int width, int height, int rowpixels) : bitmap32_t(k_bitmap_format, base, width, height, rowpixels) { }
        public bitmap_argb32(bitmap_argb32 source, rectangle subrect) : base(k_bitmap_format, source, subrect) { }

        //void wrap(uint32_t *base, int width, int height, int rowpixels) { bitmap_t::wrap(base, width, height, rowpixels); }
        //void wrap(const bitmap_argb32 &source, const rectangle &subrect) { bitmap_t::wrap(static_cast<const bitmap_t &>(source), subrect); }

        // getters
        //bitmap_format format() const { return k_bitmap_format; }

        //bitmap_argb32 &operator=(bitmap_argb32 &&) = default;
    }
}
