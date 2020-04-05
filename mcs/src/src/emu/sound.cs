// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using attoseconds_t = System.Int64;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using stream_sample_t = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    //typedef delegate<void (sound_stream &, stream_sample_t **inputs, stream_sample_t **outputs, int samples)> stream_update_delegate;
    public delegate void stream_update_delegate(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples);


    public static class sound_global
    {
        public const int STREAM_SYNC = -1;       // special rate value indicating a one-sample-at-a-time stream
                                                 // with actual rate defined by its input

        public const bool VERBOSE = false;

        public static void VPRINTF(string format, params object [] args) { if (VERBOSE) global_object.osd_printf_debug(format, args); }
    }


    // ======================> sound_stream
    public class sound_stream : global_object, simple_list_item<sound_stream>
    {
        //friend class sound_manager;


        //typedef void (*stream_update_func)(device_t *device, sound_stream *stream, void *param, stream_sample_t **inputs, stream_sample_t **outputs, int samples);


        // stream output class
        class stream_output
        {
            public sound_stream m_stream;               // owning stream
            public std.vector<stream_sample_t> m_buffer = new std.vector<stream_sample_t>();    // output buffer
            public int m_dependents;           // number of dependents
            public s16 m_gain;                 // gain to apply to the output


            // construction/destruction
            //-------------------------------------------------
            //  stream_output - constructor
            //-------------------------------------------------
            public stream_output()
            {
                m_stream = null;
                m_dependents = 0;
                m_gain = 0x100;
            }


            //stream_output &operator=(const stream_output &rhs) { assert(false); return *this; }
        }


        // stream input class
        class stream_input
        {
            public stream_output m_source;               // pointer to the sound_output for this source
            public std.vector<stream_sample_t> m_resample = new std.vector<stream_sample_t>();  // buffer for resampling to the stream's sample rate
            public attoseconds_t m_latency_attoseconds;  // latency between this stream and the input stream
            public s16 m_gain;                 // gain to apply to this input
            public s16 m_user_gain;            // user-controlled gain to apply to this input


            // construction/destruction
            //-------------------------------------------------
            //  stream_input - constructor
            //-------------------------------------------------
            public stream_input()
            {
                m_source = null;
                m_latency_attoseconds = 0;
                m_gain = 0x100;
                m_user_gain = 0x100f;
            }


            //stream_input &operator=(const stream_input &rhs) { assert(false); return *this; }
        }


        // constants
        const int OUTPUT_BUFFER_UPDATES = 5;
        const u32 FRAC_BITS = 22;
        const u32 FRAC_ONE = 1 << (int)FRAC_BITS;
        const u32 FRAC_MASK = FRAC_ONE - 1;


        // linking information
        device_t m_device;                     // owning device
        sound_stream m_next;                       // next stream in the chain

        // general information
        u32 m_sample_rate;                // sample rate of this stream
        u32 m_new_sample_rate;            // newly-set sample rate for the stream
        bool m_synchronous;                // synchronous stream that runs at the rate of its input

        // timing information
        attoseconds_t m_attoseconds_per_sample;     // number of attoseconds per sample
        s32 m_max_samples_per_update;     // maximum samples per update
        emu_timer m_sync_timer;                 // update timer for synchronous streams

        // input information
        std.vector<stream_input> m_input = new std.vector<stream_input>();              // list of streams we directly depend upon
        ListPointer<stream_sample_t> [] m_input_array;  //std::vector<stream_sample_t *> m_input_array;   // array of inputs for passing to the callback

        // resample buffer information
        u32 m_resample_bufalloc;          // allocated size of each resample buffer

        // output information
        std.vector<stream_output> m_output = new std.vector<stream_output>();            // list of streams which directly depend upon us
        ListPointer<stream_sample_t> [] m_output_array;  //std::vector<stream_sample_t *> m_output_array;  // array of outputs for passing to the callback

        // output buffer information
        u32 m_output_bufalloc;            // allocated size of each output buffer
        s32 m_output_sampindex;           // current position within each output buffer
        s32 m_output_update_sampindex;    // position at time of last global update
        s32 m_output_base_sampindex;      // sample at base of buffer, relative to the current emulated second

        // callback information
        stream_update_delegate  m_callback;                   // callback function


        // construction/destruction
        //-------------------------------------------------
        //  sound_stream - constructor
        //-------------------------------------------------
        public sound_stream(device_t device, int inputs, int outputs, int sample_rate, stream_update_delegate callback)
        {
            m_device = device;
            m_next = null;
            m_sample_rate = (UInt32)sample_rate;
            m_new_sample_rate = 0xffffffff;
            m_attoseconds_per_sample = 0;
            m_max_samples_per_update = 0;
            m_input = new std.vector<stream_input>();  //(inputs)
            for (int i = 0; i < inputs; i++) m_input.Add(new stream_input());
            m_input_array = new ListPointer<stream_sample_t>[inputs];
            m_resample_bufalloc = 0;
            m_output = new std.vector<stream_output>();  //(outputs)
            for (int i = 0; i < outputs; i++) m_output.Add(new stream_output());
            m_output_array = new ListPointer<stream_sample_t>[outputs];
            m_output_bufalloc = 0;
            m_output_sampindex = 0;
            m_output_update_sampindex = 0;
            m_output_base_sampindex = 0;
            m_callback = callback;


            // get the device's sound interface
            device_sound_interface sound;
            if (!device.interface_(out sound))
                throw new emu_fatalerror("Attempted to create a sound_stream with a non-sound device");

            if (m_callback == null)
                m_callback = sound.sound_stream_update;

            // create a unique tag for saving
            string state_tag = string.Format("{0}", m_device.machine().sound().streams().size());
            m_device.machine().save().save_item(device, "stream", state_tag, 0, m_sample_rate, "m_sample_rate");
            m_device.machine().save().register_postload(postload);

            // save the gain of each input and output
            for (int inputnum = 0; inputnum < m_input.size(); inputnum++)
            {
                m_device.machine().save().save_item(device, "stream", state_tag, inputnum, m_input[inputnum].m_gain, "m_input[inputnum].m_gain");
                m_device.machine().save().save_item(device, "stream", state_tag, inputnum, m_input[inputnum].m_user_gain, "m_input[inputnum].m_user_gain");
            }

            for (int outputnum = 0; outputnum < m_output.size(); outputnum++)
            {
                m_output[outputnum].m_stream = this;
                m_device.machine().save().save_item(device, "stream", state_tag, outputnum, m_output[outputnum].m_gain, "m_output[outputnum].m_gain");
            }

            // Mark synchronous streams as such
            m_synchronous = (int)m_sample_rate == sound_global.STREAM_SYNC;
            if (m_synchronous)
            {
                m_sample_rate = 0;
                m_sync_timer = m_device.machine().scheduler().timer_alloc(sync_update, this);
            }
            else
            {
                m_sync_timer = null;
            }

            // force an update to the sample rates; this will cause everything to be recomputed
            // and will generate the initial resample buffers for our inputs
            recompute_sample_rate_data();

            // set up the initial output buffer positions now that we have data
            m_output_base_sampindex = -m_max_samples_per_update;
        }


        // getters
        public sound_stream next() { return m_next; }
        public sound_stream m_next_get() { return m_next; }
        public void m_next_set(sound_stream value) { m_next = value; }

        public device_t device() { return m_device; }
        int sample_rate() { return (m_new_sample_rate != 0xffffffff) ? (int)m_new_sample_rate : (int)m_sample_rate; }
        //attotime sample_time() const;
        //attotime sample_period() const { return attotime(0, m_attoseconds_per_sample); }
        public int input_count() { return m_input.size(); }
        public int output_count() { return m_output.size(); }
        //const char *input_name(int inputnum) const;
        //device_t *input_source_device(int inputnum) const;
        //int input_source_outputnum(int inputnum) const;
        //float user_gain(int inputnum) const;
        //float input_gain(int inputnum) const;
        //float output_gain(int outputnum) const;


        // operations

        //-------------------------------------------------
        //  set_input - configure a stream's input
        //-------------------------------------------------
        public void set_input(int index, sound_stream input_stream, int output_index = 0, float gain = 1.0f)
        {
            sound_global.VPRINTF("stream_set_input({0}, '{1}', {2}, {3}, {4}, {5})\n", this, m_device.tag(), index, input_stream, output_index, gain);

            // make sure it's a valid input
            if (index >= m_input.size())
                throw new emu_fatalerror("stream_set_input attempted to configure nonexistant input {0} ({1} max)\n", index, m_input.size());

            // make sure it's a valid output
            if (input_stream != null && output_index >= input_stream.m_output.size())
                throw new emu_fatalerror("stream_set_input attempted to use a nonexistant output {0} ({1} max)\n", output_index, m_output.size());

            // if this input is already wired, update the dependent info
            stream_input input = m_input[index];
            if (input.m_source != null)
                input.m_source.m_dependents--;

            // wire it up
            input.m_source = (input_stream != null) ? input_stream.m_output[output_index] : null;
            input.m_gain = (Int16)(int)(0x100 * gain);
            input.m_user_gain = 0x100;

            // update the dependent info
            if (input.m_source != null)
                input.m_source.m_dependents++;

            // update sample rates now that we know the input
            recompute_sample_rate_data();
        }

        //-------------------------------------------------
        //  update - force a stream to update to
        //  the current emulated time
        //-------------------------------------------------
        public void update()
        {
            if (m_attoseconds_per_sample == 0)
                return;

            // determine the number of samples since the start of this second
            attotime time = m_device.machine().time();
            int update_sampindex = (int)(time.attoseconds() / m_attoseconds_per_sample);

            // if we're ahead of the last update, then adjust upwards
            attotime last_update = m_device.machine().sound().last_update();
            if (time.seconds() > last_update.seconds())
            {
                assert(time.seconds() == last_update.seconds() + 1);
                update_sampindex += (int)m_sample_rate;
            }

            // if we're behind the last update, then adjust downwards
            if (time.seconds() < last_update.seconds())
            {
                assert(time.seconds() == last_update.seconds() - 1);
                update_sampindex -= (int)m_sample_rate;
            }

            if (update_sampindex <= m_output_sampindex)
                return;

            // generate samples to get us up to the appropriate time
            profiler_global.g_profiler.start(profile_type.PROFILER_SOUND);

            //throw new emu_unimplemented();
#if false
            osdcomm_global.assert(m_output_sampindex - m_output_base_sampindex >= 0);
            osdcomm_global.assert(update_sampindex - m_output_base_sampindex <= m_output_bufalloc);
#endif

            generate_samples(update_sampindex - m_output_sampindex);
            profiler_global.g_profiler.stop();

            // remember this info for next time
            m_output_sampindex = update_sampindex;
        }

        //-------------------------------------------------
        //  output_since_last_update - return a pointer to
        //  the output buffer and the number of samples
        //  since the last global update
        //-------------------------------------------------
        public ListPointer<stream_sample_t> output_since_last_update(int outputnum, out int numsamples)
        {
            // force an update on the stream
            update();

            // compute the number of samples and a pointer to the output buffer
            numsamples = m_output_sampindex - m_output_update_sampindex;
            return new ListPointer<stream_sample_t>(m_output[outputnum].m_buffer, m_output_update_sampindex - m_output_base_sampindex);  // &m_output[outputnum].m_buffer[m_output_update_sampindex - m_output_base_sampindex];
        }


        // timing

        //-------------------------------------------------
        //  set_sample_rate - set the sample rate on a
        //  given stream
        //-------------------------------------------------
        public void set_sample_rate(int new_rate)
        {
            // we will update this on the next global update
            if (new_rate != sample_rate())
                m_new_sample_rate = (UInt32)new_rate;
        }


        //void set_user_gain(int inputnum, float gain);
        //void set_input_gain(int inputnum, float gain);


        //-------------------------------------------------
        //  set_output_gain - set the output gain on a
        //  given stream's output
        //-------------------------------------------------
        public void set_output_gain(int outputnum, float gain)
        {
            update();
            assert(outputnum >= 0 && outputnum < m_output.size());
            m_output[outputnum].m_gain = (s16)(int)(0x100 * gain);
        }


        // helpers called by our friends only

        //-------------------------------------------------
        //  update_with_accounting - do a regular update,
        //  but also do periodic accounting
        //-------------------------------------------------
        public void update_with_accounting(bool second_tick)
        {
            // do the normal update
            update();

            // if we've ticked over another second, adjust all the counters that are relative to
            // the current second
            int output_bufindex = m_output_sampindex - m_output_base_sampindex;
            if (second_tick)
            {
                m_output_sampindex -= (int)m_sample_rate;
                m_output_base_sampindex -= (int)m_sample_rate;
            }

            // note our current output sample
            m_output_update_sampindex = m_output_sampindex;

            // if we don't have enough output buffer space to hold two updates' worth of samples,
            // we need to shuffle things down
            if (m_output_bufalloc - output_bufindex < 2 * m_max_samples_per_update)
            {
                int samples_to_lose = output_bufindex - m_max_samples_per_update;
                if (samples_to_lose > 0)
                {
                    // if we have samples to move, do so for each output
                    if (output_bufindex > 0)
                    {
                        for (int outputnum = 0; outputnum < m_output.size(); outputnum++)
                        {
                            stream_output output = m_output[outputnum];
                            //memmove(&output.m_buffer[0], &output.m_buffer[samples_to_lose], sizeof(output.m_buffer[0]) * (output_bufindex - samples_to_lose));
                            output.m_buffer.copy(0, samples_to_lose, output.m_buffer, output_bufindex - samples_to_lose);  // TODO: not sure if there's supposed to be overlap here
                        }
                    }

                    // update the base position
                    m_output_base_sampindex += samples_to_lose;
                }
            }
        }

        //-------------------------------------------------
        //  apply_sample_rate_changes - if there is a
        //  pending sample rate change, apply it now
        //-------------------------------------------------
        public void apply_sample_rate_changes()
        {
            // skip if nothing to do
            if (m_new_sample_rate == 0xffffffff)
                return;

            // update to the new rate and remember the old rate
            u32 old_rate = m_sample_rate;
            m_sample_rate = m_new_sample_rate;
            m_new_sample_rate = 0xffffffff;

            // recompute all the data
            recompute_sample_rate_data();

            // reset our sample indexes to the current time
            if (old_rate != 0)
            {
                m_output_sampindex = (s32)((s64)m_output_sampindex * (s64)m_sample_rate / old_rate);
                m_output_update_sampindex = (s32)((s64)m_output_update_sampindex * (s64)m_sample_rate / old_rate);
            }
            else
            {
                m_output_sampindex = m_attoseconds_per_sample != 0 ? (s32)(m_device.machine().sound().last_update().attoseconds() / m_attoseconds_per_sample) : 0;
                m_output_update_sampindex = m_output_sampindex;
            }

            m_output_base_sampindex = m_output_sampindex - m_max_samples_per_update;

            // clear out the buffer
            if (m_max_samples_per_update != 0)
            {
                for (int outputnum = 0; outputnum < m_output.size(); outputnum++)  // for (auto & elem : m_output)
                {
                    var elem = m_output[outputnum];
                    memset(elem.m_buffer, 0, (UInt32)m_max_samples_per_update);  //memset(&elem.m_buffer[0], 0, m_max_samples_per_update * sizeof(elem.m_buffer[0]));
                }
            }
        }


        // internal helpers

        //-------------------------------------------------
        //  recompute_sample_rate_data - recompute sample
        //  rate data, and all streams that are affected
        //  by this stream
        //-------------------------------------------------
        void recompute_sample_rate_data()
        {
            if (m_synchronous)
            {
                m_sample_rate = 0;
                // When synchronous, pick the sample rate for the inputs, if any
                for (int inputnum = 0; inputnum < m_input.size(); inputnum++)
                {
                    stream_input input = m_input[inputnum];
                    if (input.m_source != null)
                    {
                        if (m_sample_rate == 0)
                            m_sample_rate = input.m_source.m_stream.m_sample_rate;
                        else if (m_sample_rate != input.m_source.m_stream.m_sample_rate)
                            throw new emu_fatalerror("Incompatible sample rates as input of a synchronous stream: {0} and {1}\n", m_sample_rate, input.m_source.m_stream.m_sample_rate);
                    }
                }
            }


            // recompute the timing parameters
            attoseconds_t update_attoseconds = m_device.machine().sound().update_attoseconds();

            if (m_sample_rate != 0)
            {
                m_attoseconds_per_sample = attotime.ATTOSECONDS_PER_SECOND / m_sample_rate;
                m_max_samples_per_update = (int)((update_attoseconds + m_attoseconds_per_sample - 1) / m_attoseconds_per_sample);
            }
            else
            {
                m_attoseconds_per_sample = 0;
                m_max_samples_per_update = 0;
            }

            // update resample and output buffer sizes
            allocate_resample_buffers();
            allocate_output_buffers();

            // iterate over each input
            for (int inputnum = 0; inputnum < m_input.size(); inputnum++)  // for (auto & input : m_input)
            {
                var input = m_input[inputnum];

                // if we have a source, see if its sample rate changed
                if (input.m_source != null && input.m_source.m_stream.m_sample_rate != 0)
                {
                    // okay, we have a new sample rate; recompute the latency to be the maximum
                    // sample period between us and our input
                    attoseconds_t new_attosecs_per_sample = attotime.ATTOSECONDS_PER_SECOND / input.m_source.m_stream.m_sample_rate;
                    attoseconds_t latency = Math.Max(new_attosecs_per_sample, m_attoseconds_per_sample);

                    // if the input stream's sample rate is lower, we will use linear interpolation
                    // this requires an extra sample from the source
                    if (input.m_source.m_stream.m_sample_rate < m_sample_rate)
                        latency += new_attosecs_per_sample;

                    // if our sample rates match exactly, we don't need any latency
                    else if (input.m_source.m_stream.m_sample_rate == m_sample_rate)
                        latency = 0;

                    // we generally don't want to tweak the latency, so we just keep the greatest
                    // one we've computed thus far
                    input.m_latency_attoseconds = Math.Max(input.m_latency_attoseconds, latency);

                    //throw new emu_unimplemented();
#if false
                    assert(input.m_latency_attoseconds < update_attoseconds);
#endif
                }
                else
                {
                    input.m_latency_attoseconds = 0;
                }
            }

            // If synchronous, prime the timer
            if (m_synchronous)
            {
                attotime time = m_device.machine().time();
                if (m_attoseconds_per_sample != 0)
                {
                    attoseconds_t next_edge = m_attoseconds_per_sample - (time.attoseconds() % m_attoseconds_per_sample);
                    m_sync_timer.adjust(new attotime(0, next_edge));
                }
                else
                {
                    m_sync_timer.adjust(attotime.never);
                }
            }
        }

        //-------------------------------------------------
        //  allocate_resample_buffers - recompute the
        //  resample buffer sizes and expand if necessary
        //-------------------------------------------------
        void allocate_resample_buffers()
        {
            // compute the target number of samples
            int bufsize = 2 * m_max_samples_per_update;

            // if we don't have enough room, allocate more
            if (m_resample_bufalloc < bufsize)
            {
                // this becomes the new allocation size
                m_resample_bufalloc = (UInt32)bufsize;

                // iterate over outputs and realloc their buffers
                for (int inputnum = 0; inputnum < m_input.size(); inputnum++)
                    m_input[inputnum].m_resample.resize((int)m_resample_bufalloc);
            }
        }

        //-------------------------------------------------
        //  allocate_output_buffers - recompute the
        //  output buffer sizes and expand if necessary
        //-------------------------------------------------
        void allocate_output_buffers()
        {
            // if we don't have enough room, allocate more
            int bufsize = OUTPUT_BUFFER_UPDATES * m_max_samples_per_update;
            if (m_output_bufalloc < bufsize)
            {
                // this becomes the new allocation size
                m_output_bufalloc = (UInt32)bufsize;

                // iterate over outputs and realloc their buffers
                for (int outputnum = 0; outputnum < m_output.size(); outputnum++)
                    m_output[outputnum].m_buffer.resize((int)m_output_bufalloc);
            }
        }

        //-------------------------------------------------
        //  postload - save/restore callback
        //-------------------------------------------------
        void postload()
        {
            // recompute the same rate information
            recompute_sample_rate_data();

            // make sure our output buffers are fully cleared
            for (int outputnum = 0; outputnum < m_output.size(); outputnum++)  //for (auto & elem : m_output)
            {
                var elem = m_output[outputnum];
                memset(elem.m_buffer, 0, m_output_bufalloc);  //memset(&elem.m_buffer[0], 0, m_output_bufalloc * sizeof(elem.m_buffer[0]));
            }

            // recompute the sample indexes to make sense
            m_output_sampindex = m_attoseconds_per_sample != 0 ? (s32)(m_device.machine().sound().last_update().attoseconds() / m_attoseconds_per_sample) : 0;
            m_output_update_sampindex = m_output_sampindex;
            m_output_base_sampindex = m_output_sampindex - m_max_samples_per_update;
        }

        //-------------------------------------------------
        //  generate_samples - generate the requested
        //  number of samples for a stream, making sure
        //  all inputs have the appropriate number of
        //  samples generated
        //-------------------------------------------------
        void generate_samples(int samples)
        {
            ListPointer<stream_sample_t> [] inputs = null;  //stream_sample_t **inputs = nullptr;
            ListPointer<stream_sample_t> [] outputs = null;  //stream_sample_t **outputs = nullptr;

            sound_global.VPRINTF("generate_samples({0}, {1})\n", this, samples);
            assert(samples > 0);

            // ensure all inputs are up to date and generate resampled data
            for (int inputnum = 0; inputnum < m_input.size(); inputnum++)
            {
                // update the stream to the current time
                stream_input input = m_input[inputnum];
                if (input.m_source != null)
                    input.m_source.m_stream.update();

                // generate the resampled data
                m_input_array[inputnum] = generate_resampled_data(input, (UInt32)samples);
            }

            if (!m_input.empty())
            {
                inputs = m_input_array;
            }

            // loop over all outputs and compute the output pointer
            for (int outputnum = 0; outputnum < m_output.size(); outputnum++)
            {
                stream_output output = m_output[outputnum];
                m_output_array[outputnum] = new ListPointer<stream_sample_t>(output.m_buffer, m_output_sampindex - m_output_base_sampindex);  // m_output_array[outputnum] = &output.m_buffer[m_output_sampindex - m_output_base_sampindex];
            }

            if (!m_output.empty())
            {
                outputs = m_output_array;
            }

            // run the callback
            sound_global.VPRINTF("  callback({0}, {1})\n", this, samples);
            m_callback(this, inputs, outputs, samples);
            sound_global.VPRINTF("  callback done\n");
        }

        //-------------------------------------------------
        //  generate_resampled_data - generate the
        //  resample buffer for a given input
        //-------------------------------------------------
        ListPointer<stream_sample_t> generate_resampled_data(stream_input input, UInt32 numsamples)
        {
            // if we don't have an output to pull data from, generate silence
            ListPointer<stream_sample_t> dest = new ListPointer<stream_sample_t>(input.m_resample);  // stream_sample_t *dest = input.m_resample;
            if (input.m_source == null || input.m_source.m_stream.m_attoseconds_per_sample == 0)
            {
                memset(dest, 0, numsamples);  //memset(dest, 0, numsamples * sizeof(*dest));
                return new ListPointer<stream_sample_t>(input.m_resample);
            }

            // grab data from the output
            stream_output output = input.m_source;  // stream_output &output = *input.m_source;
            sound_stream input_stream = output.m_stream;  // sound_stream &input_stream = *output.m_stream;
            s64 gain = (input.m_gain * input.m_user_gain * output.m_gain) >> 16;

            // determine the time at which the current sample begins, accounting for the
            // latency we calculated between the input and output streams
            attoseconds_t basetime = m_output_sampindex * m_attoseconds_per_sample - input.m_latency_attoseconds;

            // now convert that time into a sample in the input stream
            s32 basesample;
            if (basetime >= 0)
                basesample = (int)(basetime / input_stream.m_attoseconds_per_sample);
            else
                basesample = (int)(-(-basetime / input_stream.m_attoseconds_per_sample) - 1);

            // compute a source pointer to the first sample
            assert(basesample >= input_stream.m_output_base_sampindex);
            ListPointer<stream_sample_t> source = new ListPointer<stream_sample_t>(output.m_buffer, basesample - input_stream.m_output_base_sampindex);  // stream_sample_t *source = &output.m_buffer[basesample - input_stream.m_output_base_sampindex];

            // determine the current fraction of a sample, expressed as a fraction of FRAC_ONE
            // (Note: this formula is valid as long as input_stream.m_attoseconds_per_sample signficantly exceeds FRAC_ONE > attoseconds = 4.2E-12 s)
            u32 basefrac = (u32)((basetime - basesample * input_stream.m_attoseconds_per_sample) / ((input_stream.m_attoseconds_per_sample + FRAC_ONE - 1) >> (int)FRAC_BITS));
            assert(basefrac < FRAC_ONE);

            // compute the stepping fraction
            u32 step = (u32)(((u64)(input_stream.m_sample_rate) << (int)FRAC_BITS) / m_sample_rate);

            // if we have equal sample rates, we just need to copy
            if (step == FRAC_ONE)
            {
                while (numsamples-- != 0)
                {
                    // compute the sample
                    s64 sample = source[0];  // *source++;
                    source++;
                    dest[0] = (int)((sample * gain) >> 8);  // *dest++ = (sample * gain) >> 8;
                    dest++;
                }
            }

            // input is undersampled: point sample except where our sample period covers a boundary
            else if (step < FRAC_ONE)
            {
                while (numsamples != 0)
                {
                    // fill in with point samples until we hit a boundary
                    int nextfrac;
                    while ((nextfrac = (int)(basefrac + step)) < FRAC_ONE && numsamples-- != 0)
                    {
                        dest[0] = (int)((source[0] * gain) >> 8);  // *dest++ = (source[0] * gain) >> 8;
                        dest++;
                        basefrac = (UInt32)nextfrac;
                    }

                    // if we're done, we're done
                    if ((s32)(numsamples--) < 0)
                        break;

                    // compute starting and ending fractional positions
                    int startfrac = (int)(basefrac >> (int)(FRAC_BITS - 12));
                    int endfrac = nextfrac >> (int)(FRAC_BITS - 12);

                    // blend between the two samples accordingly
                    s64 sample = ((s64)source[0] * (0x1000 - startfrac) + (s64)source[1] * (endfrac - 0x1000)) / (endfrac - startfrac);
                    dest[0] = (int)((sample * gain) >> 8);  // *dest++ = (sample * gain) >> 8;
                    dest++;

                    // advance
                    basefrac = (UInt32)(nextfrac & FRAC_MASK);
                    source++;
                }
            }

            // input is oversampled: sum the energy
            else
            {
                // use 8 bits to allow some extra headroom
                int smallstep = (int)(step >> (int)(FRAC_BITS - 8));
                while (numsamples-- != 0)
                {
                    s64 remainder = smallstep;
                    int tpos = 0;

                    // compute the sample
                    s64 scale = (FRAC_ONE - basefrac) >> (int)(FRAC_BITS - 8);
                    s64 sample = (s64)source[tpos++] * scale;
                    remainder -= scale;
                    while (remainder > 0x100)
                    {
                        sample += (Int64)source[tpos++] * (Int64)0x100;
                        remainder -= 0x100;
                    }

                    sample += (Int64)source[tpos] * remainder;
                    sample /= smallstep;

                    dest[0] = (int)((sample * gain) >> 8);  // *dest++ = (sample * gain) >> 8;
                    dest++;

                    // advance
                    basefrac += step;
                    source += basefrac >> (int)FRAC_BITS;
                    basefrac &= FRAC_MASK;
                }
            }

            return new ListPointer<stream_sample_t>(input.m_resample);
        }

        void sync_update(object o, int param)  //(void *, INT32)
        {
            update();
            attotime time = m_device.machine().time();
            attoseconds_t next_edge = m_attoseconds_per_sample - (time.attoseconds() % m_attoseconds_per_sample);
            m_sync_timer.adjust(new attotime(0, next_edge));
        }
    }


    // ======================> sound_manager
    public class sound_manager : global_object
    {
        //friend class sound_stream;


        // reasons for muting
        const u8 MUTE_REASON_PAUSE = 0x01;
        const u8 MUTE_REASON_UI = 0x02;
        const u8 MUTE_REASON_DEBUGGER = 0x04;
        const u8 MUTE_REASON_SYSTEM = 0x08;

        // stream updates
        static readonly attotime STREAMS_UPDATE_ATTOTIME = attotime.from_hz(STREAMS_UPDATE_FREQUENCY);

        public const int STREAMS_UPDATE_FREQUENCY = 50;


        // internal state
        running_machine m_machine;              // reference to our machine
        emu_timer m_update_timer;         // timer to drive periodic updates

        u32 m_finalmix_leftover;
        std.vector<s16> m_finalmix = new std.vector<s16>();
        std.vector<s32> m_leftmix = new std.vector<s32>();
        std.vector<s32> m_rightmix = new std.vector<s32>();

        u8 m_muted;
        int m_attenuation;
        int m_nosound_mode;

        wav_file m_wavfile;

        // streams data
        std.vector<sound_stream> m_stream_list = new std.vector<sound_stream>();  //std::vector<std::unique_ptr<sound_stream>> m_stream_list;    // list of streams
        attoseconds_t m_update_attoseconds;   // attoseconds between global updates
        attotime m_last_update;          // last update time


        // construction/destruction

        //-------------------------------------------------
        //  sound_manager - constructor
        //-------------------------------------------------
        public sound_manager(running_machine machine)
        {
            m_machine = machine;
            m_update_timer = null;
            m_finalmix_leftover = 0;
            m_finalmix = new std.vector<s16>(machine.sample_rate());
            m_leftmix = new std.vector<s32>(machine.sample_rate());
            m_rightmix = new std.vector<s32>(machine.sample_rate());
            m_nosound_mode = machine.osd().no_sound() ? 1 : 0;
            m_wavfile = null;
            m_update_attoseconds = STREAMS_UPDATE_ATTOTIME.attoseconds();
            m_last_update = attotime.zero;


            // get filename for WAV file or AVI file if specified
            string wavfile = machine.options().wav_write();
            string avifile = machine.options().avi_write();

            // handle -nosound and lower sample rate if not recording WAV or AVI
            if (m_nosound_mode != 0 && string.IsNullOrEmpty(wavfile) && string.IsNullOrEmpty(avifile))
                machine.sample_rate_set(11025);

            // count the mixers
            if (sound_global.VERBOSE)
            {
                mixer_interface_iterator iter = new mixer_interface_iterator(machine.root_device());
                sound_global.VPRINTF("total mixers = {0}\n", iter.count());
            }

            // register callbacks
            machine.configuration().config_register("mixer", config_load, config_save);
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_PAUSE, pause);
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_RESUME, resume);
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_RESET, reset);
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_EXIT, stop_recording);

            // register global states
            machine.save().save_item(m_last_update, "m_last_update");

            // set the starting attenuation
            set_attenuation(machine.options().volume());

            // start the periodic update flushing timer
            m_update_timer = machine.scheduler().timer_alloc(update, this);
            m_update_timer.adjust(STREAMS_UPDATE_ATTOTIME, 0, STREAMS_UPDATE_ATTOTIME);
        }

        //~sound_manager() { }


        // getters
        running_machine machine() { return m_machine; }
        //int attenuation() const { return m_attenuation; }
        public std.vector<sound_stream> streams() { return m_stream_list; }
        public attotime last_update() { return m_last_update; }
        public attoseconds_t update_attoseconds() { return m_update_attoseconds; }


        // stream creation
        //-------------------------------------------------
        //  stream_alloc - allocate a new stream
        //-------------------------------------------------
        public sound_stream stream_alloc(device_t device, int inputs, int outputs, int sample_rate, stream_update_delegate callback = null)
        {
            var stream = new sound_stream(device, inputs, outputs, sample_rate, callback);
            m_stream_list.push_back(stream);
            return stream;
        }


        // global controls

        //-------------------------------------------------
        //  start_recording - begin audio recording
        //-------------------------------------------------
        public void start_recording()
        {
            // open the output WAV file if specified
            string wavfile = machine().options().wav_write();
            if (!string.IsNullOrEmpty(wavfile) && m_wavfile == null)
                m_wavfile = wavwrite_global.wav_open(wavfile, machine().sample_rate(), 2);
        }


        //-------------------------------------------------
        //  stop_recording - end audio recording
        //-------------------------------------------------
        void stop_recording(running_machine machine)
        {
            // close any open WAV file
            if (m_wavfile != null)
                wavwrite_global.wav_close(m_wavfile);
            m_wavfile = null;
        }


        //-------------------------------------------------
        //  set_attenuation - set the global volume
        //-------------------------------------------------
        public void set_attenuation(int attenuation)
        {
            m_attenuation = attenuation;
            machine().osd().set_mastervolume(m_muted != 0 ? -32 : m_attenuation);
        }

        public void ui_mute(bool turn_off = true) { mute(turn_off, MUTE_REASON_UI); }

        //void debugger_mute(bool turn_off = true) { mute(turn_off, MUTE_REASON_DEBUGGER); }
        public void system_mute(bool turn_off = true) { mute(turn_off, MUTE_REASON_SYSTEM); }
        //void system_enable(bool turn_on = true) { mute(!turn_on, MUTE_REASON_SYSTEM); }


        // user gain controls
        //bool indexed_mixer_input(int index, mixer_input &info) const;


        // internal helpers

        //-------------------------------------------------
        //  mute - mute sound output
        //-------------------------------------------------
        void mute(bool mute, u8 reason)
        {
            if (mute)
                m_muted |= reason;
            else
                m_muted &= (byte)~reason;

            set_attenuation(m_attenuation);
        }

        //-------------------------------------------------
        //  reset - reset all sound chips
        //-------------------------------------------------
        void reset(running_machine machine)
        {
            // reset all the sound chips
            foreach (device_sound_interface sound in new sound_interface_iterator(machine.root_device()))
                sound.device().reset();
        }

        //-------------------------------------------------
        //  pause - pause sound output
        //-------------------------------------------------
        void pause(running_machine machine)
        {
            mute(true, MUTE_REASON_PAUSE);
        }

        //-------------------------------------------------
        //  resume - resume sound output
        //-------------------------------------------------
        void resume(running_machine machine)
        {
            mute(false, MUTE_REASON_PAUSE);
        }

        //-------------------------------------------------
        //  config_load - read and apply data from the
        //  configuration file
        //-------------------------------------------------
        void config_load(config_type cfg_type, util.xml.data_node parentnode)
        {
            // we only care about game files
            if (cfg_type != config_type.GAME)
                return;

            // might not have any data
            if (parentnode == null)
                return;

            throw new emu_unimplemented();
#if false
            // iterate over channel nodes
            for (xml_data_node channelnode = xml_get_sibling(parentnode.child, "channel"); channelnode != null; channelnode = xml_get_sibling(channelnode->next, "channel"))
            {
                mixer_input info;
                if (indexed_mixer_input(xml_get_attribute_int(channelnode, "index", -1), info))
                {
                    float defvol = xml_get_attribute_float(channelnode, "defvol", 1.0f);
                    float newvol = xml_get_attribute_float(channelnode, "newvol", -1000.0f);
                    if (newvol != -1000.0f)
                        info.stream.set_user_gain(info.inputnum, newvol / defvol);
                }
            }
#endif
        }

        //-------------------------------------------------
        //  config_save - save data to the configuration
        //  file
        //-------------------------------------------------
        void config_save(config_type cfg_type, util.xml.data_node parentnode)
        {
            // we only care about game files
            if (cfg_type != config_type.GAME)
                return;

            // iterate over mixer channels
            if (parentnode != null)
            {
                throw new emu_unimplemented();
#if false
                for (int mixernum = 0; ; mixernum++)
                {
                    mixer_input info;
                    if (!indexed_mixer_input(mixernum, info))
                        break;

                    float newvol = info.stream.user_gain(info.inputnum);

                    if (newvol != 1.0f)
                    {
                        xml_data_node channelnode = xml_add_child(parentnode, "channel", null);
                        if (channelnode != null)
                        {
                            xml_set_attribute_int(channelnode, "index", mixernum);
                            xml_set_attribute_float(channelnode, "newvol", newvol);
                        }
                    }
                }
#endif
            }
        }

        //-------------------------------------------------
        //  update - mix everything down to its final form
        //  and send it to the OSD layer
        //-------------------------------------------------
        void update(object o = null, s32 param = 0)  // (void *ptr = nullptr, s32 param = 0);
        {
            sound_global.VPRINTF("sound_update\n");

            profiler_global.g_profiler.start(profile_type.PROFILER_SOUND);


            // force all the speaker streams to generate the proper number of samples
            int samples_this_update = 0;
            foreach (speaker_device speaker in new speaker_device_iterator(machine().root_device()))
                speaker.mix(m_leftmix, m_rightmix, ref samples_this_update, (m_muted & MUTE_REASON_SYSTEM) != 0);

            // now downmix the final result
            u32 finalmix_step = (UInt32)machine().video().speed_factor();
            u32 finalmix_offset = 0;
            ListPointer<s16> finalmix = new ListPointer<s16>(m_finalmix);  //s16 *finalmix = &m_finalmix[0];
            int sample;
            for (sample = (int)m_finalmix_leftover; sample < samples_this_update * 1000; sample += (int)finalmix_step)
            {
                int sampindex = sample / 1000;

                // clamp the left side
                int samp = m_leftmix[sampindex];
                if (samp < -32768)
                    samp = -32768;
                else if (samp > 32767)
                    samp = 32767;

                finalmix[finalmix_offset++] = (Int16)samp;

                // clamp the right side
                samp = m_rightmix[sampindex];
                if (samp < -32768)
                    samp = -32768;
                else if (samp > 32767)
                    samp = 32767;

                finalmix[finalmix_offset++] = (Int16)samp;
            }

            m_finalmix_leftover = (UInt32)(sample - samples_this_update * 1000);

            // play the result
            if (finalmix_offset > 0)
            {
                if (m_nosound_mode == 0)
                    machine().osd().update_audio_stream(finalmix, (int)(finalmix_offset / 2));

                machine().osd().add_audio_to_recording(finalmix, (int)finalmix_offset / 2);
                machine().video().add_sound_to_recording(finalmix, (int)(finalmix_offset / 2));

                if (m_wavfile != null)
                    wavwrite_global.wav_add_data_16(m_wavfile, finalmix, (int)finalmix_offset);
            }

            // see if we ticked over to the next second
            attotime curtime = machine().time();
            bool second_tick = false;
            if (curtime.seconds() != m_last_update.seconds())
            {
                assert(curtime.seconds() == m_last_update.seconds() + 1);
                second_tick = true;
            }

            // iterate over all the streams and update them
            foreach (var stream in m_stream_list)
                stream.update_with_accounting(second_tick);

            // remember the update time
            m_last_update = curtime;

            // update sample rates if they have changed
            foreach (var stream in m_stream_list)
                stream.apply_sample_rate_changes();


            profiler_global.g_profiler.stop();
        }
    }
}
