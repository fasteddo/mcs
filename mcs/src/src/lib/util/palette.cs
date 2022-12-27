// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using int32_t = System.Int32;
using rgb15_t = System.UInt16;  //typedef uint16_t rgb15_t;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.cpp_global;
using static mame.palette_global;


namespace mame
{
    // an rgb15_t is a single combined 15-bit R,G,B value
    //typedef uint16_t rgb15_t;


    // ======================> rgb_t
    // an rgb_t is a single combined R,G,B (and optionally alpha) value
    public class rgb_t
    {
        // constant factories
        static readonly rgb_t _black = new rgb_t(0,     0,   0);
        static readonly rgb_t _white = new rgb_t(255, 255, 255);
        static readonly rgb_t _green = new rgb_t(  0, 255,   0);
        static readonly rgb_t _amber = new rgb_t(247, 170,   0);
        static readonly rgb_t _transparent = new rgb_t(0, 0, 0, 0);

        static readonly rgb_t _red   = new rgb_t(255,   0,   0);
        static readonly rgb_t _blue  = new rgb_t(  0,   0, 255);

        // https://msdn.microsoft.com/en-us/library/aa358802.aspx
        static readonly rgb_t _pink        = new rgb_t(0xff, 0xc0, 0xcb);
        static readonly rgb_t _greenyellow = new rgb_t(0xad, 0xff, 0x2f);

        public static rgb_t black() { return _black; }
        public static rgb_t white() { return _white; }
        public static rgb_t green() { return _green; }
        public static rgb_t amber() { return _amber; }
        public static rgb_t transparent() { return _transparent; }

        public static rgb_t red() { return _red; }
        public static rgb_t blue() { return _blue; }

        public static rgb_t pink() { return _pink; }
        public static rgb_t greenyellow() { return _greenyellow; }


        uint32_t  m_data;


        // construction/destruction
        public rgb_t() { m_data = 0; }
        public rgb_t(uint32_t data) { m_data = data; }
        public rgb_t(uint8_t r, uint8_t g, uint8_t b) : this(255, r, g, b) { }
        public rgb_t(uint8_t a, uint8_t r, uint8_t g, uint8_t b) { m_data = ((UInt32)a << 24) | ((UInt32)r << 16) | ((UInt32)g << 8) | (UInt32)b; }


        // getters
        public uint8_t a() { return (uint8_t)(m_data >> 24); }
        public uint8_t r() { return (uint8_t)(m_data >> 16); }
        public uint8_t g() { return (uint8_t)(m_data >> 8); }
        public uint8_t b() { return (uint8_t)(m_data >> 0); }
        public rgb15_t as_rgb15() { return (rgb15_t)(((r() >> 3) << 10) | ((g() >> 3) << 5) | ((b() >> 3) << 0)); }
        //UINT8 brightness() const { return (r() * 222 + g() * 707 + b() * 71) / 1000; }
        //UINT32 const *ptr() const { return &m_data; }
        //void expand_rgb(uint8_t &r, uint8_t &g, uint8_t &b) const { r = m_data >> 16; g = m_data >> 8; b = m_data >> 0; }
        //void expand_rgb(int &r, int &g, int &b) const { r = (m_data >> 16) & 0xff; g = (m_data >> 8) & 0xff; b = (m_data >> 0) & 0xff; }


        // setters
        public rgb_t set_a(uint8_t a) { m_data &= ~0xff000000; m_data |= (UInt32)a << 24; return this; }
        //rgb_t set_r(byte r) { m_data &= ~0x00ff0000; m_data |= (UInt32)r << 16; return this; }
        //rgb_t set_g(byte g) { m_data &= ~0x0000ff00; m_data |= (UInt32)g <<  8; return this; }
        //rgb_t set_b(byte b) { m_data &= ~0x000000ff; m_data |= (UInt32)b <<  0; return this; }


        // implicit conversion operators
        public static implicit operator uint32_t(rgb_t rgb) { return rgb.m_data; }


        // operations
        public rgb_t scale8(uint8_t scale) { m_data = new rgb_t(clamphi((a() * scale) >> 8), clamphi((r() * scale) >> 8), clamphi((g() * scale) >> 8), clamphi((b() * scale) >> 8)); return this; }


