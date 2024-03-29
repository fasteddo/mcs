// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using psource_t_stream_ptr = mame.std.istream;  //using stream_ptr = plib::unique_ptr<std::istream>;
using size_t = System.UInt64;
using unsigned = System.UInt32;


namespace mame.plib
{
    public static class putil_global
    {
        // ----------------------------------------------------------------------------------------
        // simple hash
        // ----------------------------------------------------------------------------------------
        //template <typename V, typename T>
        public static size_t hash(string buf, size_t size)  //V hash(const T *buf, std::size_t size)
        {
            size_t result = 5381;  //V result = 5381; // NOLINT
            for (size_t pIdx = 0; pIdx != size; pIdx++)  //for (const T* p = buf; p != buf + size; p++)
                result = ((result << 5) + result ) ^ (result >> (32 - 5)) ^ (size_t)buf[(int)pIdx];  //result = ((result << 5) + result ) ^ (result >> (32 - 5)) ^ narrow_cast<std::size_t>(*p); // NOLINT
            return result;
        }


        public static string environment(string var, string default_val)
        {
            return (std.getenv(var) == null) ? default_val
                : std.getenv(var);
        }


        // FIXME:: __FUNCTION__ may be not be supported by all compilers.

        public static source_location PSOURCELOC() { return new source_location("__FILE__", 0); }  //#define PSOURCELOC() plib::source_location(__FILE__, __LINE__)
    }


    static class util  //namespace util
    {
        //pstring basename(const pstring &filename, const pstring &suffix = "");

        public static string path(string filename)
        {
            //auto p=find_last_of(filename, pstring(1, PATH_SEP));
            //if (p == pstring::npos)
            //    return "";
            //if (p == 0) // root case
            //    return filename.substr(0, 1);
            //
            //return filename.substr(0, p);

            return Path.GetDirectoryName(filename);
        }

        //bool    exists(const pstring &filename);

        public static string build_path(params string [] list)  //pstring build_path(std::initializer_list<pstring> list );
        {
            //pstring ret = "";
            //for( const auto &elem : list )
            //{
            //    if (ret.empty())
            //        ret = elem;
            //    else
            //        ret += (PATH_SEP + elem);
            //}
            //return ret;

            return Path.Combine(list);
        }

        //pstring environment(const pstring &var, const pstring &default_val);
    }


    public static class container
    {
        //template <class C, class T>
        public static bool contains<C, T>(C con, T elem) where C : ICollection<T>  //bool contains(C &con, const T &elem)
        {
            return con.Contains(elem);  //return std::find(con.begin(), con.end(), elem) != con.end();
        }


        //static constexpr const std::size_t npos = static_cast<std::size_t>(-1);

        //template <class C>
        //std::size_t index_of(C &con, const typename C::value_type &elem)

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


    //template <typename F>
    //struct functor_guard
}
