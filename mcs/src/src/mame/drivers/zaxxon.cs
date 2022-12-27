// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u16 = System.UInt16;
using u32 = System.UInt32;

using static mame._74259_global;
using static mame.digfx_global;
using static mame.drawgfx_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.generic_global;
using static mame.hash_global;
using static mame.i8255_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.z80_global;


namespace mame
{
    partial class zaxxon_state : driver_device
    {
        /*************************************
         *  Constants
         *************************************/

        static readonly XTAL MASTER_CLOCK = XTAL_global.op("48.66_MHz_XTAL");
        //static constexpr XTAL SOUND_CLOCK   = 4_MHz_XTAL;

        static readonly XTAL PIXEL_CLOCK   = MASTER_CLOCK / 8;

        const u16 HTOTAL              = 384;
        const u16 HBEND               = 0;
        const u16 HBSTART             = 256;

        const u16 VTOTAL              = 264;
        const u16 VBEND               = 16;
        const u16 VBSTART             = 240;


        /*************************************
         *  Interrupt generation
         *************************************/
#if false
        INPUT_CHANGED_MEMBER(zaxxon_state::service_switch)
        {
            /* pressing the service switch sends an NMI */
            if (newval)
                m_maincpu->pulse_input_line(INPUT_LINE_NMI, attotime::zero);
        }
#endif


        //WRITE_LINE_MEMBER(zaxxon_state::vblank_int)
        void vblank_int(int state)
        {
            throw new emu_unimplemented();
#if false
            if (state && m_int_enabled)
                m_maincpu->set_input_line(0, ASSERT_LINE);
#endif
        }


        //WRITE_LINE_MEMBER(zaxxon_state::int_enable_w)
        void int_enable_w(int state)
        {
            throw new emu_unimplemented();
#if false
            m_int_enabled = state;
            if (!m_int_enabled)
                m_maincpu->set_input_line(0, CLEAR_LINE);
#endif
        }


#if false
        /*************************************
         *  Machine setup
         *************************************/
        void zaxxon_state::machine_start()
        {
            /* register for save states */
            save_item(NAME(m_int_enabled));
            save_item(NAME(m_coin_status));
        }


        /*************************************
         *  Input handlers
         *************************************/
        uint8_t zaxxon_state::razmataz_counter_r()
        {
            /* this behavior is really unknown; however, the code is using this */
            /* counter as a sort of timeout when talking to the sound board */
            /* it needs to be increasing at a reasonable rate but not too fast */
            /* or else the sound will mess up */
            return m_razmataz_counter++ >> 8;
        }


        template <int Num>
        CUSTOM_INPUT_MEMBER(zaxxon_state::razmataz_dial_r)
        {
            int res;

            int delta = m_dials[Num]->read();

            if (delta < 0x80)
            {
                // right
                m_razmataz_dial_pos[Num] -= delta;
                res = (m_razmataz_dial_pos[Num] << 1) | 1;
            }
            else
            {
                // left
                m_razmataz_dial_pos[Num] += delta;
                res = (m_razmataz_dial_pos[Num] << 1);
            }

            return res;
        }


        /*************************************
         *  Output handlers
         *************************************/
        void zaxxon_state::zaxxon_control_w(offs_t offset, uint8_t data)
        {
            // address decode for E0F8/E0F9 (74LS138 @ U57) has its G2B enable input in common with this latch
            bool a3 = BIT(offset, 3);
            m_mainlatch[1]->write_bit((a3 ? 4 : 0) | (offset & 3), BIT(data, 0));
            if (a3 && !BIT(offset, 1))
                bg_position_w(offset & 1, data);
        }
#endif


        //WRITE_LINE_MEMBER(zaxxon_state::coin_counter_a_w)
        void coin_counter_a_w(int state)
        {
            throw new emu_unimplemented();
#if false
            machine().bookkeeping().coin_counter_w(0, state);
#endif
        }


