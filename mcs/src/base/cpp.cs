// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Diagnostics;

using size_t = System.UInt64;
using u8 = System.Byte;

using static mame.cpp_global;


namespace mame
{
    public static class cpp_global
    {
        // c++ built-in
        public static int sizeof_(object value)
        {
            // alternative to System.Runtime.InteropServices.Marshal.SizeOf(typeof(NativeType)); ?

            switch (value)
            {
                case sbyte  t: return 1;
                case byte   t: return 1;
                case Int16  t: return 2;
                case UInt16 t: return 2;
                case Int32  t: return 4;
                case UInt32 t: return 4;
                case Int64  t: return 8;
                case UInt64 t: return 8;
                case Type   t:
                    if      ((Type)value == typeof(sbyte))  return 1;
                    else if ((Type)value == typeof(byte))   return 1;
                    else if ((Type)value == typeof(Int16))  return 2;
                    else if ((Type)value == typeof(UInt16)) return 2;
                    else if ((Type)value == typeof(Int32))  return 4;
                    else if ((Type)value == typeof(UInt32)) return 4;
                    else if ((Type)value == typeof(Int64))  return 8;
                    else if ((Type)value == typeof(UInt64)) return 8;
                    else throw new mcs_notimplemented();
                default: throw new mcs_notimplemented();
            }
        }


        // c++ algorithm
        public static u8 max(u8 a, u8 b) { return std.max(a, b); }
        public static int max(int a, int b) { return std.max(a, b); }
        public static UInt32 max(UInt32 a, UInt32 b) { return std.max(a, b); }
        public static Int64 max(Int64 a, Int64 b) { return std.max(a, b); }
        public static UInt64 max(UInt64 a, UInt64 b) { return std.max(a, b); }
        public static float max(float a, float b) { return std.max(a, b); }
        public static double max(double a, double b) { return std.max(a, b); }
        public static u8 min(u8 a, u8 b) { return std.min(a, b); }
        public static int min(int a, int b) { return std.min(a, b); }
        public static UInt32 min(UInt32 a, UInt32 b) { return std.min(a, b); }
        public static Int64 min(Int64 a, Int64 b) { return std.min(a, b); }
        public static UInt64 min(UInt64 a, UInt64 b) { return std.min(a, b); }
        public static float min(float a, float b) { return std.min(a, b); }
        public static double min(double a, double b) { return std.min(a, b); }


        // c++ cassert
        [DebuggerHidden]
        [Conditional("DEBUG")]
        public static void assert(bool condition, string message = "")
        {
            if (string.IsNullOrEmpty(message))
                Debug.Assert(condition);
            else
                Debug.Assert(condition, message);
        }

        [Conditional("DEBUG")] public static void static_assert(bool condition, string message = "") { assert(condition, message); }


        // c++ cfloat  - https://www.johndcook.com/blog/2012/01/05/double-epsilon-dbl_epsilon/
        public const double DBL_EPSILON = 2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */


        // c++ cmath
        public const double M_PI = Math.PI;
        public const float NAN = float.NaN;  //corecrt_math.h

        public static double ceil(float x) { return std.ceil(x); }
        public static float exp(float x) { return std.exp(x); }
        public static double exp(double x) { return std.exp(x); }
        public static float fabs(float arg) { return std.fabs(arg); }
        public static double fabs(double arg) { return std.fabs(arg); }
        public static float fabsf(float arg) { return std.fabs(arg); }
        public static float floor(float arg) { return std.floor(arg); }
        public static double floor(double arg) { return std.floor(arg); }
        public static float floorf(float arg) { return std.floorf(arg); }
        public static double fmod(double numer, double denom) { return std.fmod(numer, denom); }
        public static float log(float arg) { return std.log(arg); }
        public static double log(double arg) { return std.log(arg); }
        public static double log10(double arg) { return std.log10(arg); }
        public static float pow(float base_, float exponent) { return std.pow(base_, exponent); }
        public static double pow(double base_, double exponent) { return std.pow(base_, exponent); }
        public static float roundf(float arg) { return (float)Math.Round(arg, MidpointRounding.AwayFromZero); }
        public static float sqrt(float arg) { return std.sqrt(arg); }
        public static double sqrt(double arg) { return std.sqrt(arg); }
        public static float tan(float arg) { return std.tan(arg); }
        public static double tan(double arg) { return std.tan(arg); }


        // c++ cctype
        public static int isspace(int c) { return char.IsWhiteSpace((char)c) ? 1 : 0; }


        // c++ cstdio
        public const int SEEK_CUR = 1;
        public const int SEEK_END = 2;
        public const int SEEK_SET = 0;

        public static void sprintf(out string str, string format, params object [] args) { str = string.Format(format, args); }


        public const size_t npos = size_t.MaxValue;
    }
}
