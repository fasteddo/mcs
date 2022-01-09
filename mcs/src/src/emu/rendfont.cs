// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using char32_t = System.UInt32;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using s16 = System.Int16;
using s32 = System.Int32;
using size_t = System.UInt64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt32;

using static mame.cmddata_global;
using static mame.corestr_global;
using static mame.osdcore_global;
using static mame.osdfile_global;
using static mame.rendfont_global;
using static mame.uicmd14_global;
using static mame.uismall_global;
using static mame.unicode_global;


namespace mame
{
    // ======================> render_font
    // a render_font describes and provides an interface to a font
    public class render_font
    {
        //friend class render_manager;


        const bool VERBOSE = false;
        static void LOG(string format, params object [] args) { if (VERBOSE) osd_printf_verbose(format, args); }


        // internal format
        enum format
        {
            UNKNOWN,
            TEXT,
            CACHED,
            OSD
        }


        // a glyph describes a single glyph
        class glyph
        {
            public s32 width;              // width from this character to the next
            public s32 xoffs;
            public s32 yoffs;       // X and Y offset from baseline to top,left of bitmap
            public s32 bmwidth;
            public s32 bmheight;  // width and height of bitmap
            public Pointer<u8> rawdata;  //const char *        rawdata;            // pointer to the raw data for this one
            public render_texture texture;            // pointer to a texture for rendering and sizing
            public bitmap_argb32 bitmap;             // pointer to the bitmap containing the raw data

            public rgb_t color;


            public glyph()
            {
                width = -1;
                xoffs = -1;
                yoffs = -1;
                bmwidth = 0;
                bmheight = 0;
                rawdata = null;
                texture = null;
                bitmap = new bitmap_argb32();
                color = new rgb_t();
            }
        }


        // constants
        const u64 CACHED_BDF_HASH_SIZE   = 1024;


        // internal state
        render_manager m_manager;
        format m_format;           // format of font data
        int m_height;           // height of the font, from ascent to descent
        int m_yoffs;            // y offset from baseline to descent
        int m_defchar;          // default substitute character
        float m_scale;            // 1 / height precomputed
        List<glyph> [] m_glyphs = new List<glyph>[17 * 256];  //glyph               *m_glyphs[17*256];  // array of glyph subtables
        std.vector<u8> m_rawdata = new std.vector<u8>();  //std::vector<char>   m_rawdata;          // pointer to the raw data for the font
        u64 m_rawsize;          // size of the raw font data
        osd_font m_osdfont;  //std::unique_ptr<osd_font> m_osdfont;    // handle to the OSD font

        int m_height_cmd;       // height of the font, from ascent to descent
        int m_yoffs_cmd;        // y offset from baseline to descent
        List<glyph> [] m_glyphs_cmd = new List<glyph>[17 * 256];  //EQUIVALENT_ARRAY(m_glyphs, glyph *) m_glyphs_cmd; // array of glyph subtables
        std.vector<u8> m_rawdata_cmd = new std.vector<u8>();  //std::vector<char>   m_rawdata_cmd;      // pointer to the raw data for the font


        // construction/destruction

        //-------------------------------------------------
        //  render_font - constructor
        //-------------------------------------------------
        public render_font(render_manager manager, string filename)
        {
            m_manager = manager;
            m_format = format.UNKNOWN;
            m_height = 0;
            m_yoffs = 0;
            m_defchar = -1;
            m_scale = 1.0f;
            m_rawsize = 0;
            m_osdfont = null;
            m_height_cmd = 0;
            m_yoffs_cmd = 0;


            //memset(m_glyphs, 0, sizeof(m_glyphs));
            for (int i = 0; i < m_glyphs.Length; i++)
                m_glyphs[i] = new List<glyph>();

            //memset(m_glyphs_cmd, 0, sizeof(m_glyphs_cmd));
            //for (int i = 0; i < m_glyphs_cmd.Length; i++)
            //    m_glyphs_cmd[i] = new List<glyph>();


            // if this is an OSD font, we're done
            if (filename != null)
            {
                m_osdfont = manager.machine().osd().font_alloc();
                if (m_osdfont != null && m_osdfont.open(manager.machine().options().font_path(), filename, out m_height))
                {
                    m_scale = 1.0f / (float)m_height;
                    m_format = format.OSD;

                    //mamep: allocate command glyph font
                    render_font_command_glyph();
                    return;
                }

                m_osdfont = null;  // m_osdfont.reset();
            }

            // if the filename is 'default' default to 'ui.bdf' for backwards compatibility
            if (filename != null && core_stricmp(filename, "default") == 0)
                filename = "ui.bdf";

            // attempt to load an external BDF font first
            if (filename != null && load_cached_bdf(filename))
            {
                //mamep: allocate command glyph font
                render_font_command_glyph();
                return;
            }

            // load the compiled in data instead
            emu_file ramfile = new emu_file(OPEN_FLAG_READ);
            std.error_condition filerr = ramfile.open_ram(new MemoryU8(font_uismall), (u32)font_uismall.Length);//, sizeof(font_uismall));
            if (!filerr)
                load_cached(ramfile, 0, 0);

            ramfile.close();

            render_font_command_glyph();
        }


        // getters
        render_manager manager() { return m_manager; }


        // size queries
        public s32 pixel_height() { return m_height; }

        //-------------------------------------------------
        //  char_width - return the width of a character
        //  at the given height
        //-------------------------------------------------
        public float char_width(float height, float aspect, char32_t ch) { return (float)(get_char(ch).width) * m_scale * height * aspect; }

