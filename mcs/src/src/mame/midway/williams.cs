// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u8 = System.Byte;
using u32 = System.UInt32;

using static mame._6821pia_global;
using static mame._74157_global;
using static mame.bankdev_global;
using static mame.dac_global;
using static mame.disound_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.hc55516_global;
using static mame.input_merger_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.m6800_global;
using static mame.m6809_global;
using static mame.nvram_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.timer_global;
using static mame.watchdog_global;
using static mame.williams_global;


namespace mame
{
    partial class williams_state : driver_device
    {
        static readonly XTAL MASTER_CLOCK = new XTAL(12_000_000);
        static readonly XTAL SOUND_CLOCK  = new XTAL(3_579_545);
    }


    partial class defender_state : williams_state
    {
        /*************************************
         *
         *  Defender memory handlers
         *
         *************************************/

        protected virtual void main_map(address_map map, device_t device)
        {
            map.op(0x0000, 0xbfff).ram().share("videoram");
            map.op(0xc000, 0xcfff).m(m_bankc000, (map2, owner) => { m_bankc000.op0.amap8(map2); });
            map.op(0xd000, 0xdfff).w(bank_select_w);
            map.op(0xd000, 0xffff).rom();
        }


        void bankc000_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map(0x0000, 0x000f).mirror(0x03e0).writeonly().share("paletteram");
            map(0x03ff, 0x03ff).w(FUNC(defender_state::watchdog_reset_w));
            map(0x0010, 0x001f).mirror(0x03e0).w(FUNC(defender_state::video_control_w));
            map(0x0400, 0x04ff).mirror(0x0300).ram().w(FUNC(defender_state::cmos_w)).share("nvram");
            map(0x0800, 0x0bff).r(FUNC(defender_state::video_counter_r));
            map(0x0c00, 0x0c03).mirror(0x03e0).rw(m_pia[1], FUNC(pia6821_device::read), FUNC(pia6821_device::write));
            map(0x0c04, 0x0c07).mirror(0x03e0).rw(m_pia[0], FUNC(pia6821_device::read), FUNC(pia6821_device::write));
            map(0x1000, 0x9fff).rom().region("maincpu", 0x10000);
            map(0xa000, 0xffff).noprw();
#endif
        }
    }


        /*************************************
         *
         *  Mayday memory handlers
         *
         *************************************/

        //void mayday_state::main_map(address_map &map)


    partial class williams_state : driver_device
    {
        /*************************************
         *
         *  General Williams memory handlers
         *
         *************************************/

        protected void base_map(address_map map, device_t device)
        {
            map.op(0x0000, 0xbfff).ram().share("videoram");
            map.op(0x0000, 0x8fff).bankr("mainbank");
            map.op(0xc000, 0xc00f).mirror(0x03f0).writeonly().share("paletteram");
            map.op(0xc804, 0xc807).mirror(0x00f0).rw(m_pia[0], (offset) => { return m_pia[0].op0.read(offset); }, (offset, data) => { m_pia[0].op0.write(offset, data); });
            map.op(0xc80c, 0xc80f).mirror(0x00f0).rw(m_pia[1], (offset) => { return m_pia[1].op0.read(offset); }, (offset, data) => { m_pia[1].op0.write(offset, data); });
            map.op(0xc900, 0xc9ff).w(vram_select_w);
            map.op(0xca00, 0xca07).mirror(0x00f8).w(blitter_w);
            map.op(0xcb00, 0xcbff).r(video_counter_r);
            map.op(0xcbff, 0xcbff).w(watchdog_reset_w);
            map.op(0xcc00, 0xcfff).ram().w(cmos_w).share("nvram");
            map.op(0xd000, 0xffff).rom();
        }
    }


    partial class sinistar_state : williams_state
    {
        /*************************************
         *
         *  Sinistar memory handlers
         *
         *************************************/
        void main_map(address_map map, device_t device)
        {
            base_map(map, device);
            map.op(0xc900, 0xc9ff).w(vram_select_w);
            map.op(0xd000, 0xdfff).ram();
            map.op(0xe000, 0xffff).rom();
        }
    }


        //void bubbles_state::main_map(address_map &map)

        //void spdball_state::main_map(address_map &map)

        //void blaster_state::main_map(address_map &map)

        //void williams2_state::common_map(address_map &map)

        //void williams2_state::bank8000_map(address_map &map)

        // mysticm and inferno: D000-DFFF is RAM
        //void williams_d000_ram_state::d000_map(address_map &map)

        // tshoot and joust2: D000-DFFF is ROM
        //void williams_d000_rom_state::d000_map(address_map &map)


    partial class defender_state : williams_state
    {
        /*************************************
         *
         *  Sound board memory handlers
         *
         *************************************/

        protected override void sound_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map(0x0000, 0x007f).ram();     // internal RAM
            map(0x0400, 0x0403).mirror(0x8000).rw(m_pia[2], FUNC(pia6821_device::read), FUNC(pia6821_device::write));
            map(0xb000, 0xffff).rom();
#endif
        }
    }


    partial class williams_state : driver_device
    {
        protected virtual void sound_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x007f).ram();     // internal RAM
            map.op(0x0080, 0x00ff).ram();     // MC6810 RAM
            map.op(0x0400, 0x0403).mirror(0x8000).rw(m_pia[2], (offset) => { return m_pia[2].op0.read(offset); }, (offset, data) => { m_pia[2].op0.write(offset, data); });
            map.op(0xb000, 0xffff).rom();
        }
    }


        /* Same as above, but for second sound board */
        //void blaster_state::sound2_map(address_map &map)


        /*************************************
         *
         *  Later sound board memory handlers
         *
         *************************************/

        //void williams2_state::sound_map(address_map &map)


        /*************************************
         *
         *  Port definitions
         *
         *************************************/

        //static INPUT_PORTS_START( monitor_controls_mysticm )

        //static INPUT_PORTS_START( monitor_controls )

    public partial class williams : construct_ioport_helper
    {
        //static INPUT_PORTS_START( defender )
        void construct_ioport_defender(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            defender_state defender_state = (defender_state)owner;

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_BUTTON1 ); PORT_NAME("Fire");
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_BUTTON2 ); PORT_NAME("Thrust");
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_BUTTON3 ); PORT_NAME("Smart Bomb");
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_BUTTON4 ); PORT_NAME("Hyperspace");
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_START2 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_START1 );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_BUTTON5 ); PORT_NAME(DEF_STR( Reverse ));
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN); PORT_2WAY();

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ); PORT_2WAY();
            PORT_BIT( 0xfe, IP_ACTIVE_HIGH, IPT_UNKNOWN );

            PORT_START("IN2");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_SERVICE1 ); PORT_NAME("Auto Up / Manual Down"); PORT_TOGGLE();
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_SERVICE ); PORT_NAME("Advance");
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_COIN3 );
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_MEMORY_RESET ); PORT_NAME("High Score Reset");
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_COIN1 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_COIN2 );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_TILT );
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_UNKNOWN );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( mayday )

        //static INPUT_PORTS_START( colony7 )

        //static INPUT_PORTS_START( jin )


        //static INPUT_PORTS_START( stargate )
        void construct_ioport_stargate(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_BUTTON1 ); PORT_NAME("Fire");
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_BUTTON2 ); PORT_NAME("Thrust");
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_BUTTON3 ); PORT_NAME("Smart Bomb");
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_BUTTON6 ); PORT_NAME("Hyperspace");
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_START2 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_START1 );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_BUTTON4 ); PORT_NAME(DEF_STR( Reverse ));
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ); PORT_8WAY();

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ); PORT_8WAY();
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_BUTTON5 ); PORT_NAME("Inviso");
            PORT_BIT( 0xfc, IP_ACTIVE_HIGH, IPT_UNKNOWN );

            PORT_START("IN2");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_SERVICE1 ); PORT_NAME("Auto Up / Manual Down"); PORT_TOGGLE();
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_SERVICE ); PORT_NAME("Advance");
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_COIN3 );
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_MEMORY_RESET ); PORT_NAME("High Score Reset");
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_COIN1 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_COIN2 );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_TILT );
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_UNKNOWN );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( robotron )
        void construct_ioport_robotron(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICKLEFT_UP ); PORT_NAME("Move Up");
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICKLEFT_DOWN ); PORT_NAME("Move Down");
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICKLEFT_LEFT ); PORT_NAME("Move Left");
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICKLEFT_RIGHT ); PORT_NAME("Move Right");
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_START1 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_START2 );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_JOYSTICKRIGHT_UP ); PORT_NAME("Fire Up");
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_JOYSTICKRIGHT_DOWN ); PORT_NAME("Fire Down");

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICKRIGHT_LEFT ); PORT_NAME("Fire Left");
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICKRIGHT_RIGHT ); PORT_NAME("Fire Right");
            PORT_BIT( 0xfc, IP_ACTIVE_HIGH, IPT_UNKNOWN );

            PORT_START("IN2");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_SERVICE1 ); PORT_NAME("Auto Up / Manual Down"); PORT_TOGGLE();
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_SERVICE ); PORT_NAME("Advance");
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_COIN3 );
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_MEMORY_RESET ); PORT_NAME("High Score Reset");
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_COIN1 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_COIN2 );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_TILT );
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_UNKNOWN );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( joust )
        void construct_ioport_joust(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");
            // 0x0f muxed from INP1/INP2
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_START2 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_START1 );
            // 0xc0 muxed from INP1A/INP2A

            PORT_START("IN1");
            // 0x03 muxed from INP1A/INP2A
            PORT_BIT( 0xfc, IP_ACTIVE_HIGH, IPT_UNUSED );

            PORT_START("IN2");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_SERVICE1 ); PORT_NAME("Auto Up / Manual Down"); PORT_TOGGLE();
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_SERVICE ); PORT_NAME("Advance");
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_COIN3 );
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_MEMORY_RESET ); PORT_NAME("High Score Reset");
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_COIN1 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_COIN2 );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_TILT );
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_UNKNOWN );

            PORT_START("INP2");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ); PORT_2WAY(); PORT_PLAYER(2);
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ); PORT_2WAY(); PORT_PLAYER(2);
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_BUTTON1 ); PORT_PLAYER(2);
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_UNUSED );

            PORT_START("INP1");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ); PORT_2WAY(); PORT_PLAYER(1);
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ); PORT_2WAY(); PORT_PLAYER(1);
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_BUTTON1 ); PORT_PLAYER(1);
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_UNUSED );

            PORT_START("INP2A");
            PORT_BIT( 0x0f, IP_ACTIVE_HIGH, IPT_UNUSED );

            PORT_START("INP1A");
            PORT_BIT( 0x0f, IP_ACTIVE_HIGH, IPT_UNUSED );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( bubbles )

        //static INPUT_PORTS_START( splat )


        //static INPUT_PORTS_START( sinistar )
        void construct_ioport_sinistar(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");
            // pseudo analog joystick, see below

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_BUTTON1 );
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_BUTTON2 );
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_START1 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_START2 );
            PORT_BIT( 0xc0, IP_ACTIVE_HIGH, IPT_UNKNOWN );

            PORT_START("IN2");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_SERVICE1 ); PORT_NAME("Auto Up / Manual Down"); PORT_TOGGLE();
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_SERVICE ); PORT_NAME("Advance");
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_COIN3 );
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_MEMORY_RESET ); PORT_NAME("High Score Reset");
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_COIN1 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_COIN2 );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_TILT );
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_UNKNOWN );

            PORT_START("49WAYX");    // converted by port_0_49way_r()
            PORT_BIT( 0xff, 0x38, IPT_AD_STICK_X ); PORT_MINMAX(0x00,0x6f); PORT_SENSITIVITY(100); PORT_KEYDELTA(10);

            PORT_START("49WAYY");    // converted by port_0_49way_r()
            PORT_BIT( 0xff, 0x38, IPT_AD_STICK_Y ); PORT_MINMAX(0x00,0x6f); PORT_SENSITIVITY(100); PORT_KEYDELTA(10); PORT_REVERSE();

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( playball )

        //static INPUT_PORTS_START( blaster )

        //static INPUT_PORTS_START( blastkit )

        //static INPUT_PORTS_START( spdball )

        //static INPUT_PORTS_START( alienar )

        //static INPUT_PORTS_START( lottofun )

        //static INPUT_PORTS_START( mysticm )

        //template <int P>
        //CUSTOM_INPUT_MEMBER(tshoot_state::gun_r)

        //static INPUT_PORTS_START( tshoot )

        //static INPUT_PORTS_START( inferno )

        //static INPUT_PORTS_START( joust2 )
    }


        /*************************************
         *
         *  Graphics definitions
         *
         *************************************/

        //static const gfx_layout williams2_layout =

        //static GFXDECODE_START( gfx_williams2 )


    partial class williams_state : driver_device
    {
        /*************************************
         *
         *  Machine driver
         *
         *************************************/

        public void williams_base(machine_config config)
        {
            // basic machine hardware
            MC6809E(config, m_maincpu, MASTER_CLOCK / 3 / 4);
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, base_map);

            M6808(config, m_soundcpu, SOUND_CLOCK); // internal clock divider of 4, effective frequency is 894.886kHz
            m_soundcpu.op0.memory().set_addrmap(AS_PROGRAM, sound_map);

            NVRAM(config, "nvram", nvram_device.default_value.DEFAULT_ALL_0); // 5101 (Defender), 5114 or 6514 (later games) + battery

            // set a timer to go off every 32 scanlines, to toggle the VA11 line and update the screen
            TIMER(config, "scan_timer").configure_scanline(va11_callback, "screen", 0, 32);

            // also set a timer to go off on scanline 240
            TIMER(config, "240_timer").configure_scanline(count240_callback, "screen", 0, 240);

            WATCHDOG_TIMER(config, m_watchdog);

            // video hardware
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_video_attributes(VIDEO_UPDATE_SCANLINE | VIDEO_ALWAYS_UPDATE);
            m_screen.op0.set_raw(MASTER_CLOCK*2/3, 512, 6, 298, 260, 7, 247);
            m_screen.op0.set_screen_update(screen_update);

            PALETTE(config, m_palette, palette_init, 256);

            // sound hardware
            SPEAKER(config, "speaker").front_center();
            MC1408(config, "dac", 0).disound.add_route(ALL_OUTPUTS, "speaker", 0.25); // mc1408.ic6

            // pia
            INPUT_MERGER_ANY_HIGH(config, "mainirq").output_handler().set_inputline(m_maincpu, M6809_IRQ_LINE).reg();

            INPUT_MERGER_ANY_HIGH(config, "soundirq").output_handler().set_inputline(m_soundcpu, M6808_IRQ_LINE).reg();

            PIA6821(config, m_pia[0], 0);
            m_pia[0].op0.readpa_handler().set_ioport("IN0").reg();
            m_pia[0].op0.readpb_handler().set_ioport("IN1").reg();

            PIA6821(config, m_pia[1], 0);
            m_pia[1].op0.readpa_handler().set_ioport("IN2").reg();
            m_pia[1].op0.writepb_handler().set(snd_cmd_w).reg();
            m_pia[1].op0.irqa_handler().set("mainirq", (int state) => { ((input_merger_any_high_device)subdevice("mainirq")).in_w<u32_const_0>(state); }).reg();  //m_pia[1]->irqa_handler().set("mainirq", FUNC(input_merger_any_high_device::in_w<0>));
            m_pia[1].op0.irqb_handler().set("mainirq", (int state) => { ((input_merger_any_high_device)subdevice("mainirq")).in_w<u32_const_1>(state); }).reg();  //m_pia[1]->irqb_handler().set("mainirq", FUNC(input_merger_any_high_device::in_w<1>));

            PIA6821(config, m_pia[2], 0);
            m_pia[2].op0.writepa_handler().set("dac", (u8 data) => { ((dac_byte_interface)subdevice("dac")).data_w(data); }).reg();  //m_pia[2]->writepa_handler().set("dac", FUNC(dac_byte_interface::data_w));
            m_pia[2].op0.irqa_handler().set("soundirq", (int state) => { ((input_merger_any_high_device)subdevice("soundirq")).in_w<u32_const_0>(state); }).reg();  //m_pia[2]->irqa_handler().set("soundirq", FUNC(input_merger_any_high_device::in_w<0>));
            m_pia[2].op0.irqb_handler().set("soundirq", (int state) => { ((input_merger_any_high_device)subdevice("soundirq")).in_w<u32_const_1>(state); }).reg();  //m_pia[2]->irqb_handler().set("soundirq", FUNC(input_merger_any_high_device::in_w<1>));
        }
    }


    partial class defender_state : williams_state
    {
        public void defender(machine_config config)
        {
            williams_base(config);

            // basic machine hardware

            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, main_map);
            m_soundcpu.op0.memory().set_addrmap(AS_PROGRAM, sound_map);

            ADDRESS_MAP_BANK(config, m_bankc000).set_map(bankc000_map).set_options(ENDIANNESS_BIG, 8, 16, 0x1000);

            m_screen.op0.set_visarea(12, 304 - 1, 7, 247 - 1);

            m_blitter_config = WILLIAMS_BLITTER_NONE;
            m_blitter_clip_address = 0x0000;
        }


        //void defender_state::jin(machine_config &config)  // needs a different screen size or the credit text is clipped
    }


    partial class williams_state : driver_device
    {
        public void stargate(machine_config config)
        {
            williams_base(config);
            m_blitter_config = WILLIAMS_BLITTER_NONE;
            m_blitter_clip_address = 0x0000;
        }


        public void robotron(machine_config config)
        {
            williams_base(config);
            m_blitter_config = WILLIAMS_BLITTER_SC1;
            m_blitter_clip_address = 0xc000;
        }
    }


    partial class williams_muxed_state : williams_state
    {
        public void williams_muxed(machine_config config)
        {
            williams_base(config);

            // basic machine hardware

            // pia
            m_pia[0].op0.readpa_handler().set_ioport("IN0").mask_u32(0x30).reg();
            m_pia[0].op0.readpa_handler().append("mux_0", () => { return ((ls157_device)subdevice("mux_0")).output_r(); }).mask_u32(0x0f).reg();
            m_pia[0].op0.readpa_handler().append("mux_1", () => { return ((ls157_device)subdevice("mux_1")).output_r(); }).lshift(6).mask_u32(0xc0).reg();
            m_pia[0].op0.readpb_handler().set_ioport("IN1").mask_u32(0xfc).reg();
            m_pia[0].op0.readpb_handler().append("mux_1", () => { return ((ls157_device)subdevice("mux_1")).output_r(); }).rshift(2).mask_u32(0x03).reg();
            m_pia[0].op0.cb2_handler().set("mux_0", (int state) => { ((ls157_device)subdevice("mux_0")).select_w(state); }).reg();
            m_pia[0].op0.cb2_handler().append("mux_1", (state) => { ((ls157_device)subdevice("mux_1")).select_w(state); }).reg();

            LS157(config, m_mux0, 0); // IC3 on interface board (actually LS257 with OC tied low)
            m_mux0.op0.a_in_callback().set_ioport("INP2").reg();
            m_mux0.op0.b_in_callback().set_ioport("INP1").reg();

            LS157(config, m_mux1, 0); // IC4 on interface board (actually LS257 with OC tied low)
            m_mux1.op0.a_in_callback().set_ioport("INP2A").reg();
            m_mux1.op0.b_in_callback().set_ioport("INP1A").reg();
        }


        public void joust(machine_config config)
        {
            williams_muxed(config);
            m_blitter_config = WILLIAMS_BLITTER_SC1;
            m_blitter_clip_address = 0xc000;
        }


        //void williams_muxed_state::splat(machine_config &config)
    }


        //void spdball_state::spdball(machine_config &config)

        //void williams_state::lottofun(machine_config &config)


    partial class sinistar_state : williams_state
    {
        public void sinistar(machine_config config)
        {
            williams_base(config);

            // basic machine hardware
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, main_map);

            // sound hardware
            HC55516(config, "cvsd", 0).disound.add_route(ALL_OUTPUTS, "speaker", 0.8);

            // pia
            m_pia[0].op0.readpa_handler().set(port_0_49way_r).reg();

            m_pia[2].op0.ca2_handler().set("cvsd", (int state) => { ((hc55516_device)subdevice("cvsd")).digit_w(state); }).reg();
            m_pia[2].op0.cb2_handler().set("cvsd", (int state) => { ((hc55516_device)subdevice("cvsd")).clock_w(state); }).reg();

            m_blitter_config = WILLIAMS_BLITTER_SC1;
            m_blitter_clip_address = 0x7400;
        }
    }


        //void bubbles_state::bubbles(machine_config &config)  // has a full 8-bit NVRAM equipped

        //void playball_state::playball(machine_config &config)

        //void blaster_state::blastkit(machine_config &config)

        //void blaster_state::blaster(machine_config &config)

        //void williams2_state::williams2_base(machine_config &config)

        //void inferno_state::inferno(machine_config &config)

        //void mysticm_state::mysticm(machine_config &config)

        //void tshoot_state::tshoot(machine_config &config)

        //void joust2_state::joust2(machine_config &config)


    partial class williams : construct_ioport_helper
    {
        /*************************************
         *
         *  ROM definitions
         *
         *************************************/

        //ROM_START( defender )
        static readonly tiny_rom_entry [] rom_defender =
        {
            ROM_REGION( 0x19000, "maincpu", 0 ),
            ROM_LOAD( "defend.1",     0x0d000, 0x0800, CRC("c3e52d7e") + SHA1("a57f5278ffe44248fc73f9925d107f4024ad981a") ),
            ROM_LOAD( "defend.4",     0x0d800, 0x0800, CRC("9a72348b") + SHA1("ed6ce796702ff32209ced3cb1ba3837dbafa526f") ),
            ROM_LOAD( "defend.2",     0x0e000, 0x1000, CRC("89b75984") + SHA1("a9481478da38f99efb67f0ecf82d084e14b93b42") ),
            ROM_LOAD( "defend.3",     0x0f000, 0x1000, CRC("94f51e9b") + SHA1("a24cfc55de56a72758c76fe2a55f1ec6c353b16f") ),
            ROM_LOAD( "defend.9",     0x10000, 0x0800, CRC("6870e8a5") + SHA1("67ccc194b1753a18af0c85f5e603355549c4f727") ),
            ROM_LOAD( "defend.12",    0x10800, 0x0800, CRC("f1f88938") + SHA1("26e48dfeefa0766837b1e762695b9532dbc8bc5e") ),
            ROM_LOAD( "defend.8",     0x11000, 0x0800, CRC("b649e306") + SHA1("9d7bc3c89e5a53c575946f06702c722b864b1ff0") ),
            ROM_LOAD( "defend.11",    0x11800, 0x0800, CRC("9deaf6d9") + SHA1("59b018ba0f3fe6eadfd387dc180ac281460358bc") ),
            ROM_LOAD( "defend.7",     0x12000, 0x0800, CRC("339e092e") + SHA1("2f89951dbe55d80df43df8dcf497171f73e726d3") ),
            ROM_LOAD( "defend.10",    0x12800, 0x0800, CRC("a543b167") + SHA1("9292b94b0d74e57e03aada4852ad1997c34122ff") ),
            ROM_LOAD( "defend.6",     0x16000, 0x0800, CRC("65f4efd1") + SHA1("a960fd1559ed74b81deba434391e49fc6ec389ca") ),

            ROM_REGION( 0x10000, "soundcpu", 0 ),
            ROM_LOAD( "video_sound_rom_1.ic12", 0xf800, 0x0800, CRC("fefd5b48") + SHA1("ceb0d18483f0691978c604db94417e6941ad7ff2") ),

            ROM_REGION( 0x0400, "proms", 0 ),
            ROM_LOAD( "decoder.2",   0x0000, 0x0200, CRC("8dd98da5") + SHA1("da979604f7a2aa8b5a6d4a5debd2e80f77569e35") ),
            ROM_LOAD( "decoder.3",   0x0200, 0x0200, CRC("c3f45f70") + SHA1("d19036cbc46b130548873597b44b8b70758f25c4") ),
            ROM_END,
        };


        //ROM_START( defenderg )

        //ROM_START( defenderb )

        //ROM_START( defenderw )

        //ROM_START( defenderj )

        //ROM_START( defndjeu )

        //ROM_START( tornado1 )

        //ROM_START( tornado2 )

        //ROM_START( zero )

        //ROM_START( zero2 )

        //ROM_START( defcmnd )

        //ROM_START( startrkd )

        //ROM_START( defence )

        //ROM_START( defenseb )

        //ROM_START( attackf )

        //ROM_START( galwars2 ) // 2 board stack: CPU and ROM boards

        //ROM_START( mayday )

        //ROM_START( maydaya )

        //ROM_START( maydayb )

        //ROM_START( batlzone )

        //ROM_START( colony7 )

        //ROM_START( colony7a )

        //ROM_START( jin )


        /*

        Stargate ROM labels are in this format:

        +--------------------+
        | STARGATE ROM 1-A   |   <-- Game name, ROM board number and ROM type (A is 2532, B is 2732)
        | (c) 1982 WILLIAMS  |
        | ELECTRONICS, INC.  |
        |     3002-1         |   <-- Williams game number & ROM number
        +--------------------+

        +--------------------+
        | Video Sound Rom 2  |
        | (c) 1981 WILLIAMS  |
        | ELECTRONICS, INC.  |
        |         STD. 744   |
        +--------------------+

        Solid Yellow (Black print) 3002-1  through 3002-12 - ROM type A, 2532
        Solid Yellow (Green print) 3002-13 through 3002-24 - ROM type B, 2732

                     |    Black print    |    Green print
         Part Number |  ROM#     Number  |  ROM#     Number
        -------------+-------------------+------------------
        A-5343-09700 | ROM 1A  - 3002-1  | ROM 1B  - 3002-13
        A-5343-09701 | ROM 2A  - 3002-2  | ROM 2B  - 3002-14
        A-5343-09702 | ROM 3A  - 3002-3  | ROM 3B  - 3002-15
        A-5343-09703 | ROM 4A  - 3002-4  | ROM 4B  - 3002-16
        A-5343-09704 | ROM 5A  - 3002-5  | ROM 5B  - 3002-17
        A-5343-09705 | ROM 6A  - 3002-6  | ROM 6B  - 3002-18
        A-5343-09706 | ROM 7A  - 3002-7  | ROM 7B  - 3002-19
        A-5343-09707 | ROM 8A  - 3002-8  | ROM 8B  - 3002-20
        A-5343-09708 | ROM 9A  - 3002-9  | ROM 9B  - 3002-21
        A-5343-09709 | ROM 10A - 3002-10 | ROM 10B - 3002-22
        A-5343-09710 | ROM 11A - 3002-11 | ROM 11B - 3002-23
        A-5343-09711 | ROM 12A - 3002-12 | ROM 12B - 3002-24

        D-8729-3002 ROM Board Assembly:
        +-----------------------------------------------+
        |        2J3                           2J4      |
        |               +---------------+               |
        |               | 6821 PIA @ 1C |               |
        |2              +---------------+             L |
        |J    4049BP    7420N       7474     SN7425N  E |
        |2      7474    74LS139N    7411PC   SN7404N  D |
        |  +----------+    +----------+    +----------+ |
        |  | ROM3  4A |    | ROM2  4C |    | ROM1  4E | |
        |  +----------+    +----------+    +----------+ |
        |  +----------+    +----------+    +----------+ |
        |  | ROM6  5A |    | ROM5  5C |    | ROM4  5E | |
        |  +----------+    +----------+    +----------+ |
        |  +----------+    +----------+    +----------+ |
        |W | ROM9  6A |  W | ROM8  6C |    | ROM7  6E | |
        |2 +----------+  4 +----------+    +----------+ |
        |  +----------+    +----------+    +----------+ |
        |w | ROM10 7A |  W | ROM11 7C |    | ROM12 7E | |
        |1 +----------+  3 +----------+    +----------+ |
        |                          +------------------+ |
        |                          |  2J1  connector  | |
        +--------------------------+------------------+-+

        Wire W2 & W4 with Zero Ohm resistors for 2532 ROMs
        Wire W1 & W3 with Zero Ohm resistors for 2732 ROMs

        */
        //ROM_START( stargate ) /* "B" ROMs labeled 3002-13 through 3002-24, identical data */
        static readonly tiny_rom_entry [] rom_stargate =
        {
            ROM_REGION( 0x19000, "maincpu", 0 ),
            ROM_LOAD( "stargate_rom_10-a_3002-10.a7", 0x0d000, 0x1000, CRC("60b07ff7") + SHA1("ba833f48ddfc1bd04ddb41b1d1c840d66ee7da30") ),
            ROM_LOAD( "stargate_rom_11-a_3002-11.c7", 0x0e000, 0x1000, CRC("7d2c5daf") + SHA1("6ca39f493eb8b370154ad46ef01976d352c929e1") ),
            ROM_LOAD( "stargate_rom_12-a_3002-12.e7", 0x0f000, 0x1000, CRC("a0396670") + SHA1("c46872550e0ca031453c6513f8f0448ecc9b5572") ),
            ROM_LOAD( "stargate_rom_1-a_3002-1.e4",   0x10000, 0x1000, CRC("88824d18") + SHA1("f003a5a9319c4eb8991fa2aae3f10c72d6b8e81a") ),
            ROM_LOAD( "stargate_rom_2-a_3002-2.c4",   0x11000, 0x1000, CRC("afc614c5") + SHA1("087c6da93318e8dc922d3d22e0a2af7b9759701c") ),
            ROM_LOAD( "stargate_rom_3-a_3002-3.a4",   0x12000, 0x1000, CRC("15077a9d") + SHA1("7badb4318b208f49d7fa65e915d0aa22a1e37915") ),
            ROM_LOAD( "stargate_rom_4-a_3002-4.e5",   0x13000, 0x1000, CRC("a8b4bf0f") + SHA1("6b4d47c2899fe9f14f9dab5928499f12078c437d") ),
            ROM_LOAD( "stargate_rom_5-a_3002-5.c5",   0x14000, 0x1000, CRC("2d306074") + SHA1("54f871983699113e31bb756d4ca885c26c2d66b4") ),
            ROM_LOAD( "stargate_rom_6-a_3002-6.a5",   0x15000, 0x1000, CRC("53598dde") + SHA1("54b02d944caf95283c9b6f0160e75ea8c4ccc97b") ),
            ROM_LOAD( "stargate_rom_7-a_3002-7.e6",   0x16000, 0x1000, CRC("23606060") + SHA1("a487ffcd4920d1056b87469735f7e1002f6a2e49") ),
            ROM_LOAD( "stargate_rom_8-a_3002-8.c6",   0x17000, 0x1000, CRC("4ec490c7") + SHA1("8726ebaf048db9608dfe365bf434ed5ca9452db7") ),
            ROM_LOAD( "stargate_rom_9-a_3002-9.a6",   0x18000, 0x1000, CRC("88187b64") + SHA1("efacc4a6d4b2af9a236c9d520de6d605c79cc5a8") ),

            ROM_REGION( 0x10000, "soundcpu", 0 ),
            ROM_LOAD( "video_sound_rom_2_std_744.ic12", 0xf800, 0x0800, CRC("2fcf6c4d") + SHA1("9c4334ac3ff15d94001b22fc367af40f9deb7d57") ), // P/N A-5342-09809

            ROM_REGION( 0x0400, "proms", 0 ),
            ROM_LOAD( "decoder_rom_4.3g", 0x0000, 0x0200, CRC("e6631c23") + SHA1("9988723269367fb44ef83f627186a1c88cf7877e") ), // Universal Horizontal decoder ROM - 7641-5 BPROM - P/N A-5342-09694
            ROM_LOAD( "decoder_rom_5.3c", 0x0200, 0x0200, CRC("f921c5fe") + SHA1("9cebb8bb935315101d248140d1b4503993ebdf8a") ), // Universal Vertical decoder ROM - 7641-5 BPROM - P/N A-5342-09695

            ROM_END,
        };


        /*

        Robotron 2084 ROM labels are in this format:

        +--------------------+
        | 2084 ROM 1-A       |   <-- Game name, ROM board number and ROM type (A is 2532, B is 2732)
        | (c) 1982 WILLIAMS  |
        | ELECTRONICS, INC.  |
        |     3005-1         |   <-- Williams game number & ROM number
        +--------------------+

        +--------------------+
        | Video Sound Rom 3  |
        | (c) 1981 WILLIAMS  |
        | ELECTRONICS, INC.  |
        |         STD. 767   |
        +--------------------+

        Yellow/Red (black print) 3005-1  through 3005-12 - ROM type B, 2732 (jumpers W1 & W3) (the "A" is overwitten with "B")
        Yellow/Red (green print) 3005-13 through 3005-24 - ROM type B, 2732 (jumpers W1 & W3)
        Solid Blue               3005-13 through 3005-24 - ROM type B, 2732 (jumpers W1 & W3)

        It's been confirmed that the Yellow labeled ROMs 3005-1 through 3005-12 are itentical to yellow labeled ROMs 3005-13 through 3005-24
        Yellow labels ROMs 3005-1 through 3005-12 are known to be labeled as "A" type ROMs with the A overwitten with "B"

        NOTE: Blue labels and later Yellow labels with red stripe share the SAME 3005-x numbers but have different data!

                   | Y/R Black | Y/R Green |Solid Blue |
        ROM | Board|  "A" ROMs |  "B" ROMs |  "B" ROMs |
         ## | Loc. |  label #  |  label #  |  label #  |
        ----+------+-----------+-----------+-----------+
          1 |  E4  |  3005-1   |  3005-13  |  3005-13  |
          2 |  C4  |  3005-2   |  3005-14  |  3005-14  |
          3 |  A4  |  3005-3   |  3005-15  |  3005-15  |
          4 |  E5  |  3005-4   |  3005-16  |  3005-16  |
          5 |  C5  |  3005-5   |  3005-17  |  3005-17  |
          6 |  A5  |  3005-6   |  3005-18  |  3005-18  |
          7 |  E6  |  3005-7   |  3005-19  |  3005-19  |
          8 |  C6  |  3005-8   |  3005-20  |  3005-20  |
          9 |  A6  |  3005-9   |  3005-21  |  3005-21  |
         10 |  A7  |  3005-10  |  3005-22  |  3005-22  |
         11 |  C7  |  3005-11  |  3005-23  |  3005-23  |
         12 |  E7  |  3005-12  |  3005-24  |  3005-24  |
        ----+------+-----------+-----------+-----------+

                |      Red label or        |
                | Yellow with red stripe   |    Solid Blue labeled
         ROM #  | Part Number     Number   |  Part Number    Number
        --------+--------------------------+------------------------
        ROM 1B  |  A-5343-09898   3005-1   |  A-5343-09945   3005-13
        ROM 2B  |  A-5343-09899   3005-2   |  A-5343-09946   3005-14
        ROM 3B  |  A-5343-09900   3005-3   |  A-5343-09947   3005-15
        ROM 4B  |  A-5343-09901   3005-4   |  A-5343-09948   3005-16
        ROM 5B  |  A-5343-09902   3005-5   |  A-5343-09949   3005-17
        ROM 6B  |  A-5343-09903   3005-6   |  A-5343-09950   3005-18
        ROM 7B  |  A-5343-09904   3005-7   |  A-5343-09951   3005-19
        ROM 8B  |  A-5343-09905   3005-8   |  A-5343-09952   3005-20
        ROM 9B  |  A-5343-09906   3005-9   |  A-5343-09953   3005-21
        ROM 10B |  A-5343-09907   3005-10  |  A-5343-09954   3005-22
        ROM 11B |  A-5343-09908   3005-11  |  A-5343-09955   3005-23
        ROM 12B |  A-5343-09909   3005-12  |  A-5343-09956   3005-24

        Robotron 2084 Manual No. 16P-3005-101 May 1982:
          - Current Robotron games use blue-label ROMs.  Earlier games have either yellow or red-labels ROMs, which are interchangable
              and may be mixed in the same game. DO NOT attempt to mix blue-label ROMs with red or yellow-label ROMs.

        D-9144-3005 ROM Board Assembly:
        +----------------------------------------------+
        |       2J3                           2J4      |
        |              +---------------+               |
        |              | 6821 PIA @ 1B |               |
        |2             +---------------+             L |
        |J   4049BP    7420N       7474     SN7425N  E |
        |2     7474    74LS139N    7411PC   SN7404N  D |
        | +----------+    +----------+    +----------+ |
        | | ROM3  4A |    | ROM2  4C |    | ROM1  4E | |
        | +----------+    +----------+    +----------+ |
        | +----------+    +----------+    +----------+ |
        | | ROM6  5A |WW  | ROM5  5C |WW  | ROM4  5E | |
        | +----------+12  +----------+34  +----------+ |
        | +----------+    +----------+    +----------+ |
        | | ROM9  6A |    | ROM8  6C |    | ROM7  6E | |
        | +----------+    +----------+    +----------+ |
        | +----------+    +----------+    +----------+ |
        | | ROM10 7A |    | ROM11 7C |    | ROM12 7E | |
        | +----------+    +----------+    +----------+ |
        | +------------------+            +----------+ |
        | | VTI 8220  VL2001 |  74LS245N  |  74154N  | |
        | +------------------+            +----------+ |
        | +------------------+  74LS244N               |
        | | VTI 8220  VL2001 |    +------------------+ |
        | +------------------+    |  2J1  connector  | |
        +-------------------------+------------------+-+

        Connectors:
        2J1 40 pin ribbon cable connetor
        2J2  6 pin header (KEY pin 4)
        2J3 10 pin header (KEY pin 9)
        2J4  9 pin header (KEY pin 1)

        LED - 7Seg LED display

        Wired W1 & W3 with Zero Ohm resistors for 2732 ROMs

        */
        //ROM_START( robotron ) /* Solid Blue labels, "B" type ROMs labeled 3005-13 through 3005-24 */
        static readonly tiny_rom_entry [] rom_robotron =
        {
            ROM_REGION( 0x19000, "maincpu", 0 ),
            ROM_LOAD( "2084_rom_10b_3005-22.a7", 0x0d000, 0x1000, CRC("13797024") + SHA1("d426a50e75dabe936de643c83a548da5e399331c") ),
            ROM_LOAD( "2084_rom_11b_3005-23.c7", 0x0e000, 0x1000, CRC("7e3c1b87") + SHA1("f8c6cbe3688f256f41a121255fc08f575f6a4b4f") ),
            ROM_LOAD( "2084_rom_12b_3005-24.e7", 0x0f000, 0x1000, CRC("645d543e") + SHA1("fad7cea868ebf17347c4bc5193d647bbd8f9517b") ),
            ROM_LOAD( "2084_rom_1b_3005-13.e4",  0x10000, 0x1000, CRC("66c7d3ef") + SHA1("f6d60e26c209c1df2cc01ac07ad5559daa1b7118") ), // == 2084_rom_1b_3005-1.e4
            ROM_LOAD( "2084_rom_2b_3005-14.c4",  0x11000, 0x1000, CRC("5bc6c614") + SHA1("4d6e82bc29f49100f7751ccfc6a9ff35695b84b3") ), // == 2084_rom_2b_3005-2.c4
            ROM_LOAD( "2084_rom_3b_3005-15.a4",  0x12000, 0x1000, CRC("e99a82be") + SHA1("06a8c8dd0b4726eb7f0bb0e89c8533931d75fc1c") ),
            ROM_LOAD( "2084_rom_4b_3005-16.e5",  0x13000, 0x1000, CRC("afb1c561") + SHA1("aaf89c19fd8f4e8750717169eb1af476aef38a5e") ),
            ROM_LOAD( "2084_rom_5b_3005-17.c5",  0x14000, 0x1000, CRC("62691e77") + SHA1("79b4680ce19bd28882ae823f0e7b293af17cbb91") ),
            ROM_LOAD( "2084_rom_6b_3005-18.a5",  0x15000, 0x1000, CRC("bd2c853d") + SHA1("f76ec5432a7939b33a27be1c6855e2dbe6d9fdc8") ),
            ROM_LOAD( "2084_rom_7b_3005-19.e6",  0x16000, 0x1000, CRC("49ac400c") + SHA1("06eae5138254723819a5e93cfd9e9f3285fcddf5") ), // == 2084_rom_7b_3005-7.e6
            ROM_LOAD( "2084_rom_8b_3005-20.c6",  0x17000, 0x1000, CRC("3a96e88c") + SHA1("7ae38a609ed9a6f62ca003cab719740ed7651b7c") ), // == 2084_rom_8b_3005-8.c6
            ROM_LOAD( "2084_rom_9b_3005-21.a6",  0x18000, 0x1000, CRC("b124367b") + SHA1("fd9d75b866f0ebbb723f84889337e6814496a103") ), // == 2084_rom_9b_3005-9.a6

            ROM_REGION( 0x10000, "soundcpu", 0 ),
            ROM_LOAD( "video_sound_rom_3_std_767.ic12", 0xf000, 0x1000, CRC("c56c1d28") + SHA1("15afefef11bfc3ab78f61ab046701db78d160ec3") ), // P/N A-5342-09910

            ROM_REGION( 0x0400, "proms", 0 ),
            ROM_LOAD( "decoder_rom_4.3g", 0x0000, 0x0200, CRC("e6631c23") + SHA1("9988723269367fb44ef83f627186a1c88cf7877e") ), // Universal Horizontal decoder ROM - 7641-5 BPROM - P/N A-5342-09694
            ROM_LOAD( "decoder_rom_6.3c", 0x0200, 0x0200, CRC("83faf25e") + SHA1("30002643d08ed983a6701a7c4b5ee74a2f4a1adb") ), // Universal Vertical decoder ROM - 7641-5 BPROM - P/N A-5342-09821

            ROM_END,
        };


        //ROM_START( robotronyo ) /* Yellow label / Red stripe & Black print or Yellow label / Red stripe & Green print "B" type ROMs numbered 3005-13 through 3005-24 */

        //ROM_START( robotronun )

        //ROM_START( robotron87 ) /* Patch by Christian Gingras in 1987 fixing 7 bugs, AKA "Shot in the corner" bug fix */

        //ROM_START( robotron12 )

        //ROM_START( robotrontd ) /* Tie-Die version starts with a "Solid Blue label" set */

        /*

        Joust ROM labels are in this format:

        +--------------------+
        | JOUST ROM 1A       |   <-- Game name, ROM board number and ROM type (A is 2532, B is 2732)
        | (c) 1982 WILLIAMS  |
        | ELECTRONICS, INC.  |
        |     3006-1         |   <-- Williams game number & ROM number
        +--------------------+

        +--------------------+
        | Video Sound Rom 4  |
        | (c) 1981 WILLIAMS  |
        | ELECTRONICS, INC.  |
        |         STD. 780   |
        +--------------------+

        Solid yellow* 3006-1  through 3006-12 - ROM type A, 2532 (jumpers W2 & W4)
        Solid green^  3006-13 through 3006-24 - ROM type B, 2732 (jumpers W1 & W3)
        Solid red     3006-28 through 3006-39 - ROM type A, 2532 (jumpers W2 & W4)
         --Missing--  3006-40 through 3006-51 - This set is unknown
        White/green   3006-52 through 3006-63 - ROM type B, 2732 (jumpers W1 & W3)

        * NOTE: Earliest examples are yellow lables with red stripe numbered 16-3006-1 through 16-3006-12
        ^ NOTE: Earliest examples have been mixed solid green labels and white labels with green stripe

                   |Solid Yellow|Solid Green| Solid Red |White/Green|
        ROM | Board|  "A" 2532  |  "B" 2732 |  "A" 2532 | "B" 2732  |
         ## | Loc. |  label #   |  label #  |  label #  |  label #  |
        ----+------+------------+-----------+-----------+-----------+
          1 |  E4  | 16-3006-1  |  3006-13  |  3006-28  |  3006-52  |
          2 |  C4  | 16-3006-2  |  3006-14  |  3006-29  |  3006-53  |
          3 |  A4  | 16-3006-3  |  3006-15  |  3006-30  |  3006-54  |
          4 |  E5  | 16-3006-4  |  3006-16  |  3006-31  |  3006-55  |
          5 |  C5  | 16-3006-5  |  3006-17  |  3006-32  |  3006-56  |
          6 |  A5  | 16-3006-6  |  3006-18  |  3006-33  |  3006-57  |
          7 |  E6  | 16-3006-7  |  3006-19  |  3006-34  |  3006-58  |
          8 |  C6  | 16-3006-8  |  3006-20  |  3006-35  |  3006-59  |
          9 |  A6  | 16-3006-9  |  3006-21  |  3006-36  |  3006-60  |
         10 |  A7  | 16-3006-10 |  3006-22  |  3006-37  |  3006-61  |
         11 |  C7  | 16-3006-11 |  3006-23  |  3006-38  |  3006-62  |
         12 |  E7  | 16-3006-12 |  3006-24  |  3006-39  |  3006-63  |
        ----+------+------------+-----------+-----------+-----------+

            Solid Yellow labeled ROMs      |      Solid Green labeled ROMs
        Part Number       ROM     Number   |  Part Number       ROM     Number
        -----------------------------------------------------------------------
        A-5343-09961-A   ROM 1A   3006-1   |  A-5343-09961-B   ROM 1B   3006-13
        A-5343-09962-A   ROM 2A   3006-2   |  A-5343-09962-B   ROM 2B   3006-14
        A-5343-09963-A   ROM 3A   3006-3   |  A-5343-09963-B   ROM 3B   3006-15
        A-5343-09964-A   ROM 4A   3006-4   |  A-5343-09964-B   ROM 4B   3006-16
        A-5343-09965-A   ROM 5A   3006-5   |  A-5343-09965-B   ROM 5B   3006-17
        A-5343-09966-A   ROM 6A   3006-6   |  A-5343-09966-B   ROM 6B   3006-18
        A-5343-09967-A   ROM 7A   3006-7   |  A-5343-10150-B   ROM 7B   3006-19 <-- Revised code with a completly different part number
        A-5343-09968-A   ROM 8A   3006-8   |  A-5343-09968-B   ROM 8B   3006-20
        A-5343-09969-A   ROM 9A   3006-9   |  A-5343-09969-B   ROM 9B   3006-21
        A-5343-09970-A   ROM 10A  3006-10  |  A-5343-10153-B   ROM 10B  3006-22 <-- Revised code with a completly different part number
        A-5343-09971-A   ROM 11A  3006-11  |  A-5343-09971-B   ROM 11B  3006-23
        A-5343-09972-A   ROM 12A  3006-12  |  A-5343-09972-B   ROM 12B  3006-24

        Joust Manual Amendment No. 16P-3006-101-AMD-1 October 1982:
          - Current JOUST games use green-label ROMs.  Earlier games have either yellow or red-labels ROMs, which are interchangable
              and may be mixed in the same game. DO NOT attempt to mix green-label ROMs with red or yellow label ROMs.
          - Boards with green-label ROMs should include jumper W1 and W3 only. Boards with red or yellow label ROMs subsitute jumpers W2 and W4

        ROMs changed in October 1982 as Instruction Manuals 16P-3006-101-T September 1982 & 16P-3006-101 October 1982 only mention Yellow-label ROMs.
          Only the 16P-3006-101-AMD-1 October 1982 Amendment and the 16P-3006-101 Revision A December 1982 manuals mention the new green label ROMs

        The "White labels with Green stripe" set (ROMs 3006-52 through 3006-63) contains the same data as the "Green label" set (ROMs 3006-13 through 3006-24).

        D-9144-3006 ROM Board Assembly:
        +----------------------------------------------+
        |       2J3                           2J4      |
        |              +---------------+               |
        |              | 6821 PIA @ 1B |               |
        |2             +---------------+             L |
        |J   4049BP    7420N       7474     SN7425N  E |
        |2     7474    74LS139N    7411PC   SN7404N  D |
        | +----------+    +----------+    +----------+ |
        | | ROM3  4A |    | ROM2  4C |    | ROM1  4E | |
        | +----------+    +----------+    +----------+ |
        | +----------+    +----------+    +----------+ |
        | | ROM6  5A |WW  | ROM5  5C |WW  | ROM4  5E | |
        | +----------+12  +----------+34  +----------+ |
        | +----------+    +----------+    +----------+ |
        | | ROM9  6A |    | ROM8  6C |    | ROM7  6E | |
        | +----------+    +----------+    +----------+ |
        | +----------+    +----------+    +----------+ |
        | | ROM10 7A |    | ROM11 7C |    | ROM12 7E | |
        | +----------+    +----------+    +----------+ |
        | +------------------+            +----------+ |
        | | VTI 8220  VL2001 |  74LS245N  |  74154N  | |
        | +------------------+            +----------+ |
        | +------------------+  74LS244N               |
        | | VTI 8220  VL2001 |    +------------------+ |
        | +------------------+    |  2J1  connector  | |
        +-------------------------+------------------+-+

        Connectors:
        2J1 40 pin ribbon cable connetor
        2J2  6 pin header (KEY pin 4)
        2J3 10 pin header (KEY pin 9)
        2J4  9 pin header (KEY pin 1)

        LED - 7Seg LED display

        Wire W1 & W3 with Zero Ohm resistors for 2732 ROMs
        Wire W2 & W4 with Zero Ohm resistors for 2532 ROMs

        */
        //ROM_START( joust ) /* Solid green labels - contains the same data as the white label with green stripe 3006-52 through 3006-63 set */
        static readonly tiny_rom_entry [] rom_joust =
        {
            ROM_REGION( 0x19000, "maincpu", 0 ),
            ROM_LOAD( "joust_rom_10b_3006-22.a7", 0x0d000, 0x1000, CRC("3f1c4f89") + SHA1("90864a8ab944df45287bf0f68ad3a85194077a82") ),
            ROM_LOAD( "joust_rom_11b_3006-23.c7", 0x0e000, 0x1000, CRC("ea48b359") + SHA1("6d38003d56bebeb1f5b4d2287d587342847aa195") ), // == joust_rom_11a_3006-11.c7
            ROM_LOAD( "joust_rom_12b_3006-24.e7", 0x0f000, 0x1000, CRC("c710717b") + SHA1("7d01764e8251c60b3cab96f7dc6dcc1c624f9d12") ), // == joust_rom_12a_3006-12.e7
            ROM_LOAD( "joust_rom_1b_3006-13.e4",  0x10000, 0x1000, CRC("fe41b2af") + SHA1("0443e00ae2eb3e66cf805562ee04309487bb0ba4") ), // == joust_rom_1a_3006-1.e4
            ROM_LOAD( "joust_rom_2b_3006-14.c4",  0x11000, 0x1000, CRC("501c143c") + SHA1("5fda266d43cbbf42eeae1a078b5209d9408ab99f") ), // == joust_rom_2a_3006-2.c4
            ROM_LOAD( "joust_rom_3b_3006-15.a4",  0x12000, 0x1000, CRC("43f7161d") + SHA1("686da120aa4bd4a41f3d93e8c79ebb343977851a") ), // == joust_rom_3a_3006-3.a4
            ROM_LOAD( "joust_rom_4b_3006-16.e5",  0x13000, 0x1000, CRC("db5571b6") + SHA1("cb1c3285344e2cfbe0a81ab9b51758c40da8a23f") ), // == joust_rom_4a_3006-4.e5
            ROM_LOAD( "joust_rom_5b_3006-17.c5",  0x14000, 0x1000, CRC("c686bb6b") + SHA1("d9cac4c46820e1a451a145864bca7a35cfab7d37") ), // == joust_rom_5a_3006-5.c5
            ROM_LOAD( "joust_rom_6b_3006-18.a5",  0x15000, 0x1000, CRC("fac5f2cf") + SHA1("febaa8cf5c3a0af901cd12d0b7909f6fec3beadd") ), // == joust_rom_6a_3006-6.a5
            ROM_LOAD( "joust_rom_7b_3006-19.e6",  0x16000, 0x1000, CRC("81418240") + SHA1("5ad14aa65e71c3856dcdb04c99edda92e406a3e3") ),
            ROM_LOAD( "joust_rom_8b_3006-20.c6",  0x17000, 0x1000, CRC("ba5359ba") + SHA1("f4ee13d5a95ed3e1050a3927a3a0ccf86ed7752d") ), // == joust_rom_8a_3006-8.c6
            ROM_LOAD( "joust_rom_9b_3006-21.a6",  0x18000, 0x1000, CRC("39643147") + SHA1("d95d3b746133eac9dcc9ee05eabecb797023f1a5") ), // == joust_rom_9a_3006-9.a6

            ROM_REGION( 0x10000, "soundcpu", 0 ),
            ROM_LOAD( "video_sound_rom_4_std_780.ic12", 0xf000, 0x1000, CRC("f1835bdd") + SHA1("af7c066d2949d36b87ea8c425ca7d12f82b5c653") ), // P/N A-5343-09973

            ROM_REGION( 0x0400, "proms", 0 ),
            ROM_LOAD( "decoder_rom_4.3g", 0x0000, 0x0200, CRC("e6631c23") + SHA1("9988723269367fb44ef83f627186a1c88cf7877e") ), // Universal Horizontal decoder ROM - 7641-5 BPROM - P/N A-5342-09694
            ROM_LOAD( "decoder_rom_6.3c", 0x0200, 0x0200, CRC("83faf25e") + SHA1("30002643d08ed983a6701a7c4b5ee74a2f4a1adb") ), // Universal Vertical decoder ROM - 7641-5 BPROM - P/N A-5342-09821

            ROM_END,
        };


        //ROM_START( jousty ) /* Solid yellow labels */

        //ROM_START( joustr ) /* Solid red labels */

        /*

        Bubbles ROM labels are in this format:

        +--------------------+
        | BUBBLES ROM 1B     |   <-- Game name, ROM board number and ROM type (B is 2732)
        | (c) 1983 WILLIAMS  |
        | ELECTRONICS, INC.  |
        |    16-3012-1       |   <-- Williams game number & ROM number
        +--------------------+

        +--------------------+
        | Video Sound Rom 5  |
        | (c) 1982 WILLIAMS  |
        | ELECTRONICS        |
        |         STD. 771   |
        +--------------------+

        ROM | Board|  "B" ROMs  |
         ## | Loc. |  label #   |  Part Number
        ----+------+------------+----------------
          1 |  E4  | 16-3012-1  |  A-5343-10111-B
          2 |  C4  | 16-3012-2  |  A-5343-10112-B
          3 |  A4  | 16-3012-3  |  A-5343-10113-B
          4 |  E5  | 16-3012-4  |  A-5343-10114-B
          5 |  C5  | 16-3012-5  |  A-5343-10115-B
          6 |  A5  | 16-3012-6  |  A-5343-10116-B
          7 |  E6  | 16-3012-7  |  A-5343-10117-B
          8 |  C6  | 16-3012-8  |  A-5343-10118-B
          9 |  A6  | 16-3012-9  |  A-5343-10119-B
         10 |  A7  | 16-3012-10 |  A-5343-10120-B
         11 |  C7  | 16-3012-11 |  A-5343-10121-B
         12 |  E7  | 16-3012-12 |  A-5343-10122-B

        Instruction Manual 16-3012-101 states Brown labels

        Observed, but currently unverified, sets include:
          Red Label "B" ROMs numbers 16-3012-13 through 16-3012-24
          Red Label "B" ROMs numbers 16-3012-52 through 16-3012-63


        D-9144-3012 ROM Board Assembly:
        +----------------------------------------------+
        |       2J3                           2J4      |
        |              +---------------+               |
        |              | 6821 PIA @ 1B |               |
        |2             +---------------+             L |
        |J   4049BP    7420N       7474     SN7425N  E |
        |2     7474    74LS139N    7411PC   SN7404N  D |
        | +----------+    +----------+    +----------+ |
        | | ROM3  4A |    | ROM2  4C |    | ROM1  4E | |
        | +----------+    +----------+    +----------+ |
        | +----------+    +----------+    +----------+ |
        | | ROM6  5A |WW  | ROM5  5C |WW  | ROM4  5E | |
        | +----------+12  +----------+34  +----------+ |
        | +----------+    +----------+    +----------+ |
        | | ROM9  6A |    | ROM8  6C |    | ROM7  6E | |
        | +----------+    +----------+    +----------+ |
        | +----------+    +----------+    +----------+ |
        | | ROM10 7A |    | ROM11 7C |    | ROM12 7E | |
        | +----------+    +----------+    +----------+ |
        | +------------------+            +----------+ |
        | | VTI 8220  VL2001 |  74LS245N  |  74154N  | |
        | +------------------+            +----------+ |
        | +------------------+  74LS244N               |
        | | VTI 8220  VL2001 |    +------------------+ |
        | +------------------+    |  2J1  connector  | |
        +-------------------------+------------------+-+

        Connectors:
        2J1 40 pin ribbon cable connetor
        2J2  6 pin header (KEY pin 4)
        2J3 10 pin header (KEY pin 9)
        2J4  9 pin header (KEY pin 1)

        LED - 7Seg LED display

        Wire W1 & W3 with Zero Ohm resistors for 2732 ROMs
        Wire W2 & W4 with Zero Ohm resistors for 2532 ROMs


        For the sound ROM:
          Instruction Manual 16-3012-101 states "ROM 13" P/N A-5342-10127 (same as Splat)
          Drawing Set 16-3012-103 states "Video Sound ROM 8"

        */
        //ROM_START( bubbles )

        //ROM_START( bubblesr )

        //ROM_START( bubblesp )

        /*

        Splat! ROM labels are in this format:

        +--------------------+
        | SPLAT ROM 1B       |   <-- Game name, ROM board number and ROM type (B is 2732)
        | (c) 1983 WILLIAMS  |
        | ELECTRONICS, INC.  |
        |    16-3011-1       |   <-- Williams game number & ROM number
        +--------------------+

        ROM | Board|  "B" ROMs  |
         ## | Loc. |  label #   |  Part Number
        ----+------+------------+----------------
          1 |  E4  | 16-3011-1  |  A-5343-10071-B
          2 |  C4  | 16-3011-2  |  A-5343-10072-B
          3 |  A4  | 16-3011-3  |  A-5343-10073-B
          4 |  E5  | 16-3011-4  |  A-5343-10074-B
          5 |  C5  | 16-3011-5  |  A-5343-10075-B
          6 |  A5  | 16-3011-6  |  A-5343-10076-B
          7 |  E6  | 16-3011-7  |  A-5343-10077-B
          8 |  C6  | 16-3011-8  |  A-5343-10078-B
          9 |  A6  | 16-3011-9  |  A-5343-10079-B
         10 |  A7  | 16-3011-10 |  A-5343-10080-B
         11 |  C7  | 16-3011-11 |  A-5343-10081-B
         12 |  E7  | 16-3011-12 |  A-5343-10082-B

        Uses a standard D-9144 ROM Board Assembly, see Joust or Robotron above

        */
        //ROM_START( splat ) /* Solid Brown labels */

        /*
        Sinistar

        Multiple different ROM boards are known to exist:

        Rev.3:
         ROM board known to come with final production rev. 2 labels with corresponding part numbers and two rev. 3
         upgrade ROMs: white labels and orange stripe:

            SINISTAR          SINISTAR
            ROM 8-B    and    ROM 11-B
            REV. 3            REV. 3

         ROM board known to come with all rev. 2 upgrade ROMs (as described below): white labels and red stripe plus the
         two rev. 3 upgrade ROMs as described above

        Rev.2:
         Earlier ROM boards known to have all upgrade styled labels, white with red stripe, in the format:

          SINISTAR
          ROM  1-B
          REV. 2

        Although not currently dumped, "true" rev.1 Sinistar ROMs are believed to be numbered 16-3004-23 through 16-3004-33,
          with speech ROMs 16-3004-34 through 16-3004-37  (labels believed to be solid brown)
        There is known to be a "perfect" version of Sinistar, that being the original version presented to Williams by the
          dev team. The dev team thought this version had the best game play while Williams decided it was too easy (IE: it
          could be played too long on one quarter)

        */
        //ROM_START( sinistar ) // rev. 3
        static readonly tiny_rom_entry [] rom_sinistar =
        {
            ROM_REGION( 0x19000, "maincpu", 0 ), // solid RED labels with final production part numbers
            ROM_LOAD( "sinistar_rom_10-b_16-3004-62.4c", 0x0e000, 0x1000, CRC("3d670417") + SHA1("81802622bee8dbea5c0f08019d87d941dcdbe292") ),
            ROM_LOAD( "sinistar_rom_11-b_16-3004-63.4a", 0x0f000, 0x1000, CRC("3162bc50") + SHA1("2f38e572ab9c731e38dfe9bad3cc8222a775c5ea") ),
            ROM_LOAD( "sinistar_rom_1-b_16-3004-53.1d",  0x10000, 0x1000, CRC("f6f3a22c") + SHA1("026d8cab07734fa294a5645edbe65a904bcbc302") ),
            ROM_LOAD( "sinistar_rom_2-b_16-3004-54.1c",  0x11000, 0x1000, CRC("cab3185c") + SHA1("423d1e3b0c07333ec582529bc4d0b7baf591820a") ),
            ROM_LOAD( "sinistar_rom_3-b_16-3004-55.1a",  0x12000, 0x1000, CRC("1ce1b3cc") + SHA1("5bc03d7249529d827dc60c087e074ab3e4ea7361") ),
            ROM_LOAD( "sinistar_rom_4-b_16-3004-56.2d",  0x13000, 0x1000, CRC("6da632ba") + SHA1("72c0c3d5a5ca87ca4d95fcedaf834206e4633950") ),
            ROM_LOAD( "sinistar_rom_5-b_16-3004-57.2c",  0x14000, 0x1000, CRC("b662e8fc") + SHA1("828a89d2ea13d8a362dae708f86bff54cb231887") ),
            ROM_LOAD( "sinistar_rom_6-b_16-3004-58.2a",  0x15000, 0x1000, CRC("2306183d") + SHA1("703e29e6446856615760a4897c0f5d79cc7bdfb2") ),
            ROM_LOAD( "sinistar_rom_7-b_16-3004-59.3d",  0x16000, 0x1000, CRC("e5dd918e") + SHA1("bf4e2ada6a59d246218544d822ba5355da925924") ),
            ROM_LOAD( "sinistar_rom_8-b_16-3004-60.3c",  0x17000, 0x1000, CRC("4785a787") + SHA1("8c7eca656b2c23b0da41a8c7ce51a2735cab85a4") ),
            ROM_LOAD( "sinistar_rom_9-b_16-3004-61.3a",  0x18000, 0x1000, CRC("50cb63ad") + SHA1("96e28e4fef98fff2649741a266fa590e0313e3b0") ),

            ROM_REGION( 0x10000, "soundcpu", 0 ),
            ROM_LOAD( "3004_speech_ic7_r1_16-3004-52.ic7", 0xb000, 0x1000, CRC("e1019568") + SHA1("442f4f3ccd2e1db2136d2ffb121ea442921f87ca") ),
            ROM_LOAD( "3004_speech_ic5_r1_16-3004-50.ic5", 0xc000, 0x1000, CRC("cf3b5ffd") + SHA1("d5d51c550581c9d46ab331dd4fd32541a2ef598e") ),
            ROM_LOAD( "3004_speech_ic6_r1_16-3004-51.ic6", 0xd000, 0x1000, CRC("ff8d2645") + SHA1("16fa2a602acbbc182dd96bab113ab18356f3daf0") ),
            ROM_LOAD( "3004_speech_ic4_r1_16-3004-49.ic4", 0xe000, 0x1000, CRC("4b56a626") + SHA1("44430cd5c110ec751b0bfb8ae99b26d443350db1") ),
            ROM_LOAD( "video_sound_rom_9_std.808.ic12",    0xf000, 0x1000, CRC("b82f4ddb") + SHA1("c70c7dd6e88897920d7709a260f27810f66aade1") ),

/*
            ROM_REGION( 0x10000, "soundcpu_b", 0 ) // Stereo sound requires 2nd sound board as used in the cockpit version
            ROM_LOAD( "3004_speech_ic7_r1_16-3004-52.ic7", 0xb000, 0x1000, CRC(e1019568) SHA1(442f4f3ccd2e1db2136d2ffb121ea442921f87ca) )
            ROM_LOAD( "3004_speech_ic5_r1_16-3004-50.ic5", 0xc000, 0x1000, CRC(cf3b5ffd) SHA1(d5d51c550581c9d46ab331dd4fd32541a2ef598e) )
            ROM_LOAD( "3004_speech_ic6_r1_16-3004-51.ic6", 0xd000, 0x1000, CRC(ff8d2645) SHA1(16fa2a602acbbc182dd96bab113ab18356f3daf0) )
            ROM_LOAD( "3004_speech_ic4_r1_16-3004-49.ic4", 0xe000, 0x1000, CRC(4b56a626) SHA1(44430cd5c110ec751b0bfb8ae99b26d443350db1) )
            ROM_LOAD( "video_sound_rom_10_std.ic12",       0xf000, 0x1000, CRC(b5c70082) SHA1(643af087b57da3a71c68372c79c5777e0c1fbef7) ) // not sure if all speech ROMs need to be here too
*/

            ROM_REGION( 0x0400, "proms", 0 ),
            ROM_LOAD( "decoder_rom_4.3g", 0x0000, 0x0200, CRC("e6631c23") + SHA1("9988723269367fb44ef83f627186a1c88cf7877e") ), // Universal Horizontal decoder ROM - 7641-5 BPROM - P/N A-5342-09694
            ROM_LOAD( "decoder_rom_6.3c", 0x0200, 0x0200, CRC("83faf25e") + SHA1("30002643d08ed983a6701a7c4b5ee74a2f4a1adb") ), // Universal Vertical decoder ROM - 7641-5 BPROM - P/N A-5342-09821

            ROM_END,
        };


        //ROM_START( sinistar2 ) // rev. 2

        //ROM_START( sinistarp ) // solid pink labels - 1982 AMOA prototype

        //ROM_START( playball )

        //ROM_START( blaster )

        //ROM_START( blastero )

        //ROM_START( blasterkit )

        //ROM_START( spdball )

        //ROM_START( alienar )

        //ROM_START( alienaru )

        //ROM_START( lottofun )

        //ROM_START( mysticm )

        //ROM_START( mysticmp )

        //ROM_START( tshoot )

        //ROM_START( inferno )

        //ROM_START( joust2 )

        //ROM_START( joust2r1 )
    }


        //void defndjeu_state::driver_start()

        //void mayday_state::driver_start()


    partial class williams : construct_ioport_helper
    {
        static void defender_state_defender(machine_config config, device_t device) { defender_state defender_state = (defender_state)device; defender_state.defender(config); }
        static void williams_state_stargate(machine_config config, device_t device) { williams_state williams_state = (williams_state)device; williams_state.stargate(config); }
        static void williams_state_robotron(machine_config config, device_t device) { williams_state williams_state = (williams_state)device; williams_state.robotron(config); }
        static void williams_muxed_state_joust(machine_config config, device_t device) { williams_muxed_state williams_muxed_state = (williams_muxed_state)device; williams_muxed_state.joust(config); }
        static void sinistar_state_sinistar(machine_config config, device_t device) { sinistar_state sinistar_state = (sinistar_state)device; sinistar_state.sinistar(config); }

        static williams m_williams = new williams();

        static device_t device_creator_defender(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new defender_state(mconfig, type, tag); }
        static device_t device_creator_stargate(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new williams_state(mconfig, type, tag); }
        static device_t device_creator_robotron(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new williams_state(mconfig, type, tag); }
        static device_t device_creator_joust(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new williams_muxed_state(mconfig, type, tag); }
        static device_t device_creator_sinistar(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new sinistar_state(mconfig, type, tag); }

        /*************************************
         *
         *  Game drivers
         *
         *************************************/

        // Defender hardware games
        public static readonly game_driver driver_defender = GAME(device_creator_defender, rom_defender, "1980", "defender", "0", defender_state_defender,    m_williams.construct_ioport_defender, driver_device.empty_init, ROT0,   "Williams",            "Defender (Red label)",              MACHINE_SUPPORTS_SAVE); // developers left Williams in 1981 and formed Vid Kidz


        // Standard Williams hardware
        public static readonly game_driver driver_stargate = GAME(device_creator_stargate, rom_stargate, "1981", "stargate", "0", williams_state_stargate,    m_williams.construct_ioport_stargate, driver_device.empty_init, ROT0,   "Williams / Vid Kidz", "Stargate",                          MACHINE_SUPPORTS_SAVE);

        public static readonly game_driver driver_robotron = GAME(device_creator_robotron, rom_robotron, "1982", "robotron", "0", williams_state_robotron,    m_williams.construct_ioport_robotron, driver_device.empty_init, ROT0,   "Williams / Vid Kidz", "Robotron: 2084 (Solid Blue label)", MACHINE_SUPPORTS_SAVE);

        public static readonly game_driver driver_joust    = GAME(device_creator_joust,    rom_joust,    "1982", "joust",    "0", williams_muxed_state_joust, m_williams.construct_ioport_robotron, driver_device.empty_init, ROT0,   "Williams",            "Joust (Green label)",               MACHINE_SUPPORTS_SAVE);

        public static readonly game_driver driver_sinistar = GAME(device_creator_sinistar, rom_sinistar, "1982", "sinistar", "0", sinistar_state_sinistar,    m_williams.construct_ioport_sinistar, driver_device.empty_init, ROT270, "Williams",            "Sinistar (revision 3)",             MACHINE_SUPPORTS_SAVE);
    }
}
