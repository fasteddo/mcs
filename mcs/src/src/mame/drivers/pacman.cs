// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using PointerU8 = mame.Pointer<System.Byte>;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;


namespace mame
{
    partial class pacman_state : driver_device
    {
        static readonly XTAL MASTER_CLOCK       = new XTAL(18432000);

        static readonly XTAL PIXEL_CLOCK        = MASTER_CLOCK/3;

        /* H counts from 128->511, HBLANK starts at 144 and ends at 240 */
        const UInt32 HTOTAL             = 384;
        const UInt32 HBEND              = 0;     /*(96+16)*/
        const UInt32 HBSTART            = 288;   /*(16)*/

        const UInt32 VTOTAL             = 264;
        const UInt32 VBEND              = 0;     /*(16)*/
        const UInt32 VBSTART            = 224;   /*(224+16)*/


        /*************************************
         *
         *  Interrupts
         *
         *************************************/

        //WRITE_LINE_MEMBER(pacman_state::vblank_irq)
        void vblank_irq(int state)
        {
            if (state != 0 && m_irq_mask)
                m_maincpu.op[0].set_input_line(device_execute_interface.INPUT_LINE_IRQ0, ASSERT_LINE);
        }

        //INTERRUPT_GEN_MEMBER(periodic_irq);
        //WRITE_LINE_MEMBER(pacman_state::vblank_nmi)
        //DECLARE_WRITE_LINE_MEMBER(s2650_interrupt);

        //WRITE_LINE_MEMBER(pacman_state::irq_mask_w)
        void irq_mask_w(int state)
        {
            m_irq_mask = state != 0;
            if (state == 0)
                m_maincpu.op[0].set_input_line(device_execute_interface.INPUT_LINE_IRQ0, CLEAR_LINE);
        }

        void pacman_interrupt_vector_w(uint8_t data)
        {
            m_interrupt_vector = data;
        }


        //IRQ_CALLBACK_MEMBER(pacman_state::interrupt_vector_r)
        int interrupt_vector_r(device_t device, int irqline)
        {
            return m_interrupt_vector;
        }


        /*************************************
        *
        *  LEDs/coin counters
        *
        *************************************/

        //WRITE_LINE_MEMBER(pacman_state::coin_counter_w)
        void coin_counter_w(int state)
        {
            machine().bookkeeping().coin_counter_w(0, state);
        }


        //WRITE_LINE_MEMBER(pacman_state::coin_lockout_global_w)
        void coin_lockout_global_w(int state)
        {
            machine().bookkeeping().coin_lockout_global_w(state == 0 ? 1 : 0);
        }


        void mspacman_enable_decode_latch(running_machine m) { m.root_device().membank("bank1").set_entry(1); }  //#define mspacman_enable_decode_latch(m)  m.root_device().membank("bank1")->set_entry(1)
        void mspacman_disable_decode_latch(running_machine m) { m.root_device().membank("bank1").set_entry(0); }  //#define mspacman_disable_decode_latch(m) m.root_device().membank("bank1")->set_entry(0)

        // any access to these ROM addresses disables the decoder, and all you see is the original Pac-Man code
        uint8_t mspacman_disable_decode_r_0x0038(offs_t offset) { mspacman_disable_decode_latch(machine()); return memregion("maincpu").base_()[offset + 0x0038]; }
        uint8_t mspacman_disable_decode_r_0x03b0(offs_t offset) { mspacman_disable_decode_latch(machine()); return memregion("maincpu").base_()[offset+0x03b0]; }
        uint8_t mspacman_disable_decode_r_0x1600(offs_t offset) { mspacman_disable_decode_latch(machine()); return memregion("maincpu").base_()[offset+0x1600]; }
        uint8_t mspacman_disable_decode_r_0x2120(offs_t offset) { mspacman_disable_decode_latch(machine()); return memregion("maincpu").base_()[offset+0x2120]; }
        uint8_t mspacman_disable_decode_r_0x3ff0(offs_t offset) { mspacman_disable_decode_latch(machine()); return memregion("maincpu").base_()[offset+0x3ff0]; }
        uint8_t mspacman_disable_decode_r_0x8000(offs_t offset) { mspacman_disable_decode_latch(machine()); return memregion("maincpu").base_()[offset+0x8000]; }
        uint8_t mspacman_disable_decode_r_0x97f0(offs_t offset) { mspacman_disable_decode_latch(machine()); return memregion("maincpu").base_()[offset+0x97f0]; }
        void mspacman_disable_decode_w(uint8_t data) { mspacman_disable_decode_latch(machine()); }

        // any access to these ROM addresses enables the decoder, and you'll see the Ms. Pac-Man code
        uint8_t mspacman_enable_decode_r_0x3ff8(offs_t offset) { mspacman_enable_decode_latch(machine()); return memregion("maincpu").base_()[offset+0x3ff8+0x10000]; }
        void mspacman_enable_decode_w(uint8_t data) { mspacman_enable_decode_latch(machine()); }


        uint8_t pacman_read_nop()
        {
            // Return value of reading the bus with no devices enabled.
            // This seems to be common but more tests are needed. Ms Pacman reads bytes in sequence
            // until it hits a 0 for a delimiter, including empty areas.  It writes to "random"
            // addresses each time. This causes the maze to invert sometimes.  See code at $95c3 where
            // level($4e13)=134. DW
            // tests on exactly what determines the value returned have thus far proved inconclusive
            return 0xbf;
        }


