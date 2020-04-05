// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using char32_t = System.UInt32;
using ListBytes = mame.ListBase<System.Byte>;
using ListBytesPointer = mame.ListPointer<System.Byte>;
using s32 = System.Int32;
using u8 = System.Byte;
using u64 = System.UInt64;


namespace mame
{
    // ======================> render_font
    // a render_font describes and provides an interface to a font
    public class render_font : global_object,
                               IDisposable
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
            public ListBytesPointer rawdata;  //const char *        rawdata;            // pointer to the raw data for this one
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
        const UInt64 CACHED_BDF_HASH_SIZE   = 1024;


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


            for (int i = 0; i < m_glyphs.Length; i++)
                m_glyphs[i] = new List<glyph>();


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
            osd_file.error filerr = ramfile.open_ram(new ListBytes(uismall_global.font_uismall), (UInt32)uismall_global.font_uismall.Length);//, sizeof(font_uismall));
            if (osd_file.error.NONE == filerr)
                load_cached(ramfile, 0, 0);

            ramfile.close();

            render_font_command_glyph();
        }

        ~render_font()
        {
            assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            //throw new emu_unimplemented();
#if false
            // free all the subtables
            foreach (var elem in m_glyphs)
            {
                if (elem != null)
                {
                    for (int charnum = 0; charnum < 256; charnum++)
                    {
                        glyph gl = elem[charnum];
                        m_manager.texture_free(gl.texture);
                    }
                    //delete[] elem;
                }
            }

            foreach (var elem in m_glyphs_cmd)
            {
                if (elem != null)
                {
                    for (int charnum = 0; charnum < 256; charnum++)
                    {
                        glyph gl = elem[charnum];
                        m_manager.texture_free(gl.texture);
                    }
                    //delete[] elem;
                }
            }
#endif

            m_isDisposed = true;
        }


        // getters
        render_manager manager() { return m_manager; }


        // size queries
        public int pixel_height() { return m_height; }

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
            foreach (var c in str)  //for (const unsigned char *ptr = (const unsigned char *)string; *ptr != 0; ptr++)
                totwidth += get_char(c).width;

            // scale the final result based on height
            return (float)(totwidth) * m_scale * height * aspect;
        }


        //-------------------------------------------------
        //  utf8string_width - return the width of a
        //  UTF8-encoded string at the given height
        //-------------------------------------------------
        public float utf8string_width(float height, float aspect, string utf8string)
        {
            int length = utf8string.Length;

            // loop over the string and accumulate widths
            int count;
            s32 totwidth = 0;
            for (int offset = 0; offset < length; offset += count)
            {
                char uchar;  //char32_t uchar;
                count = unicode_global.uchar_from_utf8(out uchar, utf8string.Substring(offset), length - offset);
                if (count < 0)
                    break;

                totwidth += get_char(uchar).width;
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
            bounds = new rectangle();

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
            UInt32 page = chnum / 256;
            if (page >= m_glyphs.Length)
            {
                if ((0 <= m_defchar) && (chnum != m_defchar))
                    return get_char((UInt32)m_defchar);
                else
                    return get_char_dummy_glyph;
            }
            else if (m_glyphs[page] == null || m_glyphs[page].Count == 0)
            {
                //mamep: make table for command glyph
                if ((m_format == format.OSD) || ((chnum >= cmddata_global.COMMAND_UNICODE) && (chnum < cmddata_global.COMMAND_UNICODE + cmddata_global.MAX_GLYPH_FONT)))
                {
                    m_glyphs[page] = new List<glyph>(256);  // new glyph[256];
                    for (int i = 0; i < 256; i++)
                        m_glyphs[page].Add(new glyph());
                }
                else if ((0 <= m_defchar) && (chnum != m_defchar))
                {
                    return get_char((UInt32)m_defchar);
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
                if (m_height_cmd != 0 && chnum >= cmddata_global.COMMAND_UNICODE && chnum < cmddata_global.COMMAND_UNICODE + cmddata_global.MAX_GLYPH_FONT)
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
            bool is_cmd = ((chnum >= cmddata_global.COMMAND_UNICODE) && (chnum < cmddata_global.COMMAND_UNICODE + cmddata_global.MAX_GLYPH_FONT));

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
                    UINT32 *dest = (desty >= 0 && desty < m_height_cmd) ? &gl.bitmap.pix32(desty, 0) : null;
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
                ListBytesPointer ptr = new ListBytesPointer(gl.rawdata);  //const char *ptr = gl.rawdata;
                byte accum = 0;
                byte accumbit = 7;
                for (int y = 0; y < gl.bmheight; y++)
                {
                    int desty = y + m_height + m_yoffs - gl.yoffs - gl.bmheight;
                    //UINT32 *dest = (desty >= 0 && desty < m_height) ? &gl.bitmap.pix32(desty) : null;
                    RawBuffer destBuf = null;
                    UInt32 destBufOffset = 0;
                    if (0 <= desty && m_height > desty)
                        destBufOffset = gl.bitmap.pix32(out destBuf, desty);

                    if (m_format == format.TEXT)
                    {
                        if (destBuf != null)
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

                                //*dest++ = (bits & 8) ? fgcol : bgcol;
                                destBuf.set_uint32((int)destBufOffset++, (bits & 8) != 0 ? fgcol : bgcol);
                                if (gl.bmwidth > ++x)
                                    //*dest++ = (bits & 4) ? fgcol : bgcol;
                                    destBuf.set_uint32((int)destBufOffset++, (bits & 4) != 0 ? fgcol : bgcol);
                                if (gl.bmwidth > ++x)
                                    //*dest++ = (bits & 2) ? fgcol : bgcol;
                                    destBuf.set_uint32((int)destBufOffset++, (bits & 2) != 0 ? fgcol : bgcol);
                                if (gl.bmwidth > ++x)
                                    //*dest++ = (bits & 1) ? fgcol : bgcol;
                                    destBuf.set_uint32((int)destBufOffset++, (bits & 1) != 0 ? fgcol : bgcol);

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
                            if (destBuf != null)
                                //*dest++ = (accum & (1 << accumbit)) ? fgcol : bgcol;
                                destBuf.set_uint32((int)destBufOffset++, (accum & (1 << accumbit)) != 0 ? fgcol : bgcol);
                            accumbit = (byte)((accumbit - 1) & 7);
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
            osd_file.error filerr;
            UInt32 chunk;
            UInt64 bytes;

            // first try to open the BDF itself
            emu_file file = new emu_file(manager().machine().options().font_path(), OPEN_FLAG_READ);
            filerr = file.open(filename);
            if (filerr != osd_file.error.NONE)
                return false;

            file.close();

            throw new emu_unimplemented();
#if false
            LOOK AT ORIGINAL SOURCE

            // determine the file size and allocate memory
            m_rawsize = file.size();
            m_rawdata.resize((int)m_rawsize + 1);

            // read the first chunk
            UInt32 bytes = file.read(new dynamic_buffer_pointer(m_rawdata), (UInt32)Math.Min(CACHED_BDF_HASH_SIZE, m_rawsize));
            if (bytes != Math.Min(CACHED_BDF_HASH_SIZE, m_rawsize))
                return false;

            // has the chunk
            UInt32 hash = coreutil_global.core_crc32(0, new dynamic_buffer_pointer(m_rawdata), bytes) ^ (UInt32)m_rawsize;

            // create the cached filename, changing the 'F' to a 'C' on the extension
            string cachedname = filename;
            cachedname = cachedname.Remove(cachedname.Length - 3, 3) + "bdc";

            // attempt to open the cached version of the font
            {
                emu_file cachefile = new emu_file(manager().machine().options().font_path(), osdcore_global.OPEN_FLAG_READ);
                filerr = cachefile.open(cachedname);
                if (filerr == file_error.FILERR_NONE)
                {
                    // if we have a cached version, load it
                    bool cachedresult = load_cached(cachefile, hash);

                    // if that worked, we're done
                    if (cachedresult)
                    {
                        // don't do that - glyphs data point into this array ...
                        // m_rawdata.reset();
                        return true;
                    }
                }
            }

            // read in the rest of the font
            if (bytes < m_rawsize)
            {
                UInt32 read = file.read(new dynamic_buffer_pointer(m_rawdata, (int)bytes), (UInt32)(m_rawsize - bytes));  // m_rawdata + bytes, m_rawsize - bytes);
                if (read != m_rawsize - bytes)
                {
                    m_rawdata.Clear();
                    return false;
                }
            }

            // NULL-terminate the data and attach it to the font
            m_rawdata[(int)m_rawsize] = 0;

            // load the BDF
            bool result = load_bdf();

            // if we loaded okay, create a cached one
            if (result)
                save_cached(cachedname, hash);

            // close the file
            return result;
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
            bdf_helper<std::vector<char>.const_iterator> helper = new bdf_helper<std::vector<char>.const_iterator>(std::cbegin(m_rawdata), std::cend(m_rawdata));

            // the first thing we want to see is the STARTFONT declaration, failing that we can't do much
            for ( ; !helper.is_keyword("STARTFONT"); helper.next_line())
            {
                if (helper.at_end())
                {
                    osd_printf_error("render_font::load_bdf: expected STARTFONT\n");
                    return false;
                }
            }

            // parse out the global information we need
            bool have_bbox = false;
            bool have_properties = false;
            bool have_defchar = false;
            for (helper.next_line(); !helper.is_keyword("CHARS"); helper.next_line())
            {
                if (helper.at_end())
                {
                    // font with no characters is useless
                    osd_printf_error("render_font::load_bdf: no glyph section found\n");
                    return false;
                }
                else if (helper.is_keyword("FONTBOUNDINGBOX"))
                {
                    // check for duplicate bounding box
                    if (have_bbox)
                    {
                        osd_printf_error("render_font::load_bdf: found additional bounding box \"%.*s\"\n", (int)(helper.value_length()), &*helper.value_begin());
                        return false;
                    }
                    have_bbox = true;

                    // parse bounding box and check that it's at least half sane
                    int width, xoffs;
                    if (4 == sscanf(&*helper.value_begin(), "%d %d %d %d", &width, &m_height, &xoffs, &m_yoffs))
                    {
                        LOG("render_font::load_bdf: got bounding box %dx%d %d,%d\n", width, m_height, xoffs, m_yoffs);
                        if ((0 >= m_height) || (0 >= width))
                        {
                            osd_printf_error("render_font::load_bdf: bounding box is invalid\n");
                            return false;
                        }
                    }
                    else
                    {
                        osd_printf_error("render_font::load_bdf: failed to parse bounding box \"%.*s\"\n", (int)(helper.value_length()), &*helper.value_begin());
                        return false;
                    }
                }
                else if (helper.is_keyword("STARTPROPERTIES"))
                {
                    // check for duplicated properties section
                    if (have_properties)
                    {
                        osd_printf_error("render_font::load_bdf: found additional properties\n");
                        return false;
                    }
                    have_properties = true;

                    // get property count for sanity check
                    int propcount;
                    if (1 != sscanf(&*helper.value_begin(), "%d", &propcount))
                    {
                        osd_printf_error("render_font::load_bdf: failed to parse property count \"%.*s\"\n", (int)(helper.value_length()), &*helper.value_begin());
                        return false;
                    }

                    int actual = 0;
                    for (helper.next_line(); !helper.is_keyword("ENDPROPERTIES"); helper.next_line())
                    {
                        ++actual;
                        if (helper.at_end())
                        {
                            // unterminated properties section
                            osd_printf_error("render_font::load_bdf: end of properties not found\n");
                            return false;
                        }
                        else if (helper.is_keyword("DEFAULT_CHAR"))
                        {
                            // check for duplicate default character
                            if (have_defchar)
                            {
                                osd_printf_error("render_font::load_bdf: found additional default character \"%.*s\"\n", (int)(helper.value_length()), &*helper.value_begin());
                                return false;
                            }
                            have_defchar = true;

                            // parse default character
                            if (1 == sscanf(&*helper.value_begin(), "%d", &m_defchar))
                            {
                                LOG("render_font::load_bdf: got default character 0x%x\n", m_defchar);
                            }
                            else
                            {
                                osd_printf_error("render_font::load_bdf: failed to parse default character \"%.*s\"\n", (int)(helper.value_length()), &*helper.value_begin());
                                return false;
                            }
                        }
                    }

                    // sanity check on number of properties
                    if (actual != propcount)
                    {
                        osd_printf_error("render_font::load_bdf: incorrect number of properties %d\n", actual);
                        return false;
                    }
                }
            }

            // compute the scale factor
            if (!have_bbox)
            {
                osd_printf_error("render_font::load_bdf: no bounding box found\n");
                return false;
            }
            m_scale = 1.0f / (float)(m_height);

            // get expected character count
            int expected;
            if (1 == sscanf(&*helper.value_begin(), "%d", &expected))
            {
                LOG("render_font::load_bdf: got character count %d\n", expected);
            }
            else
            {
                osd_printf_error("render_font::load_bdf: failed to parse character count \"%.*s\"\n", (int)(helper.value_length()), &*helper.value_begin());
                return false;
            }

            // now scan for characters
            //auto const nothex([] (char ch) { return (('0' > ch) || ('9' < ch)) && (('A' > ch) || ('Z' < ch)) && (('a' > ch) || ('z' < ch)); });
            int charcount = 0;
            for (helper.next_line(); !helper.is_keyword("ENDFONT"); helper.next_line())
            {
                if (helper.at_end())
                {
                    // unterminated font
                    osd_printf_error("render_font::load_bdf: end of font not found\n");
                    return false;
                }
                else if (helper.is_keyword("STARTCHAR"))
                {
                    // required glyph properties
                    bool have_encoding = false;
                    bool have_advance = false;
                    bool have_bbounds = false;
                    int encoding = -1;
                    int xadvance = -1;
                    int bbw = -1;
                    int bbh = -1;
                    int bbxoff = -1;
                    int bbyoff = -1;

                    // stuff for the bitmap data
                    bool in_bitmap = false;
                    int bitmap_rows = 0;
                    //char const *bitmap_data(nullptr);

                    // parse a glyph
                    for (helper.next_line(); !helper.is_keyword("ENDCHAR"); helper.next_line())
                    {
                        if (helper.at_end())
                        {
                            // unterminated glyph
                            osd_printf_error("render_font::load_bdf: end of glyph not found\n");
                            return false;
                        }
                        else if (in_bitmap)
                        {
                            // quick sanity check
                            if ((2 * ((bbw + 7) / 8)) != helper.keyword_length())
                            {
                                osd_printf_error("render_font::load_bdf: incorrect length for bitmap line \"%.*s\"\n", (int)(helper.keyword_length()), &*helper.keyword_begin());
                                return false;
                            }
                            else if (std::find_if(helper.keyword_begin(), helper.keyword_end(), nothex) != helper.keyword_end())
                            {
                                osd_printf_error("render_font::load_bdf: found invalid character in bitmap line \"%.*s\"\n", (int)(helper.keyword_length()), &*helper.keyword_begin());
                                return false;
                            }

                            // track number of rows
                            if (1 == ++bitmap_rows)
                                bitmap_data = &*helper.keyword_begin();

                        }
                        else if (helper.is_keyword("ENCODING"))
                        {
                            // check for duplicate glyph encoding
                            if (have_encoding)
                            {
                                osd_printf_error("render_font::load_bdf: found additional glyph encoding \"%.*s\"\n", (int)(helper.value_length()), &*helper.value_begin());
                                return false;
                            }
                            have_encoding = true;

                            // need to support Adobe Standard Encoding "123" and non-standard glyph index "-1 123"
                            string value = new string(helper.value_begin(), helper.value_end());
                            int aux;
                            int cnt = sscanf(value.c_str(), "%d %d", &encoding, &aux);
                            if ((2 == cnt) && (-1 == encoding) && (0 <= aux))
                            {
                                encoding = aux;
                            }
                            else if ((1 != cnt) || (0 > encoding))
                            {
                                osd_printf_error("render_font::load_bdf: failed to parse glyph encoding \"%.*s\"\n", (int)(helper.value_length()), &*helper.value_begin());
                                return false;
                            }
                            LOG("render_font::load_bdf: got glyph encoding %d\n", encoding);
                        }
                        else if (helper.is_keyword("DWIDTH"))
                        {
                            // check for duplicate advance
                            if (have_advance)
                            {
                                osd_printf_error("render_font::load_bdf: found additional pixel width \"%.*s\"\n", (int)(helper.value_length()), &*helper.value_begin());
                                return false;
                            }
                            have_advance = true;

                            // completely ignore vertical advance
                            int yadvance;
                            if (2 == sscanf(&*helper.value_begin(), "%d %d", &xadvance, &yadvance))
                            {
                                LOG("render_font::load_bdf: got pixel width %d,%d\n", xadvance, yadvance);
                            }
                            else
                            {
                                osd_printf_error("render_font::load_bdf: failed to parse pixel width \"%.*s\"\n", (int)(helper.value_length()), &*helper.value_begin());
                                return false;
                            }
                        }
                        else if (helper.is_keyword("BBX"))
                        {
                            // check for duplicate black pixel box
                            if (have_bbounds)
                            {
                                osd_printf_error("render_font::load_bdf: found additional pixel width \"%.*s\"\n", (int)(helper.value_length()), &*helper.value_begin());
                                return false;
                            }
                            have_bbounds = true;

                            // extract position/size of black pixel area
                            if (4 == sscanf(&*helper.value_begin(), "%d %d %d %d", &bbw, &bbh, &bbxoff, &bbyoff))
                            {
                                LOG("render_font::load_bdf: got black pixel box %dx%d %d,%d\n", bbw, bbh, bbxoff, bbyoff);
                                if ((0 > bbw) || (0 > bbh))
                                {
                                    osd_printf_error("render_font::load_bdf: black pixel box is invalid\n");
                                    return false;
                                }
                            }
                            else
                            {
                                osd_printf_error("render_font::load_bdf: failed to parse black pixel box \"%.*s\"\n", (int)(helper.value_length()), &*helper.value_begin());
                                return false;
                            }
                        }
                        else if (helper.is_keyword("BITMAP"))
                        {
                            // this is the bitmap - we need to already have properties before we get here
                            if (!have_advance)
                            {
                                osd_printf_error("render_font::load_bdf: glyph has no pixel width\n");
                                return false;
                            }
                            else if (!have_bbounds)
                            {
                                osd_printf_error("render_font::load_bdf: glyph has no black pixel box\n");
                                return false;
                            }
                            in_bitmap = true;
                        }
                    }

                    // now check that we have what we need
                    if (!in_bitmap)
                    {
                        osd_printf_error("render_font::load_bdf: glyph has no bitmap\n");
                        return false;
                    }
                    else if (bitmap_rows != bbh)
                    {
                        osd_printf_error("render_font::load_bdf: incorrect number of bitmap lines %d\n", bitmap_rows);
                        return false;
                    }

                    // some kinds of characters will screw us up
                    if (0 > xadvance)
                    {
                        LOG("render_font::load_bdf: ignoring character with negative x advance\n");
                    }
                    else if ((256 * ARRAY_LENGTH(m_glyphs)) <= encoding)
                    {
                        LOG("render_font::load_bdf: ignoring character with encoding outside range\n");
                    }
                    else
                    {
                        // if we don't have a subtable yet, make one
                        if (!m_glyphs[encoding / 256])
                        {
                            try
                            {
                                m_glyphs[encoding / 256] = new glyph[256];
                            }
                            catch (Exception )
                            {
                                osd_printf_error("render_font::load_bdf: allocation failed\n");
                                return false;
                            }
                        }

                        // fill in the entry
                        glyph &gl = m_glyphs[encoding / 256][encoding % 256];
                        gl.width = xadvance;
                        gl.bmwidth = bbw;
                        gl.bmheight = bbh;
                        gl.xoffs = bbxoff;
                        gl.yoffs = bbyoff;
                        gl.rawdata = bitmap_data;
                    }

                    // some progress for big fonts
                    if (0 == (++charcount % 256))
                        osd_printf_warning("Loading BDF font... (%d characters loaded)\n", charcount);
                }
            }

            // check number of characters
            if (expected != charcount)
            {
                osd_printf_error("render_font::load_bdf: incorrect character count %d\n", charcount);
                return false;
            }

            // should have bailed by now if something went wrong
            return true;
#endif
        }


        //-------------------------------------------------
        //  load_cached - load a font in cached format
        //-------------------------------------------------
        bool load_cached(emu_file file, UInt64 length, UInt32 hash)
        {
            // get the file size, read the header, and check that it looks good
            UInt64 filesize = file.size();
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
            UInt32 numchars = header.get_glyph_count();
            if ((file.tell() + ((UInt64)(numchars) * bdc_table_entry.size())) > filesize)
            {
                LOG("render_font::load_cached: BDC file is too small to hold glyph table\n");
                return false;
            }

            // now read the rest of the data
            UInt64 remaining = filesize - file.tell();
            try
            {
                m_rawdata.resize((int)remaining);
            }
            catch (Exception )
            {
                osd_printf_error("render_font::load_cached: allocation error\n");
            }
            for (UInt64 bytes_read = 0; remaining > bytes_read; )
            {
                UInt32 chunk = (UInt32)Math.Min(UInt32.MaxValue, remaining);
                if (file.read(new ListBytesPointer(m_rawdata, (int)bytes_read), chunk) != chunk)  // &m_rawdata[bytes_read], chunk) != chunk)
                {
                    osd_printf_error("render_font::load_cached: error reading BDC data\n");
                    m_rawdata.clear();
                    return false;
                }
                bytes_read += chunk;
            }

            // extract the data from the data
            UInt32 offset = (UInt32)numchars * bdc_table_entry.size();
            bdc_table_entry entry = new bdc_table_entry(m_rawdata.empty() ? null : new ListBytesPointer(m_rawdata));
            for (UInt32 chindex = 0; chindex < numchars; chindex++, entry = entry.get_next())
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
                gl.rawdata = new ListBytesPointer(m_rawdata, (int)offset);

                // advance the offset past the character
                offset += (UInt32)((gl.bmwidth * gl.bmheight + 7) / 8);
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

        bool save_cached(string filename, UInt64 length, UInt32 hash)
        {
            throw new emu_unimplemented();
        }


        void render_font_command_glyph()
        {
            // FIXME: this is copy/pasta from the BDC loading, and it shouldn't be injected into every font
            emu_file file = new emu_file(OPEN_FLAG_READ);
            if (file.open_ram(new ListBytes(uicmd14_global.font_uicmd14), (UInt32)uicmd14_global.font_uicmd14.Length) == osd_file.error.NONE)
            {
                // get the file size, read the header, and check that it looks good
                UInt64 filesize = file.size();
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
                UInt32 numchars = header.get_glyph_count();
                if ((file.tell() + ((UInt64)numchars * bdc_table_entry.size())) > filesize)
                {
                    LOG("render_font::render_font_command_glyph: BDC file is too small to hold glyph table\n");
                    file.close();
                    return;
                }

                // now read the rest of the data
                UInt64 remaining = filesize - file.tell();
                //try
                {
                    m_rawdata_cmd.resize((int)remaining);
                }
                //catch (...)
                //{
                //    global.osd_printf_error("render_font::render_font_command_glyph: allocation error\n");
                //}

                for (UInt64 bytes_read = 0; remaining > bytes_read; )
                {
                    UInt32 chunk = (UInt32)Math.Min(UInt32.MaxValue, remaining);
                    if (file.read(new ListBytesPointer(m_rawdata_cmd, (int)bytes_read), chunk) != chunk)
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
                UInt32 offset = (UInt32)numchars * bdc_table_entry.size();
                bdc_table_entry entry = new bdc_table_entry(m_rawdata_cmd.empty() ? null : new ListBytesPointer(m_rawdata_cmd));
                for (UInt32 chindex = 0; chindex < numchars; chindex++, entry = entry.get_next())
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
                    gl.rawdata = new ListBytesPointer(m_rawdata_cmd, (int)offset);

                    // advance the offset past the character
                    offset += (UInt32)((gl.bmwidth * gl.bmheight + 7) / 8);
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
        static ListBytesPointer next_line(ListBytesPointer ptr)  //static const char *next_line(const char *ptr)
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


        public static void convert_command_glyph(ref string str)
        {
            // TODO check accuracy
            //throw new emu_unimplemented();

            //(void)str.c_str(); // force NUL-termination - we depend on it later
            UInt32 len = (UInt32)str.length();
            std.vector<char> buf = new std.vector<char>(2 * ((int)len + 1));
            buf.resize(2 * ((int)len + 1));
            UInt32 j = 0;
            for (UInt32 i = 0; len > i; )
            {
                // decode UTF-8
                char uchar;  //char32_t uchar;
                int codelen = unicode_global.uchar_from_utf8(out uchar, str.Substring((int)i), (int)(len - i));
                if (0 >= codelen)
                    break;
                i += (UInt32)codelen;

                // check for three metacharacters
                fix_command_t [] fixcmd = null;
                int fixcmdOffset = 0;
                switch (uchar)
                {
                case cmddata_global.COMMAND_CONVERT_TEXT:
                    fix_strings_t [] fixtext = cmddata_global.convert_text;
                    int fixtextOffset = 0;
                    for (; fixtext[fixtextOffset].glyph_code != 0; ++fixtextOffset)
                    {
                        if (fixtext[fixtextOffset].glyph_str_len == 0)
                            fixtext[fixtextOffset].glyph_str_len = (UInt32)strlen(fixtext[fixtextOffset].glyph_str);

                        if (strncmp(fixtext[fixtextOffset].glyph_str, str.Substring((int)i), (int)fixtext[fixtextOffset].glyph_str_len) == 0)
                        {
                            uchar = (char)(fixtext[fixtextOffset].glyph_code + cmddata_global.COMMAND_UNICODE);  //uchar = fixtext->glyph_code + COMMAND_UNICODE;
                            i += (UInt32)strlen(fixtext[fixtextOffset].glyph_str);
                            break;
                        }
                    }
                    break;

                case cmddata_global.COMMAND_DEFAULT_TEXT:
                    fixcmd = cmddata_global.default_text;
                    break;

                case cmddata_global.COMMAND_EXPAND_TEXT:
                    fixcmd = cmddata_global.expand_text;
                    break;
                }

                // this substitutes a single character
                if (fixcmd != null)
                {
                    if (str[(int)i] == uchar)
                    {
                        ++i;
                    }
                    else
                    {
                        while ((fixcmd[fixcmdOffset].glyph_code != 0) && (fixcmd[fixcmdOffset].glyph_char != str[(int)i]))
                            ++fixcmdOffset;

                        if (fixcmd[fixcmdOffset].glyph_code != 0)
                        {
                            uchar = (char)(cmddata_global.COMMAND_UNICODE + fixcmd[fixcmdOffset].glyph_code);
                            ++i;
                        }
                    }
                }

                // copy character to output
                string temp;
                int outlen = unicode_global.utf8_from_uchar(out temp, /*buf.Length - j,*/ uchar);
                if (0 >= outlen)
                    break;

                for (int tempi = 0; tempi < temp.Length; tempi++)
                    buf[(int)j + tempi] = temp[tempi];

                j += (UInt32)outlen;
            }

            buf[(int)j] = '\0';
            str = new string(buf.ToArray());  //.assign(&buf[0], j);
        }
    }


    class bdc_header : global_object
    {
        public const UInt32 MAJVERSION = 1;
        public const UInt32 MINVERSION = 0;

        const UInt32 OFFS_MAGIC      = 0x00; // 0x06 bytes
        const UInt32 OFFS_MAJVERSION = 0x06; // 0x01 bytes (binary integer)
        const UInt32 OFFS_MINVERSION = 0x07; // 0x01 bytes (binary integer)
        const UInt32 OFFS_ORIGLENGTH = 0x08; // 0x08 bytes (big-endian binary integer)
        const UInt32 OFFS_ORIGHASH   = 0x10; // 0x04 bytes
        const UInt32 OFFS_GLYPHCOUNT = 0x14; // 0x04 bytes (big-endian binary integer)
        const UInt32 OFFS_HEIGHT     = 0x18; // 0x02 bytes (big-endian binary integer)
        const UInt32 OFFS_YOFFSET    = 0x1a; // 0x02 bytes (big-endian binary integer)
        const UInt32 OFFS_DEFCHAR    = 0x1c; // 0x04 bytes (big-endian binary integer)
        const UInt32 OFFS_END        = 0x20;

        static RawBuffer MAGIC = new RawBuffer((int)(OFFS_MAJVERSION - OFFS_MAGIC));  //static u8 const                 MAGIC[OFFS_MAJVERSION - OFFS_MAGIC];
        //u8 const bdc_header::MAGIC[OFFS_MAJVERSION - OFFS_MAGIC] = { 'b', 'd', 'c', 'f', 'n', 't' };


        RawBuffer m_data = new RawBuffer((int)OFFS_END);  //u8                              m_data[OFFS_END];


        public bool read(emu_file f)
        {
            return f.read(new ListBytesPointer(m_data), (UInt32)m_data.Count) == m_data.Count;
        }
        //bool write(emu_file &f)
        //{
        //    return f.write(m_data, sizeof(m_data)) == sizeof(m_data);
        //}

        public bool check_magic()
        {
            // should be done at startup, but can't figure out how to initialize it
            if (MAGIC[0] == 0)
            {
                //u8 const bdc_header::MAGIC[OFFS_MAJVERSION - OFFS_MAGIC] = { 'b', 'd', 'c', 'f', 'n', 't' };
                MAGIC[0] = Convert.ToByte('b');
                MAGIC[1] = Convert.ToByte('d');
                MAGIC[2] = Convert.ToByte('c');
                MAGIC[3] = Convert.ToByte('f');
                MAGIC[4] = Convert.ToByte('n');
                MAGIC[5] = Convert.ToByte('t');
            }

            return memcmp(new ListBytesPointer(MAGIC), new ListBytesPointer(m_data, (int)OFFS_MAGIC), OFFS_MAJVERSION - OFFS_MAGIC) == 0;
        }
        public UInt32 get_major_version()
        {
            return m_data[OFFS_MAJVERSION];
        }
        public UInt32 get_minor_version()
        {
            return m_data[OFFS_MINVERSION];
        }
        public UInt64 get_original_length()
        {
            return
                    ((UInt64)(m_data[OFFS_ORIGLENGTH + 0]) << (7 * 8)) |
                    ((UInt64)(m_data[OFFS_ORIGLENGTH + 1]) << (6 * 8)) |
                    ((UInt64)(m_data[OFFS_ORIGLENGTH + 2]) << (5 * 8)) |
                    ((UInt64)(m_data[OFFS_ORIGLENGTH + 3]) << (4 * 8)) |
                    ((UInt64)(m_data[OFFS_ORIGLENGTH + 4]) << (3 * 8)) |
                    ((UInt64)(m_data[OFFS_ORIGLENGTH + 5]) << (2 * 8)) |
                    ((UInt64)(m_data[OFFS_ORIGLENGTH + 6]) << (1 * 8)) |
                    ((UInt64)(m_data[OFFS_ORIGLENGTH + 7]) << (0 * 8));
        }
        public UInt32 get_original_hash()
        {
            return
                    ((UInt32)(m_data[OFFS_ORIGHASH + 0]) << (3 * 8)) |
                    ((UInt32)(m_data[OFFS_ORIGHASH + 1]) << (2 * 8)) |
                    ((UInt32)(m_data[OFFS_ORIGHASH + 2]) << (1 * 8)) |
                    ((UInt32)(m_data[OFFS_ORIGHASH + 3]) << (0 * 8));
        }
        public UInt32 get_glyph_count()
        {
            return
                    ((UInt32)(m_data[OFFS_GLYPHCOUNT + 0]) << (3 * 8)) |
                    ((UInt32)(m_data[OFFS_GLYPHCOUNT + 1]) << (2 * 8)) |
                    ((UInt32)(m_data[OFFS_GLYPHCOUNT + 2]) << (1 * 8)) |
                    ((UInt32)(m_data[OFFS_GLYPHCOUNT + 3]) << (0 * 8));
        }
        public UInt16 get_height()
        {
            return
                    (UInt16)(((UInt16)(m_data[OFFS_HEIGHT + 0]) << (1 * 8)) |
                    ((UInt16)(m_data[OFFS_HEIGHT + 1]) << (0 * 8)));
        }
        public Int16 get_y_offset()
        {
            return
                    (Int16)(((UInt16)(m_data[OFFS_YOFFSET + 0]) << (1 * 8)) |
                    ((UInt16)(m_data[OFFS_YOFFSET + 1]) << (0 * 8)));
        }
        public int get_default_character()
        {
            return
                    (int)(((UInt32)(m_data[OFFS_DEFCHAR + 0]) << (3 * 8)) |
                    ((UInt32)(m_data[OFFS_DEFCHAR + 1]) << (2 * 8)) |
                    ((UInt32)(m_data[OFFS_DEFCHAR + 2]) << (1 * 8)) |
                    ((UInt32)(m_data[OFFS_DEFCHAR + 3]) << (0 * 8)));
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
        const UInt32 OFFS_ENCODING   = 0x00; // 0x04 bytes (big-endian binary integer)
        const UInt32 OFFS_XADVANCE   = 0x04; // 0x02 bytes (big-endian binary integer)
        // two bytes reserved
        const UInt32 OFFS_BBXOFFSET  = 0x08; // 0x02 bytes (big-endian binary integer)
        const UInt32 OFFS_BBYOFFSET  = 0x0a; // 0x02 bytes (big-endian binary integer)
        const UInt32 OFFS_BBWIDTH    = 0x0c; // 0x02 bytes (big-endian binary integer)
        const UInt32 OFFS_BBHEIGHT   = 0x0e; // 0x02 bytes (big-endian binary integer)
        const UInt32 OFFS_END        = 0x10;

        ListBytesPointer m_ptr;  //u8                              *m_ptr;


        public bdc_table_entry(ListBytesPointer bytes)  //void *bytes)
        {
            m_ptr = bytes;
        }
        //bdc_table_entry(bdc_table_entry const &that) = default;
        //bdc_table_entry(bdc_table_entry &&that) = default;

        public bdc_table_entry get_next()
        {
            return new bdc_table_entry(new ListBytesPointer(m_ptr, (int)OFFS_END));
        }

        public UInt32 get_encoding()
        {
            return
                    ((UInt32)(m_ptr[OFFS_ENCODING + 0]) << (3 * 8)) |
                    ((UInt32)(m_ptr[OFFS_ENCODING + 1]) << (2 * 8)) |
                    ((UInt32)(m_ptr[OFFS_ENCODING + 2]) << (1 * 8)) |
                    ((UInt32)(m_ptr[OFFS_ENCODING + 3]) << (0 * 8));
        }
        public UInt16 get_x_advance()
        {
            return
                    (UInt16)(((UInt16)(m_ptr[OFFS_XADVANCE + 0]) << (1 * 8)) |
                    ((UInt16)(m_ptr[OFFS_XADVANCE + 1]) << (0 * 8)));
        }
        public Int16 get_bb_x_offset()
        {
            return
                    (Int16)(((Int16)(m_ptr[OFFS_BBXOFFSET + 0]) << (1 * 8)) |
                    ((Int16)(m_ptr[OFFS_BBXOFFSET + 1]) << (0 * 8)));
        }
        public Int16 get_bb_y_offset()
        {
            return
                    (Int16)(((Int16)(m_ptr[OFFS_BBYOFFSET + 0]) << (1 * 8)) |
                    ((Int16)(m_ptr[OFFS_BBYOFFSET + 1]) << (0 * 8)));
        }
        public UInt16 get_bb_width()
        {
            return
                    (UInt16)(((UInt16)(m_ptr[OFFS_BBWIDTH + 0]) << (1 * 8)) |
                    ((UInt16)(m_ptr[OFFS_BBWIDTH + 1]) << (0 * 8)));
        }
        public UInt16 get_bb_height()
        {
            return
                    (UInt16)(((UInt16)(m_ptr[OFFS_BBHEIGHT + 0]) << (1 * 8)) |
                    ((UInt16)(m_ptr[OFFS_BBHEIGHT + 1]) << (0 * 8)));
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

        public static UInt32 size()
        {
            return OFFS_END;
        }
    }
}
