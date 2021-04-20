// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using uint8_t = System.Byte;


namespace mame
{
    partial class taitosj_state : driver_device
    {
        required_shared_ptr<uint8_t> m_videoram_1;
        required_shared_ptr<uint8_t> m_videoram_2;
        required_shared_ptr<uint8_t> m_videoram_3;
        required_shared_ptr<uint8_t> m_spriteram;
        required_shared_ptr<uint8_t> m_paletteram;
        required_shared_ptr<uint8_t> m_characterram;
        required_shared_ptr<uint8_t> m_scroll;
        required_shared_ptr<uint8_t> m_colscrolly;
        required_shared_ptr<uint8_t> m_gfxpointer;
        required_shared_ptr<uint8_t> m_colorbank;
        required_shared_ptr<uint8_t> m_video_mode;
        required_shared_ptr<uint8_t> m_video_priority;
        required_shared_ptr<uint8_t> m_collision_reg;
        optional_shared_ptr<uint8_t> m_kikstart_scrollram;

        required_device<cpu_device> m_maincpu;
        required_device<cpu_device> m_audiocpu;
        optional_device<taito_sj_security_mcu_device> m_mcu;
        required_device<input_merger_device> m_soundnmi;
        required_device<input_merger_device> m_soundnmi2;
        required_device<dac_8bit_r2r_device> m_dac;
        required_device<discrete_sound_device> m_dacvol;
        required_device<ay8910_device> m_ay1;
        required_device<ay8910_device> m_ay2;
        required_device<ay8910_device> m_ay3;
        required_device<ay8910_device> m_ay4;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<screen_device> m_screen;
        required_device<palette_device> m_palette;

        delegate void copy_layer_func_t(bitmap_ind16 bitmap, rectangle cliprect, int which, int [] sprites_on, rectangle [] sprite_areas);  //typedef void (taitosj_state::*copy_layer_func_t)(bitmap_ind16 &, const rectangle &, int, int *, rectangle *);

        uint8_t m_input_port_4_f0;
        uint8_t [] m_kikstart_gears = new uint8_t [2];

        uint8_t m_spacecr_prot_value;
        uint8_t m_protection_value;
        //uint32_t m_address;
        uint8_t m_soundlatch_data;
        bool m_soundlatch_flag;  // 74ls74 1/2 @ GAME BOARD IC42
        bool m_sound_semaphore2;  // 74ls74 2/2 @ GAME BOARD IC42
        bitmap_ind16 [] m_layer_bitmap = new bitmap_ind16[3];
        bitmap_ind16 m_sprite_sprite_collbitmap1 = new bitmap_ind16();
        bitmap_ind16 m_sprite_sprite_collbitmap2 = new bitmap_ind16();
        bitmap_ind16 m_sprite_layer_collbitmap1 = new bitmap_ind16();
        bitmap_ind16 [] m_sprite_layer_collbitmap2 = new bitmap_ind16[3];
        int [,] m_draw_order = new int[32, 4];


        public taitosj_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_videoram_1 = new required_shared_ptr<uint8_t>(this, "videoram_1");
            m_videoram_2 = new required_shared_ptr<uint8_t>(this, "videoram_2");
            m_videoram_3 = new required_shared_ptr<uint8_t>(this, "videoram_3");
            m_spriteram = new required_shared_ptr<uint8_t>(this, "spriteram");
            m_paletteram = new required_shared_ptr<uint8_t>(this, "paletteram");
            m_characterram = new required_shared_ptr<uint8_t>(this, "characterram");
            m_scroll = new required_shared_ptr<uint8_t>(this, "scroll");
            m_colscrolly = new required_shared_ptr<uint8_t>(this, "colscrolly");
            m_gfxpointer = new required_shared_ptr<uint8_t>(this, "gfxpointer");
            m_colorbank = new required_shared_ptr<uint8_t>(this, "colorbank");
            m_video_mode = new required_shared_ptr<uint8_t>(this, "video_mode");
            m_video_priority = new required_shared_ptr<uint8_t>(this, "video_priority");
            m_collision_reg = new required_shared_ptr<uint8_t>(this, "collision_reg");
            m_kikstart_scrollram = new optional_shared_ptr<uint8_t>(this, "kikstart_scroll");  //m_kikstart_scrollram = new required_shared_ptr_uint8_t(this, "kikstart_scroll");
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_audiocpu = new required_device<cpu_device>(this, "audiocpu");
            m_mcu = new optional_device<taito_sj_security_mcu_device>(this, "bmcu");
            m_soundnmi = new required_device<input_merger_device>(this, "soundnmi");
            m_soundnmi2 = new required_device<input_merger_device>(this, "soundnmi2");
            m_dac = new required_device<dac_8bit_r2r_device>(this, "dac");
            m_dacvol = new required_device<discrete_sound_device>(this, "dacvol");
            m_ay1 = new required_device<ay8910_device>(this, "ay1");
            m_ay2 = new required_device<ay8910_device>(this, "ay2");
            m_ay3 = new required_device<ay8910_device>(this, "ay3");
            m_ay4 = new required_device<ay8910_device>(this, "ay4");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
        }
    }
}
