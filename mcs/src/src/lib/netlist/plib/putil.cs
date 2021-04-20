// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using psource_t_stream_ptr = mame.std.istream;  //using stream_ptr = plib::unique_ptr<std::istream>;
using size_t = System.UInt32;
using unsigned = System.UInt32;


namespace mame.plib
{
    public static class putil_global
    {
        // ----------------------------------------------------------------------------------------
        // string list
        // ----------------------------------------------------------------------------------------

        public static std.vector<string> psplit(string str, string onstr, bool ignore_empty = false)
        {
            return new std.vector<string>(str.Split(new string [] { onstr }, ignore_empty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None));
        }


        // this version keeps the delimeters, and stores them in a sep entry in the vector
        public static std.vector<string> psplit(string str, std.vector<string> onstrl)
        {
            string col = "";
            std.vector<string> ret = new std.vector<string>();

            var i = 0;  //auto i = str.begin();
            while (i != str.Length)  //while (i != str.end())
            {
                var p = -1;  //auto p = static_cast<std::size_t>(-1);
                for (int j = 0; j < onstrl.size(); j++)
                {
                    if (onstrl[j] == str.Substring(i, onstrl[j].Length))  //if (std::equal(onstrl[j].begin(), onstrl[j].end(), i))
                    {
                        p = j;
                        break;
                    }
                }

                if (p != -1)
                {
                    if (col != "")
                        ret.push_back(col);

                    col = "";
                    ret.push_back(onstrl[p]);
                    i += onstrl[p].length();  //i = std::next(i, static_cast<pstring::difference_type>(onstrl[p].length()));
                }
                else
                {
                    char c = str[i];  //pstring::value_type c = *i;
                    col += c;
                    i++;
                }
            }

            if (col != "")
                ret.push_back(col);

            return ret;
        }


        //std::vector<pstring> psplit(const pstring &str, const std::vector<pstring> &onstrl);
        //std::vector<std::string> psplit_r(const std::string &stri,
        //        const std::string &token,
        //        const std::size_t maxsplit);


        // ----------------------------------------------------------------------------------------
        // simple hash
        // ----------------------------------------------------------------------------------------
        //template <typename T>
        public static UInt64 hash(string buf, size_t size)  //std::size_t hash(const T *buf, std::size_t size)
        {
            UInt64 result = 5381;
            for (int pIdx = 0; pIdx != size; pIdx++)  //for (const T* p = buf; p != buf + size; p++)
                result = ((result << 5) + result ) ^ (result >> (32 - 5)) ^ (size_t)buf[pIdx];  //result = ((result << 5) + result ) ^ (result >> (32 - 5)) ^ narrow_cast<std::size_t>(*p); // NOLINT
            return result;
        }


        public static string environment(string var, string default_val)
        {
            return (std.getenv(var.c_str()) == null) ? default_val
                : std.getenv(var.c_str());
        }


        // FIXME:: __FUNCTION__ may be not be supported by all compilers.

