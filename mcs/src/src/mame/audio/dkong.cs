// license:BSD-3-Clause
// copyright-holders:Edward Fast

/* Set to 1 to use faster custom mixer */
#define DK_USE_CUSTOM

/* FIXME: Review at a later time */
#define DK_REVIEW


using System;
using System.Collections.Generic;

using offs_t = System.UInt32;  //using offs_t = u32;
using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;
using u8 = System.Byte;
using uint8_t = System.Byte;


namespace mame
{
    partial class dkong_state : driver_device
    {
        // moved from \includes\dkong.cs so that it's init before used
        static readonly XTAL MASTER_CLOCK_audio            = new XTAL(61440000);
        static readonly XTAL CLOCK_1H_audio                = MASTER_CLOCK_audio / 5 / 4;
        static readonly XTAL CLOCK_16H_audio               = CLOCK_1H_audio / 16;
        static readonly XTAL CLOCK_1VF_audio               = CLOCK_16H_audio / 12 / 2;
        static readonly XTAL CLOCK_2VF               = CLOCK_1VF_audio / 2;


        /* Discrete sound inputs */

        const int DS_SOUND0_INV       = g.NODE_01;
        const int DS_SOUND1_INV       = g.NODE_02;
        const int DS_SOUND2_INV       = g.NODE_03;
        const int DS_SOUND6_INV       = g.NODE_04;
        const int DS_SOUND7_INV       = g.NODE_05;
        //#define DS_SOUND9_INV       NODE_06
        const int DS_DAC              = g.NODE_07;
        const int DS_DISCHARGE_INV    = g.NODE_08;

        const int DS_SOUND0           = g.NODE_208;
        const int DS_SOUND1           = g.NODE_209;
        //#define DS_SOUND6           NODE_210
        //#define DS_SOUND7           NODE_211
        //#define DS_SOUND9           NODE_212

        const int DS_ADJ_DAC          = g.NODE_240;

        const int DS_OUT_SOUND0       = g.NODE_241;
        const int DS_OUT_SOUND1       = g.NODE_242;
        const int DS_OUT_SOUND2       = g.NODE_243;
        //#define DS_OUT_SOUND6       NODE_247
        //#define DS_OUT_SOUND7       NODE_248
        //#define DS_OUT_SOUND9       NODE_249
        const int DS_OUT_DAC          = g.NODE_250;

        /* Input definitions for write handlers */

        const int DS_SOUND0_INP       = DS_SOUND0_INV;
        const int DS_SOUND1_INP       = DS_SOUND1_INV;
        const int DS_SOUND2_INP       = DS_SOUND2_INV;
        const int DS_SOUND6_INP       = DS_SOUND6_INV;
        const int DS_SOUND7_INP       = DS_SOUND7_INV;
        //#define DS_SOUND9_INP       DS_SOUND9_INV


        public class int_const_DS_DISCHARGE_INV : int_const { public int value { get { return DS_DISCHARGE_INV; } } }
        public class int_const_DS_SOUND0_INP : int_const { public int value { get { return DS_SOUND0_INP; } } }
        public class int_const_DS_SOUND1_INP : int_const { public int value { get { return DS_SOUND1_INP; } } }
        public class int_const_DS_SOUND2_INP : int_const { public int value { get { return DS_SOUND2_INP; } } }
        public class int_const_DS_SOUND6_INP : int_const { public int value { get { return DS_SOUND6_INP; } } }
        public class int_const_DS_SOUND7_INP : int_const { public int value { get { return DS_SOUND7_INP; } } }


        /* General defines */

        const double DK_1N5553_V         = 0.4; /* from datasheet at 1mA */
        const double DK_SUP_V            = 5.0;
        //#define NE555_INTERNAL_R    RES_K(5)

        static double R_SERIES(double R1, double R2) { return R1 + R2; }


        /* Resistors */

