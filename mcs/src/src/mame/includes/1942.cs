// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;


namespace mame
{
    partial class _1942_state : driver_device
    {
        required_device<screen_device> m_screen;

        /* memory pointers */
        required_shared_ptr<uint8_t> m_spriteram;
        required_shared_ptr<uint8_t> m_fg_videoram;
        required_shared_ptr<uint8_t> m_bg_videoram;

        required_device<cpu_device> m_audiocpu;
        required_device<cpu_device> m_maincpu;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<palette_device> m_palette;
        required_device<generic_latch_8_device> m_soundlatch;

        /* video-related */
        tilemap_t m_fg_tilemap;
        tilemap_t m_bg_tilemap;
        int m_palette_bank;
        uint8_t [] m_scroll = new uint8_t[2];
        uint8_t [,] m_sprite_bufs = new uint8_t[2, 512];


        public _1942_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_screen = new required_device<screen_device>(this, "screen");
            m_spriteram = new required_shared_ptr<uint8_t>(this, "spriteram");
            m_fg_videoram = new required_shared_ptr<uint8_t>(this, "fg_videoram");
            m_bg_videoram = new required_shared_ptr<uint8_t>(this, "bg_videoram");
            m_audiocpu = new required_device<cpu_device>(this, "audiocpu");
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_palette = new required_device<palette_device>(this, "palette");
            m_soundlatch = new required_device<generic_latch_8_device>(this, "soundlatch");
        }
    }
}
