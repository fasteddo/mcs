// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;


namespace mame
{
    partial class snk6502_state : driver_device
    {
        protected required_device<m6502_device> m_maincpu;
        protected required_device<gfxdecode_device> m_gfxdecode;
        protected required_device<palette_device> m_palette;

        required_shared_ptr<uint8_t> m_videoram;
        required_shared_ptr<uint8_t> m_videoram2;
        required_shared_ptr<uint8_t> m_colorram;
        required_shared_ptr<uint8_t> m_charram;

        //uint8_t m_sasuke_counter = 0;
        //int m_charbank = 0;
        //int m_backcolor = 0;
        //tilemap_t *m_bg_tilemap = nullptr;
        //tilemap_t *m_fg_tilemap = nullptr;
        //rgb_t m_palette_val[64]{};
        //uint8_t m_irq_mask = 0;


        protected snk6502_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_maincpu = new required_device<m6502_device>(this, "maincpu");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_palette = new required_device<palette_device>(this, "palette");
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_videoram2 = new required_shared_ptr<uint8_t>(this, "videoram2");
            m_colorram = new required_shared_ptr<uint8_t>(this, "colorram");
            m_charram = new required_shared_ptr<uint8_t>(this, "charram");
        }

        protected override void machine_start() { throw new emu_unimplemented(); }
    }


    partial class vanguard_state : snk6502_state
    {
        required_device<address_map_bank_device> m_highmem;


        public vanguard_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_highmem = new required_device<address_map_bank_device>(this, "highmem");
        }
    }


    //class fantasy_state : public vanguard_state
}
