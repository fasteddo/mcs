// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using ListBytesPointer = mame.ListPointer<System.Byte>;
using offs_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;


namespace mame
{
    partial class _1942_state : driver_device
    {
        /* 12mhz OSC */
        static readonly XTAL MASTER_CLOCK     = new XTAL(12000000);
        static readonly XTAL MAIN_CPU_CLOCK   = MASTER_CLOCK/3;
        static readonly XTAL SOUND_CPU_CLOCK  = MASTER_CLOCK/4;
        static readonly XTAL AUDIO_CLOCK      = MASTER_CLOCK/8;
        /* 20mhz OSC - both Z80s are 4 MHz */
        //#define MASTER_CLOCK_1942P     (XTAL_20MHz)
        //#define MAIN_CPU_CLOCK_1942P      (MASTER_CLOCK_1942P/5)
        //#define SOUND_CPU_CLOCK_1942P     (MASTER_CLOCK_1942P/5)
        //#define AUDIO_CLOCK_1942P     (MASTER_CLOCK_1942P/16)


        //WRITE8_MEMBER(_1942_state::_1942_bankswitch_w)
        void _1942_bankswitch_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            membank("bank1").set_entry(data & 0x03);
        }


        //TIMER_DEVICE_CALLBACK_MEMBER(_1942_state::_1942_scanline)
        void _1942_scanline(timer_device timer, object ptr, int param)  // void *ptr, INT32 param) 
        {
            int scanline = param;

            if (scanline == 0x2c) // audio irq point 1
                m_audiocpu.target.set_input_line(0, HOLD_LINE);

            if (scanline == 0x6d) // periodic irq (writes to the soundlatch and drives freeze dip-switch), + audio irq point 2
            {
                m_maincpu.target.set_input_line_and_vector(0, HOLD_LINE, 0xcf);   /* Z80 - RST 08h */
                m_audiocpu.target.set_input_line(0, HOLD_LINE);
            }

            if (scanline == 0xaf) // audio irq point 3
                m_audiocpu.target.set_input_line(0, HOLD_LINE);

            if (scanline == 0xf0) // vblank-out irq, audio irq point 4
            {
                m_maincpu.target.set_input_line_and_vector(0, HOLD_LINE, 0xd7);   /* Z80 - RST 10h - vblank */
                m_audiocpu.target.set_input_line(0, HOLD_LINE);
            }
        }


