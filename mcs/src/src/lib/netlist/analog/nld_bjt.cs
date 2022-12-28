// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using base_device_t_constructor_param_t = mame.netlist.core_device_data_t;  //using constructor_param_t = base_device_param_t;  //using base_device_param_t = const base_device_data_t &;  //using base_device_data_t = core_device_data_t;
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
            public param_model_t_value_t m_CJE; //!< B-E zero-bias depletion capacitance
            public param_model_t_value_t m_CJC; //!< B-C zero-bias depletion capacitance


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


        // -----------------------------------------------------------------------------
        // nld_QBJT_switch
        // -----------------------------------------------------------------------------
        public class nld_QBJT_switch : base_device_t
        {
            //NETLIB_DEVICE_IMPL_NS(analog, QBJT_switch, "QBJT_SW", "MODEL")
            public static readonly factory.constructor_ptr_t decl_QBJT_switch = NETLIB_DEVICE_IMPL_NS<nld_QBJT_switch>("analog", "QBJT_SW", "MODEL");


            param_model_t m_model;
            bjt_model_t m_bjt_model;
            nld_two_terminal m_RB;  //NETLIB_NAME(two_terminal) m_RB;
            nld_two_terminal m_RC;  //NETLIB_NAME(two_terminal) m_RC;
            nld_two_terminal m_BC;  //NETLIB_NAME(two_terminal) m_BC;

            nl_fptype m_gB; // base conductance / switch on
            nl_fptype m_gC; // collector conductance / switch on
            nl_fptype m_V; // internal voltage source
            state_var<unsigned> m_state_on;


            public nld_QBJT_switch(base_device_t_constructor_param_t data)
                : base(data)
            {
                m_model = new param_model_t(this, "MODEL", "NPN");
                m_bjt_model = new bjt_model_t(m_model);
                m_RB = new nld_two_terminal(this, "m_RB", terminal_handler);
                m_RC = new nld_two_terminal(this, "m_RC", terminal_handler);
                m_BC = new nld_two_terminal(this, "m_BC", terminal_handler);
                m_gB = nlconst.cgmin();
                m_gC = nlconst.cgmin();
                m_V = nlconst.zero();
                m_state_on = new state_var<unsigned>(this, "m_state_on", 0U);


                register_sub_alias("B", m_RB.P());
                register_sub_alias("E", m_RB.N());
                register_sub_alias("C", m_RC.P());

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

                var zero = nlconst.zero();

                m_state_on.op = 0;

                m_RB.set_G_V_I(exec().gmin(), zero, zero);
                m_RC.set_G_V_I(exec().gmin(), zero, zero);

                m_BC.set_G_V_I(exec().gmin() / nlconst.magic(10.0), zero, zero);
            }


            //NETLIB_HANDLERI(terminal_handler)
            void terminal_handler()
            {
                var solver = m_RB.solver();
                if (solver != null)
                    solver.solve_now();
                else
                    m_RC.solver().solve_now();
            }


            //NETLIB_IS_DYNAMIC(true)
            public override bool is_dynamic() { return true; }


            //NETLIB_UPDATE_PARAMI();
            //NETLIB_UPDATE_PARAM(QBJT_switch)
            public override void update_param()
            {
                nl_fptype IS = m_bjt_model.m_IS.op();
                nl_fptype BF = m_bjt_model.m_BF.op();
                nl_fptype NF = m_bjt_model.m_NF.op();
                //nl_fptype VJE = m_bjt_model.dValue("VJE", 0.75);

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
                //const nl_fptype m = (m_bjt_model.m_type == bjt_type::BJT_NPN) ? nlconst::one() : -nlconst::one();
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
        public class nld_QBJT_EB : base_device_t
        {
            //NETLIB_DEVICE_IMPL_NS(analog, QBJT_EB, "QBJT_EB", "MODEL")
            public static readonly factory.constructor_ptr_t decl_QBJT_EB = NETLIB_DEVICE_IMPL_NS<nld_QBJT_EB>("analog", "QBJT_EB", "MODEL");


            param_model_t m_model;
            bjt_model_t m_bjt_model;
            generic_diode m_gD_BC;  //generic_diode<diode_e::BIPOLAR> m_gD_BC;
            generic_diode m_gD_BE;  //generic_diode<diode_e::BIPOLAR> m_gD_BE;

            nld_two_terminal m_D_CB;  //NETLIB_NAME(two_terminal) m_D_CB;  // gcc, gce - gcc, gec - gcc, gcc - gce | Ic
            nld_two_terminal m_D_EB;  //NETLIB_NAME(two_terminal) m_D_EB;  // gee, gec - gee, gce - gee, gee - gec | Ie
            nld_two_terminal m_D_EC;  //NETLIB_NAME(two_terminal) m_D_EC;  // 0, -gec, -gcc, 0 | 0

            nl_fptype m_alpha_f;
            nl_fptype m_alpha_r;

            analog.nld_C m_CJE;  //NETLIB_SUB_UPTR(analog, C) m_CJE;
            analog.nld_C m_CJC;  //NETLIB_SUB_UPTR(analog, C) m_CJC;


            public nld_QBJT_EB(base_device_t_constructor_param_t data)
                : base(data)
            {
                m_model = new param_model_t(this, "MODEL", "NPN");
                m_bjt_model = new bjt_model_t(m_model);
                m_gD_BC = new generic_diode(diode_e.BIPOLAR, this, "m_D_BC");
                m_gD_BE = new generic_diode(diode_e.BIPOLAR, this, "m_D_BE");
                m_D_CB = new nld_two_terminal(this, "m_D_CB", terminal_handler);
                m_D_EB = new nld_two_terminal(this, "m_D_EB", terminal_handler);
                m_D_EC = new nld_two_terminal(this, "m_D_EC", terminal_handler);
                m_alpha_f = 0;
                m_alpha_r = 0;


                register_sub_alias("E", m_D_EB.P());   // Cathode
                register_sub_alias("B", m_D_EB.N());   // Anode

                register_sub_alias("C", m_D_CB.P());   // Cathode

                connect(m_D_EB.P(), m_D_EC.P());
                connect(m_D_EB.N(), m_D_CB.N());
                connect(m_D_CB.P(), m_D_EC.N());

                if (m_bjt_model.m_CJE.op() > nlconst.zero())
                {
                    create_and_register_sub_device(this, "m_CJE", out m_CJE);
                    connect("B", "m_CJE.1");
                    connect("E", "m_CJE.2");
                }
                if (m_bjt_model.m_CJC.op() > nlconst.zero())
                {
                    create_and_register_sub_device(this, "m_CJC", out m_CJC);
                    connect("B", "m_CJC.1");
                    connect("C", "m_CJC.2");
                }
            }


            //NETLIB_RESETI();
            public override void reset()
            {
                throw new emu_unimplemented();
            }


            //NETLIB_HANDLERI(terminal_handler)
            void terminal_handler()
            {
                throw new emu_unimplemented();
                //auto *solver(m_D_EB.solver());
                //if (solver != nullptr)
                //    solver->solve_now();
                //else
                //    m_D_CB.solver()->solve_now();
            }


            //NETLIB_IS_DYNAMIC(true)
            public override bool is_dynamic() { return true; }


            //NETLIB_UPDATE_PARAMI();
            public override void update_param()
            {
                throw new emu_unimplemented();
            }


            //NETLIB_UPDATE_TERMINALSI();
            public override void update_terminals()
            {
                throw new emu_unimplemented();
            }
        }
    }
}
