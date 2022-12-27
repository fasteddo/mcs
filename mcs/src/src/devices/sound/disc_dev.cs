// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;
using uint8_t = System.Byte;
using unsigned = System.UInt32;

using static mame.cpp_global;
using static mame.discrete_global;
using static mame.rescap_global;


namespace mame
{
    //DISCRETE_CLASS_STEP_RESET(dsd_555_astbl, 1,
    class discrete_dsd_555_astbl_node : discrete_base_node,
                                        discrete_step_interface
    {
        const int _maxout = 1;


        double DEFAULT_555_BLEED_R { get { return RES_M(10); } }


        bool DSD_555_ASTBL__RESET { get { return DISCRETE_INPUT(0) == 0; } }
        double DSD_555_ASTBL__R1 { get { return DISCRETE_INPUT(1); } }
        double DSD_555_ASTBL__R2 { get { return DISCRETE_INPUT(2); } }
        double DSD_555_ASTBL__C  { get { return DISCRETE_INPUT(3); } }
        double DSD_555_ASTBL__CTRLV { get { return DISCRETE_INPUT(4); } }

        /* bit mask of the above RC inputs */
        const int DSD_555_ASTBL_RC_MASK = 0x0e;

        /* charge/discharge constants */
        double DSD_555_ASTBL_T_RC_BLEED { get { return DEFAULT_555_BLEED_R * DSD_555_ASTBL__C; } }
        /* Use quick charge if specified. */
        double DSD_555_ASTBL_T_RC_CHARGE(discrete_555_desc info) { return (DSD_555_ASTBL__R1 + (((info.options & DISC_555_ASTABLE_HAS_FAST_CHARGE_DIODE) != 0) ? 0 : DSD_555_ASTBL__R2)) * DSD_555_ASTBL__C; }
        double DSD_555_ASTBL_T_RC_DISCHARGE { get { return DSD_555_ASTBL__R2 * DSD_555_ASTBL__C; } }


        int             m_use_ctrlv;
        int             m_output_type;
        int             m_output_is_ac;
        double          m_ac_shift;                 /* DC shift needed to make waveform ac */
        int             m_flip_flop;                /* 555 flip/flop output state */
        double          m_cap_voltage;              /* voltage on cap */
        double          m_threshold;
        double          m_trigger;
        double          m_v_out_high;               /* Logic 1 voltage level */
        double          m_v_charge;
        Pointer<double> m_v_charge_node;  //const double *  m_v_charge_node;            /* point to output of node */
        int             m_has_rc_nodes;
        double          m_exp_bleed;
        double          m_exp_charge;
        double          m_exp_discharge;
        double          m_t_rc_bleed;
        double          m_t_rc_charge;
        double          m_t_rc_discharge;
        double          m_last_r1;
        double          m_last_r2;
        double          m_last_c;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dsd_555_astbl_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dsd_555_astbl_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dsd_555_astbl)
        public override void reset()
        {
            discrete_555_desc info = (discrete_555_desc)this.custom_data();  //DISCRETE_DECLARE_INFO(discrete_555_desc)

            m_use_ctrlv   = (this.input_is_node() >> 4) & 1;
            m_output_type = info.options & DISC_555_OUT_MASK;

            /* Use the defaults or supplied values. */
            m_v_out_high = (info.v_out_high == DEFAULT_555_HIGH) ? info.v_pos - 1.2 : info.v_out_high;

            /* setup v_charge or node */
            m_v_charge_node = m_device.node_output_ptr((int)info.v_charge);
            if (m_v_charge_node == null)
            {
                m_v_charge = (info.v_charge == DEFAULT_555_CHARGE) ? info.v_pos : info.v_charge;

                if ((info.options & DISC_555_ASTABLE_HAS_FAST_CHARGE_DIODE) != 0) m_v_charge -= 0.5;
            }

            if ((DSD_555_ASTBL__CTRLV != -1) && m_use_ctrlv == 0)
            {
                /* Setup based on supplied Control Voltage static value */
                m_threshold = DSD_555_ASTBL__CTRLV;
                m_trigger   = DSD_555_ASTBL__CTRLV / 2.0;
            }
            else
            {
                /* Setup based on v_pos power source */
                m_threshold = info.v_pos * 2.0 / 3.0;
                m_trigger   = info.v_pos / 3.0;
            }

            /* optimization if none of the values are nodes */
            m_has_rc_nodes = 0;
            if ((this.input_is_node() & DSD_555_ASTBL_RC_MASK) != 0)
            {
                m_has_rc_nodes = 1;
            }
            else
            {
                m_t_rc_bleed  = DSD_555_ASTBL_T_RC_BLEED;
                m_exp_bleed   = RC_CHARGE_EXP(m_t_rc_bleed);
                m_t_rc_charge = DSD_555_ASTBL_T_RC_CHARGE(info);
                m_exp_charge  = RC_CHARGE_EXP(m_t_rc_charge);
                m_t_rc_discharge = DSD_555_ASTBL_T_RC_DISCHARGE;
                m_exp_discharge  = RC_CHARGE_EXP(m_t_rc_discharge);
            }

            m_output_is_ac = info.options & DISC_555_OUT_AC;
            /* Calculate DC shift needed to make squarewave waveform AC */
            m_ac_shift = (m_output_is_ac != 0) ? -m_v_out_high / 2.0 : 0;

            m_flip_flop = 1;
            m_cap_voltage = 0;

            /* Step to set the output */
            this.step();
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }

