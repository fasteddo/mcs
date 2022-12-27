// license:BSD-3-Clause
// copyright-holders:Edward Fast

/* Set to 1 to use faster custom mixer */
#define DK_USE_CUSTOM

/* FIXME: Review at a later time */
#define DK_REVIEW


using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;
using u8 = System.Byte;
using uint8_t = System.Byte;

using static mame.diexec_global;
using static mame.discrete_global;
using static mame.disound_global;
using static mame.emumem_global;
using static mame.latch8_global;
using static mame.mcs48_global;
using static mame.n2a03_global;
using static mame.rescap_global;
using static mame.speaker_global;


namespace mame
{
    partial class dkong_state : driver_device
    {
        // moved from \includes\dkong.cs so that it's init before used
        static readonly XTAL MASTER_CLOCK_audio      = new XTAL(61_440_000);
        static readonly XTAL CLOCK_1H_audio          = MASTER_CLOCK_audio / 5 / 4;
        static readonly XTAL CLOCK_16H_audio         = CLOCK_1H_audio / 16;
        static readonly XTAL CLOCK_1VF_audio         = CLOCK_16H_audio / 12 / 2;
        static readonly XTAL CLOCK_2VF               = CLOCK_1VF_audio / 2;


        /* Discrete sound inputs */

        const int DS_SOUND0_INV       = NODE_01;
        const int DS_SOUND1_INV       = NODE_02;
        const int DS_SOUND2_INV       = NODE_03;
        const int DS_SOUND6_INV       = NODE_04;
        const int DS_SOUND7_INV       = NODE_05;
        const int DS_SOUND9_INV       = NODE_06;
        const int DS_DAC              = NODE_07;
        const int DS_DISCHARGE_INV    = NODE_08;

        const int DS_SOUND0           = NODE_208;
        const int DS_SOUND1           = NODE_209;
        //#define DS_SOUND6           NODE_210
        //#define DS_SOUND7           NODE_211
        //#define DS_SOUND9           NODE_212

        const int DS_ADJ_DAC          = NODE_240;

        const int DS_OUT_SOUND0       = NODE_241;
        const int DS_OUT_SOUND1       = NODE_242;
        const int DS_OUT_SOUND2       = NODE_243;
        //#define DS_OUT_SOUND6       NODE_247
        //#define DS_OUT_SOUND7       NODE_248
        const int DS_OUT_SOUND9       = NODE_249;
        const int DS_OUT_DAC          = NODE_250;

        /* Input definitions for write handlers */

        const int DS_SOUND0_INP       = DS_SOUND0_INV;
        const int DS_SOUND1_INP       = DS_SOUND1_INV;
        const int DS_SOUND2_INP       = DS_SOUND2_INV;
        const int DS_SOUND6_INP       = DS_SOUND6_INV;
        const int DS_SOUND7_INP       = DS_SOUND7_INV;
        const int DS_SOUND9_INP       = DS_SOUND9_INV;


        public class int_const_DS_DISCHARGE_INV : int_const { public int value { get { return DS_DISCHARGE_INV; } } }
        public class int_const_DS_SOUND0_INP : int_const { public int value { get { return DS_SOUND0_INP; } } }
        public class int_const_DS_SOUND1_INP : int_const { public int value { get { return DS_SOUND1_INP; } } }
        public class int_const_DS_SOUND2_INP : int_const { public int value { get { return DS_SOUND2_INP; } } }
        public class int_const_DS_SOUND6_INP : int_const { public int value { get { return DS_SOUND6_INP; } } }
        public class int_const_DS_SOUND7_INP : int_const { public int value { get { return DS_SOUND7_INP; } } }
        public class int_const_DS_SOUND9_INP : int_const { public int value { get { return DS_SOUND9_INP; } } }


        /* General defines */

        const double DK_1N5553_V         = 0.4; /* from datasheet at 1mA */
        const double DK_SUP_V            = 5.0;
        //#define NE555_INTERNAL_R    RES_K(5)

        static double R_SERIES(double R1, double R2) { return R1 + R2; }


        /* Resistors */

        static readonly double DK_R1       = RES_K(47);
        static readonly double DK_R2       = RES_K(47);
        static readonly double DK_R3       = RES_K(5.1);
        static readonly double DK_R4       = RES_K(2);
        static readonly double DK_R5       = 750;
        static readonly double DK_R6       = RES_K(4.7);
        static readonly double DK_R7       = RES_K(10);
        static readonly double DK_R8       = RES_K(100);
        static readonly double DK_R9       = RES_K(10);
        static readonly double DK_R10      = RES_K(10);
        static readonly double DK_R14      = RES_K(47);

        static readonly double DK_R15      = RES_K(5.6);
        static readonly double DK_R16      = RES_K(5.6);
        static readonly double DK_R17      = RES_K(10);
        static readonly double DK_R18      = RES_K(4.7);
        static readonly double DK_R20      = RES_K(10);
        //#define DK_R21      RES_K(5.6)
        //#define DK_R22      RES_K(5.6)
        static readonly double DK_R24      = RES_K(47);
        static readonly double DK_R25      = RES_K(5.1);
        static readonly double DK_R26      = RES_K(2);
        static readonly double DK_R27      = 150;
        static readonly double DK_R28      = RES_K(4.7);
        static readonly double DK_R29      = RES_K(10);
        static readonly double DK_R30      = RES_K(100);
        static readonly double DK_R31      = RES_K(10);
        static readonly double DK_R32      = RES_K(10);
        static readonly double DK_R35      = RES_K(1);
        static readonly double DK_R36      = RES_K(1);
        static readonly double DK_R38      = RES_K(18);
        static readonly double DK_R39      = RES_M(3.3);
        static readonly double DK_R49      = RES_K(1.2);
        static readonly double DK_R44      = RES_K(1.2);
        static readonly double DK_R45      = RES_K(10);
        static readonly double DK_R46      = RES_K(12);
        static readonly double DK_R47      = RES_K(4.3);
        static readonly double DK_R48      = RES_K(43);
        static readonly double DK_R50      = RES_K(10);
        static readonly double DK_R51      = RES_K(10);

