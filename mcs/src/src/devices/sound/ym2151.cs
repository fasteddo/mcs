// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using devcb_write8 = mame.devcb_write<System.Byte, System.Byte, mame.devcb_operators_u8_u8, mame.devcb_operators_u8_u8>;  //using devcb_write8 = devcb_write<u8>;
using offs_t = System.UInt32;  //using offs_t = u32;
using uint32_t = System.UInt32;
using u8 = System.Byte;
using ymopm_engine = mame.ymfm_engine_base<mame.ymopm_registers, mame.ymfm_engine_base_operators_ymopm_registers>;  //using ymopm_engine = ymfm_engine_base<ymopm_registers>;


namespace mame
{
    // ======================> ym2151_device
    public class ym2151_device : device_t
                                 //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(YM2151, ym2151_device, "ym2151", "YM2151 OPM")
        static device_t device_creator_ym2151_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, uint32_t clock) { return new ym2151_device(mconfig, tag, owner, clock); }
        public static readonly device_type YM2151 = DEFINE_DEVICE_TYPE(device_creator_ym2151_device, "ym2151", "YM2151 OPM");


        // YM2151 is OPM
        //using fm_engine = ymopm_engine;


        public class device_sound_interface_ym2151 : device_sound_interface
        {
            public device_sound_interface_ym2151(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((ym2151_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        device_sound_interface_ym2151 m_disound;


        // internal state
        ymopm_engine m_fm;              // core OPM engine  //fm_engine m_fm;                  // core FM engine
        sound_stream m_stream;          // sound stream
        devcb_write8 m_port_w;           // port write handler
        attotime m_busy_duration;        // precomputed busy signal duration
        u8 m_address;                    // address register
        u8 m_reset_state;                // reset state


        // constructor
        ym2151_device(machine_config mconfig, string tag, device_t owner, uint32_t clock) : this(mconfig, tag, owner, clock, YM2151)
        {
        }


        ym2151_device(machine_config mconfig, string tag, device_t owner, uint32_t clock, device_type type)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_ym2151(mconfig, this));  //device_sound_interface(mconfig, *this),

            m_disound = GetClassInterface<device_sound_interface_ym2151>();


            m_fm = new ymopm_engine(this);
            m_stream = null;
            m_port_w = new devcb_write8(this);
            m_busy_duration = m_fm.compute_busy_duration();
            m_address = 0;
            m_reset_state = 1;
        }


        public device_sound_interface_ym2151 disound { get { return m_disound; } }


        // configuration helpers
        //auto irq_handler() { return m_fm.irq_handler(); }
        //auto port_write_handler() { return m_port_w.bind(); }


        // read/write access

        //-------------------------------------------------
        //  read - handle a read from the device
        //-------------------------------------------------
        public u8 read(offs_t offset)
        {
            u8 result = 0xff;
            switch (offset & 1)
            {
                case 0: // data port (unused)
                    logerror("Unexpected read from YM2151 offset {0}\n", offset & 3);
                    break;

                case 1: // status port, YM2203 compatible
                    result = m_fm.status();
                    break;
            }

            return result;
        }


        //-------------------------------------------------
        //  write - handle a write to the register
        //  interface
        //-------------------------------------------------
        public virtual void write(offs_t offset, u8 value)
        {
            // ignore writes when the reset is active (low)
            if (m_reset_state == 0)
                return;

            switch (offset & 1)
            {
                case 0: // address port
                    m_address = value;
                    break;

                case 1: // data port

                    // force an update
                    m_stream.update();

                    // write to FM
                    m_fm.write(m_address, value);

                    // special cases
                    if (m_address == 0x01 && g.BIT(value, 1) != 0)
                    {
                        // writes to the test register can reset the LFO
                        m_fm.reset_lfo();
                    }
                    else if (m_address == 0x1b)
                    {
                        // writes to register 0x1B send the upper 2 bits to the output lines
                        m_port_w.op(0, (u8)(value >> 6), 0xff);
                    }

                    // mark busy for a bit
                    m_fm.set_busy_end(machine().time() + m_busy_duration);
                    break;
            }
        }


        //u8 status_r() { return read(1); }
        //void register_w(u8 data) { write(0, data); }
        //void data_w(u8 data) { write(1, data); }


        //DECLARE_WRITE_LINE_MEMBER(reset_w);
        //-------------------------------------------------
        //  reset_w - write to the (active low) reset line
        //-------------------------------------------------
        public void reset_w(int state)
        {
            // reset the device upon going low
            if (state == 0 && m_reset_state != 0)
                reset();
            m_reset_state = (u8)state;
        }


        // device-level overrides
        protected override void device_start()
        {
            throw new emu_unimplemented();
        }


        protected override void device_reset()
        {
            throw new emu_unimplemented();
        }


        protected override void device_clock_changed()
        {
            throw new emu_unimplemented();
        }


        // sound overrides
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            throw new emu_unimplemented();
        }
    }


    // ======================> ym2164_device
    //class ym2164_device : public ym2151_device


    // ======================> ym2414_device
    //class ym2414_device : public ym2151_device
}
