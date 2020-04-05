// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int32_t = System.Int32;
using ListBytesPointer = mame.ListPointer<System.Byte>;
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
    public class rectangle
    {
        // internal state
        int32_t m_min_x = 0;          // minimum X, or left coordinate
        int32_t m_max_x = 0;          // maximum X, or right coordinate (inclusive)
        int32_t m_min_y = 0;          // minimum Y, or top coordinate
        int32_t m_max_y = 0;          // maximum Y, or bottom coordinate (inclusive)


        // construction/destruction
        public rectangle() { }
        public rectangle(int minx, int maxx, int miny, int maxy) { m_min_x = minx; m_max_x = maxx; m_min_y = miny; m_max_y = maxy; }
        public rectangle(rectangle rect) : this(rect.m_min_x, rect.m_max_x, rect.m_min_y, rect.m_max_y) { }

        // getters
        public int32_t left() { return m_min_x; }
        public int32_t right() { return m_max_x; }
        public int32_t top() { return m_min_y; }
        public int32_t bottom() { return m_max_y; }

        public int32_t min_x { get { return m_min_x; } set { m_min_x = value; } }
        public int32_t max_x { get { return m_max_x; } set { m_max_x = value; } }
        public int32_t min_y { get { return m_min_y; } set { m_min_y = value; } }
        public int32_t max_y { get { return m_max_y; } set { m_max_y = value; } }

#if false
        // compute intersection with another rect
        rectangle &operator&=(const rectangle &src)
        {
            if (src.min_x > min_x) min_x = src.min_x;
            if (src.max_x < max_x) max_x = src.max_x;
            if (src.min_y > min_y) min_y = src.min_y;
            if (src.max_y < max_y) max_y = src.max_y;
            return *this;
        }

        // compute union with another rect
        rectangle &operator|=(const rectangle &src)
        {
            if (src.min_x < min_x) min_x = src.min_x;
            if (src.max_x > max_x) max_x = src.max_x;
            if (src.min_y < min_y) min_y = src.min_y;
            if (src.max_y > max_y) max_y = src.max_y;
            return *this;
        }
#endif

        // compute intersection with another rect
        public rectangle intersection(rectangle src)
        {
            if (src.m_min_x > m_min_x) m_min_x = src.m_min_x;
            if (src.m_max_x < m_max_x) m_max_x = src.m_max_x;
            if (src.m_min_y > m_min_y) m_min_y = src.m_min_y;
            if (src.m_max_y < m_max_y) m_max_y = src.m_max_y;
            return this;
        }


        // comparisons
        //bool operator==(const rectangle &rhs) const { return min_x == rhs.min_x && max_x == rhs.max_x && min_y == rhs.min_y && max_y == rhs.max_y; }
        //bool operator!=(const rectangle &rhs) const { return min_x != rhs.min_x || max_x != rhs.max_x || min_y != rhs.min_y || max_y != rhs.max_y; }
        //bool operator>(const rectangle &rhs) const { return min_x < rhs.min_x && min_y < rhs.min_y && max_x > rhs.max_x && max_y > rhs.max_y; }
        //bool operator>=(const rectangle &rhs) const { return min_x <= rhs.min_x && min_y <= rhs.min_y && max_x >= rhs.max_x && max_y >= rhs.max_y; }
        //bool operator<(const rectangle &rhs) const { return min_x >= rhs.min_x || min_y >= rhs.min_y || max_x <= rhs.max_x || max_y <= rhs.max_y; }
        //bool operator<=(const rectangle &rhs) const { return min_x > rhs.min_x || min_y > rhs.min_y || max_x < rhs.max_x || max_y < rhs.max_y; }


        // other helpers
        public bool empty() { return m_min_x > m_max_x || m_min_y > m_max_y; }
        public bool contains(int32_t x, int32_t y) { return x >= m_min_x && x <= m_max_x && y >= m_min_y && y <= m_max_y; }
        public bool contains(rectangle rect) { return m_min_x <= rect.m_min_x && m_max_x >= rect.m_max_x && m_min_y <= rect.m_min_y && m_max_y >= rect.m_max_y; }
        public int32_t width() { return m_max_x + 1 - m_min_x; }
        public int32_t height() { return m_max_y + 1 - m_min_y; }
        public int32_t xcenter() { return (m_min_x + m_max_x + 1) / 2; }
        public int32_t ycenter() { return (m_min_y + m_max_y + 1) / 2; }


        // setters
        public void set(int32_t minx, int32_t maxx, int32_t miny, int32_t maxy) { m_min_x = minx; m_max_x = maxx; m_min_y = miny; m_max_y = maxy; }
        public void setx(int32_t minx, int32_t maxx) { m_min_x = minx; m_max_x = maxx; }
        public void sety(int32_t miny, int32_t maxy) { m_min_y = miny; m_max_y = maxy; }
        public void set_width(int32_t width) { m_max_x = m_min_x + width - 1; }
        public void set_height(int32_t height) { m_max_y = m_min_y + height - 1; }
        void set_origin(int32_t x, int32_t y) { m_max_x += x - m_min_x; m_max_y += y - m_min_y; m_min_x = x; m_min_y = y; }
        void set_size(int32_t width, int32_t height) { set_width(width); set_height(height); }


        // offset helpers
        void offset(int32_t xdelta, int32_t ydelta) { m_min_x += xdelta; m_max_x += xdelta; m_min_y += ydelta; m_max_y += ydelta; }
        void offsetx(int32_t delta) { m_min_x += delta; m_max_x += delta; }
        void offsety(int32_t delta) { m_min_y += delta; m_max_y += delta; }
    }


    // ======================> bitmap_t
    // bitmaps describe a rectangular array of pixels
    public class bitmap_t : global_object
    {
        // internal state
        RawBuffer m_alloc;  //std::unique_ptr<uint8_t []> m_alloc;        // pointer to allocated pixel memory
        uint32_t m_allocbytes;   // size of our allocation
        RawBufferPointer m_base;  //void *          m_base;         // pointer to pixel (0,0) (adjusted for padding)
        int32_t m_rowpixels;    // pixels per row (including padding)
        int32_t m_width;        // width of the bitmap
        int32_t m_height;       // height of the bitmap
        bitmap_format m_format;       // format of the bitmap
        uint8_t m_bpp;          // bits per pixel
        palette_t m_palette;      // optional palette
        rectangle m_cliprect = new rectangle();     // a clipping rectangle covering the full bitmap


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


            assert(valid_format());

            // allocate intializes all other fields
            allocate(width, height, xslop, yslop);
        }


