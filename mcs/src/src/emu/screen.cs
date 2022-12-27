// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using attoseconds_t = System.Int64;  //typedef s64 attoseconds_t;
using devcb_write32 = mame.devcb_write<mame.Type_constant_u32>;  //using devcb_write32 = devcb_write<u32>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_timer_id = System.UInt32;  //typedef u32 device_timer_id;
using optional_memory_region = mame.memory_region_finder<mame.bool_const_false>;  //using optional_memory_region = memory_region_finder<false>;
using s8  = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using screen_device_enumerator = mame.device_type_enumerator<mame.screen_device>;  //typedef device_type_enumerator<screen_device> screen_device_enumerator;
using u8  = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt32;

using static mame.attotime_global;
using static mame.cpp_global;
using static mame.device_global;
using static mame.emucore_global;
using static mame.osdcore_global;
using static mame.profiler_global;
using static mame.render_global;
using static mame.rendertypes_global;
using static mame.rendutil_global;
using static mame.screen_global;


namespace mame
{
    // screen types
    public enum screen_type_enum
    {
        SCREEN_TYPE_INVALID = 0,
        SCREEN_TYPE_RASTER,
        SCREEN_TYPE_VECTOR,
        SCREEN_TYPE_LCD,
        SCREEN_TYPE_SVG
    }


    static partial class screen_global
    {
        public const screen_type_enum SCREEN_TYPE_RASTER = screen_type_enum.SCREEN_TYPE_RASTER;
        public const screen_type_enum SCREEN_TYPE_VECTOR = screen_type_enum.SCREEN_TYPE_VECTOR;
        public const screen_type_enum SCREEN_TYPE_LCD = screen_type_enum.SCREEN_TYPE_LCD;
        public const screen_type_enum SCREEN_TYPE_SVG = screen_type_enum.SCREEN_TYPE_SVG;
        public const screen_type_enum SCREEN_TYPE_INVALID = screen_type_enum.SCREEN_TYPE_INVALID;


        // screen_update callback flags
        public const u32 UPDATE_HAS_NOT_CHANGED = 0x0001;   // the video has not changed


        // ----- flags for video_attributes -----

        /*!
         @defgroup flags for video_attributes
         @{
         @def VIDEO_UPDATE_BEFORE_VBLANK
         update_video called at the start of the VBLANK period
         @todo hack, remove me

         @def VIDEO_UPDATE_AFTER_VBLANK
         update_video called at the end of the VBLANK period
         @todo hack, remove me

         @def VIDEO_SELF_RENDER
         indicates VIDEO_UPDATE will add container bits itself

         @def VIDEO_ALWAYS_UPDATE
         force VIDEO_UPDATE to be called even for skipped frames.
         @todo in case you need this one for model updating, then you're doing it wrong (read: hack)

         @def VIDEO_UPDATE_SCANLINE
         calls VIDEO_UPDATE for every visible scanline, even for skipped frames

         @def VIDEO_VARIABLE_WIDTH
         causes the screen to construct its final bitmap from a composite upscale of individual scanline bitmaps

         @}
         */

        public const u32 VIDEO_UPDATE_BEFORE_VBLANK      = 0x0000;
        public const u32 VIDEO_UPDATE_AFTER_VBLANK       = 0x0004;

        public const u32 VIDEO_SELF_RENDER               = 0x0008;
        public const u32 VIDEO_ALWAYS_UPDATE             = 0x0080;
        public const u32 VIDEO_UPDATE_SCANLINE           = 0x0100;
        public const u32 VIDEO_VARIABLE_WIDTH            = 0x0200;
    }


    // ======================> screen_bitmap
    class screen_bitmap
    {
        // internal state
        bitmap_format m_format;
        texture_format m_texformat;
        bitmap_t m_live;
        bitmap_ind16 m_ind16 = new bitmap_ind16();
        bitmap_rgb32 m_rgb32 = new bitmap_rgb32();


        // construction/destruction
        public screen_bitmap()
        {
            m_format = bitmap_format.BITMAP_FORMAT_RGB32;
            m_texformat = texture_format.TEXFORMAT_RGB32;
            m_live = m_rgb32;
        }

        public screen_bitmap(bitmap_ind16 orig)
        {
            m_format = bitmap_format.BITMAP_FORMAT_IND16;
            m_texformat = texture_format.TEXFORMAT_PALETTE16;
            m_live = m_ind16;
            m_ind16 = new bitmap_ind16(orig, orig.cliprect());
        }

        public screen_bitmap(bitmap_rgb32 orig)
        {
            m_format = bitmap_format.BITMAP_FORMAT_RGB32;
            m_texformat = texture_format.TEXFORMAT_RGB32;
            m_live = m_rgb32;
            m_rgb32 = new bitmap_rgb32(orig, orig.cliprect());
        }


        // internal helpers
        public bitmap_t live() { /*assert(m_live != NULL);*/  return m_live; }
        //const bitmap_t &live() const { assert(m_live != NULL); return *m_live; }


        // resizing
        void resize(int width, int height) { live().resize(width, height); }


        // conversion
        //operator bitmap_t &() { return live(); }
        public bitmap_ind16 as_ind16() { /*assert(m_format == BITMAP_FORMAT_IND16);*/ return m_ind16; }
        public bitmap_rgb32 as_rgb32() { /*assert(m_format == BITMAP_FORMAT_RGB32);*/ return m_rgb32; }


        // getters
        public int width() { return live().width(); }
        public int height() { return live().height(); }
        //INT32 rowpixels() const { return live().rowpixels(); }
        //INT32 rowbytes() const { return live().rowbytes(); }
        //UINT8 bpp() const { return live().bpp(); }
        public bitmap_format format() { return m_format; }
        public texture_format texformat() { return m_texformat; }
        public bool valid() { return live().valid(); }
        //palette_t *palette() const { return live().palette(); }
        public rectangle cliprect() { return live().cliprect(); }


        // operations
        public void set_palette(palette_t palette) { live().set_palette(palette); }

        public void set_format(bitmap_format format, texture_format texformat)
        {
            m_format = format;
            m_texformat = texformat;
            switch (format)
            {
                case bitmap_format.BITMAP_FORMAT_IND16:   m_live = m_ind16;  break;
                case bitmap_format.BITMAP_FORMAT_RGB32:   m_live = m_rgb32;  break;
                default:                                  m_live = null;     break;
            }
            m_ind16.reset();
            m_rgb32.reset();
        }
    }


    // ======================> other delegate types

    public delegate void vblank_state_delegate(screen_device device, bool vblank_state);  //typedef delegate<void (screen_device &, bool)> vblank_state_delegate;

    public delegate u32 screen_update_ind16_delegate(screen_device device, bitmap_ind16 bitmap, rectangle rect);  //typedef device_delegate<u32 (screen_device &, bitmap_ind16 &, const rectangle &)> screen_update_ind16_delegate;
    public delegate u32 screen_update_rgb32_delegate(screen_device device, bitmap_rgb32 bitmap, rectangle rect);  //typedef device_delegate<u32 (screen_device &, bitmap_rgb32 &, const rectangle &)> screen_update_rgb32_delegate;


    // ======================> screen_device
    public class screen_device : device_t
    {
        //DEFINE_DEVICE_TYPE(SCREEN, screen_device, "screen", "Video Screen")
        public static readonly emu.detail.device_type_impl SCREEN = DEFINE_DEVICE_TYPE("screen", "Video Screen", (type, mconfig, tag, owner, clock) => { return new screen_device(mconfig, tag, owner, clock); });


        const bool VERBOSE = false;
        void LOG_PARTIAL_UPDATES(string format, params object [] args) { if (VERBOSE) logerror(format, args); }


        // timer IDs
        //enum
        //{
        const int TID_VBLANK_START = 0;
        const int TID_VBLANK_END   = 1;
        const int TID_SCANLINE0    = 2;
        const int TID_SCANLINE     = 3;
        //}


        // VBLANK callbacks
        class callback_item
        {
            public vblank_state_delegate m_callback;


            public callback_item(vblank_state_delegate callback)
            {
                m_callback = callback;
            }
        }


        // auto-sizing bitmaps
        class auto_bitmap_item
        {
            public bitmap_t m_bitmap;