        static readonly double DK_R1       = g.RES_K(47);
        static readonly double DK_R2       = g.RES_K(47);
        static readonly double DK_R3       = g.RES_K(5.1);
        static readonly double DK_R4       = g.RES_K(2);
        static readonly double DK_R5       = 750;
        static readonly double DK_R6       = g.RES_K(4.7);
        static readonly double DK_R7       = g.RES_K(10);
        static readonly double DK_R8       = g.RES_K(100);
        static readonly double DK_R9       = g.RES_K(10);
        static readonly double DK_R10      = g.RES_K(10);
        static readonly double DK_R14      = g.RES_K(47);

        static readonly double DK_R15      = g.RES_K(5.6);
        static readonly double DK_R16      = g.RES_K(5.6);
        static readonly double DK_R17      = g.RES_K(10);
        static readonly double DK_R18      = g.RES_K(4.7);
        static readonly double DK_R20      = g.RES_K(10);
        //#define DK_R21      RES_K(5.6)
        //#define DK_R22      RES_K(5.6)
        static readonly double DK_R24      = g.RES_K(47);
        static readonly double DK_R25      = g.RES_K(5.1);
        static readonly double DK_R26      = g.RES_K(2);
        static readonly double DK_R27      = 150;
        static readonly double DK_R28      = g.RES_K(4.7);
        static readonly double DK_R29      = g.RES_K(10);
        static readonly double DK_R30      = g.RES_K(100);
        static readonly double DK_R31      = g.RES_K(10);
        static readonly double DK_R32      = g.RES_K(10);
        static readonly double DK_R35      = g.RES_K(1);
        static readonly double DK_R36      = g.RES_K(1);
        static readonly double DK_R38      = g.RES_K(18);
        static readonly double DK_R39      = g.RES_M(3.3);
        static readonly double DK_R49      = g.RES_K(1.2);
        static readonly double DK_R44      = g.RES_K(1.2);
        static readonly double DK_R45      = g.RES_K(10);
        static readonly double DK_R46      = g.RES_K(12);
        static readonly double DK_R47      = g.RES_K(4.3);
        static readonly double DK_R48      = g.RES_K(43);
        static readonly double DK_R50      = g.RES_K(10);
        static readonly double DK_R51      = g.RES_K(10);

        /* Capacitors */

        //#define DK_C8       CAP_U(220)
        static readonly double DK_C12      = g.CAP_U(1);
        static readonly double DK_C13      = g.CAP_U(33);
        static readonly double DK_C16      = g.CAP_U(1);
        static readonly double DK_C17      = g.CAP_U(4.7);
        static readonly double DK_C18      = g.CAP_U(1);
        static readonly double DK_C19      = g.CAP_U(1);
        static readonly double DK_C20      = g.CAP_U(3.3);
        static readonly double DK_C21      = g.CAP_U(1);

        static readonly double DK_C23      = g.CAP_U(4.7);
        static readonly double DK_C24      = g.CAP_U(10);
        static readonly double DK_C26      = g.CAP_U(3.3);
        static readonly double DK_C25      = g.CAP_U(3.3);
        static readonly double DK_C29      = g.CAP_U(3.3);
        static readonly double DK_C30      = g.CAP_U(10);
        static readonly double DK_C32      = g.CAP_U(10);
        //#define DK_C34      CAP_N(10)
        static readonly double DK_C159     = g.CAP_N(100);


        /*
         * The noice generator consists of three LS164 8+8+8
         * the output signal is taken after the xor, so
         * taking bit 0 is not exact
         */

        static readonly discrete_lfsr_desc dkong_lfsr = new discrete_lfsr_desc
        (
            g.DISC_CLK_IS_FREQ,
            24,                   /* Bit Length */
            0,                    /* Reset Value */
            10,                   /* Use Bit 10 (QC of second LS164) as F0 input 0 */
            23,                   /* Use Bit 23 (QH of third LS164) as F0 input 1 */
            g.DISC_LFSR_XOR,        /* F0 is XOR */
            g.DISC_LFSR_NOT_IN0,    /* F1 is inverted F0*/
            g.DISC_LFSR_REPLACE,    /* F2 replaces the shifted register contents */
            0x000001,             /* Everything is shifted into the first bit only */
            g.DISC_LFSR_FLAG_OUTPUT_F0, /* Output is result of F0 */
            0                     /* Output bit */
        );

