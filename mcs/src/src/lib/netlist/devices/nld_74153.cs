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
    // FIXME: timing is not 100% accurate, Strobe and Select inputs have a
    //        slightly longer timing .
    // FIXME: Truth table candidate

    //NETLIB_OBJECT(74153)
    class nld_74153 : device_t
    {
        //NETLIB_DEVICE_IMPL(74153, "TTL_74153", "+C0,+C1,+C2,+C3,+A,+B,+G,@VCC,@GND")
        public static readonly netlist.factory.constructor_ptr_t decl_74153 = NETLIB_DEVICE_IMPL<nld_74153>("TTL_74153", "+C0,+C1,+C2,+C3,+A,+B,+G,@VCC,@GND");


        object_array_t_logic_input_t<u64_const_4> m_C;
        logic_input_t m_G;

        logic_output_t m_Y;

        state_var<unsigned> m_chan;

        logic_input_t m_A;
        logic_input_t m_B;

        nld_power_pins m_power_pins;


        //NETLIB_CONSTRUCTOR(74153)
        public nld_74153(device_t_constructor_param_t data)
            : base(data)
        {
            m_C = new object_array_t_logic_input_t<u64_const_4>(this, new logic_input_t(this, "C0", sub), new logic_input_t(this, "C1", sub), new logic_input_t(this, "C2", sub), new logic_input_t(this, "C3", sub));
            m_G = new logic_input_t(this, "G", sub);
            m_Y = new logic_output_t(this, "AY"); //FIXME: Change netlists
            m_chan = new state_var<unsigned>(this, "m_chan", 0);
            m_A = new logic_input_t(this, "A", other);
            m_B = new logic_input_t(this, "B", other);
            m_power_pins = new nld_power_pins(this);
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            m_chan.op = 0;
        }

        //NETLIB_HANDLERI(other)
        void other()
        {
            m_chan.op = (m_A.op() | (m_B.op() << 1));
            sub();
        }

        //NETLIB_HANDLERI(sub)
        void sub()
        {
            std.array<netlist_time, u64_const_2> delay = new std.array<netlist_time, u64_const_2>(NLTIME_FROM_NS(23), NLTIME_FROM_NS(18));
            if (m_G.op() == 0)
            {
                var t = m_C.op(m_chan.op).op();
                m_Y.push(t, delay[t]);
            }
            else
            {
                m_Y.push(0, delay[0]);
            }
        }
    }
}
