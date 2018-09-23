// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytes = mame.ListBase<System.Byte>;


namespace mame.ui
{
    class menu_select_game : menu_select_launch
    {
        enum CONF
        {
            CONF_OPTS = 1,
            CONF_MACHINE,
            CONF_PLUGINS,
        }


        const int VISIBLE_GAMES_IN_SEARCH = 200;  //enum { VISIBLE_GAMES_IN_SEARCH = 200 };


        static bool first_start = true;
        static int m_isabios = 0;

        static std_vector<game_driver> m_sortedlist = new std_vector<game_driver>();
        std_vector<ui_system_info> m_availsortedlist = new std_vector<ui_system_info>();
        std_vector<ui_system_info> m_displaylist = new std_vector<ui_system_info>();

        game_driver [] m_searchlist = new game_driver[VISIBLE_GAMES_IN_SEARCH + 1];


        //-------------------------------------------------
        //  ctor
        //-------------------------------------------------
        menu_select_game(mame_ui_manager mui, render_container container, string gamename)
            : base(mui, container, false)
        {
            string error_string;
            string last_filter;
            ui_options moptions = mui.options();

            // load drivers cache
            init_sorted_list();

            // check if there are available icons
            ui_globals.has_icons = false;
            file_enumerator path = new file_enumerator(moptions.icons_directory());
            osd.directory.entry dir;
            while ((dir = path.next()) != null)
            {
                string src = dir.name;
                if (src.Contains(".ico") || src.Contains("icons"))
                {
                    ui_globals.has_icons = true;
                    break;
                }
            }

            // build drivers list
            if (!load_available_machines())
                build_available_list();

            if (first_start)
            {
                reselect_last.set_driver(moptions.last_used_machine());
                ui_globals.rpanel = (byte)Math.Min(Math.Max(moptions.last_right_panel(), (int)RP.RP_FIRST), (int)RP.RP_LAST);

                string tmp = moptions.last_used_filter();
                int found = tmp.find_first_of(",");
                string fake_ini;
                if (found == -1)
                {
                    fake_ini = string.Format("{0} = 1\n", tmp);
                }
                else
                {
                    string sub_filter = tmp.substr(found + 1);
                    tmp = tmp.Substring(found);  // .resize(found);
                    fake_ini = string.Format("{0} = {1}\n", tmp, sub_filter);
                }

                emu_file file = new emu_file(ui().options().ui_path(), osdcore_global.OPEN_FLAG_READ);

                ListBytes temp = new ListBytes();
                foreach (var s in fake_ini.c_str()) temp.Add(Convert.ToByte(s));

                if (file.open_ram(temp, (UInt32)fake_ini.Length) == osd_file.error.NONE)  // fake_ini.c_str()
                {
                    machine_filter flt = machine_filter.create(file);
                    if (flt != null)
                    {
                        main_filters.actual = flt.get_type();
                        main_filters.filters.emplace(main_filters.actual, flt);
                    }
                    file.close();
                }
            }

            // do this after processing the last used filter setting so it overwrites the placeholder
            load_custom_filters();
            m_filter_highlight = (int)main_filters.actual;

            if (!moptions.remember_last())
                reselect_last.reset();

            mui.machine().options().set_value(emu_options.OPTION_SNAPNAME, "%g/%i", (int)OPTION_PRIORITY.OPTION_PRIORITY_CMDLINE);

            ui_globals.curimage_view = (byte)VIEW.FIRST_VIEW;
            ui_globals.curdats_view = 0;
            ui_globals.switch_image = false;
            ui_globals.default_image = true;
            ui_globals.panels_status = (UInt16)moptions.hide_panels();
            ui_globals.curdats_total = 1;
            m_searchlist[0] = null;
        }


        //-------------------------------------------------
        //  dtor
        //-------------------------------------------------
        ~menu_select_game()
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

            string filter;
            var active_filter = main_filters.filters.find(main_filters.actual);
            if (null != active_filter)
            {
                string val = active_filter.filter_text();
                filter = val != null ? string.Format("{0},{1}", active_filter.config_name(), val) : active_filter.config_name();
            }
            else
            {
                filter = machine_filter.config_name(main_filters.actual);
            }