        //WRITE_LINE_MEMBER(zaxxon_state::coin_counter_b_w)
        void coin_counter_b_w(int state)
        {
            throw new emu_unimplemented();
#if false
            machine().bookkeeping().coin_counter_w(1, state);
#endif
        }


        // There is no external coin lockout circuitry; instead, the pcb simply latches
        // each coin input, which then needs to be explicitly cleared by the game.
        // Each coin input first passes through a debounce circuit consisting of a
        // LS175 quad flip-flop and LS10 3-input NAND gate, which is not emulated.
        //WRITE_LINE_MEMBER(zaxxon_state::coin_enable_w)
        void coin_enable_w(int state)
        {
            throw new emu_unimplemented();
#if false
            for (int n = 0; n < 3; n++)
                if (!BIT(m_mainlatch[0]->output_state(), n))
                    m_coin_status[n] = 0;
#endif
        }


#if false
        INPUT_CHANGED_MEMBER(zaxxon_state::zaxxon_coin_inserted)
        {
            if (newval && BIT(m_mainlatch[0]->output_state(), param))
                m_coin_status[param] = 1;
        }


        template <int Num>
        READ_LINE_MEMBER(zaxxon_state::zaxxon_coin_r)
        {
            return m_coin_status[Num];
        }
#endif


        /*************************************
         *  Main CPU memory handlers
         *************************************/
        /* complete memory map derived from schematics */
        void zaxxon_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map(0x0000, 0x5fff).rom();
            map(0x6000, 0x6fff).ram();
            map(0x8000, 0x83ff).mirror(0x1c00).ram().w(FUNC(zaxxon_state::zaxxon_videoram_w)).share("videoram");
            map(0xa000, 0xa0ff).mirror(0x1f00).ram().share("spriteram");
            map(0xc000, 0xc000).mirror(0x18fc).portr("SW00");
            map(0xc001, 0xc001).mirror(0x18fc).portr("SW01");
            map(0xc002, 0xc002).mirror(0x18fc).portr("DSW02");
            map(0xc003, 0xc003).mirror(0x18fc).portr("DSW03");
            map(0xc100, 0xc100).mirror(0x18ff).portr("SW100");
            map(0xc000, 0xc007).mirror(0x18f8).w("mainlatch1", FUNC(ls259_device::write_d0));
            map(0xe03c, 0xe03f).mirror(0x1f00).rw("ppi8255", FUNC(i8255_device::read), FUNC(i8255_device::write));
            map(0xe0f0, 0xe0f3).mirror(0x1f00).select(0x0008).w(FUNC(zaxxon_state::zaxxon_control_w));
#endif
        }


#if false
        void zaxxon_state::decrypted_opcodes_map(address_map &map)
        {
            map(0x0000, 0x5fff).rom().share("decrypted_opcodes");
        }

        /* derived from Zaxxon, different sound hardware */
        void zaxxon_state::ixion_map(address_map &map)
        {
            map(0x0000, 0x5fff).rom();
            map(0x6000, 0x6fff).ram();
            map(0x8000, 0x83ff).mirror(0x1c00).ram().w(FUNC(zaxxon_state::zaxxon_videoram_w)).share("videoram");
            map(0xa000, 0xa0ff).mirror(0x1f00).ram().share("spriteram");
            map(0xc000, 0xc000).mirror(0x18fc).portr("SW00");
            map(0xc001, 0xc001).mirror(0x18fc).portr("SW01");
            map(0xc002, 0xc002).mirror(0x18fc).portr("DSW02");
            map(0xc003, 0xc003).mirror(0x18fc).portr("DSW03");
            map(0xc100, 0xc100).mirror(0x18ff).portr("SW100");
            map(0xc000, 0xc007).mirror(0x18f8).w("mainlatch1", FUNC(ls259_device::write_d0));
            map(0xe03c, 0xe03c).mirror(0x1f00).rw("usbsnd", FUNC(usb_sound_device::status_r), FUNC(usb_sound_device::data_w));
            map(0xe0f0, 0xe0f3).mirror(0x1f00).select(0x0008).w(FUNC(zaxxon_state::zaxxon_control_w));
        }


