// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using u32 = System.UInt32;


namespace mame
{
    public static class softlist_global
    {
        public const int SOFTWARE_SUPPORTED_YES     = 0;
        public const int SOFTWARE_SUPPORTED_PARTIAL = 1;
        public const int SOFTWARE_SUPPORTED_NO      = 2;
    }


    // ======================> feature_list_item
    // an item in a list of name/value pairs
    class feature_list_item : simple_list_item<feature_list_item>
    {
        // internal state
        feature_list_item m_next;
        string m_name;
        string m_value;


        // construction/destruction
        feature_list_item(string name = null, string value = null)
        {
            m_next = null;
            m_name = name;
            m_value = value;
        }

        //feature_list_item(feature_list_item const &) = delete;
        //feature_list_item(feature_list_item &&) = delete;
        //feature_list_item& operator=(feature_list_item const &) = delete;
        //feature_list_item& operator=(feature_list_item &&) = delete;


        // getters
        public feature_list_item next() { return m_next; }
        public feature_list_item m_next_get() { return m_next; }
        public void m_next_set(feature_list_item value) { m_next = value; }

        public string name() { return m_name; }
        public string value() { return m_value; }
    }


    // ======================> software_part
    // a single part of a software item
    public class software_part : simple_list_item<software_part>
    {
        //friend class softlist_parser;


        // internal state
        software_part m_next;
        software_info m_info;
        string m_name;
        string m_interface;
        List<feature_list_item> m_featurelist;
        //std::vector<rom_entry>          m_romdata;


        // construction/destruction
        //-------------------------------------------------
        //  software_part - constructor
        //-------------------------------------------------
        software_part(software_info info, string name, string interface_name)
        {
            m_next = null;
            m_info = info;
            m_name = name;
            m_interface = interface_name;
        }

        //software_part(software_part const &) = delete;
        //software_part(software_part &&) = delete;
        //software_part& operator=(software_part const &) = delete;
        //software_part& operator=(software_part &&) = delete;


        // getters
        public software_part next() { return m_next; }
        public software_part m_next_get() { return m_next; }
        public void m_next_set(software_part value) { m_next = value; }

        public software_info info() { return m_info; }
        public string name() { return m_name; }
        //const char *interface() const { return m_interface; }
        //const simple_list<feature_list_item> &featurelist() const { return m_featurelist; }
        //rom_entry *romdata(uint32 index = 0) { return (index < m_romdata.count()) ? &m_romdata[index] : NULL; }


        // helpers

        //-------------------------------------------------
        //  matches_interface - determine if we match
        //  an interface in the provided list
        //-------------------------------------------------
        public bool matches_interface(string interface_list)
        {
            // if we have no interface, then we match by default
            if (m_interface.empty())
                return true;

            // find our interface at the beginning of the list or immediately following a comma
            while (true)
            {
                int foundIdx = std.strstr(interface_list, m_interface);  //char const *const found(std::strstr(interface_list, m_interface.c_str()));
                if (foundIdx == -1)  //if (!found)
                    return false;
                if (((foundIdx == 0) || (',' == interface_list[foundIdx - 1])) && ((',' == interface_list[foundIdx + m_interface.size()]) || interface_list[foundIdx + m_interface.size()] != 0))  //if (((found == interface_list) || (',' == found[-1])) && ((',' == found[m_interface.size()]) || !found[m_interface.size()]))
                    return true;
                int interface_listIdx = std.strchr(interface_list, ',');  //interface_list = std::strchr(interface_list, ',');
                if (interface_listIdx == -1)  //if (!interface_list)
                    return false;
                interface_list = interface_list.Remove(0, interface_listIdx);
                interface_list = interface_list.Remove(0, 1);  //++interface_list;
            }
        }


        //-------------------------------------------------
        //  feature - return the value of the given
        //  feature, if specified
        //-------------------------------------------------
        public string feature(string feature_name)
        {
            // scan the feature list for an entry matching feature_name and return the value
            //auto iter = std::find_if(
            //    m_featurelist.begin(),
            //    m_featurelist.end(),
            //    anon);//[&feature_name](const feature_list_item &feature) { return feature.name() == feature_name; });
            var iter = m_featurelist.Find(feature => { return feature.name() == feature_name; });

            //return iter != m_featurelist.end()
            //    ? iter->value().c_str()
            //    : nullptr;
            return iter != null ? iter.value() : null;
        }
    }


    // ======================> software_info
    // a single software item
    public class software_info
    {
        //friend class softlist_parser;


        // internal state
        u32 m_supported;
        string m_shortname;
        string m_longname;
        string m_parentname;
        string m_year;           // Copyright year on title screen, actual release dates can be tracked in external resources
        string m_publisher;
        //simple_list<feature_list_item> m_other_info;   // Here we store info like developer, serial #, etc. which belong to the software entry as a whole
        //simple_list<feature_list_item> m_shared_info;  // Here we store info like TV standard compatibility, or add-on requirements, etc. which get inherited
                                                    // by each part of this software entry (after loading these are stored in partdata->featurelist)
        std.list<software_part> m_partdata = new std.list<software_part>();


