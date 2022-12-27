// license:BSD-3-Clause
// copyright-holders:Edward Fast

//#define ASSERT_SLOW

using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;

using attoseconds_t = System.Int64;  //typedef s64 attoseconds_t;
using char32_t = System.UInt32;
using endianness_t = mame.util.endianness;  //using endianness_t = util::endianness;
using int16_t = System.Int16;
using int32_t = System.Int32;
using int64_t = System.Int64;
using ioport_value = System.UInt32;  //typedef u32 ioport_value;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using offs_t = System.UInt32;  //using offs_t = u32;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using PointerU8 = mame.Pointer<System.Byte>;
using s8 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using size_t = System.UInt64;
using std_time_t = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;
using unsigned = System.UInt32;
using uX = mame.FlexPrim;

using static mame.cpp_global;
using static mame.emucore_global;
using static mame.osdcore_global;


namespace mame
{
    public interface bool_const { bool value { get; } }
    public class bool_const_true : bool_const { public bool value { get { return true; } } }
    public class bool_const_false : bool_const { public bool value { get { return false; } } }

    public interface int_const { int value { get; } }
    public class int_const_n512 : int_const { public int value { get { return -512; } } }
    public class int_const_n256 : int_const { public int value { get { return -256; } } }
    public class int_const_n128 : int_const { public int value { get { return -128; } } }
    public class int_const_n64 : int_const { public int value { get { return -64; } } }
    public class int_const_n32 : int_const { public int value { get { return -32; } } }
    public class int_const_n16 : int_const { public int value { get { return -16; } } }
    public class int_const_n3 : int_const { public int value { get { return -3; } } }
    public class int_const_n2 : int_const { public int value { get { return -2; } } }
    public class int_const_n1 : int_const { public int value { get { return -1; } } }
    public class int_const_0 : int_const { public int value { get { return 0; } } }
    public class int_const_1 : int_const { public int value { get { return 1; } } }
    public class int_const_2 : int_const { public int value { get { return 2; } } }
    public class int_const_3 : int_const { public int value { get { return 3; } } }
    public class int_const_4 : int_const { public int value { get { return 4; } } }
    public class int_const_5 : int_const { public int value { get { return 5; } } }
    public class int_const_6 : int_const { public int value { get { return 6; } } }
    public class int_const_7 : int_const { public int value { get { return 7; } } }
    public class int_const_8 : int_const { public int value { get { return 8; } } }
    public class int_const_9 : int_const { public int value { get { return 9; } } }
    public class int_const_10 : int_const { public int value { get { return 10; } } }
    public class int_const_11 : int_const { public int value { get { return 11; } } }
    public class int_const_12 : int_const { public int value { get { return 12; } } }
    public class int_const_13 : int_const { public int value { get { return 13; } } }
    public class int_const_14 : int_const { public int value { get { return 14; } } }
    public class int_const_15 : int_const { public int value { get { return 15; } } }
    public class int_const_16 : int_const { public int value { get { return 16; } } }
    public class int_const_17 : int_const { public int value { get { return 17; } } }
    public class int_const_18 : int_const { public int value { get { return 18; } } }
    public class int_const_19 : int_const { public int value { get { return 19; } } }
    public class int_const_20 : int_const { public int value { get { return 20; } } }
    public class int_const_21 : int_const { public int value { get { return 21; } } }
    public class int_const_22 : int_const { public int value { get { return 22; } } }
    public class int_const_23 : int_const { public int value { get { return 23; } } }
    public class int_const_24 : int_const { public int value { get { return 24; } } }
    public class int_const_25 : int_const { public int value { get { return 25; } } }
    public class int_const_26 : int_const { public int value { get { return 26; } } }
    public class int_const_27 : int_const { public int value { get { return 27; } } }
    public class int_const_28 : int_const { public int value { get { return 28; } } }
    public class int_const_29 : int_const { public int value { get { return 29; } } }
    public class int_const_30 : int_const { public int value { get { return 30; } } }
    public class int_const_31 : int_const { public int value { get { return 31; } } }
    public class int_const_32 : int_const { public int value { get { return 32; } } }

    public interface u32_const { UInt32 value { get; } }
    public class u32_const_0 : u32_const { public UInt32 value { get { return 0; } } }
    public class u32_const_1 : u32_const { public UInt32 value { get { return 1; } } }
    public class u32_const_2 : u32_const { public UInt32 value { get { return 2; } } }
    public class u32_const_3 : u32_const { public UInt32 value { get { return 3; } } }
    public class u32_const_4 : u32_const { public UInt32 value { get { return 4; } } }
    public class u32_const_5 : u32_const { public UInt32 value { get { return 5; } } }
    public class u32_const_6 : u32_const { public UInt32 value { get { return 6; } } }
    public class u32_const_7 : u32_const { public UInt32 value { get { return 7; } } }
    public class u32_const_8 : u32_const { public UInt32 value { get { return 8; } } }
    public class u32_const_9 : u32_const { public UInt32 value { get { return 9; } } }
    public class u32_const_10 : u32_const { public UInt32 value { get { return 10; } } }
    public class u32_const_11 : u32_const { public UInt32 value { get { return 11; } } }
    public class u32_const_12 : u32_const { public UInt32 value { get { return 12; } } }
    public class u32_const_16 : u32_const { public UInt32 value { get { return 16; } } }
    public class u32_const_20 : u32_const { public UInt32 value { get { return 20; } } }

    public interface u64_const { UInt64 value { get; } }
    public class u64_const_0 : u64_const { public UInt64 value { get { return 0; } } }
    public class u64_const_2 : u64_const { public UInt64 value { get { return 2; } } }
    public class u64_const_3 : u64_const { public UInt64 value { get { return 3; } } }
    public class u64_const_4 : u64_const { public UInt64 value { get { return 4; } } }
    public class u64_const_5 : u64_const { public UInt64 value { get { return 5; } } }
    public class u64_const_6 : u64_const { public UInt64 value { get { return 6; } } }
    public class u64_const_7 : u64_const { public UInt64 value { get { return 7; } } }
    public class u64_const_8 : u64_const { public UInt64 value { get { return 8; } } }
    public class u64_const_16 : u64_const { public UInt64 value { get { return 16; } } }
    public class u64_const_17 : u64_const { public UInt64 value { get { return 17; } } }
    public class u64_const_29 : u64_const { public UInt64 value { get { return 29; } } }
    public class u64_const_31 : u64_const { public UInt64 value { get { return 31; } } }
    public class u64_const_37 : u64_const { public UInt64 value { get { return 37; } } }
    public class u64_const_43 : u64_const { public UInt64 value { get { return 43; } } }
    public class u64_const_64 : u64_const { public UInt64 value { get { return 64; } } }
    public class u64_const_156 : u64_const { public UInt64 value { get { return 156; } } }
    public class u64_const_256 : u64_const { public UInt64 value { get { return 256; } } }
    public class u64_const_312 : u64_const { public UInt64 value { get { return 312; } } }
    public class u64_const_6364136223846793005 : u64_const { public UInt64 value { get { return 6364136223846793005; } } }
    public class u64_const_0x5555555555555555 : u64_const { public UInt64 value { get { return 0x5555555555555555; } } }
    public class u64_const_0x71d67fffeda60000 : u64_const { public UInt64 value { get { return 0x71d67fffeda60000; } } }
    public class u64_const_0xb5026f5aa96619e9 : u64_const { public UInt64 value { get { return 0xb5026f5aa96619e9; } } }
    public class u64_const_0xfff7eee000000000 : u64_const { public UInt64 value { get { return 0xfff7eee000000000; } } }

    public interface endianness_t_const { endianness_t value { get; } }
    public class endianness_t_const_ENDIANNESS_LITTLE : endianness_t_const { public endianness_t value { get { return ENDIANNESS_LITTLE; } } }
    public class endianness_t_const_ENDIANNESS_BIG : endianness_t_const { public endianness_t value { get { return ENDIANNESS_BIG; } } }


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
                    else throw new emu_unimplemented();
                default: throw new emu_unimplemented();
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
        public static attotime max(attotime a, attotime b) { return std.max(a, b); }
        public static netlist_time max(netlist_time a, netlist_time b) { return std.max(a, b); }
        public static u8 min(u8 a, u8 b) { return std.min(a, b); }
        public static int min(int a, int b) { return std.min(a, b); }
        public static UInt32 min(UInt32 a, UInt32 b) { return std.min(a, b); }
        public static Int64 min(Int64 a, Int64 b) { return std.min(a, b); }
        public static UInt64 min(UInt64 a, UInt64 b) { return std.min(a, b); }
        public static float min(float a, float b) { return std.min(a, b); }
        public static double min(double a, double b) { return std.min(a, b); }
        public static attotime min(attotime a, attotime b) { return std.min(a, b); }


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

