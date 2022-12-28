// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.attotime_global;
using static mame.diexec_global;
using static mame.digfx_global;
using static mame.disound_global;
using static mame.drawgfx_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.inputcode_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.midway_global;
using static mame.nvram_global;
using static mame.romentry_global;
using static mame.samples_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.timer_global;
using static mame.watchdog_global;
using static mame.z80_global;
using static mame.z80ctc_global;


namespace mame
{
    partial class mcr_state : driver_device
    {
        void mcr_control_port_w(uint8_t data)
        {
            /*
                Bit layout is as follows:
                    D7 = n/c
                    D6 = cocktail flip
                    D5 = red LED
                    D4 = green LED
                    D3 = n/c
                    D2 = coin meter 3
                    D1 = coin meter 2
                    D0 = coin meter 1
            */

            machine().bookkeeping().coin_counter_w(0, (data >> 0) & 1);
            machine().bookkeeping().coin_counter_w(1, (data >> 1) & 1);
            machine().bookkeeping().coin_counter_w(2, (data >> 2) & 1);
            m_mcr_cocktail_flip = (uint8_t)((data >> 6) & 1);
        }


        //uint8_t mcr_state::solarfox_ip0_r()

        //uint8_t mcr_state::solarfox_ip1_r()

        //uint8_t mcr_state::kick_ip1_r()

        //TIMER_DEVICE_CALLBACK_MEMBER(mcr_dpoker_state::hopper_callback)

        //TIMER_DEVICE_CALLBACK_MEMBER(mcr_dpoker_state::coin_in_callback)

        //INPUT_CHANGED_MEMBER(mcr_dpoker_state::coin_in_hit)

        //uint8_t mcr_dpoker_state::ip0_r()

        //void mcr_dpoker_state::lamps1_w(uint8_t data)

        //void mcr_dpoker_state::lamps2_w(uint8_t data)

        //void mcr_dpoker_state::output_w(uint8_t data)

        //void mcr_dpoker_state::meters_w(uint8_t data)

        //void mcr_state::wacko_op4_w(uint8_t data)

        //uint8_t mcr_state::wacko_ip1_r()

        //uint8_t mcr_state::wacko_ip2_r()

        //uint8_t mcr_state::kroozr_ip1_r()

        //void mcr_state::kroozr_op4_w(uint8_t data)

        //void mcr_state::journey_op4_w(uint8_t data)


        /*************************************
         *
         *  Two Tigers I/O ports
         *
         *************************************/
        void twotiger_op4_w(uint8_t data)
        {
            for (int i = 0; i < 2; i++)
            {
                /* play tape, and loop it */
                if (!m_samples.op0.playing((uint8_t)i))
                    m_samples.op0.start((uint8_t)i, (uint32_t)i, true);

                /* bit 1 turns cassette on/off */
                m_samples.op0.pause((uint8_t)i, (~data & 2) != 0);
            }

            // bit 2: lamp control?
            // if (data & 0xfc) printf("%x ",data);
        }


        /*************************************
         *
         *  Discs of Tron I/O ports
         *
         *************************************/
#if false
        void mcr_state::dotron_op4_w(uint8_t data)
        {
            /*
                Flasher Control:
                    A 555 timer is set up in astable mode with R1=R2=56k and C=1uF giving
                    a frequency of 8.5714 Hz. The timer is enabled if J1-3 is high (1).
                    The output of the timer is connected to the input of a D-type flip
                    flop at 1A, which is clocked by the AC sync (since this is a
                    fluorescent light fixture).

                    The J1-4 input is also connected the input of another D-type flip flop
                    on the same chip at 1A. The output of this directly controls the light
                    fixture.

                    Thus:
                        J1-3 enables a strobe effect at 8.5714 Hz (77.616ms high, 38.808ms low)
                        J1-4 directly enables/disables the lamp.
                        The two outputs are wire-ored together.
            */
            /* bit 7 = FL1 (J1-3) on flasher control board */
            /* bit 6 = FL0 (J1-4) on flasher control board */
            output().set_value("backlight", (data >> 6) & 1);

            /*
                Lamp Sequencer:
                    A 556 timer is set up in astable mode with two different frequencies,
                    one using R1=R2=10k and C=10uF giving a frequency of 4.8 Hz, and the
                    second using R1=R2=5.1k and C=10uF giving a frequency of 9.4118 Hz.

                    The outputs of these clocks go into a mux at U4, whose input is
                    selected by the input bit latched from J1-6.

                    The output of the mux clocks a 16-bit binary counter at U3. The
                    output of the binary counter becomes the low 4 address bits of the
                    82S123 PROM at U2. The upper address bit comes from the input bit
                    latched from J1-5.

                    Each of the 5 output bits from the 82S123 is inverted and connected
                    to one of the lamps. The /CE pin on the 82S123 is connected to the
                    input bit latched from J1-4.

                    Thus:
                        J1-4 enables (0) or disables (1) the lamp sequencing.
                        J1-5 selects one of two 16-entry sequences stored in the 82S123.
                        J1-6 selects one of two speeds (0=4.8 Hz, 1=9.4118 Hz)

            */
            /* bit 5 = SEL1 (J1-1) on the Lamp Sequencer board */
            if (((m_last_op4 ^ data) & 0x20) && (data & 0x20))
            {
                /* bit 2 -> J1-4 = enable */
                /* bit 1 -> J1-5 = sequence select */
                /* bit 0 -> J1-6 = speed (0=slow, 1=fast) */
                logerror("Lamp: en=%d seq=%d speed=%d\n", (data >> 2) & 1, (data >> 1) & 1, data & 1);
            }
            m_last_op4 = data;

            /* bit 4 = SEL0 (J1-8) on squawk n talk board */
            /* bits 3-0 = MD3-0 connected to squawk n talk (J1-4,3,2,1) */
            m_squawk_n_talk->sound_select(data & 0x0f);
            m_squawk_n_talk->sound_int(BIT(data, 4));
        }
#endif


        //uint8_t mcr_nflfoot_state::ip2_r()

        //void mcr_nflfoot_state::op4_w(uint8_t data)


        /*************************************
         *
         *  Demolition Derby I/O ports
         *
         *************************************/
        uint8_t demoderb_ip1_r()
        {
            throw new emu_unimplemented();
#if false
            return ioport("ssio:IP1")->read() |
                (ioport(m_input_mux ? "ssio:IP1.ALT2" : "ssio:IP1.ALT1")->read() << 2);
#endif
        }


        uint8_t demoderb_ip2_r()
        {
            throw new emu_unimplemented();
#if false
            return ioport("ssio:IP2")->read() |
                (ioport(m_input_mux ? "ssio:IP2.ALT2" : "ssio:IP2.ALT1")->read() << 2);
#endif
        }


        void demoderb_op4_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            if (data & 0x40) m_input_mux = 1;
            if (data & 0x80) m_input_mux = 0;
            m_turbo_cheap_squeak->write(data);
#endif
        }


        /*************************************
         *
         *  CPU board 90009 memory handlers
         *
         *************************************/
        /* address map verified from schematics */
        void cpu_90009_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map.unmap_value_high();
            map(0x0000, 0x6fff).rom();
            map(0x7000, 0x77ff).mirror(0x0800).ram().share("nvram");
            map(0xf000, 0xf1ff).mirror(0x0200).ram().share("spriteram");
            map(0xf400, 0xf41f).mirror(0x03e0).w(m_palette, FUNC(palette_device::write8)).share("palette");
            map(0xf800, 0xf81f).mirror(0x03e0).w(m_palette, FUNC(palette_device::write8_ext)).share("palette_ext");
            map(0xfc00, 0xffff).ram().w(FUNC(mcr_state::mcr_90009_videoram_w)).share("videoram");
#endif
        }


        /* upper I/O map determined by PAL; only SSIO ports are verified from schematics */
        void cpu_90009_portmap(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map.unmap_value_high();
            map.global_mask(0xff);
            midway_ssio_device::ssio_input_ports(map, "ssio");
            map(0xe0, 0xe0).w("watchdog", FUNC(watchdog_timer_device::reset_w));
            map(0xe8, 0xe8).nopw();
            map(0xf0, 0xf3).rw(m_ctc, FUNC(z80ctc_device::read), FUNC(z80ctc_device::write));
#endif
        }


