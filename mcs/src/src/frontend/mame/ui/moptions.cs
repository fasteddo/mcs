// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public class ui_options : core_options
    {
        // core directory options
        const string OPTION_HISTORY_PATH          = "historypath";
        const string OPTION_CATEGORYINI_PATH      = "categorypath";
        public const string OPTION_CABINETS_PATH         = "cabinets_directory";
        public const string OPTION_CPANELS_PATH          = "cpanels_directory";
        public const string OPTION_PCBS_PATH             = "pcbs_directory";
        public const string OPTION_FLYERS_PATH           = "flyers_directory";
        public const string OPTION_TITLES_PATH           = "titles_directory";
        public const string OPTION_ENDS_PATH             = "ends_directory";
        public const string OPTION_MARQUEES_PATH         = "marquees_directory";
        public const string OPTION_ARTPREV_PATH          = "artwork_preview_directory";
        public const string OPTION_BOSSES_PATH           = "bosses_directory";
        public const string OPTION_LOGOS_PATH            = "logos_directory";
        public const string OPTION_SCORES_PATH           = "scores_directory";
        public const string OPTION_VERSUS_PATH           = "versus_directory";
        public const string OPTION_GAMEOVER_PATH         = "gameover_directory";
        public const string OPTION_HOWTO_PATH            = "howto_directory";
        public const string OPTION_SELECT_PATH           = "select_directory";
        const string OPTION_ICONS_PATH            = "icons_directory";
        public const string OPTION_COVER_PATH            = "covers_directory";
        const string OPTION_UI_PATH               = "ui_path";

        // core misc options
        const string OPTION_REMEMBER_LAST         = "remember_last";
        const string OPTION_ENLARGE_SNAPS         = "enlarge_snaps";
        const string OPTION_FORCED4X3             = "forced4x3";
        const string OPTION_USE_BACKGROUND        = "use_background";
        const string OPTION_SKIP_BIOS_MENU        = "skip_biosmenu";
        const string OPTION_SKIP_PARTS_MENU       = "skip_partsmenu";
        public const string OPTION_LAST_USED_FILTER      = "last_used_filter";
        public const string OPTION_LAST_RIGHT_PANEL      = "last_right_panel";
        public const string OPTION_LAST_USED_MACHINE     = "last_used_machine";
        const string OPTION_INFO_AUTO_AUDIT       = "info_audit_enabled";
        const string OPTION_HIDE_ROMLESS          = "hide_romless";

        // core UI options
        const string OPTION_INFOS_SIZE            = "infos_text_size";
        const string OPTION_FONT_ROWS             = "font_rows";
        public const string OPTION_HIDE_PANELS           = "hide_main_panel";

        public const string OPTION_UI_BORDER_COLOR       = "ui_border_color";
        public const string OPTION_UI_BACKGROUND_COLOR   = "ui_bg_color";
        public const string OPTION_UI_GFXVIEWER_BG_COLOR = "ui_gfxviewer_color";
        public const string OPTION_UI_UNAVAILABLE_COLOR  = "ui_unavail_color";
        public const string OPTION_UI_TEXT_COLOR         = "ui_text_color";
        public const string OPTION_UI_TEXT_BG_COLOR      = "ui_text_bg_color";
        public const string OPTION_UI_SUBITEM_COLOR      = "ui_subitem_color";
        public const string OPTION_UI_CLONE_COLOR        = "ui_clone_color";
        public const string OPTION_UI_SELECTED_COLOR     = "ui_selected_color";
        public const string OPTION_UI_SELECTED_BG_COLOR  = "ui_selected_bg_color";
        public const string OPTION_UI_MOUSEOVER_COLOR    = "ui_mouseover_color";
        public const string OPTION_UI_MOUSEOVER_BG_COLOR = "ui_mouseover_bg_color";
        public const string OPTION_UI_MOUSEDOWN_COLOR    = "ui_mousedown_color";
        public const string OPTION_UI_MOUSEDOWN_BG_COLOR = "ui_mousedown_bg_color";
        public const string OPTION_UI_DIPSW_COLOR        = "ui_dipsw_color";
        public const string OPTION_UI_SLIDER_COLOR       = "ui_slider_color";


        //**************************************************************************
        //  UI EXTRA OPTIONS
        //**************************************************************************
        static readonly options_entry [] s_option_entries = new options_entry[]
        {
            // search path options
            new options_entry(null,                                 null,                          options_global.OPTION_HEADER,  "UI SEARCH PATH OPTIONS"),
            new options_entry(OPTION_HISTORY_PATH,                  "history;dats;.",              options_global.OPTION_STRING,  "path to history files"),
            new options_entry(OPTION_CATEGORYINI_PATH,              "folders",                     options_global.OPTION_STRING,  "path to catagory ini files"),
            new options_entry(OPTION_CABINETS_PATH,                 "cabinets;cabdevs",            options_global.OPTION_STRING,  "path to cabinets / devices image"),
            new options_entry(OPTION_CPANELS_PATH,                  "cpanel",                      options_global.OPTION_STRING,  "path to control panel image"),
            new options_entry(OPTION_PCBS_PATH,                     "pcb",                         options_global.OPTION_STRING,  "path to pcbs image"),
            new options_entry(OPTION_FLYERS_PATH,                   "flyers",                      options_global.OPTION_STRING,  "path to flyers image"),
            new options_entry(OPTION_TITLES_PATH,                   "titles",                      options_global.OPTION_STRING,  "path to titles image"),
            new options_entry(OPTION_ENDS_PATH,                     "ends",                        options_global.OPTION_STRING,  "path to ends image"),
            new options_entry(OPTION_MARQUEES_PATH,                 "marquees",                    options_global.OPTION_STRING,  "path to marquees image"),
            new options_entry(OPTION_ARTPREV_PATH,                  "artwork preview;artpreview",  options_global.OPTION_STRING,  "path to artwork preview image"),
            new options_entry(OPTION_BOSSES_PATH,                   "bosses",                      options_global.OPTION_STRING,  "path to bosses image"),
            new options_entry(OPTION_LOGOS_PATH,                    "logo",                        options_global.OPTION_STRING,  "path to logos image"),
            new options_entry(OPTION_SCORES_PATH,                   "scores",                      options_global.OPTION_STRING,  "path to scores image"),
            new options_entry(OPTION_VERSUS_PATH,                   "versus",                      options_global.OPTION_STRING,  "path to versus image"),
            new options_entry(OPTION_GAMEOVER_PATH,                 "gameover",                    options_global.OPTION_STRING,  "path to gameover image"),
            new options_entry(OPTION_HOWTO_PATH,                    "howto",                       options_global.OPTION_STRING,  "path to howto image"),
            new options_entry(OPTION_SELECT_PATH,                   "select",                      options_global.OPTION_STRING,  "path to select image"),
            new options_entry(OPTION_ICONS_PATH,                    "icons",                       options_global.OPTION_STRING,  "path to ICOns image"),
            new options_entry(OPTION_COVER_PATH,                    "covers",                      options_global.OPTION_STRING,  "path to software cover image"),
            new options_entry(OPTION_UI_PATH,                       "ui",                          options_global.OPTION_STRING,  "path to UI files"),

            // misc options
            new options_entry(null,                                 null,       options_global.OPTION_HEADER,      "UI MISC OPTIONS"),
            new options_entry(OPTION_REMEMBER_LAST,                 "1",        options_global.OPTION_BOOLEAN,     "reselect in main menu last played game"),
            new options_entry(OPTION_ENLARGE_SNAPS,                 "1",        options_global.OPTION_BOOLEAN,     "enlarge arts (snapshot, title, etc...) in right panel (keeping aspect ratio)"),
            new options_entry(OPTION_FORCED4X3,                     "1",        options_global.OPTION_BOOLEAN,     "force the appearance of the snapshot in the list software to 4:3"),
            new options_entry(OPTION_USE_BACKGROUND,                "1",        options_global.OPTION_BOOLEAN,     "enable background image in main view"),
            new options_entry(OPTION_SKIP_BIOS_MENU,                "0",        options_global.OPTION_BOOLEAN,     "skip bios submenu, start with configured or default"),
            new options_entry(OPTION_SKIP_PARTS_MENU,               "0",        options_global.OPTION_BOOLEAN,     "skip parts submenu, start with first part"),
            new options_entry(OPTION_LAST_USED_FILTER,              "",         options_global.OPTION_STRING,      "latest used filter"),
            new options_entry(OPTION_LAST_RIGHT_PANEL + "(0-1)",    "0",        options_global.OPTION_INTEGER,     "latest right panel focus"),
            new options_entry(OPTION_LAST_USED_MACHINE,             "",         options_global.OPTION_STRING,      "latest used machine"),
            new options_entry(OPTION_INFO_AUTO_AUDIT,               "0",        options_global.OPTION_BOOLEAN,     "enable auto audit in the general info panel"),
            new options_entry(OPTION_HIDE_ROMLESS,                  "1",        options_global.OPTION_BOOLEAN,     "hide romless machine from available list"),

            // UI options
            new options_entry(null,                                 null,           options_global.OPTION_HEADER,      "UI OPTIONS"),
            new options_entry(OPTION_INFOS_SIZE + "(0.05-1.00)",    "0.75",         options_global.OPTION_FLOAT,       "UI right panel infos text size (0.05 - 1.00)"),
            new options_entry(OPTION_FONT_ROWS + "(25-40)",         "30",           options_global.OPTION_INTEGER,     "UI font lines per screen (25 - 40)"),
            new options_entry(OPTION_HIDE_PANELS + "(0-3)",         "0",            options_global.OPTION_INTEGER,     "UI hide left/right panel in main view (0 = Show all, 1 = hide left, 2 = hide right, 3 = hide both"),
            new options_entry(OPTION_UI_BORDER_COLOR,               "ffffffff",     options_global.OPTION_STRING,      "UI border color (ARGB)"),
            new options_entry(OPTION_UI_BACKGROUND_COLOR,           "ef101030",     options_global.OPTION_STRING,      "UI background color (ARGB)"),
            new options_entry(OPTION_UI_CLONE_COLOR,                "ff808080",     options_global.OPTION_STRING,      "UI clone color (ARGB)"),
            new options_entry(OPTION_UI_DIPSW_COLOR,                "ffffff00",     options_global.OPTION_STRING,      "UI dipswitch color (ARGB)"),
            new options_entry(OPTION_UI_GFXVIEWER_BG_COLOR,         "ef101030",     options_global.OPTION_STRING,      "UI gfx viewer color (ARGB)"),
            new options_entry(OPTION_UI_MOUSEDOWN_BG_COLOR,         "b0606000",     options_global.OPTION_STRING,      "UI mouse down bg color (ARGB)"),
            new options_entry(OPTION_UI_MOUSEDOWN_COLOR,            "ffffff80",     options_global.OPTION_STRING,      "UI mouse down color (ARGB)"),
            new options_entry(OPTION_UI_MOUSEOVER_BG_COLOR,         "70404000",     options_global.OPTION_STRING,      "UI mouse over bg color (ARGB)"),
            new options_entry(OPTION_UI_MOUSEOVER_COLOR,            "ffffff80",     options_global.OPTION_STRING,      "UI mouse over color (ARGB)"),
            new options_entry(OPTION_UI_SELECTED_BG_COLOR,          "ef808000",     options_global.OPTION_STRING,      "UI selected bg color (ARGB)"),
            new options_entry(OPTION_UI_SELECTED_COLOR,             "ffffff00",     options_global.OPTION_STRING,      "UI selected color (ARGB)"),
            new options_entry(OPTION_UI_SLIDER_COLOR,               "ffffffff",     options_global.OPTION_STRING,      "UI slider color (ARGB)"),
            new options_entry(OPTION_UI_SUBITEM_COLOR,              "ffffffff",     options_global.OPTION_STRING,      "UI subitem color (ARGB)"),
            new options_entry(OPTION_UI_TEXT_BG_COLOR,              "ef000000",     options_global.OPTION_STRING,      "UI text bg color (ARGB)"),
            new options_entry(OPTION_UI_TEXT_COLOR,                 "ffffffff",     options_global.OPTION_STRING,      "UI text color (ARGB)"),
            new options_entry(OPTION_UI_UNAVAILABLE_COLOR,          "ff404040",     options_global.OPTION_STRING,      "UI unavailable color (ARGB)"),
            new options_entry(null),
        };


        // construction/destruction
        //-------------------------------------------------
        //  ui_options - constructor
        //-------------------------------------------------
        public ui_options() : base() { add_entries(s_option_entries); }


        // Search path options
        //const char *history_path() const { return value(OPTION_HISTORY_PATH); }
        public string categoryini_path() { return value(OPTION_CATEGORYINI_PATH); }
        //const char *cabinets_directory() const { return value(OPTION_CABINETS_PATH); }
        //const char *cpanels_directory() const { return value(OPTION_CPANELS_PATH); }
        //const char *pcbs_directory() const { return value(OPTION_PCBS_PATH); }
        //const char *flyers_directory() const { return value(OPTION_FLYERS_PATH); }
        //const char *titles_directory() const { return value(OPTION_TITLES_PATH); }
        //const char *ends_directory() const { return value(OPTION_ENDS_PATH); }
        //const char *marquees_directory() const { return value(OPTION_MARQUEES_PATH); }
        //const char *artprev_directory() const { return value(OPTION_ARTPREV_PATH); }
        //const char *bosses_directory() const { return value(OPTION_BOSSES_PATH); }
        //const char *logos_directory() const { return value(OPTION_LOGOS_PATH); }
        //const char *scores_directory() const { return value(OPTION_SCORES_PATH); }
        //const char *versus_directory() const { return value(OPTION_VERSUS_PATH); }
        //const char *gameover_directory() const { return value(OPTION_GAMEOVER_PATH); }
        //const char *howto_directory() const { return value(OPTION_HOWTO_PATH); }
        //const char *select_directory() const { return value(OPTION_SELECT_PATH); }
        public string icons_directory() { return value(OPTION_ICONS_PATH); }
        //const char *covers_directory() const { return value(OPTION_COVER_PATH); }
        public string ui_path() { return value(OPTION_UI_PATH); }


        // Misc options
        public bool remember_last() { return bool_value(OPTION_REMEMBER_LAST); }
        public bool enlarge_snaps() { return bool_value(OPTION_ENLARGE_SNAPS); }
        public bool forced_4x3_snapshot() { return bool_value(OPTION_FORCED4X3); }
        public bool use_background_image() { return bool_value(OPTION_USE_BACKGROUND); }
        public bool skip_bios_menu() { return bool_value(OPTION_SKIP_BIOS_MENU); }
        public bool skip_parts_menu() { return bool_value(OPTION_SKIP_PARTS_MENU); }
        public string last_used_machine() { return value(OPTION_LAST_USED_MACHINE); }
        public string last_used_filter() { return value(OPTION_LAST_USED_FILTER); }
        public int last_right_panel() { return int_value(OPTION_LAST_RIGHT_PANEL); }
        public bool info_audit() { return bool_value(OPTION_INFO_AUTO_AUDIT); }
        public bool hide_romless() { return bool_value(OPTION_HIDE_ROMLESS); }


        // UI options
        public float infos_size() { return float_value(OPTION_INFOS_SIZE); }
        public int font_rows() { return int_value(OPTION_FONT_ROWS); }
        public int hide_panels() { return int_value(OPTION_HIDE_PANELS); }

        //const char *ui_border_color() const { return value(OPTION_UI_BORDER_COLOR); }
        //const char *ui_bg_color() const { return value(OPTION_UI_BACKGROUND_COLOR); }
        //const char *ui_gfx_bg_color() const { return value(OPTION_UI_GFXVIEWER_BG_COLOR); }
        //const char *ui_unavail_color() const { return value(OPTION_UI_UNAVAILABLE_COLOR); }
        //const char *ui_text_color() const { return value(OPTION_UI_TEXT_COLOR); }
        //const char *ui_text_bg_color() const { return value(OPTION_UI_TEXT_BG_COLOR); }
        //const char *ui_subitem_color() const { return value(OPTION_UI_SUBITEM_COLOR); }
        //const char *ui_clone_color() const { return value(OPTION_UI_CLONE_COLOR); }
        //const char *ui_selected_color() const { return value(OPTION_UI_SELECTED_COLOR); }
        //const char *ui_selected_bg_color() const { return value(OPTION_UI_SELECTED_BG_COLOR); }
        //const char *ui_mouseover_color() const { return value(OPTION_UI_MOUSEOVER_COLOR); }
        //const char *ui_mouseover_bg_color() const { return value(OPTION_UI_MOUSEOVER_BG_COLOR); }
        //const char *ui_mousedown_color() const { return value(OPTION_UI_MOUSEDOWN_COLOR); }
        //const char *ui_mousedown_bg_color() const { return value(OPTION_UI_MOUSEDOWN_BG_COLOR); }
        //const char *ui_dipsw_color() const { return value(OPTION_UI_DIPSW_COLOR); }
        //const char *ui_slider_color() const { return value(OPTION_UI_SLIDER_COLOR); }
    }
}