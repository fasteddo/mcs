// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using devcb_write8 = mame.devcb_write<System.Byte, System.Byte, mame.devcb_operators_u8_u8, mame.devcb_operators_u8_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write_line = mame.devcb_write<int, uint, mame.devcb_operators_s32_u32, mame.devcb_operators_u32_s32, mame.devcb_constant_1<uint, uint, mame.devcb_operators_u32_u32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using offs_t = System.UInt32;  //using offs_t = u32;
using u8 = System.Byte;
using u32 = System.UInt32;


namespace mame
{
    // ======================> addressable_latch_device
    public class addressable_latch_device : device_t
    {
        const bool LOG_ALL_WRITES          = false;
        const bool LOG_UNDEFINED_WRITES    = false;
        const bool LOG_MYSTERY_BITS        = false;


        // device callbacks
        devcb_write_line.array<uint32_constant_8> m_q_out_cb;      // output line callback array
        devcb_write8 m_parallel_out_cb;  // parallel output option

        // miscellaneous configuration
        bool m_clear_active;     // active state of clear line

        // internal state
        u8 m_address;                  // address input
        bool m_data;                     // data bit input
        u8 m_q;                        // latched output state
        bool m_enable;                   // enable/load active state
        bool m_clear;                    // clear/reset active state


        // construction/destruction
        protected addressable_latch_device(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock, bool clear_active)
            : base(mconfig, type, tag, owner, clock)
        {
            m_q_out_cb = new devcb_write_line.array<uint32_constant_8>(this, () => { return new devcb_write_line(this); });
            m_parallel_out_cb = new devcb_write8(this);
            m_clear_active = clear_active;
        }


        // static configuration
        public devcb_write_line.binder q_out_cb(int Bit) { return m_q_out_cb[Bit].bind(); }  //template <unsigned Bit> auto q_out_cb() { return m_q_out_cb[Bit].bind(); }
        public devcb_write8.binder parallel_out_cb() { return m_parallel_out_cb.bind(); }  //auto parallel_out_cb() { return m_parallel_out_cb.bind(); }


        // data write handlers

        //-------------------------------------------------
        //  write_bit - synchronously update one of the
        //  eight output lines with a new data bit
        //-------------------------------------------------
        void write_bit(offs_t offset, bool d)
        {
            write_abcd((byte)offset, d);
            enable_w(0);
            enable_w(1);
        }


        //-------------------------------------------------
        //  write_abcd - update address select and data
        //  inputs without changing enable state
        //-------------------------------------------------
        void write_abcd(u8 a, bool d)
        {
            m_address = (byte)(a & 7);
            m_data = d;

            if (m_enable)
            {
                if (m_clear)
                    clear_outputs((byte)((m_data ? 1 : 0) << m_address));
                else
                    update_bit();
            }
        }

        //-------------------------------------------------
        //  write_d0 - bus-triggered write handler using
        //  LSB of data (or CRUOUT on TMS99xx)
        //-------------------------------------------------
        public void write_d0(offs_t offset, u8 data)
        {
            if (LOG_MYSTERY_BITS && data != 0x00 && data != 0x01 && data != 0xff)
                logerror("Mystery bits written to Q{0}:{1}{2}{3}{4}{5}{6}{7}\n",  // %d:%s%s%s%s%s%s%s\n",
                    offset,
                    g.BIT(data, 7) != 0 ? " D7" : "",
                    g.BIT(data, 6) != 0 ? " D6" : "",
                    g.BIT(data, 5) != 0 ? " D5" : "",
                    g.BIT(data, 4) != 0 ? " D4" : "",
                    g.BIT(data, 3) != 0 ? " D3" : "",
                    g.BIT(data, 2) != 0 ? " D2" : "",
                    g.BIT(data, 1) != 0 ? " D1" : "");

            write_bit(offset, g.BIT(data, 0) != 0);
        }


        //void write_d1(offs_t offset, u8 data);


        //-------------------------------------------------
        //  write_d7 - bus-triggered write handler using
        //  MSB of (8-bit) data
        //-------------------------------------------------
        public void write_d7(offs_t offset, u8 data)
        {
            if (LOG_MYSTERY_BITS && data != 0x00 && data != 0x80 && data != 0xff)
                logerror("Mystery bits written to Q{0}:{1}{2}{3}{4}{5}{6}{7}\n",  // %d:%s%s%s%s%s%s%s\n",
                    offset,
                    g.BIT(data, 6) != 0 ? " D6" : "",
                    g.BIT(data, 5) != 0 ? " D5" : "",
                    g.BIT(data, 4) != 0 ? " D4" : "",
                    g.BIT(data, 3) != 0 ? " D3" : "",
                    g.BIT(data, 2) != 0 ? " D2" : "",
                    g.BIT(data, 1) != 0 ? " D1" : "",
                    g.BIT(data, 0) != 0 ? " D0" : "");

            write_bit(offset, g.BIT(data, 7) != 0);
        }


        //void write_a0(offs_t offset, u8 data = 0);
        //void write_a3(offs_t offset, u8 data = 0);
        //void write_nibble_d0(u8 data);
        //void write_nibble_d3(u8 data);
        //void clear(u8 data = 0);


