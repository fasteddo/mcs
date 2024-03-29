// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using indirect_pen_t = System.UInt16;  //typedef u16 indirect_pen_t;
using tilemap_t_logical_index = System.UInt32;  //typedef u32 logical_index;
using offs_t = System.UInt32;  //using offs_t = u32;
using PointerU8 = mame.Pointer<System.Byte>;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using s32 = System.Int32;
using size_t = System.UInt64;
using tilemap_memory_index = System.UInt32;  //typedef u32 tilemap_memory_index;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;

using static mame.cpp_global;
using static mame.device_global;
using static mame.digfx_global;
using static mame.drawgfx_global;
using static mame.profiler_global;
using static mame.tilemap_global;


namespace mame
{
    public static partial class tilemap_global
    {
        // maximum number of groups
        public const int TILEMAP_NUM_GROUPS              = 256;

        // these flags control tilemap_t::draw() behavior
        public const u32 TILEMAP_DRAW_CATEGORY_MASK = 0x0f;     // specify the category to draw
        public const u32 TILEMAP_DRAW_LAYER0 = 0x10;            // draw layer 0
        public const u32 TILEMAP_DRAW_LAYER1 = 0x20;            // draw layer 1
        public const u32 TILEMAP_DRAW_LAYER2 = 0x40;            // draw layer 2
        public const u32 TILEMAP_DRAW_OPAQUE = 0x80;            // draw everything, even transparent stuff
        public const u32 TILEMAP_DRAW_ALPHA_FLAG = 0x100;       // draw with alpha blending (in the upper 8 bits)
        public const u32 TILEMAP_DRAW_ALL_CATEGORIES = 0x200;   // draw all categories

        // per-pixel flags in the transparency_bitmap
        public const u8 TILEMAP_PIXEL_CATEGORY_MASK = 0x0f;     // category is stored in the low 4 bits
        public const u8 TILEMAP_PIXEL_TRANSPARENT = 0x00;       // transparent if in none of the layers below
        public const u8 TILEMAP_PIXEL_LAYER0 = 0x10;            // pixel is opaque in layer 0
        public const u8 TILEMAP_PIXEL_LAYER1 = 0x20;            // pixel is opaque in layer 1
        public const u8 TILEMAP_PIXEL_LAYER2 = 0x40;            // pixel is opaque in layer 2

        // per-tile flags, set by get_tile_info callback
        public const u8 TILE_FLIPX = 0x01;                      // draw this tile horizontally flipped
        public const u8 TILE_FLIPY = 0x02;                      // draw this tile vertically flipped
        public const u8 TILE_FORCE_LAYER0 = TILEMAP_PIXEL_LAYER0; // force all pixels to be layer 0 (no transparency)
        public const u8 TILE_FORCE_LAYER1 = TILEMAP_PIXEL_LAYER1; // force all pixels to be layer 1 (no transparency)
        public const u8 TILE_FORCE_LAYER2 = TILEMAP_PIXEL_LAYER2; // force all pixels to be layer 2 (no transparency)

        // tilemap global flags, used by tilemap_t::set_flip()
        public const u32 TILEMAP_FLIPX = TILE_FLIPX;            // draw the tilemap horizontally flipped
        public const u32 TILEMAP_FLIPY = TILE_FLIPY;            // draw the tilemap vertically flipped

        // set this value for a scroll row/column to fully disable it
        public const u32 TILE_LINE_DISABLED = 0x80000000;
    }


    // standard mappers
    public enum tilemap_standard_mapper
    {
        TILEMAP_SCAN_ROWS = 0,
        TILEMAP_SCAN_ROWS_FLIP_X,
        TILEMAP_SCAN_ROWS_FLIP_Y,
        TILEMAP_SCAN_ROWS_FLIP_XY,
        TILEMAP_SCAN_COLS,
        TILEMAP_SCAN_COLS_FLIP_X,
        TILEMAP_SCAN_COLS_FLIP_Y,
        TILEMAP_SCAN_COLS_FLIP_XY,
        TILEMAP_STANDARD_COUNT
    }


    // global types
    //typedef u32 tilemap_memory_index;


    // tile_data is filled in by the get_tile_info callback
    public struct tile_data
    {
        public device_gfx_interface decoder;  // set in tilemap_t::init()
        public PointerU8 pen_data;  //const u8 *      pen_data;       // required
        public PointerU8 mask_data;  //const u8 *      mask_data;      // required
        public pen_t palette_base;   // defaults to 0
        public u8 category;       // defaults to 0; range from 0..15
        public u8 group;          // defaults to 0; range from 0..TILEMAP_NUM_GROUPS
        public u8 flags;          // defaults to 0; one or more of TILE_* flags above
        public u8 pen_mask;       // defaults to 0xff; mask to apply to pen_data while rendering the tile
        public u8 gfxnum;         // defaults to 0xff; specify index of gfx for auto-invalidation on dirty
        u32 code;


        public void set(u8 _gfxnum, u32 rawcode, u32 rawcolor, u8 _flags)
        {
            gfx_element gfx = decoder.gfx(_gfxnum);
            code = rawcode % gfx.elements();
            pen_data = gfx.get_data((u32)code);
            palette_base = (u32)(gfx.colorbase() + gfx.granularity() * (rawcolor % gfx.colors()));
            flags = (u8)_flags;
            gfxnum = (u8)_gfxnum;
        }
    }


    // modern delegates
    public delegate void tilemap_get_info_delegate(tilemap_t tilemap, ref tile_data tiledata, tilemap_memory_index index);  //typedef device_delegate<void (tilemap_t &, tile_data &, tilemap_memory_index)> tilemap_get_info_delegate;
    public delegate tilemap_memory_index tilemap_mapper_delegate(u32 col, u32 row, u32 num_cols, u32 num_rows);  //typedef device_delegate<tilemap_memory_index (u32, u32, u32, u32)> tilemap_mapper_delegate;


    // ======================> tilemap_t
    // core tilemap structure
    public class tilemap_t : simple_list_item<tilemap_t>
    {
        //friend class tilemap_device;
        //friend class tilemap_manager;
        //friend class simple_list<tilemap_t>;


        // logical index
        //typedef u32 logical_index;


        // internal set of transparency states for rendering
        enum trans_t
        {
            WHOLLY_TRANSPARENT,
            WHOLLY_OPAQUE,
            MASKED
        }


        // blitting parameters for rendering
        struct blit_parameters
        {
            public bitmap_ind8 priority;
            public rectangle cliprect;
            public u32 tilemap_priority_code;
            public u8 mask;
            public u8 value;
            public u8 alpha;
        }


        // internal usage to mark tiles dirty
        const u8 TILE_FLAG_DIRTY = 0xff;

        // invalid logical index
        const tilemap_t_logical_index INVALID_LOGICAL_INDEX = tilemap_t_logical_index.MaxValue;

        // maximum index in each array
        const pen_t MAX_PEN_TO_FLAGS = 256;


        // managers and devices
        tilemap_manager m_manager;              // reference to the owning manager
        device_t m_device;               // pointer to our owning device
        device_palette_interface m_palette;              // palette used for drawing
        tilemap_t m_next;                 // pointer to next tilemap
        object m_user_data;            // user data value

        // basic tilemap metrics
        u32 m_rows;                 // number of tile rows
        u32 m_cols;                 // number of tile columns
        u16 m_tilewidth;            // width of a single tile in pixels
        u16 m_tileheight;           // height of a single tile in pixels
        u32 m_width;                // width of the full tilemap in pixels
        u32 m_height;               // height of the full tilemap in pixels

        // logical <-> memory mappings
        tilemap_mapper_delegate m_mapper;               // callback to map a row/column to a memory index
        std.vector<tilemap_t_logical_index> m_memory_to_logical = new std.vector<tilemap_t_logical_index>();   // map from memory index to logical index
        std.vector<tilemap_memory_index> m_logical_to_memory = new std.vector<tilemap_memory_index>();   // map from logical index to memory index

        // callback to interpret video RAM for the tilemap
        tilemap_get_info_delegate m_tile_get_info;        // callback to get information about a tile
        tile_data m_tileinfo;             // structure to hold the data for a tile

        // global tilemap states
        bool m_enable;               // true if we are enabled
        u8 m_attributes;           // global attributes (flipx/y)
        bool m_all_tiles_dirty;      // true if all tiles are dirty
        bool m_all_tiles_clean;      // true if all tiles are clean
        u32 m_palette_offset;       // palette offset
        u32 m_gfx_used;             // bitmask of gfx items used
        u32 [] m_gfx_dirtyseq = new u32[MAX_GFX_ELEMENTS]; // dirtyseq values from last check

        // scroll information
        u32 m_scrollrows;           // number of independently scrolled rows
        u32 m_scrollcols;           // number of independently scrolled columns
        std.vector<s32> m_rowscroll = new std.vector<s32>();   // array of rowscroll values
        std.vector<s32> m_colscroll = new std.vector<s32>();   // array of colscroll values
        s32 m_dx;                   // global horizontal scroll offset
        s32 m_dx_flipped;           // global horizontal scroll offset when flipped
        s32 m_dy;                   // global vertical scroll offset
        s32 m_dy_flipped;           // global vertical scroll offset when flipped

        // pixel data
        bitmap_ind16 m_pixmap = new bitmap_ind16();               // cached pixel data

        // transparency mapping
        bitmap_ind8 m_flagsmap = new bitmap_ind8();             // per-pixel flags
        std.vector<u8> m_tileflags = new std.vector<u8>();   // per-tile flags
        u8 [] m_pen_to_flags = new u8[MAX_PEN_TO_FLAGS * TILEMAP_NUM_GROUPS]; // mapping of pens to flags


        // tilemap_manager controls our allocations
        //-------------------------------------------------
        //  tilemap_t - constructor
        //-------------------------------------------------
        public tilemap_t(device_t owner)
        {
            m_mapper = null;
            m_tile_get_info = null;


            // until init() is called, data is floating; this is deliberate
        }


