// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using pen_t = System.UInt32;
using s8 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using stream_sample_t = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;


namespace mame
{
    // pen_t is used to represent pixel values in bitmaps
    //typedef UInt32 pen_t;

    // stream_sample_t is used to represent a single sample in a sound stream
    //typedef INT32 stream_sample_t;


    // constants for expression endianness
    public enum endianness_t
    {
        ENDIANNESS_LITTLE,
        ENDIANNESS_BIG
    }


    public class emucore_global
    {
        //extern const char *const endianness_names[2];
        public static readonly string [] endianness_names = { "little", "big" };


        // declare native endianness to be one or the other
        //#ifdef LSB_FIRST
        public const endianness_t ENDIANNESS_NATIVE = endianness_t.ENDIANNESS_LITTLE;
        //#else
        //const endianness_t ENDIANNESS_NATIVE = ENDIANNESS_BIG;
        //#endif


        // orientation of bitmaps
        public const UInt32 ORIENTATION_FLIP_X              = 0x0001;  // mirror everything in the X direction
        public const UInt32 ORIENTATION_FLIP_Y              = 0x0002;  // mirror everything in the Y direction
        public const UInt32 ORIENTATION_SWAP_XY             = 0x0004;  // mirror along the top-left/bottom-right diagonal

        public const UInt32 ROT0                            = 0;
        public const UInt32 ROT90                           = ORIENTATION_SWAP_XY | ORIENTATION_FLIP_X;  // rotate clockwise 90 degrees
        public const UInt32 ROT180                          = ORIENTATION_FLIP_X | ORIENTATION_FLIP_Y;   // rotate 180 degrees
        public const UInt32 ROT270                          = ORIENTATION_SWAP_XY | ORIENTATION_FLIP_Y;  // rotate counter-clockwise 90 degrees


        //**************************************************************************
        //  COMMON MACROS
        //**************************************************************************

        // macro for defining a copy constructor and assignment operator to prevent copying
        //define DISABLE_COPYING(_Type) \
        //private: \
        //    _Type(const _Type &); \
        //    _Type &operator=(const _Type &)

        //// macro for declaring enumeration operators that increment/decrement like plain old C
        //#define DECLARE_ENUM_INCDEC_OPERATORS(TYPE) \
        //inline TYPE &operator++(TYPE &value) { return value = TYPE(std::underlying_type_t<TYPE>(value) + 1); } \
        //inline TYPE &operator--(TYPE &value) { return value = TYPE(std::underlying_type_t<TYPE>(value) - 1); } \
        //inline TYPE operator++(TYPE &value, int) { TYPE const old(value); ++value; return old; } \
        //inline TYPE operator--(TYPE &value, int) { TYPE const old(value); --value; return old; }

        // macro for declaring bitwise operators for an enumerated type
        //#define DECLARE_ENUM_BITWISE_OPERATORS(TYPE) \
        //constexpr TYPE operator~(TYPE value) { return TYPE(~std::underlying_type_t<TYPE>(value)); } \
        //constexpr TYPE operator&(TYPE a, TYPE b) { return TYPE(std::underlying_type_t<TYPE>(a) & std::underlying_type_t<TYPE>(b)); } \
        //constexpr TYPE operator|(TYPE a, TYPE b) { return TYPE(std::underlying_type_t<TYPE>(a) | std::underlying_type_t<TYPE>(b)); } \
        //inline TYPE &operator&=(TYPE &a, TYPE b) { return a = a & b; } \
        //inline TYPE &operator|=(TYPE &a, TYPE b) { return a = a | b; }

        // this macro passes an item followed by a string version of itself as two consecutive parameters
        //#define NAME(x) x, #x

        // this macro wraps a function 'x' and can be used to pass a function followed by its name
        //#define FUNC(x) &x, #x


        // standard assertion macros
        //#undef assert_always

        //#if defined(MAME_DEBUG_FAST)
        //#define assert_always(x, msg)   do { if (!(x)) throw emu_fatalerror("Fatal error: %s\nCaused by assert: %s:%d: %s", msg, __FILE__, __LINE__, #x); } while (0)
        //#elif defined(MAME_DEBUG)
        //#define assert_always(x, msg)   do { if (!(x)) throw emu_fatalerror("Fatal error: %s\nCaused by assert: %s:%d: %s", msg, __FILE__, __LINE__, #x); } while (0)
        //#else
        //#define assert_always(x, msg)   do { if (!(x)) throw emu_fatalerror("Fatal error: %s (%s:%d)", msg, __FILE__, __LINE__); } while (0)
        //#endif

