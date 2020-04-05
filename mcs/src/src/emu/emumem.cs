// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;
using offs_t = System.UInt32;
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
    //using read16m_delegate = device_delegate<u16 (address_space &, offs_t)>;
    //using read32m_delegate = device_delegate<u32 (address_space &, offs_t)>;
    //using read64m_delegate = device_delegate<u64 (address_space &, offs_t)>;

    //using read8s_delegate  = device_delegate<u8  (offs_t, u8 )>;
    //using read16s_delegate = device_delegate<u16 (offs_t, u16)>;
    //using read32s_delegate = device_delegate<u32 (offs_t, u32)>;
    //using read64s_delegate = device_delegate<u64 (offs_t, u64)>;

    //using read8sm_delegate  = device_delegate<u8  (offs_t)>;
    public delegate u8 read8sm_delegate(offs_t offset);
    public delegate u16 read16sm_delegate(offs_t offset);
    public delegate u32 read32sm_delegate(offs_t offset);
    public delegate u64 read64sm_delegate(offs_t offset);

    //using read16sm_delegate = device_delegate<u16 (offs_t)>;
    //using read32sm_delegate = device_delegate<u32 (offs_t)>;
    //using read64sm_delegate = device_delegate<u64 (offs_t)>;

    //using read8mo_delegate  = device_delegate<u8  (address_space &)>;
    //using read16mo_delegate = device_delegate<u16 (address_space &)>;
    //using read32mo_delegate = device_delegate<u32 (address_space &)>;
    //using read64mo_delegate = device_delegate<u64 (address_space &)>;

    //using read8smo_delegate  = device_delegate<u8  ()>;
    public delegate u8 read8smo_delegate();
    public delegate u16 read16smo_delegate();
    public delegate u32 read32smo_delegate();
    public delegate u64 read64smo_delegate();


    // ======================> write_delegate
    // declare delegates for each width
    //typedef device_delegate<void (address_space &, offs_t, u8,  u8 )> write8_delegate;
    public delegate void write8_delegate(address_space space, offs_t offset, u8 data, u8 mem_mask);
    public delegate void write16_delegate(address_space space, offs_t offset, u16 data, u16 mem_mask);
    public delegate void write32_delegate(address_space space, offs_t offset, u32 data, u32 mem_mask);
    public delegate void write64_delegate(address_space space, offs_t offset, u64 data, u64 mem_mask);

    //using write8m_delegate  = device_delegate<void (address_space &, offs_t, u8 )>;
    //using write16m_delegate = device_delegate<void (address_space &, offs_t, u16)>;
    //using write32m_delegate = device_delegate<void (address_space &, offs_t, u32)>;
    //using write64m_delegate = device_delegate<void (address_space &, offs_t, u64)>;

    //using write8s_delegate  = device_delegate<void (offs_t, u8,  u8 )>;
    //using write16s_delegate = device_delegate<void (offs_t, u16, u16)>;
    //using write32s_delegate = device_delegate<void (offs_t, u32, u32)>;
    //using write64s_delegate = device_delegate<void (offs_t, u64, u64)>;

    //using write8sm_delegate  = device_delegate<void (offs_t, u8 )>;
    public delegate void write8sm_delegate(offs_t offset, u8 data);
    public delegate void write16sm_delegate(offs_t offset, u16 data);
    public delegate void write32sm_delegate(offs_t offset, u32 data);
    public delegate void write64sm_delegate(offs_t offset, u64 data);

    //using write8mo_delegate  = device_delegate<void (address_space &, u8 )>;
    //using write16mo_delegate = device_delegate<void (address_space &, u16)>;
    //using write32mo_delegate = device_delegate<void (address_space &, u32)>;
    //using write64mo_delegate = device_delegate<void (address_space &, u64)>;

    //using write8smo_delegate  = device_delegate<void (u8 )>;
    public delegate void write8smo_delegate(u8 data);
    public delegate void write16smo_delegate(u16 data);
    public delegate void write32smo_delegate(u32 data);
    public delegate void write64smo_delegate(u64 data);


