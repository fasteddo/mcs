// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_time = mame.plib.ptime_i64;  //using netlist_time = plib::ptime<std::int64_t, NETLIST_INTERNAL_RES>;
using nl_fptype = System.Double;


// -----------------------------------------------------------------------------
// Implementation
// -----------------------------------------------------------------------------
namespace mame.netlist
{
    namespace analog
    {
        static class nlid_twoterm_global
        {
            //template <class C>
            //inline core_device_t &bselect(bool b, C &d1, core_device_t &d2)
            //{
            //    core_device_t *h = dynamic_cast<core_device_t *>(&d1);
            //    return b ? *h : d2;
            //}
            public static core_device_t bselect(bool b, object d1, core_device_t d2)
            {
                var h = (d1 is core_device_t) ? (core_device_t)d1 : null;
                return b ? h : d2;
            }
            //template<>
            public static core_device_t bselect(bool b, netlist_state_t d1, core_device_t d2)
            {
                //plib::unused_var(d1);
                if (b)
                    throw new nl_exception("bselect with netlist and b==true");
                return d2;
            }
        }


        // -----------------------------------------------------------------------------
        // nld_twoterm
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(twoterm)
        class nld_twoterm : device_t
        {
            terminal_t m_P;
            terminal_t m_N;


            //NETLIB_CONSTRUCTOR_EX(twoterm, bool terminals_owned = false)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_twoterm(object owner, string name, bool terminals_owned = false)
                : base(owner, name)
            {
                m_P = new terminal_t(nlid_twoterm_global.bselect(terminals_owned, owner, this), (terminals_owned ? name + "." : "") + "1", m_N);
                m_N = new terminal_t(nlid_twoterm_global.bselect(terminals_owned, owner, this), (terminals_owned ? name + "." : "") + "2", m_P);
            }


            public terminal_t P { get { return m_P; } }
            public terminal_t N { get { return m_N; } }


            //NETLIB_UPDATE_TERMINALSI() { }
            //NETLIB_RESETI() { }


            //NETLIB_UPDATEI();
            //NETLIB_UPDATE(twoterm)
            protected override void update()
            {
                /* only called if connected to a rail net ==> notify the solver to recalculate */
                solve_now();
            }


            public void solve_now()
            {
                /* we only need to call the non-rail terminal */
                if (m_P.has_net() && !m_P.net().isRailNet())
                    m_P.solve_now();
                else if (m_N.has_net() && !m_N.net().isRailNet())
                    m_N.solve_now();
            }


            public void solve_later(netlist_time delay = null)  //void solve_later(netlist_time delay = netlist_time::quantum()) noexcept;
            {
                // handle default param
                if (delay == null)
                    delay = netlist_time.quantum();

                // we only need to call the non-rail terminal
                if (m_P.has_net() && !m_P.net().isRailNet())
                    m_P.schedule_solve_after(delay);
                else if (m_N.has_net() && !m_N.net().isRailNet())
                    m_N.schedule_solve_after(delay);
            }


            protected void set_G_V_I(nl_fptype G, nl_fptype V, nl_fptype I)
            {
                //               GO, GT,        I
                m_P.set_go_gt_I( -G,  G, (  V) * G - I);
                m_N.set_go_gt_I( -G,  G, ( -V) * G + I);
            }


            protected nl_fptype deltaV()
            {
                return m_P.net().Q_Analog() - m_N.net().Q_Analog();
            }


            //nl_fptype V1P() const noexcept
            //{
            //    return m_P.net().Q_Analog();
            //}

            //nl_fptype V2N() const noexcept
            //{
            //    return m_N.net().Q_Analog();
            //}


            protected void set_mat(nl_fptype a11, nl_fptype a12, nl_fptype rhs1,
                                   nl_fptype a21, nl_fptype a22, nl_fptype rhs2)
            {
                //               GO,  GT,     I
                m_P.set_go_gt_I(a12, a11, rhs1);
                m_N.set_go_gt_I(a21, a22, rhs2);
            }
        }


