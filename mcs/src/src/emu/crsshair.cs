// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using screen_device_enumerator = mame.device_type_enumerator<mame.screen_device>;  //typedef device_type_enumerator<screen_device> screen_device_enumerator;
using u8 = System.Byte;
using u16 = System.UInt16;


namespace mame
{
    public static class crsshair_global
    {
        /* user settings for visibility mode */
        public const int CROSSHAIR_VISIBILITY_OFF                = 0;
        public const int CROSSHAIR_VISIBILITY_ON                 = 1;
        public const int CROSSHAIR_VISIBILITY_AUTO               = 2;
        public const int CROSSHAIR_VISIBILITY_DEFAULT            = CROSSHAIR_VISIBILITY_AUTO;

        /* range allowed for auto visibility */
        public const int CROSSHAIR_VISIBILITY_AUTOTIME_MIN           = 0;
        public const int CROSSHAIR_VISIBILITY_AUTOTIME_MAX           = 50;
        public const int CROSSHAIR_VISIBILITY_AUTOTIME_DEFAULT       = 15;

        public const int CROSSHAIR_RAW_SIZE      = 100;
        public const int CROSSHAIR_RAW_ROWBYTES  = ((CROSSHAIR_RAW_SIZE + 7) / 8);
    }


    // ======================> render_crosshair
    class render_crosshair : global_object, IDisposable
    {
        /* raw bitmap */
        static readonly byte [] crosshair_raw_top =
        {
            0x00,0x20,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x40,0x00,
            0x00,0x70,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xe0,0x00,
            0x00,0xf8,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0xf0,0x00,
            0x01,0xf8,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0xf8,0x00,
            0x03,0xfc,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x03,0xfc,0x00,
            0x07,0xfe,0x00,0x00,0x00,0x0f,0xfe,0x00,0x00,0x00,0x07,0xfe,0x00,
            0x0f,0xff,0x00,0x00,0x01,0xff,0xff,0xf0,0x00,0x00,0x0f,0xff,0x00,
            0x1f,0xff,0x80,0x00,0x1f,0xff,0xff,0xff,0x00,0x00,0x1f,0xff,0x80,
            0x3f,0xff,0x80,0x00,0xff,0xff,0xff,0xff,0xe0,0x00,0x1f,0xff,0xc0,
            0x7f,0xff,0xc0,0x03,0xff,0xff,0xff,0xff,0xf8,0x00,0x3f,0xff,0xe0,
            0xff,0xff,0xe0,0x07,0xff,0xff,0xff,0xff,0xfc,0x00,0x7f,0xff,0xf0,
            0x7f,0xff,0xf0,0x1f,0xff,0xff,0xff,0xff,0xff,0x00,0xff,0xff,0xe0,
            0x3f,0xff,0xf8,0x7f,0xff,0xff,0xff,0xff,0xff,0xc1,0xff,0xff,0xc0,
            0x0f,0xff,0xf8,0xff,0xff,0xff,0xff,0xff,0xff,0xe1,0xff,0xff,0x00,
            0x07,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xfb,0xff,0xfe,0x00,
            0x03,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xfc,0x00,
            0x01,0xff,0xff,0xff,0xff,0xf0,0x01,0xff,0xff,0xff,0xff,0xf8,0x00,
            0x00,0x7f,0xff,0xff,0xff,0x00,0x00,0x1f,0xff,0xff,0xff,0xe0,0x00,
            0x00,0x3f,0xff,0xff,0xf8,0x00,0x00,0x03,0xff,0xff,0xff,0xc0,0x00,
            0x00,0x1f,0xff,0xff,0xe0,0x00,0x00,0x00,0xff,0xff,0xff,0x80,0x00,
            0x00,0x0f,0xff,0xff,0x80,0x00,0x00,0x00,0x3f,0xff,0xff,0x00,0x00,
            0x00,0x03,0xff,0xfe,0x00,0x00,0x00,0x00,0x0f,0xff,0xfc,0x00,0x00,
            0x00,0x01,0xff,0xfc,0x00,0x00,0x00,0x00,0x07,0xff,0xf8,0x00,0x00,
            0x00,0x03,0xff,0xf8,0x00,0x00,0x00,0x00,0x01,0xff,0xf8,0x00,0x00,
            0x00,0x07,0xff,0xfc,0x00,0x00,0x00,0x00,0x03,0xff,0xfc,0x00,0x00,
            0x00,0x0f,0xff,0xfe,0x00,0x00,0x00,0x00,0x07,0xff,0xfe,0x00,0x00,
            0x00,0x0f,0xff,0xff,0x00,0x00,0x00,0x00,0x0f,0xff,0xfe,0x00,0x00,
            0x00,0x1f,0xff,0xff,0x80,0x00,0x00,0x00,0x1f,0xff,0xff,0x00,0x00,
            0x00,0x1f,0xff,0xff,0x80,0x00,0x00,0x00,0x1f,0xff,0xff,0x00,0x00,
            0x00,0x3f,0xfe,0xff,0xc0,0x00,0x00,0x00,0x3f,0xff,0xff,0x80,0x00,
            0x00,0x7f,0xfc,0x7f,0xe0,0x00,0x00,0x00,0x7f,0xe7,0xff,0xc0,0x00,
            0x00,0x7f,0xf8,0x3f,0xf0,0x00,0x00,0x00,0xff,0xc3,0xff,0xc0,0x00,
            0x00,0xff,0xf8,0x1f,0xf8,0x00,0x00,0x01,0xff,0x83,0xff,0xe0,0x00,
            0x00,0xff,0xf0,0x07,0xf8,0x00,0x00,0x01,0xfe,0x01,0xff,0xe0,0x00,
            0x00,0xff,0xf0,0x03,0xfc,0x00,0x00,0x03,0xfc,0x01,0xff,0xe0,0x00,
            0x01,0xff,0xe0,0x01,0xfe,0x00,0x00,0x07,0xf8,0x00,0xff,0xf0,0x00,
            0x01,0xff,0xe0,0x00,0xff,0x00,0x00,0x0f,0xf0,0x00,0xff,0xf0,0x00,
            0x01,0xff,0xc0,0x00,0x3f,0x80,0x00,0x1f,0xc0,0x00,0x7f,0xf0,0x00,
            0x01,0xff,0xc0,0x00,0x1f,0x80,0x00,0x1f,0x80,0x00,0x7f,0xf0,0x00,
            0x03,0xff,0xc0,0x00,0x0f,0xc0,0x00,0x3f,0x00,0x00,0x7f,0xf8,0x00,
            0x03,0xff,0x80,0x00,0x07,0xe0,0x00,0x7e,0x00,0x00,0x3f,0xf8,0x00,
            0x03,0xff,0x80,0x00,0x01,0xf0,0x00,0xf8,0x00,0x00,0x3f,0xf8,0x00,
            0x03,0xff,0x80,0x00,0x00,0xf8,0x01,0xf0,0x00,0x00,0x3f,0xf8,0x00,
            0x03,0xff,0x80,0x00,0x00,0x78,0x01,0xe0,0x00,0x00,0x3f,0xf8,0x00,
            0x07,0xff,0x00,0x00,0x00,0x3c,0x03,0xc0,0x00,0x00,0x3f,0xfc,0x00,
            0x07,0xff,0x00,0x00,0x00,0x0e,0x07,0x00,0x00,0x00,0x1f,0xfc,0x00,
            0x07,0xff,0x00,0x00,0x00,0x07,0x0e,0x00,0x00,0x00,0x1f,0xfc,0x00,
            0x07,0xff,0x00,0x00,0x00,0x03,0x9c,0x00,0x00,0x00,0x1f,0xfc,0x00,
            0x07,0xff,0x00,0x00,0x00,0x01,0x98,0x00,0x00,0x00,0x1f,0xfc,0x00,
            0x07,0xff,0x00,0x00,0x00,0x00,0x60,0x00,0x00,0x00,0x1f,0xfc,0x00
        };


