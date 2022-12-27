// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint8_t = System.Byte;
using u32 = System.UInt32;

using static mame.attotime_global;
using static mame.ay8910_global;
using static mame.digfx_global;
using static mame.disound_global;
using static mame.drawgfx_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.netlist_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.z80_global;


namespace mame
{
#if false
    void tnx1_state::driver_start()
    {
        decrypt_rom();

        save_item(NAME(m_prot0));
        save_item(NAME(m_prot1));
        save_item(NAME(m_prot_shift));
        save_item(NAME(m_nmi_enabled));

        m_prot0 = 0;
        m_prot1 = 0;
        m_prot_shift = 0;
        m_nmi_enabled = false;
    }

    void tpp2_state::driver_start()
    {
        tnx1_state::driver_start();

        save_item(NAME(m_watchdog_enabled));
        save_item(NAME(m_watchdog_counter));

        m_watchdog_enabled = false;
        m_watchdog_counter = 0;
    }

    void tnx1_state::decrypt_rom()
    {
        uint8_t *rom = memregion("maincpu")->base();
        int len = memregion("maincpu")->bytes();

        /* decrypt the program ROMs */
        std::vector<uint8_t> buffer(len);
        for (int i = 0; i < len; i++)
            buffer[i] = bitswap<8>(rom[bitswap<16>(i, 15, 14, 13, 12, 11, 10, 8, 7, 0, 1, 2, 4, 5, 9, 3, 6) ^ 0xfc], 3, 4, 2, 5, 1, 6, 0, 7);
        std::copy_n(buffer.begin(), len, rom);
    }

    void popeyebl_state::decrypt_rom()
    {
        uint8_t* rom = memregion("blprot")->base();
        for (int i = 0; i < 0x80; i++)
        {
            rom[i + 0x00] ^= 0xf; // opcodes
            rom[i + 0x80] ^= 0x3; // data
        }
    }

    void tpp2_state::decrypt_rom()
    {
        uint8_t *rom = memregion("maincpu")->base();
        int len = memregion("maincpu")->bytes();

        /* decrypt the program ROMs */
        std::vector<uint8_t> buffer(len);
        for (int i = 0; i < len; i++)
            buffer[i] = bitswap<8>(rom[bitswap<16>(i, 15, 14, 13, 12, 11, 10, 8, 7, 6, 3, 9, 5, 4, 2, 1, 0) ^ 0x3f], 3, 4, 2, 5, 1, 6, 0, 7);
        std::copy_n(buffer.begin(), len, rom);
    }

    void tnx1_state::refresh_w(offs_t offset, uint8_t data)
    {
        const bool nmi_enabled = ((offset >> 8) & 1) != 0;
        if (m_nmi_enabled != nmi_enabled)
        {
            m_nmi_enabled = nmi_enabled;

            if (!m_nmi_enabled)
                m_maincpu->set_input_line(INPUT_LINE_NMI, CLEAR_LINE);
        }
    }

    void tpp2_state::refresh_w(offs_t offset, uint8_t data)
    {
        tnx1_state::refresh_w(offset, data);

        m_watchdog_enabled = ((offset >> 9) & 1) != 0;
    }

    WRITE_LINE_MEMBER(tnx1_state::screen_vblank)
    {
        if (state)
        {
            std::copy_n(m_dmasource.target(), m_dmasource.bytes(), m_sprite_ram.begin());
            std::copy_n(m_dmasource.target(), 3, m_background_scroll);
            m_palette_bank = m_dmasource[3];

            m_field ^= 1;
            if (m_nmi_enabled)
                m_maincpu->set_input_line(INPUT_LINE_NMI, ASSERT_LINE);
        }
    }

    WRITE_LINE_MEMBER(tpp2_state::screen_vblank)
    {
        tnx1_state::screen_vblank(state);

        if (state)
        {
            uint8_t watchdog_counter = m_watchdog_counter;

            if (m_nmi_enabled || !m_watchdog_enabled)
                watchdog_counter = 0;
            else
                watchdog_counter = (watchdog_counter + 1) & 0xf;

            if ((watchdog_counter ^ m_watchdog_counter) & 4)
            {
                m_maincpu->set_input_line(INPUT_LINE_RESET, watchdog_counter & 4 ? ASSERT_LINE : CLEAR_LINE);
                m_aysnd->reset();
            }

            m_watchdog_counter = watchdog_counter;
        }
    }
#endif


    partial class tnx1_state : driver_device
    {
#if false
        /* the protection device simply returns the last two values written shifted left */
        /* by a variable amount. */

