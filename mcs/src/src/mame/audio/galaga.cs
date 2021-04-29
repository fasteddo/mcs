// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    partial class galaga_state : driver_device
    {
        /* nodes - sounds */
        const int GALAGA_CHANL1_SND = g.NODE_11;
        const int GALAGA_CHANL2_SND = g.NODE_12;
        const int GALAGA_CHANL3_SND = g.NODE_13;


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
            g.DISC_MIXER_IS_OP_AMP,
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
            g.DISCRETE_INPUT_DATA(NAMCO_54XX_0_DATA(g.NODE_01)),
            g.DISCRETE_INPUT_DATA(NAMCO_54XX_1_DATA(g.NODE_01)),
            g.DISCRETE_INPUT_DATA(NAMCO_54XX_2_DATA(g.NODE_01)),

            /************************************************
             * CHANL1 sound
             ************************************************/
            g.DISCRETE_DAC_R1(g.NODE_20,
                            NAMCO_54XX_2_DATA(g.NODE_01),
                            4,          /* 4V - unmeasured*/
                            galaga_54xx_dac),
            g.DISCRETE_OP_AMP_FILTER(GALAGA_CHANL1_SND,
                            1,          /* ENAB */
                            g.NODE_20,    /* INP0 */
                            0,          /* INP1 - not used */
                            g.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, galaga_chanl1_filt),

            /************************************************
             * CHANL2 sound
             ************************************************/
            g.DISCRETE_DAC_R1(g.NODE_30,
                            NAMCO_54XX_1_DATA(g.NODE_01),
                            4,          /* 4V - unmeasured*/
                            galaga_54xx_dac),
            g.DISCRETE_OP_AMP_FILTER(GALAGA_CHANL2_SND,
                            1,          /* ENAB */
                            g.NODE_30,    /* INP0 */
                            0,          /* INP1 - not used */
                            g.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, galaga_chanl2_filt),

            /************************************************
             * CHANL3 sound
             ************************************************/
            g.DISCRETE_DAC_R1(g.NODE_40,
                            NAMCO_54XX_0_DATA(g.NODE_01),
                            4,          /* 4V - unmeasured*/
                            galaga_54xx_dac),
            g.DISCRETE_OP_AMP_FILTER(GALAGA_CHANL3_SND,
                            1,          /* ENAB */
                            g.NODE_40,    /* INP0 */
                            0,          /* INP1 - not used */
                            g.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, galaga_chanl3_filt),

            /************************************************
             * Output
             ************************************************/
            g.DISCRETE_MIXER3(g.NODE_90,
                            1,                  /* ENAB */
                            GALAGA_CHANL1_SND,  /* IN0 */
                            GALAGA_CHANL2_SND,  /* IN1 */
                            GALAGA_CHANL3_SND,  /* IN2 */
                            galaga_final_mixer /* INFO */),
            g.DISCRETE_OUTPUT(g.NODE_90, 1),

            g.DISCRETE_SOUND_END,
        };
    }
}
