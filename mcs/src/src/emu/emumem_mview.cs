// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;  //using offs_t = u32;
using PointerU8 = mame.Pointer<System.Byte>;
using u64 = System.UInt64;


namespace mame
{
    //#define VERBOSE 0
    //
    //#if VERBOSE
    //template <typename Format, typename... Params> static void VPRINTF(Format &&fmt, Params &&...args)
    //{
    //    util::stream_format(std::cerr, std::forward<Format>(fmt), std::forward<Params>(args)...);
    //}
    //#else
    //template <typename Format, typename... Params> static void VPRINTF(Format &&, Params &&...) {}
    //#endif

    //namespace {

    //template <typename Delegate> struct handler_width;
    //template <> struct handler_width<read8_delegate> { static constexpr int value = 0; };
    //template <> struct handler_width<read8m_delegate> { static constexpr int value = 0; };
    //template <> struct handler_width<read8s_delegate> { static constexpr int value = 0; };
    //template <> struct handler_width<read8sm_delegate> { static constexpr int value = 0; };
    //template <> struct handler_width<read8mo_delegate> { static constexpr int value = 0; };
    //template <> struct handler_width<read8smo_delegate> { static constexpr int value = 0; };
    //template <> struct handler_width<write8_delegate> { static constexpr int value = 0; };
    //template <> struct handler_width<write8m_delegate> { static constexpr int value = 0; };
    //template <> struct handler_width<write8s_delegate> { static constexpr int value = 0; };
    //template <> struct handler_width<write8sm_delegate> { static constexpr int value = 0; };
    //template <> struct handler_width<write8mo_delegate> { static constexpr int value = 0; };
    //template <> struct handler_width<write8smo_delegate> { static constexpr int value = 0; };
    //template <> struct handler_width<read16_delegate> { static constexpr int value = 1; };
    //template <> struct handler_width<read16m_delegate> { static constexpr int value = 1; };
    //template <> struct handler_width<read16s_delegate> { static constexpr int value = 1; };
    //template <> struct handler_width<read16sm_delegate> { static constexpr int value = 1; };
    //template <> struct handler_width<read16mo_delegate> { static constexpr int value = 1; };
    //template <> struct handler_width<read16smo_delegate> { static constexpr int value = 1; };
    //template <> struct handler_width<write16_delegate> { static constexpr int value = 1; };
    //template <> struct handler_width<write16m_delegate> { static constexpr int value = 1; };
    //template <> struct handler_width<write16s_delegate> { static constexpr int value = 1; };
    //template <> struct handler_width<write16sm_delegate> { static constexpr int value = 1; };
    //template <> struct handler_width<write16mo_delegate> { static constexpr int value = 1; };
    //template <> struct handler_width<write16smo_delegate> { static constexpr int value = 1; };
    //template <> struct handler_width<read32_delegate> { static constexpr int value = 2; };
    //template <> struct handler_width<read32m_delegate> { static constexpr int value = 2; };
    //template <> struct handler_width<read32s_delegate> { static constexpr int value = 2; };
    //template <> struct handler_width<read32sm_delegate> { static constexpr int value = 2; };
    //template <> struct handler_width<read32mo_delegate> { static constexpr int value = 2; };
    //template <> struct handler_width<read32smo_delegate> { static constexpr int value = 2; };
    //template <> struct handler_width<write32_delegate> { static constexpr int value = 2; };
    //template <> struct handler_width<write32m_delegate> { static constexpr int value = 2; };
    //template <> struct handler_width<write32s_delegate> { static constexpr int value = 2; };
    //template <> struct handler_width<write32sm_delegate> { static constexpr int value = 2; };
    //template <> struct handler_width<write32mo_delegate> { static constexpr int value = 2; };
    //template <> struct handler_width<write32smo_delegate> { static constexpr int value = 2; };
    //template <> struct handler_width<read64_delegate> { static constexpr int value = 3; };
    //template <> struct handler_width<read64m_delegate> { static constexpr int value = 3; };
    //template <> struct handler_width<read64s_delegate> { static constexpr int value = 3; };
    //template <> struct handler_width<read64sm_delegate> { static constexpr int value = 3; };
    //template <> struct handler_width<read64mo_delegate> { static constexpr int value = 3; };
    //template <> struct handler_width<read64smo_delegate> { static constexpr int value = 3; };
    //template <> struct handler_width<write64_delegate> { static constexpr int value = 3; };
    //template <> struct handler_width<write64m_delegate> { static constexpr int value = 3; };
    //template <> struct handler_width<write64s_delegate> { static constexpr int value = 3; };
    //template <> struct handler_width<write64sm_delegate> { static constexpr int value = 3; };
    //template <> struct handler_width<write64mo_delegate> { static constexpr int value = 3; };
    //template <> struct handler_width<write64smo_delegate> { static constexpr int value = 3; };
    //} // anonymous namespace

