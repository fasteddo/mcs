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
                m_ranges[i] = init;
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


        //uX read(offs_t offset, uX mem_mask) override;
        public override u8 read(offs_t offset, u8 mem_mask)
        {
            return m_dispatch[(offset >> (int)LowBits) & BITMASK].read(offset, mem_mask);
        }


        //void *get_ptr(offs_t offset) const override;


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


        //void dump_map(std::vector<memory_entry> &map) const override;

        //std::string name() const override;

        public override void populate_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read handler)
        {
            offs_t start_entry = start >> (int)LowBits;
            offs_t end_entry = end >> (int)LowBits;
            range_cut_before(ostart-1, (int)start_entry);
            range_cut_after(oend+1, (int)end_entry);

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

        //void populate_mismatched_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, const memory_units_descriptor<Width, AddrShift, Endian> &descriptor, u8 rkey, std::vector<mapping> &mappings) override;
        //void populate_mismatched_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, const memory_units_descriptor<Width, AddrShift, Endian> &descriptor, std::vector<mapping> &mappings) override;
        //void populate_passthrough_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings) override;
        //void populate_passthrough_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings) override;
        //void detach(const std::unordered_set<handler_entry *> &handlers) override;

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


        //void enumerate_references(handler_entry::reflist &refs) const override;


        void populate_nomirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read handler)
        {
            var cur = m_dispatch[entry];
            if (cur.is_dispatch())
            {
                cur.populate_nomirror(start, end, ostart, oend, handler);
            }
            else
            {
                var subdispatch = new handler_entry_read_dispatch((int)LowBits, Width, AddrShift, Endian, m_space, m_ranges[entry], cur);
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
                var subdispatch = new handler_entry_read_dispatch((int)LowBits, Width, AddrShift, Endian, m_space, m_ranges[entry], cur);
                cur.unref();
                m_dispatch[entry] = subdispatch;
                subdispatch.populate_mirror(start, end, ostart, oend, mirror, handler);
            }
        }


        //void populate_mismatched_nomirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, const memory_units_descriptor<Width, AddrShift, Endian> &descriptor, u8 rkey, std::vector<mapping> &mappings);
        //void populate_mismatched_mirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, const memory_units_descriptor<Width, AddrShift, Endian> &descriptor, std::vector<mapping> &mappings);
        //void mismatched_patch(const memory_units_descriptor<Width, AddrShift, Endian> &descriptor, u8 rkey, std::vector<mapping> &mappings, handler_entry_read<Width, AddrShift, Endian> *&target);

        //void populate_passthrough_nomirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings);
        //void populate_passthrough_mirror_subdispatch(offs_t entry, offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings);
        //void passthrough_patch(handler_entry_read_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings, handler_entry_read<Width, AddrShift, Endian> *&target);
    }
}