        /* complete memory map derived from schematics */
        void zaxxon_state::congo_map(address_map &map)
        {
            map(0x0000, 0x7fff).rom();
            map(0x8000, 0x8fff).ram();
            map(0xa000, 0xa3ff).mirror(0x1800).ram().w(FUNC(zaxxon_state::zaxxon_videoram_w)).share("videoram");
            map(0xa400, 0xa7ff).mirror(0x1800).ram().w(FUNC(zaxxon_state::congo_colorram_w)).share("colorram");
            map(0xc000, 0xc000).mirror(0x1fc4).portr("SW00");
            map(0xc001, 0xc001).mirror(0x1fc4).portr("SW01");
            map(0xc002, 0xc002).mirror(0x1fc4).portr("DSW02");
            map(0xc003, 0xc003).mirror(0x1fc4).portr("DSW03");
            map(0xc008, 0xc008).mirror(0x1fc7).portr("SW100");
            map(0xc018, 0xc01f).mirror(0x1fc0).w("mainlatch1", FUNC(ls259_device::write_d0));
            map(0xc020, 0xc027).mirror(0x1fc0).w("mainlatch2", FUNC(ls259_device::write_d0));
            map(0xc028, 0xc029).mirror(0x1fc4).w(FUNC(zaxxon_state::bg_position_w));
            map(0xc030, 0xc033).mirror(0x1fc4).w(FUNC(zaxxon_state::congo_sprite_custom_w));
            map(0xc038, 0xc03f).mirror(0x1fc0).w("soundlatch", FUNC(generic_latch_8_device::write));
        }


