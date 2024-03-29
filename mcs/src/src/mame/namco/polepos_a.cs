// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using stream_buffer_sample_t = System.Single;  //using sample_t = float;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.cpp_global;
using static mame.device_global;
using static mame.discrete_global;
using static mame.namco52_global;
using static mame.namco54_global;
using static mame.polepos_global;
using static mame.rescap_global;


namespace mame
{
    class polepos_sound_device : device_t
                                 //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(POLEPOS_SOUND, polepos_sound_device, "polepos_sound", "Pole Position Custom Sound")
        public static readonly emu.detail.device_type_impl POLEPOS_SOUND = DEFINE_DEVICE_TYPE("polepos_sound", "Pole Position Custom Sound", (type, mconfig, tag, owner, clock) => { return new polepos_sound_device(mconfig, tag, owner, clock); });


        public class device_sound_interface_polepos : device_sound_interface
        {
            public device_sound_interface_polepos(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((polepos_sound_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        class filter2_context  //struct filter2_context
        {
            public double x0 = 0.0;  /* x[k], x[k-1], x[k-2], current and previous 2 input values */
            double x1 = 0.0;
            double x2 = 0.0;
            public double y0 = 0.0;  /* y[k], y[k-1], y[k-2], current and previous 2 output values */
            double y1 = 0.0;
            double y2 = 0.0;
            double a1 = 0.0;            /* digital filter coefficients, denominator */
            double a2 = 0.0;
            double b0 = 0.0;  /* digital filter coefficients, numerator */
            double b1 = 0.0;
            double b2 = 0.0;


            //filter2_context() { }


            public void setup(device_t device, int type, double fc, double d, double gain)
            {
                int sample_rate = device.machine().sample_rate();
                double two_over_T = 2 * sample_rate;
                double two_over_T_squared = two_over_T * two_over_T;

                /* calculate digital filter coefficents */
                /* cutoff freq, in radians/sec */
                /*w = 2.0*M_PI*fc; no pre-warping */
                double w = sample_rate * 2.0 * tan(M_PI * fc / sample_rate); /* pre-warping */
                double w_squared = w * w;

                /* temp variable */
                double den = two_over_T_squared + d * w * two_over_T + w_squared;

                a1 = 2.0 * (-two_over_T_squared + w_squared) / den;
                a2 = (two_over_T_squared - d * w * two_over_T + w_squared) / den;

                switch (type)
                {
                case FILTER_LOWPASS:
                    b0 = b2 = w_squared/den;
                    b1 = 2.0*(b0);
                    break;
                case FILTER_BANDPASS:
                    b0 = d*w*two_over_T/den;
                    b1 = 0.0;
                    b2 = -(b0);
                    break;
                case FILTER_HIGHPASS:
                    b0 = b2 = two_over_T_squared/den;
                    b1 = -2.0*(b0);
                    break;
                default:
                    device.logerror("filter2_setup() - Invalid filter type for 2nd order filter.");
                    break;
                }

                b0 *= gain;
                b1 *= gain;
                b2 *= gain;
            }


            public void opamp_m_bandpass_setup(device_t device, double r1, double r2, double r3, double c1, double c2)
            {
                if (r1 == 0)
                {
                    device.logerror("filter_opamp_m_bandpass_setup() - r1 can not be 0");
                    return; /* Filter can not be setup.  Undefined results. */
                }

                double r_in;
                double gain;

                if (r2 == 0)
                {
                    gain = 1;
                    r_in = r1;
                }
                else
                {
                    gain = r2 / (r1 + r2);
                    r_in = 1.0 / (1.0 / r1 + 1.0 / r2);
                }

                double fc = 1.0 / (2 * M_PI * sqrt(r_in * r3 * c1 * c2));
                double d = (c1 + c2) / sqrt(r3 / r_in * c1 * c2);
                gain *= -r3 / r_in * c2 / (c1 + c2);

                setup(device, FILTER_BANDPASS, fc, d, gain);
            }


            public void reset()
            {
                x0 = 0;
                x1 = 0;
                x2 = 0;
                y0 = 0;
                y1 = 0;
                y2 = 0;
            }


            public void step()
            {
                throw new emu_unimplemented();
            }
        }


        device_sound_interface_polepos m_disound;


        uint32_t m_current_position;
        int m_sample_msb;
        int m_sample_lsb;
        int m_sample_enable;
        sound_stream m_stream;
        filter2_context [] m_filter_engine = new filter2_context [3] { new filter2_context(), new filter2_context(), new filter2_context() };


        polepos_sound_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, POLEPOS_SOUND, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_polepos(mconfig, this));  //device_sound_interface(mconfig, *this),
            m_disound = GetClassInterface<device_sound_interface_polepos>();


            m_current_position = 0;
            m_sample_msb = 0;
            m_sample_lsb = 0;
            m_sample_enable = 0;
            m_stream = null;
        }


        public device_sound_interface_polepos disound { get { return m_disound; } }


        // device-level overrides
        protected override void device_start()
        {
            m_stream = m_disound.stream_alloc(0, 1, OUTPUT_RATE);
            m_sample_msb = m_sample_lsb = 0;
            m_sample_enable = 0;

            /* setup the filters */
            m_filter_engine[0].opamp_m_bandpass_setup(this, RES_K(220), RES_K(33), RES_K(390), CAP_U(.01),  CAP_U(.01));
            m_filter_engine[1].opamp_m_bandpass_setup(this, RES_K(150), RES_K(22), RES_K(330), CAP_U(.0047),  CAP_U(.0047));
            /* Filter 3 is a little different.  Because of the input capacitor, it is
             * a high pass filter. */
            m_filter_engine[2].setup(this, FILTER_HIGHPASS, 950, Q_TO_DAMP(.707), 1);
        }


        protected override void device_reset()
        {
            int loop;
            for (loop = 0; loop < 3; loop++)
                m_filter_engine[loop].reset();
        }


        // sound stream update overrides
        protected virtual void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)
        {
            uint32_t step;
            uint32_t clock;
            uint32_t slot;
            Pointer<uint8_t> base_;  //uint8_t *base;
            double volume;
            double i_total;
            var buffer = outputs[0];  //auto &buffer = outputs[0];
            int loop;

            /* if we're not enabled, just fill with 0 */
            if (m_sample_enable == 0)
            {
                buffer.fill(0);
                return;
            }

            /* determine the effective clock rate */
            clock = (unscaled_clock() / 16) * (uint32_t)((m_sample_msb + 1) * 64 + m_sample_lsb + 1) / (64 * 64);
            step = (clock << 12) / OUTPUT_RATE;

            /* determine the volume */
            slot = (uint32_t)((m_sample_msb >> 3) & 7);
            volume = volume_table[slot];
            base_ = new Pointer<uint8_t>(machine().root_device().memregion("engine").base_(), (int)(slot * 0x800));  //base = &machine().root_device().memregion("engine")->base()[slot * 0x800];

            /* fill in the sample */
            for (int sampindex = 0; sampindex < buffer.samples(); sampindex++)
            {
                m_filter_engine[0].x0 = (3.4 / 255 * base_[(m_current_position >> 12) & 0x7ff] - 2) * volume;
                m_filter_engine[1].x0 = m_filter_engine[0].x0;
                m_filter_engine[2].x0 = m_filter_engine[0].x0;

                i_total = 0;
                for (loop = 0; loop < 3; loop++)
                {
                    m_filter_engine[loop].step();
                    /* The op-amp powered @ 5V will clip to 0V & 3.5V.
                     * Adjusted to vRef of 2V, we will clip as follows: */
                    if (m_filter_engine[loop].y0 > 1.5) m_filter_engine[loop].y0 = 1.5;
                    if (m_filter_engine[loop].y0 < -2)  m_filter_engine[loop].y0 = -2;

                    i_total += m_filter_engine[loop].y0 / r_filt_out[loop];
                }

                i_total *= r_filt_total / 2;  /* now contains voltage adjusted by final gain */

                buffer.put(sampindex, (stream_buffer_sample_t)i_total);
                m_current_position += step;
            }
        }


