// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using u8 = System.Byte;
using uint8_t = System.Byte;


namespace mame
{
    partial class m52_state : driver_device
    {
        /* board mod changes? */
        int m_spritelimit;
        bool m_do_bg_fills;

        tilemap_t m_tx_tilemap;


        /* memory pointers */
        required_shared_ptr_uint8_t m_videoram;
        required_shared_ptr_uint8_t m_colorram;
        optional_shared_ptr_uint8_t m_spriteram;

        /* video-related */
        uint8_t                m_bg1xpos;
        uint8_t                m_bg1ypos;
        uint8_t                m_bg2xpos;
        uint8_t                m_bg2ypos;
        uint8_t                m_bgcontrol;


        required_device<cpu_device> m_maincpu;
        required_device<gfxdecode_device> m_sp_gfxdecode;
        required_device<gfxdecode_device> m_tx_gfxdecode;
        required_device<gfxdecode_device> m_bg_gfxdecode;
        required_device<screen_device> m_screen;
        required_device<palette_device> m_sp_palette;
        required_device<palette_device> m_tx_palette;
        required_device<palette_device> m_bg_palette;


        public m52_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_videoram = new required_shared_ptr_uint8_t(this, "videoram");
            m_colorram = new required_shared_ptr_uint8_t(this, "colorram");
            m_spriteram = new optional_shared_ptr_uint8_t(this, "spriteram");
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_sp_gfxdecode = new required_device<gfxdecode_device>(this, "sp_gfxdecode");
            m_tx_gfxdecode = new required_device<gfxdecode_device>(this, "tx_gfxdecode");
            m_bg_gfxdecode = new required_device<gfxdecode_device>(this, "bg_gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_sp_palette = new required_device<palette_device>(this, "sp_palette");
            m_tx_palette = new required_device<palette_device>(this, "tx_palette");
            m_bg_palette = new required_device<palette_device>(this, "bg_palette");
        }


        //void m52(machine_config &config);

        //DECLARE_WRITE8_MEMBER(m52_videoram_w);
        //DECLARE_WRITE8_MEMBER(m52_colorram_w);
        //DECLARE_READ8_MEMBER(m52_protection_r);


        //virtual void machine_reset() override;
        //virtual void video_start() override;
        //virtual DECLARE_WRITE8_MEMBER(m52_scroll_w);


        //DECLARE_WRITE8_MEMBER(m52_bg1ypos_w);
        //DECLARE_WRITE8_MEMBER(m52_bg1xpos_w);
        //DECLARE_WRITE8_MEMBER(m52_bg2xpos_w);
        //DECLARE_WRITE8_MEMBER(m52_bg2ypos_w);
        //DECLARE_WRITE8_MEMBER(m52_bgcontrol_w);
        //DECLARE_WRITE8_MEMBER(m52_flipscreen_w);
        //TILE_GET_INFO_MEMBER(get_tile_info);
        //void init_palette();
        //void init_sprite_palette(const int *resistances_3, const int *resistances_2, double *weights_r, double *weights_g, double *weights_b, double scale);
        //uint32_t screen_update_m52(screen_device &screen, bitmap_rgb32 &bitmap, const rectangle &cliprect);
        //void draw_background(bitmap_rgb32 &bitmap, const rectangle &cliprect, int xpos, int ypos, int image);
        //void draw_sprites(bitmap_rgb32 &bitmap, const rectangle &cliprect, int initoffs);

        //void main_map(address_map &map);
        //void main_portmap(address_map &map);


        // wrappers because I don't know how to find the correct device during construct_ startup

        //WRITE8_MEMBER( irem_audio_device::cmd_w )
        public void irem_audio_device_cmd_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            irem_audio_device device = (irem_audio_device)subdevice("irem_audio");
            device.cmd_w(space, offset, data, mem_mask);
        }
    }
}
