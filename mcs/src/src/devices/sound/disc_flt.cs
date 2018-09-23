// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using osd_ticks_t = System.UInt64;
using uint8_t = System.Byte;


namespace mame
{
    static class disc_flt_global
    {
        public static void calculate_filter2_coefficients(discrete_base_node node, double fc, double d, double type, ref discrete_filter_coeff coeff)
        {
            double w;   /* cutoff freq, in radians/sec */
            double w_squared;
            double den; /* temp variable */
            double two_over_T = 2 * node.sample_rate();
            double two_over_T_squared = two_over_T * two_over_T;

            /* calculate digital filter coefficents */
            /*w = 2.0*M_PI*fc; no pre-warping */
            w = node.sample_rate() * 2.0 * Math.Tan(Math.PI * fc / node.sample_rate()); /* pre-warping */
            w_squared = w * w;

            den = two_over_T_squared + d*w*two_over_T + w_squared;

            coeff.a1 = 2.0 * (-two_over_T_squared + w_squared) / den;
            coeff.a2 = (two_over_T_squared - d * w * two_over_T + w_squared) / den;

            if (type == discrete_global.DISC_FILTER_LOWPASS)
            {
                coeff.b0 = coeff.b2 = w_squared/den;
                coeff.b1 = 2.0 * (coeff.b0);
            }
            else if (type == discrete_global.DISC_FILTER_BANDPASS)
            {
                coeff.b0 = d * w * two_over_T / den;
                coeff.b1 = 0.0;
                coeff.b2 = -(coeff.b0);
            }
            else if (type == discrete_global.DISC_FILTER_HIGHPASS)
            {
                coeff.b0 = coeff.b2 = two_over_T_squared / den;
                coeff.b1 = -2.0 * (coeff.b0);
            }
            else
            {
                /* FIXME: reenable */
                //node->device->discrete_log("calculate_filter2_coefficients() - Invalid filter type for 2nd order filter.");
            }
        }
    }


    struct discrete_filter_coeff
    {
        public double x1, x2;      /* x[k-1], x[k-2], previous 2 input values */
        public double y1, y2;      /* y[k-1], y[k-2], previous 2 output values */
        public double a1, a2;      /* digital filter coefficients, denominator */
        public double b0, b1, b2;  /* digital filter coefficients, numerator */
    }


#if false
    DISCRETE_CLASS_STEP_RESET(dst_filter1, 1,
        /* uses x1, y1, a1, b0, b1 */
        struct discrete_filter_coeff m_fc;
    );
#endif

#if false
    DISCRETE_CLASS_STEP_RESET(dst_filter2, 1,
        struct discrete_filter_coeff m_fc;
    );
#endif

#if false
    DISCRETE_CLASS_STEP_RESET(dst_sallen_key, 1,
        struct discrete_filter_coeff m_fc;
    );
#endif