        /*************************************
         *
         *  Main CPU memory handlers
         *
         *************************************/

        void pacman_map(address_map map, device_t device)
        {
            //A lot of games don't have an a15 at the cpu.  Generally only games with a cpu daughter board can access the full 32k of romspace.
            map.op(0x0000, 0x3fff).mirror(0x8000).rom();
            map.op(0x4000, 0x43ff).mirror(0xa000).ram().w(pacman_videoram_w).share("videoram");
            map.op(0x4400, 0x47ff).mirror(0xa000).ram().w(pacman_colorram_w).share("colorram");
            map.op(0x4800, 0x4bff).mirror(0xa000).r(pacman_read_nop).nopw();
            map.op(0x4c00, 0x4fef).mirror(0xa000).ram();
            map.op(0x4ff0, 0x4fff).mirror(0xa000).ram().share("spriteram");
            map.op(0x5000, 0x5007).mirror(0xaf38).w(m_mainlatch, (offset, data) => { m_mainlatch.op[0].write_d0(offset, data); });  //FUNC(addressable_latch_device::write_d0));
            map.op(0x5040, 0x505f).mirror(0xaf00).w(m_namco_sound, (offset, data) => { m_namco_sound.op[0].pacman_sound_w(offset, data); });  //FUNC(namco_device::pacman_sound_w));
            map.op(0x5060, 0x506f).mirror(0xaf00).writeonly().share("spriteram2");
            map.op(0x5070, 0x507f).mirror(0xaf00).nopw();
            map.op(0x5080, 0x5080).mirror(0xaf3f).nopw();
            map.op(0x50c0, 0x50c0).mirror(0xaf3f).w(m_watchdog, (data) => { m_watchdog.op[0].reset_w(data); });  //FUNC(watchdog_timer_device::reset_w));
            map.op(0x5000, 0x5000).mirror(0xaf3f).portr("IN0");
            map.op(0x5040, 0x5040).mirror(0xaf3f).portr("IN1");
            map.op(0x5080, 0x5080).mirror(0xaf3f).portr("DSW1");
            map.op(0x50c0, 0x50c0).mirror(0xaf3f).portr("DSW2");
        }


        void mspacman_map(address_map map, device_t device)
        {
            /* start with 0000-3fff and 8000-bfff mapped to the ROMs */
            map.op(0x0000, 0xffff).bankr("bank1");
            map.op(0x4000, 0x7fff).mirror(0x8000).unmaprw();

            map.op(0x4000, 0x43ff).mirror(0xa000).ram().w(pacman_videoram_w).share("videoram");
            map.op(0x4400, 0x47ff).mirror(0xa000).ram().w(pacman_colorram_w).share("colorram");
            map.op(0x4800, 0x4bff).mirror(0xa000).r(pacman_read_nop).nopw();
            map.op(0x4c00, 0x4fef).mirror(0xa000).ram();
            map.op(0x4ff0, 0x4fff).mirror(0xa000).ram().share("spriteram");
            map.op(0x5000, 0x5007).mirror(0xaf38).w(m_mainlatch, (offset, data) => { m_mainlatch.op[0].write_d0(offset, data); });  //FUNC(ls259_device::write_d0));
            map.op(0x5040, 0x505f).mirror(0xaf00).w(m_namco_sound, (offset, data) => { m_namco_sound.op[0].pacman_sound_w(offset, data); });  //FUNC(namco_device::pacman_sound_w));
            map.op(0x5060, 0x506f).mirror(0xaf00).writeonly().share("spriteram2");
            map.op(0x5070, 0x507f).mirror(0xaf00).nopw();
            map.op(0x5080, 0x5080).mirror(0xaf3f).nopw();
            map.op(0x50c0, 0x50c0).mirror(0xaf3f).w(m_watchdog, (data) => { m_watchdog.op[0].reset_w(data); });  //FUNC(watchdog_timer_device::reset_w));
            map.op(0x5000, 0x5000).mirror(0xaf3f).portr("IN0");
            map.op(0x5040, 0x5040).mirror(0xaf3f).portr("IN1");
            map.op(0x5080, 0x5080).mirror(0xaf3f).portr("DSW1");
            map.op(0x50c0, 0x50c0).mirror(0xaf3f).portr("DSW2");

            /* overlay decode enable/disable on top */
            map.op(0x0038, 0x003f).rw(mspacman_disable_decode_r_0x0038, mspacman_disable_decode_w);
            map.op(0x03b0, 0x03b7).rw(mspacman_disable_decode_r_0x03b0, mspacman_disable_decode_w);
            map.op(0x1600, 0x1607).rw(mspacman_disable_decode_r_0x1600, mspacman_disable_decode_w);
            map.op(0x2120, 0x2127).rw(mspacman_disable_decode_r_0x2120, mspacman_disable_decode_w);
            map.op(0x3ff0, 0x3ff7).rw(mspacman_disable_decode_r_0x3ff0, mspacman_disable_decode_w);
            map.op(0x3ff8, 0x3fff).rw(mspacman_enable_decode_r_0x3ff8, mspacman_enable_decode_w);
            map.op(0x8000, 0x8007).rw(mspacman_disable_decode_r_0x8000, mspacman_disable_decode_w);
            map.op(0x97f0, 0x97f7).rw(mspacman_disable_decode_r_0x97f0, mspacman_disable_decode_w);
        }


