// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Reflection;

using offs_t = System.UInt32;
using PointerU8 = mame.Pointer<System.Byte>;
using u8 = System.Byte;
using u16 = System.UInt16;
using u64 = System.UInt64;


namespace mame
{
    // address map handler types
    public enum map_handler_type
    {
        AMH_NONE = 0,
        AMH_RAM,
        AMH_ROM,
        AMH_NOP,
        AMH_UNMAP,
        AMH_DEVICE_DELEGATE,
        AMH_DEVICE_DELEGATE_M,
        AMH_DEVICE_DELEGATE_S,
        AMH_DEVICE_DELEGATE_SM,
        AMH_DEVICE_DELEGATE_MO,
        AMH_DEVICE_DELEGATE_SMO,
        AMH_PORT,
        AMH_BANK,
        AMH_DEVICE_SUBMAP,
        AMH_VIEW
    }


    // address map handler data
    public class map_handler_data
    {
        public map_handler_type m_type = map_handler_type.AMH_NONE;             // type of the handler
        public u8 m_bits = 0;             // width of the handler in bits, or 0 for default
        public string m_name = null;             // name of the handler
        public string m_tag = null;              // tag for I/O ports and banks


        public map_handler_data() { }
    }


    // ======================> address_map_entry
    // address_map_entry is a linked list element describing one address range in a map
    public class address_map_entry : global_object, simple_list_item<address_map_entry>
    {
        //friend class address_map;

        //template <typename T, typename Ret, typename... Params>
        //struct is_addrmap_method { static constexpr bool value = std::is_constructible<address_map_constructor, Ret (T::*)(Params...), const char *, T*>::value; };

        //template <typename T, typename Ret, typename... Params>
        //static std::enable_if_t<is_addrmap_method<T, Ret, Params...>::value, address_map_constructor> make_delegate(Ret (T::*func)(Params...), const char *name, T *obj)
        //{ return address_map_constructor(func, name, obj); }


        //template <typename T, bool Reqd>
        static device_t find_device<T, Reqd>(device_finder<T, Reqd> finder)  //static device_t &find_device(const device_finder<T, Reqd> &finder) {
            where Reqd : bool_constant, new()
        {
            std.pair<device_t, string> target = finder.finder_target();  //const std::pair<device_t &, const char *> target(finder.finder_target());
            device_t device = target.first.subdevice(target.second);  //device_t *device(target.first.subdevice(target.second));
            if (device == null)
                throw new emu_fatalerror("Device {0} not found in %s\n", target.second, target.first.tag());
            return device;
        }


        //template <typename T, typename U>
        //static T *make_pointer(U &obj)
        //{
        //    if constexpr (std::is_convertible_v<std::add_pointer_t<U>, std::add_pointer_t<T> >)
        //        return &downcast<T &>(obj);
        //    else
        //        return &dynamic_cast<T &>(obj);
        //}

        //template <typename T> static std::enable_if_t<emu::detail::is_device_implementation<T>::value, const char *> get_tag(T &obj) { return obj.tag(); }
        //template <typename T> static std::enable_if_t<emu::detail::is_device_interface<T>::value, const char *> get_tag(T &obj) { return obj.device().tag(); }


        // public state
        address_map_entry m_next;                 // pointer to the next entry
        address_map m_map;                  // reference to our owning map
        public device_t m_devbase;              // reference to "base" device for tag lookups

        // basic information
        public offs_t m_addrstart;            // start address
        public offs_t m_addrend;              // end address
        public offs_t m_addrmirror;           // mirror bits
        public offs_t m_addrmask;             // mask bits
        public offs_t m_addrselect;           // select bits
        public u64 m_mask;                 // mask for which lanes apply
        public int m_cswidth;              // chip select width override
        public map_handler_data m_read = new map_handler_data();                 // data for read handler
        public map_handler_data m_write = new map_handler_data();                // data for write handler
        public string m_share;                // tag of a shared memory block
        public string m_region;               // tag of region containing the memory backing this entry
        public offs_t m_rgnoffs;              // offset within the region

        // handlers
        public read8_delegate          m_rproto8;              // 8-bit read proto-delegate
        public read16_delegate         m_rproto16;             // 16-bit read proto-delegate
        public read32_delegate         m_rproto32;             // 32-bit read proto-delegate
        public read64_delegate         m_rproto64;             // 64-bit read proto-delegate
        public write8_delegate         m_wproto8;              // 8-bit write proto-delegate
        public write16_delegate        m_wproto16;             // 16-bit write proto-delegate
        public write32_delegate        m_wproto32;             // 32-bit write proto-delegate
        public write64_delegate        m_wproto64;             // 64-bit write proto-delegate

        public read8m_delegate         m_rproto8m;             // 8-bit read proto-delegate
        public read16m_delegate        m_rproto16m;            // 16-bit read proto-delegate
        public read32m_delegate        m_rproto32m;            // 32-bit read proto-delegate
        public read64m_delegate        m_rproto64m;            // 64-bit read proto-delegate
        public write8m_delegate        m_wproto8m;             // 8-bit write proto-delegate
        public write16m_delegate       m_wproto16m;            // 16-bit write proto-delegate
        public write32m_delegate       m_wproto32m;            // 32-bit write proto-delegate
        public write64m_delegate       m_wproto64m;            // 64-bit write proto-delegate

        public read8s_delegate         m_rproto8s;             // 8-bit read proto-delegate
        public read16s_delegate        m_rproto16s;            // 16-bit read proto-delegate
        public read32s_delegate        m_rproto32s;            // 32-bit read proto-delegate
        public read64s_delegate        m_rproto64s;            // 64-bit read proto-delegate
        public write8s_delegate        m_wproto8s;             // 8-bit write proto-delegate
        public write16s_delegate       m_wproto16s;            // 16-bit write proto-delegate
        public write32s_delegate       m_wproto32s;            // 32-bit write proto-delegate
        public write64s_delegate       m_wproto64s;            // 64-bit write proto-delegate

        public read8sm_delegate        m_rproto8sm;            // 8-bit read proto-delegate
        public read16sm_delegate       m_rproto16sm;           // 16-bit read proto-delegate
        public read32sm_delegate       m_rproto32sm;           // 32-bit read proto-delegate
        public read64sm_delegate       m_rproto64sm;           // 64-bit read proto-delegate
        public write8sm_delegate       m_wproto8sm;            // 8-bit write proto-delegate
        public write16sm_delegate      m_wproto16sm;           // 16-bit write proto-delegate
        public write32sm_delegate      m_wproto32sm;           // 32-bit write proto-delegate
        public write64sm_delegate      m_wproto64sm;           // 64-bit write proto-delegate

        public read8mo_delegate        m_rproto8mo;            // 8-bit read proto-delegate
        public read16mo_delegate       m_rproto16mo;           // 16-bit read proto-delegate
        public read32mo_delegate       m_rproto32mo;           // 32-bit read proto-delegate
        public read64mo_delegate       m_rproto64mo;           // 64-bit read proto-delegate
        public write8mo_delegate       m_wproto8mo;            // 8-bit write proto-delegate
        public write16mo_delegate      m_wproto16mo;           // 16-bit write proto-delegate
        public write32mo_delegate      m_wproto32mo;           // 32-bit write proto-delegate
        public write64mo_delegate      m_wproto64mo;           // 64-bit write proto-delegate

