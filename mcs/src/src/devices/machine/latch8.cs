// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    public class latch8_device : device_t
    {
        //DEFINE_DEVICE_TYPE(LATCH8, latch8_device, "latch8", "8-bit latch")
        static device_t device_creator_latch8_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new latch8_device(mconfig, tag, owner, clock); }
        public static readonly device_type LATCH8 = DEFINE_DEVICE_TYPE(device_creator_latch8_device, "latch8", "8-bit latch");


        // internal state
        uint8_t            m_value;
        uint8_t            m_has_write;
        uint8_t            m_has_read;

        /* only for byte reads, does not affect bit reads and node_map */
        uint32_t           m_maskout;
        uint32_t           m_xorvalue;  /* after mask */
        uint32_t           m_nosync;

        devcb_write_line.array<devcb_write_line, uint32_constant_8> m_write_cb;
        devcb_read_line.array<devcb_read_line, uint32_constant_8> m_read_cb;


        latch8_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, LATCH8, tag, owner, clock)
        {
            m_value = 0;
            m_has_write = 0;
            m_has_read = 0;
            m_maskout = 0;
            m_xorvalue = 0;
            m_nosync = 0;
            m_write_cb = new devcb_write_line.array<devcb_write_line, uint32_constant_8>(this, () => { return new devcb_write_line(this); });
            m_read_cb = new devcb_read_line.array<devcb_read_line, uint32_constant_8>(this, () => { return new devcb_read_line(this); });
        }


        /* write & read full byte */

        public uint8_t read(offs_t offset)
        {
            uint8_t res;

            assert(offset == 0);

            res = m_value;
            if (m_has_read != 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (!m_read_cb[i].isnull())
                        res = (uint8_t)((res & ~(1 << i)) | (m_read_cb[i].op() << i));
                }
            }

            return (uint8_t)((res & ~m_maskout) ^ m_xorvalue);
        }


        public void write(offs_t offset, uint8_t data)
        {
            assert(offset == 0);

            if (m_nosync != 0xff)
                machine().scheduler().synchronize(timerproc, (0xFF << 8) | data);
            else
                update(data, 0xFF);
        }


        /* reset the latch */
        //void reset_w(offs_t offset, uint8_t data);

        /* read bit x                 */
        /* return (latch >> x) & 0x01 */

        //DECLARE_READ_LINE_MEMBER( bit0_r ) { return BIT(m_value, 0); }
        //DECLARE_READ_LINE_MEMBER( bit1_r ) { return BIT(m_value, 1); }
        //DECLARE_READ_LINE_MEMBER( bit2_r ) { return BIT(m_value, 2); }
        public int bit3_r() { return BIT(m_value, 3); }  //DECLARE_READ_LINE_MEMBER( bit3_r ) { return BIT(m_value, 3); }
        //DECLARE_READ_LINE_MEMBER( bit4_r ) { return BIT(m_value, 4); }
        //DECLARE_READ_LINE_MEMBER( bit5_r ) { return BIT(m_value, 5); }
        //DECLARE_READ_LINE_MEMBER( bit6_r ) { return BIT(m_value, 6); }
        //DECLARE_READ_LINE_MEMBER( bit7_r ) { return BIT(m_value, 7); }

        /* read inverted bit x        */
        /* return (latch >> x) & 0x01 */

        //DECLARE_READ_LINE_MEMBER( bit0_q_r ) { return BIT(m_value, 0) ^ 1; }
        //DECLARE_READ_LINE_MEMBER( bit1_q_r ) { return BIT(m_value, 1) ^ 1; }
        //DECLARE_READ_LINE_MEMBER( bit2_q_r ) { return BIT(m_value, 2) ^ 1; }
        //DECLARE_READ_LINE_MEMBER( bit3_q_r ) { return BIT(m_value, 3) ^ 1; }
        public int bit4_q_r() { return BIT(m_value, 4) ^ 1; }  //DECLARE_READ_LINE_MEMBER( bit4_q_r ) { return BIT(m_value, 4) ^ 1; }
        public int bit5_q_r() { return BIT(m_value, 5) ^ 1; }  //DECLARE_READ_LINE_MEMBER( bit5_q_r ) { return BIT(m_value, 5) ^ 1; }
        //DECLARE_READ_LINE_MEMBER( bit6_q_r ) { return BIT(m_value, 6) ^ 1; }
        //DECLARE_READ_LINE_MEMBER( bit7_q_r ) { return BIT(m_value, 7) ^ 1; }

        /* write bit x from data into bit determined by offset */
        /* latch = (latch & ~(1<<offset)) | (((data >> x) & 0x01) << offset) */

        public void bit0_w(offs_t offset, uint8_t data) { bitx_w(0, offset, data); }
        //void bit1_w(offs_t offset, uint8_t data);
        //void bit2_w(offs_t offset, uint8_t data);
        //void bit3_w(offs_t offset, uint8_t data);
        //void bit4_w(offs_t offset, uint8_t data);
        //void bit5_w(offs_t offset, uint8_t data);
        //void bit6_w(offs_t offset, uint8_t data);
        //void bit7_w(offs_t offset, uint8_t data);

        /* Bit mask specifying bits to be masked *out* */
        public void set_maskout(uint32_t maskout) { m_maskout = maskout; }

        /* Bit mask specifying bits to be inverted */
        public void set_xorvalue(uint32_t xorvalue) { m_xorvalue = xorvalue; }

        /* Bit mask specifying bits not needing cpu synchronization. */
        //void set_nosync(uint32_t nosync) { m_nosync = nosync; }

        /* Write bit to discrete node */
        public devcb_write.binder write_cb(int N) { return m_write_cb[N].bind(); }

        /* Upon read, replace bits by reading from another device handler */
        public devcb_read.binder read_cb(int N) { return m_read_cb[N].bind(); }


        // device-level overrides

        protected override void device_start()
        {
            /* setup nodemap */
            foreach (var cb in m_write_cb)
            {
                if (!cb.isnull()) m_has_write = 1;
                cb.resolve();
            }

            /* setup device read handlers */
            foreach (var cb in m_read_cb)
            {
                if (!cb.isnull()) m_has_read = 1;
                cb.resolve();
            }

            save_item(NAME(new { m_value }));
        }


        protected override void device_reset()
        {
            m_value = 0;
        }


        protected override void device_validity_check(validity_checker valid) { throw new emu_unimplemented(); }


        //TIMER_CALLBACK_MEMBER( latch8_device::timerproc )
        void timerproc(object ptr, int param)
        {
            uint8_t new_val = (uint8_t)(param & 0xFF);
            uint8_t mask = (uint8_t)(param >> 8);

            update( new_val, mask);
        }


        void update(uint8_t new_val, uint8_t mask)
        {
            uint8_t old_val = m_value;

            m_value = (uint8_t)((m_value & ~mask) | (new_val & mask));

            if (m_has_write != 0)
            {
                uint8_t changed = (uint8_t)(old_val ^ m_value);
                for (int i = 0; i < 8; i++)
                {
                    if (BIT(changed, i) != 0 && !m_write_cb[i].isnull())
                        m_write_cb[i].op(BIT(m_value, i));
                }
            }
        }


        void bitx_w(int bit, offs_t offset, uint8_t data)
        {
            uint8_t mask = (uint8_t)(1 << (int)offset);
            uint8_t masked_data = (uint8_t)(((data >> bit) & 0x01) << (int)offset);

            assert(offset < 8);

            /* No need to synchronize ? */
            if ((m_nosync & mask) != 0)
                update(masked_data, mask);
            else
                machine().scheduler().synchronize(timerproc, (mask << 8) | masked_data);
        }
    }
}
