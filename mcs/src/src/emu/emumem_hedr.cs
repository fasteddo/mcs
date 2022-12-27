// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using unsigned = System.UInt32;
using uX = mame.FlexPrim;

using static mame.emu.detail.emumem_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.util;


namespace mame
{
    // handler_entry_read_dispatch

    // dispatches an access among multiple handlers indexed on part of the address

    //template<int HighBits, int Width, int AddrShift>
    class handler_entry_read_dispatch<int_HighBits, int_Width, int_AddrShift> : handler_entry_read<int_Width, int_AddrShift>, IDisposable
        where int_HighBits : int_const, new()
        where int_Width : int_const, new()
        where int_AddrShift : int_const, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using mapping = typename handler_entry_read<Width, AddrShift, Endian>::mapping;


        static readonly int HighBits = new int_HighBits().value;


        class u64_const_COUNT : u64_const { public UInt64 value { get { return COUNT; } } }

        //using std::array<handler_entry_read<Width, AddrShift, Endian> *, COUNT>::array;
        class handler_array : std.array<handler_entry_read<int_Width, int_AddrShift>, u64_const_COUNT>
        {
            public handler_array()
            {
                std.fill(this, () => { return null; });
            }
        }


        //using std::array<handler_entry::range, COUNT>::array;
        class range_array : std.array<handler_entry.range, u64_const_COUNT>
        {
            public range_array()
            {
                std.fill(this, () => { return new handler_entry.range() { start = 0, end = 0 }; });  //std::fill(this->begin(), this->end(), handler_entry::range{ 0, 0 });
            }
        }


        static readonly int Level    = handler_entry_dispatch_level(HighBits);
        static readonly u32 LowBits  = (u32)handler_entry_dispatch_lowbits(HighBits, Width, AddrShift);
        static readonly u32 BITCOUNT = (u32)HighBits > LowBits ? (u32)HighBits - LowBits : 0;
        static readonly u32 COUNT    = 1U << (int)BITCOUNT;
        static readonly offs_t BITMASK  = make_bitmask32(BITCOUNT);
        static readonly offs_t LOWMASK  = make_bitmask32(LowBits);
        static readonly offs_t HIGHMASK = make_bitmask32(HighBits) ^ LOWMASK;
        static readonly offs_t UPMASK   = ~make_bitmask32(HighBits);

        public class int_const_Level : int_const { public int value { get { return Level; } } }
        public class int_const_LowBits : int_const { public int value { get { return (int)LowBits; } } }


        memory_view m_view;

        std.vector<handler_array> m_dispatch_array = new std.vector<handler_array>();
        std.vector<range_array> m_ranges_array = new std.vector<range_array>();

        Pointer<handler_entry_read<int_Width, int_AddrShift>> m_a_dispatch;  //handler_entry_read<Width, AddrShift> **m_a_dispatch;
        Pointer<handler_entry.range> m_a_ranges;  //handler_entry::range *m_a_ranges;

        Pointer<handler_entry_read<int_Width, int_AddrShift>> m_u_dispatch;  //handler_entry_read<Width, AddrShift> **m_u_dispatch;
        Pointer<handler_entry.range> m_u_ranges;  //handler_entry::range *m_u_ranges;


        public handler_entry_read_dispatch(address_space space, handler_entry.range init, handler_entry_read<int_Width, int_AddrShift> handler)
            : base(space, handler_entry.F_DISPATCH)
        {
            m_ranges_array.resize(1);
            m_ranges_array[0] = new range_array();
            m_dispatch_array.resize(1);
            m_dispatch_array[0] = new handler_array();
            m_a_ranges = m_ranges_array[0].data();
            m_a_dispatch = m_dispatch_array[0].data();
            m_u_ranges = m_ranges_array[0].data();
            m_u_dispatch = m_dispatch_array[0].data();


            if (handler == null)
                handler = space.get_unmap_r<int_Width, int_AddrShift>();

            handler.ref_((s32)COUNT);

            for (unsigned i = 0; i != COUNT; i++)
            {
                m_u_dispatch[i] = handler;
                m_u_ranges[i] = new range(init);
            }
        }


        //handler_entry_read_dispatch(address_space *space, memory_view &view);


