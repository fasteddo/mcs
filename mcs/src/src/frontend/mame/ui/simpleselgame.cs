// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame.ui
{
    class simple_menu_select_game : menu
    {
        // internal state
        //bool                    m_nomatch;
        //bool                    m_error;
        //bool                    m_rerandomize;
        //std::string             m_search;
        //int                     m_matchlist[VISIBLE_GAMES_IN_LIST];
        //std::vector<const game_driver *>    m_driverlist;
        //std::unique_ptr<driver_enumerator>  m_drivlist;

        // cached driver flags
        //const game_driver *     m_cached_driver;
        //machine_flags::type     m_cached_flags;
        //device_t::feature_type  m_cached_unemulated;
        //device_t::feature_type  m_cached_imperfect;
        //rgb_t                   m_cached_color;


        //-------------------------------------------------
        //  ctor
        //-------------------------------------------------
        simple_menu_select_game(mame_ui_manager mui, render_container container, string gamename)
            : base(mui, container)
        {
            throw new emu_unimplemented();
        }


        // force game select menu
        public static void force_game_select(mame_ui_manager mui, render_container container) { throw new emu_unimplemented(); }


        protected override void custom_render(object selectedref, float top, float bottom, float x, float y, float x2, float y2) { throw new emu_unimplemented(); }
        protected override bool custom_ui_cancel() { throw new emu_unimplemented(); }


        protected override void populate(ref float customtop, ref float custombottom) { throw new emu_unimplemented(); }
        protected override void handle(event_ ev) { throw new emu_unimplemented(); }


        // internal methods
        //void build_driver_list();
        //void inkey_select(const event &menu_event);
        //void inkey_cancel();
        //void inkey_special(const event &menu_event);
    }
}