        [Conditional("ASSERT_SLOW")] public static void assert_slow(bool condition) { assert(condition); }
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


    public class emu_unimplemented : emu_fatalerror
    {
        [DebuggerHidden]
        public emu_unimplemented() : base("Unimplemented") { }
    }


    public static class std
    {
        public static size_t size(string x) { return (size_t)x.Length; }
        public static size_t size<T>(T [] x) { return (size_t)x.Length; }
        public static size_t size<T>(MemoryContainer<T> x) { return (size_t)x.Count; }


        // c++ chrono
        public static class chrono
        {
            public static class system_clock
            {
                public static DateTimeOffset now() { return DateTimeOffset.Now; }
                public static DateTimeOffset from_time_t(std_time_t t) { return DateTimeOffset.FromUnixTimeSeconds(t); }
                public static std_time_t to_time_t(DateTimeOffset t) { return t.ToUnixTimeSeconds(); }
            }

            public static TimeSpan hours(int hours) { return new TimeSpan(hours, 0, 0); }
        }


        // c++ algorithm
        public static int clamp(int v, int lo, int hi) { return ExtensionIComparable.Clamp(v, lo, hi); }
        public static float clamp(float v, float lo, float hi) { return ExtensionIComparable.Clamp(v, lo, hi); }
        public static double clamp(double v, double lo, double hi) { return ExtensionIComparable.Clamp(v, lo, hi); }
        public static void fill<T>(MemoryContainer<T> destination, T value) { std.memset(destination, value); }
        public static void fill<T>(IList<T> destination, T value) { std.memset(destination, value); }
        public static void fill<T>(IList<T> destination, Func<T> value) { std.memset(destination, value); }
        public static void fill<T>(T [,] destination, T value) { std.memset(destination, value); }
        public static void fill_n<T>(MemoryContainer<T> destination, size_t count, T value) { std.memset(destination, value, count); }
        public static void fill_n<T>(Pointer<T> destination, size_t count, T value) { std.memset(destination, value, count); }
        public static void fill_n(PointerU16 destination, size_t count, UInt16 value) { std.memset(destination, value, count); }
        public static void fill_n(PointerU32 destination, size_t count, UInt32 value) { std.memset(destination, value, count); }
        public static void fill_n(PointerU64 destination, size_t count, UInt64 value) { std.memset(destination, value, count); }
        public static void fill_n<T>(IList<T> destination, size_t count, T value) { std.memset(destination, value, count); }
        public static T find_if<T>(IEnumerable<T> list, Func<T, bool> pred) { foreach (var item in list) { if (pred(item)) return item; } return default;  }
        public static int lower_bound<C, V>(C collection, V value) where C : IList<V> where V : IComparable<V> { return lower_bound<C, V, V>(collection, value, (i, v) => { return i.CompareTo(v) < 0; }); }
        public static int lower_bound<C, I, V>(C collection, V value, Func<I, V, bool> func)
            where C : IList<I>
        {
            int first = 0;
            int count = collection.Count;

            if (count == 0)
                return -1;

            while (0 < count)
            {
                int count2 = count / 2;
                int mid = first + count2;
                if (func(collection[mid], value))
                {
                    first = mid + 1;
                    count -= count2 + 1;
                }
                else
                {
                    count = count2;
                }
            }

            return first;
        }
        // std::min/max behaves differently than Math.Min/Max with respect to NaN
        // https://docs.microsoft.com/en-us/dotnet/api/system.math.min?view=net-5.0#System_Math_Min_System_Single_System_Single_
        // https://stackoverflow.com/a/39919244
        public static u8 max(u8 a, u8 b) { return Math.Max(a, b); }
        public static int max(int a, int b) { return Math.Max(a, b); }
        public static UInt32 max(UInt32 a, UInt32 b) { return Math.Max(a, b); }
        public static Int64 max(Int64 a, Int64 b) { return Math.Max(a, b); }
        public static UInt64 max(UInt64 a, UInt64 b) { return Math.Max(a, b); }
        public static float max(float a, float b) { if (float.IsNaN(a) || float.IsNaN(b)) return a; else return Math.Max(a, b); }
        public static double max(double a, double b) { if (double.IsNaN(a) || double.IsNaN(b)) return a; else return Math.Max(a, b); }
        public static attotime max(attotime a, attotime b) { return attotime.Max(a, b); }
        public static netlist_time max(netlist_time a, netlist_time b) { return netlist_time.Max(a, b); }
        public static u8 min(u8 a, u8 b) { return Math.Min(a, b); }
        public static int min(int a, int b) { return Math.Min(a, b); }
        public static UInt32 min(UInt32 a, UInt32 b) { return Math.Min(a, b); }
        public static Int64 min(Int64 a, Int64 b) { return Math.Min(a, b); }
        public static UInt64 min(UInt64 a, UInt64 b) { return Math.Min(a, b); }
        public static float min(float a, float b) { if (float.IsNaN(a) || float.IsNaN(b)) return a; else return Math.Min(a, b); }
        public static double min(double a, double b) { if (double.IsNaN(a) || double.IsNaN(b)) return a; else return Math.Min(a, b); }
        public static attotime min(attotime a, attotime b) { return attotime.Min(a, b); }
        public static void sort<T>(MemoryContainer<T> list, Comparison<T> pred) { list.Sort(pred); }


        // c++ cmath
        public static int abs(int arg) { return Math.Abs(arg); }
        public static float abs(float arg) { return Math.Abs(arg); }
        public static double abs(double arg) { return Math.Abs(arg); }
        public static float atan(float arg) { return (float)Math.Atan(arg); }
        public static double atan(double arg) { return Math.Atan(arg); }
        public static float ceil(float x) { return (float)Math.Ceiling(x); }
        public static double ceil(double x) { return Math.Ceiling(x); }
        public static float cos(float arg) { return (float)Math.Cos(arg); }
        public static double cos(double arg) { return Math.Cos(arg); }
        public static float exp(float x) { return (float)Math.Exp(x); }
        public static double exp(double x) { return Math.Exp(x); }
        public static float fabs(float arg) { return Math.Abs(arg); }
        public static double fabs(double arg) { return Math.Abs(arg); }
        public static float fabsf(float arg) { return Math.Abs(arg); }
        public static float floor(float arg) { return (float)Math.Floor(arg); }
        public static double floor(double arg) { return Math.Floor(arg); }
        public static float floorf(float arg) { return (float)Math.Floor(arg); }
        public static bool isnan(float arg) { return Single.IsNaN(arg); }
        public static bool isnan(double arg) { return Double.IsNaN(arg); }
        public static double fmod(double numer, double denom) { return numer % denom; }
        public static float log(float arg) { return (float)Math.Log(arg); }
        public static double log(double arg) { return Math.Log(arg); }
        public static double log10(double arg) { return Math.Log10(arg); }
        public static double log1p(double arg) { return Math.Abs(arg) > 1e-4 ? Math.Log(1.0 + arg) : (-0.5 * arg + 1.0) * arg; }  //https://stackoverflow.com/a/50012422
        public static float pow(float base_, float exponent) { return (float)Math.Pow(base_, exponent); }
        public static double pow(double base_, double exponent) { return Math.Pow(base_, exponent); }
        public static int lround(double x) { return (int)Math.Round(x, MidpointRounding.AwayFromZero); }
        public static float sin(float arg) { return (float)Math.Sin(arg); }
        public static double sin(double arg) { return Math.Sin(arg); }
        public static float sqrt(float arg) { return (float)Math.Sqrt(arg); }
        public static double sqrt(double arg) { return Math.Sqrt(arg); }
        public static float tan(float arg) { return (float)Math.Tan(arg); }
        public static double tan(double arg) { return Math.Tan(arg); }
        public static float trunc(float arg) { return (float)Math.Truncate(arg); }
        public static double trunc(double arg) { return Math.Truncate(arg); }


        // c++ cstdlib
        public static void abort() { terminate(); }
        public static int atoi(string str) { return Convert.ToInt32(str); }
        public static string getenv(string env_var) { return System.Environment.GetEnvironmentVariable(env_var); }
        public static UInt64 strtoul(string str, string endptr, int base_) { return Convert.ToUInt64(str, 16); }