        //-------------------------------------------------
        //  string_width - return the width of a string
        //  at the given height
        //-------------------------------------------------
        public float string_width(float height, float aspect, string str)  // const char *string)
        {
            // loop over the string and accumulate widths
            int totwidth = 0;
            char schar;  //char32_t schar;

            // loop over characters
            while (!str.empty())
            {
                int scharcount = uchar_from_utf8(out schar, str);
                totwidth += get_char(schar).width;
                str = str.Substring(scharcount);  //string.remove_prefix(scharcount);
            }

            // scale the final result based on height
            return (float)totwidth * m_scale * height * aspect;
        }


        //-------------------------------------------------
        //  utf8string_width - return the width of a
        //  UTF8-encoded string at the given height
        //-------------------------------------------------
        public float utf8string_width(float height, float aspect, string utf8string)
        {
            // loop over the string and accumulate widths
            s32 totwidth = 0;
            while (!utf8string.empty())
            {
                char uchar;  //char32_t uchar;
                int count = uchar_from_utf8(out uchar, utf8string);
                if (count < 0)
                    break;

                totwidth += get_char(uchar).width;
                utf8string = utf8string.Substring(count);  //utf8string.remove_prefix(count);
            }

            // scale the final result based on height
            return (float)totwidth * m_scale * height * aspect;
        }


        // texture/bitmap queries

        //-------------------------------------------------
        //  get_char_texture_and_bounds - return the
        //  texture for a character and compute the
        //  bounds of the final bitmap
        //-------------------------------------------------
        public render_texture get_char_texture_and_bounds(float height, float aspect, char32_t chnum, ref render_bounds bounds)
        {
            glyph gl = get_char(chnum);

            // on entry, assume x0,y0 are the top,left coordinate of the cell and add
            // the character bounding box to that position
            float scale = m_scale * height;
            bounds.x0 += (float)gl.xoffs * scale * aspect;

            // compute x1,y1 from there based on the bitmap size
            bounds.x1 = bounds.x0 + (float)gl.bmwidth * scale * aspect;
            bounds.y1 = bounds.y0 + (float)m_height * scale;

            // return the texture
            return gl.texture;
        }

        //-------------------------------------------------
        //  get_scaled_bitmap_and_bounds - return a
        //  scaled bitmap and bounding rect for a char
        //-------------------------------------------------
        public void get_scaled_bitmap_and_bounds(bitmap_argb32 dest, float height, float aspect, char32_t chnum, out rectangle bounds)
        {
            bounds = default;

            glyph gl = get_char(chnum);

            // on entry, assume x0,y0 are the top,left coordinate of the cell and add
            // the character bounding box to that position
            float scale = m_scale * height;
            bounds.min_x = (int)((float)(gl.xoffs) * scale * aspect);
            bounds.min_y = 0;

            // compute x1,y1 from there based on the bitmap size
            bounds.set_width((int)((float)(gl.bmwidth) * scale * aspect));
            bounds.set_height((int)((float)(m_height) * scale));

            // if the bitmap isn't big enough, bail
            if (dest.width() < bounds.width() || dest.height() < bounds.height())
                return;

            // if no texture, fill the target
            if (gl.texture == null)
            {
                dest.fill(0);
                return;
            }

            throw new emu_unimplemented();
#if false
            // scale the font
            bitmap_argb32 tempbitmap = new bitmap_argb32(&dest.pix(0), bounds.width(), bounds.height(), dest.rowpixels());
            render_texture.hq_scale(tempbitmap, gl.bitmap, gl.bitmap.cliprect(), null);
#endif
        }


        static glyph get_char_dummy_glyph = new glyph();

        // helpers
        //-------------------------------------------------
        //  get_char - return a pointer to a character
        //  in a font, expanding if necessary
        //-------------------------------------------------
        glyph get_char(char32_t chnum)
        {
            unsigned page = chnum / 256;
            if (page >= m_glyphs.Length)
            {
                if ((0 <= m_defchar) && (chnum != m_defchar))
                    return get_char((char32_t)m_defchar);
                else
                    return get_char_dummy_glyph;
            }
            else if (m_glyphs[page] == null || m_glyphs[page].Count == 0)
            {
                //mamep: make table for command glyph
                if ((m_format == format.OSD) || ((chnum >= COMMAND_UNICODE) && (chnum < COMMAND_UNICODE + MAX_GLYPH_FONT)))
                {
                    m_glyphs[page] = new List<glyph>(256);  // new glyph[256];
                    for (int i = 0; i < 256; i++)
                        m_glyphs[page].Add(new glyph());
                }
                else if ((0 <= m_defchar) && (chnum != m_defchar))
                {
                    return get_char((char32_t)m_defchar);
                }
                else
                {
                    return get_char_dummy_glyph;
                }
            }

            // if the character isn't generated yet, do it now
            glyph gl = m_glyphs[page][(int)(chnum % 256)];
            if (!gl.bitmap.valid())
            {
                //mamep: command glyph support
                if (m_height_cmd != 0 && chnum >= COMMAND_UNICODE && chnum < COMMAND_UNICODE + MAX_GLYPH_FONT)
                {
                    glyph glyph_ch = m_glyphs_cmd[page][(int)(chnum % 256)];
                    float scale = (float)m_height / (float)m_height_cmd;
                    if (m_format == format.OSD)
                        scale *= 0.90f;

                    if (!glyph_ch.bitmap.valid())
                        char_expand(chnum, glyph_ch);

                    //mamep: for color glyph
                    gl.color = glyph_ch.color;

                    gl.width = (int)(glyph_ch.width * scale + 0.5f);
                    gl.xoffs = (int)(glyph_ch.xoffs * scale + 0.5f);
                    gl.yoffs = (int)(glyph_ch.yoffs * scale + 0.5f);
                    gl.bmwidth = (int)(glyph_ch.bmwidth * scale + 0.5f);
                    gl.bmheight = (int)(glyph_ch.bmheight * scale + 0.5f);

                    gl.bitmap.allocate(gl.bmwidth, gl.bmheight);
                    rectangle clip = new rectangle(
                            0, glyph_ch.bitmap.width() - 1,
                            0, glyph_ch.bitmap.height() - 1);
                    render_texture.hq_scale(gl.bitmap, glyph_ch.bitmap, clip, null);

                    /* wrap a texture around the bitmap */
                    gl.texture = m_manager.texture_alloc(render_texture.hq_scale);
                    gl.texture.set_bitmap(gl.bitmap, gl.bitmap.cliprect(), texture_format.TEXFORMAT_ARGB32);
                }
                else
                {
                    char_expand(chnum, gl);
                }
            }

            return gl;
        }

