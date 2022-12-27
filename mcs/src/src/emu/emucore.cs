// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

using endianness_t = mame.util.endianness;  //using endianness_t = util::endianness;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using s8 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;

using static mame.emucore_global;
using static mame.osdcore_global;


namespace mame
{
    // explicitly sized integers
    //using osd::u8;
    //using osd::u16;
    //using osd::u32;
    //using osd::u64;
    //using osd::s8;
    //using osd::s16;
    //using osd::s32;
    //using osd::s64;

    // useful utility functions
    //using util::underlying_value;
    //using util::enum_value;
    //using util::make_bitmask;
    //using util::BIT;
    //using util::bitswap;
    //using util::iabs;
    //using util::string_format;

    //using endianness_t = util::endianness;

    public interface endianness_t_const { endianness_t value { get; } }
    public class endianness_t_const_ENDIANNESS_LITTLE : endianness_t_const { public endianness_t value { get { return ENDIANNESS_LITTLE; } } }
    public class endianness_t_const_ENDIANNESS_BIG : endianness_t_const { public endianness_t value { get { return ENDIANNESS_BIG; } } }


    //using util::BYTE_XOR_BE;
    //using util::BYTE_XOR_LE;
    //using util::BYTE4_XOR_BE;
    //using util::BYTE4_XOR_LE;
    //using util::WORD_XOR_BE;
    //using util::WORD_XOR_LE;
    //using util::BYTE8_XOR_BE;
    //using util::BYTE8_XOR_LE;
    //using util::WORD2_XOR_BE;
    //using util::WORD2_XOR_LE;
    //using util::DWORD_XOR_BE;
    //using util::DWORD_XOR_LE;


    // pen_t is used to represent pixel values in bitmaps
    //typedef u32 pen_t;


