// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using int16_t = System.Int16;
using stream_buffer_sample_t = System.Single;  //using sample_t = float;
using uint32_t = System.UInt32;

using static mame.cpp_global;
using static mame.device_global;
using static mame.emucore_global;
using static mame.rescap_global;
using static mame.sn76477_global;


namespace mame
{
    /*****************************************************************************
     *
     *  Interface definition
     *
     *****************************************************************************/

    public class sn76477_device : device_t
                                  //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(SN76477, sn76477_device, "sn76477", "TI SN76477 CSG")
        public static readonly emu.detail.device_type_impl SN76477 = DEFINE_DEVICE_TYPE("sn76477", "TI SN76477 CSG", (type, mconfig, tag, owner, clock) => { return new sn76477_device(mconfig, tag, owner, clock); });


        public class device_sound_interface_sn76477 : device_sound_interface
        {
            public device_sound_interface_sn76477(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((sn76477_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        device_sound_interface_sn76477 m_disound;


        const int VERBOSE                 = 1;

        /* if 1, create a .wav file of the sound produced */
        const int LOG_WAV                 = 0;

        /* if 1 and LOG_WAV is 1, only logs to .wav file in chip is enabled */
        const int LOG_WAV_ENABLED_ONLY    = 0;

        /* determines what value will be logged in the left channel of the .wav file */
        const int LOG_WAV_VALUE_L         = 0;   /* 0 = OUT voltage */
                                            /* 1 = enable line */
                                            /* 2 = one-shot cap voltage */
                                            /* 3 = a/d cap voltage */
                                            /* 4 = slf cap voltage */
                                            /* 5 = vco cap voltage */
                                            /* 6 = noise filter cap voltage */

        /* determines what value will be logged in the right channel of the .wav file
           same values as for the left channel above */
        const int LOG_WAV_VALUE_R         = 3;

        const double LOG_WAV_GAIN_FACTOR     = 1000;

        //#define LOG_WAV_FILE_NAME       "sn76477_%s.wav"

        void LOG(int n, string format, params object [] args) { if (VERBOSE >= n) logerror(format, args); }


        static void CHECK_BOOLEAN(int state) { assert((state & 0x01) == state); }
        //#define CHECK_POSITIVE     assert(data >= 0.0)
        //#define CHECK_VOLTAGE      assert((data >= 0.0) && (data <= 5.0))
        //#define CHECK_CAP_VOLTAGE  assert(((data >= 0.0) && (data <= 5.0)) || (data == EXTERNAL_VOLTAGE_DISCONNECT))

        /*****************************************************************************
         *
         *  Constants
         *
         *****************************************************************************/

        const double ONE_SHOT_CAP_VOLTAGE_MIN    = 0;         /* the voltage at which the one-shot starts from (measured) */
        const double ONE_SHOT_CAP_VOLTAGE_MAX    = 2.5;       /* the voltage at which the one-shot finishes (measured) */
        const double ONE_SHOT_CAP_VOLTAGE_RANGE  = ONE_SHOT_CAP_VOLTAGE_MAX - ONE_SHOT_CAP_VOLTAGE_MIN;

        const double SLF_CAP_VOLTAGE_MIN         = 0.33;      /* the voltage at the bottom peak of the SLF triangle wave (measured) */
        const double SLF_CAP_VOLTAGE_MAX         = 2.37;      /* the voltage at the top peak of the SLF triangle wave (measured) */
        const double SLF_CAP_VOLTAGE_RANGE       = SLF_CAP_VOLTAGE_MAX - SLF_CAP_VOLTAGE_MIN;

        const double VCO_MAX_EXT_VOLTAGE         = 2.35;      /* the external voltage at which the VCO saturates and produces no output,
                                                                 also used as the voltage threshold for the SLF */
        const double VCO_TO_SLF_VOLTAGE_DIFF     = 0.35;
        const double VCO_CAP_VOLTAGE_MIN         = SLF_CAP_VOLTAGE_MIN;   /* the voltage at the bottom peak of the VCO triangle wave */
        const double VCO_CAP_VOLTAGE_MAX         = SLF_CAP_VOLTAGE_MAX + VCO_TO_SLF_VOLTAGE_DIFF; /* the voltage at the bottom peak of the VCO triangle wave */
        const double VCO_CAP_VOLTAGE_RANGE       = VCO_CAP_VOLTAGE_MAX - VCO_CAP_VOLTAGE_MIN;
        const double VCO_DUTY_CYCLE_50           = 5.0;       /* the high voltage that produces a 50% duty cycle */
        const double VCO_MIN_DUTY_CYCLE          = 18;        /* the smallest possible duty cycle, in % */

        static readonly double NOISE_MIN_CLOCK_RES = RES_K(10);   /* the maximum resistor value that still produces a noise (measured) */
        static readonly double NOISE_MAX_CLOCK_RES = RES_M(3.3);  /* the minimum resistor value that still produces a noise (measured) */
        const double NOISE_CAP_VOLTAGE_MIN       = 0;         /* the minimum voltage that the noise filter cap can hold (measured) */
        const double NOISE_CAP_VOLTAGE_MAX       = 5.0;       /* the maximum voltage that the noise filter cap can hold (measured) */
        const double NOISE_CAP_VOLTAGE_RANGE     = NOISE_CAP_VOLTAGE_MAX - NOISE_CAP_VOLTAGE_MIN;
        const double NOISE_CAP_HIGH_THRESHOLD    = 3.35;      /* the voltage at which the filtered noise bit goes to 0 (measured) */
        const double NOISE_CAP_LOW_THRESHOLD     = 0.74;      /* the voltage at which the filtered noise bit goes to 1 (measured) */

        const double AD_CAP_VOLTAGE_MIN          = 0;         /* the minimum voltage the attack/decay cap can hold (measured) */
        const double AD_CAP_VOLTAGE_MAX          = 4.44;      /* the minimum voltage the attack/decay cap can hold (measured) */
        const double AD_CAP_VOLTAGE_RANGE        = AD_CAP_VOLTAGE_MAX - AD_CAP_VOLTAGE_MIN;

        const double OUT_CENTER_LEVEL_VOLTAGE    = 2.57;      /* the voltage that gets outputted when the volumne is 0 (measured) */
        const double OUT_HIGH_CLIP_THRESHOLD     = 3.51;      /* the maximum voltage that can be put out (measured) */
        const double OUT_LOW_CLIP_THRESHOLD      = 0.715;     /* the minimum voltage that can be put out (measured) */


        /* gain factors for OUT voltage in 0.1V increments (measured) */
        static readonly double [] out_pos_gain =
        {
            0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.01,  /* 0.0 - 0.9V */
            0.03, 0.11, 0.15, 0.19, 0.21, 0.23, 0.26, 0.29, 0.31, 0.33,  /* 1.0 - 1.9V */
            0.36, 0.38, 0.41, 0.43, 0.46, 0.49, 0.52, 0.54, 0.57, 0.60,  /* 2.0 - 2.9V */
            0.62, 0.65, 0.68, 0.70, 0.73, 0.76, 0.80, 0.82, 0.84, 0.87,  /* 3.0 - 3.9V */
            0.90, 0.93, 0.96, 0.98, 1.00                                 /* 4.0 - 4.4V */
        };

        static readonly double [] out_neg_gain =
        {
             0.00,  0.00,  0.00,  0.00,  0.00,  0.00,  0.00,  0.00,  0.00, -0.01,  /* 0.0 - 0.9V */
            -0.02, -0.09, -0.13, -0.15, -0.17, -0.19, -0.22, -0.24, -0.26, -0.28,  /* 1.0 - 1.9V */
            -0.30, -0.32, -0.34, -0.37, -0.39, -0.41, -0.44, -0.46, -0.48, -0.51,  /* 2.0 - 2.9V */
            -0.53, -0.56, -0.58, -0.60, -0.62, -0.65, -0.67, -0.69, -0.72, -0.74,  /* 3.0 - 3.9V */
            -0.76, -0.78, -0.81, -0.84, -0.85                                      /* 4.0 - 4.4V */
        };


        /* chip's external interface */
        uint32_t m_enable;
        uint32_t m_envelope_mode;
        uint32_t m_vco_mode;
        uint32_t m_mixer_mode;

        double m_one_shot_res;
        double m_one_shot_cap;
        uint32_t m_one_shot_cap_voltage_ext;

        double m_slf_res;
        double m_slf_cap;
        uint32_t m_slf_cap_voltage_ext;

        double m_vco_voltage;
        double m_vco_res;
        double m_vco_cap;
        uint32_t m_vco_cap_voltage_ext;

        double m_noise_clock_res;
        uint32_t m_noise_clock_ext;
        uint32_t m_noise_clock;
        double m_noise_filter_res;
        double m_noise_filter_cap;
        uint32_t m_noise_filter_cap_voltage_ext;

        double m_attack_res;
        double m_decay_res;
        double m_attack_decay_cap;
        uint32_t m_attack_decay_cap_voltage_ext;

        double m_amplitude_res;
        double m_feedback_res;
        double m_pitch_voltage;

        // internal state
        double m_one_shot_cap_voltage;        // voltage on the one-shot cap
        uint32_t m_one_shot_running_ff;       // 1 = one-shot running, 0 = stopped

        double m_slf_cap_voltage;             // voltage on the SLF cap
        uint32_t m_slf_out_ff;                // output of the SLF

        double m_vco_cap_voltage;             // voltage on the VCO cap
        uint32_t m_vco_out_ff;                // output of the VCO
        uint32_t m_vco_alt_pos_edge_ff;       // keeps track of the # of positive edges for VCO Alt envelope

        double m_noise_filter_cap_voltage;    // voltage on the noise filter cap
        uint32_t m_real_noise_bit_ff;         // the current noise bit before filtering
        uint32_t m_filtered_noise_bit_ff;     // the noise bit after filtering
        uint32_t m_noise_gen_count;           // noise freq emulation

        double m_attack_decay_cap_voltage;    // voltage on the attack/decay cap

        uint32_t m_rng;                       // current value of the random number generator

        // configured by the drivers and used to setup m_mixer_mode & m_envelope_mode at start
        uint32_t m_mixer_a;
        uint32_t m_mixer_b;
        uint32_t m_mixer_c;
        uint32_t m_envelope_1;
        uint32_t m_envelope_2;

        /* others */
        sound_stream m_channel;              // returned by stream_create()
        int m_our_sample_rate;                // from machine.sample_rate()

        util.wav_file m_file;  //util::wav_file_ptr m_file;            // handle of the wave file to produce


        sn76477_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, SN76477, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_sn76477(mconfig, this));  //device_sound_interface(mconfig, *this)
            m_disound = GetClassInterface<device_sound_interface_sn76477>();

            m_enable = 0;
            m_envelope_mode = 0;
            m_vco_mode = 0;
            m_mixer_mode = 0;
            m_one_shot_res = RES_INF;
            m_one_shot_cap = 0;
            m_one_shot_cap_voltage_ext = 0;
            m_slf_res = RES_INF;
            m_slf_cap = 0;
            m_slf_cap_voltage_ext = 0;
            m_vco_voltage = 0;
            m_vco_res = RES_INF;
            m_vco_cap = 0;
            m_vco_cap_voltage_ext = 0;
            m_noise_clock_res = RES_INF;
            m_noise_clock_ext = 0;
            m_noise_clock = 0;
            m_noise_filter_res = RES_INF;
            m_noise_filter_cap = 0;
            m_noise_filter_cap_voltage_ext = 0;
            m_attack_res = RES_INF;
            m_decay_res = RES_INF;
            m_attack_decay_cap = 0;
            m_attack_decay_cap_voltage_ext = 0;
            m_amplitude_res = RES_INF;
            m_feedback_res = RES_INF;
            m_pitch_voltage = 0;
            m_one_shot_cap_voltage = 0;
            m_one_shot_running_ff = 0;
            m_slf_cap_voltage = 0;
            m_slf_out_ff = 0;
            m_vco_cap_voltage = 0;
            m_vco_out_ff = 0;
            m_vco_alt_pos_edge_ff = 0;
            m_noise_filter_cap_voltage = 0;
            m_real_noise_bit_ff = 0;
            m_filtered_noise_bit_ff = 0;
            m_noise_gen_count = 0;
            m_attack_decay_cap_voltage = 0;
            m_rng = 0;
            m_mixer_a = 0;
            m_mixer_b = 0;
            m_mixer_c = 0;
            m_envelope_1 = 0;
            m_envelope_2 = 0;
            m_channel = null;
            m_our_sample_rate = 0;
            m_file = null;
        }


        public device_sound_interface_sn76477 disound { get { return m_disound; } }


        public void set_noise_params(double clock_res, double filter_res, double filter_cap)
        {
            m_noise_clock_res = clock_res;
            m_noise_filter_res = filter_res;
            m_noise_filter_cap = filter_cap;
        }
        public void set_decay_res(double decay_res) { m_decay_res = decay_res; }
        public void set_attack_params(double decay_cap, double res)
        {
            m_attack_decay_cap = decay_cap;
            m_attack_res = res;
        }
        public void set_amp_res(double amp_res) { m_amplitude_res = amp_res; }
        public void set_feedback_res(double feedback_res) { m_feedback_res = feedback_res; }
        public void set_vco_params(double volt, double cap, double res)
        {
            m_vco_voltage = volt;
            m_vco_cap = cap;
            m_vco_res = res;
        }
        public void set_pitch_voltage(double volt) { m_pitch_voltage = volt; }
        public void set_slf_params(double cap, double res)
        {
            m_slf_cap = cap;
            m_slf_res = res;
        }
        public void set_oneshot_params(double cap, double res)
        {
            m_one_shot_cap = cap;
            m_one_shot_res = res;
        }
        public void set_vco_mode(uint32_t mode) { m_vco_mode = mode; }
        public void set_mixer_params(uint32_t a, uint32_t b, uint32_t c)
        {
            m_mixer_a = a;
            m_mixer_b = b;
            m_mixer_c = c;
        }
        public void set_envelope_params(uint32_t env1, uint32_t env2)
        {
            m_envelope_1 = env1;
            m_envelope_2 = env2;
        }
        public void set_enable(uint32_t enable) { m_enable = enable; }


        /* these functions take 0 or 1 as a logic input */
        //WRITE_LINE_MEMBER( enable_w );      /* active LO, 0 = enabled, 1 = disabled */
        public void enable_w(int state)
        {
            CHECK_BOOLEAN(state);

            if (state != m_enable)
            {
                m_channel.update();

                m_enable = (uint32_t)state;

                    /* if falling edge */
                if (m_enable == 0)
                {
                    /* start the attack phase */
                    m_attack_decay_cap_voltage = AD_CAP_VOLTAGE_MIN;

                    /* one-shot runs regardless of envelope mode */
                    m_one_shot_running_ff = 1;
                }

                log_enable_line();
            }
        }


        //WRITE_LINE_MEMBER( mixer_a_w );
        public void mixer_a_w(int state)
        {
            CHECK_BOOLEAN(state);

            if (state != ((m_mixer_mode >> 0) & 0x01))
            {
                m_channel.update();

                m_mixer_mode = (m_mixer_mode & ~0x01U) | (uint32_t)state;

                log_mixer_mode();
            }
        }

        //WRITE_LINE_MEMBER( mixer_b_w );
        public void mixer_b_w(int state)
        {
            CHECK_BOOLEAN(state);

            if (state != ((m_mixer_mode >> 1) & 0x01))
            {
                m_channel.update();

                m_mixer_mode = (m_mixer_mode & ~0x02U) | ((uint32_t)state << 1);

                log_mixer_mode();
            }
        }

        //WRITE_LINE_MEMBER( mixer_c_w );
        public void mixer_c_w(int state)
        {
            CHECK_BOOLEAN(state);

            if (state != ((m_mixer_mode >> 2) & 0x01))
            {
                m_channel.update();

                m_mixer_mode = (m_mixer_mode & ~0x04U) | ((uint32_t)state << 2);

                log_mixer_mode();
            }
        }

        //WRITE_LINE_MEMBER( envelope_1_w );
        public void envelope_1_w(int state)
        {
            CHECK_BOOLEAN(state);

            if (state != ((m_envelope_mode >> 0) & 0x01))
            {
                m_channel.update();

                m_envelope_mode = (m_envelope_mode & ~0x01U) | (uint32_t)state;

                log_envelope_mode();
            }
        }

        //WRITE_LINE_MEMBER( envelope_2_w );
        public void envelope_2_w(int state)
        {
            CHECK_BOOLEAN(state);

            if (state != ((m_envelope_mode >> 1) & 0x01))
            {
                m_channel.update();

                m_envelope_mode = (m_envelope_mode & ~0x02U) | ((uint32_t)state << 1);

                log_envelope_mode();
            }
        }

        //WRITE_LINE_MEMBER( vco_w );         /* 0 = external, 1 = controlled by SLF */
        //WRITE_LINE_MEMBER( noise_clock_w ); /* noise clock write, if noise_clock_res = 0 */

        /* these functions take a resistor value in Ohms */
        //void one_shot_res_w(double data);
        //void slf_res_w(double data);
        //void vco_res_w(double data);
        //void noise_clock_res_w(double data);  /* = 0 if the noise gen is clocked via noise_clock */
        //void noise_filter_res_w(double data);
        //void decay_res_w(double data);
        //void attack_res_w(double data);
        //void amplitude_res_w(double data);
        //void feedback_res_w(double data);

        /* these functions take a capacitor value in Farads or the voltage on it in Volts */
        //static constexpr double EXTERNAL_VOLTAGE_DISCONNECT = -1.0;  /* indicates that the voltage is internally computed,
        //                                                           can be used in all the functions that take a
        //                                                           voltage on a capacitor */
        //void one_shot_cap_w(double data);
        //void one_shot_cap_voltage_w(double data);
        //void slf_cap_w(double data);
        //void slf_cap_voltage_w(double data);
        //void vco_cap_w(double data);
        //void vco_cap_voltage_w(double data);
        //void noise_filter_cap_w(double data);
        //void noise_filter_cap_voltage_w(double data);
        //void attack_decay_cap_w(double data);
        //void attack_decay_cap_voltage_w(double data);

        /* these functions take a voltage value in Volts */
        //void vco_voltage_w(double data);
        //void pitch_voltage_w(double data);


        // device-level overrides
        protected override void device_start()
        {
            m_channel = m_disound.stream_alloc(0, 1, (uint32_t)machine().sample_rate());

            if (clock() > 0)
            {
                m_our_sample_rate = (int)clock();
            }
            else
            {
                m_our_sample_rate = machine().sample_rate();
            }

            intialize_noise();

            // set up mixer and envelope modes, based on interface values
            mixer_a_w((int)m_mixer_a);
            mixer_b_w((int)m_mixer_b);
            mixer_c_w((int)m_mixer_c);
            envelope_1_w((int)m_envelope_1);
            envelope_2_w((int)m_envelope_2);

            m_one_shot_cap_voltage = ONE_SHOT_CAP_VOLTAGE_MIN;
            m_slf_cap_voltage = SLF_CAP_VOLTAGE_MIN;
            m_vco_cap_voltage = VCO_CAP_VOLTAGE_MIN;
            m_noise_filter_cap_voltage = NOISE_CAP_VOLTAGE_MIN;
            m_attack_decay_cap_voltage = AD_CAP_VOLTAGE_MIN;

            state_save_register();

            log_complete_state();

            if (LOG_WAV != 0)
                open_wav_file();
        }


        protected override void device_stop()
        {
            if (LOG_WAV != 0)
                close_wav_file();
        }


        // sound stream update overrides
        protected virtual void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            double one_shot_cap_charging_step;
            double one_shot_cap_discharging_step;
            double slf_cap_charging_step;
            double slf_cap_discharging_step;
            double vco_duty_cycle_multiplier;
            double vco_cap_charging_step;
            double vco_cap_discharging_step;
            double vco_cap_voltage_max;
            uint32_t noise_gen_freq;
            double noise_filter_cap_charging_step;
            double noise_filter_cap_discharging_step;
            double attack_decay_cap_charging_step;
            double attack_decay_cap_discharging_step;
            int    attack_decay_cap_charging;
            double voltage_out;
            double center_to_peak_voltage_out;

            var buffer = outputs[0];

            /* compute charging values, doing it here ensures that we always use the latest values */
            one_shot_cap_charging_step = compute_one_shot_cap_charging_rate() / m_our_sample_rate;
            one_shot_cap_discharging_step = compute_one_shot_cap_discharging_rate() / m_our_sample_rate;

            slf_cap_charging_step = compute_slf_cap_charging_rate() / m_our_sample_rate;
            slf_cap_discharging_step = compute_slf_cap_discharging_rate() / m_our_sample_rate;

            vco_duty_cycle_multiplier = (1 - compute_vco_duty_cycle()) * 2;
            vco_cap_charging_step = compute_vco_cap_charging_discharging_rate() / vco_duty_cycle_multiplier / m_our_sample_rate;
            vco_cap_discharging_step = compute_vco_cap_charging_discharging_rate() * vco_duty_cycle_multiplier / m_our_sample_rate;

            noise_filter_cap_charging_step = compute_noise_filter_cap_charging_rate() / m_our_sample_rate;
            noise_filter_cap_discharging_step = compute_noise_filter_cap_discharging_rate() / m_our_sample_rate;
            noise_gen_freq = compute_noise_gen_freq();

            attack_decay_cap_charging_step = compute_attack_decay_cap_charging_rate() / m_our_sample_rate;
            attack_decay_cap_discharging_step = compute_attack_decay_cap_discharging_rate() / m_our_sample_rate;

            center_to_peak_voltage_out = compute_center_to_peak_voltage_out();


            /* process 'samples' number of samples */
            for (int sampindex = 0; sampindex < buffer.samples(); sampindex++)
            {
                /* update the one-shot cap voltage */
                if (m_one_shot_cap_voltage_ext == 0)
                {
                    if (m_one_shot_running_ff != 0)
                    {
                        /* charging */
                        m_one_shot_cap_voltage = min(m_one_shot_cap_voltage + one_shot_cap_charging_step, ONE_SHOT_CAP_VOLTAGE_MAX);
                    }
                    else
                    {
                        /* discharging */
                        m_one_shot_cap_voltage = max(m_one_shot_cap_voltage - one_shot_cap_discharging_step, ONE_SHOT_CAP_VOLTAGE_MIN);
                    }
                }

                if (m_one_shot_cap_voltage >= ONE_SHOT_CAP_VOLTAGE_MAX)
                {
                    m_one_shot_running_ff = 0;
                }


                /* update the SLF (super low frequency oscillator) */
                if (m_slf_cap_voltage_ext == 0)
                {
                    /* internal */
                    if (m_slf_out_ff == 0)
                    {
                        /* charging */
                        m_slf_cap_voltage = min(m_slf_cap_voltage + slf_cap_charging_step, SLF_CAP_VOLTAGE_MAX);
                    }
                    else
                    {
                        /* discharging */
                        m_slf_cap_voltage = max(m_slf_cap_voltage - slf_cap_discharging_step, SLF_CAP_VOLTAGE_MIN);
                    }
                }

                if (m_slf_cap_voltage >= SLF_CAP_VOLTAGE_MAX)
                {
                    m_slf_out_ff = 1;
                }
                else if (m_slf_cap_voltage <= SLF_CAP_VOLTAGE_MIN)
                {
                    m_slf_out_ff = 0;
                }


                /* update the VCO (voltage controlled oscillator) */
                if (m_vco_mode != 0)
                {
                    /* VCO is controlled by SLF */
                    vco_cap_voltage_max =  m_slf_cap_voltage + VCO_TO_SLF_VOLTAGE_DIFF;
                }
                else
                {
                    /* VCO is controlled by external voltage */
                    vco_cap_voltage_max = m_vco_voltage + VCO_TO_SLF_VOLTAGE_DIFF;
                }

                if (m_vco_cap_voltage_ext == 0)
                {
                    if (m_vco_out_ff == 0)
                    {
                        /* charging */
                        m_vco_cap_voltage = min(m_vco_cap_voltage + vco_cap_charging_step, vco_cap_voltage_max);
                    }
                    else
                    {
                        /* discharging */
                        m_vco_cap_voltage = max(m_vco_cap_voltage - vco_cap_discharging_step, VCO_CAP_VOLTAGE_MIN);
                    }
                }

                if (m_vco_cap_voltage >= vco_cap_voltage_max)
                {
                    if (m_vco_out_ff == 0)
                    {
                        /* positive edge */
                        m_vco_alt_pos_edge_ff = m_vco_alt_pos_edge_ff == 0 ? 1U : 0U;
                    }

                    m_vco_out_ff = 1;
                }
                else if (m_vco_cap_voltage <= VCO_CAP_VOLTAGE_MIN)
                {
                    m_vco_out_ff = 0;
                }


                /* update the noise generator */
                while (m_noise_clock_ext == 0 && (m_noise_gen_count <= noise_gen_freq))
                {
                    m_noise_gen_count = m_noise_gen_count + (uint32_t)m_our_sample_rate;

                    m_real_noise_bit_ff = generate_next_real_noise_bit();
                }

                m_noise_gen_count = m_noise_gen_count - noise_gen_freq;


                /* update the noise filter */
                if (m_noise_filter_cap_voltage_ext == 0)
                {
                    /* internal */
                    if (m_real_noise_bit_ff != 0)
                    {
                        /* charging */
                        m_noise_filter_cap_voltage = min(m_noise_filter_cap_voltage + noise_filter_cap_charging_step, NOISE_CAP_VOLTAGE_MAX);
                    }
                    else
                    {
                        /* discharging */
                        m_noise_filter_cap_voltage = max(m_noise_filter_cap_voltage - noise_filter_cap_discharging_step, NOISE_CAP_VOLTAGE_MIN);
                    }
                }

                /* check the thresholds */
                if (m_noise_filter_cap_voltage >= NOISE_CAP_HIGH_THRESHOLD)
                {
                    m_filtered_noise_bit_ff = 0;
                }
                else if (m_noise_filter_cap_voltage <= NOISE_CAP_LOW_THRESHOLD)
                {
                    m_filtered_noise_bit_ff = 1;
                }


                /* based on the envelope mode figure out the attack/decay phase we are in */
                switch (m_envelope_mode)
                {
                case 0:     /* VCO */
                    attack_decay_cap_charging = (int)m_vco_out_ff;
                    break;

                case 1:     /* one-shot */
                    attack_decay_cap_charging = (int)m_one_shot_running_ff;
                    break;

                case 2:
                default:    /* mixer only */
                    attack_decay_cap_charging = 1;  /* never a decay phase */
                    break;

                case 3:     /* VCO with alternating polarity */
                    attack_decay_cap_charging = (m_vco_out_ff != 0 && m_vco_alt_pos_edge_ff != 0) ? 1 : 0;
                    break;
                }


                /* update a/d cap voltage */
                if (m_attack_decay_cap_voltage_ext == 0)
                {
                    if (attack_decay_cap_charging != 0)
                    {
                        if (attack_decay_cap_charging_step > 0)
                        {
                            m_attack_decay_cap_voltage = min(m_attack_decay_cap_voltage + attack_decay_cap_charging_step, AD_CAP_VOLTAGE_MAX);
                        }
                        else
                        {
                            /* no attack, voltage to max instantly */
                            m_attack_decay_cap_voltage = AD_CAP_VOLTAGE_MAX;
                        }
                    }
                    else
                    {
                        /* discharging */
                        if (attack_decay_cap_discharging_step > 0)
                        {
                            m_attack_decay_cap_voltage = max(m_attack_decay_cap_voltage - attack_decay_cap_discharging_step, AD_CAP_VOLTAGE_MIN);
                        }
                        else
                        {
                            /* no decay, voltage to min instantly */
                            m_attack_decay_cap_voltage = AD_CAP_VOLTAGE_MIN;
                        }
                    }
                }


                /* mix the output, if enabled, or not saturated by the VCO */
                if (m_enable == 0 && (m_vco_cap_voltage <= VCO_CAP_VOLTAGE_MAX))
                {
                    uint32_t out_;

                    /* enabled */
                    switch (m_mixer_mode)
                    {
                    case 0:     /* VCO */
                        out_ = m_vco_out_ff;
                        break;

                    case 1:     /* SLF */
                        out_ = m_slf_out_ff;
                        break;

                    case 2:     /* noise */
                        out_ = m_filtered_noise_bit_ff;
                        break;

                    case 3:     /* VCO and noise */
                        out_ = m_vco_out_ff & m_filtered_noise_bit_ff;
                        break;

                    case 4:     /* SLF and noise */
                        out_ = m_slf_out_ff & m_filtered_noise_bit_ff;
                        break;

                    case 5:     /* VCO, SLF and noise */
                        out_ = m_vco_out_ff & m_slf_out_ff & m_filtered_noise_bit_ff;
                        break;

                    case 6:     /* VCO and SLF */
                        out_ = m_vco_out_ff & m_slf_out_ff;
                        break;

                    case 7:     /* inhibit */
                    default:
                        out_ = 0;
                        break;
                    }

                    /* determine the OUT voltage from the attack/delay cap voltage and clip it */
                    if (out_ != 0)
                    {
                        voltage_out = OUT_CENTER_LEVEL_VOLTAGE + center_to_peak_voltage_out * out_pos_gain[(int)(m_attack_decay_cap_voltage * 10)];
                        voltage_out = min(voltage_out, OUT_HIGH_CLIP_THRESHOLD);
                    }
                    else
                    {
                        voltage_out = OUT_CENTER_LEVEL_VOLTAGE + center_to_peak_voltage_out * out_neg_gain[(int)(m_attack_decay_cap_voltage * 10)];
                        voltage_out = max(voltage_out, OUT_LOW_CLIP_THRESHOLD);
                    }
                }
                else
                {
                    /* disabled */
                    voltage_out = OUT_CENTER_LEVEL_VOLTAGE;
                }


                /* convert it to a signed 16-bit sample,
                   -32767 = OUT_LOW_CLIP_THRESHOLD
                        0 = OUT_CENTER_LEVEL_VOLTAGE
                    32767 = 2 * OUT_CENTER_LEVEL_VOLTAGE + OUT_LOW_CLIP_THRESHOLD

                              / Vout - Vmin    \
                    sample = |  ----------- - 1 |
                              \ Vcen - Vmin    /
                 */
                buffer.put(sampindex, (stream_buffer_sample_t)(((voltage_out - OUT_LOW_CLIP_THRESHOLD) / (OUT_CENTER_LEVEL_VOLTAGE - OUT_LOW_CLIP_THRESHOLD)) - 1));

                if (LOG_WAV != 0 && (m_enable == 0 || LOG_WAV_ENABLED_ONLY == 0))
                {
                    int16_t log_data_l;
                    int16_t log_data_r;

                    switch (LOG_WAV_VALUE_L)
                    {
                    case 0:
                        log_data_l = (int16_t)(LOG_WAV_GAIN_FACTOR * voltage_out);
                        break;
                    case 1:
                        log_data_l = (int16_t)(LOG_WAV_GAIN_FACTOR * m_enable);
                        break;
                    case 2:
                        log_data_l = (int16_t)(LOG_WAV_GAIN_FACTOR * m_one_shot_cap_voltage);
                        break;
                    case 3:
                        log_data_l = (int16_t)(LOG_WAV_GAIN_FACTOR * m_attack_decay_cap_voltage);
                        break;
                    case 4:
                        log_data_l = (int16_t)(LOG_WAV_GAIN_FACTOR * m_slf_cap_voltage);
                        break;
                    case 5:
                        log_data_l = (int16_t)(LOG_WAV_GAIN_FACTOR * m_vco_cap_voltage);
                        break;
                    case 6:
                        log_data_l = (int16_t)(LOG_WAV_GAIN_FACTOR * m_noise_filter_cap_voltage);
                        break;
                    }

                    switch (LOG_WAV_VALUE_R)
                    {
                    case 0:
                        log_data_r = (int16_t)(LOG_WAV_GAIN_FACTOR * voltage_out);
                        break;
                    case 1:
                        log_data_r = (int16_t)(LOG_WAV_GAIN_FACTOR * m_enable);
                        break;
                    case 2:
                        log_data_r = (int16_t)(LOG_WAV_GAIN_FACTOR * m_one_shot_cap_voltage);
                        break;
                    case 3:
                        log_data_r = (int16_t)(LOG_WAV_GAIN_FACTOR * m_attack_decay_cap_voltage);
                        break;
                    case 4:
                        log_data_r = (int16_t)(LOG_WAV_GAIN_FACTOR * m_slf_cap_voltage);
                        break;
                    case 5:
                        log_data_r = (int16_t)(LOG_WAV_GAIN_FACTOR * m_vco_cap_voltage);
                        break;
                    case 6:
                        log_data_r = (int16_t)(LOG_WAV_GAIN_FACTOR * m_noise_filter_cap_voltage);
                        break;
                    }

                    add_wav_data(log_data_l, log_data_r);
                }
            }
        }


