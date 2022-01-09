// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.IO;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives;

using int64_t = System.Int64;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using static mame.corestr_global;
using static mame.cpp_global;
using static mame.osdcore_global;
using static mame.osdfile_global;


namespace mame
{
    public static partial class util
    {
        /***************************************************************************
            TYPE DEFINITIONS
        ***************************************************************************/

        class CFileInStream //: ISeekInStream
        {
            public random_read file;  //random_read::ptr    file;
            //std::uint64_t       currfpos = 0;
            public uint64_t length = 0;


            public CFileInStream()
            {
                //Read = [] (void *pp, void *buf, size_t *size) { return reinterpret_cast<CFileInStream *>(pp)->read(buf, *size); };
                //Seek = [] (void *pp, Int64 *pos, ESzSeek origin) { return reinterpret_cast<CFileInStream *>(pp)->seek(*pos, origin); };
            }

            //SRes read(void *data, std::size_t &size) noexcept

            //SRes seek(Int64 &pos, ESzSeek origin) noexcept
        }


        class m7z_file_impl
        {
            //typedef std::unique_ptr<m7z_file_impl> ptr;


            const int CACHE_SIZE = 8;
            static m7z_file_impl [] s_cache = new m7z_file_impl[CACHE_SIZE];  //static std::array<ptr, CACHE_SIZE>      s_cache;
            static object s_cache_mutex = new object();  //static std::mutex                       s_cache_mutex;


            string m_filename;             // copy of _7Z filename (for caching)

            int m_curr_file_idx;        // current file index
            bool m_curr_is_dir;          // current file is directory
            string m_curr_name;            // current file name
            int64_t m_curr_length;  //uint64_t m_curr_length;          // current file uncompressed length
            int64_t m_curr_modified;  //std::chrono::system_clock::time_point   m_curr_modified;        // current file modification time
            int64_t m_curr_crc;  //uint32_t m_curr_crc;             // current file crc

            //std_vector<UInt16> m_utf16_buf;
            //std_vector<char32_t> m_uchar_buf;
            //std_vector<char> m_utf8_buf;

            CFileInStream m_archive_stream = new CFileInStream();
            //CLookToRead                             m_look_stream;
            //CSzArEx                                 m_db;
            //ISzAlloc                                m_alloc_imp;
            //ISzAlloc                                m_alloc_temp_imp;
            bool m_inited;

            // cached stuff for solid blocks
            UInt32 m_block_index;
            //Byte *                                  m_out_buffer;
            //int m_out_buffer_size;


            SevenZipArchive m_sharpCompressArchive = null;


            public m7z_file_impl(string filename)
            {
                m_filename = filename;
                m_curr_file_idx = -1;
                m_curr_is_dir = false;
                m_curr_name = "";
                m_curr_length = 0;
                m_curr_modified = 0;
                m_curr_crc = 0;
                //, m_utf16_buf()
                //, m_uchar_buf()
                //, m_utf8_buf()
                m_inited = false;
                m_block_index = 0;
                //m_out_buffer(nullptr);
                //m_out_buffer_size = 0;


                //m_alloc_imp.Alloc = &SzAlloc;
                //m_alloc_imp.Free = &SzFree;

                //m_alloc_temp_imp.Alloc = &SzAllocTemp;
                //m_alloc_temp_imp.Free = &SzFreeTemp;

                //LookToRead_CreateVTable(&m_look_stream, False);
                //m_look_stream.realStream = &m_archive_stream;
                //LookToRead_Init(&m_look_stream);
            }

            //m7z_file_impl(random_read::ptr &&file) noexcept
            //    : m7z_file_impl(std::string())
            //{
            //    m_archive_stream.file = std::move(file);
            //}

            //~m7z_file_impl()


            public static m7z_file_impl find_cached(string filename)
            {
                lock (s_cache_mutex)  //std::lock_guard<std::mutex> guard(s_cache_mutex);
                {
                    for (int cachenum = 0; cachenum < s_cache.Length; cachenum++)
                    {
                        // if we have a valid entry and it matches our filename, use it and remove from the cache
                        if (s_cache[cachenum] != null && (filename == s_cache[cachenum].m_filename))
                        {
                            //using std::swap;
                            m7z_file_impl result;  //ptr result;
                            result = s_cache[cachenum];  //std::swap(s_cache[cachenum], result);
                            s_cache[cachenum] = null;
                            osd_printf_verbose("un7z: found {0} in cache\n", filename);
                            return result;
                        }
                    }
                }

                return null;  //return ptr();
            }


