// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using attoseconds_t = System.Int64;  //typedef s64 attoseconds_t;
using mixer_interface_enumerator = mame.device_interface_enumerator<mame.device_mixer_interface>;  //typedef device_interface_enumerator<device_mixer_interface> mixer_interface_enumerator;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using sound_interface_enumerator = mame.device_interface_enumerator<mame.device_sound_interface>;  //typedef device_interface_enumerator<device_sound_interface> sound_interface_enumerator;
using speaker_device_enumerator = mame.device_type_enumerator<mame.speaker_device>;  //using speaker_device_enumerator = device_type_enumerator<speaker_device>;
using stream_buffer_sample_t = System.Single;  //using sample_t = float;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt32;
using System.Diagnostics;

namespace mame
{
    /***************************************************************************

        sound.h

        Core sound interface functions and definitions.

    ****************************************************************************

        In MAME, sound is represented as a graph of sound "streams". Each
        stream has a fixed number of inputs and outputs, and is responsible
        for producing sound on demand.

        The graph is driven from the outputs, which are speaker devices.
        These devices are updated on a regular basis (~50 times per second),
        and when an update occurs, the graph is walked from the speaker
        through each input, until all connected streams are up to date.

        Individual streams can also be updated manually. This is important
        for sound chips and CPU-driven devices, who should force any
        affected streams to update prior to making changes.

        Sound streams are *not* part of the device execution model. This is
        very important to understand. If the process of producing the ouput
        stream affects state that might be consumed by an executing device
        (e.g., a CPU), then care must be taken to ensure that the stream is
        updated frequently enough

        The model for timing sound samples is very important and explained
        here. Each stream source has a clock (aka sample rate). Each clock
        edge represents a sample that is held for the duration of one clock
        period. This model has interesting effects:

        For example, if you have a 10Hz clock, and call stream.update() at
        t=0.91, it will compute 10 samples (for clock edges 0.0, 0.1, 0.2,
        ..., 0.7, 0.8, and 0.9). And then if you ask the stream what its
        current end time is (via stream.sample_time()), it will say t=1.0,
        which is in the future, because it knows it will hold that last
        sample until 1.0s.

        Sound generation callbacks are presented with a std::vector of inputs
        and outputs. The vectors contain objects of read_stream_view and
        write_stream_view respectively, which wrap access to a circular buffer
        of samples. Sound generation callbacks are expected to fill all the
        samples described by the outputs' write_stream_view objects. At the
        moment, all outputs have the same sample rate, so the number of samples
        that need to be generated will be consistent across all outputs.

        By default, the inputs will have been resampled to match the output
        sample rate, unless otherwise specified.

    ***************************************************************************/

    public static class sound_global
    {
        //**************************************************************************
        //  CONSTANTS
        //**************************************************************************

        // special sample-rate values
        public const u32 SAMPLE_RATE_INVALID = 0xffffffff;
        public const u32 SAMPLE_RATE_INPUT_ADAPTIVE = 0xfffffffe;
        public const u32 SAMPLE_RATE_OUTPUT_ADAPTIVE = 0xfffffffd;

        // anything below this sample rate is effectively treated as "off"
        public const u32 SAMPLE_RATE_MINIMUM = 50;


        //**************************************************************************
        //  DEBUGGING
        //**************************************************************************

        // turn this on to enable aggressive assertions and other checks
        //#ifdef MAME_DEBUG
        //#define SOUND_DEBUG (1)
        //#else
        //#define SOUND_DEBUG (0)
        //#endif

        // if SOUND_DEBUG is on, make assertions fire regardless of MAME_DEBUG
        //#if (SOUND_DEBUG)
        //#define sound_assert(x) do { if (!(x)) { osd_printf_error("sound_assert: " #x "\n"); osd_break_into_debugger("sound_assert: " #x "\n"); } } while (0)
        //#else
        //#define sound_assert assert
        //#endif
        [Conditional("DEBUG")] public static void sound_assert(bool condition) { emucore_global.assert(condition, "sound_assert\n"); }


        const bool VERBOSE = false;  //#define VERBOSE         (0)

        public static void VPRINTF(string x, params object [] args) { if (VERBOSE) g.osd_printf_debug(x, args); } //#define VPRINTF(x)      do { if (VERBOSE) osd_printf_debug x; } while (0)

        //#define LOG_OUTPUT_WAV  (0)
    }


    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> stream_buffer
    public class stream_buffer
    {
        // stream_buffer is an internal class, not directly accessed
        // outside of the classes below
        //friend class read_stream_view;
        //friend class write_stream_view;
        //friend class sound_stream;
        //friend class sound_stream_output;

        // the one public bit is the sample type
        //using sample_t = float;


        // internal state
        u32 m_end_second;                     // current full second of the buffer end
        u32 m_end_sample;                     // current sample number within the final second
        u32 m_sample_rate;                    // sample rate of the data in the buffer
        attoseconds_t m_sample_attos;         // pre-computed attoseconds per sample
        std.vector<stream_buffer_sample_t> m_buffer;       // vector of actual buffer data


        // constructor/destructor
        public stream_buffer(u32 sample_rate = 48000)
        {
            m_end_second = 0;
            m_end_sample = 0;
            m_sample_rate = sample_rate;
            m_sample_attos = (sample_rate == 0) ? attotime.ATTOSECONDS_PER_SECOND : ((attotime.ATTOSECONDS_PER_SECOND + sample_rate - 1) / sample_rate);
            m_buffer = new std.vector<stream_buffer_sample_t>(sample_rate);
        }

        //~stream_buffer();


        // disable copying of stream_buffers directly
        //stream_buffer(stream_buffer const &src) = delete;
        //stream_buffer &operator=(stream_buffer const &rhs) = delete;


        // return the current sample rate
        public u32 sample_rate() { return m_sample_rate; }


        // set a new sample rate
        //-------------------------------------------------
        //  set_sample_rate - set a new sample rate for
        //  this buffer
        //-------------------------------------------------
        public void set_sample_rate(u32 rate, bool resample)
        {
            // skip if nothing is actually changing
            if (rate == m_sample_rate)
                return;

            // force resampling off if coming to or from an invalid rate, or if we're at time 0 (startup)
            sound_global.sound_assert(rate >= sound_global.SAMPLE_RATE_MINIMUM - 1);

            if (rate < sound_global.SAMPLE_RATE_MINIMUM || m_sample_rate < sound_global.SAMPLE_RATE_MINIMUM || (m_end_second == 0 && m_end_sample == 0))
                resample = false;

            // note the time and period of the current buffer (end_time is AFTER the final sample)
            attotime prevperiod = sample_period();
            attotime prevend = end_time();

            // compute the time and period of the new buffer
            attotime newperiod = new attotime(0, (attotime.ATTOSECONDS_PER_SECOND + rate - 1) / rate);
            attotime newend = new attotime(prevend.seconds(), (prevend.attoseconds() / newperiod.attoseconds()) * newperiod.attoseconds());

            // buffer a short runway of previous samples; in order to support smooth
            // sample rate changes (needed by, e.g., Q*Bert's Votrax), we buffer a few
            // samples at the previous rate, and then reconstitute them resampled
            // (via simple point sampling) at the new rate. The litmus test is the
            // voice when jumping off the edge in Q*Bert; without this extra effort
            // it is crackly and/or glitchy at times
            stream_buffer_sample_t [] buffer = new stream_buffer_sample_t [64];
            int buffered_samples = (int)std.min(m_sample_rate, std.min(rate, (u32)std.size(buffer)));

            // if the new rate is lower, downsample into our holding buffer;
            // otherwise just copy into our holding buffer for later upsampling
            bool new_rate_higher = (rate > m_sample_rate);
            if (resample)
            {
                if (!new_rate_higher)
                {
                    backfill_downsample(buffer, buffered_samples, newend, newperiod);
                }
                else
                {
                    u32 end = m_end_sample;
                    for (int index = 0; index < buffered_samples; index++)
                    {
                        end = prev_index(end);
#if (SOUND_DEBUG)
                        // multiple resamples can occur before clearing out old NaNs so
                        // neuter them for this specific case
                        if (std::isnan(m_buffer[end]))
                            buffer[index] = 0;
                        else
#endif
                            buffer[index] = get((s32)end);
                    }
                }
            }

            // ensure our buffer is large enough to hold a full second at the new rate
            if (m_buffer.size() < rate)
                m_buffer.resize(rate);

            // set the new rate
            m_sample_rate = rate;
            m_sample_attos = newperiod.attoseconds();

            // compute the new end sample index based on the buffer time
            m_end_sample = time_to_buffer_index(prevend, false, true);

            // if the new rate is higher, upsample from our temporary buffer;
            // otherwise just copy our previously-downsampled data
            if (resample)
            {
#if (SOUND_DEBUG)
                // for aggressive debugging, fill the buffer with NANs to catch anyone
                // reading beyond what we resample below
                fill(NAN);
#endif

                if (new_rate_higher)
                {
                    backfill_upsample(buffer, buffered_samples, prevend, prevperiod);
                }
                else
                {
                    u32 end = m_end_sample;
                    for (int index = 0; index < buffered_samples; index++)
                    {
                        end = prev_index(end);
                        put((s32)end, buffer[index]);
                    }
                }
            }

            // if not resampling, clear the buffer
            else
            {
                fill(0);
            }
        }