        public read8smo_delegate       m_rproto8smo;           // 8-bit read proto-delegate
        public read16smo_delegate      m_rproto16smo;          // 16-bit read proto-delegate
        public read32smo_delegate      m_rproto32smo;          // 32-bit read proto-delegate
        public read64smo_delegate      m_rproto64smo;          // 64-bit read proto-delegate
        public write8smo_delegate      m_wproto8smo;           // 8-bit write proto-delegate
        public write16smo_delegate     m_wproto16smo;          // 16-bit write proto-delegate
        public write32smo_delegate     m_wproto32smo;          // 32-bit write proto-delegate
        public write64smo_delegate     m_wproto64smo;          // 64-bit write proto-delegate

        public device_t m_submap_device;
        public address_map_constructor m_submap_delegate;
        public memory_view m_view;

        // information used during processing
        public PointerU8 m_memory;  //void *                  m_memory;               // pointer to memory backing this entry


        // construction/destruction

        //-------------------------------------------------
        //  address_map_entry - constructor
        //-------------------------------------------------
        public address_map_entry(device_t device, address_map map, offs_t start, offs_t end)
        {
            m_next = null;
            m_map = map;
            m_devbase = device;
            m_addrstart = start;
            m_addrend = end;
            m_addrmirror = 0;
            m_addrmask = 0;
            m_addrselect = 0;
            m_mask = 0;
            m_cswidth = 0;
            m_share = null;
            m_region = null;
            m_rgnoffs = 0;
            m_rproto8 = null;
            m_rproto16 = null;
            m_rproto32 = null;
            m_rproto64 = null;
            m_wproto8 = null;
            m_wproto16 = null;
            m_wproto32 = null;
            m_wproto64 = null;
            m_rproto8m = null;
            m_rproto16m = null;
            m_rproto32m = null;
            m_rproto64m = null;
            m_wproto8m = null;
            m_wproto16m = null;
            m_wproto32m = null;
            m_wproto64m = null;
            m_rproto8s = null;
            m_rproto16s = null;
            m_rproto32s = null;
            m_rproto64s = null;
            m_wproto8s = null;
            m_wproto16s = null;
            m_wproto32s = null;
            m_wproto64s = null;
            m_rproto8sm = null;
            m_rproto16sm = null;
            m_rproto32sm = null;
            m_rproto64sm = null;
            m_wproto8sm = null;
            m_wproto16sm = null;
            m_wproto32sm = null;
            m_wproto64sm = null;
            m_rproto8mo = null;
            m_rproto16mo = null;
            m_rproto32mo = null;
            m_rproto64mo = null;
            m_wproto8mo = null;
            m_wproto16mo = null;
            m_wproto32mo = null;
            m_wproto64mo = null;
            m_rproto8smo = null;
            m_rproto16smo = null;
            m_rproto32smo = null;
            m_rproto64smo = null;
            m_wproto8smo = null;
            m_wproto16smo = null;
            m_wproto32smo = null;
            m_wproto64smo = null;
            m_submap_device = null;
            m_memory = null;
        }


        // getters
        public address_map_entry next() { return m_next; }
        public address_map_entry m_next_get() { return m_next; }
        public void m_next_set(address_map_entry value) { m_next = value; }


        // simple inline setters
        public address_map_entry mirror(offs_t _mirror) { m_addrmirror = _mirror; return this; }
        //address_map_entry &select(offs_t _select) { m_addrselect = _select; return *this; }
        public address_map_entry region(string tag, offs_t offset) { m_region = tag; m_rgnoffs = offset; return this; }
        public address_map_entry share(string tag) { m_share = tag; return this; }


        // slightly less simple inline setters
        //template<bool _reqd> address_map_entry &region(const memory_region_finder<_reqd> &finder, offs_t offset) {
        //    const std::pair<device_t &, const char *> target(finder.finder_target());
        //    assert(&target.first == &m_devbase);
        //    return region(target.second, offset);
        //}
        //template<typename _ptrt, bool _reqd> address_map_entry &region(const region_ptr_finder<_ptrt, _reqd> &finder, offs_t offset) {
        //    const std::pair<device_t &, const char *> target(finder.finder_target());
        //    assert(&target.first == &m_devbase);
        //    return region(target.second, offset);
        //}


        //template<typename _ptrt, bool _reqd>
        public address_map_entry share(finder_base finder)  //template<typename _ptrt, bool _reqd> address_map_entry &share(const shared_ptr_finder<_ptrt, _reqd> &finder) {
        {
            std.pair<device_t, string> target = finder.finder_target();
            assert(target.first == m_devbase);
            return share(target.second);
        }


        public address_map_entry rom() { m_read.m_type = map_handler_type.AMH_ROM; return this; }
        public address_map_entry ram() { m_read.m_type = map_handler_type.AMH_RAM; m_write.m_type = map_handler_type.AMH_RAM; return this; }
        public address_map_entry readonly_() { m_read.m_type = map_handler_type.AMH_RAM; return this; }
        public address_map_entry writeonly() { m_write.m_type = map_handler_type.AMH_RAM; return this; }
        public address_map_entry unmaprw() { m_read.m_type = map_handler_type.AMH_UNMAP; m_write.m_type = map_handler_type.AMH_UNMAP; return this; }
        //address_map_entry &unmapr() { m_read.m_type = AMH_UNMAP; return *this; }
        //address_map_entry &unmapw() { m_write.m_type = AMH_UNMAP; return *this; }
        public address_map_entry noprw() { m_read.m_type = map_handler_type.AMH_NOP; m_write.m_type = map_handler_type.AMH_NOP; return this; }
        public address_map_entry nopr() { m_read.m_type = map_handler_type.AMH_NOP; return this; }
        public address_map_entry nopw() { m_write.m_type = map_handler_type.AMH_NOP; return this; }


        // address mask setting
        //address_map_entry &mask(offs_t _mask);


        // unit mask setting
        public address_map_entry umask16(u16 _mask) { m_mask = ((u64)_mask << 48) | ((u64)_mask << 32) | ((u64)_mask << 16) | _mask; return this; }
        //address_map_entry &umask32(u32 _mask);
        public address_map_entry umask64(u64 _mask) { m_mask = _mask; return this; }


        // chip select width setting
        public address_map_entry cswidth(int _cswidth) { m_cswidth = _cswidth; return this; }


        // I/O port configuration
        public address_map_entry portr(string tag) { m_read.m_type = map_handler_type.AMH_PORT; m_read.m_tag = tag; return this; }
        //address_map_entry &portw(const char *tag) { m_write.m_type = AMH_PORT; m_write.m_tag = tag; return *this; }
        //address_map_entry &portrw(const char *tag) { read_port(tag); write_port(tag); return *this; }


