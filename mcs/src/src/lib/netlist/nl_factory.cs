// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using log_type = mame.plib.plog_base<mame.netlist.nl_config_global.bool_const_NL_DEBUG>;  //using log_type =  plib::plog_base<NL_DEBUG>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;

using static mame.cpp_global;
using static mame.netlist.nl_errstr_global;


namespace mame
{
    public static class nl_factory_global
    {
        //#define NETLIB_DEVICE_IMPL_ALIAS(p_alias, chip, p_name, p_def_param) \
        //    NETLIB_DEVICE_IMPL_BASE(devices, p_alias, chip, p_name, p_def_param) \
        public static netlist.factory.constructor_ptr_t NETLIB_DEVICE_IMPL_ALIAS<chip>(string p_alias, string p_name, string p_def_param) { return NETLIB_DEVICE_IMPL_BASE<chip>("devices", p_alias, p_name, p_def_param); }

        //#define NETLIB_DEVICE_IMPL(chip, p_name, p_def_param) \
        //    NETLIB_DEVICE_IMPL_NS(devices, chip, p_name, p_def_param)
        public static netlist.factory.constructor_ptr_t NETLIB_DEVICE_IMPL<chip>(string p_name, string p_def_param) { return NETLIB_DEVICE_IMPL_NS<chip>("devices", p_name, p_def_param); }

        //#define NETLIB_DEVICE_IMPL_NS(ns, chip, p_name, p_def_param) \
        //    NETLIB_DEVICE_IMPL_BASE(ns, chip, chip, p_name, p_def_param) \
        public static netlist.factory.constructor_ptr_t NETLIB_DEVICE_IMPL_NS<chip>(string ns, string p_name, string p_def_param) { return NETLIB_DEVICE_IMPL_BASE<chip>("devices", "devices", p_name, p_def_param); }

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
        static netlist.factory.constructor_ptr_t NETLIB_DEVICE_IMPL_BASE<chip>(string ns, string p_alias, string p_name, string p_def_param)
        {
            return () => 
            {
                netlist.factory.properties sl = new netlist.factory.properties(p_def_param, plib.pg.PSOURCELOC());
                return new netlist.factory.device_element_t<chip>(p_name, sl);
            };
        }
    }


    namespace netlist.factory
    {
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
        class device_element_t<C> : element_t
        {
            object [] m_args;  //std::tuple<Args...> m_args;


