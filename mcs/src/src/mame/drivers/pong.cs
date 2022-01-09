// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using ioport_value = System.UInt32;  //typedef u32 ioport_value;
using u16 = System.UInt16;
using u32 = System.UInt32;

using static mame.device_creator_helper_global;
using static mame.device_global;
using static mame.disound_global;
using static mame.emucore_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.ioport_global;
using static mame.ioport_ioport_type_helper;
using static mame.rescap_global;
using static mame.romentry_global;
using static mame.screen_global;


namespace mame
{
    class ttl_mono_state : driver_device
    {
        const int NS_PER_CLOCK_PONG = (int)((double)netlist.config.INTERNAL_RES / (double)7159000 + 0.5);  //static const int NS_PER_CLOCK_PONG  = static_cast<int>((double) netlist::config::INTERNAL_RES::value / (double) 7159000 + 0.5);
        protected const int MASTER_CLOCK_PONG = (int)((double)netlist.config.INTERNAL_RES / (double)NS_PER_CLOCK_PONG + 0.5);  //static const int MASTER_CLOCK_PONG  = static_cast<int>((double) netlist::config::INTERNAL_RES::value / (double) NS_PER_CLOCK_PONG + 0.5);

        protected const int V_TOTAL_PONG    = 0x105+1;       // 262
        protected const int H_TOTAL_PONG    = 0x1C5+1;       // 454

        //#define MASTER_CLOCK_BREAKOUT    (14318000)
        //static const int NS_PER_CLOCK_BREAKOUT  = static_cast<int>((double) netlist::config::INTERNAL_RES::value / (double) 14318000 + 0.5);
        //static const int MASTER_CLOCK_BREAKOUT  = static_cast<int>((double) netlist::config::INTERNAL_RES::value / (double) NS_PER_CLOCK_BREAKOUT + 0.5);

        //static const int V_TOTAL_BREAKOUT       = (0xFC);       // 252
        //static const int H_TOTAL_BREAKOUT       = (448*2);      // 448


        public enum input_changed_enum
        {
            IC_PADDLE1,
            IC_PADDLE2,
            IC_COIN,
            IC_SWITCH,
            IC_VR1,
            IC_VR2
        }


        // devices
        required_device<netlist_mame_device> m_maincpu;
        protected required_device<fixedfreq_device> m_video;
        protected required_device<dac_16bit_r2r_twos_complement_device> m_dac;  //required_device<dac_word_interface> m_dac; /* just to have a sound device */


        protected ttl_mono_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<netlist_mame_device>(this, "maincpu");
            m_video = new required_device<fixedfreq_device>(this, "fixfreq");
            m_dac = new required_device<dac_16bit_r2r_twos_complement_device>(this, "dac");                /* just to have a sound device */
        }


        //NETDEV_ANALOG_CALLBACK_MEMBER(sound_cb_analog)
        //{
        //    m_dac->write(std::round(16384 * data));
        //}

        //NETDEV_LOGIC_CALLBACK_MEMBER(sound_cb_logic)
        protected void sound_cb_logic(int data, attotime time)
        {
            m_dac.op0.write((u16)(16384 * data));
        }


        // driver_device overrides
        protected override void machine_start() { throw new emu_unimplemented(); }
        protected override void machine_reset() { throw new emu_unimplemented(); }

