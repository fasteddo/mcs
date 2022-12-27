// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.IO;
using System.Text;

using char16_t = System.UInt16;
using char32_t = System.UInt32;
using int64_t = System.Int64;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using PointerU8 = mame.Pointer<System.Byte>;
using size_t = System.UInt64;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using static mame.corefile_global;
using static mame.corefile_internal;
using static mame.cpp_global;
using static mame.osdcomm_global;
using static mame.osdfile_global;
using static mame.unicode_global;


namespace mame
{
    public static class corefile_internal
    {
        public const int EOF = -1;  // taken from stdio.h

        public const int FILE_BUFFER_SIZE        = 512;

        //#define OPEN_FLAG_HAS_CRC       0x10000
    }


    public static partial class util
    {
        /***************************************************************************
            ADDITIONAL OPEN FLAGS
        ***************************************************************************/

        public const uint32_t OPEN_FLAG_NO_BOM        = 0x0100;      /* don't output BOM */


        public abstract class core_file : random_read_write, IDisposable
        {
            //typedef std::unique_ptr<core_file> ptr;


            // ----- file open/close -----

            // open a file with the specified filename
            //-------------------------------------------------
            //  open - open a file for access and
            //  return an error code
            //-------------------------------------------------
            public static std.error_condition open(string filename, uint32_t openflags, out core_file file)
            {
                file = null;

                // attempt to open the file
                osd_file f;
                uint64_t length = 0;
                var filerr = m_osdfile.open(filename, openflags, out f, out length); // FIXME: allow osd_file to accept std::string_view
                if (filerr)
                    return filerr;

                try { file = new core_osd_file(openflags, f, length); }
                catch (Exception) { return std.errc.not_enough_memory; }

                return new std.error_condition();
            }


            // open a RAM-based "file" using the given data and length (read-only)
            //-------------------------------------------------
            //  open_ram - open a RAM-based buffer for file-
            //  like access and return an error code
            //-------------------------------------------------
            public static std.error_condition open_ram(PointerU8 data, size_t length, uint32_t openflags, out core_file file)  //std::error_condition open_ram(void const *data, std::size_t length, std::uint32_t openflags, ptr &file)
            {
                file = null;

                // can only do this for read access
                if ((openflags & OPEN_FLAG_WRITE) != 0 || (openflags & OPEN_FLAG_CREATE) != 0)
                    return std.errc.invalid_argument;

                // if length is non-zero, data must be non-null
                if (length != 0 && data == null)
                    return std.errc.invalid_argument;

                // platforms where size_t is larger than 64 bits are theoretically possible
                //if (std::uint64_t(length) != length)
                //    return std::errc::file_too_large;

                core_file result = new core_in_memory_file(openflags, data, length, false);  //ptr result = new core_in_memory_file(openflags, data, length, false);
                if (result == null)
                    return std.errc.not_enough_memory;

                file = result;
                return new std.error_condition();
            }

            // open a RAM-based "file" using the given data and length (read-only), copying the data
            //static std::error_condition open_ram_copy(const void *data, std::size_t length, std::uint32_t openflags, ptr &file);

            // open a proxy "file" that forwards requests to another file object
            //static std::error_condition open_proxy(core_file &file, ptr &proxy);

            // close an open file
            //~core_file() { }

            ~core_file()
            {
                assert(m_isDisposed);  // can remove
            }

            bool m_isDisposed = false;
            public virtual void Dispose()
            {
                close();
                m_isDisposed = true;
            }


            public abstract void close();


            // ----- file positioning -----

            public abstract std.error_condition seek(int64_t offset, int whence);

            public abstract std.error_condition tell(out uint64_t result);

            public abstract std.error_condition length(out uint64_t result);

            public abstract std.error_condition read(PointerU8 buffer, size_t length, out size_t actual);
            public abstract std.error_condition read_at(uint64_t offset, PointerU8 buffer, size_t length, out size_t actual);

            public abstract std.error_condition finalize();
            public abstract std.error_condition flush();
            public abstract std.error_condition write(PointerU8 buffer, size_t length, out size_t actual);
            public abstract std.error_condition write_at(uint64_t offset, PointerU8 buffer, size_t length, out size_t actual);


            // return true if we are at the EOF
            protected abstract bool eof();


            // ----- file read -----

            // read one character from the file
            public abstract int getc();

