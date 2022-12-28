// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_t_constructor_param_t = mame.netlist.core_device_data_t;  //using constructor_param_t = device_param_t;  //using device_param_t = const device_data_t &;  //using device_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using uint_fast8_t = System.Byte;

using static mame.netlist.nltypes_global;
using static mame.nl_factory_global;


namespace mame.netlist.devices
{
    // FIXME: timing, see 74107 for example, use template

    //NETLIB_OBJECT(7450)
    class nld_7450 : device_t
    {
        //NETLIB_DEVICE_IMPL(7450,        "TTL_7450_ANDORINVERT", "+A,+B,+C,+D,@VCC,@GND")
        public static readonly netlist.factory.constructor_ptr_t decl_7450 = NETLIB_DEVICE_IMPL<nld_7450>("TTL_7450_ANDORINVERT", "+A,+B,+C,+D,@VCC,@GND");


        static readonly std.array<netlist_time, u64_const_2> times = new std.array<netlist_time, u64_const_2>(NLTIME_FROM_NS(15), NLTIME_FROM_NS(22));


        logic_input_t m_A;
        logic_input_t m_B;
        logic_input_t m_C;
        logic_input_t m_D;
        logic_output_t m_Q;
        nld_power_pins m_power_pins;


        //NETLIB_CONSTRUCTOR(7450)
        public nld_7450(device_t_constructor_param_t data)
            : base(data)
        {
            m_A = new logic_input_t(this, "A", inputs);
            m_B = new logic_input_t(this, "B", inputs);
            m_C = new logic_input_t(this, "C", inputs);
            m_D = new logic_input_t(this, "D", inputs);
            m_Q = new logic_output_t(this, "Q");
            m_power_pins = new nld_power_pins(this);
        }


        //NETLIB_RESETI();


        //NETLIB_HANDLERI(inputs)
        void inputs()
        {
            m_A.activate();
            m_B.activate();
            m_C.activate();
            m_D.activate();
            var t1 = m_A.op() & m_B.op();
            var t2 = m_C.op() & m_D.op();

            uint_fast8_t res = 0;
            if ((t1 ^ 1) != 0)
            {
                if ((t2 ^ 1) != 0)
                {
                    res = 1;
                }
                else
                {
                    m_A.inactivate();
                    m_B.inactivate();
                }
            }
            else
            {
                if ((t2 ^ 1) != 0)
                {
                    m_C.inactivate();
                    m_D.inactivate();
                }
            }

            m_Q.push(res, times[res]);// ? 22000 : 15000);
        }
    }
}
