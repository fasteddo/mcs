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

    //template<int Width, int AddrShift, int Endian>
    abstract class handler_entry_read_address : handler_entry_read
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        protected offs_t m_address_base;
        protected offs_t m_address_mask;


        protected handler_entry_read_address(int Width, int AddrShift, int Endian, address_space space, u32 flags) : base(Width, AddrShift, Endian, space, flags) { }
        //~handler_entry_read_address() = default;

        public void set_address_info(offs_t base_, offs_t mask)
        {
            m_address_base = base_;
            m_address_mask = mask;
        }
    }


    //template<int Width, int AddrShift, int Endian>
    abstract class handler_entry_write_address : handler_entry_write
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        protected offs_t m_address_base;
        protected offs_t m_address_mask;


        protected handler_entry_write_address(int Width, int AddrShift, int Endian, address_space space, u32 flags) : base(Width, AddrShift, Endian, space, flags) { }
        //~handler_entry_write_address() = default;

        public void set_address_info(offs_t base_, offs_t mask)
        {
            m_address_base = base_;
            m_address_mask = mask;
        }
    }
}
