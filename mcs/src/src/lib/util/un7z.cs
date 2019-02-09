// license:BSD-3-Clause
// copyright-holders:Edward Fast

using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.IO;

using int64_t = System.Int64;
using ListBytes = mame.ListBase<System.Byte>;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame.util
{
    /***************************************************************************
        TYPE DEFINITIONS
    ***************************************************************************/

    class CSzFile
    {
        uint64_t currfpos;
        public uint64_t length;
        public osd_file osdfile;


        public CSzFile()
        {
            currfpos = 0;
            length = 0;
            osdfile = null;
        }


#if false
        SRes read(void *data, std::size_t &size)
        {
            if (!osdfile)
            {
                osd_printf_error("un7z: called CSzFile::read without file\n");
                return SZ_ERROR_READ;
            }

            if (!size)
                return SZ_OK;

            // TODO: this casts a size_t to a uint32_t, so it will fail if >=4GB is requested at once (need a loop)
            std::uint32_t read_length(0);
            auto const err = osdfile->read(data, currfpos, size, read_length);
            size = read_length;
            currfpos += read_length;

            return (osd_file::error::NONE == err) ? SZ_OK : SZ_ERROR_READ;
        }

        SRes seek(Int64 &pos, ESzSeek origin)
        {
            switch (origin)
            {
            case SZ_SEEK_CUR: currfpos += pos; break;
            case SZ_SEEK_SET: currfpos = pos; break;
            case SZ_SEEK_END: currfpos = length + pos; break;
            default: return SZ_ERROR_READ;
            }
            pos = currfpos;
            return SZ_OK;
        }
#endif
    }


    class CFileInStream : //ISeekInStream
                          CSzFile
    {
        public CFileInStream()
        {
            //Read = &FileInStream_Read;
            //Seek = &FileInStream_Seek;
        }
    }


    class m7z_file_impl : global_object
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
            //m_utf16_buf = new std_vector<UInt16>(128);
            //m_uchar_buf = new std_vector<double>(128);
            //m_utf8_buf = new std_vector<char>(512);
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

        //~m7z_file_impl()
        //{
        //    if (m_out_buffer)
        //        IAlloc_Free(&m_alloc_imp, m_out_buffer);
        //    if (m_inited)
        //        SzArEx_Free(&m_db, &m_alloc_imp);
        //}


        public static m7z_file_impl find_cached(string filename)
        {
            lock (s_cache_mutex)  //std::lock_guard<std::mutex> guard(s_cache_mutex);
            {
                for (int cachenum = 0; cachenum < s_cache.Length; cachenum++)
                {
                    // if we have a valid entry and it matches our filename, use it and remove from the cache
                    if (s_cache[cachenum] != null && (filename == s_cache[cachenum].m_filename))
                    {
                        m7z_file_impl result;
                        result = s_cache[cachenum];  //std::swap(s_cache[cachenum], result);
                        s_cache[cachenum] = null;
                        osd_printf_verbose("un7z: found {0} in cache\n", filename.c_str());
                        return result;
                    }
                }
            }

            return null;  //return ptr();
        }


        public static void close(m7z_file_impl archive)
        {
            if (archive == null) return;

            // close the open files
            //global.osd_printf_verbose("un7z: closing archive file {0} and sending to cache\n", archive.m_filename.c_str());

            archive.m_sharpCompressArchive.Dispose();
            archive.m_archive_stream.osdfile = null;


            //throw new emu_unimplemented();
#if false
            // find the first nullptr entry in the cache
            lock (s_cache_mutex)  //std::lock_guard<std::mutex> guard(s_cache_mutex);
            {
                int cachenum;
                for (cachenum = 0; cachenum < s_cache.Length; cachenum++)
                    if (s_cache[cachenum] == null)
                        break;

                // if no room left in the cache, free the bottommost entry
                if (cachenum == s_cache.Length)
                {
                    cachenum--;
                    global.osd_printf_verbose("un7z: removing {0} from cache to make space\n", s_cache[cachenum].m_filename.c_str());
                    s_cache[cachenum] = null;
                }

                // move everyone else down and place us at the top
                for ( ; cachenum > 0; cachenum--)
                    s_cache[cachenum] = s_cache[cachenum - 1];
                s_cache[0] = archive;
            }
#endif
        }


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
                for (int cachenum = 0; cachenum < s_cache.Length; s_cache[cachenum++] = null) { }
            }
        }


        public archive_file.error initialize()
        {
            osd_file.error err = osdcore_global.m_osdfile.open(m_filename, OPEN_FLAG_READ, out m_archive_stream.osdfile, out m_archive_stream.length);
            if (err != osd_file.error.NONE)
                return archive_file.error.FILE_ERROR;

            //global.osd_printf_verbose("un7z: opened archive file {0}\n", m_filename.c_str());

#if false
            CrcGenerateTable(); // FIXME: doesn't belong here - it should be called once statically

            SzArEx_Init(&m_db);
            m_inited = true;
            SRes const res = SzArEx_Open(&m_db, &m_look_stream.s, &m_alloc_imp, &m_alloc_temp_imp);
            if (res != SZ_OK)
            {
                osd_printf_error("un7z: error opening %s as 7z archive (%d)\n", m_filename.c_str(), int(res));
                switch (res)
                {
                case SZ_ERROR_UNSUPPORTED:  return archive_file.error.UNSUPPORTED;
                case SZ_ERROR_MEM:          return archive_file.error.OUT_OF_MEMORY;
                case SZ_ERROR_INPUT_EOF:    return archive_file.error.FILE_TRUNCATED;
                default:                    return archive_file.error.FILE_ERROR;
                }
            }

            return archive_file::error::NONE;
#endif

            try
            {
                m_sharpCompressArchive = SevenZipArchive.Open(m_archive_stream.osdfile.stream);
            }
            catch (Exception e)
            {
                m_sharpCompressArchive = null;
                return archive_file.error.FILE_ERROR;
            }

            return archive_file.error.NONE;
        }


        public int first_file() { return search(0, 0, "", false, false, false); }
        public int next_file() { throw new emu_unimplemented(); } //return (m_curr_file_idx < 0) ? -1 : search(m_curr_file_idx + 1, 0, "", false, false, false); }

        public int search(uint32_t crc) { return search(0, crc, "", true, false, false); }
        public int search(string filename, bool partialpath) { return search(0, 0, filename, false, true, partialpath); }
        public int search(uint32_t crc, string filename, bool partialpath) { return search(0, crc, filename, true, true, partialpath); }

        public bool current_is_directory() { throw new emu_unimplemented(); }  //{ return m_curr_is_dir; }
        public string current_name() { throw new emu_unimplemented(); } //{ return m_curr_name; }
        public int64_t current_uncompressed_length() { return m_curr_length; }
        public virtual int64_t current_last_modified() { throw new emu_unimplemented(); } //{ return m_curr_modified; }
        public int64_t current_crc() { return m_curr_crc; }


        public archive_file.error decompress(ListBytes buffer, uint32_t length)
        {
#if false
            // if we don't have enough buffer, error
            if (length < m_curr_length)
            {
                osd_printf_error("un7z: buffer too small to decompress %s from %s\n", m_curr_name.c_str(), m_filename.c_str());
                return archive_file::error::BUFFER_TOO_SMALL;
            }

            // make sure the file is open..
            if (!m_archive_stream.osdfile)
            {
                m_archive_stream.currfpos = 0;
                osd_file::error const err = osd_file::open(m_filename, OPEN_FLAG_READ, m_archive_stream.osdfile, m_archive_stream.length);
                if (err != osd_file::error::NONE)
                {
                    osd_printf_error("un7z: error reopening archive file %s (%d)\n", m_filename.c_str(), int(err));
                    return archive_file::error::FILE_ERROR;
                }
                osd_printf_verbose("un7z: reopened archive file %s\n", m_filename.c_str());
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
                osd_printf_error("un7z: error decompressing %s from %s (%d)\n", m_curr_name.c_str(), m_filename.c_str(), int(res));
                switch (res)
                {
                case SZ_ERROR_UNSUPPORTED:  return archive_file::error::UNSUPPORTED;
                case SZ_ERROR_MEM:          return archive_file::error::OUT_OF_MEMORY;
                case SZ_ERROR_INPUT_EOF:    return archive_file::error::FILE_TRUNCATED;
                default:                    return archive_file::error::DECOMPRESS_ERROR;
                }
            }

            // copy to destination buffer
            std::memcpy(buffer, m_out_buffer + offset, (std::min<std::size_t>)(length, out_size_processed));
            return archive_file::error::NONE;
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

                    return archive_file.error.NONE;
                }
            }

            return archive_file.error.FILE_ERROR;
        }


        int search(
            int i,
            uint32_t search_crc,
            string search_filename,
            bool matchcrc,
            bool matchname,
            bool partialpath)
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
                //auto const partialoffset(m_utf8_buf.size() - 1 - search_filename.length());
                //bool const partialpossible((m_utf8_buf.size() > (search_filename.length() + 1)) && (m_utf8_buf[partialoffset - 1] == '/'));
                bool namematch = core_stricmp(search_filename.c_str(), entry.Key) == 0;  //const bool namematch( !core_stricmp(search_filename.c_str(), &m_utf8_buf[0]) || (partialpath && partialpossible && !core_stricmp(search_filename.c_str(), &m_utf8_buf[partialoffset])));

                bool found = ((!matchcrc && !matchname) || !is_dir) && (!matchcrc || crcmatch) && (!matchname || namematch);
                if (found)
                {
                    m_curr_file_idx = cur;
                    m_curr_is_dir = is_dir;
                    m_curr_name = entry.Key;  //m_curr_name = &m_utf8_buf[0];
                    m_curr_length = size;
                    //set_curr_modified();
                    m_curr_crc = crc;

                    return cur;
                }

                cur++;
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

        public override error decompress(ListBytes buffer, uint32_t length) { return m_impl.decompress(buffer, length); }
    }
}
