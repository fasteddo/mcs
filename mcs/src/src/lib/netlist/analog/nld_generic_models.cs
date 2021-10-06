// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;


namespace mame.netlist.analog
{
    // -----------------------------------------------------------------------------
    // A generic capacitor model
    // -----------------------------------------------------------------------------

    enum capacitor_e
    {
        VARIABLE_CAPACITY,
        CONSTANT_CAPACITY
    }


    //template <capacitor_e TYPE>
    //class generic_capacitor

    //template <>
    class generic_capacitor_variable  //<capacitor_e::VARIABLE_CAPACITY>
    {
        state_var<nl_fptype> m_h;
        state_var<nl_fptype> m_c;
        state_var<nl_fptype> m_v;
        nl_fptype m_gmin;


        generic_capacitor_variable(core_device_t dev, string name)
        {
            m_h = new state_var<nl_fptype>(dev, name + ".m_h", nlconst.zero());
            m_c = new state_var<nl_fptype>(dev, name + ".m_c", nlconst.zero());
            m_v = new state_var<nl_fptype>(dev, name + ".m_v", nlconst.zero());
            m_gmin = nlconst.zero();
        }

        capacitor_e type() { return capacitor_e.VARIABLE_CAPACITY; }

        // Circuit Simulation, page 284, 5.360
        // q(un+1) - q(un) = int(un, un+1, C(U)) = (C0+C1)/2 * (un+1-un)
        // The direct application of formulas 5.359 and 5.360 has
        // issues with pulses. Therefore G and Ieq are expressed differently
        // so that G depends on un+1 only and Ieq on un only.
        // In both cases, i = G * un+1 + Ieq

        //nl_fptype G(nl_fptype cap) const noexcept

        //nl_fptype Ieq(nl_fptype cap, nl_fptype v) const noexcept

        //void timestep(nl_fptype cap, nl_fptype v, nl_fptype step) noexcept

        //void restore_state() noexcept

        //void setparams(nl_fptype gmin) noexcept { m_gmin = gmin; }
    }


    // "Circuit simulation", page 274
    //template <>
    class generic_capacitor_constant  //<capacitor_e::CONSTANT_CAPACITY>
    {
        state_var<nl_fptype> m_h;
        state_var<nl_fptype> m_v;
        nl_fptype m_gmin;


        public generic_capacitor_constant(core_device_t dev, string name)
        {
            m_h = new state_var<nl_fptype>(dev, name + ".m_h", nlconst.zero());
            m_v = new state_var<nl_fptype>(dev, name + ".m_v", nlconst.zero());
            m_gmin = nlconst.zero();
        }


        public capacitor_e type() { return capacitor_e.CONSTANT_CAPACITY; }

        public nl_fptype G(nl_fptype cap) { return cap * m_h.op +  m_gmin; }

        public nl_fptype Ieq(nl_fptype cap, nl_fptype v)
        {
            //plib::unused_var(v);
            return - G(cap) * m_v.op;
        }

        public void timestep(nl_fptype cap, nl_fptype v, nl_fptype step)
        {
            //plib::unused_var(cap);
            m_h.op = plib.pg.reciprocal(step);
            m_v.op = v;
        }

        public void setparams(nl_fptype gmin) { m_gmin = gmin; }
    }


//#if (NL_USE_BACKWARD_EULER)
#if true
    // Constant model for constant capacitor model
    // Backward Euler
    // "Circuit simulation", page 274
    struct generic_capacitor_const
    {
        nl_fptype m_gmin;


        public generic_capacitor_const(core_device_t dev, string name)
        {
            m_gmin = nlconst.zero();

            //plib::unused_var(dev, name);
        }

        // Returns { G, Ieq }
        public std.pair<nl_fptype, nl_fptype> timestep(nl_fptype cap, nl_fptype v, nl_fptype step)
        {
            nl_fptype h = plib.pg.reciprocal(step);
            nl_fptype G = cap * h + m_gmin;
            return new std.pair<nl_fptype, nl_fptype>(G, - G * v);
        }

        public void restore_state()
        {
            // this one has no state
        }

        public void setparams(nl_fptype gmin) { m_gmin = gmin; }
    }
#endif


    // -----------------------------------------------------------------------------
    // A generic diode model to be used in other devices (Diode, BJT ...)
    // -----------------------------------------------------------------------------
    enum diode_e
    {
        BIPOLAR,
        MOS
    }

    //template <diode_e TYPE>
    class generic_diode
    {
        diode_e TYPE;

        //string m_name;

        // owning object must save those ...
        state_var<nl_fptype> m_Vd;
        state_var<nl_fptype> m_Id;
        state_var<nl_fptype> m_G;

        nl_fptype m_Vt;
        nl_fptype m_Vmin;
        nl_fptype m_Is;
        nl_fptype m_logIs;
        nl_fptype m_gmin;

        nl_fptype m_VtInv;
        nl_fptype m_Vcrit;
#if !USE_TEXTBOOK_DIODE
        //nl_fptype m_Imin;
        nl_fptype m_Icrit_p_Is;
#endif


        public generic_diode(diode_e TYPE_, core_device_t dev, string name)
        {
            TYPE = TYPE_;

            m_Vd = new state_var<nl_fptype>(dev, name + ".m_Vd", nlconst.diode_start_voltage());
            m_Id = new state_var<nl_fptype>(dev, name + ".m_Id", nlconst.zero());
            m_G = new state_var<nl_fptype>(dev,  name + ".m_G", nlconst.cgminalt());
            m_Vt = nlconst.zero();
            m_Vmin = nlconst.zero(); // not used in MOS model
            m_Is = nlconst.zero();
            m_logIs = nlconst.zero();
            m_gmin = nlconst.cgminalt();
            m_VtInv = nlconst.zero();
            m_Vcrit = nlconst.zero();


            set_param(
                nlconst.np_Is()
              , nlconst.one()
              , nlconst.cgminalt()
              , nlconst.T0());
            //m_name = name;
        }


