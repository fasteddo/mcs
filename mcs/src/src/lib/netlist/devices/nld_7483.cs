// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_t_constructor_param_t = mame.netlist.core_device_data_t;  //using constructor_param_t = device_param_t;  //using device_param_t = const device_data_t &;  //using device_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;
using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using state_var_u8 = mame.netlist.state_var<System.Byte>;  //using state_var_u8 = state_var<std::uint8_t>;
using uint8_t = System.Byte;

using static mame.netlist.nltypes_global;
using static mame.nl_factory_global;


namespace mame.netlist.devices
{
    //NETLIB_OBJECT(7483)
    class nld_7483 : device_t
    {
        //NETLIB_DEVICE_IMPL(7483, "TTL_7483", "+A1,+A2,+A3,+A4,+B1,+B2,+B3,+B4,+C0,@VCC,@GND")
        public static readonly netlist.factory.constructor_ptr_t decl_7483 = NETLIB_DEVICE_IMPL<nld_7483>("TTL_7483", "+A1,+A2,+A3,+A4,+B1,+B2,+B3,+B4,+C0,@VCC,@GND");


        logic_input_t m_C0;
        logic_input_t m_A1;
        logic_input_t m_A2;
        logic_input_t m_A3;
        logic_input_t m_A4;
        logic_input_t m_B1;
        logic_input_t m_B2;
        logic_input_t m_B3;
        logic_input_t m_B4;

        state_var_u8 m_a;
        state_var_u8 m_b;
        state_var_u8 m_lastr;

        logic_output_t m_S1;
        logic_output_t m_S2;
        logic_output_t m_S3;
        logic_output_t m_S4;
        logic_output_t m_C4;
        nld_power_pins m_power_pins;


        //NETLIB_CONSTRUCTOR(7483)
        public nld_7483(device_t_constructor_param_t data)
            : base(data)
        {
            m_C0 = new logic_input_t(this, "C0", c0);
            m_A1 = new logic_input_t(this, "A1", upd_a);
            m_A2 = new logic_input_t(this, "A2", upd_a);
            m_A3 = new logic_input_t(this, "A3", upd_a);
            m_A4 = new logic_input_t(this, "A4", upd_a);
            m_B1 = new logic_input_t(this, "B1", upd_b);
            m_B2 = new logic_input_t(this, "B2", upd_b);
            m_B3 = new logic_input_t(this, "B3", upd_b);
            m_B4 = new logic_input_t(this, "B4", upd_b);
            m_a = new state_var_u8(this, "m_a", 0);
            m_b = new state_var_u8(this, "m_b", 0);
            m_lastr = new state_var_u8(this, "m_lastr", 0);
            m_S1 = new logic_output_t(this, "S1");
            m_S2 = new logic_output_t(this, "S2");
            m_S3 = new logic_output_t(this, "S3");
            m_S4 = new logic_output_t(this, "S4");
            m_C4 = new logic_output_t(this, "C4");
            m_power_pins = new nld_power_pins(this);
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            m_lastr.op = 0;
        }


        //NETLIB_HANDLERI(c0)
        void c0()
        {
            var r = (uint8_t)(m_a.op + m_b.op + m_C0.op());  //auto r = static_cast<uint8_t>(m_a + m_b + m_C0());

            if (r != m_lastr.op)
            {
                m_lastr.op = r;
                m_S1.push((netlist_sig_t)((r >> 0) & 1U), NLTIME_FROM_NS(23));
                m_S2.push((netlist_sig_t)((r >> 1) & 1U), NLTIME_FROM_NS(23));
                m_S3.push((netlist_sig_t)((r >> 2) & 1U), NLTIME_FROM_NS(23));
                m_S4.push((netlist_sig_t)((r >> 3) & 1U), NLTIME_FROM_NS(23));
                m_C4.push((netlist_sig_t)((r >> 4) & 1U), NLTIME_FROM_NS(23));
            }
        }

        //NETLIB_HANDLERI(upd_a)
        void upd_a()
        {
            m_a.op = (uint8_t)((m_A1.op() << 0) | (m_A2.op() << 1) | (m_A3.op() << 2) | (m_A4.op() << 3));  //m_a = static_cast<uint8_t>((m_A1() << 0) | (m_A2() << 1) | (m_A3() << 2) | (m_A4() << 3));
            c0();
        }

        //NETLIB_HANDLERI(upd_b)
        void upd_b()
        {
            m_b.op = (uint8_t)((m_B1.op() << 0) | (m_B2.op() << 1) | (m_B3.op() << 2) | (m_B4.op() << 3));  //m_b = static_cast<uint8_t>((m_B1() << 0) | (m_B2() << 1) | (m_B3() << 2) | (m_B4() << 3));
            c0();
        }
    }
}
