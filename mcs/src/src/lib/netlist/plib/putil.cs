// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using size_t = System.UInt32;


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
                result = ((result << 5) + result ) ^ (result >> (32 - 5)) ^ (size_t)buf[pIdx];  //result = ((result << 5) + result ) ^ (result >> (32 - 5)) ^ static_cast<std::size_t>(*p);
            return result;
        }


        public static string environment(string var, string default_val)
        {
            return (std.getenv(var.c_str()) == null) ? default_val
                : std.getenv(var.c_str());
        }
    }


    public static class container
    {
        //template <class C, class T>
        public static bool contains<C, T>(C con, T elem) where C : ICollection<T>  //bool contains(C &con, const T &elem)
        {
            return con.Contains(elem);  //return std::find(con.begin(), con.end(), elem) != con.end();
        }

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

        public abstract std.istream stream(string name);  //virtual stream_ptr stream(const pstring &name) = 0;
    }


    /// \brief Generic sources collection.
    ///
    /// \tparam TS base stream class. Default is psource_t
    ///
    //template <typename TS = psource_t>
    class psource_collection_t
    {
        //using source_type = plib::unique_ptr<TS>;
        //using list_t = std::vector<source_type>;

        std.vector<psource_t> m_collection = new std.vector<psource_t>();  //list_t m_collection;


        public psource_collection_t() { }


        //COPYASSIGNMOVE(psource_collection_t, delete)
        //virtual ~psource_collection_t() noexcept = default;


        public void add_source(psource_t src)  //void add_source(source_type &&src)
        {
            m_collection.push_back(src);
        }


        //template <typename S = TS>
        std.istream get_stream(string name)  //typename S::stream_ptr get_stream(pstring name)
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
