// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;

using static mame._74259_global;
using static mame.adc0804_global;
using static mame.diexec_global;
using static mame.digfx_global;
using static mame.discrete_global;
using static mame.disound_global;
using static mame.drawgfx_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.namco_global;
using static mame.namco06_global;
using static mame.namco51_global;
using static mame.namco52_global;
using static mame.namco53_global;
using static mame.namco54_global;
using static mame.nvram_global;
using static mame.polepos_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.timer_global;
using static mame.watchdog_global;
using static mame.z80_global;
using static mame.z8000_global;


namespace mame
{
    partial class polepos_state : driver_device
    {
        static readonly XTAL MASTER_CLOCK    = new XTAL(24_576_000);
    }


    partial class polepos : construct_ioport_helper
    {
        void POLEPOS_TOGGLE() { PORT_TOGGLE(); }  //#define POLEPOS_TOGGLE  PORT_TOGGLE
    }


    partial class polepos_state : driver_device
    {
        /*************************************************************************************/
        /* Pole Position II protection                                                       */
        /*************************************************************************************/

        //uint16_t polepos_state::polepos2_ic25_r(offs_t offset)


        uint8_t analog_r()
        {
            throw new emu_unimplemented();
#if false
            return ioport(m_adc_input ? "ACCEL" : "BRAKE")->read();
#endif
        }


        uint8_t ready_r()
        {
            throw new emu_unimplemented();
#if false
            int ret = 0xff;

            if (m_screen->vpos() >= 128)
                ret ^= 0x02;

            if (!m_adc->intr_r())
                ret ^= 0x08; /* ADC End Flag */

            return ret;
#endif
        }


        //WRITE_LINE_MEMBER(polepos_state::gasel_w)
        public void gasel_w(int state)
        {
            m_adc_input = state;
        }


        //WRITE_LINE_MEMBER(polepos_state::sb0_w)
        public void sb0_w(int state)
        {
            m_auto_start_mask = state == 0 ? 1 : 0;
        }


        //template<bool sub1>
        void z8002_nvi_enable_w<bool_sub1>(uint16_t data)
            where bool_sub1 : bool_const, new()
        {
            throw new emu_unimplemented();
#if false
            data &= 1;

            m_sub_irq_mask = data;
            if (!data)
                (sub1 ? m_subcpu : m_subcpu2)->set_input_line(z8002_device::NVI_LINE, CLEAR_LINE);
#endif
        }


        //READ_LINE_MEMBER(polepos_state::auto_start_r)
        public int auto_start_r()
        {
            return m_auto_start_mask;
        }


        void out_(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            // no start lamps in pole position
            //  output().set_led_value(1,data & 1);
            //  output().set_led_value(0,data & 2);
            machine().bookkeeping().coin_counter_w(1,~data & 4);
            machine().bookkeeping().coin_counter_w(0,~data & 8);
#endif
        }


        //WRITE_LINE_MEMBER(polepos_state::lockout)
        public void lockout(int state)
        {
            throw new emu_unimplemented();
#if false
            machine().bookkeeping().coin_lockout_global_w(state);
#endif
        }


        uint8_t namco_52xx_rom_r(offs_t offset)
        {
            throw new emu_unimplemented();
#if false
            uint32_t length = memregion("52xx")->bytes();
            logerror("ROM @ %04X\n", offset);
            return (offset < length) ? memregion("52xx")->base()[offset] : 0xff;
#endif
        }


        uint8_t namco_52xx_si_r()
        {
            throw new emu_unimplemented();
#if false
            /* pulled to +5V */
            return 1;
#endif
        }


        uint8_t namco_53xx_k_r()
        {
            throw new emu_unimplemented();
#if false
            /* hardwired to 0 */
            return 0;
#endif
        }


        uint8_t steering_changed_r()
        {
            throw new emu_unimplemented();
#if false
            /* read the current steering value and update our delta */
            uint8_t steer_new = ioport("STEER")->read();
            m_steer_accum += (int8_t)(steer_new - m_steer_last) * 2;
            m_steer_last = steer_new;

            /* if we have delta, clock things */
            if (m_steer_accum < 0)
            {
                m_steer_delta = 0;
                m_steer_accum++;
            }
            else if (m_steer_accum > 0)
            {
                m_steer_delta = 1;
                m_steer_accum--;
            }

            return m_steer_accum & 1;
#endif
        }


        uint8_t steering_delta_r()
        {
            throw new emu_unimplemented();
#if false
            return m_steer_delta;
#endif
        }


        //TIMER_DEVICE_CALLBACK_MEMBER(polepos_state::scanline)
        void scanline(timer_device timer, object ptr, s32 param)  //void *ptr, s32 param
        {
            int scanline = param;

            if (((scanline == 64) || (scanline == 192)) && (m_latch.op0.q0_r() != 0)) // 64V
                m_maincpu.op0.set_input_line(0, ASSERT_LINE);

            if (scanline == 240 && m_sub_irq_mask != 0)  // VBLANK
            {
                m_subcpu.op0.set_input_line(z8002_device.NVI_LINE, ASSERT_LINE);
                m_subcpu2.op0.set_input_line(z8002_device.NVI_LINE, ASSERT_LINE);
            }
        }


