// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read_line = mame.devcb_read<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_read_line = devcb_read<int, 1U>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using int32_t = System.Int32;
using stream_buffer_sample_t = System.Single;  //using sample_t = float;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.emucore_global;
using static mame.hc55516_global;
using static mame.hc55516_internal;


namespace mame
{
    public class cvsd_device_base : device_t
                             //device_sound_interface
    {
        public class device_sound_interface_cvsd : device_sound_interface
        {
            public device_sound_interface_cvsd(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((cvsd_device_base)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        //enum
        //{
            protected const bool RISING = true;
        //    FALLING=false
        //};


        protected device_sound_interface m_disound;


        // callbacks
        devcb_write_line m_clock_state_push_cb; ///TODO: get rid of this, if you use it you should feel bad
        devcb_read_line m_digin_pull_cb;
        devcb_write_line m_digout_push_cb;

        // const state defined by constructor
        bool m_active_clock_edge;
        uint8_t m_shiftreg_mask; // it may be desirable to allow this to be changed by the user under some circumstances

        // internal state
        sound_stream m_stream;
        bool m_last_clock_state;
        bool m_buffered_bit;
        uint8_t m_shiftreg;
        stream_buffer_sample_t m_curr_sample;
        stream_buffer_sample_t m_next_sample;
        uint32_t m_samples_generated;


        protected cvsd_device_base(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, bool active_clock_edge, uint8_t shiftreg_mask)
            : base(mconfig, type, tag, owner, clock)
        {
            //, device_sound_interface(mconfig, *this)

            m_clock_state_push_cb = new devcb_write_line(this);
            m_digin_pull_cb = new devcb_read_line(this);
            m_digout_push_cb = new devcb_write_line(this);
            m_active_clock_edge = active_clock_edge;
            m_shiftreg_mask = shiftreg_mask;
            m_last_clock_state = false;
            m_buffered_bit = false;
            m_shiftreg = 0;
            m_curr_sample = 0;
            m_next_sample = 0;
            m_samples_generated = 0;
        }


        public device_sound_interface disound { get { return m_disound; } }


        //auto clock_state_cb() { return m_clock_state_push_cb.bind(); } // A clock state change callback. Using this is a bad idea due to lack of synchronization to other devices. TODO: remove this.
        //auto digin_cb() { return m_digin_pull_cb.bind(); } // Digital in pull callback function, for use if a clock is specified and we need to pull in the digital in pin state, otherwise unused. TODO: this is not hooked up yet, and should be.
        //auto digout_cb() { return m_digout_push_cb.bind(); } // Digital out push callback function. TODO: this is not hooked up or implemented yet, although it is only really relevant for devices which use the CVSD chips in encode mode.

        //READ_LINE_MEMBER( clock_r ); // Clock pull, really only relevant of something manually polls the clock (and clock is specified), which is a very bad design pattern and will cause synchronization/missed clock transition issues. This function WILL ASSERT if it is called and the clock hz is NOT specified! TODO: remove all use of this, and remove it.
        //WRITE_LINE_MEMBER( mclock_w ); // Clock push; this function WILL ASSERT if it is called and the clock hz IS specified!
        //WRITE_LINE_MEMBER( digin_w ); // Digital in push to the pin, as a pseudo 'buffer' implemented within the cvsd device itself. This is not technically accurate to hardware, and in the future should be deprecated in favor of digin_cb once the latter is implemented.
        //WRITE_LINE_MEMBER( dec_encq_w ); //DEC/ENC decode/encode select push. This is not implemented yet, and relies on an input audio stream. TODO: implement this beyond a do-nothing stub
        //READ_LINE_MEMBER( digout_r ); // Digital out pull. TODO: this is not hooked up or implemented yet, although it is only really relevant for devices which use the CVSD chips in encode mode.
        //void audio_in_w(stream_buffer::sample_t data); // Audio In pin, an analog value of the audio waveform being pushed to the chip. TODO: this is not hooked up or implemented yet, and this should really be handled as an input stream from a separate DAC device, not a value push function at all.


        public void digit_w(int digit)  /* sets the buffered digit (0 or 1), common to all chips. TODO: replace all use of this with digin_cb once implemented */
        {
            m_stream.update();
            m_buffered_bit = digit != 0 ? true : false;
        }


        public void clock_w(int state) { throw new emu_unimplemented(); }  /* sets the clock state (0 or 1, clocked on the rising edge), common to all chips */

        protected virtual int clock_state_r() { throw new emu_unimplemented(); }  /* returns whether the clock is currently LO or HI, common to all chips. TODO: get rid of all use of this, then get rid of it. */


        // device-level overrides
        protected override void device_start()
        {
            /* create the stream */
            m_stream = m_disound.stream_alloc(0, 1, SAMPLE_RATE);

            save_item(NAME(new { m_last_clock_state }));
            save_item(NAME(new { m_buffered_bit }));
            save_item(NAME(new { m_shiftreg }));
            save_item(NAME(new { m_curr_sample }));
            save_item(NAME(new { m_next_sample }));
            save_item(NAME(new { m_samples_generated }));
        }


        protected override void device_reset()
        {
            m_last_clock_state = false;  //0;
        }


        ////virtual void device_clock_changed() override;


        // sound stream update overrides
        protected virtual void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { throw new emu_unimplemented(); }


        // specific internal handler overrides, overridden by each chip
        protected virtual void process_bit(bool bit, bool clock_state) { throw new emu_unimplemented(); }

        ///TODO: get rid of these
        //inline bool is_external_oscillator();
        //inline bool is_clock_changed(bool clock_state);
        //inline bool is_active_clock_transition(bool clock_state);
        //inline bool current_clock_state();
    }


    public class hc55516_device : cvsd_device_base
    {
        //DEFINE_DEVICE_TYPE(HC55516, hc55516_device, "hc55516", "HC-55516")
        public static readonly emu.detail.device_type_impl HC55516 = DEFINE_DEVICE_TYPE("hc55516", "HC-55516", (type, mconfig, tag, owner, clock) => { return new hc55516_device(mconfig, tag, owner, clock); });


        public class device_sound_interface_hc55516 : device_sound_interface
        {
            public device_sound_interface_hc55516(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((hc55516_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        // callbacks
        devcb_write_line m_agc_push_cb;
        devcb_read_line m_fzq_pull_cb;

        // const coefficients defined by constructor
        uint32_t m_sylmask;
        int32_t m_sylshift;
        int32_t m_syladd;
        int32_t m_intshift;

        // internal state
        int32_t m_sylfilter;
        int32_t m_intfilter;
        bool m_agc;
        bool m_buffered_fzq;


        hc55516_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, HC55516, tag, owner, clock, 0xfc0, 6, 0xfc1, 4)
        {
            m_class_interfaces.Add(new device_sound_interface_hc55516(mconfig, this));  //device_sound_interface(mconfig, *this),
            m_disound = GetClassInterface<device_sound_interface_hc55516>();
        }


        hc55516_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, uint32_t sylmask, int32_t sylshift, int32_t syladd, int32_t intshift)
            : base(mconfig, type, tag, owner, clock, RISING, 0x7)
        {
            m_agc_push_cb = new devcb_write_line(this);
            m_fzq_pull_cb = new devcb_read_line(this);
            m_sylmask = sylmask;
            m_sylshift = sylshift;
            m_syladd = syladd;
            m_intshift = intshift;
            m_sylfilter = 0;
            m_intfilter = 0;
            m_agc = true;
            m_buffered_fzq = true;
        }


        //auto fzq_cb() { return m_fzq_pull_cb.bind(); }  // /FZ (partial reset) pull callback, ok to leave unconnected (we assume it is pulled high)
        //auto agc_cb() { return m_agc_push_cb.bind(); }  // AGC callback function, called to push the state if the AGC pin changes, ok to leave unconnected

        //WRITE_LINE_MEMBER( fzq_w ); // /FZ (partial reset) push
        //READ_LINE_MEMBER( agc_r ); // AGC pull
        /* TODO: These are only relevant for encode mode, which isn't done yet! */
        //WRITE_LINE_MEMBER( aptq_w ); // /APT (silence encoder output) push
        //WRITE_LINE_MEMBER( dec_encq_w ); // DEC/ENC decode/encode select push


        // device-level overrides
        protected override void device_start()
        {
            base.device_start();
            save_item(NAME(new { m_sylfilter }));
            save_item(NAME(new { m_intfilter }));
            save_item(NAME(new { m_agc }));
            save_item(NAME(new { m_buffered_fzq }));

            /* resolve lines */
            m_agc_push_cb.resolve();
            m_fzq_pull_cb.resolve();
        }


        protected override void device_reset()
        {
            base.device_reset();
            // simulate /FZ having been held for a while
            m_sylfilter = 0x3f;
            m_intfilter = 0;
            m_agc = true;
            m_buffered_fzq = true; // assuming /FZ was just released and is now high/inactive
        }


        // sound stream update overrides
        protected override void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { throw new emu_unimplemented(); }


        // internal handlers
        protected override void process_bit(bool bit, bool clock_state) { throw new emu_unimplemented(); }
    }


    //class hc55532_device : public hc55516_device

    //class mc3417_device : public cvsd_device_base

    //class mc3418_device : public mc3417_device


    static class hc55516_internal
    {
        /* fixed samplerate of 192khz */
        public const uint32_t SAMPLE_RATE             = 48000 * 4;

        //#define INTEGRATOR_LEAK_TC      0.001
        //#define FILTER_DECAY_TC         0.004
        //#define FILTER_CHARGE_TC        0.004
        //#define FILTER_MIN              0.0416
        //#define FILTER_MAX              1.0954
        //#define SAMPLE_GAIN             (10000.0 / 32768.0)
    }


    public static class hc55516_global
    {
        public static hc55516_device HC55516(machine_config mconfig, string tag, uint32_t clock) { return emu.detail.device_type_impl.op<hc55516_device>(mconfig, tag, hc55516_device.HC55516, clock); }
    }
}