        /* per-player colors */
        static readonly rgb_t [] crosshair_colors =
        {
            new rgb_t(0x40,0x40,0xff),
            new rgb_t(0xff,0x40,0x40),
            new rgb_t(0x40,0xff,0x40),
            new rgb_t(0xff,0xff,0x40),
            new rgb_t(0xff,0x40,0xff),
            new rgb_t(0x40,0xff,0xff),
            new rgb_t(0xff,0xff,0xff)
        };


        // private state
        running_machine m_machine;  // reference to our machine
        int m_player;   // player number
        bool m_used;     // usage for this crosshair
        u8 m_mode;     // visibility mode for this crosshair
        bool m_visible;  // visibility for this crosshair
        bitmap_argb32 m_bitmap;  //std::unique_ptr<bitmap_argb32>  m_bitmap;    // bitmap for this crosshair
        render_texture m_texture;  // texture for this crosshair
        screen_device m_screen;   // the screen on which this crosshair is drawn
        float m_x;        // current X position
        float m_y;        // current Y position
        float m_last_x;   // last X position
        float m_last_y;   // last Y position
        u16 m_time;     // time since last movement
        string m_name = "";     // name of png file


        // construction/destruction
        //-------------------------------------------------
        //  render_crosshair - constructor
        //-------------------------------------------------
        public render_crosshair(running_machine machine, int player)
        {
            m_machine = machine;
            m_player = player;
            m_used = false;
            m_mode = CROSSHAIR_VISIBILITY_OFF;
            m_visible = false;
            m_texture = null;
            m_x = 0.0f;
            m_y = 0.0f;
            m_last_x = 0.0f;
            m_last_y = 0.0f;
            m_time = 0;


            // for now, use the main screen
            m_screen = new screen_device_enumerator(machine.root_device()).first();
        }