        //-------------------------------------------------
        //  char_expand - expand the raw data for a
        //  character into a bitmap
        //-------------------------------------------------
        void char_expand(char32_t chnum, glyph gl)
        {
            LOG("render_font::char_expand: expanding character {0}\n", chnum);

            rgb_t fgcol = (gl.color != 0 ? gl.color : new rgb_t(0xff, 0xff, 0xff, 0xff));
            rgb_t bgcol = new rgb_t(0x00, 0xff, 0xff, 0xff);
            bool is_cmd = ((chnum >= COMMAND_UNICODE) && (chnum < COMMAND_UNICODE + MAX_GLYPH_FONT));

            if (is_cmd)
            {
                throw new emu_unimplemented();
#if false
                // punt if nothing there
                if (gl.bmwidth == 0 || gl.bmheight == 0 || gl.rawdata == null)
                    return;

                // allocate a new bitmap of the size we need
                gl.bitmap.allocate(gl.bmwidth, m_height_cmd);
                gl.bitmap.fill(0);

                // extract the data
                const char *ptr = gl.rawdata;
                byte accum = 0;
                byte accumbit = 7;
                for (int y = 0; y < gl.bmheight; y++)
                {
                    int desty = y + m_height_cmd + m_yoffs_cmd - gl.yoffs - gl.bmheight;
                    u32 *dest = (desty >= 0 && desty < m_height_cmd) ? &gl.bitmap.pix(desty, 0) : nullptr;
                    {
                        for (int x = 0; x < gl.bmwidth; x++)
                        {
                            if (accumbit == 7)
                                accum = *ptr++;
                            if (dest != null)
                                *dest++ = (accum & (1 << accumbit)) ? color : rgb_t(0x00,0xff,0xff,0xff);
                            accumbit = (accumbit - 1) & 7;
                        }
                    }
                }
#endif
            }
            else if (m_format == format.OSD)
            {
                throw new emu_unimplemented();
#if false
                // if we're an OSD font, query the info
#endif
            }
            else if (gl.bmwidth == 0 || gl.bmheight == 0 || gl.rawdata == null)
            {
                // abort if nothing there
                LOG("render_font::char_expand: empty bitmap bounds or no raw data\n");
                return;
            }
            else
            {
                // other formats need to parse their data
                LOG("render_font::char_expand: building bitmap from raw data\n");

                // allocate a new bitmap of the size we need
                gl.bitmap.allocate(gl.bmwidth, m_height);
                gl.bitmap.fill(0);

                // extract the data
                Pointer<u8> ptr = new Pointer<u8>(gl.rawdata);  //const char *ptr = gl.rawdata;
                u8 accum = 0;
                u8 accumbit = 7;
                for (int y = 0; y < gl.bmheight; y++)
                {
                    int desty = y + m_height + m_yoffs - gl.yoffs - gl.bmheight;
                    PointerU32 dest = ((0 <= desty) && (m_height > desty)) ? gl.bitmap.pix(desty) : null;  //u32 *dest(((0 <= desty) && (m_height > desty)) ? &gl.bitmap.pix(desty) : nullptr);

                    if (m_format == format.TEXT)
                    {
                        if (dest != null)
                        {
                            for (int x = 0; gl.bmwidth > x; )
                            {
                                // scan for the next hex digit
                                int bits = -1;
                                while ('\r' != ptr[0] && '\n' != ptr[0] && 0 > bits)  // while (('\r' != *ptr) && ('\n' != *ptr) && (0 > bits))
                                {
                                    if (ptr[0] >= '0' && ptr[0] <= '9')
                                    {
                                        bits = ptr[0] - '0';  //bits = *ptr++ - '0';
                                        ptr++;
                                    }
                                    else if (ptr[0] >= 'A' && ptr[0] <= 'F')
                                    {
                                        bits = ptr[0] - 'A' + 10;
                                        ptr++;
                                    }
                                    else if (ptr[0] >= 'a' && ptr[0] <= 'f')
                                    {
                                        bits = ptr[0] - 'a' + 10;
                                        ptr++;
                                    }
                                    else
                                    {
                                        ptr++;
                                    }
                                }

                                // expand the four bits
                                dest[0] = (bits & 8) != 0 ? fgcol : bgcol;  dest++;  //*dest++ = (bits & 8) ? fgcol : bgcol;
                                if (gl.bmwidth > ++x)
                                    { dest[0] = (bits & 4) != 0 ? fgcol : bgcol;  dest++; }  //*dest++ = (bits & 4) ? fgcol : bgcol;
                                if (gl.bmwidth > ++x)
                                    { dest[0] = (bits & 2) != 0 ? fgcol : bgcol;  dest++; }  //*dest++ = (bits & 2) ? fgcol : bgcol;
                                if (gl.bmwidth > ++x)
                                    { dest[0] = (bits & 1) != 0 ? fgcol : bgcol;  dest++; }  //*dest++ = (bits & 1) ? fgcol : bgcol;

                                x++;
                            }

                            // advance to the next line
                            ptr = next_line(ptr);
                        }
                    }
                    else if (m_format == format.CACHED)
                    {
                        for (int x = 0; x < gl.bmwidth; x++)
                        {
                            if (accumbit == 7)
                            {
                                accum = ptr[0];
                                ptr++;
                            }

                            if (dest != null)
                                { dest[0] = (accum & (1 << accumbit)) != 0 ? fgcol : bgcol;  dest++; }  //*dest++ = (accum & (1 << accumbit)) ? fgcol : bgcol;

                            accumbit = (u8)((accumbit - 1) & 7);
                        }
                    }
                }
            }

            // wrap a texture around the bitmap
            gl.texture = m_manager.texture_alloc(render_texture.hq_scale);
            gl.texture.set_bitmap(gl.bitmap, gl.bitmap.cliprect(), texture_format.TEXFORMAT_ARGB32);
        }

