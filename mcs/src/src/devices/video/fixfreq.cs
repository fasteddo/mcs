// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using fixedfreq_monitor_state_time_type = System.Double;  //using time_type = double;
using int32_t = System.Int32;
using ioport_value = System.UInt32;  //typedef u32 ioport_value;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;

using static mame.attotime_global;
using static mame.device_global;
using static mame.emucore_global;
using static mame.fixfreq_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.render_global;
using static mame.rendertypes_global;
using static mame.screen_global;


namespace mame
{
    class fixedfreq_monitor_desc
    {
        public uint32_t m_monitor_clock;
        public int m_fieldcount;
        public double m_sync_threshold;
        public double m_gain;
        public int m_hscale;
        public double m_vsync_threshold;

        int m_hvisible;
        int m_hfrontporch;
        int m_hsync;
        int m_hbackporch;
        int m_vvisible;
        int m_vfrontporch;
        int m_vsync;
        int m_vbackporch;


        public fixedfreq_monitor_desc()
        {
            // default to NTSC "704x480@30i"
            m_monitor_clock = 13500000;
            m_fieldcount = 2;
            m_sync_threshold = 0.3;
            m_gain = 1.0 / 3.7;
            m_hscale = 1;
            m_vsync_threshold = 0.600;
            // trigger at 91% of vsync length 1-exp(-0.6)
            m_hvisible = 704;
            m_hfrontporch = 728;
            m_hsync = 791;
            m_hbackporch = 858;
            m_vvisible = 480;
            m_vfrontporch = 486;
            m_vsync = 492;
            m_vbackporch = 525;
        }


        //uint32_t monitor_clock() const noexcept { return m_monitor_clock; }
        public double clock_period() { return 1.0 / (double)m_monitor_clock; }

        public int minh() { return (m_hbackporch - m_hsync) * m_hscale; }
        public int maxh() { return (m_hbackporch - m_hsync + m_hvisible) * m_hscale - 1; }
        public int minv() { return m_vbackporch - m_vsync; }
        public int maxv() { return m_vbackporch - m_vsync + m_vvisible - 1; }

        public int htotal_scaled() { return m_hbackporch * m_hscale; }

        public int vbackporch_width() { return m_vbackporch - m_vsync; }
        public int vsync_width() { return m_vsync - m_vfrontporch; }
        public int vfrontporch_width() { return m_vfrontporch - m_vvisible; }
        public int vvisible_width() { return m_vvisible; }
        public int vtotal() { return m_vbackporch; }

        public int hbackporch_width() { return m_hbackporch - m_hsync; }
        public int hsync_width() { return m_hsync - m_hfrontporch; }
        public int hfrontporch_width() { return m_hfrontporch - m_hvisible; }
        public int hvisible_width() { return m_hvisible; }
        public int htotal() { return m_hbackporch; }

        public void set_h_rel(int vw, int fpw, int sw, int bpw)
        {
            m_hvisible = vw;
            m_hfrontporch = m_hvisible + fpw;
            m_hsync = m_hfrontporch + sw;
            m_hbackporch = m_hsync + bpw;
        }

        public void set_v_rel(int vw, int fpw, int sw, int bpw)
        {
            m_vvisible = vw;
            m_vfrontporch = m_vvisible + fpw;
            m_vsync = m_vfrontporch + sw;
            m_vbackporch = m_vsync + bpw;
        }

        //double vsync_filter_timeconst() const noexcept
        //{
        //    return double(m_monitor_clock)
        //        / (double(m_hbackporch) * vsync_width());
        //}

        //double hsync_filter_timeconst() const noexcept
        //{
        //    return double(m_monitor_clock) / double(hsync_width());
        //}
    }


    interface fixedfreq_monitor_intf
    {
        //virtual ~fixedfreq_monitor_intf() = default;
        void vsync_end_cb(double refresh_time, uint32_t field);
    }


