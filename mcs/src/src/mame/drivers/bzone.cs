// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using u32 = System.UInt32;
using uint8_t = System.Byte;

using static mame.avgdvg_global;
using static mame.diexec_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.m6502_global;
using static mame.mathbox_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.vector_global;
using static mame.watchdog_global;


namespace mame
{
    partial class bzone_state : driver_device
    {
        /*************************************
         *
         *  Save state registration
         *
         *************************************/

        protected override void machine_start()
        {
            save_item(NAME(new { m_analog_data }));
            m_startled.resolve();
        }


        //void redbaron_state::machine_start()

        //void redbaron_state::machine_reset()


        /*************************************
         *
         *  Interrupt handling
         *
         *************************************/

        //INTERRUPT_GEN_MEMBER(bzone_state::bzone_interrupt)
        void bzone_interrupt(device_t device)
        {
            if ((ioport("IN0").read() & 0x10) != 0)
                device.execute().pulse_input_line(INPUT_LINE_NMI, attotime.zero);
        }


        /*************************************
         *
         *  Battlezone input ports
         *
         *************************************/

        //READ_LINE_MEMBER(bzone_state::clock_r)
        public int clock_r()
        {
            return (m_maincpu.op0.total_cycles() & 0x100) != 0 ? 1 : 0;
        }


        void bzone_coin_counter_w(offs_t offset, uint8_t data)
        {
            machine().bookkeeping().coin_counter_w((int)offset, data);
        }


        /*************************************
         *
         *  Red Baron input ports
         *
         *************************************/

        //uint8_t redbaron_state::redbaron_joy_r()

        //void redbaron_state::redbaron_joysound_w(uint8_t data)


        /*************************************
         *
         *  Red Baron EAROM
         *
         *************************************/

        //uint8_t redbaron_state::earom_read()

        //void redbaron_state::earom_write(offs_t offset, uint8_t data)

        //void redbaron_state::earom_control_w(uint8_t data)


        /*************************************
         *
         *  Main CPU memory handlers
         *
         *************************************/
        void bzone_map(address_map map, device_t device)
        {
            map.global_mask(0x7fff);
            map.op(0x0000, 0x03ff).ram();
            map.op(0x0800, 0x0800).portr("IN0");
            map.op(0x0a00, 0x0a00).portr("DSW0");
            map.op(0x0c00, 0x0c00).portr("DSW1");
            map.op(0x1000, 0x1000).w(bzone_coin_counter_w);
            map.op(0x1200, 0x1200).w("avg", (data) => { ((avg_device)subdevice("avg")).go_w(data); });
            map.op(0x1400, 0x1400).w("watchdog", (data) => { ((watchdog_timer_device)subdevice("watchdog")).reset_w(data); });
            map.op(0x1600, 0x1600).w("avg", (data) => { ((avg_device)subdevice("avg")).reset_w(data); });
            map.op(0x1800, 0x1800).r(m_mathbox, () => { return m_mathbox.op0.status_r(); });
            map.op(0x1810, 0x1810).r(m_mathbox, () => { return m_mathbox.op0.lo_r(); });
            map.op(0x1818, 0x1818).r(m_mathbox, () => { return m_mathbox.op0.hi_r(); });
            map.op(0x1820, 0x182f).rw("pokey", (data) => { return ((pokey_device)subdevice("pokey")).read(data); }, (offset, data) => { ((pokey_device)subdevice("pokey")).write(offset, data); });
            map.op(0x1840, 0x1840).w(bzone_sounds_w);
            map.op(0x1860, 0x187f).w(m_mathbox, (offset, data) => { m_mathbox.op0.go_w(offset, data); });
            map.op(0x2000, 0x2fff).ram();
            map.op(0x3000, 0x7fff).rom();
        }


        //void bzone_state::bradley_map(address_map &map)

        //void redbaron_state::redbaron_map(address_map &map)
    }


    public partial class bzone : construct_ioport_helper
    {
        /*************************************
         *
         *  Port definitions
         *
         *************************************/

