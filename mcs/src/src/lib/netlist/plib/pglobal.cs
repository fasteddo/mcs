// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;


namespace mame.plib
{
    static class pglobal
    {
        // pmath
        public static nl_fptype reciprocal(nl_fptype v) { return pmath_global.reciprocal(v); }
        public static nl_fptype abs(nl_fptype v) { return pmath_global.abs(v); }
        public static nl_fptype cos(nl_fptype v) { return pmath_global.cos(v); }
        public static nl_fptype log(nl_fptype v) { return pmath_global.log(v); }
        public static nl_fptype pow(nl_fptype v, nl_fptype p) { return pmath_global.pow(v, p); }
        public static nl_fptype sin(nl_fptype v) { return pmath_global.sin(v); }
        public static nl_fptype sqrt(nl_fptype v) { return pmath_global.sqrt(v); }
        public static nl_fptype trunc(nl_fptype v) { return pmath_global.trunc(v); }
        public static nl_fptype exp(nl_fptype v) { return pmath_global.exp(v); }
        public static nl_fptype floor(nl_fptype v) { return pmath_global.floor(v); }


        // pstonum
        public static int pstonum_ne_int(bool CLOCALE, string arg, out bool err) { return pstonum_global.pstonum_ne_int(CLOCALE, arg, out err); }
        public static double pstonum_ne_nl_fptype(bool CLOCALE, string arg, out bool err) { return pstonum_global.pstonum_ne_nl_fptype(CLOCALE, arg, out err); }


        // pstrutil
        public static bool startsWith(string str, string arg) { return pstrutil_global.startsWith(str, arg); }
        public static bool endsWith(string str, string value) { return pstrutil_global.endsWith(str, value); }
        public static string ucase(string str) { return pstrutil_global.ucase(str); }
        public static string trim(string str) { return pstrutil_global.trim(str); }
        public static string left(string str, int len) { return pstrutil_global.left(str, len); }
        public static string right(string str, int nlen) { return pstrutil_global.right(str, nlen); }
        public static string replace_all(string str, string search, string replace) { return pstrutil_global.replace_all(str, search, replace); }


        // putil
        public static std.vector<string> psplit(string str, string onstr, bool ignore_empty = false) { return putil_global.psplit(str, onstr, ignore_empty); }
        public static std.vector<string> psplit(string str, std.vector<string> onstrl) { return putil_global.psplit(str, onstrl); }
        public static string environment(string var, string default_val) { return putil_global.environment(var, default_val); }
    }
}