#if false
        void mcr_state::cpu_90009_dp_map(address_map &map)
        {
            cpu_90009_map(map);
            map(0x8000, 0x81ff).ram();  // meter ram, is it battery backed?
        }

        void mcr_state::cpu_90009_dp_portmap(address_map &map)
        {
            cpu_90009_portmap(map);
            map(0x24, 0x24).portr("P24");
            map(0x28, 0x28).portr("P28");
            map(0x2c, 0x2c).portr("P2C");

            map(0x2c, 0x2c).w(FUNC(mcr_dpoker_state::lamps1_w));
            map(0x30, 0x30).w(FUNC(mcr_dpoker_state::lamps2_w));
            map(0x34, 0x34).w(FUNC(mcr_dpoker_state::output_w));
            map(0x3f, 0x3f).w(FUNC(mcr_dpoker_state::meters_w));
        }
#endif

        /*************************************
         *
         *  CPU board 90010 memory handlers
         *
         *************************************/
        /* address map verified from schematics */
        void cpu_90010_map(address_map map, device_t device)
        {
            map.unmap_value_high();
            map.op(0x0000, 0xbfff).rom();
            map.op(0xc000, 0xc7ff).mirror(0x1800).ram().share("nvram");
            map.op(0xe000, 0xe1ff).mirror(0x1600).ram().share("spriteram");
            map.op(0xe800, 0xefff).mirror(0x1000).ram().w(mcr_90010_videoram_w).share("videoram");
        }


        /* upper I/O map determined by PAL; only SSIO ports are verified from schematics */
        void cpu_90010_portmap(address_map map, device_t device)
        {
            map.unmap_value_high();
            map.global_mask(0xff);
            midway_ssio_device.ssio_input_ports(map, "ssio", this);
            map.op(0xe0, 0xe0).w("watchdog", (data) => { ((watchdog_timer_device)subdevice("watchdog")).reset_w(data); });
            map.op(0xe8, 0xe8).nopw();
            map.op(0xf0, 0xf3).rw(m_ctc, (offset) => { return m_ctc.op0.read(offset); }, (offset, data) => { m_ctc.op0.write(offset, data); });
        }


        /*************************************
         *
         *  CPU board 91490 memory handlers
         *
         *************************************/
        /* address map verified from schematics */
        void cpu_91490_map(address_map map, device_t device)
        {
            map.unmap_value_high();
            map.op(0x0000, 0xdfff).rom();
            map.op(0xe000, 0xe7ff).ram().share("nvram");
            map.op(0xe800, 0xe9ff).mirror(0x0200).ram().share("spriteram");
            map.op(0xf000, 0xf7ff).ram().w(mcr_91490_videoram_w).share("videoram");
            map.op(0xf800, 0xf87f).mirror(0x0780).w(mcr_paletteram9_w).share("paletteram");
        }


        /* upper I/O map determined by PAL; only SSIO ports are verified from schematics */
        void cpu_91490_portmap(address_map map, device_t device)
        {
            map.unmap_value_high();
            map.global_mask(0xff);
            midway_ssio_device.ssio_input_ports(map, "ssio", this);
            map.op(0xe0, 0xe0).w("watchdog", (data) => { ((watchdog_timer_device)subdevice("watchdog")).reset_w(data); });
            map.op(0xe8, 0xe8).nopw();
            map.op(0xf0, 0xf3).rw(m_ctc, (offset) => { return m_ctc.op0.read(offset); }, (offset, data) => { m_ctc.op0.write(offset, data); });
        }
    }


        //void mcr_nflfoot_state::ipu_91695_map(address_map &map)

        //void mcr_nflfoot_state::ipu_91695_portmap(address_map &map)


    public partial class mcr : construct_ioport_helper
    {
        /*************************************
         *
         *  Port definitions
         *
         *************************************/

        //static INPUT_PORTS_START( solarfox )

        //static INPUT_PORTS_START( kick )

        //static INPUT_PORTS_START( kickc )

        //static INPUT_PORTS_START( dpoker )

        //static INPUT_PORTS_START( shollow )


        /* verified from wiring diagram, plus DIP switches from manual */
        //static INPUT_PORTS_START( tron )
        void construct_ioport_tron(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("ssio:IP0");  /* J4 1-8 */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_TILT );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_SERVICE1 );
            PORT_SERVICE( 0x80, IP_ACTIVE_LOW );

            PORT_START("ssio:IP1");  /* J4 10-13,15-18 */
            PORT_BIT( 0xff, 0x00, IPT_DIAL ); PORT_SENSITIVITY(50); PORT_KEYDELTA(10); PORT_CODE_DEC(KEYCODE_Z); PORT_CODE_INC(KEYCODE_X); PORT_REVERSE();

            PORT_START("ssio:IP2");  /* J5 1-8 */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY(); PORT_COCKTAIL();

            PORT_START("ssio:IP3");  /* DIPSW @ B3 */
            PORT_DIPNAME( 0x01, 0x00, "Coin Meters" );  PORT_DIPLOCATION("SW1:1");
            PORT_DIPSETTING(    0x01, "1" );
            PORT_DIPSETTING(    0x00, "2" );
            PORT_DIPNAME( 0x02, 0x00, DEF_STR( Cabinet ) );  PORT_DIPLOCATION("SW1:2");
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x02, DEF_STR( Cocktail ) );
            PORT_DIPNAME( 0x04, 0x00, DEF_STR( Allow_Continue ) );  PORT_DIPLOCATION("SW1:3");
            PORT_DIPSETTING(    0x04, DEF_STR( No ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Yes ) );
            PORT_DIPUNUSED_DIPLOC( 0x08, 0x00, "SW1:4" );
            PORT_DIPUNUSED_DIPLOC( 0x10, 0x00, "SW1:5" );
            PORT_DIPUNUSED_DIPLOC( 0x20, 0x00, "SW1:6" );
            PORT_DIPUNUSED_DIPLOC( 0x40, 0x00, "SW1:7" );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();

            // According to the manual, SW1 is a bank of *10* switches (9 is unused and 10 is freeze)
            // Where are the values for the other two bits read?

            PORT_START("ssio:IP4");  /* J6 1-8 */
            PORT_BIT( 0xff, 0x00, IPT_DIAL ); PORT_SENSITIVITY(50); PORT_KEYDELTA(10); PORT_REVERSE(); PORT_COCKTAIL();

            PORT_START("ssio:DIP");
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNKNOWN );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( tron3 )

        //static INPUT_PORTS_START( kroozr )

        //static INPUT_PORTS_START( domino )

        //static INPUT_PORTS_START( journey )

        //static INPUT_PORTS_START( wacko )


        /* not verified, no manual found */
        //static INPUT_PORTS_START( twotiger )
        void construct_ioport_twotiger(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("ssio:IP0");  /* J4 1-8 */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_START3 ); PORT_NAME("Dogfight Start");
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_TILT );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_SERVICE1 );
            PORT_SERVICE( 0x80, IP_ACTIVE_LOW );

            PORT_START("ssio:IP1");  /* J4 10-13,15-18 */
            PORT_BIT( 0xff, 0x67, IPT_AD_STICK_X ); PORT_MINMAX(0, 206); PORT_SENSITIVITY(100); PORT_KEYDELTA(10); PORT_PLAYER(2);

            PORT_START("ssio:IP2");  /* J5 1-8 */
            PORT_BIT( 0xff, 0x67, IPT_AD_STICK_X ); PORT_MINMAX(0, 206); PORT_SENSITIVITY(100); PORT_KEYDELTA(10); PORT_PLAYER(1);

            PORT_START("ssio:IP3");  /* DIPSW @ B3 */
            PORT_DIPNAME( 0x01, 0x00, "Shot Speed" );
            PORT_DIPSETTING(    0x01, "Fast" );
            PORT_DIPSETTING(    0x00, "Slow" );
            PORT_DIPNAME( 0x02, 0x00, "Dogfight" );
            PORT_DIPSETTING(    0x00, "1 Credit" );
            PORT_DIPSETTING(    0x02, "2 Credits" );
            PORT_BIT( 0xfc, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("ssio:IP4");  /* J6 1-8 */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_BUTTON3 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON2 );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_BUTTON3 ); PORT_PLAYER(2);
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON2 ); PORT_PLAYER(2);
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_PLAYER(2);
            PORT_BIT( 0xc0, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("ssio:DIP");
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNKNOWN );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( twotigrc )


        /* verified from wiring diagram, plus DIP switches from manual */
        //static INPUT_PORTS_START( tapper )
        void construct_ioport_tapper(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("ssio:IP0");  /* J4 1-8 */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_TILT );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_SERVICE1 );
            PORT_SERVICE( 0x80, IP_ACTIVE_LOW );

            PORT_START("ssio:IP1");  /* J4 10-13,15-18 */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_4WAY();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_4WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_4WAY();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_4WAY();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0xe0, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("ssio:IP2");  /* J5 1-8 */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0xe0, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("ssio:IP3");  /* DIPSW @ B3 */
            PORT_BIT( 0x03, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_DIPNAME( 0x04, 0x00, DEF_STR( Demo_Sounds ) );
            PORT_DIPSETTING(    0x04, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_BIT( 0x38, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_DIPNAME( 0x40, 0x40, DEF_STR( Cabinet ) );
            PORT_DIPSETTING(    0x40, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Cocktail ) );
            PORT_DIPNAME( 0x80, 0x80, "Coin Meters" );
            PORT_DIPSETTING(    0x80, "1" );
            PORT_DIPSETTING(    0x00, "2" );

            PORT_START("ssio:IP4");  /* J6 1-8 */
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("ssio:DIP");
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNKNOWN );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( timber )


        /* verified from wiring diagram, plus DIP switches from manual */
        //static INPUT_PORTS_START( dotron )
        void construct_ioport_dotron(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("ssio:IP0");  /* J4 1-8 */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_TILT );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_SERVICE1 );
            PORT_SERVICE( 0x80, IP_ACTIVE_LOW );

            PORT_START("ssio:IP1");  /* J4 10-13,15-18 */
            PORT_BIT( 0x7f, 0x00, IPT_DIAL ); PORT_SENSITIVITY(50); PORT_KEYDELTA(10); PORT_REVERSE();
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("ssio:IP2");  /* J5 1-8 */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON3 ); PORT_NAME("Aim Down");
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_BUTTON4 ); PORT_NAME("Aim Up");
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_BUTTON2 );
            PORT_DIPNAME( 0x80, 0x80, DEF_STR( Cabinet ) );
            PORT_DIPSETTING(    0x00, "Environmental" );
            PORT_DIPSETTING(    0x80, DEF_STR( Upright ) );

            PORT_START("ssio:IP3");  /* DIPSW @ B3 */
            PORT_DIPNAME( 0x01, 0x01, "Coin Meters" );
            PORT_DIPSETTING(    0x01, "1" );
            PORT_DIPSETTING(    0x00, "2" );
            PORT_BIT( 0xfe, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("ssio:IP4");  /* J6 1-8 */
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("ssio:DIP");
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNKNOWN );

            PORT_START("FAKE");  /* fake port to make aiming up & down easier */
            PORT_BIT( 0xff, 0x00, IPT_TRACKBALL_Y ); PORT_SENSITIVITY(100); PORT_KEYDELTA(10);

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( dotrone )

        //static INPUT_PORTS_START( nflfoot )


        /* "wiring diagram was not available at time of publication" according to the manual */
        /* DIPs verified from the manual */
        //static INPUT_PORTS_START( demoderb )
        void construct_ioport_demoderb(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("ssio:IP0");  /* J4 1-8 */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_SERVICE( 0x20, IP_ACTIVE_LOW );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_TILT );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("ssio:IP1");  /* J4 10-13,15-18 */    /* The high 6 bits contain the steering wheel value */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_PLAYER(1);
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON2 ); PORT_PLAYER(1);

            PORT_START("ssio:IP1.ALT1"); /* J4 10-13,15-18 */    /* The high 6 bits contain the steering wheel value */
            PORT_BIT( 0x3f, 0x00, IPT_DIAL ); PORT_SENSITIVITY(50); PORT_KEYDELTA(10); PORT_INVERT(); PORT_PLAYER(1);

            PORT_START("ssio:IP1.ALT2"); /* IN1 (muxed) -- the high 6 bits contain the steering wheel value */
            PORT_BIT( 0x3f, 0x00, IPT_DIAL ); PORT_SENSITIVITY(50); PORT_KEYDELTA(10); PORT_INVERT(); PORT_PLAYER(3);

            PORT_START("ssio:IP2");  /* J5 1-8 */    /* The high 6 bits contain the steering wheel value */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_PLAYER(2);
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON2 ); PORT_PLAYER(2);

            PORT_START("ssio:IP2.ALT1"); /* J5 1-8 */    /* The high 6 bits contain the steering wheel value */
            PORT_BIT( 0x3f, 0x00, IPT_DIAL ); PORT_SENSITIVITY(50); PORT_KEYDELTA(10); PORT_INVERT(); PORT_PLAYER(2);

            PORT_START("ssio:IP2.ALT2"); /* IN2 (muxed) -- the high 6 bits contain the steering wheel value */
            PORT_BIT( 0x3f, 0x00, IPT_DIAL ); PORT_SENSITIVITY(50); PORT_KEYDELTA(10); PORT_INVERT(); PORT_PLAYER(4);

            PORT_START("ssio:IP3");  /* DIPSW @ B3 */
            PORT_DIPNAME( 0x01, 0x01, DEF_STR( Cabinet ) );
            PORT_DIPSETTING(    0x01, "2P Upright" );
            PORT_DIPSETTING(    0x00, "4P Cocktail" );
            PORT_DIPNAME( 0x02, 0x02, DEF_STR( Difficulty ) );
            PORT_DIPSETTING(    0x02, DEF_STR( Normal ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Harder ) );
            PORT_DIPNAME( 0x04, 0x04, DEF_STR( Free_Play ) );
            PORT_DIPSETTING(    0x04, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x08, 0x08, "Reward Screen" );
            PORT_DIPSETTING(    0x08, "Expanded" );
            PORT_DIPSETTING(    0x00, "Limited" );
            PORT_DIPNAME( 0x30, 0x30, DEF_STR( Coinage ) );
            PORT_DIPSETTING(    0x20, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _2C_2C ) );
            PORT_DIPSETTING(    0x30, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x10, DEF_STR( _1C_2C ) );
            PORT_BIT( 0xc0, IP_ACTIVE_LOW, IPT_UNKNOWN );

            PORT_START("ssio:IP4");  /* J6 1-8 */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_COIN3 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_COIN4 );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_START3 );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_START4 );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_PLAYER(3);
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_BUTTON2 ); PORT_PLAYER(3);
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_PLAYER(4);
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_BUTTON2 ); PORT_PLAYER(4);

            PORT_START("ssio:DIP");
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNKNOWN );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( demoderbc )
    }


    partial class mcr_state : driver_device
    {
        /*************************************
         *
         *  Graphics definitions
         *
         *************************************/

        static readonly gfx_layout mcr_bg_layout = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,2),
            4,
            ArrayCombineUInt32(STEP2((int)RGN_FRAC(1,2),1), STEP2((int)RGN_FRAC(0,2),1)),
            STEP8(0,2),
            STEP8(0,16),
            16*8
        );


        static readonly gfx_layout mcr_sprite_layout = new gfx_layout
        (
            32,32,
            RGN_FRAC(1,4),
            4,
            STEP4(0,1),
            ArrayCombineUInt32(STEP2((int)RGN_FRAC(0,4)+0,4),  STEP2((int)RGN_FRAC(1,4)+0,4),  STEP2((int)RGN_FRAC(2,4)+0,4),  STEP2((int)RGN_FRAC(3,4)+0,4),
                               STEP2((int)RGN_FRAC(0,4)+8,4),  STEP2((int)RGN_FRAC(1,4)+8,4),  STEP2((int)RGN_FRAC(2,4)+8,4),  STEP2((int)RGN_FRAC(3,4)+8,4),
                               STEP2((int)RGN_FRAC(0,4)+16,4), STEP2((int)RGN_FRAC(1,4)+16,4), STEP2((int)RGN_FRAC(2,4)+16,4), STEP2((int)RGN_FRAC(3,4)+16,4),
                               STEP2((int)RGN_FRAC(0,4)+24,4), STEP2((int)RGN_FRAC(1,4)+24,4), STEP2((int)RGN_FRAC(2,4)+24,4), STEP2((int)RGN_FRAC(3,4)+24,4)),
            STEP32(0,32),
            32*32
        );


        //static GFXDECODE_START( gfx_mcr )
        static readonly gfx_decode_entry [] gfx_mcr =
        {
            GFXDECODE_SCALE( "gfx1", 0, mcr_bg_layout,     0, 4, 2, 2 ), /* colors 0-15, 2x2 */
            GFXDECODE_ENTRY( "gfx2", 0, mcr_sprite_layout, 0, 4 ),       /* colors 16-31 */

            //GFXDECODE_END
        };


        /*************************************
         *
         *  Sound definitions
         *
         *************************************/

        //static const char *const journey_sample_names[] =


        static readonly string [] twotiger_sample_names =
        {
            "*twotiger",
            "left",
            "right",
            null
        };


        /*************************************
         *
         *  Machine driver
         *
         *************************************/
        /* 90009 CPU board plus 90908/90913/91483 sound board */
        void mcr_90009(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, MAIN_OSC_MCR_I / 8);
            m_maincpu.op0.z80daisy.set_daisy_config(mcr_daisy_chain);
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, cpu_90009_map);
            m_maincpu.op0.memory().set_addrmap(AS_IO, cpu_90009_portmap);

            TIMER(config, "scantimer").configure_scanline(mcr_interrupt, "screen", 0, 1);

            Z80CTC(config, m_ctc, MAIN_OSC_MCR_I / 8 /* same as "maincpu" */);
            m_ctc.op0.intr_callback().set_inputline(m_maincpu, INPUT_LINE_IRQ0).reg();
            m_ctc.op0.zc_callback<int_const_0>().set(m_ctc, (int state) => { m_ctc.op0.trg1(state); }).reg();

            WATCHDOG_TIMER(config, "watchdog").set_vblank_count("screen", 16);

            NVRAM(config, "nvram", nvram_device.default_value.DEFAULT_ALL_1);

            /* video hardware */
            screen_device screen = SCREEN(config, "screen", SCREEN_TYPE_RASTER);
            screen.set_video_attributes(VIDEO_UPDATE_BEFORE_VBLANK);
            screen.set_refresh_hz(30);
            screen.set_vblank_time(ATTOSECONDS_IN_USEC(2500)); /* not accurate */
            screen.set_size(32*16, 30*16);
            screen.set_visarea(0*16, 32*16-1, 0*16, 30*16-1);
            screen.set_screen_update(screen_update_mcr);
            screen.set_palette(m_palette);

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_mcr);
            PALETTE(config, m_palette).set_format(palette_device.xrbg_444_t.xRBG_444, 32);

            /* sound hardware */
            SPEAKER(config, "lspeaker").front_left();
            SPEAKER(config, "rspeaker").front_right();
            MIDWAY_SSIO(config, m_ssio);
            m_ssio.op0.dimixer.add_route(0, "lspeaker", 1.0);
            m_ssio.op0.dimixer.add_route(1, "rspeaker", 1.0);
        }


        //void mcr_dpoker_state::mcr_90009_dp(machine_config &config)


        /* 90010 CPU board plus 90908/90913/91483 sound board */
        public void mcr_90010(machine_config config)
        {
            mcr_90009(config);

            /* basic machine hardware */
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, cpu_90010_map);
            m_maincpu.op0.memory().set_addrmap(AS_IO, cpu_90010_portmap);

            /* video hardware */
            m_palette.op0.set_format(palette_device.xrbg_444_t.xRBG_444, 64);
        }


        /* as above, plus 8-track tape */
        public void mcr_90010_tt(machine_config config)
        {
            mcr_90010(config);

            /* sound hardware */
            SAMPLES(config, m_samples);
            m_samples.op0.set_channels(2);
            m_samples.op0.set_samples_names(twotiger_sample_names);
            m_samples.op0.disound.add_route(0, "lspeaker", 0.25);
            m_samples.op0.disound.add_route(1, "rspeaker", 0.25);
        }


