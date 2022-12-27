// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using endianness_t = mame.util.endianness;  //using endianness_t = util::endianness;
using indirect_pen_t = System.UInt16;  //typedef u16 indirect_pen_t;
using offs_t = System.UInt32;  //using offs_t = u32;
using optional_memory_region = mame.memory_region_finder<mame.bool_const_false>;  //using optional_memory_region = memory_region_finder<false>;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;

using static mame.cpp_global;
using static mame.device_global;
using static mame.emucore_global;
using static mame.emupal_global;
using static mame.palette_global;


namespace mame
{
    // ======================> raw_to_rgb_converter
    public class raw_to_rgb_converter
    {
        // helper function
        //typedef rgb_t (*raw_to_rgb_func)(u32 raw);
        public delegate rgb_t raw_to_rgb_func(u32 raw);


        // internal data
        int m_bytes_per_entry = 0;
        raw_to_rgb_func m_func = null;


        // constructor
        public raw_to_rgb_converter(int bytes_per_entry = 0, raw_to_rgb_func func = null)
        {
            m_bytes_per_entry = bytes_per_entry;
            m_func = func;
        }


        // getters
        public int bytes_per_entry() { return m_bytes_per_entry; }


        // helpers
        //rgb_t operator()(u32 raw) const { return (*m_func)(raw); }
        public rgb_t op(u32 raw) { return m_func(raw); }


        // generic raw-to-RGB conversion helpers
        //template<int RedBits, int GreenBits, int BlueBits, int RedShift, int GreenShift, int BlueShift>
        public static rgb_t standard_rgb_decoder<int_RedBits, int_GreenBits, int_BlueBits, int_RedShift, int_GreenShift, int_BlueShift>(u32 raw)
            where int_RedBits : int_const, new()
            where int_GreenBits : int_const, new()
            where int_BlueBits : int_const, new()
            where int_RedShift : int_const, new()
            where int_GreenShift : int_const, new()
            where int_BlueShift : int_const, new()
        {
            int RedShift = new int_RedShift().value;
            int GreenShift = new int_GreenShift().value;
            int BlueShift = new int_BlueShift().value;

            u8 r = palexpand<int_RedBits>((uint8_t)(raw >> RedShift));
            u8 g = palexpand<int_GreenBits>((uint8_t)(raw >> GreenShift));
            u8 b = palexpand<int_BlueBits>((uint8_t)(raw >> BlueShift));
            return new rgb_t(r, g, b);
        }


        // data-inverted generic raw-to-RGB conversion helpers
        //template<int RedBits, int GreenBits, int BlueBits, int RedShift, int GreenShift, int BlueShift>
        public static rgb_t inverted_rgb_decoder<int_RedBits, int_GreenBits, int_BlueBits, int_RedShift, int_GreenShift, int_BlueShift>(u32 raw)
            where int_RedBits : int_const, new()
            where int_GreenBits : int_const, new()
            where int_BlueBits : int_const, new()
            where int_RedShift : int_const, new()
            where int_GreenShift : int_const, new()
            where int_BlueShift : int_const, new()
        {
            int RedShift = new int_RedShift().value;
            int GreenShift = new int_GreenShift().value;
            int BlueShift = new int_BlueShift().value;

            u8 r = palexpand<int_RedBits>((uint8_t)(~raw >> RedShift));
            u8 g = palexpand<int_GreenBits>((uint8_t)(~raw >> GreenShift));
            u8 b = palexpand<int_BlueBits>((uint8_t)(~raw >> BlueShift));
            return new rgb_t(r, g, b);
        }

#if false
        template<int IntBits, int RedBits, int GreenBits, int BlueBits, int IntShift, int RedShift, int GreenShift, int BlueShift>
        static rgb_t standard_irgb_decoder(u32 raw)
        {
            u8 const i = palexpand<IntBits>(raw >> IntShift);
            u8 const r = (i * palexpand<RedBits>(raw >> RedShift)) >> 8;
            u8 const g = (i * palexpand<GreenBits>(raw >> GreenShift)) >> 8;
            u8 const b = (i * palexpand<BlueBits>(raw >> BlueShift)) >> 8;
            return rgb_t(r, g, b);
        }
#endif


