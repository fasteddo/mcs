// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;

using static mame.device_global;


namespace mame
{
    class speechrom_device : device_t
    {
        //DEFINE_DEVICE_TYPE(SPEECHROM, speechrom_device, "speechrom", "TI Speech ROM")
        public static readonly emu.detail.device_type_impl SPEECHROM = DEFINE_DEVICE_TYPE("speechrom", "TI Speech ROM", (type, mconfig, tag, owner, clock) => { return new speechrom_device(mconfig, tag, owner, clock); });


        object m_speechrom_data;  //uint8_t *m_speechrom_data;           /* pointer to speech ROM data */
        unsigned m_speechROMlen;  //unsigned int m_speechROMlen;       /* length of data pointed by speechrom_data, from 0 to 2^18 */
        unsigned m_speechROMaddr;  //unsigned int m_speechROMaddr;      /* 18 bit pointer in ROM */
        int m_load_pointer;                /* which 4-bit nibble will be affected by load address */
        int m_ROM_bits_count;              /* current bit position in ROM */
        bool m_reverse;


        // construction/destruction
        speechrom_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, SPEECHROM, tag, owner, clock)
        {
            m_speechrom_data = null;
            m_speechROMlen = 0;
            m_speechROMaddr = 0;
            m_load_pointer = 0;
            m_ROM_bits_count = 0;
            m_reverse = false;
        }


        /// TODO: implement bus behaviour
        public int read(int count) { throw new emu_unimplemented(); }
        public void load_address(int data) { throw new emu_unimplemented(); }
        public void read_and_branch() { throw new emu_unimplemented(); }
        //void set_reverse_bit_order(bool reverse) { m_reverse = reverse; }


        // device-level overrides
        protected override void device_start() { throw new emu_unimplemented(); }
    }
}
