// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using indirect_pen_t = System.UInt16;
using pen_t = System.UInt32;
using u32 = System.UInt32;


namespace mame
{
    //typedef u16 indirect_pen_t;


    //typedef device_delegate<void (palette_device &)> palette_init_delegate;
    public delegate void palette_init_delegate(palette_device device);


    public static class emupal_global
    {
        //**************************************************************************
        //  DEVICE CONFIGURATION MACROS
        //**************************************************************************

        public static void MCFG_PALETTE_ADD(out device_t device, machine_config config, device_t owner, string tag, u32 entries)
        {
            mconfig_global.MCFG_DEVICE_ADD(out device, config, owner, tag, palette_device.PALETTE, 0);//entries);
            ((palette_device)device).palette_device_after_ctor(entries);
        }
        public static void MCFG_PALETTE_ADD(out device_t device, machine_config config, device_t owner, device_finder<palette_device> finder, u32 entries)
        {
            var target = finder.finder_target();  //std::pair<device_t &, char const *> const target(finder.finder_target());
            mconfig_global.MCFG_DEVICE_ADD(out device, config, owner, target.second(), palette_device.PALETTE, 0);//entries);
            finder.target = (palette_device)device;
            ((palette_device)device).palette_device_after_ctor(entries);
        }
        //define MCFG_PALETTE_ADD_INIT_BLACK(_tag, _entries)             MCFG_PALETTE_ADD(_tag, _entries)             palette_device::static_set_init(*device, palette_init_delegate(FUNC(palette_device::palette_init_all_black), downcast<palette_device *>(device)));

        //#define MCFG_PALETTE_MODIFY MCFG_DEVICE_MODIFY


        public static void MCFG_PALETTE_INIT_OWNER(device_t device, palette_init_delegate method) { ((palette_device)device).set_init(method); }  //palette_init_delegate(&_class::PALETTE_INIT_NAME(_method), #_class "::palette_init_" #_method, downcast<_class *>(owner)));
        //define MCFG_PALETTE_INIT_DEVICE(_tag, _class, _method)             palette_device::static_set_init(*device, palette_init_delegate(&_class::PALETTE_INIT_NAME(_method), #_class "::palette_init_" #_method, _tag));

        //define MCFG_PALETTE_FORMAT(_format)             palette_device::static_set_format(*device, PALETTE_FORMAT_##_format);
        //define MCFG_PALETTE_FORMAT_CLASS(_bytes_per_entry, _class, _method)             palette_device::static_set_format(*device, raw_to_rgb_converter(_bytes_per_entry, &_class::PALETTE_DECODER_NAME(_method)));
        //define MCFG_PALETTE_MEMBITS(_width)             palette_device::static_set_membits(*device, _width);
        //define MCFG_PALETTE_ENDIANNESS(_endianness)             palette_device::static_set_endianness(*device, _endianness);
        static void MCFG_PALETTE_ENTRIES(device_t device, u32 entries) { ((palette_device)device).set_entries(entries); }
        public static void MCFG_PALETTE_INDIRECT_ENTRIES(device_t device, u32 entries) { ((palette_device)device).set_indirect_entries(entries); }
        //define MCFG_PALETTE_ENABLE_SHADOWS()             palette_device::static_enable_shadows(*device);
        //define MCFG_PALETTE_ENABLE_HILIGHTS()             palette_device::static_enable_hilights(*device);


        // monochrome palettes
        //#define MCFG_PALETTE_ADD_MONOCHROME(_tag)             MCFG_PALETTE_ADD(_tag, 2)             palette_device::static_set_init(*device, palette_init_delegate(FUNC(palette_device::palette_init_monochrome), downcast<palette_device *>(device)));
        //#define MCFG_PALETTE_ADD_MONOCHROME_INVERTED(_tag)             MCFG_PALETTE_ADD(_tag, 2)             palette_device::static_set_init(*device, palette_init_delegate(FUNC(palette_device::palette_init_monochrome_inverted), downcast<palette_device *>(device)));
        //#define MCFG_PALETTE_ADD_MONOCHROME_HIGHLIGHT(_tag)             MCFG_PALETTE_ADD(_tag, 3)             palette_device::static_set_init(*device, palette_init_delegate(FUNC(palette_device::palette_init_monochrome_highlight), downcast<palette_device *>(device)));

