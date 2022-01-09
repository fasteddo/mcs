// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using icon_cache = mame.util.lru_cache_map<mame.game_driver, mame.ui.menu_select_game.texture_and_bitmap>;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using size_t = System.UInt64;
using software_list_device_enumerator = mame.device_type_enumerator<mame.software_list_device>;  //typedef device_type_enumerator<software_list_device> software_list_device_enumerator;
using system_list_system_reference_vector = mame.std.vector<mame.ui_system_info>;  //using system_reference_vector = std::vector<system_reference>;
using system_list_system_vector = mame.std.vector<mame.ui_system_info>;  //using system_vector = std::vector<ui_system_info>;
using u32 = System.UInt32;
using u64 = System.UInt64;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;

using static mame.cpp_global;
using static mame.emucore_global;
using static mame.emuopts_global;
using static mame.gamedrv_global;
using static mame.language_global;
using static mame.osdfile_global;
using static mame.ui.auditmenu_global;
using static mame.unicode_global;
using static mame.util;
using static mame.utils_global;
using static mame.version_global;


namespace mame.ui
{
    class menu_select_game : menu_select_launch, IDisposable
    {
        //using icon_cache = texture_lru<game_driver const *>;

        //enum
        //{
        const int CONF_OPTS    = 1;
        const int CONF_MACHINE = 2;
        //}


        const uint32_t FLAGS_UI = FLAG_LEFT_ARROW | FLAG_RIGHT_ARROW;


        //static std.vector<game_driver> m_sortedlist = new std.vector<game_driver>();
        //std.vector<ui_system_info> m_availsortedlist = new std.vector<ui_system_info>();
        //std.vector<ui_system_info> m_displaylist = new std.vector<ui_system_info>();

        //game_driver [] m_searchlist = new game_driver[VISIBLE_GAMES_IN_SEARCH + 1];

        system_list m_persistent_data;
        icon_cache m_icons;
        string m_icon_paths;
        std.vector<ui_system_info> m_displaylist;  //std::vector<std::reference_wrapper<ui_system_info const> > m_displaylist;

        std.vector<std.pair<double, ui_system_info>> m_searchlist;  //std::vector<std::pair<double, std::reference_wrapper<ui_system_info const> > > m_searchlist;
        unsigned m_searched_fields;
        bool m_populated_favorites;

        static bool s_first_start = true;