        protected override void machine_start()
        {
            save_item(NAME(new { m_steer_last }));
            save_item(NAME(new { m_steer_delta }));
            save_item(NAME(new { m_steer_accum }));
            save_item(NAME(new { m_last_result }));
            save_item(NAME(new { m_last_signed }));
            save_item(NAME(new { m_last_unsigned }));
            save_item(NAME(new { m_adc_input }));
            save_item(NAME(new { m_auto_start_mask }));
            save_item(NAME(new { m_sub_irq_mask }));
        }


        protected override void machine_reset()
        {
        }


        /*********************************************************************
         * CPU memory structures
         *********************************************************************/
        void z80_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x2fff).rom().region("maincpu", 0);
            map.op(0x3000, 0x37ff).mirror(0x0800).ram().share("nvram");                 /* Battery Backup */
            map.op(0x4000, 0x47ff).rw(sprite_r, sprite_w);           /* Motion Object */
            map.op(0x4800, 0x4bff).rw(road_r, road_w);               /* Road Memory */
            map.op(0x4c00, 0x4fff).rw(alpha_r, alpha_w);             /* Alphanumeric (char ram) */
            map.op(0x5000, 0x57ff).rw(view_r, view_w);               /* Background Memory */

            map.op(0x8000, 0x83bf).mirror(0x0c00).ram();                                   /* Sound Memory */
            map.op(0x83c0, 0x83ff).mirror(0x0c00).rw(m_namco_sound, (offs_t offset) => { return m_namco_sound.op0.polepos_sound_r(offset); }, (offs_t offset, uint8_t data) => { m_namco_sound.op0.polepos_sound_w(offset, data); });    /* Sound data */

