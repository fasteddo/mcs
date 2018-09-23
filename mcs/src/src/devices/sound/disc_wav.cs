// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using osd_ticks_t = System.UInt64;


namespace mame
{
    //DISCRETE_CLASS_STEP_RESET(dss_lfsr_noise, 2,
    //class DISCRETE_CLASS_NAME(_name): public discrete_base_node, public discrete_step_interface         \
    class discrete_dss_lfsr_noise_node : discrete_base_node,
                                         discrete_step_interface
    {
        const int _maxout = 2;


        //unsigned int    m_lfsr_reg;
        //int             m_last;                 /* Last clock state */
        //double          m_t_clock;              /* fixed counter clock in seconds */
        //double          m_t_left;               /* time unused during last sample in seconds */
        //double          m_sample_step;
        //double          m_t;
        //uint8_t           m_reset_on_high;
        //uint8_t           m_invert_output;
        //uint8_t           m_out_is_f0;
        //uint8_t           m_out_lfsr_reg;


        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dss_lfsr_noise_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        ~discrete_dss_lfsr_noise_node() { }


        //DISCRETE_STEP(dss_lfsr_noise)
        public void step()
        {
            throw new emu_unimplemented();
        }


        //DISCRETE_RESET(dss_lfsr_noise)
        public override void reset()
        {
            throw new emu_unimplemented();
        }


        protected override int max_output() { return _maxout; }
    }


#if false
    DISCRETE_CLASS_STEP_RESET(dss_noise, 2,
        double          m_phase;
    );
#endif


    //DISCRETE_CLASS_STEP_RESET(dss_note, 1,
    //class DISCRETE_CLASS_NAME(_name): public discrete_base_node, public discrete_step_interface         \
    class discrete_dss_note_node : discrete_base_node,
                                   discrete_step_interface
    {
        const int _maxout = 2;


        int             m_clock_type;
        int             m_out_type;
        int             m_last;                 /* Last clock state */
        double          m_t_clock;              /* fixed counter clock in seconds */
        double          m_t_left;               /* time unused during last sample in seconds */
        int             m_max1;                 /* Max 1 Count stored as int for easy use. */
        int             m_max2;                 /* Max 2 Count stored as int for easy use. */
        int             m_count1;               /* current count1 */
        int             m_count2;               /* current count2 */


        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dss_note_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        ~discrete_dss_note_node() { }


        //DISCRETE_STEP(dss_note)
        public void step()
        {
            throw new emu_unimplemented();
        }


        //DISCRETE_RESET(dss_note)
        public override void reset()
        {
            throw new emu_unimplemented();
        }


        protected override int max_output() { return _maxout; }
    }


    //DISCRETE_CLASS_STEP_RESET(dss_squarewfix, 1,
    //class DISCRETE_CLASS_NAME(_name): public discrete_base_node, public discrete_step_interface         \
    class discrete_dss_squarewfix_node : discrete_base_node,
                                         discrete_step_interface
    {
        const int _maxout = 1;


        int             m_flip_flop;
        double          m_sample_step;
        double          m_t_left;
        double          m_t_off;
        double          m_t_on;


        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
        public discrete_dss_squarewfix_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
        ~discrete_dss_squarewfix_node() { }


        //DISCRETE_STEP(dss_squarewfix)
        public void step()
        {
            throw new emu_unimplemented();
        }


        //DISCRETE_RESET(dss_squarewfix)
        public override void reset()
        {
            throw new emu_unimplemented();
        }


        protected override int max_output() { return _maxout; }
    }
}
