// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.IO;

using char16_t = System.UInt16;
using char32_t = System.UInt32;
using int64_t = System.Int64;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using size_t = System.UInt64;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame
{
    public static partial class util
    {
        public const int EOF = -1;  // taken from stdio.h

        /***************************************************************************
            ADDITIONAL OPEN FLAGS
        ***************************************************************************/

        //#define OPEN_FLAG_NO_BOM        0x0100      /* don't output BOM */

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


        public abstract class core_file
        {
            //typedef std::unique_ptr<core_file> ptr;


            // ----- file open/close -----

            // open a file with the specified filename
            public static std.error_condition open(string filename, uint32_t openflags, out core_file file)
            {
                file = null;

                //try
                {
                    // attempt to open the file
                    osd_file f;
                    uint64_t length = 0;
                    var filerr = osdfile_global.m_osdfile.open(filename, openflags, out f, out length); // FIXME: allow osd_file to accept std::string_view
                    if (filerr)
                        return filerr;

                    file = new core_osd_file(openflags, f, length);
                    return new std.error_condition();
                }
#if false
                catch (Exception)
                {
                    return std::errc::not_enough_memory;
                }
#endif
            }


            // open a RAM-based "file" using the given data and length (read-only)
            /*-------------------------------------------------
                open_ram - open a RAM-based buffer for file-
                like access and return an error code
            -------------------------------------------------*/
            public static std.error_condition open_ram(MemoryU8 data, size_t length, uint32_t openflags, out core_file file)  //std::error_condition open_ram(void const *data, std::size_t length, std::uint32_t openflags, ptr &file)
            {
                file = null;

                // can only do this for read access
                if ((openflags & g.OPEN_FLAG_WRITE) != 0 || (openflags & g.OPEN_FLAG_CREATE) != 0)
                    return std.errc.invalid_argument;

                //try
                {
                    file = new core_in_memory_file(openflags, data, length, false);
                    return new std.error_condition();
                }
#if false
                catch (Exception)
                {
                    return std::errc::not_enough_memory;
                }
#endif
            }

            // open a RAM-based "file" using the given data and length (read-only), copying the data
            //static std::error_condition open_ram_copy(const void *data, std::size_t length, std::uint32_t openflags, ptr &file);

            // open a proxy "file" that forwards requests to another file object
            //static std::error_condition open_proxy(core_file &file, ptr &proxy);

            // close an open file
            //~core_file() { }


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
            public abstract uint32_t read(Pointer<uint8_t> buffer, uint32_t length);  //virtual std::uint32_t read(void *buffer, std::uint32_t length) = 0;

            // read one character from the file
            public abstract int getc();

            // put back one character from the file
            public abstract int ungetc(int c);

            // read a full line of text from the file
            public abstract string gets(out string s, int n);

            // get a pointer to a buffer that holds the full file data in RAM
            // this function may cause the full file data to be read
            public abstract MemoryU8 buffer();  //virtual const void *buffer() = 0;

            // open a file with the specified filename, read it into memory, and return a pointer
            /*-------------------------------------------------
                load - open a file with the specified
                filename, read it into memory, and return a
                pointer
            -------------------------------------------------*/
            public static std.error_condition load(string filename, out MemoryU8 data)  //std::error_condition load(std::string const &filename, void **data, std::uint32_t &length)
            {
                data = new MemoryU8();

                core_file file;

                // attempt to open the file
                var err = open(filename, g.OPEN_FLAG_READ, out file);
                if (err)
                    return err;

                // get the size
                var size = file.size();
                if ((uint32_t)size != size)
                    return std.errc.file_too_large;

                // allocate memory
                //*data = malloc(size);
                data.Resize((int)size);
                //if (!*data)
                //    return std::errc::not_enough_memory;
                //length = std::uint32_t(size);

                // read the data
                if (file.read(new Pointer<uint8_t>(data), (UInt32)size) != size)
                {
                    data.Clear();  //free(*data);
                    return std.errc.io_error; // TODO: revisit this error code
                }

                // close the file and return data
                return new std.error_condition();
            }

            //static std::error_condition load(std::string_view filename, std::vector<uint8_t> &data);


            // ----- file write -----

            // standard binary write to a file
            public abstract uint32_t write(Pointer<uint8_t> buffer, uint32_t length);  //virtual std::uint32_t write(const void *buffer, std::uint32_t length) = 0;

            // write a line of text to the file
            public abstract int puts(string s);

            // printf-style text write to a file
            public abstract int vprintf(string str);  //util::format_argument_pack<std::ostream> const &args) = 0;
            //template <typename Format, typename... Params> int printf(Format &&fmt, Params &&...args) { return vprintf(util::make_format_argument_pack(std::forward<Format>(fmt), std::forward<Params>(args)...)); }
            public int printf(string format, params object [] args) { return vprintf(string.Format(format, args)); }

            // file truncation
            //virtual std::error_condition truncate(std::uint64_t offset) = 0;

            // flush file buffers
            public abstract std.error_condition flush();
        }


        //class core_proxy_file : public core_file


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
                        MemoryU8 bom = new MemoryU8(4, true);  //std::uint8_t bom[4];
                        int pos = 0;

                        if (read(new Pointer<uint8_t>(bom), 4) == 4)
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
                        seek(pos, g.SEEK_SET);
                    }

                    // fetch the next character
                    char16_t [] utf16_buffer = new char16_t[unicode_global.UTF16_CHAR_MAX];
                    var uchar = char32_t.MaxValue;  //(char32_t)~0;
                    switch (m_text_type)
                    {
                    default:
                    case text_file_type.OSD:
                        {
                            MemoryU8 default_bufferBuf = new MemoryU8(16, true);  //char [] default_buffer = new char[16];
                            var readlen = read(new Pointer<uint8_t>(default_bufferBuf), (UInt32)default_bufferBuf.Count);
                            if (readlen > 0)
                            {
                                //var charlen = osd_uchar_from_osdchar(&uchar, default_buffer, readlen / sizeof(default_buffer[0]));
                                uchar = default_bufferBuf[0];
                                var charlen = 1;
                                seek((Int64)(charlen * 1) - readlen, g.SEEK_CUR);  // sizeof(default_buffer[0])
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
                    result = EOF;
                }
                else
                {
                    result = m_back_chars[m_back_char_head++];
                    m_back_char_head %= (int)std.size(m_back_chars);
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
                m_back_char_tail %= (int)std.size(m_back_chars);
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
                    if (c == EOF)
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


            protected bool read_access() { return 0U != (m_openflags & g.OPEN_FLAG_READ); }
            protected bool write_access() { return 0U != (m_openflags & g.OPEN_FLAG_WRITE); }
            //bool no_bom() { return 0U != (m_openflags & osdcore_global.OPEN_FLAG_NO_BOM); }


            //bool has_putback() const { return m_back_char_head != m_back_char_tail; }
            protected void clear_putback() { m_back_char_head = m_back_char_tail = 0; }
        }


        class core_in_memory_file : core_text_file
        {
            bool m_data_allocated;   // was the data allocated by us?
            MemoryU8 m_data;  //void const *    m_data;             // file data, if RAM-based
            uint64_t m_offset;           // current file offset
            uint64_t m_length;           // total file length


            public core_in_memory_file(uint32_t openflags, MemoryU8 data, size_t length, bool copy)  //core_in_memory_file(std::uint32_t openflags, void const *data, std::size_t length, bool copy)
                : base(openflags)
            {
                m_data_allocated = false;
                m_data = copy ? null : data;
                m_offset = 0;
                m_length = length;


                if (copy)
                {
                    MemoryU8 buf = allocate();  // void *const buf = allocate();
                    if (buf != null)
                        std.memcpy(new Pointer<uint8_t>(buf), new Pointer<uint8_t>(data), length);  //if (buf) std::memcpy(buf, data, length);
                }
            }

            //~core_in_memory_file() { purge(); }


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
                case g.SEEK_SET:
                    m_offset = (UInt64)offset;
                    break;

                case g.SEEK_CUR:
                    m_offset += (UInt64)offset;
                    break;

                case g.SEEK_END:
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
            public override uint32_t read(Pointer<uint8_t> buffer, uint32_t length)  //std::uint32_t read(void *buffer, std::uint32_t length)
            {
                clear_putback();

                // handle RAM-based files
                var bytes_read = safe_buffer_copy(new Pointer<uint8_t>(m_data), (UInt32)m_offset, (UInt32)m_length, buffer, 0, length);
                m_offset += bytes_read;
                return bytes_read;
            }

            public override MemoryU8 buffer() { return m_data; }  //virtual void const *buffer() override { return m_data; }


            public override uint32_t write(Pointer<uint8_t> buffer, uint32_t length) { return 0; }  //virtual std::uint32_t write(void const *buffer, std::uint32_t length) override { return 0; }


            //virtual std::error_condition truncate(std::uint64_t offset) override;


            public override std.error_condition flush() { clear_putback(); return new std.error_condition(); }


            protected core_in_memory_file(uint32_t openflags, uint64_t length)
                : base(openflags)
            {
                m_data_allocated = false;
                m_data = null;
                m_offset = 0;
                m_length = length;
            }


            protected bool is_loaded() { return null != m_data; }


            protected MemoryU8 allocate()  //void *allocate()
            {
                if (m_data != null)
                    return null;

                MemoryU8 data = new MemoryU8((int)m_length, true);  //void *data = malloc(m_length);
                data.Resize((int)m_length);
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
            protected static UInt32 safe_buffer_copy(  //std::size_t safe_buffer_copy(
                    Pointer<uint8_t> source, UInt32 sourceoffs, UInt32 sourcelen,  //void const *source, std::size_t sourceoffs, std::size_t sourcelen,
                    Pointer<uint8_t> dest, UInt32 destoffs, UInt32 destlen)  //void *dest, std::size_t destoffs, std::size_t destlen)
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
                    std.memcpy(
                        new Pointer<uint8_t>(dest, (int)destoffs),
                        new Pointer<uint8_t>(source, (int)sourceoffs),
                        bytes_to_copy);
                }

                return bytes_to_copy;
            }
        }


        class core_osd_file : core_in_memory_file
        {
            const int FILE_BUFFER_SIZE = 512;


            osd_file m_file;                     //osd_file::ptr   m_file;                     // OSD file handle
            uint64_t m_bufferbase;               // base offset of internal buffer
            uint32_t m_bufferbytes;              // bytes currently loaded into buffer
            MemoryU8 m_buffer = new MemoryU8(FILE_BUFFER_SIZE, true);  //std::uint8_t    m_buffer[FILE_BUFFER_SIZE]; // buffer data


            public core_osd_file(uint32_t openmode, osd_file file, uint64_t length)
                : base(openmode, length)
            {
                m_file = file;
                m_bufferbase = 0;
                m_bufferbytes = 0;
            }


            /*-------------------------------------------------
                read - read from a file
            -------------------------------------------------*/
            public override uint32_t read(Pointer<uint8_t> buffer, uint32_t length)  //std::uint32_t read(void *buffer, std::uint32_t length)
            {
                if (m_file == null || is_loaded())
                    return base.read(buffer, length);

                // flush any buffered char
                clear_putback();

                uint32_t bytes_read = 0;

                // if we're within the buffer, consume that first
                if (is_buffered())
                    bytes_read += safe_buffer_copy(new Pointer<uint8_t>(m_buffer), (uint32_t)(offset() - m_bufferbase), m_bufferbytes, buffer, bytes_read, length);

                // if we've got a small amount left, read it into the buffer first
                if (bytes_read < length)
                {
                    if ((length - bytes_read) < (m_buffer.Count / 2))
                    {
                        // read as much as makes sense into the buffer
                        m_bufferbase = offset() + bytes_read;
                        m_bufferbytes = 0;
                        m_file.read(new Pointer<uint8_t>(m_buffer), m_bufferbase, (uint32_t)m_buffer.Count, out m_bufferbytes);  //m_file->read(m_buffer, m_bufferbase, sizeof(m_buffer), m_bufferbytes);

                        // do a bounded copy from the buffer to the destination
                        bytes_read += safe_buffer_copy(new Pointer<uint8_t>(m_buffer), 0, m_bufferbytes, buffer, bytes_read, length);
                    }
                    else
                    {
                        // read the remainder directly from the file
                        uint32_t new_bytes_read = 0;
                        m_file.read(new Pointer<uint8_t>(buffer, (int)bytes_read), offset() + bytes_read, length - bytes_read, out new_bytes_read);  //m_file->read(reinterpret_cast<std::uint8_t *>(buffer) + bytes_read, offset() + bytes_read, length - bytes_read, new_bytes_read);
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
            public override MemoryU8 buffer()  //void const *buffer()
            {
                // if we already have data, just return it
                if (!is_loaded() && length() != 0)
                {
                    // allocate some memory
                    MemoryU8 buf = allocate();  // void *buf = allocate();
                    if (buf == null)
                        return null;

                    // read the file
                    uint32_t read_length = 0;
                    var filerr = m_file.read(new Pointer<uint8_t>(buf), 0, (uint32_t)length(), out read_length);
                    if (filerr || (read_length != length()))
                        purge();
                    else
                        m_file = null;  //m_file.reset(); // close the file because we don't need it anymore
                }

                return base.buffer();
            }


            /*-------------------------------------------------
                write - write to a file
            -------------------------------------------------*/
            public override uint32_t write(Pointer<uint8_t> buffer, uint32_t length)  //std::uint32_t write(void const *buffer, std::uint32_t length)
            {
                // can't write to RAM-based stuff
                if (is_loaded())
                    return base.write(buffer, length);

                // flush any buffered char
                clear_putback();

                // invalidate any buffered data
                m_bufferbytes = 0;

                // do the write
                uint32_t bytes_written = 0;
                m_file.write(buffer, offset(), length, out bytes_written);

                // return the number of bytes written
                add_offset(bytes_written);
                return bytes_written;
            }


            //virtual std::error_condition truncate(std::uint64_t offset) override;
            //virtual std::error_condition flush() override;


            bool is_buffered() { return (offset() >= m_bufferbase) && (offset() < (m_bufferbase + m_bufferbytes)); }
        }
    }
}
