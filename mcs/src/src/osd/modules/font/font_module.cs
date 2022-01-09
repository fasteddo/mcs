// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    abstract class font_module : osd_module
    {
        public const string OSD_FONT_PROVIDER = "uifontprovider";


        public font_module(string type, string name) : base(type, name) { }


        /** attempt to allocate a font instance */
        public abstract osd_font font_alloc();

        /** attempt to list available font families */
        //abstract bool get_font_families(std::string const &font_path, std::vector<std::pair<std::string, std::string> > &result) = 0;
    }
}
