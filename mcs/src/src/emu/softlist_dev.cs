// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using software_list_device_enumerator = mame.device_type_enumerator<mame.software_list_device>;  //typedef device_type_enumerator<software_list_device> software_list_device_enumerator;
using u32 = System.UInt32;

using static mame.device_global;


namespace mame
{
    public enum software_compatibility
    {
        SOFTWARE_IS_COMPATIBLE,
        SOFTWARE_IS_INCOMPATIBLE,
        SOFTWARE_NOT_COMPATIBLE
    }


    // ======================> software_list_device
    // device representing a software list
    public class software_list_device : device_t
    {
        //DEFINE_DEVICE_TYPE(SOFTWARE_LIST, software_list_device, "software_list", "Software List")
        public static readonly emu.detail.device_type_impl SOFTWARE_LIST = DEFINE_DEVICE_TYPE("software_list", "Software List", (type, mconfig, tag, owner, clock) => { return new software_list_device(mconfig, tag, owner, clock); });


        // configuration state
        string m_list_name;
        //softlist_type               m_list_type;
        //const char *                m_filter;

        // internal state
        //bool                        m_parsed;
        //std::string                 m_filename;
        //std::string                 m_shortname;
        //std::string                 m_description;
        //std::string                 m_errors;
        //List<software_info> m_infolist;


        // construction/destruction
        //-------------------------------------------------
        //  software_list_device - constructor
        //-------------------------------------------------
        public software_list_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : base(mconfig, SOFTWARE_LIST, tag, owner, clock)
        {
            throw new emu_unimplemented();
#if false
            m_list_type(softlist_type::ORIGINAL_SYSTEM),
            m_filter(nullptr),
            m_parsed(false),
            m_file(mconfig.options().hash_path(), OPEN_FLAG_READ),
            m_description("")
#endif
        }


        // inline configuration helpers
        //software_list_device &set_type(const char *list, softlist_type list_type) { m_list_name.assign(list); m_list_type = list_type; return *this; }
        //software_list_device &set_original(const char *list) { return set_type(list, softlist_type::ORIGINAL_SYSTEM); }
        //software_list_device &set_compatible(const char *list) { return set_type(list, softlist_type::COMPATIBLE_SYSTEM); }
        //software_list_device &set_filter(const char *filter) { m_filter = filter; return *this; }


        // getters
        public string list_name() { return m_list_name; }
        //softlist_type list_type() const { return m_list_type; }
        public bool is_original() { throw new emu_unimplemented(); }  //bool is_original() const { return softlist_type::ORIGINAL_SYSTEM == m_list_type; }
        //bool is_compatible() const { return softlist_type::COMPATIBLE_SYSTEM == m_list_type; }
        public string filter() { throw new emu_unimplemented(); }  //const char *filter() const { return m_filter; }


        // getters that may trigger a parse
        //const std::string &description() { if (!m_parsed) parse(); return m_description; }
        //bool valid() { if (!m_parsed) parse(); return !m_infolist.empty(); }
        //const char *errors_string() { if (!m_parsed) parse(); return m_errors.c_str(); }
        public std.list<software_info> get_info() { throw new emu_unimplemented(); }  //{ if (!m_parsed) parse(); return m_infolist; }


        // operations
        public software_info find(string look_for) { throw new emu_unimplemented(); }
        //void find_approx_matches(const std::string &name, int matches, const software_info **list, const char *interface);
        public void release() { throw new emu_unimplemented(); }
        public software_compatibility is_compatible(software_part part) { throw new emu_unimplemented(); }


        // static helpers
        public static software_list_device find_by_name(machine_config mconfig, string name) { throw new emu_unimplemented(); }
        public static void display_matches(machine_config config, string interface_name, string name) { throw new emu_unimplemented(); }
        //static device_image_interface *find_mountable_image(const machine_config &mconfig, const software_part &part, std::function<bool (const device_image_interface &)> filter);
        //static device_image_interface find_mountable_image(machine_config mconfig, software_part part);


        // device-level overrides
        protected override void device_start() { throw new emu_unimplemented(); }
        protected override void device_validity_check(validity_checker valid) { throw new emu_unimplemented(); }


        // internal helpers
        //void parse();
        //void internal_validity_check(validity_checker &valid) ATTR_COLD;
    }


    // device type iterator
    //typedef device_type_enumerator<software_list_device> software_list_device_enumerator;
}
