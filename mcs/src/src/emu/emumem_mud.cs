// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using s8 = System.SByte;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    // Descriptors for subunit support

    //template<int Width, int AddrShift, endianness_t Endian>
    public class memory_units_descriptor<int_Width, int_AddrShift, endianness_t_Endian> : global_object
        where int_Width : int_constant, new()
        where int_AddrShift : int_constant, new()
        where endianness_t_Endian : endianness_t_constant, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        protected static readonly int Width = new int_Width().value;
        protected static readonly int AddrShift = new int_AddrShift().value;
        protected static readonly endianness_t Endian = new endianness_t_Endian().value;


        public struct entry
        {
            public uX m_amask;
            public uX m_dmask;
            public s8 m_ashift;
            public u8 m_dshift;
            public u8 m_offset;

            public entry(uX amask, uX dmask, s8 ashift, u8 dshift, u8 offset) { m_amask = amask; m_dmask = dmask; m_ashift = ashift; m_dshift = dshift; m_offset = offset; }
        }


        std.map<u8, std.vector<entry>> m_entries_for_key = new std.map<u8, std.vector<entry>>();
        offs_t m_addrstart;
        offs_t m_addrend;
        offs_t m_handler_start;
        offs_t m_handler_mask;
        handler_entry m_handler;
        std.array<u8, uint32_constant_4> m_keymap = new std.array<u8, uint32_constant_4>();  //std::array<u8, 4> m_keymap;
        u8 m_access_width;
        u8 m_access_endian;


        //template<int Width, int AddrShift, int Endian>
        public memory_units_descriptor(u8 access_width, u8 access_endian, handler_entry handler, offs_t addrstart, offs_t addrend, offs_t mask, uX unitmask, int cswidth)  //memory_units_descriptor(u8 access_width, u8 access_endian, handler_entry handler, offs_t addrstart, offs_t addrend, offs_t mask, uX unitmask, int cswidth);
        {
            m_handler = handler;
            m_access_width = access_width;
            m_access_endian = access_endian;


            u32 bits_per_access = 8U << access_width;
            u32 NATIVE_MASK = Width + AddrShift >= 0 ? coretmpl_global.make_bitmask32(Width + AddrShift) : 0;

            // Compute the real base addresses
            m_addrstart = addrstart & ~NATIVE_MASK;
            m_addrend = addrend & ~NATIVE_MASK;

            // Compute the masks and the keys
            uX [] umasks = new uX[4];  //std.array<uX, 4> umasks;
            umasks.Fill(unitmask);  //umasks.fill(unitmask);

            uX smask;
            uX emask;
            if (Endian == endianness_t.ENDIANNESS_BIG)
            {
                smask = coretmpl_global.make_bitmask_uX(Width, 8 * new uX(Width, 0).sizeof_() - ((addrstart - m_addrstart) << (3 - AddrShift)));  //smask =  make_bitmask<uX>(8 * sizeof(uX) - ((addrstart - m_addrstart) << (3 - AddrShift)));
                emask = ~coretmpl_global.make_bitmask_uX(Width, 8 * new uX(Width, 0).sizeof_() - ((addrend - m_addrend + 1) << (3 - AddrShift)));  //emask = ~make_bitmask<uX>(8 * sizeof(uX) - ((addrend - m_addrend + 1) << (3 - AddrShift)));
            }
            else
            {
                smask = ~coretmpl_global.make_bitmask_uX(Width, (addrstart - m_addrstart) << (3 - AddrShift));  //smask = ~make_bitmask<uX>((addrstart - m_addrstart) << (3 - AddrShift));
                emask = coretmpl_global.make_bitmask_uX(Width, (addrend - m_addrend + 1) << (3 - AddrShift));  //emask =  make_bitmask<uX>((addrend - m_addrend + 1) << (3 - AddrShift));
            }

            umasks[handler_entry.START]                     &= smask;
            umasks[handler_entry.END]                       &= emask;
            umasks[handler_entry.START | handler_entry.END] &= smask & emask;

            for (u32 i = 0; i < 4; i++)
                m_keymap[i] = mask_to_ukey(umasks[i]);  //m_keymap[i] = mask_to_ukey<uX>(umasks[i]);

            // Compute the shift
            uX dmask = coretmpl_global.make_bitmask_uX(Width, bits_per_access);  //uX dmask = make_bitmask<uX>(bits_per_access);
            u32 active_count = 0;
            for (u32 i = 0; i != 8 << Width; i += (u32)bits_per_access)
            {
                if ((unitmask & (dmask << (int)i)) != 0)
                    active_count++;
            }

            u32 active_count_log = active_count == 1 ? 0U : active_count == 2 ? 1U : active_count == 4 ? 2U : active_count == 8 ? 3U : 0xff;
            if (active_count_log == 0xff)
                throw new emu_fatalerror("memory_units_descriptor() - abort");  //abort();

            s8 base_shift = (s8)(Width - access_width - active_count_log);
            s8 shift = (s8)(base_shift + access_width + AddrShift);


            // Build the handler characteristics
            m_handler_start = shift < 0 ? addrstart << -shift : addrstart >> shift;
            m_handler_mask = shift < 0 ? (mask << -shift) | make_bitmask32(-shift) : mask >> shift;  //m_handler_mask = shift < 0 ? (mask << -shift) | make_bitmask<offs_t>(-shift) : mask >> shift;

            for (u32 i = 0; i < 4; i++)
            {
                if (m_entries_for_key.find(m_keymap[i]) == null)  //if (m_entries_for_key.find(m_keymap[i]) == m_entries_for_key.end())
                {
                    m_entries_for_key[m_keymap[i]] = new std.vector<entry>();
                    generate(m_keymap[i], unitmask, umasks[i], (u32)cswidth, bits_per_access, (u8)base_shift, shift, active_count);
                }
            }
        }


        public offs_t get_handler_start() { return m_handler_start; }
        public offs_t get_handler_mask() { return m_handler_mask; }


        public u8 rkey_to_ukey(u8 rkey) { return m_keymap[rkey]; }
        public std.vector<entry> get_entries_for_key(u8 key) { return m_entries_for_key.find(key); }

        public u8 get_subunit_width() { return m_access_width; }
        public u8 get_subunit_endian() { return m_access_endian; }

        //void set_subunit_handler(handler_entry *handler) { m_handler = handler; }
        public handler_entry get_subunit_handler() { return m_handler; }

        
        //template<int Width, int AddrShift, int Endian>
        void generate(u8 ukey, uX gumask, uX umask, u32 cswidth, u32 bits_per_access, u8 base_shift, s8 shift, u32 active_count)
        {
            var entries = m_entries_for_key[ukey];

            // Compute the selection masks
            if (cswidth == 0)
                cswidth = bits_per_access;

            uX csmask = coretmpl_global.make_bitmask_uX(Width, cswidth);  //uX csmask = make_bitmask<uX>(cswidth);
            uX dmask = coretmpl_global.make_bitmask_uX(Width, bits_per_access);  //uX dmask = make_bitmask<uX>(bits_per_access);

            u32 offset = 0;

            for (u32 i = 0; i != 8 << Width; i += bits_per_access)
            {
                uX numask = dmask << (int)i;
                if ((umask & numask) != 0)
                {
                    uX amask = csmask << (int)(i & ~(cswidth - 1));
                    entries.emplace_back(new entry(amask, numask, shift, (u8)i, (u8)(Endian == endianness_t.ENDIANNESS_BIG ? active_count - 1 - offset : offset)));
                }

                if ((gumask & numask) != 0)
                    offset ++;
            }
        }


        //template<typename T> static u8 mask_to_ukey(T mask);

        u8 mask_to_ukey(uX mask)
        {
            switch (mask.width)
            {
                case 0: return mask_to_ukey_u8(mask.x8);
                case 1: return mask_to_ukey_u16(mask.x16);
                case 2: return mask_to_ukey_u32(mask.x32);
                case 3: return mask_to_ukey_u64(mask.x64);
                default: throw new emu_unimplemented();
            }
        }

        u8 mask_to_ukey_u8(u8 mask) { throw new emu_unimplemented(); }

        u8 mask_to_ukey_u16(u16 mask)  //template<> u8 mask_to_ukey<u16>(u16 mask)
        {
            return (u8)(
                (((mask & 0xff00) != 0) ? 0x02 : 0x00) |
                (((mask & 0x00ff) != 0) ? 0x01 : 0x00));
        }

        u8 mask_to_ukey_u32(u32 mask)  //template<> u8 mask_to_ukey<u32>(u32 mask)
        {
            return (u8)(
                (((mask & 0xff000000) != 0) ? 0x08 : 0x00) |
                (((mask & 0x00ff0000) != 0) ? 0x04 : 0x00) |
                (((mask & 0x0000ff00) != 0) ? 0x02 : 0x00) |
                (((mask & 0x000000ff) != 0) ? 0x01 : 0x00));
        }

        u8 mask_to_ukey_u64(u64 mask)  //template<> u8 mask_to_ukey<u64>(u64 mask)
        {
            return (u8)(
                (((mask & 0xff00000000000000) != 0) ? 0x80 : 0x00) |
                (((mask & 0x00ff000000000000) != 0) ? 0x40 : 0x00) |
                (((mask & 0x0000ff0000000000) != 0) ? 0x20 : 0x00) |
                (((mask & 0x000000ff00000000) != 0) ? 0x10 : 0x00) |
                (((mask & 0x00000000ff000000) != 0) ? 0x08 : 0x00) |
                (((mask & 0x0000000000ff0000) != 0) ? 0x04 : 0x00) |
                (((mask & 0x000000000000ff00) != 0) ? 0x02 : 0x00) |
                (((mask & 0x00000000000000ff) != 0) ? 0x01 : 0x00));
        }
    }
}
