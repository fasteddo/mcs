// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using offs_t = System.UInt32;  //using offs_t = u32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;

using static mame.device_creator_helper_global;
using static mame.device_global;
using static mame.discrete_global;
using static mame.disound_global;
using static mame.emucore_global;
using static mame.rescap_global;
using static mame.util;


namespace mame
{
    //class midway_tone_generator_device_base : public device_t

    //class seawolf_audio_device : public device_t

    //class gunfight_audio_device : public device_t

    //class boothill_audio_device : public midway_tone_generator_device_base

    //class desertgu_audio_device : public midway_tone_generator_device_base

    //class dplay_audio_device : public midway_tone_generator_device_base

    //class gmissile_audio_device : public device_t

    //class m4_audio_device : public device_t

    //class clowns_audio_device : public midway_tone_generator_device_base

    //class spacwalk_audio_device : public midway_tone_generator_device_base

    //class dogpatch_audio_device : public midway_tone_generator_device_base

    //class spcenctr_audio_device : public device_t

    //class phantom2_audio_device : public device_t


    public class invaders_audio_device : device_t
    {
        //DEFINE_DEVICE_TYPE(INVADERS_AUDIO, invaders_audio_device, "invaders_audio", "Taito Space Invaders Audio")
        static device_t device_creator_invaders_audio_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new invaders_audio_device(mconfig, tag, owner, clock); }
        public static readonly device_type INVADERS_AUDIO = DEFINE_DEVICE_TYPE(device_creator_invaders_audio_device, "invaders_audio", "Taito Space Invaders Audio");


        static readonly discrete_lfsr_desc midway_lfsr = new discrete_lfsr_desc
        (
            DISC_CLK_IS_FREQ,
            17,                 // bit length
                                // the RC network fed into pin 4 has the effect of presetting all bits high at power up
            0x1ffff,            // reset value
            4,                  // use bit 4 as XOR input 0
            16,                 // use bit 16 as XOR input 1
            DISC_LFSR_XOR,      // feedback stage1 is XOR
            DISC_LFSR_OR,       // feedback stage2 is just stage 1 output OR with external feed
            DISC_LFSR_REPLACE,  // feedback stage3 replaces the shifted register contents
            0x000001,           // everything is shifted into the first bit only
            0,                  // output is not inverted
            12                  // output bit
        );


        // sound board 1 or 2, for multi-board games
        static int INVADERS_NODE(int _node, int _board) { return NODE_(_node + ((_board - 1) * 100)); }

        // nodes - inputs
        const int INVADERS_SAUCER_HIT_EN                = 01;
        const int INVADERS_FLEET_DATA                   = 02;
        const int INVADERS_BONUS_MISSLE_BASE_EN         = 03;
        const int INVADERS_INVADER_HIT_EN               = 04;
        const int INVADERS_EXPLOSION_EN                 = 05;
        const int INVADERS_MISSILE_EN                   = 06;


        // nodes - sounds
        const int INVADERS_NOISE                        = NODE_10;
        const int INVADERS_SAUCER_HIT_SND               = 11;
        const int INVADERS_FLEET_SND                    = 12;
        const int INVADERS_BONUS_MISSLE_BASE_SND        = 13;
        const int INVADERS_INVADER_HIT_SND              = 14;
        const int INVADERS_EXPLOSION_SND                = 15;
        const int INVADERS_MISSILE_SND                  = 16;

        static readonly discrete_block INVADERS_NOISE_GENERATOR =
            DISCRETE_LFSR_NOISE(INVADERS_NOISE,             /* IC N5, pin 10 */
                1,                                      /* ENAB */
                1,                                      /* no RESET */
                7515,                                   /* CLK in Hz */
                12,                                     /* p-p AMPL */
                0,                                      /* no FEED input */
                12.0/2,                                 /* dc BIAS */
                midway_lfsr);


        /************************************************
         * Saucer Hit
         ************************************************/
        static readonly discrete_op_amp_info invaders_saucer_hit_op_amp_B3_9 = new discrete_op_amp_info
        (
            DISC_OP_AMP_IS_NORTON,
            0,              // no r1
            RES_K(100),     // R72
            RES_M(1),       // R71
            0,              // no r4
            CAP_U(1),       // C23
            0,              // vN
            12              // vP
        );