            public auto_bitmap_item(bitmap_t bitmap)
            {
                m_bitmap = bitmap;
            }
        }


        // globally accessible constants
        const int DEFAULT_FRAME_RATE = 60;
        public static readonly attotime DEFAULT_FRAME_PERIOD = attotime.from_hz(DEFAULT_FRAME_RATE);


        // static data
        static u32 m_id_counter; // incremented for each constructed screen_device,
                                 // used as a unique identifier during runtime


        // inline configuration data
        screen_type_enum m_type;                     // type of screen
        int m_orientation;              // orientation flags combined with system flags
        std.pair<unsigned, unsigned> m_phys_aspect;  //std::pair<unsigned, unsigned> m_phys_aspect;    // physical aspect ratio
        bool m_oldstyle_vblank_supplied; // set_vblank_time call used
        attoseconds_t m_refresh;                  // default refresh period
        attoseconds_t m_vblank;                   // duration of a VBLANK
        float m_xoffset;
        float m_yoffset;       // default X/Y offsets
        float m_xscale;
        float m_yscale;         // default X/Y scale factor
        screen_update_ind16_delegate m_screen_update_ind16; // screen update callback (16-bit palette)
        screen_update_rgb32_delegate m_screen_update_rgb32; // screen update callback (32-bit RGB)
        devcb_write_line m_screen_vblank;         // screen vblank line callback
        devcb_write32 m_scanline_cb;              // screen scanline callback
        optional_device<device_palette_interface> m_palette;  //optional_device<device_palette_interface> m_palette;      // our palette
        u32 m_video_attributes;         // flags describing the video system
        optional_memory_region m_svg_region;               // the region in which the svg data is in

        // internal state
        render_container m_container;                // pointer to our container
        //std::unique_ptr<svg_renderer> m_svg; // the svg renderer

        // dimensions
        int m_max_width;                // maximum width encountered
        int m_width;                    // current width (HTOTAL)
        int m_height;                   // current height (VTOTAL)
        rectangle m_visarea;                  // current visible area (HBLANK end/start, VBLANK end/start)
        std.vector<int> m_scan_widths;              // current width, in samples, of each individual scanline

        // textures and bitmaps
        texture_format m_texformat;                // texture format
        render_texture [] m_texture = new render_texture[2];               // 2x textures for the screen bitmap
        screen_bitmap [] m_bitmap = new screen_bitmap[2];                // 2x bitmaps for rendering
        std.vector<bitmap_t> [] m_scan_bitmaps = new std.vector<bitmap_t>[2];      // 2x bitmaps for each individual scanline
        bitmap_ind8 m_priority = new bitmap_ind8();                 // priority bitmap
        bitmap_ind64 m_burnin = new bitmap_ind64();                   // burn-in bitmap
        u8 m_curbitmap;                // current bitmap index
        u8 m_curtexture;               // current texture index
        bool m_changed;                  // has this bitmap changed?
        s32 m_last_partial_scan;        // scanline of last partial update
        s32 m_partial_scan_hpos;        // horizontal pixel last rendered on this partial scanline
        //bitmap_argb32       m_screen_overlay_bitmap;    // screen overlay bitmap
        u32 m_unique_id;                // unique id for this screen_device
        rgb_t m_color;                    // render color
        u8 m_brightness;               // global brightness

        // screen timing
        attoseconds_t m_frame_period;             // attoseconds per frame
        attoseconds_t m_scantime;                 // attoseconds per scanline
        attoseconds_t m_pixeltime;                // attoseconds per pixel
        attoseconds_t m_vblank_period;            // attoseconds per VBLANK period
        attotime m_vblank_start_time;        // time of last VBLANK start
        attotime m_vblank_end_time;          // time of last VBLANK end
        emu_timer m_vblank_begin_timer;       // timer to signal VBLANK start
        emu_timer m_vblank_end_timer;         // timer to signal VBLANK end
        emu_timer m_scanline0_timer;          // scanline 0 timer
        emu_timer m_scanline_timer;           // scanline timer
        u64 m_frame_number;             // the current frame number
        u32 m_partial_updates_this_frame;// partial update counter this frame

        bool m_is_primary_screen;

        std.vector<callback_item> m_callback_list = new std.vector<callback_item>();     // list of VBLANK callbacks
        std.vector<auto_bitmap_item> m_auto_bitmap_list = new std.vector<auto_bitmap_item>(); // list of registered bitmaps


        // construction/destruction

        //-------------------------------------------------
        //  screen_device - constructor
        //-------------------------------------------------
        public screen_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, SCREEN, tag, owner, clock)
        {
            m_type = screen_type_enum.SCREEN_TYPE_RASTER;
            m_orientation = ROT0;
            m_phys_aspect = new std.pair<unsigned, unsigned>(0U, 0U);
            m_oldstyle_vblank_supplied = false;
            m_refresh = 0;
            m_vblank = 0;
            m_xoffset = 0.0f;
            m_yoffset = 0.0f;
            m_xscale = 1.0f;
            m_yscale = 1.0f;
            m_screen_update_ind16 = null;
            m_screen_update_rgb32 = null;
            m_screen_vblank = new devcb_write_line(this);
            m_scanline_cb = new devcb_write32(this);
            m_palette = new optional_device<device_palette_interface>(this, finder_base.DUMMY_TAG);
            m_video_attributes = 0;
            m_svg_region = new optional_memory_region(this, DEVICE_SELF);
            m_container = null;
            m_max_width = 100;
            m_width = 100;
            m_height = 100;
            m_visarea = new rectangle(0, 99, 0, 99);
            m_texformat = texture_format.TEXFORMAT_UNDEFINED;
            m_curbitmap = 0;
            m_curtexture = 0;
            m_changed = true;
            m_last_partial_scan = 0;
            m_partial_scan_hpos = -1;
            m_color = new rgb_t(0xff, 0xff, 0xff, 0xff);
            m_brightness = 0xff;
            m_frame_period = DEFAULT_FRAME_PERIOD.as_attoseconds();
            m_scantime = 1;
            m_pixeltime = 1;
            m_vblank_period = 0;
            m_vblank_start_time = attotime.zero;
            m_vblank_end_time = attotime.zero;
            m_vblank_begin_timer = null;
            m_vblank_end_timer = null;
            m_scanline0_timer = null;
            m_scanline_timer = null;
            m_frame_number = 0;
            m_partial_updates_this_frame = 0;


            for (int i = 0; i < m_bitmap.Length; i++)
                m_bitmap[i] = new screen_bitmap();


            m_unique_id = m_id_counter;
            m_id_counter++;
            //memset(m_texture, 0, sizeof(m_texture));
        }


        public screen_device(machine_config mconfig, string tag, device_t owner, screen_type_enum type)
            : this(mconfig, tag, owner, (u32)0)
        {
            set_type(type);
        }


        public screen_device(machine_config mconfig, string tag, device_t owner, screen_type_enum type, rgb_t color)
            : this(mconfig, tag, owner, (u32)0)
        {
            set_type(type);
            set_color(color);
        }


        public void screen_device_after_ctor(screen_type_enum type)
        {
            set_type(type);
        }

        //~screen_device() { destroy_scan_bitmaps(); }


        // configuration readers
        public screen_type_enum screen_type() { return m_type; }
        public int orientation() { assert(configured()); return m_orientation; }

        //-------------------------------------------------
        //  physical_aspect - determine the physical
        //  aspect ratio to be used for rendering
        //-------------------------------------------------
        public std.pair<unsigned, unsigned> physical_aspect()  //std::pair<unsigned, unsigned> physical_aspect() const;
        {
            assert(configured());

            std.pair<unsigned, unsigned> phys_aspect = m_phys_aspect;

            // physical aspect ratio unconfigured
            if (phys_aspect.first == 0 || phys_aspect.second == 0)
            {
                switch (m_type)
                {
                case SCREEN_TYPE_RASTER:
                case SCREEN_TYPE_VECTOR:
                    phys_aspect = std.make_pair(4U, 3U); // assume standard CRT
                    break;
                case SCREEN_TYPE_LCD:
                case SCREEN_TYPE_SVG:
                    phys_aspect = std.make_pair(~0U, ~0U); // assume square pixels
                    break;
                case SCREEN_TYPE_INVALID:
                default:
                    throw new emu_fatalerror("{0}: invalid screen type configured\n", tag());
                }
            }

            // square pixels?
            if ((~0U == phys_aspect.first) && (~0U == phys_aspect.second))
            {
                //phys_aspect.first = visible_area().width();
                //phys_aspect.second = visible_area().height();
                phys_aspect = new std.pair<unsigned, unsigned>((unsigned)visible_area().width(), (unsigned)visible_area().height());
            }

            // always keep this in reduced form
            unsigned tempFirst = phys_aspect.first;
            unsigned tempSecond = phys_aspect.second;
            util.reduce_fraction(ref tempFirst, ref tempSecond);
            phys_aspect = new std.pair<unsigned, unsigned>(tempFirst, tempSecond);

            return phys_aspect;
        }

