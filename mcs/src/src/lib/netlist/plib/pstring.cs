// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.plib
{
    public class pstring_global
    {
        //template<typename T, typename S>
        //public static T pstonum<T>(string arg)
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


        //template<typename R, typename T>
        //public static R pstonum_ne<R>(string str, out bool err)
        public static bool pstonum_ne_bool(string arg, out bool err)
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
        public static int pstonum_ne_int(string arg, out bool err) { int ret; err = !int.TryParse(arg, out ret); return ret; }
        public static float pstonum_ne_float(string arg, out bool err) { float ret; err = !float.TryParse(arg, out ret); return ret; }
        public static double pstonum_ne_double(string arg, out bool err) { double ret; err = !double.TryParse(arg, out ret); return ret; }


        public static bool endsWith(string str, string value) { return str.EndsWith(value); }
        public static string ucase(string str) { return str.ToUpper(); }
        public static string trim(string str) { return str.Trim(); }
        public static string left(string str, int len) { return str.Substring(0, len); }
    }
}
