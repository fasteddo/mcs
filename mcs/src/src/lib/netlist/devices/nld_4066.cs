// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;


namespace mame
{
    public static class nld_4066_global
    {
        //#define CD4066_GATE(name)                                                       \
        //        NET_REGISTER_DEV(CD4066_GATE, name)
        public static void CD4066_GATE(netlist.nlparse_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "CD4066_GATE", name); }
    }


    namespace netlist.devices
    {
        //NETLIB_OBJECT(CD4066_GATE)
        class nld_CD4066_GATE : device_t
        {
            //NETLIB_DEVICE_IMPL(CD4066_GATE,         "CD4066_GATE",            "")
            public static readonly netlist.factory.constructor_ptr_t decl_CD4066_GATE = NETLIB_DEVICE_IMPL<nld_CD4066_GATE>("CD4066_GATE", "");


            detail.family_setter_t m_famsetter;

            nld_power_pins             m_supply;
            analog.nld_R_base m_R;

            analog_input_t             m_control;
            param_fp_t                 m_base_r;
            state_var<bool>            m_last;


            //NETLIB_CONSTRUCTOR(CD4066_GATE)
            public nld_CD4066_GATE(object owner, string name)
                : base(owner, name)
            {
                m_famsetter = new detail.family_setter_t(this, "CD4XXX");  //NETLIB_FAMILY("CD4XXX")
                m_supply = new nld_power_pins(this, "VDD", "VSS");
                m_R = new analog.nld_R_base(this, "R");
                m_control = new analog_input_t(this, "CTL");
                m_base_r = new param_fp_t(this, "BASER", nlconst.magic(270.0));
                m_last = new state_var<bool>(this, "m_last", false);
            }


            //NETLIB_RESETI();
            //NETLIB_RESET(CD4066_GATE)
            public override void reset()
            {
                // Start in off condition
                // FIXME: is ROFF correct?
                m_R.set_R(plib.pglobal.reciprocal(exec().gmin()));
            }


            //NETLIB_UPDATEI();
            //NETLIB_UPDATE(CD4066_GATE)
            public override void update()
            {
                nl_fptype sup = (m_supply.VCC().Q_Analog() - m_supply.GND().Q_Analog());
                nl_fptype low = nlconst.magic(0.45) * sup;
                nl_fptype high = nlconst.magic(0.55) * sup;
                nl_fptype in_ = m_control.op() - m_supply.GND().Q_Analog();
                nl_fptype rON = m_base_r.op() * nlconst.magic(5.0) / sup;
                nl_fptype R = -nlconst.one();
                bool new_state = false;

                if (in_ < low)
                {
                    R = plib.pglobal.reciprocal(exec().gmin());
                }
                else if (in_ > high)
                {
                    R = rON;
                    new_state = true;
                }
                //printf("%s %f %f %g\n", name().c_str(), sup, in, R);
                if (R > nlconst.zero() && (m_last.op != new_state))
                {
                    m_last.op = new_state;
                    m_R.update();
                    m_R.set_R(R);
                    m_R.solve_later();
                }
            }
        }
    }
}