        double compute_one_shot_cap_charging_rate() /* in V/sec */
        {
            /* this formula was derived using the data points below

             Res (kohms)  Cap (uF)   Time (millisec)
                 47         0.33         11.84
                 47         1.0          36.2
                 47         1.5          52.1
                 47         2.0          76.4
                100         0.33         24.4
                100         1.0          75.2
                100         1.5         108.5
                100         2.0         158.4
            */

            double ret = 0;

            if ((m_one_shot_res > 0) && (m_one_shot_cap > 0))
            {
                ret = ONE_SHOT_CAP_VOLTAGE_RANGE / (0.8024 * m_one_shot_res * m_one_shot_cap + 0.002079);
            }
            else if (m_one_shot_cap > 0)
            {
                /* if no resistor, there is no current to charge the cap,
                   effectively making the one-shot time effectively infinite */
                ret = +1e-30;
            }
            else if (m_one_shot_res > 0)
            {
                /* if no cap, the voltage changes extremely fast,
                   effectively making the one-shot time 0 */
                ret = +1e+30;
            }

            return ret;
        }


        double compute_one_shot_cap_discharging_rate()
        {
            /* this formula was derived using the data points below

            Cap (uF)   Time (microsec)
              0.33           300
              1.0            850
              1.5           1300
              2.0           1900
            */

            double ret = 0;

            if ((m_one_shot_res > 0) && (m_one_shot_cap > 0))
            {
                ret = ONE_SHOT_CAP_VOLTAGE_RANGE / (854.7 * m_one_shot_cap + 0.00001795);
            }
            else if (m_one_shot_res > 0)
            {
                /* if no cap, the voltage changes extremely fast,
                   effectively making the one-shot time 0 */
                ret = +1e+30;
            }

            return ret;
        }


