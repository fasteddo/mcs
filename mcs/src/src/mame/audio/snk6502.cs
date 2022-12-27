// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using int32_t = System.Int32;
using required_memory_region = mame.memory_region_finder<mame.bool_const_true>;  //using required_memory_region = memory_region_finder<true>;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.disound_global;
using static mame.rescap_global;
using static mame.samples_global;
using static mame.sn76477_global;
using static mame.snk6502_global;
using static mame.speaker_global;


namespace mame
{
    class snk6502_sound_device : device_t
                                 //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(SNK6502_SOUND,  snk6502_sound_device,  "snk6502_sound",  "SNK6502 Custom Sound")
        public static readonly emu.detail.device_type_impl SNK6502_SOUND = DEFINE_DEVICE_TYPE("snk6502_sound", "SNK6502 Custom Sound", (type, mconfig, tag, owner, clock) => { return new snk6502_sound_device(mconfig, tag, owner, clock); });


        public class device_sound_interface_snk6502 : device_sound_interface
        {
            public device_sound_interface_snk6502(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((snk6502_sound_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }
        }


        device_sound_interface_snk6502 m_disound;


        //static constexpr unsigned NUM_CHANNELS = 3;

        //struct tone_t
        //{
        //    int mute;
        //    int offset;
        //    int base;
        //    int mask;
        //    int32_t   sample_rate;
        //    int32_t   sample_step;
        //    int32_t   sample_cur;
        //    int16_t   form[16];
        //};

        // internal state
        //tone_t m_tone_channels[NUM_CHANNELS];
        int32_t m_tone_clock_expire;
        int32_t m_tone_clock;
        sound_stream m_tone_stream;

        optional_device<samples_device> m_samples;
        required_memory_region m_rom;
        int m_sound0_stop_on_rollover;

        int m_hd38880_cmd;
        uint32_t m_hd38880_addr;
        int m_hd38880_data_bytes;
        double m_hd38880_speed;


        snk6502_sound_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, SNK6502_SOUND, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_snk6502(mconfig, this));  //device_sound_interface(mconfig, *this);
            m_disound = GetClassInterface<device_sound_interface_snk6502>();


            m_tone_clock_expire = 0;
            m_tone_clock = 0;
            m_tone_stream = null;
            m_samples = new optional_device<samples_device>(this, "^samples");
            m_rom = new required_memory_region(this, DEVICE_SELF_OWNER);
            m_sound0_stop_on_rollover = 0;
            m_hd38880_cmd = 0;
            m_hd38880_addr = 0;
            m_hd38880_data_bytes = 0;
            m_hd38880_speed = 0;
        }


        public device_sound_interface_snk6502 disound { get { return m_disound; } }


        //DECLARE_READ_LINE_MEMBER(music0_playing);

        //void set_music_freq(int freq);
        //void set_music_clock(double clock_time);
        //void set_channel_base(int channel, int base, int mask = 0xff);
        //void mute_channel(int channel);
        //void unmute_channel(int channel);
        //void set_sound0_stop_on_rollover(int value) { m_sound0_stop_on_rollover = value; }
        //void reset_offset(int channel) { m_tone_channels[channel].offset = 0; }

        //void speech_w(uint8_t data, const uint16_t *table, int start);

        //void build_waveform(int channel, int mask);
        //void sasuke_build_waveform(int mask);
        //void satansat_build_waveform(int mask);


        // device-level overrides
        protected override void device_start() { throw new emu_unimplemented(); }


