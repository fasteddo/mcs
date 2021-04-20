// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using log_type = mame.plib.plog_base<mame.netlist.callbacks_t>;  //using log_type =  plib::plog_base<callbacks_t, NL_DEBUG>;


namespace mame.netlist
{
    namespace factory
    {
        public static class nl_factory_global
        {
            //#define NETLIB_DEVICE_IMPL_ALIAS(p_alias, chip, p_name, p_def_param) \
            //    NETLIB_DEVICE_IMPL_BASE(devices, p_alias, chip, p_name, p_def_param) \
            public static factory.constructor_ptr_t NETLIB_DEVICE_IMPL_ALIAS<chip>(string p_alias, string p_name, string p_def_param) { return NETLIB_DEVICE_IMPL_BASE<chip>("devices", p_alias, p_name, p_def_param); }

            //#define NETLIB_DEVICE_IMPL(chip, p_name, p_def_param) \
            //    NETLIB_DEVICE_IMPL_NS(devices, chip, p_name, p_def_param)
            public static factory.constructor_ptr_t NETLIB_DEVICE_IMPL<chip>(string p_name, string p_def_param) { return NETLIB_DEVICE_IMPL_NS<chip>("devices", p_name, p_def_param); }

            //#define NETLIB_DEVICE_IMPL_NS(ns, chip, p_name, p_def_param) \
            //    NETLIB_DEVICE_IMPL_BASE(ns, chip, chip, p_name, p_def_param) \
            public static factory.constructor_ptr_t NETLIB_DEVICE_IMPL_NS<chip>(string ns, string p_name, string p_def_param) { return NETLIB_DEVICE_IMPL_BASE<chip>("devices", "devices", p_name, p_def_param); }

            //#define NETLIB_DEVICE_IMPL_BASE(ns, p_alias, chip, p_name, p_def_param) \
            //    static factory::element_t::uptr NETLIB_NAME(p_alias ## _c) () \
            //    { \
            //        using devtype = factory::device_element_t<ns :: NETLIB_NAME(chip)>; \
            //        factory::properties sl(p_def_param, PSOURCELOC()); \
            //        return devtype::create(p_name, std::move(sl)); \
            //    } \
            //    \
            //    extern factory::constructor_ptr_t decl_ ## p_alias; \
            //    factory::constructor_ptr_t decl_ ## p_alias = NETLIB_NAME(p_alias ## _c);
            static factory.constructor_ptr_t NETLIB_DEVICE_IMPL_BASE<chip>(string ns, string p_alias, string p_name, string p_def_param)
            {
                throw new emu_unimplemented();
#if false
                return (classname) => { return new factory.device_element_t<chip>(p_name, classname, p_def_param, "__FILE__"); };
#endif
            }
        }


        public enum element_type
        {
            BUILTIN,
            MACRO
        }


        public class properties
        {
            string m_defparam;
            plib.source_location m_sourceloc;
            element_type m_type;


            public properties(string defparam, plib.source_location sourceloc)
            {
                m_defparam = defparam;
                m_sourceloc = sourceloc;
                m_type = element_type.BUILTIN;
            }

            //~properties() = default;
            //PCOPYASSIGNMOVE(properties, default)


            public string defparam()
            {
                return m_defparam;
            }


            //plib::source_location source() const noexcept
            //{
            //    return m_sourceloc;
            //}


            public element_type type() { return m_type; }


            public properties set_type(element_type t)
            {
                m_type = t;
                return this;
            }
        }


        // FIXME: this doesn't do anything, check how to remove
        class nld_wrapper : base_device_t  //class NETLIB_NAME(wrapper) : public base_device_t
        {
            public nld_wrapper(netlist_state_t anetlist, string name)  //NETLIB_NAME(wrapper)(netlist_state_t &anetlist, const pstring &name)
                : base(anetlist, name)
            {
            }


            //NETLIB_RESETI() { }
        }


        // -----------------------------------------------------------------------------
        // net_dev class factory
        // -----------------------------------------------------------------------------
        public abstract class element_t
        {
            //using dev_uptr = device_arena::unique_ptr<core_device_t>;
            //using uptr = host_arena::unique_ptr<element_t>;
            //using pointer = element_t *;


            string m_name;                              ///< device name
            properties m_properties;                    ///< source file and other information and settings


            public element_t(string name, properties props)
            {
                m_name = name;
                m_properties = props;
            }


            //~element_t() { }

            //PCOPYASSIGNMOVE(element_t, default)


            public abstract core_device_t make_device(device_arena pool, netlist_state_t anetlist, string name);  //virtual dev_uptr make_device(device_arena &pool, netlist_state_t &anetlist, const pstring &name) = 0;


            public string name() { return m_name; }
            public string param_desc() { return m_properties.defparam(); }
            //plib::source_location source() const noexcept { return m_properties.source(); }
            public element_type type() { return m_properties.type(); }
        }


        //template <class C, typename... Args>
        class device_element_t : element_t
        {
            //std::tuple<Args...> m_args;


