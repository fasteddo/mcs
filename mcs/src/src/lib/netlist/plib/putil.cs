// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.plib
{
    public class putil_global
    {
        //template<typename T, bool CLOCALE, typename S>
        //T pstonum(const S &arg)
        public static bool pstonum_bool(string arg)
        {
            // Convert.ToBoolean() only matches "True" or "False"
            if      (arg == "1" || arg == "true") return true;
            else if (arg == "0" || arg == "false") return false;
            else return Convert.ToBoolean(arg);
        }
        public static int pstonum_int(string arg) { return Convert.ToInt32(arg); }
        public static float pstonum_float(string arg) { return Convert.ToSingle(arg); }
        public static double pstonum_double(string arg) { return Convert.ToDouble(arg); }


        //template<typename R, bool CLOCALE, typename T>
        //R pstonum_ne(const T &str, bool &err) noexcept
        public static bool pstonum_ne_bool(bool CLOCALE, string arg, out bool err)
        {
            // bool.TryParse() only matches "True" or "False"
            err = false;
            if      (arg == "1" || arg == "true") return true;
            else if (arg == "0" || arg == "false") return false;
            else
            {
                bool ret;
                err = !bool.TryParse(arg, out ret);
                return ret;
            }
        }
        public static int pstonum_ne_int(bool CLOCALE, string arg, out bool err) { int ret; err = !int.TryParse(arg, out ret); return ret; }
        public static UInt32 pstonum_ne_unsigned(bool CLOCALE, string arg, out bool err) { UInt32 ret; err = !UInt32.TryParse(arg, out ret); return ret; }
        public static float pstonum_ne_float(bool CLOCALE, string arg, out bool err) { float ret; err = !float.TryParse(arg, out ret); return ret; }
        public static double pstonum_ne_double(bool CLOCALE, string arg, out bool err) { double ret; err = !double.TryParse(arg, out ret); return ret; }
    }


    public static class penum_base
    {
        //static int from_string_int(const pstring &str, const pstring &x);
        //static std::string nthstr(int n, const pstring &str);


        //P_ENUM

        public static bool set_from_string(string s, out netlist.devices.matrix_type_e m_v)
        {
            //int f = from_string_int(strings(), s); \
            //if (f>=0) { m_v = static_cast<E>(f); return true; } else { return false; } \
            if (Enum.IsDefined(typeof(netlist.devices.matrix_type_e), s))
            {
                m_v = (netlist.devices.matrix_type_e)Enum.Parse(typeof(netlist.devices.matrix_type_e), s);
                return true;
            }
            else
            {
                m_v = default(netlist.devices.matrix_type_e);
                return false;
            }
        }

        public static bool set_from_string(string s, out netlist.devices.matrix_sort_type_e m_v)
        {
            //int f = from_string_int(strings(), s); \
            //if (f>=0) { m_v = static_cast<E>(f); return true; } else { return false; } \
            if (Enum.IsDefined(typeof(netlist.devices.matrix_sort_type_e), s))
            {
                m_v = (netlist.devices.matrix_sort_type_e)Enum.Parse(typeof(netlist.devices.matrix_sort_type_e), s);
                return true;
            }
            else
            {
                m_v = default(netlist.devices.matrix_sort_type_e);
                return false;
            }
        }
    }


    // ----------------------------------------------------------------------------------------
    // A Generic netlist sources implementation
    // ----------------------------------------------------------------------------------------
    public abstract class psource_t
    {
        //using stream_ptr = plib::unique_ptr<std::istream>;

        protected psource_t() { }

        //COPYASSIGNMOVE(psource_t, delete)

        //virtual ~psource_t() noexcept = default;

        public abstract std.istream stream(string name);  //virtual stream_ptr stream(const pstring &name) = 0;
    }


    /**! Generic sources collection
     *
     * @tparam TS base stream class. Default is psource_t
     */
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
        public bool for_all(string name, for_all_F lambda)
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
