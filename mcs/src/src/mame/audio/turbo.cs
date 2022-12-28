// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint8_t = System.Byte;

using static mame.samples_global;
using static mame.speaker_global;


namespace mame
{
    //#define DISCRETE_TEST (0)


    partial class turbo_state : turbo_base_state
    {
        /*************************************
         *  Turbo shared state updates
         *************************************/
        //void turbo_state::update_samples()
        //{
        //    // accelerator sounds
        //    // BSEL == 3 --> off
        //    // BSEL == 2 --> standard
        //    // BSEL == 1 --> tunnel
        //    // BSEL == 0 --> ???
        //    if (m_bsel == 3 && m_samples->playing(5))
        //        m_samples->stop(5);
        //    else if (m_bsel != 3 && !m_samples->playing(5))
        //        m_samples->start(5, 7, true);
        //    if (m_samples->playing(5))
        //        m_samples->set_frequency(5, m_samples->base_frequency(5) * ((m_accel & 0x3f) / 5.25 + 1));
        //}


        //TIMER_CALLBACK_MEMBER(turbo_state::update_sound_a)


        /*************************************
         *  Turbo PPI write handlers
         *************************************/
        void sound_a_w(uint8_t data)
        {
            throw new emu_unimplemented();
        }


        void sound_b_w(uint8_t data)
        {
            throw new emu_unimplemented();
        }


        void sound_c_w(uint8_t data)
        {
            throw new emu_unimplemented();
        }


        /*************************************
         *  Turbo sound hardware description
         *************************************/
        //static const char *const turbo_sample_names[] =


        void turbo_samples(machine_config config)
        {
            throw new emu_unimplemented();
        }


        //void subroc3d_state::sound_a_w(uint8_t data)

        //inline void subroc3d_state::update_volume(int leftchan, uint8_t dis, uint8_t dir)

        //void subroc3d_state::sound_b_w(uint8_t data)

        //void subroc3d_state::sound_c_w(uint8_t data)

        //static const char *const subroc3d_sample_names[] =

        //void subroc3d_state::subroc3d_samples(machine_config &config)

        //void buckrog_state::update_samples()

        //void buckrog_state::sound_a_w(uint8_t data)

        //void buckrog_state::sound_b_w(uint8_t data)

        //static const char *const buckrog_sample_names[]=

        //void buckrog_state::buckrog_samples(machine_config &config)



//OLD 


#if false
        /*************************************
         *
         *  Turbo shared state updates
         *
         *************************************/

        void turbo_state::update_samples()
        {
            // accelerator sounds
            // BSEL == 3 --> off
            // BSEL == 2 --> standard
            // BSEL == 1 --> tunnel
            // BSEL == 0 --> ???
            if (m_bsel == 3 && m_samples->playing(5))
                m_samples->stop(5);
            else if (m_bsel != 3 && !m_samples->playing(5))
                m_samples->start(5, 7, true);
            if (m_samples->playing(5))
                m_samples->set_frequency(5, m_samples->base_frequency(5) * ((m_accel & 0x3f) / 5.25 + 1));
        }


#if DISCRETE_TEST

        TIMER_CALLBACK_MEMBER(turbo_state::update_sound_a)
        {
            discrete_device *discrete = machine.device<discrete_device>("discrete");
            int data = param;

            // missing short crash sample, but I've never seen it triggered
            discrete->write(0, !(data & 0x01));
            discrete->write(1, (data >> 1) & 1);
            discrete->write(2, (data >> 2) & 1);
            discrete->write(3, (data >> 3) & 1);
            discrete->write(4, (data >> 4) & 1);
            discrete->write(5, !(data & 0x20));
            discrete->write(6, !(data & 0x40));

        if (!((data >> 1) & 1)) osd_printf_debug("/TRIG1\n");
        if (!((data >> 2) & 1)) osd_printf_debug("/TRIG2\n");
        if (!((data >> 3) & 1)) osd_printf_debug("/TRIG3\n");
        if (!((data >> 4) & 1)) osd_printf_debug("/TRIG4\n");

        //  osel = (osel & 6) | ((data >> 5) & 1);
        //  update_samples(samples);
        }
#else
        TIMER_CALLBACK_MEMBER(turbo_state::update_sound_a)
        {
        }
#endif


        /*************************************
         *  Turbo PPI write handlers
         *************************************/
        void sound_a_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
#if !DISCRETE_TEST
#endif
#if !DISCRETE_TEST
            uint8_t diff = data ^ m_sound_state[0];
#endif
            m_sound_state[0] = data;

#if !DISCRETE_TEST

            // /CRASH.S: channel 0
            if ((diff & 0x01) && !(data & 0x01)) m_samples->start(0, 5);