        // c++ cstring
        public static int memcmp<T>(MemoryContainer<T> ptr1, MemoryContainer<T> ptr2, size_t num) { return ptr1.CompareTo(ptr2, (int)num) ? 0 : 1; }  //  const void * ptr1, const void * ptr2, size_t num
        public static int memcmp<T>(Pointer<T> ptr1, Pointer<T> ptr2, size_t num) { return ptr1.CompareTo(ptr2, (int)num) ? 0 : 1; }  //  const void * ptr1, const void * ptr2, size_t num
        public static void memcpy<T>(MemoryContainer<T> destination, MemoryContainer<T> source, size_t num) { source.CopyTo(0, destination, 0, (int)num); }  //void * destination, const void * source, size_t num );
        public static void memcpy<T>(Pointer<T> destination, Pointer<T> source, size_t num) { source.CopyTo(0, destination, 0, (int)num); }  //void * destination, const void * source, size_t num );
        public static void memset<T>(MemoryContainer<T> destination, T value) { destination.Fill(value); }
        public static void memset<T>(MemoryContainer<T> destination, T value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset<T>(Pointer<T> destination, T value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset(PointerU16 destination, UInt16 value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset(PointerU32 destination, UInt32 value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset(PointerU64 destination, UInt64 value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset<T>(IList<T> destination, T value) { memset(destination, value, (size_t)destination.Count); }
        public static void memset<T>(IList<T> destination, T value, size_t num) { for (int i = 0; i < (int)num; i++) destination[i] = value; }
        public static void memset<T>(IList<T> destination, Func<T> value) { memset(destination, value, (size_t)destination.Count); }
        public static void memset<T>(IList<T> destination, Func<T> value, size_t num) { for (int i = 0; i < (int)num; i++) destination[i] = value(); }
        public static void memset<T>(T [,] destination, T value) { for (int i = 0; i < destination.GetLength(0); i++) for (int j = 0; j < destination.GetLength(1); j++) destination[i, j] = value; }
        public static int strchr(string str, char character) { return str.IndexOf(character); }
        public static int strcmp(string str1, string str2) { return string.Compare(str1, str2); }
        public static size_t strlen(string str) { return (size_t)str.Length; }
        public static int strncmp(string str1, string str2, size_t num) { return string.Compare(str1, 0, str2, 0, (int)num); }
        public static int strstr(string str1, string str2) { return str1.IndexOf(str2); }
        public static string to_string(int val) { return val.ToString(); }
        public static string to_string(UInt32 val) { return val.ToString(); }
        public static string to_string(Int64 val) { return val.ToString(); }
        public static string to_string(UInt64 val) { return val.ToString(); }
        public static string to_string(float val) { return val.ToString(); }
        public static string to_string(double val) { return val.ToString(); }


        // c++ exception
        public static void terminate() { throw new emu_fatalerror("std.terminate() called"); }


        // c++ iostream
        public static void cerr(string s) { osd_printf_debug(s); }


        // c++ numeric
        public static UInt32 gcd(UInt32 a, UInt32 b)
        {
            return b != 0 ? gcd(b, a % b) : a;
        }


        // c++ utility
        public static void swap<T>(ref T val1, ref T val2)
        {
            assert(typeof(T).GetTypeInfo().IsValueType);

            T temp = val1;
            val1 = val2;
            val2 = temp;
        }

        public static std.pair<T, V> make_pair<T, V>(T t, V v) { return new std.pair<T, V>(t, v); }


        // c++ array
        public class array<T, size_t_N> : IList<T>
            where size_t_N : u64_const, new()
        {
            protected static readonly size_t N = new size_t_N().value;


            MemoryContainer<T> m_data = new MemoryContainer<T>((int)N, true);


            public array() { }
            public array(params T [] args)
            {
                if (args.Length != (int)N)
                    throw new emu_fatalerror("array() parameter count doen't match size. Provided: {0}, Expected: {1}", args.Length, N);

                for (int i = 0; i < args.Length; i++)
                    m_data[i] = args[i];
            }


            // IList
            public int IndexOf(T value) { return m_data.IndexOf(value); }
            void IList<T>.Insert(int index, T value) { throw new emu_unimplemented(); }
            void IList<T>.RemoveAt(int index) { throw new emu_unimplemented(); }
            void ICollection<T>.Add(T value) { throw new emu_unimplemented(); }
            bool ICollection<T>.Contains(T value) { throw new emu_unimplemented(); }
            void ICollection<T>.Clear() { throw new emu_unimplemented(); }
            void ICollection<T>.CopyTo(T [] array, int index) { throw new emu_unimplemented(); }
            bool ICollection<T>.Remove(T value) { throw new emu_unimplemented(); }
            public int Count { get { return m_data.Count; } }
            bool ICollection<T>.IsReadOnly { get { throw new emu_unimplemented(); } }
            IEnumerator IEnumerable.GetEnumerator() { return m_data.GetEnumerator(); }
            IEnumerator<T> IEnumerable<T>.GetEnumerator() { return ((IEnumerable<T>)m_data).GetEnumerator(); }


            public static bool operator ==(array<T, size_t_N> lhs, array<T, size_t_N> rhs)
            {
                // TODO available in .NET 3.5 and higher
                //return Enumerable.SequenceEquals(lhs, rhs);

                if (ReferenceEquals(lhs, rhs))
                    return true;

                if (ReferenceEquals(lhs, default) || ReferenceEquals(rhs, default))
                    return false;

                if (lhs.size() != rhs.size())
                    return false;

                EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                for (size_t i = 0; i < lhs.size(); i++)
                {
                    if (!comparer.Equals(lhs[i], rhs[i])) return false;
                }

                return true;
            }

            public static bool operator !=(array<T, size_t_N> lhs, array<T, size_t_N> rhs)
            {
                return !(lhs == rhs);
            }


            public override bool Equals(object obj)
            {
                return this == (array<T, size_t_N>)obj;
            }


            public override int GetHashCode()
            {
                return m_data.GetHashCode();
            }

            public T this[int index] { get { return m_data[index]; } set { m_data[index] = value; } }
            public T this[UInt64 index] { get { return m_data[index]; } set { m_data[index] = value; } }


            public size_t size() { return (size_t)Count; }
            public void fill(T value) { std.fill(this, value); }
            public Pointer<T> data() { return new Pointer<T>(m_data); }
        }


        // c++ errc
        public enum errc
        {
            address_family_not_supported,
            address_in_use,
            address_not_available,
            already_connected,
            argument_list_too_long,
            argument_out_of_domain,
            bad_address,
            bad_file_descriptor,
            bad_message,
            broken_pipe,
            connection_aborted,
            connection_already_in_progress,
            connection_refused,
            connection_reset,
            cross_device_link,
            destination_address_required,
            device_or_resource_busy,
            directory_not_empty,
            executable_format_error,
            file_exists,
            file_too_large,
            filename_too_long,
            function_not_supported,
            host_unreachable,
            identifier_removed,
            illegal_byte_sequence,
            inappropriate_io_control_operation,
            interrupted,
            invalid_argument,
            invalid_seek,
            io_error,
            is_a_directory,
            message_size,
            network_down,
            network_reset,
            network_unreachable,
            no_buffer_space,
            no_child_process,
            no_link,
            no_lock_available,
            no_message_available,
            no_message,
            no_protocol_option,
            no_space_on_device,
            no_stream_resources,
            no_such_device_or_address,
            no_such_device,
            no_such_file_or_directory,
            no_such_process,
            not_a_directory,
            not_a_socket,
            not_a_stream,
            not_connected,
            not_enough_memory,
            not_supported,
            operation_canceled,
            operation_in_progress,
            operation_not_permitted,
            operation_not_supported,
            operation_would_block,
            owner_dead,
            permission_denied,
            protocol_error,
            protocol_not_supported,
            read_only_file_system,
            resource_deadlock_would_occur,
            resource_unavailable_try_again,
            result_out_of_range,
            state_not_recoverable,
            stream_timeout,
            text_file_busy,
            timed_out,
            too_many_files_open_in_system,
            too_many_files_open,
            too_many_links,
            too_many_symbolic_link_levels,
            value_too_large,
            wrong_protocol_type,
        }


        // c++ error_category
        public class error_category
        {
            public virtual string name() { return "error_category.name() - TODO needs implementation"; }  //{ throw new emu_unimplemented(); }
            public virtual string message(int condition) { return string.Format("error_category.message({0}) - TODO needs implementation", condition); }  //{ throw new emu_unimplemented(); }
        }

        class generic_category : error_category
        {
            public override string name() { return "generic"; }
        }


        // c++ error_condition
        public class error_condition
        {
            int m_value;
            error_category m_category;

            public error_condition() { m_value = 0; m_category = new std.generic_category(); }
            public error_condition(errc e) { m_value = (int)e; m_category = new std.generic_category(); }

            public int value() { return m_value; }
            public error_category category() { return m_category; }
            public string message() { return category().message(value()); }

            public static bool operator ==(error_condition obj1, errc obj2) { return obj1 == new error_condition(obj2); }
            public static bool operator !=(error_condition obj1, errc obj2) { return obj1 != new error_condition(obj2); }

            public static implicit operator bool(error_condition e) { return !object.ReferenceEquals(e, null) && e.value() != 0; }
            public static implicit operator error_condition(errc d) { return new error_condition(d); }

            public override bool Equals(object obj)
            {
                return obj is error_condition condition &&
                       m_value == condition.m_value &&
                       EqualityComparer<error_category>.Default.Equals(m_category, condition.m_category);
            }

            public override int GetHashCode()
            {
                int hashCode = 1089423243;
                hashCode = hashCode * -1521134295 + m_value.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<error_category>.Default.GetHashCode(m_category);
                return hashCode;
            }
        }


        // c++ forward_list
        public class forward_list<T> : IEnumerable<T>
        {
            LinkedList<T> m_list = new LinkedList<T>();


            public forward_list() { }
            public forward_list(IEnumerable<T> collection) { m_list = new LinkedList<T>(collection); }


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_list.GetEnumerator(); }
            IEnumerator<T> IEnumerable<T>.GetEnumerator() { return m_list.GetEnumerator(); }


            public void emplace_front(T item) { m_list.AddFirst(item); }
        }


        // c++ istream
        public class istream
        {
        }


        // c++ list
        public class list<T> : IEnumerable<T>
        {
            LinkedList<T> m_list = new LinkedList<T>();


            public list() { }
            public list(IEnumerable<T> collection) { m_list = new LinkedList<T>(collection); }


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_list.GetEnumerator(); }
            IEnumerator<T> IEnumerable<T>.GetEnumerator() { return m_list.GetEnumerator(); }


            // std::list functions
            public void clear() { m_list.Clear(); }
            public LinkedListNode<T> emplace_back(T item) { return m_list.AddLast(item); }
            public bool empty() { return m_list.Count == 0; }
            public void push_back(T item) { m_list.AddLast(item); }
            public void push_front(T item) { m_list.AddFirst(item); }
            public size_t size() { return (size_t)m_list.Count; }
        }


        // c++ map
        public class map<K, V> : IEnumerable<KeyValuePair<K, V>>
        {
            Dictionary<K, V> m_dictionary = new Dictionary<K, V>();


            public map() { }
            //public Dictionary(int capacity);
            //public Dictionary(IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary);
            //public Dictionary(int capacity, IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer);
            //protected Dictionary(SerializationInfo info, StreamingContext context);


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_dictionary.GetEnumerator(); }
            IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() { return m_dictionary.GetEnumerator(); }


            public V this[K key] { get { return m_dictionary[key]; } set { m_dictionary[key] = value; } }


            public void Add(K key, V value) { m_dictionary.Add(key, value); }


            // std::map functions
            public void clear() { m_dictionary.Clear(); }
            public bool emplace(K key, V value) { if (m_dictionary.ContainsKey(key)) { return false; } else { m_dictionary.Add(key, value); return true; } }
            public void erase(K key) { m_dictionary.Remove(key); }
            public V find(K key) { V value; if (m_dictionary.TryGetValue(key, out value)) return value; else return default; }
            public size_t size() { return (size_t)m_dictionary.Count; }
            public bool try_emplace(K key, V value) { return emplace(key, value); }  // HACK - probably not the correct implementation
        }


        // c++ multimap
        public class multimap<K, V>
        {
            Dictionary<K, List<V>> m_dictionary = new Dictionary<K, List<V>>();


            public multimap() { }
            //public Dictionary(int capacity);
            //public Dictionary(IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary);
            //public Dictionary(int capacity, IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer);
            //protected Dictionary(SerializationInfo info, StreamingContext context);
        }


        // c++ pair
        public class pair<T, V>
        {
            Tuple<T, V> m_pair;


            public pair(T key, V value) { m_pair = Tuple.Create(key, value); }

            public override string ToString() { return m_pair.ToString(); }


            public T first { get { return m_pair.Item1; } }
            public V second { get { return m_pair.Item2; } }
        }


        // c++ set
        public class set<T> : IEnumerable<T>
        {
            HashSet<T> m_set = new HashSet<T>();


            public set() { }
            //public HashSet(IEqualityComparer<T> comparer);
            //public HashSet(IEnumerable<T> collection);
            //public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer);
            //protected HashSet(SerializationInfo info, StreamingContext context);


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_set.GetEnumerator(); }
            IEnumerator<T> IEnumerable<T>.GetEnumerator() { return m_set.GetEnumerator(); }


            public bool ContainsIf(Func<T, bool> predicate)
            {
                foreach (var element in m_set)
                {
                    if (predicate(element))
                        return true;
                }
                return false;
            }


            // std::set functions
            public void clear() { m_set.Clear(); }
            public bool emplace(T item) { return m_set.Add(item); }
            public bool empty() { return m_set.Count == 0; }
            public bool erase(T item) { return m_set.Remove(item); }
            public bool find(T item) { return m_set.Contains(item); }
            public T find(Func<T, bool> predicate) { return m_set.FirstOrDefault(predicate); }
            public bool insert(T item) { return m_set.Add(item); }
            public size_t size() { return (size_t)m_set.Count; }
        }


        // c++ stack
        public class stack<T>
        {
            Stack<T> m_stack = new Stack<T>();


            // std::stack functions
            public bool empty() { return m_stack.Count == 0; }
            public T top() { return m_stack.Peek(); }
            public void push(T value) { m_stack.Push(value); }
            public void pop() { m_stack.Pop(); }
        }


        // c++ stdexcept
        public class out_of_range : ArgumentOutOfRangeException
        {
            public out_of_range() : base() { }
            public out_of_range(string paramName) : base(paramName) { }
        }


        // c++ unordered_map
        public class unordered_map<K, V> : IEnumerable<KeyValuePair<K, V>>
        {
            Dictionary<K, V> m_dictionary = new Dictionary<K, V>();


            public unordered_map() { }
            //public Dictionary(int capacity);
            //public Dictionary(IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary);
            //public Dictionary(int capacity, IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer);
            //protected Dictionary(SerializationInfo info, StreamingContext context);


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_dictionary.GetEnumerator(); }
            IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() { return m_dictionary.GetEnumerator(); }


            // this behavior matches std::unordered_map [] operator, except it doesn't new() the object, but uses default instead.
            // this can cause some unexpected issues.  see models_t.value_str() for example
            public V this[K key]
            {
                get
                {
                    V value;
                    if (m_dictionary.TryGetValue(key, out value))
                        return value;

                    value = default;
                    m_dictionary.Add(key, value);
                    return value;
                }
                set
                {
                    m_dictionary[key] = value;
                }
            }


            // std::unordered_map functions
            public V at(K key) { V value; if (m_dictionary.TryGetValue(key, out value)) return value; else return default; }
            public void clear() { m_dictionary.Clear(); }
            public bool emplace(K key, V value) { if (m_dictionary.ContainsKey(key)) { return false; } else { m_dictionary.Add(key, value); return true; } }
            public bool empty() { return m_dictionary.Count == 0; }
            public bool erase(K key) { return m_dictionary.Remove(key); }
            public V find(K key) { return at(key); }
            public bool insert(K key, V value) { if (m_dictionary.ContainsKey(key)) { return false; } else { m_dictionary.Add(key, value); return true; } }
            public bool insert(std.pair<K, V> keyvalue) { return insert(keyvalue.first, keyvalue.second); }
            public size_t size() { return (size_t)m_dictionary.Count; }
        }