        //DECLARE_WRITE_LINE_MEMBER(clson_w);
        public void clson_w(int state)
        {
            if (state == 0)
            {
                polepos_engine_sound_lsb_w(0);
                polepos_engine_sound_msb_w(0);
            }
        }


        public void polepos_engine_sound_lsb_w(uint8_t data)
        {
            /* Update stream first so all samples at old frequency are updated. */
            m_stream.update();
            m_sample_lsb = data & 62;
            m_sample_enable = data & 1;
        }


        public void polepos_engine_sound_msb_w(uint8_t data)
        {
            m_stream.update();
            m_sample_msb = data & 63;
        }
    }


    static class polepos_global
    {
        public const uint32_t OUTPUT_RATE         = 24000;

        const double POLEPOS_R166        = 1000.0;
        const double POLEPOS_R167        = 2200.0;
        const double POLEPOS_R168        = 4700.0;

        /* resistor values when shorted by 4066 running at 5V */
        const double POLEPOS_R166_SHUNT  = 1.0 / (1.0 / POLEPOS_R166 + 1.0 / 250);
        const double POLEPOS_R167_SHUNT  = 1.0 / (1.0 / POLEPOS_R166 + 1.0 / 250);
        const double POLEPOS_R168_SHUNT  = 1.0 / (1.0 / POLEPOS_R166 + 1.0 / 250);

