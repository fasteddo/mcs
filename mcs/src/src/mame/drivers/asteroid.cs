// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u32 = System.UInt32;
using uint8_t = System.Byte;

using static mame._74153_global;
using static mame.avgdvg_global;
using static mame.disound_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.er2055_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.input_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.m6502_global;
using static mame.output_latch_global;
using static mame.pokey_global;
using static mame.rescap_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.util;
using static mame.vector_global;
using static mame.watchdog_global;


namespace mame
{
    partial class asteroid_state : driver_device
    {
        static readonly XTAL MASTER_CLOCK = new XTAL(12_096_000);
        static readonly XTAL CLOCK_3KHZ = MASTER_CLOCK / 4096;


        /*************************************
         *
         *  Coin counters
         *
         *************************************/

        //WRITE_LINE_MEMBER(asteroid_state::coin_counter_left_w)
        void coin_counter_left_w(int state)
        {
            machine().bookkeeping().coin_counter_w(0, state);
        }

        //WRITE_LINE_MEMBER(asteroid_state::coin_counter_center_w)
        void coin_counter_center_w(int state)
        {
            machine().bookkeeping().coin_counter_w(1, state);
        }

        //WRITE_LINE_MEMBER(asteroid_state::coin_counter_right_w)
        void coin_counter_right_w(int state)
        {
            machine().bookkeeping().coin_counter_w(2, state);
        }



        /*************************************
         *
         *  High score EAROM
         *
         *************************************/

        //uint8_t asteroid_state::earom_read()

        //void asteroid_state::earom_write(offs_t offset, uint8_t data)


        void earom_control_w(uint8_t data)
        {
            // CK = DB0, C1 = /DB2, C2 = DB1, CS1 = DB3, /CS2 = GND
            m_earom.op0.set_control((uint8_t)BIT(data, 3), 1, BIT(data, 2) == 0 ? (uint8_t)1 : (uint8_t)0, (uint8_t)BIT(data, 1));
            m_earom.op0.set_clk(BIT(data, 0));
        }



        /*************************************
         *
         *  Main CPU memory handlers
         *
         *************************************/

        void asteroid_map(address_map map, device_t device)
        {
            map.global_mask(0x7fff);
            map.op(0x0000, 0x01ff).ram();
            map.op(0x0200, 0x02ff).bankrw("ram1");
            map.op(0x0300, 0x03ff).bankrw("ram2");
            map.op(0x2000, 0x2007).r(asteroid_IN0_r).nopw();     // IN0
            map.op(0x2400, 0x2407).r(asteroid_IN1_r);            // IN1
            map.op(0x2800, 0x2803).r(asteroid_DSW1_r).nopw();    // DSW1
            map.op(0x3000, 0x3000).w(m_dvg, (data) => { m_dvg.op0.go_w(data); });
            map.op(0x3200, 0x3200).w("outlatch", (data) => { ((output_latch_device)subdevice("outlatch")).write(data); });
            map.op(0x3400, 0x3400).w("watchdog", (data) => { ((watchdog_timer_device)subdevice("watchdog")).reset_w(data); });
            map.op(0x3600, 0x3600).w(asteroid_explode_w);
            map.op(0x3a00, 0x3a00).w(asteroid_thump_w);
            map.op(0x3c00, 0x3c07).w("audiolatch", (offset, data) => { ((ls259_device)subdevice("audiolatch")).write_d7(offset, data); });
            map.op(0x3e00, 0x3e00).w(asteroid_noise_reset_w);
            map.op(0x4000, 0x47ff).ram();                     // vector RAM
            map.op(0x5000, 0x57ff).rom();                     // vector ROM
            map.op(0x6800, 0x7fff).rom();
        }


        void astdelux_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
#endif
        }


        //void asteroid_state::llander_map(address_map &map)



        /*************************************
         *
         *  Port definitions
         *
         *************************************/