        //#define BZONEIN0\
        void BZONEIN0(device_t owner)
        {
            bzone_state bzone_state = (bzone_state)owner;

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x0c, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_SERVICE( 0x10, IP_ACTIVE_LOW );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_SERVICE1 ); PORT_NAME("Diagnostic Step");
            /* bit 6 is the VG HALT bit. We set it to "low" */
            /* per default (busy vector processor). */
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_READ_LINE_DEVICE_MEMBER("avg", () => { return ((avg_device)bzone_state.subdevice("avg")).done_r(); });
            /* bit 7 is tied to a 3kHz clock */
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_READ_LINE_MEMBER(bzone_state.clock_r);
        }


        //#define BZONEDSW0\
        void BZONEDSW0()
        {
            PORT_START("DSW0");
            PORT_DIPNAME( 0x03, 0x01, DEF_STR( Lives ) ); PORT_DIPLOCATION("M10:1,2");
            PORT_DIPSETTING(    0x00, "2" );
            PORT_DIPSETTING(    0x01, "3" );
            PORT_DIPSETTING(    0x02, "4" );
            PORT_DIPSETTING(    0x03, "5" );
            PORT_DIPNAME( 0x0c, 0x04, "Missile appears at" ); PORT_DIPLOCATION("M10:3,4");
            PORT_DIPSETTING(    0x00, "5000" );
            PORT_DIPSETTING(    0x04, "10000" );
            PORT_DIPSETTING(    0x08, "20000" );
            PORT_DIPSETTING(    0x0c, "30000" );
            PORT_DIPNAME( 0x30, 0x10, DEF_STR( Bonus_Life ) ); PORT_DIPLOCATION("M10:5,6");
            PORT_DIPSETTING(    0x10, "15k and 100k" );
            PORT_DIPSETTING(    0x20, "25k and 100k" );
            PORT_DIPSETTING(    0x30, "50k and 100k" );
            PORT_DIPSETTING(    0x00, DEF_STR( None ) );
            PORT_DIPNAME( 0xc0, 0x00, DEF_STR( Language ) ); PORT_DIPLOCATION("M10:7,8");
            PORT_DIPSETTING(    0x00, DEF_STR( English ));
            PORT_DIPSETTING(    0x40, DEF_STR( German ));
            PORT_DIPSETTING(    0x80, DEF_STR( French ));
            PORT_DIPSETTING(    0xc0, DEF_STR( Spanish ));
        }


