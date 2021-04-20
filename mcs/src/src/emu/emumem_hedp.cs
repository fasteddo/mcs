// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using u8 = System.Byte;


namespace mame
{
    // handler_entry_read_delegate/handler_entry_write_delegate

    // Executes an access through called a delegate, usually containing a handler or a lambda

    //template<int Width, int AddrShift, endianness_t Endian, typename READ>
    class handler_entry_read_delegate : handler_entry_read_address
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_read_address<Width, AddrShift, Endian>;


        //READ m_delegate;
        read8_delegate m_delegate8;
        read8sm_delegate m_delegate8sm;
        read8smo_delegate m_delegate8smo;
        read16_delegate m_delegate16;
        read16s_delegate m_delegate16s;


        public handler_entry_read_delegate(int Width, int AddrShift, endianness_t Endian, address_space space, object /*READ*/ delegate_) : base(Width, AddrShift, Endian, space, 0)
        {
            if      (delegate_ is read8_delegate)    m_delegate8    = (read8_delegate)delegate_;
            else if (delegate_ is read8sm_delegate)  m_delegate8sm  = (read8sm_delegate)delegate_;
            else if (delegate_ is read8smo_delegate) m_delegate8smo = (read8smo_delegate)delegate_;
            else if (delegate_ is read16_delegate)   m_delegate16   = (read16_delegate)delegate_;
            else if (delegate_ is read16s_delegate)  m_delegate16s  = (read16s_delegate)delegate_;
            else throw new emu_unimplemented();
        }

        //~handler_entry_read_delegate() = default;


        public override uX read(int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX mem_mask)
        {
            //return read_impl<READ>(offset, mem_mask);
            if (m_delegate8 != null)         return read_impl(m_delegate8,    WidthOverride, AddrShiftOverride, EndianOverride, offset, mem_mask);
            else if (m_delegate8sm != null)  return read_impl(m_delegate8sm,  WidthOverride, AddrShiftOverride, EndianOverride, offset, mem_mask);
            else if (m_delegate8smo != null) return read_impl(m_delegate8smo, WidthOverride, AddrShiftOverride, EndianOverride, offset, mem_mask);
            else if (m_delegate16 != null)   return read_impl(m_delegate16,   WidthOverride, AddrShiftOverride, EndianOverride, offset, mem_mask);
            else if (m_delegate16s != null)  return read_impl(m_delegate16s,  WidthOverride, AddrShiftOverride, EndianOverride, offset, mem_mask);
            else throw new emu_unimplemented();
        }


        protected override string name() { throw new emu_unimplemented(); }


        //template<typename R>
        //    std::enable_if_t<std::is_same<R, read8_delegate>::value ||
        //                     std::is_same<R, read16_delegate>::value ||
        //                     std::is_same<R, read32_delegate>::value ||
        //                     std::is_same<R, read64_delegate>::value,
        //                     uX> read_impl(offs_t offset, uX mem_mask);
        uX read_impl(read8_delegate delegate_, int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX mem_mask)
        {
            //return m_delegate(*inh::m_space, ((offset - inh::m_address_base) & inh::m_address_mask) >> (Width + AddrShift), mem_mask);
            return new uX(WidthOverride, m_delegate8(m_space, ((offset - m_address_base) & m_address_mask) >> (WidthOverride + AddrShiftOverride), mem_mask.x8));
        }

        uX read_impl(read16_delegate delegate_, int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX mem_mask)
        {
            //return m_delegate(*inh::m_space, ((offset - inh::m_address_base) & inh::m_address_mask) >> (Width + AddrShift), mem_mask);
            return new uX(WidthOverride, m_delegate16(m_space, ((offset - m_address_base) & m_address_mask) >> (WidthOverride + AddrShiftOverride), mem_mask.x16));
        }

        //template<typename R>
        //    std::enable_if_t<std::is_same<R, read8m_delegate>::value ||
        //                     std::is_same<R, read16m_delegate>::value ||
        //                     std::is_same<R, read32m_delegate>::value ||
        //                     std::is_same<R, read64m_delegate>::value,
        //                     uX> read_impl(offs_t offset, uX mem_mask);

        //template<typename R>
        //    std::enable_if_t<std::is_same<R, read8s_delegate>::value ||
        //                     std::is_same<R, read16s_delegate>::value ||
        //                     std::is_same<R, read32s_delegate>::value ||
        //                     std::is_same<R, read64s_delegate>::value,
        //                     uX> read_impl(offs_t offset, uX mem_mask);
        uX read_impl(read16s_delegate delegate_, int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX mem_mask)
        {
            //return m_delegate(((offset - inh::m_address_base) & inh::m_address_mask) >> (Width + AddrShift), mem_mask);
            return new uX(WidthOverride, m_delegate16s(((offset - m_address_base) & m_address_mask) >> (WidthOverride + AddrShiftOverride), mem_mask.x16));
        }

