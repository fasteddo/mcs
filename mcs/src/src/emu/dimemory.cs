// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using memory_interface_enumerator = mame.device_interface_enumerator<mame.device_memory_interface>;  //typedef device_interface_enumerator<device_memory_interface> memory_interface_enumerator;
using size_t = System.UInt64;
using u32 = System.UInt32;


namespace mame
{
    public class space_config_vector : std.vector<std.pair<int, address_space_config>>
    {
        public space_config_vector() : base() { }
        public space_config_vector(int count, std.pair<int, address_space_config> data = default) : base(count, data) { }
        public space_config_vector(u32 count, std.pair<int, address_space_config> data = default) : base(count, data) { }
        public space_config_vector(IEnumerable<std.pair<int, address_space_config>> collection) : base(collection) { }
    }


    // ======================> device_memory_interface
    public abstract class device_memory_interface : device_interface
    {
        //friend class device_scheduler;
        //template <typename T, typename U> struct is_related_class { static constexpr bool value = std::is_convertible<std::add_pointer_t<T>, std::add_pointer_t<U> >::value; };
        //template <typename T, typename U> struct is_related_device { static constexpr bool value = emu::detail::is_device_implementation<T>::value && is_related_class<T, U>::value; };
        //template <typename T, typename U> struct is_related_interface { static constexpr bool value = emu::detail::is_device_interface<T>::value && is_related_class<T, U>::value; };
        //template <typename T, typename U> struct is_unrelated_device { static constexpr bool value = emu::detail::is_device_implementation<T>::value && !is_related_class<T, U>::value; };
        //template <typename T, typename U> struct is_unrelated_interface { static constexpr bool value = emu::detail::is_device_interface<T>::value && !is_related_class<T, U>::value; };

        //using space_config_vector = std::vector<std::pair<int, const address_space_config *>>;


        // configuration
        std.vector<address_map_constructor> m_address_map = new std.vector<address_map_constructor>(); // address maps for each address space

        // internal state
        std.vector<address_space_config> m_address_config = new std.vector<address_space_config>(); // configuration for each space
        std.vector<address_space> m_addrspace = new std.vector<address_space>(); // reported address spaces


        // construction/destruction

        //-------------------------------------------------
        //  device_memory_interface - constructor
        //-------------------------------------------------
        public device_memory_interface(machine_config mconfig, device_t device)
            : base(device, "memory")
        {
            // configure the fast accessor
            device.interfaces().m_memory = this;
        }


        // configuration access
        public address_map_constructor get_addrmap(int spacenum = 0) { return spacenum >= 0 && spacenum < (int)(m_address_map.size()) ? m_address_map[spacenum] : null; }
        public address_space_config space_config(int spacenum = 0) { return spacenum >= 0 && spacenum < (int)(m_address_config.size()) ? m_address_config[spacenum] : null; }
        public int max_space_count() { return (int)m_address_config.size(); }


        // configuration helpers
        //template <typename T, typename U, typename Ret, typename... Params>
        //std::enable_if_t<is_related_device<T, U>::value> set_addrmap(int spacenum, T &obj, Ret (U::*func)(Params...)) { set_addrmap(spacenum, address_map_constructor(func, obj.tag(), &downcast<U &>(obj))); }
        //template <typename T, typename U, typename Ret, typename... Params>
        //std::enable_if_t<is_related_interface<T, U>::value> set_addrmap(int spacenum, T &obj, Ret (U::*func)(Params...)) { set_addrmap(spacenum, address_map_constructor(func, obj.device().tag(), &downcast<U &>(obj))); }
        //template <typename T, typename U, typename Ret, typename... Params>
        //std::enable_if_t<is_unrelated_device<T, U>::value> set_addrmap(int spacenum, T &obj, Ret (U::*func)(Params...)) { set_addrmap(spacenum, address_map_constructor(func, obj.tag(), &dynamic_cast<U &>(obj))); }
        //template <typename T, typename U, typename Ret, typename... Params>
        //std::enable_if_t<is_unrelated_interface<T, U>::value> set_addrmap(int spacenum, T &obj, Ret (U::*func)(Params...)) { set_addrmap(spacenum, address_map_constructor(func, obj.device().tag(), &dynamic_cast<U &>(obj))); }
        //template <typename T, typename Ret, typename... Params>
        //void set_addrmap(int spacenum, Ret (T::*func)(Params...));


        //-------------------------------------------------
        //  set_addrmap - connect an address map to a device
        //-------------------------------------------------
        public void set_addrmap(int spacenum, address_map_constructor map)
        {
            g.assert(0 <= spacenum);

            if (spacenum >= (int)(m_address_map.size()))
                m_address_map.resize((size_t)spacenum + 1);

            m_address_map[spacenum] = map;
        }


        // basic information getters
        public bool has_space(int index = 0) { return index >= 0 && index < (int)m_addrspace.size() && m_addrspace[index] != null; }
        public bool has_configured_map(int index = 0) { return index >= 0 && index < (int)m_address_map.size() && m_address_map[index] != null; }
        public address_space space(int index = 0) { g.assert(index >= 0 && index < (int)m_addrspace.size() && m_addrspace[index] != null); return m_addrspace[index]; }


        // address translation
        //bool translate(int spacenum, int intention, offs_t &address) { return memory_translate(spacenum, intention, address); }

        // deliberately ambiguous functions; if you have the memory interface
        // just use it
        //device_memory_interface &memory() { return *this; }


        // setup functions - these are called in sequence for all device_memory_interface by the memory manager
        //template <typename Space>
        public void allocate(address_space Space, memory_manager manager, int spacenum)
        {
            g.assert((0 <= spacenum) && (max_space_count() > spacenum));
            m_addrspace.resize(std.max(m_addrspace.size(), (size_t)spacenum + 1));
            g.assert(m_addrspace[spacenum] == null);
            m_addrspace[spacenum] = Space;  //std::make_unique<Space>(manager, *this, spacenum, space_config(spacenum)->addr_width());
        }
        public void prepare_maps() { foreach (var space in m_addrspace) { if (space != null) { space.prepare_map(); } } }
        public void populate_from_maps() { foreach (var space in m_addrspace) { if (space != null) { space.populate_from_map(); } } }
        public void set_log_unmap(bool log) { foreach (var space in m_addrspace) { if (space != null) { space.set_log_unmap(log); } } }


        // required overrides
        protected abstract space_config_vector memory_space_config();


        // optional operation overrides
        //virtual bool memory_translate(int spacenum, int intention, offs_t &address);


        // interface-level overrides

        //-------------------------------------------------
        //  interface_config_complete - perform final
        //  memory configuration setup
        //-------------------------------------------------
        public override void interface_config_complete()
        {
            space_config_vector r = memory_space_config();
            foreach (var entry in r)
            {
                if (entry.first >= (int)(m_address_config.size()))
                    m_address_config.resize((size_t)entry.first + 1);
                m_address_config[entry.first] = entry.second;
            }
        }


        public override void interface_post_stop()
        {
            foreach (var space in m_addrspace)
                if (space != null) space.Dispose();
        }


        //-------------------------------------------------
        //  interface_validity_check - perform validity
        //  checks on the memory configuration
        //-------------------------------------------------
        protected override void interface_validity_check(validity_checker valid)
        {
            throw new emu_unimplemented();
        }
    }


    // iterator
    //typedef device_interface_enumerator<device_memory_interface> memory_interface_enumerator;
}
