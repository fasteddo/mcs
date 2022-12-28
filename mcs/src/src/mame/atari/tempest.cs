// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using offs_t = System.UInt32;  //using offs_t = u32;
using u32 = System.UInt32;
using uint8_t = System.Byte;

using static mame.avgdvg_global;
using static mame.disound_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.er2055_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.m6502_global;
using static mame.mathbox_global;
using static mame.pokey_global;
using static mame.rescap_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.vector_global;
using static mame.watchdog_global;


namespace mame
{
    partial class tempest_state : driver_device
    {
        static readonly XTAL MASTER_CLOCK = XTAL_global.op("12.096_MHz_XTAL");
        static readonly XTAL CLOCK_3KHZ   = MASTER_CLOCK / 4096;

        const string TEMPEST_KNOB_P1_TAG = "KNOBP1";
        const string TEMPEST_KNOB_P2_TAG = "KNOBP2";
        const string TEMPEST_BUTTONS_P1_TAG = "BUTTONSP1";
        const string TEMPEST_BUTTONS_P2_TAG = "BUTTONSP2";


        required_device<m6502_device> m_maincpu;
        required_device<mathbox_device> m_mathbox;
        required_device<watchdog_timer_device> m_watchdog;
        required_device<avg_tempest_device> m_avg;
        required_device<er2055_device> m_earom;
        required_region_ptr<uint8_t> m_rom;

        required_ioport m_knob_p1;
        required_ioport m_knob_p2;
        required_ioport m_buttons_p1;
        required_ioport m_buttons_p2;
        required_ioport m_in1;
        required_ioport m_in2;
        output_finder<u32_const_2> m_leds;

        //uint8_t m_player_select;


        public tempest_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_maincpu = new required_device<m6502_device>(this, "maincpu");
            m_mathbox = new required_device<mathbox_device>(this, "mathbox");
            m_watchdog = new required_device<watchdog_timer_device>(this, "watchdog");
            m_avg = new required_device<avg_tempest_device>(this, "avg");
            m_earom = new required_device<er2055_device>(this, "earom");
            m_rom = new required_region_ptr<uint8_t>(this, "maincpu");
            m_knob_p1 = new required_ioport(this, TEMPEST_KNOB_P1_TAG);
            m_knob_p2 = new required_ioport(this, TEMPEST_KNOB_P2_TAG);
            m_buttons_p1 = new required_ioport(this, TEMPEST_BUTTONS_P1_TAG);
            m_buttons_p2 = new required_ioport(this, TEMPEST_BUTTONS_P2_TAG);
            m_in1 = new required_ioport(this, "IN1_DSW0");
            m_in2 = new required_ioport(this, "IN2");
            m_leds = new output_finder<u32_const_2>(this, "led{0}", 0U);
        }


        protected override void machine_start()
        {
            throw new emu_unimplemented();
    #if false
            m_leds.resolve();
            save_item(NAME(m_player_select));
    #endif
        }


#if false
        /*************************************
         *
         *  Interrupt ack
         *
         *************************************/

        void tempest_state::wdclr_w(uint8_t data)
        {
            m_maincpu->set_input_line(0, CLEAR_LINE);
            m_watchdog->watchdog_reset();
        }

        /*************************************
         *
         *  Input ports
         *
         *************************************/

        CUSTOM_INPUT_MEMBER(tempest_state::tempest_knob_r)
        {
            return (m_player_select == 0) ? m_knob_p1->read() : m_knob_p2->read();
        }

        CUSTOM_INPUT_MEMBER(tempest_state::tempest_buttons_r)
        {
            return (m_player_select == 0) ? m_buttons_p1->read() : m_buttons_p2->read();
        }


        READ_LINE_MEMBER(tempest_state::clock_r)
        {
            /* Emulate the 3kHz source on bit 7 (divide 1.5MHz by 512) */
            return (m_maincpu->total_cycles() & 0x100) ? 1 : 0;
        }
#endif


        uint8_t input_port_1_bit_r(offs_t offset)
        {
            throw new emu_unimplemented();
#if false
            return (m_in1->read() & (1 << offset)) ? 0 : 228;
#endif
        }