        //-------------------------------------------------
        //  ctor
        //-------------------------------------------------
        menu_select_game(mame_ui_manager mui, render_container container, string gamename)
            : base(mui, container, false)
        {
            m_persistent_data = system_list.instance();
            m_icons = new icon_cache(MAX_ICONS_RENDER);
            m_icon_paths = "";
            m_displaylist = new std.vector<ui_system_info>();
            m_searchlist = new std.vector<std.pair<double, ui_system_info>>();
            m_searched_fields = (unsigned)system_list.available.AVAIL_NONE;
            m_populated_favorites = false;


            string error_string;
            string last_filter;
            ui_options moptions = mui.options();

            // load drivers cache
            m_persistent_data.cache_data(mui.options());

            // check if there are available system icons
            check_for_icons(null);

            // build drivers list
            if (!load_available_machines())
                build_available_list();

            if (s_first_start)
            {
                //s_first_start = false; TODO: why wasn't it ever clearing the first start flag?

                reselect_last.set_driver(moptions.last_used_machine());
                ui_globals.rpanel = (uint8_t)std.min(std.max(moptions.last_right_panel(), (int)RP_FIRST), (int)RP_LAST);

                string tmp = moptions.last_used_filter();
                size_t found = tmp.find_first_of(',');
                string fake_ini;
                if (found == npos)
                {
                    fake_ini = util.string_format("\uFEFF{0} = 1\n", tmp);
                }
                else
                {
                    string sub_filter = tmp.substr(found + 1);
                    tmp = tmp.Substring((int)found);  // .resize(found);
                    fake_ini = util.string_format("\uFEFF{0} = {1}\n", tmp, sub_filter);
                }

                emu_file file = new emu_file(ui().options().ui_path(), OPEN_FLAG_READ);

                MemoryU8 fake_ini_buffer = new MemoryU8(System.Text.Encoding.ASCII.GetBytes(fake_ini));
                if (!file.open_ram(fake_ini_buffer, (u32)fake_ini.size()))  //if (!file.open_ram(fake_ini.c_str(), fake_ini.size()))
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

            mui.machine().options().set_value(OPTION_SNAPNAME, "%g/%i", OPTION_PRIORITY_CMDLINE);

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
            base.Dispose();

            //string error_string;
            string last_driver = "";
            ui_system_info system;
            ui_software_info swinfo;
            get_selection(out swinfo, out system);
            if (swinfo != null)
                last_driver = swinfo.shortname;
            else if (system != null)
                last_driver = system.driver.name;

            string filter = m_persistent_data.filter_data().get_config_string();

            ui_options mopt = ui().options();
            mopt.set_value(ui_options.OPTION_LAST_RIGHT_PANEL, ui_globals.rpanel, OPTION_PRIORITY_CMDLINE);
            mopt.set_value(ui_options.OPTION_LAST_USED_FILTER, filter, OPTION_PRIORITY_CMDLINE);
            mopt.set_value(ui_options.OPTION_LAST_USED_MACHINE, last_driver, OPTION_PRIORITY_CMDLINE);
            mopt.set_value(ui_options.OPTION_HIDE_PANELS, ui_globals.panels_status, OPTION_PRIORITY_CMDLINE);
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
            // drop any existing menus and start the system selection menu
            menu.stack_reset(mui);
            menu.stack_push_special_main(new menu_select_game(mui, container, null));
            mui.show_menu();

            // make sure MAME is paused
            mui.machine().pause();
        }


        protected override void menu_activated()
        {
            throw new emu_unimplemented();
        }


        protected override void populate(ref float customtop, ref float custombottom)
        {
            foreach (var icon in m_icons) // TODO: why is this here?  maybe better on resize or setting change?
                icon.second().texture = null;  //icon.second().texture.reset();

            set_switch_image();
            bool have_prev_selected = false;
            int old_item_selected = -1;

            if (!isfavorite())
            {
                if (m_populated_favorites)
                    m_prev_selected = null;

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
                            if (flt.apply(it.second))
                                m_displaylist.emplace_back(it.second);
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
                            m_displaylist.Add(it.second);
                    }
                }
                else
                {
                    // if filter is set on category, build category list
                    var sorted = m_persistent_data.sorted_list();
                    if (flt == null)
                    {
                        foreach (ui_system_info sysinfo in sorted)
                            m_displaylist.emplace_back(sysinfo);
                    }
                    else
                    {
                        foreach (ui_system_info sysinfo in sorted)
                        {
                            if (flt.apply(sysinfo))
                                m_displaylist.emplace_back(sysinfo);
                        }
                    }
                }

                // iterate over entries
                int curitem = 0;
                foreach (ui_system_info elem in m_displaylist)
                {
                    have_prev_selected = have_prev_selected || (elem == m_prev_selected);
                    if ((old_item_selected == -1) && (elem.driver.name == reselect_last.driver()))
                        old_item_selected = curitem;

                    item_append(elem.description, elem.is_clone ? FLAG_INVERT : 0, elem);
                    curitem++;
                }
            }
            else
            {
                // populate favorites list
                if (!m_populated_favorites)
                    m_prev_selected = null;
                m_populated_favorites = true;
                m_search = "";  //m_search.clear();
                int curitem = 0;

                mame_machine_manager.instance().favorite().apply_sorted(
                (info) =>  //[this, &have_prev_selected, &old_item_selected, curitem = 0] (ui_software_info const &info) mutable
                {
                    have_prev_selected = have_prev_selected || (info == (ui_software_info)m_prev_selected);
                    if (info.startempty != 0)
                    {
                        if (old_item_selected == -1 && info.shortname == reselect_last.driver())
                            old_item_selected = curitem;

                        bool cloneof = std.strcmp(info.driver.parent, "0") != 0;
                        if (cloneof)
                        {
                            int cx = driver_list.find(info.driver.parent);
                            if ((0 <= cx) && ((driver_list.driver((size_t)cx).flags & machine_flags.type.IS_BIOS_ROOT) != 0))
                                cloneof = false;
                        }

                        item_append(info.longname, cloneof ? FLAG_INVERT : 0, info);
                    }
                    else
                    {
                        if (old_item_selected == -1 && info.shortname == reselect_last.driver())
                            old_item_selected = curitem;

                        item_append(info.longname, info.devicetype, info.parentname.empty() ? 0 : FLAG_INVERT, info);
                    }

                    curitem++;
                });
            }