        public int width() { return m_width; }
        public int height() { return m_height; }
        public rectangle visible_area() { return m_visarea; }
        public rectangle cliprect() { return m_bitmap[0].cliprect(); }
        bool oldstyle_vblank_supplied() { return m_oldstyle_vblank_supplied; }
        public attoseconds_t refresh_attoseconds() { return m_refresh; }
        attoseconds_t vblank_attoseconds() { return m_vblank; }
        public bitmap_format format() { return m_screen_update_ind16 != null ? bitmap_format.BITMAP_FORMAT_IND16 : bitmap_format.BITMAP_FORMAT_RGB32; }
        public float xoffset() { return m_xoffset; }
        public float yoffset() { return m_yoffset; }
        public float xscale() { return m_xscale; }
        public float yscale() { return m_yscale; }
        public bool has_screen_update() { return m_screen_update_ind16 != null || m_screen_update_rgb32 != null; }


        // inline configuration helpers

        public void set_type(screen_type_enum type) { assert(!configured()); m_type = type; }
        //void set_orientation(int orientation) { assert(!configured()); m_orientation = orientation; }
        //void set_physical_aspect(unsigned x, unsigned y) { assert(!configured()); m_phys_aspect = std::make_pair(x, y); }
        //void set_native_aspect() { assert(!configured()); m_phys_aspect = std::make_pair(~0U, ~0U); }

        /// \brief Configure screen parameters
        ///
        /// \param [in] pixclock Pixel clock frequency in Hertz.
        /// \param [in] htotal Total pixel clocks per line, including
        ///   horizontal blanking period.
        /// \param [in] hbend Index of first visible pixel after horizontal
        ///   blanking period ends.
        /// \param [in] hbstart Index of first pixel in horzontal blanking
        ///   period after visible pixels.
        /// \param [in] vtotal Total lines per frame, including vertical
        ///   blanking period.
        /// \param [in] vbend Index of first visible line after vertical
        ///   blanking period ends.
        /// \param [in] vbstart Index of first line in vertical blanking
        ///   period after visible lines.
        /// \return Reference to device for method chaining.
        public screen_device set_raw(u32 pixclock, u16 htotal, u16 hbend, u16 hbstart, u16 vtotal, u16 vbend, u16 vbstart)
        {
            assert(pixclock != 0);
            clock_set(pixclock);
            m_refresh = HZ_TO_ATTOSECONDS(pixclock) * htotal * vtotal;
            m_vblank = m_refresh / vtotal * (vtotal - (vbstart - vbend));
            m_width = htotal;
            m_height = vtotal;
            m_visarea.set(hbend, hbstart != 0 ? hbstart - 1 : htotal - 1, vbend, vbstart - 1);
            return this;
        }

        public screen_device set_raw(XTAL xtal, u16 htotal, u16 hbend, u16 hbstart, u16 vtotal, u16 vbend, u16 vbstart)
        {
            xtal.validate("Configuring screen " + tag());
            return set_raw(xtal.value(), htotal, hbend, hbstart, vtotal, vbend, vbstart);
        }

        public screen_device set_raw(XTAL xtal, u16 htotal, u16 vtotal, rectangle visarea)
        {
            return set_raw(xtal, htotal, (u16)visarea.left(), (u16)(visarea.right() + 1), vtotal, (u16)visarea.top(), (u16)(visarea.bottom() + 1));
        }

        public void set_refresh(attoseconds_t rate) { m_refresh = rate; }

        /// \brief Set refresh rate in Hertz
        ///
        /// Sets refresh rate in Hertz (frames per second). Used in
        /// conjunction with #set_vblank_time, #set_size and #set_visarea.
        /// For raster displays, please use #set_raw to configure screen
        /// parameters in terms of pixel clock.
        /// \param [in] hz Desired refresh rate.
        /// \return Reference to device for method chaining.
        /// 
        public screen_device set_refresh_hz(s32 hz) { set_refresh(HZ_TO_ATTOSECONDS(hz)); return this; }  //template <typename T> screen_device &set_refresh_hz(T &&hz) { set_refresh(HZ_TO_ATTOSECONDS(std::forward<T>(hz))); return *this; }
        public screen_device set_refresh_hz(XTAL hz) { set_refresh(HZ_TO_ATTOSECONDS(hz)); return this; }  //template <typename T> screen_device &set_refresh_hz(T &&hz) { set_refresh(HZ_TO_ATTOSECONDS(std::forward<T>(hz))); return *this; }

        /// \brief Set vertical blanking interval time
        ///
        /// Sets vertical blanking interval period.  Used in conjunction
        /// with #set_refresh_hz, #set_size and #set_visarea.  For raster
        /// displays, please use #set_raw to configure screen parameters in
        /// terms of pixel clock.
        /// \param [in] time Length of vertical blanking interval.
        /// \return Reference to device for method chaining.
        public screen_device set_vblank_time(attoseconds_t time)
        {
            m_vblank = time;
            m_oldstyle_vblank_supplied = true;
            return this;
        }

        /// \brief Set total screen size
        ///
        /// Set the total screen size in pixels, including blanking areas if
        /// applicable.  This sets the size of the screen bitmap.  Used in
        /// conjunction with #set_refresh_hz, #set_vblank_time and
        /// #set_visarea.  For raster displays, please use #set_raw to
        /// configure screen parameters in terms of pixel clock.
        /// \param [in] width Total width in pixels, including horizontal
        ///   blanking period if applicable.
        /// \param [in] height Total height in lines, including vertical
        ///   blanking period if applicable.
        /// \return Reference to device for method chaining.
        public screen_device set_size(u16 width, u16 height)
        {
            m_width = width;
            m_height = height;
            return this;
        }

        /// \brief Set visible screen area
        ///
        /// Set visible screen area.  This should fit within the total
        /// screen area.  Used in conjunction with #set_refresh_hz,
        /// #set_vblank_time and #set_size.  For raster displays, please
        /// use #set_raw to configure screen parameters in terms of pixel
        /// clock.
        /// \param [in] minx First visible pixel index after horizontal
        ///   blanking period ends.
        /// \param [in] maxx Last visible pixel index before horizontal
        ///   blanking period starts.
        /// \param [in] miny First visible line index after vertical
        ///   blanking period ends.
        /// \param [in] maxy Last visible line index before vertical
        ///   blanking period starts.
        /// \return Reference to device for method chaining.
        public screen_device set_visarea(s16 minx, s16 maxx, s16 miny, s16 maxy) { m_visarea.set(minx, maxx, miny, maxy); return this; }

        /// \brief Set visible area to full area
        ///
        /// Set visible screen area to the full screen area (i.e. no
        /// horizontal or vertical blanking period).  This is generally not
        /// possible for raster displays, but is useful for other display
        /// simulations.  Must be called after calling #set_size.
        /// \return Reference to device for method chaining.
        /// \sa set_visarea
        screen_device set_visarea_full() { m_visarea.set(0, m_width - 1, 0, m_height - 1); return this; }

        void set_default_position(double xscale, double xoffs, double yscale, double yoffs)
        {
            m_xscale = (float)xscale;
            m_xoffset = (float)xoffs;
            m_yscale = (float)yscale;
            m_yoffset = (float)yoffs;
        }


