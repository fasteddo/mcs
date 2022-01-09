// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using param_model_t_value_t = mame.netlist.param_model_t.value_base_t<System.Double, mame.netlist.param_model_t.value_base_t_operators_double>;  //using value_t = value_base_t<nl_fptype>;
using unsigned = System.UInt32;

using static mame.netlist.nl_errstr_global;
using static mame.nl_factory_global;


namespace mame.netlist
{
    namespace analog
    {
        class diode
        {
            nl_fptype m_Is;
            nl_fptype m_VT;
            nl_fptype m_VT_inv;


            public diode()
            {
                m_Is = nlconst.np_Is();
                m_VT = nlconst.np_VT();
                m_VT_inv = plib.pg.reciprocal(m_VT);
            }

            public diode(nl_fptype Is, nl_fptype n)
            {
                m_Is = Is;
                m_VT = nlconst.np_VT(n);
                m_VT_inv = plib.pg.reciprocal(m_VT);
            }


            void set(nl_fptype Is, nl_fptype n)
            {
                m_Is = Is;
                m_VT = nlconst.np_VT(n);
                m_VT_inv = plib.pg.reciprocal(m_VT);
            }


            nl_fptype I(nl_fptype V) { return m_Is * plib.pg.exp(V * m_VT_inv) - m_Is; }
            nl_fptype g(nl_fptype V) { return m_Is * m_VT_inv * plib.pg.exp(V * m_VT_inv); }
            public nl_fptype V(nl_fptype I) { return plib.pg.log1p(I / m_Is) * m_VT; } // log1p(x)=log(1.0 + x)
            public nl_fptype gI(nl_fptype I) { return m_VT_inv * (I + m_Is); }
        }


        // -----------------------------------------------------------------------------
        // nld_Q - Base classes
        // -----------------------------------------------------------------------------

        public enum bjt_type
        {
            BJT_NPN,
            BJT_PNP
        }


        class bjt_model_t
        {
            public bjt_type m_type;
            public param_model_t_value_t m_IS;  //!< transport saturation current
            public param_model_t_value_t m_BF;  //!< ideal maximum forward beta
            public param_model_t_value_t m_NF;  //!< forward current emission coefficient
            param_model_t_value_t m_BR;  //!< ideal maximum reverse beta
            param_model_t_value_t m_NR;  //!< reverse current emission coefficient
            param_model_t_value_t m_CJE; //!< B-E zero-bias depletion capacitance
            param_model_t_value_t m_CJC; //!< B-C zero-bias depletion capacitance


            public bjt_model_t(param_model_t model)
            {
                m_type = (model.type() == "NPN") ? bjt_type.BJT_NPN : bjt_type.BJT_PNP;
                m_IS  = new param_model_t_value_t(model, "IS");
                m_BF  = new param_model_t_value_t(model, "BF");
                m_NF  = new param_model_t_value_t(model, "NF");
                m_BR  = new param_model_t_value_t(model, "BR");
                m_NR  = new param_model_t_value_t(model, "NR");
                m_CJE = new param_model_t_value_t(model, "CJE");
                m_CJC = new param_model_t_value_t(model, "CJC");
            }
        }


        // Have a common start for transistors

        //NETLIB_BASE_OBJECT(QBJT)
        public class nld_QBJT : base_device_t
        {
            protected param_model_t m_model;

            bjt_type m_qtype;


            //NETLIB_CONSTRUCTOR_EX(QBJT, const pstring &model = "NPN")
            public nld_QBJT(object owner, string name, string model = "NPN")
                : base(owner, name)
            {
                m_model = new param_model_t(this, "MODEL", model);
                m_qtype = bjt_type.BJT_NPN;
            }


            //NETLIB_IS_DYNAMIC(true)
            public override bool is_dynamic() { return true; }


            //NETLIB_RESETI();

            //bjt_type qtype() const noexcept { return m_qtype; }
            //bool is_qtype(bjt_type atype) const noexcept { return m_qtype == atype; }
            protected void set_qtype(bjt_type atype) { m_qtype = atype; }
        }


        // -----------------------------------------------------------------------------
        // nld_QBJT_switch
        // -----------------------------------------------------------------------------

        //NETLIB_OBJECT_DERIVED(QBJT_switch, QBJT)
        public class nld_QBJT_switch : nld_QBJT
        {
            //NETLIB_DEVICE_IMPL_NS(analog, QBJT_switch, "QBJT_SW", "MODEL")
            public static readonly factory.constructor_ptr_t decl_QBJT_switch = NETLIB_DEVICE_IMPL_NS<nld_QBJT_switch>("analog", "QBJT_SW", "MODEL");


