// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u32 = System.UInt32;

using static mame.attotime_global;
using static mame.bankdev_global;
using static mame.digfx_global;
using static mame.drawgfx_global;
using static mame.driver_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.m6502_global;
using static mame.mc6845_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.snk6502_global;


namespace mame
{
    partial class snk6502_state : driver_device
    {
        protected static readonly XTAL MASTER_CLOCK    = new XTAL(11_289_000);


#if false
        void snk6502_state::machine_start()
        {
            // these could be split in different MACHINE_STARTs to save only
            // what's actually needed, but is the extra complexity really worth it?
            save_item(NAME(m_sasuke_counter)); // sasuke only
            save_item(NAME(m_charbank));
            save_item(NAME(m_backcolor));
            save_item(NAME(m_irq_mask)); // satansat only
        }
#endif


        //TIMER_DEVICE_CALLBACK_MEMBER(snk6502_state::sasuke_update_counter)

        //void snk6502_state::sasuke_start_counter()


        /*************************************
         *  Custom input ports
         *************************************/
        //CUSTOM_INPUT_MEMBER(snk6502_state::sasuke_count_r)


        /*************************************
         *  Memory maps
         *************************************/
        //void snk6502_state::sasuke_map(address_map &map)

        //void snk6502_state::satansat_map(address_map &map)
    }


    partial class vanguard_state : snk6502_state
    {
#if false
        uint8_t vanguard_state::highmem_r(offs_t offset)
        {
            // RDY toggles on ϕ2 during each access to memory above $3FFF, generating one wait state
            if (!machine().side_effects_disabled())
                m_maincpu->adjust_icount(-1);

            return m_highmem->read8(offset + 0x4000);
        }

        void vanguard_state::highmem_w(offs_t offset, uint8_t data)
        {
            // RDY toggles on ϕ2 during each access to memory above $3FFF, but 6502 does not apply wait states to writes
            m_highmem->write8(offset + 0x4000, data);
        }
#endif


        void vanguard_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map(0x0000, 0x03ff).ram();
            map(0x0400, 0x07ff).ram().w(FUNC(vanguard_state::videoram2_w)).share("videoram2");
            map(0x0800, 0x0bff).ram().w(FUNC(vanguard_state::videoram_w)).share("videoram");
            map(0x0c00, 0x0fff).ram().w(FUNC(vanguard_state::colorram_w)).share("colorram");
            map(0x1000, 0x1fff).ram().w(FUNC(vanguard_state::charram_w)).share("charram");
            map(0x3000, 0x3000).w("crtc", FUNC(mc6845_device::address_w));
            map(0x3001, 0x3001).w("crtc", FUNC(mc6845_device::register_w));
            map(0x3100, 0x3102).w("snk6502", FUNC(vanguard_sound_device::sound_w));
            map(0x3103, 0x3103).w(FUNC(vanguard_state::flipscreen_w));
            map(0x3104, 0x3104).portr("IN0");
            map(0x3105, 0x3105).portr("IN1");
            map(0x3106, 0x3106).portr("DSW");
            map(0x3107, 0x3107).portr("IN2");
            map(0x3200, 0x3200).w(FUNC(vanguard_state::scrollx_w));
            map(0x3300, 0x3300).w(FUNC(vanguard_state::scrolly_w));
            map(0x3400, 0x3400).w("snk6502", FUNC(vanguard_sound_device::speech_w)); // speech
            map(0x4000, 0xffff).rw(FUNC(vanguard_state::highmem_r), FUNC(vanguard_state::highmem_w));
#endif
        }


        void vanguard_upper_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map(0x4000, 0xbfff).rom().region("maincpu", 0x4000);
            map(0xf000, 0xffff).rom().region("maincpu", 0xf000); /* for the reset / interrupt vectors */
#endif
        }
    }


    //void fantasy_state::fantasy_map(address_map &map)

    //void fantasy_state::pballoon_map(address_map &map)

    //void fantasy_state::pballoon_upper_map(address_map &map)


#if false
    /*************************************
     *  Port definitions
     *************************************/
    INPUT_CHANGED_MEMBER(snk6502_state::coin_inserted)
    {
        m_maincpu->set_input_line(INPUT_LINE_NMI, newval ? CLEAR_LINE : ASSERT_LINE);
    }
