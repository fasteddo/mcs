// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

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


        //unsigned line() const noexcept { return m_line; }
        //unsigned column() const noexcept { return m_col; }
        //pstring file_name() const noexcept { return m_file; }
        //pstring function_name() const noexcept { return m_func; }

        //source_location &operator ++() noexcept
        //{
        //    ++m_line;
        //    return *this;
        //}
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
    //class psource_str_t : public psource_t


    /// \brief Generic sources collection.
    ///
    class psource_collection_t
    {
        //using source_ptr = std::unique_ptr<psource_t>;
        //using list_t = std::vector<source_ptr>;


        psource_collection_t_list_t m_collection;


        psource_collection_t() { }

        //PCOPYASSIGNMOVE(psource_collection_t, delete)
        //virtual ~psource_collection_t() noexcept = default;


        //template <typename S, typename... Args>
        public void add_source(plib.psource_t args)  //void add_source(Args&&... args)
        {
            throw new emu_unimplemented();
#if false
            static_assert(std::is_base_of<psource_t, S>::value, "S must inherit from plib::psource_t");
#endif

            var src = args;  //auto src(std::make_unique<S>(std::forward<Args>(args)...));
            m_collection.push_back(src);
        }


        //template <typename S = psource_t>
        //istream_uptr get_stream(pstring name)
        //{
        //    for (auto &s : m_collection)
        //    {
        //        auto *source(dynamic_cast<S *>(s.get()));
        //        if (source)
        //        {
        //            auto strm = source->stream(name);
        //            if (!strm.empty())
        //                return strm;
        //        }
        //    }
        //    return istream_uptr();
        //}


        //template <typename S, typename F>
        public bool for_all<T>(Func<T, bool> lambda)  //bool for_all(F lambda)
            where T : psource_t
        {
            foreach (var s in m_collection)
            {
                var source = (T)s;  //auto *source(dynamic_cast<S *>(s.get()));
                if (source != null)
                {
                    if (lambda(source))
                        return true;
                }
            }

            return false;
        }
    }
} // namespace plib