        //READ_LINE_MEMBER(asteroid_state::clock_r)
        public int clock_r()
        {
            return (m_maincpu.op0.total_cycles() & 0x100) != 0 ? 1 : 0;
        }
    }


    public partial class asteroid : construct_ioport_helper
    {
        //static INPUT_PORTS_START( asteroid )
        void construct_ioport_asteroid(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            asteroid_state asteroid_state = (asteroid_state)owner;

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_UNKNOWN );
            // Bit 2 is the 3 KHz source and Bit 3 the VG_HALT bit
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_READ_LINE_MEMBER(asteroid_state.clock_r);
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_CUSTOM ); PORT_READ_LINE_DEVICE_MEMBER("dvg", () => { return ((dvg_device)asteroid_state.subdevice("dvg")).done_r(); });
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_BUTTON5 ); PORT_CODE(KEYCODE_SPACE); PORT_CODE(JOYCODE_BUTTON3);       // Hyperspace
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_BUTTON3 ); PORT_CODE(KEYCODE_LCONTROL); PORT_CODE(JOYCODE_BUTTON1);    // Fire
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_SERVICE1 ); PORT_NAME("Diagnostic Step");
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_TILT );
            PORT_SERVICE( 0x80, IP_ACTIVE_HIGH );

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_COIN1 );
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_COIN2 );
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_COIN3 );
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_START1 );
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_START2 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_BUTTON4 ); PORT_CODE(KEYCODE_LALT); PORT_CODE(JOYCODE_BUTTON2);        // Thrust
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_BUTTON2 ); PORT_CODE(KEYCODE_RIGHT); PORT_CODE(JOYCODE_X_RIGHT_SWITCH);// Right
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_BUTTON1 ); PORT_CODE(KEYCODE_LEFT); PORT_CODE(JOYCODE_X_LEFT_SWITCH);  // Left

            PORT_START("DSW1");
            PORT_DIPNAME( 0x03, 0x00, DEF_STR( Language ) ); PORT_DIPLOCATION("SW:1,2");
            PORT_DIPSETTING (   0x00, DEF_STR( English ) );
            PORT_DIPSETTING (   0x01, DEF_STR( German ) );
            PORT_DIPSETTING (   0x02, DEF_STR( French ) );
            PORT_DIPSETTING (   0x03, DEF_STR( Spanish ) );
            PORT_DIPNAME( 0x04, 0x04, DEF_STR( Lives ) );    PORT_DIPLOCATION("SW:3");
            PORT_DIPSETTING (   0x04, "3" );
            PORT_DIPSETTING (   0x00, "4" );
            PORT_DIPNAME( 0x08, 0x00, "Center Mech" );       PORT_DIPLOCATION("SW:4"); // Left/Center for 3-door mech
            PORT_DIPSETTING (   0x00, "X 1" );
            PORT_DIPSETTING (   0x08, "X 2" );
            PORT_DIPNAME( 0x30, 0x00, "Right Mech" );        PORT_DIPLOCATION("SW:5,6");
            PORT_DIPSETTING (   0x00, "X 1" );
            PORT_DIPSETTING (   0x10, "X 4" );
            PORT_DIPSETTING (   0x20, "X 5" );
            PORT_DIPSETTING (   0x30, "X 6" );
            PORT_DIPNAME( 0xc0, 0x80, DEF_STR( Coinage ) );  PORT_DIPLOCATION("SW:7,8");
            PORT_DIPSETTING (   0xc0, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING (   0x80, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING (   0x40, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING (   0x00, DEF_STR( Free_Play ) );

            PORT_START("COCKTAIL");
            PORT_CONFNAME(1, 0, DEF_STR(Cabinet));
            PORT_CONFSETTING(0, DEF_STR(Upright));
            PORT_CONFSETTING(1, DEF_STR(Cocktail));

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( asteroidb )

        //static INPUT_PORTS_START( aerolitos )

        //static INPUT_PORTS_START( asterock )


        //static INPUT_PORTS_START( astdelux )
        void construct_ioport_astdelux(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            throw new emu_unimplemented();
#if false
#endif
        }


        //static INPUT_PORTS_START( llander )

        //static INPUT_PORTS_START( llander1 )

        //static INPUT_PORTS_START( llandert )
    }


    partial class asteroid_state : driver_device
    {
        /*************************************
         *
         *  Machine drivers
         *
         *************************************/

        void asteroid_base(machine_config config)
        {
            // Basic machine hardware
            M6502(config, m_maincpu, MASTER_CLOCK / 8);
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, asteroid_map);
            m_maincpu.op0.execute().set_periodic_int(asteroid_interrupt, attotime.from_hz(CLOCK_3KHZ / 12));

            WATCHDOG_TIMER(config, "watchdog");

            TTL153(config, m_dsw_sel);

            output_latch_device outlatch = OUTPUT_LATCH(config, "outlatch"); // LS174 at N11
            outlatch.bit_handler<u32_const_0>().set_output("led1").invert().reg(); // 2 PLYR START LAMP
            outlatch.bit_handler<u32_const_1>().set_output("led0").invert().reg(); // 1 PLYR START LAMP
            outlatch.bit_handler<u32_const_2>().set_membank("ram1").reg(); // RAMSEL
            outlatch.bit_handler<u32_const_2>().append_membank("ram2").reg();
            outlatch.bit_handler<u32_const_2>().append(cocktail_inv_w).reg();
            outlatch.bit_handler<u32_const_3>().set((write_line_delegate)coin_counter_left_w).reg(); // COIN CNTRL
            outlatch.bit_handler<u32_const_4>().set((write_line_delegate)coin_counter_center_w).reg(); // COIN CNTRC
            outlatch.bit_handler<u32_const_5>().set((write_line_delegate)coin_counter_right_w).reg(); // COIN CNTRR

            // Video hardware
            VECTOR(config, "vector");
            screen_device screen = SCREEN(config, "screen", SCREEN_TYPE_VECTOR);
            screen.set_refresh_hz(CLOCK_3KHZ / 12 / 4);
            screen.set_size(400, 300);
            screen.set_visarea(522, 1566, 394, 1182);
            screen.set_screen_update("vector", (screen_device screen2, bitmap_rgb32 bitmap, rectangle cliprect) => { return ((vector_device)subdevice("vector")).screen_update(screen2, bitmap, cliprect); });

            DVG(config, m_dvg, 0);
            m_dvg.op0.set_vector("vector");
            m_dvg.op0.set_memory(m_maincpu, AS_PROGRAM, 0x4000);
        }


        public void asteroid(machine_config config)
        {
            asteroid_base(config);
        
            // Sound hardware
            asteroid_sound(config);
        }


        //void asteroid_state::asterock(machine_config &config)


        public void astdelux(machine_config config)
        {
            asteroid_base(config);

            // Basic machine hardware
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, astdelux_map);

            ER2055(config, m_earom);

            // Sound hardware
            astdelux_sound(config);

            pokey_device pokey = POKEY(config, "pokey", MASTER_CLOCK / 8);
            pokey.allpot_r().set_ioport("DSW2").reg();
            pokey.set_output_rc(RES_K(10), CAP_U(0.015), 5.0);
            pokey.disound.add_route(ALL_OUTPUTS, "mono", 1.0);

            config.device_remove("outlatch");

            ls259_device audiolatch = subdevice<ls259_device>("audiolatch");
            audiolatch.q_out_cb<u32_const_0>().set_output("led0").invert().reg(); // START1
            audiolatch.q_out_cb<u32_const_1>().set_output("led1").invert().reg(); // START2
            audiolatch.q_out_cb<u32_const_4>().set_membank("ram1").reg(); // RAMSEL
            audiolatch.q_out_cb<u32_const_4>().append_membank("ram2").reg();
            audiolatch.q_out_cb<u32_const_4>().append(cocktail_inv_w).reg();
            audiolatch.q_out_cb<u32_const_5>().set((write_line_delegate)coin_counter_left_w).reg(); // LEFT COIN
            audiolatch.q_out_cb<u32_const_6>().set((write_line_delegate)coin_counter_center_w).reg(); // CENTER COIN
            audiolatch.q_out_cb<u32_const_7>().set((write_line_delegate)coin_counter_right_w).reg(); // RIGHT COIN
        }


        //void asteroid_state::llander(machine_config &config)
    }


    partial class asteroid : construct_ioport_helper
    {
        /*************************************
         *
         *  ROM definitions
         *
         *************************************/

        //ROM_START( asteroid )
        static readonly tiny_rom_entry [] rom_asteroid =
        {
            ROM_REGION( 0x8000, "maincpu", 0 ),
            ROM_LOAD( "035145-04e.ef2", 0x6800, 0x0800, CRC("b503eaf7") + SHA1("5369dcfe01c0b9e48b15a96a0de8d23ee8ef9145") ),
            ROM_LOAD( "035144-04e.h2",  0x7000, 0x0800, CRC("25233192") + SHA1("51b2865fa897cdaa84ac6500c4b4833a80827019") ),
            ROM_LOAD( "035143-02.j2",   0x7800, 0x0800, CRC("312caa02") + SHA1("1ce2eac1ab90b972e3f1fc3d250908f26328d6cb") ),
            // Vector ROM
            ROM_LOAD( "035127-02.np3",  0x5000, 0x0800, CRC("8b71fd9e") + SHA1("8cd5005e531eafa361d6b7e9eed159d164776c70") ),

            // DVG PROM
            ROM_REGION( 0x100, "dvg:prom", 0 ),
            ROM_LOAD( "034602-01.c8",   0x0000, 0x0100, CRC("97953db8") + SHA1("8cbded64d1dd35b18c4d5cece00f77e7b2cab2ad") ),

            ROM_END,
        };


        //ROM_START( asteroid2 )

        //ROM_START( asteroid1 )

        //ROM_START( asteroidb1 )

        //ROM_START( asteroidb2 )

        //ROM_START( spcrocks )

        //ROM_START( aerolitos )

        //ROM_START( asterock )

        //ROM_START( asterockv )

        //ROM_START( meteorite )

        //ROM_START( meteorts )

        //ROM_START( meteorho )

        //ROM_START( meteorbl )

        //ROM_START( hyperspc )


        //ROM_START( astdelux )
        static readonly tiny_rom_entry [] rom_astdelux =
        {
            ROM_REGION( 0x8000, "maincpu", 0 ),
            ROM_LOAD( "036430-02.d1",  0x6000, 0x0800, CRC("a4d7a525") + SHA1("abe262193ec8e1981be36928e9a89a8ac95cd0ad") ),
            ROM_LOAD( "036431-02.ef1", 0x6800, 0x0800, CRC("d4004aae") + SHA1("aa2099b8fc62a79879efeea70ea1e9ed77e3e6f0") ),
            ROM_LOAD( "036432-02.fh1", 0x7000, 0x0800, CRC("6d720c41") + SHA1("198218cd2f43f8b83e4463b1f3a8aa49da5015e4") ),
            ROM_LOAD( "036433-03.j1",  0x7800, 0x0800, CRC("0dcc0be6") + SHA1("bf10ffb0c4870e777d6b509cbede35db8bb6b0b8") ),
            // Vector ROM
            ROM_LOAD( "036800-02.r2",  0x4800, 0x0800, CRC("bb8cabe1") + SHA1("cebaa1b91b96e8b80f2b2c17c6fd31fa9f156386") ),
            ROM_LOAD( "036799-01.np2", 0x5000, 0x0800, CRC("7d511572") + SHA1("1956a12bccb5d3a84ce0c1cc10c6ad7f64e30b40") ),

            // DVG PROM
            ROM_REGION( 0x100, "dvg:prom", 0 ),
            ROM_LOAD( "034602-01.c8",  0x0000, 0x0100, CRC("97953db8") + SHA1("8cbded64d1dd35b18c4d5cece00f77e7b2cab2ad") ),

            ROM_REGION( 0x40, "earom", ROMREGION_ERASE00 ), // default to zero fill to suppress invalid high score display

            ROM_END,
        };


        //ROM_START( astdelux2 )

        //ROM_START( astdelux1 )

        //ROM_START( llander )

        //ROM_START( llander1 )

        //ROM_START( llandert )


        /*************************************
         *
         *  Driver initialization
         *
         *************************************/

        //void asteroid_state::init_asteroidb()

        //void asteroid_state::init_asterock()


        /*************************************
         *
         *  Game drivers
         *
         *************************************/

        static void asteroid_state_asteroid(machine_config config, device_t device) { asteroid_state asteroid_state = (asteroid_state)device; asteroid_state.asteroid(config); }
        static void asteroid_state_astdelux(machine_config config, device_t device) { asteroid_state asteroid_state = (asteroid_state)device; asteroid_state.astdelux(config); }

        static asteroid m_asteroid = new asteroid();

        static device_t device_creator_asteroid(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new asteroid_state(mconfig, type, tag); }


        //                                                        creator,                  rom           YEAR    NAME        PARENT  MACHINE                  INPUT                                 INIT                      ROT   COMPANY, FULLNAME,                   FLAGS                  LAYOUT
        public static readonly game_driver driver_asteroid = GAME(device_creator_asteroid,  rom_asteroid, "1979", "asteroid", "0",    asteroid_state_asteroid, m_asteroid.construct_ioport_asteroid, driver_device.empty_init, ROT0, "Atari", "Asteroids (rev 4)",        MACHINE_SUPPORTS_SAVE);

        public static readonly game_driver driver_astdelux = GAMEL(device_creator_asteroid, rom_astdelux, "1980", "astdelux", "0",    asteroid_state_astdelux, m_asteroid.construct_ioport_astdelux, driver_device.empty_init, ROT0, "Atari", "Asteroids Deluxe (rev 3)", MACHINE_SUPPORTS_SAVE, null /*layout_astdelux*/);
    }
}