    struct fixedfreq_monitor_line
    {
        public float y;
        public float x;
        public float xr;
        public uint32_t col;
    }


    class fixedfreq_monitor_state
    {
        //using time_type = double;


        fixedfreq_monitor_desc m_desc;
        fixedfreq_monitor_intf m_intf;

        public double m_last_sync_val;
        uint32_t m_col;
        public float m_last_x;
        public int m_last_y;
        public fixedfreq_monitor_state_time_type m_last_sync_time;
        public fixedfreq_monitor_state_time_type m_line_time;
        public fixedfreq_monitor_state_time_type m_last_hsync_time;
        public fixedfreq_monitor_state_time_type m_last_vsync_time;

        public fixedfreq_monitor_state_time_type m_last_line_duration;
        public fixedfreq_monitor_state_time_type m_last_field_time;

        /* sync separator */
        public double m_vsync_filter;

        public int m_sig_vsync;
        public int m_sig_composite;
        public int m_sig_field;
        public std.vector<fixedfreq_monitor_line> m_fragments = new std.vector<fixedfreq_monitor_line>();


        public fixedfreq_monitor_state(fixedfreq_monitor_desc desc, fixedfreq_monitor_intf intf)
        {
            m_desc = desc;
            m_intf = intf;
            m_last_sync_val = 0;
            m_col = 0;
            m_last_x = 0;
            m_last_y = 0;
            m_last_sync_time = (fixedfreq_monitor_state_time_type)0;
            m_line_time = (fixedfreq_monitor_state_time_type)0;
            m_last_hsync_time = (fixedfreq_monitor_state_time_type)0;
            m_last_vsync_time = (fixedfreq_monitor_state_time_type)0;
            m_last_line_duration = (fixedfreq_monitor_state_time_type)0;
            m_last_field_time = (fixedfreq_monitor_state_time_type)0;
            m_vsync_filter = 0;
            m_sig_vsync = 0;
            m_sig_composite = 0;
            m_sig_field = 0;
        }


        /***
         * \brief To be called after monitor parameters are set
         */
        public void start()
        {
            // FIXME: once moved to netlist this may no longer be necessary.
            //        Only copies constructor init

            m_last_sync_val = 0.0;
            m_col = new rgb_t(0,0,0);
            m_last_x = 0;
            m_last_y = 0;
            m_last_sync_time = (fixedfreq_monitor_state_time_type)0;
            m_line_time = (fixedfreq_monitor_state_time_type)0;
            m_last_hsync_time = (fixedfreq_monitor_state_time_type)0;
            m_last_vsync_time = (fixedfreq_monitor_state_time_type)0;

            /* sync separator */
            m_vsync_filter = 0.0;

            m_sig_vsync = 0;
            m_sig_composite = 0;
            m_sig_field = 0;

            // htotal = m_desc.m_hbackporch;
            // vtotal = m_desc.m_vbackporch;

            /* sync separator */

            // m_vsync_threshold = (exp(- 3.0/(3.0+3.0))) - exp(-1.0);
            // printf("trigger %f with len %f\n", m_vsync_threshold, 1e6 /
            // m_vsync_filter_timeconst);
            //  Minimum frame period to be passed to video system ?

            m_fragments.clear();

            // m_intf.vsync_end_cb(m_desc.clock_period() * m_desc.vtotal() *
            // m_desc.htotal(), 0);
        }


        public void reset()
        {
            m_last_sync_val = 0;
            m_col = 0;
            m_last_x = 0;
            m_last_y = 0;
            m_vsync_filter = 0;
            m_sig_vsync = 0;
            m_sig_composite = 0;
            m_sig_field = 0;
            m_fragments.clear();
        }


        //void update_sync_channel(const time_type &time, double newval);
        //void update_bm(const time_type &time);
        //void update_composite_monochrome(const time_type &time, double newval);
        //void update_red(const time_type &time, double data);
        //void update_green(const time_type &time, double data);
        //void update_blue(const time_type &time, double data);
        //void update_sync(const time_type &time, double data);
    }


