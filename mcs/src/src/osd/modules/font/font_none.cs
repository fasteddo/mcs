// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    class osd_font_none : osd_font
    {
        public bool open(string font_path, string name, out int height) { height = 0; return false; }
        //virtual void close() override { }
        //virtual bool get_bitmap(char32_t chnum, bitmap_argb32 &bitmap, std::int32_t &width, std::int32_t &xoffs, std::int32_t &yoffs) override { return false; }
    }


    class font_none : font_module
    {
        //MODULE_DEFINITION(FONT_NONE, font_none)
        static osd_module module_creator_font_none() { return new font_none(); }
        public static readonly module_type FONT_NONE = MODULE_DEFINITION(module_creator_font_none);


        font_none() : base(OSD_FONT_PROVIDER, "none") { }

        public override int init(osd_options options) { return 0; }

        public override osd_font font_alloc() { return new osd_font_none(); }

        //virtual bool get_font_families(std::string const &font_path, std::vector<std::pair<std::string, std::string> > &result) override { return false; }
    }
}