        // return the current sample period in attoseconds
        public attoseconds_t sample_period_attoseconds() { return m_sample_attos; }
        public attotime sample_period() { return new attotime(0, m_sample_attos); }

        // return the attotime of the current end of buffer
        public attotime end_time() { return index_time((s32)m_end_sample); }


        // set the ending time (for forced resyncs; generally not used)
        public void set_end_time(attotime time)
        {
            m_end_second = (u32)time.seconds();
            m_end_sample = (u32)(time.attoseconds() / m_sample_attos);
        }


        // return the effective buffer size; currently it is a full second of audio
        // at the current sample rate, but this maybe change in the future
        public u32 size() { return m_sample_rate; }


        // read the sample at the given index (clamped); should be valid in all cases
        public stream_buffer_sample_t get(s32 index)
        {
            sound_global.sound_assert((u32)index < size());

            stream_buffer_sample_t value = m_buffer[index];

            sound_global.sound_assert(!std.isnan(value));

            return value;
        }


        // write the sample at the given index (clamped)
        public void put(s32 index, stream_buffer_sample_t data)
        {
            sound_global.sound_assert((u32)index < size());

            m_buffer[index] = data;
        }


        // simple helpers to step indexes
        public u32 next_index(u32 index) { index++; return (index == size()) ? 0 : index; }
        u32 prev_index(u32 index) { return (index == 0) ? (size() - 1) : (index - 1); }


        // clamp an index to the size of the buffer; allows for indexing +/- one
        // buffers' worth of range
        u32 clamp_index(s32 index)
        {
            if (index < 0)
                index += (s32)size();
            else if (index >= size())
                index -= (s32)size();

            sound_global.sound_assert(index >= 0 && index < size());

            return (u32)index;
        }


        // fill the buffer with the given value
        void fill(stream_buffer_sample_t value) { std.fill_n(m_buffer, m_buffer.size(), value); }  //{ std::fill_n(&m_buffer[0], m_buffer.size(), value); }


        // return the attotime of a given index within the buffer
        //-------------------------------------------------
        //  index_time - return the attotime of a given
        //  index within the buffer
        //-------------------------------------------------
        public attotime index_time(s32 index)
        {
            index = (s32)clamp_index(index);
            return new attotime((s32)m_end_second - ((index > m_end_sample) ? 1 : 0), index * m_sample_attos);
        }


        // given an attotime, return the buffer index corresponding to it
        //-------------------------------------------------
        //  time_to_buffer_index - given an attotime,
        //  return the buffer index corresponding to it
        //-------------------------------------------------
        public u32 time_to_buffer_index(attotime time, bool round_up, bool allow_expansion = false)
        {
            // compute the sample index within the second
            int sample = (int)((time.attoseconds() + (round_up ? (m_sample_attos - 1) : 0)) / m_sample_attos);

            sound_global.sound_assert(sample >= 0 && sample <= size());

            // if the time is past the current end, make it the end
            if (time.seconds() > m_end_second || (time.seconds() == m_end_second && sample > m_end_sample))
            {
                sound_global.sound_assert(allow_expansion);

                m_end_sample = (u32)sample;
                m_end_second = (u32)time.m_seconds;

                // due to round_up, we could tweak over the line into the next second
                if (sample >= size())
                {
                    m_end_sample -= size();
                    m_end_second++;
                }
            }

            // if the time is before the start, fail
            if (time.seconds() + 1 < m_end_second || (time.seconds() + 1 == m_end_second && sample < m_end_sample))
                throw new emu_fatalerror("Attempt to create an out-of-bounds view");

            return clamp_index(sample);
        }


        // downsample from our buffer into a temporary buffer
        //-------------------------------------------------
        //  backfill_downsample - this is called BEFORE
        //  the sample rate change to downsample from the
        //  end of the current buffer into a temporary
        //  holding location
        //-------------------------------------------------
        void backfill_downsample(stream_buffer_sample_t [] dest, int samples, attotime newend, attotime newperiod)
        {
            // compute the time of the first sample to be backfilled; start one period before
            attotime time = newend - newperiod;

            // loop until we run out of buffered data
            int dstindex;
            for (dstindex = 0; dstindex < samples && time.seconds() >= 0; dstindex++)
            {
                u32 srcindex = time_to_buffer_index(time, false);

#if (SOUND_DEBUG)
                // multiple resamples can occur before clearing out old NaNs so
                // neuter them for this specific case
                if (std::isnan(m_buffer[srcindex]))
                    dest[dstindex] = 0;
                else
#endif
                    dest[dstindex] = get((s32)srcindex);

                time -= newperiod;
            }

            for ( ; dstindex < samples; dstindex++)
                dest[dstindex] = 0;
        }


        // upsample from a temporary buffer into our buffer
        //-------------------------------------------------
        //  backfill_upsample - this is called AFTER the
        //  sample rate change to take a copied buffer
        //  of samples at the old rate and upsample them
        //  to the new (current) rate
        //-------------------------------------------------
        void backfill_upsample(stream_buffer_sample_t [] src, int samples, attotime prevend, attotime prevperiod)
        {
            // compute the time of the first sample to be backfilled; start one period before
            attotime time = end_time() - sample_period();

            // also adjust the buffered sample end time to point to the sample time of the
            // final sample captured
            prevend -= prevperiod;

            // loop until we run out of buffered data
            u32 end = m_end_sample;
            int srcindex = 0;
            while (true)
            {
                // if our backfill time is before the current buffered sample time,
                // back up until we have a sample that covers this time
                while (time < prevend && srcindex < samples)
                {
                    prevend -= prevperiod;
                    srcindex++;
                }

                // stop when we run out of source
                if (srcindex >= samples)
                    break;

                // write this sample at the pevious position
                end = prev_index(end);
                put((s32)end, src[srcindex]);

                // back up to the next sample time
                time -= sample_period();
            }
        }


#if SOUND_DEBUG
        // for debugging, provide an interface to write a WAV stream
        void open_wav(char const *filename);
        void flush_wav();

        // internal debugging state
        util::wav_file_ptr m_wav_file;        // pointer to the current WAV file
        u32 m_last_written = 0;               // last written sample index
#endif
    }


    // ======================> read_stream_view
    public class read_stream_view
    {
        //using sample_t = stream_buffer::sample_t;


        // internal state
        protected stream_buffer m_buffer;              // pointer to the stream buffer we're viewing  //stream_buffer *m_buffer;              // pointer to the stream buffer we're viewing
        s32 m_end;                            // ending sample index (always >= start)
        protected s32 m_start;                          // starting sample index
        stream_buffer_sample_t m_gain;                      // overall gain factor


        // private constructor used by write_stream_view that allows for expansion
        public read_stream_view(stream_buffer buffer, attotime start, attotime end) 
            : this(buffer, 0, (s32)buffer.time_to_buffer_index(end, true, true), (stream_buffer_sample_t)1.0)  //read_stream_view(&buffer, 0, buffer.time_to_buffer_index(end, true, true), 1.0)
        {
            // start has to be set after end, since end can expand the buffer and
            // potentially invalidate start
            m_start = (s32)buffer.time_to_buffer_index(start, false);
            normalize_start_end();
        }


        // base constructor to simplify some of the code
        public read_stream_view(stream_buffer buffer, s32 start, s32 end, stream_buffer_sample_t gain)
        {
            m_buffer = buffer;
            m_end = end;
            m_start = start;
            m_gain = gain;


            normalize_start_end();
        }


        // empty constructor so we can live in an array or vector
        public read_stream_view()
            : this(null, 0, 0, 1)
        {
        }


        // constructor that covers the given time period
        public read_stream_view(stream_buffer buffer, attotime start, attotime end, stream_buffer_sample_t gain)
            : this(buffer, (s32)buffer.time_to_buffer_index(start, false), (s32)buffer.time_to_buffer_index(end, true), gain)  //: read_stream_view(&buffer, buffer.time_to_buffer_index(start, false), buffer.time_to_buffer_index(end, true), gain)
        {
        }


        // copy constructor
        public read_stream_view(read_stream_view src)
            : this(src.m_buffer, src.m_start, src.m_end, src.m_gain)
        {
        }

        // copy constructor that sets a different start time
        public read_stream_view(read_stream_view src, attotime start)
            : this(src.m_buffer, (s32)src.m_buffer.time_to_buffer_index(start, false), src.m_end, src.m_gain)
        {
        }


        // copy assignment
        //read_stream_view &operator=(read_stream_view const &rhs)
        //{
        //    m_buffer = rhs.m_buffer;
        //    m_start = rhs.m_start;
        //    m_end = rhs.m_end;
        //    m_gain = rhs.m_gain;
        //    normalize_start_end();
        //    return *this;
        //}


        // return the local gain
        public stream_buffer_sample_t gain() { return m_gain; }

        // return the sample rate of the data
        public u32 sample_rate() { return m_buffer.sample_rate(); }

        // return the sample period (in attoseconds) of the data
        public attoseconds_t sample_period_attoseconds() { return m_buffer.sample_period_attoseconds(); }
        public attotime sample_period() { return m_buffer.sample_period(); }