    //address_map_entry &memory_view::memory_view_entry::operator()(offs_t start, offs_t end)
    //{
    //    return (*m_map)(start, end);
    //}


    //template<int Level, int Width, int AddrShift, endianness_t Endian>
    //class memory_view_entry_specific : public memory_view::memory_view_entry
    class memory_view_entry_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> : memory_view.memory_view_entry
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using NativeType = uX;
        //using address_space_installer::install_read_tap;
        //using address_space_installer::install_write_tap;
        //using address_space_installer::install_readwrite_tap;


        // constants describing the native size
        //static constexpr u32 NATIVE_BYTES = 1 << Width;
        //static constexpr u32 NATIVE_STEP = AddrShift >= 0 ? NATIVE_BYTES << iabs(AddrShift) : NATIVE_BYTES >> iabs(AddrShift);
        //static constexpr u32 NATIVE_MASK = NATIVE_STEP - 1;
        //static constexpr u32 NATIVE_BITS = 8 * NATIVE_BYTES;

        //static constexpr offs_t offset_to_byte(offs_t offset) { return AddrShift < 0 ? offset << iabs(AddrShift) : offset >> iabs(AddrShift); }


        public memory_view_entry_specific(address_space_config config, memory_manager manager, memory_view view, int id) : base(config, manager, view, id)
        { }

        //virtual ~memory_view_entry_specific() = default;


        //handler_entry_read <Width, AddrShift, Endian> *r() { return static_cast<handler_entry_read <Width, AddrShift, Endian> *>(m_view.m_handler_read); }
        //handler_entry_write<Width, AddrShift, Endian> *w() { return static_cast<handler_entry_write<Width, AddrShift, Endian> *>(m_view.m_handler_write); }

        //void invalidate_caches(read_or_write readorwrite) { return m_view.m_space->invalidate_caches(readorwrite); }


        protected override void populate_from_map(address_map map = null)
        {
            throw new emu_unimplemented();
#if false
            // no map specified, use the space-specific one
            if (map == null)
                map = m_map;

            prepare_map_generic(map, true);

            // Force the slot to exist, in case the map is empty
            r().select_u(m_id);
            w().select_u(m_id);

            // install the handlers, using the original, unadjusted memory map
            foreach (address_map_entry entry in map.m_entrylist)
            {
                // map both read and write halves
                populate_map_entry(entry, read_or_write.READ);
                populate_map_entry(entry, read_or_write.WRITE);
            }
#endif
        }


