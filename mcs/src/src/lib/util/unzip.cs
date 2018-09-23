// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytes = mame.ListBase<System.Byte>;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame.util
{
    // describes an open archive file
    public abstract class archive_file
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
            //throw new emu_unimplemented();

            result = null;
            return error.UNSUPPORTED;
        }

        // close an archive file (may actually be left open due to caching)
        //virtual ~archive_file();

        // clear out all open files from the cache
        public static void cache_clear()
        {
            //throw new emu_unimplemented();
        }


        /* ----- contained file access ----- */

        // iterating over files - returns negative on reaching end
        //virtual int first_file() = 0;
        //virtual int next_file() = 0;


        // find a file index by crc, filename or both - returns non-negative on match
        public abstract int search(uint32_t crc);
        public abstract int search(string filename, bool partialpath);
        public abstract int search(uint32_t crc, string filename, bool partialpath);


        // information on most recently found file
        //virtual bool current_is_directory() const = 0;
        //virtual const std::string &current_name() const = 0;
        public abstract uint64_t current_uncompressed_length();
        //virtual std::chrono::system_clock::time_point current_last_modified() const = 0;
        public abstract uint32_t current_crc();


        // decompress the most recently found file in the ZIP
        public abstract error decompress(ListBytes buffer, uint32_t length);  //void *buffer, std::uint32_t length)
    }
}
