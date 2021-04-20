// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using log_type = mame.plib.plog_base<mame.netlist.callbacks_t>;//, NL_DEBUG>;
using nlmempool = mame.plib.mempool;


namespace mame.netlist
{
    //class netlist_t;
    //class device_t;
    //class setup_t;

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
            //    static plib::unique_ptr<factory::element_t> NETLIB_NAME(p_alias ## _c) \
            //            (const pstring &classname) \
            //    { \
            //        using devtype = factory::device_element_t<ns :: NETLIB_NAME(chip)>; \
            //        return devtype::create(p_name, classname, p_def_param, __FILE__); \
            //    } \
            //    \
            //    factory::constructor_ptr_t decl_ ## p_alias = NETLIB_NAME(p_alias ## _c);
            static factory.constructor_ptr_t NETLIB_DEVICE_IMPL_BASE<chip>(string ns, string p_alias, string p_name, string p_def_param)
            {
                return (classname) => { return new factory.device_element_t<chip>(p_name, classname, p_def_param, "__FILE__"); };
            }
        }


        class nld_wrapper : device_t  //class NETLIB_NAME(wrapper) : public device_t
        {
            public nld_wrapper(netlist_state_t anetlist, string name)  //NETLIB_NAME(wrapper)(netlist_state_t &anetlist, const pstring &name)
                : base(anetlist, name)
            {
            }


            public override void reset() { }  //NETLIB_RESETI() { }
            public override void update() { }  //NETLIB_UPDATEI() { }
        }


        // -----------------------------------------------------------------------------
        // net_dev class factory
        // -----------------------------------------------------------------------------
        public abstract class element_t
        {
            string m_name;                             ///< device name
            string m_classname;                        ///< device class name
            string m_def_param;                        ///< default parameter
            string m_sourcefile;                       ///< source file


            protected element_t(string name, string classname, string def_param)
            {
                m_name = name;
                m_classname = classname;
                m_def_param = def_param;
                m_sourcefile = "<unknown>";
            }

            protected element_t(string name, string classname, string def_param, string sourcefile)
            {
                m_name = name;
                m_classname = classname;
                m_def_param = def_param;
                m_sourcefile = sourcefile;
            }

            //~element_t() { }

            //PCOPYASSIGNMOVE(element_t, default)


            public abstract device_t make_device(nlmempool pool, netlist_state_t anetlist, string name);  //virtual unique_pool_ptr<device_t> make_device(nlmempool &pool, netlist_state_t &anetlist, const pstring &name) = 0;

            public virtual void macro_actions(nlparse_t nparser, string name)
            {
                //plib::unused_var(nparser);
                //plib::unused_var(name);
            }

            public string name() { return m_name; }
            //const pstring &classname() const { return m_classname; }
            public string param_desc() { return m_def_param; }
            //const pstring &sourcefile() const { return m_sourcefile; }
        }


        //template <class C>
        class device_element_t<C> : element_t
        {
            public device_element_t(string name, string classname, string def_param) : base(name, classname, def_param) { }
            public device_element_t(string name, string classname, string def_param, string sourcefile) : base(name, classname, def_param, sourcefile) { }


            //unique_pool_ptr<device_t> make_device(nlmempool &pool, netlist_state_t &anetlist, const pstring &name) override { return pool.make_unique<C>(anetlist, name); }
            public override device_t make_device(nlmempool pool, netlist_state_t anetlist, string name)
            {
                //return pool.make_unique<C>(anetlist, name);
                Type type = typeof(C);
                if      (type == typeof(nld_sound_in))              return new nld_sound_in(anetlist, name);
                else if (type == typeof(nld_sound_out))             return new nld_sound_out(anetlist, name);
                else if (type == typeof(analog.nld_C))              return new analog.nld_C(anetlist, name);
                else if (type == typeof(analog.nld_opamp))          return new analog.nld_opamp(anetlist, name);
                else if (type == typeof(analog.nld_POT))            return new analog.nld_POT(anetlist, name);
                else if (type == typeof(analog.nld_R))              return new analog.nld_R(anetlist, name);
                else if (type == typeof(devices.nld_analog_input))  return new devices.nld_analog_input(anetlist, name);
                else if (type == typeof(devices.nld_CD4066_GATE))   return new devices.nld_CD4066_GATE(anetlist, name);
                else if (type == typeof(devices.nld_gnd))           return new devices.nld_gnd(anetlist, name);
                else if (type == typeof(devices.nld_logic_input))   return new devices.nld_logic_input(anetlist, name);
                else if (type == typeof(devices.nld_netlistparams)) return new devices.nld_netlistparams(anetlist, name);
                else if (type == typeof(devices.nld_solver))        return new devices.nld_solver(anetlist, name);
                else throw new emu_fatalerror("type {0} not handled yet.  add it to switch statement here", type);
            }


            //static plib::unique_ptr<device_element_t<C>> create(const pstring &name, const pstring &classname, const pstring &def_param, const pstring &sourcefile)
            //{
            //    return plib::make_unique<device_element_t<C>>(name, classname, def_param, sourcefile);
            //}
        }


        public class list_t : std.vector<element_t>  //class list_t : public std::vector<plib::unique_ptr<element_t>>
        {
            log_type m_log;


            public list_t(log_type alog) { m_log = alog; }
            //~list_t() { }
            //PCOPYASSIGNMOVE(list_t, delete)


            //template<class device_class>
            public void register_device<device_class>(string name, string classname, string def_param, string sourcefile)
            {
                register_device(new device_element_t<device_class>(name, classname, def_param, sourcefile));  //register_device(device_element_t<device_class>::create(name, classname, def_param, sourcefile));
            }

            public void register_device(element_t factory)  //void register_device(plib::unique_ptr<element_t> &&factory);
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

            public element_t factory_by_name(string devname)
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
            //bool is_class(element_t *f) { return dynamic_cast<device_element_t<C> *>(f) != nullptr; }
            public bool is_class<C>(element_t f) { return f is device_element_t<C>; }
        }


        // -----------------------------------------------------------------------------
        // factory_creator_ptr_t
        // -----------------------------------------------------------------------------

        //using constructor_ptr_t = plib::unique_ptr<element_t> (*)(const pstring &classname);
        public delegate element_t constructor_ptr_t(string classname);

        //template <typename T>
        //plib::unique_ptr<element_t> constructor_t(const pstring &name, const pstring &classname,
        //        const pstring &def_param)
        //{
        //    return plib::make_unique<device_element_t<T>>(name, classname, def_param);
        //}


        // -----------------------------------------------------------------------------
        // factory_lib_entry_t: factory class to wrap macro based chips/elements
        // -----------------------------------------------------------------------------
        class library_element_t : element_t
        {
            public library_element_t(string name, string classname, string def_param, string source)
                : base(name, classname, def_param, source) {  }


            public override device_t make_device(nlmempool pool, netlist_state_t anetlist, string name)  //unique_pool_ptr<device_t> make_device(nlmempool &pool, netlist_state_t &anetlist, const pstring &name) override;
            {
                return new nld_wrapper(anetlist, name);  //return pool.make_unique<NETLIB_NAME(wrapper)>(anetlist, name);
            }

            public override void macro_actions(nlparse_t nparser, string name)
            {
                nparser.namespace_push(name);
                nparser.include(this.name());
                nparser.namespace_pop();
            }
        }
    }

    namespace devices
    {
        // in net_lib.cs
        //void initialize_factory(factory::list_t &factory);
    }
}