        uint8_t input_port_2_bit_r(offs_t offset)
        {
            throw new emu_unimplemented();
#if false
            return (m_in2->read() & (1 << offset)) ? 0 : 228;
#endif
        }


#if false
        /*************************************
         *
         *  Output ports
         *
         *************************************/

        void tempest_state::tempest_led_w(uint8_t data)
        {
            m_leds[0] = BIT(~data, 1);
            m_leds[1] = BIT(~data, 0);
            /* FLIP is bit 0x04 */
            m_player_select = data & 0x04;
        }


        void tempest_state::tempest_coin_w(uint8_t data)
        {
            machine().bookkeeping().coin_counter_w(0, (data & 0x01));
            machine().bookkeeping().coin_counter_w(1, (data & 0x02));
            machine().bookkeeping().coin_counter_w(2, (data & 0x04));
            m_avg->set_flip_x(data & 0x08);
            m_avg->set_flip_y(data & 0x10);
        }



        /*************************************
         *
         *  High score EAROM
         *
         *************************************/

        uint8_t tempest_state::earom_read()
        {
            return m_earom->data();
        }

        void tempest_state::earom_write(offs_t offset, uint8_t data)
        {
            m_earom->set_address(offset & 0x3f);
            m_earom->set_data(data);
        }

        void tempest_state::earom_control_w(uint8_t data)
        {
            // CK = EDB0, C1 = /EDB2, C2 = EDB1, CS1 = EDB3, /CS2 = GND
            m_earom->set_control(BIT(data, 3), 1, !BIT(data, 2), BIT(data, 1));
            m_earom->set_clk(BIT(data, 0));
        }



        /*************************************
         *
         *  Main CPU memory handlers
         *
         *************************************/

        uint8_t tempest_state::rom_ae1f_r()
        {
            // This is needed to ensure that the routine starting at ae1c passes checks and does not corrupt data;
            // config.m_perfect_cpu_quantum = subtag("maincpu"); would be very taxing on this driver.
            machine().scheduler().perfect_quantum(attotime::from_usec(100));
            machine().scheduler().abort_timeslice();

            return m_rom[0xae1f];
        }
#endif