        protected override memory_passthrough_handler install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<uX> tap, memory_passthrough_handler mph)
        {
            throw new emu_unimplemented();
#if false
            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            check_range_optimize_mirror("install_read_tap", addrstart, addrend, addrmirror, nstart, nend, nmask, nmirror);
            if (mph == null)
                mph = m_view.m_space->make_mph();

            r().select_u(m_id);
            w().select_u(m_id);

            var handler = new handler_entry_read_tap<Width, AddrShift, Endian>(m_view.m_space, mph, name, tap);
            r().populate_passthrough(nstart, nend, nmirror, handler);
            handler.unref();

            invalidate_caches(read_or_write.READ);

            return mph;
#endif
        }


        protected override memory_passthrough_handler install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<uX> tap, memory_passthrough_handler mph)
        {
            throw new emu_unimplemented();
#if false
            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            check_range_optimize_mirror("install_write_tap", addrstart, addrend, addrmirror, nstart, nend, nmask, nmirror);
            if (mph == null)
                mph = m_view.m_space->make_mph();

            r().select_u(m_id);
            w().select_u(m_id);

            var handler = new handler_entry_write_tap<Width, AddrShift, Endian>(m_view.m_space, mph, name, tap);
            w().populate_passthrough(nstart, nend, nmirror, handler);
            handler->unref();

            invalidate_caches(read_or_write.WRITE);

            return mph;
#endif
        }


        protected override memory_passthrough_handler install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<uX> tapr, install_tap_func<uX> tapw, memory_passthrough_handler mph)
        {
            throw new emu_unimplemented();
#if false
            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            check_range_optimize_mirror("install_readwrite_tap", addrstart, addrend, addrmirror, nstart, nend, nmask, nmirror);
            if (mph == null)
                mph = m_view.m_space->make_mph();

            r().select_u(m_id);
            w().select_u(m_id);

            var rhandler = new handler_entry_read_tap<Width, AddrShift, Endian>(m_view.m_space, mph, name, tapr);
            r().populate_passthrough(nstart, nend, nmirror, rhandler);
            rhandler.unref();

            var whandler = new handler_entry_write_tap<Width, AddrShift, Endian>(m_view.m_space, mph, name, tapw);
            w().populate_passthrough(nstart, nend, nmirror, whandler);
            whandler->unref();

            invalidate_caches(read_or_write.READWRITE);

            return mph;
#endif
        }


        protected override void unmap_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, bool quiet)
        {
            throw new emu_unimplemented();
#if false
            VPRINTF("memory_view::unmap({0}{1}-{2}{3} mirror={4}{5}, {6}, {7})\n",
                    m_addrchars, addrstart, m_addrchars, addrend,
                    m_addrchars, addrmirror,
                    (readorwrite == read_or_write.READ) ? "read" : (readorwrite == read_or_write.WRITE) ? "write" : (readorwrite == read_or_write.READWRITE) ? "read/write" : "??",
                    quiet ? "quiet" : "normal");

            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            check_range_optimize_mirror("unmap_generic", addrstart, addrend, addrmirror, nstart, nend, nmask, nmirror);

            r().select_u(m_id);
            w().select_u(m_id);

            // read space
            if (readorwrite == read_or_write.READ || readorwrite == read_or_write.READWRITE)
            {
                var handler = static_cast<handler_entry_read<Width, AddrShift, Endian> *>(quiet ? m_view.m_space->nop_r() : m_view.m_space->unmap_r());
                handler.ref_();
                r().populate(nstart, nend, nmirror, handler);
            }

            // write space
            if (readorwrite == read_or_write.WRITE || readorwrite == read_or_write.READWRITE)
            {
                var handler = static_cast<handler_entry_write<Width, AddrShift, Endian> *>(quiet ? m_view.m_space->nop_w() : m_view.m_space->unmap_w());
                handler.ref_();
                w().populate(nstart, nend, nmirror, handler);
            }

            invalidate_caches(readorwrite);
#endif
        }


        protected override void install_ram_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, PointerU8 baseptr) { throw new emu_unimplemented(); }
        protected override void install_bank_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_bank rbank, memory_bank wbank) { throw new emu_unimplemented(); }
        protected override void install_view(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_view view) { throw new emu_unimplemented(); }
        protected override void install_readwrite_port(offs_t addrstart, offs_t addrend, offs_t addrmirror, string rtag, string wtag) { throw new emu_unimplemented(); }
        protected override void install_device_delegate(offs_t addrstart, offs_t addrend, device_t device, address_map_constructor map, u64 unitmask, int cswidth) { throw new emu_unimplemented(); }


        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8_delegate rhandler, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16_delegate rhandler, write16_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32_delegate rhandler, write32_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64_delegate rhandler, write64_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8m_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8m_delegate rhandler, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16m_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16m_delegate rhandler, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32m_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32m_delegate rhandler, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64m_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64m_delegate rhandler, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16sm_delegate rhandler, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32sm_delegate rhandler, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64sm_delegate rhandler, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8mo_delegate rhandler, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16mo_delegate rhandler, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32mo_delegate rhandler, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64mo_delegate rhandler, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32smo_delegate rhandler, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64smo_delegate rhandler, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }


        //template<typename READ>
        void install_read_handler_impl<READ>(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ handler_r)
        {
            throw new emu_unimplemented();
#if false
            try { handler_r.resolve(); }
            catch (const binding_type_exception &) {
                osd_printf_error("Binding error while installing read handler %s for range 0x%X-0x%X mask 0x%X mirror 0x%X select 0x%X umask 0x%X\n", handler_r.name(), addrstart, addrend, addrmask, addrmirror, addrselect, unitmask);
                throw;
            }
            install_read_handler_helper<handler_width<READ>::value>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_r);
