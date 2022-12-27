// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;
using uint8_t = System.Byte;

using static mame.cpp_global;
using static mame.disc_mth_internal;
using static mame.discrete_global;
using static mame.emucore_global;
using static mame.rescap_global;


namespace mame
{
    //DISCRETE_CLASS_STEP(dst_adder, 1, /* no context */ );
    class discrete_dst_adder_node : discrete_base_node,
                                    discrete_step_interface
    {
        const int _maxout = 1;


        double DST_ADDER__ENABLE { get { return DISCRETE_INPUT(0); } }
        double DST_ADDER__IN0 { get { return DISCRETE_INPUT(1); } }
        double DST_ADDER__IN1 { get { return DISCRETE_INPUT(2); } }
        double DST_ADDER__IN2 { get { return DISCRETE_INPUT(3); } }
        double DST_ADDER__IN3 { get { return DISCRETE_INPUT(4); } }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_adder_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_adder_node() { }


        // discrete_base_node

        public override void reset() { step(); }
        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_adder)
        public void step()
        {
            if (DST_ADDER__ENABLE != 0)
            {
                set_output(0,  DST_ADDER__IN0 + DST_ADDER__IN1 + DST_ADDER__IN2 + DST_ADDER__IN3);
            }
            else
            {
                set_output(0, 0);
            }
        }
    }


    //DISCRETE_CLASS_STEP(dst_clamp, 1, /* no context */ );
    class discrete_dst_clamp_node : discrete_base_node,
                                    discrete_step_interface
    {
        const int _maxout = 1;


        double DST_CLAMP__IN { get { return DISCRETE_INPUT(0); } }
        double DST_CLAMP__MIN { get { return DISCRETE_INPUT(1); } }
        double DST_CLAMP__MAX { get { return DISCRETE_INPUT(2); } }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_clamp_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_clamp_node() { }


        // discrete_base_node

        public override void reset() { step(); }
        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_clamp)
        public void step()
        {
            if (DST_CLAMP__IN < DST_CLAMP__MIN)
                set_output(0,  DST_CLAMP__MIN);
            else if (DST_CLAMP__IN > DST_CLAMP__MAX)
                set_output(0,  DST_CLAMP__MAX);
            else
                set_output(0, DST_CLAMP__IN);
        }
    }


    //DISCRETE_CLASS_STEP(dst_divide, 1, /* no context */ );
    class discrete_dst_divide_node : discrete_base_node,
                                     discrete_step_interface
    {
        const int _maxout = 1;


        double DST_DIVIDE__ENABLE { get { return DISCRETE_INPUT(0); } }
        double DST_DIVIDE__IN { get { return DISCRETE_INPUT(1); } }
        double DST_DIVIDE__DIV { get { return DISCRETE_INPUT(2); } }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_divide_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_divide_node() { }


        // discrete_base_node

        public override void reset() { step(); }
        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_divide)
        public void step()
        {
            if (DST_DIVIDE__ENABLE != 0)
            {
                if (DST_DIVIDE__DIV == 0)
                {
                    set_output(0, double.MaxValue); /* Max out but don't break */
                    m_device.discrete_log("dst_divider_step() - Divide by Zero attempted in NODE_{0}.\n", this.index());
                }
                else
                {
                    set_output(0, DST_DIVIDE__IN / DST_DIVIDE__DIV);
                }
            }
            else
            {
                set_output(0, 0);
            }
        }
    }


    //DISCRETE_CLASS_STEP(dst_gain, 1, /* no context */ );
    class discrete_dst_gain_node : discrete_base_node,
                                   discrete_step_interface
    {
        const int _maxout = 1;


        double DST_GAIN__IN { get { return DISCRETE_INPUT(0); } }
        double DST_GAIN__GAIN { get { return DISCRETE_INPUT(1); } }
        double DST_GAIN__OFFSET { get { return DISCRETE_INPUT(2); } }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_gain_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_gain_node() { }


        // discrete_base_node

        public override void reset() { step(); }
        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_gain)
        public void step()
        {
            set_output(0, DST_GAIN__IN * DST_GAIN__GAIN + DST_GAIN__OFFSET);
        }
    }


    static partial class disc_mth_internal
    {
        public static int dst_trigger_function(int trig0, int trig1, int trig2, int function)
        {
            int result = 1;
            switch (function)
            {
                case DISC_OP_AMP_TRIGGER_FUNCTION_TRG0:
                    result = trig0;
                    break;
                case DISC_OP_AMP_TRIGGER_FUNCTION_TRG0_INV:
                    result = trig0 == 0 ? 1 : 0;
                    break;
                case DISC_OP_AMP_TRIGGER_FUNCTION_TRG1:
                    result = trig1;
                    break;
                case DISC_OP_AMP_TRIGGER_FUNCTION_TRG1_INV:
                    result = trig1 == 0 ? 1 : 0;
                    break;
                case DISC_OP_AMP_TRIGGER_FUNCTION_TRG2:
                    result = trig2;
                    break;
                case DISC_OP_AMP_TRIGGER_FUNCTION_TRG2_INV:
                    result = trig2 == 0 ? 1 : 0;
                    break;
                case DISC_OP_AMP_TRIGGER_FUNCTION_TRG01_AND:
                    result = trig0 != 0 && trig1 != 0 ? 1 : 0;
                    break;
                case DISC_OP_AMP_TRIGGER_FUNCTION_TRG01_NAND:
                    result = (!(trig0 != 0 && trig1 != 0)) ? 1 : 0;
                    break;
            }

            return result;
        }
    }


    //DISCRETE_CLASS_STEP(dst_logic_inv, 1, /* no context */ );
    class discrete_dst_logic_inv_node : discrete_base_node,
                                        discrete_step_interface
    {
        const int _maxout = 1;


        double DST_LOGIC_INV__IN { get { return DISCRETE_INPUT(0); } }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_logic_inv_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_logic_inv_node() { }


        // discrete_base_node

        public override void reset() { step(); }
        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_logic_inv)
        public void step()
        {
            set_output(0,  DST_LOGIC_INV__IN != 0 ? 0.0 : 1.0);
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_bits_decode, 8,
    class discrete_dst_bits_decode_node : discrete_base_node,
                                          discrete_step_interface
    {
        const int _maxout = 8;


        double DST_BITS_DECODE__IN { get { return DISCRETE_INPUT(0); } }
        double DST_BITS_DECODE__FROM { get { return DISCRETE_INPUT(1); } }
        double DST_BITS_DECODE__TO { get { return DISCRETE_INPUT(2); } }
        double DST_BITS_DECODE__VOUT { get { return DISCRETE_INPUT(3); } }


        int             m_count = 0;
        int             m_decode_x_time = 0;
        int             m_from = 0;
        int             m_last_val = 0;
        int             m_last_had_x_time = 0;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_bits_decode_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_bits_decode_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_bits_decode)
        public override void reset()
        {
            m_from = (int)DST_BITS_DECODE__FROM;
            m_count = (int)DST_BITS_DECODE__TO - m_from + 1;
            if (DST_BITS_DECODE__VOUT == 0)
                m_decode_x_time = 1;
            else
                m_decode_x_time = 0;

            m_last_had_x_time = 0;

            this.step();
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_bits_decode)
        public void step()
        {
            int new_val = (int)DST_BITS_DECODE__IN;
            int last_val = m_last_val;
            int last_had_x_time = m_last_had_x_time;

            if (last_val != new_val || last_had_x_time != 0)
            {
                int i;
                int new_bit;
                int last_bit;
                int last_bit_had_x_time;
                int bit_changed;
                double x_time = DST_BITS_DECODE__IN - new_val;
                int from = m_from;
                int count = m_count;
                int decode_x_time = m_decode_x_time;
                int has_x_time = x_time > 0 ? 1 : 0;
                double out_ = 0;
                double v_out = DST_BITS_DECODE__VOUT;

                for (i = 0; i < count; i++ )
                {
                    new_bit = (new_val >> (i + from)) & 1;
                    last_bit = (last_val >> (i + from)) & 1;
                    last_bit_had_x_time = (last_had_x_time >> (i + from)) & 1;
                    bit_changed = last_bit != new_bit ? 1 : 0;

                    if (!(bit_changed != 0) && !(last_bit_had_x_time != 0))
                        continue;

                    if (decode_x_time != 0)
                    {
                        out_ = new_bit;
                        if (bit_changed != 0)
                            out_ += x_time;
                    }
                    else
                    {
                        out_ = v_out;
                        if ((has_x_time != 0) && (bit_changed != 0))
                        {
                            if (new_bit != 0)
                                out_ *= x_time;
                            else
                                out_ *= (1.0 - x_time);
                        }
                        else
                        {
                            out_ *= new_bit;
                        }
                    }

                    set_output(i, out_);
                    if ((has_x_time != 0) && (bit_changed != 0))
                        /* set */
                        m_last_had_x_time |= 1 << (i + from);
                    else
                        /* clear */
                        m_last_had_x_time &= ~(1 << (i + from));
                }

                m_last_val = new_val;
            }
        }
    }


    //DISCRETE_CLASS_STEP(dst_logic_and, 1, /* no context */ );
    class discrete_dst_logic_and_node : discrete_base_node,
                                        discrete_step_interface
    {
        const int _maxout = 1;


        double DST_LOGIC_AND__IN0 { get { return DISCRETE_INPUT(0); } }
        double DST_LOGIC_AND__IN1 { get { return DISCRETE_INPUT(1); } }
        double DST_LOGIC_AND__IN2 { get { return DISCRETE_INPUT(2); } }
        double DST_LOGIC_AND__IN3 { get { return DISCRETE_INPUT(3); } }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_logic_and_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_logic_and_node() { }


        // discrete_base_node

        public override void reset() { step(); }
        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_logic_and)
        public void step()
        {
            set_output(0, (DST_LOGIC_AND__IN0 != 0 && DST_LOGIC_AND__IN1 != 0 && DST_LOGIC_AND__IN2 != 0 && DST_LOGIC_AND__IN3 != 0) ? 1.0 : 0.0);
        }
    }


    //DISCRETE_CLASS_STEP(dst_logic_nand, 1, /* no context */ );
    class discrete_dst_logic_nand_node : discrete_base_node,
                                         discrete_step_interface
    {
        const int _maxout = 1;


        bool DST_LOGIC_NAND__IN0 { get { return DISCRETE_INPUT(0) != 0; } }
        bool DST_LOGIC_NAND__IN1 { get { return DISCRETE_INPUT(1) != 0; } }
        bool DST_LOGIC_NAND__IN2 { get { return DISCRETE_INPUT(2) != 0; } }
        bool DST_LOGIC_NAND__IN3 { get { return DISCRETE_INPUT(3) != 0; } }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_logic_nand_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_logic_nand_node() { }


        // discrete_base_node

        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_logic_nand)
        public void step()
        {
            set_output(0, (DST_LOGIC_NAND__IN0 && DST_LOGIC_NAND__IN1 && DST_LOGIC_NAND__IN2 && DST_LOGIC_NAND__IN3) ? 0.0 : 1.0);
        }
    }


    //DISCRETE_CLASS_STEP(dst_logic_or, 1, /* no context */ );

    //DISCRETE_CLASS_STEP(dst_logic_nor, 1, /* no context */ );

    //DISCRETE_CLASS_STEP(dst_logic_xor, 1, /* no context */ );

    //DISCRETE_CLASS_STEP(dst_logic_nxor, 1, /* no context */ );


    //DISCRETE_CLASS_STEP_RESET(dst_logic_dff, 1,
    class discrete_dst_logic_dff_node : discrete_base_node,
                                        discrete_step_interface
    {
        const int _maxout = 1;


        bool DST_LOGIC_DFF__RESET { get { return DISCRETE_INPUT(0) == 0; } }
        bool DST_LOGIC_DFF__SET { get { return DISCRETE_INPUT(1) == 0; } }
        double DST_LOGIC_DFF__CLOCK { get { return DISCRETE_INPUT(2); } }
        double DST_LOGIC_DFF__DATA { get { return DISCRETE_INPUT(3); } }


        int             m_last_clk = 0;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_logic_dff_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_logic_dff_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_logic_dff)
        public override void reset()
        {
            m_last_clk = 0;
            set_output(0, 0);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_logic_dff)
        public void step()
        {
            int clk = (int)DST_LOGIC_DFF__CLOCK;

            if (DST_LOGIC_DFF__RESET)
                set_output(0, 0);
            else if (DST_LOGIC_DFF__SET)
                set_output(0, 1);
            else if (m_last_clk == 0 && clk != 0)    /* low to high */
                set_output(0, DST_LOGIC_DFF__DATA);

            m_last_clk = clk;
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_logic_jkff, 1,
    //double          m_v_out = 0;
    //int             m_last_clk = 0;
    //);

    //DISCRETE_CLASS_STEP_RESET(dst_logic_shift, 1,
    //double          m_t_left = 0.0;             /* time unused during last sample in seconds */
    //uint32_t        m_shift_data = 0;
    //uint32_t        m_bit_mask = 0;
    //uint8_t         m_clock_type = 0;
    //uint8_t         m_reset_on_high = 0;
    //uint8_t         m_shift_r = 0;
    //uint8_t         m_last = 0;
    //);

    //DISCRETE_CLASS_STEP(dst_lookup_table, 1, /* no context */ );

    //DISCRETE_CLASS_STEP_RESET(dst_multiplex, 1,
    //int             m_size = 0;
    //);


    //DISCRETE_CLASS_STEP_RESET(dst_oneshot, 1,
    class discrete_dst_oneshot_node : discrete_base_node,
                                      discrete_step_interface
    {
        const int _maxout = 1;


        double DST_ONESHOT__RESET { get { return DISCRETE_INPUT(0); } }
        double DST_ONESHOT__TRIG { get { return DISCRETE_INPUT(1); } }
        double DST_ONESHOT__AMP { get { return DISCRETE_INPUT(2); } }
        double DST_ONESHOT__WIDTH { get { return DISCRETE_INPUT(3); } }
        int DST_ONESHOT__TYPE { get { return (int)DISCRETE_INPUT(4); } }


        double m_countdown = 0.0;
        int m_state = 0;
        int m_last_trig = 0;
        int m_type = 0;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_oneshot_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_oneshot_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_oneshot)
        public override void reset()
        {
            m_countdown = 0;
            m_state     = 0;

            m_last_trig = 0;
            m_type = DST_ONESHOT__TYPE;

            set_output(0,  (m_type & DISC_OUT_ACTIVE_LOW) != 0 ? DST_ONESHOT__AMP : 0);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_oneshot)
        public void step()
        {
            int trigger = (DST_ONESHOT__TRIG != 0) ? 1 : 0;

            /* If the state is triggered we will need to countdown later */
            int do_count = m_state;

            if (DST_ONESHOT__RESET != 0)
            {
                /* Hold in Reset */
                set_output(0, 0);
                m_state  = 0;
            }
            else
            {
                /* are we at an edge? */
                if (trigger != m_last_trig)
                {
                    /* There has been a trigger edge */
                    m_last_trig = trigger;

                    /* Is it the proper edge trigger */
                    if ((m_type & DISC_ONESHOT_REDGE) != 0 ? (trigger != 0) : (trigger == 0))
                    {
                        if (m_state == 0)
                        {
                            /* We have first trigger */
                            m_state     = 1;
                            set_output(0, (m_type & DISC_OUT_ACTIVE_LOW) != 0 ? 0 : DST_ONESHOT__AMP);
                            m_countdown = DST_ONESHOT__WIDTH;
                        }
                        else
                        {
                            /* See if we retrigger */
                            if ((m_type & DISC_ONESHOT_RETRIG) != 0)
                            {
                                /* Retrigger */
                                m_countdown = DST_ONESHOT__WIDTH;
                                do_count = 0;
                            }
                        }
                    }
                }

                if (do_count != 0)
                {
                    m_countdown -= this.sample_time();
                    if(m_countdown <= 0.0)
                    {
                        set_output(0, (m_type & DISC_OUT_ACTIVE_LOW) != 0 ? DST_ONESHOT__AMP : 0);
                        m_countdown = 0;
                        m_state     = 0;
                    }
                }
            }
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_ramp, 1,
    class discrete_dst_ramp_node : discrete_base_node,
                                   discrete_step_interface
    {
        const int _maxout = 1;


        double DST_RAMP__ENABLE { get { return DISCRETE_INPUT(0); } }
        double DST_RAMP__DIR { get { return DISCRETE_INPUT(1); } }
        double DST_RAMP__GRAD { get { return DISCRETE_INPUT(2); } }
        double DST_RAMP__START { get { return DISCRETE_INPUT(3); } }
        double DST_RAMP__END { get { return DISCRETE_INPUT(4); } }
        double DST_RAMP__CLAMP { get { return DISCRETE_INPUT(5); } }


        double          m_v_out = 0.0;
        double          m_step = 0.0;
        int             m_dir = 0;                  /* 1 if End is higher then Start */
        int             m_last_en = 0;              /* Keep track of the last enable value */


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_ramp_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_ramp_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_ramp)
        public override void reset()
        {
            m_v_out = DST_RAMP__CLAMP;
            m_step = DST_RAMP__GRAD / this.sample_rate();
            m_dir = ((DST_RAMP__END - DST_RAMP__START) == fabs(DST_RAMP__END - DST_RAMP__START)) ? 1 : 0;
            m_last_en = 0;
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_ramp)
        public void step()
        {
            if (DST_RAMP__ENABLE != 0)
            {
                if (m_last_en == 0)
                {
                    m_last_en = 1;
                    m_v_out = DST_RAMP__START;
                }

                if (m_dir != 0 ? DST_RAMP__DIR != 0 : DST_RAMP__DIR == 0) m_v_out += m_step;
                else m_v_out -= m_step;

                /* Clamp to min/max */
                if (m_dir != 0 ? (m_v_out < DST_RAMP__START)
                               : (m_v_out > DST_RAMP__START)) m_v_out = DST_RAMP__START;
                if (m_dir != 0 ? (m_v_out > DST_RAMP__END)
                               : (m_v_out < DST_RAMP__END)) m_v_out = DST_RAMP__END;
            }
            else
            {
                m_last_en = 0;
                /* Disabled so clamp to output */
                m_v_out = DST_RAMP__CLAMP;
            }

            set_output(0, m_v_out);
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_samphold, 1,
    class discrete_dst_samphold_node : discrete_base_node,
                                       discrete_step_interface
    {
        const int _maxout = 1;


        double DST_SAMPHOLD__IN0 { get { return DISCRETE_INPUT(0); } }
        double DST_SAMPHOLD__CLOCK { get { return DISCRETE_INPUT(1); } }
        double DST_SAMPHOLD__TYPE { get { return DISCRETE_INPUT(2); } }


        double          m_last_input = 0.0;
        int             m_clocktype = 0;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_samphold_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_logic_dff_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_samphold)
        public override void reset()
        {
            set_output(0, 0);
            m_last_input = -1;
            /* Only stored in here to speed up and save casting in the step function */
            m_clocktype = (int)DST_SAMPHOLD__TYPE;
            this.step();
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_samphold)
        public void step()
        {
            switch (m_clocktype)
            {
                case DISC_SAMPHOLD_REDGE:
                    /* Clock the whole time the input is rising */
                    if (DST_SAMPHOLD__CLOCK > m_last_input) set_output(0,  DST_SAMPHOLD__IN0);
                    break;
                case DISC_SAMPHOLD_FEDGE:
                    /* Clock the whole time the input is falling */
                    if(DST_SAMPHOLD__CLOCK < m_last_input) set_output(0,  DST_SAMPHOLD__IN0);
                    break;
                case DISC_SAMPHOLD_HLATCH:
                    /* Output follows input if clock != 0 */
                    if (DST_SAMPHOLD__CLOCK != 0) set_output(0,  DST_SAMPHOLD__IN0);
                    break;
                case DISC_SAMPHOLD_LLATCH:
                    /* Output follows input if clock == 0 */
                    if (DST_SAMPHOLD__CLOCK == 0) set_output(0,  DST_SAMPHOLD__IN0);
                    break;
                default:
                    m_device.discrete_log("dst_samphold_step - Invalid clocktype passed");
                    break;
            }

            /* Save the last value */
            m_last_input = DST_SAMPHOLD__CLOCK;
        }
    }


    //DISCRETE_CLASS_STEP(dst_switch, 1, /* no context */ );
    class discrete_dst_switch_node : discrete_base_node,
                                     discrete_step_interface
    {
        const int _maxout = 1;


        bool DST_SWITCH__ENABLE { get { return DISCRETE_INPUT(0) != 0; } }
        bool DST_SWITCH__SWITCH { get { return DISCRETE_INPUT(1) != 0; } }
        double DST_SWITCH__IN0 { get { return DISCRETE_INPUT(2); } }
        double DST_SWITCH__IN1 { get { return DISCRETE_INPUT(3); } }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_switch_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_switch_node() { }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set;  }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_switch)
        public void step()
        {
            if (DST_SWITCH__ENABLE)
            {
                set_output(0, DST_SWITCH__SWITCH ? DST_SWITCH__IN1 : DST_SWITCH__IN0);
            }
            else
            {
                set_output(0, 0);
            }
        }
    }


    //DISCRETE_CLASS_STEP(dst_aswitch, 1, /* no context */ );


    //class DISCRETE_CLASS_NAME(dst_transform): public discrete_base_node, public discrete_step_interface
    class discrete_dst_transform_node : discrete_base_node,
                                        discrete_step_interface
    {
        const int _maxout = 1;


        class double_stack
        {
            const int MAX_TRANS_STACK = 16;

            double [] stk = new double [MAX_TRANS_STACK];
            int pIdx = 0;  //double *p;


            public double_stack() { pIdx = 0; }  //{ p(&stk[0]) }

            public void push(double v)
            {
                //Store THEN increment
                assert(pIdx <= MAX_TRANS_STACK - 1);  //p <= &stk[MAX_TRANS_STACK-1]);
                stk[pIdx++] = v;  //*p++ = v;
            }
            
            public double pop()
            {
                //decrement THEN read
                assert(pIdx > 0);  //assert(p > &stk[0]);
                pIdx--;  //p--;
                return stk[pIdx];  //return *p;
            }
        }


        //#define DISCRETE_CLASS_INPUT(_name, _num)   inline double _name (void) { return *(m_input[_num]); }
        double I_IN0() { return m_input[0].m_pointer[0]; }  //DISCRETE_CLASS_INPUT(I_IN0,     0);
        double I_IN1() { return m_input[1].m_pointer[0]; }  //DISCRETE_CLASS_INPUT(I_IN1,     1);
        double I_IN2() { return m_input[2].m_pointer[0]; }  //DISCRETE_CLASS_INPUT(I_IN2,     2);
        double I_IN3() { return m_input[3].m_pointer[0]; }  //DISCRETE_CLASS_INPUT(I_IN3,     3);
        double I_IN4() { return m_input[4].m_pointer[0]; }  //DISCRETE_CLASS_INPUT(I_IN4,     4);


        enum token
        {
            TOK_END = 0,
            TOK_MULT,
            TOK_DIV,
            TOK_ADD,
            TOK_MINUS,
            TOK_0,
            TOK_1,
            TOK_2,
            TOK_3,
            TOK_4,
            TOK_DUP,
            TOK_ABS,         /* absolute value */
            TOK_NEG,         /* * -1 */
            TOK_NOT,         /* Logical NOT of Last Value */
            TOK_EQUAL,       /* Logical = */
            TOK_GREATER,     /* Logical > */
            TOK_LESS,        /* Logical < */
            TOK_AND,         /* Bitwise AND */
            TOK_OR,          /* Bitwise OR */
            TOK_XOR          /* Bitwise XOR */
        }


        token [] precomp = new token[32];  //enum token precomp[32]{ TOK_END };


        //DISCRETE_CLASS_CONSTRUCTOR(dst_transform, base)
        public discrete_dst_transform_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(dst_transform)
        //~discrete_dst_transform_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_transform)
        public override void reset()
        {
            string fPTR = (string)this.custom_data();  //const char *fPTR = (const char *)this->custom_data();
            int fPTRIdx = 0;
            token [] p = precomp;
            int pIdx = 0;  //enum token *p = &precomp[0];

            while (fPTRIdx != fPTR.Length)  //(*fPTR != 0)
            {
                switch (fPTR[fPTRIdx++])  //(*fPTR++)
                {
                    case '*':   p[pIdx] = token.TOK_MULT;      break;  // *p = TOK_MULT;      break;
                    case '/':   p[pIdx] = token.TOK_DIV;       break;
                    case '+':   p[pIdx] = token.TOK_ADD;       break;
                    case '-':   p[pIdx] = token.TOK_MINUS;     break;
                    case '0':   p[pIdx] = token.TOK_0;         break;
                    case '1':   p[pIdx] = token.TOK_1;         break;
                    case '2':   p[pIdx] = token.TOK_2;         break;
                    case '3':   p[pIdx] = token.TOK_3;         break;
                    case '4':   p[pIdx] = token.TOK_4;         break;
                    case 'P':   p[pIdx] = token.TOK_DUP;       break;
                    case 'a':   p[pIdx] = token.TOK_ABS;       break; /* absolute value */
                    case 'i':   p[pIdx] = token.TOK_NEG;       break; /* * -1 */
                    case '!':   p[pIdx] = token.TOK_NOT;       break; /* Logical NOT of Last Value */
                    case '=':   p[pIdx] = token.TOK_EQUAL;     break; /* Logical = */
                    case '>':   p[pIdx] = token.TOK_GREATER;   break; /* Logical > */
                    case '<':   p[pIdx] = token.TOK_LESS;      break; /* Logical < */
                    case '&':   p[pIdx] = token.TOK_AND;       break; /* Bitwise AND */
                    case '|':   p[pIdx] = token.TOK_OR;        break; /* Bitwise OR */
                    case '^':   p[pIdx] = token.TOK_XOR;       break; /* Bitwise XOR */
                    default:
                        m_device.discrete_log("dst_transform_step - Invalid function type/variable passed: {0}", (string)this.custom_data());
                        /* that is enough to fatalerror */
                        fatalerror("dst_transform_step - Invalid function type/variable passed: {0}\n", (string)this.custom_data());
                        break;
                }

                pIdx++;
            }

            p[pIdx] = token.TOK_END;
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_transform)
        public void step()
        {
            double_stack stack = new double_stack();
            double top;

            token [] fPTR = precomp;  //enum token *fPTR = &precomp[0];
            int fPTRIdx = 0;

            top = double.PositiveInfinity;  //top = HUGE_VAL;

            while (fPTR[fPTRIdx] != token.TOK_END)  //while (*fPTR != TOK_END)
            {
                switch (fPTR[fPTRIdx++])  //switch (*fPTR++)
                {
                    case token.TOK_MULT:      top = stack.pop() * top;                    break;
                    case token.TOK_DIV:       top = stack.pop() / top;                    break;
                    case token.TOK_ADD:       top = stack.pop() + top;                    break;
                    case token.TOK_MINUS:     top = stack.pop() - top;                    break;
                    case token.TOK_0:         stack.push(top); top = I_IN0();             break;
                    case token.TOK_1:         stack.push(top); top = I_IN1();             break;
                    case token.TOK_2:         stack.push(top); top = I_IN2();             break;
                    case token.TOK_3:         stack.push(top); top = I_IN3();             break;
                    case token.TOK_4:         stack.push(top); top = I_IN4();             break;
                    case token.TOK_DUP:       stack.push(top);                            break;
                    case token.TOK_ABS:       top = fabs(top);                            break;  /* absolute value */
                    case token.TOK_NEG:       top = -top;                                 break;  /* * -1 */
                    case token.TOK_NOT:       top = top == 0 ? 1 : 0;                     break;  /* Logical NOT of Last Value */
                    case token.TOK_EQUAL:     top = (int)stack.pop() == (int)top ? 1 : 0; break;  /* Logical = */
                    case token.TOK_GREATER:   top = (stack.pop() > top) ? 1 : 0;          break;  /* Logical > */
                    case token.TOK_LESS:      top = (stack.pop() < top) ? 1 : 0;          break;  /* Logical < */
                    case token.TOK_AND:       top = (int)stack.pop() & (int)top;          break;  /* Bitwise AND */
                    case token.TOK_OR:        top = (int)stack.pop() | (int)top;          break;  /* Bitwise OR */
                    case token.TOK_XOR:       top = (int)stack.pop() ^ (int)top;          break;  /* Bitwise XOR */
                    case token.TOK_END:       break; /* please compiler */
                }
            }

            set_output(0, top);
        }
    }


    /* Component specific */

    //DISCRETE_CLASS_STEP_RESET(dst_comp_adder, 1,
    class discrete_dst_comp_adder_node : discrete_base_node,
                                         discrete_step_interface
    {
        const int _maxout = 1;


        double DST_COMP_ADDER__SELECT { get { return DISCRETE_INPUT(0); } }


        double [] m_total = new double[256];


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)
        public discrete_dst_comp_adder_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)
        //~discrete_dst_comp_adder_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_comp_adder)
        public override void reset()
        {
            //DISCRETE_DECLARE_INFO(discrete_comp_adder_table)
            discrete_comp_adder_table info = (discrete_comp_adder_table)custom_data();

            int i;
            int bit;
            int bit_length = info.length;

            assert(bit_length <= 8);

            /* pre-calculate all possible values to speed up step routine */
            for (i = 0; i < 256; i++)
            {
                switch (info.type)
                {
                    case DISC_COMP_P_CAPACITOR:
                        m_total[i] = info.cDefault;
                        for(bit = 0; bit < bit_length; bit++)
                        {
                            if ((i & (1 << bit)) != 0)
                                m_total[i] += info.c[bit];
                        }
                        break;
                    case DISC_COMP_P_RESISTOR:
                        m_total[i] = (info.cDefault != 0) ? 1.0 / info.cDefault : 0;
                        for(bit = 0; bit < bit_length; bit++)
                        {
                            if (((i & (1 << bit)) != 0) && (info.c[bit] != 0))
                                m_total[i] += 1.0 / info.c[bit];
                        }

                        if (m_total[i] != 0)
                            m_total[i] = 1.0 / m_total[i];

                        break;
                }
            }

            set_output(0, m_total[0]);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_comp_adder)
        public void step()
        {
            int select;

            select = (int)DST_COMP_ADDER__SELECT;
            assert(select < 256);
            set_output(0, m_total[select]);
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_dac_r1, 1,
    class discrete_dst_dac_r1_node : discrete_base_node,
                                     discrete_step_interface
    {
        const int _maxout = 1;

        /************************************************************************
         *
         * DST_DAC_R1 - R1 Ladder DAC with cap smoothing
         *
         * input[0]    - Binary Data Input
         * input[1]    - Data On Voltage (3.4 for TTL)
         *
         * also passed discrete_dac_r1_ladder structure
         *
         * Mar 2004, D Renaud.
         * Nov 2010, D Renaud. - optimized for speed
         ************************************************************************/
        //#define DST_DAC_R1__DATA        DISCRETE_INPUT(0)
        //#define DST_DAC_R1__VON         DISCRETE_INPUT(1)
        public double DST_DAC_R1__DATA() { return DISCRETE_INPUT(0); }
        public double DST_DAC_R1__VON() { return DISCRETE_INPUT(1); }


        double m_v_out = 0.0;
        double m_exponent = 0.0;
        double m_last_v = 0.0;
        double [] m_v_step = new double[256];
        int m_has_c_filter = 0;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)
        public discrete_dst_dac_r1_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)
        //~discrete_dst_dac_r1_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_dac_r1)
        public override void reset()
        {
            //DISCRETE_DECLARE_INFO(discrete_dac_r1_ladder)
            discrete_dac_r1_ladder info = (discrete_dac_r1_ladder)custom_data();

            int bit;
            int ladderLength = info.ladderLength;
            int total_steps = 1 << ladderLength;
            double r_total = 0;
            double i_bias;
            double v_on = DST_DAC_R1__VON();

            m_last_v = 0;

            /* Calculate the Millman current of the bias circuit */
            if (info.rBias > 0)
                i_bias = info.vBias / info.rBias;
            else
                i_bias = 0;

            /*
             * We will do a small amount of error checking.
             * But if you are an idiot and pass a bad ladder table
             * then you deserve a crash.
             */
            if (ladderLength < 2 && info.rBias == 0 && info.rGnd == 0)
            {
                /* You need at least 2 resistors for a ladder */
                m_device.discrete_log("dst_dac_r1_reset - Ladder length too small");
            }

            if (ladderLength > DISC_LADDER_MAXRES)
            {
                m_device.discrete_log("dst_dac_r1_reset - Ladder length exceeds DISC_LADDER_MAXRES");
            }

            /*
             * Calculate the total of all resistors in parallel.
             * This is the combined resistance of the voltage sources.
             * This is used for the charging curve.
             */
            for (bit = 0; bit < ladderLength; bit++)
            {
                if (info.r[bit] > 0)
                    r_total += 1.0 / info.r[bit];
            }

            if (info.rBias > 0) r_total += 1.0 / info.rBias;
            if (info.rGnd > 0)  r_total += 1.0 / info.rGnd;

            r_total = 1.0 / r_total;

            m_v_out = 0;

            if (info.cFilter > 0)
            {
                m_has_c_filter = 1;
                /* Setup filter constant */
                m_exponent = RC_CHARGE_EXP(r_total * info.cFilter);
            }
            else
            {
                m_has_c_filter = 0;
            }

            /* pre-calculate all possible values to speed up step routine */
            for (int i = 0; i < total_steps; i++)
            {
                double i_total = i_bias;
                for (bit = 0; bit < ladderLength; bit++)
                {
                    /* Add up currents of ON circuits per Millman. */

                    /* ignore if no resistor present */
                    if (info.r[bit] > 0)
                    {
                        double i_bit;
                        int bit_val = (i >> bit) & 0x01;

                        if (bit_val != 0)
                            i_bit   = v_on / info.r[bit];
                        else
                            i_bit = 0;
                        i_total += i_bit;
                    }
                }
                m_v_step[i] = i_total * r_total;
            }
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_dac_r1)
        public void step()
        {
            int     data = (int)DST_DAC_R1__DATA();
            double  v = m_v_step[data];
            double  x_time = DST_DAC_R1__DATA() - data;
            double  last_v = m_last_v;

            m_last_v = v;

            if (x_time > 0)
                v = x_time * (v - last_v) + last_v;

            /* Filter if needed, else just output voltage */
            if (m_has_c_filter != 0)
            {
                double v_diff = v - m_v_out;
                /* optimization - if charged close enough to voltage */
                if (fabs(v_diff) < 0.000001)
                {
                    m_v_out = v;
                }
                else
                {
                    m_v_out += v_diff * m_exponent;
                }
            }
            else
            {
                m_v_out = v;
            }

            set_output(0, m_v_out);
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_diode_mix, 1,
    class discrete_dst_diode_mix_node : discrete_base_node,
                                        discrete_step_interface
    {
        const int _maxout = 1;


        const int DST_DIODE_MIX_INP_OFFSET    = 0;
        double DST_DIODE_MIX__INP(int addr) { return DISCRETE_INPUT(DST_DIODE_MIX_INP_OFFSET + addr); }


        int m_size = 0;
        double [] m_v_junction = new double [8];


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)
        public discrete_dst_diode_mix_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)
        //~discrete_dst_diode_mix_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_diode_mix)
        public override void reset()
        {
            double [] info = (double [])this.custom_data();  //DISCRETE_DECLARE_INFO(double)
            int infoIdx = 0;

            int     addr;

            m_size = this.active_inputs() - DST_DIODE_MIX_INP_OFFSET;
            assert(m_size <= 8);

            for (addr = 0; addr < m_size; addr++)
            {
                if (info == null)
                {
                    /* setup default junction voltage */
                    m_v_junction[addr] = 0.5;
                }
                else
                {
                    /* use supplied junction voltage */
                    m_v_junction[addr] = info[infoIdx++];
                }
            }

            this.step();
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_diode_mix)
        public void step()
        {
            double val;
            double max = 0;
            int    addr;

            for (addr = 0; addr < m_size; addr++)
            {
                val = DST_DIODE_MIX__INP(addr) - m_v_junction[addr];
                if (val > max) max = val;
            }

            if (max < 0) max = 0;
            set_output(0,  max);
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_integrate, 1,
    //    double          m_v_out = 0.0;
    //    double          m_change = 0.0;
    //    double          m_v_max_in = 0.0;       /* v1 - norton VBE */
    //    double          m_v_max_in_d = 0.0;     /* v1 - norton VBE - diode drop */
    //    double          m_v_max_out = 0.0;
    //);


    //DISCRETE_CLASS_STEP_RESET(dst_mixer, 1,
    class discrete_dst_mixer_node : discrete_base_node,
                                    discrete_step_interface
    {
        const int _maxout = 1;

        const int DISC_MIXER_MAX_INPS = 8;


        /************************************************************************
         *
         * DST_MIXER  - Mixer/Gain stage
         *
         * input[0]    - Enable input value
         * input[1]    - Input 1
         * input[2]    - Input 2
         * input[3]    - Input 3
         * input[4]    - Input 4
         * input[5]    - Input 5
         * input[6]    - Input 6
         * input[7]    - Input 7
         * input[8]    - Input 8
         *
         * Also passed discrete_mixer_info structure
         *
         * Mar 2004, D Renaud.
         ************************************************************************/
        /*
         * The input resistors can be a combination of static values and nodes.
         * If a node is used then its value is in series with the static value.
         * Also if a node is used and its value is 0, then that means the
         * input is disconnected from the circuit.
         *
         * There are 3 basic types of mixers, defined by the 2 types.  The
         * op amp mixer is further defined by the prescence of rI.  This is a
         * brief explanation.
         *
         * DISC_MIXER_IS_RESISTOR
         * The inputs are high pass filtered if needed, using (rX || rF) * cX.
         * Then Millman is used for the voltages.
         * r = (1/rF + 1/r1 + 1/r2...)
         * i = (v1/r1 + v2/r2...)
         * v = i * r
         *
         * DISC_MIXER_IS_OP_AMP - no rI
         * This is just a summing circuit.
         * The inputs are high pass filtered if needed, using rX * cX.
         * Then a modified Millman is used for the voltages.
         * i = ((vRef - v1)/r1 + (vRef - v2)/r2...)
         * v = i * rF
         *
         * DISC_MIXER_IS_OP_AMP_WITH_RI
         * The inputs are high pass filtered if needed, using (rX + rI) * cX.
         * Then Millman is used for the voltages including vRef/rI.
         * r = (1/rI + 1/r1 + 1/r2...)
         * i = (vRef/rI + v1/r1 + v2/r2...)
         * The voltage is then modified by an inverting amp formula.
         * v = vRef + (rF/rI) * (vRef - (i * r))
         */
        //#define DST_MIXER__ENABLE       DISCRETE_INPUT(0)
        //#define DST_MIXER__IN(bit)      DISCRETE_INPUT(bit + 1)
        public double DST_MIXER__ENABLE() { return DISCRETE_INPUT(0); }
        public double DST_MIXER__IN(int bit) { return DISCRETE_INPUT(bit + 1); }


        int m_type = 0;
        int m_size = 0;
        int m_r_node_bit_flag = 0;
        int m_c_bit_flag = 0;
        double m_r_total = 0.0;
        Pointer<double> [] m_r_node = new Pointer<double> [DISC_MIXER_MAX_INPS];  //const double *  m_r_node[DISC_MIXER_MAX_INPS];      /* Either pointer to resistance node output OR NULL */
        double [] m_r_last = new double[DISC_MIXER_MAX_INPS];
        double [] m_exponent_rc = new double[DISC_MIXER_MAX_INPS]; /* For high pass filtering cause by cIn */
        double [] m_v_cap = new double[DISC_MIXER_MAX_INPS];       /* cap voltage of each input */
        double m_exponent_c_f = 0.0;         /* Low pass on mixed inputs */
        double m_exponent_c_amp = 0.0;       /* Final high pass caused by out cap and amp input impedance */
        double m_v_cap_f = 0.0;              /* cap voltage of cF */
        double m_v_cap_amp = 0.0;            /* cap voltage of cAmp */
        double m_gain = 0.0;                 /* used for DISC_MIXER_IS_OP_AMP_WITH_RI */


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)
        public discrete_dst_mixer_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)
        //~discrete_dst_mixer_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_mixer)
        public override void reset()
        {
            //DISCRETE_DECLARE_INFO(discrete_mixer_desc)
            discrete_mixer_desc info = (discrete_mixer_desc)custom_data();


            int     bit;
            double  rTemp = 0;

            /* link to r_node outputs */
            m_r_node_bit_flag = 0;

            for (bit = 0; bit < 8; bit++)
            {
                Pointer<double> node_output_ptr = m_device.node_output_ptr(info.r_node[bit]);
                m_r_node[bit] = node_output_ptr != null ? new Pointer<double>(node_output_ptr) : null;
                if (m_r_node[bit] != null)
                {
                    m_r_node_bit_flag |= 1 << bit;
                }

                /* flag any caps */
                if (info.c[bit] != 0)
                    m_c_bit_flag |= 1 << bit;
            }

            m_size = active_inputs() - 1;

            /*
             * THERE IS NO ERROR CHECKING!!!!!!!!!
             * If you pass a bad ladder table
             * then you deserve a crash.
             */

            m_type = info.type;
            if ((info.type == DISC_MIXER_IS_OP_AMP) && (info.rI != 0))
                m_type = DISC_MIXER_IS_OP_AMP_WITH_RI;

            /*
             * Calculate the total of all resistors in parallel.
             * This is the combined resistance of the voltage sources.
             * Also calculate the exponents while we are here.
             */
            m_r_total = 0;
            for (bit = 0; bit < m_size; bit++)
            {
                if ((info.r[bit] != 0) && info.r_node[bit] == 0)
                {
                    m_r_total += 1.0 / info.r[bit];
                }

                m_v_cap[bit]       = 0;
                m_exponent_rc[bit] = 0;
                if (info.c[bit] != 0 && info.r_node[bit] == 0)
                {
                    switch (m_type)
                    {
                        case DISC_MIXER_IS_RESISTOR:
                            /* is there an rF? */
                            if (info.rF != 0)
                            {
                                rTemp = 1.0 / ((1.0 / info.r[bit]) + (1.0 / info.rF));
                                break;
                            }
                            /* else, fall through and just use the resistor value */
                            goto case DISC_MIXER_IS_OP_AMP;  //[[fallthrough]];

                        case DISC_MIXER_IS_OP_AMP:
                            rTemp = info.r[bit];
                            break;

                        case DISC_MIXER_IS_OP_AMP_WITH_RI:
                            rTemp = info.r[bit] + info.rI;
                            break;
                    }

                    /* Setup filter constants */
                    m_exponent_rc[bit] = RC_CHARGE_EXP(rTemp * info.c[bit]);
                }
            }

            if (info.rF != 0)
            {
                if (m_type == DISC_MIXER_IS_RESISTOR) m_r_total += 1.0 / info.rF;
            }

            if (m_type == DISC_MIXER_IS_OP_AMP_WITH_RI) m_r_total += 1.0 / info.rI;

            m_v_cap_f      = 0;
            m_exponent_c_f = 0;
            if (info.cF != 0)
            {
                /* Setup filter constants */
                m_exponent_c_f = RC_CHARGE_EXP(((info.type == DISC_MIXER_IS_OP_AMP) ? info.rF : (1.0 / m_r_total)) * info.cF);
            }

            m_v_cap_amp      = 0;
            m_exponent_c_amp = 0;
            if (info.cAmp != 0)
            {
                /* Setup filter constants */
                /* We will use 100k ohms as an average final stage impedance. */
                /* Your amp/speaker system will have more effect on incorrect filtering then any value used here. */
                m_exponent_c_amp = RC_CHARGE_EXP(RES_K(100) * info.cAmp);
            }

            if (m_type == DISC_MIXER_IS_OP_AMP_WITH_RI) m_gain = info.rF / info.rI;

            set_output(0, 0);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_mixer)
        public void step()
        {
            //DISCRETE_DECLARE_INFO(discrete_mixer_desc)
            discrete_mixer_desc info = (discrete_mixer_desc)custom_data();


            double  v;
            double  vTemp;
            double  r_total;
            double  rTemp;
            double  rTemp2 = 0;
            double  i = 0;      /* total current of inputs */
            int     bit;
            int     connected;

            /* put commonly used stuff in local variables for speed */
            int     r_node_bit_flag = m_r_node_bit_flag;
            int     c_bit_flag = m_c_bit_flag;
            int     bit_mask = 1;
            int     has_rF = info.rF != 0 ? 1 : 0;
            int     type = m_type;
            double  v_ref = info.vRef;
            double  rI = info.rI;

            if (DST_MIXER__ENABLE() != 0)
            {
                r_total = m_r_total;

                if (m_r_node_bit_flag != 0)
                {
                    /* loop and do any high pass filtering for connected caps */
                    /* but first see if there is an r_node for the current path */
                    /* if so, then the exponents need to be re-calculated */
                    for (bit = 0; bit < m_size; bit++)
                    {
                        rTemp     = info.r[bit];
                        connected = 1;
                        vTemp     = DST_MIXER__IN(bit);

                        /* is there a resistor? */
                        if ((r_node_bit_flag & bit_mask) != 0)
                        {
                            /* a node has the possibility of being disconnected from the circuit. */
                            if (m_r_node[bit][0] == 0)
                            {
                                connected = 0;
                            }
                            else
                            {
                                /* value currently holds resistance */
                                rTemp   += m_r_node[bit][0];
                                r_total += 1.0 / rTemp;
                                /* is there a capacitor? */
                                if ((c_bit_flag & bit_mask) != 0)
                                {
                                    switch (type)
                                    {
                                        case DISC_MIXER_IS_RESISTOR:
                                            /* is there an rF? */
                                            if (has_rF != 0)
                                            {
                                                rTemp2 = RES_2_PARALLEL(rTemp, info.rF);
                                                break;
                                            }
                                            /* else, fall through and just use the resistor value */
                                            goto case DISC_MIXER_IS_OP_AMP;  //[[fallthrough]];

                                        case DISC_MIXER_IS_OP_AMP:
                                            rTemp2 = rTemp;
                                            break;

                                        case DISC_MIXER_IS_OP_AMP_WITH_RI:
                                            rTemp2 = rTemp + rI;
                                            break;
                                    }

                                    /* Re-calculate exponent if resistor is a node and has changed value */
                                    if (m_r_node[bit][0] != m_r_last[bit])
                                    {
                                        m_exponent_rc[bit] = RC_CHARGE_EXP(rTemp2 * info.c[bit]);
                                        m_r_last[bit] = m_r_node[bit][0];
                                    }
                                }
                            }
                        }

                        if (connected != 0)
                        {
                            /* is there a capacitor? */
                            if ((c_bit_flag & bit_mask) != 0)
                            {
                                /* do input high pass filtering if needed. */
                                m_v_cap[bit] += (vTemp - v_ref - m_v_cap[bit]) * m_exponent_rc[bit];
                                vTemp -= m_v_cap[bit];
                            }

                            i += ((type == DISC_MIXER_IS_OP_AMP) ? v_ref - vTemp : vTemp) / rTemp;
                        }

                        bit_mask = bit_mask << 1;
                    }
                }
                else if (c_bit_flag != 0)
                {
                    /* no r_nodes, so just do high pass filtering */
                    for (bit = 0; bit < m_size; bit++)
                    {
                        vTemp = DST_MIXER__IN(bit);

                        if ((c_bit_flag & (1 << bit)) != 0)
                        {
                            /* do input high pass filtering if needed. */
                            m_v_cap[bit] += (vTemp - v_ref - m_v_cap[bit]) * m_exponent_rc[bit];
                            vTemp -= m_v_cap[bit];
                        }

                        i += ((type == DISC_MIXER_IS_OP_AMP) ? v_ref - vTemp : vTemp) / info.r[bit];
                    }
                }
                else
                {
                    /* no r_nodes or c_nodes, mixing only */
                    if (type == DISC_MIXER_IS_OP_AMP)
                    {
                        for (bit = 0; bit < m_size; bit++)
                            i += ( v_ref - DST_MIXER__IN(bit) ) / info.r[bit];
                    }
                    else
                    {
                        for (bit = 0; bit < m_size; bit++)
                            i += DST_MIXER__IN(bit) / info.r[bit];
                    }
                }

                if (type == DISC_MIXER_IS_OP_AMP_WITH_RI)
                    i += v_ref / rI;

                r_total = 1.0 / r_total;

                /* If resistor network or has rI then Millman is used.
                 * If op-amp then summing formula is used. */
                v = i * ((type == DISC_MIXER_IS_OP_AMP) ? info.rF : r_total);

                if (type == DISC_MIXER_IS_OP_AMP_WITH_RI)
                    v = v_ref + (m_gain * (v_ref - v));

                /* Do the low pass filtering for cF */
                if (info.cF != 0)
                {
                    if (r_node_bit_flag != 0)
                    {
                        /* Re-calculate exponent if resistor nodes are used */
                        m_exponent_c_f =  RC_CHARGE_EXP(r_total * info.cF);
                    }

                    m_v_cap_f += (v - v_ref - m_v_cap_f) * m_exponent_c_f;
                    v = m_v_cap_f;
                }

                /* Do the high pass filtering for cAmp */
                if (info.cAmp != 0)
                {
                    m_v_cap_amp += (v - m_v_cap_amp) * m_exponent_c_amp;
                    v -= m_v_cap_amp;
                }

                set_output(0,  v * info.gain);
            }
            else
            {
                set_output(0,  0);
            }
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_op_amp, 1,
    class discrete_dst_op_amp_node : discrete_base_node,
                                     discrete_step_interface
    {
        const int _maxout = 1;


        double DST_OP_AMP__ENABLE { get { return DISCRETE_INPUT(0); } }
        double DST_OP_AMP__INP0 { get { return DISCRETE_INPUT(1); } }
        double DST_OP_AMP__INP1 { get { return DISCRETE_INPUT(2); } }


        uint8_t m_has_cap = 0;
        uint8_t m_has_r1 = 0;
        uint8_t m_has_r4 = 0;
        double m_v_max = 0.0;
        double m_i_fixed = 0.0;
        double m_v_cap = 0.0;
        double m_exponent = 0.0;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)
        public discrete_dst_op_amp_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)
        //~discrete_dst_op_amp_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_op_amp)
        public override void reset()
        {
            //DISCRETE_DECLARE_INFO(discrete_op_amp_info)
            discrete_op_amp_info info = (discrete_op_amp_info)custom_data();

            m_has_r1 = info.r1 > 0 ? (uint8_t)1 : (uint8_t)0;
            m_has_r4 = info.r4 > 0 ? (uint8_t)1 : (uint8_t)0;

            m_v_max = info.vP - OP_AMP_NORTON_VBE;

            m_v_cap = 0;
            if (info.c > 0)
            {
                m_has_cap = 1;
                /* Setup filter constants */
                if (m_has_r4 != 0)
                {
                    /* exponential charge */
                    m_exponent = RC_CHARGE_EXP(info.r4 * info.c);
                }
                else
                {
                    /* linear charge */
                    m_exponent = this.sample_rate() * info.c;
                }
            }

            if (info.r3 > 0)
                m_i_fixed = (info.vP - OP_AMP_NORTON_VBE) / info.r3;
            else
                m_i_fixed = 0;
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_op_amp)
        public void step()
        {
            //DISCRETE_DECLARE_INFO(discrete_op_amp_info)
            discrete_op_amp_info info = (discrete_op_amp_info)custom_data();

            double i_pos = 0;
            double i_neg = 0;
            double i     = 0;
            double v_out;

            if (DST_OP_AMP__ENABLE != 0)
            {
                switch (info.type)
                {
                    case DISC_OP_AMP_IS_NORTON:
                        /* work out neg pin current */
                        if (m_has_r1 != 0)
                        {
                            i_neg = (DST_OP_AMP__INP0 - OP_AMP_NORTON_VBE) / info.r1;
                            if (i_neg < 0) i_neg = 0;
                        }
                        i_neg += m_i_fixed;

                        /* work out neg pin current */
                        i_pos = (DST_OP_AMP__INP1 - OP_AMP_NORTON_VBE) / info.r2;
                        if (i_pos < 0) i_pos = 0;

                        /* work out current across r4 */
                        i = i_pos - i_neg;

                        if (m_has_cap != 0)
                        {
                            if (m_has_r4 != 0)
                            {
                                /* voltage across r4 charging cap */
                                i *= info.r4;
                                /* exponential charge */
                                m_v_cap += (i - m_v_cap) * m_exponent;
                            }
                            else
                            {
                                /* linear charge */
                                m_v_cap += i / m_exponent;
                            }

                            v_out = m_v_cap;
                        }
                        else
                        {
                            if (m_has_r4 != 0)
                            {
                                v_out = i * info.r4;
                            }
                            else
                            {
                                /* output just swings to rail when there is no r4 */
                                if (i > 0)
                                    v_out = m_v_max;
                                else
                                    v_out = 0;
                            }
                        }

                        /* clamp output */
                        if (v_out > m_v_max) v_out = m_v_max;
                        else if (v_out < info.vN) v_out = info.vN;

                        m_v_cap = v_out;

                        set_output(0, v_out);
                        break;

                    default:
                        set_output(0, 0);
                        break;
                }
            }
            else
            {
                set_output(0, 0);
            }
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_op_amp_1sht, 1,
    class discrete_dst_op_amp_1sht_node : discrete_base_node,
                                          discrete_step_interface
    {
        const int _maxout = 1;


        double DST_OP_AMP_1SHT__TRIGGER { get { return DISCRETE_INPUT(0); } }


        double m_v_out = 0.0;
        double m_i_fixed = 0.0;
        double m_v_max = 0.0;
        double m_r34ratio = 0.0;
        double m_v_cap1 = 0.0;
        double m_v_cap2 = 0.0;
        double m_exponent1c = 0.0;
        double m_exponent1d = 0.0;
        double m_exponent2 = 0.0;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)
        public discrete_dst_op_amp_1sht_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)
        //~discrete_dst_op_amp_1sht_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_op_amp_1sht)
        public override void reset()
        {
            //DISCRETE_DECLARE_INFO(discrete_op_amp_1sht_info)
            discrete_op_amp_1sht_info info = (discrete_op_amp_1sht_info)custom_data();

            m_exponent1c = RC_CHARGE_EXP(RES_2_PARALLEL(info.r3, info.r4) * info.c1);
            m_exponent1d = RC_CHARGE_EXP(info.r4 * info.c1);
            m_exponent2  = RC_CHARGE_EXP(info.r2 * info.c2);
            m_i_fixed  = (info.vP - OP_AMP_NORTON_VBE) / info.r1;
            m_v_cap1   = m_v_cap2 = 0;
            m_v_max    = info.vP - OP_AMP_NORTON_VBE;
            m_r34ratio = info.r3 / (info.r3 + info.r4);
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_op_amp_1sht)
        public void step()
        {
            //DISCRETE_DECLARE_INFO(discrete_op_amp_1sht_info)
            discrete_op_amp_1sht_info info = (discrete_op_amp_1sht_info)custom_data();

            double i_pos;
            double i_neg;
            double v;

            /* update trigger circuit */
            i_pos  = (DST_OP_AMP_1SHT__TRIGGER - m_v_cap2) / info.r2;
            i_pos += m_v_out / info.r5;
            m_v_cap2 += (DST_OP_AMP_1SHT__TRIGGER - m_v_cap2) * m_exponent2;

            /* calculate currents and output */
            i_neg = (m_v_cap1 - OP_AMP_NORTON_VBE) / info.r3;
            if (i_neg < 0) i_neg = 0;
            i_neg += m_i_fixed;

            if (i_pos > i_neg) m_v_out = m_v_max;
            else m_v_out = info.vN;

            /* update c1 */
            /* rough value of voltage at anode of diode if discharging */
            v = m_v_out + 0.6;
            if (m_v_cap1 > m_v_out)
            {
                /* discharge */
                if (m_v_cap1 > v)
                    /* immediate discharge through diode */
                    m_v_cap1 = v;
                else
                    /* discharge through r4 */
                    m_v_cap1 += (m_v_out - m_v_cap1) * m_exponent1d;
            }
            else
            {
                /* charge */
                m_v_cap1 += ((m_v_out - OP_AMP_NORTON_VBE) * m_r34ratio + OP_AMP_NORTON_VBE - m_v_cap1) * m_exponent1c;
            }

            set_output(0, m_v_out);
        }
    }


    //DISCRETE_CLASS_STEP_RESET(dst_tvca_op_amp, 1,
    class discrete_dst_tvca_op_amp_node : discrete_base_node,
                                          discrete_step_interface
    {
        const int _maxout = 1;


        double DST_TVCA_OP_AMP__TRG0 { get { return DISCRETE_INPUT(0); } }
        double DST_TVCA_OP_AMP__TRG1 { get { return DISCRETE_INPUT(1); } }
        double DST_TVCA_OP_AMP__TRG2 { get { return DISCRETE_INPUT(2); } }
        double DST_TVCA_OP_AMP__INP0 { get { return DISCRETE_INPUT(3); } }
        double DST_TVCA_OP_AMP__INP1 { get { return DISCRETE_INPUT(4); } }


        double m_v_out_max = 0.0;            /* Maximum output voltage */
        double [] m_v_trig = new double [2];            /* Voltage used to charge cap1 based on function F3 */
        double m_v_trig2 = 0.0;              /* Voltage used to charge cap2 */
        double m_v_trig3 = 0.0;              /* Voltage used to charge cap3 */
        double m_i_fixed = 0.0;              /* Fixed current going into - input */
        double [] m_exponent_c = new double [2];        /* Charge exponents based on function F3 */
        double [] m_exponent_d = new double [2];        /* Discharge exponents based on function F3 */
        double [] m_exponent2 = new double [2];         /* Discharge/charge exponents based on function F4 */
        double [] m_exponent3 = new double [2];         /* Discharge/charge exponents based on function F5 */
        double m_exponent4 = 0.0;            /* Discharge/charge exponents for c4 */
        double m_v_cap1 = 0.0;               /* charge on cap c1 */
        double m_v_cap2 = 0.0;               /* charge on cap c2 */
        double m_v_cap3 = 0.0;               /* charge on cap c3 */
        double m_v_cap4 = 0.0;               /* charge on cap c4 */
        double m_r67 = 0.0;                  /* = r6 + r7 (for easy use later) */
        uint8_t m_has_c4 = 0;
        uint8_t m_has_r4 = 0;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)
        public discrete_dst_tvca_op_amp_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)
        //~discrete_dst_tvca_op_amp_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dst_tvca_op_amp)
        public override void reset()
        {
            //DISCRETE_DECLARE_INFO(discrete_op_amp_tvca_info)
            discrete_op_amp_tvca_info info = (discrete_op_amp_tvca_info)custom_data();

            m_r67 = info.r6 + info.r7;

            m_v_out_max = info.vP - OP_AMP_NORTON_VBE;
            /* This is probably overkill because R5 is usually much lower then r6 or r7,
             * but it is better to play it safe. */
            m_v_trig[0] = (info.v1 - 0.6) * RES_VOLTAGE_DIVIDER(info.r5, info.r6);
            m_v_trig[1] = (info.v1 - 0.6 - OP_AMP_NORTON_VBE) * RES_VOLTAGE_DIVIDER(info.r5, m_r67) + OP_AMP_NORTON_VBE;
            m_i_fixed   = m_v_out_max / info.r1;

            m_v_cap1 = 0;
            /* Charge rate through r5 */
            /* There can be a different charge rates depending on function F3. */
            m_exponent_c[0] = RC_CHARGE_EXP(RES_2_PARALLEL(info.r5, info.r6) * info.c1);
            m_exponent_c[1] = RC_CHARGE_EXP(RES_2_PARALLEL(info.r5, m_r67) * info.c1);
            /* Discharge rate through r6 + r7 */
            m_exponent_d[1] = RC_CHARGE_EXP(m_r67 * info.c1);
            /* Discharge rate through r6 */
            if (info.r6 != 0)
            {
                m_exponent_d[0] = RC_CHARGE_EXP(info.r6 * info.c1);
            }

            m_v_cap2       = 0;
            m_v_trig2      = (info.v2 - 0.6 - OP_AMP_NORTON_VBE) * RES_VOLTAGE_DIVIDER(info.r8, info.r9);
            m_exponent2[0] = RC_CHARGE_EXP(info.r9 * info.c2);
            m_exponent2[1] = RC_CHARGE_EXP(RES_2_PARALLEL(info.r8, info.r9) * info.c2);
            m_v_cap3       = 0;
            m_v_trig3      = (info.v3 - 0.6 - OP_AMP_NORTON_VBE) * RES_VOLTAGE_DIVIDER(info.r10, info.r11);
            m_exponent3[0] = RC_CHARGE_EXP(info.r11 * info.c3);
            m_exponent3[1] = RC_CHARGE_EXP(RES_2_PARALLEL(info.r10, info.r11) * info.c3);
            m_v_cap4       = 0;

            if (info.r4 != 0) m_has_r4 = 1;
            if (info.c4 != 0) m_has_c4 = 1;
            if (m_has_r4 != 0 && m_has_c4 != 0)
                m_exponent4    = RC_CHARGE_EXP(info.r4 * info.c4);

            this.step();
        }


        protected override int max_output() { return _maxout; }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_tvca_op_amp)
        public void step()
        {
            //DISCRETE_DECLARE_INFO(discrete_op_amp_tvca_info)
            discrete_op_amp_tvca_info info = (discrete_op_amp_tvca_info)custom_data();

            int trig0;
            int trig1;
            int trig2;
            int f3;
            double i2 = 0;     /* current through r2 */
            double i3 = 0;     /* current through r3 */
            double i_neg = 0;  /* current into - input */
            double i_pos = 0;  /* current into + input */
            double i_out = 0;  /* current at output */

            double v_out;

            trig0 = (int)DST_TVCA_OP_AMP__TRG0;
            trig1 = (int)DST_TVCA_OP_AMP__TRG1;
            trig2 = (int)DST_TVCA_OP_AMP__TRG2;
            f3 = dst_trigger_function(trig0, trig1, trig2, info.f3);

            if ((info.r2 != 0) && dst_trigger_function(trig0, trig1, trig2, info.f0) != 0)
            {
                /* r2 is present, so we assume Input 0 is connected and valid. */
                i2 = (DST_TVCA_OP_AMP__INP0 - OP_AMP_NORTON_VBE) / info.r2;
                if ( i2 < 0) i2 = 0;
            }

            if ((info.r3 != 0) && dst_trigger_function(trig0, trig1, trig2, info.f1) != 0)
            {
                /* r2 is present, so we assume Input 1 is connected and valid. */
                /* Function F1 is not grounding the circuit. */
                i3 = (DST_TVCA_OP_AMP__INP1 - OP_AMP_NORTON_VBE) / info.r3;
                if ( i3 < 0) i3 = 0;
            }

            /* Calculate current going in to - input. */
            i_neg = m_i_fixed + i2 + i3;

            /* Update the c1 cap voltage. */
            if (dst_trigger_function(trig0, trig1, trig2, info.f2) != 0)
            {
                /* F2 is not grounding the circuit so we charge the cap. */
                m_v_cap1 += (m_v_trig[f3] - m_v_cap1) * m_exponent_c[f3];
            }
            else
            {
                /* F2 is at ground.  The diode blocks this so F2 and r5 are out of circuit.
                 * So now the discharge rate is dependent upon F3.
                 * If F3 is at ground then we discharge to 0V through r6.
                 * If F3 is out of circuit then we discharge to OP_AMP_NORTON_VBE through r6+r7. */
                m_v_cap1 += ((f3 != 0 ? OP_AMP_NORTON_VBE : 0.0) - m_v_cap1) * m_exponent_d[f3];
            }

            /* Calculate c1 current going in to + input. */
            i_pos = (m_v_cap1 - OP_AMP_NORTON_VBE) / m_r67;
            if ((i_pos < 0) || f3 == 0) i_pos = 0;

            /* Update the c2 cap voltage and current. */
            if (info.r9 != 0)
            {
                f3 = dst_trigger_function(trig0, trig1, trig2, info.f4);
                m_v_cap2 += ((f3 != 0 ? m_v_trig2 : 0) - m_v_cap2) * m_exponent2[f3];
                i_pos += m_v_cap2 / info.r9;
            }

            /* Update the c3 cap voltage and current. */
            if (info.r11 != 0)
            {
                f3 = dst_trigger_function(trig0, trig1, trig2, info.f5);
                m_v_cap3 += ((f3 != 0 ? m_v_trig3 : 0) - m_v_cap3) * m_exponent3[f3];
                i_pos += m_v_cap3 / info.r11;
            }

            /* Calculate output current. */
            i_out = i_pos - i_neg;
            if (i_out < 0) i_out = 0;

            /* Convert to voltage for final output. */
            if (m_has_c4 != 0)
            {
                if (m_has_r4 != 0)
                {
                    /* voltage across r4 charging cap */
                    i_out *= info.r4;
                    /* exponential charge */
                    m_v_cap4 += (i_out - m_v_cap4) * m_exponent4;
                }
                else
                {
                    /* linear charge */
                    m_v_cap4 += i_out / m_exponent4;
                }

                if (m_v_cap4 < 0)
                    m_v_cap4 = 0;

                v_out = m_v_cap4;
            }
            else
            {
                v_out = i_out * info.r4;
            }

            /* Clip the output if needed. */
            if (v_out > m_v_out_max) v_out = m_v_out_max;

            set_output(0, v_out);
        }
    }


    //DISCRETE_CLASS_STEP(dst_xtime_buffer, 1, /* no context */ );
    class discrete_dst_xtime_buffer_node : discrete_base_node,
                                           discrete_step_interface
    {
        const int _maxout = 1;


        double DST_XTIME_BUFFER__IN { get { return DISCRETE_INPUT(0); } }
        double DST_XTIME_BUFFER_OUT_LOW { get { return DISCRETE_INPUT(1); } }
        double DST_XTIME_BUFFER_OUT_HIGH { get { return DISCRETE_INPUT(2); } }
        double DST_XTIME_BUFFER_INVERT { get { return DISCRETE_INPUT(3); } }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_xtime_buffer_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_xtime_buffer_node() { }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set;  }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_xtime_buffer)
        public void step()
        {
            int in0 = (int)DST_XTIME_BUFFER__IN;
            int out_ = in0;
            int out_is_energy = 1;

            double x_time = DST_XTIME_BUFFER__IN - in0;

            double out_low = DST_XTIME_BUFFER_OUT_LOW;
            double out_high = DST_XTIME_BUFFER_OUT_HIGH;

            if (out_low ==0 && out_high == 0)
                out_is_energy = 0;

            if (DST_XTIME_BUFFER_INVERT != 0)
                out_ ^= 1;

            if (out_is_energy != 0)
            {
                if (x_time > 0)
                {
                    double diff = out_high - out_low;
                    diff = out_ != 0 ? diff * x_time : diff * (1.0 - x_time);
                    set_output(0,  out_low + diff);
                }
                else
                {
                    set_output(0,  out_ != 0 ? out_high : out_low);
                }
            }
            else
            {
                set_output(0,  out_ + x_time);
            }
        }
    }


    //DISCRETE_CLASS_STEP(dst_xtime_and, 1, /* no context */ );
    class discrete_dst_xtime_and_node : discrete_base_node,
                                        discrete_step_interface
    {
        const int _maxout = 1;


        double DST_XTIME_AND__IN0 { get { return DISCRETE_INPUT(0); } }
        double DST_XTIME_AND__IN1 { get { return DISCRETE_INPUT(1); } }
        double DST_XTIME_AND_OUT_LOW { get { return DISCRETE_INPUT(2); } }
        double DST_XTIME_AND_OUT_HIGH { get { return DISCRETE_INPUT(3); } }
        double DST_XTIME_AND_INVERT { get { return DISCRETE_INPUT(4); } }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_xtime_and_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_xtime_and_node() { }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set;  }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_xtime_and)
        public void step()
        {
            int in0 = (int)DST_XTIME_AND__IN0;
            int in1 = (int)DST_XTIME_AND__IN1;
            int out_ = 0;
            int out_is_energy = 1;

            double x_time = 0;
            double x_time0 = DST_XTIME_AND__IN0 - in0;
            double x_time1 = DST_XTIME_AND__IN1 - in1;

            int in0_has_xtime = x_time0 > 0 ? 1 : 0;
            int in1_has_xtime = x_time1 > 0 ? 1 : 0;

            double out_low = DST_XTIME_AND_OUT_LOW;
            double out_high = DST_XTIME_AND_OUT_HIGH;

            if (out_low ==0 && out_high == 0)
                out_is_energy = 0;

            switch ((in0 << 3) | (in1 << 2) | ((in0_has_xtime < 1) ? 1 : 0) | in1_has_xtime)
            {
                // these are all 0
                //case XTIME__IN0_0__IN1_0__IN0_NOX__IN1_NOX:
                //case XTIME__IN0_0__IN1_1__IN0_NOX__IN1_NOX:
                //case XTIME__IN0_1__IN1_0__IN0_NOX__IN1_NOX:
                //case XTIME__IN0_0__IN1_0__IN0_NOX__IN1_X:
                //case XTIME__IN0_0__IN1_0__IN0_X__IN1_NOX:
                //case XTIME__IN0_0__IN1_1__IN0_NOX__IN1_X:
                //case XTIME__IN0_1__IN1_0__IN0_X__IN1_NOX:
                //  break;

                case XTIME__IN0_1__IN1_1__IN0_NOX__IN1_NOX:
                    out_ = 1;
                    break;

                case XTIME__IN0_0__IN1_1__IN0_X__IN1_NOX:
                    /*
                     * in0  1   ------
                     *      0         -------
                     *          ...^....^...
                     *
                     * in1  1   -------------
                     *      0
                     *          ...^....^...
                     *
                     * out  1   ------
                     *      0         ------
                     *          ...^....^...
                     */
                    x_time = x_time0;
                    break;

                case XTIME__IN0_1__IN1_0__IN0_NOX__IN1_X:
                    /*
                     * in0  1   -------------
                     *      0
                     *          ...^....^...
                     *
                     * in1  1   ------
                     *      0         -------
                     *          ...^....^...
                     *
                     * out  1   ------
                     *      0         ------
                     *          ...^....^...
                     */
                    x_time = x_time1;
                    break;

                case XTIME__IN0_0__IN1_0__IN0_X__IN1_X:
                    /*
                     * in0  1   -----              -------
                     *      0        --------             ------
                     *          ...^....^...       ...^....^...
                     *
                     * in1  1   -------            -----
                     *      0          ------           --------
                     *          ...^....^...       ...^....^...
                     *
                     * out  1   -----              -----
                     *      0        -------            -------
                     *          ...^....^...       ...^....^...
                     */
                    // use x_time of input that went to 0 first/longer
                    if (x_time0 >= x_time1)
                        x_time = x_time0;
                    else
                        x_time = x_time1;
                    break;

                case XTIME__IN0_0__IN1_1__IN0_X__IN1_X:
                    /*
                     * in0  1   -------           -----
                     *      0          -----           -------
                     *          ...^....^...      ...^....^...
                     *
                     * in1  1        -------             -----
                     *      0   -----             -------
                     *          ...^....^...      ...^....^...
                     *
                     * out  1        --
                     *      0   -----  -----      ------------
                     *          ...^....^...      ...^....^...
                     */
                    // may have went high for a bit in this cycle
                    //if (x_time0 < x_time1)
                    //  x_time = time1 - x_time0;
                    break;

                case XTIME__IN0_1__IN1_0__IN0_X__IN1_X:
                    /*
                     * in0  1        -------             -----
                     *      0   -----             -------
                     *          ...^....^...      ...^....^...
                     *
                     * in1  1   -------           -----
                     *      0          -----           -------
                     *          ...^....^...      ...^....^...
                     *
                     * out  1        --
                     *      0   -----  -----      ------------
                     *          ...^....^...      ...^....^...
                     */
                    // may have went high for a bit in this cycle
                    //if (x_time0 > x_time1)
                    //  x_time = x_time0 - x_time1;
                    break;

                case XTIME__IN0_1__IN1_1__IN0_NOX__IN1_X:
                    /*
                     * in0  1   ------------
                     *      0
                     *          ...^....^...
                     *
                     * in1  1         ------
                     *      0   ------
                     *          ...^....^...
                     *
                     * out  1         ------
                     *      0   ------
                     *          ...^....^...
                     */
                    out_ = 1;
                    x_time = x_time1;
                    break;

                case XTIME__IN0_1__IN1_1__IN0_X__IN1_NOX:
                    /*
                     * in1  0         ------
                     *      0   ------
                     *          ...^....^...
                     *
                     * in1  1   ------------
                     *      0
                     *          ...^....^...
                     *
                     * out  1         ------
                     *      0   ------
                     *          ...^....^...
                     */
                    out_ = 1;
                    x_time = x_time0;
                    break;

                case XTIME__IN0_1__IN1_1__IN0_X__IN1_X:
                    /*
                     * in0  1         ------          --------
                     *      0   ------            ----
                     *          ...^....^...      ...^....^...
                     *
                     * in1  1       --------            ------
                     *      0   ----              ------
                     *          ...^....^...      ...^....^...
                     *
                     * out  1         ------            ------
                     *      0   ------            ------
                     *          ...^....^...      ...^....^...
                     */
                    out_ = 1;
                    if (x_time0 < x_time1)
                        x_time = x_time0;
                    else
                        x_time = x_time1;
                    break;
            }

            if (DST_XTIME_AND_INVERT != 0)
                out_ ^= 1;

            if (out_is_energy != 0)
            {
                if (x_time > 0)
                {
                    double diff = out_high - out_low;
                    diff = out_ != 0 ? diff * x_time : diff * (1.0 - x_time);
                    set_output(0,  out_low + diff);
                }
                else
                {
                    set_output(0,  out_ != 0 ? out_high : out_low);
                }
            }
            else
            {
                set_output(0,  out_ + x_time);
            }
        }
    }


    //DISCRETE_CLASS_STEP(dst_xtime_or, 1, /* no context */ );
    class discrete_dst_xtime_or_node : discrete_base_node,
                                       discrete_step_interface
    {
        const int _maxout = 1;


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_xtime_or_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_xtime_or_node() { }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set;  }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_xtime_or)
        public void step()
        {
            throw new emu_unimplemented();
        }
    }


    //DISCRETE_CLASS_STEP(dst_xtime_xor, 1, /* no context */ );
    class discrete_dst_xtime_xor_node : discrete_base_node,
                                        discrete_step_interface
    {
        const int _maxout = 1;


        double DST_XTIME_XOR__IN0 { get { return DISCRETE_INPUT(0); } }
        double DST_XTIME_XOR__IN1 { get { return DISCRETE_INPUT(1); } }
        double DST_XTIME_XOR_OUT_LOW { get { return DISCRETE_INPUT(2); } }
        double DST_XTIME_XOR_OUT_HIGH { get { return DISCRETE_INPUT(3); } }
        double DST_XTIME_XOR_INVERT { get { return DISCRETE_INPUT(4); } }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dst_xtime_xor_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        //~discrete_dst_xtime_xor_node() { }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set;  }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dst_xtime_xor)
        public void step()
        {
            int in0 = (int)DST_XTIME_XOR__IN0;
            int in1 = (int)DST_XTIME_XOR__IN1;
            int out_ = 1;
            int out_is_energy = 1;

            double x_time = 0;
            double x_time0 = DST_XTIME_XOR__IN0 - in0;
            double x_time1 = DST_XTIME_XOR__IN1 - in1;

            int in0_has_xtime = x_time0 > 0 ? 1 : 0;
            int in1_has_xtime = x_time1 > 0 ? 1 : 0;

            double out_low = DST_XTIME_XOR_OUT_LOW;
            double out_high = DST_XTIME_XOR_OUT_HIGH;

            if (out_low ==0 && out_high == 0)
                out_is_energy = 0;

            switch ((in0 << 3) | (in1 << 2) | ((in0_has_xtime < 1) ? 1 : 0) | in1_has_xtime)
            {
                // these are all 1
                //case XTIME__IN0_0__IN1_1__IN0_NOX__IN1_NOX:
                //case XTIME__IN0_1__IN1_0__IN0_NOX__IN1_NOX:
                //  break;

                case XTIME__IN0_1__IN1_1__IN0_NOX__IN1_NOX:
                case XTIME__IN0_0__IN1_0__IN0_NOX__IN1_NOX:
                    out_ = 0;
                    break;

                case XTIME__IN0_1__IN1_0__IN0_X__IN1_NOX:
                    /*
                     * in0  1         ------
                     *      0   ------
                     *          ...^....^...
                     *
                     * in1  1
                     *      0   ------------
                     *          ...^....^...
                     *
                     * out  1         ------
                     *      0   ------
                     *          ...^....^...
                     */
                case XTIME__IN0_0__IN1_1__IN0_X__IN1_NOX:
                    /*
                     * in0  1   ------
                     *      0         -------
                     *          ...^....^...
                     *
                     * in1  1   -------------
                     *      0
                     *          ...^....^...
                     *
                     * out  1         ------
                     *      0   ------
                     *          ...^....^...
                     */
                    x_time = x_time0;
                    break;

                case XTIME__IN0_0__IN1_1__IN0_NOX__IN1_X:
                    /*
                     * in0  1
                     *      0   ------------
                     *          ...^....^...
                     *
                     * in1  1         ------
                     *      0   ------
                     *          ...^....^...
                     *
                     * out  1         ------
                     *      0   ------
                     *          ...^....^...
                     */
                case XTIME__IN0_1__IN1_0__IN0_NOX__IN1_X:
                    /*
                     * in0  1   -------------
                     *      0
                     *          ...^....^...
                     *
                     * in1  1   ------
                     *      0         -------
                     *          ...^....^...
                     *
                     * out  1         ------
                     *      0   ------
                     *          ...^....^...
                     */
                    x_time = x_time1;
                    break;

                case XTIME__IN0_0__IN1_0__IN0_X__IN1_NOX:
                    /*
                     * in0  1   ------
                     *      0         ------
                     *          ...^....^...
                     *
                     * in1  1
                     *      0   ------------
                     *          ...^....^...
                     *
                     * out  1   ------
                     *      0         ------
                     *          ...^....^...
                     */
                case XTIME__IN0_1__IN1_1__IN0_X__IN1_NOX:
                    /*
                     * in1  0         ------
                     *      0   ------
                     *          ...^....^...
                     *
                     * in1  1   ------------
                     *      0
                     *          ...^....^...
                     *
                     * out  1   ------
                     *      0         ------
                     *          ...^....^...
                     */
                    out_ = 0;
                    x_time = x_time0;
                    break;

                case XTIME__IN0_0__IN1_0__IN0_NOX__IN1_X:
                    /*
                     * in0  1
                     *      0   ------------
                     *          ...^....^...
                     *
                     * in1  1   ------
                     *      0         ------
                     *          ...^....^...
                     *
                     * out  1   ------
                     *      0         ------
                     *          ...^....^...
                     */
                case XTIME__IN0_1__IN1_1__IN0_NOX__IN1_X:
                    /*
                     * in0  1   ------------
                     *      0
                     *          ...^....^...
                     *
                     * in1  1         ------
                     *      0   ------
                     *          ...^....^...
                     *
                     * out  1   ------
                     *      0         ------
                     *          ...^....^...
                     */
                    out_ = 0;
                    x_time = x_time1;
                    break;

                case XTIME__IN0_0__IN1_0__IN0_X__IN1_X:
                    /*
                     * in0  1   -----              -------
                     *      0        -------              -----
                     *          ...^....^...       ...^....^...
                     *
                     * in1  1   -------            -----
                     *      0          -----            -------
                     *          ...^....^...       ...^....^...
                     *
                     * out  1        --                 --
                     *      0   -----  -----       -----  -----
                     *          ...^....^...       ...^....^...
                     */
                case XTIME__IN0_1__IN1_1__IN0_X__IN1_X:
                    /*
                     * in0  1         ------          --------
                     *      0   ------            ----
                     *          ...^....^...      ...^....^...
                     *
                     * in1  1       --------            ------
                     *      0   ----              ------
                     *          ...^....^...      ...^....^...
                     *
                     * out  1       --                --
                     *      0   ----  ------      ----  ------
                     *          ...^....^...      ...^....^...
                     */
                    out_ = 0;
                    /* Not sure if it is better to use 0
                     * or the total energy which would smear the switch points together.
                     * Let's try just using 0 */
                    // x_time = abs(x_time0 - x_time1);
                    break;

                case XTIME__IN0_0__IN1_1__IN0_X__IN1_X:
                    /*
                     * in0  1   -------           -----
                     *      0          -----           -------
                     *          ...^....^...      ...^....^...
                     *
                     * in1  1        -------             -----
                     *      0   -----             -------
                     *          ...^....^...      ...^....^...
                     *
                     * out  1   -----  -----      -----  -----
                     *      0        --                --
                     *          ...^....^...      ...^....^...
                     */
                case XTIME__IN0_1__IN1_0__IN0_X__IN1_X:
                    /*
                     * in0  1        -------             -----
                     *      0   -----             -------
                     *          ...^....^...      ...^....^...
                     *
                     * in1  1   -------           -----
                     *      0          -----           -------
                     *          ...^....^...      ...^....^...
                     *
                     * out  1   -----  -----      -----  -----
                     *      0        --                --
                     *          ...^....^...      ...^....^...
                     */
                    /* Not sure if it is better to use 1
                     * or the total energy which would smear the switch points together.
                     * Let's try just using 1 */
                    // x_time = 1.0 - abs(x_time0 - x_time1);
                    break;
            }

            if (DST_XTIME_XOR_INVERT != 0)
                out_ ^= 1;

            if (out_is_energy != 0)
            {
                if (x_time > 0)
                {
                    double diff = out_high - out_low;
                    diff = out_ != 0 ? diff * x_time : diff * (1.0 - x_time);
                    set_output(0,  out_low + diff);
                }
                else
                    set_output(0,  out_ != 0 ? out_high : out_low);
            }
            else
                set_output(0,  out_ + x_time);
        }
    }


    static partial class disc_mth_internal
    {
        /* the different logic and xtime states */
        //enum
        //{
            public const int XTIME__IN0_0__IN1_0__IN0_NOX__IN1_NOX =  0;
            public const int XTIME__IN0_0__IN1_0__IN0_NOX__IN1_X   =  1;
            public const int XTIME__IN0_0__IN1_0__IN0_X__IN1_NOX   =  2;
            public const int XTIME__IN0_0__IN1_0__IN0_X__IN1_X     =  3;
            const int XTIME__IN0_0__IN1_1__IN0_NOX__IN1_NOX =  4;
            public const int XTIME__IN0_0__IN1_1__IN0_NOX__IN1_X   =  5;
            public const int XTIME__IN0_0__IN1_1__IN0_X__IN1_NOX   =  6;
            public const int XTIME__IN0_0__IN1_1__IN0_X__IN1_X     =  7;
            const int XTIME__IN0_1__IN1_0__IN0_NOX__IN1_NOX =  8;
            public const int XTIME__IN0_1__IN1_0__IN0_NOX__IN1_X   =  9;
            public const int XTIME__IN0_1__IN1_0__IN0_X__IN1_NOX   = 10;
            public const int XTIME__IN0_1__IN1_0__IN0_X__IN1_X     = 11;
            public const int XTIME__IN0_1__IN1_1__IN0_NOX__IN1_NOX = 12;
            public const int XTIME__IN0_1__IN1_1__IN0_NOX__IN1_X   = 13;
            public const int XTIME__IN0_1__IN1_1__IN0_X__IN1_NOX   = 14;
            public const int XTIME__IN0_1__IN1_1__IN0_X__IN1_X     = 15;
        //};
    }
}