        void main_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map(0x0000, 0x07ff).ram();
            map(0x0800, 0x080f).writeonly().share("avg:colorram");
            map(0x0c00, 0x0c00).portr("IN0");
            map(0x0d00, 0x0d00).portr("DSW1");
            map(0x0e00, 0x0e00).portr("DSW2");
            map(0x2000, 0x2fff).ram();
            map(0x3000, 0x3fff).rom().region("vectorrom", 0);
            map(0x4000, 0x4000).w(FUNC(tempest_state::tempest_coin_w));
            map(0x4800, 0x4800).w(m_avg, FUNC(avg_device::go_w));
            map(0x5000, 0x5000).w(FUNC(tempest_state::wdclr_w));
            map(0x5800, 0x5800).w(m_avg, FUNC(avg_device::reset_w));
            map(0x6000, 0x603f).w(FUNC(tempest_state::earom_write));
            map(0x6040, 0x6040).r(m_mathbox, FUNC(mathbox_device::status_r)).w(FUNC(tempest_state::earom_control_w));
            map(0x6050, 0x6050).r(FUNC(tempest_state::earom_read));
            map(0x6060, 0x6060).r(m_mathbox, FUNC(mathbox_device::lo_r));
            map(0x6070, 0x6070).r(m_mathbox, FUNC(mathbox_device::hi_r));
            map(0x6080, 0x609f).w(m_mathbox, FUNC(mathbox_device::go_w));
            map(0x60c0, 0x60cf).rw("pokey1", FUNC(pokey_device::read), FUNC(pokey_device::write));
            map(0x60d0, 0x60df).rw("pokey2", FUNC(pokey_device::read), FUNC(pokey_device::write));
            map(0x60e0, 0x60e0).w(FUNC(tempest_state::tempest_led_w));
            map(0x9000, 0xdfff).rom();
            map(0xae1f, 0xae1f).r(FUNC(tempest_state::rom_ae1f_r));
            map(0xf000, 0xffff).rom(); /* for the reset / interrupt vectors */
#endif
        }
    }


    public partial class tempest : construct_ioport_helper
    {
        /*************************************
         *
         *  Port definitions
         *
         *************************************/

        //static INPUT_PORTS_START( tempest )
        void construct_ioport_tempest(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            throw new emu_unimplemented();
#if false
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0")
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_COIN3 )
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_COIN2 )
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_COIN1 )
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_TILT )
            PORT_SERVICE( 0x10, IP_ACTIVE_LOW )
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_SERVICE1 ) PORT_NAME("Diagnostic Step")
            /* bit 6 is the VG HALT bit. We set it to "low" */
            /* per default (busy vector processor). */
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_CUSTOM ) PORT_READ_LINE_DEVICE_MEMBER("avg", avg_device, done_r)
            /* bit 7 is tied to a 3kHz (?) clock */
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_CUSTOM ) PORT_READ_LINE_MEMBER(tempest_state, clock_r)

            PORT_START("IN1_DSW0")
            PORT_BIT( 0x0f, IP_ACTIVE_HIGH, IPT_CUSTOM ) PORT_CUSTOM_MEMBER(tempest_state, tempest_knob_r)
            /* The next one is reponsible for cocktail mode.
             * According to the documentation, this is not a switch, although
             * it may have been planned to put it on the Math Box PCB, D/E2 )
             */
            PORT_DIPNAME( 0x10, 0x10, DEF_STR( Cabinet ) )
            PORT_DIPSETTING(    0x10, DEF_STR( Upright ) )
            PORT_DIPSETTING(    0x00, DEF_STR( Cocktail ) )
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_UNKNOWN )
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_UNKNOWN )
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_UNKNOWN )

            PORT_START("IN2")
            PORT_DIPNAME(  0x03, 0x03, DEF_STR( Difficulty ) ) PORT_DIPLOCATION("DE2:4,3")
            PORT_DIPSETTING(     0x02, DEF_STR( Easy ) )
            PORT_DIPSETTING(     0x03, "Medium1" )
            PORT_DIPSETTING(     0x00, "Medium2" )
            PORT_DIPSETTING(     0x01, DEF_STR( Hard ) )
            PORT_DIPNAME(  0x04, 0x04, "Rating" ) PORT_DIPLOCATION("DE2:2")
            PORT_DIPSETTING(     0x04, "1, 3, 5, 7, 9" )
            PORT_DIPSETTING(     0x00, "tied to high score" )
            PORT_BIT(0x18, IP_ACTIVE_HIGH, IPT_CUSTOM ) PORT_CUSTOM_MEMBER(tempest_state, tempest_buttons_r)
            PORT_BIT(0x20, IP_ACTIVE_LOW, IPT_START1 )
            PORT_BIT(0x40, IP_ACTIVE_LOW, IPT_START2 )
            PORT_BIT(0x80, IP_ACTIVE_LOW, IPT_UNKNOWN )

            PORT_START("DSW1")  /* N13 on analog vector generator PCB */
            PORT_DIPNAME( 0x03, 0x00, DEF_STR( Coinage ) ) PORT_DIPLOCATION("N13:8,7")
            PORT_DIPSETTING(    0x01, DEF_STR( 2C_1C ) )
            PORT_DIPSETTING(    0x00, DEF_STR( 1C_1C ) )
            PORT_DIPSETTING(    0x03, DEF_STR( 1C_2C ) )
            PORT_DIPSETTING(    0x02, DEF_STR( Free_Play ) )
            PORT_DIPNAME( 0x0c, 0x00, "Right Coin" ) PORT_DIPLOCATION("N13:6,5")
            PORT_DIPSETTING(    0x00, "*1" )
            PORT_DIPSETTING(    0x04, "*4" )
            PORT_DIPSETTING(    0x08, "*5" )
            PORT_DIPSETTING(    0x0c, "*6" )
            PORT_DIPNAME( 0x10, 0x00, "Left Coin" ) PORT_DIPLOCATION("N13:4")
            PORT_DIPSETTING(    0x00, "*1" )
            PORT_DIPSETTING(    0x10, "*2" )
            PORT_DIPNAME( 0xe0, 0x00, "Bonus Coins" ) PORT_DIPLOCATION("N13:3,2,1")
            PORT_DIPSETTING(    0x00, DEF_STR( None ) )
            PORT_DIPSETTING(    0x80, "1 each 5" )
            PORT_DIPSETTING(    0x40, "1 each 4 (+Demo)" )
            PORT_DIPSETTING(    0xa0, "1 each 3" )
            PORT_DIPSETTING(    0x60, "2 each 4 (+Demo)" )
            PORT_DIPSETTING(    0x20, "1 each 2" )
            PORT_DIPSETTING(    0xc0, "Freeze Mode" )
            PORT_DIPSETTING(    0xe0, "Freeze Mode" )

            PORT_START("DSW2")  /* L12 on analog vector generator PCB */
            PORT_DIPNAME( 0x01, 0x00, "Minimum" ) PORT_DIPLOCATION("L12:8")
            PORT_DIPSETTING(    0x00, "1 Credit" )
            PORT_DIPSETTING(    0x01, "2 Credit" )
            PORT_DIPNAME( 0x06, 0x00, DEF_STR( Language ) ) PORT_DIPLOCATION("L12:7,6")
            PORT_DIPSETTING(    0x00, DEF_STR( English ) )
            PORT_DIPSETTING(    0x02, DEF_STR( French ) )
            PORT_DIPSETTING(    0x04, DEF_STR( German ) )
            PORT_DIPSETTING(    0x06, DEF_STR( Spanish ) )
            PORT_DIPNAME( 0x38, 0x00, DEF_STR( Bonus_Life ) ) PORT_DIPLOCATION("L12:5,4,3")
            PORT_DIPSETTING(    0x08, "10000" )
            PORT_DIPSETTING(    0x00, "20000" )
            PORT_DIPSETTING(    0x10, "30000" )
            PORT_DIPSETTING(    0x18, "40000" )
            PORT_DIPSETTING(    0x20, "50000" )
            PORT_DIPSETTING(    0x28, "60000" )
            PORT_DIPSETTING(    0x30, "70000" )
            PORT_DIPSETTING(    0x38, DEF_STR( None ) )
            PORT_DIPNAME( 0xc0, 0x00, DEF_STR( Lives ) ) PORT_DIPLOCATION("L12:2,1")
            PORT_DIPSETTING(    0xc0, "2" )
            PORT_DIPSETTING(    0x00, "3" )
            PORT_DIPSETTING(    0x40, "4" )
            PORT_DIPSETTING(    0x80, "5" )

            PORT_START(TEMPEST_KNOB_P1_TAG)
            /* This is the Tempest spinner input. It only uses 4 bits. */
            PORT_BIT( 0x0f, 0x00, IPT_DIAL ) PORT_SENSITIVITY(100) PORT_KEYDELTA(20) PORT_PLAYER(1) PORT_FULL_TURN_COUNT(72)
            PORT_BIT( 0xf0, IP_ACTIVE_LOW, IPT_UNUSED )

            PORT_START(TEMPEST_KNOB_P2_TAG)
            /* This is the Tempest spinner input. It only uses 4 bits. */
            PORT_BIT( 0x0f, 0x00, IPT_DIAL ) PORT_SENSITIVITY(100) PORT_KEYDELTA(20) PORT_PLAYER(2) PORT_FULL_TURN_COUNT(72)
            PORT_BIT( 0xf0, IP_ACTIVE_LOW, IPT_UNUSED )

            PORT_START(TEMPEST_BUTTONS_P1_TAG)
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_BUTTON2 ) PORT_PLAYER(1)
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON1 ) PORT_PLAYER(1)
            PORT_BIT( 0xfc, IP_ACTIVE_LOW, IPT_UNUSED )

            PORT_START(TEMPEST_BUTTONS_P2_TAG)
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_BUTTON2 ) PORT_PLAYER(2)
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON1 ) PORT_PLAYER(2)
            PORT_BIT( 0xfc, IP_ACTIVE_LOW, IPT_UNUSED )

            INPUT_PORTS_END
