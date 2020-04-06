// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.emu
{
    namespace detail
    {
        // ======================> device_delegate_helper
        // device_delegate_helper does non-template work
        public class device_delegate_helper
        {
            // internal state
            device_t m_base;  //std::reference_wrapper<device_t> m_base;
            string m_tag;


            // construct/assign
            protected device_delegate_helper(device_t owner, string tag = null) { m_base = owner;  m_tag = tag; }
            //template <class DeviceClass, bool Required> device_delegate_helper(device_finder<DeviceClass, Required> const &finder);
            //device_delegate_helper(device_delegate_helper const &) = default;
            //device_delegate_helper &operator=(device_delegate_helper const &) = default;


            // accessors
            //char const *finder_tag() const { return m_tag; }
            //std::pair<device_t &, char const *> finder_target() const { return std::make_pair(m_base, m_tag); }


            // internal helpers
            //delegate_late_bind &bound_object() const;
            //void set_tag(device_t &object) { m_base = object; m_tag = nullptr; }
            //void set_tag(device_t &base, char const *tag) { m_base = base; m_tag = tag; }
            //void set_tag(char const *tag);
            //template <class DeviceClass, bool Required> void set_tag(device_finder<DeviceClass, Required> const &finder);
        }
    }


    // ======================> device_delegate
    // device_delegate is a delegate that wraps with a device tag and can be easily
    // late bound without replicating logic everywhere
    //template<typename Signature>
    //class device_delegate : public named_delegate<Signature>, public device_delegate_helper
    //template <typename ReturnType, typename... Params>
    class device_delegate : detail.device_delegate_helper  //class device_delegate<ReturnType (Params...)> : protected named_delegate<ReturnType (Params...)>, public detail::device_delegate_helper
    {
        //using basetype = named_delegate<ReturnType (Params...)>;

        //template <class T, class U> struct is_related_device_implementation
        //{ static constexpr bool value = std::is_base_of<T, U>::value && std::is_base_of<device_t, U>::value; };
        //template <class T, class U> struct is_related_device_interface
        //{ static constexpr bool value = std::is_base_of<T, U>::value && std::is_base_of<device_interface, U>::value && !std::is_base_of<device_t, U>::value; };
        //template <class T, class U> struct is_related_device
        //{ static constexpr bool value = is_related_device_implementation<T, U>::value || is_related_device_interface<T, U>::value; };

        //template <class T> static std::enable_if_t<is_related_device_implementation<T, T>::value, device_t &> get_device(T &object) { return object; }
        //template <class T> static std::enable_if_t<is_related_device_interface<T, T>::value, device_t &> get_device(T &object) { return object.device(); }

        //template <unsigned Count> using array = device_delegate_array<ReturnType (Params...), Count>;

        //template <typename T> struct supports_callback
        //{ static constexpr bool value = std::is_constructible<device_delegate, device_t &, char const *, T, char const *>::value; };


        // construct/assign
        device_delegate(device_t owner) : base(owner) { }  //explicit device_delegate(device_t &owner) : basetype(), detail::device_delegate_helper(owner) { }
        //device_delegate(device_delegate const &) = default;
        //device_delegate &operator=(device_delegate const &) = default;

        // construct with prototype and target
        //device_delegate(basetype const &proto, device_t &object) : basetype(proto, object), detail::device_delegate_helper(object) { }
        //device_delegate(device_delegate const &proto, device_t &object) : basetype(proto, object), detail::device_delegate_helper(object) { }

        // construct with base and tag
        //template <class D>
        //device_delegate(device_t &base, char const *tag, ReturnType (D::*funcptr)(Params...), char const *name)
        //    : basetype(funcptr, name, static_cast<D *>(nullptr))
        //    , detail::device_delegate_helper(base, tag)
        //{ }
        //template <class D>
        //device_delegate(device_t &base, char const *tag, ReturnType (D::*funcptr)(Params...) const, char const *name)
        //    : basetype(funcptr, name, static_cast<D *>(nullptr))
        //    , detail::device_delegate_helper(base, tag)
        //{ }
        //template <class D>
        //device_delegate(device_t &base, char const *tag, ReturnType (*funcptr)(D &, Params...), char const *name)
        //    : basetype(funcptr, name, static_cast<D *>(nullptr))
        //    , detail::device_delegate_helper(base, tag)
        //{ }

        // construct with device finder
        //template <class D, bool R, class E>
        //device_delegate(device_finder<D, R> const &finder, ReturnType (E::*funcptr)(Params...), char const *name)
        //    : basetype(funcptr, name, static_cast<E *>(nullptr))
        //    , detail::device_delegate_helper(finder)
        //{ }
        //template <class D, bool R, class E>
        //device_delegate(device_finder<D, R> const &finder, ReturnType (E::*funcptr)(Params...) const, char const *name)
        //    : basetype(funcptr, name, static_cast<E *>(nullptr))
        //    , detail::device_delegate_helper(finder)
        //{ }
        //template <class D, bool R, class E>
        //device_delegate(device_finder<D, R> const &finder, ReturnType (*funcptr)(E &, Params...), char const *name)
        //    : basetype(funcptr, name, static_cast<E *>(nullptr))
        //    , detail::device_delegate_helper(finder)
        //{ }

        // construct with target object
        //template <class T, class D>
        //device_delegate(T &object, ReturnType (D::*funcptr)(Params...), std::enable_if_t<is_related_device<D, T>::value, char const *> name)
        //    : basetype(funcptr, name, static_cast<D *>(&object))
        //    , detail::device_delegate_helper(get_device(object))
        //{ }
        //template <class T, class D>
        //device_delegate(T &object, ReturnType (D::*funcptr)(Params...) const, std::enable_if_t<is_related_device<D, T>::value, char const *> name)
        //    : basetype(funcptr, name, static_cast<D *>(&object))
        //    , detail::device_delegate_helper(get_device(object))
        //{ }
        //template <class T, class D>
        //device_delegate(T &object, ReturnType (*funcptr)(D &, Params...), std::enable_if_t<is_related_device<D, T>::value, char const *> name)
        //    : basetype(funcptr, name, static_cast<D *>(&object))
        //    , detail::device_delegate_helper(get_device(object))
        //{ }

        // construct with callable object
        //template <typename T>
        //device_delegate(device_t &owner, T &&funcptr, std::enable_if_t<std::is_constructible<std::function<ReturnType (Params...)>, T>::value, char const *> name)
        //    : basetype(std::forward<T>(funcptr), name)
        //    , detail::device_delegate_helper(owner)
        //{ basetype::operator=(basetype(std::forward<T>(funcptr), name)); }

        // setters that implicitly bind to the current device
        //template <class D> void set(ReturnType (D::*funcptr)(Params...), char const *name)
        //{ basetype::operator=(basetype(funcptr, name, static_cast<D *>(nullptr))); set_tag(nullptr); }
        //template <class D> void set(ReturnType (D::*funcptr)(Params...) const, char const *name)
        //{ basetype::operator=(basetype(funcptr, name, static_cast<D *>(nullptr))); set_tag(nullptr); }
        //template <class D> void set(ReturnType (*funcptr)(D &, Params...), char const *name)
        //{ basetype::operator=(basetype(funcptr, name, static_cast<D *>(nullptr))); set_tag(nullptr); }

        // setters that take a tag-like object specifying the target
        //template <typename T, class D> void set(T &&tag, ReturnType (D::*funcptr)(Params...), char const *name)
        //{ basetype::operator=(basetype(funcptr, name, static_cast<D *>(nullptr))); set_tag(std::forward<T>(tag)); }
        //template <typename T, class D> void set(T &&tag, ReturnType (D::*funcptr)(Params...) const, char const *name)
        //{ basetype::operator=(basetype(funcptr, name, static_cast<D *>(nullptr))); set_tag(std::forward<T>(tag)); }
        //template <typename T, class D> void set(T &&tag, ReturnType (*funcptr)(D &, Params...), char const *name)
        //{ basetype::operator=(basetype(funcptr, name, static_cast<D *>(nullptr))); set_tag(std::forward<T>(tag)); }

        // setters that take a target object
        //template <class T, class D> std::enable_if_t<std::is_base_of<D, T>::value> set(T &object, ReturnType (D::*funcptr)(Params...), char const *name)
        //{ basetype::operator=(basetype(funcptr, name, static_cast<D *>(&object))); set_tag(finder_target().first); }
        //template <class T, class D> std::enable_if_t<std::is_base_of<D, T>::value> set(T &object, ReturnType (D::*funcptr)(Params...) const, char const *name)
        //{ basetype::operator=(basetype(funcptr, name, static_cast<D *>(&object))); set_tag(finder_target().first); }
        //template <class T, class D> std::enable_if_t<std::is_base_of<D, T>::value> set(T &object, ReturnType (*funcptr)(D &, Params...), char const *name)
        //{ basetype::operator=(basetype(funcptr, name, static_cast<D *>(&object))); set_tag(finder_target().first); }

        // setter that takes a functoid
        //template <typename T> std::enable_if_t<std::is_constructible<std::function<ReturnType (Params...)>, T>::value> set(T &&funcptr, char const *name)
        //{ basetype::operator=(basetype(std::forward<T>(funcptr), name)); }

        // unsetter
        //void set(std::nullptr_t)
        //{ basetype::operator=(basetype()); set_tag(finder_target().first); }

        // perform the binding
        //void resolve() { if (!basetype::isnull() && !basetype::has_object()) basetype::late_bind(bound_object()); }

        // accessors
        //using basetype::operator();
        //using basetype::isnull;
        //using basetype::has_object;
        //using basetype::name;
    }
}