        static readonly discrete_op_amp_osc_info invaders_saucer_hit_osc = new discrete_op_amp_osc_info
        (
            DISC_OP_AMP_OSCILLATOR_1 | DISC_OP_AMP_IS_NORTON | DISC_OP_AMP_OSCILLATOR_OUT_CAP,
            RES_M(1),       // R70
            RES_K(470),     // R64
            RES_K(100),     // R61
            RES_K(120),     // R63
            RES_M(1),       // R62
            0,              // no r6
            0,              // no r7
            0,              // no r8
            CAP_U(0.1),     // C21
            12             // vP
        );


        static readonly discrete_op_amp_osc_info invaders_saucer_hit_vco = new discrete_op_amp_osc_info
        (
            DISC_OP_AMP_OSCILLATOR_VCO_1 | DISC_OP_AMP_IS_NORTON | DISC_OP_AMP_OSCILLATOR_OUT_SQW,
            RES_M(1),       // R65
            RES_K(470),     // R66
            RES_K(680),     // R67
            RES_M(1),       // R69
            RES_M(1),       // R68
            0,              // no r6
            0,              // no r7
            0,              // no r8
            CAP_P(470),     // C22
            12             // vP
        );


        static readonly discrete_op_amp_info invaders_saucer_hit_op_amp_B3_10 = new discrete_op_amp_info
        (
            DISC_OP_AMP_IS_NORTON,
            RES_K(680),     // R73
            RES_K(680),     // R77
            RES_M(2.7),     // R74
            RES_K(680),     // R75
            0,              // no c
            0,              // vN
            12              // vP
        );


        static discrete_block INVADERS_SAUCER_HIT_1(int _board)
        {
            return DISCRETE_INPUTX_LOGIC(INVADERS_NODE(INVADERS_SAUCER_HIT_EN, _board), 12, 0, 0);
        }

        static discrete_block INVADERS_SAUCER_HIT_2(int _board)
        {
            return DISCRETE_OP_AMP(INVADERS_NODE(20, _board),                      /* IC B3, pin 9 */
                1,                                                      /* ENAB */
                0,                                                      /* no IN0 */
                INVADERS_NODE(INVADERS_SAUCER_HIT_EN, _board),          /* IN1 */
                invaders_saucer_hit_op_amp_B3_9);
        }

        static discrete_block INVADERS_SAUCER_HIT_3(int _board)
        {
            return DISCRETE_OP_AMP_OSCILLATOR(INVADERS_NODE(21, _board),           /* IC A4, pin 5 */
                1,                                                      /* ENAB */
                invaders_saucer_hit_osc);
        }

        static discrete_block INVADERS_SAUCER_HIT_4(int _board)
        {
            return DISCRETE_OP_AMP_VCO1(INVADERS_NODE(22, _board),                 /* IC A4, pin 9 */
                1,                                                      /* ENAB */
                INVADERS_NODE(21, _board),                              /* VMOD1 */
                invaders_saucer_hit_vco);
        }

        static discrete_block INVADERS_SAUCER_HIT_5(int _board)
        {
            return DISCRETE_OP_AMP(INVADERS_NODE(INVADERS_SAUCER_HIT_SND, _board), /* IC B3, pin 10 */
                1,                                                      /* ENAB */
                INVADERS_NODE(22, _board),                              /* IN0 */
                INVADERS_NODE(20, _board),                              /* IN1 */
                invaders_saucer_hit_op_amp_B3_10);
        }


        /************************************************
         * Fleet movement
         ************************************************/
        static readonly discrete_comp_adder_table invaders_thump_resistors = new discrete_comp_adder_table
        (
            DISC_COMP_P_RESISTOR,
            0,                          // no cDefault
            4,                          // length
            new double [] { RES_K(20) + RES_K(20),    // R126 + R127
                RES_K(68),              // R128
                RES_K(82),              // R129
                RES_K(100) }            // R130
        );

        static readonly discrete_555_desc invaders_thump_555 = new discrete_555_desc
        (
            DISC_555_OUT_ENERGY | DISC_555_OUT_DC,
            5,
            5.0 - 0.6,                  // 5V - diode drop
            DEFAULT_TTL_V_LOGIC_1       // Output of F3 7411 buffer
        );


        static discrete_block INVADERS_FLEET_1(int _board)
        {
            return DISCRETE_INPUT_DATA(INVADERS_NODE(INVADERS_FLEET_DATA, _board));
        }

        static discrete_block INVADERS_FLEET_2(int _board)
        {
            return DISCRETE_COMP_ADDER(INVADERS_NODE(30, _board),
                INVADERS_NODE(INVADERS_FLEET_DATA, _board),             /* DATA */
                invaders_thump_resistors);
        }

