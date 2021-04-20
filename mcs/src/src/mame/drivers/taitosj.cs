// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ioport_value = System.UInt32;
using offs_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;


namespace mame
{
    partial class taitosj_state : driver_device
    {
        //WRITE8_MEMBER(taitosj_state::taitosj_sndnmi_msk_w)
        void taitosj_sndnmi_msk_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            /* B0 is the sound nmi enable, active low */
            m_soundnmi.target.in_w(0, (~data) & 1);
        }


        //WRITE8_MEMBER(taitosj_state::soundlatch_w)
        void soundlatch_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            machine().scheduler().synchronize(soundlatch_w_cb, data);
        }


        //WRITE8_MEMBER(taitosj_state::input_port_4_f0_w)
        void input_port_4_f0_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_input_port_4_f0 = (uint8_t)(data >> 4);
        }


        // EPORT2
        //WRITE8_MEMBER(taitosj_state::sound_semaphore2_w)
        void sound_semaphore2_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            machine().scheduler().synchronize(sound_semaphore2_w_cb, data);
        }


        //CUSTOM_INPUT_MEMBER(taitosj_state::input_port_4_f0_r)
        public ioport_value input_port_4_f0_r()
        {
            return m_input_port_4_f0;
        }


        void taitosj_main_nomcu_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x5fff).rom();
            map.op(0x6000, 0x7fff).bankr("bank1");
            map.op(0x8000, 0x87ff).ram();
            map.op(0x8800, 0x8800).mirror(0x07fe).rw(taitosj_fake_data_r, taitosj_fake_data_w);
            map.op(0x8801, 0x8801).mirror(0x07fe).r(taitosj_fake_status_r);
            map.op(0x9000, 0xbfff).w(taitosj_characterram_w).share("characterram");
            map.op(0xc000, 0xc3ff).ram();
            map.op(0xc400, 0xc7ff).ram().share("videoram_1");
            map.op(0xc800, 0xcbff).ram().share("videoram_2");
            map.op(0xcc00, 0xcfff).ram().share("videoram_3");
            map.op(0xd000, 0xd05f).ram().share("colscrolly");
            map.op(0xd100, 0xd1ff).ram().share("spriteram");
            map.op(0xd200, 0xd27f).mirror(0x0080).ram().share("paletteram");
            map.op(0xd300, 0xd300).mirror(0x00ff).writeonly().share("video_priority");
            map.op(0xd400, 0xd403).mirror(0x00f0).readonly_().share("collision_reg");
            map.op(0xd404, 0xd404).mirror(0x00f3).r(taitosj_gfxrom_r);
            map.op(0xd408, 0xd408).mirror(0x00f0).portr("IN0");
            map.op(0xd409, 0xd409).mirror(0x00f0).portr("IN1");
            map.op(0xd40a, 0xd40a).mirror(0x00f0).portr("DSW1");         /* DSW1 */
            map.op(0xd40b, 0xd40b).mirror(0x00f0).portr("IN2");
            map.op(0xd40c, 0xd40c).mirror(0x00f0).portr("IN3");          /* Service */
            map.op(0xd40d, 0xd40d).mirror(0x00f0).portr("IN4");
            map.op(0xd40e, 0xd40f).mirror(0x00f0).w(m_ay1, (offset, data) => { m_ay1.target.address_data_w(offset, data); });  //m_ay1, FUNC(ay8910_device::address_data_w));
            map.op(0xd40f, 0xd40f).mirror(0x00f0).r(m_ay1, () => { return m_ay1.target.data_r(); });  //m_ay1, FUNC(ay8910_device::data_r));   /* DSW2 and DSW3 */
            map.op(0xd500, 0xd505).mirror(0x00f0).writeonly().share("scroll");
            map.op(0xd506, 0xd507).mirror(0x00f0).writeonly().share("colorbank");
            map.op(0xd508, 0xd508).mirror(0x00f0).w(taitosj_collision_reg_clear_w);
            map.op(0xd509, 0xd50a).mirror(0x00f0).writeonly().share("gfxpointer");
            map.op(0xd50b, 0xd50b).mirror(0x00f0).w(soundlatch_w);
            map.op(0xd50c, 0xd50c).mirror(0x00f0).w(sound_semaphore2_w);
            map.op(0xd50d, 0xd50d).mirror(0x00f0).w("watchdog", (data) => { ((watchdog_timer_device)subdevice("watchdog")).reset_w(data); });  //FUNC(watchdog_timer_device::reset_w));
            map.op(0xd50e, 0xd50e).mirror(0x00f0).w(taitosj_bankswitch_w);
            map.op(0xd50f, 0xd50f).mirror(0x00f0).nopw();
            map.op(0xd600, 0xd600).mirror(0x00ff).writeonly().share("video_mode");
            map.op(0xd700, 0xdfff).noprw();
            map.op(0xe000, 0xffff).rom();
        }


        /* only difference is taitosj_fake_ replaced with taitosj_mcu_ */
        void taitosj_main_mcu_map(address_map map, device_t device)
        {
            taitosj_main_nomcu_map(map, m_globals.helper_device);
            map.op(0x8800, 0x8801).mirror(0x07fe).rw(m_mcu, (space, offset, mem_mask) => { return m_mcu.target.data_r(space, offset, mem_mask); }, (space, offset, data, mem_mask) => { m_mcu.target.data_w(space, offset, data, mem_mask); });  //FUNC(taito_sj_security_mcu_device::data_r), FUNC(taito_sj_security_mcu_device::data_w));
        }


        //TIMER_CALLBACK_MEMBER(taitosj_state::soundlatch_w_cb)
        void soundlatch_w_cb(object ptr, int param)
        {
            if (m_soundlatch_flag && (m_soundlatch_data != param))
                logerror("Warning: soundlatch written before being read. Previous: {0}, new: {1}\n", m_soundlatch_data, param);

            m_soundlatch_data = (uint8_t)param;
            m_soundlatch_flag = true;
            m_soundnmi.target.in_w(1, 1);
        }


        //TIMER_CALLBACK_MEMBER(taitosj_state::soundlatch_clear7_w_cb)
        void soundlatch_clear7_w_cb(object ptr, int param)
        {
            if (m_soundlatch_flag)
                logerror("Warning: soundlatch bit 7 cleared before being read. Previous: {0}, new: {1}\n", m_soundlatch_data, m_soundlatch_data & 0x7f);

            m_soundlatch_data &= 0x7F;
        }


        //TIMER_CALLBACK_MEMBER(taitosj_state::sound_semaphore2_w_cb)
        void sound_semaphore2_w_cb(object ptr, int param)
        {
            m_sound_semaphore2 = (param & 1) != 0;
            m_soundnmi2.target.in_w(1, (param & 1));
        }


        //TIMER_CALLBACK_MEMBER(taitosj_state::sound_semaphore2_clear_w_cb)
        void sound_semaphore2_clear_w_cb(object ptr, int param)
        {
            m_sound_semaphore2 = false;
            m_soundnmi2.target.in_w(1, 0);
        }


        // RD5000
        //READ8_MEMBER(taitosj_state::soundlatch_r)
        u8 soundlatch_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            if (!machine().side_effects_disabled())
            {
                m_soundlatch_flag = false;
                m_soundnmi.target.in_w(1, 0);
            }

            return m_soundlatch_data;
        }


        // RD5001
        //READ8_MEMBER(taitosj_state::soundlatch_flags_r)
        u8 soundlatch_flags_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return (u8)((m_soundlatch_flag ? 8 : 0) | (m_sound_semaphore2 ? 4 : 0) | 3);
        }


        // WR5000
        //WRITE8_MEMBER(taitosj_state::soundlatch_clear7_w)
        void soundlatch_clear7_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            machine().scheduler().synchronize(soundlatch_clear7_w_cb, data);
        }


        // WR5001
        //WRITE8_MEMBER(taitosj_state::sound_semaphore2_clear_w)
        void sound_semaphore2_clear_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            machine().scheduler().synchronize(sound_semaphore2_clear_w_cb, data);
        }


        void taitosj_audio_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x3fff).rom();
            map.op(0x4000, 0x43ff).ram();
            map.op(0x4800, 0x4801).mirror(0x07f8).w(m_ay2, (data) => { m_ay2.target.data_w(data); });  //FUNC(ay8910_device::address_data_w));
            map.op(0x4801, 0x4801).mirror(0x07f8).r(m_ay2, () => { return m_ay2.target.data_r(); });  //FUNC(ay8910_device::data_r));
            map.op(0x4802, 0x4803).mirror(0x07f8).w(m_ay3, (data) => { m_ay3.target.data_w(data); });  //FUNC(ay8910_device::address_data_w));
            map.op(0x4803, 0x4803).mirror(0x07f8).r(m_ay3, () => { return m_ay3.target.data_r(); });  //FUNC(ay8910_device::data_r));
            map.op(0x4804, 0x4805).mirror(0x07fa).w(m_ay4, (data) => { m_ay4.target.data_w(data); });  //FUNC(ay8910_device::address_data_w));
            map.op(0x4805, 0x4805).mirror(0x07fa).r(m_ay4, () => { return m_ay4.target.data_r(); });  //FUNC(ay8910_device::data_r));
            map.op(0x5000, 0x5000).mirror(0x07fc).rw(soundlatch_r, soundlatch_clear7_w);
            map.op(0x5001, 0x5001).mirror(0x07fc).rw(soundlatch_flags_r, sound_semaphore2_clear_w);
            map.op(0xe000, 0xefff).rom(); /* space for diagnostic ROM */
        }
    }


    partial class taitosj : global_object
    {
        void DSW2_PORT()
        {
            PORT_DIPNAME( 0x0f, 0x00, DEF_STR( Coin_A ) );
            PORT_DIPSETTING(    0x0f, DEF_STR( _9C_1C ) );
            PORT_DIPSETTING(    0x0e, DEF_STR( _8C_1C ) );
            PORT_DIPSETTING(    0x0d, DEF_STR( _7C_1C ) );
            PORT_DIPSETTING(    0x0c, DEF_STR( _6C_1C ) );
            PORT_DIPSETTING(    0x0b, DEF_STR( _5C_1C ) );
            PORT_DIPSETTING(    0x0a, DEF_STR( _4C_1C ) );
            PORT_DIPSETTING(    0x09, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x08, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x01, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _1C_3C ) );
            PORT_DIPSETTING(    0x03, DEF_STR( _1C_4C ) );
            PORT_DIPSETTING(    0x04, DEF_STR( _1C_5C ) );
            PORT_DIPSETTING(    0x05, DEF_STR( _1C_6C ) );
            PORT_DIPSETTING(    0x06, DEF_STR( _1C_7C ) );
            PORT_DIPSETTING(    0x07, DEF_STR( _1C_8C ) );
            PORT_DIPNAME( 0xf0, 0x00, DEF_STR( Coin_B ) );
            PORT_DIPSETTING(    0xf0, DEF_STR( _9C_1C ) );
            PORT_DIPSETTING(    0xe0, DEF_STR( _8C_1C ) );
            PORT_DIPSETTING(    0xd0, DEF_STR( _7C_1C ) );
            PORT_DIPSETTING(    0xc0, DEF_STR( _6C_1C ) );
            PORT_DIPSETTING(    0xb0, DEF_STR( _5C_1C ) );
            PORT_DIPSETTING(    0xa0, DEF_STR( _4C_1C ) );
            PORT_DIPSETTING(    0x90, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x80, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x10, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x20, DEF_STR( _1C_3C ) );
            PORT_DIPSETTING(    0x30, DEF_STR( _1C_4C ) );
            PORT_DIPSETTING(    0x40, DEF_STR( _1C_5C ) );
            PORT_DIPSETTING(    0x50, DEF_STR( _1C_6C ) );
            PORT_DIPSETTING(    0x60, DEF_STR( _1C_7C ) );
            PORT_DIPSETTING(    0x70, DEF_STR( _1C_8C ) );
        }


        void COMMON_IN0()
        {
            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_BUTTON2 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_UNKNOWN );
        }


        void COMMON_IN1()
        {
            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_BUTTON2 ); PORT_COCKTAIL();
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_UNKNOWN );
        }


        void COMMON_IN2()
        {
            PORT_START("IN2");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_START2 );
        }


        void COMMON_IN3(ioport_value coin3state)
        {
            PORT_START("IN3");      /* Service */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x10, coin3state, IPT_COIN3 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_TILT );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_UNKNOWN );
        }


        //static INPUT_PORTS_START( junglek )
        void construct_ioport_junglek(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            taitosj_state taitosj_state = (taitosj_state)owner;

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_UNKNOWN );

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_UNKNOWN );

            COMMON_IN2();

            COMMON_IN3(IP_ACTIVE_HIGH);

            PORT_START("IN4");
            PORT_BIT( 0x0f, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0xf0, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_CUSTOM_MEMBER(DEVICE_SELF, taitosj_state.input_port_4_f0_r);    // from sound CPU

            PORT_START("DSW1");
            PORT_DIPNAME( 0x03, 0x03, "Finish Bonus" );
            PORT_DIPSETTING(    0x03, DEF_STR( None ) );
            PORT_DIPSETTING(    0x02, "Timer x1" );
            PORT_DIPSETTING(    0x01, "Timer x2" );
            PORT_DIPSETTING(    0x00, "Timer x3" );
            PORT_DIPNAME( 0x04, 0x04, DEF_STR( Unused ) );
            PORT_DIPSETTING(    0x04, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x18, 0x18, DEF_STR( Lives ) );
            PORT_DIPSETTING(    0x18, "3" );
            PORT_DIPSETTING(    0x10, "4" );
            PORT_DIPSETTING(    0x08, "5" );
            PORT_DIPSETTING(    0x00, "6" );
            PORT_SERVICE( 0x20, IP_ACTIVE_LOW );
            PORT_DIPNAME( 0x40, 0x00, DEF_STR( Flip_Screen ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x40, DEF_STR( On ) );
            PORT_DIPNAME( 0x80, 0x00, DEF_STR( Cabinet ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x80, DEF_STR( Cocktail ) );

            PORT_START("DSW2");      /* Coinage */
            DSW2_PORT();

            PORT_START("DSW3");
            PORT_DIPNAME( 0x03, 0x03, DEF_STR( Bonus_Life ) );
            PORT_DIPSETTING(    0x02, "10000" );
            PORT_DIPSETTING(    0x01, "20000" );
            PORT_DIPSETTING(    0x00, "30000" );
            PORT_DIPSETTING(    0x03, DEF_STR( None ) );
            PORT_DIPNAME( 0x04, 0x04, DEF_STR( Unused ) );
            PORT_DIPSETTING(    0x04, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x08, 0x08, DEF_STR( Unused ) );
            PORT_DIPSETTING(    0x08, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x10, 0x10, DEF_STR( Unused ) );
            PORT_DIPSETTING(    0x10, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x20, 0x20, "Year Display" );
            PORT_DIPSETTING(    0x00, DEF_STR( No ) );
            PORT_DIPSETTING(    0x20, DEF_STR( Yes ) );
            PORT_DIPNAME( 0x40, 0x40, "Infinite Lives (Cheat)");
            PORT_DIPSETTING(    0x40, DEF_STR( No ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Yes ) );
            PORT_DIPNAME( 0x80, 0x80, DEF_STR( Coinage ) );
            PORT_DIPSETTING(    0x80, "A and B" );
            PORT_DIPSETTING(    0x00, "A only" );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( elevator )
        void construct_ioport_elevator(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            taitosj_state taitosj_state = (taitosj_state)owner;

            COMMON_IN0();

            COMMON_IN1();

            COMMON_IN2();

            COMMON_IN3(IP_ACTIVE_HIGH);

            PORT_START("IN4");
            PORT_BIT( 0x0f, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0xf0, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_CUSTOM_MEMBER(DEVICE_SELF, taitosj_state.input_port_4_f0_r);    // from sound CPU

            PORT_START("DSW1");
            PORT_DIPNAME( 0x03, 0x03, DEF_STR( Bonus_Life ) );    PORT_DIPLOCATION("SW1:1,2");
            PORT_DIPSETTING(    0x03, "10000" );
            PORT_DIPSETTING(    0x02, "15000" );
            PORT_DIPSETTING(    0x01, "20000" );
            PORT_DIPSETTING(    0x00, "25000" );
            PORT_DIPNAME( 0x04, 0x04, DEF_STR( Free_Play ) );     PORT_DIPLOCATION("SW1:3");
            PORT_DIPSETTING(    0x04, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x18, 0x18, DEF_STR( Lives ) );         PORT_DIPLOCATION("SW1:4,5");
            PORT_DIPSETTING(    0x18, "3" );
            PORT_DIPSETTING(    0x10, "4" );
            PORT_DIPSETTING(    0x08, "5" );
            PORT_DIPSETTING(    0x00, "6" );
            PORT_DIPNAME( 0x20, 0x20, DEF_STR( Unknown ) );
            PORT_DIPSETTING(    0x20, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x40, 0x40, DEF_STR( Flip_Screen ) );   PORT_DIPLOCATION("SW1:7");
            PORT_DIPSETTING(    0x40, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x80, 0x00, DEF_STR( Cabinet ) );       PORT_DIPLOCATION("SW1:8");
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x80, DEF_STR( Cocktail ) );

            PORT_START("DSW2");      /* Coinage */
            DSW2_PORT();

            PORT_START("DSW3");
            PORT_DIPNAME( 0x03, 0x03, DEF_STR( Difficulty ) );    PORT_DIPLOCATION("SW3:1,2");
            PORT_DIPSETTING(    0x03, DEF_STR( Easiest ) );
            PORT_DIPSETTING(    0x02, DEF_STR( Easy ) );
            PORT_DIPSETTING(    0x01, DEF_STR( Normal ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Hard ) );
            PORT_DIPNAME( 0x04, 0x04, DEF_STR( Unknown ) );
            PORT_DIPSETTING(    0x04, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x08, 0x08, DEF_STR( Unknown ) );
            PORT_DIPSETTING(    0x08, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x10, 0x10, "Coinage Display" );        PORT_DIPLOCATION("SW3:5");
            PORT_DIPSETTING(    0x10, "Coins/Credits" );
            PORT_DIPSETTING(    0x00, "Insert Coin" );
            PORT_DIPNAME( 0x20, 0x20, "Year Display" );           PORT_DIPLOCATION("SW1:6");
            PORT_DIPSETTING(    0x00, DEF_STR( No ) );
            PORT_DIPSETTING(    0x20, DEF_STR( Yes ) );
            PORT_DIPNAME( 0x40, 0x40, "Invulnerability (Cheat)"); PORT_DIPLOCATION("SW3:7");
            PORT_DIPSETTING(    0x40, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x80, 0x80, DEF_STR( Coinage ) );       PORT_DIPLOCATION("SW3:8");
            PORT_DIPSETTING(    0x80, "A and B" );
            PORT_DIPSETTING(    0x00, "A only" );

            INPUT_PORTS_END();
        }
    }


    partial class taitosj_state : driver_device
    {
        static readonly gfx_layout charlayout = new gfx_layout
        (
            8,8,    /* 8*8 characters */
            256,    /* 256 characters */
            3,      /* 3 bits per pixel */
            new u32 [] { 512*8*8, 256*8*8, 0 },        /* the bitplanes are separated */
            new u32 [] { 7, 6, 5, 4, 3, 2, 1, 0 },
            new u32 [] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
            8*8     /* every char takes 8 consecutive bytes */
        );

        static readonly gfx_layout spritelayout = new gfx_layout
        (
            16,16,  /* 16*16 sprites */
            64,             /* 64 sprites */
            3,      /* 3 bits per pixel */
            new u32 [] { 128*16*16, 64*16*16, 0 },     /* the bitplanes are separated */
            new u32 [] { 7, 6, 5, 4, 3, 2, 1, 0,
                8*8+7, 8*8+6, 8*8+5, 8*8+4, 8*8+3, 8*8+2, 8*8+1, 8*8+0 },
            new u32 [] { 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
                    16*8, 17*8, 18*8, 19*8, 20*8, 21*8, 22*8, 23*8 },
            32*8    /* every sprite takes 32 consecutive bytes */
        );


        //static GFXDECODE_START( gfx_taitosj )
        static readonly gfx_decode_entry [] gfx_taitosj = new gfx_decode_entry[]
        {
            GFXDECODE_ENTRY( null, 0x9000, charlayout,   0, 8 ),    /* the game dynamically modifies this */
            GFXDECODE_ENTRY( null, 0x9000, spritelayout, 0, 8 ),    /* the game dynamically modifies this */
            GFXDECODE_ENTRY( null, 0xa800, charlayout,   0, 8 ),    /* the game dynamically modifies this */
            GFXDECODE_ENTRY( null, 0xa800, spritelayout, 0, 8 ),    /* the game dynamically modifies this */
            //GFXDECODE_END
        };


        static readonly discrete_dac_r1_ladder taitosj_dacvol_ladder = new discrete_dac_r1_ladder
        (
            8,          // size of ladder
            new double [] { RES_K(680), RES_K(330), RES_K(150), RES_K(82), RES_K(39), RES_K(20), RES_K(10), RES_K(4.7) },
            0,
            0,          // no rBias
            0,          // no rGnd
            0           // no cap
        );


        //DISCRETE_SOUND_START(taitosj_dacvol_discrete)
        protected static readonly discrete_block [] taitosj_dacvol_discrete = new discrete_block []
        {
            DISCRETE_INPUT_DATA(NODE_01),
            DISCRETE_DAC_R1(NODE_02, NODE_01, DEFAULT_TTL_V_LOGIC_1, taitosj_dacvol_ladder),
            DISCRETE_OUTPUT(NODE_02, 9637),

            DISCRETE_SOUND_END
        };


        //WRITE8_MEMBER(taitosj_state::taitosj_dacvol_w)
        void taitosj_dacvol_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_dacvol.target.write(NODE_01, (uint8_t)(data ^ 0xff)); // 7416 hex inverter
        }


        public void nomcu(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, new XTAL(8000000)/2);      /* 8 MHz / 2, on CPU board */
            m_maincpu.target.memory().set_addrmap(AS_PROGRAM, taitosj_main_nomcu_map);
            m_maincpu.target.execute().set_vblank_int("screen", irq0_line_hold);

            Z80(config, m_audiocpu, new XTAL(6000000)/2);    /* 6 MHz / 2, on GAME board */
            m_audiocpu.target.memory().set_addrmap(AS_PROGRAM, taitosj_audio_map);
                    /* interrupts: */
                    /* - no interrupts synced with vblank */
                    /* - NMI triggered by the main CPU */
                    /* - periodic IRQ, with frequency 6000000/(4*16*16*10*16) = 36.621 Hz, */
            m_audiocpu.target.execute().set_periodic_int(irq0_line_hold, attotime.from_hz(new XTAL(6000000)/(4*16*16*10*16)));

            /* video hardware */
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.target.set_refresh_hz(60);
            m_screen.target.set_vblank_time(ATTOSECONDS_IN_USEC(2500) /* not accurate */);
            m_screen.target.set_size(32*8, 32*8);
            m_screen.target.set_visarea(0*8, 32*8-1, 2*8, 30*8-1);
            m_screen.target.set_screen_update(screen_update_taitosj);
            m_screen.target.set_palette(m_palette);

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_taitosj);
            PALETTE(config, m_palette).set_entries(64);

            /* sound hardware */
            SPEAKER(config, "speaker").front_center();

            INPUT_MERGER_ALL_HIGH(config, m_soundnmi).output_handler().set(m_soundnmi2, (state) => { m_soundnmi2.target.in_w(0, state); }).reg();  //FUNC(input_merger_device::in_w<0>));

            INPUT_MERGER_ANY_HIGH(config, m_soundnmi2).output_handler().set_inputline(m_audiocpu, device_execute_interface.INPUT_LINE_NMI).reg();

            AY8910(config, m_ay1, new XTAL(6000000)/4); // 6mhz/4 on GAME board, AY-3-8910 @ IC53 (this is the only AY which uses proper mixing resistors, the 3 below have outputs tied together)
            m_ay1.target.port_a_read_callback().set_ioport("DSW2").reg();
            m_ay1.target.port_b_read_callback().set_ioport("DSW3").reg();
            m_ay1.target.disound.add_route(ALL_OUTPUTS, "speaker", 0.15);

            AY8910(config, m_ay2, new XTAL(6000000)/4); // 6mhz/4 on GAME board, AY-3-8910 @ IC51
            m_ay2.target.set_flags(ay8910_device.AY8910_SINGLE_OUTPUT);
            m_ay2.target.port_a_write_callback().set(m_dac, (space, offset, data, mem_mask) => { m_dac.target.data_w(space, offset, data, mem_mask); }).reg();  //FUNC(dac_byte_interface::data_w));
            m_ay2.target.port_b_write_callback().set(taitosj_dacvol_w).reg();
            m_ay2.target.disound.add_route(ALL_OUTPUTS, "speaker", 0.5);

            AY8910(config, m_ay3, new XTAL(6000000)/4); // 6mhz/4 on GAME board, AY-3-8910 @ IC49
            m_ay3.target.set_flags(ay8910_device.AY8910_SINGLE_OUTPUT);
            m_ay3.target.port_a_write_callback().set(input_port_4_f0_w).reg();
            m_ay3.target.disound.add_route(ALL_OUTPUTS, "speaker", 0.5);

            AY8910(config, m_ay4, new XTAL(6000000)/4); // 6mhz/4 on GAME board, AY-3-8910 @ IC50
            m_ay4.target.set_flags(ay8910_device.AY8910_SINGLE_OUTPUT);
            /* TODO: Implement ay4 Port A bits 0 and 1 which connect to a 7416 open
               collector inverter, to selectively tie none, either or both of two
               capacitors between the ay4 audio output signal and ground, or between
               audio output signal and high-z (i.e. do nothing).
               Bio Attack uses this?
            */
            m_ay4.target.port_b_write_callback().set(taitosj_sndnmi_msk_w).reg();
            m_ay4.target.disound.add_route(ALL_OUTPUTS, "speaker", 1.0);

            WATCHDOG_TIMER(config, "watchdog").set_vblank_count("screen", 128); // 74LS393 on CPU board, counts 128 vblanks before firing watchdog

            DAC_8BIT_R2R(config, m_dac, 0).disound.add_route(ALL_OUTPUTS, "speaker", 0.15); // 30k r-2r network
            DISCRETE(config, m_dacvol, taitosj_dacvol_discrete);
            m_dacvol.target.disound.add_route(0, "dac", 1.0, dac_global.DAC_VREF_POS_INPUT);
            m_dacvol.target.disound.add_route(0, "dac", -1.0, dac_global.DAC_VREF_NEG_INPUT);
        }


        /* same as above, but with additional 68705 MCU */
        public void mcu(machine_config config)
        {
            nomcu(config);

            /* basic machine hardware */
            m_maincpu.target.memory().set_addrmap(AS_PROGRAM, taitosj_main_mcu_map);

            TAITO_SJ_SECURITY_MCU(config, m_mcu, new XTAL(3000000));   /* xtal is 3MHz, divided by 4 internally */
            m_mcu.target.set_int_mode(taito_sj_security_mcu_device.int_mode.LATCH);
            m_mcu.target.m68read_cb().set(mcu_mem_r).reg();
            m_mcu.target.m68write_cb().set(mcu_mem_w).reg();
            m_mcu.target.m68intrq_cb().set(mcu_intrq_w).reg();
            m_mcu.target.busrq_cb().set(mcu_busrq_w).reg();

            config.set_maximum_quantum(attotime.from_hz(6000));
        }
    }


    partial class taitosj : global_object
    {
        /***************************************************************************
          Game driver(s)
        ***************************************************************************/

        //ROM_START( junglek )
        static readonly List<tiny_rom_entry> rom_junglek = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x12000, "maincpu", 0 ),
            ROM_LOAD( "kn21-1.bin",   0x00000, 0x1000, CRC("45f55d30") + SHA1("bb9518d7728938f673a663801e47ae0438cdbea1") ),
            ROM_LOAD( "kn22-1.bin",   0x01000, 0x1000, CRC("07cc9a21") + SHA1("3fe35935e0a430ab0edc6a762623972fa37ea926") ),
            ROM_LOAD( "kn43.bin",     0x02000, 0x1000, CRC("a20e5a48") + SHA1("af961b671dc4c865d0181d08a70b902bb96f29d0") ),
            ROM_LOAD( "kn24.bin",     0x03000, 0x1000, CRC("19ea7f83") + SHA1("2399cc89f73811575c3f644d5c04ef13ceec6838") ),
            ROM_LOAD( "kn25.bin",     0x04000, 0x1000, CRC("844365ea") + SHA1("af34712620e4b784a5014283d3111048c5f81a56") ),
            ROM_LOAD( "kn46.bin",     0x05000, 0x1000, CRC("27a95fd5") + SHA1("160ee5d11126ac4155b479e43ec1bd6a4e9e21e7") ),
            ROM_LOAD( "kn47.bin",     0x06000, 0x1000, CRC("5c3199e0") + SHA1("c57dec92998b971d76aecd23674c25cf7b8be667") ),
            ROM_LOAD( "kn28.bin",     0x07000, 0x1000, CRC("194a2d09") + SHA1("88999493e470acdcf932efff71cd6155387a63d0") ),
            /* 10000-10fff space for another banked ROM (not used) */
            ROM_LOAD( "kn60.bin",     0x11000, 0x1000, CRC("1a9c0a26") + SHA1("82f4cebeba90419e83a00427b671985824babd7a") ), /* banked at 7000 */

            ROM_REGION( 0x10000, "audiocpu", 0 ),
            ROM_LOAD( "kn37.bin",     0x0000, 0x1000, CRC("dee7f5d4") + SHA1("cd8179a17ccd054fb470c4eee97192c2dd226397") ),
            ROM_LOAD( "kn38.bin",     0x1000, 0x1000, CRC("bffd3d21") + SHA1("a2b3393e9694d6979d39ab0f1ab82b7ef892b3da") ),
            ROM_LOAD( "kn59-1.bin",   0x2000, 0x1000, CRC("cee485fc") + SHA1("1e0c52ec6b1d3cfd47247db71bcf3fe476c32039") ),

            ROM_REGION( 0x8000, "gfx1", 0 ),   /* graphic ROMs used at runtime */
            ROM_LOAD( "kn29.bin",     0x0000, 0x1000, CRC("8f83c290") + SHA1("aa95ed2d2e15f573e092e8eed7d80479512d9409") ),
            ROM_LOAD( "kn30.bin",     0x1000, 0x1000, CRC("89fd19f1") + SHA1("fc7dfe3a1d78ac37a036fa9d8ebf3a33a2f4cbe8") ),
            ROM_LOAD( "kn51.bin",     0x2000, 0x1000, CRC("70e8fc12") + SHA1("505c90c662d372d28cb38201433054b8e3d723d1") ),
            ROM_LOAD( "kn52.bin",     0x3000, 0x1000, CRC("bcbac1a3") + SHA1("bcd5fc9b3791ab67e0ad9f9ced7226853e9a2a00") ),
            ROM_LOAD( "kn53.bin",     0x4000, 0x1000, CRC("b946c87d") + SHA1("d16cb6bf38e00ae11c204cbf8f400f8a85c807c2") ),
            ROM_LOAD( "kn34.bin",     0x5000, 0x1000, CRC("320db2e1") + SHA1("ca8722010712302b491eb5f51d73043bcb2ddc8f") ),
            ROM_LOAD( "kn55.bin",     0x6000, 0x1000, CRC("70aef58f") + SHA1("df7454a1c3676181eca698bb3b2ef3253a45ca0f") ),
            ROM_LOAD( "kn56.bin",     0x7000, 0x1000, CRC("932eb667") + SHA1("4bf7c01ab212b616931a21a43a453521aa01ff36") ),

            ROM_REGION( 0x0100, "proms", 0 ),      /* layer PROM */
            ROM_LOAD( "eb16.22",      0x0000, 0x0100, CRC("b833b5ea") + SHA1("d233f1bf8a3e6cd876853ffd721b9b64c61c9047") ),

            ROM_END
        };


        //ROM_START( jungleh )
        static readonly List<tiny_rom_entry> rom_jungleh = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x12000, "maincpu", 0 ),
            ROM_LOAD( "kn41a",        0x00000, 0x1000, CRC("6bf118d8") + SHA1("d6de28766aab90b5dbca7f74612ec8eafd144348") ),
            ROM_LOAD( "kn42.bin",     0x01000, 0x1000, CRC("bade53af") + SHA1("c3d2cf776598cb2d8684fa0b3ea7af90af9e8dae") ),
            ROM_LOAD( "kn43.bin",     0x02000, 0x1000, CRC("a20e5a48") + SHA1("af961b671dc4c865d0181d08a70b902bb96f29d0") ),
            ROM_LOAD( "kn44.bin",     0x03000, 0x1000, CRC("44c770d3") + SHA1("57a1ddc07009f0dbd423cbe111b886e919a8bb0a") ),
            ROM_LOAD( "kn45.bin",     0x04000, 0x1000, CRC("f60a3d06") + SHA1("7c387f0aeb9497b026d8838ee6ea7ff11dea506a") ),
            ROM_LOAD( "kn46a",        0x05000, 0x1000, CRC("ac89c155") + SHA1("bac17c9828002b644f15933149a205a008a561d3") ),
            ROM_LOAD( "kn47.bin",     0x06000, 0x1000, CRC("5c3199e0") + SHA1("c57dec92998b971d76aecd23674c25cf7b8be667") ),
            ROM_LOAD( "kn48a",        0x07000, 0x1000, CRC("ef80e931") + SHA1("b3ddcc37860a2693d45a85970926662cbb96bd0e") ),
            /* 10000-10fff space for another banked ROM (not used) */
            ROM_LOAD( "kn60.bin",     0x11000, 0x1000, CRC("1a9c0a26") + SHA1("82f4cebeba90419e83a00427b671985824babd7a") ), /* banked at 7000 */

            ROM_REGION( 0x10000, "audiocpu", 0 ),
            ROM_LOAD( "kn57-1.bin",   0x0000, 0x1000, CRC("62f6763a") + SHA1("84eadbc5c6a37c53c104e4ac1fd273b6b2a335e5") ),
            ROM_LOAD( "kn58-1.bin",   0x1000, 0x1000, CRC("9ef46c7f") + SHA1("867d9352cde4d6496f59e790cbbf15302a55364e") ),
            ROM_LOAD( "kn59-1.bin",   0x2000, 0x1000, CRC("cee485fc") + SHA1("1e0c52ec6b1d3cfd47247db71bcf3fe476c32039") ),

            ROM_REGION( 0x8000, "gfx1", 0 ),       /* graphic ROMs used at runtime */
            ROM_LOAD( "kn49a",        0x0000, 0x1000, CRC("b139e792") + SHA1("10c39abc49786154396c00bd35a51b826e5f6bd0") ),
            ROM_LOAD( "kn50a",        0x1000, 0x1000, CRC("1046019f") + SHA1("b2d3ab8a53ef3ca55165a5bda9be0829f53be6c9") ),
            ROM_LOAD( "kn51a",        0x2000, 0x1000, CRC("da50c8a4") + SHA1("de5f9b953f277986679ab958772571d8417a0ce2") ),
            ROM_LOAD( "kn52a",        0x3000, 0x1000, CRC("0444f06c") + SHA1("80569807ae36b4c5ad90e9e736ce9d0d0ea486ec") ),
            ROM_LOAD( "kn53a",        0x4000, 0x1000, CRC("6a17803e") + SHA1("d7ab6a240bb1ac80d3903cb694e55fd6d3670faa") ),
            ROM_LOAD( "kn54a",        0x5000, 0x1000, CRC("d41428c7") + SHA1("8c926db731073313daced31a168da6ac07a6d5cb") ),
            ROM_LOAD( "kn55.bin",     0x6000, 0x1000, CRC("70aef58f") + SHA1("df7454a1c3676181eca698bb3b2ef3253a45ca0f") ),
            ROM_LOAD( "kn56a",        0x7000, 0x1000, CRC("679c1101") + SHA1("218cd75f77c858c3714a8f03aea2c7ee88a212dd") ),

            ROM_REGION( 0x0100, "proms", 0 ),      /* layer PROM */
            ROM_LOAD( "eb16.22",      0x0000, 0x0100, CRC("b833b5ea") + SHA1("d233f1bf8a3e6cd876853ffd721b9b64c61c9047") ),

            ROM_END
        };


        //ROM_START( elevator ) // 5 board set, using 2732s on both mainboard and square rom board, and 68705 on daughterboard at bottom of stack, upside down
        static readonly List<tiny_rom_entry> rom_elevator = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x12000, "maincpu", 0 ), // on CPU BOARD
            ROM_LOAD( "ea_12.2732.ic69",  0x0000, 0x1000, CRC("24e277ef") + SHA1("764e3b3a34bf0ec849d58023f710e5b0a0d0ccb5") ), // needs label verified
            ROM_LOAD( "ea_13.2732.ic68",  0x1000, 0x1000, CRC("13702e39") + SHA1("b72fea84f8322463ff224e3b06698a1ed7e305b7") ), // needs label verified
            ROM_LOAD( "ea_14.2732.ic67",  0x2000, 0x1000, CRC("46f52646") + SHA1("11b68b89ab0f580bfe88047e59bd9bba237a2eb4") ), // needs label verified
            ROM_LOAD( "ea_15.2732.ic66",  0x3000, 0x1000, CRC("e22fe57e") + SHA1("50888975e698c4d2a124e5731d0922df43eb01ef") ), // needs label verified
            ROM_LOAD( "ea_16.2732.ic65",  0x4000, 0x1000, CRC("c10691d7") + SHA1("a7657d3d661421d1fca3b04e4025725272b77203") ), // needs label verified \ one of these is probably -1
            ROM_LOAD( "ea_17.2732.ic64",  0x5000, 0x1000, CRC("8913b293") + SHA1("163daa07b6d45469f18e4f4a1904b60a890c8699") ), // needs label verified /
            ROM_LOAD( "ea_18.2732.ic55",  0x6000, 0x1000, CRC("1cabda08") + SHA1("8fff75a354ee7589bd0ffe8b0271fd9111b2b241") ), // needs label verified \ one of these is probably -1
            ROM_LOAD( "ea_19.2732.ic54",  0x7000, 0x1000, CRC("f4647b4f") + SHA1("711a9447d30b35bc38e149e0cf6e835ff06efd54") ), // needs label verified /
            /* 10000-11fff space for banked ROMs (not used) */

            ROM_REGION( 0x10000, "audiocpu", 0 ), // on GAME BOARD
            ROM_LOAD( "ea_9.2732.ic70",  0x0000, 0x1000, CRC("6d5f57cb") + SHA1("abb916d675ee85032697d656121d4f525202cab3") ),
            ROM_LOAD( "ea_10.2732.ic71", 0x1000, 0x1000, CRC("f0a769a1") + SHA1("9970fba3afeaaaa7fd217f0704fb9df9cf13cf65") ),

            ROM_REGION( 0x0800, "pal", 0 ), // on GAME BOARD
            ROM_LOAD( "ww15.pal16l8.ic24.jed.bin",  0x0000, 0x0117, CRC("c3ec20d6") + SHA1("4bcdd92ca6b75ba825a7f90b1f35d8dcaeaf8a96") ), // what format is this? jed2bin?

            ROM_REGION( 0x0800, "bmcu:mcu", 0 ),       /* 2k for the microcontroller */
            ROM_LOAD( "ba3__11.mc68705p3.ic4",       0x0000, 0x0800, CRC("9ce75afc") + SHA1("4c8f5d926ae2bec8fcb70692125b9e1c863166c6") ), // IC4 on the older Z80+security daughterboard; The MCU itself has a strange custom from-factory silkscreen, rather than "MC68705P3S" it is labeled "15-00011-001 // DA68237"

            ROM_REGION( 0x8000, "gfx1", 0 ),       /* graphic ROMs used at runtime, on Square ROM board */
            ROM_LOAD( "ea_20.2732.ic1",   0x0000, 0x1000, CRC("bbbb3fba") + SHA1("a8e3a0886ea5dc8e70aa280b4cef5fb26ca0e125") ),
            ROM_LOAD( "ea_21.2732.ic2",   0x1000, 0x1000, CRC("639cc2fd") + SHA1("0ba292ac34dbf779a929db6358cd842d38077b3d") ),
            ROM_LOAD( "ea_22.2732.ic3",   0x2000, 0x1000, CRC("61317eea") + SHA1("f1a18c09e31edb4ec3ad7ab853f425383ca22314") ),
            ROM_LOAD( "ea_23.2732.ic4",   0x3000, 0x1000, CRC("55446482") + SHA1("0767701213920d30d5a3a226b25cfbbd3f24437a") ),
            ROM_LOAD( "ea_24.2732.ic5",   0x4000, 0x1000, CRC("77895c0f") + SHA1("fe116c53a7e8ac523a17249a56df9f40b503b30d") ),
            ROM_LOAD( "ea_25.2732.ic6",   0x5000, 0x1000, CRC("9a1b6901") + SHA1("646491c1d28904d9e662b1bff554bb74ec47708d") ),
            ROM_LOAD( "ea_26.2732.ic7",   0x6000, 0x1000, CRC("839112ec") + SHA1("30bca7f5214bf424aa10184094947496f054ddf4") ),
            ROM_LOAD( "ea_27.2732.ic8",   0x7000, 0x1000, CRC("db7ff692") + SHA1("4d0d9ab0c9d8d758e121f2bcfc6422ffadf2d760") ),

            ROM_REGION( 0x0100, "proms", 0 ),      /* layer PROM */
            ROM_LOAD( "eb16.ic22",      0x0000, 0x0100, CRC("b833b5ea") + SHA1("d233f1bf8a3e6cd876853ffd721b9b64c61c9047") ),

            ROM_END
        };
    }


    partial class taitosj_state : driver_device
    {
        void reset_common(running_machine machine)
        {
            m_sound_semaphore2 = false;
            m_soundnmi2.target.in_w(1, 0);
            m_soundlatch_data = 0xff;
            m_soundlatch_flag = false;
            m_soundnmi.target.in_w(1, 0);
            m_soundnmi.target.in_w(0, 0);
            m_sound_semaphore2 = false;
            m_ay1.target.disound.set_output_gain(0, 0.0f); // 3 outputs for Ay1 since it doesn't use tied together outs
            m_ay1.target.disound.set_output_gain(1, 0.0f);
            m_ay1.target.disound.set_output_gain(2, 0.0f);
            m_ay2.target.disound.set_output_gain(0, 0.0f);
            m_ay3.target.disound.set_output_gain(0, 0.0f);
            m_ay4.target.disound.set_output_gain(0, 0.0f);
            m_dac.target.disound.set_output_gain(0, 0.0f);
            m_input_port_4_f0 = 0;
            /* start in 1st gear */
            m_kikstart_gears[0] = 0x02;
            m_kikstart_gears[1] = 0x02;
        }


        void init_common()
        {
            save_item(NAME(new { m_soundlatch_data }));
            save_item(NAME(new { m_soundlatch_flag }));
            save_item(NAME(new { m_sound_semaphore2 }));
            save_item(NAME(new { m_input_port_4_f0 }));
            save_item(NAME(new { m_kikstart_gears }));

            machine().add_notifier(machine_notification.MACHINE_NOTIFY_RESET, reset_common);
        }


        public void init_taitosj()
        {
            init_common();
        }
    }


    partial class taitosj : global_object
    {
        static void taitosj_state_nomcu(machine_config config, device_t device) { taitosj_state taitosj_state = (taitosj_state)device; taitosj_state.nomcu(config); }
        static void taitosj_state_init_taitosj(device_t owner) { taitosj_state taitosj_state = (taitosj_state)owner; taitosj_state.init_taitosj(); }
        static void taitosj_state_mcu(machine_config config, device_t device) { taitosj_state taitosj_state = (taitosj_state)device; taitosj_state.mcu(config); }


        static taitosj m_taitosj = new taitosj();


        static device_t device_creator_junglek(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new taitosj_state(mconfig, (device_type)type, tag); }
        static device_t device_creator_jungleh(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new taitosj_state(mconfig, (device_type)type, tag); }
        static device_t device_creator_elevator(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new taitosj_state(mconfig, (device_type)type, tag); }


        //                                                                                                           rom          parent      machine               inp                                  init
        public static readonly game_driver driver_junglek  = GAME( device_creator_junglek,   rom_junglek,   "1982",  "junglek",   "0",        taitosj_state_nomcu,  m_taitosj.construct_ioport_junglek,  taitosj_state_init_taitosj, ROT180, "Taito Corporation", "Jungle King (Japan)", MACHINE_SUPPORTS_SAVE );
        public static readonly game_driver driver_jungleh  = GAME( device_creator_jungleh,   rom_jungleh,   "1982",  "jungleh",   "junglek",  taitosj_state_nomcu,  m_taitosj.construct_ioport_junglek,  taitosj_state_init_taitosj, ROT180, "Taito America Corporation", "Jungle Hunt (US)", MACHINE_SUPPORTS_SAVE );
        public static readonly game_driver driver_elevator = GAME( device_creator_elevator,  rom_elevator,  "1983",  "elevator",  "0",        taitosj_state_mcu,    m_taitosj.construct_ioport_elevator, taitosj_state_init_taitosj, ROT0,   "Taito Corporation", "Elevator Action (5 pcb version, 1.1)", MACHINE_SUPPORTS_SAVE );
    }
}
