// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using bitmap_vector = mame.std_vector<mame.bitmap_argb32>;
using cache_ptr_map = mame.std_map<mame.running_machine, mame.ui.menu_select_launch.cache>;
using flags_cache = mame.util.lru_cache_map<mame.game_driver, mame.ui.menu_select_launch.system_flags>;
using icon_cache = mame.util.lru_cache_map<mame.game_driver, System.Collections.Generic.KeyValuePair<mame.render_texture, mame.bitmap_argb32>>;
using osd_ticks_t = System.UInt64;
using s_bios = mame.std_vector<System.Collections.Generic.KeyValuePair<string, int>>;
using s_parts = mame.std_unordered_map<string, string>;
using texture_ptr_vector = mame.std_vector<mame.render_texture>;


namespace mame.ui
{
    abstract class menu_select_launch : menu
    {
        // tab navigation
        protected enum focused_menu
        {
            MAIN,
            LEFT,
            RIGHTTOP,
            RIGHTBOTTOM
        }


        public class system_flags
        {
            machine_flags.type m_machine_flags;
            emu.detail.device_feature.type m_unemulated_features;
            emu.detail.device_feature.type m_imperfect_features;
            bool m_has_keyboard;
            bool m_has_analog;
            rgb_t m_status_color;


            public system_flags(machine_static_info info)
            {
                m_machine_flags = info.machine_flags_get();
                m_unemulated_features = info.unemulated_features();
                m_imperfect_features = info.imperfect_features();
                m_has_keyboard = info.has_keyboard();
                m_has_analog = info.has_analog();
                m_status_color = info.status_color();
            }


            //system_flags(system_flags const &) = default;
            //system_flags(system_flags &&) = default;
            //system_flags &operator=(system_flags const &) = default;
            //system_flags &operator=(system_flags &&) = default;

            public machine_flags.type machine_flags() { return m_machine_flags; }
            public emu.detail.device_feature.type unemulated_features() { return m_unemulated_features; }
            public emu.detail.device_feature.type imperfect_features() { return m_imperfect_features; }
            public bool has_keyboard() { return m_has_keyboard; }
            public bool has_analog() { return m_has_analog; }
            public rgb_t status_color() { return m_status_color; }
        }


        public static class reselect_last
        {
            static string s_driver;
            static string s_software;
            static string s_swlist;
            static bool s_reselect = false;


            public static string driver() { return s_driver; }
            public static string software() { return s_software; }
            //static std::string const &swlist() { return s_swlist; }

            public static void reselect(bool value) { s_reselect = value; }
            public static bool get() { return s_reselect; }

            public static void reset()
            {
                s_driver = "";
                s_software = "";
                s_swlist = "";
                reselect(false);
            }

            public static void set_driver(string name)
            {
                s_driver = name;
                s_software = "";
                s_swlist = "";
            }

            public static void set_driver(game_driver driver) { set_driver(driver.name); }
            public static void set_software(game_driver driver, ui_software_info swinfo) { throw new emu_unimplemented(); }
        }


        public class cache
        {
            bitmap_argb32 m_snapx_bitmap;
            render_texture m_snapx_texture;
            game_driver m_snapx_driver;
            ui_software_info m_snapx_software;

            bitmap_argb32 m_no_avail_bitmap;
            bitmap_argb32 m_star_bitmap;
            render_texture m_star_texture;

            bitmap_vector m_toolbar_bitmap;
            bitmap_vector m_sw_toolbar_bitmap;
            texture_ptr_vector m_toolbar_texture;
            texture_ptr_vector m_sw_toolbar_texture;


            public cache(running_machine machine)
            {
                m_snapx_bitmap = new bitmap_argb32(0, 0);
                m_snapx_texture = null;
                m_snapx_driver = null;
                m_snapx_software = null;
                m_no_avail_bitmap = new bitmap_argb32(256, 256);
                m_star_bitmap = new bitmap_argb32(32, 32);
                m_star_texture = null;
                m_toolbar_bitmap = new bitmap_vector();
                m_sw_toolbar_bitmap = new bitmap_vector();
                m_toolbar_texture = new texture_ptr_vector();
                m_sw_toolbar_texture = new texture_ptr_vector();


                render_manager render = machine.render();

#if false
                auto const texture_free([&render](render_texture *texture) { render.texture_free(texture); });
#endif

                // create a texture for snapshot
                m_snapx_texture = render.texture_alloc(render_texture.hq_scale);  //, texture_free);

                //std::memcpy(m_no_avail_bitmap.pix32(0), no_avail_bmp, 256 * 256 * sizeof(uint32_t));
                RawBuffer m_no_avail_bitmapBuf;
                UInt32 m_no_avail_bitmapOffset = m_no_avail_bitmap.pix32(out m_no_avail_bitmapBuf, 0);
                for (int i = 0; i < 256 * 256; i++)  // sizeof(UInt32)
                    m_no_avail_bitmapBuf.set_uint32((int)m_no_avail_bitmapOffset + i, defimg_global.no_avail_bmp[i]);

                //std::memcpy(m_star_bitmap.pix32(0), favorite_star_bmp, 32 * 32 * sizeof(uint32_t));
                RawBuffer m_star_bitmapBuf;
                UInt32 m_star_bitmapOffset = m_star_bitmap.pix32(out m_star_bitmapBuf, 0);
                for (int i = 0; i < 32 * 32; i++)  // sizeof(UInt32)
                    m_star_bitmapBuf.set_uint32((int)m_star_bitmapOffset + i, starimg_global.favorite_star_bmp[i]);

                m_star_texture = render.texture_alloc();  //, texture_free);
                m_star_texture.set_bitmap(m_star_bitmap, m_star_bitmap.cliprect(), texture_format.TEXFORMAT_ARGB32);

                m_toolbar_bitmap.reserve(toolbar_global.UI_TOOLBAR_BUTTONS);
                m_sw_toolbar_bitmap.reserve(toolbar_global.UI_TOOLBAR_BUTTONS);
                m_toolbar_texture.reserve(toolbar_global.UI_TOOLBAR_BUTTONS);
                m_sw_toolbar_texture.reserve(toolbar_global.UI_TOOLBAR_BUTTONS);

                for (UInt32 i = 0; i < toolbar_global.UI_TOOLBAR_BUTTONS; ++i)
                {
                    m_toolbar_bitmap.emplace_back(new bitmap_argb32(32, 32));
                    m_sw_toolbar_bitmap.emplace_back(new bitmap_argb32(32, 32));
                    m_toolbar_texture.emplace_back(render.texture_alloc());  //, texture_free));
                    m_sw_toolbar_texture.emplace_back(render.texture_alloc());  //, texture_free));

                    //std::memcpy(m_toolbar_bitmap.back().pix32(0), toolbar_bitmap_bmp[i], 32 * 32 * sizeof(uint32_t));
                    RawBuffer m_toolbar_bitmapBuf;
                    UInt32 m_toolbar_bitmapOffset = m_toolbar_bitmap.back().pix32(out m_toolbar_bitmapBuf, 0);
                    for (int idx = 0; idx < 32 * 32; idx++)  // sizeof(UInt32)
                        m_toolbar_bitmapBuf.set_uint32((int)m_toolbar_bitmapOffset + idx, toolbar_global.toolbar_bitmap_bmp[i, idx]);

                    if (m_toolbar_bitmap.back().valid())
                        m_toolbar_texture.back().set_bitmap(m_toolbar_bitmap.back(), m_toolbar_bitmap.back().cliprect(), texture_format.TEXFORMAT_ARGB32);
                    else
                        m_toolbar_bitmap.back().reset();

                    if ((i == 0U) || (i == 2U))
                    {
                        //std::memcpy(&m_sw_toolbar_bitmap.back()->pix32(0), toolbar_bitmap_bmp[i], 32 * 32 * sizeof(uint32_t));
                        RawBuffer m_sw_toolbar_bitmapBuf;
                        UInt32 m_sw_toolbar_bitmapOffset = m_sw_toolbar_bitmap.back().pix32(out m_sw_toolbar_bitmapBuf, 0);
                        for (int idx = 0; idx < 32 * 32; idx++)  // sizeof(UInt32)
                            m_sw_toolbar_bitmapBuf.set_uint32((int)m_sw_toolbar_bitmapOffset + idx, toolbar_global.toolbar_bitmap_bmp[i, idx]);

                        if (m_sw_toolbar_bitmap.back().valid())
                            m_sw_toolbar_texture.back().set_bitmap(m_sw_toolbar_bitmap.back(), m_sw_toolbar_bitmap.back().cliprect(), texture_format.TEXFORMAT_ARGB32);
                        else
                            m_sw_toolbar_bitmap.back().reset();
                    }
                    else
                    {
                        m_sw_toolbar_bitmap.back().reset();
                    }
                }
            }

            public bitmap_argb32 snapx_bitmap() { return m_snapx_bitmap; }
            public render_texture snapx_texture() { return m_snapx_texture; }
            public bool snapx_driver_is(game_driver value) { return m_snapx_driver == value; }
            public bool snapx_software_is(ui_software_info software) { return m_snapx_software == software; }
            public void set_snapx_driver(game_driver value) { m_snapx_driver = value; }
            public void set_snapx_software(ui_software_info software) { m_snapx_software = software; }

            public bitmap_argb32 no_avail_bitmap() { return m_no_avail_bitmap; }
            public render_texture star_texture() { return m_star_texture; }

            public bitmap_vector toolbar_bitmap() { return m_toolbar_bitmap; }
            public bitmap_vector sw_toolbar_bitmap() { return m_sw_toolbar_bitmap; }
            public texture_ptr_vector toolbar_texture() { return m_toolbar_texture; }
            public texture_ptr_vector sw_toolbar_texture() { return m_sw_toolbar_texture; }
        }


        class software_parts : menu
        {
            ui_software_info m_uiinfo;
            s_parts m_parts;


            //-------------------------------------------------
            //  ctor
            //-------------------------------------------------
            software_parts(mame_ui_manager mui, render_container container, s_parts parts, ui_software_info ui_info)
                : base(mui, container)
            {
                m_uiinfo = ui_info;
                m_parts = parts;
            }


            protected override void custom_render(object selectedref, float top, float bottom, float x, float y, float x2, float y2) { throw new emu_unimplemented(); }


            //-------------------------------------------------
            //  populate
            //-------------------------------------------------
            protected override void populate(ref float customtop, ref float custombottom)
            {
                throw new emu_unimplemented();
            }


            //-------------------------------------------------
            //  handle
            //-------------------------------------------------
            protected override void handle()
            {
                // process the menu
                menu_event menu_event = process(0);
                if (menu_event != null && menu_event.iptkey == (int)ioport_type.IPT_UI_SELECT && menu_event.itemref != null)
                {
                    foreach (var elem in m_parts)
                    {
                        throw new emu_unimplemented();
                    }
                }
            }
        }


        class bios_selection : menu
        {
            object m_driver;  //void const  *m_driver;
            bool m_software;
            bool m_inlist;
            s_bios m_bios;


            //-------------------------------------------------
            //  ctor
            //-------------------------------------------------
            public bios_selection(mame_ui_manager mui, render_container container, s_bios biosname, game_driver driver, bool inlist)
                : this(mui, container, biosname, driver, false, inlist)
            {
            }

            public bios_selection(mame_ui_manager mui, render_container container, s_bios biosname, ui_software_info swinfo, bool inlist)
                : this(mui, container, biosname, swinfo, true, inlist)
            {
            }

            public bios_selection(mame_ui_manager mui, render_container container, s_bios biosname, object driver, bool software, bool inlist)
                : base(mui, container)
            {
                m_driver = driver;
                m_software = software;
                m_inlist = inlist;
                m_bios = biosname;
            }


            protected override void custom_render(object selectedref, float top, float bottom, float x, float y, float x2, float y2) { throw new emu_unimplemented(); }


            //-------------------------------------------------
            //  populate
            //-------------------------------------------------
            protected override void populate(ref float customtop, ref float custombottom)
            {
                foreach (var elem in m_bios)
                    item_append(elem.Key, "", 0, elem.Key);

                item_append(menu_item_type.SEPARATOR);
                customtop = ui().get_line_height() + (3.0f * ui_global.UI_BOX_TB_BORDER);
            }


            //-------------------------------------------------
            //  handle
            //-------------------------------------------------
            protected override void handle()
            {
                // process the menu
                menu_event menu_event = process(0);
                if (menu_event != null && menu_event.iptkey == (int)ioport_type.IPT_UI_SELECT && menu_event.itemref != null)
                {
                    foreach (var elem in m_bios)
                    {
                        throw new emu_unimplemented();
                    }
                }
            }
        }


        //using bitmap_vector = std::vector<bitmap_argb32>;
        //using texture_ptr_vector = std::vector<texture_ptr>;
        //using s_parts = std::unordered_map<std::string, std::string>;
        //using s_bios = std::vector<std::pair<std::string, int>>;
        //using cache_ptr = std::shared_ptr<cache>;
        //using cache_ptr_map = std::map<running_machine *, cache_ptr>;
        //using flags_cache = util::lru_cache_map<game_driver const *, system_flags>;
        //using icon_cache = util::lru_cache_map<game_driver const *, std::pair<texture_ptr, bitmap_argb32> >;


        const int MAX_ICONS_RENDER = 128;