#if false
    namespace emu { namespace detail {

    template <typename D, typename T, typename Enable = void> struct rw_device_class  { };

    template <typename D, typename T, typename Ret, typename... Params>
    struct rw_device_class<D, Ret (T::*)(Params...), std::enable_if_t<std::is_constructible<D, Ret (T::*)(Params...), const char *, const char *, T *>::value> > { using type = T; };
    template <typename D, typename T, typename Ret, typename... Params>
    struct rw_device_class<D, Ret (T::*)(Params...) const, std::enable_if_t<std::is_constructible<D, Ret (T::*)(Params...) const, const char *, const char *, T *>::value> > { using type = T; };
    template <typename D, typename T, typename Ret, typename... Params>
    struct rw_device_class<D, Ret (*)(T &, Params...), std::enable_if_t<std::is_constructible<D, Ret (*)(T &, Params...), const char *, const char *, T *>::value> > { using type = T; };

    template <typename D, typename T> using rw_device_class_t  = typename rw_device_class <D, T>::type;

    template <typename T>
    inline read8_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read8_delegate, std::remove_reference_t<T> > *obj)
    { return read8_delegate(func, name, tag, obj); }
    template <typename T>
    inline read16_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read16_delegate, std::remove_reference_t<T> > *obj)
    { return read16_delegate(func, name, tag, obj); }
    template <typename T>
    inline read32_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read32_delegate, std::remove_reference_t<T> > *obj)
    { return read32_delegate(func, name, tag, obj); }
    template <typename T>
    inline read64_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read64_delegate, std::remove_reference_t<T> > *obj)
    { return read64_delegate(func, name, tag, obj); }

    template <typename T>
    inline write8_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write8_delegate, std::remove_reference_t<T> > *obj)
    { return write8_delegate(func, name, tag, obj); }
    template <typename T>
    inline write16_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write16_delegate, std::remove_reference_t<T> > *obj)
    { return write16_delegate(func, name, tag, obj); }
    template <typename T>
    inline write32_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write32_delegate, std::remove_reference_t<T> > *obj)
    { return write32_delegate(func, name, tag, obj); }
    template <typename T>
    inline write64_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write64_delegate, std::remove_reference_t<T> > *obj)
    { return write64_delegate(func, name, tag, obj); }


    template <typename T>
    inline read8m_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read8m_delegate, std::remove_reference_t<T> > *obj)
    { return read8m_delegate(func, name, tag, obj); }
    template <typename T>
    inline read16m_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read16m_delegate, std::remove_reference_t<T> > *obj)
    { return read16m_delegate(func, name, tag, obj); }
    template <typename T>
    inline read32m_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read32m_delegate, std::remove_reference_t<T> > *obj)
    { return read32m_delegate(func, name, tag, obj); }
    template <typename T>
    inline read64m_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read64m_delegate, std::remove_reference_t<T> > *obj)
    { return read64m_delegate(func, name, tag, obj); }

    template <typename T>
    inline write8m_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write8m_delegate, std::remove_reference_t<T> > *obj)
    { return write8m_delegate(func, name, tag, obj); }
    template <typename T>
    inline write16m_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write16m_delegate, std::remove_reference_t<T> > *obj)
    { return write16m_delegate(func, name, tag, obj); }
    template <typename T>
    inline write32m_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write32m_delegate, std::remove_reference_t<T> > *obj)
    { return write32m_delegate(func, name, tag, obj); }
    template <typename T>
    inline write64m_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write64m_delegate, std::remove_reference_t<T> > *obj)
    { return write64m_delegate(func, name, tag, obj); }


    template <typename T>
    inline read8s_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read8s_delegate, std::remove_reference_t<T> > *obj)
    { return read8s_delegate(func, name, tag, obj); }
    template <typename T>
    inline read16s_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read16s_delegate, std::remove_reference_t<T> > *obj)
    { return read16s_delegate(func, name, tag, obj); }
    template <typename T>
    inline read32s_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read32s_delegate, std::remove_reference_t<T> > *obj)
    { return read32s_delegate(func, name, tag, obj); }
    template <typename T>
    inline read64s_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read64s_delegate, std::remove_reference_t<T> > *obj)
    { return read64s_delegate(func, name, tag, obj); }

    template <typename T>
    inline write8s_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write8s_delegate, std::remove_reference_t<T> > *obj)
    { return write8s_delegate(func, name, tag, obj); }
    template <typename T>
    inline write16s_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write16s_delegate, std::remove_reference_t<T> > *obj)
    { return write16s_delegate(func, name, tag, obj); }
    template <typename T>
    inline write32s_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write32s_delegate, std::remove_reference_t<T> > *obj)
    { return write32s_delegate(func, name, tag, obj); }
    template <typename T>
    inline write64s_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write64s_delegate, std::remove_reference_t<T> > *obj)
    { return write64s_delegate(func, name, tag, obj); }


    template <typename T>
    inline read8sm_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read8sm_delegate, std::remove_reference_t<T> > *obj)
    { return read8sm_delegate(func, name, tag, obj); }
    template <typename T>
    inline read16sm_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read16sm_delegate, std::remove_reference_t<T> > *obj)
    { return read16sm_delegate(func, name, tag, obj); }
    template <typename T>
    inline read32sm_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read32sm_delegate, std::remove_reference_t<T> > *obj)
    { return read32sm_delegate(func, name, tag, obj); }
    template <typename T>
    inline read64sm_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read64sm_delegate, std::remove_reference_t<T> > *obj)
    { return read64sm_delegate(func, name, tag, obj); }

    template <typename T>
    inline write8sm_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write8sm_delegate, std::remove_reference_t<T> > *obj)
    { return write8sm_delegate(func, name, tag, obj); }
    template <typename T>
    inline write16sm_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write16sm_delegate, std::remove_reference_t<T> > *obj)
    { return write16sm_delegate(func, name, tag, obj); }
    template <typename T>
    inline write32sm_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write32sm_delegate, std::remove_reference_t<T> > *obj)
    { return write32sm_delegate(func, name, tag, obj); }
    template <typename T>
    inline write64sm_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write64sm_delegate, std::remove_reference_t<T> > *obj)
    { return write64sm_delegate(func, name, tag, obj); }


    template <typename T>
    inline read8mo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read8mo_delegate, std::remove_reference_t<T> > *obj)
    { return read8mo_delegate(func, name, tag, obj); }
    template <typename T>
    inline read16mo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read16mo_delegate, std::remove_reference_t<T> > *obj)
    { return read16mo_delegate(func, name, tag, obj); }
    template <typename T>
    inline read32mo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read32mo_delegate, std::remove_reference_t<T> > *obj)
    { return read32mo_delegate(func, name, tag, obj); }
    template <typename T>
    inline read64mo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read64mo_delegate, std::remove_reference_t<T> > *obj)
    { return read64mo_delegate(func, name, tag, obj); }

    template <typename T>
    inline write8mo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write8mo_delegate, std::remove_reference_t<T> > *obj)
    { return write8mo_delegate(func, name, tag, obj); }
    template <typename T>
    inline write16mo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write16mo_delegate, std::remove_reference_t<T> > *obj)
    { return write16mo_delegate(func, name, tag, obj); }
    template <typename T>
    inline write32mo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write32mo_delegate, std::remove_reference_t<T> > *obj)
    { return write32mo_delegate(func, name, tag, obj); }
    template <typename T>
    inline write64mo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write64mo_delegate, std::remove_reference_t<T> > *obj)
    { return write64mo_delegate(func, name, tag, obj); }


    template <typename T>
    inline read8smo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read8smo_delegate, std::remove_reference_t<T> > *obj)
    { return read8smo_delegate(func, name, tag, obj); }
    template <typename T>
    inline read16smo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read16smo_delegate, std::remove_reference_t<T> > *obj)
    { return read16smo_delegate(func, name, tag, obj); }
    template <typename T>
    inline read32smo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read32smo_delegate, std::remove_reference_t<T> > *obj)
    { return read32smo_delegate(func, name, tag, obj); }
    template <typename T>
    inline read64smo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<read64smo_delegate, std::remove_reference_t<T> > *obj)
    { return read64smo_delegate(func, name, tag, obj); }

    template <typename T>
    inline write8smo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write8smo_delegate, std::remove_reference_t<T> > *obj)
    { return write8smo_delegate(func, name, tag, obj); }
    template <typename T>
    inline write16smo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write16smo_delegate, std::remove_reference_t<T> > *obj)
    { return write16smo_delegate(func, name, tag, obj); }
    template <typename T>
    inline write32smo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write32smo_delegate, std::remove_reference_t<T> > *obj)
    { return write32smo_delegate(func, name, tag, obj); }
    template <typename T>
    inline write64smo_delegate make_delegate(T &&func, const char *name, const char *tag, rw_device_class_t<write64smo_delegate, std::remove_reference_t<T> > *obj)
    { return write64smo_delegate(func, name, tag, obj); }



    template <typename L>
    inline std::enable_if_t<std::is_constructible<read8_delegate, L, const char *>::value, read8_delegate> make_lr8_delegate(L l, const char *name)
    { return read8_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read8m_delegate, L, const char *>::value, read8m_delegate> make_lr8_delegate(L l, const char *name)
    { return read8m_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read8s_delegate, L, const char *>::value, read8s_delegate> make_lr8_delegate(L l, const char *name)
    { return read8s_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read8sm_delegate, L, const char *>::value, read8sm_delegate> make_lr8_delegate(L l, const char *name)
    { return read8sm_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read8mo_delegate, L, const char *>::value, read8mo_delegate> make_lr8_delegate(L l, const char *name)
    { return read8mo_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read8smo_delegate, L, const char *>::value, read8smo_delegate> make_lr8_delegate(L l, const char *name)
    { return read8smo_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read16_delegate, L, const char *>::value, read16_delegate> make_lr16_delegate(L l, const char *name)
    { return read16_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read16m_delegate, L, const char *>::value, read16m_delegate> make_lr16_delegate(L l, const char *name)
    { return read16m_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read16s_delegate, L, const char *>::value, read16s_delegate> make_lr16_delegate(L l, const char *name)
    { return read16s_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read16sm_delegate, L, const char *>::value, read16sm_delegate> make_lr16_delegate(L l, const char *name)
    { return read16sm_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read16mo_delegate, L, const char *>::value, read16mo_delegate> make_lr16_delegate(L l, const char *name)
    { return read16mo_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read16smo_delegate, L, const char *>::value, read16smo_delegate> make_lr16_delegate(L l, const char *name)
    { return read16smo_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read32_delegate, L, const char *>::value, read32_delegate> make_lr32_delegate(L l, const char *name)
    { return read32_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read32m_delegate, L, const char *>::value, read32m_delegate> make_lr32_delegate(L l, const char *name)
    { return read32m_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read32s_delegate, L, const char *>::value, read32s_delegate> make_lr32_delegate(L l, const char *name)
    { return read32s_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read32sm_delegate, L, const char *>::value, read32sm_delegate> make_lr32_delegate(L l, const char *name)
    { return read32sm_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read32mo_delegate, L, const char *>::value, read32mo_delegate> make_lr32_delegate(L l, const char *name)
    { return read32mo_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read32smo_delegate, L, const char *>::value, read32smo_delegate> make_lr32_delegate(L l, const char *name)
    { return read32smo_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read64_delegate, L, const char *>::value, read64_delegate> make_lr64_delegate(L l, const char *name)
    { return read64_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read64m_delegate, L, const char *>::value, read64m_delegate> make_lr64_delegate(L l, const char *name)
    { return read64m_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read64s_delegate, L, const char *>::value, read64s_delegate> make_lr64_delegate(L l, const char *name)
    { return read64s_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read64sm_delegate, L, const char *>::value, read64sm_delegate> make_lr64_delegate(L l, const char *name)
    { return read64sm_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read64mo_delegate, L, const char *>::value, read64mo_delegate> make_lr64_delegate(L l, const char *name)
    { return read64mo_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<read64smo_delegate, L, const char *>::value, read64smo_delegate> make_lr64_delegate(L l, const char *name)
    { return read64smo_delegate(l, name); }


    template <typename L>
    inline std::enable_if_t<std::is_constructible<write8_delegate, L, const char *>::value, write8_delegate> make_lw8_delegate(L l, const char *name)
    { return write8_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write8m_delegate, L, const char *>::value, write8m_delegate> make_lw8_delegate(L l, const char *name)
    { return write8m_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write8s_delegate, L, const char *>::value, write8s_delegate> make_lw8_delegate(L l, const char *name)
    { return write8s_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write8sm_delegate, L, const char *>::value, write8sm_delegate> make_lw8_delegate(L l, const char *name)
    { return write8sm_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write8mo_delegate, L, const char *>::value, write8mo_delegate> make_lw8_delegate(L l, const char *name)
    { return write8mo_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write8smo_delegate, L, const char *>::value, write8smo_delegate> make_lw8_delegate(L l, const char *name)
    { return write8smo_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write16_delegate, L, const char *>::value, write16_delegate> make_lw16_delegate(L l, const char *name)
    { return write16_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write16m_delegate, L, const char *>::value, write16m_delegate> make_lw16_delegate(L l, const char *name)
    { return write16m_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write16s_delegate, L, const char *>::value, write16s_delegate> make_lw16_delegate(L l, const char *name)
    { return write16s_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write16sm_delegate, L, const char *>::value, write16sm_delegate> make_lw16_delegate(L l, const char *name)
    { return write16sm_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write16mo_delegate, L, const char *>::value, write16mo_delegate> make_lw16_delegate(L l, const char *name)
    { return write16mo_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write16smo_delegate, L, const char *>::value, write16smo_delegate> make_lw16_delegate(L l, const char *name)
    { return write16smo_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write32_delegate, L, const char *>::value, write32_delegate> make_lw32_delegate(L l, const char *name)
    { return write32_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write32m_delegate, L, const char *>::value, write32m_delegate> make_lw32_delegate(L l, const char *name)
    { return write32m_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write32s_delegate, L, const char *>::value, write32s_delegate> make_lw32_delegate(L l, const char *name)
    { return write32s_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write32sm_delegate, L, const char *>::value, write32sm_delegate> make_lw32_delegate(L l, const char *name)
    { return write32sm_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write32mo_delegate, L, const char *>::value, write32mo_delegate> make_lw32_delegate(L l, const char *name)
    { return write32mo_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write32smo_delegate, L, const char *>::value, write32smo_delegate> make_lw32_delegate(L l, const char *name)
    { return write32smo_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write64_delegate, L, const char *>::value, write64_delegate> make_lw64_delegate(L l, const char *name)
    { return write64_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write64m_delegate, L, const char *>::value, write64m_delegate> make_lw64_delegate(L l, const char *name)
    { return write64m_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write64s_delegate, L, const char *>::value, write64s_delegate> make_lw64_delegate(L l, const char *name)
    { return write64s_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write64sm_delegate, L, const char *>::value, write64sm_delegate> make_lw64_delegate(L l, const char *name)
    { return write64sm_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write64mo_delegate, L, const char *>::value, write64mo_delegate> make_lw64_delegate(L l, const char *name)
    { return write64mo_delegate(l, name); }

    template <typename L>
    inline std::enable_if_t<std::is_constructible<write64smo_delegate, L, const char *>::value, write64smo_delegate> make_lw64_delegate(L l, const char *name)
    { return write64smo_delegate(l, name); }
#endif


    // =====================-> Width -> types

    //template<int Width> struct handler_entry_size {};
    //template<> struct handler_entry_size<0> { using uX = u8;  };
    //template<> struct handler_entry_size<1> { using uX = u16; };
    //template<> struct handler_entry_size<2> { using uX = u32; };
    //template<> struct handler_entry_size<3> { using uX = u64; };


    // =====================-> Address segmentation for the search tree

    //constexpr int handler_entry_dispatch_lowbits(int highbits, int width, int ashift)
    //{
    //    return (highbits > 48) ? 48 :
    //        (highbits > 32) ? 32 :
    //        (highbits > 14) ? 14 :
    //        width + ashift;
    //}


    // ======================> memopry_units_descritor forwards declaration

    //template<int Width, int AddrShift, int Endian> class memory_units_descriptor;


    // read or write constants
    public enum read_or_write
    {
        READ = 1,
        WRITE = 2,
        READWRITE = 3
    }


    public static class emumem_global
    {
        const bool MEM_DUMP = false;
        const bool VERBOSE = false;
        public const bool VALIDATE_REFCOUNTS = true;


        // address space names for common use
        public const int AS_PROGRAM = 0; // program address space
        public const int AS_DATA    = 1; // data address space
        public const int AS_IO      = 2; // I/O address space
        public const int AS_OPCODES = 3; // (decrypted) opcodes, when separate from data accesses


        // other address map constants
        public const int MEMORY_BLOCK_CHUNK = 65536;                   // minimum chunk size of allocated memory blocks


        // helpers for checking address alignment
        public static bool WORD_ALIGNED(UInt32 a) { return (a & 1) == 0; }
        public static bool DWORD_ALIGNED(UInt32 a) { return (a & 3) == 0; }
        public static bool QWORD_ALIGNED(UInt32 a) { return (a & 7) == 0; }


        // helper macro for merging data with the memory mask
        //#define COMBINE_DATA(varptr)            (*(varptr) = (*(varptr) & ~mem_mask) | (data & mem_mask))
        public static void COMBINE_DATA(ref UInt32 varptr, UInt32 data, UInt32 mem_mask) { varptr = (varptr & ~mem_mask) | (data & mem_mask); }


        public static void VPRINTF(string format, params object [] args) { if (VERBOSE) global_object.osd_printf_info(format, args); }


        // =====================-> Width -> types
        //template<int Width> struct handler_entry_size {};
        //template<> struct handler_entry_size<0> { typedef u8  uX; typedef read8_delegate  READ; typedef write8_delegate  WRITE; };
        //template<> struct handler_entry_size<1> { typedef u16 uX; typedef read16_delegate READ; typedef write16_delegate WRITE; };
        //template<> struct handler_entry_size<2> { typedef u32 uX; typedef read32_delegate READ; typedef write32_delegate WRITE; };
        //template<> struct handler_entry_size<3> { typedef u64 uX; typedef read64_delegate READ; typedef write64_delegate WRITE; };


        // ======================> address offset -> byte offset

        public static offs_t memory_offset_to_byte(offs_t offset, int AddrShift) { return AddrShift < 0 ? offset << global_object.iabs(AddrShift) : offset >> global_object.iabs(AddrShift); }


        // ======================> generic read/write decomposition routines

        static Type memory_read_generic_FindType(int size)
        {
            switch (size)
            {
                case 0: return typeof(u8);
                case 1: return typeof(u16);
                case 2: return typeof(u32);
                case 3: return typeof(u64);
                default: throw new emu_unimplemented();
            }
        }

        public delegate u8 memory_read_generic8_rop(offs_t offset, u8 mask);

        // generic direct read
        //template<int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, typename T>
        //typename emu::detail::handler_entry_size<TargetWidth>::uX  memory_read_generic(T rop, offs_t address, typename emu::detail::handler_entry_size<TargetWidth>::uX mask)
        public static u8 memory_read_generic8(int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, memory_read_generic8_rop rop, offs_t address, u8 mask)
        {
            Type TargetType = memory_read_generic_FindType(TargetWidth);  //using TargetType = typename emu::detail::handler_entry_size<TargetWidth>::uX;
            Type NativeType = memory_read_generic_FindType(Width);  //using NativeType = typename emu::detail::handler_entry_size<Width>::uX;

            u32 TARGET_BYTES = 1U << TargetWidth;
            u32 TARGET_BITS = 8 * TARGET_BYTES;
            u32 NATIVE_BYTES = 1U << Width;
            u32 NATIVE_BITS = 8 * NATIVE_BYTES;
            u32 NATIVE_STEP = AddrShift >= 0 ? NATIVE_BYTES << global_object.iabs(AddrShift) : NATIVE_BYTES >> global_object.iabs(AddrShift);
            u32 NATIVE_MASK = Width + AddrShift >= 0 ? (1U << (Width + AddrShift)) - 1 : 0;

            // equal to native size and aligned; simple pass-through to the native reader
            if (NATIVE_BYTES == TARGET_BYTES && (Aligned || (address & NATIVE_MASK) == 0))
                return rop(address & ~NATIVE_MASK, mask);

            // if native size is larger, see if we can do a single masked read (guaranteed if we're aligned)
            if (NATIVE_BYTES > TARGET_BYTES)
            {
                u32 offsbits2 = 8 * (memory_offset_to_byte(address, AddrShift) & (NATIVE_BYTES - (Aligned ? TARGET_BYTES : 1)));  // renamed due to dup var name below
                if (Aligned || (offsbits2 + TARGET_BITS <= NATIVE_BITS))
                {
                    if (Endian != (int)endianness_t.ENDIANNESS_LITTLE) offsbits2 = NATIVE_BITS - TARGET_BITS - offsbits2;
                    return (u8)(rop(address & ~NATIVE_MASK, (u8 /*NativeType*/)(mask << (int)offsbits2)) >> (int)offsbits2);
                }
            }

            // determine our alignment against the native boundaries, and mask the address
            u32 offsbits = 8 * (memory_offset_to_byte(address, AddrShift) & (NATIVE_BYTES - 1));
            address &= ~NATIVE_MASK;

            // if we're here, and native size is larger or equal to the target, we need exactly 2 reads
            if (NATIVE_BYTES >= TARGET_BYTES)
            {
                // little-endian case
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                    // read lower bits from lower address
                    u8 /*TargetType*/ result = 0;
                    u8 /*NativeType*/ curmask = (u8 /*NativeType*/)(mask << (int)offsbits);
                    if (curmask != 0) result = (u8)(rop(address, curmask) >> (int)offsbits);

                    // read upper bits from upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = (u8)(mask >> (int)offsbits);
                    if (curmask != 0) result |= (u8)(rop(address + NATIVE_STEP, curmask) << (int)offsbits);
                    return result;
                }

                // big-endian case
                else
                {
                    // left-justify the mask to the target type
                    u32 LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT = ((NATIVE_BITS >= TARGET_BITS) ? (NATIVE_BITS - TARGET_BITS) : 0);
                    u8 /*NativeType*/ result = 0;
                    u8 /*NativeType*/ ljmask = (u8 /*NativeType*/)(mask << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                    u8 /*NativeType*/ curmask = (u8)(ljmask >> (int)offsbits);

                    // read upper bits from lower address
                    if (curmask != 0) result = (u8)(rop(address, curmask) << (int)offsbits);
                    offsbits = NATIVE_BITS - offsbits;

                    // read lower bits from upper address
                    curmask = (u8)(ljmask << (int)offsbits);
                    if (curmask != 0) result |= (u8)(rop(address + NATIVE_STEP, curmask) >> (int)offsbits);

                    // return the un-justified result
                    return (u8)(result >> (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                }
            }

            // if we're here, then we have 2 or more reads needed to get our final result
            else
            {
                // compute the maximum number of loops; we do it this way so that there are
                // a fixed number of loops for the compiler to unroll if it desires
                u32 MAX_SPLITS_MINUS_ONE = TARGET_BYTES / NATIVE_BYTES - 1;
                u8 /*TargetType*/ result = 0;

                // little-endian case
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                        // read lowest bits from first address
                    u8 /*NativeType*/ curmask = (u8)(mask << (int)offsbits);
                    if (curmask != 0) result = (u8)(rop(address, curmask) >> (int)offsbits);

                    // read middle bits from subsequent addresses
                    offsbits = NATIVE_BITS - offsbits;
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        address += NATIVE_STEP;
                        curmask = (u8)(mask >> (int)offsbits);
                        if (curmask != 0) result |= (u8 /*TargetType*/)(rop(address, curmask) << (int)offsbits);
                        offsbits += NATIVE_BITS;
                    }

                    // if we're not aligned and we still have bits left, read uppermost bits from last address
                    if (!Aligned && offsbits < TARGET_BITS)
                    {
                        curmask = (u8)(mask >> (int)offsbits);
                        if (curmask != 0) result |= (u8 /*TargetType*/)(rop(address + NATIVE_STEP, curmask) << (int)offsbits);
                    }
                }

                // big-endian case
                else
                {
                    // read highest bits from first address
                    offsbits = TARGET_BITS - (NATIVE_BITS - offsbits);
                    u8 /*NativeType*/ curmask = (u8)(mask >> (int)offsbits);
                    if (curmask != 0) result = (u8 /*TargetType*/)(rop(address, curmask) << (int)offsbits);

                    // read middle bits from subsequent addresses
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        offsbits -= NATIVE_BITS;
                        address += NATIVE_STEP;
                        curmask = (u8)(mask >> (int)offsbits);
                        if (curmask != 0) result |= (u8 /*TargetType*/)(rop(address, curmask) << (int)offsbits);
                    }

                    // if we're not aligned and we still have bits left, read lowermost bits from the last address
                    if (!Aligned && offsbits != 0)
                    {
                        offsbits = NATIVE_BITS - offsbits;
                        curmask = (u8)(mask << (int)offsbits);
                        if (curmask != 0) result |= (u8)(rop(address + NATIVE_STEP, curmask) >> (int)offsbits);
                    }
                }
                return result;
            }
        }


        public delegate u16 memory_read_generic16_rop(offs_t offset, u16 mask);

        // generic direct read
        //template<int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, typename T>
        //typename handler_entry_size<TargetWidth>::uX  memory_read_generic(T rop, offs_t address, typename handler_entry_size<TargetWidth>::uX mask)
        public static u16 memory_read_generic16(int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, memory_read_generic16_rop rop, offs_t address, u16 mask)
        {
            Type TargetType = memory_read_generic_FindType(TargetWidth);  //using TargetType = typename handler_entry_size<TargetWidth>::uX;
            Type NativeType = memory_read_generic_FindType(Width);  //using NativeType = typename handler_entry_size<Width>::uX;

            u32 TARGET_BYTES = 1U << TargetWidth;
            u32 TARGET_BITS = 8 * TARGET_BYTES;
            u32 NATIVE_BYTES = 1U << Width;
            u32 NATIVE_BITS = 8 * NATIVE_BYTES;
            u32 NATIVE_STEP = AddrShift >= 0 ? NATIVE_BYTES << global_object.iabs(AddrShift) : NATIVE_BYTES >> global_object.iabs(AddrShift);
            u32 NATIVE_MASK = Width + AddrShift >= 0 ? (1U << (Width + AddrShift)) - 1 : 0;

            // equal to native size and aligned; simple pass-through to the native reader
            if (NATIVE_BYTES == TARGET_BYTES && (Aligned || (address & NATIVE_MASK) == 0))
                return rop(address & ~NATIVE_MASK, mask);

            // if native size is larger, see if we can do a single masked read (guaranteed if we're aligned)
            if (NATIVE_BYTES > TARGET_BYTES)
            {
                u32 offsbits2 = 8 * (memory_offset_to_byte(address, AddrShift) & (NATIVE_BYTES - (Aligned ? TARGET_BYTES : 1)));  // renamed due to dup var name below
                if (Aligned || (offsbits2 + TARGET_BITS <= NATIVE_BITS))
                {
                    if (Endian != (int)endianness_t.ENDIANNESS_LITTLE) offsbits2 = NATIVE_BITS - TARGET_BITS - offsbits2;
                    return (u16)(rop(address & ~NATIVE_MASK, (u16 /*NativeType*/)(mask << (int)offsbits2)) >> (int)offsbits2);
                }
            }

            // determine our alignment against the native boundaries, and mask the address
            u32 offsbits = 8 * (memory_offset_to_byte(address, AddrShift) & (NATIVE_BYTES - 1));
            address &= ~NATIVE_MASK;

            // if we're here, and native size is larger or equal to the target, we need exactly 2 reads
            if (NATIVE_BYTES >= TARGET_BYTES)
            {
                // little-endian case
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                    // read lower bits from lower address
                    u16 /*TargetType*/ result = 0;
                    u16 /*NativeType*/ curmask = (u16 /*NativeType*/)(mask << (int)offsbits);
                    if (curmask != 0) result = (u16)(rop(address, curmask) >> (int)offsbits);

                    // read upper bits from upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = (u16)(mask >> (int)offsbits);
                    if (curmask != 0) result |= (u16)(rop(address + NATIVE_STEP, curmask) << (int)offsbits);
                    return result;
                }

                // big-endian case
                else
                {
                    // left-justify the mask to the target type
                    u32 LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT = ((NATIVE_BITS >= TARGET_BITS) ? (NATIVE_BITS - TARGET_BITS) : 0);
                    u16 /*NativeType*/ result = 0;
                    u16 /*NativeType*/ ljmask = (u16 /*NativeType*/)(mask << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                    u16 /*NativeType*/ curmask = (u16)(ljmask >> (int)offsbits);

                    // read upper bits from lower address
                    if (curmask != 0) result = (u16)(rop(address, curmask) << (int)offsbits);
                    offsbits = NATIVE_BITS - offsbits;

                    // read lower bits from upper address
                    curmask = (u16)(ljmask << (int)offsbits);
                    if (curmask != 0) result |= (u16)(rop(address + NATIVE_STEP, curmask) >> (int)offsbits);

                    // return the un-justified result
                    return (u16)(result >> (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                }
            }

            // if we're here, then we have 2 or more reads needed to get our final result
            else
            {
                // compute the maximum number of loops; we do it this way so that there are
                // a fixed number of loops for the compiler to unroll if it desires
                u32 MAX_SPLITS_MINUS_ONE = TARGET_BYTES / NATIVE_BYTES - 1;
                u16 /*TargetType*/ result = 0;

                // little-endian case
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                        // read lowest bits from first address
                    u16 /*NativeType*/ curmask = (u16)(mask << (int)offsbits);
                    if (curmask != 0) result = (u16)(rop(address, curmask) >> (int)offsbits);

                    // read middle bits from subsequent addresses
                    offsbits = NATIVE_BITS - offsbits;
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        address += NATIVE_STEP;
                        curmask = (u16)(mask >> (int)offsbits);
                        if (curmask != 0) result |= (u16 /*TargetType*/)(rop(address, curmask) << (int)offsbits);
                        offsbits += NATIVE_BITS;
                    }

                    // if we're not aligned and we still have bits left, read uppermost bits from last address
                    if (!Aligned && offsbits < TARGET_BITS)
                    {
                        curmask = (u16)(mask >> (int)offsbits);
                        if (curmask != 0) result |= (u16 /*TargetType*/)(rop(address + NATIVE_STEP, curmask) << (int)offsbits);
                    }
                }

                // big-endian case
                else
                {
                    // read highest bits from first address
                    offsbits = TARGET_BITS - (NATIVE_BITS - offsbits);
                    u16 /*NativeType*/ curmask = (u16)(mask >> (int)offsbits);
                    if (curmask != 0) result = (u16 /*TargetType*/)(rop(address, curmask) << (int)offsbits);

                    // read middle bits from subsequent addresses
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        offsbits -= NATIVE_BITS;
                        address += NATIVE_STEP;
                        curmask = (u16)(mask >> (int)offsbits);
                        if (curmask != 0) result |= (u16 /*TargetType*/)(rop(address, curmask) << (int)offsbits);
                    }

                    // if we're not aligned and we still have bits left, read lowermost bits from the last address
                    if (!Aligned && offsbits != 0)
                    {
                        offsbits = NATIVE_BITS - offsbits;
                        curmask = (u16)(mask << (int)offsbits);
                        if (curmask != 0) result |= (u16)(rop(address + NATIVE_STEP, curmask) >> (int)offsbits);
                    }
                }
                return result;
            }
        }


        public delegate u32 memory_read_generic32_rop(offs_t offset, u32 mask);

        // generic direct read
        //template<int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, typename T>
        //typename handler_entry_size<TargetWidth>::uX  memory_read_generic(T rop, offs_t address, typename handler_entry_size<TargetWidth>::uX mask)
        public static u32 memory_read_generic32(int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, memory_read_generic32_rop rop, offs_t address, u32 mask)
        {
            Type TargetType = memory_read_generic_FindType(TargetWidth);  //using TargetType = typename handler_entry_size<TargetWidth>::uX;
            Type NativeType = memory_read_generic_FindType(Width);  //using NativeType = typename handler_entry_size<Width>::uX;

            u32 TARGET_BYTES = 1U << TargetWidth;
            u32 TARGET_BITS = 8 * TARGET_BYTES;
            u32 NATIVE_BYTES = 1U << Width;
            u32 NATIVE_BITS = 8 * NATIVE_BYTES;
            u32 NATIVE_STEP = AddrShift >= 0 ? NATIVE_BYTES << global_object.iabs(AddrShift) : NATIVE_BYTES >> global_object.iabs(AddrShift);
            u32 NATIVE_MASK = Width + AddrShift >= 0 ? (1U << (Width + AddrShift)) - 1 : 0;

            // equal to native size and aligned; simple pass-through to the native reader
            if (NATIVE_BYTES == TARGET_BYTES && (Aligned || (address & NATIVE_MASK) == 0))
                return rop(address & ~NATIVE_MASK, mask);

            // if native size is larger, see if we can do a single masked read (guaranteed if we're aligned)
            if (NATIVE_BYTES > TARGET_BYTES)
            {
                u32 offsbits2 = 8 * (memory_offset_to_byte(address, AddrShift) & (NATIVE_BYTES - (Aligned ? TARGET_BYTES : 1)));  // renamed due to dup var name below
                if (Aligned || (offsbits2 + TARGET_BITS <= NATIVE_BITS))
                {
                    if (Endian != (int)endianness_t.ENDIANNESS_LITTLE) offsbits2 = NATIVE_BITS - TARGET_BITS - offsbits2;
                    return (u32)(rop(address & ~NATIVE_MASK, (u32 /*NativeType*/)(mask << (int)offsbits2)) >> (int)offsbits2);
                }
            }

            // determine our alignment against the native boundaries, and mask the address
            u32 offsbits = 8 * (memory_offset_to_byte(address, AddrShift) & (NATIVE_BYTES - 1));
            address &= ~NATIVE_MASK;

            // if we're here, and native size is larger or equal to the target, we need exactly 2 reads
            if (NATIVE_BYTES >= TARGET_BYTES)
            {
                // little-endian case
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                    // read lower bits from lower address
                    u32 /*TargetType*/ result = 0;
                    u32 /*NativeType*/ curmask = (u32 /*NativeType*/)(mask << (int)offsbits);
                    if (curmask != 0) result = (u32)(rop(address, curmask) >> (int)offsbits);

                    // read upper bits from upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = (u32)(mask >> (int)offsbits);
                    if (curmask != 0) result |= (u32)(rop(address + NATIVE_STEP, curmask) << (int)offsbits);
                    return result;
                }

                // big-endian case
                else
                {
                    // left-justify the mask to the target type
                    u32 LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT = ((NATIVE_BITS >= TARGET_BITS) ? (NATIVE_BITS - TARGET_BITS) : 0);
                    u32 /*NativeType*/ result = 0;
                    u32 /*NativeType*/ ljmask = (u32 /*NativeType*/)(mask << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                    u32 /*NativeType*/ curmask = (u32)(ljmask >> (int)offsbits);

                    // read upper bits from lower address
                    if (curmask != 0) result = (u32)(rop(address, curmask) << (int)offsbits);
                    offsbits = NATIVE_BITS - offsbits;

                    // read lower bits from upper address
                    curmask = (u32)(ljmask << (int)offsbits);
                    if (curmask != 0) result |= (u32)(rop(address + NATIVE_STEP, curmask) >> (int)offsbits);

                    // return the un-justified result
                    return (u32)(result >> (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                }
            }

            // if we're here, then we have 2 or more reads needed to get our final result
            else
            {
                // compute the maximum number of loops; we do it this way so that there are
                // a fixed number of loops for the compiler to unroll if it desires
                u32 MAX_SPLITS_MINUS_ONE = TARGET_BYTES / NATIVE_BYTES - 1;
                u32 /*TargetType*/ result = 0;

                // little-endian case
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                        // read lowest bits from first address
                    u32 /*NativeType*/ curmask = (u32)(mask << (int)offsbits);
                    if (curmask != 0) result = (u32)(rop(address, curmask) >> (int)offsbits);

                    // read middle bits from subsequent addresses
                    offsbits = NATIVE_BITS - offsbits;
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        address += NATIVE_STEP;
                        curmask = (u32)(mask >> (int)offsbits);
                        if (curmask != 0) result |= (u32 /*TargetType*/)(rop(address, curmask) << (int)offsbits);
                        offsbits += NATIVE_BITS;
                    }

                    // if we're not aligned and we still have bits left, read uppermost bits from last address
                    if (!Aligned && offsbits < TARGET_BITS)
                    {
                        curmask = (u32)(mask >> (int)offsbits);
                        if (curmask != 0) result |= (u32 /*TargetType*/)(rop(address + NATIVE_STEP, curmask) << (int)offsbits);
                    }
                }

                // big-endian case
                else
                {
                    // read highest bits from first address
                    offsbits = TARGET_BITS - (NATIVE_BITS - offsbits);
                    u32 /*NativeType*/ curmask = (u32)(mask >> (int)offsbits);
                    if (curmask != 0) result = (u32 /*TargetType*/)(rop(address, curmask) << (int)offsbits);

                    // read middle bits from subsequent addresses
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        offsbits -= NATIVE_BITS;
                        address += NATIVE_STEP;
                        curmask = (u32)(mask >> (int)offsbits);
                        if (curmask != 0) result |= (u32 /*TargetType*/)(rop(address, curmask) << (int)offsbits);
                    }

                    // if we're not aligned and we still have bits left, read lowermost bits from the last address
                    if (!Aligned && offsbits != 0)
                    {
                        offsbits = NATIVE_BITS - offsbits;
                        curmask = (u32)(mask << (int)offsbits);
                        if (curmask != 0) result |= (u32)(rop(address + NATIVE_STEP, curmask) >> (int)offsbits);
                    }
                }
                return result;
            }
        }


        public delegate u64 memory_read_generic64_rop(offs_t offset, u64 mask);

        // generic direct read
        //template<int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, typename T>
        //typename handler_entry_size<TargetWidth>::uX  memory_read_generic(T rop, offs_t address, typename handler_entry_size<TargetWidth>::uX mask)
        public static u64 memory_read_generic64(int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, memory_read_generic64_rop rop, offs_t address, u64 mask)
        {
            Type TargetType = memory_read_generic_FindType(TargetWidth);  //using TargetType = typename handler_entry_size<TargetWidth>::uX;
            Type NativeType = memory_read_generic_FindType(Width);  //using NativeType = typename handler_entry_size<Width>::uX;

            u32 TARGET_BYTES = 1U << TargetWidth;
            u32 TARGET_BITS = 8 * TARGET_BYTES;
            u32 NATIVE_BYTES = 1U << Width;
            u32 NATIVE_BITS = 8 * NATIVE_BYTES;
            u32 NATIVE_STEP = AddrShift >= 0 ? NATIVE_BYTES << global_object.iabs(AddrShift) : NATIVE_BYTES >> global_object.iabs(AddrShift);
            u32 NATIVE_MASK = Width + AddrShift >= 0 ? (1U << (Width + AddrShift)) - 1 : 0;

            // equal to native size and aligned; simple pass-through to the native reader
            if (NATIVE_BYTES == TARGET_BYTES && (Aligned || (address & NATIVE_MASK) == 0))
                return rop(address & ~NATIVE_MASK, mask);

            // if native size is larger, see if we can do a single masked read (guaranteed if we're aligned)
            if (NATIVE_BYTES > TARGET_BYTES)
            {
                u32 offsbits2 = 8 * (memory_offset_to_byte(address, AddrShift) & (NATIVE_BYTES - (Aligned ? TARGET_BYTES : 1)));  // renamed due to dup var name below
                if (Aligned || (offsbits2 + TARGET_BITS <= NATIVE_BITS))
                {
                    if (Endian != (int)endianness_t.ENDIANNESS_LITTLE) offsbits2 = NATIVE_BITS - TARGET_BITS - offsbits2;
                    return (u64)(rop(address & ~NATIVE_MASK, (u64 /*NativeType*/)(mask << (int)offsbits2)) >> (int)offsbits2);
                }
            }

            // determine our alignment against the native boundaries, and mask the address
            u32 offsbits = 8 * (memory_offset_to_byte(address, AddrShift) & (NATIVE_BYTES - 1));
            address &= ~NATIVE_MASK;

            // if we're here, and native size is larger or equal to the target, we need exactly 2 reads
            if (NATIVE_BYTES >= TARGET_BYTES)
            {
                // little-endian case
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                    // read lower bits from lower address
                    u64 /*TargetType*/ result = 0;
                    u64 /*NativeType*/ curmask = (u64 /*NativeType*/)(mask << (int)offsbits);
                    if (curmask != 0) result = (u64)(rop(address, curmask) >> (int)offsbits);

                    // read upper bits from upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = (u64)(mask >> (int)offsbits);
                    if (curmask != 0) result |= (u64)(rop(address + NATIVE_STEP, curmask) << (int)offsbits);
                    return result;
                }

                // big-endian case
                else
                {
                    // left-justify the mask to the target type
                    u32 LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT = ((NATIVE_BITS >= TARGET_BITS) ? (NATIVE_BITS - TARGET_BITS) : 0);
                    u64 /*NativeType*/ result = 0;
                    u64 /*NativeType*/ ljmask = (u64 /*NativeType*/)(mask << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                    u64 /*NativeType*/ curmask = (u64)(ljmask >> (int)offsbits);

                    // read upper bits from lower address
                    if (curmask != 0) result = (u64)(rop(address, curmask) << (int)offsbits);
                    offsbits = NATIVE_BITS - offsbits;

                    // read lower bits from upper address
                    curmask = (u64)(ljmask << (int)offsbits);
                    if (curmask != 0) result |= (u64)(rop(address + NATIVE_STEP, curmask) >> (int)offsbits);

                    // return the un-justified result
                    return (u64)(result >> (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                }
            }

            // if we're here, then we have 2 or more reads needed to get our final result
            else
            {
                // compute the maximum number of loops; we do it this way so that there are
                // a fixed number of loops for the compiler to unroll if it desires
                u32 MAX_SPLITS_MINUS_ONE = TARGET_BYTES / NATIVE_BYTES - 1;
                u64 /*TargetType*/ result = 0;

                // little-endian case
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                        // read lowest bits from first address
                    u64 /*NativeType*/ curmask = (u64)(mask << (int)offsbits);
                    if (curmask != 0) result = (u64)(rop(address, curmask) >> (int)offsbits);

                    // read middle bits from subsequent addresses
                    offsbits = NATIVE_BITS - offsbits;
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        address += NATIVE_STEP;
                        curmask = (u64)(mask >> (int)offsbits);
                        if (curmask != 0) result |= (u64 /*TargetType*/)(rop(address, curmask) << (int)offsbits);
                        offsbits += NATIVE_BITS;
                    }

                    // if we're not aligned and we still have bits left, read uppermost bits from last address
                    if (!Aligned && offsbits < TARGET_BITS)
                    {
                        curmask = (u64)(mask >> (int)offsbits);
                        if (curmask != 0) result |= (u64 /*TargetType*/)(rop(address + NATIVE_STEP, curmask) << (int)offsbits);
                    }
                }

                // big-endian case
                else
                {
                    // read highest bits from first address
                    offsbits = TARGET_BITS - (NATIVE_BITS - offsbits);
                    u64 /*NativeType*/ curmask = (u64)(mask >> (int)offsbits);
                    if (curmask != 0) result = (u64 /*TargetType*/)(rop(address, curmask) << (int)offsbits);

                    // read middle bits from subsequent addresses
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        offsbits -= NATIVE_BITS;
                        address += NATIVE_STEP;
                        curmask = (u64)(mask >> (int)offsbits);
                        if (curmask != 0) result |= (u64 /*TargetType*/)(rop(address, curmask) << (int)offsbits);
                    }

                    // if we're not aligned and we still have bits left, read lowermost bits from the last address
                    if (!Aligned && offsbits != 0)
                    {
                        offsbits = NATIVE_BITS - offsbits;
                        curmask = (u64)(mask << (int)offsbits);
                        if (curmask != 0) result |= (u64)(rop(address + NATIVE_STEP, curmask) >> (int)offsbits);
                    }
                }
                return result;
            }
        }


        public delegate void memory_write_generic8_wop(offs_t offset, u8 data, u8 mask);

        // generic direct write
        //template<int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, typename T>
        //void memory_write_generic(T wop, offs_t address, typename emu::detail::handler_entry_size<TargetWidth>::uX data, typename emu::detail::handler_entry_size<TargetWidth>::uX mask)
        public static void memory_write_generic8(int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, memory_write_generic8_wop wop, offs_t address, u8 data, u8 mask)
        {
            Type NativeType = memory_read_generic_FindType(Width);  //using NativeType = typename emu::detail::handler_entry_size<Width>::uX;

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
                    if (Endian != (int)endianness_t.ENDIANNESS_LITTLE) offsbits2 = NATIVE_BITS - TARGET_BITS - offsbits2;
                    wop(address & ~NATIVE_MASK, (u8 /*NativeType*/)(data << (int)offsbits2), (u8 /*NativeType*/)(mask << (int)offsbits2));
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
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                    // write lower bits to lower address
                    u8 /*NativeType*/ curmask = (u8 /*NativeType*/)(mask << (int)offsbits);
                    if (curmask != 0) wop(address, (u8 /*NativeType*/)(data << (int)offsbits), curmask);

                    // write upper bits to upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = (u8)(mask >> (int)offsbits);
                    if (curmask != 0) wop(address + NATIVE_STEP, (u8)(data >> (int)offsbits), curmask);
                }

                // big-endian case
                else
                {
                    // left-justify the mask and data to the target type
                    u32 LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT = ((NATIVE_BITS >= TARGET_BITS) ? (NATIVE_BITS - TARGET_BITS) : 0);
                    u8 /*NativeType*/ ljdata = (u8 /*NativeType*/)(data << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                    u8 /*NativeType*/ ljmask = (u8 /*NativeType*/)(mask << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                        // write upper bits to lower address
                    u8 /*NativeType*/ curmask = (u8)(ljmask >> (int)offsbits);
                    if (curmask != 0) wop(address, (u8)(ljdata >> (int)offsbits), curmask);
                        // write lower bits to upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = (u8)(ljmask << (int)offsbits);
                    if (curmask != 0) wop(address + NATIVE_STEP, (u8)(ljdata << (int)offsbits), curmask);
                }
            }

            // if we're here, then we have 2 or more writes needed to get our final result
            else
            {
                // compute the maximum number of loops; we do it this way so that there are
                // a fixed number of loops for the compiler to unroll if it desires
                u32 MAX_SPLITS_MINUS_ONE = TARGET_BYTES / NATIVE_BYTES - 1;

                // little-endian case
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                    // write lowest bits to first address
                    u8 /*NativeType*/ curmask = (u8)(mask << (int)offsbits);
                    if (curmask != 0) wop(address, (u8)(data << (int)offsbits), curmask);

                    // write middle bits to subsequent addresses
                    offsbits = NATIVE_BITS - offsbits;
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        address += NATIVE_STEP;
                        curmask = (u8)(mask >> (int)offsbits);
                        if (curmask != 0) wop(address, (u8)(data >> (int)offsbits), curmask);
                        offsbits += NATIVE_BITS;
                    }

                    // if we're not aligned and we still have bits left, write uppermost bits to last address
                    if (!Aligned && offsbits < TARGET_BITS)
                    {
                        curmask = (u8)(mask >> (int)offsbits);
                        if (curmask != 0) wop(address + NATIVE_STEP, (u8)(data >> (int)offsbits), curmask);
                    }
                }

                // big-endian case
                else
                {
                    // write highest bits to first address
                    offsbits = TARGET_BITS - (NATIVE_BITS - offsbits);
                    u8 /*NativeType*/ curmask = (u8)(mask >> (int)offsbits);
                    if (curmask != 0) wop(address, (u8)(data >> (int)offsbits), curmask);

                    // write middle bits to subsequent addresses
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        offsbits -= NATIVE_BITS;
                        address += NATIVE_STEP;
                        curmask = (u8)(mask >> (int)offsbits);
                        if (curmask != 0) wop(address, (u8)(data >> (int)offsbits), curmask);
                    }

                    // if we're not aligned and we still have bits left, write lowermost bits to the last address
                    if (!Aligned && offsbits != 0)
                    {
                        offsbits = NATIVE_BITS - offsbits;
                        curmask = (u8)(mask << (int)offsbits);
                        if (curmask != 0) wop(address + NATIVE_STEP, (u8)(data << (int)offsbits), curmask);
                    }
                }
            }
        }


        public delegate void memory_write_generic16_wop(offs_t offset, u16 data, u16 mask);

        // generic direct write
        //template<int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, typename T>
        //void memory_write_generic(T wop, offs_t address, typename handler_entry_size<TargetWidth>::uX data, typename handler_entry_size<TargetWidth>::uX mask)
        public static void memory_write_generic16(int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, memory_write_generic16_wop wop, offs_t address, u16 data, u16 mask)
        {
            Type NativeType = memory_read_generic_FindType(Width);  //using NativeType = typename handler_entry_size<Width>::uX;

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
                    if (Endian != (int)endianness_t.ENDIANNESS_LITTLE) offsbits2 = NATIVE_BITS - TARGET_BITS - offsbits2;
                    wop(address & ~NATIVE_MASK, (u16 /*NativeType*/)(data << (int)offsbits2), (u16 /*NativeType*/)(mask << (int)offsbits2));
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
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                    // write lower bits to lower address
                    u16 /*NativeType*/ curmask = (u16 /*NativeType*/)(mask << (int)offsbits);
                    if (curmask != 0) wop(address, (u16 /*NativeType*/)(data << (int)offsbits), curmask);

                    // write upper bits to upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = (u16)(mask >> (int)offsbits);
                    if (curmask != 0) wop(address + NATIVE_STEP, (u16)(data >> (int)offsbits), curmask);
                }

                // big-endian case
                else
                {
                    // left-justify the mask and data to the target type
                    u32 LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT = ((NATIVE_BITS >= TARGET_BITS) ? (NATIVE_BITS - TARGET_BITS) : 0);
                    u16 /*NativeType*/ ljdata = (u16 /*NativeType*/)(data << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                    u16 /*NativeType*/ ljmask = (u16 /*NativeType*/)(mask << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                        // write upper bits to lower address
                    u16 /*NativeType*/ curmask = (u16)(ljmask >> (int)offsbits);
                    if (curmask != 0) wop(address, (u16)(ljdata >> (int)offsbits), curmask);
                        // write lower bits to upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = (u16)(ljmask << (int)offsbits);
                    if (curmask != 0) wop(address + NATIVE_STEP, (u16)(ljdata << (int)offsbits), curmask);
                }
            }

            // if we're here, then we have 2 or more writes needed to get our final result
            else
            {
                // compute the maximum number of loops; we do it this way so that there are
                // a fixed number of loops for the compiler to unroll if it desires
                u32 MAX_SPLITS_MINUS_ONE = TARGET_BYTES / NATIVE_BYTES - 1;

                // little-endian case
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                    // write lowest bits to first address
                    u16 /*NativeType*/ curmask = (u16)(mask << (int)offsbits);
                    if (curmask != 0) wop(address, (u16)(data << (int)offsbits), curmask);

                    // write middle bits to subsequent addresses
                    offsbits = NATIVE_BITS - offsbits;
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        address += NATIVE_STEP;
                        curmask = (u16)(mask >> (int)offsbits);
                        if (curmask != 0) wop(address, (u16)(data >> (int)offsbits), curmask);
                        offsbits += NATIVE_BITS;
                    }

                    // if we're not aligned and we still have bits left, write uppermost bits to last address
                    if (!Aligned && offsbits < TARGET_BITS)
                    {
                        curmask = (u16)(mask >> (int)offsbits);
                        if (curmask != 0) wop(address + NATIVE_STEP, (u16)(data >> (int)offsbits), curmask);
                    }
                }

                // big-endian case
                else
                {
                    // write highest bits to first address
                    offsbits = TARGET_BITS - (NATIVE_BITS - offsbits);
                    u16 /*NativeType*/ curmask = (u16)(mask >> (int)offsbits);
                    if (curmask != 0) wop(address, (u16)(data >> (int)offsbits), curmask);

                    // write middle bits to subsequent addresses
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        offsbits -= NATIVE_BITS;
                        address += NATIVE_STEP;
                        curmask = (u16)(mask >> (int)offsbits);
                        if (curmask != 0) wop(address, (u16)(data >> (int)offsbits), curmask);
                    }

                    // if we're not aligned and we still have bits left, write lowermost bits to the last address
                    if (!Aligned && offsbits != 0)
                    {
                        offsbits = NATIVE_BITS - offsbits;
                        curmask = (u16)(mask << (int)offsbits);
                        if (curmask != 0) wop(address + NATIVE_STEP, (u16)(data << (int)offsbits), curmask);
                    }
                }
            }
        }


        public delegate void memory_write_generic32_wop(offs_t offset, u32 data, u32 mask);

        // generic direct write
        //template<int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, typename T>
        //void memory_write_generic(T wop, offs_t address, typename handler_entry_size<TargetWidth>::uX data, typename handler_entry_size<TargetWidth>::uX mask)
        public static void memory_write_generic32(int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, memory_write_generic32_wop wop, offs_t address, u32 data, u32 mask)
        {
            Type NativeType = memory_read_generic_FindType(Width);  //using NativeType = typename handler_entry_size<Width>::uX;

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
                    if (Endian != (int)endianness_t.ENDIANNESS_LITTLE) offsbits2 = NATIVE_BITS - TARGET_BITS - offsbits2;
                    wop(address & ~NATIVE_MASK, (u32 /*NativeType*/)(data << (int)offsbits2), (u32 /*NativeType*/)(mask << (int)offsbits2));
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
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                    // write lower bits to lower address
                    u32 /*NativeType*/ curmask = (u32 /*NativeType*/)(mask << (int)offsbits);
                    if (curmask != 0) wop(address, (u32 /*NativeType*/)(data << (int)offsbits), curmask);

                    // write upper bits to upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = (u32)(mask >> (int)offsbits);
                    if (curmask != 0) wop(address + NATIVE_STEP, (u32)(data >> (int)offsbits), curmask);
                }

                // big-endian case
                else
                {
                    // left-justify the mask and data to the target type
                    u32 LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT = ((NATIVE_BITS >= TARGET_BITS) ? (NATIVE_BITS - TARGET_BITS) : 0);
                    u32 /*NativeType*/ ljdata = (u32 /*NativeType*/)(data << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                    u32 /*NativeType*/ ljmask = (u32 /*NativeType*/)(mask << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                        // write upper bits to lower address
                    u32 /*NativeType*/ curmask = (u32)(ljmask >> (int)offsbits);
                    if (curmask != 0) wop(address, (u32)(ljdata >> (int)offsbits), curmask);
                        // write lower bits to upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = (u32)(ljmask << (int)offsbits);
                    if (curmask != 0) wop(address + NATIVE_STEP, (u32)(ljdata << (int)offsbits), curmask);
                }
            }

            // if we're here, then we have 2 or more writes needed to get our final result
            else
            {
                // compute the maximum number of loops; we do it this way so that there are
                // a fixed number of loops for the compiler to unroll if it desires
                u32 MAX_SPLITS_MINUS_ONE = TARGET_BYTES / NATIVE_BYTES - 1;

                // little-endian case
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                    // write lowest bits to first address
                    u32 /*NativeType*/ curmask = (u32)(mask << (int)offsbits);
                    if (curmask != 0) wop(address, (u32)(data << (int)offsbits), curmask);

                    // write middle bits to subsequent addresses
                    offsbits = NATIVE_BITS - offsbits;
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        address += NATIVE_STEP;
                        curmask = (u32)(mask >> (int)offsbits);
                        if (curmask != 0) wop(address, (u32)(data >> (int)offsbits), curmask);
                        offsbits += NATIVE_BITS;
                    }

                    // if we're not aligned and we still have bits left, write uppermost bits to last address
                    if (!Aligned && offsbits < TARGET_BITS)
                    {
                        curmask = (u32)(mask >> (int)offsbits);
                        if (curmask != 0) wop(address + NATIVE_STEP, (u32)(data >> (int)offsbits), curmask);
                    }
                }

                // big-endian case
                else
                {
                    // write highest bits to first address
                    offsbits = TARGET_BITS - (NATIVE_BITS - offsbits);
                    u32 /*NativeType*/ curmask = (u32)(mask >> (int)offsbits);
                    if (curmask != 0) wop(address, (u32)(data >> (int)offsbits), curmask);

                    // write middle bits to subsequent addresses
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        offsbits -= NATIVE_BITS;
                        address += NATIVE_STEP;
                        curmask = (u32)(mask >> (int)offsbits);
                        if (curmask != 0) wop(address, (u32)(data >> (int)offsbits), curmask);
                    }

                    // if we're not aligned and we still have bits left, write lowermost bits to the last address
                    if (!Aligned && offsbits != 0)
                    {
                        offsbits = NATIVE_BITS - offsbits;
                        curmask = (u32)(mask << (int)offsbits);
                        if (curmask != 0) wop(address + NATIVE_STEP, (u32)(data << (int)offsbits), curmask);
                    }
                }
            }
        }


        public delegate void memory_write_generic64_wop(offs_t offset, u64 data, u64 mask);

        // generic direct write
        //template<int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, typename T>
        //void memory_write_generic(T wop, offs_t address, typename handler_entry_size<TargetWidth>::uX data, typename handler_entry_size<TargetWidth>::uX mask)
        public static void memory_write_generic64(int Width, int AddrShift, int Endian, int TargetWidth, bool Aligned, memory_write_generic64_wop wop, offs_t address, u64 data, u64 mask)
        {
            Type NativeType = memory_read_generic_FindType(Width);  //using NativeType = typename handler_entry_size<Width>::uX;

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
                    if (Endian != (int)endianness_t.ENDIANNESS_LITTLE) offsbits2 = NATIVE_BITS - TARGET_BITS - offsbits2;
                    wop(address & ~NATIVE_MASK, (u64 /*NativeType*/)(data << (int)offsbits2), (u64 /*NativeType*/)(mask << (int)offsbits2));
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
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                    // write lower bits to lower address
                    u64 /*NativeType*/ curmask = (u64 /*NativeType*/)(mask << (int)offsbits);
                    if (curmask != 0) wop(address, (u64 /*NativeType*/)(data << (int)offsbits), curmask);

                    // write upper bits to upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = (u64)(mask >> (int)offsbits);
                    if (curmask != 0) wop(address + NATIVE_STEP, (u64)(data >> (int)offsbits), curmask);
                }

                // big-endian case
                else
                {
                    // left-justify the mask and data to the target type
                    u32 LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT = ((NATIVE_BITS >= TARGET_BITS) ? (NATIVE_BITS - TARGET_BITS) : 0);
                    u64 /*NativeType*/ ljdata = (u64 /*NativeType*/)(data << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                    u64 /*NativeType*/ ljmask = (u64 /*NativeType*/)(mask << (int)LEFT_JUSTIFY_TARGET_TO_NATIVE_SHIFT);
                        // write upper bits to lower address
                    u64 /*NativeType*/ curmask = (u64)(ljmask >> (int)offsbits);
                    if (curmask != 0) wop(address, (u64)(ljdata >> (int)offsbits), curmask);
                        // write lower bits to upper address
                    offsbits = NATIVE_BITS - offsbits;
                    curmask = (u64)(ljmask << (int)offsbits);
                    if (curmask != 0) wop(address + NATIVE_STEP, (u64)(ljdata << (int)offsbits), curmask);
                }
            }

            // if we're here, then we have 2 or more writes needed to get our final result
            else
            {
                // compute the maximum number of loops; we do it this way so that there are
                // a fixed number of loops for the compiler to unroll if it desires
                u32 MAX_SPLITS_MINUS_ONE = TARGET_BYTES / NATIVE_BYTES - 1;

                // little-endian case
                if (Endian == (int)endianness_t.ENDIANNESS_LITTLE)
                {
                    // write lowest bits to first address
                    u64 /*NativeType*/ curmask = (u64)(mask << (int)offsbits);
                    if (curmask != 0) wop(address, (u64)(data << (int)offsbits), curmask);

                    // write middle bits to subsequent addresses
                    offsbits = NATIVE_BITS - offsbits;
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        address += NATIVE_STEP;
                        curmask = (u64)(mask >> (int)offsbits);
                        if (curmask != 0) wop(address, (u64)(data >> (int)offsbits), curmask);
                        offsbits += NATIVE_BITS;
                    }

                    // if we're not aligned and we still have bits left, write uppermost bits to last address
                    if (!Aligned && offsbits < TARGET_BITS)
                    {
                        curmask = (u64)(mask >> (int)offsbits);
                        if (curmask != 0) wop(address + NATIVE_STEP, (u64)(data >> (int)offsbits), curmask);
                    }
                }

                // big-endian case
                else
                {
                    // write highest bits to first address
                    offsbits = TARGET_BITS - (NATIVE_BITS - offsbits);
                    u64 /*NativeType*/ curmask = (u64)(mask >> (int)offsbits);
                    if (curmask != 0) wop(address, (u64)(data >> (int)offsbits), curmask);

                    // write middle bits to subsequent addresses
                    for (u32 index = 0; index < MAX_SPLITS_MINUS_ONE; index++)
                    {
                        offsbits -= NATIVE_BITS;
                        address += NATIVE_STEP;
                        curmask = (u64)(mask >> (int)offsbits);
                        if (curmask != 0) wop(address, (u64)(data >> (int)offsbits), curmask);
                    }

                    // if we're not aligned and we still have bits left, write lowermost bits to the last address
                    if (!Aligned && offsbits != 0)
                    {
                        offsbits = NATIVE_BITS - offsbits;
                        curmask = (u64)(mask << (int)offsbits);
                        if (curmask != 0) wop(address + NATIVE_STEP, (u64)(data << (int)offsbits), curmask);
                    }
                }
            }
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

#if false
    // a line in the memory structure dump
    struct memory_entry {
        offs_t start, end;
        class handler_entry *entry;
    };
#endif


    // =====================-> The root class of all handlers

    // Handlers the refcounting as part of the interface

    public class handler_entry : global_object, IDisposable
    {
        //DISABLE_COPYING(handler_entry);

        //template<int Width, int AddrShift, endianness_t Endian> friend class address_space_specific;


        // Typing flags
        public const u32 F_DISPATCH    = 0x00000001; // handler that forwards the access to other handlers
        //static constexpr u32 F_UNITS       = 0x00000002; // handler that merges/splits an access among multiple handlers (unitmask support)
        //static constexpr u32 F_PASSTHROUGH = 0x00000004; // handler that passes through the request to another handler

        // Start/end of range flags
        //static constexpr u8 START = 1;
        //static constexpr u8 END   = 2;

        // Intermediary structure for reference count checking
        //class reflist {
        //public:
        //    void add(const handler_entry *entry);
        //
        //    void propagate();
        //    void check();
        //
        //private:
        //    std::unordered_map<const handler_entry *, u32> refcounts;
        //    std::unordered_set<const handler_entry *> seen;
        //    std::unordered_set<const handler_entry *> todo;
        //};


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

            void intersect(offs_t _start, offs_t _end)
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
        //inline bool is_units() const { return m_flags & F_UNITS; }
        //inline bool is_passthrough() const { return m_flags & F_PASSTHROUGH; }

        //virtual void dump_map(std::vector<memory_entry> &map) const;

        //virtual std::string name() const = 0;
        //virtual void enumerate_references(handler_entry::reflist &refs) const;
        //u32 get_refcount() const { return m_refcount; }
    }


    // =====================-> The parent class of all read handlers

    // Provides the populate/read/get_ptr/lookup API

    //template<int Width, int AddrShift, int Endian> class handler_entry_read_passthrough;

    //template<int Width, int AddrShift, int Endian>
    public abstract class handler_entry_read : handler_entry
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;

        //struct mapping {
        //    handler_entry_read<Width, AddrShift, Endian> *original;
        //    handler_entry_read<Width, AddrShift, Endian> *patched;
        //    u8 ukey;
        //};


        // template parameters
        protected int Width;
        protected int AddrShift;
        protected int Endian;


        public handler_entry_read(int Width, int AddrShift, int Endian, address_space space, u32 flags) : base(space, flags) { this.Width = Width; this.AddrShift = AddrShift; this.Endian = Endian; }
        //~handler_entry_read() {}


        //virtual uX read(offs_t offset, uX mem_mask) = 0;
        public abstract u8 read(offs_t offset, u8 mem_mask);


        //virtual void *get_ptr(offs_t offset) const;


        public virtual void lookup(offs_t address, ref offs_t start, ref offs_t end, ref handler_entry_read handler)
        {
            fatalerror("lookup called on non-dispatching class\n");
        }


        public void populate(offs_t start, offs_t end, offs_t mirror, handler_entry_read handler)
        {
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


        //inline void populate_mismatched(offs_t start, offs_t end, offs_t mirror, const memory_units_descriptor<Width, AddrShift, Endian> &descriptor) {
        //    std::vector<mapping> mappings;
        //    if(mirror)
        //        populate_mismatched_mirror(start, end, start, end, mirror, descriptor, mappings);
        //    else
        //        populate_mismatched_nomirror(start, end, start, end, descriptor, START|END, mappings);
        //}

        //virtual void populate_mismatched_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, const memory_units_descriptor<Width, AddrShift, Endian> &descriptor, u8 rkey, std::vector<mapping> &mappings);
        //virtual void populate_mismatched_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, const memory_units_descriptor<Width, AddrShift, Endian> &descriptor, std::vector<mapping> &mappings);

        //inline void populate_passthrough(offs_t start, offs_t end, offs_t mirror, handler_entry_read_passthrough<Width, AddrShift, Endian> *handler) {
        //    std::vector<mapping> mappings;
        //    if(mirror)
        //        populate_passthrough_mirror(start, end, start, end, mirror, handler, mappings);
        //    else
        //        populate_passthrough_nomirror(start, end, start, end, handler, mappings);
        //}

        //virtual void populate_passthrough_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_read_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings);
        //virtual void populate_passthrough_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_read_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings);

        // Remove a set of passthrough handlers, leaving the lower handler in their place
        //virtual void detach(const std::unordered_set<handler_entry *> &handlers);
    }


    // =====================-> The parent class of all write handlers

    // Provides the populate/write/get_ptr/lookup API

    //template<int Width, int AddrShift, int Endian> class handler_entry_write_passthrough;

    //template<int Width, int AddrShift, int Endian>
    public abstract class handler_entry_write : handler_entry
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;

        //struct mapping {
        //    handler_entry_write<Width, AddrShift, Endian> *original;
        //    handler_entry_write<Width, AddrShift, Endian> *patched;
        //    u8 ukey;
        //};


        // template parameters
        protected int Width;
        protected int AddrShift;
        protected int Endian;


        public handler_entry_write(int Width, int AddrShift, int Endian, address_space space, u32 flags) : base(space, flags) { this.Width = Width; this.AddrShift = AddrShift; this.Endian = Endian; }
        //~handler_entry_write() {}


        public abstract void write(offs_t offset, u8 data, u8 mem_mask);


        //virtual void *get_ptr(offs_t offset) const;
        //virtual void lookup(offs_t address, offs_t &start, offs_t &end, handler_entry_write<Width, AddrShift, Endian> *&handler) const;


        public void populate(offs_t start, offs_t end, offs_t mirror, handler_entry_write handler)
        {
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


        //inline void populate_mismatched(offs_t start, offs_t end, offs_t mirror, const memory_units_descriptor<Width, AddrShift, Endian> &descriptor) {
        //    std::vector<mapping> mappings;
        //    if(mirror)
        //        populate_mismatched_mirror(start, end, start, end, mirror, descriptor, mappings);
        //    else
        //        populate_mismatched_nomirror(start, end, start, end, descriptor, START|END, mappings);
        //}

        //virtual void populate_mismatched_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, const memory_units_descriptor<Width, AddrShift, Endian> &descriptor, u8 rkey, std::vector<mapping> &mappings);
        //virtual void populate_mismatched_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, const memory_units_descriptor<Width, AddrShift, Endian> &descriptor, std::vector<mapping> &mappings);

        //inline void populate_passthrough(offs_t start, offs_t end, offs_t mirror, handler_entry_write_passthrough<Width, AddrShift, Endian> *handler) {
        //    std::vector<mapping> mappings;
        //    if(mirror)
        //        populate_passthrough_mirror(start, end, start, end, mirror, handler, mappings);
        //    else
        //        populate_passthrough_nomirror(start, end, start, end, handler, mappings);
        //}

        //virtual void populate_passthrough_nomirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, handler_entry_write_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings);
        //virtual void populate_passthrough_mirror(offs_t start, offs_t end, offs_t ostart, offs_t oend, offs_t mirror, handler_entry_write_passthrough<Width, AddrShift, Endian> *handler, std::vector<mapping> &mappings);

        // Remove a set of passthrough handlers, leaving the lower handler in their place
        //virtual void detach(const std::unordered_set<handler_entry *> &handlers);
    }


#if false
    // =====================-> Passthrough handler management structure
    class memory_passthrough_handler
    {
        template<int Width, int AddrShift, int Endian> friend class handler_entry_read_passthrough;
        template<int Width, int AddrShift, int Endian> friend class handler_entry_write_passthrough;

    public:
        memory_passthrough_handler(address_space &space) : m_space(space) {}

        inline void remove();

    private:
        address_space &m_space;
        std::unordered_set<handler_entry *> m_handlers;

        void add_handler(handler_entry *handler) { m_handlers.insert(handler); }
        void remove_handler(handler_entry *handler) { m_handlers.erase(m_handlers.find(handler)); }
    };
#endif

    // =====================-> Forward declaration for address_space

    //template<int Width, int AddrShift, int Endian> class handler_entry_read_unmapped;
    //template<int Width, int AddrShift, int Endian> class handler_entry_write_unmapped;


    // ======================> memory_access_cache

    // memory_access_cache contains state data for cached access
    //template<int Width, int AddrShift, int Endian>
    public class memory_access_cache : global_object, IDisposable
    {
        //using NativeType = typename emu::detail::handler_entry_size<Width>::uX;

        u32 NATIVE_BYTES; // computed in ctor  = 1 << Width;
        u32 NATIVE_MASK; // computed in ctor  = Width + AddrShift >= 0 ? (1 << (Width + AddrShift)) - 1 : 0;


        // template parameters
        int Width;
        int AddrShift;
        int Endian;


        // internal state
        address_space m_space;
        int m_notifier_id;             // id to remove the notifier on destruction
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
        public memory_access_cache(int Width, int AddrShift, int Endian, address_space space,
                        handler_entry_read root_read,  //handler_entry_read <Width, AddrShift, Endian> root_read,
                        handler_entry_write root_write)  //handler_entry_write<Width, AddrShift, Endian> root_write)
        {
            this.Width = Width;
            this.AddrShift = AddrShift;
            this.Endian = Endian;


            NATIVE_BYTES = 1U << Width;
            NATIVE_MASK = (u32)(Width + AddrShift >= 0 ? (1 << (Width + AddrShift)) - 1 : 0);


            m_space = space;
            m_addrmask = space.addrmask();
            m_addrstart_r = 1;
            m_addrend_r = 0;
            m_addrstart_w = 1;
            m_addrend_w = 0;
            m_cache_r = null;
            m_cache_w = null;
            m_root_read = root_read;
            m_root_write = root_write;


            m_notifier_id = space.add_change_notifier((read_or_write mode) =>
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
        }

        ~memory_access_cache()
        {
            assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            m_space.remove_change_notifier(m_notifier_id);
            m_isDisposed = true;
        }


        // getters
        //address_space &space() const { return m_space; }

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

        //void *read_ptr(offs_t address) {
        //    check_address_r(address);
        //    return m_cache_r->get_ptr(address);
        //}


        public u8 read_byte(offs_t address) { address &= m_addrmask; return Width == 0 ? read_native8(address & ~NATIVE_MASK) : memory_read_generic8(Width, AddrShift, Endian, 0, true, (offs_t offset, u8 mask) => { return read_native8(offset, mask); }, address, 0xff); }
        //u16 read_word(offs_t address) { address &= m_addrmask; return Width == 1 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffff); }
        //u16 read_word(offs_t address, u16 mask) { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u16 read_word_unaligned(offs_t address) { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffff); }
        //u16 read_word_unaligned(offs_t address, u16 mask) { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u32 read_dword(offs_t address) { address &= m_addrmask; return Width == 2 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffff); }
        //u32 read_dword(offs_t address, u32 mask) { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u32 read_dword_unaligned(offs_t address) { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffff); }
        //u32 read_dword_unaligned(offs_t address, u32 mask) { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u64 read_qword(offs_t address) { address &= m_addrmask; return Width == 3 ? read_native(address & ~NATIVE_MASK) : memory_read_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffffffffffffU); }
        //u64 read_qword(offs_t address, u64 mask) { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }
        //u64 read_qword_unaligned(offs_t address) { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, 0xffffffffffffffffU); }
        //u64 read_qword_unaligned(offs_t address, u64 mask) { address &= m_addrmask; return memory_read_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType mask) -> NativeType { return read_native(offset, mask); }, address, mask); }

        //void write_byte(offs_t address, u8 data) { address &= m_addrmask; if (Width == 0) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 0, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xff); }
        //void write_word(offs_t address, u16 data) { address &= m_addrmask; if (Width == 1) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffff); }
        //void write_word(offs_t address, u16 data, u16 mask) { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 1, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_word_unaligned(offs_t address, u16 data) { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffff); }
        //void write_word_unaligned(offs_t address, u16 data, u16 mask) { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 1, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_dword(offs_t address, u32 data) { address &= m_addrmask; if (Width == 2) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffff); }
        //void write_dword(offs_t address, u32 data, u32 mask) { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 2, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_dword_unaligned(offs_t address, u32 data) { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffff); }
        //void write_dword_unaligned(offs_t address, u32 data, u32 mask) { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 2, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_qword(offs_t address, u64 data) { address &= m_addrmask; if (Width == 3) write_native(address & ~NATIVE_MASK, data); else memory_write_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffffffffffffU); }
        //void write_qword(offs_t address, u64 data, u64 mask) { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 3, true>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }
        //void write_qword_unaligned(offs_t address, u64 data) { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, 0xffffffffffffffffU); }
        //void write_qword_unaligned(offs_t address, u64 data, u64 mask) { address &= m_addrmask; memory_write_generic<Width, AddrShift, Endian, 3, false>([this](offs_t offset, NativeType data, NativeType mask) { write_native(offset, data, mask); }, address, data, mask); }


        //NativeType read_native(offs_t address, NativeType mask = ~NativeType(0));
        //template<int Width, int AddrShift, int Endian> typename emu::detail::handler_entry_size<Width>::uX memory_access_cache<Width, AddrShift, Endian>::read_native(offs_t address, typename emu::detail::handler_entry_size<Width>::uX mask)
        //{
        //    check_address_r(address);
        //    return m_cache_r->read(address, mask);
        //}

        u8 read_native8(offs_t address, u8 mask = u8.MaxValue)
        {
            check_address_r(address);
            return m_cache_r.read(address, mask);
        }


        //void write_native(offs_t address, NativeType data, NativeType mask = ~NativeType(0));
        //template<int Width, int AddrShift, int Endian> void memory_access_cache<Width, AddrShift, Endian>::write_native(offs_t address, typename emu::detail::handler_entry_size<Width>::uX data, typename emu::detail::handler_entry_size<Width>::uX mask)
        //{
        //    check_address_w(address);
        //    m_cache_w->write(address, data, mask);
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
        bool m_is_octal;                 // to determine if messages/debugger will show octal or hex

        address_map_constructor m_internal_map;


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


        // getters
        public string name() { return m_name; }
        public endianness_t endianness() { return m_endianness; }
        public int data_width() { return m_data_width; }
        public int addr_width() { return m_addr_width; }
        public int addr_shift() { return m_addr_shift; }
        public byte logaddr_width() { return m_logaddr_width; }
        //int page_shift() const { return m_page_shift; }
        public bool is_octal() { return m_is_octal; }

        public address_map_constructor internal_map { get { return m_internal_map; } }


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
        //template<int Width, int AddrShift, int Endian> friend class handler_entry_read_unmapped;
        //template<int Width, int AddrShift, int Endian> friend class handler_entry_write_unmapped;
        //template<int Width, int AddrShift, int Endian> friend class memory_access_cache;


        public delegate void notifier_func(read_or_write row);


        class notifier_t
        {
            public notifier_func m_notifier;
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


        //template<int Width, int AddrShift, int Endian> memory_access_cache<Width, AddrShift, Endian> *cache() const
        public memory_access_cache cache(int Width, int AddrShift, int Endian)
        {
            if (AddrShift != m_config.addr_shift())
                fatalerror("Requesting cache() with address shift {0} while the config says {1}\n", AddrShift, m_config.addr_shift());
            if (8 << Width != m_config.data_width())
                fatalerror("Requesting cache() with data width {0} while the config says {1}\n", 8 << Width, m_config.data_width());
            if (Endian != (int)m_config.endianness())
                fatalerror("Requesting cache() with endianness {0} while the config says {1}\n",
                           endianness_names[Endian], endianness_names[(int)m_config.endianness()]);

            return create_cache();  //static_cast<memory_access_cache<Width, AddrShift, Endian> *>(create_cache());
        }


        public int add_change_notifier(notifier_func n)  //std::function<void (read_or_write)> n);
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
        int addr_width() { return m_config.addr_width(); }
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
        //UInt16 *install_read_handler(offs_t addrstart, offs_t addrend, read16_delegate rhandler, UInt64 unitmask = 0) { return install_read_handler(addrstart, addrend, 0, 0, rhandler, unitmask); }
        //UInt16 *install_write_handler(offs_t addrstart, offs_t addrend, write16_delegate whandler, UInt64 unitmask = 0) { return install_write_handler(addrstart, addrend, 0, 0, whandler, unitmask); }
        //UInt16 *install_readwrite_handler(offs_t addrstart, offs_t addrend, read16_delegate rhandler, write16_delegate whandler, UInt64 unitmask = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, rhandler, whandler, unitmask); }
        //UInt32 *install_read_handler(offs_t addrstart, offs_t addrend, read32_delegate rhandler, UInt64 unitmask = 0) { return install_read_handler(addrstart, addrend, 0, 0, rhandler, unitmask); }
        //UInt32 *install_write_handler(offs_t addrstart, offs_t addrend, write32_delegate whandler, UInt64 unitmask = 0) { return install_write_handler(addrstart, addrend, 0, 0, whandler, unitmask); }
        //UInt32 *install_readwrite_handler(offs_t addrstart, offs_t addrend, read32_delegate rhandler, write32_delegate whandler, UInt64 unitmask = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, rhandler, whandler, unitmask); }
        //UInt64 *install_read_handler(offs_t addrstart, offs_t addrend, read64_delegate rhandler, UInt64 unitmask = 0) { return install_read_handler(addrstart, addrend, 0, 0, rhandler, unitmask); }
        //UInt64 *install_write_handler(offs_t addrstart, offs_t addrend, write64_delegate whandler, UInt64 unitmask = 0) { return install_write_handler(addrstart, addrend, 0, 0, whandler, unitmask); }
        //UInt64 *install_readwrite_handler(offs_t addrstart, offs_t addrend, read64_delegate rhandler, write64_delegate whandler, UInt64 unitmask = 0) { return install_readwrite_handler(addrstart, addrend, 0, 0, rhandler, whandler, unitmask); }

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

        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;

        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
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

        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) = 0;
        //virtual void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) = 0;
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
            m_map.import_submaps(m_manager.machine(), m_device.owner() != null ? m_device.owner() : m_device, data_width(), endianness());

            // extract global parameters specified by the map
            m_unmap = (m_map.unmapval == 0) ? 0UL : ~0UL;
            if (m_map.globalmask != 0)
            {
                if ((m_map.globalmask & ~m_addrmask) != 0)
                    fatalerror("Can't set a global address mask of {0} on a {1}-bits address width bus.\n", m_map.globalmask, addr_width());  //%08x

                m_addrmask = m_map.globalmask;
            }

            // make a pass over the address map, adjusting for the device and getting memory pointers
            foreach (address_map_entry entry in m_map.entrylist)
            {
                // computed adjusted addresses first
                offs_t addrstart_temp = entry.addrstart;
                offs_t addrend_temp = entry.addrend;
                offs_t addrmask_temp = entry.addrmask;
                offs_t addrmirror_temp = entry.addrmirror;
                adjust_addresses(ref addrstart_temp, ref addrend_temp, ref addrmask_temp, ref addrmirror_temp);
                entry.addrstart = addrstart_temp;
                entry.addrend = addrend_temp;
                entry.addrmask = addrmask_temp;
                entry.addrmirror = addrmirror_temp;

                // if we have a share entry, add it to our map
                if (entry.share_get != null)
                {
                    // if we can't find it, add it to our map
                    string fulltag = entry.devbase.subtag(entry.share_get);
                    if (m_manager.shares().find(fulltag) == null)
                    {
                        emumem_global.VPRINTF("Creating share '{0}' of length {1}\n", fulltag, entry.addrend + 1 - entry.addrstart);
                        m_manager.shares().emplace(fulltag.c_str(), new memory_share((byte)m_config.data_width(), address_to_byte(entry.addrend + 1 - entry.addrstart), endianness()));
                    }
                }

                // if this is a ROM handler without a specified region, attach it to the implicit region
                if (m_spacenum == 0 && entry.read.type == map_handler_type.AMH_ROM && entry.region_var == null)
                {
                    // make sure it fits within the memory region before doing so, however
                    if (entry.addrend < devregionsize)
                    {
                        entry.region_var = m_device.tag();
                        entry.rgnoffs = address_to_byte(entry.addrstart);
                    }
                }

                // validate adjusted addresses against implicit regions
                if (entry.region_var != null && entry.share_get == null)
                {
                    // determine full tag
                    string fulltag = entry.devbase.subtag(entry.region_var);

                    // find the region
                    memory_region region = m_manager.machine().root_device().memregion(fulltag);
                    if (region == null)
                        fatalerror("device '{0}' {1} space memory map entry {2}-{3} references nonexistant region \"{4}\"\n", m_device.tag(), m_name, entry.addrstart, entry.addrend, entry.region_var);

                    // validate the region
                    if (entry.rgnoffs + m_config.addr2byte(entry.addrend - entry.addrstart + 1) > region.bytes())
                        fatalerror("device '{0}' {1} space memory map entry {2}-{3} extends beyond region \"{4}\" size ({5})\n", m_device.tag(), m_name, entry.addrstart, entry.addrend, entry.region_var, region.bytes());
                }

                // convert any region-relative entries to their memory pointers
                if (entry.region_var != null)
                {
                    // determine full tag
                    string fulltag = entry.devbase.subtag(entry.region_var);

                    // set the memory address
                    entry.memory = new ListBytesPointer(m_manager.machine().root_device().memregion(fulltag).base_(), (int)entry.rgnoffs);  //entry.m_memory = m_manager.machine().root_device().memregion(fulltag.c_str())->base() + entry.m_rgnoffs;
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
            var blocklist = m_manager.blocklist;

            // make a first pass over the memory map and track blocks with hardcoded pointers
            // we do this to make sure they are found by space_find_backing_memory first
            // do it back-to-front so that overrides work correctly
            int tail = blocklist.size();
            foreach (var entry in m_map.entrylist)
            {
                if (entry.memory != null)
                    blocklist.insert(0 + tail, new memory_block(this, entry.addrstart, entry.addrend, entry.memory));  //blocklist.insert(blocklist.begin() + tail, new memory_block(this, entry.addrstart, entry.addrend, entry.memory_ptr));
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
                offs_t curblockstart = unassigned.addrstart / emumem_global.MEMORY_BLOCK_CHUNK;
                offs_t curblockend = unassigned.addrend / emumem_global.MEMORY_BLOCK_CHUNK;

                // loop while we keep finding unassigned blocks in neighboring MEMORY_BLOCK_CHUNK chunks
                bool changed;
                do
                {
                    changed = false;

                    // scan for unmapped blocks in the adjusted map
                    for (address_map_entry entry = m_map.entrylist.first(); entry != null; entry = entry.next())
                    {
                        if (entry.memory == null && entry != unassigned && needs_backing_store(entry))
                        {
                            // get block start/end blocks for this block
                            offs_t blockstart = entry.addrstart / emumem_global.MEMORY_BLOCK_CHUNK;
                            offs_t blockend = entry.addrend / emumem_global.MEMORY_BLOCK_CHUNK;

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
                    foreach (address_map_entry entry in m_map.entrylist)
                    {
                        if (entry.addrstart == bank.second().addrstart() && entry.memory != null)
                        {
                            bank.second().set_base(entry.memory);
                            emumem_global.VPRINTF("assigned bank '{0}' pointer to memory from range {1:x8}-{2:x8} [{3}]\n", bank.second().tag(), entry.addrstart, entry.addrend, entry.memory);
                            break;
                        }
                    }
                }
            }
        }


        public handler_entry_read_unmapped get_unmap_r() { return (handler_entry_read_unmapped)m_unmap_r; }  //template<int Width, int AddrShift, int Endian> handler_entry_read_unmapped <Width, AddrShift, Endian> *get_unmap_r() const { return static_cast<handler_entry_read_unmapped <Width, AddrShift, Endian> *>(m_unmap_r); }
        public handler_entry_write_unmapped get_unmap_w() { return (handler_entry_write_unmapped)m_unmap_w; }  //template<int Width, int AddrShift, int Endian> handler_entry_write_unmapped<Width, AddrShift, Endian> *get_unmap_w() const { return static_cast<handler_entry_write_unmapped<Width, AddrShift, Endian> *>(m_unmap_w); }


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
            foreach (address_map_entry entry in map.entrylist)
            {
                // map both read and write halves
                populate_map_entry(entry, read_or_write.READ);
                populate_map_entry(entry, read_or_write.WRITE);
            }

            if (emumem_global.VALIDATE_REFCOUNTS)
                validate_reference_counts();
        }


        // internal helpers
        protected abstract memory_access_cache create_cache();  //void *create_cache();


        //-------------------------------------------------
        //  populate_map_entry - map a single read or
        //  write entry based on information from an
        //  address map entry
        //-------------------------------------------------
        void populate_map_entry(address_map_entry entry, read_or_write readorwrite)
        {
            map_handler_data data = (readorwrite == read_or_write.READ) ? entry.read : entry.write;

            // based on the handler type, alter the bits, name, funcptr, and object
            switch (data.type)
            {
                case map_handler_type.AMH_NONE:
                    return;

                case map_handler_type.AMH_ROM:
                    // writes to ROM are no-ops
                    if (readorwrite == read_or_write.WRITE)
                        return;
                    // fall through to the RAM case otherwise
                    install_ram_generic(entry.addrstart, entry.addrend, entry.addrmirror, readorwrite, null);
                    break;

                case map_handler_type.AMH_RAM:
                    install_ram_generic(entry.addrstart, entry.addrend, entry.addrmirror, readorwrite, null);
                    break;

                case map_handler_type.AMH_NOP:
                    unmap_generic(entry.addrstart, entry.addrend, entry.addrmirror, readorwrite, true);
                    break;

                case map_handler_type.AMH_UNMAP:
                    unmap_generic(entry.addrstart, entry.addrend, entry.addrmirror, readorwrite, false);
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE:
                    if (readorwrite == read_or_write.READ)
                    {
                        switch (data.bits)
                        {
                            case 8:     install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read8_delegate(entry.rproto8/*,   entry.devbase()*/), entry.mask, entry.cswidth_get); break;
                            case 16:    install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read16_delegate(entry.rproto16/*, entry.devbase()*/), entry.mask, entry.cswidth_get); break;
                            case 32:    install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read32_delegate(entry.rproto32/*, entry.devbase()*/), entry.mask, entry.cswidth_get); break;
                            case 64:    install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read64_delegate(entry.rproto64/*, entry.devbase()*/), entry.mask, entry.cswidth_get); break;
                        }
                    }
                    else
                    {
                        switch (data.bits)
                        {
                            case 8:     install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write8_delegate(entry.wproto8/*,   entry.devbase()*/), entry.mask, entry.cswidth_get); break;
                            case 16:    install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write16_delegate(entry.wproto16/*, entry.devbase()*/), entry.mask, entry.cswidth_get); break;
                            case 32:    install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write32_delegate(entry.wproto32/*, entry.devbase()*/), entry.mask, entry.cswidth_get); break;
                            case 64:    install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write64_delegate(entry.wproto64/*, entry.devbase()*/), entry.mask, entry.cswidth_get); break;
                        }
                    }
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE_M:
                    if (readorwrite == read_or_write.READ)
                        switch (data.bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read8m_delegate(entry.m_rproto8m/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                            case 16:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read16m_delegate(entry.m_rproto16m/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                            case 32:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read32m_delegate(entry.m_rproto32m/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                            case 64:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read64m_delegate(entry.m_rproto64m/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                        }
                    else
                        switch (data.bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write8m_delegate(entry.m_wproto8m/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                            case 16:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write16m_delegate(entry.m_wproto16m/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                            case 32:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write32m_delegate(entry.m_wproto32m/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                            case 64:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write64m_delegate(entry.m_wproto64m/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                        }
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE_S:
                    if (readorwrite == read_or_write.READ)
                        switch (data.bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read8s_delegate(entry.m_rproto8s/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                            case 16:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read16s_delegate(entry.m_rproto16s/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                            case 32:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read32s_delegate(entry.m_rproto32s/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                            case 64:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read64s_delegate(entry.m_rproto64s/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                        }
                    else
                        switch (data.bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write8s_delegate(entry.m_wproto8s/*, entry.m_devbas*/), entry.mask, entry.cswidth_get); break;
                            case 16:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write16s_delegate(entry.m_wproto16s/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                            case 32:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write32s_delegate(entry.m_wproto32s/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                            case 64:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write64s_delegate(entry.m_wproto64s/*, entry.m_devbase*/), entry.mask, entry.cswidth_get); break;
                        }
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE_SM:
                    if (readorwrite == read_or_write.READ)
                        switch (data.bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read8sm_delegate(entry.m_rproto8sm, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 16:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read16sm_delegate(entry.m_rproto16sm, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 32:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read32sm_delegate(entry.m_rproto32sm, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 64:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read64sm_delegate(entry.m_rproto64sm, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                        }
                    else
                        switch (data.bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write8sm_delegate(entry.m_wproto8sm, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 16:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write16sm_delegate(entry.m_wproto16sm, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 32:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write32sm_delegate(entry.m_wproto32sm, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 64:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write64sm_delegate(entry.m_wproto64sm, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                        }
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE_MO:
                    if (readorwrite == read_or_write.READ)
                        switch (data.bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read8mo_delegate(entry.m_rproto8mo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 16:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read16mo_delegate(entry.m_rproto16mo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 32:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read32mo_delegate(entry.m_rproto32mo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 64:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read64mo_delegate(entry.m_rproto64mo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                        }
                    else
                        switch (data.bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write8mo_delegate(entry.m_wproto8mo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 16:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write16mo_delegate(entry.m_wproto16mo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 32:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write32mo_delegate(entry.m_wproto32mo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 64:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write64mo_delegate(entry.m_wproto64mo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                        }
                    break;

                case map_handler_type.AMH_DEVICE_DELEGATE_SMO:
                    if (readorwrite == read_or_write.READ)
                        switch (data.bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read8smo_delegate(entry.m_rproto8smo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 16:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read16smo_delegate(entry.m_rproto16smo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 32:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read32smo_delegate(entry.m_rproto32smo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 64:    throw new emu_unimplemented();  //install_read_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new read64smo_delegate(entry.m_rproto64smo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                        }
                    else
                        switch (data.bits)
                        {
                            case 8:     throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write8smo_delegate(entry.m_wproto8smo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 16:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write16smo_delegate(entry.m_wproto16smo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 32:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write32smo_delegate(entry.m_wproto32smo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                            case 64:    throw new emu_unimplemented();  //install_write_handler(entry.addrstart, entry.addrend, entry.addrmask, entry.addrmirror, entry.addrselect, new write64smo_delegate(entry.m_wproto64smo, entry.m_devbase), entry.mask, entry.cswidth_get); break;
                        }
                    break;

                case map_handler_type.AMH_PORT:
                    install_readwrite_port(entry.addrstart, entry.addrend, entry.addrmirror,
                                    (readorwrite == read_or_write.READ) ? entry.devbase.subtag(data.tag) : "",
                                    (readorwrite == read_or_write.WRITE) ? entry.devbase.subtag(data.tag) : "");
                    break;

                case map_handler_type.AMH_BANK:
                    install_bank_generic(entry.addrstart, entry.addrend, entry.addrmirror,
                                    (readorwrite == read_or_write.READ) ? entry.devbase.subtag(data.tag) : "",
                                    (readorwrite == read_or_write.WRITE) ? entry.devbase.subtag(data.tag) : "");
                    break;

                case map_handler_type.AMH_DEVICE_SUBMAP:
                    throw new emu_fatalerror("Internal mapping error: leftover mapping of '{0}'.\n", data.tag);
            }
        }


        protected abstract void unmap_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, bool quiet);
        protected abstract void install_ram_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, ListBytesPointer baseptr);  //void *baseptr)
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

            // Check if we have to adjust the unitmask and addresses
            nunitmask = 0xffffffffffffffffU >> (64 - m_config.data_width());
            if (unitmask != 0)
                nunitmask &= unitmask;

            if ((addrstart & default_lowbits_mask) != 0 || ((~addrend) & default_lowbits_mask) != 0)
            {
                if (((addrstart ^ addrend) & ~default_lowbits_mask) != 0)
                    fatalerror("{0}: In range {1}-{2} mask {3} mirror {4} select {5}, start or end is unaligned while the range spans more than one slot (granularity = {6}).\n", function, addrstart, addrend, addrmask, addrmirror, addrselect, default_lowbits_mask + 1);
                offs_t lowbyte = m_config.addr2byte(addrstart & default_lowbits_mask);
                offs_t highbyte = m_config.addr2byte((addrend & default_lowbits_mask) + 1);
                if (m_config.endianness() == endianness_t.ENDIANNESS_LITTLE)
                {
                    u64 hmask = 0xffffffffffffffffU >> (64 - 8 * (int)highbyte);
                    nunitmask = (nunitmask << (8 * (int)lowbyte)) & hmask;
                }
                else
                {
                    u64 hmask = 0xffffffffffffffffU >> ((64 - m_config.data_width()) + 8 * (int)lowbyte);
                    nunitmask = (nunitmask << (m_config.data_width() - 8 * (int)highbyte)) & hmask;
                }

                addrstart &= ~default_lowbits_mask;
                addrend |= default_lowbits_mask;
                if (changing_bits < default_lowbits_mask)
                    changing_bits = default_lowbits_mask;
            }

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
        protected ListBytesPointer find_backing_memory(offs_t addrstart, offs_t addrend)
        {
            emumem_global.VPRINTF("address_space::find_backing_memory('{0}',{1},{2:x8}-{3:x8}) -> ", m_device.tag(), m_name, addrstart, addrend);

            if (m_map == null)
                return null;

            // look in the address map first, last winning for overrides
            ListBytesPointer result = null;  //void *result = nullptr;
            foreach (var entry in m_map.entrylist)
            {
                if (entry.memory != null && addrstart >= entry.addrstart && addrend <= entry.addrend)
                {
                    emumem_global.VPRINTF("found in entry {0:x8}-{1:x8} [{2} - {3:x8}]\n", entry.addrstart, entry.addrend, entry.memory, address_to_byte(addrstart - entry.addrstart));
                    result = new ListBytesPointer(entry.memory, (int)address_to_byte(addrstart - entry.addrstart));  //result = (u8 *)entry.m_memory + address_to_byte(addrstart - entry.m_addrstart);
                }
            }

            if (result != null)
                return result;

            // if not found there, look in the allocated blocks
            foreach (var block in m_manager.blocklist)
            {
                if (block.contains(this, addrstart, addrend))
                {
                    emumem_global.VPRINTF("found in allocated memory block {0:x8}-{1:x8} [{2} - {3:x8}]\n", block.addrstart(), block.addrend(), block.data(), address_to_byte(addrstart - block.addrstart()));
                    return new ListBytesPointer(block.data(), (int)address_to_byte(addrstart - block.addrstart()));  //return block->data() + address_to_byte(addrstart - block->addrstart());
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
            if (entry.share_get != null)
            {
                string fulltag = entry.devbase.subtag(entry.share_get);
                var share = m_manager.shares().find(fulltag);
                if (share != null && share.ptr() == null)
                    return true;
            }

            // if we're writing to any sort of bank or RAM, then yes, we do need backing
            if (entry.write.type == map_handler_type.AMH_BANK || entry.write.type == map_handler_type.AMH_RAM)
                return true;

            // if we're reading from RAM or from ROM outside of address space 0 or its region, then yes, we do need backing
            memory_region region = m_manager.machine().root_device().memregion(m_device.tag());
            if (entry.read.type == map_handler_type.AMH_RAM ||
               (entry.read.type == map_handler_type.AMH_ROM && (m_spacenum != 0 || region == null || entry.addrstart >= region.bytes())))
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
        address_map_entry block_assign_intersecting(offs_t addrstart, offs_t addrend, ListBytesPointer base_)  //u8 *base)
        {
            address_map_entry unassigned = null;

            // loop over the adjusted map and assign memory to any blocks we can
            foreach (address_map_entry entry in m_map.entrylist)
            {
                // if we haven't assigned this block yet, see if we have a mapped shared pointer for it
                if (entry.memory == null && entry.share_get != null)
                {
                    string fulltag = entry.devbase.subtag(entry.share_get);
                    var share = m_manager.shares().find(fulltag);
                    if (share != null && share.ptr() != null)
                    {
                        entry.memory = share.ptr();
                        emumem_global.VPRINTF("memory range {0:x8}-{1:x8} -> shared_ptr '{2}' [{3}]\n", entry.addrstart, entry.addrend, entry.share_get, entry.memory);
                    }
                    else
                    {
                        emumem_global.VPRINTF("memory range {0:x8}-{1:x8} -> shared_ptr '{2}' but not found\n", entry.addrstart, entry.addrend, entry.share_get);
                    }
                }

                // otherwise, look for a match in this block
                if (entry.memory == null && entry.addrstart >= addrstart && entry.addrend <= addrend)
                {
                    entry.memory = new ListBytesPointer(base_, (int)m_config.addr2byte(entry.addrstart - addrstart));  //entry.m_memory = base + m_config.addr2byte(entry.m_addrstart - addrstart);
                    emumem_global.VPRINTF("memory range {0:x8}-{1:x8} -> found in block from {2:x8}-{3:x8} [{4}]\n", entry.addrstart, entry.addrend, addrstart, addrend, entry.memory);
                }

                // if we're the first match on a shared pointer, assign it now
                if (entry.memory != null && entry.share_get != null)
                {
                    string fulltag = entry.devbase.subtag(entry.share_get);
                    var share = m_manager.shares().find(fulltag);
                    if (share != null && share.ptr() == null)
                    {
                        share.set_ptr(entry.memory);
                        emumem_global.VPRINTF("setting shared_ptr '{0}' = {1}\n", entry.share_get, entry.memory);
                    }
                }

                // keep track of the first unassigned entry
                if (entry.memory == null && unassigned == null && needs_backing_store(entry))
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
        ListBytesPointer m_data;  //u8 *                    m_data;                 // pointer to the data for this block
        //std::vector<u8>         m_allocated;            // pointer to the actually allocated block


        // construction/destruction
        //-------------------------------------------------
        //  memory_block - constructor
        //-------------------------------------------------
        public memory_block(address_space space, offs_t addrstart, offs_t addrend, ListBytesPointer memory = null)  //, void *memory = NULL)
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
                    m_data = new ListBytesPointer(new RawBuffer(length));
                }
                else
                {
                    //m_allocated.resize(length + 0xfff);
                    //memset(&m_allocated[0], 0, length + 0xfff);
                    //m_data = reinterpret_cast<u8 *>((reinterpret_cast<uintptr_t>(&m_allocated[0]) + 0xfff) & ~0xfff);
                    m_data = new ListBytesPointer(new RawBuffer(length + 0xfff), (0 + 0xfff) & ~0xfff);  // ???
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
        public ListBytesPointer data() { return m_data; }  //u8 *data() const { return m_data; }

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


        public delegate void alloc_notifier(ListBytesPointer base_);


        // internal state
        running_machine m_machine;              // need the machine to free our memory
        std.vector<ListBytesPointerRef> m_entries = new std.vector<ListBytesPointerRef>();  //std::vector<u8 *>       m_entries;              // the entries
        bool m_anonymous;            // are we anonymous or explicit?
        offs_t m_addrstart;            // start offset
        offs_t m_addrend;              // end offset
        int m_curentry;             // current entry
        string m_name;                 // friendly name for this bank
        string m_tag;                  // tag for this bank
        std.vector<bank_reference> m_reflist = new std.vector<bank_reference>();  // std::vector<std::unique_ptr<bank_reference>> m_reflist;     // list of address spaces referencing this bank
        std.vector<alloc_notifier> m_alloc_notifier = new std.vector<alloc_notifier>();  //std::vector<std::function<void (void *)>> m_alloc_notifier; // list of notifier targets when allocating


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
        public ListBytesPointerRef base_() { return m_entries.empty() ? null : m_entries[m_curentry]; }
        public string tag() { return m_tag; }
        //const char *name() const { return m_name; }

        public string uuid { get { return m_uuid; } }


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
        public void set_base(ListBytesPointer base_)  // (void *base)
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

            m_entries[m_curentry] = new ListBytesPointerRef();
            m_entries[m_curentry].m_listPtr = base_;  //reinterpret_cast<u8 *>(base_);

            foreach (var cb in m_alloc_notifier)
                cb(base_);

            m_alloc_notifier.clear();
        }


        // configure and set entries
        //-------------------------------------------------
        //  configure_entry - configure an entry
        //-------------------------------------------------
        public void configure_entry(int entrynum, ListBytesPointer base_)  // void *base_)
        {
            // must be positive
            if (entrynum < 0)
                throw new emu_fatalerror("memory_bank::configure_entry called with out-of-range entry {0}", entrynum);

            // if we haven't allocated this many entries yet, expand our array
            if (entrynum >= m_entries.size())
                m_entries.resize(entrynum+1);

            // set the entry
            m_entries[entrynum] = new ListBytesPointerRef(base_);  // reinterpret_cast<u8 *>(base_);
        }

        //-------------------------------------------------
        //  configure_entries - configure multiple entries
        //-------------------------------------------------
        public void configure_entries(int startentry, int numentries, ListBytesPointer base_, offs_t stride)  // void *base_, offs_t stride)
        {
            if (startentry + numentries >= (int)m_entries.size())
            {
                //m_entries.resize(startentry + numentries+1);
                m_entries.clear();
                for (int i = 0; i < startentry + numentries+1; i++)
                    m_entries.Add(new ListBytesPointerRef());
            }

            // fill in the requested bank entries
            for (int entrynum = 0; entrynum < numentries; entrynum ++)
                m_entries[entrynum + startentry].m_listPtr = new ListBytesPointer(base_, (int)(entrynum * (int)stride));  //reinterpret_cast<u8 *>(base_) +  entrynum * stride ;
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
        public void add_notifier(alloc_notifier cb)  //void add_notifier(std::function<void (void *)> cb)
        {
            m_alloc_notifier.emplace_back(cb);
        }
    }


    // ======================> memory_share
    // a memory share contains information about shared memory region
    public class memory_share
    {
        // internal state
        ListBytesPointer m_ptr;  //void *                  m_ptr;                  // pointer to the memory backing the region
        u64 m_bytes;                // size of the shared region in bytes
        endianness_t m_endianness;           // endianness of the memory
        u8 m_bitwidth;             // width of the shared region in bits
        u8 m_bytewidth;            // width in bytes, rounded up to a power of 2


        // construction/destruction
        public memory_share(u8 width, /*size_t*/ u64 bytes, endianness_t endianness, ListBytesPointer ptr = null)  //void *ptr = nullptr)
        {
            m_ptr = ptr;  //m_ptr(ptr),
            m_bytes = bytes;
            m_endianness = endianness;
            m_bitwidth = width;
            m_bytewidth = (width <= 8 ? (byte)1 : width <= 16 ? (byte)2 : width <= 32 ? (byte)4 : (byte)8);
        }

        // getters
        public ListBytesPointer ptr() { if (this == null) return null; return m_ptr; }  //void *ptr() const { return m_ptr; }
        public u64 bytes() { return m_bytes; }
        public endianness_t endianness() { return m_endianness; }
        public u8 bitwidth() { return m_bitwidth; }
        public u8 bytewidth() { return m_bytewidth; }

        // setters
        public void set_ptr(ListBytesPointer ptr) { m_ptr = ptr; }  //void set_ptr(void *ptr) { m_ptr = ptr; }
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
        public std.vector<u8> base_() { return (this != null) ? m_buffer : null; }  //u8 *base() { return (m_buffer.size() > 0) ? &m_buffer[0] : nullptr; }
        //u8 *end() { return base() + m_buffer.size(); }
        public u32 bytes() { return (u32)m_buffer.size(); }
        //const char *name() const { return m_name; }

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
        //template<int Width, int AddrShift, endianness_t Endian> friend class address_space_specific;
        //friend memory_region::memory_region(running_machine &machine, const char *name, u32 length, u8 width, endianness_t endian);


        // internal state
        running_machine m_machine;              // reference to the machine
        bool m_initialized;          // have we completed initialization?

        std.vector<memory_block> m_blocklist = new std.vector<memory_block>();  //std::vector<std::unique_ptr<memory_block>>   m_blocklist;            // head of the list of memory blocks

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
            if (!machine().options().log() && !machine().options().oslog() && !((machine().debug_flags_get & machine_global.DEBUG_FLAG_ENABLED) == machine_global.DEBUG_FLAG_ENABLED))
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

        public bool initialized { get { return m_initialized; } }
        public std.vector<memory_block> blocklist { get { return m_blocklist; } }


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
                temptag = string_format("anon_{0}", bank.get().uuid);  // %p
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


    // ======================> address_space_specific
    // this is a derived class of address_space with specific width, endianness, and table size
    //template<int Width, int AddrShift, endianness_t Endian>
    class address_space_specific : address_space
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using NativeType = uX;
        //using this_type = address_space_specific<Width, AddrShift, Endian>;


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
        protected override void install_ram_generic(offs_t addrstart, offs_t addrend, offs_t addrmirror, read_or_write readorwrite, ListBytesPointer baseptr)  //void *baseptr)
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
                if (bank.base_() == null || bank.base_().m_listPtr == null)
                {
                    ListBytesPointer backing = find_backing_memory(addrstart, addrend);  //void *backing = find_backing_memory(addrstart, addrend);
                    if (backing != null)
                        bank.set_base(backing);
                }

                // if we still don't have a pointer, and we're past the initialization phase, allocate a new block
                if (bank.base_() == null && m_manager.initialized)
                {
                    if (m_manager.machine().phase() >= machine_phase.RESET)
                        fatalerror("Attempted to call install_ram_generic() after initialization time without a baseptr!\n");

                    var block = new memory_block(this, addrstart, addrend);
                    bank.set_base(block.get().data());
                    m_manager.blocklist.push_back(block);
                }

                var hand_r = new handler_entry_read_memory(Width, AddrShift, (int)Endian, this);
                if (bank.base_() != null)
                {
                    hand_r.set_base(bank.base_());
                }
                else
                {
                    delayed_ref(hand_r);
                    bank.add_notifier((ListBytesPointer base_) => { hand_r.set_base(new ListBytesPointerRef(base_)); delayed_unref(hand_r); });
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
                    ListBytesPointer backing = find_backing_memory(addrstart, addrend);  //void *backing = find_backing_memory(addrstart, addrend);
                    if (backing != null)
                        bank.set_base(backing);
                }

                // if we still don't have a pointer, and we're past the initialization phase, allocate a new block
                if (bank.base_() == null && m_manager.initialized)
                {
                    if (m_manager.machine().phase() >= machine_phase.RESET)
                        fatalerror("Attempted to call install_ram_generic() after initialization time without a baseptr!\n");

                    var block = new memory_block(this, address_to_byte(addrstart), address_to_byte_end(addrend));
                    bank.set_base(block.get().data());
                    m_manager.blocklist.push_back(block);
                }

                var hand_w = new handler_entry_write_memory(Width, AddrShift, (int)Endian, this);
                if (bank.base_() != null)
                {
                    hand_w.set_base(bank.base_());
                }
                else
                {
                    delayed_ref(hand_w);
                    bank.add_notifier((ListBytesPointer base_) => { hand_w.set_base(new ListBytesPointerRef(base_)); delayed_unref(hand_w); });
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

                var hand_r = new handler_entry_read_memory_bank(Width, AddrShift, (int)Endian, this, bank);
                hand_r.set_address_info(nstart, nmask);
                m_root_read.populate(nstart, nend, nmirror, hand_r);
            }

            // map the write bank
            if (wtag != "")
            {
                string fulltag = device().siblingtag(wtag);
                memory_bank bank = bank_find_or_allocate(fulltag.c_str(), addrstart, addrend, addrmirror, read_or_write.WRITE);

                var hand_w = new handler_entry_write_memory_bank(Width, AddrShift, (int)Endian, this, bank);
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
                var hand_r = new handler_entry_read_ioport(Width, AddrShift, (int)Endian, this, port);
                m_root_read.populate(nstart, nend, nmirror, hand_r);
            }

            if (wtag != "")
            {
                // find the port
                ioport_port port = device().owner().ioport(wtag);
                if (port == null)
                    fatalerror("Attempted to map non-existent port '{0}' for write in space {1} of device '{2}'\n", wtag.c_str(), name(), device().tag());

                // map the range and set the ioport
                var hand_w = new handler_entry_write_ioport(Width, AddrShift, (int)Endian, this, port);
                m_root_write.populate(nstart, nend, nmirror, hand_w);
            }

            invalidate_caches(rtag != "" ? wtag != "" ? read_or_write.READWRITE : read_or_write.READ : read_or_write.WRITE);
        }


        protected override void install_device_delegate(offs_t addrstart, offs_t addrend, device_t device, address_map_constructor map, u64 unitmask = 0, int cswidth = 0) { throw new emu_unimplemented(); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8_delegate rhandler, u64 unitmask = 0, int cswidth = 0)
        {
            install_read_handler_helper(0, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, rhandler);
        }

        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0)
        {
            install_write_handler_helper(0, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, whandler);
        }

        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8_delegate rhandler, write8_delegate whandler, u64 unitmask = 0, int cswidth = 0) { throw new emu_unimplemented(); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { throw new emu_unimplemented(); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16_delegate whandler, u64 unitmask = 0, int cswidth = 0) { throw new emu_unimplemented(); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16_delegate rhandler, write16_delegate whandler, u64 unitmask = 0, int cswidth = 0) { throw new emu_unimplemented(); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { throw new emu_unimplemented(); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32_delegate whandler, u64 unitmask = 0, int cswidth = 0) { throw new emu_unimplemented(); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32_delegate rhandler, write32_delegate whandler, u64 unitmask = 0, int cswidth = 0) { throw new emu_unimplemented(); }
        protected override void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64_delegate rhandler, u64 unitmask = 0, int cswidth = 0) { throw new emu_unimplemented(); }
        protected override void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64_delegate whandler, u64 unitmask = 0, int cswidth = 0) { throw new emu_unimplemented(); }
        protected override void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64_delegate rhandler, write64_delegate whandler, u64 unitmask = 0, int cswidth = 0) { throw new emu_unimplemented(); }

        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8m_delegate rhandler, write8m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16m_delegate rhandler, write16m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32m_delegate rhandler, write32m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64m_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64m_delegate rhandler, write64m_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;

        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8s_delegate rhandler, write8s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16s_delegate rhandler, write16s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32s_delegate rhandler, write32s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64s_delegate rhandler, write64s_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;

        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8sm_delegate rhandler, write8sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16sm_delegate rhandler, write16sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32sm_delegate rhandler, write32sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64sm_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64sm_delegate rhandler, write64sm_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;

        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8mo_delegate rhandler, write8mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16mo_delegate rhandler, write16mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32mo_delegate rhandler, write32mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64mo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64mo_delegate rhandler, write64mo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;

        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read8smo_delegate rhandler, write8smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read16smo_delegate rhandler, write16smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read32smo_delegate rhandler, write32smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_read_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64smo_delegate rhandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_write_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;
        //void install_readwrite_handler(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, read64smo_delegate rhandler, write64smo_delegate whandler, u64 unitmask = 0, int cswidth = 0) override;


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


            m_unmap_r = new handler_entry_read_unmapped(Width, AddrShift, (int)Endian, this);
            m_unmap_w = new handler_entry_write_unmapped(Width, AddrShift, (int)Endian, this);
            m_nop_r = new handler_entry_read_nop(Width, AddrShift, (int)Endian, this);
            m_nop_w = new handler_entry_write_nop(Width, AddrShift, (int)Endian, this);

            handler_entry.range r = new handler_entry.range() { start = 0, end = 0xffffffff >> (32 - address_width) };

            switch (address_width)
            {
                case  1: m_root_read = new handler_entry_read_dispatch( Math.Max(1, Width), Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( Math.Max(1, Width), Width, AddrShift, (int)Endian, this, r, null); break;
                case  2: m_root_read = new handler_entry_read_dispatch( Math.Max(2, Width), Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( Math.Max(2, Width), Width, AddrShift, (int)Endian, this, r, null); break;
                case  3: m_root_read = new handler_entry_read_dispatch( Math.Max(3, Width), Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( Math.Max(3, Width), Width, AddrShift, (int)Endian, this, r, null); break;
                case  4: m_root_read = new handler_entry_read_dispatch( 4, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( 4, Width, AddrShift, (int)Endian, this, r, null); break;
                case  5: m_root_read = new handler_entry_read_dispatch( 5, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( 5, Width, AddrShift, (int)Endian, this, r, null); break;
                case  6: m_root_read = new handler_entry_read_dispatch( 6, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( 6, Width, AddrShift, (int)Endian, this, r, null); break;
                case  7: m_root_read = new handler_entry_read_dispatch( 7, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( 7, Width, AddrShift, (int)Endian, this, r, null); break;
                case  8: m_root_read = new handler_entry_read_dispatch( 8, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( 8, Width, AddrShift, (int)Endian, this, r, null); break;
                case  9: m_root_read = new handler_entry_read_dispatch( 9, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch( 9, Width, AddrShift, (int)Endian, this, r, null); break;
                case 10: m_root_read = new handler_entry_read_dispatch(10, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(10, Width, AddrShift, (int)Endian, this, r, null); break;
                case 11: m_root_read = new handler_entry_read_dispatch(11, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(11, Width, AddrShift, (int)Endian, this, r, null); break;
                case 12: m_root_read = new handler_entry_read_dispatch(12, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(12, Width, AddrShift, (int)Endian, this, r, null); break;
                case 13: m_root_read = new handler_entry_read_dispatch(13, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(13, Width, AddrShift, (int)Endian, this, r, null); break;
                case 14: m_root_read = new handler_entry_read_dispatch(14, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(14, Width, AddrShift, (int)Endian, this, r, null); break;
                case 15: m_root_read = new handler_entry_read_dispatch(15, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(15, Width, AddrShift, (int)Endian, this, r, null); break;
                case 16: m_root_read = new handler_entry_read_dispatch(16, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(16, Width, AddrShift, (int)Endian, this, r, null); break;
                case 17: m_root_read = new handler_entry_read_dispatch(17, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(17, Width, AddrShift, (int)Endian, this, r, null); break;
                case 18: m_root_read = new handler_entry_read_dispatch(18, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(18, Width, AddrShift, (int)Endian, this, r, null); break;
                case 19: m_root_read = new handler_entry_read_dispatch(19, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(19, Width, AddrShift, (int)Endian, this, r, null); break;
                case 20: m_root_read = new handler_entry_read_dispatch(20, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(20, Width, AddrShift, (int)Endian, this, r, null); break;
                case 21: m_root_read = new handler_entry_read_dispatch(21, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(21, Width, AddrShift, (int)Endian, this, r, null); break;
                case 22: m_root_read = new handler_entry_read_dispatch(22, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(22, Width, AddrShift, (int)Endian, this, r, null); break;
                case 23: m_root_read = new handler_entry_read_dispatch(23, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(23, Width, AddrShift, (int)Endian, this, r, null); break;
                case 24: m_root_read = new handler_entry_read_dispatch(24, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(24, Width, AddrShift, (int)Endian, this, r, null); break;
                case 25: m_root_read = new handler_entry_read_dispatch(25, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(25, Width, AddrShift, (int)Endian, this, r, null); break;
                case 26: m_root_read = new handler_entry_read_dispatch(26, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(26, Width, AddrShift, (int)Endian, this, r, null); break;
                case 27: m_root_read = new handler_entry_read_dispatch(27, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(27, Width, AddrShift, (int)Endian, this, r, null); break;
                case 28: m_root_read = new handler_entry_read_dispatch(28, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(28, Width, AddrShift, (int)Endian, this, r, null); break;
                case 29: m_root_read = new handler_entry_read_dispatch(29, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(29, Width, AddrShift, (int)Endian, this, r, null); break;
                case 30: m_root_read = new handler_entry_read_dispatch(30, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(30, Width, AddrShift, (int)Endian, this, r, null); break;
                case 31: m_root_read = new handler_entry_read_dispatch(31, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(31, Width, AddrShift, (int)Endian, this, r, null); break;
                case 32: m_root_read = new handler_entry_read_dispatch(32, Width, AddrShift, (int)Endian, this, r, null); m_root_write = new handler_entry_write_dispatch(32, Width, AddrShift, (int)Endian, this, r, null); break;
                default: fatalerror("Unhandled address bus width {0}\n", address_width); break;
            }
        }


        protected override memory_access_cache create_cache()
        {
            return new memory_access_cache(Width, AddrShift, (int)Endian, this, m_root_read, m_root_write);
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
        //NativeType read_native(offs_t offset, NativeType mask)
        //{
        //    g_profiler.start(PROFILER_MEMREAD);
        //
        //    uX result = m_root_read->read(offset, mask);
        //
        //    g_profiler.stop();
        //    return result;
        //}

        byte read_native8(offs_t offset, byte mask)
        {
            byte result = 0;
            Type NativeType = result.GetType();


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMREAD);

            throw new emu_unimplemented();
#if false
            result = m_root_read.read(offset, mask);
#endif

            profiler_global.g_profiler.stop();

            return result;
        }

        UInt16 read_native16(offs_t offset, UInt16 mask)
        {
            UInt16 result = 0;
            Type NativeType = result.GetType();


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMREAD);

            throw new emu_unimplemented();
#if false
            result = m_root_read.read(offset, mask);
#endif

            profiler_global.g_profiler.stop();

            return result;
        }

        UInt32 read_native32(offs_t offset, UInt32 mask)
        {
            UInt32 result = 0;
            Type NativeType = result.GetType();


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMREAD);

            throw new emu_unimplemented();
#if false
            result = m_root_read.read(offset, mask);
#endif

            profiler_global.g_profiler.stop();

            return result;
        }

        UInt64 read_native64(offs_t offset, UInt64 mask)
        {
            UInt64 result = 0;
            Type NativeType = result.GetType();


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMREAD);

            throw new emu_unimplemented();
#if false
            result = m_root_read.read(offset, mask);
#endif

            profiler_global.g_profiler.stop();

            return result;
        }


        // mask-less native read
        //NativeType read_native(offs_t offset)
        //{
        //    g_profiler.start(PROFILER_MEMREAD);
        //
        //    uX result = m_root_read->read(offset, uX(0xffffffffffffffffU));
        //
        //    g_profiler.stop();
        //    return result;
        //}

        byte read_native8(offs_t offset)
        {
            byte result = 0;
            Type NativeType = result.GetType();


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMREAD);

            result = m_root_read.read(offset, byte.MaxValue);

            profiler_global.g_profiler.stop();
            return result;
        }

        UInt16 read_native16(offs_t offset)
        {
            UInt16 result = 0;
            Type NativeType = result.GetType();


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMREAD);

            throw new emu_unimplemented();
#if false
            result = m_root_read.read(offset, UInt16.MaxValue);
#endif

            profiler_global.g_profiler.stop();
            return result;
        }

        UInt32 read_native32(offs_t offset)
        {
            UInt32 result = 0;
            Type NativeType = result.GetType();


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMREAD);

            throw new emu_unimplemented();
#if false
            result = m_root_read.read(offset, UInt32.MaxValue);
#endif

            profiler_global.g_profiler.stop();
            return result;
        }

        UInt64 read_native64(offs_t offset)
        {
            UInt64 result = 0;
            Type NativeType = result.GetType();


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMREAD);

            throw new emu_unimplemented();
#if false
            result = m_root_read.read(offset, UInt64.MaxValue);
#endif

            profiler_global.g_profiler.stop();
            return result;
        }


        // native write
        //void write_native(offs_t offset, NativeType data, NativeType mask)
        //{
        //    g_profiler.start(PROFILER_MEMWRITE);
        //
        //    m_root_write->write(offset, data, mask);
        //
        //    g_profiler.stop();
        //}

        void write_native8(offs_t offset, byte data, byte mask)
        {
            Type NativeType = typeof(byte);


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMWRITE);

            throw new emu_unimplemented();
#if false
            m_root_write.write(offset, data, mask);
#endif

            profiler_global.g_profiler.stop();
        }

        void write_native16(offs_t offset, UInt16 data, UInt16 mask)
        {
            Type NativeType = typeof(UInt16);


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMWRITE);

            throw new emu_unimplemented();
#if false
            m_root_write.write(offset, data, mask);
#endif

            profiler_global.g_profiler.stop();
        }

        void write_native32(offs_t offset, UInt32 data, UInt32 mask)
        {
            Type NativeType = typeof(UInt32);


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMWRITE);

            throw new emu_unimplemented();
#if false
            m_root_write.write(offset, data, mask);
#endif

            profiler_global.g_profiler.stop();
        }

        void write_native64(offs_t offset, UInt64 data, UInt64 mask)
        {
            Type NativeType = typeof(UInt64);


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMWRITE);

            throw new emu_unimplemented();
#if false
            m_root_write.write(offset, data, mask);
#endif

            profiler_global.g_profiler.stop();
        }


        // mask-less native write
        //void write_native(offs_t offset, NativeType data)
        //{
        //    g_profiler.start(PROFILER_MEMWRITE);
        //
        //    m_root_write->write(offset, data, uX(0xffffffffffffffffU));
        //
        //    g_profiler.stop();
        //}

        // mask-less native write
        void write_native8(offs_t offset, byte data)
        {
            Type NativeType = typeof(byte);


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMWRITE);

            m_root_write.write(offset, data, byte.MaxValue);

            profiler_global.g_profiler.stop();
        }

        void write_native16(offs_t offset, UInt16 data)
        {
            Type NativeType = typeof(UInt16);


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMWRITE);

            throw new emu_unimplemented();
#if false
            m_root_write.write(offset, data, UInt16.MaxValue);
#endif

            profiler_global.g_profiler.stop();
        }

        void write_native32(offs_t offset, UInt32 data)
        {
            Type NativeType = typeof(UInt32);


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMWRITE);

            throw new emu_unimplemented();
#if false
            m_root_write.write(offset, data, UInt32.MaxValue);
#endif

            profiler_global.g_profiler.stop();
        }

        void write_native64(offs_t offset, UInt64 data)
        {
            Type NativeType = typeof(UInt64);


            profiler_global.g_profiler.start(profile_type.PROFILER_MEMWRITE);

            throw new emu_unimplemented();
#if false
            m_root_write.write(offset, data, UInt64.MaxValue);
#endif

            profiler_global.g_profiler.stop();
        }


        // virtual access to these functions
        public override u8 read_byte(offs_t address) { address &= addrmask(); return Width == 0 ? read_native8(address & ~NATIVE_MASK) : memory_read_generic8((int)Width, AddrShift, (int)Endian, 0, true, (offs_t offset, u8 mask) => { return read_native8(offset, mask); }, address, 0xff); }
        public override u16 read_word(offs_t address) { address &= addrmask(); return Width == 1 ? read_native16(address & ~NATIVE_MASK) : memory_read_generic16((int)Width, AddrShift, (int)Endian, 1, true, (offs_t offset, u16 mask) => { return read_native16(offset, mask); }, address, 0xffff); }
        public override u16 read_word(offs_t address, u16 mask) { address &= addrmask(); return memory_read_generic16((int)Width, AddrShift, (int)Endian, 1, true, (offs_t offset, u16 mask2) => { return read_native16(offset, mask2); }, address, mask); }
        public override u16 read_word_unaligned(offs_t address) { address &= addrmask(); return memory_read_generic16((int)Width, AddrShift, (int)Endian, 1, false, (offs_t offset, u16 mask) => { return read_native16(offset, mask); }, address, 0xffff); }
        public override u16 read_word_unaligned(offs_t address, u16 mask) { address &= addrmask(); return memory_read_generic16((int)Width, AddrShift, (int)Endian, 1, false, (offs_t offset, u16 mask2) => { return read_native16(offset, mask2); }, address, mask); }
        public override u32 read_dword(offs_t address) { address &= addrmask(); return Width == 2 ? read_native32(address & ~NATIVE_MASK) : memory_read_generic32((int)Width, AddrShift, (int)Endian, 2, true, (offs_t offset, u32 mask) => { return read_native32(offset, mask); }, address, 0xffffffff); }
        public override u32 read_dword(offs_t address, u32 mask) { address &= addrmask(); return memory_read_generic32((int)Width, AddrShift, (int)Endian, 2, true, (offs_t offset, u32 mask2) => { return read_native32(offset, mask2); }, address, mask); }
        public override u32 read_dword_unaligned(offs_t address) { address &= addrmask(); return memory_read_generic32((int)Width, AddrShift, (int)Endian, 2, false, (offs_t offset, u32 mask) => { return read_native32(offset, mask); }, address, 0xffffffff); }
        public override u32 read_dword_unaligned(offs_t address, u32 mask) { address &= addrmask(); return memory_read_generic32((int)Width, AddrShift, (int)Endian, 2, false, (offs_t offset, u32 mask2) => { return read_native32(offset, mask2); }, address, mask); }
        public override u64 read_qword(offs_t address) { address &= addrmask(); return Width == 3 ? read_native64(address & ~NATIVE_MASK) : memory_read_generic64((int)Width, AddrShift, (int)Endian, 3, true, (offs_t offset, u64 mask) => { return read_native64(offset, mask); }, address, 0xffffffffffffffffU); }
        public override u64 read_qword(offs_t address, u64 mask) { address &= addrmask(); return memory_read_generic64((int)Width, AddrShift, (int)Endian, 3, true, (offs_t offset, u64 mask2) => { return read_native64(offset, mask2); }, address, mask); }
        public override u64 read_qword_unaligned(offs_t address) { address &= addrmask(); return memory_read_generic64((int)Width, AddrShift, (int)Endian, 3, false, (offs_t offset, u64 mask) => { return read_native64(offset, mask); }, address, 0xffffffffffffffffU); }
        public override u64 read_qword_unaligned(offs_t address, u64 mask) { address &= addrmask(); return memory_read_generic64((int)Width, AddrShift, (int)Endian, 3, false, (offs_t offset, u64 mask2) => { return read_native64(offset, mask2); }, address, mask); }

        public override void write_byte(offs_t address, u8 data) { address &= addrmask(); if (Width == 0) write_native8(address & ~NATIVE_MASK, data); else memory_write_generic8((int)Width, AddrShift, (int)Endian, 0, true, (offs_t offset, u8 data2, u8 mask) => { write_native8(offset, data2, mask); }, address, data, 0xff); }
        public override void write_word(offs_t address, u16 data) { address &= addrmask(); if (Width == 1) write_native16(address & ~NATIVE_MASK, data); else memory_write_generic16((int)Width, AddrShift, (int)Endian, 1, true, (offs_t offset, u16 data2, u16 mask) => { write_native16(offset, data2, mask); }, address, data, 0xffff); }
        public override void write_word(offs_t address, u16 data, u16 mask) { address &= addrmask(); memory_write_generic16((int)Width, AddrShift, (int)Endian, 1, true, (offs_t offset, u16 data2, u16 mask2) => { write_native16(offset, data2, mask2); }, address, data, mask); }
        public override void write_word_unaligned(offs_t address, u16 data) { address &= addrmask(); memory_write_generic16((int)Width, AddrShift, (int)Endian, 1, false, (offs_t offset, u16 data2, u16 mask) => { write_native16(offset, data2, mask); }, address, data, 0xffff); }
        public override void write_word_unaligned(offs_t address, u16 data, u16 mask) { address &= addrmask(); memory_write_generic16((int)Width, AddrShift, (int)Endian, 1, false, (offs_t offset, u16 data2, u16 mask2) => { write_native16(offset, data2, mask2); }, address, data, mask); }
        public override void write_dword(offs_t address, u32 data) { address &= addrmask(); if (Width == 2) write_native32(address & ~NATIVE_MASK, data); else memory_write_generic32((int)Width, AddrShift, (int)Endian, 2, true, (offs_t offset, u32 data2, u32 mask) => { write_native32(offset, data2, mask); }, address, data, 0xffffffff); }
        public override void write_dword(offs_t address, u32 data, u32 mask) { address &= addrmask(); memory_write_generic32((int)Width, AddrShift, (int)Endian, 2, true, (offs_t offset, u32 data2, u32 mask2) => { write_native32(offset, data2, mask2); }, address, data, mask); }
        public override void write_dword_unaligned(offs_t address, u32 data) { address &= addrmask(); memory_write_generic32((int)Width, AddrShift, (int)Endian, 2, false, (offs_t offset, u32 data2, u32 mask) => { write_native32(offset, data2, mask); }, address, data, 0xffffffff); }
        public override void write_dword_unaligned(offs_t address, u32 data, u32 mask) { address &= addrmask(); memory_write_generic32((int)Width, AddrShift, (int)Endian, 2, false, (offs_t offset, u32 data2, u32 mask2) => { write_native32(offset, data2, mask2); }, address, data, mask); }
        public override void write_qword(offs_t address, u64 data) { address &= addrmask(); if (Width == 3) write_native64(address & ~NATIVE_MASK, data); else memory_write_generic64((int)Width, AddrShift, (int)Endian, 3, true, (offs_t offset, u64 data2, u64 mask) => { write_native64(offset, data2, mask); }, address, data, 0xffffffffffffffffU); }
        public override void write_qword(offs_t address, u64 data, u64 mask) { address &= addrmask(); memory_write_generic64((int)Width, AddrShift, (int)Endian, 3, true, (offs_t offset, u64 data2, u64 mask2) => { write_native64(offset, data2, mask2); }, address, data, mask); }
        public override void write_qword_unaligned(offs_t address, u64 data) { address &= addrmask(); memory_write_generic64((int)Width, AddrShift, (int)Endian, 3, false, (offs_t offset, u64 data2, u64 mask) => { write_native64(offset, data2, mask); }, address, data, 0xffffffffffffffffU); }
        public override void write_qword_unaligned(offs_t address, u64 data, u64 mask) { address &= addrmask(); memory_write_generic64((int)Width, AddrShift, (int)Endian, 3, false, (offs_t offset, u64 data2, u64 mask2) => { write_native64(offset, data2, mask2); }, address, data, mask); }


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


        //template<int AccessWidth, typename READ> std::enable_if_t<(Width == AccessWidth)>
        void install_read_handler_helper(int AccessWidth, offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
                                         read8_delegate handler_r)  //READ handler_r)
        {
            emumem_global.VPRINTF("address_space::install_read_handler({0}-{1} mask={2} mirror={3}, space width={4}, handler width={5}, {6}, {7})\n",
                     core_i64_hex_format(addrstart, m_addrchars), core_i64_hex_format(addrend, m_addrchars),
                     core_i64_hex_format(addrmask, m_addrchars), core_i64_hex_format(addrmirror, m_addrchars),
                     8 << Width, 8 << AccessWidth,
                     ((read8_delegate)(handler_r.Target)).Method.Name, core_i64_hex_format(unitmask, (byte)(data_width() / 4)));

            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            u64 nunitmask;
            int ncswidth;
            check_optimize_all("install_read_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, out nstart, out nend, out nmask, out nmirror, out nunitmask, out ncswidth);

            var hand_r = new handler_entry_read_delegate(Width, AddrShift, (int)Endian, this, handler_r);
            hand_r.set_address_info(nstart, nmask);
            m_root_read.populate(nstart, nend, nmirror, hand_r);
            invalidate_caches(read_or_write.READ);
        }

#if false
        template<int AccessWidth> std::enable_if_t<(Width > AccessWidth)>
        install_read_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
                                    typename handler_entry_size<AccessWidth>::READ handler_r)
        {
            VPRINTF(("address_space::install_read_handler(%s-%s mask=%s mirror=%s, space width=%d, handler width=%d, %s, %s)\n",
                     core_i64_hex_format(addrstart, m_addrchars), core_i64_hex_format(addrend, m_addrchars),
                     core_i64_hex_format(addrmask, m_addrchars), core_i64_hex_format(addrmirror, m_addrchars),
                     8 << Width, 8 << AccessWidth,
                     handler_r.name(), core_i64_hex_format(unitmask, data_width() / 4)));

            offs_t nstart, nend, nmask, nmirror;
            u64 nunitmask;
            int ncswidth;
            check_optimize_all("install_read_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, nstart, nend, nmask, nmirror, nunitmask, ncswidth);

            auto hand_r = new handler_entry_read_delegate<AccessWidth, -AccessWidth, Endian>(this, handler_r);
            memory_units_descriptor<Width, AddrShift, Endian> descriptor(AccessWidth, Endian, hand_r, nstart, nend, nmask, nunitmask, ncswidth);
            hand_r->set_address_info(descriptor.get_handler_start(), descriptor.get_handler_mask());
            m_root_read->populate_mismatched(nstart, nend, nmirror, descriptor);
            hand_r->unref();
            invalidate_caches(read_or_write::READ);
        }


        template<int AccessWidth> std::enable_if_t<(Width < AccessWidth)>
        install_read_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
                                    typename handler_entry_size<AccessWidth>::READ handler_r)
        {
            fatalerror("install_read_handler: cannot install a %d-wide handler in a %d-wide bus", 8 << AccessWidth, 8 << Width);
        }
#endif

        //template<int AccessWidth, typename WRITE> std::enable_if_t<(Width == AccessWidth)>
        void install_write_handler_helper(int AccessWidth, offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
                                          write8_delegate handler_w)  // WRITE handler_w)
        {
            emumem_global.VPRINTF("address_space::install_write_handler({0}-{1} mask={2} mirror={3}, space width={4}, handler width={5}, {6}, {7})\n",
                     core_i64_hex_format(addrstart, m_addrchars), core_i64_hex_format(addrend, m_addrchars),
                     core_i64_hex_format(addrmask, m_addrchars), core_i64_hex_format(addrmirror, m_addrchars),
                     8 << Width, 8 << AccessWidth,
                     ((write8_delegate)(handler_w.Target)).Method.Name, core_i64_hex_format(unitmask, (byte)(data_width() / 4)));

            offs_t nstart;
            offs_t nend;
            offs_t nmask;
            offs_t nmirror;
            u64 nunitmask;
            int ncswidth;
            check_optimize_all("install_write_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, out nstart, out nend, out nmask, out nmirror, out nunitmask, out ncswidth);

            var hand_w = new handler_entry_write_delegate(Width, AddrShift, (int)Endian, this, handler_w);
            hand_w.set_address_info(nstart, nmask);
            m_root_write.populate(nstart, nend, nmirror, hand_w);
            invalidate_caches(read_or_write.WRITE);
        }

#if false
        template<int AccessWidth> std::enable_if_t<(Width > AccessWidth)>
        install_write_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
                                     typename handler_entry_size<AccessWidth>::WRITE handler_w)
        {
            VPRINTF(("address_space::install_write_handler(%s-%s mask=%s mirror=%s, space width=%d, handler width=%d, %s, %s)\n",
                     core_i64_hex_format(addrstart, m_addrchars), core_i64_hex_format(addrend, m_addrchars),
                     core_i64_hex_format(addrmask, m_addrchars), core_i64_hex_format(addrmirror, m_addrchars),
                     8 << Width, 8 << AccessWidth,
                     handler_w.name(), core_i64_hex_format(unitmask, data_width() / 4)));

            offs_t nstart, nend, nmask, nmirror;
            u64 nunitmask;
            int ncswidth;
            check_optimize_all("install_write_handler", 8 << AccessWidth, addrstart, addrend, addrmask, addrmirror, addrselect, unitmask, cswidth, nstart, nend, nmask, nmirror, nunitmask, ncswidth);

            auto hand_w = new handler_entry_write_delegate<AccessWidth, -AccessWidth, Endian>(this, handler_w);
            memory_units_descriptor<Width, AddrShift, Endian> descriptor(AccessWidth, Endian, hand_w, nstart, nend, nmask, nunitmask, ncswidth);
            hand_w->set_address_info(descriptor.get_handler_start(), descriptor.get_handler_mask());
            m_root_write->populate_mismatched(nstart, nend, nmirror, descriptor);
            hand_w->unref();
            invalidate_caches(read_or_write::WRITE);
        }


        template<int AccessWidth> std::enable_if_t<(Width < AccessWidth)>
        install_write_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
                                     typename handler_entry_size<AccessWidth>::WRITE handler_w)
        {
            fatalerror("install_write_handler: cannot install a %d-wide handler in a %d-wide bus", 8 << AccessWidth, 8 << Width);
        }



        template<int AccessWidth> std::enable_if_t<(Width == AccessWidth)>
        install_readwrite_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
                                         typename handler_entry_size<AccessWidth>::READ  handler_r,
                                         typename handler_entry_size<AccessWidth>::WRITE handler_w)
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

            auto hand_r = new handler_entry_read_delegate <Width, AddrShift, Endian>(this, handler_r);
            hand_r->set_address_info(nstart, nmask);
            m_root_read ->populate(nstart, nend, nmirror, hand_r);

            auto hand_w = new handler_entry_write_delegate<Width, AddrShift, Endian>(this, handler_w);
            hand_w->set_address_info(nstart, nmask);
            m_root_write->populate(nstart, nend, nmirror, hand_w);

            invalidate_caches(read_or_write::READWRITE);
        }

        template<int AccessWidth> std::enable_if_t<(Width > AccessWidth)>
        install_readwrite_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
                                         typename handler_entry_size<AccessWidth>::READ  handler_r,
                                         typename handler_entry_size<AccessWidth>::WRITE handler_w)
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

            auto hand_r = new handler_entry_read_delegate <AccessWidth, -AccessWidth, Endian>(this, handler_r);
            memory_units_descriptor<Width, AddrShift, Endian> descriptor(AccessWidth, Endian, hand_r, nstart, nend, nmask, nunitmask, ncswidth);
            hand_r->set_address_info(descriptor.get_handler_start(), descriptor.get_handler_mask());
            m_root_read ->populate_mismatched(nstart, nend, nmirror, descriptor);
            hand_r->unref();

            auto hand_w = new handler_entry_write_delegate<AccessWidth, -AccessWidth, Endian>(this, handler_w);
            descriptor.set_subunit_handler(hand_w);
            hand_w->set_address_info(descriptor.get_handler_start(), descriptor.get_handler_mask());
            m_root_write->populate_mismatched(nstart, nend, nmirror, descriptor);
            hand_w->unref();

            invalidate_caches(read_or_write::READWRITE);
        }


        template<int AccessWidth> std::enable_if_t<(Width < AccessWidth)>
        install_readwrite_handler_helper(offs_t addrstart, offs_t addrend, offs_t addrmask, offs_t addrmirror, offs_t addrselect, u64 unitmask, int cswidth,
                                         typename handler_entry_size<AccessWidth>::READ  handler_r,
                                         typename handler_entry_size<AccessWidth>::WRITE handler_w)
        {
            fatalerror("install_readwrite_handler: cannot install a %d-wide handler in a %d-wide bus", 8 << AccessWidth, 8 << Width);
        }
#endif
    }
}
