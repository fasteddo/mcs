// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u32 = System.UInt32;

using static mame.digfx_global;
using static mame.disound_global;
using static mame.drawgfx_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.generic_global;
using static mame.gottlieb_global;
using static mame.hash_global;
using static mame.i86_global;
using static mame.nvram_global;
using static mame.romentry_global;
using static mame.samples_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.watchdog_global;


namespace mame
{
    partial class gottlieb_state : driver_device
    {
        //#define LOG_AUDIO_DECODE    (0)

        static readonly XTAL SYSTEM_CLOCK        = new XTAL(20_000_000);
        static readonly XTAL CPU_CLOCK           = new XTAL(15_000_000);
        //#define NTSC_CLOCK          XTAL(14'318'181)
        //#define LASERDISC_CLOCK     PERIOD_OF_555_ASTABLE(16000, 10000, 0.001e-6)

        //#define AUDIORAM_SIZE       0x400


        /*************************************
         *  Initialization
         *************************************/
#if false
        void gottlieb_state::machine_start()
        {
            m_leds.resolve();
            m_knockers.resolve();
            /* register for save states */
            save_item(NAME(m_joystick_select));
            save_item(NAME(m_track));
            save_item(NAME(m_knocker_prev));
            save_item(NAME(m_gfxcharlo));
            save_item(NAME(m_gfxcharhi));
            save_item(NAME(m_weights));

            /* see if we have a laserdisc */
            if (m_laserdisc != nullptr)
            {
                /* attach to the I/O ports */
                m_maincpu->space(AS_PROGRAM).install_read_handler(0x05805, 0x05807, 0, 0x07f8, 0, read8sm_delegate(*this, FUNC(gottlieb_state::laserdisc_status_r)));
                m_maincpu->space(AS_PROGRAM).install_write_handler(0x05805, 0x05805, 0, 0x07f8, 0, write8smo_delegate(*this, FUNC(gottlieb_state::laserdisc_command_w)));    /* command for the player */
                m_maincpu->space(AS_PROGRAM).install_write_handler(0x05806, 0x05806, 0, 0x07f8, 0, write8smo_delegate(*this, FUNC(gottlieb_state::laserdisc_select_w)));

                /* allocate a timer for serial transmission, and one for philips code processing */
                m_laserdisc_bit_timer = timer_alloc(TIMER_LASERDISC_BIT);
                m_laserdisc_philips_timer = timer_alloc(TIMER_LASERDISC_PHILIPS);

                /* create some audio RAM */
                m_laserdisc_audio_buffer = std::make_unique<u8[]>(AUDIORAM_SIZE);
                m_laserdisc_status = 0x38;

                /* more save state registration */
                save_item(NAME(m_laserdisc_select));
                save_item(NAME(m_laserdisc_status));
                save_item(NAME(m_laserdisc_philips_code));

                save_pointer(NAME(m_laserdisc_audio_buffer), AUDIORAM_SIZE);
                save_item(NAME(m_laserdisc_audio_address));
                save_item(NAME(m_laserdisc_last_samples));
                save_item(NAME(m_laserdisc_last_time));
                save_item(NAME(m_laserdisc_last_clock));
                save_item(NAME(m_laserdisc_zero_seen));
                save_item(NAME(m_laserdisc_audio_bits));
                save_item(NAME(m_laserdisc_audio_bit_count));
            }
        }


        void gottlieb_state::machine_reset()
        {
            /* if we have a laserdisc, reset our philips code callback for the next line 17 */
            if (m_laserdisc != nullptr)
                m_laserdisc_philips_timer->adjust(m_screen->time_until_pos(17), 17);
        }


        /*************************************
         *  Input ports
         *************************************/

        template <int N>
        CUSTOM_INPUT_MEMBER(gottlieb_state::track_delta_r)
        {
            return (N ? m_track_y : m_track_x)->read() - m_track[N];
        }


        void gottlieb_state::analog_reset_w(u8 data)
        {
            /* reset the trackball counters */
            m_track[0] = m_track_x.read_safe(0);
            m_track[1] = m_track_y.read_safe(0);
        }


        //CUSTOM_INPUT_MEMBER(gottlieb_state::stooges_joystick_r)