        // assignment operators
        //rgb_t &operator=(UINT32 rhs) { m_data = rhs; return *this; }
        //rgb_t &operator+=(const rgb_t &rhs) { m_data = uint32_t(*this + rhs); return *this; }
        //rgb_t &operator-=(const rgb_t &rhs) { m_data = uint32_t(*this - rhs); return *this; }


        // arithmetic operators
        //constexpr rgb_t operator+(const rgb_t &rhs) const { return rgb_t(clamphi(a() + rhs.a()), clamphi(r() + rhs.r()), clamphi(g() + rhs.g()), clamphi(b() + rhs.b())); }
        //constexpr rgb_t operator-(const rgb_t &rhs) const { return rgb_t(clamplo(a() - rhs.a()), clamplo(r() - rhs.r()), clamplo(g() - rhs.g()), clamplo(b() - rhs.b())); }
        public static rgb_t operator+(rgb_t left, rgb_t right) { return new rgb_t(clamphi(left.a() + right.a()), clamphi(left.r() + right.r()), clamphi(left.g() + right.g()), clamphi(left.b() + right.b())); }
        public static rgb_t operator-(rgb_t left, rgb_t right) { return new rgb_t(clamplo(left.a() - right.a()), clamplo(left.r() - right.r()), clamplo(left.g() - right.g()), clamplo(left.b() - right.b())); }


        // static helpers
        public static uint8_t clamp(int32_t value) { return (value < 0) ? (uint8_t)0 : (value > 255) ? (uint8_t)255 : (uint8_t)value; }
        static uint8_t clamphi(int32_t value) { return (value > 255) ? (uint8_t)255 : (uint8_t)value; }
        static uint8_t clamplo(int32_t value) { return (value < 0) ? (uint8_t)0 : (uint8_t)value; }
    }


    // ======================> palette_client
    // a single palette client
    public class palette_client : IDisposable
    {
        // internal object to track dirty states
        class dirty_state
        {
            // internal state
            std.vector<uint32_t> m_dirty = new std.vector<uint32_t>();   // bitmap of dirty entries
            uint32_t m_mindirty;             // minimum dirty entry
            uint32_t m_maxdirty;             // minimum dirty entry


            // construction
            //-------------------------------------------------
            //  dirty_state - constructor
            //-------------------------------------------------
            public dirty_state()
            {
                m_mindirty = 0;
                m_maxdirty = 0;
            }


            // operations
            //-------------------------------------------------
            //  dirty_list - return the current list and
            //  min/max values
            //-------------------------------------------------
            public MemoryContainer<uint32_t> dirty_list(out uint32_t mindirty, out uint32_t maxdirty)  //const uint32_t *dirty_list(uint32_t &mindirty, uint32_t &maxdirty);
            {
                // fill in the mindirty/maxdirty
                mindirty = m_mindirty;
                maxdirty = m_maxdirty;

                // if nothing to report, report nothing
                return (m_mindirty > m_maxdirty) ? null : m_dirty;
            }

            //-------------------------------------------------
            //  resize - resize the dirty array and mark all
            //  dirty
            //-------------------------------------------------
            public void resize(uint32_t colors)
            {
                // resize to the correct number of dwords and mark all entries dirty
                uint32_t dirty_dwords = (colors + 31) / 32;
                m_dirty.resize(dirty_dwords);
                std.fill(m_dirty, uint32_t.MaxValue);  //std::fill(m_dirty.begin(), m_dirty.end(), ~uint32_t(0));

                // mark all entries dirty
                m_dirty[dirty_dwords - 1] &= (1U << ((int)colors % 32)) - 1;

                // set min/max
                m_mindirty = 0;
                m_maxdirty = colors - 1;
            }

            //-------------------------------------------------
            //  mark_dirty - mark a single entry dirty
            //-------------------------------------------------
            public void mark_dirty(uint32_t index)
            {
                m_dirty[index / 32] |= 1U << ((int)index % 32);
                m_mindirty = std.min(m_mindirty, index);
                m_maxdirty = std.min(m_maxdirty, index);
            }