            // /TRIG1: channel 1
            if ((diff & 0x02) && !(data & 0x02)) m_samples->start(1, 0);

            // /TRIG2: channel 1
            if ((diff & 0x04) && !(data & 0x04)) m_samples->start(1, 1);

            // /TRIG3: channel 1
            if ((diff & 0x08) && !(data & 0x08)) m_samples->start(1, 2);

            // /TRIG4: channel 1
            if ((diff & 0x10) && !(data & 0x10)) m_samples->start(1, 3);

            // OSEL0
            m_osel = (m_osel & 6) | ((data >> 5) & 1);

            // /SLIP: channel 2
            if ((diff & 0x40) && !(data & 0x40)) m_samples->start(2, 4);

            // /CRASH.L: channel 3
            if ((diff & 0x80) && !(data & 0x80)) m_samples->start(3, 5);

            // update any samples
            update_samples();

#else

            if (((data ^ m_last_sound_a) & 0x1e) && (m_last_sound_a & 0x1e) != 0x1e)
                machine().scheduler().timer_set(attotime::from_hz(20000), FUNC(update_sound_a), data);
            else
                update_sound_a(data);

            m_last_sound_a = data;

#endif
#endif
        }


        void sound_b_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            uint8_t diff = data ^ m_sound_state[1];
            m_sound_state[1] = data;

            // ACC0-ACC5
            m_accel = data & 0x3f;
            m_tachometer = m_accel;

            // /AMBU: channel 4
            if ((diff & 0x40) && !(data & 0x40) && !m_samples->playing(4)) m_samples->start(4, 8, true);
            if ((diff & 0x40) &&  (data & 0x40)) m_samples->stop(4);

            // /SPIN: channel 2
            if ((diff & 0x80) && !(data & 0x80)) m_samples->start(2, 6);

            // update any samples
            update_samples();
#endif
        }


        void sound_c_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            // OSEL1-2
            m_osel = (m_osel & 1) | ((data & 3) << 1);

            // BSEL0-1
            m_bsel = (data >> 2) & 3;

            // SPEED0-3
            m_speed = (data >> 4) & 0x0f;

            // update any samples
            update_samples();
#endif
        }


        /*************************************
         *  Turbo sound hardware description
         *************************************/
        static readonly string [] turbo_sample_names =
        {
            "*turbo",
            "01",       // 0: Trig1
            "02",       // 1: Trig2
            "03",       // 2: Trig3
            "04",       // 3: Trig4
            "05",       // 4: Screech
            "06",       // 5: Crash
            "skidding", // 6: Spin
            "idle",     // 7: Idle
            "ambulanc", // 8: Ambulance
            null
        };


        void turbo_samples(machine_config config)
        {
            // this is the cockpit speaker configuration
            SPEAKER(config, "fspeaker", 0.0, 0.0, 1.0);     // front
            SPEAKER(config, "bspeaker",  0.0, 0.0, -0.5);   // back
            SPEAKER(config, "lspeaker", -0.2, 0.0, 1.0);    // left
            SPEAKER(config, "rspeaker", 0.2, 0.0, 1.0);     // right

            SAMPLES(config, m_samples);
            m_samples.op0.set_channels(10);
            m_samples.op0.set_samples_names(turbo_sample_names);

            // channel 0 = CRASH.S -> CRASH.S/SM
            m_samples.op0.disound.add_route(0, "fspeaker", 0.25);

            // channel 1 = TRIG1-4 -> ALARM.M/F/R/L
            m_samples.op0.disound.add_route(1, "fspeaker", 0.25);
            m_samples.op0.disound.add_route(1, "rspeaker", 0.25);
            m_samples.op0.disound.add_route(1, "lspeaker",  0.25);

            // channel 2 = SLIP/SPIN -> SKID.F/R/L/M
            m_samples.op0.disound.add_route(2, "fspeaker", 0.25);
            m_samples.op0.disound.add_route(2, "rspeaker", 0.25);
            m_samples.op0.disound.add_route(2, "lspeaker",  0.25);

            // channel 3 = CRASH.L -> CRASH.L/LM
            m_samples.op0.disound.add_route(3, "bspeaker",  0.25);

            // channel 4 = AMBU -> AMBULANCE/AMBULANCE.M
            m_samples.op0.disound.add_route(4, "fspeaker", 0.25);

            // channel 5 = ACCEL+BSEL -> MYCAR.F/W/M + MYCAR0.F/M + MYCAR1.F/M
            m_samples.op0.disound.add_route(5, "fspeaker", 0.25);
            m_samples.op0.disound.add_route(5, "bspeaker",  0.25);

            // channel 6 = OSEL -> OCAR.F/FM
            m_samples.op0.disound.add_route(6, "fspeaker", 0.25);

            // channel 7 = OSEL -> OCAR.L/LM
            m_samples.op0.disound.add_route(7, "lspeaker",  0.25);

            // channel 8 = OSEL -> OCAR.R/RM
            m_samples.op0.disound.add_route(8, "rspeaker", 0.25);

            // channel 9 = OSEL -> OCAR.W/WM
            m_samples.op0.disound.add_route(9, "bspeaker",  0.25);
        }