        ~render_crosshair()
        {
            assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            m_machine.render().texture_free(m_texture);
            m_isDisposed = true;
        }


        // getters
        //running_machine &machine() const { return m_machine; }
        //int player() const { return m_player; }
        //bool is_used() const { return m_used; }
        //u8 mode() const { return m_mode; }
        //bool is_visible() const { return m_visible; }
        //screen_device *screen() const { return m_screen; }
        //float x() const { return m_x; }
        //float y() const { return m_y; }
        //const char *bitmap_name() const { return m_name.c_str(); }

        // setters
        public void set_used(bool used) { m_used = used; }
        public void set_mode(byte mode) { m_mode = mode; }
        public void set_visible(bool visible) { m_visible = visible; }
        //void set_screen(screen_device *screen) { m_screen = screen; }
        //void setxy(float x, float y);
        //void set_bitmap_name(const char *name);

        //-------------------------------------------------
        //  set_default_bitmap - reset to default bitmap
        //-------------------------------------------------
        public void set_default_bitmap()
        {
            // update bitmap if name has changed
            bool changed = !m_name.empty();
            m_name = "";
            if (changed || m_bitmap == null)
                create_bitmap();
        }

        // updates
        //void animate(u16 auto_time);
        //void draw(render_container &container, u8 fade);


        // private helpers
        //-------------------------------------------------
        //  create_bitmap - create the rendering
        //  structures for the given player
        //-------------------------------------------------
        void create_bitmap()
        {
            rgb_t color = m_player < std.size(crosshair_colors) ? crosshair_colors[m_player] : rgb_t.white();

            // if we have a bitmap and texture for this player, kill it
            if (m_bitmap == null)
            {
                m_bitmap = new bitmap_argb32();
                m_texture = m_machine.render().texture_alloc(render_texture.hq_scale);
            }
            else
            {
                m_bitmap.reset();
            }

            emu_file crossfile = new emu_file(m_machine.options().crosshair_path(), OPEN_FLAG_READ);
            if (!m_name.empty())
            {
                // look for user specified file
                if (crossfile.open(m_name + ".png") == osd_file.error.NONE)
                {
                    render_load_png(out m_bitmap, crossfile.core_file_get());
                    crossfile.close();
                }
            }
            else
            {
                // look for default cross?.png in crsshair/game dir
                string filename = string_format("cross{0}.png", m_player + 1);
                if (crossfile.open(m_machine.system().name + (PATH_SEPARATOR + filename)) == osd_file.error.NONE)
                {
                    render_load_png(out m_bitmap, crossfile.core_file_get());
                    crossfile.close();
                }

                // look for default cross?.png in crsshair dir
                if (!m_bitmap.valid() && (crossfile.open(filename) == osd_file.error.NONE))
                {
                    render_load_png(out m_bitmap, crossfile.core_file_get());
                    crossfile.close();
                }
            }

            /* if that didn't work, use the built-in one */
            if (!m_bitmap.valid())
            {
                /* allocate a blank bitmap to start with */
                m_bitmap.allocate(CROSSHAIR_RAW_SIZE, CROSSHAIR_RAW_SIZE);
                m_bitmap.fill(new rgb_t(0x00,0xff,0xff,0xff));

                /* extract the raw source data to it */
                for (int y = 0; y < CROSSHAIR_RAW_SIZE / 2; y++)
                {
                    /* assume it is mirrored vertically */
                    PointerU32 dest0 = m_bitmap.pix(y);  //u32 *dest0 = &m_bitmap->pix(y);
                    PointerU32 dest1 = m_bitmap.pix(CROSSHAIR_RAW_SIZE - 1 - y);  //u32 *dest1 = &m_bitmap->pix(CROSSHAIR_RAW_SIZE - 1 - y);

                    /* extract to two rows simultaneously */
                    for (int x = 0; x < CROSSHAIR_RAW_SIZE; x++)
                    {
                        if (((crosshair_raw_top[y * CROSSHAIR_RAW_ROWBYTES + x / 8] << (x % 8)) & 0x80) != 0)
                        {
                            dest0[x] = dest1[x] = new rgb_t(0xff,0x00,0x00,0x00) | color;
                        }
                    }
                }
            }

            /* reference the new bitmap */
            m_texture.set_bitmap(m_bitmap, m_bitmap.cliprect(), texture_format.TEXFORMAT_ARGB32);
        }


