// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using endianness_t = mame.util.endianness;  //using endianness_t = util::endianness;
using gfx_interface_enumerator = mame.device_interface_enumerator<mame.device_gfx_interface>;  //typedef device_interface_enumerator<device_gfx_interface> gfx_interface_enumerator;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;

using static mame.cpp_global;
using static mame.digfx_global;
using static mame.emucore_global;
using static mame.osdcore_global;


namespace mame
{
    public static class digfx_global
    {
        public const int MAX_GFX_ELEMENTS = 32;
        public const int MAX_GFX_PLANES = 8;
        public const int MAX_GFX_SIZE = 32;

        public static readonly u32 [] EXTENDED_XOFFS = new u32[] { 0 };  //#define EXTENDED_XOFFS          { 0 }
        public static readonly u32 [] EXTENDED_YOFFS = new u32[] { 0 };  //#define EXTENDED_YOFFS          { 0 }

        public const u32 GFX_RAW                 = 0x12345678;
        //#define GFXLAYOUT_RAW( name, width, height, linemod, charmod ) \
        //const gfx_layout name = { width, height, RGN_FRAC(1,1), 8, { GFX_RAW }, { 0 }, { linemod }, charmod };
        // When planeoffset[0] is set to GFX_RAW, the gfx data is left as-is, with no conversion.
        // No buffer is allocated for the decoded data, and gfxdata is set to point to the source
        // data.
        // yoffset[0] is the line modulo (*8) and charincrement the char modulo (*8). They are *8
        // for consistency with the usual behaviour, but the bottom 3 bits are not used.
        //
        // This special mode can be used for graphics that are already in 8bpp linear format,
        // or for unusual formats that don't fit our generic model and need to be decoded using
        // custom code. See blend_gfx() in atarigen.c for an example of the latter usage.

        // these macros describe gfx_layouts in terms of fractions of a region
        // they can be used for total, planeoffset, xoffset, yoffset
        public static u32 RGN_FRAC(u32 num, u32 den) { return 0x80000000 | (((num) & 0x0f) << 27) | (((den) & 0x0f) << 23); }
        public static bool IS_FRAC(u32 offset) { return (offset & 0x80000000) != 0; }
        public static u32 FRAC_NUM(u32 offset) { return (offset >> 27) & 0x0f; }
        public static u32 FRAC_DEN(u32 offset) { return (offset >> 23) & 0x0f; }
        public static u32 FRAC_OFFSET(u32 offset) { return offset & 0x007fffff; }


        // these macros are useful in gfx_layouts