        //#define BZONEDSW1\
        void BZONEDSW1()
        {
            PORT_START("DSW1");
            PORT_DIPNAME( 0x03, 0x03, DEF_STR( Coinage ) ); PORT_DIPLOCATION("P10:1,2");
            PORT_DIPSETTING(    0x03, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x01, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Free_Play ) );
            PORT_DIPNAME( 0x0c, 0x00, DEF_STR( Coin_B ) ); PORT_DIPLOCATION("P10:3,4");
            PORT_DIPSETTING(    0x00, "*1" );
            PORT_DIPSETTING(    0x04, "*4" );
            PORT_DIPSETTING(    0x08, "*5" );
            PORT_DIPSETTING(    0x0c, "*6" );
            PORT_DIPNAME( 0x10, 0x00, DEF_STR( Coin_A ) ); PORT_DIPLOCATION("P10:5");
            PORT_DIPSETTING(    0x00, "*1" );
            PORT_DIPSETTING(    0x10, "*2" );
            PORT_DIPNAME( 0xe0, 0x00, "Bonus Coins" ); PORT_DIPLOCATION("P10:6,7,8");
            PORT_DIPSETTING(    0x00, DEF_STR( None ) );
            PORT_DIPSETTING(    0x20, "3 credits/2 coins" );
            PORT_DIPSETTING(    0x40, "5 credits/4 coins" );
            PORT_DIPSETTING(    0x60, "6 credits/4 coins" );
            PORT_DIPSETTING(    0x80, "6 credits/5 coins" );
        }


        //#define BZONEADJ \
        void BZONEADJ()
        {
            PORT_START("R11");
            PORT_ADJUSTER( 40, "R11 - Engine Frequency" );
        }


        //static INPUT_PORTS_START( bzone )
        void construct_ioport_bzone(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            BZONEIN0(owner);
            BZONEDSW0();
            BZONEDSW1();

            PORT_START("IN3");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICKRIGHT_DOWN ); PORT_2WAY();
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICKRIGHT_UP ); PORT_2WAY();
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICKLEFT_DOWN ); PORT_2WAY();
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICKLEFT_UP ); PORT_2WAY();
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_BUTTON1 );
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_START1 );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_START2 );
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_UNUSED );

            BZONEADJ();

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( redbaron )

        //static INPUT_PORTS_START( bradley )
    }


    partial class bzone_state : driver_device
    {
        /*************************************
         *
         *  Machine driver
         *
         *************************************/
        void bzone_base(machine_config config)
        {
            /* basic machine hardware */
            M6502(config, m_maincpu, BZONE_MASTER_CLOCK / 8);
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, bzone_map);
            m_maincpu.op0.execute().set_periodic_int(bzone_interrupt, attotime.from_hz(BZONE_CLOCK_3KHZ / 12));

            WATCHDOG_TIMER(config, "watchdog");

            /* video hardware */
            VECTOR(config, "vector");
            SCREEN(config, m_screen, SCREEN_TYPE_VECTOR);
            m_screen.op0.set_refresh_hz(BZONE_CLOCK_3KHZ / 12 / 6);
            m_screen.op0.set_size(400, 300);
            m_screen.op0.set_visarea(0, 580, 0, 400);
            m_screen.op0.set_screen_update("vector", (screen_device screen, bitmap_rgb32 bitmap, rectangle cliprect) => { return ((vector_device)subdevice("vector")).screen_update(screen, bitmap, cliprect); });

            avg_device avg = AVG_BZONE(config, "avg", 0);
            avg.set_vector("vector");
            avg.set_memory(m_maincpu, AS_PROGRAM, 0x2000);

            /* Drivers */
            MATHBOX(config, m_mathbox, 0);
        }


        public void bzone(machine_config config)
        {
            bzone_base(config);

            /* sound hardware */
            bzone_audio(config);
        }


        //void bzone_state::bradley(machine_config &config)

        //void redbaron_state::redbaron(machine_config &config)
    }


    partial class bzone : construct_ioport_helper
    {
        /*************************************
         *
         *  ROM definitions
         *
         *************************************/

        /* Battle Zone

        For the Analog Vec Gen A035742 PCB:

        The -01 revision uses PROMs, -02 uses ROMs

        Rom Component Equivalents & Locations:

        -01 P.C. Boards     -02 P.C. Boards
        ---------------------------------------
        036415-01 (A3)
                            036421-01 (A3)
        036418-01 (E3)


        036416-01 (B/C3)
                            036422-01 (B/C3)
        036419-01 (F/H3)

        */
        //ROM_START( bzone ) /* Analog Vec Gen A035742-02 */
        static readonly tiny_rom_entry [] rom_bzone =
        {
            ROM_REGION( 0x8000, "maincpu", 0 ),
            ROM_LOAD( "036414-02.e1",  0x5000, 0x0800, CRC("13de36d5") + SHA1("40e356ddc5c042bc1ce0b71f51e8b6de72daf1e4") ),
            ROM_LOAD( "036413-01.h1",  0x5800, 0x0800, CRC("5d9d9111") + SHA1("42638cff53a9791a0f18d316f62a0ea8eea4e194") ),
            ROM_LOAD( "036412-01.j1",  0x6000, 0x0800, CRC("ab55cbd2") + SHA1("6bbb8316d9f8588ea0893932f9174788292b8edc") ),
            ROM_LOAD( "036411-01.k1",  0x6800, 0x0800, CRC("ad281297") + SHA1("54c5e06b2e69eb731a6c9b1704e4340f493e7ea5") ),
            ROM_LOAD( "036410-01.lm1", 0x7000, 0x0800, CRC("0b7bfaa4") + SHA1("33ae0f68b4e2eae9f3aecbee2d0b29003ce460b2") ),
            ROM_LOAD( "036409-01.n1",  0x7800, 0x0800, CRC("1e14e919") + SHA1("448fab30535e6fad7e0ab4427bc06bbbe075e797") ),
            /* Vector Generator ROMs */
            ROM_LOAD( "036422-01.bc3", 0x3000, 0x0800, CRC("7414177b") + SHA1("147d97a3b475e738ce00b1a7909bbd787ad06eda") ),
            ROM_LOAD( "036421-01.a3",  0x3800, 0x0800, CRC("8ea8f939") + SHA1("b71e0ab0e220c3e64dc2b094c701fb1a960b64e4") ),  // 036421-01e.a3 same contents

            /* AVG PROM */
            ROM_REGION( 0x100, "avg:prom", 0 ),
            ROM_LOAD( "036408-01.k7",   0x0000, 0x0100, CRC("5903af03") + SHA1("24bc0366f394ad0ec486919212e38be0f08d0239") ),

            /* Mathbox PROMs */
            ROM_REGION( 0x20, "user2", 0 ),
            ROM_LOAD( "036174-01.b1",   0x0000, 0x0020, CRC("8b04f921") + SHA1("317b3397482f13b2d1bc21f296d3b3f9a118787b") ),

            ROM_REGION32_BE( 0x400, "user3", 0 ),
            ROMX_LOAD( "036175-01.m1", 0, 0x100, CRC("2af82e87") + SHA1("3816835a9ccf99a76d246adf204989d9261bb065"), ROM_NIBBLE | ROM_SHIFT_NIBBLE_LO | ROM_SKIP(3)),
            ROMX_LOAD( "036176-01.l1", 0, 0x100, CRC("b31f6e24") + SHA1("ce5f8ca34d06a5cfa0076b47400e61e0130ffe74"), ROM_NIBBLE | ROM_SHIFT_NIBBLE_HI | ROM_SKIP(3)),
            ROMX_LOAD( "036177-01.k1", 1, 0x100, CRC("8119b847") + SHA1("c4fbaedd4ce1ad6a4128cbe902b297743edb606a"), ROM_NIBBLE | ROM_SHIFT_NIBBLE_LO | ROM_SKIP(3)),
            ROMX_LOAD( "036178-01.j1", 1, 0x100, CRC("09f5a4d5") + SHA1("d6f2ac07ca9ee385c08831098b0dcaf56808993b"), ROM_NIBBLE | ROM_SHIFT_NIBBLE_HI | ROM_SKIP(3)),
            ROMX_LOAD( "036179-01.h1", 2, 0x100, CRC("823b61ae") + SHA1("d99a839874b45f64e14dae92a036e47a53705d16"), ROM_NIBBLE | ROM_SHIFT_NIBBLE_LO | ROM_SKIP(3)),
            ROMX_LOAD( "036180-01.f1", 2, 0x100, CRC("276eadd5") + SHA1("55718cd8ec4bcf75076d5ef0ee1ed2551e19d9ba"), ROM_NIBBLE | ROM_SHIFT_NIBBLE_HI | ROM_SKIP(3)),

            ROM_END,
        };


        //ROM_START( bzonea ) /* Analog Vec Gen A035742-02 */

        //ROM_START( bzonec ) /* cocktail version */

        //ROM_START( bradley )


        /* Red Barron

        For the Analog Vec Gen A035742 PCB:

        The -01 revision uses PROMs, -02 uses ROMs

        Rom Component Equivalents & Locations:

        -01 P.C. Boards     -02 P.C. Boards
        ---------------------------------------
        037005-01 (A3)
                            037007-01 (A3)
        037003-01 (E3)


        037004-01 (B/C3)
                            037006-01 (B/C3)
        037002-01 (F/H3)

        Program rom locations as same as redbarona listed below
        */

        //ROM_START( redbaron ) /* Analog Vec Gen A035742-02 Rev. C+ */

        //ROM_START( redbarona ) /* Analog Vec Gen A035742-02 */
    }


        /*************************************
         *
         *  Driver initialization
         *
         *************************************/

        //uint8_t bzone_state::analog_data_r()

        //void bzone_state::analog_select_w(offs_t offset, uint8_t data)


    partial class bzone : construct_ioport_helper
    {
        /*************************************
         *
         *  Game drivers
         *
         *************************************/

        static void bzone_state_bzone(machine_config config, device_t device) { bzone_state bzone_state = (bzone_state)device; bzone_state.bzone(config); }

        static bzone m_bzone = new bzone();

        static device_t device_creator_bzone(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new bzone_state(mconfig, type, tag); }


        public static readonly game_driver driver_bzone = GAMEL(device_creator_bzone, rom_bzone, "1980", "bzone", "0", bzone_state_bzone, m_bzone.construct_ioport_bzone, driver_device.empty_init, ROT0, "Atari", "Battle Zone (rev 2)", MACHINE_SUPPORTS_SAVE, null /*layout_bzone*/);
    }
}
