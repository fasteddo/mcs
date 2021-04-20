// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using s32 = System.Int32;
using stream_sample_t = System.Int32;
using u32 = System.UInt32;


namespace mame
{
    //#ifndef SPEAKER_TRACK_MAX_SAMPLE
    //#ifdef MAME_DEBUG
    //#define SPEAKER_TRACK_MAX_SAMPLE (1)
    //#else
    //#define SPEAKER_TRACK_MAX_SAMPLE (0)
    //#endif
    //#endif


    // ======================> speaker_device
    public class speaker_device : device_t
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
        const int BUCKETS_PER_SECOND = 10;
        std.vector<s32> m_max_sample;
        s32 m_current_max;
        u32 m_samples_this_bucket;


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
            m_current_max = 0;
            m_samples_this_bucket = 0;
        }

        //~speaker_device()


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

            // track maximum sample value for each 0.1s bucket
            if (machine().options().speaker_report() != 0)
            {
                throw new emu_unimplemented();
#if false
                u32 samples_per_bucket = m_mixer_stream.sample_rate() / BUCKETS_PER_SECOND;
                for (int sample = 0; sample < samples_this_update; sample++)
                {
                    m_current_max = std.max(m_current_max, std.abs(stream_buf[sample]));
                    if (++m_samples_this_bucket >= samples_per_bucket)
                    {
                        m_max_sample.push_back(m_current_max);
                        m_current_max = 0;
                        m_samples_this_bucket = 0;
                    }
                }
#endif
            }

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


        //-------------------------------------------------
        //  device_stop - cleanup and report
        //-------------------------------------------------
        protected override void device_stop()
        {
            // level 1: just report if there was any clipping
            // level 2: report the overall maximum, even if no clipping
            // level 3: print a detailed list of all the times there was clipping
            // level 4: print a detailed list of every bucket
            int report = machine().options().speaker_report();
            if (report != 0)
            {
                m_max_sample.push_back(m_current_max);

                // determine overall maximum and number of clipped buckets
                s32 overallmax = 0;
                u32 clipped = 0;
                foreach (var curmax in m_max_sample)
                {
                    overallmax = std.max(overallmax, curmax);
                    if (curmax > 32767)
                        clipped++;
                }

                // levels 1 and 2 just get a summary
                if (clipped != 0 || report == 2 || report == 4)
                    osd_printf_info("Speaker \"%s\" - max = %d (gain *= %.3f) - clipped in %d/%d (%d%%) buckets\n", tag(), overallmax, 32767.0 / (overallmax != 0 ? overallmax : 1), clipped, m_max_sample.size(), clipped * 100 / m_max_sample.size());

                // levels 3 and 4 get a full dump
                if (report >= 3)
                {
                    double t = 0;
                    foreach (var curmax in m_max_sample)
                    {
                        if (curmax > 32767 || report == 4)
                            osd_printf_info("   t={0}  max={1}\n", t, curmax);
                        t += 1.0 / (double)BUCKETS_PER_SECOND;
                    }
                }
            }
        }
    }


    // speaker device iterator
    //typedef device_type_iterator<&device_creator<speaker_device>, speaker_device> speaker_device_iterator;
    public class speaker_device_iterator : device_type_iterator<speaker_device>
    {
        public speaker_device_iterator(device_t root, int maxdepth = 255) : base(speaker_device.SPEAKER, root, maxdepth) { }
    }
}
