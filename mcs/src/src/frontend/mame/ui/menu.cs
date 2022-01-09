// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using char32_t = System.UInt32;
using global_state_map = mame.std.map<mame.running_machine, mame.ui.menu.global_state>;
using texture_ptr = mame.render_texture;
using uint32_t = System.UInt32;

using static mame.corestr_global;
using static mame.cpp_global;
using static mame.emucore_global;
using static mame.language_global;
using static mame.osdfile_global;
using static mame.render_global;
using static mame.rendertypes_global;
using static mame.rendutil_global;
using static mame.ui_global;
using static mame.utils_global;


namespace mame.ui
{
    public abstract class menu : IDisposable
    {
        //using global_state_ptr = std::shared_ptr<global_state>;
        //using global_state_map = std::map<running_machine *, global_state_ptr>;


        // flags for menu items
        //enum : uint32_t
        //{
        public const uint32_t FLAG_LEFT_ARROW     = 1 << 0;
        public const uint32_t FLAG_RIGHT_ARROW    = 1 << 1;
        protected const uint32_t FLAG_INVERT         = 1 << 2;
        const uint32_t FLAG_MULTILINE      = 1 << 3;
        const uint32_t FLAG_REDTEXT        = 1 << 4;
        protected const uint32_t FLAG_DISABLE        = 1 << 5;
        const uint32_t FLAG_UI_HEADING     = 1 << 7;
        const uint32_t FLAG_COLOR_BOX      = 1 << 8;
        //}


        // flags to pass to process
        //enum
        //{
        const uint32_t PROCESS_NOKEYS      = 1 << 0;
        const uint32_t PROCESS_LR_ALWAYS   = 1 << 1;
        protected const uint32_t PROCESS_LR_REPEAT   = 1 << 2;
        const uint32_t PROCESS_CUSTOM_NAV  = 1 << 3;
        protected const uint32_t PROCESS_CUSTOM_ONLY = 1 << 4;
        const uint32_t PROCESS_ONLYCHAR    = 1 << 5;
        protected const uint32_t PROCESS_NOINPUT     = 1 << 6;
        const uint32_t PROCESS_NOIMAGE     = 1 << 7;
        //}


        // options for reset
        protected enum reset_options
        {
            SELECT_FIRST,
            REMEMBER_POSITION,
            REMEMBER_REF
        }


        public class global_state : widgets_manager, IDisposable
        {
            //using bitmap_ptr = widgets_manager::bitmap_ptr;
            //using texture_ptr = widgets_manager::texture_ptr;


            running_machine m_machine;
            bitmap_argb32 m_bgrnd_bitmap;
            render_texture m_bgrnd_texture;

            menu m_stack;
            menu m_free;


            public global_state(running_machine machine, ui_options options)
                : base(machine)
            {
                m_machine = machine;
                m_bgrnd_bitmap = null;
                m_bgrnd_texture = null;  //, m_bgrnd_texture(nullptr, machine.render())
                m_stack = null;
                m_free = null;


                render_manager render = machine.render();

                // create a texture for main menu background
                m_bgrnd_texture = render.texture_alloc(render_texture.hq_scale);  //m_bgrnd_texture.reset(render.texture_alloc(render_texture::hq_scale));
                if (options.use_background_image() && (machine.system() == ___empty.driver____empty))
                {
                    m_bgrnd_bitmap = new bitmap_argb32(0, 0);
                    emu_file backgroundfile = new emu_file(".", OPEN_FLAG_READ);
                    if (!backgroundfile.open("background.jpg"))
                    {
                        render_load_jpeg(out m_bgrnd_bitmap, backgroundfile.core_file_get());
                        backgroundfile.close();
                    }

                    if (!m_bgrnd_bitmap.valid() && !backgroundfile.open("background.png"))
                    {
                        render_load_png(out m_bgrnd_bitmap, backgroundfile.core_file_get());
                        backgroundfile.close();
                    }

                    if (m_bgrnd_bitmap.valid())
                        m_bgrnd_texture.set_bitmap(m_bgrnd_bitmap, m_bgrnd_bitmap.cliprect(), texture_format.TEXFORMAT_ARGB32);
                    else
                        m_bgrnd_bitmap.reset();
                }
            }

            ~global_state()
            {
                assert(m_isDisposed);  // can remove
            }

            bool m_isDisposed = false;
            public void Dispose()
            {
                //throw new emu_unimplemented();
#if false
                // it shouldn't really be possible to get here with active menus because of reference loops
                assert(!m_stack);
                assert(!m_free);
#endif

                stack_reset();
                clear_free_list();

                m_isDisposed = true;
            }


            public bitmap_argb32 bgrnd_bitmap() { return m_bgrnd_bitmap; }
            public render_texture bgrnd_texture() { return m_bgrnd_texture; }


            //template <typename T>
            public T topmost_menu<T>() where T : menu { return (T)m_stack; }


            public void stack_push(menu menu)
            {
                menu.m_parent = m_stack;
                m_stack = menu;
                m_stack.machine().ui_input().reset();
            }


            public void stack_pop()
            {
                if (m_stack != null)
                {
                    menu menu = m_stack;
                    m_stack = menu.m_parent;
                    menu.m_parent = m_free;
                    m_free = menu;
                    m_free.Dispose();
                    m_machine.ui_input().reset();
                }
            }


            public void stack_reset()
            {
                while (m_stack != null)
                    stack_pop();
            }


            public void clear_free_list()
            {
                // free stack is in reverse order - unwind it properly
                menu reversed = null;  //std::unique_ptr<menu> reversed;
                while (m_free != null)
                {
                    menu menu = m_free;  //std::unique_ptr<menu> menu(std::move(m_free));
                    m_free = menu.m_parent;  //m_free = std::move(menu->m_parent);
                    menu.m_parent = reversed;  //menu->m_parent = std::move(reversed);
                    reversed = menu;  //reversed = std::move(menu);
                }

                while (reversed != null)
                    reversed = reversed.m_parent;  //reversed = std::move(reversed->m_parent);
            }


            public bool stack_has_special_main_menu()
            {
                for (var menu = m_stack; menu != null; menu = menu.m_parent)
                {
                    if (menu.is_special_main_menu())
                        return true;
                }

                return false;
            }
        }


        // menu-related events
        protected class menu_event
        {
            public object itemref;  //void                *itemref;   // reference for the selected item
            public menu_item_type type;       // item type (eventually will go away when itemref is proper ui_menu_item class rather than void*)
            public int iptkey;     // one of the IPT_* values from inptport.h
            public char32_t unichar;    // unicode character if iptkey == IPT_SPECIAL
            public render_bounds mouse;      // mouse position if iptkey == IPT_CUSTOM
        }


        int m_selected;   // which item is selected
        int m_hover;      // which item is being hovered over
        std.vector<menu_item> m_items = new std.vector<menu_item>();      // array of items

        protected int top_line;           // main box top line
        protected int m_visible_lines;    // main box visible lines
        protected int m_visible_items;    // number of visible items


