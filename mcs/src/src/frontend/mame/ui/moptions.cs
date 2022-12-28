// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint32_t = System.UInt32;

using static mame.options_global;


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
        const string OPTION_SYSTEM_NAMES           = "system_names";
        const string OPTION_SKIP_WARNINGS          = "skip_warnings";
        const string OPTION_REMEMBER_LAST          = "remember_last";
        const string OPTION_ENLARGE_SNAPS          = "enlarge_snaps";
        const string OPTION_FORCED4X3              = "forced4x3";
        const string OPTION_USE_BACKGROUND         = "use_background";
        const string OPTION_SKIP_BIOS_MENU         = "skip_biosmenu";
        const string OPTION_SKIP_PARTS_MENU        = "skip_partsmenu";
        public const string OPTION_LAST_USED_FILTER      = "last_used_filter";
        public const string OPTION_LAST_RIGHT_PANEL      = "last_right_panel";
        public const string OPTION_LAST_USED_MACHINE     = "last_used_machine";
        const string OPTION_INFO_AUTO_AUDIT        = "info_audit_enabled";
        const string OPTION_HIDE_ROMLESS           = "hide_romless";
        const string OPTION_UNTHROTTLE_MUTE        = "unthrottle_mute";

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
            new options_entry(null,                                 null,                          core_options.option_type.HEADER,  "UI SEARCH PATH OPTIONS"),
            new options_entry(OPTION_HISTORY_PATH,                  "history;dats;.",              core_options.option_type.STRING,  "path to system/software info files"),
            new options_entry(OPTION_CATEGORYINI_PATH,              "folders",                     core_options.option_type.STRING,  "path to category ini files"),
            new options_entry(OPTION_CATEGORYINI_PATH,              "folders",                     core_options.option_type.STRING,  "path to catagory ini files"),
            new options_entry(OPTION_CABINETS_PATH,                 "cabinets;cabdevs",            core_options.option_type.STRING,  "path to cabinets / devices image"),
            new options_entry(OPTION_CPANELS_PATH,                  "cpanel",                      core_options.option_type.STRING,  "path to control panel image"),
            new options_entry(OPTION_PCBS_PATH,                     "pcb",                         core_options.option_type.STRING,  "path to pcbs image"),
            new options_entry(OPTION_FLYERS_PATH,                   "flyers",                      core_options.option_type.STRING,  "path to flyers image"),
            new options_entry(OPTION_TITLES_PATH,                   "titles",                      core_options.option_type.STRING,  "path to titles image"),
            new options_entry(OPTION_ENDS_PATH,                     "ends",                        core_options.option_type.STRING,  "path to ends image"),
            new options_entry(OPTION_MARQUEES_PATH,                 "marquees",                    core_options.option_type.STRING,  "path to marquees image"),
            new options_entry(OPTION_ARTPREV_PATH,                  "artwork preview;artpreview",  core_options.option_type.STRING,  "path to artwork preview image"),
            new options_entry(OPTION_BOSSES_PATH,                   "bosses",                      core_options.option_type.STRING,  "path to bosses image"),
            new options_entry(OPTION_LOGOS_PATH,                    "logo",                        core_options.option_type.STRING,  "path to logos image"),
            new options_entry(OPTION_SCORES_PATH,                   "scores",                      core_options.option_type.STRING,  "path to scores image"),
            new options_entry(OPTION_VERSUS_PATH,                   "versus",                      core_options.option_type.STRING,  "path to versus image"),
            new options_entry(OPTION_GAMEOVER_PATH,                 "gameover",                    core_options.option_type.STRING,  "path to gameover image"),
            new options_entry(OPTION_HOWTO_PATH,                    "howto",                       core_options.option_type.STRING,  "path to howto image"),
            new options_entry(OPTION_SELECT_PATH,                   "select",                      core_options.option_type.STRING,  "path to select image"),
            new options_entry(OPTION_ICONS_PATH,                    "icons",                       core_options.option_type.STRING,  "path to ICOns image"),
            new options_entry(OPTION_COVER_PATH,                    "covers",                      core_options.option_type.STRING,  "path to software cover image"),
            new options_entry(OPTION_UI_PATH,                       "ui",                          core_options.option_type.STRING,  "path to UI files"),

            // misc options
            new options_entry(null,                                 null,       core_options.option_type.HEADER,      "UI MISC OPTIONS"),
            new options_entry(OPTION_SYSTEM_NAMES,                  "",         core_options.option_type.STRING,      "translated system names file" ),
            new options_entry(OPTION_SKIP_WARNINGS,                 "0",        core_options.option_type.BOOLEAN,     "display fewer repeated warnings about imperfect emulation" ),
            new options_entry(OPTION_REMEMBER_LAST,                 "1",        core_options.option_type.BOOLEAN,     "initially select last used system in main menu" ),
            new options_entry(OPTION_ENLARGE_SNAPS,                 "1",        core_options.option_type.BOOLEAN,     "enlarge artwork (snapshot, title, etc.) in right panel (keeping aspect ratio)" ),
            new options_entry(OPTION_FORCED4X3,                     "1",        core_options.option_type.BOOLEAN,     "force the appearance of the snapshot in the list software to 4:3"),
            new options_entry(OPTION_USE_BACKGROUND,                "1",        core_options.option_type.BOOLEAN,     "enable background image in main view"),
            new options_entry(OPTION_SKIP_BIOS_MENU,                "0",        core_options.option_type.BOOLEAN,     "skip bios submenu, start with configured or default"),
            new options_entry(OPTION_SKIP_PARTS_MENU,               "0",        core_options.option_type.BOOLEAN,     "skip parts submenu, start with first part"),
            new options_entry(OPTION_LAST_USED_FILTER,              "",         core_options.option_type.STRING,      "latest used filter"),
            new options_entry(OPTION_LAST_RIGHT_PANEL + "(0-1)",    "0",        core_options.option_type.INTEGER,     "latest right panel focus"),
            new options_entry(OPTION_LAST_USED_MACHINE,             "",         core_options.option_type.STRING,      "latest used machine"),
            new options_entry(OPTION_INFO_AUTO_AUDIT,               "0",        core_options.option_type.BOOLEAN,     "enable auto audit in the general info panel"),
            new options_entry(OPTION_HIDE_ROMLESS,                  "1",        core_options.option_type.BOOLEAN,     "hide romless machine from available list"),
            new options_entry(OPTION_UNTHROTTLE_MUTE + ";utm",      "0",        core_options.option_type.BOOLEAN,     "mute audio when running unthrottled" ),

            // UI options
            new options_entry(null,                                 null,           core_options.option_type.HEADER,      "UI OPTIONS"),
            new options_entry(OPTION_INFOS_SIZE + "(0.20-1.00)",    "0.75",         core_options.option_type.FLOAT,       "UI right panel infos text size (0.20 - 1.00)"),
            new options_entry(OPTION_FONT_ROWS + "(25-40)",         "30",           core_options.option_type.INTEGER,     "UI font lines per screen (25 - 40)"),
            new options_entry(OPTION_HIDE_PANELS + "(0-3)",         "0",            core_options.option_type.INTEGER,     "UI hide left/right panel in main view (0 = Show all, 1 = hide left, 2 = hide right, 3 = hide both"),
            new options_entry(OPTION_UI_BORDER_COLOR,               "ffffffff",     core_options.option_type.STRING,      "UI border color (ARGB)"),
            new options_entry(OPTION_UI_BACKGROUND_COLOR,           "ef101030",     core_options.option_type.STRING,      "UI background color (ARGB)"),
            new options_entry(OPTION_UI_CLONE_COLOR,                "ff808080",     core_options.option_type.STRING,      "UI clone color (ARGB)"),
            new options_entry(OPTION_UI_DIPSW_COLOR,                "ffffff00",     core_options.option_type.STRING,      "UI dipswitch color (ARGB)"),
            new options_entry(OPTION_UI_GFXVIEWER_BG_COLOR,         "ef101030",     core_options.option_type.STRING,      "UI gfx viewer color (ARGB)"),
            new options_entry(OPTION_UI_MOUSEDOWN_BG_COLOR,         "b0606000",     core_options.option_type.STRING,      "UI mouse down bg color (ARGB)"),
            new options_entry(OPTION_UI_MOUSEDOWN_COLOR,            "ffffff80",     core_options.option_type.STRING,      "UI mouse down color (ARGB)"),
            new options_entry(OPTION_UI_MOUSEOVER_BG_COLOR,         "70404000",     core_options.option_type.STRING,      "UI mouse over bg color (ARGB)"),
            new options_entry(OPTION_UI_MOUSEOVER_COLOR,            "ffffff80",     core_options.option_type.STRING,      "UI mouse over color (ARGB)"),
            new options_entry(OPTION_UI_SELECTED_BG_COLOR,          "ef808000",     core_options.option_type.STRING,      "UI selected bg color (ARGB)"),
            new options_entry(OPTION_UI_SELECTED_COLOR,             "ffffff00",     core_options.option_type.STRING,      "UI selected color (ARGB)"),
            new options_entry(OPTION_UI_SLIDER_COLOR,               "ffffffff",     core_options.option_type.STRING,      "UI slider color (ARGB)"),
            new options_entry(OPTION_UI_SUBITEM_COLOR,              "ffffffff",     core_options.option_type.STRING,      "UI subitem color (ARGB)"),
            new options_entry(OPTION_UI_TEXT_BG_COLOR,              "ef000000",     core_options.option_type.STRING,      "UI text bg color (ARGB)"),
            new options_entry(OPTION_UI_TEXT_COLOR,                 "ffffffff",     core_options.option_type.STRING,      "UI text color (ARGB)"),
            new options_entry(OPTION_UI_UNAVAILABLE_COLOR,          "ff404040",     core_options.option_type.STRING,      "UI unavailable color (ARGB)"),
            new options_entry(null),
        };


        // construction/destruction
        //-------------------------------------------------
        //  ui_options - constructor
        //-------------------------------------------------
        public ui_options() : base() { add_entries(s_option_entries); }


        // Search path options
        public string history_path() { return value(OPTION_HISTORY_PATH); }
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
        public string system_names() { return value(OPTION_SYSTEM_NAMES); }
        public bool skip_warnings() { return bool_value(OPTION_SKIP_WARNINGS); }
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
        public bool unthrottle_mute() { return bool_value(OPTION_UNTHROTTLE_MUTE); }


        // UI options
        public float infos_size() { return float_value(OPTION_INFOS_SIZE); }
        public int font_rows() { return int_value(OPTION_FONT_ROWS); }
        public int hide_panels() { return int_value(OPTION_HIDE_PANELS); }

        public rgb_t border_color() { return rgb_value(OPTION_UI_BORDER_COLOR); }
        public rgb_t background_color() { return rgb_value(OPTION_UI_BACKGROUND_COLOR); }
        public rgb_t gfxviewer_bg_color() { return rgb_value(OPTION_UI_GFXVIEWER_BG_COLOR); }
        public rgb_t unavailable_color() { return rgb_value(OPTION_UI_UNAVAILABLE_COLOR); }
        public rgb_t text_color() { return rgb_value(OPTION_UI_TEXT_COLOR); }
        public rgb_t text_bg_color() { return rgb_value(OPTION_UI_TEXT_BG_COLOR); }
        public rgb_t subitem_color() { return rgb_value(OPTION_UI_SUBITEM_COLOR); }
        public rgb_t clone_color() { return rgb_value(OPTION_UI_CLONE_COLOR); }
        public rgb_t selected_color() { return rgb_value(OPTION_UI_SELECTED_COLOR); }
        public rgb_t selected_bg_color() { return rgb_value(OPTION_UI_SELECTED_BG_COLOR); }
        public rgb_t mouseover_color() { return rgb_value(OPTION_UI_MOUSEOVER_COLOR); }
        public rgb_t mouseover_bg_color() { return rgb_value(OPTION_UI_MOUSEOVER_BG_COLOR); }
        public rgb_t mousedown_color() { return rgb_value(OPTION_UI_MOUSEDOWN_COLOR); }
        public rgb_t mousedown_bg_color() { return rgb_value(OPTION_UI_MOUSEDOWN_BG_COLOR); }
        public rgb_t dipsw_color() { return rgb_value(OPTION_UI_DIPSW_COLOR); }
        public rgb_t slider_color() { return rgb_value(OPTION_UI_SLIDER_COLOR); }


        //-------------------------------------------------
        //  rgb_value - decode an RGB option
        //-------------------------------------------------
        rgb_t rgb_value(string option)
        {
            // find the entry
            core_options.entry entry = get_entry(option);  //core_options::entry::shared_const_ptr entry = get_entry(option);

            // look up the value, and sanity check the result
            string value = entry.value();
            int len = (int)std.strlen(value);
            if (len != 8)
                value = entry.default_value();

            // convert to an rgb_t
            return new rgb_t((uint32_t)std.strtoul(value, null, 16));
        }
    }
}