        double compute_slf_cap_charging_rate() /* in V/sec */
        {
            /* this formula was derived using the data points below

            Res (kohms)  Cap (uF)   Time (millisec)
                 47        0.47          14.3
                120        0.47          35.6
                200        0.47          59.2
                 47        1.00          28.6
                120        1.00          71.6
                200        1.00         119.0
            */
            double ret = 0;

            if ((m_slf_res > 0) && (m_slf_cap > 0))
            {
                ret = SLF_CAP_VOLTAGE_RANGE / (0.5885 * m_slf_res * m_slf_cap + 0.001300);
            }

            return ret;
        }


        double compute_slf_cap_discharging_rate() /* in V/sec */
        {
            /* this formula was derived using the data points below

            Res (kohms)  Cap (uF)   Time (millisec)
                 47        0.47          13.32
                120        0.47          32.92
                200        0.47          54.4
                 47        1.00          26.68
                120        1.00          66.2
                200        1.00         109.6
            */
            double ret = 0;

            if ((m_slf_res > 0) && (m_slf_cap > 0))
            {
                ret = SLF_CAP_VOLTAGE_RANGE / (0.5413 * m_slf_res * m_slf_cap + 0.001343);
            }

            return ret;
        }


        double compute_vco_cap_charging_discharging_rate() /* in V/sec */
        {
            double ret = 0;

            if ((m_vco_res > 0) && (m_vco_cap > 0))
            {
                ret = 0.64 * 2 * VCO_CAP_VOLTAGE_RANGE / (m_vco_res * m_vco_cap);
            }

            return ret;
        }


