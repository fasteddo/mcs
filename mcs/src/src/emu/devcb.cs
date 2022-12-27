// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_read_line = mame.devcb_read<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_read_line = devcb_read<int, 1U>;
using devcb_value = mame.FlexPrim;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write32 = mame.devcb_write<mame.Type_constant_u32>;  //using devcb_write32 = devcb_write<u32>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using ioport_value = System.UInt32;  //typedef u32 ioport_value;
using offs_t = System.UInt32;  //using offs_t = u32;
using s8 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using size_t = System.UInt64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt32;

using static mame.cpp_global;
using static mame.device_global;


namespace mame
{
    //**************************************************************************
    //  DELEGATE TYPES
    //**************************************************************************

    public delegate int read_line_delegate();  //typedef device_delegate<int ()> read_line_delegate;
    public delegate void write_line_delegate(int param);  //typedef device_delegate<void (int)> write_line_delegate;


    //namespace emu::detail {
    //template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<read_line_delegate, std::remove_reference_t<T> > > > { using type = read_line_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    //template <typename T> struct rw_delegate_type<T, void_t<rw_device_class_t<write_line_delegate, std::remove_reference_t<T> > > > { using type = write_line_delegate; using device_class = rw_device_class_t<type, std::remove_reference_t<T> >; };
    //} // namespace emu::detail


    public interface devcb_value_const { devcb_value value { get; } }

    public class devcb_value_const_unsigned_MaxValue<Type_T> : devcb_value_const where Type_T : Type_constant, new() { static readonly Type T = new Type_T().value;  public devcb_value value { get { return new devcb_value(devcb_value.make_unsigned(T), u64.MaxValue); } } }
    public class devcb_value_const_unsigned_1<Type_T> : devcb_value_const where Type_T : Type_constant, new() { static readonly Type T = new Type_T().value;  public devcb_value value { get { return new devcb_value(devcb_value.make_unsigned(T), 1U); } } }


    public interface Type_constant { Type value { get; } }

    public class Type_constant_u8 : Type_constant { public Type value { get { return typeof(u8); } } }
    public class Type_constant_u16 : Type_constant { public Type value { get { return typeof(u16); } } }
    public class Type_constant_s32 : Type_constant { public Type value { get { return typeof(s32); } } }
    public class Type_constant_u32 : Type_constant { public Type value { get { return typeof(u32); } } }
    public class Type_constant_ioport_value : Type_constant { public Type value { get { return typeof(ioport_value); } } }


    public interface IResolve
    {
        void resolve();
    }


    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    /// \brief Base callback helper
    ///
    /// Provides utilities for supporting multiple read/write/transform
    /// signatures, and the base exclusive-or/mask transform methods.
    public abstract class devcb_base
    {
        device_t m_owner;


        protected abstract void validity_check(validity_checker valid);


        // This is in C++17 but not C++14
        //template <typename... T> struct void_wrapper { using type = void; };
        //template <typename... T> using void_t = typename void_wrapper<T...>::type;

        // Intermediate is larger of input and output, mask is forced to unsigned
        //template <typename T, typename U, typename Enable = void> struct intermediate;
        //template <typename T, typename U> struct intermediate<T, U, std::enable_if_t<sizeof(T) >= sizeof(U)> > { using type = T; };
        //template <typename T, typename U> struct intermediate<T, U, std::enable_if_t<sizeof(T) < sizeof(U)> > { using type = U; };
        //template <typename T, typename U> using intermediate_t = typename intermediate<T, U>::type;
        //template <typename T, typename U> using mask_t = std::make_unsigned_t<intermediate_t<T, U> >;

        // Detecting candidates for transform functions
        //template <typename Input, typename Result, typename Func> using is_transform_form3 = std::is_invocable_r<Result, Func, offs_t &, Input, std::make_unsigned_t<Input> &>;
        //template <typename Input, typename Result, typename Func> using is_transform_form4 = std::is_invocable_r<Result, Func, offs_t &, Input>;
        //template <typename Input, typename Result, typename Func> using is_transform_form6 = std::is_invocable_r<Result, Func, Input>;
        //template <typename Input, typename Result, typename Func> using is_transform = std::bool_constant<is_transform_form3<Input, Result, Func>::value || is_transform_form4<Input, Result, Func>::value || is_transform_form6<Input, Result, Func>::value>;

        // Determining the result type of a transform function
        //template <typename Input, typename Result, typename Func, typename Enable = void> struct transform_result;
        //template <typename Input, typename Result, typename Func> struct transform_result<Input, Result, Func, std::enable_if_t<is_transform_form3<Input, Result, Func>::value> > { using type = std::invoke_result_t<Func, offs_t &, Input, std::make_unsigned_t<Input> &>; };
        //template <typename Input, typename Result, typename Func> struct transform_result<Input, Result, Func, std::enable_if_t<is_transform_form4<Input, Result, Func>::value> > { using type = std::invoke_result_t<Func, offs_t &, Input>; };
        //template <typename Input, typename Result, typename Func> struct transform_result<Input, Result, Func, std::enable_if_t<is_transform_form6<Input, Result, Func>::value> > { using type = std::invoke_result_t<Func, Input>; };
        //template <typename Input, typename Result, typename Func> using transform_result_t = typename transform_result<Input, Result, Func>::type;

        // Mapping method types to delegate types
        //template <typename T> using delegate_type_t = emu::detail::rw_delegate_type_t<T>;
        //template <typename T> using delegate_device_class_t = emu::detail::rw_delegate_device_class_t<T>;

        // Invoking transform callbacks
        //template <typename Input, typename Result, typename T> static std::enable_if_t<is_transform_form3<Input, Result, T>::value, mask_t<transform_result_t<Input, Result, T>, Result> > invoke_transform(T const &cb, offs_t &offset, Input data, std::make_unsigned_t<Input> &mem_mask) { return std::make_unsigned_t<transform_result_t<Input, Result, T> >(cb(offset, data, mem_mask)); }
        //template <typename Input, typename Result, typename T> static std::enable_if_t<is_transform_form4<Input, Result, T>::value, mask_t<transform_result_t<Input, Result, T>, Result> > invoke_transform(T const &cb, offs_t &offset, Input data, std::make_unsigned_t<Input> &mem_mask) { return std::make_unsigned_t<transform_result_t<Input, Result, T> >(cb(offset, data)); }
        //template <typename Input, typename Result, typename T> static std::enable_if_t<is_transform_form6<Input, Result, T>::value, mask_t<transform_result_t<Input, Result, T>, Result> > invoke_transform(T const &cb, offs_t &offset, Input data, std::make_unsigned_t<Input> &mem_mask) { return std::make_unsigned_t<transform_result_t<Input, Result, T> >(cb(data)); }
        protected static devcb_value invoke_transform(Delegate cb, ref offs_t offset, devcb_value data, ref devcb_value mem_mask)
        {
            return (devcb_value)cb.DynamicInvoke(offset, data, mem_mask);
        }

        // Working with devices and interfaces
        //template <typename T> static std::enable_if_t<emu::detail::is_device_implementation<T>::value, const char *> get_tag(T &obj) { return obj.tag(); }
        //template <typename T> static std::enable_if_t<emu::detail::is_device_interface<T>::value, const char *> get_tag(T &obj) { return obj.device().tag(); }
        //template <typename T, typename U> static T &cast_reference(U &obj)
        //{
        //    if constexpr (std::is_convertible_v<std::add_pointer_t<U>, std::add_pointer_t<T> >)
        //        return downcast<T &>(obj);
        //    else
        //        return dynamic_cast<T &>(obj);
        //}


        /// \brief Base transform helper
        ///
        /// Provides member functions for setting exclusive-or, mask and
        /// shifts.  Exclusive-or and mask values are stored; it's assumed
        /// that the implementation supports lamba transforms to allow
        /// shifts.
        //template <typename T, typename Impl>
        public class transform_base<Type_T, Impl>
            where Type_T : Type_constant, new()
        {
            static readonly Type T = new Type_T().value;


            public devcb_value m_exor = new devcb_value(devcb_value.make_unsigned(T), 0U);  //std::make_unsigned_t<T> m_exor = std::make_unsigned_t<T>(0);
            public devcb_value m_mask;  //std::make_unsigned_t<T> m_mask;
            public bool m_inherited_mask = true;



            // MCS - moved to child class

            //Impl &exor(std::make_unsigned_t<T> val) { m_exor ^= val; return static_cast<Impl &>(*this); }
            //Impl &mask(std::make_unsigned_t<T> val) { m_mask = m_inherited_mask ? val : (m_mask & val); m_inherited_mask = false; return static_cast<Impl &>(*this); }
            //Impl &invert() { return exor(~std::make_unsigned_t<T>(0)); }

            //auto rshift(unsigned val)
            //{
            //    auto trans(static_cast<Impl &>(*this).transform([val] (offs_t offset, T data, std::make_unsigned_t<T> &mem_mask) { mem_mask >>= val; return data >> val; }));
            //    return inherited_mask() ? std::move(trans) : std::move(trans.mask(m_mask >> val));
            //}
            //auto lshift(unsigned val)
            //{
            //    auto trans(static_cast<Impl &>(*this).transform([val] (offs_t offset, T data, std::make_unsigned_t<T> &mem_mask) { mem_mask <<= val; return data << val; }));
            //    return inherited_mask() ? std::move(trans) : std::move(trans.mask(m_mask << val));
            //}

            //auto bit(unsigned val) { return std::move(rshift(val).mask(T(1U))); }


            public devcb_value exor() { return m_exor & m_mask; }  //constexpr std::make_unsigned_t<T> exor() const { return m_exor & m_mask; }
            public devcb_value mask() { return m_mask; }  //constexpr std::make_unsigned_t<T> mask() const { return m_mask; }

            //constexpr bool need_exor() const { return std::make_unsigned_t<T>(0) != (m_exor & m_mask); }
            //constexpr bool need_mask() const { return std::make_unsigned_t<T>(~std::make_unsigned_t<T>(0)) != m_mask; }

            public bool inherited_mask() { return m_inherited_mask; }


            public transform_base(devcb_value mask) { m_mask = new devcb_value(devcb_value.make_unsigned(T), mask); }  //constexpr transform_base(std::make_unsigned_t<T> mask) : m_mask(mask) { }
            //constexpr transform_base(transform_base const &) = default;
            //transform_base(transform_base &&) = default;
            //transform_base &operator=(transform_base const &) = default;
            //transform_base &operator=(transform_base &&) = default;
        }


        /// \brief Callback array helper
        ///
        /// Simplifies construction and resolution of arrays of callbacks.
        //template <typename T, unsigned Count>
        public class array<T, unsigned_Count> : std.array<T, unsigned_Count>  //class array : public std::array<T, Count>
            where T : IResolve
            where unsigned_Count : u64_const, new()
        {
            //using std::array<T, Count>::array;


            //template <unsigned... V>
            public array(device_t owner, int unused, Func<T> add_function)  //array(device_t &owner, std::integer_sequence<unsigned, V...> const &)
                : base()  //: std::array<T, Count>{{ { make_one<V>(owner) }... }}
            {
                for (size_t i = 0; i < this.size(); i++)
                    this[i] = add_function();
            }


            public array(device_t owner, Func<T> add_function) : this(owner, 0, add_function) { }  //array(device_t &owner) : array(owner, std::make_integer_sequence<unsigned, Count>()) { }


            //template <unsigned N> device_t &make_one(device_t &owner) { return owner; }


            public virtual void resolve_all()
            {
                foreach (T elem in this)
                {
                    elem.resolve();
                }
            }
        }


        public class Type_constant_mask_t<Type_T, Type_U> : Type_constant
            where Type_T : Type_constant, new()
            where Type_U : Type_constant, new()
        {
            static readonly Type T = new Type_T().value;
            static readonly Type U = new Type_U().value;

            Type intermediate_t { get { return sizeof_(T) >= sizeof_(U) ? T : U; } }

            public Type value { get { return devcb_value.make_unsigned(intermediate_t); } }
        }


        public class Type_constant_transform_result_t<Type_Input, Type_Result, Func> : Type_constant
            where Type_Input : Type_constant, new()
        {
            static readonly Type Input = new Type_Input().value;

            public Type value { get { return Input; } }
        }


        protected devcb_base(device_t owner)
        {
            m_owner = owner;
        }

        //~devcb_base() { }


        protected device_t owner() { return m_owner; }
    }


    /// \brief Read callback utilities
    ///
    /// Helpers that don't need to be templated on callback type.
    public abstract class devcb_read_base : devcb_base
    {
        // Detecting candidates for read functions
        //template <typename Result, typename Func> using is_read_form1 = std::is_invocable_r<Result, Func, offs_t, Result>;
        //template <typename Result, typename Func> using is_read_form2 = std::is_invocable_r<Result, Func, offs_t>;
        //template <typename Result, typename Func> using is_read_form3 = std::is_invocable_r<Result, Func>;
        //template <typename Result, typename Func> using is_read = std::bool_constant<is_read_form1<Result, Func>::value || is_read_form2<Result, Func>::value || is_read_form3<Result, Func>::value>;

        // Determining the result type of a read function
        //template <typename Result, typename Func, typename Enable = void> struct read_result;
        //template <typename Result, typename Func> struct read_result<Result, Func, std::enable_if_t<is_read_form1<Result, Func>::value> > { using type = std::invoke_result_t<Func, offs_t, std::make_unsigned_t<Result>>; };
        //template <typename Result, typename Func> struct read_result<Result, Func, std::enable_if_t<is_read_form2<Result, Func>::value> > { using type = std::invoke_result_t<Func, offs_t>; };
        //template <typename Result, typename Func> struct read_result<Result, Func, std::enable_if_t<is_read_form3<Result, Func>::value> > { using type = std::invoke_result_t<Func>; };
        //template <typename Result, typename Func> using read_result_t = typename read_result<Result, Func>::type;

        // Detecting candidates for read delegates
        //template <typename T, typename Enable = void> struct is_read_method : public std::false_type { };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read8s_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read16s_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read32s_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read64s_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read8sm_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read16sm_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read32sm_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read64sm_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read8smo_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read16smo_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read32smo_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read64smo_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read_line_delegate, std::remove_reference_t<T> > > > : public std::true_type { };

        // Invoking read callbacks
        //template <typename Result, typename T> static std::enable_if_t<is_read_form1<Result, T>::value, mask_t<read_result_t<Result, T>, Result> > invoke_read(T const &cb, offs_t offset, std::make_unsigned_t<Result> mem_mask) { return std::make_unsigned_t<read_result_t<Result, T> >(cb(offset, mem_mask)); }
        //template <typename Result, typename T> static std::enable_if_t<is_read_form2<Result, T>::value, mask_t<read_result_t<Result, T>, Result> > invoke_read(T const &cb, offs_t offset, std::make_unsigned_t<Result> mem_mask) { return std::make_unsigned_t<read_result_t<Result, T> >(cb(offset)); }
        //template <typename Result, typename T> static std::enable_if_t<is_read_form3<Result, T>::value, mask_t<read_result_t<Result, T>, Result> > invoke_read(T const &cb, offs_t offset, std::make_unsigned_t<Result> mem_mask) { return std::make_unsigned_t<read_result_t<Result, T> >(cb()); }
        protected static devcb_value invoke_read<Input, Input_unsigned, T>(T cb, offs_t offset, devcb_value mem_mask)
            where T : Delegate
        {
            //return std::make_unsigned_t<read_result_t<Result, T> >(cb(offset, mem_mask));
            switch (cb)
            {
                case read8s_delegate t1:     return new devcb_value(typeof(u8),  (u8)cb.DynamicInvoke(offset, mem_mask.u8));
                case read16s_delegate t2:    return new devcb_value(typeof(u16), (u16)cb.DynamicInvoke(offset, mem_mask.u16));
                case read32s_delegate t3:    return new devcb_value(typeof(u32), (u32)cb.DynamicInvoke(offset, mem_mask.u32));
                case read64s_delegate t4:    return new devcb_value(typeof(u64), (u64)cb.DynamicInvoke(offset, mem_mask.u64));

                case read8sm_delegate t5:    return new devcb_value(typeof(u8),  (u8)cb.DynamicInvoke(offset));
                case read16sm_delegate t6:   return new devcb_value(typeof(u16), (u16)cb.DynamicInvoke(offset));
                case read32sm_delegate t7:   return new devcb_value(typeof(u32), (u32)cb.DynamicInvoke(offset));
                case read64sm_delegate t8:   return new devcb_value(typeof(u64), (u64)cb.DynamicInvoke(offset));

                case read8smo_delegate t9:   return new devcb_value(typeof(u8),  (u8)cb.DynamicInvoke());
                case read16smo_delegate t10: return new devcb_value(typeof(u16), (u16)cb.DynamicInvoke());
                case read32smo_delegate t11: return new devcb_value(typeof(u32), (u32)cb.DynamicInvoke());
                case read64smo_delegate t12: return new devcb_value(typeof(u64), (u64)cb.DynamicInvoke());

                case read_line_delegate t13: return new devcb_value(typeof(s32), (s32)cb.DynamicInvoke());

                default: throw new emu_unimplemented();
            }
        }