            //-------------------------------------------------
            //  reset - clear the dirty array to mark all
            //  entries as clean
            //-------------------------------------------------
            public void reset()
            {
                //throw new emu_unimplemented();
#if false
                if (m_mindirty <= m_maxdirty)
                    std::fill(&m_dirty[m_mindirty / 32], &m_dirty[m_maxdirty / 32] + 1, 0);
#endif
                m_mindirty = (uint32_t)(m_dirty.size() * 32 - 1);
                m_maxdirty = 0;
            }
        }


        // internal state
        palette_t m_palette;                  // reference to the palette
        palette_client m_next;                     // pointer to next client
        int m_liveIdx = 0;  //dirty_state *   m_live;                     // live dirty state
        int m_previousIdx = 0;  //dirty_state *   m_previous;                 // previous dirty state
        dirty_state [] m_dirty = new dirty_state[2];                 // two dirty states


        // construction/destruction
        //-------------------------------------------------
        //  palette_client - constructor
        //-------------------------------------------------
        public palette_client(palette_t palette)
        {
            m_palette = palette;
            m_next = null;
            m_liveIdx = 0;  // m_live(&m_dirty[0]),
            m_previousIdx = 1;  // m_previous(&m_dirty[1])


            // add a reference to the palette
            palette.ref_();


            // resize the dirty lists
            uint32_t total_colors = (uint32_t)(palette.num_colors() * palette.num_groups());
            m_dirty[0] = new dirty_state();
            m_dirty[1] = new dirty_state();
            m_dirty[0].resize(total_colors);
            m_dirty[1].resize(total_colors);

            // now add us to the list of clients
            m_next = palette.m_client_list;
            palette.m_client_list = this;
        }

        ~palette_client()
        {
            assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            //throw new emu_unimplemented();
#if false
            // first locate and remove ourself from our palette's list
            for (palette_client **curptr = &m_palette.m_client_list; *curptr != NULL; curptr = &(*curptr)->m_next)
            {
                if (*curptr == this)
                {
                    *curptr = m_next;
                    break;
                }
            }
#endif

            // now deref the palette
            m_palette.deref();

            m_isDisposed = true;
        }


        // getters
        public palette_client next() { return m_next; }

        public palette_t palette() { return m_palette; }

        //-------------------------------------------------
        //  dirty_list - atomically get the current dirty
        //  list for a client
        //-------------------------------------------------
        public MemoryContainer<uint32_t> dirty_list(out uint32_t mindirty, out uint32_t maxdirty)  //const uint32_t *dirty_list(uint32_t &mindirty, uint32_t &maxdirty)
        {
            // if nothing to report, report nothing and don't swap
            MemoryContainer<uint32_t> result = m_dirty[m_liveIdx].dirty_list(out mindirty, out maxdirty);
            if (result == null)
                return null;

            // swap the live and previous lists
            int tempIdx = m_liveIdx;  // dirty_state *temp = m_live;
            m_liveIdx = m_previousIdx;
            m_previousIdx = tempIdx;

            // reset new live one and return the pointer to the previous
            m_dirty[m_liveIdx].reset();  // m_live.reset();

            return result;
        }


        // dirty marking
        public void mark_dirty(uint32_t index) { m_dirty[m_liveIdx].mark_dirty(index); }
    }


    // ======================> palette_t
    // a palette object
    public class palette_t
    {
        // friend class palette_client;

        // internal state
        uint32_t m_refcount;                   // reference count on the palette
        uint32_t m_numcolors;                  // number of colors in the palette
        uint32_t m_numgroups;                  // number of groups in the palette

        float m_brightness;                 // overall brightness value
        float m_contrast;                   // overall contrast value
        float m_gamma;                      // overall gamma value
        uint8_t [] m_gamma_map = new uint8_t[256];             // gamma map

        std.vector<rgb_t> m_entry_color;           // array of raw colors
        std.vector<float> m_entry_contrast;        // contrast value for each entry
        std.vector<rgb_t> m_adjusted_color;        // array of adjusted colors
        std.vector<rgb15_t> m_adjusted_rgb15;  //std::vector<rgb_t> m_adjusted_rgb15;        // array of adjusted colors as RGB15

        std.vector<float> m_group_bright;          // brightness value for each group
        std.vector<float> m_group_contrast;        // contrast value for each group

        public palette_client m_client_list;                // list of clients for this palette


