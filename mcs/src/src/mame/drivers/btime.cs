// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using ioport_value = System.UInt32;  //typedef u32 ioport_value;
using s32 = System.Int32;
using u32 = System.UInt32;
using uint8_t = System.Byte;

using static mame.ay8910_global;
using static mame.decocpu7_global;
using static mame.device_global;
using static mame.diexec_global;
using static mame.digfx_global;
using static mame.discrete_global;
using static mame.disound_global;
using static mame.drawgfx_global;
using static mame.driver_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.gen_latch_global;
using static mame.hash_global;
using static mame.input_merger_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.m6502_global;
using static mame.rescap_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.timer_global;
using static mame.util;


namespace mame
{
    partial class btime_state : driver_device
    {
        static readonly XTAL MASTER_CLOCK     = new XTAL(12_000_000);
        static readonly XTAL HCLK             = MASTER_CLOCK / 2;
        static readonly XTAL HCLK1            = HCLK / 2;
        static readonly XTAL HCLK2            = HCLK1 / 2;
        //#define HCLK4            (HCLK2/2)

        //enum
        //{
        const uint8_t AUDIO_ENABLE_DIRECT = 0;        /* via direct address in memory map */
        const uint8_t AUDIO_ENABLE_AY8910 = 1;         /* via ay-8910 port A */
        //};


        void audio_nmi_enable_w(uint8_t data)
        {
            /* for most games, this serves as the NMI enable for the audio CPU; however,
               lnc and disco use bit 0 of the first AY-8910's port A instead; many other
               games also write there in addition to this address */
            if (m_audio_nmi_enable_type == AUDIO_ENABLE_DIRECT)
                m_audionmi.op0.in_w<u32_const_0>(BIT(data, 0));
        }


        void ay_audio_nmi_enable_w(uint8_t data)
        {
            /* port A bit 0, when 1, inhibits the NMI */
            if (m_audio_nmi_enable_type == AUDIO_ENABLE_AY8910)
                m_audionmi.op0.in_w<u32_const_0>(BIT(~data, 0));
        }


        //TIMER_DEVICE_CALLBACK_MEMBER(btime_state::audio_nmi_gen)
        void audio_nmi_gen(timer_device timer, object ptr, s32 param)  //void *ptr, s32 param
        {
            int scanline = param;
            m_audionmi.op0.in_w<u32_const_1>((scanline & 8) >> 3);
        }


        void btime_map(address_map map, device_t owner)
        {
            map.op(0x0000, 0x07ff).ram().share("rambase");
            map.op(0x0c00, 0x0c0f).ram().w(m_palette, (offset, data) => { m_palette.op0.write8(offset, data); }).share("palette");
            map.op(0x1000, 0x13ff).ram().share("videoram");
            map.op(0x1400, 0x17ff).ram().share("colorram");
            map.op(0x1800, 0x1bff).rw(btime_mirrorvideoram_r, btime_mirrorvideoram_w);
            map.op(0x1c00, 0x1fff).rw(btime_mirrorcolorram_r, btime_mirrorcolorram_w);
            map.op(0x4000, 0x4000).portr("P1").nopw();
            map.op(0x4001, 0x4001).portr("P2");
            map.op(0x4002, 0x4002).portr("SYSTEM").w(btime_video_control_w);
            map.op(0x4003, 0x4003).portr("DSW1").w(m_soundlatch, (data) => { m_soundlatch.op0.write(data); });
            map.op(0x4004, 0x4004).portr("DSW2").w(bnj_scroll1_w);
            map.op(0xb000, 0xffff).rom();
        }


        //void btime_state::cookrace_map(address_map &map)

        //void btime_state::tisland_map(address_map &map)

        //void btime_state::zoar_map(address_map &map)

        //void btime_state::lnc_map(address_map &map)

        //void btime_state::mmonkey_map(address_map &map)

        //void btime_state::bnj_map(address_map &map)

        //void btime_state::disco_map(address_map &map)

        //void btime_state::protenn_map(address_map &map)