        void _1942_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x7fff).rom();
            map.op(0x8000, 0xbfff).bankr("bank1");
            map.op(0xc000, 0xc000).portr("SYSTEM");
            map.op(0xc001, 0xc001).portr("P1");
            map.op(0xc002, 0xc002).portr("P2");
            map.op(0xc003, 0xc003).portr("DSWA");
            map.op(0xc004, 0xc004).portr("DSWB");
            map.op(0xc800, 0xc800).w(m_soundlatch.target, (space, offset, data, mem_mask) => { m_soundlatch.target.write(data); });
            map.op(0xc802, 0xc803).w(_1942_scroll_w);
            map.op(0xc804, 0xc804).w(_1942_c804_w);
            map.op(0xc805, 0xc805).w(_1942_palette_bank_w);
            map.op(0xc806, 0xc806).w(_1942_bankswitch_w);
            map.op(0xcc00, 0xcc7f).ram().share("spriteram");
            map.op(0xd000, 0xd7ff).ram().w(_1942_fgvideoram_w).share("fg_videoram");
            map.op(0xd800, 0xdbff).ram().w(_1942_bgvideoram_w).share("bg_videoram");
            map.op(0xe000, 0xefff).ram();
        }


        void sound_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x3fff).rom();
            map.op(0x4000, 0x47ff).ram();
            map.op(0x6000, 0x6000).r(m_soundlatch.target, (space, offset, mem_mask) => { return m_soundlatch.target.read(); });  //r(m_soundlatch, FUNC(generic_latch_8_device::read));
            map.op(0x8000, 0x8001).w("ay1", (space, offset, data, mem_mask) => { ((ay8910_device)subdevice("ay1")).address_data_w(offset, data); });  //w("ay1", FUNC(ay8910_device::address_data_w));
            map.op(0xc000, 0xc001).w("ay2", (space, offset, data, mem_mask) => { ((ay8910_device)subdevice("ay2")).address_data_w(offset, data); });  //w("ay2", FUNC(ay8910_device::address_data_w));
        }
    }


    partial class _1942 : global_object
    {
        //static INPUT_PORTS_START( 1942 )
        void construct_ioport_1942(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("SYSTEM");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x0c, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_SERVICE1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_COIN1 );

            PORT_START("P1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_BUTTON2 );
            PORT_BIT( 0xc0, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("P2");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_BUTTON2 ); PORT_COCKTAIL();
            PORT_BIT( 0xc0, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("DSWA");
            PORT_DIPNAME( 0x07, 0x07, DEF_STR( Coin_A ) );       PORT_DIPLOCATION("SWA:8,7,6");
            PORT_DIPSETTING(    0x01, DEF_STR( _4C_1C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x04, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x07, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x03, DEF_STR( _2C_3C ) );
            PORT_DIPSETTING(    0x06, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x05, DEF_STR( _1C_4C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Free_Play ) );
            PORT_DIPNAME( 0x08, 0x00, DEF_STR( Cabinet ) );      PORT_DIPLOCATION("SWA:5");
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x08, DEF_STR( Cocktail ) );
            PORT_DIPNAME( 0x30, 0x30, DEF_STR( Bonus_Life ) );   PORT_DIPLOCATION("SWA:4,3");
            PORT_DIPSETTING(    0x30, "20K 80K 80K+" );
            PORT_DIPSETTING(    0x20, "20K 100K 100K+" );
            PORT_DIPSETTING(    0x10, "30K 80K 80K+" );
            PORT_DIPSETTING(    0x00, "30K 100K 100K+" );
            PORT_DIPNAME( 0xc0, 0x40, DEF_STR( Lives ) );        PORT_DIPLOCATION("SWA:2,1");
            PORT_DIPSETTING(    0x80, "1" );
            PORT_DIPSETTING(    0x40, "2" );
            PORT_DIPSETTING(    0xc0, "3" );
            PORT_DIPSETTING(    0x00, "5" );

            PORT_START("DSWB");
            PORT_DIPNAME( 0x07, 0x07, DEF_STR( Coin_B ) );       PORT_DIPLOCATION("SWB:8,7,6");
            PORT_DIPSETTING(    0x01, DEF_STR( _4C_1C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x04, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x07, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x03, DEF_STR( _2C_3C ) );
            PORT_DIPSETTING(    0x06, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x05, DEF_STR( _1C_4C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Free_Play ) );
            PORT_SERVICE_DIPLOC(0x08, IP_ACTIVE_LOW, "SWB:5" );
            PORT_DIPNAME( 0x10, 0x10, DEF_STR( Flip_Screen ) );  PORT_DIPLOCATION("SWB:4");
            PORT_DIPSETTING(    0x10, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x60, 0x60, DEF_STR( Difficulty ) );   PORT_DIPLOCATION("SWB:3,2");
            PORT_DIPSETTING(    0x40, DEF_STR( Easy ) );
            PORT_DIPSETTING(    0x60, DEF_STR( Normal ) );
            PORT_DIPSETTING(    0x20, DEF_STR( Difficult ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Very_Difficult ) );
            PORT_DIPNAME( 0x80, 0x80, "Screen Stop" );           PORT_DIPLOCATION("SWB:1");
            PORT_DIPSETTING(    0x80, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );

            INPUT_PORTS_END();
        }
    }


    partial class _1942_state : driver_device
    {
        static readonly gfx_layout charlayout = new gfx_layout(
            8,8,
            RGN_FRAC(1,1),
            2,
            new UInt32[] { 4, 0 },
            new UInt32[] { 0, 1, 2, 3, 8+0, 8+1, 8+2, 8+3 },
            new UInt32[] { 0*16, 1*16, 2*16, 3*16, 4*16, 5*16, 6*16, 7*16 },
            16*8
        );


        static readonly gfx_layout tilelayout = new gfx_layout(
            16,16,
            RGN_FRAC(1,3),
            3,
            new UInt32[] { RGN_FRAC(0,3), RGN_FRAC(1,3), RGN_FRAC(2,3) },
            new UInt32[] { 0, 1, 2, 3, 4, 5, 6, 7,
                    16*8+0, 16*8+1, 16*8+2, 16*8+3, 16*8+4, 16*8+5, 16*8+6, 16*8+7 },
            new UInt32[] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
                    8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
            32*8
        );


        static readonly gfx_layout spritelayout = new gfx_layout(
            16,16,
            RGN_FRAC(1,2),
            4,
            new UInt32[] { RGN_FRAC(1,2)+4, RGN_FRAC(1,2)+0, 4, 0 },
            new UInt32[] { 0, 1, 2, 3, 8+0, 8+1, 8+2, 8+3,
                    16*16+0, 16*16+1, 16*16+2, 16*16+3, 16*16+8+0, 16*16+8+1, 16*16+8+2, 16*16+8+3 },
            new UInt32[] { 0*16, 1*16, 2*16, 3*16, 4*16, 5*16, 6*16, 7*16,
                    8*16, 9*16, 10*16, 11*16, 12*16, 13*16, 14*16, 15*16 },
            64*8
        );


        //static GFXDECODE_START( gfx_1942 )
        static readonly gfx_decode_entry [] gfx_1942 = new gfx_decode_entry[]
        {
            GFXDECODE_ENTRY( "gfx1", 0, charlayout,             0, 64 ),
            GFXDECODE_ENTRY( "gfx2", 0, tilelayout,          64*4, 4*32 ),
            GFXDECODE_ENTRY( "gfx3", 0, spritelayout, 64*4+4*32*8, 16 ),

            //GFXDECODE_END
        };


        protected override void machine_start()
        {
            save_item(m_palette_bank, "m_palette_bank");
            save_item(m_scroll, "m_scroll");
        }


        protected override void machine_reset()
        {
            m_palette_bank = 0;
            m_scroll[0] = 0;
            m_scroll[1] = 0;
        }


        public void _1942(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, MAIN_CPU_CLOCK);    /* 4 MHz ??? */
            m_maincpu.target.memory().set_addrmap(AS_PROGRAM, _1942_map);

            TIMER(config, "scantimer").configure_scanline(_1942_scanline, "screen", 0, 1);

            Z80(config, m_audiocpu, SOUND_CPU_CLOCK);  /* 3 MHz ??? */
            m_audiocpu.target.memory().set_addrmap(AS_PROGRAM, sound_map);

            /* video hardware */
            GFXDECODE(config, m_gfxdecode, m_palette, gfx_1942);

            PALETTE(config, m_palette, _1942_palette, 64*4+4*32*8+16*16, 256);

            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.target.set_refresh_hz(60);
            m_screen.target.set_vblank_time(ATTOSECONDS_IN_USEC(0));
            m_screen.target.set_size(32*8, 32*8);
            m_screen.target.set_visarea(0*8, 32*8-1, 2*8, 30*8-1);
            m_screen.target.set_screen_update(screen_update);
            m_screen.target.set_palette(m_palette);

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            GENERIC_LATCH_8(config, m_soundlatch);

            ay8910_device ay1 = AY8910(config, "ay1", AUDIO_CLOCK);  /* 1.5 MHz */
            ay1.set_flags(ay8910_global.AY8910_RESISTOR_OUTPUT);
            ay1.set_resistors_load((int)10000.0, (int)10000.0, (int)10000.0);
            ay1.disound.add_route(0, "snd_nl", 1.0, 0);
            ay1.disound.add_route(1, "snd_nl", 1.0, 1);
            ay1.disound.add_route(2, "snd_nl", 1.0, 2);

            ay8910_device ay2 = AY8910(config, "ay2", AUDIO_CLOCK);  /* 1.5 MHz */
            ay2.set_flags(ay8910_global.AY8910_RESISTOR_OUTPUT);
            ay2.set_resistors_load((int)10000.0, (int)10000.0, (int)10000.0);
            ay2.disound.add_route(0, "snd_nl", 1.0, 3);
            ay2.disound.add_route(1, "snd_nl", 1.0, 4);
            ay2.disound.add_route(2, "snd_nl", 1.0, 5);

            /* NETLIST configuration using internal AY8910 resistor values */

            /* Minimize resampling between ay8910 and netlist */
            NETLIST_SOUND(config, "snd_nl", AUDIO_CLOCK / 8 / 2)
                .set_source(netlist_1942)
                .disound.add_route(ALL_OUTPUTS, "mono", 5.0);
            NETLIST_STREAM_INPUT(config, "snd_nl:cin0", 0, "R_AY1_1.R");
            NETLIST_STREAM_INPUT(config, "snd_nl:cin1", 1, "R_AY1_2.R");
            NETLIST_STREAM_INPUT(config, "snd_nl:cin2", 2, "R_AY1_3.R");
            NETLIST_STREAM_INPUT(config, "snd_nl:cin3", 3, "R_AY2_1.R");
            NETLIST_STREAM_INPUT(config, "snd_nl:cin4", 4, "R_AY2_2.R");
            NETLIST_STREAM_INPUT(config, "snd_nl:cin5", 5, "R_AY2_3.R");

            NETLIST_STREAM_OUTPUT(config, "snd_nl:cout0", 0, "R1.1").set_mult_offset(70000.0, 0.0);
            //NETLIST_STREAM_OUTPUT(config, "snd_nl:cout0", 0, "VR.2");
        }
    }


    partial class _1942 : global_object
    {
        /***************************************************************************

          Game driver(s)

        ***************************************************************************/

        //ROM_START( 1942 )
        static readonly List<tiny_rom_entry> rom_1942 = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x20000, "maincpu", ROMREGION_ERASEFF ), /* 64k for code + 3*16k for the banked ROMs images */
            ROM_LOAD( "srb-03.m3", 0x00000, 0x4000, CRC("d9dafcc3") + SHA1("a089a9bc55fb7d6d0ac53f91b258396d5d62677a") ),
            ROM_LOAD( "srb-04.m4", 0x04000, 0x4000, CRC("da0cf924") + SHA1("856fbb302c9a4ec7850a26ab23dab8467f79bba4") ),
            ROM_LOAD( "srb-05.m5", 0x10000, 0x4000, CRC("d102911c") + SHA1("35ba1d82bd901940f61d8619273463d02fc0a952") ),
            ROM_LOAD( "srb-06.m6", 0x14000, 0x2000, CRC("466f8248") + SHA1("2ccc8fc59962d3001fbc10e8d2f20a254a74f251") ),
            ROM_LOAD( "srb-07.m7", 0x18000, 0x4000, CRC("0d31038c") + SHA1("b588eaf6fddd66ecb2d9832dc197f286f1ccd846") ),

            ROM_REGION( 0x10000, "audiocpu", 0 ),
            ROM_LOAD( "sr-01.c11", 0x0000, 0x4000, CRC("bd87f06b") + SHA1("821f85cf157f81117eeaba0c3cf0337eac357e58") ),

            ROM_REGION( 0x2000, "gfx1", 0 ),
            ROM_LOAD( "sr-02.f2", 0x0000, 0x2000, CRC("6ebca191") + SHA1("0dbddadde54a0ab66994c4a8726be05c6ca88a0e") ),    /* characters */

            ROM_REGION( 0xc000, "gfx2", 0 ),
            ROM_LOAD( "sr-08.a1", 0x0000, 0x2000, CRC("3884d9eb") + SHA1("5cbd9215fa5ba5a61208b383700adc4428521aed") ),    /* tiles */
            ROM_LOAD( "sr-09.a2", 0x2000, 0x2000, CRC("999cf6e0") + SHA1("5b8b685038ec98b781908b92eb7fb9506db68544") ),
            ROM_LOAD( "sr-10.a3", 0x4000, 0x2000, CRC("8edb273a") + SHA1("85fdd4c690ed31e6396e3c16aa02140ee7ea2d61") ),
            ROM_LOAD( "sr-11.a4", 0x6000, 0x2000, CRC("3a2726c3") + SHA1("187c92ef591febdcbd1d42ab850e0cbb62c00873") ),
            ROM_LOAD( "sr-12.a5", 0x8000, 0x2000, CRC("1bd3d8bb") + SHA1("ef4dce605eb4dc8035985a415315ec61c21419c6") ),
            ROM_LOAD( "sr-13.a6", 0xa000, 0x2000, CRC("658f02c4") + SHA1("f087d69e49e38cf3107350cde18fcf85a8fa04f0") ),

            ROM_REGION( 0x10000, "gfx3", 0 ),
            ROM_LOAD( "sr-14.l1", 0x00000, 0x4000, CRC("2528bec6") + SHA1("29f7719f18faad6bd1ec6735cc24e69168361470") ),   /* sprites */
            ROM_LOAD( "sr-15.l2", 0x04000, 0x4000, CRC("f89287aa") + SHA1("136fff6d2a4f48a488fc7c620213761459c3ada0") ),
            ROM_LOAD( "sr-16.n1", 0x08000, 0x4000, CRC("024418f8") + SHA1("145b8d5d6c8654cd090955a98f6dd8c8dbafe7c1") ),
            ROM_LOAD( "sr-17.n2", 0x0c000, 0x4000, CRC("e2c7e489") + SHA1("d4b5d575c021f58f6966df189df94e08c5b3621c") ),

            ROM_REGION( 0x0300, "palproms", 0 ),
            ROM_LOAD( "sb-5.e8",  0x0000, 0x0100, CRC("93ab8153") + SHA1("a792f24e5c0c3c4a6b436102e7a98199f878ece1") ),    /* red component */
            ROM_LOAD( "sb-6.e9",  0x0100, 0x0100, CRC("8ab44f7d") + SHA1("f74680a6a987d74b3acb32e6396f20e127874149") ),    /* green component */
            ROM_LOAD( "sb-7.e10", 0x0200, 0x0100, CRC("f4ade9a4") + SHA1("62ad31d31d183cce213b03168daa035083b2f28e") ),    /* blue component */

            ROM_REGION( 0x0100, "charprom", 0 ),
            ROM_LOAD( "sb-0.f1",  0x0000, 0x0100, CRC("6047d91b") + SHA1("1ce025f9524c1033e48c5294ee7d360f8bfebe8d") ),    /* char lookup table */

            ROM_REGION( 0x0100, "tileprom", 0 ),
            ROM_LOAD( "sb-4.d6",  0x0000, 0x0100, CRC("4858968d") + SHA1("20b5dbcaa1a4081b3139e7e2332d8fe3c9e55ed6") ),    /* tile lookup table */

            ROM_REGION( 0x0100, "sprprom", 0 ),
            ROM_LOAD( "sb-8.k3",  0x0000, 0x0100, CRC("f6fad943") + SHA1("b0a24ea7805272e8ebf72a99b08907bc00d5f82f") ),    /* sprite lookup table */

            ROM_REGION( 0x0400, "proms", 0 ),
            ROM_LOAD( "sb-2.d1",  0x0000, 0x0100, CRC("8bb8b3df") + SHA1("49de2819c4c92057fedcb20425282515d85829aa") ),    /* tile palette selector? (not used) */
            ROM_LOAD( "sb-3.d2",  0x0100, 0x0100, CRC("3b0c99af") + SHA1("38f30ac1e48632634e409f328ee3051b987de7ad") ),    /* tile palette selector? (not used) */
            ROM_LOAD( "sb-1.k6",  0x0200, 0x0100, CRC("712ac508") + SHA1("5349d722ab6733afdda65f6e0a98322f0d515e86") ),    /* interrupt timing (not used) */
            ROM_LOAD( "sb-9.m11", 0x0300, 0x0100, CRC("4921635c") + SHA1("aee37d6cdc36acf0f11ff5f93e7b16e4b12f6c39") ),    /* video timing? (not used) */

            ROM_END,
        };
    }


    partial class _1942_state : driver_device
    {
        public override void driver_init()
        {
            ListBytesPointer ROM = new ListBytesPointer(memregion("maincpu").base_());  //uint8_t *ROM = memregion("maincpu")->base();
            membank("bank1").configure_entries(0, 4, new ListBytesPointer(ROM, 0x10000), 0x4000);  //membank("bank1")->configure_entries(0, 4, &ROM[0x10000], 0x4000);
        }
    }


    partial class _1942 : global_object
    {
        static void _1942_state__1942(machine_config config, device_t device) { _1942_state _1942_state = (_1942_state)device; _1942_state._1942(config); }
        static void _1942_state_driver_init(device_t owner) { _1942_state _1942_state = (_1942_state)owner; _1942_state.driver_init(); }


        static _1942 m_1942 = new _1942();


        static device_t device_creator_1942(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new _1942_state(mconfig, type, tag); }


        //                                                     creator,             rom       YEAR,   NAME,   PARENT,  MACHINE,                 INPUT,                        INIT,                          MONITOR,COMPANY, FULLNAME,FLAGS
        public static readonly game_driver driver_1942 = GAME( device_creator_1942, rom_1942, "1984", "1942", null,    _1942._1942_state__1942, m_1942.construct_ioport_1942, _1942._1942_state_driver_init, ROT270, "Capcom", "1942 (Revision B)", MACHINE_SUPPORTS_SAVE);
    }
}
