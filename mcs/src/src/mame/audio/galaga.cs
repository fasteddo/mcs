// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;


namespace mame
{
    partial class galaga_state : driver_device
    {
        /* nodes - sounds */
        const int GALAGA_CHANL1_SND = NODE_11;
        const int GALAGA_CHANL2_SND = NODE_12;
        const int GALAGA_CHANL3_SND = NODE_13;


        /*************************************
         *
         *  Galaga/Xevious
         *
         *  Discrete sound emulation: Feb 2007, D.R.
         *
         *************************************/

        static readonly double GALAGA_54XX_DAC_R = 1.0 / (1.0 / RES_K(47) + 1.0 / RES_K(22) + 1.0 / RES_K(10) + 1.0 / RES_K(4.7));


        static readonly discrete_dac_r1_ladder galaga_54xx_dac = new discrete_dac_r1_ladder
        (
            4,              /* number of DAC bits */
            new double [] { RES_K(47),
                RES_K(22),
                RES_K(10),
                RES_K(4.7) },
            0, 0, 0, 0      /* nothing extra */
        );


        static readonly double GALAGA_VREF = 5.0 * (RES_K(2.2) / (RES_K(3.3) + RES_K(2.2)));


        static readonly discrete_op_amp_filt_info galaga_chanl1_filt = new discrete_op_amp_filt_info
        (
            GALAGA_54XX_DAC_R + RES_K(100),
            0,                  /* no second input */
            RES_K(22),
            0,                  /* not used */
            RES_K(220),
            CAP_U(0.001),
            CAP_U(0.001),
            0,                  /* not used */
            GALAGA_VREF,        /* vRef */
            5,                  /* vP */
            0                   /* vN */
        );


        static readonly discrete_op_amp_filt_info galaga_chanl2_filt = new discrete_op_amp_filt_info
        (
            GALAGA_54XX_DAC_R + RES_K(47),
            0,                  /* no second input */
            RES_K(10),
            0,                  /* not used */
            RES_K(150),
            CAP_U(0.01),
            CAP_U(0.01),
            0,                  /* not used */
            GALAGA_VREF,      /* vRef */
            5,                  /* vP */
            0                   /* vN */
        );

        static readonly discrete_op_amp_filt_info galaga_chanl3_filt = new discrete_op_amp_filt_info
        (
            GALAGA_54XX_DAC_R + RES_K(150),
            0,                  /* no second input */
            RES_K(22),
            0,                  /* not used */
            RES_K(470),
            CAP_U(0.01),
            CAP_U(0.01),
            0,                  /* not used */
            GALAGA_VREF,      /* vRef */
            5,                  /* vP */
            0                   /* vN */
        );

        static readonly discrete_mixer_desc galaga_final_mixer = new discrete_mixer_desc
        (
            DISC_MIXER_IS_OP_AMP,
            new double [] { RES_K(33),
                RES_K(33),
                RES_K(10) },
            new int [] {0},            /* no rNode{} */
            new double [] {0},            /* no c{} */
            0,              /* no rI */
            RES_K(3.3),
            0,              /* no cF */
            CAP_U(0.1),
            GALAGA_VREF,    /* vRef */
            40800          /* gain */
        );


        //DISCRETE_SOUND_START(galaga_discrete)
        protected static readonly discrete_block [] galaga_discrete = new discrete_block []
        {
            /************************************************
             * Input register mapping
             ************************************************/
            DISCRETE_INPUT_DATA(NAMCO_54XX_0_DATA(NODE_01)),
            DISCRETE_INPUT_DATA(NAMCO_54XX_1_DATA(NODE_01)),
            DISCRETE_INPUT_DATA(NAMCO_54XX_2_DATA(NODE_01)),

            /************************************************
             * CHANL1 sound
             ************************************************/
            DISCRETE_DAC_R1(NODE_20,
                            NAMCO_54XX_2_DATA(NODE_01),
                            4,          /* 4V - unmeasured*/
                            galaga_54xx_dac),
            DISCRETE_OP_AMP_FILTER(GALAGA_CHANL1_SND,
                            1,          /* ENAB */
                            NODE_20,    /* INP0 */
                            0,          /* INP1 - not used */
                            DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, galaga_chanl1_filt),

            /************************************************
             * CHANL2 sound
             ************************************************/
            DISCRETE_DAC_R1(NODE_30,
                            NAMCO_54XX_1_DATA(NODE_01),
                            4,          /* 4V - unmeasured*/
                            galaga_54xx_dac),
            DISCRETE_OP_AMP_FILTER(GALAGA_CHANL2_SND,
                            1,          /* ENAB */
                            NODE_30,    /* INP0 */
                            0,          /* INP1 - not used */
                            DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, galaga_chanl2_filt),

            /************************************************
             * CHANL3 sound
             ************************************************/
            DISCRETE_DAC_R1(NODE_40,
                            NAMCO_54XX_0_DATA(NODE_01),
                            4,          /* 4V - unmeasured*/
                            galaga_54xx_dac),
            DISCRETE_OP_AMP_FILTER(GALAGA_CHANL3_SND,
                            1,          /* ENAB */
                            NODE_40,    /* INP0 */
                            0,          /* INP1 - not used */
                            DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, galaga_chanl3_filt),

            /************************************************
             * Output
             ************************************************/
            DISCRETE_MIXER3(NODE_90,
                            1,                  /* ENAB */
                            GALAGA_CHANL1_SND,  /* IN0 */
                            GALAGA_CHANL2_SND,  /* IN1 */
                            GALAGA_CHANL3_SND,  /* IN2 */
                            galaga_final_mixer /* INFO */),
            DISCRETE_OUTPUT(NODE_90, 1),

            DISCRETE_SOUND_END(),
        };
    }
}
