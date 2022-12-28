// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using stream_buffer_sample_t = System.Single;  //using sample_t = float;
using uint32_t = System.UInt32;

using static mame.cpp_global;
using static mame.device_global;
using static mame.emucore_global;
using static mame.flt_biquad_global;
using static mame.flt_biquad_internal;
using static mame.sound_global;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> filter_biquad_device
    public class filter_biquad_device : device_t
                                        //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(FILTER_BIQUAD, filter_biquad_device, "filter_biquad", "Biquad Filter")
        public static readonly emu.detail.device_type_impl FILTER_BIQUAD = DEFINE_DEVICE_TYPE("filter_biquad", "Biquad Filter", (type, mconfig, tag, owner, clock) => { return new filter_biquad_device(mconfig, tag, owner, clock); });


        public class device_sound_interface_filter_biquad : device_sound_interface
        {
            public device_sound_interface_filter_biquad(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((filter_biquad_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        enum biquad_type : int
        {
            LOWPASS1P = 0,
            HIGHPASS1P,
            LOWPASS,
            HIGHPASS,
            BANDPASS,
            NOTCH,
            PEAK,
            LOWSHELF,
            HIGHSHELF,
            RAWPARAMS
        }


        struct biquad_params
        {
            public biquad_type type;
            public double fc;
            public double q;
            public double gain;
        }


        device_sound_interface_filter_biquad m_disound;


        sound_stream m_stream;
        biquad_type m_type;
        int m_last_sample_rate;
        double m_fc;
        double m_q;
        double m_gain;

        stream_buffer_sample_t m_input;
        double m_w0;
        double m_w1;
        double m_w2; /* w[k], w[k-1], w[k-2], current and previous intermediate values */
        stream_buffer_sample_t m_output;
        double m_a1;
        double m_a2; /* digital filter coefficients, denominator */
        double m_b0;
        double m_b1;
        double m_b2;  /* digital filter coefficients, numerator */


        filter_biquad_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, FILTER_BIQUAD, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_filter_biquad(mconfig, this));  //device_sound_interface(mconfig, *this),
            m_disound = GetClassInterface<device_sound_interface_filter_biquad>();


            m_stream = null;
            m_type = biquad_type.HIGHPASS;
            m_last_sample_rate = 0;
            m_fc = 16.0;
            m_q = M_SQRT2 / 2.0;
            m_gain = 1.0;
            m_input = 0;
            m_w0 = 0.0;
            m_w1 = 0.0;
            m_w2 = 0.0;
            m_output = 0;
            m_a1 = 0.0;
            m_a2 = 0.0;
            m_b0 = 1.0;
            m_b1 = 0.0;
            m_b2 = 0.0;
        }


        public device_sound_interface_filter_biquad disound { get { return m_disound; } }


        // set up the filter with the specified filter parameters and return a pointer to the new device

        //filter_biquad_device& setup(biquad_type type, double fc, double q, double gain);

        filter_biquad_device setup(biquad_params p)
        {
            m_type = p.type;
            m_fc = p.fc;
            m_q = p.q;
            m_gain = p.gain;
            return this;
        }


        // modify an existing instance with new filter parameters
        //void modify(biquad_type type, double fc, double q, double gain);
        //void modify(biquad_params p);

        // set up the filter with raw biquad coefficients
        //filter_biquad_device& setup_raw(double a1, double a2, double b0, double b1, double b2);
        //void modify_raw(double a1, double a2, double b0, double b1, double b2);

        // Helper setup functions to create common filters representable by biquad filters:
        // (and, as needed, modify/update/recalc helpers)

        // Sallen-Key low-pass
        //filter_biquad_device& opamp_sk_lowpass_setup(double r1, double r2, double r3, double r4, double c1, double c2);
        //void opamp_sk_lowpass_modify(double r1, double r2, double r3, double r4, double c1, double c2);
        //biquad_params opamp_sk_lowpass_calc(double r1, double r2, double r3, double r4, double c1, double c2);

        // TODO when needed: Sallen-Key band-pass

        // TODO when needed: Sallen-Key band-reject

        // TODO when needed: Sallen-Key high-pass

        // Multiple-Feedback low-pass
        public filter_biquad_device opamp_mfb_lowpass_setup(double r1, double r2, double r3, double c1, double c2)
        {
            filter_biquad_device.biquad_params p = opamp_mfb_lowpass_calc(r1, r2, r3, c1, c2);
            return setup(p);
        }


        //void opamp_mfb_lowpass_modify(double r1, double r2, double r3, double c1, double c2);


        biquad_params opamp_mfb_lowpass_calc(double r1, double r2, double r3, double c1, double c2)
        {
            filter_biquad_device.biquad_params r = new filter_biquad_device.biquad_params();

            if ((r1 == 0) || ((r2 == 0) && (c1 != 0)) || (r3 == 0) || (c2 == 0))
            {
                fatalerror("filter_biquad_device::opamp_mfb_lowpass_calc() - only c1 can be 0 (and if c1 is 0, r2 can also be 0); parameters were: r1: {0}, r2: {1}, r3: {2}, c1: {3}, c2: {4}", r1, r2, r3, c1, c2); /* Filter can not be setup.  Undefined results. */
            }

            r.gain = -r3 / r1;
            r.q = (M_SQRT2 / 2.0);

            if (c1 == 0) // if both R2 and C1 are 0, it is the 'proper' first order case. If C1 is 0 (Williams...) the filter is 1st order. There do exist some unusual filters where R2 is not 0, though. In both cases this yields a single-pole filter with limited configurable gain, and a Q of ~0.707. R2 being zero makes the (r1 * r3) numerator term cancel out to 1.0.
            {
                r.fc = (r1 * r3) / (2 * M_PI * ((r1 * r2) + (r1 * r3) + (r2 * r3)) * r3 * c2);
                r.type = biquad_type.LOWPASS1P;
            }
            else // common case, (r2 != 0) && (c1 != 0)
            {
                r.fc = 1.0 / (2 * M_PI * std.sqrt(r2 * r3 * c1 * c2));
                r.q = std.sqrt(r2 * r3 * c1 * c2) / ((r3 * c2) + (r2 * c2) + ((r2 * c2) * -r.gain));
                r.type = biquad_type.LOWPASS;
            }

#if FLT_BIQUAD_DEBUG_SETUP
                logerror("filter_biquad_device::opamp_mfb_lowpass_calc(%f, %f, %f, %f, %f) yields:\n\ttype = %d, fc = %f, Q = %f, gain = %f\n", r1, r2, r3, c1*1000000, c2*1000000, static_cast<int>(r.type), r.fc, r.q, r.gain);
#endif

            return r;
        }


        // Multiple-Feedback band-pass
        //filter_biquad_device& opamp_mfb_bandpass_setup(double r1, double r2, double r3, double c1, double c2);

        // Multiple-Feedback high-pass
        //filter_biquad_device& opamp_mfb_highpass_setup(double r1, double r2, double c1, double c2, double c3);

        // Differentiator band-pass
        //filter_biquad_device& opamp_diff_bandpass_setup(double r1, double r2, double c1, double c2);
        //void opamp_diff_bandpass_modify(double r1, double r2, double c1, double c2);
        //biquad_params opamp_diff_bandpass_calc(double r1, double r2, double c1, double c2);


        // device-level overrides
        protected override void device_start()
        {
            m_stream = m_disound.stream_alloc(1, 1, SAMPLE_RATE_OUTPUT_ADAPTIVE);
            m_last_sample_rate = 0;
            recalc();

            save_item(NAME(new { m_type }));
            save_item(NAME(new { m_last_sample_rate }));
            save_item(NAME(new { m_fc }));
            save_item(NAME(new { m_q }));
            save_item(NAME(new { m_gain }));
            save_item(NAME(new { m_input }));
            save_item(NAME(new { m_w0 }));
            save_item(NAME(new { m_w1 }));
            save_item(NAME(new { m_w2 }));
            save_item(NAME(new { m_output }));
            save_item(NAME(new { m_a1 }));
            save_item(NAME(new { m_a2 }));
            save_item(NAME(new { m_b0 }));
            save_item(NAME(new { m_b1 }));
            save_item(NAME(new { m_b2 }));
        }


        // sound stream update overrides
        protected virtual void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)
        {
            var src = inputs[0];
            var dst = outputs[0];

            if (m_last_sample_rate != m_stream.sample_rate())
            {
                recalc();
                m_last_sample_rate = (int)m_stream.sample_rate();
            }

            for (int sampindex = 0; sampindex < dst.samples(); sampindex++)
            {
                m_input = src.get(sampindex);
                step();
                dst.put(sampindex, m_output);
            }
        }


        void recalc()
        {
            if (m_type == biquad_type.RAWPARAMS)
                return; // if we're dealing with raw parameters, just return, don't touch anything.

            double MGain = fabs(m_gain); // absolute multiplicative gain
            double DBGain = log10(MGain) * 20.0; // gain in dB
            double AMGain = pow(10, fabs(DBGain) / 20.0); // multiplicative gain of absolute DB
            double K = tan(M_PI * m_fc / m_stream.sample_rate());
            double Ksquared = K * K;
            double KoverQ = K / m_q;
            double normal = 1.0 / (1.0 + KoverQ + Ksquared);

            switch (m_type)
            {
                case biquad_type.LOWPASS1P:
                    m_a1 = exp(-2.0 * M_PI * (m_fc / m_stream.sample_rate()));
                    m_b0 = 1.0 - m_a1;
                    m_a1 = -m_a1;
                    m_b1 = m_b2 = m_a2 = 0.0;
                    break;
                case biquad_type.HIGHPASS1P:
                    m_a1 = -exp(-2.0 * M_PI * (0.5 - m_fc / m_stream.sample_rate()));
                    m_b0 = 1.0 + m_a1;
                    m_a1 = -m_a1;
                    m_b1 = m_b2 = m_a2 = 0.0;
                    break;
                case biquad_type.LOWPASS:
                    m_b0 = Ksquared * normal;
                    m_b1 = 2.0 * m_b0;
                    m_b2 = 1.0 * m_b0;
                    m_a1 = 2.0 * (Ksquared - 1.0) * normal;
                    m_a2 = (1.0 - KoverQ + Ksquared) * normal;
                    break;
                case biquad_type.HIGHPASS:
                    m_b0 = 1.0 * normal;
                    m_b1 = -2.0 * m_b0;
                    m_b2 = 1.0 * m_b0;
                    m_a1 = 2.0 * (Ksquared - 1.0) * normal;
                    m_a2 = (1.0 - KoverQ + Ksquared) * normal;
                    break;
                case biquad_type.BANDPASS:
                    m_b0 = KoverQ * normal;
                    m_b1 = 0.0;
                    m_b2 = -1.0 * m_b0;
                    m_a1 = 2.0 * (Ksquared - 1.0) * normal;
                    m_a2 = (1.0 - KoverQ + Ksquared) * normal;
                    break;
                case biquad_type.NOTCH:
                    m_b0 = (1.0 + Ksquared) * normal;
                    m_b1 = 2.0 * (Ksquared - 1.0) * normal;
                    m_b2 = 1.0 * m_b0;
                    m_a1 = 1.0 * m_b1;
                    m_a2 = (1.0 - KoverQ + Ksquared) * normal;
                    break;
                case biquad_type.PEAK:
                    if (DBGain >= 0.0)
                    {
                        m_b0 = (1.0 + (AMGain * KoverQ) + Ksquared) * normal;
                        m_b1 = 2.0 * (Ksquared - 1.0) * normal;
                        m_b2 = (1.0 - (AMGain * KoverQ) + Ksquared) * normal;
                        m_a1 = 1.0 * m_b1;
                        m_a2 = (1.0 - KoverQ + Ksquared) * normal;
                    }
                    else
                    {
                        normal = 1.0 / (1.0 + (AMGain * KoverQ) + Ksquared);
                        m_b0 = (1.0 + KoverQ + Ksquared) * normal;
                        m_b1 = 2.0 * (Ksquared - 1.0) * normal;
                        m_b2 = (1.0 - KoverQ + Ksquared) * normal;
                        m_a1 = 1.0 * m_b1;
                        m_a2 = (1.0 - (AMGain * KoverQ) + Ksquared) * normal;
                    }
                    break;
                case biquad_type.LOWSHELF:
                    if (DBGain >= 0.0)
                    {
                        normal = 1.0 / (1.0 + M_SQRT2 * K + Ksquared);
                        m_b0 = (1.0 + std.sqrt(2.0 * AMGain) * K + AMGain * Ksquared) * normal;
                        m_b1 = 2.0 * (AMGain * Ksquared - 1.0) * normal;
                        m_b2 = (1.0 - std.sqrt(2.0 * AMGain) * K + AMGain * Ksquared) * normal;
                        m_a1 = 2.0 * (Ksquared - 1.0) * normal;
                        m_a2 = (1.0 - M_SQRT2 * K + Ksquared) * normal;
                    }
                    else
                    {
                        normal = 1.0 / (1.0 + std.sqrt(2.0 * AMGain) * K + AMGain * Ksquared);
                        m_b0 = (1.0 + M_SQRT2 * K + Ksquared) * normal;
                        m_b1 = 2.0 * (Ksquared - 1.0) * normal;
                        m_b2 = (1.0 - M_SQRT2 * K + Ksquared) * normal;
                        m_a1 = 2.0 * (AMGain * Ksquared - 1.0) * normal;
                        m_a2 = (1.0 - std.sqrt(2.0 * AMGain) * K + AMGain * Ksquared) * normal;
                    }
                    break;
                case biquad_type.HIGHSHELF:
                    if (DBGain >= 0.0)
                    {
                        normal = 1.0 / (1.0 + M_SQRT2 * K + Ksquared);
                        m_b0 = (AMGain + std.sqrt(2.0 * AMGain) * K + Ksquared) * normal;
                        m_b1 = 2.0 * (Ksquared - AMGain) * normal;
                        m_b2 = (AMGain - std.sqrt(2.0 * AMGain) * K + Ksquared) * normal;
                        m_a1 = 2.0 * (Ksquared - 1.0) * normal;
                        m_a2 = (1.0 - M_SQRT2 * K + Ksquared) * normal;
                    }
                    else
                    {
                        normal = 1.0 / (AMGain + std.sqrt(2.0 * AMGain) * K + Ksquared);
                        m_b0 = (1.0 + M_SQRT2 * K + Ksquared) * normal;
                        m_b1 = 2.0 * (Ksquared - 1.0) * normal;
                        m_b2 = (1.0 - M_SQRT2 * K + Ksquared) * normal;
                        m_a1 = 2.0 * (Ksquared - AMGain) * normal;
                        m_a2 = (AMGain - std.sqrt(2.0 * AMGain) * K + Ksquared) * normal;
                    }
                    break;
                default:
                    fatalerror("filter_biquad_device::recalc() - Invalid filter type!");
                    break;
            }

#if FLT_BIQUAD_DEBUG
            logerror("Calculated Parameters:\n");
            logerror( "Gain (dB): %f, (raw): %f\n", DBGain, MGain);
            logerror( "k: %f\n", K);
            logerror( "normal: %f\n", normal);
            logerror("b0: %f\n", m_b0);
            logerror("b1: %f\n", m_b1);
            logerror("b2: %f\n", m_b2);
            logerror("a1: %f\n", m_a1);
            logerror("a2: %f\n", m_a2);
#endif

            // peak and shelf filters do not use gain for the entire signal, only for the peak/shelf portions
            // side note: the first order lowpass and highpass filter analogs technically don't have gain either,
            // but this can be 'faked' by adjusting the bx factors, so we support that anyway, even if it isn't realistic.
            if (   (m_type != biquad_type.PEAK)
                && (m_type != biquad_type.LOWSHELF)
                && (m_type != biquad_type.HIGHSHELF))
            {
                m_b0 *= m_gain;
                m_b1 *= m_gain;
                m_b2 *= m_gain;

#if FLT_BIQUAD_DEBUG
                logerror("b0g: %f\n", m_b0);
                logerror("b1g: %f\n", m_b1);
                logerror("b2g: %f\n", m_b2);
#endif
            }
        }


        void step()
        {
            m_w2 = m_w1;
            m_w1 = m_w0;
            m_w0 = (-m_a1 * m_w1) + (-m_a2 * m_w2) + m_input;
            m_output = (stream_buffer_sample_t)((m_b0 * m_w0) + (m_b1 * m_w1) + (m_b2 * m_w2));
        }
    }


    static class flt_biquad_internal
    {
        // we need the M_SQRT2 constant
        //#ifndef M_SQRT2
        public const double M_SQRT2 = 1.41421356237309504880;
        //#endif
    }


    public static class flt_biquad_global
    {
        public static filter_biquad_device FILTER_BIQUAD<bool_Required>(machine_config mconfig, device_finder<filter_biquad_device, bool_Required> finder) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, filter_biquad_device.FILTER_BIQUAD, 0); }
    }
}