            public static void close(m7z_file_impl archive)
            {
                // if the filename isn't empty, the implementation can be cached
                if (archive != null && !archive.m_filename.empty())
                {
                    archive.m_sharpCompressArchive.Dispose();
                    archive.m_archive_stream.file = null;

#if false
                    // close the open files
                    osd_printf_verbose("un7z: closing archive file %s and sending to cache\n", archive->m_filename);
                    archive->m_archive_stream.file.reset();

                    // find the first nullptr entry in the cache
                    std::lock_guard<std::mutex> guard(s_cache_mutex);
                    std::size_t cachenum;
                    for (cachenum = 0; cachenum < s_cache.size(); cachenum++)
                        if (!s_cache[cachenum])
                            break;

                    // if no room left in the cache, free the bottommost entry
                    if (cachenum == s_cache.size())
                    {
                        cachenum--;
                        osd_printf_verbose("un7z: removing %s from cache to make space\n", s_cache[cachenum]->m_filename);
                        s_cache[cachenum].reset();
                    }

                    // move everyone else down and place us at the top
                    for ( ; cachenum > 0; cachenum--)
                        s_cache[cachenum] = std::move(s_cache[cachenum - 1]);
                    s_cache[0] = std::move(archive);
#endif
                }

                // make sure it's cleaned up
                archive = null;  //archive.reset();
            }


            /*-------------------------------------------------
                _7z_file_cache_clear - clear the _7Z file
                cache and free all memory
            -------------------------------------------------*/
            public static void m7z_file_cache_clear()
            {
                // This is a trampoline called from unzip.cpp to avoid the need to have the zip and 7zip code in one file
                cache_clear();
            }


            static void cache_clear()
            {
                // clear call cache entries
                lock (s_cache_mutex)  //std::lock_guard<std::mutex> guard(s_cache_mutex);
                {
                    for (int i = 0; i < s_cache.Length; i++)  //for (auto &cached : s_cache)
                        s_cache[i] = null;  //cached.reset();
                }
            }


            public std.error_condition initialize()
            {
#if false
                try
                {
                    if (m_utf16_buf.size() < 128)
                        m_utf16_buf.resize(128);
                    if (m_uchar_buf.size() < 128)
                        m_uchar_buf.resize(128);
                    m_utf8_buf.reserve(512);
                }
                catch (...)
                {
                    return std::errc::not_enough_memory;
                }
#endif

                if (m_archive_stream.file == null)
                {
                    osd_file file;  //osd_file::ptr file;
                    std.error_condition err = m_osdfile.open(m_filename, OPEN_FLAG_READ, out file, out m_archive_stream.length);  //std::error_condition const err = osd_file::open(m_filename, OPEN_FLAG_READ, file, m_archive_stream.length);
                    if (err)
                        return err;

                    m_archive_stream.file = osd_file_read(file);  //m_archive_stream.file = osd_file_read(std::move(file));
                    //osd_printf_verbose("un7z: opened archive file %s\n", m_filename);
                }
                else if (m_archive_stream.length == 0)
                {
                    std.error_condition err = m_archive_stream.file.length(out m_archive_stream.length);  //std::error_condition const err = m_archive_stream.file->length(m_archive_stream.length);
                    if (err)
                    {
                        osd_printf_verbose(
                                "un7z: error getting length of archive file {0} ({1}:{2} {3})\n",
                                m_filename, err.category().name(), err.value(), err.message());
                        return err;
                    }
                }

#if false
                // TODO: coordinate this with other LZMA users in the codebase?
                struct crc_table_generator { crc_table_generator() { CrcGenerateTable(); } };
                static crc_table_generator crc_table;

                SzArEx_Init(&m_db);
                m_inited = true;
                SRes const res = SzArEx_Open(&m_db, &m_look_stream.s, &m_alloc_imp, &m_alloc_temp_imp);
                if (res != SZ_OK)
                {
                    osd_printf_error("un7z: error opening %s as 7z archive (%d)\n", m_filename, int(res));
                    switch (res)
                    {
                    case SZ_ERROR_UNSUPPORTED:  return archive_file::error::UNSUPPORTED;
                    case SZ_ERROR_MEM:          return std::errc::not_enough_memory;
                    case SZ_ERROR_INPUT_EOF:    return archive_file::error::FILE_TRUNCATED;
                    default:                    return std::errc::io_error; // TODO: better default error?
                    }
                }

                return std::error_condition();
#endif

                try
                {
                    m_sharpCompressArchive = SevenZipArchive.Open(m_archive_stream.file.stream);
                }
                catch (Exception e)
                {
                    m_sharpCompressArchive = null;
                    return std.errc.io_error;
                }

                return new std.error_condition();
            }