        // construction/destruction
        //-------------------------------------------------
        //  software_info - constructor
        //-------------------------------------------------
        software_info(string name, string parent, string supported)
        {
            m_supported = softlist_global.SOFTWARE_SUPPORTED_YES;
            m_shortname = name;
            m_parentname = parent;


            // handle the supported flag if provided
            if (supported == "partial")
                m_supported = softlist_global.SOFTWARE_SUPPORTED_PARTIAL;
            else if (supported == "no")
                m_supported = softlist_global.SOFTWARE_SUPPORTED_NO;
        }

        //software_info(software_info const &) = delete;
        //software_info(software_info &&) = delete;
        //software_info& operator=(software_info const &) = delete;
        //software_info& operator=(software_info &&) = delete;


        // getters
        public string shortname() { return m_shortname; }
        public string longname() { return m_longname; }
        public string parentname() { return m_parentname; }
        //const char *year() const { return m_year; }
        //const char *publisher() const { return m_publisher; }
        //const simple_list<feature_list_item> &other_info() const { return m_other_info; }
        //const simple_list<feature_list_item> &shared_info() const { return m_shared_info; }
        //UINT32 supported() const { return m_supported; }
        public std.list<software_part> parts() { return m_partdata; }


        // additional operations

        //-------------------------------------------------
        //  find_part - find a part by name with an
        //  optional interface match
        //-------------------------------------------------
        public software_part find_part(string part_name, string interface_name = null)
        {
            // look for the part by name and match against the interface if provided
            //var iter = std::find_if(
            //    m_partdata.begin(),
            //    m_partdata.end(),
            //    [&](const software_part &part)
            //    {
            //        // try to match the part_name (or all parts if part_name is empty), and then try
            //        // to match the interface (or all interfaces if interface is nullptr)
            //        return (part_name.empty() || part_name == part.name())
            //            && (interface == nullptr || part.matches_interface(interface));
            //    });
            software_part iter = null;
            foreach (var part in m_partdata)
            {
                if ((part_name.empty() || part_name == part.name())
                    && (interface_name == null || part.matches_interface(interface_name)))
                {
                    iter = part;
                    break;
                }
            }

            //return iter != null  //m_partdata.end()
            //    ? &*iter
            //    : null;
            return iter;
        }


        //-------------------------------------------------
        //  has_multiple_parts - return true if we have
        //  more than one part matching the given
        //  interface
        //-------------------------------------------------
        public bool has_multiple_parts(string interface_name)
        {
            int count = 0;

            // increment the count for each match and stop if we hit more than 1
            foreach (software_part part in m_partdata)
            {
                if (part.matches_interface(interface_name))
                {
                    if (++count > 1)
                        return true;
                }
            }

            return false;
        }
    }


    // ======================> softlist_parser
    class softlist_parser
    {
        enum parse_position
        {
            POS_ROOT,
            POS_MAIN,
            POS_SOFT,
            POS_PART,
            POS_DATA
        }


        // internal parsing state
        //util::core_file &                   m_file;
        //std::string                         m_filename;
        //std::list<software_info> &  m_infolist;
        //std::ostringstream &        m_errors;
        //struct XML_ParserStruct *   m_parser;
        //bool                        m_done;
        //std::string &               m_description;
        //bool                    m_data_accum_expected;
        //std::string             m_data_accum;
        //software_info *         m_current_info;
        //software_part *         m_current_part;
        //parse_position          m_pos;


        // construction (== execution)
        //softlist_parser(util::core_file &file, const std::string &filename, std::string &description, std::list<software_info> &infolist, std::ostringstream &errors);


        // internal parsing helpers
        //const char *infoname() const { return (m_current_info != nullptr) ? m_current_info->shortname().c_str() : "???"; }
        //int line() const;
        //int column() const;
        //const char *parser_error() const;


        // internal error helpers
        //template <typename Format, typename... Params> void parse_error(Format &&fmt, Params &&... args);
        //void unknown_tag(const char *tagname) { parse_error("Unknown tag: %s", tagname); }
        //void unknown_attribute(const char *attrname) { parse_error("Unknown attribute: %s", attrname); }


        // internal helpers
        //template <typename T> std::vector<std::string> parse_attributes(const char **attributes, const T &attrlist);
        //bool parse_name_and_value(const char **attributes, std::string &name, std::string &value);
        //void add_rom_entry(std::string &&name, std::string &&hashdata, u32 offset, u32 length, u32 flags);


        // expat callbacks
        //static void start_handler(void *data, const char *tagname, const char **attributes);
        //static void data_handler(void *data, const char *s, int len);
        //static void end_handler(void *data, const char *name);


        // internal parsing
        //void parse_root_start(const char *tagname, const char **attributes);
        //void parse_main_start(const char *tagname, const char **attributes);
        //void parse_soft_start(const char *tagname, const char **attributes);
        //void parse_part_start(const char *tagname, const char **attributes);
        //void parse_data_start(const char *tagname, const char **attributes);
        //void parse_soft_end(const char *name);
    }
}