        double compute_vco_duty_cycle() /* no measure, just a number */
        {
            double ret = 0.5;   /* 50% */

            if ((m_vco_voltage > 0) && (m_pitch_voltage != VCO_DUTY_CYCLE_50))
            {
                ret = max(0.5 * (m_pitch_voltage / m_vco_voltage), (VCO_MIN_DUTY_CYCLE / 100.0));

                ret = min(ret, 1);
            }

            return ret;
        }


        uint32_t compute_noise_gen_freq() /* in Hz */
        {
            /* this formula was derived using the data points below

             Res (ohms)   Freq (Hz)
                10k         97493
                12k         83333
                15k         68493
                22k         49164
                27k         41166
                33k         34449
                36k         31969
                47k         25126
                56k         21322
                68k         17721.5
                82k         15089.2
                100k        12712.0
                150k         8746.4
                220k         6122.4
                270k         5101.5
                330k         4217.2
                390k         3614.5
                470k         3081.7
                680k         2132.7
                820k         1801.8
                  1M         1459.9
                2.2M          705.13
                3.3M          487.59
            */

            uint32_t ret = 0;

            if ((m_noise_clock_res >= NOISE_MIN_CLOCK_RES) &&
                (m_noise_clock_res <= NOISE_MAX_CLOCK_RES))
            {
                ret = (uint32_t)(339100000 * pow(m_noise_clock_res, -0.8849));
            }

            return ret;
        }