#if false
        /* 91475 CPU board plus 90908/90913/91483 sound board plus cassette interface */
        void mcr_state::mcr_91475(machine_config &config)
        {
            mcr_90010(config);

            /* video hardware */
            m_palette->set_format(palette_device::xRBG_444, 128);

            /* sound hardware */
            SAMPLES(config, m_samples);
            m_samples->set_channels(1);
            m_samples->set_samples_names(journey_sample_names);
            m_samples->add_route(ALL_OUTPUTS, "lspeaker", 0.25);
            m_samples->add_route(ALL_OUTPUTS, "rspeaker", 0.25);
        }
#endif


        /* 91490 CPU board plus 90908/90913/91483 sound board */
        public void mcr_91490(machine_config config)
        {
            mcr_90010(config);

            /* basic machine hardware */
            m_maincpu.op0.set_clock(5000000);
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, cpu_91490_map);
            m_maincpu.op0.memory().set_addrmap(AS_IO, cpu_91490_portmap);

            m_ctc.op0.set_clock(5000000 /* same as "maincpu" */);
        }


#if false
        /* 91490 CPU board plus 90908/90913/91483 sound board plus Squawk n' Talk sound board */
        void mcr_state::mcr_91490_snt(machine_config &config)
        {
            mcr_91490(config);

            /* basic machine hardware */
            BALLY_SQUAWK_N_TALK(config, m_squawk_n_talk);
            m_squawk_n_talk->add_route(ALL_OUTPUTS, "lspeaker", 1.0);
            m_squawk_n_talk->add_route(ALL_OUTPUTS, "rspeaker", 1.0);
        }
