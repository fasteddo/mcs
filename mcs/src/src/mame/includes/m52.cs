// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;


namespace mame
{
    partial class m52_state : driver_device
    {
        /* board mod changes? */
        int m_spritelimit = 0;
        bool m_do_bg_fills = false;

        tilemap_t m_tx_tilemap = null;

        required_device<z80_device> m_maincpu;  //required_device<cpu_device> m_maincpu;
        required_device<screen_device> m_screen;


        /* memory pointers */
        required_shared_ptr<uint8_t> m_videoram;
        required_shared_ptr<uint8_t> m_colorram;
        optional_shared_ptr<uint8_t> m_spriteram;

        /* video-related */
        uint8_t m_bg1xpos = 0;
        uint8_t m_bg1ypos = 0;
        uint8_t m_bg2xpos = 0;
        uint8_t m_bg2ypos = 0;
        uint8_t m_bgcontrol = 0;


        required_device<gfxdecode_device> m_sp_gfxdecode;
        required_device<gfxdecode_device> m_tx_gfxdecode;
        required_device<gfxdecode_device> m_bg_gfxdecode;
        required_device<palette_device> m_sp_palette;
        required_device<palette_device> m_tx_palette;
        required_device<palette_device> m_bg_palette;


        public m52_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<z80_device>(this, "maincpu");
            m_screen = new required_device<screen_device>(this, "screen");
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_colorram = new required_shared_ptr<uint8_t>(this, "colorram");
            m_spriteram = new optional_shared_ptr<uint8_t>(this, "spriteram");
            m_sp_gfxdecode = new required_device<gfxdecode_device>(this, "sp_gfxdecode");
            m_tx_gfxdecode = new required_device<gfxdecode_device>(this, "tx_gfxdecode");
            m_bg_gfxdecode = new required_device<gfxdecode_device>(this, "bg_gfxdecode");
            m_sp_palette = new required_device<palette_device>(this, "sp_palette");
            m_tx_palette = new required_device<palette_device>(this, "tx_palette");
            m_bg_palette = new required_device<palette_device>(this, "bg_palette");
        }
    }
}