            // put back one character from the file
            public abstract int ungetc(int c);

            // read a full line of text from the file
            public abstract string gets(out string s, int n);


            // open a file with the specified filename, read it into memory, and return a pointer
            //-------------------------------------------------
            //  load - open a file with the specified
            //  filename, read it into memory, and return a
            //  pointer
            //-------------------------------------------------
            public static std.error_condition load(string filename, out PointerU8 data, out uint32_t length)  //std::error_condition load(std::string const &filename, void **data, std::uint32_t &length)
            {
                data = new PointerU8(new MemoryU8());
                length = 0;

                std.error_condition err;

                // attempt to open the file
                core_file file;  //ptr file;
                err = open(filename, OPEN_FLAG_READ, out file);
                if (err)
                    return err;

                // get the size
                uint64_t size;
                err = file.length(out size);
                if (err)
                    return err;
                else if ((uint32_t)size != size) // TODO: change interface to use size_t rather than uint32_t for output size
                    return std.errc.file_too_large;

                // allocate memory
                data = new PointerU8(new MemoryU8((int)size, true));  //*data = std::malloc(std::size_t(size));
                //if (!*data)
                //    return std.errc.not_enough_memory;
                length = (uint32_t)size;

                // read the data
                if (size != 0)
                {
                    size_t actual;
                    err = file.read(data, (size_t)size, out actual);  //err = file->read(*data, std::size_t(size), actual);
                    if (err || (size != actual))
                    {
                        //std::free(*data);
                        data = new PointerU8(new MemoryU8());
                        length = 0;
                        if (err)
                            return err;
                        else
                            return std.errc.io_error; // TODO: revisit this error code - either interrupted by an async signal or file truncated out from under us
                    }
                }

                // close the file and return data
                return new std.error_condition();
            }


            //static std::error_condition load(std::string_view filename, std::vector<uint8_t> &data);


            // ----- file write -----

            // write a line of text to the file
            public abstract int puts(string s);

            // printf-style text write to a file
            public abstract int vprintf(string str);  //util::format_argument_pack<std::ostream> const &args) = 0;
            //template <typename Format, typename... Params> int printf(Format &&fmt, Params &&...args) { return vprintf(util::make_format_argument_pack(std::forward<Format>(fmt), std::forward<Params>(args)...)); }
            public int printf(string format, params object [] args) { return vprintf(string.Format(format, args)); }

            // file truncation
            protected abstract std.error_condition truncate(uint64_t offset);

            public abstract Stream stream { get; }
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


            const int CRLF = 3;  //#error CRLF undefined: must be 1 (CR), 2 (LF) or 3 (CR/LF)


            uint32_t m_openflags;                    // flags we were opened with
            text_file_type m_text_type;                    // text output format
            char [] m_back_chars = new char[UTF8_CHAR_MAX];  //char                m_back_chars[UTF8_CHAR_MAX];    // buffer to hold characters for ungetc
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


