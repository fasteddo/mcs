// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using u32 = System.UInt32;
using uint32_t = System.UInt32;


namespace mame
{
    partial class m52_state : driver_device
    {
        static readonly XTAL MASTER_CLOCK        = new XTAL(18432000);


        /*************************************
         *
         *  Memory maps
         *
         *************************************/

        void main_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x3fff).rom();
            map.op(0x8000, 0x83ff).ram().w(m52_videoram_w).share("videoram");
            map.op(0x8400, 0x87ff).ram().w(m52_colorram_w).share("colorram");
            map.op(0x8800, 0x8800).mirror(0x07ff).r(m52_protection_r);
            map.op(0xc800, 0xcbff).mirror(0x0400).writeonly().share("spriteram"); // only 0x100 of this used by video code?
            map.op(0xd000, 0xd000).mirror(0x07fc).w("irem_audio", (data) => { ((irem_audio_device)subdevice("irem_audio")).cmd_w(data); });  //FUNC(irem_audio_device::cmd_w));
            map.op(0xd001, 0xd001).mirror(0x07fc).w(m52_flipscreen_w);   /* + coin counters */
            map.op(0xd000, 0xd000).mirror(0x07f8).portr("IN0");
            map.op(0xd001, 0xd001).mirror(0x07f8).portr("IN1");
            map.op(0xd002, 0xd002).mirror(0x07f8).portr("IN2");
            map.op(0xd003, 0xd003).mirror(0x07f8).portr("DSW1");
            map.op(0xd004, 0xd004).mirror(0x07f8).portr("DSW2");
            map.op(0xe000, 0xe7ff).ram();
        }


        void main_portmap(address_map map, device_t device)
        {
            map.global_mask(0xff);
            map.op(0x00, 0x00).mirror(0x1f).w(m52_scroll_w);
            map.op(0x40, 0x40).mirror(0x1f).w(m52_bg1xpos_w);
            map.op(0x60, 0x60).mirror(0x1f).w(m52_bg1ypos_w);
            map.op(0x80, 0x80).mirror(0x1f).w(m52_bg2xpos_w);
            map.op(0xa0, 0xa0).mirror(0x1f).w(m52_bg2ypos_w);
            map.op(0xc0, 0xc0).mirror(0x1f).w(m52_bgcontrol_w);
        }
    }


    partial class m52 : construct_ioport_helper
    {
        /*************************************
         *
         *  Port definitions
         *
         *************************************/

        /* Same as m57, m58 and m62 (IREM Z80 hardware) */
        //static INPUT_PORTS_START( m52 )
        void construct_ioport_m52(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");
            /* Start 1 & 2 also restarts and freezes the game with stop mode on
               and are used in test mode to enter and esc the various tests */
            PORT_BIT( 0x01, g.IP_ACTIVE_LOW, g.IPT_START1 );
            PORT_BIT( 0x02, g.IP_ACTIVE_LOW, g.IPT_START2 );
            /* coin input must be active for 19 frames to be consistently recognized */
            PORT_BIT( 0x04, g.IP_ACTIVE_LOW, g.IPT_SERVICE1 ); PORT_IMPULSE(19);
            PORT_BIT( 0x08, g.IP_ACTIVE_LOW, g.IPT_COIN1 );
            PORT_BIT( 0xf0, g.IP_ACTIVE_LOW, g.IPT_UNUSED );

            PORT_START("IN1");
            PORT_BIT( 0x01, g.IP_ACTIVE_LOW, g.IPT_JOYSTICK_RIGHT ); PORT_8WAY();
            PORT_BIT( 0x02, g.IP_ACTIVE_LOW, g.IPT_JOYSTICK_LEFT );  PORT_8WAY();
            PORT_BIT( 0x04, g.IP_ACTIVE_LOW, g.IPT_JOYSTICK_DOWN );  PORT_8WAY();
            PORT_BIT( 0x08, g.IP_ACTIVE_LOW, g.IPT_JOYSTICK_UP );    PORT_8WAY();
            PORT_BIT( 0x10, g.IP_ACTIVE_LOW, g.IPT_UNUSED );
            PORT_BIT( 0x20, g.IP_ACTIVE_LOW, g.IPT_BUTTON2 );
            PORT_BIT( 0x40, g.IP_ACTIVE_LOW, g.IPT_UNUSED );
            PORT_BIT( 0x80, g.IP_ACTIVE_LOW, g.IPT_BUTTON1 );

            PORT_START("IN2");
            PORT_BIT( 0x01, g.IP_ACTIVE_LOW, g.IPT_JOYSTICK_RIGHT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, g.IP_ACTIVE_LOW, g.IPT_JOYSTICK_LEFT );  PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, g.IP_ACTIVE_LOW, g.IPT_JOYSTICK_DOWN );  PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, g.IP_ACTIVE_LOW, g.IPT_JOYSTICK_UP );    PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, g.IP_ACTIVE_LOW, g.IPT_COIN2 );
            PORT_BIT( 0x20, g.IP_ACTIVE_LOW, g.IPT_BUTTON2 ); PORT_COCKTAIL();
            PORT_BIT( 0x40, g.IP_ACTIVE_LOW, g.IPT_UNUSED );
            PORT_BIT( 0x80, g.IP_ACTIVE_LOW, g.IPT_BUTTON1 ); PORT_COCKTAIL();

            /* DSW1 is so different from game to game that it isn't included here */

            PORT_START("DSW2");
            PORT_DIPNAME( 0x01, 0x01, g.DEF_STR( g.Flip_Screen ) ); PORT_DIPLOCATION("SW2:1");
            PORT_DIPSETTING(    0x01, g.DEF_STR( g.Off ) );
            PORT_DIPSETTING(    0x00, g.DEF_STR( g.On ) );
            PORT_DIPNAME( 0x02, 0x00, g.DEF_STR( g.Cabinet ) ); PORT_DIPLOCATION("SW2:2");
            PORT_DIPSETTING(    0x00, g.DEF_STR( g.Upright ) );
            PORT_DIPSETTING(    0x02, g.DEF_STR( g.Cocktail ) );
            PORT_DIPNAME( 0x04, 0x04, "Coin Mode" ); PORT_DIPLOCATION("SW2:3");
            PORT_DIPSETTING(    0x04, "Mode 1" );
            PORT_DIPSETTING(    0x00, "Mode 2" );
            PORT_DIPUNKNOWN_DIPLOC( 0x08, g.IP_ACTIVE_LOW, "SW2:4" );
            PORT_DIPUNKNOWN_DIPLOC( 0x10, g.IP_ACTIVE_LOW, "SW2:5" );
            PORT_DIPUNKNOWN_DIPLOC( 0x20, g.IP_ACTIVE_LOW, "SW2:6" );
            PORT_DIPNAME( 0x40, 0x40, "Invulnerability (Cheat)"); PORT_DIPLOCATION("SW2:7");
            PORT_DIPSETTING(    0x40, g.DEF_STR( g.Off ) );
            PORT_DIPSETTING(    0x00, g.DEF_STR( g.On ) );
            PORT_SERVICE_DIPLOC( 0x80, g.IP_ACTIVE_LOW, "SW2:8" );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( mpatrol )
        void construct_ioport_mpatrol(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_INCLUDE(construct_ioport_m52, ref errorbuf);

            PORT_MODIFY("IN1");
            PORT_BIT( 0x01, g.IP_ACTIVE_LOW, g.IPT_JOYSTICK_RIGHT ); PORT_2WAY();
            PORT_BIT( 0x02, g.IP_ACTIVE_LOW, g.IPT_JOYSTICK_LEFT );  PORT_2WAY();
            PORT_BIT( 0x04, g.IP_ACTIVE_LOW, g.IPT_UNUSED );             /* IPT_JOYSTICK_DOWN */
            PORT_BIT( 0x08, g.IP_ACTIVE_LOW, g.IPT_UNUSED );             /* IPT_JOYSTICK_UP */

            PORT_MODIFY("IN2");
            PORT_BIT( 0x01, g.IP_ACTIVE_LOW, g.IPT_JOYSTICK_RIGHT ); PORT_2WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, g.IP_ACTIVE_LOW, g.IPT_JOYSTICK_LEFT );  PORT_2WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, g.IP_ACTIVE_LOW, g.IPT_UNUSED );             /* IPT_JOYSTICK_DOWN PORT_COCKTAIL */
            PORT_BIT( 0x08, g.IP_ACTIVE_LOW, g.IPT_UNUSED );             /* IPT_JOYSTICK_UP   PORT_COCKTAIL */

            PORT_MODIFY("DSW2");
            PORT_DIPUNUSED_DIPLOC( 0x08, g.IP_ACTIVE_LOW, "SW2:4" );   /* should have been "slow motion" but no conditional jump at 0x00c3 */
            /* In stop mode, press 2 to stop and 1 to restart */
            PORT_DIPNAME( 0x10, 0x10, "Stop Mode (Cheat)"); PORT_DIPLOCATION("SW2:5");
            PORT_DIPSETTING(    0x10, g.DEF_STR( g.Off ) );
            PORT_DIPSETTING(    0x00, g.DEF_STR( g.On ) );
            PORT_DIPNAME( 0x20, 0x20, "Sector Selection (Cheat)"); PORT_DIPLOCATION("SW2:6");
            PORT_DIPSETTING(    0x20, g.DEF_STR( g.Off ) );
            PORT_DIPSETTING(    0x00, g.DEF_STR( g.On ) );

            PORT_START("DSW1");
            PORT_DIPNAME( 0x03, 0x02, g.DEF_STR( g.Lives ) ); PORT_DIPLOCATION("SW1:1,2");
            PORT_DIPSETTING(    0x00, "1" );
            PORT_DIPSETTING(    0x01, "2" );
            PORT_DIPSETTING(    0x02, "3" );
            PORT_DIPSETTING(    0x03, "5" );
            PORT_DIPNAME( 0x0c, 0x0c, g.DEF_STR( g.Bonus_Life ) ); PORT_DIPLOCATION("SW1:3,4");
            PORT_DIPSETTING(    0x0c, "10000 30000 50000" );
            PORT_DIPSETTING(    0x08, "20000 40000 60000" );
            PORT_DIPSETTING(    0x04, "10000" );
            PORT_DIPSETTING(    0x00, g.DEF_STR( g.None ) );
            iremipt_global.IREM_Z80_COINAGE_TYPE_1_LOC(this, "SW1");

            INPUT_PORTS_END();
        }
    }


    partial class m52_state : driver_device
    {
        /*************************************
        *
        *  Graphics layouts
        *
        *************************************/

        static readonly gfx_layout charlayout = new gfx_layout(
            8,8,
            g.RGN_FRAC(1,2),
            2,
            new u32[] { g.RGN_FRAC(0,2), g.RGN_FRAC(1,2) },
            g.STEP8(0,1),
            g.STEP8(0,8),
            8 * 8
        );


        static readonly gfx_layout spritelayout = new gfx_layout(
            16,16,
            g.RGN_FRAC(1,3),
            3,
            new u32[] { g.RGN_FRAC(2,3), g.RGN_FRAC(0,3), g.RGN_FRAC(1,3) },
            g.ArrayCombineUInt32(g.STEP8(0,1), g.STEP8(16 * 8,1)),
            g.STEP16(0,8),
            32 * 8
        );


        static readonly uint32_t [] bgcharlayout_xoffset = g.ArrayCombineUInt32(
            g.STEP4(0x000,1), g.STEP4(0x008,1), g.STEP4(0x010,1), g.STEP4(0x018,1),
            g.STEP4(0x020,1), g.STEP4(0x028,1), g.STEP4(0x030,1), g.STEP4(0x038,1),
            g.STEP4(0x040,1), g.STEP4(0x048,1), g.STEP4(0x050,1), g.STEP4(0x058,1),
            g.STEP4(0x060,1), g.STEP4(0x068,1), g.STEP4(0x070,1), g.STEP4(0x078,1),
            g.STEP4(0x080,1), g.STEP4(0x088,1), g.STEP4(0x090,1), g.STEP4(0x098,1),
            g.STEP4(0x0a0,1), g.STEP4(0x0a8,1), g.STEP4(0x0b0,1), g.STEP4(0x0b8,1),
            g.STEP4(0x0c0,1), g.STEP4(0x0c8,1), g.STEP4(0x0d0,1), g.STEP4(0x0d8,1),
            g.STEP4(0x0e0,1), g.STEP4(0x0e8,1), g.STEP4(0x0f0,1), g.STEP4(0x0f8,1),
            g.STEP4(0x100,1), g.STEP4(0x108,1), g.STEP4(0x110,1), g.STEP4(0x118,1),
            g.STEP4(0x120,1), g.STEP4(0x128,1), g.STEP4(0x130,1), g.STEP4(0x138,1),
            g.STEP4(0x140,1), g.STEP4(0x148,1), g.STEP4(0x150,1), g.STEP4(0x158,1),
            g.STEP4(0x160,1), g.STEP4(0x168,1), g.STEP4(0x170,1), g.STEP4(0x178,1),
            g.STEP4(0x180,1), g.STEP4(0x188,1), g.STEP4(0x190,1), g.STEP4(0x198,1),
            g.STEP4(0x1a0,1), g.STEP4(0x1a8,1), g.STEP4(0x1b0,1), g.STEP4(0x1b8,1),
            g.STEP4(0x1c0,1), g.STEP4(0x1c8,1), g.STEP4(0x1d0,1), g.STEP4(0x1d8,1),
            g.STEP4(0x1e0,1), g.STEP4(0x1e8,1), g.STEP4(0x1f0,1), g.STEP4(0x1f8,1));


        static readonly uint32_t [] bgcharlayout_yoffset = g.ArrayCombineUInt32(
            g.STEP32(0x0000,0x200), g.STEP32(0x4000,0x200), g.STEP32(0x8000,0x200), g.STEP32(0xc000,0x200));


        static readonly gfx_layout bgcharlayout = new gfx_layout(
            256, 128, /* 256x64 image format */
            1,       /* 1 image */
            2,       /* 2 bits per pixel */
            new u32[] { 4, 0 },       /* the two bitplanes for 4 pixels are packed into one byte */
            g.EXTENDED_XOFFS,
            g.EXTENDED_YOFFS,
            0x8000,
            new MemoryContainer<u32>(bgcharlayout_xoffset),
            new MemoryContainer<u32>(bgcharlayout_yoffset)
        );


        //static GFXDECODE_START(gfx_m52_sp)
        static readonly gfx_decode_entry [] gfx_m52_sp = new gfx_decode_entry[]
        {
            g.GFXDECODE_ENTRY("sp", 0x0000, spritelayout, 0, 16),

            //GFXDECODE_END
        };

        //static GFXDECODE_START(gfx_m52_tx)
        static readonly gfx_decode_entry [] gfx_m52_tx = new gfx_decode_entry[]
        {
            g.GFXDECODE_ENTRY("tx", 0x0000, charlayout, 0, 128),

            //GFXDECODE_END
        };

        //static GFXDECODE_START(gfx_m52_bg)
        static readonly gfx_decode_entry [] gfx_m52_bg = new gfx_decode_entry[]
        {
            g.GFXDECODE_ENTRY("bg0", 0x0000, bgcharlayout, 0 * 4, 1),
            g.GFXDECODE_ENTRY("bg1", 0x0000, bgcharlayout, 1 * 4, 1),
            g.GFXDECODE_ENTRY("bg2", 0x0000, bgcharlayout, 2 * 4, 1),

            //GFXDECODE_END
        };


        protected override void machine_reset()
        {
            m_bg1xpos = 0;
            m_bg1ypos = 0;
            m_bg2xpos = 0;
            m_bg2ypos = 0;
            m_bgcontrol = 0;
        }


        /*************************************
        *
        *  Machine drivers
        *
        *************************************/

        public void m52(machine_config config)
        {
            /* basic machine hardware */
            g.Z80(config, m_maincpu, MASTER_CLOCK / 6);
            m_maincpu.op[0].memory().set_addrmap(g.AS_PROGRAM, main_map);
            m_maincpu.op[0].memory().set_addrmap(g.AS_IO, main_portmap);
            m_maincpu.op[0].execute().set_vblank_int("screen", irq0_line_hold);

            /* video hardware */
            g.PALETTE(config, m_sp_palette).set_entries(256, 32);
            g.GFXDECODE(config, m_sp_gfxdecode, m_sp_palette, gfx_m52_sp);

            g.PALETTE(config, m_tx_palette).set_entries(512);
            g.GFXDECODE(config, m_tx_gfxdecode, m_tx_palette, gfx_m52_tx);

            g.PALETTE(config, m_bg_palette).set_entries(3 * 4, 32);
            g.GFXDECODE(config, m_bg_gfxdecode, m_bg_palette, gfx_m52_bg);

            g.SCREEN(config, m_screen, g.SCREEN_TYPE_RASTER);
            m_screen.op[0].set_raw(MASTER_CLOCK / 3, 384, 136, 376, 282, 22, 274);
            m_screen.op[0].set_screen_update(screen_update_m52);

            /* sound hardware */
            //m52_sound_c_audio(config);
            g.IREM_M52_SOUNDC_AUDIO(config, "irem_audio", 0);
        }
    }


    partial class m52 : construct_ioport_helper
    {
        /*************************************
        *
        *  ROM definitions
        *
        *************************************/

        //ROM_START(mpatrol)
        static readonly MemoryContainer<tiny_rom_entry> rom_mpatrol = new MemoryContainer<tiny_rom_entry>()
        {
            g.ROM_REGION(0x10000, "maincpu", 0),
            g.ROM_LOAD("mpa-1.3m", 0x0000, 0x1000, g.CRC("5873a860") + g.SHA1("8c03726d6e049c3edbc277440184e31679f78258")),
            g.ROM_LOAD("mpa-2.3l", 0x1000, 0x1000, g.CRC("f4b85974") + g.SHA1("dfb2efb57378a20af6f20569f4360cde95596f93")),
            g.ROM_LOAD("mpa-3.3k", 0x2000, 0x1000, g.CRC("2e1a598c") + g.SHA1("112c3c9678db8a8540a8df3708020c87fd10c91b")),
            g.ROM_LOAD("mpa-4.3j", 0x3000, 0x1000, g.CRC("dd05b587") + g.SHA1("727961b0dafa4a96b580d51013336db2a18aff1e")),

            g.ROM_REGION(0x8000, "irem_audio:iremsound", 0),
            g.ROM_LOAD("mp-s1.1a", 0x7000, 0x1000, g.CRC("561d3108") + g.SHA1("4998c68a9e9a8002251fa8f07aa1082444a9dc80")),

            g.ROM_REGION(0x2000, "tx", 0),
            g.ROM_LOAD("mpe-5.3e", 0x0000, 0x1000, g.CRC("e3ee7f75") + g.SHA1("b03d0d56150d3e9da4a4c871338097b4f450b649")),
            g.ROM_LOAD("mpe-4.3f", 0x1000, 0x1000, g.CRC("cca6d023") + g.SHA1("fecb3059fb09897a096add9452b50aec55c07545")),

            /* 0x2000-0x2fff is intentionally left as 0x00 fill, unused bitplane */
            g.ROM_REGION(0x3000, "sp", g.ROMREGION_ERASE00),
            g.ROM_LOAD("mpb-2.3m", 0x0000, 0x1000, g.CRC("707ace5e") + g.SHA1("93c682e13e74bce29ced3a87bffb29569c114c3b")),
            g.ROM_LOAD("mpb-1.3n", 0x1000, 0x1000, g.CRC("9b72133a") + g.SHA1("1393ef92ae1ad58a4b62ca1660c0793d30a8b5e2")),

            /* 0x1000-01fff is intentionally left as 0xff fill for the bg regions */
            g.ROM_REGION(0x2000, "bg0", g.ROMREGION_ERASEFF),
            g.ROM_LOAD("mpe-1.3l", 0x0000, 0x1000, g.CRC("c46a7f72") + g.SHA1("8bb7c9acaf6833fb6c0575b015991b873a305a84")),

            g.ROM_REGION(0x2000, "bg1", g.ROMREGION_ERASEFF),
            g.ROM_LOAD("mpe-2.3k", 0x0000, 0x1000, g.CRC("c7aa1fb0") + g.SHA1("14c6c76e1d0db2c0745e5d6d33ea6945fac8e9ee")),

            g.ROM_REGION(0x2000, "bg2", g.ROMREGION_ERASEFF),
            g.ROM_LOAD("mpe-3.3h", 0x0000, 0x1000, g.CRC("a0919392") + g.SHA1("8a090cb8d483a3d67c7360058e3fdd70e151cd62")),

            g.ROM_REGION(0x0200, "tx_pal", 0),
            g.ROM_LOAD("mpc-4.2a", 0x0000, 0x0200, g.CRC("07f99284") + g.SHA1("dfc52958f2520e1ce4446dd4c84c91413bbacf76")),

            g.ROM_REGION(0x0020, "bg_pal", 0),
            g.ROM_LOAD("mpc-3.1m", 0x0000, 0x0020, g.CRC("6a57eff2") + g.SHA1("2d1c12dab5915da2ccd466e39436c88be434d634")),

            g.ROM_REGION(0x0020, "spr_pal", 0),
            g.ROM_LOAD("mpc-1.1f", 0x0000, 0x0020, g.CRC("26979b13") + g.SHA1("8c41a8cce4f3384c392a9f7a223a50d7be0e14a5")),

            g.ROM_REGION(0x0100, "spr_clut", 0),
            g.ROM_LOAD("mpc-2.2h", 0x0000, 0x0100, g.CRC("7ae4cd97") + g.SHA1("bc0662fac82ffe65f02092d912b2c2b0c7a8ac2b")),

            g.ROM_REGION(0x0200, "unkprom", 0), // PROM is on bottom board of 4-board stack
            g.ROM_LOAD("mp_7621-5.7h", 0x0000, 0x0200, g.CRC("cf1fd9d0") + g.SHA1("f9575bc59bf21dfecd10133264835e02890562f8")),

            g.ROM_END,
        };



        /*************************************
        *
        *  Game drivers
        *
        *************************************/

        static void m52_state_m52(machine_config config, device_t device) { m52_state m52_state = (m52_state)device; m52_state.m52(config); }


        static m52 m_m52 = new m52();


        static device_t device_creator_m52(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new m52_state(mconfig, (device_type)type, tag); }


        //                                                         creator,            rom          YEAR,   NAME,       PARENT,  MACHINE,        INPUT,                           INIT,                     MONITOR, COMPANY, FULLNAME,      FLAGS
        public static readonly game_driver driver_mpatrol = g.GAME(device_creator_m52, rom_mpatrol, "1982", "mpatrol",  "0",     m52_state_m52,  m_m52.construct_ioport_mpatrol,  driver_device.empty_init, g.ROT0,  "Irem",  "Moon Patrol", g.MACHINE_SUPPORTS_SAVE);
    }
}