        //DISCRETE_STEP(dsd_555_astbl)
        public void step()
        {
            discrete_555_desc info = (discrete_555_desc)this.custom_data();  //DISCRETE_DECLARE_INFO(discrete_555_desc)

            int     count_f = 0;
            int     count_r = 0;
            double  dt;                             /* change in time */
            double  x_time  = 0;                    /* time since change happened */
            double  v_cap   = m_cap_voltage;    /* Current voltage on capacitor, before dt */
            double  v_cap_next = 0;                 /* Voltage on capacitor, after dt */
            double  v_charge, exponent = 0;
            uint8_t   flip_flop = (uint8_t)m_flip_flop;
            uint8_t   update_exponent = 0;
            double  v_out = 0.0;

            /* put commonly used stuff in local variables for speed */
            double  threshold = m_threshold;
            double  trigger   = m_trigger;

            if (DSD_555_ASTBL__RESET)
            {
                /* We are in RESET */
                set_output(0, 0);
                m_flip_flop   = 1;
                m_cap_voltage = 0;
                return;
            }

            /* Check: if the Control Voltage node is connected. */
            if (m_use_ctrlv != 0)
            {
                /* If CV is less then .25V, the circuit will oscillate way out of range.
                 * So we will just ignore it when it happens. */
                if (DSD_555_ASTBL__CTRLV < .25) return;
                /* If it is a node then calculate thresholds based on Control Voltage */
                threshold = DSD_555_ASTBL__CTRLV;
                trigger   = DSD_555_ASTBL__CTRLV / 2.0;
                /* Since the thresholds may have changed we need to update the FF */
                if (v_cap >= threshold)
                {
                    flip_flop = 0;
                    count_f++;
                }
                else
                if (v_cap <= trigger)
                {
                    flip_flop = 1;
                    count_r++;
                }
            }

            /* get the v_charge and update each step if it is a node */
            if (m_v_charge_node != null)
            {
                v_charge = m_v_charge_node[0];  // v_charge = *m_v_charge_node;
                if ((info.options & DISC_555_ASTABLE_HAS_FAST_CHARGE_DIODE) != 0) v_charge -= 0.5;
            }
            else
            {
                v_charge = m_v_charge;
            }


            /* Calculate future capacitor voltage.
             * ref@ http://www.physics.rutgers.edu/ugrad/205/capacitance.html
             * The formulas from the ref pages have been modified to reflect that we are stepping the change.
             * dt = time of sample (1/sample frequency)
             * VC = Voltage across capacitor
             * VC' = Future voltage across capacitor
             * Vc = Voltage change
             * Vr = is the voltage across the resistor.  For charging it is Vcc - VC.  Discharging it is VC - 0.
             * R = R1+R2 (for charging)  R = R2 for discharging.
             * Vc = Vr*(1-exp(-dt/(R*C)))
             * VC' = VC + Vc (for charging) VC' = VC - Vc for discharging.
             *
             * We will also need to calculate the amount of time we overshoot the thresholds
             * dt = amount of time we overshot
             * Vc = voltage change overshoot
             * dt = R*C(log(1/(1-(Vc/Vr))))
             */

            dt = this.sample_time();

            /* Sometimes a switching network is used to setup the capacitance.
             * These may select no capacitor, causing oscillation to stop.
             */
            if (DSD_555_ASTBL__C == 0)
            {
                flip_flop = 1;
                /* The voltage goes high because the cap circuit is open. */
                v_cap_next = v_charge;
                v_cap      = v_charge;
                m_cap_voltage = 0;
            }
            else
            {
                /* Update charge contstants and exponents if nodes changed */
                if (m_has_rc_nodes != 0 && (DSD_555_ASTBL__R1 != m_last_r1 || DSD_555_ASTBL__C != m_last_c || DSD_555_ASTBL__R2 != m_last_r2))
                {
                    m_t_rc_bleed  = DSD_555_ASTBL_T_RC_BLEED;
                    m_t_rc_charge = DSD_555_ASTBL_T_RC_CHARGE(info);
                    m_t_rc_discharge = DSD_555_ASTBL_T_RC_DISCHARGE;
                    m_exp_bleed  = RC_CHARGE_EXP(m_t_rc_bleed);
                    m_exp_charge = RC_CHARGE_EXP(m_t_rc_charge);
                    m_exp_discharge = RC_CHARGE_EXP(m_t_rc_discharge);
                    m_last_r1 = DSD_555_ASTBL__R1;
                    m_last_r2 = DSD_555_ASTBL__R2;
                    m_last_c  = DSD_555_ASTBL__C;
                }
                /* Keep looping until all toggling in time sample is used up. */
                do
                {
                    if (flip_flop != 0)
                    {
                        if (DSD_555_ASTBL__R1 == 0)
                        {
                            /* Oscillation disabled because there is no longer any charge resistor. */
                            /* Bleed the cap due to circuit losses. */
                            if (update_exponent != 0)
                                exponent = RC_CHARGE_EXP_DT(m_t_rc_bleed, dt);
                            else
                                exponent = m_exp_bleed;
                            v_cap_next = v_cap - (v_cap * exponent);
                            dt = 0;
                        }
                        else
                        {
                            /* Charging */
                            if (update_exponent != 0)
                                exponent = RC_CHARGE_EXP_DT(m_t_rc_charge, dt);
                            else
                                exponent = m_exp_charge;
                            v_cap_next = v_cap + ((v_charge - v_cap) * exponent);
                            dt = 0;

                            /* has it charged past upper limit? */
                            if (v_cap_next >= threshold)
                            {
                                /* calculate the overshoot time */
                                dt     = m_t_rc_charge  * log(1.0 / (1.0 - ((v_cap_next - threshold) / (v_charge - v_cap))));
                                x_time = dt;
                                v_cap_next  = threshold;
                                flip_flop = 0;
                                count_f++;
                                update_exponent = 1;
                            }
                        }
                    }
                    else
                    {
                        /* Discharging */
                        if(DSD_555_ASTBL__R2 != 0)
                        {
                            if (update_exponent != 0)
                                exponent = RC_CHARGE_EXP_DT(m_t_rc_discharge, dt);
                            else
                                exponent = m_exp_discharge;
                            v_cap_next = v_cap - (v_cap * exponent);
                            dt = 0;
                        }
                        else
                        {
                            /* no discharge resistor so we immediately discharge */
                            v_cap_next = trigger;
                        }

                        /* has it discharged past lower limit? */
                        if (v_cap_next <= trigger)
                        {
                            /* calculate the overshoot time */
                            if (v_cap_next < trigger)
                                dt = m_t_rc_discharge  * log(1.0 / (1.0 - ((trigger - v_cap_next) / v_cap)));
                            x_time = dt;
                            v_cap_next  = trigger;
                            flip_flop = 1;
                            count_r++;
                            update_exponent = 1;
                        }
                    }
                    v_cap = v_cap_next;
                } while (dt != 0);

                m_cap_voltage = v_cap;
            }

            /* Convert last switch time to a ratio */
            x_time = x_time / this.sample_time();

            switch (m_output_type)
            {
                case DISC_555_OUT_SQW:
                    if (count_f + count_r >= 2)
                        /* force at least 1 toggle */
                        v_out =  m_flip_flop != 0 ? 0 : m_v_out_high;
                    else
                        v_out =  flip_flop * m_v_out_high;
                    v_out += m_ac_shift;
                    break;
                case DISC_555_OUT_CAP:
                    v_out =  v_cap;
                    /* Fake it to AC if needed */
                    if (m_output_is_ac != 0)
                        v_out -= threshold * 3.0 /4.0;
                    break;
                case DISC_555_OUT_ENERGY:
                    if (x_time == 0) x_time = 1.0;
                    v_out = m_v_out_high * (flip_flop != 0 ? x_time : (1.0 - x_time));
                    v_out += m_ac_shift;
                    break;
                case DISC_555_OUT_LOGIC_X:
                    v_out =  flip_flop + x_time;
                    break;
                case DISC_555_OUT_COUNT_F_X:
                    v_out = count_f != 0 ? count_f + x_time : count_f;
                    break;
                case DISC_555_OUT_COUNT_R_X:
                    v_out =  count_r != 0 ? count_r + x_time : count_r;
                    break;
                case DISC_555_OUT_COUNT_F:
                    v_out =  count_f;
                    break;
                case DISC_555_OUT_COUNT_R:
                    v_out =  count_r;
                    break;
            }
            set_output(0, v_out);
            m_flip_flop = flip_flop;
        }
    }


