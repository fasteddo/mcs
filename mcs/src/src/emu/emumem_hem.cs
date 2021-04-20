// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using PointerU8 = mame.Pointer<System.Byte>;
using u8 = System.Byte;
using u16 = System.UInt16;


namespace mame
{
    // handler_entry_read_memory/handler_entry_write_memory

    // Accesses fixed memory (non-banked rom or ram)

    //template<int Width, int AddrShift, endianness_t Endian>
    class handler_entry_read_memory<int_Width, int_AddrShift, endianness_t_Endian> : handler_entry_read_address<int_Width, int_AddrShift, endianness_t_Endian>
        where int_Width : int_constant, new()
        where int_AddrShift : int_constant, new()
        where endianness_t_Endian : endianness_t_constant, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_read_address<Width, AddrShift, Endian>;


        PointerRef<u8> m_base;  //uX *m_base;


        public handler_entry_read_memory(address_space space, PointerU8 base_) : base(space, 0)  //handler_entry_read_memory(address_space *space, void *base) : handler_entry_read_address<Width, AddrShift, Endian>(space, 0), m_base(reinterpret_cast<uX *>(base)) {}
        {
            m_base = new PointerRef<u8>(base_);  //m_base(reinterpret_cast<uX *>(base))
        }

        //~handler_entry_read_memory() = default;


        public override uX read(offs_t offset, uX mem_mask)
        {
            //return m_base[((offset - inh::m_address_base) & inh::m_address_mask) >> (Width + AddrShift)];
            switch (Width)
            {
                case 0: return new uX(Width, m_base.m_pointer[((offset - m_address_base) & m_address_mask) >> (Width + AddrShift)]);
                case 1: return new uX(Width, new PointerU16(m_base.m_pointer)[((offset - m_address_base) & m_address_mask) >> (Width + AddrShift)]);
                case 2: throw new emu_unimplemented();
                case 3: throw new emu_unimplemented();
                default: throw new emu_unimplemented();
            }
        }


        public override object get_ptr(offs_t offset)
        {
            throw new emu_unimplemented();
        }


