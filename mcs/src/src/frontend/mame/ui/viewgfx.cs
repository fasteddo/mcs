// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using gfx_interface_enumerator = mame.device_interface_enumerator<mame.device_gfx_interface>;  //typedef device_interface_enumerator<device_gfx_interface> gfx_interface_enumerator;
using int32_t = System.Int32;
using palette_interface_enumerator = mame.device_interface_enumerator<mame.device_palette_interface>;  //typedef device_interface_enumerator<device_palette_interface> palette_interface_enumerator;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using screen_device_enumerator = mame.device_type_enumerator<mame.screen_device>;  //typedef device_type_enumerator<screen_device> screen_device_enumerator;
using size_t = System.UInt64;
using u8 = System.Byte;
using u32 = System.UInt32;
using unsigned = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.cpp_global;
using static mame.digfx_global;
using static mame.emucore_global;
using static mame.language_global;
using static mame.render_global;
using static mame.rendertypes_global;
using static mame.rendutil_global;
using static mame.tilemap_global;
using static mame.ui_global;
using static mame.viewgfx_global;
using static mame.viewgfx_internal;


namespace mame
{
    public static class viewgfx_global
    {
        /***************************************************************************
            MAIN ENTRY POINT
        ***************************************************************************/

        //-------------------------------------------------
        //  ui_gfx_ui_handler - primary UI handler
        //
        //  NOTE: this must not be called before machine
        //  initialization is complete, as some drivers
        //  create or modify gfx sets in VIDEO_START
        //-------------------------------------------------
        public static uint32_t ui_gfx_ui_handler(render_container container, mame_ui_manager mui, bool uistate)
        {
            return mui.get_session_data(typeof(gfx_viewer), () => { return new gfx_viewer(mui.machine()); }).handle(mui, container, uistate);  //return mui.get_session_data<gfx_viewer, gfx_viewer>(mui.machine()).handle(mui, container, uistate);
        }
    }


    public static class viewgfx_internal
    {
        public class gfx_viewer
        {
            enum view
            {
                PALETTE = 0,
                GFXSET,
                TILEMAP
            }


            class palette
            {
                //enum subset
                //{
                //    PENS,
                //    INDIRECT
                //}


                device_palette_interface m_interface = null;
                unsigned m_count;
                unsigned m_index = 0U;
                //subset m_which = subset::PENS;
                //unsigned m_columns = 16U;
                //int m_offset = 0;


                public palette(running_machine machine)
                {
                    m_count = (unsigned)new palette_interface_enumerator(machine.root_device()).count();


                    if (m_count != 0)
                        set_device(machine);
                }


                public device_palette_interface interface_()
                {
                    return m_interface;
                }

                //bool indirect() const noexcept
                //{
                //    return subset::INDIRECT == m_which;
                //}

                //unsigned columns() const noexcept
                //{
                //    return m_columns;
                //}

                //unsigned index(unsigned x, unsigned y) const noexcept
                //{
                //    return m_offset + (y * m_columns) + x;
                //}

                //void handle_keys(running_machine &machine);


                void set_device(running_machine machine)
                {
                    m_interface = new palette_interface_enumerator(machine.root_device()).byindex((int)m_index);
                }


                //void next_group(running_machine &machine) noexcept
                //{
                //    if ((subset::PENS == m_which) && m_interface->indirect_entries())
                //    {
                //        m_which = subset::INDIRECT;
                //    }
                //    else if ((m_count - 1) > m_index)
                //    {
                //        ++m_index;
                //        set_device(machine);
                //        m_which = subset::PENS;
                //    }
                //}

                //void prev_group(running_machine &machine) noexcept
                //{
                //    if (subset::INDIRECT == m_which)
                //    {
                //        m_which = subset::PENS;
                //    }
                //    else if (0 < m_index)
                //    {
                //        --m_index;
                //        set_device(machine);
                //        m_which = m_interface->indirect_entries() ? subset::INDIRECT : subset::PENS;
                //    }
                //}
            }


            class gfxset
            {
                class setinfo
                {
                    public device_palette_interface m_palette = null;
                    //int m_offset = 0;
                    //unsigned m_color = 0;
                    public unsigned m_color_count = 0U;
                    public uint8_t m_rotate = 0;
                    //uint8_t m_columns = 16U;
                    //bool m_integer_scale = false;


                    //void next_color() noexcept
                    //{
                    //    if ((m_color_count - 1) > m_color)
                    //        ++m_color;
                    //    else
                    //        m_color = 0U;
                    //}