        static discrete_block INVADERS_FLEET_3(int _board)
        {
            return DISCRETE_555_ASTABLE(INVADERS_NODE(31, _board),                 /* IC F3, pin 6 */
                1,                                                      /* RESET */
                INVADERS_NODE(30, _board),                              /* R1 */
                RES_K(75),                                              /* R131 */
                CAP_U(0.1),                                             /* C29 */
                invaders_thump_555);
        }

        static discrete_block INVADERS_FLEET_4(int _board)
        {
            return DISCRETE_RCFILTER(INVADERS_NODE(32, _board),
                INVADERS_NODE(31, _board),                              /* IN0 */
                100,                                                    /* R132 */
                CAP_U(4.7) );                                            /* C31 */
        }

        static discrete_block INVADERS_FLEET_5(int _board)
        {
            return DISCRETE_RCFILTER(INVADERS_NODE(INVADERS_FLEET_SND, _board),
                INVADERS_NODE(32, _board),                              /* IN0 */
                100 + 100,                                              /* R132 + R133 */
                CAP_U(10) );                                             /* C32 */
        }


        static readonly discrete_555_desc invaders_bonus_555 = new discrete_555_desc
        (
            DISC_555_OUT_SQW | DISC_555_OUT_DC,
            5.0,                        // 5V
            DEFAULT_555_VALUES_1, DEFAULT_555_VALUES_2
        );


        static discrete_block INVADERS_BONUS_MISSLE_BASE_1(int _board)
        {
            return DISCRETE_INPUT_LOGIC(INVADERS_NODE(INVADERS_BONUS_MISSLE_BASE_EN, _board));
        }

        static discrete_block INVADERS_BONUS_MISSLE_BASE_2(int _board)
        {
            return DISCRETE_555_ASTABLE(INVADERS_NODE(40, _board),                 /* IC F4, pin 9 */
                INVADERS_NODE(INVADERS_BONUS_MISSLE_BASE_EN, _board),   /* RESET */
                RES_K(100),                                             /* R94 */
                RES_K(47),                                              /* R95 */
                CAP_U(1),                                               /* C34 */
                invaders_bonus_555);
        }

        static discrete_block INVADERS_BONUS_MISSLE_BASE_3(int _board)
        {
            return DISCRETE_SQUAREWFIX(INVADERS_NODE(41, _board),
                1,                                                      /* ENAB */
                480,                                                    /* FREQ */
                1,                                                      /* AMP */
                50,                                                     /* DUTY */
                1.0/2,                                                  /* BIAS */
                0);                                                     /* PHASE */
        }

        static discrete_block INVADERS_BONUS_MISSLE_BASE_4(int _board)
        {
            return DISCRETE_LOGIC_AND3(INVADERS_NODE(42, _board),                  /* IC F3, pin 12 */
                INVADERS_NODE(INVADERS_BONUS_MISSLE_BASE_EN, _board),    /* INP0 */
                INVADERS_NODE(41, _board),                              /* INP1 */
                INVADERS_NODE(40, _board) );                            /* INP2 */
        }

        static discrete_block INVADERS_BONUS_MISSLE_BASE_5(int _board)
        {
            return DISCRETE_GAIN(INVADERS_NODE(INVADERS_BONUS_MISSLE_BASE_SND, _board),/* adjust from logic to TTL voltage level */
                INVADERS_NODE(42, _board),                              /* IN0 */
                DEFAULT_TTL_V_LOGIC_1);                                 /* GAIN */
        }


        static readonly discrete_op_amp_info invaders_invader_hit_op_amp_D3_10 = new discrete_op_amp_info
        (
            DISC_OP_AMP_IS_NORTON,
            0,                          // no r1
            RES_K(10),                  // R53
            RES_M(1),                   // R137
            0,                          // no r4
            CAP_U(0.47),                // C19
            0,                          // vN
            12                          // vP
        );

        static readonly discrete_op_amp_osc_info invaders_invader_hit_vco = new discrete_op_amp_osc_info
        (
            DISC_OP_AMP_OSCILLATOR_VCO_1 | DISC_OP_AMP_IS_NORTON | DISC_OP_AMP_OSCILLATOR_OUT_CAP,
            RES_M(1),                   // R42
            RES_K(470),                 // R43
            RES_K(680),                 // R44
            RES_M(1),                   // R46
            RES_M(1),                   // R45
            0,                          // no r6
            0,                          // no r7
            0,                          // no r8
            CAP_P(330),                 // C16
            12                         // vP
        );

        static readonly discrete_op_amp_info invaders_invader_hit_op_amp_D3_4 = new discrete_op_amp_info
        (
            DISC_OP_AMP_IS_NORTON,
            RES_K(470),                 // R55
            RES_K(680),                 // R54
            RES_M(2.7),                 // R56
            RES_K(680),                 // R57
            0,                          // no c
            0,                          // vN
            12                          // vP
        );


