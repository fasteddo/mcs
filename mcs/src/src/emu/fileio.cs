// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Linq;

using emu_file_searchpath_vector = mame.std.vector<mame.std.pair<mame.path_iterator, string>>;  //using searchpath_vector = std::vector<std::pair<path_iterator, std::string> >;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using s64 = System.Int64;
using size_t = System.UInt64;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;


namespace mame
{
    // ======================> path_iterator
    // helper class for iterating over configured paths
    class path_iterator
    {
        // internal state
        string m_searchpath;
        int m_currentIdx;  // std::string::const_iterator m_current;
        char m_separator;
        bool m_is_first;


        // construction/destruction
        //-------------------------------------------------
        //  path_iterator - constructor
        //-------------------------------------------------
        public path_iterator(string searchpath)
        {
            m_searchpath = searchpath;
            m_currentIdx = 0;  //m_searchpath.cbegin();
            m_separator = ';'; // FIXME this should be a macro - UNIX prefers :
            m_is_first = true;
        }


        public path_iterator(path_iterator that)
        {
            m_searchpath = that.m_searchpath;
            m_currentIdx = that.m_currentIdx;  //, m_current(std::next(m_searchpath.cbegin(), std::distance(that.m_searchpath.cbegin(), that.m_current)))
            m_separator = that.m_separator;
            m_is_first = that.m_is_first;
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
            var sep = m_searchpath.IndexOf(m_separator, m_currentIdx);  //auto const sep(std::find(m_current, m_searchpath.cend(), m_separator));
            if (sep == -1) sep = m_searchpath.Length;
            buffer = m_searchpath.Substring(m_currentIdx, sep - m_currentIdx);  //buffer.assign(m_current, sep);
            m_currentIdx = sep;
            if (m_searchpath.Length != m_currentIdx)  //m_searchpath.cend() != m_current)
                ++m_currentIdx;

            // append the name if we have one
            if (!string.IsNullOrEmpty(name))
                util.path_append(ref buffer, name);

            // bump the index and return true
            m_is_first = false;
            return true;
        }


        // reset
        //-------------------------------------------------
        //  path_iterator::reset - let's go again
        //-------------------------------------------------
        public void reset()
        {
            m_currentIdx = 0; //m_searchpath.cbegin();
            m_is_first = true;
        }


