// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int16_t = System.Int16;


namespace mame
{
    abstract class sound_module
    {
        public const string OSD_SOUND_PROVIDER = "sound";


        public int m_sample_rate;
        public int m_audio_latency;


        sound_module()
        {
            m_sample_rate = 0;
            m_audio_latency = 1;
        }

        //~sound_module() { }


        public abstract void update_audio_stream(bool is_throttled, Pointer<int16_t> buffer, int samples_this_frame);  //virtual void update_audio_stream(bool is_throttled, const int16_t *buffer, int samples_this_frame) = 0;
        public abstract void set_mastervolume(int attenuation);


        int sample_rate() { return m_sample_rate; }
    }
}
