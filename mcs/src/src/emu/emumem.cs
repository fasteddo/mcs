// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Reflection;

using memory_interface_enumerator = mame.device_interface_enumerator<mame.device_memory_interface>;  //typedef device_interface_enumerator<device_memory_interface> memory_interface_enumerator;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using offs_t = System.UInt32;  //using offs_t = u32;
using PointerU8 = mame.Pointer<System.Byte>;
using s8  = System.SByte;
using s32 = System.Int32;
using size_t = System.UInt32;
using u8  = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    // offsets and addresses are 32-bit (for now...)
    //using offs_t = u32;


    // address map constructors are delegates that build up an address_map
    //using address_map_constructor = named_delegate<void (address_map &)>;
    public delegate void address_map_constructor(address_map map, device_t owner);


    // ======================> read_delegate
    // declare delegates for each width
    //using read8_delegate  = device_delegate<u8  (address_space &, offs_t, u8 )>;
    public delegate u8 read8_delegate(address_space space, offs_t offset, u8 mem_mask);
    public delegate u16 read16_delegate(address_space space, offs_t offset, u16 mem_mask);
    public delegate u32 read32_delegate(address_space space, offs_t offset, u32 mem_mask);
    public delegate u64 read64_delegate(address_space space, offs_t offset, u64 mem_mask);

    //using read8m_delegate  = device_delegate<u8  (address_space &, offs_t)>;
    public delegate u8 read8m_delegate(address_space space, offs_t offset);
    public delegate u16 read16m_delegate(address_space space, offs_t offset);
    public delegate u32 read32m_delegate(address_space space, offs_t offset);
    public delegate u64 read64m_delegate(address_space space, offs_t offset);

    //using read8s_delegate  = device_delegate<u8  (offs_t, u8 )>;
    public delegate u8 read8s_delegate(offs_t offset, u8 mem_mask);
    public delegate u16 read16s_delegate(offs_t offset, u16 mem_mask);
    public delegate u32 read32s_delegate(offs_t offset, u32 mem_mask);
    public delegate u64 read64s_delegate(offs_t offset, u64 mem_mask);

    //using read8sm_delegate  = device_delegate<u8  (offs_t)>;
    public delegate u8 read8sm_delegate(offs_t offset);
    public delegate u16 read16sm_delegate(offs_t offset);
    public delegate u32 read32sm_delegate(offs_t offset);
    public delegate u64 read64sm_delegate(offs_t offset);

    //using read8mo_delegate  = device_delegate<u8  (address_space &)>;
    public delegate u8 read8mo_delegate(address_space space);
    public delegate u16 read16mo_delegate(address_space space);
    public delegate u32 read32mo_delegate(address_space space);
    public delegate u64 read64mo_delegate(address_space space);

    //using read8smo_delegate  = device_delegate<u8  ()>;
    public delegate u8 read8smo_delegate();
    public delegate u16 read16smo_delegate();
    public delegate u32 read32smo_delegate();
    public delegate u64 read64smo_delegate();


    // ======================> write_delegate
    // declare delegates for each width
    //using write8_delegate  = device_delegate<void (address_space &, offs_t, u8,  u8 )>;
    public delegate void write8_delegate(address_space space, offs_t offset, u8 data, u8 mem_mask);
    public delegate void write16_delegate(address_space space, offs_t offset, u16 data, u16 mem_mask);
    public delegate void write32_delegate(address_space space, offs_t offset, u32 data, u32 mem_mask);
    public delegate void write64_delegate(address_space space, offs_t offset, u64 data, u64 mem_mask);

    //using write8m_delegate  = device_delegate<void (address_space &, offs_t, u8 )>;
    public delegate void write8m_delegate(address_space space, offs_t offset, u8 data);
    public delegate void write16m_delegate(address_space space, offs_t offset, u16 data);
    public delegate void write32m_delegate(address_space space, offs_t offset, u32 data);
    public delegate void write64m_delegate(address_space space, offs_t offset, u64 data);

    //using write8s_delegate  = device_delegate<void (offs_t, u8,  u8 )>;
    public delegate void write8s_delegate(offs_t offset, u8 data, u8 mem_mask);
    public delegate void write16s_delegate(offs_t offset, u16 data, u16 mem_mask);
    public delegate void write32s_delegate(offs_t offset, u32 data, u32 mem_mask);
    public delegate void write64s_delegate(offs_t offset, u64 data, u64 mem_mask);

    //using write8sm_delegate  = device_delegate<void (offs_t, u8 )>;
    public delegate void write8sm_delegate(offs_t offset, u8 data);
    public delegate void write16sm_delegate(offs_t offset, u16 data);
    public delegate void write32sm_delegate(offs_t offset, u32 data);
    public delegate void write64sm_delegate(offs_t offset, u64 data);

    //using write8mo_delegate  = device_delegate<void (address_space &, u8 )>;
    public delegate void write8mo_delegate(address_space space, u8 data);
    public delegate void write16mo_delegate(address_space space, u16 data);
    public delegate void write32mo_delegate(address_space space, u32 data);
    public delegate void write64mo_delegate(address_space space, u64 data);

    //using write8smo_delegate  = device_delegate<void (u8 )>;
    public delegate void write8smo_delegate(u8 data);
    public delegate void write16smo_delegate(u16 data);
    public delegate void write32smo_delegate(u32 data);
    public delegate void write64smo_delegate(u64 data);