        /*************************************
         *  Output ports
         *************************************/
        void gottlieb_state::general_output_w(u8 data)
        {
            /* bits 0-3 control video features, and are different for laserdisc games */
            if (m_laserdisc == nullptr)
                video_control_w(data);
            else
                laserdisc_video_control_w(data);

            /* bit 4 normally controls the coin meter */
            machine().bookkeeping().coin_counter_w(0, data & 0x10);

            /* bit 5 doesn't have a generic function */
            /* bit 6 controls "COIN1"; it appears that no games used this */
            /* bit 7 controls the optional coin lockout; it appears that no games used this */
        }


        // custom overrides
        //void gottlieb_state::reactor_output_w(u8 data)


        void gottlieb_state::qbert_output_w(u8 data)
        {
            general_output_w(data & ~0x20);

            // bit 5 controls the knocker
            qbert_knocker(BIT(data, 5));
        }
#endif


        //void gottlieb_state::qbertqub_output_w(u8 data)

        //void gottlieb_state::stooges_output_w(u8 data)


        /*************************************
         *  Laserdisc I/O interface
         *************************************/
        //uint8_t gottlieb_state::laserdisc_status_r(offs_t offset)

        //void gottlieb_state::laserdisc_select_w(u8 data)

        //void gottlieb_state::laserdisc_command_w(u8 data)

        /*************************************
         *  Laserdisc command/status interfacing
         *************************************/
        //TIMER_CALLBACK_MEMBER(gottlieb_state::laserdisc_philips_callback)

        //TIMER_CALLBACK_MEMBER(gottlieb_state::laserdisc_bit_off_callback)

        //TIMER_CALLBACK_MEMBER(gottlieb_state::laserdisc_bit_callback)

        /*************************************
         *  Laserdisc sound channel
         *************************************/
        //inline void gottlieb_state::audio_end_state()

        //void gottlieb_state::audio_process_clock(bool logit)

        //void gottlieb_state::audio_handle_zero_crossing(const attotime &zerotime, bool logit)

        //void gottlieb_state::laserdisc_audio_process(int samplerate, int samples, const int16_t *ch0, const int16_t *ch1)


        //**************************************************************************
        //  QBERT MECHANICAL KNOCKER
        //**************************************************************************
        //-------------------------------------------------
        //  qbert cabinets have a mechanical knocker near the floor,
        //  MAME simulates this with a sample.
        //  (like all MAME samples, it is optional. If you actually have
        //   a real kicker/knocker, hook it up via output "knocker0")
        //-------------------------------------------------
#if false
        void gottlieb_state::qbert_knocker(u8 knock)
        {
            //output().set_value("knocker0", knock);
            m_knockers[0] = knock ? 1 : 0;

            // start sound on rising edge
            if (knock & ~m_knocker_prev)
                m_knocker_sample->start(0, 0);
            m_knocker_prev = knock;
        }
#endif


        static readonly string [] qbert_knocker_names =
        {
            "*qbert",
            "knocker",
            null   /* end of array */
        };


        void qbert_knocker(machine_config config)
        {
            SPEAKER(config, "knocker", 0.0, 0.0, 1.0);

            SAMPLES(config, m_knocker_sample);
            m_knocker_sample.op0.set_channels(1);
            m_knocker_sample.op0.set_samples_names(qbert_knocker_names);
            m_knocker_sample.op0.disound.add_route(ALL_OUTPUTS, "knocker", 1.0);
        }


        /*************************************
        *  Interrupt generation
        *************************************/
#if false
        void gottlieb_state::device_timer(emu_timer &timer, device_timer_id id, int param)
        {
            switch (id)
            {
            case TIMER_LASERDISC_PHILIPS:
                laserdisc_philips_callback(param);
                break;
            case TIMER_LASERDISC_BIT_OFF:
                laserdisc_bit_off_callback(param);
                break;
            case TIMER_LASERDISC_BIT:
                laserdisc_bit_callback(param);
                break;
            case TIMER_NMI_CLEAR:
                nmi_clear(param);
                break;
            default:
                throw emu_fatalerror("Unknown id in gottlieb_state::device_timer");
            }
        }

        TIMER_CALLBACK_MEMBER(gottlieb_state::nmi_clear)
        {
            m_maincpu->set_input_line(INPUT_LINE_NMI, CLEAR_LINE);
        }
#endif