        uint8_t tnx1_state::protection_r(offs_t offset)
        {
            if (offset == 0)
            {
                return ((m_prot1 << m_prot_shift) | (m_prot0 >> (8-m_prot_shift))) & 0xff;
            }
            else    /* offset == 1 */
            {
                /* the game just checks if bit 2 is clear. Returning 0 seems to be enough. */
                return 0;
            }
        }

        void tnx1_state::protection_w(offs_t offset, uint8_t data)
        {
            if (offset == 0)
            {
                /* this is the same as the level number (1-3) */
                m_prot_shift = data & 0x07;
            }
            else    /* offset == 1 */
            {
                m_prot0 = m_prot1;
                m_prot1 = data;
            }
        }


        void tnx1_state::maincpu_common_map(address_map &map)
        {
            map(0x0000, 0x7fff).rom().region("maincpu",0);
            map(0x8c00, 0x8e7f).ram().share("dmasource");
            map(0x8e80, 0x8fff).ram().share("ramhigh");
            map(0xa000, 0xa3ff).w(FUNC(tnx1_state::popeye_videoram_w)).share("videoram");
            map(0xa400, 0xa7ff).w(FUNC(tnx1_state::popeye_colorram_w)).share("colorram");
            map(0xc000, 0xcfff).w(FUNC(tnx1_state::background_w));
            map(0xe000, 0xe001).rw(FUNC(tnx1_state::protection_r), FUNC(tnx1_state::protection_w));
        }

        void tnx1_state::maincpu_program_map(address_map &map)
        {
            maincpu_common_map(map);
            map(0x8000, 0x87ff).ram().share("ramlow");
            map(0x8800, 0x8bff).nopw(); // Attempts to initialize this area with 00 on boot
        }
#endif
    }


#if false
    void tpp2_state::maincpu_program_map(address_map &map)
    {
        maincpu_common_map(map);
        // 8000-87ff is unpopulated (7f)
        map(0x8800, 0x8bff).ram().share("ramlow"); // 7h
        map(0xc000, 0xdfff).w(FUNC(tpp2_state::background_w));
    }

    void tpp2_noalu_state::maincpu_program_map(address_map &map)
    {
        tpp2_state::maincpu_program_map(map);
        map(0xe000, 0xe001).noprw(); // game still writes level number & reads status, but then discards it
    }

    void popeyebl_state::maincpu_program_map(address_map &map)
    {
        tnx1_state::maincpu_program_map(map);
        map(0xe000, 0xe01f).rom().region("blprot", 0x80);
    }

    void popeyebl_state::decrypted_opcodes_map(address_map& map)
    {
        tnx1_state::maincpu_program_map(map);
        map(0xe000, 0xe01f).rom().region("blprot", 0);
    }
#endif


    partial class tnx1_state : driver_device
    {
        void maincpu_io_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map.global_mask(0xff);
            map(0x00, 0x01).w("aysnd", FUNC(ay8910_device::address_data_w));
            map(0x00, 0x00).portr("P1");
            map(0x01, 0x01).portr("P2");
            map(0x02, 0x02).portr("SYSTEM");
            map(0x03, 0x03).r("aysnd", FUNC(ay8910_device::data_r));
#endif
        }
    }


    //class brazehs : public T


#if false
    READ_LINE_MEMBER(tnx1_state::dsw1_read)
    {
        return m_io_dsw1->read() >> m_dswbit;
    }
