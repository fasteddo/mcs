// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;


namespace mame
{
    partial class pacman_state : driver_device
    {
        required_device<z80_device> m_maincpu;  //required_device<cpu_device> m_maincpu;
        optional_device<ls259_device> m_mainlatch;
        optional_device<namco_device> m_namco_sound;
        required_device<watchdog_timer_device> m_watchdog;
        optional_shared_ptr<uint8_t> m_spriteram;
        optional_shared_ptr<uint8_t> m_spriteram2;
        optional_shared_ptr<uint8_t> m_s2650_spriteram;
        required_shared_ptr<uint8_t> m_videoram;
        optional_shared_ptr<uint8_t> m_colorram;
        optional_shared_ptr<uint8_t> m_s2650games_tileram;
        optional_shared_ptr<uint8_t> m_rocktrv2_prot_data;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<palette_device> m_palette;

        //uint8_t m_cannonb_bit_to_read = 0;
        //int m_mystery = 0;
        //uint8_t m_counter = 0;
        //int m_bigbucks_bank = 0;
        //uint8_t m_rocktrv2_question_bank = 0;
        tilemap_t m_bg_tilemap = null;
        uint8_t m_charbank = 0;
        uint8_t m_spritebank = 0;
        uint8_t m_palettebank = 0;
        uint8_t m_colortablebank = 0;
        uint8_t m_flipscreen = 0;
        uint8_t m_bgpriority = 0;
        int m_xoffsethack = 0;
        uint8_t m_inv_spr = 0;
        //uint8_t m_maketrax_counter = 0;
        //uint8_t m_maketrax_offset = 0;
        //int m_maketrax_disable_protection = 0;

        bool m_irq_mask = false;
        uint8_t m_interrupt_vector = 0;


        public pacman_state(machine_config mconfig, device_type type, string tag) 
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<z80_device>(this, "maincpu");
            m_mainlatch = new optional_device<ls259_device>(this, "mainlatch");
            m_namco_sound = new optional_device<namco_device>(this, "namco");
            m_watchdog = new required_device<watchdog_timer_device>(this, "watchdog");
            m_spriteram = new optional_shared_ptr<uint8_t>(this, "spriteram");
            m_spriteram2 = new optional_shared_ptr<uint8_t>(this, "spriteram2");
            m_s2650_spriteram = new optional_shared_ptr<uint8_t>(this, "s2650_spriteram");
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_colorram = new optional_shared_ptr<uint8_t>(this, "colorram");
            m_s2650games_tileram = new optional_shared_ptr<uint8_t>(this, "s2650_tileram");
            m_rocktrv2_prot_data = new optional_shared_ptr<uint8_t>(this, "rocktrv2_prot");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_palette = new required_device<palette_device>(this, "palette");
        }
    }
}