        //INTERRUPT_GEN_MEMBER(gottlieb_state::interrupt)
        void interrupt(device_t device)
        {
            throw new emu_unimplemented();
#if false
            /* assert the NMI and set a timer to clear it at the first visible line */
            device.execute().set_input_line(INPUT_LINE_NMI, ASSERT_LINE);
            timer_set(m_screen->time_until_pos(0), TIMER_NMI_CLEAR);

            /* if we have a laserdisc, update it */
            if (m_laserdisc != nullptr)
            {
                /* set the "disc ready" bit, which basically indicates whether or not we have a proper video frame */
                if (!m_laserdisc->video_active())
                    m_laserdisc_status &= ~0x20;
                else
                    m_laserdisc_status |= 0x20;
            }
#endif
        }


        /*************************************
         *  Main CPU memory handlers
         *************************************/
#if false
        void gottlieb_state::sound_w(u8 data)
        {
            if (m_r1_sound != nullptr)
                m_r1_sound->write(data);
            if (m_r2_sound != nullptr)
                m_r2_sound->write(data);
        }
#endif


        //void gottlieb_state::reactor_map(address_map &map)


        void gottlieb_base_map(address_map map)
        {
            throw new emu_unimplemented();
#if false
            map.global_mask(0xffff);
            map(0x0000, 0x0fff).ram().share("nvram");
            map(0x3000, 0x30ff).mirror(0x0700).writeonly().share("spriteram");                           /* FRSEL */
            map(0x3800, 0x3bff).mirror(0x0400).ram().w(FUNC(gottlieb_state::videoram_w)).share("videoram");       /* BRSEL */
            map(0x4000, 0x4fff).ram().w(FUNC(gottlieb_state::charram_w)).share("charram");               /* BOJRSEL1 */
            map(0x5000, 0x501f).mirror(0x07e0).w(FUNC(gottlieb_state::palette_w)).share("paletteram");       /* COLSEL */
            map(0x5800, 0x5800).mirror(0x07f8).w("watchdog", FUNC(watchdog_timer_device::reset_w));
            map(0x5801, 0x5801).mirror(0x07f8).w(FUNC(gottlieb_state::analog_reset_w));                        /* A1J2 interface */
            map(0x5802, 0x5802).mirror(0x07f8).w(FUNC(gottlieb_state::sound_w));                                  /* OP20-27 */
            map(0x5803, 0x5803).mirror(0x07f8).w(FUNC(gottlieb_state::general_output_w));                               /* OP30-37 */
        //  map(0x5804, 0x5804).mirror(0x07f8).w(FUNC(gottlieb_state::));                                            /* OP40-47 */
            map(0x5800, 0x5800).mirror(0x07f8).portr("DSW");
            map(0x5801, 0x5801).mirror(0x07f8).portr("IN1");                                      /* IP10-17 */
            map(0x5802, 0x5802).mirror(0x07f8).portr("IN2");                                      /* trackball H */
            map(0x5803, 0x5803).mirror(0x07f8).portr("IN3");                                      /* trackball V */
            map(0x5804, 0x5804).mirror(0x07f8).portr("IN4");                                      /* IP40-47 */
            map(0x6000, 0xffff).rom();
#endif
        }


        void gottlieb_ram_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            gottlieb_base_map(map);

            map(0x1000, 0x2fff).ram();
#endif
        }


        //void gottlieb_state::gottlieb_ram_rom_map(address_map &map)

