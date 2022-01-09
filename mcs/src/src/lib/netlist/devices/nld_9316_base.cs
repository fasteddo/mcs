// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using size_t = System.UInt64;
using unsigned = System.UInt32;

using static mame.nl_factory_global;


namespace mame.netlist.devices
{
    //template <typename D>
    //NETLIB_OBJECT(9316_base)
    class nld_9316_base : device_t
    {
        desc_base D;  // template parameter


        //template <unsigned N>
        //static constexpr unsigned rollover(unsigned v) noexcept { return v <= N ? v : 0; }
        //template <>
        //constexpr unsigned rollover<15>(unsigned v) noexcept { return v & 15; }
        static unsigned rollover(unsigned N, unsigned v) { return N == 15 ? (v & 15) : (v <= N ? v : 0); }


        logic_input_t m_CLK;
        logic_input_t m_ENT;

        logic_input_t m_LOADQ;

        logic_input_t m_ENP;
        logic_input_t m_CLRQ;

        object_array_t_logic_input_t<u64_const_4> m_ABCD;

        logic_output_t m_RC;
        object_array_t_logic_output_t<u64_const_4> m_Q;

        /* counter state */
        state_var<unsigned> m_cnt;
        /* cached pins */
        state_var<unsigned> m_abcd;
        state_var<unsigned> m_loadq;
        state_var<unsigned> m_ent;
        nld_power_pins m_power_pins;


        //NETLIB_CONSTRUCTOR(9316_base)
        public nld_9316_base(object owner, string name, desc_base desc)
            : base(owner, name)
        {
            D = desc;

            m_CLK = new logic_input_t(this, "CLK", clk);
            m_ENT = new logic_input_t(this, "ENT", other);
            m_LOADQ = new logic_input_t(this, "LOADQ", other);
            m_ENP = new logic_input_t(this, "ENP", other);
            m_CLRQ = new logic_input_t(this, "CLRQ", other);
            m_ABCD = new object_array_t_logic_input_t<u64_const_4>(this, new logic_input_t(this, "A", abcd), new logic_input_t(this, "B", abcd), new logic_input_t(this, "C", abcd), new logic_input_t(this, "D", abcd));
            m_RC = new logic_output_t(this, "RC");
            m_Q = new object_array_t_logic_output_t<u64_const_4>(this, new logic_output_t(this, "QA"), new logic_output_t(this, "QB"), new logic_output_t(this, "QC"), new logic_output_t(this, "QD"));
            m_cnt = new state_var<unsigned>(this, "m_cnt", 0);
            m_abcd = new state_var<unsigned>(this, "m_abcd", 0);
            m_loadq = new state_var<unsigned>(this, "m_loadq", 0);
            m_ent = new state_var<unsigned>(this, "m_ent", 0);
            m_power_pins = new nld_power_pins(this);
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            m_CLK.set_state(logic_t.state_e.STATE_INP_LH);
            m_cnt.op = 0;
            m_abcd.op = 0;
        }


        //NETLIB_HANDLERI(other)
        void other()
        {
            var CLRQ = m_CLRQ.op();
            m_ent.op = m_ENT.op();
            m_loadq.op = m_LOADQ.op();
            
            if ((((m_loadq.op ^ 1U) != 0) || (m_ent.op != 0 && m_ENP.op() != 0)) && (!D.ASYNC || CLRQ != 0))
            {
                m_CLK.activate_lh();
            }
            else
            {
                m_CLK.inactivate();
                if (D.ASYNC && (CLRQ == 0) && (m_cnt.op > 0))
                {
                    m_cnt.op = 0;
                    m_Q.push(0, D.tCLR(0));
                }
            }

            m_RC.push((m_ent.op != 0 && (m_cnt.op == D.MAXCNT)) ? 1U : 0U, D.tRC(0));
        }


        //NETLIB_HANDLERI(clk)
        void clk()
        {
            if (!D.ASYNC && m_CLRQ.op() == 0)
            {
                m_Q.push(0, D.tCLR(0));
                m_cnt.op = 0;
            }
            else
            {
                unsigned cnt = m_loadq.op != 0 ? rollover(D.MAXCNT, m_cnt.op + 1U) : m_abcd.op;
                m_RC.push((m_ent.op != 0 && (cnt == D.MAXCNT)) ? 1U : 0U, D.tRC(0));
                m_Q.push(cnt, D.tLDCNT(0));
                m_cnt.op = cnt;
            }
        }


        //NETLIB_HANDLERI(abcd)
        void abcd()
        {
            m_abcd.op = (unsigned)m_ABCD.op();
        }
    }


    class desc_9316 : desc_base
    {
        public override netlist_time delay(size_t N) { throw new emu_unimplemented(); }

        //using ASYNC  = desc_const_t<bool, true>;
        //using MAXCNT = desc_const_t<unsigned, 15>;
        //using tRC    = time_ns<27>;
        //using tCLR   = time_ns<36>;
        //using tLDCNT = time_ns<20>;
        public override bool ASYNC { get { return true; } }
        public override unsigned MAXCNT { get { return 15; } }
        public override netlist_time tRC(size_t N) { return times_ns1.value(27, N); }
        public override netlist_time tCLR(size_t N) { return times_ns1.value(36, N); }
        public override netlist_time tLDCNT(size_t N) { return times_ns1.value(20, N); }
    }

    //class desc_9310 : desc_base
    //{
    //    //using ASYNC  = desc_const_t<bool, true>;
    //    //using MAXCNT = desc_const_t<unsigned, 10>;
    //    //using tRC    = time_ns<27>;
    //    //using tCLR   = time_ns<36>;
    //    //using tLDCNT = time_ns<20>;
    //}

    //class desc_74161 : desc_base
    //{
    //    //using ASYNC  = desc_const_t<bool, true>;
    //    //using MAXCNT = desc_const_t<unsigned, 15>;
    //    //using tRC    = time_ns<27>;
    //    //using tCLR   = time_ns<36>;
    //    //using tLDCNT = time_ns<20>;
    //}

    //class desc_74163 : desc_base
    //{
    //    //using ASYNC  = desc_const_t<bool, false>;
    //    //using MAXCNT = desc_const_t<unsigned, 15>;
    //    //using tRC    = time_ns<27>;
    //    //using tCLR   = time_ns<36>;
    //    //using tLDCNT = time_ns<20>;
    //}


    //using NETLIB_NAME(9310) = NETLIB_NAME(9316_base)<desc_9310>;


    //using NETLIB_NAME(9316) = NETLIB_NAME(9316_base)<desc_9316>;
    class nld_9316 : nld_9316_base
    {
        //NETLIB_DEVICE_IMPL(9316,     "TTL_9316",     "+CLK,+ENP,+ENT,+CLRQ,+LOADQ,+A,+B,+C,+D,@VCC,@GND")
        public static readonly netlist.factory.constructor_ptr_t decl_9316 = NETLIB_DEVICE_IMPL<nld_9316>("TTL_9316", "+CLK,+ENP,+ENT,+CLRQ,+LOADQ,+A,+B,+C,+D,@VCC,@GND");

        public nld_9316(object owner, string name)
            : base(owner, name, new desc_9316())
        { }
    }


    //using NETLIB_NAME(74161) = NETLIB_NAME(9316_base)<desc_74161>;
    //using NETLIB_NAME(74161_fixme) = NETLIB_NAME(9316_base)<desc_74161>;
    //using NETLIB_NAME(74163) = NETLIB_NAME(9316_base)<desc_74163>;
}
