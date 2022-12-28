// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u16 = System.UInt16;
using u32 = System.UInt32;

using static mame._74157_global;
using static mame._74259_global;
using static mame.diexec_global;
using static mame.digfx_global;
using static mame.disound_global;
using static mame.drawgfx_global;
using static mame.driver_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.m6809_global;
using static mame.namco_global;
using static mame.namcoio_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.watchdog_global;


namespace mame
{
    partial class mappy_state : driver_device
    {
        /*************************************
         *  Constants
         *************************************/

        static readonly XTAL MASTER_CLOCK        = new XTAL(18_432_000);

        static readonly XTAL PIXEL_CLOCK         = MASTER_CLOCK / 3;

        // H counts from 128->511, HBLANK starts at 144 and ends at 240
        const u16 HTOTAL              = 384;
        const u16 HBEND               = 0;     // (96+16)
        const u16 HBSTART             = 288;   // (16)

        const u16 VTOTAL              = 264;
        const u16 VBEND               = 0;     // (16)
        const u16 VBSTART             = 224;   // (224+16)


        /***************************************************************************/

        //WRITE_LINE_MEMBER(mappy_state::int_on_w)
        void int_on_w(int state)
        {
            throw new emu_unimplemented();
#if false
            m_main_irq_mask = state;
            if (!state)
                m_maincpu->set_input_line(0, CLEAR_LINE);
#endif
        }


        //WRITE_LINE_MEMBER(mappy_state::int_on_2_w)
        void int_on_2_w(int state)
        {
            throw new emu_unimplemented();
#if false
            m_sub_irq_mask = state;
            if (!state)
                m_subcpu->set_input_line(0, CLEAR_LINE);
#endif
        }


#if false
        WRITE_LINE_MEMBER(mappy_state::int_on_3_w)
        {
            m_sub2_irq_mask = state;
            if (!state)
                m_subcpu2->set_input_line(0, CLEAR_LINE);
        }

        WRITE_LINE_MEMBER(mappy_state::mappy_flip_w)
        {
            flip_screen_set(state);
        }


        template<uint8_t Chip>
        TIMER_CALLBACK_MEMBER(mappy_state::namcoio_run_timer)
        {
            m_namcoio[Chip]->customio_run();
        }
#endif


        //WRITE_LINE_MEMBER(mappy_state::vblank_irq)
        void vblank_irq(int state)
        {
            throw new emu_unimplemented();
#if false
            if (!state)
                return;

            if (m_main_irq_mask)
                m_maincpu->set_input_line(0, ASSERT_LINE);

            if (!m_namcoio[0]->read_reset_line())        // give the cpu a tiny bit of time to write the command before processing it
                m_namcoio_run_timer[0]->adjust(attotime::from_usec(50));

            if (!m_namcoio[1]->read_reset_line())        // give the cpu a tiny bit of time to write the command before processing it
                m_namcoio_run_timer[1]->adjust(attotime::from_usec(50));

            if (m_sub_irq_mask)
                m_subcpu->set_input_line(0, ASSERT_LINE);

            if (m_subcpu2.found() && m_sub2_irq_mask)
                m_subcpu2->set_input_line(0, ASSERT_LINE);
#endif
        }


        void superpac_cpu1_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map(0x0000, 0x07ff).ram().w(FUNC(mappy_state::superpac_videoram_w)).share("videoram");
            map(0x0800, 0x1fff).ram().share("spriteram");   // work RAM with embedded sprite RAM
            map(0x2000, 0x2000).rw(FUNC(mappy_state::superpac_flipscreen_r), FUNC(mappy_state::superpac_flipscreen_w));
            map(0x4000, 0x43ff).rw(m_namco_15xx, FUNC(namco_15xx_device::sharedram_r), FUNC(namco_15xx_device::sharedram_w));   // shared RAM with the sound CPU
            map(0x4800, 0x480f).rw("namcoio_1", FUNC(namcoio_device::read), FUNC(namcoio_device::write));   // custom I/O chips interface
            map(0x4810, 0x481f).rw("namcoio_2", FUNC(namcoio_device::read), FUNC(namcoio_device::write));   // custom I/O chips interface
            map(0x5000, 0x500f).w("mainlatch", FUNC(ls259_device::write_a0));   // various control bits
            map(0x8000, 0x8000).w("watchdog", FUNC(watchdog_timer_device::reset_w));
            map(0xa000, 0xffff).rom();
#endif
        }


