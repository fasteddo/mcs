// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    //DISCRETE_CLASS_STEP_RESET(dss_counter, 1,
    class discrete_dss_counter_node : discrete_base_node,
                                      discrete_step_interface
    {
        const int _maxout = 1;


        double DSS_COUNTER__ENABLE { get { return DISCRETE_INPUT(0); } }
        double DSS_COUNTER__RESET { get { return DISCRETE_INPUT(1); } }
        double DSS_COUNTER__CLOCK { get { return DISCRETE_INPUT(2); } }
        double DSS_COUNTER__MIN { get { return DISCRETE_INPUT(3); } }
        double DSS_COUNTER__MAX { get { return DISCRETE_INPUT(4); } }
        double DSS_COUNTER__DIR { get { return DISCRETE_INPUT(5); } }
        double DSS_COUNTER__INIT { get { return DISCRETE_INPUT(6); } }
        double DSS_COUNTER__CLOCK_TYPE { get { return DISCRETE_INPUT(7); } }
        double DSS_7492__CLOCK_TYPE { get { return DSS_COUNTER__MIN; } }


        static readonly int [] disc_7492_count = new int [6] {0x00, 0x01, 0x02, 0x04, 0x05, 0x06};


        int             m_clock_type;
        int             m_out_type;
        int             m_is_7492;
        int             m_last_clock;
        uint32_t          m_last_count;
        ////uint32_t          m_last;                 /* Last clock state */
        uint32_t          m_min;
        uint32_t          m_max;
        uint32_t          m_diff;
        double          m_t_left;               /* time unused during last sample in seconds */


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dss_counter_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dss_counter_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dss_counter)
        public override void reset()
        {
            if (((int)DSS_COUNTER__CLOCK_TYPE & g.DISC_COUNTER_IS_7492) != 0)
            {
                m_is_7492    = 1;
                m_clock_type = (int)DSS_7492__CLOCK_TYPE;
                m_max = 5;
                m_min = 0;
                m_diff = 6;
            }
            else
            {
                m_is_7492    = 0;
                m_clock_type = (int)DSS_COUNTER__CLOCK_TYPE;
                m_max = (uint32_t)DSS_COUNTER__MAX;
                m_min = (uint32_t)DSS_COUNTER__MIN;
                m_diff = m_max - m_min + 1;
            }


            if (m_is_7492 == 0 && (DSS_COUNTER__MAX < DSS_COUNTER__MIN))
                fatalerror("MAX < MIN in NODE_{0}\n", this.index());

            m_out_type    = m_clock_type & g.DISC_OUT_MASK;
            m_clock_type &= g.DISC_CLK_MASK;

            m_t_left = 0;
            m_last_count = 0;
            m_last_clock = 0;
            set_output(0, DSS_COUNTER__INIT); /* count starts at reset value */
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dss_counter)
        public void step()
        {
            double cycles;
            double ds_clock;
            int clock = 0;
            int inc = 0;
            uint32_t last_count = m_last_count;  /* it is different then output in 7492 */
            double x_time = 0;
            uint32_t count = last_count;

            ds_clock = DSS_COUNTER__CLOCK;
            if (m_clock_type == g.DISC_CLK_IS_FREQ)
            {
                /* We need to keep clocking the internal clock even if disabled. */
                cycles = (m_t_left + this.sample_time()) * ds_clock;
                inc    = (int)cycles;
                m_t_left = (cycles - inc) / ds_clock;
                if (inc != 0) x_time = m_t_left / this.sample_time();
            }
            else
            {
                clock  = (int)ds_clock;
                /* x_time from input clock */
                x_time = ds_clock - clock;
            }


            /* If reset enabled then set output to the reset value.  No x_time in reset. */
            if (DSS_COUNTER__RESET != 0)
            {
                m_last_count = (uint32_t)(int)DSS_COUNTER__INIT;
                set_output(0, (int)DSS_COUNTER__INIT);
                return;
            }

            /*
             * Only count if module is enabled.
             * This has the effect of holding the output at it's current value.
             */
            if (DSS_COUNTER__ENABLE != 0)
            {
                double v_out;

                switch (m_clock_type)
                {
                    case g.DISC_CLK_ON_F_EDGE:
                    case g.DISC_CLK_ON_R_EDGE:
                        /* See if the clock has toggled to the proper edge */
                        clock = (clock != 0) ? 1 : 0;
                        if (m_last_clock != clock)
                        {
                            m_last_clock = clock;
                            if (m_clock_type == clock)
                            {
                                /* Toggled */
                                inc = 1;
                            }
                        }
                        break;

                    case g.DISC_CLK_BY_COUNT:
                        /* Clock number of times specified. */
                        inc = clock;
                        break;
                }

                /* use loops because init is not always min or max */
                if (DSS_COUNTER__DIR != 0)
                {
                    count = (uint32_t)(count + inc);
                    while (count > m_max)
                    {
                        count -= m_diff;
                    }
                }
                else
                {
                    count = (uint32_t)(count - inc);
                    while (count < m_min || count > (0xffffffff - inc))
                    {
                        count += m_diff;
                    }
                }

                m_last_count = count;
                v_out = m_is_7492 != 0 ? (double)disc_7492_count[count] : (double)count;

                if (count != last_count)
                {
                    /* the x_time is only output if the output changed. */
                    switch (m_out_type)
                    {
                        case g.DISC_OUT_HAS_XTIME:
                            v_out += x_time;
                            break;
                        case g.DISC_OUT_IS_ENERGY:
                            if (x_time == 0) x_time = 1.0;
                            v_out = last_count;
                            if (count > last_count)
                                v_out += (count - last_count) * x_time;
                            else
                                v_out -= (last_count - count) * x_time;
                            break;
                    }
                }

                set_output(0, v_out);
            }
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dss_lfsr_noise, 2,
    class discrete_dss_lfsr_noise_node : discrete_base_node,
                                         discrete_step_interface
    {
        const int _maxout = 2;


        //#define DSS_COUNTER__ENABLE     DISCRETE_INPUT(0)
        //#define DSS_COUNTER__RESET      DISCRETE_INPUT(1)
        double DSS_COUNTER__CLOCK { get { return DISCRETE_INPUT(2); } }
        //#define DSS_COUNTER__MIN        DISCRETE_INPUT(3)
        //#define DSS_COUNTER__MAX        DISCRETE_INPUT(4)
        //#define DSS_COUNTER__DIR        DISCRETE_INPUT(5)
        //#define DSS_COUNTER__INIT       DISCRETE_INPUT(6)
        //#define DSS_COUNTER__CLOCK_TYPE DISCRETE_INPUT(7)
        //#define DSS_7492__CLOCK_TYPE     DSS_COUNTER__MIN

        double DSS_LFSR_NOISE__ENABLE { get { return DISCRETE_INPUT(0); } }
        double DSS_LFSR_NOISE__RESET { get { return DISCRETE_INPUT(1); } }
        double DSS_LFSR_NOISE__CLOCK { get { return DISCRETE_INPUT(2); } }
        double DSS_LFSR_NOISE__AMP { get { return DISCRETE_INPUT(3); } }
        double DSS_LFSR_NOISE__FEED { get { return DISCRETE_INPUT(4); } }
        double DSS_LFSR_NOISE__BIAS { get { return DISCRETE_INPUT(5); } }


        UInt32 m_lfsr_reg;  //unsigned int    m_lfsr_reg;
        int m_last;                 /* Last clock state */
        double m_t_clock;              /* fixed counter clock in seconds */
        double m_t_left;               /* time unused during last sample in seconds */
        //double          m_sample_step;
        //double          m_t;
        uint8_t m_reset_on_high;
        uint8_t m_invert_output;
        uint8_t m_out_is_f0;
        uint8_t m_out_lfsr_reg;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dss_lfsr_noise_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dss_lfsr_noise_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dss_lfsr_noise)
        public override void reset()
        {
            discrete_lfsr_desc info = (discrete_lfsr_desc)this.custom_data();  //DISCRETE_DECLARE_INFO(discrete_lfsr_desc);

            int fb0;
            int fb1;
            int fbresult;
            double v_out;

            m_reset_on_high = ((info.flags & g.DISC_LFSR_FLAG_RESET_TYPE_H) != 0) ? (uint8_t)1 : (uint8_t)0;
            m_invert_output = (uint8_t)(info.flags & g.DISC_LFSR_FLAG_OUT_INVERT);
            m_out_is_f0 = ((info.flags & g.DISC_LFSR_FLAG_OUTPUT_F0) != 0) ? (uint8_t)1 : (uint8_t)0;
            m_out_lfsr_reg = ((info.flags & g.DISC_LFSR_FLAG_OUTPUT_SR_SN1) != 0) ? (uint8_t)1 : (uint8_t)0;

            if ((info.clock_type < g.DISC_CLK_ON_F_EDGE) || (info.clock_type > g.DISC_CLK_IS_FREQ))
                m_device.discrete_log("Invalid clock type passed in NODE_{0}\n", this.index());

            m_last = (DSS_COUNTER__CLOCK != 0) ? 1 : 0;
            if (info.clock_type == g.DISC_CLK_IS_FREQ) m_t_clock = 1.0 / DSS_LFSR_NOISE__CLOCK;
            m_t_left = 0;

            m_lfsr_reg = (UInt32)info.reset_value;

            /* Now get and store the new feedback result */
            /* Fetch the feedback bits */
            fb0 = (int)((m_lfsr_reg >> info.feedback_bitsel0) & 0x01);
            fb1 = (int)((m_lfsr_reg >> info.feedback_bitsel1) & 0x01);
            /* Now do the combo on them */
            fbresult = dss_lfsr_function(m_device, info.feedback_function0, fb0, fb1, 0x01);
            m_lfsr_reg = (UInt32)dss_lfsr_function(m_device, g.DISC_LFSR_REPLACE, (int)m_lfsr_reg, fbresult << info.bitlength, (2 << info.bitlength ) - 1);

            /* Now select and setup the output bit */
            v_out = (m_lfsr_reg >> info.output_bit) & 0x01;

            /* Final inversion if required */
            if ((info.flags & g.DISC_LFSR_FLAG_OUT_INVERT) != 0) v_out = (v_out != 0) ? 0 : 1;

            /* Gain stage */
            v_out = (v_out != 0) ? DSS_LFSR_NOISE__AMP / 2 : -DSS_LFSR_NOISE__AMP / 2;
            /* Bias input as required */
            v_out += DSS_LFSR_NOISE__BIAS;

            set_output(0, v_out);
            set_output(1, 0);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dss_lfsr_noise)
        public void step()
        {
            discrete_lfsr_desc info = (discrete_lfsr_desc)this.custom_data();  //DISCRETE_DECLARE_INFO(discrete_lfsr_desc);

            double cycles;
            int clock;
            int inc = 0;
            int fb0;
            int fb1;
            int fbresult = 0;
            int noise_feed;

            if (info.clock_type == g.DISC_CLK_IS_FREQ)
            {
                /* We need to keep clocking the internal clock even if disabled. */
                cycles = (m_t_left + this.sample_time()) / m_t_clock;
                inc    = (int)cycles;
                m_t_left = (cycles - inc) * m_t_clock;
            }

            /* Reset everything if necessary */
            if (((DSS_LFSR_NOISE__RESET == 0) ? 0 : 1) == m_reset_on_high)
            {
                this.reset();
                return;
            }

            switch (info.clock_type)
            {
                case g.DISC_CLK_ON_F_EDGE:
                case g.DISC_CLK_ON_R_EDGE:
                    /* See if the clock has toggled to the proper edge */
                    clock = (DSS_LFSR_NOISE__CLOCK != 0) ? 1 : 0;
                    if (m_last != clock)
                    {
                        m_last = clock;
                        if (info.clock_type == clock)
                        {
                            /* Toggled */
                            inc = 1;
                        }
                    }
                    break;

                case g.DISC_CLK_BY_COUNT:
                    /* Clock number of times specified. */
                    inc = (int)DSS_LFSR_NOISE__CLOCK;
                    break;
            }

            if (inc > 0)
            {
                double v_out;

                noise_feed = (DSS_LFSR_NOISE__FEED != 0 ? 0x01 : 0x00);
                for (clock = 0; clock < inc; clock++)
                {
                    /* Fetch the last feedback result */
                    fbresult = (int)((m_lfsr_reg >> info.bitlength) & 0x01);

                    /* Stage 2 feedback combine fbresultNew with infeed bit */
                    fbresult = dss_lfsr_function(m_device, info.feedback_function1, fbresult, noise_feed, 0x01);

                    /* Stage 3 first we setup where the bit is going to be shifted into */
                    fbresult = fbresult * info.feedback_function2_mask;
                    /* Then we left shift the register, */
                    m_lfsr_reg = m_lfsr_reg << 1;
                    /* Now move the fbresult into the shift register and mask it to the bitlength */
                    m_lfsr_reg = (UInt32)dss_lfsr_function(m_device, info.feedback_function2, fbresult, (int)m_lfsr_reg, (1 << info.bitlength) - 1 );

                    /* Now get and store the new feedback result */
                    /* Fetch the feedback bits */
                    fb0 = (int)((m_lfsr_reg >> info.feedback_bitsel0) & 0x01);
                    fb1 = (int)((m_lfsr_reg >> info.feedback_bitsel1) & 0x01);
                    /* Now do the combo on them */
                    fbresult = dss_lfsr_function(m_device, info.feedback_function0, fb0, fb1, 0x01);
                    m_lfsr_reg = (UInt32)dss_lfsr_function(m_device, g.DISC_LFSR_REPLACE, (int)m_lfsr_reg, fbresult << info.bitlength, (2 << info.bitlength) - 1);

                }
                /* Now select the output bit */
                if (m_out_is_f0 != 0)
                    v_out = fbresult & 0x01;
                else
                    v_out = (m_lfsr_reg >> info.output_bit) & 0x01;

                /* Final inversion if required */
                if (m_invert_output != 0) v_out = v_out != 0 ? 0 : 1;

                /* Gain stage */
                v_out = v_out != 0 ? DSS_LFSR_NOISE__AMP / 2 : -DSS_LFSR_NOISE__AMP / 2;
                /* Bias input as required */
                v_out = v_out + DSS_LFSR_NOISE__BIAS;

                set_output(0, v_out);

                /* output the lfsr reg ?*/
                if (m_out_lfsr_reg != 0)
                    set_output(1, (double) m_lfsr_reg);
            }

            if (DSS_LFSR_NOISE__ENABLE == 0)
            {
                set_output(0, 0);
            }
        }


        static int dss_lfsr_function(discrete_device dev, int myfunc, int in0, int in1, int bitmask)
        {
            int retval;

            in0 &= bitmask;
            in1 &= bitmask;

            switch (myfunc)
            {
                case g.DISC_LFSR_XOR:
                    retval = in0 ^ in1;
                    break;
                case g.DISC_LFSR_OR:
                    retval = in0 | in1;
                    break;
                case g.DISC_LFSR_AND:
                    retval = in0 & in1;
                    break;
                case g.DISC_LFSR_XNOR:
                    retval = in0 ^ in1;
                    retval = retval ^ bitmask;  /* Invert output */
                    break;
                case g.DISC_LFSR_NOR:
                    retval = in0 | in1;
                    retval = retval ^ bitmask;  /* Invert output */
                    break;
                case g.DISC_LFSR_NAND:
                    retval = in0 & in1;
                    retval = retval ^ bitmask;  /* Invert output */
                    break;
                case g.DISC_LFSR_IN0:
                    retval = in0;
                    break;
                case g.DISC_LFSR_IN1:
                    retval = in1;
                    break;
                case g.DISC_LFSR_NOT_IN0:
                    retval = in0 ^ bitmask;
                    break;
                case g.DISC_LFSR_NOT_IN1:
                    retval = in1 ^ bitmask;
                    break;
                case g.DISC_LFSR_REPLACE:
                    retval = in0 & ~in1;
                    retval = retval | in1;
                    break;
                case g.DISC_LFSR_XOR_INV_IN0:
                    retval = in0 ^ bitmask; /* invert in0 */
                    retval = retval ^ in1;  /* xor in1 */
                    break;
                case g.DISC_LFSR_XOR_INV_IN1:
                    retval = in1 ^ bitmask; /* invert in1 */
                    retval = retval ^ in0;  /* xor in0 */
                    break;
                default:
                    dev.discrete_log("dss_lfsr_function - Invalid function type passed");
                    retval = 0;
                    break;
            }
            return retval;
        }
    }