        // return the number of samples represented by the buffer
        public u32 samples() { return (u32)(m_end - m_start); }

        // return the starting or ending time of the buffer
        public attotime start_time() { return m_buffer.index_time(m_start); }
        public attotime end_time() { return m_buffer.index_time(m_end); }

        // set the gain
        //read_stream_view &set_gain(float gain) { m_gain = gain; return *this; }

        // apply an additional gain factor
        public read_stream_view apply_gain(float gain) { m_gain *= gain; return this; }

        // safely fetch a gain-scaled sample from the buffer
        public stream_buffer_sample_t get(s32 index)
        {
            sound_global.sound_assert((u32)index < samples());

            index += m_start;
            if (index >= m_buffer.size())
                index -= (s32)m_buffer.size();
            return m_buffer.get(index) * m_gain;
        }

        // safely fetch a raw sample from the buffer; if you use this, you need to
        // apply the gain yourself for correctness
        //sample_t getraw(s32 index) const
        //{
        //    sound_assert(u32(index) < samples());
        //    index += m_start;
        //    if (index >= m_buffer->size())
        //        index -= m_buffer->size();
        //    return m_buffer->get(index);
        //}

        // normalize start/end
        void normalize_start_end()
        {
            // ensure that end is always greater than start; we'll
            // wrap to the buffer length as needed
            if (m_end < m_start && m_buffer != null)
                m_end += (s32)m_buffer.size();

            sound_global.sound_assert(m_end >= m_start);
        }
    }


    // ======================> write_stream_view
    public class write_stream_view : read_stream_view
    {
        // empty constructor so we can live in an array or vector
        public write_stream_view()
        {
        }


        // constructor that covers the given time period
        public write_stream_view(stream_buffer buffer, attotime start, attotime end)
            : base(buffer, start, end)
        {
        }


        // constructor that converts from a read_stream_view
        public write_stream_view(read_stream_view src)
            : base(src)
        {
        }


        // safely write a sample to the buffer
        public void put(s32 start, stream_buffer_sample_t sample)
        {
            m_buffer.put((s32)index_to_buffer_index(start), sample);
        }


        // write a sample to the buffer, clamping to +/- the clamp value
        //void put_clamp(s32 index, sample_t sample, sample_t clamp = 1.0)
        //{
        //    assert(clamp >= sample_t(0));
        //    put(index, std::clamp(sample, -clamp, clamp));
        //}


        // write a sample to the buffer, converting from an integer with the given maximum
        public void put_int(s32 index, s32 sample, s32 max)
        {
            put(index, (stream_buffer_sample_t)sample * (1.0f / (stream_buffer_sample_t)max));
        }


        // write a sample to the buffer, converting from an integer with the given maximum
        public void put_int_clamp(s32 index, s32 sample, s32 maxclamp)
        {
            g.assert(maxclamp >= 0);
            put_int(index, std.clamp(sample, -maxclamp, maxclamp), maxclamp);
        }

        // safely add a sample to the buffer
        void add(s32 start, stream_buffer_sample_t sample)
        {
            u32 index = index_to_buffer_index(start);
            m_buffer.put((s32)index, m_buffer.get((s32)index) + sample);
        }


        // add a sample to the buffer, converting from an integer with the given maximum
        public void add_int(s32 index, s32 sample, s32 max)
        {
            add(index, (stream_buffer_sample_t)sample * (1.0f / (stream_buffer_sample_t)max));
        }


        // fill part of the view with the given value
        public void fill(stream_buffer_sample_t value, s32 start, s32 count)
        {
            if (start + count > samples())
                count = (s32)samples() - start;
            u32 index = index_to_buffer_index(start);
            for (s32 sampindex = 0; sampindex < count; sampindex++)
            {
                m_buffer.put((s32)index, value);
                index = m_buffer.next_index(index);
            }
        }


        public void fill(stream_buffer_sample_t value, s32 start) { fill(value, start, (s32)samples() - start); }
        public void fill(stream_buffer_sample_t value) { fill(value, 0, (s32)samples()); }


        // copy data from another view
        public void copy(read_stream_view src, s32 start, s32 count)
        {
            if (start + count > samples())
                count = (s32)samples() - start;
            u32 index = index_to_buffer_index(start);
            for (s32 sampindex = 0; sampindex < count; sampindex++)
            {
                m_buffer.put((s32)index, src.get(start + sampindex));
                index = m_buffer.next_index(index);
            }
        }

        public void copy(read_stream_view src, s32 start) { copy(src, start, (s32)samples() - start); }
        public void copy(read_stream_view src) { copy(src, 0, (s32)samples()); }


        // add data from another view to our current values
        public void add(read_stream_view src, s32 start, s32 count)
        {
            if (start + count > samples())
                count = (s32)samples() - start;
            u32 index = index_to_buffer_index(start);
            for (s32 sampindex = 0; sampindex < count; sampindex++)
            {
                m_buffer.put((s32)index, m_buffer.get((s32)index) + src.get(start + sampindex));
                index = m_buffer.next_index(index);
            }
        }

        public void add(read_stream_view src, s32 start) { add(src, start, (s32)samples() - start); }
        public void add(read_stream_view src) { add(src, 0, (s32)samples()); }


        // given a stream starting offset, return the buffer index
        u32 index_to_buffer_index(s32 start)
        {
            sound_global.sound_assert((u32)start < samples());

            u32 index = (u32)(start + m_start);
            if (index >= m_buffer.size())
                index -= m_buffer.size();
            return index;
        }
    }


    // ======================> sound_stream_output
    public class sound_stream_output
    {
#if SOUND_DEBUG
        friend class sound_stream;
#endif


        // internal state
        sound_stream m_stream;               // owning stream  //sound_stream *m_stream;               // owning stream
        stream_buffer m_buffer = new stream_buffer();               // output buffer
        u32 m_index;                          // output index within the stream
        stream_buffer_sample_t m_gain;       // gain to apply to the output
        std.vector<sound_stream_output> m_resampler_list = new std.vector<sound_stream_output>(); // list of resamplers we're connected to


        // construction/destruction
        public sound_stream_output()
        {
            m_stream = null;
            m_index = 0;
            m_gain = (stream_buffer_sample_t)1.0;
        }


        // initialization
        public void init(sound_stream stream, u32 index, string tag)
        {
            // set the passed-in data
            m_stream = stream;
            m_index = index;

            // save our state
            var save = stream.device().machine().save();
            save.save_item(stream.device(), "stream.output", tag, (int)index, g.NAME(new { m_gain }));

#if (LOG_OUTPUT_WAV)
            std::string filename = stream.device().machine().basename();
            filename += stream.device().tag();
            for (int index = 0; index < filename.size(); index++)
                if (filename[index] == ':')
                    filename[index] = '_';
            if (dynamic_cast<default_resampler_stream *>(&stream) != nullptr)
                filename += "_resampler";
            filename += "_OUT_";
            char buf[10];
            sprintf(buf, "%d", index);
            filename += buf;
            filename += ".wav";
            m_buffer.open_wav(filename.c_str());
#endif
        }


        // no copying allowed
        //sound_stream_output(sound_stream_output const &src) = delete;
        //sound_stream_output &operator=(sound_stream_output const &rhs) = delete;


        // simple getters
        public sound_stream stream()
        {
            sound_global.sound_assert(m_stream != null);

            return m_stream;
        }

        public attotime end_time() { return m_buffer.end_time(); }
        public u32 index() { return m_index; }
        public stream_buffer_sample_t gain() { return m_gain; }
        u32 buffer_sample_rate() { return m_buffer.sample_rate(); }

        // simple setters
        public void set_gain(float gain) { m_gain = gain; }

        // return a friendly name
        //std::string name() const;

        // handle a changing sample rate
        public void sample_rate_changed(u32 rate) { m_buffer.set_sample_rate(rate, true); }

        // return an output view covering a time period
        public write_stream_view view(attotime start, attotime end) { return new write_stream_view(m_buffer, start, end); }

        // resync the buffer to the given end time
        public void set_end_time(attotime end) { m_buffer.set_end_time(end); }


        // attempt to optimize resamplers by reusing them where possible
        //-------------------------------------------------
        //  optimize_resampler - optimize resamplers by
        //  either returning the native rate or another
        //  input's resampler if they can be reused
        //-------------------------------------------------
        public sound_stream_output optimize_resampler(sound_stream_output input_resampler)
        {
            // if no resampler, or if the resampler rate matches our rate, return ourself
            if (input_resampler == null || buffer_sample_rate() == input_resampler.buffer_sample_rate())
                return this;

            // scan our list of resamplers to see if there's another match
            foreach (var resampler in m_resampler_list)
            {
                if (resampler.buffer_sample_rate() == input_resampler.buffer_sample_rate())
                    return resampler;
            }

            // add the input to our list and return the one we were given back
            m_resampler_list.push_back(input_resampler);
            return input_resampler;
        }
    }


    // ======================> sound_stream_input
    public class sound_stream_input
    {
#if SOUND_DEBUG
        friend class sound_stream;
#endif


