// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using endianness_t = mame.util.endianness;  //using endianness_t = util::endianness;
using offs_t = System.UInt32;  //using offs_t = u32;
using PointerU8 = mame.Pointer<System.Byte>;
using size_t = System.UInt64;
using u16 = System.UInt16;
using u64 = System.UInt64;
using uX = mame.FlexPrim;

using static mame.emu.detail.emumem_global;
using static mame.emucore_global;
using static mame.emumem_mview_global;


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


    public partial class memory_view
    {
        public abstract partial class memory_view_entry : address_space_installer
        {
            //address_map_entry &operator()(offs_t start, offs_t end);
            public address_map_entry op(offs_t start, offs_t end)  //address_map_entry &memory_view::memory_view_entry::operator()(offs_t start, offs_t end)
            {
                return m_map.op(start, end);  //return (*m_map)(start, end);
            }
        }
    }


    //template<int Level, int Width, int AddrShift>
    class memory_view_entry_specific<int_Level, int_Width, int_AddrShift> : memory_view.memory_view_entry  //class memory_view_entry_specific : public memory_view::memory_view_entry
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


        //handler_entry_read <Width, AddrShift> *r() { return static_cast<handler_entry_read <Width, AddrShift> *>(m_view.m_handler_read); }
        //handler_entry_write<Width, AddrShift> *w() { return static_cast<handler_entry_write<Width, AddrShift> *>(m_view.m_handler_write); }

        //void invalidate_caches(read_or_write readorwrite) { return m_view.m_space->invalidate_caches(readorwrite); }


        protected override void populate_from_map(address_map map = null)
        {
            throw new emu_unimplemented();
#if false
            // no map specified, use the space-specific one and import the submaps
            if (map == nullptr) {
                map = m_map.get();
                map->import_submaps(m_manager.machine(), m_view.m_device, data_width(), endianness(), addr_shift());
            }

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
            offs_t nstart, nend, nmask, nmirror;
            check_range_optimize_mirror("install_read_tap", addrstart, addrend, addrmirror, nstart, nend, nmask, nmirror);
            auto impl = m_view.m_space->make_mph(mph);

            r()->select_u(m_id);
            w()->select_u(m_id);

            auto handler = new handler_entry_read_tap<Width, AddrShift>(m_view.m_space, *impl, name, tap);
            r()->populate_passthrough(nstart, nend, nmirror, handler);
            handler->unref();

            invalidate_caches(read_or_write::READ);

            return impl;
#endif
        }


        protected override memory_passthrough_handler install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<uX> tap, memory_passthrough_handler mph)
        {
            throw new emu_unimplemented();
#if false
            offs_t nstart, nend, nmask, nmirror;
            check_range_optimize_mirror("install_write_tap", addrstart, addrend, addrmirror, nstart, nend, nmask, nmirror);
            auto impl = m_view.m_space->make_mph(mph);

            r()->select_u(m_id);
            w()->select_u(m_id);

            auto handler = new handler_entry_write_tap<Width, AddrShift>(m_view.m_space, *impl, name, tap);
            w()->populate_passthrough(nstart, nend, nmirror, handler);
            handler->unref();

            invalidate_caches(read_or_write::WRITE);

            return impl;
#endif
        }


        protected override memory_passthrough_handler install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<uX> tapr, install_tap_func<uX> tapw, memory_passthrough_handler mph)
        {
            throw new emu_unimplemented();
#if false
            offs_t nstart, nend, nmask, nmirror;
            check_range_optimize_mirror("install_readwrite_tap", addrstart, addrend, addrmirror, nstart, nend, nmask, nmirror);
            auto impl = m_view.m_space->make_mph(mph);

            r()->select_u(m_id);
            w()->select_u(m_id);

            auto rhandler = new handler_entry_read_tap <Width, AddrShift>(m_view.m_space, *impl, name, tapr);
            r() ->populate_passthrough(nstart, nend, nmirror, rhandler);
            rhandler->unref();

            auto whandler = new handler_entry_write_tap<Width, AddrShift>(m_view.m_space, *impl, name, tapw);
            w()->populate_passthrough(nstart, nend, nmirror, whandler);
            whandler->unref();

            invalidate_caches(read_or_write::READWRITE);

            return impl;
#endif
        }


        protected override void unmap_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, u16 flags, read_or_write readorwrite, bool quiet) { throw new emu_unimplemented(); }
        protected override void install_ram_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, u16 flags, read_or_write readorwrite, PointerU8 baseptr) { throw new emu_unimplemented(); }
        protected override void install_bank_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, u16 flags, memory_bank rbank, memory_bank wbank) { throw new emu_unimplemented(); }
        protected override void install_view(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_view view) { throw new emu_unimplemented(); }
        protected override void install_readwrite_port(offs_t addrstart, offs_t addrend, offs_t addrmirror, u16 flags, string rtag, string wtag) { throw new emu_unimplemented(); }
        protected override void install_device_delegate(offs_t addrstart, offs_t addrend, device_t device, address_map_constructor map, u64 unitmask, int cswidth, u16 flags) { throw new emu_unimplemented(); }


        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8_delegate rhandler, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16_delegate rhandler, write16_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32_delegate rhandler, write32_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64_delegate rhandler, write64_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8m_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8m_delegate rhandler, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16m_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16m_delegate rhandler, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32m_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32m_delegate rhandler, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64m_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64m_delegate rhandler, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        public override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16sm_delegate rhandler, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32sm_delegate rhandler, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64sm_delegate rhandler, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8mo_delegate rhandler, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16mo_delegate rhandler, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32mo_delegate rhandler, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64mo_delegate rhandler, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32smo_delegate rhandler, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64smo_delegate rhandler, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0, u16 flags = 0)
        { install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, flags, rhandler, whandler); }


        //template<typename READ>
        void install_read_handler_impl<READ>(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, u16 flags, READ handler_r)
        {
            throw new emu_unimplemented();
        }


        //template<typename WRITE>
        void install_write_handler_impl<WRITE>(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, u16 flags, WRITE handler_w)
        {
            throw new emu_unimplemented();
        }


        //template<typename READ, typename WRITE>
        void install_readwrite_handler_impl<READ, WRITE>(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, u16 flags, READ handler_r, WRITE handler_w)
        {
            throw new emu_unimplemented();
        }


        //template<int AccessWidth, typename READ>
        //void install_read_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, const READ &handler_r)

        //template<int AccessWidth, typename WRITE>
        //void install_write_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
        //                             const WRITE &handler_w)

        //template<int AccessWidth, typename READ, typename WRITE>
        //void install_readwrite_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
        //                                 const READ  &handler_r,
        //                                 const WRITE &handler_w)
    }


    //namespace {
    public static class emumem_mview_global
    {
        //template<int Level, int Width, int AddrShift>
        public static memory_view.memory_view_entry mve_make_1<int_Level, int_Width, int_AddrShift>(address_space_config config, memory_manager manager, memory_view view, int id)  //template<int Level, int Width, int AddrShift> memory_view::memory_view_entry *mve_make_1(const address_space_config &config, memory_manager &manager, memory_view &view, int id) {
        {
            return new memory_view_entry_specific<int_Level, int_Width, int_AddrShift>(config, manager, view, id);
        }

        //template<int Width, int AddrShift>
        public static memory_view.memory_view_entry mve_make_2<int_Width, int_AddrShift>(int Level, address_space_config config, memory_manager manager, memory_view view, int id)  //template<int Width, int AddrShift> memory_view::memory_view_entry *mve_make_2(int Level, const address_space_config &config, memory_manager &manager, memory_view &view, int id) {
        {
            switch (Level)
            {
                case 0: return mve_make_1<int_const_0, int_Width, int_AddrShift>(config, manager, view, id);
                case 1: return mve_make_1<int_const_1, int_Width, int_AddrShift>(config, manager, view, id);
                default: std.abort(); return null;
            }
        }

        public static memory_view.memory_view_entry mve_make(int Level, int Width, int AddrShift, endianness_t Endian, address_space_config config, memory_manager manager, memory_view view, int id)
        {
            switch (Width | (AddrShift + 4))
            {
                case  8|(4+1): return mve_make_2<int_const_0, int_const_1> (Level, config, manager, view, id);
                case  8|(4-0): return mve_make_2<int_const_0, int_const_0> (Level, config, manager, view, id);
                case 16|(4+3): return mve_make_2<int_const_1, int_const_3> (Level, config, manager, view, id);
                case 16|(4-0): return mve_make_2<int_const_1, int_const_0> (Level, config, manager, view, id);
                case 16|(4-1): return mve_make_2<int_const_1, int_const_n1>(Level, config, manager, view, id);
                case 32|(4+3): return mve_make_2<int_const_2, int_const_3> (Level, config, manager, view, id);
                case 32|(4-0): return mve_make_2<int_const_2, int_const_0> (Level, config, manager, view, id);
                case 32|(4-1): return mve_make_2<int_const_2, int_const_n1>(Level, config, manager, view, id);
                case 32|(4-2): return mve_make_2<int_const_2, int_const_n2>(Level, config, manager, view, id);
                case 64|(4-0): return mve_make_2<int_const_3, int_const_0> (Level, config, manager, view, id);
                case 64|(4-1): return mve_make_2<int_const_3, int_const_n1>(Level, config, manager, view, id);
                case 64|(4-2): return mve_make_2<int_const_3, int_const_n2>(Level, config, manager, view, id);
                case 64|(4-3): return mve_make_2<int_const_3, int_const_n3>(Level, config, manager, view, id);
                default: std.abort(); return null;
            }
        }
    }


    public partial class memory_view
    {
        //memory_view_entry &operator[](int slot);
        public memory_view_entry op(int slot)
        {
            if (m_config == null)
                fatalerror("A view must be in a map or a space before it can be setup.");

            var i = m_entry_mapping.find(slot);
            if (i == default)
            {
                memory_view_entry e;
                int id = (int)m_entries.size();
                e = mve_make(handler_entry_dispatch_level(m_config.addr_width()), m_config.data_width(), m_config.addr_shift(), m_config.endianness(),
                             m_config, m_device.machine().memory(), this, id);
                m_entries.resize((size_t)id + 1);
                m_entries[id] = e;  //m_entries[id].reset(e);
                m_entry_mapping[slot] = id;
                if (m_handler_read != null)
                {
                    m_handler_read.select_u(id);
                    m_handler_write.select_u(id);
                }
                return e;
            }
            else
            {
                return m_entries[i];
            }
        }
    }


    public partial class memory_view
    {
        public abstract partial class memory_view_entry : address_space_installer
        {
            protected memory_view_entry(address_space_config config, memory_manager manager, memory_view view, int id)
                 : base(config, manager)
            {
                throw new emu_unimplemented();
            }


            //void memory_view::memory_view_entry::prepare_map_generic(address_map &map, bool allow_alloc)
            //void memory_view::memory_view_entry::prepare_device_map(address_map &map)

            //std::string memory_view::memory_view_entry::key() const
        }
    }


    public partial class memory_view
    {
        public memory_view(device_t device, string name)
        {
            m_device = device;
            m_name = name;
            m_config = null;
            m_addrstart = 0;
            m_addrend = 0;
            m_space = null;
            m_handler_read = null;
            m_handler_write = null;
            m_cur_id = -1;
            m_cur_slot = -1;


            device.view_register(this);
        }


        public void register_state()
        {
            throw new emu_unimplemented();
        }


        //void memory_view::disable()
        //void memory_view::select(int slot)
        //int memory_view::id_to_slot(int id) const
        //void memory_view::initialize_from_address_map(offs_t addrstart, offs_t addrend, const address_space_config &config)
    }


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
    //template<int Level, int Width, int AddrShift> void memory_view_entry_specific<Level, Width, AddrShift>::install_ram_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, u16 flags, read_or_write readorwrite, void *baseptr)
    //template<int Level, int Width, int AddrShift> void memory_view_entry_specific<Level, Width, AddrShift>::unmap_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, u16 flags, read_or_write readorwrite, bool quiet)
    //template<int Level, int Width, int AddrShift, endianness_t Endian> void memory_view_entry_specific<Level, Width, AddrShift, Endian>::install_view(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_view &view)
    //template<int Level, int Width, int AddrShift, endianness_t Endian> memory_passthrough_handler *memory_view_entry_specific<Level, Width, AddrShift, Endian>::install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, uX &data, uX mem_mask)> tap, memory_passthrough_handler *mph)
    //template<int Level, int Width, int AddrShift, endianness_t Endian> memory_passthrough_handler *memory_view_entry_specific<Level, Width, AddrShift, Endian>::install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, uX &data, uX mem_mask)> tap, memory_passthrough_handler *mph)
    //template<int Level, int Width, int AddrShift, endianness_t Endian> memory_passthrough_handler *memory_view_entry_specific<Level, Width, AddrShift, Endian>::install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, uX &data, uX mem_mask)> tapr, std::function<void (offs_t offset, uX &data, uX mem_mask)> tapw, memory_passthrough_handler *mph)
    //template<int Level, int Width, int AddrShift> void memory_view_entry_specific<Level, Width, AddrShift>::install_device_delegate(offs_t addrstart, offs_t addrend, device_t &device, address_map_constructor &delegate, u64 unitmask, int cswidth, u16 flags)
    //template<int Level, int Width, int AddrShift> void memory_view_entry_specific<Level, Width, AddrShift>::install_readwrite_port(offs_t addrstart, offs_t addrend, offs_t addrmirror, u16 flags, std::string rtag, std::string wtag)
    //template<int Level, int Width, int AddrShift> void memory_view_entry_specific<Level, Width, AddrShift>::install_bank_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, u16 flags, memory_bank *rbank, memory_bank *wbank)
}