        // Basic math
        //
        // I(V) = f(V)
        //
        // G(V) = df/dV(V)
        //
        // Ieq(V) = I(V) - V * G(V)
        //
        //
        public void update_diode(nl_fptype nVd)
        {
            if (TYPE == diode_e.BIPOLAR)
            {
#if USE_TEXTBOOK_DIODE
                if (nVd > m_Vcrit)
                {
                    // if the old voltage is less than zero and new is above
                    // make sure we move enough so that matrix and current
                    // changes.
                    const nl_fptype old = std::max(nlconst::zero(), m_Vd());
                    const nl_fptype d = std::min(+fp_constants<nl_fptype>::DIODE_MAXDIFF(), nVd - old);
                    const nl_fptype a = plib::abs(d) * m_VtInv;
                    m_Vd = old + plib::signum(d) * plib::log1p(a) * m_Vt;
                }
                else
                    m_Vd = std::max(-fp_constants<nl_fptype>::DIODE_MAXDIFF(), nVd);

                if (m_Vd < m_Vmin)
                {
                    m_G = m_gmin;
                    m_Id = - m_Is;
                }
                else
                {
                    const auto IseVDVt = plib::exp(m_logIs + m_Vd * m_VtInv);
                    m_Id = IseVDVt - m_Is;
                    m_G = IseVDVt * m_VtInv + m_gmin;
                }
#else
                //printf("%s: %g %g\n", m_name.c_str(), nVd, (nl_fptype) m_Vd);
                m_Vd.op = nVd;
                if (nVd > m_Vcrit)
                {
                    m_Id.op = m_Icrit_p_Is - m_Is + (m_Vd.op - m_Vcrit) * m_Icrit_p_Is * m_VtInv;
                    m_G.op = m_Icrit_p_Is * m_VtInv + m_gmin;
                }
                else if (m_Vd.op < m_Vmin)
                {
                    m_G.op = m_gmin;
                    //m_Id = m_Imin + (m_Vd - m_Vmin) * m_gmin;
                    //m_Imin = m_gmin * m_Vt - m_Is;
                    m_Id.op = (m_Vd.op - m_Vmin + m_Vt) * m_gmin - m_Is;
                }
                else
                {
                    var IseVDVt = plib.pg.exp(m_logIs + m_Vd.op * m_VtInv);
                    m_Id.op = IseVDVt - m_Is;
                    m_G.op = IseVDVt * m_VtInv + m_gmin;
                }
#endif
            }
            else if (TYPE == diode_e.MOS)
            {
                m_Vd.op = nVd;
                if (nVd < nlconst.zero())
                {
                    m_G.op = m_Is * m_VtInv + m_gmin;
                    m_Id.op = m_G.op * m_Vd.op;
                }
                else // log stepping should already be done in mosfet
                {
                    var IseVDVt = plib.pg.exp(std.min(+fp_constants_double.DIODE_MAXVOLT(), m_logIs + m_Vd.op * m_VtInv));
                    m_Id.op = IseVDVt - m_Is;
                    m_G.op = IseVDVt * m_VtInv + m_gmin;
                }
            }
        }


        public void set_param(nl_fptype Is, nl_fptype n, nl_fptype gmin, nl_fptype temp)
        {
            m_Is = Is;
            m_logIs = plib.pg.log(Is);
            m_gmin = gmin;

            m_Vt = nlconst.np_VT(n, temp);
            m_VtInv = plib.pg.reciprocal(m_Vt);

#if USE_TEXTBOOK_DIODE
            m_Vmin = nlconst::diode_min_cutoff_mult() * m_Vt;
            // Vcrit : f(V) has smallest radius of curvature rho(V) == min(rho(v))
            m_Vcrit = m_Vt * plib::log(m_Vt / m_Is / nlconst::sqrt2());
#else
            m_Vmin = plib.pg.log(m_gmin * m_Vt / m_Is) * m_Vt;
            //m_Imin = plib::exp(m_logIs + m_Vmin * m_VtInv) - m_Is;
            //m_Imin = m_gmin * m_Vt - m_Is;
            // Fixme: calculate max dissipation voltage - use use 0.5 (500mW) here for typical diode
            // P = V * I = V * (Is*exp(V/Vt) - Is)
            // P ~= V * I = V * Is*exp(V/Vt)
            // ln(P/Is) = ln(V)+V/Vt ~= V - 1 + V/vt
            // V = (1+ln(P/Is))/(1 + 1/Vt)

            m_Vcrit = (nlconst.one() + plib.pg.log(nlconst.half() / m_Is)) / (nlconst.one() + m_VtInv);
            //printf("Vcrit: %f\n", m_Vcrit);
            m_Icrit_p_Is = plib.pg.exp(m_logIs + m_Vcrit * m_VtInv);
            //m_Icrit = plib::exp(m_logIs + m_Vcrit * m_VtInv) - m_Is;
#endif

        }


        nl_fptype I() { return m_Id.op; }
        public nl_fptype G() { return m_G.op; }
        public nl_fptype Ieq() { return m_Id.op - m_Vd.op * m_G.op; }
        nl_fptype Vd() { return m_Vd.op; }
    }
}