        double compute_noise_filter_cap_charging_rate() /* in V/sec */
        {
            /* this formula was derived using the data points below

             R*C        Time (sec)
            .000068     .0000184
            .0001496    .0000378
            .0002244    .0000548
            .0003196    .000077
            .0015       .000248
            .0033       .000540
            .00495      .000792
            .00705      .001096
            */

            double ret = 0;

            if ((m_noise_filter_res > 0) && (m_noise_filter_cap > 0))
            {
                ret = NOISE_CAP_VOLTAGE_RANGE / (0.1571 * m_noise_filter_res * m_noise_filter_cap + 0.00001430);
            }
            else if (m_noise_filter_cap > 0)
            {
                /* if no resistor, there is no current to charge the cap,
                   effectively making the filter's output constants */
                ret = +1e-30;
            }
            else if (m_noise_filter_res > 0)
            {
                /* if no cap, the voltage changes extremely fast,
                   effectively disabling the filter */
                ret = +1e+30;
            }

            return ret;
        }


        double compute_noise_filter_cap_discharging_rate()
        {
            /* this formula was derived using the data points below

             R*C        Time (sec)
            .000068     .000016
            .0001496    .0000322
            .0002244    .0000472
            .0003196    .0000654
            .0015       .000219
            .0033       .000468
            .00495      .000676
            .00705      .000948
            */

            double ret = 0;

            if ((m_noise_filter_res > 0) && (m_noise_filter_cap > 0))
            {
                ret = NOISE_CAP_VOLTAGE_RANGE / (0.1331 * m_noise_filter_res * m_noise_filter_cap + 0.00001734);
            }
            else if (m_noise_filter_cap > 0)
            {
                /* if no resistor, there is no current to charge the cap,
                   effectively making the filter's output constants */
                ret = +1e-30;
            }
            else if (m_noise_filter_res > 0)
            {
                /* if no cap, the voltage changes extremely fast,
                   effectively disabling the filter */
                ret = +1e+30;
            }

            return ret;
        }


