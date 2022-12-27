// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;

using static mame.diexec_global;
using static mame.digfx_global;
using static mame.drawgfx_global;
using static mame.driver_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.generic_global;
using static mame.hash_global;
using static mame.i8257_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.z80_global;
using static mame.watchdog_global;
using static mame.z80dma_global;


namespace mame
{
    partial class dkong_state : driver_device
    {
        /*************************************
         *
         *  statics
         *
         *************************************/

        uint8_t memory_read_byte(offs_t offset)
        {
            address_space prog_space = m_maincpu.op0.memory().space(AS_PROGRAM);
            return prog_space.read_byte(offset);
        }


        void memory_write_byte(offs_t offset, uint8_t data)
        {
            address_space prog_space = m_maincpu.op0.memory().space(AS_PROGRAM);
            prog_space.write_byte(offset, data);
        }


        /*************************************
         *
         *  Machine setup
         *
         *************************************/

        //MACHINE_START_MEMBER(dkong_state,dkong2b)
        void machine_start_dkong2b()
        {
            m_hardware_type = HARDWARE_TKG04;

            save_item(NAME(new { m_decrypt_counter }));
            save_item(NAME(new { m_dma_latch }));
        }


        //MACHINE_START_MEMBER(dkong_state,dkong3)
        void machine_start_dkong3()
        {
            m_hardware_type = HARDWARE_TKG04;
        }


        //MACHINE_RESET_MEMBER(dkong_state,dkong)
        void machine_reset_dkong()
        {
            /* nothing */
        }


        /*************************************
         *
         *  DMA handling
         *
         *************************************/

        //uint8_t dkong_state::hb_dma_read_byte(offs_t offset)
        //void dkong_state::hb_dma_write_byte(offs_t offset, uint8_t data)


        uint8_t p8257_ctl_r()
        {
            return m_dma_latch;
        }


        void p8257_ctl_w(uint8_t data)
        {
            m_dma_latch = data;
        }


        /*************************************
         *
         *  Output ports
         *
         *************************************/

        void dkong3_coin_counter_w(offs_t offset, uint8_t data)
        {
            throw new emu_unimplemented();
        }


        void p8257_drq_w(uint8_t data)
        {
            m_dma8257.op0.dreq0_w(data & 0x01);
            m_dma8257.op0.dreq1_w(data & 0x01);
            machine().scheduler().abort_timeslice(); // transfer occurs immediately
            machine().scheduler().boost_interleave(attotime.zero, attotime.from_usec(100)); // smooth things out a bit
        }


        uint8_t dkong_in2_r(offs_t offset)
        {
            // 2 board DK and all DKjr has a watchdog
            if (m_watchdog.op0 != null)
                m_watchdog.op0.watchdog_reset();

            uint8_t r = (uint8_t)ioport("IN2").read();
            machine().bookkeeping().coin_counter_w((int)offset, r >> 7);
            if ((ioport("SERVICE1").read() & 1) != 0)
                r |= 0x80; /* service ==> coin */

            return r;
        }


        //uint8_t dkong_state::epos_decrypt_rom(offs_t offset)
        //void dkong_state::s2650_data_w(uint8_t data)
        //uint8_t dkong_state::s2650_port0_r()
        //uint8_t dkong_state::s2650_port1_r()


        void dkong3_2a03_reset_w(uint8_t data)
        {
            throw new emu_unimplemented();
        }


        //uint8_t dkong_state::strtheat_inputport_0_r()
        //uint8_t dkong_state::strtheat_inputport_1_r()


        void dkong_z80dma_rdy_w(uint8_t data)
        {
            throw new emu_unimplemented();
        }


        void nmi_mask_w(uint8_t data)
        {
            m_nmi_mask = (uint8_t)(data & 1);
            if (m_nmi_mask == 0)
                m_maincpu.op0.set_input_line(INPUT_LINE_NMI, CLEAR_LINE);
        }


        /*************************************
         *
         *  Main CPU memory handlers
         *
         *************************************/

        void dkong_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x4fff).rom();
            map.op(0x6000, 0x6bff).ram();
            map.op(0x7000, 0x73ff).ram().share("sprite_ram"); /* sprite set 1 */
            map.op(0x7400, 0x77ff).ram().w(dkong_videoram_w).share("video_ram");
            map.op(0x7800, 0x780f).rw(m_dma8257, (offset) => { return m_dma8257.op0.read(offset); }, (offset, data) => { m_dma8257.op0.write(offset, data); });  //FUNC(i8257_device::read), FUNC(i8257_device::write));   /* P8257 control registers */
            map.op(0x7c00, 0x7c00).portr("IN0").w("ls175.3d", (offset, data) => { ((latch8_device)subdevice("ls175.3d")).write(offset, data); });  //FUNC(latch8_device::write));    /* IN0, sound CPU intf */
            map.op(0x7c80, 0x7c80).portr("IN1").w(radarscp_grid_color_w);/* IN1 */