#endif
    }


    /*
        Cockpit: CN2 1+2 -> FRONT
                 CN2 3+4 -> REAR
                 CN2 5+6 -> RIGHT
                 CN2 7+8 -> LEFT

        Upright: CN2 1+2 -> UPPER
                 CN2 3+4 -> LOWER

        F.OUT = CRASH.S +
                ALARM.F +
                SKID.F +
                OCAR.F +
                MYCAR.F +
                MYCAR0.F +
                MYCAR1.F +
                AMBULACE

        W.OUT = CRASH.L +
                OCAR.W +
                MYCAR.W +
                MYCAR0.W +
                MYCAR1.W +
                SLF

        R.OUT = ALARM.R +
                SKID.R +
                OCAR.R

        L.OUT = ALARM.L +
                SKID.L +
                OCAR.L

        M.OUT = CRASH.SM +
                CRASH.LM +
                SKID.M +
                ALARM.M +
                AMBULACE.M +
                MYCAR.M +
                MYCAR0.M +
                MYCAR1.M +
                OCAR.FM +
                OCAR.LM +
                OCAR.RM +
                OCAR.WM
    */


    //void subroc3d_state::sound_a_w(uint8_t data)

    //inline void subroc3d_state::update_volume(int leftchan, uint8_t dis, uint8_t dir)

    //void subroc3d_state::sound_b_w(uint8_t data)

    //void subroc3d_state::sound_c_w(uint8_t data)

    //static const char *const subroc3d_sample_names[] =

    //void subroc3d_state::subroc3d_samples(machine_config &config)

    //void buckrog_state::update_samples()

    //void buckrog_state::sound_a_w(uint8_t data)

    //void buckrog_state::sound_b_w(uint8_t data)

    //static const char *const buckrog_sample_names[]=

    //void buckrog_state::buckrog_samples(machine_config &config)


    /*************************************
     *  Discrete test code
     *************************************/
