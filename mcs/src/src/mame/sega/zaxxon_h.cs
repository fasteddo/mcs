// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;

using static mame.emucore_global;


namespace mame
{
    partial class zaxxon_state : driver_device
    {
        required_device<z80_device> m_maincpu;
        required_device_array<ls259_device, u32_const_2> m_mainlatch;
        optional_device<i8255_device> m_ppi;
        optional_device<samples_device> m_samples;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<palette_device> m_palette;

        optional_ioport_array<u32_const_2> m_dials;

        required_shared_ptr<uint8_t> m_videoram;
        memory_share_creator<uint8_t> m_spriteram;
        optional_shared_ptr<uint8_t> m_colorram;
        optional_shared_ptr<uint8_t> m_decrypted_opcodes;

        //uint8_t m_int_enabled = 0;
        //uint8_t m_coin_status[3]{};

        //uint8_t m_razmataz_dial_pos[2]{};
        //uint16_t m_razmataz_counter = 0;

        //uint8_t m_sound_state[3]{};
        //uint8_t m_bg_enable = 0;
        //uint8_t m_bg_color = 0;
        //uint16_t m_bg_position = 0;
        //uint8_t m_fg_color = 0;
        //bool m_flip_screen = false;

        //uint8_t m_congo_fg_bank = 0;
        //uint8_t m_congo_color_bank = 0;
        //uint8_t m_congo_custom[4]{};

        //const uint8_t *m_color_codes;
        //tilemap_t *m_fg_tilemap = nullptr;
        //tilemap_t *m_bg_tilemap = nullptr;


        public zaxxon_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_maincpu = new required_device<z80_device>(this, "maincpu");
            m_mainlatch = new required_device_array<ls259_device, u32_const_2>(this, "mainlatch{0}", 1, (base_, tag_) => { return new device_finder<ls259_device, bool_const_true>(base_, tag_); });
            m_ppi = new optional_device<i8255_device>(this, "ppi8255");
            m_samples = new optional_device<samples_device>(this, "samples");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_palette = new required_device<palette_device>(this, "palette");
            m_dials = new optional_ioport_array<u32_const_2>(this, "DIAL.{0}", 0);
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_spriteram = new memory_share_creator<uint8_t>(this, "spriteram", 0x100, ENDIANNESS_LITTLE);
            m_colorram = new optional_shared_ptr<uint8_t>(this, "colorram");
            m_decrypted_opcodes = new optional_shared_ptr<uint8_t>(this, "decrypted_opcodes");
        }


        protected override void machine_start() { throw new emu_unimplemented(); }
        protected override void video_start() { throw new emu_unimplemented(); }
    }
}
