// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using base_device_t_constructor_data_t = mame.netlist.core_device_data_t;  //using constructor_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;
using base_device_t_constructor_param_t = mame.netlist.core_device_data_t;  //using constructor_param_t = base_device_param_t;  //using base_device_param_t = const base_device_data_t &;  //using base_device_data_t = core_device_data_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using param_fp_t = mame.netlist.param_num_t<System.Double, mame.netlist.param_num_t_operators_double>;  //using param_fp_t = param_num_t<nl_fptype>;
using param_logic_t = mame.netlist.param_num_t<bool, mame.netlist.param_num_t_operators_bool>;  //using param_logic_t = param_num_t<bool>;
using param_model_t_value_t = mame.netlist.param_model_t.value_base_t<System.Double, mame.netlist.param_model_t.value_base_t_operators_double>;  //using value_t = value_base_t<nl_fptype>;

using static mame.nl_factory_global;


// -----------------------------------------------------------------------------
// Implementation
// -----------------------------------------------------------------------------
namespace mame.netlist.analog
{
    // -----------------------------------------------------------------------------
    // nld_two_terminal
    // -----------------------------------------------------------------------------
    public class nld_two_terminal : base_device_t
    {
        public terminal_t m_P;
        public terminal_t m_N;


        public nld_two_terminal(base_device_t_constructor_param_t data)
            : base(data)
        {
            m_P = new terminal_t(this, "1", terminal_handler);//, &m_N, NETLIB_DELEGATE(terminal_handler));
            m_N = new terminal_t(this, "2", terminal_handler);//, &m_P, NETLIB_DELEGATE(terminal_handler));
            m_P.terminal_t_after_ctor(m_N);
            m_N.terminal_t_after_ctor(m_P);
        }


        // This constructor covers the case in which the terminals are "owned"
        // by the device using a two_terminal. In this case it passes
        // the terminal handler on to the terminals.

        public nld_two_terminal(base_device_t owner, string name, nl_delegate owner_delegate)
            : base(new base_device_t_constructor_data_t(owner.state(), owner.name() + "." + name))
        {
            m_P = new terminal_t(owner, name + ".1", owner_delegate);//, &m_N, owner_delegate);
            m_N = new terminal_t(owner, name + ".2", owner_delegate);//, &m_P, owner_delegate);
            m_P.terminal_t_after_ctor(m_N);
            m_N.terminal_t_after_ctor(m_P);
        }


        //// NETLIB_UPDATE_TERMINALSI() { }
        //// NETLIB_RESETI() { }


        //NETLIB_HANDLERI(terminal_handler);
        //NETLIB_HANDLER(twoterm, terminal_handler)
        void terminal_handler()
        {
            // only called if connected to a rail net ==> notify the solver to
            // recalculate
            // printf("%s update\n", this->name().c_str());
            solve_now();
        }


        public solver.matrix_solver_t solver()
        {
            var solv = m_P.solver();
            if (solv != null)
                return solv;
            return m_N.solver();
        }


        public void solve_now()
        {
            var solv = solver();
            if (solv != null)
                solv.solve_now();
        }


        //template <typename F>
        public void change_state(Action f)  //void change_state(F f) const
        {
            var solv = solver();
            if (solv != null)
                solv.change_state(f);
        }


        public void set_G_V_I(nl_fptype G, nl_fptype V, nl_fptype I)
        {
            //               GO, GT,        I
            m_P.set_go_gt_I( -G,  G, (  V) * G - I);
            m_N.set_go_gt_I( -G,  G, ( -V) * G + I);
        }


        protected nl_fptype deltaV()
        {
            return m_P.net().Q_Analog() - m_N.net().Q_Analog();
        }


        //nl_fptype V1P() const noexcept { return m_P.net().Q_Analog(); }
        //nl_fptype V2N() const noexcept { return m_N.net().Q_Analog(); }


        protected void set_mat(nl_fptype a11, nl_fptype a12, nl_fptype rhs1,
                               nl_fptype a21, nl_fptype a22, nl_fptype rhs2)
        {
            //               GO,  GT,     I
            m_P.set_go_gt_I(a12, a11, rhs1);
            m_N.set_go_gt_I(a21, a22, rhs2);
        }


        protected void clear_mat()
        {
            var z = nlconst.zero();
            //               GO,  GT,     I
            m_P.set_go_gt_I(z, z, z);
            m_N.set_go_gt_I(z, z, z);
        }


        /// \brief Get a const reference to the m_P terminal
        ///
        /// This is typically called during initialization to connect
        /// terminals.
        ///
        /// \returns Reference to m_P terminal.
        public terminal_t P() { return m_P; }