        double compute_attack_decay_cap_charging_rate()  /* in V/sec */
        {
            double ret = 0;

            if ((m_attack_res > 0) && (m_attack_decay_cap > 0))
            {
                ret = AD_CAP_VOLTAGE_RANGE / (m_attack_res * m_attack_decay_cap);
            }
            else if (m_attack_decay_cap > 0)
            {
                /* if no resistor, there is no current to charge the cap,
                   effectively making the attack time infinite */
                ret = +1e-30;
            }
            else if (m_attack_res > 0)
            {
                /* if no cap, the voltage changes extremely fast,
                   effectively making the attack time 0 */
                ret = +1e+30;
            }

            return ret;
        }


        double compute_attack_decay_cap_discharging_rate()  /* in V/sec */
        {
            double ret = 0;

            if ((m_decay_res > 0) && (m_attack_decay_cap > 0))
            {
                ret = AD_CAP_VOLTAGE_RANGE / (m_decay_res * m_attack_decay_cap);
            }
            else if (m_attack_decay_cap > 0)
            {
                /* if no resistor, there is no current to charge the cap,
                   effectively making the decay time infinite */
                ret = +1e-30;
            }
            else if (m_attack_res > 0)
            {
                /* if no cap, the voltage changes extremely fast,
                   effectively making the decay time 0 */
                ret = +1e+30;
            }

            return ret;
        }