        // Delegate characteristics
        //template <typename T, typename Dummy = void> struct delegate_traits;
        //template <typename Dummy> struct delegate_traits<read8s_delegate, Dummy> { static constexpr u8 default_mask = ~u8(0); };
        //template <typename Dummy> struct delegate_traits<read16s_delegate, Dummy> { static constexpr u16 default_mask = ~u16(0); };
        //template <typename Dummy> struct delegate_traits<read32s_delegate, Dummy> { static constexpr u32 default_mask = ~u32(0); };
        //template <typename Dummy> struct delegate_traits<read64s_delegate, Dummy> { static constexpr u64 default_mask = ~u64(0); };
        //template <typename Dummy> struct delegate_traits<read8sm_delegate, Dummy> { static constexpr u8 default_mask = ~u8(0); };
        //template <typename Dummy> struct delegate_traits<read16sm_delegate, Dummy> { static constexpr u16 default_mask = ~u16(0); };
        //template <typename Dummy> struct delegate_traits<read32sm_delegate, Dummy> { static constexpr u32 default_mask = ~u32(0); };
        //template <typename Dummy> struct delegate_traits<read64sm_delegate, Dummy> { static constexpr u64 default_mask = ~u64(0); };
        //template <typename Dummy> struct delegate_traits<read8smo_delegate, Dummy> { static constexpr u8 default_mask = ~u8(0); };
        //template <typename Dummy> struct delegate_traits<read16smo_delegate, Dummy> { static constexpr u16 default_mask = ~u16(0); };
        //template <typename Dummy> struct delegate_traits<read32smo_delegate, Dummy> { static constexpr u32 default_mask = ~u32(0); };
        //template <typename Dummy> struct delegate_traits<read64smo_delegate, Dummy> { static constexpr u64 default_mask = ~u64(0); };
        //template <typename Dummy> struct delegate_traits<read_line_delegate, Dummy> { static constexpr unsigned default_mask = 1U; };
        public class delegate_traits<T>
        {
            public static devcb_value default_mask
            {
                get
                {
                    if (typeof(T) == typeof(read8sm_delegate) ||
                        typeof(T) == typeof(read8smo_delegate))
                        return new devcb_value(typeof(u8), u8.MaxValue);
                    else if (typeof(T) == typeof(read_line_delegate))
                        return new devcb_value(typeof(u32), 1U);
                    else
                        throw new emu_unimplemented();
                }
            }
        }


        public class Type_constant_read_result_t<Result, Func> : Type_constant
        {
            public Type value
            {
                get
                {
                    if (typeof(Func) == typeof(read8sm_delegate) ||
                        typeof(Func) == typeof(read8smo_delegate))
                        return typeof(u8);
                    else if (typeof(Func) == typeof(read_line_delegate))
                        return typeof(s32);
                    else
                        throw new emu_unimplemented();
                }
            }
        }


        //using devcb_base::devcb_base;
        protected devcb_read_base(device_t owner) : base(owner) { }
        //~devcb_read_base() { }
    }


    /// \brief Write callback utilities
    ///
    /// Helpers that don't need to be templated on callback type.
    public abstract class devcb_write_base : devcb_base
    {
        // Detecting candidates for write functions
        //template <typename Input, typename Func> using is_write_form1 = std::is_invocable<Func, offs_t, Input, std::make_unsigned_t<Input> >;
        //template <typename Input, typename Func> using is_write_form2 = std::is_invocable<Func, offs_t, Input>;
        //template <typename Input, typename Func> using is_write_form3 = std::is_invocable<Func, Input>;
        //template <typename Input, typename Func> using is_write = std::bool_constant<is_write_form1<Input, Func>::value || is_write_form2<Input, Func>::value || is_write_form3<Input, Func>::value>;

        // Detecting candidates for write delegates
        //template <typename T, typename Enable = void> struct is_write_method : public std::false_type { };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write8s_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write16s_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write32s_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write64s_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write8sm_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write16sm_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write32sm_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write64sm_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write8smo_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write16smo_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write32smo_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write64smo_delegate, std::remove_reference_t<T> > > > : public std::true_type { };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write_line_delegate, std::remove_reference_t<T> > > > : public std::true_type { };

        // Invoking write callbacks
        //template <typename Input, typename T> static std::enable_if_t<is_write_form1<Input, T>::value> invoke_write(T const &cb, offs_t &offset, Input data, std::make_unsigned_t<Input> mem_mask) { return cb(offset, data, mem_mask); }
        //template <typename Input, typename T> static std::enable_if_t<is_write_form2<Input, T>::value> invoke_write(T const &cb, offs_t &offset, Input data, std::make_unsigned_t<Input> mem_mask) { return cb(offset, data); }
        //template <typename Input, typename T> static std::enable_if_t<is_write_form3<Input, T>::value> invoke_write(T const &cb, offs_t &offset, Input data, std::make_unsigned_t<Input> mem_mask) { return cb(data); }
        //template <typename Input, typename T>
        protected static void invoke_write<Input, T>(T cb, offs_t offset, devcb_value data, devcb_value mem_mask)
            where T : Delegate
        {
            //return cb(offset, data, mem_mask);
            switch (cb)
            {
                case write8s_delegate t1:     cb.DynamicInvoke(offset, data.u8, mem_mask.u8); break;
                case write16s_delegate t2:    cb.DynamicInvoke(offset, data.u16, mem_mask.u16); break;
                case write32s_delegate t3:    cb.DynamicInvoke(offset, data.u32, mem_mask.u32); break;
                case write64s_delegate t4:    cb.DynamicInvoke(offset, data.u64, mem_mask.u64); break;

                case write8sm_delegate t5:    cb.DynamicInvoke(offset, data.u8); break;
                case write16sm_delegate t6:   cb.DynamicInvoke(offset, data.u16); break;
                case write32sm_delegate t7:   cb.DynamicInvoke(offset, data.u32); break;
                case write64sm_delegate t8:   cb.DynamicInvoke(offset, data.u64); break;

                case write8smo_delegate t9:   cb.DynamicInvoke(data.u8); break;
                case write16smo_delegate t10: cb.DynamicInvoke(data.u16); break;
                case write32smo_delegate t11: cb.DynamicInvoke(data.u32); break;
                case write64smo_delegate t12: cb.DynamicInvoke(data.u64); break;

                case write_line_delegate t13: cb.DynamicInvoke(data.s32); break;

                default: throw new emu_unimplemented();
            }
        }


        // Delegate characteristics
        //template <typename T, typename Dummy = void> struct delegate_traits;
        //template <typename Dummy> struct delegate_traits<write8s_delegate, Dummy> { using input_t = u8; static constexpr u8 default_mask = ~u8(0); };
        //template <typename Dummy> struct delegate_traits<write16s_delegate, Dummy> { using input_t = u16; static constexpr u16 default_mask = ~u16(0); };
        //template <typename Dummy> struct delegate_traits<write32s_delegate, Dummy> { using input_t = u32; static constexpr u32 default_mask = ~u32(0); };
        //template <typename Dummy> struct delegate_traits<write64s_delegate, Dummy> { using input_t = u64; static constexpr u64 default_mask = ~u64(0); };
        //template <typename Dummy> struct delegate_traits<write8sm_delegate, Dummy> { using input_t = u8; static constexpr u8 default_mask = ~u8(0); };
        //template <typename Dummy> struct delegate_traits<write16sm_delegate, Dummy> { using input_t = u16; static constexpr u16 default_mask = ~u16(0); };
        //template <typename Dummy> struct delegate_traits<write32sm_delegate, Dummy> { using input_t = u32; static constexpr u32 default_mask = ~u32(0); };
        //template <typename Dummy> struct delegate_traits<write64sm_delegate, Dummy> { using input_t = u64; static constexpr u64 default_mask = ~u64(0); };
        //template <typename Dummy> struct delegate_traits<write8smo_delegate, Dummy> { using input_t = u8; static constexpr u8 default_mask = ~u8(0); };
        //template <typename Dummy> struct delegate_traits<write16smo_delegate, Dummy> { using input_t = u16; static constexpr u16 default_mask = ~u16(0); };
        //template <typename Dummy> struct delegate_traits<write32smo_delegate, Dummy> { using input_t = u32; static constexpr u32 default_mask = ~u32(0); };
        //template <typename Dummy> struct delegate_traits<write64smo_delegate, Dummy> { using input_t = u64; static constexpr u64 default_mask = ~u64(0); };
        //template <typename Dummy> struct delegate_traits<write_line_delegate, Dummy> { using input_t = int; static constexpr unsigned default_mask = 1U; };
        public class delegate_traits<T>
        {
            public class input_t : Type_constant
            {
                public Type value
                {
                    get
                    {
                        if (typeof(T) == typeof(write8sm_delegate) ||
                            typeof(T) == typeof(write8smo_delegate))
                            return typeof(u8);
                        else if (typeof(T) == typeof(write_line_delegate))
                            return typeof(u32);
                        else
                            throw new emu_unimplemented();
                    }
                }
            }

            public static devcb_value default_mask
            {
                get
                {
                    if (typeof(T) == typeof(write8sm_delegate) ||
                        typeof(T) == typeof(write8smo_delegate))
                        return new devcb_value(typeof(u8), u8.MaxValue);
                    else if (typeof(T) == typeof(write_line_delegate))
                        return new devcb_value(typeof(u32), 1U);
                    else
                        throw new emu_unimplemented();
                }
            }
        }


        //using devcb_base::devcb_base;
        protected devcb_write_base(device_t owner) : base(owner) { }
        //~devcb_write_base() { }
    }


    public class devcb_read<Type_Result> : devcb_read<Type_Result, devcb_value_const_unsigned_MaxValue<Type_Result>>
        where Type_Result : Type_constant, new()
    {
        public devcb_read(device_t owner) : base(owner) { }
    }

    /// \brief Read callback helper
    ///
    /// Allows binding a variety of signatures, composing a result from
    /// multiple callbacks, and chained arbitrary transforms.  Transforms
    /// may modify the offset and mask.
    //template <typename Result, std::make_unsigned_t<Result> DefaultMask = std::make_unsigned_t<Result>(~std::make_unsigned_t<Result>(0))>
    public class devcb_read<Type_Result, devcb_value_const_unsigned_DefaultMask> : devcb_read_base, IResolve  //class devcb_read : public devcb_read_base
        where Type_Result : Type_constant, new()
        where devcb_value_const_unsigned_DefaultMask : devcb_value_const, new()
    {
        static readonly Type Result = new Type_Result().value;
        static readonly devcb_value DefaultMask = new devcb_value_const_unsigned_DefaultMask().value;


        public delegate devcb_value func_t(offs_t offset, devcb_value mem_mask);  //using func_t = std::function<Result (offs_t, std::make_unsigned_t<Result>)>;



        protected abstract class creator
        {
            //using ptr = std::unique_ptr<creator>;


            devcb_value m_mask;  //std::make_unsigned_t<Result> m_mask;


            protected creator(devcb_value mask) { m_mask = mask; }
            //virtual ~creator() { }


            protected abstract void validity_check(validity_checker valid);


            public abstract func_t create();


            devcb_value mask() { return m_mask; }  //std::make_unsigned_t<Result> mask() const { return m_mask; }
        }


        public interface creator_impl_builder
        {
            devcb_value mask();
            void build(Delegate chain);
        }


        public delegate devcb_value transform_function_delegate(offs_t offset, devcb_value data, ref devcb_value mem_mask);


        //template <typename T>
        protected class creator_impl<T> : creator
            where T : creator_impl_builder
        {
            T m_builder;


            public creator_impl(T builder) : base(builder.mask()) { m_builder = builder; }


            protected override void validity_check(validity_checker valid) { throw new emu_unimplemented(); }  //virtual void validity_check(validity_checker &valid) const override { m_builder.validity_check(valid); }


            public override func_t create()
            {
                func_t result = null;
                m_builder.build(new Action<func_t>((f) => { var cb = f;  result = (offs_t offset, devcb_value mem_mask) => { return cb(offset, mem_mask); }; }));  //m_builder.build([&result] (auto &&f) { result = [cb = std::move(f)] (offs_t offset, typename T::input_mask_t mem_mask) { return cb(offset, mem_mask); }; });
                return result;
            }
        }


        //class log_creator : public creator


        public abstract class builder_base : IDisposable
        {
            bool m_disposedValue = false; // To detect redundant calls


            protected devcb_read<Type_Result, devcb_value_const_unsigned_DefaultMask> m_target;
            protected bool m_append;
            protected bool m_consumed = false;
            bool m_built = false;


            public builder_base(devcb_read<Type_Result, devcb_value_const_unsigned_DefaultMask> target, bool append) { m_target = target;  m_append = append; }
            //builder_base(builder_base const &) = delete;
            //builder_base(builder_base &&) = default;

            ~builder_base()
            {
                assert(m_consumed, string.Format("~builder_base() - {0} - {1} - {2}\n", m_target.owner().owner().GetType(), m_target.owner().tag(), m_target.owner().name()));
            }


            //builder_base &operator=(builder_base const &) = delete;
            //builder_base &operator=(builder_base &&) = default;


            protected virtual void Dispose(bool disposing)
            {
                if (!m_disposedValue)
                {
                    if (disposing)
                    {
                        // dispose managed state (managed objects).
                    }

                    // free unmanaged resources (unmanaged objects) and override a finalizer below.
                    assert(m_consumed);

                    m_disposedValue = true;
                }
            }

            // calls register_creator().  do this here instead of in the dtor
            public void Dispose() { Dispose(true); }
            public void reg() { Dispose(); }


            public abstract void build(Delegate chain);


            public void consume() { m_consumed = true; }
            protected void built() { assert(!m_built); m_built = true; }


            //template <typename T>
            protected void register_creator<T>()
                where T : creator_impl_builder
            {
                if (!m_consumed)
                {
                    if (!m_append)
                        m_target.m_creators.clear();

                    consume();

                    m_target.m_creators.emplace_back(new creator_impl<T>((T)(creator_impl_builder)this));  //m_target.m_creators.emplace_back(std::make_unique<creator_impl<T> >(std::move(static_cast<T &>(*this))));
                }
            }
        }


        public abstract class builder_base_with_transform_base<Type_T, Impl, Impl_OPS, Impl_output_t> : builder_base, creator_impl_builder//, creator_impl_transform<Impl, Impl_OPS, Impl_output_t>
            where Type_T : Type_constant, new()
            where Impl : builder_base
            where Impl_OPS : io_operations<Impl>, new()
            where Impl_output_t : Type_constant, new()
        {
            static readonly Impl_OPS ops = new Impl_OPS();


            transform_base<Type_T, Impl> m_transform_base;


            protected builder_base_with_transform_base(devcb_read<Type_Result, devcb_value_const_unsigned_DefaultMask> target, bool append, devcb_value mask) : base(target, append)
            {
                m_transform_base = new transform_base<Type_T, Impl>(mask);
            }


            // transform_base

            protected Impl mask(devcb_value val) { m_transform_base.m_mask = m_transform_base.m_inherited_mask ? new devcb_value(m_transform_base.m_mask.type, val) : new devcb_value(m_transform_base.m_mask.type, m_transform_base.m_mask & val); m_transform_base.m_inherited_mask = false; return ops.cast(this); }  //Impl &mask(std::make_unsigned_t<T> val) { m_mask = m_inherited_mask ? val : (m_mask & val); m_inherited_mask = false; return static_cast<Impl &>(*this); }
            public Impl mask_u32(u32 val) { return mask(new devcb_value(val.GetType(), val)); }

            public transform_builder<Impl, Impl_OPS, Impl_output_t, transform_function_delegate> rshift(unsigned val)  //auto rshift(unsigned val)
            {
                var trans = this.transform((offs_t offset, devcb_value data, ref devcb_value mem_mask) => { mem_mask >>= (int)val; return data >> (int)val; });  //auto trans(static_cast<Impl &>(*this).transform([val] (offs_t offset, T data, std::make_unsigned_t<T> &mem_mask) { mem_mask >>= val; return data >> val; }));
                if (inherited_mask())
                    return trans;
                else
                    return trans.mask(m_transform_base.m_mask >> (int)val);  //return std::move(trans.mask(m_mask >> val));
            }

            public transform_builder<Impl, Impl_OPS, Impl_output_t, transform_function_delegate> lshift(unsigned val)  //auto lshift(unsigned val)
            {
                var trans = this.transform((offs_t offset, devcb_value data, ref devcb_value mem_mask) => { mem_mask <<= (int)val; return data << (int)val; });  //auto trans(static_cast<Impl &>(*this).transform([val] (offs_t offset, T data, std::make_unsigned_t<T> &mem_mask) { mem_mask <<= val; return data << val; }));
                if (inherited_mask())
                    return trans;
                else
                    return trans.mask(m_transform_base.m_mask << (int)val);  //return std::move(trans.mask(m_mask << val));
            }


            public virtual transform_builder<Impl, Impl_OPS, Impl_output_t, transform_function_delegate> transform(transform_function_delegate cb) { throw new emu_unimplemented(); }

            //auto bit(unsigned val) { return std::move(rshift(val).mask(T(1U))); }

            protected devcb_value exor() { return m_transform_base.exor(); }  //constexpr std::make_unsigned_t<T> exor() const { return m_exor & m_mask; }
            public devcb_value mask() { return m_transform_base.mask(); }  //constexpr std::make_unsigned_t<T> mask() const { return m_mask; }

            //constexpr bool need_exor() const { return std::make_unsigned_t<T>(0) != (m_exor & m_mask); }
            //constexpr bool need_mask() const { return std::make_unsigned_t<T>(~std::make_unsigned_t<T>(0)) != m_mask; }

            protected bool inherited_mask() { return m_transform_base.m_inherited_mask; }
        }


        public interface io_operations<Impl>
        {
            Type input_t { get; }
            Type output_t { get; }
            Type input_mask_t { get; }

            Impl cast(builder_base base_);
        }


        //template <typename Source, typename Func>
        public class transform_builder<Source, Source_OPS, Source_output_t, Func_> :  //class transform_builder : public builder_base, public transform_base<mask_t<transform_result_t<typename Source::output_t, Result, Func>, Result>, transform_builder<Source, Func> >
                    builder_base_with_transform_base<Type_constant_mask_t<Type_constant_transform_result_t<Source_output_t, Type_Result, Func_>, Type_Result>, 
                                                     transform_builder<Source, Source_OPS, Source_output_t, Func_>, 
                                                     transform_builder<Source, Source_OPS, Source_output_t, Func_>.io_ops_transform_builder, 
                                                     transform_builder<Source, Source_OPS, Source_output_t, Func_>.Type_constant_output_t>,
                    IDisposable
            where Source : builder_base
            where Source_OPS : io_operations<Source>, new()
            where Source_output_t : Type_constant, new()
            where Func_ : Delegate
        {
            static readonly Source_OPS source_ops = new Source_OPS();


            //template <typename T, typename U> friend class transform_builder;


            //using input_t = typename Source::output_t;
            //using output_t = mask_t<transform_result_t<input_t, Result, Func>, Result>;
            //using input_mask_t = mask_t<input_t, typename Source::input_mask_t>;
            public class io_ops_transform_builder : io_operations<transform_builder<Source, Source_OPS, Source_output_t, Func_>>
            {
                static readonly Source_OPS source_ops = new Source_OPS();
                static readonly Type output_t_Type = new Type_constant_mask_t<Type_constant_transform_result_t<Type_constant_input_t, Type_Result, Func_>, Type_Result>().value;

                public Type input_t { get { return source_ops.output_t; } }
                public Type output_t { get { return output_t_Type; } }
                public Type input_mask_t { get { throw new emu_unimplemented(); } }

                public transform_builder<Source, Source_OPS, Source_output_t, Func_> cast(builder_base base_) { return (transform_builder<Source, Source_OPS, Source_output_t, Func_>)base_; }

                public class Type_constant_input_t : Type_constant { static readonly io_ops_transform_builder ops = new io_ops_transform_builder();  public Type value { get { return ops.input_t; } } }
            }

            static readonly io_ops_transform_builder io_ops = new io_ops_transform_builder();

            public class Type_constant_output_t : Type_constant { public Type value { get { throw new emu_unimplemented(); } } }


            Source m_src;
            Func_ m_cb;


            //template <typename T>
            public transform_builder(devcb_read<Type_Result, devcb_value_const_unsigned_DefaultMask> target, bool append, Source src, Func_ cb, devcb_value mask)  //transform_builder(devcb_read &target, bool append, Source &&src, T &&cb, output_t mask)
                : base(target, append, new devcb_value(io_ops.output_t, mask))  //: builder_base(target, append)
            {
                //, transform_base<output_t, transform_builder>(mask)


                m_src = src;
                m_cb = cb;


                m_src.consume();
            }


            //transform_builder(transform_builder &&that)
            //    : builder_base(std::move(that))
            //    , transform_base<output_t, transform_builder>(std::move(that))
            //    , m_src(std::move(that.m_src))
            //    , m_cb(std::move(that.m_cb))
            //{
            //    m_src.consume();
            //    that.consume();
            //    that.built();
            //}


            //~transform_builder() { this->template register_creator<transform_builder>(); }
            protected override void Dispose(bool disposing) { register_creator<transform_builder<Source, Source_OPS, Source_output_t, Func_>>(); base.Dispose(disposing); }


            //template <typename T>
            public override transform_builder<transform_builder<Source, Source_OPS, Source_output_t, Func_>, io_ops_transform_builder, Type_constant_output_t, transform_function_delegate> transform(transform_function_delegate cb)  //std::enable_if_t<is_transform<output_t, Result, T>::value, transform_builder<transform_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                throw new emu_unimplemented();
            }


            //void validity_check(validity_checker &valid) const { m_src.validity_check(valid); }


            delegate devcb_value build_parameter_callback(devcb_value param1, devcb_value param2, devcb_value param3);

            //template <typename T>
            public override void build(Delegate chain)  //void build(T &&chain)
            {
                assert(this.m_consumed);

                var c = chain;
                var wrap = new Action<func_t>((f) => { this.build(c, f); });  //auto wrap([this, c = std::forward<T>(chain)] (auto &&f) mutable { this->build(std::move(c), std::move(f)); });
                m_src.build(wrap);  //m_src.build(std::move(wrap));
            }


            //transform_builder(transform_builder const &) = delete;
            //transform_builder &operator=(transform_builder const &) = delete;
            //transform_builder &operator=(transform_builder &&that) = delete;


            //template <typename T, typename U>
            void build(Delegate chain, Delegate f)  //void build(T &&chain, U &&f)
            {
                assert(this.m_consumed);
                this.built();

                var src = f;
                var cb = this.m_cb;
                var exor = this.exor();
                var mask = this.mask();
                chain.DynamicInvoke(
                        (func_t)((offs_t offset, devcb_value mem_mask) =>  //[src = std::forward<U>(f), cb = std::move(this->m_cb), exor = this->exor(), mask = this->mask()] (offs_t &offset, input_mask_t &mem_mask)
                        {
                            devcb_value source_mask = new devcb_value(source_ops.input_mask_t, mem_mask);  //typename Source::input_mask_t source_mask(mem_mask);
                            var data = (devcb_value)src.DynamicInvoke(offset, source_mask);  //auto const data(src(offset, source_mask));
                            mem_mask = source_mask & mask;  //mem_mask = source_mask & mask;
                            return (invoke_transform(cb, ref offset, data, ref mem_mask) ^ exor) & mask;  //return (devcb_read::invoke_transform<input_t, Result>(cb, offset, data, mem_mask) ^ exor) & mask;
                        }));
            }
        }


