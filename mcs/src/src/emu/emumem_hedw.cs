// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;
using unsigned = System.UInt32;


namespace mame
{
    // handler_entry_write_dispatch

    // dispatches an access among multiple handlers indexed on part of the address

    //template<int HighBits, int Width, int AddrShift, endianness_t Endian>
    class handler_entry_write_dispatch<int_HighBits, int_Width, int_AddrShift, endianness_t_Endian> : handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian>
        where int_HighBits : int_constant, new()
        where int_Width : int_constant, new()
        where int_AddrShift : int_constant, new()
        where endianness_t_Endian : endianness_t_constant, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using mapping = typename handler_entry_write<Width, AddrShift, Endian>::mapping;


        static readonly int HighBits = new int_HighBits().value;


        class uint32_constant_COUNT : uint32_constant { public UInt32 value { get { return COUNT; } } }

        //using std::array<handler_entry_write<Width, AddrShift, Endian> *, COUNT>::array;
        class handler_array : std.array<handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian>, uint32_constant_COUNT>
        {
            public handler_array()
            {
                std.fill(this, () => { return null; });
            }
        }


        //using std::array<handler_entry::range, COUNT>::array;
        class range_array : std.array<handler_entry.range, uint32_constant_COUNT>
        {
            public range_array()
            {
                std.fill(this, () => { return new handler_entry.range() { start = 0, end = 0 }; });  //std::fill(this->begin(), this->end(), handler_entry::range{ 0, 0 });
            }
        }


        static readonly int Level    = emumem_global.handler_entry_dispatch_level(HighBits);
        static readonly u32 LowBits  = (u32)emumem_global.handler_entry_dispatch_lowbits(HighBits, Width, AddrShift);
        static readonly u32 BITCOUNT = (u32)HighBits > LowBits ? (u32)HighBits - LowBits : 0;
        static readonly u32 COUNT    = 1U << (int)BITCOUNT;
        static readonly offs_t BITMASK  = g.make_bitmask32(BITCOUNT);
        static readonly offs_t LOWMASK  = g.make_bitmask32(LowBits);
        static readonly offs_t HIGHMASK = g.make_bitmask32(HighBits) ^ LOWMASK;
        static readonly offs_t UPMASK   = ~g.make_bitmask32(HighBits);

        public class int_constant_Level : int_constant { public int value { get { return Level; } } }
        public class int_constant_LowBits : int_constant { public int value { get { return (int)LowBits; } } }


        memory_view m_view;

        std.vector<handler_array> m_dispatch_array = new std.vector<handler_array>();
        std.vector<range_array> m_ranges_array = new std.vector<range_array>();

        Pointer<handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian>> m_a_dispatch;  //handler_entry_write<Width, AddrShift, Endian> **m_a_dispatch;
        Pointer<handler_entry.range> m_a_ranges;  //handler_entry::range *m_a_ranges;

        Pointer<handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian>> m_u_dispatch;  //handler_entry_write<Width, AddrShift, Endian> **m_u_dispatch;
        Pointer<handler_entry.range> m_u_ranges;  //handler_entry::range *m_u_ranges;


        public handler_entry_write_dispatch(address_space space, handler_entry.range init, handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> handler) : base(space, handler_entry.F_DISPATCH)
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
                handler = space.get_unmap_w<int_Width, int_AddrShift, endianness_t_Endian>();

            handler.ref_((s32)COUNT);