        static readonly KeyValuePair<string, string> [] arts_info = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("Snapshots",       emu_options.OPTION_SNAPSHOT_DIRECTORY),
            new KeyValuePair<string, string>("Cabinets",        ui_options.OPTION_CABINETS_PATH),
            new KeyValuePair<string, string>("Control Panels",  ui_options.OPTION_CPANELS_PATH),
            new KeyValuePair<string, string>("PCBs",            ui_options.OPTION_PCBS_PATH),
            new KeyValuePair<string, string>("Flyers",          ui_options.OPTION_FLYERS_PATH),
            new KeyValuePair<string, string>("Titles",          ui_options.OPTION_TITLES_PATH),
            new KeyValuePair<string, string>("Ends",            ui_options.OPTION_ENDS_PATH),
            new KeyValuePair<string, string>("Artwork Preview", ui_options.OPTION_ARTPREV_PATH),
            new KeyValuePair<string, string>("Bosses",          ui_options.OPTION_BOSSES_PATH),
            new KeyValuePair<string, string>("Logos",           ui_options.OPTION_LOGOS_PATH),
            new KeyValuePair<string, string>("Versus",          ui_options.OPTION_VERSUS_PATH),
            new KeyValuePair<string, string>("Game Over",       ui_options.OPTION_GAMEOVER_PATH),
            new KeyValuePair<string, string>("HowTo",           ui_options.OPTION_HOWTO_PATH),
            new KeyValuePair<string, string>("Scores",          ui_options.OPTION_SCORES_PATH),
            new KeyValuePair<string, string>("Select",          ui_options.OPTION_SELECT_PATH),
            new KeyValuePair<string, string>("Marquees",        ui_options.OPTION_MARQUEES_PATH),
            new KeyValuePair<string, string>("Covers",          ui_options.OPTION_COVER_PATH),
        };


        static readonly string [] hover_msg =
        {
            "Add or remove favorites",
            "Export displayed list to file",
            "Show DATs view",
        };


        protected int visible_items;
        protected object m_prev_selected;  //void    *m_prev_selected;
        int m_total_lines;
        protected int m_topline_datsview;   // right box top line
        protected int m_filter_highlight;
        protected string m_search;


        bool m_ui_error;
        string m_error_text;

        game_driver m_info_driver;
        ui_software_info m_info_software;
        int m_info_view;
        std_vector<string> m_items_list;
        string m_info_buffer;


        cache m_cache;
        bool m_is_swlist;
        protected focused_menu m_focus;
        bool m_pressed;              // mouse button held down
        osd_ticks_t m_repeat;

        int m_right_visible_lines;  // right box lines

        flags_cache m_flags;
        icon_cache m_icons;

        //static std::mutex       s_cache_guard;
        static cache_ptr_map s_caches = new cache_ptr_map();


        public menu_select_launch(mame_ui_manager mui, render_container container, bool is_swlist)
            : base(mui, container)
        {
            m_prev_selected = null;
            m_total_lines = 0;
            m_topline_datsview = 0;
            m_filter_highlight = 0;
            m_ui_error = false;
            m_info_driver = null;
            m_info_software = null;
            m_info_view = -1;
            m_items_list = null;
            m_info_buffer = "";
            m_cache = null;
            m_is_swlist = is_swlist;
            m_focus = focused_menu.MAIN;
            m_pressed = false;
            m_repeat = 0;
            m_right_visible_lines = 0;
            m_flags = new flags_cache(256);
            m_icons = new icon_cache(MAX_ICONS_RENDER);


            // set up persistent cache for machine run
            {
                //throw new emu_unimplemented();
#if false
                std::lock_guard<std::mutex> guard(s_cache_guard);
#endif

                var found = s_caches.find(machine());
                if (found != null)
                {
                    global.assert(found != null);
                    m_cache = found;
                }
                else
                {
                    m_cache = new cache(machine());
                    s_caches.emplace(machine(), m_cache);
                    add_cleanup_callback(menu_select_launch.exit);
                }
            }
        }


        protected focused_menu get_focus() { return m_focus; }
        protected void set_focus(focused_menu focus) { m_focus = focus; }


        protected bool dismiss_error()
        {
            bool result = m_ui_error;
            if (result)
            {
                m_ui_error = false;
                m_error_text = "";
                machine().ui_input().reset();
            }
            return result;
        }


        protected void set_error(reset_options ropt, string message)
        {
            reset(ropt);
            m_ui_error = true;
            m_error_text = message;
        }


        //-------------------------------------------------
        //  get overall emulation status for a system
        //-------------------------------------------------
        protected system_flags get_system_flags(game_driver driver)
        {
            // try the cache
            var found = m_flags.find(driver);  //flags_cache::const_iterator const found(m_flags.find(&driver));
            if (null != found)
                return found;

            // aggregate flags
            emu_options clean_options = new emu_options();
            machine_config mconfig = new machine_config(driver, clean_options);

            var flags = new system_flags(new machine_static_info(mconfig));
            m_flags.emplace(driver, flags);
            return flags;  //.first.second;
        }


        protected void launch_system(game_driver driver) { launch_system(ui(), driver, null, null, 0); }
        //void launch_system(game_driver const &driver, ui_software_info const &swinfo) { launch_system(ui(), driver, &swinfo, nullptr, nullptr); }
        //void launch_system(game_driver const &driver, ui_software_info const &swinfo, std::string const &part) { launch_system(ui(), driver, &swinfo, &part, nullptr); }


        //-------------------------------------------------
        //  perform our special rendering
        //-------------------------------------------------
        protected override void custom_render(object selectedref, float top, float bottom, float origx1, float origy1, float origx2, float origy2)  // void * selectedref
        {
            string [] tempbuf = new string[5];

            // determine the text for the header
            make_topbox_text(out tempbuf[0], out tempbuf[1], out tempbuf[2]);

            float y1 = origy1 - 3.0f * ui_global.UI_BOX_TB_BORDER - ui().get_line_height();

            string [] tempbuf2 = new string[3] { tempbuf[0], tempbuf[1], tempbuf[2] };

            draw_text_box(
                    tempbuf2, //tempbuf, tempbuf + 3,
                    origx1, origx2, origy1 - top, y1,
                    text_layout.text_justify.CENTER, text_layout.word_wrapping.NEVER, true,
                    ui_global.UI_TEXT_COLOR, ui_global.UI_BACKGROUND_COLOR, 1.0f);

            // draw toolbar
            draw_toolbar(origx1, y1, origx2, origy1 - ui_global.UI_BOX_TB_BORDER);

            // determine the text to render below
            ui_software_info swinfo;
            game_driver driver;
            get_selection(out swinfo, out driver);

            bool isstar = false;
            rgb_t color = ui_global.UI_BACKGROUND_COLOR;
            if (swinfo != null && ((swinfo.startempty != 1) || driver == null))
            {
                isstar = mame_machine_manager.instance().favorite().isgame_favorite(swinfo);

                // first line is long name or system
                tempbuf[0] = make_software_description(swinfo);

                // next line is year, publisher
                tempbuf[1] = string.Format("{0}, {1}", swinfo.year, swinfo.publisher);  // "%1$s, %2$-.100s"

                // next line is parent/clone
                if (!swinfo.parentname.empty())
                    tempbuf[2] = string.Format("Software is clone of: {0}", !swinfo.parentlongname.empty() ? swinfo.parentlongname : swinfo.parentname);  // %1$-.100s
                else
                    tempbuf[2] = "Software is parent";

                // next line is supported status
                if (swinfo.supported == softlist_global.SOFTWARE_SUPPORTED_NO)
                {
                    tempbuf[3] = "Supported: No";
                    color = ui_global.UI_RED_COLOR;
                }
                else if (swinfo.supported == softlist_global.SOFTWARE_SUPPORTED_PARTIAL)
                {
                    tempbuf[3] = "Supported: Partial";
                    color = ui_global.UI_YELLOW_COLOR;
                }
                else
                {
                    tempbuf[3] = "Supported: Yes";
                    color = ui_global.UI_GREEN_COLOR;
                }

                // last line is romset name
                tempbuf[4] = string.Format("romset: {0}", swinfo.shortname);  // %1$-.100s
            }
            else if (driver != null)
            {
                isstar = mame_machine_manager.instance().favorite().isgame_favorite(driver);

                // first line is game description/game name
                tempbuf[0] = make_driver_description(driver);

                // next line is year, manufacturer
                tempbuf[1] = string.Format("{0}, {1}", driver.year, driver.manufacturer);  // %1$s, %2$-.100s

                // next line is clone/parent status
                int cloneof = driver_list.non_bios_clone(driver);

                if (cloneof != -1)
                    tempbuf[2] = string.Format("Driver is clone of: {0}", driver_list.driver((UInt32)cloneof).type.fullname());  // %1$-.100s
                else
                    tempbuf[2] = "Driver is parent";

                // next line is overall driver status
                system_flags flags = get_system_flags(driver);
                if ((flags.machine_flags() & machine_flags.type.NOT_WORKING) != 0)
                    tempbuf[3] = "Overall: NOT WORKING";
                else if (((flags.unemulated_features() | flags.imperfect_features()) & emu.detail.device_feature.type.PROTECTION) != 0)
                    tempbuf[3] = "Overall: Unemulated Protection";
                else
                    tempbuf[3] = "Overall: Working";

                // next line is graphics, sound status
                if ((flags.unemulated_features() & emu.detail.device_feature.type.GRAPHICS) != 0)
                    tempbuf[4] = "Graphics: Unimplemented, ";
                else if (((flags.unemulated_features() | flags.imperfect_features()) & (emu.detail.device_feature.type.GRAPHICS | emu.detail.device_feature.type.PALETTE)) != 0)
                    tempbuf[4] = "Graphics: Imperfect, ";
                else
                    tempbuf[4] = "Graphics: OK, ";

                if ((driver.flags & machine_flags.type.NO_SOUND_HW) != 0)
                    tempbuf[4] += "Sound: None";
                else if ((flags.unemulated_features() & emu.detail.device_feature.type.SOUND) != 0)
                    tempbuf[4] += "Sound: Unimplemented";
                else if ((flags.imperfect_features() & emu.detail.device_feature.type.SOUND) != 0)
                    tempbuf[4] += "Sound: Imperfect";
                else
                    tempbuf[4] += "Sound: OK";

                color = flags.status_color();
            }
            else
            {
                string copyright = emulator_info.get_copyright();
                int found = copyright.IndexOf("\n");

                tempbuf[0] = "";
                tempbuf[1] = string.Format("{0} {1}", emulator_info.get_appname(), version_global.build_version);  // %1$s %2$s
                tempbuf[2] = copyright.substr(0, found);
                tempbuf[3] = copyright.substr(found + 1);
                tempbuf[4] = "";
            }

            // draw the footer
            draw_text_box(
                    tempbuf, //std::begin(tempbuf), std::end(tempbuf),
                    origx1, origx2, origy2 + ui_global.UI_BOX_TB_BORDER, origy2 + bottom,
                    text_layout.text_justify.CENTER, text_layout.word_wrapping.NEVER, true,
                    ui_global.UI_TEXT_COLOR, color, 1.0f);

            // is favorite? draw the star
            if (isstar)
                draw_star(origx1 + ui_global.UI_BOX_LR_BORDER, origy2 + (2.0f * ui_global.UI_BOX_TB_BORDER));
        }


        // handlers
        protected void inkey_navigation()
        {
            switch (get_focus())
            {
            case focused_menu.MAIN:
                if (selected <= visible_items)
                {
                    m_prev_selected = get_selection_ref();
                    selected = visible_items + 1;
                }
                else
                {
                    if (ui_globals.panels_status != (UInt16)HIDE.HIDE_LEFT_PANEL)
                    {
                        set_focus(focused_menu.LEFT);
                    }
                    else if (ui_globals.panels_status == (UInt16)HIDE.HIDE_BOTH)
                    {
                        for (int x = 0; x < item.size(); ++x)
                            if (item[x].refobj == m_prev_selected)
                                selected = x;
                    }
                    else
                    {
                        set_focus(focused_menu.RIGHTTOP);
                    }
                }
                break;

            case focused_menu.LEFT:
                if (ui_globals.panels_status != (UInt16)HIDE.HIDE_RIGHT_PANEL)
                {
                    set_focus(focused_menu.RIGHTTOP);
                }
                else
                {
                    set_focus(focused_menu.MAIN);
                    select_prev();
                }
                break;

            case focused_menu.RIGHTTOP:
                set_focus(focused_menu.RIGHTBOTTOM);
                break;

            case focused_menu.RIGHTBOTTOM:
                set_focus(focused_menu.MAIN);
                select_prev();
                break;
            }
        }


        protected abstract void inkey_export();


        protected void inkey_dats()
        {
            ui_software_info software;
            game_driver driver;
            get_selection(out software, out driver);

            throw new emu_unimplemented();
        }


        // draw arrow

