// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;
using uint8_t = System.Byte;

using static mame.discrete_global;
using static mame.disound_global;
using static mame.pokey_global;
using static mame.rescap_global;
using static mame.speaker_global;
using static mame.util;


namespace mame
{
    partial class bzone_state : driver_device
    {
        /* This sets an amount of gain boost to apply to the final signal
         * that will drive it into clipping.  The slider is ajusted by the
         * reverse factor, so that the final result is not clipped.
         * This allows for the user to easily adjust the sound into the clipping
         * range so it sounds more like a real cabinet.
         */
        //#define BZ_FINAL_GAIN   2

        const int BZ_NOISE_CLOCK      = 12000;

        const double TTL_OUT = 3.4;

        /*************************************
         *
         *  Discrete Sound Defines
         *
         *************************************/

        /* Discrete Sound Input Nodes */
        const int BZ_INPUT            = NODE_01;     /* at M2 LS273 */
        const int BZ_INP_EXPLO        = NODE_10_00;
        const int BZ_INP_EXPLOLS      = NODE_10_01;
        const int BZ_INP_SHELL        = NODE_10_02;
        //#define BZ_INP_SHELLLS      NODE_10_03
        const int BZ_INP_ENGREV       = NODE_10_04;
        const int BZ_INP_SOUNDEN      = NODE_10_05;
        //#define BZ_INP_STARTLED     NODE_10_06
        const int BZ_INP_MOTEN        = NODE_10_07;

        /* Adjusters */
        const int BZ_R11_POT          = NODE_11;

        /* Discrete Sound Output Nodes */
        const int BZ_NOISE            = NODE_20;
        const int BZ_SHELL_SND        = NODE_21;
        const int BZ_EXPLOSION_SND    = NODE_22;
        const int BZ_ENGINE_SND       = NODE_23;
        const int BZ_POKEY_SND        = NODE_24;

        /* Parts List - Resistors */
        static readonly double BZ_R5           = RES_K(1);
        static readonly double BZ_R6           = RES_K(4.7);
        static readonly double BZ_R7           = RES_K(1);
        static readonly double BZ_R8           = RES_K(100);
        static readonly double BZ_R9           = RES_K(22);
        static readonly double BZ_R10          = RES_K(100);
        static readonly double BZ_R11          = RES_K(250);
        static readonly double BZ_R12          = RES_K(33);
        static readonly double BZ_R13          = RES_K(10);
        static readonly double BZ_R14          = RES_K(22);
        static readonly double BZ_R15          = RES_K(1);
        static readonly double BZ_R16          = RES_K(1);
        static readonly double BZ_R17          = RES_K(22);
        static readonly double BZ_R18          = RES_K(10);
        static readonly double BZ_R19          = RES_K(33);
        static readonly double BZ_R20          = RES_K(33);
        static readonly double BZ_R21          = RES_K(33);
        static readonly double BZ_R25          = RES_K(100);
        static readonly double BZ_R26          = RES_K(33);
        static readonly double BZ_R27          = RES_K(330);
        static readonly double BZ_R28          = RES_K(100);
        static readonly double BZ_R29          = RES_K(22);
        //#define BZ_R30          RES_K(10)
        //#define BZ_R31          RES_K(100)
        static readonly double BZ_R32          = RES_K(330);
        static readonly double BZ_R33          = RES_K(330);
        static readonly double BZ_R34          = RES_K(33);
        static readonly double BZ_R35          = RES_K(33);

        /* Parts List - Capacitors */
        static readonly double BZ_C9           = CAP_U(4.7);
        static readonly double BZ_C11          = CAP_U(0.015);
        static readonly double BZ_C13          = CAP_U(10);
        static readonly double BZ_C14          = CAP_U(10);
        static readonly double BZ_C20          = CAP_U(0.1);
        static readonly double BZ_C21          = CAP_U(0.0047);
        static readonly double BZ_C22          = CAP_U(0.0047);
        static readonly double BZ_C29          = CAP_U(0.47);

        /*************************************
         *
         *  Discrete Sound static structs
         *
         *************************************/


        static readonly discrete_lfsr_desc bzone_lfsr = new discrete_lfsr_desc
        (
            DISC_CLK_IS_FREQ,
            16,                     /* Bit Length */
            0,                      /* Reset Value */
            3,                      /* Use Bit 10 (QC of second LS164) as F0 input 0 */
            14,                     /* Use Bit 23 (QH of third LS164) as F0 input 1 */
            DISC_LFSR_XOR,          /* F0 is XOR */
            DISC_LFSR_NOT_IN0,      /* F1 is inverted F0*/
            DISC_LFSR_REPLACE,      /* F2 replaces the shifted register contents */
            0x000001,               /* Everything is shifted into the first bit only */
            DISC_LFSR_FLAG_OUTPUT_SR_SN1, /* output the complete shift register to sub node 1*/
            15                  /* Output bit */
        );


