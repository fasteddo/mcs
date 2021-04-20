// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;


namespace mame
{
    public static class nld_opamps_global
    {
        // ----------------------------------------------------------------------------------------
        // Macros
        // ----------------------------------------------------------------------------------------

        //#define OPAMP(name, model)                                                     \
        //        NET_REGISTER_DEV(OPAMP, name)                                          \
        //        NETDEV_PARAMI(name, MODEL, model)
        public static void OPAMP(netlist.nlparse_t setup, string name, string model)
        {
            netlist.nl_setup_global.NET_REGISTER_DEV(setup, "OPAMP", name);
            netlist.nl_setup_global.NETDEV_PARAMI(setup, name, "MODEL", model);
        }
    }

    namespace netlist.analog
    {
        class opamp_model_t
        {
            public param_model_t.value_t m_TYPE;   //!< Model Type, 1 and 3 are supported
            public param_model_t.value_t m_FPF;    //!< frequency of first pole
            public param_model_t.value_t m_SLEW;   //!< unity gain slew rate
            public param_model_t.value_t m_RI;     //!< input resistance
            public param_model_t.value_t m_RO;     //!< output resistance
            public param_model_t.value_t m_UGF;    //!< unity gain frequency (transition frequency)
            public param_model_t.value_t m_VLL;    //!< low output swing minus low supply rail
            public param_model_t.value_t m_VLH;    //!< high supply rail minus high output swing
            public param_model_t.value_t m_DAB;    //!< Differential Amp Bias - total quiescent current


            public opamp_model_t(param_model_t model)
            {
                m_TYPE = new param_model_t.value_t(model, "TYPE");
                m_FPF = new param_model_t.value_t(model, "FPF");
                m_SLEW = new param_model_t.value_t(model, "SLEW");
                m_RI = new param_model_t.value_t(model, "RI");
                m_RO = new param_model_t.value_t(model, "RO");
                m_UGF = new param_model_t.value_t(model, "UGF");
                m_VLL = new param_model_t.value_t(model, "VLL");
                m_VLH = new param_model_t.value_t(model, "VLH");
                m_DAB = new param_model_t.value_t(model, "DAB");
            }
        }


        //NETLIB_BASE_OBJECT(opamp)
        class nld_opamp : base_device_t
        {
            //NETLIB_DEVICE_IMPL_NS(analog, opamp, "OPAMP", "MODEL")
            public static readonly factory.constructor_ptr_t decl_opamp = NETLIB_DEVICE_IMPL_NS<nld_opamp>("analog", "OPAMP", "MODEL");


            analog.nld_R_base m_RP;
            analog.nld_VCCS m_G1;
            analog.nld_C m_CP;
#if TEST_ALT_OUTPUT
            NETLIB_SUB_UPTR(analog, R_base) m_RO;
#endif
            analog.nld_VCVS m_EBUF;
            analog.nld_D m_DP;
            analog.nld_D m_DN;

            analog_input_t m_VCC;
            analog_input_t m_GND;

            param_model_t m_model;
            opamp_model_t m_modacc;
            analog_output_t m_VH;
            analog_output_t m_VL;
            analog_output_t m_VREF;

            // state
            int m_type;


