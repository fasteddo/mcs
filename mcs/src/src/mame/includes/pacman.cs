// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    partial class pacman_state : driver_device
    {
        required_device<cpu_device> m_maincpu;
        optional_device<ls259_device> m_mainlatch;
        optional_device<namco_device> m_namco_sound;
        required_device<watchdog_timer_device> m_watchdog;
        optional_shared_ptr_uint8_t  m_spriteram;
        optional_shared_ptr_uint8_t  m_spriteram2;
        optional_shared_ptr_uint8_t  m_s2650_spriteram;
        required_shared_ptr_uint8_t  m_videoram;
        optional_shared_ptr_uint8_t  m_colorram;
        optional_shared_ptr_uint8_t  m_s2650games_tileram;
        optional_shared_ptr_uint8_t  m_rocktrv2_prot_data;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<palette_device> m_palette;


        public pacman_state(machine_config mconfig, device_type type, string tag) 
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_mainlatch = new optional_device<ls259_device>(this, "mainlatch");
            m_namco_sound = new optional_device<namco_device>(this, "namco");
            m_watchdog = new required_device<watchdog_timer_device>(this, "watchdog");
            m_spriteram = new optional_shared_ptr_uint8_t(this, "spriteram");
            m_spriteram2 = new optional_shared_ptr_uint8_t(this, "spriteram2");
            m_s2650_spriteram = new optional_shared_ptr_uint8_t(this, "s2650_spriteram");
            m_videoram = new required_shared_ptr_uint8_t(this, "videoram");
            m_colorram = new optional_shared_ptr_uint8_t(this, "colorram");
            m_s2650games_tileram = new optional_shared_ptr_uint8_t(this, "s2650_tileram");
            m_rocktrv2_prot_data = new optional_shared_ptr_uint8_t(this, "rocktrv2_prot");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_palette = new required_device<palette_device>(this, "palette");
        }


        //UINT8 m_cannonb_bit_to_read;
        //int m_mystery;
        //UINT8 m_counter;
        //int m_bigbucks_bank;
        //UINT8 m_rocktrv2_question_bank;
        tilemap_t m_bg_tilemap;
        byte m_charbank;
        byte m_spritebank;
        byte m_palettebank;
        byte m_colortablebank;
        byte m_flipscreen;
        byte m_bgpriority;
        int m_xoffsethack;
        byte m_inv_spr;
        //uint8_t m_maketrax_counter;
        //uint8_t m_maketrax_offset;
        //int m_maketrax_disable_protection;

        byte m_irq_mask;
    }
}