        //-------------------------------------------------
        //  draw common arrows
        //-------------------------------------------------
        void draw_common_arrow(float origx1, float origy1, float origx2, float origy2, int current, int dmin, int dmax, float title_size)
        {
            var line_height = ui().get_line_height();
            var lr_arrow_width = 0.4f * line_height * machine().render().ui_aspect();
            var gutter_width = lr_arrow_width * 1.3f;

            // set left-right arrows dimension
            float ar_x0 = 0.5f * (origx2 + origx1) + 0.5f * title_size + gutter_width - lr_arrow_width;
            float ar_y0 = origy1 + 0.1f * line_height;
            float ar_x1 = 0.5f * (origx2 + origx1) + 0.5f * title_size + gutter_width;
            float ar_y1 = origy1 + 0.9f * line_height;

            float al_x0 = 0.5f * (origx2 + origx1) - 0.5f * title_size - gutter_width;
            float al_y0 = origy1 + 0.1f * line_height;
            float al_x1 = 0.5f * (origx2 + origx1) - 0.5f * title_size - gutter_width + lr_arrow_width;
            float al_y1 = origy1 + 0.9f * line_height;

            rgb_t fgcolor_right, fgcolor_left;
            fgcolor_right = fgcolor_left = ui_global.UI_TEXT_COLOR;

            // set hover
            if (mouse_in_rect(ar_x0, ar_y0, ar_x1, ar_y1) && current != dmax)
            {
                ui().draw_textured_box(container(), ar_x0 + 0.01f, ar_y0, ar_x1 - 0.01f, ar_y1, ui_global.UI_MOUSEOVER_BG_COLOR, new rgb_t(43, 43, 43),
                        hilight_main_texture(), global.PRIMFLAG_BLENDMODE((UInt32)BLENDMODE.BLENDMODE_ALPHA) | global.PRIMFLAG_TEXWRAP(1));
                hover = (int)HOVER.HOVER_UI_RIGHT;
                fgcolor_right = ui_global.UI_MOUSEOVER_COLOR;
            }
            else if (mouse_in_rect(al_x0, al_y0, al_x1, al_y1) && current != dmin)
            {
                ui().draw_textured_box(container(), al_x0 + 0.01f, al_y0, al_x1 - 0.01f, al_y1, ui_global.UI_MOUSEOVER_BG_COLOR, new rgb_t(43, 43, 43),
                        hilight_main_texture(), global.PRIMFLAG_BLENDMODE((UInt32)BLENDMODE.BLENDMODE_ALPHA) | global.PRIMFLAG_TEXWRAP(1));
                hover = (int)HOVER.HOVER_UI_LEFT;
                fgcolor_left = ui_global.UI_MOUSEOVER_COLOR;
            }

            // apply arrow
            if (dmax == dmin)
                return;
            else if (current == dmin)
                draw_arrow(ar_x0, ar_y0, ar_x1, ar_y1, fgcolor_right, emucore_global.ROT90);
            else if (current == dmax)
                draw_arrow(al_x0, al_y0, al_x1, al_y1, fgcolor_left, emucore_global.ROT90 ^ emucore_global.ORIENTATION_FLIP_X);
            else
            {
                draw_arrow(ar_x0, ar_y0, ar_x1, ar_y1, fgcolor_right, emucore_global.ROT90);
                draw_arrow(al_x0, al_y0, al_x1, al_y1, fgcolor_left, emucore_global.ROT90 ^ emucore_global.ORIENTATION_FLIP_X);
            }
        }


        //-------------------------------------------------
        //  draw info arrow
        //-------------------------------------------------
        void draw_info_arrow(int ub, float origx1, float origx2, float oy1, float line_height, float text_size, float ud_arrow_width)
        {
            rgb_t fgcolor = ui_global.UI_TEXT_COLOR;
            UInt32 orientation = (ub == 0) ? emucore_global.ROT0 : emucore_global.ROT0 ^ emucore_global.ORIENTATION_FLIP_Y;

            if (mouse_in_rect(origx1, oy1, origx2, oy1 + (line_height * text_size)))
            {
                ui().draw_textured_box(container(), origx1 + 0.01f, oy1, origx2 - 0.01f, oy1 + (line_height * text_size), ui_global.UI_MOUSEOVER_BG_COLOR,
                        new rgb_t(43, 43, 43), hilight_main_texture(), global.PRIMFLAG_BLENDMODE((UInt32)BLENDMODE.BLENDMODE_ALPHA) | global.PRIMFLAG_TEXWRAP(1));
                hover = (ub == 0) ? (int)HOVER.HOVER_DAT_UP : (int)HOVER.HOVER_DAT_DOWN;
                fgcolor = ui_global.UI_MOUSEOVER_COLOR;
            }

            draw_arrow(0.5f * (origx1 + origx2) - 0.5f * (ud_arrow_width * text_size), oy1 + 0.25f * (line_height * text_size),
                    0.5f * (origx1 + origx2) + 0.5f * (ud_arrow_width * text_size), oy1 + 0.75f * (line_height * text_size), fgcolor, orientation);
        }


        protected bool draw_error_text()
        {
            if (m_ui_error)
                ui().draw_text_box(container(), m_error_text.c_str(), text_layout.text_justify.CENTER, 0.5f, 0.5f, ui_global.UI_RED_COLOR);

            return m_ui_error;
        }


        //template <typename Filter>
        protected float draw_left_panel(
                machine_filter.type current,
                std_map<machine_filter.type, machine_filter> filters,
                float x1, float y1, float x2, float y2)
        {
            if ((ui_globals.panels_status != (UInt16)HIDE.SHOW_PANELS) && (ui_globals.panels_status != (UInt16)HIDE.HIDE_RIGHT_PANEL))
                return draw_collapsed_left_panel(x1, y1, x2, y2);

            // calculate line height
            float line_height = ui().get_line_height();
            float text_size = ui().options().infos_size();
            float sc = y2 - y1 - (2.0f * ui_global.UI_BOX_TB_BORDER);
            float line_height_max = line_height * text_size;
            if (((float)machine_filter.type.COUNT * line_height_max) > sc)
            {
                float lm = sc / (float)machine_filter.type.COUNT;
                line_height_max = line_height * (lm / line_height);
            }

            // calculate horizontal offset for unadorned names
            string tmp = "_# ";
            render_font.convert_command_glyph(ref tmp);
            float text_sign = ui().get_string_width(tmp.c_str(), text_size);

            // get the maximum width of a filter name
            float left_width = 0.0f;
            for (machine_filter.type x = machine_filter.type.FIRST; machine_filter.type.COUNT > x; ++x)
                left_width = Math.Max(ui().get_string_width(machine_filter.display_name(x), text_size) + text_sign, left_width);

            // outline the box and inset by the border width
            float origy1 = y1;
            float origy2 = y2;
            x2 = x1 + left_width + 2.0f * ui_global.UI_BOX_LR_BORDER;
            ui().draw_outlined_box(container(), x1, y1, x2, y2, ui_global.UI_BACKGROUND_COLOR);
            x1 += ui_global.UI_BOX_LR_BORDER;
            x2 -= ui_global.UI_BOX_LR_BORDER;
            y1 += ui_global.UI_BOX_TB_BORDER;
            y2 -= ui_global.UI_BOX_TB_BORDER;

            // now draw the rows
            var active_filter = filters.find(current);
            for (machine_filter.type filter = machine_filter.type.FIRST; machine_filter.type.COUNT > filter; ++filter)
            {
                string str = "";
                if (null != active_filter)
                {
                    str = active_filter.adorned_display_name(filter);
                }
                else
                {
                    if (current == filter)
                    {
                        str = "_> ";
                        render_font.convert_command_glyph(ref str);
                    }
                    str += machine_filter.display_name(filter);
                }

                // handle mouse hover in passing
                rgb_t bgcolor = ui_global.UI_TEXT_BG_COLOR;
                rgb_t fgcolor = ui_global.UI_TEXT_COLOR;
                if (mouse_in_rect(x1, y1, x2, y1 + line_height_max))
                {
                    bgcolor = ui_global.UI_MOUSEOVER_BG_COLOR;
                    fgcolor = ui_global.UI_MOUSEOVER_COLOR;
                    hover = (int)HOVER.HOVER_FILTER_FIRST + (int)filter;
                    highlight(x1, y1, x2, y1 + line_height_max, bgcolor);
                }

                // draw primary highlight if keyboard focus is here
                if ((m_filter_highlight == (int)filter) && (get_focus() == focused_menu.LEFT))
                {
                    fgcolor = new rgb_t(0xff, 0xff, 0xff, 0x00);
                    bgcolor = new rgb_t(0xff, 0xff, 0xff, 0xff);
                    ui().draw_textured_box(
                            container(),
                            x1, y1, x2, y1 + line_height_max,
                            bgcolor, new rgb_t(255, 43, 43, 43),
                            hilight_main_texture(), global.PRIMFLAG_BLENDMODE((UInt32)BLENDMODE.BLENDMODE_ALPHA) | global.PRIMFLAG_TEXWRAP(1));
                }

                // finally draw the text itself and move to the next line
                float x1t = x1 + ((str == machine_filter.display_name(filter)) ? text_sign : 0.0f);
                float unused1;
                float unused2;
                ui().draw_text_full(
                        container(), str.c_str(),
                        x1t, y1, x2 - x1,
                        text_layout.text_justify.LEFT, text_layout.word_wrapping.NEVER,
                        mame_ui_manager.draw_mode.NORMAL, fgcolor, bgcolor,
                        out unused1, out unused2, text_size);
                y1 += line_height_max;
            }

            x1 = x2 + ui_global.UI_BOX_LR_BORDER;
            x2 = x1 + 2.0f * ui_global.UI_BOX_LR_BORDER;
            y1 = origy1;
            y2 = origy2;
            float space = x2 - x1;
            float lr_arrow_width = 0.4f * space * machine().render().ui_aspect();

            // set left-right arrows dimension
            float ar_x0 = 0.5f * (x2 + x1) - 0.5f * lr_arrow_width;
            float ar_y0 = 0.5f * (y2 + y1) + 0.1f * space;
            float ar_x1 = ar_x0 + lr_arrow_width;
            float ar_y1 = 0.5f * (y2 + y1) + 0.9f * space;

            ui().draw_outlined_box(container(), x1, y1, x2, y2, new rgb_t(0xef, 0x12, 0x47, 0x7b));

            rgb_t fgcolor2 = ui_global.UI_TEXT_COLOR;
            if (mouse_in_rect(x1, y1, x2, y2))
            {
                fgcolor2 = ui_global.UI_MOUSEOVER_COLOR;
                hover = (int)HOVER.HOVER_LPANEL_ARROW;
            }

            draw_arrow(ar_x0, ar_y0, ar_x1, ar_y1, fgcolor2, emucore_global.ROT90 ^ emucore_global.ORIENTATION_FLIP_X);
            return x2 + ui_global.UI_BOX_LR_BORDER;
        }


        //template <typename T>
        protected bool select_bios(game_driver driver, bool inlist)
        {
            //throw new emu_unimplemented();
            return false;
#if false
            s_bios biosname;
            if (ui().options().skip_bios_menu() || !has_multiple_bios(driver, biosname))
                return false;

            menu.stack_push(new bios_selection(ui(), container(), biosname, driver, inlist));
            return true;
#endif
        }


        //bool select_part(software_info const &info, ui_software_info const &ui_info);


        protected object get_selection_ptr()
        {
            object selected_ref = get_selection_ref();

            //return (uintptr_t(selected_ref) > skip_main_items) ? selected_ref : m_prev_selected;
            if (selected_ref == null)
            {
                if (m_prev_selected is ui_system_info)
                    return ((ui_system_info)m_prev_selected).driver;  //m_prev_selected;
                else
                    throw new emu_unimplemented();  // put a check for whatever type is the ref object
            }
            else if (selected_ref is int)  // || selected_ref is CONF)
            {
                if ((int)selected_ref > skip_main_items)
                    return selected_ref;
                else if (m_prev_selected is ui_system_info)
                    return ((ui_system_info)m_prev_selected).driver;  //m_prev_selected;
                else
                    throw new emu_unimplemented();  // put a check for whatever type is the ref object
            }
            else if (selected_ref is ui_system_info)
            {
                return ((ui_system_info)selected_ref).driver;
            }
            else
            {
                throw new emu_unimplemented();  // put a check for whatever type is the ref object
            }
        }


        void reset_pressed() { m_pressed = false; m_repeat = 0; }
        bool mouse_pressed() { return (osdcore_global.m_osdcore.osd_ticks() >= m_repeat); }


        void set_pressed()
        {
            if (m_repeat == 0)
                m_repeat = osdcore_global.m_osdcore.osd_ticks() + osdcore_global.m_osdcore.osd_ticks_per_second() / 2;
            else
                m_repeat = osdcore_global.m_osdcore.osd_ticks() + osdcore_global.m_osdcore.osd_ticks_per_second() / 4;
            m_pressed = true;
        }


        bool snapx_valid() { return m_cache.snapx_bitmap().valid(); }


        // draw left panel
        protected abstract float draw_left_panel(float x1, float y1, float x2, float y2);


        //-------------------------------------------------
        //  draw collapsed left panel
        //-------------------------------------------------
        float draw_collapsed_left_panel(float x1, float y1, float x2, float y2)
        {
            float space = x2 - x1;
            float lr_arrow_width = 0.4f * space * machine().render().ui_aspect();

            // set left-right arrows dimension
            float ar_x0 = 0.5f * (x2 + x1) - (0.5f * lr_arrow_width);
            float ar_y0 = 0.5f * (y2 + y1) + (0.1f * space);
            float ar_x1 = ar_x0 + lr_arrow_width;
            float ar_y1 = 0.5f * (y2 + y1) + (0.9f * space);

            ui().draw_outlined_box(container(), x1, y1, x2, y2, new rgb_t(0xef, 0x12, 0x47, 0x7b)); // FIXME: magic numbers in colour?

            rgb_t fgcolor = ui_global.UI_TEXT_COLOR;
            if (mouse_in_rect(x1, y1, x2, y2))
            {
                fgcolor = ui_global.UI_MOUSEOVER_COLOR;
                hover = (int)HOVER.HOVER_LPANEL_ARROW;
            }

            draw_arrow(ar_x0, ar_y0, ar_x1, ar_y1, fgcolor, emucore_global.ROT90);

            return x2 + ui_global.UI_BOX_LR_BORDER;
        }


