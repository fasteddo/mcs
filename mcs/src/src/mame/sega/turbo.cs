// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;

using static mame._74259_global;
using static mame.digfx_global;
using static mame.drawgfx_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.generic_global;
using static mame.hash_global;
using static mame.i8255_global;
using static mame.i8279_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.z80_global;


namespace mame
{
    partial class turbo_base_state : driver_device
    {
        /*************************************
         *  Constants
         *************************************/
        protected static readonly XTAL MASTER_CLOCK        = new XTAL(19_968_000);

        protected static readonly XTAL PIXEL_CLOCK         = MASTER_CLOCK / 4 * TURBO_X_SCALE;

        protected const u16 HTOTAL              = 320 * TURBO_X_SCALE;
        protected const u16 HBEND               = 0;
        protected const u16 HBSTART             = 256 * TURBO_X_SCALE;

        protected const u16 VTOTAL              = 264;
        protected const u16 VBEND               = 0;
        protected const u16 VBSTART             = 224;


        /*************************************
         *  Machine init
         *************************************/
#if false
        void turbo_base_state::machine_start()
        {
            m_digits.resolve();
            m_lamp.resolve();

            save_item(NAME(m_i8279_scanlines));
            save_item(NAME(m_sound_state));
        }
#endif
    }


    //void buckrog_state::machine_start()

    //void buckrog_state::machine_reset()

    //void subroc3d_state::machine_start()


    partial class turbo_state : turbo_base_state
    {
#if false
        void turbo_state::machine_start()
        {
            turbo_base_state::machine_start();

            m_tachometer.resolve();
            m_speed.resolve();

            save_item(NAME(m_osel));
            save_item(NAME(m_bsel));
            save_item(NAME(m_opa));
            save_item(NAME(m_opb));
            save_item(NAME(m_opc));
            save_item(NAME(m_ipa));
            save_item(NAME(m_ipb));
            save_item(NAME(m_ipc));
            save_item(NAME(m_fbpla));
            save_item(NAME(m_fbcol));
            save_item(NAME(m_collision));
            save_item(NAME(m_last_analog));
            save_item(NAME(m_accel));
        }
#endif

        /*************************************
         *  Turbo 8255 PPI handling
         *************************************/

        /*
            chip index:
                0 = IC75 - CPU Board, Sheet 6, D7
                1 = IC32 - CPU Board, Sheet 6, D6
                2 = IC123 - CPU Board, Sheet 6, D4
                3 = IC6 - CPU Board, Sheet 5, D7
        */

        void ppi0a_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            // bit0-7 = 0PA0-7
            m_opa = data;
#endif
        }


        void ppi0b_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            // bit0-7 = 0PB0-7
            m_opb = data;
#endif
        }


        void ppi0c_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            // bit0-7 = 0PC0-7
            m_opc = data;
#endif
        }


        void ppi1a_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            // bit0-7 = 1PA0-7
            m_ipa = data;
#endif
        }


        void ppi1b_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            // bit0-7 = 1PB0-7
            m_ipb = data;
#endif
        }


        void ppi1c_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            // bit0-7 = 1PC0-7
            m_ipc = data;
#endif
        }


        void ppi3c_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            // bit 0-3 = PLA0-3
            // bit 4-6 = COL0-2
            // bit   7 = n/c
            m_fbpla = data & 0x0f;
            m_fbcol = (data >> 4) & 0x07;
#endif
        }
    }


    //void subroc3d_state::ppi0a_w(uint8_t data)

    //void subroc3d_state::ppi0c_w(uint8_t data)

    //void subroc3d_state::ppi0b_w(uint8_t data)

    //void buckrog_state::ppi0a_w(uint8_t data)

    //void buckrog_state::ppi0b_w(uint8_t data)

    //void buckrog_state::ppi0c_w(uint8_t data)

    //void buckrog_state::ppi1c_w(uint8_t data)


    partial class turbo_base_state : driver_device
    {
        /*************************************
         *  8279 display/keyboard driver
         *************************************/
        protected void scanlines_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            m_i8279_scanlines = data;
#endif
        }


        protected void digit_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            static const uint8_t ls48_map[16] =
                { 0x3f,0x06,0x5b,0x4f,0x66,0x6d,0x7c,0x07,0x7f,0x67,0x58,0x4c,0x62,0x69,0x78,0x00 };

            m_digits[m_i8279_scanlines * 2] = ls48_map[data & 0x0f];
            m_digits[m_i8279_scanlines * 2 + 1] = ls48_map[data>>4];
