// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using size_t = System.UInt64;
using unsigned = System.UInt32;


namespace mame.plib
{
    static class pg
    {
        // pexception
        public static void terminate(string msg) { pexception_global.terminate(msg); }


        // pmath
        public static T reciprocal<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global<T, OPS>.reciprocal(v); }
        public static nl_fptype reciprocal(nl_fptype v) { return pmath_global<nl_fptype, nl_fptype_ops>.reciprocal(v); }
        public static T abs<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global<T, OPS>.abs(v); }
        public static nl_fptype abs(nl_fptype v) { return pmath_global<nl_fptype, nl_fptype_ops>.abs(v); }
        //public static T sqrt<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global<T, OPS>.sqrt(v); }
        public static nl_fptype sqrt(nl_fptype v) { return pmath_global<nl_fptype, nl_fptype_ops>.sqrt(v); }
        //public static T exp<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global<T, OPS>.exp(v); }
        public static nl_fptype exp(nl_fptype v) { return pmath_global<nl_fptype, nl_fptype_ops>.exp(v); }
        //public static T log<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global<T, OPS>.log(v); }
        public static nl_fptype log(nl_fptype v) { return pmath_global<nl_fptype, nl_fptype_ops>.log(v); }
        //public static T floor<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global<T, OPS>.floor(v); }
        public static nl_fptype floor(nl_fptype v) { return pmath_global<nl_fptype, nl_fptype_ops>.floor(v); }
        public static nl_fptype log1p(nl_fptype v) { return pmath_global<nl_fptype, nl_fptype_ops>.log1p(v); }
        //public static T sin<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global<T, OPS>.sin(v); }
        public static nl_fptype sin(nl_fptype v) { return pmath_global<nl_fptype, nl_fptype_ops>.sin(v); }
        //public static T cos<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global<T, OPS>.cos(v); }
        public static nl_fptype cos(nl_fptype v) { return pmath_global<nl_fptype, nl_fptype_ops>.cos(v); }
        //public static T trunc<T, OPS>(T v) where OPS : constants_operators<T>, new() { return pmath_global<T, OPS>.trunc(v); }
        public static nl_fptype trunc(nl_fptype v) { return pmath_global<nl_fptype, nl_fptype_ops>.trunc(v); }
        //public static T pow<T, OPS>(T v, T p) where OPS : constants_operators<T>, new() { return pmath_global<T, OPS>.pow(v, p); }
        public static nl_fptype pow(nl_fptype v, nl_fptype p) { return pmath_global<nl_fptype, nl_fptype_ops>.pow(v, p); }
        public static nl_fptype clamp(nl_fptype v, nl_fptype low, nl_fptype high) { return pmath_global<nl_fptype, nl_fptype_ops>.clamp(v, low, high); } 


        // pstonum
        public static int pstonum_ne_int(bool CLOCALE, string arg, out bool err) { return pstonum_global.pstonum_ne_int(CLOCALE, arg, out err); }
        public static UInt32 pstonum_ne_unsigned(bool CLOCALE, string arg, out bool err) { return pstonum_global.pstonum_ne_unsigned(CLOCALE, arg, out err); }
        public static Int64 pstonum_ne_int64(bool CLOCALE, string arg, out bool err) { return pstonum_global.pstonum_ne_int64(CLOCALE, arg, out err); }
        public static double pstonum_ne_nl_fptype(bool CLOCALE, string arg, out bool err) { return pstonum_global.pstonum_ne_nl_fptype(CLOCALE, arg, out err); }


        // pstrutil
        public static bool startsWith(string str, string arg) { return pstrutil_global.startsWith(str, arg); }
        public static bool endsWith(string str, string value) { return pstrutil_global.endsWith(str, value); }
        public static std.vector<string> psplit(string str, string onstr, bool ignore_empty = false) { return pstrutil_global.psplit(str, onstr, ignore_empty); }
        public static std.vector<string> psplit(string str, char onstr, bool ignore_empty = false) { return pstrutil_global.psplit(str, onstr, ignore_empty); }
        public static std.vector<string> psplit(string str, std.vector<string> onstrl) { return pstrutil_global.psplit(str, onstrl); }
        public static string ucase(string str) { return pstrutil_global.ucase(str); }
        public static string trim(string str) { return pstrutil_global.trim(str); }
        public static string left(string str, size_t len) { return pstrutil_global.left(str, len); }
        public static string right(string str, size_t nlen) { return pstrutil_global.right(str, nlen); }
        public static string replace_all(string str, string search, string replace) { return pstrutil_global.replace_all(str, search, replace); }


        // ptypes
        public static Type fast_type_for_bits(unsigned bits) { return ptypes_global.fast_type_for_bits(bits); }


        // putil
        public static size_t hash(string buf, size_t size) { return putil_global.hash(buf, size); }
        public static string environment(string var, string default_val) { return putil_global.environment(var, default_val); }
        public static source_location PSOURCELOC() { return putil_global.PSOURCELOC(); }


        // vector_ops
        public static void vec_add_mult_scalar_p<T, T_OPS>(int n, Pointer<T> result, Pointer<T> v, T scalar) where T_OPS : plib.constants_operators<T>, new() { vector_ops_global<T, T_OPS>.vec_add_mult_scalar_p(n, result, v, scalar); }
        public static void vec_add_mult_scalar_p(int n, Pointer<nl_fptype> result, Pointer<nl_fptype> v, nl_fptype scalar) { vector_ops_global<nl_fptype, nl_fptype_ops>.vec_add_mult_scalar_p(n, result, v, scalar); }
    }
}
