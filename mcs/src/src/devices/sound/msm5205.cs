// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_timer_id = System.UInt32;  //typedef u32 device_timer_id;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using s32 = System.Int32;
using stream_buffer_sample_t = System.Single;  //using sample_t = float;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;
using uint8_t = System.Byte;

using static mame.cpp_global;
using static mame.device_global;
using static mame.disound_global;
using static mame.emucore_global;
using static mame.msm5205_global;
using static mame.util;


namespace mame
{
    public class msm5205_device : device_t
                                  //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(MSM5205, msm5205_device, "msm5205", "OKI MSM5205 ADPCM")
        public static readonly emu.detail.device_type_impl MSM5205 = DEFINE_DEVICE_TYPE("msm5205", "OKI MSM5205 ADPCM", (type, mconfig, tag, owner, clock) => { return new msm5205_device(mconfig, tag, owner, clock); });


        public class device_sound_interface_msm5205 : device_sound_interface
        {
            public device_sound_interface_msm5205(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((msm5205_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        /* step size index shift table */
        static readonly int [] index_shift = new int [8] { -1, -1, -1, -1, 2, 4, 6, 8 };


        // MSM5205 default master clock is 384KHz
        //static constexpr int S96_3B = 0;     // prescaler 1/96(4KHz) , data 3bit
        //static constexpr int S48_3B = 1;     // prescaler 1/48(8KHz) , data 3bit
        //static constexpr int S64_3B = 2;     // prescaler 1/64(6KHz) , data 3bit
        //static constexpr int SEX_3B = 3;     // VCK slave mode       , data 3bit
        public static int S96_4B = 4;     // prescaler 1/96(4KHz) , data 4bit
        //static constexpr int S48_4B = 5;     // prescaler 1/48(8KHz) , data 4bit
        //static constexpr int S64_4B = 6;     // prescaler 1/64(6KHz) , data 4bit
        //static constexpr int SEX_4B = 7;     // VCK slave mode       , data 4bit


        //enum
        //{
        const int TIMER_VCK = 0;
        const int TIMER_ADPCM_CAPTURE = 1;
        //}


        device_sound_interface_msm5205 m_disound;


        sound_stream m_stream;     // number of stream system
        emu_timer m_vck_timer;     // VCK callback timer
        emu_timer m_capture_timer; // delay after VCK active edge for ADPCM input capture
        u8 m_data;                  // next adpcm data
        bool m_vck;                 // VCK signal
        bool m_reset;               // reset pin signal
        bool m_s1;                  // prescaler selector S1
        bool m_s2;                  // prescaler selector S2
        u8 m_bitwidth;              // bit width selector -3B/4B
        s32 m_signal;               // current ADPCM signal
        s32 m_step;                 // current ADPCM step
        u8 m_dac_bits;              // DAC output bits (10 for MSM5205, 12 for MSM6585)
        int [] m_diff_lookup = new int[49*16];

        devcb_write_line m_vck_cb;
        devcb_write_line m_vck_legacy_cb;


        msm5205_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : this(mconfig, MSM5205, tag, owner, clock, 10)
        {
        }


        msm5205_device(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock, u8 dac_bits)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_msm5205(mconfig, this));  //device_sound_interface(mconfig, *this),
            m_disound = GetClassInterface<device_sound_interface_msm5205>();

            m_s1 = false;
            m_s2 = false;
            m_bitwidth = 4;
            m_dac_bits = dac_bits;
            m_vck_cb = new devcb_write_line(this);
            m_vck_legacy_cb = new devcb_write_line(this);
        }


        public device_sound_interface_msm5205 disound { get { return m_disound; } }
        public device_sound_interface add_route(u32 output, string target, double gain, u32 input = AUTO_ALLOC_INPUT, u32 mixoutput = 0) { return disound.add_route(output, target, gain, input, mixoutput); }
        public device_sound_interface add_route(u32 output, device_sound_interface target, double gain, u32 input = AUTO_ALLOC_INPUT, u32 mixoutput = 0) { return disound.add_route(output, target, gain, input, mixoutput); }
        public device_sound_interface add_route(u32 output, speaker_device target, double gain, u32 input = AUTO_ALLOC_INPUT, u32 mixoutput = 0) { return disound.add_route(output, target, gain, input, mixoutput); }
        public device_sound_interface add_route(u32 output, device_t base_, string target, double gain, u32 input, u32 mixoutput) { return disound.add_route(output, base_, target, gain, input, mixoutput); }
        public sound_stream stream_alloc(int inputs, int outputs, u32 sample_rate) { return disound.stream_alloc(inputs, outputs, sample_rate); }
        public sound_stream stream_alloc(int inputs, int outputs, u32 sample_rate, sound_stream_flags flags) { return disound.stream_alloc(inputs, outputs, sample_rate, flags); }


        public void set_prescaler_selector(int select)
        {
            m_s1 = BIT(select, 1) != 0;
            m_s2 = BIT(select, 0) != 0;
            m_bitwidth = ((select & 4) != 0) ? (u8)4 : (u8)3;
        }


        public devcb_write_line.binder vck_callback() { return m_vck_cb.bind(); }
        //auto vck_legacy_callback() { return m_vck_legacy_cb.bind(); }


        // reset signal should keep for 2cycle of VCLK
        public void reset_w(int state)
        {
            m_reset = state != 0;
        }


        // adpcmata is latched after vclk_interrupt callback
        /*
         *    Handle an update of the data to the chip
         */
        public void data_w(uint8_t data)
        {
            if (m_bitwidth == 4)
                m_data = (u8)(data & 0x0f);
            else
                m_data = (u8)((data & 0x07) << 1); /* unknown */
        }


        // VCLK slave mode option
        // if VCLK and reset or data is changed at the same time,
        // call vclk_w after data_w and reset_w.
        //void vclk_w(int state);


        // option , selected pin selector
        /*
         *    Handle a change of the selector
         */
        public void playmode_w(int select)
        {
            int bitwidth = ((select & 4) != 0) ? 4 : 3;

            if ((select & 3) != (((m_s1 ? 1 : 0) << 1) | (m_s2 ? 1 : 0)))
            {
                m_stream.update();

                m_s1 = BIT(select, 1) != 0;
                m_s2 = BIT(select, 0) != 0;

                /* timer set */
                notify_clock_changed();
            }

            if (m_bitwidth != bitwidth)
            {
                m_stream.update();
                m_bitwidth = (u8)bitwidth;
            }
        }


        //void s1_w(int state);
        //void s2_w(int state);


        // device-level overrides
        protected override void device_start()
        {
            m_vck_cb.resolve_safe();
            m_vck_legacy_cb.resolve();

            /* compute the difference tables */
            compute_tables();

            /* stream system initialize */
            m_stream = stream_alloc(0, 1, clock());
            m_vck_timer = timer_alloc(TIMER_VCK);
            m_capture_timer = timer_alloc(TIMER_ADPCM_CAPTURE);

            /* register for save states */
            save_item(NAME(new { m_data }));
            save_item(NAME(new { m_vck }));
            save_item(NAME(new { m_reset }));
            save_item(NAME(new { m_s1 }));
            save_item(NAME(new { m_s2 }));
            save_item(NAME(new { m_bitwidth }));
            save_item(NAME(new { m_signal }));
            save_item(NAME(new { m_step }));
        }


        protected override void device_reset()
        {
            /* initialize work */
            m_data    = 0;
            m_vck     = false;
            m_reset   = false;
            m_signal  = 0;
            m_step    = 0;
        }


        protected override void device_clock_changed()
        {
            m_stream.set_sample_rate(clock());
            int prescaler = get_prescaler();
            if (prescaler != 0)
            {
                logerror("/{0} prescaler selected\n", prescaler);

                attotime half_period = clocks_to_attotime((u64)(prescaler / 2));
                m_vck_timer.adjust(half_period, 0, half_period);
            }
            else
            {
                logerror("VCK slave mode selected\n");
                m_vck_timer.adjust(attotime.never);
            }
        }


        protected override void device_timer(emu_timer timer, device_timer_id id, int param)
        {
            switch (id)
            {
                case TIMER_VCK:
                    m_vck = !m_vck;
                    m_vck_cb.op_s32(m_vck ? 1 : 0);
                    if (!m_vck)
                        m_capture_timer.adjust(attotime.from_hz(clock() / 6)); // 15.6 usec at 384KHz
                    break;

                case TIMER_ADPCM_CAPTURE:
                    update_adpcm();
                    break;
            }
        }


        // timer callback at VCK low edge on MSM5205 (at rising edge on MSM6585)
        void update_adpcm()
        {
            int val;
            int new_signal;

            // callback user handler and latch next data
            if (!m_vck_legacy_cb.isnull())
                m_vck_legacy_cb.op_s32(1);

            // reset check at last hiedge of VCK
            if (m_reset)
            {
                new_signal = 0;
                m_step = 0;
            }
            else
            {
                /* update signal */
                /* !! MSM5205 has internal 12bit decoding, signal width is 0 to 8191 !! */
                val = m_data;
                new_signal = m_signal + m_diff_lookup[m_step * 16 + (val & 15)];

                if (new_signal > 2047) new_signal = 2047;
                else if (new_signal < -2048) new_signal = -2048;

                m_step += index_shift[val & 7];

                if (m_step > 48) m_step = 48;
                else if (m_step < 0) m_step = 0;
            }

            /* update when signal changed */
            if ( m_signal != new_signal)
            {
                m_stream.update();
                m_signal = new_signal;
            }
        }


        // device_sound_interface overrides
        //-------------------------------------------------
        //  sound_stream_update - handle a stream update
        //-------------------------------------------------
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            var output = outputs[0];

            /* if this voice is active */
            if (m_signal != 0)
            {
                stream_buffer_sample_t sample_scale = (stream_buffer_sample_t)(1.0 / (double)(1 << 12));
                int dac_mask = (m_dac_bits >= 12) ? 0 : (1 << (12 - m_dac_bits)) - 1;
                stream_buffer_sample_t val = (stream_buffer_sample_t)(m_signal & ~dac_mask) * sample_scale;
                output.fill(val);
            }
            else
            {
                output.fill(0);
            }
        }