        // helpers
        //template <typename T>
        //static std::string concatenate_paths(T &&paths)
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
        public osd.directory.entry next(string subdir = null)
        {
            // loop over potentially empty directories
            while (true)
            {
                // if no open directory, get the next path
                while (m_curdir == null)
                {
                    // if we fail to get anything more, we're done
                    if (!m_iterator.next(out m_pathbuffer, subdir))
                        return null;

                    // open the path
                    m_curdir = osdfile_global.m_osddirectory.open(m_pathbuffer);  //m_curdir = osd::directory::open(m_pathbuffer);
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
    public class emu_file : IDisposable
    {
        const int VERBOSE = 0;  //#define VERBOSE 1
        static void LOG_OUTPUT_FUNC_fileio(device_t device, string format, params object [] args) { g.osd_printf_verbose(format, args); }  //#define LOG_OUTPUT_FUNC osd_printf_verbose
        public static void LOG(string format, params object [] args) { g.LOG(VERBOSE, null, LOG_OUTPUT_FUNC_fileio, format, args); }


        enum empty_t { EMPTY }


        //using searchpath_vector = std::vector<std::pair<path_iterator, std::string> >;


        const u32 OPEN_FLAG_HAS_CRC  = 0x10000;


        // internal state
        string m_filename;                     // original filename provided
        string m_fullpath;                     // full filename
        util.core_file m_file;  //util::core_file::ptr    m_file;                 // core file pointer
        emu_file_searchpath_vector m_iterator;             // iterator for paths
        emu_file_searchpath_vector m_mediapaths;           // media-path iterator
        bool m_first;                // true if this is the start of iteration
        u32 m_crc;                          // file's CRC
        u32 m_openflags;                    // flags we used for the open
        util.hash_collection m_hashes = new util.hash_collection();                       // collection of hashes

        util.archive_file m_zipfile;  //std::unique_ptr<util::archive_file> m_zipfile;  // ZIP file pointer
        std.vector<u8> m_zipdata = new std.vector<u8>();  // ZIP file data
        u64 m_ziplength;                    // ZIP file length

        bool m_remove_on_close;              // flag: remove the file when closing
        int m_restrict_to_mediapath;    // flag: restrict to paths inside the media-path


        // file open/creation

        //-------------------------------------------------
        //  emu_file - constructor
        //-------------------------------------------------
        public emu_file(u32 openflags) : this(new path_iterator(""), openflags) { }

        //template <typename T>
        public emu_file(string searchpath, u32 openflags)  //emu_file(T &&searchpath, std::enable_if_t<std::is_constructible<path_iterator, T>::value, u32> openflags)
            : this(new path_iterator(searchpath), openflags)
        { }

        //template <typename T, typename U, typename V, typename... W>
        public emu_file(string searchpath1, IEnumerable<string> searchpath2, u32 openflags)  //emu_file(T &&searchpath, U &&x, V &&y, W &&... z)
            : this(0U, empty_t.EMPTY)
        {
            //m_iterator.reserve(sizeof...(W) + 1);
            //m_mediapaths.reserve(sizeof...(W) + 1);
            set_searchpaths(searchpath1, searchpath2, openflags);  //set_searchpaths(std::forward<T>(searchpath), std::forward<U>(x), std::forward<V>(y), std::forward<W>(z)...);
        }

        emu_file(u32 openflags, empty_t unused)
        {
            m_filename = "";
            m_fullpath = "";
            m_file = null;
            m_iterator = new emu_file_searchpath_vector();
            m_mediapaths = new emu_file_searchpath_vector();
            m_first = true;
            m_crc = 0;
            m_openflags = openflags;
            m_zipfile = null;
            m_ziplength = 0;
            m_remove_on_close = false;
            m_restrict_to_mediapath = 0;


            // sanity check the open flags
            if ((m_openflags & OPEN_FLAG_HAS_CRC) != 0 && (m_openflags & g.OPEN_FLAG_WRITE) != 0)
                throw new emu_fatalerror("Attempted to open a file for write with OPEN_FLAG_HAS_CRC");
        }

        emu_file(path_iterator searchpath, u32 openflags) : this(openflags, empty_t.EMPTY)
        {
            m_iterator.emplace_back(new std.pair<path_iterator, string>(new path_iterator(searchpath), ""));
            m_mediapaths.emplace_back(new std.pair<path_iterator, string>(new path_iterator(searchpath), ""));
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
            foreach (char scan in types)
                if (already_have.find_first_of(scan) == g.npos)
                    needed += scan;

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
                m_hashes.compute(new Pointer<u8>(m_zipdata), (uint32_t)m_zipdata.size(), needed);  //m_hashes.compute(&m_zipdata[0], m_zipdata.size(), needed.c_str());
                return m_hashes;
            }

            // read the data if we can
            MemoryU8 filedata = m_file.buffer();  //const u8 *filedata = (const u8 *)m_file->buffer();
            if (filedata == null)
                return m_hashes;

            // compute the hash
            m_hashes.compute(new Pointer<u8>(filedata), (UInt32)m_file.size(), needed);

            return m_hashes;
        }


        // setters
        //void remove_on_close() { m_remove_on_close = true; }
        //void set_openflags(u32 openflags) { assert(!m_file); m_openflags = openflags; }
        public void set_restrict_to_mediapath(int rtmp) { m_restrict_to_mediapath = rtmp; }


        // open/close

        //-------------------------------------------------
        //  open - open a file by searching paths
        //-------------------------------------------------

        public std.error_condition open(string name)
        {
            // remember the filename and CRC info
            m_filename = name;
            m_crc = 0;
            m_openflags &= ~OPEN_FLAG_HAS_CRC;

            // reset the iterator and open_next
            m_first = true;
            return open_next();
        }

        public std.error_condition open(string name, u32 crc)
        {
            // remember the filename and CRC info
            m_filename = name;
            m_crc = crc;
            m_openflags |= OPEN_FLAG_HAS_CRC;

            // reset the iterator and open_next
            m_first = true;
            return open_next();
        }


        //-------------------------------------------------
        //  open_next - open the next file that matches
        //  the filename by iterating over paths
        //-------------------------------------------------
        public std.error_condition open_next()
        {
            // if we're open from a previous attempt, close up now
            if (m_file != null)
                close();

            // loop over paths
            LOG(null, "emu_file: open next '{0}'\n", m_filename);
            std.error_condition filerr = std.errc.no_such_file_or_directory;
            while (filerr)
            {
                if (m_first)
                {
                    m_first = false;
                    for (int iIdx = 0; iIdx < m_iterator.Count; iIdx++)  //for (searchpath_vector::value_type &i : m_iterator)
                    {
                        var i = m_iterator[iIdx];
                        i.first.reset();

                        //if (!i.first.next(i.second))
                        string next;
                        var ret = i.first.next(out next);
                        m_iterator[iIdx] = new std.pair<path_iterator, string>(i.first, next);
                        if (!ret)
                            return filerr;
                    }
                }
                else
                {
                    int iIdx = 0;  //searchpath_vector::iterator i(m_iterator.begin());
                    while (iIdx != m_iterator.Count)  //while (i != m_iterator.end())
                    {
                        var i = m_iterator[iIdx];

                        //if (i->first.next(i->second))
                        string next;
                        var ret = i.first.next(out next);
                        m_iterator[iIdx] = new std.pair<path_iterator, string>(i.first, next);
                        i = m_iterator[iIdx];
                        if (ret)
                        {
                            LOG(null, "emu_file: next path {0} '{1}'\n", iIdx, i.second);  //LOG("emu_file: next path %d '%s'\n", std::distance(m_iterator.begin(), i), i->second);
                            for (int jIdx = 0; iIdx != jIdx; ++jIdx)  //for (searchpath_vector::iterator j = m_iterator.begin(); i != j; ++j)
                            {
                                var j = m_iterator[jIdx];
                                j.first.reset();

                                //j->first.next(j->second);
                                string jnext;
                                j.first.next(out jnext);
                                m_iterator[jIdx] = new std.pair<path_iterator, string>(j.first, jnext);
                            }
                            break;
                        }
                        ++iIdx;  //++i;
                    }
                    if (m_iterator.Count == iIdx)  //if (m_iterator.end() == i)
                        return filerr;
                }

                // build full path
                m_fullpath = m_fullpath.clear_();
                foreach (var path in m_iterator)  //for (searchpath_vector::value_type const &path : m_iterator)
                {
                    m_fullpath = m_fullpath.append_(path.second);
                    if (!m_fullpath.empty() && !util.is_directory_separator(m_fullpath.back()))
                        m_fullpath = m_fullpath.append_(g.PATH_SEPARATOR);
                }
                m_fullpath = m_fullpath.append_(m_filename);

                // attempt to open the file directly
                LOG(null, "emu_file: attempting to open '{0}' directly\n", m_fullpath);
                filerr = util.core_file.open(m_fullpath, m_openflags, out m_file);

                // if we're opening for read-only we have other options
                if (filerr && ((m_openflags & (g.OPEN_FLAG_READ | g.OPEN_FLAG_WRITE)) == g.OPEN_FLAG_READ))
                {
                    LOG(null, "emu_file: attempting to open '{0}' from archives\n", m_fullpath);
                    filerr = attempt_zipped();
                }
            }

            return filerr;
        }


        //-------------------------------------------------
        //  open_ram - open a "file" which is actually
        //  just an array of data in RAM
        //-------------------------------------------------
        public std.error_condition open_ram(MemoryU8 data, u32 length)  //const void *data, u32 length)
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
                osdfile_global.m_osdfile.remove(m_fullpath);

            m_remove_on_close = false;

            // reset our hashes and path as well
            m_hashes.reset();
            m_fullpath = "";
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
        public u32 read(Pointer<u8> buffer, u32 length)  //u32 read(void *buffer, u32 length)
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
        public u32 write(Pointer<u8> buffer, u32 length)  //u32 write(const void *buffer, u32 length)
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


        //template <typename T>
        //void set_searchpaths(T &&searchpath, u32 openflags)
        //{
        //    m_iterator.emplace_back(searchpath, "");
        //    m_mediapaths.emplace_back(std::forward<T>(searchpath), "");
        //    m_openflags = openflags;
        //}
        //template <typename T, typename U, typename V, typename... W>
        //void set_searchpaths(T &&searchpath, U &&x, V &&y, W &&... z)
        //{
        //    m_iterator.emplace_back(searchpath, "");
        //    m_mediapaths.emplace_back(std::forward<T>(searchpath), "");
        //    set_searchpaths(std::forward<U>(x), std::forward<V>(y), std::forward<W>(z)...);
        //}
        void set_searchpaths(string searchpath1, IEnumerable<string> searchpath2, u32 openflags)
        {
            List<string> searchpath = new List<string>();
            searchpath.Add(searchpath1);
            searchpath.AddRange(searchpath2);
            set_searchpaths(searchpath, openflags);
        }
        void set_searchpaths(IEnumerable<string> searchpath, u32 openflags)
        {
            foreach (var path in searchpath)
            {
                m_iterator.emplace_back(new std.pair<path_iterator, string>(new path_iterator(path), ""));
                m_mediapaths.emplace_back(new std.pair<path_iterator, string>(new path_iterator(path), ""));
            }
            m_openflags = openflags;
        }


        //-------------------------------------------------
        //  part_of_mediapath - checks if 'path' is part of
        //  any media path
        //-------------------------------------------------
        bool part_of_mediapath(string path)
        {
            if (m_restrict_to_mediapath == 0)
                return true;

            for (size_t i = 0U; (m_mediapaths.size() > i) && ((0 > m_restrict_to_mediapath) || (i < (size_t)m_restrict_to_mediapath)); i++)
            {
                m_mediapaths[i].first.reset();

                //if (!m_mediapaths[i].first.next(m_mediapaths[i].second))
                string next;
                var ret = m_mediapaths[i].first.next(out next);
                m_mediapaths[i] = new std.pair<path_iterator, string>(m_mediapaths[i].first, next);
                if (!ret)
                    return false;
            }

            string mediapath = "";
            while (true)
            {
                mediapath = "";  //mediapath.clear();
                for (size_t i = 0U; (m_mediapaths.size() > i) && ((0 > m_restrict_to_mediapath) || (i < (size_t)m_restrict_to_mediapath)); i++)
                {
                    mediapath = mediapath.append_(m_mediapaths[i].second);
                    if (!mediapath.empty() && !util.is_directory_separator(mediapath.back()))
                        mediapath = mediapath.append_(g.PATH_SEPARATOR);
                }

                if (path.compare(0, mediapath.size(), mediapath) == 0)
                {
                    LOG(null, "emu_file: path '{0}' matches media path '{1}'\n", path, mediapath);
                    return true;
                }

                {
                    size_t i = 0U;
                    while ((m_mediapaths.size() > i) && ((0 > m_restrict_to_mediapath) || (i < (size_t)m_restrict_to_mediapath)))
                    {
                        //if (m_mediapaths[i].first.next(m_mediapaths[i].second))
                        string next;
                        var ret = m_mediapaths[i].first.next(out next);
                        m_mediapaths[i] = new std.pair<path_iterator, string>(m_mediapaths[i].first, next);
                        if (ret)
                        {
                            for (size_t j = 0U; i != j; j++)
                            {
                                m_mediapaths[j].first.reset();

                                //m_mediapaths[j].first.next(m_mediapaths[j].second);
                                string next2;
                                var ret2 = m_mediapaths[j].first.next(out next2);
                                m_mediapaths[j] = new std.pair<path_iterator, string>(m_mediapaths[j].first, next2);
                            }
                            break;
                        }
                        i++;
                    }

                    if ((m_mediapaths.size() == i) || ((0 <= m_restrict_to_mediapath) && (i == (size_t)m_restrict_to_mediapath)))
                    {
                        LOG(null, "emu_file: path '{0}' not in media path\n", path);
                        return false;
                    }
                }
            }
        }


        //-------------------------------------------------
        //  compressed_file_ready - ensure our zip is ready
        //   loading if needed
        //-------------------------------------------------
        std.error_condition compressed_file_ready()
        {
            // load the ZIP file now if we haven't yet
            return m_zipfile != null ? load_zipped_file() : new std.error_condition();
        }


        // internal helpers

        //-------------------------------------------------
        //  attempt_zipped - attempt to open a ZIPped file
        //-------------------------------------------------
        delegate std.error_condition open_func(string filename, out util.archive_file result);  //typedef std::error_condition (*open_func)(std::string_view filename, util::archive_file::ptr &result);
        static readonly string [] suffixes = new string[] { ".zip", ".7z" };
        static readonly open_func [] open_funcs = new open_func[] { util.archive_file.open_zip, util.archive_file.open_7z };

        std.error_condition attempt_zipped()
        {
            //typedef std::error_condition (*open_func)(std::string_view filename, util::archive_file::ptr &result);
            //char const *const suffixes[] = { ".zip", ".7z" };
            //open_func const open_funcs[ARRAY_LENGTH(suffixes)] = { &util::archive_file::open_zip, &util::archive_file::open_7z };

            // loop over archive types
            string savepath = m_fullpath;
            string filename = "";
            for (unsigned i = 0; i < std.size(suffixes); i++, m_fullpath = savepath, filename = "")
            {
                // loop over directory parts up to the start of filename
                while (true)
                {
                    if (!part_of_mediapath(m_fullpath))
                        break;

                    // find the final path separator
                    //auto const dirsepiter(std::find_if(m_fullpath.rbegin(), m_fullpath.rend(), util::is_directory_separator));
                    //if (dirsepiter == m_fullpath.rend())
                    //    break;
                    //std::string::size_type const dirsep(std::distance(m_fullpath.begin(), dirsepiter.base()) - 1);
                    var dirsep = m_fullpath.find_last_of(g.PATH_SEPARATOR[0]);
                    if (dirsep == g.npos)
                        break;

                    // insert the part from the right of the separator into the head of the filename
                    if (!filename.empty())
                        filename = filename.insert_(0, "/");
                    filename = filename.insert_(0, m_fullpath.substr(dirsep + 1));

                    // remove this part of the filename and append an archive extension
                    m_fullpath = m_fullpath.Substring(0, (int)dirsep);
                    m_fullpath += suffixes[i];
                    LOG(null, "emu_file: looking for '{0}' in archive '{1}'\n", filename, m_fullpath);

                    // attempt to open the archive file
                    util.archive_file zip;
                    std.error_condition ziperr = open_funcs[i](m_fullpath, out zip);

                    // chop the archive suffix back off the filename before continuing
                    m_fullpath = m_fullpath.substr(0, dirsep);

                    // if we failed to open this file, continue scanning
                    if (ziperr)
                        continue;

                    int header = -1;

                    // see if we can find a file with the right name and (if available) CRC
                    if ((m_openflags & OPEN_FLAG_HAS_CRC) != 0)
                        header = zip.search(m_crc, filename, false);
                    if (header < 0 && ((m_openflags & OPEN_FLAG_HAS_CRC) != 0))
                        header = zip.search(m_crc, filename, true);

                    // if that failed, look for a file with the right CRC, but the wrong filename
                    if (header < 0 && ((m_openflags & OPEN_FLAG_HAS_CRC) != 0))
                        header = zip.search(m_crc);

                    // if that failed, look for a file with the right name;
                    // reporting a bad checksum is more helpful and less confusing than reporting "ROM not found"
                    if (header < 0)
                        header = zip.search(filename, false);
                    if (header < 0)
                        header = zip.search(filename, true);

                    // if we got it, read the data
                    if (header >= 0)
                    {
                        m_zipfile = zip;
                        m_ziplength = m_zipfile.current_uncompressed_length();

                        // build a hash with just the CRC
                        m_hashes.reset();
                        m_hashes.add_crc(m_zipfile.current_crc());
                        m_fullpath = savepath;
                        return (m_openflags & g.OPEN_FLAG_NO_PRELOAD) != 0 ? new std.error_condition() : load_zipped_file();
                    }

                    // close up the archive file and try the next level
                    zip.Dispose();
                    zip = null;  //.reset();
                }
            }

            return std.errc.no_such_file_or_directory;
        }


        //-------------------------------------------------
        //  load_zipped_file - load a ZIPped file
        //-------------------------------------------------
        std.error_condition load_zipped_file()
        {
            g.assert(m_file == null);
            g.assert(m_zipdata.empty());
            g.assert(m_zipfile != null);

            // allocate some memory
            m_zipdata.resize(m_ziplength);

            // read the data into our buffer and return
            var ziperr = m_zipfile.decompress(m_zipdata, (UInt32)m_zipdata.size());
            if (ziperr)
            {
                m_zipdata.clear();
                return ziperr;
            }

            // convert to RAM file
            std.error_condition filerr = util.core_file.open_ram(m_zipdata, m_zipdata.size(), m_openflags, out m_file);
            if (filerr)
            {
                m_zipdata.clear();
                return filerr;
            }

            // close out the ZIP file
            m_zipfile.Dispose();
            m_zipfile = null;
            return new std.error_condition();
        }
    }
}
