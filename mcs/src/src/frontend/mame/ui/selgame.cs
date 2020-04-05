// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using icon_cache = mame.util.lru_cache_map<mame.game_driver, mame.ui.menu_select_game.texture_and_bitmap>;
using ListBytes = mame.ListBase<System.Byte>;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame.ui
{
    class menu_select_game : menu_select_launch, IDisposable
    {
        //using icon_cache = texture_lru<game_driver const *>;

        //enum
        //{
        const int CONF_OPTS    = 1;
        const int CONF_MACHINE = 2;
        const int CONF_PLUGINS = 3;
        //}


        const uint32_t FLAGS_UI = FLAG_LEFT_ARROW | FLAG_RIGHT_ARROW;


        class persistent_data
        {
            public enum available //: unsigned
            {
                AVAIL_NONE              = 0,
                AVAIL_SORTED_LIST       = 1 << 0,
                AVAIL_BIOS_COUNT        = 1 << 1,
                AVAIL_UCS_SHORTNAME     = 1 << 2,
                AVAIL_UCS_DESCRIPTION   = 1 << 3,
                AVAIL_UCS_MANUF_DESC    = 1 << 4,
                AVAIL_FILTER_DATA       = 1 << 5
            }


            // synchronisation
            object m_mutex = new object();  //std::mutex                      m_mutex;
            //std::condition_variable         m_condition;
            //std::unique_ptr<std::thread>    m_thread;
            bool m_started;  //std::atomic<bool>               m_started;
            UInt32 m_available;  //std::atomic<unsigned>           m_available;

            // data
            std.vector<ui_system_info> m_sorted_list = new std.vector<ui_system_info>();
            machine_filter_data m_filter_data = new machine_filter_data();
            int m_bios_count;


            static persistent_data data = new persistent_data();


            persistent_data()
            {
                m_started = false;
                m_available = (UInt32)available.AVAIL_NONE;
                m_bios_count = 0;
            }


            //~persistent_data()
            //{
            //    if (m_thread)
            //        m_thread->join();
            //}


            public void cache_data()
            {
                lock (m_mutex)
                {
                    do_start_caching();
                }
            }


            public bool is_available(available desired)
            {
                return (m_available & (UInt32)desired) == (UInt32)desired;  //return (m_available.load(std::memory_order_acquire) & desired) == desired;
            }


            void wait_available(available desired)
            {
                if (!is_available(desired))
                {
                    lock (m_mutex)  //std::unique_lock<std::mutex> lock(m_mutex);
                    {
                        do_start_caching();

                        //throw new emu_unimplemented();
#if false
                        m_condition.wait(lock, [this, desired] () { return is_available(desired); });
#endif
                    }
                }
            }


            public std.vector<ui_system_info> sorted_list()
            {
                wait_available(available.AVAIL_SORTED_LIST);
                return m_sorted_list;
            }


            public int bios_count()
            {
                wait_available(available.AVAIL_BIOS_COUNT);
                return m_bios_count;
            }


            //bool unavailable_systems()
            //{
            //    wait_available(AVAIL_SORTED_LIST);
            //    return std::find_if(m_sorted_list.begin(), m_sorted_list.end(), [] (ui_system_info const &info) { return !info.available; }) != m_sorted_list.end();
            //}


            public machine_filter_data filter_data()
            {
                wait_available(available.AVAIL_FILTER_DATA);
                return m_filter_data;
            }


            public static persistent_data instance() { return data; }


            void notify_available(available value)
            {
                lock (m_mutex)
                {
                    //throw new emu_unimplemented();
#if false
                    m_available.fetch_or(value, std::memory_order_release);
                    m_condition.notify_all();
#endif
                }
            }


            void do_start_caching()
            {
                if (!m_started)
                {
                    m_started = true;

                    //throw new emu_unimplemented();
#if false
                    m_thread = std::make_unique<std::thread>([this] { do_cache_data(); });
#else
                    do_cache_data();
#endif
                }
            }


            void do_cache_data()
            {
                // generate full list
                m_sorted_list.reserve(driver_list.total());
                std.unordered_set<string> manufacturers;
                std.unordered_set<string> years;
                for (int x = 0; x < driver_list.total(); ++x)
                {
                    game_driver driver = driver_list.driver(x);
                    if (driver != ___empty.driver____empty)
                    {
                        if ((driver.flags & machine_flags.type.IS_BIOS_ROOT) != 0)
                            ++m_bios_count;

                        m_sorted_list.emplace_back(new ui_system_info(driver, x, false));
                        m_filter_data.add_manufacturer(driver.manufacturer);
                        m_filter_data.add_year(driver.year);
                    }
                }

                // notify that BIOS count is valie
                notify_available(available.AVAIL_BIOS_COUNT);

                // sort drivers and notify
                //std::stable_sort(
                //        m_sorted_list.begin(),
                //        m_sorted_list.end(),
                //        [] (ui_system_info const &lhs, ui_system_info const &rhs) { return sorted_game_list(lhs.driver, rhs.driver); });
                m_sorted_list.Sort((lhs, rhs) => { return auditmenu_global.sorted_game_list(lhs.driver, rhs.driver); });
                notify_available(available.AVAIL_SORTED_LIST);

                // sort manufacturers and years
                m_filter_data.finalise();
                notify_available(available.AVAIL_FILTER_DATA);

                // convert shortnames to UCS-4
                foreach (ui_system_info info in m_sorted_list)
                    info.ucs_shortname = unicode_global.ustr_from_utf8(unicode_global.normalize_unicode(info.driver.name, unicode_global.unicode_normalization_form.D, true));
                notify_available(available.AVAIL_UCS_SHORTNAME);

                // convert descriptions to UCS-4
                foreach (ui_system_info info in m_sorted_list)
                    info.ucs_description = unicode_global.ustr_from_utf8(unicode_global.normalize_unicode(info.driver.type.fullname(), unicode_global.unicode_normalization_form.D, true));
                notify_available(available.AVAIL_UCS_DESCRIPTION);

                // convert "<manufacturer> <description>" to UCS-4
                string buf;
                foreach (ui_system_info info in m_sorted_list)
                {
                    buf = info.driver.manufacturer;
                    buf += ' ';
                    buf = buf.append(info.driver.type.fullname());
                    info.ucs_manufacturer_description = unicode_global.ustr_from_utf8(unicode_global.normalize_unicode(buf, unicode_global.unicode_normalization_form.D, true));
                }
                notify_available(available.AVAIL_UCS_MANUF_DESC);
            }
        }


        //static std.vector<game_driver> m_sortedlist = new std.vector<game_driver>();
        //std.vector<ui_system_info> m_availsortedlist = new std.vector<ui_system_info>();
        //std.vector<ui_system_info> m_displaylist = new std.vector<ui_system_info>();

        //game_driver [] m_searchlist = new game_driver[VISIBLE_GAMES_IN_SEARCH + 1];

        persistent_data m_persistent_data;
        icon_cache m_icons;
        string m_icon_paths;
        std.vector<ui_system_info> m_displaylist;  //std::vector<std::reference_wrapper<ui_system_info const> > m_displaylist;

        std.vector<KeyValuePair<double, ui_system_info>> m_searchlist;  //std::vector<std::pair<double, std::reference_wrapper<ui_system_info const> > > m_searchlist;
        UInt32 m_searched_fields;
        bool m_populated_favorites;

        static bool s_first_start = true;


        //-------------------------------------------------
        //  ctor
        //-------------------------------------------------
        menu_select_game(mame_ui_manager mui, render_container container, string gamename)
            : base(mui, container, false)
        {
            m_persistent_data = persistent_data.instance();
            m_icons = new icon_cache(MAX_ICONS_RENDER);
            m_icon_paths = "";
            m_displaylist = new std.vector<ui_system_info>();
            m_searchlist = new std.vector<KeyValuePair<double, ui_system_info>>();
            m_searched_fields = (UInt32)persistent_data.available.AVAIL_NONE;
            m_populated_favorites = false;


            string error_string;
            string last_filter;
            ui_options moptions = mui.options();

            // load drivers cache
            m_persistent_data.cache_data();

            // check if there are available system icons
            check_for_icons(null);

            // build drivers list
            if (!load_available_machines())
                build_available_list();

            if (s_first_start)
            {
                //s_first_start = false; TODO: why wansn't it ever clearing the first start flag?

                reselect_last.set_driver(moptions.last_used_machine());
                ui_globals.rpanel = (byte)std.min(std.max(moptions.last_right_panel(), (int)utils_global.RP_FIRST), (int)utils_global.RP_LAST);

                string tmp = moptions.last_used_filter();
                int found = tmp.find_first_of(",");
                string fake_ini;
                if (found == -1)
                {
                    fake_ini = string_format("{0} = 1\n", tmp);
                }
                else
                {
                    string sub_filter = tmp.substr(found + 1);
                    tmp = tmp.Substring(found);  // .resize(found);
                    fake_ini = string_format("{0} = {1}\n", tmp, sub_filter);
                }

                emu_file file = new emu_file(ui().options().ui_path(), OPEN_FLAG_READ);

                ListBytes temp = new ListBytes();
                foreach (var s in fake_ini.c_str()) temp.Add(Convert.ToByte(s));

                if (file.open_ram(temp, (UInt32)fake_ini.Length) == osd_file.error.NONE)  // fake_ini.c_str()
                {
                    m_persistent_data.filter_data().load_ini(file);
                    file.close();
                }
            }

            // do this after processing the last used filter setting so it overwrites the placeholder
            load_custom_filters();
            m_filter_highlight = (int)m_persistent_data.filter_data().get_current_filter_type();

            if (!moptions.remember_last())
                reselect_last.reset();

            mui.machine().options().set_value(emu_options.OPTION_SNAPNAME, "%g/%i", mame_options.OPTION_PRIORITY_CMDLINE);

            ui_globals.curdats_view = 0;
            ui_globals.panels_status = (uint16_t)moptions.hide_panels();
            ui_globals.curdats_total = 1;
        }

        ~menu_select_game()
        {
            assert(m_isDisposed);  // can remove
        }

        public override void Dispose()
        {
            //string error_string;
            string last_driver = "";
            game_driver driver;
            ui_software_info swinfo;
            get_selection(out swinfo, out driver);
            if (swinfo != null)
                last_driver = swinfo.shortname;
            else
            if (driver != null)
                last_driver = driver.name;

            string filter = m_persistent_data.filter_data().get_config_string();

            ui_options mopt = ui().options();
            mopt.set_value(ui_options.OPTION_LAST_RIGHT_PANEL, ui_globals.rpanel, mame_options.OPTION_PRIORITY_CMDLINE);
            mopt.set_value(ui_options.OPTION_LAST_USED_FILTER, filter.c_str(), mame_options.OPTION_PRIORITY_CMDLINE);
            mopt.set_value(ui_options.OPTION_LAST_USED_MACHINE, last_driver.c_str(), mame_options.OPTION_PRIORITY_CMDLINE);
            mopt.set_value(ui_options.OPTION_HIDE_PANELS, ui_globals.panels_status, mame_options.OPTION_PRIORITY_CMDLINE);
            ui().save_ui_options();

            m_isDisposed = true;
        }


        // force game select menu
        //-------------------------------------------------
        //  force the game select menu to be visible
        //  and inescapable
        //-------------------------------------------------
        public static void force_game_select(mame_ui_manager mui, render_container container)
        {
            // reset the menu stack
            stack_reset(mui.machine());

            // add the quit entry followed by the game select entry
            stack_push_special_main(new menu_quit_game(mui, container));
            stack_push(new menu_select_game(mui, container, null));

            // force the menus on
            mui.show_menu();

            // make sure MAME is paused
            mui.machine().pause();
        }


        protected override void populate(ref float customtop, ref float custombottom)
        {
            foreach (var icon in m_icons) // TODO: why is this here?  maybe better on resize or setting change?
                icon.second().texture = null;  //icon.second().texture.reset();

            set_switch_image();
            int old_item_selected = -1;

            if (!isfavorite())
            {
                m_populated_favorites = false;
                m_displaylist.clear();
                machine_filter flt = m_persistent_data.filter_data().get_current_filter();

                // if search is not empty, find approximate matches
                if (!string.IsNullOrEmpty(m_search))
                {
                    populate_search();

                    if (flt != null)
                    {
                        for (int i = 0; i < m_searchlist.Count && MAX_VISIBLE_SEARCH > m_displaylist.size(); i++)  //for (auto it = m_searchlist.begin(); (m_searchlist.end() != it) && (MAX_VISIBLE_SEARCH > m_displaylist.size()); ++it)
                        {
                            var it = m_searchlist[i];
                            if (flt.apply(it.second()))
                                m_displaylist.emplace_back(it.second());
                        }
                    }
                    else
                    {
                        //std.transform(
                        //        m_searchlist.begin(),
                        //        std.next(m_searchlist.begin(), std.min(m_searchlist.size(), MAX_VISIBLE_SEARCH)),
                        //        std.back_inserter(m_displaylist),
                        //        [] (auto const &entry) { return entry.second; });
                        foreach (var it in m_searchlist)
                            m_displaylist.Add(it.second());
                    }
                }
                else
                {
                    // if filter is set on category, build category list
                    std.vector<ui_system_info> sorted = m_persistent_data.sorted_list();
                    if (flt == null)
                    {
                        //std::copy(sorted.begin(), sorted.end(), std::back_inserter(m_displaylist));
                        foreach (var it in sorted)
                            m_displaylist.Add(it);
                    }
                    else
                    {
                        flt.apply(sorted, m_displaylist);  //flt->apply(sorted.begin(), sorted.end(), std::back_inserter(m_displaylist));
                    }
                }

                // iterate over entries
                int curitem = 0;
                foreach (ui_system_info elem in m_displaylist)
                {
                    if (old_item_selected == -1 && elem.driver.name == reselect_last.driver())
                        old_item_selected = curitem;

                    bool cloneof = strcmp(elem.driver.parent, "0") != 0;
                    if (cloneof)
                    {
                        int cx = driver_list.find(elem.driver.parent);
                        if (cx != -1 && ((driver_list.driver(cx).flags & machine_flags.type.IS_BIOS_ROOT) != 0))
                            cloneof = false;
                    }

                    item_append(elem.driver.type.fullname(), "", (cloneof) ? (FLAGS_UI | FLAG_INVERT) : FLAGS_UI, elem.driver);
                    curitem++;
                }
            }
            else
            {
                // populate favorites list
                m_populated_favorites = true;
                m_search = "";  //m_search.clear();
                int curitem = 0;

                mame_machine_manager.instance().favorite().apply_sorted((info) =>
                {
                    if (info.startempty == 1)
                    {
                        if (old_item_selected == -1 && info.shortname == reselect_last.driver())
                            old_item_selected = curitem;

                        bool cloneof = strcmp(info.driver.parent, "0") != 0;
                        if (cloneof)
                        {
                            int cx = driver_list.find(info.driver.parent);
                            if (cx != -1 && ((driver_list.driver(cx).flags & machine_flags.type.IS_BIOS_ROOT) != 0))
                                cloneof = false;
                        }

                        item_append(info.longname, "", cloneof ? (FLAGS_UI | FLAG_INVERT) : FLAGS_UI, info);
                    }
                    else
                    {
                        if (old_item_selected == -1 && info.shortname == reselect_last.driver())
                            old_item_selected = curitem;

                        item_append(info.longname, info.devicetype, info.parentname.empty() ? FLAGS_UI : (FLAG_INVERT | FLAGS_UI), info);
                    }
                    curitem++;
                });
            }

            item_append(menu_item_type.SEPARATOR, FLAGS_UI);

            // add special items
            if (stack_has_special_main_menu())
            {
                item_append("Configure Options", "", FLAGS_UI, CONF_OPTS);
                item_append("Configure Machine", "", FLAGS_UI, CONF_MACHINE);
                skip_main_items = 2;
                if (machine().options().plugins())
                {
                    item_append("Plugins", "", FLAGS_UI, CONF_PLUGINS);
                    skip_main_items++;
                }
            }
            else
            {
                skip_main_items = 0;
            }

            // configure the custom rendering
            customtop = 3.0f * ui().get_line_height() + 5.0f * UI_BOX_TB_BORDER;
            custombottom = 5.0f * ui().get_line_height() + 3.0f * UI_BOX_TB_BORDER;

            // reselect prior game launched, if any
            if (old_item_selected != -1)
            {
                selected = old_item_selected;
                if (ui_globals.visible_main_lines == 0)
                    top_line = (selected != 0) ? selected - 1 : 0;
                else
                    top_line = selected - (ui_globals.visible_main_lines / 2);

                if (reselect_last.software().empty())
                    reselect_last.reset();
            }
            else
            {
                reselect_last.reset();
            }
        }


        //-------------------------------------------------
        //  handle
        //-------------------------------------------------
        protected override void handle()
        {
            if (m_prev_selected == null)
                m_prev_selected = item[0].ref_;

            // if I have to load datfile, perform a hard reset
            if (ui_globals.reset)
            {
                ui_globals.reset = false;
                machine().schedule_hard_reset();
                stack_reset();
                return;
            }

            // if I have to reselect a software, force software list submenu
            if (reselect_last.get())
            {
                game_driver driver;
                ui_software_info software;
                get_selection(out software, out driver);

                throw new emu_unimplemented();
#if false
                menu.stack_push(new menu_select_software(ui(), container(), *driver));
#endif
                return;
            }

            // ignore pause keys by swallowing them before we process the menu
            machine().ui_input().pressed((int)ioport_type.IPT_UI_PAUSE);

            // process the menu
            menu_event menu_event = process(PROCESS_LR_REPEAT);
            if (menu_event != null)
            {
                if (dismiss_error())
                {
                    // reset the error on any future menu_event
                }
                else
                {
                switch (menu_event.iptkey)
                {
                case (int)ioport_type.IPT_UI_UP:
                    if ((get_focus() == focused_menu.LEFT) && ((int)machine_filter.type.FIRST < m_filter_highlight))
                        --m_filter_highlight;
                    break;

                case (int)ioport_type.IPT_UI_DOWN:
                    if ((get_focus() == focused_menu.LEFT) && ((int)machine_filter.type.LAST > m_filter_highlight))
                        m_filter_highlight++;
                    break;

                case (int)ioport_type.IPT_UI_HOME:
                    if (get_focus() == focused_menu.LEFT)
                        m_filter_highlight = (int)machine_filter.type.FIRST;
                    break;

                case (int)ioport_type.IPT_UI_END:
                    if (get_focus() == focused_menu.LEFT)
                        m_filter_highlight = (int)machine_filter.type.LAST;
                    break;

                case (int)ioport_type.IPT_UI_CONFIGURE:
                    inkey_navigation();
                    break;

                case (int)ioport_type.IPT_UI_EXPORT:
                    inkey_export();
                    break;

                case (int)ioport_type.IPT_UI_DATS:
                    inkey_dats();
                    break;

                default:
                    if (menu_event.itemref != null)
                    {
                        switch (menu_event.iptkey)
                        {
                        case (int)ioport_type.IPT_UI_SELECT:
                            if (get_focus() == focused_menu.MAIN)
                            {
                                if (m_populated_favorites)
                                    inkey_select_favorite(menu_event);
                                else
                                    inkey_select(menu_event);
                            }
                            break;

                        case (int)ioport_type.IPT_CUSTOM:
                            // handle IPT_CUSTOM (mouse right click)
                            if (!m_populated_favorites)
                            {
                                throw new emu_unimplemented();
                            }
                            else
                            {
                                throw new emu_unimplemented();
                            }
                            break;

                        case (int)ioport_type.IPT_UI_LEFT:
                            if (ui_globals.rpanel == utils_global.RP_IMAGES)
                            {
                                // Images
                                previous_image_view();
                            }
                            else if (ui_globals.rpanel == utils_global.RP_INFOS)
                            {
                                // Infos
                                change_info_pane(-1);
                            }
                            break;

                        case (int)ioport_type.IPT_UI_RIGHT:
                            if (ui_globals.rpanel == utils_global.RP_IMAGES)
                            {
                                // Images
                                next_image_view();
                            }
                            else if (ui_globals.rpanel == utils_global.RP_INFOS)
                            {
                                // Infos
                                change_info_pane(1);
                            }
                            break;

                        case (int)ioport_type.IPT_UI_FAVORITES:
                            throw new emu_unimplemented();
                            break;

                        case (int)ioport_type.IPT_UI_AUDIT_FAST:
                            throw new emu_unimplemented();
                            break;

                        case (int)ioport_type.IPT_UI_AUDIT_ALL:
                            throw new emu_unimplemented();
                            break;
                        }
                    }
                    break;
                }
                }
            }

            // if we're in an error state, overlay an error message
            draw_error_text();
        }


        // drawing
        //-------------------------------------------------
        //  draw left box
        //-------------------------------------------------
        protected override float draw_left_panel(float x1, float y1, float x2, float y2)
        {
            machine_filter_data filter_data = m_persistent_data.filter_data();
            return draw_left_panel/*<machine_filter>*/(filter_data.get_current_filter_type(), filter_data.get_filters(), x1, y1, x2, y2);
        }


        protected override render_texture get_icon_texture(int linenum, object selectedref) { throw new emu_unimplemented(); }


        // get selected software and/or driver
        //-------------------------------------------------
        //  get selected software and/or driver
        //-------------------------------------------------
        protected override void get_selection(out ui_software_info software, out game_driver driver)
        {
            software = null;
            driver = null;

            if (m_populated_favorites)
            {
                software = (ui_software_info)get_selection_ptr();
                driver = software != null ? software.driver : null;
            }
            else
            {
                software = null;
                driver = (game_driver)get_selection_ptr();
            }
        }


        protected override bool accept_search() { return !isfavorite(); }


        // text for main top/bottom panels
        protected override void make_topbox_text(out string line0, out string line1, out string line2)
        {
            line0 = "";
            line1 = "";
            line2 = "";

            line0 = string.Format("{0} {1} ( {2} / {3} machines ({4} BIOS) )",
                    emulator_info.get_appname(),
                    version_global.bare_build_version,
                    visible_items,
                    (driver_list.total() - 1),
                    m_persistent_data.bios_count());

            if (m_populated_favorites)
            {
                line1 = "";
            }
            else
            {
                machine_filter it = m_persistent_data.filter_data().get_current_filter();
                string filter = it != null ? it.filter_text() : null;
                if (filter != null)
                    line1 = string_format("{0}: {1} - Search: {2}_", it.display_name(), filter, m_search);  // %1$s: %2$s - Search: %3$s_
                else
                    line1 = string_format("Search: {0}_", m_search);  // %1$s_
            }

            line2 = "";
        }


        protected override string make_driver_description(game_driver driver)
        {
            // first line is game name
            return string.Format("Romset: {0}", driver.name);  // %1$-.100s
        }


        protected override string make_software_description(ui_software_info software)
        {
            // first line is system
            return string.Format("System: {0}", software.driver.type.fullname());  // %1$-.100s
        }


        // filter navigation
        protected override void filter_selected()
        {
            if (((int)machine_filter.type.FIRST <= m_filter_highlight) && ((int)machine_filter.type.LAST >= m_filter_highlight))
            {
                m_persistent_data.filter_data().get_filter((machine_filter.type)m_filter_highlight).show_ui(
                        ui(),
                        container(),
                        (filter) =>
                        {
                            set_switch_image();
                            machine_filter.type new_type = filter.get_type();
                            if (machine_filter.type.CUSTOM == new_type)
                            {
                                emu_file file = new emu_file(ui().options().ui_path(), OPEN_FLAG_WRITE | OPEN_FLAG_CREATE | OPEN_FLAG_CREATE_PATHS);
                                if (file.open("custom_", emulator_info.get_configname(), "_filter.ini") == osd_file.error.NONE)
                                {
                                    filter.save_ini(file, 0);
                                    file.close();
                                }
                            }
                            m_persistent_data.filter_data().set_current_filter_type(new_type);
                            reset(reset_options.SELECT_FIRST);
                        });
            }
        }


        // toolbar
        protected override void inkey_export()
        {
            std.vector<game_driver> list = new std.vector<game_driver>();
            if (m_populated_favorites)
            {
                // iterate over favorites
                mame_machine_manager.instance().favorite().apply((info) =>
                {
                    assert(info.driver != null);
                    if (info.startempty != 0)
                        list.push_back(info.driver);
                });
            }
            else
            {
                list.reserve(m_displaylist.size());
                foreach (ui_system_info info in m_displaylist)
                    list.emplace_back(info.driver);
            }

            throw new emu_unimplemented();
        }


        // internal methods

        //-------------------------------------------------
        //  change what's displayed in the info box
        //-------------------------------------------------
        void change_info_pane(int delta)
        {
            game_driver drv;
            ui_software_info soft;
            get_selection(out soft, out drv);

            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  build a list of available drivers
        //-------------------------------------------------
        void build_available_list()
        {
            int total = driver_list.total();
            std.vector<bool> included = new std.vector<bool>(total, false);

            // iterate over ROM directories and look for potential ROMs
            file_enumerator path = new file_enumerator(machine().options().media_path());
            for (osd.directory.entry dir = path.next(); dir != null; dir = path.next())
            {
                string drivername;  //char drivername[50];
                int dstIdx = 0;  // char *dst = drivername;
                int srcIdx = 0;  //const char *src;

                // build a name for it
                //for (src = dir->name; *src != 0 && *src != '.' && dst < &drivername[ARRAY_LENGTH(drivername) - 1]; ++src)
                //    *dst++ = tolower(uint8_t(*src));
                //*dst = 0;
                drivername = dir.name.ToLower();

                int drivnum = driver_list.find(drivername);
                if (0 <= drivnum)
                {
                    included[drivnum] = true;
                }
            }

            // now check and include NONE_NEEDED
            if (!ui().options().hide_romless())
            {
                throw new emu_unimplemented();
            }


            // copy into the persistent sorted list
            foreach (ui_system_info info in m_persistent_data.sorted_list())
                info.available = included[info.index];
        }


        //-------------------------------------------------
        //  returns if the search can be activated
        //-------------------------------------------------
        bool isfavorite()
        {
            return machine_filter.type.FAVORITE == m_persistent_data.filter_data().get_current_filter_type();
        }


        //-------------------------------------------------
        //  populate search list
        //-------------------------------------------------
        void populate_search()
        {
            // ensure search list is populated
            if (m_searchlist.empty())
            {
                std.vector<ui_system_info> sorted = m_persistent_data.sorted_list();
                m_searchlist.reserve(sorted.size());
                foreach (ui_system_info info in sorted)
                    m_searchlist.emplace_back(new KeyValuePair<double, ui_system_info>(1.0, info));
            }

            // keep track of what we matched against
            string ucs_search = unicode_global.ustr_from_utf8(unicode_global.normalize_unicode(m_search, unicode_global.unicode_normalization_form.D, true));  //const std::u32string ucs_search(ustr_from_utf8(normalize_unicode(m_search, unicode_normalization_form::D, true)));

            // match shortnames
            if (m_persistent_data.is_available(persistent_data.available.AVAIL_UCS_SHORTNAME))
            {
                m_searched_fields |= (UInt32)persistent_data.available.AVAIL_UCS_SHORTNAME;
                for (int i = 0; i < m_searchlist.Count; i++)  //for (std::pair<double, std::reference_wrapper<ui_system_info const> > &info : m_searchlist)
                    m_searchlist[i] = new KeyValuePair<double, ui_system_info>(corestr_global.edit_distance(ucs_search, m_searchlist[i].second().ucs_shortname), m_searchlist[i].second());  //info.first = util::edit_distance(ucs_search, info.second.get().ucs_shortname);
            }

            // match descriptions
            if (m_persistent_data.is_available(persistent_data.available.AVAIL_UCS_DESCRIPTION))
            {
                m_searched_fields |= (UInt32)persistent_data.available.AVAIL_UCS_DESCRIPTION;
                for (int i = 0; i < m_searchlist.Count; i++)  //for (std::pair<double, std::reference_wrapper<ui_system_info const> > &info : m_searchlist)
                {
                    if (m_searchlist[i].first() != 0)  //if (info.first)
                    {
                        double penalty = corestr_global.edit_distance(ucs_search, m_searchlist[i].second().ucs_description);  //double const penalty(util::edit_distance(ucs_search, info.second.get().ucs_description));
                        m_searchlist[i] = new KeyValuePair<double, ui_system_info>(std.min(penalty, m_searchlist[i].first()), m_searchlist[i].second());  //info.first = (std::min)(penalty, info.first);
                    }
                }
            }

            // match "<manufacturer> <description>"
            if (m_persistent_data.is_available(persistent_data.available.AVAIL_UCS_MANUF_DESC))
            {
                m_searched_fields |= (UInt32)persistent_data.available.AVAIL_UCS_MANUF_DESC;
                for (int i = 0; i < m_searchlist.Count; i++)  //for (std::pair<double, std::reference_wrapper<ui_system_info const> > &info : m_searchlist)
                {
                    if (m_searchlist[i].first() != 0)  //if (info.first)
                    {
                        double penalty = corestr_global.edit_distance(ucs_search, m_searchlist[i].second().ucs_manufacturer_description);  //double const penalty(util::edit_distance(ucs_search, info.second.get().ucs_manufacturer_description));
                        m_searchlist[i] = new KeyValuePair<double, ui_system_info>(std.min(penalty, m_searchlist[i].first()), m_searchlist[i].second());  //info.first = (std::min)(penalty, info.first);
                    }
                }
            }

            // sort according to edit distance
            //std::stable_sort(
            //        m_searchlist.begin(),
            //        m_searchlist.end());
            //        [] (auto const &lhs, auto const &rhs) { return lhs.first < rhs.first; });
            m_searchlist.Sort((lhs, rhs) => { return lhs.first().CompareTo(rhs.first()); });
        }


        //-------------------------------------------------
        //  save drivers infos to file
        //-------------------------------------------------
        //void init_sorted_list()


        //-------------------------------------------------
        //  load drivers infos from file
        //-------------------------------------------------
        bool load_available_machines()
        {
            // try to load available drivers from file
            emu_file file = new emu_file(ui().options().ui_path(), OPEN_FLAG_READ);
            if (file.open(emulator_info.get_configname(), "_avail.ini") != osd_file.error.NONE)
                return false;

            string rbuf;  //char rbuf[MAX_CHAR_INFO];
            string readbuf;
            file.gets(out rbuf, utils_global.MAX_CHAR_INFO);
            file.gets(out rbuf, utils_global.MAX_CHAR_INFO);
            readbuf = utils_global.chartrimcarriage(rbuf);
            string a_rev = string.Format("{0}{1}", utils_global.UI_VERSION_TAG, version_global.bare_build_version);

            // version not matching ? exit
            if (a_rev != readbuf)
            {
                file.close();
                return false;
            }

            // load available list
            std.unordered_set<string> available = new std.unordered_set<string>();
            while (file.gets(out rbuf, utils_global.MAX_CHAR_INFO) != null)
            {
                readbuf = rbuf;
                readbuf = readbuf.Trim();

                if (readbuf.empty() || ('#' == readbuf[0])) // ignore empty lines and line comments
                    ;
                else if ('[' == readbuf[0]) // throw out the rest of the file if we find a section heading
                    break;
                else
                    available.emplace(readbuf);
            }
            file.close();

            // turn it into the sorted system list we all love
            foreach (ui_system_info info in m_persistent_data.sorted_list())
            {
                var it = available.find(info.driver.name);
                bool found = it;
                info.available = found;
                if (found)
                    available.erase(info.driver.name);
            }

            return true;
        }


        //-------------------------------------------------
        //  load custom filters info from file
        //-------------------------------------------------
        void load_custom_filters()
        {
            emu_file file = new emu_file(ui().options().ui_path(), OPEN_FLAG_READ);
            if (file.open("custom_", emulator_info.get_configname(), "_filter.ini") == osd_file.error.NONE)
            {
                machine_filter flt = machine_filter.create(file, m_persistent_data.filter_data());
                if (flt != null)
                    m_persistent_data.filter_data().set_filter(flt); // not emplace/insert - could replace bogus filter from ui.ini line

                file.close();
            }
        }


        static string make_error_text(bool summary, media_auditor auditor)
        {
            string str = "";  //std::ostringstream str;
            str += "The selected machine is missing one or more required ROM or CHD images. Please select a different machine.\n\n";
            if (summary)
            {
                auditor.summarize(null, ref str);
                str += "\n";
            }
            str += "Press any key to continue.";
            return str.str();
        }


        // General info
        //-------------------------------------------------
        //  generate general info
        //-------------------------------------------------
        protected override void general_info(game_driver driver, out string buffer)
        {
            system_flags flags = get_system_flags(driver);
            string str = "";  //std::ostringstream str;

            str += "#j2\n";

            str += string.Format("Romset\t{0}\n", driver.name);  // %1$-.100s
            str += string.Format("Year\t{0}\n", driver.year);
            str += string.Format("Manufacturer\t{0}\n", driver.manufacturer);  //%1$-.100s

            int cloneof = driver_list.non_bios_clone(driver);
            if (cloneof != -1)
                str += string.Format("Driver is Clone of\t{0}\n", driver_list.driver(cloneof).type.fullname());  //%1$-.100s
            else
                str += "Driver is Parent\t\n";

            if (flags.has_analog())
                str += "Analog Controls\tYes\n";
            if (flags.has_keyboard())
                str += "Keyboard Inputs\tYes\n";

            if (((UInt64)driver.flags & MACHINE_NOT_WORKING) != 0)
                str += "Overall\tNOT WORKING\n";
            else if (((flags.unemulated_features() | flags.imperfect_features()) & emu.detail.device_feature.type.PROTECTION) != 0)
                str += "Overall\tUnemulated Protection\n";
            else
                str += "Overall\tWorking\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.GRAPHICS) != 0)
                str += "Graphics\tUnimplemented\n";
            else if ((flags.unemulated_features() & emu.detail.device_feature.type.PALETTE) != 0)
                str += "Graphics\tWrong Colors\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.PALETTE) != 0)
                str += "Graphics\tImperfect Colors\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.GRAPHICS) != 0)
                str += "Graphics\tImperfect\n";
            else
                str += "Graphics\tOK\n";

            if ((flags.machine_flags() & machine_flags.type.NO_SOUND_HW) != 0)
                str += "Sound\tNone\n";
            else if ((flags.unemulated_features() & emu.detail.device_feature.type.SOUND) != 0)
                str += "Sound\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.SOUND) != 0)
                str += "Sound\tImperfect\n";
            else
                str += "Sound\tOK\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.CAPTURE) != 0)
                str += "Capture\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.CAPTURE) != 0)
                str += "Capture\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.CAMERA) != 0)
                str += "Camera\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.CAMERA) != 0)
                str += "Camera\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.MICROPHONE) != 0)
                str += "Microphone\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.MICROPHONE) != 0)
                str += "Microphone\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.CONTROLS) != 0)
                str += "Controls\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.CONTROLS) != 0)
                str += "Controls\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.KEYBOARD) != 0)
                str += "Keyboard\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.KEYBOARD) != 0)
                str += "Keyboard\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.MOUSE) != 0)
                str += "Mouse\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.MOUSE) != 0)
                str += "Mouse\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.MEDIA) != 0)
                str += "Media\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.MEDIA) != 0)
                str += "Media\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.DISK) != 0)
                str += "Disk\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.DISK) != 0)
                str += "Disk\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.PRINTER) != 0)
                str += "Printer\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.PRINTER) != 0)
                str += "Printer\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.TAPE) != 0)
                str += "Mag. Tape\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.TAPE) != 0)
                str += "Mag. Tape\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.PUNCH) != 0)
                str += "Punch Tape\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.PUNCH) != 0)
                str += "Punch Tape\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.DRUM) != 0)
                str += "Mag. Drum\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.DRUM) != 0)
                str += "Mag. Drum\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.ROM) != 0)
                str += "(EP)ROM\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.ROM) != 0)
                str += "(EP)ROM\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.COMMS) != 0)
                str += "Communications\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.COMMS) != 0)
                str += "Communications\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.LAN) != 0)
                str += "LAN\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.LAN) != 0)
                str += "LAN\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.WAN) != 0)
                str += "WAN\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.WAN) != 0)
                str += "WAN\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.TIMING) != 0)
                str += "Timing\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.TIMING) != 0)
                str += "Timing\tImperfect\n";

            str += ((flags.machine_flags() & machine_flags.type.MECHANICAL) != 0        ? "Mechanical Machine\tYes\n"         : "Mechanical Machine\tNo\n");
            str += ((flags.machine_flags() & machine_flags.type.REQUIRES_ARTWORK) != 0  ? "Requires Artwork\tYes\n"           : "Requires Artwork\tNo\n");
            str += ((flags.machine_flags() & machine_flags.type.CLICKABLE_ARTWORK) != 0 ? "Requires Clickable Artwork\tYes\n" : "Requires Clickable Artwork\tNo\n");
            str += ((flags.machine_flags() & machine_flags.type.NO_COCKTAIL) != 0       ? "Support Cocktail\tYes\n"           : "Support Cocktail\tNo\n");
            str += ((flags.machine_flags() & machine_flags.type.IS_BIOS_ROOT) != 0      ? "Driver is BIOS\tYes\n"             : "Driver is BIOS\tNo\n");
            str += ((flags.machine_flags() & machine_flags.type.SUPPORTS_SAVE) != 0     ? "Support Save\tYes\n"               : "Support Save\tNo\n");
            str += (((UInt32)flags.machine_flags() & ORIENTATION_SWAP_XY) != 0 ? "Screen Orientation\tVertical\n" : "Screen Orientation\tHorizontal\n");

            bool found = false;
            foreach (tiny_rom_entry region in new romload.entries(driver.rom).get_regions())
            {
                if (romload.region.is_diskdata(region))
                {
                    found = true;
                    break;
                }
            }

            str += found ? "Requires CHD\tYes\n" : "Requires CHD\tNo\n";

            // audit the game first to see if we're going to work
            if (ui().options().info_audit())
            {
                driver_enumerator enumerator = new driver_enumerator(machine().options(), driver);
                enumerator.next();
                media_auditor auditor = new media_auditor(enumerator);
                media_auditor.summary summary = auditor.audit_media(media_auditor.AUDIT_VALIDATE_FAST);
                media_auditor.summary summary_samples = auditor.audit_samples();

                // if everything looks good, schedule the new driver
                if (summary == media_auditor.summary.CORRECT || summary == media_auditor.summary.BEST_AVAILABLE || summary == media_auditor.summary.NONE_NEEDED)
                    str += "ROM Audit Result\tOK\n";
                else
                    str += "ROM Audit Result\tBAD\n";

                if (summary_samples == media_auditor.summary.NONE_NEEDED)
                    str += "Samples Audit Result\tNone Needed\n";
                else if (summary_samples == media_auditor.summary.CORRECT || summary_samples == media_auditor.summary.BEST_AVAILABLE)
                    str += "Samples Audit Result\tOK\n";
                else
                    str += "Samples Audit Result\tBAD\n";
            }
            else
            {
                str += "ROM Audit \tDisabled\nSamples Audit \tDisabled\n";
            }

            buffer = str;
        }


        // handlers

        //-------------------------------------------------
        //  handle select key event
        //-------------------------------------------------
        void inkey_select(menu_event menu_event)
        {
            game_driver driver = menu_event.itemref is game_driver ? (game_driver)menu_event.itemref : null;
            int driverint = menu_event.itemref is int ? (int)menu_event.itemref : -1;

            if (driverint == CONF_OPTS)
            {
                // special case for configure options

                throw new emu_unimplemented();
            }
            else if (driverint == CONF_MACHINE)
            {
                // special case for configure machine

                throw new emu_unimplemented();
            }
            else if (driverint == CONF_PLUGINS)
            {
                // special case for configure plugins

                throw new emu_unimplemented();
            }
            else
            {
                // anything else is a driver

                // audit the game first to see if we're going to work
                driver_enumerator enumerator = new driver_enumerator(machine().options(), driver);
                enumerator.next();
                media_auditor auditor = new media_auditor(enumerator);
                media_auditor.summary summary = auditor.audit_media(media_auditor.AUDIT_VALIDATE_FAST);


                // always pass audit
                //throw new emu_unimplemented();
                summary = media_auditor.summary.CORRECT;


                // if everything looks good, schedule the new driver
                if (summary == media_auditor.summary.CORRECT || summary == media_auditor.summary.BEST_AVAILABLE || summary == media_auditor.summary.NONE_NEEDED)
                {
                    foreach (software_list_device swlistdev in new software_list_device_iterator(enumerator.config().root_device()))
                    {
                        if (!swlistdev.get_info().empty())
                        {
                            throw new emu_unimplemented();
                        }
                    }

                    if (!select_bios(driver, false))
                        launch_system(driver);
                }
                else
                {
                    // otherwise, display an error
                    set_error(reset_options.REMEMBER_REF, make_error_text(media_auditor.summary.NOTFOUND != summary, auditor));
                }
            }
        }


        //-------------------------------------------------
        //  handle select key event for favorites menu
        //-------------------------------------------------
        void inkey_select_favorite(menu_event menu_event)
        {
            ui_software_info ui_swinfo = (ui_software_info)menu_event.itemref;
            int ui_swinfoint = (int)menu_event.itemref;

            if (ui_swinfoint == CONF_OPTS)
            {
                throw new emu_unimplemented();
            }
            else if (ui_swinfoint == CONF_MACHINE)
            {
                // special case for configure machine
                if (m_prev_selected != null)
                {
                    ui_software_info swinfo = (ui_software_info)m_prev_selected;

                    throw new emu_unimplemented();
                }
                return;
            }
            else if (ui_swinfoint == CONF_PLUGINS)
            {
                throw new emu_unimplemented();
            }
            else if (ui_swinfo.startempty == 1)
            {
                // audit the game first to see if we're going to work
                driver_enumerator enumerator = new driver_enumerator(machine().options(), ui_swinfo.driver);
                enumerator.next();
                media_auditor auditor = new media_auditor(enumerator);
                media_auditor.summary summary = auditor.audit_media(media_auditor.AUDIT_VALIDATE_FAST);

                if (summary == media_auditor.summary.CORRECT || summary == media_auditor.summary.BEST_AVAILABLE || summary == media_auditor.summary.NONE_NEEDED)
                {
                    throw new emu_unimplemented();
                }
                else
                {
                    // otherwise, display an error
                    set_error(reset_options.REMEMBER_REF, make_error_text(media_auditor.summary.NOTFOUND != summary, auditor));
                }
            }
            else
            {
                // first validate
                driver_enumerator drv = new driver_enumerator(machine().options(), ui_swinfo.driver);
                media_auditor auditor = new media_auditor(drv);
                drv.next();
                software_list_device swlist = software_list_device.find_by_name(drv.config(), ui_swinfo.listname.c_str());
                software_info swinfo = swlist.find(ui_swinfo.shortname.c_str());

                media_auditor.summary summary = auditor.audit_software(swlist.list_name(), swinfo, media_auditor.AUDIT_VALIDATE_FAST);

                if (summary == media_auditor.summary.CORRECT || summary == media_auditor.summary.BEST_AVAILABLE || summary == media_auditor.summary.NONE_NEEDED)
                {
                    throw new emu_unimplemented();
                }
                else
                {
                    // otherwise, display an error
                    set_error(reset_options.REMEMBER_POSITION, make_error_text(media_auditor.summary.NOTFOUND != summary, auditor));
                }
            }
        }
    }
}