            public int first_file()
            {
                return search(0, 0, "", false, false, false);
            }

            public int next_file()
            {
                throw new emu_unimplemented();
                //return (m_curr_file_idx < 0) ? -1 : search(m_curr_file_idx + 1, 0, std::string_view(), false, false, false);
            }

            public int search(uint32_t crc)
            {
                return search(0, crc, "", true, false, false);
            }

            public int search(string filename, bool partialpath)
            {
                return search(0, 0, filename, false, true, partialpath);
            }

            public int search(uint32_t crc, string filename, bool partialpath)
            {
                return search(0, crc, filename, true, true, partialpath);
            }

            public bool current_is_directory() { throw new emu_unimplemented(); }  //{ return m_curr_is_dir; }
            public string current_name() { throw new emu_unimplemented(); } //{ return m_curr_name; }
            public int64_t current_uncompressed_length() { return m_curr_length; }
            public virtual int64_t current_last_modified() { throw new emu_unimplemented(); } //{ return m_curr_modified; }
            public int64_t current_crc() { return m_curr_crc; }


            public std.error_condition decompress(MemoryU8 buffer, uint32_t length)
            {
#if false
                // if we don't have enough buffer, error
                if (length < m_curr_length)
                {
                    osd_printf_error("un7z: buffer too small to decompress %s from %s\n", m_curr_name, m_filename);
                    return archive_file::error::BUFFER_TOO_SMALL;
                }

                // make sure the file is open..
                if (!m_archive_stream.file)
                {
                    m_archive_stream.currfpos = 0; // FIXME: should it really be changing the file pointer out from under LZMA?
                    osd_file::ptr file;
                    std::error_condition const err = osd_file::open(m_filename, OPEN_FLAG_READ, file, m_archive_stream.length);
                    if (err)
                    {
                        osd_printf_error(
                                "un7z: error reopening archive file %s (%s:%d %s)\n",
                                m_filename, err.category().name(), err.value(), err.message());
                        return err;
                    }
                    m_archive_stream.file = osd_file_read(std::move(file));
                    osd_printf_verbose("un7z: reopened archive file %s\n", m_filename);
                }

                std::size_t offset(0);
                std::size_t out_size_processed(0);
                SRes const res = SzArEx_Extract(
                        &m_db, &m_look_stream.s, m_curr_file_idx,           // requested file
                        &m_block_index, &m_out_buffer, &m_out_buffer_size,  // solid block caching
                        &offset, &out_size_processed,                       // data size/offset
                        &m_alloc_imp, &m_alloc_temp_imp);                   // allocator helpers
                if (res != SZ_OK)
                {
                    osd_printf_error("un7z: error decompressing %s from %s (%d)\n", m_curr_name, m_filename, int(res));
                    switch (res)
                    {
                    case SZ_ERROR_UNSUPPORTED:  return archive_file::error::UNSUPPORTED;
                    case SZ_ERROR_MEM:          return std::errc::not_enough_memory;
                    case SZ_ERROR_INPUT_EOF:    return archive_file::error::FILE_TRUNCATED;
                    default:                    return archive_file::error::DECOMPRESS_ERROR;
                    }
                }

                // copy to destination buffer
                std::memcpy(buffer, m_out_buffer + offset, (std::min<std::size_t>)(length, out_size_processed));
                return std::error_condition();
#endif
                int cur = 0;
                foreach (var entry in m_sharpCompressArchive.Entries)
                {
                    if (cur++ == m_curr_file_idx)
                    {
                        MemoryStream stream = new MemoryStream((int)entry.Size);
                        entry.WriteTo(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        int bufferPos = 0;
                        while (bufferPos < stream.Length)
                            buffer[bufferPos++] = Convert.ToByte(stream.ReadByte());

                        return new std.error_condition();
                    }
                }

                return std.errc.io_error;
            }


            int search(
                int i,
                uint32_t search_crc,
                string search_filename,
                bool matchcrc,
                bool matchname,
                bool partialpath)
            {
                try
                {
                    int cur = 0;

                    foreach (var entry in m_sharpCompressArchive.Entries)  //for ( ; i < m_db.NumFiles; i++)
                    {
                        if (cur < i - 1)
                            continue;

                        //make_utf8_name(i);
                        bool is_dir = entry.IsDirectory;  //bool const is_dir(SzArEx_IsDir(&m_db, i));
                        int64_t size = entry.Size;  //const std::uint64_t size(SzArEx_GetFileSize(&m_db, i));
                        int64_t crc = entry.Crc;  //const std::uint32_t crc(m_db.CRCs.Vals[i]);

                        bool crcmatch = crc == search_crc;  //const bool crcmatch(SzBitArray_Check(m_db.CRCs.Defs, i) && (crc == search_crc));
                        bool found;
                        if (!matchname)
                        {
                            found = !matchcrc || (crcmatch && !is_dir);
                        }
                        else
                        {
                            var partialoffset = entry.Key.size() - search_filename.length();  //auto const partialoffset = m_utf8_buf.size() - search_filename.length();
                            bool namematch = (search_filename.length() == entry.Key.size()) && (search_filename.empty() || (core_stricmp(search_filename, entry.Key) == 0));  //const bool namematch = (search_filename.length() == m_utf8_buf.size()) && (search_filename.empty() || !core_strnicmp(&search_filename[0], &m_utf8_buf[0], search_filename.length()));
                            bool partialmatch = partialpath && ((entry.Key.size() > search_filename.length()) && (entry.Key[(int)partialoffset - 1] == '/')) && (search_filename.empty() || core_strnicmp(search_filename, entry.Key.Substring((int)partialoffset), search_filename.length()) == 0);  //bool const partialmatch = partialpath && ((m_utf8_buf.size() > search_filename.length()) && (m_utf8_buf[partialoffset - 1] == '/')) && (search_filename.empty() || !core_strnicmp(&search_filename[0], &m_utf8_buf[partialoffset], search_filename.length()));
                            found = (!matchcrc || crcmatch) && (namematch || partialmatch);  //found = (!matchcrc || crcmatch) && (namematch || partialmatch);
                        }

                        if (found)
                        {
                            // set the name first - resizing it can throw an exception, and we want the state to be consistent
                            m_curr_name = entry.Key;  //m_curr_name.assign(m_utf8_buf.begin(), m_utf8_buf.end());
                            m_curr_file_idx = cur;  //m_curr_file_idx = i;
                            m_curr_is_dir = is_dir;
                            m_curr_length = size;
                            //set_curr_modified();
                            m_curr_crc = crc;

                            return cur;  //return i;
                        }

                        cur++;
                    }
                }
                catch (Exception)  //catch (...)
                {
                    // allocation error handling name
                }
                return -1;
            }

            //void make_utf8_name(int index);
            //void set_curr_modified();
        }


