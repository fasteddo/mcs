// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;


namespace mame
{
    partial class digdug_state : galaga_state
    {
        required_device<er2055_device> m_earom;
        required_shared_ptr<uint8_t> m_digdug_objram;
        required_shared_ptr<uint8_t> m_digdug_posram;
        required_shared_ptr<uint8_t> m_digdug_flpram;

        uint8_t m_bg_select = 0;
        uint8_t m_tx_color_mode = 0;
        uint8_t m_bg_disable = 0;
        uint8_t m_bg_color_bank = 0;


        public digdug_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_earom = new required_device<er2055_device>(this, "earom");
            m_digdug_objram = new required_shared_ptr<uint8_t>(this, "digdug_objram");
            m_digdug_posram = new required_shared_ptr<uint8_t>(this, "digdug_posram");
            m_digdug_flpram = new required_shared_ptr<uint8_t>(this, "digdug_flpram");
        }
    }
}