        //void mappy_state::phozon_cpu1_map(address_map &map)

        //void mappy_state::mappy_cpu1_map(address_map &map)


        void superpac_cpu2_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map(0x0000, 0x03ff).rw(m_namco_15xx, FUNC(namco_15xx_device::sharedram_r), FUNC(namco_15xx_device::sharedram_w));   // shared RAM with the main CPU (also sound registers)
            map(0x2000, 0x200f).w("mainlatch", FUNC(ls259_device::write_a0));   // various control bits
            map(0xe000, 0xffff).rom();
#endif
        }


        //void mappy_state::phozon_cpu2_map(address_map &map)

        //void mappy_state::mappy_cpu2_map(address_map &map)

        //void mappy_state::phozon_cpu3_map(address_map &map)
    }


    partial class mappy : construct_ioport_helper
    {
#if false
        //#define NAMCO_56IN0\
            PORT_START("P1")    /* 56XX #0 pins 22-29 */\
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ) PORT_4WAY\
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ) PORT_4WAY\
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ) PORT_4WAY\
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ) PORT_4WAY\
            PORT_START("P2")    /* 56XX #0 pins 22-29 */\
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ) PORT_4WAY PORT_COCKTAIL\
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ) PORT_4WAY PORT_COCKTAIL\
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ) PORT_4WAY PORT_COCKTAIL\
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ) PORT_4WAY PORT_COCKTAIL

        //#define NAMCO_5XIN0\
            PORT_START("P1") /* 56XX #0 pins 22-29 */\
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ) PORT_8WAY\
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ) PORT_8WAY\
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ) PORT_8WAY\
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ) PORT_8WAY\
            PORT_START("P2") /* 56XX #0 pins 22-29 */\
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ) PORT_8WAY PORT_COCKTAIL\
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ) PORT_8WAY PORT_COCKTAIL\
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ) PORT_8WAY PORT_COCKTAIL\
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ) PORT_8WAY PORT_COCKTAIL

        //#define NAMCO_56IN1\
            PORT_START("BUTTONS")   /* 56XX #0 pins 30-33 and 38-41 */\
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_BUTTON1 )\
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON1 ) PORT_COCKTAIL\
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_START1 )\
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_START2 )\
            PORT_START("COINS") /* 56XX #0 pins 30-33 and 38-41 */\
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_COIN1 )\
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_COIN2 )\
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_UNUSED )\
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_SERVICE1 )

        //#define NAMCO_56DSW0\
            PORT_START("DSW0")  /* 56XX #1 pins 30-33 */\
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_UNUSED )\
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_UNUSED )\
            PORT_DIPNAME( 0x04, 0x04, DEF_STR( Cabinet ) )\
            PORT_DIPSETTING(    0x04, DEF_STR( Upright ) )\
            PORT_DIPSETTING(    0x00, DEF_STR( Cocktail ) )\
            PORT_SERVICE( 0x08, IP_ACTIVE_LOW )