            //-------------------------------------------------
            //  getc - read a character from a file
            //-------------------------------------------------
            public override int getc()
            {
                // refresh buffer, if necessary
                if (m_back_char_head == m_back_char_tail)
                {
                    // do we need to check the byte order marks?
                    uint64_t pos;
                    if (!tell(out pos))
                    {
                        if (pos == 0)
                        {
                            size_t readlen;
                            MemoryU8 bom = new MemoryU8(4, true);  //std::uint8_t bom[4];
                            read(new PointerU8(bom), 4, out readlen);
                            if (readlen == 4)
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

                            seek((int64_t)pos, SEEK_SET); // FIXME: don't assume seeking is possible, check for errors
                        }
                    }

                    // fetch the next character
                    // FIXME: all of this plays fast and loose with error checking and seeks backwards far too frequently
                    MemoryU8 utf16_buffer = new MemoryU8(UTF16_CHAR_MAX, true);  //char16_t utf16_buffer[UTF16_CHAR_MAX];
                    var uchar = char32_t.MaxValue;  //auto uchar = char32_t(~0);
                    switch (m_text_type)
                    {
                    default:
                    case text_file_type.OSD:
                        {
                            MemoryU8 default_buffer = new MemoryU8(16, true);  //char default_buffer[16];
                            size_t readlen;
                            read(new PointerU8(default_buffer), (size_t)default_buffer.Count, out readlen);  //read(default_buffer, sizeof(default_buffer), readlen);
                            if (readlen > 0)
                            {
                                //auto const charlen = osd_uchar_from_osdchar(&uchar, default_buffer, readlen / sizeof(default_buffer[0]));
                                uchar = default_buffer[0];
                                var charlen = 1;
                                seek((int64_t)(charlen * 1) - (int64_t)readlen, SEEK_CUR);  //seek(std::int64_t(charlen * sizeof(default_buffer[0])) - readlen, SEEK_CUR);
                            }
                        }
                        break;

                    case text_file_type.UTF8:
                        {
                            MemoryU8 utf8_buffer = new MemoryU8(UTF8_CHAR_MAX, true);  //char utf8_buffer[UTF8_CHAR_MAX];
                            size_t readlen;
                            read(new PointerU8(utf8_buffer), (size_t)utf8_buffer.Count, out readlen);
                            if (readlen > 0)
                            {
                                //auto const charlen = uchar_from_utf8(&uchar, utf8_buffer, readlen / sizeof(utf8_buffer[0]));
                                uchar = utf8_buffer[0];
                                var charlen = 1;
                                seek((int64_t)(charlen * 1) - (int64_t)readlen, SEEK_CUR);  //seek(std::int64_t(charlen * sizeof(utf8_buffer[0])) - readlen, SEEK_CUR);
                            }
                        }
                        break;

                    case text_file_type.UTF16BE:
                        {
                            size_t readlen;
                            read(new PointerU8(utf16_buffer), (size_t)utf16_buffer.Count, out readlen);  //read(utf16_buffer, sizeof(utf16_buffer), readlen);
                            if (readlen > 0)
                            {
                                throw new emu_unimplemented();
                            }
                        }
                        break;

                    case text_file_type.UTF16LE:
                        {
                            size_t readlen;
                            read(new PointerU8(utf16_buffer), (size_t)utf16_buffer.Count, out readlen);  //read(utf16_buffer, sizeof(utf16_buffer), readlen);
                            if (readlen > 0)
                            {
                                throw new emu_unimplemented();
                            }
                        }
                        break;

                    case text_file_type.UTF32BE:
                        {
                            // FIXME: deal with read returning short
                            size_t readlen;
                            MemoryU8 ucharTemp = new MemoryU8(4, true);
                            read(new PointerU8(ucharTemp), 4, out readlen);  //read(&uchar, sizeof(uchar), readlen);
                            if (4 == readlen)  //if (sizeof(uchar) == readlen)
                                throw new emu_unimplemented();
                        }
                        break;

                    case text_file_type.UTF32LE:
                        {
                            // FIXME: deal with read returning short
                            size_t readlen;
                            MemoryU8 ucharTemp = new MemoryU8(4, true);
                            read(new PointerU8(ucharTemp), 4, out readlen);  //read(&uchar, sizeof(uchar), readlen);
                            if (4 == readlen)  //if (sizeof(uchar) == readlen)
                                uchar = little_endianize_int32(ucharTemp.GetUInt32());
                        }
                        break;
                    }

                    if (uchar != char32_t.MaxValue)  //if (uchar != ~0)
                    {
                        // place the new character in the ring buffer
                        m_back_char_head = 0;
                        m_back_char_tail = utf8_from_uchar(out string back_chars, uchar);  //m_back_char_tail = utf8_from_uchar(m_back_chars, std::size(m_back_chars), uchar);
                        back_chars.CopyTo(0, m_back_chars, 0, back_chars.Length);
                        //assert(file->back_char_tail != -1);
                    }
                }

                // now read from the ring buffer
                int result;
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


            //-------------------------------------------------
            //  ungetc - put back a character read from a
            //  file
            //-------------------------------------------------
            public override int ungetc(int c)
            {
                m_back_chars[m_back_char_tail++] = (char)c;
                m_back_char_tail %= (int)std.size(m_back_chars);
                return c;
            }


            //-------------------------------------------------
            //  gets - read a line from a text file
            //-------------------------------------------------
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
                        curStr += (char)c;  //*cur++ = c;
                        n--;
                    }
                }

                // if we put nothing in, return nullptr
                if (curStr == s)
                    return null;

                /* otherwise, terminate */
                //if (n > 0)
                //    *cur++ = 0;
                s = curStr;
                return s;
            }


