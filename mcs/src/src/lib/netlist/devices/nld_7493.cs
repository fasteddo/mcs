// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_t_constructor_param_t = mame.netlist.core_device_data_t;  //using constructor_param_t = device_param_t;  //using device_param_t = const device_data_t &;  //using device_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using unsigned = System.UInt32;

using static mame.netlist.nltypes_global;
using static mame.nl_factory_global;


namespace mame.netlist.devices
{
    //NETLIB_OBJECT(7493)
    class nld_7493 : device_t
    {
        //NETLIB_DEVICE_IMPL(7493,        "TTL_7493", "+CLKA,+CLKB,+R1,+R2,@VCC,@GND")
        public static readonly netlist.factory.constructor_ptr_t decl_7493 = NETLIB_DEVICE_IMPL<nld_7493>("TTL_7493", "+CLKA,+CLKB,+R1,+R2,@VCC,@GND");


        static readonly std.array<netlist_time, u64_const_3> out_delay = new std.array<netlist_time, u64_const_3>(NLTIME_FROM_NS(18), NLTIME_FROM_NS(36), NLTIME_FROM_NS(54));


        logic_input_t m_CLKA;
        logic_input_t m_CLKB;

        logic_output_t m_QA;
        object_array_t_logic_output_t<u64_const_3> m_QBCD;

        state_var<unsigned> m_a;
        state_var<unsigned> m_bcd;

        logic_input_t m_R1;
        logic_input_t m_R2;

        nld_power_pins m_power_pins;


        //NETLIB_CONSTRUCTOR(7493)
        public nld_7493(device_t_constructor_param_t data)
            : base(data)
        {
            m_CLKA = new logic_input_t(this, "CLKA", updA);
            m_CLKB = new logic_input_t(this, "CLKB", updB);
            m_QA = new logic_output_t(this, "QA");
            m_QBCD = new object_array_t_logic_output_t<u64_const_3>(this, new logic_output_t(this, "QB"), new logic_output_t(this, "QC"), new logic_output_t(this, "QD"));
            m_a = new state_var<unsigned>(this, "m_a", 0);
            m_bcd = new state_var<unsigned>(this, "m_b", 0);
            m_R1 = new logic_input_t(this, "R1", inputs);
            m_R2 = new logic_input_t(this, "R2", inputs);
            m_power_pins = new nld_power_pins(this);
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            m_a.op = m_bcd.op = 0;
            m_CLKA.set_state(logic_t.state_e.STATE_INP_HL);
            m_CLKB.set_state(logic_t.state_e.STATE_INP_HL);
        }

        //NETLIB_HANDLERI(inputs)
        void inputs()
        {
            if (!(m_R1.op() != 0 && m_R2.op() != 0))
            {
                m_CLKA.activate_hl();
                m_CLKB.activate_hl();
            }
            else
            {
                m_CLKA.inactivate();
                m_CLKB.inactivate();
                m_QA.push(0, NLTIME_FROM_NS(40));
                m_QBCD.push(0, NLTIME_FROM_NS(40));
                m_a.op = m_bcd.op = 0;
            }
        }

        //NETLIB_HANDLERI(updA)
        void updA()
        {
            m_a.op ^= 1;
            m_QA.push(m_a.op, out_delay[0]);
        }

        //NETLIB_HANDLERI(updB)
        void updB()
        {
            ++m_bcd.op;
            var cnt = (m_bcd.op &= 0x07);
            m_QBCD.push(cnt, out_delay);
        }
    }
}
