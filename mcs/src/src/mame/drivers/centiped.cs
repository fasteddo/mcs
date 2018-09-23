// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;


namespace mame
{
    partial class centiped_state : driver_device
    {
        /*************************************
         *
         *  Interrupts
         *
         *************************************/

        //TIMER_DEVICE_CALLBACK_MEMBER(centiped_state::generate_interrupt)
        public void generate_interrupt(timer_device timer, object ptr, int param)  // void *ptr, INT32 param) 
        {
            int scanline = param;

            /* IRQ is clocked on the rising edge of 16V, equal to the previous 32V */
            if ((scanline & 16) != 0)
                m_maincpu.target.execute().set_input_line(0, ((scanline - 1) & 32) != 0 ? line_state.ASSERT_LINE : line_state.CLEAR_LINE);

            /* do a partial update now to handle sprite multiplexing (Maze Invaders) */
            m_screen.target.update_partial(scanline);
        }


        //MACHINE_START_MEMBER(centiped_state,centiped)
        public void machine_start_centiped()
        {
            save_item(m_oldpos, "m_oldpos");
            save_item(m_sign, "m_sign");
            save_item(m_dsw_select, "m_dsw_select");
            save_item(m_prg_bank, "m_prg_bank");
        }


        //MACHINE_RESET_MEMBER(centiped_state,centiped)
        public void machine_reset_centiped()
        {
            m_maincpu.target.execute().set_input_line(0, line_state.CLEAR_LINE);
            m_prg_bank = 0;

            if (m_earom.found())
                earom_control_w(machine().dummy_space(), 0, 0);
        }


        //WRITE8_MEMBER(centiped_state::irq_ack_w)
        public void irq_ack_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_maincpu.target.execute().set_input_line(0, line_state.CLEAR_LINE);
        }


        /*************************************
         *
         *  Input ports
         *
         *************************************/

        /*
         * This wrapper routine is necessary because Centipede requires a direction bit
         * to be set or cleared. The direction bit is held until the mouse is moved
         * again.
         *
         * There is a 4-bit counter, and two inputs from the trackball: DIR and CLOCK.
         * CLOCK makes the counter move in the direction of DIR. Since DIR is latched
         * only when a CLOCK arrives, the DIR bit in the input port doesn't change
         * until the trackball actually moves.
         *
         * There is also a CLR input to the counter which could be used by the game to
         * clear the counter, but Centipede doesn't use it (though it would be a good
         * idea to support it anyway).
         *
         * The counter is read 240 times per second. There is no provision whatsoever
         * to prevent the counter from wrapping around between reads.
         */

        static readonly string [] read_trackball_portnames = new string[] { "IN0", "IN1", "IN2" };
        static readonly string [] read_trackball_tracknames = new string[] { "TRACK0_X", "TRACK0_Y", "TRACK1_X", "TRACK1_Y" };

        int read_trackball(int idx, int switch_port)
        {
            byte newpos;
            //static const char *const portnames[] = { "IN0", "IN1", "IN2" };
            //static const char *const tracknames[] = { "TRACK0_X", "TRACK0_Y", "TRACK1_X", "TRACK1_Y" };

            /* adjust idx if we're cocktail flipped */
            if (m_flipscreen != 0)
                idx += 2;

            /* if we're to read the dipswitches behind the trackball data, do it now */
            if (m_dsw_select != 0)
                return (int)(ioport(read_trackball_portnames[switch_port]).read() & 0x7f) | m_sign[idx];

            /* get the new position and adjust the result */
            newpos = (byte)ioport(read_trackball_tracknames[idx]).read();
            if (newpos != m_oldpos[idx])
            {
                m_sign[idx] = (byte)((newpos - m_oldpos[idx]) & 0x80);
                m_oldpos[idx] = newpos;
            }

            /* blend with the bits from the switch port */
            return (int)(ioport(read_trackball_portnames[switch_port]).read() & 0x70) | (m_oldpos[idx] & 0x0f) | m_sign[idx];
        }