        //template <typename F>
        //std::enable_if_t<screen_update_ind16_delegate::supports_callback<F>::value> set_screen_update(F &&callback, const char *name)
        //{
        //    m_screen_update_ind16.set(std::forward<F>(callback), name);
        //    m_screen_update_rgb32 = screen_update_rgb32_delegate(*this);
        //}
        //template <typename F>
        //std::enable_if_t<screen_update_rgb32_delegate::supports_callback<F>::value> set_screen_update(F &&callback, const char *name)
        //{
        //    m_screen_update_ind16 = screen_update_ind16_delegate(*this);
        //    m_screen_update_rgb32.set(std::forward<F>(callback), name);
        //}
        //template <typename T, typename F>
        //std::enable_if_t<screen_update_ind16_delegate::supports_callback<F>::value> set_screen_update(T &&target, F &&callback, const char *name)
        //{
        //    m_screen_update_ind16.set(std::forward<T>(target), std::forward<F>(callback), name);
        //    m_screen_update_rgb32 = screen_update_rgb32_delegate(*this);
        //}
        //template <typename T, typename F>
        //std::enable_if_t<screen_update_rgb32_delegate::supports_callback<F>::value> set_screen_update(T &&target, F &&callback, const char *name)
        //{
        //    m_screen_update_ind16 = screen_update_ind16_delegate(*this);
        //    m_screen_update_rgb32.set(std::forward<T>(target), std::forward<F>(callback), name);
        //}

        public void set_screen_update(string tag, screen_update_ind16_delegate callback) { set_screen_update(callback); }
        public void set_screen_update(screen_update_ind16_delegate callback)
        {
            m_screen_update_ind16 = callback;
            m_screen_update_rgb32 = null;
        }

        public void set_screen_update(string tag, screen_update_rgb32_delegate callback) { set_screen_update(callback); }
        public void set_screen_update(screen_update_rgb32_delegate callback)
        {
            m_screen_update_ind16 = null;
            m_screen_update_rgb32 = callback;
        }


        public devcb_write_line.binder screen_vblank() { return m_screen_vblank.bind(); }  //auto screen_vblank() { return m_screen_vblank.bind(); }
        //auto scanline() { m_video_attributes |= VIDEO_UPDATE_SCANLINE; return m_scanline_cb.bind(); }

        //template<typename T>
        public screen_device set_palette(string tag) { m_palette.set_tag(tag); return this; }
        public screen_device set_palette(finder_base finder) { m_palette.set_tag(finder); return this; }

        //screen_device &set_no_palette() { m_palette.set_tag(finder_base::DUMMY_TAG); return *this; }
        public screen_device set_video_attributes(u32 flags) { m_video_attributes = flags; return this; }
        screen_device set_color(rgb_t color) { m_color = color; return this; }
        //template <typename T> screen_device &set_svg_region(T &&tag) { m_svg_region.set_tag(std::forward<T>(tag)); return *this; } // default region is device tag


        // information getters
        public render_container container() { assert(m_container != null); return m_container; }
        public bitmap_ind8 priority() { return m_priority; }
        public device_palette_interface palette() { assert(m_palette != null); return m_palette.op0; }
        public bool has_palette() { return m_palette != null; }
        //screen_bitmap &curbitmap() { return m_bitmap[m_curtexture]; }


        // dynamic configuration

        //-------------------------------------------------
        //  configure - configure screen parameters
        //-------------------------------------------------
        public void configure(int width, int height, rectangle visarea, attoseconds_t frame_period)
        {
            // validate arguments
            assert(width > 0);
            assert(height > 0);
            assert(visarea.left() >= 0);
            assert(visarea.top() >= 0);
            //global.assert(visarea.right() < width);
            //global.assert(visarea.bottom() < height);
            assert(m_type == screen_type_enum.SCREEN_TYPE_VECTOR || m_type == screen_type_enum.SCREEN_TYPE_SVG || visarea.left() < width);
            assert(m_type == screen_type_enum.SCREEN_TYPE_VECTOR || m_type == screen_type_enum.SCREEN_TYPE_SVG || visarea.top() < height);
            assert(frame_period > 0);

            // fill in the new parameters
            m_max_width = std.max(m_max_width, width);
            m_width = width;
            m_height = height;
            m_visarea = visarea;

            // reallocate bitmap(s) if necessary
            realloc_screen_bitmaps();

            // compute timing parameters
            m_frame_period = frame_period;
            m_scantime = frame_period / height;
            m_pixeltime = frame_period / (height * width);

            // if an old style VBLANK_TIME was specified in the MACHINE_CONFIG,
            // use it; otherwise calculate the VBLANK period from the visible area
            if (m_oldstyle_vblank_supplied)
                m_vblank_period = m_vblank;
            else
                m_vblank_period = m_scantime * (height - visarea.height());

            // we are now fully configured with the new parameters
            // and can safely call time_until_pos(), etc.

            // if the frame period was reduced so that we are now past the end of the frame,
            // call the VBLANK start timer now; otherwise, adjust it for the future
            attoseconds_t delta = (machine().time() - m_vblank_start_time).as_attoseconds();
            if (delta >= m_frame_period)
                vblank_begin();
            else
                m_vblank_begin_timer.adjust(time_until_vblank_start());

            // if we are on scanline 0 already, call the scanline 0 timer
            // by hand now; otherwise, adjust it for the future
            if (vpos() == 0)
                reset_partial_updates();
            else
                m_scanline0_timer.adjust(time_until_pos(0));

            // adjust speed if necessary
            machine().video().update_refresh_speed();
        }


        //-------------------------------------------------
        //  reset_origin - reset the timing such that the
        //  given (x,y) occurs at the current time
        //-------------------------------------------------
        public void reset_origin(int beamy = 0, int beamx = 0)
        {
            // compute the effective VBLANK start/end times
            attotime curtime = machine().time();
            m_vblank_end_time = curtime - new attotime(0, beamy * m_scantime + beamx * m_pixeltime);
            m_vblank_start_time = m_vblank_end_time - new attotime(0, m_vblank_period);

            // if we are resetting relative to (0,0) == VBLANK end, call the
            // scanline 0 timer by hand now; otherwise, adjust it for the future
            if (beamy == 0 && beamx == 0)
                reset_partial_updates();
            else
                m_scanline0_timer.adjust(time_until_pos(0));

            // if we are resetting relative to (visarea.bottom() + 1, 0) == VBLANK start,
            // call the VBLANK start timer now; otherwise, adjust it for the future
            if (beamy == ((m_visarea.bottom() + 1) % m_height) && beamx == 0)
                vblank_begin();
            else
                m_vblank_begin_timer.adjust(time_until_vblank_start());
        }


        //void set_visible_area(int min_x, int max_x, int min_y, int max_y);
        //void set_brightness(UINT8 brightness) { m_brightness = brightness; }


        // beam positioning and state

        //-------------------------------------------------
        //  vpos - returns the current vertical position
        //  of the beam
        //-------------------------------------------------
        public int vpos()
        {
            attoseconds_t delta = (machine().time() - m_vblank_start_time).as_attoseconds();
            int vpos;

            // round to the nearest pixel
            delta += m_pixeltime / 2;

            // compute the v position relative to the start of VBLANK
            vpos = (int)(delta / m_scantime);

            // adjust for the fact that VBLANK starts at the bottom of the visible area
            return (m_visarea.bottom() + 1 + vpos) % m_height;
        }


        //-------------------------------------------------
        //  hpos - returns the current horizontal position
        //  of the beam
        //-------------------------------------------------
        public int hpos()
        {
            attoseconds_t delta = (machine().time() - m_vblank_start_time).as_attoseconds();

            // round to the nearest pixel
            delta += m_pixeltime / 2;

            // compute the v position relative to the start of VBLANK
            int vpos = (int)(delta / m_scantime);

            // subtract that from the total time
            delta -= vpos * m_scantime;

            // return the pixel offset from the start of this scanline
            return (int)(delta / m_pixeltime);
        }


        public int vblank() { return (machine().time() < m_vblank_end_time) ? 1 : 0; }  //DECLARE_READ_LINE_MEMBER(vblank) const { return (machine().time() < m_vblank_end_time) ? 1 : 0; }
        //DECLARE_READ_LINE_MEMBER(hblank) const { int const curpos = hpos(); return (curpos < m_visarea.left() || curpos > m_visarea.right()) ? 1 : 0; }


        // timing