        // static constructor: used to ensure same new/delete is used
        //-------------------------------------------------
        //  alloc - static allocator
        //-------------------------------------------------
        public static palette_t alloc(uint32_t numcolors, uint32_t numgroups = 1) { return new palette_t(numcolors, numgroups); }


        // reference counting
        public void ref_() { m_refcount++; }
        public void deref()
        {
            //if (--m_refcount == 0)
            //    delete this;
        }


        // getters
        public int num_colors() { return (int)m_numcolors; }
        public int num_groups() { return (int)m_numgroups; }
        public int max_index() { return (int)(m_numcolors * m_numgroups + 2); }
        public uint32_t black_entry() { return m_numcolors * m_numgroups + 0; }
        public uint32_t white_entry() { return m_numcolors * m_numgroups + 1; }


        // overall adjustments
        //void set_brightness(float brightness);
        //void set_contrast(float contrast);
        //void set_gamma(float gamma);


        // entry getters
        public rgb_t entry_color(uint32_t index) { return index < m_numcolors ? m_entry_color[index] : rgb_t.black(); }
        //rgb_t entry_adjusted_color(UINT32 index) const { return (index < m_numcolors * m_numgroups) ? m_adjusted_color[index] : rgb_t::black; }
        public float entry_contrast(uint32_t index) { return index < m_numcolors ? m_entry_contrast[index] : 1.0f; }


        // entry setters

        //-------------------------------------------------
        //  entry_set_color - set the raw RGB color for a
        //  given palette index
        //-------------------------------------------------
        public void entry_set_color(uint32_t index, rgb_t rgb)
        {
            // if unchanged, ignore
            if (m_entry_color[index] == rgb)
                return;

            assert(index < m_numcolors);

            // set the color
            m_entry_color[index] = rgb;

            // update across all groups
            for (int groupnum = 0; groupnum < m_numgroups; groupnum++)
                update_adjusted_color((uint32_t)groupnum, index);
        }


        //void entry_set_red_level(UINT32 index, UINT8 level);
        //void entry_set_green_level(UINT32 index, UINT8 level);
        //void entry_set_blue_level(UINT32 index, UINT8 level);


        //-------------------------------------------------
        //  entry_set_contrast - set the contrast
        //  adjustment for a single palette index
        //-------------------------------------------------
        public void entry_set_contrast(uint32_t index, float contrast)
        {
            // if unchanged, ignore
            if (m_entry_contrast[index] == contrast)
                return;

            assert(index < m_numcolors);

            // set the contrast
            m_entry_contrast[index] = contrast;

            // update across all groups
            for (int groupnum = 0; groupnum < m_numgroups; groupnum++)
                update_adjusted_color((uint32_t)groupnum, index);
        }


        // entry list getters
        public MemoryContainer<rgb_t> entry_list_raw() { return m_entry_color; }  //const rgb_t *entry_list_raw() const { return m_entry_color; }
        public MemoryContainer<rgb_t> entry_list_adjusted() { return m_adjusted_color; }  //rgb_t *entry_list_adjusted() { return m_adjusted_color; }
        //const rgb_t *entry_list_adjusted_rgb15() const { return m_adjusted_rgb15; }


        // group adjustments

        //void group_set_brightness(UINT32 group, float brightness);

        //-------------------------------------------------
        //  group_set_contrast - configure overall
        //  contrast for a palette group
        //-------------------------------------------------
        public void group_set_contrast(uint32_t group, float contrast)
        {
            // if unchanged, ignore
            if (m_group_contrast[group] == contrast)
                return;

            assert(group < m_numgroups);

            // set the contrast
            m_group_contrast[group] = contrast;

            // update across all colors
            for (int index = 0; index < m_numcolors; index++)
                update_adjusted_color(group, (uint32_t)index);
        }