        /* Capacitors */

        //#define DK_C8       CAP_U(220)
        static readonly double DK_C12      = CAP_U(1);
        static readonly double DK_C13      = CAP_U(33);
        static readonly double DK_C16      = CAP_U(1);
        static readonly double DK_C17      = CAP_U(4.7);
        static readonly double DK_C18      = CAP_U(1);
        static readonly double DK_C19      = CAP_U(1);
        static readonly double DK_C20      = CAP_U(3.3);
        static readonly double DK_C21      = CAP_U(1);

        static readonly double DK_C23      = CAP_U(4.7);
        static readonly double DK_C24      = CAP_U(10);
        static readonly double DK_C26      = CAP_U(3.3);
        static readonly double DK_C25      = CAP_U(3.3);
        static readonly double DK_C29      = CAP_U(3.3);
        static readonly double DK_C30      = CAP_U(10);
        static readonly double DK_C32      = CAP_U(10);
        //#define DK_C34      CAP_N(10)
        static readonly double DK_C159     = CAP_N(100);


        /*
         * The noice generator consists of three LS164 8+8+8
         * the output signal is taken after the xor, so
         * taking bit 0 is not exact
         */

        static readonly discrete_lfsr_desc dkong_lfsr = new discrete_lfsr_desc
        (
            DISC_CLK_IS_FREQ,
            24,                   /* Bit Length */
            0,                    /* Reset Value */
            10,                   /* Use Bit 10 (QC of second LS164) as F0 input 0 */
            23,                   /* Use Bit 23 (QH of third LS164) as F0 input 1 */
            DISC_LFSR_XOR,        /* F0 is XOR */
            DISC_LFSR_NOT_IN0,    /* F1 is inverted F0*/
            DISC_LFSR_REPLACE,    /* F2 replaces the shifted register contents */
            0x000001,             /* Everything is shifted into the first bit only */
            DISC_LFSR_FLAG_OUTPUT_F0, /* Output is result of F0 */
            0                     /* Output bit */
        );

        static readonly double [] dkong_diode_mix_table = new double [2] { DK_1N5553_V, DK_1N5553_V * 2 };


        static readonly discrete_mixer_desc dkong_mixer_desc = new discrete_mixer_desc
        (
            DISC_MIXER_IS_RESISTOR,
            new double [] { DK_R2, DK_R24, DK_R1, DK_R14 },
            new int [] { 0, 0, 0 },  /* no variable resistors */
            new double [] { 0, 0, 0 },  /* no node capacitors */
#if DK_REVIEW
            0, RES_K(10),
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
            DISC_555_OUT_ENERGY | DISC_555_OUT_DC,
            DK_SUP_V,
            DEFAULT_555_CHARGE,
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
            RES_K(5.6), RES_K(5.6), 0, 0, 0,
            CAP_N(22), CAP_N(10), 0
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

            double NE555_CV_R { get { return RES_2_PARALLEL(RES_K(5), RES_K(10)); } }


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
                double  i_total = DKONG_CUSTOM_V / RES_K(5);
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
                m_r_in[0] = RES_2_PARALLEL((DKONG_CUSTOM_R1 + DKONG_CUSTOM_R2), DKONG_CUSTOM_R3);
                m_r_in[1] = RES_2_PARALLEL(DKONG_CUSTOM_R2, DKONG_CUSTOM_R3);
                /* precalculate total charging resistance */
                m_r_total[0] = RES_2_PARALLEL(m_r_in[0] + DKONG_CUSTOM_R4, NE555_CV_R);
                m_r_total[1] = RES_2_PARALLEL((m_r_in[1] + DKONG_CUSTOM_R4), NE555_CV_R);
                /* precalculate charging exponents */
                m_exp[0] = RC_CHARGE_EXP(m_r_total[0] * DKONG_CUSTOM_C);
                m_exp[1] = RC_CHARGE_EXP(m_r_total[1] * DKONG_CUSTOM_C);

                m_out_v = 0;
            }


            protected override int max_output() { return _maxout; }
        }
#endif