            ui_options mopt = ui().options();
            mopt.set_value(ui_options.OPTION_LAST_RIGHT_PANEL, ui_globals.rpanel, (int)OPTION_PRIORITY.OPTION_PRIORITY_CMDLINE);
            mopt.set_value(ui_options.OPTION_LAST_USED_FILTER, filter.c_str(), (int)OPTION_PRIORITY.OPTION_PRIORITY_CMDLINE);
            mopt.set_value(ui_options.OPTION_LAST_USED_MACHINE, last_driver.c_str(), (int)OPTION_PRIORITY.OPTION_PRIORITY_CMDLINE);
            mopt.set_value(ui_options.OPTION_HIDE_PANELS, ui_globals.panels_status, (int)OPTION_PRIORITY.OPTION_PRIORITY_CMDLINE);
            ui().save_ui_options();
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
            ui_globals.redraw_icon = true;
            ui_globals.switch_image = true;
            int old_item_selected = -1;
            UInt32 flags_ui = (UInt32)(FLAG.FLAG_LEFT_ARROW | FLAG.FLAG_RIGHT_ARROW);

            if (!isfavorite())
            {
                // if search is not empty, find approximate matches
                if (!string.IsNullOrEmpty(m_search))
                {
                    populate_search();
                }
                else
                {
                    // reset search string
                    m_search = "";
                    m_displaylist.clear();

                    // if filter is set on category, build category list
                    var it = main_filters.filters.find(main_filters.actual);
                    if (null == it)
                        m_displaylist = m_availsortedlist;
                    else
                        it.apply(m_availsortedlist, m_displaylist);  // it.apply(m_availsortedlist.begin(), m_availsortedlist.end(), std::back_inserter(m_displaylist));

                    // iterate over entries
                    int curitem = 0;
                    foreach (ui_system_info elem in m_displaylist)
                    {
                        if (old_item_selected == -1 && elem.driver.name == reselect_last.driver())
                            old_item_selected = curitem;

                        bool cloneof = global.strcmp(elem.driver.parent, "0") != 0;
                        if (cloneof)
                        {
                            int cx = driver_list.find(elem.driver.parent);
                            if (cx != -1 && (((UInt64)driver_list.driver((UInt32)cx).flags & gamedrv_global.MACHINE_IS_BIOS_ROOT) != 0))
                                cloneof = false;
                        }

                        item_append(elem.driver.type.fullname(), "", cloneof ? (flags_ui | (UInt32)menu.FLAG.FLAG_INVERT) : flags_ui, elem);
                        curitem++;
                    }
                }
            }
            else
            {
                // populate favorites list
                m_search = "";
                int curitem = 0;

                // iterate over entries
                foreach (var favmap in mame_machine_manager.instance().favorite().list())
                {
                    var flags = flags_ui | (UInt32)menu.FLAG.FLAG_UI_FAVORITE;
                    if (favmap.Value[0].startempty == 1)
                    {
                        if (old_item_selected == -1 && favmap.Value[0].shortname == reselect_last.driver())
                            old_item_selected = curitem;

                        bool cloneof = global.strcmp(favmap.Value[0].driver.parent, "0") != 0;
                        if (cloneof)
                        {
                            int cx = driver_list.find(favmap.Value[0].driver.parent);
                            if (cx != -1 && (((UInt64)driver_list.driver((UInt32)cx).flags & gamedrv_global.MACHINE_IS_BIOS_ROOT) != 0))
                                cloneof = false;
                        }

                        item_append(favmap.Value[0].longname, "", (cloneof) ? (flags | (UInt32)menu.FLAG.FLAG_INVERT) : flags, favmap.Value);
                    }
                    else
                    {
                        if (old_item_selected == -1 && favmap.Value[0].shortname == reselect_last.driver())
                            old_item_selected = curitem;

                        item_append(favmap.Value[0].longname, favmap.Value[0].devicetype,
                                    favmap.Value[0].parentname.empty() ? flags : ((UInt32)menu.FLAG.FLAG_INVERT | flags), favmap.Value);
                    }

                    curitem++;
                }
            }