            public device_element_t(string name, properties props, params object [] args)  //device_element_t(const pstring &name, properties &&props, Args&&... args)
                : base(name, props)
            {
                m_args = args;  //m_args = std::forward<Args>(args)...)
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

            public core_device_t make_device(device_arena pool, netlist_state_t anetlist, string name, object [] args)
            {
                //return plib::make_unique<C>(pool, anetlist, name, std::forward<Args>(std::get<Is>(args))...);
                ////return anetlist.make_pool_object<C>(anetlist, name, std::forward<Args>(std::get<Is>(args))...);

                if      (typeof(C) == typeof(analog.nld_C))                   return new analog.nld_C(anetlist, name);
                else if (typeof(C) == typeof(analog.nld_D))                   return new analog.nld_D(anetlist, name);
                else if (typeof(C) == typeof(analog.nld_opamp))               return new analog.nld_opamp(anetlist, name);
                else if (typeof(C) == typeof(analog.nld_POT))                 return new analog.nld_POT(anetlist, name);
                else if (typeof(C) == typeof(analog.nld_QBJT_switch))         return new analog.nld_QBJT_switch(anetlist, name);
                else if (typeof(C) == typeof(analog.nld_R))                   return new analog.nld_R(anetlist, name);
                else if (typeof(C) == typeof(analog.nld_switch2))             return new analog.nld_switch2(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_74107))              return new devices.nld_74107(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_74153))              return new devices.nld_74153(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_7448))               return new devices.nld_7448(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_7450))               return new devices.nld_7450(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_7474))               return new devices.nld_7474(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_7483))               return new devices.nld_7483(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_7490))               return new devices.nld_7490(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_7493))               return new devices.nld_7493(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_9316))               return new devices.nld_9316(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_analog_input))       return new devices.nld_analog_input(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_CD4066_GATE))        return new devices.nld_CD4066_GATE(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_gnd))                return new devices.nld_gnd(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_logic_input))        return new devices.nld_logic_input(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_mainclock))          return new devices.nld_mainclock(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_NE555))              return new devices.nld_NE555(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_netlistparams))      return new devices.nld_netlistparams(anetlist, name);
                else if (typeof(C) == typeof(devices.nld_solver))             return new devices.nld_solver(anetlist, name);
                else if (typeof(C) == typeof(interface_.nld_analog_callback)) { assert(args.Length == 2);  return new interface_.nld_analog_callback(anetlist, name, (nl_fptype)args[0], (interface_.nld_analog_callback.FUNC)args[1]); }
                else if (typeof(C) == typeof(interface_.nld_logic_callback))  { assert(args.Length == 1);  return new interface_.nld_logic_callback(anetlist, name, (interface_.nld_logic_callback.FUNC)args[0]); }
                else if (typeof(C) == typeof(nld_sound_in))                   return new nld_sound_in(anetlist, name);
                else throw new emu_unimplemented();
            }


            public override core_device_t make_device(device_arena pool, netlist_state_t anetlist, string name)
            {
                return make_device(pool, anetlist, name, m_args);
            }


            //static uptr create(const pstring &name, properties &&props, Args&&... args)
            //{
            //    return plib::make_unique<device_element_t<C, Args...>, host_arena>(name,
            //        std::move(props), std::forward<Args>(args)...);
            //}

            public static element_t create_nld_sound_in(string name, properties props)
            {
                return new device_element_t<nld_sound_in>(name, props);
            }

            public static element_t create_nld_analog_callback(string name, properties props, params object [] args)
            {
                return new device_element_t<interface_.nld_analog_callback>(name, props, args);
            }

            public static element_t create_nld_logic_callback(string name, properties props, params object [] args)
            {
                return new device_element_t<interface_.nld_logic_callback>(name, props, args);
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
                add(device_element_t<nld_sound_in>.create_nld_sound_in(name, props));
            }

            public void add_nld_analog_callback<cb_t, fptype, lb_t>(string name, properties props, fptype fp, lb_t lb)
            {
                add(device_element_t<interface_.nld_analog_callback>.create_nld_analog_callback(name, props, fp, lb));
            }

            public void add_nld_logic_callback(string name, properties props, netlist.interface_.nld_logic_callback.FUNC lb)
            {
                add(device_element_t<interface_.nld_logic_callback>.create_nld_logic_callback(name, props, lb));
            }


            public void add(element_t factory)  //void add(element_t::uptr &&factory) noexcept(false);
            {
                if (exists(factory.name()))
                {
                    m_log.fatal.op(MF_FACTORY_ALREADY_CONTAINS_1(factory.name()));
                    throw new nl_exception(MF_FACTORY_ALREADY_CONTAINS_1(factory.name()));
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

                m_log.fatal.op(MF_CLASS_1_NOT_FOUND(devname));
                throw new nl_exception(MF_CLASS_1_NOT_FOUND(devname));
            }


            //template <class C>
            public bool is_class<C>(element_t f) { return f is device_element_t<C>; }  //bool is_class(element_t::pointer f) noexcept { return dynamic_cast<device_element_t<C> *>(f) != nullptr; }


            bool exists(string name)
            {
                foreach (var e in this)
                {
                    if (e.name() == name)
                        return true;
                }

                return false;
            }
        }


        // -----------------------------------------------------------------------------
        // factory_creator_ptr_t
        // -----------------------------------------------------------------------------

        public delegate element_t constructor_ptr_t();  //using constructor_ptr_t = element_t::uptr (*const)();

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


    namespace netlist.devices
    {
        // in net_lib.cs
        //void initialize_factory(factory::list_t &factory);
    }
}