        //-------------------------------------------------
        //  load_cached_bdf - attempt to load a cached
        //  version of the BDF font 'filename'; if that
        //  fails, fall back on the regular BDF loader
        //  and create a new cached version
        //-------------------------------------------------
        bool load_cached_bdf(string filename)
        {
            std.error_condition filerr;
            u32 chunk;
            u64 bytes;

            // first try to open the BDF itself
            emu_file file = new emu_file(manager().machine().options().font_path(), OPEN_FLAG_READ);
            filerr = file.open(filename);
            if (filerr)
                return false;

            file.close();

            throw new emu_unimplemented();
#if false
#endif
        }

        //-------------------------------------------------
        //  load_bdf - parse and load a BDF font
        //-------------------------------------------------
        bool load_bdf()
        {
            // set the format to text
            m_format = format.TEXT;

            throw new emu_unimplemented();
#if false
#endif
        }


        //-------------------------------------------------
        //  load_cached - load a font in cached format
        //-------------------------------------------------
        bool load_cached(emu_file file, u64 length, u32 hash)
        {
            // get the file size, read the header, and check that it looks good
            u64 filesize = file.size();
            bdc_header header = new bdc_header();
            if (!header.read(file))
            {
                osd_printf_warning("render_font::load_cached: error reading BDC header\n");
                return false;
            }
            else if (!header.check_magic() || (bdc_header.MAJVERSION != header.get_major_version()) || (bdc_header.MINVERSION != header.get_minor_version()))
            {
                LOG("render_font::load_cached: incompatible BDC file\n");
                return false;
            }
            else if (length != 0 && ((header.get_original_length() != length) || (header.get_original_hash() != hash)))
            {
                LOG("render_font::load_cached: BDC file does not match original BDF file\n");
                return false;
            }

            // get global properties from the header
            m_height = header.get_height();
            m_scale = 1.0f / (float)(m_height);
            m_yoffs = header.get_y_offset();
            m_defchar = header.get_default_character();
            u32 numchars = header.get_glyph_count();
            if ((file.tell() + ((u64)(numchars) * bdc_table_entry.size())) > filesize)
            {
                LOG("render_font::load_cached: BDC file is too small to hold glyph table\n");
                return false;
            }

            // now read the rest of the data
            u64 remaining = filesize - file.tell();
            try
            {
                m_rawdata.resize(remaining);
            }
            catch (Exception )
            {
                osd_printf_error("render_font::load_cached: allocation error\n");
            }
            for (u64 bytes_read = 0; remaining > bytes_read; )
            {
                u32 chunk = (u32)std.min(u32.MaxValue, remaining);
                if (file.read(new Pointer<u8>(m_rawdata, (int)bytes_read), chunk) != chunk)  //if (file.read(&m_rawdata[bytes_read], chunk) != chunk)
                {
                    osd_printf_error("render_font::load_cached: error reading BDC data\n");
                    m_rawdata.clear();
                    return false;
                }
                bytes_read += chunk;
            }

            // extract the data from the data
            size_t offset = (size_t)numchars * bdc_table_entry.size();
            bdc_table_entry entry = new bdc_table_entry(m_rawdata.empty() ? null : new Pointer<u8>(m_rawdata));
            for (unsigned chindex = 0; chindex < numchars; chindex++, entry = entry.get_next())
            {
                // if we don't have a subtable yet, make one
                int chnum = (int)entry.get_encoding();
                LOG("render_font::load_cached: loading character {0}\n", chnum);
                if (m_glyphs[chnum / 256] == null || m_glyphs[chnum / 256].Count == 0)
                {
                    //try
                    {
                        m_glyphs[chnum / 256] = new List<glyph>(256);  // new glyph[256];
                        for (int i = 0; i < 256; i++)
                            m_glyphs[chnum / 256].Add(new glyph());
                    }
                    //catch (Exception )
                    //{
                    //    global.osd_printf_error("render_font::load_cached: allocation error\n");
                    //    m_rawdata.clear();
                    //    return false;
                    //}
                }

                // fill in the entry
                glyph gl = m_glyphs[chnum / 256][chnum % 256];
                gl.width = entry.get_x_advance();
                gl.xoffs = entry.get_bb_x_offset();
                gl.yoffs = entry.get_bb_y_offset();
                gl.bmwidth = entry.get_bb_width();
                gl.bmheight = entry.get_bb_height();
                gl.rawdata = new Pointer<u8>(m_rawdata, (int)offset);

                // advance the offset past the character
                offset += (size_t)((gl.bmwidth * gl.bmheight + 7) / 8);
                if (m_rawdata.size() < offset)
                {
                    osd_printf_verbose("render_font::load_cached: BDC file too small to hold all glyphs\n");
                    m_rawdata.clear();
                    return false;
                }
            }

            // got everything
            m_format = format.CACHED;
            return true;
        }