        //static DISCRETE_SOUND_START(dkong2b_discrete)
        static readonly discrete_block [] dkong2b_discrete = 
        {
            /************************************************/
            /* Input register mapping for dkong             */
            /************************************************/

            /* DISCRETE_INPUT_DATA */
            DISCRETE_INPUT_NOT(DS_SOUND2_INV),
                DISCRETE_INPUT_NOT(DS_SOUND1_INV),   /* IC 6J, pin 12 */
                DISCRETE_INPUT_NOT(DS_SOUND0_INV),   /* IC 6J, pin 2 */
                DISCRETE_INPUT_NOT(DS_DISCHARGE_INV),
                //DISCRETE_INPUT_DATA(DS_DAC)

                /************************************************/
                /* Stomp                                        */
                /************************************************/
                /* Noise */
                DISCRETE_TASK_START(1),
                DISCRETE_LFSR_NOISE(NODE_11, 1, 1, CLOCK_2VF.dvalue(), 1.0, 0, 0.5, dkong_lfsr),
                DISCRETE_COUNTER(NODE_12, 1, 0, NODE_11, 0, 7, DISC_COUNT_UP, 0, DISC_CLK_ON_R_EDGE),    /* LS161, IC 3J */
                DISCRETE_TRANSFORM3(NODE_13,NODE_12,3,DK_SUP_V,"01>2*"),

                /* Stomp */
                /* C21 is discharged via Q5 BE */
                DISCRETE_RCDISC_MODULATED(NODE_15,DS_SOUND2_INV,0,DK_R10,0,0,DK_R9,DK_C21,DK_SUP_V),
                /* Q5 */
                DISCRETE_TRANSFORM2(NODE_16, NODE_15, 0.6, "01>"),
                DISCRETE_RCDISC2(NODE_17,NODE_16,DK_SUP_V,DK_R8+DK_R7,0.0,DK_R7,DK_C20),

                DISCRETE_DIODE_MIXER2(NODE_20, NODE_17, NODE_13, dkong_diode_mix_table), /* D1, D2 + D3 */

                DISCRETE_RCINTEGRATE(NODE_22,NODE_20,DK_R5, RES_2_PARALLEL(DK_R4+DK_R3,DK_R6),0,DK_C19,DK_SUP_V,DISC_RC_INTEGRATE_TYPE1),
                DISCRETE_MULTIPLY(DS_OUT_SOUND0, NODE_22, DK_R3 / R_SERIES(DK_R3, DK_R4)),
                DISCRETE_TASK_END(),

                /************************************************/
                /* Jump                                         */
                /************************************************/
                /*  tt */
                /* 4049B Inverter Oscillator build from 3 inverters */
                DISCRETE_TASK_START(1),
                DISCRETE_INVERTER_OSC(NODE_25, 1, 0, DK_R38, DK_R39, DK_C26, 0, dkong_inverter_osc_desc_jump),

#if DK_USE_CUSTOM
                /* custom mixer for 555 CV voltage */
                DISCRETE_CUSTOM8<discrete_dkong_custom_mixer_node>(NODE_28, DS_SOUND1_INV, NODE_25,
                            DK_R32, DK_R50, DK_R51, DK_R49, DK_C24, DK_SUP_V, null),
#else
                DISCRETE_LOGIC_INVERT(DS_SOUND1,DS_SOUND1_INV),
                DISCRETE_MULTIPLY(NODE_24,DS_SOUND1,DK_SUP_V),
                DISCRETE_TRANSFORM3(NODE_26,DS_SOUND1,DK_R32,DK_R49+DK_R50,"01*2+"),
                DISCRETE_MIXER4(NODE_28, 1, NODE_24, NODE_25, DK_SUP_V, 0, &dkong_rc_jump_desc),
#endif
                /* 555 Voltage controlled */
                DISCRETE_555_ASTABLE_CV(NODE_29, 1, RES_K(47), RES_K(27), CAP_N(47), NODE_28,
                                        dkong_555_vco_desc),

                /* Jump trigger */
                DISCRETE_RCDISC_MODULATED(NODE_33,DS_SOUND1_INV,0,DK_R32,0,0,DK_R31,DK_C18,DK_SUP_V),

                DISCRETE_TRANSFORM2(NODE_34, NODE_33, 0.6, "01>"),
                DISCRETE_RCDISC2(NODE_35, NODE_34,DK_SUP_V,R_SERIES(DK_R30,DK_R29),0.0,DK_R29,DK_C17),

                DISCRETE_DIODE_MIXER2(NODE_38, NODE_35, NODE_29, dkong_diode_mix_table),

                DISCRETE_RCINTEGRATE(NODE_39,NODE_38,DK_R27, RES_2_PARALLEL(DK_R28,DK_R26+DK_R25),0,DK_C16,DK_SUP_V,DISC_RC_INTEGRATE_TYPE1),
                DISCRETE_MULTIPLY(DS_OUT_SOUND1,NODE_39,DK_R25/(DK_R26+DK_R25)),
                DISCRETE_TASK_END(),

                /************************************************/
                /* Walk                                         */
                /************************************************/
                DISCRETE_TASK_START(1),
                DISCRETE_INVERTER_OSC(NODE_51,1,0,DK_R47,DK_R48,DK_C30,0,dkong_inverter_osc_desc_walk),

#if DK_USE_CUSTOM
                /* custom mixer for 555 CV voltage */
                DISCRETE_CUSTOM8<discrete_dkong_custom_mixer_node>(NODE_54, DS_SOUND0_INV, NODE_51,
                            DK_R36, DK_R45, DK_R46, DK_R44, DK_C29, DK_SUP_V, null),
#else
                DISCRETE_LOGIC_INVERT(DS_SOUND0,DS_SOUND0_INV),
                DISCRETE_MULTIPLY(NODE_50,DS_SOUND0,DK_SUP_V),
                DISCRETE_TRANSFORM3(NODE_52,DS_SOUND0,DK_R46,R_SERIES(DK_R44,DK_R45),"01*2+"),
                DISCRETE_MIXER4(NODE_54, 1, NODE_50, NODE_51, DK_SUP_V, 0,&dkong_rc_walk_desc),
#endif

                /* 555 Voltage controlled */
                DISCRETE_555_ASTABLE_CV(NODE_55, 1, RES_K(47), RES_K(27), CAP_N(33), NODE_54, dkong_555_vco_desc),
                /* Trigger */
                DISCRETE_RCDISC_MODULATED(NODE_60,DS_SOUND0_INV,NODE_55,DK_R36,DK_R18,DK_R35,DK_R17,DK_C25,DK_SUP_V),
                /* Filter and divide - omitted C22 */
                DISCRETE_CRFILTER(NODE_61, NODE_60, DK_R15+DK_R16, DK_C23),
                DISCRETE_MULTIPLY(DS_OUT_SOUND2, NODE_61, DK_R15/(DK_R15+DK_R16)),
                DISCRETE_TASK_END(),

                /************************************************/
                /* DAC                                          */
                /************************************************/

                DISCRETE_TASK_START(1),
                /* Mixing - DAC */
                DISCRETE_ADJUSTMENT(DS_ADJ_DAC, 0, 1, DISC_LINADJ, "VR2"),

                /* Buffer DAC first to input stream 0 */
                DISCRETE_INPUT_BUFFER(DS_DAC, 0),
                //DISCRETE_INPUT_DATA(DS_DAC)
                /* Signal decay circuit Q7, R20, C32 */
                DISCRETE_RCDISC(NODE_70, DS_DISCHARGE_INV, 1, DK_R20, DK_C32),
                DISCRETE_TRANSFORM4(NODE_71, DS_DAC,  DK_SUP_V/256.0, NODE_70, DS_DISCHARGE_INV, "01*3!2+*"),

                /* following the DAC are two opamps. The first is a current-to-voltage changer
                 * for the DAC08 which delivers a variable output current.
                 *
                 * The second one is a Sallen Key filter ...
                 * http://www.t-linespeakers.org/tech/filters/Sallen-Key.html
                 * f = w / 2 / pi  = 1 / ( 2 * pi * 5.6k*sqrt(22n*10n)) = 1916 Hz
                 * Q = 1/2 * sqrt(22n/10n)= 0.74
                 */
                DISCRETE_SALLEN_KEY_FILTER(NODE_73, 1, NODE_71, DISC_SALLEN_KEY_LOW_PASS, dkong_sallen_key_info),

                /* Adjustment VR2 */
#if DK_NO_FILTERS
                DISCRETE_MULTIPLY(DS_OUT_DAC, NODE_71, DS_ADJ_DAC)
#else
                DISCRETE_MULTIPLY(DS_OUT_DAC, NODE_73, DS_ADJ_DAC),
#endif
                DISCRETE_TASK_END(),

                /************************************************/
                /* Amplifier                                    */
                /************************************************/

                DISCRETE_TASK_START(2),

                DISCRETE_MIXER4(NODE_288, 1, DS_OUT_SOUND0, DS_OUT_SOUND1, DS_OUT_DAC, DS_OUT_SOUND2, dkong_mixer_desc),

                /* Amplifier: internal amplifier */
                DISCRETE_ADDER2(NODE_289,1,NODE_288,5.0*43.0/(100.0+43.0)),
                DISCRETE_RCINTEGRATE(NODE_294,NODE_289,0,150,1000, CAP_U(33),DK_SUP_V,DISC_RC_INTEGRATE_TYPE3),
                DISCRETE_CRFILTER(NODE_295,NODE_294, RES_K(50), DK_C13),
                /*DISCRETE_CRFILTER(NODE_295,1,NODE_294, 1000, DK_C13) */
                /* EZV20 equivalent filter circuit ... */
                DISCRETE_CRFILTER(NODE_296,NODE_295, RES_K(1), CAP_U(4.7)),
#if DK_NO_FILTERS
                DISCRETE_OUTPUT(NODE_288, 32767.0/5.0 * 10)
#else
                DISCRETE_OUTPUT(NODE_296, 32767.0/5.0 * 3.41),
                /* Test */
                //DISCRETE_CSVLOG2(NODE_296, NODE_288)
                //DISCRETE_WAVELOG1(NODE_296, 32767.0/5.0 * 3.41)
#endif
                DISCRETE_TASK_END(),

            DISCRETE_SOUND_END,
        };