        //-------------------------------------------------
        //  time_until_pos - returns the amount of time
        //  remaining until the beam is at the given
        //  hpos,vpos
        //-------------------------------------------------
        public attotime time_until_pos(int vpos, int hpos = 0)
        {
            // validate arguments
            assert(vpos >= 0);
            assert(hpos >= 0);

            // since we measure time relative to VBLANK, compute the scanline offset from VBLANK
            vpos += m_height - (m_visarea.bottom() + 1);
            vpos %= m_height;

            // compute the delta for the given X,Y position
            attoseconds_t targetdelta = (attoseconds_t)vpos * m_scantime + (attoseconds_t)hpos * m_pixeltime;

            // if we're past that time (within 1/2 of a pixel), head to the next frame
            attoseconds_t curdelta = (machine().time() - m_vblank_start_time).as_attoseconds();
            if (targetdelta <= curdelta + m_pixeltime / 2)
                targetdelta += m_frame_period;
            while (targetdelta <= curdelta)
                targetdelta += m_frame_period;

            // return the difference
            return new attotime(0, targetdelta - curdelta);
        }

        attotime time_until_vblank_start() { return time_until_pos(m_visarea.bottom() + 1); }

        //-------------------------------------------------
        //  time_until_vblank_end - returns the amount of
        //  time remaining until the end of the current
        //  VBLANK (if in progress) or the end of the next
        //  VBLANK
        //-------------------------------------------------
        attotime time_until_vblank_end()
        {
            // if we are in the VBLANK region, compute the time until the end of the current VBLANK period
            attotime target_time = m_vblank_end_time;
            if (vblank() == 0)
                target_time += new attotime(0, m_frame_period);
            return target_time - machine().time();
        }

        //attotime time_until_update() const { return (m_video_attributes & VIDEO_UPDATE_AFTER_VBLANK) ? time_until_vblank_end() : time_until_vblank_start(); }
        //attotime scan_period() const { return attotime(0, m_scantime); }
        //attotime pixel_period() const { return attotime(0, m_pixeltime); }
        public attotime frame_period() { return (this == null) ? DEFAULT_FRAME_PERIOD : new attotime(0, m_frame_period); }
        public u64 frame_number() { return m_frame_number; }


        // pixel-level access
        //u32 pixel(s32 x, s32 y);
        //void pixels(u32* buffer);


        // updating
        public int partial_updates() { return (int)m_partial_updates_this_frame; }


        public int partial_scan_hpos() { return m_partial_scan_hpos; }


        //-------------------------------------------------
        //  update_partial - perform a partial update from
        //  the last scanline up to and including the
        //  specified scanline
        //-----------------------------------------------*/
        public bool update_partial(int scanline)
        {
            if (machine().video().frame_update_count() % 400 == 0)
            {
                LOG_PARTIAL_UPDATES("Partial: update_partial({0}, {1}): ", tag(), scanline);
            }

            // these two checks only apply if we're allowed to skip frames
            if ((m_video_attributes & VIDEO_ALWAYS_UPDATE) == 0)
            {
                // if skipping this frame, bail
                if (machine().video().skip_this_frame())
                {
                    LOG_PARTIAL_UPDATES("skipped due to frameskipping\n");
                    return false;
                }

                // skip if this screen is not visible anywhere
                if (!machine().render().is_live(this))
                {
                    LOG_PARTIAL_UPDATES("skipped because screen not live\n");
                    return false;
                }
            }

            // skip if we already rendered this line
            if (scanline < m_last_partial_scan)
            {
                LOG_PARTIAL_UPDATES("skipped because line was already rendered\n");
                return false;
            }

            // set the range of scanlines to render
            rectangle clip = m_visarea;
            clip.sety(std.max(clip.top(), m_last_partial_scan), std.min(clip.bottom(), scanline));

            // skip if entirely outside of visible area
            if (clip.top() > clip.bottom())
            {
                LOG_PARTIAL_UPDATES("skipped because outside of visible area\n");
                return false;
            }

            // otherwise, render

            if (machine().video().frame_update_count() % 400 == 0)
            {
                LOG_PARTIAL_UPDATES("updating {0}-{1}\n", clip.top(), clip.bottom());
            }


            g_profiler.start(profile_type.PROFILER_VIDEO);


            u32 flags = 0;
            if ((m_video_attributes & VIDEO_VARIABLE_WIDTH) != 0)
            {
                rectangle scan_clip = clip;
                for (int y = clip.top(); y <= clip.bottom(); y++)
                {
                    scan_clip.sety(y, y);
                    pre_update_scanline(y);

                    screen_bitmap curbitmap = m_bitmap[m_curbitmap];
                    switch (curbitmap.format())
                    {
                        default:
                        case bitmap_format.BITMAP_FORMAT_IND16:   flags |= m_screen_update_ind16(this, (bitmap_ind16)m_scan_bitmaps[m_curbitmap][y], scan_clip);   break;
                        case bitmap_format.BITMAP_FORMAT_RGB32:   flags |= m_screen_update_rgb32(this, (bitmap_rgb32)m_scan_bitmaps[m_curbitmap][y], scan_clip);   break;
                    }

                    m_partial_updates_this_frame++;
                }
            }
            else
            {
                if (m_type != SCREEN_TYPE_SVG)
                {
                    screen_bitmap curbitmap = m_bitmap[m_curbitmap];
                    switch (curbitmap.format())
                    {
                        default:
                        case bitmap_format.BITMAP_FORMAT_IND16:   flags = m_screen_update_ind16(this, curbitmap.as_ind16(), clip);   break;
                        case bitmap_format.BITMAP_FORMAT_RGB32:   flags = m_screen_update_rgb32(this, curbitmap.as_rgb32(), clip);   break;
                    }
                }
                else
                {
                    throw new emu_unimplemented();
#if false
                    flags = m_svg.render(this, m_bitmap[m_curbitmap].as_rgb32(), clip);
#endif
                }
                m_partial_updates_this_frame++;
            }


            m_partial_updates_this_frame++;


            g_profiler.stop();


            // if we modified the bitmap, we have to commit
            m_changed = ((m_changed ? 1U : 0) | ~flags & UPDATE_HAS_NOT_CHANGED) != 0;

            // remember where we left off
            m_last_partial_scan = scanline + 1;
            m_partial_scan_hpos = -1;

            return true;
        }