        // memory bank configuration
        public address_map_entry bankr(string tag) { m_read.m_type = map_handler_type.AMH_BANK; m_read.m_tag = tag; return this; }
        //address_map_entry &bankw(const char *tag) { m_write.m_type = AMH_BANK; m_write.m_tag = tag; return *this; }
        //address_map_entry &bankrw(const char *tag) { read_bank(tag); write_bank(tag); return *this; }


        //template<bool _reqd> address_map_entry &bankr(const memory_bank_finder<_reqd> &finder) {
        //    const std::pair<device_t &, const char *> target(finder.finder_target());
        //    assert(&target.first == &m_devbase);
        //    return bankr(target.second);
        //}
        //template<bool _reqd> address_map_entry &bankw(const memory_bank_finder<_reqd> &finder) {
        //    const std::pair<device_t &, const char *> target(finder.finder_target());
        //    assert(&target.first == &m_devbase);
        //    return bankw(target.second);
        //}
        //template<bool _reqd> address_map_entry &bankrw(const memory_bank_finder<_reqd> &finder) {
        //    const std::pair<device_t &, const char *> target(finder.finder_target());
        //    assert(&target.first == &m_devbase);
        //    return bankrw(target.second);
        //}


        // type setters
        //address_map_entry &set_read_type(map_handler_type _type) { m_read.m_type = _type; return *this; }
        //address_map_entry &set_write_type(map_handler_type _type) { m_write.m_type = _type; return *this; }


        // submap referencing
        //-------------------------------------------------
        //  m - set up a handler for
        //  retrieve a submap from a device
        //-------------------------------------------------
        public address_map_entry m(string tag, address_map_constructor func)
        {
            m_read.m_type = map_handler_type.AMH_DEVICE_SUBMAP;
            m_read.m_tag = tag;
            m_write.m_type = map_handler_type.AMH_DEVICE_SUBMAP;
            m_write.m_tag = tag;
            m_submap_device = null;
            m_submap_delegate = func;

            return this;
        }

        public address_map_entry m(device_t device, address_map_constructor func)
        {
            m_read.m_type = map_handler_type.AMH_DEVICE_SUBMAP;
            m_read.m_tag = null;
            m_write.m_type = map_handler_type.AMH_DEVICE_SUBMAP;
            m_write.m_tag = null;
            m_submap_device = device;
            m_submap_delegate = func;

            return this;
        }


        // view initialization
        public void view(memory_view mv)
        {
            m_read.m_type = map_handler_type.AMH_VIEW;
            m_read.m_tag = null;
            m_write.m_type = map_handler_type.AMH_VIEW;
            m_write.m_tag = null;
            m_view = mv;
            mv.initialize_from_address_map(m_addrstart, m_addrend, m_map.get_config());
        }


        // implicit base -> delegate converter
        //template <typename T, typename Ret, typename... Params>
        //address_map_entry &r(Ret (T::*read)(Params...), const char *read_name)
        //{ return r(emu::detail::make_delegate(*make_pointer<T>(m_devbase), read, read_name)); }

        //template <typename T, typename Ret, typename... Params>
        //address_map_entry &w(Ret (T::*write)(Params...), const char *write_name)
        //{ return w(emu::detail::make_delegate(*make_pointer<T>(m_devbase), write, write_name)); }

        //template <typename T, typename RetR, typename... ParamsR, typename U, typename RetW, typename... ParamsW>
        //address_map_entry &rw(RetR (T::*read)(ParamsR...), const char *read_name, RetW (U::*write)(ParamsW...), const char *write_name)
        //{ return r(emu::detail::make_delegate(*make_pointer<T>(m_devbase), read, read_name)).w(emu::detail::make_delegate(*make_pointer<U>(m_devbase), write, write_name)); }
        public address_map_entry rw(read8_delegate rfunc, write8_delegate wfunc) { return r(rfunc).w(wfunc); }
        public address_map_entry rw(read8sm_delegate rfunc, write8sm_delegate wfunc) { return r(rfunc).w(wfunc); }
        public address_map_entry rw(read8sm_delegate rfunc, write8smo_delegate wfunc) { return r(rfunc).w(wfunc); }
        public address_map_entry rw(read8smo_delegate rfunc, write8smo_delegate wfunc) { return r(rfunc).w(wfunc); }
        public address_map_entry rw(read8smo_delegate rfunc, write8sm_delegate wfunc) { return r(rfunc).w(wfunc); }
        public address_map_entry rw(read16_delegate rfunc, write16_delegate wfunc) { return r(rfunc).w(wfunc); }
        public address_map_entry rw(read16m_delegate rfunc, write16m_delegate wfunc) { return r(rfunc).w(wfunc); }
        public address_map_entry rw(read16s_delegate rfunc, write16s_delegate wfunc) { return r(rfunc).w(wfunc); }
        public address_map_entry rw(read16sm_delegate rfunc, write16sm_delegate wfunc) { return r(rfunc).w(wfunc); }

        //template <typename T, typename Ret, typename... Params>
        //address_map_entry &m(Ret (T::*map)(Params...), const char *map_name)
        //{ return m(&m_devbase, make_delegate(map, map_name, make_pointer<T>(m_devbase))); }


        // device tag -> delegate converter
        //template <typename T, typename Ret, typename... Params>
        //address_map_entry &r(const char *tag, Ret (T::*read)(Params...), const char *read_name)
        //{ return r(emu::detail::make_delegate(m_devbase, tag, read, read_name)); }
        public address_map_entry r(string tag, read8_delegate func) { return r(func); }
        public address_map_entry r(string tag, read16_delegate func) { return r(func); }
        public address_map_entry r(string tag, read8s_delegate func) { return r(func); }
        public address_map_entry r(string tag, read16s_delegate func) { return r(func); }
        public address_map_entry r(string tag, read8mo_delegate func) { return r(func); }
        public address_map_entry r(string tag, read8smo_delegate func) { return r(func); }

        //template <typename T, typename Ret, typename... Params>
        //address_map_entry &w(const char *tag, Ret (T::*write)(Params...), const char *write_name)
        //{ return w(emu::detail::make_delegate(m_devbase, tag, write, write_name)); }
        public address_map_entry w(string tag, write8_delegate func) { return w(func); }
        public address_map_entry w(string tag, write16_delegate func) { return w(func); }
        public address_map_entry w(string tag, write8s_delegate func) { return w(func); }
        public address_map_entry w(string tag, write16s_delegate func) { return w(func); }
        public address_map_entry w(string tag, write8sm_delegate func) { return w(func); }
        public address_map_entry w(string tag, write8smo_delegate func) { return w(func); }
        public address_map_entry w(string tag, write16smo_delegate func) { return w(func); }

        //template <typename T, typename RetR, typename... ParamsR, typename U, typename RetW, typename... ParamsW>
        //address_map_entry &rw(const char *tag, RetR (T::*read)(ParamsR...), const char *read_name, RetW (U::*write)(ParamsW...), const char *write_name)
        //{ return r(emu::detail::make_delegate(m_devbase, tag, read, read_name)).w(emu::detail::make_delegate(m_devbase, tag, write, write_name)); }
        public address_map_entry rw(string tag, read8_delegate rfunc, write8_delegate wfunc) { return r(rfunc).w(wfunc); }
        public address_map_entry rw(string tag, read8m_delegate rfunc, write8sm_delegate wfunc) { return r(rfunc).w(wfunc); }
        public address_map_entry rw(string tag, read8sm_delegate rfunc, write8sm_delegate wfunc) { return r(rfunc).w(wfunc); }
        public address_map_entry rw(string tag, read8smo_delegate rfunc, write8smo_delegate wfunc) { return r(rfunc).w(wfunc); }