        public static readonly double [] volume_table = new double [8]
        {
            (POLEPOS_R168_SHUNT + POLEPOS_R167_SHUNT + POLEPOS_R166_SHUNT + 2200) / 10000,
            (POLEPOS_R168_SHUNT + POLEPOS_R167_SHUNT + POLEPOS_R166       + 2200) / 10000,
            (POLEPOS_R168_SHUNT + POLEPOS_R167       + POLEPOS_R166_SHUNT + 2200) / 10000,
            (POLEPOS_R168_SHUNT + POLEPOS_R167       + POLEPOS_R166       + 2200) / 10000,
            (POLEPOS_R168       + POLEPOS_R167_SHUNT + POLEPOS_R166_SHUNT + 2200) / 10000,
            (POLEPOS_R168       + POLEPOS_R167_SHUNT + POLEPOS_R166       + 2200) / 10000,
            (POLEPOS_R168       + POLEPOS_R167       + POLEPOS_R166_SHUNT + 2200) / 10000,
            (POLEPOS_R168       + POLEPOS_R167       + POLEPOS_R166       + 2200) / 10000
        };


        public static readonly double [] r_filt_out = new double [3] { RES_K(4.7), RES_K(7.5), RES_K(10) };
        public static readonly double r_filt_total = 1.0 / (1.0 / RES_K(4.7) + 1.0 / RES_K(7.5) + 1.0 / RES_K(10));


        /* Max filter order */
        //#define FILTER_ORDER_MAX 51

        /* Define to use integer calculation */
        //#define FILTER_USE_INT

        //#ifdef FILTER_USE_INT
        //typedef int filter_real;
        //#define FILTER_INT_FRACT 15 /* fractional bits */
        //#else
        //typedef double filter_real;
        //#endif

        //struct filter
        //{
        //    filter_real xcoeffs[(FILTER_ORDER_MAX+1)/2];
        //    unsigned order;
        //};

        //struct filter_state
        //{
        //    unsigned prev_mac;
        //    filter_real xprev[FILTER_ORDER_MAX];
        //};

        /* Filter types */
        public const int FILTER_LOWPASS      = 0;
        public const int FILTER_HIGHPASS     = 1;
        public const int FILTER_BANDPASS     = 2;

        public static double Q_TO_DAMP(double q) { return 1.0 / q; }


        /*************************************
         *
         *  Pole Position
         *
         *  Discrete sound emulation: Feb 2007, D.R.
         *
         *************************************/

        /* nodes - sounds */
        const int POLEPOS_CHANL1_SND      = NODE_11;
        const int POLEPOS_CHANL2_SND      = NODE_12;
        const int POLEPOS_CHANL3_SND      = NODE_13;
        const int POLEPOS_CHANL4_SND      = NODE_14;

