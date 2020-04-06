// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.ui
{
    class simple_menu_select_game : menu
    {
        // internal state
        //enum { VISIBLE_GAMES_IN_LIST = 15 };
        //UINT8                   m_error;
        //bool                    m_rerandomize;
        string m_search;  //char                    m_search[40];
        //int                     m_skip_main_items;
        //int                     m_matchlist[VISIBLE_GAMES_IN_LIST];
        std.vector<game_driver> m_driverlist;
        //std::unique_ptr<driver_enumerator> m_drivlist;


        //-------------------------------------------------
        //  ctor
        //-------------------------------------------------
        simple_menu_select_game(mame_ui_manager mui, render_container container, string gamename)
            : base(mui, container)
        {
            m_driverlist = new std.vector<game_driver>(driver_list.total() + 1);


            throw new emu_unimplemented();
        }


        protected override void populate(ref float customtop, ref float custombottom)
        {
            throw new emu_unimplemented();
        }

        protected override void handle()
        {
            throw new emu_unimplemented();
        }


        //virtual void custom_render(void *selectedref, float top, float bottom, float x, float y, float x2, float y2) override;


        // force game select menu
        public static void force_game_select(mame_ui_manager mui, render_container container)
        {
            throw new emu_unimplemented();
        }


        protected override bool menu_has_search_active() { return !string.IsNullOrEmpty(m_search); }


        // internal methods
        //void build_driver_list();
        //void inkey_select(const ui_menu_event *menu_event);
        //void inkey_cancel(const ui_menu_event *menu_event);
        //void inkey_special(const ui_menu_event *menu_event);
    }
}
