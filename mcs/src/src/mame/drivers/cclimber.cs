// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u32 = System.UInt32;

using static mame._74259_global;
using static mame.attotime_global;
using static mame.cclimber_global;
using static mame.diexec_global;
using static mame.digfx_global;
using static mame.drawgfx_global;
using static mame.driver_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.input_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.z80_global;


namespace mame
{
    partial class cclimber_state : driver_device
    {
        protected override void machine_start()
        {
            save_item(NAME(new { m_nmi_mask }));
        }


        //void cclimber_state::swimmer_sh_soundlatch_w(uint8_t data)

        //void cclimber_state::yamato_p0_w(uint8_t data)

        //void cclimber_state::yamato_p1_w(uint8_t data)

        //uint8_t cclimber_state::yamato_p0_r()

        //uint8_t cclimber_state::yamato_p1_r()

        //void cclimber_state::toprollr_rombank_w(int state)


        //MACHINE_RESET_MEMBER(cclimber_state,cclimber)
        void machine_reset_cclimber()
        {
            /* Disable interrupts, River Patrol / Silver Land needs this otherwise returns bad RAM on POST */
            m_nmi_mask = false;  //0;

            m_toprollr_rombank = 0;
        }


        void nmi_mask_w(int state)
        {
            m_nmi_mask = state != 0;
        }


        //uint8_t cclimber_state::bagmanf_a000_r()


        /* Note that River Patrol reads/writes to a000-a4f0. This is a bug in the code.
           The instruction at 0x0593 should say LD DE,$8000 */

        void rpatrol_map(address_map map)
        {
            map.op(0x0000, 0x5fff).rom();
            map.op(0x6000, 0x6bff).ram();             /* Crazy Kong only */
            map.op(0x8000, 0x83ff).ram();
            map.op(0x8800, 0x88ff).ram().share("bigspriteram");
            map.op(0x8900, 0x8bff).ram();             /* not used, but initialized */
            map.op(0x9000, 0x93ff).mirror(0x0400).ram().share("videoram");
            /* 9800-9bff and 9c00-9fff share the same RAM, interleaved */
            /* (9800-981f for scroll, 9c20-9c3f for color RAM, and so on) */
            map.op(0x9800, 0x981f).ram().share("column_scroll");
            map.op(0x9820, 0x987f).ram();  /* not used, but initialized */
            map.op(0x9880, 0x989f).ram().share("spriteram");
            map.op(0x98a0, 0x98db).ram();  /* not used, but initialized */
            map.op(0x98dc, 0x98df).ram().share("bigspritectrl");
            map.op(0x98e0, 0x9bff).ram();  /* not used, but initialized */
            map.op(0x9c00, 0x9fff).ram().w(cclimber_colorram_w).share("colorram");
            map.op(0xa000, 0xa007).w(m_mainlatch, (offset, data) => { m_mainlatch.op0.write_d0(offset, data); });
            map.op(0xa000, 0xa000).portr("P1");
            map.op(0xa800, 0xa800).portr("P2");
            map.op(0xb000, 0xb000).portr("DSW");
            map.op(0xb800, 0xb800).portr("SYSTEM");
        }


        void cclimber_map(address_map map, device_t device)
        {
            rpatrol_map(map);

            map.op(0xa800, 0xa800).portr("P2").w("cclimber_audio", (data) => { ((cclimber_audio_device)subdevice("cclimber_audio")).sample_rate_w(data); });
            map.op(0xb000, 0xb000).portr("DSW").w("cclimber_audio", (data) => { ((cclimber_audio_device)subdevice("cclimber_audio")).sample_volume_w(data); });
        }


        void decrypted_opcodes_map(address_map map, device_t owner)
        {
            map.op(0x0000, 0x5fff).rom().share("decrypted_opcodes");
        }


        //void cclimber_state::cannonb_map(address_map &map)

        //void cclimber_state::swimmer_map(address_map &map)

        //void cclimber_state::guzzler_map(address_map &map)

        //void cclimber_state::yamato_map(address_map &map)

        //void cclimber_state::yamato_decrypted_opcodes_map(address_map &map)

        //void cclimber_state::toprollr_map(address_map &map)

        //void cclimber_state::bagmanf_map(address_map &map)

