// license:BSD-3-Clause
// copyright-holders:Edward Fast

#define NL_USE_BACKWARD_EULER

using System;

using device_t_constructor_param_t = mame.netlist.core_device_data_t;  //using constructor_param_t = device_param_t;  //using device_param_t = const device_data_t &;  //using device_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;
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


        sub_device_wrapper<analog.nld_R_base> m_R1;  //NETLIB_SUB_NS(analog, R_base) m_R1;
        sub_device_wrapper<analog.nld_R_base> m_R2;  //NETLIB_SUB_NS(analog, R_base) m_R2;
        sub_device_wrapper<analog.nld_R_base> m_R3;  //NETLIB_SUB_NS(analog, R_base) m_R3;
        sub_device_wrapper<analog.nld_R_base> m_ROUT;  //NETLIB_SUB_NS(analog, R_base) m_ROUT;
        sub_device_wrapper<analog.nld_R_base> m_RDIS;  //NETLIB_SUB_NS(analog, R_base) m_RDIS;

        logic_input_t m_RESET;
        analog_input_t m_THRES;
        analog_input_t m_TRIG;
        analog_output_t m_OUT;

        state_var<bool> m_last_out;
        state_var<bool> m_ff;
        state_var<bool> m_last_reset;
        state_var<nl_fptype> m_overshoot;
        state_var<nl_fptype> m_undershoot;
        nl_fptype m_overshoot_limit;


        //NETLIB_CONSTRUCTOR(NE555)
        public nld_NE555(device_t_constructor_param_t data)
            : base(data)
        {
            m_R1 = new sub_device_wrapper<analog.nld_R_base>(this, new analog.nld_R_base(new device_t_constructor_param_t(this, "R1")));
            m_R2 = new sub_device_wrapper<analog.nld_R_base>(this, new analog.nld_R_base(new device_t_constructor_param_t(this, "R2")));
            m_R3 = new sub_device_wrapper<analog.nld_R_base>(this, new analog.nld_R_base(new device_t_constructor_param_t(this, "R3")));
            m_ROUT = new sub_device_wrapper<analog.nld_R_base>(this, new analog.nld_R_base(new device_t_constructor_param_t(this, "ROUT")));
            m_RDIS = new sub_device_wrapper<analog.nld_R_base>(this, new analog.nld_R_base(new device_t_constructor_param_t(this, "RDIS")));
            m_RESET = new logic_input_t(this, "RESET", inputs);     // Pin 4
            m_THRES = new analog_input_t(this, "THRESH", inputs);    // Pin 6
            m_TRIG = new analog_input_t(this, "TRIG", inputs);       // Pin 2
            m_OUT = new analog_output_t(this, "_OUT");        // to Pin 3 via ROUT
            m_last_out = new state_var<bool>(this, "m_last_out", false);
            m_ff = new state_var<bool>(this, "m_ff", false);
            m_last_reset = new state_var<bool>(this, "m_last_reset", false);
            m_overshoot = new state_var<nl_fptype>(this, "m_overshoot", 0.0);
            m_undershoot = new state_var<nl_fptype>(this, "m_undershoot", 0.0);
            m_overshoot_limit = 0.0;


            register_sub_alias("GND",  "R3.2");    // Pin 1
            register_sub_alias("CONT", "R1.2");    // Pin 5
            register_sub_alias("DISCH", "RDIS.1"); // Pin 7
            register_sub_alias("VCC",  "R1.1");    // Pin 8
            register_sub_alias("OUT",  "ROUT.1");  // Pin 3

            connect("R1.2", "R2.1");
            connect("R2.2", "R3.1");
            connect("RDIS.2", "R3.2");
            connect("_OUT", "ROUT.2");
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            /* FIXME make resistances a parameter, properly model other variants */
            m_R1.op().set_R(nlconst.magic(5000));
            m_R2.op().set_R(nlconst.magic(5000));
            m_R3.op().set_R(nlconst.magic(5000));
            m_ROUT.op().set_R(nlconst.magic(20));
            m_RDIS.op().set_R(nlconst.magic(R_OFF));

            m_last_out.op = true;
            // Check for astable setup, usually TRIG AND THRES connected. Enable
            // overshoot compensation in this case.
            if (m_TRIG.net() == m_THRES.net())
                m_overshoot_limit = nlconst.magic(0.5);
        }


        //NETLIB_HANDLERI(inputs)
        void inputs()
        {
            // FIXME: assumes GND is connected to 0V.

            var reset = m_RESET.op();

            nl_fptype v_threshold = clamp_hl(m_R2.op().P().op(), nlconst.magic(0.7), nlconst.magic(1.4));
            nl_fptype v_trigger = clamp_hl(m_R2.op().N().op(), nlconst.magic(0.7), nlconst.magic(1.4));

            // avoid artificial oscillation due to overshoot compensation when
            // the control input is used.
            var overshoot_limit = std.min(m_overshoot_limit, std.max(0.0, (v_threshold - v_trigger) / 3.0));

            if (reset == 0 && m_last_reset.op)
            {
                m_ff.op = false;
            }
            else
            {
#if (NL_USE_BACKWARD_EULER)
                bool threshold_exceeded = (m_THRES.op() + m_overshoot.op > v_threshold);
                bool trigger_exceeded = (m_TRIG.op() - m_overshoot.op > v_trigger);
#else
                const bool threshold_exceeded = (m_THRES() + m_overshoot > v_threshold);
                const bool trigger_exceeded = (m_TRIG() - m_undershoot > v_trigger);
#endif
                if (!trigger_exceeded)
                {
                    m_ff.op = true;
                }
                else if (threshold_exceeded)
                {
                    m_ff.op = false;
                }
            }

            bool out_ = (reset == 0 ? false : m_ff.op);

            if (m_last_out.op && !out_)
            {
#if (NL_USE_BACKWARD_EULER)
                m_overshoot.op += ((m_THRES.op() - v_threshold)) * 2.0;
#else
                m_overshoot += ((m_THRES() - v_threshold));
#endif
                m_overshoot.op = plib.pg.clamp(m_overshoot.op, nlconst.zero(), overshoot_limit);
                //if (this->name() == "IC6_2")
                //  printf("%f %s %f %f %f\n", exec().time().as_double(), this->name().c_str(), m_overshoot(), m_R2.P()(), m_THRES());
                m_RDIS.op().change_state(() =>
                    {
                        m_RDIS.op().set_R(nlconst.magic(R_ON));
                    });
                m_OUT.push(m_R3.op().N().op());
            }
            else if (!m_last_out.op && out_)
            {
#if (NL_USE_BACKWARD_EULER)
                m_overshoot.op += (v_trigger - m_TRIG.op()) * 2.0;
                m_overshoot.op = plib.pg.clamp(m_overshoot.op, nlconst.zero(), overshoot_limit);
#else
                m_undershoot += (v_trigger - m_TRIG());
                m_undershoot = plib::clamp(m_undershoot(), nlconst::zero(), overshoot_limit);
#endif
                m_RDIS.op().change_state(() =>
                    {
                        m_RDIS.op().set_R(nlconst.magic(R_OFF));
                    });
                // FIXME: Should be delayed by 100ns
                m_OUT.push(m_R1.op().P().op());
            }

            m_last_reset.op = reset != 0;
            m_last_out.op = out_;
        }


        nl_fptype clamp_hl(nl_fptype v, nl_fptype a, nl_fptype b)
        {
            nl_fptype vcc = m_R1.op().P().op();
            return plib.pg.clamp(v, b, vcc - a);
        }
    }
}