        // 8-bit palettes
        //#define MCFG_PALETTE_ADD_3BIT_RGB(_tag)             MCFG_PALETTE_ADD(_tag, 8)             palette_device::static_set_init(*device, palette_init_delegate(FUNC(palette_device::palette_init_3bit_rgb), downcast<palette_device *>(device)));
        //#define MCFG_PALETTE_ADD_3BIT_RBG(_tag)             MCFG_PALETTE_ADD(_tag, 8)             palette_device::static_set_init(*device, palette_init_delegate(FUNC(palette_device::palette_init_3bit_rbg), downcast<palette_device *>(device)));
        //#define MCFG_PALETTE_ADD_3BIT_BRG(_tag)             MCFG_PALETTE_ADD(_tag, 8)             palette_device::static_set_init(*device, palette_init_delegate(FUNC(palette_device::palette_init_3bit_brg), downcast<palette_device *>(device)));
        //#define MCFG_PALETTE_ADD_3BIT_GRB(_tag)             MCFG_PALETTE_ADD(_tag, 8)             palette_device::static_set_init(*device, palette_init_delegate(FUNC(palette_device::palette_init_3bit_grb), downcast<palette_device *>(device)));
        //#define MCFG_PALETTE_ADD_3BIT_GBR(_tag)             MCFG_PALETTE_ADD(_tag, 8)             palette_device::static_set_init(*device, palette_init_delegate(FUNC(palette_device::palette_init_3bit_gbr), downcast<palette_device *>(device)));
        //#define MCFG_PALETTE_ADD_3BIT_BGR(_tag)             MCFG_PALETTE_ADD(_tag, 8)             palette_device::static_set_init(*device, palette_init_delegate(FUNC(palette_device::palette_init_3bit_bgr), downcast<palette_device *>(device)));

        // 15-bit palettes
        //define MCFG_PALETTE_ADD_RRRRRGGGGGBBBBB(_tag)             MCFG_PALETTE_ADD(_tag, 32768)             palette_device::static_set_init(*device, palette_init_delegate(FUNC(palette_device::palette_init_RRRRRGGGGGBBBBB), downcast<palette_device *>(device)));

        // 16-bit palettes
        //define MCFG_PALETTE_ADD_BBBBBGGGGGRRRRR(_tag)             MCFG_PALETTE_ADD(_tag, 32768)             palette_device::static_set_init(*device, palette_init_delegate(FUNC(palette_device::palette_init_BBBBBGGGGGRRRRR), downcast<palette_device *>(device)));
        //define MCFG_PALETTE_ADD_RRRRRGGGGGGBBBBB(_tag)             MCFG_PALETTE_ADD(_tag, 65536)             palette_device::static_set_init(*device, palette_init_delegate(FUNC(palette_device::palette_init_RRRRRGGGGGGBBBBB), downcast<palette_device *>(device)));
        //#define MCFG_PALETTE_ADD_BBBBBGGGGGGRRRRR(_tag)     MCFG_PALETTE_ADD(_tag, 65536)     downcast<palette_device &>(*device).set_init(palette_init_delegate(FUNC(palette_device::palette_init_BBBBBGGGGGGRRRRR), downcast<palette_device *>(device)));


        // other standard palettes
        //#define MCFG_PALETTE_ADD_RRRRGGGGBBBB_PROMS(_tag, _region, _entries)             MCFG_PALETTE_ADD(_tag, _entries)             downcast<palette_device &>(*device).set_prom_region(_region);             downcast<palette_device &>(*device).set_init(palette_init_delegate(FUNC(palette_device::palette_init_RRRRGGGGBBBB_proms), downcast<palette_device *>(device)));

        // not implemented yet
        //define MCFG_PALETTE_ADD_HARDCODED(_tag, _array)             MCFG_PALETTE_ADD(_tag, sizeof(_array) / 3)             palette_device::static_set_init(*device, palette_init_delegate(FUNC(palette_device::palette_init_RRRRGGGGBBBB_proms), downcast<palette_device *>(device)));
    }


