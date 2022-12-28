// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using int8_t = System.SByte;
using int16_t = System.Int16;
using required_shared_ptr_u16 = mame.shared_ptr_finder_u16<mame.bool_const_true>;  //using optional_shared_ptr_u16 = shared_ptr_finder_u16<false>;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;


namespace mame
{
    partial class polepos_state : driver_device
    {
        required_device<z80_device> m_maincpu;  //required_device<cpu_device> m_maincpu;
        required_device<z8002_device> m_subcpu;  //required_device<cpu_device> m_subcpu;
        required_device<z8002_device> m_subcpu2;  //required_device<cpu_device> m_subcpu2;
        optional_device<cpu_device> m_sound_z80;
        optional_device<generic_latch_8_device> m_soundlatch;
        optional_device<namco_device> m_namco_sound;
        required_device<ls259_device> m_latch;
        required_device<adc0804_device> m_adc;
        required_shared_ptr_u16 m_sprite16_memory;
        required_shared_ptr_u16 m_road16_memory;
        required_shared_ptr_u16 m_alpha16_memory;
        required_shared_ptr_u16 m_view16_memory;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<screen_device> m_screen;
        required_device<palette_device> m_palette;

        uint8_t m_steer_last = 0;
        uint8_t m_steer_delta = 0;
        int16_t m_steer_accum = 0;
        int16_t m_last_result = 0;
        int8_t m_last_signed = 0;
        uint8_t m_last_unsigned = 0;
        int m_adc_input = 0;
        int m_auto_start_mask = 0;

        uint16_t [] m_vertical_position_modifier = new uint16_t [256];
        uint16_t m_road16_vscroll = 0;
        tilemap_t m_bg_tilemap = null;
        tilemap_t m_tx_tilemap = null;
        int m_chacl = 0;
        uint16_t m_scroll = 0;
        uint8_t m_sub_irq_mask = 0;


        public polepos_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_maincpu = new required_device<z80_device>(this, "maincpu");
            m_subcpu = new required_device<z8002_device>(this, "sub");
            m_subcpu2 = new required_device<z8002_device>(this, "sub2");
            m_sound_z80 = new optional_device<cpu_device>(this, "soundz80bl");
            m_soundlatch = new optional_device<generic_latch_8_device>(this, "soundlatch");
            m_namco_sound = new optional_device<namco_device>(this, "namco");
            m_latch = new required_device<ls259_device>(this, "latch");
            m_adc = new required_device<adc0804_device>(this, "adc");
            m_sprite16_memory = new required_shared_ptr_u16(this, "sprite16_memory");
            m_road16_memory = new required_shared_ptr_u16(this, "road16_memory");
            m_alpha16_memory = new required_shared_ptr_u16(this, "alpha16_memory");
            m_view16_memory = new required_shared_ptr_u16(this, "view16_memory");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
        }
    }
}
