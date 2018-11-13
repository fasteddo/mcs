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
    public partial class _1942_state : driver_device
    {
        //WRITE8_MEMBER(_1942_state::_1942_bankswitch_w)
        public void _1942_bankswitch_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            throw new emu_unimplemented();
#if false
            membank("bank1")->set_entry(data & 0x03);
#endif
        }


        //TIMER_DEVICE_CALLBACK_MEMBER(_1942_state::_1942_scanline)
        public void _1942_scanline(timer_device timer, object ptr, int param)  // void *ptr, INT32 param) 
        {
            throw new emu_unimplemented();
#if false
            int scanline = param;

            if(scanline == 240) // vblank-out irq
                m_maincpu->set_input_line_and_vector(0, HOLD_LINE, 0xd7);   /* RST 10h - vblank */

            if(scanline == 0) // unknown irq event, presumably vblank-in or a periodic one (writes to the soundlatch and drives freeze dip-switch)
                m_maincpu->set_input_line_and_vector(0, HOLD_LINE, 0xcf);   /* RST 08h */
#endif
        }
    }


    public class _1942 : device_init_helpers
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


        void NLFILT(string RA, string R1, string C1, string R2)
        {
            NET_C(RA+".1", "V5");
            NET_C(RA+".2", R1+".1");
            NET_C(R1+".2", "GND");
            NET_C(R1+".1", C1+".1");
            NET_C(C1+".2", R2+".1");
        }


        //static NETLIST_START(nl_1942)
        void netlist_nl_1942(netlist.setup_t setup)
        {
            NETLIST_START(setup);

            /* Standard stuff */

            SOLVER("Solver", 48000);
            ANALOG_INPUT("V5", 5);
            PARAM("Solver"+".ACCURACY", 1e-6);
            PARAM("Solver"+".GS_LOOPS", 6);
            PARAM("Solver"+".SOR_FACTOR", 1.0);
            //PARAM(Solver.DYNAMIC_TS, 1)
            //PARAM(Solver.LTE, 5e-8)

            /* AY 8910 internal resistors */

            RES("R_AY1_1", 1000);
            RES("R_AY1_2", 1000);
            RES("R_AY1_3", 1000);
            RES("R_AY2_1", 1000);
            RES("R_AY2_2", 1000);
            RES("R_AY2_3", 1000);

            RES("R2", 220000);
            RES("R3", 220000);
            RES("R4", 220000);
            RES("R5", 220000);
            RES("R6", 220000);
            RES("R7", 220000);

            RES("R11", 10000);
            RES("R12", 10000);
            RES("R13", 10000);
            RES("R14", 10000);
            RES("R15", 10000);
            RES("R16", 10000);

            CAP("CC7", 10e-6);
            CAP("CC8", 10e-6);
            CAP("CC9", 10e-6);
            CAP("CC10", 10e-6);
            CAP("CC11", 10e-6);
            CAP("CC12", 10e-6);

            NLFILT("R_AY2_2", "R15", "CC8", "R3");
            NLFILT("R_AY2_3", "R13", "CC7", "R2");
            NLFILT("R_AY2_1", "R11", "CC9", "R4");

            NLFILT("R_AY1_2", "R14", "CC11", "R6");
            NLFILT("R_AY1_3", "R12", "CC10", "R5");
            NLFILT("R_AY1_1", "R16", "CC12", "R7");

            POT("VR", 2000);
            NET_C("VR"+".3", "GND");

            NET_C("R2"+".2", "VR"+".1");
            NET_C("R3"+".2", "VR"+".1");
            NET_C("R4"+".2", "VR"+".1");
            NET_C("R5"+".2", "VR"+".1");
            NET_C("R6"+".2", "VR"+".1");
            NET_C("R7"+".2", "VR"+".1");

            CAP("CC6", 10e-6);
            RES("R1", 100000);

            NET_C("CC6"+".1", "VR"+".2");
            NET_C("CC6"+".2", "R1"+".1");
            CAP("CC3", 220e-6);
            NET_C("R1"+".2", "CC3"+".1");
            NET_C("CC3"+".2", "GND");

            NETLIST_END();
        }


        //void _1942_state::_1942_map(address_map &map)
        void _1942_state__1942_map(address_map map, device_t device)
        {
            _1942_state _1942_state = (_1942_state)device;

            map.op(0x0000, 0x7fff).rom();
            map.op(0x8000, 0xbfff).bankr("bank1");
            map.op(0xc000, 0xc000).portr("SYSTEM");
            map.op(0xc001, 0xc001).portr("P1");
            map.op(0xc002, 0xc002).portr("P2");
            map.op(0xc003, 0xc003).portr("DSWA");
            map.op(0xc004, 0xc004).portr("DSWB");
            map.op(0xc800, 0xc800).w(_1942_state.soundlatch.target, _1942_state.generic_latch_8_device_write);
            map.op(0xc802, 0xc803).w(_1942_state._1942_scroll_w);
            map.op(0xc804, 0xc804).w(_1942_state._1942_c804_w);
            map.op(0xc805, 0xc805).w(_1942_state._1942_palette_bank_w);
            map.op(0xc806, 0xc806).w(_1942_state._1942_bankswitch_w);
            map.op(0xcc00, 0xcc7f).ram().share("spriteram");
            map.op(0xd000, 0xd7ff).ram().w(_1942_state._1942_fgvideoram_w).share("fg_videoram");
            map.op(0xd800, 0xdbff).ram().w(_1942_state._1942_bgvideoram_w).share("bg_videoram");
            map.op(0xe000, 0xefff).ram();
        }


        //void _1942_state::sound_map(address_map &map)
        void _1942_state_sound_map(address_map map, device_t device)
        {
            _1942_state _1942_state = (_1942_state)device;

            map.op(0x0000, 0x3fff).rom();
            map.op(0x4000, 0x47ff).ram();
            map.op(0x6000, 0x6000).r(_1942_state.soundlatch.target, _1942_state.generic_latch_8_device_read);
            map.op(0x8000, 0x8001).w("ay1", _1942_state.ay8910_device_address_data_w_ay1);
            map.op(0xc000, 0xc001).w("ay2", _1942_state.ay8910_device_address_data_w_ay2);
        }


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


        //MACHINE_CONFIG_START(_1942_state::_1942)
        void _1942_state__1942(machine_config config, device_t owner, device_t device)
        {
            MACHINE_CONFIG_START(config, owner, device);

            _1942_state _1942_state = (_1942_state)helper_owner;

            /* basic machine hardware */
            MCFG_DEVICE_ADD("maincpu", z80_device.Z80, MAIN_CPU_CLOCK);    /* 4 MHz ??? */
            MCFG_DEVICE_PROGRAM_MAP(_1942_state__1942_map);
            MCFG_TIMER_DRIVER_ADD_SCANLINE("scantimer", _1942_state._1942_scanline, "screen", 0, 1);

            MCFG_DEVICE_ADD("audiocpu", z80_device.Z80, SOUND_CPU_CLOCK);  /* 3 MHz ??? */
            MCFG_DEVICE_PROGRAM_MAP(_1942_state_sound_map);
            MCFG_DEVICE_PERIODIC_INT_DRIVER(_1942_state.irq0_line_hold, 4*60);


            /* video hardware */
            MCFG_DEVICE_ADD("gfxdecode", gfxdecode_device.GFXDECODE);//, "palette", gfx_1942);
            MCFG_DEVICE_ADD_gfxdecode_device("palette", gfx_1942);

            MCFG_PALETTE_ADD(_1942_state.palette, 64*4+4*32*8+16*16);
            MCFG_PALETTE_INDIRECT_ENTRIES(256);
            MCFG_PALETTE_INIT_OWNER(_1942_state.palette_init_1942);

            screen_device screen = SCREEN(config, "screen", screen_type_enum.SCREEN_TYPE_RASTER);
            screen.set_refresh_hz(60);
            screen.set_vblank_time(attotime.ATTOSECONDS_IN_USEC(0));
            screen.set_size(32*8, 32*8);
            screen.set_visarea(0*8, 32*8-1, 2*8, 30*8-1);
            screen.set_screen_update(_1942_state.screen_update);
            screen.set_palette(_1942_state.palette);

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            GENERIC_LATCH_8(config, _1942_state.soundlatch);

            MCFG_DEVICE_ADD("ay1", ay8910_device.AY8910, AUDIO_CLOCK);  /* 1.5 MHz */
            MCFG_AY8910_OUTPUT_TYPE(ay8910_global.AY8910_RESISTOR_OUTPUT);
            MCFG_AY8910_RES_LOADS((int)10000.0, (int)10000.0, (int)10000.0);

            MCFG_SOUND_ROUTE(0, "snd_nl", 1.0, 0);
            MCFG_SOUND_ROUTE(1, "snd_nl", 1.0, 1);
            MCFG_SOUND_ROUTE(2, "snd_nl", 1.0, 2);

            MCFG_DEVICE_ADD("ay2", ay8910_device.AY8910, AUDIO_CLOCK);  /* 1.5 MHz */
            MCFG_AY8910_OUTPUT_TYPE(ay8910_global.AY8910_RESISTOR_OUTPUT);
            MCFG_AY8910_RES_LOADS((int)10000.0, (int)10000.0, (int)10000.0);

            MCFG_SOUND_ROUTE(0, "snd_nl", 1.0, 3);
            MCFG_SOUND_ROUTE(1, "snd_nl", 1.0, 4);
            MCFG_SOUND_ROUTE(2, "snd_nl", 1.0, 5);

            /* NETLIST configuration using internal AY8910 resistor values */

            /* Minimize resampling between ay8910 and netlist */
            MCFG_DEVICE_ADD("snd_nl", netlist_mame_sound_device.NETLIST_SOUND, AUDIO_CLOCK / 8 / 2);
            MCFG_NETLIST_SETUP(netlist_nl_1942);
            MCFG_SOUND_ROUTE(ALL_OUTPUTS, "mono", 5.0);
            MCFG_NETLIST_STREAM_INPUT("snd_nl", 0, "R_AY1_1.R");
            MCFG_NETLIST_STREAM_INPUT("snd_nl", 1, "R_AY1_2.R");
            MCFG_NETLIST_STREAM_INPUT("snd_nl", 2, "R_AY1_3.R");
            MCFG_NETLIST_STREAM_INPUT("snd_nl", 3, "R_AY2_1.R");
            MCFG_NETLIST_STREAM_INPUT("snd_nl", 4, "R_AY2_2.R");
            MCFG_NETLIST_STREAM_INPUT("snd_nl", 5, "R_AY2_3.R");

            MCFG_NETLIST_STREAM_OUTPUT("snd_nl", 0, "R1.1");
            //MCFG_NETLIST_STREAM_OUTPUT("snd_nl", 0, "VR.2")
            MCFG_NETLIST_ANALOG_MULT_OFFSET(70000.0, 0.0);

            MACHINE_CONFIG_END();
        }


        /***************************************************************************

          Game driver(s)

        ***************************************************************************/

        //ROM_START( 1942 )
        static readonly List<tiny_rom_entry> rom_1942 = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x20000, "maincpu", romentry_global.ROMREGION_ERASEFF ), /* 64k for code + 3*16k for the banked ROMs images */
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

            ROM_REGION( 0x0a00, "proms", 0 ),
            ROM_LOAD( "sb-5.e8",  0x0000, 0x0100, CRC("93ab8153") + SHA1("a792f24e5c0c3c4a6b436102e7a98199f878ece1") ),    /* red component */
            ROM_LOAD( "sb-6.e9",  0x0100, 0x0100, CRC("8ab44f7d") + SHA1("f74680a6a987d74b3acb32e6396f20e127874149") ),    /* green component */
            ROM_LOAD( "sb-7.e10", 0x0200, 0x0100, CRC("f4ade9a4") + SHA1("62ad31d31d183cce213b03168daa035083b2f28e") ),    /* blue component */
            ROM_LOAD( "sb-0.f1",  0x0300, 0x0100, CRC("6047d91b") + SHA1("1ce025f9524c1033e48c5294ee7d360f8bfebe8d") ),    /* char lookup table */
            ROM_LOAD( "sb-4.d6",  0x0400, 0x0100, CRC("4858968d") + SHA1("20b5dbcaa1a4081b3139e7e2332d8fe3c9e55ed6") ),    /* tile lookup table */
            ROM_LOAD( "sb-8.k3",  0x0500, 0x0100, CRC("f6fad943") + SHA1("b0a24ea7805272e8ebf72a99b08907bc00d5f82f") ),    /* sprite lookup table */
            ROM_LOAD( "sb-2.d1",  0x0600, 0x0100, CRC("8bb8b3df") + SHA1("49de2819c4c92057fedcb20425282515d85829aa") ),    /* tile palette selector? (not used) */
            ROM_LOAD( "sb-3.d2",  0x0700, 0x0100, CRC("3b0c99af") + SHA1("38f30ac1e48632634e409f328ee3051b987de7ad") ),    /* tile palette selector? (not used) */
            ROM_LOAD( "sb-1.k6",  0x0800, 0x0100, CRC("712ac508") + SHA1("5349d722ab6733afdda65f6e0a98322f0d515e86") ),    /* interrupt timing (not used) */
            ROM_LOAD( "sb-9.m11", 0x0900, 0x0100, CRC("4921635c") + SHA1("aee37d6cdc36acf0f11ff5f93e7b16e4b12f6c39") ),    /* video timing? (not used) */

            ROM_END(),
        };


        static void _1942_state_driver_init(running_machine machine, device_t owner)
        {
            ListBytesPointer ROM = new ListBytesPointer(owner.memregion("gfx1").base_());  //uint8_t *ROM = memregion("maincpu")->base();
            owner.membank("bank1").configure_entries(0, 4, new ListBytesPointer(ROM, 0x10000), 0x4000);  //membank("bank1")->configure_entries(0, 4, &ROM[0x10000], 0x4000);
        }




        static _1942 m_1942 = new _1942();


        static device_t device_creator_1942(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new _1942_state(mconfig, type, tag); }


        //                                                     creator,             rom       YEAR,   NAME,       PARENT,  MACHINE,                        INPUT,                              INIT,                      MONITOR,COMPANY, FULLNAME,FLAGS
        public static readonly game_driver driver_1942 = GAME( device_creator_1942, rom_1942, "1984", "1942",     null,    m_1942._1942_state__1942,       m_1942.construct_ioport_1942,       _1942_state_driver_init,   ROT270, "Capcom", "1942 (Revision B)", MACHINE_SUPPORTS_SAVE );
    }
}
