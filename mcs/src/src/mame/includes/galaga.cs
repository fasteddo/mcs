// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    partial class galaga_state : driver_device
    {
        /* memory pointers, devices */
        protected optional_shared_ptr<uint8_t> m_videoram;
        optional_shared_ptr<uint8_t> m_galaga_ram1;
        optional_shared_ptr<uint8_t> m_galaga_ram2;
        optional_shared_ptr<uint8_t> m_galaga_ram3;
        protected optional_device<ls259_device> m_videolatch; // not present on Xevious
        protected required_device<cpu_device> m_maincpu;
        protected required_device<cpu_device> m_subcpu;
        protected required_device<cpu_device> m_subcpu2;
        protected required_device<namco_device> m_namco_sound;
        protected required_device<gfxdecode_device> m_gfxdecode;
        protected required_device<screen_device> m_screen;
        protected required_device<palette_device> m_palette;
        output_finder<u32_const_2> m_leds;
        optional_device<starfield_05xx_device> m_starfield; // not present on battles, digdug, xevious
        emu_timer m_cpu3_interrupt_timer;

        uint32_t m_galaga_gfxbank; // used by gatsbee

        /* shared */
        protected tilemap_t m_fg_tilemap;
        protected tilemap_t m_bg_tilemap;

        uint8_t m_main_irq_mask;
        uint8_t m_sub_irq_mask;
        uint8_t m_sub2_nmi_mask;


        public galaga_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_videoram = new optional_shared_ptr<uint8_t>(this, "videoram");
            m_galaga_ram1 = new optional_shared_ptr<uint8_t>(this, "galaga_ram1");
            m_galaga_ram2 = new optional_shared_ptr<uint8_t>(this, "galaga_ram2");
            m_galaga_ram3 = new optional_shared_ptr<uint8_t>(this, "galaga_ram3");
            m_videolatch = new optional_device<ls259_device>(this, "videolatch");
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_subcpu = new required_device<cpu_device>(this, "sub");
            m_subcpu2 = new required_device<cpu_device>(this, "sub2");
            m_namco_sound = new required_device<namco_device>(this, "namco");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
            m_leds = new output_finder<u32_const_2>(this, "led{0}", 0U);
            m_starfield = new optional_device<starfield_05xx_device>(this, "starfield");
            m_galaga_gfxbank = 0;
            m_main_irq_mask = 0;
            m_sub_irq_mask = 0;
            m_sub2_nmi_mask = 0;
        }
    }
}