        /// \brief Get a const reference to the m_N terminal
        ///
        /// This is typically called during initialization to connect
        /// terminals.
        ///
        /// \returns Reference to m_N terminal.
        public terminal_t N() { return m_N; }

        /// \brief Get a reference to the m_P terminal
        ///
        /// This call is only allowed from the core. Device code should never
        /// need to call this.
        ///
        /// \returns Reference to m_P terminal.
        public terminal_t setup_P() { return m_P; }

        /// \brief Get a reference to the m_N terminal
        ///
        /// This call is only allowed from the core. Device code should never
        /// need to call this.
        ///
        /// \returns Reference to m_P terminal.
        public terminal_t setup_N() { return m_N; }
    }


    // -----------------------------------------------------------------------------
    // nld_R
    // -----------------------------------------------------------------------------
    class nld_R_base : nld_two_terminal
    {
        public nld_R_base(base_device_t_constructor_param_t data)
            : base(data)
        {
        }


        public void set_R(nl_fptype R)
        {
            nl_fptype G = plib.pg.reciprocal(R);
            set_mat( G, -G, nlconst.zero(),
                    -G,  G, nlconst.zero());
        }


        //void set_G(nl_fptype G) const noexcept
        //{
        //    set_mat( G, -G, nlconst::zero(),
        //            -G,  G, nlconst::zero());
        //}


        // NETLIB_RESETI();
        // NETLIB_UPDATEI();
    }


    class nld_R : nld_R_base
    {
        //NETLIB_DEVICE_IMPL_NS(analog, R,    "RES",   "R")
        public static readonly factory.constructor_ptr_t decl_R = NETLIB_DEVICE_IMPL_NS<nld_R>("analog", "RES", "R");


        // protect set_R ... it's a recipe to disaster when used to bypass the
        // parameter
        //using nld_R_base::set_G;
        //using nld_R_base::set_R;


        param_fp_t m_R;


        public nld_R(base_device_t_constructor_param_t data)
            : base(data)
        {
            m_R = new param_fp_t(this, "R", nlconst.magic(1e9));
        }


        //NETLIB_UPDATEI() { }


        public override void reset() { set_R(std.max(m_R.op(), exec().gmin())); }  //NETLIB_RESETI() { set_R(std::max(m_R(), exec().gmin())); }


        //NETLIB_UPDATE_PARAMI();
        public override void update_param()
        {
            // FIXME: We only need to update the net first if this is a time
            // stepping net
            change_state(() => { set_R(std.max(m_R.op(), exec().gmin())); });
        }
    }


    // -----------------------------------------------------------------------------
    // nld_POT
    // -----------------------------------------------------------------------------
    class nld_POT : base_device_t
    {
        //NETLIB_DEVICE_IMPL_NS(analog, POT,  "POT",   "R")
        public static readonly factory.constructor_ptr_t decl_POT = NETLIB_DEVICE_IMPL_NS<nld_POT>("analog", "POT", "R");


        sub_device_wrapper<analog.nld_R_base> m_R1;  //NETLIB_SUB_NS(analog, R_base) m_R1;
        sub_device_wrapper<analog.nld_R_base> m_R2;  //NETLIB_SUB_NS(analog, R_base) m_R2;

        param_fp_t m_R;
        param_fp_t m_Dial;
        param_logic_t m_DialIsLog;
        param_logic_t m_Reverse;


        public nld_POT(base_device_t_constructor_param_t data)
            : base(data)
        {
            m_R1 = new sub_device_wrapper<nld_R_base>(this, new nld_R_base(new base_device_t_constructor_data_t(this, "_R1")));
            m_R2 = new sub_device_wrapper<nld_R_base>(this, new nld_R_base(new base_device_t_constructor_data_t(this, "_R2")));
            m_R = new param_fp_t(this, "R", 10000);
            m_Dial = new param_fp_t(this, "DIAL", nlconst.half());
            m_DialIsLog = new param_logic_t(this, "DIALLOG", false);
            m_Reverse = new param_logic_t(this, "REVERSE", false);


            register_sub_alias("1", m_R1.op().P());
            register_sub_alias("2", m_R1.op().N());
            register_sub_alias("3", m_R2.op().N());

            connect(m_R2.op().P(), m_R1.op().N());
        }


        //// NETLIB_UPDATEI();


        //NETLIB_RESETI();
        public override void reset()
        {
            nl_fptype v = m_Dial.op();
            if (m_DialIsLog.op())
                v = (plib.pg.exp(v) - nlconst.one()) / (plib.pg.exp(nlconst.one()) - nlconst.one());

            m_R1.op().set_R(std.max(m_R.op() * v, exec().gmin()));
            m_R2.op().set_R(std.max(m_R.op() * (nlconst.one() - v), exec().gmin()));
        }

