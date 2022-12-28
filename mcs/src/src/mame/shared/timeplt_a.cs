// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;


namespace mame
{
    public class timeplt_audio_device : device_t
    {
        //DEFINE_DEVICE_TYPE(TIMEPLT_AUDIO, timeplt_audio_device, "timplt_audio", "Time Pilot Audio")
        public static readonly emu.detail.device_type_impl TIMEPLT_AUDIO = DEFINE_DEVICE_TYPE("timplt_audio", "Time Pilot Audio", (type, mconfig, tag, owner, clock) => { return new timeplt_audio_device(mconfig, tag, owner, clock); });


        required_device<cpu_device> m_soundcpu;

        // internal state
        required_device<generic_latch_8_device> m_soundlatch;
        required_device_array<filter_rc_device, u32_const_3> m_filter_0;
        required_device_array<filter_rc_device, u32_const_3> m_filter_1;

        uint8_t m_last_irq_state;


        timeplt_audio_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 14_318_181)
            : this(mconfig, TIMEPLT_AUDIO, tag, owner, clock)
        {
        }


        timeplt_audio_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_soundcpu = new required_device<cpu_device>(this, "tpsound");
            m_soundlatch = new required_device<generic_latch_8_device>(this, "soundlatch");
            m_filter_0 = new required_device_array<filter_rc_device, u32_const_3>(this, "filter.0.{0}", 0, (base_, tag_) => { return new device_finder<filter_rc_device, bool_const_true>(base_, tag_); });
            m_filter_1 = new required_device_array<filter_rc_device, u32_const_3>(this, "filter.1.{0}", 0, (base_, tag_) => { return new device_finder<filter_rc_device, bool_const_true>(base_, tag_); });
            m_last_irq_state = 0;
        }



        //void sound_data_w(uint8_t data);
        //DECLARE_WRITE_LINE_MEMBER(sh_irqtrigger_w);
        //DECLARE_WRITE_LINE_MEMBER(mute_w);


        // device-level overrides
        protected override void device_add_mconfig(machine_config config)
        {
            throw new emu_unimplemented();
        }


        protected override void device_start()
        {
            throw new emu_unimplemented();
        }


        //void filter_w(offs_t offset, uint8_t data);
        //uint8_t portB_r();

        //void timeplt_sound_map(address_map &map);

        //void set_filter(filter_rc_device &device, int data);
    }


    //class locomotn_audio_device : public timeplt_audio_device
}