        static readonly double [] dkong_diode_mix_table = new double [2] { DK_1N5553_V, DK_1N5553_V * 2 };


        static readonly discrete_mixer_desc dkong_mixer_desc = new discrete_mixer_desc
        (
            g.DISC_MIXER_IS_RESISTOR,
            new double [] { DK_R2, DK_R24, DK_R1, DK_R14 },
            new int [] { 0, 0, 0 },  /* no variable resistors */
            new double [] { 0, 0, 0 },  /* no node capacitors */
#if DK_REVIEW
            0, g.RES_K(10),
#else
            0, 0,
#endif
            DK_C159,
            DK_C12,
            0, 1
        );

        /* There is no load on the output for the jump circuit
         * For the walk circuit, the voltage does not matter */

        static readonly discrete_555_desc dkong_555_vco_desc = new discrete_555_desc
        (
            g.DISC_555_OUT_ENERGY | g.DISC_555_OUT_DC,
            DK_SUP_V,
            g.DEFAULT_555_CHARGE,
            DK_SUP_V - 0.5
        );

        static readonly discrete_dss_inverter_osc_node.description dkong_inverter_osc_desc_jump = new discrete_dss_inverter_osc_node.description
        (
            discrete_dss_inverter_osc_node.DEFAULT_CD40XX_VALUES(DK_SUP_V),
            discrete_dss_inverter_osc_node.IS_TYPE1
        );

        static readonly discrete_dss_inverter_osc_node.description dkong_inverter_osc_desc_walk = new discrete_dss_inverter_osc_node.description
        (
            discrete_dss_inverter_osc_node.DEFAULT_CD40XX_VALUES(DK_SUP_V),
            discrete_dss_inverter_osc_node.IS_TYPE2
        );

        static readonly discrete_op_amp_filt_info dkong_sallen_key_info = new discrete_op_amp_filt_info
        (
            g.RES_K(5.6), g.RES_K(5.6), 0, 0, 0,
            g.CAP_N(22), g.CAP_N(10), 0
        );


#if DK_USE_CUSTOM
        /************************************************************************
         *
         * Custom dkong mixer
         *
         * input[0]    - In1 (Logic)
         * input[1]    - In2
         * input[2]    - R1
         * input[3]    - R2
         * input[4]    - R3
         * input[5]    - R4
         * input[6]    - C
         * input[7]    - B+
         *
         *              V (B+)                               V (B+)
         *                v                                    v
         *                |       Node Output <----.       .-- | -----
         *                Z                        |       |   Z
         *                Z R1                     |       |   Z 5k
         *                Z                        |       |   Z     555
         *          |\    |     R2          R4     |           |    internal
         *  In1 >---| >o--+---/\/\/\--+---/\/\/\---+-----------+ CV
         *          |/                |            |           |
         *                            |           ---      |   Z
         *             R3             |           --- C    |   Z 10k
         *  In2 >----/\/\/\-----------'            |       |   Z
         *                                         |       '-- | -----
         *                                        Gnd         Gnd
         *
         ************************************************************************/