        //void gottlieb_state::gottlieb_rom_map(address_map &map)
    }


    partial class gottlieb : construct_ioport_helper
    {
        /*************************************
         *  Port definitions
         *************************************/

        //static INPUT_PORTS_START( reactor )


        //static INPUT_PORTS_START( qbert )
        void construct_ioport_qbert(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            throw new emu_unimplemented();
#if false
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("DSW")
            PORT_DIPNAME( 0x01, 0x00, DEF_STR( Demo_Sounds ) )      PORT_DIPLOCATION("DSW:!2")
            PORT_DIPSETTING(    0x01, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x00, DEF_STR( On ) )
            PORT_DIPNAME( 0x02, 0x02, "Kicker" )                PORT_DIPLOCATION("DSW:!6")
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x02, DEF_STR( On ) )
            PORT_DIPNAME( 0x04, 0x00, DEF_STR( Cabinet ) )          PORT_DIPLOCATION("DSW:!4")
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) )
            PORT_DIPSETTING(    0x04, DEF_STR( Cocktail ) )
            PORT_DIPNAME( 0x08, 0x00, "Demo Mode (Unlim Lives, Start=Adv (Cheat)")  PORT_DIPLOCATION("DSW:!1")
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x08, DEF_STR( On ) )
            PORT_DIPNAME( 0x10, 0x00, DEF_STR( Free_Play ) )        PORT_DIPLOCATION("DSW:!3")
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x10, DEF_STR( On ) )
            PORT_DIPUNUSED_DIPLOC( 0x20, 0x00, "DSW:!5" )
            PORT_DIPUNUSED_DIPLOC( 0x40, 0x00, "DSW:!7" )
            PORT_DIPUNUSED_DIPLOC( 0x80, 0x00, "DSW:!8" )
            /* 0x40 must be connected to the IP16 line */

            PORT_START("IN1")
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_START1 )
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_START2 )
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_COIN1 )
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_COIN2 )
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_UNKNOWN )
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_UNKNOWN )
            PORT_SERVICE( 0x40, IP_ACTIVE_LOW )
            PORT_BIT(0x80, IP_ACTIVE_HIGH, IPT_SERVICE ) PORT_NAME("Select in Service Mode") PORT_CODE(KEYCODE_F1)

            PORT_START("IN2")   /* trackball H not used */
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNUSED )

            PORT_START("IN3")   /* trackball V not used */
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNUSED )

            PORT_START("IN4")      /* joystick - actually 4-Way but assigned as 8-Way to allow diagonal mapping */
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ) PORT_8WAY
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ) PORT_8WAY
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ) PORT_8WAY
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ) PORT_8WAY
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ) PORT_8WAY PORT_COCKTAIL
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ) PORT_8WAY PORT_COCKTAIL
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ) PORT_8WAY PORT_COCKTAIL
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ) PORT_8WAY PORT_COCKTAIL

            INPUT_PORTS_END
