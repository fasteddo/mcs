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
        public const uint32_t FLAG_LEFT_ARROW   = 1 << 0;
        public const uint32_t FLAG_RIGHT_ARROW  = 1 << 1;
        protected const uint32_t FLAG_INVERT    = 1 << 2;
        protected const uint32_t FLAG_DISABLE   = 1 << 4;
        const uint32_t FLAG_UI_HEADING          = 1 << 5;
        const uint32_t FLAG_COLOR_BOX           = 1 << 6;
        //}


        // flags to pass to set_process_flags
        //enum
        //{
        const uint32_t PROCESS_NOKEYS                = 1 << 0;
        const uint32_t PROCESS_LR_ALWAYS             = 1 << 1;
        protected const uint32_t PROCESS_LR_REPEAT   = 1 << 2;
        const uint32_t PROCESS_CUSTOM_NAV            = 1 << 3;
        protected const uint32_t PROCESS_CUSTOM_ONLY = 1 << 4;
        const uint32_t PROCESS_ONLYCHAR              = 1 << 5;
        protected const uint32_t PROCESS_NOINPUT     = 1 << 6;
        const uint32_t PROCESS_IGNOREPAUSE           = 1 << 7;
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


            mame_ui_manager m_ui;
            bitmap_argb32 m_bgrnd_bitmap;
            render_texture m_bgrnd_texture;

            menu m_stack;
            menu m_free;

            bool m_hide;


            public global_state(mame_ui_manager ui)
                : base(ui.machine())
            {
                m_ui = ui;
                m_bgrnd_bitmap = null;
                m_bgrnd_texture = null;  //, m_bgrnd_texture(nullptr, ui.machine().render())
                m_stack = null;
                m_free = null;
                m_hide = false;


                render_manager render = ui.machine().render();

                // create a texture for main menu background
                m_bgrnd_texture = render.texture_alloc(render_texture.hq_scale);  //m_bgrnd_texture.reset(render.texture_alloc(render_texture::hq_scale));
                if (ui.options().use_background_image() && (ui.machine().system() == ___empty.driver____empty))
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
                if (m_stack != null && m_stack.is_active())
                {
                    m_stack.m_active = false;
                    m_stack.menu_deactivated();
                }

                menu.m_parent = m_stack;
                m_stack = menu;
                m_stack.machine().ui_input().reset();
            }


            public void stack_pop()
            {
                if (m_stack != null)
                {
                    if (m_stack.is_one_shot())
                        m_hide = true;

                    if (m_stack.is_active())
                    {
                        m_stack.m_active = false;
                        m_stack.menu_deactivated();
                    }

                    m_stack.menu_dismissed();

                    menu menu = m_stack;
                    m_stack = menu.m_parent;
                    menu.m_parent = m_free;
                    m_free = menu;
                    m_free.Dispose();
                    m_ui.machine().ui_input().reset();
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
                {
                    var parent = reversed.m_parent;
                    reversed.Dispose();
                    reversed = parent;  //reversed = std::move(reversed->m_parent);
                }
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


            public void hide_menu() { m_hide = true; }


            public uint32_t ui_handler(render_container container, mame_ui_manager mui)
            {
                // if we have no menus stacked up, start with the main menu
                if (m_stack == null)
                    stack_push(new menu_main(m_ui, container));

                // ensure topmost menu is active - need a loop because it could push another menu
                while (m_stack != null && !m_stack.is_active())
                {
                    m_stack.m_active = true;
                    m_stack.menu_activated();
                }

                // update the menu state
                m_hide = false;
                if (m_stack != null)
                    m_stack.do_handle();

                // clear up anything pending being released
                clear_free_list();

                // if the menus are to be hidden, return a cancel here
                if (m_ui.is_menu_active() && (m_hide || m_stack == null))
                {
                    if (m_stack != null)
                    {
                        if (m_stack.is_one_shot())
                        {
                            stack_pop();
                        }
                        else if (m_stack.is_active())
                        {
                            m_stack.m_active = false;
                            m_stack.menu_deactivated();
                        }
                    }

                    return UI_HANDLER_CANCEL;
                }

                return 0;
            }
        }

        // this is to satisfy the std::any requirement that objects be copyable
        //class global_state_wrapper : public global_state


        // menu-related events
        protected class event_
        {
            public object itemref;  //void                *itemref;   // reference for the selected item or nullptr
            public menu_item item;      // selected item or nullptr
            public int iptkey;     // one of the IPT_* values from inptport.h
            public char32_t unichar;    // unicode character if iptkey == IPT_SPECIAL
            public render_bounds mouse;      // mouse position if iptkey == IPT_CUSTOM
        }


        protected int top_line;           // main box top line
        protected int m_visible_lines;    // main box visible lines
        protected int m_visible_items;    // number of visible items


        global_state m_global_state;  //global_state            &m_global_state;        // reference to global state for session
        mame_ui_manager m_ui;              // UI we are attached to
        render_container m_container;       // render_container we render to
        menu m_parent;  //std::unique_ptr<menu>   m_parent;               // pointer to parent menu in the stack

        std.vector<menu_item> m_items;                // array of items

        uint32_t m_process_flags;        // event processing options
        int m_selected;             // which item is selected
        int m_hover;                // which item is being hovered over
        bool m_special_main_menu;    // true if no real emulation running under the menu
        bool m_one_shot;             // true for menus outside the normal stack
        bool m_needs_prev_menu_item; // true to automatically create item to dismiss menu
        bool m_active;               // whether the menu is currently visible and topmost

        event_ m_event;                // the UI event that occurred

        float m_customtop;            // amount of extra height to add at the top
        float m_custombottom;         // amount of extra height to add at the bottom

        int m_resetpos;             // item index to select after repopulating
        object m_resetref;  //void                    *m_resetref;            // item reference value to select after repopulating

        bool m_mouse_hit;
        bool m_mouse_button;
        float m_mouse_x;
        float m_mouse_y;


        //-------------------------------------------------
        //  ui_menu - menu constructor
        //-------------------------------------------------
        protected menu(mame_ui_manager mui, render_container container)
        {
            m_global_state = get_global_state(mui);
            m_ui = mui;
            m_container = container;
            m_parent = null;
            m_items = new std.vector<menu_item>();
            m_process_flags = 0;
            m_selected = 0;
            m_hover = 1;
            m_special_main_menu = false;
            m_one_shot = false;
            m_needs_prev_menu_item = true;
            m_active = false;
            m_event = new event_();
            m_customtop = 0.0f;
            m_custombottom = 0.0f;
            m_resetpos = 0;
            m_resetref = null;
            m_mouse_hit = false;
            m_mouse_button = false;
            m_mouse_x = -1.0f;
            m_mouse_y = -1.0f;


            reset(reset_options.SELECT_FIRST);

            top_line = 0;
            m_visible_lines = 0;
            m_visible_items = 0;
        }

        ~menu()
        {
            assert(m_isDisposed);  // can remove
        }

        protected bool m_isDisposed = false;
        public virtual void Dispose()
        {
            m_global_state.Dispose();

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
            // allocate a new item and populate it
            menu_item pitem = new menu_item(type, ref_, flags);
            pitem.set_text(text);
            pitem.set_subtext(subtext);

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
            if ((m_resetpos == (int)index) || (m_resetref != null && (m_resetref == ref_)))
                m_selected = (int)index;
            if (m_resetpos == (int)m_items.size() - 1)
                m_selected = (int)m_items.size() - 1;
        }


        //-------------------------------------------------
        //  item_append - append a new item to the
        //  end of the menu
        //-------------------------------------------------
        protected void item_append(menu_item item) { item_append(item.text(), item.subtext(), item.flags(), item.ref_(), item.type()); }


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


        // reset the menus, clearing everything
        public static void stack_reset(mame_ui_manager ui) { get_global_state(ui).stack_reset(); }

        // push a new menu onto the stack
        //template <typename T, typename... Params>
        public static void stack_push(menu menu)  //static void stack_push(Params &&... args)
        {
            stack_push_internal(menu);  //stack_push(std::make_unique<T>(std::forward<Params>(args)...));
        }

        //template <typename T, typename... Params>
        public static void stack_push_special_main(menu menu)  //static void stack_push_special_main(Params &&... args)
        {
            //std::unique_ptr<menu> ptr(std::make_unique<T>(std::forward<Params>(args)...));
            menu.set_special_main_menu(true);
            stack_push(menu);
        }

        // pop a menu from the stack
        public static void stack_pop(mame_ui_manager ui) { get_global_state(ui).stack_pop(); }

        // test if one of the menus in the stack requires hide disable
        public static bool stack_has_special_main_menu(mame_ui_manager ui) { return get_global_state(ui).stack_has_special_main_menu(); }


        public static Func<render_container, mame_ui_manager, uint32_t> get_ui_handler(mame_ui_manager mui)  //static delegate<uint32_t (render_container &)> get_ui_handler(mame_ui_manager &mui);
        {
            global_state state = get_global_state(mui);
            return state.ui_handler;  //return delegate<uint32_t (render_container &)>(&global_state::ui_handler, &state);
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

            handle(process());
        }


        protected mame_ui_manager ui() { return m_ui; }
        protected running_machine machine() { return m_ui.machine(); }
        protected render_container container() { return m_container; }

        protected bool is_special_main_menu() { return m_special_main_menu; }
        bool is_one_shot() { return m_one_shot; }
        bool is_active() { return m_active; }

        protected void set_one_shot(bool oneshot) { m_one_shot = oneshot; }
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


        //template <typename T> T *topmost_menu() const { return m_global_state.topmost_menu<T>(); }
        //template <typename T> static T *topmost_menu(mame_ui_manager &ui) { return get_global_state(ui).topmost_menu<T>(); }

        protected void stack_pop() { m_global_state.stack_pop(); }
        protected void stack_reset() { m_global_state.stack_reset(); }
        protected bool stack_has_special_main_menu() { return m_global_state.stack_has_special_main_menu(); }


        protected menu_item item(int index) { return m_items[index]; }
        protected int item_count() { return (int)m_items.size(); }


        // retrieves the ref of the currently selected menu item or nullptr
        protected object get_selection_ref() { return selection_valid() ? m_items[m_selected].ref_() : null; }


        protected menu_item selected_item() { return m_items[m_selected]; }
        //menu_item const &selected_item() const { return m_items[m_selected]; }
        protected int selected_index() { return m_selected; }
        bool selection_valid() { return (0 <= m_selected) && ((int)m_items.size() > m_selected); }
        protected bool is_selected(int index) { return selection_valid() && (m_selected == index); }
        protected bool is_first_selected() { return 0 == m_selected; }
        protected bool is_last_selected() { return ((int)m_items.size() - 1) == m_selected; }


        // changes the index of the currently selected menu item
        protected void set_selection(object selected_itemref)  //void set_selection(void *selected_itemref);
        {
            m_selected = -1;
            for (int itemnum = 0; itemnum < (int)m_items.size(); itemnum++)
            {
                if (m_items[itemnum].ref_() == selected_itemref)
                {
                    m_selected = itemnum;
                    break;
                }
            }
        }

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
        //void set_top_line(int index) { top_line = (0 < index) ? (index - 1) : index; }
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
            float origwidth = origx2 - origx1 - (2.0f * lrborder);
            float maxwidth = origwidth;
            foreach (var it in iter)  //for (Iter it = begin; it != end; ++it)
            {
                string line = it;
                if (!line.empty())
                {
                    var layout = ui().create_layout(container(), 1.0f, justify, wrap);
                    layout.add_text(it, rgb_t.white(), rgb_t.black(), text_size);
                    maxwidth = std.max(layout.actual_width(), maxwidth);
                }
            }

            if (scale && (origwidth < maxwidth))
            {
                text_size *= origwidth / maxwidth;
                maxwidth = origwidth;
            }

            // draw containing box
            float boxleft = 0.5f - (maxwidth * 0.5f) - lrborder;
            float boxright = 0.5f + (maxwidth * 0.5f) + lrborder;
            ui().draw_outlined_box(container(), boxleft, y1, boxright, y2, bgcolor);

            // inset box and draw content
            float textleft = 0.5f - (maxwidth * 0.5f);
            y1 += ui().box_tb_border();
            foreach (var it in iter)  //for (Iter it = begin; it != end; ++it)
            {
                ui().draw_text_full(
                        container(), it,
                        textleft, y1, maxwidth, justify, wrap,
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


        // overridable event handling

        protected void set_process_flags(uint32_t flags) { m_process_flags = flags; }


        //-------------------------------------------------
        //  handle_events - generically handle
        //  input events for a menu
        //-------------------------------------------------
        protected virtual void handle_events(uint32_t flags, event_ ev)
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
                                if (is_special_main_menu())
                                    machine().schedule_exit();
                            }
                            stop = true;
                        }
                        break;

                    // caught scroll event
                    case ui_event.type.MOUSE_WHEEL:
                        if (!custom_mouse_scroll((0 < local_menu_event.zdelta) ? -local_menu_event.num_lines : local_menu_event.num_lines) && (flags & (PROCESS_ONLYCHAR | PROCESS_CUSTOM_NAV)) == 0)
                        {
                            if (local_menu_event.zdelta > 0)
                            {
                                if (is_first_selected())
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
                                if (is_last_selected())
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
            bool ignorepause = (flags & PROCESS_IGNOREPAUSE) != 0 || stack_has_special_main_menu();

            // bail if no items
            if (m_items.size() == 0)
                return;

            // if we hit select, return TRUE or pop the stack, depending on the item
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_SELECT, 0))
            {
                if (is_last_selected() && m_needs_prev_menu_item)
                {
                    iptkey = (int)ioport_type.IPT_UI_CANCEL;
                    stack_pop();
                    if (is_special_main_menu())
                        machine().schedule_exit();
                }
                return;
            }

            // UI configure hides the menus
            if ((flags & PROCESS_NOKEYS) == 0 && exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_CONFIGURE, 0) && !m_global_state.stack_has_special_main_menu())
            {
                if (is_one_shot())
                    stack_pop();
                else
                    m_global_state.hide_menu();
                return;
            }

            // bail out
            if ((flags & PROCESS_ONLYCHAR) != 0)
                return;

            // hitting cancel also pops the stack
            if (exclusive_input_pressed(ref iptkey, (int)ioport_type.IPT_UI_CANCEL, 0))
            {
                if (!custom_ui_cancel())
                {
                    stack_pop();
                    if (is_special_main_menu())
                        machine().schedule_exit();
                }
                return;
            }

            // validate the current selection
            validate_selection(1);

            // swallow left/right keys if they are not appropriate
            bool ignoreleft = (flags & PROCESS_LR_ALWAYS) == 0 && (selected_item().flags() & FLAG_LEFT_ARROW) == 0;
            bool ignoreright = (flags & PROCESS_LR_ALWAYS) == 0 && (selected_item().flags() & FLAG_RIGHT_ARROW) == 0;

            // accept left/right/prev/next keys as-is with repeat if appropriate
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
                for (int code = (int)ioport_type.IPT_UI_FIRST + 1; code < (int)ioport_type.IPT_UI_LAST; code++)
                {
                    switch (code)
                    {
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


        protected virtual bool custom_ui_cancel() { return false; }
        protected virtual bool custom_mouse_down() { return false; }
        protected virtual bool custom_mouse_scroll(int lines) { return false; }

        // event notifications
        protected virtual void menu_activated() { }
        protected virtual void menu_deactivated() { }
        protected virtual void menu_dismissed() { }


        protected static bool is_selectable(menu_item item)
        {
            return ((item.flags() & menu.FLAG_DISABLE) == 0 && (item.type() != menu_item_type.SEPARATOR));
        }


        // get arrows status
        //template <typename T>
        //static uint32_t get_arrow_flags(T min, T max, T actual)


        // process a menu, drawing it and returning any interesting events
        //-------------------------------------------------
        //  process - process a menu, drawing it
        //  and returning any interesting events
        //-------------------------------------------------
        protected event_ process()
        {
            // reset the event
            m_event.iptkey = (int)ioport_type.IPT_INVALID;

            // first make sure our selection is valid
            validate_selection(1);

            // if we're not running the emulation, draw parent menus in the background
            Func<menu, bool> draw_parent = null;
            draw_parent = (menu parent) =>  //[] (auto &self, menu *parent) -> bool
                    {
                        if (parent == null || !(parent.is_special_main_menu() || draw_parent(parent.m_parent)))  //if (!parent || !(parent->is_special_main_menu() || self(self, parent->m_parent.get())))
                            return false;
                        else
                            parent.draw(PROCESS_NOINPUT);

                        return true;
                    };

            if (draw_parent(m_parent))
                container().add_rect(0.0f, 0.0f, 1.0f, 1.0f, new rgb_t(114, 0, 0, 0), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));

            // draw the menu proper
            draw(m_process_flags);

            // process input
            if ((m_process_flags & (PROCESS_NOKEYS | PROCESS_NOINPUT)) == 0)
            {
                // read events
                handle_events(m_process_flags, m_event);

                // handle the keys if we don't already have an event
                if (m_event.iptkey == (int)ioport_type.IPT_INVALID)
                    handle_keys(m_process_flags, ref m_event.iptkey);
            }

            // update the selected item in the event
            if ((m_event.iptkey != (int)ioport_type.IPT_INVALID) && selection_valid())
            {
                m_event.itemref = get_selection_ref();
                m_event.item = m_items[m_selected];
                return m_event;
            }
            else
            {
                return null;
            }
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
            bool noinput = (flags & PROCESS_NOINPUT) != 0;
            float aspect = machine().render().ui_aspect(container());
            float line_height = ui().get_line_height();
            float lr_arrow_width = 0.4f * line_height * aspect;
            float ud_arrow_width = line_height * aspect;
            float gutter_width = lr_arrow_width * 1.3f;
            float lr_border = ui().box_lr_border() * aspect;

            if (is_special_main_menu())
                draw_background();

            // compute the width and height of the full menu
            float visible_width = 0;
            float visible_main_menu_height = 0;
            foreach (var pitem in m_items)
            {
                // compute width of left hand side
                float total_width = gutter_width + ui().get_string_width(pitem.text()) + gutter_width;

                // add in width of right hand side
                if (!pitem.subtext().empty())
                    total_width += 2.0f * gutter_width + ui().get_string_width(pitem.subtext());
                else if ((pitem.flags() & FLAG_UI_HEADING) != 0)
                    total_width += 4.0f * ud_arrow_width;

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
            visible_width = std.min(visible_width, 1.0f - ((lr_border + (aspect * UI_LINE_WIDTH)) * 2.0f));

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
                    string itemtext = pitem.text();
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
                    else if (pitem.type() == menu_item_type.SEPARATOR)
                    {
                        // if we're just a divider, draw a line
                        container().add_line(visible_left, line_y0 + 0.5f * line_height, visible_left + visible_width, line_y0 + 0.5f * line_height, UI_LINE_WIDTH, ui().colors().border_color(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
                    }
                    else if (pitem.subtext().empty())
                    {
                        // if we don't have a subitem, just draw the string centered
                        if ((pitem.flags() & FLAG_UI_HEADING) != 0)
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
                        bool subitem_invert = (pitem.flags() & FLAG_INVERT) != 0;
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

                        if ((pitem.flags() & FLAG_COLOR_BOX) != 0)
                        {
                            rgb_t color = new rgb_t((uint32_t)std.strtoul(pitem.subtext(), null, 16));

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
                            string subitem_text = pitem.subtext();

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
                            if (core_stricmp(pitem.subtext(), "On") == 0)
                                fgcolor2 = new rgb_t(0x00,0xff,0x00);

                            if (core_stricmp(pitem.subtext(), "Off") == 0)
                                fgcolor2 = new rgb_t(0xff,0x00,0x00);

                            if (core_stricmp(pitem.subtext(), "Auto") == 0)
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
                        if (is_selected(itemnum) && (pitem.flags() & FLAG_LEFT_ARROW) != 0)
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
                        if (is_selected(itemnum) && (pitem.flags() & FLAG_RIGHT_ARROW) != 0)
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
                bool subitem_invert = (pitem.flags() & FLAG_INVERT) != 0;
                var linenum = m_selected - top_line;
                float line_y = visible_top + (float)linenum * line_height;
                float target_width, target_height;

                // compute the multi-line target width/height
                ui().draw_text_full(
                        container(),
                        pitem.subtext(),
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
                        pitem.subtext(),
                        target_x, target_y, target_width,
                        text_layout.text_justify.RIGHT, text_layout.word_wrapping.WORD,
                        mame_ui_manager.draw_mode.NORMAL, ui().colors().selected_color(), ui().colors().selected_bg_color(),
                        out _, out _);
            }

            // if there is something special to add, do it by calling the virtual method
            custom_render(get_selection_ref(), m_customtop, m_custombottom, x1, y1, x2, y2);
        }


        //-------------------------------------------------
        //  set_special_main_menu - set whether the
        //  menu has special needs
        //-------------------------------------------------
        public void set_special_main_menu(bool special) { m_special_main_menu = special; }


        // to be implemented in derived classes
        protected abstract void populate(ref float customtop, ref float custombottom);


        // to be implemented in derived classes
        protected abstract void handle(event_ ev);


        // push a new menu onto the stack
        static void stack_push_internal(menu menu) { menu.m_global_state.stack_push(menu); }  //void stack_push(std::unique_ptr<menu> &&menu);


        //void extra_text_draw_box(float origx1, float origx2, float origy, float yspan, std::string_view text, int direction);


        bool first_item_visible() { return top_line <= 0; }
        bool last_item_visible() { return (top_line + m_visible_lines) >= (int)m_items.size(); }


        static global_state get_global_state(mame_ui_manager ui)
        {
            return ui.get_session_data(typeof(menu), () => { return new global_state(ui); });  //return ui.get_session_data<menu, global_state_wrapper>(ui);
        }
    }


    //template <typename Base = menu>
    abstract class autopause_menu : menu  //class autopause_menu : public Base
    {
        //using Base::Base;


        bool m_was_paused = false;
        bool m_unpaused = false;


        protected autopause_menu(mame_ui_manager mui, render_container container) : base(mui, container) { }


        protected override void menu_activated()
        {
            m_was_paused = this.machine().paused();
            if (m_was_paused)
                m_unpaused = false;
            else if (!m_unpaused)
                this.machine().pause();

            base.menu_activated();
        }

        protected override void menu_deactivated()
        {
            m_unpaused = !this.machine().paused();
            if (!m_was_paused && !m_unpaused)
                this.machine().resume();

            base.menu_deactivated();
        }
    }
}