#if false
    DISCRETE_CLASS_STEP_RESET(dsd_555_mstbl, 1,
        int             m_trig_is_logic;
        int             m_trig_discharges_cap;
        int             m_output_type;
        double          m_ac_shift;                 /* DC shift needed to make waveform ac */
        int             m_flip_flop;                /* 555 flip/flop output state */
        int             m_has_rc_nodes;
        double          m_exp_charge;
        double          m_cap_voltage;              /* voltage on cap */
        double          m_threshold;
        double          m_trigger;
        double          m_v_out_high;               /* Logic 1 voltage level */
        double          m_v_charge;
    );
#endif


    //DISCRETE_CLASS_STEP_RESET(dsd_555_cc, 1,
    class discrete_dsd_555_cc_node : discrete_base_node,
                                     discrete_step_interface
    {
        const int _maxout = 1;


        double DEFAULT_555_BLEED_R { get { return RES_M(10); } }


        double DSD_555_CC__RESET { get { return (!(DISCRETE_INPUT(0) != 0)) ? 1 : 0; } }
        double DSD_555_CC__VIN { get { return DISCRETE_INPUT(1); } }
        double DSD_555_CC__R { get { return DISCRETE_INPUT(2); } }
        double DSD_555_CC__C { get { return DISCRETE_INPUT(3); } }
        double DSD_555_CC__RBIAS { get { return DISCRETE_INPUT(4); } }
        double DSD_555_CC__RGND { get { return DISCRETE_INPUT(5); } }
        double DSD_555_CC__RDIS { get { return DISCRETE_INPUT(6); } }

        /* bit mask of the above RC inputs not including DSD_555_CC__R */
        const int DSD_555_CC_RC_MASK  = 0x78;

        /* charge/discharge constants */
        double DSD_555_CC_T_RC_BLEED { get { return DEFAULT_555_BLEED_R * DSD_555_CC__C; } }
        double DSD_555_CC_T_RC_DISCHARGE_01 { get { return DSD_555_CC__RDIS * DSD_555_CC__C; } }
        double DSD_555_CC_T_RC_DISCHARGE_NO_I { get { return DSD_555_CC__RGND * DSD_555_CC__C; } }
        double DSD_555_CC_T_RC_CHARGE(double r_charge) { return r_charge * DSD_555_CC__C; }
        double DSD_555_CC_T_RC_DISCHARGE(double r_discharge) { return r_discharge * DSD_555_CC__C; }


        unsigned m_type;                     /* type of 555cc circuit */
        int             m_output_type;
        int             m_output_is_ac;
        double          m_ac_shift;                 /* DC shift needed to make waveform ac */
        int             m_flip_flop;                /* 555 flip/flop output state */
        double          m_cap_voltage;              /* voltage on cap */
        double          m_threshold;
        double          m_trigger;
        double          m_v_out_high;               /* Logic 1 voltage level */
        double          m_v_cc_source;
        int             m_has_rc_nodes;
        double          m_exp_bleed;
        double          m_exp_charge;
        double          m_exp_discharge;
        double          m_exp_discharge_01;
        double          m_exp_discharge_no_i;
        double          m_t_rc_charge;
        double          m_t_rc_discharge;
        double          m_t_rc_discharge_01;
        double          m_t_rc_discharge_no_i;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dsd_555_cc_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dsd_555_cc_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dsd_555_cc)
        public override void reset()
        {
            discrete_555_cc_desc info = (discrete_555_cc_desc)this.custom_data();  //DISCRETE_DECLARE_INFO(discrete_555_cc_desc);

            double r_temp;
            double r_discharge = 0;
            double r_charge = 0;

            m_flip_flop   = 1;
            m_cap_voltage = 0;

            m_output_type = info.options & DISC_555_OUT_MASK;

            /* Use the defaults or supplied values. */
            m_v_out_high  = (info.v_out_high  == DEFAULT_555_HIGH) ? info.v_pos - 1.2 : info.v_out_high;
            m_v_cc_source = (info.v_cc_source == DEFAULT_555_CC_SOURCE) ? info.v_pos : info.v_cc_source;

            /* Setup based on v_pos power source */
            m_threshold = info.v_pos * 2.0 / 3.0;
            m_trigger   = info.v_pos / 3.0;

            m_output_is_ac = info.options & DISC_555_OUT_AC;
            /* Calculate DC shift needed to make squarewave waveform AC */
            m_ac_shift     = (m_output_is_ac != 0) ? -m_v_out_high / 2.0 : 0;

            /* There are 8 different types of basic oscillators
             * depending on the resistors used.  We will determine
             * the type of circuit at reset, because the ciruit type
             * is constant.  See Below.
             */
            m_type = (unsigned)(((DSD_555_CC__RDIS > 0) ? 1 : 0) | (((DSD_555_CC__RGND  > 0) ? 1 : 0) << 1) | (((DSD_555_CC__RBIAS  > 0) ? 1 : 0) << 2));

            /* optimization if none of the values are nodes */
            m_has_rc_nodes = 0;

            if ((this.input_is_node() & DSD_555_CC_RC_MASK) != 0)
            {
                m_has_rc_nodes = 1;
            }
            else
            {
                switch (m_type) /* see dsd_555_cc_reset for descriptions */
                {
                    case 1:
                        r_discharge = DSD_555_CC__RDIS;
                        goto case 0;  //[[fallthrough]];
                    case 0:
                        break;
                    case 3:
                        r_discharge = RES_2_PARALLEL(DSD_555_CC__RDIS, DSD_555_CC__RGND);
                        goto case 2;  //[[fallthrough]];
                    case 2:
                        r_charge = DSD_555_CC__RGND;
                        break;
                    case 4:
                        r_charge = DSD_555_CC__RBIAS;
                        break;
                    case 5:
                        r_charge = DSD_555_CC__RBIAS + DSD_555_CC__RDIS;
                        r_discharge = DSD_555_CC__RDIS;
                        break;
                    case 6:
                        r_charge = RES_2_PARALLEL(DSD_555_CC__RBIAS, DSD_555_CC__RGND);
                        break;
                    case 7:
                        r_temp   = DSD_555_CC__RBIAS + DSD_555_CC__RDIS;
                        r_charge = RES_2_PARALLEL(r_temp, DSD_555_CC__RGND);
                        r_discharge = RES_2_PARALLEL(DSD_555_CC__RGND, DSD_555_CC__RDIS);
                        break;
                }

                m_exp_bleed  = RC_CHARGE_EXP(DSD_555_CC_T_RC_BLEED);
                m_t_rc_discharge_01 = DSD_555_CC_T_RC_DISCHARGE_01;
                m_exp_discharge_01  = RC_CHARGE_EXP(m_t_rc_discharge_01);
                m_t_rc_discharge_no_i = DSD_555_CC_T_RC_DISCHARGE_NO_I;
                m_exp_discharge_no_i  = RC_CHARGE_EXP(m_t_rc_discharge_no_i);
                m_t_rc_charge = DSD_555_CC_T_RC_CHARGE(r_charge);
                m_exp_charge  = RC_CHARGE_EXP(m_t_rc_charge);
                m_t_rc_discharge = DSD_555_CC_T_RC_DISCHARGE(r_discharge);
                m_exp_discharge  = RC_CHARGE_EXP(m_t_rc_discharge);
            }

            /* Step to set the output */
            this.step();

            /*
             * TYPES:
             * Note: These are equivalent circuits shown without the 555 circuitry.
             *       See the schematic in src\sound\discrete.h for full hookup info.
             *
             * DISCRETE_555_CC_TO_DISCHARGE_PIN
             * When the CC source is connected to the discharge pin, it allows the
             * circuit to charge when the 555 is in charge mode.  But when in discharge
             * mode, the CC source is grounded, disabling it's effect.
             *
             * [0]
             * No resistors.  Straight constant current charge of capacitor.
             * When there is not any charge current, the cap will bleed off.
             * Once the lower threshold(trigger) is reached, the output will
             * go high but the cap will continue to discharge due to losses.
             *   .------+---> cap_voltage      CHARGING:
             *   |      |                 dv (change in voltage) compared to dt (change in time in seconds).
             * .---.   ---                dv = i * dt / C; where i is current in amps and C is capacitance in farads.
             * | i |   --- C              cap_voltage = cap_voltage + dv
             * '---'    |
             *   |      |               DISCHARGING:
             *  gnd    gnd                instantaneous
             *
             * [1]
             * Same as type 1 but with rDischarge.  rDischarge has no effect on the charge rate because
             * of the constant current source i.
             * When there is not any charge current, the cap will bleed off.
             * Once the lower threshold(trigger) is reached, the output will
             * go high but the cap will continue to discharge due to losses.
             *   .----ZZZ-----+---> cap_voltage      CHARGING:
             *   | rDischarge |                 dv (change in voltage) compared to dt (change in time in seconds).
             * .---.         ---                dv = i * dt / C; where i is current in amps and C is capacitance in farads.
             * | i |         --- C              cap_voltage = cap_voltage + dv
             * '---'          |
             *   |            |               DISCHARGING:
             *  gnd          gnd                through rDischarge
             *
             * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
             * !!!!! IMPORTANT NOTE ABOUT TYPES 3 - 7 !!!!!
             * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
             *
             * From here on in all the circuits have either an rBias or rGnd resistor.
             * This converts the constant current into a voltage source.
             * So all the remaining circuit types will be converted to this circuit.
             * When discharging, rBias is out of the equation because the 555 is grounding the circuit
             * after that point.
             *
             * .------------.     Rc                  Rc is the equivilent circuit resistance.
             * |     v      |----ZZZZ---+---> cap_voltage    v  is the equivilent circuit voltage.
             * |            |           |
             * '------------'          ---            Then the standard RC charging formula applies.
             *       |                 --- C
             *       |                  |             NOTE: All the following types are converted to Rc and v values.
             *      gnd                gnd
             *
             * [2]
             * When there is not any charge current, the cap will bleed off.
             * Once the lower threshold(trigger) is reached, the output will
             * go high but the cap will continue to discharge due to rGnd.
             *   .-------+------+------> cap_voltage         CHARGING:
             *   |       |      |                       v = vi = i * rGnd
             * .---.    ---     Z                       Rc = rGnd
             * | i |    --- C   Z rGnd
             * '---'     |      |                     DISCHARGING:
             *   |       |      |                       instantaneous
             *  gnd     gnd    gnd
             *
             * [3]
             * When there is not any charge current, the cap will bleed off.
             * Once the lower threshold(trigger) is reached, the output will
             * go high but the cap will continue to discharge due to rGnd.
             *   .----ZZZ-----+------+------> cap_voltage    CHARGING:
             *   | rDischarge |      |                  v = vi = i * rGnd
             * .---.         ---     Z                  Rc = rGnd
             * | i |         --- C   Z rGnd
             * '---'          |      |                DISCHARGING:
             *   |            |      |                  through rDischarge || rGnd  ( || means in parallel)
             *  gnd          gnd    gnd
             *
             * [4]
             *     .---ZZZ---+------------+-------------> cap_voltage      CHARGING:
             *     |  rBias  |            |                           Rc = rBias
             * .-------.   .---.         ---                          vi = i * rBias
             * | vBias |   | i |         --- C                        v = vBias + vi
             * '-------'   '---'          |
             *     |         |            |                         DISCHARGING:
             *    gnd       gnd          gnd                          instantaneous
             *
             * [5]
             *     .---ZZZ---+----ZZZ-----+-------------> cap_voltage      CHARGING:
             *     |  rBias  | rDischarge |                           Rc = rBias + rDischarge
             * .-------.   .---.         ---                          vi = i * rBias
             * | vBias |   | i |         --- C                        v = vBias + vi
             * '-------'   '---'          |
             *     |         |            |                         DISCHARGING:
             *    gnd       gnd          gnd                          through rDischarge
             *
             * [6]
             *     .---ZZZ---+------------+------+------> cap_voltage      CHARGING:
             *     |  rBias  |            |      |                    Rc = rBias || rGnd
             * .-------.   .---.         ---     Z                    vi = i * Rc
             * | vBias |   | i |         --- C   Z rGnd               v = vBias * (rGnd / (rBias + rGnd)) + vi
             * '-------'   '---'          |      |
             *     |         |            |      |                  DISCHARGING:
             *    gnd       gnd          gnd    gnd                   instantaneous
             *
             * [7]
             *     .---ZZZ---+----ZZZ-----+------+------> cap_voltage      CHARGING:
             *     |  rBias  | rDischarge |      |                    Rc = (rBias + rDischarge) || rGnd
             * .-------.   .---.         ---     Z                    vi = i * rBias * (rGnd / (rBias + rDischarge + rGnd))
             * | vBias |   | i |         --- C   Z rGnd               v = vBias * (rGnd / (rBias + rDischarge + rGnd)) + vi
             * '-------'   '---'          |      |
             *     |         |            |      |                  DISCHARGING:
             *    gnd       gnd          gnd    gnd                   through rDischarge || rGnd
             */

            /*
             * DISCRETE_555_CC_TO_CAP
             *
             * When the CC source is connected to the capacitor, it allows the
             * current to charge the cap while it is in discharge mode, slowing the
             * discharge.  So in charge mode it charges linearly from the constant
             * current cource.  But when in discharge mode it behaves like circuit
             * type 2 above.
             *   .-------+------+------> cap_voltage         CHARGING:
             *   |       |      |                       dv = i * dt / C
             * .---.    ---     Z                       cap_voltage = cap_voltage + dv
             * | i |    --- C   Z rDischarge
             * '---'     |      |                     DISCHARGING:
             *   |       |      |                       v = vi = i * rGnd
             *  gnd     gnd   discharge                 Rc = rDischarge
             */
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dsd_555_cc)
        public void step()
        {
            discrete_555_cc_desc info = (discrete_555_cc_desc)this.custom_data();  //DISCRETE_DECLARE_INFO(discrete_555_cc_desc)

            int     count_f  = 0;
            int     count_r  = 0;
            double  i;                  /* Charging current created by vIn */
            double  r_charge = 0;       /* Equivalent charging resistor */
            double  r_discharge = 0;    /* Equivalent discharging resistor */
            double  vi     = 0;         /* Equivalent voltage from current source */
            double  v_bias = 0;         /* Equivalent voltage from bias voltage */
            double  v      = 0;         /* Equivalent voltage total from current source and bias circuit if used */
            double  dt;                 /* change in time */
            double  x_time = 0;         /* time since change happened */
            double  t_rc ;              /* RC time constant */
            double  v_cap;              /* Current voltage on capacitor, before dt */
            double  v_cap_next = 0;     /* Voltage on capacitor, after dt */
            double  v_vcharge_limit;    /* vIn and the junction voltage limit the max charging voltage from i */
            double  r_temp;             /* play thing */
            double  exponent;
            uint8_t   update_exponent;
            uint8_t   update_t_rc;
            uint8_t   flip_flop = (uint8_t)m_flip_flop;

            double v_out = 0;


            if ((DSD_555_CC__RESET) != 0)
            {
                /* We are in RESET */
                set_output(0, 0);
                m_flip_flop   = 1;
                m_cap_voltage = 0;
                return;
            }

            dt    = this.sample_time();    /* Change in time */
            v_cap = m_cap_voltage;  /* Set to voltage before change */
            v_vcharge_limit = DSD_555_CC__VIN + info.v_cc_junction;    /* the max v_cap can be and still be charged by i */
            /* Calculate charging current */
            i = (m_v_cc_source - v_vcharge_limit) / DSD_555_CC__R;

            if ( i < 0) i = 0;

            if ((info.options & DISCRETE_555_CC_TO_CAP) != 0)
            {
                vi = i * DSD_555_CC__RDIS;
            }
            else
            {
                switch (m_type) /* see dsd_555_cc_reset for descriptions */
                {
                    case 1:
                        r_discharge = DSD_555_CC__RDIS;
                        goto case 0;  //[[fallthrough]];
                    case 0:
                        break;
                    case 3:
                        r_discharge = RES_2_PARALLEL(DSD_555_CC__RDIS, DSD_555_CC__RGND);
                        goto case 2;  //[[fallthrough]];
                    case 2:
                        r_charge = DSD_555_CC__RGND;
                        vi       = i * r_charge;
                        break;
                    case 4:
                        r_charge = DSD_555_CC__RBIAS;
                        vi       = i * r_charge;
                        v_bias   = info.v_pos;
                        break;
                    case 5:
                        r_charge = DSD_555_CC__RBIAS + DSD_555_CC__RDIS;
                        vi      = i * DSD_555_CC__RBIAS;
                        v_bias  = info.v_pos;
                        r_discharge = DSD_555_CC__RDIS;
                        break;
                    case 6:
                        r_charge = RES_2_PARALLEL(DSD_555_CC__RBIAS, DSD_555_CC__RGND);
                        vi      = i * r_charge;
                        v_bias  = info.v_pos * RES_VOLTAGE_DIVIDER(DSD_555_CC__RGND, DSD_555_CC__RBIAS);
                        break;
                    case 7:
                        r_temp   = DSD_555_CC__RBIAS + DSD_555_CC__RDIS;
                        r_charge = RES_2_PARALLEL(r_temp, DSD_555_CC__RGND);
                        r_temp  += DSD_555_CC__RGND;
                        r_temp   = DSD_555_CC__RGND / r_temp;   /* now has voltage divider ratio, not resistance */
                        vi      = i * DSD_555_CC__RBIAS * r_temp;
                        v_bias  = info.v_pos * r_temp;
                        r_discharge = RES_2_PARALLEL(DSD_555_CC__RGND, DSD_555_CC__RDIS);
                        break;
                }
            }

            /* Keep looping until all toggling in time sample is used up. */
            update_t_rc = (uint8_t)m_has_rc_nodes;
            update_exponent = update_t_rc;
            do
            {
                if (m_type <= 1)
                {
                    /* Standard constant current charge */
                    if (flip_flop != 0)
                    {
                        if (i == 0)
                        {
                            /* No charging current, so we have to discharge the cap
                                * due to cap and circuit losses.
                                */
                            if (update_exponent != 0)
                            {
                                t_rc     = DSD_555_CC_T_RC_BLEED;
                                exponent = RC_CHARGE_EXP_DT(t_rc, dt);
                            }
                            else
                            {
                                exponent = m_exp_bleed;
                            }

                            v_cap_next = v_cap - (v_cap * exponent);
                            dt = 0;
                        }
                        else
                        {
                            /* Charging */
                            /* iC=C*dv/dt  works out to dv=iC*dt/C */
                            v_cap_next = v_cap + (i * dt / DSD_555_CC__C);
                            /* Yes, if the cap voltage has reached the max voltage it can,
                                * and the 555 threshold has not been reached, then oscillation stops.
                                * This is the way the actual electronics works.
                                * This is why you never play with the pots after being factory adjusted
                                * to work in the proper range. */
                            if (v_cap_next > v_vcharge_limit) v_cap_next = v_vcharge_limit;

                            dt = 0;

                            /* has it charged past upper limit? */
                            if (v_cap_next >= m_threshold)
                            {
                                /* calculate the overshoot time */
                                dt     = DSD_555_CC__C * (v_cap_next - m_threshold) / i;
                                x_time = dt;
                                v_cap_next = m_threshold;
                                flip_flop = 0;
                                count_f++;
                                update_exponent = 1;
                            }
                        }
                    }
                    else if (DSD_555_CC__RDIS != 0)
                    {
                        /* Discharging */
                        if (update_t_rc != 0)
                            t_rc = DSD_555_CC_T_RC_DISCHARGE_01;
                        else
                            t_rc = m_t_rc_discharge_01;

                        if (update_exponent != 0)
                            exponent = RC_CHARGE_EXP_DT(t_rc, dt);
                        else
                            exponent = m_exp_discharge_01;

                        if ((info.options & DISCRETE_555_CC_TO_CAP) != 0)
                        {
                            /* Asteroids - Special Case */
                            /* Charging in discharge mode */
                            /* If the cap voltage is past the current source charging limit
                                * then only the bias voltage will charge the cap. */
                            v          = (v_cap < v_vcharge_limit) ? vi : v_vcharge_limit;
                            v_cap_next = v_cap + ((v - v_cap) * exponent);
                        }
                        else
                        {
                            v_cap_next = v_cap - (v_cap * exponent);
                        }

                        dt = 0;
                        /* has it discharged past lower limit? */
                        if (v_cap_next <= m_trigger)
                        {
                            dt     = t_rc  * log(1.0 / (1.0 - ((m_trigger - v_cap_next) / v_cap)));
                            x_time = dt;
                            v_cap_next  = m_trigger;
                            flip_flop = 1;
                            count_r++;
                            update_exponent = 1;
                        }
                    }
                    else    /* Immediate discharge. No change in dt. */
                    {
                        x_time = dt;
                        v_cap_next = m_trigger;
                        flip_flop = 1;
                        count_r++;
                    }
                }
                else
                {
                    /* The constant current gets changed to a voltage due to a load resistor. */
                    if (flip_flop != 0)
                    {
                        if ((i == 0) && (DSD_555_CC__RBIAS == 0))
                        {
                            /* No charging current, so we have to discharge the cap
                                * due to rGnd.
                                */
                            if (update_t_rc != 0)
                                t_rc = DSD_555_CC_T_RC_DISCHARGE_NO_I;
                            else
                                t_rc = m_t_rc_discharge_no_i;

                            if (update_exponent != 0)
                                exponent = RC_CHARGE_EXP_DT(t_rc, dt);
                            else
                                exponent = m_exp_discharge_no_i;

                            v_cap_next = v_cap - (v_cap * exponent);
                            dt = 0;
                        }
                        else
                        {
                            /* Charging */
                            /* If the cap voltage is past the current source charging limit
                                * then only the bias voltage will charge the cap. */
                            v = v_bias;
                            if (v_cap < v_vcharge_limit) v += vi;
                            else if (m_type <= 3) v = v_vcharge_limit;

                            if (update_t_rc != 0)
                                t_rc = DSD_555_CC_T_RC_CHARGE(r_charge);
                            else
                                t_rc = m_t_rc_charge;

                            if (update_exponent != 0)
                                exponent = RC_CHARGE_EXP_DT(t_rc, dt);
                            else
                                exponent = m_exp_charge;

                            v_cap_next = v_cap + ((v - v_cap) * exponent);
                            dt         = 0;

                            /* has it charged past upper limit? */
                            if (v_cap_next >= m_threshold)
                            {
                                /* calculate the overshoot time */
                                dt     = t_rc  * log(1.0 / (1.0 - ((v_cap_next - m_threshold) / (v - v_cap))));
                                x_time = dt;
                                v_cap_next = m_threshold;
                                flip_flop = 0;
                                count_f++;
                                update_exponent = 1;
                            }
                        }
                    }
                    else if (r_discharge != 0)  /* Discharging */
                    {
                        if (update_t_rc != 0)
                            t_rc = DSD_555_CC_T_RC_DISCHARGE(r_discharge);
                        else
                            t_rc = m_t_rc_discharge;

                        if (update_exponent != 0)
                            exponent = RC_CHARGE_EXP_DT(t_rc, dt);
                        else
                            exponent = m_exp_discharge;

                        v_cap_next = v_cap - (v_cap * exponent);
                        dt = 0;

                        /* has it discharged past lower limit? */
                        if (v_cap_next <= m_trigger)
                        {
                            /* calculate the overshoot time */
                            dt     = t_rc  * log(1.0 / (1.0 - ((m_trigger - v_cap_next) / v_cap)));
                            x_time = dt;
                            v_cap_next = m_trigger;
                            flip_flop = 1;
                            count_r++;
                            update_exponent = 1;
                        }
                    }
                    else    /* Immediate discharge. No change in dt. */
                    {
                        x_time = dt;
                        v_cap_next = m_trigger;
                        flip_flop = 1;
                        count_r++;
                    }
                }

                v_cap = v_cap_next;

            } while (dt != 0);

            m_cap_voltage = v_cap;

            /* Convert last switch time to a ratio */
            x_time = x_time / this.sample_time();

            switch (m_output_type)
            {
                case DISC_555_OUT_SQW:
                    if (count_f + count_r >= 2)
                        /* force at least 1 toggle */
                        v_out =  (m_flip_flop != 0) ? 0 : m_v_out_high;
                    else
                        v_out = flip_flop * m_v_out_high;
                    /* Fake it to AC if needed */
                    v_out += m_ac_shift;
                    break;
                case DISC_555_OUT_CAP:
                    v_out = v_cap + m_ac_shift;
                    break;
                case DISC_555_OUT_ENERGY:
                    if (x_time == 0) x_time = 1.0;
                    v_out = m_v_out_high * ((flip_flop != 0) ? x_time : (1.0 - x_time));
                    v_out += m_ac_shift;
                    break;
                case DISC_555_OUT_LOGIC_X:
                    v_out = flip_flop + x_time;
                    break;
                case DISC_555_OUT_COUNT_F_X:
                    v_out = (count_f != 0) ? count_f + x_time : count_f;
                    break;
                case DISC_555_OUT_COUNT_R_X:
                    v_out = (count_r != 0) ? count_r + x_time : count_r;
                    break;
                case DISC_555_OUT_COUNT_F:
                    v_out = count_f;
                    break;
                case DISC_555_OUT_COUNT_R:
                    v_out = count_r;
                    break;
            }

            set_output(0, v_out);
            m_flip_flop = flip_flop;
        }
    }


