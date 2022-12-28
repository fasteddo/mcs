// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using base_device_t_constructor_param_t = mame.netlist.core_device_data_t;  //using constructor_param_t = base_device_param_t;  //using base_device_param_t = const base_device_data_t &;  //using base_device_data_t = core_device_data_t;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using param_model_t_value_t = mame.netlist.param_model_t.value_base_t<System.Double, mame.netlist.param_model_t.value_base_t_operators_double>;  //using value_t = value_base_t<nl_fptype>;
using unsigned = System.UInt32;

using static mame.cpp_global;
using static mame.netlist.nl_errstr_global;
using static mame.nl_factory_global;


namespace mame.netlist.analog
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
            // nl_fptype VJE = m_bjt_model.dValue("VJE", 0.75);

            nl_fptype alpha = BF / (nlconst.one() + BF);

#if false
#else
            // diode d(IS, NF);

            // Assume 5mA Collector current for switch operation

            var cc = nlconst.magic(0.005);
            // Get voltage across diode
            // m_V = d.V(cc / alpha);
            m_V = plib.pg.log1p((cc / alpha) / IS) * nlconst.np_VT(NF);

            // Base current is 0.005 / beta
            // as a rough estimate, we just scale the conductance down

            m_gB = plib.pg.reciprocal((m_V / (cc / BF)));

            // m_gB = d.gI(0.005 / alpha);

            if (m_gB < exec().gmin())
                m_gB = exec().gmin();

            // m_gC = d.gI(cc); // very rough estimate
            m_gC = plib.pg.reciprocal(nlconst.np_VT(NF)) * (cc + IS);