        static readonly double POLEPOS_54XX_DAC_R = 1.0 / (1.0 / RES_K(47) + 1.0 / RES_K(22) + 1.0 / RES_K(10) + 1.0 / RES_K(4.7));
        static readonly discrete_dac_r1_ladder polepos_54xx_dac = new discrete_dac_r1_ladder
        (
            4,               /* number of DAC bits */
                                /* 54XX_0   54XX_1  54XX_2 */
            new double [] { RES_K(47),     /* R124,    R136,   R152 */
                RES_K(22),   /* R120,    R132,   R142 */
                RES_K(10),   /* R119,    R131,   R138 */
                RES_K(4.7) },/* R118,    R126,   R103 */
            0, 0, 0, 0       /* nothing extra */
        );

        //#define POLEPOS_52XX_DAC_R (1.0 / (1.0 / RES_K(100) + 1.0 / RES_K(47) + 1.0 / RES_K(22) + 1.0 / RES_K(10)))
        static readonly discrete_dac_r1_ladder polepos_52xx_dac = new discrete_dac_r1_ladder
        (
            4,              /* number of DAC bits */
            new double [] { RES_K(100),   /* R160 */
                RES_K(47),  /* R159 */
                RES_K(22),  /* R155 */
                RES_K(10)}, /* R154 */
            0, 0, 0, 0      /* nothing extra */
        );

        /*                           R117        R116         R117 */
        static readonly double POLEPOS_VREF = 5.0 * (RES_K(1) / (RES_K(1.5) + RES_K(1)));

        static readonly discrete_op_amp_filt_info polepos_chanl1_filt = new discrete_op_amp_filt_info
        (
            POLEPOS_54XX_DAC_R + RES_K(22), /* R121 */
            0,                  /* no second input */
            RES_K(12),          /* R125 */
            0,                  /* not used */
            RES_K(120),         /* R122 */
            CAP_U(0.0022),      /* C27 */
            CAP_U(0.0022),      /* C28 */
            0,                  /* not used */
            POLEPOS_VREF,       /* vRef */
            5,                  /* vP */
            0                   /* vN */
        );

        static readonly discrete_op_amp_filt_info polepos_chanl2_filt = new discrete_op_amp_filt_info
        (
            POLEPOS_54XX_DAC_R + RES_K(15), /* R133 */
            0,                  /* no second input */
            RES_K(15),          /* R137 */
            0,                  /* not used */
            RES_K(120),         /* R134 */
            CAP_U(0.022),       /* C29 */
            CAP_U(0.022),       /* C30 */
            0,                  /* not used */
            POLEPOS_VREF,       /* vRef */
            5,                  /* vP */
            0                   /* vN */
        );

        static readonly discrete_op_amp_filt_info polepos_chanl3_filt = new discrete_op_amp_filt_info
        (
            POLEPOS_54XX_DAC_R + RES_K(22), /* R139 */
            0,                  /* no second input */
            RES_K(22),          /* R143 */
            0,                  /* not used */
            RES_K(180),         /* R140 */
            CAP_U(0.047),       /* C33 */
            CAP_U(0.047),       /* C34 */
            0,                  /* not used */
            POLEPOS_VREF,       /* vRef */
            5,                  /* vP */
            0                   /* vN */
        );