#if false
    DISCRETE_CLASS_STEP_RESET(dss_noise, 2,
        double          m_phase;
    );
#endif


    //DISCRETE_CLASS_STEP_RESET(dss_note, 1,
    class discrete_dss_note_node : discrete_base_node,
                                   discrete_step_interface
    {
        const int _maxout = 1;


        //double DSS_LFSR_NOISE__BIAS { get { return DISCRETE_INPUT(5); } }
        double DSS_NOTE__ENABLE { get { return DISCRETE_INPUT(0); } }
        double DSS_NOTE__CLOCK { get { return DISCRETE_INPUT(1); } }
        double DSS_NOTE__DATA { get { return DISCRETE_INPUT(2); } }
        double DSS_NOTE__MAX1 { get { return DISCRETE_INPUT(3); } }
        double DSS_NOTE__MAX2 { get { return DISCRETE_INPUT(4); } }
        double DSS_NOTE__CLOCK_TYPE { get { return DISCRETE_INPUT(5); } }


        int             m_clock_type;
        int             m_out_type;
        int             m_last;                 /* Last clock state */
        double          m_t_clock;              /* fixed counter clock in seconds */
        double          m_t_left;               /* time unused during last sample in seconds */
        int             m_max1;                 /* Max 1 Count stored as int for easy use. */
        int             m_max2;                 /* Max 2 Count stored as int for easy use. */
        int             m_count1;               /* current count1 */
        int             m_count2;               /* current count2 */


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dss_note_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dss_note_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dss_note)
        public override void reset()
        {
            m_clock_type = (int)DSS_NOTE__CLOCK_TYPE & g.DISC_CLK_MASK;
            m_out_type   = (int)DSS_NOTE__CLOCK_TYPE & g.DISC_OUT_MASK;

            m_last    = (DSS_NOTE__CLOCK != 0) ? 1 : 0;
            m_t_left  = 0;
            m_t_clock = 1.0 / DSS_NOTE__CLOCK;

            m_count1 = (int)DSS_NOTE__DATA;
            m_count2 = 0;
            m_max1   = (int)DSS_NOTE__MAX1;
            m_max2   = (int)DSS_NOTE__MAX2;
            set_output(0, 0);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dss_note)
        public void step()
        {
            double  cycles;
            int     clock  = 0;
            int     last_count2;
            int     inc = 0;
            double  x_time = 0;
            double  v_out;

            if (m_clock_type == g.DISC_CLK_IS_FREQ)
            {
                /* We need to keep clocking the internal clock even if disabled. */
                cycles = (m_t_left + this.sample_time()) / m_t_clock;
                inc    = (int)cycles;
                m_t_left = (cycles - inc) * m_t_clock;
                if (inc != 0) x_time = m_t_left / this.sample_time();
            }
            else
            {
                /* separate clock info from x_time info. */
                clock = (int)DSS_NOTE__CLOCK;
                x_time = DSS_NOTE__CLOCK - clock;
            }

            if (DSS_NOTE__ENABLE != 0)
            {
                last_count2 = m_count2;

                switch (m_clock_type)
                {
                    case g.DISC_CLK_ON_F_EDGE:
                    case g.DISC_CLK_ON_R_EDGE:
                        /* See if the clock has toggled to the proper edge */
                        clock = (clock != 0) ? 1 : 0;
                        if (m_last != clock)
                        {
                            m_last = clock;
                            if (m_clock_type == clock)
                            {
                                /* Toggled */
                                inc = 1;
                            }
                        }
                        break;

                    case g.DISC_CLK_BY_COUNT:
                        /* Clock number of times specified. */
                        inc = clock;
                        break;
                }

                /* Count output as long as the data loaded is not already equal to max 1 count. */
                if (DSS_NOTE__DATA != DSS_NOTE__MAX1)
                {
                    for (clock = 0; clock < inc; clock++)
                    {
                        m_count1++;
                        if (m_count1 > m_max1)
                        {
                            /* Max 1 count reached.  Load Data into counter. */
                            m_count1  = (int)DSS_NOTE__DATA;
                            m_count2 += 1;
                            if (m_count2 > m_max2) m_count2 = 0;
                        }
                    }
                }

                v_out = m_count2;
                if (m_count2 != last_count2)
                {
                    /* the x_time is only output if the output changed. */
                    switch (m_out_type)
                    {
                        case g.DISC_OUT_IS_ENERGY:
                            if (x_time == 0) x_time = 1.0;
                            v_out = last_count2;
                            if (m_count2 > last_count2)
                                v_out += (m_count2 - last_count2) * x_time;
                            else
                                v_out -= (last_count2 - m_count2) * x_time;
                            break;
                        case g.DISC_OUT_HAS_XTIME:
                            v_out += x_time;
                            break;
                    }
                }
                set_output(0, v_out);
            }
            else
            {
                set_output(0, 0);
            }
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dss_squarewfix, 1,
    class discrete_dss_squarewfix_node : discrete_base_node,
                                         discrete_step_interface
    {
        const int _maxout = 1;


        double DSS_SQUAREWFIX__ENABLE { get { return DISCRETE_INPUT(0); } }
        double DSS_SQUAREWFIX__FREQ { get { return DISCRETE_INPUT(1); } }
        double DSS_SQUAREWFIX__AMP { get { return DISCRETE_INPUT(2); } }
        double DSS_SQUAREWFIX__DUTY { get { return DISCRETE_INPUT(3); } }
        double DSS_SQUAREWFIX__BIAS { get { return DISCRETE_INPUT(4); } }
        double DSS_SQUAREWFIX__PHASE { get { return DISCRETE_INPUT(5); } }


        int             m_flip_flop;
        double          m_sample_step;
        double          m_t_left;
        double          m_t_off;
        double          m_t_on;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dss_squarewfix_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dss_squarewfix_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dss_squarewfix)
        public override void reset()
        {
            m_sample_step = 1.0 / this.sample_rate();
            m_flip_flop   = 1;

            /* Do the intial time shift and convert freq to off/on times */
            m_t_off   = 1.0 / DSS_SQUAREWFIX__FREQ; /* cycle time */
            m_t_left  = DSS_SQUAREWFIX__PHASE / 360.0;  /* convert start phase to % */
            m_t_left  = m_t_left - (int)m_t_left;   /* keep % between 0 & 1 */
            m_t_left  = (m_t_left < 0) ? 1.0 + m_t_left : m_t_left; /* if - then flip to + phase */
            m_t_left *= m_t_off;
            m_t_on    = m_t_off * (DSS_SQUAREWFIX__DUTY / 100.0);
            m_t_off  -= m_t_on;

            m_t_left = -m_t_left;

            /* toggle output and work out intial time shift */
            while (m_t_left <= 0)
            {
                m_flip_flop = (m_flip_flop != 0) ? 0 : 1;
                m_t_left   += (m_flip_flop != 0) ? m_t_on : m_t_off;
            }

            /* Step the output */
            this.step();
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dss_squarewfix)
        public void step()
        {
            m_t_left -= m_sample_step;

            /* The enable input only curtails output, phase rotation still occurs */
            while (m_t_left <= 0)
            {
                m_flip_flop = (m_flip_flop != 0) ? 0 : 1;
                m_t_left   += (m_flip_flop != 0) ? m_t_on : m_t_off;
            }

            if (DSS_SQUAREWFIX__ENABLE != 0)
            {
                /* Add gain and DC Bias component */

                m_t_off  = 1.0 / DSS_SQUAREWFIX__FREQ;  /* cycle time */
                m_t_on   = m_t_off * (DSS_SQUAREWFIX__DUTY / 100.0);
                m_t_off -= m_t_on;

                set_output(0, ((m_flip_flop != 0) ? DSS_SQUAREWFIX__AMP / 2.0 : -(DSS_SQUAREWFIX__AMP / 2.0)) + DSS_SQUAREWFIX__BIAS);
            }
            else
            {
                set_output(0, 0);
            }
        }
    }


    /* Component specific modules */

    //class DISCRETE_CLASS_NAME(dss_inverter_osc): public discrete_base_node, public discrete_step_interface
    public class discrete_dss_inverter_osc_node : discrete_base_node,
                                                  discrete_step_interface

    {
        const int DSS_INV_TAB_SIZE    = 500;
        public static double [] DEFAULT_CD40XX_VALUES(double _vB) { return new double [] { _vB, _vB * 0.02, _vB * 0.98, _vB / 5.0 * 1.5, _vB / 5.0 * 3.5, 0.1 }; }


        public struct description
        {
            public double  vB;
            public double  vOutLow;
            public double  vOutHigh;
            public double  vInFall;    // voltage that triggers the gate input to go low (0V) on fall
            public double  vInRise;    // voltage that triggers the gate input to go high (vGate) on rise
            public double  clamp;      // voltage is clamped to -clamp ... vb+clamp if clamp>= 0;
            public int     options;    // bitmapped options

            public description(double [] values, int options)
            {
                assert(values.Length == 6);
                this.vB       = values[0];
                this.vOutLow  = values[1];
                this.vOutHigh = values[2];
                this.vInFall  = values[3];
                this.vInRise  = values[4];
                this.clamp    = values[5];
                this.options  = options;
            }
        }

        //enum
        //{
        public const int IS_TYPE1 = 0x00;
        public const int IS_TYPE2 = 0x01;
        const int IS_TYPE3 = 0x02;
        const int IS_TYPE4 = 0x03;
        const int IS_TYPE5 = 0x04;
        const int TYPE_MASK = 0x0f;
        const int OUT_IS_LOGIC = 0x10;
        //}


        double I_ENABLE() { return m_input[0].m_pointer[0]; }  //DISCRETE_CLASS_INPUT(I_ENABLE,  0);
        double I_MOD() { return m_input[1].m_pointer[0]; }  //DISCRETE_CLASS_INPUT(I_MOD,     1);
        double I_RC() { return m_input[2].m_pointer[0]; }  //DISCRETE_CLASS_INPUT(I_RC,      2);
        double I_RP() { return m_input[3].m_pointer[0]; }  //DISCRETE_CLASS_INPUT(I_RP,      3);
        double I_C() { return m_input[4].m_pointer[0]; }  //DISCRETE_CLASS_INPUT(I_C,       4);
        double I_R2() { return m_input[5].m_pointer[0]; }  //DISCRETE_CLASS_INPUT(I_R2,      5);


        double          mc_v_cap;
        double          mc_v_g2_old;
        double          mc_w;
        double          mc_wc;
        double          mc_rp;
        double          mc_r1;
        double          mc_r2;
        double          mc_c;
        double          mc_tf_a;
        double          mc_tf_b;
        double [] mc_tf_tab = new double [DSS_INV_TAB_SIZE];


        //DISCRETE_CLASS_CONSTRUCTOR(dss_inverter_osc, base)
        public discrete_dss_inverter_osc_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(dss_inverter_osc)
        //~discrete_dss_inverter_osc_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dss_inverter_osc)
        public override void reset()
        {
            description info = (description)this.custom_data();  //DISCRETE_DECLARE_INFO(description)

            int i;

            /* exponent */
            mc_w  = Math.Exp(-this.sample_time() / (I_RC() * I_C()));
            mc_wc = Math.Exp(-this.sample_time() / ((I_RC() * I_RP()) / (I_RP() + I_RC()) * I_C()));
            set_output(0, 0);
            mc_v_cap    = 0;
            mc_v_g2_old = 0;
            mc_rp   = I_RP();
            mc_r1   = I_RC();
            mc_r2   = I_R2();
            mc_c    = I_C();
            mc_tf_b = (Math.Log(0.0 - Math.Log(info.vOutLow/info.vB)) - Math.Log(0.0 - Math.Log((info.vOutHigh/info.vB))) ) / Math.Log(info.vInRise / info.vInFall);
            mc_tf_a = Math.Log(0.0 - Math.Log(info.vOutLow/info.vB)) - mc_tf_b * Math.Log(info.vInRise/info.vB);
            mc_tf_a = Math.Exp(mc_tf_a);

            for (i = 0; i < DSS_INV_TAB_SIZE; i++)
            {
                mc_tf_tab[i] = this.tftab((double)i / (double)(DSS_INV_TAB_SIZE - 1) * info.vB);
            }
        }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dss_inverter_osc)
        public void step()
        {
            description info = (description)this.custom_data();  //DISCRETE_DECLARE_INFO(description)

            double diff = 0;
            double vG1 = 0;
            double vG2 = 0;
            double vG3 = 0;
            double vI;
            double vMix;
            double rMix;
            int clamped;
            double v_out;

            /* Get new state */
            vI = mc_v_cap + mc_v_g2_old;
            switch (info.options & TYPE_MASK)
            {
                case IS_TYPE1:
                case IS_TYPE3:
                    vG1 = this.tf(vI);
                    vG2 = this.tf(vG1);
                    vG3 = this.tf(vG2);
                    break;
                case IS_TYPE2:
                    vG1 = 0;
                    vG3 = this.tf(vI);
                    vG2 = this.tf(vG3);
                    break;
                case IS_TYPE4:
                    vI  = Math.Min(I_ENABLE(), vI + 0.7);
                    vG1 = 0;
                    vG3 = this.tf(vI);
                    vG2 = this.tf(vG3);
                    break;
                case IS_TYPE5:
                    vI  = Math.Max(I_ENABLE(), vI - 0.7);
                    vG1 = 0;
                    vG3 = this.tf(vI);
                    vG2 = this.tf(vG3);
                    break;
                default:
                    fatalerror("DISCRETE_INVERTER_OSC - Wrong type on NODE_{0}\n", this.index());
                    break;
            }

            clamped = 0;
            if (info.clamp >= 0.0)
            {
                if (vI < -info.clamp)
                {
                    vI = -info.clamp;
                    clamped = 1;
                }
                else if (vI > info.vB+info.clamp)
                {
                    vI = info.vB + info.clamp;
                    clamped = 1;
                }
            }

            switch (info.options & TYPE_MASK)
            {
                case IS_TYPE1:
                case IS_TYPE2:
                case IS_TYPE3:
                    if (clamped != 0)
                    {
                        double ratio = mc_rp / (mc_rp + mc_r1);
                        diff = vG3 * (ratio)
                                - (mc_v_cap + vG2)
                                + vI * (1.0 - ratio);
                        diff = diff - diff * mc_wc;
                    }
                    else
                    {
                        diff = vG3 - (mc_v_cap + vG2);
                        diff = diff - diff * mc_w;
                    }
                    break;
                case IS_TYPE4:
                    /*  FIXME handle r2 = 0  */
                    rMix = (mc_r1 * mc_r2) / (mc_r1 + mc_r2);
                    vMix = rMix* ((vG3 - vG2) / mc_r1 + (I_MOD() -vG2) / mc_r2);
                    if (vMix < (vI-vG2-0.7))
                    {
                        rMix = 1.0 / rMix + 1.0 / mc_rp;
                        rMix = 1.0 / rMix;
                        vMix = rMix* ( (vG3-vG2) / mc_r1 + (I_MOD() - vG2) / mc_r2
                                + (vI - 0.7 - vG2) / mc_rp);
                    }
                    diff = vMix - mc_v_cap;
                    diff = diff - diff * Math.Exp(-this.sample_time() / (mc_c * rMix));
                    break;
                case IS_TYPE5:
                    /*  FIXME handle r2 = 0  */
                    rMix = (mc_r1 * mc_r2) / (mc_r1 + mc_r2);
                    vMix = rMix* ((vG3 - vG2) / mc_r1 + (I_MOD() - vG2) / mc_r2);
                    if (vMix > (vI -vG2 + 0.7))
                    {
                        rMix = 1.0 / rMix + 1.0 / mc_rp;
                        rMix = 1.0 / rMix;
                        vMix = rMix * ( (vG3 - vG2) / mc_r1 + (I_MOD() - vG2) / mc_r2
                                + (vI + 0.7 - vG2) / mc_rp);
                    }
                    diff = vMix - mc_v_cap;
                    diff = diff - diff * Math.Exp(-this.sample_time()/(mc_c * rMix));
                    break;
                default:
                    fatalerror("DISCRETE_INVERTER_OSC - Wrong type on NODE_{0}\n", this.index());
                    break;
            }

            mc_v_cap   += diff;
            mc_v_g2_old = vG2;

            if ((info.options & TYPE_MASK) == IS_TYPE3)
                v_out = vG1;
            else
                v_out = vG3;

            if ((info.options & OUT_IS_LOGIC) != 0)
                v_out = (v_out > info.vInFall) ? 1 : 0;

            set_output(0, v_out);
        }


        //inline double DISCRETE_CLASS_FUNC(dss_inverter_osc, tftab)(double x)
        double tftab(double x)
        {
            description info = (description)this.custom_data();  //DISCRETE_DECLARE_INFO(description)

            x = x / info.vB;
            if (x > 0)
                return info.vB * Math.Exp(-mc_tf_a * Math.Pow(x, mc_tf_b));
            else
                return info.vB;
        }


        //inline double DISCRETE_CLASS_FUNC(dss_inverter_osc, tf)(double x)
        double tf(double x)
        {
            description info = (description)this.custom_data();  //DISCRETE_DECLARE_INFO(description)

            if (x < 0.0)
                return info.vB;
            else if (x <= info.vB)
                return mc_tf_tab[(int)((double)(DSS_INV_TAB_SIZE - 1) * x / info.vB)];
            else
                return mc_tf_tab[DSS_INV_TAB_SIZE - 1];
        }
    }
}
