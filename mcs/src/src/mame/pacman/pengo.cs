// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;

using static mame._74259_global;
using static mame.digfx_global;
using static mame.disound_global;
using static mame.drawgfx_global;
using static mame.driver_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.namco_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.segacrpt_device_global;
using static mame.speaker_global;
using static mame.watchdog_global;
using static mame.z80_global;


namespace mame
{
    partial class pengo_state : pacman_state
    {
        optional_shared_ptr<uint8_t> m_decrypted_opcodes;
        required_device<ls259_device> m_latch;


        public pengo_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_decrypted_opcodes = new optional_shared_ptr<uint8_t>(this, "decrypted_opcodes");
            m_latch = new required_device<ls259_device>(this, "latch");
        }


        /*************************************
         *
         *  Constants
         *
         *************************************/

        const u32 MASTER_CLOCK        = 18432000;

        const u32 PIXEL_CLOCK         = MASTER_CLOCK / 3;

        // H counts from 128->511, HBLANK starts at 128+16=144 and ends at 128+64+32+16=240
        const u16 HTOTAL              = 384;
        const u16 HBEND               = 0;     /*(96+16)*/
        const u16 HBSTART             = 288;   /*(16)*/

        const u16 VTOTAL              = 264;
        const u16 VBEND               = 0;     /*(16)*/
        const u16 VBSTART             = 224;   /*(224+16)*/


        /*************************************
         *  Main CPU memory handlers
         *************************************/
        //template <uint8_t Which>
        //WRITE_LINE_MEMBER(pengo_state::coin_counter_w)
        void coin_counter_w<uint8_t_Which>(int state)
            where uint8_t_Which : u8_const, new() 
        {
            throw new emu_unimplemented();
#if false
            machine().bookkeeping().coin_counter_w(Which, state);
#endif
        }


        void pengo_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map(0x0000, 0x7fff).rom();
            map(0x8000, 0x83ff).ram().w(FUNC(pengo_state::pacman_videoram_w)).share("videoram"); // video and color RAM, scratchpad RAM, sprite codes
            map(0x8400, 0x87ff).ram().w(FUNC(pengo_state::pacman_colorram_w)).share("colorram");
            map(0x8800, 0x8fef).ram().share("mainram");
            map(0x8ff0, 0x8fff).ram().share("spriteram");
            map(0x9000, 0x901f).w(m_namco_sound, FUNC(namco_device::pacman_sound_w));
            map(0x9020, 0x902f).writeonly().share("spriteram2");
            map(0x9000, 0x903f).portr("DSW1");
            map(0x9040, 0x907f).portr("DSW0");
            map(0x9040, 0x9047).w(m_latch, FUNC(ls259_device::write_d0));
            map(0x9070, 0x9070).w(m_watchdog, FUNC(watchdog_timer_device::reset_w));
            map(0x9080, 0x90bf).portr("IN1");
            map(0x90c0, 0x90ff).portr("IN0");
#endif
        }


        void decrypted_opcodes_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map(0x0000, 0x7fff).rom().share(m_decrypted_opcodes);
            map(0x8800, 0x8fef).ram().share("mainram");
            map(0x8ff0, 0x8fff).ram().share("spriteram");