#endif


    partial class snk6502 : construct_ioport_helper
    {
        //static INPUT_PORTS_START( snk6502_generic_joy8way )
        void construct_ioport_snk6502_generic_joy8way(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            throw new emu_unimplemented();
#if false
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0")
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_UNKNOWN )
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_UNKNOWN )
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_UNKNOWN )
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_UNKNOWN )
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN) PORT_8WAY
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP) PORT_8WAY
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ) PORT_8WAY
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ) PORT_8WAY

            PORT_START("IN1")
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_UNKNOWN )
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_UNKNOWN )
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_UNKNOWN )
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_UNKNOWN )
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ) PORT_8WAY PORT_COCKTAIL
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ) PORT_8WAY PORT_COCKTAIL
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ) PORT_8WAY PORT_COCKTAIL
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ) PORT_8WAY PORT_COCKTAIL

            PORT_START("IN2")
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_COIN2 ) PORT_IMPULSE(1) PORT_CHANGED_MEMBER(DEVICE_SELF, snk6502_state,coin_inserted, 0)
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_COIN1 ) PORT_IMPULSE(1) PORT_CHANGED_MEMBER(DEVICE_SELF, snk6502_state,coin_inserted, 0)
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_UNKNOWN )
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_UNKNOWN )
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_UNKNOWN )
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_UNKNOWN )
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_START2 )
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_START1 )

            PORT_START("DSW")
            PORT_DIPNAME( 0x01, 0x01, DEF_STR( Cabinet ) ) PORT_DIPLOCATION("SW1:!1")
            PORT_DIPSETTING(    0x01, DEF_STR( Upright ) )
            PORT_DIPSETTING(    0x00, DEF_STR( Cocktail ) )
            PORT_DIPNAME( 0x0e, 0x02, DEF_STR( Coinage ) ) PORT_DIPLOCATION("SW1:!2,!3,!4")
            PORT_DIPSETTING (   0x02, DEF_STR( 2C_1C ) )
            PORT_DIPSETTING (   0x00, DEF_STR( 1C_1C ) )
            PORT_DIPSETTING (   0x08, DEF_STR( 1C_2C ) )
            PORT_DIPSETTING (   0x04, DEF_STR( 1C_3C ) )
            PORT_DIPSETTING (   0x0c, DEF_STR( 1C_6C ) )
        /*  PORT_DIPSETTING (   0x06, DEF_STR( 1C_1C ) ) */
        /*  PORT_DIPSETTING (   0x0a, DEF_STR( 1C_1C ) ) */
        /*  PORT_DIPSETTING (   0x0e, DEF_STR( 1C_1C ) ) */
            PORT_DIPNAME( 0x30, 0x00, DEF_STR( Lives ) ) PORT_DIPLOCATION("SW1:!5,!6")
            PORT_DIPSETTING(    0x00, "3" )
            PORT_DIPSETTING(    0x10, "4" )
            PORT_DIPSETTING(    0x20, "5" )
        /*  PORT_DIPSETTING(    0x30, "3" ) */
            PORT_DIPNAME( 0x40, 0x00, "Coinage Bonus" ) PORT_DIPLOCATION("SW1:!7")      /* see notes */
            PORT_DIPSETTING (   0x40, DEF_STR( 2C_1C ) )
            PORT_DIPSETTING (   0x00, DEF_STR( 1C_1C ) )
            PORT_DIPUNUSED_DIPLOC( 0x80, IP_ACTIVE_HIGH, "SW1:!8" )

            INPUT_PORTS_END
#endif
        }


        //static INPUT_PORTS_START( satansat )

        //static INPUT_PORTS_START( sasuke )


        //static INPUT_PORTS_START( vanguard )
        void construct_ioport_vanguard(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            throw new emu_unimplemented();
#if false
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_INCLUDE(snk6502_generic_joy8way)

            PORT_MODIFY("IN0")
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_BUTTON3 )                /* fire down */
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_BUTTON4 )                /* fire up */
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_BUTTON2 )                /* fire right */
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_BUTTON1 )                /* fire left */

            PORT_MODIFY("IN1")
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_BUTTON3 ) PORT_COCKTAIL  /* fire down */
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_BUTTON4 ) PORT_COCKTAIL  /* fire up */
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_BUTTON2 ) PORT_COCKTAIL  /* fire right */
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_BUTTON1 ) PORT_COCKTAIL  /* fire left */

            PORT_MODIFY("IN2")
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_CUSTOM ) PORT_READ_LINE_DEVICE_MEMBER("snk6502:custom", snk6502_sound_device, music0_playing)     // music0 playing

            INPUT_PORTS_END