#endif


    partial class popeye : construct_ioport_helper
    {
        //static INPUT_PORTS_START( skyskipr )

        //READ_LINE_MEMBER( tnx1_state::pop_field_r )


        //static INPUT_PORTS_START( popeye )
        void construct_ioport_popeye(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            throw new emu_unimplemented();
#if false
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("P1")    /* IN0 */
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ) PORT_4WAY
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ) PORT_4WAY
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ) PORT_4WAY
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ) PORT_4WAY
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_BUTTON1 )
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_UNKNOWN ) /* probably unused */
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_UNKNOWN ) /* probably unused */
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_UNKNOWN ) /* probably unused */

            PORT_START("P2")    /* IN1 */
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ) PORT_4WAY PORT_COCKTAIL
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ) PORT_4WAY PORT_COCKTAIL
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ) PORT_4WAY PORT_COCKTAIL
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ) PORT_4WAY PORT_COCKTAIL
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_BUTTON1 ) PORT_COCKTAIL
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_UNKNOWN ) /* probably unused */
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_UNKNOWN ) /* probably unused */
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_UNKNOWN ) /* probably unused */

            PORT_START("SYSTEM")   /* IN2 */
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_UNKNOWN ) /* probably unused */
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_UNKNOWN ) /* probably unused */
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_START1 )
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_START2 )
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_CUSTOM ) PORT_READ_LINE_MEMBER(tnx1_state, pop_field_r) // inverted init e/o signal (even odd)
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_COIN2 )
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_SERVICE1 )
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_COIN1 )

            PORT_START("DSW0")  /* DSW0 */
            PORT_DIPNAME( 0x0f, 0x0f, DEF_STR( Coinage ) )    PORT_DIPLOCATION("SW1:1,2,3,4")
            PORT_DIPSETTING(    0x08, DEF_STR( 6C_1C ) )
            PORT_DIPSETTING(    0x05, DEF_STR( 5C_1C ) )
            PORT_DIPSETTING(    0x09, DEF_STR( 4C_1C ) )
            PORT_DIPSETTING(    0x0a, DEF_STR( 3C_1C ) )
            PORT_DIPSETTING(    0x0d, DEF_STR( 2C_1C ) )
            PORT_DIPSETTING(    0x0f, DEF_STR( 1C_1C ) )
            PORT_DIPSETTING(    0x0e, DEF_STR( 1C_2C ) )
            PORT_DIPSETTING(    0x03, DEF_STR( 1C_3C ) )
            PORT_DIPSETTING(    0x00, DEF_STR( Free_Play ) )
            PORT_BIT(0x10, IP_ACTIVE_LOW, IPT_UNUSED)
            PORT_CONFNAME( 0x60, 0x40, "Copyright" )
            PORT_CONFSETTING(    0x40, "Nintendo" )
            PORT_CONFSETTING(    0x20, "Nintendo Co.,Ltd" )
            PORT_CONFSETTING(    0x60, "Nintendo of America" )
        //  PORT_CONFSETTING(    0x00, "Nintendo of America" )
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_CUSTOM ) PORT_READ_LINE_MEMBER(tnx1_state, dsw1_read)

            PORT_START("DSW1")  /* DSW1 */
            PORT_DIPNAME( 0x03, 0x01, DEF_STR( Lives ) )       PORT_DIPLOCATION("SW2:1,2")
            PORT_DIPSETTING(    0x03, "1" )
            PORT_DIPSETTING(    0x02, "2" )
            PORT_DIPSETTING(    0x01, "3" )
            PORT_DIPSETTING(    0x00, "4" )
            PORT_DIPNAME( 0x0c, 0x0c, DEF_STR( Difficulty ) )  PORT_DIPLOCATION("SW2:3,4")
            PORT_DIPSETTING(    0x0c, DEF_STR( Easy ) )
            PORT_DIPSETTING(    0x08, DEF_STR( Medium ) )
            PORT_DIPSETTING(    0x04, DEF_STR( Hard ) )
            PORT_DIPSETTING(    0x00, DEF_STR( Hardest ) )
            PORT_DIPNAME( 0x30, 0x30, DEF_STR( Bonus_Life ) )  PORT_DIPLOCATION("SW2:5,6")
            PORT_DIPSETTING(    0x30, "40000" )
            PORT_DIPSETTING(    0x20, "60000" )
            PORT_DIPSETTING(    0x10, "80000" )
            PORT_DIPSETTING(    0x00, DEF_STR( None ) )
            PORT_DIPNAME( 0x40, 0x00, DEF_STR( Demo_Sounds ) ) PORT_DIPLOCATION("SW2:7")
            PORT_DIPSETTING(    0x40, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x00, DEF_STR( On ) )
            PORT_DIPNAME( 0x80, 0x00, DEF_STR( Cabinet ) )     PORT_DIPLOCATION("SW2:8")
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) )
            PORT_DIPSETTING(    0x80, DEF_STR( Cocktail ) )

            PORT_START("MCONF")
            PORT_CONFNAME( 0x03, 0x00, "Interlace mode" )
            PORT_CONFSETTING(    0x00, "False Progressive" )
            PORT_CONFSETTING(    0x01, "Interlaced (scanline skip)" )
            PORT_CONFSETTING(    0x02, "Interlaced (bitmap)" )

            INPUT_PORTS_END
