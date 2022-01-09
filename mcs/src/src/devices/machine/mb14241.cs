// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;

using static mame.device_global;
using static mame.emucore_global;


namespace mame
{
    public class mb14241_device : device_t
    {
        //DEFINE_DEVICE_TYPE(MB14241, mb14241_device, "mb14241", "MB14241 Data Shifter")
        static device_t device_creator_mb14241_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new mb14241_device(mconfig, tag, owner, clock); }
        public static readonly device_type MB14241 = DEFINE_DEVICE_TYPE(device_creator_mb14241_device, "mb14241", "MB14241 Data Shifter");


        // internal state
        u16 m_shift_data;  // 15 bits only
        u8 m_shift_count;  // 3 bits


        mb14241_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : base(mconfig, MB14241, tag, owner, clock)
        {
            m_shift_data = 0;
            m_shift_count = 0;
        }


        public void shift_count_w(u8 data)
        {
            m_shift_count = (u8)(~data & 0x07);
        }

        public void shift_data_w(u8 data)
        {
            m_shift_data = (u16)((m_shift_data >> 8) | ((u16)data << 7));
        }

        public u8 shift_result_r()
        {
            return (u8)((m_shift_data >> m_shift_count) & 0x00ff);
        }


        // device-level overrides
        protected override void device_start()
        {
            save_item(NAME(new { m_shift_data }));
            save_item(NAME(new { m_shift_count }));
        }


        protected override void device_reset()
        {
            m_shift_data = 0;
            m_shift_count = 0;
        }
    }
}