            // add special items
            if (stack_has_special_main_menu())
            {
                item_append(menu_item_type.SEPARATOR, 0);
                item_append(__("Configure Options"), 0, CONF_OPTS);
                item_append(__("Configure Machine"), 0, CONF_MACHINE);
                skip_main_items = 3;

                if (m_prev_selected != null && !have_prev_selected)
                    m_prev_selected = item(0).ref_();
            }
            else
            {
                skip_main_items = 0;
            }

            // configure the custom rendering
            customtop = 3.0f * ui().get_line_height() + 5.0f * ui().box_tb_border();
            custombottom = 4.0f * ui().get_line_height() + 3.0f * ui().box_tb_border();

            // reselect prior game launched, if any
            if (old_item_selected != -1)
            {
                set_selected_index(old_item_selected);
                if (ui_globals.visible_main_lines == 0)
                    top_line = (selected_index() != 0) ? selected_index() - 1 : 0;
                else
                    top_line = selected_index() - (ui_globals.visible_main_lines / 2);

                if (reselect_last.software().empty())
                    reselect_last.reset();
            }
            else
            {
                reselect_last.reset();
            }
        }


        class cache_reset : IDisposable
        { 
            ~cache_reset()
            {
                //assert(m_isDisposed);  // can remove

                //system_list.instance().reset_cache();
            }

            bool m_isDisposed = false;
            public void Dispose()
            {
                system_list.instance().reset_cache();
            }
        }