            //-------------------------------------------------
            //  puts - write a string to a text file
            //-------------------------------------------------
            public override int puts(string s)
            {
                // TODO: what to do about write errors or short writes (interrupted)?
                // The API doesn't lend itself to reporting the error as the return
                // value includes extra bytes inserted like the UTF-8 marker and
                // carriage returns.
                var convbuf = new MemoryU8();  //char convbuf[1024];
                int pconvbufIdx = 0;  //char *pconvbuf = convbuf;
                int count = 0;

                // is this the beginning of the file?  if so, write a byte order mark
                if (!no_bom())
                {
                    uint64_t offset;
                    if (!tell(out offset))
                    {
                        if (offset == 0)
                        {
                            convbuf.Add(0xef);  //*pconvbuf++ = char(0xef);
                            convbuf.Add(0xbb);  //*pconvbuf++ = char(0xbb);
                            convbuf.Add(0xbf);  //*pconvbuf++ = char(0xbf);
                        }
                    }
                }

                // convert '\n' to platform dependant line endings
                foreach (char ch in s)
                {
                    if (ch == '\n')
                    {
                        if (CRLF == 1)      // CR only
                        {
                            convbuf.Add(13);  //*pconvbuf++ = 13;
                        }
                        else if (CRLF == 2) // LF only
                        {
                            convbuf.Add(10);  //*pconvbuf++ = 10;
                        }
                        else if (CRLF == 3) // CR+LF
                        {
                            convbuf.Add(13);  //*pconvbuf++ = 13;
                            convbuf.Add(10);  //*pconvbuf++ = 10;
                        }
                    }
                    else
                    {
                        convbuf.AddRange(Encoding.ASCII.GetBytes(new string(ch, 1)));  //*pconvbuf++ = ch;
                    }

                    // if we overflow, break into chunks
                    //if (pconvbuf >= convbuf + std::size(convbuf) - 10)
                    //{
                    //    std::size_t written;
                    //    write(convbuf, pconvbuf - convbuf, written); // FIXME: error ignored here
                    //    count += written;
                    //    pconvbuf = convbuf;
                    //}
                }

                //var convbufPointer = new Pointer<uint8_t>(convbuf);

                // final flush
                if (convbuf.Count != 0)  //if (pconvbuf != convbuf)
                {
                    size_t written;
                    write(new PointerU8(convbuf), (size_t)convbuf.Count, out written);  //write(convbuf, pconvbuf - convbuf, written); // FIXME: error ignored here
                    count += (int)written;
                }

                return count;
            }


            //-------------------------------------------------
            //  vprintf - vfprintf to a text file
            //-------------------------------------------------
            public override int vprintf(string str)  //util::format_argument_pack<std::ostream> const &args) override;
            {
                throw new emu_unimplemented();
            }


            protected bool read_access() { return 0U != (m_openflags & OPEN_FLAG_READ); }
            protected bool write_access() { return 0U != (m_openflags & OPEN_FLAG_WRITE); }
            bool no_bom() { return 0U != (m_openflags & util.OPEN_FLAG_NO_BOM); }