        [Conditional("DEBUG")]
        public static void assert(bool condition)
        {
            Debug.Assert(condition);

            // Trace asserts are enabled in Release builds, unlike Debug.Assert()
#if false
            Trace.Assert(condition);
#endif
        }

        public static void assert_always(bool condition, string message)
        {
            // Trace asserts are enabled in Release builds, unlike Debug.Assert()
            Trace.Assert(condition, message);
        }


        // macros to convert radians to degrees and degrees to radians
        //template <typename T> constexpr auto RADIAN_TO_DEGREE(T const &x) { return (180.0 / M_PI) * x; }
        //template <typename T> constexpr auto DEGREE_TO_RADIAN(T const &x) { return (M_PI / 180.0) * x; }


        // endian-based value: first value is if 'endian' is little-endian, second is if 'endian' is big-endian
        //#define ENDIAN_VALUE_LE_BE(endian,leval,beval)  (((endian) == ENDIANNESS_LITTLE) ? (leval) : (beval))

        // endian-based value: first value is if native endianness is little-endian, second is if native is big-endian
        //#define NATIVE_ENDIAN_VALUE_LE_BE(leval,beval)  ENDIAN_VALUE_LE_BE(ENDIANNESS_NATIVE, leval, beval)

        // endian-based value: first value is if 'endian' matches native, second is if 'endian' doesn't match native
        //#define ENDIAN_VALUE_NE_NNE(endian,neval,nneval) (((endian) == ENDIANNESS_NATIVE) ? (neval) : (nneval))
    }


    [StructLayout(LayoutKind.Explicit)]
    public struct PAIR_B
    {
        [FieldOffset(0)]
        public u8 l;
        [FieldOffset(1)]
        public u8 h;
        [FieldOffset(2)]
        public u8 h2;
        [FieldOffset(3)]
        public u8 h3;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct PAIR_W
    {
        [FieldOffset(0)]
        public u16 l;
        [FieldOffset(2)]
        public u16 h;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct PAIR_SB
    {
        [FieldOffset(0)]
        public s8 l;
        [FieldOffset(1)]
        public s8 h;
        [FieldOffset(2)]
        public s8 h2;
        [FieldOffset(3)]
        public s8 h3;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct PAIR_SW
    {
        [FieldOffset(0)]
        public s16 l;
        [FieldOffset(2)]
        public s16 h;
    }

    // PAIR is an endian-safe union useful for representing 32-bit CPU registers
    [StructLayout(LayoutKind.Explicit)]
    public struct PAIR
    {
        //#ifdef LSB_FIRST
        [FieldOffset(0)]
        public PAIR_B b;    //struct { UINT8 l,h,h2,h3; } b;
        [FieldOffset(0)]
        public PAIR_W w;    //struct { UINT16 l,h; } w;
        [FieldOffset(0)]
        public PAIR_SB sb;  //struct { INT8 l,h,h2,h3; } sb;
        [FieldOffset(0)]
        public PAIR_SW sw;  //struct { INT16 l,h; } sw;
        //#else
        //struct { UINT8 h3,h2,h,l; } b;
        //struct { INT8 h3,h2,h,l; } sb;
        //struct { UINT16 h,l; } w;
        //struct { INT16 h,l; } sw;
        //#endif

        [FieldOffset(0)]
        public u32 d;

        [FieldOffset(0)]
        public s32 sd;
    }


    // emu_exception is the base class for all emu-related exceptions
    public class emu_exception : Exception
    {
    }


    // emu_fatalerror is a generic fatal exception that provides an error string
    public class emu_fatalerror : emu_exception
    {
        string text;
        int code;


        public emu_fatalerror(string format, params object [] args)
            : this(0, format, args)
        {
        }

        public emu_fatalerror(int exitcode, string format, params object [] args)
        {
            code = exitcode;
            text = string.Format(format, args);

            string error = string.Format("emu_fatalerror: {0}code: {1}\n", text, code);
            osdcore_global.m_osdcore.osd_break_into_debugger(error);
        }


        public string str() { return text; }
        public int exitcode() { return code; }
    }


    class tag_add_exception : emu_exception
    {
        string m_tag;

        public tag_add_exception(string tag) : base() { m_tag = tag; }
        string tag() { return m_tag.c_str(); }
    }


    public class emu_unimplemented : emu_fatalerror { public emu_unimplemented() : base("Unimplemented") { } }
}
