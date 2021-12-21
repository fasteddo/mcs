// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;  //using offs_t = u32;
using PointerU8 = mame.Pointer<System.Byte>;
using u8  = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using uX = mame.FlexPrim;


namespace mame
{
    //**************************************************************************
    //  DEBUGGING
    //**************************************************************************

    //#define VERBOSE 0

    //#if VERBOSE
    //template <typename Format, typename... Params> static void VPRINTF(Format &&fmt, Params &&...args)
    //{
    //    util::stream_format(std::cerr, std::forward<Format>(fmt), std::forward<Params>(args)...);
    //}
    //#else
    //template <typename Format, typename... Params> static void VPRINTF(Format &&, Params &&...) {}
    //#endif

    //#define VALIDATE_REFCOUNTS 0


    //**************************************************************************
    //  CONSTANTS
    //**************************************************************************

    //namespace
    //{
    //template <typename Delegate> struct handler_width;
    class handler_width_read8_delegate : int_const_0 { }
    class handler_width_n_read8_delegate : int_const_0 { }
    class handler_width_read8m_delegate : int_const_0 { }
    class handler_width_n_read8m_delegate : int_const_0 { }
    class handler_width_read8s_delegate : int_const_0 { }
    class handler_width_n_read8s_delegate : int_const_0 { }
    class handler_width_read8sm_delegate : int_const_0 { }
    class handler_width_n_read8sm_delegate : int_const_0 { }
    class handler_width_read8mo_delegate : int_const_0 { }
    class handler_width_n_read8mo_delegate : int_const_0 { }
    class handler_width_read8smo_delegate : int_const_0 { }
    class handler_width_n_read8smo_delegate : int_const_0 { }
    class handler_width_write8_delegate : int_const_0 { }
    class handler_width_n_write8_delegate : int_const_0 { }
    class handler_width_write8m_delegate : int_const_0 { }
    class handler_width_n_write8m_delegate : int_const_0 { }
    class handler_width_write8s_delegate : int_const_0 { }
    class handler_width_n_write8s_delegate : int_const_0 { }
    class handler_width_write8sm_delegate : int_const_0 { }
    class handler_width_n_write8sm_delegate : int_const_0 { }
    class handler_width_write8mo_delegate : int_const_0 { }
    class handler_width_n_write8mo_delegate : int_const_0 { }
    class handler_width_write8smo_delegate : int_const_0 { }
    class handler_width_n_write8smo_delegate : int_const_0 { }
    class handler_width_read16_delegate : int_const_1 { }
    class handler_width_n_read16_delegate : int_const_n1 { }
    class handler_width_read16m_delegate : int_const_1 { }
    class handler_width_n_read16m_delegate : int_const_n1 { }
    class handler_width_read16s_delegate : int_const_1 { }
    class handler_width_n_read16s_delegate : int_const_n1 { }
    class handler_width_read16sm_delegate : int_const_1 { }
    class handler_width_n_read16sm_delegate : int_const_n1 { }
    class handler_width_read16mo_delegate : int_const_1 { }
    class handler_width_n_read16mo_delegate : int_const_n1 { }
    class handler_width_read16smo_delegate : int_const_1 { }
    class handler_width_n_read16smo_delegate : int_const_n1 { }
    class handler_width_write16_delegate : int_const_1 { }
    class handler_width_n_write16_delegate : int_const_n1 { }
    class handler_width_write16m_delegate : int_const_1 { }
    class handler_width_n_write16m_delegate : int_const_n1 { }
    class handler_width_write16s_delegate : int_const_1 { }
    class handler_width_n_write16s_delegate : int_const_n1 { }
    class handler_width_write16sm_delegate : int_const_1 { }
    class handler_width_n_write16sm_delegate : int_const_n1 { }
    class handler_width_write16mo_delegate : int_const_1 { }
    class handler_width_n_write16mo_delegate : int_const_n1 { }
    class handler_width_write16smo_delegate : int_const_1 { }
    class handler_width_n_write16smo_delegate : int_const_n1 { }
    class handler_width_read32_delegate : int_const_2 { }
    class handler_width_n_read32_delegate : int_const_n2 { }
    class handler_width_read32m_delegate : int_const_2 { }
    class handler_width_n_read32m_delegate : int_const_n2 { }
    class handler_width_read32s_delegate : int_const_2 { }
    class handler_width_n_read32s_delegate : int_const_n2 { }
    class handler_width_read32sm_delegate : int_const_2 { }
    class handler_width_n_read32sm_delegate : int_const_n2 { }
    class handler_width_read32mo_delegate : int_const_2 { }
    class handler_width_n_read32mo_delegate : int_const_n2 { }
    class handler_width_read32smo_delegate : int_const_2 { }
    class handler_width_n_read32smo_delegate : int_const_n2 { }
    class handler_width_write32_delegate : int_const_2 { }
    class handler_width_n_write32_delegate : int_const_n2 { }
    class handler_width_write32m_delegate : int_const_2 { }
    class handler_width_n_write32m_delegate : int_const_n2 { }
    class handler_width_write32s_delegate : int_const_2 { }
    class handler_width_n_write32s_delegate : int_const_n2 { }
    class handler_width_write32sm_delegate : int_const_2 { }
    class handler_width_n_write32sm_delegate : int_const_n2 { }
    class handler_width_write32mo_delegate : int_const_2 { }
    class handler_width_n_write32mo_delegate : int_const_n2 { }
    class handler_width_write32smo_delegate : int_const_2 { }
    class handler_width_n_write32smo_delegate : int_const_n2 { }
    class handler_width_read64_delegate : int_const_3 { }
    class handler_width_n_read64_delegate : int_const_n3 { }
    class handler_width_read64m_delegate : int_const_3 { }
    class handler_width_n_read64m_delegate : int_const_n3 { }
    class handler_width_read64s_delegate : int_const_3 { }
    class handler_width_n_read64s_delegate : int_const_n3 { }
    class handler_width_read64sm_delegate : int_const_3 { }
    class handler_width_n_read64sm_delegate : int_const_n3 { }
    class handler_width_read64mo_delegate : int_const_3 { }
    class handler_width_n_read64mo_delegate : int_const_n3 { }
    class handler_width_read64smo_delegate : int_const_3 { }
    class handler_width_n_read64smo_delegate : int_const_n3 { }
    class handler_width_write64_delegate : int_const_3 { }
    class handler_width_n_write64_delegate : int_const_n3 { }
    class handler_width_write64m_delegate : int_const_3 { }
    class handler_width_n_write64m_delegate : int_const_n3 { }
    class handler_width_write64s_delegate : int_const_3 { }
    class handler_width_n_write64s_delegate : int_const_n3 { }
    class handler_width_write64sm_delegate : int_const_3 { }
    class handler_width_n_write64sm_delegate : int_const_n3 { }
    class handler_width_write64mo_delegate : int_const_3 { }
    class handler_width_n_write64mo_delegate : int_const_n3 { }
    class handler_width_write64smo_delegate : int_const_3 { }
    class handler_width_n_write64smo_delegate : int_const_n3 { }
    //} // anonymous namespace


    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> address_space_specific