        // c++ unordered_set
        public class unordered_set<T>
        {
            HashSet<T> m_set = new HashSet<T>();


            public unordered_set() { }
            //public HashSet(IEqualityComparer<T> comparer);
            //public HashSet(IEnumerable<T> collection);
            //public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer);
            //protected HashSet(SerializationInfo info, StreamingContext context);


            // std::unordered_set functions
            public void clear() { m_set.Clear(); }
            public bool emplace(T item) { return m_set.Add(item); }
            public bool erase(T item) { return m_set.Remove(item); }
            public bool find(T item) { return m_set.Contains(item); }
            public bool insert(T item) { return m_set.Add(item); }
        }


        // c++ vector
        public class vector<T> : MemoryContainer<T>
        {
            public vector() : base() { }
            // this is different behavior as List<T> so that it matches how std::vector works
            public vector(s32 count, T data = default) : base(count) { resize((size_t)count, data); }
            public vector(size_t count, T data = default) : base((int)count) { resize(count, data); }
            public vector(IEnumerable<T> collection) : base(collection) { }


            // std::vector functions
            public T front() { return this[0]; }
            public T back() { return empty() ? default : this[Count - 1]; }
            public void clear() { Clear(); }
            public Pointer<T> data() { return new Pointer<T>(this); }
            public void emplace(int index, T item) { Insert(index, item); }
            public void emplace_back(T item) { Add(item); }
            public bool empty() { return Count == 0; }
            public void erase(int index) { RemoveAt(index); }
            public void insert(int index, T item) { Insert(index, item); }
            public void pop_back() { if (Count > 0) { RemoveAt(Count - 1); } }
            public void push_back(T item) { Add(item); }
            public void push_front(T item) { Insert(0, item); }
            public void resize(size_t count, T data = default) { Resize((int)count, data); }
            public void resize(size_t count, Func<T> creator) { Resize((int)count, creator); }
            public void reserve(size_t value) { Capacity = (int)value; }
            public size_t size() { return (size_t)Count; }
        }
    }