#endif


        //static INPUT_PORTS_START( superpac )
        void construct_ioport_superpac(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            throw new emu_unimplemented();
#if false
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            NAMCO_56IN0
            NAMCO_56IN1
            NAMCO_56DSW0

            PORT_START("DSW1")  // 56XX #1 pins 22-29
            PORT_DIPNAME( 0x0f, 0x0f, DEF_STR( Difficulty ) )   PORT_DIPLOCATION("SW1:1,2,3,4")
            PORT_DIPSETTING(    0x0f, "Rank 0-Normal" )
            PORT_DIPSETTING(    0x0e, "Rank 1-Easiest" )
            PORT_DIPSETTING(    0x0d, "Rank 2" )
            PORT_DIPSETTING(    0x0c, "Rank 3" )
            PORT_DIPSETTING(    0x0b, "Rank 4" )
            PORT_DIPSETTING(    0x0a, "Rank 5" )
            PORT_DIPSETTING(    0x09, "Rank 6-Medium" )
            PORT_DIPSETTING(    0x08, "Rank 7" )
            PORT_DIPSETTING(    0x07, "Rank 8-Default" )
            PORT_DIPSETTING(    0x06, "Rank 9" )
            PORT_DIPSETTING(    0x05, "Rank A" )
            PORT_DIPSETTING(    0x04, "Rank B-Hardest" )
            PORT_DIPSETTING(    0x03, "Rank C-Easy Auto" )
            PORT_DIPSETTING(    0x02, "Rank D-Auto" )
            PORT_DIPSETTING(    0x01, "Rank E-Auto" )
            PORT_DIPSETTING(    0x00, "Rank F-Hard Auto" )
            PORT_DIPNAME( 0x30, 0x30, DEF_STR( Coin_B ) )       PORT_DIPLOCATION("SW1:5,6")
            PORT_DIPSETTING(    0x30, DEF_STR( 1C_1C ) )
            PORT_DIPSETTING(    0x20, DEF_STR( 1C_2C ) )
            PORT_DIPSETTING(    0x10, DEF_STR( 2C_1C ) )
            PORT_DIPSETTING(    0x00, DEF_STR( 2C_3C ) )
            PORT_DIPNAME( 0x40, 0x40, DEF_STR( Demo_Sounds ) )  PORT_DIPLOCATION("SW1:7")
            PORT_DIPSETTING(    0x40, DEF_STR( On ) )
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) )
            // When Freeze is on, press P1 button 1 to skip levels
            PORT_DIPNAME( 0x80, 0x80, "Freeze / Rack Test (Cheat)" ) PORT_CODE(KEYCODE_F1) PORT_TOGGLE PORT_DIPLOCATION("SW1:8")
            PORT_DIPSETTING(    0x80, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x00, DEF_STR( On ) )

            PORT_START("DSW2")  // 56XX #1 pins 38-41 multiplexed
            PORT_DIPNAME( 0x07, 0x07, DEF_STR( Coin_A ) )       PORT_DIPLOCATION("SW2:1,2,3")
            PORT_DIPSETTING(    0x07, DEF_STR( 1C_1C ) )
            PORT_DIPSETTING(    0x06, DEF_STR( 1C_2C ) )
            PORT_DIPSETTING(    0x05, DEF_STR( 1C_3C ) )
            PORT_DIPSETTING(    0x04, DEF_STR( 1C_6C ) )
            PORT_DIPSETTING(    0x03, DEF_STR( 1C_7C ) )
            PORT_DIPSETTING(    0x02, DEF_STR( 2C_1C ) )
            PORT_DIPSETTING(    0x01, DEF_STR( 2C_3C ) )
            PORT_DIPSETTING(    0x00, DEF_STR( 3C_1C ) )
            PORT_DIPNAME( 0x38, 0x38, DEF_STR( Bonus_Life ) )   PORT_DIPLOCATION("SW2:4,5,6")
            PORT_DIPSETTING(    0x38, "30k & 100k Only" )           PORT_CONDITION("DSW2",0xc0,NOTEQUALS,0x00)
            PORT_DIPSETTING(    0x30, "30k & 80k Only" )            PORT_CONDITION("DSW2",0xc0,NOTEQUALS,0x00)
            PORT_DIPSETTING(    0x28, "30k & 120k Only" )           PORT_CONDITION("DSW2",0xc0,NOTEQUALS,0x00)
            PORT_DIPSETTING(    0x20, "30k, 80k & Every 80k" )      PORT_CONDITION("DSW2",0xc0,NOTEQUALS,0x00)
            PORT_DIPSETTING(    0x18, "30k, 100k & Every 100k" )    PORT_CONDITION("DSW2",0xc0,NOTEQUALS,0x00)
            PORT_DIPSETTING(    0x10, "30k, 120k & Every 120k" )    PORT_CONDITION("DSW2",0xc0,NOTEQUALS,0x00)
            PORT_DIPSETTING(    0x08, "30k Only" )                  PORT_CONDITION("DSW2",0xc0,NOTEQUALS,0x00)
            PORT_DIPSETTING(    0x38, "30k & 100k Only" )           PORT_CONDITION("DSW2",0xc0,EQUALS,0x00)
            PORT_DIPSETTING(    0x30, "30k & 120k Only" )           PORT_CONDITION("DSW2",0xc0,EQUALS,0x00)
            PORT_DIPSETTING(    0x28, "40k & 120k Only" )           PORT_CONDITION("DSW2",0xc0,EQUALS,0x00)
            PORT_DIPSETTING(    0x20, "30k, 100k & Every 100k" )    PORT_CONDITION("DSW2",0xc0,EQUALS,0x00)
            PORT_DIPSETTING(    0x18, "40k, 120k & Every 120k" )    PORT_CONDITION("DSW2",0xc0,EQUALS,0x00)
            PORT_DIPSETTING(    0x10, "30k Only" )                  PORT_CONDITION("DSW2",0xc0,EQUALS,0x00) // Manual shows 100k only, Test Mode shows 30k which is what we use
            PORT_DIPSETTING(    0x08, "40k Only" )                  PORT_CONDITION("DSW2",0xc0,EQUALS,0x00)
            PORT_DIPSETTING(    0x00, DEF_STR( None ) )
            PORT_DIPNAME( 0xc0, 0xc0, DEF_STR( Lives ) )        PORT_DIPLOCATION("SW2:7,8")
            PORT_DIPSETTING(    0xc0, "3" )
            PORT_DIPSETTING(    0x80, "1" )
            PORT_DIPSETTING(    0x40, "2" )
            PORT_DIPSETTING(    0x00, "5" )

            INPUT_PORTS_END
