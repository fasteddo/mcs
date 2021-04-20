// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using u32 = System.UInt32;


namespace mame
{
    // handler_entry_read_address/handler_entry_write_address

    // parent class for final handlers which want an address base and a mask

    //template<int Width, int AddrShift, endianness_t Endian>
    abstract class handler_entry_read_address<int_Width, int_AddrShift, endianness_t_Endian> : handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian>
        where int_Width : int_constant, new()
        where int_AddrShift : int_constant, new()
        where endianness_t_Endian : endianness_t_constant, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        protected offs_t m_address_base;
        protected offs_t m_address_mask;


        protected handler_entry_read_address(address_space space, u32 flags) : base(space, flags) { }
        //~handler_entry_read_address() = default;

        public void set_address_info(offs_t base_, offs_t mask)
        {
            m_address_base = base_ & ~NATIVE_MASK;  //m_address_base = base & ~handler_entry_read<Width, AddrShift, Endian>::NATIVE_MASK;
            m_address_mask = mask;
        }
    }


    //template<int Width, int AddrShift, endianness_t Endian>
    abstract class handler_entry_write_address<int_Width, int_AddrShift, endianness_t_Endian> : handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian>
        where int_Width : int_constant, new()
        where int_AddrShift : int_constant, new()
        where endianness_t_Endian : endianness_t_constant, new()
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
            m_address_base = base_ & ~handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian>.NATIVE_MASK;  //m_address_base = base & ~handler_entry_write<Width, AddrShift, Endian>::NATIVE_MASK;
            m_address_mask = mask;
        }
    }
}
