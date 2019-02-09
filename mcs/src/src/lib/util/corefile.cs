// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.IO;

using char16_t = System.UInt16;
using char32_t = System.UInt32;
using int64_t = System.Int64;
using ListBytes = mame.ListBase<System.Byte>;
using ListBytesPointer = mame.ListPointer<System.Byte>;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame.util
{
    public static class corefile_global
    {
        public const int EOF = -1;  // taken from stdio.h

        /***************************************************************************
            ADDITIONAL OPEN FLAGS
        ***************************************************************************/

        //#define OPEN_FLAG_NO_BOM        0x0100      /* don't output BOM */

        public const int FCOMPRESS_NONE   = 0;           /* no compression */
        public const int FCOMPRESS_MIN    = 1;           /* minimal compression */
        public const int FCOMPRESS_MEDIUM = 6;           /* standard compression */
        public const int FCOMPRESS_MAX    = 9;           /* maximum compression */


        public const int FILE_BUFFER_SIZE        = 512;

        //#define OPEN_FLAG_HAS_CRC       0x10000


        /* ----- filename utilities ----- */

        /* extract the base part of a filename (remove extensions and paths) */
        /*-------------------------------------------------
            core_filename_extract_base - extract the base
            name from a filename; note that this makes
            assumptions about path separators
        -------------------------------------------------*/
        public static string core_filename_extract_base(string name, bool strip_extension = false)
        {
            if (strip_extension)
                return Path.GetFileNameWithoutExtension(name);
            else
                return Path.GetFileName(name);
        }


        // extracts the file extension from a filename
        //std::string core_filename_extract_extension(const std::string &filename, bool strip_period = false);


        /* true if the given filename ends with a particular extension */
        public static bool core_filename_ends_with(string filename, string extension)
        {
            return filename.EndsWith(extension);
        }


        /*-------------------------------------------------
            is_directory_separator - is a given character
            a directory separator? The following logic
            works for most platforms
        -------------------------------------------------*/
        public static bool is_directory_separator(char c)
        {
//#if defined(WIN32)
            return ('\\' == c) || ('/' == c) || (':' == c);
//#else
//            return '/' == c;
//#endif
        }
    }


    public abstract class core_file : global_object
    {
        //typedef std::unique_ptr<core_file> ptr;


        // ----- file open/close -----

        // open a file with the specified filename
        public static osd_file.error open(string filename, uint32_t openflags, out core_file file)
        {
            file = null;

            //try
            {
                // attempt to open the file
                osd_file f;
                UInt64 length = 0;
                var filerr = osdcore_global.m_osdfile.open(filename, openflags, out f, out length);
                if (filerr != osd_file.error.NONE)
                    return filerr;

                file = new core_osd_file(openflags, f, length);
                return osd_file.error.NONE;
            }
#if false
            catch (Exception)
            {
                return osd_file.error.OUT_OF_MEMORY;
            }
#endif
        }


        // open a RAM-based "file" using the given data and length (read-only)
        /*-------------------------------------------------
            open_ram - open a RAM-based buffer for file-
            like access and return an error code
        -------------------------------------------------*/
        public static osd_file.error open_ram(ListBytes data, UInt32 length, uint32_t openflags, out core_file file)  // void const *data
        {
            file = null;

            // can only do this for read access
            if ((openflags & OPEN_FLAG_WRITE) != 0 || (openflags & OPEN_FLAG_CREATE) != 0)
                return osd_file.error.INVALID_ACCESS;

            //try
            {
                file = new core_in_memory_file(openflags, data, length, false);
                return osd_file.error.NONE;
            }
#if false
            catch (Exception)
            {
                return osd_file.error.OUT_OF_MEMORY;
            }
#endif
        }

        // open a RAM-based "file" using the given data and length (read-only), copying the data
        //static osd_file::error open_ram_copy(const void *data, std::size_t length, std::uint32_t openflags, ptr &file);

        // open a proxy "file" that forwards requests to another file object
        //static osd_file::error open_proxy(core_file &file, ptr &proxy);

        // close an open file
        //~core_file() { }

        // enable/disable streaming file compression via zlib; level is 0 to disable compression, or up to 9 for max compression
        public abstract osd_file.error compress(int level);


        // ----- file positioning -----

        // adjust the file pointer within the file
        public abstract int seek(int64_t offset, int whence);

        // return the current file pointer
        public abstract uint64_t tell();

        // return true if we are at the EOF
        //virtual bool eof() const = 0;

        // return the total size of the file
        public abstract uint64_t size();


        // ----- file read -----

        // standard binary read from a file
        public abstract uint32_t read(ListBytesPointer buffer, uint32_t length);  //void *buffer, std::uint32_t length) = 0;

        // read one character from the file
        public abstract int getc();

        // put back one character from the file
        public abstract int ungetc(int c);

        // read a full line of text from the file
        public abstract string gets(out string s, int n);

        // get a pointer to a buffer that holds the full file data in RAM
        // this function may cause the full file data to be read
        public abstract ListBytes buffer();  //virtual const void *buffer() = 0;

        // open a file with the specified filename, read it into memory, and return a pointer
        //static osd_file::error load(std::string const &filename, void **data, std::uint32_t &length);

        /*-------------------------------------------------
            load - open a file with the specified
            filename, read it into memory, and return a
            pointer
        -------------------------------------------------*/
        public static osd_file.error load(string filename, out ListBytes data)  //void **data, std::uint32_t &length)
        {
            data = new ListBytes();

            core_file file;

            // attempt to open the file
            var err = open(filename, OPEN_FLAG_READ, out file);
            if (err != osd_file.error.NONE)
                return err;

            // get the size
            var size = file.size();
            if ((UInt32)size != size)
                return osd_file.error.OUT_OF_MEMORY;

            // allocate memory
            //*data = malloc(size);
            //length = std::uint32_t(size);
            data.resize((int)size);

            // read the data
            if (file.read(new ListBytesPointer(data), (UInt32)size) != size)
            {
                data.Clear();
                return osd_file.error.FAILURE;
            }

            // close the file and return data
            return osd_file.error.NONE;
        }


        // ----- file write -----

        // standard binary write to a file
        public abstract uint32_t write(ListBytesPointer buffer, uint32_t length);  //const void *buffer, std::uint32_t length) = 0;

        // write a line of text to the file
        public abstract int puts(string s);

        // printf-style text write to a file
        public abstract int vprintf(string str);  //util::format_argument_pack<std::ostream> const &args) = 0;
        //template <typename Format, typename... Params> int printf(Format &&fmt, Params &&...args) { return vprintf(util::make_format_argument_pack(std::forward<Format>(fmt), std::forward<Params>(args)...)); }
        public int printf(string format, params object [] args) { return vprintf(string.Format(format, args)); }

        // file truncation
        //virtual osd_file::error truncate(std::uint64_t offset) = 0;

        // flush file buffers
        public abstract osd_file.error flush();
    }


    public class zlib_data
    {
        //typedef std::unique_ptr<zlib_data> ptr;


        //bool            m_compress, m_decompress;
        //z_stream        m_stream;
        //std::uint8_t    m_buffer[1024];
        //std::uint64_t   m_realoffset;
        //std::uint64_t   m_nextoffset;


        zlib_data(uint64_t offset) { throw new emu_unimplemented(); }


        public static int start_compression(int level, uint64_t offset, zlib_data data) { throw new emu_unimplemented(); }
        public static int start_decompression(uint64_t offset, zlib_data data) { throw new emu_unimplemented(); }

        ~zlib_data() { throw new emu_unimplemented(); }

        public UInt32 buffer_size() { throw new emu_unimplemented(); }
        public ListBytes buffer_data() { throw new emu_unimplemented(); }  //void *buffer_data() { return m_buffer; }


        // general-purpose output buffer manipulation
        public bool output_full() { throw new emu_unimplemented(); }
        public UInt32 output_space() { throw new emu_unimplemented(); }

        public void set_output(ListBytesPointer data, uint32_t size) { throw new emu_unimplemented(); }  //void *data, std::uint32_t size)


        // working with output to the internal buffer
        public bool has_output() { throw new emu_unimplemented(); }
        public UInt32 output_size() { throw new emu_unimplemented(); }

        public void reset_output() { throw new emu_unimplemented(); }


        // general-purpose input buffer manipulation
        public bool has_input() { throw new emu_unimplemented(); }
        public UInt32 input_size() { throw new emu_unimplemented(); }

        public void set_input(ListBytesPointer data, uint32_t size) { throw new emu_unimplemented(); }  //void const *data, std::uint32_t size)

        // working with input from the internal buffer
        public void reset_input(uint32_t size) { throw new emu_unimplemented(); }


        public int compress() { throw new emu_unimplemented(); }
        public int finalise() { throw new emu_unimplemented(); }
        public int decompress() { throw new emu_unimplemented(); }


        public uint64_t realoffset() { throw new emu_unimplemented(); }
        public void add_realoffset(uint32_t increment) { throw new emu_unimplemented(); }


        public bool is_nextoffset(uint64_t value) { throw new emu_unimplemented(); }
        public void add_nextoffset(uint32_t increment) { throw new emu_unimplemented(); }
    }


#if false
    class core_proxy_file : public core_file
    {
    public:
        core_proxy_file(core_file &file) : m_file(file) { }
        virtual ~core_proxy_file() override { }
        virtual osd_file::error compress(int level) override { return m_file.compress(level); }

        virtual int seek(std::int64_t offset, int whence) override { return m_file.seek(offset, whence); }
        virtual std::uint64_t tell() const override { return m_file.tell(); }
        virtual bool eof() const override { return m_file.eof(); }
        virtual std::uint64_t size() const override { return m_file.size(); }

        virtual std::uint32_t read(void *buffer, std::uint32_t length) override { return m_file.read(buffer, length); }
        virtual int getc() override { return m_file.getc(); }
        virtual int ungetc(int c) override { return m_file.ungetc(c); }
        virtual char *gets(char *s, int n) override { return m_file.gets(s, n); }
        virtual const void *buffer() override { return m_file.buffer(); }

        virtual std::uint32_t write(const void *buffer, std::uint32_t length) override { return m_file.write(buffer, length); }
        virtual int puts(const char *s) override { return m_file.puts(s); }
        virtual int vprintf(util::format_argument_pack<std::ostream> const &args) override { return m_file.vprintf(args); }
        virtual osd_file::error truncate(std::uint64_t offset) override { return m_file.truncate(offset); }

        virtual osd_file::error flush() override { return m_file.flush(); }

    private:
        core_file &m_file;
    };
#endif


    abstract class core_text_file : core_file
    {
        enum text_file_type
        {
            OSD,        // OSD dependent encoding format used when BOMs missing
            UTF8,       // UTF-8
            UTF16BE,    // UTF-16 (big endian)
            UTF16LE,    // UTF-16 (little endian)
            UTF32BE,    // UTF-32 (UCS-4) (big endian)
            UTF32LE     // UTF-32 (UCS-4) (little endian)
        }


        uint32_t m_openflags;                    // flags we were opened with
        text_file_type m_text_type;                    // text output format
        char [] m_back_chars = new char[unicode_global.UTF8_CHAR_MAX];  //char                m_back_chars[UTF8_CHAR_MAX];    // buffer to hold characters for ungetc
        int m_back_char_head;               // head of ungetc buffer
        int m_back_char_tail;               // tail of ungetc buffer
        string m_printf_buffer;  //ovectorstream       m_printf_buffer;                // persistent buffer for formatted output


        protected core_text_file(uint32_t openflags)
        {
            m_openflags = openflags;
            m_text_type = text_file_type.OSD;
            m_back_char_head = 0;
            m_back_char_tail = 0;
            m_printf_buffer = "";
        }


        /*-------------------------------------------------
            getc - read a character from a file
        -------------------------------------------------*/
        public override int getc()
        {
            int result;

            // refresh buffer, if necessary
            if (m_back_char_head == m_back_char_tail)
            {
                // do we need to check the byte order marks?
                if (tell() == 0)
                {
                    RawBuffer bom = new RawBuffer(4);  //std::uint8_t bom[4];
                    int pos = 0;

                    if (read(new ListBytesPointer(bom), 4) == 4)
                    {
                        if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                        {
                            m_text_type = text_file_type.UTF8;
                            pos = 3;
                        }
                        else if (bom[0] == 0x00 && bom[1] == 0x00 && bom[2] == 0xfe && bom[3] == 0xff)
                        {
                            m_text_type = text_file_type.UTF32BE;
                            pos = 4;
                        }
                        else if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0x00 && bom[3] == 0x00)
                        {
                            m_text_type = text_file_type.UTF32LE;
                            pos = 4;
                        }
                        else if (bom[0] == 0xfe && bom[1] == 0xff)
                        {
                            m_text_type = text_file_type.UTF16BE;
                            pos = 2;
                        }
                        else if (bom[0] == 0xff && bom[1] == 0xfe)
                        {
                            m_text_type = text_file_type.UTF16LE;
                            pos = 2;
                        }
                        else
                        {
                            m_text_type = text_file_type.OSD;
                            pos = 0;
                        }
                    }
                    seek(pos, emu_file.SEEK_SET);
                }

                // fetch the next character
                char16_t [] utf16_buffer = new char16_t[unicode_global.UTF16_CHAR_MAX];
                char32_t uchar = char32_t.MaxValue;  //(char32_t)~0;
                switch (m_text_type)
                {
                default:
                case text_file_type.OSD:
                    {
                        RawBuffer default_bufferBuf = new RawBuffer(16);  //char [] default_buffer = new char[16];
                        var readlen = read(new ListBytesPointer(default_bufferBuf), (UInt32)default_bufferBuf.Count);
                        if (readlen > 0)
                        {
                            //var charlen = osd_uchar_from_osdchar(&uchar, default_buffer, readlen / sizeof(default_buffer[0]));
                            uchar = default_bufferBuf[0];
                            var charlen = 1;
                            seek((Int64)(charlen * 1) - readlen, emu_file.SEEK_CUR);  // sizeof(default_buffer[0])
                        }
                    }
                    break;

                case text_file_type.UTF8:
                    {
                        throw new emu_unimplemented();
                    }
                    break;

                case text_file_type.UTF16BE:
                    {
                        throw new emu_unimplemented();
                    }
                    break;

                case text_file_type.UTF16LE:
                    {
                        throw new emu_unimplemented();
                    }
                    break;

                case text_file_type.UTF32BE:
                    throw new emu_unimplemented();
                    break;

                case text_file_type.UTF32LE:
                    throw new emu_unimplemented();
                    break;
                }

                if (uchar != char32_t.MaxValue)  // ~0)
                {
                    // place the new character in the ring buffer
                    m_back_char_head = 0;
                    string temp;
                    m_back_char_tail = unicode_global.utf8_from_uchar(out temp, uchar);
                    m_back_chars = temp.ToCharArray();
                    //assert(file->back_char_tail != -1);
                }
            }

            // now read from the ring buffer
            if (m_back_char_head == m_back_char_tail)
            {
                result = corefile_global.EOF;
            }
            else
            {
                result = m_back_chars[m_back_char_head++];
                m_back_char_head %= m_back_chars.Length;
            }

            return result;
        }


        /*-------------------------------------------------
            ungetc - put back a character read from a
            file
        -------------------------------------------------*/
        public override int ungetc(int c)
        {
            m_back_chars[m_back_char_tail++] = (char)c;
            m_back_char_tail %= m_back_chars.Length;
            return c;
        }


        /*-------------------------------------------------
            gets - read a line from a text file
        -------------------------------------------------*/
        public override string gets(out string s, int n)
        {
            s = "";

            //char *cur = s;
            string curStr = s;

            // loop while we have characters
            while (n > 0)
            {
                int c = getc();
                if (c == corefile_global.EOF)
                {
                    break;
                }
                else if (c == 0x0d) // if there's a CR, look for an LF afterwards
                {
                    int c2 = getc();
                    if (c2 != 0x0a)
                        ungetc(c2);
                    curStr += (char)0x0d;  //*cur++ = 0x0d;
                    n--;
                    break;
                }
                else if (c == 0x0a) // if there's an LF, reinterp as a CR for consistency
                {
                    curStr += (char)0x0d;  //*cur++ = 0x0d;
                    n--;
                    break;
                }
                else // otherwise, pop the character in and continue
                {
                    curStr += c;  //*cur++ = c;
                    n--;
                }
            }

            // if we put nothing in, return nullptr
            if (curStr == s)
                return null;

            /* otherwise, terminate */
            //if (n > 0)
            //    *cur++ = 0;
            return s;
        }


        /*-------------------------------------------------
            puts - write a line to a text file
        -------------------------------------------------*/
        public override int puts(string s)
        {
            throw new emu_unimplemented();
        }


        /*-------------------------------------------------
            vprintf - vfprintf to a text file
        -------------------------------------------------*/
        public override int vprintf(string str)  //util::format_argument_pack<std::ostream> const &args) override;
        {
            throw new emu_unimplemented();
        }


        protected bool read_access() { return 0U != (m_openflags & OPEN_FLAG_READ); }
        protected bool write_access() { return 0U != (m_openflags & OPEN_FLAG_WRITE); }
        //bool no_bom() { return 0U != (m_openflags & osdcore_global.OPEN_FLAG_NO_BOM); }


        //bool has_putback() const { return m_back_char_head != m_back_char_tail; }
        protected void clear_putback() { m_back_char_head = m_back_char_tail = 0; }
    }


    class core_in_memory_file : core_text_file
    {
        bool m_data_allocated;   // was the data allocated by us?
        ListBytes m_data;  //void const *    m_data;             // file data, if RAM-based
        uint64_t m_offset;           // current file offset
        uint64_t m_length;           // total file length


        public core_in_memory_file(uint32_t openflags, ListBytes data, UInt32 length, bool copy)  // void *data
            : base(openflags)
        {
            m_data_allocated = false;
            m_data = copy ? null : data;
            m_offset = 0;
            m_length = length;


            if (copy)
            {
                ListBytes buf = allocate();  // void *const buf = allocate();
                if (buf != null)
                    memcpy(new ListBytesPointer(buf), new ListBytesPointer(data), length);  // std::memcpy(buf, data, length);
            }
        }

        //~core_in_memory_file() { purge(); }


        public override osd_file.error compress(int level) { return osd_file.error.INVALID_ACCESS; }


        /*-------------------------------------------------
            seek - seek within a file
        -------------------------------------------------*/
        public override int seek(int64_t offset, int whence)
        {
            // flush any buffered char
            clear_putback();

            // switch off the relative location
            switch (whence)
            {
            case emu_file.SEEK_SET:
                m_offset = (UInt64)offset;
                break;

            case emu_file.SEEK_CUR:
                m_offset += (UInt64)offset;
                break;

            case emu_file.SEEK_END:
                m_offset = m_length + (UInt64)offset;
                break;
            }
            return 0;
        }


        public override uint64_t tell() { return m_offset; }


        //virtual bool eof() const override;

        public override uint64_t size() { return m_length; }


        /*-------------------------------------------------
            read - read from a file
        -------------------------------------------------*/
        public override uint32_t read(ListBytesPointer buffer, uint32_t length)  //void *buffer, std::uint32_t length)
        {
            clear_putback();

            // handle RAM-based files
            var bytes_read = safe_buffer_copy(new ListBytesPointer(m_data), (UInt32)m_offset, (UInt32)m_length, buffer, 0, length);
            m_offset += bytes_read;
            return bytes_read;
        }

        public override ListBytes buffer() { return m_data; }


        public override uint32_t write(ListBytesPointer buffer, uint32_t length) { return 0; }  //void const *buffer, std::uint32_t length)


        //virtual osd_file::error truncate(std::uint64_t offset) override;


        public override osd_file.error flush() { clear_putback(); return osd_file.error.NONE; }


        protected core_in_memory_file(uint32_t openflags, uint64_t length)
            : base(openflags)
        {
            m_data_allocated = false;
            m_data = null;
            m_offset = 0;
            m_length = length;
        }


        protected bool is_loaded() { return null != m_data; }


        protected ListBytes allocate()  // void *allocate()
        {
            if (m_data != null)
                return null;

            ListBytes data = new ListBytes((int)m_length);  // void *data = malloc(m_length);
            data.resize((int)m_length);
            if (data != null)
            {
                m_data_allocated = true;
                m_data = data;
            }

            return data;
        }


        protected void purge()
        {
            if (m_data != null && m_data_allocated)
                m_data = null;  //free(const_cast<void *>(m_data));
            m_data_allocated = false;
            m_data = null;
        }


        protected uint64_t offset() { return m_offset; }
        protected void add_offset(uint32_t increment) { m_offset += increment; m_length = Math.Max(m_length, m_offset); }
        protected uint64_t length() { return m_length; }
        //void set_length(std::uint64_t value) { m_length = value; m_offset = (std::min)(m_offset, m_length); }


        /*-------------------------------------------------
            safe_buffer_copy - copy safely from one
            bounded buffer to another
        -------------------------------------------------*/
        protected static UInt32 safe_buffer_copy(
                ListBytesPointer source, UInt32 sourceoffs, UInt32 sourcelen,  // void const *source, UInt32 sourceoffs, UInt32 sourcelen,
                ListBytesPointer dest, UInt32 destoffs, UInt32 destlen)  // void *dest, UInt32 destoffs, UInt32 destlen)
        {
            var sourceavail = sourcelen - sourceoffs;
            var destavail = destlen - destoffs;
            var bytes_to_copy = Math.Min(sourceavail, destavail);
            if (bytes_to_copy > 0)
            {
                //std::memcpy(
                //        reinterpret_cast<std::uint8_t *>(dest) + destoffs,
                //        reinterpret_cast<std::uint8_t const *>(source) + sourceoffs,
                //        bytes_to_copy);
                memcpy(new ListBytesPointer(dest, (int)destoffs), new ListBytesPointer(source, (int)sourceoffs), bytes_to_copy);
            }

            return bytes_to_copy;
        }
    }


    class core_osd_file : core_in_memory_file, IDisposable
    {
        const int FILE_BUFFER_SIZE = 512;


        osd_file m_file;                     // OSD file handle
        zlib_data m_zdata;                    // compression data
        uint64_t m_bufferbase;               // base offset of internal buffer
        uint32_t m_bufferbytes;              // bytes currently loaded into buffer
        RawBuffer m_buffer = new RawBuffer(FILE_BUFFER_SIZE);  //std::uint8_t    m_buffer[FILE_BUFFER_SIZE]; // buffer data


        public core_osd_file(uint32_t openmode, osd_file file, uint64_t length)
            : base(openmode, length)
        {
            m_file = file;
            m_zdata = null; //()
            m_bufferbase = 0;
            m_bufferbytes = 0;
        }

        ~core_osd_file()
        {
            assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            // close files and free memory
            if (m_zdata != null)
                compress(corefile_global.FCOMPRESS_NONE);

            m_isDisposed = true;
        }


        /*-------------------------------------------------
            compress - enable/disable streaming file
            compression via zlib; level is 0 to disable
            compression, or up to 9 for max compression
        -------------------------------------------------*/
        public override osd_file.error compress(int level)
        {
            osd_file.error result = osd_file.error.NONE;

            // can only do this for read-only and write-only cases
            if (read_access() && write_access())
                return osd_file.error.INVALID_ACCESS;

            // if we have been compressing, flush and free the data
            if (m_zdata != null && (level == corefile_global.FCOMPRESS_NONE))
            {
                int zerr = zlib_global.Z_OK;

                // flush any remaining data if we are writing
                while (write_access() && (zerr != zlib_global.Z_STREAM_END))
                {
                    // deflate some more
                    zerr = m_zdata.finalise();
                    if ((zerr != zlib_global.Z_STREAM_END) && (zerr != zlib_global.Z_OK))
                    {
                        result = osd_file.error.INVALID_DATA;
                        break;
                    }

                    // write the data
                    if (m_zdata.has_output())
                    {
                        UInt32 actualdata;
                        var filerr = m_file.write(new ListBytesPointer(m_zdata.buffer_data()), m_zdata.realoffset(), m_zdata.output_size(), out actualdata);
                        if (filerr != osd_file.error.NONE)
                            break;
                        m_zdata.add_realoffset(actualdata);
                        m_zdata.reset_output();
                    }
                }

                // free memory
                m_zdata = null;
            }

            // if we are just starting to compress, allocate a new buffer
            if (m_zdata == null && (level > corefile_global.FCOMPRESS_NONE))
            {
                int zerr;

                // initialize the stream and compressor
                if (write_access())
                    zerr = zlib_data.start_compression(level, offset(), m_zdata);
                else
                    zerr = zlib_data.start_decompression(offset(), m_zdata);

                // on error, return an error
                if (zerr != zlib_global.Z_OK)
                    return osd_file.error.OUT_OF_MEMORY;

                // flush buffers
                m_bufferbytes = 0;
            }

            return result;
        }


        /*-------------------------------------------------
            seek - seek within a file
        -------------------------------------------------*/
        public override int seek(int64_t offset, int whence)
        {
            // error if compressing
            if (m_zdata != null)
                return 1;
            else
                return base.seek(offset, whence);
        }


        /*-------------------------------------------------
            read - read from a file
        -------------------------------------------------*/
        public override uint32_t read(ListBytesPointer buffer, uint32_t length)  //void *buffer, std::uint32_t length)
        {
            if (m_file == null || is_loaded())
                return base.read(buffer, length);

            // flush any buffered char
            clear_putback();

            UInt32 bytes_read = 0;

            // if we're within the buffer, consume that first
            if (is_buffered())
                bytes_read += safe_buffer_copy(new ListBytesPointer(m_buffer), (UInt32)(offset() - m_bufferbase), m_bufferbytes, buffer, bytes_read, length);

            // if we've got a small amount left, read it into the buffer first
            if (bytes_read < length)
            {
                if ((length - bytes_read) < (m_buffer.Count / 2))
                {
                    // read as much as makes sense into the buffer
                    m_bufferbase = offset() + bytes_read;
                    m_bufferbytes = 0;
                    osd_or_zlib_read(new ListBytesPointer(m_buffer), m_bufferbase, (UInt32)m_buffer.Count, out m_bufferbytes);

                    // do a bounded copy from the buffer to the destination
                    bytes_read += safe_buffer_copy(new ListBytesPointer(m_buffer), 0, m_bufferbytes, buffer, bytes_read, length);
                }
                else
                {
                    // read the remainder directly from the file
                    UInt32 new_bytes_read = 0;
                    osd_or_zlib_read(new ListBytesPointer(buffer, (int)bytes_read), offset() + bytes_read, length - bytes_read, out new_bytes_read);
                    bytes_read += new_bytes_read;
                }
            }

            // return the number of bytes read
            add_offset(bytes_read);
            return bytes_read;
        }


        /*-------------------------------------------------
            buffer - return a pointer to the file buffer;
            if it doesn't yet exist, load the file into
            RAM first
        -------------------------------------------------*/
        public override ListBytes buffer()  //void const *core_osd_file::buffer()
        {
            // if we already have data, just return it
            if (!is_loaded() && length() != 0)
            {
                // allocate some memory
                ListBytes buf = allocate();  // void *buf = allocate();
                if (buf == null) return null;

                // read the file
                UInt32 read_length = 0;
                var filerr = osd_or_zlib_read(new ListBytesPointer(buf), 0, (UInt32)length(), out read_length);
                if ((filerr != osd_file.error.NONE) || (read_length != length()))
                {
                    purge();
                }
                else
                {
                    // close the file because we don't need it anymore
                    m_file = null;
                }
            }

            return base.buffer();
        }


        /*-------------------------------------------------
            write - write to a file
        -------------------------------------------------*/
        public override uint32_t write(ListBytesPointer buffer, uint32_t length)  //void *buffer, std::uint32_t length)
        {
            // can't write to RAM-based stuff
            if (is_loaded())
                return base.write(buffer, length);

            // flush any buffered char
            clear_putback();

            // invalidate any buffered data
            m_bufferbytes = 0;

            // do the write
            UInt32 bytes_written = 0;
            osd_or_zlib_write(buffer, offset(), length, out bytes_written);

            // return the number of bytes written
            add_offset(bytes_written);
            return bytes_written;
        }


        //virtual osd_file::error truncate(std::uint64_t offset) override;
        //virtual osd_file::error flush() override;


        bool is_buffered() { return (offset() >= m_bufferbase) && (offset() < (m_bufferbase + m_bufferbytes)); }


        /*-------------------------------------------------
            osd_or_zlib_read - wrapper for osd_read that
            handles zlib-compressed data
        -------------------------------------------------*/
        osd_file.error osd_or_zlib_read(ListBytesPointer buffer, uint64_t offset, uint32_t length, out uint32_t actual)  //void *buffer
        {
            actual = 0;

            // if no compression, just pass through
            if (m_zdata == null)
                return m_file.read(buffer, offset, length, out actual);

            // if the offset doesn't match the next offset, fail
            if (!m_zdata.is_nextoffset(offset))
                return osd_file.error.INVALID_ACCESS;

            // set up the destination
            osd_file.error filerr = osd_file.error.NONE;
            m_zdata.set_output(buffer, length);
            while (!m_zdata.output_full())
            {
                // if we didn't make progress, report an error or the end
                if (m_zdata.has_input())
                {
                    var zerr = m_zdata.decompress();
                    if (zlib_global.Z_OK != zerr)
                    {
                        if (zlib_global.Z_STREAM_END != zerr) filerr = osd_file.error.INVALID_DATA;
                        break;
                    }
                }

                // fetch more data if needed
                if (!m_zdata.has_input())
                {
                    UInt32 actualdata = 0;
                    filerr = m_file.read(new ListBytesPointer(m_zdata.buffer_data()), m_zdata.realoffset(), m_zdata.buffer_size(), out actualdata);
                    if (filerr != osd_file.error.NONE) break;
                    m_zdata.add_realoffset(actualdata);
                    m_zdata.reset_input(actualdata);
                    if (!m_zdata.has_input()) break;
                }
            }

            // adjust everything
            actual = length - m_zdata.output_space();
            m_zdata.add_nextoffset(actual);
            return filerr;
        }


        /*-------------------------------------------------
            osd_or_zlib_write - wrapper for osd_write that
            handles zlib-compressed data
        -------------------------------------------------*/
        /**
         * @fn  osd_file::error osd_or_zlib_write(void const *buffer, std::uint64_t offset, std::uint32_t length, std::uint32_t actual)
         *
         * @brief   OSD or zlib write.
         *
         * @param   buffer          The buffer.
         * @param   offset          The offset.
         * @param   length          The length.
         * @param [in,out]  actual  The actual.
         *
         * @return  A osd_file::error.
         */
        osd_file.error osd_or_zlib_write(ListBytesPointer buffer, uint64_t offset, uint32_t length, out uint32_t actual)  //void const *buffer
        {
            actual = 0;

            // if no compression, just pass through
            if (m_zdata == null)
                return m_file.write(buffer, offset, length, out actual);

            // if the offset doesn't match the next offset, fail
            if (!m_zdata.is_nextoffset(offset))
                return osd_file.error.INVALID_ACCESS;

            // set up the source
            m_zdata.set_input(buffer, length);
            while (m_zdata.has_input())
            {
                // if we didn't make progress, report an error or the end
                var zerr = m_zdata.compress();
                if (zerr != zlib_global.Z_OK)
                {
                    actual = length - m_zdata.input_size();
                    m_zdata.add_nextoffset(actual);
                    return osd_file.error.INVALID_DATA;
                }

                // write more data if we are full up
                if (m_zdata.output_full())
                {
                    UInt32 actualdata = 0;
                    var filerr = m_file.write(new ListBytesPointer(m_zdata.buffer_data()), m_zdata.realoffset(), m_zdata.output_size(), out actualdata);
                    if (filerr != osd_file.error.NONE)
                        return filerr;
                    m_zdata.add_realoffset(actualdata);
                    m_zdata.reset_output();
                }
            }

            // we wrote everything
            actual = length;
            m_zdata.add_nextoffset(actual);
            return osd_file.error.NONE;
        }
    }
}