        // other standard decoders
        //static rgb_t IRRRRRGGGGGBBBBB_decoder(u32 raw);
        //static rgb_t RRRRGGGGBBBBRGBx_decoder(u32 raw);  // bits 3/2/1 are LSb
        //static rgb_t xRGBRRRRGGGGBBBB_bit0_decoder(u32 raw);  // bits 14/13/12 are LSb
        //static rgb_t xRGBRRRRGGGGBBBB_bit4_decoder(u32 raw);  // bits 14/13/12 are MSb
        //static rgb_t xBGRBBBBGGGGRRRR_bit0_decoder(u32 raw);  // bits 12/13/14 are LSb
    }


    public class device_palette_interface_palette_device : device_palette_interface
    {
        palette_device m_palette_device;

        public device_palette_interface_palette_device(machine_config mconfig, palette_device palette_device)
            : base(mconfig, palette_device)
        {
            m_palette_device = palette_device;
        }


        protected override u32 palette_entries() { return m_palette_device.m_entries; }
        protected override u32 palette_indirect_entries() { return m_palette_device.m_indirect_entries; }
        protected override bool palette_shadows_enabled() { return m_palette_device.m_enable_shadows; }
        protected override bool palette_hilights_enabled() { return m_palette_device.m_enable_hilights; }
    }


