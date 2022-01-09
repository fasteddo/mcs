// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint32_t = System.UInt32;


namespace mame.ui
{
    abstract class menu_textbox : menu
    {
        //std::optional<text_layout> m_layout;
        //float m_layout_width;
        //float m_desired_width;
        //int m_desired_lines;
        //int m_window_lines;
        //int m_top_line;


        protected menu_textbox(mame_ui_manager mui, render_container container)
            : base(mui, container)
        {
            throw new emu_unimplemented();
        }

        //virtual ~menu_textbox() override;


        //void reset_layout();
        //void handle_key(int key);

        protected abstract void populate_text(text_layout layout, float width, int lines);

        protected override bool custom_mouse_scroll(int lines) { throw new emu_unimplemented(); }


        protected override void draw(uint32_t flags) { throw new emu_unimplemented(); }
    }
}
