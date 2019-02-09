// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytes = mame.ListBase<System.Byte>;
using ListBytesPointer = mame.ListPointer<System.Byte>;
using s64 = System.Int64;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    // ======================> path_iterator
    // helper class for iterating over configured paths
    class path_iterator : global_object
    {
        // internal state
        string m_searchpath;
        int m_currentIdx;  // std::string::const_iterator m_current;
        bool m_is_first;


        // construction/destruction
        //-------------------------------------------------
        //  path_iterator - constructor
        //-------------------------------------------------
        public path_iterator(string searchpath)
        {
            m_searchpath = searchpath;
            m_currentIdx = 0;  //m_searchpath.cbegin();
            m_is_first = true;
        }


        // getters
        //-------------------------------------------------
        //  path_iterator::next - get the next entry in a
        //  multipath sequence
        //-------------------------------------------------
        public bool next(out string buffer, string name = null)
        {
            buffer = "";

            // if none left, return false to indicate we are done
            if (!m_is_first && (m_searchpath.Length == m_currentIdx))  //(m_searchpath.cend() == m_current))
                return false;

            // copy up to the next separator
            var sep = m_searchpath.IndexOf(';', m_currentIdx);  //var sep = std::find(m_current, m_searchpath.cend(), ';'); // FIXME this should be a macro - UNIX prefers :
            if (sep == -1) sep = m_searchpath.Length;
            buffer = m_searchpath.Substring(m_currentIdx, sep - m_currentIdx);  //.assign(m_current, sep);
            m_currentIdx = sep;
            if (m_searchpath.Length != m_currentIdx)  //m_searchpath.cend() != m_current)
                ++m_currentIdx;

            // append the name if we have one
            if (name != null)
            {
                // compute the full pathname
                if (!buffer.empty() && !is_directory_separator(buffer.back()))
                    buffer += PATH_SEPARATOR;

                buffer += name;
            }

            // bump the index and return true
            m_is_first = false;
            return true;
        }


        // reset
        //-------------------------------------------------
        //  path_iteratr::reset - let's go again
        //-------------------------------------------------
        public void reset()
        {
            m_currentIdx = 0; //m_searchpath.cbegin();
            m_is_first = true;
        }
    }


    // ======================> file_enumerator
    // iterate over all files in all paths specified in the searchpath
    class file_enumerator
    {
        // internal state
        path_iterator m_iterator;
        osd.directory m_curdir;
        string m_pathbuffer;


        // construction/destruction
        //-------------------------------------------------
        //  file_enumerator - constructor
        //-------------------------------------------------
        public file_enumerator(string searchpath)
        {
            m_iterator = new path_iterator(searchpath);
        }


        // iterator
        //-------------------------------------------------
        //  next - return information about the next file
        //  in the search path
        //-------------------------------------------------
        public osd.directory.entry next()
        {
            // loop over potentially empty directories
            while (true)
            {
                // if no open directory, get the next path
                while (m_curdir == null)
                {
                    // if we fail to get anything more, we're done
                    if (!m_iterator.next(out m_pathbuffer))
                        return null;

                    // open the path
                    m_curdir = osdcore_global.m_osddirectory.open(m_pathbuffer);
                }

                // get the next entry from the current directory
                osd.directory.entry result = m_curdir.read();
                if (result != null)
                    return result;

                // we're done; close this directory
                m_curdir = null; //.reset();
            }
        }
    }


    // ======================> emu_file
    public class emu_file : global_object, IDisposable
    {
        // from stdio.h
        public const int SEEK_CUR = 1;
        public const int SEEK_END = 2;
        public const int SEEK_SET = 0;


        const UInt32 OPEN_FLAG_HAS_CRC  = 0x10000;


        // internal state
        string m_filename = "";                     // original filename provided
        string m_fullpath = "";                     // full filename
        util.core_file m_file;                         // core file pointer
        path_iterator m_iterator;                     // iterator for paths
        path_iterator m_mediapaths;           // media-path iterator
        u32 m_crc;                          // file's CRC
        u32 m_openflags;                    // flags we used for the open
        util.hash_collection m_hashes = new util.hash_collection();                       // collection of hashes

        util.archive_file m_zipfile;  //std::unique_ptr<util::archive_file> m_zipfile;  // ZIP file pointer
        std.vector<u8> m_zipdata = new std.vector<u8>();  // ZIP file data
        u64 m_ziplength;                    // ZIP file length

        bool m_remove_on_close;              // flag: remove the file when closing
        bool m_restrict_to_mediapath;    // flag: restrict to paths inside the media-path


        // file open/creation

        //-------------------------------------------------
        //  emu_file - constructor
        //-------------------------------------------------
        public emu_file(u32 openflags)
        {
            m_file = null;
            m_iterator = new path_iterator("");
            m_mediapaths = new path_iterator("");
            m_crc = 0;
            m_openflags = openflags;
            m_zipfile = null;
            m_ziplength = 0;
            m_remove_on_close = false;
            m_restrict_to_mediapath = false;


            // sanity check the open flags
            if ((m_openflags & OPEN_FLAG_HAS_CRC) > 0 && (m_openflags & OPEN_FLAG_WRITE) > 0)
                throw new emu_fatalerror("Attempted to open a file for write with OPEN_FLAG_HAS_CRC");
        }

        public emu_file(string searchpath, u32 openflags)
        {
            m_file = null;
            m_iterator = new path_iterator(searchpath);
            m_mediapaths = new path_iterator(searchpath);
            m_crc = 0;
            m_openflags = openflags;
            m_zipfile = null;
            m_ziplength = 0;
            m_remove_on_close = false;
            m_restrict_to_mediapath = false;


            // sanity check the open flags
            if ((m_openflags & OPEN_FLAG_HAS_CRC) > 0 && (m_openflags & OPEN_FLAG_WRITE) > 0)
                throw new emu_fatalerror("Attempted to open a file for write with OPEN_FLAG_HAS_CRC");
        }

        ~emu_file()
        {
            //throw new emu_unimplemented();
#if false
            global.assert(m_isDisposed);  // can remove
#endif
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            // close in the standard way
            close();

            m_isDisposed = true;
        }


        // getters
        //operator core_file &();
        public util.core_file core_file_get()
        {
            // load the ZIP file now if we haven't yet
            if (compressed_file_ready())
                throw new emu_fatalerror("operator core_file & used on invalid file");

            // return the core file
            return m_file;
        }

        public bool is_open() { return m_file != null; }
        public string filename() { return m_filename; }
        public string fullpath() { return m_fullpath; }
        //UINT32 openflags() const { return m_openflags; }

        //-------------------------------------------------
        //  hash - returns the hash for a file
        //-------------------------------------------------
        public util.hash_collection hashes(string types)
        {
            // determine the hashes we already have
            string already_have = m_hashes.hash_types();

            // determine which hashes we need
            string needed = "";
            for (int scanIdx = 0; scanIdx < types.Length; scanIdx++)
                if (already_have.IndexOf(types[scanIdx], 0) == -1)
                    needed += types[scanIdx];

            // if we need nothing, skip it
            if (string.IsNullOrEmpty(needed))
                return m_hashes;

            // load the ZIP file if needed
            if (compressed_file_ready())
                return m_hashes;

            if (m_file == null)
                return m_hashes;

            // if we have ZIP data, just hash that directly
            if (!m_zipdata.empty())
            {
                m_hashes.compute(new ListBytesPointer(m_zipdata), (UInt32)m_zipdata.size(), needed.c_str());
                return m_hashes;
            }

            // read the data if we can
            ListBytes filedata = m_file.buffer();
            if (filedata == null)
                return m_hashes;

            // compute the hash
            m_hashes.compute(new ListBytesPointer(filedata), (UInt32)m_file.size(), needed);

            return m_hashes;
        }


        bool restrict_to_mediapath() { return m_restrict_to_mediapath; }

        //-------------------------------------------------
        //  part_of_mediapath - checks if 'path' is part of
        //  any media path
        //-------------------------------------------------
        bool part_of_mediapath(string path)
        {
            bool result = false;
            string mediapath;
            m_mediapaths.reset();
            while (m_mediapaths.next(out mediapath, null) && !result)
            {
                if (path.compare(mediapath.substr(0, mediapath.length())) != 0)
                    result = true;
            }

            return result;
        }


        // setters
        //void remove_on_close() { m_remove_on_close = true; }
        //void set_openflags(UINT32 openflags) { assert(m_file == NULL); m_openflags = openflags; }
        public void set_restrict_to_mediapath(bool rtmp = true) { m_restrict_to_mediapath = rtmp; }


        // open/close

        //-------------------------------------------------
        //  open - open a file by searching paths
        //-------------------------------------------------

        public osd_file.error open(string name)
        {
            // remember the filename and CRC info
            m_filename = name;
            m_crc = 0;
            m_openflags &= ~OPEN_FLAG_HAS_CRC;

            // reset the iterator and open_next
            m_iterator.reset();
            return open_next();
        }

        public osd_file.error open(string name1, string name2) { return open(name1 + name2); }
        public osd_file.error open(string name1, string name2, string name3) { return open(name1 + name2 + name3); }
        public osd_file.error open(string name1, string name2, string name3, string name4) { return open(name1 + name2 + name3 + name4); }

        public osd_file.error open(string name, UInt32 crc)
        {
            // remember the filename and CRC info
            m_filename = name;
            m_crc = crc;
            m_openflags |= OPEN_FLAG_HAS_CRC;

            // reset the iterator and open_next
            m_iterator.reset();
            return open_next();
        }

        public osd_file.error open(string name1, string name2, UInt32 crc) { return open(name1 + name2, crc); }
        public osd_file.error open(string name1, string name2, string name3, UInt32 crc) { return open(name1 + name2 + name3, crc); }
        public osd_file.error open(string name1, string name2, string name3, string name4, UInt32 crc) { return open(name1 + name2 + name3 + name4, crc); }

        //-------------------------------------------------
        //  open_next - open the next file that matches
        //  the filename by iterating over paths
        //-------------------------------------------------
        osd_file.error open_next()
        {
            // if we're open from a previous attempt, close up now
            if (m_file != null)
                close();

            // loop over paths
            osd_file.error filerr = osd_file.error.NOT_FOUND;

            while (m_iterator.next(out m_fullpath, m_filename))
            {
                // attempt to open the file directly
                filerr = util.core_file.open(m_fullpath, m_openflags, out m_file);
                if (filerr == osd_file.error.NONE)
                    break;

                // if we're opening for read-only we have other options
                if ((m_openflags & (OPEN_FLAG_READ | OPEN_FLAG_WRITE)) == OPEN_FLAG_READ)
                {
                    filerr = attempt_zipped();
                    if (filerr == osd_file.error.NONE)
                        break;
                }
            }

            return filerr;
        }


        //-------------------------------------------------
        //  open_ram - open a "file" which is actually
        //  just an array of data in RAM
        //-------------------------------------------------
        public osd_file.error open_ram(ListBytes data, u32 length)  //const void *data, u32 length)
        {
            // set a fake filename and CRC
            m_filename = "RAM";
            m_crc = 0;

            // use the core_file's built-in RAM support
            return util.core_file.open_ram(data, length, m_openflags, out m_file);
        }


        //-------------------------------------------------
        //  close - close a file and free all data; also
        //  remove the file if requested
        //-------------------------------------------------
        public void close()
        {
            // close files and free memory
            if (m_zipfile != null)
                m_zipfile.Dispose();
            m_zipfile = null;
            m_file = null;

            m_zipdata.clear();

            if (m_remove_on_close)
                osdcore_global.m_osdfile.remove(m_fullpath);

            m_remove_on_close = false;

            // reset our hashes and path as well
            m_hashes.reset();
            m_fullpath = "";
        }


        // control
        public osd_file.error compress(int level)
        {
            return m_file.compress(level);
        }


        // position

        //-------------------------------------------------
        //  seek - seek within a file
        //-------------------------------------------------
        public int seek(s64 offset, int whence)
        {
            // load the ZIP file now if we haven't yet
            if (compressed_file_ready())
                return 1;

            // seek if we can
            if (m_file != null)
                return m_file.seek(offset, whence);

            return 1;
        }


        //-------------------------------------------------
        //  tell - return the current file position
        //-------------------------------------------------
        public u64 tell()
        {
            // load the ZIP file now if we haven't yet
            if (compressed_file_ready())
                return 0;

            // tell if we can
            if (m_file != null)
                return m_file.tell();

            return 0;
        }


        //bool eof();

        //-------------------------------------------------
        //  size - returns the size of a file
        //-------------------------------------------------
        public u64 size()
        {
            // use the ZIP length if present
            if (m_zipfile != null)
                return m_ziplength;

            // return length if we can
            if (m_file != null)
                return m_file.size();

            return 0;
        }


        // reading

        //-------------------------------------------------
        //  read - read from a file
        //-------------------------------------------------
        public u32 read(ListBytesPointer buffer, u32 length)  //void *buffer, UINT32 length)
        {
            // load the ZIP file now if we haven't yet
            if (compressed_file_ready())
                return 0;

            // read the data if we can
            if (m_file != null)
                return m_file.read(buffer, length);

            return 0;
        }


        //int getc();
        //int ungetc(int c);


        //-------------------------------------------------
        //  gets - read a line from a text file
        //-------------------------------------------------
        public string gets(out string s, int n)  // char *gets(char *s, int n);
        {
            s = "";

            // load the ZIP file now if we haven't yet
            if (compressed_file_ready())
                return null;

            // read the data if we can
            if (m_file != null)
                return m_file.gets(out s, n);

            return null;
        }


        // writing

        //-------------------------------------------------
        //  write - write to a file
        //-------------------------------------------------
        public u32 write(ListBytesPointer buffer, u32 length)  //const void *buffer, u32 length)
        {
            // write the data if we can
            if (m_file != null)
                return m_file.write(buffer, length);

            return 0;
        }

        //-------------------------------------------------
        //  puts - write a line to a text file
        //-------------------------------------------------
        public int puts(string s)
        {
            // write the data if we can
            if (m_file != null)
                return m_file.puts(s);

            return 0;
        }


        //-------------------------------------------------
        //  vfprintf - vfprintf to a text file
        //-------------------------------------------------
        //int emu_file::vprintf(util::format_argument_pack<std::ostream> const &args)
        public int vprintf(string args)
        {
            // write the data if we can
            return m_file != null ? m_file.vprintf(args) : 0;
        }
        //template <typename Format, typename... Params> int printf(Format &&fmt, Params &&...args) { return vprintf(util::make_format_argument_pack(std::forward<Format>(fmt), std::forward<Params>(args)...)); }
        public int printf(string format, params object [] args) { return vprintf(string.Format(format, args)); }


        // buffers
        //-------------------------------------------------
        //  flush - flush file buffers
        //-------------------------------------------------
        public void flush()
        {
            // flush the buffers if we can
            if (m_file != null)
                m_file.flush();
        }


        //-------------------------------------------------
        //  compressed_file_ready - ensure our zip is ready
        //   loading if needed
        //-------------------------------------------------
        bool compressed_file_ready()
        {
            // load the ZIP file now if we haven't yet
            if (m_zipfile != null && (load_zipped_file() != osd_file.error.NONE))
                return true;

            return false;
        }


        // internal helpers

        //-------------------------------------------------
        //  attempt_zipped - attempt to open a ZIPped file
        //-------------------------------------------------
        //typedef util::archive_file::error (*open_func)(const std::string &filename, util::archive_file::ptr &result);
        delegate util.archive_file.error open_func(string filename, out util.archive_file result);

        osd_file.error attempt_zipped()
        {
            string [] suffixes = new string[] { ".zip", ".7z" };  //char const *const suffixes[] = { ".zip", ".7z" };
            open_func [] open_funcs = new open_func[] { util.archive_file.open_zip, util.archive_file.open_7z };

            // loop over archive types
            string savepath = m_fullpath;
            string filename = "";
            for (int i = 0; i < suffixes.Length; i++, m_fullpath = savepath, filename = "")
            {
                // loop over directory parts up to the start of filename
                while (true)
                {
                    // find the final path separator
                    var dirsep = m_fullpath.find_last_of(PATH_SEPARATOR[0]);
                    if (dirsep == -1)
                        break;

                    if (restrict_to_mediapath() && !part_of_mediapath(m_fullpath))
                        break;

                    // insert the part from the right of the separator into the head of the filename
                    if (!filename.empty()) filename = filename.Insert(0, "/");
                    filename = filename.Insert(0, m_fullpath.substr(dirsep + 1));

                    // remove this part of the filename and append an archive extension
                    m_fullpath = m_fullpath.Substring(0, dirsep);
                    m_fullpath += suffixes[i];

                    // attempt to open the archive file
                    util.archive_file zip;
                    util.archive_file.error ziperr = open_funcs[i](m_fullpath, out zip);

                    // chop the archive suffix back off the filename before continuing
                    m_fullpath = m_fullpath.substr(0, dirsep);

                    // if we failed to open this file, continue scanning
                    if (ziperr != util.archive_file.error.NONE)
                        continue;

                    int header = -1;

                    // see if we can find a file with the right name and (if available) CRC
                    if ((m_openflags & OPEN_FLAG_HAS_CRC) != 0) header = zip.search(m_crc, filename, false);
                    if (header < 0 && ((m_openflags & OPEN_FLAG_HAS_CRC) != 0)) header = zip.search(m_crc, filename, true);

                    // if that failed, look for a file with the right CRC, but the wrong filename
                    if (header < 0 && ((m_openflags & OPEN_FLAG_HAS_CRC) != 0)) header = zip.search(m_crc);

                    // if that failed, look for a file with the right name;
                    // reporting a bad checksum is more helpful and less confusing than reporting "ROM not found"
                    if (header < 0) header = zip.search(filename, false);
                    if (header < 0) header = zip.search(filename, true);

                    // if we got it, read the data
                    if (header >= 0)
                    {
                        m_zipfile = zip;
                        m_ziplength = m_zipfile.current_uncompressed_length();

                        // build a hash with just the CRC
                        m_hashes.reset();
                        m_hashes.add_crc(m_zipfile.current_crc());
                        return ((m_openflags & OPEN_FLAG_NO_PRELOAD) != 0) ? osd_file.error.NONE : load_zipped_file();
                    }

                    // close up the archive file and try the next level
                    zip.Dispose();
                    zip = null;  //.reset();
                }
            }

            return osd_file.error.NOT_FOUND;
        }


        //-------------------------------------------------
        //  load_zipped_file - load a ZIPped file
        //-------------------------------------------------
        osd_file.error load_zipped_file()
        {
            assert(m_file == null);
            assert(m_zipdata.empty());
            assert(m_zipfile != null);

            // allocate some memory
            m_zipdata.resize((int)m_ziplength);

            // read the data into our buffer and return
            var ziperr = m_zipfile.decompress(m_zipdata, (UInt32)m_zipdata.size());
            if (ziperr != util.archive_file.error.NONE)
            {
                m_zipdata.clear();
                return osd_file.error.FAILURE;
            }

            // convert to RAM file
            osd_file.error filerr = util.core_file.open_ram(m_zipdata, (UInt32)m_zipdata.size(), m_openflags, out m_file);
            if (filerr != osd_file.error.NONE)
            {
                m_zipdata.clear();
                return osd_file.error.FAILURE;
            }

            // close out the ZIP file
            m_zipfile.Dispose();
            m_zipfile = null;
            return osd_file.error.NONE;
        }
    }
}
