// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    // ======================> plugin_options
    class plugin_options
    {
        // ======================> plugin
        public class plugin
        {
            string m_name;
            string m_description;
            string m_type;
            string m_directory;
            public bool m_start;
        }


        //std::list<plugin> m_plugins;


        public plugin_options() { }


        // accessors
        //std.list<plugin> plugins() { return m_plugins; }
        //const std::list<plugin> &plugins() const { return m_plugins; }

        // methods
        public void scan_directory(string path, bool recursive)
        {
            throw new emu_unimplemented();
        }


        //bool load_plugin(const std::string &path);


        public plugin find(string name)
        {
            throw new emu_unimplemented();
        }


        // INI functionality
        public void parse_ini_file(util.core_file inifile)
        {
            throw new emu_unimplemented();
        }

        //std::string output_ini() const;
    }
}