#if false
        /**
         * @fn  bitmap_t::bitmap_t(bitmap_format format, int bpp, void *base, int width, int height, int rowpixels)
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
        bitmap_t::bitmap_t(bitmap_format format, byte bpp, void *baseptr, int width, int height, int rowpixels)
            : m_alloc(NULL),
                m_allocbytes(0),
                m_base(base),
                m_rowpixels(rowpixels),
                m_width(width),
                m_height(height),
                m_format(format),
                m_bpp(bpp),
                m_palette(NULL),
                m_cliprect(0, width - 1, 0, height - 1)
        {
            assert(valid_format());
        }
#endif

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


            assert(format == source.m_format);
            assert(bpp == source.m_bpp);
            assert(source.cliprect().contains(subrect));
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
            assert(m_format != bitmap_format.BITMAP_FORMAT_INVALID);
            assert(m_bpp == 8 || m_bpp == 16 || m_bpp == 32 || m_bpp == 64);

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
            m_allocbytes = (UInt32)(m_rowpixels * (m_height + 2 * yslop) * m_bpp / 8);
            m_alloc = new RawBuffer(m_allocbytes);  //m_alloc = new byte[m_allocbytes];

            // clear to 0 by default
            memset(m_alloc, (uint8_t)0, m_allocbytes);

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

            // if we need more memory, just realloc
            if (new_allocbytes > m_allocbytes)
            {
                palette_t palette = m_palette;
                allocate(width, height, xslop, yslop);
                set_palette(palette);
                return;
            }

            // otherwise, reconfigure
            m_rowpixels = new_rowpixels;
            m_width = width;
            m_height = height;
            m_cliprect.set(0, width - 1, 0, height - 1);

            // re-compute the base
            compute_base(xslop, yslop);
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

        public void fill(uint32_t color) { fill(color, m_cliprect); }

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
        public void fill(uint32_t color, rectangle cliprect)
        {
            // if we have a cliprect, intersect with that
            rectangle fill = cliprect;
            fill.intersection(cliprect);  // fill &= m_cliprect;
            if (fill.empty())
                return;

            // based on the bpp go from there
            switch (m_bpp)
            {
                case 8:
                    // 8bpp always uses memset
                    for (int32_t y = fill.top(); y <= fill.bottom(); y++)
                    {
                        memset(raw_pixptr(y, fill.left()), (uint8_t)color, (UInt32)fill.width());
                    }
                    break;

                case 16:
                    // 16bpp can use memset if the bytes are equal
                    if ((uint8_t)(color >> 8) == (uint8_t)color)
                    {
                        for (int32_t y = fill.top(); y <= fill.bottom(); y++)
                        {
                            memset(raw_pixptr(y, fill.left()), (uint8_t)color, (UInt32)fill.width() * 2);
                        }
                    }
                    else
                    {
                        // Fill the first line the hard way
                        UInt16BufferPointer destrow = (UInt16BufferPointer)pixt<uint16_t>(fill.top());  //uint16_t *destrow = &pixt<uint16_t>(fill.top());
                        for (int32_t x = fill.left(); x <= fill.right(); x++)
                            destrow[x] = (uint16_t)color;

                        // For the other lines, just copy the first one
                        UInt16BufferPointer destrow0 = (UInt16BufferPointer)pixt<uint16_t>(fill.top(), fill.left());  //void *destrow0 = &pixt<uint16_t>(fill.top(), fill.left());
                        for (int32_t y = fill.top() + 1; y <= fill.bottom(); y++)
                        {
                            destrow = (UInt16BufferPointer)pixt<uint16_t>(y, fill.left());  //destrow = &pixt<uint16_t>(y, fill.left());
                            //memcpy(destrow, destrow0, fill.width() * 2);
                            for (int i = 0; i < fill.width(); i++)  // * 2
                                destrow[i] = destrow0[i];
                        }
                    }
                    break;

                case 32:
                    // 32bpp can use memset if the bytes are equal
                    if ((uint8_t)(color >> 8) == (uint8_t)color && (uint16_t)(color >> 16) == (uint16_t)color)
                    {
                        for (int32_t y = fill.top(); y <= fill.bottom(); y++)
                        {
                            memset(pixt<uint32_t>(y, fill.left()), (uint8_t)color, (UInt32)fill.width() * 4);
                        }
                    }
                    else
                    {
                        // Fill the first line the hard way
                        UInt32BufferPointer destrow = (UInt32BufferPointer)pixt<uint32_t>(fill.top());  //uint32_t *destrow  = &pixt<uint32_t>(fill.top());
                        for (int32_t x = fill.left(); x <= fill.right(); x++)
                            destrow[x] = (uint32_t)color;

                        // For the other lines, just copy the first one
                        UInt32BufferPointer destrow0 = (UInt32BufferPointer)pixt<uint32_t>(fill.top(), fill.left());  //uint32_t *destrow0 = &pixt<uint32_t>(fill.top(), fill.left());
                        for (int32_t y = fill.top() + 1; y <= fill.bottom(); y++)
                        {
                            destrow = (UInt32BufferPointer)pixt<uint32_t>(y, fill.left());  //destrow = &pixt<uint32_t>(y, fill.left());
                            //memcpy(destrow, destrow0, fill.width() * 4);
                            for (int i = 0; i < fill.width(); i++)  // * 4
                                destrow[i] = destrow0[i];
                        }
                    }
                    break;

                case 64:
                    // 64bpp can use memset if the bytes are equal
                    if ((uint8_t)(color >> 8) == (uint8_t)color && (uint16_t)(color >> 16) == (uint16_t)color) // FIXME: really?  wat about the upper bits that would be zeroed when done the "hard way"?
                    {
                        for (int32_t y = fill.top(); y <= fill.bottom(); y++)
                        {
                            memset(pixt<uint64_t>(y, fill.left()), (uint8_t)color, (UInt32)fill.width() * 8);
                        }
                    }
                    else
                    {
                        // Fill the first line the hard way
                        UInt64BufferPointer destrow = (UInt64BufferPointer)pixt<uint64_t>(fill.top());  //uint64_t *destrow  = &pixt<uint64_t>(fill.top());
                        for (int32_t x = fill.left(); x <= fill.right(); x++)
                            destrow[x] = (uint64_t)color;

                        // For the other lines, just copy the first one
                        UInt64BufferPointer destrow0 = (UInt64BufferPointer)pixt<uint64_t>(fill.top(), fill.left());  //uint64_t *destrow0 = &pixt<uint64_t>(fill.top(), fill.left());
                        for (int32_t y = fill.top() + 1; y <= fill.bottom(); y++)
                        {
                            destrow = (UInt64BufferPointer)pixt<uint64_t>(y, fill.left());  //destrow = &pixt<uint64_t>(y, fill.left());
                            //memcpy(destrow, destrow0, (UInt32)fill.width() * 8);
                            for (int i = 0; i < fill.width(); i++)  // * 8
                                destrow[i] = destrow0[i];
                        }
                    }
                    break;

                default:
                    break;
            }
        }


        void plot_box(int x, int y, int width, int height, uint32_t color)
        {
            rectangle clip = new rectangle(x, x + width - 1, y, y + height - 1);
            fill(color, clip);
        }


        // pixel access

        //template<typename PixelType>
        //PixelType &pixt(int32_t y, int32_t x = 0) const { return *(reinterpret_cast<PixelType *>(m_base) + y * m_rowpixels + x); }
        protected RawBufferPointer pixt<PixelType>(int32_t y, int32_t x = 0)
        {
            if      (typeof(PixelType) == typeof(uint8_t))  return new RawBufferPointer(m_base) + (y * m_rowpixels + x);
            else if (typeof(PixelType) == typeof(uint16_t)) return new UInt16BufferPointer(m_base) + (y * m_rowpixels + x);
            else if (typeof(PixelType) == typeof(uint32_t)) return new UInt32BufferPointer(m_base) + (y * m_rowpixels + x);
            else if (typeof(PixelType) == typeof(uint64_t)) return new UInt64BufferPointer(m_base) + (y * m_rowpixels + x);
            else throw new emu_fatalerror("bitmap_t.pix() - Unknown type. {0}", typeof(PixelType));
        }

        public RawBufferPointer raw_pixptr(int32_t y, int32_t x = 0)  //void *raw_pixptr(int32_t y, int32_t x = 0) const
        {
            return new RawBufferPointer(m_base, (y * m_rowpixels + x) * m_bpp / 8);  //return reinterpret_cast<uint8_t *>(m_base) + (y * m_rowpixels + x) * m_bpp / 8; }
        }


        // for use by subclasses only to ensure type correctness
        //void wrap(void *base, int width, int height, int rowpixels);
        //void wrap(const bitmap_t &source, const rectangle &subrect);


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
            m_base = new RawBufferPointer(m_alloc, (m_rowpixels * yslop + xslop) * (m_bpp / 8));  // m_base = m_alloc + (m_rowpixels * yslop + xslop) * (m_bpp / 8);
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
    public class bitmap_specific<PixelType, PixelTypeBufferPointer> : bitmap_t where PixelTypeBufferPointer : RawBufferPointer
    {
        int PixelBits;  //static constexpr int PixelBits = 8 * sizeof(PixelType);


        // construction/destruction -- subclasses only
        //bitmap_specific(bitmap_specific<PixelType> &&) = default;
        protected bitmap_specific(int sizeof_PixelType, bitmap_format format, int width = 0, int height = 0, int xslop = 0, int yslop = 0) : base(format, (uint8_t)(8 * sizeof_PixelType), width, height, xslop, yslop) { PixelBits = 8 * sizeof_PixelType; }
#if false
        protected bitmap_specific(int sizeof_PixelType, bitmap_format format, PixelType base_, int width, int height, int rowpixels) : base(format, (uint8_t)(8 * sizeof_PixelBits), base_, width, height, rowpixels) { PixelBits = 8 * sizeof_PixelBits; }
#endif
        protected bitmap_specific(int sizeof_PixelType, bitmap_format format, bitmap_specific<PixelType, PixelTypeBufferPointer> source, rectangle subrect) : base(format, (uint8_t)(8 * sizeof_PixelType), source, subrect) { PixelBits = 8 * sizeof_PixelType; }


        //bitmap_specific<PixelType> &operator=(bitmap_specific<PixelType> &&) = default;


        //using pixel_t = PixelType;


        // getters
        public new uint8_t bpp() { return (uint8_t)PixelBits; }


        // pixel accessors
        public PixelTypeBufferPointer pix(int32_t y, int32_t x = 0) { return (PixelTypeBufferPointer)pixt<PixelType>(y, x); }
        public RawBufferPointer pix8(int32_t y, int32_t x = 0) { static_assert(PixelBits == 8, "must be 8bpp"); return pixt<PixelType>(y, x); }
        public UInt16BufferPointer pix16(int32_t y, int32_t x = 0) { static_assert(PixelBits == 16, "must be 16bpp"); return (UInt16BufferPointer)pixt<PixelType>(y, x); }
        public UInt32BufferPointer pix32(int32_t y, int32_t x = 0) { static_assert(PixelBits == 32, "must be 32bpp"); return (UInt32BufferPointer)pixt<PixelType>(y, x); }
        public UInt64BufferPointer pix64(int32_t y, int32_t x = 0) { static_assert(PixelBits == 64, "must be 64bpp"); return (UInt64BufferPointer)pixt<PixelType>(y, x); }
    }


    // 8bpp bitmaps
    //using bitmap8_t = bitmap_specific<uint8_t>;
    public class bitmap8_t : bitmap_specific<uint8_t, RawBufferPointer>
    {
        //bitmap8_t(bitmap_specific<PixelType> &&) = default;
        protected bitmap8_t(bitmap_format format, int width = 0, int height = 0, int xslop = 0, int yslop = 0) : base(1, format, width, height, xslop, yslop) { }
#if false
        bitmap8_t(bitmap_format format, PixelType *base, int width, int height, int rowpixels) : base(format, PixelBits, base, width, height, rowpixels) { }
#endif
        protected bitmap8_t(bitmap_format format, bitmap_specific<uint8_t, RawBufferPointer> source, rectangle subrect) : base(1, format, source, subrect) { }
    }

    // 16bpp bitmaps
    //using bitmap16_t = bitmap_specific<uint16_t>;
    public class bitmap16_t : bitmap_specific<uint16_t, UInt16BufferPointer>
    {
        //bitmap16_t(bitmap_specific<PixelType> &&) = default;
        protected bitmap16_t(bitmap_format format, int width = 0, int height = 0, int xslop = 0, int yslop = 0) : base(2, format, width, height, xslop, yslop) { }
#if false
        bitmap16_t(bitmap_format format, PixelType *base, int width, int height, int rowpixels) : base(format, PixelBits, base, width, height, rowpixels) { }
#endif
        protected bitmap16_t(bitmap_format format, bitmap_specific<uint16_t, UInt16BufferPointer> source, rectangle subrect) : base(2, format, source, subrect) { }
    }

    // 32bpp bitmaps
    //using bitmap32_t = bitmap_specific<uint32_t>;
    public class bitmap32_t : bitmap_specific<uint32_t, UInt32BufferPointer>
    {
        //bitmap32_t(bitmap_specific<PixelType> &&) = default;
        protected bitmap32_t(bitmap_format format, int width = 0, int height = 0, int xslop = 0, int yslop = 0) : base(4, format, width, height, xslop, yslop) { }
#if false
        bitmap32_t(bitmap_format format, PixelType *base, int width, int height, int rowpixels) : base(format, PixelBits, base, width, height, rowpixels) { }
#endif
        protected bitmap32_t(bitmap_format format, bitmap_specific<uint32_t, UInt32BufferPointer> source, rectangle subrect) : base(4, format, source, subrect) { }
    }

    // 64bpp bitmaps
    //using bitmap64_t = bitmap_specific<uint64_t>;
    public class bitmap64_t : bitmap_specific<uint64_t, UInt64BufferPointer>
    {
        //bitmap64_t(bitmap_specific<PixelType> &&) = default;
        protected bitmap64_t(bitmap_format format, int width = 0, int height = 0, int xslop = 0, int yslop = 0) : base(8, format, width, height, xslop, yslop) { }
#if false
        bitmap64_t(bitmap_format format, PixelType *base, int width, int height, int rowpixels) : base(format, PixelBits, base, width, height, rowpixels) { }
#endif
        protected bitmap64_t(bitmap_format format, bitmap_specific<uint64_t, UInt64BufferPointer> source, rectangle subrect) : base(8, format, source, subrect) { }
    }


    // ======================> bitmap_ind8, bitmap_ind16, bitmap_ind32, bitmap_ind64

    // BITMAP_FORMAT_IND8 bitmaps
    public class bitmap_ind8 : bitmap8_t
    {
        const bitmap_format k_bitmap_format = bitmap_format.BITMAP_FORMAT_IND8;

        // construction/destruction
        //bitmap_ind8(bitmap_ind8 &&) = default;
        public bitmap_ind8(int width = 0, int height = 0, int xslop = 0, int yslop = 0) : base(k_bitmap_format, width, height, xslop, yslop) { }
#if false
        public bitmap_ind8(UINT8 *base, int width, int height, int rowpixels) : base(k_bitmap_format, base, width, height, rowpixels) { }
#endif
        public bitmap_ind8(bitmap_ind8 source, rectangle subrect) : base(k_bitmap_format, source, subrect) { }

        //void wrap(UINT8 *base, int width, int height, int rowpixels) { bitmap_t::wrap(base, width, height, rowpixels); }
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
        public bitmap_ind16(int width = 0, int height = 0, int xslop = 0, int yslop = 0) : base(k_bitmap_format, width, height, xslop, yslop) { }
#if false
        public bitmap_ind16(UINT16 *base, int width, int height, int rowpixels) : bitmap16_t(k_bitmap_format, base, width, height, rowpixels) { }
#endif
        public bitmap_ind16(bitmap_ind16 source, rectangle subrect) : base(k_bitmap_format, source, subrect) { }

        //void wrap(UINT16 *base, int width, int height, int rowpixels) { bitmap_t::wrap(base, width, height, rowpixels); }
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
#if false
        bitmap_ind32(UINT32 *base, int width, int height, int rowpixels) : base(k_bitmap_format, base, width, height, rowpixels) { }
#endif
        bitmap_ind32(bitmap_ind32 source, rectangle subrect) : base(k_bitmap_format, source, subrect) { }

        //void wrap(UINT32 *base, int width, int height, int rowpixels) { bitmap_t::wrap(base, width, height, rowpixels); }
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
#if false
        bitmap_ind64(UINT64 *base, int width, int height, int rowpixels) : base(k_bitmap_format, base, width, height, rowpixels) { }
#endif
        public bitmap_ind64(bitmap_ind64 source, rectangle subrect) : base(k_bitmap_format, source, subrect) { }

        //void wrap(UINT64 *base, int width, int height, int rowpixels) { bitmap_t::wrap(base, width, height, rowpixels); }
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
#if false
        bitmap_yuy16(UINT16 *base, int width, int height, int rowpixels) : base(k_bitmap_format, base, width, height, rowpixels) { }
#endif
        bitmap_yuy16(bitmap_yuy16 source, rectangle subrect) : base(k_bitmap_format, source, subrect) { }

        //void wrap(UINT16 *base, int width, int height, int rowpixels) { bitmap_t::wrap(base, width, height, rowpixels); }
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
#if false
        public bitmap_rgb32(UINT32 *base, int width, int height, int rowpixels) : base(k_bitmap_format, base, width, height, rowpixels) { }
#endif
        public bitmap_rgb32(bitmap_rgb32 source, rectangle subrect) : base(k_bitmap_format, source, subrect) { }

        //void wrap(UINT32 *base, int width, int height, int rowpixels) { bitmap_t::wrap(base, width, height, rowpixels); }
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
#if false
        public bitmap_argb32(UINT32 *base, int width, int height, int rowpixels) : base(k_bitmap_format, base, width, height, rowpixels) { }
#endif
        public bitmap_argb32(bitmap_argb32 source, rectangle subrect) : base(k_bitmap_format, source, subrect) { }

        //void wrap(UINT32 *base, int width, int height, int rowpixels) { bitmap_t::wrap(base, width, height, rowpixels); }
        //void wrap(const bitmap_argb32 &source, const rectangle &subrect) { bitmap_t::wrap(static_cast<const bitmap_t &>(source), subrect); }

        // getters
        //bitmap_format format() const { return k_bitmap_format; }

        //bitmap_argb32 &operator=(bitmap_argb32 &&) = default;
    }
}
