// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint8_t = System.Byte;

using static mame.disound_global;
using static mame.samples_global;


namespace mame
{
    partial class zaxxon_state : driver_device
    {
        /*************************************
         *  Zaxxon sound hardware description
         *************************************/
        static readonly string [] zaxxon_sample_names =
        {
            "*zaxxon",
            "03",   /* 0 - Homing Missile */
            "02",   /* 1 - Base Missile */
            "01",   /* 2 - Laser (force field) */
            "00",   /* 3 - Battleship (end of level boss) */
            "11",   /* 4 - S-Exp (enemy explosion) */
            "10",   /* 5 - M-Exp (ship explosion) */
            "08",   /* 6 - Cannon (ship fire) */
            "23",   /* 7 - Shot (enemy fire) */
            "21",   /* 8 - Alarm 2 (target lock) */
            "20",   /* 9 - Alarm 3 (low fuel) */
            "05",   /* 10 - initial background noise */
            "04",   /* 11 - looped asteroid noise */
            null
        };


        void zaxxon_samples(machine_config config)
        {
            SAMPLES(config, m_samples);
            m_samples.op0.set_channels(12);
            m_samples.op0.set_samples_names(zaxxon_sample_names);
            m_samples.op0.disound.add_route(ALL_OUTPUTS, "speaker", 0.25);
        }


        /*************************************
         *  Zaxxon PPI write handlers
         *************************************/
        void zaxxon_sound_a_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            uint8_t diff = data ^ m_sound_state[0];
            m_sound_state[0] = data;

            /* PLAYER SHIP A/B: volume */
            m_samples->set_volume(10, 0.5 + 0.157 * (data & 0x03));
            m_samples->set_volume(11, 0.5 + 0.157 * (data & 0x03));

            /* PLAYER SHIP C: channel 10 */
            if ((diff & 0x04) && !(data & 0x04)) m_samples->start(10, 10, true);
            if ((diff & 0x04) &&  (data & 0x04)) m_samples->stop(10);

            /* PLAYER SHIP D: channel 11 */
            if ((diff & 0x08) && !(data & 0x08)) m_samples->start(11, 11, true);
            if ((diff & 0x08) &&  (data & 0x08)) m_samples->stop(11);

            /* HOMING MISSILE: channel 0 */
            if ((diff & 0x10) && !(data & 0x10)) m_samples->start(0, 0, true);
            if ((diff & 0x10) &&  (data & 0x10)) m_samples->stop(0);

            /* BASE MISSILE: channel 1 */
            if ((diff & 0x20) && !(data & 0x20)) m_samples->start(1, 1);

            /* LASER: channel 2 */
            if ((diff & 0x40) && !(data & 0x40)) m_samples->start(2, 2, true);
            if ((diff & 0x40) &&  (data & 0x40)) m_samples->stop(2);

            /* BATTLESHIP: channel 3 */
            if ((diff & 0x80) && !(data & 0x80)) m_samples->start(3, 3, true);
            if ((diff & 0x80) &&  (data & 0x80)) m_samples->stop(3);
#endif
        }


        void zaxxon_sound_b_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            uint8_t diff = data ^ m_sound_state[1];
            m_sound_state[1] = data;

            /* S-EXP: channel 4 */
            if ((diff & 0x10) && !(data & 0x10)) m_samples->start(4, 4);

            /* M-EXP: channel 5 */
            if ((diff & 0x20) && !(data & 0x20) && !m_samples->playing(5)) m_samples->start(5, 5);

            /* CANNON: channel 6 */
            if ((diff & 0x80) && !(data & 0x80)) m_samples->start(6, 6);
#endif
        }


        void zaxxon_sound_c_w(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            uint8_t diff = data ^ m_sound_state[2];
            m_sound_state[2] = data;

            /* SHOT: channel 7 */
            if ((diff & 0x01) && !(data & 0x01)) m_samples->start(7, 7);

            /* ALARM2: channel 8 */
            if ((diff & 0x04) && !(data & 0x04)) m_samples->start(8, 8);

            /* ALARM3: channel 9 */
            if ((diff & 0x08) && !(data & 0x08) && !m_samples->playing(9)) m_samples->start(9, 9);
#endif
        }


        //static const char *const congo_sample_names[] =

        //void zaxxon_state::congo_samples(machine_config &config)

        //void zaxxon_state::congo_sound_b_w(uint8_t data)

        //void zaxxon_state::congo_sound_c_w(uint8_t data)
    }
}
