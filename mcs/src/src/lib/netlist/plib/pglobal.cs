// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using size_t = System.UInt32;


namespace mame.plib
{
    static class pglobal
    {
        // pexception
        public static void terminate(string msg) { pexception_global.terminate(msg); }


        // pmath
        public static T reciprocal<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global.reciprocal<T, OPS>(v); }
        public static T abs<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global.abs<T, OPS>(v); }
        public static T sqrt<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global.sqrt<T, OPS>(v); }
        public static T exp<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global.exp<T, OPS>(v); }
        public static T log<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global.log<T, OPS>(v); }
        public static T floor<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global.floor<T, OPS>(v); }
        public static T sin<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global.sin<T, OPS>(v); }
        public static T cos<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global.cos<T, OPS>(v); }
        public static T trunc<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global.trunc<T, OPS>(v); }
        public static T pow<T, OPS>(T v, T p) where OPS : constants_operators<T>, new() { return pmath_global.pow<T, OPS>(v, p); }


        // pstonum
        public static int pstonum_ne_int(bool CLOCALE, string arg, out bool err) { return pstonum_global.pstonum_ne_int(CLOCALE, arg, out err); }
        public static double pstonum_ne_nl_fptype(bool CLOCALE, string arg, out bool err) { return pstonum_global.pstonum_ne_nl_fptype(CLOCALE, arg, out err); }


        // pstrutil
        public static bool startsWith(string str, string arg) { return pstrutil_global.startsWith(str, arg); }
        public static bool endsWith(string str, string value) { return pstrutil_global.endsWith(str, value); }
        public static std.vector<string> psplit(string str, string onstr, bool ignore_empty = false) { return pstrutil_global.psplit(str, onstr, ignore_empty); }
        public static std.vector<string> psplit(string str, char onstr, bool ignore_empty = false) { return pstrutil_global.psplit(str, onstr, ignore_empty); }
        public static std.vector<string> psplit(string str, std.vector<string> onstrl) { return pstrutil_global.psplit(str, onstrl); }
        public static string ucase(string str) { return pstrutil_global.ucase(str); }
        public static string trim(string str) { return pstrutil_global.trim(str); }
        public static string left(string str, int len) { return pstrutil_global.left(str, len); }
        public static string right(string str, int nlen) { return pstrutil_global.right(str, nlen); }
        public static string replace_all(string str, string search, string replace) { return pstrutil_global.replace_all(str, search, replace); }


        // putil
        public static UInt64 hash(string buf, size_t size) { return putil_global.hash(buf, size); }
        public static string environment(string var, string default_val) { return putil_global.environment(var, default_val); }
        public static source_location PSOURCELOC() { return putil_global.PSOURCELOC(); }


        // vector_ops
        public static void vec_add_mult_scalar_p<T, T_OPS>(int n, Pointer<T> result, Pointer<T> v, T scalar) where T_OPS : plib.constants_operators<T>, new() { vector_ops_global.vec_add_mult_scalar_p<T, T_OPS>(n, result, v, scalar); }
    }
}