#endif
        }
    }


    partial class turbo_state : turbo_base_state
    {
        /*************************************
         *  Misc Turbo inputs/outputs
         *************************************/
#if false
        uint8_t turbo_state::collision_r()
        {
            m_screen->update_partial(m_screen->vpos());
            return m_dsw3->read() | (m_collision & 15);
        }


        void turbo_state::collision_clear_w(uint8_t data)
        {
            m_screen->update_partial(m_screen->vpos());
            m_collision = 0;
        }
#endif


        uint8_t analog_r()
        {
            throw new emu_unimplemented();
#if false
            return m_dial->read() - m_last_analog;
#endif
        }


        void analog_reset_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            m_last_analog = m_dial->read();
#endif
        }


        //WRITE_LINE_MEMBER(turbo_state::coin_meter_1_w)
        void coin_meter_1_w(int state)
        {
            throw new emu_unimplemented();
#if false
            machine().bookkeeping().coin_counter_w(0, state);
#endif
        }


        //WRITE_LINE_MEMBER(turbo_state::coin_meter_2_w)
        void coin_meter_2_w(int state)
        {
            throw new emu_unimplemented();
#if false
            machine().bookkeeping().coin_counter_w(1, state);
#endif
        }


        //WRITE_LINE_MEMBER(turbo_state::start_lamp_w)
        void start_lamp_w(int state)
        {
            throw new emu_unimplemented();
#if false
            m_lamp = state ? 1 : 0;
#endif
        }
    }


    //uint8_t buckrog_state::subcpu_command_r()

    //uint8_t buckrog_state::port_2_r()

    //uint8_t buckrog_state::port_3_r()

    //TIMER_CALLBACK_MEMBER(buckrog_state::delayed_i8255_w)

    //void buckrog_state::i8255_0_w(offs_t offset, uint8_t data)


    partial class turbo_state : turbo_base_state
    {
#if false
        uint8_t turbo_state::spriteram_r(offs_t offset)
        {
            offset = (offset & 0x07) | ((offset & 0xf0) >> 1);
            return m_alt_spriteram[offset];
        }

        void turbo_state::spriteram_w(offs_t offset, uint8_t data)
        {
            offset = (offset & 0x07) | ((offset & 0xf0) >> 1);
            m_alt_spriteram[offset] = data;
        }
#endif

        /*************************************
         *  Turbo CPU memory handlers
         *************************************/
        void prg_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map(0x0000, 0x5fff).rom();
            map(0xa000, 0xa0ff).mirror(0x0700).rw(FUNC(turbo_state::spriteram_r), FUNC(turbo_state::spriteram_w));
            map(0xa800, 0xa807).mirror(0x07f8).w("outlatch", FUNC(ls259_device::write_d0));
            map(0xb000, 0xb3ff).mirror(0x0400).ram().share(m_sprite_position);
            map(0xb800, 0xbfff).w(FUNC(turbo_state::analog_reset_w));
            map(0xe000, 0xe7ff).ram().w(FUNC(turbo_state::videoram_w)).share(m_videoram);
            map(0xe800, 0xefff).w(FUNC(turbo_state::collision_clear_w));
            map(0xf000, 0xf7ff).ram();
            map(0xf800, 0xf803).mirror(0x00fc).rw(m_i8255[0], FUNC(i8255_device::read), FUNC(i8255_device::write));
            map(0xf900, 0xf903).mirror(0x00fc).rw(m_i8255[1], FUNC(i8255_device::read), FUNC(i8255_device::write));
            map(0xfa00, 0xfa03).mirror(0x00fc).rw(m_i8255[2], FUNC(i8255_device::read), FUNC(i8255_device::write));
            map(0xfb00, 0xfb03).mirror(0x00fc).rw(m_i8255[3], FUNC(i8255_device::read), FUNC(i8255_device::write));
            map(0xfc00, 0xfc01).mirror(0x00fe).rw("i8279", FUNC(i8279_device::read), FUNC(i8279_device::write));
            map(0xfd00, 0xfdff).portr("INPUT");
            map(0xfe00, 0xfeff).r(FUNC(turbo_state::collision_r));
#endif
        }
    }


    //void subroc3d_state::prg_map(address_map &map)

    //void buckrog_state::main_prg_map(address_map &map)

    //void buckrog_state::decrypted_opcodes_map(address_map &map)

    //void buckrog_state::sub_prg_map(address_map &map)

    //void buckrog_state::sub_portmap(address_map &map)


    partial class turbo : construct_ioport_helper
    {
        /*************************************
         *  Port definitions
         *************************************/

        //static INPUT_PORTS_START( turbo )
        void construct_ioport_turbo(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            throw new emu_unimplemented();
#if false
            INPUT_PORTS_START(owner, portlist, ref errorbuf);
            PORT_START("INPUT") // IN0
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_BUTTON2 )                // ACCEL B
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON1 )                // ACCEL A
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_BUTTON3 ) PORT_TOGGLE    // SHIFT
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_START1 )
            PORT_SERVICE_NO_TOGGLE( 0x10, IP_ACTIVE_LOW )
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_SERVICE1 )
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_COIN2 )
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_COIN1 )

            PORT_START("DSW1")  // DSW1
            PORT_DIPNAME( 0x03, 0x03, "Car On Extended Play" )  PORT_DIPLOCATION("SW1:1,2")
            PORT_DIPSETTING(    0x00, "1" )
            PORT_DIPSETTING(    0x01, "2" )
            PORT_DIPSETTING(    0x02, "3" )
            PORT_DIPSETTING(    0x03, "4" )
            PORT_DIPNAME( 0x04, 0x00, DEF_STR( Game_Time ) )    PORT_DIPLOCATION("SW1:3")
            PORT_DIPSETTING(    0x04, "Fixed (55 sec)" )
            PORT_DIPSETTING(    0x00, "Adjustable" )
            PORT_DIPNAME( 0x08, 0x08, DEF_STR( Difficulty ) )   PORT_DIPLOCATION("SW1:4")
            PORT_DIPSETTING(    0x08, DEF_STR( Easy ))
            PORT_DIPSETTING(    0x00, DEF_STR( Hard ))
            PORT_DIPNAME( 0x10, 0x10, "Game Mode" )             PORT_DIPLOCATION("SW1:5")
            PORT_DIPSETTING(    0x00, "No Collisions (cheat)" )
            PORT_DIPSETTING(    0x10, DEF_STR( Normal ) )
            PORT_DIPNAME( 0x20, 0x20, "Initial Entry" )         PORT_DIPLOCATION("SW1:6")
            PORT_DIPSETTING(    0x00, DEF_STR( Off ))
            PORT_DIPSETTING(    0x20, DEF_STR( On ))
            PORT_DIPNAME( 0x40, 0x40, DEF_STR( Unknown ) )      PORT_DIPLOCATION("SW1:7")
            PORT_DIPSETTING(    0x40, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x00, DEF_STR( On ) )
            PORT_DIPNAME( 0x80, 0x80, DEF_STR( Unknown ) )      PORT_DIPLOCATION("SW1:8")
            PORT_DIPSETTING(    0x80, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x00, DEF_STR( On ) )

            PORT_START("DSW2")  // DSW2
            PORT_DIPNAME( 0x03, 0x03, DEF_STR( Game_Time ) )    PORT_DIPLOCATION("SW2:1,2")
            PORT_DIPSETTING(    0x00, "60 seconds" )
            PORT_DIPSETTING(    0x01, "70 seconds" )
            PORT_DIPSETTING(    0x02, "80 seconds" )
            PORT_DIPSETTING(    0x03, "90 seconds" )
            PORT_DIPNAME( 0x1c, 0x1c, DEF_STR( Coin_B ))        PORT_DIPLOCATION("SW2:3,4,5")
            PORT_DIPSETTING(    0x18, DEF_STR( 4C_1C ))
            PORT_DIPSETTING(    0x14, DEF_STR( 3C_1C ))
            PORT_DIPSETTING(    0x10, DEF_STR( 2C_1C ))
            PORT_DIPSETTING(    0x00, DEF_STR( 1C_1C ))
            PORT_DIPSETTING(    0x1c, DEF_STR( 1C_1C ))
            PORT_DIPSETTING(    0x04, DEF_STR( 1C_2C ))
            PORT_DIPSETTING(    0x08, DEF_STR( 1C_3C ))
            PORT_DIPSETTING(    0x0c, DEF_STR( 1C_6C ))
            PORT_DIPNAME( 0xe0, 0xe0, DEF_STR( Coin_A ))        PORT_DIPLOCATION("SW2:6,7,8")
            PORT_DIPSETTING(    0xc0, DEF_STR( 4C_1C ))
            PORT_DIPSETTING(    0xa0, DEF_STR( 3C_1C ))
            PORT_DIPSETTING(    0x80, DEF_STR( 2C_1C ))
            PORT_DIPSETTING(    0x00, DEF_STR( 1C_1C ))
            PORT_DIPSETTING(    0xe0, DEF_STR( 1C_1C ))
            PORT_DIPSETTING(    0x20, DEF_STR( 1C_2C ))
            PORT_DIPSETTING(    0x40, DEF_STR( 1C_3C ))
            PORT_DIPSETTING(    0x60, DEF_STR( 1C_6C ))

            PORT_START("DSW3")  // Collision and DSW 3
            PORT_BIT( 0x0f,     0x00, IPT_CUSTOM ) // Merged with collision bits
            PORT_DIPNAME( 0x10, 0x10, DEF_STR( Unknown ) )      PORT_DIPLOCATION("SW3:1")
            PORT_DIPSETTING(    0x10, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x00, DEF_STR( On ) )
            PORT_DIPNAME( 0x20, 0x20, DEF_STR( Unknown ) )      PORT_DIPLOCATION("SW3:2")
            PORT_DIPSETTING(    0x20, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x00, DEF_STR( On ) )
            PORT_DIPNAME( 0x40, 0x40, "Tachometer" )            PORT_DIPLOCATION("SW3:3")
            PORT_DIPSETTING(    0x40, "Analog (Meter)")
            PORT_DIPSETTING(    0x00, "Digital (LED)")
            PORT_DIPNAME( 0x80, 0x80, "Sound System" )          PORT_DIPLOCATION("SW3:4")
            PORT_DIPSETTING(    0x80, DEF_STR( Upright ) )
            PORT_DIPSETTING(    0x00, "Cockpit")

            PORT_START("DIAL")  // IN0
            PORT_BIT( 0xff, 0, IPT_DIAL ) PORT_SENSITIVITY(10) PORT_KEYDELTA(30)

            // this is actually a variable resistor
            PORT_START("VR1")
            PORT_ADJUSTER(31, "Sprite scale offset")

            // this is actually a variable resistor
            PORT_START("VR2")
            PORT_ADJUSTER(91, "Sprite scale gain")

            INPUT_PORTS_END
