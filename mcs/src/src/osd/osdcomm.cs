// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int16_t = System.Int16;
using int32_t = System.Int32;
using int64_t = System.Int64;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame
{
    public static class osdcomm_global
    {
        static uint32_t swapendian_int32_partial16(uint32_t val) { return ((val << 8) & 0xFF00FF00U) | ((val >> 8) & 0x00FF00FFU); }
        public static uint32_t swapendian_int32(uint32_t val) { return (swapendian_int32_partial16(val) << 16) | (swapendian_int32_partial16(val) >> 16); }


        //#ifdef LSB_FIRST
        //constexpr uint16_t big_endianize_int16(uint16_t x) { return flipendian_int16(x); }
        //constexpr uint32_t big_endianize_int32(uint32_t x) { return flipendian_int32(x); }
        //constexpr uint64_t big_endianize_int64(uint64_t x) { return flipendian_int64(x); }
        public static int16_t little_endianize_int16(int16_t x) { return x; }
        public static uint16_t little_endianize_int16(uint16_t x) { return x; }
        public static int32_t little_endianize_int32(int32_t x) { return x; }
        public static uint32_t little_endianize_int32(uint32_t x) { return x; }
        public static int64_t little_endianize_int64(int64_t x) { return x; }
        public static uint64_t little_endianize_int64(uint64_t x) { return x; }
        //#else
        //constexpr uint16_t big_endianize_int16(uint16_t x) { return x; }
        //constexpr uint32_t big_endianize_int32(uint32_t x) { return x; }
        //constexpr uint64_t big_endianize_int64(uint64_t x) { return x; }
        //constexpr uint16_t little_endianize_int16(uint16_t x) { return flipendian_int16(x); }
        //constexpr uint32_t little_endianize_int32(uint32_t x) { return flipendian_int32(x); }
        //constexpr uint64_t little_endianize_int64(uint64_t x) { return flipendian_int64(x); }
        //#endif /* LSB_FIRST */
    }
}