        //-------------------------------------------------
        //  save_cached - save a font in cached format
        //-------------------------------------------------

        bool save_cached(string filename, u64 length, u32 hash)
        {
            throw new emu_unimplemented();
#if false
#endif
        }


        void render_font_command_glyph()
        {
            // FIXME: this is copy/pasta from the BDC loading, and it shouldn't be injected into every font
            emu_file file = new emu_file(OPEN_FLAG_READ);
            if (!file.open_ram(new MemoryU8(font_uicmd14), (u32)font_uicmd14.Length))
            {
                // get the file size, read the header, and check that it looks good
                u64 filesize = file.size();
                bdc_header header = new bdc_header();
                if (!header.read(file))
                {
                    osd_printf_warning("render_font::render_font_command_glyph: error reading BDC header\n");
                    file.close();
                    return;
                }
                else if (!header.check_magic() || (bdc_header.MAJVERSION != header.get_major_version()) || (bdc_header.MINVERSION != header.get_minor_version()))
                {
                    LOG("render_font::render_font_command_glyph: incompatible BDC file\n");
                    file.close();
                    return;
                }

                // get global properties from the header
                m_height_cmd = header.get_height();
                m_yoffs_cmd = header.get_y_offset();
                u32 numchars = header.get_glyph_count();
                if ((file.tell() + ((u64)numchars * bdc_table_entry.size())) > filesize)
                {
                    LOG("render_font::render_font_command_glyph: BDC file is too small to hold glyph table\n");
                    file.close();
                    return;
                }

                // now read the rest of the data
                u64 remaining = filesize - file.tell();
                //try
                {
                    m_rawdata_cmd.resize(remaining);
                }
                //catch (...)
                //{
                //    global.osd_printf_error("render_font::render_font_command_glyph: allocation error\n");
                //}

                for (u64 bytes_read = 0; remaining > bytes_read; )
                {
                    u32 chunk = (u32)std.min(u32.MaxValue, remaining);
                    if (file.read(new Pointer<u8>(m_rawdata_cmd, (int)bytes_read), chunk) != chunk)
                    {
                        osd_printf_error("render_font::render_font_command_glyph: error reading BDC data\n");
                        m_rawdata_cmd.clear();
                        file.close();
                        return;
                    }
                    bytes_read += chunk;
                }

                file.close();

                // extract the data from the data
                size_t offset = (size_t)numchars * bdc_table_entry.size();
                bdc_table_entry entry = new bdc_table_entry(m_rawdata_cmd.empty() ? null : new Pointer<u8>(m_rawdata_cmd));
                for (unsigned chindex = 0; chindex < numchars; chindex++, entry = entry.get_next())
                {
                    // if we don't have a subtable yet, make one
                    int chnum = (int)entry.get_encoding();
                    LOG("render_font::render_font_command_glyph: loading character {0}\n", chnum);
                    if (m_glyphs_cmd[chnum / 256] == null)
                    {
                        //try
                        {
                            m_glyphs_cmd[chnum / 256] = new List<glyph>(256);  // new glyph[256];
                            for (int i = 0; i < 256; i++)
                                m_glyphs_cmd[chnum / 256].Add(new glyph());
                        }
                        //catch (...)
                        //{
                        //    osd_printf_error("render_font::render_font_command_glyph: allocation error\n");
                        //    m_rawdata_cmd.clear();
                        //    return;
                        //}
                    }

                    // fill in the entry
                    glyph gl = m_glyphs_cmd[chnum / 256][chnum % 256];
                    gl.width = entry.get_x_advance();
                    gl.xoffs = entry.get_bb_x_offset();
                    gl.yoffs = entry.get_bb_y_offset();
                    gl.bmwidth = entry.get_bb_width();
                    gl.bmheight = entry.get_bb_height();
                    gl.rawdata = new Pointer<u8>(m_rawdata_cmd, (int)offset);

                    // advance the offset past the character
                    offset += (size_t)((gl.bmwidth * gl.bmheight + 7) / 8);
                    if (m_rawdata_cmd.size() < offset)
                    {
                        osd_printf_verbose("render_font::render_font_command_glyph: BDC file too small to hold all glyphs\n");
                        m_rawdata_cmd.clear();
                        return;
                    }
                }
            }
        }


        //-------------------------------------------------
        //  next_line - return a pointer to the start of
        //  the next line
        //-------------------------------------------------
        static Pointer<u8> next_line(Pointer<u8> ptr)  //static const char *next_line(const char *ptr)
        {
            // scan forward until we hit the end or a carriage return
            while (ptr[0] != 13 && ptr[0] != 10 && ptr[0] != 0)
                ptr++;

            // if we hit the end, return NULL
            if (ptr[0] == 0)
                return null;

            // eat the trailing linefeed if present
            ptr++;
            if (ptr[0] == 10)
                ptr++;

            return ptr;
        }
    }