        handler_entry_read_dispatch(handler_entry_read_dispatch<int_HighBits, int_Width, int_AddrShift> src)
            : base(src.m_space, handler_entry.F_DISPATCH)
        {
            m_view = null;


            m_ranges_array.resize(1);
            m_dispatch_array.resize(1);
            m_a_ranges = m_ranges_array[0].data();
            m_a_dispatch = m_dispatch_array[0].data();
            m_u_ranges = m_ranges_array[0].data();
            m_u_dispatch = m_dispatch_array[0].data();

            for (unsigned i = 0; i != COUNT; i++)
            {
                m_u_dispatch[i] = src.m_u_dispatch[i].dup();
                m_u_ranges[i] = src.m_u_ranges[i];
            }
        }


        ~handler_entry_read_dispatch()
        {
            //throw new emu_unimplemented();
#if false
            global.assert(m_isDisposed);  // can remove
#endif
        }

        bool m_isDisposed = false;
        public override void Dispose()
        {
            foreach (var d in m_dispatch_array)
                foreach (var p in d)
                    if (p != null)
                        p.unref();

            m_isDisposed = true;
            base.Dispose();
        }


        public override uX read(offs_t offset, uX mem_mask)
        {
            return dispatch_read<int_const_Level, int_Width, int_AddrShift>(HIGHMASK, offset, mem_mask, m_a_dispatch);
        }


        protected override std.pair<uX, u16> read_flags(offs_t offset, uX mem_mask) { throw new emu_unimplemented(); }


        public override object get_ptr(offs_t offset)
        {
            throw new emu_unimplemented();
        }


        public override void lookup(offs_t address, ref offs_t start, ref offs_t end, ref handler_entry_read<int_Width, int_AddrShift> handler)
        {
            offs_t slot = (address >> (int)LowBits) & BITMASK;
            var h = m_a_dispatch[slot];
            if (h.is_dispatch() || h.is_view())
            {
                h.lookup(address, ref start, ref end, ref handler);
            }
            else
            {
                start = m_a_ranges[slot].start;
                end = m_a_ranges[slot].end;
                handler = h;
            }
        }


        public override offs_t dispatch_entry(offs_t address)
        {
            return (address & HIGHMASK) >> (int)LowBits;
        }


        protected override void dump_map(std.vector<memory_entry> map)
        {
            throw new emu_unimplemented();
        }


        protected override string name() { throw new emu_unimplemented(); }


