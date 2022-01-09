// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;

using static mame.device_global;
using static mame.emucore_global;
using static mame.util;


namespace mame
{
    public class output_latch_device : device_t
    {
        //DEFINE_DEVICE_TYPE(OUTPUT_LATCH, output_latch_device, "output_latch", "Output Latch")
        static device_t device_creator_output_latch_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, uint32_t clock) { return new output_latch_device(mconfig, tag, owner, clock); }
        public static readonly device_type OUTPUT_LATCH = DEFINE_DEVICE_TYPE(device_creator_output_latch_device, "output_latch", "Output Latch");


        devcb_write_line.array<u64_const_8> m_bit_handlers;

        int [] m_bits = new int [8];


        output_latch_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, OUTPUT_LATCH, tag, owner, clock)
        {
            m_bit_handlers = new devcb_write_line.array<u64_const_8>(this, () => { return new devcb_write_line(this); });
            m_bits = new int [] { -1, -1, -1, -1, -1, -1, -1, -1 };
        }


        //template <unsigned Bit>
        public devcb_write_line.binder bit_handler<unsigned_Bit>() where unsigned_Bit : u32_const, new() { unsigned Bit = new unsigned_Bit().value;  return m_bit_handlers[Bit].bind(); }  //auto bit_handler() { return m_bit_handlers[Bit].bind(); }


        public void write(uint8_t data)
        {
            for (unsigned i = 0; 8 > i; ++i)
            {
                int bit = BIT(data, (int)i);
                if (bit != m_bits[i])
                {
                    m_bits[i] = bit;
                    if (m_bit_handlers[i] != null)
                        m_bit_handlers[i].op_s32(bit);
                }
            }
        }



        protected override void device_resolve_objects()
        {
            m_bit_handlers.resolve_all_safe();
        }


        protected override void device_start()
        {
            save_item(NAME(new { m_bits }));
        }
    }
}
