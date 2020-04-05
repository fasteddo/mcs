// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
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
            map.op(0xd000, 0xd000).mirror(0x07fc).w("irem_audio", (space, offset, data, mem_mask) => { ((irem_audio_device)subdevice("irem_audio")).cmd_w(space, offset, data, mem_mask); });  //FUNC(irem_audio_device::cmd_w));
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


    partial class m52 : global_object
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
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_START2 );
            /* coin input must be active for 19 frames to be consistently recognized */
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_SERVICE1 ); PORT_IMPULSE(19);
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0xf0, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT );  PORT_8WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN );  PORT_8WAY();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_UP );    PORT_8WAY();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_BUTTON2 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_BUTTON1 );

            PORT_START("IN2");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT );  PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN );  PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_UP );    PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_BUTTON2 ); PORT_COCKTAIL();
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();

            /* DSW1 is so different from game to game that it isn't included here */

            PORT_START("DSW2");
            PORT_DIPNAME( 0x01, 0x01, DEF_STR( Flip_Screen ) ); PORT_DIPLOCATION("SW2:1");
            PORT_DIPSETTING(    0x01, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x02, 0x00, DEF_STR( Cabinet ) ); PORT_DIPLOCATION("SW2:2");
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x02, DEF_STR( Cocktail ) );
            PORT_DIPNAME( 0x04, 0x04, "Coin Mode" ); PORT_DIPLOCATION("SW2:3");
            PORT_DIPSETTING(    0x04, "Mode 1" );
            PORT_DIPSETTING(    0x00, "Mode 2" );
            PORT_DIPUNKNOWN_DIPLOC( 0x08, IP_ACTIVE_LOW, "SW2:4" );
            PORT_DIPUNKNOWN_DIPLOC( 0x10, IP_ACTIVE_LOW, "SW2:5" );
            PORT_DIPUNKNOWN_DIPLOC( 0x20, IP_ACTIVE_LOW, "SW2:6" );
            PORT_DIPNAME( 0x40, 0x40, "Invulnerability (Cheat)"); PORT_DIPLOCATION("SW2:7");
            PORT_DIPSETTING(    0x40, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_SERVICE_DIPLOC( 0x80, IP_ACTIVE_LOW, "SW2:8" );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( mpatrol )
        void construct_ioport_mpatrol(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_INCLUDE(construct_ioport_m52, ref errorbuf);

            PORT_MODIFY("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_2WAY();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT );  PORT_2WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_UNUSED );             /* IPT_JOYSTICK_DOWN */
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_UNUSED );             /* IPT_JOYSTICK_UP */

            PORT_MODIFY("IN2");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_2WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT );  PORT_2WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_UNUSED );             /* IPT_JOYSTICK_DOWN PORT_COCKTAIL */
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_UNUSED );             /* IPT_JOYSTICK_UP   PORT_COCKTAIL */

            PORT_MODIFY("DSW2");
            PORT_DIPUNUSED_DIPLOC( 0x08, IP_ACTIVE_LOW, "SW2:4" );   /* should have been "slow motion" but no conditional jump at 0x00c3 */
            /* In stop mode, press 2 to stop and 1 to restart */
            PORT_DIPNAME( 0x10, 0x10, "Stop Mode (Cheat)"); PORT_DIPLOCATION("SW2:5");
            PORT_DIPSETTING(    0x10, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x20, 0x20, "Sector Selection (Cheat)"); PORT_DIPLOCATION("SW2:6");
            PORT_DIPSETTING(    0x20, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );

            PORT_START("DSW1");
            PORT_DIPNAME( 0x03, 0x02, DEF_STR( Lives ) ); PORT_DIPLOCATION("SW1:1,2");
            PORT_DIPSETTING(    0x00, "1" );
            PORT_DIPSETTING(    0x01, "2" );
            PORT_DIPSETTING(    0x02, "3" );
            PORT_DIPSETTING(    0x03, "5" );
            PORT_DIPNAME( 0x0c, 0x0c, DEF_STR( Bonus_Life ) ); PORT_DIPLOCATION("SW1:3,4");
            PORT_DIPSETTING(    0x0c, "10000 30000 50000" );
            PORT_DIPSETTING(    0x08, "20000 40000 60000" );
            PORT_DIPSETTING(    0x04, "10000" );
            PORT_DIPSETTING(    0x00, DEF_STR( None ) );
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
            RGN_FRAC(1,2),
            2,
            new u32[] { RGN_FRAC(0,2), RGN_FRAC(1,2) },
            STEP8(0,1),
            STEP8(0,8),
            8 * 8
        );


        static readonly gfx_layout spritelayout = new gfx_layout(
            16,16,
            RGN_FRAC(1,3),
            3,
            new u32[] { RGN_FRAC(2,3), RGN_FRAC(0,3), RGN_FRAC(1,3) },
            ArrayCombineUInt32(STEP8(0,1), STEP8(16 * 8,1)),
            STEP16(0,8),
            32 * 8
        );


        static readonly uint32_t [] bgcharlayout_xoffset = ArrayCombineUInt32(
            STEP4(0x000,1), STEP4(0x008,1), STEP4(0x010,1), STEP4(0x018,1),
            STEP4(0x020,1), STEP4(0x028,1), STEP4(0x030,1), STEP4(0x038,1),
            STEP4(0x040,1), STEP4(0x048,1), STEP4(0x050,1), STEP4(0x058,1),
            STEP4(0x060,1), STEP4(0x068,1), STEP4(0x070,1), STEP4(0x078,1),
            STEP4(0x080,1), STEP4(0x088,1), STEP4(0x090,1), STEP4(0x098,1),
            STEP4(0x0a0,1), STEP4(0x0a8,1), STEP4(0x0b0,1), STEP4(0x0b8,1),
            STEP4(0x0c0,1), STEP4(0x0c8,1), STEP4(0x0d0,1), STEP4(0x0d8,1),
            STEP4(0x0e0,1), STEP4(0x0e8,1), STEP4(0x0f0,1), STEP4(0x0f8,1),
            STEP4(0x100,1), STEP4(0x108,1), STEP4(0x110,1), STEP4(0x118,1),
            STEP4(0x120,1), STEP4(0x128,1), STEP4(0x130,1), STEP4(0x138,1),
            STEP4(0x140,1), STEP4(0x148,1), STEP4(0x150,1), STEP4(0x158,1),
            STEP4(0x160,1), STEP4(0x168,1), STEP4(0x170,1), STEP4(0x178,1),
            STEP4(0x180,1), STEP4(0x188,1), STEP4(0x190,1), STEP4(0x198,1),
            STEP4(0x1a0,1), STEP4(0x1a8,1), STEP4(0x1b0,1), STEP4(0x1b8,1),
            STEP4(0x1c0,1), STEP4(0x1c8,1), STEP4(0x1d0,1), STEP4(0x1d8,1),
            STEP4(0x1e0,1), STEP4(0x1e8,1), STEP4(0x1f0,1), STEP4(0x1f8,1));


        static readonly uint32_t [] bgcharlayout_yoffset = ArrayCombineUInt32(
            STEP32(0x0000,0x200), STEP32(0x4000,0x200), STEP32(0x8000,0x200), STEP32(0xc000,0x200));


        static readonly gfx_layout bgcharlayout = new gfx_layout(
            256, 128, /* 256x64 image format */
            1,       /* 1 image */
            2,       /* 2 bits per pixel */
            new u32[] { 4, 0 },       /* the two bitplanes for 4 pixels are packed into one byte */
            EXTENDED_XOFFS,
            EXTENDED_YOFFS,
            0x8000,
            new ListBase<u32>(bgcharlayout_xoffset),
            new ListBase<u32>(bgcharlayout_yoffset)
        );


        //static GFXDECODE_START(gfx_m52_sp)
        static readonly gfx_decode_entry [] gfx_m52_sp = new gfx_decode_entry[]
        {
            GFXDECODE_ENTRY("sp", 0x0000, spritelayout, 0, 16),

            //GFXDECODE_END
        };

        //static GFXDECODE_START(gfx_m52_tx)
        static readonly gfx_decode_entry [] gfx_m52_tx = new gfx_decode_entry[]
        {
            GFXDECODE_ENTRY("tx", 0x0000, charlayout, 0, 128),

            //GFXDECODE_END
        };

        //static GFXDECODE_START(gfx_m52_bg)
        static readonly gfx_decode_entry [] gfx_m52_bg = new gfx_decode_entry[]
        {
            GFXDECODE_ENTRY("bg0", 0x0000, bgcharlayout, 0 * 4, 1),
            GFXDECODE_ENTRY("bg1", 0x0000, bgcharlayout, 1 * 4, 1),
            GFXDECODE_ENTRY("bg2", 0x0000, bgcharlayout, 2 * 4, 1),

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

        //MACHINE_CONFIG_START(m52_state::m52)
        public void m52(machine_config config)
        {
            MACHINE_CONFIG_START(config);

            /* basic machine hardware */
            MCFG_DEVICE_ADD("maincpu", z80_device.Z80, MASTER_CLOCK / 6);
            MCFG_DEVICE_PROGRAM_MAP(main_map);
            MCFG_DEVICE_IO_MAP(main_portmap);
            MCFG_DEVICE_VBLANK_INT_DRIVER("screen", irq0_line_hold);

            /* video hardware */
            PALETTE(config, m_sp_palette).set_entries(256, 32);
            GFXDECODE(config, m_sp_gfxdecode, m_sp_palette, gfx_m52_sp);

            PALETTE(config, m_tx_palette).set_entries(512);
            GFXDECODE(config, m_tx_gfxdecode, m_tx_palette, gfx_m52_tx);

            PALETTE(config, m_bg_palette).set_entries(3 * 4, 32);
            GFXDECODE(config, m_bg_gfxdecode, m_bg_palette, gfx_m52_bg);

            MCFG_SCREEN_ADD("screen", SCREEN_TYPE_RASTER);
            MCFG_SCREEN_RAW_PARAMS(MASTER_CLOCK / 3, 384, 136, 376, 282, 22, 274);
            MCFG_SCREEN_UPDATE_DRIVER(screen_update_m52);

            /* sound hardware */
            //m52_sound_c_audio(config);
            MCFG_DEVICE_ADD("irem_audio", m52_soundc_audio_device.IREM_M52_SOUNDC_AUDIO, 0);

            MACHINE_CONFIG_END();
        }
    }


    partial class m52 : global_object
    {
        /*************************************
        *
        *  ROM definitions
        *
        *************************************/

        //ROM_START(mpatrol)
        static readonly List<tiny_rom_entry> rom_mpatrol = new List<tiny_rom_entry>()
        {
            ROM_REGION(0x10000, "maincpu", 0),
            ROM_LOAD("mpa-1.3m", 0x0000, 0x1000, CRC("5873a860") + SHA1("8c03726d6e049c3edbc277440184e31679f78258")),
            ROM_LOAD("mpa-2.3l", 0x1000, 0x1000, CRC("f4b85974") + SHA1("dfb2efb57378a20af6f20569f4360cde95596f93")),
            ROM_LOAD("mpa-3.3k", 0x2000, 0x1000, CRC("2e1a598c") + SHA1("112c3c9678db8a8540a8df3708020c87fd10c91b")),
            ROM_LOAD("mpa-4.3j", 0x3000, 0x1000, CRC("dd05b587") + SHA1("727961b0dafa4a96b580d51013336db2a18aff1e")),

            ROM_REGION(0x8000, "irem_audio:iremsound", 0),
            ROM_LOAD("mp-s1.1a", 0x7000, 0x1000, CRC("561d3108") + SHA1("4998c68a9e9a8002251fa8f07aa1082444a9dc80")),

            ROM_REGION(0x2000, "tx", 0),
            ROM_LOAD("mpe-5.3e", 0x0000, 0x1000, CRC("e3ee7f75") + SHA1("b03d0d56150d3e9da4a4c871338097b4f450b649")),
            ROM_LOAD("mpe-4.3f", 0x1000, 0x1000, CRC("cca6d023") + SHA1("fecb3059fb09897a096add9452b50aec55c07545")),

            /* 0x2000-0x2fff is intentionally left as 0x00 fill, unused bitplane */
            ROM_REGION(0x3000, "sp", ROMREGION_ERASE00),
            ROM_LOAD("mpb-2.3m", 0x0000, 0x1000, CRC("707ace5e") + SHA1("93c682e13e74bce29ced3a87bffb29569c114c3b")),
            ROM_LOAD("mpb-1.3n", 0x1000, 0x1000, CRC("9b72133a") + SHA1("1393ef92ae1ad58a4b62ca1660c0793d30a8b5e2")),

            /* 0x1000-01fff is intentionally left as 0xff fill for the bg regions */
            ROM_REGION(0x2000, "bg0", ROMREGION_ERASEFF),
            ROM_LOAD("mpe-1.3l", 0x0000, 0x1000, CRC("c46a7f72") + SHA1("8bb7c9acaf6833fb6c0575b015991b873a305a84")),

            ROM_REGION(0x2000, "bg1", ROMREGION_ERASEFF),
            ROM_LOAD("mpe-2.3k", 0x0000, 0x1000, CRC("c7aa1fb0") + SHA1("14c6c76e1d0db2c0745e5d6d33ea6945fac8e9ee")),

            ROM_REGION(0x2000, "bg2", ROMREGION_ERASEFF),
            ROM_LOAD("mpe-3.3h", 0x0000, 0x1000, CRC("a0919392") + SHA1("8a090cb8d483a3d67c7360058e3fdd70e151cd62")),

            ROM_REGION(0x0200, "tx_pal", 0),
            ROM_LOAD("mpc-4.2a", 0x0000, 0x0200, CRC("07f99284") + SHA1("dfc52958f2520e1ce4446dd4c84c91413bbacf76")),

            ROM_REGION(0x0020, "bg_pal", 0),
            ROM_LOAD("mpc-3.1m", 0x0000, 0x0020, CRC("6a57eff2") + SHA1("2d1c12dab5915da2ccd466e39436c88be434d634")),

            ROM_REGION(0x0020, "spr_pal", 0),
            ROM_LOAD("mpc-1.1f", 0x0000, 0x0020, CRC("26979b13") + SHA1("8c41a8cce4f3384c392a9f7a223a50d7be0e14a5")),

            ROM_REGION(0x0100, "spr_clut", 0),
            ROM_LOAD("mpc-2.2h", 0x0000, 0x0100, CRC("7ae4cd97") + SHA1("bc0662fac82ffe65f02092d912b2c2b0c7a8ac2b")),

            ROM_END,
        };



        /*************************************
        *
        *  Game drivers
        *
        *************************************/

        static void m52_state_m52(machine_config config, device_t device) { m52_state m52_state = (m52_state)device; m52_state.m52(config); }


        static m52 m_m52 = new m52();


        static device_t device_creator_m52(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new m52_state(mconfig, type, tag); }


        //                                                       creator,            rom          YEAR,   NAME,       PARENT,  MACHINE,            INPUT,                           INIT,                     MONITOR, COMPANY, FULLNAME,      FLAGS
        public static readonly game_driver driver_mpatrol = GAME(device_creator_m52, rom_mpatrol, "1982", "mpatrol",  null,    m52.m52_state_m52,  m_m52.construct_ioport_mpatrol,  driver_device.empty_init, ROT0,    "Irem",  "Moon Patrol", MACHINE_SUPPORTS_SAVE);
    }
}