        global_state m_global_state;  //global_state_ptr m_global_state;
        bool m_special_main_menu;
        bool m_needs_prev_menu_item;
        mame_ui_manager m_ui;              // UI we are attached to
        render_container m_container;       // render_container we render to
        menu m_parent;           // pointer to parent menu
        menu_event m_event;            // the UI event that occurred

        float m_customtop;        // amount of extra height to add at the top
        float m_custombottom;     // amount of extra height to add at the bottom

        int m_resetpos;         // reset position
        object m_resetref;  //void                    *m_resetref;        // reset reference

        bool m_mouse_hit;
        bool m_mouse_button;
        float m_mouse_x;
        float m_mouse_y;


        //static std::mutex       s_global_state_guard;
        static global_state_map s_global_states = new global_state_map();


        //-------------------------------------------------
        //  ui_menu - menu constructor
        //-------------------------------------------------
        protected menu(mame_ui_manager mui, render_container container)
        {
            m_selected = 0;
            m_items = new std.vector<menu_item>();
            m_visible_lines = 0;
            m_visible_items = 0;
            m_global_state = get_global_state(mui.machine());
            m_special_main_menu = false;
            m_needs_prev_menu_item = true;
            m_ui = mui;
            m_container = container;
            m_parent = null;
            m_event = new menu_event();
            m_customtop = 0.0f;
            m_custombottom = 0.0f;
            m_resetpos = 0;
            m_resetref = null;
            m_mouse_hit = false;
            m_mouse_button = false;
            m_mouse_x = -1.0f;
            m_mouse_y = -1.0f;

            assert(m_global_state != null); // not calling init is bad


            reset(reset_options.SELECT_FIRST);

            top_line = 0;
        }

        ~menu()
        {
            assert(m_isDisposed);  // can remove
        }

        protected bool m_isDisposed = false;
        public virtual void Dispose()
        {
            m_isDisposed = true;
        }


        // append a new item to the end of the menu
        //-------------------------------------------------
        //  item_append - append a new item to the
        //  end of the menu
        //-------------------------------------------------
        protected void item_append(string text, uint32_t flags, object ref_, menu_item_type type = menu_item_type.UNKNOWN) { item_append(text, string.Empty, flags, ref_, type); }  //void item_append(const std::string &text, uint32_t flags, void *ref, menu_item_type type = menu_item_type::UNKNOWN) { item_append(std::string(text), std::string(), flags, ref, type); }
        //void item_append(const std::string &text, const std::string &subtext, uint32_t flags, void *ref, menu_item_type type = menu_item_type::UNKNOWN) { item_append(std::string(text), std::string(subtext), flags, ref, type); }
        //void item_append(std::string &&text, uint32_t flags, void *ref, menu_item_type type = menu_item_type::UNKNOWN) { item_append(text, std::string(), flags, ref, type); }
        protected void item_append(string text, string subtext, uint32_t flags, object ref_, menu_item_type type = menu_item_type.UNKNOWN)  //void item_append(std::string &&text, std::string &&subtext, uint32_t flags, void *ref, menu_item_type type = menu_item_type::UNKNOWN);
        {
            if ((flags & FLAG_MULTILINE) != 0) // only allow multiline as the first item
                assert(m_items.size() == 1);
            else if (m_items.size() >= 2) // only allow a single multi-line item
                assert((m_items[0].flags & FLAG_MULTILINE) == 0);

            // allocate a new item and populate it
            menu_item pitem = new menu_item();
            pitem.text = text;
            pitem.subtext = subtext;
            pitem.flags = flags;
            pitem.ref_ = ref_;
            pitem.type = type;

            // append to array
            var index = m_items.size();
            if (!m_items.empty() && m_needs_prev_menu_item)
            {
                m_items.emplace((int)m_items.size() - 1, pitem);  //item.end() - 1, pitem);
                --index;
            }
            else
            {
                m_items.emplace_back(pitem);
            }

            // update the selection if we need to
            if (m_resetpos == (int)index || (m_resetref != null && m_resetref == ref_))
                m_selected = (int)index;
            if (m_resetpos == (int)m_items.size() - 1)
                m_selected = (int)m_items.size() - 1;
        }


        //-------------------------------------------------
        //  item_append - append a new item to the
        //  end of the menu
        //-------------------------------------------------
        protected void item_append(menu_item item) { item_append(item.text, item.subtext, item.flags, item.ref_, item.type); }


        //-------------------------------------------------
        //  item_append - append a new item to the
        //  end of the menu
        //-------------------------------------------------
        protected void item_append(menu_item_type type, uint32_t flags = 0)
        {
            if (type == menu_item_type.SEPARATOR)
                item_append(menu_item.MENU_SEPARATOR_ITEM, flags, null, menu_item_type.SEPARATOR);
        }


        //void item_append_on_off(const std::string &text, bool state, uint32_t flags, void *ref, menu_item_type type = menu_item_type::UNKNOWN);


        // Global initialization
        //-------------------------------------------------
        //  init - initialize the menu system
        //-------------------------------------------------
        public static void init(running_machine machine, ui_options mopt)
        {
            // initialize the menu stack
            {
                //throw new emu_unimplemented();
#if false
                lock_guard<mutex> guard(s_global_state_guard);
#endif
                var state = new global_state(machine, mopt);
                var ins = s_global_states.emplace(machine, state);
                assert(ins); // calling init twice is bad

                if (ins)
                    machine.add_notifier(machine_notification.MACHINE_NOTIFY_EXIT, exit); // add an exit callback to free memory
                    //machine_notify_delegate(&menu::exit, &machine)
                else
                    state.stack_reset();  //ins.first.second.stack_reset();
            }
        }


        // reset the menus, clearing everything
        public static void stack_reset(running_machine machine) { get_global_state(machine).stack_reset(); }

        // push a new menu onto the stack
        //template <typename T, typename... Params>
        public static void stack_push(menu menu)  // Params &&... args)
        {
            stack_push_internal(menu);  //stack_push(std::unique_ptr<menu>(global_alloc_clear<T>(std::forward<Params>(args)...)));
        }

        //template <typename T, typename... Params>
        public static void stack_push_special_main(menu menu)  // Params &&... args)
        {
            //std::unique_ptr<menu> ptr(global_alloc_clear<T>(std::forward<Params>(args)...));
            menu.set_special_main_menu(true);
            stack_push(menu);
        }

        // pop a menu from the stack
        public static void stack_pop(running_machine machine) { get_global_state(machine).stack_pop(); }

        // test if one of the menus in the stack requires hide disable
        public static bool stack_has_special_main_menu(running_machine machine) { return get_global_state(machine).stack_has_special_main_menu(); }


        // master handler