        double compute_center_to_peak_voltage_out()
        {
            /* this formula was derived using the data points below

             Ra (kohms)  Rf (kohms)   Voltage
                150         47          1.28
                200         47          0.96
                 47         22          1.8
                100         22          0.87
                150         22          0.6
                200         22          0.45
                 47         10          0.81
                100         10          0.4
                150         10          0.27
            */

            double ret = 0;

            if (m_amplitude_res > 0)
            {
                ret = 3.818 * (m_feedback_res / m_amplitude_res) + 0.03;
            }

            return ret;
        }


        void log_enable_line()
        {
            string [] desc =
            {
                "Enabled", "Inhibited"
            };

            LOG(1, "SN76477:              Enable line (9): {0} [{1}]\n", m_enable, desc[m_enable]);
        }

        void log_mixer_mode()
        {
            string [] desc =
            {
                "VCO", "SLF", "Noise", "VCO/Noise",
                "SLF/Noise", "SLF/VCO/Noise", "SLF/VCO", "Inhibit"
            };

            LOG(1, "SN76477:           Mixer mode (25-27): {0} [{1}]\n", m_mixer_mode, desc[m_mixer_mode]);
        }

        void log_envelope_mode()
        {
            string [] desc =
            {
                "VCO", "One-Shot", "Mixer Only", "VCO with Alternating Polarity"
            };

            LOG(1, "SN76477:         Envelope mode (1,28): {0} [{1}]\n", m_envelope_mode, desc[m_envelope_mode]);
        }

        void log_vco_mode()
        {
            string [] desc =
            {
                "External (Pin 16)", "Internal (SLF)"
            };

            LOG(1, "SN76477:                VCO mode (22): {0} [{1}]\n", m_vco_mode, desc[m_vco_mode]);
        }

        void log_one_shot_time()
        {
            if (m_one_shot_cap_voltage_ext == 0)
            {
                if (compute_one_shot_cap_charging_rate() > 0)
                {
                    LOG(1, "SN76477:        One-shot time (23,24): %{0} sec\n", ONE_SHOT_CAP_VOLTAGE_RANGE * (1 / compute_one_shot_cap_charging_rate()));
                }
                else
                {
                    LOG(1, "SN76477:        One-shot time (23,24): N/A\n");
                }
            }
            else
            {
                LOG(1, "SN76477:        One-shot time (23,24): External (cap = {0}V)\n", m_one_shot_cap_voltage);
            }
        }

        void log_slf_freq()
        {
            if (m_slf_cap_voltage_ext == 0)
            {
                if (compute_slf_cap_charging_rate() > 0)
                {
                    double charging_time = (1 / compute_slf_cap_charging_rate()) * SLF_CAP_VOLTAGE_RANGE;
                    double discharging_time = (1 / compute_slf_cap_discharging_rate()) * SLF_CAP_VOLTAGE_RANGE;

                    LOG(1, "SN76477:        SLF frequency (20,21): {0} Hz\n", 1 / (charging_time + discharging_time));
                }
                else
                {
                    LOG(1, "SN76477:        SLF frequency (20,21): N/A\n");
                }
            }
            else
            {
                LOG(1, "SN76477:        SLF frequency (20,21): External (cap = {0}V)\n", m_slf_cap_voltage);
            }
        }

        void log_vco_pitch_voltage()
        {
            LOG(1, "SN76477:       VCO pitch voltage (19): {0}V\n", m_pitch_voltage);
        }

        void log_vco_duty_cycle()
        {
            LOG(1, "SN76477:       VCO duty cycle (16,19): {0}%%\n", compute_vco_duty_cycle() * 100.0);
        }

        void log_vco_freq()
        {
            if (m_vco_cap_voltage_ext == 0)
            {
                if (compute_vco_cap_charging_discharging_rate() > 0)
                {
                    double min_freq = compute_vco_cap_charging_discharging_rate() / (2 * VCO_CAP_VOLTAGE_RANGE);
                    double max_freq = compute_vco_cap_charging_discharging_rate() / (2 * VCO_TO_SLF_VOLTAGE_DIFF);

                    LOG(1, "SN76477:        VCO frequency (17,18): {0} Hz - {1} Hz\n", min_freq, max_freq);
                }
                else
                {
                    LOG(1, "SN76477:        VCO frequency (17,18): N/A\n");
                }
            }
            else
            {
                LOG(1, "SN76477:        VCO frequency (17,18): External (cap = {0}V)\n", m_vco_cap_voltage);
            }
        }

        void log_vco_ext_voltage()
        {
            if (m_vco_voltage <= VCO_MAX_EXT_VOLTAGE)
            {
                double min_freq = compute_vco_cap_charging_discharging_rate() / (2 * VCO_CAP_VOLTAGE_RANGE);
                double max_freq = compute_vco_cap_charging_discharging_rate() / (2 * VCO_TO_SLF_VOLTAGE_DIFF);

                LOG(1, "SN76477:        VCO ext. voltage (16): {0}V ({1} Hz)\n",
                        m_vco_voltage,
                        min_freq + ((max_freq - min_freq) * m_vco_voltage / VCO_MAX_EXT_VOLTAGE));
            }
            else
            {
                LOG(1, "SN76477:        VCO ext. voltage (16): {0}V (saturated, no output)\n", m_vco_voltage);
            }
        }

        void log_noise_gen_freq()
        {
            if (m_noise_clock_ext != 0)
            {
                LOG(1, "SN76477:      Noise gen frequency (4): External\n");
            }
            else
            {
                if (compute_noise_gen_freq() > 0)
                {
                    LOG(1, "SN76477:      Noise gen frequency (4): {0} Hz\n", compute_noise_gen_freq());
                }
                else
                {
                    LOG(1, "SN76477:      Noise gen frequency (4): N/A\n");
                }
            }
        }

