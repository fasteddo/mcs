// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;
using uint8_t = System.Byte;


namespace mame
{
    static class disc_flt_global
    {
        public static void calculate_filter2_coefficients(discrete_base_node node, double fc, double d, double type, ref discrete_filter_coeff coeff)
        {
            double wc;   /* cutoff freq, in radians/sec */
            double wc_squared;
            double den; /* temp variable */
            double two_over_T = 2 * node.sample_rate();
            double two_over_T_squared = two_over_T * two_over_T;

            /* calculate digital filter coefficents */
            /*wc = 2.0*M_PI*fc; no pre-warping */
            wc = node.sample_rate() * 2.0 * Math.Tan(Math.PI * fc / node.sample_rate()); /* pre-warping */
            wc_squared = wc * wc;

            den = two_over_T_squared + d*wc*two_over_T + wc_squared;

            coeff.a1 = 2.0 * (-two_over_T_squared + wc_squared) / den;
            coeff.a2 = (two_over_T_squared - d * wc * two_over_T + wc_squared) / den;

            if (type == global_object.DISC_FILTER_LOWPASS)
            {
                coeff.b0 = coeff.b2 = wc_squared/den;
                coeff.b1 = 2.0 * (coeff.b0);
            }
            else if (type == global_object.DISC_FILTER_BANDPASS)
            {
                coeff.b0 = d * wc * two_over_T / den;
                coeff.b1 = 0.0;
                coeff.b2 = -(coeff.b0);
            }
            else if (type == global_object.DISC_FILTER_HIGHPASS)
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


    public struct discrete_filter_coeff
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


    //DISCRETE_CLASS_STEP_RESET(dst_sallen_key, 1,
    class discrete_dst_sallen_key_node : discrete_base_node,
                                         discrete_step_interface
    {
        const int _maxout = 1;


        double DST_SALLEN_KEY__ENABLE { get { return DISCRETE_INPUT(0); } }
        double DST_SALLEN_KEY__INP0 { get { return DISCRETE_INPUT(1); } }
        double DST_SALLEN_KEY__TYPE { get { return DISCRETE_INPUT(2); } }


        discrete_filter_coeff m_fc;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)
        public discrete_dst_sallen_key_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)
        //~discrete_dst_sallen_key_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_sallen_key)
        public override void reset()
        {
            discrete_op_amp_filt_info info = (discrete_op_amp_filt_info)custom_data();  //DISCRETE_DECLARE_INFO(discrete_op_amp_filt_info);

            double freq = 0;
            double q = 0;

            switch ((int)DST_SALLEN_KEY__TYPE)
            {
                case DISC_SALLEN_KEY_LOW_PASS:
                    freq = 1.0 / ( 2.0 * Math.PI * Math.Sqrt(info.c1 * info.c2 * info.r1 * info.r2));
                    q = Math.Sqrt(info.c1 * info.c2 * info.r1 * info.r2) / (info.c2 * (info.r1 + info.r2));
                    break;
                default:
                    fatalerror("Unknown sallen key filter type\n");
                    break;
            }

            calculate_filter2_coefficients(this, freq, 1.0 / q, DISC_FILTER_LOWPASS, ref m_fc);
            set_output(0,  0);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_sallen_key)
        public void step()
        {
            double gain = 1.0;
            double v_out;

            if (DST_SALLEN_KEY__ENABLE == 0.0)
            {
                gain = 0.0;
            }

            v_out = -m_fc.a1 * m_fc.y1 - m_fc.a2 * m_fc.y2 +
                            m_fc.b0 * gain * DST_SALLEN_KEY__INP0 + m_fc.b1 * m_fc.x1 + m_fc.b2 * m_fc.x2;

            m_fc.x2 = m_fc.x1;
            m_fc.x1 = gain * DST_SALLEN_KEY__INP0;
            m_fc.y2 = m_fc.y1;
            m_fc.y1 = v_out;
            set_output(0, v_out);
        }
    }