        //template <typename T, typename Ret, typename... Params>
        //address_map_entry &m(const char *tag, Ret (T::*map)(Params...), const char *map_name)
        //{ return m(tag, make_delegate(map, map_name, static_cast<T *>(nullptr))); }


        // device reference -> delegate converter
        //template <typename T, typename U, typename Ret, typename... Params>
        //address_map_entry &r(T &obj, Ret (U::*read)(Params...), const char *read_name)
        //{ return r(emu::detail::make_delegate(*make_pointer<U>(obj), read, read_name)); }

        //template <typename T, typename U, typename Ret, typename... Params>
        //address_map_entry &w(T &obj, Ret (U::*write)(Params...), const char *write_name)
        //{ return w(emu::detail::make_delegate(*make_pointer<U>(obj), write, write_name)); }

        //template <typename T, typename U, typename RetR, typename... ParamsR, typename V, typename RetW, typename... ParamsW>
        //address_map_entry &rw(T &obj, RetR (U::*read)(ParamsR...), const char *read_name, RetW (V::*write)(ParamsW...), const char *write_name)
        //{ return r(emu::detail::make_delegate(*make_pointer<U>(obj), read, read_name)).w(emu::detail::make_delegate(make_pointer<V>(obj), write, write_name)); }

        //template <typename T, typename U, typename Ret, typename... Params>
        //address_map_entry &m(T &obj, Ret (U::*map)(Params...), const char *map_name)
        //{ return m(make_pointer<device_t>(obj), make_delegate(map, map_name, make_pointer<U>(obj))); }


        // device finder -> delegate converter
        //template <typename T, bool Reqd, typename U, typename Ret, typename... Params>
        public address_map_entry r<T, Reqd>(device_finder<T, Reqd> finder, read8sm_delegate read)  //address_map_entry &r(device_finder<T, Reqd> &finder, Ret (U::*read)(Params...), const char *read_name) {
            where Reqd : bool_constant, new()
        {
            device_t device = find_device(finder);
            return r(read);
        }

        public address_map_entry r<T, Reqd>(device_finder<T, Reqd> finder, read8smo_delegate read)  //address_map_entry &r(device_finder<T, Reqd> &finder, Ret (U::*read)(Params...), const char *read_name) {
            where Reqd : bool_constant, new()
        {
            device_t device = find_device(finder);
            return r(read);
        }

        public address_map_entry r<T, Reqd>(device_finder<T, Reqd> finder, read16smo_delegate read)  //address_map_entry &r(device_finder<T, Reqd> &finder, Ret (U::*read)(Params...), const char *read_name) {
            where Reqd : bool_constant, new()
        {
            device_t device = find_device(finder);
            return r(read);
        }

        //template <typename T, bool Reqd, typename U, typename Ret, typename... Params>
        //address_map_entry &r(const device_finder<T, Reqd> &finder, Ret (U::*read)(Params...), const char *read_name) {
        //    device_t &device(find_device(finder));
        //    return r(emu::detail::make_delegate(device, DEVICE_SELF, read, read_name));
        //}


        //template <typename T, bool Reqd, typename U, typename Ret, typename... Params>
        public address_map_entry w<T, Reqd>(device_finder<T, Reqd> finder, write16s_delegate write)  //address_map_entry &w(device_finder<T, Reqd> &finder, Ret (U::*write)(Params...), const char *write_name) {
            where Reqd : bool_constant, new()
        {
            device_t device = find_device(finder);
            return w(write);  //return w(emu::detail::make_delegate(device, DEVICE_SELF, write, write_name));
        }

        //template <typename T, bool Reqd, typename U, typename Ret, typename... Params>
        public address_map_entry w<T, Reqd>(device_finder<T, Reqd> finder, write8sm_delegate write)  //address_map_entry &w(device_finder<T, Reqd> &finder, Ret (U::*write)(Params...), const char *write_name) {
            where Reqd : bool_constant, new()
        {
            device_t device = find_device(finder);
            return w(write);  //return w(emu::detail::make_delegate(device, DEVICE_SELF, write, write_name));
        }

        public address_map_entry w<T, Reqd>(device_finder<T, Reqd> finder, write8smo_delegate write)  //address_map_entry &w(device_finder<T, Reqd> &finder, Ret (U::*write)(Params...), const char *write_name) {
            where Reqd : bool_constant, new()
        {
            device_t device = find_device(finder);
            return w(write);  //return w(emu::detail::make_delegate(device, DEVICE_SELF, write, write_name));
        }

        //template <typename T, bool Reqd, typename U, typename Ret, typename... Params>
        //address_map_entry &w(const device_finder<T, Reqd> &finder, Ret (U::*write)(Params...), const char *write_name) {
        //    device_t &device(find_device(finder));
        //    return w(emu::detail::make_delegate(device, DEVICE_SELF, write, write_name));
        //}


        //template <typename T, bool Reqd, typename U, typename RetR, typename... ParamsR, typename V, typename RetW, typename... ParamsW>
        public address_map_entry rw<T, Reqd>(device_finder<T, Reqd> finder, read8sm_delegate read, write8sm_delegate write)  //address_map_entry &rw(device_finder<T, Reqd> &finder, RetR (U::*read)(ParamsR...), const char *read_name, RetW (V::*write)(ParamsW...), const char *write_name) {
            where Reqd : bool_constant, new()
        { 
            device_t device = find_device(finder);
            return r(read).w(write);  //return r(emu::detail::make_delegate(device, DEVICE_SELF, read, read_name)).w(emu::detail::make_delegate(device, DEVICE_SELF, write, write_name));
        }

        //template <typename T, bool Reqd, typename U, typename RetR, typename... ParamsR, typename V, typename RetW, typename... ParamsW>
        public address_map_entry rw<T, Reqd>(device_finder<T, Reqd> finder, read8m_delegate read, write8sm_delegate write)  //address_map_entry &rw(device_finder<T, Reqd> &finder, RetR (U::*read)(ParamsR...), const char *read_name, RetW (V::*write)(ParamsW...), const char *write_name) {
            where Reqd : bool_constant, new()
        { 
            device_t device = find_device(finder);
            return r(read).w(write);  //return r(emu::detail::make_delegate(device, DEVICE_SELF, read, read_name)).w(emu::detail::make_delegate(device, DEVICE_SELF, write, write_name));
        }