        //static const discrete_op_amp_filt_info bzone_explo_0 =

        //static const discrete_op_amp_filt_info bzone_explo_1 =

        //static const discrete_op_amp_filt_info bzone_shell_0 =

        //static const discrete_op_amp_filt_info bzone_shell_1 =


        static readonly discrete_555_desc bzone_vco_desc = new discrete_555_desc
        (
            DISC_555_OUT_DC,
            5.0,
            DEFAULT_555_CHARGE,
            1.0 // Logic output
        );


        static readonly discrete_mixer_desc bzone_eng_mixer_desc = new discrete_mixer_desc
        (
            DISC_MIXER_IS_RESISTOR,
            new double [] { BZ_R20, BZ_R21, BZ_R34, BZ_R35 },
            new int [] { 0, 0, 0, 0 },
            new double [] { 0, 0, 0, 0 },
            0, 0,
            BZ_C29,
            0, /* no out cap */
            0, TTL_OUT      /* inputs are logic */
        );


        static readonly discrete_mixer_desc bzone_final_mixer_desc = new discrete_mixer_desc
        (
            DISC_MIXER_IS_RESISTOR,
            new double [] { BZ_R25, BZ_R28, BZ_R26 + BZ_R20 / 4, BZ_R27 },
            new int [] { 0, 0, 0, 0 },
            new double [] { 0, 0, 0, 0 },
            0, BZ_R29,
            0,
            BZ_C20, /* The speakers are driven by a +/- signal, just using the cap is good enough */
            0, 1
        );


        /************************************************************************
         *
         * Custom Battlezone filter
         *
         *         .------.         r2           c
         *         |     O|-----+--ZZZZ--+-------||---------.
         *         | 4066 |     |        |                  |
         *  IN0 >--|c    I|-.   Z r1     |       r5         |
         *         '------' |   Z        +------ZZZZ--------+
         *                  |   Z        |                  |
         *                 gnd  |        |           |\     |
         *                     gnd       |           | \    |
         *                               '-----------|- \   |
         *            r3                             |   >--+----> Netlist Node
         *  IN1 >----ZZZZ----------------+-----------|+ /
         *                               |           | /
         *                               Z r4        |/
         *                               Z
         *                               Z
         *                               |           VP = B+
         *                              gnd
         *
         ************************************************************************/

        const double CD4066_R_ON = 270;


        //DISCRETE_CLASS_STEP_RESET(bzone_custom_filter, 1,
        class discrete_bzone_custom_filter : discrete_base_node,
                                             discrete_step_interface
        {
            const int _maxout = 1;


            double BZONE_CUSTOM_FILTER__IN0 { get { return DISCRETE_INPUT(0); } }
            double BZONE_CUSTOM_FILTER__IN1 { get { return DISCRETE_INPUT(1); } }
            double BZONE_CUSTOM_FILTER__R1 { get { return DISCRETE_INPUT(2); } }
            double BZONE_CUSTOM_FILTER__R2 { get { return DISCRETE_INPUT(3); } }
            double BZONE_CUSTOM_FILTER__R3 { get { return DISCRETE_INPUT(4); } }
            double BZONE_CUSTOM_FILTER__R4 { get { return DISCRETE_INPUT(5); } }
            double BZONE_CUSTOM_FILTER__R5 { get { return DISCRETE_INPUT(6); } }
            double BZONE_CUSTOM_FILTER__C { get { return DISCRETE_INPUT(7); } }
            double BZONE_CUSTOM_FILTER__VP { get { return DISCRETE_INPUT(8); } }


            double m_v_in1_gain = 0;
            double m_v_p = 0;
            double m_exponent = 0;
            double [] m_gain = new double [2];
            double m_out_v = 0;


            //DISCRETE_CLASS_CONSTRUCTOR(_name, base)                             \
            public discrete_bzone_custom_filter() : base() { }

            //DISCRETE_CLASS_DESTRUCTOR(_name)                                    \
            //~discrete_bzone_custom_filter() { }


            protected override int max_output() { return _maxout; }


            // discrete_step_interface

            public osd_ticks_t run_time { get; set; }
            public discrete_base_node self { get; set; }


            //DISCRETE_STEP(bzone_custom_filter)
            public void step()
            {
                int     in0 = (BZONE_CUSTOM_FILTER__IN0 == 0) ? 0 : 1;
                double  v = 0;

                if (BZONE_CUSTOM_FILTER__IN1 > 0)
                    v = 0;

                v = BZONE_CUSTOM_FILTER__IN1 * m_v_in1_gain * m_gain[in0];
                if (v > m_v_p) v = m_v_p;
                if (v < 0) v = 0;

                m_out_v += (v - m_out_v) * m_exponent;
                set_output(0, m_out_v);
            }

            //DISCRETE_RESET(bzone_custom_filter)
            public override void reset()
            {
                m_gain[0] = BZONE_CUSTOM_FILTER__R1 + BZONE_CUSTOM_FILTER__R2;
                m_gain[0] = BZONE_CUSTOM_FILTER__R5 / m_gain[0] + 1;
                m_gain[1] = RES_2_PARALLEL(CD4066_R_ON, BZONE_CUSTOM_FILTER__R1) + BZONE_CUSTOM_FILTER__R2;
                m_gain[1] = BZONE_CUSTOM_FILTER__R5 / m_gain[1] + 1;
                m_v_in1_gain = RES_VOLTAGE_DIVIDER(BZONE_CUSTOM_FILTER__R3, BZONE_CUSTOM_FILTER__R4);
                m_v_p = BZONE_CUSTOM_FILTER__VP - OP_AMP_VP_RAIL_OFFSET;
                m_exponent = RC_CHARGE_EXP(BZONE_CUSTOM_FILTER__R5 * BZONE_CUSTOM_FILTER__C);
                m_out_v = 0.0;
            }
        }