#endif
        }


        //void pengo_state::jrpacmbl_map(address_map &map)
    }


    partial class pengo : construct_ioport_helper
    {
        /*************************************
         *  Port definitions
         *************************************/

        //static INPUT_PORTS_START( pengo )
        void construct_ioport_pengo(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            throw new emu_unimplemented();
#if false
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0")
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ) PORT_4WAY
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ) PORT_4WAY
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ) PORT_4WAY
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ) PORT_4WAY
            /* the coin input must stay low for no less than 2 frames and no more
               than 9 frames to pass the self test check.
               Moreover, this way we avoid the game freezing until the user releases
               the "coin" key. */
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_COIN1 ) PORT_IMPULSE(2)
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_COIN2 ) PORT_IMPULSE(2)
            /* Coin Aux doesn't need IMPULSE to pass the test, but it still needs it
               to avoid the freeze. */
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_COIN3 ) PORT_IMPULSE(2)
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_BUTTON1 )

            PORT_START("IN1")
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ) PORT_4WAY PORT_COCKTAIL
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ) PORT_4WAY PORT_COCKTAIL
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ) PORT_4WAY PORT_COCKTAIL
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ) PORT_4WAY PORT_COCKTAIL
            PORT_SERVICE_NO_TOGGLE(0x10, IP_ACTIVE_LOW)
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_START1 )
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_START2 )
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_BUTTON1 ) PORT_COCKTAIL

            PORT_START("DSW0")
            PORT_DIPNAME( 0x01, 0x00, DEF_STR( Bonus_Life ) )       PORT_DIPLOCATION("SW1:1")
            PORT_DIPSETTING(    0x00, "30000" )
            PORT_DIPSETTING(    0x01, "50000" )
            PORT_DIPNAME( 0x02, 0x00, DEF_STR( Demo_Sounds ) )      PORT_DIPLOCATION("SW1:2")
            PORT_DIPSETTING(    0x02, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x00, DEF_STR( On ) )
            PORT_DIPNAME( 0x04, 0x00, DEF_STR( Cabinet ) )          PORT_DIPLOCATION("SW1:3")
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) )
            PORT_DIPSETTING(    0x04, DEF_STR( Cocktail ) )
            PORT_DIPNAME( 0x18, 0x10, DEF_STR( Lives ) )            PORT_DIPLOCATION("SW1:4,5")
            PORT_DIPSETTING(    0x18, "2" )
            PORT_DIPSETTING(    0x10, "3" )
            PORT_DIPSETTING(    0x08, "4" )
            PORT_DIPSETTING(    0x00, "5" )
            PORT_DIPNAME( 0x20, 0x20, "Rack Test (Cheat)" ) PORT_CODE(KEYCODE_F1)   PORT_DIPLOCATION("SW1:6")
            PORT_DIPSETTING(    0x20, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x00, DEF_STR( On ) )
            PORT_DIPNAME( 0xc0, 0x80, DEF_STR( Difficulty ) )       PORT_DIPLOCATION("SW1:7,8")
            PORT_DIPSETTING(    0xc0, DEF_STR( Easy ) )
            PORT_DIPSETTING(    0x80, DEF_STR( Medium ) )
            PORT_DIPSETTING(    0x40, DEF_STR( Hard ) )
            PORT_DIPSETTING(    0x00, DEF_STR( Hardest ) )

            PORT_START("DSW1")
            PORT_DIPNAME( 0x0f, 0x0c, DEF_STR( Coin_A ) )           PORT_DIPLOCATION("SW2:1,2,3,4")
            PORT_DIPSETTING(    0x00, DEF_STR( 4C_1C ) )
            PORT_DIPSETTING(    0x08, DEF_STR( 3C_1C ) )
            PORT_DIPSETTING(    0x04, DEF_STR( 2C_1C ) )
            PORT_DIPSETTING(    0x09, "2 Coins/1 Credit 5/3" )
            PORT_DIPSETTING(    0x05, "2 Coins/1 Credit 4/3" )
            PORT_DIPSETTING(    0x0c, DEF_STR( 1C_1C ) )
            PORT_DIPSETTING(    0x0d, "1 Coin/1 Credit 5/6" )
            PORT_DIPSETTING(    0x03, "1 Coin/1 Credit 4/5" )
            PORT_DIPSETTING(    0x0b, "1 Coin/1 Credit 2/3" )
            PORT_DIPSETTING(    0x02, DEF_STR( 1C_2C ) )
            PORT_DIPSETTING(    0x07, "1 Coin/2 Credits 5/11" )
            PORT_DIPSETTING(    0x0f, "1 Coin/2 Credits 4/9" )
            PORT_DIPSETTING(    0x0a, DEF_STR( 1C_3C ) )
            PORT_DIPSETTING(    0x06, DEF_STR( 1C_4C ) )
            PORT_DIPSETTING(    0x0e, DEF_STR( 1C_5C ) )
            PORT_DIPSETTING(    0x01, DEF_STR( 1C_6C ) )
            PORT_DIPNAME( 0xf0, 0xc0, DEF_STR( Coin_B ) )           PORT_DIPLOCATION("SW2:5,6,7,8")
            PORT_DIPSETTING(    0x00, DEF_STR( 4C_1C ) )
            PORT_DIPSETTING(    0x80, DEF_STR( 3C_1C ) )
            PORT_DIPSETTING(    0x40, DEF_STR( 2C_1C ) )
            PORT_DIPSETTING(    0x90, "2 Coins/1 Credit 5/3" )
            PORT_DIPSETTING(    0x50, "2 Coins/1 Credit 4/3" )
            PORT_DIPSETTING(    0xc0, DEF_STR( 1C_1C ) )
            PORT_DIPSETTING(    0xd0, "1 Coin/1 Credit 5/6" )
            PORT_DIPSETTING(    0x30, "1 Coin/1 Credit 4/5" )
            PORT_DIPSETTING(    0xb0, "1 Coin/1 Credit 2/3" )
            PORT_DIPSETTING(    0x20, DEF_STR( 1C_2C ) )
            PORT_DIPSETTING(    0x70, "1 Coin/2 Credits 5/11" )
            PORT_DIPSETTING(    0xf0, "1 Coin/2 Credits 4/9" )
            PORT_DIPSETTING(    0xa0, DEF_STR( 1C_3C ) )
            PORT_DIPSETTING(    0x60, DEF_STR( 1C_4C ) )
            PORT_DIPSETTING(    0xe0, DEF_STR( 1C_5C ) )
            PORT_DIPSETTING(    0x10, DEF_STR( 1C_6C ) )

            INPUT_PORTS_END