#if false
    DISCRETE_CLASS_STEP_RESET(dsd_555_vco1, 1,
        int             m_ctrlv_is_node;
        int             m_output_type;
        int             m_output_is_ac;
        double          m_ac_shift;                 /* DC shift needed to make waveform ac */
        int             m_flip_flop;                /* flip/flop output state */
        double          m_v_out_high;               /* 555 high voltage */
        double          m_threshold;                /* falling threshold */
        double          m_trigger;                  /* rising threshold */
        double          m_i_charge;                 /* charge current */
        double          m_i_discharge;              /* discharge current */
        double          m_cap_voltage;              /* current capacitor voltage */
    );
#endif

#if false
    DISCRETE_CLASS_STEP_RESET(dsd_566, 1,
        //unsigned int    m_state[2];                 /* keeps track of excess flip_flop changes during the current step */
        int             m_flip_flop;                /* 566 flip/flop output state */
        double          m_cap_voltage;              /* voltage on cap */
        double          m_v_sqr_low;                /* voltage for a squarewave at low */
        double          m_v_sqr_high;               /* voltage for a squarewave at high */
        double          m_v_sqr_diff;
        double          m_threshold_low;            /* falling threshold */
        double          m_threshold_high;           /* rising threshold */
        double          m_ac_shift;                 /* used to fake AC */
        double          m_v_osc_stable;
        double          m_v_osc_stop;
        int             m_fake_ac;
        int             m_out_type;
    );