                    //void prev_color() noexcept
                    //{
                    //    if (m_color)
                    //        --m_color;
                    //    else
                    //        m_color = m_color_count - 1;
                    //}
                }


                class devinfo
                {
                    device_gfx_interface m_interface;
                    unsigned m_setcount;
                    setinfo [] m_sets = new setinfo[MAX_GFX_ELEMENTS];


                    public devinfo(device_gfx_interface interface_, device_palette_interface first_palette, u8 rotate)
                    {
                        m_interface = interface_;
                        m_setcount = 0U;


                        for (gfx_element gfx; (MAX_GFX_ELEMENTS > m_setcount) && ((gfx = interface_.gfx((int)m_setcount)) != null); ++m_setcount)
                        {
                            var set = m_sets[m_setcount];
                            if (gfx.has_palette())
                            {
                                set.m_palette = gfx.palette();
                                set.m_color_count = gfx.colors();
                            }
                            else
                            {
                                set.m_palette = first_palette;
                                set.m_color_count = first_palette.entries() / gfx.granularity();
                                if (set.m_color_count == 0)
                                    set.m_color_count = 1U;
                            }
                            set.m_rotate = rotate;
                        }
                    }


                    //device_gfx_interface &interface() const noexcept
                    //{
                    //    return *m_interface;
                    //}

                    //unsigned setcount() const noexcept
                    //{
                    //    return m_setcount;
                    //}

                    //setinfo const &set(unsigned index) const noexcept
                    //{
                    //    return m_sets[index];
                    //}

                    //setinfo &set(unsigned index) noexcept
                    //{
                    //    return m_sets[index];
                    //}
                }


                std.vector<devinfo> m_devices = new std.vector<devinfo>();
                //unsigned m_device = 0U;
                //unsigned m_set = 0U;


                public gfxset(running_machine machine)
                {
                    // get useful defaults
                    uint8_t rotate = (uint8_t)(machine.system().flags & machine_flags.type.MASK_ORIENTATION);
                    device_palette_interface first_palette = new palette_interface_enumerator(machine.root_device()).first();

                    // iterate over graphics decoders
                    foreach (device_gfx_interface interface_ in new gfx_interface_enumerator(machine.root_device()))
                    {
                        // if there are any exposed graphics sets, add the device
                        if (interface_.gfx(0) != null)
                            m_devices.emplace_back(new devinfo(interface_, first_palette, rotate));
                    }
                }


                public bool has_gfx()
                {
                    return !m_devices.empty();
                }


                //bool handle_keys(running_machine &machine, int xcells, int ycells);


                //bool next_group() noexcept
                //{
                //    if ((m_devices[m_device].setcount() - 1) > m_set)
                //    {
                //        ++m_set;
                //        return true;
                //    }
                //    else if ((m_devices.size() - 1) > m_device)
                //    {
                //        ++m_device;
                //        m_set = 0U;
                //        return true;
                //    }
                //    else
                //    {
                //        return false;
                //    }
                //}

                //bool prev_group() noexcept
                //{
                //    if (m_set)
                //    {
                //        --m_set;
                //        return true;
                //    }
                //    else if (m_device)
                //    {
                //        --m_device;
                //        m_set = m_devices[m_device].setcount() - 1;
                //        return true;
                //    }
                //    else
                //    {
                //        return false;
                //    }
                //}
            }


            class tilemap
            {
                //static constexpr int MAX_ZOOM_LEVEL = 8; // maximum tilemap zoom ratio screen:native
                //static constexpr int MIN_ZOOM_LEVEL = 8; // minimum tilemap zoom ratio native:screen


                class info
                {
                    //int m_xoffs = 0;
                    //int m_yoffs = 0;
                    //unsigned m_zoom = 1U;
                    //bool m_zoom_frac = false;
                    //bool m_auto_zoom = true;
                    public uint8_t m_rotate = 0;
                    //uint32_t m_flags = TILEMAP_DRAW_ALL_CATEGORIES;


