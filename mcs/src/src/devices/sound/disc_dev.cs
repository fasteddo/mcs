// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using osd_ticks_t = System.UInt64;


namespace mame
{
    //DISCRETE_CLASS_STEP_RESET(dsd_555_astbl, 1,
    //class DISCRETE_CLASS_NAME(_name): public discrete_base_node, public discrete_step_interface         \
    class discrete_dsd_555_astbl_node : discrete_base_node,
                                        discrete_step_interface
    {
        const int _maxout = 1;


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
        //const double *  m_v_charge_node;            /* point to output of node */
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


        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dsd_555_astbl_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        ~discrete_dsd_555_astbl_node() { }


        //DISCRETE_STEP(dsd_555_astbl)
        public void step()
        {
            throw new emu_unimplemented();
        }


        //virtual void reset(void) override;                                                   \

        protected override int max_output() { return _maxout; }
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
    //class DISCRETE_CLASS_NAME(_name): public discrete_base_node, public discrete_step_interface         \
    class discrete_dsd_555_cc_node : discrete_base_node,
                                     discrete_step_interface
    {
        const int _maxout = 1;


        //unsigned int    m_type;                     /* type of 555cc circuit */
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


        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dsd_555_cc_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        ~discrete_dsd_555_cc_node() { }


        //DISCRETE_STEP(dsd_555_cc)
        public void step()
        {
            throw new emu_unimplemented();
        }


        //DISCRETE_RESET(dsd_555_cc)
        public override void reset()
        {
            throw new emu_unimplemented();
        }


        protected override int max_output() { return _maxout; }
    }
}