    // ======================> fixedfreq_device
    public class fixedfreq_device : device_t,
                                    //device_video_interface,
                                    fixedfreq_monitor_intf
    {
        //DEFINE_DEVICE_TYPE(FIXFREQ, fixedfreq_device, "fixfreq", "Fixed-Frequency Monochrome Monitor")
        public static readonly emu.detail.device_type_impl FIXFREQ = DEFINE_DEVICE_TYPE("fixfreq", "Fixed-Frequency Monochrome Monitor", (type, mconfig, tag, owner, clock) => { return new fixedfreq_device(mconfig, tag, owner, clock); });


        //using time_type = fixedfreq_monitor_state::time_type;


        enum fixedfreq_tag_id_e
        {
            HVISIBLE,
            HFRONTPORCH,
            HSYNC,
            HBACKPORCH,
            VVISIBLE,
            VFRONTPORCH,
            VSYNC,
            VBACKPORCH,
            SYNCTHRESHOLD,
            VSYNCTHRESHOLD,
            GAIN,
            SCANLINE_HEIGHT
        }


        const int VERBOSE = 0;
        //#define LOG_OUTPUT_STREAM std::cerr
        //#include "logmacro.h"
        void LOG(string format, params object [] args) { logmacro_global.LOG(VERBOSE, this, format, args); }


        required_ioport m_enable;
        required_ioport m_vector;
        float m_scanline_height;
        double m_last_rt;

        /* adjustable by drivers */
        fixedfreq_monitor_desc m_monitor;
        fixedfreq_monitor_state m_state;


        public device_video_interface m_divideo;


        // construction/destruction
        fixedfreq_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : this(mconfig, FIXFREQ, tag, owner, clock)
        {
        }

        fixedfreq_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_divideo = new device_video_interface(mconfig, this, false);  //device_video_interface(mconfig, *this, false),

            m_enable = new required_ioport(this, "ENABLE");
            m_vector = new required_ioport(this, "VECTOR");
            m_scanline_height = 1.0f;
            m_last_rt = 0;
            m_monitor = new fixedfreq_monitor_desc();
            m_state = new fixedfreq_monitor_state(m_monitor, this);
        }


        // inline configuration helpers
        public fixedfreq_device set_monitor_clock(uint32_t clock)
        {
            m_monitor.m_monitor_clock = clock;
            return this;
        }

        public fixedfreq_device set_fieldcount(int count)
        {
            m_monitor.m_fieldcount = count;
            return this;
        }

        public fixedfreq_device set_threshold(double threshold)
        {
            m_monitor.m_sync_threshold = threshold;
            return this;
        }

        //fixedfreq_device &set_vsync_threshold(double threshold) { m_monitor.m_vsync_threshold = threshold; return *this; }

        public fixedfreq_device set_gain(double gain)
        {
            m_monitor.m_gain = gain;
            return this;
        }

        public fixedfreq_device set_horz_params(int visible, int frontporch, int sync, int backporch)
        {
            m_monitor.set_h_rel(visible, frontporch - visible, sync - frontporch, backporch - sync);
            return this;
        }

        public fixedfreq_device set_vert_params(int visible, int frontporch, int sync, int backporch)
        {
            m_monitor.set_v_rel(visible, frontporch - visible, sync - frontporch, backporch - sync);
            return this;
        }

        public fixedfreq_device set_horz_scale(int hscale)
        {
            m_monitor.m_hscale = hscale;
            return this;
        }