        public static u32 [] ArrayCombineUInt32(params object [] objects)
        {
            List<u32> output = new List<u32>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] is u32 object_u32) { output.Add(object_u32); }
                else if (objects[i] is u32[] object_u32_array) { output.AddRange(object_u32_array); }
                else if (objects[i] is s32 object_s32) { output.Add((u32)object_s32); }
            }

            return output.ToArray();
        }

        public static u32 [] STEP2(int START, int STEP) { return ArrayCombineUInt32(START, START + STEP); }  //#define STEP2(START,STEP)       (START),(START)+(STEP)
        public static u32 [] STEP4(int START, int STEP) { return ArrayCombineUInt32(STEP2(START, STEP), STEP2(START + 2 * STEP, STEP)); }  //#define STEP4(START,STEP)       STEP2(START,STEP),STEP2((START)+2*(STEP),STEP)
        public static u32 [] STEP8(int START, int STEP) { return ArrayCombineUInt32(STEP4(START, STEP), STEP4(START + 4 * STEP, STEP)); }  //#define STEP8(START,STEP)       STEP4(START,STEP),STEP4((START)+4*(STEP),STEP)
        public static u32 [] STEP16(int START, int STEP) { return ArrayCombineUInt32(STEP8(START, STEP), STEP8(START + 8 * STEP, STEP)); }  //#define STEP16(START,STEP)      STEP8(START,STEP),STEP8((START)+8*(STEP),STEP)
        public static u32 [] STEP32(int START, int STEP) { return ArrayCombineUInt32(STEP16(START, STEP), STEP16(START + 16 * STEP, STEP)); }  //#define STEP32(START,STEP)      STEP16(START,STEP),STEP16((START)+16*(STEP),STEP)
        //#define STEP64(START,STEP)      STEP32(START,STEP),STEP32((START)+32*(STEP),STEP)
        //#define STEP128(START,STEP)     STEP64(START,STEP),STEP64((START)+64*(STEP),STEP)
        //#define STEP256(START,STEP)     STEP128(START,STEP),STEP128((START)+128*(STEP),STEP)
        //#define STEP512(START,STEP)     STEP256(START,STEP),STEP256((START)+256*(STEP),STEP)
        //#define STEP1024(START,STEP)    STEP512(START,STEP),STEP512((START)+512*(STEP),STEP)
        //#define STEP2048(START,STEP)    STEP1024(START,STEP),STEP1024((START)+1024*(STEP),STEP)


        //**************************************************************************
        //  GRAPHICS INFO MACROS
        //**************************************************************************

        // optional horizontal and vertical scaling factors
        const u32 GFXENTRY_XSCALEMASK   = 0x000000ff;
        const u32 GFXENTRY_YSCALEMASK   = 0x0000ff00;
        static u32 GFXENTRY_XSCALE(u32 x) { return ((x-1) << 0) & GFXENTRY_XSCALEMASK; }
        static u32 GFXENTRY_YSCALE(u32 x) { return ((x-1) << 8) & GFXENTRY_YSCALEMASK; }
        public static u32 GFXENTRY_GETXSCALE(u32 x) { return ((x & GFXENTRY_XSCALEMASK) >> 0) + 1; }
        public static u32 GFXENTRY_GETYSCALE(u32 x) { return ((x & GFXENTRY_YSCALEMASK) >> 8) + 1; }

        // GFXENTRY_RAM means region tag refers to a RAM share instead of a ROM region
        const u32 GFXENTRY_ROM          = 0x00000000;
        const u32 GFXENTRY_RAM          = 0x00010000;
        public static bool GFXENTRY_ISROM(u32 x) { return (x & GFXENTRY_RAM) == 0; }
        public static bool GFXENTRY_ISRAM(u32 x) { return (x & GFXENTRY_RAM) != 0; }

        // GFXENTRY_DEVICE means region tag is relative to this device instead of its owner
        const u32 GFXENTRY_DEVICE       = 0x00020000;
        public static bool GFXENTRY_ISDEVICE(u32 x) { return (x & GFXENTRY_DEVICE) != 0; }

        // GFXENTRY_REVERSE reverses the bit order in the layout (0-7 = LSB-MSB instead of MSB-LSB)
        const u32 GFXENTRY_REVERSE      = 0x00040000;
        public static bool GFXENTRY_ISREVERSE(u32 x) { return (x & GFXENTRY_REVERSE) != 0; }


        // these macros are used for declaring gfx_decode_entry info arrays
        //#define GFXDECODE_START( name ) const gfx_decode_entry name[] = {
        //#define GFXDECODE_END { 0 } };

        // use these to declare a gfx_decode_entry array as a member of a device class
        //#define DECLARE_GFXDECODE_MEMBER( name ) static const gfx_decode_entry name[]
        //#define GFXDECODE_MEMBER( name ) const gfx_decode_entry name[] = {
        // common gfx_decode_entry macros
        //#define GFXDECODE_ENTRYX(region,offset,layout,start,colors,flags) { region, offset, &layout, start, colors, flags },
        public static gfx_decode_entry GFXDECODE_ENTRY(string region, u32 offset, gfx_layout layout, u16 start, u16 colors) { return new gfx_decode_entry(region, offset, layout, start, colors, 0); }

        // specialized gfx_decode_entry macros
        public static gfx_decode_entry GFXDECODE_RAM(string region, u32 offset, gfx_layout layout, u16 start, u16 colors) { return new gfx_decode_entry(region, offset, layout, start, colors, GFXENTRY_RAM); }
        //#define GFXDECODE_DEVICE(region,offset,layout,start,colors) { region, offset, &layout, start, colors, GFXENTRY_DEVICE },
        //#define GFXDECODE_DEVICE_RAM(region,offset,layout,start,colors) { region, offset, &layout, start, colors, GFXENTRY_DEVICE | GFXENTRY_RAM },
        public static gfx_decode_entry GFXDECODE_SCALE(string region, u32 offset, gfx_layout layout, u16 start, u16 colors, u32 x, u32 y) { return new gfx_decode_entry(region, offset, layout, start, colors, GFXENTRY_XSCALE(x) | GFXENTRY_YSCALE(y)); }
        //#define GFXDECODE_REVERSEBITS(region,offset,layout,start,colors) { region, offset, &layout, start, colors, GFXENTRY_REVERSE },
    }


    public struct gfx_layout
    {
        public u16 width;              // pixel width of each element
        public u16 height;             // pixel height of each element
        public u32 total;              // total number of elements, or RGN_FRAC()
        public u16 planes;             // number of bitplanes
        public u32 [] planeoffset; // bit offset of each bitplane
        public u32 [] xoffset; // bit offset of each horizontal pixel
        public u32 [] yoffset; // bit offset of each vertical pixel
        public u32 charincrement;      // distance between two consecutive elements (in bits)
        public Pointer<u32> extxoffs;  //const u32 *     extxoffs;           // extended X offset array for really big layouts
        public Pointer<u32> extyoffs;  //const u32 *     extyoffs;           // extended Y offset array for really big layouts


        public gfx_layout
            (
                u16 width,
                u16 height,
                u32 total,
                u16 planes,
                u32 [] planeoffset,
                u32 [] xoffset,
                u32 [] yoffset,
                u32 charincrement,
                Pointer<u32> extxoffs = null,
                Pointer<u32> extyoffs = null
            )
        {
            this.width = width;
            this.height = height;
            this.total = total;
            this.planes = planes;
            this.planeoffset = new u32[MAX_GFX_PLANES]; // bit offset of each bitplane
            Array.Copy(planeoffset, this.planeoffset, planeoffset.Length);
            this.xoffset = new u32[MAX_GFX_SIZE]; // bit offset of each horizontal pixel
            Array.Copy(xoffset, this.xoffset, xoffset.Length);
            this.yoffset = new u32[MAX_GFX_SIZE]; // bit offset of each vertical pixel
            Array.Copy(yoffset, this.yoffset, yoffset.Length);
            this.charincrement = charincrement;
            this.extxoffs = extxoffs != null ? new Pointer<u32>(extxoffs) : null;
            this.extyoffs = extyoffs != null ? new Pointer<u32>(extyoffs) : null;
        }

        public gfx_layout(gfx_layout rhs) : this(rhs.width, rhs.height, rhs.total, rhs.planes, rhs.planeoffset, rhs.xoffset, rhs.yoffset, rhs.charincrement, rhs.extxoffs, rhs.extyoffs) { }

        public u32 xoffs(int x) { return extxoffs != default ? extxoffs[x] : xoffset[x]; }
        public u32 yoffs(int y) { return extyoffs != default ? extyoffs[y] : yoffset[y]; }
    }


    public struct gfx_decode_entry
    {
        public string memory_region;      // memory region where the data resides
        public u32 start;              // offset of beginning of data to decode
        public gfx_layout gfxlayout;        // pointer to gfx_layout describing the layout; NULL marks the end of the array
        public u16 color_codes_start;  // offset in the color lookup table where color codes start
        public u16 total_color_codes;  // total number of color codes
        public u32 flags;              // flags and optional scaling factors

        public gfx_decode_entry(string memory_region, u32 start, gfx_layout gfxlayout, u16 color_codes_start, u16 total_color_codes, u32 flags)
        {
            this.memory_region = memory_region;
            this.start = start;
            this.gfxlayout = gfxlayout;
            this.color_codes_start = color_codes_start;
            this.total_color_codes = total_color_codes;
            this.flags = flags;
        }
    }


    // ======================> device_gfx_interface
    public class device_gfx_interface : device_interface
    {
        //static const gfx_decode_entry empty[];


        optional_device<device_palette_interface> m_palette; // configured tag for palette device
        gfx_element [] m_gfx = new gfx_element[MAX_GFX_ELEMENTS];    // array of pointers to graphic sets

        // configuration
        gfx_decode_entry [] m_gfxdecodeinfo;        // pointer to array of gfx decode information
        bool m_palette_is_disabled;  // no palette associated with this gfx decode

        // internal state
        bool m_decoded;                  // have we processed our decode info yet?


        // construction/destruction
        //-------------------------------------------------
        //  device_gfx_interface - constructor
        //-------------------------------------------------
        public device_gfx_interface(machine_config mconfig, device_t device, gfx_decode_entry [] gfxinfo = null, string palette_tag = finder_base.DUMMY_TAG)
            : base(device, "gfx")
        {
            m_palette = new optional_device<device_palette_interface>(this.op, palette_tag);
            m_gfxdecodeinfo = gfxinfo;
            m_palette_is_disabled = false;
            m_decoded = false;
        }


        // configuration

        public void set_info(gfx_decode_entry [] gfxinfo) { m_gfxdecodeinfo = gfxinfo; }
        //template <typename T> void set_palette(T &&tag) { m_palette.set_tag(std::forward<T>(tag)); }
        public void set_palette(string tag) { m_palette.set_tag(tag); }
        public void set_palette(finder_base tag) { m_palette.set_tag(tag); }


        //-------------------------------------------------
        //  set_palette_disable: configuration helper to
        //  disable the use of a palette by the device
        //-------------------------------------------------
        void set_palette_disable(bool disable)
        {
            m_palette_is_disabled = disable;
        }


        // getters
        public device_palette_interface palette() { assert(m_palette.bool_); return m_palette.op0; }  //{ assert(m_palette); return *m_palette; }
        public gfx_element gfx(int index) { assert(index < MAX_GFX_ELEMENTS); return m_gfx[index]; }


        // decoding
        //-------------------------------------------------
        //  decode_gfx - parse gfx decode info and
        //  create gfx elements
        //-------------------------------------------------
        void decode_gfx(gfx_decode_entry [] gfxdecodeinfo)
        {
            // skip if nothing to do
            if (gfxdecodeinfo == null)
                return;

            // local variables to hold mutable copies of gfx layout data
            gfx_layout glcopy;
            std.vector<u32> extxoffs = new std.vector<u32>(0);
            std.vector<u32> extyoffs = new std.vector<u32>(0);

            // loop over all elements
            for (u8 curgfx = 0; curgfx < MAX_GFX_ELEMENTS && curgfx < gfxdecodeinfo.Length /*&& gfxdecodeinfo[curgfx].gfxlayout != null*/; curgfx++)
            {
                ref gfx_decode_entry gfx = ref gfxdecodeinfo[curgfx];

                // extract the scale factors and xormask
                u32 xscale = GFXENTRY_GETXSCALE(gfx.flags);
                u32 yscale = GFXENTRY_GETYSCALE(gfx.flags);
                u32 xormask = GFXENTRY_ISREVERSE(gfx.flags) ? 7U : 0U;

                // resolve the region
                u32 region_length;
                Pointer<u8> region_base;  //const u8     *region_base;
                u8 region_width;
                endianness_t region_endianness;

                if (gfx.memory_region != null)
                {
                    device_t basedevice = (GFXENTRY_ISDEVICE(gfx.flags)) ? device() : device().owner();
                    if (GFXENTRY_ISRAM(gfx.flags))
                    {
                        memory_share share = basedevice.memshare(gfx.memory_region);
                        //assert(share != NULL);
                        region_length = (u32)(8 * share.bytes());
                        region_base = share.ptr();  //region_base = reinterpret_cast<u8 *>(share->ptr());
                        region_width = share.bytewidth();
                        region_endianness = share.endianness();
                    }
                    else
                    {
                        memory_region region = basedevice.memregion(gfx.memory_region);
                        //assert(region != NULL);
                        region_length = 8 * region.bytes();
                        region_base = new Pointer<u8>(region.base_());  //region_base = region->base();
                        region_width = region.bytewidth();
                        region_endianness = region.endianness();
                    }
                }
                else
                {
                    region_length = 0;
                    region_base = null;
                    region_width = 1;
                    region_endianness = ENDIANNESS_NATIVE;
                }

                if (region_endianness != ENDIANNESS_NATIVE)
                {
                    switch (region_width)
                    {
                        case 2:
                            xormask |= 0x08;
                            break;
                        case 4:
                            xormask |= 0x18;
                            break;
                        case 8:
                            xormask |= 0x38;
                            break;
                    }
                }


                // copy the layout into our temporary variable
                glcopy = new gfx_layout(gfx.gfxlayout);  //memcpy(&glcopy, gfx.gfxlayout, sizeof(gfx_layout));


                // if the character count is a region fraction, compute the effective total
                if (IS_FRAC(glcopy.total))
                {
                    //assert(region_length != 0);
                    glcopy.total = region_length / glcopy.charincrement * FRAC_NUM(glcopy.total) / FRAC_DEN(glcopy.total);
                }

                // for non-raw graphics, decode the X and Y offsets
                if (glcopy.planeoffset[0] != GFX_RAW)
                {
                    // copy the X and Y offsets into our temporary arrays
                    extxoffs.resize(glcopy.width * xscale);
                    extyoffs.resize(glcopy.height * yscale);

                    //memcpy(&extxoffs[0], (glcopy.extxoffs != null) ? glcopy.extxoffs : glcopy.xoffset, glcopy.width * sizeof(UInt32));
                    //memcpy(&extyoffs[0], (glcopy.extyoffs != null) ? glcopy.extyoffs : glcopy.yoffset, glcopy.height * sizeof(UInt32));
                    for (int i = 0; i < glcopy.width; i++)
                        extxoffs[i] = glcopy.extxoffs != null ? glcopy.extxoffs[i] : glcopy.xoffset[i];

                    for (int i = 0; i < glcopy.height; i++)
                        extyoffs[i] = glcopy.extyoffs != null ? glcopy.extyoffs[i] : glcopy.yoffset[i];

                    // always use the extended offsets here
                    glcopy.extxoffs = new Pointer<u32>(extxoffs);
                    glcopy.extyoffs = new Pointer<u32>(extyoffs);

                    // expand X and Y by the scale factors
                    if (xscale > 1)
                    {
                        glcopy.width *= (u16)xscale;
                        for (int j = glcopy.width - 1; j >= 0; j--)
                            extxoffs[j] = extxoffs[(int)(j / xscale)];
                    }
                    if (yscale > 1)
                    {
                        glcopy.height *= (u16)yscale;
                        for (int j = glcopy.height - 1; j >= 0; j--)
                            extyoffs[j] = extyoffs[(int)(j / yscale)];
                    }

                    // loop over all the planes, converting fractions
                    for (int j = 0; j < glcopy.planes; j++)
                    {
                        u32 value1 = glcopy.planeoffset[j];
                        if (IS_FRAC(value1))
                        {
                            //assert(region_length != 0);
                            glcopy.planeoffset[j] = FRAC_OFFSET(value1) + region_length * FRAC_NUM(value1) / FRAC_DEN(value1);
                        }
                    }

                    // loop over all the X/Y offsets, converting fractions
                    for (int j = 0; j < glcopy.width; j++)
                    {
                        u32 value2 = extxoffs[j];
                        if (IS_FRAC(value2))
                        {
                            //assert(region_length != 0);
                            extxoffs[j] = FRAC_OFFSET(value2) + region_length * FRAC_NUM(value2) / FRAC_DEN(value2);
                        }
                    }

                    for (int j = 0; j < glcopy.height; j++)
                    {
                        u32 value3 = extyoffs[j];
                        if (IS_FRAC(value3))
                        {
                            //assert(region_length != 0);
                            extyoffs[j] = FRAC_OFFSET(value3) + region_length * FRAC_NUM(value3) / FRAC_DEN(value3);
                        }
                    }
                }

                // otherwise, just use the line modulo
                else
                {
                    int base_ = (int)gfx.start;
                    int end = (int)(region_length / 8);
                    int linemod = (int)glcopy.yoffset[0];
                    while (glcopy.total > 0)
                    {
                        int elementbase = (int)(base_ + (glcopy.total - 1) * glcopy.charincrement / 8);
                        int lastpixelbase = elementbase + glcopy.height * linemod / 8 - 1;
                        if (lastpixelbase < end)
                            break;
                        glcopy.total--;
                    }
                }

                // allocate the graphics
                m_gfx[curgfx] = new gfx_element(m_palette.op0, glcopy, region_base != null ? new Pointer<u8>(region_base, (int)gfx.start) : null, xormask, gfx.total_color_codes, gfx.color_codes_start);
            }

            m_decoded = true;
        }

        void decode_gfx() { decode_gfx(m_gfxdecodeinfo); }


        //void set_gfx(int index, gfx_element *element) { assert(index < MAX_GFX_ELEMENTS); m_gfx[index].reset(element); }


        // interface-level overrides

        //-------------------------------------------------
        //  interface_validity_check - validate graphics
        //  decoding configuration
        //-------------------------------------------------
        protected override void interface_validity_check(validity_checker valid)
        {
            if (!m_palette_is_disabled && !m_palette.bool_)
            {
                std.pair<device_t, string> target = m_palette.finder_target();
                if (target.second == finder_base.DUMMY_TAG)
                {
                    osd_printf_error("No palette specified for device '{0}'\n", device().tag());
                }
                else
                {
                    osd_printf_error(
                            "Device '{0}' specifies nonexistent device '{1}' relative to '{2}' as palette\n",
                            device().tag(),
                            target.second,
                            target.first.tag());
                }
            }

            if (m_gfxdecodeinfo == null)
                return;

            // validate graphics decoding entries
            for (int gfxnum = 0; gfxnum < MAX_GFX_ELEMENTS /*&& m_gfxdecodeinfo[gfxnum].gfxlayout != null*/; gfxnum++)
            {
                ref gfx_decode_entry gfx = ref m_gfxdecodeinfo[gfxnum];
                ref gfx_layout layout = ref gfx.gfxlayout;

                // currently we are unable to validate RAM-based entries
                string region = gfx.memory_region;
                if (region != null && GFXENTRY_ISROM(gfx.flags))
                {
                    // resolve the region
                    string gfxregion;
                    if (GFXENTRY_ISDEVICE(gfx.flags))
                        gfxregion = device().subtag(region);
                    else
                        gfxregion = device().owner().subtag(region);

                    u32 region_length = (u32)valid.region_length(gfxregion);
                    if (region_length == 0)
                        osd_printf_error("gfx[{0}] references nonexistent region '{1}'\n", gfxnum, gfxregion);

                    // if we have a valid region, and we're not using auto-sizing, check the decode against the region length
                    else if (!IS_FRAC(layout.total))
                    {
                        // determine which plane is at the largest offset
                        int start = 0;
                        for (int plane = 0; plane < layout.planes; plane++)
                        {
                            if (layout.planeoffset[plane] > start)
                                start = (int)layout.planeoffset[plane];
                        }
                        start &= ~(int)(layout.charincrement - 1);

                        // determine the total length based on this info
                        int len = (int)(layout.total * layout.charincrement);

                        // do we have enough space in the region to cover the whole decode?
                        int avail = (int)(region_length - (gfx.start & ~(layout.charincrement / 8 - 1)));

                        // if not, this is an error
                        if ((start + len) / 8 > avail)
                            osd_printf_error("gfx[{0}] extends past allocated memory of region '{1}'\n", gfxnum, region);
                    }
                }

                int xscale = (int)GFXENTRY_GETXSCALE(gfx.flags);
                int yscale = (int)GFXENTRY_GETYSCALE(gfx.flags);

                // verify raw decode, which can only be full-region and have no scaling
                if (layout.planeoffset[0] == GFX_RAW)
                {
                    if (layout.total != RGN_FRAC(1,1))
                        osd_printf_error("gfx[{0}] RAW layouts can only be RGN_FRAC(1,1)\n", gfxnum);
                    if (xscale != 1 || yscale != 1)
                        osd_printf_error("gfx[{0}] RAW layouts do not support xscale/yscale\n", gfxnum);
                }

                // verify traditional decode doesn't have too many planes,
                // and has extended offset arrays if its width and/or height demand them
                else
                {
                    throw new emu_unimplemented();
                }
            }
        }

        //-------------------------------------------------
        //  interface_pre_start - make sure all our input
        //  devices are started
        //-------------------------------------------------
        public override void interface_pre_start()
        {
            if (!m_palette_is_disabled && !m_palette.bool_)
            {
                std.pair<device_t, string> target = m_palette.finder_target();
                if (target.second == finder_base.DUMMY_TAG)
                {
                    fatalerror("No palette specified for device {0}\n", device().tag());
                }
                else
                {
                    fatalerror(
                            "Device '{0}' specifies nonexistent device '{1}' relative to '{2}' as palette\n",
                            device().tag(),
                            target.second,
                            target.first.tag());
                }
            }

            // if palette device isn't started, wait for it
            // if (m_palette && !m_palette->device().started())
            //  throw device_missing_dependencies();
        }

        //-------------------------------------------------
        //  interface_post_start - decode gfx, if we
        //  haven't done so already
        //-------------------------------------------------
        public override void interface_post_start()
        {
            if (!m_decoded)
                decode_gfx(m_gfxdecodeinfo);
        }
    }


    // iterator
    //typedef device_interface_enumerator<device_gfx_interface> gfx_interface_enumerator;
}
