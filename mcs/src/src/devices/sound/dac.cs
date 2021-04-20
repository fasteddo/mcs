// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using stream_sample_t = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint32_t = System.UInt32;


namespace mame
{
    static class dac_global
    {
        public const UInt32 DAC_VREF_POS_INPUT = 0;
        public const UInt32 DAC_VREF_NEG_INPUT = 1;


        public const double dac_gain_r2r = 1.0;
        const double dac_gain_binary_weighted = 2.0;


        //template <unsigned bits>
        public static stream_sample_t dac_multiply(int bits, double vref, stream_sample_t code)
        {
            return bits > 1 ? (stream_sample_t)((vref * code) / (1 << bits)) : (stream_sample_t)(vref * code);
        }
    }


    interface dac_bit_interface
    {
        void flipscreen_w(int state);  //virtual DECLARE_WRITE_LINE_MEMBER(write) = 0;
        void data_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff);  //virtual DECLARE_WRITE8_MEMBER(data_w) = 0;
    }


    interface dac_byte_interface
    {
        void write(byte data);
        void data_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff);  //virtual DECLARE_WRITE8_MEMBER(data_w) = 0;
    }


    //template <unsigned bits>
    public abstract class dac_code
    {
        // template parameter
        protected int bits;


        public sound_stream m_stream;
        public stream_sample_t m_code;
        protected double m_gain;


        protected dac_code(int bits, double gain)
        {
            this.bits = bits;

            m_stream = null;
            m_code = 0;
            m_gain = gain;
        }


        public void setCode(stream_sample_t code)
        {
            code &= ~(~(stream_sample_t)0 << bits);  //code &= ~(~std::make_unsigned_t<stream_sample_t>(0) << bits);
            if (m_code != code)
            {
                m_stream.update();
                m_code = code;
            }
        }


        public abstract void sound_stream_update_tag(sound_stream stream, Pointer<stream_sample_t> [] inputs, Pointer<stream_sample_t> [] outputs, int samples);
    }


    //template <unsigned bits>
    public class dac_code_binary : dac_code//<bits>
    {
        //using dac_code<bits>::dac_code;


        public dac_code_binary(int bits, double gain)
            : base(bits, gain)
        { }


        public override void sound_stream_update_tag(sound_stream stream, Pointer<stream_sample_t> [] inputs, Pointer<stream_sample_t> [] outputs, int samples)
        {
            for (int samp = 0; samp < samples; samp++)
            {
                double vref_pos = inputs[dac_global.DAC_VREF_POS_INPUT][samp] * this.m_gain;
                double vref_neg = inputs[dac_global.DAC_VREF_NEG_INPUT][samp] * this.m_gain;
                stream_sample_t vout = (stream_sample_t)(vref_neg + dac_global.dac_multiply(bits, vref_pos - vref_neg, this.m_code));
                outputs[0][samp] = vout;
            }
        }
    }


    //template <typename _dac_code>
    public class dac_device : device_t
                              //device_sound_interface,
                              //_dac_code
    {
        public class device_sound_interface_dac : device_sound_interface
        {
            public device_sound_interface_dac(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, Pointer<stream_sample_t> [] inputs, Pointer<stream_sample_t> [] outputs, int samples) { ((dac_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs, samples); }
        }


        device_sound_interface_dac m_disound;
        protected dac_code_binary m_dac_code;


        protected dac_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, double gain)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_dac(mconfig, this));  // device_sound_interface(mconfig, *this);
            m_disound = GetClassInterface<device_sound_interface_dac>();

            m_dac_code = new dac_code_binary(8, gain);  //_dac_code(gain)
        }


        public device_sound_interface_dac disound { get { return m_disound; } }


        protected override void device_start()
        {
            m_disound = GetClassInterface<device_sound_interface_dac>();

            m_dac_code.m_stream = m_disound.stream_alloc(2, 1, 48000 * 4);

            save_item(NAME(new { m_dac_code.m_code }));
        }


        // device_sound_interface overrides
        void device_sound_interface_sound_stream_update(sound_stream stream, Pointer<stream_sample_t> [] inputs, Pointer<stream_sample_t> [] outputs, int samples)
        {
            m_dac_code.sound_stream_update_tag(stream, inputs, outputs, samples);
        }
    }


