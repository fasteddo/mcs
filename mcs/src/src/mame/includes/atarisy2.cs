// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int8_t = System.SByte;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    partial class atarisy2_state : atarigen_state
    {
        required_device<t11_device> m_maincpu;
        required_device<m6502_device> m_audiocpu;
        required_device<atari_motion_objects_device> m_mob;
        required_shared_ptr_uint16_t m_slapstic_base;
        required_device<address_map_bank_device> m_vrambank;

        uint8_t m_interrupt_enable;

        required_device<tilemap_device> m_playfield_tilemap;
        required_device<tilemap_device> m_alpha_tilemap;

        int8_t m_pedal_count;

        required_device<atari_sound_comm_device> m_soundcomm;
        required_device<ym2151_device> m_ym2151;
        required_device_array_pokey_device m_pokey;  //required_device_array<pokey_device, 2> m_pokey;
        optional_device<tms5220c_device> m_tms5220;  //optional_device<tms5220_device> m_tms5220;

        uint8_t m_p2portwr_state;
        uint8_t m_p2portrd_state;

        required_memory_bank_array/*<2>*/ m_rombank;
        required_device<atari_slapstic_device> m_slapstic;

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

        output_finder/*<2>*/ m_leds;


        //static const atari_motion_objects_config s_mob_config;


        public atarisy2_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<t11_device>(this, "maincpu");
            m_audiocpu = new required_device<m6502_device>(this, "audiocpu");
            m_mob = new required_device<atari_motion_objects_device>(this, "mob");
            m_slapstic_base = new required_shared_ptr_uint16_t(this, "slapstic_base");
            m_vrambank = new required_device<address_map_bank_device>(this, "vrambank");
            m_playfield_tilemap = new required_device<tilemap_device>(this, "playfield");
            m_alpha_tilemap = new required_device<tilemap_device>(this, "alpha");
            m_soundcomm = new required_device<atari_sound_comm_device>(this, "soundcomm");
            m_ym2151 = new required_device<ym2151_device>(this, "ymsnd");
            m_pokey = new required_device_array_pokey_device(2, this, "pokey{0}", 1);
            m_tms5220 = new optional_device<tms5220c_device>(this, "tms");
            m_rombank = new required_memory_bank_array(2, this, "rombank{0}", 1);
            m_slapstic = new required_device<atari_slapstic_device>(this, "slapstic");
            m_leds = new output_finder(2, this, "led{0}", 0);
        }
    }
}
