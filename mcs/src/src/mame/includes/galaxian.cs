// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using ListBytesPointer = mame.ListPointer<System.Byte>;
using offs_t = System.UInt32;
using u8 = System.Byte;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    partial class galaxian_state : driver_device
    {
        static galaxian_state()
        {
            // use a static ctor here to make sure GALAXIAN_MASTER_CLOCK is initialized properly across partial classes, see \audio\galaxian.cs
            // https://stackoverflow.com/questions/29086844/static-field-initialization-order-with-partial-classes
            GALAXIAN_MASTER_CLOCK = XTAL_global.op("18.432_MHz_XTAL");
            SOUND_CLOCK           = GALAXIAN_MASTER_CLOCK/6/2;          /* 1.536 MHz */
            RNG_RATE              = GALAXIAN_MASTER_CLOCK/3*2;          /* RNG clock is XTAL/3*2 see Aaron's note in video/galaxian.c */
        }


        /* master clocks */
        static readonly XTAL GALAXIAN_MASTER_CLOCK = XTAL_global.op("18.432_MHz_XTAL");
        static readonly XTAL KONAMI_SOUND_CLOCK = XTAL_global.op("14.318181_MHz_XTAL");
        //static constexpr XTAL SIDAM_MASTER_CLOCK(12_MHz_XTAL);

        /* we scale horizontally by 3 to render stars correctly */
        const int GALAXIAN_XSCALE = 3;
        /* the Sidam bootlegs have a 12 MHz XTAL instead */
        //static constexpr int SIDAM_XSCALE    = 2;

        static readonly XTAL GALAXIAN_PIXEL_CLOCK = (GALAXIAN_XSCALE*GALAXIAN_MASTER_CLOCK / 3);
        //static constexpr XTAL SIDAM_PIXEL_CLOCK(SIDAM_XSCALE*SIDAM_MASTER_CLOCK / 2);

        /* H counts from 128->511, HBLANK starts at 130 and ends at 250 */
        /* we normalize this here so that we count 0->383 with HBLANK */
        /* from 264-383 */
        const int GALAXIAN_HTOTAL  = (384 * GALAXIAN_XSCALE);
        const int GALAXIAN_HBEND   = (0 * GALAXIAN_XSCALE);
        //static constexpr int GALAXIAN_H0START = (6*GALAXIAN_XSCALE);
        //static constexpr int GALAXIAN_HBSTART = (264*GALAXIAN_XSCALE)
        const int GALAXIAN_H0START = (0 * GALAXIAN_XSCALE);
        const int GALAXIAN_HBSTART = (256 * GALAXIAN_XSCALE);

        const int GALAXIAN_VTOTAL  = (264);
        const int GALAXIAN_VBEND   = (16);
        const int GALAXIAN_VBSTART = (224 + 16);

        //static constexpr int SIDAM_HTOTAL     = (384 * SIDAM_XSCALE);
        //static constexpr int SIDAM_HBEND      = (0 * SIDAM_XSCALE);
        //static constexpr int SIDAM_H0START    = (0 * SIDAM_XSCALE);
        //static constexpr int SIDAM_HBSTART    = (256 * SIDAM_XSCALE);


        required_device<cpu_device> m_maincpu;
        optional_device<cpu_device> m_audiocpu;
        optional_device<cpu_device> m_audio2;
        optional_device<dac_byte_interface> m_dac;
        optional_device_array_ay8910_device m_ay8910;  //optional_device_array<ay8910_device/*, 3*/> m_ay8910;
        optional_device<ay8910_device> m_ay8910_cclimber;
        optional_device<digitalker_device> m_digitalker;
        optional_device_array_i8255_device m_ppi8255;  //optional_device_array<i8255_device, 3> m_ppi8255;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<screen_device> m_screen;
        required_device<palette_device> m_palette;
        optional_device<generic_latch_8_device> m_soundlatch;
        optional_device<discrete_device> m_discrete;

        optional_ioport m_fake_select;
        optional_ioport_array m_tenspot_game_dsw;  //optional_ioport_array<10> m_tenspot_game_dsw;

        required_shared_ptr_uint8_t m_spriteram;
        required_shared_ptr_uint8_t m_videoram;
        optional_shared_ptr_uint8_t m_decrypted_opcodes;
        output_manager.output_finder/*<2>*/ m_lamps;

        int m_bullets_base;
        int m_sprites_base;
        int m_numspritegens;
        //int m_counter_74ls161[2];
        //int m_direction[2];
        //uint8_t m_gmgalax_selected_game;
        //uint8_t m_zigzag_ay8910_latch;
        //uint8_t m_kingball_speech_dip;
        //uint8_t m_kingball_sound;
        //uint8_t m_mshuttle_ay8910_cs;
        //uint16_t m_protection_state;
        //uint8_t m_protection_result;
        uint8_t m_konami_sound_control;
        //uint8_t m_sfx_sample_control;
        //uint8_t m_moonwar_port_select;
        uint8_t m_irq_enabled;
        int m_irq_line;
        //int m_tenspot_current_game;
        uint8_t m_frogger_adjust;
        uint8_t m_x_scale;
        uint8_t m_h0_start;
        uint8_t m_sfx_tilemap;

        /* video extension callbacks */
        //typedef void (galaxian_state::*galaxian_extend_tile_info_func)(uint16_t *code, uint8_t *color, uint8_t attrib, uint8_t x);
        delegate void galaxian_extend_tile_info_func(ref uint16_t code, ref uint8_t color, uint8_t attrib, uint8_t x);

        //typedef void (galaxian_state::*galaxian_extend_sprite_info_func)(const uint8_t *base, uint8_t *sx, uint8_t *sy, uint8_t *flipx, uint8_t *flipy, uint16_t *code, uint8_t *color);
        delegate void galaxian_extend_sprite_info_func(ListBytesPointer basePtr, ref uint8_t sx, ref uint8_t sy, ref uint8_t flipx, ref uint8_t flipy, ref uint16_t code, ref uint8_t color);

        //typedef void (galaxian_state::*galaxian_draw_bullet_func)(bitmap_rgb32 &bitmap, const rectangle &cliprect, int offs, int x, int y);
        delegate void galaxian_draw_bullet_func(bitmap_rgb32 bitmap, rectangle cliprect, int offs, int x, int y);

        //typedef void (galaxian_state::*galaxian_draw_background_func)(bitmap_rgb32 &bitmap, const rectangle &cliprect);
        delegate void galaxian_draw_background_func(bitmap_rgb32 bitmap, rectangle cliprect);


        galaxian_extend_tile_info_func m_extend_tile_info_ptr;
        galaxian_extend_sprite_info_func m_extend_sprite_info_ptr;
        galaxian_draw_bullet_func m_draw_bullet_ptr;
        galaxian_draw_background_func m_draw_background_ptr;

        tilemap_t m_bg_tilemap;
        uint8_t m_flipscreen_x;
        uint8_t m_flipscreen_y;
        uint8_t m_background_enable;
        uint8_t m_background_red;
        uint8_t m_background_green;
        uint8_t m_background_blue;
        uint32_t m_star_rng_origin;
        uint32_t m_star_rng_origin_frame;
        rgb_t [] m_star_color = new rgb_t[64];
        uint8_t [] m_stars;  //std::unique_ptr<uint8_t[]> m_stars;
        uint8_t m_stars_enabled;
        uint8_t m_stars_blink_state;
        rgb_t [] m_bullet_color = new rgb_t[8];
        uint8_t [] m_gfxbank = new uint8_t[5];


        public galaxian_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_audiocpu = new optional_device<cpu_device>(this, "audiocpu");
            m_audio2 = new optional_device<cpu_device>(this, "audio2");
            m_dac = new optional_device<dac_byte_interface>(this, "dac");
            m_ay8910 = new optional_device_array_ay8910_device(3, this, "8910.{0}", 0);  // "8910.%u"
            m_ay8910_cclimber = new optional_device<ay8910_device>(this, "cclimber_audio:aysnd");
            m_digitalker = new optional_device<digitalker_device>(this, "digitalker");
            m_ppi8255 = new optional_device_array_i8255_device(3, this, "ppi8255_{0}", 0);  // ppi8255_%u
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
            m_soundlatch = new optional_device<generic_latch_8_device>(this, "soundlatch");
            m_discrete = new optional_device<discrete_device>(this, "konami");
            m_fake_select = new optional_ioport(this, "FAKE_SELECT");
            m_tenspot_game_dsw = new optional_ioport_array(10, this, "IN2_GAME{0}", 0);  //{"IN2_GAME0", "IN2_GAME1", "IN2_GAME2", "IN2_GAME3", "IN2_GAME4", "IN2_GAME5", "IN2_GAME6", "IN2_GAME7", "IN2_GAME8", "IN2_GAME9"});
            m_spriteram = new required_shared_ptr_uint8_t(this, "spriteram");
            m_videoram = new required_shared_ptr_uint8_t(this, "videoram");
            m_decrypted_opcodes = new optional_shared_ptr_uint8_t(this, "decrypted_opcodes");
            m_lamps = new output_manager.output_finder(2, this, "lamp{0}", 0U);  //"lamp%u"
        }


        protected override void machine_start() { m_lamps.resolve(); }
    }
}
