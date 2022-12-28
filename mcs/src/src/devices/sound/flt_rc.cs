// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u32 = System.UInt32;
using uint32_t = System.UInt32;

using static mame.device_global;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> filter_rc_device
    public class filter_rc_device : device_t
                                    //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(FILTER_RC, filter_rc_device, "filter_rc", "RC Filter")
        public static readonly emu.detail.device_type_impl FILTER_RC = DEFINE_DEVICE_TYPE("filter_rc", "RC Filter", (type, mconfig, tag, owner, clock) => { return new filter_rc_device(mconfig, tag, owner, clock); });


        public class device_sound_interface_filter_rc : device_sound_interface
        {
            public device_sound_interface_filter_rc(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((filter_rc_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        //enum
        //{
        //    LOWPASS_3R   = 0,
        //    LOWPASS      = 2,
        //    HIGHPASS     = 3,
        //    AC           = 4
        //};


        device_sound_interface_filter_rc m_disound;


        //sound_stream*  m_stream;
        //stream_buffer::sample_t m_k;
        //stream_buffer::sample_t m_memory;
        //int            m_type;
        //int            m_last_sample_rate;
        //double         m_R1;
        //double         m_R2;
        //double         m_R3;
        //double         m_C;


        filter_rc_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, FILTER_RC, tag, owner, clock)
        {
            throw new emu_unimplemented();

            //m_class_interfaces.Add(new device_sound_interface_filter_rc(mconfig, this));  // device_sound_interface(mconfig, *this);
            //m_disound = GetClassInterface<device_sound_interface_filter_rc>();

            //m_stream(nullptr),
            //m_k(0),
            //m_memory(0),
            //m_type(LOWPASS_3R),
            //m_last_sample_rate(0),
            //m_R1(1),
            //m_R2(1),
            //m_R3(1),
            //m_C(0)
        }


        // configuration
        //filter_rc_device &set_rc(int type, double R1, double R2, double R3, double C)
        //{
        //    m_type = type;
        //    m_R1 = R1;
        //    m_R2 = R2;
        //    m_R3 = R3;
        //    m_C = C;
        //    return *this;
        //}

        //filter_rc_device &set_lowpass(double R, double C)
        //{
        //    m_type = LOWPASS;
        //    m_R1 = R;
        //    m_R2 = 0;
        //    m_R3 = 0;
        //    m_C = C;
        //    return *this;
        //}

        //filter_rc_device &filter_rc_set_RC(int type, double R1, double R2, double R3, double C)
        //{
        //    m_stream->update();
        //    set_rc(type, R1, R2, R3, C);
        //    m_last_sample_rate = 0;
        //    return *this;
        //}

        //filter_rc_device &set_ac()
        //{
        //    return set_rc(filter_rc_device::AC, RES_K(10), 0, 0, CAP_U(1));
        //}


        // device-level overrides
        protected override void device_start() { throw new emu_unimplemented(); }


        // sound stream update overrides
        protected virtual void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { throw new emu_unimplemented(); }


        //void recalc();
    }
}