        //DISCRETE_CLASS_STEP_RESET(dkong_custom_mixer, 1,
        class discrete_dkong_custom_mixer_node : discrete_base_node,
                                                 discrete_step_interface
        {
            const int _maxout = 1;


            double DKONG_CUSTOM_IN1 { get { return DISCRETE_INPUT(0); } }
            double DKONG_CUSTOM_IN2 { get { return DISCRETE_INPUT(1); } }
            double DKONG_CUSTOM_R1 { get { return DISCRETE_INPUT(2); } }
            double DKONG_CUSTOM_R2 { get { return DISCRETE_INPUT(3); } }
            double DKONG_CUSTOM_R3 { get { return DISCRETE_INPUT(4); } }
            double DKONG_CUSTOM_R4 { get { return DISCRETE_INPUT(5); } }
            double DKONG_CUSTOM_C { get { return DISCRETE_INPUT(6); } }
            double DKONG_CUSTOM_V { get { return DISCRETE_INPUT(7); } }

            double NE555_CV_R { get { return g.RES_2_PARALLEL(g.RES_K(5), g.RES_K(10)); } }


            double [] m_i_in1 = new double [2];
            double [] m_r_in = new double [2];
            double [] m_r_total = new double [2];
            double [] m_exp = new double [2];
            double m_out_v;


            public osd_ticks_t run_time { get; set; }
            public discrete_base_node self { get; set; }


            //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
            public discrete_dkong_custom_mixer_node() : base() { }

            //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
            //~discrete_dkong_custom_mixer_node() { }


            //DISCRETE_STEP( dkong_custom_mixer )
            public void step()
            {
                int     in_1    = (int)DKONG_CUSTOM_IN1;

                /* start of with 555 current */
                double  i_total = DKONG_CUSTOM_V / g.RES_K(5);
                /* add in current from In1 */
                i_total += m_i_in1[in_1];
                /* add in current from In2 */
                i_total += DKONG_CUSTOM_IN2 / DKONG_CUSTOM_R3;
                /* charge cap */
                /* node->output is cap voltage, (i_total * m_r_total[in_1]) is current charge voltage */
                m_out_v += (i_total * m_r_total[in_1] - m_out_v) * m_exp[in_1];
                set_output(0, m_out_v);
            }


            //DISCRETE_RESET( dkong_custom_mixer )
            public override void reset()
            {
                /* everything is based on the input to the O.C. inverter */
                /* precalculate current from In1 */
                m_i_in1[0] = DKONG_CUSTOM_V / (DKONG_CUSTOM_R1 + DKONG_CUSTOM_R2);
                m_i_in1[1] = 0;
                /* precalculate total resistance for input circuit */
                m_r_in[0] = g.RES_2_PARALLEL((DKONG_CUSTOM_R1 + DKONG_CUSTOM_R2), DKONG_CUSTOM_R3);
                m_r_in[1] = g.RES_2_PARALLEL(DKONG_CUSTOM_R2, DKONG_CUSTOM_R3);
                /* precalculate total charging resistance */
                m_r_total[0] = g.RES_2_PARALLEL(m_r_in[0] + DKONG_CUSTOM_R4, NE555_CV_R);
                m_r_total[1] = g.RES_2_PARALLEL((m_r_in[1] + DKONG_CUSTOM_R4), NE555_CV_R);
                /* precalculate charging exponents */
                m_exp[0] = RC_CHARGE_EXP(m_r_total[0] * DKONG_CUSTOM_C);
                m_exp[1] = RC_CHARGE_EXP(m_r_total[1] * DKONG_CUSTOM_C);

                m_out_v = 0;
            }


            protected override int max_output() { return _maxout; }
        }
#endif


