// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using indirect_pen_t = System.UInt16;  //typedef u16 indirect_pen_t;
using palette_interface_enumerator = mame.device_interface_enumerator<mame.device_palette_interface>;  //typedef device_interface_enumerator<device_palette_interface> palette_interface_enumerator;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using size_t = System.UInt64;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;

using static mame.cpp_global;
using static mame.emucore_global;
using static mame.palette_global;


namespace mame
{
    //typedef u16 indirect_pen_t;


    // ======================> device_palette_interface
    public abstract class device_palette_interface : device_interface
    {
        class shadow_table_data
        {
            public Pointer<pen_t> base_;  //pen_t *            base;               // pointer to the base of the table
            //s16                dr;                 // delta red value
            //s16                dg;                 // delta green value
            //s16                db;                 // delta blue value
            //bool               noclip;             // clip?
        }


        const int MAX_SHADOW_PRESETS = 4;

        const float PALETTE_DEFAULT_SHADOW_FACTOR = 0.6f;
        const float PALETTE_DEFAULT_HIGHLIGHT_FACTOR = 1.0f/PALETTE_DEFAULT_SHADOW_FACTOR;


        // internal state
        palette_t m_palette;              // the palette itself
        Pointer<rgb_t> m_pens = new Pointer<rgb_t>();  //const pen_t *       m_pens;                 // remapped palette pen numbers
        public bitmap_format m_format;               // format assumed for palette data
        Pointer<pen_t> m_shadow_table;  //pen_t *             m_shadow_table;         // table for looking up a shadowed pen
        u32 m_shadow_group;         // index of the shadow group, or 0 if none
        u32 m_hilight_group;        // index of the hilight group, or 0 if none
        pen_t m_white_pen;            // precomputed white pen value
        pen_t m_black_pen;            // precomputed black pen value

        // indirection state
        std.vector<rgb_t> m_indirect_colors = new std.vector<rgb_t>();          // actual colors set for indirection
        std.vector<indirect_pen_t> m_indirect_pens = new std.vector<indirect_pen_t>();   // indirection values

        shadow_table_data [] m_shadow_tables = new shadow_table_data [MAX_SHADOW_PRESETS]; // array of shadow table data

        std.vector<pen_t> m_save_pen = new std.vector<pen_t>();           // pens for save/restore
        std.vector<float> m_save_contrast = new std.vector<float>();      // brightness for save/restore

        std.vector<rgb_t> m_pen_array = new std.vector<rgb_t>();  // std::vector<pen_t> m_pen_array;
        std.vector<pen_t> m_shadow_array = new std.vector<pen_t>();
        std.vector<pen_t> m_hilight_array;


        // construction/destruction
        //-------------------------------------------------
        //  device_palette_interface - constructor
        //-------------------------------------------------
        public device_palette_interface(machine_config mconfig, device_t device)
            : base(device, "palette")
        {
            m_palette = null;
            m_pens = null;
            m_format = bitmap_format.BITMAP_FORMAT_RGB32;
            m_shadow_table = null;
            m_shadow_group = 0;
            m_hilight_group = 0;
            m_white_pen = 0;
            m_black_pen = 0;


            for (int i = 0; i < m_shadow_tables.Length; i++)
                m_shadow_tables[i] = new shadow_table_data();
        }


        // getters
        public u32 entries() { return palette_entries(); }
        public u32 indirect_entries() { return palette_indirect_entries(); }
        public palette_t palette() { return m_palette; }
        //const pen_t &pen(int index) const { return m_pens[index]; }
        public Pointer<rgb_t> pens() { return m_pens; }  //const pen_t *pens() const { return m_pens; }
        public Pointer<pen_t> shadow_table() { return new Pointer<pen_t>(m_shadow_table); }
        //rgb_t pen_color(pen_t pen) const { return m_palette->entry_color(pen); }
        //double pen_contrast(pen_t pen) const { return m_palette->entry_contrast(pen); }
        public pen_t black_pen() { return m_black_pen; }
        //pen_t white_pen() const { return m_white_pen; }
        //bool shadows_enabled() const { return palette_shadows_enabled(); }
        //bool hilights_enabled() const { return palette_hilights_enabled(); }


