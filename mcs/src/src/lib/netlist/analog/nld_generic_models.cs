// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_double = System.Double;


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
        state_var<double> m_h;
        state_var<double> m_c;
        state_var<double> m_v;
        nl_double m_gmin;


        generic_capacitor_variable(device_t dev, string name)
        {
            m_h = new state_var<double>(dev, name + ".m_h", 0.0);
            m_c = new state_var<double>(dev, name + ".m_c", 0.0);
            m_v = new state_var<double>(dev, name + ".m_v", 0.0);
            m_gmin = 0.0;
        }

        capacitor_e type() { return capacitor_e.VARIABLE_CAPACITY; }

        // Circuit Simulation, page 284, 5.360
        // q(un+1) - q(un) = int(un, un+1, C(U)) = (C0+C1)/2 * (un+1-un)
        // The direct application of formulas 5.359 and 5.360 has
        // issues with pulses. Therefore G and Ieq are expressed differently
        // so that G depends on un+1 only and Ieq on un only.
        // In both cases, i = G * un+1 + Ieq

        //nl_double G(nl_double cap) const
        //{
        //    //return m_h * cap +  m_gmin;
        //    return m_h * 0.5 * (cap + m_c) +  m_gmin;
        //    //return m_h * cap +  m_gmin;
        //}

        //nl_double Ieq(nl_double cap, nl_double v) const
        //{
        //    plib::unused_var(v);
        //    //return -m_h * 0.5 * ((cap + m_c) * m_v + (cap - m_c) * v) ;
        //    return -m_h * 0.5 * (cap + m_c) * m_v;
        //    //return -m_h * cap * m_v;
        //}

        //void timestep(nl_double cap, nl_double v, nl_double step)
        //{
        //    m_h = 1.0 / step;
        //    m_c = cap;
        //    m_v = v;
        //}

        //void setparams(nl_double gmin) { m_gmin = gmin; }
    }


    // "Circuit simulation", page 274
    //template <>
    class generic_capacitor_constant  //<capacitor_e::CONSTANT_CAPACITY>
    {
        state_var<nl_double> m_h;
        state_var<double> m_v;
        nl_double m_gmin;


        public generic_capacitor_constant(device_t dev, string name)
        {
            m_h = new state_var<nl_double>(dev, name + ".m_h", 0.0);
            m_v = new state_var<nl_double>(dev, name + ".m_v", 0.0);
            m_gmin = 0.0;
        }


        public capacitor_e type() { return capacitor_e.CONSTANT_CAPACITY; }

        public nl_double G(nl_double cap) { return cap * m_h.op +  m_gmin; }

        public nl_double Ieq(nl_double cap, nl_double v)
        {
            //plib::unused_var(v);
            return - G(cap) * m_v.op;
        }

        //void timestep(nl_double cap, nl_double v, nl_double step)
        //{
        //    plib::unused_var(cap);
        //    m_h = 1.0 / step;
        //    m_v = v;
        //}

        public void setparams(nl_double gmin) { m_gmin = gmin; }
    }
}