        /****************************************************************
         *
         * DkongJR Discrete Sound Interface
         *
         ****************************************************************/

        static readonly double JR_R2       = 120;
        static readonly double JR_R3       = RES_K(100);
        static readonly double JR_R4       = RES_K(47);
        static readonly double JR_R5       = RES_K(150);
        static readonly double JR_R6       = RES_K(20);
        static readonly double JR_R8       = RES_K(47);
        static readonly double JR_R9       = RES_K(47);
        static readonly double JR_R10      = RES_K(10);
        static readonly double JR_R11      = RES_K(20);
        static readonly double JR_R12      = RES_K(10);
        static readonly double JR_R13      = RES_K(47);
        static readonly double JR_R14      = RES_K(30);
        static readonly double JR_R17      = RES_K(47);
        static readonly double JR_R18      = RES_K(100);
        static readonly double JR_R19      = 100;
        static readonly double JR_R20      = RES_K(10);
        static readonly double JR_R24      = RES_K(4.7);
        static readonly double JR_R25      = RES_K(47);
        static readonly double JR_R27      = RES_K(10);
        static readonly double JR_R28      = RES_K(100);
        static readonly double JR_R33      = RES_K(1);
        static readonly double JR_R34      = RES_K(1);
        static readonly double JR_R35      = RES_K(1);


        static readonly double JR_C13      = CAP_U(4.7);
        static readonly double JR_C14      = CAP_U(4.7);
        static readonly double JR_C15      = CAP_U(22);
        static readonly double JR_C16      = CAP_U(3.3);
        static readonly double JR_C17      = CAP_U(3.3);
        static readonly double JR_C18      = CAP_N(22);
        static readonly double JR_C19      = CAP_N(4.7);
        //#define JR_C20      CAP_U(0.12)
        static readonly double JR_C21      = CAP_N(56);
        static readonly double JR_C22      = CAP_N(220);
        static readonly double JR_C23      = CAP_U(0.47);
        static readonly double JR_C24      = CAP_U(47);
        static readonly double JR_C25      = CAP_U(1);
        static readonly double JR_C26      = CAP_U(47);
        static readonly double JR_C27      = CAP_U(22);
        static readonly double JR_C28      = CAP_U(10);
        static readonly double JR_C29      = CAP_U(10);
        static readonly double JR_C30      = CAP_U(0.47);
        static readonly double JR_C32      = CAP_U(10);
        static readonly double JR_C37      = CAP_U(0.12);
        //#define JR_C39      CAP_U(0.47)
        static readonly double JR_C161     = CAP_U(1);
        static readonly double JR_C155     = CAP_U(0.01);

        const int TTL_HIGH    = 4;
        const int GND         = 0;


        /* KT = 0.25 for diode circuit, 0.33 else */

        public static discrete_block DISCRETE_LS123(int _N, double _T, double _R, double _C)  //#define DISCRETE_LS123(_N, _T, _R, _C) \
            { return DISCRETE_ONESHOTR(_N, 0, _T, TTL_HIGH, (0.25 * (_R) * (_C) * (1.0+700.0/(_R))), DISC_ONESHOT_RETRIG | DISC_ONESHOT_REDGE); }
        public static discrete_block DISCRETE_LS123_INV(int _N, double _T, double _R, double _C)  //#define DISCRETE_LS123_INV(_N, _T, _R, _C) \
            { return DISCRETE_ONESHOTR(_N, 0, _T, TTL_HIGH, (0.25 * (_R) * (_C) * (1.0+700.0/(_R))), DISC_ONESHOT_RETRIG | DISC_ONESHOT_REDGE | DISC_OUT_ACTIVE_LOW); }


        static readonly discrete_mixer_desc dkongjr_mixer_desc = new discrete_mixer_desc
        (
            DISC_MIXER_IS_RESISTOR,
            new double [] { JR_R5, JR_R3, JR_R6, JR_R4, JR_R25 },
            new int [] { 0,0,0,0,0 },    /* no variable resistors */
            new double [] { 0,0,0,0,0 },    /* no node capacitors */
            0, 0,
            JR_C155,        /* cF */
            JR_C161,        /* cAmp */
            0, 1
        );