        // draw infos
        //-------------------------------------------------
        //  draw infos
        //-------------------------------------------------
        void infos_render(float origx1, float origy1, float origx2, float origy2)
        {
            float line_height = ui().get_line_height();
            float text_size = ui().options().infos_size();
            std_vector<int> xstart;
            std_vector<int> xend;
            string first = "";
            ui_software_info software;
            game_driver driver;
            int total;
            get_selection(out software, out driver);

            if (software != null && ((software.startempty == 0) || driver == null))
            {
                m_info_driver = null;
                first = "Usage";

                if (m_info_software != software || m_info_view != ui_globals.cur_sw_dats_view)
                {
                    m_info_buffer = "";
                    if (software == m_info_software)
                    {
                        m_info_view = ui_globals.cur_sw_dats_view;
                    }
                    else
                    {
                        m_info_view = 0;
                        m_info_software = software;
                        ui_globals.cur_sw_dats_view = 0;

                        m_items_list.clear();
                        mame_machine_manager.instance().lua().call_plugin("data_list", software.shortname + "," + software.listname, out m_items_list);
                        ui_globals.cur_sw_dats_total = (byte)(m_items_list.size() + 1);
                    }

                    if (m_info_view == 0)
                    {
                        m_info_buffer = software.usage;
                    }
                    else
                    {
                        m_info_buffer = "";
                        mame_machine_manager.instance().lua().call_plugin("data", m_info_view - 1, out m_info_buffer);
                    }
                }
                total = ui_globals.cur_sw_dats_total;
            }
            else if (driver != null)
            {
                m_info_software = null;
                first = "General Info";

                if (driver != m_info_driver || ui_globals.curdats_view != m_info_view)
                {
                    m_info_buffer = "";
                    if (driver == m_info_driver)
                    {
                        m_info_view = ui_globals.curdats_view;
                    }
                    else
                    {
                        m_info_driver = driver;
                        m_info_view = 0;
                        ui_globals.curdats_view = 0;

                        m_items_list.clear();
                        mame_machine_manager.instance().lua().call_plugin("data_list", driver.name, out m_items_list);
                        ui_globals.curdats_total = (byte)(m_items_list.size() + 1);
                    }

                    if (m_info_view == 0)
                    {
                        general_info(driver, out m_info_buffer);
                    }
                    else
                    {
                        m_info_buffer = "";
                        mame_machine_manager.instance().lua().call_plugin("data", m_info_view - 1, out m_info_buffer);
                    }
                }

                total = ui_globals.curdats_total;
            }
            else
            {
                return;
            }

            origy1 += ui_global.UI_BOX_TB_BORDER;
            float gutter_width = 0.4f * line_height * machine().render().ui_aspect() * 1.3f;
            float ud_arrow_width = line_height * machine().render().ui_aspect();
            float oy1 = origy1 + line_height;

            string snaptext = m_info_view != 0 ? m_items_list[m_info_view - 1].c_str() : first;

            // get width of widest title
            float title_size = 0.0f;
            for (UInt32 x = 0; total > x; ++x)
            {
                string name = x != 0 ? m_items_list[(int)x - 1].c_str() : first;
                float txt_length = 0.0f;
                float unused;
                ui().draw_text_full(
                        container(), name,
                        origx1, origy1, origx2 - origx1,
                        text_layout.text_justify.CENTER, text_layout.word_wrapping.NEVER,
                        mame_ui_manager.draw_mode.NONE, ui_global.UI_TEXT_COLOR, ui_global.UI_TEXT_BG_COLOR,
                        out txt_length, out unused);
                txt_length += 0.01f;
                title_size = Math.Max(txt_length, title_size);
            }

            rgb_t fgcolor = ui_global.UI_TEXT_COLOR;
            rgb_t bgcolor = ui_global.UI_TEXT_BG_COLOR;
            if (get_focus() == focused_menu.RIGHTBOTTOM)
            {
                fgcolor = new rgb_t(0xff, 0xff, 0xff, 0x00);
                bgcolor = new rgb_t(0xff, 0xff, 0xff, 0xff);
            }

            float middle = origx2 - origx1;

            // check size
            float sc = title_size + 2.0f * gutter_width;
            float tmp_size = (sc > middle) ? ((middle - 2.0f * gutter_width) / sc) : 1.0f;
            title_size *= tmp_size;

            if (bgcolor != ui_global.UI_TEXT_BG_COLOR)
            {
                ui().draw_textured_box(container(), origx1 + ((middle - title_size) * 0.5f), origy1, origx1 + ((middle + title_size) * 0.5f),
                        origy1 + line_height, bgcolor, new rgb_t(255, 43, 43, 43), hilight_main_texture(), global.PRIMFLAG_BLENDMODE((UInt32)BLENDMODE.BLENDMODE_ALPHA) | global.PRIMFLAG_TEXWRAP(1));
            }

            float unused1;
            float unused2;
            ui().draw_text_full(container(), snaptext.c_str(), origx1, origy1, origx2 - origx1, text_layout.text_justify.CENTER,
                    text_layout.word_wrapping.NEVER, mame_ui_manager.draw_mode.NORMAL, fgcolor, bgcolor, out unused1, out unused2, tmp_size);

            char justify = 'l'; // left justify
            if ((m_info_buffer.length() >= 3) && (m_info_buffer[0] == '#'))
            {
                if (m_info_buffer[1] == 'j')
                    justify = m_info_buffer[2];
            }

            draw_common_arrow(origx1, origy1, origx2, origy2, m_info_view, 0, total - 1, title_size);
            if (justify == 'f')
            {
                m_total_lines = ui().wrap_text(
                        container(), m_info_buffer.c_str(),
                        0.0f, 0.0f, 1.0f - (2.0f * gutter_width),
                        out xstart, out xend,
                        text_size);
            }
            else
            {
                m_total_lines = ui().wrap_text(
                        container(), m_info_buffer.c_str(),
                        origx1, origy1, origx2 - origx1 - (2.0f * gutter_width),
                        out xstart, out xend,
                        text_size);
            }

            int r_visible_lines = (int)Math.Floor((origy2 - oy1) / (line_height * text_size));
            if (m_total_lines < r_visible_lines)
                r_visible_lines = m_total_lines;
            if (m_topline_datsview < 0)
                m_topline_datsview = 0;
            if (m_topline_datsview + r_visible_lines >= m_total_lines)
                m_topline_datsview = m_total_lines - r_visible_lines;

            if (mouse_in_rect(origx1 + gutter_width, oy1, origx2 - gutter_width, origy2))
                hover = (int)HOVER.HOVER_INFO_TEXT;

            sc = origx2 - origx1 - (2.0f * gutter_width);
            for (int r = 0; r < r_visible_lines; ++r)
            {
                int itemline = r + m_topline_datsview;
                string tempbuf = m_info_buffer.substr(xstart[itemline], xend[itemline] - xstart[itemline]);
                if (tempbuf[0] == '#')
                    continue;

                if (r == 0 && m_topline_datsview != 0) // up arrow
                {
                    draw_info_arrow(0, origx1, origx2, oy1, line_height, text_size, ud_arrow_width);
                }
                else if (r == r_visible_lines - 1 && itemline != m_total_lines - 1) // bottom arrow
                {
                    draw_info_arrow(1, origx1, origx2, oy1, line_height, text_size, ud_arrow_width);
                }
                else if (justify == '2') // two-column layout
                {
                    // split at first tab
                    int splitpos = tempbuf.find("\t");
                    string leftcol = tempbuf.substr(0, (-1 == splitpos) ? 0 : splitpos);
                    string rightcol = tempbuf.substr((-1 == splitpos) ? 0 : (splitpos + 1));

                    // measure space needed, condense if necessary
                    float leftlen = ui().get_string_width(leftcol.c_str(), text_size);
                    float rightlen = ui().get_string_width(rightcol.c_str(), text_size);
                    float textlen = leftlen + rightlen;
                    float tmp_size3 = (textlen > sc) ? (text_size * (sc / textlen)) : text_size;

                    // draw in two parts
                    ui().draw_text_full(
                            container(), leftcol.c_str(),
                            origx1 + gutter_width, oy1, sc,
                            text_layout.text_justify.LEFT, text_layout.word_wrapping.TRUNCATE,
                            mame_ui_manager.draw_mode.NORMAL, ui_global.UI_TEXT_COLOR, ui_global.UI_TEXT_BG_COLOR,
                            out unused1, out unused2,
                            tmp_size3);
                    ui().draw_text_full(
                            container(), rightcol.c_str(),
                            origx1 + gutter_width, oy1, sc,
                            text_layout.text_justify.RIGHT, text_layout.word_wrapping.TRUNCATE,
                            mame_ui_manager.draw_mode.NORMAL, ui_global.UI_TEXT_COLOR, ui_global.UI_TEXT_BG_COLOR,
                            out unused1, out unused2,
                            tmp_size3);
                }
                else if (justify == 'f' || justify == 'p') // full or partial justify
                {
                    // check size
                    float textlen = ui().get_string_width(tempbuf.c_str(), text_size);
                    float tmp_size3 = (textlen > sc) ? text_size * (sc / textlen) : text_size;
                    ui().draw_text_full(
                            container(), tempbuf.c_str(),
                            origx1 + gutter_width, oy1, origx2 - origx1,
                            text_layout.text_justify.LEFT, text_layout.word_wrapping.TRUNCATE,
                            mame_ui_manager.draw_mode.NORMAL, ui_global.UI_TEXT_COLOR, ui_global.UI_TEXT_BG_COLOR,
                            out unused1, out unused2,
                            tmp_size3);
                }
                else
                {
                    ui().draw_text_full(
                            container(), tempbuf.c_str(),
                            origx1 + gutter_width, oy1, origx2 - origx1,
                            text_layout.text_justify.LEFT, text_layout.word_wrapping.TRUNCATE,
                            mame_ui_manager.draw_mode.NORMAL, ui_global.UI_TEXT_COLOR, ui_global.UI_TEXT_BG_COLOR,
                            out unused1, out unused2,
                            text_size);
                }

                oy1 += (line_height * text_size);
            }
            // return the number of visible lines, minus 1 for top arrow and 1 for bottom arrow
            m_right_visible_lines = r_visible_lines - ((m_topline_datsview != 0) ? 1 : 0) - ((m_topline_datsview + r_visible_lines != m_total_lines) ? 1 : 0);
        }

        protected abstract void general_info(game_driver driver, out string buffer);

        // get selected software and/or driver
        protected abstract void get_selection(out ui_software_info software, out game_driver driver);


        protected virtual bool accept_search() { return true; }


        void select_prev()
        {
            if (m_prev_selected == null)
            {
                selected = 0;
            }
            else
            {
                for (int x = 0; x < item.size(); ++x)
                {
                    if (item[x].refobj == m_prev_selected)
                    {
                        selected = x;
                        break;
                    }
                }
            }
        }


        //-------------------------------------------------
        //  draw toolbar
        //-------------------------------------------------
        void draw_toolbar(float x1, float y1, float x2, float y2)
        {
            // draw a box
            ui().draw_outlined_box(container(), x1, y1, x2, y2, new rgb_t(0xEF, 0x12, 0x47, 0x7B));

            // take off the borders
            x1 += ui_global.UI_BOX_LR_BORDER;
            x2 -= ui_global.UI_BOX_LR_BORDER;
            y1 += ui_global.UI_BOX_TB_BORDER;
            y2 -= ui_global.UI_BOX_TB_BORDER;

            texture_ptr_vector t_texture = m_is_swlist ? m_cache.sw_toolbar_texture() : m_cache.toolbar_texture();
            bitmap_vector t_bitmap = m_is_swlist ? m_cache.sw_toolbar_bitmap() : m_cache.toolbar_bitmap();

            //var num_valid = std::count_if(std::begin(t_bitmap), std::end(t_bitmap), [](bitmap_ptr const &e) { return e && e->valid(); });
            int num_valid = 0;
            foreach (var e in t_bitmap)
                num_valid += (e != null && e.valid()) ? 1 : 0;

            float space_x = (y2 - y1) * container().manager().ui_aspect(container());
            float total = ((float)num_valid * space_x) + ((float)(num_valid - 1) * 0.001f);
            x1 += (x2 - x1) * 0.5f - total * 0.5f;
            x2 = x1 + space_x;

            for (int z = 0; z < toolbar_global.UI_TOOLBAR_BUTTONS; ++z)
            {
                if (t_bitmap[z] != null && t_bitmap[z].valid())
                {
                    rgb_t color = new rgb_t(0xEFEFEFEF);
                    if (mouse_in_rect(x1, y1, x2, y2))
                    {
                        hover = (int)(HOVER.HOVER_B_FAV + z);
                        color = rgb_t.white();
                        float ypos = y2 + ui().get_line_height() + 2.0f * ui_global.UI_BOX_TB_BORDER;
                        ui().draw_text_box(container(), hover_msg[z], text_layout.text_justify.CENTER, 0.5f, ypos, ui_global.UI_BACKGROUND_COLOR);
                    }

                    container().add_quad(x1, y1, x2, y2, color, t_texture[z], global.PRIMFLAG_BLENDMODE((UInt32)BLENDMODE.BLENDMODE_ALPHA));
                    x1 += space_x + ((z < toolbar_global.UI_TOOLBAR_BUTTONS - 1) ? 0.001f : 0.0f);
                    x2 = x1 + space_x;
                }
            }
        }


        //-------------------------------------------------
        //  draw favorites star
        //-------------------------------------------------
        void draw_star(float x0, float y0)
        {
            float y1 = y0 + ui().get_line_height();
            float x1 = x0 + ui().get_line_height() * container().manager().ui_aspect();
            container().add_quad(x0, y0, x1, y1, rgb_t.white(), m_cache.star_texture(), global.PRIMFLAG_BLENDMODE((UInt32)BLENDMODE.BLENDMODE_ALPHA) | global.PRIMFLAG_PACKABLE);
        }