        //-------------------------------------------------
        //  handle
        //-------------------------------------------------
        protected override void handle(event_ ev)
        {
            if (m_prev_selected != null)
                m_prev_selected = item(0).ref_();

            // if I have to reselect a software, force software list submenu
            if (reselect_last.get())
            {
                // FIXME: this is never hit, need a better way to return to software selection if necessary

                ui_system_info system;
                ui_software_info software;
                get_selection(out software, out system);

                throw new emu_unimplemented();
#if false
                menu::stack_push<menu_select_software>(ui(), container(), *system);
#endif
                return;
            }

            // FIXME: everything above here used to run before events were processed

            // process the menu
            if (ev != null)
            {
                if (dismiss_error())
                {
                    // reset the error on any subsequent menu event
                }
                else
                {
                switch (ev.iptkey)
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

                case (int)ioport_type.IPT_UI_EXPORT:
                    inkey_export();
                    break;

                case (int)ioport_type.IPT_UI_DATS:
                    inkey_dats();
                    break;

                default:
                    if (ev.itemref != null)
                    {
                        switch (ev.iptkey)
                        {
                        case (int)ioport_type.IPT_UI_SELECT:
                            if (get_focus() == focused_menu.MAIN)
                            {
                                if (m_populated_favorites)
                                    inkey_select_favorite(ev);
                                else
                                    inkey_select(ev);
                            }
                            break;

                        case (int)ioport_type.IPT_CUSTOM:
                            // handle IPT_CUSTOM (mouse right click)
                            if (!m_populated_favorites)
                            {
                                throw new emu_unimplemented();
#if false
#endif
                            }
                            else
                            {
                                throw new emu_unimplemented();
#if false
#endif
                            }
                            break;

                        case (int)ioport_type.IPT_UI_LEFT:
                            if (ui_globals.rpanel == RP_IMAGES)
                            {
                                // Images
                                previous_image_view();
                            }
                            else if (ui_globals.rpanel == RP_INFOS)
                            {
                                // Infos
                                change_info_pane(-1);
                            }
                            break;

                        case (int)ioport_type.IPT_UI_RIGHT:
                            if (ui_globals.rpanel == RP_IMAGES)
                            {
                                // Images
                                next_image_view();
                            }
                            else if (ui_globals.rpanel == RP_INFOS)
                            {
                                // Infos
                                change_info_pane(1);
                            }
                            break;

                        case (int)ioport_type.IPT_UI_FAVORITES:
                            throw new emu_unimplemented();
#if false
#endif
                            break;

                        case (int)ioport_type.IPT_UI_AUDIT:
                            throw new emu_unimplemented();
#if false
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
        protected override void get_selection(out ui_software_info software, out ui_system_info system)
        {
            if (m_populated_favorites)
            {
                software = (ui_software_info)get_selection_ptr();
                system = software != null ? m_persistent_data.systems()[driver_list.find(software.driver.name)] : null;
            }
            else
            {
                software = null;
                system = (ui_system_info)get_selection_ptr();
            }
        }


        protected override bool accept_search() { return !isfavorite(); }


        // text for main top/bottom panels
        protected override void make_topbox_text(out string line0, out string line1, out string line2)
        {
            line0 = "";
            line1 = "";
            line2 = "";

            line0 = string_format(__("{0} {1} ( {2} / {3} machines ({4} BIOS) )"),
                    emulator_info.get_appname(),
                    bare_build_version,
                    m_available_items,
                    (driver_list.total() - 1),
                    m_persistent_data.bios_count());

            if (m_populated_favorites)
            {
                line1 = line1.clear_();
            }
            else
            {
                machine_filter it = m_persistent_data.filter_data().get_current_filter();
                string filter = it != null ? it.filter_text() : null;
                if (filter != null)
                    line1 = string_format(__("{0}: {1} - Search: {2}_"), it.display_name(), filter, m_search);  // %1$s: %2$s - Search: %3$s_
                else
                    line1 = string_format(__("Search: {0}_"), m_search);  // %1$s_
            }

            line2 = line2.clear_();
        }


        protected override string make_software_description(ui_software_info software, ui_system_info system)
        {
            // first line is system
            return string_format(__("System: {0}"), system.description);  //return string_format(_("System: %1$-.100s"), system->description);
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
                                if (!file.open(util.string_format("custom_{0}_filter.ini", emulator_info.get_configname())))
                                {
                                    filter.save_ini(file, 0);
                                    file.close();
                                }
                            }
                            m_persistent_data.filter_data().set_current_filter_type(new_type);
                            reset(reset_options.REMEMBER_REF);
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
#if false
#endif
        }


        // internal methods

        //-------------------------------------------------
        //  change what's displayed in the info box
        //-------------------------------------------------
        void change_info_pane(int delta)
        {
            throw new emu_unimplemented();
#if false
#endif
        }


        //-------------------------------------------------
        //  build a list of available drivers
        //-------------------------------------------------
        void build_available_list()
        {
            size_t total = driver_list.total();
            std.vector<bool> included = new std.vector<bool>(total, false);

            // iterate over ROM directories and look for potential ROMs
            file_enumerator path = new file_enumerator(machine().options().media_path());
            for (osd.directory.entry dir = path.next(); dir != null; dir = path.next())
            {
                string drivername;  //char drivername[50];
                int dstIdx = 0;  // char *dst = drivername;
                int srcIdx = 0;  //const char *src;

                // build a name for it
                //for (src = dir->name; *src != 0 && *src != '.' && dst < &drivername[std::size(drivername) - 1]; ++src)
                //    *dst++ = tolower(uint8_t(*src));
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
                var sorted = m_persistent_data.sorted_list();
                m_searchlist.reserve(sorted.size());
                foreach (ui_system_info info in sorted)
                    m_searchlist.emplace_back(new std.pair<double, ui_system_info>(1.0, info));
            }

            // keep track of what we matched against
            string ucs_search = ustr_from_utf8(normalize_unicode(m_search, unicode_normalization_form.D, true));  //const std::u32string ucs_search(ustr_from_utf8(normalize_unicode(m_search, unicode_normalization_form::D, true)));

            // check available search data
            if (m_persistent_data.is_available(system_list.available.AVAIL_UCS_SHORTNAME))
                m_searched_fields |= (unsigned)system_list.available.AVAIL_UCS_SHORTNAME;
            if (m_persistent_data.is_available(system_list.available.AVAIL_UCS_DESCRIPTION))
                m_searched_fields |= (unsigned)system_list.available.AVAIL_UCS_DESCRIPTION;
            if (m_persistent_data.is_available(system_list.available.AVAIL_UCS_MANUF_DESC))
                m_searched_fields |= (unsigned)system_list.available.AVAIL_UCS_MANUF_DESC;
            if (m_persistent_data.is_available(system_list.available.AVAIL_UCS_DFLT_DESC))
                m_searched_fields |= (unsigned)system_list.available.AVAIL_UCS_DFLT_DESC;
            if (m_persistent_data.is_available(system_list.available.AVAIL_UCS_MANUF_DFLT_DESC))
                m_searched_fields |= (unsigned)system_list.available.AVAIL_UCS_MANUF_DFLT_DESC;

            for (int i = 0; i < m_searchlist.Count; i++)  //for (std::pair<double, std::reference_wrapper<ui_system_info const> > &info : m_searchlist)
            {
                var info = m_searchlist[i];

                m_searchlist[i] = std.make_pair(1.0, info.second);
                ui_system_info sys = info.second;

                // match shortnames
                if ((m_searched_fields & (unsigned)system_list.available.AVAIL_UCS_SHORTNAME) != 0)
                    m_searchlist[i] = std.make_pair(util.edit_distance(ucs_search, sys.ucs_shortname), info.second);

                // match reading
                if (info.first != 0 && !sys.ucs_reading_description.empty())
                {
                    m_searchlist[i] = std.make_pair(std.min(util.edit_distance(ucs_search, sys.ucs_reading_description), info.first), info.second);

                    // match "<manufacturer> <reading>"
                    if (info.first != 0)
                        m_searchlist[i] = std.make_pair(std.min(util.edit_distance(ucs_search, sys.ucs_manufacturer_reading_description), info.first), info.second);
                }

                // match descriptions
                if (info.first != 0 && (m_searched_fields & (unsigned)system_list.available.AVAIL_UCS_DESCRIPTION) != 0)
                    m_searchlist[i] = std.make_pair(std.min(util.edit_distance(ucs_search, sys.ucs_description), info.first), info.second);

                // match "<manufacturer> <description>"
                if (info.first != 0 && (m_searched_fields & (unsigned)system_list.available.AVAIL_UCS_MANUF_DESC) != 0)
                    m_searchlist[i] = std.make_pair(std.min(util.edit_distance(ucs_search, sys.ucs_manufacturer_description), info.first), info.second);

                // match default description
                if (info.first != 0 && (m_searched_fields & (unsigned)system_list.available.AVAIL_UCS_DFLT_DESC) != 0 && !sys.ucs_default_description.empty())
                {
                    m_searchlist[i] = std.make_pair(std.min(util.edit_distance(ucs_search, sys.ucs_default_description), info.first), info.second);

                    // match "<manufacturer> <default description>"
                    if (info.first != 0 && (m_searched_fields & (unsigned)system_list.available.AVAIL_UCS_MANUF_DFLT_DESC) != 0)
                        m_searchlist[i] = std.make_pair(std.min(util.edit_distance(ucs_search, sys.ucs_manufacturer_default_description), info.first), info.second);
                }
            }

            // sort according to edit distance
            //std::stable_sort(
            //        m_searchlist.begin(),
            //        m_searchlist.end());
            //        [] (auto const &lhs, auto const &rhs) { return lhs.first < rhs.first; });
            m_searchlist.Sort((lhs, rhs) => { return lhs.first.CompareTo(rhs.first); });
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
            if (file.open(emulator_info.get_configname() + "_avail.ini"))
                return false;

            string rbuf;  //char rbuf[MAX_CHAR_INFO];
            string readbuf;
            file.gets(out rbuf, MAX_CHAR_INFO);
            file.gets(out rbuf, MAX_CHAR_INFO);
            readbuf = chartrimcarriage(rbuf);
            string a_rev = string_format("{0}{1}", UI_VERSION_TAG, bare_build_version);

            // version not matching ? exit
            if (a_rev != readbuf)
            {
                file.close();
                return false;
            }

            // load available list
            std.unordered_set<string> available = new std.unordered_set<string>();
            while (file.gets(out rbuf, MAX_CHAR_INFO) != null)
            {
                readbuf = rbuf.Trim();  //readbuf = strtrimspace(rbuf);

                if (readbuf.empty() || ('#' == readbuf[0])) // ignore empty lines and line comments
                { }
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
            if (!file.open(util.string_format("custom_{0}_filter.ini", emulator_info.get_configname())))
            {
                machine_filter flt = machine_filter.create(file, m_persistent_data.filter_data());
                if (flt != null)
                    m_persistent_data.filter_data().set_filter(flt); // not emplace/insert - could replace bogus filter from ui.ini line

                file.close();
            }
        }


        // handlers

        //-------------------------------------------------
        //  handle select key event
        //-------------------------------------------------
        void inkey_select(event_ menu_event)
        {
            var system = (ui_system_info)menu_event.itemref;
            int driverint = menu_event.itemref is int ? (int)menu_event.itemref : -1;

            if (driverint == CONF_OPTS)
            {
                // special case for configure options

                throw new emu_unimplemented();
#if false
#endif
            }
            else if (driverint == CONF_MACHINE)
            {
                // special case for configure machine

                throw new emu_unimplemented();
#if false
#endif
            }
            else
            {
                // anything else is a driver

                driver_enumerator enumerator = new driver_enumerator(machine().options(), system.driver);
                enumerator.next();

                // if there are software entries, show a software selection menu
                foreach (software_list_device swlistdev in new software_list_device_enumerator(enumerator.config().root_device()))
                {
                    if (!swlistdev.get_info().empty())
                    {
                        throw new emu_unimplemented();
#if false
#endif
                    }
                }

                // audit the system ROMs first to see if we're going to work
                media_auditor auditor = new media_auditor(enumerator);
                media_auditor.summary summary = auditor.audit_media(media_auditor.AUDIT_VALIDATE_FAST);


                // MCS - always pass audit
                //throw new emu_unimplemented();
                summary = media_auditor.summary.CORRECT;


                // if everything looks good, schedule the new driver
                if (audit_passed(summary))
                {
                    if (!select_bios(system.driver, false))
                        launch_system(system.driver);
                }
                else
                {
                    // otherwise, display an error
                    set_error(reset_options.REMEMBER_REF, make_system_audit_fail_text(auditor, summary));
                }
            }
        }


        //-------------------------------------------------
        //  handle select key event for favorites menu
        //-------------------------------------------------
        void inkey_select_favorite(event_ menu_event)
        {
            ui_software_info ui_swinfo = (ui_software_info)menu_event.itemref;
            int ui_swinfoint = (int)menu_event.itemref;

            if (ui_swinfoint == CONF_OPTS)
            {
                throw new emu_unimplemented();
#if false
#endif
            }
            else if (ui_swinfoint == CONF_MACHINE)
            {
                // special case for configure machine
                if (m_prev_selected != null)
                {
                    ui_software_info swinfo = (ui_software_info)m_prev_selected;

                    throw new emu_unimplemented();
#if false
#endif
                }
                return;
            }
            else if (ui_swinfo.startempty == 1)
            {
                driver_enumerator enumerator = new driver_enumerator(machine().options(), ui_swinfo.driver);
                enumerator.next();

                // if there are software entries, show a software selection menu
                foreach (software_list_device swlistdev in new software_list_device_enumerator(enumerator.config().root_device()))
                {
                    if (!swlistdev.get_info().empty())
                    {
                        throw new emu_unimplemented();
#if false
#endif
                    }
                }

                // audit the system ROMs first to see if we're going to work
                media_auditor auditor = new media_auditor(enumerator);
                media_auditor.summary summary = auditor.audit_media(media_auditor.AUDIT_VALIDATE_FAST);

                if (audit_passed(summary))
                {
                    // if everything looks good, schedule the new driver
                    if (!select_bios(ui_swinfo.driver, false))
                    {
                        reselect_last.reselect(true);
                        launch_system(ui_swinfo.driver);
                    }
                }
                else
                {
                    // otherwise, display an error
                    set_error(reset_options.REMEMBER_REF, make_system_audit_fail_text(auditor, summary));
                }
            }
            else
            {
                // first audit the system ROMs
                driver_enumerator drv = new driver_enumerator(machine().options(), ui_swinfo.driver);
                media_auditor auditor = new media_auditor(drv);
                drv.next();

                media_auditor.summary sysaudit = auditor.audit_media(media_auditor.AUDIT_VALIDATE_FAST);
                if (!audit_passed(sysaudit))
                {
                    set_error(reset_options.REMEMBER_REF, make_system_audit_fail_text(auditor, sysaudit));
                }
                else
                {
                    // now audit the software
                    software_list_device swlist = software_list_device.find_by_name(drv.config(), ui_swinfo.listname);
                    software_info swinfo = swlist.find(ui_swinfo.shortname);

                    media_auditor.summary swaudit = auditor.audit_software(swlist, swinfo, media_auditor.AUDIT_VALIDATE_FAST);

                    if (audit_passed(swaudit))
                    {
                        throw new emu_unimplemented();
#if false
#endif
                    }
                    else
                    {
                        // otherwise, display an error
                        set_error(reset_options.REMEMBER_REF, make_software_audit_fail_text(auditor, swaudit));
                    }
                }
            }
        }
    }
}
