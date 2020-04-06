// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.plib
{
    public static class putil_global
    {
        // ----------------------------------------------------------------------------------------
        // string list
        // ----------------------------------------------------------------------------------------

        public static string [] psplit(string str, string onstr, bool ignore_empty = false)
        {
            return str.Split(onstr.ToCharArray(), ignore_empty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        //std::vector<pstring> psplit(const pstring &str, const std::vector<pstring> &onstrl);
        //std::vector<std::string> psplit_r(const std::string &stri,
        //        const std::string &token,
        //        const std::size_t maxsplit);
    }


    public static class penum_base
    {
        //static int from_string_int(const pstring &str, const pstring &x);
        //static std::string nthstr(int n, const pstring &str);


        //P_ENUM

        public static bool set_from_string(string s, out netlist.solver.matrix_type_e m_v)
        {
            //int f = from_string_int(strings(), s); \
            //if (f>=0) { m_v = static_cast<E>(f); return true; } else { return false; } \
            if (Enum.IsDefined(typeof(netlist.solver.matrix_type_e), s))
            {
                m_v = (netlist.solver.matrix_type_e)Enum.Parse(typeof(netlist.solver.matrix_type_e), s);
                return true;
            }
            else
            {
                m_v = default;
                return false;
            }
        }

        public static bool set_from_string(string s, out netlist.solver.matrix_sort_type_e m_v)
        {
            //int f = from_string_int(strings(), s); \
            //if (f>=0) { m_v = static_cast<E>(f); return true; } else { return false; } \
            if (Enum.IsDefined(typeof(netlist.solver.matrix_sort_type_e), s))
            {
                m_v = (netlist.solver.matrix_sort_type_e)Enum.Parse(typeof(netlist.solver.matrix_sort_type_e), s);
                return true;
            }
            else
            {
                m_v = default;
                return false;
            }
        }

        public static bool set_from_string(string s, out netlist.solver.matrix_fp_type_e m_v)
        {
            //int f = from_string_int(strings(), s); \
            //if (f>=0) { m_v = static_cast<E>(f); return true; } else { return false; } \
            if (Enum.IsDefined(typeof(netlist.solver.matrix_fp_type_e), s))
            {
                m_v = (netlist.solver.matrix_fp_type_e)Enum.Parse(typeof(netlist.solver.matrix_fp_type_e), s);
                return true;
            }
            else
            {
                m_v = default;
                return false;
            }
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

        std.vector<psource_t> m_collection;  //list_t m_collection;


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


        public delegate bool for_all_F(netlist.source_netlist_t source);

        //template <typename S, typename F>
        public bool for_all(for_all_F lambda)
        {
            foreach (var s in m_collection)
            {
                var source = s;  //auto source(dynamic_cast<S *>(s.get()));
                if (source != null)
                {
                    if (lambda((netlist.source_netlist_t)source))
                        return true;
                }
            }
            return false;
        }
    }
}