        /* complete memory map derived from schematics */
        void zaxxon_state::congo_sound_map(address_map &map)
        {
            map(0x0000, 0x1fff).rom();
            map(0x4000, 0x47ff).mirror(0x1800).ram();
            map(0x6000, 0x6000).mirror(0x1fff).w("sn1", FUNC(sn76489a_device::write));
            map(0x8000, 0x8003).mirror(0x1ffc).rw("ppi8255", FUNC(i8255_device::read), FUNC(i8255_device::write));
            map(0xa000, 0xa000).mirror(0x1fff).w("sn2", FUNC(sn76489a_device::write));
        }
#endif
    }


    partial class zaxxon : construct_ioport_helper
    {
        /*************************************
         *  Port definitions
         *************************************/
        //static INPUT_PORTS_START( zaxxon )
        void construct_ioport_zaxxon(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            throw new emu_unimplemented();
#if false
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("SW00")
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ) PORT_8WAY
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ) PORT_8WAY
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ) PORT_8WAY
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ) PORT_8WAY
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_BUTTON1 )
            PORT_BIT( 0xe0, IP_ACTIVE_HIGH, IPT_UNUSED )

            PORT_START("SW01")
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ) PORT_8WAY PORT_COCKTAIL
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ) PORT_8WAY PORT_COCKTAIL
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ) PORT_8WAY PORT_COCKTAIL
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ) PORT_8WAY PORT_COCKTAIL
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_BUTTON1 ) PORT_COCKTAIL
            PORT_BIT( 0xe0, IP_ACTIVE_HIGH, IPT_UNUSED )

            PORT_START("SW100")
            PORT_BIT( 0x03, IP_ACTIVE_HIGH, IPT_UNUSED )
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_START1 )
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_START2 )
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_UNUSED )
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_CUSTOM ) PORT_READ_LINE_MEMBER(zaxxon_state, zaxxon_coin_r<0>)
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_CUSTOM ) PORT_READ_LINE_MEMBER(zaxxon_state, zaxxon_coin_r<1>)
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_CUSTOM ) PORT_READ_LINE_MEMBER(zaxxon_state, zaxxon_coin_r<2>)

            PORT_START("COIN")
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_COIN1 )    PORT_CHANGED_MEMBER(DEVICE_SELF, zaxxon_state,zaxxon_coin_inserted, 0)
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_COIN2 )    PORT_CHANGED_MEMBER(DEVICE_SELF, zaxxon_state,zaxxon_coin_inserted, 1)
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_SERVICE1 ) PORT_CHANGED_MEMBER(DEVICE_SELF, zaxxon_state,zaxxon_coin_inserted, 2)

            PORT_START("SERVICESW")
            PORT_SERVICE_NO_TOGGLE( 0x01, IP_ACTIVE_HIGH ) PORT_CHANGED_MEMBER(DEVICE_SELF, zaxxon_state,service_switch, 0)

            PORT_START("DSW02")
            PORT_DIPNAME( 0x03, 0x03, DEF_STR( Bonus_Life ) ) PORT_DIPLOCATION("SW1:!1,!2")
            PORT_DIPSETTING(    0x03, "10000" )
            PORT_DIPSETTING(    0x01, "20000" )
            PORT_DIPSETTING(    0x02, "30000" )
            PORT_DIPSETTING(    0x00, "40000" )
            PORT_DIPNAME( 0x04, 0x04, DEF_STR( Unused ) ) PORT_DIPLOCATION("SW1:!3")
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x04, DEF_STR( On ) )
            PORT_DIPNAME( 0x08, 0x08, DEF_STR( Unused ) ) PORT_DIPLOCATION("SW1:!4")
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x08, DEF_STR( On ) )
            PORT_DIPNAME( 0x30, 0x30, DEF_STR( Lives ) ) PORT_DIPLOCATION("SW1:!5,!6")
            PORT_DIPSETTING(    0x30, "3" )
            PORT_DIPSETTING(    0x10, "4" )
            PORT_DIPSETTING(    0x20, "5" )
            PORT_DIPSETTING(    0x00, DEF_STR( Free_Play ) )
            PORT_DIPNAME( 0x40, 0x40, "Sound" ) PORT_DIPLOCATION("SW1:!7")
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x40, DEF_STR( On ) )
            PORT_DIPNAME( 0x80, 0x00, DEF_STR( Cabinet ) ) PORT_DIPLOCATION("SW1:!8")
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) )
            PORT_DIPSETTING(    0x80, DEF_STR( Cocktail ) )

            PORT_START("DSW03")
            PORT_DIPNAME( 0x0f, 0x03, DEF_STR ( Coin_B ) ) PORT_DIPLOCATION("SW2:!1,!2,!3,!4")
            PORT_DIPSETTING(    0x0f, DEF_STR ( 4C_1C ) )
            PORT_DIPSETTING(    0x07, DEF_STR ( 3C_1C ) )
            PORT_DIPSETTING(    0x0b, DEF_STR ( 2C_1C ) )
            PORT_DIPSETTING(    0x06, "2C/1C 5C/3C 6C/4C" )
            PORT_DIPSETTING(    0x0a, "2C/1C 3C/2C 4C/3C" )
            PORT_DIPSETTING(    0x03, DEF_STR ( 1C_1C ) )
            PORT_DIPSETTING(    0x02, "1C/1C 5C/6C" )
            PORT_DIPSETTING(    0x0c, "1C/1C 4C/5C" )
            PORT_DIPSETTING(    0x04, "1C/1C 2C/3C" )
            PORT_DIPSETTING(    0x0d, DEF_STR ( 1C_2C ) )
            PORT_DIPSETTING(    0x08, "1C/2C 5C/11C" )
            PORT_DIPSETTING(    0x00, "1C/2C 4C/9C" )
            PORT_DIPSETTING(    0x05, DEF_STR ( 1C_3C ) )
            PORT_DIPSETTING(    0x09, DEF_STR ( 1C_4C ) )
            PORT_DIPSETTING(    0x01, DEF_STR ( 1C_5C ) )
            PORT_DIPSETTING(    0x0e, DEF_STR ( 1C_6C ) )
            PORT_DIPNAME( 0xf0, 0x30, DEF_STR ( Coin_A ) ) PORT_DIPLOCATION("SW2:!5,!6,!7,!8")
            PORT_DIPSETTING(    0xf0, DEF_STR ( 4C_1C ) )
            PORT_DIPSETTING(    0x70, DEF_STR ( 3C_1C ) )
            PORT_DIPSETTING(    0xb0, DEF_STR ( 2C_1C ) )
            PORT_DIPSETTING(    0x60, "2C/1C 5C/3C 6C/4C" )
            PORT_DIPSETTING(    0xa0, "2C/1C 3C/2C 4C/3C" )
            PORT_DIPSETTING(    0x30, DEF_STR ( 1C_1C ) )
            PORT_DIPSETTING(    0x20, "1C/1C 5C/6C" )
            PORT_DIPSETTING(    0xc0, "1C/1C 4C/5C" )
            PORT_DIPSETTING(    0x40, "1C/1C 2C/3C" )
            PORT_DIPSETTING(    0xd0, DEF_STR ( 1C_2C ) )
            PORT_DIPSETTING(    0x80, "1C/2C 5C/11C" )
            PORT_DIPSETTING(    0x00, "1C/2C 4C/9C" )
            PORT_DIPSETTING(    0x50, DEF_STR ( 1C_3C ) )
            PORT_DIPSETTING(    0x90, DEF_STR ( 1C_4C ) )
            PORT_DIPSETTING(    0x10, DEF_STR ( 1C_5C ) )
            PORT_DIPSETTING(    0xe0, DEF_STR ( 1C_6C ) )

            INPUT_PORTS_END