            item_append(menu_item_type.SEPARATOR, flags_ui);

            // add special items
            if (stack_has_special_main_menu())
            {
                item_append("Configure Options", "", flags_ui, CONF.CONF_OPTS);
                item_append("Configure Machine", "", flags_ui, CONF.CONF_MACHINE);
                skip_main_items = 2;
                if (machine().options().plugins())
                {
                    item_append("Plugins", "", flags_ui, CONF.CONF_PLUGINS);
                    skip_main_items++;
                }
            }
            else
            {
                skip_main_items = 0;
            }

            // configure the custom rendering
            customtop = 3.0f * ui().get_line_height() + 5.0f * ui_global.UI_BOX_TB_BORDER;
            custombottom = 5.0f * ui().get_line_height() + 3.0f * ui_global.UI_BOX_TB_BORDER;

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
                m_prev_selected = item[0].refobj;

            // if I have to load datfile, perform a hard reset
            if (ui_globals.reset)
            {
                ui_globals.reset = false;
                machine().schedule_hard_reset();
                stack_reset();
                return;
            }

            // if i have to reselect a software, force software list submenu
            if (reselect_last.get())
            {
                game_driver driver;
                ui_software_info software;
                get_selection(out software, out driver);

                throw new emu_unimplemented();
#if false
                menu.stack_push(new menu_select_software(ui(), container(), driver));
#endif
                return;
            }

            // ignore pause keys by swallowing them before we process the menu
            machine().ui_input().pressed((int)ioport_type.IPT_UI_PAUSE);

            // process the menu
            menu_event menu_event = process((UInt32)PROCESS.PROCESS_LR_REPEAT);
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
                                if (isfavorite())
                                    inkey_select_favorite(menu_event);
                                else
                                    inkey_select(menu_event);
                            }
                            break;

                        case (int)ioport_type.IPT_CUSTOM:
                            // handle IPT_CUSTOM (mouse right click)
                            if (!isfavorite())
                            {
                                throw new emu_unimplemented();
#if false
                                menu.stack_push(new menu_machine_configure(
                                        ui(), container(),
                                        (game_driver)m_prev_selected,
                                        menu_event.mouse.x0, menu_event.mouse.y0));
#endif
                            }
                            else
                            {
                                throw new emu_unimplemented();
#if false
                                ui_software_info sw = (ui_software_info)m_prev_selected;
                                menu.stack_push(new menu_machine_configure(
                                        ui(), container(),
                                        (game_driver)sw.driver,
                                        menu_event.mouse.x0, menu_event.mouse.y0));
#endif
                            }
                            break;

                        case (int)ioport_type.IPT_UI_LEFT:
                            if (ui_globals.rpanel == (byte)RP.RP_IMAGES && ui_globals.curimage_view > (byte)VIEW.FIRST_VIEW)
                            {
                                // Images
                                ui_globals.curimage_view--;
                                ui_globals.switch_image = true;
                                ui_globals.default_image = false;
                            }
                            else if (ui_globals.rpanel == (byte)RP.RP_INFOS)
                            {
                                // Infos
                                change_info_pane(-1);
                            }
                            break;

                        case (int)ioport_type.IPT_UI_RIGHT:
                            if (ui_globals.rpanel == (byte)RP.RP_IMAGES && ui_globals.curimage_view < (byte)VIEW.LAST_VIEW)
                            {
                                // Images
                                ui_globals.curimage_view++;
                                ui_globals.switch_image = true;
                                ui_globals.default_image = false;
                            }
                            else if (ui_globals.rpanel == (byte)RP.RP_INFOS)
                            {
                                // Infos
                                change_info_pane(1);
                            }
                            break;