        //READ8_MEMBER(centiped_state::centiped_IN0_r)
        public u8 centiped_IN0_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return (u8)read_trackball(0, 0);
        }


        //READ8_MEMBER(centiped_state::centiped_IN2_r)
        public u8 centiped_IN2_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return (u8)read_trackball(1, 2);
        }


#if false
        READ8_MEMBER(centiped_state::milliped_IN1_r)
        {
            return read_trackball(1, 1);
        }
#endif

#if false
        READ8_MEMBER(centiped_state::milliped_IN2_r)
        {
            uint8_t data = ioport("IN2")->read();

            /* MSH - 15 Feb, 2007
             * The P2 X Joystick inputs are not properly handled in
             * the Milliped code, so we are forcing the P2 inputs
             * into the P1 Joystick handler, this require remapping
             * the inputs, and has the good side effect of disabling
             * the actual Joy1 inputs while control_select is no zero.
             */
            if (m_control_select != 0)
            {
                /* Bottom 4 bits is our joystick inputs */
                uint8_t joy2data = ioport("IN3")->read() & 0x0f;
                data = data & 0xf0; /* Keep the top 4 bits */
                data |= (joy2data & 0x0a) >> 1; /* flip left and up */
                data |= (joy2data & 0x05) << 1; /* flip right and down */
            }
            return data;
        }
#endif

#if false
        WRITE_LINE_MEMBER(centiped_state::input_select_w)
        {
            m_dsw_select = !state;
        }
#endif

#if false
        /* used P2 controls if 1, P1 controls if 0 */
        WRITE_LINE_MEMBER(centiped_state::control_select_w)
        {
            m_control_select = state;
        }
#endif


#if false
        READ8_MEMBER(centiped_state::mazeinv_input_r)
        {
            static const char *const sticknames[] = { "STICK0", "STICK1", "STICK2", "STICK3" };

            return ioport(sticknames[m_control_select])->read();
        }
#endif


#if false
        WRITE8_MEMBER(centiped_state::mazeinv_input_select_w)
        {
            m_control_select = offset & 3;
        }
#endif

#if false
        READ8_MEMBER(centiped_state::bullsdrt_data_port_r)
        {
            switch (space.device().safe_pc())
            {
                case 0x0033:
                case 0x6b19:
                    return 0x01;

                default:
                    break;
            }

            return 0;
        }
#endif


        /*************************************
         *
         *  Output ports
         *
         *************************************/

        //READ8_MEMBER(centiped_state::caterplr_unknown_r)
        public u8 caterplr_unknown_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            throw new emu_unimplemented();
#if false
            return machine().rand() % 0xff;
