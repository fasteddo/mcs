// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using s8 = System.SByte;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;


namespace mame
{
    // handler_entry_read_units/handler_entry_write_units

    // merges/splits an access among multiple handlers (unitmask support)

    //template<int Width, int AddrShift, endianness_t Endian>
    class handler_entry_read_units : handler_entry_read, IDisposable  //class handler_entry_read_units : public handler_entry_read<Width, AddrShift, Endian>
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_read<Width, AddrShift, Endian>;


        u32 SUBUNIT_COUNT;// = 1 << Width;


        struct subunit_info
        {
            public handler_entry m_handler;              // the handler itself, the root to hide the templatization

            public uX m_amask;                // access mask (for filtering access)
            public uX m_dmask;                // data mask (for removal on collisions)
            public s8 m_ashift;               // shift to apply to the address (positive = right)
            public u8 m_offset;               // offset to add to the address post shift
            public u8 m_dshift;               // data shift of the subunit

            public u8 m_width;                // access width (0..3)
            public u8 m_endian;               // endianness

            public subunit_info(handler_entry handler, uX amask, uX dmask, s8 ashift, u8 offset, u8 dshift, u8 width, u8 endian) { this.m_handler = handler; this.m_amask = amask; this.m_dmask = dmask; this.m_ashift = ashift; this.m_offset = offset; this.m_dshift = dshift; this.m_width = width; this.m_endian = endian; }
        }


        subunit_info [] m_subunit_infos;  //subunit_info         m_subunit_infos[SUBUNIT_COUNT]; // subunit information
        uX m_unmap;                        // "unmapped" value to add to reads
        u8 m_subunits;                     // number of subunits


        public handler_entry_read_units(int Width, int AddrShift, endianness_t Endian, memory_units_descriptor descriptor, u8 ukey, address_space space)
            : base(Width, AddrShift, Endian, space, F_UNITS)
        {
            SUBUNIT_COUNT = 1U << Width;
            m_subunit_infos = new subunit_info[SUBUNIT_COUNT];


            m_subunits = 0;


            var entries = descriptor.get_entries_for_key(ukey);
            fill(descriptor, entries);
            Array.Sort(m_subunit_infos, 0, m_subunits, Comparer<subunit_info>.Create((a, b) => { return a.m_offset < b.m_offset ? -1 : 1; }));  //std::sort(m_subunit_infos, m_subunit_infos + m_subunits, [](const subunit_info &a, const subunit_info &b) { return a.m_offset < b.m_offset; });
        }


        public handler_entry_read_units(int Width, int AddrShift, endianness_t Endian, memory_units_descriptor descriptor, u8 ukey, handler_entry_read_units src)
            : base(Width, AddrShift, Endian, src.m_space, F_UNITS)
        {
            SUBUNIT_COUNT = 1U << Width;
            m_subunit_infos = new subunit_info[SUBUNIT_COUNT];


            m_subunits = 0;


            uX fullmask = new uX(Width, 0);
            var entries = descriptor.get_entries_for_key(ukey);
            foreach (var e in entries)
                fullmask |= e.m_dmask;

            for (u32 i = 0; i != src.m_subunits; i++)
            {
                if ((src.m_subunit_infos[i].m_dmask & fullmask) == 0)
                {
                    m_subunit_infos[m_subunits] = src.m_subunit_infos[i];
                    m_subunit_infos[m_subunits].m_handler.ref_();
                    m_subunits++;
                }
            }

            fill(descriptor, entries);
            Array.Sort(m_subunit_infos, 0, m_subunits, Comparer<subunit_info>.Create((a, b) => { return a.m_offset < b.m_offset ? -1 : 1; }));  //std::sort(m_subunit_infos, m_subunit_infos + m_subunits, [](const subunit_info &a, const subunit_info &b) { return a.m_offset < b.m_offset; });
        }


        ~handler_entry_read_units()
        {
            //throw new emu_unimplemented();
#if false
            global.assert(m_isDisposed);  // can remove
#endif
        }

        bool m_isDisposed = false;
        public override void Dispose()
        {
            throw new emu_unimplemented();
#if false
            for(u32 i=0; i != m_subunits; i++)
                m_subunit_infos[i].m_handler->unref();

            m_isDisposed = true;
            base.Dispose();
#endif
        }


