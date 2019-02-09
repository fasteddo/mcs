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
    partial class dkong_state : driver_device
    {
        /*************************************
         *
         *  statics
         *
         *************************************/

        //READ8_MEMBER(dkong_state::memory_read_byte)
        public u8 memory_read_byte(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            address_space prog_space = m_maincpu.target.GetClassInterface<device_memory_interface>().space(AS_PROGRAM);
            return prog_space.read_byte(offset);
        }


        //WRITE8_MEMBER(dkong_state::memory_write_byte)
        public void memory_write_byte(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            address_space prog_space = m_maincpu.target.GetClassInterface<device_memory_interface>().space(AS_PROGRAM);
            prog_space.write_byte(offset, data);
        }


        /*************************************
         *
         *  Machine setup
         *
         *************************************/

        //MACHINE_START_MEMBER(dkong_state,dkong2b)
        public void machine_start_dkong2b()
        {
            m_hardware_type = HARDWARE_TKG04;

            save_item(m_decrypt_counter, "m_decrypt_counter");
            save_item(m_dma_latch, "m_dma_latch");
        }


        //MACHINE_RESET_MEMBER(dkong_state,dkong)
        public void machine_reset_dkong()
        {
            /* nothing */
        }


        /*************************************
         *
         *  DMA handling
         *
         *************************************/

        //READ8_MEMBER(dkong_state::hb_dma_read_byte)
        //{
        //    int   bucket = m_rev_map[(offset>>10) & 0x1ff];
        //    int   addr;
        //
        //    if (bucket < 0)
        //        fatalerror("hb_dma_read_byte - unmapped access for 0x%02x - bucket 0x%02x\n", offset, bucket);
        //
        //    addr = ((bucket << 7) & 0x7c00) | (offset & 0x3ff);
        //    address_space &prog_space = m_maincpu->space(AS_PROGRAM);
        //    return prog_space.read_byte(addr);
        //}

        //WRITE8_MEMBER(dkong_state::hb_dma_write_byte)
        //{
        //    int   bucket = m_rev_map[(offset>>10) & 0x1ff];
        //    int   addr;
        //
        //    if (bucket < 0)
        //        fatalerror("hb_dma_read_byte - unmapped access for 0x%02x - bucket 0x%02x\n", offset, bucket);
        //
        //    addr = ((bucket << 7) & 0x7c00) | (offset & 0x3ff);
        //    address_space &prog_space = m_maincpu->space(AS_PROGRAM);
        //    prog_space.write_byte(addr, data);
        //}

        //READ8_MEMBER(dkong_state::p8257_ctl_r)
        public u8 p8257_ctl_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return m_dma_latch;
        }


        //WRITE8_MEMBER(dkong_state::p8257_ctl_w)
        public void p8257_ctl_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_dma_latch = data;
        }


        /*************************************
         *
         *  Output ports
         *
         *************************************/

        //WRITE8_MEMBER(dkong_state::dkong3_coin_counter_w)
        //{
        //    machine().bookkeeping().coin_counter_w(offset, data & 0x01);
        //}


        //WRITE8_MEMBER(dkong_state::p8257_drq_w)
        public void p8257_drq_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_dma8257.target.dreq0_w(data & 0x01);
            m_dma8257.target.dreq1_w(data & 0x01);
            machine().scheduler().abort_timeslice(); // transfer occurs immediately
            machine().scheduler().boost_interleave(attotime.zero, attotime.from_usec(100)); // smooth things out a bit
        }


        //READ8_MEMBER(dkong_state::dkong_in2_r)
        public u8 dkong_in2_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            // 2 board DK and all DKjr has a watchdog
            if (m_watchdog.target != null)
                m_watchdog.target.reset_w(space, 0, 0);

            uint8_t r = (uint8_t)ioport("IN2").read();
            machine().bookkeeping().coin_counter_w((int)offset, r >> 7);
            if ((ioport("SERVICE1").read() & 1) != 0)
                r |= 0x80; /* service ==> coin */

            return r;
        }


        //READ8_MEMBER(dkong_state::dkong_in2_r)
        //{
        //    // 2 board DK and all DKjr has a watchdog
        //    if (m_watchdog)
        //        m_watchdog->reset_w(space, 0, 0);
        //
        //    uint8_t r = ioport("IN2")->read();
        //    machine().bookkeeping().coin_counter_w(offset, r >> 7);
        //    if (ioport("SERVICE1")->read() & 1)
        //        r |= 0x80; /* service ==> coin */
        //    return r;
        //}

        //READ8_MEMBER(dkong_state::s2650_mirror_r)
        //{
        //    return space.read_byte(0x1000 + offset);
        //}


        //WRITE8_MEMBER(dkong_state::s2650_mirror_w)
        //{
        //    space.write_byte(0x1000 + offset, data);
        //}


        //READ8_MEMBER(dkong_state::epos_decrypt_rom)
        //{
        //    if (offset & 0x01)
        //    {
        //        m_decrypt_counter = m_decrypt_counter - 1;
        //        if (m_decrypt_counter < 0)
        //            m_decrypt_counter = 0x0F;
        //    }
        //    else
        //    {
        //        m_decrypt_counter = (m_decrypt_counter + 1) & 0x0F;
        //    }
        //
        //    switch(m_decrypt_counter)
        //    {
        //        case 0x08:  membank("bank1")->set_entry(0);      break;
        //        case 0x09:  membank("bank1")->set_entry(1);      break;
        //        case 0x0A:  membank("bank1")->set_entry(2);      break;
        //        case 0x0B:  membank("bank1")->set_entry(3);      break;
        //        default:
        //            logerror("Invalid counter = %02X\n",m_decrypt_counter);
        //            break;
        //    }
        //
        //    return 0;
        //}


        //WRITE8_MEMBER(dkong_state::s2650_data_w)
        //{
        //#if DEBUG_PROTECTION
        //    logerror("write : pc = %04x, loopback = %02x\n",m_maincpu->pc(), data);
        //#endif
        //
        //    m_hunchloopback = data;
        //}

        //WRITE_LINE_MEMBER(dkong_state::s2650_fo_w)
        //{
        //#if DEBUG_PROTECTION
        //    logerror("%s write : FO = %02x\n", machine().describe_context(), data);
        //#endif
        //
        //    m_main_fo = state;
        //
        //    if (m_main_fo)
        //        m_hunchloopback = 0xfb;
        //}

        //READ8_MEMBER(dkong_state::s2650_port0_r)
        //{
        //#if DEBUG_PROTECTION
        //    logerror("port 0 : pc = %04x, loopback = %02x fo=%d\n",m_maincpu->pc(), m_hunchloopback, m_main_fo);
        //#endif
        //
        //    switch (m_protect_type)
        //    {
        //        case DK2650_SHOOTGAL:
        //        case DK2650_HUNCHBKD:
        //            if (m_main_fo)
        //                return m_hunchloopback;
        //            else
        //                return m_hunchloopback--;
        //        case DK2650_SPCLFORC:
        //            if (!m_main_fo)
        //                return m_hunchloopback;
        //            else
        //                return m_hunchloopback--;
        //    }
        //    fatalerror("Unhandled read from port 0 : pc = %4x\n",m_maincpu->pc());
        //}


        //READ8_MEMBER(dkong_state::s2650_port1_r)
        //{
        //#if DEBUG_PROTECTION
        //    logerror("port 1 : pc = %04x, loopback = %02x fo=%d\n",m_maincpu->pc(), m_hunchloopback, m_main_fo);
        //#endif
        //
        //    switch (m_protect_type)
        //    {
        //        case DK2650_HUNCHBKD:
        //            return m_hunchloopback--;
        //        case DK2650_EIGHTACT:
        //        case DK2650_HERBIEDK:
        //            if (m_hunchloopback & 0x80)
        //                return m_prot_cnt;
        //            else
        //                return ++m_prot_cnt;
        //    }
        //    fatalerror("Unhandled read from port 1 : pc = %4x\n",m_maincpu->pc());
        //}


        //WRITE8_MEMBER(dkong_state::dkong3_2a03_reset_w)
        //{
        //    if (data & 1)
        //    {
        //        m_dev_n2a03a->set_input_line(INPUT_LINE_RESET, CLEAR_LINE);
        //        m_dev_n2a03b->set_input_line(INPUT_LINE_RESET, CLEAR_LINE);
        //    }
        //    else
        //    {
        //        m_dev_n2a03a->set_input_line(INPUT_LINE_RESET, ASSERT_LINE);
        //        m_dev_n2a03b->set_input_line(INPUT_LINE_RESET, ASSERT_LINE);
        //    }
        //}

        //READ8_MEMBER(dkong_state::strtheat_inputport_0_r)
        //{
        //    if(ioport("DSW0")->read() & 0x40)
        //    {
        //        /* Joystick inputs */
        //        return ioport("IN0")->read();
        //    }
        //    else
        //    {
        //        /* Steering Wheel inputs */
        //        return (ioport("IN0")->read() & ~3) | (ioport("IN4")->read() & 3);
        //    }
        //}


        //READ8_MEMBER(dkong_state::strtheat_inputport_1_r)
        //{
        //    if(ioport("DSW0")->read() & 0x40)
        //    {
        //        /* Joystick inputs */
        //        return ioport("IN1")->read();
        //    }
        //    else
        //    {
        //        /* Steering Wheel inputs */
        //        return (ioport("IN1")->read() & ~3) | (ioport("IN5")->read() & 3);
        //    }
        //}

        //WRITE8_MEMBER(dkong_state::dkong_z80dma_rdy_w)
        //{
        //    m_z80dma->rdy_w(data & 0x01);
        //}


        //WRITE8_MEMBER(dkong_state::nmi_mask_w)
        public void nmi_mask_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_nmi_mask = (uint8_t)(data & 1);
            if (m_nmi_mask == 0)
                m_maincpu.target.set_input_line(device_execute_interface.INPUT_LINE_NMI, CLEAR_LINE);
        }


        /*************************************
         *
         *  Main CPU memory handlers
         *
         *************************************/

        public void dkong_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x4fff).rom();
            map.op(0x6000, 0x6bff).ram();
            map.op(0x7000, 0x73ff).ram().share("sprite_ram"); /* sprite set 1 */
            map.op(0x7400, 0x77ff).ram().w(dkong_videoram_w).share("video_ram");
            map.op(0x7800, 0x780f).rw(i8257_device_read, i8257_device_write);   /* P8257 control registers */
            map.op(0x7c00, 0x7c00).portr("IN0").w("ls175.3d", latch8_device_write);    /* IN0, sound CPU intf */
            map.op(0x7c80, 0x7c80).portr("IN1").w(radarscp_grid_color_w);/* IN1 */

            map.op(0x7d00, 0x7d00).r(dkong_in2_r);                               /* IN2 */
            map.op(0x7d00, 0x7d07).w(latch8_device_bit0_w);          /* Sound signals */

            map.op(0x7d80, 0x7d80).portr("DSW0").w(dkong_audio_irq_w);   /* DSW0 */
            map.op(0x7d81, 0x7d81).w(radarscp_grid_enable_w);
            map.op(0x7d82, 0x7d82).w(dkong_flipscreen_w);
            map.op(0x7d83, 0x7d83).w(dkong_spritebank_w);                       /* 2 PSL Signal */
            map.op(0x7d84, 0x7d84).w(nmi_mask_w);
            map.op(0x7d85, 0x7d85).w(p8257_drq_w);          /* P8257 ==> /DRQ0 /DRQ1 */
            map.op(0x7d86, 0x7d87).w(dkong_palettebank_w);
        }
    }


    public partial class dkong : global_object
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
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_READ_LINE_DEVICE_MEMBER("virtual_p2", dkong_state.latch8_device_bit4_q_r); /* status from sound cpu */
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

            PORT_INCLUDE( construct_ioport_dkong_in0_4, owner, portlist, ref errorbuf );
            PORT_INCLUDE( construct_ioport_dkong_in1_4, owner, portlist, ref errorbuf );
            PORT_INCLUDE( construct_ioport_dkong_in2, owner, portlist, ref errorbuf );
            PORT_INCLUDE( construct_ioport_dkong_dsw0, owner, portlist, ref errorbuf );

            PORT_INCLUDE( construct_ioport_dkong_config, owner, portlist, ref errorbuf );

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
        static readonly gfx_decode_entry [] gfx_dkong = new gfx_decode_entry[]
        {
            GFXDECODE_ENTRY( "gfx1", 0x0000, generic_global.gfx_8x8x2_planar,   0, 64 ),
            GFXDECODE_ENTRY( "gfx2", 0x0000, spritelayout,       0, 64 ),
            //GFXDECODE_END
        };


        /*************************************
         *
         *  Machine driver
         *
         *************************************/

        //WRITE_LINE_MEMBER(dkong_state::vblank_irq)
        public void vblank_irq(int state)
        {
            if (state != 0 && m_nmi_mask != 0)
                m_maincpu.target.set_input_line(device_execute_interface.INPUT_LINE_NMI, ASSERT_LINE);
        }


        //WRITE_LINE_MEMBER(dkong_state::busreq_w )
        public void busreq_w(int state)
        {
            // since our Z80 has no support for BUSACK, we assume it is granted immediately
            m_maincpu.target.set_input_line(z80_device.Z80_INPUT_LINE_BUSRQ, state);
            m_maincpu.target.set_input_line(device_execute_interface.INPUT_LINE_HALT, state); // do we need this?

            if (m_z80dma.target != null)
                throw new emu_unimplemented();
            else if (m_dma8257.target != null)
                m_dma8257.target.hlda_w(state);
        }


        //MACHINE_CONFIG_START(dkong_state::dkong_base)
        void dkong_base(machine_config config)
        {
            MACHINE_CONFIG_START(config, this);

            /* basic machine hardware */
            MCFG_DEVICE_ADD(m_maincpu, z80_device.Z80, CLOCK_1H);
            MCFG_DEVICE_PROGRAM_MAP(dkong_map);

            MCFG_MACHINE_START_OVERRIDE(config, machine_start_dkong2b);
            MCFG_MACHINE_RESET_OVERRIDE(config, machine_reset_dkong);

            I8257(config, m_dma8257, CLOCK_1H);
            m_dma8257.target.out_hrq_cb().set(busreq_w).reg();
            m_dma8257.target.in_memr_cb().set(memory_read_byte).reg();
            m_dma8257.target.out_memw_cb().set(memory_write_byte).reg();
            m_dma8257.target.in_ior_cb(1).set(p8257_ctl_r).reg();
            m_dma8257.target.out_iow_cb(0).set(p8257_ctl_w).reg();
            m_dma8257.target.set_reverse_rw_mode(true); // why?

            /* video hardware */
            MCFG_SCREEN_ADD(m_screen, SCREEN_TYPE_RASTER);
            MCFG_SCREEN_RAW_PARAMS(PIXEL_CLOCK, HTOTAL, HBEND, HBSTART, VTOTAL, VBEND, VBSTART);
            MCFG_SCREEN_UPDATE_DRIVER(screen_update_dkong);
            MCFG_SCREEN_PALETTE(m_palette);
            MCFG_SCREEN_VBLANK_CALLBACK(vblank_irq);

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_dkong);
            PALETTE(config, m_palette, dkong2b_palette, DK2B_PALETTE_LENGTH);

            MCFG_VIDEO_START_OVERRIDE(config, video_start_dkong);

            MACHINE_CONFIG_END();
        }


        //MACHINE_CONFIG_START(dkong_state::dkong2b)
        public void dkong2b(machine_config config)
        {
            dkong_base(config);

            MACHINE_CONFIG_START(config, this);

            /* basic machine hardware */
            MCFG_MACHINE_START_OVERRIDE(config, machine_start_dkong2b);
            m_palette.target.set_entries(DK2B_PALETTE_LENGTH);

            /* sound hardware */
            dkong2b_audio(config);

            WATCHDOG_TIMER(config, m_watchdog);

            MACHINE_CONFIG_END();
        }
    }


    public partial class dkong : global_object
    {
        /*************************************
         *
         *  ROM definitions
         *
         *************************************/

        //ROM_START( dkong ) /* Confirmed TKG-04 Upgrade as mentioned in Nintendo Service Department Bulletin # TKG-02 12-11-81 */
        static readonly List<tiny_rom_entry> rom_dkong = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "c_5et_g.bin",  0x0000, 0x1000, CRC("ba70b88b") + SHA1("d76ebecfea1af098d843ee7e578e480cd658ac1a") ),
            ROM_LOAD( "c_5ct_g.bin",  0x1000, 0x1000, CRC("5ec461ec") + SHA1("acb11a8fbdbb3ab46068385fe465f681e3c824bd") ),
            ROM_LOAD( "c_5bt_g.bin",  0x2000, 0x1000, CRC("1c97d324") + SHA1("c7966261f3a1d3296927e0b6ee1c58039fc53c1f") ),
            ROM_LOAD( "c_5at_g.bin",  0x3000, 0x1000, CRC("b9005ac0") + SHA1("3fe3599f6fa7c496f782053ddf7bacb453d197c4") ),
            ROM_LOAD( "diag.bin",     0x4000, 0x1000, NO_DUMP ),

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

            ROM_END(),
        };


        static void dkong_state_dkong2b(machine_config config, device_t device) { dkong_state dkong_state = (dkong_state)device; dkong_state.dkong2b(config); }


        static dkong m_dkong = new dkong();


        static device_t device_creator_dkong(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new dkong_state(mconfig, type, tag); }


        /*************************************
         *
         *  Game drivers
         *
         *************************************/

        //                                                      creator,              rom        YEAR,   NAME,       PARENT,  MACHINE,                     INPUT,                          INIT,                      MONITOR,COMPANY, FULLNAME,FLAGS
        public static readonly game_driver driver_dkong = GAME( device_creator_dkong, rom_dkong, "1981", "dkong",    null,    dkong.dkong_state_dkong2b,   m_dkong.construct_ioport_dkong, driver_device.empty_init,  ROT270, "Nintendo of America", "Donkey Kong (US set 1)",    MACHINE_SUPPORTS_SAVE );
    }
}