        // -----------------------------------------------------------------------------
        // nld_R
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT_DERIVED(R_base, twoterm)
        class nld_R_base : nld_twoterm
        {
            //NETLIB_CONSTRUCTOR_DERIVED(R_base, twoterm)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_R_base(object owner, string name)
                : base(owner, name)
            {
            }


            public void set_R(nl_fptype R)
            {
                nl_fptype G = plib.pmath_global.reciprocal(R);
                set_mat( G, -G, nlconst.zero(),
                        -G,  G, nlconst.zero());
            }


            //NETLIB_RESETI();
            //NETLIB_RESET(R_base)
            public override void reset()
            {
                base.reset();  //NETLIB_NAME(twoterm)::reset();
                set_R(plib.pmath_global.reciprocal(exec().gmin()));
            }

            //NETLIB_UPDATEI();
        }


        //NETLIB_OBJECT_DERIVED(R, R_base)
        class nld_R : nld_R_base
        {
            //NETLIB_DEVICE_IMPL_NS(analog, R,    "RES",   "R")
            static factory.element_t nld_R_c(string classname)
            { return new factory.device_element_t<nld_R>("RES", classname, "R", "__FILE__"); }
            public static factory.constructor_ptr_t decl_R = nld_R_c;


            // protect set_R ... it's a recipe to desaster when used to bypass the parameter
            //using NETLIB_NAME(R_base)::set_R;


            param_fp_t m_R;


            //NETLIB_CONSTRUCTOR_DERIVED(R, R_base)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_R(object owner, string name)
                : base(owner, name)
            {
                m_R = new param_fp_t(this, "R", nlconst.magic(1e9));
            }


            //NETLIB_UPDATEI() { }


            //NETLIB_RESETI();
            //NETLIB_RESET(R)
            public override void reset()
            {
                base.reset();  //NETLIB_NAME(twoterm)::reset();
                set_R(std.max(m_R.op(), exec().gmin()));
            }

            //NETLIB_UPDATE_PARAMI();
            //NETLIB_UPDATE_PARAM(R)
            public override void update_param()
            {
                // FIXME: We only need to update the net first if this is a time stepping net
                solve_now();
                set_R(std.max(m_R.op(), exec().gmin()));
                solve_later();
            }

            /* protect set_R ... it's a recipe to desaster when used to bypass the parameter */
            //using NETLIB_NAME(R_base)::set_R;
        }


        // -----------------------------------------------------------------------------
        // nld_POT
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(POT)
        class nld_POT : device_t
        {
            //NETLIB_DEVICE_IMPL_NS(analog, POT,  "POT",   "R")
            static factory.element_t nld_POT_c(string classname)
            { return new factory.device_element_t<nld_POT>("POT", classname, "R", "__FILE__"); }
            public static factory.constructor_ptr_t decl_POT = nld_POT_c;


            nld_R_base m_R1;  //NETLIB_SUB(R_base) m_R1;
            nld_R_base m_R2;  //NETLIB_SUB(R_base) m_R2;

            param_fp_t m_R;
            param_fp_t m_Dial;
            param_logic_t m_DialIsLog;
            param_logic_t m_Reverse;


            //NETLIB_CONSTRUCTOR(POT)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_POT(object owner, string name)
                : base(owner, name)
            {
                m_R1 = new nld_R_base(this, "_R1");
                m_R2 = new nld_R_base(this, "_R2");
                m_R = new param_fp_t(this, "R", 10000);
                m_Dial = new param_fp_t(this, "DIAL", nlconst.half());
                m_DialIsLog = new param_logic_t(this, "DIALLOG", false);
                m_Reverse = new param_logic_t(this, "REVERSE", false);


                register_subalias("1", m_R1.P);
                register_subalias("2", m_R1.N);
                register_subalias("3", m_R2.N);

                connect(m_R2.P, m_R1.N);
            }


            //NETLIB_UPDATEI();

            //NETLIB_RESETI();
            public override void reset()
            {
                nl_fptype v = m_Dial.op();
                if (m_DialIsLog.op())
                    v = (plib.pmath_global.exp(v) - nlconst.one()) / (plib.pmath_global.exp(nlconst.one()) - nlconst.one());

                m_R1.set_R(std.max(m_R.op() * v, exec().gmin()));
                m_R2.set_R(std.max(m_R.op() * (nlconst.one() - v), exec().gmin()));
            }