                        case (int)ioport_type.IPT_UI_FAVORITES:
                            throw new emu_unimplemented();
#if false
                            if (uintptr_t(menu_event.itemref) > skip_main_items)
                            {
                                favorite_manager mfav = mame_machine_manager.instance().favorite();
                                if (!isfavorite())
                                {
                                    game_driver driver = reinterpret_cast<game_driver>(menu_event.itemref);
                                    if (!mfav.isgame_favorite(driver))
                                    {
                                        mfav.add_favorite_game(driver);
                                        machine().popmessage("{0}\n added to favorites list.", driver.type.fullname());
                                    }
                                    else
                                    {
                                        mfav.remove_favorite_game();
                                        machine().popmessage("{0}\n removed from favorites list.", driver.type.fullname());
                                    }
                                }
                                else
                                {
                                    ui_software_info swinfo = reinterpret_cast<ui_software_info>(menu_event.itemref);
                                    machine().popmessage("{0}\n removed from favorites list.", swinfo.longname);
                                    mfav.remove_favorite_game(swinfo);
                                    reset(reset_options.SELECT_FIRST);
                                }
                            }
#endif
                            break;

                        case (int)ioport_type.IPT_UI_AUDIT_FAST:
                            throw new emu_unimplemented();
#if false
                            if (m_availsortedlist.Find((info) => { return !info.available; }) != null)  // if (std.find_if(m_availsortedlist.begin(), m_availsortedlist.end(), [] (ui_system_info const &info) { return !info.available; }) != m_availsortedlist.end())
                                menu.stack_push(new menu_audit(ui(), container(), m_availsortedlist, menu_audit.mode.FAST));
#endif
                            break;

                        case (int)ioport_type.IPT_UI_AUDIT_ALL:
                            throw new emu_unimplemented();
#if false
                            menu.stack_push(new menu_audit(ui(), container(), m_availsortedlist, menu_audit.mode.ALL));
#endif
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


        // draw left panel
        //-------------------------------------------------
        //  draw left box
        //-------------------------------------------------
        protected override float draw_left_panel(float x1, float y1, float x2, float y2)
        {
            return draw_left_panel/*<machine_filter>*/(main_filters.actual, main_filters.filters, x1, y1, x2, y2);
        }


        // get selected software and/or driver
        //-------------------------------------------------
        //  get selected software and/or driver
        //-------------------------------------------------
        protected override void get_selection(out ui_software_info software, out game_driver driver)
        {
            software = null;
            driver = null;

            if ((item[0].flags & (UInt32)FLAG.FLAG_UI_FAVORITE) != 0)  // TODO: work out why this doesn't use isfavorite()
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
                    m_isabios);