#endif
        }


        //static INPUT_PORTS_START( pacnpal )

        //static INPUT_PORTS_START( grobda )

        //static INPUT_PORTS_START( phozon )

        //static INPUT_PORTS_START( mappy )

        //static INPUT_PORTS_START( todruaga )

        //static INPUT_PORTS_START( digdug2 )

        //static INPUT_PORTS_START( motos )
    }


    partial class mappy_state : driver_device
    {
        static readonly gfx_layout charlayout = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,1),
            2,
            new u32[] { 0, 4 },
            new u32[] { 8*8+0, 8*8+1, 8*8+2, 8*8+3, 0, 1, 2, 3 },
            new u32[] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
            16*8
        );


        static readonly gfx_layout spritelayout_2bpp = new gfx_layout
        (
            16,16,
            RGN_FRAC(1,1),
            2,
            new u32[] { 0, 4 },
            new u32[] { 0, 1, 2, 3, 8*8, 8*8+1, 8*8+2, 8*8+3,
                    16*8+0, 16*8+1, 16*8+2, 16*8+3, 24*8+0, 24*8+1, 24*8+2, 24*8+3 },
            new u32[] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
                    32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64*8
        );


        //static const gfx_layout spritelayout_8x8 =

        //static const gfx_layout spritelayout_4bpp =


        //static GFXDECODE_START( gfx_superpac )
        static readonly gfx_decode_entry [] gfx_superpac =
        {
            GFXDECODE_ENTRY( "gfx1", 0, charlayout,           0, 64 ),
            GFXDECODE_ENTRY( "gfx2", 0, spritelayout_2bpp, 64*4, 64 ),

            //GFXDECODE_END
        };


        //static GFXDECODE_START( gfx_phozon )

        //static GFXDECODE_START( gfx_mappy )

        //static GFXDECODE_START( gfx_todruaga )


        /***************************************************************************
          Custom I/O initialization
        ***************************************************************************/
#if false
        void mappy_state::out_lamps(uint8_t data)
        {
            m_leds[0] = BIT(data, 0);
            m_leds[1] = BIT(data, 1);
            machine().bookkeeping().coin_lockout_global_w(data & 4);
            machine().bookkeeping().coin_counter_w(0, ~data & 8);
        }

        void mappy_state::machine_start()
        {
            m_leds.resolve();

            m_namcoio_run_timer[0] = timer_alloc(FUNC(mappy_state::namcoio_run_timer<0>), this);
            m_namcoio_run_timer[1] = timer_alloc(FUNC(mappy_state::namcoio_run_timer<1>), this);

            save_item(NAME(m_main_irq_mask));
            save_item(NAME(m_sub_irq_mask));
            save_item(NAME(m_sub2_irq_mask));
        }