        //-------------------------------------------------
        //  ui_menu_ui_handler - displays the current menu
        //  and calls the menu handler
        //-------------------------------------------------
        public static uint32_t ui_handler(render_container container, mame_ui_manager mui)
        {
            global_state state = get_global_state(mui.machine());

            // if we have no menus stacked up, start with the main menu
            if (state.topmost_menu<menu>() == null)
                state.stack_push(new menu_main(mui, container));

            // update the menu state
            if (state.topmost_menu<menu>() != null)
                state.topmost_menu<menu>().do_handle();

            // clear up anything pending to be released
            state.clear_free_list();

            // if the menus are to be hidden, return a cancel here
            if (mui.is_menu_active() && ((mui.machine().ui_input().pressed((int)ioport_type.IPT_UI_CONFIGURE) && !state.stack_has_special_main_menu()) || state.topmost_menu<menu>() == null))
                return UI_HANDLER_CANCEL;

            return 0;
        }


        // Used by sliders
        //-------------------------------------------------
        //  validate_selection - validate the
        //  current selection and ensure it is on a
        //  correct item
        //-------------------------------------------------
        protected void validate_selection(int scandir)
        {
            // clamp to be in range
            if (m_selected < 0)
                m_selected = 0;
            else if (m_selected >= (int)m_items.size())
                m_selected = (int)m_items.size() - 1;

            // skip past unselectable items
            while (!is_selectable(m_items[m_selected]))
                m_selected = (m_selected + (int)m_items.size() + scandir) % (int)m_items.size();
        }


        /***************************************************************************
            MENU STACK MANAGEMENT
        ***************************************************************************/

        void do_handle()
        {
            if (m_items.empty())
            {
                // add an item to return - this is a really hacky way of doing this
                if (m_needs_prev_menu_item)
                    item_append(__("Return to Previous Menu"), 0, null);

                // let implementation add other items
                populate(ref m_customtop, ref m_custombottom);
            }

            handle();
        }


