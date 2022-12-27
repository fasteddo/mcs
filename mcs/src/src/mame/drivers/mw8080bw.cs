// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using ioport_value = System.UInt32;  //typedef u32 ioport_value;
using offs_t = System.UInt32;  //using offs_t = u32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;

using static mame.device_global;
using static mame.driver_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.i8085_global;
using static mame.input_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.mb14241_global;
using static mame.mw8080bw_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.watchdog_global;


namespace mame
{
    partial class mw8080bw_state : driver_device
    {
        //INPUT_CHANGED_MEMBER(mw8080bw_state::direct_coin_count)
        public void direct_coin_count(ioport_field field, u32 param, ioport_value oldval, ioport_value newval)
        {
            machine().bookkeeping().coin_counter_w(0, (int)newval);
        }


        /*************************************
         *
         *  Special shifter circuit
         *
         *************************************/
        //u8 mw8080bw_state::mw8080bw_shift_result_rev_r()


        /*************************************
         *
         *  Main CPU memory handlers
         *
         *************************************/
        void main_map(address_map map, device_t device)
        {
            map.global_mask(0x7fff);
            map.op(0x0000, 0x1fff).rom().nopw();
            map.op(0x2000, 0x3fff).mirror(0x4000).ram().share("main_ram");
            map.op(0x4000, 0x5fff).rom().nopw();
        }


        /*************************************
         *
         *  Root driver structure
         *
         *************************************/
        protected void mw8080bw_root(machine_config config)
        {
            /* basic machine hardware */
            i8080_cpu_device maincpu = I8080(config, m_maincpu, MW8080BW_CPU_CLOCK);
            maincpu.memory().set_addrmap(AS_PROGRAM, main_map);
            maincpu.execute().set_irq_acknowledge_callback(interrupt_vector);
            maincpu.out_inte_func().set((write_line_delegate)int_enable_w).reg();

            MCFG_MACHINE_RESET_OVERRIDE(config, machine_reset_mw8080bw);

            /* video hardware */
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_raw(MW8080BW_PIXEL_CLOCK, MW8080BW_HTOTAL, MW8080BW_HBEND, MW8080BW_HPIXCOUNT, MW8080BW_VTOTAL, MW8080BW_VBEND, MW8080BW_VBSTART);
            m_screen.op0.set_screen_update(screen_update_mw8080bw);
        }
    }


        /*************************************
         *
         *  Sea Wolf (PCB #596)
         *
         *************************************/
        //void seawolf_state::machine_start()

        //void seawolf_state::explosion_lamp_w(u8 data)

        //void seawolf_state::periscope_lamp_w(u8 data)

        //CUSTOM_INPUT_MEMBER(seawolf_state::erase_input_r)

        //void seawolf_state::io_map(address_map &map)

        // the 30 position encoder is verified
        //static const ioport_value seawolf_controller_table[30] =

        //static INPUT_PORTS_START( seawolf )

        //void seawolf_state::seawolf(machine_config &config)


    partial class gunfight_state : mw8080bw_state
    {
        /*************************************
         *
         *  Gun Fight (PCB #597)
         *
         *************************************/
        void io_w(offs_t offset, u8 data)
        {
            throw new emu_unimplemented();
        }


        void io_map(address_map map, device_t device)
        {
            map.global_mask(0x7);
            map.op(0x00, 0x00).mirror(0x04).portr("IN0");
            map.op(0x01, 0x01).mirror(0x04).portr("IN1");
            map.op(0x02, 0x02).mirror(0x04).portr("IN2");
            map.op(0x03, 0x03).mirror(0x04).r(m_mb14241, () => { return m_mb14241.op0.shift_result_r(); });

            map.op(0x00, 0x07).w(io_w); // no decoder, just 3 AND gates
        }
    }


    public partial class mw8080bw : construct_ioport_helper
    {
        static readonly ioport_value [] gunfight_controller_table =
        {
            0x06, 0x02, 0x00, 0x04, 0x05, 0x01, 0x03
        };