#endif
        }


        //static INPUT_PORTS_START( popeyef )
    }


    partial class tnx1_state : driver_device
    {
        static readonly gfx_layout charlayout = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,1),
            1,
            new u32[] { 0 },
            new u32[] { 7, 6, 5, 4, 3, 2, 1, 0 },
            new u32[] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
            8*8
        );

        static readonly gfx_layout spritelayout = new gfx_layout
        (
            16,16,
            RGN_FRAC(1,4),
            2,
            new u32[] { 0, RGN_FRAC(1,2) },
            new u32[] {RGN_FRAC(1,4)+7,RGN_FRAC(1,4)+6,RGN_FRAC(1,4)+5,RGN_FRAC(1,4)+4,
                RGN_FRAC(1,4)+3,RGN_FRAC(1,4)+2,RGN_FRAC(1,4)+1,RGN_FRAC(1,4)+0,
                7,6,5,4,3,2,1,0 },
            new u32[] { 15*8, 14*8, 13*8, 12*8, 11*8, 10*8, 9*8, 8*8,
                7*8, 6*8, 5*8, 4*8, 3*8, 2*8, 1*8, 0*8 },
            16*8
        );

        //static GFXDECODE_START( gfx_popeye )
        static readonly gfx_decode_entry [] gfx_popeye =
        {
            GFXDECODE_SCALE( "gfx1", 0, charlayout,   16, 16, 2, 2 ), /* chars */
            GFXDECODE_ENTRY( "gfx2", 0, spritelayout, 16+16*2, 8 ) /* sprites */

            //GFXDECODE_END
        };


        void popeye_portB_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            /* bit 0 flips screen */
            flip_screen_set(data & 1);

            /* bits 1-3 select DSW1 bit to read */
            m_dswbit = (data & 0x0e) >> 1;