        void compute_tables()
        {
            /* nibble to bit map */
            int [,] nbl2bit = new int [16, 4]
            {
                { 1, 0, 0, 0}, { 1, 0, 0, 1}, { 1, 0, 1, 0}, { 1, 0, 1, 1},
                { 1, 1, 0, 0}, { 1, 1, 0, 1}, { 1, 1, 1, 0}, { 1, 1, 1, 1},
                {-1, 0, 0, 0}, {-1, 0, 0, 1}, {-1, 0, 1, 0}, {-1, 0, 1, 1},
                {-1, 1, 0, 0}, {-1, 1, 0, 1}, {-1, 1, 1, 0}, {-1, 1, 1, 1}
            };

            int step;
            int nib;

            /* loop over all possible steps */
            for (step = 0; step <= 48; step++)
            {
                /* compute the step value */
                int stepval = (int)floor(16.0 * pow(11.0 / 10.0, (double)step));

                /* loop over all nibbles and compute the difference */
                for (nib = 0; nib < 16; nib++)
                {
                    m_diff_lookup[step*16 + nib] = nbl2bit[nib, 0] *
                        (stepval   * nbl2bit[nib, 1] +
                         stepval/2 * nbl2bit[nib, 2] +
                         stepval/4 * nbl2bit[nib, 3] +
                         stepval/8);
                }
            }
        }


        protected virtual int get_prescaler()
        {
            if (m_s1)
                return m_s2 ? 0 : 64;
            else
                return m_s2 ? 48 : 96;
        }
    }


    //class msm6585_device : public msm5205_device


    static class msm5205_global
    {
        public static msm5205_device MSM5205<bool_Required>(machine_config mconfig, device_finder<msm5205_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, msm5205_device.MSM5205, clock); }
    }
}