        //template <typename T, bool Reqd, typename U, typename RetR, typename... ParamsR, typename V, typename RetW, typename... ParamsW>
        //address_map_entry &rw(const device_finder<T, Reqd> &finder, RetR (U::*read)(ParamsR...), const char *read_name, RetW (V::*write)(ParamsW...), const char *write_name) {
        //    device_t &device(find_device(finder));
        //    return r(emu::detail::make_delegate(device, DEVICE_SELF, read, read_name))
        //            .w(emu::detail::make_delegate(device, DEVICE_SELF, write, write_name));
        //}


        //template <typename T, bool Reqd, typename U, typename Ret, typename... Params>
        public address_map_entry m<T, Reqd>(device_finder<T, Reqd> finder, address_map_constructor map)  //address_map_entry &m(device_finder<T, Reqd> &finder, Ret (U::*map)(Params...), const char *map_name) {
            where Reqd : bool_constant, new()
        {
            device_t device = find_device(finder);
            return m(device, map);
        }

        //template <typename T, bool Reqd, typename U, typename Ret, typename... Params>
        //address_map_entry &m(const device_finder<T, Reqd> &finder, Ret (U::*map)(Params...), const char *map_name) {
        //    device_t &device(find_device(finder));
        //    return m(&device, make_delegate(map, map_name, make_pointer<U>(device)));
        //}


        // lambda -> delegate converter
        //template <typename T> address_map_entry &lr8(T &&read, const char *name)
        //{ return r(emu::detail::make_lr8_delegate(m_devbase, std::forward<T>(read), name)); }

        //template <typename T> address_map_entry &lr16(T &&read, const char *name)
        //{ return r(emu::detail::make_lr16_delegate(m_devbase, std::forward<T>(read), name)); }

        //template <typename T> address_map_entry &lr32(T &&read, const char *name)
        //{ return r(emu::detail::make_lr32_delegate(m_devbase, std::forward<T>(read), name)); }

        //template <typename T> address_map_entry &lr64(T &&read, const char *name)
        //{ return r(emu::detail::make_lr64_delegate(m_devbase, std::forward<T>(read), name)); }

        //template <typename T> address_map_entry &lw8(T &&write, const char *name)
        //{ return w(emu::detail::make_lw8_delegate(m_devbase, std::forward<T>(write), name)); }

        //template <typename T> address_map_entry &lw16(T &&write, const char *name)
        //{ return w(emu::detail::make_lw16_delegate(m_devbase, std::forward<T>(write), name)); }

        //template <typename T> address_map_entry &lw32(T &&write, const char *name)
        //{ return w(emu::detail::make_lw32_delegate(m_devbase, std::forward<T>(write), name)); }

        //template <typename T> address_map_entry &lw64(T &&write, const char *name)
        //{ return w(emu::detail::make_lw64_delegate(m_devbase, std::forward<T>(write), name)); }

        //template <typename T, typename U> address_map_entry &lrw8(T &&read, const char *read_name, U &&write, const char *write_name)
        //{ return r(emu::detail::make_lr8_delegate(m_devbase, std::forward<T>(read), read_name)).w(emu::detail::make_lw8_delegate(m_devbase, std::forward<U>(write), write_name)); }
        public address_map_entry lrw8(read8_delegate read, string read_name, write8_delegate write, string write_name)
        { return r(read).w(write); }
        public address_map_entry lrw8(read8smo_delegate read, string read_name, write8smo_delegate write, string write_name)
        { return r(read).w(write); }

        //template <typename T, typename U> address_map_entry &lrw16(T &&read, const char *read_name, U &&write, const char *write_name)
        //{ return r(emu::detail::make_lr16_delegate(m_devbase, std::forward<T>(read), read_name)).w(emu::detail::make_lw16_delegate(m_devbase, std::forward<U>(write), write_name)); }

        //template <typename T, typename U> address_map_entry &lrw32(T &&read, const char *read_name, U &&write, const char *write_name)
        //{ return r(emu::detail::make_lr32_delegate(m_devbase, std::forward<T>(read), read_name)).w(emu::detail::make_lw32_delegate(m_devbase, std::forward<U>(write), write_name)); }

        //template <typename T, typename U> address_map_entry &lrw64(T &&read, const char *read_name, U &&write, const char *write_name)
        //{ return r(emu::detail::make_lr64_delegate(m_devbase, std::forward<T>(read), read_name)).w(emu::detail::make_lw64_delegate(m_devbase, std::forward<U>(write), write_name)); }


        // handler setters for 8-bit delegates
        //-------------------------------------------------
        //  r/w/rw - handler setters for
        //  8-bit read/write delegates
        //-------------------------------------------------

        public address_map_entry r(read8_delegate func)
        {
            assert(func != null);
            m_read.m_type = map_handler_type.AMH_DEVICE_DELEGATE;
            m_read.m_bits = 8;
            m_read.m_name = func.GetMethodInfo().Name;
            m_rproto8 = func;
            return this;
        }


        public address_map_entry w(write8_delegate func)
        {
            assert(func != null);
            m_write.m_type = map_handler_type.AMH_DEVICE_DELEGATE;
            m_write.m_bits = 8;
            m_write.m_name = func.GetMethodInfo().Name;
            m_wproto8 = func;
            return this;
        }


        public address_map_entry r(read8m_delegate func)
        {
            assert(func != null);
            m_read.m_type = map_handler_type.AMH_DEVICE_DELEGATE_M;
            m_read.m_bits = 8;
            m_read.m_name = func.GetMethodInfo().Name;
            m_rproto8m = func;
            return this;
        }

        public address_map_entry w(write8m_delegate func)
        {
            assert(func != null);
            m_write.m_type = map_handler_type.AMH_DEVICE_DELEGATE_M;
            m_write.m_bits = 8;
            m_write.m_name = func.GetMethodInfo().Name;
            m_wproto8m = func;
            return this;
        }


        public address_map_entry r(read8s_delegate func)
        {
            assert(func != null);
            m_read.m_type = map_handler_type.AMH_DEVICE_DELEGATE_S;
            m_read.m_bits = 8;
            m_read.m_name = func.GetMethodInfo().Name;
            m_rproto8s = func;
            return this;
        }


        public address_map_entry w(write8s_delegate func)
        {
            assert(func != null);
            m_write.m_type = map_handler_type.AMH_DEVICE_DELEGATE_S;
            m_write.m_bits = 8;
            m_write.m_name = func.GetMethodInfo().Name;
            m_wproto8s = func;
            return this;
        }


        public address_map_entry r(read8sm_delegate func)
        {
            assert(func != null);
            m_read.m_type = map_handler_type.AMH_DEVICE_DELEGATE_SM;
            m_read.m_bits = 8;
            m_read.m_name = func.GetMethodInfo().Name;
            m_rproto8sm = func;
            return this;
        }


        public address_map_entry w(write8sm_delegate func)
        {
            assert(func != null);
            m_write.m_type = map_handler_type.AMH_DEVICE_DELEGATE_SM;
            m_write.m_bits = 8;
            m_write.m_name = func.GetMethodInfo().Name;
            m_wproto8sm = func;
            return this;
        }


        public address_map_entry r(read8mo_delegate func)
        {
            assert(func != null);
            m_read.m_type = map_handler_type.AMH_DEVICE_DELEGATE_MO;
            m_read.m_bits = 8;
            m_read.m_name = func.GetMethodInfo().Name;
            m_rproto8mo = func;
            return this;
        }