        protected override void video_start() { throw new emu_unimplemented(); }
    }


    partial class pong_state : ttl_mono_state
    {
        // sub devices
        required_device<netlist_mame_logic_input_device> m_sw1a;
        required_device<netlist_mame_logic_input_device> m_sw1b;


        public pong_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_sw1a = new required_device<netlist_mame_logic_input_device>(this, "maincpu:sw1a");
            m_sw1b = new required_device<netlist_mame_logic_input_device>(this, "maincpu:sw1b");
        }


        //NETLIST_START(pong)
        void netlist_pong(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.MEMREGION_SOURCE(this, "maincpu");
            h.PARAM("NETLIST.USE_DEACTIVATE", 1);
            h.INCLUDE("pong_schematics");

            h.NETLIST_END();
        }


        // driver_device overrides
        protected override void machine_start() { }
        protected override void machine_reset() { }
        protected override void video_start() { }
    }


    //class breakout_state : public ttl_mono_state
    //{
    //public:
    //    breakout_state(const machine_config &mconfig, device_type type, const char *tag)
    //        : ttl_mono_state(mconfig, type, tag),
    //        m_led_serve(*this, "maincpu:led_serve"),
    //        m_lamp_credit1(*this, "maincpu:lamp_credit1"),
    //        m_lamp_credit2(*this, "maincpu:lamp_credit2"),
    //        m_coin_counter(*this, "maincpu:coin_counter"),
    //        m_sw1_1(*this, "maincpu:sw1_1"),
    //        m_sw1_2(*this, "maincpu:sw1_2"),
    //        m_sw1_3(*this, "maincpu:sw1_3"),
    //        m_sw1_4(*this, "maincpu:sw1_4")
    //    {
    //    }
    //    required_device<netlist_mame_analog_output_device> m_led_serve;
    //    required_device<netlist_mame_analog_output_device> m_lamp_credit1;
    //    required_device<netlist_mame_analog_output_device> m_lamp_credit2;
    //    required_device<netlist_mame_analog_output_device> m_coin_counter;
    //
    //    required_device<netlist_mame_logic_input_device> m_sw1_1;
    //    required_device<netlist_mame_logic_input_device> m_sw1_2;
    //    required_device<netlist_mame_logic_input_device> m_sw1_3;
    //    required_device<netlist_mame_logic_input_device> m_sw1_4;
    //
    //    NETDEV_ANALOG_CALLBACK_MEMBER(serve_cb)
    //    {
    //        output().set_value("serve_led", (data < 3.5) ? 1 : 0);
    //    }
    //
    //    NETDEV_ANALOG_CALLBACK_MEMBER(credit1_cb)
    //    {
    //        output().set_value("lamp_credit1", (data < 2.0) ? 0 : 1);
    //    }
    //
    //    NETDEV_ANALOG_CALLBACK_MEMBER(credit2_cb)
    //    {
    //        output().set_value("lamp_credit2", (data < 2.0) ? 0 : 1);
    //    }
    //
    //    NETDEV_ANALOG_CALLBACK_MEMBER(coin_counter_cb)
    //    {
    //        machine().bookkeeping().coin_counter_w(0, (data > 2.0) ? 0 : 1);
    //    }
    //
    //    DECLARE_INPUT_CHANGED_MEMBER(cb_free_play)
    //    {
    //        m_sw1_1->write((newval>>0) & 1);
    //        m_sw1_2->write((newval>>1) & 1);
    //        m_sw1_3->write((newval>>2) & 1);
    //        m_sw1_4->write((newval>>3) & 1);
    //    }
    //
    //    void breakout(machine_config &config);
    //protected:
    //
    //    // driver_device overrides
    //    virtual void machine_start() override { };
    //    virtual void machine_reset() override { };
    //    virtual void video_start() override  { };
    //
    //private:
    //
    //};

    //class rebound_state : public ttl_mono_state
    //{
    //public:
    //    rebound_state(const machine_config &mconfig, device_type type, const char *tag)
    //        : ttl_mono_state(mconfig, type, tag)
    //        , m_sw1a(*this, "maincpu:dsw1a")
    //        , m_sw1b(*this, "maincpu:dsw1b")
    //    {
    //    }
    //
    //    // sub devices
    //    required_device<netlist_mame_logic_input_device> m_sw1a;
    //    required_device<netlist_mame_logic_input_device> m_sw1b;
    //
    //    DECLARE_INPUT_CHANGED_MEMBER(input_changed);
    //
    //    NETDEV_ANALOG_CALLBACK_MEMBER(led_credit_cb)
    //    {
    //        output().set_value("credit_led", (data < 3.5) ? 1 : 0);
    //    }
    //
    //    NETDEV_ANALOG_CALLBACK_MEMBER(coin_counter_cb)
    //    {
    //        machine().bookkeeping().coin_counter_w(0, (data < 1.0));
    //    }
    //
    //    void rebound(machine_config &config);
    //
    //protected:
    //
    //    // driver_device overrides
    //    virtual void machine_start() override { };
    //    virtual void machine_reset() override { };
    //    virtual void video_start() override  { };
    //
    //private:
    //
    //};


    partial class pong_state : ttl_mono_state
    {
        //INPUT_CHANGED_MEMBER(pong_state::input_changed)
        public void input_changed(ioport_field field, u32 param, ioport_value oldval, ioport_value newval)
        {
            throw new emu_unimplemented();
#if false
            int numpad = param;
    
            switch (numpad)
            {
            case IC_SWITCH:
                m_sw1a->write(newval ? 1 : 0);
                m_sw1b->write(newval ? 1 : 0);
                break;
            }
#endif
        }
    }


    //INPUT_CHANGED_MEMBER(rebound_state::input_changed)
    //{
    //    int numpad = param;
    //
    //    switch (numpad)
    //    {
    //    case IC_SWITCH:
    //        m_sw1a->write(newval ? 1 : 0);
    //        m_sw1b->write(newval ? 1 : 0);
    //        break;
    //    }
    //}


    partial class pong : construct_ioport_helper
    {
        //static INPUT_PORTS_START( pong )
        void construct_ioport_pong(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            pong_state pong_state = (pong_state)owner;

            PORT_START( "PADDLE0" ); /* fake input port for player 1 paddle */
            PORT_BIT( 0xff, 0x00, IPT_PADDLE ); PORT_SENSITIVITY(2); PORT_KEYDELTA(100); PORT_CENTERDELTA(0);   NETLIST_ANALOG_PORT_CHANGED("maincpu", "pot0", (field, param, oldval, newval) => { ((netlist_mame_analog_input_device)pong_state.subdevice("maincpu:pot0")).input_changed(field, param, oldval, newval); });

            PORT_START( "PADDLE1" ); /* fake input port for player 2 paddle */
            PORT_BIT( 0xff, 0x00, IPT_PADDLE ); PORT_SENSITIVITY(2); PORT_KEYDELTA(100); PORT_CENTERDELTA(0); PORT_PLAYER(2); NETLIST_ANALOG_PORT_CHANGED("maincpu", "pot1", (field, param, oldval, newval) => { ((netlist_mame_analog_input_device)pong_state.subdevice("maincpu:pot1")).input_changed(field, param, oldval, newval); });

            PORT_START("IN0"); /* fake as well */
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_COIN1 );     NETLIST_LOGIC_PORT_CHANGED("maincpu", "coinsw", (field, param, oldval, newval) => { ((netlist_mame_logic_input_device)pong_state.subdevice("maincpu:coinsw")).input_changed(field, param, oldval, newval); });

            PORT_DIPNAME( 0x06, 0x00, "Game Won" );          PORT_DIPLOCATION("SW1A:1,SW1B:1"); PORT_CHANGED_MEMBER(DEVICE_SELF, pong_state.input_changed, (u32)pong_state.input_changed_enum.IC_SWITCH);
            PORT_DIPSETTING(    0x00, "11" );
            PORT_DIPSETTING(    0x06, "15" );

            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_SERVICE );  PORT_NAME("Antenna"); NETLIST_LOGIC_PORT_CHANGED("maincpu", "antenna", (field, param, oldval, newval) => { ((netlist_mame_logic_input_device)pong_state.subdevice("maincpu:antenna")).input_changed(field, param, oldval, newval); });

            PORT_START("VR1");
            PORT_ADJUSTER( 50, "VR1 - 50k, Paddle 1 adjustment" );   NETLIST_ANALOG_PORT_CHANGED("maincpu", "vr0", (field, param, oldval, newval) => { ((netlist_mame_analog_input_device)pong_state.subdevice("maincpu:vr0")).input_changed(field, param, oldval, newval); });
            PORT_START("VR2");
            PORT_ADJUSTER( 50, "VR2 - 50k, Paddle 2 adjustment" );   NETLIST_ANALOG_PORT_CHANGED("maincpu", "vr1", (field, param, oldval, newval) => { ((netlist_mame_analog_input_device)pong_state.subdevice("maincpu:vr1")).input_changed(field, param, oldval, newval); });

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( pongd )
        //    PORT_START( "PADDLE0" ) /* fake input port for player 1 paddle */
        //    PORT_BIT( 0xff, 0x00, IPT_PADDLE ) PORT_SENSITIVITY(2) PORT_KEYDELTA(100) PORT_CENTERDELTA(0)   NETLIST_ANALOG_PORT_CHANGED("maincpu", "pot0")
        //
        //    PORT_START( "PADDLE1" ) /* fake input port for player 2 paddle */
        //    PORT_BIT( 0xff, 0x00, IPT_PADDLE ) PORT_SENSITIVITY(2) PORT_KEYDELTA(100) PORT_CENTERDELTA(0) PORT_PLAYER(2) NETLIST_ANALOG_PORT_CHANGED("maincpu", "pot1")
        //
        //    PORT_START( "PADDLE2" ) /* fake input port for player 3 paddle */
        //    PORT_BIT( 0xff, 0x00, IPT_PADDLE ) PORT_SENSITIVITY(2) PORT_KEYDELTA(100) PORT_CENTERDELTA(0) PORT_PLAYER(3) NETLIST_ANALOG_PORT_CHANGED("maincpu", "pot2")
        //
        //    PORT_START( "PADDLE3" ) /* fake input port for player 4 paddle */
        //    PORT_BIT( 0xff, 0x00, IPT_PADDLE ) PORT_SENSITIVITY(2) PORT_KEYDELTA(100) PORT_CENTERDELTA(0) PORT_PLAYER(4) NETLIST_ANALOG_PORT_CHANGED("maincpu", "pot3")
        //
        //    PORT_START("IN0") /* fake as well */
        //    PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_COIN1 )     NETLIST_LOGIC_PORT_CHANGED("maincpu", "coinsw")
        //    PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_START1 )     NETLIST_LOGIC_PORT_CHANGED("maincpu", "startsw")
        //
        //#if 0
        //    PORT_DIPNAME( 0x06, 0x00, "Game Won" )          PORT_DIPLOCATION("SW1A:1,SW1B:1") PORT_CHANGED_MEMBER(DEVICE_SELF, pong_state, input_changed, IC_SWITCH)
        //    PORT_DIPSETTING(    0x00, "11" )
        //    PORT_DIPSETTING(    0x06, "15" )
        //
        //    PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_SERVICE )  PORT_NAME("Antenna") NETLIST_LOGIC_PORT_CHANGED("maincpu", "antenna")
        //
        //    PORT_START("VR1")
        //    PORT_ADJUSTER( 50, "VR1 - 50k, Paddle 1 adjustment" )   NETLIST_ANALOG_PORT_CHANGED("maincpu", "vr0")
        //    PORT_START("VR2")
        //    PORT_ADJUSTER( 50, "VR2 - 50k, Paddle 2 adjustment" )   NETLIST_ANALOG_PORT_CHANGED("maincpu", "vr1")
        //#endif
        //INPUT_PORTS_END

        //static INPUT_PORTS_START( breakout )
        //
        //    PORT_START( "PADDLE0" ) /* fake input port for player 1 paddle */
        //    PORT_BIT( 0xff, 0x00, IPT_PADDLE ) PORT_SENSITIVITY(1) PORT_KEYDELTA(200) PORT_CENTERDELTA(0)   NETLIST_ANALOG_PORT_CHANGED("maincpu", "pot1")
        //
        //    PORT_START( "PADDLE1" ) /* fake input port for player 2 paddle */
        //    PORT_BIT( 0xff, 0x00, IPT_PADDLE ) PORT_SENSITIVITY(1) PORT_KEYDELTA(200) PORT_CENTERDELTA(0) PORT_PLAYER(2) NETLIST_ANALOG_PORT_CHANGED("maincpu", "pot2")
        //
        //    PORT_START("IN0") /* fake as well */
        //    PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_COIN1 )     NETLIST_LOGIC_PORT_CHANGED("maincpu", "coinsw1")
        //    PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_COIN2 )     NETLIST_LOGIC_PORT_CHANGED("maincpu", "coinsw2")
        //    PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_START1 )     NETLIST_LOGIC_PORT_CHANGED("maincpu", "startsw1")
        //    PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_START2 )     NETLIST_LOGIC_PORT_CHANGED("maincpu", "startsw2")
        //    PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_BUTTON1 )     NETLIST_LOGIC_PORT_CHANGED("maincpu", "servesw")
        //    PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_SERVICE )  PORT_NAME("Antenna") NETLIST_LOGIC_PORT_CHANGED("maincpu", "antenna")
        //
        //
        //    PORT_START("DIPS")
        //    PORT_DIPNAME( 0x01, 0x00, "Balls" )          PORT_DIPLOCATION("SW4:1") NETLIST_LOGIC_PORT_CHANGED("maincpu", "sw4")
        //    PORT_DIPSETTING(    0x00, "3" )
        //    PORT_DIPSETTING(    0x01, "5" )
        //    PORT_DIPNAME( 0x02, 0x00, DEF_STR( Coinage ) )      PORT_DIPLOCATION("SW3:1") NETLIST_LOGIC_PORT_CHANGED("maincpu", "sw3")
        //    PORT_DIPSETTING(    0x00, DEF_STR( 1C_1C ) )
        //    PORT_DIPSETTING(    0x02, DEF_STR( 1C_2C ) )
        //    PORT_DIPNAME( 0x04, 0x00, DEF_STR( Cabinet ) )      PORT_DIPLOCATION("SW2:1") NETLIST_LOGIC_PORT_CHANGED("maincpu", "sw2")
        //    PORT_DIPSETTING(    0x00, DEF_STR( Cocktail ) )
        //    PORT_DIPSETTING(    0x04, DEF_STR( Upright ) )
        //    PORT_DIPNAME( 0xf0, 0x00, DEF_STR( Free_Play ) )    PORT_DIPLOCATION("SW1:1,2,3,4") PORT_CHANGED_MEMBER(DEVICE_SELF, breakout_state, cb_free_play, 0)
        //    PORT_DIPSETTING(    0x00, "No Free Play" )
        //    PORT_DIPSETTING(    0x10, "100" )
        //    PORT_DIPSETTING(    0x20, "200" )
        //    PORT_DIPSETTING(    0x30, "300" )
        //    PORT_DIPSETTING(    0x40, "400" )
        //    PORT_DIPSETTING(    0x50, "500" )
        //    PORT_DIPSETTING(    0x60, "600" )
        //    PORT_DIPSETTING(    0x70, "700" )
        //    PORT_DIPSETTING(    0x80, "800" )
        //
        //INPUT_PORTS_END

        //static INPUT_PORTS_START( rebound )
        //// FIXME later
        //    PORT_START( "PADDLE0" ) /* fake input port for player 1 paddle */
        //    PORT_BIT( 0xff, 0x00, IPT_PADDLE ) PORT_SENSITIVITY(1) PORT_KEYDELTA(100) PORT_CENTERDELTA(0)   NETLIST_ANALOG_PORT_CHANGED("maincpu", "pot1")
        //
        //    PORT_START( "PADDLE1" ) /* fake input port for player 2 paddle */
        //    PORT_BIT( 0xff, 0x00, IPT_PADDLE ) PORT_SENSITIVITY(1) PORT_KEYDELTA(100) PORT_CENTERDELTA(0) PORT_PLAYER(2) NETLIST_ANALOG_PORT_CHANGED("maincpu", "pot2")
        //
        //    PORT_START("IN0") /* fake as well */
        //    PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_COIN1 )     NETLIST_LOGIC_PORT_CHANGED("maincpu", "coinsw")
        //    PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_START1 )     NETLIST_LOGIC_PORT_CHANGED("maincpu", "startsw")
        //    PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_SERVICE )  PORT_NAME("Antenna") NETLIST_LOGIC_PORT_CHANGED("maincpu", "antenna")
        //
        //    PORT_START("DIPS")
        //    PORT_DIPNAME( 0x03, 0x00, "Game Won" )          PORT_DIPLOCATION("SW1A:1,SW1A:2") PORT_CHANGED_MEMBER(DEVICE_SELF, rebound_state, input_changed, IC_SWITCH)
        //    PORT_DIPSETTING(    0x00, "11" )
        //    PORT_DIPSETTING(    0x03, "15" )
        //
        //    PORT_DIPNAME( 0x04, 0x00, DEF_STR( Coinage ) )      PORT_DIPLOCATION("SW1A:3") NETLIST_LOGIC_PORT_CHANGED("maincpu", "dsw2")
        //    PORT_DIPSETTING(    0x00, DEF_STR( 1C_1C ) )
        //    PORT_DIPSETTING(    0x04, DEF_STR( 1C_2C ) )
        //INPUT_PORTS_END
    }


    partial class pong_state : ttl_mono_state
    {
        public void pong(machine_config config)
        {
            /* basic machine hardware */
            NETLIST_CPU(config, "maincpu", (u32)netlist.config.DEFAULT_CLOCK()).set_source(this, netlist_pong);

            NETLIST_ANALOG_INPUT(config, "maincpu:vr0", "ic_b9_R.R").set_mult_offset(1.0 / 100.0 * RES_K(50), RES_K(56) );
            NETLIST_ANALOG_INPUT(config, "maincpu:vr1", "ic_a9_R.R").set_mult_offset(1.0 / 100.0 * RES_K(50), RES_K(56) );
            NETLIST_ANALOG_INPUT(config, "maincpu:pot0", "ic_b9_POT.DIAL");
            NETLIST_ANALOG_INPUT(config, "maincpu:pot1", "ic_a9_POT.DIAL");
            NETLIST_LOGIC_INPUT(config, "maincpu:sw1a", "sw1a.POS", 0);
            NETLIST_LOGIC_INPUT(config, "maincpu:sw1b", "sw1b.POS", 0);
            NETLIST_LOGIC_INPUT(config, "maincpu:coinsw", "coinsw.POS", 0);
            NETLIST_LOGIC_INPUT(config, "maincpu:antenna", "antenna.IN", 0);

            NETLIST_LOGIC_OUTPUT(config, "maincpu:snd0", 0).set_params("sound", sound_cb_logic);
            NETLIST_ANALOG_OUTPUT(config, "maincpu:vid0", 0).set_params("videomix", "fixfreq", (data, time) => { ((fixedfreq_device)subdevice("fixfreq")).update_composite_monochrome(data, time); });

            /* video hardware */
            SCREEN(config, "screen", SCREEN_TYPE_RASTER);
            //SCREEN(config, "screen", SCREEN_TYPE_VECTOR);
            FIXFREQ(config, m_video)
                .set_monitor_clock(MASTER_CLOCK_PONG)
                .set_horz_params(H_TOTAL_PONG-66,H_TOTAL_PONG-40,H_TOTAL_PONG-8,H_TOTAL_PONG)
                .set_vert_params(V_TOTAL_PONG-22,V_TOTAL_PONG-10,V_TOTAL_PONG-8,V_TOTAL_PONG)
                .set_fieldcount(1)
                .set_threshold(0.11)
                .set_gain(2.37)
                .set_horz_scale(4)
                .m_divideo.set_screen("screen");
            /* sound hardware */
            SPEAKER(config, "speaker").front_center();
            DAC_16BIT_R2R_TWOS_COMPLEMENT(config, m_dac, 0).add_route(ALL_OUTPUTS, "speaker", 0.5); // unknown DAC
        }
    }


        //void breakout_state::breakout(machine_config &config)
        //{
        //    /* basic machine hardware */
        //    NETLIST_CPU(config, "maincpu", netlist::config::DEFAULT_CLOCK()).set_source(NETLIST_NAME(breakout));
        //
        //    NETLIST_ANALOG_INPUT(config, "maincpu:pot1", "POTP1.DIAL");
        //    NETLIST_ANALOG_INPUT(config, "maincpu:pot2", "POTP2.DIAL");
        //    NETLIST_LOGIC_INPUT(config, "maincpu:coinsw1", "COIN1.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:coinsw2", "COIN2.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:startsw1", "START1.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:startsw2", "START2.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:servesw", "SERVE.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:sw4", "S4.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:sw3", "S3.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:sw2", "S2.POS", 0);
        //
        //    NETLIST_LOGIC_INPUT(config, "maincpu:sw1_1", "S1_1.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:sw1_2", "S1_2.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:sw1_3", "S1_3.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:sw1_4", "S1_4.POS", 0);
        //
        //    NETLIST_LOGIC_INPUT(config, "maincpu:antenna", "antenna.IN", 0);
        //
        //    NETLIST_ANALOG_OUTPUT(config, "maincpu:snd0", 0).set_params("sound", FUNC(breakout_state::sound_cb_analog));
        //    NETLIST_ANALOG_OUTPUT(config, "maincpu:vid0", 0).set_params("videomix", "fixfreq", FUNC(fixedfreq_device::update_composite_monochrome));
        //
        //    // Leds and lamps
        //
        //    NETLIST_ANALOG_OUTPUT(config, "maincpu:led_serve", 0).set_params("CON_P", FUNC(breakout_state::serve_cb));
        //    NETLIST_ANALOG_OUTPUT(config, "maincpu:lamp_credit1", 0).set_params("CON_CREDIT1", FUNC(breakout_state::credit1_cb));
        //    NETLIST_ANALOG_OUTPUT(config, "maincpu:lamp_credit2", 0).set_params("CON_CREDIT2", FUNC(breakout_state::credit2_cb));
        //    NETLIST_ANALOG_OUTPUT(config, "maincpu:coin_counter", 0).set_params("CON_T", FUNC(breakout_state::coin_counter_cb));
        //
        //    /* video hardware */
        //    /* The Pixel width is a 2,1,2,1,2,1,1,1 repeating pattern
        //     * Thus we must use double resolution horizontally
        //     */
        //    SCREEN(config, "screen", SCREEN_TYPE_RASTER);
        //    //SCREEN(config, "screen", SCREEN_TYPE_VECTOR);
        //    FIXFREQ(config, m_video)
        //        .set_monitor_clock(MASTER_CLOCK_BREAKOUT)
        //        .set_horz_params((H_TOTAL_BREAKOUT-176),(H_TOTAL_BREAKOUT-144),(H_TOTAL_BREAKOUT-80),  (H_TOTAL_BREAKOUT))
        //        .set_vert_params(V_TOTAL_BREAKOUT-24,V_TOTAL_BREAKOUT-24,V_TOTAL_BREAKOUT-20, V_TOTAL_BREAKOUT)
        //        .set_fieldcount(1)
        //        .set_threshold(1.0)
        //        .set_gain(2.66)
        //        .set_horz_scale(2)
        //        .set_screen("screen");
        //
        //    /* sound hardware */
        //    SPEAKER(config, "speaker").front_center();
        //    DAC_16BIT_R2R_TWOS_COMPLEMENT(config, m_dac, 0).add_route(ALL_OUTPUTS, "speaker", 0.5); // unknown DAC
        //}


        //void pong_state::pongf(machine_config &config)
        //{
        //    pong(config);
        //
        //    /* basic machine hardware */
        //
        //    subdevice<netlist_mame_device>("maincpu")->set_setup_func(NETLIST_NAME(pongf));
        //}

        //void pong_state::pongd(machine_config &config)
        //{
        //    /* basic machine hardware */
        //    NETLIST_CPU(config, "maincpu", netlist::config::DEFAULT_CLOCK()).set_source(NETLIST_NAME(pongdoubles));
        //
        //    NETLIST_ANALOG_INPUT(config, "maincpu:pot0", "A10_POT.DIAL");
        //    NETLIST_ANALOG_INPUT(config, "maincpu:pot1", "B10_POT.DIAL");
        //    NETLIST_ANALOG_INPUT(config, "maincpu:pot2", "B9B_POT.DIAL");
        //    NETLIST_ANALOG_INPUT(config, "maincpu:pot3", "B9A_POT.DIAL");
        //    NETLIST_LOGIC_INPUT(config, "maincpu:sw1a", "DIPSW1.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:sw1b", "DIPSW2.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:coinsw", "COIN_SW.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:startsw", "START_SW.POS", 0);
        //
        //#if 0
        //    NETLIST_LOGIC_INPUT(config, "maincpu:antenna", "antenna.IN", 0, 0x01)
        //#endif
        //
        //    NETLIST_ANALOG_OUTPUT(config, "maincpu:snd0", 0).set_params("AUDIO", FUNC(pong_state::sound_cb_analog));
        //    NETLIST_ANALOG_OUTPUT(config, "maincpu:vid0", 0).set_params("videomix", "fixfreq", FUNC(fixedfreq_device::update_composite_monochrome));
        //
        //    /* video hardware */
        //    SCREEN(config, "screen", SCREEN_TYPE_RASTER);
        //    FIXFREQ(config, m_video)
        //        .set_monitor_clock(MASTER_CLOCK_PONG)
        //        .set_horz_params(H_TOTAL_PONG-76,H_TOTAL_PONG-56,H_TOTAL_PONG-8,H_TOTAL_PONG)
        //        .set_vert_params(V_TOTAL_PONG-22,V_TOTAL_PONG-10,V_TOTAL_PONG-8,V_TOTAL_PONG)
        //        .set_fieldcount(1)
        //        .set_threshold(0.11)
        //        .set_gain(2.7)
        //        .set_horz_scale(2)
        //        .set_screen("screen");
        //
        //    /* sound hardware */
        //    SPEAKER(config, "speaker").front_center();
        //    DAC_16BIT_R2R_TWOS_COMPLEMENT(config, m_dac, 0).add_route(ALL_OUTPUTS, "speaker", 0.5); // unknown DAC
        //}

        //void rebound_state::rebound(machine_config &config)
        //{
        //    /* basic machine hardware */
        //    NETLIST_CPU(config, "maincpu", netlist::config::DEFAULT_CLOCK()).set_source(NETLIST_NAME(rebound));
        //
        //    // FIXME: Later
        //    NETLIST_ANALOG_INPUT(config, "maincpu:pot1", "POTP1.DIAL");
        //    NETLIST_ANALOG_INPUT(config, "maincpu:pot2", "POTP2.DIAL");
        //    NETLIST_LOGIC_INPUT(config, "maincpu:antenna", "antenna.IN", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:coinsw", "COIN1_SW.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:startsw", "START_SW.POS", 0);
        //
        //    NETLIST_LOGIC_INPUT(config, "maincpu:dsw1a", "DSW1a.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:dsw1b", "DSW1b.POS", 0);
        //    NETLIST_LOGIC_INPUT(config, "maincpu:dsw2", "DSW2.POS", 0);
        //
        //    NETLIST_ANALOG_OUTPUT(config, "maincpu:snd0", 0).set_params("sound", FUNC(rebound_state::sound_cb_analog));
        //    NETLIST_ANALOG_OUTPUT(config, "maincpu:vid0", 0).set_params("videomix", "fixfreq", FUNC(fixedfreq_device::update_composite_monochrome));
        //
        //    NETLIST_ANALOG_OUTPUT(config, "maincpu:led_credit", 0).set_params("CON11", FUNC(rebound_state::led_credit_cb));
        //    NETLIST_ANALOG_OUTPUT(config, "maincpu:coin_counter", 0).set_params("CON10", FUNC(rebound_state::coin_counter_cb));
        //
        //    /* video hardware */
        //    SCREEN(config, "screen", SCREEN_TYPE_RASTER);
        //    FIXFREQ(config, m_video)
        //        .set_monitor_clock(MASTER_CLOCK_PONG)
        //        .set_horz_params(H_TOTAL_PONG-66,H_TOTAL_PONG-56,H_TOTAL_PONG-8,H_TOTAL_PONG)
        //        .set_vert_params(V_TOTAL_PONG-22,V_TOTAL_PONG-12,V_TOTAL_PONG-8,V_TOTAL_PONG)
        //        .set_fieldcount(1)
        //        .set_threshold(1.0)
        //        .set_gain(1.8)
        //        .set_horz_scale(2)
        //        .set_screen("screen");
        //
        //    /* sound hardware */
        //    SPEAKER(config, "speaker").front_center();
        //    //FIXME: this is not related to reality at all.
        //    DAC_16BIT_R2R_TWOS_COMPLEMENT(config, m_dac, 0).add_route(ALL_OUTPUTS, "speaker", 0.5); // unknown DAC
        //}


    partial class pong : construct_ioport_helper
    {
        /***************************************************************************

          Game driver(s)

        ***************************************************************************/

        //ROM_START( pong ) /* dummy to satisfy game entry*/
        static readonly MemoryContainer<tiny_rom_entry> rom_pong = new MemoryContainer<tiny_rom_entry>()
        {
            ROM_REGION( 0x10000, "maincpu", 0 ), /* enough for netlist */
            ROM_LOAD( "pong.netlist", 0x000000, 18273, CRC("d249ce49") + SHA1("e1d2cfca74b75f0520965639e6947a351650fc3e") ),

            ROM_END,
        };


        //ROM_START( breakout )
        //    ROM_REGION( 0x10000, "maincpu", ROMREGION_ERASE00 )
        //ROM_END

        //ROM_START( pongf ) /* dummy to satisfy game entry*/
        //    ROM_REGION( 0x10000, "maincpu", ROMREGION_ERASE00 )
        //ROM_END

        //ROM_START( pongd ) /* dummy to satisfy game entry*/
        //    ROM_REGION( 0x10000, "maincpu", ROMREGION_ERASE00 )
        //ROM_END

        //ROM_START( rebound ) /* dummy to satisfy game entry*/
        //    ROM_REGION( 0x10000, "maincpu", ROMREGION_ERASE00 )
        //ROM_END


        /*   // 100% TTL - NO ROMS

        ROM_START( pongbarl ) // dummy to satisfy game entry
            ROM_REGION( 0x10000, "maincpu", ROMREGION_ERASE00 )
        ROM_END

        ROM_START( coupedav ) // dummy to satisfy game entry
            ROM_REGION( 0x10000, "maincpu", ROMREGION_ERASE00 )
        ROM_END

        ROM_START( cktpong ) // dummy to satisfy game entry
            ROM_REGION( 0x10000, "maincpu", ROMREGION_ERASE00 )
        ROM_END

        ROM_START( drpong ) // dummy to satisfy game entry
            ROM_REGION( 0x10000, "maincpu", ROMREGION_ERASE00 )
        ROM_END

        ROM_START( pupppong ) // dummy to satisfy game entry
            ROM_REGION( 0x10000, "maincpu", ROMREGION_ERASE00 )
        ROM_END

        ROM_START( snoopong ) // dummy to satisfy game entry
            ROM_REGION( 0x10000, "maincpu", ROMREGION_ERASE00 )
        ROM_END

        ROM_START( suprpong ) // dummy to satisfy game entry
            ROM_REGION( 0x10000, "maincpu", ROMREGION_ERASE00 )
        ROM_END

        ROM_START( breakckt ) // dummy to satisfy game entry
            ROM_REGION( 0x10000, "maincpu", ROMREGION_ERASE00 )
        ROM_END

        ROM_START( consolet ) // dummy to satisfy game entry
            ROM_REGION( 0x10000, "maincpu", ROMREGION_ERASE00 )
        ROM_END
        */


        static void pong_state_pong(machine_config config, device_t device) { pong_state pong_state = (pong_state)device; pong_state.pong(config); }

        static pong m_pong = new pong();

        static device_t device_creator_pong(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new pong_state(mconfig, (device_type)type, tag); }


        public static readonly game_driver driver_pong = GAME(device_creator_pong, rom_pong, "1972", "pong", "0", pong_state_pong, m_pong.construct_ioport_pong, driver_device.empty_init, ROT0, "Atari", "Pong (Rev E) external [TTL]", MACHINE_SUPPORTS_SAVE);
        //GAME(  1972, pongf,    pong, pongf,    pong,      pong_state,     empty_init, ROT0,  "Atari", "Pong (Rev E) [TTL]", MACHINE_SUPPORTS_SAVE)
        //GAME(  1973, pongd,     0, pongd,    pongd,     pong_state,     empty_init, ROT0,  "Atari", "Pong Doubles [TTL]", MACHINE_SUPPORTS_SAVE)
        //GAMEL( 1974, rebound,   0, rebound,  rebound,   rebound_state,  empty_init, ROT0,  "Atari", "Rebound (Rev B) [TTL]", MACHINE_SUPPORTS_SAVE, layout_rebound)
        //GAMEL( 1976, breakout,  0, breakout, breakout,  breakout_state, empty_init, ROT90, "Atari", "Breakout [TTL]", MACHINE_SUPPORTS_SAVE, layout_breakout)

        // 100% TTL
        //GAMEL(1974, spike,      rebound,  rebound,  rebound,  rebound_state,  empty_init, ROT0,  "Atari/Kee", "Spike [TTL]", MACHINE_IS_SKELETON)
        //GAMEL(1974, volleyball, rebound,  rebound,  rebound,  rebound_state,  empty_init, ROT0,  "Atari", "Volleyball [TTL]", MACHINE_IS_SKELETON)
        //GAME( 1973, coupedav,   pongd,    pongd,    pongd,    pong_state,     empty_init, ROT0,  "Atari France", "Coupe Davis [TTL]", MACHINE_SUPPORTS_SAVE)
        //GAME( 1973, pongbarl,   pong,     pong,     pong,     pong_state,     empty_init, ROT0,  "Atari", "Pong In-A-Barrel [TTL]", MACHINE_SUPPORTS_SAVE)
        //GAME( 1974, cktpong,    pong,     pong,     pong,     pong_state,     empty_init, ROT0,  "Atari / National Entertainment Co.", "Cocktail Pong [TTL]", MACHINE_SUPPORTS_SAVE)
        //GAME( 1974, drpong,     pong,     pong,     pong,     pong_state,     empty_init, ROT0,  "Atari", "Dr. Pong [TTL]", MACHINE_SUPPORTS_SAVE)
        //GAME( 1974, pupppong,   pong,     pong,     pong,     pong_state,     empty_init, ROT0,  "Atari", "Puppy Pong [TTL]", MACHINE_SUPPORTS_SAVE)
        //GAME( 1974, snoopong,   pong,     pong,     pong,     pong_state,     empty_init, ROT0,  "Atari", "Snoopy Pong [TTL]", MACHINE_SUPPORTS_SAVE)
        //GAME( 1974, suprpong,   0,        suprpong, pong,     pong_state,     empty_init, ROT0,  "Atari", "Superpong [TTL]", MACHINE_SUPPORTS_SAVE)
        //GAMEL( 1976, breakckt,  breakout, breakout, breakout, breakout_state, empty_init, ROT90, "Atari", "Breakout Cocktail [TTL]", MACHINE_SUPPORTS_SAVE, layout_breakckt)
        //GAMEL( 1976, consolet,  breakout, breakout, breakout, breakout_state, empty_init, ROT90, "Atari Europe", "Consolette [TTL]", MACHINE_SUPPORTS_SAVE, layout_consolet)
    }
}
