// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    class plugin_options : core_options
    {
        //static const options_entry s_option_entries[];
        static readonly options_entry [] s_option_entries = new options_entry[]
        {
            new options_entry(null,  null,  options_global.OPTION_HEADER,  "PLUGINS OPTIONS"),
            new options_entry(null),
        };


        std_list<string> m_descriptions;


        // construction/destruction
        //-------------------------------------------------
        //  plugin_options - constructor
        //-------------------------------------------------
        public plugin_options() : base()
        {
            add_entries(s_option_entries);
        }


        public void parse_json(string path)
        {
            //throw new emu_unimplemented();
        }
    }
}