        static discrete_block INVADERS_INVADER_HIT_1(int _board, string _type)
        {
            return DISCRETE_INPUTX_LOGIC(INVADERS_NODE(INVADERS_INVADER_HIT_EN, _board), 5, 0, 0);
        }

        static discrete_block INVADERS_INVADER_HIT_2(int _board, string _type)
        {
            return DISCRETE_OP_AMP_ONESHOT(INVADERS_NODE(50, _board),              /* IC D3, pin 9 */
                INVADERS_NODE(INVADERS_INVADER_HIT_EN, _board),         /* TRIG */
                _type == "invaders" ? invaders_invader_hit_1sht : throw new emu_unimplemented());  //&_type##_invader_hit_1sht)
        }

        static discrete_block INVADERS_INVADER_HIT_3(int _board, string _type)
        {
            return DISCRETE_OP_AMP(INVADERS_NODE(51, _board),                      /* IC D3, pin 10 */
                1,                                                      /* ENAB */
                0,                                                      /* no IN0 */
                INVADERS_NODE(50, _board),                              /* IN1 */
                invaders_invader_hit_op_amp_D3_10);
        }

        static discrete_block INVADERS_INVADER_HIT_4(int _board, string _type)
        {
            return DISCRETE_OP_AMP_OSCILLATOR(INVADERS_NODE(52, _board),           /* IC B4, pin 5 */
                1,                                                      /* ENAB */
                _type == "invaders" ? invaders_invader_hit_osc : throw new emu_unimplemented());  //&_type##_invader_hit_osc)
        }

        static discrete_block INVADERS_INVADER_HIT_5(int _board, string _type)
        {
            return DISCRETE_OP_AMP_VCO1(INVADERS_NODE(53, _board),                 /* IC B4, pin 4 */
                1,                                                      /* ENAB */
                INVADERS_NODE(52, _board),                              /* VMOD1 */
                invaders_invader_hit_vco);
        }

        static discrete_block INVADERS_INVADER_HIT_6(int _board, string _type)
        {
            return DISCRETE_OP_AMP(INVADERS_NODE(INVADERS_INVADER_HIT_SND, _board),/* IC D3, pin 4 */
                1,                                                      /* ENAB */
                INVADERS_NODE(53, _board),                              /* IN0 */
                INVADERS_NODE(51, _board),                              /* IN1 */
                invaders_invader_hit_op_amp_D3_4);
        }


        static readonly discrete_op_amp_1sht_info invaders_missle_1sht = new discrete_op_amp_1sht_info
        (
            DISC_OP_AMP_1SHT_1 | DISC_OP_AMP_IS_NORTON,
            RES_M(4.7),                         // R32
            RES_K(100),                         // R30
            RES_M(1),                           // R31
            RES_M(1),                           // R33
            RES_M(2.2),                         // R34
            CAP_U(1),                           // C12, CAP_U(0.22) on Taito PCB
            CAP_P(470),                         // C15
            0,                                  // vN
            12                                  // vP
        );

        static readonly discrete_op_amp_info invaders_missle_op_amp_B3 = new discrete_op_amp_info
        (
            DISC_OP_AMP_IS_NORTON,
            0,                                  // no r1
            RES_K(10),                          // R35
            RES_M(1.5),                         // R36
            0,                                  // no r4
            CAP_U(0.22),                        // C13
            0,                                  // vN
            12                                  // vP
        );

        static readonly discrete_op_amp_osc_info invaders_missle_op_amp_osc = new discrete_op_amp_osc_info
        (
            DISC_OP_AMP_OSCILLATOR_VCO_3 | DISC_OP_AMP_IS_NORTON | DISC_OP_AMP_OSCILLATOR_OUT_SQW, // DISC_OP_AMP_OSCILLATOR_VCO_3 doesn't fully represent the actual circuit on a Taito PCB, missile sound is off
            1.0 / (1.0 / RES_M(1) + 1.0 / RES_K(330)) + RES_M(1.5),     // R29||R11 + R12
            RES_M(1),                           // R16
            RES_K(560),                         // R17
            RES_M(2.2),                         // R19
            RES_M(1),                           // R16
            RES_M(4.7),                         // R14
            RES_M(3.3),                         // R13, RES_M(2.2) on Taito PCB
            0,                                  // no r8
            CAP_P(330),                         // C58
            12                                 // vP
        );