        void audio_map(address_map map, device_t owner)
        {
            map.op(0x0000, 0x03ff).mirror(0x1c00).ram().share("audio_rambase");
            map.op(0x2000, 0x3fff).w("ay1", (data) => { ((ay8910_device)subdevice("ay1")).data_w(data); });
            map.op(0x4000, 0x5fff).w("ay1", (data) => { ((ay8910_device)subdevice("ay1")).address_w(data); });
            map.op(0x6000, 0x7fff).w("ay2", (data) => { ((ay8910_device)subdevice("ay2")).data_w(data); });
            map.op(0x8000, 0x9fff).w("ay2", (data) => { ((ay8910_device)subdevice("ay2")).address_w(data); });
            map.op(0xa000, 0xbfff).r(m_soundlatch, () => { return m_soundlatch.op0.read(); });
            map.op(0xc000, 0xdfff).w(audio_nmi_enable_w);
            map.op(0xe000, 0xefff).mirror(0x1000).rom();
        }


        //void btime_state::disco_audio_map(address_map &map)


        //INPUT_CHANGED_MEMBER(btime_state::coin_inserted_irq_hi)
        public void coin_inserted_irq_hi(ioport_field field, u32 param, ioport_value oldval, ioport_value newval)
        {
            if (newval != 0)
                m_maincpu.op0.set_input_line(0, HOLD_LINE);
        }


        //INPUT_CHANGED_MEMBER(btime_state::coin_inserted_irq_lo)

        //INPUT_CHANGED_MEMBER(btime_state::coin_inserted_nmi_lo)

        //uint8_t btime_state::zoar_dsw1_read()
    }


    partial class btime : construct_ioport_helper
    {
        //static INPUT_PORTS_START( btime )
        void construct_ioport_btime(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            btime_state btime_state = (btime_state)owner;

            PORT_START("P1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_4WAY();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_4WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_4WAY();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_4WAY();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("P2");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("SYSTEM");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_TILT );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_COIN1 ); PORT_CHANGED_MEMBER(DEVICE_SELF, btime_state.coin_inserted_irq_hi, 0);
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_COIN2 ); PORT_CHANGED_MEMBER(DEVICE_SELF, btime_state.coin_inserted_irq_hi, 0);

            PORT_START("DSW1"); // At location 15D on sound PCB
            PORT_DIPNAME( 0x03, 0x03, DEF_STR( Coin_A ) );     PORT_DIPLOCATION("SW1:1,2");
            PORT_DIPSETTING(    0x00, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x03, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x01, DEF_STR( _1C_3C ) );
            PORT_DIPNAME( 0x0c, 0x0c, DEF_STR( Coin_B ) );     PORT_DIPLOCATION("SW1:3,4");
            PORT_DIPSETTING(    0x00, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x0c, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x08, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x04, DEF_STR( _1C_3C ) );
            PORT_DIPNAME( 0x10, 0x10, "Leave Off" );           PORT_DIPLOCATION("SW1:5"); // Must be OFF. No test mode in ROM
            PORT_DIPSETTING(    0x10, DEF_STR( Off ) );                                  // so this locks up the game at boot-up if on
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPUNUSED_DIPLOC( 0x20, IP_ACTIVE_LOW, "SW1:6" );
            PORT_DIPNAME( 0x40, 0x00, DEF_STR( Cabinet ) );    PORT_DIPLOCATION("SW1:7");
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x40, DEF_STR( Cocktail ) );
//            PORT_DIPNAME( 0x80, 0x00, "Screen" )              PORT_DIPLOCATION("SW1:8") // Manual states this is Screen Invert
//            PORT_DIPSETTING(    0x00, "Normal" )
//            PORT_DIPSETTING(    0x80, "Invert" )
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_CUSTOM  ); PORT_VBLANK("screen");  // Schematics show this is connected to DIP SW2.8