#endif
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


    //struct mna2


    struct mna3
    {
        //using row = std::array<nl_fptype, 4>;
        std.array<std.array<nl_fptype, u64_const_4>, u64_const_3> arr;  //std::array<row, 3> arr;

        //const row         &operator[](std::size_t i) const { return arr[i]; }
        public std.array<nl_fptype, u64_const_4> op(int i) { return arr[i]; }
    }


    class nld_three_terminal : base_device_t
    {
        nld_two_terminal m_P0_P2; // gee, gec - gee, gce - gee, gee - gec | Ie
        nld_two_terminal m_P1_P2; // gcc, gce - gcc, gec - gcc, gcc - gce | Ic
        nld_two_terminal m_P0_P1; // 0, -gec, -gcc, 0 | 0


        public nld_three_terminal(base_device_t_constructor_param_t data, std.array<string, u64_const_3> pins)
            : base(data)
        {
            m_P0_P2 = new nld_two_terminal(this, "m_P1_P3", terminal_handler);
            m_P1_P2 = new nld_two_terminal(this, "m_P2_P3", terminal_handler);
            m_P0_P1 = new nld_two_terminal(this, "m_P1_P2", terminal_handler);


            register_sub_alias(pins[0], m_P0_P2.P()); // Emitter - row 1
            register_sub_alias(pins[1], m_P1_P2.P()); // Collector- row 2
            register_sub_alias(pins[2], m_P0_P2.N()); // Base -row 3

            connect(m_P0_P2.P(), m_P0_P1.P());
            connect(m_P0_P2.N(), m_P1_P2.N());
            connect(m_P1_P2.P(), m_P0_P1.N());
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            if (m_P0_P2.solver() == null && m_P1_P2.solver() == null)
                throw new nl_exception(MF_DEVICE_FRY_1(this.name()));
        }


        //NETLIB_HANDLERI(terminal_handler)
        void terminal_handler()
        {
            var solver = m_P0_P2.solver();
            if (solver != null)
                solver.solve_now();
            else
                m_P1_P2.solver().solve_now();
        }


        //template <int PIN1, int PIN2>
        nl_fptype delta_V(int PIN1, int PIN2)
        {
            static_assert(PIN1 >= 0 && PIN2 >= 0 && PIN1 <= 2 && PIN2 <= 2, "out of bounds pin number");
            int sel = PIN1 * 10 + PIN2;

            if (sel == 0)
                return 0.0;
            else if (sel == 1) // P0 P1
                return m_P0_P1.deltaV();
            else if (sel == 2) // P0 P2
                return m_P0_P2.deltaV();
            else if (sel == 10) // P1 P0
                return -m_P0_P1.deltaV();
            else if (sel == 11) // P1 P1
                return 0.0;
            else if (sel == 12) // P1 P2
                return m_P1_P2.deltaV();
            else if (sel == 20) // P2 P0
                return -m_P0_P2.deltaV();
            else if (sel == 21) // P2 P1
                return -m_P1_P2.deltaV();
            else if (sel == 22) // P2 P2
                return 0.0;

            return 0;
        }


        void set_mat_ex(double xee, double xec, double xeb, double xIe,
                        double xce, double xcc, double xcb, double xIc,
                        double xbe, double xbc, double xbb, double xIb)
        {
            //using row2 = std::array<nl_fptype, 3>;

            // rows 0 and 2
            m_P0_P2.set_mat(new std.array<std.array<nl_fptype, u64_const_3>, u64_const_2>(
                new std.array<nl_fptype, u64_const_3>(xee, xeb, xIe),  //row2{xee, xeb, xIe},
                new std.array<nl_fptype, u64_const_3>(xbe, xbb, xIb)  //row2{xbe, xbb, xIb}
            ));
            // rows 1 and 2
            m_P1_P2.set_mat(new std.array<std.array<nl_fptype, u64_const_3>, u64_const_2>(
                new std.array<nl_fptype, u64_const_3>(xcc, xcb, xIc),  //row2{xcc, xcb, xIc},
                new std.array<nl_fptype, u64_const_3>(xbc, 0,   0  )  //row2{xbc, 0,   0  }
            ));
            // rows 0 and 1
            m_P0_P1.set_mat(new std.array<std.array<nl_fptype, u64_const_3>, u64_const_2>(
                new std.array<nl_fptype, u64_const_3>(0,   xec, 0),  //row2{0,   xec, 0},
                new std.array<nl_fptype, u64_const_3>(xce, 0,   0)  //row2{xce, 0,   0}
            ));
        }


        void set_mat_ex(mna3 m)
        {
            //using row2 = std::array<nl_fptype, 3>;

            // rows 0 and 2
            m_P0_P2.set_mat(new std.array<std.array<nl_fptype, u64_const_3>, u64_const_2>(
                new std.array<nl_fptype, u64_const_3>(m.op(0)[0], m.op(0)[2], m.op(0)[3]),  //row2{m[0][0], m[0][2], m[0][3]},
                new std.array<nl_fptype, u64_const_3>(m.op(2)[0], m.op(2)[2], m.op(2)[3])  //row2{m[2][0], m[2][2], m[2][3]}
            ));
            // rows 1 and 2
            m_P1_P2.set_mat(new std.array<std.array<nl_fptype, u64_const_3>, u64_const_2>(
                new std.array<nl_fptype, u64_const_3>(m.op(1)[1], m.op(1)[2], m.op(1)[3]),  //row2{m[1][1], m[1][2], m[1][3]},
                new std.array<nl_fptype, u64_const_3>(m.op(2)[1], 0,       0      )  //row2{m[2][1], 0,       0      }
            ));
            // rows 0 and 1
            m_P0_P1.set_mat(new std.array<std.array<nl_fptype, u64_const_3>, u64_const_2>(
                new std.array<nl_fptype, u64_const_3>(0,       m.op(0)[1], 0),  //row2{0,       m[0][1], 0},
                new std.array<nl_fptype, u64_const_3>(m.op(1)[0], 0,       0)  //row2{m[1][0], 0,       0}
            ));
        }
    }


    // -----------------------------------------------------------------------------
    // nld_QBJT_EB
    // -----------------------------------------------------------------------------
    class nld_QBJT_EB : nld_three_terminal
    {
        //NETLIB_DEVICE_IMPL_NS(analog, QBJT_EB, "QBJT_EB", "MODEL")
        public static readonly factory.constructor_ptr_t decl_QBJT_EB = NETLIB_DEVICE_IMPL_NS<nld_QBJT_EB>("analog", "QBJT_EB", "MODEL");


        //enum pins
        //{
        //    E = 0,
        //    C = 1,
        //    B = 2
        //}


        param_model_t m_model;
        bjt_model_t m_bjt_model;
        generic_diode m_gD_BC;  //generic_diode<diode_e::BIPOLAR> m_gD_BC;
        generic_diode m_gD_BE;  //generic_diode<diode_e::BIPOLAR> m_gD_BE;

        nl_fptype m_alpha_f;
        nl_fptype m_alpha_r;

        analog.nld_C m_CJE;  //NETLIB_SUB_UPTR(analog, C) m_CJE;
        analog.nld_C m_CJC;  //NETLIB_SUB_UPTR(analog, C) m_CJC;


        public nld_QBJT_EB(base_device_t_constructor_param_t data)
            : base(data, new std.array<string, u64_const_3>("E", "C", "B"))
        {
            m_model = new param_model_t(this, "MODEL", "NPN");
            m_bjt_model = new bjt_model_t(m_model);
            m_gD_BC = new generic_diode(diode_e.BIPOLAR, this, "m_D_BC");
            m_gD_BE = new generic_diode(diode_e.BIPOLAR, this, "m_D_BE");
            m_alpha_f = 0;
            m_alpha_r = 0;


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
            base.reset();

            throw new emu_unimplemented();
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
