// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using base_device_t_constructor_param_t = mame.netlist.core_device_data_t;  //using constructor_param_t = base_device_param_t;  //using base_device_param_t = const base_device_data_t &;  //using base_device_data_t = core_device_data_t;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using param_logic_t = mame.netlist.param_num_t<bool, mame.netlist.param_num_t_operators_bool>;  //using param_logic_t = param_num_t<bool>;

using static mame.nl_factory_global;


namespace mame.netlist
{
    namespace analog
    {
        // ----------------------------------------------------------------------------------------
        // SWITCH
        // ----------------------------------------------------------------------------------------
        //class nld_switch1 : public base_device_t


        // ----------------------------------------------------------------------------------------
        // SWITCH2
        // ----------------------------------------------------------------------------------------
        class nld_switch2 : base_device_t
        {
            //NETLIB_DEVICE_IMPL_NS(analog, switch2, "SWITCH2", "")
            public static readonly factory.constructor_ptr_t decl_switch2 = NETLIB_DEVICE_IMPL_NS<nld_switch2>("analog", "SWITCH2", "");


            nl_fptype R_OFF { get { return plib.pg.reciprocal(exec().gmin()); } }
            static readonly nl_fptype R_ON = nlconst.magic(0.01);


            sub_device_wrapper<analog.nld_R_base> m_R1;  //NETLIB_SUB_NS(analog, R_base) m_R1;
            sub_device_wrapper<analog.nld_R_base> m_R2;  //NETLIB_SUB_NS(analog, R_base) m_R2;
            param_logic_t m_POS;


            public nld_switch2(base_device_t_constructor_param_t data)
                : base(data)
            {
                m_R1 = new sub_device_wrapper<nld_R_base>(this, new nld_R_base(new base_device_t_constructor_param_t(this, "R1")));
                m_R2 = new sub_device_wrapper<nld_R_base>(this, new nld_R_base(new base_device_t_constructor_param_t(this, "R2")));
                m_POS = new param_logic_t(this, "POS", false);


                connect(m_R1.op().N(), m_R2.op().N());

                register_sub_alias("1", m_R1.op().P());
                register_sub_alias("2", m_R2.op().P());

                register_sub_alias("Q", m_R1.op().N());
            }


            //NETLIB_RESETI();
            public override void reset()
            {
                m_R1.op().set_R(R_ON);
                m_R2.op().set_R(R_OFF);
            }


//#ifdef FIXMELATER
            //NETLIB_UPDATE(switch2)
            //{
            //    if (!m_POS())
            //    {
            //        m_R1.set_R(R_ON);
            //        m_R2.set_R(R_OFF);
            //    }
            //    else
            //    {
            //        m_R1.set_R(R_OFF);
            //        m_R2.set_R(R_ON);
            //    }
            //}
//#endif


            //NETLIB_UPDATE_PARAMI();
            //NETLIB_UPDATE_PARAM(switch2)
            public override void update_param()
            {
                // R1 and R2 are connected. However this net may be a rail net.
                // The code here thus is a bit more complex.

                nl_fptype r1 = m_POS.op() ? R_OFF : R_ON;
                nl_fptype r2 = m_POS.op() ? R_ON : R_OFF;

                if (m_R1.op().solver() == m_R2.op().solver())
                {
                    m_R1.op().change_state(() => { m_R1.op().set_R(r1); m_R2.op().set_R(r2); });
                }
                else
                {
                    m_R1.op().change_state(() => { m_R1.op().set_R(r1); });
                    m_R2.op().change_state(() => { m_R2.op().set_R(r2); });
                }
            }
        }
    }
}
