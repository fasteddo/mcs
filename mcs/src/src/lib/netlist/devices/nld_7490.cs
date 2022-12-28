// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_t_constructor_param_t = mame.netlist.core_device_data_t;  //using constructor_param_t = device_param_t;  //using device_param_t = const device_data_t &;  //using device_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;
using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using state_var_u8 = mame.netlist.state_var<System.Byte>;  //using state_var_u8 = state_var<std::uint8_t>;

using static mame.netlist.nltypes_global;
using static mame.nl_factory_global;


namespace mame.netlist.devices
{
    //NETLIB_OBJECT(7490)
    class nld_7490 : device_t
    {
        //NETLIB_DEVICE_IMPL(7490,     "TTL_7490",        "+A,+B,+R1,+R2,+R91,+R92,@VCC,@GND")
        public static readonly netlist.factory.constructor_ptr_t decl_7490 = NETLIB_DEVICE_IMPL<nld_7490>("TTL_7490", "+A,+B,+R1,+R2,+R91,+R92,@VCC,@GND");


        static readonly std.array<netlist_time, u64_const_4> delay = new std.array<netlist_time, u64_const_4>
        (
            NLTIME_FROM_NS(18),
            NLTIME_FROM_NS(36) - NLTIME_FROM_NS(18),
            NLTIME_FROM_NS(54) - NLTIME_FROM_NS(18),
            NLTIME_FROM_NS(72) - NLTIME_FROM_NS(18)
        );


        logic_input_t m_A;
        logic_input_t m_B;
        logic_input_t m_R1;
        logic_input_t m_R2;
        logic_input_t m_R91;
        logic_input_t m_R92;

        state_var_u8 m_cnt;
        state_var<netlist_sig_t> m_last_A;
        state_var<netlist_sig_t> m_last_B;

        object_array_t_logic_output_t<u64_const_4> m_Q;
        nld_power_pins m_power_pins;


        //NETLIB_CONSTRUCTOR(7490)
        public nld_7490(device_t_constructor_param_t data)
            : base(data)
        {
            m_A = new logic_input_t(this, "A", inputs);
            m_B = new logic_input_t(this, "B", inputs);
            m_R1 = new logic_input_t(this, "R1", inputs);
            m_R2 = new logic_input_t(this, "R2", inputs);
            m_R91 = new logic_input_t(this, "R91", inputs);
            m_R92 = new logic_input_t(this, "R92", inputs);
            m_cnt = new state_var_u8(this, "m_cnt", 0);
            m_last_A = new state_var<netlist_sig_t>(this, "m_last_A", 0);
            m_last_B = new state_var<netlist_sig_t>(this, "m_last_B", 0);
            m_Q = new object_array_t_logic_output_t<u64_const_4>(this, new logic_output_t(this, "QA"), new logic_output_t(this, "QB"), new logic_output_t(this, "QC"), new logic_output_t(this, "QD"));
            m_power_pins = new nld_power_pins(this);
        }


        //NETLIB_HANDLERI(inputs)
        void inputs()
        {
            netlist_sig_t new_A = m_A.op();
            netlist_sig_t new_B = m_B.op();

            if ((m_R91.op() & m_R92.op()) != 0)
            {
                m_cnt.op = 9;
                m_Q.push(9, delay);
            }
            else if ((m_R1.op() & m_R2.op()) != 0)
            {
                m_cnt.op = 0;
                m_Q.push(0, delay);
            }
            else
            {
                if (m_last_A.op != 0 && new_A == 0)  // High - Low
                {
                    m_cnt.op ^= 1;
                    m_Q.op(0).push(m_cnt.op & 1U, delay[0]);
                }

                if (m_last_B.op != 0 && new_B == 0)  // High - Low
                {
                    m_cnt.op += 2;
                    if (m_cnt.op >= 10)
                        m_cnt.op &= 1; /* Output A is not reset! */

                    m_Q.push(m_cnt.op, delay);
                }
            }

            m_last_A.op = new_A;
            m_last_B.op = new_B;
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            m_cnt.op = 0;
            m_last_A.op = 0;
            m_last_B.op = 0;
        }
    }
}
