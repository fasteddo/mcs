// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int64_t = System.Int64;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame.util
{
    // describes an open archive file
    public abstract class archive_file : global_object, IDisposable
    {
        // Error types
        public enum error
        {
            NONE = 0,
            OUT_OF_MEMORY,
            FILE_ERROR,
            BAD_SIGNATURE,
            DECOMPRESS_ERROR,
            FILE_TRUNCATED,
            FILE_CORRUPT,
            UNSUPPORTED,
            BUFFER_TOO_SMALL
        }

        //typedef std::unique_ptr<archive_file> ptr;


        /* ----- archive file access ----- */

        // open a ZIP file and parse its central directory
        /*-------------------------------------------------
            zip_file_open - opens a ZIP file for reading
        -------------------------------------------------*/
        public static error open_zip(string filename, out archive_file result)
        {
            //throw new emu_unimplemented();

            result = null;
            return error.UNSUPPORTED;
        }


        // open a 7Z file and parse its central directory
        public static error open_7z(string filename, out archive_file result)
        {
            // ensure we start with a nullptr result
            result = null;  //result.reset();

            // see if we are in the cache, and reopen if so
            m7z_file_impl newimpl = m7z_file_impl.find_cached(filename);

            if (newimpl == null)
            {
                // allocate memory for the 7z file structure
                try { newimpl = new m7z_file_impl(filename); }  //try { newimpl = std.make_unique<m7z_file_impl>(filename); }
                catch (Exception e) { return error.OUT_OF_MEMORY; }
                error err = newimpl.initialize();
                if (err != error.NONE) return err;
            }

            try
            {
                result = new m7z_file_wrapper(newimpl);  //result = std.make_unique<m7z_file_wrapper>(std::move(newimpl));
                return error.NONE;
            }
            catch (Exception e)
            {
                m7z_file_impl.close(newimpl);
                return error.OUT_OF_MEMORY;
            }
        }

        // close an archive file (may actually be left open due to caching)
        ~archive_file()
        {
            assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public virtual void Dispose()
        {
            m_isDisposed = true;
        }


        // clear out all open files from the cache
        public static void cache_clear()
        {
            zip_file_impl.cache_clear();
            m7z_file_impl.m7z_file_cache_clear();
        }


        /* ----- contained file access ----- */

        // iterating over files - returns negative on reaching end
        protected abstract int first_file();
        protected abstract int next_file();


        // find a file index by crc, filename or both - returns non-negative on match
        public abstract int search(uint32_t crc);
        public abstract int search(string filename, bool partialpath);
        public abstract int search(uint32_t crc, string filename, bool partialpath);


        // information on most recently found file
        protected abstract bool current_is_directory();
        protected abstract string current_name();
        public abstract uint64_t current_uncompressed_length();
        protected abstract int64_t current_last_modified();  //virtual std::chrono::system_clock::time_point current_last_modified() const = 0;
        public abstract uint32_t current_crc();


        // decompress the most recently found file in the ZIP
        public abstract error decompress(MemoryU8 buffer, uint32_t length);  //void *buffer, std::uint32_t length)
    }


    class zip_file_impl
    {
        public static void cache_clear()
        {
            //throw new emu_unimplemented();
#if false
            // clear call cache entries
            std::lock_guard<std::mutex> guard(s_cache_mutex);
            for (std::size_t cachenum = 0; cachenum < s_cache.size(); s_cache[cachenum++].reset()) { }
#endif
        }
    }
}
