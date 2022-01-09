// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using size_t = System.UInt64;
using unsigned = System.UInt32;

using static mame.nl_factory_global;


namespace mame.netlist.devices
{
    //template <typename D>
    //NETLIB_OBJECT(74107_base)
    class nld_74107_base : device_t
    {
        desc_base D;  // template parameter

        logic_input_t m_clk;

        logic_input_t m_J;
        logic_input_t m_K;
        logic_input_t m_clrQ;

        logic_output_t m_Q;
        logic_output_t m_QQ;

        nld_power_pins m_power_pins;


        //NETLIB_CONSTRUCTOR(74107_base)
        public nld_74107_base(object owner, string name, desc_base desc)
            : base(owner, name)
        {
            D = desc;

            m_clk = new logic_input_t(this, "CLK", clk);
            m_J = new logic_input_t(this, "J", other);
            m_K = new logic_input_t(this, "K", other);
            m_clrQ = new logic_input_t(this, "CLRQ", other);
            m_Q = new logic_output_t(this, "Q");
            m_QQ = new logic_output_t(this, "QQ");
            m_power_pins = new nld_power_pins(this);
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            m_clk.set_state(logic_t.state_e.STATE_INP_HL);
            ////m_Q.initial(0);
            ////m_QQ.initial(1);
        }


        //NETLIB_HANDLERI(other)
        void other()
        {
            if (m_clrQ.op() == 0)
            {
                m_clk.inactivate();
                newstate(0);
            }
            else if ((m_J.op() | m_K.op()) != 0)
            {
                m_clk.activate_hl();
            }
        }


        //NETLIB_HANDLERI(clk)
        void clk()
        {
            netlist_sig_t t = m_Q.net().Q();
            /*
             *  J K  Q1 Q2 F t   Q
             *  0 0   0  1 0 0   0
             *  0 1   0  0 0 0   0
             *  1 0   0  0 1 0   1
             *  1 1   1  0 0 0   1
             *  0 0   0  1 0 1   1
             *  0 1   0  0 0 1   0
             *  1 0   0  0 1 1   1
             *  1 1   1  0 0 1   0
             */
            if (((m_J.op() & m_K.op()) ^ 1) != 0)
                m_clk.inactivate();

            newstate(((t ^ 1) & m_J.op()) | (t & (m_K.op() ^ 1)));
        }


        void newstate(netlist_sig_t state)
        {
            m_Q.push(state, D.delay(state));
            m_QQ.push(state ^ 1, D.delay(state ^ 1));
        }
    }


    class desc_74107 : desc_base
    {
        //using delay = times_ns2<16, 25>;
        public override netlist_time delay(size_t N) { return times_ns2.value(16, 25, N); }
        public override bool ASYNC { get { throw new emu_unimplemented(); } }
        public override unsigned MAXCNT { get { throw new emu_unimplemented(); } }
        public override netlist_time tRC(size_t N) { throw new emu_unimplemented(); }
        public override netlist_time tCLR(size_t N) { throw new emu_unimplemented(); }
        public override netlist_time tLDCNT(size_t N) { throw new emu_unimplemented(); }
    }

    //class desc_74107A : desc_base
    //{
    //    //using delay = times_ns2<15, 15>;
    //}


    //using NETLIB_NAME(74107) = NETLIB_NAME(74107_base)<desc_74107>;
    class nld_74107 : nld_74107_base
    {
        //NETLIB_DEVICE_IMPL(74107,       "TTL_74107",    "+CLK,+J,+K,+CLRQ,@VCC,@GND")
        public static readonly netlist.factory.constructor_ptr_t decl_74107 = NETLIB_DEVICE_IMPL<nld_74107>("TTL_74107", "+CLK,+J,+K,+CLRQ,@VCC,@GND");

        public nld_74107(object owner, string name)
            : base(owner, name, new desc_74107())
        { }
    }

    //using NETLIB_NAME(74107A) = NETLIB_NAME(74107_base)<desc_74107A>;
    //NETLIB_DEVICE_IMPL(74107A,      "TTL_74107A",   "+CLK,+J,+K,+CLRQ,@VCC,@GND")
}