        // setters
        public void set_pen_color(pen_t pen, rgb_t rgb) { m_palette.entry_set_color(pen, rgb); }
        //void set_pen_red_level(pen_t pen, u8 level) { m_palette->entry_set_red_level(pen, level); }
        //void set_pen_green_level(pen_t pen, u8 level) { m_palette->entry_set_green_level(pen, level); }
        //void set_pen_blue_level(pen_t pen, u8 level) { m_palette->entry_set_blue_level(pen, level); }
        public void set_pen_color(pen_t pen, u8 r, u8 g, u8 b) { m_palette.entry_set_color(pen, new rgb_t(r, g, b)); }
        //void set_pen_colors(pen_t color_base, const rgb_t *colors, int color_count) { while (color_count--) set_pen_color(color_base++, *colors++); }
        //template <size_t N> void set_pen_colors(pen_t color_base, const rgb_t (&colors)[N]) { set_pen_colors(color_base, colors, N); }
        public void set_pen_colors(pen_t color_base, std.vector<rgb_t> colors) { for (unsigned i = 0; i != colors.size(); i++) set_pen_color(color_base + (pen_t)i, colors[i]); }
        //void set_pen_contrast(pen_t pen, double bright) { m_palette->entry_set_contrast(pen, bright); }
        public void set_format(bitmap_format format) { m_format = format; }

        // indirection (aka colortables)
        public indirect_pen_t pen_indirect(int index) { return m_indirect_pens[index]; }
        public rgb_t indirect_color(int index) { return m_indirect_colors[index]; }

        //-------------------------------------------------
        //  set_indirect_color - set an indirect color
        //-------------------------------------------------
        public void set_indirect_color(int index, rgb_t rgb)
        {
            // make sure we are in range
            assert(index < (int)m_indirect_colors.size());

            // alpha doesn't matter
            rgb.set_a(255);

            // update if it has changed
            if (m_indirect_colors[index] != rgb)
            {
                m_indirect_colors[index] = rgb;

                // update the palette for any colortable entries that reference it
                for (u32 pen = 0; pen < m_indirect_pens.size(); pen++)
                {
                    if (m_indirect_pens[(int)pen] == index)
                        m_palette.entry_set_color(pen, rgb);
                }
            }
        }

        //-------------------------------------------------
        //  set_pen_indirect - set an indirect pen index
        //-------------------------------------------------
        public void set_pen_indirect(pen_t pen, indirect_pen_t index)
        {
            // make sure we are in range
            assert(pen < entries() && index < indirect_entries());

            m_indirect_pens[(int)pen] = index;

            m_palette.entry_set_color(pen, m_indirect_colors[index]);
        }

        //-------------------------------------------------
        //  transpen_mask - return a mask of pens that
        //  whose indirect values match the given
        //  transcolor
        //-------------------------------------------------
        public u32 transpen_mask(gfx_element gfx, u32 color, indirect_pen_t transcolor)
        {
            u32 entry = gfx.colorbase() + (color % gfx.colors()) * gfx.granularity();

            // make sure we are in range
            assert(entry < m_indirect_pens.size());
            assert(gfx.depth() <= 32);

            // either gfx->color_depth entries or as many as we can get up until the end
            int count = (int)std.min((size_t)gfx.depth(), m_indirect_pens.size() - entry);

            // set a bit anywhere the transcolor matches
            u32 mask = 0;
            for (int bit = 0; bit < count; bit++)
            {
                if (m_indirect_pens[(int)entry++] == transcolor)
                    mask |= 1U << bit;
            }

            // return the final mask
            return mask;
        }


        // shadow config
        void set_shadow_factor(double factor) { assert(m_shadow_group != 0); m_palette.group_set_contrast(m_shadow_group, (float)factor); }
        void set_highlight_factor(double factor) { assert(m_hilight_group != 0); m_palette.group_set_contrast(m_hilight_group, (float)factor); }
        //void set_shadow_mode(int mode) { assert(mode >= 0 && mode < MAX_SHADOW_PRESETS); m_shadow_table = m_shadow_tables[mode].base; }


        // interface-level overrides

        //-------------------------------------------------
        //  interface_validity_check - validation for a
        //  device after the configuration has been
        //  constructed
        //-------------------------------------------------
        protected override void interface_validity_check(validity_checker valid)
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  interface_pre_start - work to be done prior to
        //  actually starting a device
        //-------------------------------------------------
        public override void interface_pre_start()
        {
            // allocate the palette
            u32 numentries = palette_entries();
            allocate_palette(numentries);
            allocate_color_tables();
            allocate_shadow_tables();

            // allocate indirection tables
            int indirect_colors = (int)palette_indirect_entries();
            if (indirect_colors > 0)
            {
                m_indirect_colors.resize((size_t)indirect_colors);
                for (int color = 0; color < indirect_colors; color++)
                {
                    // alpha = 0 ensures change is detected the first time set_indirect_color() is called
                    m_indirect_colors[color] = rgb_t.transparent();
                }

                m_indirect_pens.resize(numentries);
                for (int pen = 0; pen < numentries; pen++)
                    m_indirect_pens[pen] = (indirect_pen_t)(pen % indirect_colors);
            }
        }