        static readonly discrete_op_amp_info invaders_missle_op_amp_A3 = new discrete_op_amp_info
        (
            DISC_OP_AMP_IS_NORTON,
            RES_K(560),                         // R22
            RES_K(470),                         // R15
            RES_M(2.7),                         // R20
            RES_K(560),                         // R21
            0,                                  // no c
            0,                                  // vN
            12                                  // vP
        );

        static readonly discrete_op_amp_tvca_info invaders_missle_tvca = new discrete_op_amp_tvca_info
        (
            RES_M(2.7),                         // R25
            RES_K(560),                         // R23
            0,                                  // no r3
            RES_K(560),                         // R26
            RES_K(1),                           //
            0,                                  // no r6
            RES_K(560),                         // R60
            0,                                  // no r8
            0,                                  // no r9
            0,                                  // no r10
            0,                                  // no r11
            CAP_U(0.1),                         // C14
            0,                                  // no c2
            0, 0,                               // no c3, c4
            5,                                  // v1
            0,                                  // no v2
            0,                                  // no v3
            12,                                 // vP
            DISC_OP_AMP_TRIGGER_FUNCTION_NONE,  // no f0
            DISC_OP_AMP_TRIGGER_FUNCTION_NONE,  // no f1
            DISC_OP_AMP_TRIGGER_FUNCTION_TRG0,  // f2
            DISC_OP_AMP_TRIGGER_FUNCTION_NONE,  // no f3
            DISC_OP_AMP_TRIGGER_FUNCTION_NONE,  // no f4
            DISC_OP_AMP_TRIGGER_FUNCTION_NONE   // no f5
        );


        static discrete_block INVADERS_MISSILE_1(int _board, string _type)
        {
            return DISCRETE_INPUTX_LOGIC(INVADERS_NODE(INVADERS_MISSILE_EN, _board), 5, 0, 0);
        }

        static discrete_block INVADERS_MISSILE_2(int _board, string _type)
        {
            return DISCRETE_OP_AMP_ONESHOT(INVADERS_NODE(70, _board),              /* IC B3, pin 4 */
                INVADERS_NODE(INVADERS_MISSILE_EN, _board),             /* TRIG */
                _type == "invaders" ? invaders_missle_1sht : throw new emu_unimplemented());  //&_type##_missle_1sht)
        }

        static discrete_block INVADERS_MISSILE_3(int _board, string _type)
        {
            return DISCRETE_OP_AMP(INVADERS_NODE(71, _board),                      /* IC B3, pin 5 */
                1,                                                      /* ENAB */
                0,                                                      /* no IN0 */
                INVADERS_NODE(70, _board),                              /* IN1 */
                invaders_missle_op_amp_B3);
        }

        static discrete_block INVADERS_MISSILE_4(int _board, string _type)
        {
            /* next 2 modules simulate the D1 voltage drop */
            return DISCRETE_ADDER2(INVADERS_NODE(72, _board),
                1,                                                      /* ENAB */
                INVADERS_NODE(71, _board),                              /* IN0 */
                -0.5);                                                  /* IN1 */
        }

        static discrete_block INVADERS_MISSILE_5(int _board, string _type)
        {
            return DISCRETE_CLAMP(INVADERS_NODE(73, _board),
                INVADERS_NODE(72, _board),                              /* IN0 */
                0,                                                      /* MIN */
                12);                                                    /* MAX */
        }

        static discrete_block INVADERS_MISSILE_6(int _board, string _type)
        {
            return DISCRETE_CRFILTER(INVADERS_NODE(74, _board),
                INVADERS_NOISE,                                         /* IN0 */
                RES_M(1) + RES_K(330),                                  /* R29, R11 */
                CAP_U(0.1) );                                           /* C57 */
        }

        static discrete_block INVADERS_MISSILE_7(int _board, string _type)
        {
            return DISCRETE_GAIN(INVADERS_NODE(75, _board),
                INVADERS_NODE(74, _board),                              /* IN0 */
                RES_K(330) / (RES_M(1) + RES_K(330)));                    /* GAIN - R29 : R11 */
        }

        static discrete_block INVADERS_MISSILE_8(int _board, string _type)
        {
            return DISCRETE_OP_AMP_VCO2(INVADERS_NODE(76, _board),                 /* IC C1, pin 4 */
                1,                                                      /* ENAB */
                INVADERS_NODE(75, _board),                              /* VMOD1 */
                INVADERS_NODE(73, _board),                              /* VMOD2 */
                invaders_missle_op_amp_osc);
        }