#endif


        void superpac_common(machine_config config)
        {
            // basic machine hardware
            MC6809E(config, m_maincpu, PIXEL_CLOCK / 4);  // 1.536 MHz
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, superpac_cpu1_map);

            MC6809E(config, m_subcpu, PIXEL_CLOCK / 4);   // 1.536 MHz
            m_subcpu.op0.memory().set_addrmap(AS_PROGRAM, superpac_cpu2_map);

            ls259_device mainlatch = LS259(config, "mainlatch"); // 2M on CPU board
            mainlatch.q_out_cb<u32_const_0>().set((write_line_delegate)int_on_2_w).reg();
            mainlatch.q_out_cb<u32_const_1>().set((write_line_delegate)int_on_w).reg();
            mainlatch.q_out_cb<u32_const_3>().set(m_namco_15xx, (int state) => { m_namco_15xx.op0.sound_enable_w(state); }).reg();  //FUNC(namco_15xx_device::sound_enable_w));
            mainlatch.q_out_cb<u32_const_4>().set(m_namcoio[0], (int state) => { m_namcoio[0].op0.set_reset_line(state); }).invert().reg();  //FUNC(namcoio_device::set_reset_line)).invert();
            mainlatch.q_out_cb<u32_const_4>().append(m_namcoio[1], (int state) => { m_namcoio[1].op0.set_reset_line(state); }).invert().reg();  //FUNC(namcoio_device::set_reset_line)).invert();
            mainlatch.q_out_cb<u32_const_5>().set_inputline(m_subcpu, INPUT_LINE_RESET).invert().reg();

            WATCHDOG_TIMER(config, "watchdog").set_vblank_count("screen", 8);

            config.set_maximum_quantum(attotime.from_hz(6000));    // 100 CPU slices per frame - a high value to ensure proper synchronization of the CPUs

            ls157_device dipmux = LS157(config, "dipmux");
            dipmux.a_in_callback().set_ioport("DSW2").reg();
            dipmux.b_in_callback().set_ioport("DSW2").rshift(4).reg();

            // video hardware
            GFXDECODE(config, m_gfxdecode, m_palette, gfx_superpac);
            PALETTE(config, m_palette, superpac_palette, 64*4+64*4, 32);

            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_raw(PIXEL_CLOCK, HTOTAL, HBEND, HBSTART, VTOTAL, VBEND, VBSTART);
            m_screen.op0.set_screen_update(screen_update_superpac);
            m_screen.op0.set_palette(m_palette);
            m_screen.op0.screen_vblank().set((write_line_delegate)vblank_irq).reg();   // cause IRQs on both CPUs; also update the custom I/O chips

            MCFG_VIDEO_START_OVERRIDE(config, video_start_superpac);

            // sound hardware
            SPEAKER(config, "speaker").front_center();

            NAMCO_15XX(config, m_namco_15xx, 18432000 / 768);
            m_namco_15xx.op0.set_voices(8);
            m_namco_15xx.op0.disound.add_route(ALL_OUTPUTS, "speaker", 1.0);
        }


        public void superpac(machine_config config)
        {
            superpac_common(config);

            NAMCO_56XX(config, m_namcoio[0], 0);
            m_namcoio[0].op0.in_callback<u32_const_0>().set_ioport("COINS").reg();
            m_namcoio[0].op0.in_callback<u32_const_1>().set_ioport("P1").reg();
            m_namcoio[0].op0.in_callback<u32_const_2>().set_ioport("P2").reg();
            m_namcoio[0].op0.in_callback<u32_const_3>().set_ioport("BUTTONS").reg();

            NAMCO_56XX(config, m_namcoio[1], 0);
            m_namcoio[1].op0.in_callback<u32_const_0>().set("dipmux", () => { return ((ls157_device)subdevice("dipmux")).output_r(); }).reg();  //FUNC(ls157_device::output_r));
            m_namcoio[1].op0.in_callback<u32_const_1>().set_ioport("DSW1").reg();
            m_namcoio[1].op0.in_callback<u32_const_2>().set_ioport("DSW1").rshift(4).reg();
            m_namcoio[1].op0.in_callback<u32_const_3>().set_ioport("DSW0").reg();
            m_namcoio[1].op0.out_callback<u32_const_0>().set("dipmux", (int state) => { ((ls157_device)subdevice("dipmux")).select_w(state); }).bit(0).reg();  //FUNC(ls157_device::select_w)).bit(0);
        }


        //void mappy_state::pacnpal(machine_config &config)

        //void mappy_state::grobda(machine_config &config)

        //void mappy_state::phozon(machine_config &config)

        //void mappy_state::mappy_common(machine_config &config)

        //void mappy_state::mappy(machine_config &config)

        //void mappy_state::digdug2(machine_config &config)

        //void mappy_state::todruaga(machine_config &config)

        //void mappy_state::motos(machine_config &config)
    }


    partial class mappy : construct_ioport_helper
    {
        //ROM_START( superpac )
        static readonly tiny_rom_entry [] rom_superpac =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "sp1-2.1c",     0xc000, 0x2000, CRC("4bb33d9c") + SHA1("dd87f71b4db090a32a6b791079eedd17580cc741") ),
            ROM_LOAD( "sp1-1.1b",     0xe000, 0x2000, CRC("846fbb4a") + SHA1("f6bf90281986b9b7a3ef1dbbeddb722182e84d7c") ),

            ROM_REGION( 0x10000, "sub", 0 ), // 64k for the second CPU
            ROM_LOAD( "spc-3.1k",     0xf000, 0x1000, CRC("04445ddb") + SHA1("ce7d14963d5ddaefdeaf433a6f82c43cd1611d9b") ),

            ROM_REGION( 0x1000, "gfx1", 0 ),
            ROM_LOAD( "sp1-6.3c",     0x0000, 0x1000, CRC("91c5935c") + SHA1("10579edabc26a0910253fab7d41b4c19ecdaaa09") ),

            ROM_REGION( 0x2000, "gfx2", 0 ),
            ROM_LOAD( "spv-2.3f",     0x0000, 0x2000, CRC("670a42f2") + SHA1("9171922df07e31fd1dc415766f7d2cc50a9d10dc") ),

            ROM_REGION( 0x0220, "proms", 0 ),
            ROM_LOAD( "superpac.4c",  0x0000, 0x0020, CRC("9ce22c46") + SHA1("d97f53ef4c5ef26659a22ed0de4ce7ef3758c924") ), // palette
            ROM_LOAD( "superpac.4e",  0x0020, 0x0100, CRC("1253c5c1") + SHA1("df46a90170e9761d45c90fbd04ef2aa1e8c9944b") ), // chars
            ROM_LOAD( "superpac.3l",  0x0120, 0x0100, CRC("d4d7026f") + SHA1("a486573437c54bfb503424574ad82655491e85e1") ), // sprites

            ROM_REGION( 0x0100, "namco", 0 ),    // sound prom
            ROM_LOAD( "superpac.3m",  0x0000, 0x0100, CRC("ad43688f") + SHA1("072f427453efb1dda8147da61804fff06e1bc4d5") ),

            ROM_END,
        };


        //ROM_START( superpacm )

        //ROM_START( pacnpal )

        //ROM_START( pacnpal2 )

        //ROM_START( pacnchmp )

        //ROM_START( grobda )

        //ROM_START( grobda2 )

        //ROM_START( grobda3 )

        //ROM_START( phozon )

        //ROM_START( phozons )

        //ROM_START( mappy )

        //ROM_START( mappyj )

        //ROM_START( todruaga )

        //ROM_START( todruagao )

        //ROM_START( todruagas )

        //ROM_START( digdug2 )

        //ROM_START( digdug2o )

        //ROM_START( motos )
    }


    //void mappy_state::init_digdug2()


    public partial class mappy : construct_ioport_helper
    {
        static void mappy_state_superpac(machine_config config, device_t device) { mappy_state mappy_state = (mappy_state)device; mappy_state.superpac(config); }

        static mappy m_mappy = new mappy();

        static device_t device_creator_superpac(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new mappy_state(mconfig, type, tag); }

        // 2x6809, static tilemap, 2bpp sprites (Super Pacman type)
        public static readonly game_driver driver_superpac = GAME(device_creator_superpac, rom_superpac, "1982", "superpac", "0", mappy_state_superpac, m_mappy.construct_ioport_superpac, driver_device.empty_init, ROT90, "Namco", "Super Pac-Man", MACHINE_SUPPORTS_SAVE);
    }
}