        void log_noise_filter_freq()
        {
            if (m_noise_filter_cap_voltage_ext == 0)
            {
                double charging_rate = compute_noise_filter_cap_charging_rate();

                if (charging_rate > 0)
                {
                    if (charging_rate < 1000000.0)
                    {
                        double charging_time = (1 / charging_rate) * NOISE_CAP_VOLTAGE_RANGE;
                        double discharging_time = (1 / charging_rate) * NOISE_CAP_VOLTAGE_RANGE;

                        LOG(1, "SN76477: Noise filter frequency (5,6): {0} Hz\n", 1 / (charging_time + discharging_time));
                    }
                    else
                    {
                        LOG(1, "SN76477: Noise filter frequency (5,6): Very Large (Filtering Disabled)\n");
                    }
                }
                else
                {
                    LOG(1, "SN76477: Noise filter frequency (5,6): N/A\n");
                }
            }
            else
            {
                LOG(1, "SN76477: Noise filter frequency (5,6): External (cap = {0}V)\n", m_noise_filter_cap);
            }
        }

        void log_attack_time()
        {
            if (m_attack_decay_cap_voltage_ext == 0)
            {
                if (compute_attack_decay_cap_charging_rate() > 0)
                {
                    LOG(1, "SN76477:           Attack time (8,10): {0} sec\n", AD_CAP_VOLTAGE_RANGE * (1 / compute_attack_decay_cap_charging_rate()));
                }
                else
                {
                    LOG(1, "SN76477:           Attack time (8,10): N/A\n");
                }
            }
            else
            {
                LOG(1, "SN76477:           Attack time (8,10): External (cap = {0}V)\n", m_attack_decay_cap_voltage);
            }
        }

        void log_decay_time()
        {
            if (m_attack_decay_cap_voltage_ext == 0)
            {
                if (compute_attack_decay_cap_discharging_rate() > 0)
                {
                    LOG(1, "SN76477:             Decay time (7,8): {0} sec\n", AD_CAP_VOLTAGE_RANGE * (1 / compute_attack_decay_cap_discharging_rate()));
                }
                else
                {
                    LOG(1, "SN76477:            Decay time (8,10): N/A\n");
                }
            }
            else
            {
                LOG(1, "SN76477:             Decay time (7, 8): External (cap = {0}V)\n", m_attack_decay_cap_voltage);
            }
        }

        void log_voltage_out()
        {
            LOG(1, "SN76477:    Voltage OUT range (11,12): {0}V - {1}V (clips above {2}V)\n",
                    OUT_CENTER_LEVEL_VOLTAGE + compute_center_to_peak_voltage_out() * out_neg_gain[(int)(AD_CAP_VOLTAGE_MAX * 10)],
                    OUT_CENTER_LEVEL_VOLTAGE + compute_center_to_peak_voltage_out() * out_pos_gain[(int)(AD_CAP_VOLTAGE_MAX * 10)],
                    OUT_HIGH_CLIP_THRESHOLD);
        }


        void log_complete_state()
        {
            log_enable_line();
            log_mixer_mode();
            log_envelope_mode();
            log_vco_mode();
            log_one_shot_time();
            log_slf_freq();
            log_vco_freq();
            log_vco_ext_voltage();
            log_vco_pitch_voltage();
            log_vco_duty_cycle();
            log_noise_filter_freq();
            log_noise_gen_freq();
            log_attack_time();
            log_decay_time();
            log_voltage_out();
        }


        void open_wav_file() { throw new emu_unimplemented(); }
        void close_wav_file() { throw new emu_unimplemented(); }
        void add_wav_data(int16_t data_l, int16_t data_r) { throw new emu_unimplemented(); }


        void intialize_noise()
        {
            m_rng = 0;
        }


        uint32_t generate_next_real_noise_bit()
        {
            uint32_t out_ = ((m_rng >> 28) & 1) ^ ((m_rng >> 0) & 1);

                /* if bits 0-4 and 28 are all zero then force the output to 1 */
            if ((m_rng & 0x1000001f) == 0)
            {
                out_ = 1;
            }

            m_rng = (m_rng >> 1) | (out_ << 30);

            return out_;
        }


        void state_save_register()
        {
            save_item(NAME(new { m_enable }));
            save_item(NAME(new { m_envelope_mode }));
            save_item(NAME(new { m_vco_mode }));
            save_item(NAME(new { m_mixer_mode }));

            save_item(NAME(new { m_one_shot_res }));
            save_item(NAME(new { m_one_shot_cap }));
            save_item(NAME(new { m_one_shot_cap_voltage_ext }));

            save_item(NAME(new { m_slf_res }));
            save_item(NAME(new { m_slf_cap }));
            save_item(NAME(new { m_slf_cap_voltage_ext }));

            save_item(NAME(new { m_vco_voltage }));
            save_item(NAME(new { m_vco_res }));
            save_item(NAME(new { m_vco_cap }));
            save_item(NAME(new { m_vco_cap_voltage_ext }));

            save_item(NAME(new { m_noise_clock_res }));
            save_item(NAME(new { m_noise_clock_ext }));
            save_item(NAME(new { m_noise_clock }));
            save_item(NAME(new { m_noise_filter_res }));
            save_item(NAME(new { m_noise_filter_cap }));
            save_item(NAME(new { m_noise_filter_cap_voltage_ext }));

            save_item(NAME(new { m_attack_res }));
            save_item(NAME(new { m_decay_res }));
            save_item(NAME(new { m_attack_decay_cap }));
            save_item(NAME(new { m_attack_decay_cap_voltage_ext }));

            save_item(NAME(new { m_amplitude_res }));
            save_item(NAME(new { m_feedback_res }));
            save_item(NAME(new { m_pitch_voltage }));

            save_item(NAME(new { m_one_shot_cap_voltage }));
            save_item(NAME(new { m_one_shot_running_ff }));

            save_item(NAME(new { m_slf_cap_voltage }));
            save_item(NAME(new { m_slf_out_ff }));

            save_item(NAME(new { m_vco_cap_voltage }));
            save_item(NAME(new { m_vco_out_ff }));
            save_item(NAME(new { m_vco_alt_pos_edge_ff }));

            save_item(NAME(new { m_noise_filter_cap_voltage }));
            save_item(NAME(new { m_real_noise_bit_ff }));
            save_item(NAME(new { m_filtered_noise_bit_ff }));
            save_item(NAME(new { m_noise_gen_count }));

            save_item(NAME(new { m_attack_decay_cap_voltage }));

            save_item(NAME(new { m_rng }));
        }
    }


    static class sn76477_global
    {
        public static sn76477_device SN76477<bool_Required>(machine_config mconfig, device_finder<sn76477_device, bool_Required> finder) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, sn76477_device.SN76477, 0); }
    }
}