        static discrete_block INVADERS_MISSILE_9(int _board, string _type)
        {
            return DISCRETE_OP_AMP(INVADERS_NODE(77, _board),                      /* IC A3, pin 9 */
                1,                                                      /* ENAB */
                INVADERS_NODE(76, _board),                              /* IN0 */
                INVADERS_NODE(73, _board),                              /* IN1 */
                invaders_missle_op_amp_A3);
        }

        static discrete_block INVADERS_MISSILE_10(int _board, string _type)
        {
            return DISCRETE_OP_AMP_TRIG_VCA(INVADERS_NODE(INVADERS_MISSILE_SND, _board),   /* IC A3, pin 10 */
                INVADERS_NODE(INVADERS_MISSILE_EN, _board),             /* TRG0 */
                0,                                                      /* no TRG1 */
                0,                                                      /* no TRG2 */
                INVADERS_NODE(77, _board),                              /* IN0 */
                0,                                                      /* no IN1 */
                invaders_missle_tvca);
        }


        static readonly discrete_op_amp_1sht_info invaders_explosion_1sht = new discrete_op_amp_1sht_info
        (
            DISC_OP_AMP_1SHT_1 | DISC_OP_AMP_IS_NORTON,
            RES_M(4.7),                         // R90
            RES_K(100),                         // R88
            RES_M(1),                           // R91
            RES_M(1),                           // R89
            RES_M(2.2),                         // R92
            CAP_U(2.2),                         // C24
            CAP_P(470),                         // C25
            0,                                  // vN
            12                                  // vP
        );

        static readonly discrete_op_amp_tvca_info invaders_explosion_tvca = new discrete_op_amp_tvca_info
        (
            RES_M(2.7),                         // R80
            RES_K(680),                         // R79
            0,                                  // no r3
            RES_K(680),                         // R82
            RES_K(10),                          // R93
            0,                                  // no r6
            RES_K(680),                         // R83
            0,                                  // no r8
            0,                                  // no r9
            0,                                  // no r10
            0,                                  // no r11
            CAP_U(1),                           // C26
            0,                                  // no c2
            0, 0,                               // no c3, c4
            12.0 - OP_AMP_NORTON_VBE,           // v1
            0,                                  // no v2
            0,                                  // no v3
            12,                                 // vP
            DISC_OP_AMP_TRIGGER_FUNCTION_NONE,  // no f0
            DISC_OP_AMP_TRIGGER_FUNCTION_NONE,  // no f1
            DISC_OP_AMP_TRIGGER_FUNCTION_TRG0,  // f2
            DISC_OP_AMP_TRIGGER_FUNCTION_NONE,  // no f3
            DISC_OP_AMP_TRIGGER_FUNCTION_NONE,  // no f4
            DISC_OP_AMP_TRIGGER_FUNCTION_NONE   // no f5
        );


        static discrete_block INVADERS_EXPLOSION_1(int _board)
        {
            return DISCRETE_INPUTX_LOGIC(INVADERS_NODE(INVADERS_EXPLOSION_EN, _board), 5, 0, 0);
        }

        static discrete_block INVADERS_EXPLOSION_2(int _board)
        {
            return DISCRETE_OP_AMP_ONESHOT(INVADERS_NODE(60, _board),              /* IC D2, pin 10 */
                INVADERS_NODE(INVADERS_EXPLOSION_EN, _board),           /* TRIG */
                invaders_explosion_1sht);
        }

        static discrete_block INVADERS_EXPLOSION_3(int _board)
        {
            return DISCRETE_OP_AMP_TRIG_VCA(INVADERS_NODE(61, _board),             /* IC D2, pin 4 */
                INVADERS_NODE(60, _board),                              /* TRG0 */
                0,                                                      /* no TRG1 */
                0,                                                      /* no TRG2 */
                INVADERS_NOISE,                                         /* IN0 */
                0,                                                      /* no IN1 */
                invaders_explosion_tvca);
        }

        static discrete_block INVADERS_EXPLOSION_4(int _board)
        {
            return DISCRETE_RCFILTER(INVADERS_NODE(62, _board),
                INVADERS_NODE(61, _board),                              /* IN0 */
                RES_K(5.6),                                             /* R84 */
                CAP_U(0.1) );                                           /* C27 */
        }

        static discrete_block INVADERS_EXPLOSION_5(int _board)
        {
            return DISCRETE_RCFILTER(INVADERS_NODE(INVADERS_EXPLOSION_SND, _board),
                INVADERS_NODE(62, _board),                              /* IN0 */
                RES_K(5.6) + RES_K(6.8),                                /* R84 + R85 */
                CAP_U(0.1) );                                           /* C28 */
        }