            //NETLIB_UPDATE_PARAMI();
            //NETLIB_UPDATE_PARAM(POT)
            public override void update_param()
            {
                // FIXME: We only need to update the net first if this is a time stepping net
                m_R1.solve_now();
                m_R2.solve_now();

                nl_fptype v = m_Dial.op();
                if (m_DialIsLog.op())
                    v = (plib.pmath_global.exp(v) - nlconst.one()) / (plib.pmath_global.exp(nlconst.one()) - nlconst.one());
                if (m_Reverse.op())
                    v = nlconst.one() - v;

                m_R1.set_R(std.max(m_R.op() * v, exec().gmin()));
                m_R2.set_R(std.max(m_R.op() * (nlconst.one() - v), exec().gmin()));
                m_R1.solve_later();
                m_R2.solve_later();
            }
        }


        // -----------------------------------------------------------------------------
        // nld_C
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT_DERIVED(C, twoterm)
        class nld_C : nld_twoterm
        {
            //NETLIB_DEVICE_IMPL_NS(analog, C,    "CAP",   "C")
            static factory.element_t nld_C_c(string classname)
            { return new factory.device_element_t<nld_C>("CAP", classname, "C", "__FILE__"); }
            public static factory.constructor_ptr_t decl_C = nld_C_c;


            param_fp_t m_C;

            //generic_capacitor<capacitor_e::VARIABLE_CAPACITY> m_cap;
            generic_capacitor_constant m_cap;  //generic_capacitor<capacitor_e.CONSTANT_CAPACITY> m_cap;


            //NETLIB_CONSTRUCTOR_DERIVED(C, twoterm)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_C(netlist_state_t owner, string name)
                : base(owner, name)
            {
                m_C = new param_fp_t(this, "C", nlconst.magic(1e-6));
                m_cap = new generic_capacitor_constant(this, "m_cap");  //, m_cap(*this, "m_cap")
            }


            //NETLIB_IS_TIMESTEP(true)
            public override bool is_timestep() { return true; }

            //NETLIB_TIMESTEPI();
            public override void timestep(nl_fptype step)
            {
                throw new emu_unimplemented();
            }


            //NETLIB_IS_DYNAMIC(m_cap.type() == capacitor_e::VARIABLE_CAPACITY)
            public override bool is_dynamic() { return m_cap.type() == capacitor_e.VARIABLE_CAPACITY; }


            //NETLIB_UPDATE_TERMINALSI()
            public override void update_terminals()
            {
                nl_fptype I = m_cap.Ieq(m_C.op(), deltaV());
                nl_fptype G = m_cap.G(m_C.op());
                set_mat( G, -G, -I,
                        -G,  G,  I);
            }


            //NETLIB_RESETI();
            public override void reset()
            {
                m_cap.setparams(exec().gmin());
            }


            //NETLIB_UPDATEI();


            //NETLIB_UPDATE_PARAMI();
            //NETLIB_UPDATE_PARAM(C)
            //FIXME: should be able to change
            public override void update_param() { }
        }
    } //namespace analog


    namespace devices
    {
        //NETLIB_DEVICE_IMPL_NS(analog, R,    "RES",   "R")
        //NETLIB_DEVICE_IMPL_NS(analog, POT,  "POT",   "R")
        //NETLIB_DEVICE_IMPL_NS(analog, POT2, "POT2",  "R")
        //NETLIB_DEVICE_IMPL_NS(analog, C,    "CAP",   "C")
        //NETLIB_DEVICE_IMPL_NS(analog, L,    "IND",   "L")
        //NETLIB_DEVICE_IMPL_NS(analog, D,    "DIODE", "MODEL")
        //NETLIB_DEVICE_IMPL_NS(analog, VS,   "VS",    "V")
        //NETLIB_DEVICE_IMPL_NS(analog, CS,   "CS",    "I")
    }
} // namespace netlist
