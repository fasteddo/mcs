// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;  //using offs_t = u32;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    public class galaxian_sound_device : device_t
    {
        //DEFINE_DEVICE_TYPE(GALAXIAN_SOUND, galaxian_sound_device, "galaxian_sound", "Galaxian Custom Sound")
        static device_t device_creator_galaxian_sound_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new galaxian_sound_device(mconfig, tag, owner, clock); }
        public static readonly device_type GALAXIAN_SOUND = g.DEFINE_DEVICE_TYPE(device_creator_galaxian_sound_device, "galaxian_sound", "Galaxian Custom Sound");


        // internal state
        required_device<discrete_sound_device> m_discrete;  //required_device<discrete_device> m_discrete;
        uint8_t m_lfo_val;


        galaxian_sound_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, GALAXIAN_SOUND, tag, owner, clock)
        {
        }

        galaxian_sound_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_discrete = new required_device<discrete_sound_device>(this, "discrete");
            m_lfo_val = 0;
        }


        /* FIXME: May be replaced by one call! */
        public void sound_w(offs_t offset, uint8_t data)
        {
            data &= 0x01;
            switch (offset & 7)
            {
                case 0:     /* FS1 (controls 555 timer at 8R) */
                case 1:     /* FS2 (controls 555 timer at 8S) */
                case 2:     /* FS3 (controls 555 timer at 8T) */
                    background_enable_w(offset, data);
                    break;

                case 3:     /* HIT */
                    noise_enable_w(data);
                    break;

                case 4:     /* n/c */
                    break;

                case 5:     /* FIRE */
                    fire_enable_w(data);
                    break;

                case 6:     /* VOL1 */
                case 7:     /* VOL2 */
                    vol_w(offset & 1, data);
                    break;
            }
        }


        /* IC 9J */
        public void pitch_w(uint8_t data)
        {
            m_discrete.op[0].write(galaxian_state.GAL_INP_PITCH, data);
        }


        void vol_w(offs_t offset, uint8_t data)
        {
            m_discrete.op[0].write((offs_t)g.NODE_RELATIVE(galaxian_state.GAL_INP_VOL1, (int)offset), (uint8_t)(data & 0x01));
        }


        void noise_enable_w(uint8_t data)
        {
            m_discrete.op[0].write(galaxian_state.GAL_INP_HIT, (uint8_t)(data & 0x01));
        }


        void background_enable_w(offs_t offset, uint8_t data)
        {
            m_discrete.op[0].write((offs_t)g.NODE_RELATIVE(galaxian_state.GAL_INP_FS1, (int)offset), (uint8_t)(data & 0x01));
        }


        void fire_enable_w(uint8_t data)
        {
            m_discrete.op[0].write(galaxian_state.GAL_INP_FIRE, (uint8_t)(data & 0x01));
        }


        public void lfo_freq_w(offs_t offset, uint8_t data)
        {
            uint8_t lfo_val_new = (uint8_t)((m_lfo_val & ~(1U << (int)offset)) | ((uint32_t)(data & 0x01) << (int)offset));

            if (m_lfo_val != lfo_val_new)
            {
                m_lfo_val = lfo_val_new;
                m_discrete.op[0].write(galaxian_state.GAL_INP_BG_DAC, m_lfo_val);
            }
        }


        // device-level overrides
        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            m_lfo_val = 0;

            save_item(g.NAME(new { m_lfo_val }));
        }


        //-------------------------------------------------
        //  machine_add_config - add device configuration
        //-------------------------------------------------
        protected override void device_add_mconfig(machine_config config)
        {
            // sound hardware
            g.DISCRETE(config, m_discrete).disound.add_route(g.ALL_OUTPUTS, ":speaker", 1.0);
            m_discrete.op[0].set_intf(galaxian_state.galaxian_discrete);
        }
    }


    partial class galaxian_state : driver_device
    {
        // initialized in \includes\galaxian.cs because of order of operation issues due to partial classes
        // https://stackoverflow.com/questions/29086844/static-field-initialization-order-with-partial-classes
        static readonly XTAL SOUND_CLOCK;//             = GALAXIAN_MASTER_CLOCK/6/2;          /* 1.536 MHz */
        static readonly XTAL RNG_RATE;//                = GALAXIAN_MASTER_CLOCK/3*2;          /* RNG clock is XTAL/3*2 see Aaron's note in video/galaxian.c */

        /* 74LS259 */
        public const int GAL_INP_BG_DAC          = g.NODE_10;     /* at 9M Q4 to Q7 in schematics */

        public const int GAL_INP_FS1             = g.NODE_20;     /* FS1 9L Q0 */
        const int GAL_INP_FS2             = g.NODE_21;     /* FS2 9L Q1 */
        const int GAL_INP_FS3             = g.NODE_22;     /* FS3 9L Q2 */
        public const int GAL_INP_HIT             = g.NODE_23;     /* HIT 9L Q3 */
        //#define GAL_9L_Q4             NODE_24
        public const int GAL_INP_FIRE            = g.NODE_25;     /* FIRE 9L Q5 */
        public const int GAL_INP_VOL1            = g.NODE_26;     /* VOL1 9L Q6 */
        const int GAL_INP_VOL2            = g.NODE_27;     /* VOL2 9L Q7 */

        public const int GAL_INP_PITCH           = g.NODE_28;     /* at 6T in schematics */

        const double TTL_OUT                 = 4.0;

        static readonly double GAL_R15                 = g.RES_K(100);
        static readonly double GAL_R16                 = g.RES_K(220);
        static readonly double GAL_R17                 = g.RES_K(470);
        static readonly double GAL_R18                 = g.RES_K(1000);
        static readonly double GAL_R19                 = g.RES_K(330);

        static readonly double GAL_R20                 = g.RES_K(15);
        static readonly double GAL_R21                 = g.RES_K(100);
        static readonly double GAL_R22                 = g.RES_K(100);
        static readonly double GAL_R23                 = g.RES_K(470);
        static readonly double GAL_R24                 = g.RES_K(10);
        static readonly double GAL_R25                 = g.RES_K(100);
        static readonly double GAL_R26                 = g.RES_K(330);
        static readonly double GAL_R27                 = g.RES_K(10);
        static readonly double GAL_R28                 = g.RES_K(100);
        static readonly double GAL_R29                 = g.RES_K(220);

        static readonly double GAL_R30                 = g.RES_K(10);
        static readonly double GAL_R31                 = g.RES_K(47);
        static readonly double GAL_R32                 = g.RES_K(47);
        static readonly double GAL_R33                 = g.RES_K(10);
        /*
         * R34 is given twice on galaxian board and both times as 5.1k. On moon cresta
         * it is only listed once and given as 15k. This is more in line with recordings
         */
        static readonly double GAL_R34                 = g.RES_K(5.1);
        //#define MCRST_R34                   RES_K(15)

        static readonly double GAL_R35                 = g.RES_K(150);
        static readonly double GAL_R36                 = g.RES_K(22);
        static readonly double GAL_R37                 = g.RES_K(470);
        static readonly double GAL_R38                 = g.RES_K(33);
        static readonly double GAL_R39                 = g.RES_K(22);

        /* The hit sound is too low compared with recordings
         * There may be an issue with the op-amp band filter
         */
        static readonly double GAL_R40                 = g.RES_K(2.2) * 0.6;    /* Volume adjust */
        static readonly double GAL_R41                 = g.RES_K(100);
        static readonly double GAL_R43                 = g.RES_K(2.2);
        static readonly double GAL_R44                 = g.RES_K(10);
        static readonly double GAL_R45                 = g.RES_K(22);
        static readonly double GAL_R46                 = g.RES_K(10);
        static readonly double GAL_R47                 = g.RES_K(2.2);
        static readonly double GAL_R48                 = g.RES_K(2.2);
        static readonly double GAL_R49                 = g.RES_K(10);

        static readonly double GAL_R50                 = g.RES_K(22);
        static readonly double GAL_R51                 = g.RES_K(33);
        static readonly double GAL_R52                 = g.RES_K(15);

        static readonly double GAL_R91                 = g.RES_K(10);

        static readonly double GAL_C15                 = g.CAP_U(1);
        static readonly double GAL_C17                 = g.CAP_U(0.01);
        static readonly double GAL_C18                 = g.CAP_U(0.01);
        static readonly double GAL_C19                 = g.CAP_U(0.01);

        static readonly double GAL_C20                 = g.CAP_U(0.1);
        static readonly double GAL_C21                 = g.CAP_U(2.2);
        static readonly double GAL_C22                 = g.CAP_U(0.01);
        static readonly double GAL_C23                 = g.CAP_U(0.01);
        static readonly double GAL_C25                 = g.CAP_U(1);
        static readonly double GAL_C26                 = g.CAP_U(0.01);
        static readonly double GAL_C27                 = g.CAP_U(0.01);
        static readonly double GAL_C28                 = g.CAP_U(47);

        static readonly double GAL_C46                 = g.CAP_U(0.1);


        /*************************************
         *
         *  Structures for discrete core
         *
         *************************************/

        static readonly discrete_dac_r1_ladder galaxian_bck_dac = new discrete_dac_r1_ladder
        (
            4,          // size of ladder
            new double [] {GAL_R18, GAL_R17, GAL_R16, GAL_R15, 0,0,0,0},
            4.4,        // 5V - diode junction (0.6V)
            GAL_R20,    // rBIAS
            GAL_R19,    // rGnd
            0           // no C
        );

        static readonly discrete_555_cc_desc galaxian_bck_vco = new discrete_555_cc_desc
        (
            g.DISC_555_OUT_DC | g.DISC_555_OUT_CAP,
            5,      // B+ voltage of 555
            g.DEFAULT_555_VALUES_1, g.DEFAULT_555_VALUES_2,
            0.7     // Q2 junction voltage
        );

        static readonly discrete_555_desc galaxian_555_vco_desc = new discrete_555_desc
        (
            g.DISC_555_OUT_ENERGY | g.DISC_555_OUT_DC,
            5.0,
            g.DEFAULT_555_CHARGE,
            (5.0 - 0.5)         // 10k means no real load
        );

        static readonly discrete_555_desc galaxian_555_fire_vco_desc = new discrete_555_desc
        (
            g.DISC_555_OUT_DC,
            5.0,
            g.DEFAULT_555_CHARGE,
            1.0 // Logic output
        );

        static readonly discrete_mixer_desc galaxian_bck_mixer_desc = new discrete_mixer_desc
        (
            g.DISC_MIXER_IS_RESISTOR,
            new double [] {GAL_R24, GAL_R27, GAL_R30},
            new int [] {0,0,0},
            new double [] {0,0,0,0},  /* no node capacitors */
            0, 0,
            GAL_C20,
            0,
            0, 1
        );

        static readonly discrete_lfsr_desc galaxian_lfsr = new discrete_lfsr_desc
        (
            g.DISC_CLK_IS_FREQ,
            17,                     /* Bit Length */
            0,                      /* Reset Value */
            4,                      /* Use Bit 10 (QC of second LS164) as F0 input 0 */
            16,                     /* Use Bit 23 (QH of third LS164) as F0 input 1 */
            g.DISC_LFSR_XOR_INV_IN1,  /* F0 is XOR */
            g.DISC_LFSR_IN0,          /* F1 is inverted F0*/
            g.DISC_LFSR_REPLACE,      /* F2 replaces the shifted register contents */
            0x000001,               /* Everything is shifted into the first bit only */
            g.DISC_LFSR_FLAG_OUTPUT_F0, /* Output is result of F0 */
            0                       /* Output bit */
        );

        static readonly discrete_mixer_desc galaxian_mixerpre_desc = new discrete_mixer_desc
        (
            g.DISC_MIXER_IS_RESISTOR,
            new double [] {GAL_R51, 0, GAL_R50, 0, GAL_R34},      /* A, C, C, D */
            new int [] {0, GAL_INP_VOL1, 0, GAL_INP_VOL2, 0},
            new double [] {0,0,0,0,0},
            0, 0,
            0,
            0,
            0, 1
        );

        static readonly discrete_mixer_desc galaxian_mixer_desc = new discrete_mixer_desc
        (
            g.DISC_MIXER_IS_RESISTOR,
            new double [] {GAL_R34, GAL_R40, GAL_R43},        /* A, C, C, D */
            new int [] {0, 0, 0},
            new double [] {0,0,GAL_C26},
            0, GAL_R91,
            0,
            GAL_C46,
            0, 1
        );

#if false
        /* moon cresta has different mixing */
        static const discrete_mixer_desc mooncrst_mixer_desc =
        {
            DISC_MIXER_IS_RESISTOR,
            {GAL_R51, 0, GAL_R50, 0, MCRST_R34, GAL_R40, GAL_R43},      /* A, C, C, D */
            {0, GAL_INP_VOL1, 0, GAL_INP_VOL2, 0, 0, 0},
            {0,0,0,0,0,0,GAL_C26},
            0, 0*GAL_R91,
            0,
            GAL_C46,
            0, 1
        };
#endif

        static readonly discrete_op_amp_filt_info galaxian_bandpass_desc = new discrete_op_amp_filt_info
        (
            GAL_R35, GAL_R36, 0, 0,
            GAL_R37,
            GAL_C22, GAL_C23, 0,
            5.0*GAL_R39/(GAL_R38+GAL_R39),
            5, 0
        );


        /*************************************
         *
         *  Discrete Sound Blocks
         *
         *************************************/

        //DISCRETE_SOUND_START(galaxian_discrete)
        public static readonly discrete_block [] galaxian_discrete;  // init moved to constructor because of out-of-order construction of partial classes
        static discrete_block [] galaxian_discrete_construct() { return new discrete_block []
        {
            /************************************************/
            /* Input register mapping for galaxian          */
            /************************************************/
            g.DISCRETE_INPUT_DATA(GAL_INP_BG_DAC),

            /* FS1 to FS3 */
            g.DISCRETE_INPUT_LOGIC(GAL_INP_FS1),
            g.DISCRETE_INPUT_LOGIC(GAL_INP_FS2),
            g.DISCRETE_INPUT_LOGIC(GAL_INP_FS3),

            /* HIT */
            g.DISCRETE_INPUTX_DATA(GAL_INP_HIT, TTL_OUT, 0, 0),

            /* FIRE */
            g.DISCRETE_INPUT_LOGIC(GAL_INP_FIRE),

            /* Turns on / off resistors in mixer */
            g.DISCRETE_INPUTX_DATA(GAL_INP_VOL1, GAL_R49, 0, 0),
            g.DISCRETE_INPUTX_DATA(GAL_INP_VOL2, GAL_R52, 0, 0),

            /* Pitch */
            g.DISCRETE_INPUT_DATA(GAL_INP_PITCH),

            g.DISCRETE_TASK_START(0),

                /************************************************/
                /* NOISE                                        */
                /************************************************/

                /* since only a sample of the LFSR is latched @V2 we let the lfsr
                 * run at a lower speed
                 */
                g.DISCRETE_LFSR_NOISE(g.NODE_150, 1, 1, (int)(RNG_RATE.dvalue()/100), 1.0, 0, 0.5, galaxian_lfsr),
                g.DISCRETE_SQUAREWFIX(g.NODE_151,1,60*264/2,1.0,50,0.5,0),  /* 2V signal */
                g.DISCRETE_LOGIC_DFLIPFLOP(g.NODE_152,1,1,g.NODE_151,g.NODE_150),
            g.DISCRETE_TASK_END(),

            /* Group Background and pitch */
            g.DISCRETE_TASK_START(1),

                /************************************************/
                /* Background                                   */
                /************************************************/

                g.DISCRETE_DAC_R1(g.NODE_100, GAL_INP_BG_DAC, TTL_OUT, galaxian_bck_dac),
                g.DISCRETE_555_CC(g.NODE_105, 1, g.NODE_100, GAL_R21, GAL_C15, 0, 0, 0, galaxian_bck_vco),
                // Next is mult/add opamp circuit
                g.DISCRETE_MULTADD(g.NODE_110, g.NODE_105, GAL_R33/g.RES_3_PARALLEL(GAL_R31,GAL_R32,GAL_R33),
                        -5.0*GAL_R33/GAL_R31),
                g.DISCRETE_CLAMP(g.NODE_111,g.NODE_110,0.0,5.0),
                // The three 555
                g.DISCRETE_555_ASTABLE_CV(g.NODE_115, GAL_INP_FS1, GAL_R22, GAL_R23, GAL_C17, g.NODE_111, galaxian_555_vco_desc),
                g.DISCRETE_555_ASTABLE_CV(g.NODE_116, GAL_INP_FS2, GAL_R25, GAL_R26, GAL_C18, g.NODE_111, galaxian_555_vco_desc),
                g.DISCRETE_555_ASTABLE_CV(g.NODE_117, GAL_INP_FS3, GAL_R28, GAL_R29, GAL_C19, g.NODE_111, galaxian_555_vco_desc),

                g.DISCRETE_MIXER3(g.NODE_120, 1, g.NODE_115, g.NODE_116, g.NODE_117, galaxian_bck_mixer_desc),

                /************************************************/
                /* PITCH                                        */
                /************************************************/

                /* two cascaded LS164 which are reset to pitch latch value,
                 * thus generating SOUND_CLOCK / (256 - pitch_clock) signal
                 *
                 * One possibility to implement this is
                 * DISCRETE_TRANSFORM3(NODE_130, SOUND_CLOCK, 256, GAL_INP_PITCH, "012-/")
                 * DISCRETE_COUNTER(NODE_132, 1, 0, NODE_130, 0, 15, DISC_COUNT_UP, 0, DISC_CLK_IS_FREQ)
                 * but there is a native choice:
                 */
                g.DISCRETE_NOTE(g.NODE_132, 1, (int)SOUND_CLOCK.dvalue(), GAL_INP_PITCH, 255, 15, g.DISC_CLK_IS_FREQ),

                /* from the 74393 (counter 2 above) only QA, QC, QD are used.
                 * We decode three here and use SUB_NODE(133,x) below to access.
                 */
                g.DISCRETE_BITS_DECODE(g.NODE_133, g.NODE_132, 0, 3, TTL_OUT),     /* QA-QD 74393 */

            /* End of this task */
            g.DISCRETE_TASK_END(),

            g.DISCRETE_TASK_START(1),

                /************************************************/
                /* HIT                                          */
                /************************************************/

                /* Not 100% correct - switching causes high impedance input for node_157
                 * this is not emulated */
                g.DISCRETE_RCDISC5(g.NODE_155, g.NODE_152, GAL_INP_HIT, (GAL_R35 + GAL_R36), GAL_C21),
                g.DISCRETE_OP_AMP_FILTER(g.NODE_157, 1, g.NODE_155, 0, g.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, galaxian_bandpass_desc),
            g.DISCRETE_TASK_END(),

            g.DISCRETE_TASK_START(1),
                /************************************************/
                /* FIRE                                         */
                /************************************************/

                g.DISCRETE_LOGIC_INVERT(g.NODE_170, GAL_INP_FIRE),
                g.DISCRETE_MULTIPLY(g.NODE_171, TTL_OUT, GAL_INP_FIRE),
                g.DISCRETE_MULTIPLY(g.NODE_172, TTL_OUT, g.NODE_170), // inverted
                g.DISCRETE_RCFILTER(g.NODE_173, g.NODE_172, GAL_R47, GAL_C28),
                /* Mix noise and 163 */
                g.DISCRETE_TRANSFORM5(g.NODE_177, g.NODE_152, TTL_OUT, 1.0/GAL_R46, g.NODE_173, 1.0/GAL_R48,
                        "01*2*34*+" ),
                //DISCRETE_MULTIPLY(NODE_174, 1, TTL_OUT, NODE_152)
                //DISCRETE_MULTIPLY(NODE_175, 1, 1.0/GAL_R46, NODE_174)
                //DISCRETE_MULTIPLY(NODE_176, 1, 1.0/GAL_R48, NODE_173)
                //DISCRETE_ADDER2(NODE_177, 1, NODE_175, NODE_176)
                g.DISCRETE_MULTIPLY(g.NODE_178, g.RES_2_PARALLEL(GAL_R46, GAL_R48), g.NODE_177),

                g.DISCRETE_555_ASTABLE_CV(g.NODE_181, 1, GAL_R44, GAL_R45, GAL_C27, g.NODE_178, galaxian_555_fire_vco_desc),

                /* 555 toggles discharge on rc discharge module */
                g.DISCRETE_RCDISC5(g.NODE_182, g.NODE_181, g.NODE_171, (GAL_R41), GAL_C25),

            /* End of task */
            g.DISCRETE_TASK_END(),

            /************************************************/
            /* FINAL MIX                                    */
            /************************************************/

            g.DISCRETE_TASK_START(2),
                g.DISCRETE_MIXER5(g.NODE_279, 1, g.NODE_133_00, g.NODE_133_02, g.NODE_133_02, g.NODE_133_03, g.NODE_120, galaxian_mixerpre_desc),
                g.DISCRETE_MIXER3(g.NODE_280, 1, g.NODE_279, g.NODE_157, g.NODE_182, galaxian_mixer_desc),
                g.DISCRETE_OUTPUT(g.NODE_280, 32767.0/5.0*5),
            g.DISCRETE_TASK_END(),

            g.DISCRETE_SOUND_END,
        }; }
    }
}
