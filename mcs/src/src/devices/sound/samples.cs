// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using int16_t = System.Int16;
using int32_t = System.Int32;
using ListBytesPointer = mame.ListPointer<System.Byte>;
using stream_sample_t = System.Int32;
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
        static device_t device_creator_samples_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock = 0) { return new samples_device(mconfig, tag, owner, clock); }
        public static readonly device_type SAMPLES = DEFINE_DEVICE_TYPE(device_creator_samples_device, "samples", "Samples");


        class device_sound_interface_samples : device_sound_interface
        {
            public device_sound_interface_samples(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples) { ((samples_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs, samples); }
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
            public ListBase<int16_t> source = new ListBase<int16_t>();  //const int16_t *   source;
            public int32_t source_length;
            public int32_t source_num;
            public uint32_t pos;
            public uint32_t frac;
            public uint32_t step;
            //uint32_t          basefreq;
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
        }


        // configuration helpers
        //void set_channels(uint8_t channels) { m_channels = channels; }
        //void set_samples_names(const char *const *names) { m_names = names; }

        // start callback helpers
        //void set_samples_start_callback(start_cb_delegate callback) { m_samples_start_cb = callback; }
        //template <class FunctionClass> void set_samples_start_callback(const char *devname, void (FunctionClass::*callback)(), const char *name)
        //{
        //    set_samples_start_callback(start_cb_delegate(callback, name, devname, static_cast<FunctionClass *>(nullptr)));
        //}
        //template <class FunctionClass> void set_samples_start_callback(void (FunctionClass::*callback)(), const char *name)
        //{
        //    set_samples_start_callback(start_cb_delegate(callback, name, nullptr, static_cast<FunctionClass *>(nullptr)));
        //}


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
            assert(channel < m_channels);

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
            RawBuffer buf = new RawBuffer(4);  //uint8_t buf[4];
            uint32_t offset = file.read(new ListBytesPointer(buf), 4);
            if (offset < 4)
            {
                osd_printf_warning("Unable to read {0}, 0-byte file?\n", file.filename());
                return false;
            }

            // look for the appropriate RIFF tag
            if (buf[0] == 'R' && buf[1] == 'I' && buf[2] == 'F' && buf[3] == 'F')  // memcmp(&buf[0], "RIFF", 4) == 0)
                return read_wav_sample(file, sample);
            else if (buf[0] == 'f' && buf[1] == 'L' && buf[2] == 'a' && buf[3] == 'C')  // memcmp(&buf[0], "fLaC", 4) == 0)
                return read_flac_sample(file, sample);

            // if nothing appropriate, emit a warning
            osd_printf_warning("Unable to read {0}, corrupt file?\n", file.filename());

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
                chan.stream = m_disound.stream_alloc(0, 1, machine().sample_rate());
                chan.source = null;
                chan.source_num = -1;
                chan.step = 0;
                chan.loop = false;
                chan.paused = false;

                // register with the save state system
                save_item(chan.source_length, "chan.source_length", channel);
                save_item(chan.source_num, "chan.source_num", channel);
                save_item(chan.pos, "chan.pos", channel);
                save_item(chan.frac, "chan.frac", channel);
                save_item(chan.step, "chan.step", channel);
                save_item(chan.loop, "chan.loop", channel);
                save_item(chan.paused, "chan.paused", channel);
            }

            // initialize any custom handlers
            //m_samples_start_cb.bind_relative_to(owner());
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
                    chan.source = sample.data;
                    chan.source_length = sample.data.Count;
                    if (sample.data.Count == 0)
                        chan.source_num = -1;
                }

                // validate the position against the length in case the sample is smaller
                if (chan.source != null && chan.pos >= chan.source_length)
                {
                    if (chan.loop)
                        chan.pos %= (UInt32)chan.source_length;
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
        void device_sound_interface_sound_stream_update(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples)
        {
            // find the channel with this stream
            for (int channel = 0; channel < m_channels; channel++)
            {
                if (stream == m_channel[channel].stream)
                {
                    channel_t chan = m_channel[channel];
                    ListPointer<stream_sample_t> buffer = new ListPointer<stream_sample_t>(outputs[0]);  //stream_sample_t * buffer = outputs[0];

                    // process if we still have a source and we're not paused
                    if (chan.source != null && !chan.paused)
                    {
                        // load some info locally
                        uint32_t pos = chan.pos;
                        uint32_t frac = chan.frac;
                        uint32_t step = chan.step;
                        ListBase<int16_t> sample = chan.source;  //const int16_t *sample = chan.source;
                        uint32_t sample_length = (UInt32)chan.source_length;

                        while (samples-- != 0)
                        {
                            // do a linear interp on the sample
                            int sample1 = sample[(int)pos];
                            int sample2 = sample[(int)((pos + 1) % sample_length)];
                            int fracmult = (int)(frac >> (FRAC_BITS - 14));
                            buffer[0] = ((0x4000 - fracmult) * sample1 + fracmult * sample2) >> 14;  //*buffer++ = ((0x4000 - fracmult) * sample1 + fracmult * sample2) >> 14;
                            buffer++;

                            // advance
                            frac += step;
                            pos += frac >> FRAC_BITS;
                            frac = frac & ((1 << FRAC_BITS) - 1);

                            // handle looping/ending
                            if (pos >= sample_length)
                            {
                                if (chan.loop)
                                {
                                    pos %= sample_length;
                                }
                                else
                                {
                                    chan.source = null;
                                    chan.source_num = -1;
                                    if (samples > 0)
                                        memset(buffer, 0, (UInt32)samples);  //memset(buffer, 0, samples * sizeof(*buffer));

                                    break;
                                }
                            }
                        }

                        // push position back out
                        chan.pos = pos;
                        chan.frac = frac;
                    }
                    else
                    {
                        memset(buffer, 0, (UInt32)samples);  //memset(buffer, 0, samples * sizeof(*buffer));
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

            RawBuffer filesizeBuffer = new RawBuffer(4);
            offset += file.read(new ListBytesPointer(filesizeBuffer), 4);
            if (offset < 8)
            {
                osd_printf_warning("Unexpected size offset {0} ({1})\n", offset, file.filename());
                return false;
            }

            filesize = filesizeBuffer.get_uint32();
            filesize = little_endianize_int32(filesize);

            // read the RIFF file type and make sure it's a WAVE file
            RawBuffer buf = new RawBuffer(32);  //char [] buf = new char[32];
            offset += file.read(new ListBytesPointer(buf), 4);
            if (offset < 12)
            {
                osd_printf_warning("Unexpected WAVE offset {0} ({1})\n", offset, file.filename());
                return false;
            }

            if (!(buf[0] == 'W' && buf[1] == 'A' && buf[2] == 'V' && buf[3] == 'E'))  // memcmp(&buf[0], "WAVE", 4) != 0)
            {
                osd_printf_warning("Could not find WAVE header ({0})\n", file.filename());
                return false;
            }

            // seek until we find a format tag
            uint32_t length;
            RawBuffer lengthBuffer = new RawBuffer(4);
            while (true)
            {
                offset += file.read(new ListBytesPointer(buf), 4);
                offset += file.read(new ListBytesPointer(lengthBuffer), 4);
                length = lengthBuffer.get_uint32();
                length = little_endianize_int32(length);
                if (buf[0] == 'f' && buf[1] == 'm' && buf[2] == 't' && buf[3] == ' ')  //if (memcmp(&buf[0], "fmt ", 4) == 0)
                    break;

                // seek to the next block
                file.seek(length, emu_file.SEEK_CUR);
                offset += length;
                if (offset >= filesize)
                {
                    osd_printf_warning("Could not find fmt tag ({0})\n", file.filename());
                    return false;
                }
            }

            // read the format -- make sure it is PCM
            uint16_t temp16;
            RawBuffer temp16Buffer = new RawBuffer(2);
            offset += file.read(new ListBytesPointer(temp16Buffer), 2);
            temp16 = temp16Buffer.get_uint16();
            temp16 = little_endianize_int16(temp16);
            if (temp16 != 1)
            {
                osd_printf_warning("unsupported format {0} - only PCM is supported ({1})\n", temp16, file.filename());
                return false;
            }

            // number of channels -- only mono is supported
            offset += file.read(new ListBytesPointer(temp16Buffer), 2);
            temp16 = temp16Buffer.get_uint16();
            temp16 = little_endianize_int16(temp16);
            if (temp16 != 1)
            {
                osd_printf_warning("unsupported number of channels {0} - only mono is supported ({1})\n", temp16, file.filename());
                return false;
            }

            // sample rate
            uint32_t rate;
            RawBuffer rateBuffer = new RawBuffer(4);
            offset += file.read(new ListBytesPointer(rateBuffer), 4);
            rate = rateBuffer.get_uint32();
            rate = little_endianize_int32(rate);

            // bytes/second and block alignment are ignored
            offset += file.read(new ListBytesPointer(buf), 6);

            // bits/sample
            uint16_t bits;
            RawBuffer bitsBuffer = new RawBuffer(2);
            offset += file.read(new ListBytesPointer(bitsBuffer), 2);
            bits = bitsBuffer.get_uint16();
            bits = little_endianize_int16(bits);
            if (bits != 8 && bits != 16)
            {
                osd_printf_warning("unsupported bits/sample {0} - only 8 and 16 are supported ({1})\n", bits, file.filename());
                return false;
            }

            // seek past any extra data
            file.seek(length - 16, emu_file.SEEK_CUR);
            offset += length - 16;

            // seek until we find a data tag
            while (true)
            {
                offset += file.read(new ListBytesPointer(buf), 4);
                offset += file.read(new ListBytesPointer(lengthBuffer), 4);
                length = lengthBuffer.get_uint32();
                length = little_endianize_int32(length);
                if (buf[0] == 'd' && buf[1] == 'a' && buf[2] == 't' && buf[3] == 'a')  //if (memcmp(&buf[0], "data", 4) == 0)
                    break;

                // seek to the next block
                file.seek(length, emu_file.SEEK_CUR);
                offset += length;
                if (offset >= filesize)
                {
                    osd_printf_warning("Could not find data tag ({0})\n", file.filename());
                    return false;
                }
            }

            // if there was a 0 length data block, we're done
            if (length == 0)
            {
                osd_printf_warning("empty data block ({0})\n", file.filename());
                return false;
            }

            // fill in the sample data
            sample.frequency = rate;

            // read the data in
            if (bits == 8)
            {
                sample.data.resize((int)length);
                RawBuffer sample_data_8bit = new RawBuffer(length);
                file.read(new ListBytesPointer(sample_data_8bit), length);

                // convert 8-bit data to signed samples
                ListBytesPointer tempptr = new ListBytesPointer(sample_data_8bit);  //uint8_t *tempptr = reinterpret_cast<uint8_t *>(&sample.data[0]);
                for (int sindex = (int)length - 1; sindex >= 0; sindex--)
                    sample.data[sindex] = (Int16)((sbyte)(tempptr[sindex] ^ 0x80) * 256);
            }
            else
            {
                // 16-bit data is fine as-is
                sample.data.resize((int)length / 2);
                RawBuffer sample_data_8bit = new RawBuffer(length);
                file.read(new ListBytesPointer(sample_data_8bit), length);

                // swap high/low on big-endian systems
                if (ENDIANNESS_NATIVE != endianness_t.ENDIANNESS_LITTLE)
                {
                    for (UInt32 sindex = 0; sindex < length / 2; sindex++)
                        sample.data[sindex] = (Int16)little_endianize_int16(sample_data_8bit.get_uint16((int)sindex));  //sample.data[sindex]);
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
            sample.data.resize((int)decoder.total_samples());
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
            m_sample.resize(iter.count());

            // load the samples
            int index = 0;
            for (string samplename = iter.first(); samplename != null; index++, samplename = iter.next())
            {
                // attempt to open as FLAC first
                emu_file file = new emu_file(machine().options().sample_path(), OPEN_FLAG_READ);
                osd_file.error filerr = file.open(basename, PATH_SEPARATOR, samplename, ".flac");
                if (filerr != osd_file.error.NONE && altbasename != null)
                    filerr = file.open(altbasename, PATH_SEPARATOR, samplename, ".flac");

                // if not, try as WAV
                if (filerr != osd_file.error.NONE)
                    filerr = file.open(basename, PATH_SEPARATOR, samplename, ".wav");
                if (filerr != osd_file.error.NONE && altbasename != null)
                    filerr = file.open(altbasename, PATH_SEPARATOR, samplename, ".wav");

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
    //typedef device_type_iterator<&device_creator<samples_device>, samples_device> samples_device_iterator;
    public class samples_device_iterator : device_type_iterator<samples_device>
    {
        public samples_device_iterator(device_t root, int maxdepth = 255) : base(samples_device.SAMPLES, root, maxdepth) { }
    }


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