    // ======================> raw_to_rgb_converter
    class raw_to_rgb_converter
    {
        // helper function
        //typedef rgb_t (*raw_to_rgb_func)(UINT32 raw);
        public delegate rgb_t raw_to_rgb_func(UInt32 raw);


        // internal data
        int m_bytes_per_entry;
        raw_to_rgb_func m_func;


        // constructor
        public raw_to_rgb_converter(int bytes_per_entry = 0, raw_to_rgb_func func = null)
        {
            m_bytes_per_entry = bytes_per_entry;
            m_func = func;
        }


        // getters
        public int bytes_per_entry() { return m_bytes_per_entry; }


        // helpers
        //rgb_t operator()(UINT32 raw) const { return (*m_func)(raw); }


#if false
        // generic raw-to-RGB conversion helpers
        template<int RedBits, int GreenBits, int BlueBits, int RedShift, int GreenShift, int BlueShift>
        static rgb_t standard_rgb_decoder(u32 raw)
        {
            u8 const r = palexpand<RedBits>(raw >> RedShift);
            u8 const g = palexpand<GreenBits>(raw >> GreenShift);
            u8 const b = palexpand<BlueBits>(raw >> BlueShift);
            return rgb_t(r, g, b);
        }

        // data-inverted generic raw-to-RGB conversion helpers
        template<int RedBits, int GreenBits, int BlueBits, int RedShift, int GreenShift, int BlueShift>
        static rgb_t inverted_rgb_decoder(u32 raw)
        {
            u8 const r = palexpand<RedBits>(~raw >> RedShift);
            u8 const g = palexpand<GreenBits>(~raw >> GreenShift);
            u8 const b = palexpand<BlueBits>(~raw >> BlueShift);
            return rgb_t(r, g, b);
        }

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
        //static rgb_t IRRRRRGGGGGBBBBB_decoder(UINT32 raw);
        //static rgb_t RRRRGGGGBBBBRGBx_decoder(UINT32 raw);  // bits 3/2/1 are LSb
        //static rgb_t xRGBRRRRGGGGBBBB_bit0_decoder(UINT32 raw);  // bits 14/13/12 are LSb
        //static rgb_t xRGBRRRRGGGGBBBB_bit4_decoder(UINT32 raw);  // bits 14/13/12 are MSb
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
        static device_t device_creator_palette_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new palette_device(mconfig, tag, owner); }
        public static readonly device_type PALETTE = DEFINE_DEVICE_TYPE(device_creator_palette_device, "palette", "palette");


        device_palette_interface_palette_device m_device_palette_interface;


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
        palette_init_delegate m_init;

        // palette RAM
        raw_to_rgb_converter m_raw_to_rgb;          // format of palette RAM
        memory_array m_paletteram;           // base memory
        memory_array m_paletteram_ext;       // extended memory


        public device_palette_interface_palette_device device_palette_interface { get { return m_device_palette_interface; } }


        // construction/destruction
        public palette_device(machine_config mconfig, string tag, device_t owner)//, u32 entries)
            : base(mconfig, PALETTE, tag, owner, 0)
        {
            //device_palette_interface(mconfig, *this),
            m_class_interfaces.Add(new device_palette_interface_palette_device(mconfig, this));


            m_device_palette_interface = GetClassInterface<device_palette_interface_palette_device>();


            m_entries = 0;
            m_indirect_entries = 0;
            m_enable_shadows = false;
            m_enable_hilights = false;
            m_membits = 0;
            m_membits_supplied = false;
            m_endianness = endianness_t.ENDIANNESS_BIG;
            m_endianness_supplied = false;
            m_prom_region = new optional_memory_region(this, finder_base.DUMMY_TAG);
            m_init = null;
            m_raw_to_rgb = null;

            //set_entries(entries);
        }

        public void palette_device_after_ctor(u32 entries)
        {
            set_entries(entries);
        }