#endif
        }


        //static INPUT_PORTS_START( jrpacmbl )
    }


    partial class pengo_state : pacman_state
    {
        /*************************************
         *  Graphics layouts
         *************************************/

        static readonly gfx_layout tilelayout = new gfx_layout
        (
            8,8,    // 8*8 characters
            RGN_FRAC(1,2),    // 256 characters
            2,  // 2 bits per pixel
            new u32[] { 0, 4 },   // the two bitplanes for 4 pixels are packed into one byte
            new u32[] { 8*8+0, 8*8+1, 8*8+2, 8*8+3, 0, 1, 2, 3 }, // bits are packed in groups of four
            new u32[] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
            16*8    // every char takes 16 bytes
        );


        static readonly gfx_layout spritelayout = new gfx_layout
        (
            16,16,  // 16*16 sprites
            RGN_FRAC(1,2),  // 64 sprites
            2,  // 2 bits per pixel
            new u32[] { 0, 4 },   // the two bitplanes for 4 pixels are packed into one byte
            new u32[] { 8*8, 8*8+1, 8*8+2, 8*8+3, 16*8+0, 16*8+1, 16*8+2, 16*8+3,
                    24*8+0, 24*8+1, 24*8+2, 24*8+3, 0, 1, 2, 3 },
            new u32[] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
                    32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64*8    // every sprite takes 64 bytes
        );


        //static GFXDECODE_START( gfx_pengo )
        static readonly gfx_decode_entry [] gfx_pengo =
        {
            GFXDECODE_ENTRY( "gfx1", 0x0000, tilelayout,   0, 128 ),
            GFXDECODE_ENTRY( "gfx1", 0x2000, spritelayout, 0, 128 ),

            //GFXDECODE_END
        };


        /*************************************
         *  Machine drivers
         *************************************/
        void pengo(machine_config config)
        {
            // basic machine hardware
            Z80(config, m_maincpu, MASTER_CLOCK / 6);
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, pengo_map);
            m_maincpu.op0.memory().set_addrmap(AS_OPCODES, decrypted_opcodes_map);

            LS259(config, m_latch); // U27
            m_latch.op0.q_out_cb<u32_const_0>().set((write_line_delegate)irq_mask_w).reg();
            m_latch.op0.q_out_cb<u32_const_1>().set("namco", (int state) => { ((namco_device)subdevice("namco")).sound_enable_w(state); }).reg();  //FUNC(namco_device::sound_enable_w));
            m_latch.op0.q_out_cb<u32_const_2>().set((write_line_delegate)pengo_palettebank_w).reg();
            m_latch.op0.q_out_cb<u32_const_3>().set((write_line_delegate)flipscreen_w).reg();
            m_latch.op0.q_out_cb<u32_const_4>().set((write_line_delegate)coin_counter_w<u8_const_0>).reg();
            m_latch.op0.q_out_cb<u32_const_5>().set((write_line_delegate)coin_counter_w<u8_const_1>).reg();
            m_latch.op0.q_out_cb<u32_const_6>().set((write_line_delegate)pengo_colortablebank_w).reg();
            m_latch.op0.q_out_cb<u32_const_7>().set((write_line_delegate)pengo_gfxbank_w).reg();

            WATCHDOG_TIMER(config, m_watchdog);

            // video hardware
            GFXDECODE(config, m_gfxdecode, m_palette, gfx_pengo);
            PALETTE(config, m_palette, pacman_palette, 128 * 4, 32);

            screen_device screen = SCREEN(config, "screen", SCREEN_TYPE_RASTER);
            screen.set_raw(PIXEL_CLOCK, HTOTAL, HBEND, HBSTART, VTOTAL, VBEND, VBSTART);
            screen.set_screen_update(screen_update_pacman);
            screen.set_palette(m_palette);
            screen.screen_vblank().set((write_line_delegate)vblank_irq).reg();

            MCFG_VIDEO_START_OVERRIDE(config, video_start_pengo);

            // sound hardware
            SPEAKER(config, "mono").front_center();

            NAMCO(config, m_namco_sound, MASTER_CLOCK / 6 / 32);
            m_namco_sound.op0.set_voices(3);
            m_namco_sound.op0.disound.add_route(ALL_OUTPUTS, "mono", 1.0);
        }


        //void pengo_state::pengou(machine_config &config)


        public void pengoe(machine_config config)
        {
            pengo(config);
            sega_315_5010_device maincpu = SEGA_315_5010(config.replace(), m_maincpu, MASTER_CLOCK / 6);
            maincpu.memory().set_addrmap(AS_PROGRAM, pengo_map);
            maincpu.memory().set_addrmap(AS_OPCODES, decrypted_opcodes_map);
            maincpu.set_decrypted_tag(":decrypted_opcodes");
        }


        //void pengo_state::jrpacmbl(machine_config &config)
    }


    partial class pengo : construct_ioport_helper
    {
        /*************************************
         *  ROM definitions
         *************************************/


        //ROM_START( pengo ) // Uses Sega 315-5010 encrypted Z80 CPU
        static readonly tiny_rom_entry [] rom_pengo =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "epr-1689c.ic8",  0x0000, 0x1000, CRC("f37066a8") + SHA1("0930de17a763a527057f60783a92662b09554426") ),
            ROM_LOAD( "epr-1690b.ic7",  0x1000, 0x1000, CRC("baf48143") + SHA1("4c97529e61eeca5d94938b1dfbeac41bf8cbaf7d") ),
            ROM_LOAD( "epr-1691b.ic15", 0x2000, 0x1000, CRC("adf0eba0") + SHA1("c8949fbdbfe5023ee17a789ef60205e834a76c81") ),
            ROM_LOAD( "epr-1692b.ic14", 0x3000, 0x1000, CRC("a086d60f") + SHA1("7079769d14dfe3873ffe29623ba0a93413706c6d") ),
            ROM_LOAD( "epr-1693b.ic21", 0x4000, 0x1000, CRC("b72084ec") + SHA1("c0508951c2ad8dc31481be8b3bfee2063e3fb0d7") ),
            ROM_LOAD( "epr-1694b.ic20", 0x5000, 0x1000, CRC("94194a89") + SHA1("7b47aec61593efd758e2a031f72a854bb0ba8af1") ),
            ROM_LOAD( "epr-5118b.ic32", 0x6000, 0x1000, CRC("af7b12c4") + SHA1("207ed466546f40ca60a38031b83aef61446902e2") ),
            ROM_LOAD( "epr-5119c.ic31", 0x7000, 0x1000, CRC("933950fe") + SHA1("fec7236b3dee2ea6e39c68440a6d2d9e3f72675a") ),

            ROM_REGION( 0x4000, "gfx1", 0 ),
            ROM_LOAD( "epr-1640.ic92",  0x0000, 0x1000, CRC("d7eec6cd") + SHA1("e542bcc28f292be9a0a29d949de726e0b55e654a") ), // tiles (bank 1)
            ROM_CONTINUE(               0x2000, 0x1000 ), // sprites (bank 1)
            ROM_LOAD( "epr-1695.ic105", 0x1000, 0x1000, CRC("5bfd26e9") + SHA1("bdec535e486b43a8f5550334beff423eeace10b2") ), // tiles (bank 2)
            ROM_CONTINUE(               0x3000, 0x1000 ), // sprites (bank 2)

            ROM_REGION( 0x0420, "proms", 0 ),
            ROM_LOAD( "pr1633.ic78",    0x0000, 0x0020, CRC("3a5844ec") + SHA1("680eab0e1204c9b74adc11588461651b474021bb") ), // color palette
            ROM_LOAD( "pr1634.ic88",    0x0020, 0x0400, CRC("766b139b") + SHA1("3fcd66610fcaee814953a115bf5e04788923181f") ), // color lookup

            ROM_REGION( 0x0200, "namco", 0 ),
            ROM_LOAD( "pr1635.ic51",    0x0000, 0x0100, CRC("c29dea27") + SHA1("563c9770028fe39188e62630711589d6ed242a66") ), // waveform
            ROM_LOAD( "pr1636.ic70",    0x0100, 0x0100, CRC("77245b66") + SHA1("0c4d0bee858b97632411c440bea6948a74759746") ), // timing - not used

            ROM_END,
        };


        //ROM_START( pengo2 ) // Uses Sega 315-5010 encrypted Z80 CPU

        //ROM_START( pengo2u ) // Sega game ID# 834-5092 PENGO REV.A

        //ROM_START( pengo3u ) //  Sega game ID# 834-5091 PENGO

        //ROM_START( pengo4 ) // Sega game ID# 834-5081 PENGO (REV.A of this set known to exist, but not currently dumped) - Uses Sega 315-5010 encrypted Z80 CPU

        //ROM_START( pengo5 ) // Sega game ID# 834-5081 PENGO - PCB has an additional label Bally N.E. - Uses Sega 315-5010 encrypted Z80 CPU

        //ROM_START( pengo6 ) // Sega game ID# 834-5078 PENGO  REV.A - Uses Sega 315-5007 encrypted Z80 CPU

        //ROM_START( pengob ) // based on pengo6, uses daughterboard with a Z80 plus additional circuitry to replicate Sega's 315-5007 encryption

        //ROM_START( penta ) // based on pengo6, uses daughterboard with a Z80 plus additional circuitry to replicate Sega's 315-5007 encryption

        //ROM_START( jrpacmbl )
    }


