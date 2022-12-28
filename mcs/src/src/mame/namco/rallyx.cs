// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u32 = System.UInt32;
using uint8_t = System.Byte;

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
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.namco_global;
using static mame.romentry_global;
using static mame.samples_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.watchdog_global;
using static mame.z80_global;


namespace mame
{
    partial class rallyx_state : driver_device
    {
        static readonly XTAL MASTER_CLOCK = XTAL_global.op("18.432_MHz_XTAL");


        /*************************************
         *
         *  Memory handlers
         *
         *************************************/

        void rallyx_interrupt_vector_w(uint8_t data)
        {
            m_interrupt_vector = data;
        }


        //IRQ_CALLBACK_MEMBER(rallyx_state::interrupt_vector_r)
        int interrupt_vector_r(device_t device, int irqline)
        {
            return m_interrupt_vector;
        }


        void bang_w(int state)
        {
            if (state == 0 && m_last_bang)
                m_samples.op0.start(0, 0);

            m_last_bang = state != 0;
        }


        void irq_mask_w(int state)
        {
            m_main_irq_mask = state != 0;
            if (state == 0)
                m_maincpu.op0.set_input_line(0, CLEAR_LINE);
        }


        //void rallyx_state::nmi_mask_w(int state)


        void sound_on_w(int state)
        {
            // this doesn't work in New Rally X so I'm not supporting it
            //m_namco_sound->pacman_sound_enable_w(state);
        }


        void flip_screen_w(int state)
        {
            flip_screen_set(state);
        }


        void coin_lockout_w(int state)
        {
            machine().bookkeeping().coin_lockout_w(0, state == 0 ? 1 : 0);
        }


        void coin_counter_1_w(int state)
        {
            machine().bookkeeping().coin_counter_w(0, state);
        }


        //void rallyx_state::coin_counter_2_w(int state)


        /*************************************
         *
         *  Address maps
         *
         *************************************/