        class m7z_file_wrapper : archive_file, IDisposable
        {
            m7z_file_impl m_impl;


            public m7z_file_wrapper(m7z_file_impl impl)
            {
                m_impl = impl;

                assert(m_impl != null);
            }

            //~m7z_file_wrapper()
            //{
            //    m7z_file_impl.close(m_impl);
            //}

            public override void Dispose()
            {
                m7z_file_impl.close(m_impl);
                base.Dispose();
            }


            protected override int first_file() { return m_impl.first_file(); }
            protected override int next_file() { return m_impl.next_file(); }

            public override int search(uint32_t crc) { return m_impl.search(crc); }
            public override int search(string filename, bool partialpath) { return m_impl.search(filename, partialpath); }
            public override int search(uint32_t crc, string filename, bool partialpath) { return m_impl.search(crc, filename, partialpath); }

            protected override bool current_is_directory() { return m_impl.current_is_directory(); }

            protected override string current_name() { return m_impl.current_name(); }
            public override uint64_t current_uncompressed_length() { return (uint64_t)m_impl.current_uncompressed_length(); }
            protected override int64_t current_last_modified() { return m_impl.current_last_modified(); }
            public override uint32_t current_crc() { return (uint32_t)m_impl.current_crc(); }

            public override std.error_condition decompress(MemoryU8 buffer, uint32_t length) { return m_impl.decompress(buffer, length); }
        }
    }
}