                    //bool zoom_in(float pixelscale) noexcept
                    //{
                    //    if (m_auto_zoom)
                    //    {
                    //        // auto zoom never uses fractional factors
                    //        m_zoom = std::min<int>(std::lround(pixelscale) + 1, MAX_ZOOM_LEVEL);
                    //        m_zoom_frac = false;
                    //        m_auto_zoom = false;
                    //        return true;
                    //    }
                    //    else if (m_zoom_frac)
                    //    {
                    //        m_zoom--;
                    //        if (m_zoom == 1)
                    //            m_zoom_frac = false; // entering integer zoom range
                    //        return true;
                    //    }
                    //    else if (MAX_ZOOM_LEVEL > m_zoom)
                    //    {
                    //        m_zoom++; // remaining in integer zoom range
                    //        return true;
                    //    }
                    //    else
                    //    {
                    //        return false;
                    //    }
                    //}

                    //bool zoom_out(float pixelscale) noexcept
                    //{
                    //    if (m_auto_zoom)
                    //    {
                    //        // auto zoom never uses fractional factors
                    //        m_zoom = std::lround(pixelscale) - 1;
                    //        m_zoom_frac = !m_zoom;
                    //        if (m_zoom_frac)
                    //            m_zoom = 2;
                    //        m_auto_zoom = false;
                    //        return true;
                    //    }
                    //    else if (!m_zoom_frac)
                    //    {
                    //        if (m_zoom == 1)
                    //        {
                    //            m_zoom++;
                    //            m_zoom_frac = true; // entering fractional zoom range
                    //        }
                    //        else
                    //        {
                    //            m_zoom--; // remaining in integer zoom range
                    //        }
                    //        return true;
                    //    }
                    //    else if (MIN_ZOOM_LEVEL > m_zoom)
                    //    {
                    //        m_zoom++; // remaining in fractional zoom range
                    //        return true;
                    //    }
                    //    else
                    //    {
                    //        return false;
                    //    }
                    //}

                    //bool next_category() noexcept
                    //{
                    //    if (TILEMAP_DRAW_ALL_CATEGORIES == m_flags)
                    //    {
                    //        m_flags = 0U;
                    //        return true;
                    //    }
                    //    else if (TILEMAP_DRAW_CATEGORY_MASK > m_flags)
                    //    {
                    //        ++m_flags;
                    //        return true;
                    //    }
                    //    else
                    //    {
                    //        return false;
                    //    }
                    //}

                    //bool prev_catagory() noexcept
                    //{
                    //    if (!m_flags)
                    //    {
                    //        m_flags = TILEMAP_DRAW_ALL_CATEGORIES;
                    //        return true;
                    //    }
                    //    else if (TILEMAP_DRAW_ALL_CATEGORIES != m_flags)
                    //    {
                    //        --m_flags;
                    //        return true;
                    //    }
                    //    else
                    //    {
                    //        return false;
                    //    }
                    //}
                }


                std.vector<info> m_info = new std.vector<info>();
                //unsigned m_index = 0U;


                public tilemap(running_machine machine)
                {
                    uint8_t rotate = (uint8_t)(machine.system().flags & machine_flags.type.MASK_ORIENTATION);
                    m_info.resize((size_t)machine.tilemap().count());
                    foreach (var info in m_info)
                        info.m_rotate = rotate;
                }


                //unsigned index() const noexcept
                //{
                //    return m_index;
                //}

                //float zoom_scale() const noexcept
                //{
                //    auto const &info = m_info[m_index];
                //    return info.m_zoom_frac ? (1.0f / float(info.m_zoom)) : float(info.m_zoom);
                //}

                //bool auto_zoom() const noexcept
                //{
                //    return m_info[m_index].m_auto_zoom;
                //}

                //uint8_t rotate() const noexcept
                //{
                //    return m_info[m_index].m_rotate;
                //}

                //uint32_t flags() const noexcept
                //{
                //    return m_info[m_index].m_flags;
                //}

                //int xoffs() const noexcept
                //{
                //    return m_info[m_index].m_xoffs;
                //}

                //int yoffs() const noexcept
                //{
                //    return m_info[m_index].m_yoffs;
                //}

                //bool handle_keys(running_machine &machine, float pixelscale);


                //static int scroll_step(running_machine &machine)
                //{
                //    auto &input = machine.input();
                //    if (input.code_pressed(KEYCODE_LCONTROL) || input.code_pressed(KEYCODE_RCONTROL))
                //        return 64;
                //    else if (input.code_pressed(KEYCODE_LSHIFT) || input.code_pressed(KEYCODE_RSHIFT))
                //        return 1;
                //    else
                //        return 8;
                //}
            }


            running_machine m_machine;
            view m_mode = view.PALETTE;

            //bitmap_rgb32 m_bitmap;
            //render_texture *m_texture = nullptr;
            bool m_bitmap_dirty = false;