            for (unsigned i = 0; i != COUNT; i++)
            {
                m_u_dispatch[i] = handler;
                m_u_ranges[i] = new range(init);
            }
        }


        //handler_entry_write_dispatch(address_space *space, memory_view &view);


        handler_entry_write_dispatch(handler_entry_write_dispatch<int_HighBits, int_Width, int_AddrShift, endianness_t_Endian> src)
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


        ~handler_entry_write_dispatch()
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


        public override void write(offs_t offset, uX data, uX mem_mask)
        {
            emumem_global.dispatch_write<int_constant_Level, int_Width, int_AddrShift, endianness_t_Endian>(HIGHMASK, offset, data, mem_mask, m_a_dispatch);
        }


        public override object get_ptr(offs_t offset)
        {
            throw new emu_unimplemented();
        }


        protected override void lookup(offs_t address, ref offs_t start, ref offs_t end, ref handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> handler)
        {
            throw new emu_unimplemented();
        }


        protected override void dump_map(std.vector<memory_entry> map)
        {
            throw new emu_unimplemented();
        }


        protected override string name() { throw new emu_unimplemented(); }


        public override void populate_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> handler)
        {
            offs_t start_entry = (start & HIGHMASK) >> (int)LowBits;
            offs_t end_entry = (end & HIGHMASK) >> (int)LowBits;
            range_cut_before(ostart-1, (int)start_entry);
            range_cut_after(oend+1, (int)end_entry);

            if (LowBits <= Width + AddrShift)
            {
                if (handler.is_view())
                    handler.init_handlers(start_entry, end_entry, LowBits, m_u_dispatch, m_u_ranges);
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
                        handler.init_handlers(start_entry, end_entry, LowBits, m_u_dispatch, m_u_ranges);
                    m_u_dispatch[start_entry].unref();
                    m_u_dispatch[start_entry] = handler;
                    m_u_ranges[start_entry].set(ostart, oend);
                }
                else
                {
                    populate_nomirror_subdispatch(start_entry, start & LOWMASK, end & LOWMASK, ostart, oend, handler);
                }
            }
            else
            {
                if ((start & LOWMASK) != 0)
                {
                    populate_nomirror_subdispatch(start_entry, start & LOWMASK, LOWMASK, ostart, oend, handler);
                    start_entry++;
                    if (start_entry <= end_entry)
                        handler.ref_();
                }

                if ((end & LOWMASK) != LOWMASK)
                {
                    populate_nomirror_subdispatch(end_entry, 0, end & LOWMASK, ostart, oend, handler);
                    end_entry--;
                    if (start_entry <= end_entry)
                        handler.ref_();
                }

                if (start_entry <= end_entry)
                {
                    if (handler.is_view())
                        handler.init_handlers(start_entry, end_entry, LowBits, m_u_dispatch, m_u_ranges);
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


        public override void populate_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> handler)
        {
            offs_t hmirror = mirror & HIGHMASK;
            offs_t lmirror = mirror & LOWMASK;

            if (lmirror != 0)
            {
                // If lmirror is non-zero, then each mirror instance is a single entry
                offs_t add = 1 + ~hmirror;
                offs_t offset = 0;
                offs_t base_entry = start >> (int)LowBits;
                start &= LOWMASK;
                end &= LOWMASK;
                do
                {
                    if (offset != 0)
                        handler.ref_();

                    populate_mirror_subdispatch(base_entry | (offset >> (int)LowBits), start, end, ostart | offset, oend | offset, lmirror, handler);
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


        public override void populate_mismatched_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, memory_units_descriptor<int_Width, int_AddrShift, endianness_t_Endian> descriptor, u8 rkey, std.vector<mapping> mappings)
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
                    m_u_ranges[ent].intersect(ostart, oend);
                }
            }
            else if (start_entry == end_entry)
            {
                if ((start & LOWMASK) == 0 && (end & LOWMASK) == LOWMASK)
                {
                    if (m_u_dispatch[start_entry].is_dispatch())
                    {
                        m_u_dispatch[start_entry].populate_mismatched_nomirror(start & LOWMASK, end & LOWMASK, ostart, oend, descriptor, rkey, mappings);
                    }
                    else
                    {
                        var temp = m_u_dispatch[start_entry];  mismatched_patch(descriptor, rkey, mappings, ref temp);  m_u_dispatch[start_entry] = temp;  //mismatched_patch(descriptor, rkey, mappings, m_u_dispatch[start_entry]);
                        m_u_ranges[start_entry].intersect(ostart, oend);
                    }
                }
                else
                {
                    populate_mismatched_nomirror_subdispatch(start_entry, start & LOWMASK, end & LOWMASK, ostart, oend, descriptor, rkey, mappings);
                }
            }
            else
            {
                if ((start & LOWMASK) != 0)
                {
                    populate_mismatched_nomirror_subdispatch(start_entry, start & LOWMASK, LOWMASK, ostart, oend, descriptor, (u8)(rkey & unchecked((u8)~handler_entry.END)), mappings);
                    start_entry++;
                    rkey &= unchecked((u8)~handler_entry.START);
                }

                if ((end & LOWMASK) != LOWMASK)
                {
                    populate_mismatched_nomirror_subdispatch(end_entry, 0, end & LOWMASK, ostart, oend, descriptor, (u8)(rkey & unchecked((u8)~handler_entry.START)), mappings);
                    end_entry--;
                    rkey &= unchecked((u8)~handler_entry.END);
                }

                if (start_entry <= end_entry)
                {
                    for(offs_t ent = start_entry; ent <= end_entry; ent++)
                    {
                        u8 rkey1 = rkey;
                        if (ent != start_entry)
                            rkey1 &= unchecked((u8)~handler_entry.START);
                        if (ent != end_entry)
                            rkey1 &= unchecked((u8)~handler_entry.END);

                        if (m_u_dispatch[ent].is_dispatch())
                        {
                            m_u_dispatch[ent].populate_mismatched_nomirror(start & LOWMASK, end & LOWMASK, ostart, oend, descriptor, rkey1, mappings);
                        }
                        else
                        {
                            var temp = m_u_dispatch[ent];  mismatched_patch(descriptor, rkey1, mappings, ref temp);  m_u_dispatch[ent] = temp;  //mismatched_patch(descriptor, rkey1, mappings, m_u_dispatch[ent]);
                            m_u_ranges[ent].intersect(ostart, oend);
                        }
                    }
                }
            }
        }

        public override void populate_mismatched_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, memory_units_descriptor<int_Width, int_AddrShift, endianness_t_Endian> descriptor, std.vector<mapping> mappings)
        {
            offs_t hmirror = mirror & HIGHMASK;
            offs_t lmirror = mirror & LOWMASK;

            if (lmirror != 0)
            {
                // If lmirror is non-zero, then each mirror instance is a single entry
                offs_t add = 1 + ~hmirror;
                offs_t offset = 0;
                offs_t base_entry = start >> (int)LowBits;
                start &= LOWMASK;
                end &= LOWMASK;
                do
                {
                    populate_mismatched_mirror_subdispatch(base_entry | (offset >> (int)LowBits), start, end, ostart | offset, oend | offset, lmirror, descriptor, mappings);
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
                    populate_mismatched_nomirror(start | offset, end | offset, ostart | offset, oend | offset, descriptor, handler_entry.START|handler_entry.END, mappings);
                    offset = (offset + add) & hmirror;
                } while (offset != 0);
            }
        }

        protected override void populate_passthrough_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_write_passthrough<int_Width, int_AddrShift, endianness_t_Endian> handler, std.vector<mapping> mappings)
        {
            throw new emu_unimplemented();
        }

        protected override void populate_passthrough_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_write_passthrough<int_Width, int_AddrShift, endianness_t_Endian> handler, std.vector<mapping> mappings)
        {
            throw new emu_unimplemented();
        }


        public override void detach(std.unordered_set<handler_entry> handlers)
        {
            throw new emu_unimplemented();
        }


        void range_cut_before(offs_t address) { range_cut_before(address, (int)COUNT); }
        void range_cut_before(offs_t address, int start)
        {
            while (--start >= 0 && m_u_dispatch[start] != null)
            {
                if ((int)LowBits > -AddrShift && m_u_dispatch[start].is_dispatch())
                {
                    ((handler_entry_write_dispatch<int_constant_LowBits, int_Width, int_AddrShift, endianness_t_Endian>)m_u_dispatch[start]).range_cut_before(address);  //static_cast<handler_entry_write_dispatch<LowBits, Width, AddrShift, Endian> *>(m_dispatch[start])->range_cut_before(address);
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
                    ((handler_entry_write_dispatch<int_constant_LowBits, int_Width, int_AddrShift, endianness_t_Endian>)m_u_dispatch[start]).range_cut_after(address);  //static_cast<handler_entry_write_dispatch<LowBits, Width, AddrShift, Endian> *>(m_dispatch[start])->range_cut_after(address);
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
        public override Pointer<handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian>> get_dispatch()  //const handler_entry_write<Width, AddrShift, Endian> *const *get_dispatch() const override;
        {
            return m_a_dispatch;
        }


        public override void select_a(int id)  //template<int HighBits, int Width, int AddrShift, endianness_t Endian> void handler_entry_write_dispatch<HighBits, Width, AddrShift, Endian>::select_a(int id)
        {
            u32 i = (u32)id + 1;
            if (i >= m_dispatch_array.size())
                fatalerror("out-of-range view selection.");

            m_a_ranges = m_ranges_array[i].data();
            m_a_dispatch = m_dispatch_array[i].data();
        }


        public override void select_u(int id)  //template<int HighBits, int Width, int AddrShift, endianness_t Endian> void handler_entry_write_dispatch<HighBits, Width, AddrShift, Endian>::select_u(int id)
        {
            u32 i = (u32)id + 1;
            if (i > m_dispatch_array.size())
            {
                fatalerror("out-of-range view update selection.");
            }
            else if (i == m_dispatch_array.size())
            {
                u32 aid = (u32)(m_a_dispatch.Offset - m_dispatch_array.data().Offset);  //u32 aid = (handler_array *)m_a_dispatch - m_dispatch_array.data();

                m_dispatch_array.resize((int)i + 1);
                m_ranges_array.resize((int)i + 1);
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


        public override void init_handlers(offs_t start_entry, offs_t end_entry, u32 lowbits, Pointer<handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian>> dispatch, Pointer<handler_entry.range> ranges)  //template<int HighBits, int Width, int AddrShift, endianness_t Endian> void handler_entry_write_dispatch<HighBits, Width, AddrShift, Endian>::init_handlers(offs_t start_entry, offs_t end_entry, u32 lowbits, handler_entry_write<Width, AddrShift, Endian> **dispatch, handler_entry::range *ranges)
        {
            if (m_view == null)
                fatalerror("init_handlers called on non-view handler_entry_write_dispatch.");
            if (!m_dispatch_array.empty())
                fatalerror("init_handlers called twice on handler_entry_write_dispatch.");

            m_ranges_array.resize(1);
            m_dispatch_array.resize(1);
            m_a_ranges = m_ranges_array[0].data();
            m_a_dispatch = m_dispatch_array[0].data();
            m_u_ranges = m_ranges_array[0].data();
            m_u_dispatch = m_dispatch_array[0].data();

            Func<handler_entry.range, handler_entry.range> filter = (handler_entry.range r) =>  //auto filter = [s = m_view->m_addrstart, e = m_view->m_addrend] (handler_entry::range r) {
            {
                r.intersect(m_view.m_addrstart, m_view.m_addrend);  //r.intersect(s, e);
                return r;
            };

            if (lowbits != LowBits)
            {
                u32 dt = lowbits - LowBits;
                u32 ne = 1U << (int)dt;
                for (offs_t entry = start_entry; entry <= end_entry; entry++)
                {
                    dispatch[entry].ref_((int)ne);
                    u32 e0 = (entry << (int)dt) & BITMASK;
                    for (offs_t e = 0; e != ne; e++)
                    {
                        m_u_dispatch[e0 | e] = dispatch[entry];
                        m_u_ranges[e0 | e] = filter(ranges[entry]);
                    }
                }
            }
            else
            {
                for (offs_t entry = start_entry; entry <= end_entry; entry++)
                {
                    m_u_dispatch[entry & BITMASK] = dispatch[entry];
                    m_u_ranges[entry & BITMASK] = filter(ranges[entry]);
                    dispatch[entry].ref_();
                }
            }
        }


        public override handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> dup()  //template<int HighBits, int Width, int AddrShift, endianness_t Endian> handler_entry_write<Width, AddrShift, Endian> *handler_entry_write_dispatch<HighBits, Width, AddrShift, Endian>::dup()
        {
            if (m_view != null)
            {
                base.ref_();
                return this;
            }

            return new handler_entry_write_dispatch<int_HighBits, int_Width, int_AddrShift, endianness_t_Endian>(this);
        }


        void populate_nomirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> handler)
        {
            var cur = m_u_dispatch[entry];
            if (cur.is_dispatch())
            {
                cur.populate_nomirror(start, end, ostart, oend, handler);
            }
            else
            {
                var subdispatch = new handler_entry_write_dispatch<int_constant_LowBits, int_Width, int_AddrShift, endianness_t_Endian>(m_space, m_u_ranges[entry], cur);
                cur.unref();
                m_u_dispatch[entry] = subdispatch;
                subdispatch.populate_nomirror(start, end, ostart, oend, handler);
            }
        }


        void populate_mirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> handler)
        {
            var cur = m_u_dispatch[entry];
            if (cur.is_dispatch())
            {
                cur.populate_mirror(start, end, ostart, oend, mirror, handler);
            }
            else
            {
                var subdispatch = new handler_entry_write_dispatch<int_constant_LowBits, int_Width, int_AddrShift, endianness_t_Endian>(m_space, m_u_ranges[entry], cur);
                cur.unref();
                m_u_dispatch[entry] = subdispatch;
                subdispatch.populate_mirror(start, end, ostart, oend, mirror, handler);
            }
        }


        void populate_mismatched_nomirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, memory_units_descriptor<int_Width, int_AddrShift, endianness_t_Endian> descriptor, u8 rkey, std.vector<mapping> mappings)
        {
            var cur = m_u_dispatch[entry];
            if (cur.is_dispatch())
            {
                cur.populate_mismatched_nomirror(start, end, ostart, oend, descriptor, rkey, mappings);
            }
            else
            {
                var subdispatch = new handler_entry_write_dispatch<int_constant_LowBits, int_Width, int_AddrShift, endianness_t_Endian>(m_space, m_u_ranges[entry], cur);
                cur.unref();
                m_u_dispatch[entry] = subdispatch;
                subdispatch.populate_mismatched_nomirror(start, end, ostart, oend, descriptor, rkey, mappings);
            }
        }


        void populate_mismatched_mirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, memory_units_descriptor<int_Width, int_AddrShift, endianness_t_Endian> descriptor, std.vector<mapping> mappings)
        {
            var cur = m_u_dispatch[entry];
            if (cur.is_dispatch())
            {
                cur.populate_mismatched_mirror(start, end, ostart, oend, mirror, descriptor, mappings);
            }
            else
            {
                var subdispatch = new handler_entry_write_dispatch<int_HighBits, int_Width, int_AddrShift, endianness_t_Endian>(m_space, m_u_ranges[entry], cur);
                cur.unref();
                m_u_dispatch[entry] = subdispatch;
                subdispatch.populate_mismatched_mirror(start, end, ostart, oend, mirror, descriptor, mappings);
            }
        }


        void mismatched_patch(memory_units_descriptor<int_Width, int_AddrShift, endianness_t_Endian> descriptor, u8 rkey, std.vector<mapping> mappings, ref handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> target)
        {
            u8 ukey = descriptor.rkey_to_ukey(rkey);
            handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> original = target.is_units() ? target : null;
            handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> replacement = null;
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
                    replacement = new handler_entry_write_units<int_Width, int_AddrShift, endianness_t_Endian>(descriptor, ukey, (handler_entry_write_units<int_Width, int_AddrShift, endianness_t_Endian>)original);
                else
                    replacement = new handler_entry_write_units<int_Width, int_AddrShift, endianness_t_Endian>(descriptor, ukey, m_space);

                mappings.emplace_back(new mapping()  { original = original, patched = replacement, ukey = ukey });
            }
            else
            {
                replacement.ref_();
            }

            target.unref();
            target = replacement;
        }


        //void populate_passthrough_nomirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_write_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings);
        //void populate_passthrough_mirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_write_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings);
        //void passthrough_patch(handler_entry_write_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings, handler_entry_write<Width, AddrShift, Endian> *&target);
    }
}
