// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    // handler_entry_read_passthrough/handler_entry_write_passthrough

    // parent class for handlers which want to tap the access and usually pass it on to another handler

    //template<int Width, int AddrShift>
    public abstract class handler_entry_read_passthrough<int_Width, int_AddrShift> : handler_entry_read<int_Width, int_AddrShift>, IDisposable  //class handler_entry_read_passthrough : public handler_entry_read<Width, AddrShift>
        where int_Width : int_const, new()
        where int_AddrShift : int_const, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        emu.detail.memory_passthrough_handler_impl m_mph;
        handler_entry_read<int_Width, int_AddrShift> m_next;  //handler_entry_read<Width, AddrShift, Endian> *m_next;


        handler_entry_read_passthrough(address_space space, emu.detail.memory_passthrough_handler_impl mph) : base(space, handler_entry.F_PASSTHROUGH) { m_mph = mph; m_next = null; }
        handler_entry_read_passthrough(address_space space, emu.detail.memory_passthrough_handler_impl mph, handler_entry_read<int_Width, int_AddrShift> next) : base(space, handler_entry.F_PASSTHROUGH) { m_mph = mph; m_next = next;  next.ref_(); mph.add_handler(this); }

        ~handler_entry_read_passthrough()
        {
            //throw new emu_unimplemented();
#if false
            global.assert(m_isDisposed);  // can remove
#endif
        }

        bool m_isDisposed = false;
        public override void Dispose()
        {
            if (m_next != null)
            {
                m_mph.remove_handler(this);
                m_next.unref();
            }

            m_isDisposed = true;
            base.Dispose();
        }


        protected abstract handler_entry_read_passthrough<int_Width, int_AddrShift> instantiate(handler_entry_read<int_Width, int_AddrShift> next);


        //handler_entry_read<Width, AddrShift> *get_subhandler() const { return m_next; }


        public override void detach(std.unordered_set<handler_entry> handlers)
        {
            throw new emu_unimplemented();
        }
    }

    //template<int Width, int AddrShift>
    public abstract class handler_entry_write_passthrough<int_Width, int_AddrShift> : handler_entry_write<int_Width, int_AddrShift>, IDisposable  //class handler_entry_write_passthrough : public handler_entry_write<Width, AddrShift>
        where int_Width : int_const, new()
        where int_AddrShift : int_const, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        emu.detail.memory_passthrough_handler_impl m_mph;
        handler_entry_write<int_Width, int_AddrShift> m_next;  //handler_entry_write<Width, AddrShift> *m_next;


        handler_entry_write_passthrough(address_space space, emu.detail.memory_passthrough_handler_impl mph) : base(space, handler_entry.F_PASSTHROUGH) { m_mph = mph; m_next = null; }
        handler_entry_write_passthrough(address_space space, emu.detail.memory_passthrough_handler_impl mph, handler_entry_write<int_Width, int_AddrShift> next) : base(space, handler_entry.F_PASSTHROUGH) { m_mph = mph; m_next = next;  next.ref_(); mph.add_handler(this); }

        ~handler_entry_write_passthrough()
        {
            //throw new emu_unimplemented();
#if false
            global.assert(m_isDisposed);  // can remove
#endif
        }

        bool m_isDisposed = false;
        public override void Dispose()
        {
            if (m_next != null)
            {
                m_mph.remove_handler(this);
                m_next.unref();
            }

            m_isDisposed = true;
            base.Dispose();
        }


        protected abstract handler_entry_write_passthrough<int_Width, int_AddrShift> instantiate(handler_entry_write<int_Width, int_AddrShift> next);


        //handler_entry_write<Width, AddrShift> *get_subhandler() const { return m_next; }


        public override void detach(std.unordered_set<handler_entry> handlers)
        {
            throw new emu_unimplemented();
        }
    }
}