#endif
        }


        //static INPUT_PORTS_START( fantasy )

        //static INPUT_PORTS_START( fantasyu )

        //static INPUT_PORTS_START( pballoon )

        //static INPUT_PORTS_START( nibbler )

        //static INPUT_PORTS_START( nibbler8 )

        //static INPUT_PORTS_START( nibbler6 )
    }


    partial class snk6502_state : driver_device
    {
        /*************************************
         *  Graphics layouts
         *************************************/
        //static const gfx_layout swapcharlayout =

        static readonly gfx_layout charlayout = new gfx_layout
        (
            8,8,    /* 8*8 characters */
            RGN_FRAC(1,2),
            2,      /* 2 bits per pixel */
            new u32[] { 0, RGN_FRAC(1,2) }, /* the two bitplanes are separated */
            new u32[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new u32[] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
            8*8     /* every char takes 8 consecutive bytes */
        );


        static readonly gfx_layout charlayout_memory = new gfx_layout
        (
            8,8,    /* 8*8 characters */
            256,    /* 256 characters */
            2,      /* 2 bits per pixel */
            new u32[] { 0, 256*8*8 }, /* the two bitplanes are separated */
            new u32[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new u32[] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
            8*8     /* every char takes 8 consecutive bytes */
        );


        //static GFXDECODE_START( gfx_sasuke )

        //static GFXDECODE_START( gfx_satansat )


        //static GFXDECODE_START( gfx_vanguard )
        protected static readonly gfx_decode_entry [] gfx_vanguard =
        {
            GFXDECODE_ENTRY( null,           0x1000, charlayout_memory,   0, 8 ),    /* the game dynamically modifies this */
            GFXDECODE_ENTRY( "gfx1", 0x0000, charlayout,        8*4, 8 )

            //GFXDECODE_END
        };


        /*************************************
         *  Interrupt Generators
         *************************************/
        //INTERRUPT_GEN_MEMBER(snk6502_state::satansat_interrupt)


        //INTERRUPT_GEN_MEMBER(snk6502_state::snk6502_interrupt)
        protected void snk6502_interrupt(device_t device)
        {
            throw new emu_unimplemented();
#if false
            device.execute().set_input_line(M6502_IRQ_LINE, HOLD_LINE); /* one IRQ per frame */
#endif
        }


        /*************************************
         *  Machine initialisation
         *************************************/
        //MACHINE_RESET_MEMBER(snk6502_state,sasuke)

        /*************************************
         *  Machine drivers
         *************************************/
        //void snk6502_state::sasuke(machine_config &config)

        //void snk6502_state::satansat(machine_config &config)
    }


    partial class vanguard_state : snk6502_state
    {
        public void vanguard(machine_config config)
        {
            // basic machine hardware
            M6502(config, m_maincpu, MASTER_CLOCK / 8); // runs twice as fast as CRTC
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, vanguard_map);
            m_maincpu.op0.execute().set_vblank_int("screen", snk6502_interrupt);

            ADDRESS_MAP_BANK(config, m_highmem);
            m_highmem.op0.memory().set_addrmap(0, vanguard_upper_map);
            m_highmem.op0.set_data_width(8);
            m_highmem.op0.set_addr_width(16);

            // video hardware
            screen_device screen = SCREEN(config, "screen", SCREEN_TYPE_RASTER);
            screen.set_refresh_hz((MASTER_CLOCK / 16) / (45 * 32 * 8));
            screen.set_vblank_time(ATTOSECONDS_IN_USEC(0));
            screen.set_size(32*8, 32*8);
            screen.set_visarea(0*8, 32*8-1, 0*8, 28*8-1);
            screen.set_screen_update(screen_update);

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_vanguard);
            PALETTE(config, m_palette, snk6502_palette, 64);
            MCFG_VIDEO_START_OVERRIDE(config, video_start_snk6502);

            mc6845_device crtc = MC6845(config, "crtc", MASTER_CLOCK / 16);
            crtc.divideo.set_screen("screen");
            crtc.set_show_border_area(false);
            crtc.set_char_width(8);

            // sound hardware
            VANGUARD_SOUND(config, "snk6502", 0);
        }
    }


    //void fantasy_state::fantasy(machine_config &config)

    //void fantasy_state::nibbler(machine_config &config)

    //void fantasy_state::pballoon(machine_config &config)


    public partial class snk6502 : construct_ioport_helper
    {
        /*************************************
         *  ROM definitions
         *************************************/
        //ROM_START( sasuke )

        //ROM_START( satansat )

        //ROM_START( satansata )

        //ROM_START( zarzon )

        //ROM_START( satansatind )

        //ROM_START( vanguard )
        static readonly tiny_rom_entry [] rom_vanguard =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "sk4_ic07.bin", 0x4000, 0x1000, CRC("6a29e354") + SHA1("ff953962ebc14a28cfc96f8e269cb1e1c188ed8a") ),
            ROM_LOAD( "sk4_ic08.bin", 0x5000, 0x1000, CRC("302bba54") + SHA1("1944f229481328a0635fafda65054106f42a532a") ),
            ROM_LOAD( "sk4_ic09.bin", 0x6000, 0x1000, CRC("424755f6") + SHA1("b4762b40c7ed70d4b90319a1a30983a41a096afb") ),
            ROM_LOAD( "sk4_ic10.bin", 0x7000, 0x1000, CRC("54603274") + SHA1("31571a560dbe300417b3ed5b114fa1d9ef742da9") ),
            ROM_LOAD( "sk4_ic13.bin", 0x8000, 0x1000, CRC("fde157d0") + SHA1("3f705fb6a410004f4f86283694e3694e49701af6") ),
            ROM_RELOAD(               0xf000, 0x1000 ),  /* for the reset and interrupt vectors */
            ROM_LOAD( "sk4_ic14.bin", 0x9000, 0x1000, CRC("0d5b47d0") + SHA1("922621c23f33fe756cb6baa12e5465c4e64f2dda") ),
            ROM_LOAD( "sk4_ic15.bin", 0xa000, 0x1000, CRC("8549b8f8") + SHA1("375bc6f7e15564d5cf7e00c44e2651793c56d6ca") ),
            ROM_LOAD( "sk4_ic16.bin", 0xb000, 0x1000, CRC("062e0be2") + SHA1("45aaf315a62f37460e32d3ba99caaacf4c994810") ),

            ROM_REGION( 0x1000, "gfx1", 0 ),
            ROM_LOAD( "sk5_ic50.bin", 0x0000, 0x0800, CRC("e7d4315b") + SHA1("b99e4ea07292a0eabaa6098037c92a5678627cec") ),
            ROM_LOAD( "sk5_ic51.bin", 0x0800, 0x0800, CRC("96e87858") + SHA1("4e9ccb055919c8acf5837e062857647d5363af60") ),

            ROM_REGION( 0x0040, "proms", 0 ),
            ROM_LOAD( "sk5_ic7.bin",  0x0000, 0x0020, CRC("ad782a73") + SHA1("ddf44f74a20f10ed976c434a885857dade1f86d7") ), /* foreground colors */
            ROM_LOAD( "sk5_ic6.bin",  0x0020, 0x0020, CRC("7dc9d450") + SHA1("9b2d1dfb3270a562d14bd54bfb3405a9095becc0") ), /* background colors */

            ROM_REGION( 0x1000, "snk6502", 0 ),  /* sound ROMs */
            ROM_LOAD( "sk4_ic51.bin", 0x0000, 0x0800, CRC("d2a64006") + SHA1("3f20b59ce1954f65535cd5603ca9271586428e35") ),  /* sound ROM 1 */
            ROM_LOAD( "sk4_ic52.bin", 0x0800, 0x0800, CRC("cc4a0b6f") + SHA1("251b24d60083d516c4ba686d75b41e04d10f7198") ),  /* sound ROM 2 */

            ROM_REGION( 0x5800, "speech", 0 ),   /* space for the speech ROMs (not supported) */
            //ROM_LOAD( "hd38882.bin",  0x0000, 0x4000, NO_DUMP )   /* HD38882 internal ROM */
            ROM_LOAD( "sk6_ic07.bin", 0x4000, 0x0800, CRC("2b7cbae9") + SHA1("3d44a0232d7c94d8170cc06e90cc30bd57c99202") ),
            ROM_LOAD( "sk6_ic08.bin", 0x4800, 0x0800, CRC("3b7e9d7c") + SHA1("d9033188068b2aaa1502c89cf09f955eded8fa7a") ),
            ROM_LOAD( "sk6_ic11.bin", 0x5000, 0x0800, CRC("c36df041") + SHA1("8b51934229b961180d1edb99be3a4d337d37f66f") ),

            ROM_END,
        };


        //ROM_START( vanguardc )

        //ROM_START( vanguardj )

        //ROM_START( vanguardg )

        //ROM_START( fantasyu )

        //ROM_START( fantasyg )

        //ROM_START( fantasyg2 )

        //ROM_START( fantasyj )

        //ROM_START( pballoon )

        //ROM_START( pballoonr )

        //ROM_START( nibbler ) /* revision 9 - rom labels match manual part numbers/locations */

        //ROM_START( nibbler8 ) /* revision 8 */

        //ROM_START( nibbler7 ) /* revision 7 */

        //ROM_START( nibbler6 ) /* revision 6 */

        //ROM_START( nibblera ) /* revision 9 - alternate? */

        //ROM_START( nibblerp ) /* revision 6 + extra soundrom */

        //ROM_START( nibblero ) /* revision 8 */


        /*************************************
         *  Game drivers
         *************************************/

        static void vanguard_state_vanguard(machine_config config, device_t device) { vanguard_state vanguard_state = (vanguard_state)device; vanguard_state.vanguard(config); }

        static snk6502 m_snk6502 = new snk6502();

        static device_t device_creator_vanguard(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new vanguard_state(mconfig, type, tag); }

        public static readonly game_driver driver_vanguard = GAME(device_creator_vanguard, rom_vanguard, "1981", "vanguard", "0", vanguard_state_vanguard, m_snk6502.construct_ioport_vanguard, driver_device.empty_init, ROT90, "SNK", "Vanguard (SNK)", MACHINE_IMPERFECT_SOUND | MACHINE_SUPPORTS_SAVE);
    }
}