#endif
        }


        //WRITE_LINE_MEMBER(centiped_state::coin_counter_left_w)
        public void coin_counter_left_w(int state)
        {
            machine().bookkeeping().coin_counter_w(0, state);
        }


        //WRITE_LINE_MEMBER(centiped_state::coin_counter_center_w)
        public void coin_counter_center_w(int state)
        {
            machine().bookkeeping().coin_counter_w(1, state);
        }


        //WRITE_LINE_MEMBER(centiped_state::coin_counter_right_w)
        public void coin_counter_right_w(int state)
        {
            machine().bookkeeping().coin_counter_w(2, state);
        }


        //WRITE_LINE_MEMBER(centiped_state::bullsdrt_coin_count_w)
        public void bullsdrt_coin_count_w(int state)
        {
            throw new emu_unimplemented();
#if false
            machine().bookkeeping().coin_counter_w(0, state);
#endif
        }


        /*************************************
         *
         *  High score EAROM
         *
         *************************************/

        //READ8_MEMBER(centiped_state::earom_read)
        public u8 earom_read(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return m_earom.target.data();
        }

        //WRITE8_MEMBER(centiped_state::earom_write)
        public void earom_write(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_earom.target.set_address((uint8_t)(offset & 0x3f));
            m_earom.target.set_data(data);
        }


        //WRITE8_MEMBER(centiped_state::earom_control_w)
        public void earom_control_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            // CK = DB0, C1 = /DB1, C2 = DB2, CS1 = DB3, /CS2 = GND
            m_earom.target.set_control((uint8_t)global.BIT(data, 3), 1, global.BIT(data, 1) == 0 ? (uint8_t)1 : (uint8_t)0, (uint8_t)global.BIT(data, 2));
            m_earom.target.set_clk(global.BIT(data, 0));
        }
    }


    public class centiped : device_init_helpers
    {
        /*************************************
         *
         *  Centipede CPU memory handlers
         *
         *************************************/

        //void centiped_state::centiped_base_map(address_map &map)
        void centiped_state_centiped_base_map(address_map map, device_t device)
        {
            centiped_state centiped_state = (centiped_state)device;

            map.global_mask(0x3fff);
            map.op(0x0000, 0x03ff).ram().share("rambase");
            map.op(0x0400, 0x07bf).ram().w(centiped_state.centiped_videoram_w).share("videoram");
            map.op(0x07c0, 0x07ff).ram().share("spriteram");
            map.op(0x0800, 0x0800).portr("DSW1");
            map.op(0x0801, 0x0801).portr("DSW2");
            map.op(0x0c00, 0x0c00).r(centiped_state.centiped_IN0_r);
            map.op(0x0c01, 0x0c01).portr("IN1");
            map.op(0x0c02, 0x0c02).r(centiped_state.centiped_IN2_r);
            map.op(0x0c03, 0x0c03).portr("IN3");
            map.op(0x1400, 0x140f).w(centiped_state.centiped_paletteram_w).share("paletteram");
            map.op(0x1600, 0x163f).nopr().w(centiped_state.earom_write);
            map.op(0x1680, 0x1680).w(centiped_state.earom_control_w);
            map.op(0x1700, 0x173f).r(centiped_state.earom_read);
            map.op(0x1800, 0x1800).w(centiped_state.irq_ack_w);
            map.op(0x1c00, 0x1c07).nopr().w("outlatch", centiped_state.ls259_device_write_d7_outlatch);
            map.op(0x2000, 0x2000).w("watchdog", centiped_state.watchdog_timer_device_reset_w);
            map.op(0x2000, 0x3fff).rom();
        }


        //void centiped_state::centiped_map(address_map &map)
        void centiped_state_centiped_map(address_map map, device_t device)
        {
            centiped_state state = (centiped_state)device;

            centiped_state_centiped_base_map(map, device);

            map.op(0x1000, 0x100f).rw("pokey", state.pokey_device_read, state.pokey_device_write);
        }


        /*************************************
         *
         *  Port definitions
         *
         *************************************/

        /* The input ports are identical for the real one and the bootleg one, except
           that one of the languages is Italian in the bootleg one instead of Spanish */

        //static INPUT_PORTS_START( centiped )
        void construct_ioport_centiped(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");
            PORT_BIT( 0x0f, IP_ACTIVE_HIGH, IPT_CUSTOM );   /* trackball data */
            PORT_DIPNAME( 0x10, 0x00, DEF_STR( Cabinet ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x10, DEF_STR( Cocktail ) );
            PORT_SERVICE( 0x20, IP_ACTIVE_LOW );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_VBLANK("screen");
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_CUSTOM );   /* trackball sign bit */

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_TILT );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_SERVICE1 );

            PORT_START("IN2");
            PORT_BIT( 0x0f, IP_ACTIVE_HIGH, IPT_CUSTOM );   /* trackball data */
            PORT_BIT( 0x70, IP_ACTIVE_HIGH, IPT_UNKNOWN );
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_CUSTOM );   /* trackball sign bit */

            PORT_START("IN3");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY();
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY();
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY();

            PORT_START("DSW1");
            PORT_DIPNAME( 0x03, 0x00, DEF_STR( Language ) );     PORT_DIPLOCATION("N9:!1,!2");
            PORT_DIPSETTING(    0x00, DEF_STR( English ) );
            PORT_DIPSETTING(    0x01, DEF_STR( German ) );
            PORT_DIPSETTING(    0x02, DEF_STR( French ) );
            PORT_DIPSETTING(    0x03, DEF_STR( Spanish ) );
            PORT_DIPNAME( 0x0c, 0x04, DEF_STR( Lives ) );        PORT_DIPLOCATION("N9:!3,!4");
            PORT_DIPSETTING(    0x00, "2" );
            PORT_DIPSETTING(    0x04, "3" );
            PORT_DIPSETTING(    0x08, "4" );
            PORT_DIPSETTING(    0x0c, "5" );
            PORT_DIPNAME( 0x30, 0x10, DEF_STR( Bonus_Life ) );   PORT_DIPLOCATION("N9:!5,!6");
            PORT_DIPSETTING(    0x00, "10000" );
            PORT_DIPSETTING(    0x10, "12000" );
            PORT_DIPSETTING(    0x20, "15000" );
            PORT_DIPSETTING(    0x30, "20000" );
            PORT_DIPNAME( 0x40, 0x40, DEF_STR( Difficulty ) );   PORT_DIPLOCATION("N9:!7");
            PORT_DIPSETTING(    0x40, DEF_STR( Easy ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Hard ) );
            PORT_DIPNAME( 0x80, 0x00, "Credit Minimum" );        PORT_DIPLOCATION("N9:!8");
            PORT_DIPSETTING(    0x00, "1" );
            PORT_DIPSETTING(    0x80, "2" );

            PORT_START("DSW2");
            PORT_DIPNAME( 0x03, 0x02, DEF_STR( Coinage ) );      PORT_DIPLOCATION("N8:!1,!2");
            PORT_DIPSETTING(    0x03, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x01, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Free_Play ) );
            PORT_DIPNAME( 0x0c, 0x00, "Right Coin" );            PORT_DIPLOCATION("N8:!3,!4");
            PORT_DIPSETTING(    0x00, "*1" );
            PORT_DIPSETTING(    0x04, "*4" );
            PORT_DIPSETTING(    0x08, "*5" );
            PORT_DIPSETTING(    0x0c, "*6" );
            PORT_DIPNAME( 0x10, 0x00, "Left Coin" );             PORT_DIPLOCATION("N8:!5");
            PORT_DIPSETTING(    0x00, "*1" );
            PORT_DIPSETTING(    0x10, "*2" );
            PORT_DIPNAME( 0xe0, 0x00, "Bonus Coins" );           PORT_DIPLOCATION("N8:!6,!7,!8");
            PORT_DIPSETTING(    0x00, DEF_STR( None ) );
            PORT_DIPSETTING(    0x20, "3 credits/2 coins" );
            PORT_DIPSETTING(    0x40, "5 credits/4 coins" );
            PORT_DIPSETTING(    0x60, "6 credits/4 coins" );
            PORT_DIPSETTING(    0x80, "6 credits/5 coins" );
            PORT_DIPSETTING(    0xa0, "4 credits/3 coins" );

            PORT_START("TRACK0_X");  /* IN6, fake trackball input port. */
            PORT_BIT( 0xff, 0x00, IPT_TRACKBALL_X ); PORT_SENSITIVITY(50); PORT_KEYDELTA(10); PORT_REVERSE();

            PORT_START("TRACK0_Y");  /* IN7, fake trackball input port. */
            PORT_BIT( 0xff, 0x00, IPT_TRACKBALL_Y ); PORT_SENSITIVITY(50); PORT_KEYDELTA(10);

            PORT_START("TRACK1_X");  /* IN8, fake trackball input port. */
            PORT_BIT( 0xff, 0x00, IPT_TRACKBALL_X ); PORT_SENSITIVITY(50); PORT_KEYDELTA(10); PORT_COCKTAIL();

            PORT_START("TRACK1_Y");  /* IN9, fake trackball input port. */
            PORT_BIT( 0xff, 0x00, IPT_TRACKBALL_Y ); PORT_SENSITIVITY(50); PORT_KEYDELTA(10); PORT_REVERSE(); PORT_COCKTAIL();

            INPUT_PORTS_END();
        }


        /*************************************
         *
         *  Graphics layouts: Centipede/Millipede
         *
         *************************************/

        static readonly gfx_layout charlayout = new gfx_layout(
            8,8,
            RGN_FRAC(1,2),
            2,
            digfx_global.ArrayCombineUInt32( RGN_FRAC(1,2), 0 ),
            digfx_global.ArrayCombineUInt32( 0, 1, 2, 3, 4, 5, 6, 7 ),
            digfx_global.ArrayCombineUInt32( 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 ),
            8*8
        );

        static readonly gfx_layout spritelayout = new gfx_layout(
            8,16,
            RGN_FRAC(1,2),
            2,
            digfx_global.ArrayCombineUInt32( RGN_FRAC(1,2), 0 ),
            digfx_global.ArrayCombineUInt32( 0, 1, 2, 3, 4, 5, 6, 7 ),
            digfx_global.ArrayCombineUInt32( 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
                    8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 ),
            16*8
        );

        //static GFXDECODE_START( gfx_centiped )
        static readonly gfx_decode_entry [] gfx_centiped = new gfx_decode_entry[]
        {
            GFXDECODE_ENTRY( "gfx1", 0, charlayout,     0, 1 ),
            GFXDECODE_ENTRY( "gfx1", 0, spritelayout,   4, 4*4*4 ),
            //GFXDECODE_END
        };