#endif
        }


        //static INPUT_PORTS_START( subroc3d )

        //static INPUT_PORTS_START( buckrog )
    }


    partial class turbo_state : turbo_base_state
    {
        /*************************************
         *  Graphics definitions
         *************************************/
        //static GFXDECODE_START( gfx_turbo )
        static readonly gfx_decode_entry [] gfx_turbo =
        {
            GFXDECODE_ENTRY( "fgtiles", 0, gfx_8x8x2_planar, 0, 64 )

            //GFXDECODE_END
        };


        /*************************************
         *  Machine drivers
         *************************************/
        public void turbo(machine_config config)
        {
            // basic machine hardware
            Z80(config, m_maincpu, MASTER_CLOCK / 4);
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, prg_map);
            m_maincpu.op0.execute().set_vblank_int("screen", irq0_line_hold);

            I8255(config, m_i8255[0]);
            m_i8255[0].op0.out_pa_callback().set(ppi0a_w).reg();
            m_i8255[0].op0.out_pb_callback().set(ppi0b_w).reg();
            m_i8255[0].op0.out_pc_callback().set(ppi0c_w).reg();

            I8255(config, m_i8255[1]);
            m_i8255[1].op0.out_pa_callback().set(ppi1a_w).reg();
            m_i8255[1].op0.out_pb_callback().set(ppi1b_w).reg();
            m_i8255[1].op0.out_pc_callback().set(ppi1c_w).reg();

            I8255(config, m_i8255[2]);
            m_i8255[2].op0.out_pa_callback().set(sound_a_w).reg();
            m_i8255[2].op0.out_pb_callback().set(sound_b_w).reg();
            m_i8255[2].op0.out_pc_callback().set(sound_c_w).reg();

            I8255(config, m_i8255[3]);
            m_i8255[3].op0.in_pa_callback().set(analog_r).reg();
            m_i8255[3].op0.in_pb_callback().set_ioport("DSW2").reg();
            m_i8255[3].op0.out_pc_callback().set(ppi3c_w).reg();

            i8279_device kbdc = I8279(config, "i8279", MASTER_CLOCK / 16); // clock = H1
            kbdc.out_sl_callback().set(scanlines_w).reg(); // scan SL lines
            kbdc.out_disp_callback().set(digit_w).reg();   // display A&B
            kbdc.in_rl_callback().set_ioport("DSW1").reg();                   // kbd RL lines

            ls259_device outlatch = LS259(config, "outlatch"); // IC125 - outputs passed through CN5
            outlatch.q_out_cb<u32_const_0>().set((write_line_delegate)coin_meter_1_w).reg();
            outlatch.q_out_cb<u32_const_1>().set((write_line_delegate)coin_meter_2_w).reg();
            outlatch.q_out_cb<u32_const_3>().set((write_line_delegate)start_lamp_w).reg();

            // video hardware
            GFXDECODE(config, m_gfxdecode, "palette", gfx_turbo);
            PALETTE(config, "palette", palette, 256);

            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_video_attributes(VIDEO_ALWAYS_UPDATE);
            m_screen.op0.set_raw(PIXEL_CLOCK, HTOTAL, HBEND, HBSTART, VTOTAL, VBEND, VBSTART);
            m_screen.op0.set_screen_update(screen_update);
            m_screen.op0.set_palette("palette");

            // sound hardware
            turbo_samples(config);
        }
    }


    //void subroc3d_state::subroc3d(machine_config &config)

    //void buckrog_state::buckrog(machine_config &config)

    //void buckrog_state::buckrogu(machine_config &config)

    //void buckrog_state::buckroge(machine_config &config)


    partial class turbo : construct_ioport_helper
    {
        /*************************************
         *  ROM definitions
         *************************************/

        //ROM_START( turbo )
        static readonly tiny_rom_entry [] rom_turbo =
        {
            ROM_REGION( 0x6000, "maincpu", 0 ),
            ROM_LOAD( "epr-1513.cpu-ic76",  0x0000, 0x2000, CRC("0326adfc") + SHA1("d9f06f0bc78667fa58c4b8ab3a3897d0dd0bdfbf") ),
            ROM_LOAD( "epr-1514.cpu-ic89",  0x2000, 0x2000, CRC("25af63b0") + SHA1("9af4b3da83a4cef79b7dd0e9061132c499872c1c") ),
            ROM_LOAD( "epr-1515.cpu-ic103", 0x4000, 0x2000, CRC("059c1c36") + SHA1("ba870e6f45ff15aa148b2c2f213c879144aaacf0") ),

            ROM_REGION( 0x20000, "sprites", 0 ),
            ROM_LOAD( "epr-1246.prom-ic84", 0x00000, 0x2000, CRC("555bfe9a") + SHA1("1e56385475eeff044dcd9b44a154991d3efe995e") ), // level 0
            ROM_RELOAD(                     0x02000, 0x2000 ),
            ROM_LOAD( "epr-1247.prom-ic86", 0x04000, 0x2000, CRC("c8c5e4d5") + SHA1("da70297340ddea0cd7fe04f2d94ea65f8202d0e5") ), // level 1
            ROM_RELOAD(                     0x06000, 0x2000 ),
            ROM_LOAD( "epr-1248.prom-ic88", 0x08000, 0x2000, CRC("82fe5b94") + SHA1("b96688ca0cfd90fdc4ee7c2e6c0b66726cc5713c") ), // level 2
            ROM_RELOAD(                     0x0a000, 0x2000 ),
            ROM_LOAD( "epr-1249.prom-ic90", 0x0c000, 0x2000, CRC("e258e009") + SHA1("598d382db0f789ea2fde749b7467abed545de25a") ), // level 3
            ROM_LOAD( "epr-1250.prom-ic108",0x0e000, 0x2000, CRC("aee6e05e") + SHA1("99b9b1ec996746ddf713ed38192f350f1f32a847") ),
            ROM_LOAD( "epr-1251.prom-ic92", 0x10000, 0x2000, CRC("292573de") + SHA1("3ddc980d11478a6a6e4082c2f76c1ab82ffe2f36") ), // level 4
            ROM_LOAD( "epr-1252.prom-ic110",0x12000, 0x2000, CRC("aee6e05e") + SHA1("99b9b1ec996746ddf713ed38192f350f1f32a847") ),
            ROM_LOAD( "epr-1253.prom-ic94", 0x14000, 0x2000, CRC("92783626") + SHA1("13979eb964112436182d2a92f21803bcc28f4a4a") ), // level 5
            ROM_LOAD( "epr-1254.prom-ic112",0x16000, 0x2000, CRC("aee6e05e") + SHA1("99b9b1ec996746ddf713ed38192f350f1f32a847") ),
            ROM_LOAD( "epr-1255.prom-ic32", 0x18000, 0x2000, CRC("485dcef9") + SHA1("0f760ebb42cc2580a29758c72428a41d74477ce6") ), // level 6
            ROM_LOAD( "epr-1256.prom-ic47", 0x1a000, 0x2000, CRC("aee6e05e") + SHA1("99b9b1ec996746ddf713ed38192f350f1f32a847") ),
            ROM_LOAD( "epr-1257.prom-ic34", 0x1c000, 0x2000, CRC("4ca984ce") + SHA1("99f294fb203f23929b44baa2dd1825c67dde08a1") ), // level 7
            ROM_LOAD( "epr-1258.prom-ic49", 0x1e000, 0x2000, CRC("aee6e05e") + SHA1("99b9b1ec996746ddf713ed38192f350f1f32a847") ),

            ROM_REGION( 0x1000, "fgtiles", 0 ),
            ROM_LOAD( "epr-1244.cpu-ic111", 0x0000, 0x0800, CRC("17f67424") + SHA1("6126562510f1509f3487faaa3b9d7470ab600a2c") ),
            ROM_LOAD( "epr-1245.cpu-ic122", 0x0800, 0x0800, CRC("2ba0b46b") + SHA1("5d4d4f19ad7a911c7b37db190a420faf665546b4") ),

            ROM_REGION( 0x4800, "road", 0 ),
            ROM_LOAD( "epr-1125.cpu-ic1",   0x0000, 0x0800, CRC("65b5d44b") + SHA1("bbdd5db013c9d876e9666f17c48569c7531bfc08") ),
            ROM_LOAD( "epr-1126.cpu-ic2",   0x0800, 0x0800, CRC("685ace1b") + SHA1("99c8d36ac910169b27676d18c894433c2ba44853") ),
            ROM_LOAD( "epr-1127.cpu-ic13",  0x1000, 0x0800, CRC("9233c9ca") + SHA1("cbf9a0f564d8ace1ccd701c1769dbc001d465851") ),
            ROM_LOAD( "epr-1238.cpu-ic14",  0x1800, 0x0800, CRC("d94fd83f") + SHA1("1e3a68259d2ede623d5a7306fdf693a4eab301f0") ),
            ROM_LOAD( "epr-1239.cpu-ic27",  0x2000, 0x0800, CRC("4c41124f") + SHA1("d73a9441552c77fb3078553195794311a950d589") ),
            ROM_LOAD( "epr-1240.cpu-ic28",  0x2800, 0x0800, CRC("371d6282") + SHA1("f5902b357d976822d46aa6404b7bd30855d435a9") ),
            ROM_LOAD( "epr-1241.cpu-ic41",  0x3000, 0x0800, CRC("1109358a") + SHA1("27a5351a4e87309671e72115299420315a93dba6") ),
            ROM_LOAD( "epr-1242.cpu-ic42",  0x3800, 0x0800, CRC("04866769") + SHA1("1f9c0d53766fdaf8de57d3df05f291c2ca3dc5fb") ),
            ROM_LOAD( "epr-1243.cpu-ic74",  0x4000, 0x0800, CRC("29854c48") + SHA1("cab89bc30f83d9746931ddf6f95a6d0c8a517e5d") ),

            ROM_REGION( 0x1020, "proms", 0 ),
            ROM_LOAD( "pr-1114.prom-ic13",  0x0000, 0x0020, CRC("78aded46") + SHA1("c78afe804f8b8e837b0c502de5b8715a41fb92b9") ),  // road red/green color table
            ROM_LOAD( "pr-1115.prom-ic18",  0x0020, 0x0020, CRC("5394092c") + SHA1("129ff61104979ff6a3c3af8bf81c04ae9b133c9e") ),  // road collision/enable
            ROM_LOAD( "pr-1116.prom-ic20",  0x0040, 0x0020, CRC("3956767d") + SHA1("073aaf57175526660fcf7af2e16e7f1d1aaba9a9") ),  // collision detection
            ROM_LOAD( "pr-1117.prom-ic21",  0x0060, 0x0020, CRC("f06d9907") + SHA1("f11db7800f41b03e79f5eef8d7ef3ae0a6277518") ),  // road green/blue color table
            ROM_LOAD( "pr-1118.cpu-ic99",   0x0100, 0x0100, CRC("07324cfd") + SHA1("844abc2042d6810fa34d84ff1ed57744886c6ea6") ),  // background color table
            ROM_LOAD( "pr-1119.cpu-ic50",   0x0200, 0x0200, CRC("57ebd4bc") + SHA1("932649da3537666f95833a8a8aff506217bd9aa1") ),  // sprite Y scaling
            ROM_LOAD( "pr-1120.cpu-ic62",   0x0400, 0x0200, CRC("8dd4c8a8") + SHA1("e8d9cf08f115d57c44746fa0ff28f47b064b4193") ),  // video timing
            ROM_LOAD( "pr-1121.prom-ic29",  0x0600, 0x0200, CRC("7692f497") + SHA1("42468c0705df9928e15ff8deb7e793a6c0c04353") ),  // palette
            ROM_LOAD( "pr-1122.prom-ic11",  0x0800, 0x0400, CRC("1a86ce70") + SHA1("cab708b9a089b2e28f2298c1e4fae6e200923527") ),  // sprite priorities
            ROM_LOAD( "pr-1123.prom-ic12",  0x0c00, 0x0400, CRC("02d2cb52") + SHA1("c34d6b60355747ce20fcb8d322df0e188d187f10") ),  // sprite/road/background priorities
            ROM_LOAD( "pr-1279.sound-ic40", 0x1000, 0x0020, CRC("b369a6ae") + SHA1("dda7c6cf58ce5173f29a3084c85393c0c4587086") ),  // sound board PROM

            ROM_END,
        };


        //ROM_START( turboa )

        //ROM_START( turbob )

        //ROM_START( turboc )

        //ROM_START( turbod )

        //ROM_START( turboe )

        //ROM_START( turbobl )

        //ROM_START( subroc3d )

        //ROM_START( buckrog ) // CPU BOARD Sega ID#  834-5158-01, ROM BOARD Sega ID# 834-5152-01

        //ROM_START( buckrogn )

        //ROM_START( buckrogn2 )

        //ROM_START( zoom909 )
    }