        static readonly discrete_mixer_desc dkongjr_s1_mixer_desc = new discrete_mixer_desc
        (
            DISC_MIXER_IS_RESISTOR,
            new double [] { JR_R13, JR_R12 },
            new int [] { 0 }, new double [] { 0 }, 0, 0, JR_C24, 0, 0, 1     /* r_node{}, c{}, rI, rF, cF, cAmp, vRef, gain */
        );


        static readonly discrete_lfsr_desc dkongjr_lfsr = new discrete_lfsr_desc
        (
            DISC_CLK_IS_FREQ,
            16,                   /* Bit Length */
            0,                    /* Reset Value */
            2,                    /* Use Bit 2 (QC of first LS164) as F0 input 0 */
            15,                   /* Use Bit 15 (QH of secong LS164) as F0 input 1 */
            DISC_LFSR_XOR,        /* F0 is XOR */
            DISC_LFSR_NOT_IN0,    /* F1 is inverted F0*/
            DISC_LFSR_REPLACE,    /* F2 replaces the shifted register contents */
            0x000001,             /* Everything is shifted into the first bit only */
            DISC_LFSR_FLAG_OUTPUT_F0 | DISC_LFSR_FLAG_OUT_INVERT, /* Output is result of F0 */
            0                     /* Output bit */
        );


        const int DS_SOUND9_EN = DS_SOUND9_INV;


