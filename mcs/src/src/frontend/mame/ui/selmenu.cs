// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using bitmap_vector = mame.std.vector<mame.bitmap_argb32>;
using cache_ptr_map = mame.std.map<mame.running_machine, mame.ui.menu_select_launch.cache>;
using device_t_feature = mame.emu.detail.device_feature;  //using feature = emu::detail::device_feature;
using device_t_feature_type = mame.emu.detail.device_feature.type;  //using feature_type = emu::detail::device_feature::type;
using flags_cache = mame.util.lru_cache_map<mame.game_driver, mame.ui.menu_select_launch.system_flags>;
using int32_t = System.Int32;
using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;
using PointerU8 = mame.Pointer<System.Byte>;
using s_bios = mame.std.vector<mame.std.pair<string, int>>;
using s_parts = mame.std.unordered_map<string, string>;
using size_t = System.UInt64;
using texture_ptr = mame.render_texture;
using texture_ptr_vector = mame.std.vector<mame.render_texture>;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;

using static mame.cpp_global;
using static mame.emucore_global;
using static mame.emuopts_global;
using static mame.language_global;
using static mame.osdcore_global;
using static mame.osdfile_global;
using static mame.render_global;
using static mame.rendertypes_global;
using static mame.rendfont_global;
using static mame.rendutil_global;
using static mame.ui.defimg_global;
using static mame.ui.selmenu_global;
using static mame.ui.starimg_global;
using static mame.ui.toolbar_global;
using static mame.ui_global;
using static mame.unicode_global;
using static mame.util;
using static mame.utils_global;
using static mame.version_global;


namespace mame.ui
{
    static class selmenu_global
    {
        public static void load_image(out bitmap_argb32 bitmap, emu_file file, string base_)
        {
            bitmap = new bitmap_argb32();

            if (!file.open(base_ + ".png"))
            {
                render_load_png(out bitmap, file.core_file_get());
                file.close();
            }

            if (!bitmap.valid() && !file.open(base_ + ".jpg"))
            {
                render_load_jpeg(out bitmap, file.core_file_get());
                file.close();
            }

            if (!bitmap.valid() && !file.open(base_ + ".bmp"))
            {
                render_load_msdib(out bitmap, file.core_file_get());
                file.close();
            }
        }


        public static void load_driver_image(out bitmap_argb32 bitmap, emu_file file, game_driver driver)
        {
            // try to load snapshot first from saved "0000.png" file
            string fullname = driver.name;
            load_image(out bitmap, file, util.path_concat(fullname, "0000"));

            // if fail, attempt to load from standard file
            if (!bitmap.valid())
                load_image(out bitmap, file, fullname);

            // if fail again, attempt to load from parent file
            if (!bitmap.valid())
            {
                // ignore BIOS sets
                bool isclone = std.strcmp(driver.parent, "0") != 0;
                if (isclone)
                {
                    int cx = driver_list.find(driver.parent);
                    if ((0 <= cx) && (driver_list.driver((size_t)cx).flags & machine_flags.type.IS_BIOS_ROOT) != 0)
                        isclone = false;
                }

                if (isclone)
                {
                    fullname = driver.parent;
                    load_image(out bitmap, file, util.path_concat(fullname, "0000"));

                    if (!bitmap.valid())
                        load_image(out bitmap, file, fullname);
                }
            }
        }
    }


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


        //enum
        //{
        public const uint8_t FIRST_VIEW    = 0;
        public const uint8_t SNAPSHOT_VIEW = FIRST_VIEW;
        public const uint8_t CABINETS_VIEW = FIRST_VIEW +  1;
        const uint8_t CPANELS_VIEW  = FIRST_VIEW +  2;
        const uint8_t PCBS_VIEW     = FIRST_VIEW +  3;
        const uint8_t FLYERS_VIEW   = FIRST_VIEW +  4;
        const uint8_t TITLES_VIEW   = FIRST_VIEW +  5;
        const uint8_t ENDS_VIEW     = FIRST_VIEW +  6;
        const uint8_t ARTPREV_VIEW  = FIRST_VIEW +  7;
        const uint8_t BOSSES_VIEW   = FIRST_VIEW +  8;
        const uint8_t LOGOS_VIEW    = FIRST_VIEW +  9;
        const uint8_t VERSUS_VIEW   = FIRST_VIEW + 10;
        const uint8_t GAMEOVER_VIEW = FIRST_VIEW + 11;
        const uint8_t HOWTO_VIEW    = FIRST_VIEW + 12;
        const uint8_t SCORES_VIEW   = FIRST_VIEW + 13;
        const uint8_t SELECT_VIEW   = FIRST_VIEW + 14;
        const uint8_t MARQUEES_VIEW = FIRST_VIEW + 15;
        const uint8_t COVERS_VIEW   = FIRST_VIEW + 16;
        const uint8_t LAST_VIEW     = COVERS_VIEW;
        //}


        public class texture_and_bitmap
        {
            public texture_ptr texture;
            bitmap_argb32 bitmap;

            //template <typename T>
            texture_and_bitmap(texture_ptr tex) { texture = tex; }  //texture_and_bitmap(T &&tex) : texture(std::forward<T>(tex)) { }
            //texture_and_bitmap(texture_and_bitmap &&that) = default;
            //texture_and_bitmap &operator=(texture_and_bitmap &&that) = default;
        }

        //template <typename Key, typename Compare = std::less<Key> >
        //using texture_lru = util::lru_cache_map<Key, texture_and_bitmap, Compare>;


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

            bitmap_vector m_toolbar_bitmaps;
            texture_ptr_vector m_toolbar_textures;


            public cache(running_machine machine)
            {
                m_snapx_bitmap = new bitmap_argb32(0, 0);
                m_snapx_texture = null;  //, m_snapx_texture(nullptr, machine.render())
                m_snapx_driver = null;
                m_snapx_software = null;
                m_no_avail_bitmap = new bitmap_argb32(256, 256);
                m_toolbar_bitmaps = new bitmap_vector();
                m_toolbar_textures = new texture_ptr_vector();


                render_manager render = machine.render();

                // create a texture for snapshot
                m_snapx_texture = render.texture_alloc(render_texture.hq_scale);  //m_snapx_texture.reset(render.texture_alloc(render_texture::hq_scale));

                //std::memcpy(&m_no_avail_bitmap.pix(0), no_avail_bmp, 256 * 256 * sizeof(uint32_t));
                PointerU32 m_no_avail_bitmapBuf = m_no_avail_bitmap.pix(0);
                for (int i = 0; i < 256 * 256; i++)
                    m_no_avail_bitmapBuf[i] = no_avail_bmp[i];

                m_toolbar_bitmaps.resize(UI_TOOLBAR_BUTTONS);
                m_toolbar_textures.reserve(UI_TOOLBAR_BUTTONS);
            }

            public bitmap_argb32 snapx_bitmap() { return m_snapx_bitmap; }
            public render_texture snapx_texture() { return m_snapx_texture; }
            public bool snapx_driver_is(game_driver value) { return m_snapx_driver == value; }
            public bool snapx_software_is(ui_software_info software) { return m_snapx_software == software; }
            public void set_snapx_driver(game_driver value) { m_snapx_driver = value; }
            public void set_snapx_software(ui_software_info software) { m_snapx_software = software; }

            public bitmap_argb32 no_avail_bitmap() { return m_no_avail_bitmap; }

            public bitmap_vector toolbar_bitmaps() { return m_toolbar_bitmaps; }
            public texture_ptr_vector toolbar_textures() { return m_toolbar_textures; }