#endif
        }


        //template<typename WRITE>
        void install_write_handler_impl<WRITE>(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, WRITE handler_w)
        {
            throw new emu_unimplemented();
#if false
            try { handler_w.resolve(); }
            catch (const binding_type_exception &) {
                osd_printf_error("Binding error while installing write handler %s for range 0x%X-0x%X mask 0x%X mirror 0x%X select 0x%X umask 0x%X\n", handler_w.name(), addrstart, addrend, addrmask, addrmirror, addrselect, unitmask);
                throw;
            }
            install_write_handler_helper<handler_width<WRITE>::value>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_w);
#endif
        }


        //template<typename READ, typename WRITE>
        void install_readwrite_handler_impl<READ, WRITE>(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ handler_r, WRITE handler_w)
        {
            throw new emu_unimplemented();
#if false
            static_assert(handler_width<READ>::value == handler_width<WRITE>::value, "handler widths do not match");
            try { handler_r.resolve(); }
            catch (const binding_type_exception &) {
                osd_printf_error("Binding error while installing read handler %s for range 0x%X-0x%X mask 0x%X mirror 0x%X select 0x%X umask 0x%X\n", handler_r.name(), addrstart, addrend, addrmask, addrmirror, addrselect, unitmask);
                throw;
            }
            try { handler_w.resolve(); }
            catch (const binding_type_exception &) {
                osd_printf_error("Binding error while installing write handler %s for range 0x%X-0x%X mask 0x%X mirror 0x%X select 0x%X umask 0x%X\n", handler_w.name(), addrstart, addrend, addrmask, addrmirror, addrselect, unitmask);
                throw;
            }
            install_readwrite_handler_helper<handler_width<READ>::value>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_r, handler_w);
