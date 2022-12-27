// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using int32_t = System.Int32;
using u8 = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.diexec_global;
using static mame.emucore_global;
using static mame.sn76496_global;
using static mame.sn76496_internal;


namespace mame
{
    public class sn76496_base_device : device_t
                                //device_sound_interface
    {
        public class device_sound_interface_sn76496 : device_sound_interface
        {
            public device_sound_interface_sn76496(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((sn76496_base_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        device_sound_interface_sn76496 m_disound;


        bool m_ready_state;
        devcb_write_line m_ready_handler;

        sound_stream m_sound;

        int32_t m_feedback_mask;    // mask for feedback
        int32_t m_whitenoise_tap1;  // mask for white noise tap 1 (higher one, usually bit 14)
        int32_t m_whitenoise_tap2;  // mask for white noise tap 2 (lower one, usually bit 13)
        bool m_negate;           // output negate flag
        bool m_stereo;           // whether we're dealing with stereo or not
        int32_t m_clock_divider;    // clock divider
        bool m_ncr_style_psg;    // flag to ignore writes to regs 1,3,5,6,7 with bit 7 low
        bool m_sega_style_psg;   // flag to make frequency zero acts as if it is one more than max (0x3ff+1) or if it acts like 0; the initial register is pointing to 0x3 instead of 0x0; the volume reg is preloaded with 0xF instead of 0x0

        int32_t [] m_vol_table = new int32_t [16];    // volume table (for 4-bit to db conversion)
        int32_t [] m_register = new int32_t [8];      // registers
        int32_t m_last_register;    // last register written
        int32_t [] m_volume = new int32_t [4];        // db volume of voice 0-2 and noise
        uint32_t m_RNG;              // noise generator LFSR
        int32_t m_current_clock;
        int32_t m_stereo_mask;      // the stereo output mask
        int32_t [] m_period = new int32_t [4];        // Length of 1/2 of waveform
        int32_t [] m_count = new int32_t [4];         // Position within the waveform
        int32_t [] m_output = new int32_t [4];        // 1-bit output of each channel, pre-volume

        emu_timer m_ready_timer;


        protected sn76496_base_device(
                machine_config mconfig,
                device_type type,
                string tag,
                int feedbackmask,
                int noisetap1,
                int noisetap2,
                bool negate,
                bool stereo,
                int clockdivider,
                bool ncr,
                bool sega,
                device_t owner,
                uint32_t clock)
        : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_sn76496(mconfig, this));  // device_sound_interface(mconfig, *this);
            m_disound = GetClassInterface<device_sound_interface_sn76496>();


            m_ready_handler = new devcb_write_line(this);
            m_feedback_mask = feedbackmask;
            m_whitenoise_tap1 = noisetap1;
            m_whitenoise_tap2 = noisetap2;
            m_negate = negate;
            m_stereo = stereo;
            m_clock_divider = clockdivider;
            m_ncr_style_psg = ncr;
            m_sega_style_psg = sega;
        }


        public device_sound_interface_sn76496 disound { get { return m_disound; } }


        //auto ready_cb() { return m_ready_handler.bind(); }
        //void stereo_w(u8 data);


        public void write(u8 data) { throw new emu_unimplemented(); }


        //DECLARE_READ_LINE_MEMBER( ready_r ) { return m_ready_state ? 1 : 0; }


        protected override void device_start()
        {
            int sample_rate = (int)(clock() / 2);
            int i;
            double out_;
            int gain;

            m_ready_handler.resolve_safe();

            m_sound = disound.stream_alloc(0, (m_stereo ? 2 : 1), (uint32_t)sample_rate);

            for (i = 0; i < 4; i++) m_volume[i] = 0;

            m_last_register = m_sega_style_psg ? 3 : 0; // Sega VDP PSG defaults to selected period reg for 2nd channel
            for (i = 0; i < 8; i += 2)
            {
                m_register[i] = 0;
                m_register[i + 1] = 0x0;   // volume = 0x0 (max volume) on reset; this needs testing on chips other than SN76489A and Sega VDP PSG
            }

            for (i = 0; i < 4; i++)
            {
                m_output[i] = 0;
                m_period[i] = 0;
                m_count[i] = 0;
            }

            m_RNG = (uint32_t)m_feedback_mask;
            m_output[3] = (int32_t)(m_RNG & 1);

            m_stereo_mask = 0xFF;           // all channels enabled
            m_current_clock = m_clock_divider - 1;

            // set gain
            gain = 0;

            gain &= 0xff;

            // increase max output basing on gain (0.2 dB per step)
            out_ = MAX_OUTPUT / 4; // four channels, each gets 1/4 of the total range
            while (gain-- > 0)
                out_ *= 1.023292992; // = (10 ^ (0.2/20))

            // build volume table (2dB per step)
            for (i = 0; i < 15; i++)
            {
                // limit volume to avoid clipping
                if (out_ > MAX_OUTPUT / 4) m_vol_table[i] = MAX_OUTPUT / 4;
                else m_vol_table[i] = (int32_t)out_;

                out_ /= 1.258925412; /* = 10 ^ (2/20) = 2dB */
            }
            m_vol_table[15] = 0;

            m_ready_state = true;

            m_ready_timer = timer_alloc(0);
            m_ready_handler.op_s32(ASSERT_LINE);

            register_for_save_states();
        }


        protected override void device_clock_changed()
        {
            m_sound.set_sample_rate(clock() / 2);
        }


        //virtual void    device_timer(emu_timer &timer, device_timer_id id, int param) override;


        protected virtual void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { throw new emu_unimplemented(); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;


        //inline bool     in_noise_mode();


        void register_for_save_states()
        {
            save_item(NAME(new { m_vol_table }));
            save_item(NAME(new { m_register }));
            save_item(NAME(new { m_last_register }));
            save_item(NAME(new { m_volume }));
            save_item(NAME(new { m_RNG }));
            //  save_item(NAME(m_clock_divider));
            save_item(NAME(new { m_current_clock }));
            //  save_item(NAME(m_feedback_mask));
            //  save_item(NAME(m_whitenoise_tap1));
            //  save_item(NAME(m_whitenoise_tap2));
            //  save_item(NAME(m_negate));
            //  save_item(NAME(m_stereo));
            save_item(NAME(new { m_stereo_mask }));
            save_item(NAME(new { m_period }));
            save_item(NAME(new { m_count }));
            save_item(NAME(new { m_output }));
            //  save_item(NAME(m_sega_style_psg));
        }
    }


    //class sn76496_device : public sn76496_base_device

    //class y2404_device : public sn76496_base_device

    //class sn76489_device : public sn76496_base_device


    // SN76489A: whitenoise verified, phase verified, periodic verified (by plgdavid)
    public class sn76489a_device : sn76496_base_device
    {
        //DEFINE_DEVICE_TYPE(SN76489A, sn76489a_device,  "sn76489a",     "SN76489A")
        public static readonly emu.detail.device_type_impl SN76489A = DEFINE_DEVICE_TYPE("sn76489a", "SN76489A", (type, mconfig, tag, owner, clock) => { return new sn76489a_device(mconfig, tag, owner, clock); });


        sn76489a_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, SN76489A, tag, 0x10000, 0x04, 0x08, false, false, 8, false, true, owner, clock)
        { }
    }


    //class sn76494_device : public sn76496_base_device

    //class sn94624_device : public sn76496_base_device

    //class ncr8496_device : public sn76496_base_device

    //class pssj3_device : public sn76496_base_device

    //class gamegear_device : public sn76496_base_device

    //class segapsg_device : public sn76496_base_device


    static class sn76496_internal
    {
        public const int MAX_OUTPUT = 0x7fff;
    }


    public static class sn76496_global
    {
        public static sn76489a_device SN76489A(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<sn76489a_device>(mconfig, tag, sn76489a_device.SN76489A, clock); }
    }
}