        public override void populate_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read<int_Width, int_AddrShift> handler)
        {
            offs_t start_entry = (start & HIGHMASK) >> (int)LowBits;
            offs_t end_entry = (end & HIGHMASK) >> (int)LowBits;
            range_cut_before(ostart - 1, (int)start_entry);
            range_cut_after(oend + 1, (int)end_entry);

            if (LowBits <= Width + AddrShift)
            {
                if (handler.is_view())
                {
                    int delta = (int)(dispatch_entry(ostart) - handler.dispatch_entry(ostart));
                    handler.init_handlers(start >> (int)LowBits, end >> (int)LowBits, LowBits, ostart, oend, m_u_dispatch + delta, m_u_ranges + delta);
                }
                handler.ref_((int)(end_entry - start_entry));
                for (offs_t ent = start_entry; ent <= end_entry; ent++)
                {
                    m_u_dispatch[ent].unref();
                    m_u_dispatch[ent] = handler;
                    m_u_ranges[ent].set(ostart, oend);
                }
            }
            else if (start_entry == end_entry)
            {
                if ((start & LOWMASK) == 0 && (end & LOWMASK) == LOWMASK)
                {
                    if (handler.is_view())
                    {
                        int delta = (int)(dispatch_entry(ostart) - handler.dispatch_entry(ostart));
                        handler.init_handlers(start >> (int)LowBits, end >> (int)LowBits, LowBits, ostart, oend, m_u_dispatch + delta, m_u_ranges + delta);
                    }
                    m_u_dispatch[start_entry].unref();
                    m_u_dispatch[start_entry] = handler;
                    m_u_ranges[start_entry].set(ostart, oend);
                }
                else
                {
                    populate_nomirror_subdispatch(start_entry, start, end, ostart, oend, handler);
                }
            }
            else
            {
                if ((start & LOWMASK) != 0)
                {
                    populate_nomirror_subdispatch(start_entry, start, start | LOWMASK, ostart, oend, handler);
                    start_entry++;
                    start = (start | LOWMASK) + 1;
                    if (start_entry <= end_entry)
                        handler.ref_();
                }

                if ((end & LOWMASK) != LOWMASK)
                {
                    populate_nomirror_subdispatch(end_entry, end & ~LOWMASK, end, ostart, oend, handler);
                    end_entry--;
                    end = (end & ~LOWMASK) - 1;
                    if (start_entry <= end_entry)
                        handler.ref_();
                }

                if (start_entry <= end_entry)
                {
                    if (handler.is_view())
                    {
                        int delta = (int)(dispatch_entry(ostart) - handler.dispatch_entry(ostart));
                        handler.init_handlers(start >> (int)LowBits, end >> (int)LowBits, LowBits, ostart, oend, m_u_dispatch + delta, m_u_ranges + delta);
                    }
                    handler.ref_((int)(end_entry - start_entry));
                    for (offs_t ent = start_entry; ent <= end_entry; ent++)
                    {
                        m_u_dispatch[ent].unref();
                        m_u_dispatch[ent] = handler;
                        m_u_ranges[ent].set(ostart, oend);
                    }
                }
            }
        }

        public override void populate_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read<int_Width, int_AddrShift> handler)
        {
            offs_t hmirror = mirror & HIGHMASK;
            offs_t lmirror = mirror & LOWMASK;

            if (lmirror != 0)
            {
                // If lmirror is non-zero, then each mirror instance is a single entry
                offs_t add = 1 + ~hmirror;
                offs_t offset = 0;
                offs_t base_entry = start >> (int)LowBits;
                do
                {
                    if (offset != 0)
                        handler.ref_();

                    populate_mirror_subdispatch(base_entry | (offset >> (int)LowBits), start | offset, end | offset, ostart | offset, oend | offset, lmirror, handler);
                    offset = (offset + add) & hmirror;
                } while (offset != 0);
            }
            else
            {
                // If lmirror is zero, call the nomirror version as needed
                offs_t add = 1 + ~hmirror;
                offs_t offset = 0;
                do
                {
                    if (offset != 0)
                        handler.ref_();
                    populate_nomirror(start | offset, end | offset, ostart | offset, oend | offset, handler);
                    offset = (offset + add) & hmirror;
                } while (offset != 0);
            }
        }


        public override void populate_mismatched_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, memory_units_descriptor<int_Width, int_AddrShift> descriptor, u8 rkey, std.vector<mapping> mappings)
        {
            offs_t start_entry = (start & HIGHMASK) >> (int)LowBits;
            offs_t end_entry = (end & HIGHMASK) >> (int)LowBits;
            range_cut_before(ostart - 1, (int)start_entry);
            range_cut_after(oend + 1, (int)end_entry);

            if (LowBits <= Width + AddrShift)
            {
                for (offs_t ent = start_entry; ent <= end_entry; ent++)
                {
                    u8 rkey1 = rkey;
                    if (ent != start_entry)
                        rkey1 &= unchecked((u8)~handler_entry.START);
                    if (ent != end_entry)
                        rkey1 &= unchecked((u8)~handler_entry.END);
                    var temp = m_u_dispatch[ent];  mismatched_patch(descriptor, rkey1, mappings, ref temp);  m_u_dispatch[ent] = temp;  //mismatched_patch(descriptor, rkey1, mappings, m_u_dispatch[ent]);
                    m_u_ranges[ent].set(ostart, oend);
                }
            }
            else if (start_entry == end_entry)
            {
                if ((start & LOWMASK) == 0 && (end & LOWMASK) == LOWMASK)
                {
                    if (m_u_dispatch[start_entry].is_dispatch())
                    {
                        m_u_dispatch[start_entry].populate_mismatched_nomirror(start, end, ostart, oend, descriptor, rkey, mappings);
                    }
                    else
                    {
                        var temp = m_u_dispatch[start_entry];  mismatched_patch(descriptor, rkey, mappings, ref temp);  temp = m_u_dispatch[start_entry];  //mismatched_patch(descriptor, rkey, mappings, m_u_dispatch[start_entry]);
                        m_u_ranges[start_entry].set(ostart, oend);
                    }
                }
                else
                {
                    populate_mismatched_nomirror_subdispatch(start_entry, start, end, ostart, oend, descriptor, rkey, mappings);
                }
            }
            else
            {
                if ((start & LOWMASK) != 0)
                {
                    populate_mismatched_nomirror_subdispatch(start_entry, start, start | LOWMASK, ostart, oend, descriptor, (u8)(rkey & ~handler_entry.END), mappings);
                    start_entry++;
                    rkey &= unchecked((u8)~handler_entry.START);
                }

                if ((end & LOWMASK) != LOWMASK)
                {
                    populate_mismatched_nomirror_subdispatch(end_entry, end & ~LOWMASK, end, ostart, oend, descriptor, (u8)(rkey & ~handler_entry.START), mappings);
                    end_entry--;
                    rkey &= unchecked((u8)~handler_entry.END);
                }

                offs_t base_ = start & ~LOWMASK;
                for (offs_t ent = start_entry; ent <= end_entry; ent++)
                {
                    u8 rkey1 = rkey;
                    if (ent != start_entry)
                        rkey1 &= unchecked((u8)~handler_entry.START);
                    if (ent != end_entry)
                        rkey1 &= unchecked((u8)~handler_entry.END);

                    if (m_u_dispatch[ent].is_dispatch())
                    {
                        m_u_dispatch[ent].populate_mismatched_nomirror(base_ | (ent << (int)LowBits), base_ | (ent << (int)LowBits) | LOWMASK, ostart, oend, descriptor, rkey1, mappings);
                    }
                    else
                    {
                        var temp = m_u_dispatch[ent];  mismatched_patch(descriptor, rkey1, mappings, ref temp);  temp = m_u_dispatch[ent];  //mismatched_patch(descriptor, rkey1, mappings, m_u_dispatch[ent]);
                        m_u_ranges[ent].set(ostart, oend);
                    }
                }
            }
        }


        public override void populate_mismatched_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, memory_units_descriptor<int_Width, int_AddrShift> descriptor, std.vector<mapping> mappings)
        {
            offs_t hmirror = mirror & HIGHMASK;
            offs_t lmirror = mirror & LOWMASK;

            if (lmirror != 0)
            {
                // If lmirror is non-zero, then each mirror instance is a single entry
                offs_t add = 1 + ~hmirror;
                offs_t offset = 0;
                offs_t base_entry = start >> (int)LowBits;
                do {
                    populate_mismatched_mirror_subdispatch(base_entry | (offset >> (int)LowBits), start, end, ostart | offset, oend | offset, lmirror, descriptor, mappings);
                    offset = (offset + add) & hmirror;
                } while(offset != 0);
            }
            else
            {
                // If lmirror is zero, call the nomirror version as needed
                offs_t add = 1 + ~hmirror;
                offs_t offset = 0;
                do {
                    populate_mismatched_nomirror(start | offset, end | offset, ostart | offset, oend | offset, descriptor, handler_entry.START|handler_entry.END, mappings);
                    offset = (offset + add) & hmirror;
                } while(offset != 0);
            }
        }


        protected override void populate_passthrough_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read_passthrough<int_Width, int_AddrShift> handler, std.vector<mapping> mappings)
        {
            throw new emu_unimplemented();
        }

        protected override void populate_passthrough_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read_passthrough<int_Width, int_AddrShift> handler, std.vector<mapping> mappings)
        {
            throw new emu_unimplemented();
        }


        public override void detach(std.unordered_set<handler_entry> handlers)
        {
            throw new emu_unimplemented();
        }


        void range_cut_before(offs_t address) { range_cut_before(address, (int)COUNT); }
        void range_cut_before(offs_t address, int start)  // = COUNT)
        {
            while (--start >= 0 && m_u_dispatch[start] != null)
            {
                if ((int)LowBits > -AddrShift && m_u_dispatch[start].is_dispatch())
                {
                    ((handler_entry_read_dispatch<int_const_LowBits, int_Width, int_AddrShift>)m_u_dispatch[start]).range_cut_before(address);  //static_cast<handler_entry_read_dispatch<LowBits, Width, AddrShift> *>(m_dispatch[start]).range_cut_before(address);
                    break;
                }

                if (m_u_ranges[start].end <= address)
                    break;

                m_u_ranges[start].end = address;
            }
        }

        void range_cut_after(offs_t address, int start = -1)
        {
            while (++start < COUNT && m_u_dispatch[start] != null)
            {
                if ((int)LowBits > -AddrShift && m_u_dispatch[start].is_dispatch())
                {
                    ((handler_entry_read_dispatch<int_const_LowBits, int_Width, int_AddrShift>)m_u_dispatch[start]).range_cut_after(address);  //static_cast<handler_entry_read_dispatch<LowBits, Width, AddrShift> *>(m_dispatch[start]).range_cut_after(address);
                    break;
                }

                if (m_u_ranges[start].start >= address)
                    break;

                m_u_ranges[start].start = address;
            }
        }


        protected override void enumerate_references(handler_entry.reflist refs)
        {
            throw new emu_unimplemented();
        }


        //template<int HighBits, int Width, int AddrShift, endianness_t Endian>
        public override Pointer<handler_entry_read<int_Width, int_AddrShift>> get_dispatch()  //handler_entry_read<Width, AddrShift> *const *handler_entry_read_dispatch<HighBits, Width, AddrShift>::get_dispatch() const
        {
            return m_a_dispatch;
        }


        public override void select_a(int id)  //template<int HighBits, int Width, int AddrShift> void handler_entry_read_dispatch<HighBits, Width, AddrShift>::select_a(int id)
        {
            u32 i = (u32)id + 1;
            if (i >= m_dispatch_array.size())
                fatalerror("out-of-range view selection.");

            m_a_ranges = m_ranges_array[i].data();
            m_a_dispatch = m_dispatch_array[i].data();
        }


        public override void select_u(int id)  //template<int HighBits, int Width, int AddrShift> void handler_entry_read_dispatch<HighBits, Width, AddrShift>::select_u(int id)
        {
            u32 i = (u32)id + 1;
            if (i > m_dispatch_array.size())
            {
                fatalerror("out-of-range view update selection.");
            }
            else if (i == m_dispatch_array.size())
            {
                u32 aid = (u32)(m_a_dispatch.Offset - m_dispatch_array.data().Offset);  //u32 aid = (handler_array *)(m_a_dispatch) - m_dispatch_array.data();

                m_dispatch_array.resize(i + 1);
                m_ranges_array.resize(i + 1);
                m_a_ranges = m_ranges_array[aid].data();
                m_a_dispatch = m_dispatch_array[aid].data();
                m_u_ranges = m_ranges_array[i].data();
                m_u_dispatch = m_dispatch_array[i].data();

                for (u32 entry = 0; entry != COUNT; entry++)
                {
                    if (m_dispatch_array[0][entry] != null)
                    {
                        m_u_dispatch[entry] = m_dispatch_array[0][entry].dup();
                        m_u_ranges[entry] = m_ranges_array[0][entry];
                    }
                }
            }
            else
            {
                m_u_ranges = m_ranges_array[i].data();
                m_u_dispatch = m_dispatch_array[i].data();
            }
        }


        public override void init_handlers(offs_t start_entry, offs_t end_entry, u32 lowbits, offs_t ostart, offs_t oend, Pointer<handler_entry_read<int_Width, int_AddrShift>> dispatch, Pointer<handler_entry.range> ranges)  //template<int HighBits, int Width, int AddrShift> void handler_entry_read_dispatch<HighBits, Width, AddrShift>::init_handlers(offs_t start_entry, offs_t end_entry, u32 lowbits, handler_entry_read<Width, AddrShift> **dispatch, handler_entry::range *ranges)
        {
            if (lowbits < LowBits)
            {
                offs_t entry = start_entry >> (int)LowBits;
                if (entry != (end_entry >> (int)LowBits))
                   fatalerror("Recursive init_handlers spanning multiple entries.\n");
                entry &= BITMASK;
                handler_entry_read_dispatch<int_const_LowBits, int_Width, int_AddrShift> subdispatch = null;
                if ((m_u_dispatch[entry].flags() & handler_entry.F_DISPATCH) != 0)
                {
                    subdispatch = (handler_entry_read_dispatch<int_const_LowBits, int_Width, int_AddrShift>)m_u_dispatch[entry];
                }
                else if ((m_u_dispatch[entry].flags() & handler_entry.F_UNMAP) == 0)
                {
                    fatalerror("Collision on multiple init_handlers calls");
                }
                else
                {
                    m_u_dispatch[entry].unref();
                    m_u_dispatch[entry] = subdispatch = new handler_entry_read_dispatch<int_const_LowBits, int_Width, int_AddrShift>(this.m_space, m_u_ranges[entry], null);
                }

                int delta = (int)(dispatch_entry(ostart) - subdispatch.dispatch_entry(ostart));
                subdispatch.init_handlers(start_entry, end_entry, lowbits, ostart, oend, dispatch + delta, ranges + delta);
            }
            else if (lowbits != LowBits)
            {
                u32 dt = lowbits - LowBits;
                u32 ne = 1U << (int)dt;
                u32 ee = end_entry - start_entry;
                if (m_view != null)
                {
                    Func<handler_entry.range, handler_entry.range> filter = (handler_entry.range r) => { var s = m_view.m_addrstart; var e = m_view.m_addrend; r.intersect(s, e); return r; };  //auto filter = [s = m_view->m_addrstart, e = m_view->m_addrend] (handler_entry::range r) { r.intersect(s, e); return r; };

                    for (offs_t entry = 0; entry <= ee; entry++)
                    {
                        dispatch[entry].ref_((int)ne);
                        u32 e0 = (entry << (int)dt) & BITMASK;
                        for (offs_t e = 0; e != ne; e++)
                        {
                            offs_t e1 = e0 | e;
                            if ((m_u_dispatch[e1].flags() & handler_entry.F_UNMAP) == 0)
                                fatalerror("Collision on multiple init_handlers calls");

                            m_u_dispatch[e1].unref();
                            m_u_dispatch[e1] = dispatch[entry];
                            m_u_ranges[e1] = filter(ranges[entry]);
                        }
                    }
                }
                else
                {
                    for (offs_t entry = 0; entry <= ee; entry++)
                    {
                        dispatch[entry].ref_((int)ne);
                        u32 e0 = (entry << (int)dt) & BITMASK;
                        for (offs_t e = 0; e != ne; e++)
                        {
                            offs_t e1 = e0 | e;
                            if ((m_u_dispatch[e1].flags() & handler_entry.F_UNMAP) == 0)
                                fatalerror("Collision on multiple init_handlers calls");

                            m_u_dispatch[e1].unref();
                            m_u_dispatch[e1] = dispatch[entry];
                            m_u_ranges[e1] = ranges[entry];
                        }
                    }
                }
            }
            else
            {
                if (m_view != null)
                {
                    Func<handler_entry.range, handler_entry.range> filter = (handler_entry.range r) => { var s = m_view.m_addrstart; var e = m_view.m_addrend; r.intersect(s, e); return r; };  //auto filter = [s = m_view->m_addrstart, e = m_view->m_addrend] (handler_entry::range r) { r.intersect(s, e); return r; };

                    for (offs_t entry = start_entry & BITMASK; entry <= (end_entry & BITMASK); entry++)
                    {
                        if ((m_u_dispatch[entry].flags() & handler_entry.F_UNMAP) == 0)
                            fatalerror("Collision on multiple init_handlers calls");

                        m_u_dispatch[entry].unref();
                        m_u_dispatch[entry] = dispatch[entry];
                        m_u_ranges[entry] = filter(ranges[entry]);
                        dispatch[entry].ref_();
                    }
                }
                else
                {
                    for (offs_t entry = start_entry & BITMASK; entry <= (end_entry & BITMASK); entry++)
                    {
                        if ((m_u_dispatch[entry].flags() & handler_entry.F_UNMAP) == 0)
                            fatalerror("Collision on multiple init_handlers calls");

                        m_u_dispatch[entry].unref();
                        m_u_dispatch[entry] = dispatch[entry];
                        m_u_ranges[entry] = ranges[entry];
                        dispatch[entry].ref_();
                    }
                }
            }
        }


        public override handler_entry_read<int_Width, int_AddrShift> dup()  //template<int HighBits, int Width, int AddrShift> handler_entry_read<Width, AddrShift, Endian> *handler_entry_read_dispatch<HighBits, Width, AddrShift>::dup()
        {
            if (m_view != null)
            {
                base.ref_();
                return this;
            }

            return new handler_entry_read_dispatch<int_HighBits, int_Width, int_AddrShift>(this);
        }


        void populate_nomirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read<int_Width, int_AddrShift> handler)
        {
            var cur = m_u_dispatch[entry];
            if (cur.is_dispatch())
            {
                cur.populate_nomirror(start, end, ostart, oend, handler);
            }
            else
            {
                var subdispatch = new handler_entry_read_dispatch<int_const_LowBits, int_Width, int_AddrShift>(m_space, m_u_ranges[entry], cur);  //auto subdispatch = new handler_entry_read_dispatch<LowBits, Width, AddrShift>(handler_entry::m_space, m_ranges[entry], cur);
                cur.unref();
                m_u_dispatch[entry] = subdispatch;
                subdispatch.populate_nomirror(start, end, ostart, oend, handler);
            }
        }


        void populate_mirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read<int_Width, int_AddrShift> handler)
        {
            var cur = m_u_dispatch[entry];
            if (cur.is_dispatch())
            {
                cur.populate_mirror(start, end, ostart, oend, mirror, handler);
            }
            else
            {
                var subdispatch = new handler_entry_read_dispatch<int_const_LowBits, int_Width, int_AddrShift>(m_space, m_u_ranges[entry], cur);  //auto subdispatch = new handler_entry_read_dispatch<LowBits, Width, AddrShift>(handler_entry::m_space, m_u_ranges[entry], cur);
                cur.unref();
                m_u_dispatch[entry] = subdispatch;
                subdispatch.populate_mirror(start, end, ostart, oend, mirror, handler);
            }
        }


        void populate_mismatched_nomirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, memory_units_descriptor<int_Width, int_AddrShift> descriptor, u8 rkey, std.vector<mapping> mappings)
        {
            var cur = m_u_dispatch[entry];
            if (cur.is_dispatch())
            {
                cur.populate_mismatched_nomirror(start, end, ostart, oend, descriptor, rkey, mappings);
            }
            else
            {
                var subdispatch = new handler_entry_read_dispatch<int_const_LowBits, int_Width, int_AddrShift>(m_space, m_u_ranges[entry], cur);  //auto subdispatch = new handler_entry_read_dispatch<LowBits, Width, AddrShift>(handler_entry::m_space, m_u_ranges[entry], cur);
                cur.unref();
                m_u_dispatch[entry] = subdispatch;
                subdispatch.populate_mismatched_nomirror(start, end, ostart, oend, descriptor, rkey, mappings);
            }
        }


        void populate_mismatched_mirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, memory_units_descriptor<int_Width, int_AddrShift> descriptor, std.vector<mapping> mappings)
        {
            var cur = m_u_dispatch[entry];
            if (cur.is_dispatch())
            {
                cur.populate_mismatched_mirror(start, end, ostart, oend, mirror, descriptor, mappings);
            }
            else
            {
                var subdispatch = new handler_entry_read_dispatch<int_const_LowBits, int_Width, int_AddrShift>(m_space, m_u_ranges[entry], cur);  //auto subdispatch = new handler_entry_read_dispatch<LowBits, Width, AddrShift>(handler_entry::m_space, m_u_ranges[entry], cur);
                cur.unref();
                m_u_dispatch[entry] = subdispatch;
                subdispatch.populate_mismatched_mirror(start, end, ostart, oend, mirror, descriptor, mappings);
            }
        }


        void mismatched_patch(memory_units_descriptor<int_Width, int_AddrShift> descriptor, u8 rkey, std.vector<mapping> mappings, ref handler_entry_read<int_Width, int_AddrShift> target)
        {
            u8 ukey = descriptor.rkey_to_ukey(rkey);
            handler_entry_read<int_Width, int_AddrShift> original = target.is_units() ? target : null;
            handler_entry_read<int_Width, int_AddrShift> replacement = null;
            foreach (var p in mappings)
            {
                if (p.ukey == ukey && p.original == original)
                {
                    replacement = p.patched;
                    break;
                }
            }

            if (replacement == null)
            {
                if (original != null)
                    replacement = new handler_entry_read_units<int_Width, int_AddrShift>(descriptor, ukey, (handler_entry_read_units<int_Width, int_AddrShift>)original);
                else
                    replacement = new handler_entry_read_units<int_Width, int_AddrShift>(descriptor, ukey, m_space);

                mappings.emplace_back(new mapping() { original = original, patched = replacement, ukey = ukey });
            }
            else
            {
                replacement.ref_();
            }

            target.unref();
            target = replacement;
        }


        //void populate_passthrough_nomirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read_passthrough<Width, AddrShift> *handler, std::vector<mapping> &mappings);
        //void populate_passthrough_mirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read_passthrough<Width, AddrShift> *handler, std::vector<mapping> &mappings);
        //void passthrough_patch(handler_entry_read_passthrough<Width, AddrShift> *handler, std::vector<mapping> &mappings, handler_entry_read<Width, AddrShift> *&target);
    }
}