        // pre-defined configurations
        //fixedfreq_device &set_mode_ntsc720() // ModeLine "720x480@30i" 13.5 720 736
        //                                     // 799 858 480 486 492 525 interlace
        //                                     // -hsync -vsync
        //{
        //    set_monitor_clock(13500000);
        //    set_horz_params(720, 736, 799, 858);
        //    set_vert_params(480, 486, 492, 525);
        //    set_fieldcount(2);
        //    set_threshold(0.3);
        //    return *this;
        //}
        //fixedfreq_device &set_mode_ntsc704() // ModeLine "704x480@30i" 13.5 704 728
        //                                     // 791 858 480 486 492 525
        //{
        //    set_monitor_clock(13500000);
        //    set_horz_params(704, 728, 791, 858);
        //    set_vert_params(480, 486, 492, 525);
        //    set_fieldcount(2);
        //    set_threshold(0.3);
        //    return *this;
        //}


        static uint32_t nom_col(uint32_t col)
        {
            float r = ((col >> 16) & 0xff);
            float g = ((col >>  8) & 0xff);
            float b = ((col >>  0) & 0xff);

            float m = std.max(r, std.max(g,b));
            if (m == 0.0f)
                return 0;

            return (((uint32_t) m ) << 24) | (((uint32_t) (r / m * 255.0f) ) << 16)
                | (((uint32_t) (g / m * 255.0f) ) << 8) | (((uint32_t) (b / m * 255.0f) ) <<  0);
        }


        static void draw_testpat(screen_device screen, bitmap_rgb32 bitmap, rectangle cliprect)
        {
            // Test pattern Grey scale
            const int stripes = 255;
            // auto va(screen.visible_area());
            var va = cliprect;

            for (int i = 0; i < stripes; i++)
            {
                int l = va.left() + (i * va.width() / stripes);
                int w = (va.left() +  (i + 1) * va.width() / stripes) - l;
                int v = (255 * i) / stripes;
                bitmap.plot_box(l, va.top() + 20, w, va.height() / 2 - 20, new rgb_t(0xff, (uint8_t)v, (uint8_t)v, (uint8_t)v));
            }

            {
                int l = va.left() + va.width() / 4;
                int w = va.width() / 4;
                int t = va.top() + va.height() / 2;
                int h = va.height() / 2;
                // 50% Test pattern
                for (int i = t; i < t + h; i += 2)
                {
                    bitmap.plot_box(l, i, w, i, new rgb_t(0xff, 0xff, 0xff, 0xff));
                    bitmap.plot_box(l, i + 1, w, i + 1, new rgb_t(0xff, 0, 0, 0));
                }

                l += va.width() / 4;
                bitmap.plot_box(l, t, w, h, new rgb_t(0xff, 0xc3, 0xc3, 0xc3)); // 195
            }
        }


