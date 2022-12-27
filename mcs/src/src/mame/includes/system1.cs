// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using optional_memory_bank = mame.memory_bank_finder<mame.bool_const_false>;  //using optional_memory_bank = memory_bank_finder<false>;
using required_memory_bank = mame.memory_bank_finder<mame.bool_const_true>;  //using required_memory_bank = memory_bank_finder<true>;
using required_memory_region = mame.memory_region_finder<mame.bool_const_true>;  //using required_memory_region = memory_region_finder<true>;
using u8 = System.Byte;


namespace mame
{
    partial class system1_state : driver_device
    {
        // video related
        MemoryU8 m_videoram;  //std::unique_ptr<u8[]> m_videoram;
        Action<u8, u8> m_videomode_custom;  //void (system1_state::*m_videomode_custom)(u8 data, u8 prevdata);
        u8 m_videomode_prev;
        u8 [] m_mix_collide;  //std::unique_ptr<u8[]> m_mix_collide;
        u8 m_mix_collide_summary;
        u8 [] m_sprite_collide;  //std::unique_ptr<u8[]> m_sprite_collide;
        u8 m_sprite_collide_summary;
        bitmap_ind16 m_sprite_bitmap = new bitmap_ind16();
        u8 m_video_mode;
        u8 m_videoram_bank;
        tilemap_t [] m_tilemap_page = new tilemap_t [8];  //tilemap_t *m_tilemap_page[8];
        u8 m_tilemap_pages;

        // protection, miscs
        u8 m_mute_xor;
        u8 m_dakkochn_mux_data;
        u8 m_mcu_control;
        u8 m_nob_maincpu_latch;
        u8 m_nob_mcu_latch;
        u8 m_nob_mcu_status;
        //int m_nobb_inport23_step;


        // devices
        required_device<z80_device> m_maincpu;
        required_device<z80_device> m_soundcpu;  //required_device<cpu_device> m_soundcpu;
        optional_device<i8751_device> m_mcu;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<screen_device> m_screen;
        required_device<palette_device> m_palette;
        required_device<generic_latch_8_device> m_soundlatch;
        optional_device<i8255_device> m_ppi8255;
        optional_device<z80pio_device> m_pio;

        // shared pointers
        required_shared_ptr<u8> m_ram;
        required_shared_ptr<u8> m_spriteram;
        required_shared_ptr<u8> m_paletteram;
        optional_shared_ptr<u8> m_decrypted_opcodes;

        // memory regions
        required_memory_region m_maincpu_region;
        required_region_ptr<u8> m_spriterom;
        required_region_ptr<u8> m_lookup_prom;
        optional_region_ptr<u8> m_color_prom;

        // banks
        required_memory_bank m_bank1;
        optional_memory_bank m_bank0d;
        optional_memory_bank m_bank1d;
        MemoryU8 m_banked_decrypted_opcodes;  //std::unique_ptr<u8[]> m_banked_decrypted_opcodes;


        public system1_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_videoram = null;
            m_videomode_custom = null;
            m_maincpu = new required_device<z80_device>(this, "maincpu");
            m_soundcpu = new required_device<z80_device>(this, "soundcpu");
            m_mcu = new optional_device<i8751_device>(this, "mcu");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
            m_soundlatch = new required_device<generic_latch_8_device>(this, "soundlatch");
            m_ppi8255 = new optional_device<i8255_device>(this, "ppi8255");
            m_pio = new optional_device<z80pio_device>(this, "pio");
            m_ram = new required_shared_ptr<u8>(this, "ram");
            m_spriteram = new required_shared_ptr<u8>(this, "spriteram");
            m_paletteram = new required_shared_ptr<u8>(this, "paletteram");
            m_decrypted_opcodes = new optional_shared_ptr<u8>(this, "decrypted_opcodes");
            m_maincpu_region = new required_memory_region(this, "maincpu");
            m_spriterom = new required_region_ptr<u8>(this, "sprites");
            m_lookup_prom = new required_region_ptr<u8>(this, "lookup_proms");
            m_color_prom = new optional_region_ptr<u8>(this, "color_proms");
            m_bank1 = new required_memory_bank(this, "bank1");
            m_bank0d = new optional_memory_bank(this, "bank0d");
            m_bank1d = new optional_memory_bank(this, "bank1d");
            m_banked_decrypted_opcodes = null;
        }
    }
}