        static readonly discrete_mixer_desc invaders_mixer = new discrete_mixer_desc
        (
            DISC_MIXER_IS_OP_AMP,               // type
            new double [] { RES_K(200),                       // R78
                RES_K(10) + 100 + 100,          // R134 + R133 + R132
                RES_K(150),                     // R136
                RES_K(200),                     // R59
                RES_K(2) + RES_K(6.8) + RES_K(5.6), // R86 + R85 + R84
                RES_K(150) },                   // R28
            new int [] {0},                                // no rNode{}
            new double [] { 0,
                0,
                0,
                0,
                0,
                CAP_U(0.001) },                 // C11
            0,                                  // no rI
            RES_K(100),                         // R105
            0,                                  // no cF
            CAP_U(0.1),                         // C45
            0,                                  // vRef = ground
            1                                   // gain
        );


        static discrete_block INVADERS_MIXER_1(int _board, string _type)
        {
            return DISCRETE_MIXER6(INVADERS_NODE(90, _board),
                1,                                                      /* ENAB */
                INVADERS_NODE(INVADERS_SAUCER_HIT_SND, _board),         /* IN0 */
                INVADERS_NODE(INVADERS_FLEET_SND, _board),              /* IN1 */
                INVADERS_NODE(INVADERS_BONUS_MISSLE_BASE_SND, _board),  /* IN2 */
                INVADERS_NODE(INVADERS_INVADER_HIT_SND, _board),        /* IN3 */
                INVADERS_NODE(INVADERS_EXPLOSION_SND, _board),          /* IN4 */
                INVADERS_NODE(INVADERS_MISSILE_SND, _board),            /* IN5 */
                _type == "invaders" ? invaders_mixer : throw new emu_unimplemented());  //&_type##_mixer)
        }

        static discrete_block INVADERS_MIXER_2(int _board, string _type)
        {
            return DISCRETE_OUTPUT(INVADERS_NODE(90, _board), 2500);
        }


        static readonly discrete_op_amp_1sht_info invaders_invader_hit_1sht = new discrete_op_amp_1sht_info
        (
            DISC_OP_AMP_1SHT_1 | DISC_OP_AMP_IS_NORTON,
            RES_M(4.7),     // R49
            RES_K(100),     // R51
            RES_M(1),       // R48
            RES_M(1),       // R50
            RES_M(2.2),     // R52
            CAP_U(0.1),     // C18
            CAP_P(470),     // C20
            0,              // vN
            12              // vP
        );

        static readonly discrete_op_amp_osc_info invaders_invader_hit_osc = new discrete_op_amp_osc_info
        (
            DISC_OP_AMP_OSCILLATOR_1 | DISC_OP_AMP_IS_NORTON | DISC_OP_AMP_OSCILLATOR_OUT_CAP,
            RES_M(1),       // R37
            RES_K(10),      // R41
            RES_K(100),     // R38
            RES_K(120),     // R40
            RES_M(1),       // R39
            0,              // no r6
            0,              // no r7
            0,              // no r8
            CAP_U(0.1),     // C16
            12             // vP
        );


        //static DISCRETE_SOUND_START(invaders_discrete)
        static readonly discrete_block [] invaders_discrete = 
        {
            INVADERS_NOISE_GENERATOR,
            INVADERS_SAUCER_HIT_1(1), INVADERS_SAUCER_HIT_2(1), INVADERS_SAUCER_HIT_3(1), INVADERS_SAUCER_HIT_4(1), INVADERS_SAUCER_HIT_5(1),
            INVADERS_FLEET_1(1), INVADERS_FLEET_2(1), INVADERS_FLEET_3(1), INVADERS_FLEET_4(1), INVADERS_FLEET_5(1),
            INVADERS_BONUS_MISSLE_BASE_1(1), INVADERS_BONUS_MISSLE_BASE_2(1), INVADERS_BONUS_MISSLE_BASE_3(1), INVADERS_BONUS_MISSLE_BASE_4(1), INVADERS_BONUS_MISSLE_BASE_5(1),
            INVADERS_INVADER_HIT_1(1, "invaders"), INVADERS_INVADER_HIT_2(1, "invaders"), INVADERS_INVADER_HIT_3(1, "invaders"), INVADERS_INVADER_HIT_4(1, "invaders"), INVADERS_INVADER_HIT_5(1, "invaders"), INVADERS_INVADER_HIT_6(1, "invaders"),
            INVADERS_EXPLOSION_1(1), INVADERS_EXPLOSION_2(1), INVADERS_EXPLOSION_3(1), INVADERS_EXPLOSION_4(1), INVADERS_EXPLOSION_5(1),
            INVADERS_MISSILE_1(1, "invaders"), INVADERS_MISSILE_2(1, "invaders"), INVADERS_MISSILE_3(1, "invaders"), INVADERS_MISSILE_4(1, "invaders"), INVADERS_MISSILE_5(1, "invaders"),
            INVADERS_MISSILE_6(1, "invaders"), INVADERS_MISSILE_7(1, "invaders"), INVADERS_MISSILE_8(1, "invaders"), INVADERS_MISSILE_9(1, "invaders"), INVADERS_MISSILE_10(1, "invaders"),
            INVADERS_MIXER_1(1, "invaders"), INVADERS_MIXER_2(1, "invaders"),

            DISCRETE_SOUND_END,
        };