#if false
        static GFXDECODE_START( milliped )
            GFXDECODE_ENTRY( "gfx1", 0, charlayout,     0, 4 )
            GFXDECODE_ENTRY( "gfx1", 0, spritelayout, 4*4, 4*4*4*4 )
        GFXDECODE_END
#endif


        /*************************************
         *
         *  Machine drivers
         *
         *************************************/

        //MACHINE_CONFIG_START(centiped_state::centiped_base)
        void centiped_state_centiped_base(machine_config config, device_t owner, device_t device)
        {
            MACHINE_CONFIG_START(config, owner, device);

            centiped_state centiped_state = (centiped_state)helper_owner;

            /* basic machine hardware */
            MCFG_DEVICE_ADD("maincpu", m6502_device.M6502, 12096000/8);  /* 1.512 MHz (slows down to 0.75MHz while accessing playfield RAM) */

            MCFG_MACHINE_START_OVERRIDE(centiped_state.machine_start_centiped);
            MCFG_MACHINE_RESET_OVERRIDE(centiped_state.machine_reset_centiped);

            MCFG_DEVICE_ADD("earom", er2055_device.ER2055);

            LS259(config, centiped_state.outlatch);
            centiped_state.outlatch.target.q_out_cb(0).set(centiped_state.coin_counter_left_w).reg();
            centiped_state.outlatch.target.q_out_cb(1).set(centiped_state.coin_counter_center_w).reg();
            centiped_state.outlatch.target.q_out_cb(2).set(centiped_state.coin_counter_right_w).reg();
            centiped_state.outlatch.target.q_out_cb(3).set_output("led0").invert().reg(); // LED 1
            centiped_state.outlatch.target.q_out_cb(4).set_output("led1").invert().reg(); // LED 2

            MCFG_WATCHDOG_ADD("watchdog");

            /* timer */
            MCFG_TIMER_DRIVER_ADD_SCANLINE("32v", centiped_state.generate_interrupt, "screen", 0, 16);

            /* video hardware */
            MCFG_SCREEN_ADD("screen", screen_type_enum.SCREEN_TYPE_RASTER);
            MCFG_SCREEN_REFRESH_RATE(60);
            MCFG_SCREEN_SIZE(32*8, 32*8);
            MCFG_SCREEN_VISIBLE_AREA(0*8, 32*8-1, 0*8, 30*8-1);
            MCFG_SCREEN_UPDATE_DRIVER(centiped_state.screen_update_centiped);
            MCFG_SCREEN_PALETTE("palette");

            MCFG_DEVICE_ADD("gfxdecode", gfxdecode_device.GFXDECODE);//, "palette", gfx_centiped);
            MCFG_DEVICE_ADD_gfxdecode_device("palette", gfx_centiped);
            MCFG_PALETTE_ADD("palette", 4+4*4*4*4);

            MCFG_VIDEO_START_OVERRIDE(centiped_state.video_start_centiped);

            MACHINE_CONFIG_END();
        }


        //MACHINE_CONFIG_START(centiped_state::centiped)
        void centiped_state_centiped(machine_config config, device_t owner, device_t device)
        {
            centiped_state_centiped_base(config, owner, device);

            MACHINE_CONFIG_START(config, owner, device);

            centiped_state centiped_state = (centiped_state)helper_owner;

            MCFG_DEVICE_MODIFY("maincpu");
            MCFG_DEVICE_PROGRAM_MAP(centiped_state_centiped_map);

            // M10
            centiped_state.outlatch.target.q_out_cb(7).set(centiped_state.flip_screen_w).reg();

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            MCFG_DEVICE_ADD("pokey", pokey_device.POKEY, 12096000/8);
            MCFG_POKEY_OUTPUT_OPAMP_LOW_PASS(RES_K(3.3), CAP_U(0.01), 5.0);
            MCFG_SOUND_ROUTE(ALL_OUTPUTS, "mono", 0.5);

            MACHINE_CONFIG_END();
        }


        /*************************************
         *
         *  ROM definitions
         *
         *************************************/

        /*
            Alt. part numbers for mask ROMs:

            136001-*03 = 136001-*07
            136001-*04 = 136001-*08
            136001-*05 = 136001-*09
            136001-*06 = 136001-*10
            136001-*01 = 136001-*11
            136001-*02 = 136001-*12
        */

        //ROM_START( centiped )
        static readonly List<tiny_rom_entry> rom_centiped = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "136001-407.d1",  0x2000, 0x0800, CRC("c4d995eb") + SHA1("d0b2f0461cfa35842045d40ffb65e777703b773e") ),
            ROM_LOAD( "136001-408.e1",  0x2800, 0x0800, CRC("bcdebe1b") + SHA1("53f3bf88a79ce40661c0a9381928e55d8c61777a") ),
            ROM_LOAD( "136001-409.fh1", 0x3000, 0x0800, CRC("66d7b04a") + SHA1("8fa758095b618085090491dfb5ea114cdc87f9df") ),
            ROM_LOAD( "136001-410.j1",  0x3800, 0x0800, CRC("33ce4640") + SHA1("780c2eb320f64fad6b265c0dada961646ed30174") ),

            ROM_REGION( 0x1000, "gfx1", 0 ),
            ROM_LOAD( "136001-211.f7",  0x0000, 0x0800, CRC("880acfb9") + SHA1("6c862352c329776f2f9974a0df9dbe41f9dbc361") ),
            ROM_LOAD( "136001-212.hj7", 0x0800, 0x0800, CRC("b1397029") + SHA1("974c03d29aeca672fffa4dfc00a06be6a851aacb") ),

            ROM_REGION( 0x0100, "proms", 0 ),
            ROM_LOAD( "136001-213.p4",  0x0000, 0x0100, CRC("6fa3093a") + SHA1("2b7aeca74c1ae4156bf1878453a047330f96f0a8") ),

            ROM_END(),
        };



        static centiped m_centipede = new centiped();


        static device_t device_creator_centipede(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new centiped_state(mconfig, type, tag); }


        // Centipede, Millipede, and clones
        //                                                          creator,                  rom           YEAR,   NAME,       PARENT,  MACHINE,                              INPUT,                                  INIT,                      MONITOR,COMPANY, FULLNAME,FLAGS
        public static readonly game_driver driver_centipede = GAME( device_creator_centipede, rom_centiped, "1980", "centiped", null,    m_centipede.centiped_state_centiped,  m_centipede.construct_ioport_centiped,  driver_device.empty_init,  ROT270, "Atari", "Centipede (revision 4)", MACHINE_SUPPORTS_SAVE );  /* 1 Player Only with Timer Options */
    }
}