        // utilities
        public void normalize_range(uint32_t start, uint32_t end, int lum_min = 0, int lum_max = 255)
        {
            // clamp within range
            // start = std::max(start, 0U); ==> reduces to start = start
            end = std.min(end, m_numcolors - 1);

            // find the minimum and maximum brightness of all the colors in the range
            int32_t ymin = 1000 * 255;
            int32_t ymax = 0;
            for (uint32_t index = start; index <= end; index++)
            {
                rgb_t rgb = m_entry_color[index];
                uint32_t y = (uint32_t)(299 * rgb.r() + 587 * rgb.g() + 114 * rgb.b());
                ymin = (int32_t)std.min((uint32_t)ymin, y);
                ymax = (int32_t)std.max((uint32_t)ymax, y);
            }

            // determine target minimum/maximum
            int32_t tmin = (lum_min < 0) ? ((ymin + 500) / 1000) : lum_min;
            int32_t tmax = (lum_max < 0) ? ((ymax + 500) / 1000) : lum_max;

            // now normalize the palette
            for (uint32_t index = start; index <= end; index++)
            {
                rgb_t rgb = m_entry_color[index];
                int32_t y = 299 * rgb.r() + 587 * rgb.g() + 114 * rgb.b();
                int32_t u = ((int32_t)rgb.b()-y /1000)*492 / 1000;
                int32_t v = ((int32_t)rgb.r()-y / 1000)*877 / 1000;
                int32_t target = tmin + ((y - ymin) * (tmax - tmin + 1)) / (ymax - ymin);
                uint8_t r = rgb_t.clamp(target + 1140 * v / 1000);
                uint8_t g = rgb_t.clamp(target -  395 * u / 1000 - 581 * v / 1000);
                uint8_t b = rgb_t.clamp(target + 2032 * u / 1000);
                entry_set_color(index, new rgb_t(r, g, b));
            }
        }


        // construction/destruction
        //-------------------------------------------------
        //  palette_t - constructor
        //-------------------------------------------------
        palette_t(uint32_t numcolors, uint32_t numgroups = 1)
        {
            m_refcount = 1;
            m_numcolors = numcolors;
            m_numgroups = numgroups;
            m_brightness = 0.0f;
            m_contrast = 1.0f;
            m_gamma = 1.0f;
            m_entry_color = new std.vector<rgb_t>((int)numcolors);
            m_entry_contrast = new std.vector<float>((int)numcolors);
            m_adjusted_color = new std.vector<rgb_t>((int)(numcolors * numgroups + 2));
            m_adjusted_rgb15 = new std.vector<rgb15_t>((int)(numcolors * numgroups + 2));
            m_group_bright = new std.vector<float>((int)numgroups);
            m_group_contrast = new std.vector<float>((int)numgroups);
            m_client_list = null;


            // initialize gamma map
            for (uint32_t index = 0; index < 256; index++)
                m_gamma_map[index] = (uint8_t)index;

            // initialize the per-entry data
            for (uint32_t index = 0; index < numcolors; index++)
            {
                m_entry_color[index] = rgb_t.black();
                m_entry_contrast[index] = 1.0f;
            }

            // initialize the per-group data
            for (uint32_t index = 0; index < numgroups; index++)
            {
                m_group_bright[index] = 0.0f;
                m_group_contrast[index] = 1.0f;
            }

            // initialize the expanded data
            for (uint32_t index = 0; index < numcolors * numgroups; index++)
            {
                m_adjusted_color[index] = rgb_t.black();
                m_adjusted_rgb15[index] = rgb_t.black().as_rgb15();
            }

            // add black and white as the last two colors
            m_adjusted_color[numcolors * numgroups + 0] = rgb_t.black();
            m_adjusted_rgb15[numcolors * numgroups + 0] = rgb_t.black().as_rgb15();
            m_adjusted_color[numcolors * numgroups + 1] = rgb_t.white();
            m_adjusted_rgb15[numcolors * numgroups + 1] = rgb_t.white().as_rgb15();
        }


        // internal helpers

        //-------------------------------------------------
        //  adjust_palette_entry - adjust a palette
        //  entry for brightness
        //-------------------------------------------------
        rgb_t adjust_palette_entry(rgb_t entry, float brightness, float contrast, uint8_t [] gamma_map)
        {
            int r = rgb_t.clamp((int)((float)(gamma_map[entry.r()]) * contrast + brightness));
            int g = rgb_t.clamp((int)((float)(gamma_map[entry.g()]) * contrast + brightness));
            int b = rgb_t.clamp((int)((float)(gamma_map[entry.b()]) * contrast + brightness));
            int a = entry.a();
            return new rgb_t((uint8_t)a,(uint8_t)r,(uint8_t)g,(uint8_t)b);
        }

