// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;


namespace mame
{
    public static class galaga_audio_global
    {
        /* nodes - sounds */
        const NODE GALAGA_CHANL1_SND = NODE.NODE_11;
        const NODE GALAGA_CHANL2_SND = NODE.NODE_12;
        const NODE GALAGA_CHANL3_SND = NODE.NODE_13;


        /*************************************
         *
         *  Galaga/Xevious
         *
         *  Discrete sound emulation: Feb 2007, D.R.
         *
         *************************************/

        static double GALAGA_54XX_DAC_R() { return (1.0 / (1.0 / rescap_global.RES_K(47) + 1.0 / rescap_global.RES_K(22) + 1.0 / rescap_global.RES_K(10) + 1.0 / rescap_global.RES_K(4.7))); }


        static readonly discrete_dac_r1_ladder galaga_54xx_dac = new discrete_dac_r1_ladder
        (
            4,              /* number of DAC bits */
            new double [] { rescap_global.RES_K(47),
                rescap_global.RES_K(22),
                rescap_global.RES_K(10),
                rescap_global.RES_K(4.7)},
            0, 0, 0, 0      /* nothing extra */
        );


        static double GALAGA_VREF() { return (5.0 * (rescap_global.RES_K(2.2) / (rescap_global.RES_K(3.3) + rescap_global.RES_K(2.2)))); }


        static readonly discrete_op_amp_filt_info galaga_chanl1_filt = new discrete_op_amp_filt_info
        (
            GALAGA_54XX_DAC_R() + rescap_global.RES_K(100),
            0,                  /* no second input */
            rescap_global.RES_K(22),
            0,                  /* not used */
            rescap_global.RES_K(220),
            rescap_global.CAP_U(0.001),
            rescap_global.CAP_U(0.001),
            0,                  /* not used */
            GALAGA_VREF(),        /* vRef */
            5,                  /* vP */
            0                   /* vN */
        );


        static readonly discrete_op_amp_filt_info galaga_chanl2_filt = new discrete_op_amp_filt_info
        (
            GALAGA_54XX_DAC_R() + rescap_global.RES_K(47),
            0,                  /* no second input */
            rescap_global.RES_K(10),
            0,                  /* not used */
            rescap_global.RES_K(150),
            rescap_global.CAP_U(0.01),
            rescap_global.CAP_U(0.01),
            0,                  /* not used */
            GALAGA_VREF(),      /* vRef */
            5,                  /* vP */
            0                   /* vN */
        );

        static readonly discrete_op_amp_filt_info galaga_chanl3_filt = new discrete_op_amp_filt_info
        (
            GALAGA_54XX_DAC_R() + rescap_global.RES_K(150),
            0,                  /* no second input */
            rescap_global.RES_K(22),
            0,                  /* not used */
            rescap_global.RES_K(470),
            rescap_global.CAP_U(0.01),
            rescap_global.CAP_U(0.01),
            0,                  /* not used */
            GALAGA_VREF(),      /* vRef */
            5,                  /* vP */
            0                   /* vN */
        );

        static readonly discrete_mixer_desc galaga_final_mixer = new discrete_mixer_desc
        (
            discrete_global.DISC_MIXER_IS_OP_AMP,
            new double [] { rescap_global.RES_K(33),
                rescap_global.RES_K(33),
                rescap_global.RES_K(10) },
            new int [] {0},            /* no rNode{} */
            new double [] {0},            /* no c{} */
            0,              /* no rI */
            rescap_global.RES_K(3.3),
            0,              /* no cF */
            rescap_global.CAP_U(0.1),
            GALAGA_VREF(),    /* vRef */
            40800          /* gain */
        );


        //DISCRETE_SOUND_START(galaga_discrete)
        public static readonly discrete_block [] galaga_discrete = new discrete_block []
        {

            /************************************************
             * Input register mapping
             ************************************************/
            discrete_global.DISCRETE_INPUT_DATA(namco54_global.NAMCO_54XX_0_DATA(NODE.NODE_01)),
            discrete_global.DISCRETE_INPUT_DATA(namco54_global.NAMCO_54XX_1_DATA(NODE.NODE_01)),
            discrete_global.DISCRETE_INPUT_DATA(namco54_global.NAMCO_54XX_2_DATA(NODE.NODE_01)),

            /************************************************
             * CHANL1 sound
             ************************************************/
            discrete_global.DISCRETE_DAC_R1(NODE.NODE_20,
                            namco54_global.NAMCO_54XX_2_DATA(NODE.NODE_01),
                            4,          /* 4V - unmeasured*/
                            galaga_54xx_dac),
            discrete_global.DISCRETE_OP_AMP_FILTER(GALAGA_CHANL1_SND,
                            1,          /* ENAB */
                            NODE.NODE_20,    /* INP0 */
                            0,          /* INP1 - not used */
                            discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, galaga_chanl1_filt),

            /************************************************
             * CHANL2 sound
             ************************************************/
            discrete_global.DISCRETE_DAC_R1(NODE.NODE_30,
                            namco54_global.NAMCO_54XX_1_DATA(NODE.NODE_01),
                            4,          /* 4V - unmeasured*/
                            galaga_54xx_dac),
            discrete_global.DISCRETE_OP_AMP_FILTER(GALAGA_CHANL2_SND,
                            1,          /* ENAB */
                            NODE.NODE_30,    /* INP0 */
                            0,          /* INP1 - not used */
                            discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, galaga_chanl2_filt),

            /************************************************
             * CHANL3 sound
             ************************************************/
            discrete_global.DISCRETE_DAC_R1(NODE.NODE_40,
                            namco54_global.NAMCO_54XX_0_DATA(NODE.NODE_01),
                            4,          /* 4V - unmeasured*/
                            galaga_54xx_dac),
            discrete_global.DISCRETE_OP_AMP_FILTER(GALAGA_CHANL3_SND,
                            1,          /* ENAB */
                            NODE.NODE_40,    /* INP0 */
                            0,          /* INP1 - not used */
                            discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, galaga_chanl3_filt),

            /************************************************
             * Output
             ************************************************/
            discrete_global.DISCRETE_MIXER3(NODE.NODE_90,
                            1,                  /* ENAB */
                            GALAGA_CHANL1_SND,  /* IN0 */
                            GALAGA_CHANL2_SND,  /* IN1 */
                            GALAGA_CHANL3_SND,  /* IN2 */
                            galaga_final_mixer /* INFO */),
            discrete_global.DISCRETE_OUTPUT(NODE.NODE_90, 1),

            discrete_global.DISCRETE_SOUND_END(),
        };
    }
}