#if false
    /*************************************
     *  Turbo ROM decoding
     *************************************/

    void turbo_state::rom_decode()
    {
        /*
         * The table is arranged this way (second half is mirror image of first)
         *
         *      0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F
         *
         * 0   00 00 00 00 01 01 01 01 02 02 02 02 03 03 03 03
         * 1   04 04 04 04 05 05 05 05 06 06 06 06 07 07 07 07
         * 2   08 08 08 08 09 09 09 09 0A 0A 0A 0A 0B 0B 0B 0B
         * 3   0C 0C 0C 0C 0D 0D 0D 0D 0E 0E 0E 0E 0F 0F 0F 0F
         * 4   10 10 10 10 11 11 11 11 12 12 12 12 13 13 13 13
         * 5   14 14 14 14 15 15 15 15 16 16 16 16 17 17 17 17
         * 6   18 18 18 18 19 19 19 19 1A 1A 1A 1A 1B 1B 1B 1B
         * 7   1C 1C 1C 1C 1D 1D 1D 1D 1E 1E 1E 1E 1F 1F 1F 1F
         * 8   1F 1F 1F 1F 1E 1E 1E 1E 1D 1D 1D 1D 1C 1C 1C 1C
         * 9   1B 1B 1B 1B 1A 1A 1A 1A 19 19 19 19 18 18 18 18
         * A   17 17 17 17 16 16 16 16 15 15 15 15 14 14 14 14
         * B   13 13 13 13 12 12 12 12 11 11 11 11 10 10 10 10
         * C   0F 0F 0F 0F 0E 0E 0E 0E 0D 0D 0D 0D 0C 0C 0C 0C
         * D   0B 0B 0B 0B 0A 0A 0A 0A 09 09 09 09 08 08 08 08
         * E   07 07 07 07 06 06 06 06 05 05 05 05 04 04 04 04
         * F   03 03 03 03 02 02 02 02 01 01 01 01 00 00 00 00
         *
         */
        static const uint8_t xortable[][32]=
        {
            // Table 0
            // 0x0000-0x3ff
            // 0x0800-0xbff
            // 0x4000-0x43ff
            // 0x4800-0x4bff
            { 0x00,0x44,0x0c,0x48,0x00,0x44,0x0c,0x48,
                0xa0,0xe4,0xac,0xe8,0xa0,0xe4,0xac,0xe8,
                0x60,0x24,0x6c,0x28,0x60,0x24,0x6c,0x28,
                0xc0,0x84,0xcc,0x88,0xc0,0x84,0xcc,0x88 },

            // Table 1 */
            // 0x0400-0x07ff
            // 0x0c00-0x0fff
            // 0x1400-0x17ff
            // 0x1c00-0x1fff
            // 0x2400-0x27ff
            // 0x2c00-0x2fff
            // 0x3400-0x37ff
            // 0x3c00-0x3fff
            // 0x4400-0x47ff
            // 0x4c00-0x4fff
            // 0x5400-0x57ff
            // 0x5c00-0x5fff
            { 0x00,0x44,0x18,0x5c,0x14,0x50,0x0c,0x48,
                0x28,0x6c,0x30,0x74,0x3c,0x78,0x24,0x60,
                0x60,0x24,0x78,0x3c,0x74,0x30,0x6c,0x28,
                0x48,0x0c,0x50,0x14,0x5c,0x18,0x44,0x00 }, //0x00 --> 0x10 ?

            // Table 2 */
            // 0x1000-0x13ff
            // 0x1800-0x1bff
            // 0x5000-0x53ff
            // 0x5800-0x5bff
            { 0x00,0x00,0x28,0x28,0x90,0x90,0xb8,0xb8,
                0x28,0x28,0x00,0x00,0xb8,0xb8,0x90,0x90,
                0x00,0x00,0x28,0x28,0x90,0x90,0xb8,0xb8,
                0x28,0x28,0x00,0x00,0xb8,0xb8,0x90,0x90 },

            // Table 3 */
            // 0x2000-0x23ff
            // 0x2800-0x2bff
            // 0x3000-0x33ff
            // 0x3800-0x3bff
            { 0x00,0x14,0x88,0x9c,0x30,0x24,0xb8,0xac,
                0x24,0x30,0xac,0xb8,0x14,0x00,0x9c,0x88,
                0x48,0x5c,0xc0,0xd4,0x78,0x6c,0xf0,0xe4,
                0x6c,0x78,0xe4,0xf0,0x5c,0x48,0xd4,0xc0 }
        };

        static const int findtable[]=
        {
            0,1,0,1, // 0x0000-0x0fff
            2,1,2,1, // 0x1000-0x1fff
            3,1,3,1, // 0x2000-0x2fff
            3,1,3,1, // 0x3000-0x3fff
            0,1,0,1, // 0x4000-0x4fff
            2,1,2,1  // 0x5000-0x5fff
        };

        uint8_t *rom = memregion("maincpu")->base();

        for (int offs = 0x0000; offs < 0x6000; offs++)
        {
            uint8_t src = rom [offs];
            int i = findtable[offs >> 10];
            int j = src >> 2;
            if (src & 0x80) j ^= 0x3f;
            rom[offs] = src ^ xortable[i][j];
        }
    }


    /*************************************
     *  Driver init
     *************************************/
    void turbo_state::init_turbo_enc()
    {
        rom_decode();
    }
#endif


    public partial class turbo : construct_ioport_helper
    {
        /*************************************
         *  Game drivers
         *************************************/

        static void turbo_state_turbo(machine_config config, device_t device) { turbo_state turbo_state = (turbo_state)device; turbo_state.turbo(config); }

        static turbo m_turbo = new turbo();

        static device_t device_creator_turbo(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new turbo_state(mconfig, type, tag); }

        public static readonly game_driver driver_turbo = GAMEL(device_creator_turbo, rom_turbo, "1981", "turbo", "0", turbo_state_turbo, m_turbo.construct_ioport_turbo, driver_device.empty_init, ROT270, "Sega", "Turbo (program 1513-1515)", MACHINE_IMPERFECT_SOUND, null /*layout_turbo*/);
    }
}
