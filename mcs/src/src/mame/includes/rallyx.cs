// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;


namespace mame
{
    partial class rallyx_state : driver_device
    {
        // memory pointers
        required_shared_ptr<uint8_t> m_videoram;
        required_shared_ptr<uint8_t> m_radarattr;
        Pointer<uint8_t> m_spriteram;  //uint8_t *  m_spriteram;
        Pointer<uint8_t> m_spriteram2;  //uint8_t *  m_spriteram2;
        Pointer<uint8_t> m_radarx;  //uint8_t *  m_radarx;
        Pointer<uint8_t> m_radary;  //uint8_t *  m_radary;

        // video-related
        tilemap_t m_bg_tilemap;
        tilemap_t m_fg_tilemap;

        // misc
        //static constexpr unsigned JUNGLER_MAX_STARS = 1000;

        //struct jungler_star

        bool m_last_bang;
        uint8_t m_spriteram_base;
        bool m_stars_enable;
        //uint16_t  m_total_stars;
        uint8_t [] m_drawmode_table = new uint8_t [4];
        //struct jungler_star m_stars[JUNGLER_MAX_STARS];
        bool m_main_irq_mask;
        uint8_t m_interrupt_vector;

        // devices
        required_device<cpu_device> m_maincpu;
        optional_device<namco_device> m_namco_sound;
        optional_device<samples_device> m_samples;
        optional_device<timeplt_audio_device> m_timeplt_audio;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<palette_device> m_palette;
        required_device<screen_device> m_screen;


        public rallyx_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_radarattr = new required_shared_ptr<uint8_t>(this, "radarattr");
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_namco_sound = new optional_device<namco_device>(this, "namco");
            m_samples = new optional_device<samples_device>(this, "samples");
            m_timeplt_audio = new optional_device<timeplt_audio_device>(this, "timeplt_audio");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_palette = new required_device<palette_device>(this, "palette");
            m_screen = new required_device<screen_device>(this, "screen");
        }
    }
}
