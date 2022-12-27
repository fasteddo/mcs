// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using stream_buffer_sample_t = System.Single;  //using sample_t = float;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.dac_global;
using static mame.device_global;
using static mame.disound_global;
using static mame.emucore_global;
using static mame.util;


namespace mame
{
    public static partial class dac_global
    {
        //**************************************************************************
        //  CONSTANTS
        //**************************************************************************

        public const int DAC_INPUT_RANGE_HI = 0;
        public const int DAC_INPUT_RANGE_LO = 1;


        //**************************************************************************
        //  TYPE DEFINITIONS
        //**************************************************************************

        public const stream_buffer_sample_t dac_gain_r2r = (stream_buffer_sample_t)1.0;
        public const stream_buffer_sample_t dac_gain_bw = (stream_buffer_sample_t)2.0;


        //-------------------------------------------------
        //  dac_mapper_unsigned - map an unsigned value of
        //  the given number of bits to a sample value
        //-------------------------------------------------
        public static stream_buffer_sample_t dac_mapper_unsigned(u32 input, u8 bits)
        {
            stream_buffer_sample_t scale = (stream_buffer_sample_t)1.0 / (stream_buffer_sample_t)((bits > 1) ? (1 << bits) : 1);
            input &= (1U << bits) - 1;
            return (stream_buffer_sample_t)input * scale;
        }


        //-------------------------------------------------
        //  dac_mapper_signed - map a signed value of
        //  the given number of bits to a sample value
        //-------------------------------------------------
        public static stream_buffer_sample_t dac_mapper_signed(u32 input, u8 bits)
        {
            return dac_mapper_unsigned(input ^ (1U << (bits - 1)), bits);
        }


        //stream_buffer::sample_t dac_mapper_ones_complement(u32 input, u8 bits);
        //stream_buffer::sample_t dac_mapper_sign_magnitude(u32 input, u8 bits);
    }


    // ======================> dac_mapper_callback

    //using dac_mapper_callback = stream_buffer::sample_t (*)(u32 input, u8 bits);
    public delegate stream_buffer_sample_t dac_mapper_callback(u32 input, u8 bits);


    // ======================> dac_bit_interface
    interface dac_bit_interface
    {
        //virtual DECLARE_WRITE_LINE_MEMBER(write) = 0;
        void data_w(u8 data);
    }


    // ======================> dac_byte_interface
    public interface dac_byte_interface
    {
        void write(u8 data);
        void data_w(u8 data);
    }


    // ======================> dac_word_interface
    public interface dac_word_interface
    {
        void write(u16 data);
        void data_w(u16 data);
    }


    // ======================> dac_device_base
    public class dac_device_base : device_t
                                   //device_sound_interface
    {
        public class device_sound_interface_dac_device_base : device_sound_interface
        {
            public device_sound_interface_dac_device_base(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((dac_device_base)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }
        }


        device_sound_interface_dac_device_base m_disound;


        // internal state
        sound_stream m_stream;
        stream_buffer_sample_t m_curval;
        std.vector<stream_buffer_sample_t> m_value_map;

        // configuration state
        u8 m_bits;
        dac_mapper_callback m_mapper;
        stream_buffer_sample_t m_gain;
        stream_buffer_sample_t m_range_min;
        stream_buffer_sample_t m_range_max;


        // constructor
        public dac_device_base(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, u8 bits, dac_mapper_callback mapper, stream_buffer_sample_t gain)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_dac_device_base(mconfig, this));  //device_sound_interface(mconfig, *this);
            m_disound = GetClassInterface<device_sound_interface_dac_device_base>();


            m_stream = null;
            m_curval = 0;
            m_value_map = new std.vector<stream_buffer_sample_t>(1 << bits);
            m_bits = bits;
            m_mapper = mapper;
            m_gain = gain;
            m_range_min = (bits == 1) ? (stream_buffer_sample_t)0.0 : (stream_buffer_sample_t)(-1.0);
            m_range_max = (stream_buffer_sample_t)1.0;
        }


        public device_sound_interface_dac_device_base disound { get { return m_disound; } }


        // device startup
        //-------------------------------------------------
        //  device_start - device startup
        //-------------------------------------------------
        protected override void device_start()
        {
            // precompute all gain-applied values
            for (s32 code = 0; code < (int)m_value_map.size(); code++)
                m_value_map[code] = m_mapper((u32)code, m_bits) * m_gain;

            // determine the number of inputs
            int inputs = (m_specified_inputs_mask == 0) ? 0 : 2;

            // create the stream
            m_stream = stream_alloc(inputs, 1, 48000 * 4);

            // save data
            save_item(NAME(new { m_curval }));
        }


        // device_sound_interface overrides

        protected u32 m_specified_inputs_mask { get { return m_disound.m_specified_inputs_mask; } set { m_disound.m_specified_inputs_mask = value; } }