        //static DISCRETE_SOUND_START(dkong2b_discrete)
        static readonly discrete_block [] dkong2b_discrete = new discrete_block []
        {
            /************************************************/
            /* Input register mapping for dkong             */
            /************************************************/

            /* DISCRETE_INPUT_DATA */
            g.DISCRETE_INPUT_NOT(DS_SOUND2_INV),
                g.DISCRETE_INPUT_NOT(DS_SOUND1_INV),   /* IC 6J, pin 12 */
                g.DISCRETE_INPUT_NOT(DS_SOUND0_INV),   /* IC 6J, pin 2 */
                g.DISCRETE_INPUT_NOT(DS_DISCHARGE_INV),
                //DISCRETE_INPUT_DATA(DS_DAC)

                /************************************************/
                /* Stomp                                        */
                /************************************************/
                /* Noise */
                g.DISCRETE_TASK_START(1),
                g.DISCRETE_LFSR_NOISE(g.NODE_11, 1, 1, CLOCK_2VF.dvalue(), 1.0, 0, 0.5, dkong_lfsr),
                g.DISCRETE_COUNTER(g.NODE_12, 1, 0, g.NODE_11, 0, 7, g.DISC_COUNT_UP, 0, g.DISC_CLK_ON_R_EDGE),    /* LS161, IC 3J */
                g.DISCRETE_TRANSFORM3(g.NODE_13,g.NODE_12,3,DK_SUP_V,"01>2*"),

                /* Stomp */
                /* C21 is discharged via Q5 BE */
                g.DISCRETE_RCDISC_MODULATED(g.NODE_15,DS_SOUND2_INV,0,DK_R10,0,0,DK_R9,DK_C21,DK_SUP_V),
                /* Q5 */
                g.DISCRETE_TRANSFORM2(g.NODE_16, g.NODE_15, 0.6, "01>"),
                g.DISCRETE_RCDISC2(g.NODE_17,g.NODE_16,DK_SUP_V,DK_R8+DK_R7,0.0,DK_R7,DK_C20),

                g.DISCRETE_DIODE_MIXER2(g.NODE_20, g.NODE_17, g.NODE_13, dkong_diode_mix_table), /* D1, D2 + D3 */

                g.DISCRETE_RCINTEGRATE(g.NODE_22,g.NODE_20,DK_R5, g.RES_2_PARALLEL(DK_R4+DK_R3,DK_R6),0,DK_C19,DK_SUP_V,g.DISC_RC_INTEGRATE_TYPE1),
                g.DISCRETE_MULTIPLY(DS_OUT_SOUND0, g.NODE_22, DK_R3 / R_SERIES(DK_R3, DK_R4)),
                g.DISCRETE_TASK_END(),

                /************************************************/
                /* Jump                                         */
                /************************************************/
                /*  tt */
                /* 4049B Inverter Oscillator build from 3 inverters */
                g.DISCRETE_TASK_START(1),
                g.DISCRETE_INVERTER_OSC(g.NODE_25, 1, 0, DK_R38, DK_R39, DK_C26, 0, dkong_inverter_osc_desc_jump),

#if DK_USE_CUSTOM
                /* custom mixer for 555 CV voltage */
                g.DISCRETE_CUSTOM8<discrete_dkong_custom_mixer_node>(g.NODE_28, DS_SOUND1_INV, g.NODE_25,
                            DK_R32, DK_R50, DK_R51, DK_R49, DK_C24, DK_SUP_V, null),
#else
                DISCRETE_LOGIC_INVERT(DS_SOUND1,DS_SOUND1_INV),
                DISCRETE_MULTIPLY(NODE_24,DS_SOUND1,DK_SUP_V),
                DISCRETE_TRANSFORM3(NODE_26,DS_SOUND1,DK_R32,DK_R49+DK_R50,"01*2+"),
                DISCRETE_MIXER4(NODE_28, 1, NODE_24, NODE_25, DK_SUP_V, 0, &dkong_rc_jump_desc),
#endif
                /* 555 Voltage controlled */
                g.DISCRETE_555_ASTABLE_CV(g.NODE_29, 1, g.RES_K(47), g.RES_K(27), g.CAP_N(47), g.NODE_28,
                                        dkong_555_vco_desc),

                /* Jump trigger */
                g.DISCRETE_RCDISC_MODULATED(g.NODE_33,DS_SOUND1_INV,0,DK_R32,0,0,DK_R31,DK_C18,DK_SUP_V),

                g.DISCRETE_TRANSFORM2(g.NODE_34, g.NODE_33, 0.6, "01>"),
                g.DISCRETE_RCDISC2(g.NODE_35, g.NODE_34,DK_SUP_V,R_SERIES(DK_R30,DK_R29),0.0,DK_R29,DK_C17),

                g.DISCRETE_DIODE_MIXER2(g.NODE_38, g.NODE_35, g.NODE_29, dkong_diode_mix_table),

                g.DISCRETE_RCINTEGRATE(g.NODE_39,g.NODE_38,DK_R27, g.RES_2_PARALLEL(DK_R28,DK_R26+DK_R25),0,DK_C16,DK_SUP_V,g.DISC_RC_INTEGRATE_TYPE1),
                g.DISCRETE_MULTIPLY(DS_OUT_SOUND1,g.NODE_39,DK_R25/(DK_R26+DK_R25)),
                g.DISCRETE_TASK_END(),

                /************************************************/
                /* Walk                                         */
                /************************************************/
                g.DISCRETE_TASK_START(1),
                g.DISCRETE_INVERTER_OSC(g.NODE_51,1,0,DK_R47,DK_R48,DK_C30,0,dkong_inverter_osc_desc_walk),

#if DK_USE_CUSTOM
                /* custom mixer for 555 CV voltage */
                g.DISCRETE_CUSTOM8<discrete_dkong_custom_mixer_node>(g.NODE_54, DS_SOUND0_INV, g.NODE_51,
                            DK_R36, DK_R45, DK_R46, DK_R44, DK_C29, DK_SUP_V, null),
#else
                DISCRETE_LOGIC_INVERT(DS_SOUND0,DS_SOUND0_INV),
                DISCRETE_MULTIPLY(NODE_50,DS_SOUND0,DK_SUP_V),
                DISCRETE_TRANSFORM3(NODE_52,DS_SOUND0,DK_R46,R_SERIES(DK_R44,DK_R45),"01*2+"),
                DISCRETE_MIXER4(NODE_54, 1, NODE_50, NODE_51, DK_SUP_V, 0,&dkong_rc_walk_desc),
#endif

                /* 555 Voltage controlled */
                g.DISCRETE_555_ASTABLE_CV(g.NODE_55, 1, g.RES_K(47), g.RES_K(27), g.CAP_N(33), g.NODE_54, dkong_555_vco_desc),
                /* Trigger */
                g.DISCRETE_RCDISC_MODULATED(g.NODE_60,DS_SOUND0_INV,g.NODE_55,DK_R36,DK_R18,DK_R35,DK_R17,DK_C25,DK_SUP_V),
                /* Filter and divide - omitted C22 */
                g.DISCRETE_CRFILTER(g.NODE_61, g.NODE_60, DK_R15+DK_R16, DK_C23),
                g.DISCRETE_MULTIPLY(DS_OUT_SOUND2, g.NODE_61, DK_R15/(DK_R15+DK_R16)),
                g.DISCRETE_TASK_END(),

                /************************************************/
                /* DAC                                          */
                /************************************************/

                g.DISCRETE_TASK_START(1),
                /* Mixing - DAC */
                g.DISCRETE_ADJUSTMENT(DS_ADJ_DAC, 0, 1, g.DISC_LINADJ, "VR2"),

                /* Buffer DAC first to input stream 0 */
                g.DISCRETE_INPUT_BUFFER(DS_DAC, 0),
                //DISCRETE_INPUT_DATA(DS_DAC)
                /* Signal decay circuit Q7, R20, C32 */
                g.DISCRETE_RCDISC(g.NODE_70, DS_DISCHARGE_INV, 1, DK_R20, DK_C32),
                g.DISCRETE_TRANSFORM4(g.NODE_71, DS_DAC,  DK_SUP_V/256.0, g.NODE_70, DS_DISCHARGE_INV, "01*3!2+*"),

                /* following the DAC are two opamps. The first is a current-to-voltage changer
                 * for the DAC08 which delivers a variable output current.
                 *
                 * The second one is a Sallen Key filter ...
                 * http://www.t-linespeakers.org/tech/filters/Sallen-Key.html
                 * f = w / 2 / pi  = 1 / ( 2 * pi * 5.6k*sqrt(22n*10n)) = 1916 Hz
                 * Q = 1/2 * sqrt(22n/10n)= 0.74
                 */
                g.DISCRETE_SALLEN_KEY_FILTER(g.NODE_73, 1, g.NODE_71, g.DISC_SALLEN_KEY_LOW_PASS, dkong_sallen_key_info),

                /* Adjustment VR2 */
#if DK_NO_FILTERS
                DISCRETE_MULTIPLY(DS_OUT_DAC, NODE_71, DS_ADJ_DAC)
#else
                g.DISCRETE_MULTIPLY(DS_OUT_DAC, g.NODE_73, DS_ADJ_DAC),
#endif
                g.DISCRETE_TASK_END(),

                /************************************************/
                /* Amplifier                                    */
                /************************************************/

                g.DISCRETE_TASK_START(2),

                g.DISCRETE_MIXER4(g.NODE_288, 1, DS_OUT_SOUND0, DS_OUT_SOUND1, DS_OUT_DAC, DS_OUT_SOUND2, dkong_mixer_desc),

                /* Amplifier: internal amplifier */
                g.DISCRETE_ADDER2(g.NODE_289,1,g.NODE_288,5.0*43.0/(100.0+43.0)),
                g.DISCRETE_RCINTEGRATE(g.NODE_294,g.NODE_289,0,150,1000, g.CAP_U(33),DK_SUP_V,g.DISC_RC_INTEGRATE_TYPE3),
                g.DISCRETE_CRFILTER(g.NODE_295,g.NODE_294, g.RES_K(50), DK_C13),
                /*DISCRETE_CRFILTER(NODE_295,1,NODE_294, 1000, DK_C13) */
                /* EZV20 equivalent filter circuit ... */
                g.DISCRETE_CRFILTER(g.NODE_296,g.NODE_295, g.RES_K(1), g.CAP_U(4.7)),
#if DK_NO_FILTERS
                DISCRETE_OUTPUT(NODE_288, 32767.0/5.0 * 10)
#else
                g.DISCRETE_OUTPUT(g.NODE_296, 32767.0/5.0 * 3.41),
                /* Test */
                //DISCRETE_CSVLOG2(NODE_296, NODE_288)
                //DISCRETE_WAVELOG1(NODE_296, 32767.0/5.0 * 3.41)
#endif
                g.DISCRETE_TASK_END(),

            g.DISCRETE_SOUND_END,
        };


