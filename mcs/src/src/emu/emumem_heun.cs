// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using u8 = System.Byte;


namespace mame
{
    // handler_entry_read_unmapped/handler_entry_write_unmapped

    // Logs an unmapped access

    //template<int Width, int AddrShift, int Endian>
    public class handler_entry_read_unmapped : handler_entry_read
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_read<Width, AddrShift, Endian>;

        public handler_entry_read_unmapped(int Width, int AddrShift, int Endian, address_space space) : base(Width, AddrShift, Endian, space, 0) { }
        //~handler_entry_read_unmapped() = default;


        public override uX read(int WidthOverride, int AddrShiftOverride, int EndianOverride, offs_t offset, uX mem_mask)
        {
            if (m_space.log_unmap() && !m_space.manager().machine().side_effects_disabled())
                m_space.device().logerror(m_space.is_octal()
                                                ? "{0}: unmapped {1} memory read from {2} & {3}\n"  //? "%s: unmapped %s memory read from %0*o & %0*o\n"
                                                : "{0}: unmapped {1} memory read from {2} & {3}\n",  //: "%s: unmapped %s memory read from %0*X & %0*X\n",
                                                m_space.manager().machine().describe_context(), m_space.name(),
                                                m_space.addrchars(), offset,
                                                2 << WidthOverride, mem_mask);
            return new uX(WidthOverride, m_space.unmap());
        }


        protected override string name() { throw new emu_unimplemented(); }
    }


    //template<int Width, int AddrShift, int Endian>
    public class handler_entry_write_unmapped : handler_entry_write
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_write<Width, AddrShift, Endian>;

        public handler_entry_write_unmapped(int Width, int AddrShift, int Endian, address_space space) : base(Width, AddrShift, Endian, space, 0) { }
        //~handler_entry_write_unmapped() = default;


        public override void write(int WidthOverride, int AddrShiftOverride, int EndianOverride, offs_t offset, uX data, uX mem_mask)
        {
            if (m_space.log_unmap() && !m_space.manager().machine().side_effects_disabled())
                m_space.device().logerror(m_space.is_octal()
                                                ? "{0}: unmapped {1} memory write to {2} = {3} & {4}\n"  // %0*o = %0*o & %0*o\n"
                                                : "{0}: unmapped {1} memory write to {2} = {3} & {4}\n",  // %0*X = %0*X & %0*X\n",
                                                m_space.manager().machine().describe_context(), m_space.name(),
                                                m_space.addrchars(), offset,
                                                2 << WidthOverride, data,
                                                2 << WidthOverride, mem_mask);
        }


        protected override string name() { throw new emu_unimplemented(); }
    }



    // handler_entry_read_nop/handler_entry_write_nop

    // Drops an unmapped access silently

    //template<int Width, int AddrShift, int Endian>
    class handler_entry_read_nop : handler_entry_read
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_read<Width, AddrShift, Endian>;

        public handler_entry_read_nop(int Width, int AddrShift, int Endian, address_space space) : base(Width, AddrShift, Endian, space, 0) { }
        //~handler_entry_read_nop() = default;


        public override uX read(int WidthOverride, int AddrShiftOverride, int EndianOverride, offs_t offset, uX mem_mask)
        {
            return new uX(WidthOverride, m_space.unmap());
        }


        protected override string name() { throw new emu_unimplemented(); }
    }


    //template<int Width, int AddrShift, int Endian>
    class handler_entry_write_nop : handler_entry_write
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_write<Width, AddrShift, Endian>;

        public handler_entry_write_nop(int Width, int AddrShift, int Endian, address_space space) : base(Width, AddrShift, Endian, space, 0) { }
        //~handler_entry_write_nop() = default;


        public override void write(int WidthOverride, int AddrShiftOverride, int EndianOverride, offs_t offset, uX data, uX mem_mask)
        {
        }


        protected override string name() { throw new emu_unimplemented(); }
    }
}
