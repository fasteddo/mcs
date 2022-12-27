// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;


namespace mame
{
    partial class fastfred_state : galaxold_state
    {
        required_device<ls259_device> m_outlatch;
        required_shared_ptr<uint8_t> m_videoram;
        required_shared_ptr<uint8_t> m_spriteram;
        required_shared_ptr<uint8_t> m_attributesram;
        optional_shared_ptr<uint8_t> m_background_color;
        optional_shared_ptr<uint8_t> m_imago_fg_videoram;

        int m_hardware_type;
        uint16_t m_charbank;
        uint8_t m_colorbank;
        uint8_t m_nmi_mask;
        uint8_t m_sound_nmi_mask;
        //uint8_t m_imago_sprites[0x800*3];
        //uint16_t m_imago_sprites_address;
        //uint8_t m_imago_sprites_bank;

        tilemap_t m_bg_tilemap;
        //tilemap_t *m_fg_tilemap;
        //tilemap_t *m_web_tilemap;


        public fastfred_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_outlatch = new required_device<ls259_device>(this, "outlatch");
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_spriteram = new required_shared_ptr<uint8_t>(this, "spriteram");
            m_attributesram = new required_shared_ptr<uint8_t>(this, "attributesram");
            m_background_color = new optional_shared_ptr<uint8_t>(this, "bgcolor");
            m_imago_fg_videoram = new optional_shared_ptr<uint8_t>(this, "imago_fg_vram");
        }
    }
}