        //static DISCRETE_SOUND_START(dkongjr_discrete)
        static readonly discrete_block [] dkongjr_discrete = 
        {
            /************************************************/
            /* Input register mapping for dkongjr           */
            /************************************************/

            /* DISCRETE_INPUT_DATA */
            DISCRETE_INPUT_NOT(DS_SOUND0_INV),       /* IC 6J, pin 2 */
            DISCRETE_INPUT_NOT(DS_SOUND1_INV),       /* IC 6J, pin 12 */
            DISCRETE_INPUT_NOT(DS_SOUND2_INV),       /* IC 6J, pin 4 */
            DISCRETE_INPUT_NOT(DS_SOUND6_INV),       /* unused */
            DISCRETE_INPUT_NOT(DS_SOUND7_INV),       /* IC 5J, pin 12 */
            DISCRETE_INPUT_LOGIC(DS_SOUND9_EN),      /* IC 7N pin 10 from IC 5J, pin 4 */
            DISCRETE_INPUT_NOT(DS_DISCHARGE_INV),    /* IC 7H, pin 38 */

            /************************************************
             * SOUND0 - walking
             ************************************************/

            DISCRETE_TASK_START(1),
                DISCRETE_COUNTER(NODE_100,                  /* IC 6L */
                    1, 0,                                   /* ENAB; RESET */
                    NODE_118,                               /* CLK - IC 6L, pin 10 */
                    0, 0x3FFF, DISC_COUNT_UP, 0, DISC_CLK_BY_COUNT | DISC_OUT_HAS_XTIME),

                DISCRETE_BIT_DECODE(NODE_101,               /* IC 6L, pin 6 */
                    NODE_100,  6, 0),                        /* output x_time logic */
                DISCRETE_BIT_DECODE(NODE_102,               /* IC 6L, pin 7 */
                    NODE_100,  3, 0),                        /* output x_time logic */
                DISCRETE_BIT_DECODE(NODE_103,               /* IC 6L, pin 2 */
                    NODE_100, 12, 0),                        /* output x_time logic */
                DISCRETE_BIT_DECODE(NODE_104,               /* IC 6L, pin 1 */
                    NODE_100, 11, 0),                        /* output x_time logic */

                /* LS157 Switches - IC 6K */
                DISCRETE_SWITCH(NODE_106,                       /* IC 6K, pin 7 */
                    1, DS_SOUND7_INV,                           /* ENAB; IC 6K, pin 1 */
                    NODE_101, NODE_102),                         /* IC 6K, pin 5; pin 6 */
                DISCRETE_SWITCH(NODE_107,                       /* IC 6K, pin 9 */
                    1, DS_SOUND7_INV,                           /* ENAB; IC 6K, pin 1 */
                    NODE_103, NODE_104),                         /* IC 6K, pin 11; pin 10 */

                DISCRETE_LS123(NODE_110,                        /* IC 4K, pin 5 */
                    DS_SOUND0_INV,                              /* IC 4K, pin 10 */
                    JR_R8, JR_C14),
                DISCRETE_SWITCH(NODE_111,                       /* IC 4F, pin 10 (inverter) */
                    1, NODE_110,                                /* ENAB; IC 4F, pin 11 */
                    4.14, 0.151),                                /* INP0; INP1 (measured) */

                /* Breadboarded measurements IC 5K, pin 7
                   D.R. Oct 2010
                    V       Hz
                    0.151   3139
                    0.25    2883
                    0.5     2820
                    0.75    3336
                    1       3805
                    2       6498
                    3       9796
                    4       13440
                    4.14    13980
                */

                DISCRETE_74LS624(NODE_113,                      /* IC 5K, pin 7 */
                    1,                                          /* ENAB */
                    NODE_111, DK_SUP_V,                         /* VMOD - IC 5K, pin 2; VRNG */
                    JR_C18, JR_R10, JR_C17, JR_R33,             /* C; R_FREQ_IN; C_FREQ_IN; R_RNG_IN */
                    DISC_LS624_OUT_LOGIC_X),
                DISCRETE_SWITCH(NODE_105,                       /* IC 6K, pin 4 */
                    1,                                          /* ENAB */
                    DS_SOUND7_INV,                              /* SWITCH, IC 6K, pin 1 */
                    GND, NODE_113),                              /* IC 6K, pin 2; pin 3 */

                DISCRETE_XTIME_XOR(NODE_115,                    /* IC 6N, pin 3 */
                    NODE_105, NODE_106,                         /* IC 6N, pin 1; pin 2 */
                    0, 0),                                       /* use x_time logic */

                DISCRETE_XTIME_INVERTER(NODE_116,               /* IC 5J, pin 8 */
                    NODE_107,                                   /* IC 5J, pin 9 */
                    0.135, 4.15),                                /* measured Low/High */

                /* Breadboarded measurements IC 5K, pin 10
                   D.R. Oct 2010
                    V       Hz
                    0.135   14450 - measured 74LS04 low
                    0.25    13320
                    0.5     12980
                    0.75    15150
                    1       17270
                    2       28230
                    3       41910
                    4       56950
                    4.15    59400 - measured 74LS04 high
                */

                DISCRETE_74LS624(NODE_118,                      /* IC 5K, pin 10 */
                    1,                                          /* ENAB */
                    NODE_116, DK_SUP_V,                         /* VMOD - IC 5K, pin 1; VRNG */
                    JR_C19, JR_R11, JR_C16, JR_R33,             /* C; R_FREQ_IN; C_FREQ_IN; R_RNG_IN */
                    DISC_LS624_OUT_COUNT_F_X),
                DISCRETE_SWITCH(NODE_119, 1, NODE_110, 0, 1),    /* convert from voltage to x_time logic */
                DISCRETE_XTIME_NAND(DS_OUT_SOUND0,              /* IC 5N, pin 11 */
                    NODE_119, NODE_115,                         /* IC 5N, pin 13; pin 12 */
                    0.2, 4.9),                                   /* LOW; HIGH (1k pullup to 5V) */
            DISCRETE_TASK_END(),

            /************************************************
             * SOUND1  - Jump
             ************************************************/

            DISCRETE_TASK_START(2),
                /* needs NODE_104 from TASK(1) ready */
                DISCRETE_LS123(NODE_10,                         /* IC 4K, pin 13 */
                    DS_SOUND1_INV,                              /* IC 4K, pin 8 */
                    JR_R9, JR_C15),
                DISCRETE_SWITCH(NODE_11,                        /* IC 7N, pin 6 */
                    1, NODE_10,                                 /* ENAB; SWITCH - IC 7N, pin 5 */
                    0.151, 4.14),                                /* measured Low/High */
                DISCRETE_XTIME_INVERTER(NODE_12,                /* IC 7N, pin 4 */
                    NODE_104,                                   /* IC 7N, pin 3 */
                    0.151, 4.14),                                /* measured Low/High */
                DISCRETE_MIXER2(NODE_13, 1, NODE_11, NODE_12, dkongjr_s1_mixer_desc),

                /* Breadboarded measurements IC 8L, pin 10
                   D.R. Oct 2010
                    V       Hz
                    0.151   313
                    0.25    288
                    0.5     275
                    0.75    324
                    1       370
                    2       635
                    3       965
                    4       1325
                    4.14    1378
                */

                DISCRETE_74LS624(NODE_14,                       /* IC 8L, pin 10 */
                    1,                                          /* ENAB */
                    NODE_13, DK_SUP_V,                          /* VMOD - IC 8L, pin 1, VRNG */
                    /* C_FREQ_IN is taken care of by the NODE_13 mixer */
                    JR_C22, RES_2_PARALLEL(JR_R13, JR_R12), 0, JR_R35,  /* C; R_FREQ_IN; C_FREQ_IN; R_RNG_IN */
                    DISC_LS624_OUT_ENERGY),

                DISCRETE_LOGIC_INVERT(NODE_15, NODE_10),         /* fake invert for NODE_16 */
                DISCRETE_RCDISC_MODULATED(NODE_16,              /* Q3, collector */
                    NODE_15, NODE_14, 120, JR_R27, RES_K(0.001), JR_R28, JR_C28, DK_SUP_V),
                /* The following circuit does not match 100%, however works.
                 * To be exact, we need a C-R-C-R circuit, we actually do not have.
                 */
                DISCRETE_CRFILTER_VREF(NODE_17, NODE_16, JR_R4, JR_C23, 2.5),
                DISCRETE_RCFILTER(DS_OUT_SOUND1, NODE_17, JR_R19, JR_C21),
            DISCRETE_TASK_END(),

            /************************************************
             * SOUND2 - climbing
             ************************************************/

            DISCRETE_TASK_START(1),
                /* the noise source clock is a 74LS629 IC 7P, pin 10.
                 * using JR_C20 as the timing cap, with Freq Control tied to 0V
                 * and Range tied to 5V.  This creates a fixed frequency of 710Hz.
                 * So for speed, I breadboarded and measured the frequency.
                 * Oct 2009, D.R.
                 */
                DISCRETE_LFSR_NOISE(NODE_21, 1, 1, 710, 1.0, 0, 0.5, dkongjr_lfsr),     /* IC 3J & 4J */
                DISCRETE_LS123_INV(NODE_25,                     /* IC 8N, pin 13 (fake inverted for use by NODE_26) */
                    DS_SOUND2_INV,                              /* IC 8N, pin 8 */
                    JR_R17, JR_C27),
                DISCRETE_RCDISC_MODULATED(NODE_26,              /* Q2, collector */
                    NODE_25, NODE_21, 120, JR_R24, RES_K(0.001), JR_R18, JR_C29, DK_SUP_V),
                /* The following circuit does not match 100%, however works.
                 * To be exact, we need a C-R-C-R circuit, we actually do not have.
                 */
                DISCRETE_CRFILTER_VREF(NODE_27, NODE_26, JR_R6, JR_C30, 2.5),
                DISCRETE_RCFILTER(DS_OUT_SOUND2, NODE_27, JR_R2, JR_C25),
            DISCRETE_TASK_END(),

            /************************************************
             * SOUND9 - Falling
             ************************************************/

            DISCRETE_TASK_START(1),
                DISCRETE_XTIME_INVERTER(NODE_90,        /* IC 7N, pin 8 */
                    DS_SOUND9_EN,                       /* IC 7N, pin 9 */
                    0.134, 4.16),                        /* measured Low/High */

                /* Breadboarded measurements IC 7P, pin 7
                   D.R. Oct 2010
                    V       Hz
                    0.134   570
                    0.25    538
                    0.5     489
                    0.75    560
                    1       636
                    2       1003
                    3       1484
                    4       2016
                    4.16    2111
                */
                DISCRETE_74LS624(NODE_91,               /* IC 7P, pin 7 */
                    1,                                  /* ENAB */
                    NODE_90, DK_SUP_V,                  /* VMOD - IC 7P, pin 2, VRNG */
                    JR_C37, JR_R14, JR_C26, JR_R34,     /* C; R_FREQ_IN; C_FREQ_IN; R_RNG_IN */
                    DISC_LS624_OUT_LOGIC_X),
                DISCRETE_XTIME_NAND(DS_OUT_SOUND9,      /* IC 5N, pin 8 */
                    DS_SOUND9_EN,                       /* IC 5N, pin 9 */
                    NODE_91,                            /* IC 5N, pin 10 */
                    0.2, 4.9),                           /* LOW, HIGH (1k pullup to 5V) */
            DISCRETE_TASK_END(),

            /************************************************
             * DAC
             ************************************************/

            DISCRETE_TASK_START(1),
                DISCRETE_INPUT_BUFFER(DS_DAC, 0),
                /* Signal decay circuit Q7, R20, C32 */
                DISCRETE_RCDISC(NODE_170, DS_DISCHARGE_INV, 1, JR_R20, JR_C32),
                DISCRETE_TRANSFORM4(NODE_171, DS_DAC,  DK_SUP_V/256.0, NODE_170, DS_DISCHARGE_INV, "01*3!2+*"),

                /* following the DAC are two opamps. The first is a current-to-voltage changer
                 * for the DAC08 which delivers a variable output current.
                 *
                 * The second one is a Sallen Key filter ...
                 * http://www.t-linespeakers.org/tech/filters/Sallen-Key.html
                 * f = w / 2 / pi  = 1 / ( 2 * pi * 5.6k*sqrt(22n*10n)) = 1916 Hz
                 * Q = 1/2 * sqrt(22n/10n)= 0.74
                 */

                DISCRETE_SALLEN_KEY_FILTER(DS_OUT_DAC, 1, NODE_171, DISC_SALLEN_KEY_LOW_PASS, dkong_sallen_key_info),
            DISCRETE_TASK_END(),

            /************************************************
             * Amplifier
             ************************************************/

            DISCRETE_TASK_START(3),
                DISCRETE_MIXER5(NODE_288, 1, DS_OUT_SOUND9, DS_OUT_SOUND0, DS_OUT_SOUND2, DS_OUT_SOUND1, DS_OUT_DAC, dkongjr_mixer_desc),

                /* Amplifier: internal amplifier
                 * Just a 1:n amplifier without filters - just the output filter
                 */
                DISCRETE_CRFILTER(NODE_295, NODE_288, 1000, JR_C13),
                /* approx -1.805V to 2.0V when playing, but turn on sound peaks at 2.36V */
                /* we will set the full wav range to 1.18V which will cause clipping on the turn on
                 * sound and explosions.  The real game would do this when the volume is turned up too.
                 * Reducing MAME's master volume to 50% will provide full unclipped volume.
                 */
                DISCRETE_OUTPUT(NODE_295, 32767.0/1.18),
            DISCRETE_TASK_END(),

            DISCRETE_SOUND_END,
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
            uint8_t page = (uint8_t)(m_dev_vp2.op0.read(0) & 0x47);

            if ((page & 0x40) != 0)
            {
                return (uint8_t)((m_ls175_3d.op0.read(0) & 0x0f) | (dkong_voice_status_r() << 4));
            }
            else
            {
                /* printf("%s:rom access\n",machine().describe_context().c_str()); */
                return m_snd_rom.op[(int)(0x1000 + (page & 7) * 256 + offset)];
            }
        }