    public static class rendfont_global
    {
        public static string convert_command_glyph(ref string str)
        {
            // TODO check accuracy
            //throw new emu_unimplemented();

            std.vector<char> buf = new std.vector<char>(2 * (str.length() + 1));
            size_t j = 0;
            while (!str.empty())
            {
                // decode UTF-8
                char uchar;  //char32_t uchar;
                int codelen = uchar_from_utf8(out uchar, str);  //int const codelen(uchar_from_utf8(&uchar, str));
                if (0 >= codelen)
                    break;
                str = str.Substring(codelen);  //str.remove_prefix(codelen);

                // check for three metacharacters
                fix_command_t [] fixcmd = null;  //fix_command_t const *fixcmd(nullptr);
                int fixcmdOffset = 0;
                switch (uchar)
                {
                case COMMAND_CONVERT_TEXT:
                    for (int fixtextOffset = 0; convert_text[fixtextOffset].glyph_code != 0; ++fixtextOffset)  //for (fix_strings_t const *fixtext = convert_text; fixtext->glyph_code; ++fixtext)
                    {
                        var fixtext = convert_text[fixtextOffset];
                        if (str.substr(0, fixtext.glyph_str.length()) == fixtext.glyph_str)
                        {
                            uchar = (char)(fixtext.glyph_code + COMMAND_UNICODE);
                            str = str.Substring((int)fixtext.glyph_str.length());  //str.remove_prefix(fixtext->glyph_str.length());
                            break;
                        }
                    }
                    break;

                case COMMAND_DEFAULT_TEXT:
                    fixcmd = default_text;
                    break;

                case COMMAND_EXPAND_TEXT:
                    fixcmd = expand_text;
                    break;
                }

                // this substitutes a single character
                if (fixcmd != null && !str.empty())
                {
                    if (str[0] == uchar)
                    {
                        str = str.Substring(1);  //str.remove_prefix(1);
                    }
                    else
                    {
                        while (fixcmd[fixcmdOffset].glyph_code != 0 && !str.empty() && fixcmd[fixcmdOffset].glyph_char != str[0])
                            ++fixcmdOffset;

                        if (fixcmd[fixcmdOffset].glyph_code != 0 && !str.empty())
                        {
                            uchar = (char)(COMMAND_UNICODE + fixcmd[fixcmdOffset].glyph_code);
                            str = str.Substring(1);  //str.remove_prefix(1);
                        }
                    }
                }

                // copy character to output
                string temp;
                int outlen = utf8_from_uchar(out temp, /*buf.Length - j,*/ uchar);  //int const outlen(utf8_from_uchar(&buf[j], buf.size() - j, uchar));
                if (0 >= outlen)
                    break;

                for (int tempi = 0; tempi < temp.Length; tempi++)
                    buf[(int)j + tempi] = temp[tempi];

                j += (size_t)outlen;
            }

            return new string(buf.ToArray()).Substring(0, (int)j);  //return std::string(&buf[0], j);
        }
    }


    class bdc_header
    {
        public const unsigned MAJVERSION = 1;
        public const unsigned MINVERSION = 0;

        const size_t OFFS_MAGIC      = 0x00; // 0x06 bytes
        const size_t OFFS_MAJVERSION = 0x06; // 0x01 bytes (binary integer)
        const size_t OFFS_MINVERSION = 0x07; // 0x01 bytes (binary integer)
        const size_t OFFS_ORIGLENGTH = 0x08; // 0x08 bytes (big-endian binary integer)
        const size_t OFFS_ORIGHASH   = 0x10; // 0x04 bytes
        const size_t OFFS_GLYPHCOUNT = 0x14; // 0x04 bytes (big-endian binary integer)
        const size_t OFFS_HEIGHT     = 0x18; // 0x02 bytes (big-endian binary integer)
        const size_t OFFS_YOFFSET    = 0x1a; // 0x02 bytes (big-endian binary integer)
        const size_t OFFS_DEFCHAR    = 0x1c; // 0x04 bytes (big-endian binary integer)
        const size_t OFFS_END        = 0x20;

        static readonly MemoryU8 MAGIC = new MemoryU8((int)(OFFS_MAJVERSION - OFFS_MAGIC)) { Convert.ToByte('b'), Convert.ToByte('d'), Convert.ToByte('c'), Convert.ToByte('f'), Convert.ToByte('n'), Convert.ToByte('t') };  //u8 const bdc_header::MAGIC[OFFS_MAJVERSION - OFFS_MAGIC] = { 'b', 'd', 'c', 'f', 'n', 't' };


        MemoryU8 m_data = new MemoryU8((int)OFFS_END, true);  //u8                              m_data[OFFS_END];


        public bool read(emu_file f)
        {
            return f.read(new Pointer<u8>(m_data), (u32)m_data.Count) == m_data.Count;
        }
        //bool write(emu_file &f)
        //{
        //    return f.write(m_data, sizeof(m_data)) == sizeof(m_data);
        //}