            bjt_model_t m_modacc;
            nld_twoterm m_RB;
            nld_twoterm m_RC;
            nld_twoterm m_BC;

            nl_fptype m_gB; // base conductance / switch on
            nl_fptype m_gC; // collector conductance / switch on
            nl_fptype m_V; // internal voltage source
            state_var<unsigned> m_state_on;


            //NETLIB_CONSTRUCTOR(QBJT_switch)
            public nld_QBJT_switch(object owner, string name)
                : base(owner, name)
            {
                m_modacc = new bjt_model_t(m_model);
                m_RB = new nld_twoterm(this, "m_RB", termhandler);
                m_RC = new nld_twoterm(this, "m_RC", termhandler);
                m_BC = new nld_twoterm(this, "m_BC", termhandler);
                m_gB = nlconst.cgmin();
                m_gC = nlconst.cgmin();
                m_V = nlconst.zero();
                m_state_on = new state_var<unsigned>(this, "m_state_on", 0U);


                register_subalias("B", m_RB.P());
                register_subalias("E", m_RB.N());
                register_subalias("C", m_RC.P());

                connect(m_RB.N(), m_RC.N());
                connect(m_RB.P(), m_BC.P());
                connect(m_RC.P(), m_BC.N());
            }


            //NETLIB_RESETI();
            //NETLIB_RESET(QBJT_switch)
            public override void reset()
            {
                if (m_RB.solver() == null && m_RC.solver() == null)
                    throw new nl_exception(MF_DEVICE_FRY_1(this.name()));

                base.reset();  //NETLIB_NAME(QBJT)::reset();

                var zero = nlconst.zero();

                m_state_on.op = 0;

                m_RB.set_G_V_I(exec().gmin(), zero, zero);
                m_RC.set_G_V_I(exec().gmin(), zero, zero);

                m_BC.set_G_V_I(exec().gmin() / nlconst.magic(10.0), zero, zero);
            }


            //NETLIB_HANDLERI(termhandler)
            void termhandler()
            {
                var solv = m_RB.solver();
                if (solv != null)
                    solv.solve_now();
                else
                    m_RC.solver().solve_now();
            }


            //NETLIB_UPDATE_PARAMI();
            //NETLIB_UPDATE_PARAM(QBJT_switch)
            public override void update_param()
            {
                nl_fptype IS = m_modacc.m_IS.op();
                nl_fptype BF = m_modacc.m_BF.op();
                nl_fptype NF = m_modacc.m_NF.op();
                //nl_fptype VJE = m_modacc.dValue("VJE", 0.75);

                // FIXME: check for PNP as well and bail out
                set_qtype(m_modacc.m_type);

                nl_fptype alpha = BF / (nlconst.one() + BF);

                diode d = new diode(IS, NF);

                // Assume 5mA Collector current for switch operation

                var cc = nlconst.magic(0.005);
                m_V = d.V(cc / alpha);

                // Base current is 0.005 / beta
                // as a rough estimate, we just scale the conductance down

                m_gB = plib.pg.reciprocal(m_V / (cc / BF));

                //m_gB = d.gI(0.005 / alpha);

                if (m_gB < exec().gmin())
                    m_gB = exec().gmin();

                m_gC =  d.gI(cc); // very rough estimate
            }


            //NETLIB_UPDATE_TERMINALSI();
            //NETLIB_UPDATE_TERMINALS(QBJT_switch)
            public override void update_terminals()
            {
                throw new emu_unimplemented();
                //const nl_fptype m = (is_qtype( bjt_type::BJT_NPN) ? 1 : -1);
                //
                //const unsigned new_state = (m_RB.deltaV() * m > m_V ) ? 1 : 0;
                //if (m_state_on ^ new_state)
                //{
                //    const auto zero(nlconst::zero());
                //    const nl_fptype gb = new_state ? m_gB : exec().gmin();
                //    const nl_fptype gc = new_state ? m_gC : exec().gmin();
                //    const nl_fptype v  = new_state ? m_V * m : zero;
                //
                //    m_RB.set_G_V_I(gb,   v,   zero);
                //    m_RC.set_G_V_I(gc, zero, zero);
                //    m_state_on = new_state;
                //}
            }
        }


        // -----------------------------------------------------------------------------
        // nld_QBJT_EB
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT_DERIVED(QBJT_EB, QBJT)
    }
}