        //NETLIB_UPDATE_PARAMI();
        //NETLIB_UPDATE_PARAM(POT)
        public override void update_param()
        {
            nl_fptype v = m_Dial.op();
            if (m_DialIsLog.op())
                v = (plib.pg.exp(v) - nlconst.one()) / (plib.pg.exp(nlconst.one()) - nlconst.one());
            if (m_Reverse.op())
                v = nlconst.one() - v;

            nl_fptype r1 = std.max(m_R.op() * v, exec().gmin());
            nl_fptype r2 = std.max(m_R.op() * (nlconst.one() - v), exec().gmin());

            if (m_R1.op().solver() == m_R2.op().solver())
            { 
                m_R1.op().change_state(() => { m_R1.op().set_R(r1); m_R2.op().set_R(r2); });  //m_R1.change_state([this, &r1, &r2]() { m_R1.set_R(r1); m_R2.set_R(r2); });
            }
            else
            {
                m_R1.op().change_state(() => { m_R1.op().set_R(r1); });  //m_R1.change_state([this, &r1]() { m_R1.set_R(r1); });
                m_R2.op().change_state(() => { m_R2.op().set_R(r2); });  //m_R2.change_state([this, &r2]() { m_R2.set_R(r2); });
            }
        }
    }


    //class nld_POT2 : public base_device_t


    // -----------------------------------------------------------------------------
    // nld_C
    // -----------------------------------------------------------------------------
    public class nld_C : nld_two_terminal
    {
        //NETLIB_DEVICE_IMPL_NS(analog, C,    "CAP",   "C")
        public static readonly factory.constructor_ptr_t decl_C = NETLIB_DEVICE_IMPL_NS<nld_C>("analog", "CAP", "C");


        param_fp_t m_C;
        generic_capacitor_const m_cap;


        public nld_C(base_device_t_constructor_param_t data)
            : base(data)
        {
            m_C = new param_fp_t(this, "C", nlconst.magic(1e-6));
            m_cap = new generic_capacitor_const(this, "m_cap");  //, m_cap(*this, "m_cap")
        }


        //NETLIB_IS_TIMESTEP(true)
        public override bool is_time_step() { return true; }


        //NETLIB_TIMESTEPI()
        public override void time_step(time_step_type ts_type, nl_fptype step)
        {
            if (ts_type == time_step_type.FORWARD)
            {
                // G, Ieq
                var res = m_cap.time_step(m_C.op(), deltaV(), step);
                nl_fptype G = res.first;
                nl_fptype I = res.second;
                set_mat( G, -G, -I,
                        -G,  G,  I);
            }
            else
            {
                m_cap.restore_state();
            }
        }


        //NETLIB_RESETI()
        public override void reset() { m_cap.set_parameters(exec().gmin()); }  //NETLIB_RESETI() { m_cap.set_parameters(exec().gmin()); }


        /// \brief Set capacitance
        ///
        /// This call will set the capacitance. The typical use case are
        /// are components like BJTs which use this component to model
        /// internal capacitances. Typically called during initialization.
        ///
        /// \param val Capacitance value
        ///
        public void set_cap_embedded(nl_fptype val) { m_C.set(val); }


        // NETLIB_UPDATEI();

        // FIXME: should be able to change
        //NETLIB_UPDATE_PARAMI() { }
        public override void update_param() { }
    }


    //class nld_L : public nld_two_terminal


    class diode_model_t
    {
        public param_model_t_value_t m_IS;    //!< saturation current.
        public param_model_t_value_t m_N;     //!< emission coefficient.


        public diode_model_t(param_model_t model)
        {
            m_IS = new param_model_t_value_t(model, "IS");
            m_N = new param_model_t_value_t(model, "N");
        }
    }


    class zdiode_model_t : diode_model_t
    {
        param_model_t_value_t m_NBV;    //!< reverse emission coefficient.
        param_model_t_value_t m_BV;     //!< reverse breakdown voltage.
        param_model_t_value_t m_IBV;    //!< current at breakdown voltage.


        public zdiode_model_t(param_model_t model)
            : base(model)
        {
            m_NBV = new param_model_t_value_t(model, "NBV");
            m_BV = new param_model_t_value_t(model, "BV");
            m_IBV = new param_model_t_value_t(model, "IBV");
        }
    }


    public class nld_D : nld_two_terminal
    {
        //NETLIB_DEVICE_IMPL_NS(analog, D,    "DIODE", "MODEL")
        public static readonly factory.constructor_ptr_t decl_D = NETLIB_DEVICE_IMPL_NS<nld_D>("analog", "DIODE", "MODEL");


        param_model_t m_model;
        diode_model_t m_modacc;
        generic_diode m_D;  //generic_diode<diode_e::BIPOLAR> m_D;


