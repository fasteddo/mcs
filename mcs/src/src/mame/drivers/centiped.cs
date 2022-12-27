// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;

using static mame._74259_global;
using static mame.diexec_global;
using static mame.digfx_global;
using static mame.disound_global;
using static mame.drawgfx_global;
using static mame.driver_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.er2055_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.m6502_global;
using static mame.pokey_global;
using static mame.rescap_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.timer_global;
using static mame.util;
using static mame.watchdog_global;


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
        void generate_interrupt(timer_device timer, object ptr, int param)  // void *ptr, INT32 param) 
        {
            int scanline = param;

            /* IRQ is clocked on the rising edge of 16V, equal to the previous 32V */
            if ((scanline & 16) != 0)
                m_maincpu.op0.set_input_line(0, ((scanline - 1) & 32) != 0 ? ASSERT_LINE : CLEAR_LINE);

            /* do a partial update now to handle sprite multiplexing (Maze Invaders) */
            m_screen.op0.update_partial(scanline);
        }


        protected override void machine_start()
        {
            save_item(NAME(new { m_oldpos }));
            save_item(NAME(new { m_sign }));
            save_item(NAME(new { m_dsw_select }));

            std.fill(m_oldpos, (uint8_t)0);
            std.fill(m_sign, (uint8_t)0);
            m_dsw_select = 0;
        }


        //MACHINE_RESET_MEMBER(centiped_state,centiped)
        void machine_reset_centiped()
        {
            m_maincpu.op0.set_input_line(0, CLEAR_LINE);

            if (m_earom.found())
                earom_control_w(0);
        }


        void irq_ack_w(uint8_t data)
        {
            m_maincpu.op0.set_input_line(0, CLEAR_LINE);
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
            uint8_t newpos;
            //static const char *const portnames[] = { "IN0", "IN1", "IN2" };
            //static const char *const tracknames[] = { "TRACK0_X", "TRACK0_Y", "TRACK1_X", "TRACK1_Y" };

            /* adjust idx if we're cocktail flipped */
            if (m_flipscreen != 0)
                idx += 2;

            /* if we're to read the dipswitches behind the trackball data, do it now */
            if (m_dsw_select != 0)
                return (int)(ioport(read_trackball_portnames[switch_port]).read() & 0x7f) | m_sign[idx];

            /* get the new position and adjust the result */
            newpos = (uint8_t)ioport(read_trackball_tracknames[idx]).read();
            if (newpos != m_oldpos[idx])
            {
                m_sign[idx] = (uint8_t)((newpos - m_oldpos[idx]) & 0x80);
                m_oldpos[idx] = newpos;
            }

            /* blend with the bits from the switch port */
            return (int)(ioport(read_trackball_portnames[switch_port]).read() & 0x70) | (m_oldpos[idx] & 0x0f) | m_sign[idx];
        }


        uint8_t centiped_IN0_r()
        {
            return (uint8_t)read_trackball(0, 0);
        }


        uint8_t centiped_IN2_r()
        {
            return (uint8_t)read_trackball(1, 2);
        }


        uint8_t milliped_IN1_r()
        {
            return (uint8_t)read_trackball(1, 1);
        }


        uint8_t milliped_IN2_r()
        {
            uint8_t data = (uint8_t)ioport("IN2").read();

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
                uint8_t joy2data = (uint8_t)(ioport("IN3").read() & 0x0f);
                data = (uint8_t)(data & 0xf0); /* Keep the top 4 bits */
                data |= (uint8_t)((joy2data & 0x0a) >> 1); /* flip left and up */
                data |= (uint8_t)((joy2data & 0x05) << 1); /* flip right and down */
            }

            return data;
        }


        //WRITE_LINE_MEMBER(centiped_state::input_select_w)
        void input_select_w(int state)
        {
            m_dsw_select = state == 0 ? (uint8_t)1 : (uint8_t)0;
        }


        //WRITE_LINE_MEMBER(centiped_state::control_select_w)
        void control_select_w(int state)
        {
            m_control_select = (uint8_t)state;
        }


        //uint8_t centiped_state::mazeinv_input_r()
        //void centiped_state::mazeinv_input_select_w(offs_t offset, uint8_t data)
        //uint8_t centiped_state::bullsdrt_data_port_r()


        /*************************************
         *
         *  Output ports
         *
         *************************************/

        uint8_t caterplr_unknown_r()
        {
            throw new emu_unimplemented();
#if false
            return machine().rand() % 0xff;
#endif
        }


        //WRITE_LINE_MEMBER(centiped_state::coin_counter_left_w)
        void coin_counter_left_w(int state)
        {
            machine().bookkeeping().coin_counter_w(0, state);
        }


        //WRITE_LINE_MEMBER(centiped_state::coin_counter_center_w)
        void coin_counter_center_w(int state)
        {
            machine().bookkeeping().coin_counter_w(1, state);
        }


        //WRITE_LINE_MEMBER(centiped_state::coin_counter_right_w)
        void coin_counter_right_w(int state)
        {
            machine().bookkeeping().coin_counter_w(2, state);
        }


        //WRITE_LINE_MEMBER(centiped_state::bullsdrt_coin_count_w)
        void bullsdrt_coin_count_w(int state)
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

        uint8_t earom_read()
        {
            return m_earom.op0.data();
        }


        void earom_write(offs_t offset, uint8_t data)
        {
            m_earom.op0.set_address((uint8_t)(offset & 0x3f));
            m_earom.op0.set_data(data);
        }


        void earom_control_w(uint8_t data)
        {
            // CK = DB0, C1 = /DB1, C2 = DB2, CS1 = DB3, /CS2 = GND
            m_earom.op0.set_control((uint8_t)BIT(data, 3), 1, BIT(data, 1) == 0 ? (uint8_t)1 : (uint8_t)0, (uint8_t)BIT(data, 2));
            m_earom.op0.set_clk(BIT(data, 0));
        }


        /*************************************
         *
         *  Centipede CPU memory handlers
         *
         *************************************/

        void centiped_base_map(address_map map, device_t device)
        {
            map.global_mask(0x3fff);
            map.op(0x0000, 0x03ff).ram().share("rambase");
            map.op(0x0400, 0x07bf).ram().w(centiped_videoram_w).share("videoram");
            map.op(0x07c0, 0x07ff).ram().share("spriteram");
            map.op(0x0800, 0x0800).portr("DSW1");
            map.op(0x0801, 0x0801).portr("DSW2");
            map.op(0x0c00, 0x0c00).r(centiped_IN0_r);
            map.op(0x0c01, 0x0c01).portr("IN1");
            map.op(0x0c02, 0x0c02).r(centiped_IN2_r);
            map.op(0x0c03, 0x0c03).portr("IN3");
            map.op(0x1400, 0x140f).w(centiped_paletteram_w).share("paletteram");
            map.op(0x1600, 0x163f).nopr().w(earom_write);
            map.op(0x1680, 0x1680).w(earom_control_w);
            map.op(0x1700, 0x173f).r(earom_read);
            map.op(0x1800, 0x1800).w(irq_ack_w);
            map.op(0x1c00, 0x1c07).nopr().w("outlatch", (offset, data) => { ((addressable_latch_device)subdevice("outlatch")).write_d7(offset, data); });  //FUNC(ls259_device::write_d7));
            map.op(0x2000, 0x2000).w("watchdog", (data) => { ((watchdog_timer_device)subdevice("watchdog")).reset_w(data); });  //FUNC(watchdog_timer_device::reset_w));
            map.op(0x2000, 0x3fff).rom();
        }


        void centiped_map(address_map map, device_t device)
        {
            centiped_base_map(map, device);

            map.op(0x1000, 0x100f).rw("pokey", (offset) => { return ((pokey_device)subdevice("pokey")).read(offset); }, (offset, data) => { ((pokey_device)subdevice("pokey")).write(offset, data); });  //rw("pokey", FUNC(pokey_device::read), FUNC(pokey_device::write));
        }


        /*************************************
         *
         *  Millipede CPU memory handlers
         *
         *************************************/
        void milliped_map(address_map map, device_t device)
        {
            map.global_mask(0x7fff);
            map.op(0x0000, 0x03ff).ram();
            map.op(0x0400, 0x040f).rw("pokey", (offset) => { return ((pokey_device)subdevice("pokey")).read(offset); }, (offset, data) => { ((pokey_device)subdevice("pokey")).write(offset, data); });
            map.op(0x0800, 0x080f).rw("pokey2", (offset) => { return ((pokey_device)subdevice("pokey2")).read(offset); }, (offset, data) => { ((pokey_device)subdevice("pokey2")).write(offset, data); });
            map.op(0x1000, 0x13bf).ram().w(centiped_videoram_w).share("videoram");
            map.op(0x13c0, 0x13ff).ram().share("spriteram");
            map.op(0x2000, 0x2000).r(centiped_IN0_r);
            map.op(0x2001, 0x2001).r(milliped_IN1_r);
            map.op(0x2010, 0x2010).r(milliped_IN2_r);
            map.op(0x2011, 0x2011).portr("IN3");
            map.op(0x2030, 0x2030).r(earom_read);
            map.op(0x2480, 0x249f).w(milliped_paletteram_w).share("paletteram");
            map.op(0x2500, 0x2507).w("outlatch", (offset, data) => { ((ls259_device)subdevice("outlatch")).write_d7(offset, data); });
            map.op(0x2600, 0x2600).w(irq_ack_w);
            map.op(0x2680, 0x2680).w("watchdog", (data) => { ((watchdog_timer_device)subdevice("watchdog")).reset_w(data); });
            map.op(0x2700, 0x2700).w(earom_control_w);
            map.op(0x2780, 0x27bf).w(earom_write);
            map.op(0x4000, 0x7fff).rom();
        }
    }


    partial class centiped : construct_ioport_helper
    {
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


        //static INPUT_PORTS_START( milliped )
        void construct_ioport_milliped(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");   /* $2000 */ /* see port 6 for x trackball */
            PORT_DIPNAME(0x03, 0x00, DEF_STR( Language ) );      PORT_DIPLOCATION("P8:!1,!2");
            PORT_DIPSETTING(   0x00, DEF_STR( English ) );
            PORT_DIPSETTING(   0x01, DEF_STR( German ) );
            PORT_DIPSETTING(   0x02, DEF_STR( French ) );
            PORT_DIPSETTING(   0x03, DEF_STR( Spanish ) );
            PORT_DIPNAME(0x0c, 0x04, "Bonus" );                  PORT_DIPLOCATION("P8:!3,!4");
            PORT_DIPSETTING(   0x00, "0" );
            PORT_DIPSETTING(   0x04, "0 1x" );
            PORT_DIPSETTING(   0x08, "0 1x 2x" );
            PORT_DIPSETTING(   0x0c, "0 1x 2x 3x" );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_VBLANK("screen");
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_CUSTOM );       /* trackball sign bit */

            PORT_START("IN1");   /* $2001 */ /* see port 7 for y trackball */
            /* these bits are unused */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_DIPNAME(0x04, 0x00, "Credit Minimum" );         PORT_DIPLOCATION("P8:!7");
            PORT_DIPSETTING(   0x00, "1" );
            PORT_DIPSETTING(   0x04, "2" );
            PORT_DIPNAME(0x08, 0x00, "Coin Counters" );          PORT_DIPLOCATION("P8:!8");
            PORT_DIPSETTING(   0x00, "1" );
            PORT_DIPSETTING(   0x08, "2" );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_CUSTOM );       /* trackball sign bit */

            PORT_START("IN2");   /* $2010 */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_TILT );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_SERVICE1 );

            PORT_START("IN3");   /* $2011 */
            /* Note, joystick X input for player 2 are bad in software */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_DIPNAME(0x20, 0x20, DEF_STR( Cabinet ) );
            PORT_DIPSETTING(   0x20, DEF_STR( Upright ) );
            PORT_DIPSETTING(   0x00, DEF_STR( Cocktail ) );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_SERVICE( 0x80, IP_ACTIVE_LOW );

            PORT_START("DSW1");  /* $0408 */
            PORT_DIPNAME(0x01, 0x00, "Millipede Head" );         PORT_DIPLOCATION("D5:!1");
            PORT_DIPSETTING(   0x00, DEF_STR( Easy ) );
            PORT_DIPSETTING(   0x01, DEF_STR( Hard ) );
            PORT_DIPNAME(0x02, 0x00, "Beetle" );                 PORT_DIPLOCATION("D5:!2");
            PORT_DIPSETTING(   0x00, DEF_STR( Easy ) );
            PORT_DIPSETTING(   0x02, DEF_STR( Hard ) );
            PORT_DIPNAME(0x0c, 0x04, DEF_STR( Lives ) );         PORT_DIPLOCATION("D5:!3,!4");
            PORT_DIPSETTING(   0x00, "2" );
            PORT_DIPSETTING(   0x04, "3" );
            PORT_DIPSETTING(   0x08, "4" );
            PORT_DIPSETTING(   0x0c, "5" );
            PORT_DIPNAME(0x30, 0x10, DEF_STR( Bonus_Life ) );    PORT_DIPLOCATION("D5:!5,!6");
            PORT_DIPSETTING(   0x00, "12000" );
            PORT_DIPSETTING(   0x10, "15000" );
            PORT_DIPSETTING(   0x20, "20000" );
            PORT_DIPSETTING(   0x30, DEF_STR( None ) );
            PORT_DIPNAME(0x40, 0x00, "Spider" );                 PORT_DIPLOCATION("D5:!7");
            PORT_DIPSETTING(   0x00, DEF_STR( Easy ) );
            PORT_DIPSETTING(   0x40, DEF_STR( Hard ) );
            PORT_DIPNAME(0x80, 0x00, "Starting Score Select" );  PORT_DIPLOCATION("D5:!8");
            PORT_DIPSETTING(   0x80, DEF_STR( Off ) );
            PORT_DIPSETTING(   0x00, DEF_STR( On ) );

            PORT_START("DSW2");  /* $0808 */
            PORT_DIPNAME(0x03, 0x02, DEF_STR( Coinage ) );       PORT_DIPLOCATION("B5:!1,!2");
            PORT_DIPSETTING(   0x03, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(   0x02, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(   0x01, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(   0x00, DEF_STR( Free_Play ) );
            PORT_DIPNAME(0x0c, 0x00, "Right Coin" );             PORT_DIPLOCATION("B5:!3,!4");
            PORT_DIPSETTING(   0x00, "*1" );
            PORT_DIPSETTING(   0x04, "*4" );
            PORT_DIPSETTING(   0x08, "*5" );
            PORT_DIPSETTING(   0x0c, "*6" );
            PORT_DIPNAME(0x10, 0x00, "Left Coin" );              PORT_DIPLOCATION("B5:!5");
            PORT_DIPSETTING(   0x00, "*1" );
            PORT_DIPSETTING(   0x10, "*2" );
            PORT_DIPNAME(0xe0, 0x00, "Bonus Coins" );            PORT_DIPLOCATION("B5:!6,!7,!8");
            PORT_DIPSETTING(   0x00, DEF_STR( None ) );
            PORT_DIPSETTING(   0x20, "3 credits/2 coins" );
            PORT_DIPSETTING(   0x40, "5 credits/4 coins" );
            PORT_DIPSETTING(   0x60, "6 credits/4 coins" );
            PORT_DIPSETTING(   0x80, "6 credits/5 coins" );
            PORT_DIPSETTING(   0xa0, "4 credits/3 coins" );
            PORT_DIPSETTING(   0xc0, "Demo Mode" );

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
    }


    partial class centiped_state : driver_device
    {
        /*************************************
         *
         *  Graphics layouts: Centipede/Millipede
         *
         *************************************/

        static readonly gfx_layout charlayout = new gfx_layout(
            8,8,
            RGN_FRAC(1,2),
            2,
            ArrayCombineUInt32( RGN_FRAC(1,2), 0 ),
            ArrayCombineUInt32( 0, 1, 2, 3, 4, 5, 6, 7 ),
            ArrayCombineUInt32( 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 ),
            8*8
        );

        static readonly gfx_layout spritelayout = new gfx_layout(
            8,16,
            RGN_FRAC(1,2),
            2,
            ArrayCombineUInt32( RGN_FRAC(1,2), 0 ),
            ArrayCombineUInt32( 0, 1, 2, 3, 4, 5, 6, 7 ),
            ArrayCombineUInt32( 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
                    8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 ),
            16*8
        );

        //static GFXDECODE_START( gfx_centiped )
        static readonly gfx_decode_entry [] gfx_centiped =
        {
            GFXDECODE_ENTRY( "gfx1", 0, charlayout,     0, 1 ),
            GFXDECODE_ENTRY( "gfx1", 0, spritelayout,   4, 4*4*4 ),
            //GFXDECODE_END
        };


        //static GFXDECODE_START( milliped )
        static readonly gfx_decode_entry [] gfx_milliped =
        {
            GFXDECODE_ENTRY( "gfx1", 0, charlayout,     0, 4 ),
            GFXDECODE_ENTRY( "gfx1", 0, spritelayout, 4*4, 4*4*4*4 ),
            //GFXDECODE_END
        };


        /*************************************
         *
         *  Machine drivers
         *
         *************************************/

        void centiped_base(machine_config config)
        {
            /* basic machine hardware */
            M6502(config, m_maincpu, 12096000/8);  /* 1.512 MHz (slows down to 0.75MHz while accessing playfield RAM) */

            MCFG_MACHINE_RESET_OVERRIDE(config, machine_reset_centiped);

            ER2055(config, m_earom);

            LS259(config, m_outlatch);
            m_outlatch.op0.q_out_cb<u32_const_0>().set((write_line_delegate)coin_counter_left_w).reg();
            m_outlatch.op0.q_out_cb<u32_const_1>().set((write_line_delegate)coin_counter_center_w).reg();
            m_outlatch.op0.q_out_cb<u32_const_2>().set((write_line_delegate)coin_counter_right_w).reg();
            m_outlatch.op0.q_out_cb<u32_const_3>().set_output("led0").invert().reg(); // LED 1
            m_outlatch.op0.q_out_cb<u32_const_4>().set_output("led1").invert().reg(); // LED 2

            WATCHDOG_TIMER(config, "watchdog").set_vblank_count("screen", 8);

            /* timer */
            TIMER(config, "32v").configure_scanline(generate_interrupt, "screen", 0, 16);

            /* video hardware */
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_refresh_hz(60);
            m_screen.op0.set_size(32*8, 32*8);
            m_screen.op0.set_visarea(0*8, 32*8-1, 0*8, 30*8-1);
            m_screen.op0.set_screen_update(screen_update_centiped);
            m_screen.op0.set_palette(m_palette);

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_centiped);
            PALETTE(config, m_palette).set_entries(4+4*4*4*4);

            MCFG_VIDEO_START_OVERRIDE(config, video_start_centiped);
        }


        public void centiped(machine_config config)
        {
            centiped_base(config);

            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, centiped_map);

            // M10
            m_outlatch.op0.q_out_cb<u32_const_7>().set((write_line_delegate)flip_screen_w).reg();

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            pokey_device pokey = POKEY(config, "pokey", 12096000/8);
            pokey.set_output_opamp_low_pass(RES_K(3.3), CAP_U(0.01), 5.0);
            pokey.disound.add_route(ALL_OUTPUTS, "mono", 0.5);
        }


        public void milliped(machine_config config)
        {
            centiped_base(config);

            /* basic machine hardware */
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, milliped_map);

            // 12E
            m_outlatch.op0.q_out_cb<u32_const_5>().set((write_line_delegate)input_select_w).reg(); // TBEN
            m_outlatch.op0.q_out_cb<u32_const_6>().set((write_line_delegate)flip_screen_w).reg();
            m_outlatch.op0.q_out_cb<u32_const_7>().set((write_line_delegate)control_select_w).reg(); // CNTRLSEL

            /* video hardware */
            m_gfxdecode.op0.digfx.set_info(gfx_milliped);
            m_palette.op0.set_entries(4*4+4*4*4*4*4);

            MCFG_VIDEO_START_OVERRIDE(config, video_start_milliped);
            m_screen.op0.set_screen_update(screen_update_milliped);

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            pokey_device pokey1 = POKEY(config, "pokey", 12096000 / 8);
            pokey1.allpot_r().set_ioport("DSW1").reg();
            pokey1.disound.add_route(ALL_OUTPUTS, "mono", 0.50);

            pokey_device pokey2 = POKEY(config, "pokey2", 12096000 / 8);
            pokey2.allpot_r().set_ioport("DSW2").reg();
            pokey2.disound.add_route(ALL_OUTPUTS, "mono", 0.50);
        }
    }


    partial class centiped : construct_ioport_helper
    {
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
        static readonly MemoryContainer<tiny_rom_entry> rom_centiped = new MemoryContainer<tiny_rom_entry>()
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

            ROM_END,
        };


        //ROM_START( milliped )
        static readonly MemoryContainer<tiny_rom_entry> rom_milliped = new MemoryContainer<tiny_rom_entry>()
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "136013-104.mn1", 0x4000, 0x1000, CRC("40711675") + SHA1("b595d6a0f5d3c611ade1b83a94c3b909d2124dc4") ),
            ROM_LOAD( "136013-103.l1",  0x5000, 0x1000, CRC("fb01baf2") + SHA1("9c1d0bbc20bf25dd21761a311fd1ed80aa029241") ),
            ROM_LOAD( "136013-102.jk1", 0x6000, 0x1000, CRC("62e137e0") + SHA1("9fe40db55ba1d20d4f11704f7f5df9ff75b87f30") ),
            ROM_LOAD( "136013-101.h1",  0x7000, 0x1000, CRC("46752c7d") + SHA1("ab06b1fd80271849946f90757b3837b617394929") ),

            ROM_REGION( 0x1000, "gfx1", 0 ),
            ROM_LOAD( "136013-107.r5", 0x0000, 0x0800, CRC("68c3437a") + SHA1("4c7ea33d9501456ee8f5a642da7d6c972f2bb90d") ),
            ROM_LOAD( "136013-106.p5", 0x0800, 0x0800, CRC("f4468045") + SHA1("602fcc7290f9f4eacb841c76665961ebf4307f80") ),

            ROM_REGION( 0x0100, "proms", 0 ),
            ROM_LOAD( "136001-213.e7", 0x0000, 0x0100, CRC("6fa3093a") + SHA1("2b7aeca74c1ae4156bf1878453a047330f96f0a8") ), // Sync PROM

            ROM_END,
        };


        static void centiped_state_centiped(machine_config config, device_t device) { centiped_state centiped_state = (centiped_state)device; centiped_state.centiped(config); }
        static void centiped_state_milliped(machine_config config, device_t device) { centiped_state centiped_state = (centiped_state)device; centiped_state.milliped(config); }


        static centiped m_centiped = new centiped();


        static device_t device_creator_centipede(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new centiped_state(mconfig, type, tag); }
        static device_t device_creator_milliped(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new centiped_state(mconfig, type, tag); }


        // Centipede, Millipede, and clones
        //                                                         creator,                  rom           YEAR,   NAME,       PARENT, MACHINE,                 INPUT,                                INIT,                     MONITOR, COMPANY, FULLNAME,                 FLAGS
        public static readonly game_driver driver_centipede = GAME(device_creator_centipede, rom_centiped, "1980", "centiped", "0",    centiped_state_centiped, m_centiped.construct_ioport_centiped, driver_device.empty_init, ROT270,  "Atari", "Centipede (revision 4)", MACHINE_SUPPORTS_SAVE);  /* 1 Player Only with Timer Options */
        public static readonly game_driver driver_milliped  = GAME(device_creator_milliped,  rom_milliped, "1982", "milliped", "0",    centiped_state_milliped, m_centiped.construct_ioport_milliped, driver_device.empty_init, ROT270,  "Atari", "Millipede",              MACHINE_SUPPORTS_SAVE);
    }
}
