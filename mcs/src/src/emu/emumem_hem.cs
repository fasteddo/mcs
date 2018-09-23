// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using u8 = System.Byte;


namespace mame
{
    // handler_entry_read_memory/handler_entry_write_memory

    // Accesses fixed memory (non-banked rom or ram)

    //template<int Width, int AddrShift, int Endian>
    class handler_entry_read_memory : handler_entry_read_address
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_read_address<Width, AddrShift, Endian>;


        ListBytesPointerRef m_base;  //uX *m_base;


        public handler_entry_read_memory(int Width, int AddrShift, int Endian, address_space space) : base(Width, AddrShift, Endian, space, 0) { }
        //~handler_entry_read_memory() = default;


        //uX read(offs_t offset, uX mem_mask) override;
        public override u8 read(offs_t offset, u8 mem_mask)
        {
            return m_base.m_listPtr[((offset - m_address_base) & m_address_mask) >> (Width + AddrShift)];  //return m_base[((offset - inh::m_address_base) & inh::m_address_mask) >> (Width + AddrShift)];
        }


        //void *get_ptr(offs_t offset) const override;


        public void set_base(ListBytesPointerRef base_) { m_base = base_; }

        //std::string name() const override;
    }


    //template<int Width, int AddrShift, int Endian>
    class handler_entry_write_memory : handler_entry_write_address
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_write_address<Width, AddrShift, Endian>;


        ListBytesPointerRef m_base;  //uX *m_base;


        public handler_entry_write_memory(int Width, int AddrShift, int Endian, address_space space) : base(Width, AddrShift, Endian, space, 0) { }
        //~handler_entry_write_memory() = default;


        public override void write(offs_t offset, u8 data, u8 mem_mask)
        {
            offs_t off = ((offset - m_address_base) & m_address_mask) >> (Width + AddrShift);
            m_base.m_listPtr[off] = (u8)((m_base.m_listPtr[off] & ~mem_mask) | (data & mem_mask));
        }


        //void *get_ptr(offs_t offset) const override;

        public void set_base(ListBytesPointerRef base_) { m_base = base_; }

        //std::string name() const override;
    }


    // handler_entry_read_memory_bank/handler_entry_write_memory_bank

    // Accesses banked memory, associated to a memory_bank

    //template<int Width, int AddrShift, int Endian>
    class handler_entry_read_memory_bank : handler_entry_read_address
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_read_address<Width, AddrShift, Endian>;


        memory_bank m_bank;


        public handler_entry_read_memory_bank(int Width, int AddrShift, int Endian, address_space space, memory_bank bank) : base(Width, AddrShift, Endian, space, 0) { m_bank = bank; }
        //~handler_entry_read_memory_bank() = default;


        //uX read(offs_t offset, uX mem_mask) override;
        public override u8 read(offs_t offset, u8 mem_mask)
        {
            return m_bank.base_().m_listPtr[((offset - m_address_base) & m_address_mask) >> (Width + AddrShift)];  //return static_cast<uX *>(m_bank.base())[((offset - inh::m_address_base) & inh::m_address_mask) >> (Width + AddrShift)];
        }


        //void *get_ptr(offs_t offset) const override;

        //std::string name() const override;
    }


    //template<int Width, int AddrShift, int Endian>
    class handler_entry_write_memory_bank : handler_entry_write_address
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_write_address<Width, AddrShift, Endian>;


        memory_bank m_bank;


        public handler_entry_write_memory_bank(int Width, int AddrShift, int Endian, address_space space, memory_bank bank) : base(Width, AddrShift, Endian, space, 0) { m_bank = bank; }
        //~handler_entry_write_memory_bank() = default;


        public override void write(offs_t offset, u8 data, u8 mem_mask)
        {
            throw new emu_unimplemented();
#if false
            offs_t off = ((offset - inh::m_address_base) & inh::m_address_mask) >> (Width + AddrShift);
            static_cast<uX *>(m_bank.base())[off] = (static_cast<uX *>(m_bank.base())[off] & ~mem_mask) | (data & mem_mask);
#endif
        }


        //void *get_ptr(offs_t offset) const override;

        //std::string name() const override;
    }
}