        // stream generation
        //-------------------------------------------------
        //  sound_stream_update - stream updates
        //-------------------------------------------------
        protected virtual void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            var out_ = outputs[0];

            // rails are constant
            if (inputs.size() == 0)
            {
                out_.fill(m_range_min + m_curval * (m_range_max - m_range_min));
                return;
            }

            var hi = inputs[DAC_INPUT_RANGE_HI];
            var lo = inputs[DAC_INPUT_RANGE_LO];

            // constant lo, streaming hi
            if (BIT(m_specified_inputs_mask, DAC_INPUT_RANGE_LO) == 0)
            {
                for (int sampindex = 0; sampindex < out_.samples(); sampindex++)
                    out_.put(sampindex, m_range_min + m_curval * (hi.get(sampindex) - m_range_min));
            }

            // constant hi, streaming lo
            else if (BIT(m_specified_inputs_mask, DAC_INPUT_RANGE_HI) == 0)
            {
                for (int sampindex = 0; sampindex < out_.samples(); sampindex++)
                    out_.put(sampindex, lo.get(sampindex) + m_curval * (m_range_max - lo.get(sampindex)));
            }

            // both streams provided
            else
            {
                for (int sampindex = 0; sampindex < out_.samples(); sampindex++)
                    out_.put(sampindex, lo.get(sampindex) + m_curval * (hi.get(sampindex) - lo.get(sampindex)));
            }
        }


        protected sound_stream stream_alloc(int inputs, int outputs, u32 sample_rate) { return m_disound.stream_alloc(inputs, outputs, sample_rate); }
        protected sound_stream stream_alloc(int inputs, int outputs, u32 sample_rate, sound_stream_flags flags) { return m_disound.stream_alloc(inputs, outputs, sample_rate, flags); }
        public void set_output_gain(int outputnum, float gain) { m_disound.set_output_gain(outputnum, gain); }


        // set the current value
        protected void set_value(u32 value)
        {
            m_stream.update();
            m_curval = m_value_map[value & (m_value_map.size() - 1)];
        }


        // configuration: default output range is -1..1 for all cases except
        // for 1-bit DACs, which default to 0..1
        //dac_device_base &set_output_range(stream_buffer::sample_t range_min, stream_buffer::sample_t range_max)
        //{
        //    m_range_min = range_min;
        //    m_range_max = range_max;
        //    return *this;
        //}
        //dac_device_base &set_output_range(stream_buffer::sample_t vref) { return set_output_range(-vref, vref); }
    }


    // ======================> dac_bit_device_base
    //class dac_bit_device_base : public dac_device_base, public dac_bit_interface


    // ======================> dac_byte_device_base
    public class dac_byte_device_base : dac_device_base, dac_byte_interface
    {
        protected dac_byte_device_base(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock, u8 bits, dac_mapper_callback mapper, stream_buffer_sample_t gain)
            : base(mconfig, type, tag, owner, clock, bits, mapper, gain)
        {
        }


        public void write(u8 data) { this.set_value(data); }
        public void data_w(u8 data) { this.set_value(data); }
    }


    // ======================> dac_word_device_base
    public class dac_word_device_base : dac_device_base, dac_word_interface
    {
        protected dac_word_device_base(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock, u8 bits, dac_mapper_callback mapper, stream_buffer_sample_t gain)
            : base(mconfig, type, tag, owner, clock, bits, mapper, gain)
        {
        }


        public void write(u16 data) { this.set_value(data); }
        public void data_w(u16 data) { this.set_value(data); }
    }


    //**************************************************************************
    //  DAC GENERATORS
    //**************************************************************************

    // epilog only defined in dac.cpp
    //#ifndef DAC_GENERATOR_EPILOG
    //#define DAC_GENERATOR_EPILOG(_dac_type, _dac_class, _dac_description, _dac_shortname)
    //#endif
    //#define DAC_GENERATOR_EPILOG(_dac_type, _dac_class, _dac_description, _dac_shortname) \
    //DEFINE_DEVICE_TYPE(_dac_type, _dac_class, _dac_shortname, _dac_description)

    //#define DAC_GENERATOR(_dac_type, _dac_class, _dac_base_class, _dac_mapper, _dac_bits, _dac_gain, _dac_description, _dac_shortname) \
    //DECLARE_DEVICE_TYPE(_dac_type, _dac_class) \
    //class _dac_class : public _dac_base_class \
    //{\
    //public: \
    //    _dac_class(const machine_config &mconfig, const char *tag, device_t *owner, uint32_t clock) : \
    //      _dac_base_class(mconfig, _dac_type, tag, owner, clock, _dac_bits, _dac_mapper, _dac_gain) {} \
    //}; \
    //DAC_GENERATOR_EPILOG(_dac_type, _dac_class, _dac_description, _dac_shortname)


    // DAC chips
    //DAC_GENERATOR(AD7533,    ad7533_device,    dac_word_device_base, dac_mapper_unsigned, 10, dac_gain_r2r, "AD7533 DAC",    "ad7533")
    public class ad7533_device : dac_word_device_base
    {
        //DEFINE_DEVICE_TYPE(_dac_type, _dac_class, _dac_shortname, _dac_description)
        public static readonly emu.detail.device_type_impl AD7533 = DEFINE_DEVICE_TYPE("ad7533", "AD7533 DAC", (type, mconfig, tag, owner, clock) => { return new ad7533_device(mconfig, tag, owner, clock); });

        ad7533_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, AD7533, tag, owner, clock, 10, dac_mapper_unsigned, dac_gain_r2r)
        { }
    }

    //DAC_GENERATOR(MC1408,    mc1408_device,    dac_byte_device_base, dac_mapper_unsigned,  8, dac_gain_r2r, "MC1408 DAC",    "mc1408")
    public class mc1408_device : dac_byte_device_base
    {
        //DEFINE_DEVICE_TYPE(_dac_type, _dac_class, _dac_shortname, _dac_description)
        public static readonly emu.detail.device_type_impl MC1408 = DEFINE_DEVICE_TYPE("mc1408", "MC1408 DAC", (type, mconfig, tag, owner, clock) => { return new mc1408_device(mconfig, tag, owner, clock); });

        mc1408_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, MC1408, tag, owner, clock, 8, dac_mapper_unsigned, dac_gain_r2r)
        { }
    }


    // DAC circuits/unidentified chips
    //DAC_GENERATOR(DAC_8BIT_R2R,                  dac_8bit_r2r_device,                  dac_byte_device_base, dac_mapper_unsigned,  8, dac_gain_r2r, "8-Bit R-2R DAC",                  "dac_8bit_r2r")
    public class dac_8bit_r2r_device : dac_byte_device_base
    {
        //DEFINE_DEVICE_TYPE(_dac_type, _dac_class, _dac_shortname, _dac_description)
        public static readonly emu.detail.device_type_impl DAC_8BIT_R2R = DEFINE_DEVICE_TYPE("dac_8bit_r2r", "8-Bit R-2R DAC", (type, mconfig, tag, owner, clock) => { return new dac_8bit_r2r_device(mconfig, tag, owner, clock); });

        dac_8bit_r2r_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, DAC_8BIT_R2R, tag, owner, clock, 8, dac_mapper_unsigned, dac_gain_r2r)
        { }
    }


    //DAC_GENERATOR(DAC_16BIT_R2R_TWOS_COMPLEMENT, dac_16bit_r2r_twos_complement_device, dac_word_device_base, dac_mapper_signed,   16, dac_gain_r2r, "16-Bit R-2R Twos Complement DAC", "dac_16bit_r2r_tc")
    public class dac_16bit_r2r_twos_complement_device : dac_word_device_base
    {
        //DEFINE_DEVICE_TYPE(_dac_type, _dac_class, _dac_shortname, _dac_description)
        public static readonly emu.detail.device_type_impl DAC_16BIT_R2R_TWOS_COMPLEMENT = DEFINE_DEVICE_TYPE("dac_16bit_r2r_tc", "16-Bit R-2R Twos Complement DAC", (type, mconfig, tag, owner, clock) => { return new dac_16bit_r2r_twos_complement_device(mconfig, tag, owner, clock); });

        dac_16bit_r2r_twos_complement_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, DAC_16BIT_R2R_TWOS_COMPLEMENT, tag, owner, clock, 16, dac_mapper_signed, dac_gain_r2r)
        { }
    }


    static partial class dac_global
    {
        public static ad7533_device AD7533<bool_Required>(machine_config mconfig, device_finder<ad7533_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, ad7533_device.AD7533, clock); }
        public static mc1408_device MC1408(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<mc1408_device>(mconfig, tag, mc1408_device.MC1408, clock); }
        public static mc1408_device MC1408<bool_Required>(machine_config mconfig, device_finder<mc1408_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, mc1408_device.MC1408, clock); }
        public static dac_8bit_r2r_device DAC_8BIT_R2R<bool_Required>(machine_config mconfig, device_finder<dac_8bit_r2r_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, dac_8bit_r2r_device.DAC_8BIT_R2R, clock); }
        public static dac_16bit_r2r_twos_complement_device DAC_16BIT_R2R_TWOS_COMPLEMENT<bool_Required>(machine_config mconfig, device_finder<dac_16bit_r2r_twos_complement_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, dac_16bit_r2r_twos_complement_device.DAC_16BIT_R2R_TWOS_COMPLEMENT, clock); }
    }
}
