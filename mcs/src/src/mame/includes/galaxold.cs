// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;


namespace mame
{
    /* star circuit */
    //#define STAR_COUNT  252
    //struct star_gold
    //{
    //    int x = 0, y = 0, color = 0;
    //};


    class galaxold_state : driver_device
    {
        /* devices */
        protected required_device<z80_device> m_maincpu;  //required_device<cpu_device> m_maincpu;
        protected optional_device<z80_device> m_audiocpu;  //optional_device<cpu_device> m_audiocpu;
        optional_device<ttl7474_device> m_7474_9m_1;
        optional_device<ttl7474_device> m_7474_9m_2;
        protected required_device<gfxdecode_device> m_gfxdecode;
        protected required_device<screen_device> m_screen;
        protected required_device<palette_device> m_palette;

        /* memory pointers */
        required_shared_ptr<uint8_t> m_videoram;
        required_shared_ptr<uint8_t> m_spriteram;
        optional_shared_ptr<uint8_t> m_spriteram2;
        required_shared_ptr<uint8_t> m_attributesram;
        optional_shared_ptr<uint8_t> m_bulletsram;
        optional_shared_ptr<uint8_t> m_racknrol_tiles_bank;
        output_finder<u32_const_2> m_leds;

        //int m_irq_line = 0;
        //uint8_t m__4in1_bank = 0U;
        //tilemap_t *m_bg_tilemap = nullptr;
        //int m_spriteram2_present = 0;
        //uint8_t m_gfxbank[5]{};
        //uint8_t m_flipscreen_x = 0U;
        //uint8_t m_flipscreen_y = 0U;
        //uint8_t m_color_mask = 0U;
        //tilemap_t *m_dambustr_tilemap2 = nullptr;
        //std::unique_ptr<uint8_t[]> m_dambustr_videoram2{};
        int m_leftclip;


        //void (galaxold_state::*m_modify_charcode)(uint16_t *code, uint8_t x);     /* function to call to do character banking */
        //void (galaxold_state::*m_modify_spritecode)(uint8_t *spriteram, int*, int*, int*, int); /* function to call to do sprite banking */
        //void (galaxold_state::*m_modify_color)(uint8_t *color);   /* function to call to do modify how the color codes map to the PROM */
        //void (galaxold_state::*m_modify_ypos)(uint8_t*);  /* function to call to do modify how vertical positioning bits are connected */

        //uint8_t m_timer_adjusted = 0U;
        //uint8_t m_darkplnt_bullet_color = 0U;
        //void (galaxold_state::*m_draw_bullets)(bitmap_ind16 &,const rectangle &, int, int, int);  /* function to call to draw a bullet */

        //uint8_t m_background_enable = 0U;
        //uint8_t m_background_red = 0U;
        //uint8_t m_background_green = 0U;
        //uint8_t m_background_blue = 0U;
        //void (galaxold_state::*m_draw_background)(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect);   /* function to call to draw the background */
        //int m_dambustr_bg_split_line = 0;
        //int m_dambustr_bg_color_1 = 0;
        //int m_dambustr_bg_color_2 = 0;
        //int m_dambustr_bg_priority = 0;
        //int m_dambustr_char_bank = 0;
        //std::unique_ptr<bitmap_ind16> m_dambustr_tmpbitmap{};

        //void (galaxold_state::*m_draw_stars)(bitmap_ind16 &, const rectangle &);      /* function to call to draw the star layer */
        //int m_stars_colors_start = 0;
        //int32_t m_stars_scrollpos = 0U;
        //uint8_t m_stars_on = 0U;
        //uint8_t m_stars_blink_state = 0U;
        //emu_timer *m_stars_blink_timer = nullptr;
        //emu_timer *m_stars_scroll_timer = nullptr;
        //struct star_gold m_stars[STAR_COUNT]{};


        protected galaxold_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<z80_device>(this, "maincpu");
            m_audiocpu = new optional_device<z80_device>(this, "audiocpu");
            m_7474_9m_1 = new optional_device<ttl7474_device>(this, "7474_9m_1");
            m_7474_9m_2 = new optional_device<ttl7474_device>(this, "7474_9m_2");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_spriteram = new required_shared_ptr<uint8_t>(this, "spriteram");
            m_spriteram2 = new optional_shared_ptr<uint8_t>(this, "spriteram2");
            m_attributesram = new required_shared_ptr<uint8_t>(this, "attributesram");
            m_bulletsram = new optional_shared_ptr<uint8_t>(this, "bulletsram");
            m_racknrol_tiles_bank = new optional_shared_ptr<uint8_t>(this, "racknrol_tbank");
            m_leds = new output_finder<u32_const_2>(this, "led{0}", 0U);
            m_leftclip = 2;
        }


        protected override void machine_start() { m_leds.resolve(); }
    }


    //#define galaxold_coin_counter_0_w galaxold_coin_counter_w
}