            public void cache_toolbar(running_machine machine, float width, float height)
            {
                // not bothering to transform for non-square pixels greatly simplifies this
                render_manager render = machine.render();
                render_target target = render.ui_target();
                int32_t pix_size = (int32_t)std.ceil(std.max(width * target.width(), height * target.height()));
                if (m_toolbar_textures.empty() || (m_toolbar_bitmaps[0].width() != pix_size) || (m_toolbar_bitmaps[0].height() != pix_size))
                {
                    throw new emu_unimplemented();
#if false
                    m_toolbar_textures.clear();
                    util.nsvg_rasterizer_ptr rasterizer = nsvgCreateRasterizer();
                    string xml;
                    for (unsigned i = 0; UI_TOOLBAR_BUTTONS > i; ++i)
                    {
                        // parse SVG and calculate scale
                        xml = toolbar_icons_svg[i];
                        util.nsvg_image_ptr svg = nsvgParse(xml.data(), "px", 72);
                        float xscale = (float)pix_size / svg->width;
                        float yscale = (float)pix_size / svg->height;
                        float drawscale = std.max(xscale, yscale);

                        // rasterise the SVG and clear it out of memory
                        bitmap_argb32 bitmap = m_toolbar_bitmaps[i];
                        bitmap.resize(pix_size, pix_size);
                        nsvgRasterize(
                                rasterizer.get(),
                                svg.get(),
                                0, 0, drawscale,
                                reinterpret_cast<char *>(bitmap.pix(0)),  //reinterpret_cast<unsigned char *>(&bitmap.pix(0)),
                                pix_size, pix_size,
                                bitmap.rowbytes());
                        svg.reset();

                        // correct colour format
                        for (int32_t y = 0; bitmap.height() > y; ++y)
                        {
                            PointerU32 dst = bitmap.pix(y);  //uint32_t *dst(&bitmap.pix(y));
                            for (int32_t x = 0; bitmap.width() > x; ++x, ++dst)
                            {
                                PointerU8 src = new PointerU8(dst);  //u8 const *const src(reinterpret_cast<u8 const *>(dst));
                                rgb_t d = new rgb_t(src[3], src[0], src[1], src[2]);
                                *dst = d;
                            }
                        }

                        // make a texture
                        render_texture texture = m_toolbar_textures.emplace_back(render.texture_alloc(), render);  //render_texture &texture(*m_toolbar_textures.emplace_back(render.texture_alloc(), render));
                        texture.set_bitmap(bitmap, bitmap.cliprect(), texture_format.TEXFORMAT_ARGB32);
                    }
#endif
                }
            }
        }


        // this is to satisfy the std::any requirement that objects be copyable
        class cache_wrapper : cache
        {
            running_machine m_machine;


            public cache_wrapper(running_machine machine) : base(machine) { m_machine = machine; }
            public cache_wrapper(cache_wrapper that) : base(that.m_machine) { m_machine = that.m_machine; }
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


            protected override void custom_render(object selectedref, float top, float bottom, float x, float y, float x2, float y2)
            {
                throw new emu_unimplemented();
            }


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


            protected override void custom_render(object selectedref, float top, float bottom, float x, float y, float x2, float y2)
            {
                throw new emu_unimplemented();
            }


            //-------------------------------------------------
            //  populate
            //-------------------------------------------------
            protected override void populate(ref float customtop, ref float custombottom)
            {
                foreach (var elem in m_bios)
                    item_append(elem.first, "", 0, elem.first);

                item_append(menu_item_type.SEPARATOR);
                customtop = ui().get_line_height() + (3.0f * ui().box_tb_border());
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
        //using flags_cache = util::lru_cache_map<game_driver const *, system_flags>;
        //using icon_cache = util::lru_cache_map<game_driver const *, std::pair<texture_ptr, bitmap_argb32> >;


        protected const size_t MAX_ICONS_RENDER = 128;
        protected const size_t MAX_VISIBLE_SEARCH = 200;


        static readonly std.pair<string, string> [] arts_info = new std.pair<string, string>[]
        {
            new std.pair<string, string>(N_p("selmenu-artwork", "Snapshots"),       OPTION_SNAPSHOT_DIRECTORY),
            new std.pair<string, string>(N_p("selmenu-artwork", "Cabinet"),         ui_options.OPTION_CABINETS_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "Control Panel"),   ui_options.OPTION_CPANELS_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "PCB"),             ui_options.OPTION_PCBS_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "Flyer"),           ui_options.OPTION_FLYERS_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "Title Screen"),    ui_options.OPTION_TITLES_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "Ending"),          ui_options.OPTION_ENDS_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "Artwork Preview"), ui_options.OPTION_ARTPREV_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "Bosses"),          ui_options.OPTION_BOSSES_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "Logo"),            ui_options.OPTION_LOGOS_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "Versus"),          ui_options.OPTION_VERSUS_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "Game Over"),       ui_options.OPTION_GAMEOVER_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "HowTo"),           ui_options.OPTION_HOWTO_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "Scores"),          ui_options.OPTION_SCORES_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "Select"),          ui_options.OPTION_SELECT_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "Marquees"),        ui_options.OPTION_MARQUEES_PATH),
            new std.pair<string, string>(N_p("selmenu-artwork", "Covers"),          ui_options.OPTION_COVER_PATH),
        };


        static readonly string [] hover_msg =
        {
            N_("Add or remove favorite"),
            N_("Export displayed list to file"),
            N_("Audit media"),
            N_("Show DATs view"),
        };


        protected int m_available_items;
        protected int skip_main_items;
        protected object m_prev_selected;  //void    *m_prev_selected;
        int m_total_lines;
        protected int m_topline_datsview;   // right box top line
        protected int m_filter_highlight;
        protected string m_search = "";


        bool m_ui_error;
        string m_error_text;

        game_driver m_info_driver;
        ui_software_info m_info_software;
        int m_info_view;
        std.vector<string> m_items_list;
        string m_info_buffer;
        text_layout m_info_layout;  //std::optional<text_layout>  m_info_layout;


        cache m_cache;
        bool m_is_swlist;
        protected focused_menu m_focus;
        bool m_pressed;              // mouse button held down
        osd_ticks_t m_repeat;

        int m_right_visible_lines;  // right box lines

        bool m_has_icons;
        bool m_switch_image;
        bool m_default_image;
        uint8_t m_image_view;
        flags_cache m_flags;


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
            m_items_list = new std.vector<string>();
            m_info_buffer = "";
            m_info_layout = null;
            m_cache = mui.get_session_data(typeof(menu_select_launch), new cache_wrapper(machine()));
            m_is_swlist = is_swlist;
            m_focus = focused_menu.MAIN;
            m_pressed = false;
            m_repeat = 0;
            m_right_visible_lines = 0;
            m_has_icons = false;
            m_switch_image = false;
            m_default_image = true;
            m_image_view = FIRST_VIEW;
            m_flags = new flags_cache(256);


            set_needs_prev_menu_item(false);
        }


        protected focused_menu get_focus() { return m_focus; }
        protected void set_focus(focused_menu focus) { m_focus = focus; }

        protected void next_image_view() { throw new emu_unimplemented(); }
        protected void previous_image_view() { throw new emu_unimplemented(); }


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

            var flags = new system_flags(new machine_static_info(ui().options(), mconfig));
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
            string [] tempbuf = new string[4];

            // determine the text for the header
            make_topbox_text(out tempbuf[0], out tempbuf[1], out tempbuf[2]);

            float y1 = origy1 - 3.0f * ui().box_tb_border() - ui().get_line_height();

            string [] tempbuf2 = new string[3] { tempbuf[0], tempbuf[1], tempbuf[2] };

            draw_text_box(
                    tempbuf2, //tempbuf, tempbuf + 3,
                    origx1, origx2, origy1 - top, y1,
                    text_layout.text_justify.CENTER, text_layout.word_wrapping.NEVER, true,
                    ui().colors().text_color(), ui().colors().background_color(), 1.0f);

            // draw toolbar
            draw_toolbar(origx1, y1, origx2, origy1 - ui().box_tb_border());

            // determine the text to render below
            ui_software_info swinfo;
            ui_system_info system;
            get_selection(out swinfo, out system);

            bool isstar = false;
            rgb_t color = ui().colors().background_color();
            if (swinfo != null && swinfo.startempty == 0)
            {
                isstar = mame_machine_manager.instance().favorite().is_favorite_system_software(swinfo);

                // first line is long name or system
                tempbuf[0] = make_software_description(swinfo, system);

                // next line is year, publisher
                tempbuf[1] = string_format(__("{0}, {1}"), swinfo.year, swinfo.publisher);  // "%1$s, %2$-.100s"

                // next line is parent/clone
                if (!swinfo.parentname.empty())
                    tempbuf[2] = string_format(__("Software is clone of: {0}"), !swinfo.parentlongname.empty() ? swinfo.parentlongname : swinfo.parentname);  // %1$-.100s
                else
                    tempbuf[2] = __("Software is parent");

                // next line is supported status
                if (swinfo.supported == software_support.UNSUPPORTED)
                {
                    tempbuf[3] = __("Supported: No");
                    color = UI_RED_COLOR;
                }
                else if (swinfo.supported == software_support.PARTIALLY_SUPPORTED)
                {
                    tempbuf[3] = __("Supported: Partial");
                    color = UI_YELLOW_COLOR;
                }
                else
                {
                    tempbuf[3] = __("Supported: Yes");
                    color = UI_GREEN_COLOR;
                }
            }
            else if (system != null || (swinfo != null && swinfo.driver != null))
            {
                game_driver driver = system != null ? system.driver : swinfo.driver;
                isstar = mame_machine_manager.instance().favorite().is_favorite_system(driver);

                // first line is year, manufacturer
                tempbuf[0] = string_format(__("{0}, {1}"), driver.year, driver.manufacturer);  //tempbuf[0] = string_format(_("%1$s, %2$-.100s"), driver.year, driver.manufacturer);

                // next line is clone/parent status
                int cloneof = driver_list.non_bios_clone(driver);

                if (0 > cloneof)
                    tempbuf[1] = __("Driver is parent");
                else if (system != null)
                    tempbuf[1] = string_format(__("Driver is clone of: {0}"), system.parent);  //tempbuf[1] = string_format(_("Driver is clone of: %1$-.100s"), system->parent);
                else
                    tempbuf[1] = string_format(__("Driver is clone of: {0}"), driver_list.driver((size_t)cloneof).type.fullname());  //tempbuf[1] = string_format(_("Driver is clone of: %1$-.100s"), driver_list::driver(cloneof).type.fullname());

                // next line is overall driver status
                system_flags flags = get_system_flags(driver);
                if ((flags.machine_flags() & machine_flags.type.NOT_WORKING) != 0)
                    tempbuf[2] = __("Overall: NOT WORKING");
                else if (((flags.unemulated_features() | flags.imperfect_features()) & device_t_feature_type.PROTECTION) != 0)
                    tempbuf[2] = __("Overall: Unemulated Protection");
                else
                    tempbuf[2] = __("Overall: Working");

                // next line is graphics, sound status
                if ((flags.unemulated_features() & device_t_feature_type.GRAPHICS) != 0)
                    tempbuf[3] = __("Graphics: Unimplemented, ");
                else if (((flags.unemulated_features() | flags.imperfect_features()) & (device_t_feature_type.GRAPHICS | device_t_feature_type.PALETTE)) != 0)
                    tempbuf[3] = __("Graphics: Imperfect, ");
                else
                    tempbuf[3] = __("Graphics: OK, ");

                if ((driver.flags & machine_flags.type.NO_SOUND_HW) != 0)
                    tempbuf[3] = tempbuf[3].append_(__("Sound: None"));
                else if ((flags.unemulated_features() & device_t_feature_type.SOUND) != 0)
                    tempbuf[3] = tempbuf[3].append_(__("Sound: Unimplemented"));
                else if ((flags.imperfect_features() & device_t_feature_type.SOUND) != 0)
                    tempbuf[3] = tempbuf[3].append_(__("Sound: Imperfect"));
                else
                    tempbuf[3] = tempbuf[3].append_(__("Sound: OK"));

                color = flags.status_color();
            }
            else
            {
                string copyright = emulator_info.get_copyright();
                size_t found = copyright.find('\n');

                tempbuf[0] = tempbuf[0].clear_();
                tempbuf[1] = string_format(__("{0} {1}"), emulator_info.get_appname(), build_version);  // %1$s %2$s
                tempbuf[2] = copyright.substr(0, found);
                tempbuf[3] = copyright.substr(found + 1);
            }

            // draw the footer
            draw_text_box(
                    tempbuf, //std::begin(tempbuf), std::end(tempbuf),
                    origx1, origx2, origy2 + ui().box_tb_border(), origy2 + bottom,
                    text_layout.text_justify.CENTER, text_layout.word_wrapping.NEVER, true,
                    ui().colors().text_color(), color, 1.0f);

            // is favorite? draw the star
            if (isstar)
                draw_star(origx1 + ui().box_lr_border() * machine().render().ui_aspect(container()), origy2 + (2.0f * ui().box_tb_border()));
        }


        // handlers

        void rotate_focus(int dir)
        {
            switch (get_focus())
            {
            case focused_menu.MAIN:
                if (selected_index() <= m_available_items)
                {
                    if (skip_main_items != 0 || (ui_globals.panels_status != HIDE_BOTH))
                    {
                        m_prev_selected = get_selection_ref();
                        if ((0 < dir) || (ui_globals.panels_status == HIDE_BOTH))
                            set_selected_index(m_available_items + 1);
                        else if (ui_globals.panels_status == HIDE_RIGHT_PANEL)
                            set_focus(focused_menu.LEFT);
                        else
                            set_focus(focused_menu.RIGHTBOTTOM);
                    }
                }
                else
                {
                    if ((0 > dir) || (ui_globals.panels_status == HIDE_BOTH))
                        select_prev();
                    else if (ui_globals.panels_status == HIDE_LEFT_PANEL)
                        set_focus(focused_menu.RIGHTTOP);
                    else
                        set_focus(focused_menu.LEFT);
                }
                break;

            case focused_menu.LEFT:
                if (0 > dir)
                {
                    set_focus(focused_menu.MAIN);
                    if (skip_main_items != 0)
                        set_selected_index(m_available_items + 1);
                    else
                        select_prev();
                }
                else if (ui_globals.panels_status != HIDE_RIGHT_PANEL)
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
                if (0 < dir)
                {
                    set_focus(focused_menu.RIGHTBOTTOM);
                }
                else if (ui_globals.panels_status != HIDE_LEFT_PANEL)
                {
                    set_focus(focused_menu.LEFT);
                }
                else
                {
                    set_focus(focused_menu.MAIN);
                    set_selected_index(m_available_items + 1);
                }
                break;

            case focused_menu.RIGHTBOTTOM:
                if (0 > dir)
                {
                    set_focus(focused_menu.RIGHTTOP);
                }
                else
                {
                    set_focus(focused_menu.MAIN);
                    select_prev();
                }
                break;
            }
        }


        protected abstract void inkey_export();


        protected void inkey_dats()
        {
            throw new emu_unimplemented();
        }


        // draw arrow

        //-------------------------------------------------
        //  draw common arrows
        //-------------------------------------------------
        void draw_common_arrow(float origx1, float origy1, float origx2, float origy2, int current, int dmin, int dmax, float title_size)
        {
            var line_height = ui().get_line_height();
            var lr_arrow_width = 0.4f * line_height * machine().render().ui_aspect(container());
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
            fgcolor_right = fgcolor_left = ui().colors().text_color();

            // set hover
            if (mouse_in_rect(ar_x0, ar_y0, ar_x1, ar_y1) && current != dmax)
            {
                ui().draw_textured_box(container(), ar_x0 + 0.01f, ar_y0, ar_x1 - 0.01f, ar_y1, ui().colors().mouseover_bg_color(), new rgb_t(43, 43, 43),
                        hilight_main_texture(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_TEXWRAP(1));
                set_hover(HOVER_UI_RIGHT);
                fgcolor_right = ui().colors().mouseover_color();
            }
            else if (mouse_in_rect(al_x0, al_y0, al_x1, al_y1) && current != dmin)
            {
                ui().draw_textured_box(container(), al_x0 + 0.01f, al_y0, al_x1 - 0.01f, al_y1, ui().colors().mouseover_bg_color(), new rgb_t(43, 43, 43),
                        hilight_main_texture(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_TEXWRAP(1));
                set_hover(HOVER_UI_LEFT);
                fgcolor_left = ui().colors().mouseover_color();
            }

            // apply arrow
            if (dmax == dmin)
                return;
            else if (current == dmin)
                draw_arrow(ar_x0, ar_y0, ar_x1, ar_y1, fgcolor_right, ROT90);
            else if (current == dmax)
                draw_arrow(al_x0, al_y0, al_x1, al_y1, fgcolor_left, ROT90 ^ ORIENTATION_FLIP_X);
            else
            {
                draw_arrow(ar_x0, ar_y0, ar_x1, ar_y1, fgcolor_right, ROT90);
                draw_arrow(al_x0, al_y0, al_x1, al_y1, fgcolor_left, ROT90 ^ ORIENTATION_FLIP_X);
            }
        }


        //-------------------------------------------------
        //  draw info arrow
        //-------------------------------------------------
        void draw_info_arrow(int ub, float origx1, float origx2, float oy1, float line_height, float text_size, float ud_arrow_width)
        {
            rgb_t fgcolor = ui().colors().text_color();
            uint32_t orientation = (ub == 0) ? (uint32_t)ROT0 : (uint32_t)(ROT0 ^ ORIENTATION_FLIP_Y);

            if (mouse_in_rect(origx1, oy1, origx2, oy1 + (line_height * text_size)))
            {
                ui().draw_textured_box(container(), origx1 + 0.01f, oy1, origx2 - 0.01f, oy1 + (line_height * text_size), ui().colors().mouseover_bg_color(),
                        new rgb_t(43, 43, 43), hilight_main_texture(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_TEXWRAP(1));
                set_hover((ub == 0) ? HOVER_DAT_UP : HOVER_DAT_DOWN);
                fgcolor = ui().colors().mouseover_color();
            }

            draw_arrow(0.5f * (origx1 + origx2) - 0.5f * (ud_arrow_width * text_size), oy1 + 0.25f * (line_height * text_size),
                    0.5f * (origx1 + origx2) + 0.5f * (ud_arrow_width * text_size), oy1 + 0.75f * (line_height * text_size), fgcolor, orientation);
        }


        protected bool draw_error_text()
        {
            if (m_ui_error)
                ui().draw_text_box(container(), m_error_text, text_layout.text_justify.CENTER, 0.5f, 0.5f, UI_RED_COLOR);

            return m_ui_error;
        }


        //template <typename Filter>
        protected float draw_left_panel(
                machine_filter.type current,
                std.map<machine_filter.type, machine_filter> filters,
                float x1, float y1, float x2, float y2)
        {
            if ((ui_globals.panels_status != SHOW_PANELS) && (ui_globals.panels_status != HIDE_RIGHT_PANEL))
                return draw_collapsed_left_panel(x1, y1, x2, y2);

            // calculate line height
            float line_height = ui().get_line_height();
            float text_size = ui().options().infos_size();
            float sc = y2 - y1 - (2.0f * ui().box_tb_border());
            float line_height_max = line_height * text_size;
            if (((float)machine_filter.type.COUNT * line_height_max) > sc)
            {
                float lm = sc / (float)machine_filter.type.COUNT;
                line_height_max = line_height * (lm / line_height);
            }

            // calculate horizontal offset for unadorned names
            //std::string tmp(convert_command_glyph("_# "));
            string tmp = "_# ";
            tmp = convert_command_glyph(ref tmp);
            float text_sign = ui().get_string_width(tmp, text_size);

            // get the maximum width of a filter name
            float left_width = 0.0f;
            for (machine_filter.type x = machine_filter.type.FIRST; machine_filter.type.COUNT > x; ++x)
                left_width = std.max(ui().get_string_width(machine_filter.display_name(x), text_size) + text_sign, left_width);

            // outline the box and inset by the border width
            float origy1 = y1;
            float origy2 = y2;
            float aspect = machine().render().ui_aspect(container());
            float lr_border = ui().box_lr_border() * aspect;
            x2 = x1 + left_width + 2.0f * lr_border;
            ui().draw_outlined_box(container(), x1, y1, x2, y2, ui().colors().background_color());
            x1 += lr_border;
            x2 -= lr_border;
            y1 += ui().box_tb_border();
            y2 -= ui().box_tb_border();

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
                        str = convert_command_glyph(ref str);
                    }

                    str += machine_filter.display_name(filter);
                }

                // handle mouse hover in passing
                rgb_t bgcolor = ui().colors().text_bg_color();
                rgb_t fgcolor = ui().colors().text_color();
                if (mouse_in_rect(x1, y1, x2, y1 + line_height_max))
                {
                    bgcolor = ui().colors().mouseover_bg_color();
                    fgcolor = ui().colors().mouseover_color();
                    set_hover(HOVER_FILTER_FIRST + (int)filter);
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
                            hilight_main_texture(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_TEXWRAP(1));
                }

                // finally draw the text itself and move to the next line
                float x1t = x1 + ((str == machine_filter.display_name(filter)) ? text_sign : 0.0f);
                ui().draw_text_full(
                        container(), str,
                        x1t, y1, x2 - x1,
                        text_layout.text_justify.LEFT, text_layout.word_wrapping.NEVER,
                        mame_ui_manager.draw_mode.NORMAL, fgcolor, bgcolor,
                        out _, out _, text_size);
                y1 += line_height_max;
            }

            x1 = x2 + lr_border;
            x2 = x1 + 2.0f * lr_border;
            y1 = origy1;
            y2 = origy2;
            float space = x2 - x1;
            float lr_arrow_width = 0.4f * space * aspect;

            // set left-right arrows dimension
            float ar_x0 = 0.5f * (x2 + x1) - 0.5f * lr_arrow_width;
            float ar_y0 = 0.5f * (y2 + y1) + 0.1f * space;
            float ar_x1 = ar_x0 + lr_arrow_width;
            float ar_y1 = 0.5f * (y2 + y1) + 0.9f * space;

            ui().draw_outlined_box(container(), x1, y1, x2, y2, new rgb_t(0xef, 0x12, 0x47, 0x7b));

            rgb_t fgcolor2 = ui().colors().text_color();
            if (mouse_in_rect(x1, y1, x2, y2))
            {
                fgcolor2 = ui().colors().mouseover_color();
                set_hover(HOVER_LPANEL_ARROW);
            }

            draw_arrow(ar_x0, ar_y0, ar_x1, ar_y1, fgcolor2, ROT90 ^ ORIENTATION_FLIP_X);
            return x2 + lr_border;
        }


        // icon helpers

        protected void check_for_icons(string listname)
        {
            // only ever set the flag, never clear it
            if (m_has_icons)
                return;

            // iterate over configured icon paths
            path_iterator paths = new path_iterator(ui().options().icons_directory());
            string current;
            while (paths.next(out current))
            {
                // if we're doing a software list, append it to the configured path
                if (listname != null)
                    util.path_append(ref current, listname);

                osd_printf_verbose("Checking for icons in directory {0}\n", current);

                // open and walk the directory
                osd.directory dir = m_osddirectory.open(current);
                if (dir != null)
                {
                    // this could be improved in many ways - it's just a rough go/no-go
                    osd.directory.entry entry;
                    while ((entry = dir.read()) != null)
                    {
                        current = entry.name;
                        size_t found = current.rfind(".ico");
                        if ((npos != found) && ((current.length() - 4) == found))
                        {
                            osd_printf_verbose("Entry {0} is a candidate icon file\n", entry.name);
                            m_has_icons = true;
                            return;
                        }
                        else if (("icons" == current) || (current.find("icons.") == 0U))
                        {
                            osd_printf_verbose("Entry {0} is a candidate icon collection\n", entry.name);
                            m_has_icons = true;
                            return;
                        }
                    }
                }
            }

            // nothing promising
            osd_printf_verbose(
                    "No candidate icons found for {0}{1}\n",
                    listname != null ? "software list " : "",
                    listname != null ? listname : "machines");
        }


        //std::string make_icon_paths(char const *listname) const;
        //bool scale_icon(bitmap_argb32 &&src, texture_and_bitmap &dst) const;

        // forcing refresh
        protected void set_switch_image() { m_switch_image = true; }


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
                else if (m_prev_selected == null)
                    return null;
                else if (m_prev_selected is ui_system_info)
                    return ((ui_system_info)m_prev_selected).driver;  //m_prev_selected;
                else
                    throw new emu_unimplemented();  // put a check for whatever type is the ref object
            }
            else if (selected_ref is game_driver)
            {
                return (game_driver)selected_ref;
            }
            else if (selected_ref is ui_system_info)
            {
                return (ui_system_info)selected_ref;
            }
            else
            {
                throw new emu_unimplemented();  // put a check for whatever type is the ref object
            }
        }


        protected static string make_system_audit_fail_text(media_auditor auditor, media_auditor.summary summary)
        {
            string str = "";  //std::ostringstream str;
            if (!auditor.records().empty())
            {
                str += "System media audit failed:\n";
                auditor.summarize(null, ref str);
                osd_printf_info(str);
                str = "";
            }

            str += __("Required ROM/disk images for the selected system are missing or incorrect. Please select a different system.\n\n");
            make_audit_fail_text(ref str, auditor, summary);
            return str;
        }


        protected static string make_software_audit_fail_text(media_auditor auditor, media_auditor.summary summary)
        {
            string str = "";  //std::ostringstream str;
            if (!auditor.records().empty())
            {
                str += "System media audit failed:\n";
                auditor.summarize(null, ref str);
                osd_printf_info(str);
                str = "";
            }
            str += __("Required ROM/disk images for the selected software are missing or incorrect. Please select a different software item.\n\n");
            make_audit_fail_text(ref str, auditor, summary);
            return str;
        }


        protected static bool audit_passed(media_auditor.summary summary)
        {
            return (media_auditor.summary.CORRECT == summary) || (media_auditor.summary.BEST_AVAILABLE == summary) || (media_auditor.summary.NONE_NEEDED == summary);
        }


        void reset_pressed() { m_pressed = false; m_repeat = 0; }
        bool mouse_pressed() { return (m_osdcore.osd_ticks() >= m_repeat); }


        void set_pressed()
        {
            if (m_repeat == 0)
                m_repeat = m_osdcore.osd_ticks() + m_osdcore.osd_ticks_per_second() / 2;
            else
                m_repeat = m_osdcore.osd_ticks() + m_osdcore.osd_ticks_per_second() / 4;
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
            float aspect = machine().render().ui_aspect(container());
            float space = x2 - x1;
            float lr_arrow_width = 0.4f * space * aspect;

            // set left-right arrows dimension
            float ar_x0 = 0.5f * (x2 + x1) - (0.5f * lr_arrow_width);
            float ar_y0 = 0.5f * (y2 + y1) + (0.1f * space);
            float ar_x1 = ar_x0 + lr_arrow_width;
            float ar_y1 = 0.5f * (y2 + y1) + (0.9f * space);

            ui().draw_outlined_box(container(), x1, y1, x2, y2, new rgb_t(0xef, 0x12, 0x47, 0x7b)); // FIXME: magic numbers in colour?

            rgb_t fgcolor = ui().colors().text_color();
            if (mouse_in_rect(x1, y1, x2, y2))
            {
                fgcolor = ui().options().mouseover_color();
                set_hover(HOVER_LPANEL_ARROW);
            }

            draw_arrow(ar_x0, ar_y0, ar_x1, ar_y1, fgcolor, ROT90);

            return x2 + ui().box_lr_border() * aspect;
        }


        // draw infos
        //-------------------------------------------------
        //  draw infos
        //-------------------------------------------------
        void infos_render(float origx1, float origy1, float origx2, float origy2)
        {
            float line_height = ui().get_line_height();
            float text_size = ui().options().infos_size();
            string first;
            ui_software_info software;
            ui_system_info system;
            int total;
            get_selection(out software, out system);

            if (software != null && software.startempty == 0)
            {
                m_info_driver = null;
                first = __("Software List Info");

                if (m_info_software != software || m_info_view != ui_globals.cur_sw_dats_view)
                {
                    m_info_buffer = "";
                    m_info_layout = null;
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
                        ui_globals.cur_sw_dats_total = (uint8_t)(m_items_list.size() + 1);
                    }

                    if (m_info_view == 0)
                    {
                        m_info_buffer = software.infotext;
                    }
                    else
                    {
                        m_info_buffer = "";
                        mame_machine_manager.instance().lua().call_plugin("data", m_info_view - 1, out m_info_buffer);
                    }
                }
                total = ui_globals.cur_sw_dats_total;
            }
            else if (system != null || (software != null && software.driver != null))
            {
                game_driver driver = system != null ? system.driver : software.driver;
                m_info_software = null;
                first = __("General Info");

                if (driver != m_info_driver || ui_globals.curdats_view != m_info_view)
                {
                    m_info_buffer = "";
                    m_info_layout = null;
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

                        //throw new emu_unimplemented();
#if false
                        mame_machine_manager.instance().lua().call_plugin("data_list", driver.name, out m_items_list);
#endif
                        ui_globals.curdats_total = (uint8_t)(m_items_list.size() + 1);
                    }

                    if (m_info_view == 0)
                    {
                        general_info(system, driver, out m_info_buffer);
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

            origy1 += ui().box_tb_border();
            float aspect = machine().render().ui_aspect(container());
            float gutter_width = 0.4f * line_height * aspect * 1.3f;
            float ud_arrow_width = line_height * aspect;
            float oy1 = origy1 + line_height;

            string snaptext = m_info_view != 0 ? m_items_list[m_info_view - 1] : first;

            // get width of widest title
            float title_size = 0.0f;
            for (size_t x = 0; (size_t)total > x; ++x)
            {
                string name = x != 0 ? m_items_list[x - 1] : first;
                float txt_length = 0.0f;
                ui().draw_text_full(
                        container(), name,
                        origx1, origy1, origx2 - origx1,
                        text_layout.text_justify.CENTER, text_layout.word_wrapping.NEVER,
                        mame_ui_manager.draw_mode.NONE, ui().colors().text_color(), ui().colors().text_bg_color(),
                        out txt_length, out _);
                txt_length += 0.01f;
                title_size = std.max(txt_length, title_size);
            }

            rgb_t fgcolor = ui().colors().text_color();
            rgb_t bgcolor = ui().colors().text_bg_color();
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

            if (bgcolor != ui().colors().text_bg_color())
            {
                ui().draw_textured_box(
                        container(),
                        origx1 + ((middle - title_size) * 0.5f), origy1, origx1 + ((middle + title_size) * 0.5f),
                        origy1 + line_height, bgcolor, new rgb_t(255, 43, 43, 43),
                        hilight_main_texture(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_TEXWRAP(1));
            }

            ui().draw_text_full(
                    container(),
                    snaptext,
                    origx1, origy1, origx2 - origx1,
                    text_layout.text_justify.CENTER, text_layout.word_wrapping.NEVER,
                    mame_ui_manager.draw_mode.NORMAL, fgcolor, bgcolor, out _, out _, tmp_size);

            sc = origx2 - origx1 - (2.0f * gutter_width);
            if (m_info_layout == null || (m_info_layout.width() != sc))
            {
                m_info_layout =  //m_info_layout.emplace(
                        ui().create_layout(
                            container(),
                            sc,
                            text_layout.text_justify.LEFT, text_layout.word_wrapping.WORD);
                menu_dats_view.add_info_text(m_info_layout, m_info_buffer, ui().colors().text_color(), text_size);
                m_total_lines = (int)m_info_layout.lines();
            }

            draw_common_arrow(origx1, origy1, origx2, origy2, m_info_view, 0, total - 1, title_size);
            m_right_visible_lines = (int)floor((origy2 - oy1) / (line_height * text_size));
            if (m_total_lines < m_right_visible_lines)
                m_right_visible_lines = m_total_lines;
            if (m_topline_datsview < 0)
                m_topline_datsview = 0;
            if ((m_topline_datsview + m_right_visible_lines) >= m_total_lines)
                m_topline_datsview = m_total_lines - m_right_visible_lines;

            // get the number of visible lines, minus 1 for top arrow and 1 for bottom arrow
            bool up_arrow = m_topline_datsview > 0;
            bool down_arrow = (m_topline_datsview + m_right_visible_lines) < m_total_lines;
            int r_visible_lines = m_right_visible_lines - (up_arrow ? 1 : 0) - (down_arrow ? 1 : 0);

            if (mouse_in_rect(origx1 + gutter_width, oy1, origx2 - gutter_width, origy2))
                set_hover(HOVER_INFO_TEXT);

            if (up_arrow)
                draw_info_arrow(0, origx1, origx2, oy1, line_height, text_size, ud_arrow_width);
            if (down_arrow)
                draw_info_arrow(1, origx1, origx2, oy1 + ((float)(m_right_visible_lines - 1) * line_height * text_size), line_height, text_size, ud_arrow_width);

            m_info_layout.emit(
                    container(),
                    m_topline_datsview != 0 ? ((size_t)m_topline_datsview + 1) : 0, (size_t)r_visible_lines,
                    origx1 + gutter_width, oy1 + (m_topline_datsview != 0 ? (line_height * text_size) : 0.0f));
        }


        //-------------------------------------------------
        //  generate general info
        //-------------------------------------------------
        void general_info(ui_system_info system, game_driver driver, out string buffer)
        {
            system_flags flags = get_system_flags(driver);
            string str = "";  //std::ostringstream str;

            str += "#j2\n";

            if (system != null)
                str += system.description;
            else
                str += driver.type.fullname();
            str += "\t\n\n";

            util.stream_format(ref str, __("Romset\t{0}\n"), driver.name);  //util::stream_format(str, _("Romset\t%1$s\n"), driver.name);
            util.stream_format(ref str, __("Year\t{0}\n"), driver.year);  //util::stream_format(str, _("Year\t%1$s\n"), driver.year);
            util.stream_format(ref str, __("Manufacturer\t{0}\n"), driver.manufacturer);  //util::stream_format(str, _("Manufacturer\t%1$s\n"), driver.manufacturer);

            int cloneof = driver_list.non_bios_clone(driver);
            if (0 <= cloneof)
            {
                util.stream_format(
                        ref str,
                        __("Driver is Clone of\t{0}\n"),  //_("Driver is Clone of\t%1$s\n"),
                        system != null ? system.parent : driver_list.driver((size_t)cloneof).type.fullname());
            }
            else
            {
                str += __("Driver is Parent\t\n");
            }

            if (flags.has_analog())
                str += __("Analog Controls\tYes\n");
            if (flags.has_keyboard())
                str += __("Keyboard Inputs\tYes\n");

            if ((flags.machine_flags() & machine_flags.type.NOT_WORKING) != 0)
                str += __("Overall\tNOT WORKING\n");
            else if (((flags.unemulated_features() | flags.imperfect_features()) & device_t_feature_type.PROTECTION) != 0)
                str += __("Overall\tUnemulated Protection\n");
            else
                str += __("Overall\tWorking\n");

            if ((flags.unemulated_features() & device_t_feature_type.GRAPHICS) != 0)
                str += __("Graphics\tUnimplemented\n");
            else if ((flags.unemulated_features() & device_t_feature_type.PALETTE) != 0)
                str += __("Graphics\tWrong Colors\n");
            else if ((flags.imperfect_features() & device_t_feature_type.PALETTE) != 0)
                str += __("Graphics\tImperfect Colors\n");
            else if ((flags.imperfect_features() & device_t_feature_type.GRAPHICS) != 0)
                str += __("Graphics\tImperfect\n");
            else
                str += __("Graphics\tOK\n");

            if ((flags.machine_flags() & machine_flags.type.NO_SOUND_HW) != 0)
                str += __("Sound\tNone\n");
            else if ((flags.unemulated_features() & device_t_feature_type.SOUND) != 0)
                str += __("Sound\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.SOUND) != 0)
                str += __("Sound\tImperfect\n");
            else
                str += __("Sound\tOK\n");

            if ((flags.unemulated_features() & device_t_feature_type.CAPTURE) != 0)
                str += __("Capture\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.CAPTURE) != 0)
                str += __("Capture\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.CAMERA) != 0)
                str += __("Camera\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.CAMERA) != 0)
                str += __("Camera\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.MICROPHONE) != 0)
                str += __("Microphone\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.MICROPHONE) != 0)
                str += __("Microphone\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.CONTROLS) != 0)
                str += __("Controls\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.CONTROLS) != 0)
                str += __("Controls\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.KEYBOARD) != 0)
                str += __("Keyboard\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.KEYBOARD) != 0)
                str += __("Keyboard\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.MOUSE) != 0)
                str += __("Mouse\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.MOUSE) != 0)
                str += __("Mouse\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.MEDIA) != 0)
                str += __("Media\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.MEDIA) != 0)
                str += __("Media\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.DISK) != 0)
                str += __("Disk\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.DISK) != 0)
                str += __("Disk\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.PRINTER) != 0)
                str += __("Printer\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.PRINTER) != 0)
                str += __("Printer\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.TAPE) != 0)
                str += __("Mag. Tape\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.TAPE) != 0)
                str += __("Mag. Tape\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.PUNCH) != 0)
                str += __("Punch Tape\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.PUNCH) != 0)
                str += __("Punch Tape\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.DRUM) != 0)
                str += __("Mag. Drum\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.DRUM) != 0)
                str += __("Mag. Drum\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.ROM) != 0)
                str += __("(EP)ROM\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.ROM) != 0)
                str += __("(EP)ROM\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.COMMS) != 0)
                str += __("Communications\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.COMMS) != 0)
                str += __("Communications\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.LAN) != 0)
                str += __("LAN\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.LAN) != 0)
                str += __("LAN\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.WAN) != 0)
                str += __("WAN\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.WAN) != 0)
                str += __("WAN\tImperfect\n");

            if ((flags.unemulated_features() & device_t_feature_type.TIMING) != 0)
                str += __("Timing\tUnimplemented\n");
            else if ((flags.imperfect_features() & device_t_feature_type.TIMING) != 0)
                str += __("Timing\tImperfect\n");

            str += ((flags.machine_flags() & machine_flags.type.MECHANICAL) != 0)        ? __("Mechanical Machine\tYes\n")         : __("Mechanical Machine\tNo\n");
            str += ((flags.machine_flags() & machine_flags.type.REQUIRES_ARTWORK) != 0)  ? __("Requires Artwork\tYes\n")           : __("Requires Artwork\tNo\n");
            str += ((flags.machine_flags() & machine_flags.type.CLICKABLE_ARTWORK) != 0) ? __("Requires Clickable Artwork\tYes\n") : __("Requires Clickable Artwork\tNo\n");
            if ((flags.machine_flags() & machine_flags.type.NO_COCKTAIL) != 0)
                str += __("Support Cocktail\tNo\n");
            str += ((flags.machine_flags() & machine_flags.type.IS_BIOS_ROOT) != 0)      ? __("Driver is BIOS\tYes\n")             : __("Driver is BIOS\tNo\n");
            str += ((flags.machine_flags() & machine_flags.type.SUPPORTS_SAVE) != 0)     ? __("Support Save\tYes\n")               : __("Support Save\tNo\n");
            str += (((int)flags.machine_flags() & ORIENTATION_SWAP_XY) != 0)             ? __("Screen Orientation\tVertical\n")    : __("Screen Orientation\tHorizontal\n");

            bool found = false;
            foreach (var region in new romload.entries(driver.rom).get_regions())  //for (romload::region const &region : romload::entries(driver.rom).get_regions())
            {
                if (romload.region.is_diskdata(region))
                {
                    found = true;
                    break;
                }
            }

            str += (found ? __("Requires CHD\tYes\n") : __("Requires CHD\tNo\n"));

            // audit the game first to see if we're going to work
            if (ui().options().info_audit())
            {
                driver_enumerator enumerator = new driver_enumerator(machine().options(), driver);
                enumerator.next();
                media_auditor auditor = new media_auditor(enumerator);
                media_auditor.summary summary = auditor.audit_media(media_auditor.AUDIT_VALIDATE_FAST);
                media_auditor.summary summary_samples = auditor.audit_samples();

                // if everything looks good, schedule the new driver
                if (audit_passed(summary))
                    str += __("ROM Audit Result\tOK\n");
                else
                    str += __("ROM Audit Result\tBAD\n");

                if (summary_samples == media_auditor.summary.NONE_NEEDED)
                    str += __("Samples Audit Result\tNone Needed\n");
                else if (audit_passed(summary_samples))
                    str += __("Samples Audit Result\tOK\n");
                else
                    str += __("Samples Audit Result\tBAD\n");
            }
            else
            {
                str += __("ROM Audit \tDisabled\nSamples Audit \tDisabled\n");
            }

            buffer = str;
        }


        // get selected software and/or driver
        protected abstract void get_selection(out ui_software_info software, out ui_system_info system);


        protected virtual bool accept_search() { return true; }


        void select_prev()
        {
            if (m_prev_selected == null)
            {
                set_selected_index(0);
            }
            else
            {
                for (int x = 0; x < item_count(); ++x)
                {
                    if (item(x).ref_ == m_prev_selected)
                    {
                        set_selected_index(x);
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
            ui().draw_outlined_box(container(), x1, y1, x2, y2, new rgb_t(0xef, 0x12, 0x47, 0x7b));

            // take off the borders
            float aspect = machine().render().ui_aspect(container());
            float lr_border = ui().box_lr_border() * aspect;
            x1 += lr_border;
            x2 -= lr_border;
            y1 += ui().box_tb_border();
            y2 -= ui().box_tb_border();

            // work out which buttons we're going to draw
            MemoryContainer<unsigned> SYS_TOOLBAR_BITMAPS = new MemoryContainer<unsigned>() { TOOLBAR_BITMAP_FAVORITE, TOOLBAR_BITMAP_SAVE, TOOLBAR_BITMAP_AUDIT, TOOLBAR_BITMAP_INFO };
            MemoryContainer<unsigned> SW_TOOLBAR_BITMAPS = new MemoryContainer<unsigned>() { TOOLBAR_BITMAP_FAVORITE, TOOLBAR_BITMAP_INFO };
            bool have_parent = m_is_swlist || !stack_has_special_main_menu();
            Pointer<unsigned> toolbar_bitmaps = m_is_swlist ? new Pointer<unsigned>(SW_TOOLBAR_BITMAPS) : new Pointer<unsigned>(SYS_TOOLBAR_BITMAPS);
            unsigned toolbar_count = m_is_swlist ? (unsigned)std.size(SW_TOOLBAR_BITMAPS) : (unsigned)std.size(SYS_TOOLBAR_BITMAPS);

            // calculate metrics
            float x_size = (y2 - y1) * aspect;
            float x_spacing = x_size * 1.5f;
            float backtrack_pos = x2 - x_size;
            float total_width = ((float)toolbar_count + ((float)(toolbar_count - 1) * 0.5f)) * x_size;
            m_cache.cache_toolbar(machine(), x_size, y2 - y1);

            // add backtrack button
            rgb_t color = new rgb_t(0xffcccccc);
            if (mouse_in_rect(backtrack_pos, y1, x2, y2))
            {
                set_hover(HOVER_BACKTRACK);
                color = rgb_t.white();
                float ypos = y2 + ui().get_line_height() + 2.0f * ui().box_tb_border();
                ui().draw_text_box(
                        container(),
                        have_parent ? __("Return to previous menu") : __("Exit"),
                        text_layout.text_justify.RIGHT, 1.0f - lr_border, ypos,
                        ui().colors().background_color());
            }
            container().add_quad(
                    backtrack_pos, y1, x2, y2,
                    color,
                    m_cache.toolbar_textures()[have_parent ? TOOLBAR_BITMAP_PREVMENU : TOOLBAR_BITMAP_EXIT],
                    PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));

            // now add the other buttons
            x1 = std.min(backtrack_pos - ((float)toolbar_count * x_spacing), x1 + ((x2 - x1 - total_width) * 0.5f));
            for (int z = 0; toolbar_count > z; ++z, x1 += x_spacing)
            {
                var bitmap = toolbar_bitmaps[z];
                x2 = x1 + x_size;
                color = new rgb_t(0xffcccccc);
                if (mouse_in_rect(x1, y1, x2, y2))
                {
                    bool need_selection = (TOOLBAR_BITMAP_FAVORITE == bitmap) || (TOOLBAR_BITMAP_INFO == bitmap);
                    if (!need_selection || get_selection_ptr() != null)
                    {
                        set_hover(HOVER_B_FAV + (int)bitmap);
                        color = rgb_t.white();
                    }

                    float ypos = y2 + ui().get_line_height() + 2.0f * ui().box_tb_border();
                    ui().draw_text_box(
                            container(),
                            __(hover_msg[bitmap]),
                            text_layout.text_justify.CENTER, (x1 + x2) * 0.5f, ypos,
                            ui().colors().background_color());
                }

                container().add_quad(
                        x1, y1, x2, y2,
                        color,
                        m_cache.toolbar_textures()[bitmap],
                        PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
            }
        }


        //-------------------------------------------------
        //  draw favorites star
        //-------------------------------------------------
        void draw_star(float x0, float y0)
        {
            if (TOOLBAR_BITMAP_FAVORITE < m_cache.toolbar_textures().size())
            {
                float y1 = y0 + ui().get_line_height();
                float x1 = x0 + ui().get_line_height() * container().manager().ui_aspect(container());
                container().add_quad(
                        x0, y0, x1, y1,
                        rgb_t.white(),
                        m_cache.toolbar_textures()[TOOLBAR_BITMAP_FAVORITE],
                        PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_PACKABLE);
            }
        }


        //-------------------------------------------------
        //  draw icons
        //-------------------------------------------------
        void draw_icon(int linenum, object selectedref, float x0, float y0)
        {
            render_texture icon = get_icon_texture(linenum, selectedref);
            if (icon != null)
            {
                float ud_arrow_width = ui().get_line_height() * container().manager().ui_aspect(container());
                float x1 = x0 + ud_arrow_width;
                float y1 = y0 + ui().get_line_height();
                container().add_quad(x0, y0, x1, y1, rgb_t.white(), icon, PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
            }
        }


        protected abstract render_texture get_icon_texture(int linenum, object selectedref);


        //-------------------------------------------------
        //  get title and search path for right panel
        //-------------------------------------------------
        void get_title_search(out string snaptext, out string searchstr)
        {
            // get arts title text
            snaptext = __("selmenu-artwork", arts_info[m_image_view].first);

            // get search path
            string addpath;
            if (m_image_view == SNAPSHOT_VIEW)
            {
                emu_options moptions = new emu_options();
                searchstr = machine().options().value(arts_info[m_image_view].second);
                addpath = moptions.value(arts_info[m_image_view].second);
            }
            else
            {
                ui_options moptions = new ui_options();
                searchstr = ui().options().value(arts_info[m_image_view].second);
                addpath = moptions.value(arts_info[m_image_view].second);
            }

            string tmp = searchstr;
            path_iterator path = new path_iterator(tmp);
            path_iterator path_iter = new path_iterator(addpath);
            string c_path;
            string curpath;

            // iterate over path and add path for zipped formats
            while (path.next(out curpath))
            {
                path_iter.reset();
                while (path_iter.next(out c_path))
                    searchstr += ";" + curpath + PATH_SEPARATOR + c_path;
            }
        }


        // event handling
        //-------------------------------------------------
        //  handle keys for main menu
        //-------------------------------------------------
        protected override void handle_keys(uint32_t flags, ref int iptkey)
        {
            bool ignorepause = stack_has_special_main_menu();

            // bail if no items
            if (item_count() == 0)
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

                return;
            }

            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_CANCEL, 0))
            {
                if (m_ui_error)
                {
                    // dismiss error
                }
                else if (!m_search.empty())
                {
                    // escape pressed with non-empty search text clears it
                    m_search = "";
                    reset(reset_options.REMEMBER_REF);
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
            bool ignoreleft = ((selected_item().flags & FLAG_LEFT_ARROW) == 0);
            bool ignoreright = ((selected_item().flags & FLAG_RIGHT_ARROW) == 0);
            bool leftclose = (ui_globals.panels_status == HIDE_BOTH || ui_globals.panels_status == HIDE_LEFT_PANEL);
            bool rightclose = (ui_globals.panels_status == HIDE_BOTH || ui_globals.panels_status == HIDE_RIGHT_PANEL);

            // accept left/right keys as-is with repeat
            if (!ignoreleft && exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_LEFT, (flags & PROCESS_LR_REPEAT) != 0 ? 6 : 0))
            {
                // Swap the right panel
                if (m_focus == focused_menu.RIGHTTOP)
                    ui_globals.rpanel = RP_IMAGES;
                return;
            }

            if (!ignoreright && exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_RIGHT, (flags & PROCESS_LR_REPEAT) != 0 ? 6 : 0))
            {
                // Swap the right panel
                if (m_focus == focused_menu.RIGHTTOP)
                    ui_globals.rpanel = RP_INFOS;
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
                else if (selected_index() == m_available_items + 1 || is_first_selected() || m_ui_error)
                {
                    return;
                }

                set_selected_index(selected_index() - 1);

                if (selected_index() == top_line && top_line != 0)
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
                else if (is_last_selected() || selected_index() == m_available_items - 1 || m_ui_error)
                {
                    return;
                }

                set_selected_index(selected_index() + 1);

                if (selected_index() == top_line + m_visible_items + ((top_line != 0) ? 1 : 0))
                    top_line++;
            }

            // page up backs up by m_visible_items
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_PAGE_UP, 6))
            {
                // Infos
                if (!rightclose && m_focus == focused_menu.RIGHTBOTTOM)
                {
                    m_topline_datsview -= m_right_visible_lines - 3;
                    return;
                }

                if (selected_index() < m_available_items && !m_ui_error)
                {
                    set_selected_index(std.max(selected_index() - m_visible_items, 0));

                    top_line -= m_visible_items - (top_line + (m_visible_lines == m_available_items ? 1 : 0));
                }
            }

            // page down advances by m_visible_items
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_PAGE_DOWN, 6))
            {
                // Infos
                if (!rightclose && m_focus == focused_menu.RIGHTBOTTOM)
                {
                    m_topline_datsview += m_right_visible_lines - 3;
                    return;
                }

                if (selected_index() < m_available_items && !m_ui_error)
                {
                    set_selected_index(std.min(selected_index() + m_visible_lines - 2 + (selected_index() == 0 ? 1 : 0), m_available_items - 1));

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

                if (selected_index() < m_available_items && !m_ui_error)
                    select_first_item();
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

                if (selected_index() < m_available_items && !m_ui_error)
                    set_selected_index(top_line = m_available_items - 1);
            }

            // focus next rotates throw targets forward
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_FOCUS_NEXT, 12))
            {
                if (!m_ui_error)
                    rotate_focus(1);
            }

            // focus next rotates throw targets forward
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_FOCUS_PREV, 12))
            {
                if (!m_ui_error)
                    rotate_focus(-1);
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
                    if (m_ui_error)
                        continue;

                    switch (code)
                    {
                    case (int)ioport_type.IPT_UI_FOCUS_NEXT:
                    case (int)ioport_type.IPT_UI_FOCUS_PREV:
                        continue;
                    case (int)ioport_type.IPT_UI_LEFT:
                        if (ignoreleft)
                            continue;
                        break;
                    case (int)ioport_type.IPT_UI_RIGHT:
                        if (ignoreright)
                            continue;
                        break;
                    case (int)ioport_type.IPT_UI_PAUSE:
                        if (ignorepause)
                            continue;
                        break;
                    }

                    if (exclusive_input_pressed(ref iptkey, code, 0))
                        break;
                }
            }
        }


        //-------------------------------------------------
        //  handle input events for main menu
        //-------------------------------------------------
        protected override void handle_events(uint32_t flags, menu_event ev)
        {
            if (m_pressed)
            {
                bool pressed = mouse_pressed();
                int32_t target_x;
                int32_t target_y;
                bool button;
                render_target mouse_target = machine().ui_input().find_mouse(out target_x, out target_y, out button);
                if (mouse_target != null && button && (hover() == HOVER_ARROW_DOWN || hover() == HOVER_ARROW_UP))
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
                            if (hover() >= 0 && hover() < item_count())
                            {
                                if (hover() >= m_available_items - 1 && selected_index() < m_available_items)
                                    m_prev_selected = get_selection_ref();
                                set_selected_index(hover());
                                m_focus = focused_menu.MAIN;
                            }
                            else if (hover() == HOVER_ARROW_UP)
                            {
                                set_selected_index(std.max(selected_index() - m_visible_items, 0));
                                top_line -= m_visible_items - (top_line + (m_visible_lines == m_available_items ? 1 : 0));
                                set_pressed();
                            }
                            else if (hover() == HOVER_ARROW_DOWN)
                            {
                                set_selected_index(std.min(selected_index() + m_visible_lines - 2 + (selected_index() == 0 ? 1 : 0), m_available_items - 1));
                                top_line += m_visible_lines - 2;
                                set_pressed();
                            }
                            else if (hover() == HOVER_UI_RIGHT)
                                ev.iptkey = (int)ioport_type.IPT_UI_RIGHT;
                            else if (hover() == HOVER_UI_LEFT)
                                ev.iptkey = (int)ioport_type.IPT_UI_LEFT;
                            else if (hover() == HOVER_DAT_DOWN)
                                m_topline_datsview += m_right_visible_lines - 3;
                            else if (hover() == HOVER_DAT_UP)
                                m_topline_datsview -= m_right_visible_lines - 3;
                            else if (hover() == HOVER_LPANEL_ARROW)
                            {
                                if (get_focus() == focused_menu.LEFT)
                                {
                                    set_focus(focused_menu.MAIN);
                                    select_prev();
                                }

                                if (ui_globals.panels_status == HIDE_LEFT_PANEL)
                                    ui_globals.panels_status = SHOW_PANELS;
                                else if (ui_globals.panels_status == HIDE_BOTH)
                                    ui_globals.panels_status = HIDE_RIGHT_PANEL;
                                else if (ui_globals.panels_status == SHOW_PANELS)
                                    ui_globals.panels_status = HIDE_LEFT_PANEL;
                                else if (ui_globals.panels_status == HIDE_RIGHT_PANEL)
                                    ui_globals.panels_status = HIDE_BOTH;
                            }
                            else if (hover() == HOVER_RPANEL_ARROW)
                            {
                                if ((get_focus() == focused_menu.RIGHTTOP) || (get_focus() == focused_menu.RIGHTBOTTOM))
                                {
                                    set_focus(focused_menu.MAIN);
                                    select_prev();
                                }

                                if (ui_globals.panels_status == HIDE_RIGHT_PANEL)
                                    ui_globals.panels_status = SHOW_PANELS;
                                else if (ui_globals.panels_status == HIDE_BOTH)
                                    ui_globals.panels_status = HIDE_LEFT_PANEL;
                                else if (ui_globals.panels_status == SHOW_PANELS)
                                    ui_globals.panels_status = HIDE_RIGHT_PANEL;
                                else if (ui_globals.panels_status == HIDE_LEFT_PANEL)
                                    ui_globals.panels_status = HIDE_BOTH;
                            }
                            else if (hover() == HOVER_RPANEL_ARROW)
                            {
                                if ((get_focus() == focused_menu.RIGHTTOP) || (get_focus() == focused_menu.RIGHTBOTTOM))
                                {
                                    set_focus(focused_menu.MAIN);
                                    select_prev();
                                }

                                if (ui_globals.panels_status == HIDE_RIGHT_PANEL)
                                    ui_globals.panels_status = SHOW_PANELS;
                                else if (ui_globals.panels_status == HIDE_BOTH)
                                    ui_globals.panels_status = HIDE_LEFT_PANEL;
                                else if (ui_globals.panels_status == SHOW_PANELS)
                                    ui_globals.panels_status = HIDE_RIGHT_PANEL;
                                else if (ui_globals.panels_status == HIDE_LEFT_PANEL)
                                    ui_globals.panels_status = HIDE_BOTH;
                            }
                            else if (hover() == HOVER_B_FAV)
                            {
                                ev.iptkey = (int)ioport_type.IPT_UI_FAVORITES;
                                stop = true;
                            }
                            else if (hover() == HOVER_B_EXPORT)
                            {
                                inkey_export();
                                stop = true;
                            }
                            else if (hover() == HOVER_B_AUDIT)
                            {
                                ev.iptkey = (int)ioport_type.IPT_UI_AUDIT;
                                stop = true;
                            }
                            else if (hover() == HOVER_B_DATS)
                            {
                                inkey_dats();
                                stop = true;
                            }
                            else if (hover() == HOVER_BACKTRACK)
                            {
                                ev.iptkey = (int)ioport_type.IPT_UI_CANCEL;
                                stack_pop();
                                stop = true;
                            }
                            else if (hover() >= HOVER_RP_FIRST && hover() <= HOVER_RP_LAST)
                            {
                                ui_globals.rpanel = (uint8_t)((HOVER_RP_FIRST - hover()) * (-1));
                                stop = true;
                            }
                            else if (hover() >= HOVER_FILTER_FIRST && hover() <= HOVER_FILTER_LAST)
                            {
                                m_prev_selected = null;
                                m_filter_highlight = hover() - HOVER_FILTER_FIRST;
                                filter_selected();
                                stop = true;
                            }
                        }
                        break;
                    }

                    // if we are hovering over a valid item, fake a UI_SELECT with a double-click
                    case ui_event.type.MOUSE_DOUBLE_CLICK:
                        if (hover() >= 0 && hover() < item_count())
                        {
                            set_selected_index(hover());
                            ev.iptkey = (int)ioport_type.IPT_UI_SELECT;
                        }

                        stop = true;
                        break;

                    // caught scroll event
                    case ui_event.type.MOUSE_WHEEL:
                        if (hover() >= 0 && hover() < item_count() - skip_main_items)
                        {
                            if (local_menu_event.zdelta > 0)
                            {
                                if (selected_index() >= m_available_items || is_first_selected() || m_ui_error)
                                    break;
                                set_selected_index(selected_index() - local_menu_event.num_lines);
                                if (selected_index() < top_line + ((top_line != 0) ? 1 : 0))
                                    top_line -= local_menu_event.num_lines;
                            }
                            else
                            {
                                if (selected_index() >= m_available_items - 1 || m_ui_error)
                                    break;
                                set_selected_index(std.min(selected_index() + local_menu_event.num_lines, m_available_items - 1));
                                if (selected_index() >= top_line + m_visible_items + ((top_line != 0) ? 1 : 0))
                                    top_line += local_menu_event.num_lines;
                            }
                        }
                        break;

                    // translate CHAR events into specials
                    case ui_event.type.IME_CHAR:
                        if (exclusive_input_pressed(ref ev.iptkey, (int)ioport_type.IPT_UI_FOCUS_NEXT, 0) || exclusive_input_pressed(ref ev.iptkey, (int)ioport_type.IPT_UI_FOCUS_PREV, 0))
                        {
                            stop = true;
                        }
                        else if (m_ui_error)
                        {
                            ev.iptkey = (int)ioport_type.IPT_SPECIAL;
                            stop = true;
                        }
                        else if (accept_search())
                        {
                            if (input_character(ref m_search, local_menu_event.ch, uchar_is_printable))
                                search_changed = true;
                        }
                        break;

                    case ui_event.type.MOUSE_RDOWN:
                        if (hover() >= 0 && hover() < item_count() - skip_main_items)
                        {
                            set_selected_index(hover());
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
                    case ui_event.type.WINDOW_FOCUS:
                    case ui_event.type.WINDOW_DEFOCUS:
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


        // draw game list
        //-------------------------------------------------
        //  draw main menu
        //-------------------------------------------------
        protected override void draw(uint32_t flags)
        {
            bool noinput = (flags & PROCESS_NOINPUT) != 0;
            float aspect = machine().render().ui_aspect(container());
            float lr_border = ui().box_lr_border() * aspect;
            float line_height = ui().get_line_height();
            float ud_arrow_width = line_height * aspect;
            float gutter_width = 0.52f * ud_arrow_width;
            float icon_offset = m_has_icons ? (1.5f * ud_arrow_width) : 0.0f;
            float right_panel_size = (ui_globals.panels_status == HIDE_BOTH || ui_globals.panels_status == HIDE_RIGHT_PANEL) ? 2.0f * lr_border : 0.3f;
            float visible_width = 1.0f - 4.0f * lr_border;
            float primary_left = (1.0f - visible_width) * 0.5f;
            float primary_width = visible_width;

            draw_background();

            clear_hover();
            m_available_items = item_count() - skip_main_items;
            float extra_height = skip_main_items * line_height;
            float visible_extra_menu_height = get_customtop() + get_custombottom() + extra_height;

            // locate mouse
            if (noinput)
                ignore_mouse();
            else
                map_mouse();

            // account for extra space at the top and bottom
            float visible_main_menu_height = 1.0f - 2.0f * ui().box_tb_border() - visible_extra_menu_height;
            m_visible_lines = (int)(std.trunc(visible_main_menu_height / line_height));
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
            float x1 = visible_left - lr_border;
            float y1 = visible_top - ui().box_tb_border();
            float x2 = x1 + 2.0f * lr_border;
            float y2 = visible_top + visible_main_menu_height + ui().box_tb_border() + extra_height;

            // add left box
            visible_left = draw_left_panel(x1, y1, x2, y2);
            visible_width -= right_panel_size + visible_left - 2.0f * lr_border;

            // compute and add main box
            x1 = visible_left - lr_border;
            x2 = visible_left + visible_width + lr_border;
            float line = visible_top + ((float)m_visible_lines * line_height);
            ui().draw_outlined_box(container(), x1, y1, x2, y2, ui().colors().background_color());

            // make sure the selection
            if (m_available_items < m_visible_lines)
                m_visible_lines = m_available_items;
            if (top_line < 0 || is_first_selected())
            {
                top_line = 0;
            }
            else if (selected_index() < m_available_items)
            {
                if (selected_index() >= (top_line + m_visible_lines))
                    top_line = selected_index() - (m_visible_lines / 2);
                if ((top_line + m_visible_lines) >= m_available_items)
                    top_line = m_available_items - m_visible_lines;
                else if (selected_index() >= (top_line + m_visible_lines - 2))
                    top_line = selected_index() - m_visible_lines + ((selected_index() == (m_available_items - 1)) ? 1: 2);
            }

            // determine effective positions taking into account the hilighting arrows
            float effective_width = visible_width - 2.0f * gutter_width;
            float effective_left = visible_left + gutter_width;

            if ((m_focus == focused_menu.MAIN) && (selected_index() < m_available_items))
                m_prev_selected = null;

            int n_loop = std.min(m_visible_lines, m_available_items);
            for (int linenum = 0; linenum < n_loop; linenum++)
            {
                float line_y = visible_top + ((float)linenum * line_height);
                int itemnum = top_line + linenum;
                menu_item pitem = item(itemnum);
                string itemtext = pitem.text;
                rgb_t fgcolor = ui().colors().text_color();
                rgb_t bgcolor = ui().colors().text_bg_color();
                rgb_t fgcolor3 = ui().colors().clone_color();
                float line_x0 = x1 + 0.5f * UI_LINE_WIDTH;
                float line_y0 = line_y;
                float line_x1 = x2 - 0.5f * UI_LINE_WIDTH;
                float line_y1 = line_y + line_height;

                // set the hover if this is our item
                if (mouse_in_rect(line_x0, line_y0, line_x1, line_y1) && is_selectable(pitem))
                    set_hover(itemnum);

                if (is_selected(itemnum) && m_focus == focused_menu.MAIN)
                {
                    // if we're selected, draw with a different background
                    fgcolor = new rgb_t(0xff, 0xff, 0x00);
                    bgcolor = new rgb_t(0xff, 0xff, 0xff);
                    fgcolor3 = new rgb_t(0xcc, 0xcc, 0x00);
                    ui().draw_textured_box(
                            container(),
                            line_x0 + 0.01f, line_y0, line_x1 - 0.01f, line_y1, 
                            bgcolor, new rgb_t(43, 43, 43),
                            hilight_main_texture(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_TEXWRAP(1));
                }
                else if (itemnum == hover())
                {
                    // else if the mouse is over this item, draw with a different background
                    fgcolor = fgcolor3 = ui().options().mouseover_color();
                    bgcolor = ui().colors().mouseover_bg_color();
                    highlight(line_x0, line_y0, line_x1, line_y1, bgcolor);
                }
                else if (pitem.ref_ == m_prev_selected)
                {
                    fgcolor = fgcolor3 = ui().options().mouseover_color();
                    bgcolor = ui().colors().mouseover_bg_color();
                    ui().draw_textured_box(container(), line_x0 + 0.01f, line_y0, line_x1 - 0.01f, line_y1, bgcolor, new rgb_t(43, 43, 43),
                            hilight_main_texture(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_TEXWRAP(1));
                }

                if (linenum == 0 && top_line != 0)
                {
                    // if we're on the top line, display the up arrow
                    draw_arrow(0.5f * (x1 + x2) - 0.5f * ud_arrow_width, line_y + 0.25f * line_height,
                        0.5f * (x1 + x2) + 0.5f * ud_arrow_width, line_y + 0.75f * line_height, fgcolor, ROT0);

                    if (hover() == itemnum)
                        set_hover(HOVER_ARROW_UP);
                }
                else if (linenum == m_visible_lines - 1 && itemnum != m_available_items - 1)
                {
                    // if we're on the bottom line, display the down arrow
                    draw_arrow(0.5f * (x1 + x2) - 0.5f * ud_arrow_width, line_y + 0.25f * line_height,
                        0.5f * (x1 + x2) + 0.5f * ud_arrow_width, line_y + 0.75f * line_height, fgcolor, ROT0 ^ ORIENTATION_FLIP_Y);

                    if (hover() == itemnum)
                        set_hover(HOVER_ARROW_DOWN);
                }
                else if (pitem.type == menu_item_type.SEPARATOR)
                {
                    // if we're just a divider, draw a line
                    container().add_line(visible_left, line_y + 0.5f * line_height, visible_left + visible_width, line_y + 0.5f * line_height,
                            UI_LINE_WIDTH, ui().colors().text_color(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
                }
                else if (pitem.subtext.empty())
                {
                    // draw the item centered
                    int item_invert = (int)(pitem.flags & FLAG_INVERT);
                    if (m_has_icons)
                        draw_icon(linenum, item(itemnum).ref_, effective_left, line_y);

                    ui().draw_text_full(
                            container(),
                            itemtext,
                            effective_left + icon_offset, line_y, effective_width - icon_offset,
                            text_layout.text_justify.LEFT, text_layout.word_wrapping.TRUNCATE,
                            mame_ui_manager.draw_mode.NORMAL, item_invert != 0 ? fgcolor3 : fgcolor, bgcolor,
                            out _, out _);
                }
                else
                {
                    int item_invert = (int)(pitem.flags & FLAG_INVERT);
                    string subitem_text = pitem.subtext;
                    float item_width;
                    float subitem_width;

                    // compute right space for subitem
                    ui().draw_text_full(
                            container(),
                            subitem_text,
                            effective_left + icon_offset, line_y, ui().get_string_width(pitem.subtext),
                            text_layout.text_justify.RIGHT, text_layout.word_wrapping.NEVER,
                            mame_ui_manager.draw_mode.NONE, item_invert != 0 ? fgcolor3 : fgcolor, bgcolor, 
                            out subitem_width, out _);
                    subitem_width += gutter_width;

                    // draw the item left-justified
                    if (m_has_icons)
                        draw_icon(linenum, item(itemnum).ref_, effective_left, line_y);

                    ui().draw_text_full(
                            container(),
                            itemtext,
                            effective_left + icon_offset, line_y, effective_width - icon_offset - subitem_width,
                            text_layout.text_justify.LEFT, text_layout.word_wrapping.TRUNCATE,
                            mame_ui_manager.draw_mode.NORMAL, item_invert != 0 ? fgcolor3 : fgcolor, bgcolor,
                            out item_width, out _);

                    // draw the subitem right-justified
                    ui().draw_text_full(
                            container(),
                            subitem_text,
                            effective_left + icon_offset + item_width, line_y, effective_width - icon_offset - item_width,
                            text_layout.text_justify.RIGHT, text_layout.word_wrapping.NEVER,
                            mame_ui_manager.draw_mode.NORMAL, item_invert != 0 ? fgcolor3 : fgcolor, bgcolor,
                            out _, out _);
                }
            }

            for (size_t count = (size_t)m_available_items; count < (size_t)item_count(); count++)
            {
                menu_item pitem = item((int)count);
                string itemtext = pitem.text;
                float line_x0 = x1 + 0.5f * UI_LINE_WIDTH;
                float line_y0 = line;
                float line_x1 = x2 - 0.5f * UI_LINE_WIDTH;
                float line_y1 = line + line_height;
                rgb_t fgcolor = ui().colors().text_color();
                rgb_t bgcolor = ui().colors().text_bg_color();

                if (mouse_in_rect(line_x0, line_y0, line_x1, line_y1) && is_selectable(pitem))
                    set_hover((int)count);

                // if we're selected, draw with a different background
                if (is_selected((int)count) && m_focus == focused_menu.MAIN)
                {
                    fgcolor = new rgb_t(0xff, 0xff, 0x00);
                    bgcolor = new rgb_t(0xff, 0xff, 0xff);
                    ui().draw_textured_box(container(), line_x0 + 0.01f, line_y0, line_x1 - 0.01f, line_y1, bgcolor, new rgb_t(43, 43, 43),
                            hilight_main_texture(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_TEXWRAP(1));
                }
                // else if the mouse is over this item, draw with a different background
                else if ((int)count == hover())
                {
                    fgcolor = ui().options().mouseover_color();
                    bgcolor = ui().colors().mouseover_bg_color();
                    highlight(line_x0, line_y0, line_x1, line_y1, bgcolor);
                }

                if (pitem.type == menu_item_type.SEPARATOR)
                {
                    container().add_line(
                            visible_left, line + 0.5f * line_height,
                            visible_left + visible_width, line + 0.5f * line_height,
                            UI_LINE_WIDTH,
                            ui().colors().text_color(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
                }
                else
                {
                    ui().draw_text_full(
                            container(),
                            itemtext,
                            effective_left, line, effective_width,
                            text_layout.text_justify.CENTER, text_layout.word_wrapping.TRUNCATE,
                            mame_ui_manager.draw_mode.NORMAL, fgcolor, bgcolor,
                            out _, out _);
                }
                line += line_height;
            }

            x1 = x2;
            x2 += right_panel_size;

            draw_right_panel(x1, y1, x2, y2);

            x1 = primary_left - lr_border;
            x2 = primary_left + primary_width + lr_border;

            // if there is something special to add, do it by calling the virtual method
            custom_render(get_selection_ref(), get_customtop(), get_custombottom(), x1, y1, x2, y2);

            // return the number of visible lines, minus 1 for top arrow and 1 for bottom arrow
            m_visible_items = m_visible_lines - (top_line != 0 ? 1 : 0) - ((top_line + m_visible_lines) != m_available_items ? 1 : 0);

            // noinput
            if (noinput)
            {
                int alpha = (int)((1.0f - machine().options().pause_brightness()) * 255.0f);
                if (alpha > 255)
                    alpha = 255;
                if (alpha >= 0)
                    container().add_rect(0.0f, 0.0f, 1.0f, 1.0f, new rgb_t((uint8_t)alpha, 0x00, 0x00, 0x00), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
            }
        }


        // draw right panel

        //-------------------------------------------------
        //  draw right panel
        //-------------------------------------------------
        void draw_right_panel(float origx1, float origy1, float origx2, float origy2)
        {
            float aspect = machine().render().ui_aspect(container());
            bool hide = (ui_globals.panels_status == HIDE_RIGHT_PANEL) || (ui_globals.panels_status == HIDE_BOTH);
            float x2 = hide ? origx2 : (origx1 + 2.0f * ui().box_lr_border() * aspect);
            float space = x2 - origx1;
            float lr_arrow_width = 0.4f * space * aspect;

            // set left-right arrows dimension
            float ar_x0 = 0.5f * (x2 + origx1) - 0.5f * lr_arrow_width;
            float ar_y0 = 0.5f * (origy2 + origy1) + 0.1f * space;
            float ar_x1 = ar_x0 + lr_arrow_width;
            float ar_y1 = 0.5f * (origy2 + origy1) + 0.9f * space;

            ui().draw_outlined_box(container(), origx1, origy1, origx2, origy2, new rgb_t(0xEF, 0x12, 0x47, 0x7B));

            rgb_t fgcolor = ui().colors().text_color();
            if (mouse_in_rect(origx1, origy1, x2, origy2))
            {
                fgcolor = ui().options().mouseover_color();
                set_hover(HOVER_RPANEL_ARROW);
            }

            if (hide)
            {
                draw_arrow(ar_x0, ar_y0, ar_x1, ar_y1, fgcolor, ROT90 ^ ORIENTATION_FLIP_X);
                return;
            }

            draw_arrow(ar_x0, ar_y0, ar_x1, ar_y1, fgcolor, ROT90);
            origy1 = draw_right_box_title(x2, origy1, origx2, origy2);

            if (ui_globals.rpanel == RP_IMAGES)
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
            ui().draw_outlined_box(container(), x1, y1, x2, y2, ui().colors().background_color());

            // add separator line
            container().add_line(x1 + midl, y1, x1 + midl, y1 + line_height, UI_LINE_WIDTH, ui().colors().border_color(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));

            string [] buffer = new string[RP_LAST + 1];
            buffer[RP_IMAGES] = "Images";
            buffer[RP_INFOS] = "Infos";

            // check size
            float text_size = 1.0f;
            foreach (var elem in buffer)
            {
                var textlen = ui().get_string_width(elem) + 0.01f;
                float tmp_size = (textlen > midl) ? (midl / textlen) : 1.0f;
                text_size = std.min(text_size, tmp_size);
            }

            for (int cells = RP_FIRST; cells <= RP_LAST; ++cells)
            {
                rgb_t bgcolor = ui().colors().text_bg_color();
                rgb_t fgcolor = ui().colors().text_color();

                if (mouse_in_rect(x1, y1, x1 + midl, y1 + line_height))
                {
                    if (ui_globals.rpanel != cells)
                    {
                        bgcolor = ui().colors().mouseover_bg_color();
                        fgcolor = ui().options().mouseover_color();
                        set_hover(HOVER_RP_FIRST + cells);
                    }
                }

                if (ui_globals.rpanel != cells)
                {
                    container().add_line(x1, y1 + line_height, x1 + midl, y1 + line_height, UI_LINE_WIDTH,
                            ui().colors().border_color(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
                    if (fgcolor != ui().colors().mouseover_color())
                        fgcolor = ui().colors().clone_color();
                }

                if (m_focus == focused_menu.RIGHTTOP && ui_globals.rpanel == cells)
                {
                    fgcolor = new rgb_t(0xff, 0xff, 0x00);
                    bgcolor = new rgb_t(0xff, 0xff, 0xff);
                    ui().draw_textured_box(
                            container(),
                            x1 + UI_LINE_WIDTH, y1 + UI_LINE_WIDTH, x1 + midl - UI_LINE_WIDTH, y1 + line_height,
                            bgcolor, new rgb_t(43, 43, 43), hilight_main_texture(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_TEXWRAP(1));
                }
                else if (bgcolor == ui().colors().mouseover_bg_color())
                {
                    container().add_rect(x1 + UI_LINE_WIDTH, y1 + UI_LINE_WIDTH, x1 + midl - UI_LINE_WIDTH, y1 + line_height,
                            bgcolor, PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_TEXWRAP(1));
                }

                ui().draw_text_full(
                        container(),
                        buffer[cells],
                        x1 + UI_LINE_WIDTH, y1, midl - UI_LINE_WIDTH,
                        text_layout.text_justify.CENTER, text_layout.word_wrapping.NEVER,
                        mame_ui_manager.draw_mode.NORMAL, fgcolor, bgcolor, out _, out _, text_size);

                x1 += midl;
            }

            return y1 + line_height + UI_LINE_WIDTH;
        }


        // images render

        //-------------------------------------------------
        //  perform our special rendering
        //-------------------------------------------------
        void arts_render(float origx1, float origy1, float origx2, float origy2)
        {
            ui_software_info software;
            ui_system_info system;
            get_selection(out software, out system);

            if (software != null && (software.startempty == 0 || system == null))
            {
                m_cache.set_snapx_driver(null);

                if (m_default_image)
                    m_image_view = (software.startempty == 0) ? SNAPSHOT_VIEW : CABINETS_VIEW;

                // arts title and searchpath
                string searchstr = arts_render_common(origx1, origy1, origx2, origy2);

                // loads the image if necessary
                if (!m_cache.snapx_software_is(software) || !snapx_valid() || m_switch_image)
                {
                    emu_file snapfile = new emu_file(searchstr, OPEN_FLAG_READ);
                    bitmap_argb32 tmp_bitmap;

                    if (software.startempty == 1)
                    {
                        // Load driver snapshot
                        load_driver_image(out tmp_bitmap, snapfile, software.driver);
                    }
                    else
                    {
                        // First attempt from name list
                        load_image(out tmp_bitmap, snapfile, util.path_concat(software.listname, software.shortname));

                        // Second attempt from driver name + part name
                        if (!tmp_bitmap.valid())
                            load_image(out tmp_bitmap, snapfile, util.path_concat(software.driver.name + software.part, software.shortname));
                    }

                    m_cache.set_snapx_software(software);
                    m_switch_image = false;
                    arts_render_images(tmp_bitmap, origx1, origy1, origx2, origy2);
                }

                // if the image is available, loaded and valid, display it
                draw_snapx(origx1, origy1, origx2, origy2);
            }
            else if (system != null)
            {
                m_cache.set_snapx_software(null);

                if (m_default_image)
                    m_image_view = ((system.driver.flags & machine_flags.type.MASK_TYPE) != machine_flags.type.TYPE_ARCADE) ? CABINETS_VIEW : SNAPSHOT_VIEW;

                string searchstr = arts_render_common(origx1, origy1, origx2, origy2);

                // loads the image if necessary
                if (!m_cache.snapx_driver_is(system.driver) || !snapx_valid() || m_switch_image)
                {
                    emu_file snapfile = new emu_file(searchstr, OPEN_FLAG_READ);
                    bitmap_argb32 tmp_bitmap;

                    load_driver_image(out tmp_bitmap, snapfile, system.driver);

                    m_cache.set_snapx_driver(system.driver);
                    m_switch_image = false;
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
            float gutter_width = 0.4f * line_height * machine().render().ui_aspect(container()) * 1.3f;

            string snaptext;
            string searchstr;
            get_title_search(out snaptext, out searchstr);

            // apply title to right panel
            float title_size = 0.0f;
            for (int x = FIRST_VIEW; x < LAST_VIEW; x++)
            {
                float text_length;
                ui().draw_text_full(container(),
                        __("selmenu-artwork", arts_info[x].first), origx1, origy1, origx2 - origx1,
                        text_layout.text_justify.CENTER, text_layout.word_wrapping.TRUNCATE, mame_ui_manager.draw_mode.NONE, rgb_t.white(), rgb_t.black(),
                        out text_length, out _);
                title_size = std.max(text_length + 0.01f, title_size);
            }

            rgb_t fgcolor = (m_focus == focused_menu.RIGHTBOTTOM) ? new rgb_t(0xff, 0xff, 0x00) : ui().colors().text_color();
            rgb_t bgcolor = (m_focus == focused_menu.RIGHTBOTTOM) ? new rgb_t(0xff, 0xff, 0xff) : ui().colors().text_bg_color();
            float middle = origx2 - origx1;

            // check size
            float sc = title_size + 2.0f * gutter_width;
            float tmp_size = (sc > middle) ? ((middle - 2.0f * gutter_width) / sc) : 1.0f;
            title_size *= tmp_size;

            if (bgcolor != ui().colors().text_bg_color())
            {
                ui().draw_textured_box(
                        container(),
                        origx1 + ((middle - title_size) * 0.5f), origy1 + ui().box_tb_border(),
                        origx1 + ((middle + title_size) * 0.5f), origy1 + ui().box_tb_border() + line_height,
                        bgcolor, new rgb_t(43, 43, 43),
                        hilight_main_texture(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_TEXWRAP(1));
            }

            ui().draw_text_full(container(),
                    snaptext, origx1, origy1 + ui().box_tb_border(), origx2 - origx1,
                    text_layout.text_justify.CENTER, text_layout.word_wrapping.TRUNCATE, mame_ui_manager.draw_mode.NORMAL, fgcolor, bgcolor,
                    out _, out _, tmp_size);

            draw_common_arrow(origx1, origy1 + ui().box_tb_border(), origx2, origy2, m_image_view, FIRST_VIEW, LAST_VIEW, title_size);

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
                        tmp_bitmap.pix(y, x)[0] = src.pix(y, x)[0];
                    }
                }
                no_available = true;
            }

            bitmap_argb32 snapx_bitmap = m_cache.snapx_bitmap();
            if (tmp_bitmap.valid())
            {
                float panel_width = origx2 - origx1 - 0.02f;
                float panel_height = origy2 - origy1 - 0.02f - (3.0f * ui().box_tb_border()) - (2.0f * line_height);
                int screen_width = machine().render().ui_target().width();
                int screen_height = machine().render().ui_target().height();

                if ((machine().render().ui_target().orientation() & ORIENTATION_SWAP_XY) != 0)
                    std.swap(ref screen_height, ref screen_width);

                int panel_width_pixel = (int)(panel_width * screen_width);
                int panel_height_pixel = (int)(panel_height * screen_height);

                // Calculate resize ratios for resizing
                var ratioW = (float)panel_width_pixel / tmp_bitmap.width();
                var ratioH = (float)panel_height_pixel / tmp_bitmap.height();
                var ratioI = (float)tmp_bitmap.height() / tmp_bitmap.width();
                var dest_xPixel = tmp_bitmap.width();
                var dest_yPixel = tmp_bitmap.height();

                // force 4:3 ratio min
                if (ui().options().forced_4x3_snapshot() && ratioI < 0.75f && m_image_view == SNAPSHOT_VIEW)
                {
                    // smaller ratio will ensure that the image fits in the view
                    dest_yPixel = (int)(tmp_bitmap.width() * 0.75f);
                    ratioH = (float)panel_height_pixel / dest_yPixel;
                    float ratio = std.min(ratioW, ratioH);
                    dest_xPixel = (int)(tmp_bitmap.width() * ratio);
                    dest_yPixel *= (int)ratio;
                }
                // resize the bitmap if necessary
                else if (ratioW < 1 || ratioH < 1 || (ui().options().enlarge_snaps() && !no_available))
                {
                    // smaller ratio will ensure that the image fits in the view
                    float ratio = std.min(ratioW, ratioH);
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
                    render_resample_argb_bitmap_hq(dest_bitmap, tmp_bitmap, color, true);
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
                        snapx_bitmap.pix(y + y1, x + x1)[0] = dest_bitmap.pix(y, x)[0];
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
                float y1 = origy1 + (2.0f * ui().box_tb_border()) + line_height;
                float y2 = origy2 - ui().box_tb_border() - line_height;

                // apply texture
                container().add_quad(x1, y1, x2, y2, rgb_t.white(), m_cache.snapx_texture(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
            }
        }


        // text for main top/bottom panels
        protected abstract void make_topbox_text(out string line0, out string line1, out string line2);

        protected abstract string make_software_description(ui_software_info software, ui_system_info system);


        // filter navigation
        protected abstract void filter_selected();


        static void make_audit_fail_text(ref string str, media_auditor auditor, media_auditor.summary summary)  //void menu_select_launch::make_audit_fail_text(std::ostream &str, media_auditor const &auditor, media_auditor::summary summary)
        {
            if ((media_auditor.summary.NOTFOUND != summary) && !auditor.records().empty())
            {
                string message = "";  //char const *message = nullptr;
                foreach (media_auditor.audit_record record in auditor.records())
                {
                    switch (record.substatus())
                    {
                    case media_auditor.audit_substatus.FOUND_BAD_CHECKSUM:
                        message = __("incorrect checksum");
                        break;
                    case media_auditor.audit_substatus.FOUND_WRONG_LENGTH:
                        message = __("incorrect length");
                        break;
                    case media_auditor.audit_substatus.NOT_FOUND:
                        message = __("not found");
                        break;
                    case media_auditor.audit_substatus.GOOD:
                    case media_auditor.audit_substatus.GOOD_NEEDS_REDUMP:
                    case media_auditor.audit_substatus.FOUND_NODUMP:
                    case media_auditor.audit_substatus.NOT_FOUND_NODUMP:
                    case media_auditor.audit_substatus.NOT_FOUND_OPTIONAL:
                    case media_auditor.audit_substatus.UNVERIFIED:
                        continue;
                    }

                    if (record.shared_device() != null)
                        util.stream_format(ref str, __("{0} ({1}) - {2}\n"), record.name(), record.shared_device().shortname(), message);
                    else
                        util.stream_format(ref str, __("{0} - {1}\n"), record.name(), message);
                }

                str += '\n';
            }

            str += __("Press any key to continue.");
        }


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
                        moptions.set_value(swinfo.instance, util.string_format("{0}:{1}:{2}", swinfo.listname, swinfo.shortname, part), OPTION_PRIORITY_CMDLINE);
                    else
                        moptions.set_value(OPTION_SOFTWARENAME, util.string_format("{0}:{1}", swinfo.listname, swinfo.shortname), OPTION_PRIORITY_CMDLINE);

                    moptions.set_value(OPTION_SNAPNAME, util.path_concat(swinfo.listname, swinfo.shortname), OPTION_PRIORITY_CMDLINE);
                }
                reselect_last.set_software(driver, swinfo);
            }
            else
            {
                reselect_last.set_driver(driver);
            }

            if (bios != 0)
                moptions.set_value(OPTION_BIOS, bios, OPTION_PRIORITY_CMDLINE);

            mame_machine_manager.instance().schedule_new_driver(driver);
            mui.machine().schedule_hard_reset();
            stack_reset(mui.machine());
        }


        //static bool select_part(mame_ui_manager &mui, render_container &container, software_info const &info, ui_software_info const &ui_info);
        //static bool has_multiple_bios(ui_software_info const &swinfo, s_bios &biosname);
        //static bool has_multiple_bios(game_driver const &driver, s_bios &biosname);
    }
}
