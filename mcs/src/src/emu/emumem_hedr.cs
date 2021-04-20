// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using s32 = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;


namespace mame
{
    // handler_entry_read_dispatch

    // dispatches an access among multiple handlers indexed on part of the address

    //template<int HighBits, int Width, int AddrShift, int Endian>
    class handler_entry_read_dispatch : handler_entry_read, IDisposable
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_read<Width, AddrShift, Endian>;
        //using mapping = typename inh::mapping;


        // template parameters
        int HighBits;


        u32 LowBits;  // = emu::detail::handler_entry_dispatch_lowbits(HighBits, Width, AddrShift);
        u32 BITCOUNT; // = HighBits > LowBits ? HighBits - LowBits : 0;
        u32 COUNT; // = 1 << BITCOUNT;
        offs_t BITMASK; // = make_bitmask<offs_t>(BITCOUNT);
        offs_t LOWMASK; // = make_bitmask<offs_t>(LowBits);
        offs_t HIGHMASK; // = make_bitmask<offs_t>(HighBits) ^ LOWMASK;
        //static constexpr offs_t UPMASK   = ~make_bitmask<offs_t>(HighBits);


        handler_entry_read [] m_dispatch;  //handler_entry_read<Width, AddrShift, Endian> *m_dispatch[COUNT];
        handler_entry.range [] m_ranges;


        public handler_entry_read_dispatch(int HighBits, int Width, int AddrShift, int Endian, address_space space, handler_entry.range init, handler_entry_read handler) : base(Width, AddrShift, Endian, space, handler_entry.F_DISPATCH)
        {
            this.HighBits = HighBits;


            LowBits  = (u32)handler_entry_dispatch_lowbits(HighBits, Width, AddrShift);
            BITCOUNT = (u32)HighBits > LowBits ? (u32)HighBits - LowBits : 0;
            COUNT    = 1U << (int)BITCOUNT;
            BITMASK  = make_bitmask32(BITCOUNT);
            LOWMASK  = make_bitmask32(LowBits);
            HIGHMASK = make_bitmask32((u32)HighBits) ^ LOWMASK;


            if (handler == null)
                handler = space.get_unmap_r();

            handler.ref_((s32)COUNT);

            m_dispatch = new handler_entry_read[COUNT];
            m_ranges = new handler_entry.range[COUNT];
            for (UInt32 i = 0; i != COUNT; i++)
            {
                m_dispatch[i] = handler;
                m_ranges[i] = new range() { start = init.start, end = init.end };
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
            for (UInt32 i = 0; i != COUNT; i++)
                m_dispatch[i].unref();

            m_isDisposed = true;
            base.Dispose();
        }


        public override uX read(int WidthOverride, int AddrShiftOverride, int EndianOverride, offs_t offset, uX mem_mask)
        {
            return m_dispatch[(offset >> (int)LowBits) & BITMASK].read(WidthOverride, AddrShiftOverride, EndianOverride, offset, mem_mask);
        }


        protected override object get_ptr(offs_t offset)
        {
            throw new emu_unimplemented();
        }


        public override void lookup(offs_t address, ref offs_t start, ref offs_t end, ref handler_entry_read handler)
        {
            offs_t slot = (address >> (int)LowBits) & BITMASK;
            var h = m_dispatch[slot];
            if (h.is_dispatch())
            {
                h.lookup(address, ref start, ref end, ref handler);
            }
            else
            {
                start = m_ranges[slot].start;
                end = m_ranges[slot].end;
                handler = h;
            }
        }


        protected override void dump_map(std.vector<memory_entry> map)
        {
            throw new emu_unimplemented();
        }


        protected override string name() { throw new emu_unimplemented(); }


        public override void populate_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read handler)
        {
            offs_t start_entry = start >> (int)LowBits;
            offs_t end_entry = end >> (int)LowBits;
            range_cut_before(ostart - 1, (int)start_entry);
            range_cut_after(oend + 1, (int)end_entry);

            if (LowBits <= Width + AddrShift)
            {
                handler.ref_((int)(end_entry - start_entry));
                for (offs_t ent = start_entry; ent <= end_entry; ent++)
                {
                    m_dispatch[ent].unref();
                    m_dispatch[ent] = handler;
                    m_ranges[ent].set(ostart, oend);
                }
            }
            else if (start_entry == end_entry)
            {
                if ((start & LOWMASK) == 0 && (end & LOWMASK) == LOWMASK)
                {
                    m_dispatch[start_entry].unref();
                    m_dispatch[start_entry] = handler;
                    m_ranges[start_entry].set(ostart, oend);
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
                    handler.ref_((int)(end_entry - start_entry));
                    for (offs_t ent = start_entry; ent <= end_entry; ent++)
                    {
                        m_dispatch[ent].unref();
                        m_dispatch[ent] = handler;
                        m_ranges[ent].set(ostart, oend);
                    }
                }
            }
        }

