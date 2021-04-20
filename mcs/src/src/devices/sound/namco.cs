// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int16_t = System.Int16;
using int32_t = System.Int32;
using offs_t = System.UInt32;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    public static class namco_global
    {
        /* quality parameter: internal sample rate is 192 KHz, output is 48 KHz */
        public const int INTERNAL_RATE = 192000;

        /* 16 bits: sample bits of the stream buffer    */
        /* 4 bits:  volume                  */
        /* 4 bits:  prom sample bits            */
        public const int MIXLEVEL = 1 << (16 - 4 - 4);
    }


    public class namco_audio_device : device_t
                                      //device_sound_interface
    {
        public class device_sound_interface_namco_audio : device_sound_interface
        {
            public device_sound_interface_namco_audio(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((namco_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        /* this structure defines the parameters for a channel */
        public class sound_channel
        {
            public uint32_t frequency;
            public uint32_t counter;
            public int32_t [] volume = new int32_t[2];
            public int32_t noise_sw;
            public int32_t noise_state;
            public int32_t noise_seed;
            public uint32_t noise_counter;
            public int32_t noise_hold;
            public int32_t waveform_select;
        }


        const UInt32 MAX_VOICES = 8;
        const UInt32 MAX_VOLUME = 16;


        /* stream output level */
        int16_t OUTPUT_LEVEL(int n) { return (int16_t)(n * namco_global.MIXLEVEL / m_voices); }

        /* a position of waveform sample */
        int WAVEFORM_POSITION(int n) { return (n >> m_f_fracbits) & 0x1f; }


        device_sound_interface_namco_audio m_disound;

        /* waveform region */
        optional_region_ptr_uint8_t m_wave_ptr;

        /* data about the sound system */
        protected sound_channel [] m_channel_list = new sound_channel[MAX_VOICES];
        sound_channel m_last_channel;
        protected uint8_t [] m_soundregs;  //uint8_t *m_soundregs;
        Pointer<uint8_t> m_wavedata;  //uint8_t *m_wavedata;

        /* global sound parameters */
        int m_wave_size;
        bool m_sound_enable;
        protected sound_stream m_stream;
        int m_namco_clock;
        int m_sample_rate;
        int m_f_fracbits;

        protected int m_voices;     /* number of voices */
        bool m_stereo;     /* set to indicate stereo (e.g., System 1) */

        /* decoded waveform table */
        MemoryContainer<int16_t> [] m_waveform = new MemoryContainer<int16_t>[MAX_VOLUME];  //int16_t *m_waveform[MAX_VOLUME];


        public namco_audio_device(machine_config mconfig, device_type type, string tag, device_t owner, UInt32 clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_namco_audio(mconfig, this));  //device_sound_interface(mconfig, *this),
            m_disound = GetClassInterface<device_sound_interface_namco_audio>();

            m_wave_ptr = new optional_region_ptr_uint8_t(this, DEVICE_SELF);
            m_last_channel = null;
            m_soundregs = null;
            m_wavedata = null;
            m_wave_size = 0;
            m_sound_enable = false;
            m_stream = null;
            m_namco_clock = 0;
            m_sample_rate = 0;
            m_f_fracbits = 0;
            m_voices = 0;
            m_stereo = false;

            for (int i = 0; i < m_channel_list.Length; i++)
                m_channel_list[i] = new sound_channel();
        }


        public device_sound_interface_namco_audio disound { get { return m_disound; } }


        // static configuration
        public void set_voices(int voices) { m_voices = voices; }
        //void set_stereo(bool stereo) { m_stereo = stereo; }


        public void sound_enable_w(int state)
        {
            m_sound_enable = state != 0;
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            int voiceIdx;  // sound_channel *voice;

            /* extract globals from the interface */
            m_last_channel = m_channel_list[m_voices];

            m_soundregs = new uint8_t[0x400];  //auto_alloc_array_clear(machine(), uint8_t, 0x400);

            /* build the waveform table */
            build_decoded_waveform(m_wave_ptr.target);

            /* get stream channels */
            if (m_stereo)
                m_stream = m_disound.stream_alloc(0, 2, 192000);
            else
                m_stream = m_disound.stream_alloc(0, 1, 192000);

            /* start with sound enabled, many games don't have a sound enable register */
            m_sound_enable = true;

            //throw new emu_unimplemented();
#if false
            /* register with the save state system */
            save_pointer(m_soundregs, "m_soundregs", 0x400);

            if (m_wave_ptr == null)
                save_pointer(m_wavedata, "m_wavedata", 0x400);

            save_item(m_voices, "m_voices");
            save_item(m_sound_enable, "m_sound_enable");
            save_pointer(NAME(m_waveform[0]), MAX_VOLUME * 32 * 8 * (1 + m_wave_size));
#endif

            /* reset all the voices */
            for (voiceIdx = 0; m_channel_list[voiceIdx] != m_last_channel; voiceIdx++)  //for (voice = m_channel_list; voice < m_last_channel; voice++)
            {
                sound_channel voice = m_channel_list[voiceIdx];

                int voicenum = voiceIdx;  // voice - m_channel_list;

                voice.frequency = 0;
                voice.volume[0] = voice.volume[1] = 0;
                voice.waveform_select = 0;
                voice.counter = 0;
                voice.noise_sw = 0;
                voice.noise_state = 0;
                voice.noise_seed = 1;
                voice.noise_counter = 0;
                voice.noise_hold = 0;

                /* register with the save state system */
                save_item(NAME(new { voice.frequency }), voicenum);
                save_item(NAME(new { voice.counter }), voicenum);
                save_item(NAME(new { voice.volume }), voicenum);
                save_item(NAME(new { voice.noise_sw }), voicenum);
                save_item(NAME(new { voice.noise_state }), voicenum);
                save_item(NAME(new { voice.noise_seed }), voicenum);
                save_item(NAME(new { voice.noise_hold }), voicenum);
                save_item(NAME(new { voice.noise_counter }), voicenum);
                save_item(NAME(new { voice.waveform_select }), voicenum);
            }
        }


        protected override void device_clock_changed()
        {
            int clock_multiple;

            /* adjust internal clock */
            m_namco_clock = (int)clock();
            for (clock_multiple = 0; m_namco_clock < namco_global.INTERNAL_RATE; clock_multiple++)
                m_namco_clock *= 2;

            m_f_fracbits = clock_multiple + 15;

            /* adjust output clock */
            m_sample_rate = m_namco_clock;

            logerror("Namco: freq fractional bits = {0}: internal freq = {1}, output freq = {2}\n", m_f_fracbits, m_namco_clock, m_sample_rate);

            m_stream.set_sample_rate((u32)m_sample_rate);
        }


        /* build the decoded waveform table */
        void build_decoded_waveform(Pointer<uint8_t> rgnbase)  //uint8_t *rgnbase)
        {
            MemoryContainer<int16_t> p;  //int16_t *p;
            int size;
            int offset;
            int v;

            m_wavedata = (rgnbase != null) ? new Pointer<uint8_t>(rgnbase) : new Pointer<uint8_t>(auto_alloc_array_clear<uint8_t>(machine(), 0x400));  //m_wavedata = (rgnbase != nullptr) ? rgnbase : auto_alloc_array_clear(machine(), uint8_t, 0x400);

            /* 20pacgal has waves in RAM but old sound system */
            if (rgnbase == null && m_voices != 3)
            {
                m_wave_size = 1;
                size = 32 * 16;     /* 32 samples, 16 waveforms */
            }
            else
            {
                m_wave_size = 0;
                size = 32 * 8;      /* 32 samples, 8 waveforms */
            }

            //p = auto_alloc_array(machine(), int16_t, size * MAX_VOLUME);

            for (v = 0; v < MAX_VOLUME; v++)
            {
                p = auto_alloc_array<int16_t>(machine(), (UInt32)size * MAX_VOLUME);  //p = auto_alloc_array(machine(), int16_t, size * MAX_VOLUME);
                m_waveform[v] = p;
                //p += size;
            }

            /* We need waveform data. It fails if region is not specified. */
            if (m_wavedata != null)
            {
                for (offset = 0; offset < 256; offset++)
                    update_namco_waveform(offset, m_wavedata[offset]);
            }
        }

        /* update the decoded waveform data */
        void update_namco_waveform(int offset, byte data)
        {
            if (m_wave_size == 1)
            {
                int16_t wdata;
                int v;

                /* use full byte, first 4 high bits, then low 4 bits */
                for (v = 0; v < MAX_VOLUME; v++)
                {
                    wdata = (int16_t)(((data >> 4) & 0x0f) - 8);
                    m_waveform[v][offset * 2] = OUTPUT_LEVEL(wdata * v);
                    wdata = (int16_t)((data & 0x0f) - 8);
                    m_waveform[v][offset * 2 + 1] = OUTPUT_LEVEL(wdata * v);
                }
            }
            else
            {
                int v;

                /* use only low 4 bits */
                for (v = 0; v < MAX_VOLUME; v++)
                    m_waveform[v][offset] = OUTPUT_LEVEL(((data & 0x0f) - 8) * v);
            }
        }

        /* generate sound by oversampling */
        uint32_t namco_update_one(write_stream_view buffer, Pointer<int16_t> wave, uint32_t counter, uint32_t freq)  //uint32_t namco_update_one(write_stream_view &buffer, const int16_t *wave, uint32_t counter, uint32_t freq);
        {
            for (int sampindex = 0; sampindex < buffer.samples(); sampindex++)
            {
                buffer.add_int(sampindex, wave[WAVEFORM_POSITION((int)counter)], 32768);
                counter += freq;
            }

            return counter;
        }


        // device_sound_interface overrides
        //-------------------------------------------------
        //  sound_stream_update - handle a stream update
        //-------------------------------------------------
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            if (m_stereo)
            {
                /* zap the contents of the buffers */
                outputs[0].fill(0);
                outputs[1].fill(0);

                /* if no sound, we're done */
                if (!m_sound_enable)
                    return;

                /* loop over each voice and add its contribution */
                for (int voiceIdx = 0; m_channel_list[voiceIdx] != m_last_channel; voiceIdx++)  //for (sound_channel *voice = m_channel_list; voice < m_last_channel; voice++)
                {
                    sound_channel voice = m_channel_list[voiceIdx];

                    var lmix = outputs[0];
                    var rmix = outputs[1];
                    int lv = voice.volume[0];
                    int rv = voice.volume[1];

                    if (voice.noise_sw != 0)
                    {
                        int f = (int)(voice.frequency & 0xff);

                        /* only update if we have non-zero volume and frequency */
                        if ((lv != 0 || rv != 0) && f != 0)
                        {
                            int hold_time = 1 << (m_f_fracbits - 16);
                            int hold = voice.noise_hold;
                            uint32_t delta = (uint32_t)(f << 4);
                            uint32_t c = voice.noise_counter;
                            int16_t l_noise_data = OUTPUT_LEVEL(0x07 * (lv >> 1));
                            int16_t r_noise_data = OUTPUT_LEVEL(0x07 * (rv >> 1));
                            int i;

                            /* add our contribution */
                            for (i = 0; i < lmix.samples(); i++)
                            {
                                int cnt;

                                if (voice.noise_state != 0)
                                {
                                    lmix.add_int(i, l_noise_data, 32768);
                                    rmix.add_int(i, r_noise_data, 32768);
                                }
                                else
                                {
                                    lmix.add_int(i, -l_noise_data, 32768);
                                    rmix.add_int(i, -r_noise_data, 32768);
                                }

                                if (hold != 0)
                                {
                                    hold--;
                                    continue;
                                }

                                hold =  hold_time;

                                c += delta;
                                cnt = (int)(c >> 12);
                                c &= (1 << 12) - 1;
                                for ( ; cnt > 0; cnt--)
                                {
                                    if (((voice.noise_seed + 1) & 2) != 0) voice.noise_state ^= 1;
                                    if ((voice.noise_seed & 1) != 0) voice.noise_seed ^= 0x28000;
                                    voice.noise_seed >>= 1;
                                }
                            }

                            /* update the counter and hold time for this voice */
                            voice.noise_counter = c;
                            voice.noise_hold = hold;
                        }
                    }
                    else
                    {
                        /* only update if we have non-zero frequency */
                        if (voice.frequency != 0)
                        {
                            /* save the counter for this voice */
                            uint32_t c = voice.counter;

                            /* only update if we have non-zero left volume */
                            if (lv != 0)
                            {
                                Pointer<int16_t> lw = new Pointer<int16_t>(m_waveform[lv], voice.waveform_select * 32);  //const int16_t *lw = &m_waveform[lv][voice->waveform_select * 32];

                                /* generate sound into the buffer */
                                c = namco_update_one(lmix, lw, voice.counter, voice.frequency);
                            }

                            /* only update if we have non-zero right volume */
                            if (rv != 0)
                            {
                                Pointer<int16_t> rw = new Pointer<int16_t>(m_waveform[rv], voice.waveform_select * 32);  //const int16_t *rw = &m_waveform[rv][voice->waveform_select * 32];

                                /* generate sound into the buffer */
                                c = namco_update_one(rmix, rw, voice.counter, voice.frequency);
                            }

                            /* update the counter for this voice */
                            voice.counter = c;
                        }
                    }
                }
            }
            else
            {
                int voiceIdx;  // sound_channel *voice;

                var buffer = outputs[0];

                /* zap the contents of the buffer */
                buffer.fill(0);

                /* if no sound, we're done */
                if (!m_sound_enable)
                    return;

                /* loop over each voice and add its contribution */
                for (voiceIdx = 0; m_channel_list[voiceIdx] != m_last_channel; voiceIdx++)  //for (voice = m_channel_list; voice < m_last_channel; voice++)
                {
                    sound_channel voice = m_channel_list[voiceIdx];

                    int v = voice.volume[0];
                    if (voice.noise_sw != 0)
                    {
                        int f = (int)(voice.frequency & 0xff);

                        /* only update if we have non-zero volume and frequency */
                        if (v != 0 && f != 0)
                        {
                            int hold_time = 1 << (m_f_fracbits - 16);
                            int hold = voice.noise_hold;
                            uint32_t delta = (uint32_t)(f << 4);
                            uint32_t c = voice.noise_counter;
                            int16_t noise_data = OUTPUT_LEVEL(0x07 * (v >> 1));
                            int i;

                            /* add our contribution */
                            for (i = 0; i < buffer.samples(); i++)
                            {
                                int cnt;

                                if (voice.noise_state != 0)
                                {
                                    buffer.add_int(i, noise_data, 32768);
                                }
                                else
                                {
                                    buffer.add_int(i, -noise_data, 32768);
                                }

                                if (hold != 0)
                                {
                                    hold--;
                                    continue;
                                }

                                hold =  hold_time;

                                c += delta;
                                cnt = (int)(c >> 12);
                                c &= (1 << 12) - 1;
                                for( ;cnt > 0; cnt--)
                                {
                                    if (((voice.noise_seed + 1) & 2) != 0) voice.noise_state ^= 1;
                                    if ((voice.noise_seed & 1) != 0) voice.noise_seed ^= 0x28000;
                                    voice.noise_seed >>= 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        /* only update if we have non-zero volume and frequency */
                        if (v != 0 && voice.frequency != 0)
                        {
                            Pointer<int16_t> w = new Pointer<int16_t>(m_waveform[v], voice.waveform_select * 32);  //const int16_t *w = &m_waveform[v][voice->waveform_select * 32];

                            /* generate sound into buffer and update the counter for this voice */
                            voice.counter = namco_update_one(buffer, w, voice.counter, voice.frequency);
                        }
                    }
                }
            }
        }
    }


    public class namco_device : namco_audio_device
    {
        //DEFINE_DEVICE_TYPE(NAMCO,       namco_device,       "namco",       "Namco")
        static device_t device_creator_namco_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new namco_device(mconfig, tag, owner, clock); }
        public static readonly device_type NAMCO = DEFINE_DEVICE_TYPE(device_creator_namco_device, "namco",       "Namco");


        public namco_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NAMCO, tag, owner, clock)
        { }


        public void pacman_sound_w(offs_t offset, uint8_t data)
        {
            sound_channel voice;
            int ch;

            data &= 0x0f;
            if (m_soundregs[offset] == data)
                return;

            /* update the streams */
            m_stream.update();

            /* set the register */
            m_soundregs[offset] = data;

            if (offset < 0x10)
                ch = (int)((offset - 5) / 5);
            else if (offset == 0x10)
                ch = 0;
            else
                ch = (int)((offset - 0x11) / 5);

            if (ch >= m_voices)
                return;

            /* recompute the voice parameters */
            voice = m_channel_list[ch]; // m_channel_list + ch;
            switch (offset - ch * 5)
            {
                case 0x05:
                    voice.waveform_select = data & 7;
                    break;

                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                case 0x14:
                    /* the frequency has 20 bits */
                    /* the first voice has extra frequency bits */
                    voice.frequency = (ch == 0) ? (uint32_t)m_soundregs[0x10] : 0;
                    voice.frequency += (uint32_t)(m_soundregs[ch * 5 + 0x11] << 4);
                    voice.frequency += (uint32_t)(m_soundregs[ch * 5 + 0x12] << 8);
                    voice.frequency += (uint32_t)(m_soundregs[ch * 5 + 0x13] << 12);
                    voice.frequency += (uint32_t)(m_soundregs[ch * 5 + 0x14] << 16); /* always 0 */
                    break;

                case 0x15:
                    voice.volume[0] = data;
                    break;
            }
        }


        //void polepos_sound_enable(int enable);

        //uint8_t polepos_sound_r(offs_t offset);
        //void polepos_sound_w(offs_t offset, uint8_t data);


        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            throw new emu_unimplemented();
        }
    }


    //class namco_15xx_device : public namco_audio_device

    //class namco_cus30_device : public namco_audio_device
}