#if false
    /*************************************
     *  Driver initialization
     *************************************/


    void pengo_state::decode_pengo6(int end, int nodecend)
    {
    /*
        Sega 315-5007 decryption

        The values vary, but the translation mask is always laid out like this:

          0 1 2 3 4 5 6 7 8 9 a b c d e f
        0 A A B B A A B B C C D D C C D D
        1 A A B B A A B B C C D D C C D D
        2 E E F F E E F F G G H H G G H H
        3 E E F F E E F F G G H H G G H H
        4 A A B B A A B B C C D D C C D D
        5 A A B B A A B B C C D D C C D D
        6 E E F F E E F F G G H H G G H H
        7 E E F F E E F F G G H H G G H H
        8 H H G G H H G G F F E E F F E E
        9 H H G G H H G G F F E E F F E E
        a D D C C D D C C B B A A B B A A
        b D D C C D D C C B B A A B B A A
        c H H G G H H G G F F E E F F E E
        d H H G G H H G G F F E E F F E E
        e D D C C D D C C B B A A B B A A
        f D D C C D D C C B B A A B B A A

        (e.g. 0xc0 is XORed with H)
        therefore in the following tables we only keep track of A, B, C, D, E, F, G and H.
    */
        static const uint8_t data_xortable[2][8] =
        {
            { 0xa0,0x82,0x28,0x0a,0x82,0xa0,0x0a,0x28 },    // ...............0
            { 0x88,0x0a,0x82,0x00,0x88,0x0a,0x82,0x00 }     // ...............1
        };
        static const uint8_t opcode_xortable[8][8] =
        {
            { 0x02,0x08,0x2a,0x20,0x20,0x2a,0x08,0x02 },    // ...0...0...0....
            { 0x88,0x88,0x00,0x00,0x88,0x88,0x00,0x00 },    // ...0...0...1....
            { 0x88,0x0a,0x82,0x00,0xa0,0x22,0xaa,0x28 },    // ...0...1...0....
            { 0x88,0x0a,0x82,0x00,0xa0,0x22,0xaa,0x28 },    // ...0...1...1....
            { 0x2a,0x08,0x2a,0x08,0x8a,0xa8,0x8a,0xa8 },    // ...1...0...0....
            { 0x2a,0x08,0x2a,0x08,0x8a,0xa8,0x8a,0xa8 },    // ...1...0...1....
            { 0x88,0x0a,0x82,0x00,0xa0,0x22,0xaa,0x28 },    // ...1...1...0....
            { 0x88,0x0a,0x82,0x00,0xa0,0x22,0xaa,0x28 }     // ...1...1...1....
        };

        uint8_t *rom = memregion("maincpu")->base();

        for (int A = 0x0000; A < end; A++)
        {
            uint8_t src = rom[A];

            // pick the translation table from bit 0 of the address
            int i = A & 1;

            // pick the offset in the table from bits 1, 3 and 5 of the source data
            int j = ((src >> 1) & 1) + (((src >> 3) & 1) << 1) + (((src >> 5) & 1) << 2);
            // the bottom half of the translation table is the mirror image of the top
            if (src & 0x80) j = 7 - j;

            // decode the ROM data
            rom[A] = src ^ data_xortable[i][j];

            // now decode the opcodes
            // pick the translation table from bits 4, 8 and 12 of the address
            i = ((A >> 4) & 1) + (((A >> 8) & 1) << 1) + (((A >> 12) & 1) << 2);
            m_decrypted_opcodes[A] = src ^ opcode_xortable[i][j];
        }

        for (int A = end; A < nodecend; A++)
        {
            m_decrypted_opcodes[A] = rom[A];
        }
    }

    void pengo_state::init_pengo6()
    {
        decode_pengo6(0x8000, 0x8000);
    }

    } // Anonymous namespace
#endif


    public partial class pengo : construct_ioport_helper
    {
        /*************************************
         *  Game drivers
         *************************************/

        static void pengo_state_pengoe(machine_config config, device_t device) { pengo_state pengo_state = (pengo_state)device; pengo_state.pengoe(config); }

        static pengo m_pengo = new pengo();

        static device_t device_creator_pengo(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new pengo_state(mconfig, type, tag); }

        public static readonly game_driver driver_pengo = GAME(device_creator_pengo, rom_pengo, "1982", "pengo", "0", pengo_state_pengoe, m_pengo.construct_ioport_pengo, driver_device.empty_init, ROT90, "Sega", "Pengo (set 1 rev C, encrypted)", MACHINE_SUPPORTS_SAVE);
    }
}
