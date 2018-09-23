// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using attoseconds_t = System.Int64;
using device_timer_id = System.UInt32;
using device_type = mame.emu.detail.device_type_impl_base;
using seconds_t = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    // timer IDs for devices
    //typedef UInt32 device_timer_id;

    //typedef emu::detail::device_type_impl_base const &device_type;


    namespace emu.detail
    {
        public struct device_feature
        {
            public enum type  // : u32
            {
                PROTECTION  = 1 <<  0,  //u32(1) <<  0,
                PALETTE     = 1 <<  1,  //u32(1) <<  1,
                GRAPHICS    = 1 <<  2,  //u32(1) <<  2,
                SOUND       = 1 <<  3,  //u32(1) <<  3,
                CONTROLS    = 1 <<  4,  //u32(1) <<  4,
                KEYBOARD    = 1 <<  5,  //u32(1) <<  5,
                MOUSE       = 1 <<  6,  //u32(1) <<  6,
                MICROPHONE  = 1 <<  7,  //u32(1) <<  7,
                CAMERA      = 1 <<  8,  //u32(1) <<  8,
                DISK        = 1 <<  9,  //u32(1) <<  9,
                PRINTER     = 1 << 10,  //u32(1) << 10,
                LAN         = 1 << 11,  //u32(1) << 11,
                WAN         = 1 << 12,  //u32(1) << 12,
                TIMING      = 1 << 13,  //u32(1) << 13,

                NONE        = 0,  //u32(0),
                ALL         = (1 << 14) - 1  //(u32(1) << 14) - 1U
            }
        }

        //DECLARE_ENUM_BITWISE_OPERATORS(device_feature::type);


        public class device_registrar : IEnumerable<device_type>
        {
            //class const_iterator_helper;
            //friend class device_type_impl_base;

            class const_iterator
            {
                //friend class const_iterator_helper;

                //typedef std::ptrdiff_t difference_type;
                //typedef device_type_impl_base value_type;
                //typedef device_type_impl_base *pointer;
                //typedef device_type_impl_base &reference;
                //typedef std::forward_iterator_tag iterator_category;

                //pointer m_type = nullptr;

                //const_iterator() = default;
                //const_iterator(const_iterator const &) = default;
                //const_iterator &operator=(const_iterator const &) = default;

                //bool operator==(const_iterator const &that) const { return m_type == that.m_type; }
                //bool operator!=(const_iterator const &that) const { return m_type != that.m_type; }
                //reference operator*() const { assert(m_type); return *m_type; }
                //pointer operator->() const { return m_type; }
                //const_iterator &operator++();
                //const_iterator operator++(int) { const_iterator const result(*this); ++*this; return result; }
            }


            class const_iterator_helper : const_iterator
            {
                //const_iterator_helper(device_type_impl_base *type) { m_type = type; }
            }


            List<device_type> m_devices = new List<device_type>();


            // explicit constructor is required for const variable initialization
            //constexpr device_registrar() { }

            //const_iterator begin() const { return cbegin(); }
            //const_iterator end() const { return cend(); }
            //const_iterator cbegin() const;
            //const_iterator cend() const;

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
            public IEnumerator<device_type> GetEnumerator() { return m_devices.GetEnumerator(); }


            static device_type_impl_base register_device(device_type_impl_base type) { throw new emu_unimplemented(); }
        }


        //template <class DeviceClass, char const *ShortName, char const *FullName, char const *Source> struct device_tag_struct { typedef DeviceClass type; };
        public class device_tag_struct
        {
            //typedef DeviceClass type; 
            public device_type_impl.create_func m_creator;
            public string ShortName;
            public string FullName;
            public string Source;
            public device_feature.type DeviceClass_unemulated_features;
            public device_feature.type DeviceClass_imperfect_features;

            public device_tag_struct(
                device_type_impl.create_func creator,
                string ShortName,
                string FullName,
                string Source,
                device_feature.type DeviceClass_unemulated_features,
                device_feature.type DeviceClass_imperfect_features)
            { this.m_creator = creator; this.ShortName = ShortName; this.FullName = FullName; this.Source = Source; this.DeviceClass_unemulated_features = DeviceClass_unemulated_features; this.DeviceClass_imperfect_features = DeviceClass_imperfect_features; }
        }

        //template <class DriverClass, char const *ShortName, char const *FullName, char const *Source, device_feature::type Unemulated, device_feature::type Imperfect> struct driver_tag_struct { typedef DriverClass type; };
        public class driver_tag_struct
        {
            //typedef DriverClass type; 
            public device_type_impl.create_func m_creator;
            public string ShortName;
            public string FullName;
            public string Source;
            public device_feature.type Unemulated;
            public device_feature.type Imperfect;
            public device_feature.type DriverClass_unemulated_features;
            public device_feature.type DriverClass_imperfect_features;

            public driver_tag_struct(
                device_type_impl.create_func creator,
                string ShortName,
                string FullName,
                string Source,
                device_feature.type Unemulated,
                device_feature.type Imperfect,
                device_feature.type DriverClass_unemulated_features,
                device_feature.type DriverClass_imperfect_features)
            { this.m_creator = creator; this.ShortName = ShortName; this.FullName = FullName; this.Source = Source; this.Unemulated = Unemulated; this.Imperfect = Imperfect; this.DriverClass_unemulated_features = DriverClass_unemulated_features; this.DriverClass_imperfect_features = DriverClass_imperfect_features; }
        }

        //template <class DeviceClass, char const *ShortName, char const *FullName, char const *Source> auto device_tag_func() { return device_tag_struct<DeviceClass, ShortName, FullName, Source>{ }; };
        //template <class DriverClass, char const *ShortName, char const *FullName, char const *Source, device_feature::type Unemulated, device_feature::type Imperfect> auto driver_tag_func() { return driver_tag_struct<DriverClass, ShortName, FullName, Source>{ }; };


        public class device_type_impl_base
        {
            //friend class device_registrar;

            //typedef std::unique_ptr<device_t> (*create_func)(device_type_impl_base const &type, machine_config const &mconfig, char const *tag, device_t *owner, u32 clock);
            public delegate device_t create_func(device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock);


            create_func m_creator;
            //std::type_info const &m_type;
            string m_shortname;
            string m_fullname;
            string m_source;
            device_feature.type m_unemulated_features;
            device_feature.type m_imperfect_features;

            device_type_impl_base m_next;


            //using exposed_type = device_t;


            public device_type_impl_base()
            {
                m_creator = null;
#if false
                m_type = typeid(std::nullptr_t);
#endif
                m_shortname = null;
                m_fullname = null;
                m_source = null;
                m_unemulated_features = device_feature.type.NONE;
                m_imperfect_features = device_feature.type.NONE;
                m_next = null;
            }


            //template <class DeviceClass, char const *ShortName, char const *FullName, char const *Source>
            //device_type_impl_base(device_tag_struct<DeviceClass, ShortName, FullName, Source> (*)())
            public device_type_impl_base(device_tag_struct device_tag)
            {
                m_creator = device_tag.m_creator;  // &create_device<DeviceClass>;

                //throw new emu_unimplemented();
#if false
                m_type = typeid(DeviceClass);
#endif
                m_shortname = device_tag.ShortName;
                m_fullname = device_tag.FullName;
                m_source = device_tag.Source;
                m_unemulated_features = device_tag.DeviceClass_unemulated_features;  //DeviceClass::unemulated_features();
                m_imperfect_features = device_tag.DeviceClass_imperfect_features;  //DeviceClass::imperfect_features();

                //throw new emu_unimplemented();
#if false
                m_next = device_registrar.register_device(this);
#endif
            }


            //template <class DriverClass, char const *ShortName, char const *FullName, char const *Source, device_feature::type Unemulated, device_feature::type Imperfect>
            //device_type_impl_base(driver_tag_struct<DriverClass, ShortName, FullName, Source, Unemulated, Imperfect> (*)())
            public device_type_impl_base(driver_tag_struct driver_tag)
            {
                m_creator = driver_tag.m_creator;  // &create_driver<DriverClass>)

                //throw new emu_unimplemented();
#if false
                m_type(typeid(DriverClass))
#endif
                m_shortname = driver_tag.ShortName;
                m_fullname = driver_tag.FullName;
                m_source = driver_tag.Source;
                m_unemulated_features = driver_tag.DriverClass_unemulated_features | driver_tag.Unemulated;  //DriverClass::unemulated_features() | Unemulated;
                m_imperfect_features = (driver_tag.DriverClass_imperfect_features & ~driver_tag.Unemulated) | driver_tag.Imperfect;  //(DriverClass::imperfect_features() & ~Unemulated) | Imperfect;
                m_next = null;
            }


            //device_type_impl_base(device_type_impl_base const &) = delete;
            //device_type_impl_base(device_type_impl_base &&) = delete;
            //device_type_impl_base &operator=(device_type_impl_base const &) = delete;
            //device_type_impl_base &operator=(device_type_impl_base &&) = delete;


            //template <typename DeviceClass>
            //static std::unique_ptr<device_t> create_device(device_type_impl_base const &type, machine_config const &mconfig, char const *tag, device_t *owner, u32 clock)
            //{
            //    return make_unique_clear<DeviceClass>(mconfig, tag, owner, clock);
            //}


            //template <typename DriverClass>
            //static std::unique_ptr<device_t> create_driver(device_type_impl_base const &type, machine_config const &mconfig, char const *tag, device_t *owner, u32 clock)
            //{
            //    assert(!owner);
            //    assert(!clock);

            //    return make_unique_clear<DriverClass>(mconfig, type, tag);
            //}


            //std::type_info const &type() const { return m_type; }
            public string shortname() { return m_shortname; }
            public string fullname() { return m_fullname; }
            public string source() { return m_source; }
            public device_feature.type unemulated_features() { return m_unemulated_features; }
            public device_feature.type imperfect_features() { return m_imperfect_features; }

            //std::unique_ptr<device_t> create(machine_config const &mconfig, char const *tag, device_t *owner, u32 clock) const
            //{
            //    return m_creator(*this, mconfig, tag, owner, clock);
            //}
            public device_t create(machine_config mconfig, string tag, device_t owner, UInt32 clock) { return m_creator(this, mconfig, tag, owner, clock); }

            //explicit operator bool() const { return bool(m_creator); }
            //bool operator==(device_type_impl_base const &that) const { return &that == this; }
            //bool operator!=(device_type_impl_base const &that) const { return &that != this; }
        }


        //template <class DeviceClass>
        public class device_type_impl : device_type_impl_base
        {
            //using exposed_type = DeviceClass;

            //using device_type_impl_base::device_type_impl_base;
            //using device_type_impl_base::create;

            //template <typename... Params>
            //std::unique_ptr<DeviceClass> create(machine_config &mconfig, char const *tag, device_t *owner, Params &&... args) const
            //{
            //    return make_unique_clear<DeviceClass>(mconfig, tag, owner, std::forward<Params>(args)...);
            //}

            //template <typename... Params> DeviceClass &operator()(machine_config &mconfig, char const *tag, Params &&... args) const;
            //template <typename Exposed, bool Required, typename... Params> DeviceClass &operator()(machine_config &mconfig, device_finder<Exposed, Required> &finder, Params &&... args) const;
            //template <typename... Params> DeviceClass &operator()(machine_config_replace replace, char const *tag, Params &&... args) const;
            //template <typename Exposed, bool Required, typename... Params> DeviceClass &operator()(machine_config_replace replace, device_finder<Exposed, Required> &finder, Params &&... args) const;


            //template <class DeviceClass> template <typename... Params>
            //inline DeviceClass &device_type_impl<DeviceClass>::operator()(machine_config &mconfig, char const *tag, Params &&... args) const
            //{
            //    return dynamic_cast<DeviceClass &>(*mconfig.device_add(tag, *this, std::forward<Params>(args)...));
            //}

            //template <class DeviceClass> template <typename Exposed, bool Required, typename... Params>
            //inline DeviceClass &device_type_impl<DeviceClass>::operator()(machine_config &mconfig, device_finder<Exposed, Required> &finder, Params &&... args) const
            //{
            //    std::pair<device_t &, char const *> const target(finder.finder_target());
            //    assert(&mconfig.current_device() == &target.first);
            //    DeviceClass &result(dynamic_cast<DeviceClass &>(*mconfig.device_add(target.second, *this, std::forward<Params>(args)...)));
            //    return finder = result;
            //}

            //template <class DeviceClass> template <typename... Params>
            //inline DeviceClass &device_type_impl<DeviceClass>::operator()(machine_config_replace replace, char const *tag, Params &&... args) const
            //{
            //    return dynamic_cast<DeviceClass &>(*replace.config.device_replace(tag, *this, std::forward<Params>(args)...));
            //}

            //template <class DeviceClass> template <typename Exposed, bool Required, typename... Params>
            //inline DeviceClass &device_type_impl<DeviceClass>::operator()(machine_config_replace replace, device_finder<Exposed, Required> &finder, Params &&... args) const
            //{
            //    std::pair<device_t &, char const *> const target(finder.finder_target());
            //    assert(&replace.config.current_device() == &target.first);
            //    DeviceClass &result(dynamic_cast<DeviceClass &>(*replace.config.device_replace(target.second, *this, std::forward<Params>(args)...)));
            //    return finder = result;
            //}
        }
    }


    // device types
    //typedef emu::detail::device_type_impl_base const &device_type;
    //typedef std::add_pointer_t<device_type> device_type_ptr;
    //extern emu::detail::device_registrar const registered_device_types;

    //template <
    //        typename DeviceClass,
    //        char const *ShortName,
    //        char const *FullName,
    //        char const *Source>
    //constexpr auto device_creator = &emu::detail::device_tag_func<DeviceClass, ShortName, FullName, Source>;
    public class device_creator
    {
        device_type.create_func m_creator;
        string ShortName;
        string FullName;
        string Source;
        emu.detail.device_feature.type DeviceClass_unemulated_features;
        emu.detail.device_feature.type DeviceClass_imperfect_features;

        public device_creator(
            device_type.create_func device_creator,
            string ShortName,
            string FullName,
            string Source,
            emu.detail.device_feature.type DeviceClass_unemulated_features,
            emu.detail.device_feature.type DeviceClass_imperfect_features)
        { m_creator = device_creator; this.ShortName = ShortName; this.FullName = FullName; this.Source = Source; this.DeviceClass_unemulated_features = DeviceClass_unemulated_features; this.DeviceClass_imperfect_features = DeviceClass_imperfect_features; }

        public emu.detail.device_tag_struct device_tag() { return new emu.detail.device_tag_struct(m_creator, ShortName, FullName, Source, DeviceClass_unemulated_features, DeviceClass_imperfect_features); }
    }

    //template <
    //        typename DriverClass,
    //        char const *ShortName,
    //        char const *FullName,
    //        char const *Source,
    //        emu::detail::device_feature::type Unemulated,
    //        emu::detail::device_feature::type Imperfect>
    //constexpr auto driver_device_creator = &emu::detail::driver_tag_func<DriverClass, ShortName, FullName, Source, Unemulated, Imperfect>;
    public class driver_device_creator
    {
        device_type.create_func m_creator;
        string ShortName;
        string FullName;
        string Source;
        emu.detail.device_feature.type Unemulated;
        emu.detail.device_feature.type Imperfect;
        emu.detail.device_feature.type DriverClass_unemulated_features;
        emu.detail.device_feature.type DriverClass_imperfect_features;

        public driver_device_creator(
            device_type.create_func driver_device_creator,
            string ShortName,
            string FullName,
            string Source,
            emu.detail.device_feature.type Unemulated,
            emu.detail.device_feature.type Imperfect,
            emu.detail.device_feature.type DriverClass_unemulated_features,
            emu.detail.device_feature.type DriverClass_imperfect_features)
        { m_creator = driver_device_creator; this.ShortName = ShortName; this.FullName = FullName; this.Source = Source; this.Unemulated = Unemulated; this.Imperfect = Imperfect; this.DriverClass_unemulated_features = DriverClass_unemulated_features; this.DriverClass_imperfect_features = DriverClass_imperfect_features; }

        public emu.detail.driver_tag_struct driver_tag() { return new emu.detail.driver_tag_struct(m_creator, ShortName, FullName, Source, Unemulated, Imperfect, DriverClass_unemulated_features, DriverClass_imperfect_features); }
    }


    // exception classes
    class device_missing_dependencies : emu_exception { }


    public static class device_global
    {
        // use this to refer to the owning device when providing a device tag
        public const string DEVICE_SELF = "";

        // use this to refer to the owning device's owner when providing a device tag
        public const string DEVICE_SELF_OWNER = "^";


        // macro for specifying a clock derived from an owning device
        public static u32 DERIVED_CLOCK(u32 num, u32 den) { return 0xff000000 | (num << 12) | (den << 0); }


        //**************************************************************************
        //  DEVICE CONFIGURATION MACROS
        //**************************************************************************

        // configure devices
        //#define MCFG_DEVICE_CLOCK(_clock)    device->set_clock(_clock);
        //#define MCFG_DEVICE_INPUT_DEFAULTS(_config)    device->set_input_default(DEVICE_INPUT_DEFAULTS_NAME(_config));
        //#define MCFG_DEVICE_BIOS(...)     device->set_default_bios_tag(__VA_ARGS__);

        //#define DECLARE_READ_LINE_MEMBER(name)      int  name()
        //#define READ_LINE_MEMBER(name)              int  name()
        //#define DECLARE_WRITE_LINE_MEMBER(name)     void name(ATTR_UNUSED int state)
        //#define WRITE_LINE_MEMBER(name)             void name(ATTR_UNUSED int state)


        public static emu.detail.device_registrar registered_device_types = new emu.detail.device_registrar();


        //#define DECLARE_DEVICE_TYPE(Type, Class) \
        //        class Class; \
        //        extern emu::detail::device_type_impl<Class> const &Type; \
        //        extern template class device_finder<Class, false>; \
        //        extern template class device_finder<Class, true>;

        //#define DECLARE_DEVICE_TYPE_NS(Type, Namespace, Class) \
        //        extern emu::detail::device_type_impl<Namespace::Class> const &Type; \
        //        extern template class device_finder<Namespace::Class, false>; \
        //        extern template class device_finder<Namespace::Class, true>;

        //#define DEFINE_DEVICE_TYPE(Type, Class, ShortName, FullName) \
        //        namespace { \
        //            struct Class##_device_traits { static constexpr char const shortname[] = ShortName, fullname[] = FullName, source[] = __FILE__; }; \
        //            constexpr char const Class##_device_traits::shortname[], Class##_device_traits::fullname[], Class##_device_traits::source[]; \
        //        } \
        //        emu::detail::device_type_impl<Class> const &Type = device_creator<Class, (Class##_device_traits::shortname), (Class##_device_traits::fullname), (Class##_device_traits::source)>; \
        //        template class device_finder<Class, false>; \
        //        template class device_finder<Class, true>;
        public static device_type DEFINE_DEVICE_TYPE(device_type.create_func func, string shortname, string fullname)
        {
            var traits = new gamedrv_global.game_traits(shortname, fullname);

            return new device_type(new device_creator(func, traits.shortname, traits.fullname, traits.source, device_t.unemulated_features(), device_t.imperfect_features()).device_tag());
        }

        //#define DEFINE_DEVICE_TYPE_PRIVATE(Type, Base, Class, ShortName, FullName) \
        //        namespace { \
        //            struct Class##_device_traits { static constexpr char const shortname[] = ShortName, fullname[] = FullName, source[] = __FILE__; }; \
        //            constexpr char const Class##_device_traits::shortname[], Class##_device_traits::fullname[], Class##_device_traits::source[]; \
        //        } \
        //        emu::detail::device_type_impl<Base> const &Type = device_creator<Class, (Class##_device_traits::shortname), (Class##_device_traits::fullname), (Class##_device_traits::source)>;

        //#define DEFINE_DEVICE_TYPE_NS(Type, Namespace, Class, ShortName, FullName) \
        //        namespace { \
        //            struct Class##_device_traits { static constexpr char const shortname[] = ShortName, fullname[] = FullName, source[] = __FILE__; }; \
        //            constexpr char const Class##_device_traits::shortname[], Class##_device_traits::fullname[], Class##_device_traits::source[]; \
        //        } \
        //        emu::detail::device_type_impl<Namespace::Class> const &Type = device_creator<Namespace::Class, (Class##_device_traits::shortname), (Class##_device_traits::fullname), (Class##_device_traits::source)>; \
        //        template class device_finder<Namespace::Class, false>; \
        //        template class device_finder<Namespace::Class, true>;
    }


    public class device_t : device_init_helpers,
                            simple_list_item<device_t> //: public delegate_late_bind
    {
        //friend class simple_list<device_t>;
        //friend class running_machine;
        //friend class finder_base;
        //friend class devcb_base;


        public class subdevice_list : IEnumerable<device_t>
        {
            //friend class device_t;
            //friend class machine_config;


            // private state
            public simple_list<device_t> m_list = new simple_list<device_t>();         // list of sub-devices we own
            public std_unordered_map<string, device_t> m_tagmap = new std_unordered_map<string, device_t>();   // map of devices looked up and found by subtag


            // construction/destruction
            public subdevice_list() { }


            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
            public IEnumerator<device_t> GetEnumerator()
            {
                var iter = m_list.begin();

                while (iter.current() != null)
                {
                    yield return iter.current();
                    iter.advance();
                }
            }


            simple_list<device_t>.auto_iterator begin() { return m_list.begin(); }
            simple_list<device_t>.auto_iterator end() { return m_list.end(); }


            // getters
            public device_t first() { return m_list.first(); }
            //int count() const { return m_list.count(); }
            //bool empty() const { return m_list.empty(); }


            // range iterators
            //using auto_iterator = simple_list<device_t>::auto_iterator;
            //auto_iterator begin() const { return m_list.begin(); }
            //auto_iterator end() const { return m_list.end(); }


            // private helpers
            public device_t find(string name)
            {
                device_t curdevice;
                for (curdevice = m_list.first(); curdevice != null; curdevice = curdevice.next())
                {
                    if (name == curdevice.m_basetag)
                        return curdevice;
                }

                return null;
            }
        }


        public class interface_list : IEnumerable<device_interface>
        {
            //friend class device_t;
            //friend class device_interface;
            //friend class device_memory_interface;
            //friend class device_state_interface;
            //friend class device_execute_interface;


            public device_interface m_head;         // head of interface list
            public device_execute_interface m_execute;    // pre-cached pointer to execute interface
            public device_memory_interface m_memory;      // pre-cached pointer to memory interface
            public device_state_interface m_state;        // pre-cached pointer to state interface


            class auto_iterator
            {
                // private state
                device_interface m_current;

                // construction/destruction
                public auto_iterator(device_interface intf) { m_current = intf; }

                public device_interface current() { return m_current; }
                public void advance() { m_current = m_current.interface_next(); }

                // required operator overrides
                //bool operator!=(const auto_iterator &iter) const { return m_current != iter.m_current; }
                //device_interface &operator*() const { return *m_current; }
                //const auto_iterator &operator++();
            }


            // construction/destruction
            public interface_list()
            {
                m_head = null;
                m_execute = null;
                m_memory = null;
                m_state = null;
            }


            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
            public IEnumerator<device_interface> GetEnumerator()
            {
                auto_iterator iter = new auto_iterator(m_head);

                while (iter.current() != null)
                {
                    yield return iter.current();
                    iter.advance();
                }
            }


            // getters
            public device_interface first() { return m_head; }

            // range iterators
            auto_iterator begin() { return new auto_iterator(m_head); }
            auto_iterator end() { return new auto_iterator(null); }
        }


        // keep list of interfaces, to account for lack of multiple inheritance
        public List<device_interface> m_class_interfaces = new List<device_interface>();

        public bool IsA<T>() where T : device_interface
        {
            return GetClassInterface<T>() != null;
        }

        public T GetClassInterface<T>() where T : device_interface
        {
            foreach (var i in m_class_interfaces)
            {
                if (i is T)
                    return (T)i;
            }

            return null;
        }


        //using feature = emu::detail::device_feature;
        //using feature_type = emu::detail::device_feature::type;


        // core device properties
        device_type m_type;                 // device type
        protected string m_searchpath;           // search path, used for media loading

        // device relationships & interfaces
        device_t m_owner;                // device that owns us
        device_t m_next;                 // next device by the same owner (of any type/class)
        subdevice_list m_subdevices = new subdevice_list();           // container for list of subdevices
        interface_list m_interfaces = new interface_list();           // container for list of interfaces

        // device clocks
        u32 m_configured_clock;     // originally configured device clock
        u32 m_unscaled_clock;       // current unscaled device clock
        u32 m_clock;                // current device clock, after scaling
        double m_clock_scale;          // clock scale factor
        attoseconds_t m_attoseconds_per_clock;// period in attoseconds

        device_debug m_debug;  //std::unique_ptr<device_debug> m_debug;
        machine_config m_machine_config;       // reference to the machine's configuration
        input_device_default [] m_input_defaults;   // devices input ports default overrides

        u8 m_system_bios;          // the system BIOS we wish to load
        u8 m_default_bios;         // the default system BIOS
        string m_default_bios_tag = "";     // tag of the default system BIOS

        // private state; accessor use required
        running_machine m_machine;
        save_manager m_save;
        string m_tag;                  // full tag for this instance
        string m_basetag;              // base part of the tag
        bool m_config_complete;      // have we completed our configuration?
        bool m_started;              // true if the start function has succeeded
        finder_base m_auto_finder_list;     // list of objects to auto-find
        std_vector<rom_entry> m_rom_entries = new std_vector<rom_entry>();
        std_list<devcb_base> m_callbacks = new std_list<devcb_base>();

        // string formatting buffer for logerror
        string m_string_buffer;  //mutable util::ovectorstream m_string_buffer;


        // construction/destruction

        //-------------------------------------------------
        //  device_t - constructor for a new
        //  running device; initial state is derived
        //  from the provided config
        //-------------------------------------------------
        public device_t(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock)
        {
            m_type = type;
            m_searchpath = type.shortname();
            m_owner = owner;
            m_next = null;

            m_configured_clock = clock;
            m_unscaled_clock = clock;
            m_clock = clock;
            m_clock_scale = 1.0f;
            m_attoseconds_per_clock = (clock == 0) ? 0 : attotime.HZ_TO_ATTOSECONDS(clock);

            m_machine_config = mconfig;
            m_input_defaults = null;
            m_default_bios_tag = "";

            m_machine = null;
            m_save = null;
            m_basetag = tag;
            m_config_complete = false;
            m_started = false;
            m_auto_finder_list = null;


            if (owner != null)
                m_tag = string.Format("{0}:{1}", ((owner.owner() == null) ? "" : owner.tag()), tag);
            else
                m_tag = ":";

            set_clock(clock);
        }


        // device flags
        public static emu.detail.device_feature.type unemulated_features() { return emu.detail.device_feature.type.NONE; }
        public static emu.detail.device_feature.type imperfect_features() { return emu.detail.device_feature.type.NONE; }


        // getters
        public device_t get() { return this; }  // for c++ pointer wrappers
        public device_t release() { return this; }  // for c++ pointer wrappers

        //bool has_running_machine() const { return m_machine != nullptr; }
        public running_machine machine() { /*assert(m_machine != NULL);*/ return m_machine; }
        public string tag() { return m_tag; }
        public string basetag() { return m_basetag; }
        public device_type type() { return m_type; }
        public string name() { return m_type.fullname(); }
        public string shortname() { return m_type.shortname(); }
        public string searchpath() { return m_searchpath; }
        string source() { return m_type.source(); }
        public device_t owner() { return m_owner; }

        public device_t next() { return m_next; }
        public device_t m_next_get() { return m_next; }
        public void m_next_set(device_t value) { m_next = value; }

        public u32 configured_clock() { return m_configured_clock; }
        public machine_config mconfig() { return m_machine_config; }
        public input_device_default [] input_ports_defaults() { return m_input_defaults; }

        //-------------------------------------------------
        // rom_region_vector
        //-------------------------------------------------
        public ListBase<rom_entry> rom_region_vector()
        {
            if (m_rom_entries.empty())
            {
                m_rom_entries = romload_global.rom_build_entries(device_rom_region());
            }
            return m_rom_entries;
        }

        public List<tiny_rom_entry> rom_region() { return device_rom_region(); }
        public ioport_constructor input_ports() { return device_input_ports(); }
        //string get_default_bios_tag() { return m_default_bios_tag; }
        public u8 default_bios() { global.assert(configured());  return m_default_bios; }
        public u8 system_bios() { return m_system_bios; }


        // interface helpers
        public interface_list interfaces() { return m_interfaces; }
        //template<class DeviceClass> bool interface(DeviceClass *&intf) { intf = dynamic_cast<DeviceClass *>(this); return (intf != NULL); }

        // specialized helpers for common core interfaces
        //bool interface(device_execute_interface *&intf) { intf = m_interfaces.m_execute; return (intf != nullptr); }
        //bool interface(device_execute_interface *&intf) const { intf = m_interfaces.m_execute; return (intf != nullptr); }
        //bool interface(device_memory_interface *&intf) { intf = m_interfaces.m_memory; return (intf != nullptr); }
        //bool interface(device_memory_interface *&intf) const { intf = m_interfaces.m_memory; return (intf != nullptr); }
        //bool interface(device_state_interface *&intf) { intf = m_interfaces.m_state; return (intf != nullptr); }
        //bool interface(device_state_interface *&intf) const { intf = m_interfaces.m_state; return (intf != nullptr); }

        public bool interface_<T>(out T intf) where T : device_interface { intf = GetClassInterface<T>(); return intf != null; }

        public device_execute_interface execute() { global.assert(m_interfaces.m_execute != null);  return m_interfaces.m_execute; }
        public device_memory_interface memory() { global.assert(m_interfaces.m_memory != null);  return m_interfaces.m_memory; }
        public device_state_interface state() { global.assert(m_interfaces.m_state != null); return m_interfaces.m_state; }


        public void execute_set(device_execute_interface execute) { m_interfaces.m_execute = execute; }
        public void memory_set(device_memory_interface memory) { m_interfaces.m_memory = memory; }
        public void state_set(device_state_interface state) { m_interfaces.m_state = state; }


        // owned object helpers
        public subdevice_list subdevices() { return m_subdevices; }


        // device-relative tag lookups

        //-------------------------------------------------
        //  subtag - create a fully resolved path relative
        //  to our device based on the provided tag
        //-------------------------------------------------
        public string subtag(string tag)
        {
            string result;

            // if the tag begins with a colon, ignore our path and start from the root
            if (tag.StartsWith(":"))
            {
                tag = tag.Substring(1);
                result = ":";
            }
            else
            {
                // otherwise, start with our path

                result = m_tag;
                if (result != ":")
                    result += ":";
            }

            // iterate over the tag, look for special path characters to resolve
            int caret;
            while ((caret = tag.IndexOf('^')) != -1)
            {
                // copy everything up to there
                result += tag.Substring(0, caret);
                tag = tag.Substring(caret + 1);

                // strip trailing colons
                int len = result.Length;
                while (result[--len] == ':')
                    result = result.Substring(0, len);

                // remove the last path part, leaving the last colon
                if (result != ":")
                {
                    int lastcolon = result.LastIndexOf(':', 0);
                    if (lastcolon != -1)
                        result = result.Substring(0, lastcolon + 1);
                }
            }

            // copy everything else
            result += tag;

            {
                // strip trailing colons up to the root
                int len = result.Length;
                while (len > 1 && result[--len] == ':')
                    result = result.Substring(0, len);
            }

            return result;
        }

        public string siblingtag(string tag) { return m_owner != null ? m_owner.subtag(tag) : string.Copy(tag); }

        //-------------------------------------------------
        //  memregion - return a pointer to the region
        //  info for a given region
        //-------------------------------------------------
        public memory_region memregion(string _tag)
        {
            // build a fully-qualified name and look it up
            if (_tag != null)
                return machine().memory().regions().find(subtag(_tag).c_str());
            else
                return null;
        }

        //-------------------------------------------------
        //  memshare - return a pointer to the memory share
        //  info for a given share
        //-------------------------------------------------
        public memory_share memshare(string _tag)
        {
            // build a fully-qualified name and look it up
            if (!string.IsNullOrEmpty(_tag))
                return machine().memory().shares().find(subtag(_tag).c_str());
            else
                return null;
        }

        //-------------------------------------------------
        //  membank - return a pointer to the memory
        //  bank info for a given bank
        //-------------------------------------------------
        public memory_bank membank(string _tag)
        {
            if (!string.IsNullOrEmpty(_tag))
                return machine().memory().banks().find(subtag(_tag).c_str());
            else
                return null;
        }

        //-------------------------------------------------
        //  ioport - return a pointer to the I/O port
        //  object for a given port name
        //-------------------------------------------------
        public ioport_port ioport(string tag)
        {
            // build a fully-qualified name and look it up
            return machine().ioport().port(subtag(tag));
        }

        //-------------------------------------------------
        //  subdevice - given a tag, find the device by
        //  name relative to this device
        //-------------------------------------------------
        public device_t subdevice(string tag)
        {
            // empty string or NULL means this device
            if (string.IsNullOrEmpty(tag))
                return this;

            // do a quick lookup and return that if possible
            var quick = m_subdevices.m_tagmap.find(tag);
            return quick != null ? quick : subdevice_slow(tag);
        }

        //-------------------------------------------------
        //  siblingdevice - given a tag, find the device
        //  by name relative to this device's parent
        //-------------------------------------------------
        public device_t siblingdevice(string tag)
        {
            // safety first
            if (this == null)
                return null;

            // empty string or NULL means this device
            if (string.IsNullOrEmpty(tag))
                return this;

            // leading caret implies the owner, just skip it
            if (tag[0] == '^')
                tag = tag.Remove(0);

            // query relative to the parent, if we have one
            if (m_owner != null)
                return m_owner.subdevice(tag);

            // otherwise, it's NULL unless the tag is absolute
            return (tag[0] == ':') ? subdevice(tag) : null;
        }


        //_DeviceClass subdevice<_DeviceClass>(string tag) where _DeviceClass : device_t { return (_DeviceClass)subdevice(tag); }
        public DeviceClass siblingdevice<DeviceClass>(string tag) where DeviceClass : device_t { return (DeviceClass)siblingdevice(tag); }

        //string parameter(const char *tag) const;


        // configuration helpers

        //-------------------------------------------------
        //  add_machine_configuration - add device-
        //  specific machine configuration
        //-------------------------------------------------
        public void add_machine_configuration(machine_config config)
        {
            global.assert(config == m_machine_config);
            using (machine_config.token tok = config.begin_configuration(this))  // machine_config::token const tok(config.begin_configuration(*this));
            {
                device_add_mconfig(config, m_owner, this);
                for (finder_base autodev = m_auto_finder_list; autodev != null; autodev = autodev.next())
                    autodev.end_configuration();
            }
        }


        //-------------------------------------------------
        //  set_clock - set/change the clock on
        //  a device
        //-------------------------------------------------
        void set_clock(u32 clock)
        {
            m_configured_clock = clock;

            // derive the clock from our owner if requested
            if ((clock & 0xff000000) == 0xff000000)
                calculate_derived_clock();
            else
                set_unscaled_clock(clock);
        }


        void set_clock(XTAL xtal) { set_clock(xtal.value()); }


        public void set_input_default(input_device_default [] config) { m_input_defaults = config; }
        //template <typename... Params> void set_default_bios_tag(Params &&... args) { assert(!configured()); m_default_bios_tag.assign(std::forward<Params>(args)...); }
        public void set_default_bios_tag(string tag) { m_default_bios_tag = tag; }


        // state helpers

        //-------------------------------------------------
        //  config_complete - called when the
        //  configuration of a device is complete
        //-------------------------------------------------
        public void config_complete()
        {
            // resolve default BIOS
            List<tiny_rom_entry> roms = rom_region();
            if (roms != null)
            {
                // first pass: try to find default BIOS from ROM region or machine configuration
                string defbios = m_default_bios_tag.empty() ? null : m_default_bios_tag.c_str();
                bool twopass = false;
                bool havebios = false;
                u8 firstbios = 0;
                {
                    int romIdx = 0;
                    for (tiny_rom_entry rom = roms[romIdx]; m_default_bios == 0 && !romload_global.ROMENTRY_ISEND(rom); rom = roms[++romIdx])  //  for (tiny_rom_entry rom = roms; !m_default_bios && !ROMENTRY_ISEND(rom); ++rom)
                    {
                        if (romload_global.ROMENTRY_ISSYSTEM_BIOS(rom))
                        {
                            if (!havebios)
                            {
                                havebios = true;
                                firstbios = (u8)romload_global.ROM_GETBIOSFLAGS(rom);
                            }

                            if (string.IsNullOrEmpty(defbios))
                                twopass = true;
                            else if (global.strcmp(rom.name, defbios) == 0)
                                m_default_bios = (u8)romload_global.ROM_GETBIOSFLAGS(rom);
                        }
                        else if (string.IsNullOrEmpty(defbios) && romload_global.ROMENTRY_ISDEFAULT_BIOS(rom))
                        {
                            defbios = rom.name;
                        }
                    }
                }

                // second pass is needed if default BIOS came after one or more system BIOSes
                if (havebios && m_default_bios == 0)
                {
                    if (!string.IsNullOrEmpty(defbios) && twopass)
                    {
                        int romIdx = 0;
                        for (tiny_rom_entry rom = roms[romIdx]; m_default_bios == 0 && !romload_global.ROMENTRY_ISEND(rom); rom = roms[++romIdx])
                        {
                            if (romload_global.ROMENTRY_ISSYSTEM_BIOS(rom) && global.strcmp(rom.name, defbios) == 0)
                                m_default_bios = (u8)romload_global.ROM_GETBIOSFLAGS(rom);
                        }
                    }

                    // no default BIOS declared but at least one system BIOS declared
                    if (m_default_bios == 0)
                        m_default_bios = firstbios;
                }

                // set system BIOS to the default unless something overrides it
                set_system_bios(m_default_bios);
            }

            // notify the interfaces
            foreach (device_interface intf in interfaces())
                intf.interface_config_complete();

            // then notify the device itself
            device_config_complete();

            // then mark ourselves complete
            m_config_complete = true;
        }

        public bool configured() { return m_config_complete; }
        //void validity_check(validity_checker &valid) const;
        public bool started() { return m_started; }

        //-------------------------------------------------
        //  reset - reset a device
        //-------------------------------------------------
        public void reset()
        {
            // let the interfaces do their pre-work
            foreach (device_interface intf in interfaces())
                intf.interface_pre_reset();

            // reset the device
            device_reset();

            // reset all child devices
            foreach (device_t child in subdevices())
                child.reset();

            // now allow for some post-child reset action
            device_reset_after_children();

            // let the interfaces do their post-work
            foreach (device_interface intf in interfaces())
                intf.interface_post_reset();
        }


        // clock/timing accessors
        public u32 clock() { return m_clock; }
        public void clock_set(u32 value) { m_clock = value; }
        //UINT32 unscaled_clock() const { return m_unscaled_clock; }


        //-------------------------------------------------
        //  set_unscaled_clock - sets the given device's
        //  unscaled clock
        //-------------------------------------------------
        void set_unscaled_clock(u32 clock)
        {
            // do nothing if no actual change
            if (clock == m_unscaled_clock)
                return;

            m_unscaled_clock = clock;
            m_clock = (UInt32)(m_unscaled_clock * m_clock_scale);
            m_attoseconds_per_clock = (m_clock == 0) ? 0 : attotime.HZ_TO_ATTOSECONDS(m_clock);

            // recalculate all derived clocks
            foreach (device_t child in subdevices())
                child.calculate_derived_clock();

            // if the device has already started, make sure it knows about the new clock
            if (m_started)
                notify_clock_changed();
        }


        void set_unscaled_clock(XTAL xtal) { set_unscaled_clock(xtal.value()); }


        //double clock_scale() const { return m_clock_scale; }
        //void set_clock_scale(double clockscale);

        //-------------------------------------------------
        //  clocks_to_attotime - converts a number of
        //  clock ticks to an attotime
        //-------------------------------------------------
        public attotime clocks_to_attotime(u64 numclocks)
        {
            if (m_clock == 0)
            {
                return attotime.never;
            }
            else if (numclocks < m_clock)
            {
                return new attotime(0, (attoseconds_t)numclocks * m_attoseconds_per_clock);
            }
            else
            {
                UInt32 remainder;
                UInt32 quotient = eminline_global.divu_64x32_rem(numclocks, m_clock, out remainder);
                return new attotime((seconds_t)quotient, (attoseconds_t)((UInt64)remainder * (UInt64)m_attoseconds_per_clock));
            }
        }

        //UINT64 attotime_to_clocks(const attotime &duration) const;


        // timer interfaces

        //-------------------------------------------------
        //  timer_alloc - allocate a timer for our device
        //  callback
        //-------------------------------------------------

        public emu_timer timer_alloc(device_timer_id id = 0, object ptr = null) { return machine().scheduler().timer_alloc(this, id, ptr); }

        //-------------------------------------------------
        //  timer_set - set a temporary timer that will
        //  call our device callback
        //-------------------------------------------------
        protected void timer_set(attotime duration, device_timer_id id = 0, int param = 0, object ptr = null)
        {
            machine().scheduler().timer_set(duration, this, id, param, ptr);
        }

        protected void synchronize(device_timer_id id = 0, int param = 0, object ptr = null) { timer_set(attotime.zero, id, param, ptr); }
        public void timer_expired(emu_timer timer, device_timer_id id, int param/*, void *ptr*/) { device_timer(timer, id, param/*, ptr*/); }


        // state saving interfaces
        //template<typename _ItemType>
        public void save_item<ItemType>(ItemType value, string valname, int index = 0) { global.assert(m_save != null);  m_save.save_item(this, name(), tag(), index, value, valname); }
        //template<typename ItemType> void save_pointer<ItemType>(ItemType value, string valname, UInt32 count, int index = 0) { /*assert(m_save != NULL);*/ m_save.save_pointer(this, name(), tag(), index, value, valname, count); }


        // debugging
        public device_debug debug() { return m_debug; }


        public void set_system_bios(u8 bios) { m_system_bios = bios; }

        //-------------------------------------------------
        //  findit - search for all objects in auto finder
        //  list and return status
        //-------------------------------------------------
        bool findit(bool isvalidation)
        {
            bool allfound = true;
            for (finder_base autodev = m_auto_finder_list; autodev != null; autodev = autodev.next())
            {
                if (isvalidation)
                {
                    // sanity checking
                    string tag = autodev.finder_tag();
                    if (tag == null)
                    {
                        global.osd_printf_error("Finder tag is null!\n");
                        allfound = false;
                        continue;
                    }
                    if (tag[0] == '^' && tag[1] == ':')
                    {
                        global.osd_printf_error("Malformed finder tag: {0}\n", tag);
                        allfound = false;
                        continue;
                    }
                }

                allfound &= autodev.findit(isvalidation);
            }

            return allfound;
        }


        // misc

        protected void popmessage(string format, params object [] args)
        {
            if (m_machine != null)
                m_machine.popmessage(format, args);
        }


        public void logerror(string format, params object [] args)
        {
            if (m_machine != null)
                m_machine.logerror(string.Format("[{0}] {1}", tag(), format), args);

            if (m_machine != null && m_machine.allow_logging())
            {
                profiler_global.g_profiler.start(profile_type.PROFILER_LOGERROR);

                // dump to the buffer
                //m_string_buffer.clear();
                //m_string_buffer.seekp(0);
                //util::stream_format(m_string_buffer, "[%s] ", tag());
                //util::stream_format(m_string_buffer, std::forward<Format>(fmt), std::forward<Params>(args)...);
                //m_string_buffer.put('\0');

                string msg = string.Format("[{0}] {1}", tag(), string.Format(format, args));
                m_machine.strlog(msg);  //m_machine->strlog(&m_string_buffer.vec()[0]);

                profiler_global.g_profiler.stop();
            }
        }


        // miscellaneous helpers

        //-------------------------------------------------
        //  set_machine - notify that the machine now
        //  exists
        //-------------------------------------------------
        public void set_machine(running_machine machine)
        {
            m_machine = machine;
            m_save = machine.save();
        }


        //-------------------------------------------------
        //  resolve_pre_map - find objects that may be used
        //  in memory maps
        //-------------------------------------------------
        public void resolve_pre_map()
        {
            // prepare the logerror buffer
            if (m_machine.allow_logging())
                m_string_buffer.reserve(1024);
        }

        //-------------------------------------------------
        //  resolve_post_map - find objects that are created
        //  in memory maps
        //-------------------------------------------------
        public void resolve_post_map()
        {
            // find all the registered post-map objects
            if (!findit(false))
                throw new emu_fatalerror("Missing some required objects, unable to proceed");

            // allow implementation to do additional setup
            device_resolve_objects();
        }


        //-------------------------------------------------
        //  start - start a device
        //-------------------------------------------------
        public void start()
        {
            // prepare the logerror buffer
            if (m_machine.allow_logging())
                m_string_buffer.reserve(1024);

            // let the interfaces do their pre-work
            foreach (device_interface intf in interfaces())
                intf.interface_pre_start();

            // remember the number of state registrations
            int state_registrations = machine().save().registration_count();

            // start the device
            device_start();

            // complain if nothing was registered by the device
            state_registrations = machine().save().registration_count() - state_registrations;
            device_execute_interface exec;
            device_sound_interface sound;
            if (state_registrations == 0 && (interface_(out exec) || interface_(out sound)) && type() != speaker_device.SPEAKER)
            {
                //throw new emu_unimplemented();
#if false
                logerror("Device did not register any state to save!\n");
                if (((UInt64)machine().system().flags & gamedrv_global.MACHINE_SUPPORTS_SAVE) != 0)
                    global.fatalerror("Device '{0}' did not register any state to save!\n", tag());
#endif
            }

            // let the interfaces do their post-work
            foreach (device_interface intf in interfaces())
                intf.interface_post_start();

            // force an update of the clock
            notify_clock_changed();

            // if we're debugging, create a device_debug object
            if ((machine().debug_flags_get & running_machine.DEBUG_FLAG_ENABLED) != 0)
            {
                m_debug = new device_debug(this);
                debug_setup();
            }

            // register our save states
            save_item(m_clock, "m_clock");
            save_item(m_unscaled_clock, "m_unscaled_clock");
            save_item(m_clock_scale, "m_clock_scale");

            // we're now officially started
            m_started = true;
        }

        //-------------------------------------------------
        //  stop - stop a device
        //-------------------------------------------------
        public void stop()
        {
            // let the interfaces do their pre-work
            foreach (device_interface intf in interfaces())
                intf.interface_pre_stop();

            // stop the device
            device_stop();

            // let the interfaces do their post-work
            foreach (device_interface intf in interfaces())
                intf.interface_post_stop();

            // free any debugging info
            m_debug = null;

            // we're now officially stopped, and the machine is off-limits
            m_started = false;
            m_machine = null;
        }

        //-------------------------------------------------
        //  debug_setup - set up for debugging
        //-------------------------------------------------
        void debug_setup()
        {
            // notify the interface
            foreach (device_interface intf in interfaces())
                intf.interface_debug_setup();

            // notify the device
            device_debug_setup();
        }

        //-------------------------------------------------
        //  pre_save - tell the device and its interfaces
        //  that we are about to save
        //-------------------------------------------------
        public void pre_save()
        {
            // notify the interface
            foreach (device_interface intf in interfaces())
                intf.interface_pre_save();

            // notify the device
            device_pre_save();
        }


        //-------------------------------------------------
        //  post_load - tell the device and its interfaces
        //  that we just completed a load
        //-------------------------------------------------
        public void post_load()
        {
            // notify the interface
            foreach (device_interface intf in interfaces())
                intf.interface_post_load();

            // notify the device
            device_post_load();
        }

        //-------------------------------------------------
        //  notify_clock_changed - notify all interfaces
        //  that the clock has changed
        //-------------------------------------------------
        void notify_clock_changed()
        {
            // first notify interfaces
            foreach (device_interface intf in interfaces())
                intf.interface_clock_changed();

            // then notify the device
            device_clock_changed();
        }

        //-------------------------------------------------
        //  register_auto_finder - add a new item to the
        //  list of stuff to find after we go live
        //-------------------------------------------------
        public finder_base register_auto_finder(finder_base autodev)
        {
            // add to this list
            finder_base old = m_auto_finder_list;
            m_auto_finder_list = autodev;
            return old;
        }


        public void register_callback(devcb_base callback)
        {
            m_callbacks.emplace_back(callback);
        }


        //------------------- begin derived class overrides

        // device-level overrides

        //-------------------------------------------------
        //  rom_region - return a pointer to the implicit
        //  rom region description for this device
        //-------------------------------------------------
        protected virtual List<tiny_rom_entry> device_rom_region() { return null; }

        //-------------------------------------------------
        //  device_add_mconfig - add device-specific
        //  machine configuration
        //-------------------------------------------------
        protected virtual void device_add_mconfig(machine_config config, device_t owner, device_t device) { }

        //-------------------------------------------------
        //  input_ports - return a pointer to the implicit
        //  input ports description for this device
        //-------------------------------------------------
        protected virtual ioport_constructor device_input_ports() { return null; }

        //-------------------------------------------------
        //  device_config_complete - perform any
        //  operations now that the configuration is
        //  complete
        //-------------------------------------------------
        protected virtual void device_config_complete() { }

        //-------------------------------------------------
        //  device_validity_check - validate a device after
        //  the configuration has been constructed
        //-------------------------------------------------
        protected virtual void device_validity_check(validity_checker valid) { }

        //-------------------------------------------------
        //  device_resolve_objects - resolve objects that
        //  may be needed for other devices to set
        //  initial conditions at start time
        //-------------------------------------------------
        protected virtual void device_resolve_objects() { }

        protected virtual void device_start() { }

        //-------------------------------------------------
        //  device_stop - clean up anything that needs to
        //  happen before the running_machine goes away
        //-------------------------------------------------
        protected virtual void device_stop() { }

        //-------------------------------------------------
        //  device_reset - actually handle resetting of
        //  a device; designed to be overriden by the
        //  actual device implementation
        //-------------------------------------------------
        protected virtual void device_reset() { }

        //-------------------------------------------------
        //  device_reset_after_children - hook to do
        //  reset logic that must happen after the children
        //  are reset; designed to be overriden by the
        //  actual device implementation
        //-------------------------------------------------
        protected virtual void device_reset_after_children() { }

        //-------------------------------------------------
        //  device_pre_save - called prior to saving the
        //  state, so that registered variables can be
        //  properly normalized
        //-------------------------------------------------
        protected virtual void device_pre_save() { }

        //-------------------------------------------------
        //  device_post_load - called after the loading a
        //  saved state, so that registered variables can
        //  be expanded as necessary
        //-------------------------------------------------
        protected virtual void device_post_load() { }

        //-------------------------------------------------
        //  device_clock_changed - called when the
        //  device clock is altered in any way; designed
        //  to be overridden by the actual device
        //  implementation
        //-------------------------------------------------
        protected virtual void device_clock_changed() { }

        //-------------------------------------------------
        //  device_debug_setup - called when the debugger
        //  is active to allow for device-specific setup
        //-------------------------------------------------
        protected virtual void device_debug_setup() { }

        //-------------------------------------------------
        //  device_timer - called whenever a device timer
        //  fires
        //-------------------------------------------------
        protected virtual void device_timer(emu_timer timer, device_timer_id id, int param)  /*void *ptr)*/ { }

        //------------------- end derived class overrides


        // private helpers

        //-------------------------------------------------
        //  subdevice_slow - perform a slow name lookup,
        //  caching the results
        //-------------------------------------------------
        protected device_t subdevice_slow(string tag)
        {
            // resolve the full path
            string fulltag = subtag(tag);

            // we presume the result is a rooted path; also doubled colons mess up our
            // tree walk, so catch them early
            global.assert(fulltag[0] == ':');
            global.assert(!fulltag.Contains("::"));

            // walk the device list to the final path
            device_t curdevice = mconfig().root_device();
            if (fulltag.Length > 1)
            {
                for (int start = 1, end = fulltag.IndexOf(':', start); start != 0 && curdevice != null; start = end + 1, end = fulltag.IndexOf(':', start))
                {
                    string part = (end == -1) ? fulltag.Substring(start) : fulltag.Substring(start, end - start);
                    curdevice = curdevice.subdevices().find(part);
                }
            }

            // if we got a match, add to the fast map
            if (curdevice != null)
                m_subdevices.m_tagmap.insert(tag, curdevice);

            return curdevice;
        }


        //-------------------------------------------------
        //  calculate_derived_clock - derive the device's
        //  clock from its owner, if so configured
        //-------------------------------------------------
        void calculate_derived_clock()
        {
            if ((m_configured_clock & 0xff000000) == 0xff000000)
            {
                global.assert(m_owner != null);
                set_unscaled_clock(m_owner.m_clock * ((m_configured_clock >> 12) & 0xfff) / ((m_configured_clock >> 0) & 0xfff));
            }
        }
    }


    // ======================> device_interface
    // device_interface represents runtime information for a particular device interface
    public class device_interface
    {
        // internal state
        device_interface m_interface_next;
        device_t m_device;
        string m_type;


        // construction/destruction

        //-------------------------------------------------
        //  device_interface - constructor
        //-------------------------------------------------
        protected device_interface(device_t device, string type)
        {
            m_device = device;
            m_type = type;


            //device_interface **tailptr;
            //for (tailptr = &device.m_interface_list; *tailptr != NULL; tailptr = &(*tailptr)->m_interface_next) ;
            //*tailptr = this;
            if (device.interfaces().first() == null)
            {
                device.interfaces().m_head = this;
            }
            else
            {
                for (device_interface tailptr = device.interfaces().first(); tailptr != null; tailptr = tailptr.m_interface_next)
                {
                    if (tailptr.m_interface_next == null)
                    {
                        tailptr.m_interface_next = this;
                        break;
                    }
                }
            }
        }


        //const char *interface_type() const { return m_type; }


        // casting helpers
        public device_t device() { return m_device; }
        //const device_t &device() const { return m_device; }
        //operator device_t &() { return m_device; }


        // iteration helpers
        public device_interface interface_next() { return m_interface_next; }


        // optional operation overrides
        //
        // WARNING: interface_pre_start must be callable multiple times in
        // case another interface throws a missing dependency.  In
        // particular, state saving registrations should be done in post.

        //-------------------------------------------------
        //  interface_config_complete - perform any
        //  operations now that the configuration is
        //  complete
        //-------------------------------------------------
        public virtual void interface_config_complete() { }

        //-------------------------------------------------
        //  interface_validity_check - default validation
        //  for a device after the configuration has been
        //  constructed
        //-------------------------------------------------
        protected virtual void interface_validity_check(validity_checker valid) { }

        //-------------------------------------------------
        //  interface_pre_start - called before the
        //  device's own start function
        //-------------------------------------------------
        public virtual void interface_pre_start() { }

        //-------------------------------------------------
        //  interface_post_start - called after the
        //  device's own start function
        //-------------------------------------------------
        public virtual void interface_post_start() { }

        //-------------------------------------------------
        //  interface_pre_reset - called before the
        //  device's own reset function
        //-------------------------------------------------
        public virtual void interface_pre_reset() { }

        //-------------------------------------------------
        //  interface_post_reset - called after the
        //  device's own reset function
        //-------------------------------------------------
        public virtual void interface_post_reset() { }

        //-------------------------------------------------
        //  interface_pre_stop - called before the
        //  device's own stop function
        //-------------------------------------------------
        public virtual void interface_pre_stop() { }

        //-------------------------------------------------
        //  interface_post_stop - called after the
        //  device's own stop function
        //-------------------------------------------------
        public virtual void interface_post_stop() { }

        //-------------------------------------------------
        //  interface_pre_save - called prior to saving the
        //  state, so that registered variables can be
        //  properly normalized
        //-------------------------------------------------
        public virtual void interface_pre_save() { }

        //-------------------------------------------------
        //  interface_post_load - called after the loading a
        //  saved state, so that registered variables can
        //  be expaneded as necessary
        //-------------------------------------------------
        public virtual void interface_post_load() { }

        //-------------------------------------------------
        //  interface_clock_changed - called when the
        //  device clock is altered in any way; designed
        //  to be overridden by the actual device
        //  implementation
        //-------------------------------------------------
        public virtual void interface_clock_changed() { }

        //-------------------------------------------------
        //  interface_debug_setup - called to allow
        //  interfaces to set up any debugging for this
        //  device
        //-------------------------------------------------
        public virtual void interface_debug_setup() { }
    }


    // ======================> device_iterator
    // helper class to iterate over the hierarchy of devices depth-first
    public class device_iterator : IEnumerable<device_t>
    {
        public class auto_iterator
        {
            // protected state
            protected device_t m_curdevice;
            int m_curdepth;
            int m_maxdepth;


            // construction
            public auto_iterator(device_t devptr, int curdepth, int maxdepth)
            {
                m_curdevice = devptr;
                m_curdepth = curdepth;
                m_maxdepth = maxdepth;
            }

            // getters
            public device_t current() { return m_curdevice; }
            int depth() { return m_curdepth; }

            // required operator overrides
            //bool operator==(auto_iterator const &iter) const { return m_curdevice == iter.m_curdevice; }
            //bool operator!=(auto_iterator const &iter) const { return m_curdevice != iter.m_curdevice; }
            //device_t &operator*() const { assert(m_curdevice); return *m_curdevice; }
            //device_t *operator->() const { return m_curdevice; }
            //auto_iterator &operator++() { advance(); return *this; }
            //auto_iterator operator++(int) { auto_iterator const result(*this); ++*this; return result; }


            // search depth-first for the next device
            public void advance()
            {
                // remember our starting position, and end immediately if we're nullptr
                if (m_curdevice != null)
                {
                    device_t start = m_curdevice;

                    // search down first
                    if (m_curdepth < m_maxdepth)
                    {
                        m_curdevice = start.subdevices().first();
                        if (m_curdevice != null)
                        {
                            m_curdepth++;
                            return;
                        }
                    }

                    // search next for neighbors up the ownership chain
                    while (m_curdepth > 0 && start != null)
                    {
                        // found a neighbor? great!
                        m_curdevice = start.next();
                        if (m_curdevice != null)
                            return;

                        // no? try our parent
                        start = start.owner();
                        m_curdepth--;
                    }

                    // returned to the top; we're done
                    m_curdevice = null;
                }
            }
        }


        // internal state
        device_t m_root;
        int m_maxdepth;


        // construction
        public device_iterator(device_t root, int maxdepth = 255)
        {
            m_root = root;
            m_maxdepth = maxdepth;
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<device_t> GetEnumerator()
        {
            auto_iterator iter = new auto_iterator(m_root, 0, m_maxdepth);

            while (iter.current() != null)
            {
                yield return iter.current();
                iter.advance();
            }
        }


        // standard iterators
        auto_iterator begin() { return new auto_iterator(m_root, 0, m_maxdepth); }
        auto_iterator end() { return new auto_iterator(null, 0, m_maxdepth); }

        // return first item
        device_t first() { return begin().current(); }


        // return the number of items available
        int count()
        {
            int result = 0;
            foreach (device_t item in this)
                result++;

            return result;
        }

        // return the index of a given item in the virtual list
        public int indexof(device_t device)
        {
            int index = 0;
            foreach (device_t item in this)
                if (item == device)
                    return index;
                else
                    index++;

            return -1;
        }

        // return the indexed item in the list
        public device_t byindex(int index)
        {
            foreach (device_t item in this)
                if (index-- == 0)
                    return item;

            return null;
        }
    }


    // ======================> device_type_iterator
    // helper class to find devices of a given type in the device hierarchy
    //template<device_type _DeviceType, class _DeviceClass = device_t>
    public class device_type_iterator</*DeviceType,*/ DeviceClass> : IEnumerable<DeviceClass>
        //where DeviceType : device_type
        where DeviceClass : device_t
    {
        class auto_iterator : device_iterator.auto_iterator
        {
            device_type m_type;

            // construction
            public auto_iterator(device_type type, device_t devptr, int curdepth, int maxdepth)
                : base(devptr, curdepth, maxdepth)
            {
                m_type = type;

                // make sure the first device is of the specified type
                while (m_curdevice != null && m_curdevice.type() != m_type)
                    advance();
            }

            // required operator overrides
            //bool operator==(auto_iterator const &iter) const { return m_curdevice == iter.m_curdevice; }
            //bool operator!=(auto_iterator const &iter) const { return m_curdevice != iter.m_curdevice; }

            // getters returning specified device type
            public new DeviceClass current() { return (DeviceClass)m_curdevice; }
            //_DeviceClass &operator*() const { assert(m_curdevice != nullptr); return downcast<_DeviceClass &>(*m_curdevice); }
            //DeviceClass *operator->() const { return downcast<DeviceClass *>(m_curdevice); }

            // search for devices of the specified type
            //const auto_iterator &operator++()
            //{
            //    advance();
            //    while (m_curdevice != nullptr && m_curdevice->type() != _DeviceType)
            //        advance();
            //    return *this;
            //}
            //auto_iterator operator++(int) { auto_iterator const result(*this); ++*this; return result; }

            public void advance_device_type()
            {
                advance();
                while (m_curdevice != null && m_curdevice.type() != m_type)
                    advance();
            }
        }


        // internal state
        device_type DeviceType;
        device_t m_root;
        int m_maxdepth;


        // construction
        public device_type_iterator(device_type type, device_t root, int maxdepth = 255)
        {
            DeviceType = type;
            m_root = root;
            m_maxdepth = maxdepth;
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<DeviceClass> GetEnumerator()
        {
            auto_iterator iter = new auto_iterator(DeviceType, m_root, 0, m_maxdepth);

            while (iter.current() != null)
            {
                yield return iter.current();
                iter.advance_device_type();
            }
        }


        // standard iterators
        auto_iterator begin() { return new auto_iterator(DeviceType, m_root, 0, m_maxdepth); }
        auto_iterator end() { return new auto_iterator(DeviceType, null, 0, m_maxdepth); }
        auto_iterator cbegin() { return new auto_iterator(DeviceType, m_root, 0, m_maxdepth); }
        auto_iterator cend() { return new auto_iterator(DeviceType, null, 0, m_maxdepth); }


        // getters

        // reset and return first item
        public DeviceClass first() { return begin().current(); }

        // return the number of items available
        public int count()
        {
            //return std::distance(cbegin(), cend());
            int result = 0;
            foreach (var item in this)
                result++;

            return result;
        }

        // return the index of a given item in the virtual list
        public int indexof(DeviceClass device)
        {
            int index = 0;
            foreach (DeviceClass item in this)
            {
                if (item == device)
                    return index;
                else
                    index++;
            }

            return -1;
        }

        // return the indexed item in the list
        public DeviceClass byindex(int index)
        {
            foreach (DeviceClass item in this)
            {
                if (index-- == 0)
                    return item;
            }

            return null;
        }
    }


    // ======================> device_interface_iterator
    // helper class to find devices with a given interface in the device hierarchy
    // also works for finding devices derived from a given subclass
    //template<class _InterfaceClass>
    public class device_interface_iterator<InterfaceClass> : IEnumerable<InterfaceClass> where InterfaceClass : device_interface
    {
        class auto_iterator : device_iterator.auto_iterator
        {
            // private state
            InterfaceClass m_interface;


            // construction
            public auto_iterator(device_t devptr, int curdepth, int maxdepth)
                : base(devptr, curdepth, maxdepth)
            {
                // set the iterator for the first device with the interface
                find_interface();
            }

            // getters returning specified interface type
            public new InterfaceClass current() { return m_interface; }
            //_InterfaceClass &operator*() const { assert(m_interface != nullptr); return *m_interface; }

            // search for devices with the specified interface
            //const auto_iterator &operator++() { advance(); find_interface(); return *this; }


            // private helper
            public void find_interface()
            {
                // advance until finding a device with the interface
                for ( ; m_curdevice != null; advance())
                {
                    if (m_curdevice.interface_(out m_interface))
                        return;
                }

                // if we run out of devices, make sure the interface pointer is null
                m_interface = null;
            }
        }


        // internal state
        device_t m_root;
        int m_maxdepth;


        // construction
        public device_interface_iterator(device_t root, int maxdepth = 255)
        {
            m_root = root;
            m_maxdepth = maxdepth;
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<InterfaceClass> GetEnumerator()
        {
            auto_iterator iter = new auto_iterator(m_root, 0, m_maxdepth);

            while (iter.current() != null)
            {
                yield return iter.current();
                iter.advance();
                iter.find_interface();
            }
        }


        // standard iterators
        auto_iterator begin() { return new auto_iterator(m_root, 0, m_maxdepth); }
        auto_iterator end() { return new auto_iterator(null, 0, m_maxdepth); }


        // return first item
        public InterfaceClass first() { return begin().current(); }


        // return the number of items available
        public int count()
        {
            int result = 0;
            foreach (InterfaceClass item in this)
                result++;

            return result;
        }

        // return the index of a given item in the virtual list
        int indexof(InterfaceClass intrf)
        {
            int index = 0;
            foreach (InterfaceClass item in this)
            {
                if (item == intrf)
                    return index;
                else
                    index++;
            }
            return -1;
        }

        // return the indexed item in the list
        public InterfaceClass byindex(int index)
        {
            foreach (InterfaceClass item in this)
            {
                if (index-- == 0)
                    return item;
            }

            return null;
        }
    }
}