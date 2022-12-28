// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u32 = System.UInt32;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.flt_vol_global;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> filter_volume_device
    public class filter_volume_device : device_t
                                        //public device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(FILTER_VOLUME, filter_volume_device, "filter_volume", "Volume Filter")
        public static readonly emu.detail.device_type_impl FILTER_VOLUME = DEFINE_DEVICE_TYPE("filter_volume", "Volume Filter", (type, mconfig, tag, owner, clock) => { return new filter_volume_device(mconfig, tag, owner, clock); });


        public class device_sound_interface_filter_volume : device_sound_interface
        {
            public device_sound_interface_filter_volume(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((filter_volume_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        device_sound_interface_filter_volume m_disound;


        sound_stream m_stream;
        float m_gain;


        filter_volume_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, FILTER_VOLUME, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_filter_volume(mconfig, this));  // device_sound_interface(mconfig, *this);
            m_disound = GetClassInterface<device_sound_interface_filter_volume>();


            m_stream = null;
            m_gain = 0;
        }


        public device_sound_interface_filter_volume disound { get { return m_disound; } }


        public void flt_volume_set_volume(float volume) { throw new emu_unimplemented(); }


        // device-level overrides
        protected override void device_start() { throw new emu_unimplemented(); }


        // sound stream update overrides
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            throw new emu_unimplemented();
        }
    }


    public static partial class flt_vol_global
    {
        public static filter_volume_device FILTER_VOLUME<bool_Required>(machine_config mconfig, device_finder<filter_volume_device, bool_Required> finder, u32 clock = 0) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, filter_volume_device.FILTER_VOLUME, clock); }
    }
}
