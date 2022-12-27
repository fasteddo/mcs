// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;


namespace mame
{
    partial class mappy_state : driver_device
    {
        required_shared_ptr<uint8_t> m_videoram;
        required_shared_ptr<uint8_t> m_spriteram;

        required_device<mc6809e_device> m_maincpu;
        required_device<mc6809e_device> m_subcpu;
        optional_device<cpu_device> m_subcpu2;
        required_device_array<namco56xx_device, u32_const_2> m_namcoio;
        required_device<namco_15xx_device> m_namco_15xx;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<screen_device> m_screen;
        required_device<palette_device> m_palette;
        output_finder<u32_const_2> m_leds;

        //tilemap_t *m_bg_tilemap = nullptr;
        //bitmap_ind16 m_sprite_bitmap;

        //uint8_t m_scroll = 0;

        //uint8_t m_main_irq_mask = 0;
        //uint8_t m_sub_irq_mask = 0;
        //uint8_t m_sub2_irq_mask = 0;

        //emu_timer *m_namcoio_run_timer[2]{};


        public mappy_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_spriteram = new required_shared_ptr<uint8_t>(this, "spriteram");
            m_maincpu = new required_device<mc6809e_device>(this, "maincpu");
            m_subcpu = new required_device<mc6809e_device>(this, "sub");
            m_subcpu2 = new optional_device<cpu_device>(this, "sub2");
            m_namcoio = new required_device_array<namco56xx_device, u32_const_2>(this, "namcoio_{0}", 1, (base_, tag_) => { return new device_finder<namco56xx_device, bool_const_true>(base_, tag_); });
            m_namco_15xx = new required_device<namco_15xx_device>(this, "namco");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
            m_leds = new output_finder<u32_const_2>(this, "led{0}", 0U);
        }


        protected override void machine_start() { throw new emu_unimplemented(); }
    }
}
