// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using u32 = System.UInt32;
using uint8_t = System.Byte;

using static mame._74259_global;
using static mame.attotime_global;
using static mame.ay8910_global;
using static mame.diexec_global;
using static mame.digfx_global;
using static mame.disound_global;
using static mame.drawgfx_global;
using static mame.driver_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.gen_latch_global;
using static mame.hash_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.watchdog_global;
using static mame.z80_global;


namespace mame
{
    partial class fastfred_state : galaxold_state
    {
        protected override void machine_start()
        {
            base.machine_start();
            save_item(NAME(new { m_charbank }));
            save_item(NAME(new { m_colorbank }));
            save_item(NAME(new { m_nmi_mask }));
            save_item(NAME(new { m_sound_nmi_mask }));
        }


        uint8_t fastfred_custom_io_r(offs_t offset)
        {
            switch (m_maincpu.op0.GetClassInterface<device_state_interface>().pc())
            {
                case 0x03c0: return 0x9d;
                case 0x03e6: return 0x9f;
                case 0x0407: return 0x00;
                case 0x0446: return 0x94;
                case 0x049f: return 0x01;
                case 0x04b1: return 0x00;
                case 0x0dd2: return 0x00;
                case 0x0de4: return 0x20;
                case 0x122b: return 0x10;
                case 0x123d: return 0x00;
                case 0x1a83: return 0x10;
                case 0x1a93: return 0x00;
                case 0x1b26: return 0x00;
                case 0x1b37: return 0x80;
                case 0x2491: return 0x10;
                case 0x24a2: return 0x00;
                case 0x46ce: return 0x20;
                case 0x46df: return 0x00;
                case 0x7b18: return 0x01;
                case 0x7b29: return 0x00;
                case 0x7b47: return 0x00;
                case 0x7b58: return 0x20;
            }

            logerror("Uncaught custom I/O read {0} at {1}\n", 0xc800 + offset, m_maincpu.op0.GetClassInterface<device_state_interface>().pc());
            return 0x00;
        }


        uint8_t flyboy_custom1_io_r(offs_t offset)
        {
            throw new emu_unimplemented();
#if false
            switch (m_maincpu->pc())
            {
                case 0x049d: return 0xad;   /* compare */
                case 0x04b9:            /* compare with 0x9e ??? When ??? */
                case 0x0563: return 0x03;   /* $c085 compare - starts game */
                case 0x069b: return 0x69;   /* $c086 compare         */
                case 0x076b: return 0xbb;   /* $c087 compare         */
                case 0x0852: return 0xd9;   /* $c096 compare         */
                case 0x09d5: return 0xa4;   /* $c099 compare         */
                case 0x0a83: return 0xa4;   /* $c099 compare         */
                case 0x1028:            /* $c08a  bit 0  compare */
                case 0x1051:            /* $c08a  bit 3  compare */
                case 0x107d:            /* $c08c  bit 5  compare */
                case 0x10a7:            /* $c08e  bit 1  compare */
                case 0x10d0:            /* $c08d  bit 2  compare */
                case 0x10f6:            /* $c090  bit 0  compare */
                case 0x3fb6:            /* lddr */

                return 0x00;
            }

            logerror("Uncaught custom I/O read %04X at %04X\n", 0xc085+offset, m_maincpu->pc());
            return 0x00;
#endif
        }


        uint8_t flyboy_custom2_io_r(offs_t offset)
        {
            switch (m_maincpu.op0.GetClassInterface<device_state_interface>().pc())
            {
                case 0x0395: return 0xf7;   /* $C900 compare         */
                case 0x03f5:            /* $c8fd                 */
                case 0x043d:            /* $c8fd                 */
                case 0x0471:            /* $c900                 */
                case 0x1031: return 0x01;   /* $c8fe  bit 0  compare */
                case 0x1068: return 0x04;   /* $c8fe  bit 2  compare */
                case 0x1093: return 0x20;   /* $c8fe  bit 5  compare */
                case 0x10bd: return 0x80;   /* $c8fb  bit 7  compare */
                case 0x103f:            /* $c8fe                 */
                case 0x10e4:            /* $c900                 */
                case 0x110a:            /* $c900                 */
                case 0x3fc8:            /* ld a with c8fc-c900   */

                return 0x00;
            }

            logerror("Uncaught custom I/O read {0} at {1}\n", 0xc8fb + offset, m_maincpu.op0.GetClassInterface<device_state_interface>().pc());
            return 0x00;
        }


