// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;


namespace mame
{
    partial class centiped_state : driver_device
    {
        required_device<m6502_device> m_maincpu;  //required_device<cpu_device> m_maincpu;

        uint8_t m_gfx_bank = 0;
        tilemap_t m_bg_tilemap = null;

        optional_shared_ptr<uint8_t> m_rambase;
        required_shared_ptr<uint8_t> m_videoram;
        required_shared_ptr<uint8_t> m_spriteram;
        optional_shared_ptr<uint8_t> m_paletteram;
        optional_shared_ptr<uint8_t> m_bullsdrt_tiles_bankram;

        required_device<ls259_device> m_outlatch;
        optional_device<er2055_device> m_earom;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<screen_device> m_screen;
        required_device<palette_device> m_palette;
        optional_device<ay8910_device> m_aysnd;

        uint8_t [] m_oldpos = new uint8_t[4];
        uint8_t [] m_sign = new uint8_t[4];
        uint8_t m_dsw_select = 0;
        uint8_t m_control_select = 0;
        uint8_t m_flipscreen = 0;
        uint8_t m_bullsdrt_sprites_bank = 0;
        uint8_t [] m_penmask = new uint8_t[64];


        public centiped_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<m6502_device>(this, "maincpu");
            m_rambase = new optional_shared_ptr<uint8_t>(this, "rambase");
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_spriteram = new required_shared_ptr<uint8_t>(this, "spriteram");
            m_paletteram = new optional_shared_ptr<uint8_t>(this, "paletteram");
            m_bullsdrt_tiles_bankram = new optional_shared_ptr<uint8_t>(this, "bullsdrt_bank");
            m_outlatch = new required_device<ls259_device>(this, "outlatch");
            m_earom = new optional_device<er2055_device>(this, "earom");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
            m_aysnd = new optional_device<ay8910_device>(this, "aysnd");
        }
    }
}
