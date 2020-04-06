// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using offs_t = System.UInt32;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    //**************************************************************************
    //  DELEGATE TYPES
    //**************************************************************************

    //typedef device_delegate<int ()> read_line_delegate;
    public delegate int read_line_delegate();

    //typedef device_delegate<void (int)> write_line_delegate;
    public delegate void write_line_delegate(int param);


#if false
    namespace emu { namespace detail {

    template <typename T, typename Enable = void> struct read_line_device_class { };
    template <typename T, typename Enable = void> struct write_line_device_class { };

    template <typename T>
    inline read_line_delegate make_delegate(T &&func, char const *name, char const *tag, rw_device_class_t<read_line_delegate, std::remove_reference_t<T> > *obj)
    { return read_line_delegate(func, name, tag, obj); }

    template <typename T>
    inline write_line_delegate make_delegate(T &&func, char const *name, char const *tag, rw_device_class_t<write_line_delegate, std::remove_reference_t<T> > *obj)
    { return write_line_delegate(func, name, tag, obj); }

    } } // namespace emu::detail
#endif


    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    /// \brief Base callback helper
    ///
    /// Provides utilities for supporting multiple read/write/transform
    /// signatures, and the base exclusive-or/mask transform methods.
    public class devcb_base : global_object
    {
        public delegate u32 transform_func(offs_t offset, u32 data, ref u32 mem_mask);


        device_t m_owner;
        optional_device<device_memory_interface> m_dflt_space_finder;
        int m_dflt_space_index;
        address_space m_dflt_space;


        //template <typename T> void set_space(T &&tag, int index)
        //{
        //    m_dflt_space_finder.set_tag(std::forward<T>(tag));
        //    m_dflt_space_index = index;
        //}

        //virtual void validity_check(validity_checker &valid) const;

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
        //template <typename Input, typename Result, typename Func, typename Enable = void> struct is_transform_form1 { static constexpr bool value = false; };
        //template <typename Input, typename Result, typename Func, typename Enable = void> struct is_transform_form2 { static constexpr bool value = false; };
        //template <typename Input, typename Result, typename Func, typename Enable = void> struct is_transform_form3 { static constexpr bool value = false; };
        //template <typename Input, typename Result, typename Func, typename Enable = void> struct is_transform_form4 { static constexpr bool value = false; };
        //template <typename Input, typename Result, typename Func, typename Enable = void> struct is_transform_form5 { static constexpr bool value = false; };
        //template <typename Input, typename Result, typename Func, typename Enable = void> struct is_transform_form6 { static constexpr bool value = false; };
        //template <typename Input, typename Result, typename Func> struct is_transform_form1<Input, Result, Func, std::enable_if_t<std::is_convertible<std::result_of_t<Func (address_space &, offs_t &, Input, std::make_unsigned_t<Input> &)>, Result>::value> > { static constexpr bool value = true; };
        //template <typename Input, typename Result, typename Func> struct is_transform_form2<Input, Result, Func, std::enable_if_t<std::is_convertible<std::result_of_t<Func (address_space &, offs_t &, Input)>, Result>::value> > { static constexpr bool value = true; };
        //template <typename Input, typename Result, typename Func> struct is_transform_form3<Input, Result, Func, std::enable_if_t<std::is_convertible<std::result_of_t<Func (offs_t &, Input, std::make_unsigned_t<Input> &)>, Result>::value> > { static constexpr bool value = true; };
        //template <typename Input, typename Result, typename Func> struct is_transform_form4<Input, Result, Func, std::enable_if_t<std::is_convertible<std::result_of_t<Func (offs_t &, Input)>, Result>::value> > { static constexpr bool value = true; };
        //template <typename Input, typename Result, typename Func> struct is_transform_form5<Input, Result, Func, std::enable_if_t<std::is_convertible<std::result_of_t<Func (address_space &, Input)>, Result>::value> > { static constexpr bool value = true; };
        //template <typename Input, typename Result, typename Func> struct is_transform_form6<Input, Result, Func, std::enable_if_t<std::is_convertible<std::result_of_t<Func (Input)>, Result>::value> > { static constexpr bool value = true; };
        //template <typename Input, typename Result, typename Func> struct is_transform { static constexpr bool value = is_transform_form1<Input, Result, Func>::value || is_transform_form2<Input, Result, Func>::value || is_transform_form3<Input, Result, Func>::value || is_transform_form4<Input, Result, Func>::value || is_transform_form5<Input, Result, Func>::value || is_transform_form6<Input, Result, Func>::value; };

        // Determining the result type of a transform function
        //template <typename Input, typename Result, typename Func, typename Enable = void> struct transform_result;
        //template <typename Input, typename Result, typename Func> struct transform_result<Input, Result, Func, std::enable_if_t<is_transform_form1<Input, Result, Func>::value> > { using type = std::result_of_t<Func (address_space &, offs_t &, Input, std::make_unsigned_t<Input> &)>; };
        //template <typename Input, typename Result, typename Func> struct transform_result<Input, Result, Func, std::enable_if_t<is_transform_form2<Input, Result, Func>::value> > { using type = std::result_of_t<Func (address_space &, offs_t &, Input)>; };
        //template <typename Input, typename Result, typename Func> struct transform_result<Input, Result, Func, std::enable_if_t<is_transform_form3<Input, Result, Func>::value> > { using type = std::result_of_t<Func (offs_t &, Input, std::make_unsigned_t<Input> &)>; };
        //template <typename Input, typename Result, typename Func> struct transform_result<Input, Result, Func, std::enable_if_t<is_transform_form4<Input, Result, Func>::value> > { using type = std::result_of_t<Func (offs_t &, Input)>; };
        //template <typename Input, typename Result, typename Func> struct transform_result<Input, Result, Func, std::enable_if_t<is_transform_form5<Input, Result, Func>::value> > { using type = std::result_of_t<Func (address_space &, Input)>; };
        //template <typename Input, typename Result, typename Func> struct transform_result<Input, Result, Func, std::enable_if_t<is_transform_form6<Input, Result, Func>::value> > { using type = std::result_of_t<Func (Input)>; };
        //template <typename Input, typename Result, typename Func> using transform_result_t = typename transform_result<Input, Result, Func>::type;

        // Mapping method types to delegate types
        //template <typename T, typename Enable = void> struct delegate_type;
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read8_delegate, std::remove_reference_t<T> > > > { using type = read8_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read16_delegate, std::remove_reference_t<T> > > > { using type = read16_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read32_delegate, std::remove_reference_t<T> > > > { using type = read32_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read64_delegate, std::remove_reference_t<T> > > > { using type = read64_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read8s_delegate, std::remove_reference_t<T> > > > { using type = read8s_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read16s_delegate, std::remove_reference_t<T> > > > { using type = read16s_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read32s_delegate, std::remove_reference_t<T> > > > { using type = read32s_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read64s_delegate, std::remove_reference_t<T> > > > { using type = read64s_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read8sm_delegate, std::remove_reference_t<T> > > > { using type = read8sm_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read16sm_delegate, std::remove_reference_t<T> > > > { using type = read16sm_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read32sm_delegate, std::remove_reference_t<T> > > > { using type = read32sm_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read64sm_delegate, std::remove_reference_t<T> > > > { using type = read64sm_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read8smo_delegate, std::remove_reference_t<T> > > > { using type = read8smo_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read16smo_delegate, std::remove_reference_t<T> > > > { using type = read16smo_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read32smo_delegate, std::remove_reference_t<T> > > > { using type = read32smo_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read64smo_delegate, std::remove_reference_t<T> > > > { using type = read64smo_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<read_line_delegate, std::remove_reference_t<T> > > > { using type = read_line_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write8_delegate, std::remove_reference_t<T> > > > { using type = write8_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write16_delegate, std::remove_reference_t<T> > > > { using type = write16_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write32_delegate, std::remove_reference_t<T> > > > { using type = write32_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write64_delegate, std::remove_reference_t<T> > > > { using type = write64_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write8s_delegate, std::remove_reference_t<T> > > > { using type = write8s_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write16s_delegate, std::remove_reference_t<T> > > > { using type = write16s_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write32s_delegate, std::remove_reference_t<T> > > > { using type = write32s_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write64s_delegate, std::remove_reference_t<T> > > > { using type = write64s_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write8sm_delegate, std::remove_reference_t<T> > > > { using type = write8sm_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write16sm_delegate, std::remove_reference_t<T> > > > { using type = write16sm_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write32sm_delegate, std::remove_reference_t<T> > > > { using type = write32sm_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write64sm_delegate, std::remove_reference_t<T> > > > { using type = write64sm_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write8smo_delegate, std::remove_reference_t<T> > > > { using type = write8smo_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write16smo_delegate, std::remove_reference_t<T> > > > { using type = write16smo_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write32smo_delegate, std::remove_reference_t<T> > > > { using type = write32smo_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write64smo_delegate, std::remove_reference_t<T> > > > { using type = write64smo_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> struct delegate_type<T, void_t<emu::detail::rw_device_class_t<write_line_delegate, std::remove_reference_t<T> > > > { using type = write_line_delegate; using device_class = emu::detail::rw_device_class_t<type, std::remove_reference_t<T> >; };
        //template <typename T> using delegate_type_t = typename delegate_type<T>::type;
        //template <typename T> using delegate_device_class_t = typename delegate_type<T>::device_class;

        // Invoking transform callbacks
        //template <typename Input, typename Result, typename T> static std::enable_if_t<is_transform_form1<Input, Result, T>::value, mask_t<transform_result_t<Input, Result, T>, Result> > invoke_transform(T const &cb, address_space &space, offs_t &offset, Input data, std::make_unsigned_t<Input> &mem_mask) { return std::make_unsigned_t<transform_result_t<Input, Result, T> >(cb(space, offset, data, mem_mask)); }
        //template <typename Input, typename Result, typename T> static std::enable_if_t<is_transform_form2<Input, Result, T>::value, mask_t<transform_result_t<Input, Result, T>, Result> > invoke_transform(T const &cb, address_space &space, offs_t &offset, Input data, std::make_unsigned_t<Input> &mem_mask) { return std::make_unsigned_t<transform_result_t<Input, Result, T> >(cb(space, data, offset)); }
        //template <typename Input, typename Result, typename T> static std::enable_if_t<is_transform_form3<Input, Result, T>::value, mask_t<transform_result_t<Input, Result, T>, Result> > invoke_transform(T const &cb, address_space &space, offs_t &offset, Input data, std::make_unsigned_t<Input> &mem_mask) { return std::make_unsigned_t<transform_result_t<Input, Result, T> >(cb(offset, data, mem_mask)); }
        //template <typename Input, typename Result, typename T> static std::enable_if_t<is_transform_form4<Input, Result, T>::value, mask_t<transform_result_t<Input, Result, T>, Result> > invoke_transform(T const &cb, address_space &space, offs_t &offset, Input data, std::make_unsigned_t<Input> &mem_mask) { return std::make_unsigned_t<transform_result_t<Input, Result, T> >(cb(offset, data)); }
        //template <typename Input, typename Result, typename T> static std::enable_if_t<is_transform_form5<Input, Result, T>::value, mask_t<transform_result_t<Input, Result, T>, Result> > invoke_transform(T const &cb, address_space &space, offs_t &offset, Input data, std::make_unsigned_t<Input> &mem_mask) { return std::make_unsigned_t<transform_result_t<Input, Result, T> >(cb(space, data)); }
        //template <typename Input, typename Result, typename T> static std::enable_if_t<is_transform_form6<Input, Result, T>::value, mask_t<transform_result_t<Input, Result, T>, Result> > invoke_transform(T const &cb, address_space &space, offs_t &offset, Input data, std::make_unsigned_t<Input> &mem_mask) { return std::make_unsigned_t<transform_result_t<Input, Result, T> >(cb(data)); }
        protected static u32 invoke_transform(transform_func cb, address_space space, offs_t offset, u32 data, ref u32 mem_mask) { return cb(offset, data, ref mem_mask); }

        // Working with devices and interfaces
        //template <typename T> static std::enable_if_t<emu::detail::is_device_implementation<T>::value, const char *> get_tag(T &obj) { return obj.tag(); }
        //template <typename T> static std::enable_if_t<emu::detail::is_device_interface<T>::value, const char *> get_tag(T &obj) { return obj.device().tag(); }
        //template <typename T, typename U> static std::enable_if_t<std::is_convertible<std::add_pointer_t<U>, std::add_pointer_t<T> >::value, T &> cast_reference(U &obj) { return downcast<T &>(obj); }
        //template <typename T, typename U> static std::enable_if_t<!std::is_convertible<std::add_pointer_t<U>, std::add_pointer_t<T> >::value, T &> cast_reference(U &obj) { return dynamic_cast<T &>(obj); }


        /// \brief Base transform helper
        ///
        /// Provides member functions for setting exclusive-or, mask and
        /// shifts.  Exclusive-or and mask values are stored; it's assumed
        /// that the implementation supports lamba transforms to allow
        /// shifts.
        //template <typename T, typename Impl>
        class transform_base
        {
            //std::make_unsigned_t<T> m_exor = std::make_unsigned_t<T>(0);
            //std::make_unsigned_t<T> m_mask;
            bool m_inherited_mask = true;


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

            //constexpr std::make_unsigned_t<T> exor() const { return m_exor & m_mask; }
            //constexpr std::make_unsigned_t<T> mask() const { return m_mask; }

            //constexpr bool need_exor() const { return std::make_unsigned_t<T>(0) != (m_exor & m_mask); }
            //constexpr bool need_mask() const { return std::make_unsigned_t<T>(~std::make_unsigned_t<T>(0)) != m_mask; }

            //constexpr bool inherited_mask() const { return m_inherited_mask; }


            //constexpr transform_base(std::make_unsigned_t<T> mask) : m_mask(mask) { }
            //constexpr transform_base(transform_base const &) = default;
            //transform_base(transform_base &&) = default;
            //transform_base &operator=(transform_base const &) = default;
            //transform_base &operator=(transform_base &&) = default;
        }


        protected devcb_base(device_t owner)
        {
            m_owner = owner;
            m_dflt_space_finder = new optional_device<device_memory_interface>(owner, finder_base.DUMMY_TAG);
            m_dflt_space_index = -1;
            m_dflt_space = null;


            owner.register_callback(this);
        }

        //~devcb_base() { }


        protected device_t owner() { return m_owner; }

        public void resolve()
        {
            if (m_dflt_space_finder != null && m_dflt_space_finder.target != null && m_dflt_space_finder.target.has_space(m_dflt_space_index))
                m_dflt_space = m_dflt_space_finder.target.space(m_dflt_space_index);
            else
                m_dflt_space = m_owner.machine().dummy_space();
        }

        protected address_space default_space() { assert(m_dflt_space != null); return m_dflt_space; }
    }


    /// \brief Read callback utilities
    ///
    /// Helpers that don't need to be templated on callback type.
    public class devcb_read_base : devcb_base
    {
        // Detecting candidates for read functions
        //template <typename Result, typename Func, typename Enable = void> struct is_read_form1 { static constexpr bool value = false; };
        //template <typename Result, typename Func, typename Enable = void> struct is_read_form2 { static constexpr bool value = false; };
        //template <typename Result, typename Func, typename Enable = void> struct is_read_form3 { static constexpr bool value = false; };
        //template <typename Result, typename Func, typename Enable = void> struct is_read_form4 { static constexpr bool value = false; };
        //template <typename Result, typename Func, typename Enable = void> struct is_read_form5 { static constexpr bool value = false; };
        //template <typename Result, typename Func, typename Enable = void> struct is_read_form6 { static constexpr bool value = false; };
        //template <typename Result, typename Func> struct is_read_form1<Result, Func, std::enable_if_t<std::is_convertible<std::result_of_t<Func (address_space &, offs_t, Result)>, Result>::value> > { static constexpr bool value = true; };
        //template <typename Result, typename Func> struct is_read_form2<Result, Func, std::enable_if_t<std::is_convertible<std::result_of_t<Func (address_space &, offs_t)>, Result>::value> > { static constexpr bool value = true; };
        //template <typename Result, typename Func> struct is_read_form3<Result, Func, std::enable_if_t<std::is_convertible<std::result_of_t<Func (offs_t, Result)>, Result>::value> > { static constexpr bool value = true; };
        //template <typename Result, typename Func> struct is_read_form4<Result, Func, std::enable_if_t<std::is_convertible<std::result_of_t<Func (offs_t)>, Result>::value> > { static constexpr bool value = true; };
        //template <typename Result, typename Func> struct is_read_form5<Result, Func, std::enable_if_t<std::is_convertible<std::result_of_t<Func (address_space &)>, Result>::value> > { static constexpr bool value = true; };
        //template <typename Result, typename Func> struct is_read_form6<Result, Func, std::enable_if_t<std::is_convertible<std::result_of_t<Func ()>, Result>::value> > { static constexpr bool value = true; };
        //template <typename Result, typename Func> struct is_read { static constexpr bool value = is_read_form1<Result, Func>::value || is_read_form2<Result, Func>::value || is_read_form3<Result, Func>::value || is_read_form4<Result, Func>::value || is_read_form5<Result, Func>::value || is_read_form6<Result, Func>::value; };

        // Determining the result type of a read function
        //template <typename Result, typename Func, typename Enable = void> struct read_result;
        //template <typename Result, typename Func> struct read_result<Result, Func, std::enable_if_t<is_read_form1<Result, Func>::value> > { using type = std::result_of_t<Func (address_space &, offs_t, std::make_unsigned_t<Result>)>; };
        //template <typename Result, typename Func> struct read_result<Result, Func, std::enable_if_t<is_read_form2<Result, Func>::value> > { using type = std::result_of_t<Func (address_space &, offs_t)>; };
        //template <typename Result, typename Func> struct read_result<Result, Func, std::enable_if_t<is_read_form3<Result, Func>::value> > { using type = std::result_of_t<Func (offs_t, std::make_unsigned_t<Result>)>; };
        //template <typename Result, typename Func> struct read_result<Result, Func, std::enable_if_t<is_read_form4<Result, Func>::value> > { using type = std::result_of_t<Func (offs_t)>; };
        //template <typename Result, typename Func> struct read_result<Result, Func, std::enable_if_t<is_read_form5<Result, Func>::value> > { using type = std::result_of_t<Func (address_space &)>; };
        //template <typename Result, typename Func> struct read_result<Result, Func, std::enable_if_t<is_read_form6<Result, Func>::value> > { using type = std::result_of_t<Func ()>; };
        //template <typename Result, typename Func> using read_result_t = typename read_result<Result, Func>::type;

        // Detecting candidates for read delegates
        //template <typename T, typename Enable = void> struct is_read_method { static constexpr bool value = false; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read8_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read16_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read32_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read64_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read8s_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read16s_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read32s_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read64s_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read8sm_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read16sm_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read32sm_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read64sm_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read8smo_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read16smo_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read32smo_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read64smo_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_read_method<T, void_t<emu::detail::rw_device_class_t<read_line_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };

        // Invoking read callbacks
        //template <typename Result, typename T> static std::enable_if_t<is_read_form1<Result, T>::value, mask_t<read_result_t<Result, T>, Result> > invoke_read(T const &cb, address_space &space, offs_t offset, std::make_unsigned_t<Result> mem_mask) { return std::make_unsigned_t<read_result_t<Result, T> >(cb(space, offset, mem_mask)); }
        //template <typename Result, typename T> static std::enable_if_t<is_read_form2<Result, T>::value, mask_t<read_result_t<Result, T>, Result> > invoke_read(T const &cb, address_space &space, offs_t offset, std::make_unsigned_t<Result> mem_mask) { return std::make_unsigned_t<read_result_t<Result, T> >(cb(space, offset)); }
        //template <typename Result, typename T> static std::enable_if_t<is_read_form3<Result, T>::value, mask_t<read_result_t<Result, T>, Result> > invoke_read(T const &cb, address_space &space, offs_t offset, std::make_unsigned_t<Result> mem_mask) { return std::make_unsigned_t<read_result_t<Result, T> >(cb(offset, mem_mask)); }
        //template <typename Result, typename T> static std::enable_if_t<is_read_form4<Result, T>::value, mask_t<read_result_t<Result, T>, Result> > invoke_read(T const &cb, address_space &space, offs_t offset, std::make_unsigned_t<Result> mem_mask) { return std::make_unsigned_t<read_result_t<Result, T> >(cb(offset)); }
        //template <typename Result, typename T> static std::enable_if_t<is_read_form5<Result, T>::value, mask_t<read_result_t<Result, T>, Result> > invoke_read(T const &cb, address_space &space, offs_t offset, std::make_unsigned_t<Result> mem_mask) { return std::make_unsigned_t<read_result_t<Result, T> >(cb(space)); }
        //template <typename Result, typename T> static std::enable_if_t<is_read_form6<Result, T>::value, mask_t<read_result_t<Result, T>, Result> > invoke_read(T const &cb, address_space &space, offs_t offset, std::make_unsigned_t<Result> mem_mask) { return std::make_unsigned_t<read_result_t<Result, T> >(cb()); }
        protected static u8 invoke_read(read8_delegate cb, address_space space, offs_t offset, u8 mem_mask) { return cb(space, offset, mem_mask); }
        protected static int invoke_read(read_line_delegate cb) { return cb(); }

        // Delegate characteristics
        //template <typename T, typename Dummy = void> struct delegate_traits;
        //template <typename Dummy> struct delegate_traits<read8_delegate, Dummy> { static constexpr u8 default_mask = ~u8(0); };
        //template <typename Dummy> struct delegate_traits<read16_delegate, Dummy> { static constexpr u16 default_mask = ~u16(0); };
        //template <typename Dummy> struct delegate_traits<read32_delegate, Dummy> { static constexpr u32 default_mask = ~u32(0); };
        //template <typename Dummy> struct delegate_traits<read64_delegate, Dummy> { static constexpr u64 default_mask = ~u64(0); };
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

        //using devcb_base::devcb_base;
        protected devcb_read_base(device_t owner) : base(owner) { }
        //~devcb_read_base() { }
    }


    /// \brief Write callback utilities
    ///
    /// Helpers that don't need to be templated on callback type.
    public class devcb_write_base : devcb_base
    {
        // Detecting candidates for write functions
        //template <typename Input, typename Func, typename Enable = void> struct is_write_form1 { static constexpr bool value = false; };
        //template <typename Input, typename Func, typename Enable = void> struct is_write_form2 { static constexpr bool value = false; };
        //template <typename Input, typename Func, typename Enable = void> struct is_write_form3 { static constexpr bool value = false; };
        //template <typename Input, typename Func, typename Enable = void> struct is_write_form4 { static constexpr bool value = false; };
        //template <typename Input, typename Func, typename Enable = void> struct is_write_form5 { static constexpr bool value = false; };
        //template <typename Input, typename Func, typename Enable = void> struct is_write_form6 { static constexpr bool value = false; };
        //template <typename Input, typename Func> struct is_write_form1<Input, Func, void_t<std::result_of_t<Func (address_space &, offs_t, Input, std::make_unsigned_t<Input>)> > > { static constexpr bool value = true; };
        //template <typename Input, typename Func> struct is_write_form2<Input, Func, void_t<std::result_of_t<Func (address_space &, offs_t, Input)> > > { static constexpr bool value = true; };
        //template <typename Input, typename Func> struct is_write_form3<Input, Func, void_t<std::result_of_t<Func (offs_t, Input, std::make_unsigned_t<Input>)> > > { static constexpr bool value = true; };
        //template <typename Input, typename Func> struct is_write_form4<Input, Func, void_t<std::result_of_t<Func (offs_t, Input)> > > { static constexpr bool value = true; };
        //template <typename Input, typename Func> struct is_write_form5<Input, Func, void_t<std::result_of_t<Func (address_space &, Input)> > > { static constexpr bool value = true; };
        //template <typename Input, typename Func> struct is_write_form6<Input, Func, void_t<std::result_of_t<Func (Input)> > > { static constexpr bool value = true; };
        //template <typename Input, typename Func> struct is_write { static constexpr bool value = is_write_form1<Input, Func>::value || is_write_form2<Input, Func>::value || is_write_form3<Input, Func>::value || is_write_form4<Input, Func>::value || is_write_form5<Input, Func>::value || is_write_form6<Input, Func>::value; };

        // Detecting candidates for write delegates
        //template <typename T, typename Enable = void> struct is_write_method { static constexpr bool value = false; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write8_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write16_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write32_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write64_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write8s_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write16s_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write32s_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write64s_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write8sm_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write16sm_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write32sm_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write64sm_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write8smo_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write16smo_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write32smo_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write64smo_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };
        //template <typename T> struct is_write_method<T, void_t<emu::detail::rw_device_class_t<write_line_delegate, std::remove_reference_t<T> > > > { static constexpr bool value = true; };

        // Invoking write callbacks
        //template <typename Input, typename T> static std::enable_if_t<is_write_form1<Input, T>::value> invoke_write(T const &cb, address_space &space, offs_t &offset, Input data, std::make_unsigned_t<Input> mem_mask) { return cb(space, offset, data, mem_mask); }
        //template <typename Input, typename T> static std::enable_if_t<is_write_form2<Input, T>::value> invoke_write(T const &cb, address_space &space, offs_t &offset, Input data, std::make_unsigned_t<Input> mem_mask) { return cb(space, offset, data); }
        //template <typename Input, typename T> static std::enable_if_t<is_write_form3<Input, T>::value> invoke_write(T const &cb, address_space &space, offs_t &offset, Input data, std::make_unsigned_t<Input> mem_mask) { return cb(offset, data, mem_mask); }
        //template <typename Input, typename T> static std::enable_if_t<is_write_form4<Input, T>::value> invoke_write(T const &cb, address_space &space, offs_t &offset, Input data, std::make_unsigned_t<Input> mem_mask) { return cb(offset, data); }
        //template <typename Input, typename T> static std::enable_if_t<is_write_form5<Input, T>::value> invoke_write(T const &cb, address_space &space, offs_t &offset, Input data, std::make_unsigned_t<Input> mem_mask) { return cb(space, data); }
        //template <typename Input, typename T> static std::enable_if_t<is_write_form6<Input, T>::value> invoke_write(T const &cb, address_space &space, offs_t &offset, Input data, std::make_unsigned_t<Input> mem_mask) { return cb(data); }
        protected static void invoke_write(write8_delegate cb, address_space space, offs_t offset, u8 data, u8 mem_mask) { cb(space, offset, data, mem_mask); }
        protected static void invoke_write(write_line_delegate cb, int param) { cb(param); }

        // Delegate characteristics
        //template <typename T, typename Dummy = void> struct delegate_traits;
        //template <typename Dummy> struct delegate_traits<write8_delegate, Dummy> { using input_t = u8; static constexpr u8 default_mask = ~u8(0); };
        //template <typename Dummy> struct delegate_traits<write16_delegate, Dummy> { using input_t = u16; static constexpr u16 default_mask = ~u16(0); };
        //template <typename Dummy> struct delegate_traits<write32_delegate, Dummy> { using input_t = u32; static constexpr u32 default_mask = ~u32(0); };
        //template <typename Dummy> struct delegate_traits<write64_delegate, Dummy> { using input_t = u64; static constexpr u64 default_mask = ~u64(0); };
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

        //using devcb_base::devcb_base;
        protected devcb_write_base(device_t owner) : base(owner) { }
        //~devcb_write_base() { }
    }


    /// \brief Read callback helper
    ///
    /// Allows binding a variety of signatures, composing a result from
    /// multiple callbacks, and chained arbitrary transforms.  Transforms
    /// may modify the offset and mask.
    //template <typename Result, std::make_unsigned_t<Result> DefaultMask = std::make_unsigned_t<Result>(~std::make_unsigned_t<Result>(0))>
    public class devcb_read : devcb_read_base
    {
        //using func_t = std::function<Result (address_space &, offs_t, std::make_unsigned_t<Result>)>;


        protected abstract class creator
        {
            //using ptr = std::unique_ptr<creator>;


            u32 m_mask;  //std::make_unsigned_t<Result> m_mask;


            protected creator(u32 mask) { m_mask = mask; }
            //~creator() { }

            //protected abstract void validity_check(validity_checker valid);
            public abstract read8_delegate create_r8();  // virtual func_t create() = 0;
            public abstract read_line_delegate create_rl();  // virtual func_t create() = 0;

            u32 mask() { return m_mask; }
        }


        //template <typename T>
        protected class creator_impl : creator
        {
            builder_base_with_transform_base m_builder;  //T m_builder;


            public creator_impl(builder_base_with_transform_base builder) : base(builder.mask()) { m_builder = builder; }

            //protected override void validity_check(validity_checker valid) { m_builder.validity_check(valid); }

            public override read8_delegate create_r8()
            {
                read8_delegate result = null;  // func_t result;
                m_builder.build_r8((read8_delegate f) => { var cb = f;  result = (address_space space, offs_t offset, u8 mem_mask) => { return cb(space, offset, mem_mask); }; });  // m_builder.build([&result] (auto &&f) { result = [cb = std::move(f)] (address_space &space, offs_t offset, typename T::input_mask_t mem_mask) { return cb(space, offset, mem_mask); }; });
                return result;
            }

            public override read_line_delegate create_rl()
            {
                read_line_delegate result = null;  // func_t result;
                m_builder.build_rl((read_line_delegate f) => { var cb = f;  result = () => { return cb(); }; });  // m_builder.build([&result] (auto &&f) { result = [cb = std::move(f)] (address_space &space, offs_t offset, typename T::input_mask_t mem_mask) { return cb(space, offset, mem_mask); }; });
                return result;
            }
        }


#if false
        class log_creator : public creator
        {
        public:
            log_creator(device_t &devbase, std::string &&message) : creator(0U), m_devbase(devbase), m_message(std::move(message)) { }

            virtual void validity_check(validity_checker &valid) const override { }

            virtual func_t create() override
            {
                return
                        [&devbase = m_devbase, message = std::move(m_message)] (address_space &space, offs_t offset, std::make_unsigned_t<Result> mem_mask)
                        {
                            devbase.logerror("%s: %s\n", devbase.machine().describe_context(), message);
                            return Result(0);
                        };
            }

        private:
            device_t &m_devbase;
            std::string m_message;
        };
#endif

        //template <typename Source, typename Func> class transform_builder; // workaround for MSVC


        public class builder_base : IDisposable
        {
            //template <typename T, typename U> friend class transform_builder; // workaround for MSVC

            bool m_disposedValue = false; // To detect redundant calls

            protected devcb_read m_target;
            protected bool m_append;
            protected bool m_consumed = false;
            bool m_built = false;


            public builder_base(devcb_read target, bool append) { m_target = target;  m_append = append; }
            //builder_base(builder_base const &) = delete;
            //builder_base(builder_base &&) = default;
            ~builder_base()
            {
                //global.osd_printf_debug("~builder_base() - {0} - {1} - {2}\n", m_target.owner().owner().GetType(), m_target.owner().tag(), m_target.owner().name());
                assert(m_consumed);
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


            public delegate void build_chain_func_r8(read8_delegate chain);
            public delegate void build_chain_func_rl(read_line_delegate chain);

            public virtual void build_r8(build_chain_func_r8 chain) { }
            public virtual void build_rl(build_chain_func_rl chain) { }


            public void consume() { m_consumed = true; }
            protected void built() { /*global.assert(!m_built);*/ m_built = true; }

            //template <typename T>
            protected void register_creator()
            {
                if (!m_consumed)
                {
                    if (!m_append)
                        m_target.m_creators.clear();

                    consume();

                    m_target.m_creators.emplace_back(new creator_impl((builder_base_with_transform_base)this));
                }
            }
        }


        public abstract class builder_base_with_transform_base : builder_base
        {
            /////// transform_base

            u32 m_exor = 0;  //std::make_unsigned_t<T> m_exor = std::make_unsigned_t<T>(0);
            u32 m_mask;  //std::make_unsigned_t<T> m_mask;
            bool m_inherited_mask = true;


            protected builder_base_with_transform_base(devcb_read target, bool append, u32 mask) : base(target, append) { m_mask = mask; }
            //constexpr transform_base(transform_base const &) = default;
            //transform_base(transform_base &&) = default;
            //transform_base &operator=(transform_base const &) = default;
            //transform_base &operator=(transform_base &&) = default;


            protected abstract builder_base_with_transform_base transform(transform_func cb);


            //Impl &exor(std::make_unsigned_t<T> val) { m_exor ^= val; return static_cast<Impl &>(*this); }
            public builder_base_with_transform_base mask(u8 val) { m_mask = m_inherited_mask ? val : (m_mask & val); m_inherited_mask = false; return this; }  //Impl &mask(std::make_unsigned_t<T> val) { m_mask = m_inherited_mask ? val : (m_mask & val); m_inherited_mask = false; return static_cast<Impl &>(*this); }
            //Impl &invert() { return exor(~std::make_unsigned_t<T>(0)); }

            public builder_base_with_transform_base rshift(u32 val)  //auto rshift(unsigned val)
            {
                var trans = transform((offs_t offset, u32 data, ref u32 mem_mask) => { mem_mask >>= (int)val; return data >> (int)val; });  //auto trans(static_cast<Impl &>(*this).transform([val] (offs_t offset, T data, std::make_unsigned_t<T> &mem_mask) { mem_mask >>= val; return data >> val; }));
                return inherited_mask() ? trans : trans.mask((byte)(m_mask >> (int)val));
            }

            public builder_base_with_transform_base lshift(u32 val)  //auto lshift(unsigned val)
            {
                var trans = transform((offs_t offset, u32 data, ref u32 mem_mask) => { mem_mask <<= (int)val; return data << (int)val; });  //auto trans(static_cast<Impl &>(*this).transform([val] (offs_t offset, T data, std::make_unsigned_t<T> &mem_mask) { mem_mask <<= val; return data << val; }));
                return inherited_mask() ? trans : trans.mask((byte)(m_mask << (int)val));
            }

            //auto bit(unsigned val) { return std::move(rshift(val).mask(T(1U))); }

            protected u32 exor() { return m_exor & m_mask; }  //constexpr std::make_unsigned_t<T> exor() const { return m_exor & m_mask; }
            public u32 mask() { return m_mask; }  //constexpr std::make_unsigned_t<T> mask() const { return m_mask; }

            //constexpr bool need_exor() const { return std::make_unsigned_t<T>(0) != (m_exor & m_mask); }
            //constexpr bool need_mask() const { return std::make_unsigned_t<T>(~std::make_unsigned_t<T>(0)) != m_mask; }

            protected bool inherited_mask() { return m_inherited_mask; }
        }


        //template <typename Source, typename Func>
        class transform_builder : builder_base_with_transform_base//builder_base, public transform_base<mask_t<transform_result_t<typename Source::output_t, Result, Func>, Result>, transform_builder<Source, Func> >
        {
            //template <typename T, typename U> friend class transform_builder;

            //using input_t = typename Source::output_t;
            //using output_t = mask_t<transform_result_t<input_t, Result, Func>, Result>;
            //using input_mask_t = mask_t<input_t, typename Source::input_mask_t>;


            builder_base m_src;  //Source m_src;
            transform_func m_cb;  //Func m_cb;


            //template <typename T>
            public transform_builder(devcb_read target, bool append, builder_base src, transform_func cb, u32 mask)  //, Source &&src, T &&cb, output_t mask)
                : base(target, append, mask)
            {
                //transform_base<output_t, transform_builder>(mask)
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
            protected override void Dispose(bool disposing) { register_creator(); base.Dispose(disposing); }


            //template <typename T>
            //std::enable_if_t<is_transform<output_t, Result, T>::value, transform_builder<transform_builder, std::remove_reference_t<T> > > transform(T &&cb)
            protected override builder_base_with_transform_base transform(transform_func cb)
            {
                u32 /*output_t*/ m = mask();
                if (inherited_mask())
                    mask(u8.MaxValue);  //this.mask(std::make_unsigned_t<transform_result_t<typename Source::output_t, Result, Func> >(~std::make_unsigned_t<transform_result_t<typename Source::output_t, Result, Func> >(0)));
                return new transform_builder(m_target, m_append, this, cb, m);  //return transform_builder<transform_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, std::move(*this), std::forward<T>(cb), m);
            }

            //void validity_check(validity_checker &valid) const { m_src.validity_check(valid); }

            //template <typename T>
            //void build(T &&chain)
            public override void build_r8(build_chain_func_r8 chain)
            {
                assert(m_consumed);
                var c = chain;
                build_chain_func_r8 wrap = (read8_delegate f) => { build_r8(c, f); };  // auto wrap([this, c = std::forward<T>(chain)] (auto &&f) mutable { this->build(std::move(c), std::move(f)); });
                m_src.build_r8(wrap);
            }

            public override void build_rl(build_chain_func_rl chain)
            {
                assert(m_consumed);
                var c = chain;
                build_chain_func_rl wrap = (read_line_delegate f) => { build_rl(c, f); };  // auto wrap([this, c = std::forward<T>(chain)] (auto &&f) mutable { this->build(std::move(c), std::move(f)); });
                m_src.build_rl(wrap);
            }


            //transform_builder(transform_builder const &) = delete;
            //transform_builder &operator=(transform_builder const &) = delete;
            //transform_builder &operator=(transform_builder &&that) = delete;

            //template <typename T, typename U>
            //void build(T &&chain, U &&f)
            void build_r8(build_chain_func_r8 chain, read8_delegate f)
            {
                assert(m_consumed);
                built();
                var src = f;
                var cb = m_cb;
                var exor_ = exor();
                var mask_ = mask();
                chain(
                        (address_space space, offs_t offset, u8 mem_mask) =>  //[src = std::forward<U>(f), cb = std::move(this->m_cb), exor = this->exor(), mask = this->mask()] (address_space &space, offs_t &offset, input_mask_t &mem_mask)
                        {
                            u8 source_mask = mem_mask;
                            var data = src(space, offset, source_mask);
                            mem_mask = (u8)(source_mask & mask_);
                            u32 mem_mask_u32 = mem_mask;
                            return (u8)((invoke_transform(cb, space, offset, data, ref mem_mask_u32) ^ exor_) & mask_);  // return (devcb_read.invoke_transform<input_t, Result>(cb, space, offset, data, mem_mask) ^ exor_) & mask_;
                        });
            }

            void build_rl(build_chain_func_rl chain, read_line_delegate f)
            {
                assert(m_consumed);
                built();
                var src = f;
                var cb = m_cb;
                var exor_ = exor();
                var mask_ = mask();
                chain(
                        () =>  //[src = std::forward<U>(f), cb = std::move(this->m_cb), exor = this->exor(), mask = this->mask()] (address_space &space, offs_t &offset, input_mask_t &mem_mask)
                        {
                            u8 source_mask = u8.MaxValue;  // mem_mask;
                            var data = src();
                            var mem_mask = (u8)(source_mask & mask_);
                            u32 mem_mask_u32 = mem_mask;
                            return (int)((invoke_transform(cb, null, 0, (u32)data, ref mem_mask_u32) ^ exor_) & mask_);  // return (devcb_read.invoke_transform<input_t, Result>(cb, space, offset, data, mem_mask) ^ exor_) & mask_;
                        });
            }
        }


#if false
        template <typename Func>
        class functoid_builder : public builder_base, public transform_base<mask_t<read_result_t<Result, Func>, Result>, functoid_builder<Func> >
        {
        public:
            template <typename T, typename U> friend class transform_builder;

            using output_t = mask_t<read_result_t<Result, Func>, Result>;
            using input_mask_t = std::make_unsigned_t<Result>;

            template <typename T>
            functoid_builder(devcb_read &target, bool append, T &&cb)
                : builder_base(target, append)
                , transform_base<output_t, functoid_builder>(DefaultMask)
                , m_cb(std::forward<T>(cb))
            { }
            functoid_builder(functoid_builder &&that)
                : builder_base(std::move(that))
                , transform_base<output_t, functoid_builder>(std::move(that))
                , m_cb(std::move(that.m_cb))
            {
                that.consume();
                that.built();
            }
            ~functoid_builder() { this->template register_creator<functoid_builder>(); }

            template <typename T>
            std::enable_if_t<is_transform<output_t, Result, T>::value, transform_builder<functoid_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                output_t const m(this->mask());
                if (this->inherited_mask())
                    this->mask(std::make_unsigned_t<read_result_t<Result, Func> >(~std::make_unsigned_t<read_result_t<Result, Func> >(0)));
                return transform_builder<functoid_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, std::move(*this), std::forward<T>(cb), m);
            }

            void validity_check(validity_checker &valid) const { }

            template <typename T>
            void build(T &&chain)
            {
                assert(this->m_consumed);
                this->built();
                chain(
                        [cb = std::move(this->m_cb), exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_mask_t mem_mask)
                        { return (devcb_read::invoke_read<Result>(cb, space, offset, mem_mask & mask) ^ exor) & mask; });
            }

        private:
            functoid_builder(functoid_builder const &) = delete;
            functoid_builder &operator=(functoid_builder const &) = delete;
            functoid_builder &operator=(functoid_builder &&that) = delete;

            Func m_cb;
        };
#endif


        //template <typename Delegate>
        public class delegate_builder : builder_base_with_transform_base//builder_base, transform_base<mask_t<read_result_t<Result, Delegate_>, Result>, delegate_builder<Delegate_> >
        {
            //template <typename T, typename U> friend class transform_builder;

            //using output_t = mask_t<read_result_t<Result, Delegate>, Result>;
            //using input_mask_t = std::make_unsigned_t<Result>;


            device_t m_devbase;
            read8_delegate m_delegate_r8;
            read_line_delegate m_delegate_rl;


            //template <typename T>
            public delegate_builder(devcb_read target, bool append, device_t devbase, string tag, read8_delegate func)//, string name)
                : base(target, append, u32.MaxValue)
            {
                //transform_base<output_t, delegate_builder>(DefaultMask & delegate_traits<Delegate>::default_mask)
                m_devbase = devbase;
                m_delegate_r8 = func;  //m_delegate(std::forward<T>(func), name, tag, std::add_pointer_t<devcb_read::delegate_device_class_t<T> >(nullptr))
            }

            //template <typename T>
            public delegate_builder(devcb_read target, bool append, device_t devbase, string tag, read_line_delegate func)//, string name)
                : base(target, append, u32.MaxValue)
            {
                //transform_base<output_t, delegate_builder>(DefaultMask & delegate_traits<Delegate>::default_mask)
                m_devbase = devbase;
                m_delegate_rl = func;  //m_delegate(std::forward<T>(func), name, tag, std::add_pointer_t<devcb_read::delegate_device_class_t<T> >(nullptr))
            }

            //template <typename T>
            //delegate_builder(devcb_read target, bool append, device_t devbase, devcb_read::delegate_device_class_t<T> obj, T func, string name)
            //{
            //    builder_base(target, append)
            //    transform_base<output_t, delegate_builder>(DefaultMask & delegate_traits<Delegate>::default_mask)
            //    m_devbase(devbase)
            //    m_delegate(std::forward<T>(func), name, devcb_read::get_tag(obj), &obj)
            //}

            //delegate_builder(delegate_builder that)
            //{
            //    builder_base(std::move(that))
            //    transform_base<output_t, delegate_builder>(std::move(that))
            //    m_devbase(that.m_devbase)
            //    m_delegate(std::move(that.m_delegate))
            //
            //
            //    that.consume();
            //    that.built();
            //}

            //~delegate_builder() { this->template register_creator<delegate_builder>(); }
            protected override void Dispose(bool disposing) { register_creator(); base.Dispose(disposing); }


            //template <typename T>
            //std::enable_if_t<is_transform<output_t, Result, T>::value, transform_builder<delegate_builder, std::remove_reference_t<T> > > transform(T &&cb)
            protected override builder_base_with_transform_base transform(transform_func cb)
            {
                u32 /*output_t*/ m = mask();
                if (inherited_mask())
                    mask();  //delegate_traits<Delegate>::default_mask);
                return new transform_builder(m_target, m_append, this, cb, m);  //return transform_builder<delegate_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, std::move(*this), std::forward<T>(cb), m);
            }


            //void validity_check(validity_checker &valid) const
            //{
            //    if (!m_devbase.subdevice(m_delegate.device_name()))
            //        osd_printf_error("Read callback bound to non-existent object tag %s (%s)\n", m_delegate.device_name(), m_delegate.name());
            //}

            //template <typename T>
            //void build(T &&chain)
            public override void build_r8(build_chain_func_r8 chain)
            {
                if (m_delegate_r8 == null)
                    return;  // return without calling the chain callback, which keeps 'result' == null

                assert(m_consumed);
                built();
                //m_delegate.bind_relative_to(m_devbase);
                var cb = m_delegate_r8;
                var exor_ = exor();
                var mask_ = mask();
                chain(
                        (address_space space, offs_t offset, u8 mem_mask) => // [cb = std::move(this->m_delegate), exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_mask_t mem_mask)
                        { return (u8)((invoke_read(cb, space, offset, (u8)(mem_mask & mask_)) ^ exor_) & mask_); });  // { return (devcb_read::invoke_read<Result>(cb, space, offset, mem_mask & mask) ^ exor) & mask; });
            }

            public override void build_rl(build_chain_func_rl chain)
            {
                if (m_delegate_rl == null)
                    return;  // return without calling the chain callback, which keeps 'result' == null

                assert(m_consumed);
                built();
                //m_delegate.bind_relative_to(m_devbase);
                var cb = m_delegate_rl;
                var exor_ = exor();
                var mask_ = mask();
                chain(
                        () => // [cb = std::move(this->m_delegate), exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_mask_t mem_mask)
                        { return invoke_read(cb) & (int)mask_; });  // { return (devcb_read::invoke_read<Result>(cb, space, offset, mem_mask & mask) ^ exor) & mask; });
            }


            //delegate_builder(delegate_builder const &) = delete;
            //delegate_builder &operator=(delegate_builder const &) = delete;
            //delegate_builder &operator=(delegate_builder &&that) = delete;
        }


        public class ioport_builder : builder_base_with_transform_base//builder_base, public transform_base<mask_t<ioport_value, Result>, ioport_builder>
        {
            //template <typename T, typename U> friend class transform_builder;

            //using output_t = mask_t<ioport_value, Result>;
            //using input_mask_t = std::make_unsigned_t<Result>;


            device_t m_devbase;
            string m_tag;


            public ioport_builder(devcb_read target, bool append, device_t devbase, string tag)
                : base(target, append, u32.MaxValue)
            {
                //transform_base<output_t, ioport_builder>(DefaultMask)
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
            protected override void Dispose(bool disposing) { register_creator(); base.Dispose(disposing); }


            //template <typename T>
            //std::enable_if_t<is_transform<output_t, Result, T>::value, transform_builder<ioport_builder, std::remove_reference_t<T> > > transform(T &&cb)
            protected override builder_base_with_transform_base transform(transform_func cb)
            {
                u32 /*output_t*/ m = mask();
                if (inherited_mask())
                    mask(u8.MaxValue);  //this->mask(std::make_unsigned_t<ioport_value>(~std::make_unsigned_t<ioport_value>(0)));
                return new transform_builder(m_target, m_append, this, cb, m);  //return transform_builder<ioport_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, std::move(*this), std::forward<T>(cb), m);
            }

            //void validity_check(validity_checker &valid) const { }

            //template <typename T>
            //void build(T &&chain)
            public override void build_r8(build_chain_func_r8 chain)
            {
                assert(m_consumed);
                built();
                ioport_port ioport = m_devbase.ioport(m_tag.c_str());
                if (ioport == null)
                    throw new emu_fatalerror("Read callback bound to non-existent I/O port {0} of device {1} ({2})\n", m_tag.c_str(), m_devbase.tag(), m_devbase.name());

                var port = ioport;
                var exor_ = exor();
                var mask_ = mask();
                chain(
                        (address_space space, offs_t offset, u8 mem_mask) =>  //[&port = *ioport, exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_mask_t mem_mask)
                        { return (u8)((port.read() ^ exor_) & mask_); });  // { return (port.read() ^ exor) & mask; });
            }

            public override void build_rl(build_chain_func_rl chain)
            {
                assert(m_consumed);
                built();
                ioport_port ioport = m_devbase.ioport(m_tag.c_str());
                if (ioport == null)
                    throw new emu_fatalerror("Read callback bound to non-existent I/O port {0} of device {1} ({2})\n", m_tag.c_str(), m_devbase.tag(), m_devbase.name());

                var port = ioport;
                var exor_ = exor();
                var mask_ = mask();
                chain(
                        () =>  //[&port = *ioport, exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_mask_t mem_mask)
                        { return (int)((port.read() ^ exor_) & mask_); });  // { return (port.read() ^ exor) & mask; });
            }


            //ioport_builder(ioport_builder const &) = delete;
            //ioport_builder &operator=(ioport_builder const &) = delete;
            //ioport_builder &operator=(ioport_builder &&that) = delete;
        }


        public class binder
        {
            devcb_read m_target;
            bool m_append = false;
            bool m_used = false;


            public binder(devcb_read target) { m_target = target; }
            //binder(binder const &) = delete;
            binder(binder that) { m_target = that.m_target; m_append = that.m_append; m_used = that.m_used;   that.m_used = true; }
            //binder &operator=(binder const &) = delete;
            //binder &operator=(binder &&) = delete;

            //template <typename T>
            //std::enable_if_t<is_read<Result, T>::value, functoid_builder<std::remove_reference_t<T> > > set(T &&cb)
            public delegate_builder set(read8_delegate func)
            {
                set_used();
                return new delegate_builder(m_target, m_append, m_target.owner().mconfig().current_device(), DEVICE_SELF, func);
            }

            public delegate_builder set(read_line_delegate func)
            {
                set_used();
                return new delegate_builder(m_target, m_append, m_target.owner().mconfig().current_device(), DEVICE_SELF, func);
            }

#if false
            template <typename T>
            std::enable_if_t<is_read_method<T>::value, delegate_builder<delegate_type_t<T> > > set(T &&func, char const *name)
            {
                set_used();
                return delegate_builder<delegate_type_t<T> >(m_target, m_append, m_target.owner().mconfig().current_device(), DEVICE_SELF, std::forward<T>(func), name);
            }
#endif

            //template <typename T>
            //std::enable_if_t<is_read_method<T>::value, delegate_builder<delegate_type_t<T> > > set(char const *tag, T &&func, char const *name)
            public delegate_builder set(string tag, read8_delegate func)//, char const *name)
            {
                set_used();
                return new delegate_builder(m_target, m_append, m_target.owner().mconfig().current_device(), tag, func);//, name);
            }

            public delegate_builder set(string tag, read_line_delegate func)//, char const *name)
            {
                set_used();
                return new delegate_builder(m_target, m_append, m_target.owner().mconfig().current_device(), tag, func);//, name);
            }

#if false
            template <typename T, typename U>
            std::enable_if_t<is_read_method<T>::value, delegate_builder<delegate_type_t<T> > > set(U &obj, T &&func, char const *name)
            {
                set_used();
                return delegate_builder<delegate_type_t<T> >(m_target, m_append, m_target.owner(), devcb_read::cast_reference<delegate_device_class_t<T> >(obj), std::forward<T>(func), name);
            }
#endif

            //template <typename T, typename U, bool R>
            //std::enable_if_t<is_read_method<T>::value, delegate_builder<delegate_type_t<T> > > set(device_finder<U, R> &finder, T &&func, char const *name)
            //std::enable_if_t<is_read_method<T>::value, delegate_builder<delegate_type_t<T> > > set(device_finder<U, R> const &finder, T &&func, char const *name)
            public delegate_builder set<U>(device_finder<U> finder, read8_delegate func) where U : class //, char const *name)
            {
                set_used();
                var target = finder.finder_target();  //std::pair<device_t &, char const *> const target(finder.finder_target());
                return new delegate_builder(m_target, m_append, target.first(), target.second(), func);  //return delegate_builder<delegate_type_t<T> >(m_target, m_append, target.first, target.second, std::forward<T>(func), name);
            }

            public delegate_builder set<U>(device_finder<U> finder, read_line_delegate func) where U : class //, char const *name)
            {
                set_used();
                var target = finder.finder_target();  //std::pair<device_t &, char const *> const target(finder.finder_target());
                return new delegate_builder(m_target, m_append, target.first(), target.second(), func);  //return delegate_builder<delegate_type_t<T> >(m_target, m_append, target.first, target.second, std::forward<T>(func), name);
            }


            //template <typename... Params>
            //auto append(Params &&... args)
            public delegate_builder append(string tag, read_line_delegate func)
            {
                m_append = true;
                return set(tag, func);
            }


            //template <typename... Params>
            public ioport_builder set_ioport(string args)
            {
                set_used();
                return new ioport_builder(m_target, m_append, m_target.owner().mconfig().current_device(), args);
            }

#if false
            template <bool R>
            ioport_builder set_ioport(ioport_finder<R> &finder)
            {
                set_used();
                std::pair<device_t &, char const *> const target(finder.finder_target());
                return ioport_builder(m_target, m_append, target.first, std::string(target.second));
            }

            template <bool R>
            ioport_builder set_ioport(ioport_finder<R> const &finder)
            {
                set_used();
                std::pair<device_t &, char const *> const target(finder.finder_target());
                return ioport_builder(m_target, m_append, target.first, std::string(target.second));
            }

            template <typename... Params>
            ioport_builder append_ioport(Params &&... args)
            {
                m_append = true;
                return set_ioport(std::forward<Params>(args)...);
            }

            template <typename... Params>
            void set_log(device_t &devbase, Params &&... args)
            {
                set_used();
                if (!m_append)
                    m_target.m_creators.clear();
                m_target.m_creators.emplace_back(std::make_unique<log_creator>(devbase, std::string(std::forward<Params>(args)...)));
            }

            template <typename T, typename... Params>
            std::enable_if_t<emu::detail::is_device_implementation<std::remove_reference_t<T> >::value> set_log(T &devbase, Params &&... args)
            {
                set_log(static_cast<device_t &>(devbase), std::forward<Params>(args)...);
            }

            template <typename T, typename... Params>
            std::enable_if_t<emu::detail::is_device_interface<std::remove_reference_t<T> >::value> set_log(T &devbase, Params &&... args)
            {
                set_log(devbase.device(), std::forward<Params>(args)...);
            }

            template <typename... Params>
            void set_log(Params &&... args)
            {
                set_log(m_target.owner().mconfig().current_device(), std::forward<Params>(args)...);
            }

            template <typename... Params>
            void append_log(Params &&... args)
            {
                m_append = true;
                set_log(std::forward<Params>(args)...);
            }

            auto set_constant(Result val) { return set([val] () { return val; }); }
            auto append_constant(Result val) { return append([val] () { return val; }); }
#endif


            void set_used() { assert(!m_used); m_used = true; }
        }


        protected std.vector<read8_delegate> m_functions_r8 = new std.vector<read8_delegate>();  //std::vector<func_t> m_functions;
        protected std.vector<read_line_delegate> m_functions_rl = new std.vector<read_line_delegate>();  //std::vector<func_t> m_functions;
        protected std.vector<creator_impl> m_creators = new std.vector<creator_impl>();  //std::vector<typename creator::ptr> m_creators;


        //template <typename Result, std::make_unsigned_t<Result> DefaultMask>
        //devcb_read<Result, DefaultMask>::devcb_read(device_t &owner)
        protected devcb_read(device_t owner)
        : base(owner)
        {
        }


        //template <typename Result, std::make_unsigned_t<Result> DefaultMask>
        public binder bind()
        {
            return new binder(this);
        }

        //void reset();


        //virtual void validity_check(validity_checker &valid) const override;


        //template <typename Result, std::make_unsigned_t<Result> DefaultMask>
        public new void resolve()
        {
            assert(m_functions_r8.empty());
            assert(m_functions_rl.empty());

            base.resolve();

            m_functions_r8.reserve(m_creators.size());
            m_functions_rl.reserve(m_creators.size());

            foreach (var c in m_creators)  //for (typename creator::ptr const &c : m_creators)
            {
                var create_r8 = c.create_r8();
                if (create_r8 != null) m_functions_r8.emplace_back(create_r8);
                var create_rl = c.create_rl();
                if (create_rl != null) m_functions_rl.emplace_back(create_rl);
            }

            m_creators.clear();
        }


        //template <typename Result, std::make_unsigned_t<Result> DefaultMask>
        //void devcb_read<Result, DefaultMask>::resolve_safe(Result dflt)
        public void resolve_safe(int dflt)
        {
            resolve();

            if (m_functions_r8.empty())
                m_functions_r8.emplace_back((address_space space, offs_t offset, u8 mem_mask) => { return (u8)dflt; });
            if (m_functions_rl.empty())
                m_functions_rl.emplace_back(() => { return dflt; });
        }


        //Result operator()(address_space &space, offs_t offset = 0, std::make_unsigned_t<Result> mem_mask = DefaultMask);
        //template <typename Result, std::make_unsigned_t<Result> DefaultMask>
        //Result devcb_read<Result, DefaultMask>::operator()(address_space &space, offs_t offset, std::make_unsigned_t<Result> mem_mask)
        //{
        //    assert(m_creators.empty() && !m_functions.empty());
        //    typename std::vector<func_t>::const_iterator it(m_functions.begin());
        //    std::make_unsigned_t<Result> result((*it)(space, offset, mem_mask));
        //    while (m_functions.end() != ++it)
        //        result |= (*it)(space, offset, mem_mask);
        //    return result;
        //}

        //Result operator()(offs_t offset, std::make_unsigned_t<Result> mem_mask = DefaultMask);
        //template <typename Result, std::make_unsigned_t<Result> DefaultMask>
        //Result devcb_read<Result, DefaultMask>::operator()(offs_t offset, std::make_unsigned_t<Result> mem_mask)
        //{
        //    return this->operator()(this->default_space(), offset, mem_mask);
        //}

        //Result operator()();
        //template <typename Result, std::make_unsigned_t<Result> DefaultMask>
        //Result devcb_read<Result, DefaultMask>::operator()()
        //{
        //    return this->operator()(this->default_space(), 0U, DefaultMask);
        //}


        public bool isnull() { return m_functions_r8.empty() && m_functions_rl.empty() && m_creators.empty(); }

        //explicit operator bool() const { return !m_functions.empty(); }
        //public bool op() { return !m_functions_r8.empty() || !m_functions_rl.empty(); }
    }


    /// \brief Write callback helper
    ///
    /// Allows binding a variety of signatures, sending the value to
    /// multiple callbacks, and chained arbitrary transforms.  Transforms
    /// may modify the offset and mask.
    //template <typename Input, std::make_unsigned_t<Input> DefaultMask = std::make_unsigned_t<Input>(~std::make_unsigned_t<Input>(0))>
    public class devcb_write : devcb_write_base
    {
        //using func_t = std::function<void (address_space &, offs_t, Input, std::make_unsigned_t<Input>)>;
        public delegate void func_t();


        protected abstract class creator
        {
            //using ptr = std::unique_ptr<creator>;

            //~creator() { }

            //protected abstract void validity_check(validity_checker valid);
            public abstract write8_delegate create_w8();
            public abstract write32_delegate create_w32();
            public abstract write_line_delegate create_wl();
        }


        //template <typename T>
        protected class creator_impl : creator
        {
            builder_base m_builder;  //T m_builder;


            public creator_impl(builder_base builder) { m_builder = builder; }

            //protected override void validity_check(validity_checker valid) { m_builder.validity_check(valid); }

            public override write8_delegate create_w8()
            {
                var cb = m_builder.build_w8();
                return (address_space space, offs_t offset, u8 data, u8 mem_mask) => { cb(space, offset, data, mem_mask); };  //return [cb = m_builder.build()] (address_space space, offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask) { cb(space, offset, data, mem_mask); };
            }

            public override write32_delegate create_w32()
            {
                var cb = m_builder.build_w32();
                return (address_space space, offs_t offset, u32 data, u32 mem_mask) => { cb(space, offset, data, mem_mask); };  //return [cb = m_builder.build()] (address_space space, offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask) { cb(space, offset, data, mem_mask); };
            }

            public override write_line_delegate create_wl()
            {
                var cb = m_builder.build_wl();
                return (int param) => { cb(param); };  // return [cb = m_builder.build()] (address_space &space, offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask) { cb(space, offset, data, mem_mask); };
            }
        }

#if false
        class nop_creator : public creator
        {
        public:
            virtual void validity_check(validity_checker &valid) const override { }
            virtual func_t create() override { return [] (address_space &space, offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask) { }; }
        };
#endif


        //template <typename Source, typename Func> class transform_builder; // workaround for MSVC
        //template <typename Sink, typename Func> class first_transform_builder; // workaround for MSVC
        //template <typename Func> class functoid_builder; // workaround for MSVC


        public class builder_base : IDisposable
        {
            //template <typename T, typename U> friend class transform_builder; // workaround for MSVC
            //template <typename T, typename U> friend class first_transform_builder; // workaround for MSVC
            //template <typename Func> friend class functoid_builder; // workaround for MSVC


            bool m_disposedValue = false; // To detect redundant calls


            devcb_write m_target;
            bool m_append;
            protected bool m_consumed = false;
            bool m_built = false;


            public builder_base(devcb_write target, bool append) { m_target = target;  m_append = append; }
            //builder_base(builder_base const &) = delete;
            //builder_base(builder_base &&) = default;
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

            public virtual write8_delegate build_w8() { return null; }
            public virtual write32_delegate build_w32() { return null; }
            public virtual write_line_delegate build_wl() { return null; }

            void consume() { m_consumed = true; }
            protected void built() { /*global.assert(!m_built);*/ m_built = true; }

            //template <typename T>
            protected void register_creator()
            {
                if (!m_consumed)
                {
                    if (!m_append)
                        m_target.m_creators.clear();

                    consume();

                    m_target.m_creators.emplace_back(new creator_impl(this));
                }
            }
        }


        public abstract class builder_base_with_transform_base : builder_base
        {
            /////// transform_base

            u32 m_exor = 0;  //std::make_unsigned_t<T> m_exor = std::make_unsigned_t<T>(0);
            u32 m_mask;  //std::make_unsigned_t<T> m_mask;
            bool m_inherited_mask = true;


            protected builder_base_with_transform_base(devcb_write target, bool append, u32 mask) : base(target, append) { m_mask = mask; }
            //constexpr transform_base(transform_base const &) = default;
            //transform_base(transform_base &&) = default;
            //transform_base &operator=(transform_base const &) = default;
            //transform_base &operator=(transform_base &&) = default;


            //protected abstract builder_base_with_transform_base transform(transform_func cb);


            public builder_base_with_transform_base exor(u32 val) { m_exor ^= val; return this; }  //Impl &exor(std::make_unsigned_t<T> val) { m_exor ^= val; return static_cast<Impl &>(*this); }
            public builder_base_with_transform_base mask(u8 val) { m_mask = m_inherited_mask ? val : (m_mask & val); m_inherited_mask = false; return this; }  //Impl &mask(std::make_unsigned_t<T> val) { m_mask = m_inherited_mask ? val : (m_mask & val); m_inherited_mask = false; return static_cast<Impl &>(*this); }
            public builder_base_with_transform_base invert() { return exor(u32.MaxValue); }  //Impl &invert() { return exor(~std::make_unsigned_t<T>(0)); }

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

            protected u32 exor() { return m_exor & m_mask; }
            protected u32 mask() { return m_mask; }

            //constexpr bool need_exor() const { return std::make_unsigned_t<T>(0) != (m_exor & m_mask); }
            //constexpr bool need_mask() const { return std::make_unsigned_t<T>(~std::make_unsigned_t<T>(0)) != m_mask; }

            //constexpr bool inherited_mask() const { return m_inherited_mask; }
        }


#if false
        template <typename Source, typename Func>
        class transform_builder : public builder_base, public transform_base<mask_t<transform_result_t<typename Source::output_t, typename Source::output_t, Func>, typename Source::output_t>, transform_builder<Source, Func> >
        {
        public:
            template <typename T, typename U> friend class transform_builder;

            using input_t = typename Source::output_t;
            using output_t = mask_t<transform_result_t<typename Source::output_t, typename Source::output_t, Func>, typename Source::output_t>;

            template <typename T>
            transform_builder(devcb_write &target, bool append, Source &&src, T &&cb, output_t mask)
                : builder_base(target, append)
                , transform_base<output_t, transform_builder>(mask)
                , m_src(std::move(src))
                , m_cb(std::forward<T>(cb))
            { m_src.consume(); }
            transform_builder(transform_builder &&that)
                : builder_base(std::move(that))
                , transform_base<output_t, transform_builder>(std::move(that))
                , m_src(std::move(that.m_src))
                , m_cb(std::move(that.m_cb))
            {
                m_src.consume();
                that.consume();
                that.built();
            }
            ~transform_builder() { this->template register_creator<transform_builder>(); }

            template <typename T>
            std::enable_if_t<is_transform<output_t, output_t, T>::value, transform_builder<transform_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                output_t const m(this->mask());
                if (this->inherited_mask())
                    this->mask(output_t(~output_t(0)));
                return transform_builder<transform_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, std::move(*this), std::forward<T>(cb), m);
            }

            auto build()
            {
                assert(this->m_consumed);
                this->built();
                return m_src.build(
                        [cb = std::move(m_cb), exor = this->exor(), mask = this->mask()] (address_space &space, offs_t &offset, input_t data, std::make_unsigned_t<input_t> &mem_mask)
                        {
                            auto const trans(devcb_write::invoke_transform<input_t, output_t>(cb, space, offset, data, mem_mask));
                            mem_mask &= mask;
                            return (trans ^ exor) & mask;
                        });
            }

            void validity_check(validity_checker &valid) const { m_src.validity_check(valid); }

        private:
            transform_builder(transform_builder const &) = delete;
            transform_builder &operator=(transform_builder const &) = delete;
            transform_builder &operator=(transform_builder &&that) = delete;

            template <typename T>
            auto build(T &&chain)
            {
                assert(this->m_consumed);
                this->built();
                return m_src.build(
                        [f = std::move(chain), cb = std::move(this->m_cb), exor = this->exor(), mask = this->mask()] (address_space &space, offs_t &offset, input_t data, std::make_unsigned_t<input_t> &mem_mask)
                        {
                            auto const trans(devcb_write::invoke_transform<input_t, output_t>(cb, space, offset, data, mem_mask));
                            output_t out_mask(mem_mask & mask);
                            return f(space, offset, (trans ^ exor) & mask, out_mask);
                        });
            }

            Source m_src;
            Func m_cb;
        };
#endif

#if false
        template <typename Sink, typename Func>
        class first_transform_builder : public builder_base, public transform_base<mask_t<transform_result_t<typename Sink::input_t, typename Sink::input_t, Func>, typename Sink::input_t>, first_transform_builder<Sink, Func> >
        {
        public:
            template <typename T, typename U> friend class transform_builder;

            using input_t = typename Sink::input_t;
            using output_t = mask_t<transform_result_t<typename Sink::input_t, typename Sink::input_t, Func>, typename Sink::input_t>;

            template <typename T>
            first_transform_builder(devcb_write &target, bool append, Sink &&sink, T &&cb, std::make_unsigned_t<Input> in_exor, std::make_unsigned_t<Input> in_mask, std::make_unsigned_t<output_t> mask)
                : builder_base(target, append)
                , transform_base<output_t, first_transform_builder>(mask)
                , m_sink(std::move(sink))
                , m_cb(std::forward<T>(cb))
                , m_in_exor(in_exor & in_mask)
                , m_in_mask(in_mask)
            { m_sink.consume(); }
            first_transform_builder(first_transform_builder &&that)
                : builder_base(std::move(that))
                , transform_base<output_t, first_transform_builder>(std::move(that))
                , m_sink(std::move(that.m_sink))
                , m_cb(std::move(that.m_cb))
                , m_in_exor(that.m_in_exor)
                , m_in_mask(that.m_in_mask)
            {
                m_sink.consume();
                that.consume();
                that.built();
            }
            ~first_transform_builder() { this->template register_creator<first_transform_builder>(); }

            void validity_check(validity_checker &valid) const { m_sink.validity_check(valid); }

            template <typename T>
            std::enable_if_t<is_transform<output_t, output_t, T>::value, transform_builder<first_transform_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                output_t const m(this->mask());
                if (this->inherited_mask())
                    this->mask(output_t(~output_t(0)));
                return transform_builder<first_transform_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, std::move(*this), std::forward<T>(cb), m);
            }

            auto build()
            {
                assert(this->m_consumed);
                this->built();
                return
                        [sink = m_sink.build(), cb = std::move(this->m_cb), in_exor = m_in_exor, in_mask = m_in_mask, exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        {
                            data = (data ^ in_exor) & in_mask;
                            mem_mask &= in_mask;
                            auto const trans(devcb_write::invoke_transform<input_t, output_t>(cb, space, offset, data, mem_mask));
                            mem_mask &= mask;
                            sink(space, offset, (trans ^ exor) & mask, mem_mask);
                        };
            }

        private:
            first_transform_builder(first_transform_builder const &) = delete;
            first_transform_builder operator=(first_transform_builder const &) = delete;
            first_transform_builder operator=(first_transform_builder &&that) = delete;

            template <typename T>
            auto build(T &&chain)
            {
                assert(this->m_consumed);
                this->built();
                return
                        [f = std::move(chain), sink = m_sink.build(), cb = std::move(this->m_cb), in_exor = m_in_exor, in_mask = m_in_mask, exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        {
                            data = (data ^ in_exor) & in_mask;
                            mem_mask &= in_mask;
                            auto const trans_1(devcb_write::invoke_transform<input_t, output_t>(cb, space, offset, data, mem_mask));
                            output_t out_mask(mem_mask & mask);
                            auto const trans_n(f(space, offset, (trans_1 ^ exor) & mask, out_mask));
                            sink(space, offset, trans_n, out_mask);
                        };
            }

            Sink m_sink;
            Func m_cb;
            std::make_unsigned_t<Input> const m_in_exor, m_in_mask;
        };
#endif

#if false
        template <typename Func>
        class functoid_builder : public builder_base, public transform_base<std::make_unsigned_t<Input>, functoid_builder<Func> >
        {
        private:
            class wrapped_builder : public builder_base
            {
            public:
                template <typename T, typename U> friend class first_transform_builder;

                using input_t = Input;

                wrapped_builder(functoid_builder &&that) : builder_base(std::move(that)), m_cb(std::move(that.m_cb)) { that.consume(); that.built(); }
                wrapped_builder(wrapped_builder &&that) : builder_base(std::move(that)), m_cb(std::move(that.m_cb)) { that.consume(); that.built(); }

                void validity_check(validity_checker &valid) const { }

                auto build()
                {
                    assert(this->m_consumed);
                    this->built();
                    return
                            [cb = std::move(this->m_cb)] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                            { devcb_write::invoke_write<Input>(cb, space, offset, data, mem_mask); };
                }

            private:
                wrapped_builder(wrapped_builder const &) = delete;
                wrapped_builder operator=(wrapped_builder const &) = delete;
                wrapped_builder operator=(wrapped_builder &&that) = delete;

                Func m_cb;
            };

            functoid_builder(functoid_builder const &) = delete;
            functoid_builder &operator=(functoid_builder const &) = delete;
            functoid_builder &operator=(functoid_builder &&that) = delete;

            Func m_cb;

        public:
            using input_t = Input;

            template <typename T>
            functoid_builder(devcb_write &target, bool append, T &&cb)
                : builder_base(target, append)
                , transform_base<std::make_unsigned_t<Input>, functoid_builder>(DefaultMask)
                , m_cb(std::forward<T>(cb))
            { }
            functoid_builder(functoid_builder &&that)
                : builder_base(std::move(that))
                , transform_base<std::make_unsigned_t<Input>, functoid_builder>(std::move(that))
                , m_cb(std::move(that.m_cb))
            {
                that.consume();
                that.built();
            }
            ~functoid_builder() { this->template register_creator<functoid_builder>(); }

            template <typename T>
            std::enable_if_t<is_transform<input_t, input_t, T>::value, first_transform_builder<wrapped_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                return first_transform_builder<wrapped_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, wrapped_builder(std::move(*this)), std::forward<T>(cb), this->exor(), this->mask(), DefaultMask);
            }

            void validity_check(validity_checker &valid) const { }

            auto build()
            {
                assert(this->m_consumed);
                this->built();
                return
                        [cb = std::move(this->m_cb), exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { devcb_write::invoke_write<Input>(cb, space, offset, (data ^ exor) & mask, mem_mask & mask); };
            }
        };
#endif


        //template <typename Delegate>
        public class delegate_builder : builder_base_with_transform_base//builder_base, transform_base<mask_t<Input, typename delegate_traits<Delegate_>::input_t>, delegate_builder<Delegate_> >
        {
#if false
            class wrapped_builder : builder_base
            {
                //template <typename T, typename U> friend class first_transform_builder;

                //using input_t = intermediate_t<Input, typename delegate_traits<Delegate>::input_t>;


                //device_t m_devbase;
                //Delegate_ m_delegate;


                //wrapped_builder(delegate_builder that)
                //{
                //    builder_base(std::move(that))
                //    m_devbase(that.m_devbase)
                //    m_delegate(std::move(that.m_delegate))
                //
                //
                //    that.consume();
                //    that.built();
                //}

                //wrapped_builder(wrapped_builder that)
                //{
                //    builder_base(std::move(that))
                //    m_devbase(that.m_devbase)
                //    m_delegate(std::move(that.m_delegate))
                //
                //
                //    that.consume();
                //    that.built();
                //}


                //void validity_check(validity_checker &valid) const
                //{
                //    if (!m_devbase.subdevice(m_delegate.device_name()))
                //        osd_printf_error("Write callback bound to non-existent object tag %s (%s)\n", m_delegate.device_name(), m_delegate.name());
                //}

                //auto build()
                //{
                //    assert(this->m_consumed);
                //    this->built();
                //    m_delegate.bind_relative_to(m_devbase);
                //    return
                //            [cb = std::move(this->m_delegate)] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                //            { devcb_write::invoke_write<Input>(cb, space, offset, data, mem_mask); };
                //}


                //wrapped_builder(wrapped_builder const &) = delete;
                //wrapped_builder operator=(wrapped_builder const &) = delete;
                //wrapped_builder operator=(wrapped_builder &&that) = delete;
            }
#endif

            //friend class wrapped_builder; // workaround for MSVC

            //delegate_builder(delegate_builder const &) = delete;
            //delegate_builder &operator=(delegate_builder const &) = delete;
            //delegate_builder &operator=(delegate_builder &&that) = delete;


            device_t m_devbase;
            write8_delegate m_delegate_w8;  //Delegate_ m_delegate;
            write_line_delegate m_delegate_wl;  //Delegate_ m_delegate;


            //using input_t = intermediate_t<Input, typename delegate_traits<Delegate>::input_t>;

            //template <typename T>
            public delegate_builder(devcb_write target, bool append, device_t devbase, string tag, write_line_delegate func)
                : base(target, append, u32.MaxValue)
            {
                //transform_base<mask_t<Input, typename delegate_traits<Delegate>::input_t>, delegate_builder>(DefaultMask & delegate_traits<Delegate>::default_mask)
                m_devbase = devbase;
                m_delegate_wl = func;  //(std::forward<T>(func), name, tag, std::add_pointer_t<devcb_write::delegate_device_class_t<T> >(nullptr))
            }

            public delegate_builder(devcb_write target, bool append, device_t devbase, string tag, write8_delegate func)
                : base(target, append, u32.MaxValue)
            {
                //transform_base<mask_t<Input, typename delegate_traits<Delegate>::input_t>, delegate_builder>(DefaultMask & delegate_traits<Delegate>::default_mask)
                m_devbase = devbase;
                m_delegate_w8 = func;  //(std::forward<T>(func), name, tag, std::add_pointer_t<devcb_write::delegate_device_class_t<T> >(nullptr))
            }

            //template <typename T>
            //delegate_builder(devcb_write target, bool append, device_t devbase, devcb_write::delegate_device_class_t<T> obj, T func, string name)
            //{
            //    builder_base(target, append)
            //    transform_base<mask_t<Input, typename delegate_traits<Delegate>::input_t>, delegate_builder>(DefaultMask & delegate_traits<Delegate>::default_mask)
            //    m_devbase(devbase)
            //    m_delegate(std::forward<T>(func), name, devcb_write::get_tag(obj), &obj)
            //}

            //delegate_builder(delegate_builder that)
            //{
            //    builder_base(std::move(that))
            //    transform_base<mask_t<Input, typename delegate_traits<Delegate>::input_t>, delegate_builder>(std::move(that))
            //    m_devbase(that.m_devbase)
            //    m_delegate(std::move(that.m_delegate))
            //
            //
            //    that.consume();
            //    that.built();
            //}

            //~delegate_builder() { this->template register_creator<delegate_builder>(); }
            protected override void Dispose(bool disposing) { register_creator(); base.Dispose(disposing); }


            //template <typename T>
            //std::enable_if_t<is_transform<input_t, input_t, T>::value, first_transform_builder<wrapped_builder, std::remove_reference_t<T> > > transform(T &&cb)
            //{
            //    std::make_unsigned_t<Input> const in_mask(this->inherited_mask() ? DefaultMask : this->mask());
            //    mask_t<Input, typename delegate_traits<Delegate>::input_t> const out_mask(DefaultMask & delegate_traits<Delegate>::default_mask);
            //    return first_transform_builder<wrapped_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, wrapped_builder(std::move(*this)), std::forward<T>(cb), this->exor(), in_mask, out_mask);
            //}

            //void validity_check(validity_checker &valid) const
            //{
            //    if (!m_devbase.subdevice(m_delegate.device_name()))
            //        osd_printf_error("Write callback bound to non-existent object tag %s (%s)\n", m_delegate.device_name(), m_delegate.name());
            //}


            //auto build()
            public override write8_delegate build_w8()
            {
                assert(m_consumed);
                built();
                //m_delegate.bind_relative_to(m_devbase);

                var cb = m_delegate_w8;
                var exor_ = exor();
                var mask_ = mask();
                return
                        (address_space space, offs_t offset, u8 data, u8 mem_mask) => //[cb = std::move(this->m_delegate), exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { invoke_write(cb, space, offset, (u8)((data ^ exor_) & mask_), (u8)(mem_mask & mask_)); };  //{ devcb_write::invoke_write<Input>(cb, space, offset, (data ^ exor) & mask, mem_mask & mask); };
            }

            public override write_line_delegate build_wl()
            {
                assert(m_consumed);
                built();
                //m_delegate.bind_relative_to(m_devbase);

                var cb = m_delegate_wl;
                var exor_ = exor();
                var mask_ = mask();
                return
                        (int param) => //[cb = std::move(this->m_delegate), exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { invoke_write(cb, param); };  //{ devcb_write::invoke_write<Input>(cb, space, offset, (data ^ exor) & mask, mem_mask & mask); };
            }
        }


        public class inputline_builder : builder_base_with_transform_base//builder_base, public transform_base<mask_t<Input, int>, inputline_builder>
        {
#if false
            class wrapped_builder : public builder_base
            {
            public:
                template <typename T, typename U> friend class first_transform_builder;

                using input_t = intermediate_t<Input, int>;

                wrapped_builder(inputline_builder &&that)
                    : builder_base(std::move(that))
                    , m_devbase(that.m_devbase)
                    , m_tag(that.m_tag)
                    , m_exec(that.m_exec)
                    , m_linenum(that.m_linenum)
                {
                    that.consume();
                    that.built();
                }
                wrapped_builder(wrapped_builder &&that)
                    : builder_base(std::move(that))
                    , m_devbase(that.m_devbase)
                    , m_tag(that.m_tag)
                    , m_exec(that.m_exec)
                    , m_linenum(that.m_linenum)
                {
                    that.consume();
                    that.built();
                }

                void validity_check(validity_checker &valid) const
                {
                    if (!m_exec)
                    {
                        device_t *const device(m_devbase.subdevice(m_tag));
                        if (!device)
                            osd_printf_error("Write callback bound to non-existent object tag %s\n", m_tag);
                        else if (!dynamic_cast<device_execute_interface *>(device))
                            osd_printf_error("Write callback bound to device %s (%s) that does not implement device_execute_interface\n", device->tag(), device->name());
                    }
                }

                auto build()
                {
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
                            [&exec = *m_exec, linenum = m_linenum] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                            { exec.set_input_line(linenum, data); };
                }

            private:
                wrapped_builder(wrapped_builder const &) = delete;
                wrapped_builder operator=(wrapped_builder const &) = delete;
                wrapped_builder operator=(wrapped_builder &&that) = delete;

                device_t &m_devbase;
                char const *const m_tag;
                device_execute_interface *m_exec;
                int const m_linenum;
            };
#endif


            //friend class wrapped_builder; // workaround for MSVC

            //inputline_builder(inputline_builder const &) = delete;
            //inputline_builder &operator=(inputline_builder const &) = delete;
            //inputline_builder &operator=(inputline_builder &&that) = delete;

            device_t m_devbase;
            string m_tag;
            device_execute_interface m_exec;
            int m_linenum;


            //using input_t = intermediate_t<Input, int>;

            public inputline_builder(devcb_write target, bool append, device_t devbase, string tag, int linenum)
                : base(target, append, 1)
            {
                //transform_base<mask_t<Input, int>, inputline_builder>(1U)
                m_devbase = devbase;
                m_tag = tag;
                m_exec = null;
                m_linenum = linenum;
            }

            //inputline_builder(devcb_write target, bool append, device_execute_interface exec, int linenum)
            //    : builder_base(target, append)
            //{
            //    transform_base<mask_t<Input, int>, inputline_builder>(1U)
            //    m_devbase(exec.device())
            //    m_tag(exec.device().tag())
            //    m_exec(&exec)
            //    m_linenum(linenum)
            //}

            //inputline_builder(inputline_builder that)
            //    : builder_base(std::move(that))
            //{
            //    transform_base<mask_t<Input, int>, inputline_builder>(std::move(that))
            //    m_devbase(that.m_devbase)
            //    m_tag(that.m_tag)
            //    m_exec(that.m_exec)
            //    m_linenum(that.m_linenum)
            //
            //
            //    that.consume();
            //    that.built();
            //}

            //~inputline_builder() { this->template register_creator<inputline_builder>(); }
            protected override void Dispose(bool disposing) { register_creator(); base.Dispose(disposing); }


            //template <typename T>
            //std::enable_if_t<is_transform<input_t, input_t, T>::value, first_transform_builder<wrapped_builder, std::remove_reference_t<T> > > transform(T &&cb)
            //{
            //    std::make_unsigned_t<Input> const in_mask(this->inherited_mask() ? DefaultMask : this->mask());
            //    return first_transform_builder<wrapped_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, wrapped_builder(std::move(*this)), std::forward<T>(cb), this->exor(), in_mask, 1U);
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


            //auto build()
            public override write_line_delegate build_wl()
            {
                assert(m_consumed);
                built();
                if (m_exec == null)
                {
                    device_t device = m_devbase.subdevice(m_tag);
                    if (device == null)
                        throw new emu_fatalerror("Write callback bound to non-existent object tag {0}\n", m_tag);

                    m_exec = device.GetClassInterface<device_execute_interface>();  // m_exec = dynamic_cast<device_execute_interface *>(device);
                    if (m_exec == null)
                        throw new emu_fatalerror("Write callback bound to device {0} ({1}) that does not implement device_execute_interface\n", device.tag(), device.name());
                }

                var exec = m_exec;
                var linenum = m_linenum;
                //var exor_ = exor();
                //var mask_ = mask();
                return
                        (int data) =>  // [&exec = *m_exec, linenum = m_linenum, exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { var exor_ = exor(); var mask_ = mask(); exec.set_input_line(linenum, (int)((data ^ exor_) & mask_)); };
            }
        }


#if false
        class latched_inputline_builder : public builder_base, public transform_base<std::make_unsigned_t<Input>, latched_inputline_builder>
        {
        private:
            class wrapped_builder : public builder_base
            {
            public:
                template <typename T, typename U> friend class first_transform_builder;

                using input_t = Input;

                wrapped_builder(latched_inputline_builder &&that)
                    : builder_base(std::move(that))
                    , m_devbase(that.m_devbase)
                    , m_tag(that.m_tag)
                    , m_exec(that.m_exec)
                    , m_linenum(that.m_linenum)
                    , m_value(that.m_value)
                {
                    that.consume();
                    that.built();
                }
                wrapped_builder(wrapped_builder &&that)
                    : builder_base(std::move(that))
                    , m_devbase(that.m_devbase)
                    , m_tag(that.m_tag)
                    , m_exec(that.m_exec)
                    , m_linenum(that.m_linenum)
                    , m_value(that.m_value)
                {
                    that.consume();
                    that.built();
                }

                void validity_check(validity_checker &valid) const
                {
                    if (!m_exec)
                    {
                        device_t *const device(m_devbase.subdevice(m_tag));
                        if (!device)
                            osd_printf_error("Write callback bound to non-existent object tag %s\n", m_tag);
                        else if (!dynamic_cast<device_execute_interface *>(device))
                            osd_printf_error("Write callback bound to device %s (%s) that does not implement device_execute_interface\n", device->tag(), device->name());
                    }
                }

                auto build()
                {
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
                            [&exec = *m_exec, linenum = m_linenum, value = m_value] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                            { if (data) exec.set_input_line(linenum, value); };
                }

            private:
                wrapped_builder(wrapped_builder const &) = delete;
                wrapped_builder operator=(wrapped_builder const &) = delete;
                wrapped_builder operator=(wrapped_builder &&that) = delete;

                device_t &m_devbase;
                char const *const m_tag;
                device_execute_interface *m_exec;
                int const m_linenum;
                int const m_value;
            };

            friend class wrapped_builder; // workaround for MSVC

            latched_inputline_builder(latched_inputline_builder const &) = delete;
            latched_inputline_builder &operator=(latched_inputline_builder const &) = delete;
            latched_inputline_builder &operator=(latched_inputline_builder &&that) = delete;

            device_t &m_devbase;
            char const *const m_tag;
            device_execute_interface *m_exec;
            int const m_linenum;
            int const m_value;

        public:
            using input_t = Input;

            latched_inputline_builder(devcb_write &target, bool append, device_t &devbase, char const *tag, int linenum, int value)
                : builder_base(target, append)
                , transform_base<std::make_unsigned_t<Input>, latched_inputline_builder>(DefaultMask)
                , m_devbase(devbase)
                , m_tag(tag)
                , m_exec(nullptr)
                , m_linenum(linenum)
                , m_value(value)
            { }
            latched_inputline_builder(devcb_write &target, bool append, device_execute_interface &exec, int linenum, int value)
                : builder_base(target, append)
                , transform_base<std::make_unsigned_t<Input>, latched_inputline_builder>(DefaultMask)
                , m_devbase(exec.device())
                , m_tag(exec.device().tag())
                , m_exec(&exec)
                , m_linenum(linenum)
                , m_value(value)
            { }
            latched_inputline_builder(latched_inputline_builder &&that)
                : builder_base(std::move(that))
                , transform_base<std::make_unsigned_t<Input>, latched_inputline_builder>(std::move(that))
                , m_devbase(that.m_devbase)
                , m_tag(that.m_tag)
                , m_exec(that.m_exec)
                , m_linenum(that.m_linenum)
                , m_value(that.m_value)
            {
                that.consume();
                that.built();
            }
            ~latched_inputline_builder() { this->template register_creator<latched_inputline_builder>(); }

            template <typename T>
            std::enable_if_t<is_transform<input_t, input_t, T>::value, first_transform_builder<wrapped_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                return first_transform_builder<wrapped_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, wrapped_builder(std::move(*this)), std::forward<T>(cb), this->exor(), this->mask(), DefaultMask);
            }

            void validity_check(validity_checker &valid) const
            {
                if (!m_exec)
                {
                    device_t *const device(m_devbase.subdevice(m_tag));
                    if (!device)
                        osd_printf_error("Write callback bound to non-existent object tag %s\n", m_tag);
                    else if (!dynamic_cast<device_execute_interface *>(device))
                        osd_printf_error("Write callback bound to device %s (%s) that does not implement device_execute_interface\n", device->tag(), device->name());
                }
            }

            auto build()
            {
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
                        [&exec = *m_exec, linenum = m_linenum, value = m_value, exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { if ((data ^ exor) & mask) exec.set_input_line(linenum, value); };
            }
        };
#endif

#if false
        class ioport_builder : public builder_base, public transform_base<mask_t<Input, ioport_value>, ioport_builder>
        {
        private:
            class wrapped_builder : public builder_base
            {
            public:
                template <typename T, typename U> friend class first_transform_builder;

                using input_t = intermediate_t<Input, ioport_value>;

                wrapped_builder(ioport_builder &&that)
                    : builder_base(std::move(that))
                    , m_devbase(that.m_devbase)
                    , m_tag(std::move(that.m_tag))
                {
                    that.consume();
                    that.built();
                }
                wrapped_builder(wrapped_builder &&that)
                    : builder_base(std::move(that))
                    , m_devbase(that.m_devbase)
                    , m_tag(std::move(that.m_tag))
                {
                    that.consume();
                    that.built();
                }

                void validity_check(validity_checker &valid) const { }

                auto build()
                {
                    assert(this->m_consumed);
                    this->built();
                    ioport_port *const ioport(m_devbase.ioport(m_tag.c_str()));
                    if (!ioport)
                        throw emu_fatalerror("Write callback bound to non-existent I/O port %s of device %s (%s)\n", m_tag.c_str(), m_devbase.tag(), m_devbase.name());
                    return
                            [&port = *ioport] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                            { port.write(data); };
                }

            private:
                wrapped_builder(wrapped_builder const &) = delete;
                wrapped_builder operator=(wrapped_builder const &) = delete;
                wrapped_builder operator=(wrapped_builder &&that) = delete;

                device_t &m_devbase;
                std::string m_tag;
            };

            friend class wrapped_builder; // workaround for MSVC

            ioport_builder(ioport_builder const &) = delete;
            ioport_builder &operator=(ioport_builder const &) = delete;
            ioport_builder &operator=(ioport_builder &&that) = delete;

            device_t &m_devbase;
            std::string m_tag;

        public:
            using input_t = intermediate_t<Input, ioport_value>;

            ioport_builder(devcb_write &target, bool append, device_t &devbase, std::string &&tag)
                : builder_base(target, append)
                , transform_base<mask_t<Input, ioport_value>, ioport_builder>(DefaultMask)
                , m_devbase(devbase)
                , m_tag(std::move(tag))
            { }
            ioport_builder(ioport_builder &&that)
                : builder_base(std::move(that))
                , transform_base<mask_t<Input, ioport_value>, ioport_builder>(std::move(that))
                , m_devbase(that.m_devbase)
                , m_tag(std::move(that.m_tag))
            {
                that.consume();
                that.built();
            }
            ~ioport_builder() { this->template register_creator<ioport_builder>(); }

            template <typename T>
            std::enable_if_t<is_transform<input_t, input_t, T>::value, first_transform_builder<wrapped_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                return first_transform_builder<wrapped_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, wrapped_builder(std::move(*this)), std::forward<T>(cb), this->exor(), this->mask(), DefaultMask);
            }

            void validity_check(validity_checker &valid) const { }

            auto build()
            {
                assert(this->m_consumed);
                this->built();
                ioport_port *const ioport(m_devbase.ioport(m_tag.c_str()));
                if (!ioport)
                    throw emu_fatalerror("Write callback bound to non-existent I/O port %s of device %s (%s)\n", m_tag.c_str(), m_devbase.tag(), m_devbase.name());
                return
                        [&port = *ioport, exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { port.write((data ^ exor) & mask); };
            }
        };
#endif

#if false
        class membank_builder : public builder_base, public transform_base<mask_t<Input, int>, membank_builder>
        {
        private:
            class wrapped_builder : public builder_base
            {
            public:
                template <typename T, typename U> friend class first_transform_builder;

                using input_t = intermediate_t<Input, int>;

                wrapped_builder(membank_builder &&that)
                    : builder_base(std::move(that))
                    , m_devbase(that.m_devbase)
                    , m_tag(std::move(that.m_tag))
                {
                    that.consume();
                    that.built();
                }
                wrapped_builder(wrapped_builder &&that)
                    : builder_base(std::move(that))
                    , m_devbase(that.m_devbase)
                    , m_tag(std::move(that.m_tag))
                {
                    that.consume();
                    that.built();
                }

                void validity_check(validity_checker &valid) const { }

                auto build()
                {
                    assert(this->m_consumed);
                    this->built();
                    memory_bank *const bank(m_devbase.membank(m_tag.c_str()));
                    if (!bank)
                        throw emu_fatalerror("Write callback bound to non-existent memory bank %s of device %s (%s)\n", m_tag.c_str(), m_devbase.tag(), m_devbase.name());
                    return
                            [&membank = *bank] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                            { membank.set_entry(data); };
                }

            private:
                wrapped_builder(wrapped_builder const &) = delete;
                wrapped_builder operator=(wrapped_builder const &) = delete;
                wrapped_builder operator=(wrapped_builder &&that) = delete;

                device_t &m_devbase;
                std::string m_tag;
            };

            friend class wrapped_builder; // workaround for MSVC

            membank_builder(membank_builder const &) = delete;
            membank_builder &operator=(membank_builder const &) = delete;
            membank_builder &operator=(membank_builder &&that) = delete;

            device_t &m_devbase;
            std::string m_tag;

        public:
            using input_t = intermediate_t<Input, int>;

            membank_builder(devcb_write &target, bool append, device_t &devbase, std::string &&tag)
                : builder_base(target, append)
                , transform_base<mask_t<Input, int>, membank_builder>(DefaultMask)
                , m_devbase(devbase)
                , m_tag(std::move(tag))
            { }
            membank_builder(membank_builder &&that)
                : builder_base(std::move(that))
                , transform_base<mask_t<Input, int>, membank_builder>(std::move(that))
                , m_devbase(that.m_devbase)
                , m_tag(std::move(that.m_tag))
            {
                that.consume();
                that.built();
            }
            ~membank_builder() { this->template register_creator<membank_builder>(); }

            template <typename T>
            std::enable_if_t<is_transform<input_t, input_t, T>::value, first_transform_builder<wrapped_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                return first_transform_builder<wrapped_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, wrapped_builder(std::move(*this)), std::forward<T>(cb), this->exor(), this->mask(), DefaultMask);
            }

            void validity_check(validity_checker &valid) const { }

            auto build()
            {
                assert(this->m_consumed);
                this->built();
                memory_bank *const bank(m_devbase.membank(m_tag.c_str()));
                if (!bank)
                    throw emu_fatalerror("Write callback bound to non-existent memory bank %s of device %s (%s)\n", m_tag.c_str(), m_devbase.tag(), m_devbase.name());
                return
                        [&membank = *bank, exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { membank.set_entry((data ^ exor) & mask); };
            }
        };
#endif


        public class output_builder : builder_base_with_transform_base//builder_base, public transform_base<mask_t<Input, s32>, output_builder>
        {
#if false
            class wrapped_builder : public builder_base
            {
            public:
                template <typename T, typename U> friend class first_transform_builder;

                using input_t = intermediate_t<Input, s32>;

                wrapped_builder(output_builder &&that)
                    : builder_base(std::move(that))
                    , m_devbase(that.m_devbase)
                    , m_tag(std::move(that.m_tag))
                {
                    that.consume();
                    that.built();
                }
                wrapped_builder(wrapped_builder &&that)
                    : builder_base(std::move(that))
                    , m_devbase(that.m_devbase)
                    , m_tag(std::move(that.m_tag))
                {
                    that.consume();
                    that.built();
                }

                void validity_check(validity_checker &valid) const { }

                auto build()
                {
                    assert(this->m_consumed);
                    this->built();
                    return
                            [&item = m_devbase.machine().output().find_or_create_item(m_tag.c_str(), 0)] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                            { item.set(data); };
                }

            private:
                wrapped_builder(wrapped_builder const &) = delete;
                wrapped_builder operator=(wrapped_builder const &) = delete;
                wrapped_builder operator=(wrapped_builder &&that) = delete;

                device_t &m_devbase;
                std::string m_tag;
            };
#endif

            //friend class wrapped_builder; // workaround for MSVC

            //output_builder(output_builder const &) = delete;
            //output_builder &operator=(output_builder const &) = delete;
            //output_builder &operator=(output_builder &&that) = delete;

            device_t m_devbase;
            string m_tag;


            //using input_t = intermediate_t<Input, s32>;


            public output_builder(devcb_write target, bool append, device_t devbase, string tag)
                : base(target, append, u32.MaxValue)
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
            protected override void Dispose(bool disposing) { register_creator(); base.Dispose(disposing); }


            //template <typename T>
            //std::enable_if_t<is_transform<input_t, input_t, T>::value, first_transform_builder<wrapped_builder, std::remove_reference_t<T> > > transform(T &&cb)
            //{
            //    return first_transform_builder<wrapped_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, wrapped_builder(std::move(*this)), std::forward<T>(cb), this->exor(), this->mask(), DefaultMask);
            //}

            //void validity_check(validity_checker &valid) const { }

            //auto build()
            //{
            //    assert(this->m_consumed);
            //    this->built();
            //    return
            //            [&item = m_devbase.machine().output().find_or_create_item(m_tag.c_str(), 0), exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
            //            { item.set((data ^ exor) & mask); };
            //}
            public override write8_delegate build_w8()
            {
                assert(m_consumed);
                built();

                var item = m_devbase.machine().output().find_or_create_item(m_tag.c_str(), 0);
                var exor_ = exor();
                var mask_ = mask();
                return
                        (address_space space, offs_t offset, u8 data, u8 mem_mask) => //[&item = m_devbase.machine().output().find_or_create_item(m_tag.c_str(), 0), exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { item.set((s32)((data ^ exor_) & mask_)); };  //{ item.set((data ^ exor) & mask); };
            }

            public override write_line_delegate build_wl()
            {
                assert(m_consumed);
                built();

                var item = m_devbase.machine().output().find_or_create_item(m_tag.c_str(), 0);
                var exor_ = exor();
                var mask_ = mask();
                return
                        (int param) => //[&item = m_devbase.machine().output().find_or_create_item(m_tag.c_str(), 0), exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { item.set((s32)((param ^ exor_) & mask_)); };  //{ item.set((data ^ exor) & mask); };
            }
        }

#if false
        class log_builder : public builder_base, public transform_base<std::make_unsigned_t<Input>, log_builder>
        {
        private:
            class wrapped_builder : public builder_base
            {
            public:
                template <typename T, typename U> friend class first_transform_builder;

                using input_t = Input;

                wrapped_builder(log_builder &&that)
                    : builder_base(std::move(that))
                    , m_devbase(that.m_devbase)
                    , m_message(std::move(that.m_message))
                {
                    that.consume();
                    that.built();
                }
                wrapped_builder(wrapped_builder &&that)
                    : builder_base(std::move(that))
                    , m_devbase(that.m_devbase)
                    , m_message(std::move(that.m_message))
                {
                    that.consume();
                    that.built();
                }

                void validity_check(validity_checker &valid) const { }

                auto build()
                {
                    assert(this->m_consumed);
                    this->built();
                    return
                            [&devbase = m_devbase, message = std::move(m_message)] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                            { if (data) devbase.logerror("%s: %s\n", devbase.machine().describe_context(), message); };
                }

            private:
                wrapped_builder(wrapped_builder const &) = delete;
                wrapped_builder operator=(wrapped_builder const &) = delete;
                wrapped_builder operator=(wrapped_builder &&that) = delete;

                device_t &m_devbase;
                std::string m_message;
            };

            friend class wrapped_builder; // workaround for MSVC

            log_builder(log_builder const &) = delete;
            log_builder &operator=(log_builder const &) = delete;
            log_builder &operator=(log_builder &&that) = delete;

            device_t &m_devbase;
            std::string m_message;

        public:
            using input_t = Input;

            log_builder(devcb_write &target, bool append, device_t &devbase, std::string &&message)
                : builder_base(target, append)
                , transform_base<std::make_unsigned_t<Input>, log_builder>(DefaultMask)
                , m_devbase(devbase)
                , m_message(std::move(message))
            { }
            log_builder(log_builder &&that)
                : builder_base(std::move(that))
                , transform_base<std::make_unsigned_t<Input>, log_builder>(std::move(that))
                , m_devbase(that.m_devbase)
                , m_message(std::move(that.m_message))
            {
                that.consume();
                that.built();
            }
            ~log_builder() { this->template register_creator<log_builder>(); }

            template <typename T>
            std::enable_if_t<is_transform<input_t, input_t, T>::value, first_transform_builder<wrapped_builder, std::remove_reference_t<T> > > transform(T &&cb)
            {
                return first_transform_builder<wrapped_builder, std::remove_reference_t<T> >(this->m_target, this->m_append, wrapped_builder(std::move(*this)), std::forward<T>(cb), this->exor(), this->mask(), DefaultMask);
            }

            void validity_check(validity_checker &valid) const { }

            auto build()
            {
                assert(this->m_consumed);
                this->built();
                return
                        [&devbase = m_devbase, message = std::move(m_message), exor = this->exor(), mask = this->mask()] (address_space &space, offs_t offset, input_t data, std::make_unsigned_t<input_t> mem_mask)
                        { if ((data ^ exor) & mask) devbase.logerror("%s: %s\n", devbase.machine().describe_context(), message); };
            }
        };
#endif

        public class binder
        {
            devcb_write m_target;
            bool m_append = false;
            bool m_used = false;


            public binder(devcb_write target) { m_target = target; }
            //binder(binder const &) = delete;
            binder(binder that) { m_target = that.m_target; m_append = that.m_append; m_used = that.m_used;  that.m_used = true; }
            //binder &operator=(binder const &) = delete;
            //binder &operator=(binder &&) = delete;

#if false
            template <typename T>
            std::enable_if_t<is_write<Input, T>::value, functoid_builder<std::remove_reference_t<T> > > set(T &&cb)
            {
                set_used();
                return functoid_builder<std::remove_reference_t<T> >(m_target, m_append, std::forward<T>(cb));
            }
#endif

            //template <typename T>
            //std::enable_if_t<is_write_method<T>::value, delegate_builder<delegate_type_t<T> > > set(T &&func, char const *name)
            public delegate_builder set(write_line_delegate func)//, string name)
            {
                set_used();
                return new delegate_builder(m_target, m_append, m_target.owner().mconfig().current_device(), DEVICE_SELF, func);  // delegate_builder<delegate_type_t<T> >(m_target, m_append, m_target.owner().mconfig().current_device(), DEVICE_SELF, std::forward<T>(func), name);
            }

            public delegate_builder set(write8_delegate func)//, string name)
            {
                set_used();
                return new delegate_builder(m_target, m_append, m_target.owner().mconfig().current_device(), DEVICE_SELF, func);  // delegate_builder<delegate_type_t<T> >(m_target, m_append, m_target.owner().mconfig().current_device(), DEVICE_SELF, std::forward<T>(func), name);
            }

            //template <typename T>
            //std::enable_if_t<is_write_method<T>::value, delegate_builder<delegate_type_t<T> > > set(char const *tag, T &&func, char const *name)
            public delegate_builder set(string tag, write_line_delegate func)//, string name)
            {
                return set(func);
            }

            public delegate_builder set(string tag, write8_delegate func)//, string name)
            {
                return set(func);
            }

#if false
            template <typename T, typename U>
            std::enable_if_t<is_write_method<T>::value, delegate_builder<delegate_type_t<T> > > set(U &obj, T &&func, char const *name)
            {
                set_used();
                return delegate_builder<delegate_type_t<T> >(m_target, m_append, m_target.owner(), devcb_write::cast_reference<delegate_device_class_t<T> >(obj), std::forward<T>(func), name);
            }
#endif

            //template <typename T, typename U, bool R>
            //std::enable_if_t<is_write_method<T>::value, delegate_builder<delegate_type_t<T> > > set(device_finder<U, R> &finder, T &&func, char const *name)
            //std::enable_if_t<is_write_method<T>::value, delegate_builder<delegate_type_t<T> > > set(device_finder<U, R> const &finder, T &&func, char const *name)
            public delegate_builder set<U>(device_finder<U> finder, write8_delegate func) where U : class //, string name)
            {
                set_used();
                var target = finder.finder_target();  //std::pair<device_t &, char const *> const target(finder.finder_target());
                return new delegate_builder(m_target, m_append, target.first(), target.second(), func);  //return delegate_builder<delegate_type_t<T> >(m_target, m_append, target.first, target.second, std::forward<T>(func), name);
            }

            public delegate_builder set<U>(device_finder<U> finder, write_line_delegate func) where U : class //, string name)
            {
                set_used();
                var target = finder.finder_target();  //std::pair<device_t &, char const *> const target(finder.finder_target());
                return new delegate_builder(m_target, m_append, target.first(), target.second(), func);  //return delegate_builder<delegate_type_t<T> >(m_target, m_append, target.first, target.second, std::forward<T>(func), name);
            }


            //template <typename... Params>
            //auto append(Params &&... args)
            public delegate_builder append(write_line_delegate func)
            {
                m_append = true;
                return set(func);
            }


            public inputline_builder set_inputline(string tag, int linenum)
            {
                set_used();
                return new inputline_builder(m_target, m_append, m_target.owner().mconfig().current_device(), tag, linenum);
            }

#if false
            latched_inputline_builder set_inputline(char const *tag, int linenum, int value)
            {
                set_used();
                return latched_inputline_builder(m_target, m_append, m_target.owner().mconfig().current_device(), tag, linenum, value);
            }

            inputline_builder set_inputline(device_execute_interface &obj, int linenum)
            {
                set_used();
                return inputline_builder(m_target, m_append, obj, linenum);
            }

            latched_inputline_builder set_inputline(device_execute_interface &obj, int linenum, int value)
            {
                set_used();
                return latched_inputline_builder(m_target, m_append, obj, linenum, value);
            }
#endif


            //template <typename T, bool R>
            public inputline_builder set_inputline<T>(device_finder<T> finder, int linenum) where T : class
            {
                //set_used();
                KeyValuePair<device_t, string> target = finder.finder_target();
                return new inputline_builder(m_target, m_append, target.first(), target.second(), linenum);
            }


#if false
            template <typename T, bool R>
            latched_inputline_builder set_inputline(device_finder<T, R> const &finder, int linenum, int value)
            {
                set_used();
                std::pair<device_t &, char const *> const target(finder.finder_target());
                return latched_inputline_builder(m_target, m_append, target.first, target.second, linenum, value);
            }
#endif

            //template <typename... Params>
            //auto append_inputline(Params &&... args)
            public inputline_builder append_inputline(string tag, int linenum)
            {
                m_append = true;
                return set_inputline(tag, linenum);
            }

#if false
            template <typename... Params>
            ioport_builder set_ioport(Params &&... args)
            {
                set_used();
                return ioport_builder(m_target, m_append, m_target.owner().mconfig().current_device(), std::string(std::forward<Params>(args)...));
            }

            template <bool R>
            ioport_builder set_ioport(ioport_finder<R> &finder)
            {
                set_used();
                std::pair<device_t &, char const *> const target(finder.finder_target());
                return ioport_builder(m_target, m_append, target.first, std::string(target.second));
            }

            template <bool R>
            ioport_builder set_ioport(ioport_finder<R> const &finder)
            {
                set_used();
                std::pair<device_t &, char const *> const target(finder.finder_target());
                return ioport_builder(m_target, m_append, target.first, std::string(target.second));
            }

            template <typename... Params>
            ioport_builder append_ioport(Params &&... args)
            {
                m_append = true;
                return set_ioport(std::forward<Params>(args)...);
            }

            template <typename... Params>
            membank_builder set_membank(Params &&... args)
            {
                set_used();
                return membank_builder(m_target, m_append, m_target.owner().mconfig().current_device(), std::string(std::forward<Params>(args)...));
            }

            template <bool R>
            membank_builder set_membank(memory_bank_finder<R> &finder)
            {
                set_used();
                std::pair<device_t &, char const *> const target(finder.finder_target());
                return membank_builder(m_target, m_append, target.first, std::string(target.second));
            }

            template <bool R>
            membank_builder set_membank(memory_bank_finder<R> const &finder)
            {
                set_used();
                std::pair<device_t &, char const *> const target(finder.finder_target());
                return membank_builder(m_target, m_append, target.first, std::string(target.second));
            }

            template <typename... Params>
            membank_builder append_membank(Params &&... args)
            {
                m_append = true;
                return set_membank(std::forward<Params>(args)...);
            }
#endif

            //template <typename... Params>
            //output_builder set_output(Params &&... args)
            public output_builder set_output(string tag)
            {
                set_used();
                return new output_builder(m_target, m_append, m_target.owner().mconfig().current_device(), tag);
            }

#if false
            template <typename... Params>
            output_builder append_output(Params &&... args)
            {
                m_append = true;
                return set_output(std::forward<Params>(args)...);
            }

            template <typename... Params>
            log_builder set_log(device_t &devbase, Params &&... args)
            {
                set_used();
                return log_builder(m_target, m_append, devbase, std::string(std::forward<Params>(args)...));
            }

            template <typename T, typename... Params>
            std::enable_if_t<emu::detail::is_device_implementation<std::remove_reference_t<T> >::value, log_builder> set_log(T &devbase, Params &&... args)
            {
                return set_log(static_cast<device_t &>(devbase), std::forward<Params>(args)...);
            }

            template <typename T, typename... Params>
            std::enable_if_t<emu::detail::is_device_interface<std::remove_reference_t<T> >::value, log_builder> set_log(T &devbase, Params &&... args)
            {
                return set_log(devbase.device(), std::forward<Params>(args)...);
            }

            template <typename... Params>
            log_builder set_log(Params &&... args)
            {
                return set_log(m_target.owner().mconfig().current_device(), std::forward<Params>(args)...);
            }

            template <typename... Params>
            log_builder append_log(Params &&... args)
            {
                m_append = true;
                return set_log(std::forward<Params>(args)...);
            }

            void set_nop()
            {
                set_used();
                m_target.m_creators.clear();
                m_target.m_creators.emplace_back(std::make_unique<nop_creator>());
            }
#endif


            void set_used() { assert(!m_used); m_used = true; }
        }


        protected std.vector<write8_delegate> m_functions_w8 = new std.vector<write8_delegate>();  //std::vector<func_t> m_functions;
        protected std.vector<write32_delegate> m_functions_w32 = new std.vector<write32_delegate>();  //std::vector<func_t> m_functions;
        protected std.vector<write_line_delegate> m_functions_wl = new std.vector<write_line_delegate>();  //std::vector<func_t> m_functions;
        protected std.vector<creator_impl> m_creators = new std.vector<creator_impl>();  //std::vector<typename creator::ptr> m_creators;


        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        //devcb_write<Input, DefaultMask>::devcb_write(device_t &owner)
        protected devcb_write(device_t owner)
        : base(owner)
        {
        }


        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        public binder bind()
        {
            return new binder(this);
        }

        //void reset();


        //virtual void validity_check(validity_checker &valid) const override;


        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        public new void resolve()
        {
            assert(m_functions_w8.empty());
            assert(m_functions_w32.empty());
            assert(m_functions_wl.empty());

            base.resolve();

            m_functions_w8.reserve(m_creators.size());
            m_functions_w32.reserve(m_creators.size());
            m_functions_wl.reserve(m_creators.size());

            foreach (var c in m_creators)
            {
                var create_w8 = c.create_w8();
                if (create_w8 != null) m_functions_w8.emplace_back(create_w8);
                var create_w32 = c.create_w32();
                if (create_w32 != null) m_functions_w32.emplace_back(create_w32);
                var create_wl = c.create_wl();
                if (create_wl != null) m_functions_wl.emplace_back(create_wl);
            }

            m_creators.clear();
        }


        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        //void devcb_write<Input, DefaultMask>::resolve_safe()
        public void resolve_safe()
        {
            resolve();

            if (m_functions_w8.empty())
                m_functions_w8.emplace_back((address_space space, offs_t offset, byte data, byte mem_mask) => { });
            if (m_functions_w32.empty())
                m_functions_w32.emplace_back((address_space space, offs_t offset, u32 data, u32 mem_mask) => { });
            if (m_functions_wl.empty())
                m_functions_wl.emplace_back((int data) => { });
        }


        //void operator()(address_space &space, offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask = DefaultMask);
        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        //void devcb_write<Input, DefaultMask>::operator()(address_space &space, offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask)
        //{
        //    assert(m_creators.empty() && !m_functions.empty());
        //    typename std::vector<func_t>::const_iterator it(m_functions.begin());
        //    (*it)(space, offset, data, mem_mask);
        //    while (m_functions.end() != ++it)
        //        (*it)(space, offset, data, mem_mask);
        //}

        //void operator()(address_space &space, Input data);
        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        //void devcb_write<Input, DefaultMask>::operator()(address_space &space, Input data)
        //{
        //    this->operator()(space, 0U, data, DefaultMask);
        //}

        //void operator()(offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask = DefaultMask);
        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        //void devcb_write<Input, DefaultMask>::operator()(offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask)
        //{
        //    this->operator()(this->default_space(), offset, data, mem_mask);
        //}

        //void operator()(Input data);
        //template <typename Input, std::make_unsigned_t<Input> DefaultMask>
        //void devcb_write<Input, DefaultMask>::operator()(Input data)
        //{
        //    this->operator()(this->default_space(), 0U, data, DefaultMask);
        //}


        public bool isnull() { return m_functions_w8.empty() && m_functions_w32.empty() && m_functions_wl.empty() && m_creators.empty(); }

        //explicit operator bool() const { return !m_functions.empty(); }
        public bool op() { return !m_functions_w8.empty() || !m_functions_w32.empty() || !m_functions_wl.empty(); }
    }


    class devcb_read8 : devcb_read/*<u8>*/
    {
        const u8 DefaultMask = u8.MaxValue;  //std::make_unsigned_t<Result> DefaultMask = std::make_unsigned_t<Result>(~std::make_unsigned_t<Result>(0))>

        public devcb_read8(device_t owner) : base(owner) { }

        //Result operator()(address_space &space, offs_t offset = 0, std::make_unsigned_t<Result> mem_mask = DefaultMask);
        //Result operator()(offs_t offset, std::make_unsigned_t<Result> mem_mask = DefaultMask);
        //Result operator()();
        public u8 op(address_space space, offs_t offset = 0, u8 mem_mask = DefaultMask)
        {
            assert(m_creators.empty() && !m_functions_r8.empty());  //assert(m_creators.empty() && !m_functions.empty());

            //typename std::vector<func_t>::const_iterator it(m_functions.begin());
            //std::make_unsigned_t<Result> result((*it)(space, offset, mem_mask));
            //while (m_functions.end() != ++it)
            //    result |= (*it)(space, offset, mem_mask);
            u8 result = 0;
            foreach (var func in m_functions_r8)
                result |= func(space, offset, mem_mask);

            return result;
        }
        public u8 op(offs_t offset, u8 mem_mask = DefaultMask) { return op(default_space(), offset, mem_mask); }  // return this->operator()(this->default_space(), offset, mem_mask);
        public u8 op() { return op(default_space(), 0, DefaultMask); }  // return this->operator()(this->default_space(), 0U, DefaultMask);
    }
    //using devcb_read16 = devcb_read<u16>;
    //using devcb_read32 = devcb_read<u32>;
    //using devcb_read64 = devcb_read<u64>;
    class devcb_read_line : devcb_read/*<int, 1U>*/
    {
        const u32 DefaultMask = 1;  // std::make_unsigned_t<Input> DefaultMask = std::make_unsigned_t<Input>(~std::make_unsigned_t<Input>(0))>

        public devcb_read_line(device_t owner) : base(owner) { }

        //Result operator()(address_space &space, offs_t offset = 0, std::make_unsigned_t<Result> mem_mask = DefaultMask);
        //Result operator()(offs_t offset, std::make_unsigned_t<Result> mem_mask = DefaultMask);
        //Result operator()();
        public int op(address_space space, offs_t offset = 0, u32 mem_mask = DefaultMask)
        {
            assert(m_creators.empty() && !m_functions_rl.empty());  //assert(m_creators.empty() && !m_functions.empty());

            //typename std::vector<func_t>::const_iterator it(m_functions.begin());
            //std::make_unsigned_t<Result> result((*it)(space, offset, mem_mask));
            //while (m_functions.end() != ++it)
            //    result |= (*it)(space, offset, mem_mask);
            int result = 0;
            foreach (var func in m_functions_rl)
                result |= func();

            return result;
        }
        public int op(offs_t offset, u32 mem_mask = DefaultMask) { return op(default_space(), offset, mem_mask); }  // return this->operator()(this->default_space(), offset, mem_mask);
        public int op() { return op(default_space(), 0, DefaultMask); }  // return this->operator()(this->default_space(), 0U, DefaultMask);
    }

    class devcb_write8 : devcb_write/*<u8>*/
    {
        const u8 DefaultMask = u8.MaxValue;  // std::make_unsigned_t<Input> DefaultMask = std::make_unsigned_t<Input>(~std::make_unsigned_t<Input>(0))>

        public devcb_write8(device_t owner) : base(owner) { }

        //void operator()(address_space &space, offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask = DefaultMask);
        //void operator()(address_space &space, Input data);
        //void operator()(offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask = DefaultMask);
        //void operator()(Input data);
        public void op(address_space space, offs_t offset, u8 data, u8 mem_mask = DefaultMask)
        {
            assert(m_creators.empty() && !m_functions_w8.empty());  //assert(m_creators.empty() && !m_functions.empty());

            //typename std::vector<func_t>::const_iterator it(m_functions.begin());
            //(*it)(space, offset, data, mem_mask);
            //while (m_functions.end() != ++it)
            //    (*it)(space, offset, data, mem_mask);
            foreach (var func in m_functions_w8)
                func(space, offset, data, mem_mask);
        }
        public void op(address_space space, u8 data) { op(space, 0U, data, DefaultMask); }
        public void op(offs_t offset, u8 data, u8 mem_mask = DefaultMask) { op(default_space(), offset, data, mem_mask); }
        public void op(u8 data) { op(default_space(), 0U, data, DefaultMask); }
    }

    //using devcb_write16 = devcb_write<u16>;

    class devcb_write32 : devcb_write/*<u32>*/
    {
        const u32 DefaultMask = u32.MaxValue;  // std::make_unsigned_t<Input> DefaultMask = std::make_unsigned_t<Input>(~std::make_unsigned_t<Input>(0))>

        public devcb_write32(device_t owner) : base(owner) { }

        //void operator()(address_space &space, offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask = DefaultMask);
        //void operator()(address_space &space, Input data);
        //void operator()(offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask = DefaultMask);
        //void operator()(Input data);
        public void op(address_space space, offs_t offset, u32 data, u32 mem_mask = DefaultMask)
        {
            assert(m_creators.empty() && !m_functions_w32.empty());  //assert(m_creators.empty() && !m_functions.empty());

            //typename std::vector<func_t>::const_iterator it(m_functions.begin());
            //(*it)(space, offset, data, mem_mask);
            //while (m_functions.end() != ++it)
            //    (*it)(space, offset, data, mem_mask);
            foreach (var func in m_functions_w32)
                func(space, offset, data, mem_mask);
        }
        public void op(address_space space, u32 data) { op(space, 0U, data, DefaultMask); }
        public void op(offs_t offset, u32 data, u32 mem_mask = DefaultMask) { op(default_space(), offset, data, mem_mask); }
        public void op(u32 data) { op(default_space(), 0U, data, DefaultMask); }
    }

    //using devcb_write64 = devcb_write<u64>;

    public class devcb_write_line : devcb_write/*<int, 1U>*/
    {
        const u32 DefaultMask = 1;  // std::make_unsigned_t<Input> DefaultMask = std::make_unsigned_t<Input>(~std::make_unsigned_t<Input>(0))>

        public devcb_write_line(device_t owner) : base(owner) { }

        //void operator()(address_space &space, offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask = DefaultMask);
        //void operator()(address_space &space, Input data);
        //void operator()(offs_t offset, Input data, std::make_unsigned_t<Input> mem_mask = DefaultMask);
        //void operator()(Input data);
        public void op(address_space space, offs_t offset, int data, u32 mem_mask = DefaultMask)
        {
            assert(m_creators.empty() && !m_functions_wl.empty());  //assert(m_creators.empty() && !m_functions.empty());

            //typename std::vector<func_t>::const_iterator it(m_functions.begin());
            //(*it)(space, offset, data, mem_mask);
            //while (m_functions.end() != ++it)
            //    (*it)(space, offset, data, mem_mask);
            foreach (var func in m_functions_wl)
                func(data);
        }
        public void op(address_space space, int data) { op(space, 0U, data, DefaultMask); }
        public void op(offs_t offset, int data, u32 mem_mask = DefaultMask) { op(default_space(), offset, data, mem_mask); }
        public void op(int data) { op(default_space(), 0U, data, DefaultMask); }
    }


    //**************************************************************************
    //  TEMPLATE INSTANTIATIONS
    //**************************************************************************

#if false
    extern template class devcb_read<u8>;
    extern template class devcb_read<u16>;
    extern template class devcb_read<u32>;
    extern template class devcb_read<u64>;
    extern template class devcb_read<int, 1U>;

    extern template class devcb_read8::delegate_builder<read8_delegate>;
    extern template class devcb_read8::delegate_builder<read16_delegate>;
    extern template class devcb_read8::delegate_builder<read32_delegate>;
    extern template class devcb_read8::delegate_builder<read64_delegate>;
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

    extern template class devcb_read16::delegate_builder<read8_delegate>;
    extern template class devcb_read16::delegate_builder<read16_delegate>;
    extern template class devcb_read16::delegate_builder<read32_delegate>;
    extern template class devcb_read16::delegate_builder<read64_delegate>;
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

    extern template class devcb_read32::delegate_builder<read8_delegate>;
    extern template class devcb_read32::delegate_builder<read16_delegate>;
    extern template class devcb_read32::delegate_builder<read32_delegate>;
    extern template class devcb_read32::delegate_builder<read64_delegate>;
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

    extern template class devcb_read64::delegate_builder<read8_delegate>;
    extern template class devcb_read64::delegate_builder<read16_delegate>;
    extern template class devcb_read64::delegate_builder<read32_delegate>;
    extern template class devcb_read64::delegate_builder<read64_delegate>;
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

    extern template class devcb_read_line::delegate_builder<read8_delegate>;
    extern template class devcb_read_line::delegate_builder<read16_delegate>;
    extern template class devcb_read_line::delegate_builder<read32_delegate>;
    extern template class devcb_read_line::delegate_builder<read64_delegate>;
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

    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read8_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read16_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read32_delegate> >;
    extern template class devcb_read8::creator_impl<devcb_read8::delegate_builder<read64_delegate> >;
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

    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read8_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read16_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read32_delegate> >;
    extern template class devcb_read16::creator_impl<devcb_read16::delegate_builder<read64_delegate> >;
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

    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read8_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read16_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read32_delegate> >;
    extern template class devcb_read32::creator_impl<devcb_read32::delegate_builder<read64_delegate> >;
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

    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read8_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read16_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read32_delegate> >;
    extern template class devcb_read64::creator_impl<devcb_read64::delegate_builder<read64_delegate> >;
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

    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read8_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read16_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read32_delegate> >;
    extern template class devcb_read_line::creator_impl<devcb_read_line::delegate_builder<read64_delegate> >;
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

    extern template class devcb_write8::delegate_builder<write8_delegate>;
    extern template class devcb_write8::delegate_builder<write16_delegate>;
    extern template class devcb_write8::delegate_builder<write32_delegate>;
    extern template class devcb_write8::delegate_builder<write64_delegate>;
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

    extern template class devcb_write16::delegate_builder<write8_delegate>;
    extern template class devcb_write16::delegate_builder<write16_delegate>;
    extern template class devcb_write16::delegate_builder<write32_delegate>;
    extern template class devcb_write16::delegate_builder<write64_delegate>;
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

    extern template class devcb_write32::delegate_builder<write8_delegate>;
    extern template class devcb_write32::delegate_builder<write16_delegate>;
    extern template class devcb_write32::delegate_builder<write32_delegate>;
    extern template class devcb_write32::delegate_builder<write64_delegate>;
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

    extern template class devcb_write64::delegate_builder<write8_delegate>;
    extern template class devcb_write64::delegate_builder<write16_delegate>;
    extern template class devcb_write64::delegate_builder<write32_delegate>;
    extern template class devcb_write64::delegate_builder<write64_delegate>;
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

    extern template class devcb_write_line::delegate_builder<write8_delegate>;
    extern template class devcb_write_line::delegate_builder<write16_delegate>;
    extern template class devcb_write_line::delegate_builder<write32_delegate>;
    extern template class devcb_write_line::delegate_builder<write64_delegate>;
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

    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write8_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write16_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write32_delegate> >;
    extern template class devcb_write8::creator_impl<devcb_write8::delegate_builder<write64_delegate> >;
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

    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write8_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write16_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write32_delegate> >;
    extern template class devcb_write16::creator_impl<devcb_write16::delegate_builder<write64_delegate> >;
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

    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write8_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write16_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write32_delegate> >;
    extern template class devcb_write32::creator_impl<devcb_write32::delegate_builder<write64_delegate> >;
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

    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write8_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write16_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write32_delegate> >;
    extern template class devcb_write64::creator_impl<devcb_write64::delegate_builder<write64_delegate> >;
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

    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write8_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write16_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write32_delegate> >;
    extern template class devcb_write_line::creator_impl<devcb_write_line::delegate_builder<write64_delegate> >;
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
