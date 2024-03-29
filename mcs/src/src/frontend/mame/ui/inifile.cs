// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using categoryindex = mame.std.vector<mame.std.pair<string, System.Int64>>;
using favorites_set = mame.std.set<mame.ui_software_info>;//, favorite_compare>;
using int64_t = System.Int64;
using size_t = System.UInt64;
using sorted_favorites = mame.std.vector<mame.ui_software_info>;
using uint8_t = System.Byte;
using uint64_t = System.UInt64;

using static mame.corestr_global;
using static mame.cpp_global;
using static mame.language_global;
using static mame.osdcore_global;
using static mame.osdfile_global;
using static mame.util;
using static mame.utils_global;


namespace mame
{
    //-------------------------------------------------
    //  INIFILE MANAGER
    //-------------------------------------------------

    public class inifile_manager
    {
        // ini file structure
        //using categoryindex = std::vector<std::pair<std::string, int64_t>>;

        // files indices
        static UInt16 c_file;
        static UInt16 c_cat;

        // internal state
        ui_options m_options;
        std.vector<std.pair<string, categoryindex>> m_ini_index;  //std::vector<std::pair<std::string, categoryindex> > m_ini_index;


        // construction/destruction
        //-------------------------------------------------
        //  ctor
        //-------------------------------------------------
        public inifile_manager(ui_options moptions)
        {
            m_options = moptions;
            m_ini_index = new std.vector<std.pair<string, categoryindex>>();


            // scan directories and create index
            file_enumerator path = new file_enumerator(m_options.categoryini_path());
            for (osd.directory.entry dir = path.next(); dir != null; dir = path.next())
            {
                string name = dir.name;
                if (core_filename_ends_with(name, ".ini"))
                {
                    emu_file file = new emu_file(m_options.categoryini_path(), OPEN_FLAG_READ);
                    if (!file.open(name))
                    {
                        init_category(name, file.core_file_);
                        file.close();
                    }
                }
            }

            //std::collate<wchar_t> const &coll = std::use_facet<std::collate<wchar_t>>(std::locale());
            //std::stable_sort(
            //        m_ini_index.begin(),
            //        m_ini_index.end(),
            //        [&coll] (auto const &x, auto const &y)
            //        {
            //            std::wstring const wx = wstring_from_utf8(x.first);
            //            std::wstring const wy = wstring_from_utf8(y.first);
            //            return 0 > coll.compare(wx.data(), wx.data() + wx.size(), wy.data(), wy.data() + wy.size());
            //        }
            //);
            m_ini_index.Sort((x, y) => { return core_stricmp(x.first, y.first); });
        }


        // load games from category
        public void load_ini_category(size_t file, size_t category, std.unordered_set<game_driver> result)
        {
            string filename = m_ini_index[(int)file].first;
            emu_file fp = new emu_file(m_options.categoryini_path(), OPEN_FLAG_READ);
            if (fp.open(filename))
            {
                osd_printf_error("Failed to open category file {0} for reading\n", filename);
                return;
            }

            int64_t offset = m_ini_index[(int)file].second[(int)category].second;
            if (fp.seek(offset, SEEK_SET) != 0 || (fp.tell() != (uint64_t)offset))
            {
                fp.close();
                osd_printf_error("Failed to seek to category offset in file {0}\n", filename);
                return;
            }

            string rbuf;  // char rbuf[MAX_CHAR_INFO];
            while (fp.gets(out rbuf, MAX_CHAR_INFO) != null && !string.IsNullOrEmpty(rbuf) && ('[' != rbuf[0]))
            {
                //var tail = std::find_if(std::begin(rbuf), std::prev(std::end(rbuf)));//, [] (char ch) { return !ch || ('\r' == ch) || ('\n' == ch); });
                //*tail = '\0';
                var tail = rbuf.IndexOfAny("\r\n".ToCharArray());
                rbuf = rbuf[tail..];
                int dfind = driver_list.find(rbuf);
                if (0 <= dfind)
                    result.emplace(driver_list.driver((size_t)dfind));
            }

            fp.close();
        }


        // getters
        public size_t get_file_count() { return m_ini_index.size(); }
        public string get_file_name(size_t file) { return m_ini_index[(int)file].first; }
        public size_t get_category_count(size_t file) { return m_ini_index[(int)file].second.size(); }
        public string get_category_name(size_t file, size_t category) { return m_ini_index[(int)file].second[(int)category].first; }


        // init category index
        void init_category(string filename, util.core_file file)
        {
            throw new emu_unimplemented();
        }
    }


    //-------------------------------------------------
    //  FAVORITE MANAGER
    //-------------------------------------------------
    public class favorite_manager
    {
        //using running_software_key = std::tuple<game_driver const &, char const *, std::string const &>;


        static class favorite_compare
        {
            //using is_transparent = std::true_type;

            //bool operator()(ui_software_info const &lhs, ui_software_info const &rhs) const;

            //bool operator()(ui_software_info const &lhs, game_driver const &rhs) const;
            //bool operator()(game_driver const &lhs, ui_software_info const &rhs) const;
            public static bool op(game_driver lhs, ui_software_info rhs)
            {
                assert(rhs.driver != null);

                if (rhs.startempty == 0)
                    return true;
                else
                    return 0 > std.strncmp(lhs.name, rhs.driver.name, std.size(lhs.name));
            }

            //bool operator()(ui_software_info const &lhs, running_software_key const &rhs) const;
            //bool operator()(running_software_key const &lhs, ui_software_info const &rhs) const;
        }