        // internal state
        sound_stream m_owner;                   // pointer to the owning stream  //sound_stream *m_owner;                   // pointer to the owning stream
        sound_stream_output m_native_source;    // pointer to the native sound_stream_output  //sound_stream_output *m_native_source;    // pointer to the native sound_stream_output
        sound_stream_output m_resampler_source; // pointer to the resampler output  //sound_stream_output *m_resampler_source; // pointer to the resampler output
        u32 m_index;                             // input index within the stream
        stream_buffer_sample_t m_gain;          // gain to apply to this input
        stream_buffer_sample_t m_user_gain;     // user-controlled gain to apply to this input


        // construction/destruction
        public sound_stream_input()
        {
            m_owner = null;
            m_native_source = null;
            m_resampler_source = null;
            m_index = 0;
            m_gain = (stream_buffer_sample_t)1.0;
            m_user_gain = (stream_buffer_sample_t)1.0;
        }


        // initialization
        //-------------------------------------------------
        //  init - initialization
        //-------------------------------------------------
        public void init(sound_stream stream, u32 index, string tag, sound_stream_output resampler)
        {
            // set the passed-in values
            m_owner = stream;
            m_index = index;
            m_resampler_source = resampler;

            // save our state
            var save = stream.device().machine().save();
            save.save_item(stream.device(), "stream.input", tag, (int)index, g.NAME(new { m_gain }));
            save.save_item(stream.device(), "stream.input", tag, (int)index, g.NAME(new { m_user_gain }));
        }


        // no copying allowed
        //sound_stream_input(sound_stream_input const &src) = delete;
        //sound_stream_input &operator=(sound_stream_input const &rhs) = delete;


        // simple getters
        public bool valid() { return m_native_source != null; }
        //sound_stream &owner() const { sound_assert(valid()); return *m_owner; }
        public sound_stream_output source()
        {
            sound_global.sound_assert(valid());

            return m_native_source;
        }
        //u32 index() const { return m_index; }
        //stream_buffer::sample_t gain() const { return m_gain; }
        public stream_buffer_sample_t user_gain() { return m_user_gain; }

        // simple setters
        public void set_gain(float gain) { m_gain = gain; }
        public void set_user_gain(float gain) { m_user_gain = gain; }

        // return a friendly name
        //std::string name() const;

        // connect the source
        //-------------------------------------------------
        //  set_source - wire up the output source for
        //  our consumption
        //-------------------------------------------------
        public void set_source(sound_stream_output source)
        {
            m_native_source = source;
            if (m_resampler_source != null)
                m_resampler_source.stream().set_input(0, source.stream(), (int)source.index());
        }


        // update and return an reading view
        //-------------------------------------------------
        //  update - update our source's stream to the
        //  current end time and return a view to its
        //  contents
        //-------------------------------------------------
        public read_stream_view update(attotime start, attotime end)
        {
            // shouldn't get here unless valid
            sound_global.sound_assert(valid());

            // pick an optimized resampler
            sound_stream_output source = m_native_source.optimize_resampler(m_resampler_source);

            // if not using our own resampler, keep it up to date in case we need to invoke it later
            if (m_resampler_source != null && source != m_resampler_source)
                m_resampler_source.set_end_time(end);

            // update the source, returning a view of the needed output over the start and end times
            return source.stream().update_view(start, end, source.index()).apply_gain(m_gain * m_user_gain * source.gain());
        }


        // tell inputs to apply sample rate changes
        //-------------------------------------------------
        //  apply_sample_rate_changes - tell our sources
        //  to apply any sample rate changes, informing
        //  them of our current rate
        //-------------------------------------------------
        public void apply_sample_rate_changes(u32 updatenum, u32 downstream_rate)
        {
            // shouldn't get here unless valid
            sound_global.sound_assert(valid());

            // if we have a resampler, tell it (and it will tell the native source)
            if (m_resampler_source != null)
                m_resampler_source.stream().apply_sample_rate_changes(updatenum, downstream_rate);

            // otherwise, just tell the native source directly
            else
                m_native_source.stream().apply_sample_rate_changes(updatenum, downstream_rate);
        }
    }


    // ======================> stream_update_delegate

    // new-style callback
    public delegate void stream_update_delegate(sound_stream stream, std.vector<read_stream_view> read_views, std.vector<write_stream_view> write_views);  //using stream_update_delegate = delegate<void (sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs)>;


    // ======================> sound_stream_flags
    public enum sound_stream_flags : u32
    {
        // default is no special flags
        STREAM_DEFAULT_FLAGS = 0x00,

        // specify that updates should be forced to one sample at a time, in real time
        // this implicitly creates a timer that runs at the stream's output frequency
        // so only use when strictly necessary
        STREAM_SYNCHRONOUS = 0x01,

        // specify that input streams should not be resampled; stream update handler
        // must be able to accommodate multiple strams of differing input rates
        STREAM_DISABLE_INPUT_RESAMPLING = 0x02
    }


    // ======================> sound_stream
    public class sound_stream
    {
        //friend class sound_manager;


        // protected state
        protected string m_name;                            // name of this stream


        // linking information
        device_t m_device;                            // owning device  //device_t &m_device;                            // owning device
        sound_stream m_next;                          // next stream in the chain  //sound_stream *m_next;                          // next stream in the chain

        // general information
        u32 m_sample_rate;                             // current live sample rate
        u32 m_pending_sample_rate;                     // pending sample rate for dynamic changes
        u32 m_last_sample_rate_update;                 // update number of last sample rate change
        bool m_input_adaptive;                         // adaptive stream that runs at the sample rate of its input
        bool m_output_adaptive;                        // adaptive stream that runs at the sample rate of its output
        bool m_synchronous;                            // synchronous stream that runs at the rate of its input
        bool m_resampling_disabled;                    // is resampling of input streams disabled?
        emu_timer m_sync_timer;                       // update timer for synchronous streams  //emu_timer *m_sync_timer;                       // update timer for synchronous streams

        // input information
        std.vector<sound_stream_input> m_input;       // list of streams we directly depend upon
        std.vector<read_stream_view> m_input_view;    // array of output views for passing to the callback
        std.vector<sound_stream> m_resampler_list = new std.vector<sound_stream>(); // internal list of resamplers
        stream_buffer m_empty_buffer;                  // empty buffer for invalid inputs

        // output information
        u32 m_output_base;                             // base index of our outputs, relative to our device
        std.vector<sound_stream_output> m_output;     // list of streams which directly depend upon us
        std.vector<write_stream_view> m_output_view;  // array of output views for passing to the callback

        // callback information
        stream_update_delegate m_callback_ex;       // extended callback function


        // private common constructopr
        public sound_stream(device_t device, u32 inputs, u32 outputs, u32 output_base, u32 sample_rate, sound_stream_flags flags)
        {
            m_device = device;
            m_next = null;
            m_sample_rate = (sample_rate < sound_global.SAMPLE_RATE_MINIMUM) ? (sound_global.SAMPLE_RATE_MINIMUM - 1) : (sample_rate < sound_global.SAMPLE_RATE_OUTPUT_ADAPTIVE) ? sample_rate : 48000;
            m_pending_sample_rate = sound_global.SAMPLE_RATE_INVALID;
            m_last_sample_rate_update = 0;
            m_input_adaptive = sample_rate == sound_global.SAMPLE_RATE_INPUT_ADAPTIVE;
            m_output_adaptive = sample_rate == sound_global.SAMPLE_RATE_OUTPUT_ADAPTIVE;
            m_synchronous = (flags & sound_stream_flags.STREAM_SYNCHRONOUS) != 0;
            m_resampling_disabled = (flags & sound_stream_flags.STREAM_DISABLE_INPUT_RESAMPLING) != 0;
            m_sync_timer = null;
            m_input = new std.vector<sound_stream_input>(inputs);  m_input.Fill(() => { return new sound_stream_input(); });
            m_input_view = new std.vector<read_stream_view>(inputs);
            m_empty_buffer = new stream_buffer(100);
            m_output_base = output_base;
            m_output = new std.vector<sound_stream_output>(outputs);  m_output.Fill(() => { return new sound_stream_output(); });
            m_output_view = new std.vector<write_stream_view>(outputs);


            sound_global.sound_assert(outputs > 0);

            // create a name
            m_name = m_device.name();
            m_name += " '";
            m_name += m_device.tag();
            m_name += "'";

            // create a unique tag for saving
            string state_tag = util.string_format("{0}", m_device.machine().sound().unique_id());
            var save = m_device.machine().save();

            //throw new emu_unimplemented();
#if false
            save.save_item(m_device, "stream.sample_rate", state_tag.c_str(), 0, NAME(m_sample_rate));
#endif

            save.register_postload(postload);

            // initialize all inputs
            for (unsigned inputnum = 0; inputnum < m_input.size(); inputnum++)
            {
                // allocate a resampler stream if needed, and get a pointer to its output
                sound_stream_output resampler = null;
                if (!m_resampling_disabled)
                {
                    m_resampler_list.push_back(new default_resampler_stream(m_device));
                    resampler = m_resampler_list.back().m_output[0];
                }

                // add the new input
                m_input[inputnum].init(this, inputnum, state_tag, resampler);
            }

            // initialize all outputs
            for (unsigned outputnum = 0; outputnum < m_output.size(); outputnum++)
                m_output[outputnum].init(this, outputnum, state_tag);

            // create an update timer for synchronous streams
            if (synchronous())
                m_sync_timer = m_device.machine().scheduler().timer_alloc(sync_update);

            // force an update to the sample rates
            sample_rate_changed();
        }