#endif


        //void mcr_nflfoot_state::mcr_91490_ipu(machine_config &config)


        /* 91490 CPU board plus 90908/90913/91483 sound board plus Turbo Cheap Squeak sound board */
        public void mcr_91490_tcs(machine_config config)
        {
            mcr_91490(config);

            /* basic machine hardware */
            MIDWAY_TURBO_CHEAP_SQUEAK(config, m_turbo_cheap_squeak);
            m_turbo_cheap_squeak.op0.dimixer.add_route(ALL_OUTPUTS, "lspeaker", 1.0);
            m_turbo_cheap_squeak.op0.dimixer.add_route(ALL_OUTPUTS, "rspeaker", 1.0);
        }
    }


    partial class mcr : construct_ioport_helper
    {
        /*************************************
         *
         *  ROM definitions
         *
         *************************************/

        //ROM_START( solarfox )

        //ROM_START( kick )

        //ROM_START( kickman )

        //ROM_START( kickc )

        //ROM_START( dpoker )

        //ROM_START( shollow )

        //ROM_START( shollow2 )

        /*
          TRON (Set 1 - 8/9), program ROMs stamped as "AUG 9"
        */
        //ROM_START( tron )
        static readonly tiny_rom_entry [] rom_tron = 
        {
            ROM_REGION( 0x10000, "maincpu", 0 ), /* ROM's located on the Super CPU Board (90010) */
            ROM_LOAD( "scpu-pga_lctn-c2_tron_aug_9.c2", 0x0000, 0x2000, CRC("0de0471a") + SHA1("378847604a6a9c079d887348010ab9539d5f6195") ), // labeled: SCPU-PGA   LCTN-C2   TRON   (c)1982 BALLY   MIDWAY
            ROM_LOAD( "scpu-pgb_lctn-c3_tron_aug_9.c3", 0x2000, 0x2000, CRC("8ddf8717") + SHA1("e0c294afa8ba0b0ba89e3e0fb3ff6d8fc4398e32") ), // labeled: SCPU-PGB   LCTN-C3   TRON   (c)1982 BALLY   MIDWAY
            ROM_LOAD( "scpu-pgc_lctn-c4_tron_aug_9.c4", 0x4000, 0x2000, CRC("4241e3a0") + SHA1("24c1bd2f31e194542571c391c5dccf21354115c2") ), // labeled: SCPU-PGC   LCTN-C4   TRON   (c)1982 BALLY   MIDWAY
            ROM_LOAD( "scpu-pgd_lctn-c5_tron_aug_9.c5", 0x6000, 0x2000, CRC("035d2fe7") + SHA1("1b827ca30a439d2f4cc94fcc0e90ee0cf87e018c") ), // labeled: SCPU-PGD   LCTN-C5   TRON   (c)1982 BALLY   MIDWAY
            ROM_LOAD( "scpu-pge_lctn-c6_tron_aug_9.c6", 0x8000, 0x2000, CRC("24c185d8") + SHA1("45ac7c53f6f4eba5c7bf3fc6559cddd3821eddad") ), // labeled: SCPU-PGE   LCTN-C6   TRON   (c)1982 BALLY   MIDWAY
            ROM_LOAD( "scpu-pgf_lctn-c7_tron_aug_9.c7", 0xa000, 0x2000, CRC("38c4bbaf") + SHA1("a7cd496ce75199b8279ea963520cf70d5f562bb2") ), // labeled: SCPU-PGF   LCTN-C7   TRON   (c)1982 BALLY   MIDWAY

            ROM_REGION( 0x10000, "ssio:cpu", 0 ), /* ROM's located on the Super Sound I/O Board (90913) */
            ROM_LOAD( "ssi-0a_lctn-a7_tron.a7",   0x0000, 0x1000, CRC("765e6eba") + SHA1("42efeefc8571dfc237c0be3368248f1e56add92e") ), // labeled: SSI/0A   LCTN-A7   TRON   (c)1982 BALLY   MIDWAY
            ROM_LOAD( "ssi-0b_lctn-a8_tron.a8",   0x1000, 0x1000, CRC("1b90ccdd") + SHA1("0876e5eeaa63bb8cc97f3634a6ddd8a29a9b012f") ), // labeled: SSI/0B   LCTN-A8   TRON   (c)1982 BALLY   MIDWAY
            ROM_LOAD( "ssi-0c_lctn-a9_tron.a9",   0x2000, 0x1000, CRC("3a4bc629") + SHA1("ce8452a99a313ae7429de471bbea39de08c9fd4b") ), // labeled: SSI/0C   LCTN-A9   TRON   (c)1982 BALLY   MIDWAY

            ROM_REGION( 0x04000, "gfx1", 0 ), /* ROM's located on the Super CPU Board (90010) */
            ROM_LOAD( "scpu-bgg_lctn-g3_tron.g3", 0x0000, 0x2000, CRC("1a9ed2f5") + SHA1("b0d85b47873ac8ad475da18b9540d37232cb2b7c") ), // labeled: SCPU-BGG   LCTN-G3   TRON   (c)1982 BALLY   MIDWAY
            ROM_LOAD( "scpu-bgh_lctn-g4_tron.g4", 0x2000, 0x2000, CRC("3220f974") + SHA1("a38ea5f1db27f05d9689db838ce7a8de98f34837") ), // labeled: SCPU-BGH   LCTN-G4   TRON   (c)1982 BALLY   MIDWAY

            ROM_REGION( 0x08000, "gfx2", 0 ), /* ROM's located on the MCR/II Video Gen Board (91399) */
            ROM_LOAD( "vga_lctn-e1_tron.e1",      0x0000, 0x2000, CRC("bc036d1d") + SHA1("c5d54d0b80ac768ccf6fdd32cad1ef6359fa324c") ), // labeled: VGA   LCTN-E1   TRON   (c)1982 BALLY   MIDWAY
            ROM_LOAD( "vgb_lctn-dc1_tron.dc1",    0x2000, 0x2000, CRC("58ee14d3") + SHA1("5fb4268c9c73bdfc3b1e866618979aea3f219bbc") ), // labeled: VGB   LCTN-DC1   TRON   (c)1982 BALLY   MIDWAY
            ROM_LOAD( "vgc_lctn-cb1_tron.cb1",    0x4000, 0x2000, CRC("3329f9d4") + SHA1("11f4d744374e475d2c5b195a9f70888414529dd3") ), // labeled: VGC   LCTN-CB1   TRON   (c)1982 BALLY   MIDWAY
            ROM_LOAD( "vgd_lctn-a1_tron.a1",      0x6000, 0x2000, CRC("9743f873") + SHA1("71ed80ecd8caaf9fce1d7010f95c4678c9bd7102") ), // labeled: VGD   LCTN-A1   TRON   (c)1982 BALLY   MIDWAY

            ROM_REGION( 0x0005, "scpu_pals", 0), /* PAL's located on the Super CPU Board (90010) */
            ROM_LOAD( "0066-313bx-xxqx.a12.bin", 0x0000, 0x0001, NO_DUMP),
            ROM_LOAD( "0066-315bx-xxqx.b12.bin", 0x0000, 0x0001, NO_DUMP),
            ROM_LOAD( "0066-322bx-xx0x.e3.bin",  0x0000, 0x0001, NO_DUMP),
            ROM_LOAD( "0066-316bx-xxqx.g11.bin", 0x0000, 0x0001, NO_DUMP),
            ROM_LOAD( "0066-314bx-xxqx.g12.bin", 0x0000, 0x0001, NO_DUMP),

            ROM_END,
        };


        //ROM_START( tron2 )

        //ROM_START( tron3 )

        //ROM_START( tron4 )

        //ROM_START( tron5 )

        //ROM_START( tronger )

        //ROM_START( kroozr )

        //ROM_START( domino )

        //ROM_START( wacko )

        //ROM_START( twotiger )
        static readonly tiny_rom_entry [] rom_twotiger =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "cpu_d2",  0x0000, 0x2000, CRC("a682ed24") + SHA1("e4418143b02739e417c44e6b4089354778e8d77f") ),
            ROM_LOAD( "cpu_d3",  0x2000, 0x2000, CRC("5b48fde9") + SHA1("52e07ffdd360631ea322935af5fb560afe3006ea") ),
            ROM_LOAD( "cpu_d4",  0x4000, 0x2000, CRC("f1ab8c4d") + SHA1("0c410ddd2e1cd8a19c73bc0c7aca70d8c4308eeb") ),
            ROM_LOAD( "cpu_d5",  0x6000, 0x2000, CRC("d7129900") + SHA1("af5093082cfbc9fa4b42cfc74e62adbf9b6c63db") ),

            ROM_REGION( 0x10000, "ssio:cpu", 0 ),
            ROM_LOAD( "ssio_a7",   0x0000, 0x1000, CRC("64ddc16c") + SHA1("e119e1702ea00ffb86d413ed8e68b4e9dfefa79e") ),
            ROM_LOAD( "ssio_a8",   0x1000, 0x1000, CRC("c3467612") + SHA1("c968776d9561a7ac67e95a987b6d826ec2dc748e") ),
            ROM_LOAD( "ssio_a9",   0x2000, 0x1000, CRC("c50f7b2d") + SHA1("0f4779d4955d500c50b544d945fa78a5428b86ce") ),

            ROM_REGION( 0x04000, "gfx1", 0 ),
            ROM_LOAD( "2tgrbg0.bin",  0x0000, 0x2000, CRC("52f69068") + SHA1("30422e66ae1a6fe8c10494037758758dcd1211d1") ),
            ROM_LOAD( "2tgrbg1.bin",  0x2000, 0x2000, CRC("758d4f7d") + SHA1("272127f802bccf47958b5dcc949b7468b1ba4512") ),

            ROM_REGION( 0x08000, "gfx2", 0 ),
            ROM_LOAD( "vid_d1",  0x0000, 0x2000, CRC("da5f49da") + SHA1("9396d708d5771ec19f7abdadd8c8f874af68ab10") ),
            ROM_LOAD( "vid_c1",  0x2000, 0x2000, CRC("62ed737b") + SHA1("954c1f17da2ceb77e710faf0822d29381b873a07") ),
            ROM_LOAD( "vid_b1",  0x4000, 0x2000, CRC("0939921e") + SHA1("f52d3475232557959e501f70765a4ad472300e84") ),
            ROM_LOAD( "vid_a1",  0x6000, 0x2000, CRC("ef515824") + SHA1("983af762733405b96351ef4910f4f4be40c4880e") ),

            ROM_END,
        };


        //ROM_START( twotigerc )

        //ROM_START( journey )


        //ROM_START( tapper )
        static readonly tiny_rom_entry [] rom_tapper =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "tapper_c.p.u._pg_0_1c_1-27-84.1c",   0x00000, 0x4000, CRC("bb060bb0") + SHA1("ff5a729e36faea3758c8c7b345a42dd8bb465f44") ), /* labeled TAPPER C.P.U. PG 0 1C 1/27/84 */
            ROM_LOAD( "tapper_c.p.u._pg_1_2c_1-27-84.2c",   0x04000, 0x4000, CRC("fd9acc22") + SHA1("b9f0396e2eba5772deec4725c41fa9de49658e72") ), /* labeled TAPPER C.P.U. PG 1 2C 1/27/84 */
            ROM_LOAD( "tapper_c.p.u._pg_2_3c_1-27-84.3c",   0x08000, 0x4000, CRC("b3755d41") + SHA1("434d3c27b9f1e43def081d79b9f56dbce93a9207") ), /* labeled TAPPER C.P.U. PG 2 3C 1/27/84 */
            ROM_LOAD( "tapper_c.p.u._pg_3_4c_1-27-84.4c",   0x0c000, 0x2000, CRC("77273096") + SHA1("5e4e2dc1703b39f588ba374f6a610f273d710532") ), /* labeled TAPPER C.P.U. PG 3 4C 1/27/84 */

            ROM_REGION( 0x10000, "ssio:cpu", 0 ),
            ROM_LOAD( "tapper_sound_snd_0_a7_12-7-83.a7",   0x0000, 0x1000, CRC("0e8bb9d5") + SHA1("9e281c340b7702523c86d56317efad9e3688e585") ), /* labeled TAPPER SOUND SND 0 A7 12/7/83 */
            ROM_LOAD( "tapper_sound_snd_1_a8_12-7-83.a8",   0x1000, 0x1000, CRC("0cf0e29b") + SHA1("14334b9d2bfece3fe5bda0cbd53158ead8d27e53") ), /* labeled TAPPER SOUND SND 1 A8 12/7/83 */
            ROM_LOAD( "tapper_sound_snd_2_a9_12-7-83.a9",   0x2000, 0x1000, CRC("31eb6dc6") + SHA1("b38bba5f12516d899e023f99147868e3402fbd7b") ), /* labeled TAPPER SOUND SND 2 A9 12/7/83 */
            ROM_LOAD( "tapper_sound_snd_3_a10_12-7-83.a10", 0x3000, 0x1000, CRC("01a9be6a") + SHA1("0011407c1e886071282808c0a561789b1245a789") ), /* labeled TAPPER SOUND SND 3 A10 12/7/83 */

            ROM_REGION( 0x08000, "gfx1", 0 ),
            ROM_LOAD( "tapper_c.p.u._bg_1_6f_12-7-83.6f",   0x00000, 0x4000, CRC("2a30238c") + SHA1("eb30b9bb654324340f0fc5b44776ac2440c1e869") ), /* labeled TAPPER C.P.U. BG 1 6F 12/7/83 */
            ROM_LOAD( "tapper_c.p.u._bg_0_5f_12-7-83.5f",   0x04000, 0x4000, CRC("394ab576") + SHA1("23e29ec942e1e7516ae8068837af2d1c79592378") ), /* labeled TAPPER C.P.U. BG 0 5F 12/7/83 */

            ROM_REGION( 0x20000, "gfx2", 0 ),
            ROM_LOAD( "tapper_video_fg_1_a7_12-7-83.a7",   0x00000, 0x4000, CRC("32509011") + SHA1("a38667573d235efe2dc515e52af05825fe4e0f30") ), /* labeled TAPPER VIDEO FG1 A7 12/7/83 */
            ROM_LOAD( "tapper_video_fg_0_a8_12-7-83.a8",   0x04000, 0x4000, CRC("8412c808") + SHA1("2077f79177fda26f9c674b2ab525ec3833802059") ), /* labeled TAPPER VIDEO FG0 A8 12/7/83 */
            ROM_LOAD( "tapper_video_fg_3_a5_12-7-83.a5",   0x08000, 0x4000, CRC("818fffd4") + SHA1("930142dd73fb30c4d3ec09a1e37517c6c6774024") ), /* labeled TAPPER VIDEO FG3 A5 12/7/83 */
            ROM_LOAD( "tapper_video_fg_2_a6_12-7-83.a6",   0x0c000, 0x4000, CRC("67e37690") + SHA1("d553b8517c1d03a2be0b065f4da2fa99d9e6fb30") ), /* labeled TAPPER VIDEO FG2 A6 12/7/83 */
            ROM_LOAD( "tapper_video_fg_5_a3_12-7-83.a3",   0x10000, 0x4000, CRC("800f7c8a") + SHA1("8aead89e1adaee5f0b679661c4bfba36e0d639e8") ), /* labeled TAPPER VIDEO FG5 A3 12/7/83 */
            ROM_LOAD( "tapper_video_fg_4_a4_12-7-83.a4",   0x14000, 0x4000, CRC("32674ee6") + SHA1("402c166d50b4a693959b3f0706a7931a5daef6ce") ), /* labeled TAPPER VIDEO FG4 A4 12/7/83 */
            ROM_LOAD( "tapper_video_fg_7_a1_12-7-83.a1",   0x18000, 0x4000, CRC("070b4c81") + SHA1("95879a455ecfe2e3de7fe2a75078f9e6934960f5") ), /* labeled TAPPER VIDEO FG7 A1 12/7/83 */
            ROM_LOAD( "tapper_video_fg_6_a2_12-7-83.a2",   0x1c000, 0x4000, CRC("a37aef36") + SHA1("a24696f16d467d9eea4f25aef5f4c5ff55bf51ff") ), /* labeled TAPPER VIDEO FG6 A2 12/7/83 */

            ROM_END,
        };


        //ROM_START( tapperg )

        //ROM_START( tappera )

        //ROM_START( tapperb )

        //ROM_START( sutapper ) /* Distributed by Sega, Sega game ID# 834-5384 TAPPER */

        //ROM_START( rbtapper )

        //ROM_START( timber )


        /*
        Disc of TRON 10/4/83

        referred to as version 2, known differences:
          shows "DEREZZ A RING    2000" in score table
          Sound Test menu shows sounds 1 through 16 (though the other sounds are still in the game)
          Player Input screen shows "UPRIGHT   ACTIVATE ALL DEVICES   TILT TO EXIT"
        */
        //ROM_START( dotron )
        static readonly tiny_rom_entry [] rom_dotron =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "disc_tron_uprt_pg0_10-4-83.1c",  0x00000, 0x4000, CRC("40d00195") + SHA1("e06a8097f02b9f445df0dd5c0ec13f9a0a1dcd8a") ), // labeled: DISC/TRON   UPRT PG0   10/4/83   BALLY/MIDWY
            ROM_LOAD( "disc_tron_uprt_pg1_10-4-83.2c",  0x04000, 0x4000, CRC("5a7d1300") + SHA1("8a1f088de9289cd902e72b55d3e72c3f07246778") ), // labeled: DISC/TRON   UPRT PG1   10/4/83   BALLY/MIDWY
            ROM_LOAD( "disc_tron_uprt_pg2_10-4-83.3c",  0x08000, 0x4000, CRC("cb89c9be") + SHA1("c773a68891fbf94808a2ee0036928c0c48d6673d") ), // labeled: DISC/TRON   UPRT PG2   10/4/83   BALLY/MIDWY
            ROM_LOAD( "disc_tron_uprt_pg3_10-4-83.4c",  0x0c000, 0x2000, CRC("5098faf4") + SHA1("9f861f99cb170513b68aee48bbfd60ee439d7fa9") ), // labeled: DISC/TRON   UPRT PG3   10/4/83   BALLY/MIDWY

            ROM_REGION( 0x10000, "ssio:cpu", 0 ),
            ROM_LOAD( "disc_tron_uprt_snd0_10-4-83.a7",  0x00000, 0x1000, CRC("7fb54293") + SHA1("6d538a3e48f98e269623850f1f6774848a89fd59") ), // labeled: DISC/TRON   UPRT SND0   10/4/83   BALLY/MIDWY
            ROM_LOAD( "disc_tron_uprt_snd1_10-4-83.a8",  0x01000, 0x1000, CRC("edef7326") + SHA1("5c9a64604252eea0628bf9d6221e8add82f66abe") ), // labeled: DISC/TRON   UPRT SND1   10/4/83   BALLY/MIDWY
            ROM_LOAD( "disc_tron_uprt_snd2_9-22-83.a9",  0x02000, 0x1000, CRC("e8ef6519") + SHA1("261b0463a73b403bc46df3e04f3d12173787d6e7") ), // labeled: DISC/TRON   UPRT SND2   9/22/83   BALLY/MIDWY
            ROM_LOAD( "disc_tron_uprt_snd3_9-22-83.a10", 0x03000, 0x1000, CRC("6b5aeb02") + SHA1("039d8d664f067bc0d085ad7730ef63dbd6dc387e") ), // labeled: DISC/TRON   UPRT SND3   9/22/83   BALLY/MIDWY

            ROM_REGION( 0x04000, "gfx1", 0 ),
            ROM_LOAD( "loc-bg2.6f",   0x00000, 0x2000, CRC("40167124") + SHA1("782c8192dd58a3f23ff2338452dd03206d79030a") ), // need to verify actual labels
            ROM_LOAD( "loc-bg1.5f",   0x02000, 0x2000, CRC("bb2d7a5d") + SHA1("8044be9ffca9520fd77e0da492147e553f9f7da3") ),

            ROM_REGION( 0x10000, "gfx2", 0 ),
            ROM_LOAD( "loc-g.cp4",    0x00000, 0x2000, CRC("57a2b1ff") + SHA1("b97539ffd2f5fc8b86fc2f8f233cc26ba16f82ee") ), // these ROMs dated 8/15/83 - need to verify actual labels
            ROM_LOAD( "loc-h.cp3",    0x02000, 0x2000, CRC("3bb4d475") + SHA1("3795ba1640790041da51ebeac8517cc7d32e243e") ),
            ROM_LOAD( "loc-e.cp6",    0x04000, 0x2000, CRC("ce957f1a") + SHA1("24177a8dd6dcb377cf8aee7c7b47b26f29e77e20") ),
            ROM_LOAD( "loc-f.cp5",    0x06000, 0x2000, CRC("d26053ce") + SHA1("b7fb3d1df9b80c056cf131574565addb529645e1") ),
            ROM_LOAD( "loc-c.cp8",    0x08000, 0x2000, CRC("ef45d146") + SHA1("6cd83909b4376abce287e435a10e5bc25e18b265") ),
            ROM_LOAD( "loc-d.cp7",    0x0a000, 0x2000, CRC("5e8a3ef3") + SHA1("74983c922eae1326ecd0ff14000851e0b424cc61") ),
            ROM_LOAD( "loc-a.cp0",    0x0c000, 0x2000, CRC("b35f5374") + SHA1("3f330ffde52ac57c02dfdf8e105aefcc10f87a0b") ),
            ROM_LOAD( "loc-b.cp9",    0x0e000, 0x2000, CRC("565a5c48") + SHA1("9dfafd58bd552bfda4e1799a175735ecc1369ba3") ),

            ROM_END,
        };


        //ROM_START( dotrona )

        //ROM_START( dotrone )

        //ROM_START( nflfoot )


        //ROM_START( demoderb ) /* Dipswitch selectable 2 player Upright / 4 player Cocktail */
        static readonly tiny_rom_entry [] rom_demoderb =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ), /* Actually used "DRBY" for program roms, all others used full "DERBY" */
            ROM_LOAD( "demo_drby_pro_0", 0x00000, 0x4000, CRC("be7da2f3") + SHA1("6c43a7e4334d80829a333841ccd9fdc5824c915d") ), /* Dated 6/12/85 */
            ROM_LOAD( "demo_drby_pro_1", 0x04000, 0x4000, CRC("c6f6604c") + SHA1("69ce86e762ccfd9d15accf5ddcb2406eb1b11132") ), /* Dated 6/12/85 */
            ROM_LOAD( "demo_drby_pro_2", 0x08000, 0x4000, CRC("fa93b9d9") + SHA1("61891b7850da93c16d11cd6d20a72e1b371f47d3") ), /* Dated 6/12/85 */
            ROM_LOAD( "demo_drby_pro_3", 0x0c000, 0x4000, CRC("4e964883") + SHA1("a1cb4e07c7417abc8a08bdae31de2bda8063dedc") ), /* Dated 6/12/85 */

            ROM_REGION( 0x10000, "ssio:cpu", ROMREGION_ERASE00 ),    /* 64k for the audio CPU, not populated */

            ROM_REGION( 0x10000, "tcs:cpu", 0 ), /* 64k for the Turbo Cheap Squeak */
            ROM_LOAD( "tcs_u5.bin", 0x0c000, 0x2000, CRC("eca33b2c") + SHA1("938b021ea3b0f23aed7a98a930a58af371a02303") ),
            ROM_LOAD( "tcs_u4.bin", 0x0e000, 0x2000, CRC("3490289a") + SHA1("a9d56ea60bb901267da41ab408f8e1ed3742b0ac") ),

            ROM_REGION( 0x04000, "gfx1", 0 ),
            ROM_LOAD( "demo_derby_bg_06f.6f", 0x00000, 0x2000, CRC("cf80be19") + SHA1("a2ab09ee2dc76fab472fec7520ed972ccc10e826") ), /* Dated 2/7/85 */
            ROM_LOAD( "demo_derby_bg_15f.5f", 0x02000, 0x2000, CRC("4e173e52") + SHA1("ac5ae8007a63f9c074444783c1058109327dd118") ), /* Dated 2/7/85 */

            ROM_REGION( 0x20000, "gfx2", 0 ),
            ROM_LOAD( "demo_derby_fg0_a4.a4",   0x00000, 0x4000, CRC("e57a4de6") + SHA1("d1b2396a85b984e171d751ef8e1cf970ac4ff9fb") ), /* Dated 3/11/85 */
            ROM_LOAD( "demo_derby_fg4_a3.a3",   0x04000, 0x4000, CRC("55aa667f") + SHA1("d611dbf9e8ef383d02514b0edb9ea36670193bf0") ), /* Dated 3/11/85 */
            ROM_LOAD( "demo_derby_fg1_a6.a6",   0x08000, 0x4000, CRC("70259651") + SHA1("55967aaf2a7617c8f5a199d1e07128d79ce16970") ), /* Dated 3/11/85 */
            ROM_LOAD( "demo_derby_fg5_a5.a5",   0x0c000, 0x4000, CRC("5fe99007") + SHA1("9d640b4715333efdc6300dc353991d6934929399") ), /* Dated 3/11/85 */
            ROM_LOAD( "demo_derby_fg2_a8.a8",   0x10000, 0x4000, CRC("6cab7b95") + SHA1("8faff7458ab5ff2dd096dd78b1449a4096cc6345") ), /* Dated 3/11/85 */
            ROM_LOAD( "demo_derby_fg6_a7.a7",   0x14000, 0x4000, CRC("abfb9a8b") + SHA1("14ab416bc76db25ad97353c9072048c64ec95344") ), /* Dated 3/11/85  - Mislabeled as DEMO DERBY FG1 A7 */
            ROM_LOAD( "demo_derby_fg3_a10.a10", 0x18000, 0x4000, CRC("801d9b86") + SHA1("5a8c72d1060eea1a3ad67b98aa6eff13f6837af6") ), /* Dated 3/11/85 */
            ROM_LOAD( "demo_derby_fg7_a9.a9",   0x1c000, 0x4000, CRC("0ec3f60a") + SHA1("4176b246b0ea7bce9498c20e12678f16f7173529") ), /* Dated 3/11/85 */

            ROM_END,
        };


        //ROM_START( demoderbc ) /* Only supports 4 player cocktail mode! */
    }


    partial class mcr_state : driver_device
    {
        /*************************************
         *
         *  Driver initialization
         *
         *************************************/
        void mcr_init(int cpuboard, int vidboard, int ssioboard)
        {
            m_mcr_cpu_board = (uint32_t)cpuboard;
            m_mcr_sprite_board = (uint32_t)vidboard;

            m_mcr12_sprite_xoffs = 0;
            m_mcr12_sprite_xoffs_flip = 0;

            save_item(NAME(new { m_input_mux }));
            save_item(NAME(new { m_last_op4 }));

            if (m_ssio.found())
            {
                m_ssio.op0.set_custom_output(0, 0xff, this, mcr_control_port_w);
            }
        }


        //void mcr_state::init_solarfox()

        //void mcr_state::init_kick()

        //void mcr_dpoker_state::init_dpoker()


        public void init_mcr_90010()
        {
            mcr_init(90010, 91399, 90913);
        }


        //void mcr_state::init_wacko()


        public void init_twotiger()
        {
            mcr_init(90010, 91399, 90913);

            m_ssio.op0.set_custom_output(4, 0xff, this, twotiger_op4_w);
            m_maincpu.op0.memory().space(AS_PROGRAM).install_readwrite_handler(0xe800, 0xefff, 0, 0x1000, 0, twotiger_videoram_r, twotiger_videoram_w);
        }


        //void mcr_state::init_kroozr()

        //void mcr_state::init_journey()


        public void init_mcr_91490()
        {
            mcr_init(91490, 91464, 90913);
        }


#if false
        void mcr_state::init_dotrone()
        {
            mcr_init(91490, 91464, 91657);

            m_ssio->set_custom_output(4, 0xff, *this, FUNC(mcr_state::dotron_op4_w));
        }
#endif


        //void mcr_nflfoot_state::init_nflfoot()


        public void init_demoderb()
        {
            mcr_init(91490, 91464, 90913);

            m_ssio.op0.set_custom_input(1, 0xfc, this, demoderb_ip1_r);
            m_ssio.op0.set_custom_input(2, 0xfc, this, demoderb_ip2_r);
            m_ssio.op0.set_custom_output(4, 0xff, this, demoderb_op4_w);

            // the SSIO Z80 doesn't have any program to execute
            m_ssio.op0.suspend_cpu();
        }
    }


    partial class mcr : construct_ioport_helper
    {
        /*************************************
         *
         *  Game drivers
         *
         *************************************/

        static void mcr_state_mcr_90010(machine_config config, device_t device) { mcr_state mcr_state = (mcr_state)device; mcr_state.mcr_90010(config); }
        static void mcr_state_mcr_90010_tt(machine_config config, device_t device) { mcr_state mcr_state = (mcr_state)device; mcr_state.mcr_90010_tt(config); }
        static void mcr_state_mcr_91490(machine_config config, device_t device) { mcr_state mcr_state = (mcr_state)device; mcr_state.mcr_91490(config); }
        static void mcr_state_mcr_91490_tcs(machine_config config, device_t device) { mcr_state mcr_state = (mcr_state)device; mcr_state.mcr_91490_tcs(config); }
        static void mcr_state_init_mcr_90010(device_t owner) { mcr_state mcr_state = (mcr_state)owner; mcr_state.init_mcr_90010(); }
        static void mcr_state_init_twotiger(device_t owner) { mcr_state mcr_state = (mcr_state)owner; mcr_state.init_twotiger(); }
        static void mcr_state_init_mcr_91490(device_t owner) { mcr_state mcr_state = (mcr_state)owner; mcr_state.init_mcr_91490(); }
        static void mcr_state_init_demoderb(device_t owner) { mcr_state mcr_state = (mcr_state)owner; mcr_state.init_demoderb(); }

        static mcr m_mcr = new mcr();

        static device_t device_creator_tron(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new mcr_state(mconfig, type, tag); }
        static device_t device_creator_twotiger(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new mcr_state(mconfig, type, tag); }
        static device_t device_creator_tapper(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new mcr_state(mconfig, type, tag); }
        static device_t device_creator_dotron(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new mcr_state(mconfig, type, tag); }
        static device_t device_creator_demoderb(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new mcr_state(mconfig, type, tag); }


        /* 90010 CPU board + 91399 video gen + 90913 sound I/O */
        public static readonly game_driver driver_tron     = GAME(device_creator_tron,     rom_tron,     "1982", "tron",     "0", mcr_state_mcr_90010,     m_mcr.construct_ioport_tron,     mcr_state_init_mcr_90010, ROT90,              "Bally Midway", "Tron (8/9)",                       MACHINE_SUPPORTS_SAVE);

        /* hacked 90010 CPU board + 91399 video gen + 90913 sound I/O + 8-track interface */
        public static readonly game_driver driver_twotiger = GAME(device_creator_twotiger, rom_twotiger, "1984", "twotiger", "0", mcr_state_mcr_90010_tt,  m_mcr.construct_ioport_twotiger, mcr_state_init_twotiger,  ROT0,               "Bally Midway", "Two Tigers (dedicated)",           MACHINE_IMPERFECT_SOUND | MACHINE_SUPPORTS_SAVE);

        /* 91490 CPU board + 91464 video gen + 90913 sound I/O */
        public static readonly game_driver driver_tapper   = GAME(device_creator_tapper,   rom_tapper,   "1983", "tapper",   "0", mcr_state_mcr_91490,     m_mcr.construct_ioport_tapper,   mcr_state_init_mcr_91490, ROT0,               "Bally Midway", "Tapper (Budweiser, 1/27/84)",      MACHINE_SUPPORTS_SAVE); /* Date from program ROM labels - Newest version */
        public static readonly game_driver driver_dotron   = GAME(device_creator_dotron,   rom_dotron,   "1983", "dotron",   "0", mcr_state_mcr_91490,     m_mcr.construct_ioport_dotron,   mcr_state_init_mcr_91490, ORIENTATION_FLIP_X, "Bally Midway", "Discs of Tron (Upright, 10/4/83)", MACHINE_SUPPORTS_SAVE);

        /* 91490 CPU board + 91464 video gen + 90913 sound I/O + Turbo Cheap Squeak */
        public static readonly game_driver driver_demoderb = GAME(device_creator_demoderb, rom_demoderb, "1984", "demoderb", "0", mcr_state_mcr_91490_tcs, m_mcr.construct_ioport_demoderb, mcr_state_init_demoderb,  ROT0,               "Bally Midway", "Demolition Derby",                 MACHINE_SUPPORTS_SAVE);
    }
}