        // configuration
        public void set_init(palette_init_delegate init) { m_init = init; }  //template <typename Object> void set_init(Object &&init) { m_init = std::forward<Object>(init); }
        //void set_init(palette_init_delegate callback) { m_init = callback; }
        //template <class FunctionClass> void set_init(const char *devname, void (FunctionClass::*callback)(palette_device &), const char *name)
        //{
        //    set_init(palette_init_delegate(callback, name, devname, static_cast<FunctionClass *>(nullptr)));
        //}
        public void set_init(string devname, palette_init_delegate init) { set_init(init); }
        //template <class FunctionClass> void set_init(void (FunctionClass::*callback)(palette_device &), const char *name)
        //{
        //    set_init(palette_init_delegate(callback, name, nullptr, static_cast<FunctionClass *>(nullptr)));
        //}

        //void set_format(raw_to_rgb_converter raw_to_rgb) { m_raw_to_rgb = raw_to_rgb; }
        //void set_membits(int membits) { m_membits = membits; m_membits_supplied = true; }
        //void set_endianness(endianness_t endianness) { m_endianness = endianness; m_endianness_supplied = true; }

        public void set_entries(u32 entries) { m_entries = entries; }
        public void set_indirect_entries(u32 entries) { m_indirect_entries = entries; }

        //void enable_shadows() { m_enable_shadows = true; }
        //void enable_hilights() { m_enable_hilights = true; }
        //void set_prom_region(const char *region) { m_prom_region.set_tag(region); }


        public device_palette_interface_palette_device palette_interface() { return m_device_palette_interface; }


        // palette RAM accessors
        public memory_array basemem() { return m_paletteram; }
        //memory_array &extmem() { return m_paletteram_ext; }


        // raw entry reading
        public u32 read_entry(pen_t pen)
        {
            u32 data = m_paletteram.read((int)pen);
            if (m_paletteram_ext.baseptr() != null)
                data |= m_paletteram_ext.read((int)pen) << (8 * m_paletteram.bytes_per_entry());
            return data;
        }


        // generic read/write handlers
        //DECLARE_READ8_MEMBER(read8);
        //DECLARE_READ8_MEMBER(read8_ext);
        //DECLARE_WRITE8_MEMBER(write8);
        //DECLARE_WRITE8_MEMBER(write8_ext);
        //DECLARE_WRITE8_MEMBER(write_indirect);
        //DECLARE_WRITE8_MEMBER(write_indirect_ext);
        //DECLARE_READ16_MEMBER(read16);
        //DECLARE_READ16_MEMBER(read16_ext);
        //DECLARE_WRITE16_MEMBER(write16);
        //DECLARE_WRITE16_MEMBER(write16_ext);
        //DECLARE_READ32_MEMBER(read32);
        //DECLARE_WRITE32_MEMBER(write32);


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
        //void palette_init_RRRRGGGGBBBB_proms(palette_device &palette);
        //void palette_init_RRRRRGGGGGBBBBB(palette_device &palette);
        //void palette_init_BBBBBGGGGGRRRRR(palette_device &palette);
        //void palette_init_RRRRRGGGGGGBBBBB(palette_device &palette);
        //void palette_init_BBBBBGGGGGGRRRRR(palette_device &palette);


        // helper to update palette when data changed
        //void update() { if (!m_init.isnull()) m_init(*this); }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - start up the device
        //-------------------------------------------------
        protected override void device_start()
        {
            // bind the init function
            //m_init.bind_relative_to(*owner());

            // find the memory, if present
            memory_share share = memshare(tag());
            if (share != null)
            {
                // find the extended (split) memory, if present
                string tag_ext = tag() + "_ext";
                memory_share share_ext = memshare(tag_ext);

                // make sure we have specified a format
                //assert_always(m_raw_to_rgb.bytes_per_entry() > 0, "Palette has memory share but no format specified");

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
                    //assert_always(m_membits < share->bitwidth(), "Improper use of MCFG_PALETTE_MEMBITS");
                    m_paletteram.set_membits(m_membits);
                    if (share_ext != null)
                        m_paletteram_ext.set_membits(m_membits);
                }

                // override endianness if provided
                if (m_endianness_supplied)
                {
                    // forcing endianness only makes sense when the RAM is narrower than the palette format and not split
                    //assert_always((share_ext == NULL && m_paletteram.membits() / 8 < bytes_per_entry), "Improper use of MCFG_PALETTE_ENDIANNESS");
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


        //void update_for_write(offs_t byte_offset, int bytes_modified, bool indirect = false);
    }
}
