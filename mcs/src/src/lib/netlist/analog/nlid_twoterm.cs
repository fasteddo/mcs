// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_base_t = mame.netlist.netlist_state_t;
using nl_double = System.Double;


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
                core_device_t h = (d1 is core_device_t) ? (core_device_t)d1 : null;
                return b ? h : d2;
            }
            //template<>
            public static core_device_t bselect(bool b, netlist_base_t d1, core_device_t d2)
            {
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
                m_P = new terminal_t(nlid_twoterm_global.bselect(terminals_owned, owner, this), (terminals_owned ? name + "." : "") + "1");
                m_N = new terminal_t(nlid_twoterm_global.bselect(terminals_owned, owner, this), (terminals_owned ? name + "." : "") + "2");


                m_P.otherterm = m_N;
                m_N.otherterm = m_P;
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
                /* we only need to call the non-rail terminal */
                if (m_P.has_net() && !m_P.net().isRailNet())
                    m_P.solve_now();
                else if (m_N.has_net() && !m_N.net().isRailNet())
                    m_N.solve_now();
            }


            protected void set(nl_double G, nl_double V, nl_double I)
            {
                /*      GO, GT, I                */
                m_P.set( G,  G, (  V) * G - I);
                m_N.set( G,  G, ( -V) * G + I);
            }


            ///* inline */ nl_double deltaV() const
            //{
            //    return m_P.net().Q_Analog() - m_N.net().Q_Analog();
            //}


            protected void set_mat(nl_double a11, nl_double a12, nl_double r1,
                                   nl_double a21, nl_double a22, nl_double r2)
            {
                /*      GO, GT, I                */
                m_P.set(-a12, a11, r1);
                m_N.set(-a21, a22, r2);
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


            public void set_R(nl_double R)
            {
                nl_double G = nl_config_global.NL_FCONST(1.0) / R;
                set_mat( G, -G, 0.0,
                        -G,  G, 0.0);
            }


            //NETLIB_RESETI();
            //NETLIB_RESET(R_base)
            protected override void reset()
            {
                base.reset();  //NETLIB_NAME(twoterm)::reset();
                set_R(1.0 / exec().gmin());
            }

            //NETLIB_UPDATEI();
            //NETLIB_UPDATE(R_base)
            protected override void update()
            {
                base.update();  //NETLIB_NAME(twoterm)::update();
            }
        }


        //NETLIB_OBJECT_DERIVED(R, R_base)
        class nld_R : nld_R_base
        {
            //NETLIB_DEVICE_IMPL_NS(analog, R)
            static factory.element_t nld_R_c(string name, string classname, string def_param)
            { return new factory.device_element_t<nld_R>(name, classname, def_param, "__FILE__"); }
            public static factory.constructor_ptr_t decl_R = nld_R_c;


            param_double_t m_R;


            //NETLIB_CONSTRUCTOR_DERIVED(R, R_base)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_R(object owner, string name)
                : base(owner, name)
            {
                m_R = new param_double_t(this, "R", 1e9);
            }


            //NETLIB_RESETI();
            //NETLIB_RESET(R)
            protected override void reset()
            {
                base.reset();  //NETLIB_NAME(twoterm)::reset();
                set_R(Math.Max(m_R.op(), exec().gmin()));
            }

            //NETLIB_UPDATEI() { }

            //NETLIB_UPDATE_PARAMI();
            //NETLIB_UPDATE_PARAM(R)
            public override void update_param()
            {
                update_dev();
                set_R(std.max(m_R.op(), exec().gmin()));
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
            //NETLIB_DEVICE_IMPL_NS(analog, POT)
            static factory.element_t nld_POT_c(string name, string classname, string def_param)
            { return new factory.device_element_t<nld_POT>(name, classname, def_param, "__FILE__"); }
            public static factory.constructor_ptr_t decl_POT = nld_POT_c;


            nld_R_base m_R1;  //NETLIB_SUB(R_base) m_R1;
            nld_R_base m_R2;  //NETLIB_SUB(R_base) m_R2;

            param_double_t m_R;
            param_double_t m_Dial;
            param_logic_t m_DialIsLog;


            //NETLIB_CONSTRUCTOR(POT)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_POT(object owner, string name)
                : base(owner, name)
            {
                m_R1 = new nld_R_base(this, "_R1");
                m_R2 = new nld_R_base(this, "_R2");
                m_R = new param_double_t(this, "R", 10000);
                m_Dial = new param_double_t(this, "DIAL", 0.5);
                m_DialIsLog = new param_logic_t(this, "DIALLOG", false /*0*/);


                register_subalias("1", m_R1.P);
                register_subalias("2", m_R1.N);
                register_subalias("3", m_R2.N);

                connect(m_R2.P, m_R1.N);
            }


            //NETLIB_UPDATEI();

            //NETLIB_RESETI();
            protected override void reset()
            {
                nl_double v = m_Dial.op();
                if (m_DialIsLog.op())
                    v = (std.exp(v) - 1.0) / (std.exp(1.0) - 1.0);

                m_R1.set_R(std.max(m_R.op() * v, exec().gmin()));
                m_R2.set_R(std.max(m_R.op() * (nl_config_global.NL_FCONST(1.0) - v), exec().gmin()));
            }

            //NETLIB_UPDATE_PARAMI();
            //NETLIB_UPDATE_PARAM(POT)
            public override void update_param()
            {
                m_R1.update_dev();
                m_R2.update_dev();

                nl_double v = m_Dial.op();
                if (m_DialIsLog.op())
                    v = (std.exp(v) - 1.0) / (std.exp(1.0) - 1.0);

                m_R1.set_R(std.max(m_R.op() * v, exec().gmin()));
                m_R2.set_R(std.max(m_R.op() * (nl_config_global.NL_FCONST(1.0) - v), exec().gmin()));
            }
        }


        // -----------------------------------------------------------------------------
        // nld_C
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT_DERIVED(C, twoterm)
        class nld_C : nld_twoterm
        {
            //NETLIB_DEVICE_IMPL_NS(analog, C)
            static factory.element_t nld_C_c(string name, string classname, string def_param)
            { return new factory.device_element_t<nld_C>(name, classname, def_param, "__FILE__"); }
            public static factory.constructor_ptr_t decl_C = nld_C_c;


            param_double_t m_C;

            nl_double m_GParallel;


            //NETLIB_CONSTRUCTOR_DERIVED(C, twoterm)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_C(netlist_state_t owner, string name)
                : base(owner, name)
            {
                m_C = new param_double_t(this, "C", 1e-6);
                m_GParallel = 0.0;


                //register_term("1", m_P);
                //register_term("2", m_N);
            }


            //NETLIB_IS_TIMESTEP(true)
            public override bool is_timestep() { return true; }

            //NETLIB_TIMESTEPI();
            public override void timestep(nl_double step)
            {
                throw new emu_unimplemented();
            }


            //NETLIB_RESETI();
            protected override void reset()
            {
                // FIXME: Startup conditions
                set(exec().gmin(), 0.0, -5.0 / exec().gmin());
                //set(exec().gmin(), 0.0, 0.0);
            }

            //NETLIB_UPDATEI();
            //NETLIB_UPDATE(C)
            protected override void update()
            {
                base.update();  //NETLIB_NAME(twoterm)::update();
            }

            //NETLIB_UPDATE_PARAMI();
            //NETLIB_UPDATE_PARAM(C)
            public override void update_param()
            {
                m_GParallel = exec().gmin();
            }
        }
    } //namespace analog


    namespace devices
    {
        //NETLIB_DEVICE_IMPL_NS(analog, R)
        //NETLIB_DEVICE_IMPL_NS(analog, POT)
        //NETLIB_DEVICE_IMPL_NS(analog, POT2)
        //NETLIB_DEVICE_IMPL_NS(analog, C)
        //NETLIB_DEVICE_IMPL_NS(analog, L)
        //NETLIB_DEVICE_IMPL_NS(analog, D)
        //NETLIB_DEVICE_IMPL_NS(analog, VS)
        //NETLIB_DEVICE_IMPL_NS(analog, CS)
    }

} // namespace netlist
