// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using u32 = System.UInt32;


namespace mame
{
    // handler_entry_read_address/handler_entry_write_address

    // parent class for final handlers which want an address base and a mask

    //template<int Width, int AddrShift>
    abstract class handler_entry_read_address<int_Width, int_AddrShift> : handler_entry_read<int_Width, int_AddrShift>
        where int_Width : int_const, new()
        where int_AddrShift : int_const, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        protected offs_t m_address_base;
        protected offs_t m_address_mask;


        protected handler_entry_read_address(address_space space, u32 flags) : base(space, flags) { }
        //~handler_entry_read_address() = default;

        public void set_address_info(offs_t base_, offs_t mask)
        {
            m_address_base = base_ & ~NATIVE_MASK;  //m_address_base = base & ~handler_entry_read<Width, AddrShift>::NATIVE_MASK;
            m_address_mask = mask;
        }
    }


    //template<int Width, int AddrShift>
    abstract class handler_entry_write_address<int_Width, int_AddrShift> : handler_entry_write<int_Width, int_AddrShift>
        where int_Width : int_const, new()
        where int_AddrShift : int_const, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        new static readonly u32 NATIVE_MASK = Width + AddrShift >= 0 ? (1U << (Width + AddrShift)) - 1 : 0;  //static constexpr u32 NATIVE_MASK = Width + AddrShift >= 0 ? (1 << (Width + AddrShift)) - 1 : 0;


        protected offs_t m_address_base;
        protected offs_t m_address_mask;


        protected handler_entry_write_address(address_space space, u32 flags) : base(space, flags)
        { }

        //~handler_entry_write_address() = default;

        public void set_address_info(offs_t base_, offs_t mask)
        {
            m_address_base = base_ & ~handler_entry_write<int_Width, int_AddrShift>.NATIVE_MASK;  //m_address_base = base & ~handler_entry_write<Width, AddrShift>::NATIVE_MASK;
            m_address_mask = mask;
        }
    }
}
