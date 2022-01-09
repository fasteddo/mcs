// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint32_t = System.UInt32;


namespace mame.ui
{
    //-------------------------------------------------
    //  class dats menu
    //-------------------------------------------------
    class menu_dats_view : menu_textbox
    {
        //ui_system_info const *const m_system;
        //ui_software_info const *const m_swinfo;
        //bool const m_issoft;
        //int m_actual;
        //std::string m_list, m_short, m_long, m_parent;
        //std::vector<list_items> m_items_list;


        menu_dats_view(mame_ui_manager mui, render_container container, ui_software_info swinfo)
            : base(mui, container)
        {
            throw new emu_unimplemented();
        }


        menu_dats_view(mame_ui_manager mui, render_container container, ui_system_info system = null)
            : base(mui, container)
        {
            throw new emu_unimplemented();
        }

        //virtual ~menu_dats_view() override;


        public static void add_info_text(text_layout layout, string text, rgb_t color, float size = 1.0f) { throw new emu_unimplemented(); }


        protected override void custom_render(object selectedref, float top, float bottom, float x, float y, float x2, float y2) { throw new emu_unimplemented(); }
        protected override bool custom_mouse_down() { throw new emu_unimplemented(); }

        protected override void populate_text(text_layout layout, float width, int lines) { throw new emu_unimplemented(); }


        //struct list_items
        //{
        //    list_items(std::string &&l, int i, std::string &&rev) : label(std::move(l)), option(i), revision(std::move(rev)) { }
        //
        //    std::string label;
        //    int option;
        //    std::string revision;
        //};



        protected override void populate(ref float customtop, ref float custombottom) { throw new emu_unimplemented(); }
        protected override void handle(event_ ev) { throw new emu_unimplemented(); }


        //void get_data(std::string &buffer);
        //void get_data_sw(std::string &buffer);
    }
}
