// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using categoryindex = mame.std_vector<System.Collections.Generic.KeyValuePair<string, System.Int64>>;


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
        std_vector<KeyValuePair<string, categoryindex>> m_ini_index;  //std::vector<std::pair<std::string, categoryindex> > m_ini_index;


        // construction/destruction
        //-------------------------------------------------
        //  ctor
        //-------------------------------------------------
        public inifile_manager(running_machine machine, ui_options moptions)
        {
            m_options = moptions;
            m_ini_index = new std_vector<KeyValuePair<string, categoryindex>>();


            // scan directories and create index
            file_enumerator path = new file_enumerator(m_options.categoryini_path());
            for (osd.directory.entry dir = path.next(); dir != null; dir = path.next())
            {
                string name = dir.name;
                if (global.core_filename_ends_with(name, ".ini"))
                {
                    emu_file file = new emu_file(m_options.categoryini_path(), osdcore_global.OPEN_FLAG_READ);
                    if (file.open(name) == osd_file.error.NONE)
                    {
                        init_category(name, file);
                        file.close();
                    }
                }
            }

            //std::stable_sort(m_ini_index.begin(), m_ini_index.end());//, [] (auto const &x, auto const &y) { return 0 > core_stricmp(x.first.c_str(), y.first.c_str()); });
            m_ini_index.Sort((x, y) => { return global.core_stricmp(x.Key.c_str(), y.Key.c_str()); });
        }


        // load games from category
        public void load_ini_category(UInt32 file, UInt32 category, std_unordered_set<game_driver> result)
        {
            string filename = m_ini_index[(int)file].Key;
            emu_file fp = new emu_file(m_options.categoryini_path(), osdcore_global.OPEN_FLAG_READ);
            if (fp.open(filename) != osd_file.error.NONE)
            {
                global.osd_printf_error("Failed to open category file {0} for reading\n", filename.c_str());
                return;
            }

            Int64 offset = m_ini_index[(int)file].Value[(int)category].Value;
            if (fp.seek(offset, emu_file.SEEK_SET) != 0 || (fp.tell() != (UInt64)offset))
            {
                fp.close();
                global.osd_printf_error("Failed to seek to category offset in file {0}\n", filename.c_str());
                return;
            }

            string rbuf;  // char rbuf[MAX_CHAR_INFO];
            while (fp.gets(out rbuf, utils_global.MAX_CHAR_INFO) != null && !string.IsNullOrEmpty(rbuf) && ('[' != rbuf[0]))
            {
                //var tail = std::find_if(std::begin(rbuf), std::prev(std::end(rbuf)));//, [] (char ch) { return !ch || ('\r' == ch) || ('\n' == ch); });
                //*tail = '\0';
                var tail = rbuf.IndexOfAny("\r\n".ToCharArray());
                rbuf = rbuf.Substring(tail);
                int dfind = driver_list.find(rbuf);
                if (0 <= dfind)
                    result.emplace(driver_list.driver((UInt32)dfind));
            }

            fp.close();
        }


        // getters
        public UInt32 get_file_count() { return (UInt32)m_ini_index.size(); }
        public string get_file_name(UInt32 file) { return m_ini_index[(int)file].Key; }
        public UInt32 get_category_count(UInt32 file) { return (UInt32)m_ini_index[(int)file].Value.size(); }
        public string get_category_name(UInt32 file, UInt32 category) { return m_ini_index[(int)file].Value[(int)category].Key; }


        // init category index
        void init_category(string filename, emu_file file)
        {
            throw new emu_unimplemented();
        }
    }


    //-------------------------------------------------
    //  FAVORITE MANAGER
    //-------------------------------------------------
    public class favorite_manager
    {
        // favorite indices
        std_multimap<string, ui_software_info> m_list = new std_multimap<string, ui_software_info>();  // std::multimap<std::string, ui_software_info, ci_less> m_list;


        //const char *favorite_filename = "favorites.ini";

        // current
        List<ui_software_info> m_current;  //std::multimap<std::string, ui_software_info>::iterator m_current;


        // internal state
        running_machine m_machine;  // reference to our machine
        ui_options m_options;


        // construction/destruction
        //-------------------------------------------------
        //  ctor
        //-------------------------------------------------
        public favorite_manager(running_machine machine, ui_options moptions)
        {
            m_machine = machine;
            m_options = moptions;


            parse_favorite();
        }


        // getters
        running_machine machine() { return m_machine; }
        public std_multimap<string, ui_software_info> list() { return m_list; }


        // add
        public void add_favorite_game() { throw new emu_unimplemented(); }
        public void add_favorite_game(game_driver driver) { throw new emu_unimplemented(); }
        public void add_favorite_game(ui_software_info swinfo) { throw new emu_unimplemented(); }


        // check

        //-------------------------------------------------
        //  check if game is already in favorite list
        //-------------------------------------------------
        public bool isgame_favorite()
        {
            if ((machine().system().flags & machine_flags.type.MASK_TYPE) == machine_flags.type.TYPE_ARCADE)
                return isgame_favorite(machine().system());

            var image_loaded = false;

            foreach (device_image_interface image in new image_interface_iterator(machine().root_device()))
            {
                software_info swinfo = image.software_entry();
                if (image.exists() && swinfo != null)
                {
                    image_loaded = true;
                    foreach (var current in m_list)  //for (var current = m_list.begin(); current != m_list.end(); ++current)
                    {
                        if (current.Value[0].shortname == swinfo.shortname() &&
                            current.Value[0].listname == image.software_list_name())
                        {
                            m_current = current.Value;
                            return true;
                        }
                    }
                }
            }

            if (!image_loaded)
                return isgame_favorite(machine().system());

            m_current = null;  //m_list.begin();
            return false;
        }


        //-------------------------------------------------
        //  check if game is already in favorite list
        //-------------------------------------------------
        public bool isgame_favorite(game_driver driver)
        {
            foreach (var current in m_list)  //for (var current = m_list.begin(); current != m_list.end(); ++current)
            {
                if (current.Value[0].driver == driver && current.Value[0].shortname == driver.name)
                {
                    m_current = current.Value;
                    return true;
                }
            }

            m_current = null;  //m_list.begin();
            return false;
        }


        //-------------------------------------------------
        //  check if game is already in favorite list
        //-------------------------------------------------
        public bool isgame_favorite(ui_software_info swinfo)
        {
            foreach (var current in m_list)  //for (var current = m_list.begin(); current != m_list.end(); ++current)
            {
                if (current.Value[0] == swinfo)
                {
                    m_current = current.Value;
                    return true;
                }
            }

            m_current = null;  //m_list.begin();
            return false;
        }


        // save
        //void save_favorite_games();


        // remove
        public void remove_favorite_game()
        {
            throw new emu_unimplemented();
        }

        public void remove_favorite_game(ui_software_info swinfo)
        {
            throw new emu_unimplemented();
        }


        // parse file ui_favorite
        //-------------------------------------------------
        //  parse favorite file
        //-------------------------------------------------
        void parse_favorite()
        {
            //throw new emu_unimplemented();
        }
    }
}