        //static INPUT_PORTS_START( gunfight )
        void construct_ioport_gunfight(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            gunfight_state gunfight_state = (gunfight_state)owner;

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ); PORT_8WAY(); PORT_PLAYER(1);
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ); PORT_8WAY(); PORT_PLAYER(1);
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ); PORT_8WAY(); PORT_PLAYER(1);
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ); PORT_8WAY(); PORT_PLAYER(1);
            PORT_BIT( 0x70, 0x30, IPT_POSITIONAL_V ); PORT_POSITIONS(7); PORT_REMAP_TABLE(gunfight_controller_table); PORT_INVERT(); PORT_SENSITIVITY(5); PORT_KEYDELTA(10); PORT_CENTERDELTA(0); PORT_CODE_DEC(KEYCODE_N); PORT_CODE_INC(KEYCODE_H); PORT_PLAYER(1);
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_BUTTON1 ); PORT_PLAYER(1);

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_JOYSTICK_UP ); PORT_8WAY(); PORT_PLAYER(2);
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_DOWN ); PORT_8WAY(); PORT_PLAYER(2);
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ); PORT_8WAY(); PORT_PLAYER(2);
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ); PORT_8WAY(); PORT_PLAYER(2);
            PORT_BIT( 0x70, 0x30, IPT_POSITIONAL_V ); PORT_POSITIONS(7); PORT_REMAP_TABLE(gunfight_controller_table); PORT_INVERT(); PORT_SENSITIVITY(5); PORT_KEYDELTA(10); PORT_CENTERDELTA(0); PORT_CODE_DEC(KEYCODE_M); PORT_CODE_INC(KEYCODE_J); PORT_PLAYER(2);
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_BUTTON1 ); PORT_PLAYER(2);

            PORT_START("IN2");
            PORT_DIPNAME( 0x0f, 0x00, DEF_STR( Coinage ) ); PORT_DIPLOCATION("C1:1,2,3,4");
            PORT_DIPSETTING(    0x03, DEF_STR( _4C_1C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x07, DEF_STR( _4C_2C ) );
            PORT_DIPSETTING(    0x01, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x06, DEF_STR( _3C_2C ) );
            PORT_DIPSETTING(    0x0b, DEF_STR( _4C_3C ) );
            PORT_DIPSETTING(    0x0f, DEF_STR( _4C_4C ) );
            PORT_DIPSETTING(    0x0a, DEF_STR( _3C_3C ) );
            PORT_DIPSETTING(    0x05, DEF_STR( _2C_2C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x0e, DEF_STR( _3C_4C ) );
            PORT_DIPSETTING(    0x09, DEF_STR( _2C_3C ) );
            PORT_DIPSETTING(    0x0d, DEF_STR( _2C_4C ) );
            PORT_DIPSETTING(    0x04, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x08, DEF_STR( _1C_3C ) );
            PORT_DIPSETTING(    0x0c, DEF_STR( _1C_4C ) );
            PORT_DIPNAME( 0x30, 0x10, DEF_STR( Game_Time ) ); PORT_DIPLOCATION("C1:5,6");
            PORT_DIPSETTING(    0x00, "60 seconds" );
            PORT_DIPSETTING(    0x10, "70 seconds" );
            PORT_DIPSETTING(    0x20, "80 seconds" );
            PORT_DIPSETTING(    0x30, "90 seconds" );
            PORT_BIT( 0x40, IP_ACTIVE_HIGH, IPT_COIN1 );
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_START1 );

            INPUT_PORTS_END();
        }
    }


    partial class gunfight_state : mw8080bw_state
    {
        public void gunfight(machine_config config)
        {
            mw8080bw_root(config);

            // basic machine hardware
            m_maincpu.op0.memory().set_addrmap(AS_IO, io_map);

            // there is no watchdog

            // add shifter
            MB14241(config, m_mb14241);

            // audio hardware
            GUNFIGHT_AUDIO(config, m_soundboard);
        }
    }


        /*************************************
         *
         *  Tornado Baseball (PCB #605)
         *
         *  Notes:
         *  -----
         *
         *  In baseball, the Visitor team always hits first and the Home team pitches (throws the ball).
         *  This rule gives an advantage to the Home team because they get to score last in any baseball game.
         *  It is also the team that pitches that controls the player on the field, which, in this game,
         *  is limited to moving the 3 outfielders left and right.
         *
         *  There are 3 types of cabinets using the same software:
         *
         *  Old Upright: One of everything
         *
         *  New Upright: One fielding/pitching controls, but two (Left/Right) hitting buttons
         *
         *  Cocktail:    Two of everything, but the pitching/fielding controls are swapped
         *
         *  Interestingly, the "Whistle" sound effect is controlled by a different
         *  bit on the Old Upright cabinet than the other two types.
         *
         *************************************/

        //#define TORNBASE_L_HIT_PORT_TAG         ("LHIT")
        //#define TORNBASE_R_HIT_PORT_TAG         ("RHIT")
        //#define TORNBASE_L_PITCH_PORT_TAG       ("LPITCH")
        //#define TORNBASE_R_PITCH_PORT_TAG       ("RPITCH")
        //#define TORNBASE_SCORE_SW_PORT_TAG      ("SCORESW")
        //#define TORNBASE_SCORE_DIP_PORT_TAG     ("ERASEDIP")
        //#define TORNBASE_CAB_TYPE_PORT_TAG      ("CAB")


    partial class mw8080bw_state : driver_device
    {
        //uint8_t mw8080bw_state::tornbase_get_cabinet_type()

        //CUSTOM_INPUT_MEMBER(mw8080bw_state::tornbase_hit_left_input_r)

        //CUSTOM_INPUT_MEMBER(mw8080bw_state::tornbase_hit_right_input_r)

        //CUSTOM_INPUT_MEMBER(mw8080bw_state::tornbase_pitch_left_input_r)

        //CUSTOM_INPUT_MEMBER(mw8080bw_state::tornbase_pitch_right_input_r)

        //CUSTOM_INPUT_MEMBER(mw8080bw_state::tornbase_score_input_r)

        //void mw8080bw_state::tornbase_io_w(offs_t offset, uint8_t data)

        //void mw8080bw_state::tornbase_io_map(address_map &map)
    }


    partial class mw8080bw : construct_ioport_helper
    {
        //static INPUT_PORTS_START( tornbase )

        //void mw8080bw_state::tornbase(machine_config &config)

        /*************************************
         *
         *  280 ZZZAP (PCB #610) / Laguna Racer (PCB #622)
         *
         *************************************/
        //void zzzap_state::io_map(address_map &map)

        //static INPUT_PORTS_START( zzzap )

        //static INPUT_PORTS_START( lagunar )

        //void zzzap_state::zzzap_common(machine_config &config)

        //void zzzap_state::zzzap(machine_config &config)

        //void zzzap_state::lagunar(machine_config &config)


        /*************************************
         *
         *  Amazing Maze (PCB #611)
         *
         *************************************/
        /* schematic says 12.5 Hz, but R/C values shown give 8.5Hz */
        //#define MAZE_555_B1_PERIOD      PERIOD_OF_555_ASTABLE(RES_K(33) /* R200 */, RES_K(68) /* R201 */, CAP_U(1) /* C201 */)

        //void mw8080bw_state::maze_update_discrete()

        //TIMER_CALLBACK_MEMBER(mw8080bw_state::maze_tone_timing_timer_callback)

        //MACHINE_START_MEMBER(mw8080bw_state,maze)

        //void mw8080bw_state::maze_coin_counter_w(uint8_t data)

        //void mw8080bw_state::maze_io_w(offs_t offset, uint8_t data)

        //void mw8080bw_state::maze_io_map(address_map &map)

        //static INPUT_PORTS_START( maze )

        //void mw8080bw_state::maze(machine_config &config)

        /*************************************
         *
         *  Boot Hill (PCB #612)
         *
         *************************************/
        //void boothill_state::machine_start()

        //u8 boothill_state::reversible_shift_result_r()

        //void boothill_state::reversible_shift_count_w(u8 data)

        //void boothill_state::boothill_io_map(address_map &map)

        //static const ioport_value boothill_controller_table[7] =

        //static INPUT_PORTS_START( boothill )

        //void boothill_state::boothill(machine_config &config)


        /*************************************
         *
         *  Checkmate (PCB #615)
         *
         *************************************/
        //void mw8080bw_state::checkmat_io_w(offs_t offset, uint8_t data)

        //void mw8080bw_state::checkmat_io_map(address_map &map)

        //static INPUT_PORTS_START( checkmat )

        //void mw8080bw_state::checkmat(machine_config &config)


        /*************************************
         *
         *  Desert Gun / Road Runner (PCB #618)
         *
         *************************************/
        //void desertgu_state::machine_start()

        //CUSTOM_INPUT_MEMBER(desertgu_state::gun_input_r)

        //CUSTOM_INPUT_MEMBER(desertgu_state::dip_sw_0_1_r)

        //void desertgu_state::io_map(address_map &map)

        //static INPUT_PORTS_START( desertgu )

        //void desertgu_state::desertgu(machine_config &config)


        /*************************************
         *
         *  Double Play (PCB #619) / Extra Inning (PCB #642)
         *
         *  This game comes in an upright and a cocktail cabinet.
         *  The upright one had a shared joystick and a hitting button for
         *  each player, while in the cocktail version each player
         *  had their own set of controls.  The display is never flipped,
         *  as the two players sit diagonally across from each other.
         *
         *************************************/
        //#define DPLAY_CAB_TYPE_UPRIGHT      (0)
        //#define DPLAY_CAB_TYPE_COCKTAIL     (1)

        //CUSTOM_INPUT_MEMBER(dplay_state::dplay_pitch_left_input_r)

        //CUSTOM_INPUT_MEMBER(dplay_state::dplay_pitch_right_input_r)

        //void dplay_state::io_map(address_map &map)

        //static INPUT_PORTS_START( dplay )

        //static INPUT_PORTS_START( einning )

        //void dplay_state::dplay(machine_config &config)


        /*************************************
         *
         *  Guided Missile (PCB #623)
         *
         *************************************/
        //void boothill_state::gmissile_io_map(address_map &map)

        //static INPUT_PORTS_START( gmissile )

        //void boothill_state::gmissile(machine_config &config)


        /*************************************
         *
         *  M-4 (PCB #626)
         *
         *************************************/
        //void boothill_state::m4_io_map(address_map &map)

        //static INPUT_PORTS_START( m4 )

        //void boothill_state::m4(machine_config &config)


        /*************************************
         *
         *  Clowns (PCB #630)
         *
         *************************************/
        //#define CLOWNS_CONTROLLER_P1_TAG        ("CONTP1")
        //#define CLOWNS_CONTROLLER_P2_TAG        ("CONTP2")

        //void clowns_state::machine_start()

        //CUSTOM_INPUT_MEMBER(clowns_state::controller_r)

        //void clowns_state::clowns_io_map(address_map &map)

        //static INPUT_PORTS_START( clowns )

        //static INPUT_PORTS_START( clowns1 )

        //void clowns_state::clowns(machine_config &config)


        /*************************************
         *
         *  Space Walk (PCB #640)
         *
         *************************************/
        //void clowns_state::spacwalk_io_map(address_map &map)

        //static INPUT_PORTS_START( spacwalk )

        //void clowns_state::spacwalk(machine_config &config)


        /*************************************
         *
         *  Shuffleboard (PCB #643)
         *
         *************************************/
        //void mw8080bw_state::shuffle_io_map(address_map &map)

        //static INPUT_PORTS_START( shuffle )

        //void mw8080bw_state::shuffle(machine_config &config)


        /*************************************
         *
         *  Dog Patch (PCB #644)
         *
         *************************************/
        //void mw8080bw_state::dogpatch_io_map(address_map &map)

        //static const ioport_value dogpatch_controller_table[7] =

        //static INPUT_PORTS_START( dogpatch )

        //void mw8080bw_state::dogpatch(machine_config &config)


        /*************************************
         *
         *  Space Encounters (PCB #645)
         *
         *************************************/
        //void spcenctr_state::machine_start()

        //void spcenctr_state::io_w(offs_t offset, u8 data)

        //void spcenctr_state::io_map(address_map &map)

        //static const ioport_value spcenctr_controller_table[] =

        //static INPUT_PORTS_START( spcenctr )

        //void spcenctr_state::spcenctr(machine_config &config)


        /*************************************
         *
         *  Phantom II (PCB #652)
         *
         *************************************/
        //MACHINE_START_MEMBER(mw8080bw_state,phantom2)

        //void mw8080bw_state::phantom2_io_map(address_map &map)

        //static INPUT_PORTS_START( phantom2 )

        //void mw8080bw_state::phantom2(machine_config &config)


        /*************************************
         *
         *  Bowling Alley (PCB #730)
         *
         *************************************/
        //uint8_t mw8080bw_state::bowler_shift_result_r()

        //void mw8080bw_state::bowler_lights_1_w(uint8_t data)

        //void mw8080bw_state::bowler_lights_2_w(uint8_t data)

        //void mw8080bw_state::bowler_io_map(address_map &map)

        //static INPUT_PORTS_START( bowler )
    }


    partial class mw8080bw_state : driver_device
    {
        //void mw8080bw_state::bowler(machine_config &config)


        /*************************************
         *
         *  Space Invaders (PCB #739)
         *
         *************************************/
        //MACHINE_START_MEMBER(mw8080bw_state,invaders)
        void machine_start_invaders()
        {
            this.machine_start();  //mw8080bw_state::machine_start();

            m_flip_screen = 0;

            save_item(NAME(new { m_flip_screen }));
        }


        //CUSTOM_INPUT_MEMBER(mw8080bw_state::invaders_sw6_sw7_r)
        public ioport_value invaders_sw6_sw7_r()
        {
            // upright PCB : switches visible
            // cocktail PCB: HI
        
            if (invaders_is_cabinet_cocktail() != 0)
                return 0x03;
            else
                return ioport(INVADERS_SW6_SW7_PORT_TAG).read();
        }


        //CUSTOM_INPUT_MEMBER(mw8080bw_state::invaders_sw5_r)
        public ioport_value invaders_sw5_r()
        {
            // upright PCB : switch visible
            // cocktail PCB: HI
        
            if (invaders_is_cabinet_cocktail() != 0)
                return 0x01;
            else
                return ioport(INVADERS_SW5_PORT_TAG).read();
        }


        //CUSTOM_INPUT_MEMBER(mw8080bw_state::invaders_in0_control_r)
        public ioport_value invaders_in0_control_r()
        {
            // upright PCB : P1 controls
            // cocktail PCB: HI
        
            if (invaders_is_cabinet_cocktail() != 0)
                return 0x07;
            else
                return ioport(INVADERS_P1_CONTROL_PORT_TAG).read();
        }


        //CUSTOM_INPUT_MEMBER(mw8080bw_state::invaders_in1_control_r)
        public ioport_value invaders_in1_control_r()
        {
            return ioport(INVADERS_P1_CONTROL_PORT_TAG).read();
        }


        //CUSTOM_INPUT_MEMBER(mw8080bw_state::invaders_in2_control_r)
        public ioport_value invaders_in2_control_r()
        {
            // upright PCB : P1 controls
            // cocktail PCB: P2 controls
        
            if (invaders_is_cabinet_cocktail() != 0)
                return ioport(INVADERS_P2_CONTROL_PORT_TAG).read();
            else
                return ioport(INVADERS_P1_CONTROL_PORT_TAG).read();
        }


        int invaders_is_cabinet_cocktail()
        {
            return (int)ioport(INVADERS_CAB_TYPE_PORT_TAG).read();
        }


        void invaders_io_map(address_map map, device_t device)
        {
            map.global_mask(0x7);
            map.op(0x00, 0x00).mirror(0x04).portr("IN0");
            map.op(0x01, 0x01).mirror(0x04).portr("IN1");
            map.op(0x02, 0x02).mirror(0x04).portr("IN2");
            map.op(0x03, 0x03).mirror(0x04).r(m_mb14241, () => { return m_mb14241.op0.shift_result_r(); });

            map.op(0x02, 0x02).w(m_mb14241, (data) => { m_mb14241.op0.shift_count_w(data); });
            map.op(0x03, 0x03).w("soundboard", (data) => { ((invaders_audio_device)subdevice("soundboard")).p1_w(data); });
            map.op(0x04, 0x04).w(m_mb14241, (data) => { m_mb14241.op0.shift_data_w(data); });
            map.op(0x05, 0x05).w("soundboard", (data) => { ((invaders_audio_device)subdevice("soundboard")).p2_w(data); });
            map.op(0x06, 0x06).w(m_watchdog, (data) => { m_watchdog.op0.reset_w(data); });
        }
    }


    partial class mw8080bw : construct_ioport_helper
    {
        //static INPUT_PORTS_START( invaders )
        void construct_ioport_invaders(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            mw8080bw_state mw8080bw_state = (mw8080bw_state)owner;

            PORT_START("IN0");
            PORT_DIPNAME( 0x01, 0x00, DEF_STR( Unknown ) ); PORT_DIPLOCATION("SW:8");
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x01, DEF_STR( On ) );
            PORT_BIT( 0x06, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_CUSTOM_MEMBER(DEVICE_SELF, mw8080bw_state.invaders_sw6_sw7_r);
            PORT_BIT( 0x08, IP_ACTIVE_LOW,  IPT_UNUSED );
            PORT_BIT( 0x70, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_CUSTOM_MEMBER(DEVICE_SELF, mw8080bw_state.invaders_in0_control_r);
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_CUSTOM_MEMBER(DEVICE_SELF, mw8080bw_state.invaders_sw5_r);

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW,  IPT_COIN1 ); PORT_CHANGED_MEMBER(DEVICE_SELF, mw8080bw_state.direct_coin_count, 0);
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_START2 );
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_START1 );
            PORT_BIT( 0x08, IP_ACTIVE_LOW,  IPT_UNUSED );
            PORT_BIT( 0x70, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_CUSTOM_MEMBER(DEVICE_SELF, mw8080bw_state.invaders_in1_control_r);
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_UNUSED );

            PORT_START("IN2");
            PORT_DIPNAME( 0x03, 0x00, DEF_STR( Lives ) ); PORT_DIPLOCATION("SW:3,4");
            PORT_DIPSETTING(    0x00, "3" );
            PORT_DIPSETTING(    0x01, "4" );
            PORT_DIPSETTING(    0x02, "5" );
            PORT_DIPSETTING(    0x03, "6" );
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_UNUSED ); /* in the software, this is TILI, but not connected on the Midway PCB. Is this correct? */
            PORT_DIPNAME( 0x08, 0x00, DEF_STR( Bonus_Life ) ); PORT_DIPLOCATION("SW:2");
            PORT_DIPSETTING(    0x08, "1000" );
            PORT_DIPSETTING(    0x00, "1500" );
            PORT_BIT( 0x70, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_CUSTOM_MEMBER(DEVICE_SELF, mw8080bw_state.invaders_in2_control_r);
            PORT_DIPNAME( 0x80, 0x00, "Display Coinage" ); PORT_DIPLOCATION("SW:1");
            PORT_DIPSETTING(    0x80, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );

            /* fake port for cabinet type */
            PORT_START(mw8080bw_state.INVADERS_CAB_TYPE_PORT_TAG);
            PORT_CONFNAME( 0x01, 0x00, DEF_STR( Cabinet ) );
            PORT_CONFSETTING(    0x00, DEF_STR( Upright ) );
            PORT_CONFSETTING(    0x01, DEF_STR( Cocktail ) );
            PORT_BIT( 0xfe, IP_ACTIVE_HIGH, IPT_UNUSED );

            /* fake ports for handling the various input ports based on cabinet type */
            PORT_START(mw8080bw_state.INVADERS_SW6_SW7_PORT_TAG);
            PORT_DIPNAME( 0x01, 0x00, DEF_STR( Unused ) ); PORT_DIPLOCATION("SW:7");
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x01, DEF_STR( On ) );
            PORT_DIPNAME( 0x02, 0x00, DEF_STR( Unused ) ); PORT_DIPLOCATION("SW:6");
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x02, DEF_STR( On ) );
            PORT_BIT( 0xfc, IP_ACTIVE_HIGH, IPT_UNUSED );

            PORT_START(mw8080bw_state.INVADERS_SW5_PORT_TAG);
            PORT_DIPNAME( 0x01, 0x00, DEF_STR( Unused ) ); PORT_DIPLOCATION("SW:5");
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x01, DEF_STR( On ) );
            PORT_BIT( 0xfe, IP_ACTIVE_HIGH, IPT_UNUSED );

            PORT_START(mw8080bw_state.INVADERS_P1_CONTROL_PORT_TAG);
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_BUTTON1 ); PORT_PLAYER(1);
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ); PORT_2WAY(); PORT_PLAYER(1);
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ); PORT_2WAY(); PORT_PLAYER(1);
            PORT_BIT( 0xf8, IP_ACTIVE_HIGH, IPT_UNUSED );

            PORT_START(mw8080bw_state.INVADERS_P2_CONTROL_PORT_TAG);
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_BUTTON1 ); PORT_PLAYER(2);
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ); PORT_2WAY(); PORT_PLAYER(2);
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ); PORT_2WAY(); PORT_PLAYER(2);
            PORT_BIT( 0xf8, IP_ACTIVE_HIGH, IPT_UNUSED );

            INPUT_PORTS_END();
        }
    }


    partial class mw8080bw_state : driver_device
    {
        public void invaders(machine_config config)
        {
            mw8080bw_root(config);

            // basic machine hardware
            m_maincpu.op0.memory().set_addrmap(AS_IO, invaders_io_map);

            MCFG_MACHINE_START_OVERRIDE(config, machine_start_invaders);

            WATCHDOG_TIMER(config, m_watchdog).set_time(255 * attotime.from_hz(MW8080BW_60HZ));

            // video hardware
            m_screen.op0.set_screen_update(screen_update_invaders);

            // add shifter
            MB14241(config, m_mb14241);

            // audio hardware
            INVADERS_AUDIO(config, "soundboard").  // the flip screen line is only connected on the cocktail PCB
                    flip_screen_out().set((int state) => { if (invaders_is_cabinet_cocktail() != 0) m_flip_screen = state != 0 ? (uint8_t)1 : (uint8_t)0; }).reg();
        }


        /*************************************
         *
         *  Blue Shark (PCB #742)
         *
         *************************************/
        //#define BLUESHRK_COIN_INPUT_PORT_TAG    ("COIN")

        //CUSTOM_INPUT_MEMBER(mw8080bw_state::blueshrk_coin_input_r)

        //void mw8080bw_state::blueshrk_io_map(address_map &map)

        //static INPUT_PORTS_START( blueshrk )

        //void mw8080bw_state::blueshrk(machine_config &config)


        /*************************************
         *
         *  Space Invaders II (cocktail) (PCB #851)
         *
         *************************************/
        //void mw8080bw_state::invad2ct_io_map(address_map &map)

        //static INPUT_PORTS_START( invad2ct )

        //void mw8080bw_state::invad2ct(machine_config &config)
    }


    partial class mw8080bw : construct_ioport_helper
    {
        /*************************************
         *
         *  ROM definitions
         *
         *************************************/
        //ROM_START( seawolf )

        //ROM_START( seawolfo )


        //ROM_START( gunfight )
        static readonly tiny_rom_entry [] rom_gunfight =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "7609h.bin",  0x0000, 0x0400, CRC("0b117d73") + SHA1("99d01313e251818d336281700e206d9003c71dae") ),
            ROM_LOAD( "7609g.bin",  0x0400, 0x0400, CRC("57bc3159") + SHA1("c177e3f72db9af17ab99b2481448ca26318184b9") ),
            ROM_LOAD( "7609f.bin",  0x0800, 0x0400, CRC("8049a6bd") + SHA1("215b068663e431582591001cbe028929fa96d49f") ),
            ROM_LOAD( "7609e.bin",  0x0c00, 0x0400, CRC("773264e2") + SHA1("de3f2e6841122bbe6e2fda5b87d37842c072289a") ),
            ROM_END,
        };


        //ROM_START( gunfighto )

        //ROM_START( tornbase )

        //ROM_START( 280zzzap )

        //ROM_START( maze )

        //ROM_START( boothill )

        //ROM_START( checkmat )

        //ROM_START( desertgu )

        //ROM_START( roadrunm )

        //ROM_START( dplay )

        //ROM_START( lagunar )

        //ROM_START( gmissile )

        //ROM_START( m4 )

        //ROM_START( clowns )

        //ROM_START( clowns1 )

        //ROM_START( spacwalk )

        //ROM_START( einning )

        //ROM_START( shuffle )

        //ROM_START( dogpatch )

        //ROM_START( spcenctr )

        //ROM_START( phantom2 )

        //ROM_START( bowler )


        //ROM_START( invaders )
        static readonly tiny_rom_entry [] rom_invaders =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "invaders.h", 0x0000, 0x0800, CRC("734f5ad8") + SHA1("ff6200af4c9110d8181249cbcef1a8a40fa40b7f") ),
            ROM_LOAD( "invaders.g", 0x0800, 0x0800, CRC("6bfaca4a") + SHA1("16f48649b531bdef8c2d1446c429b5f414524350") ),
            ROM_LOAD( "invaders.f", 0x1000, 0x0800, CRC("0ccead96") + SHA1("537aef03468f63c5b9e11dd61e253f7ae17d9743") ),
            ROM_LOAD( "invaders.e", 0x1800, 0x0800, CRC("14e538b0") + SHA1("1d6ca0c99f9df71e2990b610deb9d7da0125e2d8") ),

            ROM_END,
        };


        //ROM_START( blueshrk )

        //ROM_START( blueshrkmr )

        //ROM_START( blueshrkmr2 )

        //ROM_START( invad2ct )


        /*************************************
         *
         *  Game drivers
         *
         *************************************/

        static void gunfight_state_gunfight(machine_config config, device_t device) { gunfight_state gunfight_state = (gunfight_state)device; gunfight_state.gunfight(config); }
        static void mw8080bw_state_invaders(machine_config config, device_t device) { mw8080bw_state mw8080bw_state = (mw8080bw_state)device; mw8080bw_state.invaders(config); }

        static mw8080bw m_mw8080bw = new mw8080bw();

        static device_t device_creator_mw8080bw(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new mw8080bw_state(mconfig, type, tag); }
        static device_t device_creator_gunfight(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new gunfight_state(mconfig, type, tag); }


        // PCB #                                                             creator                  rom           year    name        parent machine                  inp                                   init                      monitor company,                            fullname,                            flags                  layout
        /* 597 */ public static readonly game_driver driver_gunfight = GAMEL(device_creator_gunfight, rom_gunfight, "1975", "gunfight", "0",   gunfight_state_gunfight, m_mw8080bw.construct_ioport_gunfight, driver_device.empty_init, ROT0,   "Dave Nutting Associates / Midway", "Gun Fight (set 1)",                 MACHINE_SUPPORTS_SAVE, null /*layout_gunfight*/);
        /* 739 */ public static readonly game_driver driver_invaders = GAMEL(device_creator_mw8080bw, rom_invaders, "1978", "invaders", "0",   mw8080bw_state_invaders, m_mw8080bw.construct_ioport_invaders, driver_device.empty_init, ROT270, "Taito / Midway",                   "Space Invaders / Space Invaders M", MACHINE_SUPPORTS_SAVE, null /*layout_invaders*/);
    }
}
