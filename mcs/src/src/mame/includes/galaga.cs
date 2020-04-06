// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using ioport_value = System.UInt32;
using offs_t = System.UInt32;
using u8 = System.Byte;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    partial class galaga_state : driver_device
    {
        /* memory pointers */
        protected optional_shared_ptr_uint8_t m_videoram;
        optional_shared_ptr_uint8_t m_galaga_ram1;
        optional_shared_ptr_uint8_t m_galaga_ram2;
        optional_shared_ptr_uint8_t m_galaga_ram3;
        optional_device<ls259_device> m_videolatch; // not present on Xevious
        required_device<cpu_device> m_maincpu;
        required_device<cpu_device> m_subcpu;
        required_device<cpu_device> m_subcpu2;
        required_device<namco_device> m_namco_sound;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<screen_device> m_screen;
        required_device<palette_device> m_palette;
        output_manager.output_finder/*<2>*/ m_leds;
        optional_device<starfield_05xx_device> m_starfield; // not present on battles, digdug, xevious
        emu_timer m_cpu3_interrupt_timer;

        uint32_t m_galaga_gfxbank; // used by catsbee

        /* devices */

        /* bank support */

        /* shared */
        protected tilemap_t m_fg_tilemap;
        protected tilemap_t m_bg_tilemap;

        uint8_t m_main_irq_mask;
        uint8_t m_sub_irq_mask;
        uint8_t m_sub2_nmi_mask;


        public galaga_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_videoram = new optional_shared_ptr_uint8_t(this, "videoram");
            m_galaga_ram1 = new optional_shared_ptr_uint8_t(this, "galaga_ram1");
            m_galaga_ram2 = new optional_shared_ptr_uint8_t(this, "galaga_ram2");
            m_galaga_ram3 = new optional_shared_ptr_uint8_t(this, "galaga_ram3");
            m_videolatch = new optional_device<ls259_device>(this, "videolatch");
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_subcpu = new required_device<cpu_device>(this, "sub");
            m_subcpu2 = new required_device<cpu_device>(this, "sub2");
            m_namco_sound = new required_device<namco_device>(this, "namco");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
            m_leds = new output_manager.output_finder(2, this, "led{0}", 0U);
            m_starfield = new optional_device<starfield_05xx_device>(this, "starfield");
            m_galaga_gfxbank = 0;
            m_main_irq_mask = 0;
            m_sub_irq_mask = 0;
            m_sub2_nmi_mask = 0;
        }


        public optional_device<ls259_device> videolatch { get { return m_videolatch; } }
        public required_device<cpu_device> maincpu { get { return m_maincpu; } }
        public required_device<cpu_device> subcpu { get { return m_subcpu; } }
        public required_device<cpu_device> subcpu2 { get { return m_subcpu2; } }
        public required_device<namco_device> namco_sound { get { return m_namco_sound; } }
        public required_device<gfxdecode_device> gfxdecode { get { return m_gfxdecode; } }
        public required_device<screen_device> screen { get { return m_screen; } }
        public required_device<palette_device> palette { get { return m_palette; } }
    }
}