        //-------------------------------------------------
        //  update_now - perform an update from the last
        //  beam position up to the current beam position
        //-------------------------------------------------
        public void update_now()
        {
            // these two checks only apply if we're allowed to skip frames
            if ((m_video_attributes & VIDEO_ALWAYS_UPDATE) == 0)
            {
                // if skipping this frame, bail
                if (machine().video().skip_this_frame())
                {
                    LOG_PARTIAL_UPDATES("skipped due to frameskipping\n");
                    return;
                }

                // skip if this screen is not visible anywhere
                if (!machine().render().is_live(this))
                {
                    LOG_PARTIAL_UPDATES("skipped because screen not live\n");
                    return;
                }
            }

            int current_vpos = vpos();
            int current_hpos = hpos();
            rectangle clip = m_visarea;

            // skip if we already rendered this line
            if (current_vpos < m_last_partial_scan)
            {
                LOG_PARTIAL_UPDATES("skipped because line was already rendered\n");
                return;
            }

            // if beam position is the same, there's nothing to update
            if (current_vpos == m_last_partial_scan && current_hpos == m_partial_scan_hpos)
            {
                LOG_PARTIAL_UPDATES("skipped because beam position is unchanged\n");
                return;
            }

            LOG_PARTIAL_UPDATES("update_now(): Y={0}, X={1}, last partial {2}, partial hpos {3}  (vis {4} {5})\n", current_vpos, current_hpos, m_last_partial_scan, m_partial_scan_hpos, m_visarea.right(), m_visarea.bottom());

            // start off by doing a partial update up to the line before us, in case that was necessary
            if (current_vpos > m_last_partial_scan)
            {
                // if the line before us was incomplete, we must do it in two pieces
                if (m_partial_scan_hpos >= 0)
                {
                    // now finish the previous partial scanline
                    clip.set(std.max(clip.left(), m_partial_scan_hpos + 1),
                             clip.right(),
                             std.max(clip.top(), m_last_partial_scan),
                             std.min(clip.bottom(), m_last_partial_scan));

                    // if there's something to draw, do it
                    if (!clip.empty())
                    {
                        g_profiler.start(profile_type.PROFILER_VIDEO);

                        u32 flags = 0;
                        screen_bitmap curbitmap = m_bitmap[m_curbitmap];
                        if ((m_video_attributes & VIDEO_VARIABLE_WIDTH) != 0)
                        {
                            pre_update_scanline(m_last_partial_scan);
                            switch (curbitmap.format())
                            {
                                default:
                                case bitmap_format.BITMAP_FORMAT_IND16:   flags = m_screen_update_ind16(this, (bitmap_ind16)m_scan_bitmaps[m_curbitmap][m_last_partial_scan], clip);   break;
                                case bitmap_format.BITMAP_FORMAT_RGB32:   flags = m_screen_update_rgb32(this, (bitmap_rgb32)m_scan_bitmaps[m_curbitmap][m_last_partial_scan], clip);   break;
                            }
                        }
                        else
                        {
                            switch (curbitmap.format())
                            {
                                default:
                                case bitmap_format.BITMAP_FORMAT_IND16:   flags = m_screen_update_ind16(this, curbitmap.as_ind16(), clip);   break;
                                case bitmap_format.BITMAP_FORMAT_RGB32:   flags = m_screen_update_rgb32(this, curbitmap.as_rgb32(), clip);   break;
                            }
                        }

                        g_profiler.stop();
                        m_partial_updates_this_frame++;

                        // if we modified the bitmap, we have to commit
                        m_changed = ((m_changed ? 1U : 0) | ~flags & UPDATE_HAS_NOT_CHANGED) != 0;  //m_changed |= ~flags & UPDATE_HAS_NOT_CHANGED;
                    }

                    m_partial_scan_hpos = -1;
                    m_last_partial_scan++;
                }

                if (current_vpos > m_last_partial_scan)
                {
                    update_partial(current_vpos - 1);
                }
            }

            // now draw this partial scanline
            clip = m_visarea;

            clip.set(std.max(clip.left(), m_partial_scan_hpos + 1),
                     std.min(clip.right(), current_hpos),
                     std.max(clip.top(), current_vpos),
                     std.min(clip.bottom(), current_vpos));

            // and if there's something to draw, do it
            if (!clip.empty())
            {
                g_profiler.start(profile_type.PROFILER_VIDEO);

                LOG_PARTIAL_UPDATES("doing scanline partial draw: Y {0} X {1}-{2}\n", clip.bottom(), clip.left(), clip.right());

                u32 flags = 0;
                screen_bitmap curbitmap = m_bitmap[m_curbitmap];
                if ((m_video_attributes & VIDEO_VARIABLE_WIDTH) != 0)
                {
                    pre_update_scanline(current_vpos);
                    switch (curbitmap.format())
                    {
                        default:
                        case bitmap_format.BITMAP_FORMAT_IND16:   flags = m_screen_update_ind16(this, (bitmap_ind16)m_scan_bitmaps[m_curbitmap][current_vpos], clip);   break;
                        case bitmap_format.BITMAP_FORMAT_RGB32:   flags = m_screen_update_rgb32(this, (bitmap_rgb32)m_scan_bitmaps[m_curbitmap][current_vpos], clip);   break;
                    }
                }
                else
                {
                    switch (curbitmap.format())
                    {
                        default:
                        case bitmap_format.BITMAP_FORMAT_IND16:   flags = m_screen_update_ind16(this, curbitmap.as_ind16(), clip);   break;
                        case bitmap_format.BITMAP_FORMAT_RGB32:   flags = m_screen_update_rgb32(this, curbitmap.as_rgb32(), clip);   break;
                    }
                }

                m_partial_updates_this_frame++;
                g_profiler.stop();

                // if we modified the bitmap, we have to commit
                m_changed = ((m_changed ? 1U : 0) | ~flags & UPDATE_HAS_NOT_CHANGED) != 0;  //m_changed |= ~flags & UPDATE_HAS_NOT_CHANGED;
            }

            // remember where we left off
            m_partial_scan_hpos = current_hpos;
            m_last_partial_scan = current_vpos;
        }


        //-------------------------------------------------
        //  reset_partial_updates - reset the partial
        //  updating state
        //-------------------------------------------------
        public void reset_partial_updates()
        {
            m_last_partial_scan = 0;
            m_partial_scan_hpos = -1;
            m_partial_updates_this_frame = 0;
            m_scanline0_timer.adjust(time_until_pos(0));
        }


        // additional helpers

        //-------------------------------------------------
        //  register_vblank_callback - registers a VBLANK
        //  callback
        //-------------------------------------------------
        public void register_vblank_callback(vblank_state_delegate vblank_callback)
        {
            // validate arguments
            //assert(!vblank_callback.isnull());

            // check if we already have this callback registered
            foreach (var item in m_callback_list)
            {
                if (item.m_callback == vblank_callback)
                    break;
            }

            // if not found, register
            m_callback_list.push_back(new callback_item(vblank_callback));
        }

        //-------------------------------------------------
        //  register_screen_bitmap - registers a bitmap
        //  that should track the screen size
        //-------------------------------------------------
        public void register_screen_bitmap(bitmap_t bitmap)
        {
            // append to the list
            m_auto_bitmap_list.push_back(new auto_bitmap_item(bitmap));

            // if allocating now, just do it
            bitmap.allocate(width(), height());
            if (m_palette != null && m_palette.op != null && m_palette.op0 != null)
                bitmap.set_palette(m_palette.op0.palette());
        }


        // internal to the video system

        //-------------------------------------------------
        //  update_quads - set up the quads for this
        //  screen
        //-------------------------------------------------
        public bool update_quads()
        {
            // only update if live
            if (machine().render().is_live(this))
            {
                // only update if empty and not a vector game; otherwise assume the driver did it directly
                if (m_type != screen_type_enum.SCREEN_TYPE_VECTOR && (m_video_attributes & VIDEO_SELF_RENDER) == 0)
                {
                    // if we're not skipping the frame and if the screen actually changed, then update the texture
                    if (!machine().video().skip_this_frame() && m_changed)
                    {
                        if ((m_video_attributes & VIDEO_VARIABLE_WIDTH) != 0)
                        {
                            create_composited_bitmap();
                        }

                        m_texture[m_curbitmap].set_bitmap(m_bitmap[m_curbitmap].live(), m_visarea, m_bitmap[m_curbitmap].texformat());
                        m_curtexture = m_curbitmap;
                        m_curbitmap = (u8)(1 - m_curbitmap);
                    }

                    // brightness adjusted render color
                    rgb_t color = m_color - new rgb_t(0, (u8)(0xff - m_brightness), (u8)(0xff - m_brightness), (u8)(0xff - m_brightness));

                    // create an empty container with a single quad
                    m_container.empty();
                    m_container.add_quad(0.0f, 0.0f, 1.0f, 1.0f, color, m_texture[m_curtexture], PRIMFLAG_BLENDMODE(BLENDMODE_NONE) | PRIMFLAG_SCREENTEX(1));
                }
            }

            // reset the screen changed flags
            bool result = m_changed;
            m_changed = false;
            return result;
        }