#endif


    //DISCRETE_CLASS_STEP_RESET(dsd_ls624, 1,
    class discrete_dsd_ls624_node : discrete_base_node,
                                    discrete_step_interface
    {
        const int _maxout = 1;


        double DSD_LS624__ENABLE { get { return DISCRETE_INPUT(0); } }
        double DSD_LS624__VMOD { get { return DISCRETE_INPUT(1); } }
        double DSD_LS624__VRNG { get { return DISCRETE_INPUT(2); } }
        double DSD_LS624__C { get { return DISCRETE_INPUT(3); } }
        double DSD_LS624__R_FREQ_IN { get { return DISCRETE_INPUT(4); } }
        double DSD_LS624__C_FREQ_IN { get { return DISCRETE_INPUT(5); } }
        double DSD_LS624__R_RNG_IN { get { return DISCRETE_INPUT(6); } }
        double DSD_LS624__OUTTYPE { get { return DISCRETE_INPUT(7); } }

        //#define LS624_R_EXT         600.0       /* as specified in data sheet */
        const double LS624_OUT_HIGH            = 4.5;         /* measured */
        static readonly double LS624_IN_R      = RES_K(90);   /* measured & 70K + 20k per data sheet */


        double m_exponent;
        double m_t_used;
        double m_v_cap_freq_in;
        double m_v_freq_scale;
        double m_v_rng_scale;
        int m_flip_flop;
        int m_has_freq_in_cap;
        int m_out_type;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dsd_ls624_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dsd_ls624_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dsd_ls624)
        public override void reset()
        {
            m_out_type = (int)DSD_LS624__OUTTYPE;

            m_flip_flop = 0;
            m_t_used = 0;
            m_v_freq_scale = LS624_IN_R / (DSD_LS624__R_FREQ_IN + LS624_IN_R);
            m_v_rng_scale = LS624_IN_R / (DSD_LS624__R_RNG_IN + LS624_IN_R);
            if (DSD_LS624__C_FREQ_IN > 0)
            {
                m_has_freq_in_cap = 1;
                m_exponent = RC_CHARGE_EXP(RES_2_PARALLEL(DSD_LS624__R_FREQ_IN, LS624_IN_R) * DSD_LS624__C_FREQ_IN);
                m_v_cap_freq_in = 0;
            }
            else
                m_has_freq_in_cap = 0;

            set_output(0,  0);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dsd_ls624)
        public void step()
        {
            double x_time = 0;
            double freq;
            double t1;
            double v_freq_2;
            double v_freq_3;
            double v_freq_4;
            double t_used = m_t_used;
            double dt = this.sample_time();
            double v_freq = DSD_LS624__VMOD;
            double v_rng = DSD_LS624__VRNG;
            int count_f = 0;
            int count_r = 0;

            /* coefficients */
            const double k1 = 1.9904769024796283E+03;
            const double k2 = 1.2070059213983407E+03;
            const double k3 = 1.3266985579561108E+03;
            const double k4 = -1.5500979825922698E+02;
            const double k5 = 2.8184536266938172E+00;
            const double k6 = -2.3503421582744556E+02;
            const double k7 = -3.3836786704527788E+02;
            const double k8 = -1.3569136703258670E+02;
            const double k9 = 2.9914575453819188E+00;
            const double k10 = 1.6855569086173170E+00;

            if (DSD_LS624__ENABLE == 0)
                return;

            /* scale due to input resistance */
            v_freq *= m_v_freq_scale;
            v_rng *= m_v_rng_scale;

            /* apply cap if needed */
            if (m_has_freq_in_cap != 0)
            {
                m_v_cap_freq_in += (v_freq - m_v_cap_freq_in) * m_exponent;
                v_freq = m_v_cap_freq_in;
            }

            /* Polyfunctional3D_model created by zunzun.com using sum of squared absolute error */
            v_freq_2 = v_freq * v_freq;
            v_freq_3 = v_freq_2 * v_freq;
            v_freq_4 = v_freq_3 * v_freq;
            freq = k1;
            freq += k2 * v_freq;
            freq += k3 * v_freq_2;
            freq += k4 * v_freq_3;
            freq += k5 * v_freq_4;
            freq += k6 * v_rng;
            freq += k7 * v_rng * v_freq;
            freq += k8 * v_rng * v_freq_2;
            freq += k9 * v_rng * v_freq_3;
            freq += k10 * v_rng * v_freq_4;

            freq *= CAP_U(0.1) / DSD_LS624__C;

            t1 = 0.5 / freq ;
            t_used += this.sample_time();
            do
            {
                dt = 0;
                if (t_used > t1)
                {
                    /* calculate the overshoot time */
                    t_used -= t1;
                    m_flip_flop ^= 1;
                    if (m_flip_flop != 0)
                        count_r++;
                    else
                        count_f++;
                    /* fix up any frequency increase change errors */
                    while(t_used > this.sample_time())
                        t_used -= this.sample_time();
                    x_time = t_used;
                    dt = t_used;
                }
            }while (dt != 0);

            m_t_used = t_used;

            /* Convert last switch time to a ratio */
            x_time = x_time / this.sample_time();

            switch (m_out_type)
            {
                case DISC_LS624_OUT_LOGIC_X:
                    set_output(0,  m_flip_flop  + x_time);
                    break;
                case DISC_LS624_OUT_COUNT_F_X:
                    set_output(0,  count_f != 0 ? count_f + x_time : count_f);
                    break;
                case DISC_LS624_OUT_COUNT_R_X:
                    set_output(0,   count_r != 0 ? count_r + x_time : count_r);
                    break;
                case DISC_LS624_OUT_COUNT_F:
                    set_output(0,  count_f);
                    break;
                case DISC_LS624_OUT_COUNT_R:
                    set_output(0,  count_r);
                    break;
                case DISC_LS624_OUT_ENERGY:
                    if (x_time == 0) x_time = 1.0;
                    set_output(0,  LS624_OUT_HIGH * (m_flip_flop != 0 ? x_time : (1.0 - x_time)));
                    break;
                case DISC_LS624_OUT_LOGIC:
                        set_output(0,  m_flip_flop);
                    break;
                case DISC_LS624_OUT_SQUARE:
                    set_output(0,  m_flip_flop != 0 ? LS624_OUT_HIGH : 0);
                    break;
            }
        }
    }
}
