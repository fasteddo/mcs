// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using param_fp_t = mame.netlist.param_num_t<System.Double, mame.netlist.param_num_t_operators_double>;  //using param_fp_t = param_num_t<nl_fptype>;


namespace mame.netlist.devices
{
    //NETLIB_OBJECT(CD4066_GATE)
    class nld_CD4066_GATE : device_t
    {
        //NETLIB_DEVICE_IMPL(CD4066_GATE,         "CD4066_GATE",            "")
        public static readonly netlist.factory.constructor_ptr_t decl_CD4066_GATE = NETLIB_DEVICE_IMPL<nld_CD4066_GATE>("CD4066_GATE", "");


        analog.nld_R_base m_R;
        analog_input_t m_control;
        param_fp_t m_base_r;
        state_var<bool> m_last;
        nld_power_pins m_supply;


        //NETLIB_CONSTRUCTOR_MODEL(CD4066_GATE, "CD4XXX")
        public nld_CD4066_GATE(object owner, string name)
            : base(owner, name, "CD4XXX")
        {
            m_R = new analog.nld_R_base(this, "R");
            m_control = new analog_input_t(this, "CTL", control);
            m_base_r = new param_fp_t(this, "BASER", nlconst.magic(270.0));
            m_last = new state_var<bool>(this, "m_last", false);
            m_supply = new nld_power_pins(this);
        }


        //NETLIB_RESETI();
        public override void reset()
        {
            // Start in off condition
            // FIXME: is ROFF correct?
            m_R.set_R(plib.pglobal.reciprocal<nl_fptype, nl_fptype_ops>(exec().gmin()));
        }


        //NETLIB_HANDLERI(control)
        void control()
        {
            nl_fptype sup = (m_supply.VCC().Q_Analog() - m_supply.GND().Q_Analog());
            nl_fptype in_ = m_control.op() - m_supply.GND().Q_Analog();
            nl_fptype rON = m_base_r.op() * nlconst.magic(5.0) / sup;
            nl_fptype R = -nlconst.one();
            nl_fptype low = nlconst.magic(0.45) * sup;
            nl_fptype high = nlconst.magic(0.55) * sup;
            bool new_state = false;

            if (in_ < low)
            {
                R = plib.pglobal.reciprocal<nl_fptype, nl_fptype_ops>(exec().gmin());
            }
            else if (in_ > high)
            {
                R = rON;
                new_state = true;
            }

            if (R > nlconst.zero() && (m_last.op != new_state))
            {
                m_last.op = new_state;
                m_R.change_state(() => { this.m_R.set_R(R);});  //m_R.change_state([this, &R]() -> void { this->m_R.set_R(R);});
            }
        }
    }
}