        //uint8_t fastfred_state::jumpcoas_custom_io_r(offs_t offset)

        //uint8_t fastfred_state::boggy84_custom_io_r(offs_t offset)

        //MACHINE_START_MEMBER(fastfred_state,imago)

        //WRITE_LINE_MEMBER(fastfred_state::imago_dma_irq_w)

        //void fastfred_state::imago_sprites_bank_w(uint8_t data)

        //void fastfred_state::imago_sprites_dma_w(offs_t offset, uint8_t data)

        //uint8_t fastfred_state::imago_sprites_offset_r(offs_t offset)


        //WRITE_LINE_MEMBER(fastfred_state::nmi_mask_w)
        void nmi_mask_w(int state)
        {
            m_nmi_mask = (uint8_t)state;
            if (m_nmi_mask == 0)
                m_maincpu.op0.set_input_line(INPUT_LINE_NMI, CLEAR_LINE);
        }


        void sound_nmi_mask_w(uint8_t data)
        {
            m_sound_nmi_mask = (uint8_t)(data & 1);
        }


        void fastfred_map(address_map map, device_t device)
        {
            map.op(0x0000, 0xbfff).rom();
            map.op(0xc000, 0xc7ff).ram();
            map.op(0xd000, 0xd3ff).mirror(0x400).ram().w(fastfred_videoram_w).share("videoram");
            map.op(0xd800, 0xd83f).ram().w(fastfred_attributes_w).share("attributesram");
            map.op(0xd840, 0xd85f).ram().share("spriteram");
            map.op(0xd860, 0xdbff).ram(); // Unused, but initialized
            map.op(0xe000, 0xe000).portr("BUTTONS").writeonly().share("bgcolor");
            map.op(0xe800, 0xe800).portr("JOYS");
            map.op(0xf000, 0xf007).mirror(0x07f8).w(m_outlatch, (offset, data) => { m_outlatch.op0.write_d0(offset, data); });
            map.op(0xf000, 0xf000).portr("DSW").nopw();
            map.op(0xf800, 0xf800).r("watchdog", (space) => { return ((watchdog_timer_device)subdevice("watchdog")).reset_r(space); }).w("soundlatch", (data) => { ((generic_latch_8_device)subdevice("soundlatch")).write(data); });
        }


        //void fastfred_state::jumpcoas_map(address_map &map)

        //void fastfred_state::imago_map(address_map &map)


