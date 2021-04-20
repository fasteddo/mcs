// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using char32_t = System.UInt32;
using filter_map = mame.std.map<mame.ui.machine_filter.type, mame.ui.machine_filter>;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    public static class utils_global
    {
        //enum
        //{
        public const uint8_t RP_FIRST  = 0;
        public const uint8_t RP_IMAGES = RP_FIRST;
        public const uint8_t RP_INFOS  = RP_FIRST + 1;
        public const uint8_t RP_LAST   = RP_INFOS;
        //}


        //enum
        //{
        public const uint16_t SHOW_PANELS      = 0;
        public const uint16_t HIDE_LEFT_PANEL  = 1;
        public const uint16_t HIDE_RIGHT_PANEL = 2;
        public const uint16_t HIDE_BOTH        = 3;
        //}


        //enum
        //{
        public const int HOVER_DAT_UP       = -1000;
        public const int HOVER_DAT_DOWN     = HOVER_DAT_UP +  1;
        public const int HOVER_UI_LEFT      = HOVER_DAT_UP +  2;
        public const int HOVER_UI_RIGHT     = HOVER_DAT_UP +  3;
        public const int HOVER_ARROW_UP     = HOVER_DAT_UP +  4;
        public const int HOVER_ARROW_DOWN   = HOVER_DAT_UP +  5;
        public const int HOVER_B_FAV        = HOVER_DAT_UP +  6;
        public const int HOVER_B_EXPORT     = HOVER_DAT_UP +  7;
        public const int HOVER_B_DATS       = HOVER_DAT_UP +  8;
        public const int HOVER_RPANEL_ARROW = HOVER_DAT_UP +  9;
        public const int HOVER_LPANEL_ARROW = HOVER_DAT_UP + 10;
        public const int HOVER_FILTER_FIRST = HOVER_DAT_UP + 11;
        public const int HOVER_FILTER_LAST  = HOVER_FILTER_FIRST + ((int)ui.machine_filter.type.COUNT > (int)ui.software_filter.type.COUNT ? (int)ui.machine_filter.type.COUNT : (int)ui.software_filter.type.COUNT);  // Math.Max(ui.machine_filter.type.COUNT, ui.software_filter.type.COUNT),
        public const int HOVER_RP_FIRST     = HOVER_FILTER_LAST + 1;
        public const int HOVER_RP_LAST      = HOVER_RP_FIRST + 1 + RP_LAST;
        public const int HOVER_INFO_TEXT    = HOVER_RP_LAST + 1;
        //}


        public const int MAX_CHAR_INFO   = 256;
        public const int MAX_CUST_FILTER = 8;


        public const string UI_VERSION_TAG = "# UI INFO ";


        //-------------------------------------------------
        //  search a substring with even partial matching
        //-------------------------------------------------
        public static int fuzzy_substring(string s_needle, string s_haystack)
        {
            if (s_needle.Length == 0)
                return s_haystack.Length;
            if (s_haystack.Length == 0)
                return s_needle.Length;

            s_needle = s_needle.ToLower();
            s_haystack = s_haystack.ToLower();

            if (s_needle == s_haystack)
                return 0;
            if (s_haystack.IndexOf(s_needle) != -1)
                return 0;

            var row1 = new int [s_haystack.Length + 2];  // auto *row1 = global_alloc_array_clear<int>(s_haystack.Length + 2);
            var row2 = new int [s_haystack.Length + 2];  // auto *row2 = global_alloc_array_clear<int>(s_haystack.Length + 2);

            for (int i = 0; i < s_needle.Length; ++i)
            {
                row2[0] = i + 1;
                for (int j = 0; j < s_haystack.Length; ++j)
                {
                    int cost = (s_needle[i] == s_haystack[j]) ? 0 : 1;
                    row2[j + 1] = Math.Min(row1[j + 1] + 1, Math.Min(row2[j] + 1, row1[j] + cost));
                }

                var tmp = row1;  // int *tmp = row1;
                row1 = row2;
                row2 = tmp;
            }

            var first = row1;  //int *first;
            int firstIdx = 0;
            var smallest = row2;  //int *smallest;
            int smallestIdx = 0;
            //first = smallest = row1;
            int lastIdx = s_haystack.Length;  //int *last = row1 + s_haystack.Length;

            while (++firstIdx != lastIdx)
            {
                if (first[firstIdx] < smallest[smallestIdx])  // if (*first < *smallest)
                    smallestIdx = firstIdx;  // smallest = first;
            }

            int rv = smallest[smallestIdx];  // int rv = *smallest;
            row1 = null;  // global_free_array(row1);
            row2 = null;  // global_free_array(row2);

            return rv;
        }


        public static string chartrimcarriage(string str)
        {
            //char *pstr = strrchr(str, '\n');
            //if (pstr)
            //    str[pstr - str] = '\0';
            //pstr = strrchr(str, '\r');
            //if (pstr)
            //    str[pstr - str] = '\0';
            //return str;
            return str.Trim();
        }


        //const char* strensure(const char* s);
        //int getprecisionchr(const char* s);
        //std::vector<std::string> tokenize(const std::string &text, char sep);


        public delegate bool input_character_filter(char32_t uchar);

        //-------------------------------------------------
        //  input_character - inputs a typed character
        //  into a buffer
        //-------------------------------------------------
        //template <typename F>
        public static bool input_character(string buffer, UInt32 size, char32_t unichar, input_character_filter filter)
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  input_character - inputs a typed character
        //  into a buffer
        //-------------------------------------------------
        //template <typename F>
        public static bool input_character(string buffer, char32_t unichar, input_character_filter filter)
        {
            throw new emu_unimplemented();
        }
    }


    // TODO: namespace these things

    public class ui_system_info
    {
        public game_driver driver = null;
        public int index;
        public bool available = false;

        public string ucs_shortname;  //std::u32string ucs_shortname;
        public string ucs_description;  //std::u32string ucs_description;
        public string ucs_manufacturer_description;  //std::u32string ucs_manufacturer_description;


        public ui_system_info() { }
        public ui_system_info(game_driver d, int i, bool a) { driver = d; index = i; available = a; }
    }


    // GLOBAL STRUCTURES
    public class ui_software_info
    {
        public string shortname;
        public string longname;
        public string parentname;
        public string year;
        public string publisher;
        public byte supported = 0;
        public string part;
        public game_driver driver = null;
        public string listname;
        public string interface_;
        public string instance;
        public byte startempty = 0;
        public string parentlongname;
        public string usage;
        public string devicetype;
        public bool available = false;


        public ui_software_info() {}
        public ui_software_info(string sname, string lname, string pname, string y, string pub,
            byte s, string pa, game_driver d, string li, string i, string istring, byte em,
            string plong, string u, string de, bool av)
        {
            shortname = sname; longname = lname; parentname = pname; year = y; publisher = pub;
            supported = s; part = pa; driver = d; listname = li; interface_ = i; instance = istring; startempty = em;
            parentlongname = plong; usage = u; devicetype = de; available = av;
        }

        public static bool operator!=(ui_software_info left, ui_software_info right) { return !(left == right); }
        public static bool operator==(ui_software_info left, ui_software_info right)
        {
            if (ReferenceEquals(null, left))
                return ReferenceEquals(null, right);

            return left.shortname == right.shortname && left.longname == right.longname && left.parentname == right.parentname
                && left.year == right.year && left.publisher == right.publisher && left.supported == right.supported
                && left.part == right.part && left.driver == right.driver && left.listname == right.listname
                && left.interface_ == right.interface_ && left.instance == right.instance && left.startempty == right.startempty
                && left.parentlongname == right.parentlongname && left.usage == right.usage && left.devicetype == right.devicetype;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || base.GetType() != obj.GetType()) return false;
            return this == (ui_software_info)obj;
        }
        public override int GetHashCode() { return shortname.GetHashCode() ^ longname.GetHashCode() ^ parentname.GetHashCode() ^ year.GetHashCode(); }
    }


    namespace ui
    {
        class software_filter_data
        {
            std.vector<string> m_regions;
            std.vector<string> m_publishers;
            std.vector<string> m_years;
            std.vector<string> m_device_types;
            std.vector<string> m_list_names;
            std.vector<string> m_list_descriptions;


            public std.vector<string> regions() { return m_regions; }
            public std.vector<string> publishers() { return m_publishers; }
            public std.vector<string> years() { return m_years; }
            public std.vector<string> device_types() { return m_device_types; }
            public std.vector<string> list_names() { return m_list_names; }
            public std.vector<string> list_descriptions() { return m_list_descriptions; }

            // adding entries
            //void add_region(std::string const &longname);
            //void add_publisher(std::string const &publisher);
            //void add_year(std::string const &year);
            //void add_device_type(std::string const &device_type);
            //void add_list(std::string const &name, std::string const &description);
            //void finalise();

            // use heuristics to extract meaningful parts from software list fields
            public static string extract_region(string longname)
            {
                string fullname = longname;
                fullname = fullname.ToLower();
                int found = fullname.IndexOf("(");  //std::string::size_type const found(fullname.find('('));
                if (found != -1)
                {
                    throw new emu_unimplemented();
                }

                return "<none>";
            }

            public static string extract_publisher(string publisher)
            {
                int found = publisher.find("(");  // std::string::size_type const found(publisher.find('('));
                return publisher.substr(0, found - ((found != 0 && (-1 != found)) ? 1 : 0));
            }
        }


        //template <class Impl, typename Entry>
        public abstract class filter_base<Impl, Entry> : global_object
        {
            //std::function<void (Impl &)> &&handler);
            public delegate void filter_handler(Impl filter);


            //typedef std::unique_ptr<Impl> ptr;

            //using entry_type = Entry;


            protected filter_base() { }


            public abstract string config_name();
            public abstract string display_name();
            public abstract string filter_text();

            public abstract bool apply(Entry info);

            public abstract void show_ui(mame_ui_manager mui, render_container container, filter_handler handler);  //, std::function<void (Impl &)> &&handler);

            public abstract bool wants_adjuster();
            public abstract string adjust_text();
            public abstract UInt32 arrow_flags();
            public abstract bool adjust_left();
            public abstract bool adjust_right();

            public abstract void save_ini(emu_file file, UInt32 indent);

            // moved to derived classes since templates are different
            //template <typename InputIt, class OutputIt>
            //void apply<InputIt, OutputIt>(InputIt first, InputIt last, OutputIt dest)
            //{
            //    std::copy_if(first, last, dest);//, [this] (auto const &info) { return this->apply(info); });
            //}

            // dup of abstract version above, c++ pointer version, not needed
            //bool apply(Entry info) { return apply(info); }
        }


        public abstract class machine_filter : filter_base<machine_filter, ui_system_info>
        {
            public enum type// : uint16_t
            {
                ALL = 0,
                AVAILABLE,
                UNAVAILABLE,
                WORKING,
                NOT_WORKING,
                MECHANICAL,
                NOT_MECHANICAL,
                CATEGORY,
                FAVORITE,
                BIOS,
                NOT_BIOS,
                PARENTS,
                CLONES,
                MANUFACTURER,
                YEAR,
                SAVE,
                NOSAVE,
                CHD,
                NOCHD,
                VERTICAL,
                HORIZONTAL,
                CUSTOM,

                COUNT,
                FIRST = 0,
                LAST = COUNT - 1
            }


            //using filter_base<machine_filter, ui_system_info>::config_name;
            //using filter_base<machine_filter, ui_system_info>::display_name;


            static readonly string [] MACHINE_FILTER_NAMES = new string[(int)type.COUNT]
            {
                "Unfiltered",
                "Available",
                "Unavailable",
                "Working",
                "Not Working",
                "Mechanical",
                "Not Mechanical",
                "Category",
                "Favorites",
                "BIOS",
                "Not BIOS",
                "Parents",
                "Clones",
                "Manufacturer",
                "Year",
                "Save Supported",
                "Save Unsupported",
                "CHD Required",
                "No CHD Required",
                "Vertical Screen",
                "Horizontal Screen",
                "Custom Filter"
            };


            protected machine_filter() : base() { }


            public void apply(MemoryContainer<ui_system_info> source, MemoryContainer<ui_system_info> dest)
            {
                foreach (var s in source)
                    if (apply(s))
                        dest.Add(s);
            }


            public abstract type get_type();
            public abstract string adorned_display_name(type n);

            public static machine_filter create(type n, machine_filter_data data) { return create(n, data, null, null, 0); }
            public static machine_filter create(emu_file file, machine_filter_data data) { return create(file, data, 0); }

            public static string config_name(type n)
            {
                assert(type.COUNT > n);
                return MACHINE_FILTER_NAMES[(int)n];
            }

            public static string display_name(type n)
            {
                assert(type.COUNT > n);
                return MACHINE_FILTER_NAMES[(int)n];
            }


            //-------------------------------------------------
            //  public machine filter interface
            //-------------------------------------------------
            protected static machine_filter create(type n, machine_filter_data data, string value, emu_file file, UInt32 indent)
            {
                assert(type.COUNT > n);

                switch (n)
                {
                case type.ALL:
                    return new all_machine_filter(data, value, file, indent);
                case type.AVAILABLE:
                    return new available_machine_filter(data, value, file, indent);
                case type.UNAVAILABLE:
                    return new unavailable_machine_filter(data, value, file, indent);
                case type.WORKING:
                    return new working_machine_filter(data, value, file, indent);
                case type.NOT_WORKING:
                    return new not_working_machine_filter(data, value, file, indent);
                case type.MECHANICAL:
                    return new mechanical_machine_filter(data, value, file, indent);
                case type.NOT_MECHANICAL:
                    return new not_mechanical_machine_filter(data, value, file, indent);
                case type.CATEGORY:
                    return new category_machine_filter(data, value, file, indent);
                case type.FAVORITE:
                    return new favorite_machine_filter(data, value, file, indent);
                case type.BIOS:
                    return new bios_machine_filter(data, value, file, indent);
                case type.NOT_BIOS:
                    return new not_bios_machine_filter(data, value, file, indent);
                case type.PARENTS:
                    return new parents_machine_filter(data, value, file, indent);
                case type.CLONES:
                    return new clones_machine_filter(data, value, file, indent);
                case type.MANUFACTURER:
                    return new manufacturer_machine_filter(data, value, file, indent);
                case type.YEAR:
                    return new year_machine_filter(data, value, file, indent);
                case type.SAVE:
                    return new save_machine_filter(data, value, file, indent);
                case type.NOSAVE:
                    return new nosave_machine_filter(data, value, file, indent);
                case type.CHD:
                    return new chd_machine_filter(data, value, file, indent);
                case type.NOCHD:
                    return new nochd_machine_filter(data, value, file, indent);
                case type.VERTICAL:
                    return new vertical_machine_filter(data, value, file, indent);
                case type.HORIZONTAL:
                    return new horizontal_machine_filter(data, value, file, indent);
                case type.CUSTOM:
                    return new custom_machine_filter(data, value, file, indent);
                case type.COUNT: // not valid, but needed to suppress warnings
                    break;
                }

                return null;
            }


            protected static machine_filter create(emu_file file, machine_filter_data data, UInt32 indent)
            {
                string buffer;  //= new char[utils_global.MAX_CHAR_INFO];
                if (file.gets(out buffer, utils_global.MAX_CHAR_INFO) == null)
                    return null;

                // split it into a key/value or bail
                string key = buffer;
                for (UInt32 i = 0; (2 * indent) > i; ++i)
                {
                    if ((key.length() <= i) || (' ' != key[(int)i]))
                        return null;
                }
                key = key.substr(2 * (int)indent);
                int split = key.IndexOf(" = ");
                if (-1 == split)
                    return null;
                int nl = key.find_first_of("\r\n", split);
                string value = key.substr(split + 3, (-1 == nl) ? nl : (nl - split - 3));
                key = key.Substring(split);  // key.resize(split);

                // look for a filter type that matches
                for (type n = type.FIRST; type.COUNT > n; ++n)
                {
                    if (key == config_name(n))
                        return create(n, data, value.c_str(), file, indent);
                }

                return null;
            }
        }

        //DECLARE_ENUM_INCDEC_OPERATORS(machine_filter::type)


        abstract class software_filter : filter_base<software_filter, ui_software_info>
        {
            public enum type// : uint16_t
            {
                ALL = 0,
                AVAILABLE,
                UNAVAILABLE,
                FAVORITE,
                PARENTS,
                CLONES,
                YEAR,
                PUBLISHERS,
                SUPPORTED,
                PARTIAL_SUPPORTED,
                UNSUPPORTED,
                REGION,
                DEVICE_TYPE,
                LIST,
                CUSTOM,

                COUNT,
                FIRST = 0,
                LAST = COUNT - 1
            }


            //using filter_base<software_filter, ui_software_info>::config_name;
            //using filter_base<software_filter, ui_software_info>::display_name;


            static readonly string [] SOFTWARE_FILTER_NAMES = new string[(int)type.COUNT]
            {
                "Unfiltered",
                "Available",
                "Unavailable",
                "Favorites",
                "Parents",
                "Clones",
                "Year",
                "Publisher",
                "Supported",
                "Partially Supported",
                "Unsupported",
                "Release Region",
                "Device Type",
                "Software List",
                "Custom Filter"
            };


            protected software_filter() : base() { }


            public abstract type get_type();
            protected abstract string adorned_display_name(type n);

            protected static software_filter create(type n, software_filter_data data) { return create(n, data, null, null, 0); }
            protected static software_filter create(emu_file file, software_filter_data data) { return create(file, data, 0); }

            protected static string config_name(type n)
            {
                assert(type.COUNT > n);
                return SOFTWARE_FILTER_NAMES[(int)n];
            }
            protected static string display_name(type n)
            {
                assert(type.COUNT > n);
                return SOFTWARE_FILTER_NAMES[(int)n];
            }


            protected static software_filter create(type n, software_filter_data data, string value, emu_file file, UInt32 indent)
            {
                assert(type.COUNT > n);

                switch (n)
                {
                case type.ALL:
                    return new all_software_filter(data, value, file, indent);
                case type.AVAILABLE:
                    return new available_software_filter(data, value, file, indent);
                case type.UNAVAILABLE:
                    return new unavailable_software_filter(data, value, file, indent);
                case type.FAVORITE:
                    return new favorite_software_filter(data, value, file, indent);
                case type.PARENTS:
                    return new parents_software_filter(data, value, file, indent);
                case type.CLONES:
                    return new clones_software_filter(data, value, file, indent);
                case type.YEAR:
                    return new years_software_filter(data, value, file, indent);
                case type.PUBLISHERS:
                    return new publishers_software_filter(data, value, file, indent);
                case type.SUPPORTED:
                    return new supported_software_filter(data, value, file, indent);
                case type.PARTIAL_SUPPORTED:
                    return new partial_supported_software_filter(data, value, file, indent);
                case type.UNSUPPORTED:
                    return new unsupported_software_filter(data, value, file, indent);
                case type.REGION:
                    return new region_software_filter(data, value, file, indent);
                case type.DEVICE_TYPE:
                    return new device_type_software_filter(data, value, file, indent);
                case type.LIST:
                    return new list_software_filter(data, value, file, indent);
                case type.CUSTOM:
                    return new custom_software_filter(data, value, file, indent);
                case type.COUNT: // not valid, but needed to suppress warnings
                    break;
                }

                return null;
            }


            protected static software_filter create(emu_file file, software_filter_data data, UInt32 indent)
            {
                string buffer;  //char buffer[MAX_CHAR_INFO];
                if (file.gets(out buffer, utils_global.MAX_CHAR_INFO) == null)
                    return null;

                // split it into a key/value or bail
                string key = buffer;
                for (int i = 0; (2 * indent) > i; ++i)
                {
                    if ((key.length() <= i) || (' ' != key[i]))
                        return null;
                }
                key = key.substr(2 * (int)indent);
                int split = key.find(" = ");
                if (-1 == split)
                    return null;
                int nl = key.find_first_of("\r\n", split);
                string value = key.substr(split + 3, (-1 == nl) ? nl : (nl - split - 3));
                key = key.Substring(split);  // key.resize(split);

                // look for a filter type that matches
                for (type n = type.FIRST; type.COUNT > n; ++n)
                {
                    if (key == config_name(n))
                        return create(n, data, value.c_str(), file, indent);
                }

                return null;
            }
        }

        //DECLARE_ENUM_INCDEC_OPERATORS(software_filter::type)


        public class machine_filter_data : global_object
        {
            //using filter_map = std::map<machine_filter::type, machine_filter::ptr>;

            std.vector<string> m_manufacturers = new std.vector<string>();
            std.vector<string> m_years = new std.vector<string>();

            machine_filter.type m_current_filter = machine_filter.type.ALL;
            filter_map m_filters = new filter_map();


            std.vector<string> manufacturers() { return m_manufacturers; }
            public std.vector<string> years() { return m_years; }

            // adding entries
            public void add_manufacturer(string manufacturer)
            {
                string name = extract_manufacturer(manufacturer);
                //std::vector<std::string>::iterator const pos(std::lower_bound(m_manufacturers.begin(), m_manufacturers.end(), name));
                //if ((m_manufacturers.end() == pos) || (*pos != name))
                //    m_manufacturers.emplace(pos, std::move(name));
                int pos = 0;
                for (; pos < m_manufacturers.Count; pos++)
                {
                    if (m_manufacturers[pos].CompareTo(name) >= 0)
                        break;
                }
                if ((m_manufacturers.Count == pos) || (m_manufacturers[pos] != name))
                    m_manufacturers.emplace(pos, name);
            }


            public void add_year(string year)
            {
                //std::vector<std::string>::iterator const pos(std::lower_bound(m_years.begin(), m_years.end(), year));
                //if ((m_years.end() == pos) || (*pos != year))
                //    m_years.emplace(pos, year);
                int pos = 0;
                for (; pos < m_years.Count; pos++)
                {
                    if (m_years[pos].CompareTo(year) >= 0)
                        break;
                }
                if ((m_years.Count == pos) || (m_years[pos] != year))
                    m_years.emplace(pos, year);
            }


            public void finalise()
            {
                m_manufacturers.Sort();  //std::stable_sort(m_manufacturers.begin(), m_manufacturers.end());
                m_years.Sort();  //std::stable_sort(m_years.begin(), m_years.end());
            }


            // use heuristics to extract meaningful parts from machine metadata
            public static string extract_manufacturer(string manufacturer)
            {
                int found = manufacturer.find('(');
                if ((found != -1) && (found > 0))
                    return manufacturer.substr(0, found - 1);
                else
                    return manufacturer;
            }

            // the selected filter
            public machine_filter.type get_current_filter_type() { return m_current_filter; }
            public void set_current_filter_type(machine_filter.type type) { m_current_filter = type; }

            // managing current filters
            public void set_filter(machine_filter filter) { throw new emu_unimplemented(); }
            public filter_map get_filters() { return m_filters; }
            public machine_filter get_filter(machine_filter.type type) { throw new emu_unimplemented(); }
            public machine_filter get_current_filter() { return m_filters.find(m_current_filter); }

            public string get_config_string()
            {
                var active_filter = m_filters.find(m_current_filter);
                if (null != active_filter)
                {
                    string val = active_filter.filter_text();
                    return val != null ? string_format("{0},{1}", active_filter.config_name(), val) : active_filter.config_name();
                }
                else
                {
                    return machine_filter.config_name(m_current_filter);
                }
            }


            public bool load_ini(emu_file file)
            {
                machine_filter flt = machine_filter.create(file, this);
                if (flt != null)
                {
                    // TODO: it should possibly replace an existing item here, but it may be relying on that not happening because it never clears the first start flag
                    m_current_filter = flt.get_type();
                    m_filters.emplace(m_current_filter, flt);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


#if false
        class software_filter_data
        {
        public:
            std::vector<std::string> const &regions()           const { return m_regions; }
            std::vector<std::string> const &publishers()        const { return m_publishers; }
            std::vector<std::string> const &years()             const { return m_years; }
            std::vector<std::string> const &device_types()      const { return m_device_types; }
            std::vector<std::string> const &list_names()        const { return m_list_names; }
            std::vector<std::string> const &list_descriptions() const { return m_list_descriptions; }

            // adding entries
            void add_region(std::string const &longname);
            void add_publisher(std::string const &publisher);
            void add_year(std::string const &year);
            void add_device_type(std::string const &device_type);
            void add_list(std::string const &name, std::string const &description);
            void finalise();

            // use heuristics to extract meaningful parts from software list fields
            static std::string extract_region(std::string const &longname);
            static std::string extract_publisher(std::string const &publisher);

        private:
            std::vector<std::string>    m_regions;
            std::vector<std::string>    m_publishers;
            std::vector<std::string>    m_years;
            std::vector<std::string>    m_device_types;
            std::vector<std::string>    m_list_names, m_list_descriptions;
        };
#endif
    }


    // FIXME: this stuff shouldn't all be globals

    // GLOBAL CLASS
    static class ui_globals
    {
        public static uint8_t curdats_view = 0;
        public static uint8_t curdats_total = 0;
        public static uint8_t cur_sw_dats_view = 0;
        public static uint8_t cur_sw_dats_total = 0;
        public static uint8_t rpanel = 0;
        public static bool default_image = true;
        public static bool reset = false;
        public static int visible_main_lines = 0;
        public static int visible_sw_lines = 0;
        public static uint16_t panels_status = 0;
    }


    // Manufacturers
    public static class c_mnfct
    {
        public static std.vector<string> ui = new std.vector<string>();

        public static string getname(string str)
        {
            string name = str;
            int found = name.IndexOf("(");

            if (found != -1)
                return name.substr(0, found - 1);
            else
                return name;
        }
    }


    // Years
    public static class c_year
    {
        public static std.vector<string> ui = new std.vector<string>();
    }


    struct main_filters
    {
        public static ui.machine_filter.type actual;
        public static std.map<ui.machine_filter.type, ui.machine_filter> filters = new std.map<ui.machine_filter.type, ui.machine_filter>();
    }


    namespace ui
    {
        public static class utils_globals_temp
        {
            static readonly string [] SOFTWARE_REGIONS =
            {
                "arab", "arg", "asia", "aus", "aut",
                "bel", "blr", "bra",
                "can", "chi", "chn", "cze",
                "den",
                "ecu", "esp", "euro",
                "fin", "fra",
                "gbr", "ger", "gre",
                "hkg", "hun",
                "irl", "isr", "isv", "ita",
                "jpn",
                "kaz", "kor",
                "lat", "lux",
                "mex",
                "ned", "nld", "nor", "nzl",
                "pol",
                "rus",
                "slo", "spa", "sui", "swe",
                "tha", "tpe", "tw",
                "uk", "ukr", "usa"
            };
        }


        //-------------------------------------------------
        //  base implementation for simple filters
        //-------------------------------------------------
        //template <class Base, typename Base::type Type>
        abstract class simple_filter_impl_base_machine_filter : machine_filter  // : Base
        {
            //using Base::config_name;
            //using Base::display_name;

            protected type Type;


            protected simple_filter_impl_base_machine_filter(type Type) : base() { this.Type = Type; }


            public override string config_name() { return config_name(Type); }
            public override string display_name() { return display_name(Type); }
            public override string filter_text() { return null; }

            public override void show_ui(mame_ui_manager mui, render_container container, filter_handler handler)  //, std::function<void (Base &)> &&handler)
            {
                handler(this);
            }

            public override bool wants_adjuster() { return false; }
            public override string adjust_text() { return filter_text(); }
            public override UInt32 arrow_flags() { return 0; }
            public override bool adjust_left() { return false; }
            public override bool adjust_right() { return false; }

            public override void save_ini(emu_file file, UInt32 indent)
            {
                file.puts(string.Format("{0}{1}{2} = 1\n", 2 * indent, "", config_name()).c_str());  // %2$*1$s%3$s = 1\n
            }

            public override type get_type() { return Type; }

            public override string adorned_display_name(type n)
            {
                string result = "";
                if (Type == n)
                {
                    result = "_> ";
                    //throw new emu_unimplemented();
#if false
                    render_font.convert_command_glyph(ref result);
#endif
                }
                result += display_name(n);
                return result;
            }
        }


        // HACK - DUP of above class because C# can't have a base class be a template parameter
        abstract class simple_filter_impl_base_software_filter : software_filter  // : Base
        {
            //using Base::config_name;
            //using Base::display_name;

            protected type Type;


            protected simple_filter_impl_base_software_filter(type Type) : base() { this.Type = Type; }


            public override string config_name() { return config_name(Type); }
            public override string display_name() { return display_name(Type); }
            public override string filter_text() { return null; }

            public override void show_ui(mame_ui_manager mui, render_container container, filter_handler handler)  //, std::function<void (Base &)> &&handler)
            {
                handler(this);
            }

            public override bool wants_adjuster() { return false; }
            public override string adjust_text() { return filter_text(); }
            public override UInt32 arrow_flags() { return 0; }
            public override bool adjust_left() { return false; }
            public override bool adjust_right() { return false; }

            public override void save_ini(emu_file file, UInt32 indent)
            {
                file.puts(string.Format("{0}{1}{2} = 1\n", 2 * indent, "", config_name()).c_str());  // %2$*1$s%3$s = 1\n
            }

            public override type get_type() { return Type; }

            protected override string adorned_display_name(type n)
            {
                string result = "";
                if (Type == n)
                {
                    result = "_> ";
                    //throw new emu_unimplemented();
#if false
                    render_font.convert_command_glyph(ref result);
#endif
                }
                result += display_name(n);
                return result;
            }
        }


        //-------------------------------------------------
        //  base implementation for single-choice filters
        //-------------------------------------------------
        //template <class Base, typename Base::type Type>
        abstract class choice_filter_impl_base_machine_filter : simple_filter_impl_base_machine_filter //<Base, Type>
        {
            std.vector<string> m_choices;
            UInt32 m_selection;


            protected choice_filter_impl_base_machine_filter(std.vector<string> choices, string value, machine_filter.type Type)
                : base(Type)
            {
                m_choices = choices;
                m_selection = 0;


                if (value != null)
                {
                    var found = choices.IndexOf(value);  //List<string> const_iterator_found = std::find(choices.begin(), choices.end(), value);
                    if (-1 != found)
                        m_selection = (UInt32)found;  // std::distance(choices.begin(), found);
                }
            }


            public override string filter_text() { return selection_valid() ? selection_text().c_str() : null; }

            public override void show_ui(mame_ui_manager mui, render_container container, filter_handler handler)  //, std::function<void (Base &)> &&handler)
            {
                if (m_choices.empty())
                {
                    handler(this);
                }
                else
                {
                    throw new emu_unimplemented();
                }
            }

            public override bool wants_adjuster() { return have_choices(); }

            public override UInt32 arrow_flags()
            {
                return (UInt32)(((have_choices() && m_selection != 0) ? menu.FLAG_LEFT_ARROW : 0) | ((m_choices.size() > (m_selection + 1)) ? menu.FLAG_RIGHT_ARROW : 0));
            }

            public override bool adjust_left()
            {
                if (!have_choices() || m_selection == 0)
                    return false;
                m_selection = Math.Min(m_selection - 1, (UInt32)(m_choices.size() - 1));
                return true;
            }

            public override bool adjust_right()
            {
                if (m_choices.size() <= (m_selection + 1))
                    return false;
                ++m_selection;
                return true;
            }

            public override void save_ini(emu_file file, UInt32 indent)
            {
                string text = filter_text();
                file.puts(string.Format("{0}{1}{2} = {3}\n", 2 * indent, "", config_name(), text != null ? text : "").c_str());  // %2$*1$s%3$s = %4$s\n
            }


            protected bool have_choices() { return !m_choices.empty(); }
            protected bool selection_valid() { return m_choices.size() > m_selection; }
            UInt32 selection_index() { return m_selection; }
            protected string selection_text() { return m_choices[(int)m_selection]; }
        }


        // HACK - DUP of above class because C# can't have a base class be a template parameter
        abstract class choice_filter_impl_base_software_filter : simple_filter_impl_base_software_filter //<Base, Type>
        {
            std.vector<string> m_choices;
            UInt32 m_selection;


            protected choice_filter_impl_base_software_filter(std.vector<string> choices, string value, software_filter.type Type)
                : base(Type)
            {
                m_choices = choices;
                m_selection = 0;


                if (value != null)
                {
                    var found = choices.IndexOf(value);  //std::vector<string_> const_iterator_found = std::find(choices.begin(), choices.end(), value);
                    if (-1 != found)
                        m_selection = (UInt32)found;  // std::distance(choices.begin(), found);
                }
            }


            public override string filter_text() { return selection_valid() ? selection_text().c_str() : null; }

            public override void show_ui(mame_ui_manager mui, render_container container, filter_handler handler)  //, std::function<void (Base &)> &&handler)
            {
                if (m_choices.empty())
                {
                    handler(this);
                }
                else
                {
                    throw new emu_unimplemented();
                }
            }

            public override bool wants_adjuster() { return have_choices(); }

            public override UInt32 arrow_flags()
            {
                return (UInt32)(((have_choices() && m_selection != 0) ? menu.FLAG_LEFT_ARROW : 0) | ((m_choices.size() > (m_selection + 1)) ? menu.FLAG_RIGHT_ARROW : 0));
            }

            public override bool adjust_left()
            {
                if (!have_choices() || m_selection == 0)
                    return false;
                m_selection = Math.Min(m_selection - 1, (UInt32)(m_choices.size() - 1));
                return true;
            }

            public override bool adjust_right()
            {
                if (m_choices.size() <= (m_selection + 1))
                    return false;
                ++m_selection;
                return true;
            }

            public override void save_ini(emu_file file, UInt32 indent)
            {
                string text = filter_text();
                file.puts(string.Format("{0}{1}{2} = {3}\n", 2 * indent, "", config_name(), text != null ? text : "").c_str());  // %2$*1$s%3$s = %4$s\n
            }


            protected bool have_choices() { return !m_choices.empty(); }
            protected bool selection_valid() { return m_choices.size() > m_selection; }
            protected UInt32 selection_index() { return m_selection; }
            protected string selection_text() { return m_choices[(int)m_selection]; }
        }


        //-------------------------------------------------
        //  base implementation for composite filters
        //-------------------------------------------------
        //template <class Impl, class Base, typename Base::type Type>
        abstract class composite_filter_impl_base_machine_filter : simple_filter_impl_base_machine_filter  //<Base, Type>
        {
            const int MAX = 8;


            class menu_configure : menu, IDisposable
            {
                enum FILTER //: uintptr_t
                {
                    FILTER_FIRST = 1,
                    FILTER_LAST = FILTER_FIRST + MAX - 1,
                    ADJUST_FIRST,
                    ADJUST_LAST = ADJUST_FIRST + MAX - 1,
                    REMOVE_FILTER,
                    ADD_FILTER
                }


                composite_filter_impl_base_machine_filter m_parent;  // Impl m_parent;
                std.map<type, machine_filter> [] m_saved_filters = new std.map<type, machine_filter>[MAX];  //std::map<typename Base::type, typename Base::ptr> m_saved_filters[MAX];
                filter_handler m_handler;  //std::function<void (Base &)> m_handler;
                bool m_added;


                public menu_configure(
                        mame_ui_manager mui,
                        render_container container,
                        composite_filter_impl_base_machine_filter parent,  // Impl parent
                        filter_handler handler)  //<void (Base &filter)> &&handler)
                    : base(mui, container)
                {
                    m_parent = parent;
                    m_handler = handler;
                    m_added = false;
                }

                ~menu_configure()
                {
                    assert(m_isDisposed);  // can remove
                }

                bool m_isDisposed = false;
                public void Dispose()
                {
                    m_handler(m_parent);
                    m_isDisposed = true;
                }


                protected override void custom_render(object selectedref, float top, float bottom, float x, float y, float x2, float y2)
                {
                    string [] text = { "Select custom filters:" };
                    draw_text_box(
                            text,  //std::begin(text), std::end(text),
                            x, x2, y - top, y - ui().box_tb_border(),
                            text_layout.text_justify.CENTER, text_layout.word_wrapping.NEVER, false,
                            ui().colors().text_color(), UI_GREEN_COLOR, 1.0f);
                }


                protected override void populate(ref float customtop, ref float custombottom)
                {
                    // add items for each active filter
                    UInt32 i = 0;
                    for (i = 0; (MAX > i) && m_parent.m_filters[i] != null; ++i)
                    {
                        item_append(string.Format("Filter {0}", i + 1), m_parent.m_filters[i].display_name(), get_arrow_flags(i), (UInt32)FILTER.FILTER_FIRST + i);  // %1$u
                        if (m_added)
                            set_selected_index(item_count() - 2);

                        if (m_parent.m_filters[i].wants_adjuster())
                        {
                            string name = "^!";
                            render_font.convert_command_glyph(ref name);
                            item_append(name, m_parent.m_filters[i].adjust_text(), m_parent.m_filters[i].arrow_flags(), (UInt32)FILTER.ADJUST_FIRST + i);
                        }

                        item_append(menu_item_type.SEPARATOR);
                    }

                    m_added = false;

                    // add remove/add handlers
                    if (1 < i)
                        item_append("Remove last filter", "", 0, FILTER.REMOVE_FILTER);
                    if (MAX > i)
                        item_append("Add filter", "", 0, FILTER.ADD_FILTER);

                    item_append(menu_item_type.SEPARATOR);

                    // leave space for heading
                    customtop = ui().get_line_height() + 3.0f * ui().box_tb_border();
                }


                protected override void handle()
                {
                    menu_event menu_event = process(PROCESS_LR_REPEAT);
                    if (menu_event != null && menu_event.itemref != null)
                    {
                        m_added = false;
                        bool changed = false;

                        throw new emu_unimplemented();
                    }
                }


                bool set_filter_type(UInt32 pos, machine_filter.type n)
                {
                    if (m_parent.m_filters[pos] == null || (m_parent.m_filters[pos].get_type() == 0))
                    {
                        save_filter(pos);
                        retrieve_filter(pos, n);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }


                bool append_filter()
                {
                    UInt32 pos = 0;
                    while (m_parent.m_filters[pos] != null)
                    {
                        if (MAX <= ++pos)
                            return false;
                    }

                    for (type candidate = type.FIRST; type.COUNT > candidate; ++candidate)
                    {
                        if (m_parent.check_type(pos, candidate))
                        {
                            set_filter_type(pos, candidate);
                            return true;
                        }
                    }

                    return false;
                }


                bool drop_last_filter()
                {
                    for (UInt32 i = 2; MAX >= i; ++i)
                    {
                        if ((MAX <= i) || m_parent.m_filters[i] == null)
                        {
                            save_filter(i - 1);
                            m_parent.m_filters[i - 1] = null;  //.reset();
                            return true;
                        }
                    }

                    return false;
                }


                void save_filter(UInt32 pos)
                {
                    machine_filter flt = m_parent.m_filters[pos];
                    if (flt != null && flt.wants_adjuster())
                        m_saved_filters[pos][flt.get_type()] = flt;
                }


                void retrieve_filter(UInt32 pos, machine_filter.type n)
                {
                    machine_filter flt = m_parent.m_filters[pos];
                    var found = m_saved_filters[pos].find(n);
                    if (null != found)
                    {
                        flt = found;
                        m_saved_filters[pos].erase(n);  //found);
                    }
                    else
                    {
                        throw new emu_unimplemented();
                    }
                }


                uint32_t get_arrow_flags(UInt32 pos)
                {
                    uint32_t result = 0;
                    type current = m_parent.m_filters[pos].get_type();

                    // look for a lower type that's allowed and isn't contradictory
                    type prev = current;
                    while ((type.FIRST < prev) && (FLAG_LEFT_ARROW & result) == 0)
                    {
                        if (m_parent.check_type(pos, --prev))
                            result |= FLAG_LEFT_ARROW;
                    }

                    // look for a higher type that's allowed and isn't contradictory
                    type next = current;
                    while ((type.LAST > next) && (FLAG_RIGHT_ARROW & result) == 0)
                    {
                        if (m_parent.check_type(pos, ++next))
                            result |= FLAG_RIGHT_ARROW;
                    }

                    return result;
                }
            }


            public machine_filter [] m_filters = new machine_filter[MAX];  //typename Base::ptr m_filters[MAX];


            protected composite_filter_impl_base_machine_filter(type Type) : base(Type) { }


            public override void show_ui(mame_ui_manager mui, render_container container, filter_handler handler)  //, std::function<void (Base &)> &&handler) override
            {
                menu.stack_push(new menu_configure(mui, container, this, handler));
            }

            public override bool wants_adjuster() { return true; }
            public override string adjust_text() { return "<set up filters>"; }

            public override void save_ini(emu_file file, UInt32 indent)
            {
                throw new emu_unimplemented();
            }

            public override string adorned_display_name(machine_filter.type n)
            {
                string result = "";
                if (Type == n)
                {
                    result = "_> ";
                    //throw new emu_unimplemented();
#if false
                    render_font.convert_command_glyph(ref result);
#endif
                }
                else
                {
                    for (UInt32 i = 0; (MAX > i) && m_filters[i] != null; ++i)
                    {
                        if (m_filters[i].get_type() == n)
                        {
                            result = string.Format("@custom{0} ", i + 1);
                            render_font.convert_command_glyph(ref result);
                            break;
                        }
                    }
                }

                result += display_name(n);
                return result;
            }

            public override bool apply(ui_system_info info)
            {
                throw new emu_unimplemented();
            }


            protected void populate(string value, emu_file file, UInt32 indent)
            {
                // try to load filters from a file
                if (value != null && file != null)
                {
                    UInt32 cnt = (UInt32)Math.Max(Math.Min((int)MAX, Convert.ToInt32(value)), 0);
                    for (UInt32 i = 0; cnt > i; ++i)
                    {
                        throw new emu_unimplemented();
                    }
                }

                // instantiate first allowed filter type if we're still empty
                for (type t = type.FIRST; (type.COUNT > t) && m_filters[0] == null; ++t)
                {
                    throw new emu_unimplemented();
                }
            }


            bool check_type(UInt32 pos, machine_filter.type candidate)
            {
                throw new emu_unimplemented();
            }
        }


        // HACK - DUP of above class because C# can't have a base class be a template parameter
        abstract class composite_filter_impl_base_software_filter : simple_filter_impl_base_software_filter  //<Base, Type>
        {
            const int MAX = 8;


            class menu_configure : menu, IDisposable
            {
                enum FILTER //: uintptr_t
                {
                    FILTER_FIRST = 1,
                    FILTER_LAST = FILTER_FIRST + MAX - 1,
                    ADJUST_FIRST,
                    ADJUST_LAST = ADJUST_FIRST + MAX - 1,
                    REMOVE_FILTER,
                    ADD_FILTER
                }


                composite_filter_impl_base_software_filter m_parent;  // Impl m_parent;
                //std::map<typename Base::type, typename Base::ptr> m_saved_filters[MAX];
                filter_handler m_handler;  //std::function<void (Base &)> m_handler;
                bool m_added;


                public menu_configure(
                        mame_ui_manager mui,
                        render_container container,
                        composite_filter_impl_base_software_filter parent,  //Impl parent,
                        filter_handler handler)  //<void (Base &filter)> &&handler)
                    : base(mui, container)
                {
                    m_parent = parent;
                    m_handler = handler;
                    m_added = false;
                }

                ~menu_configure()
                {
                    assert(m_isDisposed);  // can remove
                }

                bool m_isDisposed = false;
                public void Dispose()
                {
                    m_handler(m_parent);
                    m_isDisposed = true;
                }


                protected override void custom_render(object selectedref, float top, float bottom, float x, float y, float x2, float y2)
                {
                    string [] text = { "Select custom filters:" };
                    draw_text_box(
                            text,  //std::begin(text), std::end(text),
                            x, x2, y - top, y - ui().box_tb_border(),
                            text_layout.text_justify.CENTER, text_layout.word_wrapping.NEVER, false,
                            ui().colors().text_color(), UI_GREEN_COLOR, 1.0f);
                }


                protected override void populate(ref float customtop, ref float custombottom)
                {
                    // add items for each active filter
                    UInt32 i = 0;
                    for (i = 0; (MAX > i) && m_parent.m_filters[i] != null; ++i)
                    {
                        item_append(string.Format("Filter {0}", i + 1), m_parent.m_filters[i].display_name(), get_arrow_flags(i), (UInt32)FILTER.FILTER_FIRST + i);  // %1$u
                        if (m_added)
                            set_selected_index(item_count() - 2);

                        if (m_parent.m_filters[i].wants_adjuster())
                        {
                            string name = "^!";
                            render_font.convert_command_glyph(ref name);
                            item_append(name, m_parent.m_filters[i].adjust_text(), m_parent.m_filters[i].arrow_flags(), (UInt32)FILTER.ADJUST_FIRST + i);
                        }

                        item_append(menu_item_type.SEPARATOR);
                    }

                    m_added = false;

                    // add remove/add handlers
                    if (1 < i)
                        item_append("Remove last filter", "", 0, FILTER.REMOVE_FILTER);
                    if (MAX > i)
                        item_append("Add filter", "", 0, FILTER.ADD_FILTER);

                    item_append(menu_item_type.SEPARATOR);

                    // leave space for heading
                    customtop = ui().get_line_height() + 3.0f * ui().box_tb_border();
                }


                protected override void handle()
                {
                    menu_event menu_event = process(PROCESS_LR_REPEAT);
                    if (menu_event != null && menu_event.itemref != null)
                    {
                        m_added = false;
                        bool changed = false;

                        throw new emu_unimplemented();
                    }
                }


                bool set_filter_type(UInt32 pos, software_filter.type n)
                {
                    if (m_parent.m_filters[pos] == null || (m_parent.m_filters[pos].get_type() != 0))
                    {
                        save_filter(pos);
                        retrieve_filter(pos, n);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }


                bool append_filter()
                {
                    UInt32 pos = 0;
                    while (m_parent.m_filters[pos] != null)
                    {
                        if (MAX <= ++pos)
                            return false;
                    }
                    for (type candidate = type.FIRST; type.COUNT > candidate; ++candidate)
                    {
                        if (m_parent.check_type(pos, candidate))
                        {
                            set_filter_type(pos, candidate);
                            return true;
                        }
                    }
                    return false;
                }


                bool drop_last_filter()
                {
                    for (UInt32 i = 2; MAX >= i; ++i)
                    {
                        if ((MAX <= i) || m_parent.m_filters[i] == null)
                        {
                            save_filter(i - 1);
                            m_parent.m_filters[i - 1] = null;  //.reset();
                            return true;
                        }
                    }
                    return false;
                }


                void save_filter(UInt32 pos)
                {
                    throw new emu_unimplemented();
                }


                void retrieve_filter(UInt32 pos, software_filter.type n)
                {
                    throw new emu_unimplemented();
                }


                uint32_t get_arrow_flags(UInt32 pos)
                {
                    uint32_t result = 0;
                    type current = m_parent.m_filters[pos].get_type();

                    // look for a lower type that's allowed and isn't contradictory
                    type prev = current;
                    while ((type.FIRST < prev) && (FLAG_LEFT_ARROW & result) == 0)
                    {
                        if (m_parent.check_type(pos, --prev))
                            result |= FLAG_LEFT_ARROW;
                    }

                    // look for a higher type that's allowed and isn't contradictory
                    type next = current;
                    while ((type.LAST > next) && (FLAG_RIGHT_ARROW & result) == 0)
                    {
                        if (m_parent.check_type(pos, ++next))
                            result |= FLAG_RIGHT_ARROW;
                    }

                    return result;
                }
            }


            software_filter [] m_filters = new software_filter[MAX];  //typename Base::ptr m_filters[MAX];


            protected composite_filter_impl_base_software_filter(software_filter.type Type) : base(Type) { }


            public override void show_ui(mame_ui_manager mui, render_container container, filter_handler handler)  //, std::function<void (Base &)> &&handler) override
            {
                menu.stack_push(new menu_configure(mui, container, this, handler));
            }

            public override bool wants_adjuster() { return true; }
            public override string adjust_text() { return "<set up filters>"; }

            public override void save_ini(emu_file file, UInt32 indent)
            {
                throw new emu_unimplemented();
            }

            protected override string adorned_display_name(software_filter.type n)
            {
                string result = "";
                if (Type == n)
                {
                    result = "_> ";
                    //throw new emu_unimplemented();
#if false
                    render_font.convert_command_glyph(ref result);
#endif
                }
                else
                {
                    for (UInt32 i = 0; (MAX > i) && m_filters[i] != null; ++i)
                    {
                        if (m_filters[i].get_type() == n)
                        {
                            result = string.Format("@custom{0} ", i + 1);
                            render_font.convert_command_glyph(ref result);
                            break;
                        }
                    }
                }
                result += display_name(n);
                return result;
            }

            public override bool apply(ui_software_info info)
            {
                throw new emu_unimplemented();
            }


            protected void populate(string value, emu_file file, UInt32 indent)
            {
                // try to load filters from a file
                if (value != null && file != null)
                {
                    UInt32 cnt = (UInt32)Math.Max(Math.Min((int)(MAX), Convert.ToInt32(value)), 0);
                    for (UInt32 i = 0; cnt > i; ++i)
                    {
                        throw new emu_unimplemented();
                    }
                }

                // instantiate first allowed filter type if we're still empty
                for (type t = type.FIRST; (type.COUNT > t) && m_filters[0] == null; ++t)
                {
                    throw new emu_unimplemented();
                }
            }


            bool check_type(UInt32 pos, software_filter.type candidate)
            {
                throw new emu_unimplemented();
            }
        }


        //-------------------------------------------------
        //  invertable machine filters
        //-------------------------------------------------
        //template <machine_filter::type Type = machine_filter::AVAILABLE>
        class available_machine_filter_impl : simple_filter_impl_base_machine_filter
        {
            protected available_machine_filter_impl(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.AVAILABLE) : base(Type) { }

            public override bool apply(ui_system_info system) { return system.available; }
        }


        //template <machine_filter::type Type = machine_filter::WORKING>
        class working_machine_filter_impl : simple_filter_impl_base_machine_filter
        {
            protected working_machine_filter_impl(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.WORKING) : base(Type) { }

            public override bool apply(ui_system_info system) { return (system.driver.flags & machine_flags.type.NOT_WORKING) == 0; }
        }


        //template <machine_filter::type Type = machine_filter::MECHANICAL>
        class mechanical_machine_filter_impl : simple_filter_impl_base_machine_filter
        {
            protected mechanical_machine_filter_impl(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.MECHANICAL) : base(Type) { }

            public override bool apply(ui_system_info system) { return (system.driver.flags & machine_flags.type.MECHANICAL) != 0; }
        }


        //template <machine_filter::type Type = machine_filter::BIOS>
        class bios_machine_filter_impl : simple_filter_impl_base_machine_filter
        {
            protected bios_machine_filter_impl(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.BIOS) : base(Type) { }

            public override bool apply(ui_system_info system) { return (system.driver.flags & machine_flags.type.IS_BIOS_ROOT) != 0; }
        }


        //template <machine_filter::type Type = machine_filter::PARENTS>
        class parents_machine_filter_impl : simple_filter_impl_base_machine_filter
        {
            protected parents_machine_filter_impl(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.PARENTS) : base(Type) { }

            public override bool apply(ui_system_info system)
            {
                bool have_parent = strcmp(system.driver.parent, "0") != 0;
                var parent_idx = have_parent ? driver_list.find(system.driver.parent) : -1;
                return !have_parent || (0 > parent_idx) || (driver_list.driver(parent_idx).flags & machine_flags.type.IS_BIOS_ROOT) != 0;
            }
        }


        //template <machine_filter::type Type = machine_filter::CHD>
        class chd_machine_filter_impl : simple_filter_impl_base_machine_filter
        {
            protected chd_machine_filter_impl(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.CHD) : base(Type) { }

            public override bool apply(ui_system_info system)
            {
                Pointer<tiny_rom_entry> rom = system.driver.rom;
                for (int romOffset = 0; !romload_global.ROMENTRY_ISEND(rom[romOffset]); ++romOffset)
                {
                    if (romload_global.ROMENTRY_ISREGION(rom[romOffset]) && romload_global.ROMREGION_ISDISKDATA(rom[romOffset]))
                        return true;
                }

                return false;
            }
        }


        //template <machine_filter::type Type = machine_filter::SAVE>
        class save_machine_filter_impl : simple_filter_impl_base_machine_filter
        {
            protected save_machine_filter_impl(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.SAVE) : base(Type) { }

            public override bool apply(ui_system_info system) { return (system.driver.flags & machine_flags.type.SUPPORTS_SAVE) != 0; }
        }


        //template <machine_filter::type Type = machine_filter::VERTICAL>
        class vertical_machine_filter_impl : simple_filter_impl_base_machine_filter
        {
            protected vertical_machine_filter_impl(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.VERTICAL) : base(Type) { }

            public override bool apply(ui_system_info system) { return (system.driver.flags & machine_flags.type.SWAP_XY) != 0; }
        }


        //-------------------------------------------------
        //  concrete machine filters
        //-------------------------------------------------
        class manufacturer_machine_filter : choice_filter_impl_base_machine_filter // <machine_filter, machine_filter.MANUFACTURER>
        {
            public manufacturer_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent)
                : base(c_mnfct.ui, value, type.MANUFACTURER)
            {
            }

            public override bool apply(ui_system_info system)
            {
                if (!have_choices())
                    return true;
                else if (!selection_valid())
                    return false;

                string name = machine_filter_data.extract_manufacturer(system.driver.manufacturer);
                return !name.empty() && (selection_text() == name);
            }
        }


        class year_machine_filter : choice_filter_impl_base_machine_filter  //<machine_filter, machine_filter::YEAR>
        {
            public year_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent)
                : base(data.years(), value, type.YEAR)
            {
            }

            public override bool apply(ui_system_info system) { return !have_choices() || (selection_valid() && (selection_text() == system.driver.year)); }
        }


        //-------------------------------------------------
        //  complementary machine filters
        //-------------------------------------------------
        //template <template <machine_filter::type T> class Base, machine_filter::type Type>
        class inverted_machine_filter_available_machine : available_machine_filter_impl  // Base<Type>
        {
            protected inverted_machine_filter_available_machine(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type) : base(data, value, file, indent, Type) { }
            public override bool apply(ui_system_info system) { return !base.apply(system); }
        }

        class inverted_machine_filter_working_machine : working_machine_filter_impl  // Base<Type>
        {
            protected inverted_machine_filter_working_machine(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type) : base(data, value, file, indent, Type) { }
            public override bool apply(ui_system_info system) { return !base.apply(system); }
        }

        class inverted_machine_filter_mechanical_machine : mechanical_machine_filter_impl  // Base<Type>
        {
            protected inverted_machine_filter_mechanical_machine(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type) : base(data, value, file, indent, Type) { }
            public override bool apply(ui_system_info system) { return !base.apply(system); }
        }

        class inverted_machine_filter_bios_machine : bios_machine_filter_impl  // Base<Type>
        {
            protected inverted_machine_filter_bios_machine(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type) : base(data, value, file, indent, Type) { }
            public override bool apply(ui_system_info system) { return !base.apply(system); }
        }

        class inverted_machine_filter_parents_machine : parents_machine_filter_impl  // Base<Type>
        {
            protected inverted_machine_filter_parents_machine(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type) : base(data, value, file, indent, Type) { }
            public override bool apply(ui_system_info system) { return !base.apply(system); }
        }

        class inverted_machine_filter_save_machine : save_machine_filter_impl  // Base<Type>
        {
            protected inverted_machine_filter_save_machine(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type) : base(data, value, file, indent, Type) { }
            public override bool apply(ui_system_info system) { return !base.apply(system); }
        }

        class inverted_machine_filter_chd_machine : chd_machine_filter_impl  // Base<Type>
        {
            protected inverted_machine_filter_chd_machine(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type) : base(data, value, file, indent, Type) { }
            public override bool apply(ui_system_info system) { return !base.apply(system); }
        }

        class inverted_machine_filter_vertical_machine : vertical_machine_filter_impl  // Base<Type>
        {
            protected inverted_machine_filter_vertical_machine(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type) : base(data, value, file, indent, Type) { }
            public override bool apply(ui_system_info system) { return !base.apply(system); }
        }


        //using available_machine_filter      = available_machine_filter_impl<>;
        //using working_machine_filter        = working_machine_filter_impl<>;
        //using mechanical_machine_filter     = mechanical_machine_filter_impl<>;
        //using bios_machine_filter           = bios_machine_filter_impl<>;
        //using parents_machine_filter        = parents_machine_filter_impl<>;
        //using save_machine_filter           = save_machine_filter_impl<>;
        //using chd_machine_filter            = chd_machine_filter_impl<>;
        //using vertical_machine_filter       = vertical_machine_filter_impl<>;

        class available_machine_filter : available_machine_filter_impl { public available_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.AVAILABLE) : base(data, value, file, indent, Type) { } }
        class working_machine_filter : working_machine_filter_impl { public working_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.AVAILABLE) : base(data, value, file, indent, Type) { } }
        class mechanical_machine_filter : mechanical_machine_filter_impl { public mechanical_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.AVAILABLE) : base(data, value, file, indent, Type) { } }
        class bios_machine_filter : bios_machine_filter_impl { public bios_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.AVAILABLE) : base(data, value, file, indent, Type) { } }
        class parents_machine_filter : parents_machine_filter_impl { public parents_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.AVAILABLE) : base(data, value, file, indent, Type) { } }
        class save_machine_filter : save_machine_filter_impl { public save_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.AVAILABLE) : base(data, value, file, indent, Type) { } }
        class chd_machine_filter : chd_machine_filter_impl { public chd_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.AVAILABLE) : base(data, value, file, indent, Type) { } }
        class vertical_machine_filter : vertical_machine_filter_impl { public vertical_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type = type.AVAILABLE) : base(data, value, file, indent, Type) { } }

        //using unavailable_machine_filter    = inverted_machine_filter<available_machine_filter_impl, machine_filter::UNAVAILABLE>;
        //using not_working_machine_filter    = inverted_machine_filter<working_machine_filter_impl, machine_filter::NOT_WORKING>;
        //using not_mechanical_machine_filter = inverted_machine_filter<mechanical_machine_filter_impl, machine_filter::NOT_MECHANICAL>;
        //using not_bios_machine_filter       = inverted_machine_filter<bios_machine_filter_impl, machine_filter::NOT_BIOS>;
        //using clones_machine_filter         = inverted_machine_filter<parents_machine_filter_impl, machine_filter::CLONES>;
        //using nosave_machine_filter         = inverted_machine_filter<save_machine_filter_impl, machine_filter::NOSAVE>;
        //using nochd_machine_filter          = inverted_machine_filter<chd_machine_filter_impl, machine_filter::NOCHD>;
        //using horizontal_machine_filter     = inverted_machine_filter<vertical_machine_filter_impl, machine_filter::HORIZONTAL>;

        class unavailable_machine_filter : inverted_machine_filter_available_machine { public unavailable_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent) : base(data, value, file, indent, type.UNAVAILABLE) { } }
        class not_working_machine_filter : inverted_machine_filter_working_machine { public not_working_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent) : base(data, value, file, indent, type.NOT_WORKING) { } }
        class not_mechanical_machine_filter : inverted_machine_filter_mechanical_machine { public not_mechanical_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent) : base(data, value, file, indent, type.NOT_MECHANICAL) { } }
        class not_bios_machine_filter : inverted_machine_filter_bios_machine { public not_bios_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent) : base(data, value, file, indent, type.NOT_BIOS) { } }
        class clones_machine_filter : inverted_machine_filter_parents_machine { public clones_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent) : base(data, value, file, indent, type.CLONES) { } }
        class nosave_machine_filter : inverted_machine_filter_save_machine { public nosave_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent) : base(data, value, file, indent, type.NOSAVE) { } }
        class nochd_machine_filter : inverted_machine_filter_chd_machine { public nochd_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent) : base(data, value, file, indent, type.NOCHD) { } }
        class horizontal_machine_filter : inverted_machine_filter_vertical_machine { public horizontal_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent) : base(data, value, file, indent, type.HORIZONTAL) { } }


        //-------------------------------------------------
        //  dummy machine filters (special-cased in menu)
        //-------------------------------------------------
        //template <machine_filter::type Type>
        class inclusive_machine_filter_impl : simple_filter_impl_base_machine_filter  //<machine_filter, Type>
        {
            protected inclusive_machine_filter_impl(machine_filter_data data, string value, emu_file file, UInt32 indent, type Type) : base(Type) { }

            public override bool apply(ui_system_info system) { return true; }
        }


        //using all_machine_filter            = inclusive_machine_filter_impl<machine_filter::ALL>;
        //using favorite_machine_filter       = inclusive_machine_filter_impl<machine_filter::FAVORITE>;
        class all_machine_filter : inclusive_machine_filter_impl { public all_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent) : base(data, value, file, indent, type.ALL) { } }
        class favorite_machine_filter : inclusive_machine_filter_impl { public favorite_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent) : base(data, value, file, indent, type.FAVORITE) { } }


        //-------------------------------------------------
        //  category machine filter
        //-------------------------------------------------
        class category_machine_filter : simple_filter_impl_base_machine_filter  //<machine_filter, machine_filter::CATEGORY>
        {
            class menu_configure : menu
            {
                enum configure //: uintptr_t
                {
                    INI_FILE = 1,
                    SYSTEM_GROUP,
                    INCLUDE_CLONES
                }


                category_machine_filter m_parent;
                filter_handler m_handler;  //std::function<void (machine_filter &)> m_handler;
                std.pair<UInt32, bool> [] m_state;  //std::unique_ptr<std::pair<unsigned, bool> []> const m_state;
                UInt32 m_ini;


                public menu_configure(
                        mame_ui_manager mui,
                        render_container container,
                        category_machine_filter parent,
                        filter_handler handler)  //std::function<void (machine_filter &filter)> &&handler)
                    : base(mui, container)
                {
                    m_parent = parent;
                    m_handler = handler;
                    m_state = new std.pair<UInt32, bool>[mame_machine_manager.instance().inifile().get_file_count()];  //m_state(std::make_unique<std::pair<UInt32, bool> []>(mame_machine_manager.instance().inifile().get_file_count()));
                    m_ini = parent.m_ini;


                    inifile_manager mgr = mame_machine_manager.instance().inifile();
                    for (UInt32 i = 0; mgr.get_file_count() > i; ++i)
                    {
                        //m_state[i].first = (m_ini == i) ? m_parent.m_group : 0U;
                        //m_state[i].second = (m_ini == i) ? m_parent.m_include_clones : include_clones_default(mgr.get_file_name(i));
                        m_state[i] = new std.pair<char32_t, bool>((m_ini == i) ? m_parent.m_group : 0U, (m_ini == i) ? m_parent.m_include_clones : include_clones_default(mgr.get_file_name(i)));
                    }
                }


                ~menu_configure()
                {
                }


                void Dispose()
                {
                    throw new emu_unimplemented();
                }


                protected override void custom_render(object selectedref, float top, float bottom, float x, float y, float x2, float y2)
                {
                    string [] text = { "Select category:" };
                    draw_text_box(
                            text,  //std::begin(text), std::end(text),
                            x, x2, y - top, y - ui().box_tb_border(),
                            text_layout.text_justify.CENTER, text_layout.word_wrapping.NEVER, false,
                            ui().colors().text_color(), UI_GREEN_COLOR, 1.0f);
                }


                protected override void populate(ref float customtop, ref float custombottom)
                {
                    inifile_manager mgr = mame_machine_manager.instance().inifile();
                    UInt32 filecnt = mgr.get_file_count();
                    if (filecnt == 0)
                    {
                        item_append("No category INI files found", "", FLAG_DISABLE, null);
                    }
                    else
                    {
                        throw new emu_unimplemented();
                    }

                    item_append(menu_item_type.SEPARATOR);
                    customtop = ui().get_line_height() + 3.0f * ui().box_tb_border();
                }


                protected override void handle()
                {
                    menu_event menu_event = process(PROCESS_LR_REPEAT);
                    if (menu_event != null && menu_event.itemref != null)
                    {
                        bool changed = false;

                        throw new emu_unimplemented();
                    }
                }
            }


            UInt32 m_ini;
            UInt32 m_group;
            bool m_include_clones;
            string m_adjust_text;
            std.unordered_set<game_driver> m_cache;  //mutable std::unordered_set<game_driver const *> m_cache;
            bool m_cache_valid;  //mutable bool m_cache_valid;


            public category_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent)
                : base(type.CATEGORY)
            {
                m_ini = 0;
                m_group = 0;
                m_include_clones = false;
                m_adjust_text = "";
                m_cache = new std.unordered_set<game_driver>();
                m_cache_valid = false;


                inifile_manager mgr = mame_machine_manager.instance().inifile();
                if (value != null)
                {
                    //string split = std::strchr(value, '/');
                    int splitIdx = value.IndexOf("/");
                    string ini;
                    if (splitIdx != -1)
                        ini = value.Substring(splitIdx);  // assign(value, split);
                    else
                        ini = value;

                    for (UInt32 i = 0; mgr.get_file_count() > i; ++i)
                    {
                        if (mgr.get_file_name(i) == ini)
                        {
                            m_ini = i;
                            if (splitIdx != -1)
                            {
                                string group = value.Substring(splitIdx + 1);  // split + 1;
                                for (UInt32 j = 0; mgr.get_category_count(i) > j; ++j)
                                {
                                    if (mgr.get_category_name(i, j) == group)
                                    {
                                        m_group = j;
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }

                if (mgr.get_file_count() > m_ini)
                    m_include_clones = include_clones_default(mgr.get_file_name(m_ini));

                set_adjust_text();
            }

            public override string filter_text()
            {
                inifile_manager mgr = mame_machine_manager.instance().inifile();
                return ((mgr.get_file_count() > m_ini) && (mgr.get_category_count(m_ini) > m_group)) ? m_adjust_text.c_str() : null;
            }

            public override void show_ui(mame_ui_manager mui, render_container container, filter_handler handler)  //, std::function<void (machine_filter &)> &&handler) override;
            {
                menu.stack_push(new menu_configure(mui, container, this, handler));
            }

            public override bool wants_adjuster() { return mame_machine_manager.instance().inifile().get_file_count() != 0; }
            public override string adjust_text() { return m_adjust_text.c_str(); }

            public override void save_ini(emu_file file, UInt32 indent)
            {
                string text = filter_text();
                file.puts(string.Format("{0}{1}{2} = {3}\n", 2 * indent, "", config_name(), text != null ? text : "").c_str());  // %2$*1$s%3$s = %4$s\n
            }

            public override bool apply(ui_system_info system)
            {
                inifile_manager mgr = mame_machine_manager.instance().inifile();
                if (mgr.get_file_count() == 0)
                    return true;
                else if ((mgr.get_file_count() <= m_ini) || (mgr.get_category_count(m_ini) <= m_group))
                    return false;

                throw new emu_unimplemented();
            }


            void set_adjust_text()
            {
                inifile_manager mgr = mame_machine_manager.instance().inifile();
                UInt32 filecnt = mgr.get_file_count();
                if (filecnt == 0)
                {
                    m_adjust_text = "[no category INI files]";
                }
                else
                {
                    m_ini = Math.Min(m_ini, filecnt - 1);
                    UInt32 groupcnt = mgr.get_category_count(m_ini);
                    if (groupcnt == 0)
                    {
                        m_adjust_text = "[no groups in INI file]";
                    }
                    else
                    {
                        m_group = Math.Min(m_group, groupcnt - 1);
                        m_adjust_text = string.Format("{0}/{1}", mgr.get_file_name(m_ini), mgr.get_category_name(m_ini, m_group));
                    }
                }
            }

            static bool include_clones_default(string name)
            {
                return core_stricmp(name.c_str(), "category.ini") == 0 || core_stricmp(name.c_str(), "alltime.ini") == 0;
            }
        }


        //-------------------------------------------------
        //  composite machine filter
        //-------------------------------------------------
        class custom_machine_filter : composite_filter_impl_base_machine_filter  //<custom_machine_filter, machine_filter, machine_filter::CUSTOM>
        {
            machine_filter_data m_data;


            public custom_machine_filter(machine_filter_data data, string value, emu_file file, UInt32 indent)
                : base(type.CUSTOM)
            {
                m_data = data;


                populate(value, file, indent);
            }

            machine_filter create(type n) { return machine_filter.create(n, m_data); }
            machine_filter create(emu_file file, UInt32 indent) { return machine_filter.create(file, m_data, indent); }

            static bool type_allowed(UInt32 pos, type n)
            {
                return (type.FIRST <= n) && (type.LAST >= n) && (type.ALL != n) && (type.FAVORITE != n) && (type.CUSTOM != n);
            }

            static bool types_contradictory(type n, type m)
            {
                switch (n)
                {
                case type.AVAILABLE:         return type.UNAVAILABLE == m;
                case type.UNAVAILABLE:       return type.AVAILABLE == m;
                case type.WORKING:           return type.NOT_WORKING == m;
                case type.NOT_WORKING:       return type.WORKING == m;
                case type.MECHANICAL:        return type.NOT_MECHANICAL == m;
                case type.NOT_MECHANICAL:    return type.MECHANICAL == m;
                case type.BIOS:              return type.NOT_BIOS == m;
                case type.NOT_BIOS:          return type.BIOS == m;
                case type.PARENTS:           return type.CLONES == m;
                case type.CLONES:            return type.PARENTS == m;
                case type.SAVE:              return type.NOSAVE == m;
                case type.NOSAVE:            return type.SAVE == m;
                case type.CHD:               return type.NOCHD == m;
                case type.NOCHD:             return type.CHD == m;
                case type.VERTICAL:          return type.HORIZONTAL == m;
                case type.HORIZONTAL:        return type.VERTICAL == m;

                case type.ALL:
                case type.CATEGORY:
                case type.FAVORITE:
                case type.MANUFACTURER:
                case type.YEAR:
                case type.CUSTOM:
                case type.COUNT:
                    break;
                }

                return false;
            }

            static bool is_inclusion(type n)
            {
                return (type.CATEGORY == n) || (type.MANUFACTURER == n) || (type.YEAR == n);
            }
        }


        //-------------------------------------------------
        //  concrete software filters
        //-------------------------------------------------
        class all_software_filter : simple_filter_impl_base_software_filter  //<software_filter, software_filter::ALL>
        {
            public all_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent) : base(type.ALL) { }

            public override bool apply(ui_software_info info) { return true; }
        }


        class available_software_filter : simple_filter_impl_base_software_filter  //<software_filter, software_filter::AVAILABLE>
        {
            public available_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent) : base(type.AVAILABLE) { }

            public override bool apply(ui_software_info info) { return info.available; }
        }


        class unavailable_software_filter : simple_filter_impl_base_software_filter  //<software_filter, software_filter::UNAVAILABLE>
        {
            public unavailable_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent) : base(type.UNAVAILABLE) { }

            public override bool apply(ui_software_info info) { return !info.available; }
        }


        class favorite_software_filter : simple_filter_impl_base_software_filter
        {
            favorite_manager m_manager;

            public favorite_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent) : base(type.FAVORITE) { m_manager = mame_machine_manager.instance().favorite(); }

            public override bool apply(ui_software_info info) { return m_manager.is_favorite_software(info); }
        }


        class parents_software_filter : simple_filter_impl_base_software_filter  //<software_filter, software_filter::PARENTS>
        {
            public parents_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent) : base(type.PARENTS) { }

            public override bool apply(ui_software_info info) { return info.parentname.empty(); }
        }


        class clones_software_filter : simple_filter_impl_base_software_filter  //<software_filter, software_filter::CLONES>
        {
            public clones_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent) : base(type.CLONES) { }

            public override bool apply(ui_software_info info) { return !info.parentname.empty(); }
        }


        class years_software_filter : choice_filter_impl_base_software_filter  //<software_filter, software_filter::YEAR>
        {
            public years_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent)
                : base(data.years(), value, type.YEAR)
            {
            }

            public override bool apply(ui_software_info info) { return !have_choices() || (selection_valid() && (selection_text() == info.year)); }
        }


        class publishers_software_filter : choice_filter_impl_base_software_filter  //<software_filter, software_filter::PUBLISHERS>
        {
            public publishers_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent)
                : base(data.publishers(), value, type.PUBLISHERS)
            {
            }

            public override bool apply(ui_software_info info)
            {
                if (!have_choices())
                    return true;
                else if (!selection_valid())
                    return false;

                string name = software_filter_data.extract_publisher(info.publisher);
                return !name.empty() && (selection_text() == name);
            }
        }


        class supported_software_filter : simple_filter_impl_base_software_filter  //<software_filter, software_filter::SUPPORTED>
        {
            public supported_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent) : base(type.SUPPORTED) { }

            public override bool apply(ui_software_info info) { return softlist_global.SOFTWARE_SUPPORTED_YES == info.supported; }
        }


        class partial_supported_software_filter : simple_filter_impl_base_software_filter  //<software_filter, software_filter::PARTIAL_SUPPORTED>
        {
            public partial_supported_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent) : base(type.PARTIAL_SUPPORTED) { }

            public override bool apply(ui_software_info info) { return softlist_global.SOFTWARE_SUPPORTED_PARTIAL == info.supported; }
        }


        class unsupported_software_filter : simple_filter_impl_base_software_filter  //<software_filter, software_filter::UNSUPPORTED>
        {
            public unsupported_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent) : base(type.UNSUPPORTED) { }

            public override bool apply(ui_software_info info) { return softlist_global.SOFTWARE_SUPPORTED_NO == info.supported; }
        }


        class region_software_filter : choice_filter_impl_base_software_filter  //<software_filter, software_filter::REGION>
        {
            public region_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent)
                : base(data.regions(), value, type.REGION)
            {
            }

            public override bool apply(ui_software_info info)
            {
                if (!have_choices())
                    return true;
                else if (!selection_valid())
                    return false;

                string name = software_filter_data.extract_region(info.longname);
                return !name.empty() && (selection_text() == name);
            }
        }


        class device_type_software_filter : choice_filter_impl_base_software_filter  //<software_filter, software_filter::DEVICE_TYPE>
        {
            public device_type_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent)
                : base(data.device_types(), value, type.DEVICE_TYPE)
            {
            }

            public override bool apply(ui_software_info info) { return !have_choices() || (selection_valid() && (selection_text() == info.devicetype)); }
        }


        class list_software_filter : choice_filter_impl_base_software_filter  //<software_filter, software_filter::LIST>
        {
            software_filter_data m_data;


            public list_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent)
                : base(data.list_descriptions(), value, type.LIST)
            {
                m_data = data;
            }

            public override bool apply(ui_software_info info)
            {
                return !have_choices() || (selection_valid() && (m_data.list_names()[(int)selection_index()] == info.listname));
            }
        }


        //-------------------------------------------------
        //  composite software filter
        //-------------------------------------------------
        class custom_software_filter : composite_filter_impl_base_software_filter  //<custom_software_filter, software_filter, software_filter::CUSTOM>
        {
            software_filter_data m_data;


            public custom_software_filter(software_filter_data data, string value, emu_file file, UInt32 indent)
                : base(type.CUSTOM)
            {
                m_data = data;


                populate(value, file, indent);
            }

            software_filter create(type n) { return software_filter.create(n, m_data); }
            software_filter create(emu_file file, UInt32 indent) { return software_filter.create(file, m_data, indent); }

            static bool type_allowed(UInt32 pos, type n)
            {
                return (type.FIRST <= n) && (type.LAST >= n) && (type.ALL != n) && (type.CUSTOM != n);
            }

            static bool types_contradictory(type n, type m)
            {
                switch (n)
                {
                case type.AVAILABLE:         return type.UNAVAILABLE == m;
                case type.UNAVAILABLE:       return type.AVAILABLE == m;
                case type.PARENTS:           return type.CLONES == m;
                case type.CLONES:            return type.PARENTS == m;
                case type.SUPPORTED:         return (type.PARTIAL_SUPPORTED == m) || (type.UNSUPPORTED == m);
                case type.PARTIAL_SUPPORTED: return (type.SUPPORTED == m) || (type.UNSUPPORTED == m);
                case type.UNSUPPORTED:       return (type.SUPPORTED == m) || (type.PARTIAL_SUPPORTED == m);

                case type.ALL:
                case type.FAVORITE:
                case type.YEAR:
                case type.PUBLISHERS:
                case type.REGION:
                case type.DEVICE_TYPE:
                case type.LIST:
                case type.CUSTOM:
                case type.COUNT:
                    break;
                }

                return false;
            }

            static bool is_inclusion(type n)
            {
                return (type.YEAR == n) || (type.PUBLISHERS == n) || (type.REGION == n) || (type.DEVICE_TYPE == n) || (type.LIST == n);
            }
        }
    }
}