        /****************************************************************
         *
         * I/O Handlers - static
         *
         ****************************************************************/

        void dkong_voice_w(uint8_t data)
        {
            /* only provided for documentation purposes
             * not actually used
             */
            logerror("dkong_speech_w: 0x{0}\n", data);
        }


        uint8_t dkong_voice_status_r()
        {
            /* only provided for documentation purposes
             * not actually used
             */
            return 0;
        }


        uint8_t dkong_tune_r(offs_t offset)
        {
            uint8_t page = (uint8_t)(m_dev_vp2.op[0].read(0) & 0x47);

            if ((page & 0x40) != 0)
            {
                return (uint8_t)((m_ls175_3d.op[0].read(0) & 0x0f) | (dkong_voice_status_r() << 4));
            }
            else
            {
                /* printf("%s:rom access\n",machine().describe_context().c_str()); */
                return m_snd_rom.op[(int)(0x1000 + (page & 7) * 256 + offset)];
            }
        }


        void dkong_p1_w(uint8_t data)
        {
            m_discrete.op[0].write(DS_DAC, data);
        }


        /****************************************************************
         *
         * I/O Handlers - global
         *
         ****************************************************************/

        void dkong_audio_irq_w(uint8_t data)
        {
            if (data != 0)
                m_soundcpu.op[0].set_input_line(0, g.ASSERT_LINE);
            else
                m_soundcpu.op[0].set_input_line(0, g.CLEAR_LINE);
        }


