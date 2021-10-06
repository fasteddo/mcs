// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int16_t = System.Int16;
using int32_t = System.Int32;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using samples_device_enumerator = mame.device_type_enumerator<mame.samples_device>;  //typedef device_type_enumerator<samples_device> samples_device_enumerator;
using size_t = System.UInt64;
using stream_buffer_sample_t = System.Single;  //using sample_t = float;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    // ======================> samples_device
    public class samples_device : device_t
                                  //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(SAMPLES, samples_device, "samples", "Samples")
        static device_t device_creator_samples_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock = 0) { return new samples_device(mconfig, tag, owner, clock); }
        public static readonly device_type SAMPLES = g.DEFINE_DEVICE_TYPE(device_creator_samples_device, "samples", "Samples");


        class device_sound_interface_samples : device_sound_interface
        {
            public device_sound_interface_samples(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((samples_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        //typedef device_delegate<void ()> start_cb_delegate;
        public delegate void start_cb_delegate();


        class sample_t
        {
            public uint32_t frequency;      // frequency of the sample
            public std.vector<int16_t> data = new std.vector<int16_t>();  // 16-bit signed data

            // shouldn't need a copy, but in case it happens, catch it here
            //sample_t &operator=(const sample_t &rhs) { assert(false); return *this; }
        }


        class channel_t
        {
            public sound_stream stream;
            public Pointer<int16_t> source;  //const int16_t * source;
            public int32_t source_num;
            public uint32_t source_len;
            public double pos;
            uint32_t basefreq;
            public uint32_t curfreq;
            public bool loop;
            public bool paused;
        }


        // internal constants
        const byte FRAC_BITS = 24;
        //static const UINT32 FRAC_ONE = 1 << FRAC_BITS;
        //static const UINT32 FRAC_MASK = FRAC_ONE - 1;


        device_sound_interface_samples m_disound;

        // interface
        uint8_t m_channels;         // number of discrete audio channels needed
        public string [] m_names;     // array of sample names
        start_cb_delegate m_samples_start_cb; // optional callback

        // internal state
        std.vector<channel_t> m_channel = new std.vector<channel_t>();
        std.vector<sample_t> m_sample = new std.vector<sample_t>();


        // construction/destruction
        //-------------------------------------------------
        //  samples_device - constructors
        //-------------------------------------------------
        samples_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : this(mconfig, SAMPLES, tag, owner, clock)
        {
        }

        protected samples_device(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_samples(mconfig, this));

            m_channels = 0;
            m_names = null;
            m_samples_start_cb = null;
        }


        // configuration helpers
        //void set_channels(uint8_t channels) { m_channels = channels; }
        //void set_samples_names(const char *const *names) { m_names = names; }

        // start callback helpers
        //template <typename... T> void set_samples_start_callback(T &&...args) { m_samples_start_cb.set(std::forward<T>(args)...); }


        // getters
        //bool playing(UINT8 channel) const;
        //UINT32 base_frequency(UINT8 channel) const;


        // start/stop helpers
        //void start(UINT8 channel, UINT32 samplenum, bool loop = false);
        //void start_raw(UINT8 channel, const INT16 *sampledata, UINT32 samples, UINT32 frequency, bool loop = false);
        //void pause(UINT8 channel, bool pause = true);

        //-------------------------------------------------
        //  stop - stop playback on a channel
        //-------------------------------------------------
        void stop(uint8_t channel)
        {
            g.assert(channel < m_channels);

            // force an update before we start
            channel_t chan = m_channel[channel];
            chan.source = null;
            chan.source_num = -1;
        }


        //-------------------------------------------------
        //  stop_all - stop playback on all channels
        //-------------------------------------------------
        void stop_all()
        {
            // just iterate over channels and stop them
            for (byte channel = 0; channel < m_channels; channel++)
                stop(channel);
        }


        // dynamic control
        //void set_frequency(UINT8 channel, UINT32 frequency);
        //void set_volume(UINT8 channel, float volume);


        // helpers
        //-------------------------------------------------
        //  read_sample - read a WAV or FLAC file as a
        //  sample
        //-------------------------------------------------
        static bool read_sample(emu_file file, sample_t sample)
        {
            // read the core header and make sure it's a proper file
            MemoryU8 buf = new MemoryU8(4, true);  //uint8_t buf[4];
            uint32_t offset = file.read(new Pointer<uint8_t>(buf), 4);
            if (offset < 4)
            {
                g.osd_printf_warning("Unable to read {0}, 0-byte file?\n", file.filename());
                return false;
            }

            // look for the appropriate RIFF tag
            if (buf[0] == 'R' && buf[1] == 'I' && buf[2] == 'F' && buf[3] == 'F')  // memcmp(&buf[0], "RIFF", 4) == 0)
                return read_wav_sample(file, sample);
            else if (buf[0] == 'f' && buf[1] == 'L' && buf[2] == 'a' && buf[3] == 'C')  // memcmp(&buf[0], "fLaC", 4) == 0)
                return read_flac_sample(file, sample);

            // if nothing appropriate, emit a warning
            g.osd_printf_warning("Unable to read {0}, corrupt file?\n", file.filename());

            return false;
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - handle device startup
        //-------------------------------------------------
        protected override void device_start()
        {
            m_disound = GetClassInterface<device_sound_interface_samples>();

            // read audio samples
            load_samples();

            // allocate channels
            m_channel.resize(m_channels);
            for (int channel = 0; channel < m_channels; channel++)
            {
                // initialize channel
                channel_t chan = m_channel[channel];
                chan.stream = m_disound.stream_alloc(0, 1, sound_global.SAMPLE_RATE_OUTPUT_ADAPTIVE);
                chan.source = null;
                chan.source_num = -1;
                chan.pos = 0;
                chan.loop = false;
                chan.paused = false;

                // register with the save state system
                save_item(g.NAME(new { chan.source_num }), channel);
                save_item(g.NAME(new { chan.source_len }), channel);
                save_item(g.NAME(new { chan.pos }), channel);
                save_item(g.NAME(new { chan.loop }), channel);
                save_item(g.NAME(new { chan.paused }), channel);
            }

            // initialize any custom handlers
            //m_samples_start_cb.resolve();

            if (m_samples_start_cb != null)
                m_samples_start_cb();
        }


        //-------------------------------------------------
        //  device_reset - handle device reset
        //-------------------------------------------------
        protected override void device_reset()
        {
            stop_all();
        }


        //-------------------------------------------------
        //  device_post_load - handle updating after a
        //  restore
        //-------------------------------------------------
        protected override void device_post_load()
        {
            // loop over channels
            for (int channel = 0; channel < m_channels; channel++)
            {
                // attach any samples that were loaded and playing
                channel_t chan = m_channel[channel];
                if (chan.source_num >= 0 && chan.source_num < m_sample.Count)
                {
                    sample_t sample = m_sample[chan.source_num];
                    chan.source = new Pointer<int16_t>(sample.data);  //chan.source = &sample.data[0];
                    chan.source_len = (u32)sample.data.size();
                    if (sample.data.Count == 0)
                        chan.source_num = -1;
                }

                // validate the position against the length in case the sample is smaller
                double endpos = chan.source_len;
                if (chan.source != null && chan.pos >= endpos)
                {
                    if (chan.loop)
                    {
                        double posfloor = std.floor(chan.pos);
                        chan.pos -= posfloor;
                        chan.pos += (double)((int32_t)posfloor % chan.source_len);
                    }
                    else
                    {
                        chan.source = null;
                        chan.source_num = -1;
                    }
                }
            }
        }


        // device_sound_interface overrides
        //-------------------------------------------------
        //  sound_stream_update - update a sound stream
        //-------------------------------------------------
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            // find the channel with this stream
            stream_buffer_sample_t sample_scale = (stream_buffer_sample_t)(1.0 / 32768.0);
            for (int channel = 0; channel < m_channels; channel++)
            {
                if (stream == m_channel[channel].stream)
                {
                    channel_t chan = m_channel[channel];
                    var buffer = outputs[0];

                    // process if we still have a source and we're not paused
                    if (chan.source != null && !chan.paused)
                    {
                        // load some info locally
                        double step = (double)chan.curfreq / (double)buffer.sample_rate();
                        double endpos = chan.source_len;
                        Pointer<int16_t> sample = new Pointer<int16_t>(chan.source);  //const int16_t *sample = chan.source;

                        for (int sampindex = 0; sampindex < buffer.samples(); sampindex++)
                        {
                            // do a linear interp on the sample
                            double pos_floor = std.floor(chan.pos);
                            double frac = chan.pos - pos_floor;
                            int32_t ipos = (int32_t)pos_floor;

                            stream_buffer_sample_t sample1 = (stream_buffer_sample_t)sample[ipos++];
                            stream_buffer_sample_t sample2 = (stream_buffer_sample_t)sample[(ipos + 1) % (int)chan.source_len];
                            buffer.put(sampindex, (stream_buffer_sample_t)(sample_scale * ((1.0 - frac) * sample1 + frac * sample2)));

                            // advance
                            chan.pos += step;

                            // handle looping/ending
                            if (chan.pos >= endpos)
                            {
                                if (chan.loop)
                                {
                                    chan.pos -= endpos;
                                }
                                else
                                {
                                    chan.source = null;
                                    chan.source_num = -1;
                                    buffer.fill(0, sampindex);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        buffer.fill(0);
                    }
                    break;
                }
            }
        }


        // internal helpers

        //-------------------------------------------------
        //  read_wav_sample - read a WAV file as a sample
        //-------------------------------------------------
        static bool read_wav_sample(emu_file file, sample_t sample)
        {
            // we already read the opening 'RIFF' tag
            uint32_t offset = 4;

            // get the total size
            uint32_t filesize;

            MemoryU8 filesizeBuffer = new MemoryU8(4, true);
            offset += file.read(new Pointer<uint8_t>(filesizeBuffer), 4);
            if (offset < 8)
            {
                g.osd_printf_warning("Unexpected size offset {0} ({1})\n", offset, file.filename());
                return false;
            }

            filesize = filesizeBuffer.GetUInt32();
            filesize = g.little_endianize_int32(filesize);

            // read the RIFF file type and make sure it's a WAVE file
            MemoryU8 buf = new MemoryU8(32, true);  //char [] buf = new char[32];
            offset += file.read(new Pointer<uint8_t>(buf), 4);
            if (offset < 12)
            {
                g.osd_printf_warning("Unexpected WAVE offset {0} ({1})\n", offset, file.filename());
                return false;
            }

            if (!(buf[0] == 'W' && buf[1] == 'A' && buf[2] == 'V' && buf[3] == 'E'))  // memcmp(&buf[0], "WAVE", 4) != 0)
            {
                g.osd_printf_warning("Could not find WAVE header ({0})\n", file.filename());
                return false;
            }

            // seek until we find a format tag
            uint32_t length;
            MemoryU8 lengthBuffer = new MemoryU8(4, true);
            while (true)
            {
                offset += file.read(new Pointer<uint8_t>(buf), 4);
                offset += file.read(new Pointer<uint8_t>(lengthBuffer), 4);
                length = lengthBuffer.GetUInt32();
                length = g.little_endianize_int32(length);
                if (buf[0] == 'f' && buf[1] == 'm' && buf[2] == 't' && buf[3] == ' ')  //if (memcmp(&buf[0], "fmt ", 4) == 0)
                    break;

                // seek to the next block
                file.seek(length, emu_file.SEEK_CUR);
                offset += length;
                if (offset >= filesize)
                {
                    g.osd_printf_warning("Could not find fmt tag ({0})\n", file.filename());
                    return false;
                }
            }

            // read the format -- make sure it is PCM
            uint16_t temp16;
            MemoryU8 temp16Buffer = new MemoryU8(2, true);
            offset += file.read(new Pointer<uint8_t>(temp16Buffer), 2);
            temp16 = temp16Buffer.GetUInt16();
            temp16 = g.little_endianize_int16(temp16);
            if (temp16 != 1)
            {
                g.osd_printf_warning("unsupported format {0} - only PCM is supported ({1})\n", temp16, file.filename());
                return false;
            }

            // number of channels -- only mono is supported
            offset += file.read(new Pointer<uint8_t>(temp16Buffer), 2);
            temp16 = temp16Buffer.GetUInt16();
            temp16 = g.little_endianize_int16(temp16);
            if (temp16 != 1)
            {
                g.osd_printf_warning("unsupported number of channels {0} - only mono is supported ({1})\n", temp16, file.filename());
                return false;
            }

            // sample rate
            uint32_t rate;
            MemoryU8 rateBuffer = new MemoryU8(4, true);
            offset += file.read(new Pointer<uint8_t>(rateBuffer), 4);
            rate = rateBuffer.GetUInt32();
            rate = g.little_endianize_int32(rate);

            // bytes/second and block alignment are ignored
            offset += file.read(new Pointer<uint8_t>(buf), 6);

            // bits/sample
            uint16_t bits;
            MemoryU8 bitsBuffer = new MemoryU8(2, true);
            offset += file.read(new Pointer<uint8_t>(bitsBuffer), 2);
            bits = bitsBuffer.GetUInt16();
            bits = g.little_endianize_int16(bits);
            if (bits != 8 && bits != 16)
            {
                g.osd_printf_warning("unsupported bits/sample {0} - only 8 and 16 are supported ({1})\n", bits, file.filename());
                return false;
            }

            // seek past any extra data
            file.seek(length - 16, emu_file.SEEK_CUR);
            offset += length - 16;

            // seek until we find a data tag
            while (true)
            {
                offset += file.read(new Pointer<uint8_t>(buf), 4);
                offset += file.read(new Pointer<uint8_t>(lengthBuffer), 4);
                length = lengthBuffer.GetUInt32();
                length = g.little_endianize_int32(length);
                if (buf[0] == 'd' && buf[1] == 'a' && buf[2] == 't' && buf[3] == 'a')  //if (memcmp(&buf[0], "data", 4) == 0)
                    break;

                // seek to the next block
                file.seek(length, emu_file.SEEK_CUR);
                offset += length;
                if (offset >= filesize)
                {
                    g.osd_printf_warning("Could not find data tag ({0})\n", file.filename());
                    return false;
                }
            }

            // if there was a 0 length data block, we're done
            if (length == 0)
            {
                g.osd_printf_warning("empty data block ({0})\n", file.filename());
                return false;
            }

            // fill in the sample data
            sample.frequency = rate;

            // read the data in
            if (bits == 8)
            {
                sample.data.resize(length);
                MemoryU8 sample_data_8bit = new MemoryU8((int)length, true);
                file.read(new Pointer<uint8_t>(sample_data_8bit), length);

                // convert 8-bit data to signed samples
                Pointer<uint8_t> tempptr = new Pointer<uint8_t>(sample_data_8bit);  //uint8_t *tempptr = reinterpret_cast<uint8_t *>(&sample.data[0]);
                for (int sindex = (int)length - 1; sindex >= 0; sindex--)
                    sample.data[sindex] = (Int16)((sbyte)(tempptr[sindex] ^ 0x80) * 256);
            }
            else
            {
                // 16-bit data is fine as-is
                sample.data.resize(length / 2);
                MemoryU8 sample_data_8bit = new MemoryU8((int)length, true);
                file.read(new Pointer<uint8_t>(sample_data_8bit), length);

                // swap high/low on big-endian systems
                if (g.ENDIANNESS_NATIVE != endianness_t.ENDIANNESS_LITTLE)
                {
                    for (UInt32 sindex = 0; sindex < length / 2; sindex++)
                        sample.data[sindex] = (Int16)g.little_endianize_int16(sample_data_8bit.GetUInt16((int)sindex));  //sample.data[sindex]);
                }
            }

            return true;
        }


        //-------------------------------------------------
        //  read_flac_sample - read a FLAC file as a sample
        //-------------------------------------------------
        static bool read_flac_sample(emu_file file, sample_t sample)
        {
            // seek back to the start of the file
            file.seek(0, emu_file.SEEK_SET);

            // create the FLAC decoder and fill in the sample data
            flac_decoder decoder = new flac_decoder(file.core_file_get());
            sample.frequency = decoder.sample_rate();

            // error if more than 1 channel or not 16bpp
            if (decoder.channels() != 1)
                return false;
            if (decoder.bits_per_sample() != 16)
                return false;

            // resize the array and read
            sample.data.resize(decoder.total_samples());
            if (!decoder.decode_interleaved(sample.data, (UInt32)sample.data.Count))
                return false;

            // finish up and clean up
            decoder.finish();

            return true;
        }

        //-------------------------------------------------
        //  load_samples - load all the samples in our
        //  attached interface
        //  Returns true when all samples were successfully read, else false
        //-------------------------------------------------
        bool load_samples()
        {
            bool ok = true;

            // if the user doesn't want to use samples, bail
            if (!machine().options().samples())
                return false;

            // iterate over ourself
            string basename = machine().basename();
            samples_iterator iter = new samples_iterator(this);
            string altbasename = iter.altbasename();

            // pre-size the array
            m_sample.resize((size_t)iter.count());

            // load the samples
            int index = 0;
            for (string samplename = iter.first(); samplename != null; index++, samplename = iter.next())
            {
                // attempt to open as FLAC first
                emu_file file = new emu_file(machine().options().sample_path(), g.OPEN_FLAG_READ);
                osd_file.error filerr = file.open(util.string_format("{0}" + g.PATH_SEPARATOR + "{1}.flac", basename, samplename));
                if (filerr != osd_file.error.NONE && altbasename != null)
                    filerr = file.open(util.string_format("{0}" + g.PATH_SEPARATOR + "{1}.flac", altbasename, samplename));

                // if not, try as WAV
                if (filerr != osd_file.error.NONE)
                    filerr = file.open(util.string_format("{0}" + g.PATH_SEPARATOR + "{1}.wav", basename, samplename));
                if (filerr != osd_file.error.NONE && altbasename != null)
                    filerr = file.open(util.string_format("{0}" + g.PATH_SEPARATOR + "{1}.wav", altbasename, samplename));

                // if opened, read it
                if (filerr == osd_file.error.NONE)
                {
                    read_sample(file, m_sample[index]);
                }
                else if (filerr == osd_file.error.NOT_FOUND)
                {
                    logerror("{0}: Sample '{1}' NOT FOUND\n", tag(), samplename);
                    ok = false;
                }

                file.close();
            }

            return ok;
        } 
    }


    // iterator, since lots of people are interested in these devices
    //typedef device_type_enumerator<samples_device> samples_device_enumerator;


    // ======================> samples_iterator
    class samples_iterator
    {
        // internal state
        samples_device m_samples;
        int m_current;


        // construction/destruction
        public samples_iterator(samples_device device)
        {
            m_samples = device;
            m_current = -1;
        }


        // getters
        public string altbasename() { return (m_samples.m_names != null && !string.IsNullOrEmpty(m_samples.m_names[0]) && m_samples.m_names[0][0] == '*') ? m_samples.m_names[0].Substring(1) : null; }  // return (m_samples.m_names != null && m_samples.m_names[0] != null && m_samples.m_names[0][0] == '*') ? &m_samples.m_names[0][1] : null; }


        // iteration
        public string first()
        {
            if (m_samples.m_names == null || string.IsNullOrEmpty(m_samples.m_names[0]))
                return null;

            m_current = 0;

            if (m_samples.m_names[0][0] == '*')
                m_current++;

            return m_samples.m_names[m_current++];
        }

        public string next()
        {
            if (m_current == -1 || string.IsNullOrEmpty(m_samples.m_names[m_current]))
                return null;

            return m_samples.m_names[m_current++];
        }

        // counting
        public int count()
        {
            int save = m_current;
            int result = 0;
            for (string scan = first(); scan != null; scan = next())
                result++;

            m_current = save;

            return result;
        }
    }
}
