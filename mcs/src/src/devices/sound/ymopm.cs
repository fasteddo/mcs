// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using u8 = System.Byte;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.ymopm_global;


namespace mame
{
    // ======================> ym2151_device
    public class ym2151_device : ymfm_device_base<ymfm.ym2151, ymfm.opm_registers, ymfm.fm_engine_base_operators_opm_registers>
    {
        //DEFINE_DEVICE_TYPE(YM2151, ym2151_device, "ym2151", "YM2151 OPM")
        public static readonly emu.detail.device_type_impl YM2151 = DEFINE_DEVICE_TYPE("ym2151", "YM2151 OPM", (type, mconfig, tag, owner, clock) => { return new ym2151_device(mconfig, tag, owner, clock); });


        //using parent = ymfm_device_base<ymfm::ym2151>;


        // internal state
        uint8_t m_reset_state;           // reset line state


        // constructor
        ym2151_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, tag, owner, clock, YM2151, (intf) => { return new ymfm.ym2151(intf); })
        {
            m_reset_state = 1;
        }


        // configuration helpers, handled by the interface
        //auto port_write_handler() { return io_write_handler(); }

        // write access, handled by the chip implementation
        public override void write(offs_t offset, u8 data) { throw new emu_unimplemented(); }
        protected override void address_w(u8 data) { throw new emu_unimplemented(); }
        protected override void data_w(u8 data) { throw new emu_unimplemented(); }


        // reset line, active LOW
        //DECLARE_WRITE_LINE_MEMBER(reset_w);
        public void reset_w(int state) { throw new emu_unimplemented(); }
    }


    // ======================> ym2164_device
    //DECLARE_DEVICE_TYPE(YM2164, ym2164_device);
    //class ym2164_device : public ymfm_device_base<ymfm::ym2164>


    public static class ymopm_global
    {
        public static ym2151_device YM2151<bool_Required>(machine_config mconfig, device_finder<ym2151_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, ym2151_device.YM2151, clock); }
    }
}