        /*************************************
         *
         *  Sound CPU memory handlers
         *
         *************************************/
        void dkong_sound_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x0fff).rom();
        }


        void dkong_sound_io_map(address_map map, device_t device)
        {
            map.op(0x00, 0xff).rw(dkong_tune_r, dkong_voice_w);
        }


        /*************************************
         *
         *  Machine driver
         *
         *************************************/
        void dkong2b_audio(machine_config config)
        {
            /* sound latches */
            g.LATCH8(config, m_ls175_3d); /* sound cmd latch */
            m_ls175_3d.op[0].set_maskout(0xf0);
            m_ls175_3d.op[0].set_xorvalue(0x0f);

            g.LATCH8(config, m_dev_6h);
            m_dev_6h.op[0].write_cb<u32_const_0>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND0_INP>(state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND0_INP>));
            m_dev_6h.op[0].write_cb<u32_const_1>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND1_INP>(state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND1_INP>));
            m_dev_6h.op[0].write_cb<u32_const_2>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND2_INP>(state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND2_INP>));
            m_dev_6h.op[0].write_cb<u32_const_6>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND6_INP>(state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND6_INP>));
            m_dev_6h.op[0].write_cb<u32_const_7>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND7_INP>(state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND7_INP>));

            /*   If P2.Bit7 -> is apparently an external signal decay or other output control
             *   If P2.Bit6 -> activates the external compressed sample ROM (not radarscp1)
             *   If P2.Bit5 -> Signal ANSN ==> Grid enable (radarscp1)
             *   If P2.Bit4 -> status code to main cpu
             *   P2.Bit2-0  -> select the 256 byte bank for external ROM
             */

            g.LATCH8(config, m_dev_vp2);      /* virtual latch for port B */
            m_dev_vp2.op[0].set_xorvalue(0x20);  /* signal is inverted       */
            m_dev_vp2.op[0].read_cb<u32_const_5>().set(m_dev_6h, () => { return m_dev_6h.op[0].bit3_r(); }).reg();  //FUNC(latch8_device::bit3_r));
            m_dev_vp2.op[0].write_cb<u32_const_7>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_DISCHARGE_INV>(state); }).reg();  //FUNC(discrete_device::write_line<DS_DISCHARGE_INV>));

            g.MB8884(config, m_soundcpu, I8035_CLOCK);
            m_soundcpu.op[0].memory().set_addrmap(g.AS_PROGRAM, dkong_sound_map);
            m_soundcpu.op[0].memory().set_addrmap(g.AS_IO, dkong_sound_io_map);
            m_soundcpu.op[0].bus_in_cb().set(dkong_tune_r).reg();
            m_soundcpu.op[0].bus_out_cb().set(dkong_voice_w).reg();
            m_soundcpu.op[0].p1_out_cb().set(dkong_p1_w).reg(); // only write to dac
            m_soundcpu.op[0].p2_in_cb().set(m_dev_vp2, (offset) => { return m_dev_vp2.op[0].read(offset); }).reg();  //FUNC(latch8_device::read));
            m_soundcpu.op[0].p2_out_cb().set(m_dev_vp2, (offset, data) => { m_dev_vp2.op[0].write(offset, data); }).reg();  //FUNC(latch8_device::write));
            m_soundcpu.op[0].t0_in_cb().set(m_dev_6h, () => { return m_dev_6h.op[0].bit5_q_r(); }).reg();  //FUNC(latch8_device::bit5_q_r));
            m_soundcpu.op[0].t1_in_cb().set(m_dev_6h, () => { return m_dev_6h.op[0].bit4_q_r(); }).reg();  //FUNC(latch8_device::bit4_q_r));

            g.SPEAKER(config, "mono").front_center();
            g.DISCRETE(config, "discrete", dkong2b_discrete).disound.add_route(g.ALL_OUTPUTS, "mono", 1.0);
        }
    }
}