        //template <typename Func>
        //class functoid_builder : public builder_base, public transform_base<mask_t<read_result_t<Result, Func>, Result>, functoid_builder<Func> >


        //template <typename Delegate>
        public class delegate_builder<Delegate_> :  //class delegate_builder : public builder_base, public transform_base<mask_t<read_result_t<Result, Delegate>, Result>, delegate_builder<Delegate> >
                    builder_base_with_transform_base<Type_constant_mask_t<Type_constant_read_result_t<Type_Result, Delegate_>, Type_Result>,
                                                     delegate_builder<Delegate_>,
                                                     delegate_builder<Delegate_>.io_ops_delegate_builder, 
                                                     delegate_builder<Delegate_>.Type_constant_output_t>,
                    IDisposable
            where Delegate_ : Delegate
        {
            //template <typename T, typename U> friend class transform_builder;


            //using output_t = mask_t<read_result_t<Result, Delegate>, Result>;
            //using input_mask_t = std::make_unsigned_t<Result>;
            public class io_ops_delegate_builder : io_operations<delegate_builder<Delegate_>>
            {
                static readonly Type output_t_Type = new Type_constant_mask_t<Type_constant_read_result_t<Type_Result, Delegate_>, Type_Result>().value;
                static readonly Type input_mask_t_Type = devcb_value.make_unsigned(Result);

                public Type input_t { get { throw new emu_unimplemented(); } }
                public Type output_t { get { return output_t_Type; } }
                public Type input_mask_t { get { return input_mask_t_Type; } }

                public delegate_builder<Delegate_> cast(builder_base base_) { return (delegate_builder<Delegate_>)base_; }
            }

            static readonly io_ops_delegate_builder io_ops = new io_ops_delegate_builder();

            public class Type_constant_output_t : Type_constant { public Type value { get { return io_ops.output_t; } } }


            Delegate_ m_delegate;


            //template <typename T>
            public delegate_builder(devcb_read<Type_Result, devcb_value_const_unsigned_DefaultMask> target, bool append, device_t devbase, string tag, Delegate_ func)  //delegate_builder(devcb_read &target, bool append, device_t &devbase, char const *tag, T &&func, char const *name)
                : base(target, append, new devcb_value(io_ops.output_t, DefaultMask & delegate_traits<Delegate_>.default_mask))
            {
                //, transform_base<output_t, delegate_builder>(DefaultMask & delegate_traits<Delegate>::default_mask)


                m_delegate = func;  //, m_delegate(devbase, tag, std::forward<T>(func), name)
            }

            //template <typename T>
            //delegate_builder(devcb_read target, bool append, device_t devbase, devcb_read::delegate_device_class_t<T> obj, T func, string name)
            //{
            //    builder_base(target, append)
            //    transform_base<output_t, delegate_builder>(DefaultMask & delegate_traits<Delegate>::default_mask)
            //    , m_delegate(obj, std::forward<T>(func), name)
            //}

            //delegate_builder(delegate_builder that)
            //{
            //    builder_base(std::move(that))
            //    transform_base<output_t, delegate_builder>(std::move(that))
            //    m_delegate(std::move(that.m_delegate))
            //
            //
            //    that.consume();
            //    that.built();
            //}

            //~delegate_builder() { this->template register_creator<delegate_builder>(); }
            protected override void Dispose(bool disposing) { register_creator<delegate_builder<Delegate_>>(); base.Dispose(disposing); }


            //template <typename T>
            public override transform_builder<delegate_builder<Delegate_>, io_ops_delegate_builder, Type_constant_output_t, transform_function_delegate> transform(transform_function_delegate cb)  //std::enable_if_t<is_transform<output_t, Result, T>::value, transform_builder<delegate_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                devcb_value m = new devcb_value(io_ops.output_t, this.mask());  //output_t const m(this->mask());
                if (this.inherited_mask())
                    this.mask(delegate_traits<Delegate_>.default_mask);  //this->mask(delegate_traits<Delegate>::default_mask);
                return new transform_builder<delegate_builder<Delegate_>, io_ops_delegate_builder, Type_constant_output_t, transform_function_delegate>(this.m_target, this.m_append, this, cb, m);  //return transform_builder<delegate_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, std::move(*this), std::forward<T>(cb), m);
            }


            //void validity_check(validity_checker &valid) const
            //{
            //    if (!m_devbase.subdevice(m_delegate.device_name()))
            //        osd_printf_error("Read callback bound to non-existent object tag %s (%s)\n", m_delegate.device_name(), m_delegate.name());
            //}


            //template <typename T>
            public override void build(Delegate chain)  //void build(T &&chain)
            {
                assert(this.m_consumed);
                this.built();
                //m_delegate.resolve();
                var cb = this.m_delegate;
                var exor = this.exor();
                var mask = this.mask();
                chain.DynamicInvoke(  //chain(
                        (func_t)((offs_t offset, devcb_value mem_mask) =>  //[cb = std::move(this->m_delegate), exor = this->exor(), mask = this->mask()] (offs_t offset, input_mask_t mem_mask)
                        { return (invoke_read<devcb_value, devcb_value, Delegate_>(cb, offset, mem_mask & mask) ^ exor) & mask; }));  //{ return (devcb_read::invoke_read<Result>(cb, offset, mem_mask & mask) ^ exor) & mask; });
            }


