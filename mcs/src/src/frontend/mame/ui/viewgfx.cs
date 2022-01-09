// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using gfx_interface_enumerator = mame.device_interface_enumerator<mame.device_gfx_interface>;  //typedef device_interface_enumerator<device_gfx_interface> gfx_interface_enumerator;
using int32_t = System.Int32;
using palette_interface_enumerator = mame.device_interface_enumerator<mame.device_palette_interface>;  //typedef device_interface_enumerator<device_palette_interface> palette_interface_enumerator;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using screen_device_enumerator = mame.device_type_enumerator<mame.screen_device>;  //typedef device_type_enumerator<screen_device> screen_device_enumerator;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.cpp_global;
using static mame.digfx_global;
using static mame.emucore_global;
using static mame.input_global;
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
        // initialization
        //-------------------------------------------------
        //  ui_gfx_init - initialize the graphics viewer
        //-------------------------------------------------
        public static void ui_gfx_init(running_machine machine)
        {
            ui_gfx_state state = ui_gfx;
            uint8_t rotate = (uint8_t)(machine.system().flags & machine_flags.type.MASK_ORIENTATION);

            // make sure we clean up after ourselves
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_EXIT, ui_gfx_exit);

            // initialize our global state
            state.started = false;
            state.mode = 0;
            state.bitmap_dirty = false;
            state.bitmap.reset();
            state.texture = null;
            state.gfxset.devcount = 0;
            state.gfxset.devindex = 0;
            state.gfxset.set = 0;

            // set up the palette state
            state.palette.interface_ = null;
            state.palette.devcount = 0;
            state.palette.devindex = 0;
            state.palette.which = 0;
            state.palette.columns = 16;

            // set up the graphics state
            for (uint8_t i = 0; i < MAX_GFX_DECODERS; i++)
            {
                state.gfxdev[i].interface_ = null;
                state.gfxdev[i].setcount = 0;
                for (uint8_t j = 0; j < MAX_GFX_ELEMENTS; j++)
                {
                    state.gfxdev[i].rotate[j] = rotate;
                    state.gfxdev[i].columns[j] = 16;
                    state.gfxdev[i].offset[j] = 0;
                    state.gfxdev[i].color[j] = 0;
                    state.gfxdev[i].palette[j] = null;
                    state.gfxdev[i].color_count[j] = 0;
                }
            }

            // set up the tilemap state
            state.tilemap.which = 0;
            state.tilemap.xoffs = 0;
            state.tilemap.yoffs = 0;
            state.tilemap.zoom = 1;
            state.tilemap.zoom_frac = false;
            state.tilemap.auto_zoom = true;
            state.tilemap.rotate = rotate;
            state.tilemap.flags = TILEMAP_DRAW_ALL_CATEGORIES;
        }


        // returns 'true' if the internal graphics viewer has relevance
        //-------------------------------------------------
        //  ui_gfx_is_relevant - returns 'true' if the
        //  internal graphics viewer has relevance
        //
        //  NOTE: this must not be called before machine
        //  initialization is complete, as some drivers
        //  create or modify gfx sets in VIDEO_START
        //-------------------------------------------------
        static bool ui_gfx_is_relevant(running_machine machine)
        {
            ui_gfx_state state = ui_gfx;

            if (!state.started)
                ui_gfx_count_devices(machine, state);

            return state.palette.devcount > 0
                || state.gfxset.devcount > 0
                || machine.tilemap().count() > 0;
        }


        // master handler

        public static uint32_t ui_gfx_ui_handler(render_container container, mame_ui_manager mui) { return ui_gfx_ui_handler(container, mui, false); }  // added default for set_handler() in handler_ingame()

        //-------------------------------------------------
        //  ui_gfx_ui_handler - primary UI handler
        //-------------------------------------------------
        public static uint32_t ui_gfx_ui_handler(render_container container, mame_ui_manager mui, bool uistate)
        {
            ui_gfx_state state = ui_gfx;

            // if we have nothing, implicitly cancel
            if (!ui_gfx_is_relevant(mui.machine()))
                goto cancel;

            // if we're not paused, mark the bitmap dirty
            if (!mui.machine().paused())
                state.bitmap_dirty = true;

            // switch off the state to display something
again:
            switch (state.mode)
            {
                case (uint8_t)ui_gfx_modes.UI_GFX_PALETTE:
                    // if we have a palette, display it
                    if (state.palette.devcount > 0)
                    {
                        palette_handler(mui, container, state);
                        break;
                    }

                    state.mode++;
                    goto case (uint8_t)ui_gfx_modes.UI_GFX_GFXSET;  //[[fallthrough]];

                case (uint8_t)ui_gfx_modes.UI_GFX_GFXSET:
                    // if we have graphics sets, display them
                    if (state.gfxset.devcount > 0)
                    {
                        gfxset_handler(mui, container, state);
                        break;
                    }

                    state.mode++;
                    goto case (uint8_t)ui_gfx_modes.UI_GFX_TILEMAP;  //[[fallthrough]];

                case (uint8_t)ui_gfx_modes.UI_GFX_TILEMAP:
                    // if we have tilemaps, display them
                    if (mui.machine().tilemap().count() > 0)
                    {
                        tilemap_handler(mui, container, state);
                        break;
                    }

                    state.mode = (uint8_t)ui_gfx_modes.UI_GFX_PALETTE;
                    goto again;
            }

            // handle keys
            if (mui.machine().ui_input().pressed((int)ioport_type.IPT_UI_SELECT))
            {
                state.mode = (uint8_t)((state.mode + 1) % 3);
                state.bitmap_dirty = true;
            }

            if (mui.machine().ui_input().pressed((int)ioport_type.IPT_UI_PAUSE))
            {
                if (mui.machine().paused())
                    mui.machine().resume();
                else
                    mui.machine().pause();
            }

            if (mui.machine().ui_input().pressed((int)ioport_type.IPT_UI_CANCEL) || mui.machine().ui_input().pressed((int)ioport_type.IPT_UI_SHOW_GFX))
                goto cancel;

            return uistate ? 1U : 0;

cancel:
            if (!uistate)
                mui.machine().resume();

            state.bitmap_dirty = true;

            return UI_HANDLER_CANCEL;
        }
    }


    public static class viewgfx_internal
    {
        public enum ui_gfx_modes
        {
            UI_GFX_PALETTE = 0,
            UI_GFX_GFXSET,
            UI_GFX_TILEMAP
        }


        public const int MAX_GFX_DECODERS = 8;
        const int MAX_ZOOM_LEVEL = 8;
        const int MIN_ZOOM_LEVEL = 8;


        // information about a single gfx device
        public class ui_gfx_info
        {
            public device_gfx_interface interface_;    // pointer to device's gfx interface
            public uint8_t setcount;                     // how many gfx sets device has
            public uint8_t [] rotate = new uint8_t[MAX_GFX_ELEMENTS];     // current rotation (orientation) value
            public uint8_t [] columns = new uint8_t[MAX_GFX_ELEMENTS];    // number of items per row
            public int [] offset = new int[MAX_GFX_ELEMENTS];     // current offset of top,left item
            public int [] color = new int[MAX_GFX_ELEMENTS];      // current color selected
            public device_palette_interface [] palette = new device_palette_interface[MAX_GFX_ELEMENTS]; // associated palette (maybe multiple choice one day?)
            public int [] color_count = new int[MAX_GFX_ELEMENTS]; // Range of color values
        }


        public class ui_gfx_state
        {
            public bool started;        // have we called ui_gfx_count_devices() yet?
            public uint8_t mode;           // which mode are we in?

            // intermediate bitmaps
            public bool bitmap_dirty;   // is the bitmap dirty?
            public bitmap_rgb32 bitmap = new bitmap_rgb32();         // bitmap for drawing gfx and tilemaps
            public render_texture texture;        // texture for rendering the above bitmap

            // palette-specific data
            public class palette_class
            {
                public device_palette_interface interface_;     // pointer to current device
                public int devcount;             // how many palette devices exist
                public int devindex;             // which palette device is visible
                public uint8_t which;                // which subset (pens or indirect colors)?
                public uint8_t columns;              // number of items per row
                public int offset;               // current offset of top left item
            }
            public palette_class palette = new palette_class();

            // graphics-specific data
            public class gfxset_class
            {
                public uint8_t devcount;   // how many gfx devices exist
                public uint8_t devindex;   // which device is visible
                public uint8_t set;        // which set is visible
            }
            public gfxset_class gfxset = new gfxset_class();

            // information about each gfx device
            public ui_gfx_info [] gfxdev = new ui_gfx_info[MAX_GFX_DECODERS];

            // tilemap-specific data
            public class tilemap_class
            {
                public int which;                // which tilemap are we viewing?
                public int xoffs;                // current X offset
                public int yoffs;                // current Y offset
                public int zoom;                 // zoom factor
                public bool zoom_frac;      // zoom via reciprocal fractions
                public bool auto_zoom;      // auto-zoom toggle
                public uint8_t rotate;              // current rotation (orientation) value
                public uint32_t flags;                    // render flags
            }
            public tilemap_class tilemap = new tilemap_class();

            public ui_gfx_state()
            {
                for (int i = 0; i < MAX_GFX_DECODERS; i++)
                    gfxdev[i] = new ui_gfx_info();
            }
        }


        /***************************************************************************
            GLOBAL VARIABLES
        ***************************************************************************/
        public static ui_gfx_state ui_gfx = new ui_gfx_state();


        //-------------------------------------------------
        //  ui_gfx_count_devices - count the palettes,
        //  gfx decoders and gfx sets in the machine
        //-------------------------------------------------
        public static void ui_gfx_count_devices(running_machine machine, ui_gfx_state state)
        {
            // count the palette devices
            state.palette.devcount = new palette_interface_enumerator(machine.root_device()).count();

            // set the pointer to the first palette
            if (state.palette.devcount > 0)
                palette_set_device(machine, state);

            // count the gfx devices
            state.gfxset.devcount = 0;
            foreach (device_gfx_interface interface_ in new gfx_interface_enumerator(machine.root_device()))
            {
                // count the gfx sets in each device, skipping devices with none
                uint8_t count = 0;
                while (count < MAX_GFX_ELEMENTS && interface_.gfx(count) != null)
                    count++;

                // count = index of first nullptr
                if (count > 0)
                {
                    state.gfxdev[state.gfxset.devcount].interface_ = interface_;
                    state.gfxdev[state.gfxset.devcount].setcount = count;
                    for (uint8_t slot = 0; slot != count; slot++)
                    {
                        var gfx = interface_.gfx(slot);
                        if (gfx.has_palette())
                        {
                            state.gfxdev[state.gfxset.devcount].palette[slot] = gfx.palette();
                            state.gfxdev[state.gfxset.devcount].color_count[slot] = (int)gfx.colors();
                        }
                        else
                        {
                            state.gfxdev[state.gfxset.devcount].palette[slot] = state.palette.interface_;
                            state.gfxdev[state.gfxset.devcount].color_count[slot] = (int)(state.palette.interface_.entries() / gfx.granularity());
                            if (state.gfxdev[state.gfxset.devcount].color_count[slot] == 0)
                                state.gfxdev[state.gfxset.devcount].color_count[slot] = 1;
                        }
                    }

                    if (++state.gfxset.devcount == MAX_GFX_DECODERS)
                        break;
                }
            }

            state.started = true;
        }


        //-------------------------------------------------
        //  ui_gfx_exit - clean up after ourselves
        //-------------------------------------------------
        public static void ui_gfx_exit(running_machine machine)
        {
            // free the texture
            machine.render().texture_free(ui_gfx.texture);
            ui_gfx.texture = null;

            // free the bitmap
            ui_gfx.bitmap.reset();
        }


        // palette handling
        /***************************************************************************
            PALETTE VIEWER
        ***************************************************************************/

        //-------------------------------------------------
        //  palette_set_device - set the pointer to the
        //  current palette device
        //-------------------------------------------------
        static void palette_set_device(running_machine machine, ui_gfx_state state)
        {
            palette_interface_enumerator pal_iter = new palette_interface_enumerator(machine.root_device());
            state.palette.interface_ = pal_iter.byindex(state.palette.devindex);
        }


        //-------------------------------------------------
        //  palette_handler - handler for the palette
        //  viewer
        //-------------------------------------------------
        public static void palette_handler(mame_ui_manager mui, render_container container, ui_gfx_state state)
        {
            device_palette_interface palette = state.palette.interface_;
            palette_device paldev = (palette_device)palette.device();

            int total = state.palette.which != 0 ? (int)palette.indirect_entries() : (int)palette.entries();
            MemoryContainer<rgb_t> raw_color = palette.palette().entry_list_raw();  //const rgb_t *raw_color = palette->palette()->entry_list_raw();
            render_font ui_font = mui.get_font();
            float titlewidth;
            float x0;
            float y0;
            render_bounds cellboxbounds;
            render_bounds boxbounds = new render_bounds();
            int skip;

            // add a half character padding for the box
            float aspect = mui.machine().render().ui_aspect(container);
            float chheight = mui.get_line_height();
            float chwidth = ui_font.char_width(chheight, aspect, '0');
            boxbounds.x0 = 0.0f + 0.5f * chwidth;
            boxbounds.x1 = 1.0f - 0.5f * chwidth;
            boxbounds.y0 = 0.0f + 0.5f * chheight;
            boxbounds.y1 = 1.0f - 0.5f * chheight;

            // the character cell box bounds starts a half character in from the box
            cellboxbounds = boxbounds;
            cellboxbounds.x0 += 0.5f * chwidth;
            cellboxbounds.x1 -= 0.5f * chwidth;
            cellboxbounds.y0 += 0.5f * chheight;
            cellboxbounds.y1 -= 0.5f * chheight;

            // add space on the left for 5 characters of text, plus a half character of padding
            cellboxbounds.x0 += 5.5f * chwidth;

            // add space on the top for a title, a half line of padding, a header, and another half line
            cellboxbounds.y0 += 3.0f * chheight;

            // compute the cell size
            float cellwidth = (cellboxbounds.x1 - cellboxbounds.x0) / (float)state.palette.columns;
            float cellheight = (cellboxbounds.y1 - cellboxbounds.y0) / (float)state.palette.columns;

            // figure out the title
            string title_buf = "";
            util.stream_format(ref title_buf, "'{0}'", palette.device().tag());
            if (palette.indirect_entries() > 0)
                title_buf += state.palette.which != 0 ? " COLORS" : " PENS";

            // if the mouse pointer is over one of our cells, add some info about the corresponding palette entry
            int mouse_target_x;
            int mouse_target_y;
            float mouse_x;
            float mouse_y;
            bool mouse_button;
            render_target mouse_target = mui.machine().ui_input().find_mouse(out mouse_target_x, out mouse_target_y, out mouse_button);
            if (mouse_target != null && mouse_target.map_point_container(mouse_target_x, mouse_target_y, container, out mouse_x, out mouse_y)
                && cellboxbounds.x0 <= mouse_x && cellboxbounds.x1 > mouse_x
                && cellboxbounds.y0 <= mouse_y && cellboxbounds.y1 > mouse_y)
            {
                int index = state.palette.offset + (int)((mouse_x - cellboxbounds.x0) / cellwidth) + (int)((mouse_y - cellboxbounds.y0) / cellheight) * state.palette.columns;
                if (index < total)
                {
                    util.stream_format(ref title_buf, " #{0}", index);  // #%X
                    if (palette.indirect_entries() > 0 && state.palette.which == 0)
                        util.stream_format(ref title_buf, " => {0}", palette.pen_indirect(index));  // %X
                    else if (paldev != null && paldev.basemem().base_() != null)
                        util.stream_format(ref title_buf, " = {0}", paldev.read_entry((pen_t)index));  // %X

                    rgb_t col = state.palette.which != 0 ? palette.indirect_color(index) : raw_color[index];
                    util.stream_format(ref title_buf, " (A:{0} R:{1} G:{2} B:{3})", col.a(), col.r(), col.g(), col.b());  //  (A:%X R:%X G:%X B:%X)
                }
            }

            // expand the outer box to fit the title
            string title = title_buf;
            titlewidth = ui_font.string_width(chheight, aspect, title);
            x0 = 0.0f;
            if (boxbounds.x1 - boxbounds.x0 < titlewidth + chwidth)
                x0 = boxbounds.x0 - (0.5f - 0.5f * (titlewidth + chwidth));

            // go ahead and draw the outer box now
            mui.draw_outlined_box(container, boxbounds.x0 - x0, boxbounds.y0, boxbounds.x1 + x0, boxbounds.y1, mui.colors().gfxviewer_bg_color());

            // draw the title
            x0 = 0.5f - 0.5f * titlewidth;
            y0 = boxbounds.y0 + 0.5f * chheight;
            foreach (var ch in title)
            {
                container.add_char(x0, y0, chheight, aspect, rgb_t.white(), ui_font, ch);
                x0 += ui_font.char_width(chheight, aspect, ch);
            }

            // draw the top column headers
            skip = (int)(chwidth / cellwidth);
            for (int x = 0; x < state.palette.columns; x += 1 + skip)
            {
                x0 = boxbounds.x0 + 6.0f * chwidth + (float)x * cellwidth;
                y0 = boxbounds.y0 + 2.0f * chheight;
                container.add_char(x0 + 0.5f * (cellwidth - chwidth), y0, chheight, aspect, rgb_t.white(), ui_font, "0123456789ABCDEF"[x & 0xf]);

                // if we're skipping, draw a point between the character and the box to indicate which
                // one it's referring to
                if (skip != 0)
                    container.add_point(x0 + 0.5f * cellwidth, 0.5f * (y0 + chheight + cellboxbounds.y0), UI_LINE_WIDTH, rgb_t.white(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
            }

            // draw the side column headers
            skip = (int)(chheight / cellheight);
            for (int y = 0; y < state.palette.columns; y += 1 + skip)
            {
                // only display if there is data to show
                if (state.palette.offset + y * state.palette.columns < total)
                {
                    string buffer;

                    // if we're skipping, draw a point between the character and the box to indicate which
                    // one it's referring to
                    x0 = boxbounds.x0 + 5.5f * chwidth;
                    y0 = boxbounds.y0 + 3.5f * chheight + (float)y * cellheight;
                    if (skip != 0)
                        container.add_point(0.5f * (x0 + cellboxbounds.x0), y0 + 0.5f * cellheight, UI_LINE_WIDTH, rgb_t.white(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));

                    // draw the row header
                    sprintf(out buffer, "{0:X5}", state.palette.offset + y * state.palette.columns);
                    for (int x = 4; x >= 0; x--)
                    {
                        x0 -= ui_font.char_width(chheight, aspect, buffer[x]);
                        container.add_char(x0, y0 + 0.5f * (cellheight - chheight), chheight, aspect, rgb_t.white(), ui_font, buffer[x]);
                    }
                }
            }

            // now add the rectangles for the colors
            for (int y = 0; y < state.palette.columns; y++)
            {
                for (int x = 0; x < state.palette.columns; x++)
                {
                    int index = state.palette.offset + y * state.palette.columns + x;
                    if (index < total)
                    {
                        pen_t pen = state.palette.which != 0 ? palette.indirect_color(index) : raw_color[index];
                        container.add_rect(cellboxbounds.x0 + x * cellwidth, cellboxbounds.y0 + y * cellheight,
                                           cellboxbounds.x0 + (x + 1) * cellwidth, cellboxbounds.y0 + (y + 1) * cellheight,
                                           new rgb_t(0xff000000 | pen), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
                    }
                }
            }

            // handle keys
            palette_handle_keys(mui.machine(), state);
        }


        //-------------------------------------------------
        //  palette_handle_keys - handle key inputs for
        //  the palette viewer
        //-------------------------------------------------
        static void palette_handle_keys(running_machine machine, ui_gfx_state state)
        {
            device_palette_interface palette = state.palette.interface_;

            // handle zoom (minus,plus)
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_ZOOM_OUT))
                state.palette.columns = std.min((uint8_t)(state.palette.columns * 2), (uint8_t)64);
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_ZOOM_IN))
                state.palette.columns = std.max((uint8_t)(state.palette.columns / 2), (uint8_t)4);
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_ZOOM_DEFAULT))
                state.palette.columns = 16;

            // handle colormap selection (open bracket,close bracket)
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_PREV_GROUP))
            {
                if (state.palette.which != 0)
                {
                    state.palette.which = 0;
                }
                else if (state.palette.devindex > 0)
                {
                    state.palette.devindex--;
                    palette_set_device(machine, state);
                    palette = state.palette.interface_;
                    state.palette.which = (palette.indirect_entries() > 0) ? (uint8_t)1 : (uint8_t)0;
                }
            }

            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_NEXT_GROUP))
            {
                if (state.palette.which == 0 && palette.indirect_entries() > 0)
                {
                    state.palette.which = 1;
                }
                else if (state.palette.devindex < state.palette.devcount - 1)
                {
                    state.palette.devindex++;
                    palette_set_device(machine, state);
                    palette = state.palette.interface_;
                    state.palette.which = 0;
                }
            }

            // cache some info in locals
            int total = state.palette.which != 0 ? (int)palette.indirect_entries() : (int)palette.entries();

            // determine number of entries per row and total
            int rowcount = state.palette.columns;
            int screencount = rowcount * rowcount;

            // handle keyboard navigation
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_UP, 4))
                state.palette.offset -= rowcount;
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_DOWN, 4))
                state.palette.offset += rowcount;
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_PAGE_UP, 6))
                state.palette.offset -= screencount;
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_PAGE_DOWN, 6))
                state.palette.offset += screencount;
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_HOME, 4))
                state.palette.offset = 0;
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_END, 4))
                state.palette.offset = total;

            // clamp within range
            if (state.palette.offset + screencount > ((total + rowcount - 1) / rowcount) * rowcount)
                state.palette.offset = ((total + rowcount - 1) / rowcount) * rowcount - screencount;
            if (state.palette.offset < 0)
                state.palette.offset = 0;
        }


        // graphics set handling
        /***************************************************************************
            GRAPHICS VIEWER
        ***************************************************************************/

        //-------------------------------------------------
        //  gfxset_handler - handler for the graphics
        //  viewer
        //-------------------------------------------------
        public static void gfxset_handler(mame_ui_manager mui, render_container container, ui_gfx_state state)
        {
            render_font ui_font = mui.get_font();
            int dev = state.gfxset.devindex;
            int set = state.gfxset.set;
            ui_gfx_info info = state.gfxdev[dev];
            device_gfx_interface gfxinterface = info.interface_;
            gfx_element gfx = gfxinterface.gfx(set);
            float x0;
            float y0;
            render_bounds cellboxbounds;
            render_bounds boxbounds = new render_bounds();
            float cellboxwidth;
            float cellboxheight;
            int targwidth = mui.machine().render().ui_target().width();
            int targheight = mui.machine().render().ui_target().height();
            int cellxpix;
            int cellypix;
            int xcells;
            float pixelscale = 0.0f;
            int skip;

            // add a half character padding for the box
            float aspect = mui.machine().render().ui_aspect(container);
            float chheight = mui.get_line_height();
            float chwidth = ui_font.char_width(chheight, aspect, '0');
            boxbounds.x0 = 0.0f + 0.5f * chwidth;
            boxbounds.x1 = 1.0f - 0.5f * chwidth;
            boxbounds.y0 = 0.0f + 0.5f * chheight;
            boxbounds.y1 = 1.0f - 0.5f * chheight;

            // the character cell box bounds starts a half character in from the box
            cellboxbounds = boxbounds;
            cellboxbounds.x0 += 0.5f * chwidth;
            cellboxbounds.x1 -= 0.5f * chwidth;
            cellboxbounds.y0 += 0.5f * chheight;
            cellboxbounds.y1 -= 0.5f * chheight;

            // add space on the left for 5 characters of text, plus a half character of padding
            cellboxbounds.x0 += 5.5f * chwidth;

            // add space on the top for a title, a half line of padding, a header, and another half line
            cellboxbounds.y0 += 3.0f * chheight;

            // convert back to pixels
            cellboxwidth = (int)((cellboxbounds.x1 - cellboxbounds.x0) * (float)targwidth);
            cellboxheight = (int)((cellboxbounds.y1 - cellboxbounds.y0) * (float)targheight);

            // compute the number of source pixels in a cell
            cellxpix = 1 + ((info.rotate[set] & ORIENTATION_SWAP_XY) != 0 ? gfx.height() : gfx.width());
            cellypix = 1 + ((info.rotate[set] & ORIENTATION_SWAP_XY) != 0 ? gfx.width() : gfx.height());

            // compute the largest pixel scale factor that still fits
            xcells = info.columns[set];
            while (xcells > 1)
            {
                pixelscale = (cellboxwidth / xcells) / cellxpix;
                if (pixelscale != 0)
                    break;
                xcells--;
            }
            info.columns[set] = (uint8_t)xcells;

            if (pixelscale <= 0.0f)
                pixelscale = 1.0f;

            // in the Y direction, we just display as many as we can
            int ycells = (int)(cellboxheight / (pixelscale * cellypix));

            // now determine the actual cellbox size
            cellboxwidth = std.min(cellboxwidth, xcells * pixelscale * cellxpix);
            cellboxheight = std.min(cellboxheight, ycells * pixelscale * cellypix);

            // compute the size of a single cell at this pixel scale factor
            float cellwidth = (cellboxwidth / xcells) / targwidth;
            float cellheight = (cellboxheight / ycells) / targheight;

            // working from the new width/height, recompute the boxbounds
            float fullwidth = cellboxwidth / targwidth + 6.5f * chwidth;
            float fullheight = cellboxheight / targheight + 4.0f * chheight;

            // recompute boxbounds from this
            boxbounds.x0 = (1.0f - fullwidth) * 0.5f;
            boxbounds.x1 = boxbounds.x0 + fullwidth;
            boxbounds.y0 = (1.0f - fullheight) * 0.5f;
            boxbounds.y1 = boxbounds.y0 + fullheight;

            // recompute cellboxbounds
            cellboxbounds.x0 = boxbounds.x0 + 6.0f * chwidth;
            cellboxbounds.x1 = cellboxbounds.x0 + cellboxwidth / (float)targwidth;
            cellboxbounds.y0 = boxbounds.y0 + 3.5f * chheight;
            cellboxbounds.y1 = cellboxbounds.y0 + cellboxheight / (float)targheight;

            // figure out the title
            string title_buf = "";
            util.stream_format(ref title_buf, "'{0}' {1}/{2}", gfxinterface.device().tag(), set, info.setcount - 1);

            // if the mouse pointer is over a pixel in a tile, add some info about the tile and pixel
            bool found_pixel = false;
            int mouse_target_x;
            int mouse_target_y;
            float mouse_x, mouse_y;
            bool mouse_button;
            render_target mouse_target = mui.machine().ui_input().find_mouse(out mouse_target_x, out mouse_target_y, out mouse_button);
            if (mouse_target != null && mouse_target.map_point_container(mouse_target_x, mouse_target_y, container, out mouse_x, out mouse_y)
                && cellboxbounds.x0 <= mouse_x && cellboxbounds.x1 > mouse_x
                && cellboxbounds.y0 <= mouse_y && cellboxbounds.y1 > mouse_y)
            {
                int code = info.offset[set] + (int)((mouse_x - cellboxbounds.x0) / cellwidth) + (int)((mouse_y - cellboxbounds.y0) / cellheight) * xcells;
                int xpixel = (int)((mouse_x - cellboxbounds.x0) / (cellwidth / cellxpix)) % cellxpix;
                int ypixel = (int)((mouse_y - cellboxbounds.y0) / (cellheight / cellypix)) % cellypix;
                if (code < gfx.elements() && xpixel < (cellxpix - 1) && ypixel < (cellypix - 1))
                {
                    found_pixel = true;
                    if ((info.rotate[set] & ORIENTATION_FLIP_X) != 0)
                        xpixel = (cellxpix - 2) - xpixel;
                    if ((info.rotate[set] & ORIENTATION_FLIP_Y) != 0)
                        ypixel = (cellypix - 2) - ypixel;
                    if ((info.rotate[set] & ORIENTATION_SWAP_XY) != 0)
                        std.swap(ref xpixel, ref ypixel);
                    uint8_t pixdata = gfx.get_data((u32)code)[xpixel + ypixel * (int)gfx.rowbytes()];
                    util.stream_format(ref title_buf, " #{0}:{1} @ {2},{3} = {4}",  // #%X:%X @ %d,%d = %X",
                                        code, info.color[set], xpixel, ypixel,
                                        gfx.colorbase() + info.color[set] * gfx.granularity() + pixdata);
                }
            }
            if (!found_pixel)
            {
                util.stream_format(ref title_buf, " {0}x{1} COLOR {2}/{3}", gfx.width(), gfx.height(), info.color[set], info.color_count[set]);  // %dx%d COLOR %X/%X
            }

            // expand the outer box to fit the title
            string title = title_buf;
            float titlewidth = ui_font.string_width(chheight, aspect, title);
            x0 = 0.0f;

            if (boxbounds.x1 - boxbounds.x0 < titlewidth + chwidth)
                x0 = boxbounds.x0 - (0.5f - 0.5f * (titlewidth + chwidth));

            // go ahead and draw the outer box now
            mui.draw_outlined_box(container, boxbounds.x0 - x0, boxbounds.y0, boxbounds.x1 + x0, boxbounds.y1, mui.colors().gfxviewer_bg_color());

            // draw the title
            x0 = 0.5f - 0.5f * titlewidth;
            y0 = boxbounds.y0 + 0.5f * chheight;
            foreach (var ch in title)
            {
                container.add_char(x0, y0, chheight, aspect, rgb_t.white(), ui_font, ch);
                x0 += ui_font.char_width(chheight, aspect, ch);
            }

            // draw the top column headers
            skip = (int)(chwidth / cellwidth);
            for (int x = 0; x < xcells; x += 1 + skip)
            {
                x0 = boxbounds.x0 + 6.0f * chwidth + (float)x * cellwidth;
                y0 = boxbounds.y0 + 2.0f * chheight;
                container.add_char(x0 + 0.5f * (cellwidth - chwidth), y0, chheight, aspect, rgb_t.white(), ui_font, "0123456789ABCDEF"[x & 0xf]);

                // if we're skipping, draw a point between the character and the box to indicate which
                // one it's referring to
                if (skip != 0)
                    container.add_point(x0 + 0.5f * cellwidth, 0.5f * (y0 + chheight + boxbounds.y0 + 3.5f * chheight), UI_LINE_WIDTH, rgb_t.white(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
            }

            // draw the side column headers
            skip = (int)(chheight / cellheight);
            for (int y = 0; y < ycells; y += 1 + skip)
            {
                // only display if there is data to show
                if (info.offset[set] + y * xcells < gfx.elements())
                {
                    string buffer;

                    // if we're skipping, draw a point between the character and the box to indicate which
                    // one it's referring to
                    x0 = boxbounds.x0 + 5.5f * chwidth;
                    y0 = boxbounds.y0 + 3.5f * chheight + (float)y * cellheight;
                    if (skip != 0)
                        container.add_point(0.5f * (x0 + boxbounds.x0 + 6.0f * chwidth), y0 + 0.5f * cellheight, UI_LINE_WIDTH, rgb_t.white(), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));

                    // draw the row header
                    sprintf(out buffer, "{0:X5}", info.offset[set] + y * xcells);
                    for (int x = 4; x >= 0; x--)
                    {
                        x0 -= ui_font.char_width(chheight, aspect, buffer[x]);
                        container.add_char(x0, y0 + 0.5f * (cellheight - chheight), chheight, aspect, rgb_t.white(), ui_font, buffer[x]);
                    }
                }
            }

            // update the bitmap
            gfxset_update_bitmap(mui.machine(), state, xcells, ycells, gfx);

            // add the final quad
            container.add_quad(cellboxbounds.x0, cellboxbounds.y0, cellboxbounds.x1, cellboxbounds.y1,
                               rgb_t.white(), state.texture, PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));

            // handle keyboard navigation before drawing
            gfxset_handle_keys(mui.machine(), state, xcells, ycells);
        }


        //-------------------------------------------------
        //  gfxset_handle_keys - handle keys for the
        //  graphics viewer
        //-------------------------------------------------
        static void gfxset_handle_keys(running_machine machine, ui_gfx_state state, int xcells, int ycells)
        {
            // handle gfxset selection (open bracket,close bracket)
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_PREV_GROUP))
            {
                if (state.gfxset.set > 0)
                {
                    state.gfxset.set--;
                }
                else if (state.gfxset.devindex > 0)
                {
                    state.gfxset.devindex--;
                    state.gfxset.set = (uint8_t)(state.gfxdev[state.gfxset.devindex].setcount - 1);
                }

                state.bitmap_dirty = true;
            }
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_NEXT_GROUP))
            {
                if (state.gfxset.set < state.gfxdev[state.gfxset.devindex].setcount - 1)
                {
                    state.gfxset.set++;
                }
                else if (state.gfxset.devindex < state.gfxset.devcount - 1)
                {
                    state.gfxset.devindex++;
                    state.gfxset.set = 0;
                }

                state.bitmap_dirty = true;
            }

            // cache some info in locals
            int dev = state.gfxset.devindex;
            int set = state.gfxset.set;
            ui_gfx_info info = state.gfxdev[dev];
            gfx_element gfx = info.interface_.gfx(set);

            // handle cells per line (minus,plus)
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_ZOOM_OUT) && (xcells < 128))
            { info.columns[set] = (uint8_t)(xcells + 1); state.bitmap_dirty = true; }

            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_ZOOM_IN) && (xcells > 2))
            { info.columns[set] = (uint8_t)(xcells - 1); state.bitmap_dirty = true; }

            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_ZOOM_DEFAULT) && (xcells != 16))
            { info.columns[set] = 16; state.bitmap_dirty = true; }

            // handle rotation (R)
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_ROTATE))
            {
                info.rotate[set] = (uint8_t)orientation_add(ROT90, info.rotate[set]);
                state.bitmap_dirty = true;
            }

            // handle navigation within the cells (up,down,pgup,pgdown)
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_UP, 4))
            { info.offset[set] -= xcells; state.bitmap_dirty = true; }
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_DOWN, 4))
            { info.offset[set] += xcells; state.bitmap_dirty = true; }
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_PAGE_UP, 6))
            { info.offset[set] -= xcells * ycells; state.bitmap_dirty = true; }
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_PAGE_DOWN, 6))
            { info.offset[set] += xcells * ycells; state.bitmap_dirty = true; }
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_HOME, 4))
            { info.offset[set] = 0; state.bitmap_dirty = true; }
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_END, 4))
            { info.offset[set] = (int)gfx.elements(); state.bitmap_dirty = true; }

            // clamp within range
            if (info.offset[set] + xcells * ycells > ((gfx.elements() + xcells - 1) / xcells) * xcells)
            {
                info.offset[set] = (int)(((gfx.elements() + xcells - 1) / xcells) * xcells - xcells * ycells);
                state.bitmap_dirty = true;
            }
            if (info.offset[set] < 0)
            { info.offset[set] = 0; state.bitmap_dirty = true; }

            // handle color selection (left,right)
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_LEFT, 4))
            { info.color[set] -= 1; state.bitmap_dirty = true; }
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_RIGHT, 4))
            { info.color[set] += 1; state.bitmap_dirty = true; }

            // clamp within range
            if (info.color[set] >= info.color_count[set])
            { info.color[set] = info.color_count[set] - 1; state.bitmap_dirty = true; }
            if (info.color[set] < 0)
            { info.color[set] = 0; state.bitmap_dirty = true; }
        }


        //-------------------------------------------------
        //  gfxset_update_bitmap - redraw the current
        //  graphics view bitmap
        //-------------------------------------------------
        static void gfxset_update_bitmap(running_machine machine, ui_gfx_state state, int xcells, int ycells, gfx_element gfx)
        {
            int dev = state.gfxset.devindex;
            int set = state.gfxset.set;
            ui_gfx_info info = state.gfxdev[dev];

            // compute the number of source pixels in a cell
            int cellxpix = 1 + ((info.rotate[set] & ORIENTATION_SWAP_XY) != 0 ? gfx.height() : gfx.width());
            int cellypix = 1 + ((info.rotate[set] & ORIENTATION_SWAP_XY) != 0 ? gfx.width() : gfx.height());

            // realloc the bitmap if it is too small
            if (!state.bitmap.valid() || state.texture == null || state.bitmap.width() != cellxpix * xcells || state.bitmap.height() != cellypix * ycells)
            {
                // free the old stuff
                machine.render().texture_free(state.texture);
                state.bitmap.reset();

                // allocate new stuff
                state.bitmap.allocate(cellxpix * xcells, cellypix * ycells);
                state.texture = machine.render().texture_alloc();
                state.texture.set_bitmap(state.bitmap, state.bitmap.cliprect(), texture_format.TEXFORMAT_ARGB32);

                // force a redraw
                state.bitmap_dirty = true;
            }

            // handle the redraw
            if (state.bitmap_dirty)
            {
                // loop over rows
                for (int y = 0, index = info.offset[set]; y < ycells; y++)
                {
                    // make a rect that covers this row
                    rectangle cellbounds = new rectangle(0, state.bitmap.width() - 1, y * cellypix, (y + 1) * cellypix - 1);

                    // only display if there is data to show
                    if (index < gfx.elements())
                    {
                        // draw the individual cells
                        for (int x = 0; x < xcells; x++, index++)
                        {
                            // update the bounds for this cell
                            cellbounds.min_x = x * cellxpix;
                            cellbounds.max_x = (x + 1) * cellxpix - 1;

                            if (index < gfx.elements()) // only render if there is data
                                gfxset_draw_item(machine, gfx, index, state.bitmap, cellbounds.min_x, cellbounds.min_y, info.color[set], info.rotate[set], info.palette[set]);
                            else // otherwise, fill with transparency
                                state.bitmap.fill(0, cellbounds);
                        }
                    }
                    else // otherwise, fill with transparency
                    {
                        state.bitmap.fill(0, cellbounds);
                    }
                }

                // reset the texture to force an update
                state.texture.set_bitmap(state.bitmap, state.bitmap.cliprect(), texture_format.TEXFORMAT_ARGB32);
                state.bitmap_dirty = false;
            }
        }


        //-------------------------------------------------
        //  gfxset_draw_item - draw a single item into
        //  the view
        //-------------------------------------------------
        static void gfxset_draw_item(running_machine machine, gfx_element gfx, int index, bitmap_rgb32 bitmap, int dstx, int dsty, int color, int rotate, device_palette_interface dpalette)
        {
            int width = (rotate & ORIENTATION_SWAP_XY) != 0 ? gfx.height() : gfx.width();
            int height = (rotate & ORIENTATION_SWAP_XY) != 0 ? gfx.width() : gfx.height();
            Pointer<rgb_t> palette = new Pointer<rgb_t>(dpalette.palette().entry_list_raw(), (int)gfx.colorbase() + color * gfx.granularity());  //const rgb_t *palette = dpalette->palette()->entry_list_raw() + gfx.colorbase() + color * gfx.granularity();

            // loop over rows in the cell
            for (int y = 0; y < height; y++)
            {
                PointerU32 dest = bitmap.pix(dsty + y, dstx);  //uint32_t *dest = &bitmap.pix(dsty + y, dstx);
                Pointer<uint8_t> src = gfx.get_data((u32)index);  //const uint8_t *src = gfx.get_data(index);

                // loop over columns in the cell
                for (int x = 0; x < width; x++)
                {
                    int effx = x;
                    int effy = y;
                    Pointer<uint8_t> s;  //const uint8_t *s;

                    // compute effective x,y values after rotation
                    if ((rotate & ORIENTATION_SWAP_XY) == 0)
                    {
                        if ((rotate & ORIENTATION_FLIP_X) != 0)
                            effx = gfx.width() - 1 - effx;
                        if ((rotate & ORIENTATION_FLIP_Y) != 0)
                            effy = gfx.height() - 1 - effy;
                    }
                    else
                    {
                        if ((rotate & ORIENTATION_FLIP_X) != 0)
                            effx = gfx.height() - 1 - effx;
                        if ((rotate & ORIENTATION_FLIP_Y) != 0)
                            effy = gfx.width() - 1 - effy;
                        std.swap(ref effx, ref effy);
                    }

                    // get a pointer to the start of this source row
                    s = src + effy * (int)gfx.rowbytes();

                    // extract the pixel
                    dest[0] = 0xff000000 | palette[s[effx]];  dest++;  //*dest++ = 0xff000000 | palette[s[effx]];
                }
            }
        }


        // tilemap handling
        /***************************************************************************
            TILEMAP VIEWER
        ***************************************************************************/

        //-------------------------------------------------
        //  tilemap_handler - handler for the tilemap
        //  viewer
        //-------------------------------------------------
        public static void tilemap_handler(mame_ui_manager mui, render_container container, ui_gfx_state state)
        {
            render_font ui_font = mui.get_font();
            render_bounds mapboxbounds;
            render_bounds boxbounds = new render_bounds();
            int targwidth = mui.machine().render().ui_target().width();
            int targheight = mui.machine().render().ui_target().height();
            float titlewidth;
            float x0;
            float y0;
            int mapboxwidth;
            int mapboxheight;

            // get the size of the tilemap itself
            tilemap_t tilemap = mui.machine().tilemap().find(state.tilemap.which);
            uint32_t mapwidth = tilemap.width();
            uint32_t mapheight = tilemap.height();
            if ((state.tilemap.rotate & ORIENTATION_SWAP_XY) != 0)
                std.swap(ref mapwidth, ref mapheight);

            // add a half character padding for the box
            float aspect = mui.machine().render().ui_aspect(container);
            float chheight = mui.get_line_height();
            float chwidth = ui_font.char_width(chheight, aspect, '0');
            boxbounds.x0 = 0.0f + 0.5f * chwidth;
            boxbounds.x1 = 1.0f - 0.5f * chwidth;
            boxbounds.y0 = 0.0f + 0.5f * chheight;
            boxbounds.y1 = 1.0f - 0.5f * chheight;

            // the tilemap box bounds starts a half character in from the box
            mapboxbounds = boxbounds;
            mapboxbounds.x0 += 0.5f * chwidth;
            mapboxbounds.x1 -= 0.5f * chwidth;
            mapboxbounds.y0 += 0.5f * chheight;
            mapboxbounds.y1 -= 0.5f * chheight;

            // add space on the top for a title and a half line of padding
            mapboxbounds.y0 += 1.5f * chheight;

            // convert back to pixels
            mapboxwidth = (int)((mapboxbounds.x1 - mapboxbounds.x0) * (float)targwidth);
            mapboxheight = (int)((mapboxbounds.y1 - mapboxbounds.y0) * (float)targheight);

            float pixelscale;
            if (state.tilemap.auto_zoom)
            {
                // determine the maximum integral scaling factor
                pixelscale = std.min(std.floor(mapboxwidth / mapwidth), std.floor(mapboxheight / mapheight));
                pixelscale = std.max(pixelscale, 1.0f);
            }
            else
            {
                pixelscale = state.tilemap.zoom_frac ? (1.0f / state.tilemap.zoom) : (float)state.tilemap.zoom;
            }

            // recompute the final box size
            mapboxwidth = std.min(mapboxwidth, std.lround(mapwidth * pixelscale));
            mapboxheight = std.min(mapboxheight, std.lround(mapheight * pixelscale));

            // recompute the bounds, centered within the existing bounds
            mapboxbounds.x0 += 0.5f * ((mapboxbounds.x1 - mapboxbounds.x0) - (float)mapboxwidth / targwidth);
            mapboxbounds.x1 = mapboxbounds.x0 + (float)mapboxwidth / targwidth;
            mapboxbounds.y0 += 0.5f * ((mapboxbounds.y1 - mapboxbounds.y0) - (float)mapboxheight / targheight);
            mapboxbounds.y1 = mapboxbounds.y0 + (float)mapboxheight / targheight;

            // now recompute the outer box against this new info
            boxbounds.x0 = mapboxbounds.x0 - 0.5f * chwidth;
            boxbounds.x1 = mapboxbounds.x1 + 0.5f * chwidth;
            boxbounds.y0 = mapboxbounds.y0 - 2.0f * chheight;
            boxbounds.y1 = mapboxbounds.y1 + 0.5f * chheight;

            // figure out the title
            string title_buf = "";
            util.stream_format(ref title_buf, "TILEMAP {0}/{1}", state.tilemap.which + 1, mui.machine().tilemap().count() - 1);

            // if the mouse pointer is over a tile, add some info about its coordinates and color
            int32_t mouse_target_x;
            int32_t mouse_target_y;
            float mouse_x;
            float mouse_y;
            bool mouse_button;
            render_target mouse_target = mui.machine().ui_input().find_mouse(out mouse_target_x, out mouse_target_y, out mouse_button);
            if (mouse_target != null && mouse_target.map_point_container(mouse_target_x, mouse_target_y, container, out mouse_x, out mouse_y)
                && mapboxbounds.x0 <= mouse_x && mapboxbounds.x1 > mouse_x
                && mapboxbounds.y0 <= mouse_y && mapboxbounds.y1 > mouse_y)
            {
                int xpixel = (int)((mouse_x - mapboxbounds.x0) * targwidth);
                int ypixel = (int)((mouse_y - mapboxbounds.y0) * targheight);
                if ((state.tilemap.rotate & ORIENTATION_FLIP_X) != 0)
                    xpixel = (mapboxwidth - 1) - xpixel;
                if ((state.tilemap.rotate & ORIENTATION_FLIP_Y) != 0)
                    ypixel = (mapboxheight - 1) - ypixel;
                if ((state.tilemap.rotate & ORIENTATION_SWAP_XY) != 0)
                    std.swap(ref xpixel, ref ypixel);

                throw new emu_unimplemented();
            }
            else
            {
                util.stream_format(ref title_buf, " {0}x{1} OFFS {2},{3}", tilemap.width(), tilemap.height(), state.tilemap.xoffs, state.tilemap.yoffs);
            }

            if (state.tilemap.flags != TILEMAP_DRAW_ALL_CATEGORIES)
                util.stream_format(ref title_buf, " CAT {0}", state.tilemap.flags);

            // expand the outer box to fit the title
            string title = title_buf;
            titlewidth = ui_font.string_width(chheight, aspect, title);
            if (boxbounds.x1 - boxbounds.x0 < titlewidth + chwidth)
            {
                boxbounds.x0 = 0.5f - 0.5f * (titlewidth + chwidth);
                boxbounds.x1 = boxbounds.x0 + titlewidth + chwidth;
            }

            // go ahead and draw the outer box now
            mui.draw_outlined_box(container, boxbounds.x0, boxbounds.y0, boxbounds.x1, boxbounds.y1, mui.colors().gfxviewer_bg_color());

            // draw the title
            x0 = 0.5f - 0.5f * titlewidth;
            y0 = boxbounds.y0 + 0.5f * chheight;
            foreach (var ch in title)
            {
                container.add_char(x0, y0, chheight, aspect, rgb_t.white(), ui_font, ch);
                x0 += ui_font.char_width(chheight, aspect, ch);
            }

            // update the bitmap
            tilemap_update_bitmap(mui.machine(), state, std.lround(mapboxwidth / pixelscale), std.lround(mapboxheight / pixelscale));

            // add the final quad
            container.add_quad(mapboxbounds.x0, mapboxbounds.y0,
                               mapboxbounds.x1, mapboxbounds.y1,
                               rgb_t.white(), state.texture,
                               PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_TEXORIENT(state.tilemap.rotate));

            // handle keyboard input
            tilemap_handle_keys(mui.machine(), state, pixelscale);
        }


        //-------------------------------------------------
        //  tilemap_handle_keys - handle keys for the
        //  tilemap view
        //-------------------------------------------------
        static void tilemap_handle_keys(running_machine machine, ui_gfx_state state, float pixelscale)
        {
            // handle tilemap selection (open bracket,close bracket)
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_PREV_GROUP) && state.tilemap.which > 0)
            { state.tilemap.which--; state.bitmap_dirty = true; }
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_NEXT_GROUP) && state.tilemap.which < machine.tilemap().count() - 1)
            { state.tilemap.which++; state.bitmap_dirty = true; }

            // cache some info in locals
            tilemap_t tilemap = machine.tilemap().find(state.tilemap.which);
            uint32_t mapwidth = tilemap.width();
            uint32_t mapheight = tilemap.height();

            bool at_max_zoom = !state.tilemap.auto_zoom && !state.tilemap.zoom_frac && state.tilemap.zoom == MAX_ZOOM_LEVEL;
            bool at_min_zoom = !state.tilemap.auto_zoom && state.tilemap.zoom_frac && state.tilemap.zoom == MIN_ZOOM_LEVEL;

            // handle zoom (minus,plus)
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_ZOOM_OUT) && !at_min_zoom)
            {
                if (state.tilemap.auto_zoom)
                {
                    // auto zoom never uses fractional factors
                    state.tilemap.zoom = std.lround(pixelscale) - 1;
                    state.tilemap.zoom_frac = state.tilemap.zoom == 0;
                    if (state.tilemap.zoom_frac)
                        state.tilemap.zoom = 2;
                    state.tilemap.auto_zoom = false;
                }
                else if (state.tilemap.zoom_frac)
                {
                    // remaining in fractional zoom range
                    state.tilemap.zoom++;
                }
                else if (state.tilemap.zoom == 1)
                {
                    // entering fractional zoom range
                    state.tilemap.zoom++;
                    state.tilemap.zoom_frac = true;
                }
                else
                {
                    // remaining in integer zoom range
                    state.tilemap.zoom--;
                }

                state.bitmap_dirty = true;

                machine.popmessage(state.tilemap.zoom_frac ? __("Zoom = 1/{0}") : __("Zoom = {0}"), state.tilemap.zoom);
            }

            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_ZOOM_IN) && !at_max_zoom)
            {
                if (state.tilemap.auto_zoom)
                {
                    // auto zoom never uses fractional factors
                    state.tilemap.zoom = std.min(std.lround(pixelscale) + 1, MAX_ZOOM_LEVEL);
                    state.tilemap.zoom_frac = false;
                    state.tilemap.auto_zoom = false;
                }
                else if (!state.tilemap.zoom_frac)
                {
                    // remaining in integer zoom range
                    state.tilemap.zoom++;
                }
                else if (state.tilemap.zoom == 2)
                {
                    // entering integer zoom range
                    state.tilemap.zoom--;
                    state.tilemap.zoom_frac = false;
                }
                else
                {
                    // remaining in fractional zoom range
                    state.tilemap.zoom--;
                }

                state.bitmap_dirty = true;

                machine.popmessage(state.tilemap.zoom_frac ? __("Zoom = 1/{0}") : __("Zoom = {0}"), state.tilemap.zoom);
            }

            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_ZOOM_DEFAULT) && !state.tilemap.auto_zoom)
            {
                state.tilemap.auto_zoom = true;

                machine.popmessage(__("Expand to fit"));
            }

            // handle rotation (R)
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_ROTATE))
            {
                state.tilemap.rotate = (uint8_t)orientation_add(ROT90, state.tilemap.rotate);
                state.bitmap_dirty = true;
            }

            // return to (0,0) (HOME)
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_HOME))
            {
                state.tilemap.xoffs = 0;
                state.tilemap.yoffs = 0;
                state.bitmap_dirty = true;
            }

            // handle flags (category)
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_PAGE_UP) && state.tilemap.flags != TILEMAP_DRAW_ALL_CATEGORIES)
            {
                if (state.tilemap.flags > 0)
                {
                    state.tilemap.flags--;
                    machine.popmessage("Category = %d", state.tilemap.flags);
                }
                else
                {
                    state.tilemap.flags = TILEMAP_DRAW_ALL_CATEGORIES;
                    machine.popmessage("Category All");
                }
                state.bitmap_dirty = true;
            }
            if (machine.ui_input().pressed((int)ioport_type.IPT_UI_PAGE_DOWN) && (state.tilemap.flags < TILEMAP_DRAW_CATEGORY_MASK || (state.tilemap.flags == TILEMAP_DRAW_ALL_CATEGORIES)))
            {
                if (state.tilemap.flags == TILEMAP_DRAW_ALL_CATEGORIES)
                    state.tilemap.flags = 0;
                else
                    state.tilemap.flags++;
                state.bitmap_dirty = true;
                machine.popmessage("Category = %d", state.tilemap.flags);
            }

            // handle navigation (up,down,left,right), taking orientation into account
            int step = 8;
            if (machine.input().code_pressed(KEYCODE_LSHIFT)) step = 1;
            if (machine.input().code_pressed(KEYCODE_LCONTROL)) step = 64;
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_UP, 4))
            {
                if ((state.tilemap.rotate & ORIENTATION_SWAP_XY) != 0)
                    state.tilemap.xoffs -= (state.tilemap.rotate & ORIENTATION_FLIP_Y) != 0 ? -step : step;
                else
                    state.tilemap.yoffs -= (state.tilemap.rotate & ORIENTATION_FLIP_Y) != 0 ? -step : step;
                state.bitmap_dirty = true;
            }
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_DOWN, 4))
            {
                if ((state.tilemap.rotate & ORIENTATION_SWAP_XY) != 0)
                    state.tilemap.xoffs += (state.tilemap.rotate & ORIENTATION_FLIP_Y) != 0 ? -step : step;
                else
                    state.tilemap.yoffs += (state.tilemap.rotate & ORIENTATION_FLIP_Y) != 0 ? -step : step;
                state.bitmap_dirty = true;
            }
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_LEFT, 6))
            {
                if ((state.tilemap.rotate & ORIENTATION_SWAP_XY) != 0)
                    state.tilemap.yoffs -= (state.tilemap.rotate & ORIENTATION_FLIP_X) != 0 ? -step : step;
                else
                    state.tilemap.xoffs -= (state.tilemap.rotate & ORIENTATION_FLIP_X) != 0 ? -step : step;
                state.bitmap_dirty = true;
            }
            if (machine.ui_input().pressed_repeat((int)ioport_type.IPT_UI_RIGHT, 6))
            {
                if ((state.tilemap.rotate & ORIENTATION_SWAP_XY) != 0)
                    state.tilemap.yoffs += (state.tilemap.rotate & ORIENTATION_FLIP_X) != 0 ? -step : step;
                else
                    state.tilemap.xoffs += (state.tilemap.rotate & ORIENTATION_FLIP_X) != 0 ? -step : step;
                state.bitmap_dirty = true;
            }

            // clamp within range
            while (state.tilemap.xoffs < 0)
                state.tilemap.xoffs += (int)mapwidth;
            while (state.tilemap.xoffs >= mapwidth)
                state.tilemap.xoffs -= (int)mapwidth;
            while (state.tilemap.yoffs < 0)
                state.tilemap.yoffs += (int)mapheight;
            while (state.tilemap.yoffs >= mapheight)
                state.tilemap.yoffs -= (int)mapheight;
        }


        //-------------------------------------------------
        //  tilemap_update_bitmap - update the bitmap
        //  for the tilemap view
        //-------------------------------------------------
        static void tilemap_update_bitmap(running_machine machine, ui_gfx_state state, int width, int height)
        {
            // swap the coordinates back if they were talking about a rotated surface
            if ((state.tilemap.rotate & ORIENTATION_SWAP_XY) != 0)
                std.swap(ref width, ref height);

            // realloc the bitmap if it is too small
            if (!state.bitmap.valid() || state.texture == null || state.bitmap.width() != width || state.bitmap.height() != height)
            {
                // free the old stuff
                machine.render().texture_free(state.texture);
                state.bitmap.reset();

                // allocate new stuff
                state.bitmap.allocate(width, height);
                state.texture = machine.render().texture_alloc();
                state.texture.set_bitmap(state.bitmap, state.bitmap.cliprect(), texture_format.TEXFORMAT_RGB32);

                // force a redraw
                state.bitmap_dirty = true;
            }

            // handle the redraw
            if (state.bitmap_dirty)
            {
                state.bitmap.fill(0);
                tilemap_t tilemap = machine.tilemap().find(state.tilemap.which);

                screen_device first_screen = new screen_device_enumerator(machine.root_device()).first();
                if (first_screen != null)
                {
                    tilemap.draw_debug(first_screen, state.bitmap, (u32)state.tilemap.xoffs, (u32)state.tilemap.yoffs, state.tilemap.flags);
                }

                // reset the texture to force an update
                state.texture.set_bitmap(state.bitmap, state.bitmap.cliprect(), texture_format.TEXFORMAT_RGB32);
                state.bitmap_dirty = false;
            }
        }
    }
}