            protected bool has_putback() { return m_back_char_head != m_back_char_tail; }
            protected void clear_putback() { m_back_char_head = m_back_char_tail = 0; }
        }


        abstract class core_basic_file : core_text_file
        {
            uint64_t m_index;            // current file offset
            uint64_t m_size;             // total file length


            protected core_basic_file(uint32_t openflags, uint64_t length)
                : base(openflags)
            {
                m_index = 0;
                m_size = length;
            }


            //-------------------------------------------------
            //  seek - seek within a file
            //-------------------------------------------------
            public override std.error_condition seek(int64_t offset, int whence)
            {
                // flush any buffered char
                clear_putback(); // TODO: report errors; also, should the argument check happen before this?

                // switch off the relative location
                switch (whence)
                {
                case SEEK_SET:
                    if (0 > offset)
                        return std.errc.invalid_argument;

                    m_index = (uint64_t)offset;
                    return new std.error_condition();

                case SEEK_CUR:
                    if (0 > offset)
                    {
                        if ((uint64_t)(-offset) > m_index)  //if (std::uint64_t(-offset) > m_index)
                            return std.errc.invalid_argument;
                    }
                    else if ((uint64_t.MaxValue - (uint64_t)offset) < m_index)  //else if ((std::numeric_limits<std::uint64_t>::max() - offset) < m_index)
                    {
                        return std.errc.invalid_argument;
                    }

                    m_index += (uint64_t)offset;
                    return new std.error_condition();

                case SEEK_END:
                    if (0 > offset)
                    {
                        if ((uint64_t)(-offset) > m_size)  //if (std::uint64_t(-offset) > m_size)
                            return std.errc.invalid_argument;
                    }
                    else if ((uint64_t.MaxValue - (uint64_t)offset) < m_size)  //else if ((std::numeric_limits<std::uint64_t>::max() - offset) < m_size)
                    {
                        return std.errc.invalid_argument;
                    }

                    m_index = m_size + (uint64_t)offset;
                    return new std.error_condition();

                default:
                    return std.errc.invalid_argument;
                }
            }


            public override std.error_condition tell(out uint64_t result) { result = m_index; return new std.error_condition(); }
            public override std.error_condition length(out uint64_t result) { result = m_size; return new std.error_condition(); }


            //-------------------------------------------------
            //  eof - return true if we're at the end of file
            //-------------------------------------------------
            protected override bool eof()
            {
                // check for buffered chars
                if (has_putback())
                    return false;

                // if the offset == length, we're at EOF
                return (m_index >= m_size);
            }


            protected uint64_t index() { return m_index; }
            protected void add_offset(size_t increment) { m_index += increment; m_size = std.max(m_size, m_index); }
            protected uint64_t size() { return m_size; }
            protected void set_size(uint64_t value) { m_size = value; }

            protected static size_t safe_buffer_copy(
                PointerU8 source, size_t sourceoffs, size_t sourcelen,  //void const *source, std::size_t sourceoffs, std::size_t sourcelen,
                PointerU8 dest, size_t destoffs, size_t destlen)  //void *dest, std::size_t destoffs, std::size_t destlen)
            {
                var sourceavail = sourcelen - sourceoffs;
                var destavail = destlen - destoffs;
                var bytes_to_copy = std.min(sourceavail, destavail);
                if (bytes_to_copy > 0)
                {
                    std.memcpy(
                        new PointerU8(dest, (int)destoffs),  //reinterpret_cast<std::uint8_t *>(dest) + destoffs,
                        new PointerU8(source, (int)sourceoffs),  //reinterpret_cast<std::uint8_t const *>(source) + sourceoffs,
                        bytes_to_copy);
                }

                return bytes_to_copy;
            }
        }


        class core_in_memory_file : core_basic_file
        {
            bool m_data_allocated;   // was the data allocated by us?
            PointerU8 m_data;  //void const *    m_data;             // file data, if RAM-based


            public core_in_memory_file(uint32_t openflags, PointerU8 data, size_t length, bool copy)  //core_in_memory_file(std::uint32_t openflags, void const *data, std::size_t length, bool copy) noexcept
                : base(openflags, length)
            {
                m_data_allocated = false;
                m_data = copy ? null : data;


                if (copy)
                {
                    PointerU8 buf = allocate();  // void *const buf = allocate();
                    if (buf != null)
                        std.memcpy(buf, data, length);  //if (buf) std::memcpy(buf, data, length);
                }
            }

            //~core_in_memory_file() { purge(); }

            public override void close()
            {
                purge();
            }


            //-------------------------------------------------
            //  read - read from a file
            //-------------------------------------------------
            public override std.error_condition read(PointerU8 buffer, size_t length, out size_t actual)  //virtual std::error_condition read(void *buffer, std::size_t length, std::size_t &actual) noexcept override;
            {
                clear_putback();

                if (index() < size())
                    actual = safe_buffer_copy(m_data, (size_t)index(), (size_t)size(), buffer, 0, length);
                else
                    actual = 0U;

                add_offset(actual);
                return new std.error_condition();
            }

            public override std.error_condition read_at(uint64_t offset, PointerU8 buffer, size_t length, out size_t actual)  //virtual std::error_condition read_at(std::uint64_t offset, void *buffer, std::size_t length, std::size_t &actual) noexcept override;
            {
                clear_putback();

                // handle RAM-based files
                if (offset < size())
                    actual = safe_buffer_copy(m_data, (size_t)offset, (size_t)size(), buffer, 0, length);
                else
                    actual = 0U;

                return new std.error_condition();
            }

            public override std.error_condition finalize() { return new std.error_condition(); }  //virtual std::error_condition finalize() noexcept override { return std::error_condition(); }
            public override std.error_condition flush() { clear_putback(); return new std.error_condition(); }  //virtual std::error_condition flush() noexcept override { clear_putback(); return std::error_condition(); }
            public override std.error_condition write(PointerU8 buffer, size_t length, out size_t actual) { actual = 0; return std.errc.bad_file_descriptor; }  //virtual std::error_condition write(void const *buffer, std::size_t length, std::size_t &actual) noexcept override { actual = 0; return std::errc::bad_file_descriptor; }
            public override std.error_condition write_at(uint64_t offset, PointerU8 buffer, size_t length, out size_t actual) { actual = 0; return std.errc.bad_file_descriptor; }  //virtual std::error_condition write_at(std::uint64_t offset, void const *buffer, std::size_t length, std::size_t &actual) noexcept override { actual = 0; return std::errc::bad_file_descriptor; }


            public PointerU8 buffer() { return new PointerU8(m_data); }  //void const *buffer() const { return m_data; }


            //-------------------------------------------------
            //  truncate - truncate a file
            //-------------------------------------------------
            protected override std.error_condition truncate(uint64_t offset)
            {
                if (size() < offset)
                    return std.errc.io_error; // TODO: revisit this error code

                // adjust to new length and offset
                set_size(offset);
                return new std.error_condition();
            }


            protected PointerU8 allocate()  //void *allocate()
            {
                if (m_data != null || (size_t.MaxValue < size()))  //if (m_data || (std::numeric_limits<std::size_t>::max() < size()))
                    return null;

                MemoryU8 data = new MemoryU8((int)size(), true);  //void *data = malloc(size());
                data.Resize((int)size());
                if (data != null)
                {
                    m_data_allocated = true;
                    m_data = new PointerU8(data);
                }

                return new PointerU8(data);
            }


            protected void purge()
            {
                if (m_data != null && m_data_allocated)
                    m_data = null;  //free(const_cast<void *>(m_data));
                m_data_allocated = false;
                m_data = null;
            }


            public override Stream stream { get { throw new emu_unimplemented(); } }
        }


        sealed class core_osd_file : core_basic_file
        {
            const int FILE_BUFFER_SIZE = 512;


            osd_file m_file;                     //osd_file::ptr   m_file;                     // OSD file handle
            uint64_t m_bufferbase = 0;               // base offset of internal buffer
            uint32_t m_bufferbytes = 0;              // bytes currently loaded into buffer
            MemoryU8 m_buffer = new MemoryU8(FILE_BUFFER_SIZE, true);  //std::uint8_t    m_buffer[FILE_BUFFER_SIZE]; // buffer data


            public core_osd_file(uint32_t openmode, osd_file file, uint64_t length)
                : base(openmode, length)
            {
                m_file = file;
            }


            public override void close()
            {
                m_file.Dispose();
                m_file = null;
            }


            //-------------------------------------------------
            //  read - read from a file
            //-------------------------------------------------
            public override std.error_condition read(PointerU8 buffer, size_t length, out size_t actual)  //virtual std::error_condition read(void *buffer, std::size_t length, std::size_t &actual) noexcept override;
            {
                // since osd_file works like pread/pwrite, implement in terms of read_at
                // core_osd_file is declared final, so a derived class can't interfere
                std.error_condition err = read_at(index(), buffer, length, out actual);
                add_offset(actual);
                return err;
            }


            public override std.error_condition read_at(uint64_t offset, PointerU8 buffer, size_t length, out size_t actual)  //virtual std::error_condition read_at(std::uint64_t offset, void *buffer, std::size_t length, std::size_t &actual) noexcept override;
            {
                if (m_file == null)
                {
                    actual = 0U;
                    return std.errc.bad_file_descriptor;
                }

                // flush any buffered char
                clear_putback();

                actual = 0U;
                std.error_condition err = new std.error_condition();

                // if we're within the buffer, consume that first
                if (is_buffered(offset))
                    actual += safe_buffer_copy(new PointerU8(m_buffer), offset - m_bufferbase, m_bufferbytes, buffer, actual, length);  //actual += safe_buffer_copy(m_buffer, offset - m_bufferbase, m_bufferbytes, buffer, actual, length);

                // if we've got a small amount left, read it into the buffer first
                if (actual < length)
                {
                    if ((length - actual) < (size_t)(m_buffer.Count / 2))  //if ((length - actual) < (sizeof(m_buffer) / 2))
                    {
                        // read as much as makes sense into the buffer
                        m_bufferbase = offset + actual;
                        err = m_file.read(new PointerU8(m_buffer), m_bufferbase, (uint32_t)m_buffer.Count, out m_bufferbytes);  //err = m_file->read(m_buffer, m_bufferbase, sizeof(m_buffer), m_bufferbytes);

                        // do a bounded copy from the buffer to the destination if it succeeded
                        if (!err)
                            actual += safe_buffer_copy(new PointerU8(m_buffer), 0, m_bufferbytes, buffer, actual, length);
                        else
                            m_bufferbytes = 0U;
                    }
                    else
                    {
                        // read the remainder directly from the file
                        do
                        {
                            // may need to split into chunks if size_t is larger than 32 bits
                            uint32_t chunk = std.min(uint32_t.MaxValue, (uint32_t)(length - actual));  //std::uint32_t const chunk = std::min<std::common_type_t<std::uint32_t, std::size_t> >(std::numeric_limits<std::uint32_t>::max(), length - actual);
                            uint32_t bytes_read;
                            err = m_file.read(new PointerU8(buffer) + (uint32_t)actual, offset + actual, chunk, out bytes_read);  //err = m_file->read(reinterpret_cast<std::uint8_t *>(buffer) + actual, offset + actual, chunk, bytes_read);
                            if (err || bytes_read == 0)
                                break;

                            actual += bytes_read;
                        }
                        while (actual < length);
                    }
                }

                // return any errors
                return err;
            }


            //-------------------------------------------------
            //  flush - flush file buffers
            //-------------------------------------------------
            public override std.error_condition finalize()
            {
                return new std.error_condition();
            }


            public override std.error_condition flush()
            {
                // flush any buffered char
                clear_putback();

                // invalidate any buffered data
                m_bufferbytes = 0U;

                return m_file.flush();
            }


            //-------------------------------------------------
            //  write - write to a file
            //-------------------------------------------------
            public override std.error_condition write(PointerU8 buffer, size_t length, out size_t actual)  //virtual std::error_condition write(void const *buffer, std::size_t length, std::size_t &actual) noexcept override;
            {
                // since osd_file works like pread/pwrite, implement in terms of write_at
                // core_osd_file is declared final, so a derived class can't interfere
                std.error_condition err = write_at(index(), buffer, length, out actual);
                add_offset(actual);
                return err;
            }


            public override std.error_condition write_at(uint64_t offset, PointerU8 buffer, size_t length, out size_t actual)  //virtual std::error_condition write_at(std::uint64_t offset, void const *buffer, std::size_t length, std::size_t &actual) noexcept override;
            {
                // flush any buffered char
                clear_putback();

                // invalidate any buffered data
                m_bufferbytes = 0U;

                // do the write - may need to split into chunks if size_t is larger than 32 bits
                actual = 0U;
                while (length != 0)
                {
                    // bytes written not valid on error
                    uint32_t chunk = std.min(uint32_t.MaxValue, (uint32_t)length);  //std::uint32_t const chunk = std::min<std::common_type_t<std::uint32_t, std::size_t> >(std::numeric_limits<std::uint32_t>::max(), length);
                    uint32_t bytes_written;
                    std.error_condition err = m_file.write(buffer, offset, chunk, out bytes_written);
                    if (err)
                        return err;

                    assert(chunk >= bytes_written);

                    offset += bytes_written;
                    buffer = buffer + bytes_written;  //buffer = reinterpret_cast<std::uint8_t const *>(buffer) + bytes_written;
                    length -= bytes_written;
                    actual += bytes_written;
                    set_size(std.max(size(), offset));
                }

                return new std.error_condition();
            }


            //-------------------------------------------------
            //  truncate - truncate a file
            //-------------------------------------------------
            protected override std.error_condition truncate(uint64_t offset)
            {
                // truncate file
                std.error_condition err = m_file.truncate(offset);
                if (err)
                    return err;

                // and adjust to new length and offset
                set_size(offset);
                return new std.error_condition();
            }


            bool is_buffered(uint64_t offset) { return (offset >= m_bufferbase) && (offset < (m_bufferbase + m_bufferbytes)); }


            public override Stream stream { get { throw new emu_unimplemented(); } }
        }
    }


    public static class corefile_global
    {
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
    }
}