        public override uX read(int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX mem_mask)
        {
            this.ref_();

            uX result = m_unmap;
            for (int index = 0; index < m_subunits; index++)
            {
                subunit_info si = m_subunit_infos[index];
                if ((mem_mask & si.m_amask) != 0)
                {
                    offs_t aoffset = (si.m_ashift >= 0 ? offset >> si.m_ashift : offset << si.m_ashift) + si.m_offset;
                    switch (si.m_width)
                    {
                    case 0:
                        if (si.m_endian == (u8)endianness_t.ENDIANNESS_LITTLE)
                            result |= ((handler_entry_read)si.m_handler).read(0, 0, endianness_t.ENDIANNESS_LITTLE, aoffset, mem_mask >> si.m_dshift) << si.m_dshift;  //result |= uX(static_cast<handler_entry_read<0,  0, ENDIANNESS_LITTLE> *>(si.m_handler)->read(aoffset, mem_mask >> si.m_dshift)) << si.m_dshift;
                        else
                            result |= ((handler_entry_read)si.m_handler).read(0, 0, endianness_t.ENDIANNESS_BIG,    aoffset, mem_mask >> si.m_dshift) << si.m_dshift;  //result |= uX(static_cast<handler_entry_read<0,  0, ENDIANNESS_BIG   > *>(si.m_handler)->read(aoffset, mem_mask >> si.m_dshift)) << si.m_dshift;
                        break;
                    case 1:
                        if (si.m_endian == (u8)endianness_t.ENDIANNESS_LITTLE)
                            result |= ((handler_entry_read)si.m_handler).read(1, -1, endianness_t.ENDIANNESS_LITTLE, aoffset, mem_mask >> si.m_dshift) << si.m_dshift;  //result |= uX(static_cast<handler_entry_read<1, -1, ENDIANNESS_LITTLE> *>(si.m_handler)->read(aoffset, mem_mask >> si.m_dshift)) << si.m_dshift;
                        else
                            result |= ((handler_entry_read)si.m_handler).read(1, -1, endianness_t.ENDIANNESS_BIG,    aoffset, mem_mask >> si.m_dshift) << si.m_dshift;  //result |= uX(static_cast<handler_entry_read<1, -1, ENDIANNESS_BIG   > *>(si.m_handler)->read(aoffset, mem_mask >> si.m_dshift)) << si.m_dshift;
                        break;
                    case 2:
                        if (si.m_endian == (u8)endianness_t.ENDIANNESS_LITTLE)
                            result |= ((handler_entry_read)si.m_handler).read(2, -2, endianness_t.ENDIANNESS_LITTLE, aoffset, mem_mask >> si.m_dshift) << si.m_dshift;  //result |= uX(static_cast<handler_entry_read<2, -2, ENDIANNESS_LITTLE> *>(si.m_handler)->read(aoffset, mem_mask >> si.m_dshift)) << si.m_dshift;
                        else
                            result |= ((handler_entry_read)si.m_handler).read(2, -2, endianness_t.ENDIANNESS_BIG,    aoffset, mem_mask >> si.m_dshift) << si.m_dshift;  //result |= uX(static_cast<handler_entry_read<2, -2, ENDIANNESS_BIG   > *>(si.m_handler)->read(aoffset, mem_mask >> si.m_dshift)) << si.m_dshift;
                        break;
                    default:
                        throw new emu_fatalerror("handler_entry_read_units.read() - abort");  //abort();
                    }
                }
            }

            this.unref();
            return result;
        }


        protected override string name() { throw new emu_unimplemented(); }


        protected override void enumerate_references(handler_entry.reflist refs)
        {
            throw new emu_unimplemented();
        }


        void fill(memory_units_descriptor descriptor, std.vector<memory_units_descriptor.entry> entries)
        {
            handler_entry handler = descriptor.get_subunit_handler();
            handler.ref_(entries.size());
            foreach (var e in entries)
                m_subunit_infos[m_subunits++] = new subunit_info( handler, e.m_amask, e.m_dmask, e.m_ashift, e.m_offset, e.m_dshift, descriptor.get_subunit_width(), descriptor.get_subunit_endian() );

            m_unmap = new uX(Width, m_space.unmap());
            for (int i = 0; i < m_subunits; i++)
                m_unmap &= ~m_subunit_infos[i].m_dmask;
        }