#endif
        }


        //static INPUT_PORTS_START( szaxxon )

        //static INPUT_PORTS_START( futspy )

        //static INPUT_PORTS_START( razmataz )

        //static INPUT_PORTS_START( ixion )

        //static INPUT_PORTS_START( congo )
    }


    partial class zaxxon_state : driver_device
    {
        /*************************************
         *  Graphics definitions
         *************************************/
        static readonly gfx_layout zaxxon_spritelayout = new gfx_layout
        (
            32,32,
            RGN_FRAC(1,3),
            3,
            new u32 [] { RGN_FRAC(2,3), RGN_FRAC(1,3), RGN_FRAC(0,3) },
            new u32 [] { 0, 1, 2, 3, 4, 5, 6, 7,
                    8*8+0, 8*8+1, 8*8+2, 8*8+3, 8*8+4, 8*8+5, 8*8+6, 8*8+7,
                    16*8+0, 16*8+1, 16*8+2, 16*8+3, 16*8+4, 16*8+5, 16*8+6, 16*8+7,
                    24*8+0, 24*8+1, 24*8+2, 24*8+3, 24*8+4, 24*8+5, 24*8+6, 24*8+7 },
            new u32 [] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
                    32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8,
                    64*8, 65*8, 66*8, 67*8, 68*8, 69*8, 70*8, 71*8,
                    96*8, 97*8, 98*8, 99*8, 100*8, 101*8, 102*8, 103*8 },
            128*8
        );


        //static GFXDECODE_START( gfx_zaxxon )
        static readonly gfx_decode_entry [] gfx_zaxxon =
        {
            GFXDECODE_ENTRY( "gfx_tx", 0, gfx_8x8x2_planar,  0, 64*2 ),  /* characters */
            GFXDECODE_ENTRY( "gfx_bg", 0, gfx_8x8x3_planar,  0, 32*2 ),  /* background tiles */
            GFXDECODE_ENTRY( "gfx_spr", 0, zaxxon_spritelayout,  0, 32*2 ),  /* sprites */

            //GFXDECODE_END
        };


        /*************************************
         *  Machine driver
         *************************************/

        void root(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, MASTER_CLOCK / 16);
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, zaxxon_map);

            I8255A(config, m_ppi);
            m_ppi.op0.out_pa_callback().set(zaxxon_sound_a_w).reg();
            m_ppi.op0.out_pb_callback().set(zaxxon_sound_b_w).reg();
            m_ppi.op0.out_pc_callback().set(zaxxon_sound_c_w).reg();

            LS259(config, m_mainlatch[0]); // U55 on Zaxxon IC Board A
            m_mainlatch[0].op0.q_out_cb<u32_const_0>().set((write_line_delegate)coin_enable_w).reg(); // COIN EN A
            m_mainlatch[0].op0.q_out_cb<u32_const_1>().set((write_line_delegate)coin_enable_w).reg(); // COIN EN B
            m_mainlatch[0].op0.q_out_cb<u32_const_2>().set((write_line_delegate)coin_enable_w).reg(); // SERV EN
            m_mainlatch[0].op0.q_out_cb<u32_const_3>().set((write_line_delegate)coin_counter_a_w).reg(); // COUNT A
            m_mainlatch[0].op0.q_out_cb<u32_const_4>().set((write_line_delegate)coin_counter_b_w).reg(); // COUNT B
            m_mainlatch[0].op0.q_out_cb<u32_const_6>().set((write_line_delegate)flipscreen_w).reg(); // FLIP

            LS259(config, m_mainlatch[1]); // U56 on Zaxxon IC Board A
            m_mainlatch[1].op0.q_out_cb<u32_const_0>().set((write_line_delegate)int_enable_w).reg(); // INTON
            m_mainlatch[1].op0.q_out_cb<u32_const_1>().set((write_line_delegate)fg_color_w).reg(); // CREF 1
            m_mainlatch[1].op0.q_out_cb<u32_const_6>().set((write_line_delegate)bg_color_w).reg(); // CREF 3
            m_mainlatch[1].op0.q_out_cb<u32_const_7>().set((write_line_delegate)bg_enable_w).reg(); // BEN

            /* video hardware */
            GFXDECODE(config, m_gfxdecode, m_palette, gfx_zaxxon);
            PALETTE(config, m_palette, zaxxon_palette, 256);

            screen_device screen = SCREEN(config, "screen", SCREEN_TYPE_RASTER);
            screen.set_raw(PIXEL_CLOCK, HTOTAL, HBEND, HBSTART, VTOTAL, VBEND, VBSTART);
            screen.set_screen_update(screen_update_zaxxon);
            screen.set_palette(m_palette);
            screen.screen_vblank().set((write_line_delegate)vblank_int).reg();
        }


        public void zaxxon(machine_config config)
        {
            root(config);

            /* sound hardware */
            SPEAKER(config, "speaker").front_center();
            zaxxon_samples(config);
        }


        //void zaxxon_state::szaxxon(machine_config &config)

        //void zaxxon_state::szaxxone(machine_config &config)

        //void zaxxon_state::futspye(machine_config &config)

        //void zaxxon_state::razmataze(machine_config &config)

        //void zaxxon_state::ixion(machine_config &config)

        //void zaxxon_state::congo(machine_config &config)
    }


    partial class zaxxon : construct_ioport_helper
    {
        /*************************************
         *  ROM definitions
         *************************************/

        //ROM_START( zaxxon )
        static readonly tiny_rom_entry [] rom_zaxxon =
        {
            ROM_REGION( 0x6000, "maincpu", 0 ),
            ROM_LOAD( "zaxxon_rom3d.u27",  0x0000, 0x2000, CRC("6e2b4a30") + SHA1("80ac53c554c84226b119cbe3cf3470bcdbcd5762") ), /* These 3 roms had a red D stamped on them */
            ROM_LOAD( "zaxxon_rom2d.u28",  0x2000, 0x2000, CRC("1c9ea398") + SHA1("0cd259be3fa80f3d53dfa76d5ca06773cdfe5945") ),
            ROM_LOAD( "zaxxon_rom1d.u29",  0x4000, 0x1000, CRC("1c123ef9") + SHA1("2588be06ea7baca6112d58c78a1eeb98aad8a02e") ),

            ROM_REGION( 0x1000, "gfx_tx", 0 ),
            ROM_LOAD( "zaxxon_rom14.u68", 0x0000, 0x0800, CRC("07bf8c52") + SHA1("425157a1625b1bd5169c3218b958010bf6af12bb") ),
            ROM_LOAD( "zaxxon_rom15.u69", 0x0800, 0x0800, CRC("c215edcb") + SHA1("f1ded2173eb139f48d2ca86c5ef00acbe6c11cd3") ),

            ROM_REGION( 0x6000, "gfx_bg", 0 ),
            ROM_LOAD( "zaxxon_rom6.u113", 0x0000, 0x2000, CRC("6e07bb68") + SHA1("a002f3441b0f0044615ce71ecbd14edadba16270") ),
            ROM_LOAD( "zaxxon_rom5.u112", 0x2000, 0x2000, CRC("0a5bce6a") + SHA1("a86543727389931244ba8a576b543d7ac05a2585") ),
            ROM_LOAD( "zaxxon_rom4.u111", 0x4000, 0x2000, CRC("a5bf1465") + SHA1("a8cd27dfb4a606bae8bfddcf936e69e980fb1977") ),

            ROM_REGION( 0x6000, "gfx_spr", 0 ),
            ROM_LOAD( "zaxxon_rom11.u77", 0x0000, 0x2000, CRC("eaf0dd4b") + SHA1("194e2ca0a806e0cb6bb7cc8341d1fc6f2ea911f6") ),
            ROM_LOAD( "zaxxon_rom12.u78", 0x2000, 0x2000, CRC("1c5369c7") + SHA1("af6a5984c3cedfa8c9efcd669f4f205b51a433b2") ),
            ROM_LOAD( "zaxxon_rom13.u79", 0x4000, 0x2000, CRC("ab4e8a9a") + SHA1("4ac79cccc30e4adfa878b36101e97e20ac010438") ),

            ROM_REGION( 0x8000, "tilemap_dat", 0 ),
            ROM_LOAD( "zaxxon_rom8.u91",  0x0000, 0x2000, CRC("28d65063") + SHA1("e1f90716236c61df61bdc6915a8e390cb4dcbf15") ),
            ROM_LOAD( "zaxxon_rom7.u90",  0x2000, 0x2000, CRC("6284c200") + SHA1("d26a9049541479b8b19f5aa0690cf4aaa787c9b5") ),
            ROM_LOAD( "zaxxon_rom10.u93", 0x4000, 0x2000, CRC("a95e61fd") + SHA1("a0f8c15ff75affa3532abf8f340811cf415421fd") ),
            ROM_LOAD( "zaxxon_rom9.u92",  0x6000, 0x2000, CRC("7e42691f") + SHA1("2124363be8f590b74e2b15dd3f90d77dd9ca9528") ),

            ROM_REGION( 0x0200, "proms", 0 ),
            ROM_LOAD( "mro16.u76",   0x0000, 0x0100, CRC("6cc6695b") + SHA1("01ae8450ccc302e1a5ae74230d44f6f531a962e2") ), /* BPROM from TI stamped as J214A2  MRO16 */
            ROM_LOAD( "zaxxon.u72",  0x0100, 0x0100, CRC("deaa21f7") + SHA1("0cf08fb62f77d93ff7cb883c633e0db35906e11d") ), /* Same data as PR-5167 from Super Zaxxon */

            ROM_END,
        };


        //ROM_START( zaxxon2 )

        //ROM_START( zaxxon3 )

        //ROM_START( zaxxonj )

        //ROM_START( zaxxonb )

        //ROM_START( szaxxon )

        //ROM_START( futspy )

        //ROM_START( razmataz )

        //ROM_START( ixion )

        //ROM_START( congo ) /* 2 board stack, Sega game ID number for this set is 834-5180 */

        //ROM_START( congoa ) /* 3 board stack, Sega game ID number for this set is 834-5156 */

        //ROM_START( tiptop ) /* 3 board stack */
    }