    // this wraps a Memory<T> struct so that bulk operations can be done efficiently
    // std.vector and MemoryU8 derive from this.
    public class MemoryContainer<T> : IList<T>, IReadOnlyList<T>
    {
        class MemoryContainerEnumerator : IEnumerator<T>
        {
            MemoryContainer<T> m_list;
            int m_index;
            int m_endIndex;
 
            public MemoryContainerEnumerator(MemoryContainer<T> list)
            {
                m_list = list;
                m_index = -1;
                m_endIndex = list.Count;
            }
 
            public void Dispose() { }
            public bool MoveNext() { if (m_index < m_endIndex) { m_index++; return m_index < m_endIndex; } return false; }
            object IEnumerator.Current { get { return null; } }
            public T Current
            {
                get
                {
                    if (m_index < 0) throw new InvalidOperationException(string.Format("ListBaseEnumerator() - {0}", m_index));
                    if (m_index >= m_endIndex) throw new InvalidOperationException(string.Format("ListBaseEnumerator() - {0}", m_index));
                    return m_list[m_index];
                }
            }
            public void Reset() { m_index = -1; }
        }


        T [] m_data;
        Memory<T> m_memory;
        int m_actualLength = 0;


        public MemoryContainer() : this(0) { }
        public MemoryContainer(int capacity, bool allocate = false) { m_data = new T [capacity]; m_memory = new Memory<T>(m_data); if (allocate) Resize(capacity); }
        public MemoryContainer(IEnumerable<T> collection) : this() { foreach (var item in collection) Add(item); }
        public MemoryContainer(T [] collection) : this() { SetInternal(collection); }


        // IList

        public virtual T this[int index] { get { return m_data[index]; } set { m_data[index] = value; } }

        public virtual int IndexOf(T item) { return Array.IndexOf(m_data, item, 0, Count); }
        public virtual void Insert(int index, T item)
        {
            var newData = m_data;
            var newMemory = m_memory;
            if (Count + 1 > Capacity)
            {
                int newSize = GetNextCapacitySize(Count);
                newData = new T [newSize];
                newMemory = new Memory<T>(newData);
                if (Capacity > 0)
                    m_memory.Slice(0, Math.Min(index + 1, Count)).CopyTo(newMemory.Slice(0, Math.Min(index + 1, Count)));  // copy everything before index
            }
            if (Capacity > 0 && index < Count)
                m_memory.Slice(index, Count - index).CopyTo(newMemory.Slice(index + 1));  // copy everything after index
            newData[index] = item;
            m_data = newData;
            m_memory = newMemory;
            m_actualLength++;
        }
        public virtual void RemoveAt(int index) { RemoveRange(index, 1); }
        public virtual void Add(T item)
        {
            if (Count + 1 > Capacity)
                Capacity = GetNextCapacitySize(Count);

            m_data[m_actualLength] = item;
            m_actualLength++;
        }
        public virtual void Clear()
        {
            m_data = Array.Empty<T>();
            m_memory = new Memory<T>(m_data);
            m_actualLength = 0;
        }
        public virtual bool Contains(T item) { return IndexOf(item) != -1; }
        public virtual void CopyTo(T[] array, int arrayIndex) { CopyTo(0, array, arrayIndex, Count); }
        public virtual bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index == -1)
                return false;