    [StructLayout(LayoutKind.Explicit)]
    public struct PAIR_B
    {
        [FieldOffset(0)] public u8 l;
        [FieldOffset(1)] public u8 h;
        [FieldOffset(2)] public u8 h2;
        [FieldOffset(3)] public u8 h3;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct PAIR_W
    {
        [FieldOffset(0)] public u16 l;
        [FieldOffset(2)] public u16 h;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct PAIR_SB
    {
        [FieldOffset(0)] public s8 l;
        [FieldOffset(1)] public s8 h;
        [FieldOffset(2)] public s8 h2;
        [FieldOffset(3)] public s8 h3;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct PAIR_SW
    {
        [FieldOffset(0)] public s16 l;
        [FieldOffset(2)] public s16 h;
    }

    // PAIR is an endian-safe union useful for representing 32-bit CPU registers
    [StructLayout(LayoutKind.Explicit)]
    public struct PAIR  //union PAIR
    {
        //#ifdef LSB_FIRST
        [FieldOffset(0)] public PAIR_B b;    //struct { u8 l,h,h2,h3; } b;
        [FieldOffset(0)] public PAIR_W w;    //struct { u16 l,h; } w;
        [FieldOffset(0)] public PAIR_SB sb;  //struct { s8 l,h,h2,h3; } sb;
        [FieldOffset(0)] public PAIR_SW sw;  //struct { s16 l,h; } sw;
        //#else
        //struct { u8 h3,h2,h,l; } b;
        //struct { s8 h3,h2,h,l; } sb;
        //struct { u16 h,l; } w;
        //struct { s16 h,l; } sw;
        //#endif

        [FieldOffset(0)] public u32 d;
        [FieldOffset(0)] public s32 sd;
    }


    [StructLayout(LayoutKind.Explicit)]
    public struct PAIR16_B
    {
        [FieldOffset(0)] public u8 l;
        [FieldOffset(1)] public u8 h;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct PAIR16_SB
    {
        [FieldOffset(0)] public s8 l;
        [FieldOffset(1)] public s8 h;
    }

    // PAIR16 is a 16-bit extension of a PAIR
    [StructLayout(LayoutKind.Explicit)]
    public struct PAIR16  //union PAIR16
    {
        //#ifdef LSB_FIRST
        [FieldOffset(0)] public PAIR16_B  b;   //struct { u8 l,h; } b;
        [FieldOffset(0)] public PAIR16_SB sb;  //struct { s8 l,h; } sb;
        //#else
        //struct { u8 h,l; } b;
        //struct { s8 h,l; } sb;
        //#endif

        [FieldOffset(0)] public u16 w;
        [FieldOffset(0)] public s16 sw;
    }


    // PAIR64 is a 64-bit extension of a PAIR
    //union PAIR64


    public static partial class emucore_global
    {
        //**************************************************************************
        //  COMMON CONSTANTS
        //**************************************************************************

        public const endianness_t ENDIANNESS_LITTLE = util.endianness.little;
        public const endianness_t ENDIANNESS_BIG    = util.endianness.big;
        public const endianness_t ENDIANNESS_NATIVE = util.endianness.native;


        // M_PI is not part of the C/C++ standards and is not present on
        // strict ANSI compilers or when compiling under GCC with -ansi
        //#ifndef M_PI
        //#define M_PI                            3.14159265358979323846
        //#endif


        /// \name Image orientation flags
        /// \{

        public const int ORIENTATION_FLIP_X     = 0x0001;  ///< Mirror horizontally (in the X direction)
        public const int ORIENTATION_FLIP_Y     = 0x0002;  ///< Mirror vertically (in the Y direction)
        public const int ORIENTATION_SWAP_XY    = 0x0004;  ///< Mirror along the top-left/bottom-right diagonal

        public const int ROT0                   = 0;
        public const int ROT90                  = ORIENTATION_SWAP_XY | ORIENTATION_FLIP_X;  ///< Rotate 90 degrees clockwise
        public const int ROT180                 = ORIENTATION_FLIP_X | ORIENTATION_FLIP_Y;   ///< Rotate 180 degrees
        public const int ROT270                 = ORIENTATION_SWAP_XY | ORIENTATION_FLIP_Y;  ///< Rotate 90 degrees anti-clockwise (270 degrees clockwise)

        /// \}


        // these are UTF-8 encoded strings for common characters
        //#define UTF8_NBSP               "\xc2\xa0"          /* non-breaking space */

        //#define UTF8_MULTIPLY           "\xc3\x97"          /* multiplication sign */
        //#define UTF8_DIVIDE             "\xc3\xb7"          /* division sign */
        //#define UTF8_SQUAREROOT         "\xe2\x88\x9a"      /* square root symbol */
        //#define UTF8_PLUSMINUS          "\xc2\xb1"          /* plusminus symbol */

        //#define UTF8_POW_2              "\xc2\xb2"          /* superscript 2 */
        //#define UTF8_POW_X              "\xcb\xa3"          /* superscript x */
        //#define UTF8_POW_Y              "\xca\xb8"          /* superscript y */
        //#define UTF8_PRIME              "\xca\xb9"          /* prime symbol */
        //#define UTF8_DEGREES            "\xc2\xb0"          /* degrees symbol */

        //#define UTF8_SMALL_PI           "\xcf\x80"          /* Greek small letter pi */
        //#define UTF8_CAPITAL_SIGMA      "\xce\xa3"          /* Greek capital letter sigma */
        //#define UTF8_CAPITAL_DELTA      "\xce\x94"          /* Greek capital letter delta */

        //#define UTF8_MACRON             "\xc2\xaf"          /* macron symbol */
        //#define UTF8_NONSPACE_MACRON    "\xcc\x84"          /* nonspace macron, use after another char */

        //#define a_RING                  "\xc3\xa5"          /* small a with a ring */
        //#define a_UMLAUT                "\xc3\xa4"          /* small a with an umlaut */
        //#define o_UMLAUT                "\xc3\xb6"          /* small o with an umlaut */
        //#define u_UMLAUT                "\xc3\xbc"          /* small u with an umlaut */
        //#define e_ACUTE                 "\xc3\xa9"          /* small e with an acute */
        //#define n_TILDE                 "\xc3\xb1"          /* small n with a tilde */

        //#define A_RING                  "\xc3\x85"          /* capital A with a ring */
        //#define A_UMLAUT                "\xc3\x84"          /* capital A with an umlaut */
        //#define O_UMLAUT                "\xc3\x96"          /* capital O with an umlaut */
        //#define U_UMLAUT                "\xc3\x9c"          /* capital U with an umlaut */
        //#define E_ACUTE                 "\xc3\x89"          /* capital E with an acute */
        //#define N_TILDE                 "\xc3\x91"          /* capital N with a tilde */

        //#define UTF8_LEFT               "\xe2\x86\x90"      /* cursor left */
        //#define UTF8_RIGHT              "\xe2\x86\x92"      /* cursor right */
        //#define UTF8_UP                 "\xe2\x86\x91"      /* cursor up */
        //#define UTF8_DOWN               "\xe2\x86\x93"      /* cursor down */


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
        public static Tuple<object, string> NAME<T>(T x)
        {
            // https://stackoverflow.com/a/32314158

            // TODO - check to make sure args is in the correct format, eg NAME(new { blah });

            var param = typeof(T).GetProperties()[0];
            var name = param.Name;
            var value = param.GetValue(x, null);
            return new Tuple<object, string>(value, name);
        }

        // this macro wraps a function 'x' and can be used to pass a function followed by its name
        //#define FUNC(x) &x, #x


        // macros to convert radians to degrees and degrees to radians
        //template <typename T> constexpr auto RADIAN_TO_DEGREE(T const &x) { return (180.0 / M_PI) * x; }
        //template <typename T> constexpr auto DEGREE_TO_RADIAN(T const &x) { return (M_PI / 180.0) * x; }
    }


    //**************************************************************************
    //  EXCEPTION CLASSES
    //**************************************************************************

    // emu_exception is the base class for all emu-related exceptions
    public class emu_exception : Exception
    {
    }


    // emu_fatalerror is a generic fatal exception that provides an error string
    public class emu_fatalerror : emu_exception
    {
        string text;
        int code;


        [DebuggerHidden]
        public emu_fatalerror(string format, params object [] args)
            : this(0, format, args)
        {
        }

        [DebuggerHidden]
        public emu_fatalerror(int _exitcode, string format, params object [] args)
        {
            code = _exitcode;
            text = util.string_format(format, args);

            string error = string.Format("emu_fatalerror: {0}code: {1}\n", text, code);
            m_osdcore.osd_break_into_debugger(error);
        }


        public string what() { return text; }
        public int exitcode() { return code; }
    }


    public class emu_unimplemented : emu_fatalerror
    {
        [DebuggerHidden]
        public emu_unimplemented() : base("Unimplemented") { }
    }


    class tag_add_exception : emu_exception
    {
        string m_tag;

        public tag_add_exception(string tag) : base() { m_tag = tag; }
        string tag() { return m_tag; }
    }


    //**************************************************************************
    //  CASTING TEMPLATES
    //**************************************************************************

    //[[noreturn]] void report_bad_cast(const std::type_info &src_type, const std::type_info &dst_type);
    //[[noreturn]] void report_bad_device_cast(const device_t *dev, const std::type_info &src_type, const std::type_info &dst_type);

    //template <typename Dest, typename Source>
    //inline void report_bad_cast(Source *src)

    // template function for casting from a base class to a derived class that is checked
    // in debug builds and fast in release builds
    //template <typename Dest, typename Source>
    //inline Dest downcast(Source *src)

    //template<class Dest, class Source>
    //inline Dest downcast(Source &src)


    public static partial class emucore_global
    {
        [DebuggerHidden]
        public static void fatalerror(string format, params object [] args)
        {
            throw new emu_fatalerror(format, args);
        }


        // convert a series of 32 bits into a float
        //inline float u2f(u32 v)

        // convert a float into a series of 32 bits
        //inline u32 f2u(float f)

        // convert a series of 64 bits into a double
        //inline double u2d(u64 v)

        // convert a double into a series of 64 bits
        //inline u64 d2u(double d)
    }
}