        //void update_position();
    }


    // ======================> crosshair_manager
    public class crosshair_manager : global_object
    {
        // internal state
        running_machine m_machine;                  // reference to our machine

        bool m_usage;                    // true if any crosshairs are used
        render_crosshair [] m_crosshair = new render_crosshair[MAX_PLAYERS];  //std::unique_ptr<render_crosshair> m_crosshair[MAX_PLAYERS]; // per-player crosshair state
        //u8                  m_fade;                     // color fading factor
        u8 m_animation_counter;        // animation frame index
        u16 m_auto_time;                // time in seconds to turn invisible


        // construction/destruction
        //-------------------------------------------------
        //  crosshair_manager - constructor
        //-------------------------------------------------
        public crosshair_manager(running_machine machine)
        {
            m_machine = machine;
            m_usage = false;
            m_animation_counter = 0;
            m_auto_time = CROSSHAIR_VISIBILITY_AUTOTIME_DEFAULT;


            /* request a callback upon exiting */
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_EXIT, exit);

            for (int player = 0; player < MAX_PLAYERS; player++)
                m_crosshair[player] = new render_crosshair(machine, player);

            /* determine who needs crosshairs */
            foreach (var port in machine.ioport().ports())
            {
                foreach (ioport_field field in port.Value.fields())
                {
                    if (field.crosshair_axis() != crosshair_axis_t.CROSSHAIR_AXIS_NONE)
                    {
                        int player = field.player();

                        assert(player < MAX_PLAYERS);

                        /* mark as used and set the default visibility and mode */
                        m_usage = true;
                        m_crosshair[player].set_used(true);
                        m_crosshair[player].set_mode(CROSSHAIR_VISIBILITY_DEFAULT);
                        m_crosshair[player].set_visible(CROSSHAIR_VISIBILITY_DEFAULT != CROSSHAIR_VISIBILITY_OFF);
                        m_crosshair[player].set_default_bitmap();
                    }
                }
            }

            /* register callbacks for when we load/save configurations */
            if (m_usage)
                machine.configuration().config_register("crosshairs", config_load, config_save);

            /* register the animation callback */
            screen_device first_screen = new screen_device_enumerator(machine.root_device()).first();
            if (first_screen != null)
                first_screen.register_vblank_callback(animate);
        }


        /* draws crosshair(s) in a given screen, if necessary */
        public void render(screen_device screen)
        {
            //throw new emu_unimplemented();
        }


        // return true if any crosshairs are used
        //bool get_usage() const { return m_usage; }

        // getters
        //running_machine &machine() const { return m_machine; }
        //render_crosshair &get_crosshair(int player) const { assert(player >= 0 && player < MAX_PLAYERS); assert(m_crosshair[player] != nullptr); return *m_crosshair[player]; }
        //u16 auto_time() const { return m_auto_time; }
        //void set_auto_time(u16 auto_time) { m_auto_time = auto_time; }


        /*-------------------------------------------------
        exit - free memory allocated for
        the crosshairs
        -------------------------------------------------*/
        void exit(running_machine machine_)
        {
            /* free bitmaps and textures for each player */
            for (int player = 0; player < MAX_PLAYERS; player++)
            {
                m_crosshair[player].Dispose();
                m_crosshair[player] = null;
            }
        }


        void animate(screen_device device, bool vblank_state)
        {
            //throw new emu_unimplemented();
        }


        void config_load(config_type cfg_type, util.xml.data_node parentnode) { throw new emu_unimplemented(); }
        void config_save(config_type cfg_type, util.xml.data_node parentnode) { throw new emu_unimplemented(); }
    }
}