        //using favorites_set = std::set<ui_software_info, favorite_compare>;
        //using sorted_favorites = std::vector<std::reference_wrapper<ui_software_info const> >;


        const string FAVORITE_FILENAME = "favorites.ini";


        // internal state
        ui_options m_options;
        favorites_set m_favorites;
        sorted_favorites m_sorted;
        bool m_need_sort;


        // construction/destruction
        //-------------------------------------------------
        //  ctor
        //-------------------------------------------------
        public favorite_manager(ui_options options)
        {
            m_options = options;
            m_favorites = new favorites_set();
            m_sorted = new sorted_favorites();
            m_need_sort = true;


            emu_file file = new emu_file(m_options.ui_path(), OPEN_FLAG_READ);
            if (!file.open(FAVORITE_FILENAME))
            {
                string readbuf;  //char readbuf[1024];
                file.gets(out readbuf, 1024);

                while (readbuf[0] == '[')
                    file.gets(out readbuf, 1024);

                while (file.gets(out readbuf, 1024) != null)
                {
                    ui_software_info tmpmatches = new ui_software_info();
                    tmpmatches.shortname = chartrimcarriage(readbuf);
                    file.gets(out readbuf, 1024);
                    tmpmatches.longname = chartrimcarriage(readbuf);
                    file.gets(out readbuf, 1024);
                    tmpmatches.parentname = chartrimcarriage(readbuf);
                    file.gets(out readbuf, 1024);
                    tmpmatches.year = chartrimcarriage(readbuf);
                    file.gets(out readbuf, 1024);
                    tmpmatches.publisher = chartrimcarriage(readbuf);
                    file.gets(out readbuf, 1024);
                    tmpmatches.supported = (software_support)std.atoi(readbuf);
                    file.gets(out readbuf, 1024);
                    tmpmatches.part = chartrimcarriage(readbuf);
                    file.gets(out readbuf, 1024);
                    chartrimcarriage(readbuf);
                    var dx = driver_list.find(readbuf);
                    if (0 > dx)
                        continue;
                    tmpmatches.driver = driver_list.driver((size_t)dx);
                    file.gets(out readbuf, 1024);
                    tmpmatches.listname = chartrimcarriage(readbuf);
                    file.gets(out readbuf, 1024);
                    tmpmatches.interface_ = chartrimcarriage(readbuf);
                    file.gets(out readbuf, 1024);
                    tmpmatches.instance = chartrimcarriage(readbuf);
                    file.gets(out readbuf, 1024);
                    tmpmatches.startempty = (uint8_t)std.atoi(readbuf);
                    file.gets(out readbuf, 1024);
                    tmpmatches.parentlongname = chartrimcarriage(readbuf);
                    file.gets(out readbuf, 1024);
                    //tmpmatches.usage = chartrimcarriage(readbuf); TODO: recover multi-line info
                    file.gets(out readbuf, 1024);
                    tmpmatches.devicetype = chartrimcarriage(readbuf);
                    file.gets(out readbuf, 1024);
                    tmpmatches.available = std.atoi(readbuf) != 0;

                    // need to populate this, it isn't displayed anywhere else
                    tmpmatches.infotext = tmpmatches.infotext.append_(tmpmatches.longname);
                    tmpmatches.infotext = tmpmatches.infotext.append_(1, '\n');
                    tmpmatches.infotext = tmpmatches.infotext.append_(__("swlist-info", "Software list/item"));
                    tmpmatches.infotext = tmpmatches.infotext.append_(1, '\n');
                    tmpmatches.infotext = tmpmatches.infotext.append_(tmpmatches.listname);
                    tmpmatches.infotext = tmpmatches.infotext.append_(1, ':');
                    tmpmatches.infotext = tmpmatches.infotext.append_(tmpmatches.shortname);

                    m_favorites.emplace(tmpmatches);
                }

                file.close();
            }
        }


        // add
        //void add_favorite_system(game_driver const &driver);
        //void add_favorite_software(ui_software_info const &swinfo);
        //void add_favorite(running_machine &machine);


        // check
        public bool is_favorite_system(game_driver driver) { return check_impl(driver); }
        public bool is_favorite_software(ui_software_info swinfo) { throw new emu_unimplemented(); }
        public bool is_favorite_system_software(ui_software_info swinfo) { throw new emu_unimplemented(); }
        //bool is_favorite(running_machine &machine) const;


        // remove
        //void remove_favorite_system(game_driver const &driver);
        //void remove_favorite_software(ui_software_info const &swinfo);
        //void remove_favorite(running_machine &machine);


        // walk

        public delegate void apply_action(ui_software_info info);

        //template <typename T>
        public void apply(apply_action action) { throw new emu_unimplemented(); }

        //template <typename T>
        public void apply_sorted(apply_action action) { throw new emu_unimplemented(); }


        // implementation
        //template <typename T> static void apply_running_machine(running_machine &machine, T &&action);
        //template <typename T> void add_impl(T &&key);

        //template <typename T>
        bool check_impl(object key)  //bool check_impl(T const &key) const;
        {
            //return m_favorites.find(key) != m_favorites.end();
            if (key is game_driver key_game)
            {
                return m_favorites.ContainsIf((item) => { return favorite_compare.op(key_game, item); });
            }
            else  // add new type as needed
            {
                throw new emu_unimplemented();
            }
        }

        //template <typename T> bool remove_impl(T const &key);
        //void update_sorted();
        //void save_favorites();
    }
}
