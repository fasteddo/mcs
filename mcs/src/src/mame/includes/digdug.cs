// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using ioport_value = System.UInt32;
using uint8_t = System.Byte;


namespace mame
{
    partial class digdug_state : galaga_state
    {
        required_device<er2055_device> m_earom;
        required_shared_ptr_uint8_t m_digdug_objram;
        required_shared_ptr_uint8_t m_digdug_posram;
        required_shared_ptr_uint8_t m_digdug_flpram;

        uint8_t m_bg_select;
        uint8_t m_tx_color_mode;
        uint8_t m_bg_disable;
        uint8_t m_bg_color_bank;


        public digdug_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_earom = new required_device<er2055_device>(this, "earom");
            m_digdug_objram = new required_shared_ptr_uint8_t(this, "digdug_objram");
            m_digdug_posram = new required_shared_ptr_uint8_t(this, "digdug_posram");
            m_digdug_flpram = new required_shared_ptr_uint8_t(this, "digdug_flpram");
        }


        public required_device<er2055_device> earom { get { return m_earom; } }


        //void dzigzag(machine_config &config);
        //void digdug(machine_config &config);

        //TILEMAP_MAPPER_MEMBER(tilemap_scan);
        //TILE_GET_INFO_MEMBER(bg_get_tile_info);
        //TILE_GET_INFO_MEMBER(tx_get_tile_info);
        //DECLARE_VIDEO_START(digdug);
        //DECLARE_PALETTE_INIT(digdug);
        //uint32_t screen_update_digdug(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect);
        //void draw_sprites(bitmap_ind16 &bitmap, const rectangle &cliprect);
        //DECLARE_WRITE8_MEMBER(digdug_videoram_w);
        //DECLARE_WRITE8_MEMBER(bg_select_w);
        //DECLARE_WRITE_LINE_MEMBER(tx_color_mode_w);
        //DECLARE_WRITE_LINE_MEMBER(bg_disable_w);

        //DECLARE_READ8_MEMBER(earom_read);
        //DECLARE_WRITE8_MEMBER(earom_write);
        //DECLARE_WRITE8_MEMBER(earom_control_w);
        //virtual void machine_start() override;

        //void digdug_map(address_map &map);
    }
}