#endif
        }


        public virtual void config(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, new XTAL(8_000_000) / 2); /* 4 MHz */
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, maincpu_program_map);
            m_maincpu.op0.memory().set_addrmap(AS_IO, maincpu_io_map);
            m_maincpu.op0.refresh_cb().set(refresh_w).reg();

            /* video hardware */
            // FIXME: 59.94 screen refresch is the NTSC standard
            var screen = SCREEN(config, "screen", SCREEN_TYPE_RASTER);
            screen.set_refresh_hz(59.94)
                .set_vblank_time(ATTOSECONDS_IN_USEC(0))
                .set_size(32*16, 32*16)
                .set_visarea(0*16, 32*16-1, 2*16, 30*16-1)
                .set_palette(m_palette)
                .set_screen_update(screen_update);
            screen.screen_vblank().set((write_line_delegate)screen_vblank).reg();

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_popeye);
            PALETTE(config, m_palette, tnx1_palette, 16 + 16*2 + 8*4);

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            AY8910(config, m_aysnd, new XTAL(8_000_000) / 4);
            m_aysnd.op0.port_a_read_callback().set_ioport("DSW0").reg();
            m_aysnd.op0.port_b_write_callback().set(popeye_portB_w).reg();
            m_aysnd.op0.add_route(ALL_OUTPUTS, "mono", 0.40);
        }
    }


    partial class tpp2_state : tpp1_state
    {
        public override void config(machine_config config)
        {
            base.config(config);

            m_aysnd.op0.disound.reset_routes();
            m_aysnd.op0.set_flags(ay8910_device.AY8910_RESISTOR_OUTPUT); /* Does tnx1, tpp1 & popeyebl have the same filtering? */
            m_aysnd.op0.set_resistors_load((int)2000.0, (int)2000.0, (int)2000.0);
            m_aysnd.op0.add_route(0, "snd_nl", 1.0, 0);
            m_aysnd.op0.add_route(1, "snd_nl", 1.0, 1);
            m_aysnd.op0.add_route(2, "snd_nl", 1.0, 2);

            /* NETLIST configuration using internal AY8910 resistor values */

            NETLIST_SOUND(config, "snd_nl", 48000)
                .set_source(netlist_popeye)  //.set_source(NETLIST_NAME(popeye))
                .disound.add_route(ALL_OUTPUTS, "mono", 1.0);

            NETLIST_STREAM_INPUT(config, "snd_nl:cin0", 0, "R_AY1_1.R");
            NETLIST_STREAM_INPUT(config, "snd_nl:cin1", 1, "R_AY1_2.R");
            NETLIST_STREAM_INPUT(config, "snd_nl:cin2", 2, "R_AY1_3.R");

            NETLIST_STREAM_OUTPUT(config, "snd_nl:cout0", 0, "ROUT.1").set_mult_offset(1.0, -2.0);
        }
    }


    //void popeyebl_state::config(machine_config& config)


    public partial class popeye : construct_ioport_helper
    {
        /***************************************************************************

          Game ROMset(s)

        ***************************************************************************/

        //ROM_START( skyskipr )


        /*
            Popeye

            CPU/Sound Board: TPP2-01-CPU
            Video Board:     TPP2-01-VIDEO
        */
        //ROM_START( popeye )
        static readonly tiny_rom_entry [] rom_popeye =
        {
            ROM_REGION( 0x8000, "maincpu", 0 ),
            ROM_LOAD( "tpp2-c.7a", 0x0000, 0x2000, CRC("9af7c821") + SHA1("592acfe221b5c3bd9b920f639b141f37a56d6997") ),
            ROM_LOAD( "tpp2-c.7b", 0x2000, 0x2000, CRC("c3704958") + SHA1("af96d10fa9bdb86b00c89d10f67cb5ca5586f446") ),
            ROM_LOAD( "tpp2-c.7c", 0x4000, 0x2000, CRC("5882ebf9") + SHA1("5531229b37f9ba0ede7fdc24909e3c3efbc8ade4") ),
            ROM_LOAD( "tpp2-c.7e", 0x6000, 0x2000, CRC("ef8649ca") + SHA1("a0157f91600e56e2a953dadbd76da4330652e5c8") ),

            ROM_REGION( 0x0800, "gfx1", 0 ),
            ROM_LOAD( "tpp2-v.5n", 0x0000, 0x0800, CRC("cca61ddd") + SHA1("239f87947c3cc8c6693c295ebf5ea0b7638b781c") ),   /* first half is empty */
            ROM_CONTINUE(          0x0000, 0x0800 ),

            ROM_REGION( 0x8000, "gfx2", 0 ),
            ROM_LOAD( "tpp2-v.1e", 0x0000, 0x2000, CRC("0f2cd853") + SHA1("426c9b4f6579bfcebe72b976bfe4f05147d53f96") ),
            ROM_LOAD( "tpp2-v.1f", 0x2000, 0x2000, CRC("888f3474") + SHA1("ddee56b2b49bd50aaf9c98d8ef6e905e3f6ab859") ),
            ROM_LOAD( "tpp2-v.1j", 0x4000, 0x2000, CRC("7e864668") + SHA1("8e275dbb1c586f4ebca7548db05246ef0f56d7b1") ),
            ROM_LOAD( "tpp2-v.1k", 0x6000, 0x2000, CRC("49e1d170") + SHA1("bd51a4e34ce8109f26954760156e3cf05fb9db57") ),

            ROM_REGION( 0x40, "proms", 0 ),
            ROM_LOAD( "tpp2-c.4a", 0x0000, 0x0020, CRC("375e1602") + SHA1("d84159a0af5db577821c43712bc733329a43af80") ), /* background palette */
            ROM_LOAD( "tpp2-c.3a", 0x0020, 0x0020, CRC("e950bea1") + SHA1("0b48082fe79d9fcdca7e80caff1725713d0c3163") ), /* char palette */

            ROM_REGION( 0x0100, "sprpal", 0 ),
            ROM_LOAD_NIB_LOW(  "tpp2-c.5b", 0x0000, 0x0100, CRC("c5826883") + SHA1("f2c4d3473b3bfa55bffad003dc1fd79540e7e0d1") ), /* sprite palette - low 4 bits */
            ROM_LOAD_NIB_HIGH( "tpp2-c.5a", 0x0000, 0x0100, CRC("c576afba") + SHA1("013c8e8db08a03c7ba156cfefa671d26155fe835") ), /* sprite palette - high 4 bits */

            ROM_REGION( 0x0100, "timing", 0 ),
            ROM_LOAD( "tpp2-v.7j", 0x0000, 0x0100, CRC("a4655e2e) SHA1(2a620932fccb763c6c667278c0914f31b9f00ddf") ), /* video timing prom */

            ROM_END,
        };


        //ROM_START( popeyeu )

        //ROM_START( popeyef )

        //ROM_START( popeyebl )

        //ROM_START( popeyeb2 )

        //ROM_START( popeyeb3 )

        //ROM_START( popeyej )

        //ROM_START( popeyejo )

        //ROM_START( popeyehs )


        static void tpp2_state_config(machine_config config, device_t device) { tpp2_state tpp2_state = (tpp2_state)device; tpp2_state.config(config); }

        static popeye m_popeye = new popeye();

        static device_t device_creator_popeye(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new tpp2_state(mconfig, type, tag); }

        public static readonly game_driver driver_popeye = GAME(device_creator_popeye, rom_popeye, "1982", "popeye", "0", tpp2_state_config, m_popeye.construct_ioport_popeye, driver_device.empty_init, ROT0, "Nintendo", "Popeye (revision D)", MACHINE_SUPPORTS_SAVE);
    }
}