        /*************************************
         *
         *  Main CPU port handlers
         *
         *************************************/

        void writeport(address_map map, device_t device)
        {
            map.global_mask(0xff);
            map.op(0x00, 0x00).w(pacman_interrupt_vector_w);    /* Pac-Man only */
        }
    }


    partial class pacman : global_object
    {
        /*************************************
         *
         *  Port definitions
         *
         *************************************/

        //static INPUT_PORTS_START( pacman )
        void construct_ioport_pacman(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_UP );    PORT_PLAYER(1); PORT_4WAY();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT );  PORT_PLAYER(1); PORT_4WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_PLAYER(1); PORT_4WAY();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN );  PORT_PLAYER(1); PORT_4WAY();
            PORT_DIPNAME(0x10, 0x10, "Rack Test (Cheat)" ); PORT_CODE(KEYCODE_F1);
            PORT_DIPSETTING(   0x10, DEF_STR( Off ) );
            PORT_DIPSETTING(   0x00, DEF_STR( On ) );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_SERVICE1 );

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_UP );    PORT_PLAYER(2); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT );  PORT_PLAYER(2); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_PLAYER(2); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN );  PORT_PLAYER(2); PORT_4WAY(); PORT_COCKTAIL();
            PORT_SERVICE( 0x10, IP_ACTIVE_LOW );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_START2 );
            PORT_DIPNAME(0x80, 0x80, DEF_STR( Cabinet ) );
            PORT_DIPSETTING(   0x80, DEF_STR( Upright ) );
            PORT_DIPSETTING(   0x00, DEF_STR( Cocktail ) );

            PORT_START("DSW1");
            PORT_DIPNAME( 0x03, 0x01, DEF_STR( Coinage ) ); PORT_DIPLOCATION("SW:1,2");
            PORT_DIPSETTING(    0x03, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x01, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Free_Play ) );
            PORT_DIPNAME( 0x0c, 0x08, DEF_STR( Lives ) ); PORT_DIPLOCATION("SW:3,4");
            PORT_DIPSETTING(    0x00, "1" );
            PORT_DIPSETTING(    0x04, "2" );
            PORT_DIPSETTING(    0x08, "3" );
            PORT_DIPSETTING(    0x0c, "5" );
            PORT_DIPNAME( 0x30, 0x00, DEF_STR( Bonus_Life ) ); PORT_DIPLOCATION("SW:5,6");
            PORT_DIPSETTING(    0x00, "10000" );
            PORT_DIPSETTING(    0x10, "15000" );
            PORT_DIPSETTING(    0x20, "20000" );
            PORT_DIPSETTING(    0x30, DEF_STR( None ) );
            PORT_DIPNAME( 0x40, 0x40, DEF_STR( Difficulty ) ); PORT_DIPLOCATION("SW:7"); // physical location for difficulty on puckman set is split-pad between R32 and C29
            PORT_DIPSETTING(    0x40, DEF_STR( Normal ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Hard ) );
            PORT_DIPNAME( 0x80, 0x80, "Ghost Names" ); PORT_DIPLOCATION("SW:8"); // physical location for ghostnames on puckman set is split-pad between C10 and C29
            PORT_DIPSETTING(    0x80, DEF_STR( Normal ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Alternate ) );

            PORT_START("DSW2");
            PORT_BIT( 0xff, IP_ACTIVE_HIGH, IPT_UNUSED );

            INPUT_PORTS_END();
        }


        /* Ms. Pac-Man input ports are identical to Pac-Man, the only difference is */
        /* the missing Ghost Names dip switch. */
        //static INPUT_PORTS_START( mspacman )
        void construct_ioport_mspacman(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_4WAY();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_4WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_4WAY();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_4WAY();
            PORT_DIPNAME(0x10, 0x10, "Rack Test (Cheat)" ); PORT_CODE(KEYCODE_F1);
            PORT_DIPSETTING(    0x10, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_SERVICE1 );

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_SERVICE( 0x10, IP_ACTIVE_LOW );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_START2 );
            PORT_DIPNAME( 0x80, 0x80, DEF_STR( Cabinet ) );
            PORT_DIPSETTING(    0x80, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Cocktail ) );

            PORT_START("DSW1");
            PORT_DIPNAME( 0x03, 0x01, DEF_STR( Coinage ) );
            PORT_DIPSETTING(    0x03, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x01, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Free_Play ) );
            PORT_DIPNAME( 0x0c, 0x08, DEF_STR( Lives ) );
            PORT_DIPSETTING(    0x00, "1" );
            PORT_DIPSETTING(    0x04, "2" );
            PORT_DIPSETTING(    0x08, "3" );
            PORT_DIPSETTING(    0x0c, "5" );
            PORT_DIPNAME( 0x30, 0x00, DEF_STR( Bonus_Life ) );
            PORT_DIPSETTING(    0x00, "10000" );
            PORT_DIPSETTING(    0x10, "15000" );
            PORT_DIPSETTING(    0x20, "20000" );
            PORT_DIPSETTING(    0x30, DEF_STR( None ) );
            PORT_DIPNAME( 0x40, 0x40, DEF_STR( Difficulty ) );
            PORT_DIPSETTING(    0x40, DEF_STR( Normal ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Hard ) );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("DSW2");
            PORT_BIT( 0xff, IP_ACTIVE_HIGH, IPT_UNUSED );

            INPUT_PORTS_END();
        }
    }


    partial class pacman_state : driver_device
    {
        /*************************************
         *
         *  Graphics layouts
         *
         *************************************/

        static readonly gfx_layout tilelayout = new gfx_layout(
            8, 8,    /* 8*8 characters */
            RGN_FRAC(1,2),    /* 256 characters */
            2,  /* 2 bits per pixel */
            new UInt32[] { 0, 4 },   /* the two bitplanes for 4 pixels are packed into one byte */
            new UInt32[] { 8*8+0, 8*8+1, 8*8+2, 8*8+3, 0, 1, 2, 3 }, /* bits are packed in groups of four */
            new UInt32[] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
            16*8    /* every char takes 16 bytes */
        );


        static readonly gfx_layout spritelayout = new gfx_layout(
            16,16,  /* 16*16 sprites */
            RGN_FRAC(1,2),  /* 64 sprites */
            2,  /* 2 bits per pixel */
            new UInt32[] { 0, 4 },   /* the two bitplanes for 4 pixels are packed into one byte */
            new UInt32[] { 8*8, 8*8+1, 8*8+2, 8*8+3, 16*8+0, 16*8+1, 16*8+2, 16*8+3,
                    24*8+0, 24*8+1, 24*8+2, 24*8+3, 0, 1, 2, 3 },
            new UInt32[] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
                    32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64*8    /* every sprite takes 64 bytes */
        );


        //static GFXDECODE_START( gfx_pacman )
        static readonly gfx_decode_entry [] gfx_pacman = new gfx_decode_entry[]
        {
            GFXDECODE_ENTRY( "gfx1", 0x0000, tilelayout,   0, 128 ),
            GFXDECODE_ENTRY( "gfx1", 0x1000, spritelayout, 0, 128 ),

            //GFXDECODE_END
        };


        /*************************************
         *
         *  Machine drivers
         *
         *************************************/

        public void pacman(machine_config config, bool latch = true)
        {
            m_globals.helper_config = config;

            // Basic machine hardware
            Z80(config, m_maincpu, MASTER_CLOCK/6);
            m_maincpu.op[0].memory().set_addrmap(AS_PROGRAM, pacman_map);
            m_maincpu.op[0].memory().set_addrmap(AS_IO, writeport);
            m_maincpu.op[0].execute().set_irq_acknowledge_callback(interrupt_vector_r);

            if (latch)
            {
                LS259(config, m_mainlatch); // 74LS259 at 8K or 4099 at 7K
                m_mainlatch.op[0].q_out_cb(0).set((write_line_delegate)irq_mask_w).reg();
                m_mainlatch.op[0].q_out_cb(1).set("namco", (write_line_delegate)((state) => { ((namco_device)subdevice("namco")).sound_enable_w(state); })).reg();  //FUNC(namco_device::sound_enable_w));
                m_mainlatch.op[0].q_out_cb(3).set((write_line_delegate)flipscreen_w).reg();
                m_mainlatch.op[0].q_out_cb(7).set((write_line_delegate)coin_counter_w).reg();

                // NOTE(dwidel): The Pacman code uses $5004 and $5005 for LEDs and $5007 for coin lockout.  This hardware does not
                // exist on any Pacman or Puckman board I have seen.
                //m_mainlatch->q_out_cb<4>().set_output("led0");
                //m_mainlatch->q_out_cb<5>().set_output("led1");
                //m_mainlatch->q_out_cb<6>().set(FUNC(pacman_state::coin_lockout_global_w));
            }

            WATCHDOG_TIMER(config, m_watchdog);
            m_watchdog.op[0].set_vblank_count("screen", 16);

            // Video hardware
            GFXDECODE(config, m_gfxdecode, m_palette, gfx_pacman);

            PALETTE(config, m_palette, pacman_palette, 128*4, 32);

            screen_device screen = SCREEN(config, "screen", SCREEN_TYPE_RASTER);
            screen.set_raw(PIXEL_CLOCK, (u16)HTOTAL, (u16)HBEND, (u16)HBSTART, (u16)VTOTAL, (u16)VBEND, (u16)VBSTART);
            screen.set_screen_update(screen_update_pacman);
            screen.set_palette("palette");
            screen.screen_vblank().set((write_line_delegate)vblank_irq).reg();

            MCFG_VIDEO_START_OVERRIDE(config, video_start_pacman);

            // Sound hardware
            SPEAKER(config, "mono").front_center();

            NAMCO(config, m_namco_sound, MASTER_CLOCK/6/32);
            m_namco_sound.op[0].set_voices(3);
            m_namco_sound.op[0].disound.add_route(ALL_OUTPUTS, "mono", 1.0);
        }


        public void mspacman(machine_config config)
        {
            pacman(config);

            // Basic machine hardware
            m_maincpu.op[0].memory().set_addrmap(AS_PROGRAM, mspacman_map);

            m_mainlatch.op[0].q_out_cb(6).set((write_line_delegate)coin_lockout_global_w).reg();
        }
    }


    partial class pacman : global_object
    {
        /*************************************
         *
         *  ROM definitions
         *
         *************************************/

        //ROM_START( puckman )
        static readonly MemoryContainer<tiny_rom_entry> rom_puckman = new MemoryContainer<tiny_rom_entry>()
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "pm1_prg1.6e",  0x0000, 0x0800, CRC("f36e88ab") + SHA1("813cecf44bf5464b1aed64b36f5047e4c79ba176") ),
            ROM_LOAD( "pm1_prg2.6k",  0x0800, 0x0800, CRC("618bd9b3") + SHA1("b9ca52b63a49ddece768378d331deebbe34fe177") ),
            ROM_LOAD( "pm1_prg3.6f",  0x1000, 0x0800, CRC("7d177853") + SHA1("9b5ddaaa8b564654f97af193dbcc29f81f230a25") ),
            ROM_LOAD( "pm1_prg4.6m",  0x1800, 0x0800, CRC("d3e8914c") + SHA1("c2f00e1773c6864435f29c8b7f44f2ef85d227d3") ),
            ROM_LOAD( "pm1_prg5.6h",  0x2000, 0x0800, CRC("6bf4f625") + SHA1("afe72fdfec66c145b53ed865f98734686b26e921") ),
            ROM_LOAD( "pm1_prg6.6n",  0x2800, 0x0800, CRC("a948ce83") + SHA1("08759833f7e0690b2ccae573c929e2a48e5bde7f") ),
            ROM_LOAD( "pm1_prg7.6j",  0x3000, 0x0800, CRC("b6289b26") + SHA1("d249fa9cdde774d5fee7258147cd25fa3f4dc2b3") ),
            ROM_LOAD( "pm1_prg8.6p",  0x3800, 0x0800, CRC("17a88c13") + SHA1("eb462de79f49b7aa8adb0cc6d31535b10550c0ce") ),

            ROM_REGION( 0x2000, "gfx1", 0 ),
            ROM_LOAD( "pm1_chg1.5e",  0x0000, 0x0800, CRC("2066a0b7") + SHA1("6d4ccc27d6be185589e08aa9f18702b679e49a4a") ),
            ROM_LOAD( "pm1_chg2.5h",  0x0800, 0x0800, CRC("3591b89d") + SHA1("79bb456be6c39c1ccd7d077fbe181523131fb300") ),
            ROM_LOAD( "pm1_chg3.5f",  0x1000, 0x0800, CRC("9e39323a") + SHA1("be933e691df4dbe7d12123913c3b7b7b585b7a35") ),
            ROM_LOAD( "pm1_chg4.5j",  0x1800, 0x0800, CRC("1b1d9096") + SHA1("53771c573051db43e7185b1d188533056290a620") ),

            ROM_REGION( 0x0120, "proms", 0 ),
            ROM_LOAD( "pm1-1.7f",     0x0000, 0x0020, CRC("2fc650bd") + SHA1("8d0268dee78e47c712202b0ec4f1f51109b1f2a5") ), // 82s123
            ROM_LOAD( "pm1-4.4a",     0x0020, 0x0100, CRC("3eb3a8e4") + SHA1("19097b5f60d1030f8b82d9f1d3a241f93e5c75d6") ), // 82s126

            ROM_REGION( 0x0200, "namco", 0 ),    // Sound PROMs
            ROM_LOAD( "pm1-3.1m",     0x0000, 0x0100, CRC("a9cc86bf") + SHA1("bbcec0570aeceb582ff8238a4bc8546a23430081") ), // 82s126
            ROM_LOAD( "pm1-2.3m",     0x0100, 0x0100, CRC("77245b66") + SHA1("0c4d0bee858b97632411c440bea6948a74759746") ), // 82s126 - timing - not used

            ROM_END,
        };


        // ROM_START( pacman )
        static readonly MemoryContainer<tiny_rom_entry> rom_pacman = new MemoryContainer<tiny_rom_entry>()
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "pacman.6e",    0x0000, 0x1000, CRC("c1e6ab10") + SHA1("e87e059c5be45753f7e9f33dff851f16d6751181") ),
            ROM_LOAD( "pacman.6f",    0x1000, 0x1000, CRC("1a6fb2d4") + SHA1("674d3a7f00d8be5e38b1fdc208ebef5a92d38329") ),
            ROM_LOAD( "pacman.6h",    0x2000, 0x1000, CRC("bcdd1beb") + SHA1("8e47e8c2c4d6117d174cdac150392042d3e0a881") ),
            ROM_LOAD( "pacman.6j",    0x3000, 0x1000, CRC("817d94e3") + SHA1("d4a70d56bb01d27d094d73db8667ffb00ca69cb9") ),

            ROM_REGION( 0x2000, "gfx1", 0 ),
            ROM_LOAD( "pacman.5e",    0x0000, 0x1000, CRC("0c944964") + SHA1("06ef227747a440831c9a3a613b76693d52a2f0a9") ),
            ROM_LOAD( "pacman.5f",    0x1000, 0x1000, CRC("958fedf9") + SHA1("4a937ac02216ea8c96477d4a15522070507fb599") ),

            ROM_REGION( 0x0120, "proms", 0 ),
            ROM_LOAD( "82s123.7f",    0x0000, 0x0020, CRC("2fc650bd") + SHA1("8d0268dee78e47c712202b0ec4f1f51109b1f2a5") ),
            ROM_LOAD( "82s126.4a",    0x0020, 0x0100, CRC("3eb3a8e4") + SHA1("19097b5f60d1030f8b82d9f1d3a241f93e5c75d6") ),

            ROM_REGION( 0x0200, "namco", 0 ),    // Sound PROMs
            ROM_LOAD( "82s126.1m",    0x0000, 0x0100, CRC("a9cc86bf") + SHA1("bbcec0570aeceb582ff8238a4bc8546a23430081") ),
            ROM_LOAD( "82s126.3m",    0x0100, 0x0100, CRC("77245b66") + SHA1("0c4d0bee858b97632411c440bea6948a74759746") ),    // Timing - not used

            ROM_END,
        };


        //ROM_START( mspacman )
        static readonly MemoryContainer<tiny_rom_entry> rom_mspacman = new MemoryContainer<tiny_rom_entry>()
        {
            ROM_REGION( 0x20000, "maincpu", 0 ), // 64k for code+64k for decrypted code
            ROM_LOAD( "pacman.6e",    0x0000, 0x1000, CRC("c1e6ab10") + SHA1("e87e059c5be45753f7e9f33dff851f16d6751181") ),
            ROM_LOAD( "pacman.6f",    0x1000, 0x1000, CRC("1a6fb2d4") + SHA1("674d3a7f00d8be5e38b1fdc208ebef5a92d38329") ),
            ROM_LOAD( "pacman.6h",    0x2000, 0x1000, CRC("bcdd1beb") + SHA1("8e47e8c2c4d6117d174cdac150392042d3e0a881") ),
            ROM_LOAD( "pacman.6j",    0x3000, 0x1000, CRC("817d94e3") + SHA1("d4a70d56bb01d27d094d73db8667ffb00ca69cb9") ),
            ROM_LOAD( "u5",           0x8000, 0x0800, CRC("f45fbbcd") + SHA1("b26cc1c8ee18e9b1daa97956d2159b954703a0ec") ),
            ROM_LOAD( "u6",           0x9000, 0x1000, CRC("a90e7000") + SHA1("e4df96f1db753533f7d770aa62ae1973349ea4cf") ),
            ROM_LOAD( "u7",           0xb000, 0x1000, CRC("c82cd714") + SHA1("1d8ac7ad03db2dc4c8c18ade466e12032673f874") ),

            ROM_REGION( 0x2000, "gfx1", 0 ),
            ROM_LOAD( "5e",           0x0000, 0x1000, CRC("5c281d01") + SHA1("5e8b472b615f12efca3fe792410c23619f067845") ),
            ROM_LOAD( "5f",           0x1000, 0x1000, CRC("615af909") + SHA1("fd6a1dde780b39aea76bf1c4befa5882573c2ef4") ),

            ROM_REGION( 0x0120, "proms", 0 ),
            ROM_LOAD( "82s123.7f",    0x0000, 0x0020, CRC("2fc650bd") + SHA1("8d0268dee78e47c712202b0ec4f1f51109b1f2a5") ),
            ROM_LOAD( "82s126.4a",    0x0020, 0x0100, CRC("3eb3a8e4") + SHA1("19097b5f60d1030f8b82d9f1d3a241f93e5c75d6") ),

            ROM_REGION( 0x0200, "namco", 0 ),    // Sound PROMs
            ROM_LOAD( "82s126.1m",    0x0000, 0x0100, CRC("a9cc86bf") + SHA1("bbcec0570aeceb582ff8238a4bc8546a23430081") ),
            ROM_LOAD( "82s126.3m",    0x0100, 0x0100, CRC("77245b66") + SHA1("0c4d0bee858b97632411c440bea6948a74759746") ),    // Timing - not used

            ROM_END,
        };


        //ROM_START( pacplus )
        static readonly MemoryContainer<tiny_rom_entry> rom_pacplus = new MemoryContainer<tiny_rom_entry>()
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "pacplus.6e",   0x0000, 0x1000, CRC("d611ef68") + SHA1("8531c54ca6b0de0ea4ccc34e0e801ba9847e75bc") ),
            ROM_LOAD( "pacplus.6f",   0x1000, 0x1000, CRC("c7207556") + SHA1("8ba97215bdb75f0e70eb8d3223847efe4dc4fb48") ),
            ROM_LOAD( "pacplus.6h",   0x2000, 0x1000, CRC("ae379430") + SHA1("4e8613d51a80cf106f883db79685e1e22541da45") ),
            ROM_LOAD( "pacplus.6j",   0x3000, 0x1000, CRC("5a6dff7b") + SHA1("b956ae5d66683aab74b90469ad36b5bb361d677e") ),

            ROM_REGION( 0x2000, "gfx1", 0 ),
            ROM_LOAD( "pacplus.5e",   0x0000, 0x1000, CRC("022c35da") + SHA1("57d7d723c7b029e3415801f4ce83469ec97bb8a1") ),
            ROM_LOAD( "pacplus.5f",   0x1000, 0x1000, CRC("4de65cdd") + SHA1("9c0699204484be819b77f0b212c792fe9e9fae5d") ),

            ROM_REGION( 0x0120, "proms", 0 ),
            ROM_LOAD( "pacplus.7f",   0x0000, 0x0020, CRC("063dd53a") + SHA1("2e43b46ec3b101d1babab87cdaddfa944116ec06") ),
            ROM_LOAD( "pacplus.4a",   0x0020, 0x0100, CRC("e271a166") + SHA1("cf006536215a7a1d488eebc1d8a2e2a8134ce1a6") ),

            ROM_REGION( 0x0200, "namco", 0 ),    // Sound PROMs
            ROM_LOAD( "82s126.1m",    0x0000, 0x0100, CRC("a9cc86bf") + SHA1("bbcec0570aeceb582ff8238a4bc8546a23430081") ),
            ROM_LOAD( "82s126.3m",    0x0100, 0x0100, CRC("77245b66") + SHA1("0c4d0bee858b97632411c440bea6948a74759746") ),    // Timing - not used

            ROM_END,
        };
    }


    partial class pacman_state : driver_device
    {
        /*************************************
         *
         *  Driver initialization
         *
         *************************************/


        static int BITSWAP12(int val, int B11, int B10, int B9, int B8, int B7, int B6, int B5, int B4, int B3, int B2, int B1, int B0) { return bitswap(val,15,14,13,12,B11,B10,B9,B8,B7,B6,B5,B4,B3,B2,B1,B0); }
        static int BITSWAP11(int val, int B10, int B9, int B8, int B7, int B6, int B5, int B4, int B3, int B2, int B1, int B0) { return bitswap(val,15,14,13,12,11,B10,B9,B8,B7,B6,B5,B4,B3,B2,B1,B0); }


        void mspacman_install_patches(Pointer<uint8_t> ROM)  //void mspacman_install_patches(uint8_t *ROM)
        {
            int i;

            /* copy forty 8-byte patches into Pac-Man code */
            for (i = 0; i < 8; i++)
            {
                ROM[0x0410+i] = ROM[0x8008+i];
                ROM[0x08E0+i] = ROM[0x81D8+i];
                ROM[0x0A30+i] = ROM[0x8118+i];
                ROM[0x0BD0+i] = ROM[0x80D8+i];
                ROM[0x0C20+i] = ROM[0x8120+i];
                ROM[0x0E58+i] = ROM[0x8168+i];
                ROM[0x0EA8+i] = ROM[0x8198+i];

                ROM[0x1000+i] = ROM[0x8020+i];
                ROM[0x1008+i] = ROM[0x8010+i];
                ROM[0x1288+i] = ROM[0x8098+i];
                ROM[0x1348+i] = ROM[0x8048+i];
                ROM[0x1688+i] = ROM[0x8088+i];
                ROM[0x16B0+i] = ROM[0x8188+i];
                ROM[0x16D8+i] = ROM[0x80C8+i];
                ROM[0x16F8+i] = ROM[0x81C8+i];
                ROM[0x19A8+i] = ROM[0x80A8+i];
                ROM[0x19B8+i] = ROM[0x81A8+i];

                ROM[0x2060+i] = ROM[0x8148+i];
                ROM[0x2108+i] = ROM[0x8018+i];
                ROM[0x21A0+i] = ROM[0x81A0+i];
                ROM[0x2298+i] = ROM[0x80A0+i];
                ROM[0x23E0+i] = ROM[0x80E8+i];
                ROM[0x2418+i] = ROM[0x8000+i];
                ROM[0x2448+i] = ROM[0x8058+i];
                ROM[0x2470+i] = ROM[0x8140+i];
                ROM[0x2488+i] = ROM[0x8080+i];
                ROM[0x24B0+i] = ROM[0x8180+i];
                ROM[0x24D8+i] = ROM[0x80C0+i];
                ROM[0x24F8+i] = ROM[0x81C0+i];
                ROM[0x2748+i] = ROM[0x8050+i];
                ROM[0x2780+i] = ROM[0x8090+i];
                ROM[0x27B8+i] = ROM[0x8190+i];
                ROM[0x2800+i] = ROM[0x8028+i];
                ROM[0x2B20+i] = ROM[0x8100+i];
                ROM[0x2B30+i] = ROM[0x8110+i];
                ROM[0x2BF0+i] = ROM[0x81D0+i];
                ROM[0x2CC0+i] = ROM[0x80D0+i];
                ROM[0x2CD8+i] = ROM[0x80E0+i];
                ROM[0x2CF0+i] = ROM[0x81E0+i];
                ROM[0x2D60+i] = ROM[0x8160+i];
            }
        }


        public void init_mspacman()
        {
            /* CPU ROMs */

            /* Pac-Man code is in low bank */
            PointerU8 ROM = new PointerU8(memregion("maincpu").base_());  //uint8_t *ROM = memregion("maincpu")->base();

            /* decrypted Ms. Pac-Man code is in high bank */
            PointerU8 DROM = new PointerU8(memregion("maincpu").base_(), 0x10000);  // uint8_t *DROM = &memregion("maincpu")->base()[0x10000];

            /* copy ROMs into decrypted bank */
            for (int i = 0; i < 0x1000; i++)
            {
                DROM[0x0000+i] = ROM[0x0000+i]; /* pacman.6e */
                DROM[0x1000+i] = ROM[0x1000+i]; /* pacman.6f */
                DROM[0x2000+i] = ROM[0x2000+i]; /* pacman.6h */
                DROM[0x3000+i] = (byte)bitswap(ROM[0xb000+BITSWAP12(i,11,3,7,9,10,8,6,5,4,2,1,0)],0,4,5,7,6,3,2,1);  /* decrypt u7 */
            }

            for (int i = 0; i < 0x800; i++)
            {
                DROM[0x8000+i] = (byte)bitswap(ROM[0x8000+BITSWAP11(i,   8,7,5,9,10,6,3,4,2,1,0)],0,4,5,7,6,3,2,1);  /* decrypt u5 */
                DROM[0x8800+i] = (byte)bitswap(ROM[0x9800+BITSWAP12(i,11,3,7,9,10,8,6,5,4,2,1,0)],0,4,5,7,6,3,2,1);  /* decrypt half of u6 */
                DROM[0x9000+i] = (byte)bitswap(ROM[0x9000+BITSWAP12(i,11,3,7,9,10,8,6,5,4,2,1,0)],0,4,5,7,6,3,2,1);  /* decrypt half of u6 */
                DROM[0x9800+i] = ROM[0x1800+i];     /* mirror of pacman.6f high */
            }

            for (int i = 0; i < 0x1000; i++)
            {
                DROM[0xa000+i] = ROM[0x2000+i];     /* mirror of pacman.6h */
                DROM[0xb000+i] = ROM[0x3000+i];     /* mirror of pacman.6j */
            }

            /* install patches into decrypted bank */
            mspacman_install_patches(new Pointer<uint8_t>(DROM));

            /* mirror Pac-Man ROMs into upper addresses of normal bank */
            for (int i = 0; i < 0x1000; i++)
            {
                ROM[0x8000+i] = ROM[0x0000+i];
                ROM[0x9000+i] = ROM[0x1000+i];
                ROM[0xa000+i] = ROM[0x2000+i];
                ROM[0xb000+i] = ROM[0x3000+i];
            }

            /* initialize the banks */
            membank("bank1").configure_entries(0, 2, new PointerU8(ROM), 0x10000);
            membank("bank1").set_entry(1);
        }


        public void init_pacplus()
        {
            pacplus_decode();
        }
    }


    partial class pacman : global_object
    {
        static void pacman_state_pacman(machine_config config, device_t device) { pacman_state pacman_state = (pacman_state)device; pacman_state.pacman(config); }
        static void pacman_state_mspacman(machine_config config, device_t device) { pacman_state pacman_state = (pacman_state)device; pacman_state.mspacman(config); }
        static void pacman_state_init_mspacman(device_t owner) { pacman_state pacman_state = (pacman_state)owner; pacman_state.init_mspacman(); }
        static void pacman_state_init_pacplus(device_t owner) { pacman_state pacman_state = (pacman_state)owner; pacman_state.init_pacplus(); }


        static pacman m_pacman = new pacman();


        static device_t device_creator_puckman(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new pacman_state(mconfig, (device_type)type, tag); }
        static device_t device_creator_pacman(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new pacman_state(mconfig, (device_type)type, tag); }
        static device_t device_creator_mspacman(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new pacman_state(mconfig, (device_type)type, tag); }
        static device_t device_creator_pacplus(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new pacman_state(mconfig, (device_type)type, tag); }


        //                                                                                                        rom         parent     machine                       inp                                 init
        public static readonly game_driver driver_puckman  = GAME( device_creator_puckman,  rom_puckman,  "1980", "puckman",  "0",       pacman.pacman_state_pacman,   m_pacman.construct_ioport_pacman,   driver_device.empty_init,          ROT90,  "Namco", "Puck Man (Japan set 1)", MACHINE_SUPPORTS_SAVE );
        public static readonly game_driver driver_pacman   = GAME( device_creator_pacman,   rom_pacman,   "1980", "pacman",   "puckman", pacman.pacman_state_pacman,   m_pacman.construct_ioport_pacman,   driver_device.empty_init,          ROT90,  "Namco (Midway license)", "Pac-Man (Midway)", MACHINE_SUPPORTS_SAVE );
        public static readonly game_driver driver_mspacman = GAME( device_creator_mspacman, rom_mspacman, "1981", "mspacman", "0",       pacman.pacman_state_mspacman, m_pacman.construct_ioport_mspacman, pacman.pacman_state_init_mspacman, ROT90,  "Midway / General Computer Corporation", "Ms. Pac-Man", MACHINE_SUPPORTS_SAVE );
        public static readonly game_driver driver_pacplus  = GAME( device_creator_pacplus,  rom_pacplus,  "1982", "pacplus",  "0",       pacman.pacman_state_pacman,   m_pacman.construct_ioport_pacman,   pacman.pacman_state_init_pacplus,  ROT90,  "Namco (Midway license)", "Pac-Man Plus", MACHINE_SUPPORTS_SAVE );
    }
}