#endif
        }


        //static INPUT_PORTS_START( insector )

        //static INPUT_PORTS_START( tylz )

        //static INPUT_PORTS_START( argusg )

        //static INPUT_PORTS_START( mplanets )

        //static INPUT_PORTS_START( krull )

        //static INPUT_PORTS_START( kngtmare )

        //static INPUT_PORTS_START( qbertqub )

        //static INPUT_PORTS_START( curvebal )

        //static INPUT_PORTS_START( screwloo )

        //static INPUT_PORTS_START( mach3 )

        //static INPUT_PORTS_START( cobram3 )

        //static INPUT_PORTS_START( usvsthem )

        //static INPUT_PORTS_START( 3stooges )

        //static INPUT_PORTS_START( vidvince )

        //static INPUT_PORTS_START( wizwarz )
    }


    partial class gottlieb_state : driver_device
    {
        /*************************************
         *  Graphics definitions
         *************************************/

        /* the games can store char gfx data in either a 4k RAM area (128 chars), or */
        /* a 8k ROM area (256 chars). */

        static readonly gfx_layout fg_layout = new gfx_layout
        (
            16,16,
            RGN_FRAC(1,4),
            4,
            new u32[] { RGN_FRAC(0,4), RGN_FRAC(1,4), RGN_FRAC(2,4), RGN_FRAC(3,4) },
            STEP16(0,1),
            STEP16(0,16),
            32*8
        );


        //static GFXDECODE_START( gfxdecode )
        static readonly gfx_decode_entry [] gfxdecode =
        {
            GFXDECODE_RAM(   "charram", 0, gfx_8x8x4_packed_msb, 0, 1 ),   /* the game dynamically modifies this */
            GFXDECODE_ENTRY( "bgtiles", 0, gfx_8x8x4_packed_msb, 0, 1 ),
            GFXDECODE_ENTRY( "sprites", 0, fg_layout,            0, 1 ),

            //GFXDECODE_END
        };


        /*************************************
         *  Core machine drivers
         *************************************/
        void gottlieb_core(machine_config config)
        {
            /* basic machine hardware */
            I8088(config, m_maincpu, CPU_CLOCK / 3);
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, gottlieb_ram_map);
            m_maincpu.op0.execute().set_vblank_int("screen", interrupt);

            NVRAM(config, "nvram", nvram_device.default_value.DEFAULT_ALL_1);

            WATCHDOG_TIMER(config, "watchdog").set_vblank_count(m_screen, 16);

            /* video hardware */
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_raw(SYSTEM_CLOCK / 4, GOTTLIEB_VIDEO_HCOUNT, 0, GOTTLIEB_VIDEO_HBLANK, GOTTLIEB_VIDEO_VCOUNT, 0, GOTTLIEB_VIDEO_VBLANK);
            m_screen.op0.set_screen_update(screen_update);

            GFXDECODE(config, m_gfxdecode, m_palette, gfxdecode);
            PALETTE(config, m_palette).set_entries(16);

            // basic speaker configuration
            SPEAKER(config, "speaker").front_center();
        }


        //void gottlieb_state::gottlieb1(machine_config &config)

        //void gottlieb_state::gottlieb1_rom(machine_config &config)

        //void gottlieb_state::gottlieb2(machine_config &config)

        //void gottlieb_state::gottlieb2_ram_rom(machine_config &config)

        //void gottlieb_state::g2laser(machine_config &config)


        /*************************************
         *  Specific machine drivers
         *************************************/
        void gottlieb1_votrax(machine_config config)
        {
            gottlieb_core(config);
            GOTTLIEB_SOUND_REV1_VOTRAX(config, m_r1_sound, 0).dimixer.add_route(ALL_OUTPUTS, "speaker", 1.0);
        }


        //void gottlieb_state::reactor(machine_config &config)


        public void qbert(machine_config config)
        {
            gottlieb1_votrax(config);

            /* sound hardware */
            qbert_knocker(config);
        }


        //void gottlieb_state::tylz(machine_config &config)

        //void gottlieb_state::screwloo(machine_config &config)

        //void gottlieb_state::cobram3(machine_config &config)
    }


    partial class gottlieb : construct_ioport_helper
    {
        /*************************************
         *  ROM definitions
         *************************************/

        //ROM_START( reactor )


        //ROM_START( qbert )
        static readonly tiny_rom_entry [] rom_qbert =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "qb-rom2.bin",  0xa000, 0x2000, CRC("fe434526") + SHA1("4cfc5d52dd6c82163e035af82d6112c0c93a3797") ),
            ROM_LOAD( "qb-rom1.bin",  0xc000, 0x2000, CRC("55635447") + SHA1("ca6acdef1c9e06b33efe1f0a2df2dfb03723cfbe") ),
            ROM_LOAD( "qb-rom0.bin",  0xe000, 0x2000, CRC("8e318641") + SHA1("7f8f66d1e6a7905e93cce07fc92e8801370b7194") ),

            ROM_REGION( 0x10000, "r1sound:audiocpu", 0 ),
            ROM_LOAD( "qb-snd1.bin",  0x7000, 0x800, CRC("15787c07") + SHA1("8b7d03fbf2ebaa71b3a7e2f636a0d1bb9b796e43") ),
            ROM_LOAD( "qb-snd2.bin",  0x7800, 0x800, CRC("58437508") + SHA1("09d8053e7e99679b602dcda230d64db7fe6cb7f5") ),
            // also found as 1 bigger single ROM: CRC(ebcedba9) SHA1(94aee8e32bdc80bbc5dc1423ca97597bdb9d808c) )

            ROM_REGION( 0x2000, "bgtiles", 0 ),
            ROM_LOAD( "qb-bg0.bin",   0x0000, 0x1000, CRC("7a9ba824") + SHA1("12aa6df499eb6996ee35f56acac403ff6290f844") ),
            ROM_LOAD( "qb-bg1.bin",   0x1000, 0x1000, CRC("22e5b891") + SHA1("5bb67e333255c0ea679ab4312256a8a71a950db8") ),

            ROM_REGION( 0x8000, "sprites", 0 ),
            ROM_LOAD( "qb-fg3.bin",   0x0000, 0x2000, CRC("dd436d3a") + SHA1("ae16087a6ceec84551b5d7aae4036e0ed432cbb7") ), // 0xxxxxxxxxxxx = 0xFF, also found as smaller ROM: CRC(983e3e05) SHA1(14f21543c3301b15d179b3864676e76ad5dfcaf8)
            ROM_LOAD( "qb-fg2.bin",   0x2000, 0x2000, CRC("f69b9483") + SHA1("06894a1474c79c1274efbd32d7371179e7e0a661") ), // 0xxxxxxxxxxxx = 0xFF, also found as smaller ROM: CRC(b3e6c7bc) SHA1(38e34e8712c5f677fa3fada68bc4c318e9bf7ca6)
            ROM_LOAD( "qb-fg1.bin",   0x4000, 0x2000, CRC("224e8356") + SHA1("f7f26b879aa8b964ff6311136ed8157e44de736c") ), // 0xxxxxxxxxxxx = 0xFF, also found as smaller ROM: CRC(6733d069) SHA1(3b4ac832f2475d51ae7586d3eb80e355afb64222)
            ROM_LOAD( "qb-fg0.bin",   0x6000, 0x2000, CRC("2f695b85") + SHA1("807d16459838f129e10b913890bbc95065d5dd40") ), // 0xxxxxxxxxxxx = 0x00, also found as smaller ROM: CRC(3081c200) SHA1(137d95a2a58e2ed4da7145a539d1a1942c80674c)

            ROM_END,
        };


        //ROM_START( qberta )

        //ROM_START( qbertj )

        //ROM_START( myqbert )

        //ROM_START( qberttst )

        //ROM_START( qbtrktst )

        //ROM_START( insector )

        //ROM_START( tylz )

        //ROM_START( argusg )

        //ROM_START( mplanets )

        //ROM_START( mplanetsuk )

        //ROM_START( krull )

        //ROM_START( kngtmare )

        //ROM_START( sqbert )

        //ROM_START( qbertqub )

        //ROM_START( curvebal ) /* Rom labels have the following conventions:  GV-134  ROM 3, (c)1984, Mylstar Electronics, Inc., ALL RIGHTS RSV'D   etc... */

        //ROM_START( screwloo )

        //ROM_START( mach3 )

        //ROM_START( mach3a )

        //ROM_START( mach3b )

        //ROM_START( cobram3 )

        //ROM_START( cobram3a ) // ROMS came from a blister, shows same version and date as the parent, but bh00 to bh02 differ quite a bit

        //ROM_START( usvsthem )

        //ROM_START( 3stooges )

        //ROM_START( 3stoogesa )

        //ROM_START( vidvince )

        //ROM_START( wizwarz )
    }


    partial class gottlieb_state : driver_device
    {
        /*************************************
         *  Driver initialization
         *************************************/
        //void gottlieb_state::init_ramtiles()


        void init_romtiles()
        {
            throw new emu_unimplemented();
#if false
            m_gfxcharlo = m_gfxcharhi = 1;
#endif
        }


        public void init_qbert()
        {
            throw new emu_unimplemented();
#if false
            init_romtiles();
            m_maincpu->space(AS_PROGRAM).install_write_handler(0x5803, 0x5803, 0, 0x07f8, 0, write8smo_delegate(*this, FUNC(gottlieb_state::qbert_output_w)));
#endif
        }


        //void gottlieb_state::init_qbertqub()

        //void gottlieb_state::init_stooges()

        //void gottlieb_state::init_screwloo()

        //void gottlieb_state::init_vidvince()
    }


    public partial class gottlieb : construct_ioport_helper
    {
        /*************************************
         *  Game drivers
         *************************************/

        static void gottlieb_state_qbert(machine_config config, device_t device) { gottlieb_state gottlieb_state = (gottlieb_state)device; gottlieb_state.qbert(config); }

        static void gottlieb_state_init_qbert(device_t owner) { gottlieb_state gottlieb_state = (gottlieb_state)owner; gottlieb_state.init_qbert(); }

        static gottlieb m_gottlieb = new gottlieb();

        static device_t device_creator_qbert(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new gottlieb_state(mconfig, type, tag); }

        /* games using rev 1 sound board */
        public static readonly game_driver driver_qbert = GAME(device_creator_qbert, rom_qbert, "1982", "qbert", "0", gottlieb_state_qbert, m_gottlieb.construct_ioport_qbert, gottlieb_state_init_qbert, ROT270, "Gottlieb", "Q*bert (US set 1)", MACHINE_IMPERFECT_SOUND | MACHINE_SUPPORTS_SAVE);
    }
}