        public bool check_magic()
        {
            return std.memcmp(new Pointer<u8>(MAGIC), new Pointer<u8>(m_data, (int)OFFS_MAGIC), OFFS_MAJVERSION - OFFS_MAGIC) == 0;
        }
        public unsigned get_major_version()
        {
            return m_data[OFFS_MAJVERSION];
        }
        public unsigned get_minor_version()
        {
            return m_data[OFFS_MINVERSION];
        }
        public u64 get_original_length()
        {
            return
                    ((u64)(m_data[OFFS_ORIGLENGTH + 0]) << (7 * 8)) |
                    ((u64)(m_data[OFFS_ORIGLENGTH + 1]) << (6 * 8)) |
                    ((u64)(m_data[OFFS_ORIGLENGTH + 2]) << (5 * 8)) |
                    ((u64)(m_data[OFFS_ORIGLENGTH + 3]) << (4 * 8)) |
                    ((u64)(m_data[OFFS_ORIGLENGTH + 4]) << (3 * 8)) |
                    ((u64)(m_data[OFFS_ORIGLENGTH + 5]) << (2 * 8)) |
                    ((u64)(m_data[OFFS_ORIGLENGTH + 6]) << (1 * 8)) |
                    ((u64)(m_data[OFFS_ORIGLENGTH + 7]) << (0 * 8));
        }
        public u32 get_original_hash()
        {
            return
                    ((u32)(m_data[OFFS_ORIGHASH + 0]) << (3 * 8)) |
                    ((u32)(m_data[OFFS_ORIGHASH + 1]) << (2 * 8)) |
                    ((u32)(m_data[OFFS_ORIGHASH + 2]) << (1 * 8)) |
                    ((u32)(m_data[OFFS_ORIGHASH + 3]) << (0 * 8));
        }
        public u32 get_glyph_count()
        {
            return
                    ((u32)(m_data[OFFS_GLYPHCOUNT + 0]) << (3 * 8)) |
                    ((u32)(m_data[OFFS_GLYPHCOUNT + 1]) << (2 * 8)) |
                    ((u32)(m_data[OFFS_GLYPHCOUNT + 2]) << (1 * 8)) |
                    ((u32)(m_data[OFFS_GLYPHCOUNT + 3]) << (0 * 8));
        }
        public u16 get_height()
        {
            return
                    (u16)(((u16)(m_data[OFFS_HEIGHT + 0]) << (1 * 8)) |
                    ((u16)(m_data[OFFS_HEIGHT + 1]) << (0 * 8)));
        }
        public s16 get_y_offset()
        {
            return
                    (s16)(((u16)(m_data[OFFS_YOFFSET + 0]) << (1 * 8)) |
                    ((u16)(m_data[OFFS_YOFFSET + 1]) << (0 * 8)));
        }
        public s32 get_default_character()
        {
            return
                    (s32)(((u32)(m_data[OFFS_DEFCHAR + 0]) << (3 * 8)) |
                    ((u32)(m_data[OFFS_DEFCHAR + 1]) << (2 * 8)) |
                    ((u32)(m_data[OFFS_DEFCHAR + 2]) << (1 * 8)) |
                    ((u32)(m_data[OFFS_DEFCHAR + 3]) << (0 * 8)));
        }

        //void set_magic()
        //{
        //    std::memcpy(m_data + OFFS_MAGIC, MAGIC, OFFS_MAJVERSION - OFFS_MAGIC);
        //}
        //void set_version()
        //{
        //    m_data[OFFS_MAJVERSION] = MAJVERSION;
        //    m_data[OFFS_MINVERSION] = MINVERSION;
        //}
        //void set_original_length(u64 value)
        //{
        //    m_data[OFFS_ORIGLENGTH + 0] = u8((value >> (7 * 8)) & 0x00ff);
        //    m_data[OFFS_ORIGLENGTH + 1] = u8((value >> (6 * 8)) & 0x00ff);
        //    m_data[OFFS_ORIGLENGTH + 2] = u8((value >> (5 * 8)) & 0x00ff);
        //    m_data[OFFS_ORIGLENGTH + 3] = u8((value >> (4 * 8)) & 0x00ff);
        //    m_data[OFFS_ORIGLENGTH + 4] = u8((value >> (3 * 8)) & 0x00ff);
        //    m_data[OFFS_ORIGLENGTH + 5] = u8((value >> (2 * 8)) & 0x00ff);
        //    m_data[OFFS_ORIGLENGTH + 6] = u8((value >> (1 * 8)) & 0x00ff);
        //    m_data[OFFS_ORIGLENGTH + 7] = u8((value >> (0 * 8)) & 0x00ff);
        //}
        //void set_original_hash(u32 value)
        //{
        //    m_data[OFFS_ORIGHASH + 0] = u8((value >> (3 * 8)) & 0x00ff);
        //    m_data[OFFS_ORIGHASH + 1] = u8((value >> (2 * 8)) & 0x00ff);
        //    m_data[OFFS_ORIGHASH + 2] = u8((value >> (1 * 8)) & 0x00ff);
        //    m_data[OFFS_ORIGHASH + 3] = u8((value >> (0 * 8)) & 0x00ff);
        //}
        //void set_glyph_count(u32 value)
        //{
        //    m_data[OFFS_GLYPHCOUNT + 0] = u8((value >> (3 * 8)) & 0x00ff);
        //    m_data[OFFS_GLYPHCOUNT + 1] = u8((value >> (2 * 8)) & 0x00ff);
        //    m_data[OFFS_GLYPHCOUNT + 2] = u8((value >> (1 * 8)) & 0x00ff);
        //    m_data[OFFS_GLYPHCOUNT + 3] = u8((value >> (0 * 8)) & 0x00ff);
        //}
        //void set_height(u16 value)
        //{
        //    m_data[OFFS_HEIGHT + 0] = u8((value >> (1 * 8)) & 0x00ff);
        //    m_data[OFFS_HEIGHT + 1] = u8((value >> (0 * 8)) & 0x00ff);
        //}
        //void set_y_offset(s16 value)
        //{
        //    m_data[OFFS_YOFFSET + 0] = u8((value >> (1 * 8)) & 0x00ff);
        //    m_data[OFFS_YOFFSET + 1] = u8((value >> (0 * 8)) & 0x00ff);
        //}
        //void set_default_character(s32 value)
        //{
        //    m_data[OFFS_DEFCHAR + 0] = u8((value >> (3 * 8)) & 0x00ff);
        //    m_data[OFFS_DEFCHAR + 1] = u8((value >> (2 * 8)) & 0x00ff);
        //    m_data[OFFS_DEFCHAR + 2] = u8((value >> (1 * 8)) & 0x00ff);
        //    m_data[OFFS_DEFCHAR + 3] = u8((value >> (0 * 8)) & 0x00ff);
        //}
    }