            PORT_START("DSW2"); // At location 14D on sound PCB
            PORT_DIPNAME( 0x01, 0x01, DEF_STR( Lives ) );      PORT_DIPLOCATION("SW2:1");
            PORT_DIPSETTING(    0x01, "3" );
            PORT_DIPSETTING(    0x00, "5" );
            PORT_DIPNAME( 0x06, 0x02, DEF_STR( Bonus_Life ) ); PORT_DIPLOCATION("SW2:2,3");
            PORT_DIPSETTING(    0x06, "10000" );
            PORT_DIPSETTING(    0x04, "15000" );
            PORT_DIPSETTING(    0x02, "20000"  );
            PORT_DIPSETTING(    0x00, "30000"  );
            PORT_DIPNAME( 0x08, 0x08, "Enemies" );             PORT_DIPLOCATION("SW2:4");
            PORT_DIPSETTING(    0x08, "4" );
            PORT_DIPSETTING(    0x00, "6" );
            PORT_DIPNAME( 0x10, 0x00, "End of Level Pepper" ); PORT_DIPLOCATION("SW2:5");
            PORT_DIPSETTING(    0x10, DEF_STR( No ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Yes ) );
            PORT_DIPUNUSED_DIPLOC( 0x20, 0x20, "SW2:6" );  // should be OFF according to the manual
            PORT_DIPUNUSED_DIPLOC( 0x40, 0x40, "SW2:7" );  // should be OFF according to the manual
            PORT_DIPUNUSED_DIPLOC( 0x80, 0x80, "SW2:8" );  // should be OFF according to the manual

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( btime3 ) // Used for btime3 and btimem

        //static INPUT_PORTS_START( cookrace )

        //static INPUT_PORTS_START( zoar )

        //static INPUT_PORTS_START( lnc )

        //static INPUT_PORTS_START( wtennis )

        //static INPUT_PORTS_START( mmonkey )

        //static INPUT_PORTS_START( bnj )

        //static INPUT_PORTS_START( brubber ) // no test mode for brubber

        //static INPUT_PORTS_START( caractn2 ) // Lives DIP changes in this set

        //static INPUT_PORTS_START( disco )

        //static INPUT_PORTS_START( protenn )

