// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using stream_sample_t = System.Int32;
using u32 = System.UInt32;
using uint8_t = System.Byte;


namespace mame
{
    class device_sound_interface_galaxian_sound : device_sound_interface
    {
        public device_sound_interface_galaxian_sound(machine_config mconfig, device_t device) : base(mconfig, device) { }


        // device_sound_interface overrides
        //-------------------------------------------------
        //  sound_stream_update - handle a stream update
        //-------------------------------------------------
        public override void sound_stream_update(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples)
        {
            throw new emu_unimplemented();
        }
    }


    class galaxian_sound_device : device_t
                                  //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(GALAXIAN, galaxian_sound_device, "galaxian_sound", "Galaxian Custom Sound")
        static device_t device_creator_galaxian_sound_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new galaxian_sound_device(mconfig, tag, owner, clock); }
        public static readonly device_type GALAXIAN = DEFINE_DEVICE_TYPE(device_creator_galaxian_sound_device, "galaxian_sound", "Galaxian Custom Sound");


        device_sound_interface_galaxian_sound m_disound;


        // internal state
        uint8_t m_lfo_val;
        required_device<discrete_device> m_discrete;


        galaxian_sound_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, GALAXIAN, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_galaxian_sound(mconfig, this));  // device_sound_interface(mconfig, *this);


            m_lfo_val = 0;
            m_discrete = new required_device<discrete_device>(this, "^" + galaxian.GAL_AUDIO);
        }


        /* FIXME: May be replaced by one call! */
        //WRITE8_MEMBER( galaxian_sound_device::sound_w )
        public void sound_w(address_space space, offs_t offset, byte data, byte mem_mask = 0xff)
        {
            data &= 0x01;
            switch (offset & 7)
            {
                case 0:     /* FS1 (controls 555 timer at 8R) */
                case 1:     /* FS2 (controls 555 timer at 8S) */
                case 2:     /* FS3 (controls 555 timer at 8T) */
                    background_enable_w(space, offset, data);
                    break;

                case 3:     /* HIT */
                    noise_enable_w(space, 0, data);
                    break;

                case 4:     /* n/c */
                    break;

                case 5:     /* FIRE */
                    fire_enable_w(space, 0, data);
                    break;

                case 6:     /* VOL1 */
                case 7:     /* VOL2 */
                    vol_w(space, offset & 1, data);
                    break;
            }
        }


        /* IC 9J */
        //WRITE8_MEMBER( galaxian_sound_device::pitch_w )
        public void pitch_w(address_space space, offs_t offset, byte data, byte mem_mask = 0xff)
        {
            m_discrete.target.write(space, (UInt32)galaxian.GAL_INP_PITCH, data );
        }


        //WRITE8_MEMBER( galaxian_sound_device::vol_w )
        public void vol_w(address_space space, offs_t offset, byte data, byte mem_mask = 0xff)
        {
            m_discrete.target.write(space, (UInt32)discrete_global.NODE_RELATIVE(galaxian.GAL_INP_VOL1, (int)offset), (byte)(data & 0x01));
        }


        //WRITE8_MEMBER( galaxian_sound_device::noise_enable_w )
        public void noise_enable_w(address_space space, offs_t offset, byte data, byte mem_mask = 0xff)
        {
            m_discrete.target.write(space, (UInt32)galaxian.GAL_INP_HIT, (byte)(data & 0x01));
        }


        //WRITE8_MEMBER( galaxian_sound_device::background_enable_w )
        public void background_enable_w(address_space space, offs_t offset, byte data, byte mem_mask = 0xff)
        {
            m_discrete.target.write(space, (UInt32)discrete_global.NODE_RELATIVE(galaxian.GAL_INP_FS1, (int)offset), (byte)(data & 0x01));
        }


        //WRITE8_MEMBER( galaxian_sound_device::fire_enable_w )
        public void fire_enable_w(address_space space, offs_t offset, byte data, byte mem_mask = 0xff)
        {
            m_discrete.target.write(space, (UInt32)galaxian.GAL_INP_FIRE, (byte)(data & 0x01));
        }


        //WRITE8_MEMBER( galaxian_sound_device::lfo_freq_w )
        public void lfo_freq_w(address_space space, offs_t offset, byte data, byte mem_mask = 0xff)
        {
            byte lfo_val_new = (byte)((m_lfo_val & ~(1<<(int)offset)) | ((data & 0x01) << (int)offset));

            if (m_lfo_val != lfo_val_new)
            {
                m_lfo_val = lfo_val_new;
                m_discrete.target.write(space, (UInt32)galaxian.GAL_INP_BG_DAC, m_lfo_val);
            }
        }


        //void mooncrst_audio(machine_config &config);
        //void galaxian_audio(machine_config &config);


        // device-level overrides
        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            m_disound = GetClassInterface<device_sound_interface_galaxian_sound>();


            m_lfo_val = 0;

            save_item(m_lfo_val, "m_lfo_val");
        }


        // sound stream update overrides
        //virtual void sound_stream_update(sound_stream &stream, stream_sample_t **inputs, stream_sample_t **outputs, int samples) override;
    }


    public partial class galaxian : device_init_helpers
    {
        public const string GAL_AUDIO   = "discrete";


        static readonly XTAL SOUND_CLOCK             = galaxian_state.GALAXIAN_MASTER_CLOCK/6/2;          /* 1.536 MHz */
        static readonly XTAL RNG_RATE                = galaxian_state.GALAXIAN_MASTER_CLOCK/3*2;          /* RNG clock is XTAL/3*2 see Aaron's note in video/galaxian.c */

        /* 74LS259 */
        public const NODE GAL_INP_BG_DAC          = NODE.NODE_10;     /* at 9M Q4 to Q7 in schematics */

        public const NODE GAL_INP_FS1             = NODE.NODE_20;     /* FS1 9L Q0 */
        const NODE GAL_INP_FS2             = NODE.NODE_21;     /* FS2 9L Q1 */
        const NODE GAL_INP_FS3             = NODE.NODE_22;     /* FS3 9L Q2 */
        public const NODE GAL_INP_HIT             = NODE.NODE_23;     /* HIT 9L Q3 */
        //#define GAL_9L_Q4             NODE_24
        public const NODE GAL_INP_FIRE            = NODE.NODE_25;     /* FIRE 9L Q5 */
        public const NODE GAL_INP_VOL1            = NODE.NODE_26;     /* VOL1 9L Q6 */
        const NODE GAL_INP_VOL2            = NODE.NODE_27;     /* VOL2 9L Q7 */

        public const NODE GAL_INP_PITCH           = NODE.NODE_28;     /* at 6T in schematics */

        const double TTL_OUT                 = 4.0;

        static readonly double GAL_R15                 = rescap_global.RES_K(100);
        static readonly double GAL_R16                 = rescap_global.RES_K(220);
        static readonly double GAL_R17                 = rescap_global.RES_K(470);
        static readonly double GAL_R18                 = rescap_global.RES_K(1000);
        static readonly double GAL_R19                 = rescap_global.RES_K(330);

        static readonly double GAL_R20                 = rescap_global.RES_K(15);
        static readonly double GAL_R21                 = rescap_global.RES_K(100);
        static readonly double GAL_R22                 = rescap_global.RES_K(100);
        static readonly double GAL_R23                 = rescap_global.RES_K(470);
        static readonly double GAL_R24                 = rescap_global.RES_K(10);
        static readonly double GAL_R25                 = rescap_global.RES_K(100);
        static readonly double GAL_R26                 = rescap_global.RES_K(330);
        static readonly double GAL_R27                 = rescap_global.RES_K(10);
        static readonly double GAL_R28                 = rescap_global.RES_K(100);
        static readonly double GAL_R29                 = rescap_global.RES_K(220);

        static readonly double GAL_R30                 = rescap_global.RES_K(10);
        static readonly double GAL_R31                 = rescap_global.RES_K(47);
        static readonly double GAL_R32                 = rescap_global.RES_K(47);
        static readonly double GAL_R33                 = rescap_global.RES_K(10);
        /*
         * R34 is given twice on galaxian board and both times as 5.1k. On moon cresta
         * it is only listed once and given as 15k. This is more in line with recordings
         */
        static readonly double GAL_R34                 = rescap_global.RES_K(5.1);
        //#define MCRST_R34                   RES_K(15)

        static readonly double GAL_R35                 = rescap_global.RES_K(150);
        static readonly double GAL_R36                 = rescap_global.RES_K(22);
        static readonly double GAL_R37                 = rescap_global.RES_K(470);
        static readonly double GAL_R38                 = rescap_global.RES_K(33);
        static readonly double GAL_R39                 = rescap_global.RES_K(22);

        /* The hit sound is too low compared with recordings
         * There may be an issue with the op-amp band filter
         */
        static readonly double GAL_R40                 = (rescap_global.RES_K(2.2)*0.6);    /* Volume adjust */
        static readonly double GAL_R41                 = rescap_global.RES_K(100);
        static readonly double GAL_R43                 = rescap_global.RES_K(2.2);
        static readonly double GAL_R44                 = rescap_global.RES_K(10);
        static readonly double GAL_R45                 = rescap_global.RES_K(22);
        static readonly double GAL_R46                 = rescap_global.RES_K(10);
        static readonly double GAL_R47                 = rescap_global.RES_K(2.2);
        static readonly double GAL_R48                 = rescap_global.RES_K(2.2);
        static readonly double GAL_R49                 = rescap_global.RES_K(10);

        static readonly double GAL_R50                 = rescap_global.RES_K(22);
        static readonly double GAL_R51                 = rescap_global.RES_K(33);
        static readonly double GAL_R52                 = rescap_global.RES_K(15);

        static readonly double GAL_R91                 = rescap_global.RES_K(10);

        static readonly double GAL_C15                 = rescap_global.CAP_U(1);
        static readonly double GAL_C17                 = rescap_global.CAP_U(0.01);
        static readonly double GAL_C18                 = rescap_global.CAP_U(0.01);
        static readonly double GAL_C19                 = rescap_global.CAP_U(0.01);

        static readonly double GAL_C20                 = rescap_global.CAP_U(0.1);
        static readonly double GAL_C21                 = rescap_global.CAP_U(2.2);
        static readonly double GAL_C22                 = rescap_global.CAP_U(0.01);
        static readonly double GAL_C23                 = rescap_global.CAP_U(0.01);
        static readonly double GAL_C25                 = rescap_global.CAP_U(1);
        static readonly double GAL_C26                 = rescap_global.CAP_U(0.01);
        static readonly double GAL_C27                 = rescap_global.CAP_U(0.01);
        static readonly double GAL_C28                 = rescap_global.CAP_U(47);

        static readonly double GAL_C46                 = rescap_global.CAP_U(0.1);


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
            discrete_global.DISC_555_OUT_DC | discrete_global.DISC_555_OUT_CAP,
            5,      // B+ voltage of 555
            discrete_global.DEFAULT_555_VALUES_1, discrete_global.DEFAULT_555_VALUES_2,
            0.7     // Q2 junction voltage
        );

        static readonly discrete_555_desc galaxian_555_vco_desc = new discrete_555_desc
        (
            discrete_global.DISC_555_OUT_ENERGY | discrete_global.DISC_555_OUT_DC,
            5.0,
            discrete_global.DEFAULT_555_CHARGE,
            (5.0 - 0.5)         // 10k means no real load
        );

        static readonly discrete_555_desc galaxian_555_fire_vco_desc = new discrete_555_desc
        (
            discrete_global.DISC_555_OUT_DC,
            5.0,
            discrete_global.DEFAULT_555_CHARGE,
            1.0 // Logic output
        );

        static readonly discrete_mixer_desc galaxian_bck_mixer_desc = new discrete_mixer_desc
        (
            discrete_global.DISC_MIXER_IS_RESISTOR,
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
            discrete_global.DISC_CLK_IS_FREQ,
            17,                     /* Bit Length */
            0,                      /* Reset Value */
            4,                      /* Use Bit 10 (QC of second LS164) as F0 input 0 */
            16,                     /* Use Bit 23 (QH of third LS164) as F0 input 1 */
            discrete_global.DISC_LFSR_XOR_INV_IN1,  /* F0 is XOR */
            discrete_global.DISC_LFSR_IN0,          /* F1 is inverted F0*/
            discrete_global.DISC_LFSR_REPLACE,      /* F2 replaces the shifted register contents */
            0x000001,               /* Everything is shifted into the first bit only */
            discrete_global.DISC_LFSR_FLAG_OUTPUT_F0, /* Output is result of F0 */
            0                       /* Output bit */
        );

        static readonly discrete_mixer_desc galaxian_mixerpre_desc = new discrete_mixer_desc
        (
            discrete_global.DISC_MIXER_IS_RESISTOR,
            new double [] {GAL_R51, 0, GAL_R50, 0, GAL_R34},      /* A, C, C, D */
            new int [] {0, (int)GAL_INP_VOL1, 0, (int)GAL_INP_VOL2, 0},
            new double [] {0,0,0,0,0},
            0, 0,
            0,
            0,
            0, 1
        );

        static readonly discrete_mixer_desc galaxian_mixer_desc = new discrete_mixer_desc
        (
            discrete_global.DISC_MIXER_IS_RESISTOR,
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

        //static DISCRETE_SOUND_START(galaxian_discrete)
        public static readonly discrete_block [] galaxian_discrete = new discrete_block []
        {
            /************************************************/
            /* Input register mapping for galaxian          */
            /************************************************/
            DISCRETE_INPUT_DATA(GAL_INP_BG_DAC),

            /* FS1 to FS3 */
            DISCRETE_INPUT_LOGIC(GAL_INP_FS1),
            DISCRETE_INPUT_LOGIC(GAL_INP_FS2),
            DISCRETE_INPUT_LOGIC(GAL_INP_FS3),

            /* HIT */
            DISCRETE_INPUTX_DATA(GAL_INP_HIT, TTL_OUT, 0, 0),

            /* FIRE */
            DISCRETE_INPUT_LOGIC(GAL_INP_FIRE),

            /* Turns on / off resistors in mixer */
            DISCRETE_INPUTX_DATA(GAL_INP_VOL1, GAL_R49, 0, 0),
            DISCRETE_INPUTX_DATA(GAL_INP_VOL2, GAL_R52, 0, 0),

            /* Pitch */
            DISCRETE_INPUT_DATA(GAL_INP_PITCH),

            DISCRETE_TASK_START(0),

                /************************************************/
                /* NOISE                                        */
                /************************************************/

                /* since only a sample of the LFSR is latched @V2 we let the lfsr
                 * run at a lower speed
                 */
                DISCRETE_LFSR_NOISE(NODE.NODE_150, 1, 1, (int)(RNG_RATE.dvalue()/100), 1.0, 0, 0.5, galaxian_lfsr),
                DISCRETE_SQUAREWFIX(NODE.NODE_151,1,60*264/2,1.0,50,0.5,0),  /* 2V signal */
                DISCRETE_LOGIC_DFLIPFLOP(NODE.NODE_152,1,1,NODE.NODE_151,NODE.NODE_150),
            DISCRETE_TASK_END(),

            /* Group Background and pitch */
            DISCRETE_TASK_START(1),

                /************************************************/
                /* Background                                   */
                /************************************************/

                DISCRETE_DAC_R1(NODE.NODE_100, GAL_INP_BG_DAC, TTL_OUT, galaxian_bck_dac),
                DISCRETE_555_CC(NODE.NODE_105, 1, NODE.NODE_100, GAL_R21, GAL_C15, 0, 0, 0, galaxian_bck_vco),
                // Next is mult/add opamp circuit
                DISCRETE_MULTADD(NODE.NODE_110, NODE.NODE_105, GAL_R33/rescap_global.RES_3_PARALLEL(GAL_R31,GAL_R32,GAL_R33),
                        -5.0*GAL_R33/GAL_R31),
                DISCRETE_CLAMP(NODE.NODE_111,NODE.NODE_110,0.0,5.0),
                // The three 555
                DISCRETE_555_ASTABLE_CV(NODE.NODE_115, (int)GAL_INP_FS1, GAL_R22, GAL_R23, GAL_C17, NODE.NODE_111, galaxian_555_vco_desc),
                DISCRETE_555_ASTABLE_CV(NODE.NODE_116, (int)GAL_INP_FS2, GAL_R25, GAL_R26, GAL_C18, NODE.NODE_111, galaxian_555_vco_desc),
                DISCRETE_555_ASTABLE_CV(NODE.NODE_117, (int)GAL_INP_FS3, GAL_R28, GAL_R29, GAL_C19, NODE.NODE_111, galaxian_555_vco_desc),

                DISCRETE_MIXER3(NODE.NODE_120, 1, NODE.NODE_115, NODE.NODE_116, NODE.NODE_117, galaxian_bck_mixer_desc),

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
                DISCRETE_NOTE(NODE.NODE_132, 1, (int)SOUND_CLOCK.dvalue(), GAL_INP_PITCH, 255, 15,  discrete_global.DISC_CLK_IS_FREQ),

                /* from the 74393 (counter 2 above) only QA, QC, QD are used.
                 * We decode three here and use SUB_NODE(133,x) below to access.
                 */
                DISCRETE_BITS_DECODE(NODE.NODE_133, NODE.NODE_132, 0, 3, TTL_OUT),     /* QA-QD 74393 */

            /* End of this task */
            DISCRETE_TASK_END(),

            DISCRETE_TASK_START(1),

                /************************************************/
                /* HIT                                          */
                /************************************************/

                /* Not 100% correct - switching causes high impedance input for node_157
                 * this is not emulated */
                DISCRETE_RCDISC5(NODE.NODE_155, NODE.NODE_152, GAL_INP_HIT, (GAL_R35 + GAL_R36), GAL_C21),
                DISCRETE_OP_AMP_FILTER(NODE.NODE_157, 1, NODE.NODE_155, 0, discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, galaxian_bandpass_desc),
            DISCRETE_TASK_END(),

            DISCRETE_TASK_START(1),
                /************************************************/
                /* FIRE                                         */
                /************************************************/

                DISCRETE_LOGIC_INVERT(NODE.NODE_170, GAL_INP_FIRE),
                DISCRETE_MULTIPLY(NODE.NODE_171, TTL_OUT, GAL_INP_FIRE),
                DISCRETE_MULTIPLY(NODE.NODE_172, TTL_OUT, NODE.NODE_170), // inverted
                DISCRETE_RCFILTER(NODE.NODE_173, NODE.NODE_172, GAL_R47, GAL_C28),
                /* Mix noise and 163 */
                DISCRETE_TRANSFORM5(NODE.NODE_177, NODE.NODE_152, TTL_OUT, 1.0/GAL_R46, NODE.NODE_173, 1.0/GAL_R48,
                        "01*2*34*+" ),
                //DISCRETE_MULTIPLY(NODE_174, 1, TTL_OUT, NODE_152)
                //DISCRETE_MULTIPLY(NODE_175, 1, 1.0/GAL_R46, NODE_174)
                //DISCRETE_MULTIPLY(NODE_176, 1, 1.0/GAL_R48, NODE_173)
                //DISCRETE_ADDER2(NODE_177, 1, NODE_175, NODE_176)
                DISCRETE_MULTIPLY(NODE.NODE_178, rescap_global.RES_2_PARALLEL(GAL_R46, GAL_R48), NODE.NODE_177),

                DISCRETE_555_ASTABLE_CV(NODE.NODE_181, 1, GAL_R44, GAL_R45, GAL_C27, NODE.NODE_178, galaxian_555_fire_vco_desc),

                /* 555 toggles discharge on rc discharge module */
                DISCRETE_RCDISC5(NODE.NODE_182, NODE.NODE_181, NODE.NODE_171, (GAL_R41), GAL_C25),

            /* End of task */
            DISCRETE_TASK_END(),

            /************************************************/
            /* FINAL MIX                                    */
            /************************************************/

            DISCRETE_TASK_START(2),
                DISCRETE_MIXER5(NODE.NODE_279, 1, NODE.NODE_133_00, NODE.NODE_133_02, NODE.NODE_133_02, NODE.NODE_133_03, NODE.NODE_120, galaxian_mixerpre_desc),
                DISCRETE_MIXER3(NODE.NODE_280, 1, NODE.NODE_279, NODE.NODE_157, NODE.NODE_182, galaxian_mixer_desc),
                DISCRETE_OUTPUT(NODE.NODE_280, 32767.0/5.0*5),
            DISCRETE_TASK_END(),

            DISCRETE_SOUND_END(),
        };


        /*************************************
         *
         *  Driver definitions
         *
         *************************************/

        //MACHINE_CONFIG_START(galaxold_state::galaxian_audio)
        void galaxold_state_galaxian_audio(machine_config config, device_t owner, device_t device)
        {
            MACHINE_CONFIG_START(config, owner, device);

            galaxian_state state = (galaxian_state)helper_owner;

            MCFG_DEVICE_ADD("cust", galaxian_sound_device.GALAXIAN, 0);
            MCFG_SOUND_ROUTE(ALL_OUTPUTS, "speaker", 0.4);

            MCFG_DEVICE_ADD(GAL_AUDIO, discrete_sound_device.DISCRETE);//, galaxian_discrete);
            MCFG_DEVICE_ADD_discrete_sound_device(galaxian_discrete);
            MCFG_SOUND_ROUTE(ALL_OUTPUTS, "speaker", 1.0);

            MACHINE_CONFIG_END();
        }


        //MACHINE_CONFIG_START(galaxian_state::galaxian_audio)
        void galaxian_state_galaxian_audio(machine_config config, device_t owner, device_t device)
        {
            MACHINE_CONFIG_START(config, owner, device);

            galaxian_state state = (galaxian_state)helper_owner;

            MCFG_DEVICE_ADD("cust", galaxian_sound_device.GALAXIAN, 0);
            MCFG_SOUND_ROUTE(ALL_OUTPUTS, "speaker", 0.4);

            MCFG_DEVICE_ADD(GAL_AUDIO, discrete_sound_device.DISCRETE);//, galaxian_discrete);
            MCFG_DEVICE_ADD_discrete_sound_device(galaxian_discrete);
            MCFG_SOUND_ROUTE(ALL_OUTPUTS, "speaker", 1.0);

            MACHINE_CONFIG_END();
        }
    }
}
