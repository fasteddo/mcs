// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;


namespace mame
{
    public partial class _1942_state : driver_device
    {
        /* memory pointers */
        required_shared_ptr_byte m_spriteram;
        required_shared_ptr_byte m_fg_videoram;
        required_shared_ptr_byte m_bg_videoram;

        required_device<cpu_device> m_audiocpu;
        required_device<cpu_device> m_maincpu;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<palette_device> m_palette;
        required_device<generic_latch_8_device> m_soundlatch;

        /* video-related */
        //tilemap_t *m_fg_tilemap;
        //tilemap_t *m_bg_tilemap;
        //int m_palette_bank;
        //UINT8 m_scroll[2];


        public _1942_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_spriteram = new required_shared_ptr_byte(this, "spriteram");
            m_fg_videoram = new required_shared_ptr_byte(this, "fg_videoram");
            m_bg_videoram = new required_shared_ptr_byte(this, "bg_videoram");
            m_audiocpu = new required_device<cpu_device>(this, "audiocpu");
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_palette = new required_device<palette_device>(this, "palette");
            m_soundlatch = new required_device<generic_latch_8_device>(this, "soundlatch");
        }


        public required_device<palette_device> palette { get { return m_palette; } }
        public required_device<generic_latch_8_device> soundlatch { get { return m_soundlatch; } }


        //void driver_init() override;

        //TILE_GET_INFO_MEMBER(get_fg_tile_info);
        //TILE_GET_INFO_MEMBER(get_bg_tile_info);

        //void _1942(machine_config &config);

        //void machine_start() override;
        //void machine_reset() override;
        //void video_start() override;

        //void _1942_map(address_map &map);
        //void sound_map(address_map &map);

        //DECLARE_WRITE8_MEMBER(_1942_bankswitch_w);
        //DECLARE_WRITE8_MEMBER(_1942_fgvideoram_w);
        //DECLARE_WRITE8_MEMBER(_1942_bgvideoram_w);
        //DECLARE_WRITE8_MEMBER(_1942_palette_bank_w);
        //DECLARE_WRITE8_MEMBER(_1942_scroll_w);
        //DECLARE_WRITE8_MEMBER(_1942_c804_w);
        //DECLARE_PALETTE_INIT(1942);
        //TIMER_DEVICE_CALLBACK_MEMBER(_1942_scanline);
        //uint32_t screen_update(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect);
        //virtual void draw_sprites(bitmap_ind16 &bitmap, const rectangle &cliprect);
    }
}