#endif
        }

        //template<int AccessWidth, typename READ>
        //void install_read_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, const READ &handler_r)
        //{
        //    if constexpr (Width < AccessWidth) {
        //        fatalerror("install_read_handler: cannot install a %d-wide handler in a %d-wide bus", 8 << AccessWidth, 8 << Width);
        //    } else {
        //        VPRINTF("memory_view::install_read_handler(%*x-%*x mask=%*x mirror=%*x, space width=%d, handler width=%d, %s, %*x)\n",
        //                m_addrchars, addrstart, m_addrchars, addrend,
        //                m_addrchars, addrmask, m_addrchars, addrmirror,
        //                8 << Width, 8 << AccessWidth,
        //                handler_r.name(), data_width() / 4, unitmask);

        //        offs_t nstart, nend, nmask, nmirror;
        //        u64 nunitmask;
        //        int ncswidth;
        //        check_optimize_all("install_read_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, nstart, nend, nmask, nmirror, nunitmask, ncswidth);

        //        if constexpr (Width == AccessWidth) {
        //            auto hand_r = new handler_entry_read_delegate<Width, AddrShift, Endian, READ>(m_view.m_space, handler_r);
        //            hand_r->set_address_info(nstart, nmask);
        //            r()->populate(nstart, nend, nmirror, hand_r);
        //        } else {
        //            auto hand_r = new handler_entry_read_delegate<AccessWidth, -AccessWidth, Endian, READ>(m_view.m_space, handler_r);
        //            memory_units_descriptor<Width, AddrShift, Endian> descriptor(AccessWidth, Endian, hand_r, nstart, nend, nmask, nunitmask, ncswidth);
        //            hand_r->set_address_info(descriptor.get_handler_start(), descriptor.get_handler_mask());
        //            r()->populate_mismatched(nstart, nend, nmirror, descriptor);
        //            hand_r->unref();
        //        }
        //        invalidate_caches(read_or_write::READ);
        //    }
        //}

        //template<int AccessWidth, typename WRITE>
        //void install_write_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
        //                             const WRITE &handler_w)
        //{
        //    if constexpr (Width < AccessWidth) {
        //        fatalerror("install_write_handler: cannot install a %d-wide handler in a %d-wide bus", 8 << AccessWidth, 8 << Width);
        //    } else {
        //        VPRINTF("memory_view::install_write_handler(%*x-%*x mask=%*x mirror=%*x, space width=%d, handler width=%d, %s, %*x)\n",
        //                m_addrchars, addrstart, m_addrchars, addrend,
        //                m_addrchars, addrmask, m_addrchars, addrmirror,
        //                8 << Width, 8 << AccessWidth,
        //                handler_w.name(), data_width() / 4, unitmask);
        //
        //        offs_t nstart, nend, nmask, nmirror;
        //        u64 nunitmask;
        //        int ncswidth;
        //        check_optimize_all("install_write_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, nstart, nend, nmask, nmirror, nunitmask, ncswidth);

        //        if constexpr (Width == AccessWidth) {
        //            auto hand_w = new handler_entry_write_delegate<Width, AddrShift, Endian, WRITE>(m_view.m_space, handler_w);
        //            hand_w->set_address_info(nstart, nmask);
        //            w()->populate(nstart, nend, nmirror, hand_w);
        //        } else {
        //            auto hand_w = new handler_entry_write_delegate<AccessWidth, -AccessWidth, Endian, WRITE>(m_view.m_space, handler_w);
        //            memory_units_descriptor<Width, AddrShift, Endian> descriptor(AccessWidth, Endian, hand_w, nstart, nend, nmask, nunitmask, ncswidth);
        //            hand_w->set_address_info(descriptor.get_handler_start(), descriptor.get_handler_mask());
        //            w()->populate_mismatched(nstart, nend, nmirror, descriptor);
        //            hand_w->unref();
        //        }
        //        invalidate_caches(read_or_write::WRITE);
        //    }
        //}

        //template<int AccessWidth, typename READ, typename WRITE>
        //void install_readwrite_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
        //                                 const READ  &handler_r,
        //                                 const WRITE &handler_w)
        //{
        //    if constexpr (Width < AccessWidth) {
        //        fatalerror("install_readwrite_handler: cannot install a %d-wide handler in a %d-wide bus", 8 << AccessWidth, 8 << Width);
        //    } else {
        //        VPRINTF("memory_view::install_readwrite_handler(%*x-%*x mask=%*x mirror=%*x, space width=%d, handler width=%d, %s, %s, %*x)\n",
        //                m_addrchars, addrstart, m_addrchars, addrend,
        //                m_addrchars, addrmask, m_addrchars, addrmirror,
        //                8 << Width, 8 << AccessWidth,
        //                handler_r.name(), handler_w.name(), data_width() / 4, unitmask);
        //
        //        offs_t nstart, nend, nmask, nmirror;
        //        u64 nunitmask;
        //        int ncswidth;
        //        check_optimize_all("install_readwrite_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, nstart, nend, nmask, nmirror, nunitmask, ncswidth);

        //        if constexpr (Width == AccessWidth) {
        //            auto hand_r = new handler_entry_read_delegate <Width, AddrShift, Endian, READ>(m_view.m_space, handler_r);
        //            hand_r->set_address_info(nstart, nmask);
        //            r() ->populate(nstart, nend, nmirror, hand_r);

        //            auto hand_w = new handler_entry_write_delegate<Width, AddrShift, Endian, WRITE>(m_view.m_space, handler_w);
        //            hand_w->set_address_info(nstart, nmask);
        //            w()->populate(nstart, nend, nmirror, hand_w);
        //        } else {
        //            auto hand_r = new handler_entry_read_delegate <AccessWidth, -AccessWidth, Endian, READ>(m_view.m_space, handler_r);
        //            memory_units_descriptor<Width, AddrShift, Endian> descriptor(AccessWidth, Endian, hand_r, nstart, nend, nmask, nunitmask, ncswidth);
        //            hand_r->set_address_info(descriptor.get_handler_start(), descriptor.get_handler_mask());
        //            r() ->populate_mismatched(nstart, nend, nmirror, descriptor);
        //            hand_r->unref();

        //            auto hand_w = new handler_entry_write_delegate<AccessWidth, -AccessWidth, Endian, WRITE>(m_view.m_space, handler_w);
        //            descriptor.set_subunit_handler(hand_w);
        //            hand_w->set_address_info(descriptor.get_handler_start(), descriptor.get_handler_mask());
        //            w()->populate_mismatched(nstart, nend, nmirror, descriptor);
        //            hand_w->unref();
        //        }
        //        invalidate_caches(read_or_write::READWRITE);
        //    }
        //}
    }


    //namespace {
    public static class emumem_mview_global
    {
        //template<int Level, int Width, int AddrShift, endianness_t Endian>
        public static memory_view.memory_view_entry mve_make_1<int_Level, int_Width, int_AddrShift, endianness_t_Endian>(address_space_config config, memory_manager manager, memory_view view, int id)  //template<int Level, int Width, int AddrShift, endianness_t Endian> memory_view::memory_view_entry *mve_make_1(const address_space_config &config, memory_manager &manager, memory_view &view, int id) {
        {
            return new memory_view_entry_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian>(config, manager, view, id);
        }

        //template<int Width, int AddrShift, endianness_t Endian>
        public static memory_view.memory_view_entry mve_make_2<int_Width, int_AddrShift, endianness_t_Endian>(int Level, address_space_config config, memory_manager manager, memory_view view, int id)  //template<int Width, int AddrShift, endianness_t Endian> memory_view::memory_view_entry *mve_make_2(int Level, const address_space_config &config, memory_manager &manager, memory_view &view, int id) {
        {
            switch (Level)
            {
                case 0: return mve_make_1<int_constant_0, int_Width, int_AddrShift, endianness_t_Endian>(config, manager, view, id);
                case 1: return mve_make_1<int_constant_1, int_Width, int_AddrShift, endianness_t_Endian>(config, manager, view, id);
                default: std.abort(); return null;
            }
        }

        //template<int Width, int AddrShift>
        public static memory_view.memory_view_entry mve_make_3<int_Width, int_AddrShift>(int Level, endianness_t Endian, address_space_config config, memory_manager manager, memory_view view, int id)  //template<int Width, int AddrShift> memory_view::memory_view_entry *mve_make_3(int Level, endianness_t Endian, const address_space_config &config, memory_manager &manager, memory_view &view, int id) {
        {
            switch (Endian)
            {
                case endianness_t.ENDIANNESS_LITTLE: return mve_make_2<int_Width, int_AddrShift, endianness_t_constant_ENDIANNESS_LITTLE>(Level, config, manager, view, id);
                case endianness_t.ENDIANNESS_BIG:    return mve_make_2<int_Width, int_AddrShift, endianness_t_constant_ENDIANNESS_BIG>   (Level, config, manager, view, id);
                default: std.abort(); return null;
            }
        }

        public static memory_view.memory_view_entry mve_make(int Level, int Width, int AddrShift, endianness_t Endian, address_space_config config, memory_manager manager, memory_view view, int id)
        {
            switch (Width | (AddrShift + 4))
            {
                case  8|(4+1): return mve_make_3<int_constant_0, int_constant_1> (Level, Endian, config, manager, view, id);
                case  8|(4-0): return mve_make_3<int_constant_0, int_constant_0> (Level, Endian, config, manager, view, id);
                case 16|(4+3): return mve_make_3<int_constant_1, int_constant_3> (Level, Endian, config, manager, view, id);
                case 16|(4-0): return mve_make_3<int_constant_1, int_constant_0> (Level, Endian, config, manager, view, id);
                case 16|(4-1): return mve_make_3<int_constant_1, int_constant_n1>(Level, Endian, config, manager, view, id);
                case 32|(4+3): return mve_make_3<int_constant_2, int_constant_3> (Level, Endian, config, manager, view, id);
                case 32|(4-0): return mve_make_3<int_constant_2, int_constant_0> (Level, Endian, config, manager, view, id);
                case 32|(4-1): return mve_make_3<int_constant_2, int_constant_n1>(Level, Endian, config, manager, view, id);
                case 32|(4-2): return mve_make_3<int_constant_2, int_constant_n2>(Level, Endian, config, manager, view, id);
                case 64|(4-0): return mve_make_3<int_constant_3, int_constant_0> (Level, Endian, config, manager, view, id);
                case 64|(4-1): return mve_make_3<int_constant_3, int_constant_n1>(Level, Endian, config, manager, view, id);
                case 64|(4-2): return mve_make_3<int_constant_3, int_constant_n2>(Level, Endian, config, manager, view, id);
                case 64|(4-3): return mve_make_3<int_constant_3, int_constant_n3>(Level, Endian, config, manager, view, id);
                default: std.abort(); return null;
            }
        }
    }


    //memory_view::memory_view_entry &memory_view::operator[](int slot)
    //memory_view::memory_view_entry::memory_view_entry(const address_space_config &config, memory_manager &manager, memory_view &view, int id) : address_space_installer(config, manager), m_view(view), m_id(id)
    //void memory_view::memory_view_entry::prepare_map_generic(address_map &map, bool allow_alloc)
    //void memory_view::memory_view_entry::prepare_device_map(address_map &map)
    //template<int Level, int Width, int AddrShift, endianness_t Endian> void memory_view_entry_specific<Level, Width, AddrShift, Endian>::populate_from_map(address_map *map)
    //std::string memory_view::memory_view_entry::key() const
    //memory_view::memory_view(device_t &device, std::string name) : m_device(device), m_name(name), m_config(nullptr), m_addrstart(0), m_addrend(0), m_space(nullptr), m_handler_read(nullptr), m_handler_write(nullptr), m_cur_id(-1), m_cur_slot(-1)
    //void memory_view::register_state()
    //void memory_view::disable()
    //void memory_view::select(int slot)
    //int memory_view::id_to_slot(int id) const
    //void memory_view::initialize_from_address_map(offs_t addrstart, offs_t addrend, const address_space_config &config)


    //namespace {
        //template<int Width, int AddrShift, endianness_t Endian> void h_make_1(int HighBits, address_space &space, memory_view &view, handler_entry *&r, handler_entry *&w) {
        //template<int Width, int AddrShift> void h_make_2(int HighBits, endianness_t Endian, address_space &space, memory_view &view, handler_entry *&r, handler_entry *&w) {
        //void h_make(int HighBits, int Width, int AddrShift, endianness_t Endian, address_space &space, memory_view &view, handler_entry *&r, handler_entry *&w) {
    //}


    //std::pair<handler_entry *, handler_entry *> memory_view::make_handlers(address_space &space, offs_t addrstart, offs_t addrend)
    //void memory_view::make_subdispatch(std::string context)
    //void memory_view::memory_view_entry::check_range_optimize_mirror(const char *function, offs_t addrstart, offs_t addrend, offs_t addrmirror, offs_t &nstart, offs_t &nend, offs_t &nmask, offs_t &nmirror)
    //void memory_view::memory_view_entry::check_range_optimize_all(const char *function, int width, offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, offs_t &nstart, offs_t &nend, offs_t &nmask, offs_t &nmirror, u64 &nunitmask, int &ncswidth)
    //void memory_view::memory_view_entry::check_range_address(const char *function, offs_t addrstart, offs_t addrend)
    //template<int Level, int Width, int AddrShift, endianness_t Endian> void memory_view_entry_specific<Level, Width, AddrShift, Endian>::install_ram_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, void *baseptr)
    //template<int Level, int Width, int AddrShift, endianness_t Endian> void memory_view_entry_specific<Level, Width, AddrShift, Endian>::unmap_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, bool quiet)
    //template<int Level, int Width, int AddrShift, endianness_t Endian> void memory_view_entry_specific<Level, Width, AddrShift, Endian>::install_view(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_view &view)
    //template<int Level, int Width, int AddrShift, endianness_t Endian> memory_passthrough_handler *memory_view_entry_specific<Level, Width, AddrShift, Endian>::install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, uX &data, uX mem_mask)> tap, memory_passthrough_handler *mph)
    //template<int Level, int Width, int AddrShift, endianness_t Endian> memory_passthrough_handler *memory_view_entry_specific<Level, Width, AddrShift, Endian>::install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, uX &data, uX mem_mask)> tap, memory_passthrough_handler *mph)
    //template<int Level, int Width, int AddrShift, endianness_t Endian> memory_passthrough_handler *memory_view_entry_specific<Level, Width, AddrShift, Endian>::install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, uX &data, uX mem_mask)> tapr, std::function<void (offs_t offset, uX &data, uX mem_mask)> tapw, memory_passthrough_handler *mph)
    //template<int Level, int Width, int AddrShift, endianness_t Endian> void memory_view_entry_specific<Level, Width, AddrShift, Endian>::install_device_delegate(offs_t addrstart, offs_t addrend, device_t &device, address_map_constructor &delegate, u64 unitmask, int cswidth)
    //template<int Level, int Width, int AddrShift, endianness_t Endian> void memory_view_entry_specific<Level, Width, AddrShift, Endian>::install_readwrite_port(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string rtag, std::string wtag)
    //template<int Level, int Width, int AddrShift, endianness_t Endian> void memory_view_entry_specific<Level, Width, AddrShift, Endian>::install_bank_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_bank *rbank, memory_bank *wbank)
}