            //delegate_builder(delegate_builder const &) = delete;
            //delegate_builder &operator=(delegate_builder const &) = delete;
            //delegate_builder &operator=(delegate_builder &&that) = delete;
        }


        public class ioport_builder :  //class ioport_builder : public builder_base, public transform_base<mask_t<ioport_value, Result>, ioport_builder>
                    builder_base_with_transform_base<Type_constant_mask_t<Type_constant_ioport_value, Type_Result>, 
                                                     ioport_builder, 
                                                     ioport_builder.io_ops_ioport_builder, 
                                                     ioport_builder.Type_constant_output_t>,
                    IDisposable
        {
            //template <typename T, typename U> friend class transform_builder;

            //using output_t = mask_t<ioport_value, Result>;
            //using input_mask_t = std::make_unsigned_t<Result>;
            public class io_ops_ioport_builder : io_operations<ioport_builder>
            {
                static readonly Type output_t_Type = new Type_constant_mask_t<Type_constant_ioport_value, Type_Result>().value;
                static readonly Type input_mask_t_Type = devcb_value.make_unsigned(Result);

                public Type input_t { get { throw new emu_unimplemented(); } }
                public Type output_t { get { return output_t_Type; } }
                public Type input_mask_t { get { return input_mask_t_Type; } }

                public ioport_builder cast(builder_base base_) { return (ioport_builder)base_; }
            }

            static readonly io_ops_ioport_builder io_ops = new io_ops_ioport_builder();

            public class Type_constant_output_t : Type_constant { public Type value { get { return io_ops.output_t; } } }


            device_t m_devbase;
            string m_tag;


            public ioport_builder(devcb_read<Type_Result, devcb_value_const_unsigned_DefaultMask> target, bool append, device_t devbase, string tag)
                : base(target, append, new devcb_value(io_ops.output_t, DefaultMask))
            {
                //, transform_base<output_t, ioport_builder>(DefaultMask)


                m_devbase = devbase;
                m_tag = tag;
            }

            //ioport_builder(ioport_builder &&that)
            //    : builder_base(std::move(that))
            //    , transform_base<output_t, ioport_builder>(std::move(that))
            //    , m_devbase(that.m_devbase)
            //    , m_tag(std::move(that.m_tag))
            //{
            //    that.consume();
            //    that.built();
            //}

            //~ioport_builder() { this->template register_creator<ioport_builder>(); }
            protected override void Dispose(bool disposing) { register_creator<ioport_builder>(); base.Dispose(disposing); }


            //template <typename T>
            public override transform_builder<ioport_builder, io_ops_ioport_builder, Type_constant_output_t, transform_function_delegate> transform(transform_function_delegate cb)  //std::enable_if_t<is_transform<output_t, Result, T>::value, transform_builder<ioport_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                devcb_value m = new devcb_value(io_ops.output_t, this.mask());  //output_t const m(this->mask());
                if (this.inherited_mask())
                    this.mask(new devcb_value(typeof(ioport_value), ioport_value.MaxValue));  //this->mask(std::make_unsigned_t<ioport_value>(~std::make_unsigned_t<ioport_value>(0)));
                return new transform_builder<ioport_builder, io_ops_ioport_builder, Type_constant_output_t, transform_function_delegate>(this.m_target, this.m_append, this, cb, m);  //return transform_builder<ioport_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, std::move(*this), std::forward<T>(cb), m);
            }


            //void validity_check(validity_checker &valid) const { }


            //template <typename T>
            public override void build(Delegate chain)  //void build(T &&chain)
            {
                assert(this.m_consumed);
                this.built();
                ioport_port ioport = m_devbase.ioport(m_tag);
                if (ioport == null)
                    throw new emu_fatalerror("Read callback bound to non-existent I/O port {0} of device {1} ({2})\n", m_tag, m_devbase.tag(), m_devbase.name());

                var port = ioport;
                var exor = this.exor();
                var mask = this.mask();
                chain.DynamicInvoke(  //chain(
                        (func_t)((offs_t offset, devcb_value mem_mask) =>  //[&port = *ioport, exor = this->exor(), mask = this->mask()] (offs_t offset, input_mask_t mem_mask)
                        { return (new devcb_value(typeof(ioport_value), port.read()) ^ exor) & mask; }));  //{ return (port.read() ^ exor) & mask; });
            }


            //ioport_builder(ioport_builder const &) = delete;
            //ioport_builder &operator=(ioport_builder const &) = delete;
            //ioport_builder &operator=(ioport_builder &&that) = delete;
        }


        public class binder
        {
            devcb_read<Type_Result, devcb_value_const_unsigned_DefaultMask> m_target;
            bool m_append = false;
            bool m_used = false;


            public binder(devcb_read<Type_Result, devcb_value_const_unsigned_DefaultMask> target) { m_target = target; }
            //binder(binder const &) = delete;
            binder(binder that) { m_target = that.m_target; m_append = that.m_append; m_used = that.m_used;   that.m_used = true; }
            //binder &operator=(binder const &) = delete;
            //binder &operator=(binder &&) = delete;


            //template <typename T>
            public delegate_builder<T> set_internal<T>(T func)  //std::enable_if_t<is_read_method<T>::value, delegate_builder<delegate_type_t<T> > > set(T &&func, char const *name)
                where T : Delegate
            {
                set_used();
                return new delegate_builder<T>(m_target, m_append, m_target.owner().mconfig().current_device(), DEVICE_SELF, func);
            }

            public delegate_builder<read8sm_delegate> set(read8sm_delegate func) { return set_internal(func); }
            public delegate_builder<read8smo_delegate> set(read8smo_delegate func) { return set_internal(func); }
            public delegate_builder<read_line_delegate> set(read_line_delegate func) { return set_internal(func); }


            //template <typename T>
            public delegate_builder<T> set_internal<T>(string tag, T func)  //std::enable_if_t<is_read_method<T>::value, delegate_builder<delegate_type_t<T> > > set(char const *tag, T &&func, char const *name)
                where T : Delegate
            {
                set_used();
                return new delegate_builder<T>(m_target, m_append, m_target.owner().mconfig().current_device(), tag, func);
            }

            public delegate_builder<read8sm_delegate> set(string tag, read8sm_delegate func) { return set_internal(tag, func); }
            public delegate_builder<read8smo_delegate> set(string tag, read8smo_delegate func) { return set_internal(tag, func); }
            public delegate_builder<read_line_delegate> set(string tag, read_line_delegate func) { return set_internal(tag, func); }


            //template <typename T, typename U>
            //std::enable_if_t<is_read_method<T>::value, delegate_builder<delegate_type_t<T> > > set(U &obj, T &&func, char const *name)
            //{
            //    set_used();
            //    return delegate_builder<delegate_type_t<T> >(m_target, m_append, m_target.owner(), devcb_read::cast_reference<delegate_device_class_t<T> >(obj), std::forward<T>(func), name);
            //}


            //template <typename T, typename U, bool R>
            public delegate_builder<T> set_internal<T, U, bool_R>(device_finder<U, bool_R> finder, T func)  //std::enable_if_t<is_read_method<T>::value, delegate_builder<delegate_type_t<T> > > set(device_finder<U, R> &finder, T &&func, char const *name)
                where T : Delegate
                where bool_R : bool_const, new()
            {
                set_used();
                std.pair<device_t, string> target = finder.finder_target();
                return new delegate_builder<T>(m_target, m_append, target.first, target.second, func);
            }

            //template <typename T, typename U, bool R>
            //std::enable_if_t<is_read_method<T>::value, delegate_builder<delegate_type_t<T> > > set(device_finder<U, R> const &finder, T &&func, char const *name)
            //{
            //    set_used();
            //    std::pair<device_t &, char const *> const target(finder.finder_target());
            //    return delegate_builder<delegate_type_t<T> >(m_target, m_append, target.first, target.second, std::forward<T>(func), name);
            //}

            public delegate_builder<read8sm_delegate> set<U, bool_R>(device_finder<U, bool_R> finder, read8sm_delegate func) where bool_R : bool_const, new() { return set_internal(func); }
            public delegate_builder<read_line_delegate> set<U, bool_R>(device_finder<U, bool_R> finder, read_line_delegate func) where bool_R : bool_const, new() { return set_internal(func); }


            //template <typename... Params>
            public delegate_builder<T> append_internal<T>(string tag, T func)  //auto append(Params &&... args)
                where T : Delegate
            {
                m_append = true;
                return set_internal(tag, func);  //return set(std::forward<Params>(args)...);
            }

            public delegate_builder<read_line_delegate> append(string tag, read_line_delegate func) { return append_internal(tag, func); }


            //template <typename... Params>
            public ioport_builder set_ioport(string args)  //ioport_builder set_ioport(Params &&... args)
            {
                set_used();
                return new ioport_builder(m_target, m_append, m_target.owner().mconfig().current_device(), args);
            }


            //template <bool R>
            //ioport_builder set_ioport(ioport_finder<R> &finder)
            //{
            //    set_used();
            //    std::pair<device_t &, char const *> const target(finder.finder_target());
            //    return ioport_builder(m_target, m_append, target.first, std::string(target.second));
            //}

            //template <bool R>
            //ioport_builder set_ioport(ioport_finder<R> const &finder)
            //{
            //    set_used();
            //    std::pair<device_t &, char const *> const target(finder.finder_target());
            //    return ioport_builder(m_target, m_append, target.first, std::string(target.second));
            //}

            //template <typename... Params>
            //ioport_builder append_ioport(Params &&... args)
            //{
            //    m_append = true;
            //    return set_ioport(std::forward<Params>(args)...);
            //}

            //template <typename... Params>
            //void set_log(device_t &devbase, Params &&... args)
            //{
            //    set_used();
            //    if (!m_append)
            //        m_target.m_creators.clear();
            //    m_target.m_creators.emplace_back(std::make_unique<log_creator>(devbase, std::string(std::forward<Params>(args)...)));
            //}

            //template <typename T, typename... Params>
            //std::enable_if_t<emu::detail::is_device_implementation<std::remove_reference_t<T> >::value> set_log(T &devbase, Params &&... args)
            //{
            //    set_log(static_cast<device_t &>(devbase), std::forward<Params>(args)...);
            //}

            //template <typename T, typename... Params>
            //std::enable_if_t<emu::detail::is_device_interface<std::remove_reference_t<T> >::value> set_log(T &devbase, Params &&... args)
            //{
            //    set_log(devbase.device(), std::forward<Params>(args)...);
            //}

            //template <typename... Params>
            //void set_log(Params &&... args)
            //{
            //    set_log(m_target.owner().mconfig().current_device(), std::forward<Params>(args)...);
            //}

            //template <typename... Params>
            //void append_log(Params &&... args)
            //{
            //    m_append = true;
            //    set_log(std::forward<Params>(args)...);
            //}

            //auto set_constant(Result val) { return set([val] () { return val; }); }
            //auto append_constant(Result val) { return append([val] () { return val; }); }


            void set_used() { assert(!m_used); m_used = true; }
        }


        //template <unsigned Count>
        public class array<unsigned_Count> : devcb_read_base.array<devcb_read<Type_Result, devcb_value_const_unsigned_DefaultMask>, unsigned_Count>  //class array : public devcb_read_base::array<devcb_read<Result, DefaultMask>, Count>
            where unsigned_Count : u64_const, new()
        {
            //using devcb_read_base::array<devcb_read<Result, DefaultMask>, Count>::array;


            public array(device_t owner, Func<devcb_read<Type_Result, devcb_value_const_unsigned_DefaultMask>> add_function) : base(owner, add_function) { }


            void resolve_all_safe(devcb_value dflt)  //void resolve_all_safe(Result dflt)
            {
                foreach (var elem in this)  //for (devcb_read<Result, DefaultMask> &elem : *this)
                    elem.resolve_safe(dflt);
            }

            public void resolve_all_safe_u8(u8 dflt) { resolve_all_safe(new devcb_value(dflt.GetType(), dflt)); }
            public void resolve_all_safe_u16(u16 dflt) { resolve_all_safe(new devcb_value(dflt.GetType(), dflt)); }
            public void resolve_all_safe_s32(s32 dflt) { resolve_all_safe(new devcb_value(dflt.GetType(), dflt)); }
        }


        std.vector<func_t> m_functions = new std.vector<func_t>();
        std.vector<creator> m_creators = new std.vector<creator>();  //std::vector<typename creator::ptr> m_creators;


        //template <typename Result, std::make_unsigned_t<Result> DefaultMask>
        public devcb_read(device_t owner)  //devcb_read<Result, DefaultMask>::devcb_read(device_t &owner)
            : base(owner)
        {
        }


        //template <typename Result, std::make_unsigned_t<Result> DefaultMask>
        public binder bind()
        {
            return new binder(this);
        }


        //void reset();


        protected override void validity_check(validity_checker valid)  //virtual void validity_check(validity_checker &valid) const override;
        {
            throw new emu_unimplemented();
        }


        //template <typename Result, std::make_unsigned_t<Result> DefaultMask>
        public void resolve()
        {
            assert(m_functions.empty());
            m_functions.reserve(m_creators.size());
            foreach (var c in m_creators)  //for (typename creator::ptr const &c : m_creators)
                m_functions.emplace_back(c.create());
            m_creators.clear();
        }


        //template <typename Result, std::make_unsigned_t<Result> DefaultMask>
        public void resolve_safe(devcb_value dflt)  //void devcb_read<Result, DefaultMask>::resolve_safe(Result dflt)
        {
            resolve();
            if (m_functions.empty())
                m_functions.emplace_back((offs_t offset, devcb_value mem_mask) => { return dflt; });  //m_functions.emplace_back([dflt] (offs_t offset, std::make_unsigned_t<Result> mem_mask) { return dflt; });
        }

        public void resolve_safe_u8(u8 dflt) { resolve_safe(new devcb_value(dflt.GetType(), dflt)); }
        public void resolve_safe_s32(s32 dflt) { resolve_safe(new devcb_value(dflt.GetType(), dflt)); }


        //Result operator()(offs_t offset, std::make_unsigned_t<Result> mem_mask = DefaultMask);
        public devcb_value op(offs_t offset) { return op(offset, DefaultMask); }

        //template <typename Result, std::make_unsigned_t<Result> DefaultMask>
        public devcb_value op(offs_t offset, devcb_value mem_mask)  //Result devcb_read<Result, DefaultMask>::operator()(offs_t offset, std::make_unsigned_t<Result> mem_mask)
        {
            assert(m_creators.empty() && !m_functions.empty());
            //typename std::vector<func_t>::const_iterator it(m_functions.begin());
            //std::make_unsigned_t<Result> result((*it)(offset, mem_mask));
            //while (m_functions.end() != ++it)
            //    result |= (*it)(offset, mem_mask);
            devcb_value result = new devcb_value(devcb_value.make_unsigned(Result), 0);
            foreach (var it in m_functions)
                result |= it(offset, mem_mask);
            return result;
        }

        public u8 op_u8(offs_t offset) { return op(offset).u8; }
        public u8 op_u8(offs_t offset, u8 mem_mask) { return op(offset, new devcb_value(mem_mask.GetType(), mem_mask)).u8; }
        public u16 op_u16(offs_t offset) { return op(offset).u16; }


        //Result operator()();
        //template <typename Result, std::make_unsigned_t<Result> DefaultMask>
        public devcb_value op()  //Result devcb_read<Result, DefaultMask>::operator()()
        {
            return this.op(0U, DefaultMask);  //return this->operator()(0U, DefaultMask);
        }

        public u8 op_u8() { return op().u8; }
        public s32 op_s32() { return op().s32; }


        public bool isnull() { return m_functions.empty() && m_creators.empty(); }


        public bool bool_ { get { return !m_functions.empty(); } }  //explicit operator bool() const { return !m_functions.empty(); }
    }


    /// \brief Write callback helper
    ///
    /// Allows binding a variety of signatures, sending the value to
    /// multiple callbacks, and chained arbitrary transforms.  Transforms
    /// may modify the offset and mask.
    public class devcb_write<Type_Input> : devcb_write<Type_Input, devcb_value_const_unsigned_MaxValue<Type_Input>>
        where Type_Input : Type_constant, new()
    {
        public devcb_write(device_t owner) : base(owner) { }
    }


    //template <typename Input, std::make_unsigned_t<Input> DefaultMask = std::make_unsigned_t<Input>(~std::make_unsigned_t<Input>(0))>
    public class devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask> : devcb_write_base, IResolve  //class devcb_write : public devcb_write_base
        where Type_Input : Type_constant, new()
        where devcb_value_const_unsigned_DefaultMask : devcb_value_const, new()
    {
        static readonly Type Input = new Type_Input().value;
        static readonly devcb_value DefaultMask = new devcb_value_const_unsigned_DefaultMask().value;

        public class make_unsigned_Type_Input : Type_constant { public Type value { get { return devcb_value.make_unsigned(Input); } } }


        public delegate void func_t(offs_t offset, devcb_value data, devcb_value mem_mask);  //using func_t = std::function<void (offs_t, Input, std::make_unsigned_t<Input>)>;


        protected abstract class creator
        {
            //using ptr = std::unique_ptr<creator>;

            //~creator() { }

            protected abstract void validity_check(validity_checker valid);


            public abstract func_t create();
        }


        public interface creator_impl_builder
        {
            Action<offs_t, devcb_value, devcb_value> build();
        }


        //template <typename T>
        protected class creator_impl<T> : creator
            where T : creator_impl_builder
        {
            T m_builder;


            public creator_impl(T builder) { m_builder = builder; }


            protected override void validity_check(validity_checker valid) { throw new emu_unimplemented(); }


            public override func_t create()
            {
                var cb = m_builder.build();
                return (offs_t offset, devcb_value data, devcb_value mem_mask) => { cb(offset, data, mem_mask); };  //return [cb = m_builder.build()] (offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask) { cb(offset, data, mem_mask); };
            }
        }


        class nop_creator : creator
        {
            protected override void validity_check(validity_checker valid) { }


            public override func_t create() { return (offs_t offset, devcb_value data, devcb_value mem_mask) => { }; }
        }


        public abstract class builder_base : creator_impl_builder, IDisposable
        {
            bool m_disposedValue = false; // To detect redundant calls


            protected devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask> m_target;
            protected bool m_append;
            protected bool m_consumed = false;
            bool m_built = false;


            public builder_base(devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask> target, bool append) { m_target = target;  m_append = append; }

            public builder_base(builder_base that) { m_disposedValue = that.m_disposedValue; m_target = that.m_target; m_append = that.m_append; m_consumed = that.m_consumed; m_built = that.m_built; }

            ~builder_base()
            {
                assert(m_consumed, string.Format("~builder_base() - {0} - {1} - {2}\n", m_target.owner().owner().GetType(), m_target.owner().tag(), m_target.owner().name()));
            }


            protected virtual void Dispose(bool disposing)
            {
                if (!m_disposedValue)
                {
                    if (disposing)
                    {
                        // dispose managed state (managed objects).
                    }

                    // free unmanaged resources (unmanaged objects) and override a finalizer below.
                    assert(m_consumed);

                    m_disposedValue = true;
                }
            }

            // calls register_creator().  do this here instead of in the dtor
            public void Dispose() { Dispose(true); }
            public void reg() { Dispose(); }


            //builder_base &operator=(builder_base const &) = delete;
            //builder_base &operator=(builder_base &&) = default;


            public abstract Action<offs_t, devcb_value, devcb_value> build();


            public void consume() { m_consumed = true; }
            protected void built() { assert(!m_built); m_built = true; }


            //template <typename T>
            protected void register_creator<T>()
                where T : creator_impl_builder
            {
                if (!m_consumed)
                {
                    if (!m_append)
                        m_target.m_creators.clear();

                    consume();

                    m_target.m_creators.emplace_back(new creator_impl<T>((T)(creator_impl_builder)this));  //m_target.m_creators.emplace_back(std::make_unique<creator_impl<T> >(std::move(static_cast<T &>(*this))));
                }
            }
        }


        public interface builder_base_interface
        {
            builder_base_interface mask_internal(devcb_value val);
            builder_base_interface invert_interface();
            void reg();
        }


        public abstract class builder_base_with_transform_base<Type_T, Impl, Impl_OPS, Impl_output_t> : builder_base, builder_base_interface, IDisposable
            where Type_T : Type_constant, new()
            where Impl : builder_base
            where Impl_OPS : io_operations<Impl>, new()
            where Impl_output_t : Type_constant, new()
        {
            static readonly Type T = new Type_T().value;
            static readonly Impl_OPS ops = new Impl_OPS();


            public delegate devcb_value transform_function_delegate(offs_t offset, devcb_value data, ref devcb_value mem_mask);


            transform_base<Type_T, Impl> m_transform_base;


            protected builder_base_with_transform_base(devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask> target, bool append, devcb_value mask) : base(target, append)
            {
                m_transform_base = new transform_base<Type_T, Impl>(new devcb_value(devcb_value.make_unsigned(T), mask));
            }


            Impl exor(devcb_value val) { return ops.cast((builder_base)exor_internal(val)); }  //Impl &exor(std::make_unsigned_t<T> val) { m_exor ^= val; return static_cast<Impl &>(*this); }
            builder_base_interface exor_internal(devcb_value val) { m_transform_base.m_exor ^= val; return this; }  //Impl &exor(std::make_unsigned_t<T> val) { m_exor ^= val; return static_cast<Impl &>(*this); }
            public Impl mask(devcb_value val) { return ops.cast((builder_base)mask_internal(val)); }  //Impl &mask(std::make_unsigned_t<T> val) { m_mask = m_inherited_mask ? val : (m_mask & val); m_inherited_mask = false; return static_cast<Impl &>(*this); }
            public builder_base_interface mask_internal(devcb_value val) { m_transform_base.m_mask = m_transform_base.m_inherited_mask ? val : (m_transform_base.m_mask & val); m_transform_base.m_inherited_mask = false; return this; }  //Impl &mask(std::make_unsigned_t<T> val) { m_mask = m_inherited_mask ? val : (m_mask & val); m_inherited_mask = false; return static_cast<Impl &>(*this); }
            public Impl mask_u8(u8 val) { return mask(new devcb_value(val.GetType(), val)); }

            public Impl invert() { return ops.cast((builder_base)invert_interface()); }  //Impl &invert() { return exor(~std::make_unsigned_t<T>(0)); }
            public builder_base_interface invert_interface() { return exor_internal(new devcb_value(devcb_value.make_unsigned(T), u64.MaxValue)); }  //Impl &invert() { return exor(~std::make_unsigned_t<T>(0)); }


            public builder_base_interface rshift(unsigned val)  //auto rshift(unsigned val)
            {
                var trans = this.transform((offs_t offset, devcb_value data, ref devcb_value mem_mask) => { mem_mask >>= (int)val; return data >> (int)val; });  //auto trans(static_cast<Impl &>(*this).transform([val] (offs_t offset, T data, std::make_unsigned_t<T> &mem_mask) { mem_mask >>= val; return data >> val; }));
                if (inherited_mask())
                    return trans;
                else
                    return trans.mask_internal(m_transform_base.m_mask >> (int)val);  //return std::move(trans.mask(m_mask >> val));
            }


            public builder_base_interface lshift(unsigned val)  //auto lshift(unsigned val)
            {
                throw new emu_unimplemented();
            }


            public virtual builder_base_interface transform(transform_function_delegate cb) { throw new emu_unimplemented(); }


            public builder_base_interface bit(unsigned val)  //auto bit(unsigned val) { return std::move(rshift(val).mask(T(1U))); }
            {
                return rshift(val).mask_internal(new devcb_value(T, 1U));
            }


            protected devcb_value exor() { return m_transform_base.exor(); }
            protected devcb_value mask() { return m_transform_base.mask(); }


            //constexpr bool need_exor() const { return std::make_unsigned_t<T>(0) != (m_exor & m_mask); }
            //constexpr bool need_mask() const { return std::make_unsigned_t<T>(~std::make_unsigned_t<T>(0)) != m_mask; }

            protected bool inherited_mask() { return m_transform_base.m_inherited_mask; }
        }


        public interface io_operations<Impl>
        {
            Type input_t { get; }
            Type output_t { get; }
            Type input_mask_t { get; }

            Impl cast(builder_base base_);
        }


        //template <typename Source, typename Func>
        public class transform_builder<Source, Source_OPS, Source_output_t, Func_> :  //class transform_builder : public builder_base, public transform_base<mask_t<transform_result_t<typename Source::output_t, typename Source::output_t, Func>, typename Source::output_t>, transform_builder<Source, Func> >
                    builder_base_with_transform_base<Type_constant_mask_t<Type_constant_transform_result_t<Source_output_t, Source_output_t, Func_>, Source_output_t>,
                                                     transform_builder<Source, Source_OPS, Source_output_t, Func_>,
                                                     transform_builder<Source, Source_OPS, Source_output_t, Func_>.io_ops_transform_builder, 
                                                     transform_builder<Source, Source_OPS, Source_output_t, Func_>.Type_constant_output_t>,
                    IDisposable
            where Source : builder_base
            where Source_OPS : io_operations<Source>, new()
            where Source_output_t : Type_constant, new()
            where Func_ : Delegate
        {
            //template <typename T, typename U> friend class transform_builder;

            //using input_t = typename Source::output_t;
            //using output_t = mask_t<transform_result_t<typename Source::output_t, typename Source::output_t, Func>, typename Source::output_t>;
            public class io_ops_transform_builder : io_operations<transform_builder<Source, Source_OPS, Source_output_t, Func_>>
            {
                static readonly Source_OPS source_ops = new Source_OPS();
                class Type_constant_source_output_t : Type_constant { public Type value { get { return source_ops.output_t; } } }
                static readonly Type output_t_Type = new Type_constant_mask_t<Type_constant_transform_result_t<Type_constant_source_output_t, Type_constant_source_output_t, Func_>, Type_constant_source_output_t>().value;

                public Type input_t { get { return source_ops.output_t; } }
                public Type output_t { get { return output_t_Type; } }
                public Type input_mask_t { get { throw new emu_unimplemented(); } }

                public transform_builder<Source, Source_OPS, Source_output_t, Func_> cast(builder_base base_) { throw new emu_unimplemented(); }
            }

            static readonly io_ops_transform_builder io_ops = new io_ops_transform_builder();

            public class Type_constant_output_t : Type_constant { public Type value { get { throw new emu_unimplemented(); } } }


            Source m_src;
            Func_ m_cb;


            //template <typename T>
            public transform_builder(devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask> target, bool append, Source src, Func_ cb, devcb_value mask)  //transform_builder(devcb_write &target, bool append, Source &&src, T &&cb, output_t mask)
                : base(target, append, new devcb_value(io_ops.output_t, mask))
            {
                //, transform_base<output_t, transform_builder>(mask)


                m_src = src;
                m_cb = cb;


                m_src.consume();
            }

            //transform_builder(transform_builder &&that)
            //    : builder_base(std::move(that))
            //    , transform_base<output_t, transform_builder>(std::move(that))
            //    , m_src(std::move(that.m_src))
            //    , m_cb(std::move(that.m_cb))
            //{
            //    m_src.consume();
            //    that.consume();
            //    that.built();
            //}

            //~transform_builder() { this->template register_creator<transform_builder>(); }
            protected override void Dispose(bool disposing) { register_creator<transform_builder<Source, Source_OPS, Source_output_t, Func_>>(); base.Dispose(disposing); }


            //template <typename T>
            public override builder_base_interface transform(transform_function_delegate cb)  //std::enable_if_t<is_transform<output_t, output_t, T>::value, transform_builder<transform_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                throw new emu_unimplemented();
            }


            public override Action<offs_t, devcb_value, devcb_value> build()  //auto build()
            {
                throw new emu_unimplemented();
            }


            //void validity_check(validity_checker &valid) const { m_src.validity_check(valid); }


            //transform_builder(transform_builder const &) = delete;
            //transform_builder &operator=(transform_builder const &) = delete;
            //transform_builder &operator=(transform_builder &&that) = delete;


            //template <typename T>
            //auto build(T &&chain)
            //{
            //    assert(this->m_consumed);
            //    this->built();
            //    return m_src.build(
            //            [f = std::move(chain), cb = std::move(m_cb), exor = this->exor(), mask = this->mask()] (offs_t &offset, input_t data, std::make_unsigned_t<input_t> &mem_mask)
            //            {
            //                auto const trans(devcb_write::invoke_transform<input_t, output_t>(cb, offset, data, mem_mask));
            //                output_t out_mask(mem_mask & mask);
            //                return f(offset, (trans ^ exor) & mask, out_mask);
            //            });
            //}
        }


        //template <typename Sink, typename Func>
        public class first_transform_builder<Sink, Sink_input_t, Func_> :  //class first_transform_builder : public builder_base, public transform_base<mask_t<transform_result_t<typename Sink::input_t, typename Sink::input_t, Func>, typename Sink::input_t>, first_transform_builder<Sink, Func> >
                    builder_base_with_transform_base<Type_constant_mask_t<Type_constant_transform_result_t<first_transform_builder<Sink, Sink_input_t, Func_>.Type_constant_Sink_input_t, 
                                                                                                           first_transform_builder<Sink, Sink_input_t, Func_>.Type_constant_Sink_input_t,
                                                                                                           Func_>,
                                                                          first_transform_builder<Sink, Sink_input_t, Func_>.Type_constant_Sink_input_t>,
                                                     first_transform_builder<Sink, Sink_input_t, Func_>,
                                                     first_transform_builder<Sink, Sink_input_t, Func_>.io_ops_first_transform_builder,
                                                     first_transform_builder<Sink, Sink_input_t, Func_>.Type_constant_output_t>,
                    IDisposable
            where Sink : builder_base
            where Sink_input_t : Type_constant, new()
        {
            //template <typename T, typename U> friend class transform_builder;


            //using input_t = typename Sink::input_t;
            //using output_t = mask_t<transform_result_t<typename Sink::input_t, typename Sink::input_t, Func>, typename Sink::input_t>;
            public class io_ops_first_transform_builder : io_operations<first_transform_builder<Sink, Sink_input_t, Func_>>
            {
                class Type_constant_sink_input_t : Type_constant { public Type value { get { return new Type_constant_Sink_input_t().value; } } }
                static readonly Type output_t_Type = new Type_constant_mask_t<Type_constant_transform_result_t<Type_constant_sink_input_t, Type_constant_sink_input_t, Func_>, Type_constant_sink_input_t>().value;

                public Type input_t { get { throw new emu_unimplemented(); } }
                public Type output_t { get { return output_t_Type; } }
                public Type input_mask_t { get { throw new emu_unimplemented(); } }

                public first_transform_builder<Sink, Sink_input_t, Func_> cast(builder_base base_) { throw new emu_unimplemented(); }
            }

            static readonly io_ops_first_transform_builder io_ops = new io_ops_first_transform_builder();

            public class Type_constant_output_t : Type_constant { public Type value { get { throw new emu_unimplemented(); } } }
            public class Type_constant_Sink_input_t : Type_constant { public Type value { get { return new Sink_input_t().value; } } }


            Sink m_sink;
            Func_ m_cb;
            devcb_value m_in_exor;
            devcb_value m_in_mask;


            public first_transform_builder(devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask> target, bool append, Sink sink, Func_ cb, devcb_value in_exor, devcb_value in_mask, devcb_value mask)  //first_transform_builder(devcb_write &target, bool append, Sink &&sink, T &&cb, std::make_unsigned_t<Input> in_exor, std::make_unsigned_t<Input> in_mask, std::make_unsigned_t<output_t> mask)
                : base(target, append, new devcb_value(io_ops.output_t, mask))
            {
                //, transform_base<output_t, first_transform_builder>(mask)


                m_sink = sink;
                m_cb = cb;
                m_in_exor = in_exor & in_mask;
                m_in_mask = in_mask;


                m_sink.consume();
            }

            //first_transform_builder(first_transform_builder &&that)
            //    : builder_base(std::move(that))
            //    , transform_base<output_t, first_transform_builder>(std::move(that))
            //    , m_sink(std::move(that.m_sink))
            //    , m_cb(std::move(that.m_cb))
            //    , m_in_exor(that.m_in_exor)
            //    , m_in_mask(that.m_in_mask)
            //{
            //    m_sink.consume();
            //    that.consume();
            //    that.built();
            //}

            //~first_transform_builder() { this->template register_creator<first_transform_builder>(); }
            protected override void Dispose(bool disposing) { register_creator<first_transform_builder<Sink, Sink_input_t, Func_>>(); base.Dispose(disposing); }


            //void validity_check(validity_checker &valid) const { m_sink.validity_check(valid); }


            //template <typename T>
            public override builder_base_interface transform(transform_function_delegate cb)  //std::enable_if_t<is_transform<output_t, output_t, T>::value, transform_builder<first_transform_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                throw new emu_unimplemented();
            }


            public override Action<offs_t, devcb_value, devcb_value> build()  //auto build()
            {
                assert(this.m_consumed);
                this.built();

                var sink = m_sink.build();
                var cb = this.m_cb;
                var in_exor = m_in_exor;
                var in_mask = m_in_mask;
                var exor = this.exor();
                var mask = this.mask();
                throw new emu_unimplemented();
            }


            //first_transform_builder(first_transform_builder const &) = delete;
            //first_transform_builder operator=(first_transform_builder const &) = delete;
            //first_transform_builder operator=(first_transform_builder &&that) = delete;


            //template <typename T>
            //auto build(T &&chain)
            //{
            //    assert(this->m_consumed);
            //    this->built();
            //    return
            //            [f = std::move(chain), sink = m_sink.build(), cb = std::move(m_cb), in_exor = m_in_exor, in_mask = m_in_mask, exor = this->exor(), mask = this->mask()] (offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
            //            {
            //                data = (data ^ in_exor) & in_mask;
            //                mem_mask &= in_mask;
            //                auto const trans_1(devcb_write::invoke_transform<input_t, output_t>(cb, offset, data, mem_mask));
            //                output_t out_mask(mem_mask & mask);
            //                auto const trans_n(f(offset, (trans_1 ^ exor) & mask, out_mask));
            //                sink(offset, trans_n, out_mask);
            //            };
            //}
        }


        //template <typename Func>
        //class functoid_builder : public builder_base, public transform_base<std::make_unsigned_t<Input>, functoid_builder<Func> >


        //template <typename Delegate>
        public class delegate_builder<Delegate_> :  //class delegate_builder : public builder_base, public transform_base<mask_t<Input, typename delegate_traits<Delegate>::input_t>, delegate_builder<Delegate> >
                    builder_base_with_transform_base<Type_constant_mask_t<Type_Input, delegate_traits<Delegate_>.input_t>, 
                                                     delegate_builder<Delegate_>,
                                                     delegate_builder<Delegate_>.io_ops_delegate_builder,
                                                     delegate_builder<Delegate_>.Type_constant_output_t>,
                    creator_impl_builder, 
                    IDisposable
            where Delegate_ : Delegate
        {
            public class wrapped_builder : builder_base
            {
                //template <typename T, typename U> friend class first_transform_builder;


                //using input_t = intermediate_t<Input, typename delegate_traits<Delegate>::input_t>;
                public class Type_constant_input_t : Type_constant
                {
                    static readonly Type T = new Type_Input().value;
                    static readonly Type U = new delegate_traits<Delegate_>.input_t().value;

                    Type intermediate_t { get { return sizeof_(T) >= sizeof_(U) ? T : U; } }

                    public Type value { get { return intermediate_t; } }
                }


                Delegate_ m_delegate;


                public wrapped_builder(delegate_builder<Delegate_> that)
                    : base(that)
                {
                    m_delegate = that.m_delegate;


                    that.consume();
                    that.built();
                }

                wrapped_builder(wrapped_builder that)
                    : base(that)
                {
                    m_delegate = that.m_delegate;


                    that.consume();
                    that.built();
                }


                //void validity_check(validity_checker &valid) const
                //{
                //    auto const target(m_delegate.finder_target());
                //    if (target.second && !target.first.subdevice(target.second))
                //        osd_printf_error("Write callback bound to non-existent object tag %s (%s)\n", target.first.subtag(target.second), m_delegate.name());
                //}


                public override Action<offs_t, devcb_value, devcb_value> build()  //auto build()
                {
                    throw new emu_unimplemented();
                }


                //wrapped_builder(wrapped_builder const &) = delete;
                //wrapped_builder operator=(wrapped_builder const &) = delete;
                //wrapped_builder operator=(wrapped_builder &&that) = delete;
            }


            //delegate_builder(delegate_builder const &) = delete;
            //delegate_builder &operator=(delegate_builder const &) = delete;
            //delegate_builder &operator=(delegate_builder &&that) = delete;


            Delegate_ m_delegate;


            //using input_t = intermediate_t<Input, typename delegate_traits<Delegate>::input_t>;
            public class io_ops_delegate_builder : io_operations<delegate_builder<Delegate_>>
            {
                public Type input_t { get { throw new emu_unimplemented(); } }
                public Type output_t { get { throw new emu_unimplemented(); } }
                public Type input_mask_t { get { throw new emu_unimplemented(); } }

                public delegate_builder<Delegate_> cast(builder_base base_) { return (delegate_builder<Delegate_>)base_; }
            }

            public class Type_constant_output_t : Type_constant { public Type value { get { throw new emu_unimplemented(); } } }


            //template <typename T>
            public delegate_builder(devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask> target, bool append, device_t devbase, string tag, Delegate_ func)  //delegate_builder(devcb_write &target, bool append, device_t &devbase, char const *tag, T &&func, char const *name)
                : base(target, append, new devcb_value(new Type_constant_mask_t<Type_Input, delegate_traits<Delegate_>.input_t>().value, DefaultMask & delegate_traits<Delegate_>.default_mask))
            {
                //transform_base<mask_t<Input, typename delegate_traits<Delegate>::input_t>, delegate_builder>(DefaultMask & delegate_traits<Delegate>::default_mask)


                m_delegate = func;  //, m_delegate(devbase, tag, std::forward<T>(func), name)
            }

            //template <typename T>
            //delegate_builder(devcb_write &target, bool append, device_t &devbase, devcb_write::delegate_device_class_t<T> &obj, T &&func, char const *name)
            //    : builder_base(target, append)
            //    , transform_base<mask_t<Input, typename delegate_traits<Delegate>::input_t>, delegate_builder>(DefaultMask & delegate_traits<Delegate>::default_mask)
            //    , m_delegate(obj, std::forward<T>(func), name)
            //{ }
            //delegate_builder(delegate_builder &&that)
            //    : builder_base(std::move(that))
            //    , transform_base<mask_t<Input, typename delegate_traits<Delegate>::input_t>, delegate_builder>(std::move(that))
            //    , m_delegate(std::move(that.m_delegate))
            //{
            //    that.consume();
            //    that.built();
            //}

            //~delegate_builder() { this->template register_creator<delegate_builder>(); }
            protected override void Dispose(bool disposing) { register_creator<delegate_builder<Delegate_>>(); base.Dispose(disposing); }


            //template <typename T>
            public override builder_base_interface transform(transform_function_delegate cb)  //std::enable_if_t<is_transform<input_t, input_t, T>::value, first_transform_builder<wrapped_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                devcb_value in_mask = this.inherited_mask() ? DefaultMask : this.mask();  //std::make_unsigned_t<Input> const in_mask(this->inherited_mask() ? DefaultMask : this->mask());
                devcb_value out_mask = DefaultMask & delegate_traits<Delegate_>.default_mask;  //mask_t<Input, typename delegate_traits<Delegate>::input_t> const out_mask(DefaultMask & delegate_traits<Delegate>::default_mask);
                return new first_transform_builder<wrapped_builder, wrapped_builder.Type_constant_input_t, transform_function_delegate>(this.m_target, this.m_append, new wrapped_builder(this), cb, this.exor(), in_mask, out_mask);  //return first_transform_builder<wrapped_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, wrapped_builder(std::move(*this)), std::forward<T>(cb), this->exor(), in_mask, out_mask);
            }


            //void validity_check(validity_checker &valid) const
            //{
            //    if (!m_devbase.subdevice(m_delegate.device_name()))
            //        osd_printf_error("Write callback bound to non-existent object tag %s (%s)\n", m_delegate.device_name(), m_delegate.name());
            //}


            public override Action<offs_t, devcb_value, devcb_value> build()  //auto build()
            {
                assert(this.m_consumed);
                this.built();
                //m_delegate.resolve();
                var cb = this.m_delegate;
                var exor = this.exor();
                var mask = this.mask();
                return
                        (offs_t offset, devcb_value data, devcb_value mem_mask) =>  //[cb = std::move(this->m_delegate), exor = this->exor(), mask = this->mask()] (offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { invoke_write<Type_Input, Delegate_>(cb, offset, (data ^ exor) & mask, mem_mask & mask); };  //{ devcb_write::invoke_write<Input>(cb, offset, (data ^ exor) & mask, mem_mask & mask); };
            }
        }


        public class inputline_builder :  //class inputline_builder : public builder_base, public transform_base<mask_t<Input, int>, inputline_builder>
                    builder_base_with_transform_base<Type_constant_mask_t<Type_Input, Type_constant_s32>, 
                                                     inputline_builder,
                                                     inputline_builder.io_ops_inputline_builder,
                                                     inputline_builder.Type_constant_output_t>,
                    creator_impl_builder
        {
            public class wrapped_builder : builder_base
            {
                //template <typename T, typename U> friend class first_transform_builder;

                //using input_t = intermediate_t<Input, int>;


                device_t m_devbase;
                string m_tag;
                device_execute_interface m_exec;
                int m_linenum;


                wrapped_builder(inputline_builder that)
                    : base(that)
                {
                    m_devbase = that.m_devbase;
                    m_tag = that.m_tag;
                    m_exec = that.m_exec;
                    m_linenum = that.m_linenum;


                    that.consume();
                    that.built();
                }

                wrapped_builder(wrapped_builder that)
                    : base(that)
                {
                    m_devbase = that.m_devbase;
                    m_tag = that.m_tag;
                    m_exec = that.m_exec;
                    m_linenum = that.m_linenum;


                    that.consume();
                    that.built();
                }


                //void validity_check(validity_checker &valid) const
                //{
                //    if (!m_exec)
                //    {
                //        device_t *const device(m_devbase.subdevice(m_tag));
                //        if (!device)
                //            osd_printf_error("Write callback bound to non-existent object tag %s\n", m_tag);
                //        else if (!dynamic_cast<device_execute_interface *>(device))
                //            osd_printf_error("Write callback bound to device %s (%s) that does not implement device_execute_interface\n", device->tag(), device->name());
                //    }
                //}


                public override Action<offs_t, devcb_value, devcb_value> build()  //auto build()
                {
                    assert(this.m_consumed);
                    this.built();
                    if (m_exec == null)
                    {
                        device_t device = m_devbase.subdevice(m_tag);
                        if (device == null)
                            throw new emu_fatalerror("Write callback bound to non-existent object tag {0}\n", m_tag);

                        m_exec = device.GetClassInterface<device_execute_interface>();  //m_exec = dynamic_cast<device_execute_interface *>(device);
                        if (m_exec == null)
                            throw new emu_fatalerror("Write callback bound to device {0} ({1}) that does not implement device_execute_interface\n", device.tag(), device.name());
                    }

                    var exec = m_exec;
                    var linenum = m_linenum;
                    throw new emu_unimplemented();
                }


                //wrapped_builder(wrapped_builder const &) = delete;
                //wrapped_builder operator=(wrapped_builder const &) = delete;
                //wrapped_builder operator=(wrapped_builder &&that) = delete;
            }



            //inputline_builder(inputline_builder const &) = delete;
            //inputline_builder &operator=(inputline_builder const &) = delete;
            //inputline_builder &operator=(inputline_builder &&that) = delete;


            device_t m_devbase;
            string m_tag;
            device_execute_interface m_exec;
            int m_linenum;


            //using input_t = intermediate_t<Input, int>;
            public class io_ops_inputline_builder : io_operations<inputline_builder>
            {
                public Type input_t { get { throw new emu_unimplemented(); } }
                public Type output_t { get { throw new emu_unimplemented(); } }
                public Type input_mask_t { get { throw new emu_unimplemented(); } }

                public inputline_builder cast(builder_base base_) { return (inputline_builder)base_; }
            }

            public class Type_constant_output_t : Type_constant { public Type value { get { throw new emu_unimplemented(); } } }


            public inputline_builder(devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask> target, bool append, device_t devbase, string tag, int linenum)
                : base(target, append, new devcb_value(new Type_constant_mask_t<Type_Input, Type_constant_s32>().value, 1U))
            {
                //transform_base<mask_t<Input, int>, inputline_builder>(1U)


                m_devbase = devbase;
                m_tag = tag;
                m_exec = null;
                m_linenum = linenum;
            }

            //inputline_builder(devcb_write &target, bool append, device_execute_interface &exec, int linenum)
            //    : builder_base(target, append)
            //    , transform_base<mask_t<Input, int>, inputline_builder>(1U)
            //    , m_devbase(exec.device())
            //    , m_tag(exec.device().tag())
            //    , m_exec(&exec)
            //    , m_linenum(linenum)
            //{ }

            //inputline_builder(inputline_builder &&that)
            //    : builder_base(std::move(that))
            //    , transform_base<mask_t<Input, int>, inputline_builder>(std::move(that))
            //    , m_devbase(that.m_devbase)
            //    , m_tag(that.m_tag)
            //    , m_exec(that.m_exec)
            //    , m_linenum(that.m_linenum)
            //{
            //    that.consume();
            //    that.built();
            //}

            //~inputline_builder() { this->template register_creator<inputline_builder>(); }
            protected override void Dispose(bool disposing) { register_creator<inputline_builder>(); base.Dispose(disposing); }


            //template <typename T>
            public override builder_base_interface transform(transform_function_delegate cb)  //std::enable_if_t<is_transform<input_t, input_t, T>::value, first_transform_builder<wrapped_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                throw new emu_unimplemented();
            }


            //void validity_check(validity_checker &valid) const
            //{
            //    if (!m_exec)
            //    {
            //        device_t *const device(m_devbase.subdevice(m_tag));
            //        if (!device)
            //            osd_printf_error("Write callback bound to non-existent object tag %s\n", m_tag);
            //        else if (!dynamic_cast<device_execute_interface *>(device))
            //            osd_printf_error("Write callback bound to device %s (%s) that does not implement device_execute_interface\n", device->tag(), device->name());
            //    }
            //}


            public override Action<offs_t, devcb_value, devcb_value> build()  //auto build()
            {
                assert(this.m_consumed);
                this.built();
                if (m_exec == null)
                {
                    device_t device = m_devbase.subdevice(m_tag);
                    if (device == null)
                        throw new emu_fatalerror("Write callback bound to non-existent object tag {0}\n", m_tag);
                    m_exec = device.GetClassInterface<device_execute_interface>();  //m_exec = dynamic_cast<device_execute_interface *>(device);
                    if (m_exec == null)
                        throw new emu_fatalerror("Write callback bound to device {0} ({1}) that does not implement device_execute_interface\n", device.tag(), device.name());
                }

                var exec = m_exec;
                var linenum = m_linenum;
                var exor = this.exor();
                var mask = this.mask();
                return
                        (offs_t offset, devcb_value data, devcb_value mem_mask) =>  //[&exec = *m_exec, linenum = m_linenum, exor = this->exor(), mask = this->mask()] (offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { exec.set_input_line(linenum, ((data ^ exor) & mask).s32); };  //{ exec.set_input_line(linenum, (data ^ exor) & mask); };
            }
        }


        public class latched_inputline_builder : //class latched_inputline_builder : public builder_base, public transform_base<std::make_unsigned_t<Input>, latched_inputline_builder>
                    builder_base_with_transform_base<make_unsigned_Type_Input, 
                                                     latched_inputline_builder,
                                                     latched_inputline_builder.io_ops_latched_inputline_builder,
                                                     latched_inputline_builder.Type_constant_output_t>,
                    creator_impl_builder
        {
            class wrapped_builder : builder_base
            {
                //template <typename T, typename U> friend class first_transform_builder;

                //using input_t = Input;


                //device_t &m_devbase;
                //char const *const m_tag;
                //device_execute_interface *m_exec;
                //int const m_linenum;
                //int const m_value;


                wrapped_builder(latched_inputline_builder that)
                    : base(that)
                {
                    throw new emu_unimplemented();
#if false
                    , m_devbase(that.m_devbase)
                    , m_tag(that.m_tag)
                    , m_exec(that.m_exec)
                    , m_linenum(that.m_linenum)
                    , m_value(that.m_value)


                    that.consume();
                    that.built();
#endif
                }

                wrapped_builder(wrapped_builder that)
                    : base(that)
                {
                    throw new emu_unimplemented();
#if false
                    , m_devbase(that.m_devbase)
                    , m_tag(that.m_tag)
                    , m_exec(that.m_exec)
                    , m_linenum(that.m_linenum)
                    , m_value(that.m_value)


                    that.consume();
                    that.built();
#endif
                }

                //void validity_check(validity_checker &valid) const
                //{
                //    if (!m_exec)
                //    {
                //        device_t *const device(m_devbase.subdevice(m_tag));
                //        if (!device)
                //            osd_printf_error("Write callback bound to non-existent object tag %s\n", m_tag);
                //        else if (!dynamic_cast<device_execute_interface *>(device))
                //            osd_printf_error("Write callback bound to device %s (%s) that does not implement device_execute_interface\n", device->tag(), device->name());
                //    }
                //}

                public override Action<offs_t, devcb_value, devcb_value> build()  //auto build()
                {
                    throw new emu_unimplemented();
#if false
                    assert(this->m_consumed);
                    this->built();
                    if (!m_exec)
                    {
                        device_t *const device(m_devbase.subdevice(m_tag));
                        if (!device)
                            throw emu_fatalerror("Write callback bound to non-existent object tag %s\n", m_tag);
                        m_exec = dynamic_cast<device_execute_interface *>(device);
                        if (!m_exec)
                            throw emu_fatalerror("Write callback bound to device %s (%s) that does not implement device_execute_interface\n", device->tag(), device->name());
                    }
                    return
                            [&exec = *m_exec, linenum = m_linenum, value = m_value] (offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                            { if (data) exec.set_input_line(linenum, value); };
#endif
                }


                //wrapped_builder(wrapped_builder const &) = delete;
                //wrapped_builder operator=(wrapped_builder const &) = delete;
                //wrapped_builder operator=(wrapped_builder &&that) = delete;
            }


            //friend class wrapped_builder; // workaround for MSVC

            //latched_inputline_builder(latched_inputline_builder const &) = delete;
            //latched_inputline_builder &operator=(latched_inputline_builder const &) = delete;
            //latched_inputline_builder &operator=(latched_inputline_builder &&that) = delete;


            device_t m_devbase;
            string m_tag;
            device_execute_interface m_exec;
            int m_linenum;
            int m_value;


            //using input_t = Input;
            public class io_ops_latched_inputline_builder : io_operations<latched_inputline_builder>
            {
                public Type input_t { get { throw new emu_unimplemented(); } }
                public Type output_t { get { throw new emu_unimplemented(); } }
                public Type input_mask_t { get { throw new emu_unimplemented(); } }

                public latched_inputline_builder cast(builder_base base_) { return (latched_inputline_builder)base_; }
            }

            public class Type_constant_output_t : Type_constant { public Type value { get { throw new emu_unimplemented(); } } }


            public latched_inputline_builder(devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask> target, bool append, device_t devbase, string tag, int linenum, int value)
                : base(target, append, DefaultMask)
            {
                //transform_base<std::make_unsigned_t<Input>, latched_inputline_builder>(DefaultMask)


                m_devbase = devbase;
                m_tag = tag;
                m_exec = null;
                m_linenum = linenum;
                m_value = value;
            }

            //latched_inputline_builder(devcb_write &target, bool append, device_execute_interface &exec, int linenum, int value)
            //    : builder_base(target, append)
            //    , transform_base<std::make_unsigned_t<Input>, latched_inputline_builder>(DefaultMask)
            //    , m_devbase(exec.device())
            //    , m_tag(exec.device().tag())
            //    , m_exec(&exec)
            //    , m_linenum(linenum)
            //    , m_value(value)
            //{ }

            //latched_inputline_builder(latched_inputline_builder &&that)
            //    : builder_base(std::move(that))
            //    , transform_base<std::make_unsigned_t<Input>, latched_inputline_builder>(std::move(that))
            //    , m_devbase(that.m_devbase)
            //    , m_tag(that.m_tag)
            //    , m_exec(that.m_exec)
            //    , m_linenum(that.m_linenum)
            //    , m_value(that.m_value)
            //{
            //    that.consume();
            //    that.built();
            //}

            //~latched_inputline_builder() { this->template register_creator<latched_inputline_builder>(); }
            protected override void Dispose(bool disposing) { register_creator<latched_inputline_builder>(); base.Dispose(disposing); }


            //template <typename T>
            //std::enable_if_t<is_transform<input_t, input_t, T>::value, first_transform_builder<wrapped_builder, std::remove_reference_t<T> > > transform(T &&cb)
            //{
            //    return first_transform_builder<wrapped_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, wrapped_builder(std::move(*this)), std::forward<T>(cb), this->exor(), this->mask(), DefaultMask);
            //}

            //void validity_check(validity_checker &valid) const
            //{
            //    if (!m_exec)
            //    {
            //        device_t *const device(m_devbase.subdevice(m_tag));
            //        if (!device)
            //            osd_printf_error("Write callback bound to non-existent object tag %s\n", m_tag);
            //        else if (!dynamic_cast<device_execute_interface *>(device))
            //            osd_printf_error("Write callback bound to device %s (%s) that does not implement device_execute_interface\n", device->tag(), device->name());
            //    }
            //}

            public override Action<offs_t, devcb_value, devcb_value> build()  //auto build()
            {
                assert(this.m_consumed);
                this.built();
                if (m_exec == null)
                {
                    device_t device = m_devbase.subdevice(m_tag);
                    if (device == null)
                        throw new emu_fatalerror("Write callback bound to non-existent object tag {0}\n", m_tag);
                    m_exec = device.GetClassInterface<device_execute_interface>();  //m_exec = dynamic_cast<device_execute_interface *>(device);
                    if (m_exec == null)
                        throw new emu_fatalerror("Write callback bound to device {0} ({1}) that does not implement device_execute_interface\n", device.tag(), device.name());
                }

                var exec = m_exec;
                var linenum = m_linenum;
                var value = m_value;
                var exor = this.exor();
                var mask = this.mask();
                return
                        (offs_t offset, devcb_value data, devcb_value mem_mask) =>  //[&exec = *m_exec, linenum = m_linenum, value = m_value, exor = this->exor(), mask = this->mask()] (offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { if (((data ^ exor) & mask) != 0) exec.set_input_line(linenum, value); };  //{ if ((data ^ exor) & mask) exec.set_input_line(linenum, value); };
            }
        }


        //class ioport_builder : public builder_base, public transform_base<mask_t<Input, ioport_value>, ioport_builder>


        public class membank_builder :  //class membank_builder : public builder_base, public transform_base<mask_t<Input, int>, membank_builder>
                    builder_base_with_transform_base<Type_constant_mask_t<Type_Input, Type_constant_s32>, 
                                                     membank_builder,
                                                     membank_builder.io_ops_membank_builder, 
                                                     membank_builder.Type_constant_output_t>,
                    creator_impl_builder
        {
            class wrapped_builder : builder_base
            {
                //template <typename T, typename U> friend class first_transform_builder;

                //using input_t = intermediate_t<Input, int>;


                device_t m_devbase;
                string m_tag;


                wrapped_builder(membank_builder that)
                    : base(that)
                {
                    m_devbase = that.m_devbase;
                    m_tag = that.m_tag;


                    that.consume();
                    that.built();
                }

                wrapped_builder(wrapped_builder that)
                    : base(that)
                {
                    m_devbase = that.m_devbase;
                    m_tag = that.m_tag;


                    that.consume();
                    that.built();
                }


                //void validity_check(validity_checker &valid) const { }


                public override Action<offs_t, devcb_value, devcb_value> build()  //auto build()
                {
                    throw new emu_unimplemented();
#if false
                    assert(this->m_consumed);
                    this->built();
                    memory_bank *const bank(m_devbase.membank(m_tag));
                    if (!bank)
                        throw emu_fatalerror("Write callback bound to non-existent memory bank %s of device %s (%s)\n", m_tag, m_devbase.tag(), m_devbase.name());
                    return
                            [&membank = *bank] (offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                            { membank.set_entry(data); };
#endif
                }


                //wrapped_builder(wrapped_builder const &) = delete;
                //wrapped_builder operator=(wrapped_builder const &) = delete;
                //wrapped_builder operator=(wrapped_builder &&that) = delete;
            }


            //membank_builder(membank_builder const &) = delete;
            //membank_builder &operator=(membank_builder const &) = delete;
            //membank_builder &operator=(membank_builder &&that) = delete;


            device_t m_devbase;
            string m_tag;


            //using input_t = intermediate_t<Input, int>;
            public class io_ops_membank_builder : io_operations<membank_builder>
            {
                public Type input_t { get { throw new emu_unimplemented(); } }
                public Type output_t { get { throw new emu_unimplemented(); } }
                public Type input_mask_t { get { throw new emu_unimplemented(); } }

                public membank_builder cast(builder_base base_) { return (membank_builder)base_; }
            }

            public class Type_constant_output_t : Type_constant { public Type value { get { throw new emu_unimplemented(); } } }


            public membank_builder(devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask> target, bool append, device_t devbase, string tag)  //membank_builder(devcb_write &target, bool append, device_t &devbase, std::string &&tag)
                : base(target, append, new devcb_value(new Type_constant_mask_t<Type_Input, Type_constant_s32>().value, DefaultMask))
            {
                m_devbase = devbase;
                m_tag = tag;
            }

            //membank_builder(membank_builder &&that)
            //    : builder_base(std::move(that))
            //    , transform_base<mask_t<Input, int>, membank_builder>(std::move(that))
            //    , m_devbase(that.m_devbase)
            //    , m_tag(std::move(that.m_tag))
            //{
            //    that.consume();
            //    that.built();
            //}


            //~membank_builder() { this->template register_creator<membank_builder>(); }
            protected override void Dispose(bool disposing) { register_creator<membank_builder>(); base.Dispose(disposing); }


            //template <typename T>
            //std::enable_if_t<is_transform<input_t, input_t, T>::value, first_transform_builder<wrapped_builder, std::remove_reference_t<T> > > transform(T &&cb)
            //{
            //    return first_transform_builder<wrapped_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, wrapped_builder(std::move(*this)), std::forward<T>(cb), this->exor(), this->mask(), DefaultMask);
            //}


            //void validity_check(validity_checker &valid) const { }


            public override Action<offs_t, devcb_value, devcb_value> build()  //auto build()
            {
                assert(this.m_consumed);
                this.built();
                memory_bank bank = m_devbase.membank(m_tag);
                if (bank == null)
                    throw new emu_fatalerror("Write callback bound to non-existent memory bank {0} of device {1} ({2})\n", m_tag, m_devbase.tag(), m_devbase.name());

                var membank = bank;
                var exor = this.exor();
                var mask = this.mask();
                return
                        (offs_t offset, devcb_value data, devcb_value mem_mask) =>  //[&membank = *bank, exor = this->exor(), mask = this->mask()] (offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { membank.set_entry(((data ^ exor) & mask).s32); };  //{ membank.set_entry((data ^ exor) & mask); };
            }
        }


        public class output_builder :  //class output_builder : public builder_base, public transform_base<mask_t<Input, s32>, output_builder>
                    builder_base_with_transform_base<Type_constant_mask_t<Type_Input, Type_constant_s32>,
                                                     output_builder, 
                                                     output_builder.io_ops_output_builder, 
                                                     output_builder.Type_constant_output_t>,
                    creator_impl_builder
        {
            public class wrapped_builder : builder_base
            {
                //template <typename T, typename U> friend class first_transform_builder;

                //using input_t = intermediate_t<Input, s32>;


                device_t m_devbase;
                string m_tag;


                public wrapped_builder(output_builder that)
                    : base(that)
                {
                    m_devbase = that.m_devbase;
                    m_tag = that.m_tag;


                    that.consume();
                    that.built();
                }

                public wrapped_builder(wrapped_builder that)
                    : base(that)
                {
                    m_devbase = that.m_devbase;
                    m_tag = that.m_tag;


                    that.consume();
                    that.built();
                }


                //void validity_check(validity_checker &valid) const { }


                public override Action<offs_t, devcb_value, devcb_value> build()  //auto build()
                {
                    assert(this.m_consumed);
                    this.built();

                    var item = m_devbase.machine().output().find_or_create_item(m_tag, 0);
                    throw new emu_unimplemented();
                }


                //wrapped_builder(wrapped_builder const &) = delete;
                //wrapped_builder operator=(wrapped_builder const &) = delete;
                //wrapped_builder operator=(wrapped_builder &&that) = delete;
            }


            //output_builder(output_builder const &) = delete;
            //output_builder &operator=(output_builder const &) = delete;
            //output_builder &operator=(output_builder &&that) = delete;


            device_t m_devbase;
            string m_tag;


            //using input_t = intermediate_t<Input, s32>;
            public class io_ops_output_builder : io_operations<output_builder>
            {
                public Type input_t { get { throw new emu_unimplemented(); } }
                public Type output_t { get { throw new emu_unimplemented(); } }
                public Type input_mask_t { get { throw new emu_unimplemented(); } }

                public output_builder cast(builder_base base_) { return (output_builder)base_; }
            }

            public class Type_constant_output_t : Type_constant { public Type value { get { throw new emu_unimplemented(); } } }


            public output_builder(devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask> target, bool append, device_t devbase, string tag)
                : base(target, append, new devcb_value(new Type_constant_mask_t<Type_Input, Type_constant_s32>().value, DefaultMask))
            {
                //transform_base<mask_t<Input, s32>, output_builder>(DefaultMask)


                m_devbase = devbase;
                m_tag = tag;
            }

            //output_builder(output_builder &&that)
            //    : builder_base(std::move(that))
            //    , transform_base<mask_t<Input, s32>, output_builder>(std::move(that))
            //    , m_devbase(that.m_devbase)
            //    , m_tag(std::move(that.m_tag))
            //{
            //    that.consume();
            //    that.built();
            //}

            //~output_builder() { this->template register_creator<output_builder>(); }
            protected override void Dispose(bool disposing) { register_creator<output_builder>(); base.Dispose(disposing); }


            //template <typename T>
            public override builder_base_interface transform(transform_function_delegate cb)  //std::enable_if_t<is_transform<input_t, input_t, T>::value, first_transform_builder<wrapped_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                throw new emu_unimplemented();
            }


            //void validity_check(validity_checker &valid) const { }


            public override Action<offs_t, devcb_value, devcb_value> build()  //auto build()
            {
                assert(this.m_consumed);
                this.built();

                var item = m_devbase.machine().output().find_or_create_item(m_tag, 0);
                var exor = this.exor();
                var mask = this.mask();
                return
                        (offs_t offset, devcb_value data, devcb_value mem_mask) =>  //[&item = m_devbase.machine().output().find_or_create_item(m_tag, 0), exor = this->exor(), mask = this->mask()] (offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { item.set(((data ^ exor) & mask).s32); };  //{ item.set((data ^ exor) & mask); };
            }
        }


        //class log_builder : public builder_base, public transform_base<std::make_unsigned_t<Input>, log_builder>


        public class binder
        {
            devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask> m_target;
            bool m_append = false;
            bool m_used = false;


            public binder(devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask> target) { m_target = target; }
            //binder(binder const &) = delete;
            binder(binder that) { m_target = that.m_target; m_append = that.m_append; m_used = that.m_used;  that.m_used = true; }
            //binder &operator=(binder const &) = delete;
            //binder &operator=(binder &&) = delete;


            //template <typename T>
            //std::enable_if_t<is_write<Input, T>::value, functoid_builder<std::remove_reference_t<T> > > set(T &&cb)
            //{
            //    set_used();
            //    return functoid_builder<std::remove_reference_t<T> >(m_target, m_append, std::forward<T>(cb));
            //}


            //template <typename T>
            public delegate_builder<T> set_internal<T>(T func)  //std::enable_if_t<is_write_method<T>::value, delegate_builder<delegate_type_t<T> > > set(T &&func, char const *name)
                where T : Delegate
            {
                set_used();
                return new delegate_builder<T>(m_target, m_append, m_target.owner().mconfig().current_device(), DEVICE_SELF, func);  //return delegate_builder<delegate_type_t<T> >(m_target, m_append, m_target.owner().mconfig().current_device(), DEVICE_SELF, std::forward<T>(func), name);
            }

            public delegate_builder<write8sm_delegate> set(write8sm_delegate func) { return set_internal(func); }
            public delegate_builder<write8smo_delegate> set(write8smo_delegate func) { return set_internal(func); }
            public delegate_builder<write_line_delegate> set(write_line_delegate func) { return set_internal(func); }


            //template <typename T>
            public delegate_builder<T> set_internal<T>(string tag, T func)  //std::enable_if_t<is_write_method<T>::value, delegate_builder<delegate_type_t<T> > > set(char const *tag, T &&func, char const *name)
                where T : Delegate
            {
                set_used();
                return new delegate_builder<T>(m_target, m_append, m_target.owner().mconfig().current_device(), tag, func);  //return delegate_builder<delegate_type_t<T> >(m_target, m_append, m_target.owner().mconfig().current_device(), tag, std::forward<T>(func), name);
            }

            public delegate_builder<write8sm_delegate> set(string tag, write8sm_delegate func) { return set_internal(tag, func); }
            public delegate_builder<write8smo_delegate> set(string tag, write8smo_delegate func) { return set_internal(tag, func); }
            public delegate_builder<write_line_delegate> set(string tag, write_line_delegate func) { return set_internal(tag, func); }


            //template <typename T, typename U>
            //std::enable_if_t<is_write_method<T>::value, delegate_builder<delegate_type_t<T> > > set(U &obj, T &&func, char const *name)
            //{
            //    set_used();
            //    return delegate_builder<delegate_type_t<T> >(m_target, m_append, m_target.owner(), devcb_write::cast_reference<delegate_device_class_t<T> >(obj), std::forward<T>(func), name);
            //}


            //template <typename T, typename U, bool R>
            public delegate_builder<T> set_internal<T, U, bool_R>(device_finder<U, bool_R> finder, T func)  //std::enable_if_t<is_write_method<T>::value, delegate_builder<delegate_type_t<T> > > set(device_finder<U, R> &finder, T &&func, char const *name)
                where T : Delegate
                where bool_R : bool_const, new()
            {
                set_used();
                std.pair<device_t, string> target = finder.finder_target();
                return new delegate_builder<T>(m_target, m_append, target.first, target.second, func);
            }

            //template <typename T, typename U, bool R>
            //std::enable_if_t<is_write_method<T>::value, delegate_builder<delegate_type_t<T> > > set(device_finder<U, R> const &finder, T &&func, char const *name)
            //{
            //    set_used();
            //    std::pair<device_t &, char const *> const target(finder.finder_target());
            //    return delegate_builder<delegate_type_t<T> >(m_target, m_append, target.first, target.second, std::forward<T>(func), name);
            //}

            public delegate_builder<write8sm_delegate> set<U, bool_R>(device_finder<U, bool_R> finder, write8sm_delegate func) where bool_R : bool_const, new() { return set_internal(finder, func); }
            public delegate_builder<write8smo_delegate> set<U, bool_R>(device_finder<U, bool_R> finder, write8smo_delegate func) where bool_R : bool_const, new() { return set_internal(finder, func); }
            public delegate_builder<write_line_delegate> set<U, bool_R>(device_finder<U, bool_R> finder, write_line_delegate func) where bool_R : bool_const, new() { return set_internal(finder, func); }


            //template <typename... Params>
            //auto append(Params &&... args)
            public delegate_builder<T> append_internal<T>(string tag, T func)
                where T : Delegate
            {
                m_append = true;
                return set_internal(tag, func);
            }
            public delegate_builder<T> append_internal<T, U, bool_R>(device_finder<U, bool_R> finder, T func)
                where T : Delegate
                where bool_R : bool_const, new()
            {
                m_append = true;
                return set_internal(finder, func);
            }
            public delegate_builder<T> append_internal<T>(T func)
                where T : Delegate
            {
                m_append = true;
                return set_internal(func);
            }

            public delegate_builder<write_line_delegate> append(string tag, write_line_delegate func) { return append_internal(tag, func); }
            public delegate_builder<write_line_delegate> append<U, bool_R>(device_finder<U, bool_R> finder, write_line_delegate func) where bool_R : bool_const, new() { return append_internal(finder, func); }

            public delegate_builder<write_line_delegate> append(write_line_delegate func) { return append_internal(func); }


            public inputline_builder set_inputline(string tag, int linenum)
            {
                set_used();
                return new inputline_builder(m_target, m_append, m_target.owner().mconfig().current_device(), tag, linenum);
            }


            //latched_inputline_builder set_inputline(char const *tag, int linenum, int value)
            //{
            //    set_used();
            //    return latched_inputline_builder(m_target, m_append, m_target.owner().mconfig().current_device(), tag, linenum, value);
            //}

            //inputline_builder set_inputline(device_execute_interface &obj, int linenum)
            //{
            //    set_used();
            //    return inputline_builder(m_target, m_append, obj, linenum);
            //}

            //latched_inputline_builder set_inputline(device_execute_interface &obj, int linenum, int value)
            //{
            //    set_used();
            //    return latched_inputline_builder(m_target, m_append, obj, linenum, value);
            //}


            //template <typename T, bool R>
            public inputline_builder set_inputline<T, bool_R>(device_finder<T, bool_R> finder, int linenum)  //inputline_builder set_inputline(device_finder<T, R> const &finder, int linenum)
                where bool_R : bool_const, new()
            {
                set_used();
                std.pair<device_t, string> target = finder.finder_target();  //std::pair<device_t &, char const *> const target(finder.finder_target());
                return new inputline_builder(m_target, m_append, target.first, target.second, linenum);  //return inputline_builder(m_target, m_append, target.first, target.second, linenum);
            }


            public latched_inputline_builder set_inputline<T, bool_R>(device_finder<T, bool_R> finder, int linenum, int value)  //latched_inputline_builder set_inputline(device_finder<T, R> const &finder, int linenum, int value)
                where bool_R : bool_const, new()
            {
                set_used();
                std.pair<device_t, string> target = finder.finder_target();  //std::pair<device_t &, char const *> const target(finder.finder_target());
                return new latched_inputline_builder(m_target, m_append, target.first, target.second, linenum, value);  //return latched_inputline_builder(m_target, m_append, target.first, target.second, linenum, value);
            }


            //template <typename... Params>
            public inputline_builder append_inputline(string tag, int linenum)  //auto append_inputline(Params &&... args)
            {
                m_append = true;
                return set_inputline(tag, linenum);
            }

            //template <typename... Params>
            public inputline_builder append_inputline<T, bool_R>(device_finder<T, bool_R> finder, int linenum)  //auto append_inputline(Params &&... args)
                where bool_R : bool_const, new()
            {
                m_append = true;
                return set_inputline(finder, linenum);
            }


            //template <typename... Params>
            //ioport_builder set_ioport(Params &&... args)
            //{
            //    set_used();
            //    return ioport_builder(m_target, m_append, m_target.owner().mconfig().current_device(), std::string(std::forward<Params>(args)...));
            //}

            //template <bool R>
            //ioport_builder set_ioport(ioport_finder<R> &finder)
            //{
            //    set_used();
            //    std::pair<device_t &, char const *> const target(finder.finder_target());
            //    return ioport_builder(m_target, m_append, target.first, std::string(target.second));
            //}

            //template <bool R>
            //ioport_builder set_ioport(ioport_finder<R> const &finder)
            //{
            //    set_used();
            //    std::pair<device_t &, char const *> const target(finder.finder_target());
            //    return ioport_builder(m_target, m_append, target.first, std::string(target.second));
            //}

            //template <typename... Params>
            //ioport_builder append_ioport(Params &&... args)
            //{
            //    m_append = true;
            //    return set_ioport(std::forward<Params>(args)...);
            //}

            //template <typename... Params>
            public membank_builder set_membank(string tag)  //membank_builder set_membank(Params &&... args)
            {
                set_used();
                return new membank_builder(m_target, m_append, m_target.owner().mconfig().current_device(), tag);
            }

            //template <bool R>
            //membank_builder set_membank(memory_bank_finder<R> &finder)
            //{
            //    set_used();
            //    std::pair<device_t &, char const *> const target(finder.finder_target());
            //    return membank_builder(m_target, m_append, target.first, std::string(target.second));
            //}

            //template <bool R>
            //membank_builder set_membank(memory_bank_finder<R> const &finder)
            //{
            //    set_used();
            //    std::pair<device_t &, char const *> const target(finder.finder_target());
            //    return membank_builder(m_target, m_append, target.first, std::string(target.second));
            //}

            //template <typename... Params>
            public membank_builder append_membank(string tag)  //membank_builder append_membank(Params &&... args)
            {
                m_append = true;
                return set_membank(tag);
            }


            //template <typename... Params>
            public output_builder set_output(string tag)  //output_builder set_output(Params &&... args)
            {
                set_used();
                return new output_builder(m_target, m_append, m_target.owner().mconfig().current_device(), tag);  //return output_builder(m_target, m_append, m_target.owner().mconfig().current_device(), std::string(std::forward<Params>(args)...));
            }


            //template <typename... Params>
            //output_builder append_output(Params &&... args)
            //{
            //    m_append = true;
            //    return set_output(std::forward<Params>(args)...);
            //}

            //template <typename... Params>
            //log_builder set_log(device_t &devbase, Params &&... args)
            //{
            //    set_used();
            //    return log_builder(m_target, m_append, devbase, std::string(std::forward<Params>(args)...));
            //}

            //template <typename T, typename... Params>
            //std::enable_if_t<emu::detail::is_device_implementation<std::remove_reference_t<T> >::value, log_builder> set_log(T &devbase, Params &&... args)
            //{
            //    return set_log(static_cast<device_t &>(devbase), std::forward<Params>(args)...);
            //}

            //template <typename T, typename... Params>
            //std::enable_if_t<emu::detail::is_device_interface<std::remove_reference_t<T> >::value, log_builder> set_log(T &devbase, Params &&... args)
            //{
            //    return set_log(devbase.device(), std::forward<Params>(args)...);
            //}

            //template <typename... Params>
            //log_builder set_log(Params &&... args)
            //{
            //    return set_log(m_target.owner().mconfig().current_device(), std::forward<Params>(args)...);
            //}

            //template <typename... Params>
            //log_builder append_log(Params &&... args)
            //{
            //    m_append = true;
            //    return set_log(std::forward<Params>(args)...);
            //}


            public void set_nop()
            {
                set_used();
                m_target.m_creators.clear();
                m_target.m_creators.emplace_back(new nop_creator());
            }


            void set_used() { assert(!m_used); m_used = true; }
        }


        //template <unsigned Count>
        public class array<unsigned_Count> : devcb_write_base.array<devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask>, unsigned_Count>  //class array : public devcb_write_base::array<devcb_write<Input, DefaultMask>, Count>
            where unsigned_Count : u64_const, new()
        {
            //using devcb_write_base::array<devcb_write<Input, DefaultMask>, Count>::array;


            public array(device_t owner, Func<devcb_write<Type_Input, devcb_value_const_unsigned_DefaultMask>> add_function) : base(owner, add_function) { }


            public void resolve_all_safe()
            {
                foreach (var elem in this)  //for (devcb_write<Input, DefaultMask> &elem : *this)
                    elem.resolve_safe();
            }
        }


        std.vector<func_t> m_functions = new std.vector<func_t>();
        std.vector<creator> m_creators = new std.vector<creator>();  //std::vector<typename creator::ptr> m_creators;


        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        public devcb_write(device_t owner)  //devcb_write<Input, DefaultMask>::devcb_write(device_t &owner)
            : base(owner)
        {
        }


        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        public binder bind()
        {
            return new binder(this);
        }


        //void reset();


        protected override void validity_check(validity_checker valid)  //virtual void validity_check(validity_checker &valid) const override;
        {
            throw new emu_unimplemented();
        }


        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        public void resolve()
        {
            assert(m_functions.empty());
            m_functions.reserve(m_creators.size());
            foreach (creator c in m_creators)  //for (typename creator::ptr const &c : m_creators)
                m_functions.emplace_back(c.create());
            m_creators.clear();
        }


        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        public void resolve_safe()
        {
            resolve();
            if (m_functions.empty())
                m_functions.emplace_back((offs_t offset, devcb_value data, devcb_value mem_mask) => { });
        }


        //void operator()(offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask = DefaultMask);
        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        public void op(offs_t offset, devcb_value data) { op(offset, data, DefaultMask); }

        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        public void op(offs_t offset, devcb_value data, devcb_value mem_mask)
        {
            assert(m_creators.empty() && !m_functions.empty());
            //typename std::vector<func_t>::const_iterator it(m_functions.begin());
            //(*it)(offset, data, mem_mask);
            //while (m_functions.end() != ++it)
            //    (*it)(offset, data, mem_mask);
            foreach (var it in m_functions)
                it(offset, data, mem_mask);
        }

        public void op_u8(offs_t offset, u8 data) { op(offset, new devcb_value(data.GetType(), data)); }
        public void op_u8(offs_t offset, u8 data, u8 mem_mask) { op(offset, new devcb_value(data.GetType(), data), new devcb_value(mem_mask.GetType(), mem_mask)); }
        public void op_s32(offs_t offset, s32 data) { op(offset, new devcb_value(data.GetType(), data)); }
        public void op_s32(offs_t offset, s32 data, s32 mem_mask) { op(offset, new devcb_value(data.GetType(), data), new devcb_value(mem_mask.GetType(), mem_mask)); }


        //void operator()(Input data);
        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        public void op(devcb_value data)
        {
            this.op(0U, data, DefaultMask);
        }

        public void op_u8(u8 data) { op(new devcb_value(data.GetType(), data)); }
        public void op_s32(s32 data) { op(new devcb_value(data.GetType(), data)); }
        public void op_u32(u32 data) { op(new devcb_value(data.GetType(), data)); }


        public bool isnull() { return m_functions.empty() && m_creators.empty(); }

        public bool bool_ { get { return !m_functions.empty(); } }  //explicit operator bool() const { return !m_functions.empty(); }
    }


    //using devcb_read8 = devcb_read<u8>;
    //using devcb_read16 = devcb_read<u16>;
    //using devcb_read32 = devcb_read<u32>;
    //using devcb_read64 = devcb_read<u64>;
    //using devcb_read_line = devcb_read<int, 1U>;

    //using devcb_write8 = devcb_write<u8>;
    //using devcb_write16 = devcb_write<u16>;
    //using devcb_write32 = devcb_write<u32>;
    //using devcb_write64 = devcb_write<u64>;
    //using devcb_write_line = devcb_write<int, 1U>;


    //**************************************************************************
    //  TEMPLATE INSTANTIATIONS
    //**************************************************************************

