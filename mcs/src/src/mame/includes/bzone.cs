// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;


namespace mame
{
    partial class bzone_state : driver_device
    {
        static readonly XTAL BZONE_MASTER_CLOCK = new XTAL(12_096_000);
        static readonly XTAL BZONE_CLOCK_3KHZ   = BZONE_MASTER_CLOCK / 4096;


        required_device<m6502_device> m_maincpu;  //required_device<cpu_device> m_maincpu;
        required_device<mathbox_device> m_mathbox;
        optional_device<discrete_device> m_discrete;
        required_device<screen_device> m_screen;
        output_finder<u32_const_1> m_startled;  //output_finder<> m_startled;

        uint8_t m_analog_data;


        public bzone_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_maincpu = new required_device<m6502_device>(this, "maincpu");
            m_mathbox = new required_device<mathbox_device>(this, "mathbox");
            m_discrete = new optional_device<discrete_device>(this, "discrete");
            m_screen = new required_device<screen_device>(this, "screen");
            m_startled = new output_finder<u32_const_1>(this, "startled", 0);
        }
    }


    //class redbaron_state : public bzone_state
}