        //-------------------------------------------------
        //  init - initialize the tilemap
        //-------------------------------------------------
        void init_common(tilemap_manager manager, device_gfx_interface decoder, tilemap_get_info_delegate tile_get_info, u16 tilewidth, u16 tileheight, u32 cols, u32 rows)
        {
            // populate managers and devices
            m_manager = manager;

            //throw new emu_unimplemented();
#if false
            m_device = dynamic_cast<device_t *>(this);
#endif

            m_palette = decoder.palette();
            m_next = null;
            m_user_data = null;

            // populate tilemap metrics
            m_rows = rows;
            m_cols = cols;
            m_tilewidth = tilewidth;
            m_tileheight = tileheight;
            m_width = cols * tilewidth;
            m_height = rows * tileheight;

            // initialize tile information geters
            m_tile_get_info = tile_get_info;

            // reset global states
            m_enable = true;
            m_attributes = 0;
            m_all_tiles_dirty = true;
            m_all_tiles_clean = false;
            m_palette_offset = 0;
            m_gfx_used = 0;
            //memset(m_gfx_dirtyseq, 0, sizeof(m_gfx_dirtyseq));

            // reset scroll information
            m_scrollrows = 1;
            m_scrollcols = 1;
            m_rowscroll.resize(m_height);
            m_colscroll.resize(m_width);
            m_dx = 0;
            m_dx_flipped = 0;
            m_dy = 0;
            m_dy_flipped = 0;

            // allocate pixmap
            m_pixmap.allocate((int)m_width, (int)m_height);

            // allocate transparency mapping
            m_flagsmap.allocate((int)m_width, (int)m_height);
            //memset(m_pen_to_flags, 0, sizeof(m_pen_to_flags));

            // create the initial mappings
            mappings_create();

            // set up the default tile data
            //memset(&m_tileinfo, 0, sizeof(m_tileinfo));
            m_tileinfo.decoder = decoder;
            m_tileinfo.pen_mask = 0xff;
            m_tileinfo.gfxnum = 0xff;

            // allocate transparency mapping data
            for (int group = 0; group < TILEMAP_NUM_GROUPS; group++)
                map_pens_to_layer(group, 0, 0, TILEMAP_PIXEL_LAYER0);

            // save relevant state
            int instance = manager.alloc_instance();
            machine().save().save_item(m_device, "tilemap", null, instance, m_enable, "m_enable");
            machine().save().save_item(m_device, "tilemap", null, instance, m_attributes, "m_attributes");
            machine().save().save_item(m_device, "tilemap", null, instance, m_palette_offset, "m_palette_offset");
            machine().save().save_item(m_device, "tilemap", null, instance, m_scrollrows, "m_scrollrows");
            machine().save().save_item(m_device, "tilemap", null, instance, m_scrollcols, "m_scrollcols");
            machine().save().save_item(m_device, "tilemap", null, instance, m_rowscroll, "m_rowscroll");
            machine().save().save_item(m_device, "tilemap", null, instance, m_colscroll, "m_colscroll");
            machine().save().save_item(m_device, "tilemap", null, instance, m_dx, "m_dx");
            machine().save().save_item(m_device, "tilemap", null, instance, m_dx_flipped, "m_dx_flipped");
            machine().save().save_item(m_device, "tilemap", null, instance, m_dy, "m_dy");
            machine().save().save_item(m_device, "tilemap", null, instance, m_dy_flipped, "m_dy_flipped");

            // reset everything after a load
            machine().save().register_postload(postload);
        }


        public tilemap_t init(
                tilemap_manager manager,
                device_gfx_interface decoder,
                tilemap_get_info_delegate tile_get_info,
                tilemap_mapper_delegate mapper,
                u16 tilewidth,
                u16 tileheight,
                u32 cols,
                u32 rows)
        {
            // populate logical <-> memory mappings
            m_mapper = mapper;

            init_common(manager, decoder, tile_get_info, tilewidth, tileheight, cols, rows);

            return this;
        }

        public tilemap_t init(
                tilemap_manager manager,
                device_gfx_interface decoder,
                tilemap_get_info_delegate tile_get_info,
                tilemap_standard_mapper mapper,
                u16 tilewidth,
                u16 tileheight,
                u32 cols,
                u32 rows)
        {
            // populate logical <-> memory mappings
            switch (mapper)
            {
                case tilemap_standard_mapper.TILEMAP_SCAN_ROWS:         m_mapper = scan_rows;         break;
                case tilemap_standard_mapper.TILEMAP_SCAN_ROWS_FLIP_X:  m_mapper = scan_rows_flip_x;  break;
                case tilemap_standard_mapper.TILEMAP_SCAN_ROWS_FLIP_Y:  m_mapper = scan_rows_flip_y;  break;
                case tilemap_standard_mapper.TILEMAP_SCAN_ROWS_FLIP_XY: m_mapper = scan_rows_flip_xy; break;
                case tilemap_standard_mapper.TILEMAP_SCAN_COLS:         m_mapper = scan_cols;         break;
                case tilemap_standard_mapper.TILEMAP_SCAN_COLS_FLIP_X:  m_mapper = scan_cols_flip_x;  break;
                case tilemap_standard_mapper.TILEMAP_SCAN_COLS_FLIP_Y:  m_mapper = scan_cols_flip_y;  break;
                case tilemap_standard_mapper.TILEMAP_SCAN_COLS_FLIP_XY: m_mapper = scan_cols_flip_xy; break;
                default: throw new emu_fatalerror("Tilemap init unknown mapper {0}", mapper);
            }

            init_common(manager, decoder, tile_get_info, tilewidth, tileheight, cols, rows);

            return this;
        }


        public device_t device { get { return m_device; } }


        // getters
        running_machine machine() { return m_manager.machine(); }
        //device_palette_interface *palette() const { return m_palette; }
        //device_gfx_interface &decoder() const { return *m_tileinfo.decoder; }

        public tilemap_t next() { return m_next; }
        public tilemap_t m_next_get() { return m_next; }
        public void m_next_set(tilemap_t value) { m_next = value; }

        public object user_data() { return m_user_data; }  //void *user_data() const { return m_user_data; }
        //u32 rows() const { return m_rows; }
        //u32 cols() const { return m_cols; }
        //u16 tilewidth() const { return m_tilewidth; }
        //u16 tileheight() const { return m_tileheight; }
        public u32 width() { return m_width; }
        public u32 height() { return m_height; }
        //bool enabled() const { return m_enable; }
        //u32 palette_offset() const { return m_palette_offset; }
        //int scrolldx() const { return (m_attributes & TILEMAP_FLIPX) ? m_dx_flipped : m_dx; }
        //int scrolldy() const { return (m_attributes & TILEMAP_FLIPY) ? m_dy_flipped : m_dy; }
        //int scrollx(int which = 0) const { return (which < m_scrollrows) ? m_rowscroll[which] : 0; }
        //int scrolly(int which = 0) const { return (which < m_scrollcols) ? m_colscroll[which] : 0; }
        //bitmap_ind16 &pixmap() { pixmap_update(); return m_pixmap; }
        //bitmap_ind8 &flagsmap() { pixmap_update(); return m_flagsmap; }
        //u8 *tile_flags() { pixmap_update(); return &m_tileflags[0]; }
        tilemap_memory_index memory_index(u32 col, u32 row) { return m_mapper(col, row, m_cols, m_rows); }
        //void get_info_debug(u32 col, u32 row, u8 &gfxnum, u32 &code, u32 &color);


        // setters
        //void enable(bool enable = true) { m_enable = enable; }
        public void set_user_data(object user_data) { m_user_data = user_data; }  //void set_user_data(void *user_data) { m_user_data = user_data; }
        //void set_palette(device_palette_interface *palette) { m_palette = palette; }
        //void set_palette_offset(u32 offset) { m_palette_offset = offset; }
        public void set_scrolldx(int dx, int dx_flipped) { m_dx = dx; m_dx_flipped = dx_flipped; }
        public void set_scrolldy(int dy, int dy_flipped) { m_dy = dy; m_dy_flipped = dy_flipped; }
        public void set_scrollx(int which, int value) { if (which < m_scrollrows) m_rowscroll[which] = value; }
        public void set_scrolly(int which, int value) { if (which < m_scrollcols) m_colscroll[which] = value; }
        public void set_scrollx(int value) { set_scrollx(0, value); }
        public void set_scrolly(int value) { set_scrolly(0, value); }
        public void set_scroll_rows(u32 scroll_rows) { assert(scroll_rows <= m_height); m_scrollrows = scroll_rows; }
        public void set_scroll_cols(u32 scroll_cols) { assert(scroll_cols <= m_width); m_scrollcols = scroll_cols; }
        public void set_flip(u32 attributes) { if (m_attributes != attributes) { m_attributes = (u8)attributes; mappings_update(); } }


        // dirtying
        void mark_mapping_dirty() { mappings_update(); }

        //-------------------------------------------------
        //  mark_tile_dirty - mark a single tile dirty
        //  based on its memory index
        //-------------------------------------------------
        public void mark_tile_dirty(tilemap_memory_index memindex)
        {
            // only mark if within range
            if (memindex < m_memory_to_logical.size())
            {
                // there may be no logical index for a given memory index
                tilemap_t_logical_index logindex = m_memory_to_logical[memindex];
                if (logindex != INVALID_LOGICAL_INDEX)
                {
                    m_tileflags[logindex] = TILE_FLAG_DIRTY;
                    m_all_tiles_clean = false;
                }
            }
        }

        public void mark_all_dirty() { m_all_tiles_dirty = true; m_all_tiles_clean = false; }


        // pen mapping

        //-------------------------------------------------
        //  map_pens_to_layer - specify the mapping of one
        //  or more pens (where (<pen> & mask) == pen) to
        //  a layer
        //-------------------------------------------------
        void map_pens_to_layer(int group, pen_t pen, pen_t mask, u8 layermask)
        {
            //assert(group < TILEMAP_NUM_GROUPS);
            //assert((layermask & TILEMAP_PIXEL_CATEGORY_MASK) == 0);

            // we start at the index where (pen & mask) == pen, and all other bits are 0
            pen_t start = pen & mask;

            // we stop at the index where (pen & mask) == pen, and all other bits are 1
            pen_t stop = start | ~mask;

            // clamp to the number of entries actually there
            stop = std.min(stop, MAX_PEN_TO_FLAGS - 1);

            // iterate and set
            int arrayIdx = group * (int)MAX_PEN_TO_FLAGS;  //u8 *array = m_pen_to_flags + group * MAX_PEN_TO_FLAGS;
            bool changed = false;
            for (pen_t cur = start; cur <= stop; cur++)
            {
                if ((cur & mask) == pen && m_pen_to_flags[arrayIdx + cur] != layermask)  //if ((cur & mask) == pen && array[cur] != layermask)
                {
                    changed = true;
                    m_pen_to_flags[arrayIdx + cur] = layermask;  //array[cur] = layermask;
                }
            }

            // everything gets dirty if anything changed
            if (changed)
                mark_all_dirty();
        }

        void map_pen_to_layer(int group, pen_t pen, u8 layermask) { map_pens_to_layer(group, pen, tilemap_t_logical_index.MaxValue /* ~0*/, layermask); }

        //-------------------------------------------------
        //  set_transparent_pen - set a single transparent
        //  pen into the tilemap, mapping all other pens
        //  to layer 0
        //-------------------------------------------------
        public void set_transparent_pen(pen_t pen)
        {
            // reset the whole pen map to opaque
            map_pens_to_layer(0, 0, 0, TILEMAP_PIXEL_LAYER0);

            // set the single pen to transparent
            map_pen_to_layer(0, pen, TILEMAP_PIXEL_TRANSPARENT);
        }