        // sound stream update overrides
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            throw new emu_unimplemented();
        }


        //void validate_tone_channel(int channel);
    }


    class vanguard_sound_device : device_t
    {
        //DEFINE_DEVICE_TYPE(VANGUARD_SOUND, vanguard_sound_device, "vanguard_sound", "SNK Vanguard Sound")
        public static readonly emu.detail.device_type_impl VANGUARD_SOUND = DEFINE_DEVICE_TYPE("vanguard_sound", "SNK Vanguard Sound", (type, mconfig, tag, owner, clock) => { return new vanguard_sound_device(mconfig, tag, owner, clock); });


        required_device<snk6502_sound_device> m_custom;
        required_device<sn76477_device> m_sn76477_2;
        required_device<samples_device> m_samples;

        uint8_t m_last_port1;


        static readonly string [] vanguard_sample_names =
        {
            "*vanguard",

            // SN76477 and discrete
            "fire",
            "explsion",

            // HD38880 speech
            "vg_voi-0",
            "vg_voi-1",
            "vg_voi-2",
            "vg_voi-3",
            "vg_voi-4",
            "vg_voi-5",
            "vg_voi-6",
            "vg_voi-7",
            "vg_voi-8",
            "vg_voi-9",
            "vg_voi-a",
            "vg_voi-b",
            "vg_voi-c",
            "vg_voi-d",
            "vg_voi-e",
            "vg_voi-f",

            null
        };


        vanguard_sound_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, VANGUARD_SOUND, tag, owner, clock)
        {
            m_custom = new required_device<snk6502_sound_device>(this, "custom");
            m_sn76477_2 = new required_device<sn76477_device>(this, "sn76477.2");
            m_samples = new required_device<samples_device>(this, "samples");
            m_last_port1 = 0;
        }


        //void sound_w(offs_t offset, uint8_t data);
        //void speech_w(uint8_t data);


        protected override void device_add_mconfig(machine_config config)
        {
            SPEAKER(config, "mono").front_center();

            SNK6502_SOUND(config, m_custom, 0);
            m_custom.op0.disound.add_route(ALL_OUTPUTS, "mono", 0.50);

            SAMPLES(config, m_samples);
            m_samples.op0.set_channels(3);
            m_samples.op0.set_samples_names(vanguard_sample_names);
            m_samples.op0.disound.add_route(ALL_OUTPUTS, "mono", 0.25);

            sn76477_device sn76477_1 = SN76477(config, "sn76477.1");
            // SHOT A   GND: 2,9,26,27  +5V: 15,25
            sn76477_1.set_noise_params(RES_K(470), RES_M(1.5), CAP_P(220));
            sn76477_1.set_decay_res(0);
            sn76477_1.set_attack_params(0, 0);
            sn76477_1.set_amp_res(RES_K(47));
            sn76477_1.set_feedback_res(RES_K(4.7));
            sn76477_1.set_vco_params(0, 0, 0);
            sn76477_1.set_pitch_voltage(0);
            sn76477_1.set_slf_params(0, 0);
            sn76477_1.set_oneshot_params(0, 0);
            sn76477_1.set_vco_mode(0);
            sn76477_1.set_mixer_params(0, 1, 0);
            sn76477_1.set_envelope_params(1, 1);
            sn76477_1.set_enable(1);
            sn76477_1.disound.add_route(ALL_OUTPUTS, "mono", 0.50);

            SN76477(config, m_sn76477_2);
            // SHOT B   GND: 1,2,26,27  +5V: 15,25,28
            m_sn76477_2.op0.set_noise_params(RES_K(10), RES_K(30), 0);
            m_sn76477_2.op0.set_decay_res(0);
            m_sn76477_2.op0.set_attack_params(0, 0);
            m_sn76477_2.op0.set_amp_res(RES_K(47));
            m_sn76477_2.op0.set_feedback_res(RES_K(4.7));
            m_sn76477_2.op0.set_vco_params(0, 0, 0);
            m_sn76477_2.op0.set_pitch_voltage(0);
            m_sn76477_2.op0.set_slf_params(0, 0);
            m_sn76477_2.op0.set_oneshot_params(0, 0);
            m_sn76477_2.op0.set_vco_mode(0);
            m_sn76477_2.op0.set_mixer_params(0, 1, 0);
            m_sn76477_2.op0.set_envelope_params(0, 1);
            m_sn76477_2.op0.set_enable(1);
            m_sn76477_2.op0.disound.add_route(ALL_OUTPUTS, "mono", 0.25);
        }


        protected override void device_start() { throw new emu_unimplemented(); }
        protected override void device_reset() { throw new emu_unimplemented(); }
    }


    //class fantasy_sound_device : public device_t

    //class nibbler_sound_device : public fantasy_sound_device

    //class pballoon_sound_device : public fantasy_sound_device

    //class sasuke_sound_device : public device_t

    //class satansat_sound_device : public device_t


    static class snk6502_global
    {
        public static snk6502_sound_device SNK6502_SOUND<bool_Required>(machine_config mconfig, device_finder<snk6502_sound_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, snk6502_sound_device.SNK6502_SOUND, clock); }
        public static vanguard_sound_device VANGUARD_SOUND(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<vanguard_sound_device>(mconfig, tag, vanguard_sound_device.VANGUARD_SOUND, clock); }
    }
}