        //-------------------------------------------------
        //  interface_post_start - work to be done after
        //  actually starting a device
        //-------------------------------------------------
        public override void interface_post_start()
        {
            // set up save/restore of the palette
            m_save_pen.resize((size_t)m_palette.num_colors());
            m_save_contrast.resize((size_t)m_palette.num_colors());
            device().save_item(NAME(new { m_save_pen }));
            device().save_item(NAME(new { m_save_contrast }));

            // save indirection tables if we have them
            if (m_indirect_colors.size() > 0)
            {
                device().save_item(NAME(new { m_indirect_colors }));
                device().save_item(NAME(new { m_indirect_pens }));
            }
        }


        //-------------------------------------------------
        //  interface_pre_save - prepare the save arrays
        //  for saving
        //-------------------------------------------------
        public override void interface_pre_save()
        {
            throw new emu_unimplemented();
        }

        //-------------------------------------------------
        //  interface_post_load - called after restore to
        //  actually update the palette
        //-------------------------------------------------
        public override void interface_post_load()
        {
            throw new emu_unimplemented();
        }

        //-------------------------------------------------
        //  interface_post_stop - final cleanup
        //-------------------------------------------------
        public override void interface_post_stop()
        {
            // dereference the palette
            if (m_palette != null)
                m_palette.deref();
        }


        // configuration-related overrides
        protected abstract u32 palette_entries();

        protected virtual u32 palette_indirect_entries() { return 0; }
        protected virtual bool palette_shadows_enabled() { return false; }
        protected virtual bool palette_hilights_enabled() { return false; }


        // internal helpers

        //-------------------------------------------------
        //  allocate_palette - allocate and configure the
        //  palette object itself
        //-------------------------------------------------
        void allocate_palette(u32 numentries)
        {
            assert(numentries > 0);

            // determine the number of groups we need
            int numgroups = 1;
            if (palette_shadows_enabled())
                m_shadow_group = (u32)numgroups++;
            if (palette_hilights_enabled())
                m_hilight_group = (u32)numgroups++;
            if (numentries * numgroups > 65536)
                throw new emu_fatalerror("{0}({1}): Palette has more than 65536 colors.", device().shortname(), device().tag());

            // allocate a palette object containing all the colors and groups
            m_palette = palette_t.alloc(numentries, (uint32_t)numgroups);

            // configure the groups
            if (m_shadow_group != 0)
                set_shadow_factor(PALETTE_DEFAULT_SHADOW_FACTOR);
            if (m_hilight_group != 0)
                set_highlight_factor(PALETTE_DEFAULT_HIGHLIGHT_FACTOR);

            // set the initial colors to a standard rainbow
            for (int index = 0; index < numentries; index++)
                set_pen_color((pen_t)index, rgbexpand<int_const_1, int_const_1, int_const_1>((uint32_t)index, 0, 1, 2));

            // switch off the color mode
            switch (m_format)
            {
                // 16-bit paletteized case
                case bitmap_format.BITMAP_FORMAT_IND16:
                    m_black_pen = m_palette.black_entry();
                    m_white_pen = m_palette.white_entry();
                    if (m_black_pen >= 65536)
                        m_black_pen = 0;
                    if (m_white_pen >= 65536)
                        m_white_pen = 65535;
                    break;

                // 32-bit direct case
                case bitmap_format.BITMAP_FORMAT_RGB32:
                    m_black_pen = rgb_t.black();
                    m_white_pen = rgb_t.white();
                    break;

                // screenless case
                case bitmap_format.BITMAP_FORMAT_INVALID:
                default:
                    break;
            }
        }


        //-------------------------------------------------
        //  allocate_color_tables - allocate memory for
        //  pen and color tables
        //-------------------------------------------------
        void allocate_color_tables()
        {
            int total_colors = m_palette.num_colors() * m_palette.num_groups();

            // allocate memory for the pen table
            switch (m_format)
            {
                case bitmap_format.BITMAP_FORMAT_IND16:
                    // create a dummy 1:1 mapping
                    {
                        m_pen_array.resize((size_t)total_colors + 2);
                        Pointer<rgb_t> pentable = new Pointer<rgb_t>(m_pen_array);  //pen_t *pentable = &m_pen_array[0];
                        m_pens = new Pointer<rgb_t>(m_pen_array);  //m_pens = &m_pen_array[0];
                        for (int i = 0; i < total_colors + 2; i++)
                            pentable[i] = new rgb_t((uint32_t)i);  //pentable[i] = i;
                    }
                    break;

                case bitmap_format.BITMAP_FORMAT_RGB32:
                    m_pens = new Pointer<rgb_t>(m_palette.entry_list_adjusted());  // reinterpret_cast<const pen_t *>(m_palette.entry_list_adjusted());
                    break;

                default:
                    m_pens = null;
                    break;
            }
        }