        //-------------------------------------------------
        //  draw - draw a menu
        //-------------------------------------------------
        protected virtual void draw(uint32_t flags)
        {
            // first draw the FPS counter
            if (ui().show_fps_counter())
            {
                ui().draw_text_full(
                        container(),
                        machine().video().speed_text(),
                        0.0f, 0.0f, 1.0f,
                        text_layout.text_justify.RIGHT, text_layout.word_wrapping.WORD,
                        mame_ui_manager.draw_mode.OPAQUE_, rgb_t.white(), rgb_t.black(),
                        out _, out _);
            }

            bool customonly = (flags & PROCESS_CUSTOM_ONLY) != 0;
            bool noimage = (flags & PROCESS_NOIMAGE) != 0;
            bool noinput = (flags & PROCESS_NOINPUT) != 0;
            float aspect = machine().render().ui_aspect(container());
            float line_height = ui().get_line_height();
            float lr_arrow_width = 0.4f * line_height * aspect;
            float ud_arrow_width = line_height * aspect;
            float gutter_width = lr_arrow_width * 1.3f;
            float lr_border = ui().box_lr_border() * aspect;

            if (machine().system() == ___empty.driver____empty && !noimage)
                draw_background();

            // compute the width and height of the full menu
            float visible_width = 0;
            float visible_main_menu_height = 0;
            foreach (var pitem in m_items)
            {
                // compute width of left hand side
                float total_width = gutter_width + ui().get_string_width(pitem.text) + gutter_width;

                // add in width of right hand side
                if (!pitem.subtext.empty())
                    total_width += 2.0f * gutter_width + ui().get_string_width(pitem.subtext);

                // track the maximum
                if (total_width > visible_width)
                    visible_width = total_width;

                // track the height as well
                visible_main_menu_height += line_height;
            }

            // account for extra space at the top and bottom
            float visible_extra_menu_height = m_customtop + m_custombottom;

            // add a little bit of slop for rounding
            visible_width += 0.01f;
            visible_main_menu_height += 0.01f;

            // if we are too wide or too tall, clamp it down
            if (visible_width + 2.0f * lr_border > 1.0f)
                visible_width = 1.0f - 2.0f * lr_border;

            // if the menu and extra menu won't fit, take away part of the regular menu, it will scroll
            if (visible_main_menu_height + visible_extra_menu_height + 2.0f * ui().box_tb_border() > 1.0f)
                visible_main_menu_height = 1.0f - 2.0f * ui().box_tb_border() - visible_extra_menu_height;

            m_visible_lines = std.min((int)(std.floor(visible_main_menu_height / line_height)), (int)m_items.size());
            visible_main_menu_height = (float)m_visible_lines * line_height;

            // compute top/left of inner menu area by centering
            float visible_left = (1.0f - visible_width) * 0.5f;
            float visible_top = ((1.0f - visible_main_menu_height - visible_extra_menu_height) * 0.5f) + m_customtop;

            // first add us a box
            float x1 = visible_left - lr_border;
            float y1 = visible_top - ui().box_tb_border();
            float x2 = visible_left + visible_width + lr_border;
            float y2 = visible_top + visible_main_menu_height + ui().box_tb_border();
            if (!customonly)
                ui().draw_outlined_box(container(), x1, y1, x2, y2, ui().colors().background_color());

            if ((m_selected >= (top_line + m_visible_lines)) || (m_selected < (top_line + 1)))
                top_line = m_selected - (m_visible_lines / 2);
            if (top_line < 0 || is_first_selected())
                top_line = 0;
            else if ((top_line > ((int)m_items.size() - m_visible_lines)) || is_last_selected())
                top_line = (int)m_items.size() - m_visible_lines;
            else if (m_selected >= (top_line + m_visible_lines - 2))
                top_line = m_selected - m_visible_lines + ((m_selected == ((int)m_items.size() - 1)) ? 1 : 2);

            // if scrolling, show arrows
            bool show_top_arrow = (((int)m_items.size() > m_visible_lines) && !first_item_visible());
            bool show_bottom_arrow = (((int)m_items.size() > m_visible_lines) && !last_item_visible());

            // set the number of visible lines, minus 1 for top arrow and 1 for bottom arrow
            m_visible_items = m_visible_lines - (show_top_arrow ? 1 : 0) - (show_bottom_arrow ? 1 : 0);

            // determine effective positions taking into account the hilighting arrows
            float effective_width = visible_width - 2.0f * gutter_width;
            float effective_left = visible_left + gutter_width;

            // locate mouse
            if (!customonly && !noinput)
                map_mouse();
            else
                ignore_mouse();

            // loop over visible lines
            m_hover = (int)m_items.size() + 1;
            bool selected_subitem_too_big = false;
            float line_x0 = x1 + 0.5f * UI_LINE_WIDTH;
            float line_x1 = x2 - 0.5f * UI_LINE_WIDTH;
            if (!customonly)
            {
                for (int linenum = 0; linenum < m_visible_lines; linenum++)
                {
                    var itemnum = top_line + linenum;
                    menu_item pitem = m_items[itemnum];
                    string itemtext = pitem.text;
                    rgb_t fgcolor = ui().colors().text_color();
                    rgb_t bgcolor = ui().colors().text_bg_color();
                    rgb_t fgcolor2 = ui().colors().subitem_color();
                    rgb_t fgcolor3 = ui().colors().clone_color();
                    float line_y0 = visible_top + (float)linenum * line_height;
                    float line_y1 = line_y0 + line_height;

                    // work out what we're dealing with
                    bool uparrow = linenum == 0 && show_top_arrow;
                    bool downarrow = (linenum == (m_visible_lines - 1)) && show_bottom_arrow;

                    // set the hover if this is our item
                    bool hovered = mouse_in_rect(line_x0, line_y0, line_x1, line_y1);
                    if (hovered)
                    {
                        if (uparrow)
                            m_hover = HOVER_ARROW_UP;
                        else if (downarrow)
                            m_hover = HOVER_ARROW_DOWN;
                        else if (is_selectable(pitem))
                            m_hover = itemnum;
                    }

                    if (is_selected(itemnum))
                    {
                        // if we're selected, draw with a different background
                        fgcolor = fgcolor2 = fgcolor3 = ui().colors().selected_color();
                        bgcolor = ui().colors().selected_bg_color();
                    }
                    else if (hovered && (uparrow || downarrow || is_selectable(pitem)))
                    {
                        // else if the mouse is over this item, draw with a different background
                        fgcolor = fgcolor2 = fgcolor3 = ui().colors().mouseover_color();
                        bgcolor = ui().colors().mouseover_bg_color();
                    }

                    // if we have some background hilighting to do, add a quad behind everything else
                    if (bgcolor != ui().colors().text_bg_color())
                        highlight(line_x0, line_y0, line_x1, line_y1, bgcolor);

                    if (uparrow)
                    {
                        // if we're on the top line, display the up arrow
                        draw_arrow(
                                    0.5f * (x1 + x2) - 0.5f * ud_arrow_width,
                                    line_y0 + 0.25f * line_height,
                                    0.5f * (x1 + x2) + 0.5f * ud_arrow_width,
                                    line_y0 + 0.75f * line_height,
                                    fgcolor,
                                    ROT0);
                    }
                    else if (downarrow)
                    {
                        // if we're on the bottom line, display the down arrow
                        draw_arrow(
                                    0.5f * (x1 + x2) - 0.5f * ud_arrow_width,
                                    line_y0 + 0.25f * line_height,
                                    0.5f * (x1 + x2) + 0.5f * ud_arrow_width,
                                    line_y0 + 0.75f * line_height,
                                    fgcolor,
                                    ROT0 ^ ORIENTATION_FLIP_Y);
                    }
                    else if (pitem.type == menu_item_type.SEPARATOR)
                    {
                        // if we're just a divider, draw a line
                        container().add_line(visible_left, line_y0 + 0.5f * line_height, visible_left + visible_width, line_y0 + 0.5f * line_height, UI_LINE_WIDTH, ui().colors().border_color(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
                    }
                    else if (pitem.subtext.empty())
                    {
                        // if we don't have a subitem, just draw the string centered
                        if ((pitem.flags & FLAG_UI_HEADING) != 0)
                        {
                            float heading_width = ui().get_string_width(itemtext);
                            container().add_line(visible_left, line_y0 + 0.5f * line_height, visible_left + ((visible_width - heading_width) / 2) - lr_border, line_y0 + 0.5f * line_height, UI_LINE_WIDTH, ui().colors().border_color(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
                            container().add_line(visible_left + visible_width - ((visible_width - heading_width) / 2) + lr_border, line_y0 + 0.5f * line_height, visible_left + visible_width, line_y0 + 0.5f * line_height, UI_LINE_WIDTH, ui().colors().border_color(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
                        }

                        ui().draw_text_full(
                                container(),
                                itemtext,
                                effective_left, line_y0, effective_width,
                                text_layout.text_justify.CENTER, text_layout.word_wrapping.TRUNCATE,
                                mame_ui_manager.draw_mode.NORMAL, fgcolor, bgcolor,
                                out _, out _);
                    }
                    else
                    {
                        // otherwise, draw the item on the left and the subitem text on the right
                        bool subitem_invert = (pitem.flags & FLAG_INVERT) != 0;
                        float item_width;
                        float subitem_width;

                        // draw the left-side text
                        ui().draw_text_full(
                                container(),
                                itemtext,
                                effective_left, line_y0, effective_width,
                                text_layout.text_justify.LEFT, text_layout.word_wrapping.TRUNCATE,
                                mame_ui_manager.draw_mode.NORMAL, fgcolor, bgcolor,
                                out item_width, out _);

                        if ((pitem.flags & FLAG_COLOR_BOX) != 0)
                        {
                            rgb_t color = new rgb_t((uint32_t)std.strtoul(pitem.subtext, null, 16));

                            // give 2 spaces worth of padding
                            subitem_width = ui().get_string_width("FF00FF00");

                            ui().draw_outlined_box(
                                    container(),
                                    effective_left + effective_width - subitem_width, line_y0 + (UI_LINE_WIDTH * 2.0f),
                                    effective_left + effective_width, line_y1 - (UI_LINE_WIDTH * 2.0f),
                                    color);
                        }
                        else
                        {
                            string subitem_text = pitem.subtext;

                            // give 2 spaces worth of padding
                            item_width += 2.0f * gutter_width;

                            // if the subitem doesn't fit here, display dots
                            if (ui().get_string_width(subitem_text) > effective_width - item_width)
                            {
                                subitem_text = "...";
                                if (is_selected(itemnum))
                                    selected_subitem_too_big = true;
                            }

                            // customize subitem text color
                            if (core_stricmp(pitem.subtext, "On") == 0)
                                fgcolor2 = new rgb_t(0x00,0xff,0x00);

                            if (core_stricmp(pitem.subtext, "Off") == 0)
                                fgcolor2 = new rgb_t(0xff,0x00,0x00);

                            if (core_stricmp(pitem.subtext, "Auto") == 0)
                                fgcolor2 = new rgb_t(0xff,0xff,0x00);

                            // draw the subitem right-justified
                            ui().draw_text_full(
                                    container(),
                                    subitem_text,
                                    effective_left + item_width, line_y0, effective_width - item_width,
                                    text_layout.text_justify.RIGHT, text_layout.word_wrapping.TRUNCATE,
                                    mame_ui_manager.draw_mode.NORMAL, subitem_invert ? fgcolor3 : fgcolor2, bgcolor,
                                    out subitem_width, out _);
                        }

                        // apply arrows
                        if (is_selected(itemnum) && (pitem.flags & FLAG_LEFT_ARROW) != 0)
                        {
                            float l = effective_left + effective_width - subitem_width - gutter_width;
                            float r = l + lr_arrow_width;
                            draw_arrow(
                                        l, line_y0 + 0.1f * line_height, r, line_y0 + 0.9f * line_height,
                                        fgcolor,
                                        ROT90 ^ ORIENTATION_FLIP_X);
                            if (mouse_in_rect(l, line_y0 + 0.1f * line_height, r, line_y0 + 0.9f * line_height))
                                m_hover = HOVER_UI_LEFT;
                        }
                        if (is_selected(itemnum) && (pitem.flags & FLAG_RIGHT_ARROW) != 0)
                        {
                            float r = effective_left + effective_width + gutter_width;
                            float l = r - lr_arrow_width;
                            draw_arrow(
                                        l, line_y0 + 0.1f * line_height, r, line_y0 + 0.9f * line_height,
                                        fgcolor,
                                        ROT90);
                            if (mouse_in_rect(l, line_y0 + 0.1f * line_height, r, line_y0 + 0.9f * line_height))
                                m_hover = HOVER_UI_RIGHT;
                        }
                    }
                }
            }

            // if the selected subitem is too big, display it in a separate offset box
            if (selected_subitem_too_big)
            {
                menu_item pitem = selected_item();
                bool subitem_invert = (pitem.flags & FLAG_INVERT) != 0;
                var linenum = m_selected - top_line;
                float line_y = visible_top + (float)linenum * line_height;
                float target_width, target_height;

                // compute the multi-line target width/height
                ui().draw_text_full(
                        container(),
                        pitem.subtext,
                        0, 0, visible_width * 0.75f,
                        text_layout.text_justify.RIGHT, text_layout.word_wrapping.WORD,
                        mame_ui_manager.draw_mode.NONE, rgb_t.white(), rgb_t.black(),
                        out target_width, out target_height);

                // determine the target location
                float target_x = visible_left + visible_width - target_width - lr_border;
                float target_y = line_y + line_height + ui().box_tb_border();
                if (target_y + target_height + ui().box_tb_border() > visible_main_menu_height)
                    target_y = line_y - target_height - ui().box_tb_border();

                // add a box around that
                ui().draw_outlined_box(
                        container(),
                        target_x - lr_border, target_y - ui().box_tb_border(),
                        target_x + target_width + lr_border, target_y + target_height + ui().box_tb_border(),
                        subitem_invert ? ui().colors().selected_bg_color() : ui().colors().background_color());

                ui().draw_text_full(
                        container(),
                        pitem.subtext,
                        target_x, target_y, target_width,
                        text_layout.text_justify.RIGHT, text_layout.word_wrapping.WORD,
                        mame_ui_manager.draw_mode.NORMAL, ui().colors().selected_color(), ui().colors().selected_bg_color(),
                        out _, out _);
            }

            // if there is something special to add, do it by calling the virtual method
            custom_render(get_selection_ref(), m_customtop, m_custombottom, x1, y1, x2, y2);
        }


        //-------------------------------------------------
        //  draw_text_box - draw a multiline
        //  word-wrapped text box with a menu item at the
        //  bottom
        //-------------------------------------------------
        void draw_text_box()
        {
            string text = m_items[0].text;
            string backtext = m_items[1].text;
            float aspect = machine().render().ui_aspect(container());
            float line_height = mame_machine_manager.instance().ui().get_line_height();
            float lr_arrow_width = 0.4f * line_height * aspect;
            float gutter_width = lr_arrow_width;
            float lr_border = ui().box_lr_border() * aspect;
            float target_width;
            float target_height;
            float prior_width;
            float target_x;
            float target_y;

            // compute the multi-line target width/height
            ui().draw_text_full(
                    container(),
                    text,
                    0, 0, 1.0f - 2.0f * lr_border - 2.0f * gutter_width,
                    text_layout.text_justify.LEFT, text_layout.word_wrapping.WORD,
                    mame_ui_manager.draw_mode.NONE, rgb_t.white(), rgb_t.black(),
                    out target_width, out target_height);
            target_height += 2.0f * line_height;
            if (target_height > 1.0f - 2.0f * ui().box_tb_border())
                target_height = floorf((1.0f - 2.0f * ui().box_tb_border()) / line_height) * line_height;

            // maximum against "return to prior menu" text
            prior_width = ui().get_string_width(backtext) + 2.0f * gutter_width;
            target_width = std.max(target_width, prior_width);

            // determine the target location
            target_x = 0.5f - 0.5f * target_width;
            target_y = 0.5f - 0.5f * target_height;

            // make sure we stay on-screen
            if (target_x < lr_border + gutter_width)
                target_x = lr_border + gutter_width;
            if (target_x + target_width + gutter_width + lr_border > 1.0f)
                target_x = 1.0f - lr_border - gutter_width - target_width;
            if (target_y < ui().box_tb_border())
                target_y = ui().box_tb_border();
            if (target_y + target_height + ui().box_tb_border() > 1.0f)
                target_y = 1.0f - ui().box_tb_border() - target_height;

            // add a box around that
            ui().draw_outlined_box(
                    container(),
                    target_x - lr_border - gutter_width, target_y - ui().box_tb_border(),
                    target_x + target_width + gutter_width + lr_border, target_y + target_height + ui().box_tb_border(),
                    (m_items[0].flags & FLAG_REDTEXT) != 0 ? UI_RED_COLOR : ui().colors().background_color());
            ui().draw_text_full(
                    container(),
                    text,
                    target_x, target_y, target_width,
                    text_layout.text_justify.LEFT, text_layout.word_wrapping.WORD,
                    mame_ui_manager.draw_mode.NORMAL, ui().colors().text_color(), ui().colors().text_bg_color(),
                    out _, out _);

            // draw the "return to prior menu" text with a hilight behind it
            highlight(
                    target_x + 0.5f * UI_LINE_WIDTH,
                    target_y + target_height - line_height,
                    target_x + target_width - 0.5f * UI_LINE_WIDTH,
                    target_y + target_height,
                    ui().colors().selected_bg_color());
            ui().draw_text_full(
                    container(),
                    backtext,
                    target_x, target_y + target_height - line_height, target_width,
                    text_layout.text_justify.CENTER, text_layout.word_wrapping.TRUNCATE,
                    mame_ui_manager.draw_mode.NORMAL, ui().colors().selected_color(), ui().colors().selected_bg_color(),
                    out _, out _);

            // artificially set the hover to the last item so a double-click exits
            m_hover = (int)m_items.size() - 1;
        }


        protected mame_ui_manager ui() { return m_ui; }
        protected running_machine machine() { return m_ui.machine(); }
        protected render_container container() { return m_container; }


        protected void set_needs_prev_menu_item(bool needs) { m_needs_prev_menu_item = needs; }


        // free all items in the menu, and all memory allocated from the memory pool
        //-------------------------------------------------
        //  reset - free all items in the menu,
        //-------------------------------------------------
        protected void reset(reset_options options)
        {
            // based on the reset option, set the reset info
            m_resetpos = 0;
            m_resetref = null;
            if (options == reset_options.REMEMBER_POSITION)
                m_resetpos = m_selected;
            else if (options == reset_options.REMEMBER_REF)
                m_resetref = get_selection_ref();

            // reset the item count back to 0
            m_items.clear();
            m_visible_items = 0;
            m_selected = 0;
        }


        //void reset_parent(reset_options options) { m_parent->reset(options); }


        //template <typename T> T *topmost_menu() const { return m_global_state->topmost_menu<T>(); }
        //template <typename T> static T *topmost_menu(running_machine &machine) { return get_global_state(machine)->topmost_menu<T>(); }

        protected void stack_pop() { m_global_state.stack_pop(); }
        protected void stack_reset() { m_global_state.stack_reset(); }
        protected bool stack_has_special_main_menu() { return m_global_state.stack_has_special_main_menu(); }


        // process a menu, drawing it and returning any interesting events
        //-------------------------------------------------
        //  process - process a menu, drawing it
        //  and returning any interesting events
        //-------------------------------------------------
        protected menu_event process(uint32_t flags, float x0 = 0.0f, float y0 = 0.0f)
        {
            // reset the menu_event
            m_event.iptkey = (int)ioport_type.IPT_INVALID;

            // first make sure our selection is valid
            validate_selection(1);

            // draw the menu
            if (m_items.size() > 1 && (m_items[0].flags & FLAG_MULTILINE) != 0)
                draw_text_box();
            else
                draw(flags);

            // process input
            if ((flags & PROCESS_NOKEYS) == 0 && (flags & PROCESS_NOINPUT) == 0)
            {
                // read events
                handle_events(flags, m_event);

                // handle the keys if we don't already have an menu_event
                if (m_event.iptkey == (int)ioport_type.IPT_INVALID)
                    handle_keys(flags, ref m_event.iptkey);
            }

            // update the selected item in the menu_event
            if (m_event.iptkey != (int)ioport_type.IPT_INVALID && selection_valid())
            {
                m_event.itemref = get_selection_ref();
                m_event.type = m_items[m_selected].type;
                return m_event;
            }
            else
            {
                return null;
            }
        }


        //void process_parent() { m_parent->process(PROCESS_NOINPUT); }


        protected menu_item item(int index) { return m_items[index]; }
        protected int item_count() { return (int)m_items.size(); }


        // retrieves the ref of the currently selected menu item or nullptr
        protected object get_selection_ref() { return selection_valid() ? m_items[m_selected].ref_ : null; }


        protected menu_item selected_item() { return m_items[m_selected]; }
        //menu_item const &selected_item() const { return m_items[m_selected]; }
        protected int selected_index() { return m_selected; }
        bool selection_valid() { return (0 <= m_selected) && ((int)m_items.size() > m_selected); }
        protected bool is_selected(int index) { return selection_valid() && (m_selected == index); }
        protected bool is_first_selected() { return 0 == m_selected; }
        protected bool is_last_selected() { return ((int)m_items.size() - 1) == m_selected; }


        // changes the index of the currently selected menu item
        //void set_selection(void *selected_itemref);
        protected void set_selected_index(int index) { m_selected = index; }


        //-------------------------------------------------
        //  select_first_item - select the first item in
        //  the menu
        //-------------------------------------------------
        protected void select_first_item()
        {
            m_selected = top_line = 0;
            validate_selection(1);
        }


        //-------------------------------------------------
        //  select_last_item - select the last item in the
        //  menu
        //-------------------------------------------------
        protected void select_last_item()
        {
            m_selected = top_line = (int)m_items.size() - 1;
            validate_selection(-1);
        }


        protected int hover() { return m_hover; }
        protected void set_hover(int index) { m_hover = index; }
        protected void clear_hover() { m_hover = (int)m_items.size() + 1; }


        // scroll position control
        //void centre_selection() { top_line = m_selected - (m_visible_lines / 2); }


        //-------------------------------------------------
        //  exclusive_input_pressed - return TRUE if the
        //  given key is pressed and we haven't already
        //  reported a key
        //-------------------------------------------------
        protected bool exclusive_input_pressed(ref int iptkey, int key, int repeat)
        {
            if (iptkey == (int)ioport_type.IPT_INVALID && machine().ui_input().pressed_repeat(key, repeat))
            {
                iptkey = key;
                return true;
            }
            else
            {
                return false;
            }
        }


        // layout
        protected float get_customtop() { return m_customtop; }
        protected float get_custombottom() { return m_custombottom; }


        // highlight
        //-------------------------------------------------
        //  highlight
        //-------------------------------------------------
        public void highlight(float x0, float y0, float x1, float y1, rgb_t bgcolor)
        {
            container().add_quad(x0, y0, x1, y1, bgcolor, m_global_state.hilight_texture(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_TEXWRAP(1) | PRIMFLAG_PACKABLE);
        }


        protected render_texture hilight_main_texture() { return m_global_state.hilight_main_texture(); }


        // draw arrow
        //-------------------------------------------------
        //  draw_arrow
        //-------------------------------------------------
        protected void draw_arrow(float x0, float y0, float x1, float y1, rgb_t fgcolor, uint32_t orientation)
        {
            container().add_quad(x0, y0, x1, y1, fgcolor, m_global_state.arrow_texture(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_TEXORIENT(orientation) | PRIMFLAG_PACKABLE);
        }


        // draw header and footer text
        //void extra_text_render(float top, float bottom, float origx1, float origy1, float origx2, float origy2, std::string_view header, std::string_view footer);
        //void extra_text_position(float origx1, float origx2, float origy, float yspan, text_layout &layout, int direction, float &x1, float &y1, float &x2, float &y2);


        // draw a box of text - used for the custom boxes above/below menus
        //template <typename Iter>
        protected float draw_text_box(
                IEnumerable<string> iter,  //Iter begin, Iter end,
                float origx1, float origx2, float y1, float y2,
                text_layout.text_justify justify, text_layout.word_wrapping wrap, bool scale,
                rgb_t fgcolor, rgb_t bgcolor, float text_size)
        {
            // size up the text
            float lrborder = ui().box_lr_border() * machine().render().ui_aspect(container());
            float maxwidth = origx2 - origx1;
            foreach (var it in iter)  //for (Iter it = begin; it != end; ++it)
            {
                float width;
                ui().draw_text_full(
                        container(), it,
                        0.0f, 0.0f, 1.0f, justify, wrap,
                        mame_ui_manager.draw_mode.NONE, rgb_t.black(), rgb_t.white(),
                        out width, out _, text_size);
                width += 2.0f * lrborder;
                maxwidth = std.max(maxwidth, width);
            }

            if (scale && ((origx2 - origx1) < maxwidth))
            {
                text_size *= ((origx2 - origx1) / maxwidth);
                maxwidth = origx2 - origx1;
            }

            // draw containing box
            float x1 = 0.5f * (1.0f - maxwidth);
            float x2 = x1 + maxwidth;
            ui().draw_outlined_box(container(), x1, y1, x2, y2, bgcolor);

            // inset box and draw content
            x1 += lrborder;
            x2 -= lrborder;
            y1 += ui().box_tb_border();
            y2 -= ui().box_tb_border();
            foreach (var it in iter)  //for (Iter it = begin; it != end; ++it)
            {
                ui().draw_text_full(
                        container(), it,
                        x1, y1, x2 - x1, justify, wrap,
                        mame_ui_manager.draw_mode.NORMAL, fgcolor, ui().colors().text_bg_color(),
                        out _, out _, text_size);
                y1 += ui().get_line_height();
            }

            // in case you want another box of similar width
            return maxwidth;
        }


        protected void draw_background()
        {
            // draw background image if available
            if (ui().options().use_background_image() && m_global_state.bgrnd_bitmap() != null && m_global_state.bgrnd_bitmap().valid())
                container().add_quad(0.0f, 0.0f, 1.0f, 1.0f, rgb_t.white(), m_global_state.bgrnd_texture(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
        }


        // draw additional menu content
        protected virtual void custom_render(object selectedref, float top, float bottom, float x, float y, float x2, float y2) { }  // void * selectedref


        // map mouse to menu coordinates
        //-------------------------------------------------
        //  map_mouse - map mouse pointer location to menu
        //  coordinates
        //-------------------------------------------------
        protected void map_mouse()
        {
            ignore_mouse();
            int mouse_target_x;
            int mouse_target_y;
            render_target mouse_target = machine().ui_input().find_mouse(out mouse_target_x, out mouse_target_y, out m_mouse_button);
            if (mouse_target != null)
            {
                if (mouse_target.map_point_container(mouse_target_x, mouse_target_y, container(), out m_mouse_x, out m_mouse_y))
                    m_mouse_hit = true;
            }
        }


        // clear the mouse position
        //-------------------------------------------------
        //  ignore_mouse - set members to ignore mouse
        //  input
        //-------------------------------------------------
        protected void ignore_mouse()
        {
            m_mouse_hit = false;
            m_mouse_button = false;
            m_mouse_x = -1.0f;
            m_mouse_y = -1.0f;
        }


        //bool is_mouse_hit() const { return m_mouse_hit; }   // is mouse pointer inside menu's render container?
        //float get_mouse_x() const { return m_mouse_x; }     // mouse x location in menu coordinates
        //float get_mouse_y() const { return m_mouse_y; }     // mouse y location in menu coordinates


        // mouse hit test - checks whether mouse_x is in [x0, x1) and mouse_y is in [y0, y1)
        protected bool mouse_in_rect(float x0, float y0, float x1, float y1)
        {
            return m_mouse_hit && (m_mouse_x >= x0) && (m_mouse_x < x1) && (m_mouse_y >= y0) && (m_mouse_y < y1);
        }


        //-------------------------------------------------
        //  handle_events - generically handle
        //  input events for a menu
        //-------------------------------------------------
        protected virtual void handle_events(uint32_t flags, menu_event ev)
        {
            bool stop = false;
            ui_event local_menu_event;

            // loop while we have interesting events
            while (!stop && machine().ui_input().pop_event(out local_menu_event))
            {
                switch (local_menu_event.event_type)
                {
                    // if we are hovering over a valid item, select it with a single click
                    case ui_event.type.MOUSE_DOWN:
                        if (custom_mouse_down())
                            return;

                        if ((flags & PROCESS_ONLYCHAR) == 0)
                        {
                            if (m_hover >= 0 && m_hover < (int)m_items.size())
                            {
                                m_selected = m_hover;
                            }
                            else if (m_hover == HOVER_ARROW_UP)
                            {
                                if ((flags & PROCESS_CUSTOM_NAV) != 0)
                                {
                                    ev.iptkey = (int)ioport_type.IPT_UI_PAGE_UP;
                                    stop = true;
                                }
                                else
                                {
                                    m_selected -= m_visible_items;
                                    if (m_selected < 0)
                                        m_selected = 0;
                                    top_line -= m_visible_items - (last_item_visible() ? 1 : 0);
                                }
                            }
                            else if (m_hover == HOVER_ARROW_DOWN)
                            {
                                if ((flags & PROCESS_CUSTOM_NAV) != 0)
                                {
                                    ev.iptkey = (int)ioport_type.IPT_UI_PAGE_DOWN;
                                    stop = true;
                                }
                                else
                                {
                                    m_selected += m_visible_lines - 2 + (is_first_selected() ? 1 : 0);
                                    if (m_selected > (int)m_items.size() - 1)
                                        m_selected = (int)m_items.size() - 1;
                                    top_line += m_visible_lines - 2;
                                }
                            }
                            else if (m_hover == HOVER_UI_LEFT)
                            {
                                ev.iptkey = (int)ioport_type.IPT_UI_LEFT;
                                stop = true;
                            }
                            else if (m_hover == HOVER_UI_RIGHT)
                            {
                                ev.iptkey = (int)ioport_type.IPT_UI_RIGHT;
                                stop = true;
                            }
                        }
                        break;

                    // if we are hovering over a valid item, fake a UI_SELECT with a double-click
                    case ui_event.type.MOUSE_DOUBLE_CLICK:
                        if ((flags & PROCESS_ONLYCHAR) == 0 && m_hover >= 0 && m_hover < (int)m_items.size())
                        {
                            m_selected = m_hover;
                            ev.iptkey = (int)ioport_type.IPT_UI_SELECT;
                            if (is_last_selected() && m_needs_prev_menu_item)
                            {
                                ev.iptkey = (int)ioport_type.IPT_UI_CANCEL;
                                stack_pop();
                            }
                            stop = true;
                        }
                        break;

                    // caught scroll event
                    case ui_event.type.MOUSE_WHEEL:
                        if ((flags & PROCESS_ONLYCHAR) == 0)
                        {
                            if (local_menu_event.zdelta > 0)
                            {
                                if ((flags & PROCESS_CUSTOM_NAV) != 0) // FIXME: DAT menu logic - let the derived class handle this
                                {
                                    top_line -= local_menu_event.num_lines;
                                    return;
                                }
                                else if (is_first_selected())
                                {
                                    select_last_item();
                                }
                                else
                                {
                                    m_selected -= local_menu_event.num_lines;
                                    validate_selection(-1);
                                }

                                top_line -= (m_selected <= top_line && top_line != 0) ? 1 : 0;
                                if (m_selected <= top_line && m_visible_items != m_visible_lines)
                                    top_line -= local_menu_event.num_lines;
                            }
                            else
                            {
                                if ((flags & PROCESS_CUSTOM_NAV) != 0) // FIXME: DAT menu logic - let the derived class handle this
                                {
                                    top_line += local_menu_event.num_lines;
                                    return;
                                }
                                else if (is_last_selected())
                                {
                                    select_first_item();
                                }
                                else
                                {
                                    m_selected += local_menu_event.num_lines;
                                    validate_selection(1);
                                }

                                top_line += (m_selected >= top_line + m_visible_items + ((top_line != 0) ? 1 : 0)) ? 1 : 0;
                                if (m_selected >= (top_line + m_visible_items + ((top_line != 0) ? 1 : 0)))
                                    top_line += local_menu_event.num_lines;
                            }
                        }
                        break;

                    // translate CHAR events into specials
                    case ui_event.type.IME_CHAR:
                        ev.iptkey = (int)ioport_type.IPT_SPECIAL;
                        ev.unichar = local_menu_event.ch;
                        stop = true;
                        break;

                    // ignore everything else
                    default:
                        break;
                }
            }
        }


        //-------------------------------------------------
        //  handle_keys - generically handle
        //  keys for a menu
        //-------------------------------------------------
        protected virtual void handle_keys(uint32_t flags, ref int iptkey)
        {
            bool ignorepause = stack_has_special_main_menu();
            int code;

            // bail if no items
            if (m_items.size() == 0)
                return;

            // if we hit select, return TRUE or pop the stack, depending on the item
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_SELECT, 0))
            {
                if (is_last_selected() && m_needs_prev_menu_item)
                {
                    iptkey = (int)ioport_type.IPT_UI_CANCEL;
                    stack_pop(machine());
                }
                return;
            }

            // bail out
            if ((flags & PROCESS_ONLYCHAR) != 0)
                return;

            // hitting cancel also pops the stack
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_CANCEL, 0))
            {
                if (!custom_ui_cancel())
                    stack_pop();
                return;
            }

            // validate the current selection
            validate_selection(1);

            // swallow left/right keys if they are not appropriate
            bool ignoreleft = (flags & PROCESS_LR_ALWAYS) == 0 && (selected_item().flags & FLAG_LEFT_ARROW) == 0;
            bool ignoreright = (flags & PROCESS_LR_ALWAYS) == 0 && (selected_item().flags & FLAG_RIGHT_ARROW) == 0;

            // accept left/right keys as-is with repeat
            if (ignoreleft && exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_LEFT, (flags & PROCESS_LR_REPEAT) != 0 ? 6 : 0))
                return;
            if (ignoreright && exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_RIGHT, (flags & PROCESS_LR_REPEAT) != 0 ? 6 : 0))
                return;

            // up backs up by one item
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_UP, 6))
            {
                if ((flags & PROCESS_CUSTOM_NAV) != 0)
                {
                    return;
                }
                else if (is_first_selected())
                {
                    select_last_item();
                }
                else
                {
                    --m_selected;
                    validate_selection(-1);
                }

                top_line -= (m_selected <= top_line && top_line != 0) ? 1 : 0;
                if (m_selected <= top_line && m_visible_items != m_visible_lines)
                    top_line--;
            }

            // down advances by one item
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_DOWN, 6))
            {
                if ((flags & PROCESS_CUSTOM_NAV) != 0)
                {
                    return;
                }
                else if (is_last_selected())
                {
                    select_first_item();
                }
                else
                {
                    ++m_selected;
                    validate_selection(1);
                }

                top_line += (m_selected >= top_line + m_visible_items + ((top_line != 0) ? 1 : 0)) ? 1 : 0;
                if (m_selected >= (top_line + m_visible_items + ((top_line != 0) ? 1 : 0)))
                    top_line++;
            }

            // page up backs up by visitems
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_PAGE_UP, 6))
            {
                if ((flags & PROCESS_CUSTOM_NAV) != 0)
                    return;

                m_selected -= m_visible_items;
                top_line -= m_visible_items - (last_item_visible() ? 1 : 0);
                if (m_selected < 0)
                    m_selected = 0;
                validate_selection(1);
            }

