// license:BSD-3-Clause
// copyright-holders:Edward Fast

#define NL_USE_BACKWARD_EULER

using System;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;

using static mame.nl_factory_global;


namespace mame.netlist.devices
{
    //NETLIB_OBJECT(NE555)
    class nld_NE555 : device_t
    {
        //NETLIB_DEVICE_IMPL(NE555,     "NE555", "")
        public static readonly netlist.factory.constructor_ptr_t decl_NE555 = NETLIB_DEVICE_IMPL<nld_NE555>("NE555", "");

        //NETLIB_DEVICE_IMPL_ALIAS(MC1455P, NE555,     "MC1455P", "")


        const nl_fptype R_OFF = 1E20;
        const nl_fptype R_ON  = 1;


        analog.nld_R_base m_R1;  //analog::NETLIB_SUB(R_base) m_R1;  //analog::NETLIB_SUB(R_base) m_R1;
        analog.nld_R_base m_R2;  //analog::NETLIB_SUB(R_base) m_R2;  //analog::NETLIB_SUB(R_base) m_R2;
        analog.nld_R_base m_R3;  //analog::NETLIB_SUB(R_base) m_R3;  //analog::NETLIB_SUB(R_base) m_R3;
        analog.nld_R_base m_ROUT;  //analog::NETLIB_SUB(R_base) m_ROUT;  //analog::NETLIB_SUB(R_base) m_ROUT;
        analog.nld_R_base m_RDIS;  //analog::NETLIB_SUB(R_base) m_RDIS;  //analog::NETLIB_SUB(R_base) m_RDIS;

        logic_input_t m_RESET;
        analog_input_t m_THRES;
        analog_input_t m_TRIG;
        analog_output_t m_OUT;

        state_var<bool> m_last_out;
        state_var<bool> m_ff;
        state_var<bool> m_last_reset;
        state_var<nl_fptype> m_overshoot;
        state_var<nl_fptype> m_undershoot;
        nl_fptype m_ovlimit;


        //NETLIB_CONSTRUCTOR(NE555)
        public nld_NE555(object owner, string name)
            : base(owner, name)
        {
            m_R1 = new analog.nld_R_base(this, "R1");
            m_R2 = new analog.nld_R_base(this, "R2");
            m_R3 = new analog.nld_R_base(this, "R3");
            m_ROUT = new analog.nld_R_base(this, "ROUT");
            m_RDIS = new analog.nld_R_base(this, "RDIS");
            m_RESET = new logic_input_t(this, "RESET", inputs);     // Pin 4
            m_THRES = new analog_input_t(this, "THRESH", inputs);    // Pin 6
            m_TRIG = new analog_input_t(this, "TRIG", inputs);       // Pin 2
            m_OUT = new analog_output_t(this, "_OUT");        // to Pin 3 via ROUT
            m_last_out = new state_var<bool>(this, "m_last_out", false);
            m_ff = new state_var<bool>(this, "m_ff", false);
            m_last_reset = new state_var<bool>(this, "m_last_reset", false);
            m_overshoot = new state_var<nl_fptype>(this, "m_overshoot", 0.0);
            m_undershoot = new state_var<nl_fptype>(this, "m_undershoot", 0.0);
            m_ovlimit = 0.0;


            register_subalias("GND",  "R3.2");    // Pin 1
            register_subalias("CONT", "R1.2");    // Pin 5
            register_subalias("DISCH", "RDIS.1"); // Pin 7
            register_subalias("VCC",  "R1.1");    // Pin 8
            register_subalias("OUT",  "ROUT.1");  // Pin 3

            connect("R1.2", "R2.1");
            connect("R2.2", "R3.1");
            connect("RDIS.2", "R3.2");
            connect("_OUT", "ROUT.2");
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            /* FIXME make resistances a parameter, properly model other variants */
            m_R1.set_R(nlconst.magic(5000));
            m_R2.set_R(nlconst.magic(5000));
            m_R3.set_R(nlconst.magic(5000));
            m_ROUT.set_R(nlconst.magic(20));
            m_RDIS.set_R(nlconst.magic(R_OFF));

            m_last_out.op = true;
            // Check for astable setup, usually TRIG AND THRES connected. Enable
            // overshoot compensation in this case.
            if (m_TRIG.net() == m_THRES.net())
                m_ovlimit = nlconst.magic(0.5);
        }


        //NETLIB_HANDLERI(inputs)
        void inputs()
        {
            // FIXME: assumes GND is connected to 0V.

            var reset = m_RESET.op();

            nl_fptype vthresh = clamp_hl(m_R2.P().op(), nlconst.magic(0.7), nlconst.magic(1.4));
            nl_fptype vtrig = clamp_hl(m_R2.N().op(), nlconst.magic(0.7), nlconst.magic(1.4));

            // avoid artificial oscillation due to overshoot compensation when
            // the control input is used.
            var ovlimit = std.min(m_ovlimit, std.max(0.0, (vthresh - vtrig) / 3.0));

            if (reset == 0 && m_last_reset.op)
            {
                m_ff.op = false;
            }
            else
            {
#if (NL_USE_BACKWARD_EULER)
                bool bthresh = (m_THRES.op() + m_overshoot.op > vthresh);
                bool btrig = (m_TRIG.op() - m_overshoot.op > vtrig);
#else
                const bool bthresh = (m_THRES() + m_overshoot > vthresh);
                const bool btrig = (m_TRIG() - m_undershoot > vtrig);
#endif
                if (!btrig)
                {
                    m_ff.op = true;
                }
                else if (bthresh)
                {
                    m_ff.op = false;
                }
            }

            bool out_ = (reset == 0 ? false : m_ff.op);

            if (m_last_out.op && !out_)
            {
#if (NL_USE_BACKWARD_EULER)
                m_overshoot.op += ((m_THRES.op() - vthresh)) * 2.0;
#else
                m_overshoot += ((m_THRES() - vthresh));
#endif
                m_overshoot.op = plib.pg.clamp(m_overshoot.op, nlconst.zero(), ovlimit);
                //if (this->name() == "IC6_2")
                //  printf("%f %s %f %f %f\n", exec().time().as_double(), this->name().c_str(), m_overshoot(), m_R2.P()(), m_THRES());
                m_RDIS.change_state(() =>
                    {
                        m_RDIS.set_R(nlconst.magic(R_ON));
                    });
                m_OUT.push(m_R3.N().op());
            }
            else if (!m_last_out.op && out_)
            {
#if (NL_USE_BACKWARD_EULER)
                m_overshoot.op += (vtrig - m_TRIG.op()) * 2.0;
                m_overshoot.op = plib.pg.clamp(m_overshoot.op, nlconst.zero(), ovlimit);
#else
                m_undershoot += (vtrig - m_TRIG());
                m_undershoot = plib::clamp(m_undershoot(), nlconst::zero(), ovlimit);
#endif
                m_RDIS.change_state(() =>
                    {
                        m_RDIS.set_R(nlconst.magic(R_OFF));
                    });
                // FIXME: Should be delayed by 100ns
                m_OUT.push(m_R1.P().op());
            }

            m_last_reset.op = reset != 0;
            m_last_out.op = out_;
        }


        nl_fptype clamp_hl(nl_fptype v, nl_fptype a, nl_fptype b)
        {
            nl_fptype vcc = m_R1.P().op();
            return plib.pg.clamp(v, b, vcc - a);
        }
    }
}