#endif
        }
    }


    partial class tempest_state : driver_device
    {
        /*************************************
         *
         *  Machine drivers
         *
         *************************************/

        public void tempest(machine_config config)
        {
            /* basic machine hardware */
            M6502(config, m_maincpu, MASTER_CLOCK / 8);
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, main_map);
            m_maincpu.op0.execute().set_periodic_int(irq0_line_assert, attotime.from_hz(CLOCK_3KHZ / 12));

            WATCHDOG_TIMER(config, m_watchdog).set_time(attotime.from_hz(CLOCK_3KHZ / 256));

            ER2055(config, m_earom);

            /* video hardware */
            VECTOR(config, "vector");
            screen_device screen = SCREEN(config, "screen", SCREEN_TYPE_VECTOR);
            screen.set_refresh_hz(60);
            screen.set_size(400, 300);
            screen.set_visarea(0, 580, 0, 570);
            screen.set_screen_update("vector", (screen_device screen2, bitmap_rgb32 bitmap, rectangle cliprect) => { return ((vector_device)subdevice("vector")).screen_update(screen2, bitmap, cliprect); });

            AVG_TEMPEST(config, m_avg, 0);
            m_avg.op0.set_vector("vector");
            m_avg.op0.set_memory(m_maincpu, AS_PROGRAM, 0x2000);

            /* Drivers */
            MATHBOX(config, m_mathbox, 0);

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            pokey_device pokey1 = POKEY(config, "pokey1", MASTER_CLOCK / 8);
            pokey1.pot_r<u32_const_0>().set(input_port_1_bit_r).reg();
            pokey1.pot_r<u32_const_1>().set(input_port_1_bit_r).reg();
            pokey1.pot_r<u32_const_2>().set(input_port_1_bit_r).reg();
            pokey1.pot_r<u32_const_3>().set(input_port_1_bit_r).reg();
            pokey1.pot_r<u32_const_4>().set(input_port_1_bit_r).reg();
            pokey1.pot_r<u32_const_5>().set(input_port_1_bit_r).reg();
            pokey1.pot_r<u32_const_6>().set(input_port_1_bit_r).reg();
            pokey1.pot_r<u32_const_7>().set(input_port_1_bit_r).reg();
            pokey1.set_output_rc(RES_K(10), CAP_U(0.015), 5.0);
            pokey1.disound.add_route(ALL_OUTPUTS, "mono", 0.5);

            pokey_device pokey2 = POKEY(config, "pokey2", MASTER_CLOCK / 8);
            pokey2.pot_r<u32_const_0>().set(input_port_2_bit_r).reg();
            pokey2.pot_r<u32_const_1>().set(input_port_2_bit_r).reg();
            pokey2.pot_r<u32_const_2>().set(input_port_2_bit_r).reg();
            pokey2.pot_r<u32_const_3>().set(input_port_2_bit_r).reg();
            pokey2.pot_r<u32_const_4>().set(input_port_2_bit_r).reg();
            pokey2.pot_r<u32_const_5>().set(input_port_2_bit_r).reg();
            pokey2.pot_r<u32_const_6>().set(input_port_2_bit_r).reg();
            pokey2.pot_r<u32_const_7>().set(input_port_2_bit_r).reg();
            pokey2.set_output_rc(RES_K(10), CAP_U(0.015), 5.0);
            pokey2.disound.add_route(ALL_OUTPUTS, "mono", 0.5);
        }
    }


    partial class tempest : construct_ioport_helper
    {
        /*************************************
         *
         *  ROM definitions
         *
         *************************************/

        //ROM_START( tempest ) /* rev 3 */
        static readonly tiny_rom_entry [] rom_tempest =
        {
            /* Roms are for Tempest Analog Vector-Generator PCB Assembly A037383-03 or A037383-04 */
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "136002-133.d1",  0x9000, 0x1000, CRC("1d0cc503") + SHA1("7bef95db9b1102d6b1166bda0ccb276ef4cc3764") ), /* 136002-113 + 136002-114 */
            ROM_LOAD( "136002-134.f1",  0xa000, 0x1000, CRC("c88e3524") + SHA1("89144baf1efc703b2336774793ce345b37829ee7") ), /* 136002-115 + 136002-316 */
            ROM_LOAD( "136002-235.j1",  0xb000, 0x1000, CRC("a4b2ce3f") + SHA1("a5f5fb630a48c5d25346f90d4c13aaa98f60b228") ), /* 136002-217 + 136002-118 */
            ROM_LOAD( "136002-136.lm1", 0xc000, 0x1000, CRC("65a9a9f9") + SHA1("73aa7d6f4e7093ccb2d97f6344f354872bcfd72a") ), /* 136002-119 + 136002-120 */
            ROM_LOAD( "136002-237.p1",  0xd000, 0x1000, CRC("de4e9e34") + SHA1("04be074e45bf5cd95a852af97cd04e35b7f27fc4") ), /* 136002-121 + 136002-222 */
            ROM_RELOAD(                 0xf000, 0x1000 ), /* for reset/interrupt vectors */

            /* Vector ROM */
            ROM_REGION( 0x1000, "vectorrom", 0 ),
            ROM_LOAD( "136002-138.np3", 0x0000, 0x1000, CRC("9995256d") + SHA1("2b725ee1a57d423c7d7377a1744f48412e0f2f69") ),

            /* AVG PROM */
            ROM_REGION( 0x100, "avg:prom", 0 ),
            ROM_LOAD( "136002-125.d7",   0x0000, 0x0100, CRC("5903af03") + SHA1("24bc0366f394ad0ec486919212e38be0f08d0239") ),

            /* Mathbox PROMs */
            ROM_REGION( 0x20, "user2", 0 ),
            ROM_LOAD( "136002-126.a1",   0x0000, 0x0020, CRC("8b04f921") + SHA1("317b3397482f13b2d1bc21f296d3b3f9a118787b") ),

            ROM_REGION32_BE( 0x400, "user3", 0 ),
            ROMX_LOAD( "136002-132.l1", 0, 0x100, CRC("2af82e87") + SHA1("3816835a9ccf99a76d246adf204989d9261bb065"), ROM_NIBBLE | ROM_SHIFT_NIBBLE_LO | ROM_SKIP(3)),
            ROMX_LOAD( "136002-131.k1", 0, 0x100, CRC("b31f6e24") + SHA1("ce5f8ca34d06a5cfa0076b47400e61e0130ffe74"), ROM_NIBBLE | ROM_SHIFT_NIBBLE_HI | ROM_SKIP(3)),
            ROMX_LOAD( "136002-130.j1", 1, 0x100, CRC("8119b847") + SHA1("c4fbaedd4ce1ad6a4128cbe902b297743edb606a"), ROM_NIBBLE | ROM_SHIFT_NIBBLE_LO | ROM_SKIP(3)),
            ROMX_LOAD( "136002-129.h1", 1, 0x100, CRC("09f5a4d5") + SHA1("d6f2ac07ca9ee385c08831098b0dcaf56808993b"), ROM_NIBBLE | ROM_SHIFT_NIBBLE_HI | ROM_SKIP(3)),
            ROMX_LOAD( "136002-128.f1", 2, 0x100, CRC("823b61ae") + SHA1("d99a839874b45f64e14dae92a036e47a53705d16"), ROM_NIBBLE | ROM_SHIFT_NIBBLE_LO | ROM_SKIP(3)),
            ROMX_LOAD( "136002-127.e1", 2, 0x100, CRC("276eadd5") + SHA1("55718cd8ec4bcf75076d5ef0ee1ed2551e19d9ba"), ROM_NIBBLE | ROM_SHIFT_NIBBLE_HI | ROM_SKIP(3)),

            ROM_END,
        };


        //ROM_START( tempest1r ) /* rev 1 */

        //ROM_START( tempest3 ) /* rev 3 */

        //ROM_START( tempest2 ) /* rev 2 */

        //ROM_START( tempest1 ) /* rev 1 */

        //ROM_START( temptube )


        /*************************************
         *
         *  Game drivers
         *
         *************************************/

        static void tempest_state_tempest(machine_config config, device_t device) { tempest_state tempest_state = (tempest_state)device; tempest_state.tempest(config); }

        static tempest m_tempest = new tempest();

        static device_t device_creator_tempest(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new tempest_state(mconfig, type, tag); }

        public static readonly game_driver driver_tempest = GAME(device_creator_tempest, rom_tempest, "1980", "tempest", "0", tempest_state_tempest, m_tempest.construct_ioport_tempest, driver_device.empty_init, ROT270, "Atari", "Tempest (rev 3, Revised Hardware)", MACHINE_SUPPORTS_SAVE );
    }
}
