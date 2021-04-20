// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using s32 = System.Int32;
using stream_sample_t = System.Int32;
using u32 = System.UInt32;


namespace mame
{
    // ======================> speaker_device
    public class speaker_device : device_t,
                                  IDisposable
                                  // public device_mixer_interface
    {
        //DEFINE_DEVICE_TYPE(SPEAKER, speaker_device, "speaker", "Speaker")
        static device_t device_creator_speaker_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new speaker_device(mconfig, tag, owner, clock); }
        public static readonly device_type SPEAKER = DEFINE_DEVICE_TYPE(device_creator_speaker_device, "speaker", "Speaker");


        device_mixer_interface m_dimixer;

        // inline configuration state
        double m_x;
        double m_y;
        double m_z;

        // internal state
#if MAME_DEBUG
        int m_max_sample;           // largest sample value we've seen
        int m_clipped_samples;      // total number of clipped samples
        int m_total_samples;        // total number of samples
#endif


        // construction/destruction
        //-------------------------------------------------
        //  speaker_device - constructor
        //-------------------------------------------------
        speaker_device(machine_config mconfig, string tag, device_t owner, double x, double y, double z)
            : this(mconfig, tag, owner, 0)
        {
            set_position(x, y, z);
        }

        public speaker_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : base(mconfig, SPEAKER, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_mixer_interface(mconfig, this));

            m_dimixer = GetClassInterface<device_mixer_interface>();


            m_x = 0;
            m_y = 0;
            m_z = 0;

#if MAME_DEBUG
            m_max_sample(0)
            m_clipped_samples(0)
            m_total_samples(0)
#endif
        }

        ~speaker_device()
        {
            //throw new emu_unimplemented();
#if false
            global.assert(m_isDisposed);  // can remove
#endif
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
#if MAME_DEBUG
            // log the maximum sample values for all speakers
            if (m_max_sample > 0)
                osd_printf_global.osd_printf_debug("Speaker \"{0}\" - max = {1} (gain *= {2}) - {3}%% samples clipped\n", tag(), m_max_sample, 32767.0 / (m_max_sample != 0 ? m_max_sample : 1), (int)((double)m_clipped_samples * 100.0 / m_total_samples));
#endif

            m_isDisposed = true;
        }


        // inline configuration helpers

        speaker_device set_position(double x, double y, double z) { m_x = x; m_y = y; m_z = z; return this; }
        public speaker_device front_center()  { set_position( 0.0,  0.0,  1.0); return this; }
        public speaker_device front_left()    { set_position(-0.2,  0.0,  1.0); return this; }
        //speaker_device &front_floor()       { set_position( 0.0, -0.5,  1.0); return *this; }
        public speaker_device front_right()   { set_position( 0.2,  0.0,  1.0); return this; }
        //speaker_device &rear_center()       { set_position( 0.0,  0.0, -0.5); return *this; }
        //speaker_device &rear_left()         { set_position(-0.2,  0.0, -0.5); return *this; }
        //speaker_device &rear_right()        { set_position( 0.2,  0.0, -0.5); return *this; }
        //speaker_device &headrest_center()   { set_position( 0.0,  0.0, -0.1); return *this; }
        //speaker_device &headrest_left()     { set_position(-0.1,  0.0, -0.1); return *this; }
        //speaker_device &headrest_right()    { set_position( 0.1,  0.0, -0.1); return *this; }
        //speaker_device &seat()              { set_position( 0.0, -0.5,  0.0); return *this; }
        //speaker_device &backrest()          { set_position( 0.0, -0.2,  0.1); return *this; }


        // internally for use by the sound system
        //-------------------------------------------------
        //  mix - mix in samples from the speaker's stream
        //-------------------------------------------------
        public void mix(MemoryContainer<s32> leftmix, MemoryContainer<s32> rightmix, ref int samples_this_update, bool suppress)  //s32 *leftmix, s32 *rightmix, int &samples_this_update, bool suppress)
        {
            // skip if no stream
            if (m_dimixer.mixer_stream() == null)
                return;

            // update the stream, getting the start/end pointers around the operation
            int numsamples;
            Pointer<stream_sample_t> stream_buf = m_dimixer.mixer_stream().output_since_last_update(0, out numsamples);

            // set or assert that all streams have the same count
            if (samples_this_update == 0)
            {
                samples_this_update = numsamples;

                // reset the mixing streams
                std.fill_n(leftmix, samples_this_update, 0);
                std.fill_n(rightmix, samples_this_update, 0);
            }

            assert(samples_this_update == numsamples);

#if MAME_DEBUG
            // debug version: keep track of the maximum sample
            for (int sample = 0; sample < samples_this_update; sample++)
            {
                if (stream_buf[sample] > m_max_sample)
                    m_max_sample = stream_buf[sample];
                else if (-stream_buf[sample] > m_max_sample)
                    m_max_sample = -stream_buf[sample];
                if (stream_buf[sample] > 32767 || stream_buf[sample] < -32768)
                    m_clipped_samples++;
                m_total_samples++;
            }
#endif

            // mix if sound is enabled
            if (!suppress)
            {
                // if the speaker is centered, send to both left and right
                if (m_x == 0)
                {
                    for (int sample = 0; sample < samples_this_update; sample++)
                    {
                        leftmix[sample] += stream_buf[sample];
                        rightmix[sample] += stream_buf[sample];
                    }
                }
                // if the speaker is to the left, send only to the left
                else if (m_x < 0)
                {
                    for (int sample = 0; sample < samples_this_update; sample++)
                        leftmix[sample] += stream_buf[sample];
                }
                // if the speaker is to the right, send only to the right
                else
                {
                    for (int sample = 0; sample < samples_this_update; sample++)
                        rightmix[sample] += stream_buf[sample];
                }
            }
        }

        // device-level overrides

        //-------------------------------------------------
        //  device_start - handle device startup
        //-------------------------------------------------
        protected override void device_start()
        {
            // dummy save to make device.c happy
        }
    }


    // speaker device iterator
    //typedef device_type_iterator<&device_creator<speaker_device>, speaker_device> speaker_device_iterator;
    public class speaker_device_iterator : device_type_iterator<speaker_device>
    {
        public speaker_device_iterator(device_t root, int maxdepth = 255) : base(speaker_device.SPEAKER, root, maxdepth) { }
    }
}