    // this is a derived class of address_space with specific width, endianness, and table size
    //template<int Level, int Width, int AddrShift, endianness_t Endian>
    class address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> : address_space
        where int_Level : int_const, new()
        where int_Width : int_const, new()
        where int_AddrShift : int_const, new()
        where endianness_t_Endian : endianness_t_const, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using NativeType = uX;
        //using this_type = address_space_specific<Level, Width, AddrShift, Endian>;


        static readonly int Width = new int_Width().value;
        static readonly int AddrShift = new int_AddrShift().value;
        static readonly endianness_t Endian = new endianness_t_Endian().value;


        // constants describing the native size
        static readonly u32 NATIVE_BYTES = 1U << Width;  //static constexpr u32 NATIVE_BYTES = 1 << Width;
        static readonly u32 NATIVE_STEP = AddrShift >= 0 ? NATIVE_BYTES << g.iabs(AddrShift) : NATIVE_BYTES >> g.iabs(AddrShift);  //static constexpr u32 NATIVE_STEP = AddrShift >= 0 ? NATIVE_BYTES << iabs(AddrShift) : NATIVE_BYTES >> iabs(AddrShift);
        static readonly u32 NATIVE_MASK = NATIVE_STEP - 1;  //static constexpr u32 NATIVE_MASK = NATIVE_STEP - 1;
        //static constexpr u32 NATIVE_BITS = 8 * NATIVE_BYTES;  //static constexpr u32 NATIVE_BITS = 8 * NATIVE_BYTES;


        //static constexpr offs_t offset_to_byte(offs_t offset) { return AddrShift < 0 ? offset << iabs(AddrShift) : offset >> iabs(AddrShift); }


        Pointer<handler_entry_read<int_Width, int_AddrShift>> m_dispatch_read;
        Pointer<handler_entry_write<int_Width, int_AddrShift>> m_dispatch_write;


        handler_entry_read<int_Width, int_AddrShift> m_root_read;
        handler_entry_write<int_Width, int_AddrShift> m_root_write;

        std.unordered_set<handler_entry> m_delayed_unrefs;


        public class int_const_std_max_1_Width : int_const { public int value { get { return std.max(1, Width); } } }
        public class int_const_std_max_2_Width : int_const { public int value { get { return std.max(2, Width); } } }
        public class int_const_std_max_3_Width : int_const { public int value { get { return std.max(3, Width); } } }

        // construction/destruction
        public address_space_specific(memory_manager manager, device_memory_interface memory, int spacenum, int address_width)
            : base(manager, memory, spacenum)
        {
            m_unmap_r = new handler_entry_read_unmapped<int_Width, int_AddrShift>(this);
            m_unmap_w = new handler_entry_write_unmapped<int_Width, int_AddrShift>(this);
            m_nop_r = new handler_entry_read_nop<int_Width, int_AddrShift>(this);
            m_nop_w = new handler_entry_write_nop<int_Width, int_AddrShift>(this);

            handler_entry.range r = new handler_entry.range() { start = 0, end = 0xffffffff >> (32 - address_width) };

            switch (address_width)
            {
                case  1: m_root_read = new handler_entry_read_dispatch<int_const_std_max_1_Width, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch< int_const_std_max_1_Width, int_Width, int_AddrShift>(this, r, null); break;
                case  2: m_root_read = new handler_entry_read_dispatch<int_const_std_max_2_Width, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch< int_const_std_max_2_Width, int_Width, int_AddrShift>(this, r, null); break;
                case  3: m_root_read = new handler_entry_read_dispatch<int_const_std_max_3_Width, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch< int_const_std_max_3_Width, int_Width, int_AddrShift>(this, r, null); break;
                case  4: m_root_read = new handler_entry_read_dispatch<int_const_4, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_4, int_Width, int_AddrShift>(this, r, null); break;
                case  5: m_root_read = new handler_entry_read_dispatch<int_const_5, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_5, int_Width, int_AddrShift>(this, r, null); break;
                case  6: m_root_read = new handler_entry_read_dispatch<int_const_6, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_6, int_Width, int_AddrShift>(this, r, null); break;
                case  7: m_root_read = new handler_entry_read_dispatch<int_const_7, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_7, int_Width, int_AddrShift>(this, r, null); break;
                case  8: m_root_read = new handler_entry_read_dispatch<int_const_8, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_8, int_Width, int_AddrShift>(this, r, null); break;
                case  9: m_root_read = new handler_entry_read_dispatch<int_const_9, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_9, int_Width, int_AddrShift>(this, r, null); break;
                case 10: m_root_read = new handler_entry_read_dispatch<int_const_10, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_10, int_Width, int_AddrShift>(this, r, null); break;
                case 11: m_root_read = new handler_entry_read_dispatch<int_const_11, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_11, int_Width, int_AddrShift>(this, r, null); break;
                case 12: m_root_read = new handler_entry_read_dispatch<int_const_12, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_12, int_Width, int_AddrShift>(this, r, null); break;
                case 13: m_root_read = new handler_entry_read_dispatch<int_const_13, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_13, int_Width, int_AddrShift>(this, r, null); break;
                case 14: m_root_read = new handler_entry_read_dispatch<int_const_14, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_14, int_Width, int_AddrShift>(this, r, null); break;
                case 15: m_root_read = new handler_entry_read_dispatch<int_const_15, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_15, int_Width, int_AddrShift>(this, r, null); break;
                case 16: m_root_read = new handler_entry_read_dispatch<int_const_16, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_16, int_Width, int_AddrShift>(this, r, null); break;
                case 17: m_root_read = new handler_entry_read_dispatch<int_const_17, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_17, int_Width, int_AddrShift>(this, r, null); break;
                case 18: m_root_read = new handler_entry_read_dispatch<int_const_18, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_18, int_Width, int_AddrShift>(this, r, null); break;
                case 19: m_root_read = new handler_entry_read_dispatch<int_const_19, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_19, int_Width, int_AddrShift>(this, r, null); break;
                case 20: m_root_read = new handler_entry_read_dispatch<int_const_20, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_20, int_Width, int_AddrShift>(this, r, null); break;
                case 21: m_root_read = new handler_entry_read_dispatch<int_const_21, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_21, int_Width, int_AddrShift>(this, r, null); break;
                case 22: m_root_read = new handler_entry_read_dispatch<int_const_22, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_22, int_Width, int_AddrShift>(this, r, null); break;
                case 23: m_root_read = new handler_entry_read_dispatch<int_const_23, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_23, int_Width, int_AddrShift>(this, r, null); break;
                case 24: m_root_read = new handler_entry_read_dispatch<int_const_24, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_24, int_Width, int_AddrShift>(this, r, null); break;
                case 25: m_root_read = new handler_entry_read_dispatch<int_const_25, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_25, int_Width, int_AddrShift>(this, r, null); break;
                case 26: m_root_read = new handler_entry_read_dispatch<int_const_26, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_26, int_Width, int_AddrShift>(this, r, null); break;
                case 27: m_root_read = new handler_entry_read_dispatch<int_const_27, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_27, int_Width, int_AddrShift>(this, r, null); break;
                case 28: m_root_read = new handler_entry_read_dispatch<int_const_28, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_28, int_Width, int_AddrShift>(this, r, null); break;
                case 29: m_root_read = new handler_entry_read_dispatch<int_const_29, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_29, int_Width, int_AddrShift>(this, r, null); break;
                case 30: m_root_read = new handler_entry_read_dispatch<int_const_30, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_30, int_Width, int_AddrShift>(this, r, null); break;
                case 31: m_root_read = new handler_entry_read_dispatch<int_const_31, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_31, int_Width, int_AddrShift>(this, r, null); break;
                case 32: m_root_read = new handler_entry_read_dispatch<int_const_32, int_Width, int_AddrShift>(this, r, null); m_root_write = new handler_entry_write_dispatch<int_const_32, int_Width, int_AddrShift>(this, r, null); break;
                default: g.fatalerror("Unhandled address bus width {0}\n", address_width); break;
            }

            m_dispatch_read  = m_root_read.get_dispatch();
            m_dispatch_write = m_root_write.get_dispatch();
        }

