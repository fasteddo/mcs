// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Reflection;

using MemoryU8 = mame.MemoryContainer<System.Byte>;
using offs_t = System.UInt32;
using PointerU8 = mame.Pointer<System.Byte>;
using s8  = System.SByte;
using s32 = System.Int32;
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
    namespace emu { namespace detail {

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


        public static void VPRINTF(string format, params object [] args) { if (VERBOSE) global_object.osd_printf_info(format, args); }


        // =====================-> Width -> types

        //template<int Width> struct handler_entry_size {};
        //template<> struct handler_entry_size<0> { using uX = u8;  };
        //template<> struct handler_entry_size<1> { using uX = u16; };
        //template<> struct handler_entry_size<2> { using uX = u32; };
        //template<> struct handler_entry_size<3> { using uX = u64; };


        // ======================> address offset -> byte offset

        public static offs_t memory_offset_to_byte(offs_t offset, int AddrShift) { return AddrShift < 0 ? offset << global_object.iabs(AddrShift) : offset >> global_object.iabs(AddrShift); }


        // ======================> generic read/write decomposition routines

        // generic direct read
        //template<int Width, int AddrShift, endianness_t Endian, int TargetWidth, bool Aligned, typename T>
        public static uX memory_read_generic(int Width, int AddrShift, endianness_t Endian, int TargetWidth, bool Aligned, Func<offs_t, uX, uX> rop, offs_t address, uX mask)  //typename emu::detail::handler_entry_size<TargetWidth>::uX  memory_read_generic(T rop, offs_t address, typename emu::detail::handler_entry_size<TargetWidth>::uX mask)
        {
            //using TargetType = typename emu::detail::handler_entry_size<TargetWidth>::uX;
            //using NativeType = typename emu::detail::handler_entry_size<Width>::uX;

            u32 TARGET_BYTES = 1U << TargetWidth;
            u32 TARGET_BITS = 8 * TARGET_BYTES;
            u32 NATIVE_BYTES = 1U << Width;
            u32 NATIVE_BITS = 8 * NATIVE_BYTES;
            u32 NATIVE_STEP = AddrShift >= 0 ? NATIVE_BYTES << global_object.iabs(AddrShift) : NATIVE_BYTES >> global_object.iabs(AddrShift);
            u32 NATIVE_MASK = Width + AddrShift >= 0 ? global_object.make_bitmask32(Width + AddrShift) : 0;

            // equal to native size and aligned; simple pass-through to the native reader
            if (NATIVE_BYTES == TARGET_BYTES && (Aligned || (address & NATIVE_MASK) == 0))
                return new uX(TargetWidth, rop(address & ~NATIVE_MASK, mask));

            // if native size is larger, see if we can do a single masked read (guaranteed if we're aligned)
            if (NATIVE_BYTES > TARGET_BYTES)
            {
                u32 offsbits2 = 8 * (memory_offset_to_byte(address, AddrShift) & (NATIVE_BYTES - (Aligned ? TARGET_BYTES : 1)));  // renamed due to dup var name below
                if (Aligned || (offsbits2 + TARGET_BITS <= NATIVE_BITS))
                {
                    if (Endian != endianness_t.ENDIANNESS_LITTLE) offsbits2 = NATIVE_BITS - TARGET_BITS - offsbits2;
                    return new uX(TargetWidth, rop(address & ~NATIVE_MASK, new uX(Width, mask) << (int)offsbits2) >> (int)offsbits2);  //return rop(address & ~NATIVE_MASK, (NativeType)mask << offsbits) >> offsbits;
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
                    curmask = mask >> (int)offsbits;
                    if (curmask != 0) result |= rop(address + NATIVE_STEP, curmask) << (int)offsbits;
                    return result;
                }

                // big-endian case
                else
                {
                    // left-justify the mask to the target type
                    u32 LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT = ((NATIVE_BITS >= TARGET_BITS) ? (NATIVE_BITS - TARGET_BITS) : 0);
                    uX result = new uX(Width, 0);  //NativeType result = 0;
                    uX ljmask = new uX(Width, mask << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);  //NativeType ljmask = (NativeType)mask << LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT;
                    uX curmask = new uX(Width, ljmask >> (int)offsbits);  //NativeType curmask = ljmask >> offsbits;

                    // read upper bits from lower address
                    if (curmask != 0) result = rop(address, curmask) << (int)offsbits;
                    offsbits = NATIVE_BITS - offsbits;

                    // read lower bits from upper address
                    curmask = ljmask << (int)offsbits;
                    if (curmask != 0) result |= rop(address + NATIVE_STEP, curmask) >> (int)offsbits;

                    // return the un-justified result
                    return new uX(TargetWidth, result >> (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
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
                    if (curmask != 0) result = rop(address, curmask) >> (int)offsbits;

                    // read middle bits from subsequent addresses
                    offsbits = NATIVE_BITS - offsbits;
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        address += NATIVE_STEP;
                        curmask = mask >> (int)offsbits;
                        if (curmask != 0) result |= new uX(TargetWidth, rop(address, curmask)) << (int)offsbits;  //if (curmask != 0) result |= (TargetType)rop(address, curmask) << offsbits;
                        offsbits += NATIVE_BITS;
                    }

                    // if we're not aligned and we still have bits left, read uppermost bits from last address
                    if (!Aligned && offsbits < TARGET_BITS)
                    {
                        curmask = mask >> (int)offsbits;
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
                        curmask = mask >> (int)offsbits;
                        if (curmask != 0) result |= new uX(TargetWidth, rop(address, curmask)) << (int)offsbits;  //if (curmask != 0) result |= (TargetType)rop(address, curmask) << offsbits;
                    }

                    // if we're not aligned and we still have bits left, read lowermost bits from the last address
                    if (!Aligned && offsbits != 0)
                    {
                        offsbits = NATIVE_BITS - offsbits;
                        curmask = mask << (int)offsbits;
                        if (curmask != 0) result |= rop(address + NATIVE_STEP, curmask) >> (int)offsbits;
                    }
                }

                return result;
            }
        }


        // generic direct write
        //template<int Width, int AddrShift, endianness_t Endian, int TargetWidth, bool Aligned, typename T>
        public static void memory_write_generic(int Width, int AddrShift, endianness_t Endian, int TargetWidth, bool Aligned, Action<offs_t, uX, uX> wop, offs_t address, uX data, uX mask)  //void memory_write_generic(T wop, offs_t address, typename emu::detail::handler_entry_size<TargetWidth>::uX data, typename emu::detail::handler_entry_size<TargetWidth>::uX mask)
        {
            //using NativeType = typename emu::detail::handler_entry_size<Width>::uX;

            u32 TARGET_BYTES = 1U << TargetWidth;
            u32 TARGET_BITS = 8 * TARGET_BYTES;
            u32 NATIVE_BYTES = 1U << Width;
            u32 NATIVE_BITS = 8 * NATIVE_BYTES;
            u32 NATIVE_STEP = AddrShift >= 0 ? NATIVE_BYTES << global_object.iabs(AddrShift) : NATIVE_BYTES >> global_object.iabs(AddrShift);
            u32 NATIVE_MASK = Width + AddrShift >= 0 ? (1U << (Width + AddrShift)) - 1 : 0;

            // equal to native size and aligned; simple pass-through to the native writer
            if (NATIVE_BYTES == TARGET_BYTES && (Aligned || (address & NATIVE_MASK) == 0))
            {
                wop(address & ~NATIVE_MASK, data, mask);
                return;
            }

            // if native size is larger, see if we can do a single masked write (guaranteed if we're aligned)
            if (NATIVE_BYTES > TARGET_BYTES)
            {
                u32 offsbits2 = 8 * (memory_offset_to_byte(address, AddrShift) & (NATIVE_BYTES - (Aligned ? TARGET_BYTES : 1)));  // renamed due to dup var name below
                if (Aligned || (offsbits2 + TARGET_BITS <= NATIVE_BITS))
                {
                    if (Endian != endianness_t.ENDIANNESS_LITTLE) offsbits2 = NATIVE_BITS - TARGET_BITS - offsbits2;
                    wop(address & ~NATIVE_MASK, new uX(Width, data) << (int)offsbits2, new uX(Width, mask) << (int)offsbits2);  //return wop(address & ~NATIVE_MASK, (NativeType)data << offsbits, (NativeType)mask << offsbits);
                    return;
                }
            }

            // determine our alignment against the native boundaries, and mask the address
            u32 offsbits = 8 * (memory_offset_to_byte(address, AddrShift) & (NATIVE_BYTES - 1));
            address &= ~NATIVE_MASK;

            // if we're here, and native size is larger or equal to the target, we need exactly 2 writes
            if (NATIVE_BYTES >= TARGET_BYTES)
            {
                // little-endian case
                if (Endian == endianness_t.ENDIANNESS_LITTLE)
                {
                    // write lower bits to lower address
                    uX curmask = new uX(Width, mask) << (int)offsbits;  //NativeType curmask = (NativeType)mask << offsbits;
                    if (curmask != 0) wop(address, new uX(Width, data) << (int)offsbits, curmask);  //if (curmask != 0) wop(address, (NativeType)data << offsbits, curmask);

                    // write upper bits to upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = mask >> (int)offsbits;
                    if (curmask != 0) wop(address + NATIVE_STEP, data >> (int)offsbits, curmask);
                }

                // big-endian case
                else
                {
                    // left-justify the mask and data to the target type
                    u32 LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT = ((NATIVE_BITS >= TARGET_BITS) ? (NATIVE_BITS - TARGET_BITS) : 0);
                    uX ljdata = new uX(Width, data) << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT;  //NativeType ljdata = (NativeType)data << LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT;
                    uX ljmask = new uX(Width, mask) << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT;  //NativeType ljmask = (NativeType)mask << LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT;
                    // write upper bits to lower address
                    uX curmask = ljmask >> (int)offsbits;  //NativeType curmask = ljmask >> offsbits;
                    if (curmask != 0) wop(address, ljdata >> (int)offsbits, curmask);
                        // write lower bits to upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = ljmask << (int)offsbits;
                    if (curmask != 0) wop(address + NATIVE_STEP, ljdata << (int)offsbits, curmask);
                }
            }

            // if we're here, then we have 2 or more writes needed to get our final result
            else
            {
                // compute the maximum number of loops; we do it this way so that there are
                // a fixed number of loops for the compiler to unroll if it desires
                u32 MAX_SPLITS_MINUS_ONE = TARGET_BYTES / NATIVE_BYTES - 1;

                // little-endian case
                if (Endian == endianness_t.ENDIANNESS_LITTLE)
                {
                    // write lowest bits to first address
                    uX curmask = mask << (int)offsbits;  //NativeType curmask = mask << offsbits;
                    if (curmask != 0) wop(address, data << (int)offsbits, curmask);

                    // write middle bits to subsequent addresses
                    offsbits = NATIVE_BITS - offsbits;
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        address += NATIVE_STEP;
                        curmask = mask >> (int)offsbits;
                        if (curmask != 0) wop(address, data >> (int)offsbits, curmask);
                        offsbits += NATIVE_BITS;
                    }

                    // if we're not aligned and we still have bits left, write uppermost bits to last address
                    if (!Aligned && offsbits < TARGET_BITS)
                    {
                        curmask = mask >> (int)offsbits;
                        if (curmask != 0) wop(address + NATIVE_STEP, data >> (int)offsbits, curmask);
                    }
                }

                // big-endian case
                else
                {
                    // write highest bits to first address
                    offsbits = TARGET_BITS - (NATIVE_BITS - offsbits);
                    uX curmask = mask >> (int)offsbits;  //NativeType curmask = mask >> offsbits;
                    if (curmask != 0) wop(address, data >> (int)offsbits, curmask);

                    // write middle bits to subsequent addresses
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        offsbits -= NATIVE_BITS;
                        address += NATIVE_STEP;
                        curmask = mask >> (int)offsbits;
                        if (curmask != 0) wop(address, data >> (int)offsbits, curmask);
                    }

                    // if we're not aligned and we still have bits left, write lowermost bits to the last address
                    if (!Aligned && offsbits != 0)
                    {
                        offsbits = NATIVE_BITS - offsbits;
                        curmask = mask << (int)offsbits;
                        if (curmask != 0) wop(address + NATIVE_STEP, data << (int)offsbits, curmask);
                    }
                }
            }
        }


        // ======================> Direct dispatching

        public static uX dispatch_read(int Level, int Width, int AddrShift, endianness_t Endian, offs_t mask, offs_t offset, uX mem_mask, handler_entry_read [] dispatch)  //template<int Level, int Width, int AddrShift, endianness_t Endian> typename emu::detail::handler_entry_size<Width>::uX dispatch_read(offs_t mask, offs_t offset, typename emu::detail::handler_entry_size<Width>::uX mem_mask, const handler_entry_read<Width, AddrShift, Endian> *const *dispatch)
        {
            u32 LowBits = (u32)emumem_global.handler_entry_dispatch_level_to_lowbits(Level, Width, AddrShift);  //static constexpr u32 LowBits  = emu::detail::handler_entry_dispatch_level_to_lowbits(Level, Width, AddrShift);
            return dispatch[(offset & mask) >> (int)LowBits].read(Width, AddrShift, Endian, offset, mem_mask);  //return dispatch[(offset & mask) >> LowBits]->read(offset, mem_mask);
        }


        public static void dispatch_write(int Level, int Width, int AddrShift, endianness_t Endian, offs_t mask, offs_t offset, uX data, uX mem_mask, handler_entry_write [] dispatch)  //template<int Level, int Width, int AddrShift, endianness_t Endian> void dispatch_write(offs_t mask, offs_t offset, typename emu::detail::handler_entry_size<Width>::uX data, typename emu::detail::handler_entry_size<Width>::uX mem_mask, const handler_entry_write<Width, AddrShift, Endian> *const *dispatch)
        {
            u32 LowBits = (u32)emumem_global.handler_entry_dispatch_level_to_lowbits(Level, Width, AddrShift);  //static constexpr u32 LowBits  = emu::detail::handler_entry_dispatch_level_to_lowbits(Level, Width, AddrShift);
            dispatch[(offset & mask) >> (int)LowBits].write(Width, AddrShift, Endian, offset, data, mem_mask);  //return dispatch[(offset & mask) >> LowBits]->write(offset, data, mem_mask);
        }


        /*-------------------------------------------------
            core_i64_hex_format - i64 format printf helper
        -------------------------------------------------*/
        static string [] core_i64_hex_format_buffer = new string[16];
        static int core_i64_hex_format_index;
        public static string core_i64_hex_format(u64 value, u8 mindigits)
        {
            //static char [,] buffer = new char[16, 64];

            // TODO: this can overflow - e.g. when a lot of unmapped writes are logged
            //static int index;
            //char *bufbase = &buffer[index++ % 16][0];
            int bufbaseIdx = core_i64_hex_format_index++ % 16;
            //char *bufptr = bufbase;
            core_i64_hex_format_buffer[bufbaseIdx] = "";

            s8 curdigit;
            for (curdigit = 15; curdigit >= 0; curdigit--)
            {
                int nibble = (int)((value >> (curdigit * 4)) & 0xf);
                if (nibble != 0 || curdigit < mindigits)
                {
                    mindigits = (u8)curdigit;
                    //*bufptr++ = "0123456789ABCDEF"[nibble];
                    core_i64_hex_format_buffer[bufbaseIdx] += "0123456789ABCDEF"[nibble];
                }
            }

            //if (bufptr == bufbase)
            //    *bufptr++ = '0';
            //*bufptr = 0;

            return core_i64_hex_format_buffer[bufbaseIdx];
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
    }


    // a line in the memory structure dump
    public struct memory_entry
    {
        offs_t start;
        offs_t end;
        handler_entry entry;
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


        // Start/end of range flags
        public const u8 START = 1;
        public const u8 END   = 2;


        // Intermediary structure for reference count checking
        protected class reflist
        {
            //std::unordered_map<const handler_entry *, u32> refcounts;
            //std::unordered_set<const handler_entry *> seen;
            //std::unordered_set<const handler_entry *> todo;


            //void add(handler_entry entry);

            //void propagate();
            //void check();
        }


        // Address range storage
        public class range
        {
            public offs_t start;
            public offs_t end;

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
        public bool is_units() { return (m_flags & F_UNITS) != 0; }
        //inline bool is_passthrough() const { return m_flags & F_PASSTHROUGH; }


        protected virtual void dump_map(std.vector<memory_entry> map)
        {
            fatalerror("dump_map called on non-dispatching class\n");
        }

        protected abstract string name();

        protected virtual void enumerate_references(reflist refs) { }


        //u32 get_refcount() const { return m_refcount; }
    }


    // =====================-> The parent class of all read handlers

    // Provides the populate/read/get_ptr/lookup API

    //template<int Width, int AddrShift, endianness_t Endian> class handler_entry_read_passthrough;

    //template<int Width, int AddrShift, endianness_t Endian> 
    public abstract class handler_entry_read : handler_entry
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        public class mapping
        {
            public handler_entry_read original;
            public handler_entry_read patched;
            public u8 ukey;

            public mapping(handler_entry_read original, handler_entry_read patched, u8 ukey) { this.original = original; this.patched = patched; this.ukey = ukey; }
        }


        // template parameters
        protected int Width;
        protected int AddrShift;
        protected endianness_t Endian;

        protected u32 NATIVE_MASK;  //static constexpr u32 NATIVE_MASK = Width + AddrShift >= 0 ? make_bitmask<u32>(Width + AddrShift) : 0;


        public handler_entry_read(int Width, int AddrShift, endianness_t Endian, address_space space, u32 flags)
            : base(space, flags)
        {
            this.Width = Width;
            this.AddrShift = AddrShift;
            this.Endian = Endian;

            NATIVE_MASK = Width + AddrShift >= 0 ? make_bitmask32(Width + AddrShift) : 0;
        }

        //~handler_entry_read() {}


        public abstract uX read(int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX mem_mask);


        protected virtual object get_ptr(offs_t offset) { return null; }  //virtual void *get_ptr(offs_t offset) const;


        public virtual void lookup(offs_t address, ref offs_t start, ref offs_t end, ref handler_entry_read handler)
        {
            fatalerror("lookup called on non-dispatching class\n");
        }


        public void populate(offs_t start, offs_t end, offs_t mirror, handler_entry_read handler)
        {
            start &= ~NATIVE_MASK;
            end |= NATIVE_MASK;

            if (mirror != 0)
                populate_mirror(start, end, start, end, mirror, handler);
            else
                populate_nomirror(start, end, start, end, handler);
        }


        public virtual void populate_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read handler)
        {
            fatalerror("populate called on non-dispatching class\n");
        }

        public virtual void populate_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read handler)
        {
            fatalerror("populate called on non-dispatching class\n");
        }


        public void populate_mismatched(offs_t start, offs_t end, offs_t mirror, memory_units_descriptor descriptor)
        {
            start &= ~NATIVE_MASK;
            end |= NATIVE_MASK;

            std.vector<mapping> mappings = new std.vector<mapping>();
            if (mirror != 0)
                populate_mismatched_mirror(start, end, start, end, mirror, descriptor, mappings);
            else
                populate_mismatched_nomirror(start, end, start, end, descriptor, START|END, mappings);
        }

        public virtual void populate_mismatched_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, memory_units_descriptor descriptor, u8 rkey, std.vector<mapping> mappings)
        {
            fatalerror("populate_mismatched called on non-dispatching class\n");
        }

        public virtual void populate_mismatched_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, memory_units_descriptor descriptor, std.vector<mapping> mappings)
        {
            fatalerror("populate_mismatched called on non-dispatching class\n");
        }


        //inline void populate_passthrough(offs_t start, offs_t end, offs_t mirror, handler_entry_read_passthrough<Width, AddrShift, Endian> *handler) {
        //    start &= ~NATIVE_MASK;
        //    end |= NATIVE_MASK;
        //    std::vector<mapping> mappings;
        //    if(mirror)
        //        populate_passthrough_mirror(start, end, start, end, mirror, handler, mappings);
        //    else
        //        populate_passthrough_nomirror(start, end, start, end, handler, mappings);
        //}

        protected virtual void populate_passthrough_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read_passthrough handler, std.vector<mapping> mappings)
        {
            fatalerror("populate_passthrough called on non-dispatching class\n");
        }


        protected virtual void populate_passthrough_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read_passthrough handler, std.vector<mapping> mappings)
        {
            fatalerror("populate_passthrough called on non-dispatching class\n");
        }


        // Remove a set of passthrough handlers, leaving the lower handler in their place
        protected virtual void detach(std.unordered_set<handler_entry> handlers)  //virtual void detach(const std::unordered_set<handler_entry *> &handlers);
        {
            fatalerror("detach called on non-dispatching class\n");
        }


        // Return the internal structures of the root dispatch
        public virtual handler_entry_read [] get_dispatch()  //virtual const handler_entry_read<Width, AddrShift, Endian> *const *get_dispatch() const;
        {
            fatalerror("get_dispatch called on non-dispatching class\n");
            return null;
        }
    }


    // =====================-> The parent class of all write handlers

    // Provides the populate/write/get_ptr/lookup API

    //template<int Width, int AddrShift, endianness_t Endian> class handler_entry_write_passthrough;

    //template<int Width, int AddrShift, endianness_t Endian>
    public abstract class handler_entry_write : handler_entry
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;


        public class mapping
        {
            public handler_entry_write original;
            public handler_entry_write patched;
            public u8 ukey;

            public mapping(handler_entry_write original, handler_entry_write patched, u8 ukey) { this.original = original; this.patched = patched; this.ukey = ukey; }
        }


        // template parameters
        protected int Width;
        protected int AddrShift;
        protected endianness_t Endian;

        protected u32 NATIVE_MASK;  //static constexpr u32 NATIVE_MASK = Width + AddrShift >= 0 ? make_bitmask<u32>(Width + AddrShift) : 0;


        public handler_entry_write(int Width, int AddrShift, endianness_t Endian, address_space space, u32 flags)
            : base(space, flags)
        {
            this.Width = Width;
            this.AddrShift = AddrShift;
            this.Endian = Endian;

            NATIVE_MASK = Width + AddrShift >= 0 ? make_bitmask32(Width + AddrShift) : 0;
        }

        //~handler_entry_write() {}


        public abstract void write(int WidthOverride, int AddrShiftOverride, endianness_t EndianOverride, offs_t offset, uX data, uX mem_mask);


        protected virtual object get_ptr(offs_t offset) { return null; }  //virtual void *get_ptr(offs_t offset) const;


        protected virtual void lookup(offs_t address, ref offs_t start, ref offs_t end, ref handler_entry_write handler)
        {
            fatalerror("lookup called on non-dispatching class\n");
        }


        public void populate(offs_t start, offs_t end, offs_t mirror, handler_entry_write handler)
        {
            start &= ~NATIVE_MASK;
            end |= NATIVE_MASK;

            if (mirror != 0)
                populate_mirror(start, end, start, end, mirror, handler);
            else
                populate_nomirror(start, end, start, end, handler);
        }


        public virtual void populate_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_write handler)
        {
            fatalerror("populate called on non-dispatching class\n");
        }

        public virtual void populate_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_write handler)
        {
            fatalerror("populate called on non-dispatching class\n");
        }


        public void populate_mismatched(offs_t start, offs_t end, offs_t mirror, memory_units_descriptor descriptor)
        {
            start &= ~NATIVE_MASK;
            end |= NATIVE_MASK;

            std.vector<mapping> mappings = new std.vector<mapping>();
            if (mirror != 0)
                populate_mismatched_mirror(start, end, start, end, mirror, descriptor, mappings);
            else
                populate_mismatched_nomirror(start, end, start, end, descriptor, START|END, mappings);
        }


        public virtual void populate_mismatched_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, memory_units_descriptor descriptor, u8 rkey, std.vector<mapping> mappings)
        {
            fatalerror("populate_mismatched called on non-dispatching class\n");
        }


        public virtual void populate_mismatched_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, memory_units_descriptor descriptor, std.vector<mapping> mappings)
        {
            fatalerror("populate_mismatched called on non-dispatching class\n");
        }


        //inline void populate_passthrough(offs_t start, offs_t end, offs_t mirror, handler_entry_write_passthrough<Width, AddrShift, Endian> *handler) {
        //    start &= ~NATIVE_MASK;
        //    end |= NATIVE_MASK;
        //    std::vector<mapping> mappings;
        //    if(mirror)
        //        populate_passthrough_mirror(start, end, start, end, mirror, handler, mappings);
        //    else
        //        populate_passthrough_nomirror(start, end, start, end, handler, mappings);
        //}

        protected virtual void populate_passthrough_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_write_passthrough handler, std.vector<mapping> mappings)
        {
            fatalerror("populate_passthrough called on non-dispatching class\n");
        }

        protected virtual void populate_passthrough_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_write_passthrough handler, std.vector<mapping> mappings)
        {
            fatalerror("populate_passthrough called on non-dispatching class\n");
        }


        // Remove a set of passthrough handlers, leaving the lower handler in their place
        protected virtual void detach(std.unordered_set<handler_entry> handlers)
        {
            fatalerror("detach called on non-dispatching class\n");
        }


        // Return the internal structures of the root dispatch
        public virtual handler_entry_write [] get_dispatch()  //virtual const handler_entry_write<Width, AddrShift, Endian> *const *get_dispatch() const;
        {
            fatalerror("get_dispatch called on non-dispatching class\n");
            return null;
        }
    }


    // =====================-> Passthrough handler management structure
    class memory_passthrough_handler
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

    namespace emu { namespace detail {


    //template<int Level, int Width, int AddrShift, endianness_t Endian>
    public class memory_access_specific
    {
        //friend class ::address_space;

        //using NativeType = typename emu::detail::handler_entry_size<Width>::uX;
        //static constexpr u32 NATIVE_BYTES = 1 << Width;
        u32 NATIVE_MASK;  //static constexpr u32 NATIVE_MASK = Width + AddrShift >= 0 ? (1 << (Width + AddrShift)) - 1 : 0;


        // template parameters
        public int Level;
        public int Width;
        public int AddrShift;
        public endianness_t Endian;


        address_space m_space;

        offs_t m_addrmask;                // address mask

        handler_entry_read [] m_dispatch_read;  //const handler_entry_read<Width, AddrShift, Endian> *const *m_dispatch_read;
        handler_entry_write [] m_dispatch_write;  //const handler_entry_write<Width, AddrShift, Endian> *const *m_dispatch_write;


        // construction/destruction
        public memory_access_specific(int Level, int Width, int AddrShift, endianness_t Endian)
        {
            this.Level = Level;
            this.Width = Width;
            this.AddrShift = AddrShift;
            this.Endian = Endian;


            NATIVE_MASK = Width + AddrShift >= 0 ? (1U << (Width + AddrShift)) - 1 : 0U;


            m_space = null;
            m_addrmask = 0;
            m_dispatch_read = null;
            m_dispatch_write = null;
        }

        //inline address_space &space() const {
        //    return *m_space;
        //}


        public u8 read_byte(offs_t address) { return Width == 0 ? read_native(address & ~NATIVE_MASK).x8 : emumem_global.memory_read_generic(Width, AddrShift, Endian, 0, true, (offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(0, 0xff)).x8; }  //u8 read_byte(offs_t address) { return Width == 0 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 0, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xff); }
        public u16 read_word(offs_t address) { return Width == 1 ? read_native(address & ~NATIVE_MASK).x8 : emumem_global.memory_read_generic(Width, AddrShift, Endian, 1, true, (offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(1, 0xffff)).x16; }  //u16 read_word(offs_t address) { return Width == 1 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffff); }
        public u16 read_word(offs_t address, u16 mask) { return emumem_global.memory_read_generic(Width, AddrShift, Endian, 1, true, (offs_t offset, uX mask2) => { return read_native(offset, mask2); }, address, new uX(1, mask)).x16; }  //u16 read_word(offs_t address, u16 mask) { return memory_read_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
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

        public void write_byte(offs_t address, u8 data) { if (Width == 0) write_native(address & ~NATIVE_MASK, new uX(0, data)); else emumem_global.memory_write_generic(Width, AddrShift, Endian, 0, true, (offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(0, data), new uX(0, 0xff)); }  //void write_byte(offs_t address, u8 data) { if (Width == 0) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 0, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xff); }
        public void write_word(offs_t address, u16 data) { if (Width == 1) write_native(address & ~NATIVE_MASK, new uX(1, data)); else emumem_global.memory_write_generic(Width, AddrShift, Endian, 1, true, (offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(1, data), new uX(1, 0xffff)); }  //void write_word(offs_t address, u16 data) { if (Width == 1) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffff); }
        public void write_word(offs_t address, u16 data, u16 mask) { emumem_global.memory_write_generic(Width, AddrShift, Endian, 1, true, (offs_t offset, uX data2, uX mask2) => { write_native(offset, data2, mask2); }, address, new uX(1, data), new uX(1, mask)); }  //void write_word(offs_t address, u16 data, u16 mask) { memory_write_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
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


        uX read_native(offs_t address) { return read_native(address, uX.MaxValue(Width)); }
        uX read_native(offs_t address, uX mask)  //NativeType read_native(offs_t address, NativeType mask = ~NativeType(0)) {
        {
            return emumem_global.dispatch_read(Level, Width, AddrShift, Endian, offs_t.MaxValue, address & m_addrmask, mask, m_dispatch_read);  //return dispatch_read<Level, Width, AddrShift, Endian>(offs_t(-1), address & m_addrmask, mask, m_dispatch_read);;
        }


        void write_native(offs_t address, uX data) { write_native(address, uX.MaxValue(Width)); }
        void write_native(offs_t address, uX data, uX mask)  //void write_native(offs_t address, NativeType data, NativeType mask = ~NativeType(0)) {
        {
            emumem_global.dispatch_write(Level, Width, AddrShift, Endian, offs_t.MaxValue, address & m_addrmask, data, mask, m_dispatch_write);  //    dispatch_write<Level, Width, AddrShift, Endian>(offs_t(-1), address & m_addrmask, data, mask, m_dispatch_write);;
        }


        //template<int Level, int Width, int AddrShift, endianness_t Endian>
        public void set(address_space space, std.pair<object, object> rw)  //void emu::detail::memory_access_specific<Level, Width, AddrShift, Endian>::set(address_space *space, std::pair<const void *, const void *> rw)
        {
            m_space = space;
            m_addrmask = space.addrmask();
            m_dispatch_read  = (handler_entry_read [])rw.first;  //m_dispatch_read  = (const handler_entry_read <Width, AddrShift, Endian> *const *)(rw.first);
            m_dispatch_write = (handler_entry_write [])rw.second;  //m_dispatch_write = (const handler_entry_write<Width, AddrShift, Endian> *const *)(rw.second);
        }
    }


    // ======================> memory_access_cache
    // memory_access_cache contains state data for cached access
    //template<int Width, int AddrShift, endianness_t Endian>
    public class memory_access_cache : global_object
    {
        //friend class ::address_space;

        //using NativeType = typename emu::detail::handler_entry_size<Width>::uX;

        u32 NATIVE_BYTES; // computed in ctor  = 1 << Width;
        u32 NATIVE_MASK; // computed in ctor  = Width + AddrShift >= 0 ? (1 << (Width + AddrShift)) - 1 : 0;


        // template parameters
        public int Width;
        public int AddrShift;
        public endianness_t Endian;


        // internal state
        address_space m_space;
        offs_t m_addrmask;             // address mask
        offs_t m_addrstart_r;            // minimum valid address for reading
        offs_t m_addrend_r;              // maximum valid address for reading
        offs_t m_addrstart_w;             // minimum valid address for writing
        offs_t m_addrend_w;               // maximum valid address for writing
        handler_entry_read m_cache_r;  //handler_entry_read<Width, AddrShift, Endian> *m_cache_r;   // read cache
        handler_entry_write m_cache_w;  //handler_entry_write<Width, AddrShift, Endian> *m_cache_w;  // write cache

        handler_entry_read m_root_read;  //handler_entry_read <Width, AddrShift, Endian> *m_root_read;  // decode tree roots
        handler_entry_write m_root_write;  //handler_entry_write<Width, AddrShift, Endian> *m_root_write;


        // construction/destruction

        //template<int Width, int AddrShift, int Endian>
        protected memory_access_cache(int Width, int AddrShift, endianness_t Endian)
        {
            this.Width = Width;
            this.AddrShift = AddrShift;
            this.Endian = Endian;


            NATIVE_BYTES = 1U << Width;
            NATIVE_MASK = (u32)(Width + AddrShift >= 0 ? (1 << (Width + AddrShift)) - 1 : 0);


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


        // getters

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
        //void *read_ptr(offs_t address) {


        public u8 read_byte(offs_t address) { return Width == 0 ? read_native(address & ~NATIVE_MASK).x8 : memory_read_generic(Width, AddrShift, Endian, 0, true, (offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(Width, 0xff)).x8; }  //u8 read_byte(offs_t address) { return Width == 0 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 0, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xff); }
        public u16 read_word(offs_t address) { return Width == 1 ? read_native(address & ~NATIVE_MASK).x16 : memory_read_generic(Width, AddrShift, Endian, 1, true, (offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(Width, 0xffff)).x16; }  //u16 read_word(offs_t address) { return Width == 1 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffff); }
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


        //NativeType read_native(offs_t address, NativeType mask = ~NativeType(0));
        //template<int Width, int AddrShift, int Endian>
        uX read_native(offs_t address) { return read_native(address, uX.MaxValue(Width)); }
        uX read_native(offs_t address, uX mask)  //template<int Width, int AddrShift, endianness_t Endian> typename emu::detail::handler_entry_size<Width>::uX emu::detail::memory_access_cache<Width, AddrShift, Endian>::read_native(offs_t address, typename emu::detail::handler_entry_size<Width>::uX mask)
        {
            address &= m_addrmask;
            check_address_r(address);
            return m_cache_r.read(Width, AddrShift, Endian, address, mask);
        }


        //void write_native(offs_t address, NativeType data, NativeType mask = ~NativeType(0));
        //template<int Width, int AddrShift, endianness_t Endian> void emu::detail::memory_access_cache<Width, AddrShift, Endian>::write_native(offs_t address, typename emu::detail::handler_entry_size<Width>::uX data, typename emu::detail::handler_entry_size<Width>::uX mask)


        //template<int Width, int AddrShift, endianness_t Endian>
        public void set(address_space space, std.pair<object, object> rw)  //void emu::detail::memory_access_cache<Width, AddrShift, Endian>::set(address_space *space, std::pair<void *, void *> rw)
        {
            m_space = space;
            m_addrmask = space.addrmask();

            space.add_change_notifier((read_or_write mode) =>  //space->add_change_notifier([this](read_or_write mode) {
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

            m_root_read  = (handler_entry_read)rw.first;  //m_root_read  = (handler_entry_read <Width, AddrShift, Endian> *)(rw.first);
            m_root_write = (handler_entry_write)rw.second;  //m_root_write = (handler_entry_write<Width, AddrShift, Endian> *)(rw.second);

            // Protect against a wandering memset
            m_addrstart_r = 1;
            m_addrend_r = 0;
            m_cache_r = null;
            m_addrstart_w = 1;
            m_addrend_w = 0;
            m_cache_w = null;
        }
    }

    } }  //namespace emu { namespace detail


    // ======================> memory_access cache/specific type dispatcher
    //template<int HighBits, int Width, int AddrShift, endianness_t Endian>
    struct memory_access
    {
        int Level;  // = emu::detail::handler_entry_dispatch_level(HighBits);

        //using cache = emu::detail::memory_access_cache<Width, AddrShift, Endian>;
        public class cache : emu.detail.memory_access_cache
        {
            public cache(int Width, int AddrShift, endianness_t Endian) : base(Width, AddrShift, Endian) { }
        }
        public cache m_cache;

        //using specific = emu::detail::memory_access_specific<Level, Width, AddrShift, Endian>;
        public class specific : emu.detail.memory_access_specific
        {
            public specific(int Level, int Width, int AddrShift, endianness_t Endian) : base(Level, Width, AddrShift, Endian) { }
        }
        public specific m_specific;


        public memory_access(int HighBits, int Width, int AddrShift, endianness_t Endian)
        {
            Level = mame.emumem_global.handler_entry_dispatch_level(HighBits);


            m_cache = new cache(Width, AddrShift, Endian);
            m_specific = new specific(Level, Width, AddrShift, Endian);
        }
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
    // address_space holds live information about an address space
    public abstract class address_space : global_object, IDisposable
    {
        //friend class memory_bank;
        //friend class memory_block;
        //template<int Width, int AddrShift, endianness_t Endian> friend class handler_entry_read_unmapped;
        //template<int Width, int AddrShift, endianness_t Endian> friend class handler_entry_write_unmapped;


        class notifier_t
        {
            public Action<read_or_write> m_notifier;
            public int m_id;
        }


        // private state
        address_space_config m_config;       // configuration of this space
        device_t m_device;           // reference to the owning device
        address_map m_map;            // original memory map
        offs_t m_addrmask;         // physical address mask
        offs_t m_logaddrmask;      // logical address mask
        u64 m_unmap;            // unmapped value
        int m_spacenum;         // address space index
        bool m_log_unmap;        // log unmapped accesses in this space?
        string m_name;             // friendly name of the address space
        protected u8 m_addrchars;        // number of characters to use for physical addresses
        u8 m_logaddrchars;     // number of characters to use for logical addresses

        protected handler_entry m_unmap_r;
        protected handler_entry m_unmap_w;

        protected handler_entry m_nop_r;
        protected handler_entry m_nop_w;

        //std::vector<std::unique_ptr<memory_passthrough_handler>> m_mphs;

        std.vector<notifier_t> m_notifiers = new std.vector<notifier_t>();        // notifier list for address map change
        int m_notifier_id;      // next notifier id
        u32 m_in_notification;  // notification(s) currently being done

        protected memory_manager m_manager;          // reference to the owning manager


        // construction/destruction

        //-------------------------------------------------
        //  address_space - constructor
        //-------------------------------------------------
        protected address_space(memory_manager manager, device_memory_interface memory, int spacenum)
        {
            m_config = memory.space_config(spacenum);
            m_device = memory.device();
            m_addrmask = make_bitmask32((u32)m_config.addr_width());
            m_logaddrmask = make_bitmask32(m_config.logaddr_width());
            m_unmap = 0;
            m_spacenum = spacenum;
            m_log_unmap = true;
            m_name = memory.space_config(spacenum).name();
            m_manager = manager;
            m_addrchars = (byte)((m_config.addr_width() + 3) / 4);
            m_logaddrchars = (byte)((m_config.logaddr_width() + 3) / 4);
            m_notifier_id = 0;
            m_in_notification = 0;
            m_manager = manager;
        }

        ~address_space()
        {
            assert(m_isDisposed);  // can remove
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


        public memory_manager manager() { return m_manager; }
        public device_t device() { return m_device; }
        public string name() { return m_name; }
        public int spacenum() { return m_spacenum; }
        //address_map *map() const { return m_map; }


        //template<int Width, int AddrShift, endianness_t Endian> void cache(emu::detail::memory_access_cache<Width, AddrShift, Endian> &v) {
        public void cache(int Width, int AddrShift, endianness_t Endian, emu.detail.memory_access_cache v)
        {
            if (AddrShift != m_config.addr_shift())
                fatalerror("Requesting cache() with address shift {0} while the config says {1}\n", AddrShift, m_config.addr_shift());
            if (8 << Width != m_config.data_width())
                fatalerror("Requesting cache() with data width {0} while the config says {1}\n", 8 << Width, m_config.data_width());
            if (Endian != m_config.endianness())
                fatalerror("Requesting cache() with endianness {0} while the config says {1}\n",
                           endianness_names[(int)Endian], endianness_names[(int)m_config.endianness()]);

            v.set(this, get_cache_info());
        }


        //template<int Level, int Width, int AddrShift, endianness_t Endian> void specific(emu::detail::memory_access_specific<Level, Width, AddrShift, Endian> &v) {
        public void specific(int Level, int Width, int AddrShift, endianness_t Endian, emu.detail.memory_access_specific v)
        {
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


        public int add_change_notifier(Action<read_or_write> n)  //std::function<void (read_or_write)> n);
        {
            int id = m_notifier_id++;
            m_notifiers.emplace_back(new notifier_t() { m_notifier = n, m_id = id });
            return id;
        }


        public void remove_change_notifier(int id)
        {
            for (int idx = 0; idx < m_notifiers.size(); idx++)  // for (var i = m_notifiers.begin(); i != m_notifiers.end(); i++)
            {
                var i = m_notifiers[idx];

                if (i.m_id == id)
                {
                    m_notifiers.erase(idx);
                    return;
                }
            }

            fatalerror("Unknown notifier id {0}, double remove?\n", id);
        }


        protected void invalidate_caches(read_or_write mode)
        {
            if ((u32)mode != 0 & (~m_in_notification) != 0)
            {
                u32 old = m_in_notification;
                m_in_notification |= (u32)mode;
                foreach (var n in m_notifiers)
                    n.m_notifier(mode);

                m_in_notification = old;
            }
        }


        protected abstract void validate_reference_counts();

        //protected abstract void remove_passthrough(std::unordered_set<handler_entry> handlers);


        public int data_width() { return m_config.data_width(); }
        public int addr_width() { return m_config.addr_width(); }
        //int logaddr_width() const { return m_config.logaddr_width(); }
        public int alignment() { return m_config.alignment(); }
        public endianness_t endianness() { return m_config.endianness(); }
        public int addr_shift() { return m_config.addr_shift(); }
        public u64 unmap() { return m_unmap; }
        public bool is_octal() { return m_config.is_octal(); }

        public offs_t addrmask() { return m_addrmask; }
        public u8 addrchars() { return m_addrchars; }
        //offs_t logaddrmask() const { return m_logaddrmask; }
        public u8 logaddrchars() { return m_logaddrchars; }


        // debug helpers

        protected abstract string get_handler_string(read_or_write readorwrite, offs_t address);
        //protected abstract void dump_maps(std::vector<memory_entry> read_map, std::vector<memory_entry> write_map);

        public bool log_unmap() { return m_log_unmap; }
        public void set_log_unmap(bool log) { m_log_unmap = log; }


        // general accessors
        //virtual void accessors(data_accessors &accessors) const = 0;
        //virtual void *get_read_ptr(offs_t address) = 0;
        //virtual void *get_write_ptr(offs_t address) = 0;

        // read accessors
        public abstract u8 read_byte(offs_t address);
        public abstract u16 read_word(offs_t address);
        public abstract u16 read_word(offs_t address, u16 mask);
        public abstract u16 read_word_unaligned(offs_t address);
        public abstract u16 read_word_unaligned(offs_t address, u16 mask);
        public abstract u32 read_dword(offs_t address);
        public abstract u32 read_dword(offs_t address, u32 mask);
        public abstract u32 read_dword_unaligned(offs_t address);
        public abstract u32 read_dword_unaligned(offs_t address, u32 mask);
        public abstract u64 read_qword(offs_t address);
        public abstract u64 read_qword(offs_t address, u64 mask);
        public abstract u64 read_qword_unaligned(offs_t address);
        public abstract u64 read_qword_unaligned(offs_t address, u64 mask);


        // write accessors
        public abstract void write_byte(offs_t address, u8 data);
        public abstract void write_word(offs_t address, u16 data);
        public abstract void write_word(offs_t address, u16 data, u16 mask);
        public abstract void write_word_unaligned(offs_t address, u16 data);
        public abstract void write_word_unaligned(offs_t address, u16 data, u16 mask);
        public abstract void write_dword(offs_t address, u32 data);
        public abstract void write_dword(offs_t address, u32 data, u32 mask);
        public abstract void write_dword_unaligned(offs_t address, u32 data);
        public abstract void write_dword_unaligned(offs_t address, u32 data, u32 mask);
        public abstract void write_qword(offs_t address, u64 data);
        public abstract void write_qword(offs_t address, u64 data, u64 mask);
        public abstract void write_qword_unaligned(offs_t address, u64 data);
        public abstract void write_qword_unaligned(offs_t address, u64 data, u64 mask);


        // address-to-byte conversion helpers
        public offs_t address_to_byte(offs_t address) { return m_config.addr2byte(address); }
        public offs_t address_to_byte_end(offs_t address) { return m_config.addr2byte_end(address); }
        public offs_t byte_to_address(offs_t address) { return m_config.byte2addr(address); }
        //offs_t byte_to_address_end(offs_t address) const { return m_config.byte2addr_end(address); }


        // umap ranges (short form)
        //void unmap_read(offs_t addrstart, offs_t addrend, offs_t addrmirror = 0) { unmap_generic(addrstart, addrend, addrmirror, read_or_write::READ, false); }
        //void unmap_write(offs_t addrstart, offs_t addrend, offs_t addrmirror = 0) { unmap_generic(addrstart, addrend, addrmirror, read_or_write::WRITE, false); }
        //void unmap_readwrite(offs_t addrstart, offs_t addrend, offs_t addrmirror = 0) { unmap_generic(addrstart, addrend, addrmirror, read_or_write::READWRITE, false); }
        //void nop_read(offs_t addrstart, offs_t addrend, offs_t addrmirror = 0) { unmap_generic(addrstart, addrend, addrmirror, read_or_write::READ, true); }
        //void nop_write(offs_t addrstart, offs_t addrend, offs_t addrmirror = 0) { unmap_generic(addrstart, addrend, addrmirror, read_or_write::WRITE, true); }
        //void nop_readwrite(offs_t addrstart, offs_t addrend, offs_t addrmirror = 0) { unmap_generic(addrstart, addrend, addrmirror, read_or_write::READWRITE, true); }


        // umap ranges (with mirror/mask)
        //void unmap_read(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror) { unmap_generic(addrstart, addrend, addrmask, addrmirror, ROW_READ, false); }
        //void unmap_write(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror) { unmap_generic(addrstart, addrend, addrmask, addrmirror, ROW_WRITE, false); }
        //void unmap_readwrite(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror) { unmap_generic(addrstart, addrend, addrmask, addrmirror, ROW_READWRITE, false); }
        //void nop_read(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror) { unmap_generic(addrstart, addrend, addrmask, addrmirror, ROW_READ, true); }
        //void nop_write(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror) { unmap_generic(addrstart, addrend, addrmask, addrmirror, ROW_WRITE, true); }
        //void nop_readwrite(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror) { unmap_generic(addrstart, addrend, addrmask, addrmirror, ROW_READWRITE, true); }


        // install ports, banks, RAM (short form)
        //void install_read_port(offs_t addrstart, offs_t addrend, const char *rtag) { install_read_port(addrstart, addrend, 0, 0, rtag); }
        //void install_write_port(offs_t addrstart, offs_t addrend, const char *wtag) { install_write_port(addrstart, addrend, 0, 0, wtag); }
        //void install_readwrite_port(offs_t addrstart, offs_t addrend, const char *rtag, const char *wtag) { install_readwrite_port(addrstart, addrend, 0, rtag, wtag); }
        //void install_read_bank(offs_t addrstart, offs_t addrend, const char *tag) { install_read_bank(addrstart, addrend, 0, 0, tag); }
        //void install_write_bank(offs_t addrstart, offs_t addrend, const char *tag) { install_write_bank(addrstart, addrend, 0, 0, tag); }
        //void install_readwrite_bank(offs_t addrstart, offs_t addrend, const char *tag) { install_readwrite_bank(addrstart, addrend, 0, 0, tag); }
        //void install_read_bank(offs_t addrstart, offs_t addrend, memory_bank *bank) { install_read_bank(addrstart, addrend, 0, 0, bank); }
        //void install_write_bank(offs_t addrstart, offs_t addrend, memory_bank *bank) { install_write_bank(addrstart, addrend, 0, 0, bank); }
        //void install_readwrite_bank(offs_t addrstart, offs_t addrend, memory_bank *bank) { install_readwrite_bank(addrstart, addrend, 0, 0, bank); }
        //void install_rom(offs_t addrstart, offs_t addrend, offs_t addrmirror, void *baseptr = nullptr) { install_ram_generic(addrstart, addrend, addrmirror, read_or_write::READ, baseptr); }
        //void install_writeonly(offs_t addrstart, offs_t addrend, offs_t addrmirror, void *baseptr = nullptr) { install_ram_generic(addrstart, addrend, addrmirror, read_or_write::WRITE, baseptr); }
        //void install_ram(offs_t addrstart, offs_t addrend, offs_t addrmirror, void *baseptr = nullptr) { install_ram_generic(addrstart, addrend, addrmirror, read_or_write::READWRITE, baseptr); }


        // install ports, banks, RAM (with mirror/mask)
        //void install_read_port(offs_t addrstart, offs_t addrend, offs_t addrmirror, const char *rtag) { install_readwrite_port(addrstart, addrend, addrmirror, rtag, ""); }
        //void install_write_port(offs_t addrstart, offs_t addrend, offs_t addrmirror, const char *wtag) { install_readwrite_port(addrstart, addrend, addrmirror, "", wtag); }
        protected abstract void install_readwrite_port(offs_t addrstart, offs_t addrend, offs_t addrmirror, string rtag, string wtag);
        //void install_read_bank(offs_t addrstart, offs_t addrend, offs_t addrmirror, const char *tag) { install_bank_generic(addrstart, addrend, addrmirror, tag, ""); }
        //void install_write_bank(offs_t addrstart, offs_t addrend, offs_t addrmirror, const char *tag) { install_bank_generic(addrstart, addrend, addrmirror, "", tag); }
        //void install_readwrite_bank(offs_t addrstart, offs_t addrend, offs_t addrmirror, const char *tag)  { install_bank_generic(addrstart, addrend, addrmirror, tag, tag); }
        //void install_read_bank(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_bank *bank) { install_bank_generic(addrstart, addrend, addrmirror, bank, nullptr); }
        //void install_write_bank(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_bank *bank) { install_bank_generic(addrstart, addrend, addrmirror, nullptr, bank); }
        //void install_readwrite_bank(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_bank *bank)  { install_bank_generic(addrstart, addrend, addrmirror, bank, bank); }
        //void install_rom(offs_t addrstart, offs_t addrend, offs_t addrmirror, void *baseptr = nullptr) { install_ram_generic(addrstart, addrend, addrmirror, read_or_write::READ, baseptr); }
        //void install_writeonly(offs_t addrstart, offs_t addrend, offs_t addrmirror, void *baseptr = nullptr) { install_ram_generic(addrstart, addrend, addrmirror, read_or_write::WRITE, baseptr); }
        //void install_ram(offs_t addrstart, offs_t addrend, offs_t addrmirror, void *baseptr = nullptr) { install_ram_generic(addrstart, addrend, addrmirror, read_or_write::READWRITE, baseptr); }


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


        // install taps with mirroring
        //virtual memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u8  &data, u8  mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        //virtual memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u16 &data, u16 mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        //virtual memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u32 &data, u32 mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        //virtual memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        //virtual memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u8  &data, u8  mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        //virtual memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u16 &data, u16 mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        //virtual memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u32 &data, u32 mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        //virtual memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tap, memory_passthrough_handler *mph = nullptr);
        //virtual memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u8  &data, u8  mem_mask)> tapr, std::function<void (offs_t offset, u8  &data, u8  mem_mask)> tapw, memory_passthrough_handler *mph = nullptr);
        //virtual memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u16 &data, u16 mem_mask)> tapr, std::function<void (offs_t offset, u16 &data, u16 mem_mask)> tapw, memory_passthrough_handler *mph = nullptr);
        //virtual memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u32 &data, u32 mem_mask)> tapr, std::function<void (offs_t offset, u32 &data, u32 mem_mask)> tapw, memory_passthrough_handler *mph = nullptr);
        //virtual memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tapr, std::function<void (offs_t offset, u64 &data, u64 mem_mask)> tapw, memory_passthrough_handler *mph = nullptr);


        // install new-style delegate handlers (short form)
        void install_read_handler(offs_t addrstart, offs_t addrend, read8_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        void install_write_handler(offs_t addrstart, offs_t addrend, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        void install_readwrite_handler(offs_t addrstart, offs_t addrend, read8_delegate rhandler, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
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

        void install_read_handler(offs_t addrstart, offs_t addrend, read8s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read8s_delegate rhandler, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read16s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        void install_write_handler(offs_t addrstart, offs_t addrend, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read16s_delegate rhandler, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read32s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read32s_delegate rhandler, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, read64s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, read64s_delegate rhandler, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_readwrite_handler(addrstart, addrend, 0, 0, 0, rhandler, whandler, unitmask, cswidth); }

        void install_read_handler(offs_t addrstart, offs_t addrend, read8sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
        void install_write_handler(offs_t addrstart, offs_t addrend, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) { install_write_handler(addrstart, addrend, 0, 0, 0, whandler, unitmask, cswidth); }
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

        void install_read_handler(offs_t addrstart, offs_t addrend, read8smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { install_read_handler(addrstart, addrend, 0, 0, 0, rhandler, unitmask, cswidth); }
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

        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8m_delegate rhandler, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16m_delegate rhandler, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32m_delegate rhandler, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64m_delegate rhandler, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;

        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;

        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16sm_delegate rhandler, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32sm_delegate rhandler, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64sm_delegate rhandler, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;

        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8mo_delegate rhandler, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16mo_delegate rhandler, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32mo_delegate rhandler, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64mo_delegate rhandler, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;

        protected abstract void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0);
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        protected abstract void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0);
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32smo_delegate rhandler, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64smo_delegate rhandler, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;


        // setup

        //-------------------------------------------------
        //  prepare_map - allocate the address map and
        //  walk through it to find implicit memory regions
        //  and identify shared regions
        //-------------------------------------------------
        public void prepare_map()
        {
            memory_region devregion = (m_spacenum == 0) ? m_device.memregion(DEVICE_SELF) : null;
            u32 devregionsize = (devregion != null) ? devregion.bytes() : 0;

            // allocate the address map
            m_map = new address_map(m_device, m_spacenum);

            // merge in the submaps
            m_map.import_submaps(m_manager.machine(), m_device.owner() != null ? m_device.owner() : m_device, data_width(), endianness(), addr_shift());

            // extract global parameters specified by the map
            m_unmap = (m_map.m_unmapval == 0) ? 0UL : ~0UL;
            if (m_map.m_globalmask != 0)
            {
                if ((m_map.m_globalmask & ~m_addrmask) != 0)
                    fatalerror("Can't set a global address mask of {0} on a {1}-bits address width bus.\n", m_map.m_globalmask, addr_width());  //%08x

                m_addrmask = m_map.m_globalmask;
            }

            // make a pass over the address map, adjusting for the device and getting memory pointers
            foreach (address_map_entry entry in m_map.m_entrylist)
            {
                // computed adjusted addresses first
                offs_t addrstart_temp = entry.m_addrstart;
                offs_t addrend_temp = entry.m_addrend;
                offs_t addrmask_temp = entry.m_addrmask;
                offs_t addrmirror_temp = entry.m_addrmirror;
                adjust_addresses(ref addrstart_temp, ref addrend_temp, ref addrmask_temp, ref addrmirror_temp);
                entry.m_addrstart = addrstart_temp;
                entry.m_addrend = addrend_temp;
                entry.m_addrmask = addrmask_temp;
                entry.m_addrmirror = addrmirror_temp;

                // if we have a share entry, add it to our map
                if (entry.m_share != null)
                {
                    // if we can't find it, add it to our map
                    string fulltag = entry.m_devbase.subtag(entry.m_share);
                    if (m_manager.shares().find(fulltag) == null)
                    {
                        emumem_global.VPRINTF("Creating share '{0}' of length {1}\n", fulltag, entry.m_addrend + 1 - entry.m_addrstart);
                        m_manager.shares().emplace(fulltag.c_str(), new memory_share((byte)m_config.data_width(), address_to_byte(entry.m_addrend + 1 - entry.m_addrstart), endianness()));
                    }
                }

                // if this is a ROM handler without a specified region, attach it to the implicit region
                if (m_spacenum == 0 && entry.m_read.m_type == map_handler_type.AMH_ROM && entry.m_region == null)
                {
                    // make sure it fits within the memory region before doing so, however
                    if (entry.m_addrend < devregionsize)
                    {
                        entry.m_region = m_device.tag();
                        entry.m_rgnoffs = address_to_byte(entry.m_addrstart);
                    }
                }

                // validate adjusted addresses against implicit regions
                if (entry.m_region != null && entry.m_share == null)
                {
                    // determine full tag
                    string fulltag = entry.m_devbase.subtag(entry.m_region);

                    // find the region
                    memory_region region = m_manager.machine().root_device().memregion(fulltag);
                    if (region == null)
                        fatalerror("device '{0}' {1} space memory map entry {2}-{3} references nonexistant region \"{4}\"\n", m_device.tag(), m_name, entry.m_addrstart, entry.m_addrend, entry.m_region);

                    // validate the region
                    if (entry.m_rgnoffs + m_config.addr2byte(entry.m_addrend - entry.m_addrstart + 1) > region.bytes())
                        fatalerror("device '{0}' {1} space memory map entry {2}-{3} extends beyond region \"{4}\" size ({5})\n", m_device.tag(), m_name, entry.m_addrstart, entry.m_addrend, entry.m_region, region.bytes());
                }

                // convert any region-relative entries to their memory pointers
                if (entry.m_region != null)
                {
                    // determine full tag
                    string fulltag = entry.m_devbase.subtag(entry.m_region);

                    // set the memory address
                    entry.m_memory = new PointerU8(m_manager.machine().root_device().memregion(fulltag).base_(), (int)entry.m_rgnoffs);  //entry.m_memory = m_manager.machine().root_device().memregion(fulltag.c_str())->base() + entry.m_rgnoffs;
                }
            }
        }

        //-------------------------------------------------
        //  allocate_memory - determine all neighboring
        //  address ranges and allocate memory to back
        //  them
        //-------------------------------------------------
        public void allocate_memory()
        {
            var blocklist = m_manager.m_blocklist;

            // make a first pass over the memory map and track blocks with hardcoded pointers
            // we do this to make sure they are found by space_find_backing_memory first
            // do it back-to-front so that overrides work correctly
            int tail = blocklist.size();
            foreach (var entry in m_map.m_entrylist)
            {
                if (entry.m_memory != null)
                    blocklist.insert(0 + tail, new memory_block(this, entry.m_addrstart, entry.m_addrend, entry.m_memory));  //blocklist.insert(blocklist.begin() + tail, std::make_unique<memory_block>(*this, entry.m_addrstart, entry.m_addrend, entry.m_memory));
            }

            // loop over all blocks just allocated and assign pointers from them
            address_map_entry unassigned = null;
            for (var memblockIdx = /*blocklist.begin() +*/ tail; memblockIdx != blocklist.size(); ++memblockIdx)  // for (auto memblock = blocklist.begin() + tail; memblock != blocklist.end(); ++memblock)
            {
                var memblock = blocklist[memblockIdx];
                unassigned = block_assign_intersecting(memblock.addrstart(), memblock.addrend(), memblock.get().data());
            }

            // if we don't have an unassigned pointer yet, try to find one
            if (unassigned == null)
                unassigned = block_assign_intersecting(/*~0*/ offs_t.MaxValue, 0, null);//, null);

            // loop until we've assigned all memory in this space
            while (unassigned != null)
            {
                // work in MEMORY_BLOCK_CHUNK-sized chunks
                offs_t curblockstart = unassigned.m_addrstart / emumem_global.MEMORY_BLOCK_CHUNK;
                offs_t curblockend = unassigned.m_addrend / emumem_global.MEMORY_BLOCK_CHUNK;

                // loop while we keep finding unassigned blocks in neighboring MEMORY_BLOCK_CHUNK chunks
                bool changed;
                do
                {
                    changed = false;

                    // scan for unmapped blocks in the adjusted map
                    for (address_map_entry entry = m_map.m_entrylist.first(); entry != null; entry = entry.next())
                    {
                        if (entry.m_memory == null && entry != unassigned && needs_backing_store(entry))
                        {
                            // get block start/end blocks for this block
                            offs_t blockstart = entry.m_addrstart / emumem_global.MEMORY_BLOCK_CHUNK;
                            offs_t blockend = entry.m_addrend / emumem_global.MEMORY_BLOCK_CHUNK;

                            // if we intersect or are adjacent, adjust the start/end
                            if (blockstart <= curblockend + 1 && blockend >= curblockstart - 1)
                            {
                                if (blockstart < curblockstart)
                                {
                                    curblockstart = blockstart;
                                    changed = true;
                                }

                                if (blockend > curblockend)
                                {
                                    curblockend = blockend;
                                    changed = true;
                                }
                            }
                        }
                    }
                } while (changed);

                // we now have a block to allocate; do it
                offs_t curaddrstart = curblockstart * emumem_global.MEMORY_BLOCK_CHUNK;
                offs_t curaddrend = curblockend * emumem_global.MEMORY_BLOCK_CHUNK + (emumem_global.MEMORY_BLOCK_CHUNK - 1);
                var block = new memory_block(this, curaddrstart, curaddrend);

                // assign memory that intersected the new block
                unassigned = block_assign_intersecting(curaddrstart, curaddrend, block.data());
                blocklist.push_back(block);
            }
        }

        //-------------------------------------------------
        //  locate_memory - find all the requested
        //  pointers into the final allocated memory
        //-------------------------------------------------
        public void locate_memory()
        {
            // once this is done, find the starting bases for the banks
            foreach (var bank in m_manager.banks())
            {
                if (bank.second().base_() == null && bank.second().references_space(this, read_or_write.READWRITE))
                {
                    // set the initial bank pointer
                    foreach (address_map_entry entry in m_map.m_entrylist)
                    {
                        if (entry.m_addrstart == bank.second().addrstart() && entry.m_memory != null)
                        {
                            bank.second().set_base(entry.m_memory);
                            emumem_global.VPRINTF("assigned bank '{0}' pointer to memory from range {1:x8}-{2:x8} [{3}]\n", bank.second().tag(), entry.m_addrstart, entry.m_addrend, entry.m_memory);
                            break;
                        }
                    }
                }
            }
        }


        public handler_entry_read_unmapped get_unmap_r() { return (handler_entry_read_unmapped)m_unmap_r; }  //template<int Width, int AddrShift, endianness_t Endian> handler_entry_read_unmapped <Width, AddrShift, Endian> *get_unmap_r() const { return static_cast<handler_entry_read_unmapped <Width, AddrShift, Endian> *>(m_unmap_r); }
        public handler_entry_write_unmapped get_unmap_w() { return (handler_entry_write_unmapped)m_unmap_w; }  //template<int Width, int AddrShift, endianness_t Endian> handler_entry_write_unmapped<Width, AddrShift, Endian> *get_unmap_w() const { return static_cast<handler_entry_write_unmapped<Width, AddrShift, Endian> *>(m_unmap_w); }


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


        // internal helpers
        protected abstract std.pair<object, object> get_cache_info();  //virtual std::pair<void *, void *> get_cache_info() = 0;
        protected abstract std.pair<object, object> get_specific_info();  //virtual std::pair<const void *, const void *> get_specific_info() = 0;


        //-------------------------------------------------
        //  populate_map_entry - map a single read or
        //  write entry based on information from an
        //  address map entry
        //-------------------------------------------------
        void populate_map_entry(address_map_entry entry, read_or_write readorwrite)
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
                    install_ram_generic(entry.m_addrstart, entry.m_addrend, entry.m_addrmirror, readorwrite, null);
                    break;

                case map_handler_type.AMH_RAM:
                    install_ram_generic(entry.m_addrstart, entry.m_addrend, entry.m_addrmirror, readorwrite, null);
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
                        switch (data.m_bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto8m, entry.m_mask, entry.m_cswidth); break;
                            case 16:    throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto16m, entry.m_mask, entry.m_cswidth); break;
                            case 32:    throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto32m, entry.m_mask, entry.m_cswidth); break;
                            case 64:    throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto64m, entry.m_mask, entry.m_cswidth); break;
                        }
                    else
                        switch (data.m_bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto8m, entry.m_mask, entry.m_cswidth); break;
                            case 16:    throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto16m, entry.m_mask, entry.m_cswidth); break;
                            case 32:    throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto32m, entry.m_mask, entry.m_cswidth); break;
                            case 64:    throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto64m, entry.m_mask, entry.m_cswidth); break;
                        }
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE_S:
                    if (readorwrite == read_or_write.READ)
                        switch (data.m_bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto8s, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto16s, entry.m_mask, entry.m_cswidth); break;
                            case 32:    throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto32s, entry.m_mask, entry.m_cswidth); break;
                            case 64:    throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto64s, entry.m_mask, entry.m_cswidth); break;
                        }
                    else
                        switch (data.m_bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto8s, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto16s, entry.m_mask, entry.m_cswidth); break;
                            case 32:    throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto32s, entry.m_mask, entry.m_cswidth); break;
                            case 64:    throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto64s, entry.m_mask, entry.m_cswidth); break;
                        }
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE_SM:
                    if (readorwrite == read_or_write.READ)
                        switch (data.m_bits)
                        {
                            case 8:     install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto8sm, entry.m_mask, entry.m_cswidth); break;
                            case 16:    throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto16sm, entry.m_mask, entry.m_cswidth); break;
                            case 32:    throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto32sm, entry.m_mask, entry.m_cswidth); break;
                            case 64:    throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto64sm, entry.m_mask, entry.m_cswidth); break;
                        }
                    else
                        switch (data.m_bits)
                        {
                            case 8:     install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto8sm, entry.m_mask, entry.m_cswidth); break;
                            case 16:    throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto16sm, entry.m_mask, entry.m_cswidth); break;
                            case 32:    throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto32sm, entry.m_mask, entry.m_cswidth); break;
                            case 64:    throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto64sm, entry.m_mask, entry.m_cswidth); break;
                        }
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE_MO:
                    if (readorwrite == read_or_write.READ)
                        switch (data.m_bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto8mo, entry.m_mask, entry.m_cswidth); break;
                            case 16:    throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto16mo, entry.m_mask, entry.m_cswidth); break;
                            case 32:    throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto32mo, entry.m_mask, entry.m_cswidth); break;
                            case 64:    throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto64mo, entry.m_mask, entry.m_cswidth); break;
                        }
                    else
                        switch (data.m_bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto8mo, entry.m_mask, entry.m_cswidth); break;
                            case 16:    throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto16mo, entry.m_mask, entry.m_cswidth); break;
                            case 32:    throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto32mo, entry.m_mask, entry.m_cswidth); break;
                            case 64:    throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto64mo, entry.m_mask, entry.m_cswidth); break;
                        }
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE_SMO:
                    if (readorwrite == read_or_write.READ)
                        switch (data.m_bits)
                        {
                            case 8:     install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto8smo, entry.m_mask, entry.m_cswidth); break;
                            case 16:    throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto16smo, entry.m_mask, entry.m_cswidth); break;
                            case 32:    throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto32smo, entry.m_mask, entry.m_cswidth); break;
                            case 64:    throw new emu_unimplemented();  //install_read_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_rproto64smo, entry.m_mask, entry.m_cswidth); break;
                        }
                    else
                        switch (data.m_bits)
                        {
                            case 8:     install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto8smo, entry.m_mask, entry.m_cswidth); break;
                            case 16:    install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto16smo, entry.m_mask, entry.m_cswidth); break;
                            case 32:    throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto32smo, entry.m_mask, entry.m_cswidth); break;
                            case 64:    throw new emu_unimplemented();  //install_write_handler(entry.m_addrstart, entry.m_addrend, entry.m_addrmask, entry.m_addrmirror, entry.m_addrselect, entry.m_wproto64smo, entry.m_mask, entry.m_cswidth); break;
                        }
                    break;

                case map_handler_type.AMH_PORT:
                    install_readwrite_port(entry.m_addrstart, entry.m_addrend, entry.m_addrmirror,
                                    (readorwrite == read_or_write.READ) ? entry.m_devbase.subtag(data.m_tag) : "",
                                    (readorwrite == read_or_write.WRITE) ? entry.m_devbase.subtag(data.m_tag) : "");
                    break;

                case map_handler_type.AMH_BANK:
                    install_bank_generic(entry.m_addrstart, entry.m_addrend, entry.m_addrmirror,
                                    (readorwrite == read_or_write.READ) ? entry.m_devbase.subtag(data.m_tag) : "",
                                    (readorwrite == read_or_write.WRITE) ? entry.m_devbase.subtag(data.m_tag) : "");
                    break;

                case map_handler_type.AMH_DEVICE_SUBMAP:
                    throw new emu_fatalerror("Internal mapping error: leftover mapping of '{0}'.\n", data.m_tag);
            }
        }


        protected abstract void unmap_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, bool quiet);
        protected abstract void install_ram_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, PointerU8 baseptr);  //virtual void install_ram_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, void *baseptr) = 0;
        protected abstract void install_bank_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, string rtag, string wtag);
        protected abstract void install_bank_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_bank rbank, memory_bank wbank);


        //-------------------------------------------------
        //  adjust_addresses - adjust addresses for a
        //  given address space in a standard fashion
        //-------------------------------------------------
        public void adjust_addresses(ref offs_t start, ref offs_t end, ref offs_t mask, ref offs_t mirror)
        {
            // adjust start/end/mask values
            mask &= m_addrmask;
            start &= ~mirror & m_addrmask;
            end &= ~mirror & m_addrmask;
        }


        protected void check_optimize_all(string function, int width, offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, out offs_t nstart, out offs_t nend, out offs_t nmask, out offs_t nmirror, out u64 nunitmask, out int ncswidth)
        {
            if (addrstart > addrend)
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, start address is after the end address.\n", function, addrstart, addrend, addrmask, addrmirror, addrselect);
            if ((addrstart & ~m_addrmask) != 0)
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, start address is outside of the global address mask {6}, did you mean {7} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, m_addrmask, addrstart & m_addrmask);
            if ((addrend & ~m_addrmask) != 0)
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, end address is outside of the global address mask {6}, did you mean {7} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, m_addrmask, addrend & m_addrmask);

            // Check the relative data widths
            if (width > m_config.data_width())
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, cannot install a {6}-bits wide handler in a {7}-bits wide address space.\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, width, m_config.data_width());

            // Check the validity of the addresses given their intrinsic width
            // We assume that busses with non-zero address shift have a data width matching the shift (reality says yes)
            offs_t default_lowbits_mask = ((offs_t)m_config.data_width() >> (3 - m_config.addr_shift())) - 1;
            offs_t lowbits_mask = (width != 0 && m_config.addr_shift() == 0) ? ((offs_t)width >> 3) - 1 : default_lowbits_mask;

            if ((addrstart & lowbits_mask) != 0)
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, start address has low bits set, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrstart & ~lowbits_mask);
            if (((~addrend) & lowbits_mask) != 0)
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, end address has low bits unset, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrend | lowbits_mask);

            offs_t set_bits = addrstart | addrend;
            offs_t changing_bits = addrstart ^ addrend;
            // Round up to the nearest power-of-two-minus-one
            changing_bits |= changing_bits >> 1;
            changing_bits |= changing_bits >> 2;
            changing_bits |= changing_bits >> 4;
            changing_bits |= changing_bits >> 8;
            changing_bits |= changing_bits >> 16;

            if ((addrmask & ~m_addrmask) != 0)
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, mask is outside of the global address mask {6}, did you mean {7} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, m_addrmask, addrmask & m_addrmask);
            if ((addrselect & ~m_addrmask) != 0)
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, select is outside of the global address mask {6}, did you mean {7} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, m_addrmask, addrselect & m_addrmask);
            if ((addrmask & ~changing_bits) != 0)
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, mask is trying to unmask an unchanging address bit, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrmask & changing_bits);
            if ((addrmirror & changing_bits) != 0)
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, mirror touches a changing address bit, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrmirror & ~changing_bits);
            if ((addrselect & changing_bits) != 0)
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, select touches a changing address bit, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrselect & ~changing_bits);
            if ((addrmirror & set_bits) != 0)
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, mirror touches a set address bit, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrmirror & ~set_bits);
            if ((addrselect & set_bits) != 0)
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, select touches a set address bit, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrselect & ~set_bits);
            if ((addrmirror & addrselect) != 0)
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, mirror touches a select bit, did you mean {6} ?\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, addrmirror & ~addrselect);

            // Check the cswidth, if provided
            if (cswidth > m_config.data_width())
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, the cswidth of {6} is too large for a {7}-bit space.\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, cswidth, m_config.data_width());
            if ((width != 0) && (cswidth % width) != 0)
                fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, the cswidth of {6} is not a multiple of handler size {7}.\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, cswidth, width);
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
                        fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, the unitmask of {6} has incorrect granularity for {7}-bit chip selection.\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, core_i64_hex_format(unitmask, 16), cswidth);
                }
            }

            nunitmask = 0xffffffffffffffffU >> (64 - m_config.data_width());
            if (unitmask != 0)
                nunitmask &= unitmask;

            nstart = addrstart;
            nend = addrend;
            nmask = (addrmask != 0 ? addrmask : changing_bits) | addrselect;
            nmirror = (addrmirror & m_addrmask) | addrselect;
            if (nmirror != 0 && ((nstart & changing_bits) == 0) && (((~nend) & changing_bits) == 0))
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
                fatalerror("{0}: In range {1}-{2} mirror {3}, start address is after the end address.\n", function, addrstart, addrend, addrmirror);  // %x-%x
            if ((addrstart & ~m_addrmask) != 0)
                fatalerror("{0}: In range {1}-{2} mirror {3}, start address is outside of the global address mask {4}, did you mean {5} ?\n", function, addrstart, addrend, addrmirror, m_addrmask, addrstart & m_addrmask);
            if ((addrend & ~m_addrmask) != 0)
                fatalerror("{0}: In range {1}-{2} mirror {3}, end address is outside of the global address mask {4}, did you mean {5} ?\n", function, addrstart, addrend, addrmirror, m_addrmask, addrend & m_addrmask);

            offs_t lowbits_mask = (offs_t)((m_config.data_width() >> (3 - m_config.addr_shift())) - 1);
            if ((addrstart & lowbits_mask) != 0)
                fatalerror("{0}: In range {1}-{2} mirror {3}, start address has low bits set, did you mean {4} ?\n", function, addrstart, addrend, addrmirror, addrstart & ~lowbits_mask);
            if (((~addrend) & lowbits_mask) != 0)
                fatalerror("{0}: In range {1}-{2} mirror {3}, end address has low bits unset, did you mean {4} ?\n", function, addrstart, addrend, addrmirror, addrend | lowbits_mask);

            offs_t set_bits = addrstart | addrend;
            offs_t changing_bits = addrstart ^ addrend;
            // Round up to the nearest power-of-two-minus-one
            changing_bits |= changing_bits >> 1;
            changing_bits |= changing_bits >> 2;
            changing_bits |= changing_bits >> 4;
            changing_bits |= changing_bits >> 8;
            changing_bits |= changing_bits >> 16;

            if ((addrmirror & ~m_addrmask) != 0)
                fatalerror("{0}: In range {1}-{2} mirror {3}, mirror is outside of the global address mask {4}, did you mean {5} ?\n", function, addrstart, addrend, addrmirror, m_addrmask, addrmirror & m_addrmask);
            if ((addrmirror & changing_bits) != 0)
                fatalerror("{0}: In range {1}-{2} mirror {3}, mirror touches a changing address bit, did you mean {4} ?\n", function, addrstart, addrend, addrmirror, addrmirror & ~changing_bits);
            if ((addrmirror & set_bits) != 0)
                fatalerror("{0}: In range {1}-{2} mirror {3}, mirror touches a set address bit, did you mean {4} ?\n", function, addrstart, addrend, addrmirror, addrmirror & ~set_bits);

            nstart = addrstart;
            nend = addrend;
            nmask = changing_bits;
            nmirror = addrmirror;

            if (nmirror != 0 && ((nstart & changing_bits) == 0) && (((~nend) & changing_bits) == 0))
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


        //void check_address(const char *function, offs_t addrstart, offs_t addrend);


        //-------------------------------------------------
        //  find_backing_memory - return a pointer to
        //  the base of RAM associated with the given
        //  device and offset
        //-------------------------------------------------
        protected PointerU8 find_backing_memory(offs_t addrstart, offs_t addrend)  //void *find_backing_memory(offs_t addrstart, offs_t addrend);
        {
            emumem_global.VPRINTF("address_space::find_backing_memory('{0}',{1},{2:x8}-{3:x8}) -> ", m_device.tag(), m_name, addrstart, addrend);

            if (m_map == null)
                return null;

            // look in the address map first, last winning for overrides
            PointerU8 result = null;  //void *result = nullptr;
            foreach (var entry in m_map.m_entrylist)
            {
                if (entry.m_memory != null && addrstart >= entry.m_addrstart && addrend <= entry.m_addrend)
                {
                    emumem_global.VPRINTF("found in entry {0:x8}-{1:x8} [{2} - {3:x8}]\n", entry.m_addrstart, entry.m_addrend, entry.m_memory, address_to_byte(addrstart - entry.m_addrstart));
                    result = new PointerU8(entry.m_memory, (int)address_to_byte(addrstart - entry.m_addrstart));  //result = (u8 *)entry.m_memory + address_to_byte(addrstart - entry.m_addrstart);
                }
            }

            if (result != null)
                return result;

            // if not found there, look in the allocated blocks
            foreach (var block in m_manager.m_blocklist)
            {
                if (block.contains(this, addrstart, addrend))
                {
                    emumem_global.VPRINTF("found in allocated memory block {0:x8}-{1:x8} [{2} - {3:x8}]\n", block.addrstart(), block.addrend(), block.data(), address_to_byte(addrstart - block.addrstart()));
                    return new PointerU8(block.data(), (int)address_to_byte(addrstart - block.addrstart()));  //return block->data() + address_to_byte(addrstart - block->addrstart());
                }
            }

            emumem_global.VPRINTF("did not find\n");
            return null;
        }

        //-------------------------------------------------
        //  space_needs_backing_store - return whether a
        //  given memory map entry implies the need of
        //  allocating and registering memory
        //-------------------------------------------------
        bool needs_backing_store(address_map_entry entry)
        {
            // if we are sharing, and we don't have a pointer yet, create one
            if (entry.m_share != null)
            {
                string fulltag = entry.m_devbase.subtag(entry.m_share);
                var share = m_manager.shares().find(fulltag);
                if (share != null && share.ptr() == null)
                    return true;
            }

            // if we're writing to any sort of bank or RAM, then yes, we do need backing
            if (entry.m_write.m_type == map_handler_type.AMH_BANK || entry.m_write.m_type == map_handler_type.AMH_RAM)
                return true;

            // if we're reading from RAM or from ROM outside of address space 0 or its region, then yes, we do need backing
            memory_region region = m_manager.machine().root_device().memregion(m_device.tag());
            if (entry.m_read.m_type == map_handler_type.AMH_RAM ||
               (entry.m_read.m_type == map_handler_type.AMH_ROM && (m_spacenum != 0 || region == null || entry.m_addrstart >= region.bytes())))
                return true;

            // all other cases don't need backing
            return false;
        }

        //-------------------------------------------------
        //  bank_find_or_allocate - allocate a new
        //  bank, or find an existing one, and return the
        //  read/write handler
        //-------------------------------------------------
        protected memory_bank bank_find_or_allocate(string tag, offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite)
        {
            // adjust the addresses, handling mirrors and such
            offs_t addrmask = ~addrmirror;
            adjust_addresses(ref addrstart, ref addrend, ref addrmask, ref addrmirror);

            // look up the bank by name, or else by byte range
            memory_bank membank = !string.IsNullOrEmpty(tag) ? m_manager.find(tag) : m_manager.find(this, addrstart, addrend);

            // if we don't have a bank yet, create a new one
            if (membank == null)
                membank = m_manager.allocate(this, addrstart, addrend, tag);

            // add a reference for this space
            membank.add_reference(this, readorwrite);
            return membank;
        }


        //-------------------------------------------------
        //  block_assign_intersecting - find all
        //  intersecting blocks and assign their pointers
        //-------------------------------------------------
        address_map_entry block_assign_intersecting(offs_t addrstart, offs_t addrend, PointerU8 base_)  //address_map_entry *block_assign_intersecting(offs_t bytestart, offs_t byteend, u8 *base);
        {
            address_map_entry unassigned = null;

            // loop over the adjusted map and assign memory to any blocks we can
            foreach (address_map_entry entry in m_map.m_entrylist)
            {
                // if we haven't assigned this block yet, see if we have a mapped shared pointer for it
                if (entry.m_memory == null && entry.m_share != null)
                {
                    string fulltag = entry.m_devbase.subtag(entry.m_share);
                    var share = m_manager.shares().find(fulltag);
                    if (share != null && share.ptr() != null)
                    {
                        entry.m_memory = share.ptr();
                        emumem_global.VPRINTF("memory range {0:x8}-{1:x8} -> shared_ptr '{2}' [{3}]\n", entry.m_addrstart, entry.m_addrend, entry.m_share, entry.m_memory);
                    }
                    else
                    {
                        emumem_global.VPRINTF("memory range {0:x8}-{1:x8} -> shared_ptr '{2}' but not found\n", entry.m_addrstart, entry.m_addrend, entry.m_share);
                    }
                }

                // otherwise, look for a match in this block
                if (entry.m_memory == null && entry.m_addrstart >= addrstart && entry.m_addrend <= addrend)
                {
                    entry.m_memory = new PointerU8(base_, (int)m_config.addr2byte(entry.m_addrstart - addrstart));  //entry.m_memory = base + m_config.addr2byte(entry.m_addrstart - addrstart);
                    emumem_global.VPRINTF("memory range {0:x8}-{1:x8} -> found in block from {2:x8}-{3:x8} [{4}]\n", entry.m_addrstart, entry.m_addrend, addrstart, addrend, entry.m_memory);
                }

                // if we're the first match on a shared pointer, assign it now
                if (entry.m_memory != null && entry.m_share != null)
                {
                    string fulltag = entry.m_devbase.subtag(entry.m_share);
                    var share = m_manager.shares().find(fulltag);
                    if (share != null && share.ptr() == null)
                    {
                        share.set_ptr(entry.m_memory);
                        emumem_global.VPRINTF("setting shared_ptr '{0}' = {1}\n", entry.m_share, entry.m_memory);
                    }
                }

                // keep track of the first unassigned entry
                if (entry.m_memory == null && unassigned == null && needs_backing_store(entry))
                    unassigned = entry;
            }

            return unassigned;
        }
    }


    // ======================> memory_block
    // a memory block is a chunk of RAM associated with a range of memory in a device's address space
    public class memory_block
    {
        // internal state
        running_machine m_machine;              // need the machine to free our memory
        address_space m_space;                // which address space are we associated with?
        offs_t m_addrstart;
        offs_t m_addrend;  // start/end for verifying a match
        PointerU8 m_data;  //u8 *                    m_data;                 // pointer to the data for this block
        //std::vector<u8>         m_allocated;            // pointer to the actually allocated block


        // construction/destruction
        //-------------------------------------------------
        //  memory_block - constructor
        //-------------------------------------------------
        public memory_block(address_space space, offs_t addrstart, offs_t addrend, PointerU8 memory = null)  //memory_block(address_space &space, offs_t start, offs_t end, void *memory = nullptr);
        {
            m_machine = space.manager().machine();
            m_space = space;
            m_addrstart = addrstart;
            m_addrend = addrend;
            m_data = memory;


            offs_t length = space.address_to_byte(addrend + 1 - addrstart);

            emumem_global.VPRINTF("block_allocate('{0}',{1},{2:x8},{3:x8},{4})\n", space.device().tag(), space.name(), addrstart, addrend, memory);

            // allocate a block if needed
            if (m_data == null)
            {
                if (length < 4096)
                {
                    //m_allocated.resize(length);
                    //memset(&m_allocated[0], 0, length);
                    //m_data = &m_allocated[0];
                    m_data = new PointerU8(new MemoryU8((int)length, true));
                }
                else
                {
                    //m_allocated.resize(length + 0xfff);
                    //memset(&m_allocated[0], 0, length + 0xfff);
                    //m_data = reinterpret_cast<u8 *>((reinterpret_cast<uintptr_t>(&m_allocated[0]) + 0xfff) & ~0xfff);
                    m_data = new PointerU8(new MemoryU8((int)(length + 0xfff), true), (0 + 0xfff) & ~0xfff);  // ???
                }
            }


            //throw new emu_unimplemented();
#if false
            // register for saving, but only if we're not part of a memory region
            if (space.m_manager.region_containing(m_data, length) != nullptr)
                VPRINTF(("skipping save of this memory block as it is covered by a memory region\n"));
            else
            {
                int bytes_per_element = space.data_width() / 8;
                std::string name = string_format("%08x-%08x", addrstart, addrend);
                machine().save().save_memory(&space.device(), "memory", space.device().tag(), space.spacenum(), name.c_str(), m_data, bytes_per_element, (u32)length / bytes_per_element);
            }
#endif
        }


        public memory_block get() { return this; }  // for c++ smart pointers


        // getters
        //running_machine &machine() const { return m_machine; }
        public offs_t addrstart() { return m_addrstart; }
        public offs_t addrend() { return m_addrend; }
        public PointerU8 data() { return m_data; }  //u8 *data() const { return m_data; }

        // is the given range contained by this memory block?
        public bool contains(address_space space, offs_t addrstart, offs_t addrend) { return space == m_space && m_addrstart <= addrstart && m_addrend >= addrend; }
    }


    // ======================> memory_bank
    // a memory bank is a global pointer to memory that can be shared across devices and changed dynamically
    public class memory_bank
    {
        // a bank reference is an entry in a list of address spaces that reference a given bank
        class bank_reference
        {
            // internal state
            address_space m_space;            // address space that references us
            read_or_write m_readorwrite;      // used for read or write?


            // construction/destruction
            public bank_reference(address_space space, read_or_write readorwrite)
            {
                m_space = space;
                m_readorwrite = readorwrite;
            }


            // getters
            public address_space space() { return m_space; }


            // does this reference match the space+read/write combination?
            public bool matches(address_space space, read_or_write readorwrite) { return (space == m_space && (readorwrite == read_or_write.READWRITE || readorwrite == m_readorwrite)); }
        }


        // internal state
        running_machine m_machine;              // need the machine to free our memory
        std.vector<PointerRef<u8>> m_entries = new std.vector<PointerRef<u8>>();  //std::vector<u8 *>       m_entries;              // the entries
        bool m_anonymous;            // are we anonymous or explicit?
        offs_t m_addrstart;            // start offset
        offs_t m_addrend;              // end offset
        int m_curentry;             // current entry
        string m_name;                 // friendly name for this bank
        string m_tag;                  // tag for this bank
        std.vector<bank_reference> m_reflist = new std.vector<bank_reference>();  // std::vector<std::unique_ptr<bank_reference>> m_reflist;     // list of address spaces referencing this bank
        std.vector<Action<PointerU8>> m_alloc_notifier = new std.vector<Action<PointerU8>>();  //std::vector<std::function<void (void *)>> m_alloc_notifier; // list of notifier targets when allocating


        public string m_uuid = Guid.NewGuid().ToString();  // used for generating a unique name (when the pointer is used), see bank_find_or_allocate() for example


        // construction/destruction
        //-------------------------------------------------
        //  memory_bank - constructor
        //-------------------------------------------------
        public memory_bank(address_space space, int index, offs_t addrstart, offs_t addrend, string tag = null)
        {
            m_machine = space.manager().machine();
            m_anonymous = tag == null;
            m_addrstart = addrstart;
            m_addrend = addrend;
            m_curentry = 0;


            // generate an internal tag if we don't have one
            if (tag == null)
            {
                m_tag = string.Format("~{0}~", index);
                m_name = string.Format("Internal bank #{0}", index);
            }
            else
            {
                m_tag = tag;
                m_name = string.Format("Bank '{0}'", tag);
            }

            if (!m_anonymous && machine().save().registration_allowed())
                machine().save().save_item(space.device(), "memory", m_tag, 0, m_curentry, "m_curentry");
        }


        public memory_bank get() { return this; }  // for smart pointers


        // getters
        running_machine machine() { return m_machine; }
        public int entry() { return m_curentry; }
        public bool anonymous() { return m_anonymous; }
        public offs_t addrstart() { return m_addrstart; }
        public PointerRef<u8> base_() { return m_entries.empty() ? null : m_entries[m_curentry]; }
        public string tag() { return m_tag; }
        //const char *name() const { return m_name; }


        // compare a range against our range
        public bool matches_exactly(offs_t addrstart, offs_t addrend) { return m_addrstart == addrstart && m_addrend == addrend; }
        //bool fully_covers(offs_t addrstart, offs_t addrend) const { return (m_bytestart <= bytestart && m_byteend >= byteend); }
        //bool is_covered_by(offs_t addrstart, offs_t addrend) const { return (m_bytestart >= bytestart && m_byteend <= byteend); }
        //bool straddles(offs_t addrstart, offs_t addrend) const { return (m_bytestart < byteend && m_byteend > bytestart); }


        // track and verify address space references to this bank

        //-------------------------------------------------
        //  references_space - walk the list of references
        //  to find a match against the provided space
        //  and read/write
        //-------------------------------------------------
        public bool references_space(address_space space, read_or_write readorwrite)
        {
            foreach (var ref_ in m_reflist)
            {
                if (ref_.matches(space, readorwrite))
                    return true;
            }

            return false;
        }

        //-------------------------------------------------
        //  add_reference - add a new reference to the
        //  given space
        //-------------------------------------------------
        public void add_reference(address_space space, read_or_write readorwrite)
        {
            // if we already have a reference, skip it
            if (references_space(space, readorwrite))
                return;

            m_reflist.push_back(new bank_reference(space, readorwrite));
        }


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

            foreach (var cb in m_alloc_notifier)
                cb(base_);

            m_alloc_notifier.clear();
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
            // validate
            if (m_anonymous)
                throw new emu_fatalerror("memory_bank::set_entry called for anonymous bank");
            if (entrynum < 0 || entrynum >= m_entries.size())
                throw new emu_fatalerror("memory_bank::set_entry called with out-of-range entry {0}", entrynum);
            if (m_entries[entrynum] == null)
                throw new emu_fatalerror("memory_bank::set_entry called for bank '{0}' with invalid bank entry {1}", m_tag, entrynum);

            m_curentry = entrynum;
        }


        //-------------------------------------------------
        //  add_notifier - add a function used to notify when the allocation is done
        //-------------------------------------------------
        public void add_notifier(Action<PointerU8> cb)  //void add_notifier(std::function<void (void *)> cb)
        {
            m_alloc_notifier.emplace_back(cb);
        }
    }


    // ======================> memory_share
    // a memory share contains information about shared memory region
    public class memory_share
    {
        // internal state
        PointerU8 m_ptr;  //void *                  m_ptr;                  // pointer to the memory backing the region
        UInt32 m_bytes;  //size_t                  m_bytes;                // size of the shared region in bytes
        endianness_t m_endianness;           // endianness of the memory
        u8 m_bitwidth;             // width of the shared region in bits
        u8 m_bytewidth;            // width in bytes, rounded up to a power of 2


        // construction/destruction
        public memory_share(u8 width, UInt32 bytes, endianness_t endianness, PointerU8 ptr = null)  //memory_share(u8 width, size_t bytes, endianness_t endianness, void *ptr = nullptr)
        {
            m_ptr = ptr;
            m_bytes = bytes;
            m_endianness = endianness;
            m_bitwidth = width;
            m_bytewidth = (width <= 8 ? (byte)1 : width <= 16 ? (byte)2 : width <= 32 ? (byte)4 : (byte)8);
        }

        // getters
        public PointerU8 ptr() { if (this == null) return null; return m_ptr; }  //void *ptr() const { return m_ptr; }
        public u64 bytes() { return m_bytes; }
        public endianness_t endianness() { return m_endianness; }
        public u8 bitwidth() { return m_bitwidth; }
        public u8 bytewidth() { return m_bytewidth; }

        // setters
        public void set_ptr(PointerU8 ptr) { m_ptr = ptr; }  //void set_ptr(void *ptr) { m_ptr = ptr; }
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


    // ======================> memory_manager
    // holds internal state for the memory system
    public class memory_manager : global_object
    {
        //friend class address_space;
        //template<int Level, int Width, int AddrShift, endianness_t Endian> friend class address_space_specific;
        //friend memory_region::memory_region(running_machine &machine, const char *name, u32 length, u8 width, endianness_t endian);


        // internal state
        running_machine m_machine;              // reference to the machine
        public bool m_initialized;          // have we completed initialization?

        public std.vector<memory_block> m_blocklist = new std.vector<memory_block>();  //std::vector<std::unique_ptr<memory_block>>   m_blocklist;            // head of the list of memory blocks

        std.unordered_map<string, memory_bank> m_banklist = new std.unordered_map<string, memory_bank>();  //std::unordered_map<std::string, std::unique_ptr<memory_bank>>    m_banklist;             // data gathered for each bank
        std.unordered_map<string, memory_share> m_sharelist = new std.unordered_map<string, memory_share>();            // map for share lookups
        std.unordered_map<string, memory_region> m_regionlist = new std.unordered_map<string, memory_region>();           // list of memory regions


        // construction/destruction

        //-------------------------------------------------
        //  memory_manager - constructor
        //-------------------------------------------------
        public memory_manager(running_machine machine)
        {
            m_machine = machine;
            m_initialized = false;
        }

        //-------------------------------------------------
        //  initialize - initialize the memory system
        //-------------------------------------------------
        public void initialize()
        {
            // loop over devices and spaces within each device
            memory_interface_iterator iter = new memory_interface_iterator(machine().root_device());
            std.vector<device_memory_interface> memories = new std.vector<device_memory_interface>();
            foreach (device_memory_interface memory in iter)
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

            // allocate memory needed to back each address space
            foreach (var memory in memories)
                memory.allocate_memory();

            // find all the allocated pointers
            foreach (var memory in memories)
                memory.locate_memory();

            // disable logging of unmapped access when no one receives it
            //throw new emu_unimplemented();
#if false
            if (!machine().options().log() && !machine().options().oslog() && !((machine().debug_flags & machine_global.DEBUG_FLAG_ENABLED) == machine_global.DEBUG_FLAG_ENABLED))
#endif
                foreach (var memory in memories)
                    memory.set_log_unmap(false);

            // we are now initialized
            m_initialized = true;
        }


        // getters
        public running_machine machine() { return m_machine; }
        public std.unordered_map<string, memory_bank> banks() { return m_banklist; }
        public std.unordered_map<string, memory_region> regions() { return m_regionlist; }
        public std.unordered_map<string, memory_share> shares() { return m_sharelist; }


        // regions

        //-------------------------------------------------
        //  region_alloc - allocates memory for a region
        //-------------------------------------------------
        public memory_region region_alloc(string name, UInt32 length, byte width, endianness_t endian)
        {
            osd_printf_verbose("Region '{0}' created\n", name);
            // make sure we don't have a region of the same name; also find the end of the list
            if (m_regionlist.find(name) != null)
                throw new emu_fatalerror("region_alloc called with duplicate region name \"{0}\"\n", name);

            // allocate the region
            m_regionlist.emplace(name, new memory_region(machine(), name, length, width, endian));
            return m_regionlist.find(name);
        }


        //void region_free(const char *name);
        //memory_region *region_containing(const void *memory, offs_t bytes) const;


        public memory_bank find(string tag)
        {
            var bank = m_banklist.find(tag);
            if (bank != null) //m_banklist.end())
                return bank.get();

            return null;
        }

        public memory_bank find(address_space space, offs_t addrstart, offs_t addrend)
        {
            // try to find an exact match
            foreach (var bank in m_banklist)
            {
                if (bank.second().anonymous() && bank.second().references_space(space, read_or_write.READWRITE) && bank.second().matches_exactly(addrstart, addrend))
                    return bank.second().get();
            }

            // not found
            return null;
        }


        public memory_bank allocate(address_space space, offs_t addrstart, offs_t addrend, string tag = null)
        {
            var bank = new memory_bank(space, m_banklist.size(), addrstart, addrend, tag);
            string temptag;
            if (string.IsNullOrEmpty(tag))
            {
                temptag = string_format("anon_{0}", bank.get().m_uuid);  // %p
                tag = temptag.c_str();
            }

            m_banklist.emplace(tag, bank);

            return m_banklist.find(tag).get();
        }


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
                    // allocate one of the appropriate type
                    switch (spaceconfig.data_width() | (spaceconfig.addr_shift() + 4))
                    {
                        case  8|(4+1):
                            if (spaceconfig.endianness() == endianness_t.ENDIANNESS_LITTLE)
                                memory.allocate(new address_space_specific(0,  1, endianness_t.ENDIANNESS_LITTLE, this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            else
                                memory.allocate(new address_space_specific(0,  1, endianness_t.ENDIANNESS_BIG   , this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            break;

                        case  (8|(4-0)):
                            if (spaceconfig.endianness() == endianness_t.ENDIANNESS_LITTLE)
                                memory.allocate(new address_space_specific(0,  0, endianness_t.ENDIANNESS_LITTLE, this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            else
                                memory.allocate(new address_space_specific(0,  0, endianness_t.ENDIANNESS_BIG   , this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            break;

                        case 16|(4+3):
                            if (spaceconfig.endianness() == endianness_t.ENDIANNESS_LITTLE)
                                memory.allocate(new address_space_specific(1,  3, endianness_t.ENDIANNESS_LITTLE, this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            else
                                memory.allocate(new address_space_specific(1,  3, endianness_t.ENDIANNESS_BIG   , this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            break;

                        case 16|(4-0):
                            if (spaceconfig.endianness() == endianness_t.ENDIANNESS_LITTLE)
                                memory.allocate(new address_space_specific(1,  0, endianness_t.ENDIANNESS_LITTLE, this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            else
                                memory.allocate(new address_space_specific(1,  0, endianness_t.ENDIANNESS_BIG   , this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            break;

                        case 16|(4-1):
                            if (spaceconfig.endianness() == endianness_t.ENDIANNESS_LITTLE)
                                memory.allocate(new address_space_specific(1, -1, endianness_t.ENDIANNESS_LITTLE, this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            else
                                memory.allocate(new address_space_specific(1, -1, endianness_t.ENDIANNESS_BIG   , this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            break;

                        case 32|(4-0):
                            if (spaceconfig.endianness() == endianness_t.ENDIANNESS_LITTLE)
                                memory.allocate(new address_space_specific(2,  0, endianness_t.ENDIANNESS_LITTLE, this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            else
                                memory.allocate(new address_space_specific(2,  0, endianness_t.ENDIANNESS_BIG   , this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            break;

                        case 32|(4-1):
                            if (spaceconfig.endianness() == endianness_t.ENDIANNESS_LITTLE)
                                memory.allocate(new address_space_specific(2, -1, endianness_t.ENDIANNESS_LITTLE, this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            else
                                memory.allocate(new address_space_specific(2, -1, endianness_t.ENDIANNESS_BIG   , this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            break;

                        case 32|(4-2):
                            if (spaceconfig.endianness() == endianness_t.ENDIANNESS_LITTLE)
                                memory.allocate(new address_space_specific(2, -2, endianness_t.ENDIANNESS_LITTLE, this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            else
                                memory.allocate(new address_space_specific(2, -2, endianness_t.ENDIANNESS_BIG   , this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            break;

                        case 64|(4-0):
                            if (spaceconfig.endianness() == endianness_t.ENDIANNESS_LITTLE)
                                memory.allocate(new address_space_specific(3,  0, endianness_t.ENDIANNESS_LITTLE, this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            else
                                memory.allocate(new address_space_specific(3,  0, endianness_t.ENDIANNESS_BIG   , this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            break;

                        case 64|(4-1):
                            if (spaceconfig.endianness() == endianness_t.ENDIANNESS_LITTLE)
                                memory.allocate(new address_space_specific(3, -1, endianness_t.ENDIANNESS_LITTLE, this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            else
                                memory.allocate(new address_space_specific(3, -1, endianness_t.ENDIANNESS_BIG   , this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            break;

                        case 64|(4-2):
                            if (spaceconfig.endianness() == endianness_t.ENDIANNESS_LITTLE)
                                memory.allocate(new address_space_specific(3, -2, endianness_t.ENDIANNESS_LITTLE, this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            else
                                memory.allocate(new address_space_specific(3, -2, endianness_t.ENDIANNESS_BIG   , this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            break;

                        case 64|(4-3):
                            if (spaceconfig.endianness() == endianness_t.ENDIANNESS_LITTLE)
                                memory.allocate(new address_space_specific(3, -3, endianness_t.ENDIANNESS_LITTLE, this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            else
                                memory.allocate(new address_space_specific(3, -3, endianness_t.ENDIANNESS_BIG   , this, memory, spacenum, memory.space_config(spacenum).addr_width()), this, spacenum);
                            break;

                        default:
                            throw new emu_fatalerror("Invalid width {0}/shift {1} specified for address_space::allocate", spaceconfig.data_width(), spaceconfig.addr_shift());
                    }
                }
            }
        }
    }


    //**************************************************************************
    //  CONSTANTS
    //**************************************************************************

    //template <typename Delegate> struct handler_width;
    //template <> struct handler_width<read8_delegate> { static constexpr int value = 0; };
    struct handler_width_read8_delegate { public const int value = 0; }
    struct handler_width_read8m_delegate { public const int value = 0; }
    struct handler_width_read8s_delegate { public const int value = 0; }
    struct handler_width_read8sm_delegate { public const int value = 0; }
    struct handler_width_read8mo_delegate { public const int value = 0; }
    struct handler_width_read8smo_delegate { public const int value = 0; }
    struct handler_width_write8_delegate { public const int value = 0; }
    struct handler_width_write8m_delegate { public const int value = 0; }
    struct handler_width_write8s_delegate { public const int value = 0; }
    struct handler_width_write8sm_delegate { public const int value = 0; }
    struct handler_width_write8mo_delegate { public const int value = 0; }
    struct handler_width_write8smo_delegate { public const int value = 0; }
    struct handler_width_read16_delegate { public const int value = 1; }
    struct handler_width_read16m_delegate { public const int value = 1; }
    struct handler_width_read16s_delegate { public const int value = 1; }
    struct handler_width_read16sm_delegate { public const int value = 1; }
    struct handler_width_read16mo_delegate { public const int value = 1; }
    struct handler_width_read16smo_delegate { public const int value = 1; }
    struct handler_width_write16_delegate { public const int value = 1; }
    struct handler_width_write16m_delegate { public const int value = 1; }
    struct handler_width_write16s_delegate { public const int value = 1; }
    struct handler_width_write16sm_delegate { public const int value = 1; }
    struct handler_width_write16mo_delegate { public const int value = 1; }
    struct handler_width_write16smo_delegate { public const int value = 1; }
    struct handler_width_read32_delegate { public const int value = 2; }
    struct handler_width_read32m_delegate { public const int value = 2; }
    struct handler_width_read32s_delegate { public const int value = 2; }
    struct handler_width_read32sm_delegate { public const int value = 2; }
    struct handler_width_read32mo_delegate { public const int value = 2; }
    struct handler_width_read32smo_delegate { public const int value = 2; }
    struct handler_width_write32_delegate { public const int value = 2; }
    struct handler_width_write32m_delegate { public const int value = 2; }
    struct handler_width_write32s_delegate { public const int value = 2; }
    struct handler_width_write32sm_delegate { public const int value = 2; }
    struct handler_width_write32mo_delegate { public const int value = 2; }
    struct handler_width_write32smo_delegate { public const int value = 2; }
    struct handler_width_read64_delegate { public const int value = 3; }
    struct handler_width_read64m_delegate { public const int value = 3; }
    struct handler_width_read64s_delegate { public const int value = 3; }
    struct handler_width_read64sm_delegate { public const int value = 3; }
    struct handler_width_read64mo_delegate { public const int value = 3; }
    struct handler_width_read64smo_delegate { public const int value = 3; }
    struct handler_width_write64_delegate { public const int value = 3; }
    struct handler_width_write64m_delegate { public const int value = 3; }
    struct handler_width_write64s_delegate { public const int value = 3; }
    struct handler_width_write64sm_delegate { public const int value = 3; }
    struct handler_width_write64mo_delegate { public const int value = 3; }
    struct handler_width_write64smo_delegate { public const int value = 3; }

    // other address map constants
    //const int MEMORY_BLOCK_CHUNK = 65536;                   // minimum chunk size of allocated memory blocks


    // ======================> address_space_specific
    // this is a derived class of address_space with specific width, endianness, and table size
    //template<int Width, int AddrShift, endianness_t Endian>
    class address_space_specific : address_space
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using NativeType = uX;
        //using this_type = address_space_specific<Width, AddrShift, Endian>;


        static int handler_width<READORWRITE>(READORWRITE func)
        {
            if      (typeof(READORWRITE) == typeof(read8_delegate)) return handler_width_read8_delegate.value;
            else if (typeof(READORWRITE) == typeof(read8m_delegate)) return handler_width_read8m_delegate.value;
            else if (typeof(READORWRITE) == typeof(read8s_delegate)) return handler_width_read8s_delegate.value;
            else if (typeof(READORWRITE) == typeof(read8sm_delegate)) return handler_width_read8sm_delegate.value;
            else if (typeof(READORWRITE) == typeof(read8mo_delegate)) return handler_width_read8mo_delegate.value;
            else if (typeof(READORWRITE) == typeof(read8smo_delegate)) return handler_width_read8smo_delegate.value;
            else if (typeof(READORWRITE) == typeof(write8_delegate)) return handler_width_write8_delegate.value;
            else if (typeof(READORWRITE) == typeof(write8m_delegate)) return handler_width_write8m_delegate.value;
            else if (typeof(READORWRITE) == typeof(write8s_delegate)) return handler_width_write8s_delegate.value;
            else if (typeof(READORWRITE) == typeof(write8sm_delegate)) return handler_width_write8sm_delegate.value;
            else if (typeof(READORWRITE) == typeof(write8mo_delegate)) return handler_width_write8mo_delegate.value;
            else if (typeof(READORWRITE) == typeof(write8smo_delegate)) return handler_width_write8smo_delegate.value;
            else if (typeof(READORWRITE) == typeof(read16_delegate)) return handler_width_read16_delegate.value;
            else if (typeof(READORWRITE) == typeof(read16m_delegate)) return handler_width_read16m_delegate.value;
            else if (typeof(READORWRITE) == typeof(read16s_delegate)) return handler_width_read16s_delegate.value;
            else if (typeof(READORWRITE) == typeof(read16sm_delegate)) return handler_width_read16sm_delegate.value;
            else if (typeof(READORWRITE) == typeof(read16mo_delegate)) return handler_width_read16mo_delegate.value;
            else if (typeof(READORWRITE) == typeof(read16smo_delegate)) return handler_width_read16smo_delegate.value;
            else if (typeof(READORWRITE) == typeof(write16_delegate)) return handler_width_write16_delegate.value;
            else if (typeof(READORWRITE) == typeof(write16m_delegate)) return handler_width_write16m_delegate.value;
            else if (typeof(READORWRITE) == typeof(write16s_delegate)) return handler_width_write16s_delegate.value;
            else if (typeof(READORWRITE) == typeof(write16sm_delegate)) return handler_width_write16sm_delegate.value;
            else if (typeof(READORWRITE) == typeof(write16mo_delegate)) return handler_width_write16mo_delegate.value;
            else if (typeof(READORWRITE) == typeof(write16smo_delegate)) return handler_width_write16smo_delegate.value;
            else if (typeof(READORWRITE) == typeof(read32_delegate)) return handler_width_read32_delegate.value;
            else if (typeof(READORWRITE) == typeof(read32m_delegate)) return handler_width_read32m_delegate.value;
            else if (typeof(READORWRITE) == typeof(read32s_delegate)) return handler_width_read32s_delegate.value;
            else if (typeof(READORWRITE) == typeof(read32sm_delegate)) return handler_width_read32sm_delegate.value;
            else if (typeof(READORWRITE) == typeof(read32mo_delegate)) return handler_width_read32mo_delegate.value;
            else if (typeof(READORWRITE) == typeof(read32smo_delegate)) return handler_width_read32smo_delegate.value;
            else if (typeof(READORWRITE) == typeof(write32_delegate)) return handler_width_write32_delegate.value;
            else if (typeof(READORWRITE) == typeof(write32m_delegate)) return handler_width_write32m_delegate.value;
            else if (typeof(READORWRITE) == typeof(write32s_delegate)) return handler_width_write32s_delegate.value;
            else if (typeof(READORWRITE) == typeof(write32sm_delegate)) return handler_width_write32sm_delegate.value;
            else if (typeof(READORWRITE) == typeof(write32mo_delegate)) return handler_width_write32mo_delegate.value;
            else if (typeof(READORWRITE) == typeof(write32smo_delegate)) return handler_width_write32smo_delegate.value;
            else if (typeof(READORWRITE) == typeof(read64_delegate)) return handler_width_read64_delegate.value;
            else if (typeof(READORWRITE) == typeof(read64m_delegate)) return handler_width_read64m_delegate.value;
            else if (typeof(READORWRITE) == typeof(read64s_delegate)) return handler_width_read64s_delegate.value;
            else if (typeof(READORWRITE) == typeof(read64sm_delegate)) return handler_width_read64sm_delegate.value;
            else if (typeof(READORWRITE) == typeof(read64mo_delegate)) return handler_width_read64mo_delegate.value;
            else if (typeof(READORWRITE) == typeof(read64smo_delegate)) return handler_width_read64smo_delegate.value;
            else if (typeof(READORWRITE) == typeof(write64_delegate)) return handler_width_write64_delegate.value;
            else if (typeof(READORWRITE) == typeof(write64m_delegate)) return handler_width_write64m_delegate.value;
            else if (typeof(READORWRITE) == typeof(write64s_delegate)) return handler_width_write64s_delegate.value;
            else if (typeof(READORWRITE) == typeof(write64sm_delegate)) return handler_width_write64sm_delegate.value;
            else if (typeof(READORWRITE) == typeof(write64mo_delegate)) return handler_width_write64mo_delegate.value;
            else if (typeof(READORWRITE) == typeof(write64smo_delegate)) return handler_width_write64smo_delegate.value;
            else throw new emu_unimplemented();
        }


        // constants describing the native size
        u32 NATIVE_BYTES; // computed in ctor  = 1 << Width;
        u32 NATIVE_STEP; // computed in ctor  = AddrShift >= 0 ? NATIVE_BYTES << emucore_global.iabs(AddrShift) : NATIVE_BYTES >> emucore_global.iabs(AddrShift);
        u32 NATIVE_MASK; // computed in ctor  = NATIVE_STEP - 1;
        u32 NATIVE_BITS; // computed in ctor  = 8 * NATIVE_BYTES;


        // template parameter
        int Width;
        int AddrShift;
        endianness_t Endian;


        handler_entry_read m_root_read;  //handler_entry_read <Width, AddrShift, Endian> *m_root_read;
        handler_entry_write m_root_write;  //handler_entry_write<Width, AddrShift, Endian> *m_root_write;

        std.unordered_set<handler_entry> m_delayed_unrefs = new std.unordered_set<handler_entry>();


        handler_entry_read [] m_dispatch_read;  //const handler_entry_read<Width, AddrShift, Endian> *const *m_dispatch_read;
        handler_entry_write [] m_dispatch_write;  //const handler_entry_write<Width, AddrShift, Endian> *const *m_dispatch_write;


        void printf(string format, params object [] args) { osd_printf_info(format, args); }
        void assert(bool value) { assert(value); }


        offs_t offset_to_byte(offs_t offset) { return AddrShift < 0 ? offset << iabs(AddrShift) : offset >> iabs(AddrShift); }


        //-------------------------------------------------
        //  get_handler_string - return a string
        //  describing the handler at a particular offset
        //-------------------------------------------------
        protected override string get_handler_string(read_or_write readorwrite, offs_t byteaddress) { throw new emu_unimplemented(); }

        //void dump_maps(std::vector<memory_entry> &read_map, std::vector<memory_entry> &write_map) const override;


        //-------------------------------------------------
        //  unmap - unmap a section of address space
        //-------------------------------------------------
        protected override void unmap_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, bool quiet)
        {
            emumem_global.VPRINTF("address_space::unmap({0}-{1} mirror={2}, {3}, {4})\n",
                        core_i64_hex_format(addrstart, m_addrchars), core_i64_hex_format(addrend, m_addrchars),
                        core_i64_hex_format(addrmirror, m_addrchars),
                        (readorwrite == read_or_write.READ) ? "read" : (readorwrite == read_or_write.WRITE) ? "write" : (readorwrite == read_or_write.READWRITE) ? "read/write" : "??",
                        quiet ? "quiet" : "normal");

            offs_t nstart, nend, nmask, nmirror;
            check_optimize_mirror("unmap_generic", addrstart, addrend, addrmirror, out nstart, out nend, out nmask, out nmirror);

            // read space
            if (readorwrite == read_or_write.READ || readorwrite == read_or_write.READWRITE)
            {
                var handler = quiet ? (handler_entry_read)m_nop_r : (handler_entry_read)m_unmap_r;  //static_cast<handler_entry_read<Width, AddrShift, Endian> *>(quiet ? m_nop_r : m_unmap_r);
                handler.ref_();
                m_root_read.populate(nstart, nend, nmirror, handler);
            }

            // write space
            if (readorwrite == read_or_write.WRITE || readorwrite == read_or_write.READWRITE)
            {
                var handler = quiet ? (handler_entry_write)m_nop_w : (handler_entry_write)m_unmap_w;  //static_cast<handler_entry_write<Width, AddrShift, Endian> *>(quiet ? m_nop_w : m_unmap_w);
                handler.ref_();
                m_root_write.populate(nstart, nend, nmirror, handler);
            }

            invalidate_caches(readorwrite);
        }


        //-------------------------------------------------
        //  install_ram_generic - install a simple fixed
        //  RAM region into the given address space
        //-------------------------------------------------
        protected override void install_ram_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, PointerU8 baseptr)  //virtual void install_ram_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, void *baseptr)
        {
            emumem_global.VPRINTF("address_space::install_ram_generic({0}-{1} mirror={2}, {3}, {4})\n",
                        core_i64_hex_format(addrstart, m_addrchars), core_i64_hex_format(addrend, m_addrchars),
                        core_i64_hex_format(addrmirror, m_addrchars),
                        (readorwrite == read_or_write.READ) ? "read" : (readorwrite == read_or_write.WRITE) ? "write" : (readorwrite == read_or_write.READWRITE) ? "read/write" : "??",
                        baseptr == null ? "00000000" : baseptr.Count.ToString());

            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            check_optimize_mirror("install_ram_generic", addrstart, addrend, addrmirror, out nstart, out nend, out nmask, out nmirror);

            // map for read
            if (readorwrite == read_or_write.READ || readorwrite == read_or_write.READWRITE)
            {
                // find a bank and map it
                memory_bank bank = bank_find_or_allocate(null, addrstart, addrend, addrmirror, read_or_write.READ);

                // if we are provided a pointer, set it
                if (baseptr != null)
                    bank.set_base(baseptr);

                // if we don't have a bank pointer yet, try to find one
                if (bank.base_() == null || bank.base_().m_pointer == null)
                {
                    PointerU8 backing = find_backing_memory(addrstart, addrend);  //void *backing = find_backing_memory(addrstart, addrend);
                    if (backing != null)
                        bank.set_base(backing);
                }

                // if we still don't have a pointer, and we're past the initialization phase, allocate a new block
                if (bank.base_() == null && m_manager.m_initialized)
                {
                    if (m_manager.machine().phase() >= machine_phase.RESET)
                        fatalerror("Attempted to call install_ram_generic() after initialization time without a baseptr!\n");

                    var block = new memory_block(this, addrstart, addrend);
                    bank.set_base(block.get().data());
                    m_manager.m_blocklist.push_back(block);
                }

                var hand_r = new handler_entry_read_memory(Width, AddrShift, Endian, this);
                if (bank.base_() != null)
                {
                    hand_r.set_base(bank.base_());
                }
                else
                {
                    delayed_ref(hand_r);
                    bank.add_notifier((base_) => { hand_r.set_base(new PointerRef<u8>(base_)); delayed_unref(hand_r); });
                }

                hand_r.set_address_info(nstart, nmask);
                m_root_read.populate(nstart, nend, nmirror, hand_r);
            }

            // map for write
            if (readorwrite == read_or_write.WRITE || readorwrite == read_or_write.READWRITE)
            {
                // find a bank and map it
                memory_bank bank = bank_find_or_allocate(null, addrstart, addrend, addrmirror, read_or_write.WRITE);

                // if we are provided a pointer, set it
                if (baseptr != null)
                    bank.set_base(baseptr);

                // if we don't have a bank pointer yet, try to find one
                if (bank.base_() == null)
                {
                    PointerU8 backing = find_backing_memory(addrstart, addrend);  //void *backing = find_backing_memory(addrstart, addrend);
                    if (backing != null)
                        bank.set_base(backing);
                }

                // if we still don't have a pointer, and we're past the initialization phase, allocate a new block
                if (bank.base_() == null && m_manager.m_initialized)
                {
                    if (m_manager.machine().phase() >= machine_phase.RESET)
                        fatalerror("Attempted to call install_ram_generic() after initialization time without a baseptr!\n");

                    var block = new memory_block(this, address_to_byte(addrstart), address_to_byte_end(addrend));
                    bank.set_base(block.get().data());
                    m_manager.m_blocklist.push_back(block);
                }

                var hand_w = new handler_entry_write_memory(Width, AddrShift, Endian, this);
                if (bank.base_() != null)
                {
                    hand_w.set_base(bank.base_());
                }
                else
                {
                    delayed_ref(hand_w);
                    bank.add_notifier((base_) => { hand_w.set_base(new PointerRef<u8>(base_)); delayed_unref(hand_w); });
                }

                hand_w.set_address_info(nstart, nmask);
                m_root_write.populate(nstart, nend, nmirror, hand_w);
            }

            invalidate_caches(readorwrite);
        }


        //-------------------------------------------------
        //  install_bank_generic - install a range as
        //  mapping to a particular bank
        //-------------------------------------------------
        protected override void install_bank_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, string rtag, string wtag)
        {
            emumem_global.VPRINTF("address_space::install_readwrite_bank({0}-{1} mirror={2}, read=\"{3}\" / write=\"{4}\")\n",
                        core_i64_hex_format(addrstart, m_addrchars), core_i64_hex_format(addrend, m_addrchars),
                        core_i64_hex_format(addrmirror, m_addrchars),
                        rtag.empty() ? "(none)" : rtag.c_str(), wtag.empty() ? "(none)" : wtag.c_str());

            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            check_optimize_mirror("install_bank_generic", addrstart, addrend, addrmirror, out nstart, out nend, out nmask, out nmirror);

            // map the read bank
            if (rtag != "")
            {
                string fulltag = device().siblingtag(rtag);
                memory_bank bank = bank_find_or_allocate(fulltag.c_str(), addrstart, addrend, addrmirror, read_or_write.READ);

                var hand_r = new handler_entry_read_memory_bank(Width, AddrShift, Endian, this, bank);
                hand_r.set_address_info(nstart, nmask);
                m_root_read.populate(nstart, nend, nmirror, hand_r);
            }

            // map the write bank
            if (wtag != "")
            {
                string fulltag = device().siblingtag(wtag);
                memory_bank bank = bank_find_or_allocate(fulltag.c_str(), addrstart, addrend, addrmirror, read_or_write.WRITE);

                var hand_w = new handler_entry_write_memory_bank(Width, AddrShift, Endian, this, bank);
                hand_w.set_address_info(nstart, nmask);
                m_root_write.populate(nstart, nend, nmirror, hand_w);
            }

            invalidate_caches(rtag != "" ? wtag != "" ? read_or_write.READWRITE : read_or_write.READ : read_or_write.WRITE);
        }


        protected override void install_bank_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, memory_bank rbank, memory_bank wbank) { throw new emu_unimplemented(); }


        //-------------------------------------------------
        //  install_readwrite_port - install a new I/O port
        //  handler into this address space
        //-------------------------------------------------
        protected override void install_readwrite_port(offs_t addrstart, offs_t addrend, offs_t addrmirror, string rtag, string wtag)
        {
            emumem_global.VPRINTF("address_space::install_readwrite_port({0}-{1} mirror={2}, read=\"{3}\" / write=\"{4}\")\n",
                        core_i64_hex_format(addrstart, m_addrchars), core_i64_hex_format(addrend, m_addrchars),
                        core_i64_hex_format(addrmirror, m_addrchars),
                        rtag.empty() ? "(none)" : rtag.c_str(), wtag.empty() ? "(none)" : wtag.c_str());

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
                    throw new emu_fatalerror("Attempted to map non-existent port '{0}' for read in space {1} of device '{2}'\n", rtag.c_str(), name(), device().tag());

                // map the range and set the ioport
                var hand_r = new handler_entry_read_ioport(Width, AddrShift, Endian, this, port);
                m_root_read.populate(nstart, nend, nmirror, hand_r);
            }

            if (wtag != "")
            {
                // find the port
                ioport_port port = device().owner().ioport(wtag);
                if (port == null)
                    fatalerror("Attempted to map non-existent port '{0}' for write in space {1} of device '{2}'\n", wtag.c_str(), name(), device().tag());

                // map the range and set the ioport
                var hand_w = new handler_entry_write_ioport(Width, AddrShift, Endian, this, port);
                m_root_write.populate(nstart, nend, nmirror, hand_w);
            }

            invalidate_caches(rtag != "" ? wtag != "" ? read_or_write.READWRITE : read_or_write.READ : read_or_write.WRITE);
        }


        protected override void install_device_delegate(offs_t addrstart, offs_t addrend, device_t device, address_map_constructor map, u64 unitmask = 0, int cswidth = 0) { throw new emu_unimplemented(); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8_delegate rhandler, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { throw new emu_unimplemented(); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16_delegate rhandler, write16_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { throw new emu_unimplemented(); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { throw new emu_unimplemented(); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { throw new emu_unimplemented(); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32_delegate rhandler, write32_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { throw new emu_unimplemented(); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { throw new emu_unimplemented(); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { throw new emu_unimplemented(); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64_delegate rhandler, write64_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { throw new emu_unimplemented(); }

        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8m_delegate rhandler, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16m_delegate rhandler, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32m_delegate rhandler, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64m_delegate rhandler, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16sm_delegate rhandler, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32sm_delegate rhandler, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64sm_delegate rhandler, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }

        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8mo_delegate rhandler, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16mo_delegate rhandler, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32mo_delegate rhandler, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64mo_delegate rhandler, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }

        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        { install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        { install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32smo_delegate rhandler, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_read_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler); }
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_write_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler); }
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64smo_delegate rhandler, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override
        //{ install_readwrite_handler_impl(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler, whandler); }


        //using address_space::install_read_tap;
        //using address_space::install_write_tap;
        //using address_space::install_readwrite_tap;

        //virtual memory_passthrough_handler *install_read_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, uX &data, uX mem_mask)> tap, memory_passthrough_handler *mph) override;
        //virtual memory_passthrough_handler *install_write_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, uX &data, uX mem_mask)> tap, memory_passthrough_handler *mph) override;
        //virtual memory_passthrough_handler *install_readwrite_tap(offs_t addrstart, offs_t addrend, offs_t addrmirror, std::string name, std::function<void (offs_t offset, uX &data, uX mem_mask)> tapr, std::function<void (offs_t offset, uX &data, uX mem_mask)> tapw, memory_passthrough_handler *mph) override;


        // construction/destruction
        public address_space_specific(int width, int addrShift, endianness_t endian, memory_manager manager, device_memory_interface memory, int spacenum, int address_width)
            : base(manager, memory, spacenum)
        {
            Width = width;
            AddrShift = addrShift;
            Endian = endian;


            NATIVE_BYTES = 1U << Width;
            NATIVE_STEP = AddrShift >= 0 ? NATIVE_BYTES << iabs(AddrShift) : NATIVE_BYTES >> iabs(AddrShift);
            NATIVE_MASK = NATIVE_STEP - 1;
            NATIVE_BITS = 8 * NATIVE_BYTES;


            m_unmap_r = new handler_entry_read_unmapped(Width, AddrShift, Endian, this);
            m_unmap_w = new handler_entry_write_unmapped(Width, AddrShift, Endian, this);
            m_nop_r = new handler_entry_read_nop(Width, AddrShift, Endian, this);
            m_nop_w = new handler_entry_write_nop(Width, AddrShift, Endian, this);

            handler_entry.range r = new handler_entry.range() { start = 0, end = 0xffffffff >> (32 - address_width) };

            switch (address_width)
            {
                case  1: m_root_read = new handler_entry_read_dispatch( Math.Max(1, Width), Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( Math.Max(1, Width), Width, AddrShift, Endian, this, r, null); break;
                case  2: m_root_read = new handler_entry_read_dispatch( Math.Max(2, Width), Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( Math.Max(2, Width), Width, AddrShift, Endian, this, r, null); break;
                case  3: m_root_read = new handler_entry_read_dispatch( Math.Max(3, Width), Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( Math.Max(3, Width), Width, AddrShift, Endian, this, r, null); break;
                case  4: m_root_read = new handler_entry_read_dispatch( 4, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( 4, Width, AddrShift, Endian, this, r, null); break;
                case  5: m_root_read = new handler_entry_read_dispatch( 5, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( 5, Width, AddrShift, Endian, this, r, null); break;
                case  6: m_root_read = new handler_entry_read_dispatch( 6, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( 6, Width, AddrShift, Endian, this, r, null); break;
                case  7: m_root_read = new handler_entry_read_dispatch( 7, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( 7, Width, AddrShift, Endian, this, r, null); break;
                case  8: m_root_read = new handler_entry_read_dispatch( 8, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( 8, Width, AddrShift, Endian, this, r, null); break;
                case  9: m_root_read = new handler_entry_read_dispatch( 9, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( 9, Width, AddrShift, Endian, this, r, null); break;
                case 10: m_root_read = new handler_entry_read_dispatch(10, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(10, Width, AddrShift, Endian, this, r, null); break;
                case 11: m_root_read = new handler_entry_read_dispatch(11, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(11, Width, AddrShift, Endian, this, r, null); break;
                case 12: m_root_read = new handler_entry_read_dispatch(12, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(12, Width, AddrShift, Endian, this, r, null); break;
                case 13: m_root_read = new handler_entry_read_dispatch(13, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(13, Width, AddrShift, Endian, this, r, null); break;
                case 14: m_root_read = new handler_entry_read_dispatch(14, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(14, Width, AddrShift, Endian, this, r, null); break;
                case 15: m_root_read = new handler_entry_read_dispatch(15, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(15, Width, AddrShift, Endian, this, r, null); break;
                case 16: m_root_read = new handler_entry_read_dispatch(16, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(16, Width, AddrShift, Endian, this, r, null); break;
                case 17: m_root_read = new handler_entry_read_dispatch(17, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(17, Width, AddrShift, Endian, this, r, null); break;
                case 18: m_root_read = new handler_entry_read_dispatch(18, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(18, Width, AddrShift, Endian, this, r, null); break;
                case 19: m_root_read = new handler_entry_read_dispatch(19, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(19, Width, AddrShift, Endian, this, r, null); break;
                case 20: m_root_read = new handler_entry_read_dispatch(20, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(20, Width, AddrShift, Endian, this, r, null); break;
                case 21: m_root_read = new handler_entry_read_dispatch(21, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(21, Width, AddrShift, Endian, this, r, null); break;
                case 22: m_root_read = new handler_entry_read_dispatch(22, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(22, Width, AddrShift, Endian, this, r, null); break;
                case 23: m_root_read = new handler_entry_read_dispatch(23, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(23, Width, AddrShift, Endian, this, r, null); break;
                case 24: m_root_read = new handler_entry_read_dispatch(24, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(24, Width, AddrShift, Endian, this, r, null); break;
                case 25: m_root_read = new handler_entry_read_dispatch(25, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(25, Width, AddrShift, Endian, this, r, null); break;
                case 26: m_root_read = new handler_entry_read_dispatch(26, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(26, Width, AddrShift, Endian, this, r, null); break;
                case 27: m_root_read = new handler_entry_read_dispatch(27, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(27, Width, AddrShift, Endian, this, r, null); break;
                case 28: m_root_read = new handler_entry_read_dispatch(28, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(28, Width, AddrShift, Endian, this, r, null); break;
                case 29: m_root_read = new handler_entry_read_dispatch(29, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(29, Width, AddrShift, Endian, this, r, null); break;
                case 30: m_root_read = new handler_entry_read_dispatch(30, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(30, Width, AddrShift, Endian, this, r, null); break;
                case 31: m_root_read = new handler_entry_read_dispatch(31, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(31, Width, AddrShift, Endian, this, r, null); break;
                case 32: m_root_read = new handler_entry_read_dispatch(32, Width, AddrShift, Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(32, Width, AddrShift, Endian, this, r, null); break;
                default: fatalerror("Unhandled address bus width {0}\n", address_width); break;
            }

            m_dispatch_read = m_root_read.get_dispatch();
            m_dispatch_write = m_root_write.get_dispatch();
        }


        protected override std.pair<object, object> get_cache_info()
        {
            std.pair<object, object> rw = new std.pair<object, object>(m_root_read, m_root_write);  //std::pair<void *, void *> rw;
            //rw.first  = m_root_read;
            //rw.second = m_root_write;
            return rw;
        }

        protected override std.pair<object, object> get_specific_info()
        {
            std.pair<object, object> rw = new std.pair<object, object>(m_dispatch_read, m_dispatch_write);  //std::pair<const void *, const void *> rw;
            //rw.first  = m_dispatch_read;
            //rw.second = m_dispatch_write;
            return rw;
        }


        void delayed_ref(handler_entry e)
        {
            e.ref_();
            m_delayed_unrefs.insert(e);
        }

        void delayed_unref(handler_entry e)
        {
            m_delayed_unrefs.erase(e);  //m_delayed_unrefs.find(e));
            e.unref();
        }


        // accessors
        protected override void validate_reference_counts()
        {
            //throw new emu_unimplemented();
#if false
            handler_entry.reflist refs;
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

        //virtual void remove_passthrough(std::unordered_set<handler_entry *> &handlers) override {
        //    invalidate_caches(read_or_write::READWRITE);
        //    m_root_read->detach(handlers);
        //    m_root_write->detach(handlers);
        //}


        // generate accessor table
        //virtual void accessors(data_accessors &accessors) const

        // return a pointer to the read bank, or NULL if none
        //virtual void *get_read_ptr(offs_t address)

        // return a pointer to the write bank, or NULL if none
        //virtual void *get_write_ptr(offs_t address)


        // native read
        uX read_native(offs_t offset, uX mask)  //NativeType read_native(offs_t offset, NativeType mask)
        {
            profiler_global.g_profiler.start(profile_type.PROFILER_MEMREAD);

            uX result = m_root_read.read(Width, AddrShift, Endian, offset, mask);

            profiler_global.g_profiler.stop();
            return result;
        }


        // mask-less native read
        uX read_native(offs_t offset)  //NativeType read_native(offs_t offset)
        {
            profiler_global.g_profiler.start(profile_type.PROFILER_MEMREAD);

            uX result = m_root_read.read(Width, AddrShift, Endian, offset, new uX(Width, 0xffffffffffffffffU));

            profiler_global.g_profiler.stop();
            return result;
        }


        // native write
        void write_native(offs_t offset, uX data, uX mask)  //void write_native(offs_t offset, NativeType data, NativeType mask)
        {
            profiler_global.g_profiler.start(profile_type.PROFILER_MEMWRITE);

            m_root_write.write(Width, AddrShift, Endian, offset, data, mask);

            profiler_global.g_profiler.stop();
        }


        // mask-less native write
        void write_native(offs_t offset, uX data)  //void write_native(offs_t offset, NativeType data)
        {
            profiler_global.g_profiler.start(profile_type.PROFILER_MEMWRITE);

            m_root_write.write(Width, AddrShift, Endian, offset, data, new uX(Width, 0xffffffffffffffffU));

            profiler_global.g_profiler.stop();
        }


        // virtual access to these functions
        public override u8 read_byte(offs_t address) { address &= addrmask(); return Width == 0 ? read_native(address & ~NATIVE_MASK).x8 : memory_read_generic((int)Width, AddrShift, Endian, 0, true, (offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(0, 0xff)).x8; }  //u8 read_byte(offs_t address) override { address &= m_addrmask; return Width == 0 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 0, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xff); }
        public override u16 read_word(offs_t address) { address &= addrmask(); return Width == 1 ? read_native(address & ~NATIVE_MASK).x16 : memory_read_generic((int)Width, AddrShift, Endian, 1, true, (offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(1, 0xffff)).x16; }  //u16 read_word(offs_t address) override { address &= m_addrmask; return Width == 1 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffff); }
        public override u16 read_word(offs_t address, u16 mask) { address &= addrmask(); return memory_read_generic((int)Width, AddrShift, Endian, 1, true, (offs_t offset, uX mask2) => { return read_native(offset, mask2); }, address, new uX(1, mask)).x16; }  //u16 read_word(offs_t address, u16 mask) override { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        public override u16 read_word_unaligned(offs_t address) { address &= addrmask(); return memory_read_generic((int)Width, AddrShift, Endian, 1, false, (offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(1, 0xffff)).x16; }  //u16 read_word_unaligned(offs_t address) override { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffff); }
        public override u16 read_word_unaligned(offs_t address, u16 mask) { address &= addrmask(); return memory_read_generic((int)Width, AddrShift, Endian, 1, false, (offs_t offset, uX mask2) => { return read_native(offset, mask2); }, address, new uX(1, mask)).x16; }  //u16 read_word_unaligned(offs_t address, u16 mask) override { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        public override u32 read_dword(offs_t address) { address &= addrmask(); return Width == 2 ? read_native(address & ~NATIVE_MASK).x32 : memory_read_generic((int)Width, AddrShift, Endian, 2, true, (offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(2, 0xffffffff)).x32; }  //u32 read_dword(offs_t address) override { address &= m_addrmask; return Width == 2 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffff); }
        public override u32 read_dword(offs_t address, u32 mask) { address &= addrmask(); return memory_read_generic((int)Width, AddrShift, Endian, 2, true, (offs_t offset, uX mask2) => { return read_native(offset, mask2); }, address, new uX(2, mask)).x32; }  //u32 read_dword(offs_t address, u32 mask) override { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        public override u32 read_dword_unaligned(offs_t address) { address &= addrmask(); return memory_read_generic((int)Width, AddrShift, Endian, 2, false, (offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(2, 0xffffffff)).x32; }  //u32 read_dword_unaligned(offs_t address) override { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffff); }
        public override u32 read_dword_unaligned(offs_t address, u32 mask) { address &= addrmask(); return memory_read_generic((int)Width, AddrShift, Endian, 2, false, (offs_t offset, uX mask2) => { return read_native(offset, mask2); }, address, new uX(2, mask)).x32; }  //u32 read_dword_unaligned(offs_t address, u32 mask) override { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        public override u64 read_qword(offs_t address) { address &= addrmask(); return Width == 3 ? read_native(address & ~NATIVE_MASK).x64 : memory_read_generic((int)Width, AddrShift, Endian, 3, true, (offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(3, 0xffffffffffffffffU)).x64; }  //u64 read_qword(offs_t address) override { address &= m_addrmask; return Width == 3 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffffffffffffU); }
        public override u64 read_qword(offs_t address, u64 mask) { address &= addrmask(); return memory_read_generic((int)Width, AddrShift, Endian, 3, true, (offs_t offset, uX mask2) => { return read_native(offset, mask2); }, address, new uX(3, mask)).x64; }  //u64 read_qword(offs_t address, u64 mask) override { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        public override u64 read_qword_unaligned(offs_t address) { address &= addrmask(); return memory_read_generic((int)Width, AddrShift, Endian, 3, false, (offs_t offset, uX mask) => { return read_native(offset, mask); }, address, new uX(3, 0xffffffffffffffffU)).x64; }  //u64 read_qword_unaligned(offs_t address) override { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffffffffffffU); }
        public override u64 read_qword_unaligned(offs_t address, u64 mask) { address &= addrmask(); return memory_read_generic((int)Width, AddrShift, Endian, 3, false, (offs_t offset, uX mask2) => { return read_native(offset, mask2); }, address, new uX(3, mask)).x64; }  //u64 read_qword_unaligned(offs_t address, u64 mask) override { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }

        public override void write_byte(offs_t address, u8 data) { address &= addrmask(); if (Width == 0) write_native(address & ~NATIVE_MASK, new uX(0, data)); else memory_write_generic((int)Width, AddrShift, Endian, 0, true, (offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(0, data), new uX(0, 0xff)); }  //void write_byte(offs_t address, u8 data) override { address &= m_addrmask; if (Width == 0) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 0, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xff); }
        public override void write_word(offs_t address, u16 data) { address &= addrmask(); if (Width == 1) write_native(address & ~NATIVE_MASK, new uX(1, data)); else memory_write_generic((int)Width, AddrShift, Endian, 1, true, (offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(1, data), new uX(1, 0xffff)); }  //void write_word(offs_t address, u16 data) override { address &= m_addrmask; if (Width == 1) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffff); }
        public override void write_word(offs_t address, u16 data, u16 mask) { address &= addrmask(); memory_write_generic((int)Width, AddrShift, Endian, 1, true, (offs_t offset, uX data2, uX mask2) => { write_native(offset, data2, mask2); }, address, new uX(1, data), new uX(1, mask)); }  //void write_word(offs_t address, u16 data, u16 mask) override { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        public override void write_word_unaligned(offs_t address, u16 data) { address &= addrmask(); memory_write_generic((int)Width, AddrShift, Endian, 1, false, (offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(1, data), new uX(1, 0xffff)); }  //void write_word_unaligned(offs_t address, u16 data) override { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffff); }
        public override void write_word_unaligned(offs_t address, u16 data, u16 mask) { address &= addrmask(); memory_write_generic((int)Width, AddrShift, Endian, 1, false, (offs_t offset, uX data2, uX mask2) => { write_native(offset, data2, mask2); }, address, new uX(1, data), new uX(1, mask)); }  //void write_word_unaligned(offs_t address, u16 data, u16 mask) override { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        public override void write_dword(offs_t address, u32 data) { address &= addrmask(); if (Width == 2) write_native(address & ~NATIVE_MASK, new uX(2, data)); else memory_write_generic((int)Width, AddrShift, Endian, 2, true, (offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(2, data), new uX(2, 0xffffffff)); }  //void write_dword(offs_t address, u32 data) override { address &= m_addrmask; if (Width == 2) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffff); }
        public override void write_dword(offs_t address, u32 data, u32 mask) { address &= addrmask(); memory_write_generic((int)Width, AddrShift, Endian, 2, true, (offs_t offset, uX data2, uX mask2) => { write_native(offset, data2, mask2); }, address, new uX(2, data), new uX(2, mask)); }  //void write_dword(offs_t address, u32 data, u32 mask) override { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        public override void write_dword_unaligned(offs_t address, u32 data) { address &= addrmask(); memory_write_generic((int)Width, AddrShift, Endian, 2, false, (offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(2, data), new uX(2, 0xffffffff)); }  //void write_dword_unaligned(offs_t address, u32 data) override { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffff); }
        public override void write_dword_unaligned(offs_t address, u32 data, u32 mask) { address &= addrmask(); memory_write_generic((int)Width, AddrShift, Endian, 2, false, (offs_t offset, uX data2, uX mask2) => { write_native(offset, data2, mask2); }, address, new uX(2, data), new uX(2, mask)); }  //void write_dword_unaligned(offs_t address, u32 data, u32 mask) override { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        public override void write_qword(offs_t address, u64 data) { address &= addrmask(); if (Width == 3) write_native(address & ~NATIVE_MASK, new uX(3, data)); else memory_write_generic((int)Width, AddrShift, Endian, 3, true, (offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(3, data), new uX(3, 0xffffffffffffffffU)); }  //void write_qword(offs_t address, u64 data) override { address &= m_addrmask; if (Width == 3) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffffffffffffU); }
        public override void write_qword(offs_t address, u64 data, u64 mask) { address &= addrmask(); memory_write_generic((int)Width, AddrShift, Endian, 3, true, (offs_t offset, uX data2, uX mask2) => { write_native(offset, data2, mask2); }, address, new uX(3, data), new uX(3, mask)); }  //void write_qword(offs_t address, u64 data, u64 mask) override { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        public override void write_qword_unaligned(offs_t address, u64 data) { address &= addrmask(); memory_write_generic((int)Width, AddrShift, Endian, 3, false, (offs_t offset, uX data2, uX mask) => { write_native(offset, data2, mask); }, address, new uX(3, data), new uX(3, 0xffffffffffffffffU)); }  //void write_qword_unaligned(offs_t address, u64 data) override { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffffffffffffU); }
        public override void write_qword_unaligned(offs_t address, u64 data, u64 mask) { address &= addrmask(); memory_write_generic((int)Width, AddrShift, Endian, 3, false, (offs_t offset, uX data2, uX mask2) => { write_native(offset, data2, mask2); }, address, new uX(3, data), new uX(3, mask)); }  //void write_qword_unaligned(offs_t address, u64 data, u64 mask) override {address &= m_addrmask;  memory_write_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }


        // static access to these functions
        //static u8 read_byte_static(this_type &space, offs_t address) { address &= space.m_addrmask; return Width == 0 ? space.read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 0, true>([&space](offs_t offset, NativeType mask) -> NativeType { return space.read_native(offset, mask); }, address, 0xff); }
        //static u16 read_word_static(this_type &space, offs_t address) { address &= space.m_addrmask; return Width == 1 ? space.read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 1, true>([&space](offs_t offset, NativeType mask) -> NativeType { return space.read_native(offset, mask); }, address, 0xffff); }
        //static u16 read_word_masked_static(this_type &space, offs_t address, u16 mask) { address &= space.m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 1, true>([&space](offs_t offset, NativeType mask) -> NativeType { return space.read_native(offset, mask); }, address, mask); }
        //static u32 read_dword_static(this_type &space, offs_t address) { address &= space.m_addrmask; return Width == 2 ? space.read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 2, true>([&space](offs_t offset, NativeType mask) -> NativeType { return space.read_native(offset, mask); }, address, 0xffffffff); }
        //static u32 read_dword_masked_static(this_type &space, offs_t address, u32 mask) { address &= space.m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 2, true>([&space](offs_t offset, NativeType mask) -> NativeType { return space.read_native(offset, mask); }, address, mask); }
        //static u64 read_qword_static(this_type &space, offs_t address) { address &= space.m_addrmask; return Width == 3 ? space.read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 3, true>([&space](offs_t offset, NativeType mask) -> NativeType { return space.read_native(offset, mask); }, address, 0xffffffffffffffffU); }
        //static u64 read_qword_masked_static(this_type &space, offs_t address, u64 mask) { address &= space.m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 3, true>([&space](offs_t offset, NativeType mask) -> NativeType { return space.read_native(offset, mask); }, address, mask); }
        //static void write_byte_static(this_type &space, offs_t address, u8 data) { address &= space.m_addrmask; if (Width == 0) space.write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 0, true>([&space](offs_t offset, NativeType data, NativeType mask) { space.write_native(offset, data, mask); }, address, data, 0xff); }
        //static void write_word_static(this_type &space, offs_t address, u16 data) { address &= space.m_addrmask; if (Width == 1) space.write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 1, true>([&space](offs_t offset, NativeType data, NativeType mask) { space.write_native(offset, data, mask); }, address, data, 0xffff); }
        //static void write_word_masked_static(this_type &space, offs_t address, u16 data, u16 mask) { address &= space.m_addrmask; memory_write_generic<Width, AddrShift, Endian, 1, true>([&space](offs_t offset, NativeType data, NativeType mask) { space.write_native(offset, data, mask); }, address, data, mask); }
        //static void write_dword_static(this_type &space, offs_t address, u32 data) { address &= space.m_addrmask; if (Width == 2) space.write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 2, true>([&space](offs_t offset, NativeType data, NativeType mask) { space.write_native(offset, data, mask); }, address, data, 0xffffffff); }
        //static void write_dword_masked_static(this_type &space, offs_t address, u32 data, u32 mask) { address &= space.m_addrmask; memory_write_generic<Width, AddrShift, Endian, 2, true>([&space](offs_t offset, NativeType data, NativeType mask) { space.write_native(offset, data, mask); }, address, data, mask); }
        //static void write_qword_static(this_type &space, offs_t address, u64 data) { address &= space.m_addrmask; if (Width == 3) space.write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 3, false>([&space](offs_t offset, NativeType data, NativeType mask) { space.write_native(offset, data, mask); }, address, data, 0xffffffffffffffffU); }
        //static void write_qword_masked_static(this_type &space, offs_t address, u64 data, u64 mask) { address &= space.m_addrmask; memory_write_generic<Width, AddrShift, Endian, 3, false>([&space](offs_t offset, NativeType data, NativeType mask) { space.write_native(offset, data, mask); }, address, data, mask); }


        //template<typename READ>
        void install_read_handler_impl<READ>(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ handler_r)  //void install_read_handler_impl(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ &handler_r)
        {
            try { }  //handler_r.resolve(); }
            catch (binding_type_exception)
            {
                osd_printf_error("Binding error while installing read handler {0} for range 0x{1}-0x{2} mask 0x{3} mirror 0x{4} select 0x{5} umask 0x{6}\n", handler_r, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask);
                throw;
            }
            install_read_handler_helper(handler_width(handler_r), addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_r);  //install_read_handler_helper<handler_width<READ>::value>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_r);
        }


        //template<typename WRITE>
        void install_write_handler_impl<WRITE>(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, WRITE handler_w)  //void install_write_handler_impl(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, WRITE &handler_w)
        {
            try { }  //handler_w.resolve(); }
            catch (binding_type_exception)
            {
                osd_printf_error("Binding error while installing write handler {0} for range 0x{1}-0x{2} mask 0x{3} mirror 0x{4} select 0x{5} umask 0x{6}\n", handler_w, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask);
                throw;
            }
            install_write_handler_helper(handler_width(handler_w), addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_w);  //install_write_handler_helper<handler_width<WRITE>::value>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_w);
        }


        //template<typename READ, typename WRITE>
        //void install_readwrite_handler_impl(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ handler_r, WRITE handler_w)  //void install_readwrite_handler_impl(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ &handler_r, WRITE &handler_w)
        //{
        //    static_assert(handler_width<READ>::value == handler_width<WRITE>::value, "handler widths do not match");
        //    try { handler_r.resolve(); }
        //    catch (binding_type_exception)
        //    {
        //        osd_printf_error("Binding error while installing read handler %s for range 0x%X-0x%X mask 0x%X mirror 0x%X select 0x%X umask 0x%X\n", handler_r.name(), addrstart, addrend, addrmask, addrmirror, addrselect, unitmask);
        //        throw;
        //    }
        //    try { handler_w.resolve(); }
        //    catch (binding_type_exception)
        //    {
        //        osd_printf_error("Binding error while installing write handler %s for range 0x%X-0x%X mask 0x%X mirror 0x%X select 0x%X umask 0x%X\n", handler_w.name(), addrstart, addrend, addrmask, addrmirror, addrselect, unitmask);
        //        throw;
        //    }
        //    install_readwrite_handler_helper<handler_width<READ>::value>(addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_r, handler_w);
        //}


        void install_read_handler_helper<READ>(int AccessWidth, offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ handler_r)
        {
            if (Width == AccessWidth)     install_read_handler_helper_eq(AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_r);
            else if (Width > AccessWidth) install_read_handler_helper_gt(AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_r);
            else                          install_read_handler_helper_lt(AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_r);
        }


        //template<int AccessWidth, typename READ> std::enable_if_t<(Width == AccessWidth)>
        void install_read_handler_helper_eq<READ>(int AccessWidth, offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ handler_r)  //install_read_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, const READ &handler_r)
        {
            emumem_global.VPRINTF("address_space::install_read_handler({0}-{1} mask={2} mirror={3}, space width={4}, handler width={5}, {6}, {7})\n",
                     core_i64_hex_format(addrstart, m_addrchars), core_i64_hex_format(addrend, m_addrchars),
                     core_i64_hex_format(addrmask, m_addrchars), core_i64_hex_format(addrmirror, m_addrchars),
                     8 << Width, 8 << AccessWidth,
                     handler_r, core_i64_hex_format(unitmask, (u8)(data_width() / 4)));

            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            u64 nunitmask;
            int ncswidth;
            check_optimize_all("install_read_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, out nstart, out nend, out nmask, out nmirror, out nunitmask, out ncswidth);

            var hand_r = new handler_entry_read_delegate(Width, AddrShift, Endian, this, handler_r);
            hand_r.set_address_info(nstart, nmask);
            m_root_read.populate(nstart, nend, nmirror, hand_r);
            invalidate_caches(read_or_write.READ);
        }


        //template<int AccessWidth, typename READ> std::enable_if_t<(Width > AccessWidth)>
        void install_read_handler_helper_gt<READ>(int AccessWidth, offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ handler_r)  //install_read_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, const READ &handler_r)
        {
            emumem_global.VPRINTF("address_space::install_read_handler({0}-{1} mask={2} mirror={3}, space width={4}, handler width={5}, {6}, {7})\n",
                     core_i64_hex_format(addrstart, m_addrchars), core_i64_hex_format(addrend, m_addrchars),
                     core_i64_hex_format(addrmask, m_addrchars), core_i64_hex_format(addrmirror, m_addrchars),
                     8 << Width, 8 << AccessWidth,
                     handler_r, core_i64_hex_format(unitmask, (u8)(data_width() / 4)));

            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            u64 nunitmask;
            int ncswidth;
            check_optimize_all("install_read_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, out nstart, out nend, out nmask, out nmirror, out nunitmask, out ncswidth);

            var hand_r = new handler_entry_read_delegate(AccessWidth, -AccessWidth, Endian, this, handler_r);
            memory_units_descriptor descriptor = new memory_units_descriptor(Width, AddrShift, Endian, (u8)AccessWidth, (u8)Endian, hand_r, nstart, nend, nmask, new uX(Width, nunitmask), ncswidth);  //memory_units_descriptor<Width, AddrShift, Endian> descriptor(AccessWidth, Endian, hand_r, nstart, nend, nmask, nunitmask, ncswidth);
            hand_r.set_address_info(descriptor.get_handler_start(), descriptor.get_handler_mask());
            m_root_read.populate_mismatched(nstart, nend, nmirror, descriptor);
            hand_r.unref();
            invalidate_caches(read_or_write.READ);
        }


        //template<int AccessWidth, typename READ> std::enable_if_t<(Width < AccessWidth)>
        void install_read_handler_helper_lt<READ>(int AccessWidth, offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, READ handler_r)  //install_read_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, const READ &handler_r)
        {
            fatalerror("install_read_handler: cannot install a {0}-wide handler in a {1}-wide bus", 8 << AccessWidth, 8 << Width);
        }


        void install_write_handler_helper<WRITE>(int AccessWidth, offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, WRITE handler_w)
        {
            if (Width == AccessWidth)     install_write_handler_helper_eq(AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_w);
            else if (Width > AccessWidth) install_write_handler_helper_gt(AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_w);
            else                          install_write_handler_helper_lt(AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, handler_w);
        }


        //template<int AccessWidth, typename WRITE> std::enable_if_t<(Width == AccessWidth)>
        void install_write_handler_helper_eq<WRITE>(int AccessWidth, offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, WRITE handler_w)  //install_write_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, const WRITE &handler_w)
        {
            emumem_global.VPRINTF("address_space::install_write_handler({0}-{1} mask={2} mirror={3}, space width={4}, handler width={5}, {6}, {7})\n",
                     core_i64_hex_format(addrstart, m_addrchars), core_i64_hex_format(addrend, m_addrchars),
                     core_i64_hex_format(addrmask, m_addrchars), core_i64_hex_format(addrmirror, m_addrchars),
                     8 << Width, 8 << AccessWidth,
                     handler_w, core_i64_hex_format(unitmask, (byte)(data_width() / 4)));

            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            u64 nunitmask;
            int ncswidth;
            check_optimize_all("install_write_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, out nstart, out nend, out nmask, out nmirror, out nunitmask, out ncswidth);

            var hand_w = new handler_entry_write_delegate(Width, AddrShift, Endian, this, handler_w);
            hand_w.set_address_info(nstart, nmask);
            m_root_write.populate(nstart, nend, nmirror, hand_w);
            invalidate_caches(read_or_write.WRITE);
        }


        //template<int AccessWidth, typename WRITE> std::enable_if_t<(Width > AccessWidth)>
        void install_write_handler_helper_gt<WRITE>(int AccessWidth, offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, WRITE handler_w)  //install_write_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, const WRITE &handler_w)
        {
            emumem_global.VPRINTF("address_space::install_write_handler({0}-{1} mask={2} mirror={3}, space width={4}, handler width={5}, {6}, {7})\n",
                     core_i64_hex_format(addrstart, m_addrchars), core_i64_hex_format(addrend, m_addrchars),
                     core_i64_hex_format(addrmask, m_addrchars), core_i64_hex_format(addrmirror, m_addrchars),
                     8 << Width, 8 << AccessWidth,
                     handler_w, core_i64_hex_format(unitmask, (byte)(data_width() / 4)));

            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            u64 nunitmask;
            int ncswidth;
            check_optimize_all("install_write_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, out nstart, out nend, out nmask, out nmirror, out nunitmask, out ncswidth);

            var hand_w = new handler_entry_write_delegate(AccessWidth, -AccessWidth, Endian, this, handler_w);
            memory_units_descriptor descriptor = new memory_units_descriptor(Width, AddrShift, Endian, (u8)AccessWidth, (u8)Endian, hand_w, nstart, nend, nmask, new uX(Width, nunitmask), ncswidth);  //memory_units_descriptor<Width, AddrShift, Endian> descriptor(AccessWidth, Endian, hand_w, nstart, nend, nmask, nunitmask, ncswidth);
            hand_w.set_address_info(descriptor.get_handler_start(), descriptor.get_handler_mask());
            m_root_write.populate_mismatched(nstart, nend, nmirror, descriptor);
            hand_w.unref();
            invalidate_caches(read_or_write.WRITE);
        }


        //template<int AccessWidth, typename WRITE> std::enable_if_t<(Width < AccessWidth)>
        void install_write_handler_helper_lt<WRITE>(int AccessWidth, offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, WRITE handler_w)  //install_write_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth, const WRITE &handler_w)
        {
            fatalerror("install_write_handler: cannot install a {0}-wide handler in a {1}-wide bus", 8 << AccessWidth, 8 << Width);
        }


#if false
        template<int AccessWidth, typename READ, typename WRITE> std::enable_if_t<(Width == AccessWidth)>
        install_readwrite_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
                                         const READ  &handler_r,
                                         const WRITE &handler_w)
        {
            VPRINTF(("address_space::install_readwrite_handler(%s-%s mask=%s mirror=%s, space width=%d, handler width=%d, %s, %s, %s)\n",
                     core_i64_hex_format(addrstart, m_addrchars), core_i64_hex_format(addrend, m_addrchars),
                     core_i64_hex_format(addrmask, m_addrchars), core_i64_hex_format(addrmirror, m_addrchars),
                     8 << Width, 8 << AccessWidth,
                     handler_r.name(), handler_w.name(), core_i64_hex_format(unitmask, data_width() / 4)));

            offs_t nstart, nend, nmask, nmirror;
            u64 nunitmask;
            int ncswidth;
            check_optimize_all("install_readwrite_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, nstart, nend, nmask, nmirror, nunitmask, ncswidth);

            auto hand_r = new handler_entry_read_delegate <Width, AddrShift, Endian, READ>(this, handler_r);
            hand_r->set_address_info(nstart, nmask);
            m_root_read ->populate(nstart, nend, nmirror, hand_r);

            auto hand_w = new handler_entry_write_delegate<Width, AddrShift, Endian, WRITE>(this, handler_w);
            hand_w->set_address_info(nstart, nmask);
            m_root_write->populate(nstart, nend, nmirror, hand_w);

            invalidate_caches(read_or_write::READWRITE);
        }

        template<int AccessWidth, typename READ, typename WRITE> std::enable_if_t<(Width > AccessWidth)>
        install_readwrite_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
                                         const READ  &handler_r,
                                         const WRITE &handler_w)
        {
            VPRINTF(("address_space::install_readwrite_handler(%s-%s mask=%s mirror=%s, space width=%d, handler width=%d, %s, %s, %s)\n",
                     core_i64_hex_format(addrstart, m_addrchars), core_i64_hex_format(addrend, m_addrchars),
                     core_i64_hex_format(addrmask, m_addrchars), core_i64_hex_format(addrmirror, m_addrchars),
                     8 << Width, 8 << AccessWidth,
                     handler_r.name(), handler_w.name(), core_i64_hex_format(unitmask, data_width() / 4)));

            offs_t nstart, nend, nmask, nmirror;
            u64 nunitmask;
            int ncswidth;
            check_optimize_all("install_readwrite_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, nstart, nend, nmask, nmirror, nunitmask, ncswidth);

            auto hand_r = new handler_entry_read_delegate <AccessWidth, -AccessWidth, Endian, READ>(this, handler_r);
            memory_units_descriptor<Width, AddrShift, Endian> descriptor(AccessWidth, Endian, hand_r, nstart, nend, nmask, nunitmask, ncswidth);
            hand_r->set_address_info(descriptor.get_handler_start(), descriptor.get_handler_mask());
            m_root_read ->populate_mismatched(nstart, nend, nmirror, descriptor);
            hand_r->unref();

            auto hand_w = new handler_entry_write_delegate<AccessWidth, -AccessWidth, Endian, WRITE>(this, handler_w);
            descriptor.set_subunit_handler(hand_w);
            hand_w->set_address_info(descriptor.get_handler_start(), descriptor.get_handler_mask());
            m_root_write->populate_mismatched(nstart, nend, nmirror, descriptor);
            hand_w->unref();

            invalidate_caches(read_or_write::READWRITE);
        }


        template<int AccessWidth, typename READ, typename WRITE> std::enable_if_t<(Width < AccessWidth)>
        install_readwrite_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
                                         const READ  &handler_r,
                                         const WRITE &handler_w)
        {
            fatalerror("install_readwrite_handler: cannot install a %d-wide handler in a %d-wide bus", 8 << AccessWidth, 8 << Width);
        }
#endif
    }
}