        public address_map_entry w(write8mo_delegate func)
        {
            assert(func != null);
            m_write.m_type = map_handler_type.AMH_DEVICE_DELEGATE_MO;
            m_write.m_bits = 8;
            m_write.m_name = func.GetMethodInfo().Name;
            m_wproto8mo = func;
            return this;
        }


        public address_map_entry r(read8smo_delegate func)
        {
            assert(func != null);
            m_read.m_type = map_handler_type.AMH_DEVICE_DELEGATE_SMO;
            m_read.m_bits = 8;
            m_read.m_name = func.GetMethodInfo().Name;
            m_rproto8smo = func;
            return this;
        }


        public address_map_entry w(write8smo_delegate func)
        {
            assert(func != null);
            m_write.m_type = map_handler_type.AMH_DEVICE_DELEGATE_SMO;
            m_write.m_bits = 8;
            m_write.m_name = func.GetMethodInfo().Name;
            m_wproto8smo = func;
            return this;
        }


        // handler setters for 16-bit delegates
        //-------------------------------------------------
        //  r/w/rw - handler setters for
        //  16-bit read/write delegates
        //-------------------------------------------------

        public address_map_entry r(read16_delegate func)
        {
            assert(func != null);
            m_read.m_type = map_handler_type.AMH_DEVICE_DELEGATE;
            m_read.m_bits = 16;
            m_read.m_name = func.GetMethodInfo().Name;
            m_rproto16 = func;
            return this;
        }


        public address_map_entry w(write16_delegate func)
        {
            assert(func != null);
            m_write.m_type = map_handler_type.AMH_DEVICE_DELEGATE;
            m_write.m_bits = 16;
            m_write.m_name = func.GetMethodInfo().Name;
            m_wproto16 = func;
            return this;
        }


        public address_map_entry r(read16m_delegate func)
        {
            assert(func != null);
            m_read.m_type = map_handler_type.AMH_DEVICE_DELEGATE_M;
            m_read.m_bits = 16;
            m_read.m_name = func.GetMethodInfo().Name;
            m_rproto16m = func;
            return this;
        }


        public address_map_entry w(write16m_delegate func)
        {
            assert(func != null);
            m_write.m_type = map_handler_type.AMH_DEVICE_DELEGATE_M;
            m_write.m_bits = 16;
            m_write.m_name = func.GetMethodInfo().Name;
            m_wproto16m = func;
            return this;
        }


        public address_map_entry r(read16s_delegate func)
        {
            assert(func != null);
            m_read.m_type = map_handler_type.AMH_DEVICE_DELEGATE_S;
            m_read.m_bits = 16;
            m_read.m_name = func.GetMethodInfo().Name;
            m_rproto16s = func;
            return this;
        }


        public address_map_entry w(write16s_delegate func)
        {
            assert(func != null);
            m_write.m_type = map_handler_type.AMH_DEVICE_DELEGATE_S;
            m_write.m_bits = 16;
            m_write.m_name = func.GetMethodInfo().Name;
            m_wproto16s = func;
            return this;
        }


        public address_map_entry r(read16sm_delegate func)
        {
            assert(func != null);
            m_read.m_type = map_handler_type.AMH_DEVICE_DELEGATE_SM;
            m_read.m_bits = 16;
            m_read.m_name = func.GetMethodInfo().Name;
            m_rproto16sm = func;
            return this;
        }


        public address_map_entry w(write16sm_delegate func)
        {
            assert(func != null);
            m_write.m_type = map_handler_type.AMH_DEVICE_DELEGATE_SM;
            m_write.m_bits = 16;
            m_write.m_name = func.GetMethodInfo().Name;
            m_wproto16sm = func;
            return this;
        }


        public address_map_entry r(read16mo_delegate func)
        {
            assert(func != null);
            m_read.m_type = map_handler_type.AMH_DEVICE_DELEGATE_MO;
            m_read.m_bits = 16;
            m_read.m_name = func.GetMethodInfo().Name;
            m_rproto16mo = func;
            return this;
        }


        public address_map_entry w(write16mo_delegate func)
        {
            assert(func != null);
            m_write.m_type = map_handler_type.AMH_DEVICE_DELEGATE_MO;
            m_write.m_bits = 16;
            m_write.m_name = func.GetMethodInfo().Name;
            m_wproto16mo = func;
            return this;
        }


        public address_map_entry r(read16smo_delegate func)
        {
            assert(func != null);
            m_read.m_type = map_handler_type.AMH_DEVICE_DELEGATE_SMO;
            m_read.m_bits = 16;
            m_read.m_name = func.GetMethodInfo().Name;
            m_rproto16smo = func;
            return this;
        }


        public address_map_entry w(write16smo_delegate func)
        {
            assert(func != null);
            m_write.m_type = map_handler_type.AMH_DEVICE_DELEGATE_SMO;
            m_write.m_bits = 16;
            m_write.m_name = func.GetMethodInfo().Name;
            m_wproto16smo = func;
            return this;
        }


        // handler setters for 32-bit delegates
        //-------------------------------------------------
        //  r/w/rw - handler setters for
        //  32-bit read/write delegates
        //-------------------------------------------------

        public address_map_entry r(read32_delegate func)
        {
            assert(func != null);
            m_read.m_type = map_handler_type.AMH_DEVICE_DELEGATE;
            m_read.m_bits = 32;
            m_read.m_name = func.GetMethodInfo().Name;
            m_rproto32 = func;
            return this;
        }


        public address_map_entry w(write32_delegate func)
        {
            assert(func != null);
            m_write.m_type = map_handler_type.AMH_DEVICE_DELEGATE;
            m_write.m_bits = 32;
            m_write.m_name = func.GetMethodInfo().Name;
            m_wproto32 = func;
            return this;
        }


        //address_map_entry &r(read32m_delegate func);
        //address_map_entry &w(write32m_delegate func);
        //address_map_entry &r(read32s_delegate func);
        //address_map_entry &w(write32s_delegate func);
        //address_map_entry &r(read32sm_delegate func);
        //address_map_entry &w(write32sm_delegate func);
        //address_map_entry &r(read32mo_delegate func);
        //address_map_entry &w(write32mo_delegate func);
        //address_map_entry &r(read32smo_delegate func);
        //address_map_entry &w(write32smo_delegate func);


        // handler setters for 64-bit delegates
        //-------------------------------------------------
        //  r/w/rw - handler setters for
        //  64-bit read/write delegates
        //-------------------------------------------------

        public address_map_entry r(read64_delegate func)
        {
            assert(func != null);
            m_read.m_type = map_handler_type.AMH_DEVICE_DELEGATE;
            m_read.m_bits = 64;
            m_read.m_name = func.GetMethodInfo().Name;
            m_rproto64 = func;
            return this;
        }


        public address_map_entry w(write64_delegate func)
        {
            assert(func != null);
            m_write.m_type = map_handler_type.AMH_DEVICE_DELEGATE;
            m_write.m_bits = 64;
            m_write.m_name = func.GetMethodInfo().Name;
            m_wproto64 = func;
            return this;
        }