        public static source_location PSOURCELOC() { return new source_location("__FILE__", 0); }  //#define PSOURCELOC() plib::source_location(__FILE__, __LINE__)
    }


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
            m_file = "unknown";
            m_func = m_file;
            m_line = 0;
            m_col = 0;
        }

        public source_location(string file, unsigned line)
        {
            m_file = file;
            m_func = "unknown";
            m_line = line;
            m_col = 0;
        }

        public source_location(string file, string func, unsigned line)
        {
            m_file = file;
            m_func = func;
            m_line = line;
            m_col = 0;
        }


        //PCOPYASSIGNMOVE(source_location, default)

        //~source_location() = default;

        //unsigned line() const noexcept { return m_line; }
        //unsigned column() const noexcept { return m_col; }
        //pstring file_name() const noexcept { return m_file; }
        //pstring function_name() const noexcept { return m_func; }

        //source_location &operator ++() noexcept
    }


    /// \brief Base source class.
    ///
    /// Pure virtual class all other source implementations are based on.
    /// Sources provide an abstraction to read input from a variety of
    /// sources, e.g. files, memory, remote locations.
    ///
    //class psource_t


    /// \brief Generic string source.
    ///
    /// Will return the given string when name matches.
    /// Is used in preprocessor code to eliminate inclusion of certain files.
    ///
    //class psource_str_t : public psource_t


    /// \brief Generic sources collection.
    ///
    /// \tparam ARENA memory arena, defaults to aligned_arena
    ///
    //template <typename ARENA = aligned_arena>
    //class psource_collection_t


    //namespace util
    //{
    //    pstring basename(const pstring &filename, const pstring &suffix = "");
    //    pstring path(const pstring &filename);
    //    bool    exists(const pstring &filename);
    //    pstring buildpath(std::initializer_list<pstring> list );
    //    pstring environment(const pstring &var, const pstring &default_val);
    //} // namespace util


    public static class container
    {
        //template <class C, class T>
        public static bool contains<C, T>(C con, T elem) where C : ICollection<T>  //bool contains(C &con, const T &elem)
        {
            return con.Contains(elem);  //return std::find(con.begin(), con.end(), elem) != con.end();
        }


        //static constexpr const std::size_t npos = static_cast<std::size_t>(-1);

        //template <class C>
        //std::size_t indexof(C &con, const typename C::value_type &elem)

        //template <class C>
        //void insert_at(C &con, const std::size_t index, const typename C::value_type &elem)


        //template <class C>
        public static void remove<C, T>(C con, T elem) where C : ICollection<T>  //void remove(C &con, const typename C::value_type &elem)
        {
            con.Remove(elem);  //con.erase(std::remove(con.begin(), con.end(), elem), con.end());
        }
    }


    public static class penum_base
    {
        //static int from_string_int(const pstring &str, const pstring &x);
        //static std::string nthstr(int n, const pstring &str);


        //P_ENUM

        public static bool set_from_string(string s, out netlist.solver.matrix_type_e m_v)
        {
            //int f = from_string_int(strings(), s); \
            //if (f>=0) { m_v = static_cast<E>(f); return true; } \
            //return false;\
            if (Enum.IsDefined(typeof(netlist.solver.matrix_type_e), s))
            {
                m_v = (netlist.solver.matrix_type_e)Enum.Parse(typeof(netlist.solver.matrix_type_e), s);
                return true;
            }

            m_v = default;
            return false;
        }

        public static bool set_from_string(string s, out netlist.solver.matrix_sort_type_e m_v)
        {
            //int f = from_string_int(strings(), s); \
            //if (f>=0) { m_v = static_cast<E>(f); return true; } \
            //return false;\
            if (Enum.IsDefined(typeof(netlist.solver.matrix_sort_type_e), s))
            {
                m_v = (netlist.solver.matrix_sort_type_e)Enum.Parse(typeof(netlist.solver.matrix_sort_type_e), s);
                return true;
            }

            m_v = default;
            return false;
        }

        public static bool set_from_string(string s, out netlist.solver.matrix_fp_type_e m_v)
        {
            //int f = from_string_int(strings(), s); \
            //if (f>=0) { m_v = static_cast<E>(f); return true; } \
            //return false;\
            if (Enum.IsDefined(typeof(netlist.solver.matrix_fp_type_e), s))
            {
                m_v = (netlist.solver.matrix_fp_type_e)Enum.Parse(typeof(netlist.solver.matrix_fp_type_e), s);
                return true;
            }

            m_v = default;
            return false;
        }

        public static bool set_from_string(string s, out netlist.family_type m_v)
        {
            //int f = from_string_int(strings(), s); \
            //if (f>=0) { m_v = static_cast<E>(f); return true; } \
            //return false;\
            if (Enum.IsDefined(typeof(netlist.family_type), s))
            {
                m_v = (netlist.family_type)Enum.Parse(typeof(netlist.family_type), s);
                return true;
            }

            m_v = default;
            return false;
        }
    }


    /// \brief Base source class.
    ///
    /// Pure virtual class all other source implementations are based on.
    /// Sources provide an abstraction to read input from a variety of
    /// sources, e.g. files, memory, remote locations.
    ///
    public abstract class psource_t
    {
        //using stream_ptr = plib::unique_ptr<std::istream>;

        protected psource_t() { }

        //COPYASSIGNMOVE(psource_t, delete)

        //virtual ~psource_t() noexcept = default;

        public abstract psource_t_stream_ptr stream(string name);  //virtual stream_ptr stream(const pstring &name) = 0;
    }


    /// \brief Generic sources collection.
    ///
    /// \tparam ARENA memory arena, defaults to aligned_arena
    ///
    //template <typename ARENA = aligned_arena>
    class psource_collection_t
    {
        //using source_type = std::unique_ptr<psource_t>;
        //using list_t = std::vector<source_type>;

        std.vector<psource_t> m_collection = new std.vector<psource_t>();  //list_t m_collection;


        public psource_collection_t() { }


        //COPYASSIGNMOVE(psource_collection_t, delete)
        //virtual ~psource_collection_t() noexcept = default;


        public void add_source(psource_t src)  //void add_source(source_type &&src)
        {
            m_collection.push_back(src);
        }


        //template <typename S = psource_t>
        std.istream get_stream(string name)  //typename psource_t::stream_ptr get_stream(pstring name)
        {
            foreach (var s in m_collection)
            {
                var source = s;  //auto source(dynamic_cast<S *>(s.get()));
                if (source != null)
                {
                    var strm = source.stream(name);
                    if (strm != null)
                        return strm;
                }
            }
            return null;  //return typename S::stream_ptr(nullptr);
        }


        //template <typename S, typename F>
        public bool for_all(Func<netlist.source_netlist_t, bool> lambda)  //bool for_all(F lambda)
        {
            foreach (var s in m_collection)
            {
                bool isSource = s is netlist.source_netlist_t;  //auto source(dynamic_cast<S *>(s.get()));
                if (isSource)  //if (source)
                {
                    if (lambda((netlist.source_netlist_t)s))
                        return true;
                }
            }
            return false;
        }
    }
}