        //-------------------------------------------------
        //  update_burnin - update the burnin bitmap
        //-------------------------------------------------
        public void update_burnin()
        {
            //throw new emu_unimplemented();
#if false
#endif
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_validity_check - verify device
        //  configuration
        //-------------------------------------------------
        protected override void device_validity_check(validity_checker valid)
        {
            // sanity check dimensions
            if (m_width <= 0 || m_height <= 0)
                osd_printf_error("Invalid display dimensions\n");

            // sanity check display area
            if (m_type != screen_type_enum.SCREEN_TYPE_VECTOR)
            {
                if (m_visarea.empty() || m_visarea.right() >= m_width || m_visarea.bottom() >= m_height)
                    osd_printf_error("Invalid display area\n");

                // sanity check screen formats
                if (m_screen_update_ind16 == null && m_screen_update_rgb32 == null)
                    osd_printf_error("Missing SCREEN_UPDATE function\n");
            }
            else
            {
                if ((m_video_attributes & VIDEO_VARIABLE_WIDTH) != 0)
                    osd_printf_error("Non-raster display cannot have a variable width\n");
            }

            // check for invalid frame rate
            if (m_refresh == 0 || m_refresh > ATTOSECONDS_PER_SECOND)
                osd_printf_error("Invalid (under 1Hz) refresh rate\n");

            texture_format texformat = m_screen_update_ind16 != null ? texture_format.TEXFORMAT_PALETTE16 : texture_format.TEXFORMAT_RGB32;
            if (m_palette.finder_tag() != finder_base.DUMMY_TAG)
            {
                if (m_palette == null)
                    osd_printf_error("Screen references non-existent palette tag {0}\n", m_palette.finder_tag());

                if (texformat == texture_format.TEXFORMAT_RGB32)
                    osd_printf_warning("Screen does not need palette defined\n");
            }
            else if (texformat == texture_format.TEXFORMAT_PALETTE16)
            {
                osd_printf_error("Screen does not have palette defined\n");
            }
        }


        //-------------------------------------------------
        //  device_config_complete - finalise static
        //  configuration
        //-------------------------------------------------
        protected override void device_config_complete()
        {
            // combine orientation with machine orientation
            m_orientation = orientation_add(m_orientation, (int)(mconfig().gamedrv().flags & machine_flags.type.MASK_ORIENTATION));
        }


        //-------------------------------------------------
        //  device_resolve_objects - resolve objects that
        //  may be needed for other devices to set
        //  initial conditions at start time
        //-------------------------------------------------
        protected override void device_resolve_objects()
        {
            // bind our handlers
            //throw new emu_unimplemented();
#if false
            m_screen_update_ind16.resolve();
            m_screen_update_rgb32.resolve();
#endif
            m_screen_vblank.resolve_safe();
            m_scanline_cb.resolve();

            // assign our format to the palette before it starts
            if (m_palette != null && m_palette.op != null && m_palette.op0 != null)
                m_palette.op0.m_format = format();  //m_palette->m_format = format();
        }

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            // if we have a palette and it's not started, wait for it
            if (m_palette != null && m_palette.op != null && m_palette.op0 != null && !m_palette.op0.device().started())
                throw new device_missing_dependencies();

            if (m_type == screen_type_enum.SCREEN_TYPE_SVG)
            {
                throw new emu_unimplemented();
            }

            // configure bitmap formats and allocate screen bitmaps
            // svg is RGB32 too, and doesn't have any update method
            bool screen16 = m_screen_update_ind16 != null;
            texture_format texformat = screen16 ? texture_format.TEXFORMAT_PALETTE16 : texture_format.TEXFORMAT_RGB32;

            foreach (var elem in m_bitmap)
            {
                elem.set_format(format(), texformat);
                register_screen_bitmap(elem.live());
            }
            register_screen_bitmap(m_priority);

            // allocate raw textures
            m_texture[0] = machine().render().texture_alloc();
            m_texture[0].set_id((u64)m_unique_id << 57);
            m_texture[1] = machine().render().texture_alloc();
            m_texture[1].set_id(((u64)m_unique_id << 57) | 1);

            // configure the default cliparea
            render_container.user_settings settings = m_container.get_user_settings();
            settings.m_xoffset = m_xoffset;
            settings.m_yoffset = m_yoffset;
            settings.m_xscale = m_xscale;
            settings.m_yscale = m_yscale;
            m_container.set_user_settings(settings);

            // allocate the VBLANK timers
            m_vblank_begin_timer = timer_alloc(TID_VBLANK_START);
            m_vblank_end_timer = timer_alloc(TID_VBLANK_END);

            // allocate a timer to reset partial updates
            m_scanline0_timer = timer_alloc(TID_SCANLINE0);

            // allocate a timer to generate per-scanline updates
            if ((m_video_attributes & VIDEO_UPDATE_SCANLINE) != 0 || m_scanline_cb.bool_)
                m_scanline_timer = timer_alloc(TID_SCANLINE);

            // configure the screen with the default parameters
            configure(m_width, m_height, m_visarea, m_refresh);

            // reset VBLANK timing
            m_vblank_start_time = attotime.zero;
            m_vblank_end_time = new attotime(0, m_vblank_period);

            // start the timer to generate per-scanline updates
            if ((m_video_attributes & VIDEO_UPDATE_SCANLINE) != 0 || m_scanline_cb.bool_)
                m_scanline_timer.adjust(time_until_pos(0));

            // create burn-in bitmap
            if (machine().options().burnin())
            {
                throw new emu_unimplemented();
#if false
                int width;
                int height;
                if (sscanf(machine().options().snap_size(), "%dx%d", &width, &height) != 2 || width == 0 || height == 0)
                    width = height = 300;
                m_burnin.allocate(width, height);
                m_burnin.fill(0);
#endif
            }

            // load the effect overlay
            string overname = machine().options().effect();
            if (!string.IsNullOrEmpty(overname) && overname == "none")
                load_effect_overlay(overname);

            // register items for saving
            save_item(NAME(new { m_width }));
            save_item(NAME(new { m_height }));
            save_item(NAME(new { m_visarea.min_x }));
            save_item(NAME(new { m_visarea.min_y }));
            save_item(NAME(new { m_visarea.max_x }));
            save_item(NAME(new { m_visarea.max_y }));
            save_item(NAME(new { m_last_partial_scan }));
            save_item(NAME(new { m_frame_period }));
            save_item(NAME(new { m_brightness }));
            save_item(NAME(new { m_scantime }));
            save_item(NAME(new { m_pixeltime }));
            save_item(NAME(new { m_vblank_period }));
            save_item(NAME(new { m_vblank_start_time }));
            save_item(NAME(new { m_vblank_end_time }));
            save_item(NAME(new { m_frame_number }));
            if (m_oldstyle_vblank_supplied)
                logerror("{0}: Deprecated legacy Old Style screen configured (MCFG_SCREEN_VBLANK_TIME), please use MCFG_SCREEN_RAW_PARAMS instead.\n", tag());

            m_is_primary_screen = (this == new screen_device_enumerator(machine().root_device()).first());
        }


        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_reset()
        {
            // reset brightness to default
            m_brightness = 0xff;
        }


        //-------------------------------------------------
        //  device_stop - clean up before the machine goes
        //  away
        //-------------------------------------------------
        protected override void device_stop()
        {
            machine().render().texture_free(m_texture[0]);
            machine().render().texture_free(m_texture[1]);
            if (m_burnin.valid())
                finalize_burnin();
        }

        //-------------------------------------------------
        //  device_post_load - device-specific update
        //  after a save state is loaded
        //-------------------------------------------------
        protected override void device_post_load()
        {
            realloc_screen_bitmaps();
        }

        //-------------------------------------------------
        //  device_timer - called whenever a device timer
        //  fires
        //-------------------------------------------------
        protected override void device_timer(emu_timer timer, device_timer_id id, int param)
        {
            switch (id)
            {
                // signal VBLANK start
                case TID_VBLANK_START:
                    vblank_begin();
                    break;

                // signal VBLANK end
                case TID_VBLANK_END:
                    vblank_end();
                    break;

                // first scanline
                case TID_SCANLINE0:
                    reset_partial_updates();
                    if ((m_video_attributes & VIDEO_VARIABLE_WIDTH) != 0)
                    {
                        pre_update_scanline(0);
                    }
                    break;

                // subsequent scanlines when scanline updates are enabled
                case TID_SCANLINE:
                    if ((m_video_attributes & VIDEO_VARIABLE_WIDTH) != 0)
                    {
                        pre_update_scanline(param);
                    }

                    if ((m_video_attributes & VIDEO_UPDATE_SCANLINE) != 0)
                    {
                        // force a partial update to the current scanline
                        update_partial(param);
                    }

                    if (m_scanline_cb.bool_)
                        m_scanline_cb.op_u32((u32)param);

                    // compute the next visible scanline
                    param++;
                    if (param > m_visarea.bottom())
                        param = m_visarea.top();
                    m_scanline_timer.adjust(time_until_pos(param), param);
                    break;
            }
        }


        // internal helpers
        public void set_container(render_container container) { m_container = container; }