    //DISCRETE_CLASS_STEP_RESET(dst_crfilter, 1,
    class discrete_dst_crfilter_node : discrete_base_node,
                                       discrete_step_interface
    {
        const int _maxout = 1;


        double DST_CRFILTER__IN { get { return DISCRETE_INPUT(0); } }
        double DST_CRFILTER__R { get { return DISCRETE_INPUT(1); } }
        double DST_CRFILTER__C { get { return DISCRETE_INPUT(2); } }
        double DST_CRFILTER__VREF { get { return DISCRETE_INPUT(3); } }


        double m_vCap;
        double m_rc;
        double m_exponent;
        uint8_t m_has_rc_nodes;
        //UINT8           m_is_fast;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)
        public discrete_dst_crfilter_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)
        //~discrete_dst_crfilter_node() { }


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
            if (m_has_rc_nodes != 0)
            {
                double rc = DST_CRFILTER__R * DST_CRFILTER__C;
                if (rc != m_rc)
                {
                    m_rc = rc;
                    m_exponent = RC_CHARGE_EXP(rc);
                }
            }

            double v_out = DST_CRFILTER__IN - m_vCap;
            double v_diff = v_out - DST_CRFILTER__VREF;
            set_output(0,  v_out);
            m_vCap += v_diff * m_exponent;
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
        //~discrete_dst_op_amp_filt_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_op_amp_filt)
        public override void reset()
        {
            //DISCRETE_DECLARE_INFO(discrete_op_amp_filt_info)
            discrete_op_amp_filt_info info = (discrete_op_amp_filt_info)custom_data();

            /* Convert the passed filter type into an int for easy use. */
            m_type = (int)DST_OP_AMP_FILT__TYPE() & DISC_OP_AMP_FILTER_TYPE_MASK;
            m_is_norton = (int)DST_OP_AMP_FILT__TYPE() & DISC_OP_AMP_IS_NORTON;

            if (m_is_norton != 0)
            {
                m_vRef = 0;
                m_rTotal = info.r1;
                if (m_type == (DISC_OP_AMP_FILTER_IS_BAND_PASS_0 | DISC_OP_AMP_IS_NORTON))
                    m_rTotal += info.r2 +  info.r3;

                /* Setup the current to the + input. */
                m_iFixed = (info.vP - OP_AMP_NORTON_VBE) / info.r4;

                /* Set the output max. */
                m_vP =  info.vP - OP_AMP_NORTON_VBE;
                m_vN =  info.vN;
            }
            else
            {
                m_vRef = info.vRef;
                /* Set the output max. */
                m_vP =  info.vP - OP_AMP_VP_RAIL_OFFSET;
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
                case DISC_OP_AMP_FILTER_IS_LOW_PASS_1:
                case DISC_OP_AMP_FILTER_IS_LOW_PASS_1_A:
                    m_exponentC1 = RC_CHARGE_EXP(info.rF * info.c1);
                    m_exponentC2 =  0;
                    break;

                case DISC_OP_AMP_FILTER_IS_HIGH_PASS_1:
                    m_exponentC1 = RC_CHARGE_EXP(m_rTotal * info.c1);
                    m_exponentC2 =  0;
                    break;

                case DISC_OP_AMP_FILTER_IS_BAND_PASS_1:
                    m_exponentC1 = RC_CHARGE_EXP(info.rF * info.c1);
                    m_exponentC2 = RC_CHARGE_EXP(m_rTotal * info.c2);
                    break;

                case DISC_OP_AMP_FILTER_IS_BAND_PASS_1M | DISC_OP_AMP_IS_NORTON:
                    if (info.r2 == 0)
                        m_rTotal = info.r1;
                    else
                        m_rTotal = RES_2_PARALLEL(info.r1, info.r2);

                    goto case DISC_OP_AMP_FILTER_IS_BAND_PASS_1M;  //[[fallthrough]];

                case DISC_OP_AMP_FILTER_IS_BAND_PASS_1M:
                {
                    double fc = 1.0 / (2 * Math.PI * Math.Sqrt(m_rTotal * info.rF * info.c1 * info.c2));
                    double d  = (info.c1 + info.c2) / Math.Sqrt(info.rF / m_rTotal * info.c1 * info.c2);
                    double gain = -info.rF / m_rTotal * info.c2 / (info.c1 + info.c2);

                    calculate_filter2_coefficients(this, fc, d, DISC_FILTER_BANDPASS, ref m_fc);
                    m_fc.b0 *= gain;
                    m_fc.b1 *= gain;
                    m_fc.b2 *= gain;

                    if (m_is_norton != 0)
                        m_vRef = (info.vP - OP_AMP_NORTON_VBE) / info.r3 * info.rF;
                    else
                        m_vRef = info.vRef;

                    break;
                }

                case DISC_OP_AMP_FILTER_IS_BAND_PASS_0 | DISC_OP_AMP_IS_NORTON:
                    m_exponentC1 = RC_CHARGE_EXP(RES_2_PARALLEL(info.r1, info.r2 + info.r3 + info.r4) * info.c1);
                    m_exponentC2 = RC_CHARGE_EXP(RES_2_PARALLEL(info.r1 + info.r2, info.r3 + info.r4) * info.c2);
                    m_exponentC3 = RC_CHARGE_EXP((info.r1 + info.r2 + info.r3 + info.r4) * info.c3);
                    break;

                case DISC_OP_AMP_FILTER_IS_HIGH_PASS_0 | DISC_OP_AMP_IS_NORTON:
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
                    v = DST_OP_AMP_FILT__INP1() - OP_AMP_NORTON_VBE;
                    if (v < 0) v = 0;
                }
                else
                {
                    /* Millman the input voltages. */
                    i  = m_iFixed;
                    switch (m_type)
                    {
                        case DISC_OP_AMP_FILTER_IS_LOW_PASS_1_A:
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
                    case DISC_OP_AMP_FILTER_IS_LOW_PASS_1:
                        m_vC1 += (v - m_vC1) * m_exponentC1;
                        v_out = m_vC1 * m_gain + info.vRef;
                        break;

                    case DISC_OP_AMP_FILTER_IS_LOW_PASS_1_A:
                        m_vC1 += (v - m_vC1) * m_exponentC1;
                        v_out = m_vC1 * m_gain + DST_OP_AMP_FILT__INP2();
                        break;

                    case DISC_OP_AMP_FILTER_IS_HIGH_PASS_1:
                        v_out = (v - m_vC1) * m_gain + info.vRef;
                        m_vC1 += (v - m_vC1) * m_exponentC1;
                        break;

                    case DISC_OP_AMP_FILTER_IS_BAND_PASS_1:
                        v_out = (v - m_vC2);
                        m_vC2 += (v - m_vC2) * m_exponentC2;
                        m_vC1 += (v_out - m_vC1) * m_exponentC1;
                        v_out = m_vC1 * m_gain + info.vRef;
                        break;

                    case DISC_OP_AMP_FILTER_IS_BAND_PASS_0 | DISC_OP_AMP_IS_NORTON:
                        m_vC1 += (v - m_vC1) * m_exponentC1;
                        m_vC2 += (m_vC1 - m_vC2) * m_exponentC2;
                        v = m_vC2;
                        v_out = v - m_vC3;
                        m_vC3 += (v - m_vC3) * m_exponentC3;
                        i = v_out / m_rTotal;
                        v_out = (m_iFixed - i) * info.rF;
                        break;

                    case DISC_OP_AMP_FILTER_IS_HIGH_PASS_0 | DISC_OP_AMP_IS_NORTON:
                        v_out = v - m_vC1;
                        m_vC1 += (v - m_vC1) * m_exponentC1;
                        i = v_out / m_rTotal;
                        v_out = (m_iFixed - i) * info.rF;
                        break;

                    case DISC_OP_AMP_FILTER_IS_BAND_PASS_1M:
                    case DISC_OP_AMP_FILTER_IS_BAND_PASS_1M | DISC_OP_AMP_IS_NORTON:
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


    //DISCRETE_CLASS_STEP_RESET(dst_rcdisc, 1,
    class discrete_dst_rcdisc_node : discrete_base_node,
                                     discrete_step_interface
    {
        const int _maxout = 1;


        double DST_RCDISC__ENABLE { get { return DISCRETE_INPUT(0); } }
        double DST_RCDISC__IN { get { return DISCRETE_INPUT(1); } }
        double DST_RCDISC__R { get { return DISCRETE_INPUT(2); } }
        double DST_RCDISC__C { get { return DISCRETE_INPUT(3); } }


        int             m_state;
        double          m_t;                    /* time */
        double          m_exponent0;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_rcdisc_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_rcdisc_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_rcdisc)
        public override void reset()
        {
            set_output(0,  0);

            m_state = 0;
            m_t = 0;
            m_exponent0 = -1.0 * DST_RCDISC__R * DST_RCDISC__C;
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_rcdisc)
        public void step()
        {
            switch (m_state)
            {
                case 0:     /* waiting for trigger  */
                    if (DST_RCDISC__ENABLE != 0)
                    {
                        m_state = 1;
                        m_t = 0;
                    }
                    set_output(0,  0);
                    break;

                case 1:
                    if (DST_RCDISC__ENABLE != 0)
                    {
                        set_output(0,  DST_RCDISC__IN * Math.Exp(m_t / m_exponent0));
                        m_t += this.sample_time();
                    }
                    else
                    {
                        m_state = 0;
                    }
                    break;
            }
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_rcdisc2, 1,
    class discrete_dst_rcdisc2_node : discrete_base_node,
                                      discrete_step_interface
    {
        const int _maxout = 1;


        double DST_RCDISC2__ENABLE { get { return DISCRETE_INPUT(0); } }
        double DST_RCDISC2__IN0 { get { return DISCRETE_INPUT(1); } }
        double DST_RCDISC2__R0 { get { return DISCRETE_INPUT(2); } }
        double DST_RCDISC2__IN1 { get { return DISCRETE_INPUT(3); } }
        double DST_RCDISC2__R1 { get { return DISCRETE_INPUT(4); } }
        double DST_RCDISC2__C { get { return DISCRETE_INPUT(5); } }


        int             m_state;
        double          m_v_out;
        double          m_t;                    /* time */
        double          m_exponent0;
        double          m_exponent1;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_rcdisc2_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_rcdisc2_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_rcdisc2)
        public override void reset()
        {
            m_v_out = 0;

            m_state = 0;
            m_t = 0;
            m_exponent0 = RC_DISCHARGE_EXP(DST_RCDISC2__R0 * DST_RCDISC2__C);
            m_exponent1 = RC_DISCHARGE_EXP(DST_RCDISC2__R1 * DST_RCDISC2__C);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_rcdisc2)
        public void step()
        {
            double diff;

            /* Works differently to other as we are always on, no enable */
            /* exponential based in difference between input/output   */

            diff = ((DST_RCDISC2__ENABLE == 0) ? DST_RCDISC2__IN0 : DST_RCDISC2__IN1) - m_v_out;
            diff = diff - (diff * ((DST_RCDISC2__ENABLE == 0) ? m_exponent0 : m_exponent1));
            m_v_out += diff;
            set_output(0, m_v_out);
        }
    }


#if false
    DISCRETE_CLASS_STEP_RESET(dst_rcdisc3, 1,
        int             m_state;
        double          m_v_out;
        double          m_t;                    /* time */
        double          m_exponent0;
        double          m_exponent1;
        double          m_v_diode;              /* rcdisc3 */
    );
#endif

#if false
    DISCRETE_CLASS_STEP_RESET(dst_rcdisc4, 1,
        int             m_type;
        double          m_max_out;
        double          m_vC1;
        double          m_v[2];
        double          m_exp[2];
    );
#endif


    //DISCRETE_CLASS_STEP_RESET(dst_rcdisc5, 1,
    class discrete_dst_rcdisc5_node : discrete_base_node,
                                      discrete_step_interface
    {
        const int _maxout = 1;


        double DST_RCDISC5__ENABLE { get { return DISCRETE_INPUT(0); } }
        double DST_RCDISC5__IN { get { return DISCRETE_INPUT(1); } }
        double DST_RCDISC5__R { get { return DISCRETE_INPUT(2); } }
        double DST_RCDISC5__C { get { return DISCRETE_INPUT(3); } }


        int             m_state;
        double          m_t;                    /* time */
        double          m_exponent0;
        double          m_v_cap;                /* rcdisc5 */


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_rcdisc5_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_rcdisc5_node() { }


        // discrete_base_node

        //DISCRETE_RESET( dst_rcdisc5)
        public override void reset()
        {
            set_output(0,  0);

            m_state = 0;
            m_t = 0;
            m_v_cap = 0;
            m_exponent0 = RC_CHARGE_EXP(DST_RCDISC5__R * DST_RCDISC5__C);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP( dst_rcdisc5)
        public void step()
        {
            double diff;
            double u;

            /* Exponential based in difference between input/output   */

            u = DST_RCDISC5__IN - 0.7; /* Diode drop */
            if (u < 0)
                u = 0;

            diff = u - m_v_cap;

            if (DST_RCDISC5__ENABLE != 0)
            {
                if (diff < 0)
                    diff = diff * m_exponent0;

                m_v_cap += diff;
                set_output(0,  m_v_cap);
            }
            else
            {
                if (diff > 0)
                    m_v_cap = u;

                set_output(0, 0);
            }
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_rcintegrate, 1,
    class discrete_dst_rcintegrate_node : discrete_base_node,
                                          discrete_step_interface
    {
        const int _maxout = 1;


        double DST_RCINTEGRATE__IN1 { get { return DISCRETE_INPUT(0); } }
        double DST_RCINTEGRATE__R1 { get { return DISCRETE_INPUT(1); } }
        double DST_RCINTEGRATE__R2 { get { return DISCRETE_INPUT(2); } }
        double DST_RCINTEGRATE__R3 { get { return DISCRETE_INPUT(3); } }
        double DST_RCINTEGRATE__C { get { return DISCRETE_INPUT(4); } }
        double DST_RCINTEGRATE__VP { get { return DISCRETE_INPUT(5); } }
        double DST_RCINTEGRATE__TYPE { get { return DISCRETE_INPUT(6); } }

        /* reverse saturation current */
        const double IES     = 7e-15;
        const double ALPHAT  = 0.99;
        const double KT      = 0.026;
        static double EM_IC(double x) { return ALPHAT * IES * Math.Exp(x / KT - 1.0); }


        int             m_type;
        double          m_gain_r1_r2;
        double          m_f;                    /* r2,r3 gain */
        double          m_vCap;
        double          m_vCE;
        double          m_exponent0;
        double          m_exponent1;
        double          m_exp_exponent0;
        double          m_exp_exponent1;
        double          m_c_exp0;
        double          m_c_exp1;
        double          m_EM_IC_0_7;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_rcintegrate_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_rcintegrate_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_rcintegrate)
        public override void reset()
        {
            double r;
            double dt = this.sample_time();

            m_type = (int)DST_RCINTEGRATE__TYPE;

            m_vCap = 0;
            m_vCE  = 0;

            /* pre-calculate fixed values */
            m_gain_r1_r2 = RES_VOLTAGE_DIVIDER(DST_RCINTEGRATE__R1, DST_RCINTEGRATE__R2);

            r = DST_RCINTEGRATE__R1 / DST_RCINTEGRATE__R2 * DST_RCINTEGRATE__R3 + DST_RCINTEGRATE__R1 + DST_RCINTEGRATE__R3;

            m_f = RES_VOLTAGE_DIVIDER(DST_RCINTEGRATE__R3, DST_RCINTEGRATE__R2);
            m_exponent0 = -1.0 * r * m_f * DST_RCINTEGRATE__C;
            m_exponent1 = -1.0 * (DST_RCINTEGRATE__R1 + DST_RCINTEGRATE__R2) * DST_RCINTEGRATE__C;
            m_exp_exponent0 = Math.Exp(dt / m_exponent0);
            m_exp_exponent1 = Math.Exp(dt / m_exponent1);
            m_c_exp0 =  DST_RCINTEGRATE__C / m_exponent0 * m_exp_exponent0;
            m_c_exp1 =  DST_RCINTEGRATE__C / m_exponent1 * m_exp_exponent1;

            m_EM_IC_0_7 = EM_IC(0.7);

            set_output(0,  0);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP( dst_rcintegrate)
        public void step()
        {
            double diff;
            double u;
            double iQ;
            double iQc;
            double iC;
            double RG;
            double vE;
            double vP;

            u  = DST_RCINTEGRATE__IN1;
            vP = DST_RCINTEGRATE__VP;

            if ( u - 0.7  < m_vCap * m_gain_r1_r2)
            {
                /* discharge .... */
                diff  = 0.0 - m_vCap;
                iC    = m_c_exp1 * diff; /* iC */
                diff -= diff * m_exp_exponent1;
                m_vCap += diff;
                iQ = 0;
                vE = m_vCap * m_gain_r1_r2;
                RG = vE / iC;
            }
            else
            {
                /* charging */
                diff  = (vP - m_vCE) * m_f - m_vCap;
                iC    = 0.0 - m_c_exp0 * diff; /* iC */
                diff -= diff * m_exp_exponent0;
                m_vCap += diff;
                iQ = iC + (iC * DST_RCINTEGRATE__R1 + m_vCap) / DST_RCINTEGRATE__R2;
                RG = (vP - m_vCE) / iQ;
                vE = (RG - DST_RCINTEGRATE__R3) / RG * (vP - m_vCE);
            }


            u = DST_RCINTEGRATE__IN1;
            if (u > 0.7 + vE)
            {
                vE = u - 0.7;
                //iQc = EM_IC(u - vE);
                iQc = m_EM_IC_0_7;
            }
            else
            {
                iQc = EM_IC(u - vE);
            }

            m_vCE = Math.Min(vP - 0.1, vP - RG * iQc);

            /* Avoid oscillations
             * The method tends to largely overshoot - no wonder without
             * iterative solution approximation
             */

            m_vCE = Math.Max(m_vCE, 0.1 );
            m_vCE = 0.1 * m_vCE + 0.9 * (vP - vE - iQ * DST_RCINTEGRATE__R3);

            switch (m_type)
            {
                case DISC_RC_INTEGRATE_TYPE1:
                    set_output(0,  m_vCap);
                    break;
                case DISC_RC_INTEGRATE_TYPE2:
                    set_output(0,  vE);
                    break;
                case DISC_RC_INTEGRATE_TYPE3:
                    set_output(0, Math.Max(0.0, vP - iQ * DST_RCINTEGRATE__R3));
                    break;
            }
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_rcdisc_mod, 1,
    class discrete_dst_rcdisc_mod_node : discrete_base_node,
                                         discrete_step_interface
    {
        const int _maxout = 1;


        double DST_RCDISC_MOD__IN1 { get { return DISCRETE_INPUT(0); } }
        double DST_RCDISC_MOD__IN2 { get { return DISCRETE_INPUT(1); } }
        double DST_RCDISC_MOD__R1 { get { return DISCRETE_INPUT(2); } }
        double DST_RCDISC_MOD__R2 { get { return DISCRETE_INPUT(3); } }
        double DST_RCDISC_MOD__R3 { get { return DISCRETE_INPUT(4); } }
        double DST_RCDISC_MOD__R4 { get { return DISCRETE_INPUT(5); } }
        double DST_RCDISC_MOD__C { get { return DISCRETE_INPUT(6); } }
        double DST_RCDISC_MOD__VP { get { return DISCRETE_INPUT(7); } }


        double m_v_cap;
        double [] m_exp_low = new double [2];
        double [] m_exp_high = new double [4];
        double [] m_gain = new double [2];
        double [] m_vd_gain = new double [4];


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_rcdisc_mod_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_rcdisc_mod_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_rcdisc_mod)
        public override void reset()
        {
            double [] rc = new double [2];
            double [] rc2 = new double [2];

            /* pre-calculate fixed values */
            /* DST_RCDISC_MOD__IN1 <= 0.5 */
            rc[0] = DST_RCDISC_MOD__R1 + DST_RCDISC_MOD__R2;
            if (rc[0] < 1) rc[0] = 1;
            m_exp_low[0]  = RC_DISCHARGE_EXP(DST_RCDISC_MOD__C * rc[0]);
            m_gain[0]     = RES_VOLTAGE_DIVIDER(rc[0], DST_RCDISC_MOD__R4);
            /* DST_RCDISC_MOD__IN1 > 0.5 */
            rc[1] = DST_RCDISC_MOD__R2;
            if (rc[1] < 1) rc[1] = 1;
            m_exp_low[1]  = RC_DISCHARGE_EXP(DST_RCDISC_MOD__C * rc[1]);
            m_gain[1]     = RES_VOLTAGE_DIVIDER(rc[1], DST_RCDISC_MOD__R4);
            /* DST_RCDISC_MOD__IN2 <= 0.6 */
            rc2[0] = DST_RCDISC_MOD__R4;
            /* DST_RCDISC_MOD__IN2 > 0.6 */
            rc2[1] = RES_2_PARALLEL(DST_RCDISC_MOD__R3, DST_RCDISC_MOD__R4);
            /* DST_RCDISC_MOD__IN1 <= 0.5 && DST_RCDISC_MOD__IN2 <= 0.6 */
            m_exp_high[0] = RC_DISCHARGE_EXP(DST_RCDISC_MOD__C * (rc[0] + rc2[0]));
            m_vd_gain[0]  = RES_VOLTAGE_DIVIDER(rc[0], rc2[0]);
            /* DST_RCDISC_MOD__IN1 > 0.5  && DST_RCDISC_MOD__IN2 <= 0.6 */
            m_exp_high[1] = RC_DISCHARGE_EXP(DST_RCDISC_MOD__C * (rc[1] + rc2[0]));
            m_vd_gain[1]  = RES_VOLTAGE_DIVIDER(rc[1], rc2[0]);
            /* DST_RCDISC_MOD__IN1 <= 0.5 && DST_RCDISC_MOD__IN2 > 0.6 */
            m_exp_high[2] = RC_DISCHARGE_EXP(DST_RCDISC_MOD__C * (rc[0] + rc2[1]));
            m_vd_gain[2]  = RES_VOLTAGE_DIVIDER(rc[0], rc2[1]);
            /* DST_RCDISC_MOD__IN1 > 0.5  && DST_RCDISC_MOD__IN2 > 0.6 */
            m_exp_high[3] = RC_DISCHARGE_EXP(DST_RCDISC_MOD__C * (rc[1] + rc2[1]));
            m_vd_gain[3]  = RES_VOLTAGE_DIVIDER(rc[1], rc2[1]);

            m_v_cap  = 0;
            set_output(0,  0);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_rcdisc_mod)
        public void step()
        {
            double  diff;
            double  v_cap;
            double  u;
            double  vD;
            int     mod_state;
            int     mod1_state;
            int     mod2_state;

            /* Exponential based in difference between input/output   */
            v_cap = m_v_cap;

            mod1_state = DST_RCDISC_MOD__IN1 > 0.5 ? 1 : 0;
            mod2_state = DST_RCDISC_MOD__IN2 > 0.6 ? 1 : 0;
            mod_state  = (mod2_state << 1) + mod1_state;

            u = mod1_state != 0 ? 0 : DST_RCDISC_MOD__VP;
            /* Clamp */
            diff = u - v_cap;
            vD = diff * m_vd_gain[mod_state];
            if (vD < -0.6)
            {
                diff  = u + 0.6 - v_cap;
                diff -= diff * m_exp_low[mod1_state];
                v_cap += diff;
                set_output(0,  mod2_state != 0 ? 0 : -0.6);
            }
            else
            {
                diff  -= diff * m_exp_high[mod_state];
                v_cap += diff;
                /* neglecting current through R3 drawn by next8 node */
                set_output(0,  mod2_state != 0 ? 0: (u - v_cap) * m_gain[mod1_state]);
            }
            m_v_cap = v_cap;
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_rcfilter, 1,
    class discrete_dst_rcfilter_node : discrete_base_node,
                                       discrete_step_interface
    {
        const int _maxout = 1;


        double DST_RCFILTER__VIN { get { return DISCRETE_INPUT(0); } }
        double DST_RCFILTER__R { get { return DISCRETE_INPUT(1); } }
        double DST_RCFILTER__C { get { return DISCRETE_INPUT(2); } }
        double DST_RCFILTER__VREF { get { return DISCRETE_INPUT(3); } }


        double          m_v_out;
        double          m_vCap;
        double          m_rc;
        double          m_exponent;
        uint8_t         m_has_rc_nodes;
        uint8_t         m_is_fast;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_rcfilter_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_rcfilter_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_rcfilter)
        public override void reset()
        {
            m_has_rc_nodes = (uint8_t)(this.input_is_node() & 0x6);
            m_rc = DST_RCFILTER__R * DST_RCFILTER__C;
            m_exponent = RC_CHARGE_EXP(m_rc);
            m_vCap   = 0;
            m_v_out = 0;
            /* FIXME --> we really need another class here */
            if (!(m_has_rc_nodes != 0) && DST_RCFILTER__VREF == 0)
                m_is_fast = 1;
            else
                m_is_fast = 0;
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_rcfilter)
        public void step()
        {
            if (m_is_fast != 0)
            {
                m_v_out += ((DST_RCFILTER__VIN - m_v_out) * m_exponent);
            }
            else
            {
                if (m_has_rc_nodes != 0)
                {
                    double rc = DST_RCFILTER__R * DST_RCFILTER__C;
                    if (rc != m_rc)
                    {
                        m_rc = rc;
                        m_exponent = RC_CHARGE_EXP(rc);
                    }
                }

                /************************************************************************/
                /* Next Value = PREV + (INPUT_VALUE - PREV)*(1-(EXP(-TIMEDELTA/RC)))    */
                /************************************************************************/

                m_vCap += ((DST_RCFILTER__VIN - m_v_out) * m_exponent);
                m_v_out = m_vCap + DST_RCFILTER__VREF;
            }

            set_output(0, m_v_out);
        }
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


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_rcfilter_sw_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_rcfilter_sw_node() { }


        // discrete_base_node

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

                m_f1[bits] = RES_VOLTAGE_DIVIDER(rs, CD4066_ON_RES);
                m_f2[bits] = DST_RCFILTER_SW__R / (CD4066_ON_RES + rs);
            }


            /* fast cases */
            m_exp0 = RC_CHARGE_EXP((CD4066_ON_RES + DST_RCFILTER_SW__R) * DST_RCFILTER_SW__C(0));
            m_exp1 = RC_CHARGE_EXP((CD4066_ON_RES + DST_RCFILTER_SW__R) * DST_RCFILTER_SW__C(1));
            m_factor = RES_VOLTAGE_DIVIDER(DST_RCFILTER_SW__R, CD4066_ON_RES);

            set_output(0,  0);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        // FIXME: This needs optimization !
        //DISCRETE_STEP(dst_rcfilter_sw)
        public void step()
        {
            int i;
            int bits = (int)DST_RCFILTER_SW__SWITCH;
            double us = 0;
            double vIn = DST_RCFILTER_SW__VIN;
            double v_out;

            if (DST_RCFILTER_SW__ENABLE != 0)
            {
                switch (bits)
                {
                case 0:
                    v_out = vIn;
                    break;
                case 1:
                    m_vCap[0] += (vIn - m_vCap[0]) * m_exp0;
                    v_out = m_vCap[0] + (vIn - m_vCap[0]) * m_factor;
                    break;
                case 2:
                    m_vCap[1] += (vIn - m_vCap[1]) * m_exp1;
                    v_out = m_vCap[1] + (vIn - m_vCap[1]) * m_factor;
                    break;
                default:
                    for (i = 0; i < 4; i++)
                    {
                        if (( bits & (1 << i)) != 0)
                            us += m_vCap[i];
                    }
                    v_out = m_f1[bits] * vIn + m_f2[bits]  * us;
                    for (i = 0; i < 4; i++)
                    {
                        if (( bits & (1 << i)) != 0)
                            m_vCap[i] += (v_out - m_vCap[i]) * m_exp[i];
                    }
                    break;
                }

                set_output(0, v_out);
            }
            else
            {
                set_output(0, 0);
            }
        }
    }
}