        /**
         * @fn  void palette_t::update_adjusted_color(UINT32 group, UINT32 index)
         *
         * @brief   -------------------------------------------------
         *            update_adjusted_color - update a color index by group and index pair
         *          -------------------------------------------------.
         *
         * @param   group   The group.
         * @param   index   Zero-based index of the.
         */
        void update_adjusted_color(uint32_t group, uint32_t index)
        {
            // compute the adjusted value
            rgb_t adjusted = adjust_palette_entry(m_entry_color[index],
                                                    m_group_bright[group] + m_brightness,
                                                    m_group_contrast[group] * m_entry_contrast[index] * m_contrast,
                                                    m_gamma_map);

            // if not different, ignore
            uint32_t finalindex = group * m_numcolors + index;
            if (m_adjusted_color[finalindex] == adjusted)
                return;

            // otherwise, modify the adjusted color array
            m_adjusted_color[finalindex] = adjusted;
            m_adjusted_rgb15[finalindex] = adjusted.as_rgb15();

            // mark dirty in all clients
            for (palette_client client = m_client_list; client != null; client = client.next())
                client.mark_dirty(finalindex);
        }
    }


    public static class palette_global
    {
        //-------------------------------------------------
        //  palexpand - expand a palette value to 8 bits
        //-------------------------------------------------
        //template<int _NumBits>
        public static uint8_t palexpand<int__NumBits>(uint8_t bits)
            where int__NumBits : int_const, new()
        {
            int _NumBits = new int__NumBits().value;

            if (_NumBits == 1) { return (bits & 1) != 0 ? (uint8_t)0xff : (uint8_t)0x00; }
            if (_NumBits == 2) { bits &= 3;    return (uint8_t)((bits << 6) | (bits << 4) | (bits << 2) | bits); }
            if (_NumBits == 3) { bits &= 7;    return (uint8_t)((bits << 5) | (bits << 2) | (bits >> 1)); }
            if (_NumBits == 4) { bits &= 0xf;  return (uint8_t)((bits << 4) | bits); }
            if (_NumBits == 5) { bits &= 0x1f; return (uint8_t)((bits << 3) | (bits >> 2)); }
            if (_NumBits == 6) { bits &= 0x3f; return (uint8_t)((bits << 2) | (bits >> 4)); }
            if (_NumBits == 7) { bits &= 0x7f; return (uint8_t)((bits << 1) | (bits >> 6)); }
            return bits;
        }


        //-------------------------------------------------
        //  palxbit - convert an x-bit value to 8 bits
        //-------------------------------------------------

        public static uint8_t pal1bit(uint8_t bits) { return palexpand<int_const_1>(bits); }
        static uint8_t pal2bit(uint8_t bits) { return palexpand<int_const_2>(bits); }
        static uint8_t pal3bit(uint8_t bits) { return palexpand<int_const_3>(bits); }
        static uint8_t pal4bit(uint8_t bits) { return palexpand<int_const_4>(bits); }
        public static uint8_t pal5bit(uint8_t bits) { return palexpand<int_const_5>(bits); }
        static uint8_t pal6bit(uint8_t bits) { return palexpand<int_const_6>(bits); }
        static uint8_t pal7bit(uint8_t bits) { return palexpand<int_const_7>(bits); }


        //-------------------------------------------------
        //  rgbexpand - expand a 32-bit raw data to 8-bit
        //  RGB
        //-------------------------------------------------
        //template<int _RBits, int _GBits, int _BBits>
        public static rgb_t rgbexpand<int__RBits, int__GBits, int__BBits>(uint32_t data, uint8_t rshift, uint8_t gshift, uint8_t bshift)
            where int__RBits : int_const, new()
            where int__GBits : int_const, new()
            where int__BBits : int_const, new()
        {
            return new rgb_t(palexpand<int__RBits>((uint8_t)(data >> rshift)), palexpand<int__GBits>((uint8_t)(data >> gshift)), palexpand<int__BBits>((uint8_t)(data >> bshift)));
        }

        //template<int _ABits, int _RBits, int _GBits, int _BBits>
        //constexpr rgb_t argbexpand(uint32_t data, uint8_t ashift, uint8_t rshift, uint8_t gshift, uint8_t bshift)
    }
}
