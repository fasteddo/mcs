// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.exidy_global;


namespace mame
{
    class exidy_sound_device : device_t
                               //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(EXIDY, exidy_sound_device, "exidy_sfx", "Exidy SFX")
        public static readonly emu.detail.device_type_impl EXIDY = DEFINE_DEVICE_TYPE("exidy_sfx", "Exidy SFX", (type, mconfig, tag, owner, clock) => { return new exidy_sound_device(mconfig, tag, owner, clock); });


        public class device_sound_interface_exidy : device_sound_interface
        {
            public device_sound_interface_exidy(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((exidy_sound_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        /* 6840 variables */
        //struct sh6840_timer_channel
        //{
        //    uint8_t   cr = 0;
        //    uint8_t   state = 0;
        //    uint8_t   leftovers = 0;
        //    uint16_t  timer = 0;
        //    uint32_t  clocks = 0;
        //    union
        //    {
//#ifdef LSB_FIRST
        //        struct { uint8_t l, h; } b;
//#else
        //        struct { uint8_t h, l; } b;
//#endif
        //        uint16_t w;
        //    } counter;
        //};


        device_sound_interface_exidy m_disound;


        /* sound streaming variables */
        sound_stream m_stream;
        double m_freq_to_step;

        // internal state
        //sh6840_timer_channel m_sh6840_timer[3];
        //int16_t m_sh6840_volume[3];
        uint8_t m_sh6840_MSB_latch;
        uint8_t m_sh6840_LSB_latch;
        uint8_t m_sh6840_LFSR_oldxor;
        uint32_t m_sh6840_LFSR_0;
        uint32_t m_sh6840_LFSR_1;
        uint32_t m_sh6840_LFSR_2;
        uint32_t m_sh6840_LFSR_3;
        uint32_t m_sh6840_clocks_per_sample;
        uint32_t m_sh6840_clock_count;

        uint8_t m_sfxctrl;


        exidy_sound_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, EXIDY, tag, owner, clock)
        {
        }

        exidy_sound_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_exidy(mconfig, this));  // device_sound_interface(mconfig, *this);
            m_disound = GetClassInterface<device_sound_interface_exidy>();


            m_stream = null;
            m_freq_to_step = 0;
            m_sh6840_MSB_latch = 0;
            m_sh6840_LSB_latch = 0;
            m_sh6840_LFSR_oldxor = 0;
            m_sh6840_LFSR_0 = 0xffffffff;
            m_sh6840_LFSR_1 = 0xffffffff;
            m_sh6840_LFSR_2 = 0xffffffff;
            m_sh6840_LFSR_3 = 0xffffffff;
            m_sh6840_clocks_per_sample = 0;
            m_sh6840_clock_count = 0;
            m_sfxctrl = 0;
        }

        //~exidy_sound_device() {}


        public device_sound_interface_exidy disound { get { return m_disound; } }


        //uint8_t sh6840_r(offs_t offset);
        //void sh6840_w(offs_t offset, uint8_t data);
        //void sfxctrl_w(offs_t offset, uint8_t data);


        // device-level overrides
        protected override void device_start()
        {
            throw new emu_unimplemented();
        }

        protected override void device_reset()
        {
            throw new emu_unimplemented();
        }


        //void common_sh_start();
        //void common_sh_reset();

        //void sh6840_register_state_globals();


        // sound stream update overrides
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            throw new emu_unimplemented();
        }


        //virtual s32 generate_music_sample();

        //static inline void sh6840_apply_clock(sh6840_timer_channel *t, int clocks);

        //inline int sh6840_update_noise(int clocks);
    }


    //class exidy_sh8253_sound_device : public exidy_sound_device

    //class venture_sound_device : public exidy_sh8253_sound_device

    //class mtrap_sound_device : public venture_sound_device

    //class victory_sound_device : public exidy_sh8253_sound_device


    static class exidy_global
    {
        public static exidy_sound_device EXIDY<bool_Required>(machine_config mconfig, device_finder<exidy_sound_device, bool_Required> finder, uint32_t clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, exidy_sound_device.EXIDY, clock); }
    }
}
