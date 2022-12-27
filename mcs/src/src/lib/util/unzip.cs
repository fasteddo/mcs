// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using int64_t = System.Int64;
using PointerU8 = mame.Pointer<System.Byte>;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using static mame.cpp_global;


namespace mame
{
    public static partial class util
    {
        // describes an open archive file
        public abstract class archive_file : IDisposable
        {
            // Error types
            public enum error : int
            {
                BAD_SIGNATURE = 1,
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
            public static std.error_condition open_zip(string filename, out archive_file result)
            {
                //throw new emu_unimplemented();

                result = null;
                return std.errc.not_supported;
            }

            //static std::error_condition open_zip(std::unique_ptr<random_read> &&file, ptr &result) noexcept;


            // open a 7Z file and parse its central directory
            public static std.error_condition open_7z(string filename, out archive_file result)
            {
                // ensure we start with a nullptr result
                result = null;  //result.reset();

                // see if we are in the cache, and reopen if so
                m7z_file_impl newimpl = m7z_file_impl.find_cached(filename);  //m7z_file_impl::ptr newimpl(m7z_file_impl::find_cached(filename));

                if (newimpl == null)
                {
                    // allocate memory for the 7z file structure
                    try { newimpl = new m7z_file_impl(filename); }  //try { newimpl = std.make_unique<m7z_file_impl>(filename); }
                    catch (Exception e) { return std.errc.not_enough_memory; }
                    var err = newimpl.initialize();
                    if (err)
                        return err;
                }

                // allocate the archive API wrapper
                result = new m7z_file_wrapper(newimpl);  //result.reset(new (std::nothrow) m7z_file_wrapper(std::move(newimpl)));
                if (result != null)
                {
                    return new std.error_condition();
                }
                else
                {
                    m7z_file_impl.close(newimpl);  //m7z_file_impl::close(std::move(newimpl));
                    return std.errc.not_enough_memory;
                }
            }

            //static std::error_condition open_7z(std::unique_ptr<random_read> &&file, ptr &result) noexcept;


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
            public abstract std.error_condition decompress(PointerU8 buffer, uint32_t length);  //void *buffer, std::uint32_t length)
        }


        // error category for archive errors
        //std::error_category const &archive_category() noexcept;
        //inline std::error_condition make_error_condition(archive_file::error err) noexcept { return std::error_condition(int(err), archive_category()); }


        //class archive_category_impl : public std::error_category


        class zip_file_impl
        {
            //using ptr = std::unique_ptr<zip_file_impl>;

            //zip_file_impl(std::string &&filename) noexcept
            //    : m_filename(std::move(filename))
            //{
            //    std::fill(m_buffer.begin(), m_buffer.end(), 0);
            //}

            //zip_file_impl(random_read::ptr &&file) noexcept
            //    : zip_file_impl(std::string())
            //{
            //    m_file = std::move(file);
            //}

            //static ptr find_cached(std::string_view filename) noexcept

            //static void close(ptr &&zip) noexcept;


            public static void cache_clear()
            {
                //throw new emu_unimplemented();
#if false
                // clear call cache entries
                std::lock_guard<std::mutex> guard(s_cache_mutex);
                for (std::size_t cachenum = 0; cachenum < s_cache.size(); s_cache[cachenum++].reset()) { }
#endif
            }

            //std::error_condition initialize() noexcept

            //int first_file() noexcept

            //int next_file() noexcept

            //int search(std::uint32_t crc) noexcept

            //int search(std::string_view filename, bool partialpath) noexcept

            //int search(std::uint32_t crc, std::string_view filename, bool partialpath) noexcept

            //bool current_is_directory() const noexcept { return m_curr_is_dir; }

            //const std::string &current_name() const noexcept { return m_header.file_name; }

            //std::uint64_t current_uncompressed_length() const noexcept { return m_header.uncompressed_length; }

            //std::chrono::system_clock::time_point current_last_modified() const noexcept

            //std::uint32_t current_crc() const noexcept { return m_header.crc; }

            //std::error_condition decompress(void *buffer, std::size_t length) noexcept;

            //int search(std::uint32_t search_crc, std::string_view search_filename, bool matchcrc, bool matchname, bool partialpath) noexcept;

            //std::error_condition reopen() noexcept

            //static std::chrono::system_clock::time_point decode_dos_time(std::uint16_t date, std::uint16_t time) noexcept

            // ZIP file parsing
            //std::error_condition read_ecd() noexcept;
            //std::error_condition get_compressed_data_offset(std::uint64_t &offset) noexcept;

            // decompression interfaces
            //std::error_condition decompress_data_type_0(std::uint64_t offset, void *buffer, std::size_t length) noexcept;
            //std::error_condition decompress_data_type_8(std::uint64_t offset, void *buffer, std::size_t length) noexcept;
            //std::error_condition decompress_data_type_14(std::uint64_t offset, void *buffer, std::size_t length) noexcept;

            //struct file_header

            // contains extracted end of central directory information
            //struct ecd
        }


        //class zip_file_wrapper : public archive_file

        //class reader_base

        //class extra_field_reader : private reader_base

        //class local_file_header_reader : private reader_base

        //class central_dir_entry_reader : private reader_base

        //class ecd64_reader : private reader_base

        //class ecd64_locator_reader : private reader_base

        //class ecd_reader : private reader_base

        //class zip64_ext_info_reader : private reader_base

        //class utf8_path_reader : private reader_base

        //class ntfs_tag_reader : private reader_base

        //class ntfs_reader : private reader_base

        //class ntfs_times_reader : private reader_base

        //class general_flag_reader
    }


    //namespace std {
    //template <> struct is_error_condition_enum<util::archive_file::error> : public std::true_type { };
    //} // namespace std
}