        //static std::string m2r(uX mask);
    }


    //template<int Width, int AddrShift, endianness_t Endian>
    class handler_entry_write_units : handler_entry_write, IDisposable  //class handler_entry_write_units : public handler_entry_write<Width, AddrShift, Endian>
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_write<Width, AddrShift, Endian>;


        struct subunit_info
        {
            public handler_entry m_handler;              // the handler itself, the root to hide the templatization

            public uX m_amask;                // access mask (for filtering access)
            public uX m_dmask;                // data mask (for removal on collisions)
            public s8 m_ashift;               // shift to apply to the address (positive = right)
            public u8 m_offset;               // offset to add to the address post shift
            public u8 m_dshift;               // data shift of the subunit

            public u8 m_width;                // access width (0..3)
            public u8 m_endian;               // endianness

            public subunit_info(handler_entry handler, uX amask, uX dmask, s8 ashift, u8 offset, u8 dshift, u8 width, u8 endian) { this.m_handler = handler; this.m_amask = amask; this.m_dmask = dmask; this.m_ashift = ashift; this.m_offset = offset; this.m_dshift = dshift; this.m_width = width; this.m_endian = endian; }
        }


        u32 SUBUNIT_COUNT;// = 1 << Width;


        subunit_info [] m_subunit_infos;  //subunit_info         m_subunit_infos[SUBUNIT_COUNT]; // subunit information
        u8 m_subunits;                     // number of subunits


        public handler_entry_write_units(int Width, int AddrShift, endianness_t Endian, memory_units_descriptor descriptor, u8 ukey, address_space space)
            : base(Width, AddrShift, Endian, space, F_UNITS)
        {
            SUBUNIT_COUNT = 1U << Width;
            m_subunit_infos = new subunit_info[SUBUNIT_COUNT];


            m_subunits = 0;


            var entries = descriptor.get_entries_for_key(ukey);
            fill(descriptor, entries);
            Array.Sort(m_subunit_infos, 0, m_subunits, Comparer<subunit_info>.Create((a, b) => { return a.m_offset < b.m_offset ? -1 : 1; }));  //std::sort(m_subunit_infos, m_subunit_infos + m_subunits, [](const subunit_info &a, const subunit_info &b) { return a.m_offset < b.m_offset; });
        }


        public handler_entry_write_units(int Width, int AddrShift, endianness_t Endian, memory_units_descriptor descriptor, u8 ukey, handler_entry_write_units src)
            : base(Width, AddrShift, Endian, src.m_space, F_UNITS)
        {
            SUBUNIT_COUNT = 1U << Width;
            m_subunit_infos = new subunit_info[SUBUNIT_COUNT];


            m_subunits = 0;


            uX fullmask = new uX(Width, 0);
            var entries = descriptor.get_entries_for_key(ukey);
            foreach (var e in entries)
                fullmask |= e.m_dmask;

            for (u32 i = 0; i != src.m_subunits; i++)
            {
                if ((src.m_subunit_infos[i].m_dmask & fullmask) == 0)
                {
                    m_subunit_infos[m_subunits] = src.m_subunit_infos[i];
                    m_subunit_infos[m_subunits].m_handler.ref_();
                    m_subunits++;
                }
            }

            fill(descriptor, entries);
            Array.Sort(m_subunit_infos, 0, m_subunits, Comparer<subunit_info>.Create((a, b) => { return a.m_offset < b.m_offset ? -1 : 1; }));  //std::sort(m_subunit_infos, m_subunit_infos + m_subunits, [](const subunit_info &a, const subunit_info &b) { return a.m_offset < b.m_offset; });
        }


        ~handler_entry_write_units()
        {
            //throw new emu_unimplemented();
#if false
            global.assert(m_isDisposed);  // can remove
#endif
        }

        bool m_isDisposed = false;
        public override void Dispose()
        {
            throw new emu_unimplemented();
#if false
            for (u32 i = 0; i != m_subunits; i++)
                m_subunit_infos[i].m_handler.unref();

            m_isDisposed = true;
            base.Dispose();
#endif
        }


        public override void write(int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX data, uX mem_mask)  //template<int Width, int AddrShift, int Endian> void handler_entry_write_units<Width, AddrShift, Endian>::write(offs_t offset, uX data, uX mem_mask)
        {
            this.ref_();

            for (int index = 0; index < m_subunits; index++)
            {
                subunit_info si = m_subunit_infos[index];
                if ((mem_mask & si.m_amask) != 0)
                {
                    offs_t aoffset = (si.m_ashift >= 0 ? offset >> si.m_ashift : offset << si.m_ashift) + si.m_offset;
                    switch (si.m_width)
                    {
                    case 0:
                        if (si.m_endian == (u8)endianness_t.ENDIANNESS_LITTLE)
                            ((handler_entry_write)si.m_handler).write(0, 0, endianness_t.ENDIANNESS_LITTLE, aoffset, data >> si.m_dshift, mem_mask >> si.m_dshift);  //static_cast<handler_entry_write<0,  0, ENDIANNESS_LITTLE> *>(si.m_handler)->write(aoffset, data >> si.m_dshift, mem_mask >> si.m_dshift);
                        else
                            ((handler_entry_write)si.m_handler).write(0, 0, endianness_t.ENDIANNESS_BIG,    aoffset, data >> si.m_dshift, mem_mask >> si.m_dshift);  //static_cast<handler_entry_write<0,  0, ENDIANNESS_BIG   > *>(si.m_handler)->write(aoffset, data >> si.m_dshift, mem_mask >> si.m_dshift);
                        break;
                    case 1:
                        if (si.m_endian == (u8)endianness_t.ENDIANNESS_LITTLE)
                            ((handler_entry_write)si.m_handler).write(1, -1, endianness_t.ENDIANNESS_LITTLE, aoffset, data >> si.m_dshift, mem_mask >> si.m_dshift);  //static_cast<handler_entry_write<1, -1, ENDIANNESS_LITTLE> *>(si.m_handler)->write(aoffset, data >> si.m_dshift, mem_mask >> si.m_dshift);
                        else
                            ((handler_entry_write)si.m_handler).write(1, -1, endianness_t.ENDIANNESS_BIG,    aoffset, data >> si.m_dshift, mem_mask >> si.m_dshift);  //static_cast<handler_entry_write<1, -1, ENDIANNESS_BIG   > *>(si.m_handler)->write(aoffset, data >> si.m_dshift, mem_mask >> si.m_dshift);
                        break;
                    case 2:
                        if (si.m_endian == (u8)endianness_t.ENDIANNESS_LITTLE)
                            ((handler_entry_write)si.m_handler).write(2, -2, endianness_t.ENDIANNESS_LITTLE, aoffset, data >> si.m_dshift, mem_mask >> si.m_dshift);  //static_cast<handler_entry_write<2, -2, ENDIANNESS_LITTLE> *>(si.m_handler)->write(aoffset, data >> si.m_dshift, mem_mask >> si.m_dshift);
                        else
                            ((handler_entry_write)si.m_handler).write(2, -2, endianness_t.ENDIANNESS_BIG,    aoffset, data >> si.m_dshift, mem_mask >> si.m_dshift);  //static_cast<handler_entry_write<2, -2, ENDIANNESS_BIG   > *>(si.m_handler)->write(aoffset, data >> si.m_dshift, mem_mask >> si.m_dshift);
                        break;
                    default:
                        throw new emu_fatalerror("handler_entry_write_units.write() - abort");  //abort();
                    }
                }
            }

            this.unref();
        }


        protected override string name() { throw new emu_unimplemented(); }


        protected override void enumerate_references(handler_entry.reflist refs)
        {
            throw new emu_unimplemented();
        }


        void fill(memory_units_descriptor descriptor, std.vector<memory_units_descriptor.entry> entries)
        {
            handler_entry handler = descriptor.get_subunit_handler();
            handler.ref_(entries.size());
            foreach (var e in entries)
                m_subunit_infos[m_subunits++] = new subunit_info( handler, e.m_amask, e.m_dmask, e.m_ashift, e.m_offset, e.m_dshift, descriptor.get_subunit_width(), descriptor.get_subunit_endian() );
        }


        //static std::string m2r(uX mask);
    }
}