        //-------------------------------------------------
        //  realloc_screen_bitmaps - reallocate bitmaps
        //  and textures as necessary
        //-------------------------------------------------
        void realloc_screen_bitmaps()
        {
            // doesn't apply for vector games
            if (m_type == screen_type_enum.SCREEN_TYPE_VECTOR)
                return;

            // determine effective size to allocate
            bool per_scanline = (m_video_attributes & VIDEO_VARIABLE_WIDTH) != 0;
            s32 effwidth = std.max(per_scanline ? m_max_width : m_width, m_visarea.right() + 1);
            s32 effheight = std.max(m_height, m_visarea.bottom() + 1);

            // resize all registered screen bitmaps
            foreach (var item in m_auto_bitmap_list)
                item.m_bitmap.resize(effwidth, effheight);

            // re-set up textures
            if (m_palette != null && m_palette.op != null && m_palette.op0 != null)
            {
                m_bitmap[0].set_palette(m_palette.op0.palette());
                m_bitmap[1].set_palette(m_palette.op0.palette());
            }
            m_texture[0].set_bitmap(m_bitmap[0].live(), m_visarea, m_bitmap[0].texformat());
            m_texture[1].set_bitmap(m_bitmap[1].live(), m_visarea, m_bitmap[1].texformat());

            allocate_scan_bitmaps();
        }

        //-------------------------------------------------
        //  vblank_begin - call any external callbacks to
        //  signal the VBLANK period has begun
        //-------------------------------------------------
        void vblank_begin()
        {
            // reset the starting VBLANK time
            m_vblank_start_time = machine().time();
            m_vblank_end_time = m_vblank_start_time + new attotime(0, m_vblank_period);

            // if this is the primary screen and we need to update now
            if (m_is_primary_screen && (m_video_attributes & VIDEO_UPDATE_AFTER_VBLANK) == 0)
                machine().video().frame_update();

            // call the screen specific callbacks
            foreach (var item in m_callback_list)
                item.m_callback(this, true);

            m_screen_vblank.op_s32(1);

            // reset the VBLANK start timer for the next frame
            m_vblank_begin_timer.adjust(time_until_vblank_start());

            // if no VBLANK period, call the VBLANK end callback immediately, otherwise reset the timer
            if (m_vblank_period == 0)
                vblank_end();
            else
                m_vblank_end_timer.adjust(time_until_vblank_end());
        }

        //-------------------------------------------------
        //  vblank_end - call any external callbacks to
        //  signal the VBLANK period has ended
        //-------------------------------------------------
        void vblank_end()
        {
            // call the screen specific callbacks
            foreach (var item in m_callback_list)
                item.m_callback(this, false);

            m_screen_vblank.op_s32(0);

            // if this is the primary screen and we need to update now
            if (m_is_primary_screen && (m_video_attributes & VIDEO_UPDATE_AFTER_VBLANK) != 0)
                machine().video().frame_update();

            // increment the frame number counter
            m_frame_number++;
        }

        //-------------------------------------------------
        //  finalize_burnin - finalize the burnin bitmap
        //-------------------------------------------------
        void finalize_burnin()
        {
            if (!m_burnin.valid())
                return;

            throw new emu_unimplemented();
        }

        //-------------------------------------------------
        //  finalize_burnin - finalize the burnin bitmap
        //-------------------------------------------------
        void load_effect_overlay(string filename)
        {
            //throw new emu_unimplemented();
#if false
#endif
        }


        void update_scan_bitmap_size(int y)
        {
            // don't update this line if it exceeds the allocated size, which can happen on initial configuration
            if (y >= (int)m_scan_widths.size())
                return;

            // determine effective size to allocate
            s32 effwidth = std.max(m_max_width, m_visarea.right() + 1);

            if (m_scan_widths[y] == effwidth)
                return;

            m_scan_bitmaps[m_curbitmap][y].resize(effwidth, 1);
            m_scan_widths[y] = effwidth;
        }


        void pre_update_scanline(int y)
        {
            update_scan_bitmap_size(y);
        }


        void create_composited_bitmap()
        {
            screen_bitmap curbitmap = m_bitmap[m_curtexture];
            if (!curbitmap.valid())
                return;

            s32 dstwidth = std.max(m_max_width, m_visarea.right() + 1);
            int dstheight = curbitmap.height();

            switch (curbitmap.format())
            {
                default:
                case bitmap_format.BITMAP_FORMAT_IND16:
                {
                    for (int y = 0; y < dstheight; y++)
                    {
                        bitmap_ind16 srcbitmap = (bitmap_ind16)m_scan_bitmaps[m_curbitmap][y];
                        PointerU16 dst = curbitmap.as_ind16().pix(y);  //u16 *dst = &curbitmap.as_ind16().pix(y);
                        PointerU16 src = srcbitmap.pix(0);  //const u16 *src = &srcbitmap.pix(0);
                        int dx = (m_scan_widths[y] << 15) / dstwidth;
                        for (int x = 0; x < m_scan_widths[y]; x += dx)
                        {
                            dst[0] = src[x >> 15];  dst++;  //*dst++ = src[x >> 15];
                        }
                    }
                    break;
                }

                case bitmap_format.BITMAP_FORMAT_RGB32:
                {
                    for (int y = 0; y < dstheight; y++)
                    {
                        bitmap_rgb32 srcbitmap = (bitmap_rgb32)m_scan_bitmaps[m_curbitmap][y];
                        PointerU32 dst = curbitmap.as_rgb32().pix(y);  //u32 *dst = &curbitmap.as_rgb32().pix(y);
                        PointerU32 src = srcbitmap.pix(0);  //const u32 *src = &srcbitmap.pix(0);
                        int dx = (m_scan_widths[y] << 15) / dstwidth;
                        for (int x = 0; x < dstwidth << 15; x += dx)
                        {
                            dst[0] = src[x >> 15];  dst++;  //*dst++ = src[x >> 15];
                        }
                    }
                    break;
                }
            }
        }


        //void destroy_scan_bitmaps();


        //-------------------------------------------------
        //  allocate_scan_bitmaps - allocate per-scanline
        //  bitmaps if applicable
        //-------------------------------------------------
        void allocate_scan_bitmaps()
        {
            if ((m_video_attributes & VIDEO_VARIABLE_WIDTH) != 0)
            {
                bool screen16 = m_screen_update_ind16 != null;
                s32 effwidth = std.max(m_max_width, m_visarea.right() + 1);
                s32 old_height = (s32)m_scan_widths.size();
                s32 effheight = std.max(m_height, m_visarea.bottom() + 1);
                if (old_height < effheight)
                {
                    for (int i = old_height; i < effheight; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            if (screen16)
                                m_scan_bitmaps[j].push_back(new bitmap_ind16(effwidth, 1));
                            else
                                m_scan_bitmaps[j].push_back(new bitmap_rgb32(effwidth, 1));
                        }
                        m_scan_widths.push_back(effwidth);
                    }
                }
                else
                {
                    for (int i = old_height - 1; i >= effheight; i--)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            if (screen16)
                                m_scan_bitmaps[j][i] = null;  //delete (bitmap_ind16 *)m_scan_bitmaps[j][i];
                            else
                                m_scan_bitmaps[j][i] = null;  //delete (bitmap_rgb32 *)m_scan_bitmaps[j][i];
                            m_scan_bitmaps[j].erase(i);  //m_scan_bitmaps[j].erase(m_scan_bitmaps[j].begin() + i);
                        }
                        m_scan_widths.erase(i);  //m_scan_widths.erase(m_scan_widths.begin() + i);
                    }
                }
            }
        }
    }


    // iterator helper
    //typedef device_type_enumerator<screen_device> screen_device_enumerator;


    static partial class screen_global
    {
        public static screen_device SCREEN(machine_config mconfig, string tag, screen_type_enum type)
        {
            var device = emu.detail.device_type_impl.op<screen_device>(mconfig, tag, screen_device.SCREEN, 0);
            device.screen_device_after_ctor(type);
            return device;
        }
        public static screen_device SCREEN<bool_Required>(machine_config mconfig, device_finder<screen_device, bool_Required> finder, screen_type_enum type)
            where bool_Required : bool_const, new()
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, screen_device.SCREEN, 0);
            device.screen_device_after_ctor(type);
            return device;
        }
    }
}