#if false
    /*************************************
     *  Driver initialization
     *************************************/

    void zaxxon_state::init_zaxxonj()
    {
    /*
        the values vary, but the translation mask is always laid out like this:

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
            { 0x0a,0x0a,0x22,0x22,0xaa,0xaa,0x82,0x82 },    /* ...............0 */
            { 0xa0,0xaa,0x28,0x22,0xa0,0xaa,0x28,0x22 },    /* ...............1 */
        };

        static const uint8_t opcode_xortable[8][8] =
        {
            { 0x8a,0x8a,0x02,0x02,0x8a,0x8a,0x02,0x02 },    /* .......0...0...0 */
            { 0x80,0x80,0x08,0x08,0xa8,0xa8,0x20,0x20 },    /* .......0...0...1 */
            { 0x8a,0x8a,0x02,0x02,0x8a,0x8a,0x02,0x02 },    /* .......0...1...0 */
            { 0x02,0x08,0x2a,0x20,0x20,0x2a,0x08,0x02 },    /* .......0...1...1 */
            { 0x88,0x0a,0x88,0x0a,0xaa,0x28,0xaa,0x28 },    /* .......1...0...0 */
            { 0x80,0x80,0x08,0x08,0xa8,0xa8,0x20,0x20 },    /* .......1...0...1 */
            { 0x88,0x0a,0x88,0x0a,0xaa,0x28,0xaa,0x28 },    /* .......1...1...0 */
            { 0x02,0x08,0x2a,0x20,0x20,0x2a,0x08,0x02 }     /* .......1...1...1 */
        };

        uint8_t *rom = memregion("maincpu")->base();

        for (int A = 0x0000; A < 0x6000; A++)
        {
            uint8_t src = rom[A];

            /* pick the translation table from bit 0 of the address */
            int i = A & 1;

            /* pick the offset in the table from bits 1, 3 and 5 of the source data */
            int j = ((src >> 1) & 1) + (((src >> 3) & 1) << 1) + (((src >> 5) & 1) << 2);
            /* the bottom half of the translation table is the mirror image of the top */
            if (src & 0x80) j = 7 - j;

            /* decode the ROM data */
            rom[A] = src ^ data_xortable[i][j];

            /* now decode the opcodes */
            /* pick the translation table from bits 0, 4, and 8 of the address */
            i = ((A >> 0) & 1) + (((A >> 4) & 1) << 1) + (((A >> 8) & 1) << 2);
            m_decrypted_opcodes[A] = src ^ opcode_xortable[i][j];
        }
    }



    void zaxxon_state::init_razmataz()
    {
        address_space &pgmspace = m_maincpu->space(AS_PROGRAM);

        /* additional input ports are wired */
        pgmspace.install_read_port(0xc004, 0xc004, 0x18f3, "SW04");
        pgmspace.install_read_port(0xc008, 0xc008, 0x18f3, "SW08");
        pgmspace.install_read_port(0xc00c, 0xc00c, 0x18f3, "SW0C");

        /* unknown behavior expected here */
        pgmspace.install_read_handler(0xc80a, 0xc80a, read8smo_delegate(*this, FUNC(zaxxon_state::razmataz_counter_r)));

        /* additional state saving */
        save_item(NAME(m_razmataz_dial_pos));
        save_item(NAME(m_razmataz_counter));
    }
#endif


    public partial class zaxxon : construct_ioport_helper
    {
        /*************************************
         *  Game drivers
         *************************************/

        static void zaxxon_state_zaxxon(machine_config config, device_t device) { zaxxon_state zaxxon_state = (zaxxon_state)device; zaxxon_state.zaxxon(config); }

        static zaxxon m_zaxxon = new zaxxon();

        static device_t device_creator_zaxxon(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new zaxxon_state(mconfig, type, tag); }

        /* these games run on standard Zaxxon hardware */
        public static readonly game_driver driver_zaxxon = GAME(device_creator_zaxxon, rom_zaxxon, "1982", "zaxxon", "0", zaxxon_state_zaxxon, m_zaxxon.construct_ioport_zaxxon, driver_device.empty_init, ROT90, "Sega", "Zaxxon (set 1, rev D)", MACHINE_IMPERFECT_SOUND | MACHINE_SUPPORTS_SAVE);
    }
}