            //NETLIB_CONSTRUCTOR(opamp)
            public nld_opamp(object owner, string name)
                : base(owner, name)
            {
                m_RP = new nld_R_base(this, "RP1");
                m_G1 = new nld_VCCS(this, "G1");
                m_VCC = new analog_input_t(this, "VCC");
                m_GND = new analog_input_t(this, "GND");
                m_model = new param_model_t(this, "MODEL", "LM324");
                m_modacc = new opamp_model_t(m_model);
                m_VH = new analog_output_t(this, "VH");
                m_VL = new analog_output_t(this, "VL");
                m_VREF = new analog_output_t(this, "VREF");


                m_type = (int)m_modacc.m_TYPE.op();  //m_type = plib::narrow_cast<int>(m_modacc.m_TYPE);
                if (m_type < 1 || m_type > 3)
                {
                    log().fatal.op(nl_errstr_global.MF_OPAMP_UNKNOWN_TYPE(m_type));
                    throw new nl_exception(nl_errstr_global.MF_OPAMP_UNKNOWN_TYPE(m_type));
                }

                if (m_type == 1)
                {
                    register_subalias("PLUS", "G1.IP");
                    register_subalias("MINUS", "G1.IN");
                    register_subalias("OUT", "G1.OP");

                    connect("G1.ON", "VREF");
                    connect("RP1.2", "VREF");
                    connect("RP1.1", "G1.OP");

                }
                if (m_type == 2 || m_type == 3)
                {
                    create_and_register_subdevice(this, "CP1", out m_CP);
                    create_and_register_subdevice(this, "EBUF", out m_EBUF);

#if TEST_ALT_OUTPUT
                    create_and_register_subdevice("RO", m_RO);
#endif
                    register_subalias("PLUS", "G1.IP");
                    register_subalias("MINUS", "G1.IN");

                    connect("G1.ON", "VREF");
                    connect("RP1.2", "VREF");
                    connect("CP1.2", "VREF");
                    connect("EBUF.ON", "VREF");
                    connect("EBUF.IN", "VREF");

                    connect("RP1.1", "G1.OP");
                    connect("CP1.1", "RP1.1");

                    connect("EBUF.IP", "RP1.1");
                }
                if (m_type == 2)
                {
#if TEST_ALT_OUTPUT
                    connect("EBUF.OP", "RO.1");
                    register_subalias("OUT", "RO.2");
#else
                    register_subalias("OUT", "EBUF.OP");
#endif
                }
                if (m_type == 3)
                {
                    create_and_register_subdevice(this, "DN", out m_DN, "D(IS=1e-15 N=1)");
                    create_and_register_subdevice(this, "DP", out m_DP, "D(IS=1e-15 N=1)");

                    connect("DP.K", "VH");
                    connect("VL", "DN.A");
                    connect("DP.A", "DN.K");
                    connect("DN.K", "RP1.1");
#if TEST_ALT_OUTPUT
                    connect("EBUF.OP", "RO.1");
                    register_subalias("OUT", "RO.2");
#else
                    register_subalias("OUT", "EBUF.OP");
#endif
                }

            }


            //NETLIB_UPDATEI();
            //NETLIB_UPDATE(opamp)
            public override void update()
            {
                nl_fptype cVt = nlconst.np_VT(nlconst.one()); // * m_n;
                nl_fptype cId = m_modacc.m_DAB.op(); // 3 mA
                nl_fptype cVd = cVt * plib.pglobal.log(cId / nlconst.np_Is() + nlconst.one());

                m_VH.push(m_VCC.op() - m_modacc.m_VLH.op() - cVd);
                m_VL.push(m_GND.op() + m_modacc.m_VLL.op() + cVd);
                m_VREF.push((m_VCC.op() + m_GND.op()) / nlconst.two());
            }


            //NETLIB_RESETI()
            public override void reset()
            {
            }


            //NETLIB_UPDATE_PARAMI();
            //NETLIB_UPDATE_PARAM(opamp)
            public override void update_param()
            {
                m_G1.m_RI.set(m_modacc.m_RI.op());

                if (m_type == 1)
                {
                    nl_fptype RO = m_modacc.m_RO.op();
                    nl_fptype G = m_modacc.m_UGF.op() / m_modacc.m_FPF.op() / RO;
                    m_RP.set_R(RO);
                    m_G1.m_G.set(G);
                }
                if (m_type == 3 || m_type == 2)
                {
                    nl_fptype CP = m_modacc.m_DAB.op() / m_modacc.m_SLEW.op();
                    nl_fptype RP = nlconst.half() / nlconst.pi() / CP / m_modacc.m_FPF.op();
                    nl_fptype G = m_modacc.m_UGF.op() / m_modacc.m_FPF.op() / RP;

                    //printf("OPAMP %s: %g %g %g\n", name().c_str(), CP, RP, G);
                    if (m_modacc.m_SLEW.op() / (nlconst.four() * nlconst.pi() * nlconst.np_VT()) < m_modacc.m_UGF.op())
                        log().warning.op(nl_errstr_global.MW_OPAMP_FAIL_CONVERGENCE(this.name()));

                    m_CP.set_cap_embedded(CP);
                    m_RP.set_R(RP);
                    m_G1.m_G.set(G);

                }
                if (m_type == 2)
                {
                    m_EBUF.m_G.set(nlconst.one());
#if TEST_ALT_OUTPUT
                    m_EBUF->m_RO.set(0.001);
                    m_RO->set_R(m_modacc.m_RO);
#else
                    m_EBUF.m_RO.set(m_modacc.m_RO.op());
#endif
                }
                if (m_type == 3)
                {
                    m_EBUF.m_G.set(nlconst.one());
#if TEST_ALT_OUTPUT
                    m_EBUF->m_RO.set(0.001);
                    m_RO->set_R(m_modacc.m_RO);
#else
                    m_EBUF.m_RO.set(m_modacc.m_RO.op());
#endif
                }
            }
        }
    }
}