            if (isfavorite())
            {
                line1 = "";
            }
            else
            {
                var it = main_filters.filters.find(main_filters.actual);
                string filter = (null != it) ? it.filter_text() : null;
                if (filter != null)
                    line1 = string.Format("{0}: {1} - Search: {2}_", it.display_name(), filter, m_search);  // %1$s: %2$s - Search: %3$s_
                else
                    line1 = string.Format("Search: {0}_", m_search);  // %1$s_
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
                m_search = "";
                var it = main_filters.filters.find((machine_filter.type)m_filter_highlight);
                if (null == it)
                {
                    //it = main_filters::filters.emplace(machine_filter::type(m_filter_highlight), machine_filter::create(machine_filter::type(m_filter_highlight))).first;
                    it = machine_filter.create((machine_filter.type)m_filter_highlight);
                    main_filters.filters.emplace((machine_filter.type)m_filter_highlight, it);
                }

                throw new emu_unimplemented();
#if false
                it.show_ui(
                        ui(),
                        container(),
                        [this] (machine_filter &filter)
                        {
                            machine_filter::type const new_type(filter.get_type());
                            if (machine_filter::CUSTOM == new_type)
                            {
                                emu_file file(ui().options().ui_path(), OPEN_FLAG_WRITE | OPEN_FLAG_CREATE | OPEN_FLAG_CREATE_PATHS);
                                if (file.open("custom_", emulator_info::get_configname(), "_filter.ini") == osd_file::error::NONE)
                                {
                                    filter.save_ini(file, 0);
                                    file.close();
                                }
                            }
                            main_filters::actual = new_type;
                            reset(reset_options::SELECT_FIRST);
                        });
#endif
            }
        }


        // toolbar
        protected override void inkey_export()
        {
            std_vector<game_driver> list = new std_vector<game_driver>();
            if (!m_search.empty())
            {
                for (int curitem = 0; m_searchlist[curitem] != null; ++curitem)
                    list.push_back(m_searchlist[curitem]);
            }
            else
            {
                if (isfavorite())
                {
                    // iterate over favorites
                    foreach (var favmap in mame_machine_manager.instance().favorite().list())
                    {
                        if (favmap.Value[0].startempty == 1)
                            list.push_back(favmap.Value[0].driver);
                        else
                            return;
                    }
                }
                else
                {
                    list.reserve(m_displaylist.size());
                    foreach (ui_system_info info in m_displaylist)
                        list.emplace_back(info.driver);
                }
            }

            throw new emu_unimplemented();
#if false
            menu.stack_push(new menu_export(ui(), container(), list));
#endif
        }


        // internal methods

        //-------------------------------------------------
        //  change what's displayed in the info box
        //-------------------------------------------------
        void change_info_pane(int delta)
        {
            //auto const cap_delta = [this, &delta] (uint8_t &current, uint8_t &total)
            //{
            //    if ((0 > delta) && (-delta > current))
            //        delta = -int(unsigned(current));
            //    else if ((0 < delta) && ((current + unsigned(delta)) >= total))
            //        delta = int(unsigned(total - current - 1));
            //    if (delta)
            //    {
            //        current += delta;
            //        m_topline_datsview = 0;
            //    }
            //};

            game_driver drv;
            ui_software_info soft;
            get_selection(out soft, out drv);

            throw new emu_unimplemented();
#if false
            if (!isfavorite())
            {
                if (uintptr_t(drv) > skip_main_items)
                    cap_delta(ui_globals.curdats_view, ui_globals.curdats_total);
            }
            else if (uintptr_t(soft) > skip_main_items)
            {
                if (soft.startempty)
                    cap_delta(ui_globals.curdats_view, ui_globals.curdats_total);
                else
                    cap_delta(ui_globals.cur_sw_dats_view, ui_globals.cur_sw_dats_total);
            }
#endif
        }


        //-------------------------------------------------
        //  build a list of available drivers
        //-------------------------------------------------
        void build_available_list()
        {
            UInt32 total = driver_list.total();
            std_vector<bool> included = new std_vector<bool>(total, false);

            // iterate over ROM directories and look for potential ROMs
            file_enumerator path = new file_enumerator(machine().options().media_path());
            for (osd.directory.entry dir = path.next(); dir != null; dir = path.next())
            {
                string drivername;  //char drivername[50];
                int dstIdx = 0;  // char *dst = drivername;
                int srcIdx = 0;  //const char *src;

                // build a name for it
                //for (src = dir->name; *src != 0 && *src != '.' && dst < &drivername[ARRAY_LENGTH(drivername) - 1]; ++src)
                //    *dst++ = tolower((uint8_t) * src);
                //*dst = 0;
                drivername = dir.name.ToLower();

                int drivnum = driver_list.find(drivername);
                if (drivnum != -1 && !included[drivnum])
                {
                    included[drivnum] = true;
                }
            }

            // now check and include NONE_NEEDED
            if (!ui().options().hide_romless())
            {
                throw new emu_unimplemented();
#if false
                // FIXME: can't use the convenience macros tiny ROM entries
                var is_required_rom = 
                        [] (tiny_rom_entry const &rom) { return ROMENTRY_ISFILE(rom) && !ROM_ISOPTIONAL(rom) && !std::strchr(rom.hashdata, '!'); };
#endif
                for (UInt32 x = 0; x < total; ++x)
                {
                    game_driver driver = driver_list.driver(x);

                    if (!included[(int)x] && ___empty.driver____empty != driver)
                    {
                        bool noroms = true;
                        tiny_rom_entry rom;

                        throw new emu_unimplemented();
#if false
                        for (rom = driver.rom; !ROMENTRY_ISEND(rom); ++rom)
                        {
                            // check optional and NO_DUMP
                            if (is_required_rom(rom))
                            {
                                noroms = false;
                                break; // break before incrementing, or it will subtly break the check for all ROMs belonging to parent
                            }
                        }

                        if (!noroms)
                        {
                            // check if clone == parent
                            var cx = driver_list.clone(driver);
                            if ((0 <= cx) && included[cx])
                            {
                                game_driver parent = driver_list.driver((UInt32)cx);
                                if (driver.rom == parent.rom)
                                {
                                    noroms = true;
                                }
                                else
                                {
                                    // check if clone < parent
                                    noroms = true;
                                    for ( ; noroms && !ROMENTRY_ISEND(rom); ++rom)
                                    {
                                        if (is_required_rom(rom))
                                        {
                                            util.hash_collection hashes = rom.hashdata;

                                            bool found = false;
                                            for (tiny_rom_entry const *parentrom = parent.rom; !found && !ROMENTRY_ISEND(parentrom); ++parentrom)
                                            {
                                                if (is_required_rom(parentrom) && (rom.length == parentrom.length))
                                                {
                                                    util.hash_collection parenthashes = parentrom.hashdata;
                                                    if (hashes == parenthashes)
                                                        found = true;
                                                }
                                            }
                                            noroms = found;
                                        }
                                    }
                                }
                            }
                        }
#endif

                        if (noroms)
                            included[(int)x] = true;
                    }
                }
            }

            // sort
            m_availsortedlist.reserve((int)total);
            for (UInt32 x = 0; total > x; ++x)
            {
                game_driver driver = driver_list.driver(x);
                if (driver != ___empty.driver____empty)
                    m_availsortedlist.emplace_back(new ui_system_info(driver, included[(int)x]));
            }

            //std::stable_sort(
            //        m_availsortedlist.begin(),
            //        m_availsortedlist.end(),
            //        [] (ui_system_info const &a, ui_system_info const &b) { return sorted_game_list(a.driver, b.driver); });
            m_availsortedlist.Sort((a, b) => { return auditmenu_global.sorted_game_list(a.driver, b.driver); });
        }


        //-------------------------------------------------
        //  returns if the search can be activated
        //-------------------------------------------------
        bool isfavorite()
        {
            return machine_filter.type.FAVORITE == main_filters.actual;
        }


        //-------------------------------------------------
        //  populate search list
        //-------------------------------------------------
        void populate_search()
        {
            // allocate memory to track the penalty value
            std_vector<int> penalty = new std_vector<int>(VISIBLE_GAMES_IN_SEARCH, 9999);

            int index = 0;
            for (; index < m_displaylist.size(); ++index)
            {
                // pick the best match between driver name and description
                int curpenalty = utils_global.fuzzy_substring(m_search, m_displaylist[index].driver.type.fullname());
                int tmp = utils_global.fuzzy_substring(m_search, m_displaylist[index].driver.name);
                curpenalty = Math.Min(curpenalty, tmp);

                // insert into the sorted table of matches
                for (int matchnum = VISIBLE_GAMES_IN_SEARCH - 1; matchnum >= 0; --matchnum)
                {
                    // stop if we're worse than the current entry
                    if (curpenalty >= penalty[matchnum])
                        break;

                    // as long as this isn't the last entry, bump this one down
                    if (matchnum < VISIBLE_GAMES_IN_SEARCH - 1)
                    {
                        penalty[matchnum + 1] = penalty[matchnum];
                        m_searchlist[matchnum + 1] = m_searchlist[matchnum];
                    }

                    m_searchlist[matchnum] = m_displaylist[index].driver;
                    penalty[matchnum] = curpenalty;
                }
            }

            if (index < VISIBLE_GAMES_IN_SEARCH)
                m_searchlist[index] = null;
            else
                m_searchlist[VISIBLE_GAMES_IN_SEARCH] = null;

            UInt32 flags_ui = (UInt32)(FLAG.FLAG_LEFT_ARROW | FLAG.FLAG_RIGHT_ARROW);
            for (int curitem = 0; m_searchlist[curitem] != null; ++curitem)
            {
                bool cloneof = global.strcmp(m_searchlist[curitem].parent, "0") != 0;
                if (cloneof)
                {
                    int cx = driver_list.find(m_searchlist[curitem].parent);
                    if (cx != -1 && (((UInt64)driver_list.driver((UInt32)cx).flags & gamedrv_global.MACHINE_IS_BIOS_ROOT) != 0))
                        cloneof = false;
                }

                item_append(m_searchlist[curitem].type.fullname(), "", (!cloneof) ? flags_ui : ((UInt32)FLAG.FLAG_INVERT | flags_ui),
                    m_searchlist[curitem]);
            }
        }


        //-------------------------------------------------
        //  save drivers infos to file
        //-------------------------------------------------
        void init_sorted_list()
        {
            if (!m_sortedlist.empty())
                return;

            // generate full list
            std_unordered_set<string> manufacturers = new std_unordered_set<string>();
            std_unordered_set<string> years = new std_unordered_set<string>();
            for (int x = 0; x < driver_list.total(); ++x)
            {
                game_driver driver = driver_list.driver((UInt32)x);
                if (driver != ___empty.driver____empty)
                {
                    if (((UInt64)driver.flags & gamedrv_global.MACHINE_IS_BIOS_ROOT) != 0)
                        m_isabios++;

                    m_sortedlist.push_back(driver);
                    manufacturers.emplace(c_mnfct.getname(driver.manufacturer));
                    years.emplace(driver.year);
                }
            }

            // sort manufacturers - years and driver
            foreach (var it in manufacturers)  //for (var it = manufacturers.begin(); manufacturers.end() != it; it = manufacturers.erase(it))
                c_mnfct.ui.emplace_back(it);

            //std::sort(c_mnfct::ui.begin(), c_mnfct::ui.end());//, [] (std::string const &x, std::string const &y) { return 0 > core_stricmp(x.c_str(), y.c_str()); });
            c_mnfct.ui.Sort((x, y) => { return global.core_stricmp(x.c_str(), y.c_str()); });

            foreach (var it in years)  //for (var it = years.begin(); years.end() != it; it = years.erase(it))
                c_year.ui.emplace_back(it);

            c_year.ui.Sort();  //std::stable_sort(c_year::ui.begin(), c_year::ui.end());

            m_sortedlist.Sort(auditmenu_global.sorted_game_list);  //std::stable_sort(m_sortedlist.begin(), m_sortedlist.end(), sorted_game_list);
        }


        //-------------------------------------------------
        //  load drivers infos from file
        //-------------------------------------------------
        bool load_available_machines()
        {
            // try to load available drivers from file
            emu_file file = new emu_file(ui().options().ui_path(), osdcore_global.OPEN_FLAG_READ);
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
            std_unordered_set<string> available = new std_unordered_set<string>();
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

            // turn it into the sorted system list we all love
            m_availsortedlist.reserve((int)driver_list.total());
            for (UInt32 x = 0; driver_list.total() > x; ++x)
            {
                game_driver driver = driver_list.driver(x);
                if (driver != ___empty.driver____empty)
                {
                    var it = available.find(driver.name);
                    bool found = it;  //available.end() != it;
                    m_availsortedlist.emplace_back(new ui_system_info(driver, found));
                    if (found)
                        available.erase(driver.name);  //it);
                }
            }

            //std::stable_sort(
            //        m_availsortedlist.begin(),
            //        m_availsortedlist.end(),
            //        [] (ui_system_info const &a, ui_system_info const &b) { return sorted_game_list(a.driver, b.driver); });
            m_availsortedlist.Sort((a, b) => { return auditmenu_global.sorted_game_list(a.driver, b.driver); });

            file.close();
            return true;
        }


        //-------------------------------------------------
        //  load custom filters info from file
        //-------------------------------------------------
        void load_custom_filters()
        {
            emu_file file = new emu_file(ui().options().ui_path(), osdcore_global.OPEN_FLAG_READ);
            if (file.open("custom_", emulator_info.get_configname(), "_filter.ini") == osd_file.error.NONE)
            {
                machine_filter flt = machine_filter.create(file);
                if (flt != null)
                    main_filters.filters[flt.get_type()] = flt; // not emplace/insert - could replace bogus filter from ui.ini line

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
                str += string.Format("Driver is Clone of\t{0}\n", driver_list.driver((UInt32)cloneof).type.fullname());  //%1$-.100s
            else
                str += "Driver is Parent\t\n";

            if (flags.has_analog())
                str += "Analog Controls\tYes\n";
            if (flags.has_keyboard())
                str += "Keyboard Inputs\tYes\n";

            if (((UInt64)driver.flags & gamedrv_global.MACHINE_NOT_WORKING) != 0)
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

            if ((flags.unemulated_features() & emu.detail.device_feature.type.MICROPHONE) != 0)
                str += "Microphone\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.MICROPHONE) != 0)
                str += "Microphone\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.CAMERA) != 0)
                str += "Camera\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.CAMERA) != 0)
                str += "Camera\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.DISK) != 0)
                str += "Disk\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.DISK) != 0)
                str += "Disk\tImperfect\n";

            if ((flags.unemulated_features() & emu.detail.device_feature.type.PRINTER) != 0)
                str += "Printer\tUnimplemented\n";
            else if ((flags.imperfect_features() & emu.detail.device_feature.type.PRINTER) != 0)
                str += "Printer\tImperfect\n";

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
            str += (((UInt32)flags.machine_flags() & emucore_global.ORIENTATION_SWAP_XY) != 0 ? "Screen Orientation\tVertical\n" : "Screen Orientation\tHorizontal\n");

            bool found = false;
            foreach (romload.region region in new romload.entries(driver.rom).get_regions())
            {
                if (region.is_diskdata())
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
            game_driver driver = menu_event.itemref is ui_system_info ? ((ui_system_info)menu_event.itemref).driver : null;
            int driverint = menu_event.itemref is int || menu_event.itemref is CONF ? (int)menu_event.itemref : -1;

            if (driverint == (int)CONF.CONF_OPTS)
            {
                // special case for configure options

                throw new emu_unimplemented();
#if false
                menu.stack_push(new menu_game_options(ui(), container()));
#endif
            }
            else if (driverint == (int)CONF.CONF_MACHINE)
            {
                // special case for configure machine

                throw new emu_unimplemented();
#if false
                if (m_prev_selected != null)
                    menu.stack_push(new menu_machine_configure(ui(), container(), (game_driver)m_prev_selected));
#endif
                return;
            }
            else if (driverint == (int)CONF.CONF_PLUGINS)
            {
                // special case for configure plugins

                throw new emu_unimplemented();
#if false
                menu.stack_push(new menu_plugins_configure(ui(), container()));
#endif
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
#if false
                            menu.stack_push(new menu_select_software(ui(), container(), driver));
#endif
                            return;
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

            if (ui_swinfoint == (int)CONF.CONF_OPTS)
            {
                throw new emu_unimplemented();
#if false
                // special case for configure options
                menu.stack_push(new menu_game_options(ui(), container()));
#endif
            }
            else if (ui_swinfoint == (int)CONF.CONF_MACHINE)
            {
                // special case for configure machine
                if (m_prev_selected != null)
                {
                    ui_software_info swinfo = (ui_software_info)m_prev_selected;

                    throw new emu_unimplemented();
#if false
                    menu.stack_push(new menu_machine_configure(ui(), container(), (game_driver)swinfo.driver));
#endif
                }
                return;
            }
            else if (ui_swinfoint == (int)CONF.CONF_PLUGINS)
            {
                throw new emu_unimplemented();
#if false
                // special case for configure plugins
                menu.stack_push(new menu_plugins_configure(ui(), container()));
#endif
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
#if false
                    foreach (software_list_device swlistdev in new software_list_device_iterator(enumerator.config().root_device()))
                    {
                        if (!swlistdev.get_info().empty())
                        {
                            menu.stack_push(new menu_select_software(ui(), container(), ui_swinfo.driver));
                            return;
                        }
                    }

                    // if everything looks good, schedule the new driver
                    if (!select_bios(ui_swinfo.driver, false))
                    {
                        reselect_last.reselect(true);
                        launch_system(ui_swinfo.driver);
                    }
#endif
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
#if false
                    if (!select_bios(ui_swinfo, false) && !select_part(swinfo, ui_swinfo))
                        launch_system(drv.driver(), ui_swinfo, ui_swinfo.part);
#endif
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
