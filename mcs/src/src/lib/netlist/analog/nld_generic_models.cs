// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;


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
    //{
    //};


    //template <>
    class generic_capacitor_variable  //<capacitor_e::VARIABLE_CAPACITY>
    {
        state_var<nl_fptype> m_h;
        state_var<nl_fptype> m_c;
        state_var<nl_fptype> m_v;
        nl_fptype m_gmin;


        generic_capacitor_variable(device_t dev, string name)
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
        //{
        //    //return m_h * cap +  m_gmin;
        //    return m_h * nlconst::half() * (cap + m_c) +  m_gmin;
        //    //return m_h * cap +  m_gmin;
        //}

        //nl_fptype Ieq(nl_fptype cap, nl_fptype v) const noexcept
        //{
        //    plib::unused_var(v);
        //    //return -m_h * 0.5 * ((cap + m_c) * m_v + (cap - m_c) * v) ;
        //    return -m_h * nlconst::half() * (cap + m_c) * m_v;
        //    //return -m_h * cap * m_v;
        //}

        //void timestep(nl_fptype cap, nl_fptype v, nl_fptype step) noexcept
        //{
        //    m_h = plib::reciprocal(step);
        //    m_c = cap;
        //    m_v = v;
        //}

        //void setparams(nl_fptype gmin) noexcept { m_gmin = gmin; }
    }


    // "Circuit simulation", page 274
    //template <>
    class generic_capacitor_constant  //<capacitor_e::CONSTANT_CAPACITY>
    {
        state_var<nl_fptype> m_h;
        state_var<nl_fptype> m_v;
        nl_fptype m_gmin;


        public generic_capacitor_constant(device_t dev, string name)
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

        //void timestep(nl_fptype cap, nl_fptype v, nl_fptype step) noexcept
        //{
        //    plib::unused_var(cap);
        //    m_h = plib::reciprocal(step);
        //    m_v = v;
        //}

        public void setparams(nl_fptype gmin) { m_gmin = gmin; }
    }
}