    // ======================> palette_device
    public class palette_device : device_t
                                  //device_palette_interface
    {
        //DEFINE_DEVICE_TYPE(PALETTE, palette_device, "palette", "palette")
        public static readonly emu.detail.device_type_impl PALETTE = DEFINE_DEVICE_TYPE("palette", "palette", (type, mconfig, tag, owner, clock) => { return new palette_device(mconfig, tag, owner, clock); });


        //typedef device_delegate<void (palette_device &)> init_delegate;
        public delegate void init_delegate(palette_device device);


        // black-fill on start
        //enum black_t        { BLACK };

        // monochrome
        //enum mono_t         { MONOCHROME };
        //enum mono_inv_t     { MONOCHROME_INVERTED };
        //enum mono_hi_t      { MONOCHROME_HIGHLIGHT };

        // 3-bit (8-colour) - components here are LSB to MSB
        //enum rgb_3b_t       { RGB_3BIT };
        //enum rbg_3b_t       { RBG_3BIT };
        //enum grb_3b_t       { GRB_3BIT };
        //enum gbr_3b_t       { GBR_3BIT };
        //enum brg_3b_t       { BRG_3BIT };
        //enum bgr_3b_t       { BGR_3BIT };

        // 8-bit
        //enum rgb_332_t      { RGB_332, RRRGGGBB };
        //enum bgr_233_t      { BGR_233, BBGGGRRR };
        //enum rgb_332_inv_t  { RGB_332_inverted, RRRGGGBB_inverted };
        public enum bgr_233_inv_t  { BGR_233_inverted, BBGGGRRR_inverted }

        // 15-bit
        //enum rgb_555_t      { RGB_555, RRRRRGGGGGBBBBB };
        //enum grb_555_t      { GRB_555, GGGGGRRRRRBBBBB };
        //enum bgr_555_t      { BGR_555, BBBBBGGGGGRRRRR };

        // 16-bit
        //enum xrgb_333_t     { xRGB_333, xxxxxxxRRRGGGBBB };
        //enum xrbg_333_t     { xRBG_333, xxxxxxxRRRBBBGGG };
        //enum xbgr_333_t     { xBGR_333, xxxxxxxBBBGGGRRR };
        //enum xrgb_444_t     { xRGB_444, xxxxRRRRGGGGBBBB };
        public enum xrbg_444_t  { xRBG_444, xxxxRRRRBBBBGGGG }
        //enum xbrg_444_t     { xBRG_444, xxxxBBBBRRRRGGGG };
        //enum xbgr_444_t     { xBGR_444, xxxxBBBBGGGGRRRR };
        //enum rgbx_444_t     { RGBx_444, RRRRGGGGBBBBxxxx };
        //enum gbrx_444_t     { GBRx_444, GGGGBBBBRRRRxxxx };
        //enum irgb_4444_t    { IRGB_4444, IIIIRRRRGGGGBBBB };
        //enum rgbi_4444_t    { RGBI_4444, RRRRGGGGBBBBIIII };
        //enum ibgr_4444_t    { IBGR_4444, IIIIBBBBGGGGRRRR };
        //enum xrgb_555_t     { xRGB_555, xRRRRRGGGGGBBBBB };
        //enum xgrb_555_t     { xGRB_555, xGGGGGRRRRRBBBBB };
        //enum xgbr_555_t     { xGBR_555, xGGGGGBBBBBRRRRR };
        //enum xbrg_555_t     { xBRG_555, xBBBBBRRRRRGGGGG };
        //enum xbgr_555_t     { xBGR_555, xBBBBBGGGGGRRRRR };
        //enum rgbx_555_t     { RGBx_555, RRRRRGGGGGBBBBBx };
        //enum grbx_555_t     { GRBx_555, GGGGGRRRRRBBBBBx };
        //enum brgx_555_t     { BRGx_555, BBBBBRRRRRGGGGGx };
        //enum xrbg_inv_t     { xRBG_555_inverted, xRRRRRBBBBBGGGGG_inverted };
        //enum irgb_1555_t    { IRGB_1555, IRRRRRGGGGGBBBBB };
        //enum rgb_565_t      { RGB_565, RRRRRGGGGGGBBBBB };
        //enum bgr_565_t      { BGR_565, BBBBBGGGGGGBBBBB };

        // 32-bit
        //enum xrgb_888_t     { xRGB_888 };
        //enum xgrb_888_t     { xGRB_888 };
        //enum xbrg_888_t     { xBRG_888 };
        //enum xbgr_888_t     { xBGR_888 };
        //enum rgbx_888_t     { RGBx_888 };
        //enum grbx_888_t     { GRBx_888 };
        //enum bgrx_888_t     { BGRx_888 };

        // other standard formats
        //enum rgb_444_prom_t { RGB_444_PROMS, RRRRGGGGBBBB_PROMS };

        // exotic formats
        //enum rrrrggggbbbbrgbx_t      { RRRRGGGGBBBBRGBx };
        //enum xrgbrrrrggggbbbb_bit0_t { xRGBRRRRGGGGBBBB_bit0 };
        //enum xrgbrrrrggggbbbb_bit4_t { xRGBRRRRGGGGBBBB_bit4 };
        //enum xbgrbbbbggggrrrr_bit0_t { xBGRBBBBGGGGRRRR_bit0 };


        device_palette_interface_palette_device m_dipalette;


        // configuration state
        public u32 m_entries;              // number of entries in the palette
        public u32 m_indirect_entries;     // number of indirect colors in the palette
        public bool m_enable_shadows;       // are shadows enabled?
        public bool m_enable_hilights;      // are hilights enabled?
        int m_membits;              // width of palette RAM, if different from native
        bool m_membits_supplied;     // true if membits forced in static config
        endianness_t m_endianness;           // endianness of palette RAM, if different from native
        bool m_endianness_supplied;  // true if endianness forced in static config
        optional_memory_region m_prom_region;       // region where the color PROMs are
        init_delegate m_init;

        // palette RAM
        raw_to_rgb_converter m_raw_to_rgb;          // format of palette RAM
        memory_array m_paletteram = new memory_array();           // base memory
        memory_array m_paletteram_ext = new memory_array();       // extended memory


        // construction/destruction

        palette_device(machine_config mconfig, string tag, device_t owner, init_delegate init, u32 entries = 0U, u32 indirect = 0U)
            : this(mconfig, tag, owner, 0U)
        {
            set_entries(entries);
            set_indirect_entries(indirect);
            set_init(init);
        }

        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, black_t, u32 entries = 0U);
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, mono_t);
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, mono_inv_t);
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, mono_hi_t);
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, rgb_3b_t);
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, rbg_3b_t);
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, grb_3b_t);
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, gbr_3b_t);
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, brg_3b_t);
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, bgr_3b_t);
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, rgb_555_t);
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, grb_555_t);
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, bgr_555_t);
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, rgb_565_t);
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, bgr_565_t);

        public palette_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : base(mconfig, PALETTE, tag, owner, 0)
        {
            m_class_interfaces.Add(new device_palette_interface_palette_device(mconfig, this));  //device_palette_interface(mconfig, *this),


            m_dipalette = GetClassInterface<device_palette_interface_palette_device>();


            m_entries = 0;
            m_indirect_entries = 0;
            m_enable_shadows = false;
            m_enable_hilights = false;
            m_membits = 0;
            m_membits_supplied = false;
            m_endianness = ENDIANNESS_BIG;
            m_endianness_supplied = false;
            m_prom_region = new optional_memory_region(this, finder_base.DUMMY_TAG);
            m_init = null;
            m_raw_to_rgb = null;
        }

        //template <typename T>
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, rgb_444_prom_t, T &&region, u32 entries)
        //    : palette_device(mconfig, tag, owner, init_delegate(*this, FUNC(palette_device::palette_init_rgb_444_proms)), entries)
        //{
        //    set_prom_region(std::forward<T>(region));
        //}

        //template <typename F>
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, F &&init, std::enable_if_t<init_delegate::supports_callback<F>::value, const char *> name, u32 entries = 0U, u32 indirect = 0U)
        //    : palette_device(mconfig, tag, owner, 0U)
        //{ set_init(std::forward<F>(init), name).set_entries(entries, indirect); }
        //template <typename T, typename F>
        //palette_device(const machine_config &mconfig, const char *tag, device_t *owner, T &&devname, F &&init, std::enable_if_t<init_delegate::supports_callback<F>::value, const char *> name, u32 entries = 0U, u32 indirect = 0U)
        //    : palette_device(mconfig, tag, owner, 0U)
        //{ set_init(std::forward<T>(devname), std::forward<F>(init), name).set_entries(entries, indirect); }

        //template <class FunctionClass>
        palette_device(machine_config mconfig, string tag, device_t owner, init_delegate init, string name, u32 entries = 0U, u32 indirect = 0U)
            : this(mconfig, tag, owner, init, entries, indirect)
        { }


        public void palette_device_after_ctor(init_delegate init, u32 entries, u32 indirect)
        {
            set_entries(entries, indirect);
            set_init(init);
        }


        public device_palette_interface_palette_device dipalette { get { return m_dipalette; } }
        public palette_t palette() { return dipalette.palette(); }
        public Pointer<rgb_t> pens() { return dipalette.pens(); }
        public pen_t black_pen() { return dipalette.black_pen(); }
        public void set_pen_color(pen_t pen, rgb_t rgb) { dipalette.set_pen_color(pen, rgb); }
        public void set_pen_color(pen_t pen, u8 r, u8 g, u8 b) { dipalette.set_pen_color(pen, r, g, b); }
        public void set_pen_colors(pen_t color_base, std.vector<rgb_t> colors) { dipalette.set_pen_colors(color_base, colors); }
        public void set_indirect_color(int index, rgb_t rgb) { dipalette.set_indirect_color(index, rgb); }
        public void set_pen_indirect(pen_t pen, indirect_pen_t index) { dipalette.set_pen_indirect(pen, index); }
        public u32 transpen_mask(gfx_element gfx, u32 color, indirect_pen_t transcolor) { return dipalette.transpen_mask(gfx, color, transcolor); }


        // configuration
        public palette_device set_init(init_delegate init) { m_init = init; return this; }  //template <typename... T> palette_device &set_init(T &&... args) { m_init.set(std::forward<T>(args)...); return *this; }

        palette_device set_format(raw_to_rgb_converter raw_to_rgb) { m_raw_to_rgb = raw_to_rgb; return this; }

        public palette_device set_format(int bytes_per_entry, raw_to_rgb_converter.raw_to_rgb_func func, u32 entries)
        {
            set_format(new raw_to_rgb_converter(bytes_per_entry, func));
            set_entries(entries);
            return this;
        }

        //palette_device &set_format(rgb_332_t, u32 entries);
        //palette_device &set_format(bgr_233_t, u32 entries);
        //palette_device &set_format(rgb_332_inv_t, u32 entries);

        public palette_device set_format(bgr_233_inv_t _, u32 entries)
        {
            set_format(1, raw_to_rgb_converter.inverted_rgb_decoder<int_const_3,int_const_3,int_const_2, int_const_0,int_const_3,int_const_6>, entries);
            return this;
        }

        //palette_device &set_format(xrgb_333_t, u32 entries);
        //palette_device &set_format(xrbg_333_t, u32 entries);
        //palette_device &set_format(xbgr_333_t, u32 entries);
        //palette_device &set_format(xrgb_444_t, u32 entries);

        public palette_device set_format(xrbg_444_t _, u32 entries)
        {
            set_format(2, raw_to_rgb_converter.standard_rgb_decoder<int_const_4,int_const_4,int_const_4, int_const_8,int_const_0,int_const_4>, entries);
            return this;
        }

        //palette_device &set_format(xbrg_444_t, u32 entries);
        //palette_device &set_format(xbgr_444_t, u32 entries);
        //palette_device &set_format(rgbx_444_t, u32 entries);
        //palette_device &set_format(gbrx_444_t, u32 entries);
        //palette_device &set_format(irgb_4444_t, u32 entries);
        //palette_device &set_format(rgbi_4444_t, u32 entries);
        //palette_device &set_format(ibgr_4444_t, u32 entries);
        //palette_device &set_format(xrgb_555_t, u32 entries);
        //palette_device &set_format(xgrb_555_t, u32 entries);
        //palette_device &set_format(xgbr_555_t, u32 entries);
        //palette_device &set_format(xbrg_555_t, u32 entries);
        //palette_device &set_format(xbgr_555_t, u32 entries);
        //palette_device &set_format(rgbx_555_t, u32 entries);
        //palette_device &set_format(grbx_555_t, u32 entries);
        //palette_device &set_format(brgx_555_t, u32 entries);
        //palette_device &set_format(xrbg_inv_t, u32 entries);
        //palette_device &set_format(irgb_1555_t, u32 entries);
        //palette_device &set_format(rgb_565_t, u32 entries);
        //palette_device &set_format(bgr_565_t, u32 entries);
        //palette_device &set_format(xrgb_888_t, u32 entries);
        //palette_device &set_format(xgrb_888_t, u32 entries);
        //palette_device &set_format(xbrg_888_t, u32 entries);
        //palette_device &set_format(xbgr_888_t, u32 entries);
        //palette_device &set_format(rgbx_888_t, u32 entries);
        //palette_device &set_format(grbx_888_t, u32 entries);
        //palette_device &set_format(bgrx_888_t, u32 entries);
        //palette_device &set_format(rrrrggggbbbbrgbx_t, u32 entries);
        //palette_device &set_format(xrgbrrrrggggbbbb_bit0_t, u32 entries);
        //palette_device &set_format(xrgbrrrrggggbbbb_bit4_t, u32 entries);
        //palette_device &set_format(xbgrbbbbggggrrrr_bit0_t, u32 entries);
        //template <typename T> palette_device &set_format(T x, u32 entries, u32 indirect) { set_format(x, entries); set_indirect_entries(indirect); return *this; }
        //palette_device &set_membits(int membits) { m_membits = membits; m_membits_supplied = true; return *this; }
        //palette_device &set_endianness(endianness_t endianness) { m_endianness = endianness; m_endianness_supplied = true; return *this; }
        //palette_device &set_entries(u32 entries) { m_entries = entries; return *this; }
        //palette_device &set_entries(u32 entries, u32 indirect) { m_entries = entries; m_indirect_entries = indirect; return *this; }
        //palette_device &set_indirect_entries(u32 entries) { m_indirect_entries = entries; return *this; }
        public palette_device enable_shadows() { m_enable_shadows = true; return this; }
        //palette_device &enable_hilights() { m_enable_hilights = true; return *this; }
        //template <typename T> palette_device &set_prom_region(T &&region) { m_prom_region.set_tag(std::forward<T>(region)); return *this; }


        //void set_format(raw_to_rgb_converter raw_to_rgb) { m_raw_to_rgb = raw_to_rgb; }
        //void set_membits(int membits) { m_membits = membits; m_membits_supplied = true; }
        //void set_endianness(endianness_t endianness) { m_endianness = endianness; m_endianness_supplied = true; }

        public palette_device set_entries(u32 entries) { m_entries = entries; return this; }
        public palette_device set_entries(u32 entries, u32 indirect) { m_entries = entries; m_indirect_entries = indirect; return this; }
        public palette_device set_indirect_entries(u32 entries) { m_indirect_entries = entries; return this; }

        //void enable_shadows() { m_enable_shadows = true; }
        //void enable_hilights() { m_enable_hilights = true; }
        //void set_prom_region(const char *region) { m_prom_region.set_tag(region); }


        // palette RAM accessors
        public memory_array basemem() { return m_paletteram; }
        //memory_array &extmem() { return m_paletteram_ext; }


        // raw entry reading
        public u32 read_entry(pen_t pen)
        {
            u32 data = m_paletteram.read((int)pen);
            if (m_paletteram_ext.base_() != null)
                data |= m_paletteram_ext.read((int)pen) << (8 * m_paletteram.bytes_per_entry());
            return data;
        }


        // generic read/write handlers
        //u8 read8(offs_t offset);
        //u8 read8_ext(offs_t offset);


        public void write8(offs_t offset, u8 data)
        {
            m_paletteram.write8(offset, data);
            update_for_write(offset, 1);
        }


        //void write8_ext(offs_t offset, u8 data);
        //void write_indirect(offs_t offset, u8 data);
        //void write_indirect_ext(offs_t offset, u8 data);
        //u16 read16(offs_t offset);
        //u16 read16_ext(offs_t offset);


        public void write16(offs_t offset, u16 data, u16 mem_mask = u16.MaxValue)  //u16(~0))
        {
            m_paletteram.write16(offset, data, mem_mask);
            update_for_write(offset * 2, 2);
        }


        //void write16_ext(offs_t offset, u16 data, u16 mem_mask = u16(~0));
        //u32 read32(offs_t offset);
        //void write32(offs_t offset, u32 data, u32 mem_mask = u32(~0));


        // helper to update palette when data changed
        //void update() { if (!m_init.isnull()) m_init(*this); }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - start up the device
        //-------------------------------------------------
        protected override void device_start()
        {
            // bind the init function
            //m_init.resolve();

            // find the memory, if present
            memory_share share = memshare(tag());
            if (share != null)
            {
                // find the extended (split) memory, if present
                string tag_ext = tag() + "_ext";
                memory_share share_ext = memshare(tag_ext);

                // make sure we have specified a format
                if (m_raw_to_rgb.bytes_per_entry() <= 0)
                    throw new emu_fatalerror("palette_device({0}): Palette has memory share but no format specified", tag());

                // determine bytes per entry and configure
                int bytes_per_entry = m_raw_to_rgb.bytes_per_entry();
                if (share_ext == null)
                {
                    m_paletteram.set(share, bytes_per_entry);
                }
                else
                {
                    m_paletteram.set(share, bytes_per_entry / 2);
                    m_paletteram_ext.set(share_ext, bytes_per_entry / 2);
                }

                // override membits if provided
                if (m_membits_supplied)
                {
                    // forcing width only makes sense when narrower than the native bus width
                    if (m_membits >= share.bitwidth())
                        throw new emu_fatalerror("palette_device({0}): Improper use of MCFG_PALETTE_MEMBITS", tag());

                    m_paletteram.set_membits(m_membits);
                    if (share_ext != null)
                        m_paletteram_ext.set_membits(m_membits);
                }

                // override endianness if provided
                if (m_endianness_supplied)
                {
                    // forcing endianness only makes sense when the RAM is narrower than the palette format and not split
                    if (share_ext != null || (m_paletteram.membits() / 8) >= bytes_per_entry)
                        throw new emu_fatalerror("palette_device({0}): Improper use of MCFG_PALETTE_ENDIANNESS", tag());

                    m_paletteram.set_endianness(m_endianness);
                }
            }

            // call the initialization helper if present
            if (m_init != null)
                m_init(this);
        }


        // device_palette_interface overrides
        //protected override UInt32 palette_entries() { return m_entries; }
        //protected override UInt32 palette_indirect_entries() { return m_indirect_entries; }
        //protected override bool palette_shadows_enabled() { return m_enable_shadows; }
        //protected override bool palette_hilights_enabled() { return m_enable_hilights; }


        // generic palette init routines
        //void palette_init_all_black(palette_device &palette);
        //void palette_init_monochrome(palette_device &palette);
        //void palette_init_monochrome_inverted(palette_device &palette);
        //void palette_init_monochrome_highlight(palette_device &palette);
        //void palette_init_3bit_rgb(palette_device &palette);
        //void palette_init_3bit_rbg(palette_device &palette);
        //void palette_init_3bit_brg(palette_device &palette);
        //void palette_init_3bit_grb(palette_device &palette);
        //void palette_init_3bit_gbr(palette_device &palette);
        //void palette_init_3bit_bgr(palette_device &palette);
        //void palette_init_rgb_444_proms(palette_device &palette);
        //void palette_init_rgb_555(palette_device &palette);
        //void palette_init_grb_555(palette_device &palette);
        //void palette_init_bgr_555(palette_device &palette);
        //void palette_init_rgb_565(palette_device &palette);
        //void palette_init_bgr_565(palette_device &palette);


        //**************************************************************************
        //  GENERIC WRITE HANDLERS
        //**************************************************************************

        //-------------------------------------------------
        //  update_for_write - given a write of a given
        //  length to a given byte offset, update all
        //  potentially modified palette entries
        //-------------------------------------------------
        void update_for_write(offs_t byte_offset, int bytes_modified, bool indirect = false)
        {
            assert((m_indirect_entries != 0) == indirect);

            // determine how many entries were modified
            int bpe = m_paletteram.bytes_per_entry();
            assert(bpe != 0);
            int count = (bytes_modified + bpe - 1) / bpe;

            // for each entry modified, fetch the palette data and set the pen color or indirect color
            offs_t base_ = byte_offset / (offs_t)bpe;
            for (int index = 0; index < count; index++)
            {
                if (indirect)
                    set_indirect_color((int)base_ + index, m_raw_to_rgb.op(read_entry(base_ + (offs_t)index)));
                else
                    set_pen_color(base_ + (offs_t)index, m_raw_to_rgb.op(read_entry(base_ + (offs_t)index)));
            }
        }
    }


    static class emupal_global
    {
        public static palette_device PALETTE(machine_config mconfig, string tag) { return emu.detail.device_type_impl.op<palette_device>(mconfig, tag, palette_device.PALETTE, 0); }
        public static palette_device PALETTE<bool_Required>(machine_config mconfig, device_finder<palette_device, bool_Required> finder) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, palette_device.PALETTE, 0); }
        public static palette_device PALETTE<bool_Required>(machine_config mconfig, device_finder<palette_device, bool_Required> finder, palette_device.init_delegate init, u32 entries = 0U, u32 indirect = 0U)
            where bool_Required : bool_const, new()
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, palette_device.PALETTE, 0);
            device.palette_device_after_ctor(init, entries, indirect);
            return device;
        }
    }
}
