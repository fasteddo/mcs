// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_t_constructor_param_t = mame.netlist.core_device_data_t;  //using constructor_param_t = device_param_t;  //using device_param_t = const device_data_t &;  //using device_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;
using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;

using static mame.netlist.nltypes_global;
using static mame.nl_factory_global;


namespace mame.netlist.devices
{
    //NETLIB_OBJECT(7474)
    class nld_7474 : device_t
    {
        //NETLIB_DEVICE_IMPL(7474, "TTL_7474", "+CLK,+D,+CLRQ,+PREQ,@VCC,@GND")
        public static readonly netlist.factory.constructor_ptr_t decl_7474 = NETLIB_DEVICE_IMPL<nld_7474>("TTL_7474", "+CLK,+D,+CLRQ,+PREQ,@VCC,@GND");


        logic_input_t m_D;
        logic_input_t m_CLRQ;
        logic_input_t m_PREQ;
        logic_input_t m_CLK;
        logic_output_t m_Q;
        logic_output_t m_QQ;

        state_var<netlist_sig_t> m_nextD;

        nld_power_pins m_power_pins;


        //NETLIB_CONSTRUCTOR(7474)
        public nld_7474(device_t_constructor_param_t data)
            : base(data)
        {
            m_D = new logic_input_t(this, "D", inputs);
            m_CLRQ = new logic_input_t(this, "CLRQ", inputs);
            m_PREQ = new logic_input_t(this, "PREQ", inputs);
            m_CLK = new logic_input_t(this, "CLK", clk);
            m_Q = new logic_output_t(this, "Q");
            m_QQ = new logic_output_t(this, "QQ");
            m_nextD = new state_var<netlist_sig_t>(this, "m_nextD", 0);
            m_power_pins = new nld_power_pins(this);
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            m_CLK.set_state(logic_t.state_e.STATE_INP_LH);
            m_D.set_state(logic_t.state_e.STATE_INP_ACTIVE);
            m_nextD.op = 0;
        }


        //NETLIB_HANDLERI(clk)
        void clk()
        {
            newstate(m_nextD.op, m_nextD.op == 0 ? 1U : 0U);
            m_CLK.inactivate();
        }


        //NETLIB_HANDLERI(inputs)
        void inputs()
        {
            var preq = m_PREQ.op();
            var clrq = m_CLRQ.op();
            if ((preq & clrq) != 0)
            {
                m_D.activate();
                m_nextD.op = m_D.op();
                m_CLK.activate_lh();
            }
            else
            {
                newstate(preq ^ 1, clrq ^ 1);
                m_CLK.inactivate();
                m_D.inactivate();
            }
        }


        void newstate(netlist_sig_t stateQ, netlist_sig_t stateQQ)
        {
            // 0: High-to-low 40 ns, 1: Low-to-high 25 ns
            std.array<netlist_time, u64_const_2> delay = new std.array<netlist_time, u64_const_2>(NLTIME_FROM_NS(40), NLTIME_FROM_NS(25));
            m_Q.push(stateQ, delay[stateQ]);
            m_QQ.push(stateQQ, delay[stateQQ]);
        }
    }
}