        // read handlers (inlined for the sake of optimization)
        public int q0_r() { return g.BIT(m_q, 0); }  //DECLARE_READ_LINE_MEMBER(q0_r) { return BIT(m_q, 0); }
        public int q1_r() { return g.BIT(m_q, 1); }  //DECLARE_READ_LINE_MEMBER(q1_r) { return BIT(m_q, 1); }
        public int q2_r() { return g.BIT(m_q, 2); }  //DECLARE_READ_LINE_MEMBER(q2_r) { return BIT(m_q, 2); }
        public int q3_r() { return g.BIT(m_q, 3); }  //DECLARE_READ_LINE_MEMBER(q3_r) { return BIT(m_q, 3); }
        public int q4_r() { return g.BIT(m_q, 4); }  //DECLARE_READ_LINE_MEMBER(q4_r) { return BIT(m_q, 4); }
        public int q5_r() { return g.BIT(m_q, 5); }  //DECLARE_READ_LINE_MEMBER(q5_r) { return BIT(m_q, 5); }
        public int q6_r() { return g.BIT(m_q, 6); }  //DECLARE_READ_LINE_MEMBER(q6_r) { return BIT(m_q, 6); }
        public int q7_r() { return g.BIT(m_q, 7); }  //DECLARE_READ_LINE_MEMBER(q7_r) { return BIT(m_q, 7); }
        //u8 output_state() const { return m_q; }

        // control inputs

        //-------------------------------------------------
        //  enable_w - handle enable input (active low)
        //-------------------------------------------------
        //WRITE_LINE_MEMBER(addressable_latch_device::enable_w)
        void enable_w(int state)
        {
            m_enable = state == 0;
            if (m_enable)
                update_bit();
            else if (m_clear)
                clear_outputs(0);
        }

        //DECLARE_WRITE_LINE_MEMBER(clear_w);


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            // resolve callbacks
            m_q_out_cb.resolve_all();
            m_parallel_out_cb.resolve();

            // initial input state
            m_address = 0;
            m_data = false;
            m_enable = false;
            m_clear = false;

            // arbitrary initial output state
            m_q = 0xff;

            save_item(NAME(new { m_address }));
            save_item(NAME(new { m_data }));
            save_item(NAME(new { m_enable }));
            save_item(NAME(new { m_q }));
            save_item(NAME(new { m_clear }));
        }


        //-------------------------------------------------
        //  device_reset - reset the device
        //-------------------------------------------------
        protected override void device_reset()
        {
            // assume clear upon reset
            clear_outputs(m_enable ? (byte)((m_data ? 1 : 0) << m_address) : (byte)0);
        }


        // internal helpers

        //-------------------------------------------------
        //  update_bit - update one of the eight output
        //  lines with a new data bit
        //-------------------------------------------------
        void update_bit()
        {
            // first verify that the selected bit is actually changing
            if (g.BIT(m_q, m_address) == (m_data ? 1 : 0))
                return;

            if (!m_clear)
            {
                // update selected bit with new data
                m_q = (byte)((m_q & ~(1 << m_address)) | ((m_data ? 1 : 0) << m_address));
            }
            else
            {
                // clear any other bit that was formerly set
                clear_outputs(0);
                m_q = (byte)((m_data ? 1 : 0) << m_address);
            }

            // update output line via callback
            if (!m_q_out_cb[m_address].isnull())
                m_q_out_cb[m_address].op(m_data ? 1 : 0);

            // update parallel output
            if (!m_parallel_out_cb.isnull())
                m_parallel_out_cb.op(0, m_q, (byte)(1 << m_address));

            // do some logging
            if (LOG_ALL_WRITES || (LOG_UNDEFINED_WRITES && m_q_out_cb[m_address].isnull() && m_parallel_out_cb.isnull()))
                logerror("Q{0} {1} at {2}\n", m_address, m_data ? "set" : "cleared", machine().describe_context());  // Q%d %s at %s\n
        }


        //-------------------------------------------------
        //  clear_outputs - clear all output lines
        //-------------------------------------------------
        void clear_outputs(u8 new_q)
        {
            byte bits_changed = (byte)(m_q ^ new_q);
            if (bits_changed == 0)
                return;

            m_q = new_q;

            // return any previously set output lines to clear state
            for (int bit = 0; bit < 8; bit++)
            {
                if (g.BIT(bits_changed, bit) != 0 && !m_q_out_cb[bit].isnull())
                    m_q_out_cb[bit].op(g.BIT(new_q, bit));
            }

            // update parallel output
            if (!m_parallel_out_cb.isnull())
                m_parallel_out_cb.op(0, new_q, bits_changed);
        }
    }


    // ======================> ls259_device
    public class ls259_device : addressable_latch_device
    {
        //DEFINE_DEVICE_TYPE(LS259, ls259_device, "ls259", "74LS259 Addressable Latch")
        static device_t device_creator_ls259_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new ls259_device(mconfig, tag, owner, clock); }
        public static readonly device_type LS259 = DEFINE_DEVICE_TYPE(device_creator_ls259_device, "ls259", "74LS259 Addressable Latch");


        ls259_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : base(mconfig, LS259, tag, owner, clock, false)
        {
        }
    }
}