        protected virtual uint32_t screen_update(screen_device screen, bitmap_rgb32 bitmap, rectangle cliprect)
        {
            // printf("%f\n", machine().time().as_double());
            // printf("%d %lu %f %f\n", m_state.m_sig_vsync, m_state.m_fragments.size(),
            // m_state.m_fragments[0].y,
            // m_state.m_fragments[m_state.m_fragments.size()-1].y);
            bool force_vector = screen.screen_type() == SCREEN_TYPE_VECTOR || (m_vector.op0.read() & 1) != 0;
            bool debug_timing = (m_enable.op0.read() & 2) == 2;
            bool test_pat = (m_enable.op0.read() & 4) == 4;
            rgb_t backcol = debug_timing ? new rgb_t(0xff, 0xff, 0x00, 0x00) : new rgb_t(0xff, 0x00, 0x00, 0x00);

            if (!force_vector)
            {
                screen.set_video_attributes(0);
                bitmap.fill(backcol);
                foreach (var f in m_state.m_fragments)
                {
                    if (f.y < bitmap.height())
                        bitmap.plot_box((int32_t)f.x, (int32_t)f.y, (int32_t)(f.xr - f.x), 1, f.col);
                }

                if (test_pat)
                    draw_testpat(screen, bitmap, cliprect);
            }
            else
            {
                screen.set_video_attributes(VIDEO_SELF_RENDER);

                uint32_t flags = PRIMFLAG_ANTIALIAS(1)
                    | PRIMFLAG_BLENDMODE(BLENDMODE_ADD)
                    | (screen.screen_type() == SCREEN_TYPE_VECTOR ? PRIMFLAG_VECTOR(1) : 0);
                rectangle visarea = screen.visible_area();
                float xscale = 1.0f / (float)visarea.width();
                float yscale = 1.0f / (float)visarea.height();
                float xoffs = (float)visarea.min_x;
                float yoffs = (float)visarea.min_y;
                screen.container().empty();
                screen.container().add_rect(0.0f, 0.0f, 1.0f, 1.0f, new rgb_t(0xff,0x00,0x00,0x00),
                    PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA)
                    | (screen.screen_type() == SCREEN_TYPE_VECTOR ? PRIMFLAG_VECTORBUF(1) : 0));

                float last_y = -1e6f;
                foreach (var f in m_state.m_fragments)
                {
                    float x0 = (f.x - xoffs) * xscale;
                    float y0 = (f.y - yoffs) * yscale;
                    float x1 = (f.xr - xoffs) * xscale;

                    rgb_t col = (debug_timing && f.y < last_y) ? backcol : new rgb_t(f.col);

                    // FIXME: Debug check for proper vsync timing
#if false
                        auto w = m_scanline_height * xscale * 0.5;
                        screen.container().add_line(
                            x0+w, y0, x1-w, y0, m_scanline_height*yscale,
                            nom_col(f.col),
//                          (0xff << 24) | (f.col & 0xffffff),
                            flags);
#elif true
                        float y1 = (f.y + m_scanline_height - yoffs) * yscale;
                        screen.container().add_rect(
                            x0, y0, x1, y1,
                            new rgb_t(nom_col(col)),
//                          (0xaf << 24) | (f.col & 0xffffff),
                            flags);
#else
                        const float y1((f.y + m_scanline_height - yoffs) * yscale);
                        // Crashes with bgfx
                        screen.container().add_quad(
                            x0, y0, x1, y1,
                            rgb_t(nom_col(f.col)),
//                          (0xaf << 24) | (f.col & 0xffffff),
                            m_texture,
                            flags);
#endif

                    last_y = f.y;
                }
            }