        //void cclimber_state::toprollr_decrypted_opcodes_map(address_map &map)


        void cclimber_portmap(address_map map, device_t device)
        {
            map.global_mask(0xff);
            map.op(0x08, 0x09).w("cclimber_audio:aysnd", (offset, data) => { ((ay8910_device)subdevice("cclimber_audio:aysnd")).address_data_w(offset, data); });
            map.op(0x0c, 0x0c).r("cclimber_audio:aysnd", () => { return ((ay8910_device)subdevice("cclimber_audio:aysnd")).data_r(); });
        }


        //void cclimber_state::rpatrol_portmap(address_map &map)

        //void cclimber_state::yamato_portmap(address_map &map)

        //void cclimber_state::swimmer_audio_map(address_map &map)

        //void cclimber_state::yamato_audio_map(address_map &map)

        //void cclimber_state::swimmer_audio_portmap(address_map &map)

        //void cclimber_state::yamato_audio_portmap(address_map &map)
    }


    public partial class cclimber : construct_ioport_helper
    {
        //static INPUT_PORTS_START( cclimber )
        void construct_ioport_cclimber(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("P1");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICKLEFT_UP ); PORT_8WAY();
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICKLEFT_DOWN ); PORT_8WAY();
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICKLEFT_LEFT ); PORT_8WAY();
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICKLEFT_RIGHT ); PORT_8WAY();
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_JOYSTICKRIGHT_UP ); PORT_8WAY();
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_JOYSTICKRIGHT_DOWN ); PORT_8WAY();
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_JOYSTICKRIGHT_LEFT ); PORT_8WAY();
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_JOYSTICKRIGHT_RIGHT ); PORT_8WAY();

            PORT_START("P2");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICKLEFT_UP ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICKLEFT_DOWN ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICKLEFT_LEFT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICKLEFT_RIGHT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_JOYSTICKRIGHT_UP ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_JOYSTICKRIGHT_DOWN ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_JOYSTICKRIGHT_LEFT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_JOYSTICKRIGHT_RIGHT ); PORT_8WAY(); PORT_COCKTAIL();

            PORT_START("DSW");
            PORT_DIPNAME( 0x03, 0x00, DEF_STR( Lives ) ); PORT_DIPLOCATION("SW:!1,!2");
            PORT_DIPSETTING(    0x00, "3" );
            PORT_DIPSETTING(    0x01, "4" );
            PORT_DIPSETTING(    0x02, "5" );
            PORT_DIPSETTING(    0x03, "6" );
            PORT_DIPUNKNOWN_DIPLOC( 0x04, 0x00, "SW:!3" );       // Look code at 0x03c4 : 0x8076 is never tested !
            PORT_DIPNAME( 0x08, 0x00, "Rack Test (Cheat)" ); PORT_CODE(KEYCODE_F1); PORT_DIPLOCATION("SW:!4");
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x08, DEF_STR( On ) );
            PORT_DIPNAME( 0x30, 0x00, DEF_STR( Coin_A ) ); PORT_DIPLOCATION("SW:!5,!6");
            PORT_DIPSETTING(    0x30, DEF_STR( _4C_1C ) );
            PORT_DIPSETTING(    0x20, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x10, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_1C ) );
            PORT_DIPNAME( 0xc0, 0x00, DEF_STR( Coin_B ) ); PORT_DIPLOCATION("SW:!7,!8");  // Also "Bonus Life" due to code at 0x03d4
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_1C ) );                                // Bonus life : 30000 points
            PORT_DIPSETTING(    0x40, DEF_STR( _1C_2C ) );                                // Bonus life : 50000 points
            PORT_DIPSETTING(    0x80, DEF_STR( _1C_3C ) );                                // Bonus life : 30000 points
            PORT_DIPSETTING(    0xc0, DEF_STR( Free_Play ) );                            // Bonus life : 50000 points

            PORT_START("SYSTEM");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_COIN1 );
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_COIN2 );
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_START1 );
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_START2 );
            PORT_DIPNAME( 0x10, 0x10, DEF_STR( Cabinet ) );
            PORT_DIPSETTING(    0x10, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Cocktail ) );
            PORT_BIT( 0xe0, IP_ACTIVE_HIGH, IPT_UNUSED );

            INPUT_PORTS_END();
        }


        /* Same as 'cclimber' but correct "Bonus Life" Dip Switch */
        //static INPUT_PORTS_START( cclimberj )

        //static INPUT_PORTS_START( ckong )

        //static INPUT_PORTS_START( bagmanf )

        /* Similar to normal Crazy Kong except for the lives per game */
        //static INPUT_PORTS_START( ckongb )

        //static INPUT_PORTS_START( ckongb2 )

        //static INPUT_PORTS_START( cannonb )

        //static INPUT_PORTS_START( rpatrol )

        //static INPUT_PORTS_START( swimmer )

        /* Same as 'swimmer' but different "Difficulty" Dip Switch */
        //static INPUT_PORTS_START( swimmerb )

        //static INPUT_PORTS_START( guzzler )

        //static INPUT_PORTS_START( yamato )

        //static INPUT_PORTS_START( toprollr )
    }


    partial class cclimber_state : driver_device
    {
        static readonly gfx_layout cclimber_charlayout = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,2),
            2,
            new u32[] { 0, RGN_FRAC(1,2) },
            new u32[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new u32[] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
            8*8
        );

        static readonly gfx_layout cclimber_spritelayout = new gfx_layout
        (
            16,16,
            RGN_FRAC(1,2),
            2,
            new u32[] { 0, RGN_FRAC(1,2) },
            new u32[] { 0, 1, 2, 3, 4, 5, 6, 7,
                    8*8+0, 8*8+1, 8*8+2, 8*8+3, 8*8+4, 8*8+5, 8*8+6, 8*8+7 },
            new u32[] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
                    16*8, 17*8, 18*8, 19*8, 20*8, 21*8, 22*8, 23*8 },
            32*8
        );

        //static const gfx_layout cannonb_charlayout =

        //static const gfx_layout cannonb_spritelayout =

        //static const gfx_layout swimmer_charlayout =

        //static const gfx_layout swimmer_spritelayout =


        //static GFXDECODE_START( gfx_cclimber )
        static readonly gfx_decode_entry [] gfx_cclimber =
        {
            GFXDECODE_ENTRY( "gfx1", 0x0000, cclimber_charlayout,      0, 16 ), /* characters */
            GFXDECODE_ENTRY( "gfx1", 0x0000, cclimber_spritelayout,    0, 16 ), /* sprites */
            GFXDECODE_ENTRY( "gfx2", 0x0000, cclimber_charlayout,   16*4,  8 ), /* big sprites */

            //GFXDECODE_END
        };

        //static GFXDECODE_START( gfx_cannonb )

        //static GFXDECODE_START( gfx_swimmer )

        //static GFXDECODE_START( gfx_toprollr )


        void vblank_irq(int state)
        {
            if (state != 0 && m_nmi_mask)
                m_maincpu.op0.pulse_input_line(INPUT_LINE_NMI, attotime.zero);
        }


        //void cclimber_state::bagmanf_vblank_irq(int state)


        void root(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, new XTAL(18_432_000) / 3 / 2);  /* 3.072 MHz */
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, cclimber_map);
            m_maincpu.op0.memory().set_addrmap(AS_IO, cclimber_portmap);

            LS259(config, m_mainlatch, 0);
            m_mainlatch.op0.q_out_cb<u32_const_0>().set((write_line_delegate)nmi_mask_w).reg();
            m_mainlatch.op0.q_out_cb<u32_const_1>().set((write_line_delegate)flip_screen_x_w).reg();
            m_mainlatch.op0.q_out_cb<u32_const_2>().set((write_line_delegate)flip_screen_y_w).reg();

            /* video hardware */
            screen_device screen = SCREEN(config, "screen", SCREEN_TYPE_RASTER);
            screen.set_refresh_hz(60);
            screen.set_vblank_time(ATTOSECONDS_IN_USEC(0));
            screen.set_size(32*8, 32*8);
            screen.set_visarea(0*8, 32*8-1, 2*8, 30*8-1);
            screen.set_screen_update(screen_update_cclimber);
            screen.set_palette(m_palette);
            screen.screen_vblank().set((write_line_delegate)vblank_irq).reg();

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_cclimber);
            PALETTE(config, m_palette, cclimber_palette, 16*4+8*4);

            MCFG_VIDEO_START_OVERRIDE(config, video_start_cclimber);
        }


        void cclimber(machine_config config)
        {
            root(config);

            // 7J on CCG-1
            m_mainlatch.op0.q_out_cb<u32_const_4>().set("cclimber_audio", (int state) => { ((cclimber_audio_device)subdevice("cclimber_audio")).sample_trigger(state); }).reg();

            /* sound hardware */
            SPEAKER(config, "speaker").front_center();

            CCLIMBER_AUDIO(config, "cclimber_audio", 0);
        }


        //void cclimber_state::rpatrol(machine_config &config)


        public void cclimberx(machine_config config)
        {
            cclimber(config);
            m_maincpu.op0.memory().set_addrmap(AS_OPCODES, decrypted_opcodes_map);
        }


        //void cclimber_state::ckongb(machine_config &config)

        //void cclimber_state::cannonb(machine_config &config)

        //void cclimber_state::bagmanf(machine_config &config)

        //void cclimber_state::yamato(machine_config &config)

        //void cclimber_state::toprollr(machine_config &config)

        //void cclimber_state::swimmer(machine_config &config)

        //void cclimber_state::guzzler(machine_config &config)
    }


    partial class cclimber : construct_ioport_helper
    {
        /***************************************************************************

          Game driver(s)

        ***************************************************************************/

        //ROM_START( cclimber )
        static readonly tiny_rom_entry [] rom_cclimber =
        {
            ROM_REGION( 0x6000, "maincpu", 0 ),
            ROM_LOAD( "cc11",         0x0000, 0x1000, CRC("217ec4ff") + SHA1("334604c3a051d57440a9d0bfc34b809418ef1d2d") ),
            ROM_LOAD( "cc10",         0x1000, 0x1000, CRC("b3c26cef") + SHA1("f52cb5482c12a9c5fb56e2e2aec7cab0ed23e5a5") ),
            ROM_LOAD( "cc09",         0x2000, 0x1000, CRC("6db0879c") + SHA1("c0ba1976c1dcd6edadd78073173a26851ae8dd4f") ),
            ROM_LOAD( "cc08",         0x3000, 0x1000, CRC("f48c5fe3") + SHA1("79072bbbf37387998ffd031afe8eb569a16fa9bd") ),
            ROM_LOAD( "cc07",         0x4000, 0x1000, CRC("3e873baf") + SHA1("8870dc5948cdd3c8d2fe9e54a20cf6c311c94e53") ),

            ROM_REGION( 0x4000, "gfx1", 0 ),
            ROM_LOAD( "cc06",         0x0000, 0x0800, CRC("481b64cc") + SHA1("3f35c545fc784ed4f969aba2d7be6e13a5ae32b7") ),
            /* 0x0800-0x0fff - empty */
            ROM_LOAD( "cc05",         0x1000, 0x0800, CRC("2c33b760") + SHA1("2edea8fe13376fbd51a5586d97aba3b30d78e94b") ),
            /* 0x1800-0xffff - empty */
            ROM_LOAD( "cc04",         0x2000, 0x0800, CRC("332347cb") + SHA1("4115ca32af73f1791635b7d9e093bf77088a8222") ),
            /* 0x2800-0x2fff - empty */
            ROM_LOAD( "cc03",         0x3000, 0x0800, CRC("4e4b3658") + SHA1("0d39a8cb5cd6cf06008be60707f9b277a8a32a2d") ),
            /* 0x3800-0x3fff - empty */

            ROM_REGION( 0x1000, "gfx2", 0 ),
            ROM_LOAD( "cc02",         0x0000, 0x0800, CRC("14f3ecc9") + SHA1("a1b5121abfbe8f07580eb3fa6384352d239a3d75") ),
            ROM_LOAD( "cc01",         0x0800, 0x0800, CRC("21c0f9fb") + SHA1("44fad56d302a439257216ddac9fd62b3666589f1") ),

            ROM_REGION( 0x0060, "proms", 0 ),
            ROM_LOAD( "cclimber.pr1", 0x0000, 0x0020, CRC("751c3325") + SHA1("edce2bc883996c1d72dc6c1c9f62799b162d415a") ),
            ROM_LOAD( "cclimber.pr2", 0x0020, 0x0020, CRC("ab1940fa") + SHA1("8d98e05cbaa6f55770c12e0a9a8ed9c73cc54423") ),
            ROM_LOAD( "cclimber.pr3", 0x0040, 0x0020, CRC("71317756") + SHA1("1195f0a037e379cc1a3c0314cb746f5cd2bffe50") ),

            ROM_REGION( 0x2000, "cclimber_audio:samples", 0 ),
            ROM_LOAD( "cc13",         0x0000, 0x1000, CRC("e0042f75") + SHA1("86cb31b110742a0f7ae33052c88f42d00deb5468") ),
            ROM_LOAD( "cc12",         0x1000, 0x1000, CRC("5da13aaa") + SHA1("b2d41e69435d09c456648a10e33f5e1fbb0bc64c") ),

            ROM_END,
        };


        //ROM_START( cclimbera )

        //ROM_START( cclimberj )

        //ROM_START( ccboot )

        //ROM_START( ccboot2 )

        //ROM_START( ccbootmr )  /* Model Racing bootleg */

        //ROM_START( cclimbroper )

        //ROM_START( cclimbrrod )


        // Sets below are Crazy Kong Part II and have an extra screen in attract mode, showing a caged Kong and copyright

        //ROM_START( ckongpt2 )

        //ROM_START( ckongpt2a )

        //ROM_START( ckongpt2j )

        //ROM_START( ckongpt2jeu )

        //ROM_START( ckongpt2ss )

        //ROM_START( ckongpt2b )

        //ROM_START( ckongpt2b2 )

        // Sets below are 'Crazy Kong' without the extra Falcon screen or Pt. 2 subtitle, they also have worse colours

        //ROM_START( ckong )

        //ROM_START( ckongo )

        //ROM_START( ckongalc )

        //ROM_START( bigkong )

        //ROM_START( monkeyd )

        //ROM_START( dking )

        //ROM_START( ckongdks )

        /* Original ORCA PCB, with a suicide battery attached */
        //ROM_START( rpatrol )

        //ROM_START( rpatroln )

        //ROM_START( rpatrolb )

        //ROM_START( silvland )

        /*This dump was a mess.  11n and 11k seem to be bad dumps, the second half should probably be sprite data
          Comparing to set 2 11l and 11h are unnecessary, and are actually from Le Bagnard(set1), as is 5m.
          5n ID'd as unknown, but it also is from bagnard with some patches. */
        //ROM_START( cannonb )

        //ROM_START( cannonb2 )

        //ROM_START( cannonb3 )

        //ROM_START( bagmanf )

        //ROM_START( swimmer )

        //ROM_START( swimmera )

        //ROM_START( swimmerb )

        //ROM_START( guzzler )

        //ROM_START( guzzlers ) /* Swimmer Conversion, 1k vs 2k romsize in maincpu */

        //ROM_START( yamato )

        //ROM_START( yamato2 )

        //ROM_START( toprollr )
    }


        //void cclimber_state::init_yamato()

        //void cclimber_state::init_toprollr()

        //void cclimber_state::init_dking()

        //void cclimber_state::init_rpatrol()


    partial class cclimber : construct_ioport_helper
    {
        static void cclimber_state_cclimberx(machine_config config, device_t device) { cclimber_state cclimber_state = (cclimber_state)device; cclimber_state.cclimberx(config); }
        static void cclimber_state_init_cclimber(device_t owner) { cclimber_state cclimber_state = (cclimber_state)owner; cclimber_state.init_cclimber(); }

        static cclimber m_cclimber = new cclimber();

        static device_t device_creator_cclimber(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new cclimber_state(mconfig, type, tag); }

        public static readonly game_driver driver_cclimber = GAME(device_creator_cclimber, rom_cclimber, "1980", "cclimber", "0", cclimber_state_cclimberx, m_cclimber.construct_ioport_cclimber, cclimber_state_init_cclimber, ROT0, "Nichibutsu", "Crazy Climber (US set 1)", MACHINE_SUPPORTS_SAVE);
    }
}