        void dkong_p1_w(uint8_t data)
        {
            m_discrete.op0.write(DS_DAC, data);
        }


        /****************************************************************
         *
         * I/O Handlers - global
         *
         ****************************************************************/

        void dkong_audio_irq_w(uint8_t data)
        {
            if (data != 0)
                m_soundcpu.op0.set_input_line(0, ASSERT_LINE);
            else
                m_soundcpu.op0.set_input_line(0, CLEAR_LINE);
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


        void dkongjr_sound_io_map(address_map map, device_t device)
        {
            map.op(0x00, 0x00).mirror(0xff).r("ls174.3d", (offs_t offset) => { return ((latch8_device)subdevice("ls174.3d")).read(offset); });
        }


        void dkong3_sound1_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x07ff).ram();
            map.op(0x4016, 0x4016).r("latch1", (offs_t offset) => { return ((latch8_device)subdevice("latch1")).read(offset); });       /* overwrite default */
            map.op(0x4017, 0x4017).r("latch2", (offs_t offset) => { return ((latch8_device)subdevice("latch2")).read(offset); });
            map.op(0xe000, 0xffff).rom();
        }


        void dkong3_sound2_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x07ff).ram();
            map.op(0x4016, 0x4016).r("latch3", (offs_t offset) => { return ((latch8_device)subdevice("latch2")).read(offset); });       /* overwrite default */
            map.op(0xe000, 0xffff).rom();
        }


        /*************************************
         *
         *  Machine driver
         *
         *************************************/
        void dkong2b_audio(machine_config config)
        {
            /* sound latches */
            LATCH8(config, m_ls175_3d); /* sound cmd latch */
            m_ls175_3d.op0.set_maskout(0xf0);
            m_ls175_3d.op0.set_xorvalue(0x0f);

            LATCH8(config, m_dev_6h);
            m_dev_6h.op0.write_cb<u32_const_0>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND0_INP>(state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND0_INP>));
            m_dev_6h.op0.write_cb<u32_const_1>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND1_INP>(state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND1_INP>));
            m_dev_6h.op0.write_cb<u32_const_2>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND2_INP>(state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND2_INP>));
            m_dev_6h.op0.write_cb<u32_const_6>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND6_INP>(state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND6_INP>));
            m_dev_6h.op0.write_cb<u32_const_7>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND7_INP>(state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND7_INP>));

            /*   If P2.Bit7 -> is apparently an external signal decay or other output control
             *   If P2.Bit6 -> activates the external compressed sample ROM (not radarscp1)
             *   If P2.Bit5 -> Signal ANSN ==> Grid enable (radarscp1)
             *   If P2.Bit4 -> status code to main cpu
             *   P2.Bit2-0  -> select the 256 byte bank for external ROM
             */

            LATCH8(config, m_dev_vp2);      /* virtual latch for port B */
            m_dev_vp2.op0.set_xorvalue(0x20);  /* signal is inverted       */
            m_dev_vp2.op0.read_cb<u32_const_5>().set(m_dev_6h, () => { return m_dev_6h.op0.bit3_r(); }).reg();  //FUNC(latch8_device::bit3_r));
            m_dev_vp2.op0.write_cb<u32_const_7>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_DISCHARGE_INV>(state); }).reg();  //FUNC(discrete_device::write_line<DS_DISCHARGE_INV>));

            MB8884(config, m_soundcpu, I8035_CLOCK);
            m_soundcpu.op0.memory().set_addrmap(AS_PROGRAM, dkong_sound_map);
            m_soundcpu.op0.memory().set_addrmap(AS_IO, dkong_sound_io_map);
            m_soundcpu.op0.bus_in_cb().set(dkong_tune_r).reg();
            m_soundcpu.op0.bus_out_cb().set(dkong_voice_w).reg();
            m_soundcpu.op0.p1_out_cb().set(dkong_p1_w).reg(); // only write to dac
            m_soundcpu.op0.p2_in_cb().set(m_dev_vp2, (offset) => { return m_dev_vp2.op0.read(offset); }).reg();  //FUNC(latch8_device::read));
            m_soundcpu.op0.p2_out_cb().set(m_dev_vp2, (offset, data) => { m_dev_vp2.op0.write(offset, data); }).reg();  //FUNC(latch8_device::write));
            m_soundcpu.op0.t0_in_cb().set(m_dev_6h, () => { return m_dev_6h.op0.bit5_q_r(); }).reg();  //FUNC(latch8_device::bit5_q_r));
            m_soundcpu.op0.t1_in_cb().set(m_dev_6h, () => { return m_dev_6h.op0.bit4_q_r(); }).reg();  //FUNC(latch8_device::bit4_q_r));

            SPEAKER(config, "mono").front_center();
            DISCRETE(config, "discrete", dkong2b_discrete).disound.add_route(ALL_OUTPUTS, "mono", 0.9);
        }


        void dkongjr_audio(machine_config config)
        {
            /* sound latches */
            LATCH8(config, "ls174.3d").set_maskout(0xe0);

            LATCH8(config, m_dev_6h);
            m_dev_6h.op0.write_cb<u32_const_0>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND0_INP>(state); }).reg();
            m_dev_6h.op0.write_cb<u32_const_1>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND1_INP>(state); }).reg();
            m_dev_6h.op0.write_cb<u32_const_2>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND2_INP>(state); }).reg();
            m_dev_6h.op0.write_cb<u32_const_7>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND7_INP>(state); }).reg();

            latch8_device dev_5h = LATCH8(config, "ls259.5h");
            dev_5h.write_cb<u32_const_1>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_SOUND9_INP>(state); }).reg();

            LATCH8(config, "ls259.4h");

            LATCH8(config, m_dev_vp2);      /* virtual latch for port B */
            m_dev_vp2.op0.set_xorvalue(0x70);  /* all signals are inverted */
            m_dev_vp2.op0.read_cb<u32_const_6>().set("ls259.4h", () => { return ((latch8_device)subdevice("ls259.4h")).bit1_r(); }).reg();
            m_dev_vp2.op0.read_cb<u32_const_5>().set(m_dev_6h, () => { return m_dev_6h.op0.bit3_r(); }).reg();
            m_dev_vp2.op0.read_cb<u32_const_4>().set(m_dev_6h, () => { return m_dev_6h.op0.bit6_r(); }).reg();
            m_dev_vp2.op0.write_cb<u32_const_7>().set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line<int_const_DS_DISCHARGE_INV>(state); }).reg();

            MB8884(config, m_soundcpu, I8035_CLOCK);
            m_soundcpu.op0.memory().set_addrmap(AS_PROGRAM, dkong_sound_map);
            m_soundcpu.op0.memory().set_addrmap(AS_IO, dkongjr_sound_io_map);
            m_soundcpu.op0.p1_out_cb().set(dkong_p1_w).reg(); // only write to dac
            m_soundcpu.op0.p2_in_cb().set(m_dev_vp2, (offset) => { return m_dev_vp2.op0.read(offset); }).reg();
            m_soundcpu.op0.p2_out_cb().set(m_dev_vp2, (offset, data) => { m_dev_vp2.op0.write(offset, data); }).reg();
            m_soundcpu.op0.t0_in_cb().set(m_dev_6h, () => { return m_dev_6h.op0.bit5_q_r(); }).reg();
            m_soundcpu.op0.t1_in_cb().set(m_dev_6h, () => { return m_dev_6h.op0.bit4_q_r(); }).reg();

            SPEAKER(config, "mono").front_center();

            DISCRETE(config, "discrete", dkongjr_discrete).disound.add_route(ALL_OUTPUTS, "mono", 0.5);
        }


        void dkong3_audio(machine_config config)
        {
            SPEAKER(config, "mono").front_center();

            n2a03_device n2a03a = N2A03(config, "n2a03a", NTSC_APU_CLOCK);
            n2a03a.memory().set_addrmap(AS_PROGRAM, dkong3_sound1_map);
            n2a03a.dimixer.add_route(ALL_OUTPUTS, "mono", 0.50);

            n2a03_device n2a03b = N2A03(config, "n2a03b", NTSC_APU_CLOCK);
            n2a03b.memory().set_addrmap(AS_PROGRAM, dkong3_sound2_map);
            n2a03b.dimixer.add_route(ALL_OUTPUTS, "mono", 0.50);

            /* sound latches */
            LATCH8(config, "latch1");
            LATCH8(config, "latch2");
            LATCH8(config, "latch3");
        }
    }
}
