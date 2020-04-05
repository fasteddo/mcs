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
}