        public sound_stream(device_t device, u32 inputs, u32 outputs, u32 output_base, u32 sample_rate, stream_update_delegate callback, sound_stream_flags flags = sound_stream_flags.STREAM_DEFAULT_FLAGS)  //sound_stream(device_t &device, u32 inputs, u32 outputs, u32 output_base, u32 sample_rate, stream_update_delegate callback, sound_stream_flags flags = STREAM_DEFAULT_FLAGS);
            : this(device, inputs, outputs, output_base, sample_rate, flags)
        {
            m_callback_ex = callback;
        }


        public void sound_stream_after_ctor(stream_update_delegate callback)
        {
            m_callback_ex = callback;
        }


        //virtual ~sound_stream();


        // simple getters
        //sound_stream *next() const { return m_next; }
        public device_t device() { return m_device; }
        //std::string name() const { return m_name; }
        bool input_adaptive() { return m_input_adaptive || m_synchronous; }
        bool output_adaptive() { return m_output_adaptive; }
        bool synchronous() { return m_synchronous; }
        //bool resampling_disabled() const { return m_resampling_disabled; }

        // input and output getters
        public u32 input_count() { return (u32)m_input.size(); }
        public u32 output_count() { return (u32)m_output.size(); }
        //u32 output_base() const { return m_output_base; }
        public sound_stream_input input(int index)
        {
            sound_global.sound_assert(index >= 0 && index < (int)m_input.size());

            return m_input[index];
        }
        public sound_stream_output output(int index)
        {
            sound_global.sound_assert(index >= 0 && index < (int)m_output.size());

            return m_output[index];
        }

        // sample rate and timing getters
        public u32 sample_rate() { return (m_pending_sample_rate != sound_global.SAMPLE_RATE_INVALID) ? m_pending_sample_rate : m_sample_rate; }
        //attotime sample_time() const { return m_output[0].end_time(); }
        //attotime sample_period() const { return attotime(0, sample_period_attoseconds()); }
        //attoseconds_t sample_period_attoseconds() const { return (m_sample_rate != SAMPLE_RATE_INVALID) ? HZ_TO_ATTOSECONDS(m_sample_rate) : ATTOSECONDS_PER_SECOND; }


        // set the sample rate of the stream; will kick in at the next global update
        //-------------------------------------------------
        //  set_sample_rate - set the sample rate on a
        //  given stream
        //-------------------------------------------------
        public void set_sample_rate(u32 new_rate)
        {
            // we will update this on the next global update
            if (new_rate != sample_rate())
                m_pending_sample_rate = new_rate;
        }


        // connect the output 'outputnum' of given input_stream to this stream's input 'inputnum'
        //-------------------------------------------------
        //  set_input - configure a stream's input
        //-------------------------------------------------
        public void set_input(int index, sound_stream input_stream, int output_index = 0, float gain = 1.0f)
        {
            sound_global.VPRINTF("stream_set_input({0}, '{1}', {2}, {3}, {4}, {5})\n", this, m_device.tag(), index, input_stream, output_index, (double)gain);

            // make sure it's a valid input
            if (index >= (int)m_input.size())
                g.fatalerror("stream_set_input attempted to configure nonexistent input {0} ({1} max)\n", index, (int)m_input.size());

            // make sure it's a valid output
            if (input_stream != null && output_index >= (int)input_stream.m_output.size())
                g.fatalerror("stream_set_input attempted to use a nonexistent output {0} ({1} max)\n", output_index, (int)m_output.size());

            // wire it up
            m_input[index].set_source((input_stream != null) ? input_stream.m_output[output_index] : null);
            m_input[index].set_gain(gain);

            // update sample rates now that we know the input
            sample_rate_changed();
        }


        // force an update to the current time
        //-------------------------------------------------
        //  update - force a stream to update to
        //  the current emulated time
        //-------------------------------------------------
        public void update()
        {
            // ignore any update requests if we're already up to date
            attotime start = m_output[0].end_time();
            attotime end = m_device.machine().time();
            if (start >= end)
                return;

            // regular update then
            update_view(start, end);
        }


        // force an update to the current time, returning a view covering the given time period
        //-------------------------------------------------
        //  update_view - force a stream to update to
        //  the current emulated time and return a view
        //  to the generated samples from the given
        //  output number
        //-------------------------------------------------
        public read_stream_view update_view(attotime start, attotime end, u32 outputnum = 0)
        {
            sound_global.sound_assert(start <= end);
            sound_global.sound_assert(outputnum < m_output.size());

            // clean up parameters for when the asserts go away
            if (outputnum >= m_output.size())
                outputnum = 0;

            if (start > end)
                start = end;

            profiler_global.g_profiler.start(profile_type.PROFILER_SOUND);

            // reposition our start to coincide with the current buffer end
            attotime update_start = m_output[outputnum].end_time();
            if (update_start <= end)
            {
                // create views for all the outputs
                for (unsigned outindex = 0; outindex < m_output.size(); outindex++)
                    m_output_view[outindex] = m_output[outindex].view(update_start, end);

                // skip if nothing to do
                u32 samples = m_output_view[0].samples();

                sound_global.sound_assert(samples >= 0);

                if (samples != 0 && m_sample_rate >= sound_global.SAMPLE_RATE_MINIMUM)
                {
                    sound_global.sound_assert(!synchronous() || samples == 1);

                    // ensure all input streams are up to date, and create views for them as well
                    for (unsigned inputnum = 0; inputnum < m_input.size(); inputnum++)
                    {
                        if (m_input[inputnum].valid())
                            m_input_view[inputnum] = m_input[inputnum].update(update_start, end);
                        else
                            m_input_view[inputnum] = empty_view(update_start, end);

                        sound_global.sound_assert(m_input_view[inputnum].samples() > 0);
                        sound_global.sound_assert(m_resampling_disabled || m_input_view[inputnum].sample_rate() == m_sample_rate);
                    }

#if (SOUND_DEBUG)
                    // clear each output view to NANs before we call the callback
                    for (unsigned int outindex = 0; outindex < m_output.size(); outindex++)
                        m_output_view[outindex].fill(NAN);
#endif

                    // if we have an extended callback, that's all we need
                    if (m_callback_ex != null)
                        m_callback_ex(this, m_input_view, m_output_view);

#if (SOUND_DEBUG)
                    // make sure everything was overwritten
                    for (unsigned int outindex = 0; outindex < m_output.size(); outindex++)
                        for (int sampindex = 0; sampindex < m_output_view[outindex].samples(); sampindex++)
                            m_output_view[outindex].get(sampindex);

                    for (unsigned int outindex = 0; outindex < m_output.size(); outindex++)
                        m_output[outindex].m_buffer.flush_wav();
#endif
                }
            }

            profiler_global.g_profiler.stop();

            // return the requested view
            return new read_stream_view(m_output_view[outputnum], start);
        }


        // apply any pending sample rate changes; should only be called by the sound manager
        //-------------------------------------------------
        //  apply_sample_rate_changes - if there is a
        //  pending sample rate change, apply it now
        //-------------------------------------------------
        public void apply_sample_rate_changes(u32 updatenum, u32 downstream_rate)
        {
            // grab the new rate and invalidate
            u32 new_rate = (m_pending_sample_rate != sound_global.SAMPLE_RATE_INVALID) ? m_pending_sample_rate : m_sample_rate;
            m_pending_sample_rate = sound_global.SAMPLE_RATE_INVALID;

            // clamp to the minimum - 1 (anything below minimum means "off" and
            // will not call the sound callback at all)
            if (new_rate < sound_global.SAMPLE_RATE_MINIMUM)
                new_rate = sound_global.SAMPLE_RATE_MINIMUM - 1;

            // if we're input adaptive, override with the rate of our input
            if (input_adaptive() && m_input.size() > 0 && m_input[0].valid())
                new_rate = m_input[0].source().stream().sample_rate();

            // if we're output adaptive, override with the rate of our output
            if (output_adaptive())
            {
                if (m_last_sample_rate_update == updatenum)
                {
                    sound_global.sound_assert(new_rate == m_sample_rate);
                }
                else
                {
                    m_last_sample_rate_update = updatenum;
                }

                new_rate = downstream_rate;
            }

            // if something is different, process the change
            if (new_rate != sound_global.SAMPLE_RATE_INVALID && new_rate != m_sample_rate)
            {
                // update to the new rate and notify everyone
#if (SOUND_DEBUG)
                printf("stream %s changing rates %d -> %d\n", name().c_str(), m_sample_rate, new_rate);
#endif
                m_sample_rate = new_rate;
                sample_rate_changed();
            }

            // now call through our inputs and apply the rate change there
            foreach (var input in m_input)
            {
                if (input.valid())
                    input.apply_sample_rate_changes(updatenum, m_sample_rate);
            }
        }


#if SOUND_DEBUG
        // print one level of the sound graph and recursively tell our inputs to do the same
        void print_graph_recursive(int indent, int index);
#endif


