// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    partial class mw8080bw_state : driver_device
    {
        const double MW8080BW_MASTER_CLOCK             = 19968000.0;
        const double MW8080BW_CPU_CLOCK                = MW8080BW_MASTER_CLOCK / 10;
        const uint32_t MW8080BW_PIXEL_CLOCK            = (uint32_t)(MW8080BW_MASTER_CLOCK / 4);
        const uint16_t MW8080BW_HTOTAL                 = 0x140;
        const uint16_t MW8080BW_HBEND                  = 0x000;
        const uint16_t MW8080BW_HBSTART                = 0x100;
        const uint16_t MW8080BW_VTOTAL                 = 0x106;
        const uint16_t MW8080BW_VBEND                  = 0x000;
        const uint16_t MW8080BW_VBSTART                = 0x0e0;
        const int MW8080BW_VCOUNTER_START_NO_VBLANK    = 0x020;
        const int MW8080BW_VCOUNTER_START_VBLANK       = 0x0da;
        const int MW8080BW_INT_TRIGGER_COUNT_1         = 0x080;
        const int MW8080BW_INT_TRIGGER_VBLANK_1        = 0;
        const int MW8080BW_INT_TRIGGER_COUNT_2         = MW8080BW_VCOUNTER_START_VBLANK;
        const int MW8080BW_INT_TRIGGER_VBLANK_2        = 1;
        const uint32_t MW8080BW_60HZ                   = MW8080BW_PIXEL_CLOCK / MW8080BW_HTOTAL / MW8080BW_VTOTAL;

        // +4 is added to HBSTART because the hardware displays that many pixels after setting HBLANK
        const uint16_t MW8080BW_HPIXCOUNT              = MW8080BW_HBSTART + 4;


        // device/memory pointers
        protected required_device<i8080_cpu_device> m_maincpu;
        protected optional_device<mb14241_device> m_mb14241;
        optional_device<watchdog_timer_device> m_watchdog;
        required_shared_ptr<uint8_t> m_main_ram;
        optional_device<discrete_sound_device> m_discrete;
        required_device<screen_device> m_screen;

        // misc game specific
        uint8_t m_flip_screen;

        // misc game specific
        //uint16_t      m_phantom2_cloud_counter;
        //uint8_t       m_maze_tone_timing_state;   // output of IC C1, pin 5

        // timers
        emu_timer m_interrupt_timer;
        //emu_timer   *m_maze_tone_timer;

        attotime m_interrupt_time;


        public mw8080bw_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_maincpu = new required_device<i8080_cpu_device>(this, "maincpu");
            m_mb14241 = new optional_device<mb14241_device>(this, "mb14241");
            m_watchdog = new optional_device<watchdog_timer_device>(this, "watchdog");
            m_main_ram = new required_shared_ptr<uint8_t>(this, "main_ram");
            m_discrete = new optional_device<discrete_sound_device>(this, "discrete");
            m_screen = new required_device<screen_device>(this, "screen");
        }
    }


    //#define SEAWOLF_ERASE_SW_PORT_TAG   ("ERASESW")
    //#define SEAWOLF_ERASE_DIP_PORT_TAG  ("ERASEDIP")

    //class seawolf_state : public mw8080bw_state


    partial class gunfight_state : mw8080bw_state
    {
        required_device<gunfight_audio_device> m_soundboard;


        public gunfight_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_soundboard = new required_device<gunfight_audio_device>(this, "soundboard");
        }
    }


    //class boothill_state : public mw8080bw_state

    //#define DESERTGU_GUN_X_PORT_TAG         ("GUNX")
    //#define DESERTGU_GUN_Y_PORT_TAG         ("GUNY")

    //#define DESERTGU_DIP_SW_0_1_SET_1_TAG   ("DIPSW01SET1")
    //#define DESERTGU_DIP_SW_0_1_SET_2_TAG   ("DIPSW01SET2")

    //class desertgu_state : public mw8080bw_state

    //#define DPLAY_L_PITCH_PORT_TAG      ("LPITCH")
    //#define DPLAY_R_PITCH_PORT_TAG      ("RPITCH")
    //#define DPLAY_CAB_TYPE_PORT_TAG     ("CAB")

    //class dplay_state : public mw8080bw_state

    //class clowns_state : public mw8080bw_state

    //class spcenctr_state : public mw8080bw_state

    //class zzzap_state : public mw8080bw_state

    //#define TORNBASE_CAB_TYPE_UPRIGHT_OLD   (0)
    //#define TORNBASE_CAB_TYPE_UPRIGHT_NEW   (1)
    //#define TORNBASE_CAB_TYPE_COCKTAIL      (2)


    partial class mw8080bw_state : driver_device
    {
        public const string INVADERS_CAB_TYPE_PORT_TAG   = "CAB";
        public const string INVADERS_P1_CONTROL_PORT_TAG = "CONTP1";
        public const string INVADERS_P2_CONTROL_PORT_TAG = "CONTP2";
        public const string INVADERS_SW6_SW7_PORT_TAG    = "SW6SW7";
        public const string INVADERS_SW5_PORT_TAG        = "SW5";
    }


    //#define BLUESHRK_SPEAR_PORT_TAG         ("IN0")

    //#define INVADERS_CONTROL_PORT_P1 \
    //    PORT_START(INVADERS_P1_CONTROL_PORT_TAG) \
    //    INVADERS_CONTROL_PORT_PLAYER(1)

    //#define INVADERS_CONTROL_PORT_P2 \
    //    PORT_START(INVADERS_P2_CONTROL_PORT_TAG) \
    //    INVADERS_CONTROL_PORT_PLAYER(2)

    //#define INVADERS_CONTROL_PORT_PLAYER(player) \
    //    PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_BUTTON1 ) PORT_PLAYER(player) \
    //    PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ) PORT_2WAY PORT_PLAYER(player) \
    //    PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ) PORT_2WAY PORT_PLAYER(player) \
    //    PORT_BIT( 0xf8, IP_ACTIVE_HIGH, IPT_UNUSED )

    //#define INVADERS_CAB_TYPE_PORT \
    //    PORT_START(INVADERS_CAB_TYPE_PORT_TAG) \
    //    PORT_CONFNAME( 0x01, 0x00, DEF_STR( Cabinet ) ) \
    //    PORT_CONFSETTING(    0x00, DEF_STR( Upright ) ) \
    //    PORT_CONFSETTING(    0x01, DEF_STR( Cocktail ) )

    /*----------- defined in drivers/mw8080bw.c -----------*/
    //extern const internal_layout layout_invaders;
}