#if false
    namespace emu.detail {

    // TODO: replace with std::void_t when we move to C++17
    template <typename... T> struct void_wrapper { using type = void; };
    template <typename... T> using void_t = typename void_wrapper<T...>::type;

    template <typename D, typename T, typename Enable = void> struct rw_device_class  { };

    template <typename D, typename T, typename Ret, typename... Params>
    struct rw_device_class<D, Ret (T::*)(Params...), std::enable_if_t<std::is_constructible<D, device_t &, const char *, Ret (T::*)(Params...), const char *>::value> > { using type = T; };
    template <typename D, typename T, typename Ret, typename... Params>
    struct rw_device_class<D, Ret (T::*)(Params...) const, std::enable_if_t<std::is_constructible<D, device_t &, const char *, Ret (T::*)(Params...) const, const char *>::value> > { using type = T; };
    template <typename D, typename T, typename Ret, typename... Params>
    struct rw_device_class<D, Ret (*)(T &, Params...), std::enable_if_t<std::is_constructible<D, device_t &, const char *, Ret (*)(T &, Params...), const char *>::value> > { using type = T; };

    template <typename D, typename T> using rw_device_class_t  = typename rw_device_class<D, T>::type;

    template <typename T, typename Enable = void> struct rw_delegate_type;
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read8_delegate, std::remove_reference_t<T> > > > { using type = read8_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read16_delegate, std::remove_reference_t<T> > > > { using type = read16_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read32_delegate, std::remove_reference_t<T> > > > { using type = read32_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read64_delegate, std::remove_reference_t<T> > > > { using type = read64_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read8m_delegate, std::remove_reference_t<T> > > > { using type = read8m_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read16m_delegate, std::remove_reference_t<T> > > > { using type = read16m_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read32m_delegate, std::remove_reference_t<T> > > > { using type = read32m_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read64m_delegate, std::remove_reference_t<T> > > > { using type = read64m_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read8s_delegate, std::remove_reference_t<T> > > > { using type = read8s_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read16s_delegate, std::remove_reference_t<T> > > > { using type = read16s_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read32s_delegate, std::remove_reference_t<T> > > > { using type = read32s_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read64s_delegate, std::remove_reference_t<T> > > > { using type = read64s_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read8sm_delegate, std::remove_reference_t<T> > > > { using type = read8sm_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read16sm_delegate, std::remove_reference_t<T> > > > { using type = read16sm_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read32sm_delegate, std::remove_reference_t<T> > > > { using type = read32sm_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read64sm_delegate, std::remove_reference_t<T> > > > { using type = read64sm_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read8mo_delegate, std::remove_reference_t<T> > > > { using type = read8mo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read16mo_delegate, std::remove_reference_t<T> > > > { using type = read16mo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read32mo_delegate, std::remove_reference_t<T> > > > { using type = read32mo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read64mo_delegate, std::remove_reference_t<T> > > > { using type = read64mo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read8smo_delegate, std::remove_reference_t<T> > > > { using type = read8smo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read16smo_delegate, std::remove_reference_t<T> > > > { using type = read16smo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read32smo_delegate, std::remove_reference_t<T> > > > { using type = read32smo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read64smo_delegate, std::remove_reference_t<T> > > > { using type = read64smo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write8_delegate, std::remove_reference_t<T> > > > { using type = write8_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write16_delegate, std::remove_reference_t<T> > > > { using type = write16_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write32_delegate, std::remove_reference_t<T> > > > { using type = write32_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write64_delegate, std::remove_reference_t<T> > > > { using type = write64_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write8m_delegate, std::remove_reference_t<T> > > > { using type = write8m_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write16m_delegate, std::remove_reference_t<T> > > > { using type = write16m_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write32m_delegate, std::remove_reference_t<T> > > > { using type = write32m_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write64m_delegate, std::remove_reference_t<T> > > > { using type = write64m_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write8s_delegate, std::remove_reference_t<T> > > > { using type = write8s_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write16s_delegate, std::remove_reference_t<T> > > > { using type = write16s_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write32s_delegate, std::remove_reference_t<T> > > > { using type = write32s_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write64s_delegate, std::remove_reference_t<T> > > > { using type = write64s_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write8sm_delegate, std::remove_reference_t<T> > > > { using type = write8sm_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write16sm_delegate, std::remove_reference_t<T> > > > { using type = write16sm_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write32sm_delegate, std::remove_reference_t<T> > > > { using type = write32sm_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write64sm_delegate, std::remove_reference_t<T> > > > { using type = write64sm_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write8mo_delegate, std::remove_reference_t<T> > > > { using type = write8mo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write16mo_delegate, std::remove_reference_t<T> > > > { using type = write16mo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write32mo_delegate, std::remove_reference_t<T> > > > { using type = write32mo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write64mo_delegate, std::remove_reference_t<T> > > > { using type = write64mo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write8smo_delegate, std::remove_reference_t<T> > > > { using type = write8smo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write16smo_delegate, std::remove_reference_t<T> > > > { using type = write16smo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write32smo_delegate, std::remove_reference_t<T> > > > { using type = write32smo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write64smo_delegate, std::remove_reference_t<T> > > > { using type = write64smo_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    template <typename T> using rw_delegate_type_t = typename rw_delegate_type<T>::type;
    template <typename T> using rw_delegate_device_class_t = typename rw_delegate_type<T>::device_class;


    template <typename T>
    inline rw_delegate_type_t<T> make_delegate(device_t &base, char const *tag, T &&func, char const *name)
    { return rw_delegate_type_t<T>(base, tag, std::forward<T>(func), name); }

    template <typename T>
    inline rw_delegate_type_t<T> make_delegate(rw_delegate_device_class_t<T> &object, T &&func, char const *name)
    { return rw_delegate_type_t<T>(object, std::forward<T>(func), name); }


    template <typename L>
    inline std::enable_if_t<std::is_constructible<read8_delegate, device_t &, L, const char *>::value, read8_delegate> make_lr8_delegate(device_t &owner, L &&l, const char *name)
    { return read8_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read8m_delegate, device_t &, L, const char *>::value, read8m_delegate> make_lr8_delegate(device_t &owner, L &&l, const char *name)
    { return read8m_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read8s_delegate, device_t &, L, const char *>::value, read8s_delegate> make_lr8_delegate(device_t &owner, L &&l, const char *name)
    { return read8s_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read8sm_delegate, device_t &, L, const char *>::value, read8sm_delegate> make_lr8_delegate(device_t &owner, L &&l, const char *name)
    { return read8sm_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read8mo_delegate, device_t &, L, const char *>::value, read8mo_delegate> make_lr8_delegate(device_t &owner, L &&l, const char *name)
    { return read8mo_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read8smo_delegate, device_t &, L, const char *>::value, read8smo_delegate> make_lr8_delegate(device_t &owner, L &&l, const char *name)
    { return read8smo_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read16_delegate, device_t &, L, const char *>::value, read16_delegate> make_lr16_delegate(device_t &owner, L &&l, const char *name)
    { return read16_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read16m_delegate, device_t &, L, const char *>::value, read16m_delegate> make_lr16_delegate(device_t &owner, L &&l, const char *name)
    { return read16m_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read16s_delegate, device_t &, L, const char *>::value, read16s_delegate> make_lr16_delegate(device_t &owner, L &&l, const char *name)
    { return read16s_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read16sm_delegate, device_t &, L, const char *>::value, read16sm_delegate> make_lr16_delegate(device_t &owner, L &&l, const char *name)
    { return read16sm_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read16mo_delegate, device_t &, L, const char *>::value, read16mo_delegate> make_lr16_delegate(device_t &owner, L &&l, const char *name)
    { return read16mo_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read16smo_delegate, device_t &, L, const char *>::value, read16smo_delegate> make_lr16_delegate(device_t &owner, L &&l, const char *name)
    { return read16smo_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read32_delegate, device_t &, L, const char *>::value, read32_delegate> make_lr32_delegate(device_t &owner, L &&l, const char *name)
    { return read32_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read32m_delegate, device_t &, L, const char *>::value, read32m_delegate> make_lr32_delegate(device_t &owner, L &&l, const char *name)
    { return read32m_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read32s_delegate, device_t &, L, const char *>::value, read32s_delegate> make_lr32_delegate(device_t &owner, L &&l, const char *name)
    { return read32s_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read32sm_delegate, device_t &, L, const char *>::value, read32sm_delegate> make_lr32_delegate(device_t &owner, L &&l, const char *name)
    { return read32sm_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read32mo_delegate, device_t &, L, const char *>::value, read32mo_delegate> make_lr32_delegate(device_t &owner, L &&l, const char *name)
    { return read32mo_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read32smo_delegate, device_t &, L, const char *>::value, read32smo_delegate> make_lr32_delegate(device_t &owner, L &&l, const char *name)
    { return read32smo_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read64_delegate, device_t &, L, const char *>::value, read64_delegate> make_lr64_delegate(device_t &owner, L &&l, const char *name)
    { return read64_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read64m_delegate, device_t &, L, const char *>::value, read64m_delegate> make_lr64_delegate(device_t &owner, L &&l, const char *name)
    { return read64m_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read64s_delegate, device_t &, L, const char *>::value, read64s_delegate> make_lr64_delegate(device_t &owner, L &&l, const char *name)
    { return read64s_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read64sm_delegate, device_t &, L, const char *>::value, read64sm_delegate> make_lr64_delegate(device_t &owner, L &&l, const char *name)
    { return read64sm_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read64mo_delegate, device_t &, L, const char *>::value, read64mo_delegate> make_lr64_delegate(device_t &owner, L &&l, const char *name)
    { return read64mo_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read64smo_delegate, device_t &, L, const char *>::value, read64smo_delegate> make_lr64_delegate(device_t &owner, L &&l, const char *name)
    { return read64smo_delegate(owner, std::forward<L>(l), name); }


    template <typename L>
    inline std::enable_if_t<std::is_constructible<write8_delegate, device_t &, L, const char *>::value, write8_delegate> make_lw8_delegate(device_t &owner, L &&l, const char *name)
    { return write8_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write8m_delegate, device_t &, L, const char *>::value, write8m_delegate> make_lw8_delegate(device_t &owner, L &&l, const char *name)
    { return write8m_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write8s_delegate, device_t &, L, const char *>::value, write8s_delegate> make_lw8_delegate(device_t &owner, L &&l, const char *name)
    { return write8s_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write8sm_delegate, device_t &, L, const char *>::value, write8sm_delegate> make_lw8_delegate(device_t &owner, L &&l, const char *name)
    { return write8sm_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write8mo_delegate, device_t &, L, const char *>::value, write8mo_delegate> make_lw8_delegate(device_t &owner, L &&l, const char *name)
    { return write8mo_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write8smo_delegate, device_t &, L, const char *>::value, write8smo_delegate> make_lw8_delegate(device_t &owner, L &&l, const char *name)
    { return write8smo_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write16_delegate, device_t &, L, const char *>::value, write16_delegate> make_lw16_delegate(device_t &owner, L &&l, const char *name)
    { return write16_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write16m_delegate, device_t &, L, const char *>::value, write16m_delegate> make_lw16_delegate(device_t &owner, L &&l, const char *name)
    { return write16m_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write16s_delegate, device_t &, L, const char *>::value, write16s_delegate> make_lw16_delegate(device_t &owner, L &&l, const char *name)
    { return write16s_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write16sm_delegate, device_t &, L, const char *>::value, write16sm_delegate> make_lw16_delegate(device_t &owner, L &&l, const char *name)
    { return write16sm_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write16mo_delegate, device_t &, L, const char *>::value, write16mo_delegate> make_lw16_delegate(device_t &owner, L &&l, const char *name)
    { return write16mo_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write16smo_delegate, device_t &, L, const char *>::value, write16smo_delegate> make_lw16_delegate(device_t &owner, L &&l, const char *name)
    { return write16smo_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write32_delegate, device_t &, L, const char *>::value, write32_delegate> make_lw32_delegate(device_t &owner, L &&l, const char *name)
    { return write32_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write32m_delegate, device_t &, L, const char *>::value, write32m_delegate> make_lw32_delegate(device_t &owner, L &&l, const char *name)
    { return write32m_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write32s_delegate, device_t &, L, const char *>::value, write32s_delegate> make_lw32_delegate(device_t &owner, L &&l, const char *name)
    { return write32s_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write32sm_delegate, device_t &, L, const char *>::value, write32sm_delegate> make_lw32_delegate(device_t &owner, L &&l, const char *name)
    { return write32sm_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write32mo_delegate, device_t &, L, const char *>::value, write32mo_delegate> make_lw32_delegate(device_t &owner, L &&l, const char *name)
    { return write32mo_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write32smo_delegate, device_t &, L, const char *>::value, write32smo_delegate> make_lw32_delegate(device_t &owner, L &&l, const char *name)
    { return write32smo_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write64_delegate, device_t &, L, const char *>::value, write64_delegate> make_lw64_delegate(device_t &owner, L &&l, const char *name)
    { return write64_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write64m_delegate, device_t &, L, const char *>::value, write64m_delegate> make_lw64_delegate(device_t &owner, L &&l, const char *name)
    { return write64m_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write64s_delegate, device_t &, L, const char *>::value, write64s_delegate> make_lw64_delegate(device_t &owner, L &&l, const char *name)
    { return write64s_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write64sm_delegate, device_t &, L, const char *>::value, write64sm_delegate> make_lw64_delegate(device_t &owner, L &&l, const char *name)
    { return write64sm_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write64mo_delegate, device_t &, L, const char *>::value, write64mo_delegate> make_lw64_delegate(device_t &owner, L &&l, const char *name)
    { return write64mo_delegate(owner, std::forward<L>(l), name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write64smo_delegate, device_t &, L, const char *>::value, write64smo_delegate> make_lw64_delegate(device_t &owner, L &&l, const char *name)
    { return write64smo_delegate(owner, std::forward<L>(l), name); }
#endif


    public static partial class emumem_global
    {
        // =====================-> Address segmentation for the search tree

        public static int handler_entry_dispatch_level(int highbits)
        {
            return (highbits > 48) ? 3 : (highbits > 32) ? 2 : (highbits > 14) ? 1 : 0;
        }


        static int handler_entry_dispatch_level_to_lowbits(int level, int width, int ashift)
        {
            return level == 3 ? 48 : level == 2 ? 32 : level == 1 ? 14 : width + ashift;
        }


        //constexpr int handler_entry_dispatch_lowbits(int highbits, int width, int ashift)


        // ======================> memopry_units_descritor forwards declaration

        //template<int Width, int AddrShift, endianness_t Endian> class memory_units_descriptor;
    }


    // read or write constants
    public enum read_or_write
    {
        READ = 1,
        WRITE = 2,
        READWRITE = 3
    }


    public static partial class emumem_global
    {
        const bool MEM_DUMP = false;
        const bool VERBOSE = false;
        public const bool VALIDATE_REFCOUNTS = false;


        // address space names for common use
        public const int AS_PROGRAM = 0; // program address space
        public const int AS_DATA    = 1; // data address space
        public const int AS_IO      = 2; // I/O address space
        public const int AS_OPCODES = 3; // (decrypted) opcodes, when separate from data accesses


        // other address map constants
        public const int MEMORY_BLOCK_CHUNK = 65536;                   // minimum chunk size of allocated memory blocks


        //**************************************************************************
        //  MACROS
        //**************************************************************************

        // helper macro for merging data with the memory mask
        //#define COMBINE_DATA(varptr)            (*(varptr) = (*(varptr) & ~mem_mask) | (data & mem_mask))
        public static void COMBINE_DATA(ref u16 varptr, u16 data, u16 mem_mask) { varptr = (u16)((varptr & ~mem_mask) | (data & mem_mask)); }
        public static void COMBINE_DATA(ref u32 varptr, u32 data, u32 mem_mask) { varptr = (varptr & ~mem_mask) | (data & mem_mask); }

        public static bool ACCESSING_BITS_0_7(u16 mem_mask) { return (mem_mask & 0x000000ffU) != 0; }
        //#define ACCESSING_BITS_8_15             ((mem_mask & 0x0000ff00U) != 0)
        //#define ACCESSING_BITS_16_23            ((mem_mask & 0x00ff0000U) != 0)
        //#define ACCESSING_BITS_24_31            ((mem_mask & 0xff000000U) != 0)
        //#define ACCESSING_BITS_32_39            ((mem_mask & 0x000000ff00000000U) != 0)
        //#define ACCESSING_BITS_40_47            ((mem_mask & 0x0000ff0000000000U) != 0)
        //#define ACCESSING_BITS_48_55            ((mem_mask & 0x00ff000000000000U) != 0)
        //#define ACCESSING_BITS_56_63            ((mem_mask & 0xff00000000000000U) != 0)

        //#define ACCESSING_BITS_0_15             ((mem_mask & 0x0000ffffU) != 0)
        //#define ACCESSING_BITS_16_31            ((mem_mask & 0xffff0000U) != 0)
        //#define ACCESSING_BITS_32_47            ((mem_mask & 0x0000ffff00000000U) != 0)
        //#define ACCESSING_BITS_48_63            ((mem_mask & 0xffff000000000000U) != 0)

        //#define ACCESSING_BITS_0_31             ((mem_mask & 0xffffffffU) != 0)
        //#define ACCESSING_BITS_32_63            ((mem_mask & 0xffffffff00000000U) != 0)

        // macros for accessing bytes and words within larger chunks

        // read/write a byte to a 16-bit space
        //#define BYTE_XOR_BE(a)                  ((a) ^ NATIVE_ENDIAN_VALUE_LE_BE(1,0))
        //#define BYTE_XOR_LE(a)                  ((a) ^ NATIVE_ENDIAN_VALUE_LE_BE(0,1))

        // read/write a byte to a 32-bit space
        //#define BYTE4_XOR_BE(a)                 ((a) ^ NATIVE_ENDIAN_VALUE_LE_BE(3,0))
        //#define BYTE4_XOR_LE(a)                 ((a) ^ NATIVE_ENDIAN_VALUE_LE_BE(0,3))

        // read/write a word to a 32-bit space
        //#define WORD_XOR_BE(a)                  ((a) ^ NATIVE_ENDIAN_VALUE_LE_BE(2,0))
        //#define WORD_XOR_LE(a)                  ((a) ^ NATIVE_ENDIAN_VALUE_LE_BE(0,2))

        // read/write a byte to a 64-bit space
        //#define BYTE8_XOR_BE(a)                 ((a) ^ NATIVE_ENDIAN_VALUE_LE_BE(7,0))
        //#define BYTE8_XOR_LE(a)                 ((a) ^ NATIVE_ENDIAN_VALUE_LE_BE(0,7))

        // read/write a word to a 64-bit space
        //#define WORD2_XOR_BE(a)                 ((a) ^ NATIVE_ENDIAN_VALUE_LE_BE(6,0))
        //#define WORD2_XOR_LE(a)                 ((a) ^ NATIVE_ENDIAN_VALUE_LE_BE(0,6))

        // read/write a dword to a 64-bit space
        //#define DWORD_XOR_BE(a)                 ((a) ^ NATIVE_ENDIAN_VALUE_LE_BE(4,0))
        //#define DWORD_XOR_LE(a)                 ((a) ^ NATIVE_ENDIAN_VALUE_LE_BE(0,4))


        // helpers for checking address alignment
        public static bool WORD_ALIGNED(UInt32 a) { return (a & 1) == 0; }
        public static bool DWORD_ALIGNED(UInt32 a) { return (a & 3) == 0; }
        public static bool QWORD_ALIGNED(UInt32 a) { return (a & 7) == 0; }


        //#if VERBOSE
        //template <typename Format, typename... Params> static void VPRINTF(Format &&fmt, Params &&...args)
        //{
        //    util::stream_format(std::cerr, std::forward<Format>(fmt), std::forward<Params>(args)...);
        //}
        //#else
        //template <typename Format, typename... Params> static void VPRINTF(Format &&, Params &&...) {}
        //#endif
        public static void VPRINTF(string format, params object [] args) { if (VERBOSE) global_object.osd_printf_info(format, args); }


        // =====================-> Width -> types

        //template<int Width> struct handler_entry_size {};
        //template<> struct handler_entry_size<0> { using uX = u8;  };
        //template<> struct handler_entry_size<1> { using uX = u16; };
        //template<> struct handler_entry_size<2> { using uX = u32; };
        //template<> struct handler_entry_size<3> { using uX = u64; };


        // ======================> address offset -> byte offset

        public static offs_t memory_offset_to_byte(offs_t offset, int AddrShift) { return AddrShift < 0 ? offset << g.iabs(AddrShift) : offset >> g.iabs(AddrShift); }


        // ======================> generic read/write decomposition routines

        // generic direct read
        //template<int Width, int AddrShift, endianness_t Endian, int TargetWidth, bool Aligned, typename T>
        public static uX memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_TargetWidth, bool_Aligned>(Func<offs_t, uX, uX> rop, offs_t address, uX mask)  //typename emu::detail::handler_entry_size<TargetWidth>::uX  memory_read_generic(T rop, offs_t address, typename emu::detail::handler_entry_size<TargetWidth>::uX mask)
            where int_Width : int_constant, new()
            where int_AddrShift : int_constant, new()
            where endianness_t_Endian : endianness_t_constant, new()
            where int_TargetWidth : int_constant, new()
            where bool_Aligned : bool_constant, new()
        {
            //using TargetType = typename emu::detail::handler_entry_size<TargetWidth>::uX;
            //using NativeType = typename emu::detail::handler_entry_size<Width>::uX;


            int Width = new int_Width().value;
            int AddrShift = new int_AddrShift().value;
            endianness_t Endian = new endianness_t_Endian().value;
            int TargetWidth = new int_TargetWidth().value;
            bool Aligned = new bool_Aligned().value;


            u32 TARGET_BYTES = 1U << TargetWidth;
            u32 TARGET_BITS = 8 * TARGET_BYTES;
            u32 NATIVE_BYTES = 1U << Width;
            u32 NATIVE_BITS = 8 * NATIVE_BYTES;
            u32 NATIVE_STEP = AddrShift >= 0 ? NATIVE_BYTES << g.iabs(AddrShift) : NATIVE_BYTES >> g.iabs(AddrShift);
            u32 NATIVE_MASK = Width + AddrShift >= 0 ? g.make_bitmask32(Width + AddrShift) : 0;

            // equal to native size and aligned; simple pass-through to the native reader
            if (NATIVE_BYTES == TARGET_BYTES && (Aligned || (address & NATIVE_MASK) == 0))
                return rop(address & ~NATIVE_MASK, mask);

            // if native size is larger, see if we can do a single masked read (guaranteed if we're aligned)
            if (NATIVE_BYTES > TARGET_BYTES)
            {
                u32 offsbits2 = 8 * (memory_offset_to_byte(address, AddrShift) & (NATIVE_BYTES - (Aligned ? TARGET_BYTES : 1)));
                if (Aligned || (offsbits2 + TARGET_BITS <= NATIVE_BITS))
                {
                    if (Endian != endianness_t.ENDIANNESS_LITTLE) offsbits2 = NATIVE_BITS - TARGET_BITS - offsbits2;
                    return rop(address & ~NATIVE_MASK, new uX(Width, mask) << (int)offsbits2) >> (int)offsbits2;  //return rop(address & ~NATIVE_MASK, (NativeType)mask << offsbits) >> offsbits;
                }
            }

            // determine our alignment against the native boundaries, and mask the address
            u32 offsbits = 8 * (memory_offset_to_byte(address, AddrShift) & (NATIVE_BYTES - 1));
            address &= ~NATIVE_MASK;

            // if we're here, and native size is larger or equal to the target, we need exactly 2 reads
            if (NATIVE_BYTES >= TARGET_BYTES)
            {
                // little-endian case
                if (Endian == endianness_t.ENDIANNESS_LITTLE)
                {
                    // read lower bits from lower address
                    uX result = new uX(TargetWidth, 0);  //TargetType result = 0;
                    uX curmask = new uX(Width, mask) << (int)offsbits;  //NativeType curmask = (NativeType)mask << offsbits;
                    if (curmask != 0) result = rop(address, curmask) >> (int)offsbits;

                    // read upper bits from upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = new uX(Width, mask >> (int)offsbits);
                    if (curmask != 0) result |= rop(address + NATIVE_STEP, curmask) << (int)offsbits;
                    return result;
                }

                // big-endian case
                else
                {
                    // left-justify the mask to the target type
                    u32 LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT = ((NATIVE_BITS >= TARGET_BITS) ? (NATIVE_BITS - TARGET_BITS) : 0);
                    uX result = new uX(Width, 0);  //NativeType result = 0;
                    uX ljmask = new uX(Width, mask) << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT;  //NativeType ljmask = (NativeType)mask << LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT;
                    uX curmask = ljmask >> (int)offsbits;  //NativeType curmask = ljmask >> offsbits;

                    // read upper bits from lower address
                    if (curmask != 0) result = rop(address, curmask) << (int)offsbits;
                    offsbits = NATIVE_BITS - offsbits;

                    // read lower bits from upper address
                    curmask = ljmask << (int)offsbits;
                    if (curmask != 0) result |= rop(address + NATIVE_STEP, curmask) >> (int)offsbits;

                    // return the un-justified result
                    return result >> (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT;
                }
            }

            // if we're here, then we have 2 or more reads needed to get our final result
            else
            {
                // compute the maximum number of loops; we do it this way so that there are
                // a fixed number of loops for the compiler to unroll if it desires
                u32 MAX_SPLITS_MINUS_ONE = TARGET_BYTES / NATIVE_BYTES - 1;
                uX result = new uX(TargetWidth, 0);  //TargetType result = 0;

                // little-endian case
                if (Endian == endianness_t.ENDIANNESS_LITTLE)
                {
                    // read lowest bits from first address
                    uX curmask = new uX(Width, mask << (int)offsbits);  //NativeType curmask = mask << offsbits;
                    if (curmask != 0) result = new uX(TargetWidth, rop(address, curmask) >> (int)offsbits);

                    // read middle bits from subsequent addresses
                    offsbits = NATIVE_BITS - offsbits;
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        address += NATIVE_STEP;
                        curmask = new uX(Width, mask >> (int)offsbits);
                        if (curmask != 0) result |= new uX(TargetWidth, rop(address, curmask)) << (int)offsbits;  //if (curmask != 0) result |= (TargetType)rop(address, curmask) << offsbits;
                        offsbits += NATIVE_BITS;
                    }

                    // if we're not aligned and we still have bits left, read uppermost bits from last address
                    if (!Aligned && offsbits < TARGET_BITS)
                    {
                        curmask = new uX(Width, mask >> (int)offsbits);
                        if (curmask != 0) result |= new uX(TargetWidth, rop(address + NATIVE_STEP, curmask)) << (int)offsbits;  //if (curmask != 0) result |= (TargetType)rop(address + NATIVE_STEP, curmask) << offsbits;
                    }
                }

                // big-endian case
                else
                {
                    // read highest bits from first address
                    offsbits = TARGET_BITS - (NATIVE_BITS - offsbits);
                    uX curmask = new uX(Width, mask >> (int)offsbits);  //NativeType curmask = mask >> offsbits;
                    if (curmask != 0) result = new uX(TargetWidth, rop(address, curmask)) << (int)offsbits;  //if (curmask != 0) result = (TargetType)rop(address, curmask) << offsbits;

                    // read middle bits from subsequent addresses
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        offsbits -= NATIVE_BITS;
                        address += NATIVE_STEP;
                        curmask = new uX(Width, mask >> (int)offsbits);
                        if (curmask != 0) result |= new uX(TargetWidth, rop(address, curmask)) << (int)offsbits;  //if (curmask != 0) result |= (TargetType)rop(address, curmask) << offsbits;
                    }

                    // if we're not aligned and we still have bits left, read lowermost bits from the last address
                    if (!Aligned && offsbits != 0)
                    {
                        offsbits = NATIVE_BITS - offsbits;
                        curmask = new uX(Width, mask << (int)offsbits);
                        if (curmask != 0) result |= rop(address + NATIVE_STEP, curmask) >> (int)offsbits;
                    }
                }

                return result;
            }
        }


        //template<int Width, int AddrShift, endianness_t Endian, int TargetWidth, bool Aligned, typename T>
        public static void memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_TargetWidth, bool_Aligned>(Action<offs_t, uX, uX> wop, offs_t address, uX data, uX mask)
        {
            throw new emu_unimplemented();
        }


        // ======================> Direct dispatching

        public static uX dispatch_read<int_Level, int_Width, int_AddrShift, endianness_t_Endian>(offs_t mask, offs_t offset, uX mem_mask, Pointer<handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian>> dispatch)  //template<int Level, int Width, int AddrShift, endianness_t Endian> typename emu::detail::handler_entry_size<Width>::uX dispatch_read(offs_t mask, offs_t offset, typename emu::detail::handler_entry_size<Width>::uX mem_mask, const handler_entry_read<Width, AddrShift, Endian> *const *dispatch)
            where int_Level : int_constant, new()
            where int_Width : int_constant, new()
            where int_AddrShift : int_constant, new()
            where endianness_t_Endian : endianness_t_constant, new()
        {
            int Level = new int_Level().value;
            int Width = new int_Width().value;
            int AddrShift = new int_AddrShift().value;

            u32 LowBits = (u32)emumem_global.handler_entry_dispatch_level_to_lowbits(Level, Width, AddrShift);  //static constexpr u32 LowBits  = emu::detail::handler_entry_dispatch_level_to_lowbits(Level, Width, AddrShift);
            return dispatch[(offset & mask) >> (int)LowBits].read(offset, mem_mask);  //return dispatch[(offset & mask) >> LowBits]->read(offset, mem_mask);
        }


        public static void dispatch_write<int_Level, int_Width, int_AddrShift, endianness_t_Endian>(offs_t mask, offs_t offset, uX data, uX mem_mask, Pointer<handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian>> dispatch)  //template<int Level, int Width, int AddrShift, endianness_t Endian> void dispatch_write(offs_t mask, offs_t offset, typename emu::detail::handler_entry_size<Width>::uX data, typename emu::detail::handler_entry_size<Width>::uX mem_mask, const handler_entry_write<Width, AddrShift, Endian> *const *dispatch)
            where int_Level : int_constant, new()
            where int_Width : int_constant, new()
            where int_AddrShift : int_constant, new()
            where endianness_t_Endian : endianness_t_constant, new()
        {
            int Level = new int_Level().value;
            int Width = new int_Width().value;
            int AddrShift = new int_AddrShift().value;

            u32 LowBits = (u32)emumem_global.handler_entry_dispatch_level_to_lowbits(Level, Width, AddrShift);  //static constexpr u32 LowBits  = emu::detail::handler_entry_dispatch_level_to_lowbits(Level, Width, AddrShift);
            dispatch[(offset & mask) >> (int)LowBits].write(offset, data, mem_mask);  //return dispatch[(offset & mask) >> LowBits]->write(offset, data, mem_mask);
        }


        // debugging
        //-------------------------------------------------
        //  generate_memdump - internal memory dump
        //-------------------------------------------------
        public static void generate_memdump(running_machine machine)
        {
            if (MEM_DUMP)
            {
                throw new emu_unimplemented();
            }
        }


        // =====================-> Address segmentation for the search tree
        public static int handler_entry_dispatch_lowbits(int highbits, int width, int ashift)
        {
            return (highbits > 48) ? 48 :
                   (highbits > 32) ? 32 :
                   (highbits > 14) ? 14 :
                   width + ashift;
        }
    }


#if false
    // struct with function pointers for accessors; use is generally discouraged unless necessary
    struct data_accessors
    {
        u8      (*read_byte)(address_space &space, offs_t address);
        u16     (*read_word)(address_space &space, offs_t address);
        u16     (*read_word_masked)(address_space &space, offs_t address, u16 mask);
        u32     (*read_dword)(address_space &space, offs_t address);
        u32     (*read_dword_masked)(address_space &space, offs_t address, u32 mask);
        u64     (*read_qword)(address_space &space, offs_t address);
        u64     (*read_qword_masked)(address_space &space, offs_t address, u64 mask);

        void    (*write_byte)(address_space &space, offs_t address, u8 data);
        void    (*write_word)(address_space &space, offs_t address, u16 data);
        void    (*write_word_masked)(address_space &space, offs_t address, u16 data, u16 mask);
        void    (*write_dword)(address_space &space, offs_t address, u32 data);
        void    (*write_dword_masked)(address_space &space, offs_t address, u32 data, u32 mask);
        void    (*write_qword)(address_space &space, offs_t address, u64 data);
        void    (*write_qword_masked)(address_space &space, offs_t address, u64 data, u64 mask);
    };
#endif


    public struct uX
    {
        int m_width;
        u8  m_x8;
        u16 m_x16;
        u32 m_x32;
        u64 m_x64;


        public uX(int width, u64 initial) { this = default; this.m_width = width; Set(initial); }
        public uX(int width, uX initial) { this = default; this.m_width = width; Set(initial); }


        public static uX MaxValue(int width) { return new uX(width, u64.MaxValue); }


        public int width { get { return m_width; } }
        public u8 x8 { get { return (u8)Get(); } }
        public u16 x16 { get { return (u16)Get(); } }
        public u32 x32 { get { return (u32)Get(); } }
        public u64 x64 { get { return (u64)Get(); } }


        void Set(u64 value)
        {
            switch (width)
            {
                case 0: m_x8  = (u8)value; break;
                case 1: m_x16 = (u16)value; break;
                case 2: m_x32 = (u32)value; break;
                case 3: m_x64 = (u64)value; break;
                default: throw new emu_unimplemented();
            }
        }

        void Set(uX value) { Set(value.Get()); }


        u64 Get()
        {
            switch (width)
            {
                case 0: return m_x8;
                case 1: return m_x16;
                case 2: return m_x32;
                case 3: return m_x64;
                default: throw new emu_unimplemented();
            }
        }


        public static bool operator ==(uX left, uX right) { return left.Get() == right.Get(); }
        public static bool operator !=(uX left, uX right) { return left.Get() != right.Get(); }
        public static bool operator ==(uX left, u64 right) { return left.Get() == right; }
        public static bool operator !=(uX left, u64 right) { return left.Get() != right; }

        public static uX operator +(uX left, u64 right) { return new uX(left.width, left.Get() + right); }
        public static uX operator +(uX left, uX right) { return new uX(left.width, left.Get() + right.Get()); }
        public static uX operator <<(uX left, int right) { return new uX(left.width, left.Get() << right); }
        public static uX operator >>(uX left, int right) { return new uX(left.width, left.Get() >> right); }
        public static uX operator |(uX left, u64 right) { return new uX(left.width, left.Get() | right); }
        public static uX operator |(uX left, uX right) { return new uX(left.width, left.Get() | right.Get()); }
        public static uX operator &(uX left, u64 right) { return new uX(left.width, left.Get() & right); }
        public static uX operator &(uX left, uX right) { return new uX(left.width, left.Get() & right.Get()); }

        public static uX operator ~(uX left) { return new uX(left.width, ~left.Get()); }


        public u32 sizeof_()
        {
            switch (width)
            {
                case 0: return 1;
                case 1: return 2;
                case 2: return 4;
                case 3: return 8;
                default: throw new emu_unimplemented();
            }
        }
    }


    // struct with function pointers for accessors; use is generally discouraged unless necessary
    public struct data_accessors
    {
        //public Func<address_space, offs_t, u8> read_byte;  //u8      (*read_byte)(address_space &space, offs_t address);
        //public Func<address_space, offs_t, u16> read_word;  //u16     (*read_word)(address_space &space, offs_t address);
        //public Func<address_space, offs_t, u16, u16> read_word_masked;  //u16     (*read_word_masked)(address_space &space, offs_t address, u16 mask);
        //public Func<address_space, offs_t, u32> read_dword;  //u32     (*read_dword)(address_space &space, offs_t address);
        //public Func<address_space, offs_t, u32, u32> read_dword_masked;  //u32     (*read_dword_masked)(address_space &space, offs_t address, u32 mask);
        //public Func<address_space, offs_t, u64> read_qword;  //u64     (*read_qword)(address_space &space, offs_t address);
        //public Func<address_space, offs_t, u64, u64> read_qword_masked;  //u64     (*read_qword_masked)(address_space &space, offs_t address, u64 mask);

        //public Action<address_space, offs_t, u8> write_byte;  //void    (*write_byte)(address_space &space, offs_t address, u8 data);
        //public Action<address_space, offs_t, u16> write_word;  //void    (*write_word)(address_space &space, offs_t address, u16 data);
        //public Action<address_space, offs_t, u16, u16> write_word_masked;  //void    (*write_word_masked)(address_space &space, offs_t address, u16 data, u16 mask);
        //public Action<address_space, offs_t, u32> write_dword;  //void    (*write_dword)(address_space &space, offs_t address, u32 data);
        //public Action<address_space, offs_t, u32, u32> write_dword_masked;  //void    (*write_dword_masked)(address_space &space, offs_t address, u32 data, u32 mask);
        //public Action<address_space, offs_t, u64> write_qword;  //void    (*write_qword)(address_space &space, offs_t address, u64 data);
        //public Action<address_space, offs_t, u64, u64> write_qword_masked;  //void    (*write_qword_masked)(address_space &space, offs_t address, u64 data, u64 mask);
    }


    // a line in the memory structure dump
    class memory_entry_context
    {
        memory_view view;  //memory_view *view;
        bool disabled;
        int slot;
    }


    public class memory_entry
    {
        offs_t start;
        offs_t end;
        handler_entry entry;
        std.vector<memory_entry_context> context;
    }


    // =====================-> The root class of all handlers

    // Handlers the refcounting as part of the interface

    public abstract class handler_entry : global_object, IDisposable
    {
        //DISABLE_COPYING(handler_entry);

        //template<int Level, int Width, int AddrShift, endianness_t Endian> friend class address_space_specific;


        // Typing flags
        public const u32 F_DISPATCH       = 0x00000001; // handler that forwards the access to other handlers
        protected const u32 F_UNITS       = 0x00000002; // handler that merges/splits an access among multiple handlers (unitmask support)
        protected const u32 F_PASSTHROUGH = 0x00000004; // handler that passes through the request to another handler
        const u32 F_VIEW                  = 0x00000008; // handler for a view (kinda like dispatch except not entirely)


        // Start/end of range flags
        public const u8 START = 1;
        public const u8 END   = 2;


        // Intermediary structure for reference count checking
        public class reflist
        {
            //std::unordered_map<const handler_entry *, u32> refcounts;
            //std::unordered_set<const handler_entry *> seen;
            //std::unordered_set<const handler_entry *> todo;


            //public void add(handler_entry entry);
            //public void propagate();
            //public void check();
        }


        // Address range storage
        public class range
        {
            public offs_t start;
            public offs_t end;


            public range() { }
            public range(range other) { start = other.start; end = other.end; }


            public void set(offs_t _start, offs_t _end)
            {
                start = _start;
                end = _end;
            }

            public void intersect(offs_t _start, offs_t _end)
            {
                if (_start > start)
                    start = _start;
                if (_end < end)
                    end = _end;
            }
        }


        protected address_space m_space;
        u32 m_refcount;
        u32 m_flags;


        protected handler_entry(address_space space, u32 flags) { m_space = space; m_refcount = 1; m_flags = flags; }
        //~handler_entry() {}

        public virtual void Dispose() { }


        public void ref_(int count = 1) { m_refcount += (u32)count; }
        public void unref(int count = 1) { m_refcount -= (u32)count;  if (m_refcount == 0) this.Dispose(); /*delete this;*/ }
        //inline u32 flags() const { return m_flags; }

        public bool is_dispatch() { return (m_flags & F_DISPATCH) != 0; }
        public bool is_view() { return (m_flags & F_VIEW) != 0; }
        public bool is_units() { return (m_flags & F_UNITS) != 0; }
        //inline bool is_passthrough() const { return m_flags & F_PASSTHROUGH; }


        protected virtual void dump_map(std.vector<memory_entry> map)
        {
            fatalerror("dump_map called on non-dispatching class\n");
        }

        protected abstract string name();

        protected virtual void enumerate_references(reflist refs) { }


        //u32 get_refcount() const { return m_refcount; }


        public virtual void select_a(int slot)
        {
            fatalerror("select_a called on non-view\n");
        }

        public virtual void select_u(int slot)
        {
            fatalerror("select_u called on non-view\n");
        }
    }


    // =====================-> The parent class of all read handlers

    // Provides the populate/read/get_ptr/lookup API

    //template<int Width, int AddrShift, endianness_t Endian> class handler_entry_read_passthrough;


    //template<int Width, int AddrShift, endianness_t Endian>
    public abstract class handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian> : handler_entry
        where int_Width : int_constant, new()
        where int_AddrShift : int_constant, new()
        where endianness_t_Endian : endianness_t_constant, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        protected static readonly int Width = new int_Width().value;
        protected static readonly int AddrShift = new int_AddrShift().value;


        protected static readonly u32 NATIVE_MASK = Width + AddrShift >= 0 ? g.make_bitmask32(Width + AddrShift) : 0;  //static constexpr u32 NATIVE_MASK = Width + AddrShift >= 0 ? make_bitmask<u32>(Width + AddrShift) : 0;


        public class mapping
        {
            public handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian> original;
            public handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian> patched;
            public u8 ukey;
        }


        protected handler_entry_read(address_space space, u32 flags) : base(space, flags) { }

        //~handler_entry_read() {}


        public abstract uX read(offs_t offset, uX mem_mask);


        public virtual object get_ptr(offs_t offset)  //virtual void *get_ptr(offs_t offset) const;
        {
            return null;
        }


        public virtual void lookup(offs_t address, ref offs_t start, ref offs_t end, ref handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian> handler)
        {
            fatalerror("lookup called on non-dispatching class\n");
        }


        public void populate(offs_t start, offs_t end, offs_t mirror, handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian> handler)
        {
            start &= ~NATIVE_MASK;
            end |= NATIVE_MASK;
            if (mirror != 0)
                populate_mirror(start, end, start, end, mirror, handler);
            else
                populate_nomirror(start, end, start, end, handler);
        }


        public virtual void populate_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian> handler)
        {
            fatalerror("populate called on non-dispatching class\n");
        }


        public virtual void populate_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian> handler)
        {
            fatalerror("populate called on non-dispatching class\n");
        }


        public void populate_mismatched(offs_t start, offs_t end, offs_t mirror, memory_units_descriptor<int_Width, int_AddrShift, endianness_t_Endian> descriptor)
        {
            start &= ~NATIVE_MASK;
            end |= NATIVE_MASK;
            std.vector<mapping> mappings = new std.vector<mapping>();
            if (mirror != 0)
                populate_mismatched_mirror(start, end, start, end, mirror, descriptor, mappings);
            else
                populate_mismatched_nomirror(start, end, start, end, descriptor, START | END, mappings);
        }


        public virtual void populate_mismatched_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, memory_units_descriptor<int_Width, int_AddrShift, endianness_t_Endian> descriptor, u8 rkey, std.vector<mapping> mappings)
        {
            fatalerror("populate_mismatched called on non-dispatching class\n");
        }


        public virtual void populate_mismatched_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, memory_units_descriptor<int_Width, int_AddrShift, endianness_t_Endian> descriptor, std.vector<mapping> mappings)
        {
            fatalerror("populate_mismatched called on non-dispatching class\n");
        }


        //void populate_passthrough(offs_t start, offs_t end, offs_t mirror, handler_entry_read_passthrough<Width, AddrShift, Endian> *handler)
        //{
        //    start &= ~NATIVE_MASK;
        //    end |= NATIVE_MASK;
        //    std::vector<mapping> mappings;
        //    if(mirror)
        //        populate_passthrough_mirror(start, end, start, end, mirror, handler, mappings);
        //    else
        //        populate_passthrough_nomirror(start, end, start, end, handler, mappings);
        //}


        protected virtual void populate_passthrough_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read_passthrough<int_Width, int_AddrShift, endianness_t_Endian> handler, std.vector<mapping> mappings)
        {
            fatalerror("populate_passthrough called on non-dispatching class\n");
        }


        protected virtual void populate_passthrough_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read_passthrough<int_Width, int_AddrShift, endianness_t_Endian> handler, std.vector<mapping> mappings)
        {
            fatalerror("populate_passthrough called on non-dispatching class\n");
        }


        // Remove a set of passthrough handlers, leaving the lower handler in their place
        public virtual void detach(std.unordered_set<handler_entry> handlers)
        {
            fatalerror("detach called on non-dispatching class\n");
        }


        // Return the internal structures of the root dispatch
        public virtual Pointer<handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian>> get_dispatch()
        {
            fatalerror("get_dispatch called on non-dispatching class\n");
            return null;
        }


        public virtual void init_handlers(offs_t start_entry, offs_t end_entry, u32 lowbits, Pointer<handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian>> dispatch, Pointer<handler_entry.range> ranges)
        {
            fatalerror("init_handlers called on non-view class\n");
        }


        public virtual handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian> dup()
        {
            ref_();
            return this;
        }
    }


    // =====================-> The parent class of all write handlers

    // Provides the populate/write/get_ptr/lookup API

    //template<int Width, int AddrShift, endianness_t Endian> class handler_entry_write_passthrough;


    //template<int Width, int AddrShift, endianness_t Endian>
    public abstract class handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> : handler_entry
        where int_Width : int_constant, new()
        where int_AddrShift : int_constant, new()
        where endianness_t_Endian : endianness_t_constant, new()
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        protected static readonly int Width = new int_Width().value;
        protected static readonly int AddrShift = new int_AddrShift().value;
        protected static readonly endianness_t Endian = new endianness_t_Endian().value;


        protected static readonly u32 NATIVE_MASK = Width + AddrShift >= 0 ? g.make_bitmask32(Width + AddrShift) : 0;  //static constexpr u32 NATIVE_MASK = Width + AddrShift >= 0 ? make_bitmask<u32>(Width + AddrShift) : 0;


        public class mapping
        {
            public handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> original;
            public handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> patched;
            public u8 ukey;
        }


        protected handler_entry_write(address_space space, u32 flags) : base(space, flags) { }

        //virtual ~handler_entry_write() {}


        public abstract void write(offs_t offset, uX data, uX mem_mask);


        public virtual object get_ptr(offs_t offset)  //virtual void *get_ptr(offs_t offset) const;
        {
            return null;
        }


        protected virtual void lookup(offs_t address, ref offs_t start, ref offs_t end, ref handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> handler)
        {
            fatalerror("lookup called on non-dispatching class\n");
        }


        public void populate(offs_t start, offs_t end, offs_t mirror, handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> handler)
        {
            start &= ~NATIVE_MASK;
            end |= NATIVE_MASK;
            if (mirror != 0)
                populate_mirror(start, end, start, end, mirror, handler);
            else
                populate_nomirror(start, end, start, end, handler);
        }


        public virtual void populate_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> handler)
        {
            fatalerror("populate called on non-dispatching class\n");
        }


        public virtual void populate_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> handler)
        {
            fatalerror("populate called on non-dispatching class\n");
        }


        public void populate_mismatched(offs_t start, offs_t end, offs_t mirror, memory_units_descriptor<int_Width, int_AddrShift, endianness_t_Endian> descriptor)
        {
            start &= ~NATIVE_MASK;
            end |= NATIVE_MASK;

            std.vector<mapping> mappings = new std.vector<mapping>();
            if (mirror != 0)
                populate_mismatched_mirror(start, end, start, end, mirror, descriptor, mappings);
            else
                populate_mismatched_nomirror(start, end, start, end, descriptor, START | END, mappings);
        }


        public virtual void populate_mismatched_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, memory_units_descriptor<int_Width, int_AddrShift, endianness_t_Endian> descriptor, u8 rkey, std.vector<mapping> mappings)
        {
            fatalerror("populate_mismatched called on non-dispatching class\n");
        }


        public virtual void populate_mismatched_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, memory_units_descriptor<int_Width, int_AddrShift, endianness_t_Endian> descriptor, std.vector<mapping> mappings)
        {
            fatalerror("populate_mismatched called on non-dispatching class\n");
        }


        //void populate_passthrough(offs_t start, offs_t end, offs_t mirror, handler_entry_write_passthrough<Width, AddrShift, Endian> *handler)
        //{
        //    start &= ~NATIVE_MASK;
        //    end |= NATIVE_MASK;
        //    std::vector<mapping> mappings;
        //    if(mirror)
        //        populate_passthrough_mirror(start, end, start, end, mirror, handler, mappings);
        //    else
        //        populate_passthrough_nomirror(start, end, start, end, handler, mappings);
        //}


        protected virtual void populate_passthrough_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_write_passthrough<int_Width, int_AddrShift, endianness_t_Endian> handler, std.vector<mapping> mappings)
        {
            fatalerror("populate_passthrough called on non-dispatching class\n");
        }


        protected virtual void populate_passthrough_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_write_passthrough<int_Width, int_AddrShift, endianness_t_Endian> handler, std.vector<mapping> mappings)
        {
            fatalerror("populate_passthrough called on non-dispatching class\n");
        }


        // Remove a set of passthrough handlers, leaving the lower handler in their place
        public virtual void detach(std.unordered_set<handler_entry> handlers)
        {
            fatalerror("detach called on non-dispatching class\n");
        }


        // Return the internal structures of the root dispatch
        public virtual Pointer<handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian>> get_dispatch()
        {
            fatalerror("get_dispatch called on non-dispatching class\n");
            return null;
        }


        public virtual void init_handlers(offs_t start_entry, offs_t end_entry, u32 lowbits, Pointer<handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian>> dispatch, Pointer<handler_entry.range> ranges)
        {
            fatalerror("init_handlers called on non-view class\n");
        }


        public virtual handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> dup()
        {
            ref_();
            return this;
        }
    }


    // =====================-> Passthrough handler management structure
    public class memory_passthrough_handler
    {
        //template<int Width, int AddrShift, endianness_t Endian> friend class handler_entry_read_passthrough;
        //template<int Width, int AddrShift, endianness_t Endian> friend class handler_entry_write_passthrough;


        address_space m_space;
        std.unordered_set<handler_entry> m_handlers = new std.unordered_set<handler_entry>();


        memory_passthrough_handler(address_space space) { m_space = space; }


        //inline void remove();


        public void add_handler(handler_entry handler) { m_handlers.insert(handler); }
        public void remove_handler(handler_entry handler) { m_handlers.erase(handler); }  //void remove_handler(handler_entry *handler) { m_handlers.erase(m_handlers.find(handler)); }
    }


    // =====================-> Forward declaration for address_space

    //template<int Width, int AddrShift, endianness_t Endian> class handler_entry_read_unmapped;
    //template<int Width, int AddrShift, endianness_t Endian> class handler_entry_write_unmapped;


    // ======================> memory_access_specific
    // memory_access_specific does uncached but faster accesses by shortcutting the address_space virtual call

    namespace emu.detail {

    //template<int Level, int Width, int AddrShift, endianness_t Endian>
    public class memory_access_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian>
        where int_Level : int_constant, new()
        where int_Width : int_constant, new()
        where int_AddrShift : int_constant, new()
        where endianness_t_Endian : endianness_t_constant, new()
    {
        //friend class ::address_space;

        //using NativeType = typename emu::detail::handler_entry_size<Width>::uX;


        static readonly int Level = new int_Level().value;
        static readonly int Width = new int_Width().value;
        static readonly int AddrShift = new int_AddrShift().value;


        static readonly u32 NATIVE_BYTES = 1U << Width;  //static constexpr u32 NATIVE_BYTES = 1 << Width;
        static readonly u32 NATIVE_MASK = Width + AddrShift >= 0 ? (1U << (Width + AddrShift)) - 1 : 0U;  //static constexpr u32 NATIVE_MASK = Width + AddrShift >= 0 ? (1 << (Width + AddrShift)) - 1 : 0;


        address_space m_space;

        offs_t m_addrmask;                // address mask

        Pointer<handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian>> m_dispatch_read;  //const handler_entry_read<Width, AddrShift, Endian> *const *m_dispatch_read;
        Pointer<handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian>> m_dispatch_write;  //const handler_entry_write<Width, AddrShift, Endian> *const *m_dispatch_write;


        // construction/destruction
        public memory_access_specific()
        {
            m_space = null;
            m_addrmask = 0;
            m_dispatch_read = null;
            m_dispatch_write = null;
        }


        //inline address_space &space() const {
        //    return *m_space;
        //}


        public u8 read_byte(offs_t address) { return Width == 0 ? read_native(address & ~NATIVE_MASK).x8 : emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_constant_0, bool_constant_true>((offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(0, 0xff)).x8; }
        public u16 read_word(offs_t address) { return Width == 1 ? read_native(address & ~NATIVE_MASK).x16 : emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_constant_1, bool_constant_true>((offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(1, 0xffff)).x16; }
        //u16 read_word(offs_t address, u16 mask) { return memory_read_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u16 read_word_unaligned(offs_t address) { return memory_read_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffff); }
        //u16 read_word_unaligned(offs_t address, u16 mask) { return memory_read_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u32 read_dword(offs_t address) { return Width == 2 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffff); }
        //u32 read_dword(offs_t address, u32 mask) { return memory_read_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u32 read_dword_unaligned(offs_t address) { return memory_read_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffff); }
        //u32 read_dword_unaligned(offs_t address, u32 mask) { return memory_read_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u64 read_qword(offs_t address) { return Width == 3 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffffffffffffU); }
        //u64 read_qword(offs_t address, u64 mask) { return memory_read_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u64 read_qword_unaligned(offs_t address) { return memory_read_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffffffffffffU); }
        //u64 read_qword_unaligned(offs_t address, u64 mask) { return memory_read_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }

        public void write_byte(offs_t address, u8 data) { if (Width == 0) write_native(address & ~NATIVE_MASK, new uX(0, data)); else emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_constant_0, bool_constant_true>((offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(0, data), new uX(0, 0xff)); }
        public void write_word(offs_t address, u16 data) { if (Width == 1) write_native(address & ~NATIVE_MASK, new uX(1, data)); else emumem_global.memory_write_generic<int_Width, int_AddrShift, endianness_t_Endian, int_constant_1, bool_constant_true>((offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(1, data), new uX(1, 0xffff)); }
        //void write_word(offs_t address, u16 data, u16 mask) { memory_write_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_word_unaligned(offs_t address, u16 data) { memory_write_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffff); }
        //void write_word_unaligned(offs_t address, u16 data, u16 mask) { memory_write_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_dword(offs_t address, u32 data) { if (Width == 2) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffff); }
        //void write_dword(offs_t address, u32 data, u32 mask) { memory_write_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_dword_unaligned(offs_t address, u32 data) { memory_write_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffff); }
        //void write_dword_unaligned(offs_t address, u32 data, u32 mask) { memory_write_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_qword(offs_t address, u64 data) { if (Width == 3) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffffffffffffU); }
        //void write_qword(offs_t address, u64 data, u64 mask) { memory_write_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_qword_unaligned(offs_t address, u64 data) { memory_write_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffffffffffffU); }
        //void write_qword_unaligned(offs_t address, u64 data, u64 mask) { memory_write_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }


        uX read_native(offs_t address) { return read_native(address, ~new uX(Width, 0)); }
        uX read_native(offs_t address, uX mask)  //NativeType read_native(offs_t address, NativeType mask = ~NativeType(0)) {
        {
            return emumem_global.dispatch_read<int_Level, int_Width, int_AddrShift, endianness_t_Endian>(offs_t.MaxValue, address & m_addrmask, mask, m_dispatch_read);  //return dispatch_read<Level, Width, AddrShift, Endian>(offs_t(-1), address & m_addrmask, mask, m_dispatch_read);;
        }


        void write_native(offs_t address, uX data) { write_native(address, data, ~new uX(Width, 0)); }
        void write_native(offs_t address, uX data, uX mask)  //void write_native(offs_t address, NativeType data, NativeType mask = ~NativeType(0)) {
        {
            emumem_global.dispatch_write<int_Level, int_Width, int_AddrShift, endianness_t_Endian>(offs_t.MaxValue, address & m_addrmask, data, mask, m_dispatch_write);  //dispatch_write<Level, Width, AddrShift, Endian>(offs_t(-1), address & m_addrmask, data, mask, m_dispatch_write);;
        }


        public void set(address_space space, std.pair<object, object> rw)
        {
            m_space = space;
            m_addrmask = space.addrmask();
            m_dispatch_read  = (Pointer<handler_entry_read <int_Width, int_AddrShift, endianness_t_Endian>>)rw.first;
            m_dispatch_write = (Pointer<handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian>>)rw.second;
        }
    }


    // ======================> memory_access_cache
    // memory_access_cache contains state data for cached access
    //template<int Width, int AddrShift, endianness_t Endian>
    public class memory_access_cache<int_Width, int_AddrShift, endianness_t_Endian>
        where int_Width : int_constant, new()
        where int_AddrShift : int_constant, new()
        where endianness_t_Endian : endianness_t_constant, new()
    {
        //friend class ::address_space;

        //using NativeType = typename emu::detail::handler_entry_size<Width>::uX;


        static readonly int Width = new int_Width().value;
        static readonly int AddrShift = new int_AddrShift().value;


        static readonly u32 NATIVE_BYTES = 1U << Width;  //static constexpr u32 NATIVE_BYTES = 1 << Width;
        static readonly u32 NATIVE_MASK = Width + AddrShift >= 0 ? (1U << (Width + AddrShift)) - 1 : 0U;  //static constexpr u32 NATIVE_MASK = Width + AddrShift >= 0 ? (1 << (Width + AddrShift)) - 1 : 0;


        address_space m_space;

        offs_t m_addrmask;                // address mask
        offs_t m_addrstart_r;             // minimum valid address for reading
        offs_t m_addrend_r;               // maximum valid address for reading
        offs_t m_addrstart_w;             // minimum valid address for writing
        offs_t m_addrend_w;               // maximum valid address for writing
        handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian> m_cache_r;  // read cache
        handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> m_cache_w;  // write cache

        handler_entry_read<int_Width, int_AddrShift, endianness_t_Endian> m_root_read;  // decode tree roots
        handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian> m_root_write;


        // construction/destruction
        public memory_access_cache()
        {
            m_space = null;
            m_addrmask = 0;
            m_addrstart_r = 1;
            m_addrend_r = 0;
            m_addrstart_w = 1;
            m_addrend_w = 0;
            m_cache_r = null;
            m_cache_w = null;
            m_root_read = null;
            m_root_write = null;
        }

        //~memory_access_cache();


        // see if an address is within bounds, update it if not
        void check_address_r(offs_t address)
        {
            if (address >= m_addrstart_r && address <= m_addrend_r)
                return;
            m_root_read.lookup(address, ref m_addrstart_r, ref m_addrend_r, ref m_cache_r);
        }


        //void check_address_w(offs_t address) {
        //    if(address >= m_addrstart_w && address <= m_addrend_w)
        //        return;
        //    m_root_write->lookup(address, m_addrstart_w, m_addrend_w, m_cache_w);
        //}

        // accessor methods

        //inline address_space &space() const {
        //    return *m_space;
        //}

        //void *read_ptr(offs_t address) {
        //    address &= m_addrmask;
        //    check_address_r(address);
        //    return m_cache_r->get_ptr(address);
        //}

        public u8 read_byte(offs_t address) { return Width == 0 ? read_native(address & ~NATIVE_MASK).x8 : emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_constant_0, bool_constant_true>((offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(0, 0xff)).x8; }
        public u16 read_word(offs_t address) { return Width == 1 ? read_native(address & ~NATIVE_MASK).x16 : emumem_global.memory_read_generic<int_Width, int_AddrShift, endianness_t_Endian, int_constant_1, bool_constant_true>((offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(1, 0xffff)).x16; }
        //u16 read_word(offs_t address, u16 mask) { return memory_read_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u16 read_word_unaligned(offs_t address) { return memory_read_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffff); }
        //u16 read_word_unaligned(offs_t address, u16 mask) { return memory_read_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u32 read_dword(offs_t address) { return Width == 2 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffff); }
        //u32 read_dword(offs_t address, u32 mask) { return memory_read_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u32 read_dword_unaligned(offs_t address) { return memory_read_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffff); }
        //u32 read_dword_unaligned(offs_t address, u32 mask) { return memory_read_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u64 read_qword(offs_t address) { return Width == 3 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffffffffffffU); }
        //u64 read_qword(offs_t address, u64 mask) { return memory_read_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u64 read_qword_unaligned(offs_t address) { return memory_read_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffffffffffffU); }
        //u64 read_qword_unaligned(offs_t address, u64 mask) { return memory_read_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }

        //void write_byte(offs_t address, u8 data) { if (Width == 0) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 0, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xff); }
        //void write_word(offs_t address, u16 data) { if (Width == 1) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffff); }
        //void write_word(offs_t address, u16 data, u16 mask) { memory_write_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_word_unaligned(offs_t address, u16 data) { memory_write_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffff); }
        //void write_word_unaligned(offs_t address, u16 data, u16 mask) { memory_write_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_dword(offs_t address, u32 data) { if (Width == 2) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffff); }
        //void write_dword(offs_t address, u32 data, u32 mask) { memory_write_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_dword_unaligned(offs_t address, u32 data) { memory_write_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffff); }
        //void write_dword_unaligned(offs_t address, u32 data, u32 mask) { memory_write_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_qword(offs_t address, u64 data) { if (Width == 3) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffffffffffffU); }
        //void write_qword(offs_t address, u64 data, u64 mask) { memory_write_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_qword_unaligned(offs_t address, u64 data) { memory_write_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffffffffffffU); }
        //void write_qword_unaligned(offs_t address, u64 data, u64 mask) { memory_write_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }


        uX read_native(offs_t address) { return read_native(address, ~new uX(Width, 0)); }
        uX read_native(offs_t address, uX mask)  //NativeType read_native(offs_t address, NativeType mask = ~NativeType(0));
        {
            address &= m_addrmask;
            check_address_r(address);
            return m_cache_r.read(address, mask);
        }


        //void write_native(offs_t address, NativeType data, NativeType mask = ~NativeType(0));


        public void set(address_space space, std.pair<object, object> rw)
        {
            m_space = space;
            m_addrmask = space.addrmask();

            space.add_change_notifier((read_or_write mode) =>
            {
                if (((u32)mode & (u32)read_or_write.READ) != 0)
                {
                    m_addrend_r = 0;
                    m_addrstart_r = 1;
                    m_cache_r = null;
                }

                if (((u32)mode & (u32)read_or_write.WRITE) != 0)
                {
                    m_addrend_w = 0;
                    m_addrstart_w = 1;
                    m_cache_w = null;
                }
            });
            m_root_read  = (handler_entry_read <int_Width, int_AddrShift, endianness_t_Endian>)rw.first;
            m_root_write = (handler_entry_write<int_Width, int_AddrShift, endianness_t_Endian>)rw.second;

            // Protect against a wandering memset
            m_addrstart_r = 1;
            m_addrend_r = 0;
            m_cache_r = null;
            m_addrstart_w = 1;
            m_addrend_w = 0;
            m_cache_w = null;
        }
    }

    }


    // ======================> memory_access cache/specific type dispatcher
    //template<int HighBits, int Width, int AddrShift, endianness_t Endian>
    struct memory_access<int_HighBits, int_Width, int_AddrShift, endianness_t_Endian>
        where int_HighBits : int_constant, new()
        where int_Width : int_constant, new()
        where int_AddrShift : int_constant, new()
        where endianness_t_Endian : endianness_t_constant, new()
    {
        static readonly int HighBits = new int_HighBits().value;


        //static constexpr int Level = emu::detail::handler_entry_dispatch_level(HighBits);
        public class int_Level : int_constant { public int value { get { return emumem_global.handler_entry_dispatch_level(HighBits); } } }


        //using cache = emu::detail::memory_access_cache<Width, AddrShift, Endian>;
        public class cache : emu.detail.memory_access_cache<int_Width, int_AddrShift, endianness_t_Endian>
        {
            //public cache(int Width, int AddrShift, endianness_t Endian) : base(Width, AddrShift, Endian) { }
        }
        //public cache m_cache;

        //using specific = emu::detail::memory_access_specific<Level, Width, AddrShift, Endian>;
        public class specific : emu.detail.memory_access_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian>
        {
            //public specific(int Level, int Width, int AddrShift, endianness_t Endian) : base(Level, Width, AddrShift, Endian) { }
        }
        //public specific m_specific;


        //public memory_access(int HighBits, int Width, int AddrShift, endianness_t Endian)
        //{
        //    Level = mame.emumem_global.handler_entry_dispatch_level(HighBits);
        //
        //
        //    m_cache = new cache(Width, AddrShift, Endian);
        //    m_specific = new specific(Level, Width, AddrShift, Endian);
        //}
    }


    //template class memory_access_cache<0,  1, ENDIANNESS_LITTLE>;
    //template class memory_access_cache<0,  1, ENDIANNESS_BIG>;
    //template class memory_access_cache<0,  0, ENDIANNESS_LITTLE>;
    //template class memory_access_cache<0,  0, ENDIANNESS_BIG>;
    //template class memory_access_cache<1,  3, ENDIANNESS_LITTLE>;
    //template class memory_access_cache<1,  3, ENDIANNESS_BIG>;
    //template class memory_access_cache<1,  0, ENDIANNESS_LITTLE>;
    //template class memory_access_cache<1,  0, ENDIANNESS_BIG>;
    //template class memory_access_cache<1, -1, ENDIANNESS_LITTLE>;
    //template class memory_access_cache<1, -1, ENDIANNESS_BIG>;
    //template class memory_access_cache<2,  0, ENDIANNESS_LITTLE>;
    //template class memory_access_cache<2,  0, ENDIANNESS_BIG>;
    //template class memory_access_cache<2, -1, ENDIANNESS_LITTLE>;
    //template class memory_access_cache<2, -1, ENDIANNESS_BIG>;
    //template class memory_access_cache<2, -2, ENDIANNESS_LITTLE>;
    //template class memory_access_cache<2, -2, ENDIANNESS_BIG>;
    //template class memory_access_cache<3,  0, ENDIANNESS_LITTLE>;
    //template class memory_access_cache<3,  0, ENDIANNESS_BIG>;
    //template class memory_access_cache<3, -1, ENDIANNESS_LITTLE>;
    //template class memory_access_cache<3, -1, ENDIANNESS_BIG>;
    //template class memory_access_cache<3, -2, ENDIANNESS_LITTLE>;
    //template class memory_access_cache<3, -2, ENDIANNESS_BIG>;
    //template class memory_access_cache<3, -3, ENDIANNESS_LITTLE>;
    //template class memory_access_cache<3, -3, ENDIANNESS_BIG>;


    // ======================> address_space_config
    // describes an address space and provides basic functions to map addresses to bytes
    public class address_space_config : global_object
    {
        //friend class address_map;


        // state (TODO: privatize)
        string m_name;
        endianness_t m_endianness;
        u8 m_data_width;
        u8 m_addr_width;
        s8 m_addr_shift;
        u8 m_logaddr_width;
        u8 m_page_shift;
        public bool m_is_octal;                 // to determine if messages/debugger will show octal or hex

        public address_map_constructor m_internal_map;


        // construction/destruction

        //-------------------------------------------------
        //  address_space_config - constructors
        //-------------------------------------------------
        public address_space_config()
        {
            m_name = "unknown";
            m_endianness = ENDIANNESS_NATIVE;
            m_data_width = 0;
            m_addr_width = 0;
            m_addr_shift = 0;
            m_logaddr_width = 0;
            m_page_shift = 0;
            m_is_octal = false;
            m_internal_map = null;
        }

        public address_space_config(string name, endianness_t endian, u8 datawidth, u8 addrwidth, s8 addrshift = 0, address_map_constructor internal_ = null)
        {
            m_name = name;
            m_endianness = endian;
            m_data_width = datawidth;
            m_addr_width = addrwidth;
            m_addr_shift = addrshift;
            m_logaddr_width = addrwidth;
            m_page_shift = 0;
            m_is_octal = false;
            m_internal_map = internal_;
        }

        public address_space_config(string name, endianness_t endian, u8 datawidth, u8 addrwidth, s8 addrshift, u8 logwidth, u8 pageshift, address_map_constructor internal_ = null)
        {
            m_name = name;
            m_endianness = endian;
            m_data_width = datawidth;
            m_addr_width = addrwidth;
            m_addr_shift = addrshift;
            m_logaddr_width = logwidth;
            m_page_shift = pageshift;
            m_is_octal = false;
            m_internal_map = internal_;
        }


        public void CopyTo(address_space_config config)
        {
            config.m_name = m_name;
            config.m_endianness = m_endianness;
            config.m_data_width = m_data_width;
            config.m_addr_width = m_addr_width;
            config.m_addr_shift = m_addr_shift;
            config.m_logaddr_width = m_logaddr_width;
            config.m_page_shift = m_page_shift;
            config.m_is_octal = m_is_octal;
            config.m_internal_map = m_internal_map;
        }


        // getters
        public string name() { return m_name; }
        public endianness_t endianness() { return m_endianness; }
        public int data_width() { return m_data_width; }
        public int addr_width() { return m_addr_width; }
        public int addr_shift() { return m_addr_shift; }
        public byte logaddr_width() { return m_logaddr_width; }
        //int page_shift() const { return m_page_shift; }
        public bool is_octal() { return m_is_octal; }


        // Actual alignment of the bus addresses
        public int alignment() { int bytes = m_data_width / 8; return m_addr_shift < 0 ? bytes >> -m_addr_shift : bytes << m_addr_shift; }

        // Address delta to byte delta helpers
        public offs_t addr2byte(offs_t address) { return m_addr_shift < 0 ? (address << -m_addr_shift) : (address >> m_addr_shift); }
        public offs_t byte2addr(offs_t address) { return m_addr_shift > 0 ? (address << m_addr_shift) : (address >> -m_addr_shift); }

        // address-to-byte conversion helpers
        public offs_t addr2byte_end(offs_t address) { return m_addr_shift < 0 ? (UInt32)((address << -m_addr_shift) | ((1 << -m_addr_shift) - 1)) : (address >> m_addr_shift); }
        //offs_t byte2addr_end(offs_t address) { return (m_addrbus_shift > 0) ? ((address << m_addrbus_shift) | ((1 << m_addrbus_shift) - 1)) : (address >> -m_addrbus_shift); }
    }


    // ======================> address_space
    public abstract class address_space_installer : global_object
    {
        protected address_space_config m_config;       // configuration of this space
        public memory_manager m_manager;          // reference to the owning manager
        protected offs_t m_addrmask;         // physical address mask
        offs_t m_logaddrmask;      // logical address mask
        protected u8 m_addrchars;        // number of characters to use for physical addresses
        u8 m_logaddrchars;     // number of characters to use for logical addresses


        protected address_space_installer(address_space_config config, memory_manager manager)
        {
            m_config = config;
            m_manager = manager;
            m_addrmask = g.make_bitmask32(m_config.addr_width());
            m_logaddrmask = g.make_bitmask32(m_config.logaddr_width());
            m_addrchars = (u8)((m_config.addr_width() + 3) / 4);
            m_logaddrchars = (u8)((m_config.logaddr_width() + 3) / 4);
        }


        //const address_space_config &space_config() const { return m_config; }
        public int data_width() { return m_config.data_width(); }
        public int addr_width() { return m_config.addr_width(); }
        //int logaddr_width() const { return m_config.logaddr_width(); }
        //int alignment() const { return m_config.alignment(); }
        public endianness_t endianness() { return m_config.endianness(); }
        protected int addr_shift() { return m_config.addr_shift(); }
        public bool is_octal() { return m_config.is_octal(); }

        // address-to-byte conversion helpers
        protected offs_t address_to_byte(offs_t address) { return m_config.addr2byte(address); }
        //offs_t address_to_byte_end(offs_t address) const { return m_config.addr2byte_end(address); }
        //offs_t byte_to_address(offs_t address) const { return m_config.byte2addr(address); }
        //offs_t byte_to_address_end(offs_t address) const { return m_config.byte2addr_end(address); }

        public offs_t addrmask() { return m_addrmask; }
        public u8 addrchars() { return m_addrchars; }
        //offs_t logaddrmask() const { return m_logaddrmask; }
        public u8 logaddrchars() { return m_logaddrchars; }

        // unmap ranges (short form)
        //void unmap_read(offs_t addrstart, offs_t addrend, offs_t addrmirror = 0) { unmap_generic(addrstart, addrend, addrmirror, read_or_write::READ, false); }
        //void unmap_write(offs_t addrstart, offs_t addrend, offs_t addrmirror = 0) { unmap_generic(addrstart, addrend, addrmirror, read_or_write::WRITE, false); }
        //void unmap_readwrite(offs_t addrstart, offs_t addrend, offs_t addrmirror = 0) { unmap_generic(addrstart, addrend, addrmirror, read_or_write::READWRITE, false); }
        //void nop_read(offs_t addrstart, offs_t addrend, offs_t addrmirror = 0) { unmap_generic(addrstart, addrend, addrmirror, read_or_write::READ, true); }
        //void nop_write(offs_t addrstart, offs_t addrend, offs_t addrmirror = 0) { unmap_generic(addrstart, addrend, addrmirror, read_or_write::WRITE, true); }
        //void nop_readwrite(offs_t addrstart, offs_t addrend, offs_t addrmirror = 0) { unmap_generic(addrstart, addrend, addrmirror, read_or_write::READWRITE, true); }

        // install ports, banks, RAM (short form)
        //void install_read_port(offs_t addrstart, offs_t addrend, const char *rtag) { install_read_port(addrstart, addrend, 0, rtag); }
        //void install_write_port(offs_t addrstart, offs_t addrend, const char *wtag) { install_write_port(addrstart, addrend, 0, wtag); }
        //void install_readwrite_port(offs_t addrstart, offs_t addrend, const char *rtag, const char *wtag) { install_readwrite_port(addrstart, addrend, 0, rtag, wtag); }
        //void install_read_bank(offs_t addrstart, offs_t addrend, memory_bank *bank) { install_read_bank(addrstart, addrend, 0, bank); }
        //void install_write_bank(offs_t addrstart, offs_t addrend, memory_bank *bank) { install_write_bank(addrstart, addrend, 0, bank); }
        //void install_readwrite_bank(offs_t addrstart, offs_t addrend, memory_bank *bank) { install_readwrite_bank(addrstart, addrend, 0, bank); }
        //void install_rom(offs_t addrstart, offs_t addrend, void *baseptr) { install_rom(addrstart, addrend, 0, baseptr); }
        //void install_writeonly(offs_t addrstart, offs_t addrend, void *baseptr) { install_writeonly(addrstart, addrend, 0, baseptr); }
        //void install_ram(offs_t addrstart, offs_t addrend, void *baseptr) { install_ram(addrstart, addrend, 0, baseptr); }

        // install ports, banks, RAM (with mirror/mask)
        //void install_read_port(offs_t addrstart, offs_t addrend, offs_t addrmirror, const char *rtag) { install_readwrite_port(addrstart, addrend, addrmirror, rtag, ""); }
        //void install_write_port(offs_t addrstart, offs_t addrend, offs_t addrmirror, const char *wtag) { install_readwrite_port(addrstart, addrend, addrmirror, "", wtag); }
        protected abstract void install_readwrite_port(offs_t addrstart, offs_t addrend, offs_t addrmirror, string rtag, string wtag);
        //void install_read_bank(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_bank *bank) { install_bank_generic(addrstart, addrend, addrmirror, bank, nullptr); }
        //void install_write_bank(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_bank *bank) { install_bank_generic(addrstart, addrend, addrmirror, nullptr, bank); }
        //void install_readwrite_bank(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_bank *bank)  { install_bank_generic(addrstart, addrend, addrmirror, bank, bank); }
        //void install_rom(offs_t addrstart, offs_t addrend, offs_t addrmirror, void *baseptr) { install_ram_generic(addrstart, addrend, addrmirror, read_or_write::READ, baseptr); }
        //void install_writeonly(offs_t addrstart, offs_t addrend, offs_t addrmirror, void *baseptr) { install_ram_generic(addrstart, addrend, addrmirror, read_or_write::WRITE, baseptr); }
        //void install_ram(offs_t addrstart, offs_t addrend, offs_t addrmirror, void *baseptr) { install_ram_generic(addrstart, addrend, addrmirror, read_or_write::READWRITE, baseptr); }

        // install device memory maps
        //template <typename T> void install_device(offs_t addrstart, offs_t addrend, T &device, void (T::*map)(address_map &map), u64 unitmask = 0, int cswidth = 0) {
        //    address_map_constructor delegate(map, "dynamic_device_install", &device);
        //    install_device_delegate(addrstart, addrend, device, delegate, unitmask, cswidth);
        //}


        protected abstract void install_device_delegate(offs_t addrstart, offs_t addrend, device_t device, address_map_constructor map, u64 unitmask = 0, int cswidth = 0);


        // install taps without mirroring
        //memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, std::string name, std::function<void (offs_t offset, u8  &data, u8  mem_mask)> tap, memory_passthrough_handler *mph = nullptr) { return install_read_tap(addrstart, addrend, 0, name, tap, mph); }
        //memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, std::string name, std::function<void (offs_t offset, u16 &data, u16 mem_mask)> tap, memory_passthrough_handler *mph = nullptr) { return install_read_tap(addrstart, addrend, 0, name, tap, mph); }
        //memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, std::string name, std::function<void (offs_t offset, u32 &data, u32 mem_mask)> tap, memory_passthrough_handler *mph = nullptr) { return install_read_tap(addrstart, addrend, 0, name, tap, mph); }
        //memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, std::string name, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tap, memory_passthrough_handler *mph = nullptr) { return install_read_tap(addrstart, addrend, 0, name, tap, mph); }
        //memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, std::string name, std::function<void (offs_t offset, u8  &data, u8  mem_mask)> tap, memory_passthrough_handler *mph = nullptr) { return install_write_tap(addrstart, addrend, 0, name, tap, mph); }
        //memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, std::string name, std::function<void (offs_t offset, u16 &data, u16 mem_mask)> tap, memory_passthrough_handler *mph = nullptr) { return install_write_tap(addrstart, addrend, 0, name, tap, mph); }
        //memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, std::string name, std::function<void (offs_t offset, u32 &data, u32 mem_mask)> tap, memory_passthrough_handler *mph = nullptr) { return install_write_tap(addrstart, addrend, 0, name, tap, mph); }
        //memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, std::string name, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tap, memory_passthrough_handler *mph = nullptr) { return install_write_tap(addrstart, addrend, 0, name, tap, mph); }
        //memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, std::string name, std::function<void (offs_t offset, u8  &data, u8  mem_mask)> tapr, std::function<void (offs_t offset, u8  &data, u8  mem_mask)> tapw, memory_passthrough_handler *mph = nullptr) { return install_readwrite_tap(addrstart, addrend, 0, name, tapr, tapw, mph); }
        //memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, std::string name, std::function<void (offs_t offset, u16 &data, u16 mem_mask)> tapr, std::function<void (offs_t offset, u16 &data, u16 mem_mask)> tapw, memory_passthrough_handler *mph = nullptr) { return install_readwrite_tap(addrstart, addrend, 0, name, tapr, tapw, mph); }
        //memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, std::string name, std::function<void (offs_t offset, u32 &data, u32 mem_mask)> tapr, std::function<void (offs_t offset, u32 &data, u32 mem_mask)> tapw, memory_passthrough_handler *mph = nullptr) { return install_readwrite_tap(addrstart, addrend, 0, name, tapr, tapw, mph); }
        //memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, std::string name, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tapr, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tapw, memory_passthrough_handler *mph = nullptr) { return install_readwrite_tap(addrstart, addrend, 0, name, tapr, tapw, mph); }


        public delegate void install_tap_func<T>(offs_t offset, ref T data, T mem_mask);

        // install taps with mirroring
        protected virtual memory_passthrough_handler install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<u8> tap, memory_passthrough_handler mph = null)   //virtual memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u8  &data, u8  mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 8-bits wide bus read tap in a {0}-bits wide bus\n", data_width()); return null; }
        protected virtual memory_passthrough_handler install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<u16> tap, memory_passthrough_handler mph = null)   //virtual memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u16 &data, u16 mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 16-bits wide bus read tap in a {0}-bits wide bus\n", data_width()); return null; }
        protected virtual memory_passthrough_handler install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<u32> tap, memory_passthrough_handler mph = null)   //virtual memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u32 &data, u32 mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 32-bits wide bus read tap in a {0}-bits wide bus\n", data_width()); return null; }
        protected virtual memory_passthrough_handler install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<u64> tap, memory_passthrough_handler mph = null)   //virtual memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 64-bits wide bus read tap in a {0}-bits wide bus\n", data_width()); return null; }
        protected virtual memory_passthrough_handler install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<u8> tap, memory_passthrough_handler mph = null)  //virtual memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u8  &data, u8  mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 8-bits wide bus write tap in a {0}-bits wide bus\n", data_width()); return null; }
        protected virtual memory_passthrough_handler install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<u16> tap, memory_passthrough_handler mph = null)  //virtual memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u16 &data, u16 mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 16-bits wide bus write tap in a {0}-bits wide bus\n", data_width()); return null; }
        protected virtual memory_passthrough_handler install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<u32> tap, memory_passthrough_handler mph = null)  //virtual memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u32 &data, u32 mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 32-bits wide bus write tap in a {0}-bits wide bus\n", data_width()); return null; }
        protected virtual memory_passthrough_handler install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<u64> tap, memory_passthrough_handler mph = null)  //virtual memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 64-bits wide bus write tap in a {0}-bits wide bus\n", data_width()); return null; }
        public virtual memory_passthrough_handler install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<u8> tapr, install_tap_func<u8> tapw, memory_passthrough_handler mph = null)  //virtual memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u8  &data, u8  mem_mask)> tapr, std::function<void (offs_t offset, u8  &data, u8  mem_mask)> tapw, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 8-bits wide bus read/write tap in a {0}-bits wide bus\n", data_width()); return null; }
        public virtual memory_passthrough_handler install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<u16> tapr, install_tap_func<u16> tapw, memory_passthrough_handler mph = null)  //virtual memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u16 &data, u16 mem_mask)> tapr, std::function<void (offs_t offset, u16 &data, u16 mem_mask)> tapw, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 16-bits wide bus read/write tap in a {0}-bits wide bus\n", data_width()); return null; }
        protected virtual memory_passthrough_handler install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<u32> tapr, install_tap_func<u32> tapw, memory_passthrough_handler mph = null)  //virtual memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u32 &data, u32 mem_mask)> tapr, std::function<void (offs_t offset, u32 &data, u32 mem_mask)> tapw, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 32-bits wide bus read/write tap in a {0}-bits wide bus\n", data_width()); return null; }
        protected virtual memory_passthrough_handler install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<u64> tapr, install_tap_func<u64> tapw, memory_passthrough_handler mph = null)  //virtual memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tapr, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tapw, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 64-bits wide bus read/write tap in a {0}-bits wide bus\n", data_width()); return null; }

        protected virtual memory_passthrough_handler install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<uX> tap, memory_passthrough_handler mph = null)   //virtual memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u8  &data, u8  mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 8-bits wide bus read tap in a {0}-bits wide bus\n", data_width()); return null; }
        protected virtual memory_passthrough_handler install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<uX> tap, memory_passthrough_handler mph = null)  //virtual memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 64-bits wide bus write tap in a {0}-bits wide bus\n", data_width()); return null; }
        protected virtual memory_passthrough_handler install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, string name, install_tap_func<uX> tapr, install_tap_func<uX> tapw, memory_passthrough_handler mph = null)  //virtual memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tapr, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tapw, memory_passthrough_handler *mph = nullptr);
        { global_object.fatalerror("Trying to install a 64-bits wide bus read/write tap in a {0}-bits wide bus\n", data_width()); return null; }


        // install views
        //void install_view(offs_t addrstart, offs_t addrend, memory_view &view) { install_view(addrstart, addrend, 0, view); }
        protected abstract void install_view(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_view view);


        // install new-style delegate handlers (short form)
        //void install_read_handler(offs_t addrstart, offs_t addrend, read8_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read8_delegate rhandler, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read16_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write16_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read16_delegate rhandler, write16_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read32_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write32_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read32_delegate rhandler, write32_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read64_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write64_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read64_delegate rhandler, write64_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }

        //void install_read_handler(offs_t addrstart, offs_t addrend, read8m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read8m_delegate rhandler, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read16m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read16m_delegate rhandler, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read32m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read32m_delegate rhandler, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read64m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read64m_delegate rhandler, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }

        //void install_read_handler(offs_t addrstart, offs_t addrend, read8s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read8s_delegate rhandler, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read16s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read16s_delegate rhandler, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read32s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read32s_delegate rhandler, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read64s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read64s_delegate rhandler, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }

        //void install_read_handler(offs_t addrstart, offs_t addrend, read8sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read8sm_delegate rhandler, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read16sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read16sm_delegate rhandler, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read32sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read32sm_delegate rhandler, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read64sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read64sm_delegate rhandler, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }

        //void install_read_handler(offs_t addrstart, offs_t addrend, read8mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read8mo_delegate rhandler, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read16mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read16mo_delegate rhandler, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read32mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read32mo_delegate rhandler, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read64mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read64mo_delegate rhandler, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }

        //void install_read_handler(offs_t addrstart, offs_t addrend, read8smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read8smo_delegate rhandler, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read16smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read16smo_delegate rhandler, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read32smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read32smo_delegate rhandler, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read64smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read64smo_delegate rhandler, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }

        // install new-style delegate handlers (with mirror/mask)
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8_delegate rhandler, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16_delegate rhandler, write16_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32_delegate rhandler, write32_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64_delegate rhandler, write64_delegate whandler, u64 unitmask = 0, int cswidth = 0);

        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8m_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8m_delegate rhandler, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16m_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16m_delegate rhandler, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32m_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32m_delegate rhandler, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64m_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64m_delegate rhandler, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0);

        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0);

        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16sm_delegate rhandler, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32sm_delegate rhandler, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64sm_delegate rhandler, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0);

        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8mo_delegate rhandler, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16mo_delegate rhandler, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32mo_delegate rhandler, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64mo_delegate rhandler, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0);

        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32smo_delegate rhandler, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64smo_delegate rhandler, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0);


        protected abstract void unmap_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, bool quiet);
        protected abstract void install_ram_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, PointerU8 baseptr);  //virtual void install_ram_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, void *baseptr) = 0;
        protected abstract void install_bank_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_bank rbank, memory_bank wbank);  //virtual void install_bank_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_bank *rbank, memory_bank *wbank) = 0;


        //-------------------------------------------------
        //  populate_map_entry - map a single read or
        //  write entry based on information from an
        //  address map entry
        //-------------------------------------------------
        protected void populate_map_entry(address_map_entry entry, read_or_write readorwrite)
        {
            map_handler_data data = (readorwrite == read_or_write.READ) ? entry.m_read : entry.m_write;
            // based on the handler type, alter the bits, name, funcptr, and object
            switch (data.m_type)
            {
                case map_handler_type.AMH_NONE:
                    return;

                case map_handler_type.AMH_ROM:
                    // writes to ROM are no-ops
                    if (readorwrite == read_or_write.WRITE)
                        return;
                    // fall through to the RAM case otherwise
                    goto case map_handler_type.AMH_RAM;  //[[fallthrough]];

                case map_handler_type.AMH_RAM:
                    install_ram_generic(entry.m_addrstart, entry.m_addrend, entry.m_addrmirror, readorwrite, entry.m_memory);
                    break;

                case map_handler_type.AMH_NOP:
                    unmap_generic(entry.m_addrstart, entry.m_addrend, entry.m_addrmirror, readorwrite, true);
                    break;

                case map_handler_type.AMH_UNMAP:
                    unmap_generic(entry.m_addrstart, entry.m_addrend, entry.m_addrmirror, readorwrite, false);
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE:
                    if (readorwrite == read_or_write.READ)
                    {
                        switch (data.m_bits)
                        {
                            case 8:     install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto8, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto16, entry.m_mask, entry.m_cswidth); break;
                            case 32:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto32, entry.m_mask, entry.m_cswidth); break;
                            case 64:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto64, entry.m_mask, entry.m_cswidth); break;
                        }
                    }
                    else
                    {
                        switch (data.m_bits)
                        {
                            case 8:     install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto8, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto16, entry.m_mask, entry.m_cswidth); break;
                            case 32:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto32, entry.m_mask, entry.m_cswidth); break;
                            case 64:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto64, entry.m_mask, entry.m_cswidth); break;
                        }
                    }
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE_M:
                    if (readorwrite == read_or_write.READ)
                    {
                        switch (data.m_bits)
                        {
                            case 8:     install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto8m, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto16m, entry.m_mask, entry.m_cswidth); break;
                            case 32:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto32m, entry.m_mask, entry.m_cswidth); break;
                            case 64:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto64m, entry.m_mask, entry.m_cswidth); break;
                        }
                    }
                    else
                    {
                        switch (data.m_bits)
                        {
                            case 8:     install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto8m, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto16m, entry.m_mask, entry.m_cswidth); break;
                            case 32:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto32m, entry.m_mask, entry.m_cswidth); break;
                            case 64:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto64m, entry.m_mask, entry.m_cswidth); break;
                        }
                    }
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE_S:
                    if (readorwrite == read_or_write.READ)
                    {
                        switch (data.m_bits)
                        {
                            case 8:     install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto8s, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto16s, entry.m_mask, entry.m_cswidth); break;
                            case 32:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto32s, entry.m_mask, entry.m_cswidth); break;
                            case 64:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto64s, entry.m_mask, entry.m_cswidth); break;
                        }
                    }
                    else
                    {
                        switch (data.m_bits)
                        {
                            case 8:     install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto8s, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto16s, entry.m_mask, entry.m_cswidth); break;
                            case 32:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto32s, entry.m_mask, entry.m_cswidth); break;
                            case 64:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto64s, entry.m_mask, entry.m_cswidth); break;
                        }
                    }
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE_SM:
                    if (readorwrite == read_or_write.READ)
                    {
                        switch (data.m_bits)
                        {
                            case 8:     install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto8sm, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto16sm, entry.m_mask, entry.m_cswidth); break;
                            case 32:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto32sm, entry.m_mask, entry.m_cswidth); break;
                            case 64:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto64sm, entry.m_mask, entry.m_cswidth); break;
                        }
                    }
                    else
                    {
                        switch (data.m_bits)
                        {
                            case 8:     install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto8sm, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto16sm, entry.m_mask, entry.m_cswidth); break;
                            case 32:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto32sm, entry.m_mask, entry.m_cswidth); break;
                            case 64:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto64sm, entry.m_mask, entry.m_cswidth); break;
                        }
                    }
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE_MO:
                    if (readorwrite == read_or_write.READ)
                    {
                        switch (data.m_bits)
                        {
                            case 8:     install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto8mo, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto16mo, entry.m_mask, entry.m_cswidth); break;
                            case 32:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto32mo, entry.m_mask, entry.m_cswidth); break;
                            case 64:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto64mo, entry.m_mask, entry.m_cswidth); break;
                        }
                    }
                    else
                    {
                        switch (data.m_bits)
                        {
                            case 8:     install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto8mo, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto16mo, entry.m_mask, entry.m_cswidth); break;
                            case 32:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto32mo, entry.m_mask, entry.m_cswidth); break;
                            case 64:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto64mo, entry.m_mask, entry.m_cswidth); break;
                        }
                    }
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE_SMO:
                    if (readorwrite == read_or_write.READ)
                    {
                        switch (data.m_bits)
                        {
                            case 8:     install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto8smo, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto16smo, entry.m_mask, entry.m_cswidth); break;
                            case 32:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto32smo, entry.m_mask, entry.m_cswidth); break;
                            case 64:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto64smo, entry.m_mask, entry.m_cswidth); break;
                        }
                    }
                    else
                    {
                        switch (data.m_bits)
                        {
                            case 8:     install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto8smo, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto16smo, entry.m_mask, entry.m_cswidth); break;
                            case 32:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto32smo, entry.m_mask, entry.m_cswidth); break;
                            case 64:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto64smo, entry.m_mask, entry.m_cswidth); break;
                        }
                    }
                    break;

                case map_handler_type.AMH_PORT:
                    install_readwrite_port(entry.m_addrstart, entry.m_addrend, entry.m_addrmirror,
                                           (readorwrite == read_or_write.READ) ? entry.m_devbase.subtag(data.m_tag) : "",
                                           (readorwrite == read_or_write.WRITE) ? entry.m_devbase.subtag(data.m_tag) : "");
                    break;

                case map_handler_type.AMH_BANK:
                    {
                        string tag = entry.m_devbase.subtag(data.m_tag);
                        memory_bank bank = m_manager.bank_find(tag);
                        if (bank == null)
                            bank = m_manager.bank_alloc(entry.m_devbase, tag);
                        install_bank_generic(entry.m_addrstart, entry.m_addrend, entry.m_addrmirror,
                                             (readorwrite == read_or_write.READ) ? bank : null,
                                             (readorwrite == read_or_write.WRITE) ? bank : null);
                    }
                    break;

                case map_handler_type.AMH_DEVICE_SUBMAP:
                    throw new emu_fatalerror("Internal mapping error: leftover mapping of '{0}'.\n", data.m_tag);

                case map_handler_type.AMH_VIEW:
                    if (readorwrite == read_or_write.READ)
                        install_view(entry.m_addrstart, entry.m_addrend, entry.m_addrmirror, entry.m_view);
                    break;
            }
        }


        protected void adjust_addresses(ref offs_t start, ref offs_t end, ref offs_t mask, ref offs_t mirror)
        {
            // adjust start/end/mask values
            mask &= m_addrmask;
            start &= ~mirror & m_addrmask;
            end &= ~mirror & m_addrmask;
        }


        protected void check_optimize_all(string function, int width, offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, out offs_t nstart, out offs_t nend, out offs_t nmask, out offs_t nmirror, out u64 nunitmask, out int ncswidth)
        {
            if (addrstart > addrend)
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, start address is after the end address.\n", function, addrstart, addrend, addrmask, addrmirror, addrselect);
            if ((addrstart & ~m_addrmask) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, start address is outside of the global address mask {6}, did you mean {7} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, m_addrmask, addrstart & m_addrmask);
            if ((addrend & ~m_addrmask) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, end address is outside of the global address mask {6}, did you mean {7} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, m_addrmask, addrend & m_addrmask);

            // Check the relative data widths
            if (width > m_config.data_width())
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, cannot install a {6}-bits wide handler in a {7}-bits wide address space.\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, width, m_config.data_width());

            // Check the validity of the addresses given their intrinsic width
            // We assume that busses with non-zero address shift have a data width matching the shift (reality says yes)
            offs_t default_lowbits_mask = (offs_t)((m_config.data_width() >> (3 - m_config.addr_shift())) - 1);
            offs_t lowbits_mask = width != 0 && m_config.addr_shift() == 0 ? ((offs_t)width >> 3) - 1 : default_lowbits_mask;

            if ((addrstart & lowbits_mask) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, start address has low bits set, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrstart & ~lowbits_mask);
            if (((~addrend) & lowbits_mask) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, end address has low bits unset, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrend | lowbits_mask);


            offs_t set_bits = addrstart | addrend;
            offs_t changing_bits = addrstart ^ addrend;
            // Round up to the nearest power-of-two-minus-one
            changing_bits |= changing_bits >> 1;
            changing_bits |= changing_bits >> 2;
            changing_bits |= changing_bits >> 4;
            changing_bits |= changing_bits >> 8;
            changing_bits |= changing_bits >> 16;

            if ((addrmask & ~m_addrmask) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, mask is outside of the global address mask {6}, did you mean {7} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, m_addrmask, addrmask & m_addrmask);
            if ((addrselect & ~m_addrmask) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, select is outside of the global address mask {6}, did you mean {7} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, m_addrmask, addrselect & m_addrmask);
            if ((addrmask & ~changing_bits) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, mask is trying to unmask an unchanging address bit, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrmask & changing_bits);
            if ((addrmirror & changing_bits) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, mirror touches a changing address bit, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrmirror & ~changing_bits);
            if ((addrselect & changing_bits) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, select touches a changing address bit, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrselect & ~changing_bits);
            if ((addrmirror & set_bits) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, mirror touches a set address bit, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrmirror & ~set_bits);
            if ((addrselect & set_bits) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, select touches a set address bit, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrselect & ~set_bits);
            if ((addrmirror & addrselect) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, mirror touches a select bit, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrmirror & ~addrselect);

            // Check the cswidth, if provided
            if (cswidth > m_config.data_width())
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, the cswidth of {6} is too large for a {7}-bit space.\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, cswidth, m_config.data_width());
            if (width != 0 && (cswidth % width) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, the cswidth of {6} is not a multiple of handler size {7}.\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, cswidth, width);

            ncswidth = cswidth != 0 ? cswidth : width;

            // Check if the unitmask is structurally correct for the width
            // Not sure what we can actually handle regularity-wise, so don't check that yet
            if (width != 0)
            {
                // Check if the 1-blocks are of appropriate size
                u64 block_mask = 0xffffffffffffffffU >> (64 - width);
                u64 cs_mask = 0xffffffffffffffffU >> (64 - ncswidth);
                for (int pos = 0; pos < 64; pos += ncswidth)
                {
                    u64 cmask = (unitmask >> pos) & cs_mask;
                    while (cmask != 0 && (cmask & block_mask) == 0)
                        cmask >>= width;
                    if (cmask != 0 && cmask != block_mask)
                        global_object.fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, the unitmask of %016x has incorrect granularity for %d-bit chip selection.\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth);
                }
            }

            nunitmask = 0xffffffffffffffffU >> (64 - m_config.data_width());
            if (unitmask != 0)
                nunitmask &= unitmask;

            nstart = addrstart;
            nend = addrend;
            nmask = (addrmask != 0 ? addrmask : changing_bits) | addrselect;
            nmirror = (addrmirror & m_addrmask) | addrselect;

            if ((nmirror & default_lowbits_mask) != 0)
            {
                // If the mirroring/select "reaches" within the bus
                // granularity we have to adapt it and the unitmask.

                // We're sure start/end are on the same data-width-sized
                // entry, because otherwise the previous tests wouldn't have
                // passed.  So we need to clear the part of the unitmask that
                // not in the range, then replicate it following the mirror.
                // The start/end also need to be adjusted to the bus
                // granularity.

                // 1. Adjusting
                nstart &= ~default_lowbits_mask;
                nend |= default_lowbits_mask;

                // 2. Clearing
                u64 smask;
                u64 emask;
                if (m_config.endianness() == endianness_t.ENDIANNESS_BIG)
                {
                    smask =  g.make_bitmask64((u32)m_config.data_width() - ((addrstart - nstart) << (3 - m_config.addr_shift())));
                    emask = ~g.make_bitmask64((u32)m_config.data_width() - ((addrend - nstart + 1) << (3 - m_config.addr_shift())));
                }
                else
                {
                    smask = ~g.make_bitmask64((addrstart - nstart) << (3 - m_config.addr_shift()));
                    emask =  g.make_bitmask64((addrend - nstart + 1) << (3 - m_config.addr_shift()));
                }

                nunitmask &= smask & emask;

                // 3. Mirroring
                offs_t to_mirror = nmirror & default_lowbits_mask;
                if (m_config.endianness() == endianness_t.ENDIANNESS_BIG)
                {
                    for (int i = 0; to_mirror != 0; i++)
                    {
                        if (((to_mirror >> i) & 1) != 0)
                        {
                            to_mirror &= ~(1U << i);
                            nunitmask |= nunitmask >> (1 << (3 + i - m_config.addr_shift()));
                        }
                    }
                }
                else
                {
                    for (int i = 0; to_mirror != 0; i++)
                    {
                        if (((to_mirror >> i) & 1) != 0)
                        {
                            to_mirror &= ~(1U << i);
                            nunitmask |= nunitmask << (1 << (3 + i - m_config.addr_shift()));
                        }
                    }
                }

                // 4. Ajusting the mirror
                nmirror &= ~default_lowbits_mask;

                // 5. Recompute changing_bits, it matters for the next optimization.  No need to round up through
                changing_bits = nstart ^ nend;
            }


            if (nmirror != 0 && (nstart & changing_bits) == 0 && ((~nend) & changing_bits) == 0)
            {
                // If the range covers the a complete power-of-two zone, it is
                // possible to remove 1 bits from the mirror, pushing the end
                // address.  The mask will clamp, and installing the range
                // will be faster.
                while ((nmirror & (changing_bits + 1)) != 0)
                {
                    offs_t bit = nmirror & (changing_bits+1);
                    nmirror &= ~bit;
                    nend |= bit;
                    changing_bits |= bit;
                }
            }
        }


        protected void check_optimize_mirror(string function, offs_t addrstart, offs_t addrend, offs_t addrmirror, out offs_t nstart, out offs_t nend, out offs_t nmask, out offs_t nmirror)
        {
            if (addrstart > addrend)
                global_object.fatalerror("{0}: In range {1}-{2} mirror {3}, start address is after the end address.\n", function, addrstart, addrend, addrmirror);
            if ((addrstart & ~m_addrmask) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mirror {3}, start address is outside of the global address mask {4}, did you mean {5} ?\n", function, addrstart, addrend, addrmirror, m_addrmask, addrstart & m_addrmask);
            if ((addrend & ~m_addrmask) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mirror {3}, end address is outside of the global address mask {4}, did you mean {5} ?\n", function, addrstart, addrend, addrmirror, m_addrmask, addrend & m_addrmask);

            offs_t lowbits_mask = (offs_t)((m_config.data_width() >> (3 - m_config.addr_shift())) - 1);
            if ((addrstart & lowbits_mask) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mirror {3}, start address has low bits set, did you mean {4} ?\n", function, addrstart, addrend, addrmirror, addrstart & ~lowbits_mask);
            if (((~addrend) & lowbits_mask) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mirror {3}, end address has low bits unset, did you mean {4} ?\n", function, addrstart, addrend, addrmirror, addrend | lowbits_mask);

            offs_t set_bits = addrstart | addrend;
            offs_t changing_bits = addrstart ^ addrend;
            // Round up to the nearest power-of-two-minus-one
            changing_bits |= changing_bits >> 1;
            changing_bits |= changing_bits >> 2;
            changing_bits |= changing_bits >> 4;
            changing_bits |= changing_bits >> 8;
            changing_bits |= changing_bits >> 16;

            if ((addrmirror & ~m_addrmask) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mirror {3}, mirror is outside of the global address mask {4}, did you mean {5} ?\n", function, addrstart, addrend, addrmirror, m_addrmask, addrmirror & m_addrmask);
            if ((addrmirror & changing_bits) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mirror {3}, mirror touches a changing address bit, did you mean {4} ?\n", function, addrstart, addrend, addrmirror, addrmirror & ~changing_bits);
            if ((addrmirror & set_bits) != 0)
                global_object.fatalerror("{0}: In range {1}-{2} mirror {3}, mirror touches a set address bit, did you mean {4} ?\n", function, addrstart, addrend, addrmirror, addrmirror & ~set_bits);

            nstart = addrstart;
            nend = addrend;
            nmask = changing_bits;
            nmirror = addrmirror;

            if (nmirror != 0 && ((nstart & changing_bits) == 0) && ((~nend) & changing_bits) == 0)
            {
                // If the range covers the a complete power-of-two zone, it is
                // possible to remove 1 bits from the mirror, pushing the end
                // address.  The mask will clamp, and installing the range
                // will be faster.
                while ((nmirror & (changing_bits + 1)) != 0)
                {
                    offs_t bit = nmirror & (changing_bits + 1);
                    nmirror &= ~bit;
                    nend |= bit;
                    changing_bits |= bit;
                }
            }
        }


        //void check_address(const char *function, offs_t addrstart, offs_t addrend);
    }


    // address_space holds live information about an address space
    public abstract class address_space : address_space_installer, IDisposable
    {
        //friend class memory_bank;
        //friend class memory_block;
        //template<int Width, int AddrShift, endianness_t Endian> friend class handler_entry_read_unmapped;
        //template<int Width, int AddrShift, endianness_t Endian> friend class handler_entry_write_unmapped;


        class notifier_t
        {
            public Action<read_or_write> m_notifier;  //std::function<void (read_or_write)> m_notifier;
            public int m_id;
        }


        // private state
        protected device_t m_device;           // reference to the owning device
        address_map m_map;         // original memory map  //std::unique_ptr<address_map> m_map;         // original memory map
        u64 m_unmap;            // unmapped value
        int m_spacenum;         // address space index
        bool m_log_unmap;        // log unmapped accesses in this space?
        protected string m_name;             // friendly name of the address space

        protected handler_entry m_unmap_r;
        protected handler_entry m_unmap_w;

        protected handler_entry m_nop_r;
        protected handler_entry m_nop_w;

        //std::vector<std::unique_ptr<memory_passthrough_handler>> m_mphs;

        std.vector<notifier_t> m_notifiers = new std.vector<notifier_t>();        // notifier list for address map change
        int m_notifier_id;      // next notifier id
        u32 m_in_notification;  // notification(s) currently being done


        // construction/destruction
        protected address_space(memory_manager manager, device_memory_interface memory, int spacenum)
            : base(memory.space_config(spacenum), manager)
        {
            m_device = memory.device();
            m_unmap = 0;
            m_spacenum = spacenum;
            m_log_unmap = true;
            m_name = memory.space_config(spacenum).name();
            m_notifier_id = 0;
            m_in_notification = 0;
        }


        ~address_space()
        {
            global_object.assert(m_isDisposed);  // can remove
        }

    
        bool m_isDisposed = false;
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                m_unmap_r.unref();
                m_unmap_w.unref();
                m_nop_r.unref();
                m_nop_w.unref();
            }
    
            m_isDisposed = true;
        }


        // getters
        public device_t device() { return m_device; }
        public string name() { return m_name; }
        public int spacenum() { return m_spacenum; }
        //address_map *map() const { return m_map.get(); }


        //template<int Width, int AddrShift, endianness_t Endian>
        public void cache<int_Width, int_AddrShift, endianness_t_Endian>(emu.detail.memory_access_cache<int_Width, int_AddrShift, endianness_t_Endian> v)  //template<int Width, int AddrShift, endianness_t Endian> void cache(emu::detail::memory_access_cache<Width, AddrShift, Endian> &v) {
            where int_Width : int_constant, new()
            where int_AddrShift : int_constant, new()
            where endianness_t_Endian : endianness_t_constant, new()
        {
            int Width = new int_Width().value;
            int AddrShift = new int_AddrShift().value;
            endianness_t Endian = new endianness_t_Endian().value;

            if (AddrShift != m_config.addr_shift())
                fatalerror("Requesting cache() with address shift {0} while the config says {1}\n", AddrShift, m_config.addr_shift());
            if (8 << Width != m_config.data_width())
                fatalerror("Requesting cache() with data width {0} while the config says {1}\n", 8 << Width, m_config.data_width());
            if (Endian != m_config.endianness())
                fatalerror("Requesting cache() with endianness {0} while the config says {1}\n",
                           endianness_names[(int)Endian], endianness_names[(int)m_config.endianness()]);

            v.set(this, get_cache_info());
        }


        //template<int Level, int Width, int AddrShift, endianness_t Endian>
        public void specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian>(emu.detail.memory_access_specific<int_Level, int_Width, int_AddrShift, endianness_t_Endian> v)  //template<int Level, int Width, int AddrShift, endianness_t Endian> void specific(emu::detail::memory_access_specific<Level, Width, AddrShift, Endian> &v) {
            where int_Level : int_constant, new()
            where int_Width : int_constant, new()
            where int_AddrShift : int_constant, new()
            where endianness_t_Endian : endianness_t_constant, new()
        {
            int Level = new int_Level().value;
            int Width = new int_Width().value;
            int AddrShift = new int_AddrShift().value;
            endianness_t Endian = new endianness_t_Endian().value;

            if (Level != emumem_global.handler_entry_dispatch_level(m_config.addr_width()))
                fatalerror("Requesting specific() with wrong level, bad address width (the config says {0})\n", m_config.addr_width());
            if (AddrShift != m_config.addr_shift())
                fatalerror("Requesting specific() with address shift {0} while the config says {1}\n", AddrShift, m_config.addr_shift());
            if (8 << Width != m_config.data_width())
                fatalerror("Requesting specific() with data width {0} while the config says {1}\n", 8 << Width, m_config.data_width());
            if (Endian != m_config.endianness())
                fatalerror("Requesting spefific() with endianness {0} while the config says {1}\n",
                           endianness_names[(int)Endian], endianness_names[(int)m_config.endianness()]);

            v.set(this, get_specific_info());
        }


        //**************************************************************************
        //  MEMORY MAPPING HELPERS
        //**************************************************************************

        public int add_change_notifier(Action<read_or_write> n)  //int add_change_notifier(std::function<void (read_or_write)> n);
        {
            int id = m_notifier_id++;
            m_notifiers.emplace_back(new notifier_t() { m_notifier = n, m_id = id });
            return id;
        }


        //void remove_change_notifier(int id);


        protected void invalidate_caches(read_or_write mode)
        {
            if (((u32)mode & ~m_in_notification) != 0)
            {
                u32 old = m_in_notification;
                m_in_notification |= (u32)mode;
                foreach (var n in m_notifiers)
                    n.m_notifier(mode);
                m_in_notification = old;
            }
        }


        protected abstract void validate_reference_counts();

        protected abstract void remove_passthrough(std.unordered_set<handler_entry> handlers);  //virtual void remove_passthrough(std::unordered_set<handler_entry *> &handlers) = 0;


        public u64 unmap() { return m_unmap; }


        //memory_passthrough_handler *make_mph();


        // debug helpers
        protected abstract string get_handler_string(read_or_write readorwrite, offs_t byteaddress);
        protected abstract void dump_maps(std.vector<memory_entry> read_map, std.vector<memory_entry> write_map);  //virtual void dump_maps(std::vector<memory_entry> &read_map, std::vector<memory_entry> &write_map) const = 0;
        public bool log_unmap() { return m_log_unmap; }
        public void set_log_unmap(bool log) { m_log_unmap = log; }

        // general accessors
        protected abstract void accessors(data_accessors accessors);
        protected abstract object get_read_ptr(offs_t address);  //virtual void *get_read_ptr(offs_t address) const = 0;
        protected abstract object get_write_ptr(offs_t address);  //virtual void *get_write_ptr(offs_t address) const = 0;

        // read accessors
        public abstract u8 read_byte(offs_t address);
        public abstract u16 read_word(offs_t address);
        public abstract u16 read_word(offs_t address, u16 mask);
        protected abstract u16 read_word_unaligned(offs_t address);
        protected abstract u16 read_word_unaligned(offs_t address, u16 mask);
        protected abstract u32 read_dword(offs_t address);
        protected abstract u32 read_dword(offs_t address, u32 mask);
        protected abstract u32 read_dword_unaligned(offs_t address);
        protected abstract u32 read_dword_unaligned(offs_t address, u32 mask);
        protected abstract u64 read_qword(offs_t address);
        protected abstract u64 read_qword(offs_t address, u64 mask);
        protected abstract u64 read_qword_unaligned(offs_t address);
        protected abstract u64 read_qword_unaligned(offs_t address, u64 mask);

        // write accessors
        public abstract void write_byte(offs_t address, u8 data);
        public abstract void write_word(offs_t address, u16 data);
        public abstract void write_word(offs_t address, u16 data, u16 mask);
        protected abstract void write_word_unaligned(offs_t address, u16 data);
        protected abstract void write_word_unaligned(offs_t address, u16 data, u16 mask);
        protected abstract void write_dword(offs_t address, u32 data);
        protected abstract void write_dword(offs_t address, u32 data, u32 mask);
        protected abstract void write_dword_unaligned(offs_t address, u32 data);
        protected abstract void write_dword_unaligned(offs_t address, u32 data, u32 mask);
        protected abstract void write_qword(offs_t address, u64 data);
        protected abstract void write_qword(offs_t address, u64 data, u64 mask);
        protected abstract void write_qword_unaligned(offs_t address, u64 data);
        protected abstract void write_qword_unaligned(offs_t address, u64 data, u64 mask);


        // setup

        //-------------------------------------------------
        //  prepare_map - allocate the address map and
        //  walk through it to find implicit memory regions
        //  and identify shared regions
        //-------------------------------------------------
        public void prepare_map()
        {
            // allocate the address map
            m_map = new address_map(m_device, m_spacenum);

            // merge in the submaps
            m_map.import_submaps(m_manager.machine(), m_device.owner() != null ? m_device.owner() : m_device, data_width(), endianness(), addr_shift());

            // extract global parameters specified by the map
            m_unmap = (m_map.m_unmapval == 0) ? 0U : u64.MaxValue;  //m_unmap = (m_map->m_unmapval == 0) ? 0 : ~0;
            if (m_map.m_globalmask != 0)
            {
                if ((m_map.m_globalmask & ~m_addrmask) != 0)
                    global_object.fatalerror("Can't set a global address mask of {0} on a {1}-bits address width bus.\n", m_map.m_globalmask, addr_width());

                m_addrmask = m_map.m_globalmask;
            }

            prepare_map_generic(m_map, true);
        }


        //-------------------------------------------------
        //  prepare_map_generic - walk through an address
        //  map to find implicit memory regions and
        //  identify shared regions
        //-------------------------------------------------
        public void prepare_map_generic(address_map map, bool allow_alloc)
        {
            memory_region devregion = (m_spacenum == 0) ? m_device.memregion(g.DEVICE_SELF) : null;
            u32 devregionsize = (devregion != null) ? devregion.bytes() : 0;

            // make a pass over the address map, adjusting for the device and getting memory pointers
            foreach (address_map_entry entry in map.m_entrylist)
            {
                // computed adjusted addresses first
                adjust_addresses(ref entry.m_addrstart, ref entry.m_addrend, ref entry.m_addrmask, ref entry.m_addrmirror);

                // if we have a share entry, add it to our map
                if (entry.m_share != null)
                {
                    // if we can't find it, add it to our map if we're allowed to
                    string fulltag = entry.m_devbase.subtag(entry.m_share);
                    memory_share share = m_manager.share_find(fulltag);
                    if (share == null)
                    {
                        if (!allow_alloc)
                            global_object.fatalerror("Trying to create share '{0}' too late\n", fulltag);

                        emumem_global.VPRINTF("Creating share '{0}' of length {1}\n", fulltag, entry.m_addrend + 1 - entry.m_addrstart);
                        share = m_manager.share_alloc(m_device, fulltag, (u8)m_config.data_width(), address_to_byte(entry.m_addrend + 1 - entry.m_addrstart), endianness());
                    }
                    else
                    {
                        string result = share.compare((u8)m_config.data_width(), address_to_byte(entry.m_addrend + 1 - entry.m_addrstart), endianness());
                        if (!result.empty())
                            global_object.fatalerror("{0}\n", result);
                    }

                    entry.m_memory = share.ptr();
                }

                // if this is a ROM handler without a specified region and not shared, attach it to the implicit region
                if (m_spacenum == emumem_global.AS_PROGRAM && entry.m_read.m_type == map_handler_type.AMH_ROM && entry.m_region == null && entry.m_share == null)
                {
                    // make sure it fits within the memory region before doing so, however
                    if (entry.m_addrend < devregionsize)
                    {
                        entry.m_region = m_device.tag();
                        entry.m_rgnoffs = address_to_byte(entry.m_addrstart);
                    }
                }

                // validate adjusted addresses against implicit regions
                if (entry.m_region != null)
                {
                    // determine full tag
                    string fulltag = entry.m_devbase.subtag(entry.m_region);

                    // find the region
                    memory_region region = m_manager.machine().root_device().memregion(fulltag);
                    if (region == null)
                        global_object.fatalerror("device '{0}' {1} space memory map entry {2}-{3} references nonexistent region \"{4}\"\n", m_device.tag(), m_name, entry.m_addrstart, entry.m_addrend, entry.m_region);

                    // validate the region
                    if (entry.m_rgnoffs + m_config.addr2byte(entry.m_addrend - entry.m_addrstart + 1) > region.bytes())
                        global_object.fatalerror("device '{0}' {1} space memory map entry {2}-{3} extends beyond region \"{4}\" size ({5})\n", m_device.tag(), m_name, entry.m_addrstart, entry.m_addrend, entry.m_region, region.bytes());

                    if (entry.m_share != null)
                        global_object.fatalerror("device '{0}' {1} space memory map entry {2}-{3} has both .region() and .share()\n", m_device.tag(), m_name, entry.m_addrstart, entry.m_addrend);
                }

                // convert any region-relative entries to their memory pointers
                if (entry.m_region != null)
                {
                    // determine full tag
                    string fulltag = entry.m_devbase.subtag(entry.m_region);

                    // set the memory address
                    entry.m_memory = new PointerU8(m_manager.machine().root_device().memregion(fulltag).base_(), (int)entry.m_rgnoffs);  //entry.m_memory = m_manager.machine().root_device().memregion(fulltag)->base() + entry.m_rgnoffs;
                }

                // allocate anonymous ram when needed
                if (entry.m_memory == null && (entry.m_read.m_type == map_handler_type.AMH_RAM || entry.m_write.m_type == map_handler_type.AMH_RAM))
                {
                    if (!allow_alloc)
                        global_object.fatalerror("Trying to create memory in range {0}-{1} too late\n", entry.m_addrstart, entry.m_addrend);

                    entry.m_memory = m_manager.anonymous_alloc(this, address_to_byte(entry.m_addrend + 1 - entry.m_addrstart), (u8)m_config.data_width(), entry.m_addrstart, entry.m_addrend);
                }
            }
        }


        //void prepare_device_map(address_map &map);


        //-------------------------------------------------
        //  populate_from_map - walk the map in reverse
        //  order and install the appropriate handler for
        //  each case
        //-------------------------------------------------
        public void populate_from_map(address_map map = null)
        {
            // no map specified, use the space-specific one
            if (map == null)
                map = m_map;

            // no map, nothing to do
            if (map == null)
                return;

            // install the handlers, using the original, unadjusted memory map
            foreach (address_map_entry entry in map.m_entrylist)
            {
                // map both read and write halves
                populate_map_entry(entry, read_or_write.READ);
                populate_map_entry(entry, read_or_write.WRITE);
            }

            if (emumem_global.VALIDATE_REFCOUNTS)
                validate_reference_counts();
        }


        //template<int Width, int AddrShift, endianness_t Endian>
        public handler_entry_read_unmapped<int_Width, int_AddrShift, endianness_t_Endian> get_unmap_r<int_Width, int_AddrShift, endianness_t_Endian>()  //template<int Width, int AddrShift, endianness_t Endian> handler_entry_read_unmapped <Width, AddrShift, Endian> *get_unmap_r() const { return static_cast<handler_entry_read_unmapped <Width, AddrShift, Endian> *>(m_unmap_r); }
            where int_Width : int_constant, new()
            where int_AddrShift : int_constant, new()
            where endianness_t_Endian : endianness_t_constant, new()
        {
            return (handler_entry_read_unmapped<int_Width, int_AddrShift, endianness_t_Endian>)m_unmap_r;
        }

        //template<int Width, int AddrShift, endianness_t Endian>
        public handler_entry_write_unmapped<int_Width, int_AddrShift, endianness_t_Endian> get_unmap_w<int_Width, int_AddrShift, endianness_t_Endian>()  //template<int Width, int AddrShift, endianness_t Endian> handler_entry_write_unmapped<Width, AddrShift, Endian> *get_unmap_w() const { return static_cast<handler_entry_write_unmapped<Width, AddrShift, Endian> *>(m_unmap_w); }
            where int_Width : int_constant, new()
            where int_AddrShift : int_constant, new()
            where endianness_t_Endian : endianness_t_constant, new()
        {
            return (handler_entry_write_unmapped<int_Width, int_AddrShift, endianness_t_Endian>)m_unmap_w;
        }


        //handler_entry *unmap_r() const { return m_unmap_r; }
        //handler_entry *unmap_w() const { return m_unmap_w; }
        //handler_entry *nop_r() const { return m_nop_r; }
        //handler_entry *nop_w() const { return m_nop_w; }


        // internal helpers
        protected abstract std.pair<object, object> get_cache_info();  //virtual std::pair<void *, void *> get_cache_info() = 0;
        protected abstract std.pair<object, object> get_specific_info();  //virtual std::pair<const void *, const void *> get_specific_info() = 0;
    }


    // ======================> memory_bank
    // a memory bank is a global pointer to memory that can be shared across devices and changed dynamically
    public class memory_bank
    {
        // internal state
        running_machine m_machine;              // need the machine to free our memory
        std.vector<PointerRef<u8>> m_entries = new std.vector<PointerRef<u8>>();  //std::vector<u8 *>       m_entries;              // the entries
        //bool m_anonymous;            // are we anonymous or explicit?
        //offs_t m_addrstart;            // start offset
        //offs_t m_addrend;              // end offset
        int m_curentry;             // current entry
        string m_name;                 // friendly name for this bank
        string m_tag;                  // tag for this bank
        //std.vector<bank_reference> m_reflist = new std.vector<bank_reference>();  // std::vector<std::unique_ptr<bank_reference>> m_reflist;     // list of address spaces referencing this bank
        //std.vector<Action<PointerU8>> m_alloc_notifier = new std.vector<Action<PointerU8>>();  //std::vector<std::function<void (void *)>> m_alloc_notifier; // list of notifier targets when allocating


        //public string m_uuid = Guid.NewGuid().ToString();  // used for generating a unique name (when the pointer is used), see bank_find_or_allocate() for example


        // construction/destruction
        //-------------------------------------------------
        //  memory_bank - constructor
        //-------------------------------------------------
        public memory_bank(device_t device, string tag)
        {
            m_machine = device.machine();
            m_curentry = 0;


            m_tag = tag;
            m_name = util_.string_format("Bank '%s'", m_tag);
            machine().save().save_item(device, "memory", m_tag, 0, m_curentry, "m_curentry");
        }


        //public memory_bank get() { return this; }  // for smart pointers


        // getters
        running_machine machine() { return m_machine; }
        public int entry() { return m_curentry; }
        public PointerRef<u8> base_() { return m_entries.empty() ? null : m_entries[m_curentry]; }  //void *base() const { return m_entries.empty() ? nullptr : m_entries[m_curentry]; }
        public string tag() { return m_tag; }
        //const char *name() const { return m_name; }


        // set the base explicitly

        //-------------------------------------------------
        //  set_base - set the bank base explicitly
        //-------------------------------------------------
        public void set_base(PointerU8 base_)  //void set_base(void *base);
        {
            // NULL is not an option
            if (base_ == null)
                throw new emu_fatalerror("memory_bank::set_base called NULL base");

            // set the base
            if (m_entries.empty())
            {
                m_entries.resize(1);
                m_curentry = 0;
            }

            m_entries[m_curentry] = new PointerRef<u8>();
            m_entries[m_curentry].m_pointer = base_;  //reinterpret_cast<u8 *>(base_);
        }


        // configure and set entries
        //-------------------------------------------------
        //  configure_entry - configure an entry
        //-------------------------------------------------
        public void configure_entry(int entrynum, PointerU8 base_)  //void configure_entry(int entrynum, void *base);
        {
            // must be positive
            if (entrynum < 0)
                throw new emu_fatalerror("memory_bank::configure_entry called with out-of-range entry {0}", entrynum);

            // if we haven't allocated this many entries yet, expand our array
            if (entrynum >= m_entries.size())
                m_entries.resize(entrynum+1);

            // set the entry
            m_entries[entrynum] = new PointerRef<u8>(base_);  //m_entries[entrynum] = reinterpret_cast<u8 *>(base);
        }


        //-------------------------------------------------
        //  configure_entries - configure multiple entries
        //-------------------------------------------------
        public void configure_entries(int startentry, int numentries, PointerU8 base_, offs_t stride)  //void configure_entries(int startentry, int numentries, void *base, offs_t stride);
        {
            if (startentry + numentries >= (int)m_entries.size())
            {
                //m_entries.resize(startentry + numentries+1);
                m_entries.clear();
                for (int i = 0; i < startentry + numentries+1; i++)
                    m_entries.Add(new PointerRef<u8>());
            }

            // fill in the requested bank entries
            for (int entrynum = 0; entrynum < numentries; entrynum ++)
                m_entries[entrynum + startentry].m_pointer = new PointerU8(base_, entrynum * (int)stride);  //m_entries[entrynum + startentry] = reinterpret_cast<u8 *>(base) +  entrynum * stride ;
        }


        //-------------------------------------------------
        //  set_entry - set the base to a pre-configured
        //  entry
        //-------------------------------------------------
        public void set_entry(int entrynum)
        {
            if (entrynum == -1 && m_entries.empty())
                return;

            // validate
            if (entrynum < 0 || entrynum >= m_entries.size())
                throw new emu_fatalerror("memory_bank::set_entry called with out-of-range entry {0}", entrynum);
            if (m_entries[entrynum] == null)
                throw new emu_fatalerror("memory_bank::set_entry called for bank '{0}' with invalid bank entry {1}", m_tag, entrynum);

            m_curentry = entrynum;
        }
    }


    // ======================> memory_share
    // a memory share contains information about shared memory region
    public class memory_share
    {
        // internal state
        string m_name;                 // share name
        PointerU8 m_ptr;  //void *                  m_ptr;                  // pointer to the memory backing the region
        size_t m_bytes;  // size of the shared region in bytes
        endianness_t m_endianness;           // endianness of the memory
        u8 m_bitwidth;             // width of the shared region in bits
        u8 m_bytewidth;            // width in bytes, rounded up to a power of 2


        // construction/destruction
        public memory_share(string name, u8 width, size_t bytes, endianness_t endianness, PointerU8 ptr)  //memory_share(std::string name, u8 width, size_t bytes, endianness_t endianness, void *ptr)
        {
            m_name = name;
            m_ptr = new PointerU8(ptr);
            m_bytes = bytes;
            m_endianness = endianness;
            m_bitwidth = width;
            m_bytewidth = (width <= 8 ? (byte)1 : width <= 16 ? (byte)2 : width <= 32 ? (byte)4 : (byte)8);
        }


        // getters
        string name() { return m_name; }
        public PointerU8 ptr() { return m_ptr; }  //void *ptr() const { return m_ptr; }
        public u64 bytes() { return m_bytes; }
        public endianness_t endianness() { return m_endianness; }
        public u8 bitwidth() { return m_bitwidth; }
        public u8 bytewidth() { return m_bytewidth; }


        public string compare(u8 width, size_t bytes, endianness_t endianness)
        {
            throw new emu_unimplemented();
        }
    }


    // ======================> memory_region
    // memory region object
    public class memory_region
    {
        // internal data
        running_machine m_machine;
        string m_name;
        std.vector<u8> m_buffer;
        endianness_t m_endianness;
        u8 m_bitwidth;
        u8 m_bytewidth;

        // construction/destruction

        //-------------------------------------------------
        //  memory_region - constructor
        //-------------------------------------------------
        public memory_region(running_machine machine, string name, u32 length, u8 width, endianness_t endian)
        {
            m_machine = machine;
            m_name = name;
            m_buffer = new std.vector<u8>(length);
            m_endianness = endian;
            m_bitwidth = (u8)(width * 8);
            m_bytewidth = width;


            //assert(width == 1 || width == 2 || width == 4 || width == 8);
        }


        // getters
        running_machine machine() { return m_machine; }
        public MemoryU8 base_() { return (m_buffer.size() > 0) ? m_buffer : null; }  //u8 *base() { return (m_buffer.size() > 0) ? &m_buffer[0] : nullptr; }
        //u8 *end() { return base() + m_buffer.size(); }
        public u32 bytes() { return (u32)m_buffer.Count; }  //u32 bytes() const { return m_buffer.size(); }
        public string name() { return m_name; }

        // flag expansion
        public endianness_t endianness() { return m_endianness; }
        public u8 bitwidth() { return m_bitwidth; }
        public u8 bytewidth() { return m_bytewidth; }

        // data access
        //u8 &as_u8(offs_t offset = 0) { return m_buffer[offset]; }
        //u16 &as_u16(offs_t offset = 0) { return reinterpret_cast<u16 *>(base())[offset]; }
        //u32 &as_u32(offs_t offset = 0) { return reinterpret_cast<u32 *>(base())[offset]; }
        //u64 &as_u64(offs_t offset = 0) { return reinterpret_cast<u64 *>(base())[offset]; }
    }


    // ======================> memory_view
    // a memory view allows switching between submaps in the map
    public class memory_view
    {
        //template<int Level, int Width, int AddrShift, endianness_t Endian> friend class address_space_specific;
        //template<int Level, int Width, int AddrShift, endianness_t Endian> friend class memory_view_entry_specific;
        //template<int HighBits, int Width, int AddrShift, endianness_t Endian> friend class handler_entry_write_dispatch;
        //template<int HighBits, int Width, int AddrShift, endianness_t Endian> friend class handler_entry_read_dispatch;
        //friend class memory_view_entry;
        //friend class address_map_entry;
        //friend class address_map;
        //friend class device_t;

        //DISABLE_COPYING(memory_view);


        public abstract class memory_view_entry : address_space_installer
        {
            //memory_view &m_view;
            protected address_map m_map;  //std::unique_ptr<address_map> m_map;
            //int m_id;


            protected memory_view_entry(address_space_config config, memory_manager manager, memory_view view, int id)
                 : base(config, manager)
            {
                throw new emu_unimplemented();
            }


            //virtual ~memory_view_entry() = default;

            //address_map_entry &operator()(offs_t start, offs_t end);
            public address_map_entry op(offs_t start, offs_t end)
            {
                return m_map.op(start, end);
            }


            protected abstract void populate_from_map(address_map map = null);


            //std::string key() const;


            //void prepare_map_generic(address_map &map, bool allow_alloc);
            //void prepare_device_map(address_map &map);


            //void check_range_optimize_all(const char *function, int width, offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, offs_t &nstart, offs_t &nend, offs_t &nmask, offs_t &nmirror, u64 &nunitmask, int &ncswidth);
            //void check_range_optimize_mirror(const char *function, offs_t addrstart, offs_t addrend, offs_t addrmirror, offs_t &nstart, offs_t &nend, offs_t &nmask, offs_t &nmirror);
            //void check_range_address(const char *function, offs_t addrstart, offs_t addrend);
        }


        device_t m_device;
        string m_name;
        std.map<int, int> m_entry_mapping;
        std.vector<memory_view_entry> m_entries;  //std::vector<std::unique_ptr<memory_view_entry>> m_entries;
        address_space_config m_config;
        public offs_t m_addrstart;
        public offs_t m_addrend;
        address_space m_space;
        handler_entry m_handler_read;
        handler_entry m_handler_write;
        int m_cur_id;
        int m_cur_slot;
        //std::string                                     m_context;


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


        //memory_view_entry &operator[](int slot);
        public memory_view_entry op(int slot)
        {
            if (m_config == null)
                global_object.fatalerror("A view must be in a map or a space before it can be setup.");

            var i = m_entry_mapping.find(slot);
            if (i == default)
            {
                memory_view_entry e;
                int id = m_entries.size();
                e = emumem_mview_global.mve_make(emumem_global.handler_entry_dispatch_level(m_config.addr_width()), m_config.data_width(), m_config.addr_shift(), m_config.endianness(),
                             m_config, m_device.machine().memory(), this, id);
                m_entries.resize(id + 1);
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


        public void select(int slot)
        {
            var i = m_entry_mapping.find(slot);
            if (i == default)
                global_object.fatalerror("memory_view {0}: select of unknown slot {1}", m_name, slot);

            m_cur_slot = slot;
            m_cur_id = i;
            m_handler_read.select_a(m_cur_id);
            m_handler_write.select_a(m_cur_id);
        }


        //void disable();


        //const std::string &name() const { return m_name; }


        public void initialize_from_address_map(offs_t addrstart, offs_t addrend, address_space_config config)
        {
            if (m_config != null)
                global_object.fatalerror("A memory_view can be present in only one address map.");

            m_config = config;
            m_addrstart = addrstart;
            m_addrend = addrend;
        }


        //std::pair<handler_entry *, handler_entry *> make_handlers(address_space &space, offs_t addrstart, offs_t addrend);
        //void make_subdispatch(std::string context);
        //int id_to_slot(int id) const;


        public void register_state()
        {
            throw new emu_unimplemented();
        }
    }


    // ======================> memory_manager
    // holds internal state for the memory system
    public class memory_manager : global_object
    {
        //friend class address_space;
        //template<int Level, int Width, int AddrShift, endianness_t Endian> friend class address_space_specific;


        //struct stdlib_deleter { void operator()(void *p) const { free(p); } };


        // internal state
        running_machine m_machine;              // reference to the machine
        std.vector<MemoryU8> m_datablocks = new std.vector<MemoryU8>();           // list of memory blocks to free on exit  //std::vector<std::unique_ptr<void, stdlib_deleter>>               m_datablocks;           // list of memory blocks to free on exit
        std.unordered_map<string, memory_bank> m_banklist = new std.unordered_map<string, memory_bank>();             // map of banks  //std::unordered_map<std::string, std::unique_ptr<memory_bank>>    m_banklist;             // map of banks
        std.unordered_map<string, memory_share> m_sharelist = new std.unordered_map<string, memory_share>();            // map of shares  //std::unordered_map<std::string, std::unique_ptr<memory_share>>   m_sharelist;            // map of shares
        std.unordered_map<string, memory_region> m_regionlist = new std.unordered_map<string, memory_region>();           // map of memory regions  //std::unordered_map<std::string, std::unique_ptr<memory_region>>  m_regionlist;           // map of memory regions


        // construction/destruction

        //-------------------------------------------------
        //  memory_manager - constructor
        //-------------------------------------------------
        public memory_manager(running_machine machine)
        {
            m_machine = machine;
        }


        // initialize the memory spaces from the memory maps of the devices
        //-------------------------------------------------
        //  initialize - initialize the memory system
        //-------------------------------------------------
        public void initialize()
        {
            // loop over devices and spaces within each device
            std.vector<device_memory_interface> memories = new std.vector<device_memory_interface>();
            foreach (device_memory_interface memory in new memory_interface_enumerator(machine().root_device()))
            {
                memories.push_back(memory);
                allocate(memory);
            }

            allocate(m_machine.dummy().memory());

            // construct and preprocess the address_map for each space
            foreach (var memory in memories)
                memory.prepare_maps();

            // create the handlers from the resulting address maps
            foreach (var memory in memories)
                memory.populate_from_maps();

            // disable logging of unmapped access when no one receives it
            //throw new emu_unimplemented();
#if false
            if (!machine().options().log() && !machine().options().oslog() && !(machine().debug_flags & DEBUG_FLAG_ENABLED))
#endif
                foreach (var memory in memories)
                    memory.set_log_unmap(false);
        }


        // getters
        public running_machine machine() { return m_machine; }

        // used for the debugger interface memory views
        public std.unordered_map<string, memory_bank> banks() { return m_banklist; }
        public std.unordered_map<string, memory_region> regions() { return m_regionlist; }
        public std.unordered_map<string, memory_share> shares() { return m_sharelist; }


        // anonymous memory zones
        //-------------------------------------------------
        //  anonymous_alloc - allocates a anonymous memory zone
        //-------------------------------------------------
        public PointerU8 anonymous_alloc(address_space space, size_t bytes, u8 width, offs_t start, offs_t end, string key = "")  //void *anonymous_alloc(address_space &space, size_t bytes, u8 width, offs_t start, offs_t end, const std::string &key = "");
        {
            string name = util_.string_format("{0}{1}-{2}", key, start, end);
            return new PointerU8(allocate_memory(space.device(), space.spacenum(), name, width, bytes));
        }


        // shares

        //-------------------------------------------------
        //  share_alloc - allocates a shared memory zone
        //-------------------------------------------------
        public memory_share share_alloc(device_t dev, string name, u8 width, size_t bytes, endianness_t endianness)
        {
            // make sure we don't have a share of the same name; also find the end of the list
            if (m_sharelist.find(name) != default)
                fatalerror("share_alloc called with duplicate share name \"{0}\"\n", name);

            // allocate and register the memory
            MemoryU8 ptr = allocate_memory(dev, 0, name, width, bytes);

            // allocate the region
            //return m_sharelist.emplace(name, std::make_unique<memory_share>(name, width, bytes, endianness, ptr)).first->second.get();
            var share = new memory_share(name, width, bytes, endianness, new PointerU8(ptr));
            m_sharelist.emplace(name, share);
            return share;
        }


        //-------------------------------------------------
        //  share_find - find a share by name
        //-------------------------------------------------
        public memory_share share_find(string name)
        {
            var i = m_sharelist.find(name);
            return i != default ? i : null;
        }


        // banks

        //-------------------------------------------------
        //  share_alloc - allocates a banking zone
        //-------------------------------------------------
        public memory_bank bank_alloc(device_t device, string name)
        {
            // allocate the bank
            //auto const ins = m_banklist.emplace(name, std::make_unique<memory_bank>(device, name));
            var bank = new memory_bank(device, name);
            var ins = m_banklist.emplace(name, bank);

            // make sure we don't have a bank of the same name
            if (!ins)
                fatalerror("bank_alloc called with duplicate bank name \"{0}\"\n", name);

            return bank;  //return ins.first->second.get();
        }


        //-------------------------------------------------
        //  bank_find - find a bank by name
        //-------------------------------------------------
        public memory_bank bank_find(string name)
        {
            var i = m_banklist.find(name);
            return i != default ? i : null;
        }


        // regions

        //-------------------------------------------------
        //  region_alloc - allocates memory for a region
        //-------------------------------------------------
        public memory_region region_alloc(string name, u32 length, u8 width, endianness_t endian)
        {
            // make sure we don't have a region of the same name; also find the end of the list
            if (m_regionlist.find(name) != default)
                fatalerror("region_alloc called with duplicate region name \"{0}\"\n", name);

            // allocate the region
            //return m_regionlist.emplace(name, std::make_unique<memory_region>(machine(), name, length, width, endian)).first->second.get();
            var region = new memory_region(machine(), name, length, width, endian);
            m_regionlist.emplace(name, region);
            return region;
        }


        //memory_region *region_find(std::string name);
        //void region_free(std::string name);


        // Allocate the address spaces
        //-------------------------------------------------
        //  allocate - allocate memory spaces
        //-------------------------------------------------
        void allocate(device_memory_interface memory)
        {
            for (int spacenum = 0; spacenum < memory.max_space_count(); ++spacenum)
            {
                // if there is a configuration for this space, we need an address space
                address_space_config spaceconfig = memory.space_config(spacenum);
                if (spaceconfig != null)
                {
                    int level = emumem_global.handler_entry_dispatch_level(spaceconfig.addr_width());
                    // allocate one of the appropriate type
                    switch ((level << 8) | (spaceconfig.endianness() == endianness_t.ENDIANNESS_BIG ? 0x1000 : 0) |spaceconfig.data_width() | (spaceconfig.addr_shift() + 4))
                    {
                        case 0x0000|0x000| 8|(4+1): memory.allocate(new address_space_specific<int_constant_0, int_constant_0, int_constant_1, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x000| 8|(4+1): memory.allocate(new address_space_specific<int_constant_0, int_constant_0, int_constant_1, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x0000|0x100| 8|(4+1): memory.allocate(new address_space_specific<int_constant_1, int_constant_0, int_constant_1, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x100| 8|(4+1): memory.allocate(new address_space_specific<int_constant_1, int_constant_0, int_constant_1, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;

                        case 0x0000|0x000| 8|(4-0): memory.allocate(new address_space_specific<int_constant_0, int_constant_0,  int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x000| 8|(4-0): memory.allocate(new address_space_specific<int_constant_0, int_constant_0,  int_constant_0, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x0000|0x100| 8|(4-0): memory.allocate(new address_space_specific<int_constant_1, int_constant_0,  int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x100| 8|(4-0): memory.allocate(new address_space_specific<int_constant_1, int_constant_0,  int_constant_0, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;

                        case 0x0000|0x000|16|(4+3): memory.allocate(new address_space_specific<int_constant_0, int_constant_1,  int_constant_3, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x000|16|(4+3): memory.allocate(new address_space_specific<int_constant_0, int_constant_1,  int_constant_3, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x0000|0x100|16|(4+3): memory.allocate(new address_space_specific<int_constant_1, int_constant_1,  int_constant_3, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x100|16|(4+3): memory.allocate(new address_space_specific<int_constant_1, int_constant_1,  int_constant_3, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;

                        case 0x0000|0x000|16|(4-0): memory.allocate(new address_space_specific<int_constant_0, int_constant_1,  int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x000|16|(4-0): memory.allocate(new address_space_specific<int_constant_0, int_constant_1,  int_constant_0, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x0000|0x100|16|(4-0): memory.allocate(new address_space_specific<int_constant_1, int_constant_1,  int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x100|16|(4-0): memory.allocate(new address_space_specific<int_constant_1, int_constant_1,  int_constant_0, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;

                        case 0x0000|0x000|16|(4-1): memory.allocate(new address_space_specific<int_constant_0, int_constant_1, int_constant_n1, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x000|16|(4-1): memory.allocate(new address_space_specific<int_constant_0, int_constant_1, int_constant_n1, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x0000|0x100|16|(4-1): memory.allocate(new address_space_specific<int_constant_1, int_constant_1, int_constant_n1, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x100|16|(4-1): memory.allocate(new address_space_specific<int_constant_1, int_constant_1, int_constant_n1, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;

                        case 0x0000|0x000|32|(4+3): memory.allocate(new address_space_specific<int_constant_0, int_constant_2,  int_constant_3, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x000|32|(4+3): memory.allocate(new address_space_specific<int_constant_0, int_constant_2,  int_constant_3, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x0000|0x100|32|(4+3): memory.allocate(new address_space_specific<int_constant_1, int_constant_2,  int_constant_3, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x100|32|(4+3): memory.allocate(new address_space_specific<int_constant_1, int_constant_2,  int_constant_3, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;

                        case 0x0000|0x000|32|(4-0): memory.allocate(new address_space_specific<int_constant_0, int_constant_2,  int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x000|32|(4-0): memory.allocate(new address_space_specific<int_constant_0, int_constant_2,  int_constant_0, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x0000|0x100|32|(4-0): memory.allocate(new address_space_specific<int_constant_1, int_constant_2,  int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x100|32|(4-0): memory.allocate(new address_space_specific<int_constant_1, int_constant_2,  int_constant_0, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;

                        case 0x0000|0x000|32|(4-1): memory.allocate(new address_space_specific<int_constant_0, int_constant_2, int_constant_n1, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x000|32|(4-1): memory.allocate(new address_space_specific<int_constant_0, int_constant_2, int_constant_n1, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x0000|0x100|32|(4-1): memory.allocate(new address_space_specific<int_constant_1, int_constant_2, int_constant_n1, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x100|32|(4-1): memory.allocate(new address_space_specific<int_constant_1, int_constant_2, int_constant_n1, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;

                        case 0x0000|0x000|32|(4-2): memory.allocate(new address_space_specific<int_constant_0, int_constant_2, int_constant_n2, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x000|32|(4-2): memory.allocate(new address_space_specific<int_constant_0, int_constant_2, int_constant_n2, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x0000|0x100|32|(4-2): memory.allocate(new address_space_specific<int_constant_1, int_constant_2, int_constant_n2, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x100|32|(4-2): memory.allocate(new address_space_specific<int_constant_1, int_constant_2, int_constant_n2, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;

                        case 0x0000|0x000|64|(4-0): memory.allocate(new address_space_specific<int_constant_0, int_constant_3, int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x000|64|(4-0): memory.allocate(new address_space_specific<int_constant_0, int_constant_3, int_constant_0, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x0000|0x100|64|(4-0): memory.allocate(new address_space_specific<int_constant_1, int_constant_3, int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x100|64|(4-0): memory.allocate(new address_space_specific<int_constant_1, int_constant_3, int_constant_0, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;

                        case 0x0000|0x000|64|(4-1): memory.allocate(new address_space_specific<int_constant_0, int_constant_3, int_constant_n1, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x000|64|(4-1): memory.allocate(new address_space_specific<int_constant_0, int_constant_3, int_constant_n1, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x0000|0x100|64|(4-1): memory.allocate(new address_space_specific<int_constant_1, int_constant_3, int_constant_n1, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x100|64|(4-1): memory.allocate(new address_space_specific<int_constant_1, int_constant_3, int_constant_n1, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;

                        case 0x0000|0x000|64|(4-2): memory.allocate(new address_space_specific<int_constant_0, int_constant_3, int_constant_n2, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x000|64|(4-2): memory.allocate(new address_space_specific<int_constant_0, int_constant_3, int_constant_n2, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x0000|0x100|64|(4-2): memory.allocate(new address_space_specific<int_constant_1, int_constant_3, int_constant_n2, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x100|64|(4-2): memory.allocate(new address_space_specific<int_constant_1, int_constant_3, int_constant_n2, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;

                        case 0x0000|0x000|64|(4-3): memory.allocate(new address_space_specific<int_constant_0, int_constant_3, int_constant_n3, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x000|64|(4-3): memory.allocate(new address_space_specific<int_constant_0, int_constant_3, int_constant_n3, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x0000|0x100|64|(4-3): memory.allocate(new address_space_specific<int_constant_1, int_constant_3, int_constant_n3, endianness_t_constant_ENDIANNESS_LITTLE>(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;
                        case 0x1000|0x100|64|(4-3): memory.allocate(new address_space_specific<int_constant_1, int_constant_3, int_constant_n3, endianness_t_constant_ENDIANNESS_BIG   >(this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum); break;

                        default:
                            throw new emu_fatalerror("Invalid width {0}/shift {1} specified for address_space::allocate", spaceconfig.data_width(), spaceconfig.addr_shift());
                    }
                }
            }
        }


        // Allocate some ram and register it for saving
        //-------------------------------------------------
        //  allocate_memory - allocate some ram and register it for saving
        //-------------------------------------------------
        MemoryU8 allocate_memory(device_t dev, int spacenum, string name, u8 width, size_t bytes)
        {
            //void *const ptr = m_datablocks.emplace_back(malloc(bytes)).get();
            MemoryU8 ptr = new MemoryU8((int)bytes);
            m_datablocks.emplace_back(ptr);

            global_object.memset(ptr, (u8)0);

            //throw new emu_unimplemented();
#if false
            machine().save().save_memory(&dev, "memory", dev.tag(), spacenum, name.c_str(), ptr, width/8, u32(bytes) / (width/8));
#endif

            return ptr;
        }
    }
}