    //DISCRETE_CLASS_STEP_RESET(dst_crfilter, 1,
    class discrete_dst_crfilter_node : discrete_base_node,
                                       discrete_step_interface
    {
        const int _maxout = 1;


        double DST_CRFILTER__IN { get { return DISCRETE_INPUT(0); } }
        double DST_CRFILTER__R { get { return DISCRETE_INPUT(1); } }
        double DST_CRFILTER__C { get { return DISCRETE_INPUT(2); } }


        double m_vCap;
        double m_rc;
        double m_exponent;
        uint8_t m_has_rc_nodes;
        //UINT8           m_is_fast;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)
        public discrete_dst_crfilter_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)
        ~discrete_dst_crfilter_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_crfilter)
        public override void reset()
        {
            m_has_rc_nodes = (byte)(input_is_node() & 0x6);
            m_rc = DST_CRFILTER__R * DST_CRFILTER__C;
            m_exponent = RC_CHARGE_EXP(m_rc);
            m_vCap = 0;
            set_output(0,  DST_CRFILTER__IN);
        }

        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_crfilter)
        public void step()
        {
            throw new emu_unimplemented();
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_op_amp_filt, 1,
    class discrete_dst_op_amp_filt_node : discrete_base_node,
                                          discrete_step_interface
    {
        const int _maxout = 1;

        /************************************************************************
         *
         * DST_OP_AMP_FILT - Op Amp filter circuit RC filter
         *
         * input[0]    - Enable input value
         * input[1]    - IN0 node
         * input[2]    - IN1 node
         * input[3]    - Filter Type
         *
         * also passed discrete_op_amp_filt_info structure
         *
         * Mar 2004, D Renaud.
         ************************************************************************/
        //#define DST_OP_AMP_FILT__ENABLE DISCRETE_INPUT(0)
        //#define DST_OP_AMP_FILT__INP1   DISCRETE_INPUT(1)
        //#define DST_OP_AMP_FILT__INP2   DISCRETE_INPUT(2)
        //#define DST_OP_AMP_FILT__TYPE   DISCRETE_INPUT(3)
        public double DST_OP_AMP_FILT__ENABLE() { return DISCRETE_INPUT(0); }
        public double DST_OP_AMP_FILT__INP1() { return DISCRETE_INPUT(1); }
        public double DST_OP_AMP_FILT__INP2() { return DISCRETE_INPUT(2); }
        public double DST_OP_AMP_FILT__TYPE() { return DISCRETE_INPUT(3); }


        int m_type;                 /* What kind of filter */
        int m_is_norton;            /* 1 = Norton op-amps */
        double m_vRef;
        double m_vP;
        double m_vN;
        double m_rTotal;               /* All input resistance in parallel. */
        double m_iFixed;               /* Current supplied by r3 & r4 if used. */
        double m_exponentC1;
        double m_exponentC2;
        double m_exponentC3;
        double m_rRatio;               /* divide ratio of resistance network */
        double m_vC1;                  /* Charge on C1 */
        double m_vC1b;                 /* Charge on C1, part of C1 charge if needed */
        double m_vC2;                  /* Charge on C2 */
        double m_vC3;                  /* Charge on C2 */
        double m_gain;                 /* Gain of the filter */
        discrete_filter_coeff m_fc;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)
        public discrete_dst_op_amp_filt_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)
        ~discrete_dst_op_amp_filt_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_op_amp_filt)
        public override void reset()
        {
            //DISCRETE_DECLARE_INFO(discrete_op_amp_filt_info)
            discrete_op_amp_filt_info info = (discrete_op_amp_filt_info)custom_data();

            /* Convert the passed filter type into an int for easy use. */
            m_type = (int)DST_OP_AMP_FILT__TYPE() & discrete_global.DISC_OP_AMP_FILTER_TYPE_MASK;
            m_is_norton = (int)DST_OP_AMP_FILT__TYPE() & discrete_global.DISC_OP_AMP_IS_NORTON;

            if (m_is_norton != 0)
            {
                m_vRef = 0;
                m_rTotal = info.r1;
                if (m_type == (discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_0 | discrete_global.DISC_OP_AMP_IS_NORTON))
                    m_rTotal += info.r2 +  info.r3;

                /* Setup the current to the + input. */
                m_iFixed = (info.vP - discrete_global.OP_AMP_NORTON_VBE) / info.r4;

                /* Set the output max. */
                m_vP =  info.vP - discrete_global.OP_AMP_NORTON_VBE;
                m_vN =  info.vN;
            }
            else
            {
                m_vRef = info.vRef;
                /* Set the output max. */
                m_vP =  info.vP - discrete_global.OP_AMP_VP_RAIL_OFFSET;
                m_vN =  info.vN;

                /* Work out the input resistance.  It is all input and bias resistors in parallel. */
                m_rTotal  = 1.0 / info.r1;         /* There has to be an R1.  Otherwise the table is wrong. */
                if (info.r2 != 0) m_rTotal += 1.0 / info.r2;
                if (info.r3 != 0) m_rTotal += 1.0 / info.r3;
                m_rTotal = 1.0 / m_rTotal;

                m_iFixed = 0;

                m_rRatio = info.rF / (m_rTotal + info.rF);
                m_gain = -info.rF / m_rTotal;
            }

            switch (m_type)
            {
                case discrete_global.DISC_OP_AMP_FILTER_IS_LOW_PASS_1:
                case discrete_global.DISC_OP_AMP_FILTER_IS_LOW_PASS_1_A:
                    m_exponentC1 = RC_CHARGE_EXP(info.rF * info.c1);
                    m_exponentC2 =  0;
                    break;

                case discrete_global.DISC_OP_AMP_FILTER_IS_HIGH_PASS_1:
                    m_exponentC1 = RC_CHARGE_EXP(m_rTotal * info.c1);
                    m_exponentC2 =  0;
                    break;

                case discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1:
                    m_exponentC1 = RC_CHARGE_EXP(info.rF * info.c1);
                    m_exponentC2 = RC_CHARGE_EXP(m_rTotal * info.c2);
                    break;

                case discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M | discrete_global.DISC_OP_AMP_IS_NORTON:
                    if (info.r2 == 0)
                        m_rTotal = info.r1;
                    else
                        m_rTotal = rescap_global.RES_2_PARALLEL(info.r1, info.r2);

                    // fall through to below
                    goto case discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M;

                case discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M:
                {
                    double fc = 1.0 / (2 * Math.PI * Math.Sqrt(m_rTotal * info.rF * info.c1 * info.c2));
                    double d  = (info.c1 + info.c2) / Math.Sqrt(info.rF / m_rTotal * info.c1 * info.c2);
                    double gain = -info.rF / m_rTotal * info.c2 / (info.c1 + info.c2);

                    disc_flt_global.calculate_filter2_coefficients(this, fc, d, discrete_global.DISC_FILTER_BANDPASS, ref m_fc);
                    m_fc.b0 *= gain;
                    m_fc.b1 *= gain;
                    m_fc.b2 *= gain;

                    if (m_is_norton != 0)
                        m_vRef = (info.vP - discrete_global.OP_AMP_NORTON_VBE) / info.r3 * info.rF;
                    else
                        m_vRef = info.vRef;

                    break;
                }

                case discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_0 | discrete_global.DISC_OP_AMP_IS_NORTON:
                    m_exponentC1 = RC_CHARGE_EXP(rescap_global.RES_2_PARALLEL(info.r1, info.r2 + info.r3 + info.r4) * info.c1);
                    m_exponentC2 = RC_CHARGE_EXP(rescap_global.RES_2_PARALLEL(info.r1 + info.r2, info.r3 + info.r4) * info.c2);
                    m_exponentC3 = RC_CHARGE_EXP((info.r1 + info.r2 + info.r3 + info.r4) * info.c3);
                    break;

                case discrete_global.DISC_OP_AMP_FILTER_IS_HIGH_PASS_0 | discrete_global.DISC_OP_AMP_IS_NORTON:
                    m_exponentC1 = RC_CHARGE_EXP(info.r1 * info.c1);
                    break;
            }

            /* At startup there is no charge on the caps and output is 0V in relation to vRef. */
            m_vC1 = 0;
            m_vC1b = 0;
            m_vC2 = 0;
            m_vC3 = 0;

            set_output(0, info.vRef);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set;  }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_op_amp_filt)
        public void step()
        {
            //DISCRETE_DECLARE_INFO(discrete_op_amp_filt_info)
            discrete_op_amp_filt_info info = (discrete_op_amp_filt_info)custom_data();


            double v_out = 0;

            double i;
            double v = 0;

            if (DST_OP_AMP_FILT__ENABLE() != 0)
            {
                if (m_is_norton != 0)
                {
                    v = DST_OP_AMP_FILT__INP1() - discrete_global.OP_AMP_NORTON_VBE;
                    if (v < 0) v = 0;
                }
                else
                {
                    /* Millman the input voltages. */
                    i  = m_iFixed;
                    switch (m_type)
                    {
                        case discrete_global.DISC_OP_AMP_FILTER_IS_LOW_PASS_1_A:
                            i += (DST_OP_AMP_FILT__INP1() - DST_OP_AMP_FILT__INP2()) / info.r1;
                            if (info.r2 != 0)
                                i += (m_vP - DST_OP_AMP_FILT__INP2()) / info.r2;
                            if (info.r3 != 0)
                                i += (m_vN - DST_OP_AMP_FILT__INP2()) / info.r3;
                            break;

                        default:
                            i += (DST_OP_AMP_FILT__INP1() - m_vRef) / info.r1;
                            if (info.r2 != 0)
                                i += (DST_OP_AMP_FILT__INP2() - m_vRef) / info.r2;
                            break;
                    }
                    v = i * m_rTotal;
                }

                switch (m_type)
                {
                    case discrete_global.DISC_OP_AMP_FILTER_IS_LOW_PASS_1:
                        m_vC1 += (v - m_vC1) * m_exponentC1;
                        v_out = m_vC1 * m_gain + info.vRef;
                        break;

                    case discrete_global.DISC_OP_AMP_FILTER_IS_LOW_PASS_1_A:
                        m_vC1 += (v - m_vC1) * m_exponentC1;
                        v_out = m_vC1 * m_gain + DST_OP_AMP_FILT__INP2();
                        break;

                    case discrete_global.DISC_OP_AMP_FILTER_IS_HIGH_PASS_1:
                        v_out = (v - m_vC1) * m_gain + info.vRef;
                        m_vC1 += (v - m_vC1) * m_exponentC1;
                        break;

                    case discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1:
                        v_out = (v - m_vC2);
                        m_vC2 += (v - m_vC2) * m_exponentC2;
                        m_vC1 += (v_out - m_vC1) * m_exponentC1;
                        v_out = m_vC1 * m_gain + info.vRef;
                        break;

                    case discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_0 | discrete_global.DISC_OP_AMP_IS_NORTON:
                        m_vC1 += (v - m_vC1) * m_exponentC1;
                        m_vC2 += (m_vC1 - m_vC2) * m_exponentC2;
                        v = m_vC2;
                        v_out = v - m_vC3;
                        m_vC3 += (v - m_vC3) * m_exponentC3;
                        i = v_out / m_rTotal;
                        v_out = (m_iFixed - i) * info.rF;
                        break;

                    case discrete_global.DISC_OP_AMP_FILTER_IS_HIGH_PASS_0 | discrete_global.DISC_OP_AMP_IS_NORTON:
                        v_out = v - m_vC1;
                        m_vC1 += (v - m_vC1) * m_exponentC1;
                        i = v_out / m_rTotal;
                        v_out = (m_iFixed - i) * info.rF;
                        break;

                    case discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M:
                    case discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M | discrete_global.DISC_OP_AMP_IS_NORTON:
                        v_out = -m_fc.a1 * m_fc.y1 - m_fc.a2 * m_fc.y2 +
                                        m_fc.b0 * v + m_fc.b1 * m_fc.x1 + m_fc.b2 * m_fc.x2 +
                                        m_vRef;
                        m_fc.x2 = m_fc.x1;
                        m_fc.x1 = v;
                        m_fc.y2 = m_fc.y1;
                        break;
                }

                /* Clip the output to the voltage rails.
                 * This way we get the original distortion in all it's glory.
                 */
                if (v_out > m_vP) v_out = m_vP;
                if (v_out < m_vN) v_out = m_vN;
                m_fc.y1 = v_out - m_vRef;
                set_output(0, v_out);
            }
            else
            {
                set_output(0, 0);
            }
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_rcdisc5, 1,
    class discrete_dst_rcdisc5_node : discrete_base_node,
                                      discrete_step_interface
    {
        const int _maxout = 1;


        int             m_state;
        double          m_t;                    /* time */
        double          m_exponent0;
        double          m_v_cap;                /* rcdisc5 */


        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_rcdisc5_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        ~discrete_dst_rcdisc5_node() { }


        //DISCRETE_STEP( dst_rcdisc5)
        public void step()
        {
            throw new emu_unimplemented();
        }


        //DISCRETE_RESET( dst_rcdisc5)
        public override void reset()
        {
            throw new emu_unimplemented();
        }


        protected override int max_output() { return _maxout; }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_rcfilter, 1,
    class discrete_dst_rcfilter_node : discrete_base_node,
                                       discrete_step_interface
    {
        const int _maxout = 1;


        double          m_v_out;
        double          m_vCap;
        double          m_rc;
        double          m_exponent;
        //uint8_t           m_has_rc_nodes;
        //uint8_t           m_is_fast;


        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_rcfilter_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        ~discrete_dst_rcfilter_node() { }


        //DISCRETE_STEP(dst_rcfilter)
        public void step()
        {
            throw new emu_unimplemented();
        }


        //DISCRETE_RESET(dst_rcfilter)
        public override void reset()
        {
            throw new emu_unimplemented();
        }


        protected override int max_output() { return _maxout; }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_rcfilter_sw, 1,
    class discrete_dst_rcfilter_sw_node : discrete_base_node,
                                          discrete_step_interface
    {
        const int _maxout = 1;


        /************************************************************************
         *
         * DST_RCFILTER_SW - Usage of node_description values for switchable RC filter
         *
         * input[0]    - Enable input value
         * input[1]    - input value
         * input[2]    - Resistor value (initialization only)
         * input[3]    - Capacitor Value (initialization only)
         * input[4]    - Voltage reference. Usually 0V.
         *
         ************************************************************************/
        double DST_RCFILTER_SW__ENABLE { get { return DISCRETE_INPUT(0); } }
        double DST_RCFILTER_SW__VIN { get { return DISCRETE_INPUT(1); } }
        double DST_RCFILTER_SW__SWITCH { get { return DISCRETE_INPUT(2); } }
        double DST_RCFILTER_SW__R { get { return DISCRETE_INPUT(3); } }
        double DST_RCFILTER_SW__C(int x) { return DISCRETE_INPUT(4+x); }


        /* 74HC4066 : 15
         * 74VHC4066 : 15
         * UTC4066 : 270 @ 5VCC, 80 @ 15VCC
         * CD4066BC : 270 (Fairchild)
         *
         * The choice below makes scramble sound about "right". For future error reports,
         * we need the exact type of switch and at which voltage (5, 12?) it is operated.
         */
        const int CD4066_ON_RES = 40;


        double [] m_vCap = new double[4];
        double [] m_exp = new double[4];
        double m_exp0;                 /* fast case bit 0 */
        double m_exp1;                 /* fast case bit 1 */
        double m_factor;               /* fast case */
        double [] m_f1 = new double[16];
        double [] m_f2 = new double[16];


        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_rcfilter_sw_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        ~discrete_dst_rcfilter_sw_node() { }


        //DISCRETE_STEP(dst_rcfilter_sw)
        public void step()
        {
            throw new emu_unimplemented();
        }


        //DISCRETE_RESET(dst_rcfilter_sw)
        public override void reset()
        {
            int i;
            int bits;

            for (i = 0; i < 4; i++)
            {
                m_vCap[i] = 0;
                m_exp[i] = RC_CHARGE_EXP( CD4066_ON_RES * DST_RCFILTER_SW__C(i));
            }

            for (bits=0; bits < 15; bits++)
            {
                double rs = 0;

                for (i = 0; i < 4; i++)
                {
                    if (( bits & (1 << i)) != 0)
                        rs += DST_RCFILTER_SW__R;
                }

                m_f1[bits] = rescap_global.RES_VOLTAGE_DIVIDER(rs, CD4066_ON_RES);
                m_f2[bits] = DST_RCFILTER_SW__R / (CD4066_ON_RES + rs);
            }


            /* fast cases */
            m_exp0 = RC_CHARGE_EXP((CD4066_ON_RES + DST_RCFILTER_SW__R) * DST_RCFILTER_SW__C(0));
            m_exp1 = RC_CHARGE_EXP((CD4066_ON_RES + DST_RCFILTER_SW__R) * DST_RCFILTER_SW__C(1));
            m_factor = rescap_global.RES_VOLTAGE_DIVIDER(DST_RCFILTER_SW__R, CD4066_ON_RES);

            set_output(0,  0);
        }


        protected override int max_output() { return _maxout; }
    }
}