            map.op(0x7d00, 0x7d00).r(dkong_in2_r);                               /* IN2 */
            map.op(0x7d00, 0x7d07).w(m_dev_6h, (offset, data) => { m_dev_6h.op0.bit0_w(offset, data); });  //FUNC(latch8_device::bit0_w));          /* Sound signals */

            map.op(0x7d80, 0x7d80).portr("DSW0").w(dkong_audio_irq_w);   /* DSW0 */
            map.op(0x7d81, 0x7d81).w(radarscp_grid_enable_w);
            map.op(0x7d82, 0x7d82).w(dkong_flipscreen_w);
            map.op(0x7d83, 0x7d83).w(dkong_spritebank_w);                       /* 2 PSL Signal */
            map.op(0x7d84, 0x7d84).w(nmi_mask_w);
            map.op(0x7d85, 0x7d85).w(p8257_drq_w);          /* P8257 ==> /DRQ0 /DRQ1 */
            map.op(0x7d86, 0x7d87).w(dkong_palettebank_w);
        }


        void dkongjr_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x5fff).rom();
            map.op(0x6000, 0x6bff).ram();
            map.op(0x6c00, 0x6fff).ram();                                              /* DK3 bootleg only */
            map.op(0x7000, 0x73ff).ram().share("sprite_ram"); /* sprite set 1 */
            map.op(0x7400, 0x77ff).ram().w(dkong_videoram_w).share("video_ram");
            map.op(0x7800, 0x780f).rw(m_dma8257, (offset) => { return m_dma8257.op0.read(offset); }, (offset, data) => { m_dma8257.op0.write(offset, data); });  /* P8257 control registers */

            map.op(0x7c00, 0x7c00).portr("IN0").w("ls174.3d", (offset, data) => { ((latch8_device)subdevice("ls174.3d")).write(offset, data); });    /* IN0, sound interface */

            map.op(0x7c80, 0x7c87).w("ls259.4h", (offset, data) => { ((latch8_device)subdevice("ls259.4h")).bit0_w(offset, data); });     /* latch for sound and signals above */
            map.op(0x7c80, 0x7c80).portr("IN1").w(dkongjr_gfxbank_w);

            map.op(0x7d00, 0x7d00).r(dkong_in2_r);                                /* IN2 */
            map.op(0x7d00, 0x7d07).w(m_dev_6h, (offset, data) => { m_dev_6h.op0.bit0_w(offset, data); });      /* Sound addrs */

            map.op(0x7d80, 0x7d87).w("ls259.5h", (offset, data) => { ((latch8_device)subdevice("ls259.5h")).bit0_w(offset, data); });     /* latch for sound and signals above*/
            map.op(0x7d80, 0x7d80).portr("DSW0").w(dkong_audio_irq_w);   /* DSW0 */
            map.op(0x7d82, 0x7d82).w(dkong_flipscreen_w);
            map.op(0x7d83, 0x7d83).w(dkong_spritebank_w);                       /* 2 PSL Signal */
            map.op(0x7d84, 0x7d84).w(nmi_mask_w);
            map.op(0x7d85, 0x7d85).w(p8257_drq_w);        /* P8257 ==> /DRQ0 /DRQ1 */
            map.op(0x7d86, 0x7d87).w(dkong_palettebank_w);

            map.op(0x8000, 0x9fff).rom();                                             /* bootleg DKjr only */
            map.op(0xb000, 0xbfff).rom();                                             /* pestplce only */
            map.op(0xd000, 0xdfff).rom();                                             /* DK3 bootleg only */
        }


        void dkong3_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x5fff).rom();
            map.op(0x6000, 0x67ff).ram();
            map.op(0x6800, 0x6fff).ram();
            map.op(0x7000, 0x73ff).ram().share("sprite_ram"); /* sprite set 1 */
            map.op(0x7400, 0x77ff).ram().w(dkong_videoram_w).share("video_ram");
            map.op(0x7c00, 0x7c00).portr("IN0").w("latch1", (offset, data) => { ((latch8_device)subdevice("latch1")).write(offset, data); });
            map.op(0x7c80, 0x7c80).portr("IN1").w("latch2", (offset, data) => { ((latch8_device)subdevice("latch2")).write(offset, data); });
            map.op(0x7d00, 0x7d00).portr("DSW0").w("latch3", (offset, data) => { ((latch8_device)subdevice("latch3")).write(offset, data); });
            map.op(0x7d80, 0x7d80).portr("DSW1").w(dkong3_2a03_reset_w);
            map.op(0x7e80, 0x7e80).w(dkong3_coin_counter_w);
            map.op(0x7e81, 0x7e81).w(dkong3_gfxbank_w);
            map.op(0x7e82, 0x7e82).w(dkong_flipscreen_w);
            map.op(0x7e83, 0x7e83).w(dkong_spritebank_w);                 /* 2 PSL Signal */
            map.op(0x7e84, 0x7e84).w(nmi_mask_w);
            map.op(0x7e85, 0x7e85).w(dkong_z80dma_rdy_w);  /* ==> DMA Chip */
            map.op(0x7e86, 0x7e87).w(dkong_palettebank_w);
            map.op(0x8000, 0x9fff).rom();                                       /* DK3 and bootleg DKjr only */
        }


        void dkong3_io_map(address_map map, device_t device)
        {
            map.global_mask(0xff);
            map.op(0x00, 0x00).rw(m_z80dma, () => { return m_z80dma.op0.read(); }, (uint8_t data) => { m_z80dma.op0.write(data); });  /* dma controller */
        }
    }


    public partial class dkong : construct_ioport_helper
    {
        /*************************************
         *
         *  Port definitions
         *
         *************************************/

        //static INPUT_PORTS_START( dkong_in0_4 )
        void construct_ioport_dkong_in0_4(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");      /* IN0 */
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ); PORT_4WAY();
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ); PORT_4WAY();
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ); PORT_4WAY();
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ); PORT_4WAY();
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_BUTTON1 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_UNKNOWN ); /* not connected - held to high */
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_UNKNOWN ); /* not connected - held to high */
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_UNKNOWN ); /* not connected - held to high */

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( dkong_in1_4 )
        void construct_ioport_dkong_in1_4(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN1");      /* IN1 */
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_UNKNOWN ); /* not connected - held to high */
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_UNKNOWN ); /* not connected - held to high */
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_UNKNOWN ); /* not connected - held to high */

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( dkong_in2 )
        void construct_ioport_dkong_in2(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            dkong_state dkong_state = (dkong_state)owner;

            /* Bit 0x80 is (SERVICE OR COIN) !
             * Bit 0x40 mcu status (sound feedback) is inverted bit4 from port B (8039)
             * Bit 0x01 is going to the connector but it is not labeled
             */
            PORT_START("IN2");      /* IN2 */
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_SERVICE ); /* connection not labeled in schematics - diagnostic rom */
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_UNKNOWN ); /* connection not labeled in schematics - freeze or reset */
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_START1 );
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_START2 );
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_UNKNOWN );   /* not connected - held to high */
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_UNKNOWN );   /* not connected - held to high */
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_READ_LINE_DEVICE_MEMBER("virtual_p2", () => { return ((latch8_device)dkong_state.subdevice("virtual_p2")).bit4_q_r(); });  //latch8_device, bit4_q_r) /* status from sound cpu */
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_COIN1 );

            PORT_START("SERVICE1");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_SERVICE1 );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( dkong_dsw0 )
        void construct_ioport_dkong_dsw0(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("DSW0");      /* DSW0 */
            PORT_DIPNAME( 0x03, 0x00, DEF_STR( Lives ) );        PORT_DIPLOCATION( "SW1:!1,!2" );
            PORT_DIPSETTING(    0x00, "3" );
            PORT_DIPSETTING(    0x01, "4" );
            PORT_DIPSETTING(    0x02, "5" );
            PORT_DIPSETTING(    0x03, "6" );
            PORT_DIPNAME( 0x0c, 0x00, DEF_STR( Bonus_Life ) );   PORT_DIPLOCATION( "SW1:!3,!4" );
            PORT_DIPSETTING(    0x00, "7000" );
            PORT_DIPSETTING(    0x04, "10000" );
            PORT_DIPSETTING(    0x08, "15000" );
            PORT_DIPSETTING(    0x0c, "20000" );
            PORT_DIPNAME( 0x70, 0x00, DEF_STR( Coinage ) );      PORT_DIPLOCATION( "SW1:!5,!6,!7" );
            PORT_DIPSETTING(    0x70, DEF_STR( _5C_1C ) );
            PORT_DIPSETTING(    0x50, DEF_STR( _4C_1C ) );
            PORT_DIPSETTING(    0x30, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x10, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x20, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x40, DEF_STR( _1C_3C ) );
            PORT_DIPSETTING(    0x60, DEF_STR( _1C_4C ) );
            PORT_DIPNAME( 0x80, 0x80, DEF_STR( Cabinet ) );      PORT_DIPLOCATION( "SW1:!8" );
            PORT_DIPSETTING(    0x80, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Cocktail ) );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( dkong_config )
        void construct_ioport_dkong_config(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("VR2");
            PORT_ADJUSTER( 90, "VR2 - DAC Volume" );

            PORT_START("VIDHW");
            PORT_CONFNAME( 0x01, 0x01, "Video Hardware" );
            PORT_CONFSETTING(    0x00, "TKG-02 (Radarscope Conversion)" );
            PORT_CONFSETTING(    0x01, "TKG-04 (Two board set)" );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( dkong )
        void construct_ioport_dkong(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_INCLUDE( construct_ioport_dkong_in0_4, ref errorbuf );
            PORT_INCLUDE( construct_ioport_dkong_in1_4, ref errorbuf );
            PORT_INCLUDE( construct_ioport_dkong_in2, ref errorbuf );
            PORT_INCLUDE( construct_ioport_dkong_dsw0, ref errorbuf );

            PORT_INCLUDE( construct_ioport_dkong_config, ref errorbuf );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( dkong3 )
        void construct_ioport_dkong3(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");      /* IN0 */
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ); PORT_4WAY();
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ); PORT_4WAY();
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ); PORT_4WAY();
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ); PORT_4WAY();
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_BUTTON1 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_START1 );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_START2 );
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_SERVICE );

            PORT_START("IN1");      /* IN1 */
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_COIN1 ); PORT_IMPULSE(1);
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_COIN2 ); PORT_IMPULSE(1);
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_UNKNOWN );

            PORT_START("DSW0");      /* DSW0 */
            PORT_DIPNAME( 0x07, 0x00, DEF_STR( Coinage ) );          PORT_DIPLOCATION("SW2:!1,!2,!3");
            PORT_DIPSETTING(    0x02, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x04, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x06, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x01, DEF_STR( _1C_3C ) );
            PORT_DIPSETTING(    0x03, DEF_STR( _1C_4C ) );
            PORT_DIPSETTING(    0x05, DEF_STR( _1C_5C ) );
            PORT_DIPSETTING(    0x07, DEF_STR( _1C_6C ) );
            PORT_DIPUNKNOWN_DIPLOC( 0x08, 0x00, "SW2:!4" );
            PORT_DIPUNKNOWN_DIPLOC( 0x10, 0x00, "SW2:!5" );
            PORT_DIPUNKNOWN_DIPLOC( 0x20, 0x00, "SW2:!6" );
            PORT_SERVICE_DIPLOC( 0x40, IP_ACTIVE_HIGH, "SW2:!7" );
            PORT_DIPNAME( 0x80, 0x00, DEF_STR( Cabinet ) );          PORT_DIPLOCATION("SW2:!8");
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x80, DEF_STR( Cocktail ) );

            PORT_START("DSW1");      /* DSW1 */
            PORT_DIPNAME( 0x03, 0x00, DEF_STR( Lives ) );            PORT_DIPLOCATION("SW1:!1,!2");
            PORT_DIPSETTING(    0x00, "3" );
            PORT_DIPSETTING(    0x01, "4" );
            PORT_DIPSETTING(    0x02, "5" );
            PORT_DIPSETTING(    0x03, "6" );
            PORT_DIPNAME( 0x0c, 0x00, DEF_STR( Bonus_Life ) );       PORT_DIPLOCATION("SW1:!3,!4");
            PORT_DIPSETTING(    0x00, "30000" );
            PORT_DIPSETTING(    0x04, "40000" );
            PORT_DIPSETTING(    0x08, "50000" );
            PORT_DIPSETTING(    0x0c, DEF_STR( None ) );
            PORT_DIPNAME( 0x30, 0x00, "Additional Bonus" );          PORT_DIPLOCATION("SW1:!5,!6");
            PORT_DIPSETTING(    0x00, "30000" );
            PORT_DIPSETTING(    0x10, "40000" );
            PORT_DIPSETTING(    0x20, "50000" );
            PORT_DIPSETTING(    0x30, DEF_STR( None ) );
            PORT_DIPNAME( 0xc0, 0x00, DEF_STR( Difficulty ) );       PORT_DIPLOCATION("SW1:!7,!8");
            PORT_DIPSETTING(    0x00, "1" );
            PORT_DIPSETTING(    0x40, "2" );
            PORT_DIPSETTING(    0x80, "3" );
            PORT_DIPSETTING(    0xc0, "4" );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( dkongjr )
        void construct_ioport_dkongjr(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_INCLUDE( construct_ioport_dkong_in0_4, ref errorbuf );
            PORT_INCLUDE( construct_ioport_dkong_in1_4, ref errorbuf );
            PORT_INCLUDE( construct_ioport_dkong_in2, ref errorbuf );

            PORT_MODIFY("IN2");
            PORT_BIT(0x40, IP_ACTIVE_LOW, IPT_CUSTOM);   /* dkongjr does not have the mcu line connected */

            PORT_INCLUDE( construct_ioport_dkong_dsw0, ref errorbuf );
            PORT_MODIFY("DSW0");
            PORT_DIPNAME( 0x0c, 0x00, DEF_STR( Bonus_Life ) );   PORT_DIPLOCATION( "SW1:!3,!4" );
            PORT_DIPSETTING(    0x00, "10000" );
            PORT_DIPSETTING(    0x04, "15000" );
            PORT_DIPSETTING(    0x08, "20000" );
            PORT_DIPSETTING(    0x0c, "25000" );

#if DEBUG_DISC_SOUND
            PORT_START("TST")      /* TST */
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_OTHER ) PORT_CODE(KEYCODE_A)
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_OTHER ) PORT_CODE(KEYCODE_B)
#endif

            INPUT_PORTS_END();
        }
    }


    partial class dkong_state : driver_device
    {
        /*************************************
         *
         *  Graphics definitions
         *
         *************************************/

        static readonly gfx_layout spritelayout = new gfx_layout
        (
            16,16,                                  /* 16*16 sprites */
            RGN_FRAC(1,4),                          /* 128 sprites */
            2,                                      /* 2 bits per pixel */
            ArrayCombineUInt32( RGN_FRAC(1,2), RGN_FRAC(0,2) ),       /* the two bitplanes are separated */
            ArrayCombineUInt32( STEP8(0,1), STEP8((int)RGN_FRAC(1,4), 1) ), /* the two halves of the sprite are separated */
            ArrayCombineUInt32( STEP16(0,8) ),
            16*8                                    /* every sprite takes 16 consecutive bytes */
        );


        //static GFXDECODE_START( gfx_dkong )
        static readonly gfx_decode_entry [] gfx_dkong =
        {
            GFXDECODE_ENTRY( "gfx1", 0x0000, gfx_8x8x2_planar,   0, 64 ),
            GFXDECODE_ENTRY( "gfx2", 0x0000, spritelayout,       0, 64 ),
            //GFXDECODE_END
        };


        /*************************************
         *
         *  Machine driver
         *
         *************************************/

        //WRITE_LINE_MEMBER(dkong_state::vblank_irq)
        void vblank_irq(int state)
        {
            if (state != 0 && m_nmi_mask != 0)
                m_maincpu.op0.set_input_line(INPUT_LINE_NMI, ASSERT_LINE);
        }


        //WRITE_LINE_MEMBER(dkong_state::busreq_w )
        void busreq_w(int state)
        {
            // since our Z80 has no support for BUSACK, we assume it is granted immediately
            m_maincpu.op0.set_input_line(z80_device.Z80_INPUT_LINE_BUSRQ, state);
            m_maincpu.op0.set_input_line(INPUT_LINE_HALT, state); // do we need this?

            if (m_z80dma.op0 != null)
                throw new emu_unimplemented();
            else if (m_dma8257.op0 != null)
                m_dma8257.op0.hlda_w(state);
        }


        void dkong_base(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, CLOCK_1H);
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, dkong_map);

            MCFG_MACHINE_START_OVERRIDE(config, machine_start_dkong2b);
            MCFG_MACHINE_RESET_OVERRIDE(config, machine_reset_dkong);

            I8257(config, m_dma8257, CLOCK_1H);
            m_dma8257.op0.out_hrq_cb().set((write_line_delegate)busreq_w).reg();
            m_dma8257.op0.in_memr_cb().set(memory_read_byte).reg();
            m_dma8257.op0.out_memw_cb().set(memory_write_byte).reg();
            m_dma8257.op0.in_ior_cb(1).set(p8257_ctl_r).reg();
            m_dma8257.op0.out_iow_cb(0).set(p8257_ctl_w).reg();
            m_dma8257.op0.set_reverse_rw_mode(true); // why?

            /* video hardware */
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_raw(PIXEL_CLOCK, HTOTAL, HBEND, HBSTART, VTOTAL, VBEND, VBSTART);
            m_screen.op0.set_screen_update(screen_update_dkong);
            m_screen.op0.set_palette(m_palette);
            m_screen.op0.screen_vblank().set((write_line_delegate)vblank_irq).reg();

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_dkong);
            PALETTE(config, m_palette, dkong2b_palette, DK2B_PALETTE_LENGTH);

            MCFG_VIDEO_START_OVERRIDE(config, video_start_dkong);
        }


        public void dkong2b(machine_config config)
        {
            dkong_base(config);

            /* basic machine hardware */
            MCFG_MACHINE_START_OVERRIDE(config, machine_start_dkong2b);
            m_palette.op0.set_entries(DK2B_PALETTE_LENGTH);

            /* sound hardware */
            dkong2b_audio(config);

            WATCHDOG_TIMER(config, m_watchdog);
        }


        public void dkong3(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, new XTAL(8_000_000) / 2); /* verified in schematics */
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, dkong3_map);
            m_maincpu.op0.memory().set_addrmap(AS_IO, dkong3_io_map);

            MCFG_MACHINE_START_OVERRIDE(config, machine_start_dkong3);

            Z80DMA(config, m_z80dma, CLOCK_1H);
            m_z80dma.op0.out_busreq_callback().set_inputline(m_maincpu, INPUT_LINE_HALT).reg();
            m_z80dma.op0.in_mreq_callback().set(memory_read_byte).reg();
            m_z80dma.op0.out_mreq_callback().set(memory_write_byte).reg();

            /* video hardware */
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_raw(PIXEL_CLOCK, HTOTAL, HBEND, HBSTART, VTOTAL, VBEND, VBSTART);
            m_screen.op0.set_screen_update(screen_update_dkong);
            m_screen.op0.set_palette(m_palette);
            m_screen.op0.screen_vblank().set((write_line_delegate)vblank_irq).reg();
            m_screen.op0.screen_vblank().append_inputline(m_dev_n2a03a, INPUT_LINE_NMI).reg();
            m_screen.op0.screen_vblank().append_inputline(m_dev_n2a03b, INPUT_LINE_NMI).reg();

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_dkong);
            PALETTE(config, m_palette, dkong3_palette, DK3_PALETTE_LENGTH);

            MCFG_VIDEO_START_OVERRIDE(config, video_start_dkong);

            /* sound hardware */
            dkong3_audio(config);
        }


        public void dkongjr(machine_config config)
        {
            dkong_base(config);

            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, dkongjr_map);

            /* sound hardware */
            dkongjr_audio(config);

            WATCHDOG_TIMER(config, m_watchdog);
        }
    }


    partial class dkong : construct_ioport_helper
    {
        /*************************************
         *
         *  ROM definitions
         *
         *************************************/

        //ROM_START( dkong ) /* Confirmed TKG-04 Upgrade as mentioned in Nintendo Service Department Bulletin # TKG-02 12-11-81 */
        static readonly tiny_rom_entry [] rom_dkong =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "c_5et_g.bin",  0x0000, 0x1000, CRC("ba70b88b") + SHA1("d76ebecfea1af098d843ee7e578e480cd658ac1a") ),
            ROM_LOAD( "c_5ct_g.bin",  0x1000, 0x1000, CRC("5ec461ec") + SHA1("acb11a8fbdbb3ab46068385fe465f681e3c824bd") ),
            ROM_LOAD( "c_5bt_g.bin",  0x2000, 0x1000, CRC("1c97d324") + SHA1("c7966261f3a1d3296927e0b6ee1c58039fc53c1f") ),
            ROM_LOAD( "c_5at_g.bin",  0x3000, 0x1000, CRC("b9005ac0") + SHA1("3fe3599f6fa7c496f782053ddf7bacb453d197c4") ),

            ROM_REGION( 0x1800, "soundcpu", 0 ), /* sound */
            ROM_LOAD( "s_3i_b.bin",   0x0000, 0x0800, CRC("45a4ed06") + SHA1("144d24464c1f9f01894eb12f846952290e6e32ef") ),
            ROM_RELOAD(               0x0800, 0x0800 ),
            ROM_LOAD( "s_3j_b.bin",   0x1000, 0x0800, CRC("4743fe92") + SHA1("6c82b57637c0212a580591397e6a5a1718f19fd2") ),

            ROM_REGION( 0x1000, "gfx1", 0 ),
            ROM_LOAD( "v_5h_b.bin",   0x0000, 0x0800, CRC("12c8c95d") + SHA1("a57ff5a231c45252a63b354137c920a1379b70a3") ),
            ROM_LOAD( "v_3pt.bin",    0x0800, 0x0800, CRC("15e9c5e9") + SHA1("976eb1e18c74018193a35aa86cff482ebfc5cc4e") ),

            ROM_REGION( 0x2000, "gfx2", 0 ),
            ROM_LOAD( "l_4m_b.bin",   0x0000, 0x0800, CRC("59f8054d") + SHA1("793dba9bf5a5fe76328acdfb90815c243d2a65f1") ),
            ROM_LOAD( "l_4n_b.bin",   0x0800, 0x0800, CRC("672e4714") + SHA1("92e5d379f4838ac1fa44d448ce7d142dae42102f") ),
            ROM_LOAD( "l_4r_b.bin",   0x1000, 0x0800, CRC("feaa59ee") + SHA1("ecf95db5a20098804fc8bd59232c66e2e0ed3db4") ),
            ROM_LOAD( "l_4s_b.bin",   0x1800, 0x0800, CRC("20f2ef7e") + SHA1("3bc482a38bf579033f50082748ee95205b0f673d") ),

            ROM_REGION( 0x0300, "proms", 0 ),
            ROM_LOAD( "c-2k.bpr",     0x0000, 0x0100, CRC("e273ede5") + SHA1("b50ec9e1837c00c20fb2a4369ec7dd0358321127") ), /* palette low 4 bits (inverted) */
            ROM_LOAD( "c-2j.bpr",     0x0100, 0x0100, CRC("d6412358") + SHA1("f9c872da2fe8e800574ae3bf483fb3ccacc92eb3") ), /* palette high 4 bits (inverted) */
            ROM_LOAD( "v-5e.bpr",     0x0200, 0x0100, CRC("b869b8f5") + SHA1("c2bdccbf2654b64ea55cd589fd21323a9178a660") ), /* character color codes on a per-column basis */

            ROM_END,
        };


        //ROM_START( dkongjr )
        static readonly tiny_rom_entry [] rom_dkongjr =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "djr1-c_5b_f-2.5b", 0x0000, 0x1000, CRC("dea28158") + SHA1("08baf84ae6f9b40a2c743fe1d8c158c74a40e95a") ),
            ROM_CONTINUE(                 0x3000, 0x1000 ),
            ROM_LOAD( "djr1-c_5c_f-2.5c", 0x2000, 0x0800, CRC("6fb5faf6") + SHA1("ce1cfde71a9e2a8b5896a6301d386f72869a1d2e") ),
            ROM_CONTINUE(                 0x4800, 0x0800 ),
            ROM_CONTINUE(                 0x1000, 0x0800 ),
            ROM_CONTINUE(                 0x5800, 0x0800 ),
            ROM_LOAD( "djr1-c_5e_f-2.5e", 0x4000, 0x0800, CRC("d042b6a8") + SHA1("57ac237d273496b44220b4437118115ef11dbd9f") ),
            ROM_CONTINUE(                 0x2800, 0x0800 ),
            ROM_CONTINUE(                 0x5000, 0x0800 ),
            ROM_CONTINUE(                 0x1800, 0x0800 ),
            //empty socket on position 5A on pcb 0x8000, 0x1000

            ROM_REGION( 0x1000, "soundcpu", 0 ), /* sound */
            ROM_LOAD( "djr1-c_3h.3h",     0x0000, 0x1000, CRC("715da5f8") + SHA1("f708c3fd374da65cbd9fe2e191152f5d865414a0") ),

            ROM_REGION( 0x2000, "gfx1", 0 ),
            ROM_LOAD( "djr1-v.3n",        0x0000, 0x1000, CRC("8d51aca9") + SHA1("64887564b079d98e98aafa53835e398f34fe4e3f") ),
            ROM_LOAD( "djr1-v.3p",        0x1000, 0x1000, CRC("4ef64ba5") + SHA1("41a7a4005087951f57f62c9751d62a8c495e6bb3") ),

            ROM_REGION( 0x2000, "gfx2", 0 ),
            ROM_LOAD( "djr1-v_7c.7c",     0x0000, 0x0800, CRC("dc7f4164") + SHA1("07a6242e95b5c3b8dfdcd4b4950f463dba16dd77") ),
            ROM_LOAD( "djr1-v_7d.7d",     0x0800, 0x0800, CRC("0ce7dcf6") + SHA1("0654b77526c49f0dfa077ac4f1f69cf5cb2e2f64") ),
            ROM_LOAD( "djr1-v_7e.7e",     0x1000, 0x0800, CRC("24d1ff17") + SHA1("696854bf3dc5447d33b4815db357e6ce3834d867") ),
            ROM_LOAD( "djr1-v_7f.7f",     0x1800, 0x0800, CRC("0f8c083f") + SHA1("0b688ae9da296b2447fffa5e135fd6a56ec3e790") ),

            ROM_REGION( 0x0300, "proms", 0 ),
            ROM_LOAD( "djr1-c-2e.2e",     0x0000, 0x0100, CRC("463dc7ad") + SHA1("b2c9f22facc8885be2d953b056eb8dcddd4f34cb") ),   /* palette low 4 bits (inverted) */
            ROM_LOAD( "djr1-c-2f.2f",     0x0100, 0x0100, CRC("47ba0042") + SHA1("dbec3f4b8013628c5b8f83162e5f8b1f82f6ee5f") ),   /* palette high 4 bits (inverted) */
            ROM_LOAD( "djr1-v-2n.2n",     0x0200, 0x0100, CRC("dbf185bf") + SHA1("2697a991a4afdf079dd0b7e732f71c7618f43b70") ),   /* character color codes on a per-column basis */

            ROM_END,
        };


        //ROM_START( dkong3 )
        static readonly tiny_rom_entry [] rom_dkong3 =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "dk3c.7b",      0x0000, 0x2000, CRC("38d5f38e") + SHA1("5a6bb0e5070211515e3d56bd7d4c2d1655ac1621") ),
            ROM_LOAD( "dk3c.7c",      0x2000, 0x2000, CRC("c9134379") + SHA1("ecddb3694b93cb3dc98c3b1aeeee928e27529aba") ),
            ROM_LOAD( "dk3c.7d",      0x4000, 0x2000, CRC("d22e2921") + SHA1("59a4a1a36aaca19ee0a7255d832df9d042ba34fb") ),
            ROM_LOAD( "dk3c.7e",      0x8000, 0x2000, CRC("615f14b7") + SHA1("145674073e95d97c9131b6f2b03303eadb57ca78") ),

            ROM_REGION( 0x10000, "n2a03a", 0 ),  /* sound #1 */
            ROM_LOAD( "dk3c.5l",      0xe000, 0x2000, CRC("7ff88885") + SHA1("d530581778aab260e21f04c38e57ba34edea7c64") ),

            ROM_REGION( 0x10000, "n2a03b", 0 ),  /* sound #2 */
            ROM_LOAD( "dk3c.6h",      0xe000, 0x2000, CRC("36d7200c") + SHA1("7965fcb9bc1c0fdcae8a8e79df9c7b7439c506d8") ),

            ROM_REGION( 0x2000, "gfx1", 0 ),
            ROM_LOAD( "dk3v.3n",      0x0000, 0x1000, CRC("415a99c7") + SHA1("e0855b03bb1dc0d8ae46da9fe33ca30ecf6a2e96") ),
            ROM_LOAD( "dk3v.3p",      0x1000, 0x1000, CRC("25744ea0") + SHA1("4866e43e80b010ccf2c8cc94c232786521f9e26e") ),

            ROM_REGION( 0x4000, "gfx2", 0 ),
            ROM_LOAD( "dk3v.7c",      0x0000, 0x1000, CRC("8ffa1737") + SHA1("fa5896124227d412fbdf83f129ddffa32cf2053b") ),
            ROM_LOAD( "dk3v.7d",      0x1000, 0x1000, CRC("9ac84686") + SHA1("a089376b9c23094490703152ad98ed27f519402d") ),
            ROM_LOAD( "dk3v.7e",      0x2000, 0x1000, CRC("0c0af3fb") + SHA1("03e0c3f51bc3c20f95cb02f76f2d80188d5dbe36") ),
            ROM_LOAD( "dk3v.7f",      0x3000, 0x1000, CRC("55c58662") + SHA1("7f3d5a1b386cc37d466e42392ffefc928666a8dc") ),

            ROM_REGION( 0x0500, "proms", 0 ),
            ROM_LOAD( "dkc1-c.1d",    0x0000, 0x0200, CRC("df54befc") + SHA1("7912dbf0a0c8ef68f4ae0f95e55ab164da80e4a1") ), /* palette red & green component */
            ROM_LOAD( "dkc1-c.1c",    0x0200, 0x0200, CRC("66a77f40") + SHA1("c408d65990f0edd78c4590c447426f383fcd2d88") ), /* palette blue component */
            ROM_LOAD( "dkc1-v.2n",    0x0400, 0x0100, CRC("50e33434") + SHA1("b63da9bed9dc4c7da78e4c26d4ba14b65f2b7e72") ), /* character color codes on a per-column basis */

            ROM_REGION( 0x0020, "adrdecode", 0 ),
            /* address decode prom 18s030 - this has inverted outputs. The dump does not reflect this. */
            ROM_LOAD( "dkc1-v.5e",    0x0000, 0x0020, CRC("d3e2eaf8") + SHA1("87bb298137c26570dafb4ac495c87e82441e70e5") ),

            ROM_END,
        };


        static void dkong_state_dkong2b(machine_config config, device_t device) { dkong_state dkong_state = (dkong_state)device; dkong_state.dkong2b(config); }
        static void dkong_state_dkongjr(machine_config config, device_t device) { dkong_state dkong_state = (dkong_state)device; dkong_state.dkongjr(config); }
        static void dkong_state_dkong3(machine_config config, device_t device) { dkong_state dkong_state = (dkong_state)device; dkong_state.dkong3(config); }


        static dkong m_dkong = new dkong();


        static device_t device_creator_dkong(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new dkong_state(mconfig, type, tag); }
        static device_t device_creator_dkongjr(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new dkong_state(mconfig, type, tag); }
        static device_t device_creator_dkong3(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new dkong_state(mconfig, type, tag); }


        /*************************************
         *
         *  Game drivers
         *
         *************************************/

        //                                                       creator,                rom          YEAR,   NAME,      PARENT,  MACHINE,             INPUT,                            INIT,                     MONITOR, COMPANY,               FULLNAME,                          FLAGS
        public static readonly game_driver driver_dkong   = GAME(device_creator_dkong,   rom_dkong,   "1981", "dkong",   "0",     dkong_state_dkong2b, m_dkong.construct_ioport_dkong,   driver_device.empty_init, ROT270,  "Nintendo of America", "Donkey Kong (US set 1)",          MACHINE_SUPPORTS_SAVE);
        public static readonly game_driver driver_dkongjr = GAME(device_creator_dkongjr, rom_dkongjr, "1982", "dkongjr", "0",     dkong_state_dkongjr, m_dkong.construct_ioport_dkongjr, driver_device.empty_init, ROT270,  "Nintendo of America", "Donkey Kong Junior (US set F-2)", MACHINE_SUPPORTS_SAVE);
        public static readonly game_driver driver_dkong3  = GAME(device_creator_dkong3,  rom_dkong3,  "1983", "dkong3",  "0",     dkong_state_dkong3,  m_dkong.construct_ioport_dkong3,  driver_device.empty_init, ROT270,  "Nintendo of America", "Donkey Kong 3 (US)",              MACHINE_SUPPORTS_SAVE);
    }
}