        public override void populate_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read handler)
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


        public override void populate_mismatched_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, memory_units_descriptor descriptor, u8 rkey, std.vector<mapping> mappings)
        {
            offs_t start_entry = start >> (int)LowBits;
            offs_t end_entry = end >> (int)LowBits;
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
                    mismatched_patch(descriptor, rkey1, mappings, ref m_dispatch[ent]);
                    m_ranges[ent].set(ostart, oend);
                }
            }
            else if (start_entry == end_entry)
            {
                if ((start & LOWMASK) == 0 && (end & LOWMASK) == LOWMASK)
                {
                    if (m_dispatch[start_entry].is_dispatch())
                    {
                        m_dispatch[start_entry].populate_mismatched_nomirror(start & LOWMASK, end & LOWMASK, ostart, oend, descriptor, rkey, mappings);
                    }
                    else
                    {
                        mismatched_patch(descriptor, rkey, mappings, ref m_dispatch[start_entry]);
                        m_ranges[start_entry].set(ostart, oend);
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
                    populate_mismatched_nomirror_subdispatch(start_entry, start & LOWMASK, LOWMASK, ostart, oend, descriptor, (u8)(rkey & ~handler_entry.END), mappings);
                    start_entry++;
                    rkey &= unchecked((u8)~handler_entry.START);
                }

                if ((end & LOWMASK) != LOWMASK)
                {
                    populate_mismatched_nomirror_subdispatch(end_entry, 0, end & LOWMASK, ostart, oend, descriptor, (u8)(rkey & ~handler_entry.START), mappings);
                    end_entry--;
                    rkey &= unchecked((u8)~handler_entry.END);
                }

                if (start_entry <= end_entry)
                {
                    for (offs_t ent = start_entry; ent <= end_entry; ent++)
                    {
                        u8 rkey1 = rkey;
                        if (ent != start_entry)
                            rkey1 &= unchecked((u8)~handler_entry.START);
                        if (ent != end_entry)
                            rkey1 &= unchecked((u8)~handler_entry.END);

                        if (m_dispatch[ent].is_dispatch())
                        {
                            m_dispatch[ent].populate_mismatched_nomirror(start & LOWMASK, end & LOWMASK, ostart, oend, descriptor, rkey1, mappings);
                        }
                        else
                        {
                            mismatched_patch(descriptor, rkey1, mappings, ref m_dispatch[ent]);
                            m_ranges[ent].set(ostart, oend);
                        }
                    }
                }
            }
        }


        public override void populate_mismatched_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, memory_units_descriptor descriptor, std.vector<mapping> mappings)
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


        protected override void populate_passthrough_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read_passthrough handler, std.vector<mapping> mappings)
        {
            throw new emu_unimplemented();
        }

        protected override void populate_passthrough_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read_passthrough handler, std.vector<mapping> mappings)
        {
            throw new emu_unimplemented();
        }


        protected override void detach(std.unordered_set<handler_entry> handlers)
        {
            throw new emu_unimplemented();
        }


        void range_cut_before(offs_t address) { range_cut_before(address, (int)COUNT); }
        void range_cut_before(offs_t address, int start)  // = COUNT)
        {
            while (--start >= 0)
            {
                if ((int)LowBits > -AddrShift && m_dispatch[start].is_dispatch())
                {
                    ((handler_entry_read_dispatch)m_dispatch[start]).range_cut_before(address);  //static_cast<handler_entry_read_dispatch<LowBits, Width, AddrShift, Endian> *>(m_dispatch[start]).range_cut_before(address);
                    break;
                }

                if (m_ranges[start].end <= address)
                    break;

                m_ranges[start].end = address;
            }
        }

        void range_cut_after(offs_t address, int start = -1)
        {
            while (++start < COUNT)
            {
                if ((int)LowBits > -AddrShift && m_dispatch[start].is_dispatch())
                {
                    ((handler_entry_read_dispatch)m_dispatch[start]).range_cut_after(address);  //static_cast<handler_entry_read_dispatch<LowBits, Width, AddrShift, Endian> *>(m_dispatch[start]).range_cut_after(address);
                    break;
                }

                if (m_ranges[start].start >= address)
                    break;

                m_ranges[start].start = address;
            }
        }


        protected override void enumerate_references(handler_entry.reflist refs)
        {
            throw new emu_unimplemented();
        }


        void populate_nomirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read handler)
        {
            var cur = m_dispatch[entry];
            if (cur.is_dispatch())
            {
                cur.populate_nomirror(start, end, ostart, oend, handler);
            }
            else
            {
                var subdispatch = new handler_entry_read_dispatch((int)LowBits, Width, AddrShift, Endian, m_space, m_ranges[entry], cur);  //auto subdispatch = new handler_entry_read_dispatch<LowBits, Width, AddrShift, Endian>(handler_entry::m_space, m_ranges[entry], cur);
                cur.unref();
                m_dispatch[entry] = subdispatch;
                subdispatch.populate_nomirror(start, end, ostart, oend, handler);
            }
        }


        void populate_mirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read handler)
        {
            var cur = m_dispatch[entry];
            if (cur.is_dispatch())
            {
                cur.populate_mirror(start, end, ostart, oend, mirror, handler);
            }
            else
            {
                var subdispatch = new handler_entry_read_dispatch((int)LowBits, Width, AddrShift, Endian, m_space, m_ranges[entry], cur);  //auto subdispatch = new handler_entry_read_dispatch<LowBits, Width, AddrShift, Endian>(handler_entry::m_space, m_ranges[entry], cur);
                cur.unref();
                m_dispatch[entry] = subdispatch;
                subdispatch.populate_mirror(start, end, ostart, oend, mirror, handler);
            }
        }


        void populate_mismatched_nomirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, memory_units_descriptor descriptor, u8 rkey, std.vector<mapping> mappings)
        {
            var cur = m_dispatch[entry];
            if (cur.is_dispatch())
            {
                cur.populate_mismatched_nomirror(start, end, ostart, oend, descriptor, rkey, mappings);
            }
            else
            {
                var subdispatch = new handler_entry_read_dispatch((int)LowBits, Width, AddrShift, Endian, m_space, m_ranges[entry], cur);  //auto subdispatch = new handler_entry_read_dispatch<LowBits, Width, AddrShift, Endian>(handler_entry::m_space, m_ranges[entry], cur);
                cur.unref();
                m_dispatch[entry] = subdispatch;
                subdispatch.populate_mismatched_nomirror(start, end, ostart, oend, descriptor, rkey, mappings);
            }
        }


        void populate_mismatched_mirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, memory_units_descriptor descriptor, std.vector<mapping> mappings)
        {
            var cur = m_dispatch[entry];
            if (cur.is_dispatch())
            {
                cur.populate_mismatched_mirror(start, end, ostart, oend, mirror, descriptor, mappings);
            }
            else
            {
                var subdispatch = new handler_entry_read_dispatch((int)LowBits, Width, AddrShift, Endian, m_space, m_ranges[entry], cur);  //auto subdispatch = new handler_entry_read_dispatch<LowBits, Width, AddrShift, Endian>(handler_entry::m_space, m_ranges[entry], cur);
                cur.unref();
                m_dispatch[entry] = subdispatch;
                subdispatch.populate_mismatched_mirror(start, end, ostart, oend, mirror, descriptor, mappings);
            }
        }


        void mismatched_patch(memory_units_descriptor descriptor, u8 rkey, std.vector<mapping> mappings, ref handler_entry_read target)
        {
            u8 ukey = descriptor.rkey_to_ukey(rkey);
            handler_entry_read original = target.is_units() ? target : null;
            handler_entry_read replacement = null;
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
                    replacement = new handler_entry_read_units(Width, AddrShift, Endian, descriptor, ukey, (handler_entry_read_units)original);
                else
                    replacement = new handler_entry_read_units(Width, AddrShift, Endian, descriptor, ukey, m_space);

                mappings.emplace_back(new mapping(original, replacement, ukey));
            }
            else
            {
                replacement.ref_();
            }

            target.unref();
            target = replacement;
        }


        //void populate_passthrough_nomirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings);
        //void populate_passthrough_mirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings);
        //void passthrough_patch(handler_entry_read_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings, handler_entry_read<Width, AddrShift, Endian> *&target);
    }
}