            RemoveAt(index);
            return true;
        }
        public virtual int Count { get { return m_actualLength; } }
        bool ICollection<T>.IsReadOnly { get { return ((ICollection<T>)m_data).IsReadOnly; } }
        public virtual IEnumerator<T> GetEnumerator() { return new MemoryContainerEnumerator(this); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }


        // List

        public virtual int Capacity
        {
            get { return m_data.Length; }
            set
            {
                if (value < Count)
                    throw new ArgumentOutOfRangeException();

                int newSize = value;
                var newData = new T [newSize];
                var newMemory = new Memory<T>(newData);
                m_memory.CopyTo(newMemory);
                m_data = newData;
                m_memory = newMemory;
            }
        }
        public virtual void CopyTo(int index, T[] array, int arrayIndex, int count) { CopyTo(index, new Span<T>(array, arrayIndex, count), count); }
        public virtual int FindIndex(int startIndex, int count, Predicate<T> match) { return Array.FindIndex(m_data, startIndex, count, match); }
        public virtual int FindIndex(int startIndex, Predicate<T> match) { return Array.FindIndex(m_data, startIndex, Count - startIndex, match); }
        public virtual int FindIndex(Predicate<T> match) { return Array.FindIndex(m_data, 0, Count, match); }
        public virtual int RemoveAll(Predicate<T> match)
        {
            int count = 0;
            int index = Array.FindIndex(m_data, 0, Count, match);
            while (index != -1)
            {
                RemoveAt(index);
                count++;
                index = Array.FindIndex(m_data, 0, Count, match);
            }
            return count;
        }
        public virtual void RemoveRange(int index, int count)
        {
            if (index + count < Count)
                m_memory.Slice(index + count).CopyTo(m_memory.Slice(index));

            m_actualLength -= count;
        }
        public virtual void Sort(Comparison<T> comparison) { Array.Sort(m_data, 0, Count, Comparer<T>.Create(comparison)); }
        public virtual void Sort() { Array.Sort(m_data, 0, Count); }
        public virtual void Sort(IComparer<T> comparer) { Array.Sort(m_data, 0, Count, comparer); }
        public virtual T[] ToArray() { T [] newData = new T [Count]; Array.Copy(m_data, newData, Count); return newData; }


        public T [] data_raw { get { return m_data; } }
        public Memory<T> memory { get { return m_memory; } }


        // UInt64 helper
        public virtual T this[UInt64 index] { get { return m_data[index]; } set { m_data[index] = value; } }


        public bool MemoryEquals(MemoryContainer<T> other) { return m_data == other.m_data; }


        public virtual void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                Add(item);
        }


        public virtual bool CompareTo(MemoryContainer<T> right, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (!this[i].Equals(right[i]))
                    return false;
            }
            return true;
        }


        public virtual void CopyTo(int srcStart, MemoryContainer<T> dest, int destStart, int count)
        {
            CopyTo(srcStart, dest.m_memory.Slice(destStart, count).Span, count);
        }


        public virtual void CopyTo(int srcStart, Span<T> dest, int count)
        {
            m_memory.Slice(srcStart, count).Span.CopyTo(dest);
        }


        public virtual void Resize(int count) { ResizeInternal(count, (T)default); }
        public virtual void Resize(int count, T data)
        {
            assert(typeof(T).GetTypeInfo().IsValueType ? true : (data == null || data.Equals(default)) ? true : false);  // this function doesn't do what you'd expect for ref classes since it doesn't new() for each item.  Manually Add() in this case.
            ResizeInternal(count, data);
        }
        public virtual void Resize(int count, Func<T> creator)
        {
            ResizeInternal(count, creator);
        }


        protected virtual void ResizeInternal(int count, T data)
        {
            int current = Count;
            if (count < current)
            {
                RemoveRange(count, current - count);
            }
            else if (count > current)
            {
                if (count > Capacity)
                    Capacity = count;

                // Short cut for not calling Add() for each element.
                // If Fill() adds checks for Count, then we will need to call a FillInternal() instead.
                Fill(data, Count, count - current);
                m_actualLength = count;
            }
        }

        protected virtual void ResizeInternal(int count, Func<T> creator)
        {
            int current = Count;
            if (count < current)
            {
                RemoveRange(count, current - count);
            }
            else if (count > current)
            {
                if (count > Capacity)
                    Capacity = count;

                for (int i = 0; i < count - current; i++)
                    Add(creator());
            }
        }


        protected virtual void SetInternal(T [] collection)
        {
            m_data = collection;
            m_memory = new Memory<T>(m_data);
            m_actualLength = m_data.Length;
        }


        public virtual void Fill(T value) { Fill(value, 0, Count); }
        public virtual void Fill(T value, int count) { Fill(value, 0, count); }
        public virtual void Fill(T value, int start, int count)
        {
            var valueType = typeof(T).GetTypeInfo().IsValueType ? true : ((value == null || value.Equals(default)) ? true : false);
            if (valueType)
            {
                m_memory.Slice(start, count).Span.Fill(value);
            }
            else
            {
                for (int i = start; i < start + count; i++)
                    this[i] = value;
            }
        }
        public virtual void Fill(Func<T> creator)
        {
            for (int i = 0; i < Count; i++)
                this[i] = creator();
        }


        static int GetNextCapacitySize(int current) { return Math.Min(current + 1024, (current + 1) * 2); }  // cap the growth
    }


    // this attempts to act as a C++ pointer.  It has a MemoryContainer and an offset into that container.
    public class Pointer<T>
    {
        protected MemoryContainer<T> m_memory;
        protected int m_offset;


        public Pointer() { }
        public Pointer(MemoryContainer<T> memory, int offset = 0) { m_memory = memory; m_offset = offset; }
        public Pointer(Pointer<T> pointer, int offset = 0) : this(pointer.m_memory, pointer.m_offset + offset) { }


        public virtual MemoryContainer<T> Buffer { get { return m_memory; } }
        public virtual int Offset { get { return m_offset; } }
        public virtual int Count { get { return m_memory.Count; } }


        public virtual T this[int i] { get { return m_memory[m_offset + i]; } set { m_memory[m_offset + i] = value; } }
        public virtual T this[UInt64 i] { get { return m_memory[m_offset + (int)i]; } set { m_memory[m_offset + (int)i] = value; } }


        public static Pointer<T> operator +(Pointer<T> left, int right) { return new Pointer<T>(left, right); }
        public static Pointer<T> operator +(Pointer<T> left, UInt32 right) { return new Pointer<T>(left, (int)right); }
        public static Pointer<T> operator +(Pointer<T> left, Pointer<T> right) { if (!left.Buffer.MemoryEquals(right.Buffer)) return null; return new Pointer<T>(left.Buffer, left.Offset + right.Offset); }
        public static Pointer<T> operator ++(Pointer<T> left) { left.m_offset++; return left; }
        public static Pointer<T> operator -(Pointer<T> left, int right) { return new Pointer<T>(left, -right); }
        public static Pointer<T> operator -(Pointer<T> left, UInt32 right) { return new Pointer<T>(left, -(int)right); }
        public static Pointer<T> operator -(Pointer<T> left, Pointer<T> right) { if (!left.Buffer.MemoryEquals(right.Buffer)) return null; return new Pointer<T>(left.Buffer, left.Offset - right.Offset); }
        public static Pointer<T> operator --(Pointer<T> left) { left.m_offset--; return left; }


        public virtual T op { get { return this[0]; } set { this[0] = value; } }


        public virtual bool CompareTo(Pointer<T> right, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (!this[i].Equals(right[i]))
                    return false;
            }
            return true;
        }

        public virtual void CopyTo(int srcStart, Pointer<T> dest, int destStart, int count) { m_memory.CopyTo(m_offset + srcStart, dest.Buffer, dest.m_offset + destStart, count); }
        public virtual void CopyTo(int srcStart, Span<T> span, int count) { m_memory.CopyTo(srcStart, span, count); }

        public virtual void Fill(T value, int count) { Fill(value, 0, count); }
        public virtual void Fill(T value, int start, int count) { m_memory.Fill(value, m_offset + start, count); }
    }


    // this class holds a Pointer reference so that if the pointer changes, this class will track it.
    public class PointerRef<T>
    {
        public Pointer<T> m_pointer;

        public PointerRef() { }
        public PointerRef(Pointer<T> pointer) { m_pointer = pointer; }
    }


    public static class PointerU8Extension
    {
        //public static PointerU8 operator +(PointerU8 left, int right) { return new PointerU8(left, right); }
        //public static PointerU8 operator +(PointerU8 left, UInt32 right) { return new PointerU8(left, (int)right); }
        //public static PointerU8 operator ++(PointerU8 left) { left.m_offset++; return left; }
        //public static PointerU8 operator -(PointerU8 left, int right) { return new PointerU8(left, -right); }
        //public static PointerU8 operator -(PointerU8 left, UInt32 right) { return new PointerU8(left, -(int)right); }
        //public static PointerU8 operator --(PointerU8 left) { left.m_offset--; return left; }

        // offset parameter is based on unit size for each function, not based on byte size.  eg, SetUInt16 parameter offset16 is in uint16 units
        public static void SetUInt16(this Pointer<byte> pointer, int offset16, UInt16 value) { pointer.Buffer.SetUInt16Offs8(pointer.Offset + (offset16 << 1), value); }
        public static void SetUInt32(this Pointer<byte> pointer, int offset32, UInt32 value) { pointer.Buffer.SetUInt32Offs8(pointer.Offset + (offset32 << 2), value); }
        public static void SetUInt64(this Pointer<byte> pointer, int offset64, UInt64 value) { pointer.Buffer.SetUInt64Offs8(pointer.Offset + (offset64 << 3), value); }

        // offset parameter is based on unit size for each function, not based on byte size.  eg, GetUInt16 parameter offset16 is in uint16 units
        public static UInt16 GetUInt16(this Pointer<byte> pointer, int offset16) { return pointer.Buffer.GetUInt16Offs8(pointer.Offset + (offset16 << 1)); }
        public static UInt32 GetUInt32(this Pointer<byte> pointer, int offset32) { return pointer.Buffer.GetUInt32Offs8(pointer.Offset + (offset32 << 2)); }
        public static UInt64 GetUInt64(this Pointer<byte> pointer, int offset64) { return pointer.Buffer.GetUInt64Offs8(pointer.Offset + (offset64 << 3)); }

        public static bool CompareTo(this Pointer<byte> pointer, string compareTo) { return pointer.CompareTo(0, compareTo); }
        public static bool CompareTo(this Pointer<byte> pointer, int startOffset, string compareTo) { return pointer.CompareTo(startOffset, compareTo.ToCharArray()); }
        public static bool CompareTo(this Pointer<byte> pointer, int startOffset, char [] compareTo) { return pointer.Buffer.CompareTo(startOffset, compareTo); }

        public static string ToString(this Pointer<byte> pointer, int length) { return pointer.ToString(length); }
        public static string ToString(this Pointer<byte> pointer, int startOffset, int length) { return pointer.Buffer.ToString(startOffset, length); }
    }


    // use these derived pointer classes when you need to do operations on a Pointer with a different base type
    // eg, note the [] overloads use the type it designates.
    // note the get/set functions for operating on the data as if it's a different type.
    // see PointerU16, PointerU32, PointerU64
    public class PointerU16 : Pointer<byte>
    {
        public PointerU16() : base() { }
        public PointerU16(MemoryContainer<byte> list, int offset16 = 0) : base(list, offset16 << 1) { }
        public PointerU16(Pointer<byte> listPtr, int offset16 = 0) : base(listPtr, offset16 << 1) { }

        public new UInt16 this[int i] { get { return this.GetUInt16(i); } set { this.SetUInt16(i, value); } }
        public new UInt16 this[UInt64 i] { get { return this.GetUInt16((int)i); } set { this.SetUInt16((int)i, value); } }

        public static PointerU16 operator +(PointerU16 left, int right) { return new PointerU16(left, right); }
        public static PointerU16 operator +(PointerU16 left, UInt32 right) { return new PointerU16(left, (int)right); }
        public static PointerU16 operator ++(PointerU16 left) { left.m_offset += 2; return left; }
        public static PointerU16 operator -(PointerU16 left, int right) { return new PointerU16(left, -right); }
        public static PointerU16 operator -(PointerU16 left, UInt32 right) { return new PointerU16(left, -(int)right); }
        public static PointerU16 operator --(PointerU16 left) { left.m_offset -= 2; return left; }

        public new UInt16 op { get { return this[0]; } set { this[0] = value; } }

        public void Fill(UInt16 value, int count) { Fill(value, 0, count); }
        public void Fill(UInt16 value, int start, int count) { for (int i = start; i < start + count; i++) this[i] = value; }
    }

    public class PointerU32 : Pointer<byte>
    {
        public PointerU32() : base() { }
        public PointerU32(MemoryContainer<byte> list, int offset32 = 0) : base(list, offset32 << 2) { }
        public PointerU32(Pointer<byte> listPtr, int offset32 = 0) : base(listPtr, offset32 << 2) { }

        public new UInt32 this[int i] { get { return this.GetUInt32(i); } set { this.SetUInt32(i, value); } }
        public new UInt32 this[UInt64 i] { get { return this.GetUInt32((int)i); } set { this.SetUInt32((int)i, value); } }

        public static PointerU32 operator +(PointerU32 left, int right) { return new PointerU32(left, right); }
        public static PointerU32 operator +(PointerU32 left, UInt32 right) { return new PointerU32(left, (int)right); }
        public static PointerU32 operator ++(PointerU32 left) { left.m_offset += 4; return left; }
        public static PointerU32 operator -(PointerU32 left, int right) { return new PointerU32(left, -right); }
        public static PointerU32 operator -(PointerU32 left, UInt32 right) { return new PointerU32(left, -(int)right); }
        public static PointerU32 operator --(PointerU32 left) { left.m_offset -= 4; return left; }

        public new UInt32 op { get { return this[0]; } set { this[0] = value; } }

        public void Fill(UInt32 value, int count) { Fill(value, 0, count); }
        public void Fill(UInt32 value, int start, int count) { for (int i = start; i < start + count; i++) this[i] = value; }
    }

    public class PointerU64 : Pointer<byte>
    {
        public PointerU64() : base() { }
        public PointerU64(MemoryContainer<byte> list, int offset64 = 0) : base(list, offset64 << 3) { }
        public PointerU64(Pointer<byte> listPtr, int offset64 = 0) : base(listPtr, offset64 << 3) { }

        public new UInt64 this[int i] { get { return this.GetUInt64(i); } set { this.SetUInt64(i, value); } }
        public new UInt64 this[UInt64 i] { get { return this.GetUInt64((int)i); } set { this.SetUInt64((int)i, value); } }

        public static PointerU64 operator +(PointerU64 left, int right) { return new PointerU64(left, right); }
        public static PointerU64 operator +(PointerU64 left, UInt32 right) { return new PointerU64(left, (int)right); }
        public static PointerU64 operator ++(PointerU64 left) { left.m_offset += 8; return left; }
        public static PointerU64 operator -(PointerU64 left, int right) { return new PointerU64(left, -right); }
        public static PointerU64 operator -(PointerU64 left, UInt32 right) { return new PointerU64(left, -(int)right); }
        public static PointerU64 operator --(PointerU64 left) { left.m_offset -= 8; return left; }

        public new UInt64 op { get { return this[0]; } set { this[0] = value; } }

        public void Fill(UInt64 value, int count) { Fill(value, 0, count); }
        public void Fill(UInt64 value, int start, int count) { for (int i = start; i < start + count; i++) this[i] = value; }
    }


    // the usage of this should be when you have a flat piece of memory and intend to work on it with different types
    // eg, a byte array that you cast to UInt32 and do operations.
    // See PointerU8, PointerU16, etc
    public static class MemoryU8Extension
    {
        public static bool CompareTo(this MemoryContainer<byte> container, int startOffset, string compareTo) { return container.CompareTo(startOffset, compareTo.ToCharArray()); }
        public static bool CompareTo(this MemoryContainer<byte> container, int startOffset, char [] compareTo)
        {
            for (int i = 0; i < compareTo.Length; i++)
            {
                if (container[i] != compareTo[i])
                    return false;
            }
            return true;
        }

        public static string ToString(this MemoryContainer<byte> container, int startOffset, int length)
        {
            string s = "";
            for (int i = startOffset; i < startOffset + length; i++)
            {
                s += container[i];
            }

            return s;
        }


        public static int IndexOf(this MemoryContainer<byte> container, int startOffset, int endOffset, byte compare)
        {
            for (int i = startOffset; i < startOffset + endOffset; i++)
            {
                if (container[i] == compare)
                    return i;
            }

            return endOffset;
        }


        public static void SetUInt16(this MemoryContainer<byte> container, int offset16, UInt16 value) { container.SetUInt16Offs8(offset16 << 1, value); }
        public static void SetUInt32(this MemoryContainer<byte> container, int offset32, UInt32 value) { container.SetUInt32Offs8(offset32 << 2, value); }
        public static void SetUInt64(this MemoryContainer<byte> container, int offset64, UInt64 value) { container.SetUInt64Offs8(offset64 << 3, value); }

        public static void SetUInt16Offs8(this MemoryContainer<byte> container, int offset8, UInt16 value)
        {
            assert_slow(offset8 < container.Count);
#if MEMORY_BYTE_USE_BINARY_PRIMITIVES
            var span = container.memory.Slice(offset8, 2).Span;
            BinaryPrimitives.WriteUInt16LittleEndian(span, value);
#else
            var bytes = BitConverter.GetBytes(value);
#if MEMORY_BYTE_USE_ARRAY_COPY
            Array.Copy(bytes, 0, container.data, offset8, 2);
#else
            container.data_raw[offset8 + 1] = bytes[1];
            container.data_raw[offset8]     = bytes[0];
#endif
#endif
        }

        public static void SetUInt32Offs8(this MemoryContainer<byte> container, int offset8, UInt32 value)
        {
            assert_slow(offset8 < container.Count);
#if MEMORY_BYTE_USE_BINARY_PRIMITIVES
            var span = container.memory.Slice(offset8, 4).Span;
            BinaryPrimitives.WriteUInt32LittleEndian(span, value);
#else
            var bytes = BitConverter.GetBytes(value);
#if MEMORY_BYTE_USE_ARRAY_COPY
            Array.Copy(bytes, 0, container.data, offset8, 4);
#else
            container.data_raw[offset8 + 3] = bytes[3];
            container.data_raw[offset8 + 2] = bytes[2];
            container.data_raw[offset8 + 1] = bytes[1];
            container.data_raw[offset8] = bytes[0];
#endif
#endif
        }

        public static void SetUInt64Offs8(this MemoryContainer<byte> container, int offset8, UInt64 value)
        {
            assert_slow(offset8 < container.Count);
#if MEMORY_BYTE_USE_BINARY_PRIMITIVES
            var span = container.memory.Slice(offset8, 8).Span;
            BinaryPrimitives.WriteUInt64LittleEndian(span, value);
#else
            var bytes = BitConverter.GetBytes(value);
#if MEMORY_BYTE_USE_ARRAY_COPY
            Array.Copy(bytes, 0, container.data, offset8, 8);
#else
            container.data_raw[offset8 + 7] = bytes[7];
            container.data_raw[offset8 + 6] = bytes[6];
            container.data_raw[offset8 + 5] = bytes[5];
            container.data_raw[offset8 + 4] = bytes[4];
            container.data_raw[offset8 + 3] = bytes[3];
            container.data_raw[offset8 + 2] = bytes[2];
            container.data_raw[offset8 + 1] = bytes[1];
            container.data_raw[offset8]     = bytes[0];
#endif
#endif
        }

        public static UInt16 GetUInt16(this MemoryContainer<byte> container, int offset16 = 0) { return container.GetUInt16Offs8(offset16 << 1); }
        public static UInt32 GetUInt32(this MemoryContainer<byte> container, int offset32 = 0) { return container.GetUInt32Offs8(offset32 << 2); }
        public static UInt64 GetUInt64(this MemoryContainer<byte> container, int offset64 = 0) { return container.GetUInt64Offs8(offset64 << 3); }

        public static UInt16 GetUInt16Offs8(this MemoryContainer<byte> container, int offset8 = 0)
        {
            assert_slow(offset8 < container.Count);
#if MEMORY_BYTE_USE_BINARY_PRIMITIVES
            var span = container.memory.Slice(offset8, 2).Span;
            return BinaryPrimitives.ReadUInt16LittleEndian(span);
#else
            return BitConverter.ToUInt16(container.data_raw, offset8);
#endif
        }

        public static UInt32 GetUInt32Offs8(this MemoryContainer<byte> container, int offset8 = 0)
        {
            assert_slow(offset8 < container.Count);
#if MEMORY_BYTE_USE_BINARY_PRIMITIVES
            var span = container.memory.Slice(offset8, 4).Span;
            return BinaryPrimitives.ReadUInt32LittleEndian(span);
#else
            return BitConverter.ToUInt32(container.data_raw, offset8);
#endif
        }

        public static UInt64 GetUInt64Offs8(this MemoryContainer<byte> container, int offset8 = 0)
        {
            assert_slow(offset8 < container.Count);
#if MEMORY_BYTE_USE_BINARY_PRIMITIVES
            var span = container.memory.Slice(offset8, 8).Span;
            return BinaryPrimitives.ReadUInt64LittleEndian(span);
#else
            return BitConverter.ToUInt64(container.data_raw, offset8);
#endif
        }
    }


    // manual boxing class of an int
    public class intref
    {
        int m_value;

        public intref() { }
        public intref(int i) { m_value = i; }

        public int i { get { return m_value; } set { m_value = value; } }

        // these might cause trouble, by replacing the ref, which is what we want to avoid
        //public static implicit operator int(intref x) { return x.get(); }
        //public static implicit operator intref(int x) { return new intref(x); }
    }


    // manual boxing class of a double
    public class doubleref
    {
        double m_value;

        public doubleref() { }
        public doubleref(double value) { m_value = value; }

        public double d { get { return m_value; } set { m_value = value; } }

        // these might cause trouble, by replacing the ref, which is what we want to avoid
        //public static implicit operator int(intref x) { return x.get(); }
        //public static implicit operator intref(int x) { return new intref(x); }
    }


    // a flexible primitive type, used when the return is unknown (eg with templates)
    // the size is set at construction.  See uX and devcb_value
    public struct FlexPrim
    {
        Type m_type;

        // TODO - use u128 here once native type is available
        BigInteger m_value;


        public FlexPrim(Type type) : this(type, 0U) { }
        public FlexPrim(Type type, u32 value) : this(type, (u64)value) { }
        public FlexPrim(Type type, u64 value) : this(type, new BigInteger(value)) { }
        public FlexPrim(Type type, s64 value) : this(type, new BigInteger(value)) { }
        public FlexPrim(Type type, FlexPrim other) : this(type, other.Get) { }
        public FlexPrim(FlexPrim other) : this(other.m_type, other) { }
        public FlexPrim(int width) : this(width, 0U) { }
        public FlexPrim(int width, u64 value) : this(WidthToType(width), value) { }
        public FlexPrim(int width, FlexPrim other) : this(WidthToType(width), other) { }

        FlexPrim(Type type, BigInteger value)
        {
            m_type = type;
            m_value = value;
        }


        public Type type { get { return m_type; } }
        public int width { get { return TypeToWidth(type); } }

        // cast to appropriate type, but return largest type
        BigInteger Get
        {
            get
            {
                if      (m_type == typeof(u8))  return (u8)(m_value & u8.MaxValue);
                else if (m_type == typeof(u16)) return (u16)(m_value & u16.MaxValue);
                else if (m_type == typeof(u32)) return (u32)(m_value & u32.MaxValue);
                else if (m_type == typeof(u64)) return (u64)(m_value & u64.MaxValue);
                else if (m_type == typeof(s8))  return (s8)(m_value & s8.MaxValue);
                else if (m_type == typeof(s16)) return (s16)(m_value & s16.MaxValue);
                else if (m_type == typeof(s32)) return (s32)(m_value & s32.MaxValue);
                else if (m_type == typeof(s64)) return (s64)(m_value & s64.MaxValue);
                else throw new emu_unimplemented();
            }
        }


        // cast to appropriate type, and return appropriate type
        public u8 u8 { get { return (u8)Get; } }
        public u16 u16 { get { return (u16)Get; } }
        public u32 u32 { get { return (u32)Get; } }
        public u64 u64 { get { return (u64)Get; } }
        public s8 s8 { get { return (s8)Get; } }
        public s16 s16 { get { return (s16)Get; } }
        public s32 s32 { get { return (s32)Get; } }
        public s64 s64 { get { return (s64)Get; } }


        public bool IsUnsigned() { return IsUnsigned(m_type); }
        public int TypeToWidth() { return TypeToWidth(m_type); }


        public override bool Equals(Object obj)
        {
            if (obj == null || base.GetType() != obj.GetType()) return false;
            return this == (FlexPrim)obj;
        }
        public override int GetHashCode() { return m_type.GetHashCode() ^ m_value.GetHashCode(); }


        public override string ToString() { return string.Format("type: {0} Get: {1}", type, Get); }


        public static Type make_unsigned(Type type)
        {
            if      (type == typeof(u8)) return typeof(u8);
            else if (type == typeof(u16)) return typeof(u16);
            else if (type == typeof(u32)) return typeof(u32);
            else if (type == typeof(s32)) return typeof(u32);
            else throw new emu_unimplemented();
        }


        public static bool operator ==(FlexPrim left, FlexPrim right) { return left.Get == right.Get; }
        public static bool operator ==(FlexPrim left, u64 right) { return left.Get == right; }
        public static bool operator ==(FlexPrim left, u32 right) { return left.Get == right; }
        public static bool operator ==(FlexPrim left, s64 right) { return left.Get == right; }
        public static bool operator !=(FlexPrim left, FlexPrim right) { return left.Get != right.Get; }
        public static bool operator !=(FlexPrim left, u64 right) { return left.Get != right; }
        public static bool operator !=(FlexPrim left, u32 right) { return left.Get != right; }
        public static bool operator !=(FlexPrim left, s64 right) { return left.Get != right; }

        public static FlexPrim operator +(FlexPrim left, FlexPrim right) { return new FlexPrim(left.m_type, left.Get + right.Get); }
        public static FlexPrim operator +(FlexPrim left, u64 right) { return new FlexPrim(left.m_type, left.Get + right); }
        public static FlexPrim operator +(FlexPrim left, s64 right) { return new FlexPrim(left.m_type, left.Get + right); }

        public static FlexPrim operator >>(FlexPrim left, int right) { return new FlexPrim(left.m_type, left.Get >> right); }
        public static FlexPrim operator <<(FlexPrim left, int right) { return new FlexPrim(left.m_type, left.Get << right); }
        public static FlexPrim operator &(FlexPrim left, FlexPrim right) { return new FlexPrim(left.m_type, left.Get & right.Get); }
        public static FlexPrim operator &(FlexPrim left, u64 right) { return new FlexPrim(left.m_type, left.Get & right); }
        public static FlexPrim operator &(FlexPrim left, s64 right) { return new FlexPrim(left.m_type, left.Get & right); }
        public static FlexPrim operator |(FlexPrim left, FlexPrim right) { return new FlexPrim(left.m_type, left.Get | right.Get); }
        public static FlexPrim operator |(FlexPrim left, u64 right) { return new FlexPrim(left.m_type, left.Get | right); }
        public static FlexPrim operator |(FlexPrim left, s64 right) { return new FlexPrim(left.m_type, left.Get | right); }
        public static FlexPrim operator ^(FlexPrim left, FlexPrim right) { return new FlexPrim(left.m_type, left.Get ^ right.Get); }
        public static FlexPrim operator ^(FlexPrim left, u64 right) { return new FlexPrim(left.m_type, left.Get ^ right); }
        public static FlexPrim operator ^(FlexPrim left, s64 right) { return new FlexPrim(left.m_type, left.Get ^ right); }

        public static FlexPrim operator ~(FlexPrim left) { return new FlexPrim(left.m_type, ~left.Get); }


        public static bool IsUnsigned(FlexPrim value) { return IsUnsigned(value.m_type); }

        public static bool IsUnsigned(Type type)
        {
            if (type == typeof(u8) ||
                type == typeof(u16) ||
                type == typeof(u32) ||
                type == typeof(u64))
                return true;
            else if (type == typeof(s8) ||
                     type == typeof(s16) ||
                     type == typeof(s32) ||
                     type == typeof(s64))
                return false;
            else
                throw new emu_unimplemented();
        }


        public static FlexPrim MaxValue(Type type) { return new FlexPrim(type, u64.MaxValue); }
        public static FlexPrim MaxValue(int width) { return new FlexPrim(width, u64.MaxValue); }


        public static Type WidthToType(int width)
        {
            switch (width)
            {
                case 0: return typeof(u8);
                case 1: return typeof(u16);
                case 2: return typeof(u32);
                case 3: return typeof(u64);
                default: throw new emu_unimplemented();
            }
        }

        public static int TypeToWidth(Type type)
        {
            if      (type == typeof(u8)) return 0;
            else if (type == typeof(u16)) return 1;
            else if (type == typeof(u32)) return 2;
            else if (type == typeof(u64)) return 3;
            else throw new emu_unimplemented();
        }
    }
}