            device_element_t(string name, properties props)
                : base(name, props)
            {
                //m_args = std::forward<Args>(args)...)
            }


            //template <std::size_t... Is>
            //dev_uptr make_device(device_arena &pool,
            //                    netlist_state_t &anetlist,
            //                    const pstring &name, std::tuple<Args...>& args, std::index_sequence<Is...>)
            //{
            //    return plib::make_unique<C>(pool, anetlist, name, std::forward<Args>(std::get<Is>(args))...);
            //}

            //dev_uptr make_device(device_arena &pool,
            //            netlist_state_t &anetlist,
            //            const pstring &name, std::tuple<Args...>& args)
            //{
            //    return make_device(pool, anetlist, name, args, std::index_sequence_for<Args...>{});
            //}

            //dev_uptr make_device(device_arena &pool,
            //    netlist_state_t &anetlist,
            //    const pstring &name) override
            //{
            //    return make_device(pool, anetlist, name, m_args);
            //    //return pool.make_unique<C>(anetlist, name);
            //}

            public override core_device_t make_device(device_arena pool, netlist_state_t anetlist, string name)
            {
                throw new emu_unimplemented();
#if false
                return new C(pool, anetlist, name);
#endif
            }


            //static uptr create(const pstring &name, properties &&props, Args&&... args)
            //{
            //    return plib::make_unique<device_element_t<C, Args...>, host_arena>(name,
            //        std::move(props), std::forward<Args>(args)...);
            //}

            public static element_t create_nld_sound_in(string name, properties props)
            {
                return new device_element_t(name, props);
            }

            public static element_t create_nld_analog_callback(string name, properties props)
            {
                return new device_element_t(name, props);
            }
        }


        public class list_t : std.vector<element_t>  //class list_t : public std::vector<element_t::uptr>
        {
            log_type m_log;


            public list_t(log_type alog) { m_log = alog; }
            //~list_t() { }
            //PCOPYASSIGNMOVE(list_t, delete)


            //template<class device_class, typename... Args>
            //void add(const pstring &name, properties &&props, Args&&... args)
            //{
            //    add(device_element_t<device_class, Args...>::create(name, std::move(props),
            //        std::forward<Args>(args)...));
            //}
            public void add_nld_sound_in(string name, properties props)
            {
                add(device_element_t.create_nld_sound_in(name, props));
            }

            public void add_nld_analog_callback(string name, properties props)
            {
                add(device_element_t.create_nld_analog_callback(name, props));
            }


            public void add(element_t factory)  //void add(element_t::uptr &&factory) noexcept(false);
            {
                foreach (var e in this)
                {
                    if (e.name() == factory.name())
                    {
                        m_log.fatal.op(nl_errstr_global.MF_FACTORY_ALREADY_CONTAINS_1(factory.name()));
                        throw new nl_exception(nl_errstr_global.MF_FACTORY_ALREADY_CONTAINS_1(factory.name()));
                    }
                }

                push_back(factory);  //push_back(std::move(factory));
            }


            public element_t factory_by_name(string devname)  //element_t::pointer factory_by_name(const pstring &devname) noexcept(false);
            {
                foreach (var e in this)
                {
                    if (e.name() == devname)
                        return e;
                }

                m_log.fatal.op(nl_errstr_global.MF_CLASS_1_NOT_FOUND(devname));
                throw new nl_exception(nl_errstr_global.MF_CLASS_1_NOT_FOUND(devname));
            }

            //template <class C>
            public bool is_class<C>(element_t f)  //bool is_class(element_t::pointer f) noexcept { return dynamic_cast<device_element_t<C> *>(f) != nullptr; }
            {
                throw new emu_unimplemented();
#if false
                return f is device_element_t<C>;
#endif
            }
        }


        // -----------------------------------------------------------------------------
        // factory_creator_ptr_t
        // -----------------------------------------------------------------------------

        //using constructor_ptr_t = element_t::uptr (*const)();
        public delegate element_t constructor_ptr_t();

        //template <typename T>
        //element_t::uptr constructor_t(const pstring &name, properties &&props)
        //{
        //    return plib::make_unique<device_element_t<T>, host_arena>(name, std::move(props));
        //}


        // -----------------------------------------------------------------------------
        // library_element_t: factory class to wrap macro based chips/elements
        // -----------------------------------------------------------------------------
        class library_element_t : element_t
        {
            public library_element_t(string name, properties props)
                : base(name, ((properties)props).set_type(element_type.MACRO)) { }  //: element_t(name, std::move(properties(props).set_type(element_type::MACRO)))


            public override core_device_t make_device(device_arena pool, netlist_state_t anetlist, string name)  //dev_uptr make_device(device_arena &pool, netlist_state_t &anetlist, const pstring &name) override;
            {
                return new nld_wrapper(anetlist, name);  //return plib::make_unique<NETLIB_NAME(wrapper)>(pool, anetlist, name);
            }
        }
    }

    namespace devices
    {
        // in net_lib.cs
        //void initialize_factory(factory::list_t &factory);
    }
}
