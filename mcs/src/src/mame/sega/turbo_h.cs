// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using u16 = System.UInt16;
using uint8_t = System.Byte;

using static mame.emucore_global;


namespace mame
{
    partial class turbo_base_state : driver_device
    {
        // sprites are scaled in the analog domain; to give a better rendition of this, we scale in the X direction by this factor
        const u16 TURBO_X_SCALE       = 2;


        // device / memory pointers
        protected required_device<z80_device> m_maincpu;
        protected optional_device_array<i8255_device, u32_const_4> m_i8255;

        required_region_ptr<uint8_t> m_spriteroms;
        required_region_ptr<uint8_t> m_proms;

        required_shared_ptr<uint8_t> m_videoram;
        required_shared_ptr<uint8_t> m_sprite_position;

        protected required_device<samples_device> m_samples;
        optional_device<discrete_device> m_discrete;

        protected required_device<gfxdecode_device> m_gfxdecode;
        protected required_device<screen_device> m_screen;
        output_finder<u32_const_32> m_digits;
        output_finder<u32_const_0> m_lamp;

        // machine state
        //uint8_t       m_i8279_scanlines = 0;

        // sound state
        //uint8_t       m_sound_state[3]{};

        // video state
        //tilemap_t * m_fg_tilemap = nullptr;

        //struct sprite_info
        //{
        //    uint16_t  ve = 0;                 // VE0-15 signals for this row
        //    uint8_t   lst = 0;                // LST0-7 signals for this row
        //    uint32_t  latched[8]{};         // latched pixel data
        //    uint8_t   plb[8]{};             // latched PLB state
        //    uint32_t  offset[8]{};          // current offset for this row
        //    uint32_t  frac[8]{};            // leftover fraction
        //    uint32_t  step[8]{};            // stepping value
        //};

        //sprite_info m_sprite_info;


        protected turbo_base_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<z80_device>(this, "maincpu");
            m_i8255 = new optional_device_array<i8255_device, u32_const_4>(this, "i8255{0}", 0U, (base_, tag_) => { return new device_finder<i8255_device, bool_const_false>(base_, tag_); });
            m_spriteroms = new required_region_ptr<uint8_t>(this, "sprites");
            m_proms = new required_region_ptr<uint8_t>(this, "proms");
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_sprite_position = new required_shared_ptr<uint8_t>(this, "spritepos");
            m_samples = new required_device<samples_device>(this, "samples");
            m_discrete = new optional_device<discrete_device>(this, "discrete");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_digits = new output_finder<u32_const_32>(this, "digit{0}", 0U);
            m_lamp = new output_finder<u32_const_0>(this, "lamp", 0);
        }


        protected override void machine_start() { throw new emu_unimplemented(); }
        protected override void video_start() { throw new emu_unimplemented(); }
    }


    //class buckrog_state : public turbo_base_state

    //class subroc3d_state : public turbo_base_state


    partial class turbo_state : turbo_base_state
    {
        required_region_ptr<uint8_t> m_roadroms;
        memory_share_creator<uint8_t> m_alt_spriteram;
        required_ioport_array<u32_const_2> m_vr;
        required_ioport m_dsw3;
        required_ioport m_dial;
        output_finder<u32_const_0> m_tachometer;
        output_finder<u32_const_0> m_speed;

        //uint8_t       m_osel;
        //uint8_t       m_bsel;
        //uint8_t       m_opa;
        //uint8_t       m_opb;
        //uint8_t       m_opc;
        //uint8_t       m_ipa;
        //uint8_t       m_ipb;
        //uint8_t       m_ipc;
        //uint8_t       m_fbpla;
        //uint8_t       m_fbcol;
        //uint8_t       m_collision;
        //uint8_t       m_last_analog;
        //uint8_t       m_accel;


        public turbo_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_roadroms = new required_region_ptr<uint8_t>(this, "road");
            m_alt_spriteram = new memory_share_creator<uint8_t>(this, "alt_spriteram", 0x80, ENDIANNESS_LITTLE);
            m_vr = new required_ioport_array<u32_const_2>(this, "VR{0}", 1U);
            m_dsw3 = new required_ioport(this, "DSW3");
            m_dial = new required_ioport(this, "DIAL");
            m_tachometer = new output_finder<u32_const_0>(this, "tachometer", 0);
            m_speed = new output_finder<u32_const_0>(this, "speed", 0);
        }


        protected override void machine_start() { throw new emu_unimplemented(); }
    }
}