        protected override string name() { throw new emu_unimplemented(); }
    }


    //template<int Width, int AddrShift, endianness_t Endian>
    class handler_entry_write_memory<int_Width, int_AddrShift, endianness_t_Endian> : handler_entry_write_address<int_Width, int_AddrShift, endianness_t_Endian>
        where int_Width : int_constant, new()
        where int_AddrShift : int_constant, new()
        where endianness_t_Endian : endianness_t_constant, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_write_address<Width, AddrShift, Endian>;


        PointerRef<u8> m_base;  //uX *m_base;


        public handler_entry_write_memory(address_space space, object base_) : base(space, 0)  //handler_entry_write_memory(address_space *space, void *base) : handler_entry_write_address<Width, AddrShift, Endian>(space, 0), m_base(reinterpret_cast<uX *>(base)) {}
        {
            m_base = (PointerRef<u8>)base_;  //m_base(reinterpret_cast<uX *>(base))
        }

        //~handler_entry_write_memory() = default;


        public override void write(offs_t offset, uX data, uX mem_mask)
        {
            if (data.width == 0 && mem_mask.width == 0 && Endian == endianness_t.ENDIANNESS_LITTLE)
            {
                //template<> void handler_entry_write_memory<0, 0, ENDIANNESS_LITTLE>::write(offs_t offset, u8 data, u8 mem_mask)
                //m_base[(offset - inh::m_address_base) & inh::m_address_mask] = data;
                m_base.m_pointer[(offset - m_address_base) & m_address_mask] = data.x8;
            }
            else if (data.width == 0 && mem_mask.width == 0 && Endian == endianness_t.ENDIANNESS_BIG)
            {
                //template<> void handler_entry_write_memory<0, 0, ENDIANNESS_BIG>::write(offs_t offset, u8 data, u8 mem_mask)
                //m_base[(offset - inh::m_address_base) & inh::m_address_mask] = data;
                m_base.m_pointer[(offset - m_address_base) & m_address_mask] = data.x8;
            }
            else
            {
                //template<int Width, int AddrShift, endianness_t Endian> void handler_entry_write_memory<Width, AddrShift, Endian>::write(offs_t offset, uX data, uX mem_mask)

                offs_t off = ((offset - m_address_base) & m_address_mask) >> (Width + AddrShift);

                //m_base[off] = (m_base[off] & ~mem_mask) | (data & mem_mask);
                switch (Width)
                {
                    case 0: m_base.m_pointer[off] = (u8)((m_base.m_pointer[off] & ~mem_mask.x8) | (data.x8 & mem_mask.x8)); break;
                    case 1: var pointerU16 = new PointerU16(m_base.m_pointer); pointerU16[off] = (u16)((pointerU16[off] & ~mem_mask.x16) | (data.x16 & mem_mask.x16)); break;
                    case 2: throw new emu_unimplemented();
                    case 3: throw new emu_unimplemented();
                    default: throw new emu_unimplemented();
                }
            }
        }


        public override object get_ptr(offs_t offset)
        {
            throw new emu_unimplemented();
        }


        protected override string name() { throw new emu_unimplemented(); }
    }


    // handler_entry_read_memory_bank/handler_entry_write_memory_bank

    // Accesses banked memory, associated to a memory_bank

    //template<int Width, int AddrShift, endianness_t Endian>
    class handler_entry_read_memory_bank<int_Width, int_AddrShift, endianness_t_Endian> : handler_entry_read_address<int_Width, int_AddrShift, endianness_t_Endian>
        where int_Width : int_constant, new()
        where int_AddrShift : int_constant, new()
        where endianness_t_Endian : endianness_t_constant, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_read_address<Width, AddrShift, Endian>;


        memory_bank m_bank;


        public handler_entry_read_memory_bank(address_space space, memory_bank bank) : base(space, 0) { m_bank = bank; }
        //~handler_entry_read_memory_bank() = default;


        public override uX read(offs_t offset, uX mem_mask)
        {
            //return static_cast<uX *>(m_bank.base())[((offset - inh::m_address_base) & inh::m_address_mask) >> (Width + AddrShift)];
            switch (Width)
            {
                case 0: return new uX(Width, m_bank.base_().m_pointer[((offset - m_address_base) & m_address_mask) >> (Width + AddrShift)]);
                case 1: return new uX(Width, new PointerU16(m_bank.base_().m_pointer)[((offset - m_address_base) & m_address_mask) >> (Width + AddrShift)]);
                case 2: throw new emu_unimplemented();
                case 3: throw new emu_unimplemented();
                default: throw new emu_unimplemented();
            }
        }


        public override object get_ptr(offs_t offset)
        {
            throw new emu_unimplemented();
        }


        protected override string name() { throw new emu_unimplemented(); }
    }


    //template<int Width, int AddrShift, endianness_t Endian>
    class handler_entry_write_memory_bank<int_Width, int_AddrShift, endianness_t_Endian> : handler_entry_write_address<int_Width, int_AddrShift, endianness_t_Endian>
        where int_Width : int_constant, new()
        where int_AddrShift : int_constant, new()
        where endianness_t_Endian : endianness_t_constant, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_write_address<Width, AddrShift, Endian>;


        memory_bank m_bank;


        public handler_entry_write_memory_bank(address_space space, memory_bank bank) : base(space, 0) { m_bank = bank; }
        //~handler_entry_write_memory_bank() = default;


        public override void write(offs_t offset, uX data, uX mem_mask)
        {
            throw new emu_unimplemented();
#if false
            template<> void handler_entry_write_memory_bank<0, 0, ENDIANNESS_LITTLE>::write(offs_t offset, u8 data, u8 mem_mask)
            static_cast<uX *>(m_bank.base())[(offset - inh::m_address_base) & inh::m_address_mask] = data;

            template<> void handler_entry_write_memory_bank<0, 0, ENDIANNESS_BIG>::write(offs_t offset, u8 data, u8 mem_mask)
            static_cast<uX *>(m_bank.base())[(offset - inh::m_address_base) & inh::m_address_mask] = data;

            template<int Width, int AddrShift, int Endian> void handler_entry_write_memory_bank<Width, AddrShift, Endian>::write(offs_t offset, uX data, uX mem_mask)
            offs_t off = ((offset - inh::m_address_base) & inh::m_address_mask) >> (Width + AddrShift);
            static_cast<uX *>(m_bank.base())[off] = (static_cast<uX *>(m_bank.base())[off] & ~mem_mask) | (data & mem_mask);
#endif
        }


        public override object get_ptr(offs_t offset)
        {
            throw new emu_unimplemented();
        }


        protected override string name() { throw new emu_unimplemented(); }
    }
}