        //-------------------------------------------------
        //  draw icons
        //-------------------------------------------------
        float draw_icon(int linenum, object selectedref, float x0, float y0)
        {
            if (!ui_globals.has_icons || m_is_swlist)
                return 0.0f;

            float ud_arrow_width = ui().get_line_height() * container().manager().ui_aspect(container());
            game_driver driver = null;

            if ((item[0].flags & (UInt32)FLAG.FLAG_UI_FAVORITE) != 0)
            {
                ui_software_info soft = (ui_software_info)selectedref;
                if (soft.startempty == 1)
                    driver = soft.driver;
            }
            else
            {
                driver = (game_driver)selectedref;
            }

            var x1 = x0 + ud_arrow_width;
            var y1 = y0 + ui().get_line_height();

            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  get title and search path for right panel
        //-------------------------------------------------
        void get_title_search(out string snaptext, out string searchstr)
        {
            // get arts title text
            snaptext = arts_info[ui_globals.curimage_view].Key;

            // get search path
            string addpath;
            if (ui_globals.curimage_view == (byte)VIEW.SNAPSHOT_VIEW)
            {
                emu_options moptions = new emu_options();
                searchstr = machine().options().value(arts_info[ui_globals.curimage_view].Value);
                addpath = moptions.value(arts_info[ui_globals.curimage_view].Value);
            }
            else
            {
                ui_options moptions = new ui_options();
                searchstr = ui().options().value(arts_info[ui_globals.curimage_view].Value);
                addpath = moptions.value(arts_info[ui_globals.curimage_view].Value);
            }

            string tmp = searchstr;
            path_iterator path = new path_iterator(tmp.c_str());
            path_iterator path_iter = new path_iterator(addpath.c_str());
            string c_path;
            string curpath;

            // iterate over path and add path for zipped formats
            while (path.next(out curpath))
            {
                path_iter.reset();
                while (path_iter.next(out c_path))
                    searchstr += ";" + curpath + osdcore_global.PATH_SEPARATOR + c_path;
            }
        }


        // handle keys
        //-------------------------------------------------
        //  handle keys for main menu
        //-------------------------------------------------
        protected override void handle_keys(UInt32 flags, ref int iptkey)
        {
            bool ignorepause = stack_has_special_main_menu();

            // bail if no items
            if (item.size() == 0)
                return;

            // if we hit select, return true or pop the stack, depending on the item
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_SELECT, 0))
            {
                if (m_ui_error)
                {
                    // dismiss error
                }
                else if (m_focus == focused_menu.LEFT)
                {
                    m_prev_selected = null;
                    filter_selected();
                }

                if (is_last_selected() && m_focus == focused_menu.MAIN)
                {
                    iptkey = (int)ioport_type.IPT_UI_CANCEL;
                    stack_pop();
                }
                return;
            }

            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_CANCEL, 0))
            {
                if (m_ui_error)
                {
                    // dismiss error
                }
                else if (menu_has_search_active())
                {
                    // escape pressed with non-empty search text clears it
                    m_search = "";
                    reset(reset_options.SELECT_FIRST);
                }
                else
                {
                    // otherwise pop the stack
                    stack_pop();
                }
                return;
            }

            // validate the current selection
            validate_selection(1);

            // swallow left/right keys if they are not appropriate
            bool ignoreleft = ((selected_item().flags & (UInt32)FLAG.FLAG_LEFT_ARROW) == 0);
            bool ignoreright = ((selected_item().flags & (UInt32)FLAG.FLAG_RIGHT_ARROW) == 0);
            bool leftclose = (ui_globals.panels_status == (UInt16)HIDE.HIDE_BOTH || ui_globals.panels_status == (UInt16)HIDE.HIDE_LEFT_PANEL);
            bool rightclose = (ui_globals.panels_status == (UInt16)HIDE.HIDE_BOTH || ui_globals.panels_status == (UInt16)HIDE.HIDE_RIGHT_PANEL);

            // accept left/right keys as-is with repeat
            if (!ignoreleft && exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_LEFT, (flags & (UInt32)PROCESS.PROCESS_LR_REPEAT) != 0 ? 6 : 0))
            {
                // Swap the right panel
                if (m_focus == focused_menu.RIGHTTOP)
                    ui_globals.rpanel = (byte)RP.RP_IMAGES;
                return;
            }

            if (!ignoreright && exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_RIGHT, (flags & (UInt32)PROCESS.PROCESS_LR_REPEAT) != 0 ? 6 : 0))
            {
                // Swap the right panel
                if (m_focus == focused_menu.RIGHTTOP)
                    ui_globals.rpanel = (byte)RP.RP_INFOS;
                return;
            }

            // up backs up by one item
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_UP, 6))
            {
                if (!leftclose && m_focus == focused_menu.LEFT)
                {
                    return;
                }
                else if (!rightclose && m_focus == focused_menu.RIGHTBOTTOM)
                {
                    m_topline_datsview--;
                    return;
                }
                else if (selected == visible_items + 1 || is_first_selected() || m_ui_error)
                {
                    return;
                }

                selected--;

                if (selected == top_line && top_line != 0)
                    top_line--;
            }

            // down advances by one item
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_DOWN, 6))
            {
                if (!leftclose && m_focus == focused_menu.LEFT)
                {
                    return;
                }
                else if (!rightclose && m_focus == focused_menu.RIGHTBOTTOM)
                {
                    m_topline_datsview++;
                    return;
                }
                else if (is_last_selected() || selected == visible_items - 1 || m_ui_error)
                {
                    return;
                }

                selected++;

                if (selected == top_line + m_visible_items + ((top_line != 0) ? 1 : 0))
                    top_line++;
            }

            // page up backs up by m_visible_items
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_PAGE_UP, 6))
            {
                // Infos
                if (!rightclose && m_focus == focused_menu.RIGHTBOTTOM)
                {
                    m_topline_datsview -= m_right_visible_lines - 1;
                    return;
                }

                if (selected < visible_items && !m_ui_error)
                {
                    selected -= m_visible_items;

                    if (selected < 0)
                        selected = 0;

                    top_line -= m_visible_items - (top_line + (m_visible_lines == visible_items ? 1 : 0));
                }
            }

            // page down advances by m_visible_items
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_PAGE_DOWN, 6))
            {
                // Infos
                if (!rightclose && m_focus == focused_menu.RIGHTBOTTOM)
                {
                    m_topline_datsview += m_right_visible_lines - 1;
                    return;
                }

                if (selected < visible_items && !m_ui_error)
                {
                    selected += m_visible_lines - 2 + ((selected == 0) ? 1 : 0);

                    if (selected >= visible_items)
                        selected = visible_items - 1;

                    top_line += m_visible_lines - 2;
                }
            }

            // home goes to the start
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_HOME, 0))
            {
                if (!leftclose && m_focus == focused_menu.LEFT)
                {
                    return;
                }
                else if (!rightclose && m_focus == focused_menu.RIGHTBOTTOM)
                {
                    m_topline_datsview = 0;
                    return;
                }

                if (selected < visible_items && !m_ui_error)
                {
                    selected = 0;
                    top_line = 0;
                }
            }

            // end goes to the last
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_END, 0))
            {
                if (!leftclose && m_focus == focused_menu.LEFT)
                {
                    return;
                }
                else if (!rightclose && m_focus == focused_menu.RIGHTBOTTOM)
                {
                    m_topline_datsview = m_total_lines;
                    return;
                }

                if (selected < visible_items && !m_ui_error)
                    selected = top_line = visible_items - 1;
            }

            // pause enables/disables pause
            if (!m_ui_error && !ignorepause && exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_PAUSE, 0))
            {
                if (machine().paused())
                    machine().resume();
                else
                    machine().pause();
            }

            // handle a toggle cheats request
            if (!m_ui_error && machine().ui_input().pressed_repeat((int)ioport_type.IPT_UI_TOGGLE_CHEAT, 0))
                mame_machine_manager.instance().cheat().set_enable(!mame_machine_manager.instance().cheat().enabled());

            // see if any other UI keys are pressed
            if (iptkey == (int)ioport_type.IPT_INVALID)
            {
                for (int code = (int)ioport_type.IPT_UI_FIRST + 1; code < (int)ioport_type.IPT_UI_LAST; code++)
                {
                    if (m_ui_error || code == (int)ioport_type.IPT_UI_CONFIGURE || (code == (int)ioport_type.IPT_UI_LEFT && ignoreleft) || (code == (int)ioport_type.IPT_UI_RIGHT && ignoreright) || (code == (int)ioport_type.IPT_UI_PAUSE && ignorepause))
                        continue;

                    if (exclusive_input_pressed(ref iptkey, code, 0))
                        break;
                }
            }
        }


        // handle mouse
        //-------------------------------------------------
        //  handle input events for main menu
        //-------------------------------------------------
        protected override void handle_events(UInt32 flags, menu_event ev)
        {
            if (m_pressed)
            {
                bool pressed = mouse_pressed();
                int target_x;
                int target_y;
                bool button;
                render_target mouse_target = machine().ui_input().find_mouse(out target_x, out target_y, out button);
                if (mouse_target != null && button && (hover == (int)HOVER.HOVER_ARROW_DOWN || hover == (int)HOVER.HOVER_ARROW_UP))
                {
                    if (pressed)
                        machine().ui_input().push_mouse_down_event(mouse_target, target_x, target_y);
                }
                else
                {
                    reset_pressed();
                }
            }

            // loop while we have interesting events
            bool stop = false;
            bool search_changed = false;
            ui_event local_menu_event;
            while (!stop && machine().ui_input().pop_event(out local_menu_event))
            {
                switch (local_menu_event.event_type)
                {
                    // if we are hovering over a valid item, select it with a single click
                    case ui_event.type.MOUSE_DOWN:
                    {
                        if (m_ui_error)
                        {
                            ev.iptkey = (int)ioport_type.IPT_OTHER;
                            stop = true;
                        }
                        else
                        {
                            if (hover >= 0 && hover < item.size())
                            {
                                if (hover >= visible_items - 1 && selected < visible_items)
                                    m_prev_selected = get_selection_ref();
                                selected = hover;
                                m_focus = focused_menu.MAIN;
                            }
                            else if (hover == (int)HOVER.HOVER_ARROW_UP)
                            {
                                selected -= m_visible_items;
                                if (selected < 0)
                                    selected = 0;
                                top_line -= m_visible_items - (top_line + (m_visible_lines == visible_items ? 1 : 0));
                                set_pressed();
                            }
                            else if (hover == (int)HOVER.HOVER_ARROW_DOWN)
                            {
                                selected += m_visible_lines - 2 + ((selected == 0) ? 1 : 0);
                                if (selected >= visible_items)
                                    selected = visible_items - 1;
                                top_line += m_visible_lines - 2;
                                set_pressed();
                            }
                            else if (hover == (int)HOVER.HOVER_UI_RIGHT)
                                ev.iptkey = (int)ioport_type.IPT_UI_RIGHT;
                            else if (hover == (int)HOVER.HOVER_UI_LEFT)
                                ev.iptkey = (int)ioport_type.IPT_UI_LEFT;
                            else if (hover == (int)HOVER.HOVER_DAT_DOWN)
                                m_topline_datsview += m_right_visible_lines - 1;
                            else if (hover == (int)HOVER.HOVER_DAT_UP)
                                m_topline_datsview -= m_right_visible_lines - 1;
                            else if (hover == (int)HOVER.HOVER_LPANEL_ARROW)
                            {
                                if (ui_globals.panels_status == (UInt16)HIDE.HIDE_LEFT_PANEL)
                                    ui_globals.panels_status = (UInt16)HIDE.SHOW_PANELS;
                                else if (ui_globals.panels_status == (UInt16)HIDE.HIDE_BOTH)
                                    ui_globals.panels_status = (UInt16)HIDE.HIDE_RIGHT_PANEL;
                                else if (ui_globals.panels_status == (UInt16)HIDE.SHOW_PANELS)
                                    ui_globals.panels_status = (UInt16)HIDE.HIDE_LEFT_PANEL;
                                else if (ui_globals.panels_status == (UInt16)HIDE.HIDE_RIGHT_PANEL)
                                    ui_globals.panels_status = (UInt16)HIDE.HIDE_BOTH;
                            }
                            else if (hover == (int)HOVER.HOVER_RPANEL_ARROW)
                            {
                                if (ui_globals.panels_status == (UInt16)HIDE.HIDE_RIGHT_PANEL)
                                    ui_globals.panels_status = (UInt16)HIDE.SHOW_PANELS;
                                else if (ui_globals.panels_status == (UInt16)HIDE.HIDE_BOTH)
                                    ui_globals.panels_status = (UInt16)HIDE.HIDE_LEFT_PANEL;
                                else if (ui_globals.panels_status == (UInt16)HIDE.SHOW_PANELS)
                                    ui_globals.panels_status = (UInt16)HIDE.HIDE_RIGHT_PANEL;
                                else if (ui_globals.panels_status == (UInt16)HIDE.HIDE_LEFT_PANEL)
                                    ui_globals.panels_status = (UInt16)HIDE.HIDE_BOTH;
                            }
                            else if (hover == (int)HOVER.HOVER_B_FAV)
                            {
                                ev.iptkey = (int)ioport_type.IPT_UI_FAVORITES;
                                stop = true;
                            }
                            else if (hover == (int)HOVER.HOVER_B_EXPORT)
                            {
                                ev.iptkey = (int)ioport_type.IPT_UI_EXPORT;
                                stop = true;
                            }
                            else if (hover == (int)HOVER.HOVER_B_DATS)
                            {
                                ev.iptkey = (int)ioport_type.IPT_UI_DATS;
                                stop = true;
                            }
                            else if (hover >= (int)HOVER.HOVER_RP_FIRST && hover <= (int)HOVER.HOVER_RP_LAST)
                            {
                                ui_globals.rpanel = (byte)(((int)HOVER.HOVER_RP_FIRST - hover) * (-1));
                                stop = true;
                            }
                            else if (hover >= (int)HOVER.HOVER_FILTER_FIRST && hover <= (int)HOVER.HOVER_FILTER_LAST)
                            {
                                m_prev_selected = null;
                                m_filter_highlight = hover - (int)HOVER.HOVER_FILTER_FIRST;
                                filter_selected();
                                stop = true;
                            }
                        }
                        break;
                    }

                    // if we are hovering over a valid item, fake a UI_SELECT with a double-click
                    case ui_event.type.MOUSE_DOUBLE_CLICK:
                        if (hover >= 0 && hover < item.size())
                        {
                            selected = hover;
                            ev.iptkey = (int)ioport_type.IPT_UI_SELECT;
                        }

                        if (is_last_selected())
                        {
                            ev.iptkey = (int)ioport_type.IPT_UI_CANCEL;
                            stack_pop();
                        }
                        stop = true;
                        break;

                    // caught scroll event
                    case ui_event.type.MOUSE_WHEEL:
                        if (hover >= 0 && hover < item.size() - skip_main_items - 1)
                        {
                            if (local_menu_event.zdelta > 0)
                            {
                                if (selected >= visible_items || is_first_selected() || m_ui_error)
                                    break;
                                selected -= local_menu_event.num_lines;
                                if (selected < top_line + ((top_line != 0) ? 1 : 0))
                                    top_line -= local_menu_event.num_lines;
                            }
                            else
                            {
                                if (selected >= visible_items - 1 || m_ui_error)
                                    break;
                                selected += local_menu_event.num_lines;
                                if (selected > visible_items - 1)
                                    selected = visible_items - 1;
                                if (selected >= top_line + m_visible_items + ((top_line != 0) ? 1 : 0))
                                    top_line += local_menu_event.num_lines;
                            }
                        }
                        break;

                    // translate CHAR events into specials
                    case ui_event.type.IME_CHAR:
                        if (exclusive_input_pressed(ref ev.iptkey, (int)ioport_type.IPT_UI_CONFIGURE, 0))
                        {
                            ev.iptkey = (int)ioport_type.IPT_UI_CONFIGURE;
                            stop = true;
                        }
                        else
                        {
                            ev.iptkey = (int)ioport_type.IPT_SPECIAL;
                            ev.unichar = local_menu_event.ch;
                            stop = true;
                        }
                        break;

                    case ui_event.type.MOUSE_RDOWN:
                        if (hover >= 0 && hover < item.size() - skip_main_items - 1)
                        {
                            selected = hover;
                            m_prev_selected = get_selection_ref();
                            m_focus = focused_menu.MAIN;
                            ev.iptkey = (int)ioport_type.IPT_CUSTOM;
                            ev.mouse.x0 = local_menu_event.mouse_x;
                            ev.mouse.y0 = local_menu_event.mouse_y;
                            stop = true;
                        }
                        break;

                    // ignore everything else
                    default:
                        break;
                }

                // need to update search before processing certain kinds of events, but others don't matter
                if (search_changed)
                {
                    switch (machine().ui_input().peek_event_type())
                    {
                    case ui_event.type.MOUSE_DOWN:
                    case ui_event.type.MOUSE_RDOWN:
                    case ui_event.type.MOUSE_DOUBLE_CLICK:
                    case ui_event.type.MOUSE_WHEEL:
                        stop = true;
                        break;
                    case ui_event.type.NONE:
                    case ui_event.type.MOUSE_MOVE:
                    case ui_event.type.MOUSE_LEAVE:
                    case ui_event.type.MOUSE_UP:
                    case ui_event.type.MOUSE_RUP:
                    case ui_event.type.IME_CHAR:
                        break;
                    }
                }
            }

            if (search_changed)
                reset(reset_options.SELECT_FIRST);
        }


        // live search active?
        protected override bool menu_has_search_active() { return !m_search.empty(); }


        // draw game list
        //-------------------------------------------------
        //  draw main menu
        //-------------------------------------------------
        protected override void draw(UInt32 flags)
        {
            bool noinput = (flags & (UInt32)PROCESS.PROCESS_NOINPUT) != 0;
            float line_height = ui().get_line_height();
            float ud_arrow_width = line_height * machine().render().ui_aspect();
            float gutter_width = 0.52f * ud_arrow_width;
            float right_panel_size = (ui_globals.panels_status == (UInt16)HIDE.HIDE_BOTH || ui_globals.panels_status == (UInt16)HIDE.HIDE_RIGHT_PANEL) ? 2.0f * ui_global.UI_BOX_LR_BORDER : 0.3f;
            float visible_width = 1.0f - 4.0f * ui_global.UI_BOX_LR_BORDER;
            float primary_left = (1.0f - visible_width) * 0.5f;
            float primary_width = visible_width;

            //throw new emu_unimplemented();
#if false
            draw_background();
#endif

            hover = item.size() + 1;
            visible_items = (m_is_swlist) ? item.size() - 2 : item.size() - 2 - skip_main_items;
            float extra_height = (m_is_swlist) ? 2.0f * line_height : (2.0f + skip_main_items) * line_height;
            float visible_extra_menu_height = get_customtop() + get_custombottom() + extra_height;

            // locate mouse
            if (noinput)
                ignore_mouse();
            else
                map_mouse();

            // account for extra space at the top and bottom
            float visible_main_menu_height = 1.0f - 2.0f * ui_global.UI_BOX_TB_BORDER - visible_extra_menu_height;
            m_visible_lines = (int)(Math.Truncate(visible_main_menu_height / line_height));
            visible_main_menu_height = (float)m_visible_lines * line_height;

            if (!m_is_swlist)
                ui_globals.visible_main_lines = m_visible_lines;
            else
                ui_globals.visible_sw_lines = m_visible_lines;

            // compute top/left of inner menu area by centering
            float visible_left = primary_left;
            float visible_top = (1.0f - (visible_main_menu_height + visible_extra_menu_height)) * 0.5f;

            // if the menu is at the bottom of the extra, adjust
            visible_top += get_customtop();

            // compute left box size
            float x1 = visible_left - ui_global.UI_BOX_LR_BORDER;
            float y1 = visible_top - ui_global.UI_BOX_TB_BORDER;
            float x2 = x1 + 2.0f * ui_global.UI_BOX_LR_BORDER;
            float y2 = visible_top + visible_main_menu_height + ui_global.UI_BOX_TB_BORDER + extra_height;

            // add left box
            visible_left = draw_left_panel(x1, y1, x2, y2);
            visible_width -= right_panel_size + visible_left - 2.0f * ui_global.UI_BOX_LR_BORDER;

            // compute and add main box
            x1 = visible_left - ui_global.UI_BOX_LR_BORDER;
            x2 = visible_left + visible_width + ui_global.UI_BOX_LR_BORDER;
            float line = visible_top + ((float)m_visible_lines * line_height);
            ui().draw_outlined_box(container(), x1, y1, x2, y2, ui_global.UI_BACKGROUND_COLOR);

            if (visible_items < m_visible_lines)
                m_visible_lines = visible_items;
            if (top_line < 0 || is_first_selected())
                top_line = 0;
            if (selected < visible_items && top_line + m_visible_lines >= visible_items)
                top_line = visible_items - m_visible_lines;

            // determine effective positions taking into account the hilighting arrows
            float effective_width = visible_width - 2.0f * gutter_width;
            float effective_left = visible_left + gutter_width;

            if ((m_focus == focused_menu.MAIN) && (selected < visible_items))
                m_prev_selected = null;

            int n_loop = Math.Min(m_visible_lines, visible_items);
            for (int linenum = 0; linenum < n_loop; linenum++)
            {
                float line_y = visible_top + (float)linenum * line_height;
                int itemnum = top_line + linenum;
                menu_item pitem = item[itemnum];
                string itemtext = pitem.text.c_str();
                rgb_t fgcolor = ui_global.UI_TEXT_COLOR;
                rgb_t bgcolor = ui_global.UI_TEXT_BG_COLOR;
                rgb_t fgcolor3 = ui_global.UI_CLONE_COLOR;
                float line_x0 = x1 + 0.5f * ui_global.UI_LINE_WIDTH;
                float line_y0 = line_y;
                float line_x1 = x2 - 0.5f * ui_global.UI_LINE_WIDTH;
                float line_y1 = line_y + line_height;

                // set the hover if this is our item
                if (mouse_in_rect(line_x0, line_y0, line_x1, line_y1) && is_selectable(pitem))
                    hover = itemnum;

                // if we're selected, draw with a different background
                if (is_selected(itemnum) && m_focus == focused_menu.MAIN)
                {
                    fgcolor = new rgb_t(0xff, 0xff, 0x00);
                    bgcolor = new rgb_t(0xff, 0xff, 0xff);
                    fgcolor3 = new rgb_t(0xcc, 0xcc, 0x00);
                    ui().draw_textured_box(container(), line_x0 + 0.01f, line_y0, line_x1 - 0.01f, line_y1, bgcolor, new rgb_t(43, 43, 43),
                            hilight_main_texture(), global.PRIMFLAG_BLENDMODE(global.BLENDMODE_ALPHA) | global.PRIMFLAG_TEXWRAP(1));
                }
                // else if the mouse is over this item, draw with a different background
                else if (itemnum == hover)
                {
                    fgcolor = fgcolor3 = ui_global.UI_MOUSEOVER_COLOR;
                    bgcolor = ui_global.UI_MOUSEOVER_BG_COLOR;
                    highlight(line_x0, line_y0, line_x1, line_y1, bgcolor);
                }
                else if (pitem.refobj == m_prev_selected)
                {
                    fgcolor = fgcolor3 = ui_global.UI_MOUSEOVER_COLOR;
                    bgcolor = ui_global.UI_MOUSEOVER_BG_COLOR;
                    ui().draw_textured_box(container(), line_x0 + 0.01f, line_y0, line_x1 - 0.01f, line_y1, bgcolor, new rgb_t(43, 43, 43),
                            hilight_main_texture(), global.PRIMFLAG_BLENDMODE(global.BLENDMODE_ALPHA) | global.PRIMFLAG_TEXWRAP(1));
                }

                if (linenum == 0 && top_line != 0)
                {
                    // if we're on the top line, display the up arrow
                    draw_arrow(0.5f * (x1 + x2) - 0.5f * ud_arrow_width, line_y + 0.25f * line_height,
                        0.5f * (x1 + x2) + 0.5f * ud_arrow_width, line_y + 0.75f * line_height, fgcolor, emucore_global.ROT0);

                    if (hover == itemnum)
                        hover = (int)HOVER.HOVER_ARROW_UP;
                }
                else if (linenum == m_visible_lines - 1 && itemnum != visible_items - 1)
                {
                    // if we're on the bottom line, display the down arrow
                    draw_arrow(0.5f * (x1 + x2) - 0.5f * ud_arrow_width, line_y + 0.25f * line_height,
                        0.5f * (x1 + x2) + 0.5f * ud_arrow_width, line_y + 0.75f * line_height, fgcolor, emucore_global.ROT0 ^ emucore_global.ORIENTATION_FLIP_Y);

                    if (hover == itemnum)
                        hover = (int)HOVER.HOVER_ARROW_DOWN;
                }
                else if (pitem.type == menu_item_type.SEPARATOR)
                {
                    // if we're just a divider, draw a line
                    container().add_line(visible_left, line_y + 0.5f * line_height, visible_left + visible_width, line_y + 0.5f * line_height,
                            ui_global.UI_LINE_WIDTH, ui_global.UI_TEXT_COLOR, global.PRIMFLAG_BLENDMODE(global.BLENDMODE_ALPHA));
                }
                else if (pitem.subtext.empty())
                {
                    // draw the item centered
                    var item_invert = (pitem.flags & (UInt32)FLAG.FLAG_INVERT) != 0;
                    var icon = draw_icon(linenum, item[itemnum].refobj, effective_left, line_y);
                    float unused1;
                    float unused2;
                    ui().draw_text_full(container(), itemtext, effective_left + icon, line_y, effective_width - icon, text_layout.text_justify.LEFT, text_layout.word_wrapping.TRUNCATE,
                            mame_ui_manager.draw_mode.NORMAL, item_invert ? fgcolor3 : fgcolor, bgcolor, out unused1, out unused2);
                }
                else
                {
                    var item_invert = (pitem.flags & (UInt32)FLAG.FLAG_INVERT) != 0;
                    string subitem_text = pitem.subtext.c_str();
                    float item_width;
                    float subitem_width;
                    float unused1;
                    float unused2;

                    // compute right space for subitem
                    ui().draw_text_full(container(), subitem_text, effective_left, line_y, ui().get_string_width(pitem.subtext.c_str()),
                            text_layout.text_justify.RIGHT, text_layout.word_wrapping.NEVER, mame_ui_manager.draw_mode.NONE, item_invert ? fgcolor3 : fgcolor, bgcolor, out subitem_width, out unused1);
                    subitem_width += gutter_width;

                    // draw the item left-justified
                    ui().draw_text_full(container(), itemtext, effective_left, line_y, effective_width - subitem_width,
                            text_layout.text_justify.LEFT, text_layout.word_wrapping.TRUNCATE, mame_ui_manager.draw_mode.NORMAL, item_invert ? fgcolor3 : fgcolor, bgcolor, out item_width, out unused1);

                    // draw the subitem right-justified
                    ui().draw_text_full(container(), subitem_text, effective_left + item_width, line_y, effective_width - item_width,
                            text_layout.text_justify.RIGHT, text_layout.word_wrapping.NEVER, mame_ui_manager.draw_mode.NORMAL, item_invert ? fgcolor3 : fgcolor, bgcolor, out unused1, out unused2);
                }
            }

            for (UInt32 count = (UInt32)visible_items; count < item.size(); count++)
            {
                menu_item pitem = item[(int)count];
                string itemtext = pitem.text.c_str();
                float line_x0 = x1 + 0.5f * ui_global.UI_LINE_WIDTH;
                float line_y0 = line;
                float line_x1 = x2 - 0.5f * ui_global.UI_LINE_WIDTH;
                float line_y1 = line + line_height;
                rgb_t fgcolor = ui_global.UI_TEXT_COLOR;
                rgb_t bgcolor = ui_global.UI_TEXT_BG_COLOR;

                if (mouse_in_rect(line_x0, line_y0, line_x1, line_y1) && is_selectable(pitem))
                    hover = (int)count;

                // if we're selected, draw with a different background
                if (is_selected((int)count) && m_focus == focused_menu.MAIN)
                {
                    fgcolor = new rgb_t(0xff, 0xff, 0x00);
                    bgcolor = new rgb_t(0xff, 0xff, 0xff);
                    ui().draw_textured_box(container(), line_x0 + 0.01f, line_y0, line_x1 - 0.01f, line_y1, bgcolor, new rgb_t(43, 43, 43),
                            hilight_main_texture(), global.PRIMFLAG_BLENDMODE(global.BLENDMODE_ALPHA) | global.PRIMFLAG_TEXWRAP(1));
                }
                // else if the mouse is over this item, draw with a different background
                else if (count == hover)
                {
                    fgcolor = ui_global.UI_MOUSEOVER_COLOR;
                    bgcolor = ui_global.UI_MOUSEOVER_BG_COLOR;
                    highlight(line_x0, line_y0, line_x1, line_y1, bgcolor);
                }

                if (pitem.type == menu_item_type.SEPARATOR)
                {
                    container().add_line(visible_left, line + 0.5f * line_height, visible_left + visible_width, line + 0.5f * line_height,
                            ui_global.UI_LINE_WIDTH, ui_global.UI_TEXT_COLOR, global.PRIMFLAG_BLENDMODE(global.BLENDMODE_ALPHA));
                }
                else
                {
                    float unused1;
                    float unused2;
                    ui().draw_text_full(container(), itemtext, effective_left, line, effective_width, text_layout.text_justify.CENTER, text_layout.word_wrapping.TRUNCATE,
                            mame_ui_manager.draw_mode.NORMAL, fgcolor, bgcolor, out unused1, out unused2);
                }
                line += line_height;
            }

            x1 = x2;
            x2 += right_panel_size;

            draw_right_panel(x1, y1, x2, y2);

            x1 = primary_left - ui_global.UI_BOX_LR_BORDER;
            x2 = primary_left + primary_width + ui_global.UI_BOX_LR_BORDER;

            // if there is something special to add, do it by calling the virtual method
            custom_render(get_selection_ref(), get_customtop(), get_custombottom(), x1, y1, x2, y2);

            // return the number of visible lines, minus 1 for top arrow and 1 for bottom arrow
            m_visible_items = m_visible_lines - ((top_line != 0) ? 1 : 0) - (top_line + (m_visible_lines != visible_items ? 1 : 0));

            // reset redraw icon stage
            if (!m_is_swlist) ui_globals.redraw_icon = false;

            // noinput
            if (noinput)
            {
                int alpha = (int)((1.0f - machine().options().pause_brightness()) * 255.0f);
                if (alpha > 255)
                    alpha = 255;
                if (alpha >= 0)
                    container().add_rect(0.0f, 0.0f, 1.0f, 1.0f, new rgb_t((byte)alpha, 0x00, 0x00, 0x00), global.PRIMFLAG_BLENDMODE(global.BLENDMODE_ALPHA));
            }
        }


        // draw right panel

        //-------------------------------------------------
        //  draw right panel
        //-------------------------------------------------
        void draw_right_panel(float origx1, float origy1, float origx2, float origy2)
        {
            bool hide = (ui_globals.panels_status == (UInt16)HIDE.HIDE_RIGHT_PANEL) || (ui_globals.panels_status == (UInt16)HIDE.HIDE_BOTH);
            float x2 = hide ? origx2 : (origx1 + 2.0f * ui_global.UI_BOX_LR_BORDER);
            float space = x2 - origx1;
            float lr_arrow_width = 0.4f * space * machine().render().ui_aspect();

            // set left-right arrows dimension
            float ar_x0 = 0.5f * (x2 + origx1) - 0.5f * lr_arrow_width;
            float ar_y0 = 0.5f * (origy2 + origy1) + 0.1f * space;
            float ar_x1 = ar_x0 + lr_arrow_width;
            float ar_y1 = 0.5f * (origy2 + origy1) + 0.9f * space;

            ui().draw_outlined_box(container(), origx1, origy1, origx2, origy2, new rgb_t(0xEF, 0x12, 0x47, 0x7B));

            rgb_t fgcolor = ui_global.UI_TEXT_COLOR;
            if (mouse_in_rect(origx1, origy1, x2, origy2))
            {
                fgcolor = ui_global.UI_MOUSEOVER_COLOR;
                hover = (int)HOVER.HOVER_RPANEL_ARROW;
            }

            if (hide)
            {
                draw_arrow(ar_x0, ar_y0, ar_x1, ar_y1, fgcolor, emucore_global.ROT90 ^ emucore_global.ORIENTATION_FLIP_X);
                return;
            }

            draw_arrow(ar_x0, ar_y0, ar_x1, ar_y1, fgcolor, emucore_global.ROT90);
            origy1 = draw_right_box_title(x2, origy1, origx2, origy2);

            if (ui_globals.rpanel == (byte)RP.RP_IMAGES)
                arts_render(x2, origy1, origx2, origy2);
            else
                infos_render(x2, origy1, origx2, origy2);
        }


        //-------------------------------------------------
        //  draw right box title
        //-------------------------------------------------
        float draw_right_box_title(float x1, float y1, float x2, float y2)
        {
            var line_height = ui().get_line_height();
            float midl = (x2 - x1) * 0.5f;

            // add outlined box for options
            ui().draw_outlined_box(container(), x1, y1, x2, y2, ui_global.UI_BACKGROUND_COLOR);

            // add separator line
            container().add_line(x1 + midl, y1, x1 + midl, y1 + line_height, ui_global.UI_LINE_WIDTH, ui_global.UI_BORDER_COLOR, global.PRIMFLAG_BLENDMODE((UInt32)BLENDMODE.BLENDMODE_ALPHA));

            string [] buffer = new string[(int)RP.RP_LAST + 1];
            buffer[(int)RP.RP_IMAGES] = "Images";
            buffer[(int)RP.RP_INFOS] = "Infos";

            // check size
            float text_size = 1.0f;
            foreach (var elem in buffer)
            {
                var textlen = ui().get_string_width(elem.c_str()) + 0.01f;
                float tmp_size = (textlen > midl) ? (midl / textlen) : 1.0f;
                text_size = Math.Min(text_size, tmp_size);
            }

            for (int cells = (int)RP.RP_FIRST; cells <= (int)RP.RP_LAST; ++cells)
            {
                rgb_t bgcolor = ui_global.UI_TEXT_BG_COLOR;
                rgb_t fgcolor = ui_global.UI_TEXT_COLOR;

                if (mouse_in_rect(x1, y1, x1 + midl, y1 + line_height))
                {
                    if (ui_globals.rpanel != cells)
                    {
                        bgcolor = ui_global.UI_MOUSEOVER_BG_COLOR;
                        fgcolor = ui_global.UI_MOUSEOVER_COLOR;
                        hover = (int)HOVER.HOVER_RP_FIRST + cells;
                    }
                }

                if (ui_globals.rpanel != cells)
                {
                    container().add_line(x1, y1 + line_height, x1 + midl, y1 + line_height, ui_global.UI_LINE_WIDTH,
                            ui_global.UI_BORDER_COLOR, global.PRIMFLAG_BLENDMODE((UInt32)BLENDMODE.BLENDMODE_ALPHA));
                    if (fgcolor != ui_global.UI_MOUSEOVER_COLOR)
                        fgcolor = ui_global.UI_CLONE_COLOR;
                }

                if (m_focus == focused_menu.RIGHTTOP && ui_globals.rpanel == cells)
                {
                    fgcolor = new rgb_t(0xff, 0xff, 0x00);
                    bgcolor = new rgb_t(0xff, 0xff, 0xff);
                    ui().draw_textured_box(container(), x1 + ui_global.UI_LINE_WIDTH, y1 + ui_global.UI_LINE_WIDTH, x1 + midl - ui_global.UI_LINE_WIDTH, y1 + line_height,
                            bgcolor, new rgb_t(43, 43, 43), hilight_main_texture(), global.PRIMFLAG_BLENDMODE((UInt32)BLENDMODE.BLENDMODE_ALPHA) | global.PRIMFLAG_TEXWRAP(1));
                }
                else if (bgcolor == ui_global.UI_MOUSEOVER_BG_COLOR)
                {
                    container().add_rect(x1 + ui_global.UI_LINE_WIDTH, y1 + ui_global.UI_LINE_WIDTH, x1 + midl - ui_global.UI_LINE_WIDTH, y1 + line_height,
                            bgcolor, global.PRIMFLAG_BLENDMODE((UInt32)BLENDMODE.BLENDMODE_ALPHA) | global.PRIMFLAG_TEXWRAP(1));
                }

                float unused1;
                float unused2;
                ui().draw_text_full(container(), buffer[cells].c_str(), x1 + ui_global.UI_LINE_WIDTH, y1, midl - ui_global.UI_LINE_WIDTH,
                        text_layout.text_justify.CENTER, text_layout.word_wrapping.NEVER, mame_ui_manager.draw_mode.NORMAL, fgcolor, bgcolor, out unused1, out unused2, text_size);

                x1 += midl;
            }

            return y1 + line_height + ui_global.UI_LINE_WIDTH;
        }


        // images render

        //-------------------------------------------------
        //  perform our special rendering
        //-------------------------------------------------
        void arts_render(float origx1, float origy1, float origx2, float origy2)
        {
            ui_software_info software;
            game_driver driver;
            get_selection(out software, out driver);

            if (software != null && ((software.startempty == 0) || driver == null))
            {
                m_cache.set_snapx_driver(null);

                if (ui_globals.default_image)
                    ui_globals.curimage_view = (software.startempty == 0) ? (byte)VIEW.SNAPSHOT_VIEW : (byte)VIEW.CABINETS_VIEW;

                // arts title and searchpath
                string searchstr = arts_render_common(origx1, origy1, origx2, origy2);

                // loads the image if necessary
                if (!m_cache.snapx_software_is(software) || !snapx_valid() || ui_globals.switch_image)
                {
                    emu_file snapfile = new emu_file(searchstr.c_str(), osdcore_global.OPEN_FLAG_READ);
                    bitmap_argb32 tmp_bitmap;

                    if (software.startempty == 1)
                    {
                        // Load driver snapshot
                        string fullname = software.driver.name + ".png";
                        rendutil_global.render_load_png(out tmp_bitmap, snapfile, null, fullname.c_str());

                        if (!tmp_bitmap.valid())
                        {
                            fullname = software.driver.name + ".jpg";
                            rendutil_global.render_load_jpeg(out tmp_bitmap, snapfile, null, fullname.c_str());
                        }
                    }
                    else if (ui_globals.curimage_view == (byte)VIEW.TITLES_VIEW)
                    {
                        // First attempt from name list
                        string pathname = software.listname + "_titles";
                        string fullname = software.shortname + ".png";
                        rendutil_global.render_load_png(out tmp_bitmap, snapfile, pathname.c_str(), fullname.c_str());

                        if (!tmp_bitmap.valid())
                        {
                            fullname = software.shortname + ".jpg";
                            rendutil_global.render_load_jpeg(out tmp_bitmap, snapfile, pathname.c_str(), fullname.c_str());
                        }
                    }
                    else
                    {
                        // First attempt from name list
                        string pathname = software.listname;
                        string fullname = software.shortname + ".png";
                        rendutil_global.render_load_png(out tmp_bitmap, snapfile, pathname.c_str(), fullname.c_str());

                        if (!tmp_bitmap.valid())
                        {
                            fullname = software.shortname + ".jpg";
                            rendutil_global.render_load_jpeg(out tmp_bitmap, snapfile, pathname.c_str(), fullname.c_str());
                        }

                        if (!tmp_bitmap.valid())
                        {
                            // Second attempt from driver name + part name
                            pathname = software.driver.name + software.part;
                            fullname = software.shortname + ".png";
                            rendutil_global.render_load_png(out tmp_bitmap, snapfile, pathname.c_str(), fullname.c_str());

                            if (!tmp_bitmap.valid())
                            {
                                fullname = software.shortname + ".jpg";
                                rendutil_global.render_load_jpeg(out tmp_bitmap, snapfile, pathname.c_str(), fullname.c_str());
                            }
                        }
                    }

                    m_cache.set_snapx_software(software);
                    ui_globals.switch_image = false;
                    arts_render_images(tmp_bitmap, origx1, origy1, origx2, origy2);
                }

                // if the image is available, loaded and valid, display it
                draw_snapx(origx1, origy1, origx2, origy2);
            }
            else if (driver != null)
            {
                m_cache.set_snapx_software(null);

                if (ui_globals.default_image)
                    ui_globals.curimage_view = ((driver.flags & machine_flags.type.MASK_TYPE) != machine_flags.type.TYPE_ARCADE) ? (byte)VIEW.CABINETS_VIEW : (byte)VIEW.SNAPSHOT_VIEW;

                string searchstr = arts_render_common(origx1, origy1, origx2, origy2);

                // loads the image if necessary
                if (!m_cache.snapx_driver_is(driver) || !snapx_valid() || ui_globals.switch_image)
                {
                    emu_file snapfile = new emu_file(searchstr.c_str(), osdcore_global.OPEN_FLAG_READ);
                    snapfile.set_restrict_to_mediapath(true);
                    bitmap_argb32 tmp_bitmap;

                    // try to load snapshot first from saved "0000.png" file
                    string fullname = driver.name;
                    rendutil_global.render_load_png(out tmp_bitmap, snapfile, fullname.c_str(), "0000.png");

                    if (!tmp_bitmap.valid())
                        rendutil_global.render_load_jpeg(out tmp_bitmap, snapfile, fullname.c_str(), "0000.jpg");

                    // if fail, attemp to load from standard file
                    if (!tmp_bitmap.valid())
                    {
                        fullname = driver.name + ".png";
                        rendutil_global.render_load_png(out tmp_bitmap, snapfile, null, fullname.c_str());

                        if (!tmp_bitmap.valid())
                        {
                            fullname = driver.name + ".jpg";
                            rendutil_global.render_load_jpeg(out tmp_bitmap, snapfile, null, fullname.c_str());
                        }
                    }

                    // if fail again, attemp to load from parent file
                    if (!tmp_bitmap.valid())
                    {
                        // set clone status
                        bool cloneof = global.strcmp(driver.parent, "0") != 0;
                        if (cloneof)
                        {
                            int cx = driver_list.find(driver.parent);
                            if ((cx >= 0) && (driver_list.driver((UInt32)cx).flags & machine_flags.type.IS_BIOS_ROOT) != 0)
                                cloneof = false;
                        }

                        if (cloneof)
                        {
                            fullname = driver.parent + ".png";
                            rendutil_global.render_load_png(out tmp_bitmap, snapfile, null, fullname.c_str());

                            if (!tmp_bitmap.valid())
                            {
                                fullname = driver.parent + ".jpg";
                                rendutil_global.render_load_jpeg(out tmp_bitmap, snapfile, null, fullname.c_str());
                            }
                        }
                    }

                    m_cache.set_snapx_driver(driver);
                    ui_globals.switch_image = false;
                    arts_render_images(tmp_bitmap, origx1, origy1, origx2, origy2);
                }

                // if the image is available, loaded and valid, display it
                draw_snapx(origx1, origy1, origx2, origy2);
            }
        }


        //-------------------------------------------------
        //  common function for images render
        //-------------------------------------------------
        string arts_render_common(float origx1, float origy1, float origx2, float origy2)
        {
            float line_height = ui().get_line_height();
            float gutter_width = 0.4f * line_height * machine().render().ui_aspect() * 1.3f;

            string snaptext;
            string searchstr;
            get_title_search(out snaptext, out searchstr);

            // apply title to right panel
            float title_size = 0.0f;
            for (int x = (int)VIEW.FIRST_VIEW; x < (int)VIEW.LAST_VIEW; x++)
            {
                float text_length;
                float unused;
                ui().draw_text_full(container(),
                        arts_info[x].Key, origx1, origy1, origx2 - origx1,
                        text_layout.text_justify.CENTER, text_layout.word_wrapping.TRUNCATE, mame_ui_manager.draw_mode.NONE, rgb_t.white(), rgb_t.black(),
                        out text_length, out unused);
                title_size = Math.Max(text_length + 0.01f, title_size);
            }

            rgb_t fgcolor = (m_focus == focused_menu.RIGHTBOTTOM) ? new rgb_t(0xff, 0xff, 0x00) : ui_global.UI_TEXT_COLOR;
            rgb_t bgcolor = (m_focus == focused_menu.RIGHTBOTTOM) ? new rgb_t(0xff, 0xff, 0xff) : ui_global.UI_TEXT_BG_COLOR;
            float middle = origx2 - origx1;

            // check size
            float sc = title_size + 2.0f * gutter_width;
            float tmp_size = (sc > middle) ? ((middle - 2.0f * gutter_width) / sc) : 1.0f;
            title_size *= tmp_size;

            if (bgcolor != ui_global.UI_TEXT_BG_COLOR)
            {
                ui().draw_textured_box(
                        container(),
                        origx1 + ((middle - title_size) * 0.5f), origy1 + ui_global.UI_BOX_TB_BORDER,
                        origx1 + ((middle + title_size) * 0.5f), origy1 + ui_global.UI_BOX_TB_BORDER + line_height,
                        bgcolor, new rgb_t(43, 43, 43),
                        hilight_main_texture(), global.PRIMFLAG_BLENDMODE((UInt32)BLENDMODE.BLENDMODE_ALPHA) | global.PRIMFLAG_TEXWRAP(1));
            }

            float unused1;
            float unused2;
            ui().draw_text_full(container(),
                    snaptext.c_str(), origx1, origy1 + ui_global.UI_BOX_TB_BORDER, origx2 - origx1,
                    text_layout.text_justify.CENTER, text_layout.word_wrapping.TRUNCATE, mame_ui_manager.draw_mode.NORMAL, fgcolor, bgcolor,
                    out unused1, out unused2, tmp_size);

            draw_common_arrow(origx1, origy1 + ui_global.UI_BOX_TB_BORDER, origx2, origy2, ui_globals.curimage_view, (int)VIEW.FIRST_VIEW, (int)VIEW.LAST_VIEW, title_size);

            return searchstr;
        }


        //-------------------------------------------------
        //  perform rendering of image
        //-------------------------------------------------
        void arts_render_images(bitmap_argb32 tmp_bitmap, float origx1, float origy1, float origx2, float origy2)
        {
            bool no_available = false;
            float line_height = ui().get_line_height();

            // if it fails, use the default image
            if (!tmp_bitmap.valid())
            {
                tmp_bitmap.allocate(256, 256);
                bitmap_argb32 src = m_cache.no_avail_bitmap();
                for (int x = 0; x < 256; x++)
                {
                    for (int y = 0; y < 256; y++)
                    {
                        //tmp_bitmap.pix32(y, x) = src.pix32(y, x);
                        RawBuffer tmp_bitmapBuf;
                        UInt32 tmp_bitmapOffset = tmp_bitmap.pix32(out tmp_bitmapBuf, y, x);
                        RawBuffer srcBuf;
                        UInt32 srcOffset = src.pix32(out srcBuf, y, x);
                        tmp_bitmapBuf.set_uint32((int)tmp_bitmapOffset, srcBuf.get_uint32((int)srcOffset));
                    }
                }
                no_available = true;
            }

            bitmap_argb32 snapx_bitmap = m_cache.snapx_bitmap();
            if (tmp_bitmap.valid())
            {
                float panel_width = origx2 - origx1 - 0.02f;
                float panel_height = origy2 - origy1 - 0.02f - (3.0f * ui_global.UI_BOX_TB_BORDER) - (2.0f * line_height);
                int screen_width = machine().render().ui_target().width();
                int screen_height = machine().render().ui_target().height();

                if ((machine().render().ui_target().orientation() & emucore_global.ORIENTATION_SWAP_XY) != 0)
                    global.swap(ref screen_height, ref screen_width);

                int panel_width_pixel = (int)(panel_width * screen_width);
                int panel_height_pixel = (int)(panel_height * screen_height);

                // Calculate resize ratios for resizing
                var ratioW = (float)panel_width_pixel / tmp_bitmap.width();
                var ratioH = (float)panel_height_pixel / tmp_bitmap.height();
                var ratioI = (float)tmp_bitmap.height() / tmp_bitmap.width();
                var dest_xPixel = tmp_bitmap.width();
                var dest_yPixel = tmp_bitmap.height();

                // force 4:3 ratio min
                if (ui().options().forced_4x3_snapshot() && ratioI < 0.75f && ui_globals.curimage_view == (byte)VIEW.SNAPSHOT_VIEW)
                {
                    // smaller ratio will ensure that the image fits in the view
                    dest_yPixel = (int)(tmp_bitmap.width() * 0.75f);
                    ratioH = (float)panel_height_pixel / dest_yPixel;
                    float ratio = Math.Min(ratioW, ratioH);
                    dest_xPixel = (int)(tmp_bitmap.width() * ratio);
                    dest_yPixel *= (int)ratio;
                }
                // resize the bitmap if necessary
                else if (ratioW < 1 || ratioH < 1 || (ui().options().enlarge_snaps() && !no_available))
                {
                    // smaller ratio will ensure that the image fits in the view
                    float ratio = Math.Min(ratioW, ratioH);
                    dest_xPixel = (int)(tmp_bitmap.width() * ratio);
                    dest_yPixel = (int)(tmp_bitmap.height() * ratio);
                }

                bitmap_argb32 dest_bitmap;

                // resample if necessary
                if (dest_xPixel != tmp_bitmap.width() || dest_yPixel != tmp_bitmap.height())
                {
                    dest_bitmap = new bitmap_argb32();
                    dest_bitmap.allocate(dest_xPixel, dest_yPixel);
                    render_color color = new render_color() { a = 1.0f, r = 1.0f, g = 1.0f, b = 1.0f };
                    rendutil_global.render_resample_argb_bitmap_hq(dest_bitmap, tmp_bitmap, color, true);
                }
                else
                {
                    dest_bitmap = tmp_bitmap;
                }

                snapx_bitmap.allocate(panel_width_pixel, panel_height_pixel);
                int x1 = (int)((0.5f * panel_width_pixel) - (0.5f * dest_xPixel));
                int y1 = (int)((0.5f * panel_height_pixel) - (0.5f * dest_yPixel));

                for (int x = 0; x < dest_xPixel; x++)
                {
                    for (int y = 0; y < dest_yPixel; y++)
                    {
                        //snapx_bitmap.pix32(y + y1, x + x1) = dest_bitmap.pix32(y, x);
                        RawBuffer snapx_bitmapBuf;
                        UInt32 snapx_bitmapOffset = snapx_bitmap.pix32(out snapx_bitmapBuf, y + y1, x + x1);
                        RawBuffer dest_bitmapBuf;
                        UInt32 dest_bitmapOffset = dest_bitmap.pix32(out dest_bitmapBuf, y, x);
                        snapx_bitmapBuf.set_uint32((int)snapx_bitmapOffset, dest_bitmapBuf.get_uint32((int)dest_bitmapOffset));
                    }
                }

                // apply bitmap
                m_cache.snapx_texture().set_bitmap(snapx_bitmap, snapx_bitmap.cliprect(), texture_format.TEXFORMAT_ARGB32);
            }
            else
            {
                snapx_bitmap.reset();
            }
        }


        //-------------------------------------------------
        //  draw snapshot
        //-------------------------------------------------
        void draw_snapx(float origx1, float origy1, float origx2, float origy2)
        {
            // if the image is available, loaded and valid, display it
            if (snapx_valid())
            {
                float line_height = ui().get_line_height();
                float x1 = origx1 + 0.01f;
                float x2 = origx2 - 0.01f;
                float y1 = origy1 + (2.0f * ui_global.UI_BOX_TB_BORDER) + line_height;
                float y2 = origy2 - ui_global.UI_BOX_TB_BORDER - line_height;

                // apply texture
                container().add_quad(x1, y1, x2, y2, rgb_t.white(), m_cache.snapx_texture(), global.PRIMFLAG_BLENDMODE((UInt32)BLENDMODE.BLENDMODE_ALPHA));
            }
        }


        // text for main top/bottom panels
        protected abstract void make_topbox_text(out string line0, out string line1, out string line2);

        protected abstract string make_driver_description(game_driver driver);
        protected abstract string make_software_description(ui_software_info software);


        // filter navigation
        protected abstract void filter_selected();


        //-------------------------------------------------
        //  actually start an emulation session
        //-------------------------------------------------
        static void launch_system(mame_ui_manager mui, game_driver driver, ui_software_info swinfo, string part, int bios)
        {
            emu_options moptions = mui.machine().options();
            moptions.set_system_name(driver.name);

            if (swinfo != null)
            {
                if (swinfo.startempty == 0)
                {
                    if (part != null)
                        moptions.set_value(swinfo.instance, string.Format("{0}:{1}:{2}", swinfo.listname, swinfo.shortname, part), emu_options.OPTION_PRIORITY_CMDLINE);
                    else
                        moptions.set_value(emu_options.OPTION_SOFTWARENAME, string.Format("{0}:{1}", swinfo.listname, swinfo.shortname), emu_options.OPTION_PRIORITY_CMDLINE);

                    moptions.set_value(emu_options.OPTION_SNAPNAME, string.Format("{0}{1}{2}", swinfo.listname, osdcore_global.PATH_SEPARATOR, swinfo.shortname), emu_options.OPTION_PRIORITY_CMDLINE);
                }
                reselect_last.set_software(driver, swinfo);
            }
            else
            {
                reselect_last.set_driver(driver);
            }

            if (bios != 0)
                moptions.set_value(emu_options.OPTION_BIOS, bios, emu_options.OPTION_PRIORITY_CMDLINE);

            mame_machine_manager.instance().schedule_new_driver(driver);
            mui.machine().schedule_hard_reset();
            stack_reset(mui.machine());
        }


        //static bool select_part(mame_ui_manager &mui, render_container &container, software_info const &info, ui_software_info const &ui_info);
        //static bool has_multiple_bios(ui_software_info const &swinfo, s_bios &biosname);
        //static bool has_multiple_bios(game_driver const &driver, s_bios &biosname);


        // cleanup function
        static void exit(running_machine machine)
        {
            //throw new emu_unimplemented();
#if false
            std::lock_guard<std::mutex> guard(s_cache_guard);
#endif
            s_caches.erase(machine);
        }
    }
}