        required_device<sn76477_device> m_sn;
        required_device<discrete_sound_device> m_discrete;
        devcb_write_line m_flip_screen_out;
        u8 m_p2;


        invaders_audio_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : base(mconfig, INVADERS_AUDIO, tag, owner, clock)
        {
            m_sn = new required_device<sn76477_device>(this, "snsnd");
            m_discrete = new required_device<discrete_sound_device>(this, "discrete");
            m_flip_screen_out = new devcb_write_line(this);
            m_p2 = 0;
        }


        public devcb_write_line.binder flip_screen_out() { return m_flip_screen_out.bind(); }  //auto flip_screen_out() { return m_flip_screen_out.bind(); }


        public void p1_w(u8 data)
        {
            m_sn.op0.enable_w(BIT(~data, 0));    // saucer sound

            m_discrete.op0.write((offs_t)INVADERS_NODE(INVADERS_MISSILE_EN, 1), (uint8_t)(data & 0x02));
            m_discrete.op0.write((offs_t)INVADERS_NODE(INVADERS_EXPLOSION_EN, 1), (uint8_t)(data & 0x04));
            m_discrete.op0.write((offs_t)INVADERS_NODE(INVADERS_INVADER_HIT_EN, 1), (uint8_t)(data & 0x08));
            m_discrete.op0.write((offs_t)INVADERS_NODE(INVADERS_BONUS_MISSLE_BASE_EN, 1), (uint8_t)(data & 0x10));

            machine().sound().system_mute(BIT(data, 5) == 0);

            // D6 and D7 are not connected
        }


        public void p2_w(u8 data)
        {
            u8 changed = (u8)(data ^ m_p2);
            m_p2 = data;

            m_discrete.op0.write((offs_t)INVADERS_NODE(INVADERS_FLEET_DATA, 1), (uint8_t)(data & 0x0f));
            m_discrete.op0.write((offs_t)INVADERS_NODE(INVADERS_SAUCER_HIT_EN, 1), (uint8_t)(data & 0x10));

            if (BIT(changed, 5) != 0) m_flip_screen_out.op_s32(BIT(data, 5));

            // D6 and D7 are not connected
        }


        protected override void device_add_mconfig(machine_config config)
        {
            SPEAKER(config, "mono").front_center();

            SN76477(config, m_sn);
            m_sn.op0.set_noise_params(0, 0, 0);
            m_sn.op0.set_decay_res(0);
            m_sn.op0.set_attack_params(0, RES_K(100));
            m_sn.op0.set_amp_res(RES_K(56));
            m_sn.op0.set_feedback_res(RES_K(10));
            m_sn.op0.set_vco_params(0, CAP_U(0.1), RES_K(8.2));
            m_sn.op0.set_pitch_voltage(5.0);
            m_sn.op0.set_slf_params(CAP_U(1.0), RES_K(120));
            m_sn.op0.set_oneshot_params(0, 0);
            m_sn.op0.set_vco_mode(1);
            m_sn.op0.set_mixer_params(0, 0, 0);
            m_sn.op0.set_envelope_params(1, 0);
            m_sn.op0.set_enable(1);
            m_sn.op0.disound.add_route(ALL_OUTPUTS, "mono", 0.5);

            DISCRETE(config, m_discrete, invaders_discrete);
            m_discrete.op0.disound.add_route(ALL_OUTPUTS, "mono", 0.5);
        }


        protected override void device_start()
        {
            m_flip_screen_out.resolve_safe();

            m_p2 = (u8)0U;

            save_item(NAME(new { m_p2 }));
        }
    }


    //class invad2ct_audio_device : public device_t

    //class zzzap_common_audio_device : public device_t

    //class zzzap_audio_device : public zzzap_common_audio_device

    //class lagunar_audio_device : public zzzap_common_audio_device
}
