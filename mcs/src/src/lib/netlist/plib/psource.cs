// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.IO;
using System.Text;

using psource_collection_t_list_t = mame.std.vector<mame.plib.psource_t>;  //using list_t = std::vector<source_ptr>;
using unsigned = System.UInt32;


namespace mame.plib
{
    //#define PSOURCELOC() plib::source_location(__FILE__, __LINE__)


    /// \brief Source code locations.
    ///
    /// The c++20 draft for source locations is based on const char * strings.
    /// It is thus only suitable for c++ source code and not for programmatic
    /// parsing of files. This class is a replacement for dynamic use cases.
    ///
    public class source_location
    {
        string m_file;
        string m_func;
        unsigned m_line;
        unsigned m_col;


        public source_location()
        {
            m_file = "unknown"; m_func = m_file; m_line = 0; m_col = 0;
        }

        public source_location(string file, unsigned line)
        {
            m_file = file; m_func = "unknown"; m_line = line; m_col = 0;
        }

        public source_location(string file, string func, unsigned line)
        {
            m_file = file; m_func = func; m_line = line; m_col = 0;
        }

        //PCOPYASSIGNMOVE(source_location, default)

        //~source_location() = default;


        public unsigned line() { return m_line; }
        //unsigned column() const noexcept { return m_col; }
        public string file_name() { return m_file; }
        //pstring function_name() const noexcept { return m_func; }

        //source_location &operator ++() noexcept
        //{
        //    ++m_line;
        //    return *this;
        //}
        public void inc() { ++m_line; }
    }


    /// \brief Base source class.
    ///
    /// Pure virtual class all other source implementations are based on.
    /// Sources provide an abstraction to read input from a variety of
    /// sources, e.g. files, memory, remote locations.
    ///
    public abstract class psource_t
    {
        //psource_t() noexcept = default;

        //PCOPYASSIGNMOVE(psource_t, delete)

        //virtual ~psource_t() noexcept = default;

        public abstract istream_uptr stream(string name);  //virtual istream_uptr stream(const pstring &name) = 0;
    }


    /// \brief Generic string source.
    ///
    /// Will return the given string when name matches.
    /// Is used in preprocessor code to eliminate inclusion of certain files.
    ///
    class psource_str_t : psource_t
    {
        string m_name;
        string m_str;


        public psource_str_t(string name, string str)
            : base()
        {
            m_name = name;
            m_str = str;
        }

        //PCOPYASSIGNMOVE(psource_str_t, delete)
        //~psource_str_t() noexcept override = default;

        public override istream_uptr stream(string name)
        {
            if (name == m_name)
                return new istream_uptr(new MemoryStream(Encoding.ASCII.GetBytes(m_str)), name);  //return istream_uptr(std::make_unique<std::stringstream>(putf8string(m_str)), name);

            return new istream_uptr();
        }
    }


    /// \brief Generic sources collection.
    ///
    class psource_collection_t
    {
        //using source_ptr = std::unique_ptr<psource_t>;
        //using list_t = std::vector<source_ptr>;


        psource_collection_t_list_t m_collection = new psource_collection_t_list_t();


        public psource_collection_t() { }

        //PCOPYASSIGNMOVE(psource_collection_t, delete)
        //virtual ~psource_collection_t() noexcept = default;


        //template <typename S, typename... Args>
        public void add_source(plib.psource_t args)  //void add_source(Args&&... args)
        {
            //static_assert(std::is_base_of<psource_t, S>::value, "S must inherit from plib::psource_t");

            var src = args;  //auto src(std::make_unique<S>(std::forward<Args>(args)...));
            m_collection.push_back(src);
        }


        //template <typename S = psource_t>
        public istream_uptr get_stream(string name)
        {
            foreach (var s in m_collection)
            {
                var source = (psource_t)s;  //auto *source(dynamic_cast<S *>(s.get()));
                if (source != null)
                {
                    var strm = source.stream(name);
                    if (!strm.empty())
                        return strm;
                }
            }

            return new istream_uptr();
        }


        //template <typename S, typename F>
        public bool for_all<T>(Func<T, bool> lambda)  //bool for_all(F lambda)
            where T : psource_t
        {
            foreach (var s in m_collection)
            {
                var source = s is T s_t ? s_t : default;  //auto *source(dynamic_cast<S *>(s.get()));
                if (source != default)
                {
                    if (lambda(source))
                        return true;
                }
            }

            return false;
        }
    }
} // namespace plib
