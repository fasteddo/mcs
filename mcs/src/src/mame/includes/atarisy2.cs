// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using int8_t = System.SByte;
using required_region_ptr_u16 = mame.region_ptr_finder_u16<mame.bool_const_true>;  //using optional_region_ptr_u16 = region_ptr_finder_u16<false>;
using required_shared_ptr_u16 = mame.shared_ptr_finder_u16<mame.bool_const_true>;  //using optional_shared_ptr_u16 = shared_ptr_finder_u16<false>;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    partial class atarisy2_state : driver_device
    {
        required_device<t11_device> m_maincpu;
        required_device<m6502_device> m_audiocpu;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<screen_device> m_screen;
        required_device<atari_motion_objects_device> m_mob;
        required_region_ptr_u16 m_slapstic_region;

        uint8_t m_interrupt_enable;

        required_device<tilemap_device> m_playfield_tilemap;
        required_device<tilemap_device> m_alpha_tilemap;
        required_shared_ptr_u16 m_xscroll;
        required_shared_ptr_u16 m_yscroll;

        int8_t m_pedal_count;

        required_device<generic_latch_8_device> m_soundlatch;
        required_device<generic_latch_8_device> m_mainlatch;
        required_device<ym2151_device> m_ym2151;
        required_device_array<pokey_device, u32_const_2> m_pokey;
        optional_device<tms5220c_device> m_tms5220;  //optional_device<tms5220_device> m_tms5220;

        bool m_scanline_int_state;
        bool m_video_int_state;
        bool m_p2portwr_state;
        bool m_p2portrd_state;

        required_memory_bank_array<u32_const_2> m_rombank;
        required_device<atari_slapstic_device> m_slapstic;
        memory_view m_vmmu;
        required_shared_ptr_u16 m_playfieldt;
        required_shared_ptr_u16 m_playfieldb;

        uint8_t m_sound_reset_state;

        emu_timer m_yscroll_reset_timer;
        uint32_t [] m_playfield_tile_bank = new uint32_t [2];

        // 720 fake joystick
        //double          m_joy_last_angle;
        //int             m_joy_rotations;

        // 720 fake spinner
        //uint32_t          m_spin_last_rotate_count;
        //int32_t           m_spin_pos;                 /* track fake position of spinner */
        //uint32_t          m_spin_center_count;

        output_finder<u32_const_2> m_leds;


        //static const atari_motion_objects_config s_mob_config;


        public atarisy2_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<t11_device>(this, "maincpu");
            m_audiocpu = new required_device<m6502_device>(this, "audiocpu");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_mob = new required_device<atari_motion_objects_device>(this, "mob");
            m_slapstic_region = new required_region_ptr_u16(this, "maincpu");
            m_playfield_tilemap = new required_device<tilemap_device>(this, "playfield");
            m_alpha_tilemap = new required_device<tilemap_device>(this, "alpha");
            m_xscroll = new required_shared_ptr_u16(this, "xscroll");
            m_yscroll = new required_shared_ptr_u16(this, "yscroll");
            m_soundlatch = new required_device<generic_latch_8_device>(this, "soundlatch");
            m_mainlatch = new required_device<generic_latch_8_device>(this, "mainlatch");
            m_ym2151 = new required_device<ym2151_device>(this, "ymsnd");
            m_pokey = new required_device_array<pokey_device, u32_const_2>(this, "pokey{0}", 1, (base_, tag_) => { return new device_finder<pokey_device, bool_const_true>(base_, tag_); });
            m_tms5220 = new optional_device<tms5220c_device>(this, "tms");
            m_rombank = new required_memory_bank_array<u32_const_2>(this, "rombank{0}", 1);
            m_slapstic = new required_device<atari_slapstic_device>(this, "slapstic");
            m_vmmu = new memory_view(this, "vmmu");
            m_playfieldt = new required_shared_ptr_u16(this, "playfieldt");
            m_playfieldb = new required_shared_ptr_u16(this, "playfieldb");
            m_leds = new output_finder<u32_const_2>(this, "led{0}", 0);
        }
    }
}