        //address_map_entry &r(read64m_delegate func);
        //address_map_entry &w(write64m_delegate func);
        //address_map_entry &r(read64s_delegate func);
        //address_map_entry &w(write64s_delegate func);
        //address_map_entry &r(read64sm_delegate func);
        //address_map_entry &w(write64sm_delegate func);
        //address_map_entry &r(read64mo_delegate func);
        //address_map_entry &w(write64mo_delegate func);
        //address_map_entry &r(read64smo_delegate func);
        //address_map_entry &w(write64smo_delegate func);



        // helper functions
        //-------------------------------------------------
        //  unitmask_is_appropriate - verify that the
        //  provided unitmask is valid and expected
        //-------------------------------------------------
        bool unitmask_is_appropriate(u8 width, u64 unitmask, string str)
        {
#if false
            // if no mask, this must match the default width of the map
            if (unitmask == 0)
            {
                if (m_map.databits() != width)
                {
                    global.osd_printf_error("Handler {0} is a {1}-bit handler but was specified in a {2}-bit address map\n", str, width, m_map.databits());
                    return false;
                }
                return true;
            }

            // if we have a mask, we must be smaller than the default width of the map
            if (m_map.databits() < width)
            {
                global.osd_printf_error("Handler {0} is a {1}-bit handler and is too wide to be used in a {2}-bit address map\n", str, width, m_map.databits());
                return false;
            }

            // if map is narrower than 64 bits, check the mask width as well
            if (m_map.databits() < 64 && (unitmask >> m_map.databits()) != 0)
            {
                global.osd_printf_error("Handler {0} specified a mask of {1}, too wide to be used in a {2}-bit address map\n", str, unitmask, m_map.databits());  // %016X
                return false;
            }

            // the mask must represent whole units of width
            UInt32 basemask = (width == 8) ? 0xff : (width == 16) ? 0xffff : 0xffffffff;
            UInt64 singlemask = basemask;
            int count = 0;
            while (singlemask != 0)
            {
                if ((unitmask & singlemask) == singlemask)
                {
                    count++;
                }
                else if ((unitmask & singlemask) != 0)
                {
                    global.osd_printf_error("Handler {0} specified a mask of {1}; needs to be in even chunks of {2}\n", str, unitmask, basemask);  // %08X%08X
                    return false;
                }

                singlemask <<= width;
            }

#if false
            // the mask must be symmetrical
            u64 unitmask_bh = unitmask >> 8 & 0x00ff00ff00ff00ffU;
            u64 unitmask_bl = unitmask & 0x00ff00ff00ff00ffU;
            u64 unitmask_wh = unitmask >> 16 & 0x0000ffff0000ffffU;
            u64 unitmask_wl = unitmask & 0x0000ffff0000ffffU;
            u64 unitmask_dh = unitmask >> 32 & 0x00000000ffffffffU;
            u64 unitmask_dl = unitmask & 0x00000000ffffffffU;
            if ((unitmask_bh != 0 && unitmask_bl != 0 && unitmask_bh != unitmask_bl)
                || (unitmask_wh != 0 && unitmask_wl != 0 && unitmask_wh != unitmask_wl)
                || (unitmask_dh != 0 && unitmask_dl != 0 && unitmask_dh != unitmask_dl))
            {
                osd_printf_error("Handler %s specified an asymmetrical mask of %016X\n", string, unitmask);
                return false;
            }
#endif
#endif

            return true;
        }
    }


    // ======================> address_map
    // address_map holds global map parameters plus the head of the list of entries
    public class address_map : global_object
    {
        // public data
        int m_spacenum;         // space number of the map
        device_t m_device;       // associated device
        //memory_view *                   m_view;         // view, when in one
        address_space_config m_config;       // space configuration
        public u8 m_unmapval;         // unmapped memory value
        public offs_t m_globalmask;       // global mask
        public simple_list<address_map_entry> m_entrylist = new simple_list<address_map_entry>(); // list of entries


        // construction/destruction

        //-------------------------------------------------
        //  address_map - constructor
        //-------------------------------------------------
        public address_map(device_t device, int spacenum)
        {
            m_spacenum = spacenum;
            m_device = device;
            m_unmapval = 0;
            m_globalmask = 0;


            // get our memory interface
            device_memory_interface memintf;
            if (!m_device.interface_(out memintf))
                throw new emu_fatalerror("No memory interface defined for device '{0}'\n", m_device.tag());

            // and then the configuration for the current address space
            address_space_config spaceconfig = memintf.space_config(spacenum);
            if (spaceconfig == null)  //if (!device.interface(memintf))  // possible bug here
                throw new emu_fatalerror("No memory address space configuration found for device '{0}', space {1}\n", m_device.tag(), spacenum);

            // append the map provided by the owner
            if (memintf.get_addrmap(spacenum) != null)
            {
                m_device = device.owner();
                memintf.get_addrmap(spacenum)(this, m_device);
                m_device = device;
            }

            // construct the internal device map (last so it takes priority)
            if (spaceconfig.m_internal_map != null)
                spaceconfig.m_internal_map(this, m_device);
        }


        //address_map(memory_view &view);


        //-------------------------------------------------
        //  address_map - constructor in the submap case
        //-------------------------------------------------
        public address_map(device_t device, address_map_entry entry)
        {
            m_spacenum = AS_PROGRAM;
            m_device = device;
            m_unmapval = 0;
            m_globalmask = 0;


            // Retrieve the submap
            //entry.m_submap_delegate.late_bind(device);
            entry.m_submap_delegate(this, m_device);
        }

        //----------------------------------------------------------
        //  address_map - constructor dynamic device mapping case
        //----------------------------------------------------------
        public address_map(address_space space, offs_t start, offs_t end, u64 unitmask, int cswidth, device_t device, address_map_constructor submap_delegate)
        {
            m_spacenum = space.spacenum();
            m_device = device;
            m_unmapval = (u8)space.unmap();
            m_globalmask = space.addrmask();


            op(start, end).m(DEVICE_SELF, submap_delegate).umask64(unitmask).cswidth(cswidth);
        }


        // setters

        //-------------------------------------------------
        //  append - append an entry to the end of the
        //  list
        //-------------------------------------------------
        public void global_mask(offs_t mask)
        {
            m_globalmask = mask;
        }


        //void unmap_value_low() { m_unmapval = 0; }
        public void unmap_value_high() { m_unmapval = byte.MaxValue; }  // ~0; }


        // add a new entry of the given type
        public address_map_entry op(offs_t start, offs_t end)  //address_map_entry &operator()(offs_t start, offs_t end);
        {
            address_map_entry ptr = new address_map_entry(m_device, this, start, end);
            m_entrylist.append(ptr);
            return ptr;
        }