    class bdc_table_entry
    {
        const size_t OFFS_ENCODING   = 0x00; // 0x04 bytes (big-endian binary integer)
        const size_t OFFS_XADVANCE   = 0x04; // 0x02 bytes (big-endian binary integer)
        // two bytes reserved
        const size_t OFFS_BBXOFFSET  = 0x08; // 0x02 bytes (big-endian binary integer)
        const size_t OFFS_BBYOFFSET  = 0x0a; // 0x02 bytes (big-endian binary integer)
        const size_t OFFS_BBWIDTH    = 0x0c; // 0x02 bytes (big-endian binary integer)
        const size_t OFFS_BBHEIGHT   = 0x0e; // 0x02 bytes (big-endian binary integer)
        const size_t OFFS_END        = 0x10;

        Pointer<u8> m_ptr;  //u8                              *m_ptr;


        public bdc_table_entry(Pointer<u8> bytes)  //bdc_table_entry(void *bytes)
        {
            m_ptr = bytes;
        }
        //bdc_table_entry(bdc_table_entry const &that) = default;
        //bdc_table_entry(bdc_table_entry &&that) = default;

        public bdc_table_entry get_next()
        {
            return new bdc_table_entry(new Pointer<u8>(m_ptr, (int)OFFS_END));
        }

        public u32 get_encoding()
        {
            return
                    ((u32)(m_ptr[OFFS_ENCODING + 0]) << (3 * 8)) |
                    ((u32)(m_ptr[OFFS_ENCODING + 1]) << (2 * 8)) |
                    ((u32)(m_ptr[OFFS_ENCODING + 2]) << (1 * 8)) |
                    ((u32)(m_ptr[OFFS_ENCODING + 3]) << (0 * 8));
        }
        public u16 get_x_advance()
        {
            return
                    (u16)(((u16)(m_ptr[OFFS_XADVANCE + 0]) << (1 * 8)) |
                    ((u16)(m_ptr[OFFS_XADVANCE + 1]) << (0 * 8)));
        }
        public s16 get_bb_x_offset()
        {
            return
                    (s16)(((u16)(m_ptr[OFFS_BBXOFFSET + 0]) << (1 * 8)) |
                    ((u16)(m_ptr[OFFS_BBXOFFSET + 1]) << (0 * 8)));
        }
        public s16 get_bb_y_offset()
        {
            return
                    (s16)(((u16)(m_ptr[OFFS_BBYOFFSET + 0]) << (1 * 8)) |
                    ((u16)(m_ptr[OFFS_BBYOFFSET + 1]) << (0 * 8)));
        }
        public u16 get_bb_width()
        {
            return
                    (u16)(((u16)(m_ptr[OFFS_BBWIDTH + 0]) << (1 * 8)) |
                    ((u16)(m_ptr[OFFS_BBWIDTH + 1]) << (0 * 8)));
        }
        public u16 get_bb_height()
        {
            return
                    (u16)(((u16)(m_ptr[OFFS_BBHEIGHT + 0]) << (1 * 8)) |
                    ((u16)(m_ptr[OFFS_BBHEIGHT + 1]) << (0 * 8)));
        }

        //void set_encoding(u32 value)
        //{
        //    m_ptr[OFFS_ENCODING + 0] = u8((value >> (3 * 8)) & 0x00ff);
        //    m_ptr[OFFS_ENCODING + 1] = u8((value >> (2 * 8)) & 0x00ff);
        //    m_ptr[OFFS_ENCODING + 2] = u8((value >> (1 * 8)) & 0x00ff);
        //    m_ptr[OFFS_ENCODING + 3] = u8((value >> (0 * 8)) & 0x00ff);
        //}
        //void set_x_advance(u16 value)
        //{
        //    m_ptr[OFFS_XADVANCE + 0] = u8((value >> (1 * 8)) & 0x00ff);
        //    m_ptr[OFFS_XADVANCE + 1] = u8((value >> (0 * 8)) & 0x00ff);
        //}
        //void set_bb_x_offset(s16 value)
        //{
        //    m_ptr[OFFS_BBXOFFSET + 0] = u8((value >> (1 * 8)) & 0x00ff);
        //    m_ptr[OFFS_BBXOFFSET + 1] = u8((value >> (0 * 8)) & 0x00ff);
        //}
        //void set_bb_y_offset(s16 value)
        //{
        //    m_ptr[OFFS_BBYOFFSET + 0] = u8((value >> (1 * 8)) & 0x00ff);
        //    m_ptr[OFFS_BBYOFFSET + 1] = u8((value >> (0 * 8)) & 0x00ff);
        //}
        //void set_bb_width(u16 value)
        //{
        //    m_ptr[OFFS_BBWIDTH + 0] = u8((value >> (1 * 8)) & 0x00ff);
        //    m_ptr[OFFS_BBWIDTH + 1] = u8((value >> (0 * 8)) & 0x00ff);
        //}
        //void set_bb_height(u16 value)
        //{
        //    m_ptr[OFFS_BBHEIGHT + 0] = u8((value >> (1 * 8)) & 0x00ff);
        //    m_ptr[OFFS_BBHEIGHT + 1] = u8((value >> (0 * 8)) & 0x00ff);
        //}

        //bdc_table_entry &operator=(bdc_table_entry const &that) = default;
        //bdc_table_entry &operator=(bdc_table_entry &&that) = default;

        public static size_t size()
        {
            return OFFS_END;
        }
    }
}
