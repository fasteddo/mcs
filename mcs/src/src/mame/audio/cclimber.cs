// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using int16_t = System.Int16;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.ay8910_global;
using static mame.cclimber_global;
using static mame.cpp_global;
using static mame.dac_global;
using static mame.device_global;
using static mame.disound_global;
using static mame.emucore_global;
using static mame.flt_vol_global;
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


        required_device<dac_4bit_r2r_device> m_dac;
        required_device<filter_volume_device> m_volume;
        required_region_ptr<u8> m_rom;

        u8 m_sample_clockdiv;

        emu_timer m_sample_timer;

        u16 m_address;
        u8 m_start_address;
        u8 m_loop_address;
        u8 m_sample_rate;
        int m_sample_trigger;


        // construction/destruction
        cclimber_audio_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, CCLIMBER_AUDIO, tag, owner, clock)
        {
            m_dac = new required_device<dac_4bit_r2r_device>(this, "dac");
            m_volume = new required_device<filter_volume_device>(this, "volume");
            m_rom = new required_region_ptr<u8>(this, "samples");
            m_sample_clockdiv = 2;
        }


        //auto &set_sample_clockdiv(u8 div) { m_sample_clockdiv = div; return *this; } // determines base sound pitch (default 2)


        public void sample_trigger(int state)
        {
            // start playing on rising edge
            if (state != 0 && m_sample_trigger == 0)
                m_address = (u16)(m_start_address * 64);

            m_sample_trigger = state;
        }


        //void sample_trigger_w(u8 data);


        public void sample_rate_w(u8 data)
        {
            m_sample_rate = data;
        }


        public void sample_volume_w(u8 data)
        {
            m_volume.op0.flt_volume_set_volume((float)((double)(data & 0x1f) / 31.0)); // range 0-31
        }


        // device level overrides
        protected override void device_start()
        {
            assert(m_rom.bytes() == 0x2000);

            m_address = 0;
            m_start_address = 0;
            m_loop_address = 0;
            m_sample_rate = 0;
            m_sample_trigger = 0;

            m_sample_timer = timer_alloc(sample_tick);
            m_sample_timer.adjust(attotime.zero);

            // register for savestates
            save_item(NAME(m_address));
            save_item(NAME(m_start_address));
            save_item(NAME(m_loop_address));
            save_item(NAME(m_sample_rate));
            save_item(NAME(m_sample_trigger));
        }


        protected override void device_reset() { sample_volume_w(0); }


        protected override void device_add_mconfig(machine_config config)
        {
            ay8910_device aysnd = AY8910(config, "aysnd", DERIVED_CLOCK(1, 1));
            aysnd.port_a_write_callback().set(start_address_w).reg();
            aysnd.port_b_write_callback().set(loop_address_w).reg();
            aysnd.add_route(ALL_OUTPUTS, ":speaker", 0.5);

            DAC_4BIT_R2R(config, m_dac).disound.add_route(ALL_OUTPUTS, "volume", 0.5);

            FILTER_VOLUME(config, m_volume).disound.add_route(ALL_OUTPUTS, ":speaker", 1.0);
        }


        void start_address_w(u8 data) { m_start_address = data; }
        void loop_address_w(u8 data) { m_loop_address = data; }


        //TIMER_CALLBACK_MEMBER(sample_tick);
        void sample_tick(s32 param) { throw new emu_unimplemented(); }
    }


    static class cclimber_global
    {
        public static cclimber_audio_device CCLIMBER_AUDIO(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<cclimber_audio_device>(mconfig, tag, cclimber_audio_device.CCLIMBER_AUDIO, clock); }
    }
}