        /*************************************
         *
         *  Discrete Sound Blocks
         *
         *************************************/
        //static DISCRETE_SOUND_START(bzone_discrete)
        static readonly discrete_block [] bzone_discrete = 
        {
            /************************************************/
            /* Input register mapping for Battlezone        */
            /************************************************/
            DISCRETE_INPUT_DATA(BZ_INPUT),
            /* decode the bits */
            DISCRETE_BITS_DECODE(NODE_10, BZ_INPUT, 0, 7, 1),             /* IC M2, bits 0 - 7 */

            /* the pot is 250K, but we will use a smaller range to get a better adjustment range */
            DISCRETE_ADJUSTMENT(BZ_R11_POT, RES_K(75), RES_K(10), DISC_LINADJ, "R11"),


            /************************************************/
            /* NOISE                                        */
            /************************************************/

            /* 12Khz clock is divided by two by B4 74LS109 */
            DISCRETE_LFSR_NOISE(BZ_NOISE,                               /* IC H4, pin 13 */
                1, 1, BZ_NOISE_CLOCK / 2, 1.0, 0, 0.5, bzone_lfsr),

            /* divide by 2 */
            DISCRETE_COUNTER(NODE_31,                                   /* IC J5, pin 8 */
                1, 0, BZ_NOISE, 0, 1, DISC_COUNT_UP, 0, DISC_CLK_ON_R_EDGE),

            DISCRETE_BITS_DECODE(NODE_32, NODE_SUB(BZ_NOISE, 1), 11, 14, 1),     /* IC H4, pins 6, 10, 11, 12 */
            DISCRETE_LOGIC_NAND4(NODE_33,                               /* IC J4, pin 8 */
                NODE_32_00, NODE_32_01, NODE_32_02, NODE_32_03),         /* LSFR bits 11-14 */
            /* divide by 2 */
            DISCRETE_COUNTER(NODE_34,                                   /* IC J5, pin 6 */
                1, 0, NODE_33, 0, 1, DISC_COUNT_UP, 0, DISC_CLK_ON_R_EDGE),

            /************************************************/
            /* Shell                                        */
            /************************************************/
            DISCRETE_RC_CIRCUIT_1(NODE_40,                  /* IC J3, pin 9 */
                BZ_INP_SHELL, NODE_31,                      /* INP0, INP1 */
                BZ_R14 + BZ_R15, BZ_C9),
            DISCRETE_CUSTOM9<discrete_bzone_custom_filter>(BZ_SHELL_SND,                 /* IC K5, pin 1 */
                BZ_INP_EXPLOLS, NODE_40,                    /* IN0, IN1 */
                BZ_R12, BZ_R13, BZ_R14, BZ_R15, BZ_R32,
                BZ_C21,
                22,                                         /* B+ of op-amp */
                null),

            /************************************************/
            /* Explosion                                    */
            /************************************************/

            DISCRETE_RC_CIRCUIT_1(NODE_50,                  /* IC J3, pin 3 */
                BZ_INP_EXPLO, NODE_34,                      /* INP0, INP1 */
                BZ_R17 + BZ_R16, BZ_C14),
            DISCRETE_CUSTOM9<discrete_bzone_custom_filter>(BZ_EXPLOSION_SND,              /* IC K5, pin 1 */
                BZ_INP_EXPLOLS, NODE_50,                    /* IN0, IN1 */
                BZ_R19, BZ_R18, BZ_R17, BZ_R16, BZ_R33,
                BZ_C22,
                22,                                         /* B+ of op-amp */
                null),
            /************************************************/
            /* Engine                                       */
            /************************************************/

            DISCRETE_SWITCH(NODE_61,                                /* effect of IC L4, pin 2 */
                1, BZ_INP_ENGREV,                                   /* ENAB, SWITCH */
                5.0 * RES_VOLTAGE_DIVIDER(BZ_R7, BZ_R6),            /* INP0 */
                5.0 * RES_VOLTAGE_DIVIDER(BZ_R7, RES_2_PARALLEL(CD4066_R_ON + BZ_R5, BZ_R6))),   /* INP1 */
            /* R5, R6, R7 all affect the following circuit charge discharge rates */
            /* they are not emulated as their effect is less than the 5% component tolerance */
            DISCRETE_RCDISC3(NODE_62,                               /* IC K5, pin 7 */
                1, NODE_61, BZ_R8, BZ_R9, BZ_C13, -0.5),

            DISCRETE_555_ASTABLE_CV(NODE_63,                        /* IC F3, pin 3 */
                1,                                                  /* RESET */
                BZ_R10, BZ_R11_POT, BZ_C11,
                NODE_62,                                            /* CV - IC F3, pin 5 */
                bzone_vco_desc),

            DISCRETE_LOGIC_INVERT(NODE_64, BZ_INP_MOTEN),
            DISCRETE_COUNTER(NODE_65,                               /* IC F4 */
                1, NODE_64, NODE_63,                                /* ENAB, RESET, CLK */
                4, 15, DISC_COUNT_UP, 0, DISC_CLK_ON_R_EDGE),        /* MIN, MAX, DIR, INIT, CLKTYPE */
            DISCRETE_TRANSFORM2(NODE_66, NODE_65, 7, "01>"),         /* QD - IC F4, pin 11 */
            DISCRETE_TRANSFORM2(NODE_67, NODE_65, 15, "01="),        /* Ripple - IC F4, pin 15 */

            DISCRETE_COUNTER(NODE_68,                               /* IC F5 */
                1, NODE_64, NODE_63,                                /* ENAB, RESET, CLK */
                6, 15, DISC_COUNT_UP, 0, DISC_CLK_ON_R_EDGE),        /* MIN, MAX, DIR, INIT, CLKTYPE */
            DISCRETE_TRANSFORM2(NODE_69, NODE_68, 7, "01>"),         /* QD - IC F5, pin 11 */
            DISCRETE_TRANSFORM2(NODE_70, NODE_68, 15, "01="),        /* Ripple - IC F5, pin 15 */

            DISCRETE_MIXER4(BZ_ENGINE_SND, 1, NODE_66, NODE_67, NODE_69, NODE_70, bzone_eng_mixer_desc),

            /************************************************/
            /* FINAL MIX                                    */
            /************************************************/
            /* We won't bother emulating the final gain of op-amp IC K5, pin 14.
             * This signal never reaches a value where it clips, so we will
             * just output the final 16-bit level.
             */

            /* Convert Pokey output to 5V Signal */
            DISCRETE_INPUTX_STREAM(BZ_POKEY_SND, 0, 5.0 / 32768, 0),

            DISCRETE_MIXER4(NODE_280,
                BZ_INP_SOUNDEN,
                BZ_SHELL_SND, BZ_EXPLOSION_SND, BZ_ENGINE_SND, BZ_POKEY_SND,
                bzone_final_mixer_desc),
            DISCRETE_OUTPUT(NODE_280, 48000),

            DISCRETE_SOUND_END,
        };


        void bzone_sounds_w(uint8_t data)
        {
            m_discrete.op0.write(BZ_INPUT, data);

            m_startled.op.op = BIT(data, 6);
            machine().sound().system_mute(BIT(data, 5) == 0);
        }


        void bzone_audio(machine_config config)
        {
            SPEAKER(config, "mono").front_center();

            pokey_device pokey = POKEY(config, "pokey", BZONE_MASTER_CLOCK / 8);
            pokey.allpot_r().set_ioport("IN3").reg();
            pokey.set_output_rc(RES_K(10), CAP_U(0.015), 5.0);
            pokey.disound.add_route(0, "discrete", 1.0, 0);

            DISCRETE(config, "discrete", bzone_discrete).disound.add_route(ALL_OUTPUTS, "mono", 1.0);
        }
    }
}