            m_state.m_fragments.clear();
            return 0;
        }


        //NETDEV_ANALOG_CALLBACK_MEMBER(update_composite_monochrome);
        public void update_composite_monochrome(double data, attotime time) { throw new emu_unimplemented(); }

        //NETDEV_ANALOG_CALLBACK_MEMBER(update_red);
        //NETDEV_ANALOG_CALLBACK_MEMBER(update_green);
        //NETDEV_ANALOG_CALLBACK_MEMBER(update_blue);
        //NETDEV_ANALOG_CALLBACK_MEMBER(update_sync);

        //INPUT_CHANGED_MEMBER(port_changed);
        public void port_changed(ioport_field field, u32 param, ioport_value oldval, ioport_value newval) { throw new emu_unimplemented(); }

        unsigned monitor_val(unsigned param)
        {
            switch (param)
            {
                case (unsigned)fixedfreq_tag_id_e.HVISIBLE: return (unsigned)m_monitor.hvisible_width();
                case (unsigned)fixedfreq_tag_id_e.HFRONTPORCH: return (unsigned)m_monitor.hfrontporch_width();
                case (unsigned)fixedfreq_tag_id_e.HSYNC: return (unsigned)m_monitor.hsync_width();
                case (unsigned)fixedfreq_tag_id_e.HBACKPORCH: return (unsigned)m_monitor.hbackporch_width();
                case (unsigned)fixedfreq_tag_id_e.VVISIBLE: return (unsigned)m_monitor.vvisible_width();
                case (unsigned)fixedfreq_tag_id_e.VFRONTPORCH: return (unsigned)m_monitor.vfrontporch_width();
                case (unsigned)fixedfreq_tag_id_e.VSYNC: return (unsigned)m_monitor.vsync_width();
                case (unsigned)fixedfreq_tag_id_e.VBACKPORCH: return (unsigned)m_monitor.vbackporch_width();
                case (unsigned)fixedfreq_tag_id_e.SYNCTHRESHOLD: return (unsigned)(m_monitor.m_sync_threshold * 1000.0);
                case (unsigned)fixedfreq_tag_id_e.VSYNCTHRESHOLD: return (unsigned)(m_monitor.m_vsync_threshold * 1000.0);
                case (unsigned)fixedfreq_tag_id_e.GAIN: return (unsigned)(m_monitor.m_gain * 100.0);
                case (unsigned)fixedfreq_tag_id_e.SCANLINE_HEIGHT: return (unsigned)(m_scanline_height * 100.0);
            }

            return 0;
        }


        // device-level overrides
        protected override void device_config_complete()
        {
            if (!m_divideo.has_screen())
                return;

            // Video signal processing will be moved into netlist to avoid
            // aborting cpu slices. When this is done, the monitor specifications
            // need to move to the netlist as well.
            //
            // At the time of device_config_complete the monitor specification will
            // not be known - the netlist is parsed during device_start.
            // In this case we have to use some temporary fixed values, e.g.
            // screen().set_raw(7158196, 454, 0, 454, 262, 0, 262);
            // This will be overwritten during the first vblank anyhow.
            //
            // However the width and height determine the width of the mame window.
            // It is therefore recommended to use `set_raw` in the mame driver
            // to specify the window size.
            if (m_divideo.screen().refresh_attoseconds() == 0)
            {
                m_divideo.screen().set_raw(m_monitor.m_monitor_clock, (u16)m_monitor.htotal(), 0,
                    (u16)m_monitor.htotal(), (u16)m_monitor.vtotal(), 0,
                    (u16)m_monitor.vtotal());
            }

            if (!m_divideo.screen().has_screen_update())
                m_divideo.screen().set_screen_update(screen_update);

            LOG("config complete\n");
        }


        protected override void device_start()
        {
            LOG("start\n");

            m_state.start();

            // FIXME: will be done by netlist going forward
            save_item(NAME(new { m_state.m_last_sync_val }));
            save_item(NAME(new { m_state.m_last_x }));
            save_item(NAME(new { m_state.m_last_y }));
            save_item(NAME(new { m_state.m_last_sync_time }));
            save_item(NAME(new { m_state.m_line_time }));
            save_item(NAME(new { m_state.m_last_hsync_time }));
            save_item(NAME(new { m_state.m_last_vsync_time }));
            save_item(NAME(new { m_state.m_last_line_duration }));
            save_item(NAME(new { m_state.m_last_field_time }));

            /* sync separator */
            save_item(NAME(new { m_state.m_vsync_filter }));
            save_item(NAME(new { m_state.m_sig_vsync }));
            save_item(NAME(new { m_state.m_sig_composite }));
            save_item(NAME(new { m_state.m_sig_field }));

            save_item(NAME(new { m_last_rt }));
        }


        protected override void device_reset()
        {
            m_state.reset();
            LOG("Reset\n");
            // ioport("YYY")->field(0xffff)->live().value = 20;
#if false
            //IOPORT_ID(HVISIBLE)->field(~0)->set_value(m_monitor.m_hvisible);
            //IOPORT_ID(HVISIBLE)->update_defvalue(false);
            IOPORT_ID(HVISIBLE)->live().defvalue = m_monitor.m_hvisible;
            IOPORT_ID(HFRONTPORCH)->write(m_monitor.m_hsync);
            IOPORT_ID(HSYNC)->write(m_monitor.m_hfrontporch);
            IOPORT_ID(HBACKPORCH)->write(m_monitor.m_hbackporch);
            IOPORT_ID(VVISIBLE)->write(m_monitor.m_vvisible);
            IOPORT_ID(VFRONTPORCH)->write(m_monitor.m_vfrontporch);
            IOPORT_ID(VSYNC)->write(m_monitor.m_vsync);
            IOPORT_ID(VBACKPORCH)->write(m_monitor.m_vbackporch);
            IOPORT_ID(SYNCTHRESHOLD)->write(m_monitor.m_sync_threshold * 1000.0);
            IOPORT_ID(GAIN)->write(m_monitor.m_gain * 1000.0);
#endif
        }


        protected override void device_post_load() { throw new emu_unimplemented(); }


        class construct_ioport_fixedfreq : construct_ioport_helper
        {
            fixedfreq_device m_owner;

            public construct_ioport_fixedfreq(fixedfreq_device owner)
            {
                m_owner = owner;
            }


            void PORT_ADJUSTERX(fixedfreq_tag_id_e _id, string _name, ioport_value _min, ioport_value _max)  //#define PORT_ADJUSTERX(_id, _name, _min, _max) \
            {
                PORT_START(_id.ToString());
                configurer.field_alloc(ioport_type.IPT_ADJUSTER, ((fixedfreq_device)m_owner).monitor_val((unsigned)_id), 0xffff, "Monitor - " + _name);
                PORT_MINMAX(_min, _max); PORT_CHANGED_MEMBER(DEVICE_SELF, m_owner.port_changed, (u32)_id);
                PORT_CONDITION("ENABLE", 0x01, ioport_condition.condition_t.EQUALS, 0x01);
            }


            //static INPUT_PORTS_START(fixedfreq_base_ports)
            void construct_ioport_fixedfreq_base_ports(device_t owner, ioport_list portlist, ref string errorbuf)
            {
                INPUT_PORTS_START(owner, portlist, ref errorbuf);

                PORT_START("ENABLE");
                PORT_CONFNAME( 0x01, 0x00, "Display Monitor sliders" );
                PORT_CONFSETTING(    0x00, DEF_STR( Off ) );
                PORT_CONFSETTING(    0x01, DEF_STR( On ) );
                PORT_CONFNAME( 0x02, 0x00, "Visual Timing Debug" );
                PORT_CONFSETTING(    0x00, DEF_STR( Off ) );
                PORT_CONFSETTING(    0x02, DEF_STR( On ) );
                PORT_CONFNAME( 0x04, 0x00, "Display gray test pattern" ); PORT_CONDITION("VECTOR", 0x01, ioport_condition.condition_t.EQUALS, 0x00);
                PORT_CONFSETTING(    0x00, DEF_STR( Off ) );
                PORT_CONFSETTING(    0x04, DEF_STR( On ) );

                PORT_CONFNAME( 0x08, 0x00, "Interlace mode" ); PORT_CONDITION("VECTOR", 0x01, ioport_condition.condition_t.EQUALS, 0x00);
                PORT_CONFSETTING(    0x00, "Interlaced" );
                PORT_CONFSETTING(    0x08, "Progressive" );

                PORT_ADJUSTERX(fixedfreq_tag_id_e.HVISIBLE, "H Visible", 10, 1000);
                PORT_ADJUSTERX(fixedfreq_tag_id_e.HFRONTPORCH, "H Front porch width", 1, 100);
                PORT_ADJUSTERX(fixedfreq_tag_id_e.HSYNC, "H Sync width", 1, 100);
                PORT_ADJUSTERX(fixedfreq_tag_id_e.HBACKPORCH, "H Back porch width", 1, 1000);
                PORT_ADJUSTERX(fixedfreq_tag_id_e.VVISIBLE, "V Visible", 1, 1000);
                PORT_ADJUSTERX(fixedfreq_tag_id_e.VFRONTPORCH, "V Front porch width", 0, 100);
                PORT_ADJUSTERX(fixedfreq_tag_id_e.VSYNC, "V Sync width", 1, 100);
                PORT_ADJUSTERX(fixedfreq_tag_id_e.VBACKPORCH, "V Back porch width", 1, 100);
                PORT_ADJUSTERX(fixedfreq_tag_id_e.SYNCTHRESHOLD, "Sync threshold mV", 10, 2000);
                PORT_ADJUSTERX(fixedfreq_tag_id_e.VSYNCTHRESHOLD, "V Sync threshold mV", 10, 1000);
                PORT_ADJUSTERX(fixedfreq_tag_id_e.GAIN, "Signal Gain", 10, 1000);

                INPUT_PORTS_END();
            }


            //static INPUT_PORTS_START(fixedfreq_raster_ports)
            public void construct_ioport_fixedfreq_raster_ports(device_t owner, ioport_list portlist, ref string errorbuf)
            {
                INPUT_PORTS_START(owner, portlist, ref errorbuf);

                PORT_START("VECTOR");
                PORT_CONFNAME( 0x01, 0x00, "Use vector rendering" );
                PORT_CONFSETTING(    0x00, DEF_STR( Off ) );
                PORT_CONFSETTING(    0x01, DEF_STR( On ) );

                PORT_INCLUDE(construct_ioport_fixedfreq_base_ports, ref errorbuf);

                PORT_ADJUSTERX(fixedfreq_tag_id_e.SCANLINE_HEIGHT, "Scanline Height", 10, 300);

                INPUT_PORTS_END();
            }


            //static INPUT_PORTS_START(fixedfreq_vector_ports)
            public void construct_ioport_fixedfreq_vector_ports(device_t owner, ioport_list portlist, ref string errorbuf)
            {
                INPUT_PORTS_START(owner, portlist, ref errorbuf);

                PORT_INCLUDE(construct_ioport_fixedfreq_base_ports, ref errorbuf);

                PORT_ADJUSTERX(fixedfreq_tag_id_e.SCANLINE_HEIGHT, "Scanline Height", 10, 300);

                INPUT_PORTS_END();
            }
        }

        protected override ioport_constructor device_input_ports()
        {
            LOG("input ports\n");

            if (m_divideo.has_screen())
            {
                if (m_divideo.screen().screen_type() == SCREEN_TYPE_RASTER)
                    return new construct_ioport_fixedfreq(this).construct_ioport_fixedfreq_raster_ports;
                else
                    return new construct_ioport_fixedfreq(this).construct_ioport_fixedfreq_vector_ports;
            }
            else
            {
                return null;
            }
        }


        public void vsync_end_cb(double refresh_time, uint32_t field)
        {
            var expected_frame_period = m_monitor.clock_period() * m_monitor.vtotal() * m_monitor.htotal();
            bool progressive = (m_enable.op0.read() & 8) == 8;

            double mult = 0.5;

            if (!progressive && (m_monitor.m_fieldcount == 2))
            {
                if (field == 0)
                {
                    m_last_rt = refresh_time;
                    return;
                }
                else
                    mult = 1.0;
            }

            var refresh_limited = std.min(4.0 * expected_frame_period, std.max((refresh_time + m_last_rt) * mult, 0.25 * expected_frame_period));

            m_last_rt = refresh_time;
            rectangle visarea = new rectangle(m_monitor.minh(), m_monitor.maxh(), m_monitor.minv(), m_monitor.maxv());

            // reset_origin must be called first.
            m_divideo.screen().reset_origin(
                    m_state.m_last_y - (m_monitor.vsync_width() + m_monitor.vbackporch_width()),
                    0);
            m_divideo.screen().configure(
                    m_monitor.htotal_scaled(), m_monitor.vtotal(), visarea,
                    DOUBLE_TO_ATTOSECONDS(refresh_limited));
        }
    }


    public static class fixfreq_global
    {
        public static fixedfreq_device FIXFREQ<bool_Required>(machine_config mconfig, device_finder<fixedfreq_device, bool_Required> finder) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, fixedfreq_device.FIXFREQ, 0); }
    }
}