#if DISCRETE_TEST

    // Nodes - Inputs
    //#define TURBO_CRASH_EN          NODE_01
    //#define TURBO_TRIG1_INV         NODE_02
    //#define TURBO_TRIG2_INV         NODE_03
    //#define TURBO_TRIG3_INV         NODE_04
    //#define TURBO_TRIG4_INV         NODE_05
    //#define TURBO_SLIP_EN           NODE_06
    //#define TURBO_CRASHL_EN         NODE_07
    //#define TURBO_ACC_VAL           NODE_08
    //#define TURBO_AMBU_EN           NODE_09
    //#define TURBO_SPIN_EN           NODE_10
    //#define TURBO_OSEL_VAL          NODE_11
    //#define TURBO_BSEL_VAL          NODE_12

    // Nodes - Sounds
    //#define FIRETRUCK_NOISE         NODE_20

    static const discrete_555_desc turbo_alarm_555 =
    {
        DISC_555_OUT_SQW | DISC_555_OUT_DC,
        5,              // B+ voltage of 555
        DEFAULT_555_VALUES,
    };

    DISCRETE_SOUND_START(turbo_discrete)
        /************************************************/
        /* Input register mapping for turbo             */
        /************************************************/
        //                  NODE             ADDR  MASK    GAIN    OFFSET  INIT
        DISCRETE_INPUT(TURBO_CRASH_EN       ,0x00,0x001f,                  0.0)
        DISCRETE_INPUT(TURBO_TRIG1_INV      ,0x01,0x001f,                  1.0)
        DISCRETE_INPUT(TURBO_TRIG2_INV      ,0x02,0x001f,                  1.0)
        DISCRETE_INPUT(TURBO_TRIG3_INV      ,0x03,0x001f,                  1.0)
        DISCRETE_INPUT(TURBO_TRIG4_INV      ,0x04,0x001f,                  1.0)
        DISCRETE_INPUT(TURBO_SLIP_EN        ,0x05,0x001f,                  0.0)
        DISCRETE_INPUT(TURBO_CRASHL_EN      ,0x06,0x001f,                  0.0)
        DISCRETE_INPUT(TURBO_ACC_VAL        ,0x07,0x001f,                  0.0)
        DISCRETE_INPUT(TURBO_AMBU_EN        ,0x08,0x001f,                  0.0)
        DISCRETE_INPUT(TURBO_SPIN_EN        ,0x09,0x001f,                  0.0)
        DISCRETE_INPUT(TURBO_OSEL_VAL       ,0x0a,0x001f,                  0.0)
        DISCRETE_INPUT(TURBO_BSEL_VAL       ,0x0b,0x001f,                  0.0)

        /************************************************/
        /* Alarm sounds                                 */
        /************************************************/

        // 5-5-5 counter provides the input clock
        DISCRETE_555_ASTABLE(NODE_50,1,470,120,0.1e-6,&turbo_alarm_555)
        // which clocks a 74393 dual 4-bit counter, clocked on the falling edge
        DISCRETE_COUNTER(NODE_51,1,0,NODE_50,0,15,1,0,DISC_CLK_ON_F_EDGE)
        // the high bit of this counter
        DISCRETE_TRANSFORM2(NODE_52,NODE_51,8,"01/")
        // clocks the other half of the 74393
        DISCRETE_COUNTER(NODE_53,1,0,NODE_52,0,15,1,0,DISC_CLK_ON_F_EDGE)

        // trig1 triggers a LS123 retriggerable multivibrator
        DISCRETE_ONESHOT(NODE_60,TURBO_TRIG1_INV,5.0,(0.33e-9)*47*1e6, DISC_ONESHOT_FEDGE|DISC_ONESHOT_RETRIG|DISC_OUT_ACTIVE_HIGH)
        // which interacts with bit 0 of the second counter
        DISCRETE_TRANSFORM2(NODE_61,NODE_53,1,"01&")
        // via a NAND
        DISCRETE_LOGIC_NAND(NODE_62,1,NODE_60,NODE_61)

        // trig2 triggers a LS123 retriggerable multivibrator
        DISCRETE_ONESHOT(NODE_65,TURBO_TRIG2_INV,5.0,(0.33e-9)*47*10e6,DISC_ONESHOT_FEDGE|DISC_ONESHOT_RETRIG|DISC_OUT_ACTIVE_HIGH)
        // which interacts with bit 3 of the first counter via a NAND
        DISCRETE_LOGIC_NAND(NODE_66,1,NODE_65,NODE_52)

        // trig3 triggers a LS123 retriggerable multivibrator
        DISCRETE_ONESHOT(NODE_70,TURBO_TRIG3_INV,5.0,(0.33e-9)*47*33e6,DISC_ONESHOT_FEDGE|DISC_ONESHOT_RETRIG|DISC_OUT_ACTIVE_HIGH)
        // which interacts with bit 2 of the first counter
        DISCRETE_TRANSFORM3(NODE_71,NODE_51,4,1,"01/2&")
        // via a NAND
        DISCRETE_LOGIC_NAND(NODE_72,1,NODE_70,NODE_71)

        // trig4 triggers a LS123 retriggerable multivibrator
        DISCRETE_ONESHOT(NODE_75,TURBO_TRIG4_INV,5.0,(0.33e-9)*47*10e6,DISC_ONESHOT_FEDGE|DISC_ONESHOT_RETRIG|DISC_OUT_ACTIVE_HIGH)
        // which interacts with bit 1 of the first counter
        DISCRETE_TRANSFORM3(NODE_76,NODE_51,2,1,"01/2&")
        // via a NAND
        DISCRETE_LOGIC_NAND(NODE_77,1,NODE_75,NODE_76)

        // everything is effectively NANDed together
        DISCRETE_LOGIC_NAND4(NODE_80,1,NODE_62,NODE_66,NODE_72,NODE_77)

    /*

        the rest of the circuit looks like this:

                          +5V            +12V                                +---+
                           ^              ^   +--------+               1K    v   |
                           |              |   | |\     |           +---NNN--NNN--+
                           Z 1K       10K Z   | | \    |           | |\     20K  |   +--|(----> ALARM_M
                           Z              Z   +-|- \   |           | | \         |   |  4.7u
                           |              |     |   >--+---NNNN----+-|- \        |   +--|(----> ALARM_F
                           +--NNNN--|(----+-----|+ /        22K      |   >-------+---+  4.7u
        +-\                |  5.1K  4.7u  |     | /             +6V--|+ /            +--|(----> ALARM_R
        |  >o---(NODE_62)--+              Z     |/                   | /             |  4.7u
        +-/                |          10K Z                          |/              +--|(----> ALARM_L
                           |              |                                             4.7u
        +-\                |              v
        |  >o---(NODE_66)--+             GND
        +-/                |
                           |
        +-\                |
        |  >o---(NODE_72)--+
        +-/                |
                           |
        +-\                |
        |  >o---(NODE_77)--+
        +-/


    */

        /************************************************/
        /* Combine all 7 sound sources.                 */
        /************************************************/

        DISCRETE_OUTPUT(NODE_80, 16000)
    DISCRETE_SOUND_END

#endif
}