            palette m_palette;
            gfxset m_gfxset;
            tilemap m_tilemap;


            public gfx_viewer(running_machine machine)
            {
                m_machine = machine;
                m_palette = new palette(machine);
                m_gfxset = new gfxset(machine);
                m_tilemap = new tilemap(machine);
            }

            // copy constructor needed to make std::any happy
            gfx_viewer(gfx_viewer that)
                : this(that.m_machine)
            {
            }

            //~gfx_viewer()
            //{
            //    if (m_texture)
            //        m_machine.render().texture_free(m_texture);
            //}


            public uint32_t handle(mame_ui_manager mui, render_container container, bool uistate)
            {
                // implicitly cancel if there's nothing to display
                if (!is_relevant())
                    return cancel(uistate);

                // always mark the bitmap dirty if not paused
                if (!m_machine.paused())
                    m_bitmap_dirty = true;

                // try to display the selected view
                while (true)
                {
                    switch (m_mode)
                    {
                    case view.PALETTE:
                        if (m_palette.interface_() != null)
                            return handle_palette(mui, container, uistate);
                        m_mode = view.GFXSET;
                        break;

                    case view.GFXSET:
                        if (m_gfxset.has_gfx())
                            return handle_gfxset(mui, container, uistate);
                        m_mode = view.TILEMAP;
                        break;

                    case view.TILEMAP:
                        if (m_machine.tilemap().count() != 0)
                            return handle_tilemap(mui, container, uistate);
                        m_mode = view.PALETTE;
                        break;
                    }
                }
            }


            bool is_relevant()
            {
                return m_palette.interface_() != null || m_gfxset.has_gfx() || m_machine.tilemap().count() != 0;
            }

#if false
            uint32_t handle_general_keys(bool uistate)
            {
                auto &input = m_machine.ui_input();

                // UI select cycles through views
                if (input.pressed(IPT_UI_SELECT))
                {
                    m_mode = view((int(m_mode) + 1) % 3);
                    m_bitmap_dirty = true;
                }

                // pause does what you'd expect
                if (input.pressed(IPT_UI_PAUSE))
                {
                    if (m_machine.paused())
                        m_machine.resume();
                    else
                        m_machine.pause();
                }

                // cancel or graphics viewer dismisses the viewer
                if (input.pressed(IPT_UI_CANCEL) || input.pressed(IPT_UI_SHOW_GFX))
                    return cancel(uistate);

                return uistate;
            }
#endif

            uint32_t cancel(bool uistate)
            {
                if (!uistate)
                    m_machine.resume();
                m_bitmap_dirty = true;
                return UI_HANDLER_CANCEL;
            }


            uint32_t handle_palette(mame_ui_manager mui, render_container container, bool uistate) { throw new emu_unimplemented(); }

            uint32_t handle_gfxset(mame_ui_manager mui, render_container container, bool uistate) { throw new emu_unimplemented(); }

            uint32_t handle_tilemap(mame_ui_manager mui, render_container container, bool uistate) { throw new emu_unimplemented(); }

            //void update_gfxset_bitmap(int xcells, int ycells, gfx_element &gfx);
            //void update_tilemap_bitmap(int width, int height);

            //void gfxset_draw_item(gfx_element &gfx, int index, int dstx, int dsty, gfxset::setinfo const &info);

#if false
            void resize_bitmap(int32_t width, int32_t height)
            {
                if (!m_bitmap.valid() || !m_texture || (m_bitmap.width() != width) || (m_bitmap.height() != height))
                {
                    // free the old stuff
                    if (m_texture)
                        m_machine.render().texture_free(m_texture);

                    // allocate new stuff
                    m_bitmap.resize(width, height);
                    m_texture = m_machine.render().texture_alloc();
                    m_texture->set_bitmap(m_bitmap, m_bitmap.cliprect(), TEXFORMAT_ARGB32);

                    // force a redraw
                    m_bitmap_dirty = true;
                }
            }

            bool map_mouse(render_container &container, render_bounds const &clip, float &x, float &y) const
            {
                int32_t target_x, target_y;
                bool button;
                render_target *const target = m_machine.ui_input().find_mouse(&target_x, &target_y, &button);
                if (!target)
                    return false;
                else if (!target->map_point_container(target_x, target_y, container, x, y))
                    return false;
                else
                    return clip.includes(x, y);
            }
#endif
        }
    }
}
