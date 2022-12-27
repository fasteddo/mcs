// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using s32 = System.Int32;
using speaker_device_enumerator = mame.device_type_enumerator<mame.speaker_device>;  //using speaker_device_enumerator = device_type_enumerator<speaker_device>;
using stream_buffer_sample_t = System.Single;  //using sample_t = float;
using u32 = System.UInt32;

using static mame.device_global;
using static mame.osdcore_global;
using static mame.sound_global;
using static mame.speaker_global;


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
                                  //public device_mixer_interface
    {
        //DEFINE_DEVICE_TYPE(SPEAKER, speaker_device, "speaker", "Speaker")
        public static readonly emu.detail.device_type_impl SPEAKER = DEFINE_DEVICE_TYPE("speaker", "Speaker", (type, mconfig, tag, owner, clock) => { return new speaker_device(mconfig, tag, owner, clock); });


        device_mixer_interface m_dimixer;

        // inline configuration state
        double m_x;
        double m_y;
        double m_z;

        // internal state
        const int BUCKETS_PER_SECOND = 10;
        std.vector<stream_buffer_sample_t> m_max_sample;
        stream_buffer_sample_t m_current_max;
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


        // device_sound_interface
        public sound_stream device_sound_interface_output_to_stream_output(int outputnum, out int stream_outputnum) { return m_dimixer.output_to_stream_output(outputnum, out stream_outputnum); }
        public int device_sound_interface_outputs() { return m_dimixer.outputs(); }


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
        public void mix(Pointer<stream_buffer_sample_t> leftmix, Pointer<stream_buffer_sample_t> rightmix, attotime start, attotime end, int expected_samples, bool suppress)  //void mix(stream_buffer::sample_t *leftmix, stream_buffer::sample_t *rightmix, attotime start, attotime end, int expected_samples, bool suppress)
        {
            // skip if no stream
            if (m_dimixer.m_mixer_stream == null)
                return;

            // skip if invalid range
            if (start > end)
                return;

            // get a view on the desired range
            read_stream_view view = m_dimixer.m_mixer_stream.update_view(start, end);

            sound_assert(view.samples() >= expected_samples);

            // track maximum sample value for each 0.1s bucket
            if (machine().options().speaker_report() != 0)
            {
                u32 samples_per_bucket = m_dimixer.m_mixer_stream.sample_rate() / BUCKETS_PER_SECOND;
                for (int sample = 0; sample < expected_samples; sample++)
                {
                    m_current_max = std.max(m_current_max, std.fabsf(view.get(sample)));
                    if (++m_samples_this_bucket >= samples_per_bucket)
                    {
                        m_max_sample.push_back(m_current_max);
                        m_current_max = 0.0f;
                        m_samples_this_bucket = 0;
                    }
                }
            }

            // mix if sound is enabled
            if (!suppress)
            {
                // if the speaker is centered, send to both left and right
                if (m_x == 0)
                    for (int sample = 0; sample < expected_samples; sample++)
                    {
                        stream_buffer_sample_t cursample = view.get(sample);
                        leftmix[sample] += cursample;
                        rightmix[sample] += cursample;
                    }

                // if the speaker is to the left, send only to the left
                else if (m_x < 0)
                    for (int sample = 0; sample < expected_samples; sample++)
                        leftmix[sample] += view.get(sample);

                // if the speaker is to the right, send only to the right
                else
                    for (int sample = 0; sample < expected_samples; sample++)
                        rightmix[sample] += view.get(sample);
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
                stream_buffer_sample_t overallmax = 0;
                u32 clipped = 0;
                foreach (var curmax in m_max_sample)
                {
                    overallmax = std.max(overallmax, curmax);
                    if (curmax > (stream_buffer_sample_t)1.0)
                        clipped++;
                }

                // levels 1 and 2 just get a summary
                if (clipped != 0 || report == 2 || report == 4)
                    osd_printf_info("Speaker \"{0}\" - max = {1} (gain *= {2}) - clipped in {3}/{4} ({5}%%) buckets\n", tag(), overallmax, 1 / (overallmax != 0 ? overallmax : 1), clipped, m_max_sample.size(), clipped * 100 / m_max_sample.size());

                // levels 3 and 4 get a full dump
                if (report >= 3)
                {
                    const string s_stars  = "************************************************************";
                    const string s_spaces = "                                                            ";
                    int totalstars = (int)std.strlen(s_stars);
                    double t = 0;
                    if (overallmax < 1.0)
                        overallmax = (stream_buffer_sample_t)1.0;
                    int leftstars = totalstars / (int)overallmax;
                    foreach (var curmax in m_max_sample)
                    {
                        if (curmax > (stream_buffer_sample_t)1.0 || report == 4)
                        {
                            osd_printf_info("{0}: {1} |", t, curmax);
                            if (curmax == 0)
                            {
                                osd_printf_info("{0}{1}|\n", leftstars, s_spaces);
                            }
                            else if (curmax <= 1.0)
                            {
                                int stars = std.max(1, std.min(leftstars, (int)(curmax * totalstars / overallmax)));
                                osd_printf_info("{0}{1}", stars, s_stars);
                                int spaces = leftstars - stars;
                                if (spaces != 0)
                                    osd_printf_info("{0}{1}", spaces, s_spaces);
                                osd_printf_info("|\n");
                            }
                            else
                            {
                                int rightstars = std.max(1, std.min(totalstars, (int)(curmax * totalstars / overallmax)) - leftstars);
                                osd_printf_info("{0}|{1}\n", leftstars, s_stars, rightstars, s_stars);
                            }
                        }

                        t += 1.0 / (double)BUCKETS_PER_SECOND;
                    }
                }
            }
        }
    }


    //using speaker_device_enumerator = device_type_enumerator<speaker_device>;


    static class speaker_global
    {
        public static speaker_device SPEAKER(machine_config mconfig, string tag) { return emu.detail.device_type_impl.op<speaker_device>(mconfig, tag, speaker_device.SPEAKER, 0); }
    }
}