        public nld_D(base_device_t_constructor_param_t data, string model = "D")
            : base(data)
        {
            m_model = new param_model_t(this, "MODEL", model);
            m_modacc = new diode_model_t(m_model);
            m_D = new generic_diode(diode_e.BIPOLAR, this, "m_D");


            register_sub_alias("A", P());
            register_sub_alias("K", N());
        }


        //NETLIB_IS_DYNAMIC(true)
        public override bool is_dynamic() { return true; }


        //NETLIB_UPDATE_TERMINALSI();
        //NETLIB_UPDATE_TERMINALS(D)
        public override void update_terminals()
        {
            m_D.update_diode(deltaV());
            nl_fptype G = m_D.G();
            nl_fptype I = m_D.Ieq();
            set_mat( G, -G, -I,
                    -G,  G,  I);
            //set(m_D.G(), 0.0, m_D.Ieq());
        }


        //NETLIB_RESETI();
        //NETLIB_RESET(D)
        public override void reset()
        {
            nl_fptype Is = m_modacc.m_IS.op();
            nl_fptype n = m_modacc.m_N.op();

            m_D.set_param(Is, n, exec().gmin(), nlconst.T0());
            set_G_V_I(m_D.G(), nlconst.zero(), m_D.Ieq());
        }


        // NETLIB_UPDATEI();


        //NETLIB_UPDATE_PARAMI();
        //NETLIB_UPDATE_PARAM(D)
        public override void update_param()
        {
            nl_fptype Is = m_modacc.m_IS.op();
            nl_fptype n = m_modacc.m_N.op();

            m_D.set_param(Is, n, exec().gmin(), nlconst.T0());
        }
    }


    // -----------------------------------------------------------------------------
    // nld_Z - Zener Diode
    // -----------------------------------------------------------------------------
    public class nld_Z : nld_two_terminal
    {
        //NETLIB_DEVICE_IMPL_NS(analog, Z,    "ZDIODE", "MODEL")
        public static readonly factory.constructor_ptr_t decl_Z = NETLIB_DEVICE_IMPL_NS<nld_Z>("analog", "ZDIODE", "MODEL");


        param_model_t m_model;
        zdiode_model_t m_modacc;
        generic_diode m_D;  //generic_diode<diode_e::BIPOLAR> m_D;
        // REVERSE diode
        generic_diode m_R;  //generic_diode<diode_e::BIPOLAR> m_R;


        public nld_Z(base_device_t_constructor_param_t data, string model = "D")
            : base(data)
        {
            m_model = new param_model_t(this, "MODEL", model);
            m_modacc = new zdiode_model_t(m_model);
            m_D = new generic_diode(diode_e.BIPOLAR, this, "m_D");
            m_R = new generic_diode(diode_e.BIPOLAR, this, "m_R");


            register_sub_alias("A", P());
            register_sub_alias("K", N());
        }


        //NETLIB_IS_DYNAMIC(true)
        public override bool is_dynamic() { return true; }


        //NETLIB_UPDATE_TERMINALSI();
        public override void update_terminals()
        {
            throw new emu_unimplemented();
        }


        //NETLIB_RESETI();
        public override void reset()
        {
            throw new emu_unimplemented();
        }


        // NETLIB_UPDATEI();


        //NETLIB_UPDATE_PARAMI();
        public override void update_param()
        {
            throw new emu_unimplemented();
        }
    }


    // -----------------------------------------------------------------------------
    // nld_VS - Voltage source
    //
    // netlist voltage source must have inner resistance
    // -----------------------------------------------------------------------------
    //class nld_VS : public nld_two_terminal


    // -----------------------------------------------------------------------------
    // nld_CS - Current source
    // -----------------------------------------------------------------------------
    //class nld_CS : public nld_two_terminal
}

namespace mame.netlist.analog
{
    //NETLIB_DEVICE_IMPL_NS(analog, R,    "RES",   "R")
    //NETLIB_DEVICE_IMPL_NS(analog, POT,  "POT",   "R")
    //NETLIB_DEVICE_IMPL_NS(analog, POT2, "POT2",  "R")
    //NETLIB_DEVICE_IMPL_NS(analog, C,    "CAP",   "C")
    //NETLIB_DEVICE_IMPL_NS(analog, L,    "IND",   "L")
    //NETLIB_DEVICE_IMPL_NS(analog, D,    "DIODE", "MODEL")
    //NETLIB_DEVICE_IMPL_NS(analog, Z,    "ZDIODE", "MODEL")
    //NETLIB_DEVICE_IMPL_NS(analog, VS,   "VS",    "V")
    //NETLIB_DEVICE_IMPL_NS(analog, CS,   "CS",    "I")
}