        //-------------------------------------------------
        //  set_transmask - set up the first 32 pens using
        //  a foreground mask (mapping to layer 0) and a
        //  background mask (mapping to layer 1)
        //-------------------------------------------------
        public void set_transmask(int group, u32 fgmask, u32 bgmask)
        {
            // iterate over all 32 pens specified
            for (pen_t pen = 0; pen < 32; pen++)
            {
                u8 fgbits = ((fgmask >> (int)pen) & 1) != 0 ? TILEMAP_PIXEL_TRANSPARENT : TILEMAP_PIXEL_LAYER0;
                u8 bgbits = ((bgmask >> (int)pen) & 1) != 0 ? TILEMAP_PIXEL_TRANSPARENT : TILEMAP_PIXEL_LAYER1;
                map_pen_to_layer(group, pen, (u8)(fgbits | bgbits));
            }
        }

        //-------------------------------------------------
        //  configure_groups - configure groups so that
        //  when group == color, pens whose indirect value
        //  matches the given transcolor are transparent
        //-------------------------------------------------
        public void configure_groups(gfx_element gfx, indirect_pen_t transcolor)
        {
            assert(gfx.colors() <= TILEMAP_NUM_GROUPS);

            // iterate over all colors in the tilemap
            for (u32 color = 0; color < gfx.colors(); color++)
                set_transmask((int)color, m_palette.transpen_mask(gfx, color, transcolor), 0);
        }


        // drawing

        public void draw(screen_device screen, bitmap_ind16 dest, rectangle cliprect, u32 flags = TILEMAP_DRAW_ALL_CATEGORIES, u8 priority = 0, u8 priority_mask = 0xff)
        { draw_common<bitmap_ind16, u16, PixelType_operators_u16, PointerU16>(screen, dest, cliprect, flags, priority, priority_mask); }

        public void draw(screen_device screen, bitmap_rgb32 dest, rectangle cliprect, u32 flags = TILEMAP_DRAW_ALL_CATEGORIES, u8 priority = 0, u8 priority_mask = 0xff)
        { draw_common<bitmap_rgb32, u32, PixelType_operators_u32, PointerU32>(screen, dest, cliprect, flags, priority, priority_mask); }

        //void draw_roz(screen_device &screen, bitmap_ind16 &dest, const rectangle &cliprect, u32 startx, u32 starty, int incxx, int incxy, int incyx, int incyy, bool wraparound, u32 flags = TILEMAP_DRAW_ALL_CATEGORIES, u8 priority = 0, u8 priority_mask = 0xff);
        //void draw_roz(screen_device &screen, bitmap_rgb32 &dest, const rectangle &cliprect, u32 startx, u32 starty, int incxx, int incxy, int incyx, int incyy, bool wraparound, u32 flags = TILEMAP_DRAW_ALL_CATEGORIES, u8 priority = 0, u8 priority_mask = 0xff);

        //-------------------------------------------------
        //  draw_debug - draw a debug version without any
        //  rowscroll and with fixed parameters
        //-------------------------------------------------
        public void draw_debug(screen_device screen, bitmap_rgb32 dest, u32 scrollx, u32 scrolly, u32 flags = TILEMAP_DRAW_ALL_CATEGORIES)
        {
            // set up for the blit, using hard-coded parameters (no priority, etc)
            blit_parameters blit;
            bitmap_ind8 dummy_priority = new bitmap_ind8();

            // draw everything
            flags |= TILEMAP_DRAW_OPAQUE;

            configure_blit_parameters(out blit, dummy_priority, dest.cliprect(), flags, 0, 0xff);

            // compute the effective scroll positions
            scrollx = m_width  - scrollx % m_width;
            scrolly = m_height - scrolly % m_height;

            // flush the dirty state to all tiles as appropriate
            realize_all_dirty_tiles();

            // iterate to handle wraparound
            for (int ypos = (int)(scrolly - m_height); ypos <= blit.cliprect.bottom(); ypos += (int)m_height)
            {
                for (int xpos = (int)(scrollx - m_width); xpos <= blit.cliprect.right(); xpos += (int)m_width)
                    draw_instance<bitmap_rgb32, u32, PixelType_operators_u32, PointerU32>(screen, dest, blit, xpos, ypos);
            }
        }


        // mappers

        // scan in row-major order with optional flipping
        //-------------------------------------------------
        // scan_rows
        // scan_rows_flip_x
        // scan_rows_flip_y
        // scan_rows_flip_xy - scan in row-major
        //  order with optional flipping
        //-------------------------------------------------
        public static tilemap_memory_index scan_rows(u32 col, u32 row, u32 num_cols, u32 num_rows)
        {
            return row * num_cols + col;
        }

        public static tilemap_memory_index scan_rows_flip_x(u32 col, u32 row, u32 num_cols, u32 num_rows)
        {
            return row * num_cols + (num_cols - 1 - col);
        }

        public static tilemap_memory_index scan_rows_flip_y(u32 col, u32 row, u32 num_cols, u32 num_rows)
        {
            return (num_rows - 1 - row) * num_cols + col;
        }

        public static tilemap_memory_index scan_rows_flip_xy(u32 col, u32 row, u32 num_cols, u32 num_rows)
        {
            return (num_rows - 1 - row) * num_cols + (num_cols - 1 - col);
        }


        // scan in column-major order with optional flipping
        //-------------------------------------------------
        //  scan_cols
        //  scan_cols_flip_x
        //  scan_cols_flip_y
        //  scan_cols_flip_xy - scan in column-
        //  major order with optional flipping
        //-------------------------------------------------

        public static tilemap_memory_index scan_cols(u32 col, u32 row, u32 num_cols, u32 num_rows)
        {
            return col * num_rows + row;
        }

        public static tilemap_memory_index scan_cols_flip_x(u32 col, u32 row, u32 num_cols, u32 num_rows)
        {
            return (num_cols - 1 - col) * num_rows + row;
        }

        public static tilemap_memory_index scan_cols_flip_y(u32 col, u32 row, u32 num_cols, u32 num_rows)
        {
            return col * num_rows + (num_rows - 1 - row);
        }

        public static tilemap_memory_index scan_cols_flip_xy(u32 col, u32 row, u32 num_cols, u32 num_rows)
        {
            return (num_cols - 1 - col) * num_rows + (num_rows - 1 - row);
        }


        // inline helpers

        //-------------------------------------------------
        //  effective_rowscroll - return the effective
        //  rowscroll value for a given index, taking into
        //  account tilemap flip states
        //-------------------------------------------------
        s32 effective_rowscroll(int index, u32 screen_width)
        {
            // if we're flipping vertically, adjust the row number
            if ((m_attributes & TILEMAP_FLIPY) != 0)
                index = (int)(m_scrollrows - 1 - index);

            // adjust final result based on the horizontal flip and dx values
            s32 value;
            if ((m_attributes & TILEMAP_FLIPX) == 0)
                value = m_dx - m_rowscroll[index];
            else
                value = (int)(screen_width - m_width - (m_dx_flipped - m_rowscroll[index]));

            // clamp to 0..width
            if (value < 0)
                value = (int)(m_width - (-value) % m_width);
            else
                value %= (int)m_width;

            return value;
        }

        //-------------------------------------------------
        //  effective_colscroll - return the effective
        //  colscroll value for a given index, taking into
        //  account tilemap flip states
        //-------------------------------------------------
        s32 effective_colscroll(int index, u32 screen_height)
        {
            // if we're flipping horizontally, adjust the column number
            if ((m_attributes & TILEMAP_FLIPX) != 0)
                index = (int)(m_scrollcols - 1 - index);

            // adjust final result based on the vertical flip and dx values
            s32 value;
            if ((m_attributes & TILEMAP_FLIPY) == 0)
                value = m_dy - m_colscroll[index];
            else
                value = (int)(screen_height - m_height - (m_dy_flipped - m_colscroll[index]));

            // clamp to 0..height
            if (value < 0)
                value = (int)(m_height - (-value) % m_height);
            else
                value %= (int)m_height;

            return value;
        }

        //-------------------------------------------------
        //  gfx_tiles_changed - return TRUE if any
        //  gfx_elements used by this tilemap have
        //  changed
        //-------------------------------------------------
        bool gfx_elements_changed()
        {
            u32 usedmask = m_gfx_used;
            bool isdirty = false;

            // iterate over all used gfx types and set the dirty flag if any of them have changed
            for (int gfxnum = 0; usedmask != 0; usedmask >>= 1, gfxnum++)
            {
                if ((usedmask & 1) != 0)
                {
                    if (m_gfx_dirtyseq[gfxnum] != m_tileinfo.decoder.gfx(gfxnum).dirtyseq())
                    {
                        m_gfx_dirtyseq[gfxnum] = m_tileinfo.decoder.gfx(gfxnum).dirtyseq();
                        isdirty = true;
                    }
                }
            }

            return isdirty;
        }


        // inline scanline rasterizers

        //-------------------------------------------------
        //  scanline_draw_opaque_null - draw to a NULL
        //  bitmap, setting priority only
        //-------------------------------------------------
        void scanline_draw_opaque_null(int count, PointerU8 pri, u32 pcode)  //void scanline_draw_opaque_null(int count, u8 *pri, u32 pcode);
        {
            // skip entirely if not changing priority
            if (pcode == 0xff00)
                return;

            // update priority across the scanline
            for (int i = 0; i < count; i++)
                pri[i] = (u8)((pri[i] & (pcode >> 8)) | pcode);
        }

        //-------------------------------------------------
        //  scanline_draw_masked_null - draw to a NULL
        //  bitmap using a mask, setting priority only
        //-------------------------------------------------
        void scanline_draw_masked_null(PointerU8 maskptr, int mask, int value, int count, PointerU8 pri, u32 pcode)  //void scanline_draw_masked_null(const u8 *maskptr, int mask, int value, int count, u8 *pri, u32 pcode);
        {
            // skip entirely if not changing priority
            if (pcode == 0xff00)
                return;

            // update priority across the scanline, checking the mask
            for (int i = 0; i < count; i++)
            {
                if ((maskptr[i] & mask) == value)
                    pri[i] = (u8)((pri[i] & (pcode >> 8)) | pcode);
            }
        }

        //-------------------------------------------------
        //  scanline_draw_opaque_ind16 - draw to a 16bpp
        //  indexed bitmap
        //-------------------------------------------------
        void scanline_draw_opaque_ind16(PointerU16 dest, PointerU16 source, int count, PointerU8 pri, u32 pcode)  //void scanline_draw_opaque_ind16(u16 *dest, const u16 *source, int count, u8 *pri, u32 pcode)
        {
            // special case for no palette offset
            int pal = (int)pcode >> 16;
            if (pal == 0)
            {
                // use memcpy which should be well-optimized for the platform
                std.memcpy(dest, source, (size_t)(count * 2));

                // skip the rest if not changing priority
                if (pcode == 0xff00)
                    return;

                // update priority across the scanline
                for (int i = 0; i < count; i++)
                    pri[i] = (u8)((pri[i] & (pcode >> 8)) | pcode);
            }

            // priority case
            else if ((pcode & 0xffff) != 0xff00)
            {
                for (int i = 0; i < count; i++)
                {
                    dest[i] = (u16)(source[i] + pal);
                    pri[i] = (u8)((pri[i] & (pcode >> 8)) | pcode);
                }
            }

            // no priority case
            else
            {
                for (int i = 0; i < count; i++)
                    dest[i] = (u16)(source[i] + pal);
            }
        }