        void rallyx_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x3fff).rom();
            map.op(0x8000, 0x8fff).ram().w(videoram_w).share(m_videoram);
            map.op(0x9800, 0x9fff).ram();
            map.op(0xa000, 0xa000).portr("P1");
            map.op(0xa080, 0xa080).portr("P2");
            map.op(0xa100, 0xa100).portr("DSW");
            map.op(0xa000, 0xa00f).writeonly().share(m_radarattr);
            map.op(0xa080, 0xa080).w("watchdog", (data) => { ((watchdog_timer_device)subdevice("watchdog")).reset_w(data); });
            map.op(0xa100, 0xa11f).w(m_namco_sound, (offset, data) => { m_namco_sound.op0.pacman_sound_w(offset, data); });
            map.op(0xa130, 0xa130).w(scrollx_w);
            map.op(0xa140, 0xa140).w(scrolly_w);
            map.op(0xa170, 0xa170).nopw();            // ?
            map.op(0xa180, 0xa187).w("mainlatch", (offset, data) => { ((ls259_device)subdevice("mainlatch")).write_d0(offset, data); });
        }


        void io_map(address_map map, device_t device)
        {
            map.global_mask(0xff);
            map.op(0, 0).w(rallyx_interrupt_vector_w);
        }


        //void rallyx_state::jungler_map(address_map &map)
    }


    public partial class rallyx : construct_ioport_helper
    {
        /*************************************
         *
         *  Input ports
         *
         *************************************/
        //static INPUT_PORTS_START( rallyx )
        void construct_ioport_rallyx(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("P1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_SERVICE1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_4WAY();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_4WAY();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_4WAY();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_4WAY();
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_COIN1 );

            PORT_START("P2");
            PORT_DIPNAME( 0x01, 0x01, DEF_STR( Cabinet ) );      PORT_DIPLOCATION("P2:1");
            PORT_DIPSETTING(    0x01, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Cocktail ) );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_COIN2 );

            PORT_START("DSW");
            PORT_DIPNAME( 0xc0, 0xc0, DEF_STR( Coinage ) );      PORT_DIPLOCATION("DSW:7,8");
            PORT_DIPSETTING(    0x40, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0xc0, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x80, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Free_Play ) );
            PORT_DIPNAME( 0x38, 0x08, DEF_STR( Difficulty ) );   PORT_DIPLOCATION("DSW:4,5,6");
            PORT_DIPSETTING(    0x10, "1 Car, Medium" );
            PORT_DIPSETTING(    0x28, "1 Car, Hard" );
            PORT_DIPSETTING(    0x00, "2 Cars, Easy" );
            PORT_DIPSETTING(    0x18, "2 Cars, Medium" );
            PORT_DIPSETTING(    0x30, "2 Cars, Hard" );
            PORT_DIPSETTING(    0x08, "3 Cars, Easy" );
            PORT_DIPSETTING(    0x20, "3 Cars, Medium" );
            PORT_DIPSETTING(    0x38, "3 Cars, Hard" );
            PORT_DIPNAME( 0x06, 0x02, DEF_STR( Bonus_Life ) );   PORT_DIPLOCATION("DSW:2,3");
            PORT_DIPSETTING(    0x02, "15000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x00);
            PORT_DIPSETTING(    0x04, "30000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x00);
            PORT_DIPSETTING(    0x06, "40000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x00);

            PORT_DIPSETTING(    0x02, "20000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x08);
            PORT_DIPSETTING(    0x04, "40000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x08);
            PORT_DIPSETTING(    0x06, "60000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x08);

            PORT_DIPSETTING(    0x02, "10000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x10);
            PORT_DIPSETTING(    0x04, "20000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x10);
            PORT_DIPSETTING(    0x06, "30000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x10);

            PORT_DIPSETTING(    0x02, "15000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x18);
            PORT_DIPSETTING(    0x04, "30000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x18);
            PORT_DIPSETTING(    0x06, "40000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x18);

            PORT_DIPSETTING(    0x02, "20000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x20);
            PORT_DIPSETTING(    0x04, "40000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x20);
            PORT_DIPSETTING(    0x06, "60000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x20);

            PORT_DIPSETTING(    0x02, "10000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x28);
            PORT_DIPSETTING(    0x04, "20000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x28);
            PORT_DIPSETTING(    0x06, "30000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x28);

            PORT_DIPSETTING(    0x02, "15000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x30);
            PORT_DIPSETTING(    0x04, "30000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x30);
            PORT_DIPSETTING(    0x06, "40000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x30);

            PORT_DIPSETTING(    0x02, "20000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x38);
            PORT_DIPSETTING(    0x04, "40000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x38);
            PORT_DIPSETTING(    0x06, "60000" );     PORT_CONDITION("DSW", 0x38, ioport_condition.condition_t.EQUALS, 0x38);
            PORT_DIPSETTING(    0x00, DEF_STR( None ) );
            PORT_SERVICE_DIPLOC( 0x01, 0x01, "DSW:1");

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( dngrtrck )

        //static INPUT_PORTS_START( nrallyx )

        //static INPUT_PORTS_START( jungler )

        //static INPUT_PORTS_START( locomotn )

        //static INPUT_PORTS_START( tactcian )

        //static INPUT_PORTS_START( commsega )
    }


    partial class rallyx_state : driver_device
    {
        /*************************************
         *
         *  Graphics definitions
         *
         *************************************/

        static readonly gfx_layout rallyx_charlayout = new gfx_layout(
            8,8,
            RGN_FRAC(1,1),
            2,
            new u32[] { 0, 4 },
            new u32[] { 8*8+0, 8*8+1, 8*8+2, 8*8+3, 0, 1, 2, 3 },
            new u32[] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
            16*8
        );


        //static const gfx_layout jungler_charlayout =


        static readonly gfx_layout rallyx_spritelayout = new gfx_layout(
            16,16,
            RGN_FRAC(1,1),
            2,
            new u32[] { 0, 4 },
            new u32[] { 8*8+0, 8*8+1, 8*8+2, 8*8+3, 16*8+0, 16*8+1, 16*8+2, 16*8+3,
                        24*8+0, 24*8+1, 24*8+2, 24*8+3, 0, 1, 2, 3 },
            new u32[] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
                    32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64*8
        );


        //static const gfx_layout jungler_spritelayout =


        static readonly gfx_layout dotlayout = new gfx_layout(
            4,4,
            8,
            2,
            new u32[] { 6, 7 },
            new u32[] { 0*8, 1*8, 2*8, 3*8 },
            new u32[] { 0*32, 1*32, 2*32, 3*32 },
            16*8
        );


        //static GFXDECODE_START( gfx_rallyx )
        static readonly gfx_decode_entry [] gfx_rallyx =
        {
            GFXDECODE_ENTRY( "gfx1", 0, rallyx_charlayout,     0, 64 ),
            GFXDECODE_ENTRY( "gfx1", 0, rallyx_spritelayout,   0, 64 ),
            GFXDECODE_ENTRY( "gfx2", 0, dotlayout,         64 * 4,  1 ),

            //GFXDECODE_END
        };


        //static GFXDECODE_START( gfx_jungler )


        /*************************************
         *
         *  Sound interfaces
         *
         *************************************/

        static readonly string [] rallyx_sample_names =
        {
            "*rallyx",
            "bang",
            null   // end of array
        };


        /*************************************
         *
         *  Machine driver
         *
         *************************************/

        protected override void machine_start()
        {
            m_interrupt_vector = 0;

            save_item(NAME(new { m_last_bang }));
            save_item(NAME(new { m_stars_enable }));
            save_item(NAME(new { m_main_irq_mask }));
            save_item(NAME(new { m_interrupt_vector }));
        }


        void rallyx_vblank_irq(int state)
        {
            if (state != 0 && m_main_irq_mask)
                m_maincpu.op0.set_input_line(0, ASSERT_LINE);
        }


        //void rallyx_state::jungler_vblank_irq(int state)


        public void rallyx(machine_config config)
        {
            // basic machine hardware
            Z80(config, m_maincpu, MASTER_CLOCK / 6);    // 3.072 MHz
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, rallyx_map);
            m_maincpu.op0.memory().set_addrmap(AS_IO, io_map);
            m_maincpu.op0.execute().set_irq_acknowledge_callback(interrupt_vector_r);

            ls259_device mainlatch = LS259(config, "mainlatch"); // 259 at 12M or 4099 at 11M on Logic Board I
            mainlatch.q_out_cb<u32_const_0>().set((write_line_delegate)bang_w).reg(); // BANG
            mainlatch.q_out_cb<u32_const_1>().set((write_line_delegate)irq_mask_w).reg(); // INT ON
            mainlatch.q_out_cb<u32_const_2>().set((write_line_delegate)sound_on_w).reg(); // SOUND ON
            mainlatch.q_out_cb<u32_const_3>().set((write_line_delegate)flip_screen_w).reg(); // FLIP
            mainlatch.q_out_cb<u32_const_4>().set_output("led0").reg();
            mainlatch.q_out_cb<u32_const_5>().set_output("led1").reg();
            mainlatch.q_out_cb<u32_const_6>().set((write_line_delegate)coin_lockout_w).reg();
            mainlatch.q_out_cb<u32_const_7>().set((write_line_delegate)coin_counter_1_w).reg();

            WATCHDOG_TIMER(config, "watchdog");

            // video hardware
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_raw(MASTER_CLOCK / 3, 48 * 8, 0 * 8, 36 * 8, 33 * 8, 2 * 8, 30 * 8);
            m_screen.op0.set_screen_update(screen_update_rallyx);
            m_screen.op0.set_palette(m_palette);
            m_screen.op0.screen_vblank().set((write_line_delegate)rallyx_vblank_irq).reg();

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_rallyx);

            PALETTE(config, m_palette, rallyx_palette, 64 * 4 + 4, 32);
            m_palette.op0.enable_shadows();

            MCFG_VIDEO_START_OVERRIDE(config, video_start_rallyx);

            // sound hardware
            SPEAKER(config, "mono").front_center();

            NAMCO(config, m_namco_sound, MASTER_CLOCK / 6 / 32); // 96 KHz
            m_namco_sound.op0.set_voices(3);
            m_namco_sound.op0.disound.add_route(ALL_OUTPUTS, "mono", 1.0);

            SAMPLES(config, m_samples);
            m_samples.op0.set_channels(1);
            m_samples.op0.set_samples_names(rallyx_sample_names);
            m_samples.op0.disound.add_route(ALL_OUTPUTS, "mono", 0.80);
        }


        //void rallyx_state::jungler(machine_config &config)

        //void rallyx_state::tactcian(machine_config &config)

        //void rallyx_state::locomotn(machine_config &config)

        //void rallyx_state::commsega(machine_config &config)
    }


    partial class rallyx : construct_ioport_helper
    {
        /*************************************
         *
         *  ROM definition(s)
         *
         *************************************/
        //ROM_START( rallyx )
        static readonly tiny_rom_entry [] rom_rallyx =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "1b",           0x0000, 0x1000, CRC("5882700d") + SHA1("b6029e9730f1694894fe8b729ac0ba8d6712dea9") ),
            ROM_LOAD( "rallyxn.1e",   0x1000, 0x1000, CRC("ed1eba2b") + SHA1("82d3a4b34b0ff5cfdb8ca7c18ad5c63d943b8484") ),
            ROM_LOAD( "rallyxn.1h",   0x2000, 0x1000, CRC("4f98dd1c") + SHA1("8a20fadcea76802d1c412ba62086abb846ad54a8") ),
            ROM_LOAD( "rallyxn.1k",   0x3000, 0x1000, CRC("9aacccf0") + SHA1("9b22079972c0f9970d62d62751db4783a87796d5") ),

            ROM_REGION( 0x1000, "gfx1", 0 ),
            ROM_LOAD( "8e",           0x0000, 0x1000, CRC("277c1de5") + SHA1("30bc57263e8dad870c501c76bce6f42d69ab9e00") ),

            ROM_REGION( 0x0100, "gfx2", 0 ),
            ROM_LOAD( "rx1-6.8m",     0x0000, 0x0100, CRC("3c16f62c") + SHA1("7a3800be410e306cf85753b9953ffc5575afbcd6") ),  // Prom type: IM5623    - dots

            ROM_REGION( 0x0160, "proms", 0 ),
            ROM_LOAD( "rx1-1.11n",    0x0000, 0x0020, CRC("c7865434") + SHA1("70c1c9610ba6f1ead77f347e7132958958bccb31") ),  // Prom type: M3-7603-5 - palette
            ROM_LOAD( "rx1-7.8p",     0x0020, 0x0100, CRC("834d4fda") + SHA1("617864d3df0917a513e8255ad8d96ae7a04da5a1") ),  // Prom type: IM5623    - lookup table
            ROM_LOAD( "rx1-2.4n",     0x0120, 0x0020, CRC("8f574815") + SHA1("4f84162db9d58b64742c67dc689eb665b9862fb3") ),  // Prom type: N82S123N  - video layout (not used)
            ROM_LOAD( "rx1-3.7k",     0x0140, 0x0020, CRC("b8861096") + SHA1("26fad384ed7a1a1e0ba719b5578e2dbb09334a25") ),  // Prom type: M3-7603-5 - video timing (not used)

            ROM_REGION( 0x0200, "namco", 0 ), // sound proms
            ROM_LOAD( "rx1-5.3p",     0x0000, 0x0100, CRC("4bad7017") + SHA1("3e6da9d798f5e07fa18d6ce7d0b148be98c766d5") ),  // Prom type: IM5623
            ROM_LOAD( "rx1-4.2m",     0x0100, 0x0100, CRC("77245b66") + SHA1("0c4d0bee858b97632411c440bea6948a74759746") ),  // Prom type: IM5623 - not used

            ROM_END,
        };


        //ROM_START( rallyxa )

        //ROM_START( rallyxm )

        //ROM_START( rallyxmr )

        //ROM_START( dngrtrck ) // PROMs weren't dumped for this PCB, supposed to match

        //ROM_START( nrallyx )

        //ROM_START( nrallyxb )

        //ROM_START( jungler )

        //ROM_START( junglers )

        //ROM_START( junglero )

        //ROM_START( jackler ) // Board ID SL-HA-2061-21-B

        //ROM_START( savanna )

        //ROM_START( tactcian )

        //ROM_START( tactcian2 )

        //ROM_START( locomotn )

        //ROM_START( gutangtn )

        //ROM_START( cottong )

        //ROM_START( locoboot )

        //ROM_START( commsega )


        /*************************************
         *
         *  Game driver(s)
         *
         *************************************/

        static void rallyx_state_rallyx(machine_config config, device_t device) { rallyx_state rallyx_state = (rallyx_state)device; rallyx_state.rallyx(config); }

        static rallyx m_rallyx = new rallyx();

        static device_t device_creator_rallyx(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new rallyx_state(mconfig, type, tag); }


        public static readonly game_driver driver_rallyx = GAME(device_creator_rallyx, rom_rallyx, "1980", "rallyx", "0", rallyx_state_rallyx, m_rallyx.construct_ioport_rallyx, driver_device.empty_init, ROT0,  "Namco", "Rally X (32k Ver.?)", MACHINE_IMPERFECT_SOUND | MACHINE_SUPPORTS_SAVE );
    }
}