            map.op(0x9000, 0x9000).mirror(0x0eff).rw("06xx", (offs_t offset) => { return ((namco_06xx_device)subdevice("06xx")).data_r(offset); }, (offs_t offset, uint8_t data) => { ((namco_06xx_device)subdevice("06xx")).data_w(offset, data); });
            map.op(0x9100, 0x9100).mirror(0x0eff).rw("06xx", () => { return ((namco_06xx_device)subdevice("06xx")).ctrl_r(); }, (uint8_t data) => { ((namco_06xx_device)subdevice("06xx")).ctrl_w(data); });
            map.op(0xa000, 0xa000).mirror(0x0cff).r(ready_r);                 /* READY */
            map.op(0xa000, 0xa007).mirror(0x0cf8).w(m_latch, (offs_t offset, uint8_t data) => { m_latch.op0.write_d0(offset, data); });
            map.op(0xa100, 0xa100).mirror(0x0cff).w("watchdog", (uint8_t data) => { ((watchdog_timer_device)subdevice("watchdog")).reset_w(data); });
            map.op(0xa200, 0xa200).mirror(0x0cff).w("polepos", (uint8_t data) => { ((polepos_sound_device)subdevice("polepos")).polepos_engine_sound_lsb_w(data); });    /* Car Sound ( Lower Nibble ) */
            map.op(0xa300, 0xa300).mirror(0x0cff).w("polepos", (uint8_t data) => { ((polepos_sound_device)subdevice("polepos")).polepos_engine_sound_msb_w(data); });    /* Car Sound ( Upper Nibble ) */
        }


        void z80_io(address_map map, device_t device)
        {
            map.global_mask(0xff);
            map.op(0x00, 0x00).rw(m_adc, () => { return m_adc.op0.read(); }, (uint8_t data) => { m_adc.op0.write(data); });
        }


        /* the same memory map is used by both Z8002 CPUs; all RAM areas are shared */
        void z8002_map(address_map map)
        {
            map.op(0x8000, 0x8fff).ram().share(m_sprite16_memory);   /* Motion Object */
            map.op(0x9000, 0x97ff).ram().share(m_road16_memory);     /* Road Memory */
            map.op(0x9800, 0x9fff).ram().w((write16s_delegate)alpha16_w).share(m_alpha16_memory);  /* Alphanumeric (char ram) */
            map.op(0xa000, 0xafff).ram().w((write16s_delegate)view16_w).share(m_view16_memory);     /* Background memory */
            map.op(0xc000, 0xc001).mirror(0x38fe).w((write16s_delegate)view16_hscroll_w);                       /* Background horz scroll position */
            map.op(0xc100, 0xc101).mirror(0x38fe).w((write16s_delegate)road16_vscroll_w);                       /* Road vertical position */
        }


        void z8002_map_1(address_map map, device_t device)
        {
            z8002_map(map);
            map.op(0x0000, 0x7fff).rom().region("sub", 0);
            map.op(0x6000, 0x6001).mirror(0x1ffe).w((write16smo_delegate)z8002_nvi_enable_w<bool_const_true>); /* NVI enable - *NOT* shared by the two CPUs */
        }


        void z8002_map_2(address_map map, device_t device)
        {
            z8002_map(map);
            map.op(0x0000, 0x7fff).rom().region("sub2", 0);
            map.op(0x6000, 0x6001).mirror(0x1ffe).w((write16smo_delegate)z8002_nvi_enable_w<bool_const_false>); /* NVI enable - *NOT* shared by the two CPUs */
        }
    }


    partial class polepos : construct_ioport_helper
    {
        /*********************************************************************
         * Input port definitions
         *********************************************************************/

        //static INPUT_PORTS_START( polepos )
        void construct_ioport_polepos(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            polepos_state polepos_state = (polepos_state)owner;

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON3 ); PORT_NAME("Gear Change"); POLEPOS_TOGGLE(); /* Gear */
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_CUSTOM ); PORT_READ_LINE_MEMBER(polepos_state.auto_start_r);  // start 1, program controlled
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_SERVICE1 );
            PORT_SERVICE( 0x80, IP_ACTIVE_LOW );

            PORT_START("DSWA");
            PORT_DIPNAME( 0x07, 0x07, DEF_STR( Coin_A ) );       PORT_DIPLOCATION("SW1:1,2,3");
            PORT_DIPSETTING(    0x05, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x03, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x07, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x04, DEF_STR( _2C_3C ) );
            PORT_DIPSETTING(    0x06, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _1C_3C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_5C ) );
            PORT_DIPSETTING(    0x01, DEF_STR( _1C_6C ) );
            PORT_DIPNAME( 0x18, 0x18, DEF_STR( Coin_B ) );       PORT_DIPLOCATION("SW1:4,5");
            PORT_DIPSETTING(    0x10, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x18, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _2C_3C ) );
            PORT_DIPSETTING(    0x08, DEF_STR( _1C_2C ) );
            PORT_DIPNAME( 0x60, 0x60, DEF_STR( Game_Time ) );    PORT_DIPLOCATION("SW1:6,7");
            PORT_DIPSETTING(    0x60, "90 secs." );
            PORT_DIPSETTING(    0x20, "100 secs." );
            PORT_DIPSETTING(    0x40, "110 secs." );
            PORT_DIPSETTING(    0x00, "120 secs." );
            PORT_DIPNAME( 0x80, 0x80, "Racing Laps" );       PORT_DIPLOCATION("SW1:8");
            PORT_DIPSETTING(    0x80, "3" ); /* Manufacturer's recommended settings for Upright cabinet */
            PORT_DIPSETTING(    0x00, "4" ); /* Manufacturer's recommended settings for Sit-Down cabinet */

            PORT_START("DSWB");
            PORT_DIPNAME( 0x07, 0x03, "Extended Rank" );     PORT_DIPLOCATION("SW2:1,2,3");
            PORT_DIPSETTING(    0x07, "A" );
            PORT_DIPSETTING(    0x03, "B" );
            PORT_DIPSETTING(    0x05, "C" );
            PORT_DIPSETTING(    0x01, "D" );
            PORT_DIPSETTING(    0x06, "E" );
            PORT_DIPSETTING(    0x02, "F" );
            PORT_DIPSETTING(    0x04, "G" );
            PORT_DIPSETTING(    0x00, "H" );
            PORT_DIPNAME( 0x38, 0x28, "Practice Rank" );     PORT_DIPLOCATION("SW2:4,5,6");
            PORT_DIPSETTING(    0x38, "A" );
            PORT_DIPSETTING(    0x18, "B" );
            PORT_DIPSETTING(    0x28, "C" );
            PORT_DIPSETTING(    0x08, "D" );
            PORT_DIPSETTING(    0x30, "E" );
            PORT_DIPSETTING(    0x10, "F" );
            PORT_DIPSETTING(    0x20, "G" );
            PORT_DIPSETTING(    0x00, "H" );
            PORT_DIPNAME( 0x40, 0x40, DEF_STR( Unknown ) );      PORT_DIPLOCATION("SW2:7"); /* Is MPH or Km/H for "English" regions, but only Km/H for Japan ;-) */
            PORT_DIPSETTING(    0x40, DEF_STR( Off) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x80, 0x00, DEF_STR( Demo_Sounds ) );  PORT_DIPLOCATION("SW2:8");
            PORT_DIPSETTING(    0x80, DEF_STR( Off ));
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );

            PORT_START("BRAKE");
            PORT_BIT( 0xff, 0x00, IPT_PEDAL2 ); PORT_MINMAX(0,0x90); PORT_SENSITIVITY(100); PORT_KEYDELTA(16);

            PORT_START("ACCEL");
            PORT_BIT( 0xff, 0x00, IPT_PEDAL ); PORT_MINMAX(0,0x90); PORT_SENSITIVITY(100); PORT_KEYDELTA(16);

            PORT_START("STEER");
            PORT_BIT( 0xff, 0x00, IPT_DIAL ); PORT_SENSITIVITY(30); PORT_KEYDELTA(4);

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( poleposa )
        void construct_ioport_poleposa(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_INCLUDE( construct_ioport_polepos, ref errorbuf );

            PORT_MODIFY("DSWA");
            PORT_DIPNAME( 0xe0, 0xe0, DEF_STR( Coin_A ) );       PORT_DIPLOCATION("SW1:1,2,3");
            PORT_DIPSETTING(    0xc0, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x20, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x40, DEF_STR( _3C_2C ) );
            PORT_DIPSETTING(    0x80, DEF_STR( _4C_3C ) );
            PORT_DIPSETTING(    0xe0, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x60, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0xa0, DEF_STR( _1C_3C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Free_Play ) );
            PORT_DIPNAME( 0x18, 0x18, DEF_STR( Coin_B ) );       PORT_DIPLOCATION("SW1:4,5");
            PORT_DIPSETTING(    0x08, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x10, DEF_STR( _3C_2C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _4C_3C ) );
            PORT_DIPSETTING(    0x18, DEF_STR( _1C_1C ) );
            PORT_DIPNAME( 0x06, 0x06, DEF_STR( Game_Time ) );    PORT_DIPLOCATION("SW1:6,7");
            PORT_DIPSETTING(    0x06, "90 secs." );
            PORT_DIPSETTING(    0x02, "100 secs." );
            PORT_DIPSETTING(    0x04, "110 secs." );
            PORT_DIPSETTING(    0x00, "120 secs." );
            PORT_DIPNAME( 0x01, 0x01, "Racing Laps" );       PORT_DIPLOCATION("SW1:8");
            PORT_DIPSETTING(    0x01, "3" ); /* Manufacturer's recommended settings for Upright cabinet */
            PORT_DIPSETTING(    0x00, "4" ); /* Manufacturer's recommended settings for Sit-Down cabinet */

            PORT_MODIFY("DSWB");
            PORT_DIPNAME( 0xe0, 0x60, "Practice Rank" );     PORT_DIPLOCATION("SW2:1,2,3");
            PORT_DIPSETTING(    0xe0, "A" );
            PORT_DIPSETTING(    0x60, "B" );
            PORT_DIPSETTING(    0xa0, "C" );
            PORT_DIPSETTING(    0x20, "D" );
            PORT_DIPSETTING(    0xc0, "E" );
            PORT_DIPSETTING(    0x40, "F" );
            PORT_DIPSETTING(    0x80, "G" );
            PORT_DIPSETTING(    0x00, "H" );
            PORT_DIPNAME( 0x1c, 0x14, "Extended Rank" );     PORT_DIPLOCATION("SW2:4,5,6");
            PORT_DIPSETTING(    0x1c, "A" );
            PORT_DIPSETTING(    0x0c, "B" );
            PORT_DIPSETTING(    0x14, "C" );
            PORT_DIPSETTING(    0x04, "D" );
            PORT_DIPSETTING(    0x18, "E" );
            PORT_DIPSETTING(    0x08, "F" );
            PORT_DIPSETTING(    0x10, "G" );
            PORT_DIPSETTING(    0x00, "H" );
            PORT_DIPNAME( 0x02, 0x00, "Speed Unit" );        PORT_DIPLOCATION("SW2:7"); /* MPH as per Atari manuals for the US regions */
            PORT_DIPSETTING(    0x00, "mph" );
            PORT_DIPSETTING(    0x02, "km/h" );
            PORT_DIPNAME( 0x01, 0x00, DEF_STR( Demo_Sounds ) );  PORT_DIPLOCATION("SW2:8");
            PORT_DIPSETTING(    0x01, DEF_STR( Off ));
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( topracern )

        //static INPUT_PORTS_START( polepos2 )

        //static INPUT_PORTS_START( polepos2j )

        //static INPUT_PORTS_START( polepos2bi )
    }


    partial class polepos_state : driver_device
    {
        /*********************************************************************
         * Graphics layouts
         *********************************************************************/

        static readonly gfx_layout charlayout_2bpp = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,1),
            2,
            new u32 [] { 0, 4 },
            new u32 [] { 0, 1, 2, 3, 8*8+0, 8*8+1, 8*8+2, 8*8+3 },
            new u32 [] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
            8*8*2
        );


        static readonly gfx_layout bigspritelayout = new gfx_layout
        (
            32,32,
            RGN_FRAC(1,2),
            4,
            new u32 [] { 0, 4, RGN_FRAC(1,2)+0, RGN_FRAC(1,2)+4 },
            new u32 [] {  0,  1,  2,  3,  8,  9, 10, 11,
                         16, 17, 18, 19, 24, 25, 26, 27,
                         32, 33, 34, 35, 40, 41, 42, 43,
                         48, 49, 50, 51, 56, 57, 58, 59},
            new u32 [] {  0*64,  1*64,  2*64,  3*64,  4*64,  5*64,  6*64,  7*64,
                          8*64,  9*64, 10*64, 11*64, 12*64, 13*64, 14*64, 15*64,
                         16*64, 17*64, 18*64, 19*64, 20*64, 21*64, 22*64, 23*64,
                         24*64, 25*64, 26*64, 27*64, 28*64, 29*64, 30*64, 31*64 },
            32*64
        );


        static readonly gfx_layout smallspritelayout = new gfx_layout
        (
            16,16,
            RGN_FRAC(1,2),
            4,
            new u32 [] { 0, 4, RGN_FRAC(1,2), RGN_FRAC(1,2)+4 },
            new u32 [] {  0,  1,  2,  3,  8,  9, 10, 11,
                         16, 17, 18, 19, 24, 25, 26, 27 },
            new u32 [] { 0*32,  1*32,  2*32,  3*32,  4*32,  5*32,  6*32,  7*32,
                         8*32,    9*32, 10*32, 11*32, 12*32, 13*32, 14*32, 15*32 },
            16*32
        );


        //static GFXDECODE_START( gfx_polepos )
        static readonly gfx_decode_entry [] gfx_polepos =
        {
            GFXDECODE_ENTRY( "gfx1", 0, charlayout_2bpp,   0x0000, 128 ),
            GFXDECODE_ENTRY( "gfx2", 0, charlayout_2bpp,   0x0200,  64 ),
            GFXDECODE_ENTRY( "gfx3", 0, smallspritelayout, 0x0300, 128 ),
            GFXDECODE_ENTRY( "gfx4", 0, bigspritelayout,   0x0300, 128 ),

            //GFXDECODE_END
        };


        /*********************************************************************
         * Machine driver
         *********************************************************************/
        public void polepos(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, MASTER_CLOCK / 8);   /* 3.072 MHz */
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, z80_map);
            m_maincpu.op0.memory().set_addrmap(AS_IO, z80_io);

            Z8002(config, m_subcpu, MASTER_CLOCK / 8);  /* 3.072 MHz */
            m_subcpu.op0.memory().set_addrmap(AS_PROGRAM, z8002_map_1);

            Z8002(config, m_subcpu2, MASTER_CLOCK / 8); /* 3.072 MHz */
            m_subcpu2.op0.memory().set_addrmap(AS_PROGRAM, z8002_map_2);

            namco_51xx_device n51xx = NAMCO_51XX(config, "51xx", MASTER_CLOCK / 8 / 2);      /* 1.536 MHz */
            n51xx.input_callback<u32_const_0>().set_ioport("DSWB").mask_u32(0x0f).reg();
            n51xx.input_callback<u32_const_1>().set_ioport("DSWB").rshift(4).reg();
            n51xx.input_callback<u32_const_2>().set_ioport("IN0").mask_u32(0x0f).reg();
            n51xx.input_callback<u32_const_3>().set_ioport("IN0").rshift(4).reg();
            n51xx.output_callback().set(out_).reg();
            n51xx.lockout_callback().set((write_line_delegate)lockout).reg();

            namco_52xx_device n52xx = NAMCO_52XX(config, "52xx", MASTER_CLOCK / 8 / 2);      /* 1.536 MHz */
            n52xx.set_discrete("discrete");
            n52xx.set_basenote(NODE_04);
            n52xx.romread_callback().set(namco_52xx_rom_r).reg();
            n52xx.si_callback().set(namco_52xx_si_r).reg();

            namco_53xx_device n53xx = NAMCO_53XX(config, "53xx", MASTER_CLOCK / 8 / 2);      /* 1.536 MHz */
            n53xx.k_port_callback().set(namco_53xx_k_r).reg();
            n53xx.input_callback<u32_const_0>().set(steering_changed_r).reg();
            n53xx.input_callback<u32_const_1>().set(steering_delta_r).reg();
            n53xx.input_callback<u32_const_2>().set_ioport("DSWA").mask_u32(0x0f).reg();
            n53xx.input_callback<u32_const_3>().set_ioport("DSWA").rshift(4).reg();

            namco_54xx_device n54xx = NAMCO_54XX(config, "54xx", MASTER_CLOCK / 8 / 2);      /* 1.536 MHz */
            n54xx.set_discrete("discrete");
            n54xx.set_basenote(NODE_01);

            namco_06xx_device n06xx = NAMCO_06XX(config, "06xx", MASTER_CLOCK / 8 / 64);
            n06xx.set_maincpu(m_maincpu);
            n06xx.chip_select_callback<u32_const_0>().set("51xx", (int state) => { ((namco_51xx_device)subdevice("51xx")).chip_select(state); }).reg();
            n06xx.rw_callback<u32_const_0>().set("51xx", (int state) => { ((namco_51xx_device)subdevice("51xx")).rw(state); }).reg();
            n06xx.read_callback<u32_const_0>().set("51xx", () => { return ((namco_51xx_device)subdevice("51xx")).read(); }).reg();
            n06xx.write_callback<u32_const_0>().set("51xx", (data) => { ((namco_51xx_device)subdevice("51xx")).write(data); }).reg();
            n06xx.read_callback<u32_const_1>().set("53xx", () => { return ((namco_53xx_device)subdevice("53xx")).read(); }).reg();
            n06xx.chip_select_callback<u32_const_1>().set("53xx", (int state) => { ((namco_53xx_device)subdevice("53xx")).chip_select(state); }).reg();
            n06xx.write_callback<u32_const_2>().set("52xx", (data) => { ((namco_52xx_device)subdevice("52xx")).write(data); }).reg();
            n06xx.chip_select_callback<u32_const_2>().set("52xx", (int state) => { ((namco_52xx_device)subdevice("52xx")).chip_select(state); }).reg();
            n06xx.write_callback<u32_const_3>().set("54xx", (data) => { ((namco_54xx_device)subdevice("54xx")).write(data); }).reg();
            n06xx.chip_select_callback<u32_const_3>().set("54xx", (int state) => { ((namco_54xx_device)subdevice("54xx")).chip_select(state); }).reg();

            WATCHDOG_TIMER(config, "watchdog").set_vblank_count(m_screen, 16);   // 128V clocks the same as VBLANK

            NVRAM(config, "nvram", nvram_device.default_value.DEFAULT_ALL_1);

            TIMER(config, "scantimer").configure_scanline(scanline, "screen", 0, 1);

            LS259(config, m_latch); // at 8E on polepos
            m_latch.op0.q_out_cb<u32_const_0>().set_inputline(m_maincpu, 0, CLEAR_LINE).invert().reg();
            m_latch.op0.q_out_cb<u32_const_1>().set("51xx", (int state) => { ((namco_51xx_device)subdevice("51xx")).reset(state); }).reg();
            m_latch.op0.q_out_cb<u32_const_1>().append("52xx", (int state) => { ((namco_52xx_device)subdevice("52xx")).reset(state); }).reg();
            m_latch.op0.q_out_cb<u32_const_1>().append("53xx", (int state) => { ((namco_53xx_device)subdevice("53xx")).reset(state); }).reg();
            m_latch.op0.q_out_cb<u32_const_1>().append("54xx", (int state) => { ((namco_54xx_device)subdevice("54xx")).reset(state); }).reg();
            m_latch.op0.q_out_cb<u32_const_2>().set(m_namco_sound, (int state) => { m_namco_sound.op0.sound_enable_w(state); }).reg();
            m_latch.op0.q_out_cb<u32_const_2>().append("polepos", (int state) => { ((polepos_sound_device)subdevice("polepos")).clson_w(state); }).reg();
            m_latch.op0.q_out_cb<u32_const_3>().set((write_line_delegate)gasel_w).reg();
            m_latch.op0.q_out_cb<u32_const_4>().set_inputline(m_subcpu, INPUT_LINE_RESET).invert().reg();
            m_latch.op0.q_out_cb<u32_const_5>().set_inputline(m_subcpu2, INPUT_LINE_RESET).invert().reg();
            m_latch.op0.q_out_cb<u32_const_6>().set((write_line_delegate)sb0_w).reg();
            m_latch.op0.q_out_cb<u32_const_7>().set((write_line_delegate)chacl_w).reg();

            ADC0804(config, m_adc, MASTER_CLOCK / 8 / 8);
            m_adc.op0.vin_callback().set(analog_r).reg();

            /* video hardware */
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_raw(MASTER_CLOCK / 4, 384, 0, 256, 264, 16, 224+16);
            m_screen.op0.set_screen_update(screen_update);
            m_screen.op0.set_palette(m_palette);
            m_screen.op0.screen_vblank().set("51xx", (int state) => { ((namco_51xx_device)subdevice("51xx")).vblank(state); }).reg();

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_polepos);
            PALETTE(config, m_palette, polepos_palette, 0x0f00, 128);

            //throw new emu_unimplemented();