        //static INPUT_PORTS_START( sdtennis )
    }


    partial class btime_state : driver_device
    {
        static readonly gfx_layout tile8layout = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,3),
            3,
            new u32[] { RGN_FRAC(2,3), RGN_FRAC(1,3), RGN_FRAC(0,3) },
            STEP8(0,1),
            STEP8(0,8),
            8*8
        );


        //static const gfx_layout disco_tile8layout =


        static readonly gfx_layout tile16layout = new gfx_layout
        (
            16,16,
            RGN_FRAC(1,3),
            3,
            new u32[] { RGN_FRAC(2,3), RGN_FRAC(1,3), RGN_FRAC(0,3) },
            ArrayCombineUInt32(STEP8(16*8,1), STEP8(0,1)),
            STEP16(0,8),
            32*8
        );


        //static const gfx_layout disco_tile16layout =

        //static const gfx_layout bnj_tile16layout =


        //static GFXDECODE_START( gfx_btime )
        static readonly gfx_decode_entry [] gfx_btime =
        {
            GFXDECODE_ENTRY( "gfx1", 0, tile8layout,     0, 1 ), /* char set #1 */
            GFXDECODE_ENTRY( "gfx1", 0, tile16layout,    0, 1 ), /* sprites */
            GFXDECODE_ENTRY( "gfx2", 0, tile16layout,    8, 1 ), /* background tiles */

            //GFXDECODE_END
        };


        //static GFXDECODE_START( gfx_cookrace )

        //static GFXDECODE_START( gfx_lnc )

        //static GFXDECODE_START( gfx_bnj )

        //static GFXDECODE_START( gfx_zoar )

        //static GFXDECODE_START( gfx_disco )

        /***************************************************************************
          Discrete Filtering and Mixing

          All values taken from Burger Time Schematics.

         ****************************************************************************/

        static readonly discrete_mixer_desc btime_sound_mixer_desc = new discrete_mixer_desc
            (DISC_MIXER_IS_OP_AMP,
                new double [] {RES_K(100), RES_K(100)},
                new int [] {0,0},  /* no variable resistors   */
                new double [] {0,0},  /* no node capacitors      */
                0,      /* no RI */
                RES_K(10),
                CAP_P(150),
                0,      /* Modelled separately */
                0, 1);


        /* R49 has 4.7k in schematics, but listed as 47k in bill of material
         * 47k gives proper low pass filtering
         *
         * Anoid measured R49 to R52 on a Burger Time pcb. These are
         * listed below
         */
        static readonly double BTIME_R49   = RES_K(47);   /* pcb: 47.4k */

        /* The input divider R51 R50 is not independent of R52, which
         * also depends on ay internal resistance.
         * FIXME: Develop proper model when I am retired.
         *
         * With R51 being 1K, the gain is way to high (23.5). Therefore R51
         * is set to 5k, but this is a hack. With the modification,
         * sound levels are in line with observations.
         * R51,R50,R52 and R49 verified on real pcb by Anoid.
         *
         * http://www.coinopvideogames.com/videogames01.php
         * There are two recordings from 1982 where the filtered sound is way louder
         * than the music. There is a later recording
         * http://www.coinopvideogames.com/videogames03.php
         * in which the filtered sounds have volumes closer to the music.
         *
         */

        static readonly double BTIME_R52   = RES_K(1);    /* pcb: .912k = 1K || 11k */
        static readonly double BTIME_R51   = RES_K(5);    /* pcb: .923k = 1k || 11k schematics 1k */
        static readonly double BTIME_R50   = RES_K(10);   /* pcb: 1.667k = 10k || 2k */


        static readonly discrete_op_amp_filt_info btime_opamp_desc = new discrete_op_amp_filt_info
            (BTIME_R51, 0, BTIME_R50, 0, BTIME_R49, CAP_U(0.068), CAP_U(0.068), 0, 0, 5.0, -5.0);


        //static DISCRETE_SOUND_START( btime_sound_discrete )
        static readonly discrete_block [] btime_sound_discrete = 
        {
            DISCRETE_INPUTX_STREAM(NODE_01, 0, 5.0/32767.0, 0),
            DISCRETE_INPUTX_STREAM(NODE_02, 1, 5.0/32767.0, 0),
            DISCRETE_INPUTX_STREAM(NODE_03, 2, 5.0/32767.0, 0),

            DISCRETE_INPUTX_STREAM(NODE_04, 3, 5.0/32767.0, 0),
            DISCRETE_INPUTX_STREAM(NODE_05, 4, 5.0/32767.0, 0),
            DISCRETE_INPUTX_STREAM(NODE_06, 5, 5.0/32767.0, 0),

            /* Mix 5 channels 1A, 1B, 1C, 2B, 2C directly */
            DISCRETE_ADDER3(NODE_20, 1, NODE_01, NODE_02, NODE_03),
            DISCRETE_ADDER3(NODE_21, 1, NODE_20, NODE_05, NODE_06),
            DISCRETE_MULTIPLY(NODE_22, NODE_21, 0.2),

            /* Filter of channel 2A */
            DISCRETE_OP_AMP_FILTER(NODE_30, 1, NODE_04, NODE_NC, DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, btime_opamp_desc),

            DISCRETE_MIXER2(NODE_40, 1, NODE_22, NODE_30, btime_sound_mixer_desc),
            DISCRETE_CRFILTER(NODE_41, NODE_40, RES_K(10), CAP_U(10)),

            /* Amplifier is upc1181H3
             *
             * http://www.ic-ts-histo.de/fad/ics/upc1181/upc1181.htm
             *
             * A linear frequency response is mentioned as well as a lower
             * edge frequency determined by cap on pin3, however no formula given.
             *
             * not modelled here
             */

            /* Assuming a 4 Ohm impedance speaker */
            DISCRETE_CRFILTER(NODE_43, NODE_41, 3.0, CAP_U(100)),

            DISCRETE_OUTPUT(NODE_43, 32767.0 / 5.0 * 35.0),

            DISCRETE_SOUND_END,
        };


        //MACHINE_START_MEMBER(btime_state,btime)
        void machine_start_btime()
        {
            save_item(NAME(new { m_btime_palette }));
            save_item(NAME(new { m_bnj_scroll1 }));
            save_item(NAME(new { m_bnj_scroll2 }));
            save_item(NAME(new { m_btime_tilemap }));
        }


        //MACHINE_START_MEMBER(btime_state,mmonkey)


        //MACHINE_RESET_MEMBER(btime_state,btime)
        void machine_reset_btime()
        {
            /* by default, the audio NMI is disabled, except for bootlegs which don't use the enable */
            if (m_audionmi.found())
                m_audionmi.op0.in_w<u32_const_0>(0);

            m_btime_palette = 0;
            m_bnj_scroll1 = 0;
            m_bnj_scroll2 = 0;
            m_btime_tilemap[0] = 0;
            m_btime_tilemap[1] = 0;
            m_btime_tilemap[2] = 0;
            m_btime_tilemap[3] = 0;
        }


        //MACHINE_RESET_MEMBER(btime_state,lnc)

        //MACHINE_RESET_MEMBER(btime_state,mmonkey)


        public void btime(machine_config config)
        {
            /* basic machine hardware */
            DECO_CPU7(config, m_maincpu, HCLK2);   /* selectable between H2/H4 via jumper */
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, btime_map);

            M6502(config, m_audiocpu, HCLK1 / 3 / 2);
            m_audiocpu.op0.memory().set_addrmap(AS_PROGRAM, audio_map);
            TIMER(config, "8vck").configure_scanline(audio_nmi_gen, "screen", 0, 8);

            INPUT_MERGER_ALL_HIGH(config, "audionmi").output_handler().set_inputline(m_audiocpu, INPUT_LINE_NMI).reg();

            /* video hardware */
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_raw(HCLK, 384, 8, 248, 272, 8, 248);
            m_screen.op0.set_screen_update(screen_update_btime);
            m_screen.op0.set_palette(m_palette);

            MCFG_MACHINE_START_OVERRIDE(config, machine_start_btime);
            MCFG_MACHINE_RESET_OVERRIDE(config, machine_reset_btime);

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_btime);
            PALETTE(config, m_palette, btime_palette).set_format(palette_device.bgr_233_inv_t.BGR_233_inverted, 16);

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            GENERIC_LATCH_8(config, m_soundlatch);
            m_soundlatch.op0.data_pending_callback().set_inputline(m_audiocpu, 0).reg();

            ay8910_device ay1 = AY8910(config, "ay1", HCLK2);
            ay1.set_flags(AY8910_DISCRETE_OUTPUT);
            ay1.set_resistors_load((int)RES_K(5), (int)RES_K(5), (int)RES_K(5));
            ay1.port_a_write_callback().set(ay_audio_nmi_enable_w).reg();
            ay1.add_route(0, "discrete", 1.0, 0);
            ay1.add_route(1, "discrete", 1.0, 1);
            ay1.add_route(2, "discrete", 1.0, 2);

            ay8910_device ay2 = AY8910(config, "ay2", HCLK2);
            ay2.set_flags(AY8910_DISCRETE_OUTPUT);
            ay2.set_resistors_load((int)RES_K(1), (int)RES_K(5), (int)RES_K(5));
            ay2.add_route(0, "discrete", 1.0, 3);
            ay2.add_route(1, "discrete", 1.0, 4);
            ay2.add_route(2, "discrete", 1.0, 5);

            DISCRETE(config, "discrete", btime_sound_discrete).disound.add_route(ALL_OUTPUTS, "mono", 1.0);
        }


        //void btime_state::cookrace(machine_config &config)

        //void btime_state::lnc(machine_config &config)

        //void btime_state::wtennis(machine_config &config)

        //void btime_state::mmonkey(machine_config &config)

        //void btime_state::bnj(machine_config &config)

        //void btime_state::sdtennis(machine_config &config)

        //void btime_state::zoar(machine_config &config)

        //void btime_state::disco(machine_config &config)

        //void btime_state::protenn(machine_config &config)

        //void btime_state::tisland(machine_config &config)
    }


    partial class btime : construct_ioport_helper
    {
        /***************************************************************************

            Game driver(s)

        ***************************************************************************/
        //ROM_START( btime )
        static readonly tiny_rom_entry [] rom_btime =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "aa04.9b",      0xc000, 0x1000, CRC("368a25b5") + SHA1("ed3f3712423979dcb351941fa85dce6a0a7bb16b") ),
            ROM_LOAD( "aa06.13b",     0xd000, 0x1000, CRC("b4ba400d") + SHA1("8c77397e934907bc47a739f263196a0f2f81ba3d") ),
            ROM_LOAD( "aa05.10b",     0xe000, 0x1000, CRC("8005bffa") + SHA1("d0da4e360039f6a8d8142a4e8e05c1f90c0af68a") ),
            ROM_LOAD( "aa07.15b",     0xf000, 0x1000, CRC("086440ad") + SHA1("4a32bc92f8ff5fbe112f56e62d2c03da8851a7b9") ),

            ROM_REGION( 0x10000, "audiocpu", 0 ),
            ROM_LOAD( "ab14.12h",     0xe000, 0x1000, CRC("f55e5211") + SHA1("27940026d0c6212d1138d2fd88880df697218627") ),

            ROM_REGION( 0x6000, "gfx1", 0 ),
            ROM_LOAD( "aa12.7k",      0x0000, 0x1000, CRC("c4617243") + SHA1("24204d591aa2c264a852ee9ba8c4be63efd97728") ),    /* charset #1 */
            ROM_LOAD( "ab13.9k",      0x1000, 0x1000, CRC("ac01042f") + SHA1("e64b6381a9298eaf74e79fa5f1ea8e9596c58a49") ),
            ROM_LOAD( "ab10.10k",     0x2000, 0x1000, CRC("854a872a") + SHA1("3d2ecfd54a5a9d68b53cf4b4ee1f2daa6aef2123") ),
            ROM_LOAD( "ab11.12k",     0x3000, 0x1000, CRC("d4848014") + SHA1("0a55b091cd4e7f317c35defe13d5051b26042eee") ),
            ROM_LOAD( "aa8.13k",      0x4000, 0x1000, CRC("8650c788") + SHA1("d9b1ee2d1f2fd66705d497c80252861b49aa9254") ),
            ROM_LOAD( "ab9.15k",      0x5000, 0x1000, CRC("8dec15e6") + SHA1("b72633de6268ce16742bba4dcba835df860d6c2f") ),

            ROM_REGION( 0x1800, "gfx2", 0 ),
            ROM_LOAD( "ab00.1b",      0x0000, 0x0800, CRC("c7a14485") + SHA1("6a0a8e6b7860859f22daa33634e34fbf91387659") ),    /* charset #2 */
            ROM_LOAD( "ab01.3b",      0x0800, 0x0800, CRC("25b49078") + SHA1("4abdcbd4f3362c3e4463a1274731289f1a72d2e6") ),
            ROM_LOAD( "ab02.4b",      0x1000, 0x0800, CRC("b8ef56c3") + SHA1("4a03bf011dc1fb2902f42587b1174b880cf06df1") ),

            ROM_REGION( 0x0800, "bg_map", 0 ),   /* background tilemaps */
            ROM_LOAD( "ab03.6b",      0x0000, 0x0800, CRC("d26bc1f3") + SHA1("737af6e264183a1f151f277a07cf250d6abb3fd8") ),

            ROM_END,
        };


        //ROM_START( btime2 )

        //ROM_START( btime3 )

        //ROM_START( btimem )

        //ROM_START( cookrace )

        //ROM_START( tisland )

        //ROM_START( lnc )

        //ROM_START( protenn )

        //ROM_START( protennb )

        //ROM_START( wtennis )

        //ROM_START( mmonkey )

        //ROM_START( mmonkeyj )

        //ROM_START( brubber )

        //ROM_START( bnj )

        //ROM_START( bnjm )

        //ROM_START( caractn )

        //ROM_START( caractn2 )

        //ROM_START( zoar )

        //ROM_START( disco )

        //ROM_START( discof )

        //ROM_START( sdtennis )
    }


    partial class btime_state : driver_device
    {
        //uint8_t btime_state::wtennis_reset_hack_r()


        public void init_btime()
        {
            m_audio_nmi_enable_type = AUDIO_ENABLE_DIRECT;
        }


        //void btime_state::init_zoar()

        //void btime_state::init_tisland()

        //void btime_state::init_lnc()

        //void btime_state::init_bnj()

        //void btime_state::init_disco()

        //void btime_state::init_cookrace()

        //void btime_state::init_protennb()

        //void btime_state::init_wtennis()

        //void btime_state::init_sdtennis()
    }


    partial class btime : construct_ioport_helper
    {
        static void btime_state_btime(machine_config config, device_t device) { btime_state btime_state = (btime_state)device; btime_state.btime(config); }
        static void btime_state_init_btime(device_t owner) { btime_state btime_state = (btime_state)owner; btime_state.init_btime(); }

        static btime m_btime = new btime();

        static device_t device_creator_btime(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new btime_state(mconfig, type, tag); }

        public static readonly game_driver driver_btime = GAME(device_creator_btime, rom_btime, "1982", "btime", "0", btime_state_btime, m_btime.construct_ioport_btime, btime_state_init_btime, ROT270, "Data East Corporation", "Burger Time (Data East set 1)", MACHINE_SUPPORTS_SAVE);
    }
}