#if false
    template <typename _dac_code>
    class dac_generator<dac_bit_interface, _dac_code> :
        public dac_bit_interface,
        public dac_device<_dac_code>
    {
    public:
        dac_generator(const machine_config &mconfig, device_type type, const char *tag, device_t *owner, uint32_t clock, double gain) :
            dac_device<_dac_code>(mconfig, type, tag, owner, clock, gain)
        {
        }

        virtual WRITE_LINE_MEMBER(write) override { this->setCode(state); }
        virtual WRITE8_MEMBER(data_w) override { this->setCode(data); }
    };

    template <typename _dac_code>
    class dac_generator<dac_byte_interface, _dac_code> :
        public dac_byte_interface,
        public dac_device<_dac_code>
    {
    public:
        dac_generator(const machine_config &mconfig, device_type type, const char *tag, device_t *owner, uint32_t clock, double gain) :
            dac_device<_dac_code>(mconfig, type, tag, owner, clock, gain)
        {
        }

        virtual void write(unsigned char data) override { this->setCode(data); }
        virtual DECLARE_WRITE8_MEMBER(data_w) override { this->setCode(data); }
    };

    template <typename _dac_code>
    class dac_generator<dac_word_interface, _dac_code> :
        public dac_word_interface,
        public dac_device<_dac_code>
    {
    public:
        dac_generator(const machine_config &mconfig, device_type type, const char *tag, device_t *owner, uint32_t clock, double gain) :
            dac_device<_dac_code>(mconfig, type, tag, owner, clock, gain)
        {
        }

        virtual void write(unsigned short data) override { this->setCode(data); }
        virtual DECLARE_WRITE16_MEMBER(data_w) override { this->setCode(data); }
    };
#endif


    //template <typename _dac_code>
    public class dac_generator : dac_device, dac_byte_interface
    {
        protected dac_generator(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, double gain)
            : base(mconfig, type, tag, owner, clock, gain)
        { }


        // dac_byte_interface
        public void write(u8 data) { throw new emu_unimplemented(); }
        public virtual void data_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff) { m_dac_code.setCode(data); }  //virtual DECLARE_WRITE8_MEMBER(data_w) override { this->setCode(data); }
    }


    //#ifndef DAC_GENERATOR_EPILOG
    //#define DAC_GENERATOR_EPILOG(_dac_type, _dac_class, _dac_description, _dac_shortname)
    //#endif
    //#define DAC_GENERATOR_EPILOG(_dac_type, _dac_class, _dac_description, _dac_shortname) \
    //DEFINE_DEVICE_TYPE(_dac_type, _dac_class, _dac_shortname, _dac_description)

    //#define DAC_GENERATOR(_dac_type, _dac_class, _dac_interface, _dac_coding, _dac_gain, _dac_description, _dac_shortname) \
    //DECLARE_DEVICE_TYPE(_dac_type, _dac_class) \
    //class _dac_class : public dac_generator<_dac_interface, _dac_coding> \
    //{\
    //public: \
    //    _dac_class(const machine_config &mconfig, const char *tag, device_t *owner, uint32_t clock) : \
    //        dac_generator(mconfig, _dac_type, tag, owner, clock, _dac_gain) {} \
    //}; \
    //DAC_GENERATOR_EPILOG(_dac_type, _dac_class, _dac_description, _dac_shortname)


    // DAC circuits/unidentified chips
    //DAC_GENERATOR(DAC_8BIT_R2R, dac_8bit_r2r_device, dac_byte_interface, dac_code_binary<8>, dac_gain_r2r, "8-Bit R-2R DAC", "dac_8bit_r2r");
    public class dac_8bit_r2r_device : dac_generator //dac_generator_dac_byte_interface_dac_code_binary_8
    {
        //DEFINE_DEVICE_TYPE(_dac_type, _dac_class, _dac_shortname, _dac_description)
        static device_t device_creator_dac_8bit_r2r_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new dac_8bit_r2r_device(mconfig, tag, owner, clock); }
        public static readonly device_type DAC_8BIT_R2R = DEFINE_DEVICE_TYPE(device_creator_dac_8bit_r2r_device, "dac_8bit_r2r", "8-Bit R-2R DAC");

        dac_8bit_r2r_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, DAC_8BIT_R2R, tag, owner, clock, dac_global.dac_gain_r2r)
        { }
    }
}
