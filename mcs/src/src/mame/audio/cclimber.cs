// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using int16_t = System.Int16;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.ay8910_global;
using static mame.cclimber_global;
using static mame.device_global;
using static mame.disound_global;
using static mame.emucore_global;
using static mame.samples_global;


namespace mame
{
    // ======================> cclimber_audio_device
    class cclimber_audio_device : device_t
    {
        //DEFINE_DEVICE_TYPE(CCLIMBER_AUDIO, cclimber_audio_device, "cclimber_audio", "Crazy Climber Sound Board")
        public static readonly emu.detail.device_type_impl CCLIMBER_AUDIO = DEFINE_DEVICE_TYPE("cclimber_audio", "Crazy Climber Sound Board", (type, mconfig, tag, owner, clock) => { return new cclimber_audio_device(mconfig, tag, owner, clock); });


        static int SAMPLE_CONV4(int a) { return 0x1111 * (a & 0x0f) - 0x8000; }

        const uint32_t SND_CLOCK = 3072000;   /* 3.072 MHz */


        MemoryContainer<int16_t> m_sample_buf;  //std::unique_ptr<int16_t[]> m_sample_buf;    // buffer to decode samples at run time
        uint8_t m_sample_num;
        uint32_t m_sample_freq;
        uint8_t m_sample_volume;
        required_device<samples_device> m_samples;
        required_region_ptr<uint8_t> m_samples_region;


        // construction/destruction
        cclimber_audio_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, CCLIMBER_AUDIO, tag, owner, clock)
        {
            m_sample_buf = null;
            m_sample_num = 0;
            m_sample_freq = 0;
            m_sample_volume = 0;
            m_samples = new required_device<samples_device>(this, "samples");
            m_samples_region = new required_region_ptr<uint8_t>(this, "samples");
        }


        public void sample_trigger(int state)
        {
            if (state == 0)
                return;

            play_sample(32 * m_sample_num, (int)m_sample_freq, m_sample_volume);
        }


        //void sample_trigger_w(uint8_t data);


        public void sample_rate_w(uint8_t data)
        {
            // calculate the sampling frequency
            m_sample_freq = SND_CLOCK / 4 / (256 - (uint32_t)data);
        }


        public void sample_volume_w(uint8_t data)
        {
            m_sample_volume = (uint8_t)(data & 0x1f);    // range 0-31
        }


        // device level overrides
        protected override void device_start()
        {
            save_item(NAME(new { m_sample_num }));
            save_item(NAME(new { m_sample_freq }));
            save_item(NAME(new { m_sample_volume }));
        }


        protected override void device_add_mconfig(machine_config config)
        {
            ay8910_device aysnd = AY8910(config, "aysnd", SND_CLOCK / 2);
            aysnd.port_a_write_callback().set(sample_select_w).reg();
            aysnd.add_route(ALL_OUTPUTS, ":speaker", 0.5);

            SAMPLES(config, m_samples);
            m_samples.op0.set_channels(1);
            m_samples.op0.set_samples_start_callback(sh_start);
            m_samples.op0.disound.add_route(ALL_OUTPUTS, ":speaker", 0.5);
        }


        void play_sample(int start, int freq, int volume)
        {
            int romlen = (int)m_samples_region.bytes();

            // decode the ROM samples
            int len = 0;
            while (start + len < romlen && m_samples_region.op[start + len] != 0x70)
            {
                int sample;

                sample = (m_samples_region.op[start + len] & 0xf0) >> 4;
                m_sample_buf[2 * len] = (int16_t)(SAMPLE_CONV4(sample) * volume / 31);

                sample = m_samples_region.op[start + len] & 0x0f;
                m_sample_buf[2 * len + 1] = (int16_t)(SAMPLE_CONV4(sample) * volume / 31);

                len++;
            }

            m_samples.op0.start_raw(0, new Pointer<int16_t>(m_sample_buf), 2 * (uint32_t)len, (uint32_t)freq);
        }


        void sample_select_w(uint8_t data)
        {
            m_sample_num = data;
        }


        //SAMPLES_START_CB_MEMBER( sh_start );
        void sh_start()
        {
            m_sample_buf = new MemoryContainer<int16_t>((int)(2 * m_samples_region.bytes()), true);  //m_sample_buf = std::make_unique<int16_t[]>(2 * m_samples_region.bytes());

            //throw new emu_unimplemented();
#if false
            save_pointer(NAME(m_sample_buf), 2 * m_samples_region.bytes());
#endif
        }
    }


    static class cclimber_global
    {
        public static cclimber_audio_device CCLIMBER_AUDIO(machine_config mconfig, string tag, uint32_t clock) { return emu.detail.device_type_impl.op<cclimber_audio_device>(mconfig, tag, cclimber_audio_device.CCLIMBER_AUDIO, clock); }
    }
}
