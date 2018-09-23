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

    //template<int Width, int AddrShift, int Endian, typename READ>
    class handler_entry_read_delegate : handler_entry_read_address
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_read_address<Width, AddrShift, Endian>;


        read8_delegate m_delegate;  //READ m_delegate;


        public handler_entry_read_delegate(int Width, int AddrShift, int Endian, address_space space, read8_delegate /*READ*/ delegate_) : base(Width, AddrShift, Endian, space, 0) { m_delegate = delegate_; }
        //~handler_entry_read_delegate() = default;


        //uX read(offs_t offset, uX mem_mask) override;
        public override u8 read(offs_t offset, u8 mem_mask)
        {
            return m_delegate(m_space, ((offset - m_address_base) & m_address_mask) >> (Width + AddrShift), mem_mask);
        }


        //std::string name() const override;


#if false
        template<typename R>
            std::enable_if_t<std::is_same<R, read8_delegate>::value ||
                             std::is_same<R, read16_delegate>::value ||
                             std::is_same<R, read32_delegate>::value ||
                             std::is_same<R, read64_delegate>::value,
                             uX> read_impl(offs_t offset, uX mem_mask);

        template<typename R>
            std::enable_if_t<std::is_same<R, read8m_delegate>::value ||
                             std::is_same<R, read16m_delegate>::value ||
                             std::is_same<R, read32m_delegate>::value ||
                             std::is_same<R, read64m_delegate>::value,
                             uX> read_impl(offs_t offset, uX mem_mask);

        template<typename R>
            std::enable_if_t<std::is_same<R, read8s_delegate>::value ||
                             std::is_same<R, read16s_delegate>::value ||
                             std::is_same<R, read32s_delegate>::value ||
                             std::is_same<R, read64s_delegate>::value,
                             uX> read_impl(offs_t offset, uX mem_mask);

        template<typename R>
            std::enable_if_t<std::is_same<R, read8sm_delegate>::value ||
                             std::is_same<R, read16sm_delegate>::value ||
                             std::is_same<R, read32sm_delegate>::value ||
                             std::is_same<R, read64sm_delegate>::value,
                             uX> read_impl(offs_t offset, uX mem_mask);

        template<typename R>
            std::enable_if_t<std::is_same<R, read8mo_delegate>::value ||
                             std::is_same<R, read16mo_delegate>::value ||
                             std::is_same<R, read32mo_delegate>::value ||
                             std::is_same<R, read64mo_delegate>::value,
                             uX> read_impl(offs_t offset, uX mem_mask);

        template<typename R>
            std::enable_if_t<std::is_same<R, read8smo_delegate>::value ||
                             std::is_same<R, read16smo_delegate>::value ||
                             std::is_same<R, read32smo_delegate>::value ||
                             std::is_same<R, read64smo_delegate>::value,
                             uX> read_impl(offs_t offset, uX mem_mask);
#endif
    }


    //template<int Width, int AddrShift, int Endian, typename WRITE>
    class handler_entry_write_delegate : handler_entry_write_address
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_write_address<Width, AddrShift, Endian>;


        write8_delegate m_delegate;  //WRITE m_delegate;

        public handler_entry_write_delegate(int Width, int AddrShift, int Endian, address_space space, write8_delegate /*WRITE*/ delegate_) : base(Width, AddrShift, Endian, space, 0) { m_delegate = delegate_; }
        //~handler_entry_write_delegate() = default;


        public override void write(offs_t offset, u8 data, u8 mem_mask)
        {
            m_delegate(m_space, ((offset - m_address_base) & m_address_mask) >> (Width + AddrShift), data, mem_mask);
        }


        //std::string name() const override;


#if false
        template<typename W>
            std::enable_if_t<std::is_same<W, write8_delegate>::value ||
                             std::is_same<W, write16_delegate>::value ||
                             std::is_same<W, write32_delegate>::value ||
                             std::is_same<W, write64_delegate>::value,
                             void> write_impl(offs_t offset, uX data, uX mem_mask);

        template<typename W>
            std::enable_if_t<std::is_same<W, write8m_delegate>::value ||
                             std::is_same<W, write16m_delegate>::value ||
                             std::is_same<W, write32m_delegate>::value ||
                             std::is_same<W, write64m_delegate>::value,
                             void> write_impl(offs_t offset, uX data, uX mem_mask);

        template<typename W>
            std::enable_if_t<std::is_same<W, write8s_delegate>::value ||
                             std::is_same<W, write16s_delegate>::value ||
                             std::is_same<W, write32s_delegate>::value ||
                             std::is_same<W, write64s_delegate>::value,
                             void> write_impl(offs_t offset, uX data, uX mem_mask);

        template<typename W>
            std::enable_if_t<std::is_same<W, write8sm_delegate>::value ||
                             std::is_same<W, write16sm_delegate>::value ||
                             std::is_same<W, write32sm_delegate>::value ||
                             std::is_same<W, write64sm_delegate>::value,
                             void> write_impl(offs_t offset, uX data, uX mem_mask);

        template<typename W>
            std::enable_if_t<std::is_same<W, write8mo_delegate>::value ||
                             std::is_same<W, write16mo_delegate>::value ||
                             std::is_same<W, write32mo_delegate>::value ||
                             std::is_same<W, write64mo_delegate>::value,
                             void> write_impl(offs_t offset, uX data, uX mem_mask);

        template<typename W>
            std::enable_if_t<std::is_same<W, write8smo_delegate>::value ||
                             std::is_same<W, write16smo_delegate>::value ||
                             std::is_same<W, write32smo_delegate>::value ||
                             std::is_same<W, write64smo_delegate>::value,
                             void> write_impl(offs_t offset, uX data, uX mem_mask);
#endif
    }


    // handler_entry_read_ioport/handler_entry_write_ioport

    // Accesses an ioport

    //template<int Width, int AddrShift, int Endian>
    class handler_entry_read_ioport : handler_entry_read
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_read<Width, AddrShift, Endian>;


        ioport_port m_port;


        public handler_entry_read_ioport(int Width, int AddrShift, int Endian, address_space space, ioport_port port) : base(Width, AddrShift, Endian, space, 0) { m_port = port; }
        //~handler_entry_read_ioport() = default;


        //uX read(offs_t offset, uX mem_mask) override;
        public override u8 read(offs_t offset, u8 mem_mask)
        {
            return (u8)m_port.read();
        }


        //std::string name() const override;
    }


    //template<int Width, int AddrShift, int Endian>
    class handler_entry_write_ioport : handler_entry_write
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_write<Width, AddrShift, Endian>;


        ioport_port m_port;


        public handler_entry_write_ioport(int Width, int AddrShift, int Endian, address_space space, ioport_port port) : base(Width, AddrShift, Endian, space, 0) { m_port = port; }
        //~handler_entry_write_ioport() = default;


        public override void write(offs_t offset, u8 data, u8 mem_mask)
        {
            throw new emu_unimplemented();
#if false
            m_port->write(data, mem_mask);
#endif
        }


        //std::string name() const override;
    }
}
