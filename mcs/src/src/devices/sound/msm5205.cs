// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_timer_id = System.UInt32;
using device_type = mame.emu.detail.device_type_impl_base;
using stream_sample_t = System.Int32;
using s32 = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    class device_sound_interface_msm5205 : device_sound_interface
    {
        public device_sound_interface_msm5205(machine_config mconfig, device_t device) : base(mconfig, device) { }


        // device_sound_interface overrides
        //-------------------------------------------------
        //  sound_stream_update - handle a stream update
        //-------------------------------------------------
        public override void sound_stream_update(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples)
        {
            msm5205_device pokey = (msm5205_device)device();

            ListPointer<stream_sample_t> buffer = outputs[0];

            /* if this voice is active */
            if (pokey.signal != 0)
            {
                short val = (short)(pokey.signal * 16);
                while (samples != 0)
                {
                    buffer[0] = val;  //*buffer++ = val;
                    buffer++;
                    samples--;
                }
            }
            else
            {
                memset(buffer, 0, (UInt32)samples);  //memset(buffer, 0, samples * sizeof(*buffer));
            }
        }
    }


    public class msm5205_device : device_t
                                  //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(MSM5205, msm5205_device, "msm5205", "MSM5205")
        static device_t device_creator_msm5205_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new msm5205_device(mconfig, tag, owner, clock); }
        public static readonly device_type MSM5205 = DEFINE_DEVICE_TYPE(device_creator_msm5205_device, "msm5205", "MSM5205");


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
        int [] m_diff_lookup = new int[49*16];

        devcb_write_line m_vck_cb;
        devcb_write_line m_vck_legacy_cb;


        msm5205_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : this(mconfig, MSM5205, tag, owner, clock)
        {
        }


        msm5205_device(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_msm5205(mconfig, this));  //device_sound_interface(mconfig, *this),

            m_s1 = false;
            m_s2 = false;
            m_bitwidth = 4;
            m_vck_cb = new devcb_write_line(this);
            m_vck_legacy_cb = new devcb_write_line(this);
        }


        public s32 signal { get { return m_signal; } }


        public void set_prescaler_selector(int select)
        {
            m_s1 = BIT(select, 1) != 0;
            m_s2 = BIT(select, 0) != 0;
            m_bitwidth = ((select & 4) != 0) ? (u8)4 : (u8)3;
        }

        //template <class Object> devcb_base &set_vck_callback(Object &&cb) { return m_vck_cb.set_callback(std::forward<Object>(cb)); }
        public devcb_base set_vck_callback(DEVCB_INPUTLINE cb) { return m_vck_cb.set_callback(this, cb); }

        //template <class Object> devcb_base &set_vck_legacy_callback(Object &&cb) { return m_vck_legacy_cb.set_callback(std::forward<Object>(cb)); }
        public devcb_write.binder vck_callback() { return m_vck_cb.bind(); }
        //auto vck_legacy_callback() { return m_vck_legacy_cb.bind(); }


        // reset signal should keep for 2cycle of VCLK
        //WRITE_LINE_MEMBER(msm5205_device::reset_w)
        public void reset_w(int state)
        {
            m_reset = state != 0;
        }


        // adpcmata is latched after vclk_interrupt callback
        /*
         *    Handle an update of the data to the chip
         */
        public void write_data(int data)
        {
            if (m_bitwidth == 4)
                m_data = (u8)(data & 0x0f);
            else
                m_data = (u8)((data & 0x07) << 1); /* unknown */
        }


        //DECLARE_WRITE8_MEMBER(data_w) { write_data(data); }


        // VCLK slave mode option
        // if VCLK and reset or data is changed at the same time,
        // call vclk_w after data_w and reset_w.
        //DECLARE_WRITE_LINE_MEMBER(vclk_w);


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


        //DECLARE_WRITE_LINE_MEMBER(s1_w);
        //DECLARE_WRITE_LINE_MEMBER(s2_w);


        // device-level overrides
        protected override void device_start()
        {
            m_vck_cb.resolve_safe();
            m_vck_legacy_cb.resolve();

            /* compute the difference tables */
            compute_tables();

            /* stream system initialize */
            m_stream = machine().sound().stream_alloc(this, 0, 1, (int)clock());
            m_vck_timer = timer_alloc(TIMER_VCK);
            m_capture_timer = timer_alloc(TIMER_ADPCM_CAPTURE);

            /* register for save states */
            save_item(m_data, "m_data");
            save_item(m_vck, "m_vck");
            save_item(m_reset, "m_reset");
            save_item(m_s1, "m_s1");
            save_item(m_s2, "m_s2");
            save_item(m_bitwidth, "m_bitwidth");
            save_item(m_signal, "m_signal");
            save_item(m_step, "m_step");
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


        protected override void device_timer(emu_timer timer, device_timer_id id, int param, object ptr)
        {
            switch (id)
            {
                case TIMER_VCK:
                    m_vck = !m_vck;
                    m_vck_cb.op(m_vck ? 1 : 0);
                    if (!m_vck)
                        m_capture_timer.adjust(attotime.from_nsec(15600));
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
                m_vck_legacy_cb.op(1);

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


        // sound stream update overrides
        //protected override void sound_stream_update(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples);


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
                int stepval = (int)Math.Floor(16.0 * Math.Pow(11.0 / 10.0, (double)step));

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
}
