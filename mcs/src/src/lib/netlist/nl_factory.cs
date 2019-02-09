// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    //class netlist_t;
    //class device_t;
    //class setup_t;

    namespace factory
    {
        public static class nl_factory_global
        {
            // deprecated!
            //#define NETLIB_DEVICE_IMPL_DEPRECATED(chip) \
            //    static std::unique_ptr<factory::element_t> NETLIB_NAME(chip ## _c)( \
            //            const pstring &name, const pstring &classname, const pstring &def_param) \
            //    { \
            //        return std::unique_ptr<factory::element_t>(plib::palloc<factory::device_element_t<NETLIB_NAME(chip)>>(name, classname, def_param, pstring(__FILE__))); \
            //    } \
            //    factory::constructor_ptr_t decl_ ## chip = NETLIB_NAME(chip ## _c);

            // the new way ...
            //#define NETLIB_DEVICE_IMPL(chip, p_name, p_def_param) \
            //    static std::unique_ptr<factory::element_t> NETLIB_NAME(chip ## _c)( \
            //            const pstring &name, const pstring &classname, const pstring &def_param) \
            //    { \
            //        return std::unique_ptr<factory::element_t>(plib::palloc<factory::device_element_t<NETLIB_NAME(chip)>>(p_name, classname, p_def_param, pstring(__FILE__))); \
            //    } \
            //    factory::constructor_ptr_t decl_ ## chip = NETLIB_NAME(chip ## _c);

            //#define NETLIB_DEVICE_IMPL_NS(ns, chip) \
            //    static std::unique_ptr<factory::element_t> NETLIB_NAME(chip ## _c)( \
            //            const pstring &name, const pstring &classname, const pstring &def_param) \
            //    { \
            //        return std::unique_ptr<factory::element_t>(plib::palloc<factory::device_element_t<ns :: NETLIB_NAME(chip)>>(name, classname, def_param, pstring(__FILE__))); \
            //    } \
            //    factory::constructor_ptr_t decl_ ## chip = NETLIB_NAME(chip ## _c);
        }


        // -----------------------------------------------------------------------------
        // net_dev class factory
        // -----------------------------------------------------------------------------
        public abstract class element_t //: plib::nocopyassignmove
        {
            string m_name;                             /* device name */
            string m_classname;                        /* device class name */
            string m_def_param;                        /* default parameter */
            string m_sourcefile;                       /* source file */


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


            public element_t get() { return this; }


            public abstract device_t Create(netlist_state_t anetlist, string name);  //virtual plib::owned_ptr<device_t> Create(netlist_state_t &anetlist, const pstring &name) = 0;
            public virtual void macro_actions(netlist_state_t anetlist, string name) {}

            public string name() { return m_name; }
            //const pstring &classname() const { return m_classname; }
            //const pstring &param_desc() const { return m_def_param; }
            //const pstring &sourcefile() const { return m_sourcefile; }
        }


        //template <class C>
        class device_element_t<C> : element_t
        {
            public device_element_t(string name, string classname, string def_param) : base(name, classname, def_param) { }
            public device_element_t(string name, string classname, string def_param, string sourcefile) : base(name, classname, def_param, sourcefile) { }


            //plib::owned_ptr<device_t> Create(netlist_t &anetlist, const pstring &name) override { return plib::owned_ptr<device_t>::Create<C>(anetlist, name); }
            public override device_t Create(netlist_state_t anetlist, string name)
            {
                Type type = typeof(C);
                if      (type == typeof(nld_sound_in))              return new nld_sound_in(anetlist, name);
                else if (type == typeof(nld_sound_out))             return new nld_sound_out(anetlist, name);
                else if (type == typeof(analog.nld_C))              return new analog.nld_C(anetlist, name);
                else if (type == typeof(analog.nld_POT))            return new analog.nld_POT(anetlist, name);
                else if (type == typeof(analog.nld_R))              return new analog.nld_R(anetlist, name);
                else if (type == typeof(devices.nld_analog_input))  return new devices.nld_analog_input(anetlist, name);
                else if (type == typeof(devices.nld_gnd))           return new devices.nld_gnd(anetlist, name);
                else if (type == typeof(devices.nld_logic_input))   return new devices.nld_logic_input(anetlist, name);
                else if (type == typeof(devices.nld_netlistparams)) return new devices.nld_netlistparams(anetlist, name);
                else if (type == typeof(devices.nld_solver))        return new devices.nld_solver(anetlist, name);
                else throw new emu_fatalerror("type {0} not handled yet.  add it to switch statement here", type);
            }
        }


        public class list_t : std.vector<element_t>  //public std::vector<std::unique_ptr<element_t>>
        {
            setup_t m_setup;


            public list_t(setup_t setup) { m_setup = setup; }
            //~list_t() { clear(); }


            //template<class device_class>
            public void register_device<device_class>(string name, string classname, string def_param)
            {
                register_device(new device_element_t<device_class>(name, classname, def_param));  //register_device(std::unique_ptr<element_t>(plib::palloc<device_element_t<device_class>>(name, classname, def_param)));
            }

            public void register_device(element_t factory)  //void register_device(std::unique_ptr<element_t> &&factory);
            {
                foreach (var e in this)
                {
                    if (e.name() == factory.name())
                        m_setup.log().fatal.op(nl_errstr_global.MF_1_FACTORY_ALREADY_CONTAINS_1, factory.name());
                }

                push_back(factory);  //push_back(std::move(factory));
            }

            public element_t factory_by_name(string devname)
            {
                foreach (var e in this)
                {
                    if (e.name() == devname)
                        return e.get();
                }

                m_setup.log().fatal.op(nl_errstr_global.MF_1_CLASS_1_NOT_FOUND, devname);
                return null; // appease code analysis
            }

            //template <class C>
            //bool is_class(element_t *f) { return dynamic_cast<device_element_t<C> *>(f) != nullptr; }
            public bool is_class<C>(element_t f) { return f is device_element_t<C>; }
        }


        // -----------------------------------------------------------------------------
        // factory_creator_ptr_t
        // -----------------------------------------------------------------------------

        //using constructor_ptr_t = std::unique_ptr<element_t> (*)(const pstring &name, const pstring &classname,
        //        const pstring &def_param);
        public delegate element_t constructor_ptr_t(string name, string classname, string def_param);

        //template <typename T>
        //std::unique_ptr<element_t> constructor_t(const pstring &name, const pstring &classname,
        //        const pstring &def_param)
        //{
        //    return std::unique_ptr<element_t>(plib::palloc<device_element_t<T>>(name, classname, def_param));
        //}


        // -----------------------------------------------------------------------------
        // factory_lib_entry_t: factory class to wrap macro based chips/elements
        // -----------------------------------------------------------------------------
        class library_element_t : element_t
        {
            public library_element_t(setup_t setup, string name, string classname, string def_param, string source)
                : base(name, classname, def_param, source) {  }


            public override device_t Create(netlist_state_t anetlist, string name)
            {
                throw new emu_unimplemented();
#if false
                return plib::owned_ptr<device_t>::Create<NETLIB_NAME(wrapper)>(anetlist, name);
#endif
            }

            public override void macro_actions(netlist_state_t anetlist, string name)
            {
                throw new emu_unimplemented();
#if false
                anetlist.setup().namespace_push(name);
                anetlist.setup().include(this.name());
                anetlist.setup().namespace_pop();
#endif
            }
        }
    }

    namespace devices
    {
        // in net_lib.cs
        //void initialize_factory(factory::list_t &factory);
    }
}