#if false
    extern template class devcb_read<u8>;
    extern template class devcb_read<u16>;
    extern template class devcb_read<u32>;
    extern template class devcb_read<u64>;
    extern template class devcb_read<int, 1U>;

    extern template class devcb_read8::delegate_builder<read8s_delegate>;
    extern template class devcb_read8::delegate_builder<read16s_delegate>;
    extern template class devcb_read8::delegate_builder<read32s_delegate>;
    extern template class devcb_read8::delegate_builder<read64s_delegate>;
    extern template class devcb_read8::delegate_builder<read8sm_delegate>;
    extern template class devcb_read8::delegate_builder<read16sm_delegate>;
    extern template class devcb_read8::delegate_builder<read32sm_delegate>;
    extern template class devcb_read8::delegate_builder<read64sm_delegate>;
    extern template class devcb_read8::delegate_builder<read8smo_delegate>;
    extern template class devcb_read8::delegate_builder<read16smo_delegate>;
    extern template class devcb_read8::delegate_builder<read32smo_delegate>;
    extern template class devcb_read8::delegate_builder<read64smo_delegate>;
    extern template class devcb_read8::delegate_builder<read_line_delegate>;

    extern template class devcb_read16::delegate_builder<read8s_delegate>;
    extern template class devcb_read16::delegate_builder<read16s_delegate>;
    extern template class devcb_read16::delegate_builder<read32s_delegate>;
    extern template class devcb_read16::delegate_builder<read64s_delegate>;
    extern template class devcb_read16::delegate_builder<read8sm_delegate>;
    extern template class devcb_read16::delegate_builder<read16sm_delegate>;
    extern template class devcb_read16::delegate_builder<read32sm_delegate>;
    extern template class devcb_read16::delegate_builder<read64sm_delegate>;
    extern template class devcb_read16::delegate_builder<read8smo_delegate>;
    extern template class devcb_read16::delegate_builder<read16smo_delegate>;
    extern template class devcb_read16::delegate_builder<read32smo_delegate>;
    extern template class devcb_read16::delegate_builder<read64smo_delegate>;
    extern template class devcb_read16::delegate_builder<read_line_delegate>;

    extern template class devcb_read32::delegate_builder<read8s_delegate>;
    extern template class devcb_read32::delegate_builder<read16s_delegate>;
    extern template class devcb_read32::delegate_builder<read32s_delegate>;
    extern template class devcb_read32::delegate_builder<read64s_delegate>;
    extern template class devcb_read32::delegate_builder<read8sm_delegate>;
    extern template class devcb_read32::delegate_builder<read16sm_delegate>;
    extern template class devcb_read32::delegate_builder<read32sm_delegate>;
    extern template class devcb_read32::delegate_builder<read64sm_delegate>;
    extern template class devcb_read32::delegate_builder<read8smo_delegate>;
    extern template class devcb_read32::delegate_builder<read16smo_delegate>;
    extern template class devcb_read32::delegate_builder<read32smo_delegate>;
    extern template class devcb_read32::delegate_builder<read64smo_delegate>;
    extern template class devcb_read32::delegate_builder<read_line_delegate>;

    extern template class devcb_read64::delegate_builder<read8s_delegate>;
    extern template class devcb_read64::delegate_builder<read16s_delegate>;
    extern template class devcb_read64::delegate_builder<read32s_delegate>;
    extern template class devcb_read64::delegate_builder<read64s_delegate>;
    extern template class devcb_read64::delegate_builder<read8sm_delegate>;
    extern template class devcb_read64::delegate_builder<read16sm_delegate>;
    extern template class devcb_read64::delegate_builder<read32sm_delegate>;
    extern template class devcb_read64::delegate_builder<read64sm_delegate>;
    extern template class devcb_read64::delegate_builder<read8smo_delegate>;
    extern template class devcb_read64::delegate_builder<read16smo_delegate>;
    extern template class devcb_read64::delegate_builder<read32smo_delegate>;
    extern template class devcb_read64::delegate_builder<read64smo_delegate>;
    extern template class devcb_read64::delegate_builder<read_line_delegate>;

    extern template class devcb_read_line::delegate_builder<read8s_delegate>;
    extern template class devcb_read_line::delegate_builder<read16s_delegate>;
    extern template class devcb_read_line::delegate_builder<read32s_delegate>;
    extern template class devcb_read_line::delegate_builder<read64s_delegate>;
    extern template class devcb_read_line::delegate_builder<read8sm_delegate>;
    extern template class devcb_read_line::delegate_builder<read16sm_delegate>;
    extern template class devcb_read_line::delegate_builder<read32sm_delegate>;
    extern template class devcb_read_line::delegate_builder<read64sm_delegate>;
    extern template class devcb_read_line::delegate_builder<read8smo_delegate>;
    extern template class devcb_read_line::delegate_builder<read16smo_delegate>;
    extern template class devcb_read_line::delegate_builder<read32smo_delegate>;
    extern template class devcb_read_line::delegate_builder<read64smo_delegate>;
    extern template class devcb_read_line::delegate_builder<read_line_delegate>;

    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read8s_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read16s_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read32s_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read64s_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read8sm_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read16sm_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read32sm_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read64sm_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read8smo_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read16smo_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read32smo_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read64smo_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read_line_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::ioport_builder>;

    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read8s_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read16s_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read32s_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read64s_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read8sm_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read16sm_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read32sm_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read64sm_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read8smo_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read16smo_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read32smo_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read64smo_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read_line_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::ioport_builder>;

    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read8s_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read16s_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read32s_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read64s_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read8sm_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read16sm_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read32sm_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read64sm_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read8smo_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read16smo_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read32smo_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read64smo_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read_line_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::ioport_builder>;

    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read8s_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read16s_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read32s_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read64s_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read8sm_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read16sm_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read32sm_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read64sm_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read8smo_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read16smo_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read32smo_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read64smo_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read_line_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::ioport_builder>;

    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read8s_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read16s_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read32s_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read64s_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read8sm_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read16sm_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read32sm_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read64sm_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read8smo_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read16smo_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read32smo_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read64smo_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read_line_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::ioport_builder>;

    extern template class devcb_write<u8>;
    extern template class devcb_write<u16>;
    extern template class devcb_write<u32>;
    extern template class devcb_write<u64>;
    extern template class devcb_write<int, 1U>;

    extern template class devcb_write8::delegate_builder<write8s_delegate>;
    extern template class devcb_write8::delegate_builder<write16s_delegate>;
    extern template class devcb_write8::delegate_builder<write32s_delegate>;
    extern template class devcb_write8::delegate_builder<write64s_delegate>;
    extern template class devcb_write8::delegate_builder<write8sm_delegate>;
    extern template class devcb_write8::delegate_builder<write16sm_delegate>;
    extern template class devcb_write8::delegate_builder<write32sm_delegate>;
    extern template class devcb_write8::delegate_builder<write64sm_delegate>;
    extern template class devcb_write8::delegate_builder<write8smo_delegate>;
    extern template class devcb_write8::delegate_builder<write16smo_delegate>;
    extern template class devcb_write8::delegate_builder<write32smo_delegate>;
    extern template class devcb_write8::delegate_builder<write64smo_delegate>;
    extern template class devcb_write8::delegate_builder<write_line_delegate>;

    extern template class devcb_write16::delegate_builder<write8s_delegate>;
    extern template class devcb_write16::delegate_builder<write16s_delegate>;
    extern template class devcb_write16::delegate_builder<write32s_delegate>;
    extern template class devcb_write16::delegate_builder<write64s_delegate>;
    extern template class devcb_write16::delegate_builder<write8sm_delegate>;
    extern template class devcb_write16::delegate_builder<write16sm_delegate>;
    extern template class devcb_write16::delegate_builder<write32sm_delegate>;
    extern template class devcb_write16::delegate_builder<write64sm_delegate>;
    extern template class devcb_write16::delegate_builder<write8smo_delegate>;
    extern template class devcb_write16::delegate_builder<write16smo_delegate>;
    extern template class devcb_write16::delegate_builder<write32smo_delegate>;
    extern template class devcb_write16::delegate_builder<write64smo_delegate>;
    extern template class devcb_write16::delegate_builder<write_line_delegate>;

    extern template class devcb_write32::delegate_builder<write8s_delegate>;
    extern template class devcb_write32::delegate_builder<write16s_delegate>;
    extern template class devcb_write32::delegate_builder<write32s_delegate>;
    extern template class devcb_write32::delegate_builder<write64s_delegate>;
    extern template class devcb_write32::delegate_builder<write8sm_delegate>;
    extern template class devcb_write32::delegate_builder<write16sm_delegate>;
    extern template class devcb_write32::delegate_builder<write32sm_delegate>;
    extern template class devcb_write32::delegate_builder<write64sm_delegate>;
    extern template class devcb_write32::delegate_builder<write8smo_delegate>;
    extern template class devcb_write32::delegate_builder<write16smo_delegate>;
    extern template class devcb_write32::delegate_builder<write32smo_delegate>;
    extern template class devcb_write32::delegate_builder<write64smo_delegate>;
    extern template class devcb_write32::delegate_builder<write_line_delegate>;

    extern template class devcb_write64::delegate_builder<write8s_delegate>;
    extern template class devcb_write64::delegate_builder<write16s_delegate>;
    extern template class devcb_write64::delegate_builder<write32s_delegate>;
    extern template class devcb_write64::delegate_builder<write64s_delegate>;
    extern template class devcb_write64::delegate_builder<write8sm_delegate>;
    extern template class devcb_write64::delegate_builder<write16sm_delegate>;
    extern template class devcb_write64::delegate_builder<write32sm_delegate>;
    extern template class devcb_write64::delegate_builder<write64sm_delegate>;
    extern template class devcb_write64::delegate_builder<write8smo_delegate>;
    extern template class devcb_write64::delegate_builder<write16smo_delegate>;
    extern template class devcb_write64::delegate_builder<write32smo_delegate>;
    extern template class devcb_write64::delegate_builder<write64smo_delegate>;
    extern template class devcb_write64::delegate_builder<write_line_delegate>;

    extern template class devcb_write_line::delegate_builder<write8s_delegate>;
    extern template class devcb_write_line::delegate_builder<write16s_delegate>;
    extern template class devcb_write_line::delegate_builder<write32s_delegate>;
    extern template class devcb_write_line::delegate_builder<write64s_delegate>;
    extern template class devcb_write_line::delegate_builder<write8sm_delegate>;
    extern template class devcb_write_line::delegate_builder<write16sm_delegate>;
    extern template class devcb_write_line::delegate_builder<write32sm_delegate>;
    extern template class devcb_write_line::delegate_builder<write64sm_delegate>;
    extern template class devcb_write_line::delegate_builder<write8smo_delegate>;
    extern template class devcb_write_line::delegate_builder<write16smo_delegate>;
    extern template class devcb_write_line::delegate_builder<write32smo_delegate>;
    extern template class devcb_write_line::delegate_builder<write64smo_delegate>;
    extern template class devcb_write_line::delegate_builder<write_line_delegate>;

    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write8s_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write16s_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write32s_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write64s_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write8sm_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write16sm_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write32sm_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write64sm_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write8smo_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write16smo_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write32smo_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write64smo_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write_line_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::inputline_builder>;
    extern template class devcb_write8::creator_impl<devcb_write8::latched_inputline_builder>;
    extern template class devcb_write8::creator_impl<devcb_write8::ioport_builder>;
    extern template class devcb_write8::creator_impl<devcb_write8::membank_builder>;
    extern template class devcb_write8::creator_impl<devcb_write8::output_builder>;
    extern template class devcb_write8::creator_impl<devcb_write8::log_builder>;

    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write8s_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write16s_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write32s_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write64s_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write8sm_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write16sm_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write32sm_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write64sm_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write8smo_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write16smo_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write32smo_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write64smo_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write_line_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::inputline_builder>;
    extern template class devcb_write16::creator_impl<devcb_write16::latched_inputline_builder>;
    extern template class devcb_write16::creator_impl<devcb_write16::ioport_builder>;
    extern template class devcb_write16::creator_impl<devcb_write16::membank_builder>;
    extern template class devcb_write16::creator_impl<devcb_write16::output_builder>;
    extern template class devcb_write16::creator_impl<devcb_write16::log_builder>;

    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write8s_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write16s_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write32s_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write64s_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write8sm_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write16sm_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write32sm_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write64sm_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write8smo_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write16smo_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write32smo_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write64smo_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write_line_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::inputline_builder>;
    extern template class devcb_write32::creator_impl<devcb_write32::latched_inputline_builder>;
    extern template class devcb_write32::creator_impl<devcb_write32::ioport_builder>;
    extern template class devcb_write32::creator_impl<devcb_write32::membank_builder>;
    extern template class devcb_write32::creator_impl<devcb_write32::output_builder>;
    extern template class devcb_write32::creator_impl<devcb_write32::log_builder>;

    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write8s_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write16s_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write32s_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write64s_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write8sm_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write16sm_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write32sm_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write64sm_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write8smo_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write16smo_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write32smo_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write64smo_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write_line_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::inputline_builder>;
    extern template class devcb_write64::creator_impl<devcb_write64::latched_inputline_builder>;
    extern template class devcb_write64::creator_impl<devcb_write64::ioport_builder>;
    extern template class devcb_write64::creator_impl<devcb_write64::membank_builder>;
    extern template class devcb_write64::creator_impl<devcb_write64::output_builder>;
    extern template class devcb_write64::creator_impl<devcb_write64::log_builder>;

    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write8s_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write16s_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write32s_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write64s_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write8sm_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write16sm_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write32sm_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write64sm_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write8smo_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write16smo_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write32smo_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write64smo_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write_line_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::inputline_builder>;
    extern template class devcb_write_line::creator_impl<devcb_write_line::latched_inputline_builder>;
    extern template class devcb_write_line::creator_impl<devcb_write_line::ioport_builder>;
    extern template class devcb_write_line::creator_impl<devcb_write_line::membank_builder>;
    extern template class devcb_write_line::creator_impl<devcb_write_line::output_builder>;
    extern template class devcb_write_line::creator_impl<devcb_write_line::log_builder>;
#endif
}