        void sound_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x1fff).rom();
            map.op(0x2000, 0x23ff).ram();
            map.op(0x3000, 0x3000).r("soundlatch", () => { return ((generic_latch_8_device)subdevice("soundlatch")).read(); }).w(sound_nmi_mask_w);
            map.op(0x4000, 0x4000).nopw();  // Reset PSG's
            map.op(0x5000, 0x5001).w("ay8910.1", (offset, data) => { ((ay8910_device)subdevice("ay8910.1")).address_data_w(offset, data); });
            map.op(0x6000, 0x6001).w("ay8910.2", (offset, data) => { ((ay8910_device)subdevice("ay8910.2")).address_data_w(offset, data); });
            map.op(0x7000, 0x7000).nopr(); // only for Imago, read but not used
        }
    }


    partial class fastfred : construct_ioport_helper
    {
        //static INPUT_PORTS_START( common )
        void construct_ioport_common(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("BUTTONS");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_COIN1 );
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_COIN2 );
            PORT_SERVICE( 0x04, IP_ACTIVE_HIGH );
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_START1 );
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_START2 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_COIN3 );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_BUTTON1 );
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_BUTTON1 ); PORT_COCKTAIL();

            PORT_START("JOYS");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ); PORT_8WAY();
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ); PORT_8WAY();
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ); PORT_8WAY();
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ); PORT_8WAY();
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ); PORT_8WAY(); PORT_COCKTAIL();

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( fastfred )
        void construct_ioport_fastfred(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_INCLUDE( construct_ioport_common, ref errorbuf );

            PORT_START("DSW");
            PORT_DIPNAME( 0x0f, 0x00, DEF_STR( Coinage ) );
            PORT_DIPSETTING(    0x01, "A 2/1 B 2/1" );
            PORT_DIPSETTING(    0x02, "A 2/1 B 1/3" );
            PORT_DIPSETTING(    0x00, "A 1/1 B 1/1" );
            PORT_DIPSETTING(    0x03, "A 1/1 B 1/2" );
            PORT_DIPSETTING(    0x04, "A 1/1 B 1/3" );
            PORT_DIPSETTING(    0x05, "A 1/1 B 1/4" );
            PORT_DIPSETTING(    0x06, "A 1/1 B 1/5" );
            PORT_DIPSETTING(    0x07, "A 1/1 B 1/6" );
            PORT_DIPSETTING(    0x08, "A 1/2 B 1/2" );
            PORT_DIPSETTING(    0x09, "A 1/2 B 1/4" );
            PORT_DIPSETTING(    0x0a, "A 1/2 B 1/5" );
            PORT_DIPSETTING(    0x0e, "A 1/2 B 1/6" );
            PORT_DIPSETTING(    0x0b, "A 1/2 B 1/10" );
            PORT_DIPSETTING(    0x0c, "A 1/2 B 1/11" );
            PORT_DIPSETTING(    0x0d, "A 1/2 B 1/12" );
            PORT_DIPSETTING(    0x0f, DEF_STR( Free_Play ) );
            PORT_DIPNAME( 0x10, 0x00, DEF_STR( Lives ) );
            PORT_DIPSETTING(    0x00, "3" );
            PORT_DIPSETTING(    0x10, "5" );
            PORT_DIPNAME( 0x60, 0x20, DEF_STR( Bonus_Life ) );
            PORT_DIPSETTING(    0x20, "30000" );
            PORT_DIPSETTING(    0x40, "50000" );
            PORT_DIPSETTING(    0x60, "100000" );
            PORT_DIPSETTING(    0x00, DEF_STR( None ) );
            PORT_DIPNAME( 0x80, 0x00, DEF_STR( Cabinet ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x80, DEF_STR( Cocktail ) );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( flyboy )
        void construct_ioport_flyboy(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_INCLUDE( construct_ioport_common, ref errorbuf );

            PORT_MODIFY("BUTTONS");
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_UNUSED );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_UNUSED );

            PORT_START("DSW");
            PORT_DIPNAME( 0x03, 0x00, DEF_STR( Coin_A ) );
            PORT_DIPSETTING(    0x03, DEF_STR( _6C_1C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x01, DEF_STR( _1C_2C ) );
            PORT_DIPNAME( 0x0c, 0x00, DEF_STR( Coin_B ) );
            PORT_DIPSETTING(    0x0c, DEF_STR( _6C_1C ) );
            PORT_DIPSETTING(    0x08, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x04, DEF_STR( _1C_2C ) );
            PORT_DIPNAME( 0x30, 0x00, DEF_STR( Lives ) );
            PORT_DIPSETTING(    0x00, "3" );
            PORT_DIPSETTING(    0x10, "5" );
            PORT_DIPSETTING(    0x20, "7" );
            PORT_DIPSETTING(    0x30, "255 (Cheat)");
            PORT_DIPNAME( 0x40, 0x00, "Invulnerability (Cheat)");
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x40, DEF_STR( On ) );
            PORT_DIPNAME( 0x80, 0x00, DEF_STR( Cabinet ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x80, DEF_STR( Cocktail ) );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( jumpcoas )

        //static INPUT_PORTS_START( boggy84 )

        //static INPUT_PORTS_START( redrobin )

        //static INPUT_PORTS_START( imago )

        //static INPUT_PORTS_START( imagoa )
    }


    partial class fastfred_state : galaxold_state
    {
        static readonly gfx_layout charlayout = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,3),
            3,
            new u32 [] { RGN_FRAC(2,3), RGN_FRAC(1,3), RGN_FRAC(0,3) },
            new u32 [] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new u32 [] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
            8*8
        );


        static readonly gfx_layout spritelayout = new gfx_layout
        (
            16,16,
            RGN_FRAC(1,3),
            3,
            new u32 [] { RGN_FRAC(2,3), RGN_FRAC(1,3), RGN_FRAC(0,3) },
            new u32 [] { 0, 1, 2, 3, 4, 5, 6, 7,
                8*8+0, 8*8+1, 8*8+2, 8*8+3, 8*8+4, 8*8+5, 8*8+6, 8*8+7 },
            new u32 [] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
                16*8, 17*8, 18*8, 19*8, 20*8, 21*8, 22*8, 23*8 },
            32*8
        );


        //static const gfx_layout imago_spritelayout =

        //static const gfx_layout imago_char_1bpp =


        //static GFXDECODE_START( gfx_fastfred )
        static readonly gfx_decode_entry [] gfx_fastfred =
        {
            GFXDECODE_ENTRY( "gfx1", 0, charlayout,   0, 32 ),
            GFXDECODE_ENTRY( "gfx2", 0, spritelayout, 0, 32 ),
            //GFXDECODE_END
        };


        //static GFXDECODE_START( gfx_jumpcoas )

        //static GFXDECODE_START( gfx_imago )


        //WRITE_LINE_MEMBER(fastfred_state::vblank_irq)
        void vblank_irq(int state)
        {
            if (state != 0 && m_nmi_mask != 0)
                m_maincpu.op0.set_input_line(INPUT_LINE_NMI, ASSERT_LINE);
        }


        //INTERRUPT_GEN_MEMBER(fastfred_state::sound_timer_irq)
        void sound_timer_irq(device_t device)
        {
            if (m_sound_nmi_mask != 0)
                device.execute().pulse_input_line(INPUT_LINE_NMI, attotime.zero);
        }


        public void fastfred(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, new XTAL(12_432_000) / 4);   /* 3.108 MHz; xtal from pcb pics, divider not verified */
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, fastfred_map);

            Z80(config, m_audiocpu, new XTAL(12_432_000) / 8);  /* 1.554 MHz; xtal from pcb pics, divider not verified */
            m_audiocpu.op0.memory().set_addrmap(AS_PROGRAM, sound_map);
            m_audiocpu.op0.execute().set_periodic_int(sound_timer_irq, attotime.from_hz(4 * 60));

            LS259(config, m_outlatch); // "Control Signal Latch" at D10
            m_outlatch.op0.q_out_cb<u32_const_1>().set((write_line_delegate)nmi_mask_w).reg();
            m_outlatch.op0.q_out_cb<u32_const_2>().set((write_line_delegate)colorbank1_w).reg();
            m_outlatch.op0.q_out_cb<u32_const_3>().set((write_line_delegate)colorbank2_w).reg();
            m_outlatch.op0.q_out_cb<u32_const_4>().set((write_line_delegate)charbank1_w).reg();
            m_outlatch.op0.q_out_cb<u32_const_5>().set((write_line_delegate)charbank2_w).reg();
            m_outlatch.op0.q_out_cb<u32_const_6>().set((write_line_delegate)flip_screen_x_w).reg();
            m_outlatch.op0.q_out_cb<u32_const_7>().set((write_line_delegate)flip_screen_y_w).reg();

            WATCHDOG_TIMER(config, "watchdog");

            /* video hardware */
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_refresh_hz(60);
            m_screen.op0.set_vblank_time(ATTOSECONDS_IN_USEC(0)); //CLOCK/16/60
            m_screen.op0.set_size(32*8, 32*8);
            m_screen.op0.set_visarea(0*8, 32*8-1, 2*8, 30*8-1);
            m_screen.op0.set_screen_update(screen_update_fastfred);
            m_screen.op0.set_palette(m_palette);
            m_screen.op0.screen_vblank().set((write_line_delegate)vblank_irq).reg();

            GFXDECODE(config, "gfxdecode", m_palette, gfx_fastfred);

            PALETTE(config, m_palette, fastfred_palette, 32 * 8, 256);
            MCFG_VIDEO_START_OVERRIDE(config, video_start_fastfred);

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            GENERIC_LATCH_8(config, "soundlatch");

            AY8910(config, "ay8910.1", new XTAL(12_432_000) / 8).add_route(ALL_OUTPUTS, "mono", 0.25); /* 1.554 MHz; xtal from pcb pics, divider not verified */

            AY8910(config, "ay8910.2", new XTAL(12_432_000) / 8).add_route(ALL_OUTPUTS, "mono", 0.25); /* 1.554 MHz; xtal from pcb pics, divider not verified */
        }


        //void fastfred_state::jumpcoas(machine_config &config)

        //void fastfred_state::imago(machine_config &config)

        //#undef CLOCK
    }


    partial class fastfred : construct_ioport_helper
    {
        /***************************************************************************

          Game driver(s)

        ***************************************************************************/

        //ROM_START( fastfred )
        static readonly MemoryContainer<tiny_rom_entry> rom_fastfred = new MemoryContainer<tiny_rom_entry>()
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "ffr.01",       0x0000, 0x1000, CRC("15032c13") + SHA1("18ae84e87ac430e3f1cbc388ad16fb1d20aaba2f") ),
            ROM_LOAD( "ffr.02",       0x1000, 0x1000, CRC("f9642744") + SHA1("b086ad284593b7f2ad314ad5002c9a2b293b8103") ),
            ROM_LOAD( "ffr.03",       0x2000, 0x1000, CRC("f0919727") + SHA1("f16bc7de715acf0396818ce48ebe45b6a301b2cb") ),
            ROM_LOAD( "ffr.04",       0x3000, 0x1000, CRC("c778751e") + SHA1("7d9df82d2123e4e8565d8d50eed02daf455f96e8") ),
            ROM_LOAD( "ffr.05",       0x4000, 0x1000, CRC("cd6e160a") + SHA1("fd943aae88e350db192711ad0b75c0a9b21ef9c8") ),
            ROM_LOAD( "ffr.06",       0x5000, 0x1000, CRC("67f7f9b3") + SHA1("c862c04d97ffd6714c0da197a262e0a540175a65") ),
            ROM_LOAD( "ffr.07",       0x6000, 0x1000, CRC("2935c76a") + SHA1("acc2eec3c242dc904c5175e4b5b5fb025b956c17") ),
            ROM_LOAD( "ffr.08",       0x7000, 0x1000, CRC("0fb79e7b") + SHA1("82cc315708064bc498268abb8dbca2e36c3a0dcd") ),

            ROM_REGION( 0x10000, "audiocpu", 0 ),
            ROM_LOAD( "ffr.09",       0x0000, 0x1000, CRC("a1ec8d7e") + SHA1("5b4884381d0df79d3ed4246a9cf78f9b3bb14f79") ),
            ROM_LOAD( "ffr.10",       0x1000, 0x1000, CRC("460ca837") + SHA1("6d94f04e94ec15cbc5602bb303e9610ad20275fb") ),

            ROM_REGION( 0x6000, "gfx1", 0 ),
            ROM_LOAD( "ffr.14",       0x0000, 0x1000, CRC("e8a00e81") + SHA1("d93298f677baa4842f6e00b86fab099af1818467") ),
            ROM_LOAD( "ffr.17",       0x1000, 0x1000, CRC("701e0f01") + SHA1("f1f907386cf1f6676019cee56e6ee85d3117b8c3") ),
            ROM_LOAD( "ffr.15",       0x2000, 0x1000, CRC("b49b053f") + SHA1("b9f579d51fb9cc72158eef3d2d442c04099c8af1") ),
            ROM_LOAD( "ffr.18",       0x3000, 0x1000, CRC("4b208c8b") + SHA1("2cc7a1f93cc94fe54f16aa9e581bec91a7ad34ba") ),
            ROM_LOAD( "ffr.16",       0x4000, 0x1000, CRC("8c686bc2") + SHA1("73f63305209d58883f7b3cd8d766f8ad1bba6eb1") ),
            ROM_LOAD( "ffr.19",       0x5000, 0x1000, CRC("75b613f6") + SHA1("73d6d505f3ddfe2b897066d0f8e720d2718bf5d4") ),

            ROM_REGION( 0x3000, "gfx2", 0 ),
            ROM_LOAD( "ffr.11",       0x0000, 0x1000, CRC("0e1316d4") + SHA1("fa88311cdc6b6db9f892d7a2a6927acf03c8fc8d") ),
            ROM_LOAD( "ffr.12",       0x1000, 0x1000, CRC("94c06686") + SHA1("a40fa5b539da604750605ba6c8a6d1bac62f6ede") ),
            ROM_LOAD( "ffr.13",       0x2000, 0x1000, CRC("3fcfaa8e") + SHA1("2b1cf871ebf907fe41dcf1773b29066e4c20e2f3") ),

            ROM_REGION( 0x0300, "proms", 0 ),
            ROM_LOAD( "red.9h",       0x0000, 0x0100, CRC("b801e294") + SHA1("79926dc69c9088c2a5e5f15e260c644a90071ba0") ),
            ROM_LOAD( "green.8h",     0x0100, 0x0100, CRC("7da063d0") + SHA1("8e40174c4f6ba4a15edd89a6fe2b98a5e50531ff") ),
            ROM_LOAD( "blue.7h",      0x0200, 0x0100, CRC("85c05c18") + SHA1("a609a45c593fc6c491624076f7d65da55b5e603f") ),

            ROM_END,
        };


        //ROM_START( flyboy )
        static readonly MemoryContainer<tiny_rom_entry> rom_flyboy = new MemoryContainer<tiny_rom_entry>()
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "flyboy01.cpu", 0x0000, 0x1000, CRC("b05aa900") + SHA1("1ad394a438ddf96974b0b841d916766e45e8f3ba") ),
            ROM_LOAD( "flyboy02.cpu", 0x1000, 0x1000, CRC("474867f5") + SHA1("b352318eee71218155046bba9f032364e1213c02") ),
            ROM_LOAD( "rom3.cpu",     0x2000, 0x1000, CRC("d2f8f085") + SHA1("335d53b50c5ad8180bc7d77b808a638604eb7f39") ),
            ROM_LOAD( "rom4.cpu",     0x3000, 0x1000, CRC("19e5e15c") + SHA1("86c13a518cfb1666d69af73976c2fba89edf0393") ),
            ROM_LOAD( "flyboy05.cpu", 0x4000, 0x1000, CRC("207551f7") + SHA1("363f73f4a14e2018599f5e6e1ae75042d0b757d7") ),
            ROM_LOAD( "rom6.cpu",     0x5000, 0x1000, CRC("f5464c72") + SHA1("f4be4055964f523108bc98e3eb855ca1d8323e6f") ),
            ROM_LOAD( "rom7.cpu",     0x6000, 0x1000, CRC("50a1baff") + SHA1("469913e7652c6a334fb071e65cc00058b411527f") ),
            ROM_LOAD( "rom8.cpu",     0x7000, 0x1000, CRC("fe2ae95d") + SHA1("e44c36b7726892b4a360a7dc02820a3dbb21b398") ),

            ROM_REGION( 0x10000, "audiocpu", 0 ),
            ROM_LOAD( "rom9.cpu",     0x0000, 0x1000, CRC("5d05d1a0") + SHA1("cbf6144bf0b0686e4af41d8aeffd54c25f60eadc") ),
            ROM_LOAD( "rom10.cpu",    0x1000, 0x1000, CRC("7a28005b") + SHA1("71c5779aec3c40614db3ba2c6f7820e6592bf101") ),

            ROM_REGION( 0x6000, "gfx1", 0 ),
            ROM_LOAD( "rom14.rom",    0x0000, 0x1000, CRC("aeb07260") + SHA1("cf8fefa7b5b2413060ffe6a231033d443b4a4c6a") ),
            ROM_LOAD( "rom17.rom",    0x1000, 0x1000, CRC("a834325b") + SHA1("372054d525edba3e720162f9e2f31d6a1432c795") ),
            ROM_LOAD( "rom15.rom",    0x2000, 0x1000, CRC("c10c7ce2") + SHA1("bc4ffca80554dd6692b32fd82f93cb74f7f18e96") ),
            ROM_LOAD( "rom18.rom",    0x3000, 0x1000, CRC("2f196c80") + SHA1("9e1cb567aa3621e92e88e4ab4953c56e2baafb0b") ),
            ROM_LOAD( "rom16.rom",    0x4000, 0x1000, CRC("719246b1") + SHA1("ca5879289e3c7f04649407b448747fcff6a5ef47") ),
            ROM_LOAD( "rom19.rom",    0x5000, 0x1000, CRC("00c1c5d2") + SHA1("196e67ca21568b5aafc4befd9f9b6de0a677551b") ),

            ROM_REGION( 0x3000, "gfx2", 0 ),
            ROM_LOAD( "rom11.rom",    0x0000, 0x1000, CRC("ee7ec342") + SHA1("936ce03dd5ee05eea78d0e3308ce7d369397c361") ),
            ROM_LOAD( "rom12.rom",    0x1000, 0x1000, CRC("84d03124") + SHA1("92c7efc4bfe39aa47909071f9a90ec7e5c0fa1a1") ),
            ROM_LOAD( "rom13.rom",    0x2000, 0x1000, CRC("fcb33ff4") + SHA1("a76addec96b42a06df97eca37f3039f8a4727dfb") ),

            ROM_REGION( 0x0300, "proms", 0 ),
            ROM_LOAD( "red.9h",       0x0000, 0x0100, CRC("b801e294") + SHA1("79926dc69c9088c2a5e5f15e260c644a90071ba0") ),
            ROM_LOAD( "green.8h",     0x0100, 0x0100, CRC("7da063d0") + SHA1("8e40174c4f6ba4a15edd89a6fe2b98a5e50531ff") ),
            ROM_LOAD( "blue.7h",      0x0200, 0x0100, CRC("85c05c18") + SHA1("a609a45c593fc6c491624076f7d65da55b5e603f") ),

            ROM_END,
        };


        //ROM_START( flyboyb )

        //ROM_START( jumpcoas ) /* Kaneko FB-100A PCB, ROMs simply labeled 1 through 7 */

        //ROM_START( jumpcoasa ) /* Kaneko FB-100A PCB, ROMs simply labeled 1 through 7 */

        //ROM_START( jumpcoast ) /* Kaneko FB-100A PCB, ROMs simply labeled 1 through 7 */

        //ROM_START( boggy84 )

        //ROM_START( boggy84b )

        //ROM_START( boggy84b2 )

        //ROM_START( redrobin )

        //ROM_START( imago )

        //ROM_START( imagoa )
    }


    partial class fastfred_state : galaxold_state
    {
        public void init_flyboy()
        {
            m_maincpu.op0.memory().space(AS_PROGRAM).install_read_handler(0xc085, 0xc099, flyboy_custom1_io_r);
            m_maincpu.op0.memory().space(AS_PROGRAM).install_read_handler(0xc8fb, 0xc900, flyboy_custom2_io_r);
            m_hardware_type = 1;
        }


        //void fastfred_state::init_flyboyb()


        public void init_fastfred()
        {
            m_maincpu.op0.memory().space(AS_PROGRAM).install_read_handler(0xc800, 0xcfff, fastfred_custom_io_r);
            m_maincpu.op0.memory().space(AS_PROGRAM).nop_write(0xc800, 0xcfff);
            m_hardware_type = 1;
        }


        //void fastfred_state::init_jumpcoas()

        //void fastfred_state::init_boggy84b()

        //void fastfred_state::init_boggy84()

        //void fastfred_state::init_imago()
    }


    partial class fastfred : construct_ioport_helper
    {
        static void fastfred_state_fastfred(machine_config config, device_t device) { fastfred_state fastfred_state = (fastfred_state)device; fastfred_state.fastfred(config); }

        static void fastfred_state_init_flyboy(device_t owner) { fastfred_state fastfred_state = (fastfred_state)owner; fastfred_state.init_flyboy(); }
        static void fastfred_state_init_fastfred(device_t owner) { fastfred_state fastfred_state = (fastfred_state)owner; fastfred_state.init_fastfred(); }

        static fastfred m_fastfred = new fastfred();

        static device_t device_creator_fastfred(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new fastfred_state(mconfig, type, tag); }

        public static readonly game_driver driver_flyboy   = GAME(device_creator_fastfred, rom_flyboy,   "1982", "flyboy",   "0",      fastfred_state_fastfred, m_fastfred.construct_ioport_flyboy,   fastfred_state_init_flyboy,   ROT90, "Kaneko", "Fly-Boy", MACHINE_SUPPORTS_SAVE);
        public static readonly game_driver driver_fastfred = GAME(device_creator_fastfred, rom_fastfred, "1982", "fastfred", "flyboy", fastfred_state_fastfred, m_fastfred.construct_ioport_fastfred, fastfred_state_init_fastfred, ROT90, "Kaneko (Atari license)", "Fast Freddie", MACHINE_SUPPORTS_SAVE);
    }
}
