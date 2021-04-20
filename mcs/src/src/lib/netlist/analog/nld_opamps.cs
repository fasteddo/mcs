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
        class opamp_model_t : param_model_t
        {
            public value_t m_TYPE;   //!< Model Type, 1 and 3 are supported
            public value_t m_FPF;    //!< frequency of first pole
            public value_t m_SLEW;   //!< unity gain slew rate
            public value_t m_RI;     //!< input resistance
            public value_t m_RO;     //!< output resistance
            public value_t m_UGF;    //!< unity gain frequency (transition frequency)
            public value_t m_VLL;    //!< low output swing minus low supply rail
            public value_t m_VLH;    //!< high supply rail minus high output swing
            public value_t m_DAB;    //!< Differential Amp Bias - total quiescent current


            public opamp_model_t(device_t device, string name, string val)
                : base(device, name, val)
            {
                m_TYPE = new value_t(this, "TYPE");
                m_FPF = new value_t(this, "FPF");
                m_SLEW = new value_t(this, "SLEW");
                m_RI = new value_t(this, "RI");
                m_RO = new value_t(this, "RO");
                m_UGF = new value_t(this, "UGF");
                m_VLL = new value_t(this, "VLL");
                m_VLH = new value_t(this, "VLH");
                m_DAB = new value_t(this, "DAB");
            }
        }


        //NETLIB_OBJECT(opamp)
        class nld_opamp : device_t
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

            opamp_model_t m_model;
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
                m_model = new opamp_model_t(this, "MODEL", "LM324");
                m_VH = new analog_output_t(this, "VH");
                m_VL = new analog_output_t(this, "VL");
                m_VREF = new analog_output_t(this, "VREF");


                m_type = (int)m_model.m_TYPE.op();
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
                    m_CP = (analog.nld_C)create_and_register_subdevice<analog.nld_C>("CP1");
                    m_EBUF = (analog.nld_VCVS)create_and_register_subdevice<analog.nld_VCVS>("EBUF");
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
                    m_DN = (analog.nld_D)create_and_register_subdevice<analog.nld_D>("DN", "D(IS=1e-15 N=1)");
                    m_DP = (analog.nld_D)create_and_register_subdevice<analog.nld_D>("DP", "D(IS=1e-15 N=1)");

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
                nl_fptype cVt = nlconst.magic(0.0258 * 1.0); // * m_n;
                nl_fptype cId = m_model.m_DAB.op(); // 3 mA
                nl_fptype cVd = cVt * plib.pglobal.log(cId / nlconst.magic(1e-15) + nlconst.one());

                m_VH.push(m_VCC.op() - m_model.m_VLH.op() - cVd);
                m_VL.push(m_GND.op() + m_model.m_VLL.op() + cVd);
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
                m_G1.m_RI.setTo(m_model.m_RI.op());

                if (m_type == 1)
                {
                    nl_fptype RO = m_model.m_RO.op();
                    nl_fptype G = m_model.m_UGF.op() / m_model.m_FPF.op() / RO;
                    m_RP.set_R(RO);
                    m_G1.m_G.setTo(G);
                }
                if (m_type == 3 || m_type == 2)
                {
                    nl_fptype CP = m_model.m_DAB.op() / m_model.m_SLEW.op();
                    nl_fptype RP = nlconst.half() / nlconst.pi() / CP / m_model.m_FPF.op();
                    nl_fptype G = m_model.m_UGF.op() / m_model.m_FPF.op() / RP;

                    //printf("OPAMP %s: %g %g %g\n", name().c_str(), CP, RP, G);
                    if (m_model.m_SLEW.op() / (nlconst.four() * nlconst.pi() * nlconst.magic(0.0258)) < m_model.m_UGF.op())
                        log().warning.op(nl_errstr_global.MW_OPAMP_FAIL_CONVERGENCE(this.name()));

                    m_CP.m_C.setTo(CP);
                    m_RP.set_R(RP);
                    m_G1.m_G.setTo(G);

                }
                if (m_type == 2)
                {
                    m_EBUF.m_G.setTo(nlconst.one());
#if TEST_ALT_OUTPUT
                    m_EBUF->m_RO.setTo(0.001);
                    m_RO->set_R(m_model.m_RO);
#else
                    m_EBUF.m_RO.setTo(m_model.m_RO.op());
#endif
                }
                if (m_type == 3)
                {
                    m_EBUF.m_G.setTo(nlconst.one());
#if TEST_ALT_OUTPUT
                    m_EBUF->m_RO.setTo(0.001);
                    m_RO->set_R(m_model.m_RO);
#else
                    m_EBUF.m_RO.setTo(m_model.m_RO.op());
#endif
                }
            }
        }
    }
}
