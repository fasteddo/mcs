// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    public static partial class util
    {
        //**************************************************************************
        //  TYPE DEFINITIONS
        //**************************************************************************

        // constants for expression endianness
        public enum endianness
        {
            little,
            big,
//#ifdef LSB_FIRST
            native = little
//#else
//        native = big
//#endif
        }


        //**************************************************************************
        //  MACROS AND INLINE FUNCTIONS
        //**************************************************************************

        public static string endian_to_string_view(endianness e) { return e == endianness.little ? "little" : "big"; }


        // endian-based value: first value is if native endianness is little-endian, second is if native is big-endian
        //#define NATIVE_ENDIAN_VALUE_LE_BE(leval,beval)  ((util::endianness::native == util::endianness::little) ? (leval) : (beval))


        // inline functions for accessing bytes and words within larger chunks

        // read/write a byte to a 16-bit space
        //template <typename T> constexpr T BYTE_XOR_BE(T a) { return a ^ NATIVE_ENDIAN_VALUE_LE_BE(1,0); }
        //template <typename T> constexpr T BYTE_XOR_LE(T a) { return a ^ NATIVE_ENDIAN_VALUE_LE_BE(0,1); }

        // read/write a byte to a 32-bit space
        //template <typename T> constexpr T BYTE4_XOR_BE(T a) { return a ^ NATIVE_ENDIAN_VALUE_LE_BE(3,0); }
        //template <typename T> constexpr T BYTE4_XOR_LE(T a) { return a ^ NATIVE_ENDIAN_VALUE_LE_BE(0,3); }

        // read/write a word to a 32-bit space
        //template <typename T> constexpr T WORD_XOR_BE(T a) { return a ^ NATIVE_ENDIAN_VALUE_LE_BE(2,0); }
        //template <typename T> constexpr T WORD_XOR_LE(T a) { return a ^ NATIVE_ENDIAN_VALUE_LE_BE(0,2); }

        // read/write a byte to a 64-bit space
        //template <typename T> constexpr T BYTE8_XOR_BE(T a) { return a ^ NATIVE_ENDIAN_VALUE_LE_BE(7,0); }
        //template <typename T> constexpr T BYTE8_XOR_LE(T a) { return a ^ NATIVE_ENDIAN_VALUE_LE_BE(0,7); }

        // read/write a word to a 64-bit space
        //template <typename T> constexpr T WORD2_XOR_BE(T a) { return a ^ NATIVE_ENDIAN_VALUE_LE_BE(6,0); }
        //template <typename T> constexpr T WORD2_XOR_LE(T a) { return a ^ NATIVE_ENDIAN_VALUE_LE_BE(0,6); }

        // read/write a dword to a 64-bit space
        //template <typename T> constexpr T DWORD_XOR_BE(T a) { return a ^ NATIVE_ENDIAN_VALUE_LE_BE(4,0); }
        //template <typename T> constexpr T DWORD_XOR_LE(T a) { return a ^ NATIVE_ENDIAN_VALUE_LE_BE(0,4); }
    }
}