        // perform most of the initialization here
        //void init_common(u32 inputs, u32 outputs, u32 sample_rate, sound_stream_flags flags);


        // if the sample rate has changed, this gets called to update internals
        //-------------------------------------------------
        //  sample_rate_changed - recompute sample
        //  rate data, and all streams that are affected
        //  by this stream
        //-------------------------------------------------
        void sample_rate_changed()
        {
            // if invalid, just punt
            if (m_sample_rate == sound_global.SAMPLE_RATE_INVALID)
                return;

            // update all output buffers
            foreach (var output in m_output)
                output.sample_rate_changed(m_sample_rate);

            // if synchronous, prime the timer
            if (synchronous())
                reprime_sync_timer();
        }


        // handle updates after a save state load
        //-------------------------------------------------
        //  postload - save/restore callback
        //-------------------------------------------------
        void postload()
        {
            // set the end time of all of our streams to now
            foreach (var output in m_output)
                output.set_end_time(m_device.machine().time());

            // recompute the sample rate information
            sample_rate_changed();
        }


        // re-print the synchronization timer
        //-------------------------------------------------
        //  reprime_sync_timer - set up the next sync
        //  timer to go off just a hair after the end of
        //  the current sample period
        //-------------------------------------------------
        void reprime_sync_timer()
        {
            attotime curtime = m_device.machine().time();
            attotime target = m_output[0].end_time() + new attotime(0, 1);
            m_sync_timer.adjust(target - curtime);
        }


        // timer callback for synchronous streams
        //-------------------------------------------------
        //  sync_update - timer callback to handle a
        //  synchronous stream
        //-------------------------------------------------
        void sync_update(object o, s32 s)  //void sync_update(void *, s32);
        {
            update();
            reprime_sync_timer();
        }


        // return a view of 0 data covering the given time period
        //-------------------------------------------------
        //  empty_view - return an empty view covering the
        //  given time period as a substitute for invalid
        //  inputs
        //-------------------------------------------------
        read_stream_view empty_view(attotime start, attotime end)
        {
            // if our dummy buffer doesn't match our sample rate, update and clear it
            if (m_empty_buffer.sample_rate() != m_sample_rate)
                m_empty_buffer.set_sample_rate(m_sample_rate, false);

            // allocate a write view so that it can expand, and convert back to a read view
            // on the return
            return new write_stream_view(m_empty_buffer, start, end);
        }
    }


    // ======================> default_resampler_stream
    class default_resampler_stream : sound_stream
    {
        // internal state
        u32 m_max_latency;


        // construction/destruction
        public default_resampler_stream(device_t device)
            : base(device, 1, 1, 0, sound_global.SAMPLE_RATE_OUTPUT_ADAPTIVE, /*resampler_sound_update,*/ sound_stream_flags.STREAM_DISABLE_INPUT_RESAMPLING)  //sound_stream(device, 1, 1, 0, SAMPLE_RATE_OUTPUT_ADAPTIVE, stream_update_delegate(&default_resampler_stream::resampler_sound_update, this), STREAM_DISABLE_INPUT_RESAMPLING),
        {
            sound_stream_after_ctor(resampler_sound_update);


            m_max_latency = 0;


            // create a name
            m_name = "Default Resampler '";
            m_name += device.tag();
            m_name += "'";
        }


        // update handler
        //-------------------------------------------------
        //  resampler_sound_update - stream callback
        //  handler for resampling an input stream to the
        //  target sample rate of the output
        //-------------------------------------------------
        void resampler_sound_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //void resampler_sound_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs);
        {
            sound_global.sound_assert(inputs.size() == 1);
            sound_global.sound_assert(outputs.size() == 1);

            var input = inputs[0];
            var output = outputs[0];

            // if the input has an invalid rate, just fill with zeros
            if (input.sample_rate() <= 1)
            {
                output.fill(0);
                return;
            }

            // optimize_resampler ensures we should not have equal sample rates
            sound_global.sound_assert(input.sample_rate() != output.sample_rate());

            // compute the stepping value and the inverse
            stream_buffer_sample_t step = (stream_buffer_sample_t)input.sample_rate() / (stream_buffer_sample_t)output.sample_rate();
            stream_buffer_sample_t stepinv = (stream_buffer_sample_t)(1.0 / step);

            // determine the latency we need to introduce, in input samples:
            //    1 input sample for undersampled inputs
            //    1 + step input samples for oversampled inputs
            s64 latency_samples = 1 + ((step < 1.0) ? 0 : (s32)step);
            if (latency_samples <= m_max_latency)
                latency_samples = m_max_latency;
            else
                m_max_latency = (u32)latency_samples;

            attotime latency = (u32)latency_samples * input.sample_period();

            // clamp the latency to the start (only relevant at the beginning)
            s32 dstindex = 0;
            attotime output_start = output.start_time();
            var numsamples = output.samples();
            while (latency > output_start && dstindex < numsamples)
            {
                output.put(dstindex++, 0);
                output_start += output.sample_period();
            }

            if (dstindex >= numsamples)
                return;

            // create a rebased input buffer around the adjusted start time
            read_stream_view rebased = new read_stream_view(input, output_start - latency);

            sound_global.sound_assert(rebased.start_time() + latency <= output_start);

            // compute the fractional input start position
            attotime delta = output_start - (rebased.start_time() + latency);

            sound_global.sound_assert(delta.seconds() == 0);

            stream_buffer_sample_t srcpos = (stream_buffer_sample_t)((double)delta.attoseconds() / (double)rebased.sample_period_attoseconds());

            sound_global.sound_assert(srcpos <= 1.0f);

            // input is undersampled: point sample except where our sample period covers a boundary
            s32 srcindex = 0;
            if (step < 1.0)
            {
                stream_buffer_sample_t cursample = rebased.get(srcindex++);
                for ( ; dstindex < numsamples; dstindex++)
                {
                    // if still within the current sample, just replicate
                    srcpos += step;
                    if (srcpos <= 1.0)
                        output.put(dstindex, cursample);

                    // if crossing a sample boundary, blend with the neighbor
                    else
                    {
                        srcpos -= (stream_buffer_sample_t)1.0;

                        sound_global.sound_assert(srcpos <= step + 1e-5);

                        stream_buffer_sample_t prevsample = cursample;
                        cursample = rebased.get(srcindex++);
                        output.put(dstindex, stepinv * (prevsample * (step - srcpos) + srcpos * cursample));
                    }
                }

                sound_global.sound_assert(srcindex <= rebased.samples());
            }

            // input is oversampled: sum the energy
            else
            {
                float cursample = rebased.get(srcindex++);
                for ( ; dstindex < numsamples; dstindex++)
                {
                    // compute the partial first sample and advance
                    stream_buffer_sample_t scale = (stream_buffer_sample_t)(1.0 - srcpos);
                    stream_buffer_sample_t sample = cursample * scale;

                    // add in complete samples until we only have a fraction left
                    stream_buffer_sample_t remaining = step - scale;
                    while (remaining >= 1.0)
                    {
                        sample += rebased.get(srcindex++);
                        remaining -= (stream_buffer_sample_t)1.0;
                    }

                    // add in the final partial sample
                    cursample = rebased.get(srcindex++);
                    sample += cursample * remaining;
                    output.put(dstindex, sample * stepinv);

                    // our position is now the remainder
                    srcpos = remaining;

                    sound_global.sound_assert(srcindex <= rebased.samples());
                }
            }
        }
    }


    // ======================> sound_manager
    // structure describing an indexed mixer
    class mixer_input
    {
        public device_mixer_interface mixer;          // owning device interface
        public sound_stream stream;         // stream within the device
        public int inputnum;       // input on the stream
    }


    public class sound_manager
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
        running_machine m_machine;           // reference to the running machine  //running_machine &m_machine;           // reference to the running machine
        emu_timer m_update_timer;            // timer that runs the update function  //emu_timer *m_update_timer;            // timer that runs the update function
        std.vector<speaker_device> m_speakers = new std.vector<speaker_device>();  //std::vector<std::reference_wrapper<speaker_device> > m_speakers;

        u32 m_update_number;                  // current update index; used for sample rate updates
        attotime m_last_update;               // time of the last update
        u32 m_finalmix_leftover;              // leftover samples in the final mix
        u32 m_samples_this_update;            // number of samples this update
        std.vector<s16> m_finalmix;          // final mix, in 16-bit signed format
        std.vector<stream_buffer_sample_t> m_leftmix; // left speaker mix, in native format
        std.vector<stream_buffer_sample_t> m_rightmix; // right speaker mix, in native format

        stream_buffer_sample_t m_compressor_scale; // current compressor scale factor
        int m_compressor_counter;             // compressor update counter for backoff

        u8 m_muted;                           // bitmask of muting reasons
        bool m_nosound_mode;                  // true if we're in "nosound" mode
        int m_attenuation;                    // current attentuation level (at the OSD)
        int m_unique_id;                      // unique ID used for stream identification
        wav_file m_wavfile;                  // WAV file for streaming  //util::wav_file_ptr m_wavfile;         // WAV file for streaming