        protected override string get_handler_string(read_or_write readorwrite, offs_t byteaddress) { throw new emu_unimplemented(); }
        protected override void dump_maps(std.vector<memory_entry> read_map, std.vector<memory_entry> write_map) { throw new emu_unimplemented(); }


        //-------------------------------------------------
        //  unmap - unmap a section of address space
        //-------------------------------------------------
        protected override void unmap_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, bool quiet)
        {
            emumem_global.VPRINTF("address_space::unmap({0}{1}-{2}{3} mirror={4}{5}, {6}, {7})\n",
                    m_addrchars, addrstart, m_addrchars, addrend,
                    m_addrchars, addrmirror,
                    (readorwrite == read_or_write.READ) ? "read" : (readorwrite == read_or_write.WRITE) ? "write" : (readorwrite == read_or_write.READWRITE) ? "read/write" : "??",
                    quiet ? "quiet" : "normal");

            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            check_optimize_mirror("unmap_generic", addrstart, addrend, addrmirror, out nstart, out nend, out nmask, out nmirror);

            // read space
            if (readorwrite == read_or_write.READ || readorwrite == read_or_write.READWRITE)
            {
                var handler = (handler_entry_read<int_Width, int_AddrShift>)(quiet ? m_nop_r : m_unmap_r);
                handler.ref_();
                m_root_read.populate(nstart, nend, nmirror, handler);
            }

            // write space
            if (readorwrite == read_or_write.WRITE || readorwrite == read_or_write.READWRITE)
            {
                var handler = (handler_entry_write<int_Width, int_AddrShift>)(quiet ? m_nop_w : m_unmap_w);
                handler.ref_();
                m_root_write.populate(nstart, nend, nmirror, handler);
            }

            invalidate_caches(readorwrite);
        }


        //-------------------------------------------------
        //  install_ram_generic - install a simple fixed
        //  RAM region into the given address space
        //-------------------------------------------------
        protected override void install_ram_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, PointerU8 baseptr)
        {
            emumem_global.VPRINTF("address_space::install_ram_generic({0}{1}-{2}{3} mirror={4}{5}, {6}, {7})\n",
                    m_addrchars, addrstart, m_addrchars, addrend,
                    m_addrchars, addrmirror,
                    (readorwrite == read_or_write.READ) ? "read" : (readorwrite == read_or_write.WRITE) ? "write" : (readorwrite == read_or_write.READWRITE) ? "read/write" : "??",
                    baseptr);

            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            check_optimize_mirror("install_ram_generic", addrstart, addrend, addrmirror, out nstart, out nend, out nmask, out nmirror);

            // map for read
            if (readorwrite == read_or_write.READ || readorwrite == read_or_write.READWRITE)
            {
                var hand_r = new handler_entry_read_memory<int_Width, int_AddrShift>(this, baseptr);
                hand_r.set_address_info(nstart, nmask);
                m_root_read.populate(nstart, nend, nmirror, hand_r);
            }

            // map for write
            if (readorwrite == read_or_write.WRITE || readorwrite == read_or_write.READWRITE)
            {
                var hand_w = new handler_entry_write_memory<int_Width, int_AddrShift>(this, baseptr);
                hand_w.set_address_info(nstart, nmask);
                m_root_write.populate(nstart, nend, nmirror, hand_w);
            }

            invalidate_caches(readorwrite);
        }


        //-------------------------------------------------
        //  install_bank_generic - install a range as
        //  mapping to a particular bank
        //-------------------------------------------------
        protected override void install_bank_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_bank rbank, memory_bank wbank)
        {
            emumem_global.VPRINTF("address_space::install_readwrite_bank({0}{1}-{2}{3} mirror={4}{5}, read=\"{6}\" / write=\"{7}\")\n",
                    m_addrchars, addrstart, m_addrchars, addrend,
                    m_addrchars, addrmirror,
                    (rbank != null) ? rbank.tag() : "(none)", (wbank != null) ? wbank.tag() : "(none)");

            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            check_optimize_mirror("install_bank_generic", addrstart, addrend, addrmirror, out nstart, out nend, out nmask, out nmirror);

            // map the read bank
            if (rbank != null)
            {
                var hand_r = new handler_entry_read_memory_bank<int_Width, int_AddrShift>(this, rbank);
                hand_r.set_address_info(nstart, nmask);
                m_root_read.populate(nstart, nend, nmirror, hand_r);
            }

            // map the write bank
            if (wbank != null)
            {
                var hand_w = new handler_entry_write_memory_bank<int_Width, int_AddrShift>(this, wbank);
                hand_w.set_address_info(nstart, nmask);
                m_root_write.populate(nstart, nend, nmirror, hand_w);
            }

            invalidate_caches(rbank != null ? wbank != null ? read_or_write.READWRITE : read_or_write.READ : read_or_write.WRITE);
        }


        //-------------------------------------------------
        //  install_readwrite_port - install a new I/O port
        //  handler into this address space
        //-------------------------------------------------
        protected override void install_readwrite_port(offs_t addrstart, offs_t addrend, offs_t addrmirror, string rtag, string wtag)
        {
            emumem_global.VPRINTF("address_space::install_readwrite_port({0}{1}-{2}{3} mirror={4}{5}, read=\"{6}\" / write=\"{7}\")\n",
                    m_addrchars, addrstart, m_addrchars, addrend,
                    m_addrchars, addrmirror,
                    rtag.empty() ? "(none)" : rtag, wtag.empty() ? "(none)" : wtag);

            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            check_optimize_mirror("install_readwrite_port", addrstart, addrend, addrmirror, out nstart, out nend, out nmask, out nmirror);

            // read handler
            if (rtag != "")
            {
                // find the port
                ioport_port port = device().owner().ioport(rtag);
                if (port == null)
                    throw new emu_fatalerror("Attempted to map non-existent port '{0}' for read in space {1} of device '{2}'\n", rtag, m_name, m_device.tag());

                // map the range and set the ioport
                var hand_r = new handler_entry_read_ioport<int_Width, int_AddrShift>(this, port);
                m_root_read.populate(nstart, nend, nmirror, hand_r);
            }

            if (wtag != "")
            {
                // find the port
                ioport_port port = device().owner().ioport(wtag);
                if (port == null)
                    g.fatalerror("Attempted to map non-existent port '{0}' for write in space {1} of device '{2}'\n", wtag, m_name, m_device.tag());

                // map the range and set the ioport
                var hand_w = new handler_entry_write_ioport<int_Width, int_AddrShift>(this, port);
                m_root_write.populate(nstart, nend, nmirror, hand_w);
            }

            invalidate_caches(rtag != "" ? wtag != "" ? read_or_write.READWRITE : read_or_write.READ : read_or_write.WRITE);
        }


        protected override void install_device_delegate(offs_t addrstart, offs_t addrend, device_t device, address_map_constructor map, u64 unitmask = 0, int cswidth = 0) { throw new emu_unimplemented(); }
        protected override void install_view(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_view view) { throw new emu_unimplemented(); }


        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read8_delegate, handler_width_read8_delegate, handler_width_n_read8_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write8_delegate, handler_width_write8_delegate, handler_width_n_write8_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8_delegate rhandler, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read16_delegate, handler_width_read16_delegate, handler_width_n_read16_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write16_delegate, handler_width_write16_delegate, handler_width_n_write16_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16_delegate rhandler, write16_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read32_delegate, handler_width_read32_delegate, handler_width_n_read32_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write32_delegate, handler_width_write32_delegate, handler_width_n_write32_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32_delegate rhandler, write32_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read64_delegate, handler_width_read64_delegate, handler_width_n_read64_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write64_delegate, handler_width_write64_delegate, handler_width_n_write64_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64_delegate rhandler, write64_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8m_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read8m_delegate, handler_width_read8m_delegate, handler_width_n_read8m_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write8m_delegate, handler_width_write8m_delegate, handler_width_n_write8m_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8m_delegate rhandler, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16m_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read16m_delegate, handler_width_read16m_delegate, handler_width_n_read16m_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write16m_delegate, handler_width_write16m_delegate, handler_width_n_write16m_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16m_delegate rhandler, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32m_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read32m_delegate, handler_width_read32m_delegate, handler_width_n_read32m_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write32m_delegate, handler_width_write32m_delegate, handler_width_n_write32m_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32m_delegate rhandler, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64m_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read64m_delegate, handler_width_read64m_delegate, handler_width_n_read64m_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write64m_delegate, handler_width_write64m_delegate, handler_width_n_write64m_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64m_delegate rhandler, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read8s_delegate, handler_width_read8s_delegate, handler_width_n_read8s_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write8s_delegate, handler_width_write8s_delegate, handler_width_n_write8s_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read16s_delegate, handler_width_read16s_delegate, handler_width_n_read16s_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write16s_delegate, handler_width_write16s_delegate, handler_width_n_write16s_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read32s_delegate, handler_width_read32s_delegate, handler_width_n_read32s_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write32s_delegate, handler_width_write32s_delegate, handler_width_n_write32s_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read64s_delegate, handler_width_read64s_delegate, handler_width_n_read64s_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write64s_delegate, handler_width_write64s_delegate, handler_width_n_write64s_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read8sm_delegate, handler_width_read8sm_delegate, handler_width_n_read8sm_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write8sm_delegate, handler_width_write8sm_delegate, handler_width_n_write8sm_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read16sm_delegate, handler_width_read16sm_delegate, handler_width_n_read16sm_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write16sm_delegate, handler_width_write16sm_delegate, handler_width_n_write16sm_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16sm_delegate rhandler, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read32sm_delegate, handler_width_read32sm_delegate, handler_width_n_read32sm_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write32sm_delegate, handler_width_write32sm_delegate, handler_width_n_write32sm_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32sm_delegate rhandler, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read64sm_delegate, handler_width_read64sm_delegate, handler_width_n_read64sm_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write64sm_delegate, handler_width_write64sm_delegate, handler_width_n_write64sm_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64sm_delegate rhandler, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read8mo_delegate, handler_width_read8mo_delegate, handler_width_n_read8mo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write8mo_delegate, handler_width_write8mo_delegate, handler_width_n_write8mo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8mo_delegate rhandler, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read16mo_delegate, handler_width_read16mo_delegate, handler_width_n_read16mo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write16mo_delegate, handler_width_write16mo_delegate, handler_width_n_write16mo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16mo_delegate rhandler, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read32mo_delegate, handler_width_read32mo_delegate, handler_width_n_read32mo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write32mo_delegate, handler_width_write32mo_delegate, handler_width_n_write32mo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32mo_delegate rhandler, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read64mo_delegate, handler_width_read64mo_delegate, handler_width_n_read64mo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write64mo_delegate, handler_width_write64mo_delegate, handler_width_n_write64mo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64mo_delegate rhandler, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read8smo_delegate, handler_width_read8smo_delegate, handler_width_n_read8smo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write8smo_delegate, handler_width_write8smo_delegate, handler_width_n_write8smo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read16smo_delegate, handler_width_read16smo_delegate, handler_width_n_read16smo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write16smo_delegate, handler_width_write16smo_delegate, handler_width_n_write16smo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read32smo_delegate, handler_width_read32smo_delegate, handler_width_n_read32smo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write32smo_delegate, handler_width_write32smo_delegate, handler_width_n_write32smo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32smo_delegate rhandler, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl<read64smo_delegate, handler_width_read64smo_delegate, handler_width_n_read64smo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl<write64smo_delegate, handler_width_write64smo_delegate, handler_width_n_write64smo_delegate>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64smo_delegate rhandler, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }


        //using address_space::install_read_tap;
        //using address_space::install_write_tap;
        //using address_space::install_readwrite_tap;


        protected override memory_passthrough_handler install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<uX> tap, memory_passthrough_handler mph) { throw new emu_unimplemented(); }  //virtual memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, uX &data, uX mem_mask)> tap, memory_passthrough_handler *mph) override;
        protected override memory_passthrough_handler install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<uX> tap, memory_passthrough_handler mph) { throw new emu_unimplemented(); }  //virtual memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, uX &data, uX mem_mask)> tap, memory_passthrough_handler *mph) override;
        protected override memory_passthrough_handler install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<uX> tapr, install_tap_func<uX> tapw, memory_passthrough_handler mph) { throw new emu_unimplemented(); }  //virtual memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, uX &data, uX mem_mask)> tapr, std::function<void (offs_t offset, uX &data, uX mem_mask)> tapw, memory_passthrough_handler *mph) override;


        protected override std.pair<object, object> get_cache_info()
        {
            std.pair<object, object> rw = new std.pair<object, object>(m_root_read, m_root_write);  //std::pair<void *, void *> rw;
            //rw.first  = m_root_read;
            //rw.second = m_root_write;
            return rw;
        }


        protected override std.pair<object, object> get_specific_info()  //std::pair<const void *, const void *> get_specific_info() override {
        {
            std.pair<object, object> rw = new std.pair<object, object>(m_dispatch_read, m_dispatch_write);  //std::pair<const void *, const void *> rw;
            //rw.first  = m_dispatch_read;
            //rw.second = m_dispatch_write;
            return rw;
        }


        //void delayed_ref(handler_entry *e) {
        //    e->ref();
        //    m_delayed_unrefs.insert(e);
        //}

        //void delayed_unref(handler_entry *e) {
        //    m_delayed_unrefs.erase(m_delayed_unrefs.find(e));
        //    e->unref();
        //}


        protected override void validate_reference_counts()
        {
            //throw new emu_unimplemented();
#if false
            handler_entry.reflist refs = new handler_entry.reflist();
            refs.add(m_root_read);
            refs.add(m_root_write);
            refs.add(m_unmap_r);
            refs.add(m_unmap_w);
            refs.add(m_nop_r);
            refs.add(m_nop_w);
            foreach (handler_entry e in m_delayed_unrefs)
                refs.add(e);
            refs.propagate();
            refs.check();
#endif
        }


        protected override void remove_passthrough(std.unordered_set<handler_entry> handlers)  //virtual void remove_passthrough(std::unordered_set<handler_entry *> &handlers) override {
        {
            invalidate_caches(read_or_write.READWRITE);
            m_root_read.detach(handlers);
            m_root_write.detach(handlers);
        }


        // generate accessor table
        protected override void accessors(data_accessors accessors)
        {
            throw new emu_unimplemented();
#if false
            accessors.read_byte = read_byte_static;
            accessors.read_word = read_word_static;
            accessors.read_word_masked = read_word_masked_static;
            accessors.read_dword = read_dword_static;
            accessors.read_dword_masked = read_dword_masked_static;
            accessors.read_qword = read_qword_static;
            accessors.read_qword_masked = read_qword_masked_static;
            accessors.write_byte = write_byte_static;
            accessors.write_word = write_word_static;
            accessors.write_word_masked = write_word_masked_static;
            accessors.write_dword = write_dword_static;
            accessors.write_dword_masked = write_dword_masked_static;
            accessors.write_qword = write_qword_static;
            accessors.write_qword_masked = write_qword_masked_static;
#endif
        }


        // return a pointer to the read bank, or nullptr if none
        protected override object get_read_ptr(offs_t address)
        {
            return m_root_read.get_ptr(address);
        }


        // return a pointer to the write bank, or nullptr if none
        protected override object get_write_ptr(offs_t address)
        {
            return m_root_write.get_ptr(address);
        }


        // native read
        uX read_native(offs_t offset, uX mask)  //NativeType read_native(offs_t offset, NativeType mask)
        {
            return emumem_global.dispatch_read<int_Level, int_Width, int_AddrShift>(offs_t.MaxValue, offset & m_addrmask, mask, m_dispatch_read);  //    return dispatch_read<Level, Width, AddrShift>(offs_t(-1), offset & m_addrmask, mask, m_dispatch_read);
        }

        // mask-less native read
        uX read_native(offs_t offset)  //NativeType read_native(offs_t offset)
        {
            return emumem_global.dispatch_read<int_Level, int_Width, int_AddrShift>(offs_t.MaxValue, offset & m_addrmask, new uX(Width, 0xffffffffffffffffU), m_dispatch_read);  //return dispatch_read<Level, Width, AddrShift>(offs_t(-1), offset & m_addrmask, uX(0xffffffffffffffffU), m_dispatch_read);
        }

        // native write
        void write_native(offs_t offset, uX data, uX mask)  //void write_native(offs_t offset, NativeType data, NativeType mask)
        {
            emumem_global.dispatch_write<int_Level, int_Width, int_AddrShift>(offs_t.MaxValue, offset & m_addrmask, data, mask, m_dispatch_write);  //    dispatch_write<Level, Width, AddrShift>(offs_t(-1), offset & m_addrmask, data, mask, m_dispatch_write);
        }

        // mask-less native write
        void write_native(offs_t offset, uX data)  //void write_native(offs_t offset, NativeType data)
        {
            emumem_global.dispatch_write<int_Level, int_Width, int_AddrShift>(offs_t.MaxValue, offset & m_addrmask, data, new uX(Width, 0xffffffffffffffffU), m_dispatch_write);  //    dispatch_write<Level, Width, AddrShift>(offs_t(-1), offset & m_addrmask, data, uX(0xffffffffffffffffU), m_dispatch_write);
        }


        // virtual access to these functions
        public override u8 read_byte(offs_t address) { return Width == 0 ? read_native(address & ~NATIVE_MASK).u8 : emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_0, bool_const_true>((offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(0, 0xff)).u8; }
        public override u16 read_word(offs_t address) { return Width == 1 ? read_native(address & ~NATIVE_MASK).u16 : emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_1, bool_const_true>((offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(1, 0xffff)).u16; }
        public override u16 read_word(offs_t address, u16 mask) { return emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_1, bool_const_true>((offs_t offset, uX mask2) => { return read_native(offset, mask2); }, address, new uX(1, mask)).u16; }
        protected override u16 read_word_unaligned(offs_t address) { return emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_1, bool_const_false>((offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(1, 0xffff)).u16; }
        protected override u16 read_word_unaligned(offs_t address, u16 mask) { return emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_1, bool_const_false>((offs_t offset, uX mask2) => { return read_native(offset, mask2); }, address, new uX(1, mask)).u16; }
        protected override u32 read_dword(offs_t address) { return Width == 2 ? read_native(address & ~NATIVE_MASK).u32 : emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_2, bool_const_true>((offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(2, 0xffffffff)).u32; }
        protected override u32 read_dword(offs_t address, u32 mask) { return emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_2, bool_const_true>((offs_t offset, uX mask2) => { return read_native(offset, mask2); }, address, new uX(2, mask)).u32; }
        protected override u32 read_dword_unaligned(offs_t address) { return emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_2, bool_const_false>((offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(2, 0xffffffff)).u32; }
        protected override u32 read_dword_unaligned(offs_t address, u32 mask) { return emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_2, bool_const_false>((offs_t offset, uX mask2) => { return read_native(offset, mask2); }, address, new uX(2, mask)).u32; }
        protected override u64 read_qword(offs_t address) { return Width == 3 ? read_native(address & ~NATIVE_MASK).u64 : emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_3, bool_const_true>((offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(3, 0xffffffffffffffffU)).u64; }
        protected override u64 read_qword(offs_t address, u64 mask) { return emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_3, bool_const_true>((offs_t offset, uX mask2) => { return read_native(offset, mask2); }, address, new uX(3, mask)).u64; }
        protected override u64 read_qword_unaligned(offs_t address) { return emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_3, bool_const_false>((offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(3, 0xffffffffffffffffU)).u64; }
        protected override u64 read_qword_unaligned(offs_t address, u64 mask) { return emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_3, bool_const_false>((offs_t offset, uX mask2) => { return read_native(offset, mask2); }, address, new uX(3, mask)).u64; }

        public override void write_byte(offs_t address, u8 data) { if (Width == 0) write_native(address & ~NATIVE_MASK, new uX(0, data)); else emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_0, bool_const_true>((offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(0, data), new uX(0, 0xff)); }
        public override void write_word(offs_t address, u16 data) { if (Width == 1) write_native(address & ~NATIVE_MASK, new uX(1, data)); else emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_1, bool_const_true>((offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(1, data), new uX(1, 0xffff)); }
        public override void write_word(offs_t address, u16 data, u16 mask) { emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_1, bool_const_true>((offs_t offset, uX data2, uX mask2) => { write_native(offset, data2, mask2); }, address, new uX(1, data), new uX(1, mask)); }
        protected override void write_word_unaligned(offs_t address, u16 data) { emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_1, bool_const_false>((offs_t offset, uX data2, uX mask2) => { write_native(offset, data2, mask2); }, address, new uX(1, data), new uX(1, 0xffff)); }
        protected override void write_word_unaligned(offs_t address, u16 data, u16 mask) { emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_1, bool_const_false>((offs_t offset, uX data2, uX mask2) => { write_native(offset, data2, mask2); }, address, new uX(1, data), new uX(1, mask)); }
        protected override void write_dword(offs_t address, u32 data) { if (Width == 2) write_native(address & ~NATIVE_MASK, new uX(2, data)); else emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_2, bool_const_true>((offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(2, data), new uX(2, 0xffffffff)); }
        protected override void write_dword(offs_t address, u32 data, u32 mask) { emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_2, bool_const_true>((offs_t offset, uX data2, uX mask2) => { write_native(offset, data2, mask2); }, address, new uX(2, data), new uX(2, mask)); }
        protected override void write_dword_unaligned(offs_t address, u32 data) { emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_2, bool_const_false>((offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(2, data), new uX(2, 0xffffffff)); }
        protected override void write_dword_unaligned(offs_t address, u32 data, u32 mask) { emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_2, bool_const_false>((offs_t offset, uX data2, uX mask2) => { write_native(offset, data2, mask2); }, address, new uX(2, data), new uX(2, mask)); }
        protected override void write_qword(offs_t address, u64 data) { if (Width == 3) write_native(address & ~NATIVE_MASK, new uX(3, data)); else emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_3, bool_const_true>((offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(3, data), new uX(3, 0xffffffffffffffffU)); }
        protected override void write_qword(offs_t address, u64 data, u64 mask) { emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_3, bool_const_true>((offs_t offset, uX data2, uX mask2) => { write_native(offset, data2, mask2); }, address, new uX(3, data), new uX(3, mask)); }
        protected override void write_qword_unaligned(offs_t address, u64 data) { emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_3, bool_const_false>((offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(3, data), new uX(3, 0xffffffffffffffffU)); }
        protected override void write_qword_unaligned(offs_t address, u64 data, u64 mask) { emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_3, bool_const_false>((offs_t offset, uX data2, uX mask2) => { write_native(offset, data2, mask2); }, address, new uX(3, data), new uX(3, mask)); }

        // static access to these functions
        static u8 read_byte_static(address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> space, offs_t address) { return Width == 0 ? space.read_native(address & ~NATIVE_MASK).u8 : emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_0, bool_const_true>((offs_t offset, uX mask) => { return space.read_native(offset, mask); }, address, new uX(0, 0xff)).u8; }
        static u16 read_word_static(address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> space, offs_t address) { return Width == 1 ? space.read_native(address & ~NATIVE_MASK).u16 : emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_1, bool_const_true>((offs_t offset, uX mask) => { return space.read_native(offset, mask); }, address, new uX(1, 0xffff)).u16; }
        static u16 read_word_masked_static(address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> space, offs_t address, u16 mask) { return emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_1, bool_const_true>((offs_t offset, uX mask2) => { return space.read_native(offset, mask2); }, address, new uX(1, mask)).u16; }
        static u32 read_dword_static(address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> space, offs_t address) { return Width == 2 ? space.read_native(address & ~NATIVE_MASK).u32 : emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_2, bool_const_true>((offs_t offset, uX mask) => { return space.read_native(offset, mask); }, address, new uX(2, 0xffffffff)).u32; }
        static u32 read_dword_masked_static(address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> space, offs_t address, u32 mask) { return emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_2, bool_const_true>((offs_t offset, uX mask2) => { return space.read_native(offset, mask2); }, address, new uX(2, mask)).u32; }
        static u64 read_qword_static(address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> space, offs_t address) { return Width == 3 ? space.read_native(address & ~NATIVE_MASK).u64 : emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_3, bool_const_true>((offs_t offset, uX mask) => { return space.read_native(offset, mask); }, address, new uX(3, 0xffffffffffffffffU)).u64; }
        static u64 read_qword_masked_static(address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> space, offs_t address, u64 mask) { return emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_3, bool_const_true>((offs_t offset, uX mask2) => { return space.read_native(offset, mask2); }, address, new uX(3, mask)).u64; }
        static void write_byte_static(address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> space, offs_t address, u8 data) { if (Width == 0) space.write_native(address & ~NATIVE_MASK, new uX(0, data)); else emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_0, bool_const_true>((offs_t offset, uX data2, uX mask) => { space.write_native(offset, data2, mask); }, address, new uX(0, data), new uX(0, 0xff)); }
        static void write_word_static(address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> space, offs_t address, u16 data) { if (Width == 1) space.write_native(address & ~NATIVE_MASK, new uX(1, data)); else emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_1, bool_const_true>((offs_t offset, uX data2, uX mask) => { space.write_native(offset, data2, mask); }, address, new uX(1, data), new uX(1, 0xffff)); }
        static void write_word_masked_static(address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> space, offs_t address, u16 data, u16 mask) { emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_1, bool_const_true>((offs_t offset, uX data2, uX mask2) => { space.write_native(offset, data2, mask2); }, address, new uX(1, data), new uX(1, mask)); }
        static void write_dword_static(address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> space, offs_t address, u32 data) { if (Width == 2) space.write_native(address & ~NATIVE_MASK, new uX(2, data)); else emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_2, bool_const_true>((offs_t offset, uX data2, uX mask) => { space.write_native(offset, data2, mask); }, address, new uX(2, data), new uX(2, 0xffffffff)); }
        static void write_dword_masked_static(address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> space, offs_t address, u32 data, u32 mask) { emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_2, bool_const_true>((offs_t offset, uX data2, uX mask2) => { space.write_native(offset, data2, mask2); }, address, new uX(2, data), new uX(2, mask)); }
        static void write_qword_static(address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> space, offs_t address, u64 data) { if (Width == 3) space.write_native(address & ~NATIVE_MASK, new uX(3, data)); else emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_3, bool_const_false>((offs_t offset, uX data2, uX mask) => { space.write_native(offset, data2, mask); }, address, new uX(3, data), new uX(3, 0xffffffffffffffffU)); }
        static void write_qword_masked_static(address_space_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> space, offs_t address, u64 data, u64 mask) { emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_const_3, bool_const_false>((offs_t offset, uX data2, uX mask2) => { space.write_native(offset, data2, mask2); }, address, new uX(3, data), new uX(3, mask)); }


        //template<typename READ>
        void install_read_handler_impl<READ, handler_width_READ, handler_width_n_READ>(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ handler_r)  //void install_read_handler_impl(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ &handler_r)
            where handler_width_READ : int_const, new()
            where handler_width_n_READ : int_const, new()
        {
            try { }  //try { handler_r.resolve(); }
            catch (binding_type_exception)
            {
                g.osd_printf_error("Binding error while installing read handler {0} for range 0x{1}-0x{2} mask 0x{3} mirror 0x{4} select 0x{5} umask 0x{6}\n", handler_r.ToString(), addrstart, addrend, addrmask, addrmirror, addrselect, unitmask);
                throw;
            }
            install_read_handler_helper<handler_width_READ, handler_width_n_READ, READ>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_r);
        }

        //template<typename WRITE>
        void install_write_handler_impl<WRITE, handler_width_WRITE, handler_width_n_WRITE>(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, WRITE handler_w)  //void install_write_handler_impl(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, WRITE &handler_w)
            where handler_width_WRITE : int_const, new()
            where handler_width_n_WRITE : int_const, new()
        {
            try { }  //try { handler_w.resolve(); }
            catch (binding_type_exception)
            {
                g.osd_printf_error("Binding error while installing write handler {0} for range 0x{1}-0x{2} mask 0x{3} mirror 0x{4} select 0x{5} umask 0x{6}\n", handler_w.ToString(), addrstart, addrend, addrmask, addrmirror, addrselect, unitmask);
                throw;
            }
            install_write_handler_helper<handler_width_WRITE, handler_width_n_WRITE, WRITE>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_w);
        }

        //template<typename READ, typename WRITE>
        void install_readwrite_handler_impl<READ, WRITE>(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ handler_r, WRITE handler_w)  //void install_readwrite_handler_impl(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ &handler_r, WRITE &handler_w)
        {
            throw new emu_unimplemented();
#if false
            static_assert(handler_width<READ>::value == handler_width<WRITE>::value, "handler widths do not match");
            try { handler_r.resolve(); }
            catch (const binding_type_exception &)
            {
                osd_printf_error("Binding error while installing read handler %s for range 0x%X-0x%X mask 0x%X mirror 0x%X select 0x%X umask 0x%X\n", handler_r.name(), addrstart, addrend, addrmask, addrmirror, addrselect, unitmask);
                throw;
            }
            try { handler_w.resolve(); }
            catch (const binding_type_exception &)
            {
                osd_printf_error("Binding error while installing write handler %s for range 0x%X-0x%X mask 0x%X mirror 0x%X select 0x%X umask 0x%X\n", handler_w.name(), addrstart, addrend, addrmask, addrmirror, addrselect, unitmask);
                throw;
            }
            install_readwrite_handler_helper<handler_width<READ>::value>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_r, handler_w);
#endif
        }

        //template<int AccessWidth, typename READ>
        void install_read_handler_helper<int_AccessWidth, int_nAccessWidth, READ>(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ handler_r)
            where int_AccessWidth : int_const, new()
            where int_nAccessWidth : int_const, new()
        {
            int AccessWidth = new int_AccessWidth().value;

            if (Width < AccessWidth)
            {
                g.fatalerror("install_read_handler: cannot install a {0}-wide handler in a {1}-wide bus", 8 << AccessWidth, 8 << Width);
            }
            else
            {
                emumem_global.VPRINTF("address_space::install_read_handler({0}{1}-{2}{3} mask={4}{5} mirror={6}{7}, space width={8}, handler width={9}, {10}, {11}{12})\n",
                        m_addrchars, addrstart, m_addrchars, addrend,
                        m_addrchars, addrmask, m_addrchars, addrmirror,
                        8 << Width, 8 << AccessWidth,
                        handler_r.ToString(), data_width() / 4, unitmask);

                offs_t nstart;
                offs_t nend;
                offs_t nmask;
                offs_t nmirror;
                u64 nunitmask;
                int ncswidth;
                check_optimize_all("install_read_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, out nstart, out nend, out nmask, out nmirror, out nunitmask, out ncswidth);

                if (Width == AccessWidth)
                {
                    var hand_r = new handler_entry_read_delegate<int_Width, int_AddrShift>(this, handler_r);
                    hand_r.set_address_info(nstart, nmask);
                    m_root_read.populate(nstart, nend, nmirror, hand_r);
                }
                else
                {
                    var hand_r = new handler_entry_read_delegate<int_AccessWidth, int_nAccessWidth>(this, handler_r);
                    memory_units_descriptor<int_Width, int_AddrShift> descriptor = new memory_units_descriptor<int_Width, int_AddrShift>((u8)AccessWidth, (u8)Endian, hand_r, nstart, nend, nmask, new uX(Width, nunitmask), ncswidth);
                    hand_r.set_address_info(descriptor.get_handler_start(), descriptor.get_handler_mask());
                    m_root_read.populate_mismatched(nstart, nend, nmirror, descriptor);
                    hand_r.unref();
                }

                invalidate_caches(read_or_write.READ);
            }
        }


        //template<int AccessWidth, typename WRITE>
        void install_write_handler_helper<int_AccessWidth, int_nAccessWidth, WRITE>(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, WRITE handler_w)
            where int_AccessWidth : int_const, new()
            where int_nAccessWidth : int_const, new()
        {
            int AccessWidth = new int_AccessWidth().value;

            if (Width < AccessWidth)
            {
                g.fatalerror("install_write_handler: cannot install a {0}-wide handler in a {1}-wide bus", 8 << AccessWidth, 8 << Width);
            }
            else
            {
                emumem_global.VPRINTF("address_space::install_write_handler({0}{1}-{2}{3} mask={4}{5} mirror={6}{7}, space width={8}, handler width={9}, {10}, {11}{12})\n",
                        m_addrchars, addrstart, m_addrchars, addrend,
                        m_addrchars, addrmask, m_addrchars, addrmirror,
                        8 << Width, 8 << AccessWidth,
                        handler_w.ToString(), data_width() / 4, unitmask);

                offs_t nstart;
                offs_t nend;
                offs_t nmask;
                offs_t nmirror;
                u64 nunitmask;
                int ncswidth;
                check_optimize_all("install_write_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, out nstart, out nend, out nmask, out nmirror, out nunitmask, out ncswidth);

                if (Width == AccessWidth)
                {
                    var hand_w = new handler_entry_write_delegate<int_Width, int_AddrShift>(this, handler_w);
                    hand_w.set_address_info(nstart, nmask);
                    m_root_write.populate(nstart, nend, nmirror, hand_w);
                }
                else
                {
                    var hand_w = new handler_entry_write_delegate<int_AccessWidth, int_nAccessWidth>(this, handler_w);
                    memory_units_descriptor<int_Width, int_AddrShift> descriptor = new memory_units_descriptor<int_Width, int_AddrShift>((u8)AccessWidth, (u8)Endian, hand_w, nstart, nend, nmask, new uX(Width, nunitmask), ncswidth);
                    hand_w.set_address_info(descriptor.get_handler_start(), descriptor.get_handler_mask());
                    m_root_write.populate_mismatched(nstart, nend, nmirror, descriptor);
                    hand_w.unref();
                }

                invalidate_caches(read_or_write.WRITE);
            }
        }

        //template<int AccessWidth, typename READ, typename WRITE>
        //void install_readwrite_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
        //                                 const READ  &handler_r,
        //                                 const WRITE &handler_w)
    }
}