        //template<typename R>
        //    std::enable_if_t<std::is_same<R, read8sm_delegate>::value ||
        //                     std::is_same<R, read16sm_delegate>::value ||
        //                     std::is_same<R, read32sm_delegate>::value ||
        //                     std::is_same<R, read64sm_delegate>::value,
        //                     uX> read_impl(offs_t offset, uX mem_mask);
        uX read_impl(read8sm_delegate delegate_, int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX mem_mask)
        {
            //return m_delegate(((offset - inh::m_address_base) & inh::m_address_mask) >> (Width + AddrShift));
            return new uX(WidthOverride, m_delegate8sm(((offset - m_address_base) & m_address_mask) >> (Width + AddrShift)));
        }

        //template<typename R>
        //    std::enable_if_t<std::is_same<R, read8mo_delegate>::value ||
        //                     std::is_same<R, read16mo_delegate>::value ||
        //                     std::is_same<R, read32mo_delegate>::value ||
        //                     std::is_same<R, read64mo_delegate>::value,
        //                     uX> read_impl(offs_t offset, uX mem_mask);

        //template<typename R>
        //    std::enable_if_t<std::is_same<R, read8smo_delegate>::value ||
        //                     std::is_same<R, read16smo_delegate>::value ||
        //                     std::is_same<R, read32smo_delegate>::value ||
        //                     std::is_same<R, read64smo_delegate>::value,
        //                     uX> read_impl(offs_t offset, uX mem_mask);
        uX read_impl(read8smo_delegate delegate_, int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX mem_mask)
        {
            //return m_delegate();
            return new uX(WidthOverride, m_delegate8smo());
        }
    }


    //template<int Width, int AddrShift, endianness_t Endian, typename WRITE>
    class handler_entry_write_delegate : handler_entry_write_address
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_write_address<Width, AddrShift, Endian>;


        //WRITE m_delegate;
        write8_delegate     m_delegate8;
        write8sm_delegate   m_delegate8sm;
        write8smo_delegate  m_delegate8smo;
        write16_delegate    m_delegate16;
        write16s_delegate   m_delegate16s;
        write16smo_delegate m_delegate16smo;


        public handler_entry_write_delegate(int Width, int AddrShift, endianness_t Endian, address_space space, object /*WRITE*/ delegate_) : base(Width, AddrShift, Endian, space, 0)
        {
            if (delegate_ is write8_delegate)          m_delegate8 = (write8_delegate)delegate_;
            else if (delegate_ is write8sm_delegate)   m_delegate8sm = (write8sm_delegate)delegate_;
            else if (delegate_ is write8smo_delegate)  m_delegate8smo = (write8smo_delegate)delegate_;
            else if (delegate_ is write16_delegate)    m_delegate16 = (write16_delegate)delegate_;
            else if (delegate_ is write16s_delegate)   m_delegate16s = (write16s_delegate)delegate_;
            else if (delegate_ is write16smo_delegate) m_delegate16smo = (write16smo_delegate)delegate_;
            else throw new emu_unimplemented();
        }

        //~handler_entry_write_delegate() = default;


        public override void write(int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX data, uX mem_mask)
        {
            //write_impl<WRITE>(offset, data, mem_mask);
            if (m_delegate8 != null)          write_impl(m_delegate8,     WidthOverride, AddrShiftOverride, EndianOverride, offset, data, mem_mask);
            else if (m_delegate8sm != null)   write_impl(m_delegate8sm,   WidthOverride, AddrShiftOverride, EndianOverride, offset, data, mem_mask);
            else if (m_delegate8smo != null)  write_impl(m_delegate8smo,  WidthOverride, AddrShiftOverride, EndianOverride, offset, data, mem_mask);
            else if (m_delegate16 != null)    write_impl(m_delegate16,    WidthOverride, AddrShiftOverride, EndianOverride, offset, data, mem_mask);
            else if (m_delegate16s != null)   write_impl(m_delegate16s,   WidthOverride, AddrShiftOverride, EndianOverride, offset, data, mem_mask);
            else if (m_delegate16smo != null) write_impl(m_delegate16smo, WidthOverride, AddrShiftOverride, EndianOverride, offset, data, mem_mask);
            else throw new emu_unimplemented();
        }


        protected override string name() { throw new emu_unimplemented(); }


        //template<typename W>
        //    std::enable_if_t<std::is_same<W, write8_delegate>::value ||
        //                     std::is_same<W, write16_delegate>::value ||
        //                     std::is_same<W, write32_delegate>::value ||
        //                     std::is_same<W, write64_delegate>::value,
        //                     void> write_impl(offs_t offset, uX data, uX mem_mask);
        void write_impl(write8_delegate delegate_, int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX data, uX mem_mask)
        {
            m_delegate8(m_space, ((offset - m_address_base) & m_address_mask) >> (WidthOverride + AddrShiftOverride), data.x8, mem_mask.x8);  //m_delegate(*inh::m_space, ((offset - inh::m_address_base) & inh::m_address_mask) >> (Width + AddrShift), data, mem_mask);
        }

        void write_impl(write16_delegate delegate_, int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX data, uX mem_mask)
        {
            m_delegate16(m_space, ((offset - m_address_base) & m_address_mask) >> (WidthOverride + AddrShiftOverride), data.x16, mem_mask.x16);  //m_delegate(*inh::m_space, ((offset - inh::m_address_base) & inh::m_address_mask) >> (Width + AddrShift), data, mem_mask);
        }

        //template<typename W>
        //    std::enable_if_t<std::is_same<W, write8m_delegate>::value ||
        //                     std::is_same<W, write16m_delegate>::value ||
        //                     std::is_same<W, write32m_delegate>::value ||
        //                     std::is_same<W, write64m_delegate>::value,
        //                     void> write_impl(offs_t offset, uX data, uX mem_mask);

        //template<typename W>
        //    std::enable_if_t<std::is_same<W, write8s_delegate>::value ||
        //                     std::is_same<W, write16s_delegate>::value ||
        //                     std::is_same<W, write32s_delegate>::value ||
        //                     std::is_same<W, write64s_delegate>::value,
        //                     void> write_impl(offs_t offset, uX data, uX mem_mask);
        void write_impl(write16s_delegate delegate_, int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX data, uX mem_mask)
        {
            m_delegate16s(((offset - m_address_base) & m_address_mask) >> (WidthOverride + AddrShiftOverride), data.x16, mem_mask.x16);  //m_delegate(((offset - inh::m_address_base) & inh::m_address_mask) >> (Width + AddrShift), data, mem_mask);
        }

        //template<typename W>
        //    std::enable_if_t<std::is_same<W, write8sm_delegate>::value ||
        //                     std::is_same<W, write16sm_delegate>::value ||
        //                     std::is_same<W, write32sm_delegate>::value ||
        //                     std::is_same<W, write64sm_delegate>::value,
        //                     void> write_impl(offs_t offset, uX data, uX mem_mask);
        void write_impl(write8sm_delegate delegate_, int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX data, uX mem_mask)
        {
            m_delegate8sm(((offset - m_address_base) & m_address_mask) >> (WidthOverride + AddrShiftOverride), data.x8);  //m_delegate(((offset - inh::m_address_base) & inh::m_address_mask) >> (Width + AddrShift), data);
        }

        //template<typename W>
        //    std::enable_if_t<std::is_same<W, write8mo_delegate>::value ||
        //                     std::is_same<W, write16mo_delegate>::value ||
        //                     std::is_same<W, write32mo_delegate>::value ||
        //                     std::is_same<W, write64mo_delegate>::value,
        //                     void> write_impl(offs_t offset, uX data, uX mem_mask);

        //template<typename W>
        //    std::enable_if_t<std::is_same<W, write8smo_delegate>::value ||
        //                     std::is_same<W, write16smo_delegate>::value ||
        //                     std::is_same<W, write32smo_delegate>::value ||
        //                     std::is_same<W, write64smo_delegate>::value,
        //                     void> write_impl(offs_t offset, uX data, uX mem_mask);
        void write_impl(write8smo_delegate delegate_, int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX data, uX mem_mask)
        {
            m_delegate8smo(data.x8);  //m_delegate(data);
        }

        void write_impl(write16smo_delegate delegate_, int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX data, uX mem_mask)
        {
            m_delegate16smo(data.x16);  //m_delegate(data);
        }
    }


    // handler_entry_read_ioport/handler_entry_write_ioport

    // Accesses an ioport

    //template<int Width, int AddrShift, endianness_t Endian>
    class handler_entry_read_ioport : handler_entry_read
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_read<Width, AddrShift, Endian>;


        ioport_port m_port;


        public handler_entry_read_ioport(int Width, int AddrShift, endianness_t Endian, address_space space, ioport_port port) : base(Width, AddrShift, Endian, space, 0) { m_port = port; }
        //~handler_entry_read_ioport() = default;


        public override uX read(int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX mem_mask)
        {
            return new uX(WidthOverride, m_port.read());
        }


        protected override string name() { throw new emu_unimplemented(); }
    }


    //template<int Width, int AddrShift, endianness_t Endian>
    class handler_entry_write_ioport : handler_entry_write
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_write<Width, AddrShift, Endian>;


        ioport_port m_port;


        public handler_entry_write_ioport(int Width, int AddrShift, endianness_t Endian, address_space space, ioport_port port) : base(Width, AddrShift, Endian, space, 0) { m_port = port; }
        //~handler_entry_write_ioport() = default;


        public override void write(int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX data, uX mem_mask)
        {
            throw new emu_unimplemented();
        }


        protected override string name() { throw new emu_unimplemented(); }
    }
}