#if false
            config.set_default_layout(layout_polepos);
#endif

            /* sound hardware */
            SPEAKER(config, "lspeaker").front_left();
            SPEAKER(config, "rspeaker").front_right();

            NAMCO(config, m_namco_sound, MASTER_CLOCK / 512);
            m_namco_sound.op0.set_voices(8);
            m_namco_sound.op0.set_stereo(true);
            m_namco_sound.op0.disound.add_route(0, "lspeaker", 0.80);
            m_namco_sound.op0.disound.add_route(1, "rspeaker", 0.80);

            /* discrete circuit on the 54XX outputs */
            discrete_sound_device discrete = DISCRETE(config, "discrete", polepos_discrete);
            discrete.disound.add_route(ALL_OUTPUTS, "lspeaker", 0.90);
            discrete.disound.add_route(ALL_OUTPUTS, "rspeaker", 0.90);

            /* engine sound */
            polepos_sound_device polepos = POLEPOS_SOUND(config, "polepos", MASTER_CLOCK / 8);
            polepos.disound.add_route(ALL_OUTPUTS, "lspeaker", 0.90 * 0.77);
            polepos.disound.add_route(ALL_OUTPUTS, "rspeaker", 0.90 * 0.77);
        }


        //void polepos_state::bootleg_soundlatch_w(uint8_t data)
        //{
        //    if (m_soundlatch.found()) // topracern also uses this; no idea what it should do there
        //        m_soundlatch->write(data | 0xfc);
        //}

        //void polepos_state::topracern_io(address_map &map)

        //void polepos_state::sound_z80_bootleg_map(address_map &map)
        //{
        //    map(0x0000, 0x1fff).rom().region("soundz80bl", 0);
        //    map(0x2700, 0x27ff).ram();
        //    map(0x4000, 0x4000).r(m_soundlatch, FUNC(generic_latch_8_device::read));
        //    map(0x6000, 0x6000).r(m_soundlatch, FUNC(generic_latch_8_device::acknowledge_r));
        //}

        //void polepos_state::sound_z80_bootleg_iomap(address_map &map)
        //{
        //    map.global_mask(0xff);
        //    map(0x00, 0x00).rw("tms", FUNC(tms5220_device::status_r), FUNC(tms5220_device::data_w));
        //}


        //void polepos_state::topracern(machine_config &config)

        //void polepos_state::polepos2bi(machine_config &config)
    }


    partial class polepos : construct_ioport_helper
    {
        /*********************************************************************
         * ROM definitions
         *********************************************************************/

        /*
            Pole Position - Namco Version
        */

        //ROM_START( polepos )
        static readonly tiny_rom_entry [] rom_polepos = 
        {
            /* Z80 memory/ROM data */
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "pp3_9.6h",    0x0000, 0x2000, CRC("c0511173") + SHA1("88a1d4eefacbcf7d0e59edc0110edf225cad15c4") ),
            ROM_LOAD( "pp1_10b.5h",  0x2000, 0x1000, CRC("7174bcb7") + SHA1("460326a6cea201db2df813013c95562a222ea95d") ),

            /* Z8002 #1 memory/ROM data */
            ROM_REGION( 0x10000, "sub", 0 ),
            ROM_LOAD16_BYTE( "pp3_1.8m",    0x0001, 0x2000, CRC("65c1c2c2") + SHA1("69f3e2e871f1cdc1efee91688acad4417683474d") ),
            ROM_LOAD16_BYTE( "pp3_2.8l",    0x0000, 0x2000, CRC("fafb9049") + SHA1("92424c1042f520af115fb271fc11f4914a346ae2") ),

            /* Z8002 #2 memory/ROM data */
            ROM_REGION( 0x10000, "sub2", 0 ),
            ROM_LOAD16_BYTE( "pp3_5.4m",    0x0001, 0x2000, CRC("46e5c99a") + SHA1("d5fd657a9197f1751f6fca430d3ef18d37ed774e") ),
            ROM_LOAD16_BYTE( "pp3_6.4l",    0x0000, 0x2000, CRC("acc1ebc3") + SHA1("41745f5b6b0af2cb1ee80843194c070eac9e74e7") ),

            /* graphics data */
            ROM_REGION( 0x01000, "gfx1", 0 ),    /* 2bpp alpha layer */
            ROM_LOAD( "pp3_28.1f",    0x0000, 0x1000, CRC("2e77187e") + SHA1("869a7389a684ccedd14868fb03400b1f8088acca") ),

            ROM_REGION( 0x01000, "gfx2", 0 ),    /* 2bpp view layer */
            ROM_LOAD( "pp1_29.1e",    0x0000, 0x1000, CRC("706e888a") + SHA1("af1aa2199fcf73a3afbe760857ff117865350954") ),

            ROM_REGION( 0x04000, "gfx3", 0 ),    /* 4bpp 16x16 sprites */
            ROM_LOAD( "pp3_25.1n",    0x0000, 0x2000, CRC("b52c086b") + SHA1("ea4a58fcc1d829ad0efa13a02f90fadc61e6e0bc") ),    /* 4bpp sm sprites, planes 0+1 */
            ROM_LOAD( "pp3_26.1m",    0x2000, 0x2000, CRC("d24a5707") + SHA1("468319469bde6b7dc0cf8244299d8dc927059b2d") ),    /* 4bpp sm sprites, planes 2+3 */

            ROM_REGION( 0x10000, "gfx4", 0 ),    /* 4bpp 32x32 sprites */
            ROM_LOAD( "pp1_17.5n",    0x0000, 0x2000, CRC("2e134b46") + SHA1("0938f5f9f5cc6d7c1096c569449db78dbc42da01") ),    /* 4bpp lg sprites, planes 0+1 */
            ROM_LOAD( "pp1_19.4n",    0x2000, 0x2000, CRC("43ff83e1") + SHA1("8f830549a629b019125e59801e5027e4e4b3c0f2") ),
            ROM_LOAD( "pp1_21.3n",    0x4000, 0x2000, CRC("5f958eb4") + SHA1("b56d84e5e5e0ddeb0e71851ba66e5fa1b1409551") ),
            ROM_LOAD( "pp1_18.5m",    0x8000, 0x2000, CRC("6f9997d2") + SHA1("b26d505266ccf23bfd867f881756c3251c80f57b") ),    /* 4bpp lg sprites, planes 2+3 */
            ROM_LOAD( "pp1_20.4m",    0xa000, 0x2000, CRC("ec18075b") + SHA1("af7be549c5fa47551a8dca4c0a531552147fa50f") ),
            ROM_LOAD( "pp1_22.3m",    0xc000, 0x2000, CRC("1d2f30b1") + SHA1("1d88a3069e9b15febd2835dd63e5511b3b2a6b45") ),

            ROM_REGION( 0x5000, "gfx5", 0 ),     /* road generation ROMs needed at runtime */
            ROM_LOAD( "pp1_30.3a",    0x0000, 0x2000, CRC("ee6b3315") + SHA1("9cc26c6d3604c0f60d716f86e67e9d9c0487f87d") ),    /* road control */
            ROM_LOAD( "pp1_31.2a",    0x2000, 0x2000, CRC("6d1e7042") + SHA1("90113ff0c93ed86d95067290088705bb5e6608d1") ),    /* road bits 1 */
            ROM_LOAD( "pp1_32.1a",    0x4000, 0x1000, CRC("4e97f101") + SHA1("f377d053821c74aee93ebcd30a4d43e6156f3cfe") ),    /* road bits 2 */

            ROM_REGION( 0x1000, "gfx6", 0 ),     /* sprite scaling */
            ROM_LOAD( "pp1_27.1l",    0x0000, 0x1000, CRC("a61bff15") + SHA1("f7a59970831cdaaa7bf59c2221a38e4746c54244") ),    /* vertical scaling */

            /* graphics (P)ROM data */
            ROM_REGION( 0x1040, "proms", 0 ),
            ROM_LOAD( "pp1-7.8l",    0x0000, 0x0100, CRC("f07ff2ad") + SHA1("e1f3cb10a03d23f8c1d422acf271dba4e7b98cb1") ),    /* red palette */
            ROM_LOAD( "pp1-8.9l",    0x0100, 0x0100, CRC("adbde7d7") + SHA1("956ac5117c1e310f554ac705aa2dc24a796c36a5") ),    /* green palette */
            ROM_LOAD( "pp1-9.10l",   0x0200, 0x0100, CRC("ddac786a") + SHA1("d1860105bf91297533ccc4aa6775987df198d0fa") ),    /* blue palette */
            ROM_LOAD( "pp2-10.2h",   0x0300, 0x0100, CRC("1e8d0491") + SHA1("e8bf1db5c1fb04a35763099965cf5c588240bde5") ),    /* alpha color - Same as pp1-10.2h - Verified */
            ROM_LOAD( "pp1-11.4d",   0x0400, 0x0100, CRC("0e4fe8a0") + SHA1("d330b1e5ebccf5bbefcf71486fd80d816de38196") ),    /* background color */
            ROM_LOAD( "pp1-15.9a",   0x0500, 0x0100, CRC("2d502464") + SHA1("682b7dd22e51d5db52c0804b7e27e47641dfa6bd") ),    /* vertical position low */
            ROM_LOAD( "pp1-16.10a",  0x0600, 0x0100, CRC("027aa62c") + SHA1("c7030d8b64b80e107c446f6fbdd63f560c0a91c0") ),    /* vertical position med */
            ROM_LOAD( "pp1-17.11a",  0x0700, 0x0100, CRC("1f8d0df3") + SHA1("b8f17758f114f5e247b65b3f2922ca2660757e66") ),    /* vertical position hi */
            ROM_LOAD( "pp1-12.3c",   0x0800, 0x0400, CRC("7afc7cfc") + SHA1("ba2407f6eff124e881b354f13205a4c058b7cf60") ),    /* road color */
            ROM_LOAD( "pp3-6.6m",    0x0c00, 0x0400, CRC("63fb6057") + SHA1("453fbdfd053c2a026cd41b57d0a71754b69a15da") ),    /* sprite color */
            ROM_LOAD( "pp1-13.8e",   0x1000, 0x0020, CRC("4330a51b") + SHA1("9531d18ce2de4eda9913d47ef8c5cd8f05791716") ),    /* video RAM address decoder (not used) */
            ROM_LOAD( "pp1-14.9e",   0x1020, 0x0020, CRC("4330a51b") + SHA1("9531d18ce2de4eda9913d47ef8c5cd8f05791716") ),    /* video RAM address decoder (not used) */

            /* sound (P)ROM data */
            ROM_REGION( 0x0100, "namco", 0 ),
            ROM_LOAD( "pp1-5.3b",    0x0000, 0x0100, CRC("8568decc") + SHA1("0aac1fa082858d4d201e21511c609a989f9a1535") ),    /* Namco sound */

            ROM_REGION( 0x4000, "engine", 0 ),
            ROM_LOAD( "pp1_15.6a",    0x0000, 0x2000, CRC("b5ad4d5f") + SHA1("c07e77a050200d6fe9952031f971ca35f4d15ff8") ),    /* engine sound */
            ROM_LOAD( "pp1_16.5a",    0x2000, 0x2000, CRC("8fdd2f6f") + SHA1("3818dc94c60cd78c4212ab7a4367cf3d98166ee6") ),    /* engine sound */

            ROM_REGION( 0x8000, "52xx", 0 ),
            ROM_LOAD( "pp2_11.2e",    0x0000, 0x2000, CRC("5b4cf05e") + SHA1("52342572940489175607bbf5b6cfd05ee9b0f004") ),    /* voice */
            ROM_LOAD( "pp2_12.2f",    0x2000, 0x2000, CRC("32b694c2") + SHA1("101d9da28333ca290b0235eefb5ec9b094e1736e") ),    /* voice */
            ROM_LOAD( "pp2_13.1e",    0x4000, 0x2000, CRC("8842138a") + SHA1("7e94f5b6ee32f6af37df54cfb72d96f9b543f9e2") ),    /* voice */
            /* No ROM PPx 14 is present. Empty socket on the PCB */

            /* unknown or unused (P)ROM data */
            ROM_REGION( 0x0100, "user1", 0 ),
            ROM_LOAD( "pp1-4.9h",    0x0000, 0x0100, CRC("2401c817") + SHA1("8991b7994513a469e64392fa8f233af5e5f06d54") ),    /* sync chain */

            ROM_END,
        };


        //ROM_START( poleposj )

        //ROM_START( poleposa1 )

        //ROM_START( poleposa2 )

        //ROM_START( topracer )

        //ROM_START( topracera )

        //ROM_START( ppspeed )

        //ROM_START( topracern )

        //ROM_START( polepos2 )

        //ROM_START( polepos2a )

        //ROM_START( polepos2b )

        //ROM_START( polepos2bi )

        //ROM_START( polepos2bs )

        //ROM_START( grally )
    }


        /*********************************************************************
         * Initialization routines
         *********************************************************************/

        //void polepos_state::init_polepos2()


    partial class polepos : construct_ioport_helper
    {
        /*********************************************************************
         * Game drivers
         *********************************************************************/

        static void polepos_state_polepos(machine_config config, device_t device) { polepos_state polepos_state = (polepos_state)device; polepos_state.polepos(config); }

        static polepos m_polepos = new polepos();

        static device_t device_creator_polepos(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new polepos_state(mconfig, type, tag); }


        /*                                                                                            YEAR    NAME       PARENT MACHINE                INPUT                                INIT                      ROT   COMPANY  FULLNAME                 FLAGS */
        public static readonly game_driver driver_polepos = GAME(device_creator_polepos, rom_polepos, "1982", "polepos", "0",   polepos_state_polepos, m_polepos.construct_ioport_poleposa, driver_device.empty_init, ROT0, "Namco", "Pole Position (World)", MACHINE_SUPPORTS_SAVE);
    }
}