        //DISCRETE_SOUND_START(polepos_discrete)
        public static readonly discrete_block [] polepos_discrete = 
        {
            /************************************************
             * Input register mapping
             ************************************************/
            DISCRETE_INPUT_DATA(NAMCO_54XX_0_DATA(NODE_01)),
            DISCRETE_INPUT_DATA(NAMCO_54XX_1_DATA(NODE_01)),
            DISCRETE_INPUT_DATA(NAMCO_54XX_2_DATA(NODE_01)),
            DISCRETE_INPUT_DATA(NAMCO_52XX_P_DATA(NODE_04)),

            /************************************************
             * CHANL1 sound
             ************************************************/
            DISCRETE_DAC_R1(NODE_20,
                            NAMCO_54XX_2_DATA(NODE_01),
                            4,          /* 4V - unmeasured*/
                            polepos_54xx_dac),
            DISCRETE_OP_AMP_FILTER(NODE_21,
                            1,          /* ENAB */
                            NODE_20,    /* INP0 */
                            0,          /* INP1 - not used */
                            DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, polepos_chanl1_filt),
            /* fake it so 0 is now vRef */
            DISCRETE_ADDER2(POLEPOS_CHANL1_SND,
                            1,          /* ENAB */
                            NODE_21, -POLEPOS_VREF),

            /************************************************
             * CHANL2 sound
             ************************************************/
            DISCRETE_DAC_R1(NODE_30,
                            NAMCO_54XX_1_DATA(NODE_01),
                            4,          /* 4V - unmeasured*/
                            polepos_54xx_dac),
            DISCRETE_OP_AMP_FILTER(NODE_31,
                            1,          /* ENAB */
                            NODE_30,    /* INP0 */
                            0,          /* INP1 - not used */
                            DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, polepos_chanl2_filt),
            /* fake it so 0 is now vRef */
            DISCRETE_ADDER2(POLEPOS_CHANL2_SND,
                            1,          /* ENAB */
                            NODE_31, -POLEPOS_VREF),

            /************************************************
             * CHANL3 sound
             ************************************************/
            DISCRETE_DAC_R1(NODE_40,
                            NAMCO_54XX_0_DATA(NODE_01),
                            4,          /* 4V - unmeasured*/
                            polepos_54xx_dac),
            DISCRETE_OP_AMP_FILTER(NODE_41,
                            1,          /* ENAB */
                            NODE_40,    /* INP0 */
                            0,          /* INP1 - not used */
                            DISC_OP_AMP_FILTER_IS_BAND_PASS_1M, polepos_chanl3_filt),
            /* fake it so 0 is now vRef */
            DISCRETE_ADDER2(POLEPOS_CHANL3_SND,
                            1,          /* ENAB */
                            NODE_41, -POLEPOS_VREF),

            /************************************************
             * CHANL4 sound
             ************************************************/
            /* this circuit was simulated in SPICE and an equivalent filter circuit generated */
            DISCRETE_DAC_R1(NODE_50,
                            NAMCO_52XX_P_DATA(NODE_04),
                            4,          /* 4V - unmeasured*/
                            polepos_52xx_dac),
            /* fake it so 0 is now vRef */
            DISCRETE_ADDER2(NODE_51,
                            1,          /* ENAB */
                            NODE_50, -POLEPOS_VREF),
            DISCRETE_FILTER2(NODE_52,
                            1,          /* ENAB */
                            NODE_51,    /* INP0 */
                            100,        /* FREQ */
                            1.0 / 0.3,  /* DAMP */
                            DISC_FILTER_HIGHPASS),
            DISCRETE_FILTER2(NODE_53,
                            1,          /* ENAB */
                            NODE_52,    /* INP0 */
                            1200,       /* FREQ */
                            1.0 / 0.8,  /* DAMP */
                            DISC_FILTER_LOWPASS),
            DISCRETE_GAIN(NODE_54,
                            NODE_53,    /* IN0 */
                            0.5         /* overall filter GAIN */),
            /* clamp to the maximum of the op-amp shifted by vRef */
            DISCRETE_CLAMP(POLEPOS_CHANL4_SND,
                            NODE_54,    /* IN0 */
                            0,          /* MIN */
                            5.0 - OP_AMP_VP_RAIL_OFFSET - POLEPOS_VREF), /* MAX */

            /************************************************
             * Output
             ************************************************/
            DISCRETE_OUTPUT(POLEPOS_CHANL1_SND, 32767/2),
            DISCRETE_OUTPUT(POLEPOS_CHANL2_SND, 32767/2),
            DISCRETE_OUTPUT(POLEPOS_CHANL3_SND, 32767/2),
            DISCRETE_OUTPUT(POLEPOS_CHANL4_SND, 32767/2),

            DISCRETE_SOUND_END,
        };


        public static polepos_sound_device POLEPOS_SOUND(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<polepos_sound_device>(mconfig, tag, polepos_sound_device.POLEPOS_SOUND, clock); }
    }
}