        //-------------------------------------------------
        //  scanline_draw_masked_ind16 - draw to a 16bpp
        //  indexed bitmap using a mask
        //-------------------------------------------------
        void scanline_draw_masked_ind16(PointerU16 dest, PointerU16 source, PointerU8 maskptr, int mask, int value, int count, PointerU8 pri, u32 pcode)  //void scanline_draw_masked_ind16(u16 *dest, const u16 *source, const u8 *maskptr, int mask, int value, int count, u8 *pri, u32 pcode)
        {
            int pal = (int)pcode >> 16;

            // priority case
            if ((pcode & 0xffff) != 0xff00)
            {
                for (int i = 0; i < count; i++)
                {
                    if ((maskptr[i] & mask) == value)
                    {
                        dest[i] = (u16)(source[i] + pal);
                        pri[i] = (u8)((pri[i] & (pcode >> 8)) | pcode);
                    }
                }
            }

            // no priority case
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if ((maskptr[i] & mask) == value)
                        dest[i] = (u16)(source[i] + pal);
                }
            }
        }

        //-------------------------------------------------
        //  scanline_draw_opaque_rgb32 - draw to a 32bpp
        //  RGB bitmap
        //-------------------------------------------------
        void scanline_draw_opaque_rgb32(PointerU32 dest, PointerU16 source, int count, MemoryContainer<rgb_t> pens, PointerU8 pri, u32 pcode)  //void scanline_draw_opaque_rgb32(u32 *dest, const u16 *source, int count, const rgb_t *pens, u8 *pri, u32 pcode)
        {
            Pointer<rgb_t> clut = new Pointer<rgb_t>(pens, (int)pcode >> 16);  //const rgb_t *clut = &pens[pcode >> 16];

            // priority case
            if ((pcode & 0xffff) != 0xff00)
            {
                for (int i = 0; i < count; i++)
                {
                    dest[i] = clut[source[i]];
                    pri[i] = (u8)((pri[i] & (pcode >> 8)) | pcode);
                }
            }

            // no priority case
            else
            {
                for (int i = 0; i < count; i++)
                    dest[i] = clut[source[i]];
            }
        }

        //-------------------------------------------------
        //  scanline_draw_masked_rgb32 - draw to a 32bpp
        //  RGB bitmap using a mask
        //-------------------------------------------------
        void scanline_draw_masked_rgb32(PointerU32 dest, PointerU16 source, PointerU8 maskptr, int mask, int value, int count, MemoryContainer<rgb_t> pens, PointerU8 pri, u32 pcode)  //void scanline_draw_masked_rgb32(u32 *dest, const u16 *source, const u8 *maskptr, int mask, int value, int count, const rgb_t *pens, u8 *pri, u32 pcode)
        {
            Pointer<rgb_t> clut = new Pointer<rgb_t>(pens, (int)pcode >> 16);  //const rgb_t *clut = &pens[pcode >> 16];

            // priority case
            if ((pcode & 0xffff) != 0xff00)
            {
                for (int i = 0; i < count; i++)
                {
                    if ((maskptr[i] & mask) == value)
                    {
                        dest[i] = clut[source[i]];
                        pri[i] = (u8)((pri[i] & (pcode >> 8)) | pcode);
                    }
                }
            }

            // no priority case
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if ((maskptr[i] & mask) == value)
                        dest[i] = clut[source[i]];
                }
            }
        }

        //-------------------------------------------------
        //  scanline_draw_opaque_rgb32_alpha - draw to a
        //  32bpp RGB bitmap with alpha blending
        //-------------------------------------------------
        void scanline_draw_opaque_rgb32_alpha(PointerU32 dest, PointerU16 source, int count, MemoryContainer<rgb_t> pens, PointerU8 pri, u32 pcode, u8 alpha)  //void scanline_draw_opaque_rgb32_alpha(u32 *dest, const u16 *source, int count, const rgb_t *pens, u8 *pri, u32 pcode, u8 alpha)
        {
            Pointer<rgb_t> clut = new Pointer<rgb_t>(pens, (int)pcode >> 16);  //const rgb_t *clut = &pens[pcode >> 16];

            // priority case
            if ((pcode & 0xffff) != 0xff00)
            {
                for (int i = 0; i < count; i++)
                {
                    dest[i] = alpha_blend_r32(dest[i], clut[source[i]], alpha);
                    pri[i] = (u8)((pri[i] & (pcode >> 8)) | pcode);
                }
            }

            // no priority case
            else
            {
                for (int i = 0; i < count; i++)
                    dest[i] = alpha_blend_r32(dest[i], clut[source[i]], alpha);
            }
        }

        //-------------------------------------------------
        //  scanline_draw_masked_rgb32_alpha - draw to a
        //  32bpp RGB bitmap using a mask and alpha
        //  blending
        //-------------------------------------------------
        void scanline_draw_masked_rgb32_alpha(PointerU32 dest, PointerU16 source, PointerU8 maskptr, int mask, int value, int count, MemoryContainer<rgb_t> pens, PointerU8 pri, u32 pcode, u8 alpha)  //void scanline_draw_masked_rgb32_alpha(u32 *dest, const u16 *source, const u8 *maskptr, int mask, int value, int count, const rgb_t *pens, u8 *pri, u32 pcode, u8 alpha)
        {
            Pointer<rgb_t> clut = new Pointer<rgb_t>(pens, (int)pcode >> 16);  //const rgb_t *clut = &pens[pcode >> 16];

            // priority case
            if ((pcode & 0xffff) != 0xff00)
            {
                for (int i = 0; i < count; i++)
                {
                    if ((maskptr[i] & mask) == value)
                    {
                        dest[i] = alpha_blend_r32(dest[i], clut[source[i]], alpha);
                        pri[i] = (u8)((pri[i] & (pcode >> 8)) | pcode);
                    }
                }
            }

            // no priority case
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if ((maskptr[i] & mask) == value)
                        dest[i] = alpha_blend_r32(dest[i], clut[source[i]], alpha);
                }
            }
        }


        // internal helpers

        //-------------------------------------------------
        //  postload - after loading a save state
        //  invalidate everything
        //-------------------------------------------------
        void postload() { mappings_update(); }


        //-------------------------------------------------
        //  mappings_create - allocate memory for the
        //  mapping tables and compute their extents
        //-------------------------------------------------
        void mappings_create()
        {
            // compute the maximum logical index
            tilemap_t_logical_index max_logical_index = m_rows * m_cols;

            // compute the maximum memory index
            tilemap_memory_index max_memory_index = 0;
            for (u32 row = 0; row < m_rows; row++)
            {
                for (u32 col = 0; col < m_cols; col++)
                {
                    tilemap_memory_index memindex = memory_index(col, row);
                    max_memory_index = std.max(max_memory_index, memindex);
                }
            }
            max_memory_index++;

            // allocate the necessary mappings
            m_memory_to_logical.resize(max_memory_index);
            m_logical_to_memory.resize(max_logical_index);
            m_tileflags.resize(max_logical_index);

            // update the mappings
            mappings_update();
        }

        //-------------------------------------------------
        //  mappings_update - update the mappings after
        //  a major change (flip x/y changes)
        //-------------------------------------------------
        void mappings_update()
        {
            // initialize all the mappings to invalid values
            std.memset(m_memory_to_logical, (u32)0xff, m_memory_to_logical.size());  //memset(&m_memory_to_logical[0], 0xff, m_memory_to_logical.size() * sizeof(m_memory_to_logical[0]));

            // now iterate over all logical indexes and populate the memory index
            for (tilemap_t_logical_index logindex = 0; logindex < m_logical_to_memory.size(); logindex++)
            {
                u32 logical_col = logindex % m_cols;
                u32 logical_row = logindex / m_cols;
                tilemap_memory_index memindex = memory_index(logical_col, logical_row);

                // apply tilemap flip to get the final location to store
                if ((m_attributes & TILEMAP_FLIPX) != 0)
                    logical_col = (m_cols - 1) - logical_col;
                if ((m_attributes & TILEMAP_FLIPY) != 0)
                    logical_row = (m_rows - 1) - logical_row;
                u32 flipped_logindex = logical_row * m_cols + logical_col;

                // fill in entries in both arrays
                m_memory_to_logical[memindex] = flipped_logindex;
                m_logical_to_memory[flipped_logindex] = memindex;
            }

            // mark the whole tilemap dirty
            mark_all_dirty();
        }

        //**************************************************************************
        //  TILE RENDERING
        //**************************************************************************
        void realize_all_dirty_tiles()
        {
            // if all the tiles are marked dirty, or something in the gfx has changed,
            // flush the dirty status to all tiles
            if (m_all_tiles_dirty || gfx_elements_changed())
            {
                std.memset(m_tileflags, TILE_FLAG_DIRTY, m_tileflags.size());  //memset(&m_tileflags[0], TILE_FLAG_DIRTY, m_tileflags.size());
                m_all_tiles_dirty = false;
                m_gfx_used = 0;
            }
        }


        // internal drawing

        //void pixmap_update();

        //-------------------------------------------------
        //  tile_update - update a single dirty tile
        //-------------------------------------------------
        void tile_update(tilemap_t_logical_index logindex, u32 col, u32 row)
        {
            g_profiler.start(profile_type.PROFILER_TILEMAP_UPDATE);


            // call the get info callback for the associated memory index
            tilemap_memory_index memindex = m_logical_to_memory[logindex];
            m_tile_get_info(this, ref m_tileinfo, memindex);

            // apply the global tilemap flip to the returned flip flags
            u32 flags = (u32)(m_tileinfo.flags ^ (m_attributes & 0x03));

            // draw the tile, using either direct or transparent
            u32 x0 = m_tilewidth * col;
            u32 y0 = m_tileheight * row;
            m_tileflags[logindex] = tile_draw(m_tileinfo.pen_data, x0, y0,
                m_tileinfo.palette_base, m_tileinfo.category, m_tileinfo.group, (u8)flags, m_tileinfo.pen_mask);

            // if mask data is specified, apply it
            if ((flags & (TILE_FORCE_LAYER0 | TILE_FORCE_LAYER1 | TILE_FORCE_LAYER2)) == 0 && m_tileinfo.mask_data != null)
                m_tileflags[logindex] = tile_apply_bitmask(m_tileinfo.mask_data, x0, y0, m_tileinfo.category, (u8)flags);

            // track which gfx have been used for this tilemap
            if (m_tileinfo.gfxnum != 0xff && (m_gfx_used & (1 << m_tileinfo.gfxnum)) == 0)
            {
                m_gfx_used |= 1U << m_tileinfo.gfxnum;
                m_gfx_dirtyseq[m_tileinfo.gfxnum] = m_tileinfo.decoder.gfx(m_tileinfo.gfxnum).dirtyseq();
            }


            g_profiler.stop();
        }

        //-------------------------------------------------
        //  tile_draw - draw a single tile to the
        //  tilemap's internal pixmap, using the pen as
        //  the pen_to_flags lookup value, and adding
        //  the palette_base
        //-------------------------------------------------
        u8 tile_draw(PointerU8 pendata, u32 x0, u32 y0, u32 palette_base, u8 category, u8 group, u8 flags, u8 pen_mask)  //u8 tile_draw(const u8 *pendata, u32 x0, u32 y0, u32 palette_base, u8 category, u8 group, u8 flags, u8 pen_mask);
        {
            // OR in the force layer flags
            category |= (u8)(flags & (TILE_FORCE_LAYER0 | TILE_FORCE_LAYER1 | TILE_FORCE_LAYER2));

            // if we're vertically flipped, point to the bottom row and work backwards
            int dy0 = 1;
            if ((flags & TILE_FLIPY) != 0)
            {
                y0 += (u32)m_tileheight - 1;
                dy0 = -1;
            }

            // if we're horizontally flipped, point to the rightmost column and work backwards
            int dx0 = 1;
            if ((flags & TILE_FLIPX) != 0)
            {
                x0 += (u32)m_tilewidth - 1;
                dx0 = -1;
            }

            // iterate over rows
            //const u8 *penmap = m_pen_to_flags + group * MAX_PEN_TO_FLAGS;
            u32 penmapOffset = (u32)(group * MAX_PEN_TO_FLAGS);
            u8 andmask = u8.MaxValue;  //~0;
            u8 ormask = 0;
            for (u16 ty = 0; ty < m_tileheight; ty++)
            {
                PointerU16 pixptr = m_pixmap.pix((int)y0, (int)x0);  //u16 *pixptr = &m_pixmap.pix(y0, x0);
                PointerU8 flagsptr = m_flagsmap.pix((int)y0, (int)x0);  //u8 *flagsptr = &m_flagsmap.pix(y0, x0);

                // pre-advance to the next row
                y0 += (u32)dy0;

                // 8bpp data
                int xoffs = 0;
                for (u16 tx = 0; tx < m_tilewidth; tx++)
                {
                    u8 pen = (u8)(pendata[0] & pen_mask);  //u8 pen = (*pendata++) & pen_mask;
                    pendata++;
                    u8 map = m_pen_to_flags[penmapOffset + pen] ;  //u8 map = penmap[pen];
                    pixptr[xoffs] = (u16)(palette_base + pen);
                    flagsptr[xoffs] = (u8)(map | category);
                    andmask &= map;
                    ormask |= map;
                    xoffs += dx0;
                }
            }

            return (u8)(andmask ^ ormask);
        }

        //-------------------------------------------------
        //  tile_apply_bitmask - apply a bitmask to an
        //  already-rendered tile by modifying the
        //  flagsmap appropriately
        //-------------------------------------------------
        u8 tile_apply_bitmask(PointerU8 maskdata, u32 x0, u32 y0, u8 category, u8 flags)  //u8 tile_apply_bitmask(const u8 *maskdata, u32 x0, u32 y0, u8 category, u8 flags);
        {
            // if we're vertically flipped, point to the bottom row and work backwards
            int dy0 = 1;
            if ((flags & TILE_FLIPY) != 0)
            {
                y0 += (u32)m_tileheight - 1;
                dy0 = -1;
            }

            // if we're horizontally flipped, point to the rightmost column and work backwards
            int dx0 = 1;
            if ((flags & TILE_FLIPX) != 0)
            {
                x0 += (u32)m_tilewidth - 1;
                dx0 = -1;
            }

            // iterate over rows
            u8 andmask = u8.MaxValue;  //~0;
            u8 ormask = 0;
            int bitoffs = 0;
            for (u16 ty = 0; ty < m_tileheight; ty++)
            {
                // pre-advance to the next row
                PointerU8 flagsptr = m_flagsmap.pix((int)y0, (int)x0);  //u8 *flagsptr = &m_flagsmap.pix(y0, x0);

                y0 += (u32)dy0;

                // anywhere the bitmask is 0 should be transparent
                int xoffs = 0;
                for (u16 tx = 0; tx < m_tilewidth; tx++)
                {
                    u8 map = flagsptr[xoffs];

                    if ((maskdata[bitoffs / 8] & (0x80 >> (bitoffs & 7))) == 0)
                        map = flagsptr[xoffs] = (u8)(TILEMAP_PIXEL_TRANSPARENT | category);

                    andmask &= map;
                    ormask |= map;
                    bitoffs++;
                    xoffs += dx0;
                }
            }

            return (u8)(andmask ^ ormask);
        }


        //-------------------------------------------------
        //  configure_blit_parameters - fill in the
        //  standard blit parameters based on the input
        //  data; this code is shared by normal, roz,
        //  and indexed drawing code
        //-------------------------------------------------
        void configure_blit_parameters(out blit_parameters blit, bitmap_ind8 priority_bitmap, rectangle cliprect, u32 flags, u8 priority, u8 priority_mask)
        {
            blit = new blit_parameters();

            // set the target bitmap
            blit.priority = priority_bitmap;
            blit.cliprect = cliprect;

            // set the priority code and alpha
            blit.tilemap_priority_code = (u32)priority | ((u32)priority_mask << 8) | (m_palette_offset << 16);
            blit.alpha = (flags & TILEMAP_DRAW_ALPHA_FLAG) != 0 ? (u8)(flags >> 24) : (u8)0xff;

            // tile priority; unless otherwise specified, draw anything in layer 0
            blit.mask = TILEMAP_PIXEL_CATEGORY_MASK;
            blit.value = (u8)(flags & TILEMAP_PIXEL_CATEGORY_MASK);

            // if no layers specified, draw layer 0
            if ((flags & (TILEMAP_DRAW_LAYER0 | TILEMAP_DRAW_LAYER1 | TILEMAP_DRAW_LAYER2)) == 0)
                flags |= TILEMAP_DRAW_LAYER0;

            // OR in the bits from the draw masks
            blit.mask |= (u8)(flags & (TILEMAP_DRAW_LAYER0 | TILEMAP_DRAW_LAYER1 | TILEMAP_DRAW_LAYER2));
            blit.value |= (u8)(flags & (TILEMAP_DRAW_LAYER0 | TILEMAP_DRAW_LAYER1 | TILEMAP_DRAW_LAYER2));

            // for all-opaque rendering, don't check any of the layer bits
            if ((flags & TILEMAP_DRAW_OPAQUE) != 0)
            {
                blit.mask = (u8)(blit.mask & ~(TILEMAP_PIXEL_LAYER0 | TILEMAP_PIXEL_LAYER1 | TILEMAP_PIXEL_LAYER2));
                blit.value = (u8)(blit.value & ~(TILEMAP_PIXEL_LAYER0 | TILEMAP_PIXEL_LAYER1 | TILEMAP_PIXEL_LAYER2));
            }

            // don't check category if requested
            if ((flags & TILEMAP_DRAW_ALL_CATEGORIES) != 0)
            {
                blit.mask = (u8)(blit.mask & ~TILEMAP_PIXEL_CATEGORY_MASK);
                blit.value = (u8)(blit.value & ~TILEMAP_PIXEL_CATEGORY_MASK);
            }
        }

        //-------------------------------------------------
        //  draw_common - draw a tilemap to the
        //  destination with clipping; pixels apply
        //  priority/priority_mask to the priority bitmap
        //-------------------------------------------------
        //template<class _BitmapClass>
        //void tilemap_t::draw_common(screen_device &screen, _BitmapClass &dest, const rectangle &cliprect, u32 flags, u8 priority, u8 priority_mask)
        void draw_common<_BitmapClass, _BitmapClass_PixelType, _BitmapClass_PixelType_OPS, _BitmapClass_PixelTypePointer>
        (
            screen_device screen,
            _BitmapClass dest,
            rectangle cliprect,
            u32 flags,
            u8 priority,
            u8 priority_mask
        )
            where _BitmapClass : bitmap_specific<_BitmapClass_PixelType, _BitmapClass_PixelType_OPS, _BitmapClass_PixelTypePointer>
            where _BitmapClass_PixelType_OPS : PixelType_operators, new()
            where _BitmapClass_PixelTypePointer : PointerU8
        {
            // skip if disabled
            if (!m_enable)
                return;


            g_profiler.start(profile_type.PROFILER_TILEMAP_DRAW);


            // configure the blit parameters based on the input parameters
            blit_parameters blit;
            configure_blit_parameters(out blit, screen.priority(), cliprect, flags, priority, priority_mask);
            assert(dest.cliprect().contains(cliprect));
            assert(screen.cliprect().contains(cliprect) || blit.tilemap_priority_code == 0xff00);

            // flush the dirty state to all tiles as appropriate
            realize_all_dirty_tiles();

            // flip the tilemap around the center of the visible area
            rectangle visarea = screen.visible_area();
            u32 xextent = (u32)(visarea.right() + visarea.left() + 1); // x0 + x1 + 1 for calculating horizontal centre as (x0 + x1 + 1) >> 1
            u32 yextent = (u32)(visarea.bottom() + visarea.top() + 1); // y0 + y1 + 1 for calculating vertical centre as (y0 + y1 + 1) >> 1

            // XY scrolling playfield
            if (m_scrollrows == 1 && m_scrollcols == 1)
            {
                // iterate to handle wraparound
                int scrollx = effective_rowscroll(0, xextent);
                int scrolly = effective_colscroll(0, yextent);
                for (int ypos = (int)(scrolly - m_height); ypos <= blit.cliprect.bottom(); ypos += (int)m_height)
                {
                    for (int xpos = (int)(scrollx - m_width); xpos <= blit.cliprect.right(); xpos += (int)m_width)
                        draw_instance<_BitmapClass, _BitmapClass_PixelType, _BitmapClass_PixelType_OPS, _BitmapClass_PixelTypePointer>(screen, dest, blit, xpos, ypos);
                }
            }

            // scrolling rows + vertical scroll
            else if (m_scrollcols == 1)
            {
                rectangle original_cliprect = blit.cliprect;

                // iterate over Y to handle wraparound
                int rowheight = (int)(m_height / m_scrollrows);
                int scrolly = effective_colscroll(0, yextent);
                for (int ypos = (int)(scrolly - m_height); ypos <= original_cliprect.bottom(); ypos += (int)m_height)
                {
                    int firstrow = std.max((original_cliprect.top() - ypos) / rowheight, 0);
                    int lastrow =  std.min((original_cliprect.bottom() - ypos) / rowheight, (int)m_scrollrows - 1);

                    // iterate over rows in the tilemap
                    int nextrow;
                    for (int currow = firstrow; currow <= lastrow; currow = nextrow)
                    {
                        // scan forward until we find a non-matching row
                        int scrollx = effective_rowscroll(currow, xextent);
                        for (nextrow = currow + 1; nextrow <= lastrow; nextrow++)
                            if (effective_rowscroll(nextrow, xextent) != scrollx)
                                break;

                        // skip if disabled
                        if ((u32)scrollx == TILE_LINE_DISABLED)
                            continue;

                        // update the cliprect just for this set of rows
                        blit.cliprect.sety(currow * rowheight + ypos, nextrow * rowheight - 1 + ypos);
                        blit.cliprect &= original_cliprect;

                        // iterate over X to handle wraparound
                        for (int xpos = (int)(scrollx - m_width); xpos <= original_cliprect.right(); xpos += (int)m_width)
                            draw_instance<_BitmapClass, _BitmapClass_PixelType, _BitmapClass_PixelType_OPS, _BitmapClass_PixelTypePointer>(screen, dest, blit, xpos, ypos);
                    }
                }
            }

            // scrolling columns + horizontal scroll
            else if (m_scrollrows == 1)
            {
                rectangle original_cliprect = blit.cliprect;

                // iterate over columns in the tilemap
                int scrollx = effective_rowscroll(0, xextent);
                int colwidth = (int)(m_width / m_scrollcols);
                int nextcol;
                for (int curcol = 0; curcol < m_scrollcols; curcol = nextcol)
                {
                    // scan forward until we find a non-matching column
                    int scrolly = effective_colscroll(curcol, yextent);
                    for (nextcol = curcol + 1; nextcol < m_scrollcols; nextcol++)
                    {
                        if (effective_colscroll(nextcol, yextent) != scrolly)
                            break;
                    }

                    // skip if disabled
                    if ((u32)scrolly == TILE_LINE_DISABLED)
                        continue;

                    // iterate over X to handle wraparound
                    for (int xpos = (int)(scrollx - m_width); xpos <= original_cliprect.right(); xpos += (int)m_width)
                    {
                        // update the cliprect just for this set of columns
                        blit.cliprect.setx(curcol * colwidth + xpos, nextcol * colwidth - 1 + xpos);
                        blit.cliprect &= original_cliprect;

                        // iterate over Y to handle wraparound
                        for (int ypos = (int)(scrolly - m_height); ypos <= original_cliprect.bottom(); ypos += (int)m_height)
                            draw_instance<_BitmapClass, _BitmapClass_PixelType, _BitmapClass_PixelType_OPS, _BitmapClass_PixelTypePointer>(screen, dest, blit, xpos, ypos);
                    }
                }
            }


            g_profiler.stop();
        }


        //template<class _BitmapClass> void draw_roz_common(screen_device &screen, _BitmapClass &dest, const rectangle &cliprect, UINT32 startx, UINT32 starty, int incxx, int incxy, int incyx, int incyy, bool wraparound, UINT32 flags, UINT8 priority, UINT8 priority_mask);


        //-------------------------------------------------
        //  draw_instance - draw a single instance of the
        //  tilemap to the internal pixmap at the given
        //  xpos,ypos
        //-------------------------------------------------
        //template<class _BitmapClass>
        //void tilemap_t::draw_instance(screen_device &screen, _BitmapClass &dest, const blit_parameters &blit, int xpos, int ypos)
        void draw_instance<_BitmapClass, _BitmapClass_PixelType, _BitmapClass_PixelType_OPS, _BitmapClass_PixelTypePointer>
        (
            screen_device screen,
            _BitmapClass dest,
            blit_parameters blit,
            int xpos,
            int ypos
        )
            where _BitmapClass : bitmap_specific<_BitmapClass_PixelType, _BitmapClass_PixelType_OPS, _BitmapClass_PixelTypePointer>
            where _BitmapClass_PixelType_OPS : PixelType_operators, new()
            where _BitmapClass_PixelTypePointer : PointerU8
        {
            // clip destination coordinates to the tilemap
            // note that x2/y2 are exclusive, not inclusive
            int x1 = std.max(xpos, blit.cliprect.left());
            int x2 = std.min(xpos + (int)m_width, blit.cliprect.right() + 1);
            int y1 = std.max(ypos, blit.cliprect.top());
            int y2 = std.min(ypos + (int)m_height, blit.cliprect.bottom() + 1);

            // if totally clipped, stop here
            if (x1 >= x2 || y1 >= y2)
                return;

            // look up priority and destination base addresses for y1
            bitmap_ind8 priority_bitmap = blit.priority;
            PointerU8 priority_baseaddr = null;  //u8 *priority_baseaddr = nullptr;
            int prio_rowpixels = 0;
            if (priority_bitmap.valid())
            {
                prio_rowpixels = priority_bitmap.rowpixels();
                priority_baseaddr = priority_bitmap.pix(y1, xpos);
            }
            else
            {
                assert((blit.tilemap_priority_code & 0xffff) == 0xff00);
            }

            //typename _BitmapClass::pixel_t *dest_baseaddr = NULL;
            PointerU8 dest_baseaddr8 = null;
            PointerU16 dest_baseaddr16 = null;
            PointerU32 dest_baseaddr32 = null;
            PointerU64 dest_baseaddr64 = null;

            int dest_rowpixels = 0;
            if (dest.valid())
            {
                dest_rowpixels = dest.rowpixels();

                //dest_baseaddr = dest.pix(y1, xpos);
                switch (dest.bpp())
                {
                    case 8:  dest_baseaddr8 = dest.pix8(y1, xpos); break;
                    case 16: dest_baseaddr16 = dest.pix16(y1, xpos); break;
                    case 32: dest_baseaddr32 = dest.pix32(y1, xpos); break;
                    case 64: dest_baseaddr64 = dest.pix64(y1, xpos); break;
                    default: throw new emu_fatalerror("draw_instance() - unknown bpp - {0}\n", dest.bpp());
                }
            }

            // convert screen coordinates to source tilemap coordinates
            x1 -= xpos;
            y1 -= ypos;
            x2 -= xpos;
            y2 -= ypos;

            // get tilemap pixels
            PointerU16 source_baseaddr = m_pixmap.pix(y1);  //const u16 *source_baseaddr = &m_pixmap.pix(y1);
            PointerU8 mask_baseaddr = m_flagsmap.pix(y1);  //const u8 *mask_baseaddr = &m_flagsmap.pix(y1);

            // get start/stop columns, rounding outward
            int mincol = (int)(x1 / m_tilewidth);
            int maxcol = (int)((x2 + m_tilewidth - 1) / m_tilewidth);

            // set up row counter
            int y = y1;
            int nexty = m_tileheight * (y1 / m_tileheight) + m_tileheight;
            nexty = std.min(nexty, y2);

            // loop over tilemap rows
            for (;;)
            {
                int row = (int)(y / m_tileheight);
                int x_start = x1;

                // iterate across the applicable tilemap columns
                trans_t prev_trans = trans_t.WHOLLY_TRANSPARENT;
                trans_t cur_trans;
                for (int column = mincol; column <= maxcol; column++)
                {
                    int x_end;

                    // we don't actually render the last column; it is always just used for flushing
                    if (column == maxcol)
                    {
                        cur_trans = trans_t.WHOLLY_TRANSPARENT;
                    }
                    // for other columns we look up the transparency information
                    else
                    {
                        tilemap_t_logical_index logindex = (tilemap_t_logical_index)(row * m_cols + column);

                        // if the current tile is dirty, fix it
                        if (m_tileflags[logindex] == TILE_FLAG_DIRTY)
                            tile_update(logindex, (u32)column, (u32)row);

                        // if the current summary data is non-zero, we must draw masked
                        if ((m_tileflags[logindex] & blit.mask) != 0)
                            cur_trans = trans_t.MASKED;
                        // otherwise, our transparency state is constant across the tile; fetch it
                        else
                            cur_trans = ((mask_baseaddr[column * m_tilewidth] & blit.mask) == blit.value) ? trans_t.WHOLLY_OPAQUE : trans_t.WHOLLY_TRANSPARENT;
                    }

                    // if the transparency state is the same as last time, don't render yet
                    if (cur_trans == prev_trans)
                        continue;

                    // compute the end of this run, in pixels
                    x_end = (int)(column * m_tilewidth);
                    x_end = std.max(x_end, x1);
                    x_end = std.min(x_end, x2);

                    // if we're rendering something, compute the pointers
                    MemoryContainer<rgb_t> clut = m_palette.palette().entry_list_adjusted();  //rgb_t *clut = m_palette.palette().entry_list_adjusted();
                    if (prev_trans != trans_t.WHOLLY_TRANSPARENT)
                    {
                        PointerU16 source0 = source_baseaddr + x_start;  //const u16 *source0 = source_baseaddr + x_start;

                        //typename _BitmapClass::pixel_t *dest0 = dest_baseaddr + x_start;
                        PointerU8 dest0_8 = null;
                        PointerU16 dest0_16 = null;
                        PointerU32 dest0_32 = null;
                        PointerU64 dest0_64 = null;
                        switch (dest.bpp())
                        {
                            case 8:  dest0_8 = dest_baseaddr8 + x_start; break;
                            case 16: dest0_16 = dest_baseaddr16 + x_start; break;
                            case 32: dest0_32 = dest_baseaddr32 + x_start; break;
                            case 64: dest0_64 = dest_baseaddr64 + x_start; break;
                            default: throw new emu_fatalerror("draw_instance() - unknown bpp - {0}\n", dest.bpp());
                        }

                        PointerU8 pmap0 = priority_baseaddr != null ? (priority_baseaddr + x_start) : null;  //u8 *pmap0 = priority_baseaddr ? (priority_baseaddr + x_start) : nullptr;

                        // if we were opaque, use the opaque renderer
                        if (prev_trans == trans_t.WHOLLY_OPAQUE)
                        {
                            for (int cury = y; cury < nexty; cury++)
                            {
                                if (dest_baseaddr8 == null && dest_baseaddr16 == null && dest_baseaddr32 == null && dest_baseaddr64 == null)  //if (dest_baseaddr == nullptr)
                                    scanline_draw_opaque_null(x_end - x_start, pmap0, blit.tilemap_priority_code);
                                else if (dest.bpp() == 16)  // else if (sizeof(*dest0) == 2)
                                    scanline_draw_opaque_ind16(dest0_16, source0, x_end - x_start, pmap0, blit.tilemap_priority_code);  //scanline_draw_opaque_ind16(reinterpret_cast<u16 *>(dest0), source0, x_end - x_start, pmap0, blit.tilemap_priority_code);
                                else if (dest.bpp() == 32 && blit.alpha >= 0xff)  // else if (sizeof(*dest0) == 4 && blit.alpha >= 0xff)
                                    scanline_draw_opaque_rgb32(dest0_32, source0, x_end - x_start, clut, pmap0, blit.tilemap_priority_code);  //scanline_draw_opaque_rgb32(reinterpret_cast<u32 *>(dest0), source0, x_end - x_start, clut, pmap0, blit.tilemap_priority_code);
                                else if (dest.bpp() == 32)  // else if (sizeof(*dest0) == 4)
                                    scanline_draw_opaque_rgb32_alpha(dest0_32, source0, x_end - x_start, clut, pmap0, blit.tilemap_priority_code, blit.alpha);  //scanline_draw_opaque_rgb32_alpha(reinterpret_cast<u32 *>(dest0), source0, x_end - x_start, clut, pmap0, blit.tilemap_priority_code, blit.alpha);

                                //dest0 += dest_rowpixels;
                                switch (dest.bpp())
                                {
                                    case 8:  dest0_8 += dest_rowpixels; break;
                                    case 16: dest0_16 += dest_rowpixels; break;
                                    case 32: dest0_32 += dest_rowpixels; break;
                                    case 64: dest0_64 += dest_rowpixels; break;
                                    default: throw new emu_fatalerror("draw_instance() - unknown bpp - {0}\n", dest.bpp());
                                }

                                source0 += m_pixmap.rowpixels();
                                pmap0 += prio_rowpixels;
                            }
                        }
                        // otherwise use the masked renderer
                        else
                        {
                            PointerU8 mask0 = mask_baseaddr + x_start;  //const u8 *mask0 = mask_baseaddr + x_start;

                            for (int cury = y; cury < nexty; cury++)
                            {
                                if (dest_baseaddr8 == null && dest_baseaddr16 == null && dest_baseaddr32 == null && dest_baseaddr64 == null)  //if (dest_baseaddr == nullptr)
                                    scanline_draw_masked_null(mask0, blit.mask, blit.value, x_end - x_start, pmap0, blit.tilemap_priority_code);
                                else if (dest.bpp() == 16)  // else if (sizeof(*dest0) == 2)
                                    scanline_draw_masked_ind16(dest0_16, source0, mask0, blit.mask, blit.value, x_end - x_start, pmap0, blit.tilemap_priority_code);  //scanline_draw_masked_ind16(reinterpret_cast<u16 *>(dest0), source0, mask0, blit.mask, blit.value, x_end - x_start, pmap0, blit.tilemap_priority_code);
                                else if (dest.bpp() == 32 && blit.alpha >= 0xff)  // else if (sizeof(*dest0) == 4 && blit.alpha >= 0xff)
                                    scanline_draw_masked_rgb32(dest0_32, source0, mask0, blit.mask, blit.value, x_end - x_start, clut, pmap0, blit.tilemap_priority_code);  //scanline_draw_masked_rgb32(reinterpret_cast<u32 *>(dest0), source0, mask0, blit.mask, blit.value, x_end - x_start, clut, pmap0, blit.tilemap_priority_code);
                                else if (dest.bpp() == 32)  // else if (sizeof(*dest0) == 4)
                                    scanline_draw_masked_rgb32_alpha(dest0_32, source0, mask0, blit.mask, blit.value, x_end - x_start, clut, pmap0, blit.tilemap_priority_code, blit.alpha);  //scanline_draw_masked_rgb32_alpha(reinterpret_cast<u32 *>(dest0), source0, mask0, blit.mask, blit.value, x_end - x_start, clut, pmap0, blit.tilemap_priority_code, blit.alpha);

                                //dest0 += dest_rowpixels;
                                switch (dest.bpp())
                                {
                                    case 8:  dest0_8 += dest_rowpixels; break;
                                    case 16: dest0_16 += dest_rowpixels; break;
                                    case 32: dest0_32 += dest_rowpixels; break;
                                    case 64: dest0_64 += dest_rowpixels; break;
                                    default: throw new emu_fatalerror("draw_instance() - unknown bpp - {0}\n", dest.bpp());
                                }

                                source0 += m_pixmap.rowpixels();
                                mask0 += m_flagsmap.rowpixels();
                                pmap0 += prio_rowpixels;
                            }
                        }
                    }

                    // the new start is the end
                    x_start = x_end;
                    prev_trans = cur_trans;
                }

                // if this was the last row, stop
                if (nexty == y2)
                    break;

                // advance to the next row on all our bitmaps
                priority_baseaddr += prio_rowpixels * (nexty - y);
                source_baseaddr += m_pixmap.rowpixels() * (nexty - y);
                mask_baseaddr += m_flagsmap.rowpixels() * (nexty - y);

                //dest_baseaddr += dest_rowpixels * (nexty - y);
                switch (dest.bpp())
                {
                    case 8:  dest_baseaddr8 += dest_rowpixels * (nexty - y); break;
                    case 16: dest_baseaddr16 += dest_rowpixels * (nexty - y); break;
                    case 32: dest_baseaddr32 += dest_rowpixels * (nexty - y); break;
                    case 64: dest_baseaddr64 += dest_rowpixels * (nexty - y); break;
                    default: throw new emu_fatalerror("draw_instance() - unknown bpp - {0}\n", dest.bpp());
                }

                // increment the Y counter
                y = nexty;
                nexty += (int)m_tileheight;
                nexty = std.min(nexty, y2);
            }
        }


        //template<class _BitmapClass> void draw_roz_core(screen_device &screen, _BitmapClass &destbitmap, const blit_parameters &blit, UINT32 startx, UINT32 starty, int incxx, int incxy, int incyx, int incyy, bool wraparound);
    }


    // ======================> tilemap_manager
    // tilemap manager
    public class tilemap_manager : IDisposable
    {
        // internal state
        running_machine m_machine;
        simple_list<tilemap_t> m_tilemap_list = new simple_list<tilemap_t>();
        int m_instance;


        // construction/destruction

        //-------------------------------------------------
        //  tilemap_manager - constructor
        //-------------------------------------------------
        public tilemap_manager(running_machine machine)
        {
            m_machine = machine;
            m_instance = 0;
        }


        ~tilemap_manager()
        {
            assert(m_isDisposed);  // can remove

            // detach all device tilemaps since they will be destroyed as subdevices elsewhere
            bool found = true;
            while (found)
            {
                found = false;
                foreach (var tmap in m_tilemap_list)
                {
                    if (tmap.device != null)
                    {
                        found = true;
                        m_tilemap_list.detach(tmap);
                        break;
                    }
                }
            }
        }


        bool m_isDisposed = false;
        public void Dispose()
        {
            // detach all device tilemaps since they will be destroyed as subdevices elsewhere
            bool found = true;
            while (found)
            {
                found = false;
                foreach (var tmap in m_tilemap_list)
                {
                    if (tmap.device != null)
                    {
                        found = true;
                        m_tilemap_list.detach(tmap);
                        break;
                    }
                }
            }

            m_isDisposed = true;
        }


        // getters
        public running_machine machine() { return m_machine; }


        // tilemap creation

        //template <typename T, typename U>
        public tilemap_t create(device_gfx_interface decoder, tilemap_get_info_delegate tile_get_info, tilemap_mapper_delegate mapper, u16 tilewidth, u16 tileheight, u32 cols, u32 rows)  //tilemap_t &create(device_gfx_interface &decoder, T &&tile_get_info, U &&mapper, u16 tilewidth, u16 tileheight, u32 cols, u32 rows)
        { return create(decoder, tile_get_info, mapper, tilewidth, tileheight, cols, rows, null); }
        public tilemap_t create(device_gfx_interface decoder, tilemap_get_info_delegate tile_get_info, tilemap_standard_mapper mapper, u16 tilewidth, u16 tileheight, u32 cols, u32 rows)  //tilemap_t &create(device_gfx_interface &decoder, T &&tile_get_info, U &&mapper, u16 tilewidth, u16 tileheight, u32 cols, u32 rows)
        { return create(decoder, tile_get_info, mapper, tilewidth, tileheight, cols, rows, null); }
        //template <typename T, typename U, class V>
        //std::enable_if_t<std::is_base_of<device_t, V>::value, tilemap_t &> create(device_gfx_interface &decoder, T &&tile_get_info, U &&mapper, u16 tilewidth, u16 tileheight, u32 cols, u32 rows, V &allocated)
        //{ return create(decoder, std::forward<T>(tile_get_info), std::forward<U>(mapper), tilewidth, tileheight, cols, rows, &static_cast<tilemap_t &>(allocated)); }


        // tilemap list information
        public tilemap_t find(int index) { return m_tilemap_list.find(index); }
        public int count() { return m_tilemap_list.count(); }


        // global operations on all tilemaps

        //void mark_all_dirty();

        //-------------------------------------------------
        //  set_flip_all - set a global flip for all the
        //  tilemaps
        //-------------------------------------------------
        public void set_flip_all(u32 attributes)
        {
            for (tilemap_t tmap = m_tilemap_list.first(); tmap != null; tmap = tmap.next())
                tmap.set_flip(attributes);
        }


        public tilemap_t create(device_gfx_interface decoder, tilemap_get_info_delegate tile_get_info, tilemap_mapper_delegate mapper, u16 tilewidth, u16 tileheight, u32 cols, u32 rows, tilemap_t allocated)
        {
            if (allocated == null)
                allocated = new tilemap_t(machine().root_device());

            return m_tilemap_list.append(allocated.init(this, decoder, tile_get_info, mapper, tilewidth, tileheight, cols, rows));
        }

        public tilemap_t create(device_gfx_interface decoder, tilemap_get_info_delegate tile_get_info, tilemap_standard_mapper mapper, u16 tilewidth, u16 tileheight, u32 cols, u32 rows, tilemap_t allocated)
        {
            if (allocated == null)
                allocated = new tilemap_t(machine().root_device());

            return m_tilemap_list.append(allocated.init(this, decoder, tile_get_info, mapper, tilewidth, tileheight, cols, rows));
        }


        // allocate an instance index
        public int alloc_instance() { return ++m_instance; }
    }


    // ======================> tilemap_device
    public class tilemap_device : device_t
                                  //tilemap_t
    {
        //DEFINE_DEVICE_TYPE(TILEMAP, tilemap_device, "tilemap", "Tilemap")
        public static readonly emu.detail.device_type_impl TILEMAP = DEFINE_DEVICE_TYPE("tilemap", "Tilemap", (type, mconfig, tag, owner, clock) => { return new tilemap_device(mconfig, tag, owner, clock); });


        tilemap_t m_tilemap_t;


        // devices
        required_device<gfxdecode_device> m_gfxdecode;

        // configuration state
        tilemap_get_info_delegate m_get_info;
        tilemap_standard_mapper m_standard_mapper;
        tilemap_mapper_delegate m_mapper;
        int m_bytes_per_entry;
        u16 m_tile_width;
        u16 m_tile_height;
        u32 m_num_columns;
        u32 m_num_rows;
        bool m_transparent_pen_set;
        pen_t m_transparent_pen;

        // optional memory info
        memory_array m_basemem = new memory_array();              // info about base memory
        memory_array m_extmem;               // info about extension memory


        // construction/destruction
        //-------------------------------------------------
        //  tilemap_device - constructor
        //-------------------------------------------------

        //template <typename T>
        //tilemap_device(const machine_config &mconfig, const char *tag, device_t *owner, T &&gfxtag, int entrybytes
        //    , u16 tilewidth, u16 tileheight, tilemap_standard_mapper mapper, u32 columns, u32 rows, pen_t transpen)
        //    : tilemap_device(mconfig, tag, owner, (u32)0)
        //{
        //    set_gfxdecode(std::forward<T>(gfxtag));
        //    set_bytes_per_entry(entrybytes);
        //    set_layout(mapper, columns, rows);
        //    set_tile_size(tilewidth, tileheight);
        //    set_configured_transparent_pen(transpen);
        //}

        //template <typename T>
        //tilemap_device(const machine_config &mconfig, const char *tag, device_t *owner, T &&gfxtag, int entrybytes
        //    , u16 tilewidth, u16 tileheight, tilemap_standard_mapper mapper, u32 columns, u32 rows)
        //    : tilemap_device(mconfig, tag, owner, (u32)0)
        //{
        //    set_gfxdecode(std::forward<T>(gfxtag));
        //    set_bytes_per_entry(entrybytes);
        //    set_layout(mapper, columns, rows);
        //    set_tile_size(tilewidth, tileheight);
        //}

        //template <typename T>
        //tilemap_device(const machine_config &mconfig, const char *tag, device_t *owner, T &&gfxtag, int entrybytes, u16 tilewidth, u16 tileheight)
        //    : tilemap_device(mconfig, tag, owner, (u32)0)
        //{
        //    set_gfxdecode(std::forward<T>(gfxtag));
        //    set_bytes_per_entry(entrybytes);
        //    set_tile_size(tilewidth, tileheight);
        //}


        tilemap_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : base(mconfig, TILEMAP, tag, owner, clock)
        {
            m_tilemap_t = new tilemap_t((device_t)this);  //tilemap_t(static_cast<device_t &>(*this))


            m_gfxdecode = new required_device<gfxdecode_device>(this, finder_base.DUMMY_TAG);
            m_get_info = null;  //, m_get_info(*this)
            m_standard_mapper = tilemap_standard_mapper.TILEMAP_STANDARD_COUNT;
            m_mapper = null;  //, m_mapper(*this)
            m_bytes_per_entry = 0;
            m_tile_width = 8;
            m_tile_height = 8;
            m_num_columns = 64;
            m_num_rows = 64;
            m_transparent_pen_set = false;
            m_transparent_pen = 0;
        }


        public void tilemap_device_after_ctor(string gfxtag, int entrybytes, u16 tilewidth, u16 tileheight, tilemap_standard_mapper mapper, u32 columns, u32 rows, pen_t transpen)
        {
            set_gfxdecode(gfxtag);
            set_bytes_per_entry(entrybytes);
            set_layout(mapper, columns, rows);
            set_tile_size(tilewidth, tileheight);
            set_configured_transparent_pen(transpen);
        }

        public void tilemap_device_after_ctor(string gfxtag, int entrybytes, u16 tilewidth, u16 tileheight, tilemap_standard_mapper mapper, u32 columns, u32 rows)
        {
            set_gfxdecode(gfxtag);
            set_bytes_per_entry(entrybytes);
            set_layout(mapper, columns, rows);
            set_tile_size(tilewidth, tileheight);
        }


        public tilemap_t tilemap { get { return m_tilemap_t; } }


        void set_gfxdecode(string tag) { m_gfxdecode.set_tag(tag); }  //template <typename T> void set_gfxdecode(T &&tag) { m_gfxdecode.set_tag(std::forward<T>(tag)); }

        void set_bytes_per_entry(int bpe) { m_bytes_per_entry = bpe; }


        public void set_info_callback(tilemap_get_info_delegate args)  //template <typename... T> void set_info_callback(T &&... args) { m_get_info.set(std::forward<T>(args)...); }
        {
            m_get_info = args;
        }


        void set_layout(tilemap_standard_mapper mapper, u32 columns, u32 rows)
        {
            assert(tilemap_standard_mapper.TILEMAP_STANDARD_COUNT > mapper);
            m_standard_mapper = mapper;
            m_num_columns = columns;
            m_num_rows = rows;
        }

        //template <typename F>
        //void set_layout(F &&callback, const char *name, u32 columns, u32 rows)
        //{
        //    m_standard_mapper = TILEMAP_STANDARD_COUNT;
        //    m_mapper.set(std::forward<F>(callback), name);
        //    m_num_columns = columns;
        //    m_num_rows = rows;
        //}
        //template <typename T, typename F>
        //void set_layout(T &&target, F &&callback, const char *name, u32 columns, u32 rows)
        //{
        //    m_standard_mapper = TILEMAP_STANDARD_COUNT;
        //    m_mapper.set(std::forward<T>(target), std::forward<F>(callback), name);
        //    m_num_columns = columns;
        //    m_num_rows = rows;
        //}

        void set_tile_size(u16 width, u16 height) { m_tile_width = width; m_tile_height = height; }
        void set_configured_transparent_pen(pen_t pen) { m_transparent_pen_set = true; m_transparent_pen = pen; }


        // getters
        //memory_array &basemem() { return m_basemem; }
        //memory_array &extmem() { return m_extmem; }


        // write handlers
        //void write8(offs_t offset, u8 data);


        public void write16(offs_t offset, u16 data, u16 mem_mask = u16.MaxValue)// ~0)
        {
            m_basemem.write16(offset, data, mem_mask);
            offset = offset * 2 / (offs_t)m_bytes_per_entry;
            m_tilemap_t.mark_tile_dirty(offset);
            if (m_bytes_per_entry < 2)
                m_tilemap_t.mark_tile_dirty(offset + 1);
        }


        //void write32(offs_t offset, u32 data, u32 mem_mask = ~0);
        //void write8_ext(offs_t offset, u8 data);
        //void write16_ext(offs_t offset, u16 data, u16 mem_mask = ~0);
        //void write32_ext(offs_t offset, u32 data, u32 mem_mask = ~0);


        // optional memory accessors
        public u32 basemem_read(offs_t offset) { return m_basemem.read((int)offset); }
        //u32 extmem_read(offs_t offset) { return m_extmem.read(offset); }
        //void basemem_write(offs_t offset, u32 data) { m_basemem.write(offset, data); mark_tile_dirty(offset); }
        //void extmem_write(offs_t offset, u32 data) { m_extmem.write(offset, data); mark_tile_dirty(offset); }


        // device-level overrides

        //-------------------------------------------------
        //  device_start: Start up the device
        //-------------------------------------------------
        protected override void device_start()
        {
            // check configuration
            if (m_get_info == null)  //if (m_get_info.isnull())
                throw new emu_fatalerror("Tilemap device '{0}' has no get info callback!", tag());
            if (m_standard_mapper == tilemap_standard_mapper.TILEMAP_STANDARD_COUNT && m_mapper == null)  //if (m_standard_mapper == TILEMAP_STANDARD_COUNT && m_mapper.isnull())
                throw new emu_fatalerror("Tilemap device '{0}' has no mapper callback!", tag());

            if (!m_gfxdecode.op0.started())
                throw new device_missing_dependencies();

            // bind our callbacks
            //m_get_info.resolve();
            //m_mapper.resolve();

            // allocate the tilemap
            if (m_standard_mapper == tilemap_standard_mapper.TILEMAP_STANDARD_COUNT)
                machine().tilemap().create(m_gfxdecode.op0, m_get_info, m_mapper, m_tile_width, m_tile_height, m_num_columns, m_num_rows, this.m_tilemap_t);
            else
                machine().tilemap().create(m_gfxdecode.op0, m_get_info, m_standard_mapper, m_tile_width, m_tile_height, m_num_columns, m_num_rows, this.m_tilemap_t);

            // find the memory, if present
            memory_share share = memshare(tag());
            if (share != null)
            {
                m_basemem.set(share, m_bytes_per_entry);

                // look for an extension entry
                string tag_ext = tag().append_("_ext");
                share = memshare(tag_ext);
                if (share != null)
                    m_extmem.set(share, m_bytes_per_entry);
            }

            // configure the device and set the pen
            if (m_transparent_pen_set)
                m_tilemap_t.set_transparent_pen(m_transparent_pen);
        }
    }


    static partial class tilemap_global
    {
        //**************************************************************************
        //  MACROS
        //**************************************************************************

        // macros to help form flags for tilemap_t::draw
        //#define TILEMAP_DRAW_CATEGORY(x)        (x)     // specify category to draw
        //#define TILEMAP_DRAW_ALPHA(x)           (TILEMAP_DRAW_ALPHA_FLAG | (rgb_t::clamp(x) << 24))

        // function definition for a get info callback
        //#define TILE_GET_INFO_MEMBER(_name)     void _name(tilemap_t &tilemap, tile_data &tileinfo, tilemap_memory_index tile_index)

        // function definition for a logical-to-memory mapper
        //#define TILEMAP_MAPPER_MEMBER(_name)    tilemap_memory_index _name(u32 col, u32 row, u32 num_cols, u32 num_rows)

        // Helpers for setting tile attributes in the TILE_GET_INFO callback:
        //   TILE_FLIP_YX assumes that flipy is in bit 1 and flipx is in bit 0
        //   TILE_FLIP_XY assumes that flipy is in bit 0 and flipx is in bit 1
        public static u8 TILE_FLIPYX(int yx) { return (u8)(yx & 3); }  //template <typename T> constexpr u8 TILE_FLIPYX(T yx) { return u8(yx & 3); }
        //template <typename T> constexpr u8 TILE_FLIPXY(T xy) { return u8(((xy & 2) >> 1) | ((xy & 1) << 1)); }


        public static tilemap_device TILEMAP<bool_Required>(machine_config mconfig, device_finder<tilemap_device, bool_Required> finder, string gfxtag, int entrybytes, u16 tilewidth, u16 tileheight, tilemap_standard_mapper mapper, u32 columns, u32 rows)
            where bool_Required : bool_const, new()
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, tilemap_device.TILEMAP, 0);
            device.tilemap_device_after_ctor(gfxtag, entrybytes, tilewidth, tileheight, mapper, columns, rows);
            return device;
        }
        public static tilemap_device TILEMAP<bool_Required>(machine_config mconfig, device_finder<tilemap_device, bool_Required> finder, string gfxtag, int entrybytes, u16 tilewidth, u16 tileheight, tilemap_standard_mapper mapper, u32 columns, u32 rows, pen_t transpen)
            where bool_Required : bool_const, new()
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, tilemap_device.TILEMAP, 0);
            device.tilemap_device_after_ctor(gfxtag, entrybytes, tilewidth, tileheight, mapper, columns, rows, transpen);
            return device;
        }
    }
}
