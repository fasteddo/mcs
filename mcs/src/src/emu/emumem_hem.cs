// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using PointerU8 = mame.Pointer<System.Byte>;
using u8 = System.Byte;
using u16 = System.UInt16;
using uX = mame.FlexPrim;


namespace mame
{
    // handler_entry_read_memory/handler_entry_write_memory

    // Accesses fixed memory (non-banked rom or ram)

    //template<int Width, int AddrShift>
    class handler_entry_read_memory<int_Width, int_AddrShift> : handler_entry_read_address<int_Width, int_AddrShift>
        where int_Width : int_const, new()
        where int_AddrShift : int_const, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        PointerRef<u8> m_base;  //uX *m_base;


        public handler_entry_read_memory(address_space space, u16 flags, PointerU8 base_) : base(space, flags)  //handler_entry_read_memory(address_space *space, u16 flags, void *base) : handler_entry_read_address<Width, AddrShift>(space, flags), m_base(reinterpret_cast<uX *>(base)) {}
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


        protected override std.pair<uX, u16> read_flags(offs_t offset, uX mem_mask) { throw new emu_unimplemented(); }


        public override object get_ptr(offs_t offset)
        {
            throw new emu_unimplemented();
        }


        protected override string name() { throw new emu_unimplemented(); }
    }


    //template<int Width, int AddrShift>
    class handler_entry_write_memory<int_Width, int_AddrShift> : handler_entry_write_address<int_Width, int_AddrShift>
        where int_Width : int_const, new()
        where int_AddrShift : int_const, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        PointerRef<u8> m_base;  //uX *m_base;


        public handler_entry_write_memory(address_space space, u16 flags, object base_) : base(space, flags)  //handler_entry_write_memory(address_space *space, u16 flags, void *base) : handler_entry_write_address<Width, AddrShift>(space, flags), m_base(reinterpret_cast<uX *>(base)) {}
        {
            m_base = new PointerRef<u8>((Pointer<u8>)base_);  //m_base(reinterpret_cast<uX *>(base))
        }

        //~handler_entry_write_memory() = default;


        //template<int Width, int AddrShift>
        public override void write(offs_t offset, uX data, uX mem_mask)  //void handler_entry_write_memory<Width, AddrShift>::write(offs_t offset, uX data, uX mem_mask) const
        {
            if (Width == 0 && AddrShift == 0 && data.width == 0 && mem_mask == 0)
            {
                //template<> void handler_entry_write_memory<0, 0>::write(offs_t offset, u8 data, u8 mem_mask) const
                m_base.m_pointer[(offset - this.m_address_base) & this.m_address_mask] = data.u8;  //m_base[(offset - this->m_address_base) & this->m_address_mask] = data;
            }
            else
            {
                offs_t off = ((offset - this.m_address_base) & this.m_address_mask) >> (Width + AddrShift);

                //m_base[off] = (m_base[off] & ~mem_mask) | (data & mem_mask);
                switch (Width)
                {
                    case 0: m_base.m_pointer[off] = (u8)((m_base.m_pointer[off] & ~mem_mask.u8) | (data.u8 & mem_mask.u8)); break;
                    case 1: var pointerU16 = new PointerU16(m_base.m_pointer); pointerU16[off] = (u16)((pointerU16[off] & ~mem_mask.u16) | (data.u16 & mem_mask.u16)); break;
                    case 2: throw new emu_unimplemented();
                    case 3: throw new emu_unimplemented();
                    default: throw new emu_unimplemented();
                }
            }
        }

        // template specific implementation merged into above function
        //template<> void handler_entry_write_memory<0, 0>::write(offs_t offset, u8 data, u8 mem_mask) const
        //{
        //    m_base[(offset - this->m_address_base) & this->m_address_mask] = data;
        //}


        protected override u16 write_flags(offs_t offset, uX data, uX mem_mask) { throw new emu_unimplemented(); }


        public override object get_ptr(offs_t offset)
        {
            throw new emu_unimplemented();
        }


        protected override string name() { throw new emu_unimplemented(); }
    }


    // handler_entry_read_memory_bank/handler_entry_write_memory_bank

    // Accesses banked memory, associated to a memory_bank

    //template<int Width, int AddrShift>
    class handler_entry_read_memory_bank<int_Width, int_AddrShift> : handler_entry_read_address<int_Width, int_AddrShift>
        where int_Width : int_const, new()
        where int_AddrShift : int_const, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        memory_bank m_bank;


        public handler_entry_read_memory_bank(address_space space, u16 flags, memory_bank bank) : base(space, flags) { m_bank = bank; }  //handler_entry_read_memory_bank(address_space *space, u16 flags, memory_bank &bank) : handler_entry_read_address<Width, AddrShift>(space, flags), m_bank(bank) {}
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


        protected override std.pair<uX, u16> read_flags(offs_t offset, uX mem_mask) { throw new emu_unimplemented(); }


        public override object get_ptr(offs_t offset)
        {
            throw new emu_unimplemented();
        }


        protected override string name() { throw new emu_unimplemented(); }
    }


    //template<int Width, int AddrShift>
    class handler_entry_write_memory_bank<int_Width, int_AddrShift> : handler_entry_write_address<int_Width, int_AddrShift>
        where int_Width : int_const, new()
        where int_AddrShift : int_const, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        memory_bank m_bank;


        public handler_entry_write_memory_bank(address_space space, u16 flags, memory_bank bank) : base(space, flags) { m_bank = bank; }  //handler_entry_write_memory_bank(address_space *space, u16 flags, memory_bank &bank) : handler_entry_write_address<Width, AddrShift>(space, flags), m_bank(bank) {}
        //~handler_entry_write_memory_bank() = default;


        public override void write(offs_t offset, uX data, uX mem_mask)
        {
            if (Width == 0 && AddrShift == 0 && data.width == 0 && mem_mask == 0)
            {
                //template<> void handler_entry_write_memory_bank<0, 0>::write(offs_t offset, u8 data, u8 mem_mask) const
                m_bank.base_().m_pointer[(offset - this.m_address_base) & this.m_address_mask] = data.u8;  //static_cast<uX *>(m_bank.base())[(offset - this->m_address_base) & this->m_address_mask] = data;
            }
            else
            {
                offs_t off = ((offset - this.m_address_base) & this.m_address_mask) >> (Width + AddrShift);
                //static_cast<uX *>(m_bank.base())[off] = (static_cast<uX *>(m_bank.base())[off] & ~mem_mask) | (data & mem_mask);
                switch (Width)
                {
                    case 0: m_bank.base_().m_pointer[off] = (u8)((m_bank.base_().m_pointer[off] & ~mem_mask.u8) | (data.u8 & mem_mask.u8)); break;
                    case 1: var pointerU16 = new PointerU16(m_bank.base_().m_pointer); pointerU16[off] = (u16)((pointerU16[off] & ~mem_mask.u16) | (data.u16 & mem_mask.u16)); break;
                    case 2: throw new emu_unimplemented();
                    case 3: throw new emu_unimplemented();
                    default: throw new emu_unimplemented();
                }
            }
        }


        protected override u16 write_flags(offs_t offset, uX data, uX mem_mask) { throw new emu_unimplemented(); }


        public override object get_ptr(offs_t offset)
        {
            throw new emu_unimplemented();
        }


        protected override string name() { throw new emu_unimplemented(); }
    }
}
