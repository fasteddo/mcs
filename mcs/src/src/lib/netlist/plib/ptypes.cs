// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using unsigned = System.UInt32;
using uint32_t = System.UInt32;
using uint_fast64_t = System.UInt64;


namespace mame.plib
{
    // noexcept on move operator -> issue with macosx clang
    //#define PCOPYASSIGNMOVE(name, def) \
    //    PCOPYASSIGN(name, def) \
    //    PMOVEASSIGN(name, def)

    //#define PCOPYASSIGN(name, def)  \
    //    name(const name &) = def; \
    //    name &operator=(const name &) = def;

    //#define PMOVEASSIGN(name, def)  \
    //    name(name &&) /*noexcept*/ = def; \
    //    name &operator=(name &&) /*noexcept*/ = def;

    //#if defined(EMSCRIPTEN)
    //#undef EMSCRIPTEN
    //#endif

    //============================================================
    //  compile time information
    //============================================================

    /// \brief Dummy 128 bit types for platforms which don't support 128 bit
    ///
    /// Users should always consult compile_info::has_int128 prior to
    /// using the UINT128/INT128 data type.
    ///
    //struct UINT128_DUMMY {};
    //struct INT128_DUMMY {};

    //enum class ci_compiler
    //{
    //    UNKNOWN,
    //    CLANG,
    //    GCC,
    //    MSC
    //};

    //enum class ci_os
    //{
    //    UNKNOWN,
    //    LINUX,
    //    FREEBSD,
    //    OPENBSD,
    //    WINDOWS,
    //    MACOSX,
    //    EMSCRIPTEN
    //};

    //enum class ci_arch
    //{
    //    UNKNOWN,
    //    X86,
    //    ARM,
    //    MIPS
    //};

    //struct compile_info
    //{
        //using win32 = std::integral_constant<bool, true>;
        //using unicode = std::integral_constant<bool, true>;
        //using has_int128 = std::integral_constant<bool, true>;
        //using int128_type  = __int128_t;
        //using uint128_type = __uint128_t;
        //static constexpr int128_type  int128_max() { return (~static_cast<uint128_type>(0)) >> 1; }
        //static constexpr uint128_type  uint128_max() { return ~(static_cast<uint128_type>(0)); }
        //using type = std::integral_constant<ci_compiler, ci_compiler::MSC>;
        //using version = std::integral_constant<int, _MSC_VER>;
        //using is_unix = std::integral_constant<bool, false>;
        //using os = std::integral_constant<ci_os, ci_os::WINDOWS>;
        //using m64 = std::integral_constant<bool, true>;
        //using arch = std::integral_constant<ci_arch, ci_arch::X86>;
        //using mingw = std::integral_constant<bool, false>;
    //};


    //using INT128 = plib::compile_info::int128_type;
    //using UINT128 = plib::compile_info::uint128_type;


    //template<typename T> struct is_integral : public std::is_integral<T> { };
    //template<typename T> struct is_signed : public std::is_signed<T> { };
    //template<typename T> struct is_unsigned : public std::is_unsigned<T> { };
    //template<typename T> struct numeric_limits : public std::numeric_limits<T> { };

    // 128 bit support at least on GCC is not fully supported

    //template<> struct is_integral<UINT128> { static constexpr bool value = true; };
    //template<> struct is_integral<INT128> { static constexpr bool value = true; };

    //template<> struct is_signed<UINT128> { static constexpr bool value = false; };
    //template<> struct is_signed<INT128> { static constexpr bool value = true; };

    //template<> struct is_unsigned<UINT128> { static constexpr bool value = true; };
    //template<> struct is_unsigned<INT128> { static constexpr bool value = false; };

    //template<> struct numeric_limits<UINT128>
    //{
    //    static constexpr UINT128 max() noexcept
    //    {
    //        return compile_info::uint128_max();
    //    }
    //};
    //template<> struct numeric_limits<INT128>
    //{
    //    static constexpr INT128 max() noexcept
    //    {
    //        return compile_info::int128_max();
    //    }
    //};

    //template<typename T> struct is_floating_point : public std::is_floating_point<T> { };

    //template< class T >
    //struct is_arithmetic : std::integral_constant<bool,
    //    plib::is_integral<T>::value || plib::is_floating_point<T>::value> {};


