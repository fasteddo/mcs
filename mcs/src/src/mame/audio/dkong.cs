// license:BSD-3-Clause
// copyright-holders:Edward Fast

/* Set to 1 to use faster custom mixer */
#define DK_USE_CUSTOM

/* FIXME: Review at a later time */
#define DK_REVIEW


using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using osd_ticks_t = System.UInt64;
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

        const int DS_SOUND0_INV       = NODE_01;
        const int DS_SOUND1_INV       = NODE_02;
        const int DS_SOUND2_INV       = NODE_03;
        const int DS_SOUND6_INV       = NODE_04;
        const int DS_SOUND7_INV       = NODE_05;
        //#define DS_SOUND9_INV       NODE_06
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
        //#define DS_OUT_SOUND9       NODE_249
        const int DS_OUT_DAC          = NODE_250;

        /* Input definitions for write handlers */

        const int DS_SOUND0_INP       = DS_SOUND0_INV;
        const int DS_SOUND1_INP       = DS_SOUND1_INV;
        const int DS_SOUND2_INP       = DS_SOUND2_INV;
        const int DS_SOUND6_INP       = DS_SOUND6_INV;
        const int DS_SOUND7_INP       = DS_SOUND7_INV;
        //#define DS_SOUND9_INP       DS_SOUND9_INV

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
        static readonly discrete_block [] dkong2b_discrete = new discrete_block []
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
         * I/O Handlers - static
         *
         ****************************************************************/

        //WRITE8_MEMBER(dkong_state::dkong_voice_w)
        void dkong_voice_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            /* only provided for documentation purposes
             * not actually used
             */
            logerror("dkong_speech_w: 0x{0}\n", data);
        }


        //READ8_MEMBER(dkong_state::dkong_voice_status_r)
        u8 dkong_voice_status_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            /* only provided for documentation purposes
             * not actually used
             */
            return 0;
        }


        //READ8_MEMBER(dkong_state::dkong_tune_r)
        u8 dkong_tune_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            uint8_t page = (uint8_t)(m_dev_vp2.target.read(0) & 0x47);

            if ((page & 0x40) != 0)
            {
                return (u8)((m_ls175_3d.target.read(0) & 0x0F) | (dkong_voice_status_r(space, 0) << 4));
            }
            else
            {
                /* printf("%s:rom access\n",machine().describe_context().c_str()); */
                return m_snd_rom.target[(int)(0x1000 + (page & 7) * 256 + offset)];
            }
        }


        //WRITE8_MEMBER(dkong_state::dkong_p1_w)
        void dkong_p1_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_discrete.target.write(DS_DAC, data);
        }


        /****************************************************************
         *
         * I/O Handlers - global
         *
         ****************************************************************/

        //WRITE8_MEMBER(dkong_state::dkong_audio_irq_w)
        void dkong_audio_irq_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            if (data != 0)
                m_soundcpu.target.set_input_line(0, ASSERT_LINE);
            else
                m_soundcpu.target.set_input_line(0, CLEAR_LINE);
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
            LATCH8(config, m_ls175_3d); /* sound cmd latch */
            m_ls175_3d.target.set_maskout(0xf0);
            m_ls175_3d.target.set_xorvalue(0x0f);

            LATCH8(config, m_dev_6h);
            m_dev_6h.target.write_cb(0).set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line(DS_SOUND0_INP, state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND0_INP>));
            m_dev_6h.target.write_cb(1).set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line(DS_SOUND1_INP, state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND1_INP>));
            m_dev_6h.target.write_cb(2).set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line(DS_SOUND2_INP, state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND2_INP>));
            m_dev_6h.target.write_cb(6).set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line(DS_SOUND6_INP, state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND6_INP>));
            m_dev_6h.target.write_cb(7).set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line(DS_SOUND7_INP, state); }).reg();  //FUNC(discrete_device::write_line<DS_SOUND7_INP>));

            /*   If P2.Bit7 -> is apparently an external signal decay or other output control
             *   If P2.Bit6 -> activates the external compressed sample ROM (not radarscp1)
             *   If P2.Bit5 -> Signal ANSN ==> Grid enable (radarscp1)
             *   If P2.Bit4 -> status code to main cpu
             *   P2.Bit2-0  -> select the 256 byte bank for external ROM
             */

            LATCH8(config, m_dev_vp2);      /* virtual latch for port B */
            m_dev_vp2.target.set_xorvalue(0x20);  /* signal is inverted       */
            m_dev_vp2.target.read_cb(5).set(m_dev_6h, () => { return m_dev_6h.target.bit3_r(); }).reg();  //FUNC(latch8_device::bit3_r));
            m_dev_vp2.target.write_cb(7).set("discrete", (int state) => { ((discrete_device)subdevice("discrete")).write_line(DS_DISCHARGE_INV, state); }).reg();  //FUNC(discrete_device::write_line<DS_DISCHARGE_INV>));

            MB8884(config, m_soundcpu, I8035_CLOCK);
            m_soundcpu.target.memory().set_addrmap(AS_PROGRAM, dkong_sound_map);
            m_soundcpu.target.memory().set_addrmap(AS_IO, dkong_sound_io_map);
            m_soundcpu.target.bus_in_cb().set(dkong_tune_r).reg();
            m_soundcpu.target.bus_out_cb().set(dkong_voice_w).reg();
            m_soundcpu.target.p1_out_cb().set(dkong_p1_w).reg(); // only write to dac
            m_soundcpu.target.p2_in_cb().set("virtual_p2", (offset) => { return ((latch8_device)subdevice("virtual_p2")).read(offset); }).reg();  //FUNC(latch8_device::read));
            m_soundcpu.target.p2_out_cb().set("virtual_p2", (offset, data) => { ((latch8_device)subdevice("virtual_p2")).write(offset, data); }).reg();  //FUNC(latch8_device::write));
            m_soundcpu.target.t0_in_cb().set("ls259.6h", () => { return ((latch8_device)subdevice("ls259.6h")).bit5_q_r(); }).reg();  //FUNC(latch8_device::bit5_q_r));
            m_soundcpu.target.t1_in_cb().set("ls259.6h", () => { return ((latch8_device)subdevice("ls259.6h")).bit4_q_r(); }).reg();  //FUNC(latch8_device::bit4_q_r));

            SPEAKER(config, "mono").front_center();
            DISCRETE(config, "discrete", dkong2b_discrete).disound.add_route(ALL_OUTPUTS, "mono", 1.0);
        }
    }
}