        //-------------------------------------------------
        //  allocate_shadow_tables - allocate memory for
        //  shadow tables
        //-------------------------------------------------
        void allocate_shadow_tables()
        {
            int numentries = m_palette.num_colors();

            // if we have shadows, allocate shadow tables
            if (m_shadow_group != 0)
            {
                m_shadow_array.resize(65536);

                // palettized mode gets a single 64k table in slots 0 and 2
                if (m_format == bitmap_format.BITMAP_FORMAT_IND16)
                {
                    m_shadow_tables[2].base_ = new Pointer<pen_t>(m_shadow_array, 0);  //m_shadow_tables[0].base = m_shadow_tables[2].base = &m_shadow_array[0];
                    m_shadow_tables[0].base_ = new Pointer<pen_t>(m_shadow_array, 0);
                    for (int i = 0; i < 65536; i++)
                        m_shadow_array[i] = (i < numentries) ? (pen_t)(i + numentries) : (pen_t)i;
                }

                // RGB mode gets two 32k tables in slots 0 and 2
                else
                {
                    m_shadow_tables[0].base_ = new Pointer<pen_t>(m_shadow_array, 0);
                    m_shadow_tables[2].base_ = new Pointer<pen_t>(m_shadow_array, 32768);
                    configure_rgb_shadows(0, PALETTE_DEFAULT_SHADOW_FACTOR);
                }
            }

            // if we have hilights, allocate shadow tables
            if (m_hilight_group != 0)
            {
                m_hilight_array.resize(65536);

                // palettized mode gets a single 64k table in slots 1 and 3
                if (m_format == bitmap_format.BITMAP_FORMAT_IND16)
                {
                    m_shadow_tables[3].base_ = new Pointer<pen_t>(m_hilight_array, 0);  //m_shadow_tables[1].base_ = m_shadow_tables[3].base_ = &m_hilight_array[0];
                    m_shadow_tables[1].base_ = new Pointer<pen_t>(m_hilight_array, 0);
                    for (int i = 0; i < 65536; i++)
                        m_hilight_array[i] = (i < numentries) ? (pen_t)(i + 2 * numentries) : (pen_t)i;
                }

                // RGB mode gets two 32k tables in slots 1 and 3
                else
                {
                    m_shadow_tables[1].base_ = new Pointer<pen_t>(m_hilight_array, 0);
                    m_shadow_tables[3].base_ = new Pointer<pen_t>(m_hilight_array, 32768);
                    configure_rgb_shadows(1, PALETTE_DEFAULT_HIGHLIGHT_FACTOR);
                }
            }

            // set the default table
            m_shadow_table = m_shadow_tables[0].base_ != null ? new Pointer<pen_t>(m_shadow_tables[0].base_) : null;
        }


        //void set_shadow_dRGB32(int mode, int dr, int dg, int db, bool noclip);


        //-------------------------------------------------
        //  configure_rgb_shadows - configure shadows
        //  for the RGB tables
        //-------------------------------------------------
        void configure_rgb_shadows(int mode, float factor)
        {
            // only applies to RGB direct modes
            assert(m_format != bitmap_format.BITMAP_FORMAT_IND16);

            // verify the shadow table
            assert(mode >= 0 && mode < (int)std.size(m_shadow_tables));
            shadow_table_data stable = m_shadow_tables[mode];
            assert(stable.base_ != null);

            // regenerate the table
            int ifactor = (int)(factor * 256.0f);
            for (int rgb555 = 0; rgb555 < 32768; rgb555++)
            {
                u8 r  = rgb_t.clamp((pal5bit((uint8_t)(rgb555 >> 10)) * ifactor) >> 8);
                u8 gr = rgb_t.clamp((pal5bit((uint8_t)(rgb555 >> 5)) * ifactor) >> 8);
                u8 b  = rgb_t.clamp((pal5bit((uint8_t)(rgb555 >> 0)) * ifactor) >> 8);

                // store either 16 or 32 bit
                rgb_t final = new rgb_t(r, gr, b);
                if (m_format == bitmap_format.BITMAP_FORMAT_RGB32)
                    stable.base_[rgb555] = final;
                else
                    stable.base_[rgb555] = final.as_rgb15();
            }
        }
    }


    // interface type iterator
    //typedef device_interface_enumerator<device_palette_interface> palette_interface_enumerator;
}