        // streams data
        std.vector<sound_stream> m_stream_list = new std.vector<sound_stream>(); // list of streams  //std::vector<std::unique_ptr<sound_stream>> m_stream_list; // list of streams
        std.map<sound_stream, u8> m_orphan_stream_list = new std.map<sound_stream, u8>(); // list of orphaned streams  //std::map<sound_stream *, u8> m_orphan_stream_list; // list of orphaned streams
        bool m_first_reset;                   // is this our first reset?


        // construction/destruction
        public sound_manager(running_machine machine)
        {
            m_machine = machine;
            m_update_timer = null;
            m_update_number = 0;
            m_last_update = attotime.zero;
            m_finalmix_leftover = 0;
            m_samples_this_update = 0;
            m_finalmix = new std.vector<s16>(machine.sample_rate());
            m_leftmix = new std.vector<stream_buffer_sample_t>(machine.sample_rate());
            m_rightmix = new std.vector<stream_buffer_sample_t>(machine.sample_rate());
            m_compressor_scale = (stream_buffer_sample_t)1.0;
            m_compressor_counter = 0;
            m_muted = 0;
            m_nosound_mode = machine.osd().no_sound();
            m_attenuation = 0;
            m_unique_id = 0;
            m_wavfile = null;
            m_first_reset = true;


            // get filename for WAV file or AVI file if specified
            string wavfile = machine.options().wav_write();
            string avifile = machine.options().avi_write();

            // handle -nosound and lower sample rate if not recording WAV or AVI
            if (m_nosound_mode && wavfile[0] == 0 && avifile[0] == 0)
                machine.m_sample_rate = 11025;

            // count the mixers
#if VERBOSE
            mixer_interface_enumerator iter(machine.root_device());
            VPRINTF(("total mixers = %d\n", iter.count()));
#endif

            // register callbacks
            machine.configuration().config_register("mixer", config_load, config_save);
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_PAUSE, pause);
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_RESUME, resume);
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_RESET, reset);
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_EXIT, stop_recording);

            // register global states
            machine.save().save_item(g.NAME(new { m_last_update }));

            // set the starting attenuation
            set_attenuation(machine.options().volume());

            // start the periodic update flushing timer
            m_update_timer = machine.scheduler().timer_alloc(update);
            m_update_timer.adjust(STREAMS_UPDATE_ATTOTIME, 0, STREAMS_UPDATE_ATTOTIME);
        }

        //~sound_manager();


        // getters
        running_machine machine() { return m_machine; }
        //int attenuation() const { return m_attenuation; }
        public std.vector<sound_stream> streams() { return m_stream_list; }  //const std::vector<std::unique_ptr<sound_stream>> &streams() const { return m_stream_list; }
        //attotime last_update() const { return m_last_update; }
        //int sample_count() const { return m_samples_this_update; }
        public int unique_id() { return m_unique_id++; }


        // allocate a new stream with a new-style callback
        //-------------------------------------------------
        //  stream_alloc - allocate a new stream with the
        //  new-style callback and flags
        //-------------------------------------------------
        public sound_stream stream_alloc(device_t device, u32 inputs, u32 outputs, u32 sample_rate, stream_update_delegate callback, sound_stream_flags flags)
        {
            // determine output base
            u32 output_base = 0;
            foreach (var stream in m_stream_list)
            {
                if (stream.device() == device)
                    output_base += stream.output_count();
            }

            m_stream_list.push_back(new sound_stream(device, inputs, outputs, output_base, sample_rate, callback, flags));  //m_stream_list.push_back(std::make_unique<sound_stream>(device, inputs, outputs, output_base, sample_rate, callback, flags));
            return m_stream_list.back();
        }


        // WAV recording
        //bool is_recording() const { return bool(m_wavfile); }


        //-------------------------------------------------
        //  start_recording - begin audio recording
        //-------------------------------------------------
        public bool start_recording()
        {
            // open the output WAV file if specified
            string filename = machine().options().wav_write();
            return !string.IsNullOrEmpty(filename) ? start_recording(filename) : false;
        }


        bool start_recording(string filename)
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  stop_recording - end audio recording
        //-------------------------------------------------
        void stop_recording(running_machine machine)
        {
            // close any open WAV file
            m_wavfile = null;  //m_wavfile.reset();
        }


        // set the global OSD attenuation level
        //-------------------------------------------------
        //  set_attenuation - set the global volume
        //-------------------------------------------------
        void set_attenuation(float attenuation)
        {
            // currently OSD only supports integral attenuation
            m_attenuation = (int)(attenuation);
            machine().osd().set_mastervolume(m_muted != 0 ? -32 : m_attenuation);
        }


        // mute sound for one of various independent reasons
        //bool muted() const { return bool(m_muted); }
        public bool ui_mute() { return (m_muted & MUTE_REASON_UI) != 0; }
        //bool debugger_mute() const { return bool(m_muted & MUTE_REASON_DEBUGGER); }
        public bool system_mute() { return (m_muted & MUTE_REASON_SYSTEM) != 0; }
        public void ui_mute(bool turn_off) { mute(turn_off, MUTE_REASON_UI); }
        //void debugger_mute(bool turn_off) { mute(turn_off, MUTE_REASON_DEBUGGER); }
        public void system_mute(bool turn_off) { mute(turn_off, MUTE_REASON_SYSTEM); }


        // return information about the given mixer input, by index
        //-------------------------------------------------
        //  indexed_mixer_input - return the mixer
        //  device and input index of the global mixer
        //  input
        //-------------------------------------------------
        bool indexed_mixer_input(int index, out mixer_input info)
        {
            info = new mixer_input();

            // scan through the mixers until we find the indexed input
            foreach (device_mixer_interface mixer in new mixer_interface_enumerator(machine().root_device()))
            {
                if (index < mixer.inputs())
                {
                    info.mixer = mixer;
                    info.stream = mixer.input_to_stream_input(index, out info.inputnum);

                    sound_global.sound_assert(info.stream != null);

                    return true;
                }

                index -= mixer.inputs();
            }

            // didn't locate
            info.mixer = null;
            return false;
        }


        // fill the given buffer with 16-bit stereo audio samples
        //void samples(s16 *buffer);


        // set/reset the mute state for the given reason
        //-------------------------------------------------
        //  mute - mute sound output
        //-------------------------------------------------
        void mute(bool mute, u8 reason)
        {
            bool old_muted = m_muted != 0;
            if (mute)
                m_muted |= reason;
            else
                m_muted &= (u8)~reason;

            if (old_muted != (m_muted != 0))
                set_attenuation(m_attenuation);
        }


        // helper to remove items from the orphan list
        //-------------------------------------------------
        //  recursive_remove_stream_from_orphan_list -
        //  remove the given stream from the orphan list
        //  and recursively remove all our inputs
        //-------------------------------------------------
        void recursive_remove_stream_from_orphan_list(sound_stream which)
        {
            m_orphan_stream_list.erase(which);
            for (int inputnum = 0; inputnum < which.input_count(); inputnum++)
            {
                var input = which.input(inputnum);
                if (input.valid())
                    recursive_remove_stream_from_orphan_list(input.source().stream());
            }
        }


        // apply pending sample rate changes
        //-------------------------------------------------
        //  apply_sample_rate_changes - recursively
        //  update sample rates throughout the system
        //-------------------------------------------------
        void apply_sample_rate_changes()
        {
            // update sample rates if they have changed
            foreach (var speaker in new speaker_device_enumerator(machine().root_device()))
            {
                int stream_out;
                sound_stream stream = speaker.device_sound_interface_output_to_stream_output(0, out stream_out);

                // due to device removal, some speakers may end up with no outputs; just skip those
                if (stream != null)
                {
                    sound_global.sound_assert(speaker.device_sound_interface_outputs() == 1);

                    stream.apply_sample_rate_changes(m_update_number, (u32)machine().sample_rate());
                }
            }
        }


        // reset all sound chips
        //-------------------------------------------------
        //  reset - reset all sound chips
        //-------------------------------------------------
        void reset(running_machine machine_)
        {
            // reset all the sound chips
            foreach (device_sound_interface sound in new sound_interface_enumerator(machine().root_device()))
                sound.device().reset();

            // apply any sample rate changes now
            apply_sample_rate_changes();

            // on first reset, identify any orphaned streams
            if (m_first_reset)
            {
                m_first_reset = false;

                // put all the streams on the orphan list to start
                foreach (var stream in m_stream_list)
                    m_orphan_stream_list[stream] = 0;

                // then walk the graph like we do on update and remove any we touch
                foreach (speaker_device speaker in new speaker_device_enumerator(machine().root_device()))
                {
                    int dummy;
                    sound_stream output = speaker.device_sound_interface_output_to_stream_output(0, out dummy);
                    if (output != null)
                        recursive_remove_stream_from_orphan_list(output);

                    m_speakers.emplace_back(speaker);
                }

#if (SOUND_DEBUG)
                // dump the sound graph when we start up
                for (speaker_device &speaker : speaker_device_enumerator(machine().root_device()))
                {
                    int index;
                    sound_stream *output = speaker.output_to_stream_output(0, index);
                    if (output != nullptr)
                        output->print_graph_recursive(0, index);
                }

                // dump the orphan list as well
                if (m_orphan_stream_list.size() != 0)
                {
                    osd_printf_info("\nOrphaned streams:\n");
                    for (auto &stream : m_orphan_stream_list)
                        osd_printf_info("   %s\n", stream.first->name());
                }
#endif
            }
        }


        // pause/resume sound output
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


        // handle configuration load/save
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

            // iterate over channel nodes
            for (util.xml.data_node channelnode = parentnode.get_child("channel"); channelnode != null; channelnode = channelnode.get_next_sibling("channel"))
            {
                mixer_input info;
                if (indexed_mixer_input((int)channelnode.get_attribute_int("index", -1), out info))
                {
                    float defvol = channelnode.get_attribute_float("defvol", 1.0f);
                    float newvol = channelnode.get_attribute_float("newvol", -1000.0f);
                    if (newvol != -1000.0f)
                        info.stream.input(info.inputnum).set_user_gain(newvol / defvol);
                }
            }
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
                for (int mixernum = 0; ; mixernum++)
                {
                    mixer_input info;
                    if (!indexed_mixer_input(mixernum, out info))
                        break;

                    float newvol = info.stream.input(info.inputnum).user_gain();

                    if (newvol != 1.0f)
                    {
                        util.xml.data_node channelnode = parentnode.add_child("channel", null);
                        if (channelnode != null)
                        {
                            channelnode.set_attribute_int("index", mixernum);
                            channelnode.set_attribute_float("newvol", newvol);
                        }
                    }
                }
            }
        }


        // helper to adjust scale factor toward a goal
        //-------------------------------------------------
        //  adjust_toward_compressor_scale - adjust the
        //  current scale factor toward the current goal,
        //  in small increments
        //-------------------------------------------------
        stream_buffer_sample_t adjust_toward_compressor_scale(stream_buffer_sample_t curscale, stream_buffer_sample_t prevsample, stream_buffer_sample_t rawsample)  //stream_buffer::sample_t adjust_toward_compressor_scale(stream_buffer::sample_t curscale, stream_buffer::sample_t prevsample, stream_buffer::sample_t rawsample);
        {
            stream_buffer_sample_t proposed_scale = curscale;

            // if we want to get larger, increment by 0.01
            if (curscale < m_compressor_scale)
            {
                proposed_scale += 0.01f;
                if (proposed_scale > m_compressor_scale)
                    proposed_scale = m_compressor_scale;
            }

            // otherwise, decrement by 0.01
            else
            {
                proposed_scale -= 0.01f;
                if (proposed_scale < m_compressor_scale)
                    proposed_scale = m_compressor_scale;
            }

            // compute the sample at the current scale and at the proposed scale
            stream_buffer_sample_t cursample = rawsample * curscale;
            stream_buffer_sample_t proposed_sample = rawsample * proposed_scale;

            // if they trend in the same direction, it's ok to take the step
            if ((cursample < prevsample && proposed_sample < prevsample) || (cursample > prevsample && proposed_sample > prevsample))
                curscale = proposed_scale;

            // return the current scale
            return curscale;
        }


        // periodic sound update, called STREAMS_UPDATE_FREQUENCY per second
        //-------------------------------------------------
        //  update - mix everything down to its final form
        //  and send it to the OSD layer
        //-------------------------------------------------
        void update(object ptr = null, s32 param = 0)  //void update(void *ptr = nullptr, s32 param = 0);
        {
            sound_global.VPRINTF("sound_update\n");

            profiler_global.g_profiler.start(profile_type.PROFILER_SOUND);

            // determine the duration of this update
            attotime update_period = machine().time() - m_last_update;

            sound_global.sound_assert(update_period.seconds() == 0);

            // use that to compute the number of samples we need from the speakers
            attoseconds_t sample_rate_attos = attotime.HZ_TO_ATTOSECONDS(machine().sample_rate());
            m_samples_this_update = (u32)(update_period.attoseconds() / sample_rate_attos);

            // recompute the end time to an even sample boundary
            attotime endtime = m_last_update + new attotime(0, m_samples_this_update * sample_rate_attos);

            // clear out the mix bufers
            std.fill_n(m_leftmix, m_samples_this_update, 0);  //std::fill_n(&m_leftmix[0], m_samples_this_update, 0);
            std.fill_n(m_rightmix, m_samples_this_update, 0);  //std::fill_n(&m_rightmix[0], m_samples_this_update, 0);

            // force all the speaker streams to generate the proper number of samples
            foreach (speaker_device speaker in m_speakers)
                speaker.mix(new Pointer<stream_buffer_sample_t>(m_leftmix), new Pointer<stream_buffer_sample_t>(m_rightmix), m_last_update, endtime, (int)m_samples_this_update, (m_muted & MUTE_REASON_SYSTEM) != 0);  //speaker.mix(&m_leftmix[0], &m_rightmix[0], m_last_update, endtime, m_samples_this_update, (m_muted & MUTE_REASON_SYSTEM));

            // determine the maximum in this section
            stream_buffer_sample_t curmax = 0;
            for (int sampindex = 0; sampindex < m_samples_this_update; sampindex++)
            {
                var sample2 = m_leftmix[sampindex];
                if (sample2 < 0)
                    sample2 = -sample2;
                if (sample2 > curmax)
                    curmax = sample2;

                sample2 = m_rightmix[sampindex];
                if (sample2 < 0)
                    sample2 = -sample2;
                if (sample2 > curmax)
                    curmax = sample2;
            }

            // pull in current compressor scale factor before modifying
            stream_buffer_sample_t lscale = m_compressor_scale;
            stream_buffer_sample_t rscale = m_compressor_scale;

            // if we're above what the compressor will handle, adjust the compression
            if (curmax * m_compressor_scale > 1.0)
            {
                m_compressor_scale = (stream_buffer_sample_t)1.0 / curmax;
                m_compressor_counter = STREAMS_UPDATE_FREQUENCY / 5;
            }

            // if we're currently scaled, wait a bit to see if we can trend back toward 1.0
            else if (m_compressor_counter != 0)
                m_compressor_counter--;

            // try to migrate toward 0 unless we're going to introduce clipping
            else if (m_compressor_scale < 1.0 && curmax * 1.01 * m_compressor_scale < 1.0)
            {
                m_compressor_scale *= 1.01f;
                if (m_compressor_scale > 1.0)
                    m_compressor_scale = (stream_buffer_sample_t)1.0;
            }

#if (SOUND_DEBUG)
            if (lscale != m_compressor_scale)
            printf("scale=%.5f\n", m_compressor_scale);
#endif

            // track whether there are pending scale changes in left/right
            stream_buffer_sample_t lprev = 0;
            stream_buffer_sample_t rprev = 0;

            // now downmix the final result
            u32 finalmix_step = (u32)machine().video().speed_factor();
            u32 finalmix_offset = 0;
            Pointer<s16> finalmix = new Pointer<s16>(m_finalmix);  //s16 *finalmix = &m_finalmix[0];
            int sample;
            for (sample = (int)m_finalmix_leftover; sample < m_samples_this_update * 1000; sample += (int)finalmix_step)
            {
                int sampindex = sample / 1000;

                // ensure that changing the compression won't reverse direction to reduce "pops"
                stream_buffer_sample_t lsamp = m_leftmix[sampindex];
                if (lscale != m_compressor_scale && sample != m_finalmix_leftover)
                    lscale = adjust_toward_compressor_scale(lscale, lprev, lsamp);

                // clamp the left side
                lprev = lsamp *= lscale;
                if (lsamp > 1.0)
                    lsamp = (stream_buffer_sample_t)1.0;
                else if (lsamp < -1.0)
                    lsamp = (stream_buffer_sample_t)(-1.0);
                finalmix[finalmix_offset++] = (s16)(lsamp * 32767.0);

                // ensure that changing the compression won't reverse direction to reduce "pops"
                stream_buffer_sample_t rsamp = m_rightmix[sampindex];
                if (rscale != m_compressor_scale && sample != m_finalmix_leftover)
                    rscale = adjust_toward_compressor_scale(rscale, rprev, rsamp);

                // clamp the left side
                rprev = rsamp *= rscale;
                if (rsamp > 1.0)
                    rsamp = (stream_buffer_sample_t)1.0;
                else if (rsamp < -1.0)
                    rsamp = (stream_buffer_sample_t)(-1.0);
                finalmix[finalmix_offset++] = (s16)(rsamp * 32767.0);
            }

            m_finalmix_leftover = (u32)sample - m_samples_this_update * 1000;

            // play the result
            if (finalmix_offset > 0)
            {
                if (!m_nosound_mode)
                    machine().osd().update_audio_stream(finalmix, (int)finalmix_offset / 2);
                machine().osd().add_audio_to_recording(finalmix, (int)finalmix_offset / 2);
                machine().video().add_sound_to_recording(finalmix, (int)finalmix_offset / 2);
                if (m_wavfile != null)
                    wavwrite_global.wav_add_data_16(m_wavfile, finalmix, (int)finalmix_offset);
            }

            // update any orphaned streams so they don't get too far behind
            foreach (var stream in m_orphan_stream_list)
                stream.first().update();

            // remember the update time
            m_last_update = endtime;
            m_update_number++;

            // apply sample rate changes
            apply_sample_rate_changes();

            // notify that new samples have been generated
            emulator_info.sound_hook();

            profiler_global.g_profiler.stop();
        }
    }
}