        //-------------------------------------------------
        //  import_submaps - propagate in the device submaps
        //-------------------------------------------------
        public void import_submaps(running_machine machine, device_t owner, int data_width, endianness_t endian, int addr_shift)
        {
            address_map_entry prev = null;
            address_map_entry entry = m_entrylist.first();
            u64 base_unitmask = (~(u64)0) >> (64 - data_width);

            while (entry != null)
            {
                if (entry.m_read.m_type == map_handler_type.AMH_DEVICE_SUBMAP)
                {
                    device_t mapdevice = entry.m_submap_device;
                    if (mapdevice == null)
                    {
                        mapdevice = owner.subdevice(entry.m_read.m_tag);
                        if (mapdevice == null)
                            throw new emu_fatalerror("Attempted to submap a non-existent device '{0}' in space {1} of device '{2}'\n", owner.subtag(entry.m_read.m_tag).c_str(), m_spacenum, m_device.basetag());
                    }

                    // Grab the submap
                    address_map submap = new address_map(mapdevice, entry);

                    // Recursively import if needed
                    submap.import_submaps(machine, mapdevice, data_width, endian, addr_shift);

                    offs_t max_end = entry.m_addrend - entry.m_addrstart;

                    if (entry.m_mask == 0 || (entry.m_mask & base_unitmask) == base_unitmask)
                    {
                        // Easy case, no unitmask at mapping level - Merge in all the map contents in order
                        while (submap.m_entrylist.count() != 0)
                        {
                            address_map_entry subentry = submap.m_entrylist.detach_head();

                            if (addr_shift > 0)
                            {
                                subentry.m_addrstart <<= addr_shift;
                                subentry.m_addrend = ((subentry.m_addrend + 1) << addr_shift) - 1;
                                subentry.m_addrmirror <<= addr_shift;
                                subentry.m_addrmask <<= addr_shift;
                                subentry.m_addrselect <<= addr_shift;
                            }
                            else if (addr_shift < 0)
                            {
                                subentry.m_addrstart >>= -addr_shift;
                                subentry.m_addrend >>= -addr_shift;
                                subentry.m_addrmirror >>= -addr_shift;
                                subentry.m_addrmask >>= -addr_shift;
                                subentry.m_addrselect >>= -addr_shift;
                            }

                            if (subentry.m_addrend > max_end)
                                subentry.m_addrend = max_end;

                            subentry.m_addrstart += entry.m_addrstart;
                            subentry.m_addrend += entry.m_addrstart;
                            subentry.m_addrmirror |= entry.m_addrmirror;
                            subentry.m_addrmask |= entry.m_addrmask;
                            subentry.m_addrselect |= entry.m_addrselect;

                            if (subentry.m_addrstart > entry.m_addrend)
                            {
                                subentry = null;  // delete subentry;
                                continue;
                            }

                            // Insert the entry in the map
                            m_entrylist.insert_after(subentry, prev);
                            prev = subentry;
                        }
                    }
                    else
                    {
                        // There is a unitmask, calculate its ratio
                        int ratio = 0;
                        for (int i = 0; i != data_width; i++)
                        {
                            if (((entry.m_mask >> i) & 1) != 0)
                                ratio++;
                        }
                        ratio = data_width / ratio;

                        if (addr_shift > 0)
                            ratio <<= addr_shift;
                        else if (addr_shift < 0)
                            max_end = ((max_end + 1) << -addr_shift) - 1;

                        max_end = (max_end + 1) / (offs_t)ratio - 1;

                        // Then merge the contents taking the ratio into account
                        while (submap.m_entrylist.count() != 0)
                        {
                            address_map_entry subentry = submap.m_entrylist.detach_head();

                            if ((subentry.m_mask != 0) && (subentry.m_mask != 0xffffffffffffffffU))
                            {
                                // Check if the mask can actually fit
                                int subentry_ratio = 0;
                                for (int i = 0; i != data_width; i++)
                                {
                                    if (((subentry.m_mask >> i) & 1) != 0)
                                        subentry_ratio ++;
                                }
                                subentry_ratio = data_width / subentry_ratio;
                                if (ratio * subentry_ratio > data_width / 8)
                                    fatalerror("import_submap: In range {0}-{1} mask {2} mirror {3} select {4} of device {5}, the import unitmask of {6}{7} combined with an entry unitmask of {8}{9} does not fit in {10} bits.\n", subentry.m_addrstart, subentry.m_addrend, subentry.m_addrmask, subentry.m_addrmirror, subentry.m_addrselect, entry.m_read.m_tag, data_width / 4, entry.m_mask, data_width / 4, subentry.m_mask, data_width);

                                // Regenerate the unitmask
                                u64 newmask = 0;
                                int bit_in_subentry = 0;
                                for (int i = 0; i != data_width; i++)
                                {
                                    if (((entry.m_mask >> i) & 1) != 0)
                                    {
                                        if (((subentry.m_mask >> bit_in_subentry) & 1) != 0)
                                            newmask |= (u64)1 << i;

                                        bit_in_subentry ++;
                                    }
                                }
                                subentry.m_mask = newmask;
                            }
                            else
                            {
                                subentry.m_mask = entry.m_mask;
                            }

                            subentry.m_cswidth = std.max(subentry.m_cswidth, entry.m_cswidth);

                            if (subentry.m_addrend > max_end)
                                subentry.m_addrend = max_end;

                            if (addr_shift < 0)
                            {
                                subentry.m_addrstart = ((subentry.m_addrstart * (offs_t)ratio) >> -addr_shift) + entry.m_addrstart;
                                subentry.m_addrend = (((subentry.m_addrend + 1) * (offs_t)ratio - 1) >> -addr_shift) + entry.m_addrstart;
                                subentry.m_addrmirror = ((subentry.m_addrmirror / (offs_t)ratio) << -addr_shift) | entry.m_addrmirror;
                                subentry.m_addrmask = ((subentry.m_addrmask / (offs_t)ratio) << -addr_shift) | entry.m_addrmask;
                                subentry.m_addrselect = ((subentry.m_addrselect / (offs_t)ratio) << -addr_shift) | entry.m_addrselect;
                            }
                            else
                            {
                                subentry.m_addrstart = subentry.m_addrstart * (offs_t)ratio + entry.m_addrstart;
                                subentry.m_addrend = (subentry.m_addrend + 1) * (offs_t)ratio - 1 + entry.m_addrstart;
                                subentry.m_addrmirror = (subentry.m_addrmirror / (offs_t)ratio) | entry.m_addrmirror;
                                subentry.m_addrmask = (subentry.m_addrmask / (offs_t)ratio) | entry.m_addrmask;
                                subentry.m_addrselect = (subentry.m_addrselect / (offs_t)ratio) | entry.m_addrselect;
                            }

                            if (subentry.m_addrstart > entry.m_addrend)
                            {
                                subentry = null;  //delete subentry;
                                continue;
                            }

                            // Insert the entry in the map
                            m_entrylist.insert_after(subentry, prev);
                            prev = subentry;
                        }
                    }

                    address_map_entry to_delete = entry;
                    entry = entry.next();
                    m_entrylist.remove(to_delete);
                }
                else
                {
                    prev = entry;
                    entry = entry.next();
                }
            }
        }


        //void map_validity_check(validity_checker &valid, int spacenum) const;


        public address_space_config get_config()
        {
            return m_config;
        }
    }
}