    static class ptypes_global
    {
        //template<unsigned bits>
        //struct size_for_bits
        //{
        //    static_assert(bits <= 64 || (bits <= 128 && compile_info::has_int128::value), "not supported");
        //    enum { value =
        //        bits <= 8       ?   1 :
        //        bits <= 16      ?   2 :
        //        bits <= 32      ?   4 :
        //        bits <= 64      ?   8 :
        //                            16
        //    };
        //};
        public static unsigned size_for_bits(unsigned bits)
        {
            //static_assert(bits <= 64 || (bits <= 128 && compile_info::has_int128::value), "not supported");
            return bits <= 8       ?   1U :
                   bits <= 16      ?   2U :
                   bits <= 32      ?   4U :
                   bits <= 64      ?   8U :
                                      16U;
        }

        //template<unsigned N> struct least_type_for_size;
        //template<> struct least_type_for_size<1> { using type = uint_least8_t; };
        //template<> struct least_type_for_size<2> { using type = uint_least16_t; };
        //template<> struct least_type_for_size<4> { using type = uint_least32_t; };
        //template<> struct least_type_for_size<8> { using type = uint_least64_t; };
        //template<> struct least_type_for_size<16> { using type = UINT128; };

        // This is different to the standard library. Mappings provided in stdint
        // are not always fastest.
        //template<unsigned N> struct fast_type_for_size;
        //template<> struct fast_type_for_size<1> { using type = uint32_t; };
        //template<> struct fast_type_for_size<2> { using type = uint32_t; };
        //template<> struct fast_type_for_size<4> { using type = uint32_t; };
        //template<> struct fast_type_for_size<8> { using type = uint_fast64_t; };
        //template<> struct fast_type_for_size<16> { using type = UINT128; };
        public static Type fast_type_for_size(unsigned N)
        {
            switch (N)
            {
                case 1: return typeof(uint32_t);
                case 2: return typeof(uint32_t);
                case 4: return typeof(uint32_t);
                case 8: return typeof(uint_fast64_t);
                default: throw new emu_unimplemented();
            }
        }

        //template<unsigned bits>
        //struct least_type_for_bits
        //{
        //    static_assert(bits <= 64 || (bits <= 128 && compile_info::has_int128::value), "not supported");
        //    using type = typename least_type_for_size<size_for_bits<bits>::value>::type;
        //};

        //template<unsigned bits>
        //struct fast_type_for_bits
        //{
        //    static_assert(bits <= 64 || (bits <= 128 && compile_info::has_int128::value), "not supported");
        //    using type = typename fast_type_for_size<size_for_bits<bits>::value>::type;
        //};
        public static Type fast_type_for_bits(unsigned bits)
        {
            return fast_type_for_size(size_for_bits(bits));
        }
    }


    /// \brief mark arguments as not used for compiler
    ///
    /// @tparam Ts unsused parameters
    ///
    //template<typename... Ts>
    //inline void unused_var(Ts&&...) noexcept {} // NOLINT(readability-named-parameter)

    /// \brief copy type S to type D byte by byte
    ///
    /// The purpose of this copy function is to suppress compiler warnings.
    /// Use at your own risk. This is dangerous.
    ///
    /// \param s Source object
    /// \param d Destination object
    /// \tparam S Type of source object
    /// \tparam D Type of destination object
    //template <typename S, typename D>
    //void reinterpret_copy(S &s, D &d)
    //{
    //    static_assert(sizeof(D) >= sizeof(S), "size mismatch");
    //    // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
    //    auto *dp = reinterpret_cast<std::uint8_t *>(&d);
    //    // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
    //    const auto *sp = reinterpret_cast<std::uint8_t *>(&s);
    //    std::copy(sp, sp + sizeof(S), dp);
    //}


    ////============================================================
    //// Define a "has member" trait.
    ////============================================================
    //
    //#define PDEFINE_HAS_MEMBER(name, member)                                        \
    //    template <typename T> class name                                            \
    //    {                                                                           \
    //        template <typename U> static long test(decltype(&U:: member));          \
    //        template <typename U> static char test(...);                            \
    //    public:                                                                     \
    //        static constexpr const bool value = sizeof(test<T>(nullptr)) == sizeof(long);   \
    //    }
}