            // page down advances by visitems
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_PAGE_DOWN, 6))
            {
                if ((flags & PROCESS_CUSTOM_NAV) != 0)
                    return;

                m_selected += m_visible_lines - 2 + (is_first_selected() ? 1 : 0);
                top_line += m_visible_lines - 2;

                if (m_selected > (int)m_items.size() - 1)
                    m_selected = (int)m_items.size() - 1;
                validate_selection(-1);
            }

            // home goes to the start
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_HOME, 0))
            {
                if ((flags & PROCESS_CUSTOM_NAV) != 0)
                    return;

                select_first_item();
            }

            // end goes to the last
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_END, 0))
            {
                if ((flags & PROCESS_CUSTOM_NAV) != 0)
                    return;

                select_last_item();
            }

            // pause enables/disables pause
            if (!ignorepause && exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_PAUSE, 0))
            {
                if (machine().paused())
                    machine().resume();
                else
                    machine().pause();
            }

            // handle a toggle cheats request
            if (machine().ui_input().pressed_repeat((int)ioport_type.IPT_UI_TOGGLE_CHEAT, 0))
                mame_machine_manager.instance().cheat().set_enable(!mame_machine_manager.instance().cheat().enabled());

            // see if any other UI keys are pressed
            if (iptkey == (int)ioport_type.IPT_INVALID)
            {
                for (code = (int)ioport_type.IPT_UI_FIRST + 1; code < (int)ioport_type.IPT_UI_LAST; code++)
                {
                    if (code == (int)ioport_type.IPT_UI_CONFIGURE || (code == (int)ioport_type.IPT_UI_LEFT && ignoreleft) || (code == (int)ioport_type.IPT_UI_RIGHT && ignoreright) || (code == (int)ioport_type.IPT_UI_PAUSE && ignorepause))
                        continue;

                    if (exclusive_input_pressed(ref iptkey, code, 0))
                        break;
                }
            }
        }


        protected virtual bool custom_ui_cancel() { return false; }


        // overridable event handling
        protected virtual bool custom_mouse_down() { return false; }


        protected static bool is_selectable(menu_item item) { return (item.flags & (FLAG_MULTILINE | FLAG_DISABLE)) == 0 && item.type != menu_item_type.SEPARATOR; }


        // get arrows status
        //template <typename _T1, typename _T2, typename _T3>
        //UINT32 get_arrow_flags(_T1 min, _T2 max, _T3 actual)
        //{
        //    if (max == 0)
        //        return 0;
        //    else
        //        return ((actual <= min) ? MENU_FLAG_RIGHT_ARROW : (actual >= max ? MENU_FLAG_LEFT_ARROW : (MENU_FLAG_LEFT_ARROW | MENU_FLAG_RIGHT_ARROW)));
        //}


        // request the specific handling of the game selection main menu
        //-------------------------------------------------
        //  is_special_main_menu - returns whether the
        //  menu has special needs
        //-------------------------------------------------
        bool is_special_main_menu() { return m_special_main_menu; }

        //-------------------------------------------------
        //  set_special_main_menu - set whether the
        //  menu has special needs
        //-------------------------------------------------
        public void set_special_main_menu(bool special) { m_special_main_menu = special; }


        // To be reimplemented in the menu subclass
        protected abstract void populate(ref float customtop, ref float custombottom);


        // To be reimplemented in the menu subclass
        protected abstract void handle();


        // push a new menu onto the stack
        static void stack_push_internal(menu menu) { get_global_state(menu.machine()).stack_push(menu); }


        //void extra_text_draw_box(float origx1, float origx2, float origy, float yspan, std::string_view text, int direction);


        bool first_item_visible() { return top_line <= 0; }
        bool last_item_visible() { return (top_line + m_visible_lines) >= (int)m_items.size(); }


        //-------------------------------------------------
        //  exit - clean up after ourselves
        //-------------------------------------------------
        static void exit(running_machine machine)
        {
            // free menus
            global_state state = get_global_state(machine);
            state.stack_reset();
            state.clear_free_list();
            state.Dispose();

            //throw new emu_unimplemented();
#if false
            lock_guard<std::mutex> guard(s_global_state_guard);
#endif
            s_global_states.erase(machine);
        }


        static global_state get_global_state(running_machine machine)
        {
            //throw new emu_unimplemented();
#if false
            std::lock_guard<std::mutex> guard(s_global_state_guard);
#endif
            var it = s_global_states.find(machine);
            return it;  //return (it != null) ? it : new global_state();
        }
    }
}
